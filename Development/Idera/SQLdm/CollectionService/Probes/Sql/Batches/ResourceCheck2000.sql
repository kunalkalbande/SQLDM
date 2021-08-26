--------------------------------------------------------------------------------
--  Batch: Resource Check 2000
--  Tables: master..sysprocesses, 
--  Temp Tables: tempdb..Resource_Check
--  Variables: 
--		[0] - Process CPU Alerting Value
--		[1] - Filter statements for where clause
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
		cpu_time bigint)

insert into @CPU_Processes
	select 
		spid, 
		login_time,
		sum(cast(cpu as bigint))
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

	if (select isnull(object_id('tempdb..#inputbuffer_resource_check'), 0)) = 0 
	begin 
		create table #inputbuffer_resource_check (spid smallint default -1, EventType nvarchar(260), Parameters int,EventInfo nvarchar(260)) 
	end

	declare extract_too_much_cpu insensitive cursor for
		Select 
			spid
		From  @CPU_Processes p
		Where 
			not exists (
			select * 
			from 
				tempdb..Resource_Check m 
			where 
				p.spid = m.spid 
				and p.login_time = m.login_time
				and collectionservice = host_name() 
				and p.cpu_time < m.cpu_time + {0}) 
	for read only 
	Open extract_too_much_cpu 
	fetch next from extract_too_much_cpu into @spid
	while @@fetch_status = 0 
	begin 
		if exists(select spid from sysprocesses where spid = @spid)
		begin
			set @command = 'dbcc inputbuffer(' + convert(nvarchar(5),@spid) + ')'
			insert into #inputbuffer_resource_check (EventType,Parameters,EventInfo)
				exec (@command)
			update #inputbuffer_resource_check set spid = @spid where spid = -1
		end

		fetch next from extract_too_much_cpu into @spid
	End 
	Close extract_too_much_cpu 
	deallocate extract_too_much_cpu 

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
		EventInfo
	 from
	   #sysprocessescopy a (nolock)
	   right join #inputbuffer_resource_check i
	   on a.spid = i.spid
	 where
	   1 = 1
	   {1}	

	truncate table #inputbuffer_resource_check

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