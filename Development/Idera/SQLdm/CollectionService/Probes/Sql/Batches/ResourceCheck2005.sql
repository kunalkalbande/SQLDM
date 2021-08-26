--------------------------------------------------------------------------------
--  Batch: Resource Check 2005
--  Tables: master..sysprocesses, 
--  Temp Tables: tempdb..tempdb..Resource_Check
--  Variables: 
--		[0] - Process CPU Alerting Value
--		[1] - Filter statements for where clause
--		[2] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
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
if (select isnull(object_id('tempdb..Resource_Check'), 0)) = 0 
begin 
	Create table tempdb..Resource_Check (spid int, login_time datetime, cpu_time bigint, collectionservice nvarchar(512)) 
end 

select 
ident = identity(bigint),
*
into #sysprocessescopy
from 
master..sysprocesses
order by spid

delete from p
	from #sysprocessescopy p, #sysprocessescopy p2
	where p.spid = p2.spid
	and p.ident > p2.ident

if not exists(select * from tempdb..Resource_Check where collectionservice = host_name())
	insert into tempdb..Resource_Check 
		select spid, login_time, cpu, host_name() from #sysprocessescopy where cpu > {0} 

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

insert into @CPU_Processes
	select 
		spid, 
		login_time,
		sum(cast(cpu as bigint)),
		null
	from 
		#sysprocessescopy
	where 
		program_name <> '' 
		and program_name not like 'DiagnosticMan%' 
		and program_name not like 'SQL diagnostic manager%'
		and program_name <> 'SQL PerfMon' 
	group by 
		spid, 
		program_name, 
		login_time 
	having sum(cast(cpu as bigint)) > {0}

if @@rowcount > 0 
begin

	declare @resourcecheckspid smallint, @resourcecheckcommand nvarchar(255)
	declare @ResourceCheckInputbuffer table (EventType nvarchar(260), Parameters int,EventInfo nvarchar(4000)) 
	declare read_inputbuffer_resourcecheck insensitive cursor 
	for
		select spid
		from @CPU_Processes c
		where
			not exists (
				select * 
				from 
					tempdb..Resource_Check m 
				where 
					c.spid = m.spid 
					and c.login_time = m.login_time
					and collectionservice = host_name() )
	for read only
	set nocount on 
	open read_inputbuffer_resourcecheck 
	fetch read_inputbuffer_resourcecheck into @resourcecheckspid
	while @@fetch_status = 0 
	begin 

		if exists(select spid from sysprocesses where spid = @resourcecheckspid)
		begin
			set @resourcecheckcommand = 'dbcc inputbuffer(' + convert(nvarchar(5),@resourcecheckspid) + ')'
			insert into @ResourceCheckInputbuffer (EventType,Parameters,EventInfo)
				exec (@resourcecheckcommand)
			update @CPU_Processes 
				set commandtext = EventInfo from @ResourceCheckInputbuffer
				where spid = @resourcecheckspid
		end

		delete from @ResourceCheckInputbuffer

	fetch read_inputbuffer_resourcecheck into @resourcecheckspid
	end
	close read_inputbuffer_resourcecheck 
	deallocate read_inputbuffer_resourcecheck 




	select
		cast(a.spid as int),
		rtrim(convert(varchar(255),a.loginame)),
		rtrim(a.hostname),
		rtrim(a.status),
		rtrim(a.program_name),
		rtrim(a.cmd),
		rtrim(db_name(a.dbid)),
		cast(a.cpu as bigint),
		a.memusage,
		a.physical_io,
		cast(a.blocked as int),
		(select count(b.spid) from #sysprocessescopy b where b.blocked = a.spid),
		dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time),
		case
		 when a.spid < 5
		   then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate())
		 else
			dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch)
		end ,
		open_tran,
		rtrim(net_address),
		rtrim(net_library),
		convert(bigint,waittime),
		a.ecid,
		rtrim(a.lastwaittype),
		rtrim(a.waitresource),
		{2}
	 from
	   #sysprocessescopy a (nolock)
	   right join @CPU_Processes c on a.spid = c.spid
	 where 
		not exists (
			select * 
			from 
				tempdb..Resource_Check m 
			where 
				c.spid = m.spid 
				and c.login_time = m.login_time
				and collectionservice = host_name() )
		{1}

	delete from 
		tempdb..Resource_Check 
	where
		collectionservice = host_name()

	insert into tempdb..Resource_Check 
		select 
			spid, 
			login_time,
			cpu_time,
			host_name() 
		from @CPU_Processes 
end 
else 
	begin
	truncate table tempdb..Resource_Check
	select 'No resource check results'
	end

drop table #sysprocessescopy