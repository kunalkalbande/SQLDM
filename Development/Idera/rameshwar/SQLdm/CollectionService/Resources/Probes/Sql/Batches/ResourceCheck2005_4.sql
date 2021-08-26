--------------------------------------------------------------------------------
--  Batch: Resource Check 2005
--  Tables: master..sysprocesses, 
--  Temp Tables: tempdb..tempdb..Resource_Check
--  Variables: 
--		[0] - Process CPU Alerting Value
--		[1] - Filter statements for where clause
--		[2] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
--		[3] - filter SQLdm app
--		80: commandtext
--		90: case when isnull(net_address, '') = '' then commandtext else (select case when dmv.objectid is null then left(max(text),4000) else commandtext end from sys.dm_exec_sql_text(a.sql_handle) dmv group by dmv.objectid) end
--------------------------------------------------------------------------------
set nocount on

--------------------------------------------------------------------------------
--  TEMP TABLE: Resource_Check
--  Created Tables: tempdb..Resource_Check
--  Tables: master..sysprocesses
--  Purpose: Track processes which may be exceeding the CPU threshold
--   This table is used to help calculate the delta of the CPU
--------------------------------------------------------------------------------
if (select isnull(object_id('tempdb..#Resource_Check'), 0)) = 0 
begin 
	Create table tempdb..#Resource_Check (spid int, login_time datetime, cpu_time bigint, collectionservice nvarchar(512)) 
end;

if not exists(select * from tempdb..#Resource_Check where collectionservice = host_name())
begin
WITH numbered AS (select ROW_NUMBER() over (Partition by spid order by spid) as nr, * from master..sysprocesses p),
nodups AS (select * from numbered where nr = 1)
	insert into tempdb..#Resource_Check 
		select spid, login_time, cpu, host_name() from nodups where cpu > {0} 
end
--------------------------------------------------------------------------------
--  Table Variable: CPU Processes
--  Tables: none
--  Purpose: Track CPU usage per spid
--------------------------------------------------------------------------------
declare @CPU_Processes table(
		spid int, 
		login_time datetime,
		cpu_time bigint,
		commandtext nvarchar(4000))
		
;WITH numbered AS (select ROW_NUMBER() over (Partition by spid order by spid) as nr, * from master..sysprocesses p),
nodups AS (select * from numbered where nr = 1)
insert into @CPU_Processes
	select 
		spid, 
		login_time,
		sum(cast(cpu as bigint)),
		null
	from 
		nodups
	where 
		program_name <> ''
		{3}
		and program_name <> 'SQL PerfMon' 
	group by 
		spid, 
		program_name, 
		login_time 
	having sum(cast(cpu as bigint)) > {0}

declare @DBCCBuffer table (
	EventType nvarchar(260), 
	Parameters int,
	EventInfo nvarchar(4000),
	BufferID int identity(1, 1) not null primary key
) 

if @@rowcount > 0 
begin
	Declare @SQL nvarchar(max)

	-- Create a string query to run dbcc inputbuffer for all spids
	Set @SQL = (
		SELECT  N'Select ''' + Cast(spid as nvarchar) + ''', '''', ''''; begin try dbcc inputbuffer(' + Cast(spid as nvarchar) + N') with no_infomsgs end try begin catch select ''RPC Event'',0,''(unknown)'' end catch; '
		FROM @CPU_Processes c
		WHERE not exists (
					select * 
					from 
						tempdb..#Resource_Check m 
					where 
						c.spid = m.spid 
						and c.login_time = m.login_time
						and collectionservice = host_name() 
						and c.cpu_time < m.cpu_time + {0})
		FOR XML PATH(''))
	
	-- Execute the string 1 time
	Insert Into @DBCCBuffer (EventType, Parameters, EventInfo)
	exec sp_executesql @SQL;

	With BufferInfo
	As (Select Cast(DB1.EventType as int) As SPID,
			DB2.EventInfo
		From @DBCCBuffer DB1
		Inner Join @DBCCBuffer DB2 On DB2.BufferID = DB1.BufferID + 1
		And IsNumeric(DB2.EventType) = 0 And IsNumeric(DB1.EventType) = 1)
	Update p
	Set commandtext =  B.EventInfo
	From @CPU_Processes p
	Inner Join BufferInfo B On p.spid = B.SPID;


WITH numbered AS (select ROW_NUMBER() over (Partition by spid order by spid) as nr, * from master..sysprocesses p),
nodups AS (select * from numbered where nr = 1)
	select
		cast(a.spid as int),
		rtrim(convert(varchar(255),a.loginame)),
		rtrim(a.hostname),
		rtrim(a.status) as status,
		rtrim(a.program_name) as program_name,
		rtrim(a.cmd) as command,
		rtrim(db_name(a.dbid)) as dbname,
		cast(a.cpu as bigint) as cpu,
		a.memusage,
		a.physical_io,
		cast(a.blocked as int),
		(select count(b.spid) from nodups b where b.blocked = a.spid),
		dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time),
		case
		 when a.spid < 5
		   then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate())
		 else
			dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch)
		end ,
		open_tran,
		rtrim(net_address) as net_address,
		rtrim(net_library) as net_library,
		convert(bigint,waittime) as waittime,
		a.ecid,
		rtrim(a.lastwaittype),
		rtrim(a.waitresource),
		{2}
	 from
	   nodups a (nolock)
	   right join @CPU_Processes c on a.spid = c.spid
	 where commandtext <> '(unknown)' 
		and not exists (
			select * 
			from 
				tempdb..#Resource_Check m 
			where 
				c.spid = m.spid 
				and c.login_time = m.login_time
				and collectionservice = host_name() 
				and c.cpu_time < m.cpu_time + {0})
		{1}

	delete from 
		tempdb..#Resource_Check 
	where
		collectionservice = host_name()

	insert into tempdb..#Resource_Check 
		select 
			spid, 
			login_time,
			cpu_time,
			host_name() 
		from @CPU_Processes 
end 
else 
	begin
	truncate table tempdb..#Resource_Check
	select 'No resource check results'
	end

