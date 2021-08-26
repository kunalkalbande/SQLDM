--------------------------------------------------------------------------------
--  Batch: Server Overview 2005
--  Tables: 
--  XSP:	
--	Variables:	
--  [0] - sp_OACreate context
--  [1] - Session Count Segment
--	[2] - Lock Counter Statistics Segment
--	[3] - Disable OLE
--------------------------------------------------------------------------------\
set nocount on
--------------------------------------------------------------------------------
--  XP: Product version, Language, Processor Count and Physical Memory
--  Returns:
--		Index 
--		Name 
--		Internal_Value 
--		Character_Value  
--------------------------------------------------------------------------------
exec master..xp_msver 

exec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'SOFTWARE\MICROSOFT\Windows NT\CurrentVersion', N'ProductName' 

select serverproperty('MachineName')
select serverproperty('Edition')
select @@version

--------------------------------------------------------------------------------
--  Query: Server Date/Time
--  Returns: The date and time as set on the target server
--------------------------------------------------------------------------------
select getutcdate()

-------------------------------------------------------------------------------
--  QUERY: Affinity Mask
--  Tables: syscurconfigs
--  Returns:
--    Value
--  Criteria: 
--  (1) Returns value for the following configuration option:
--			1535 Affinity Mask (Bit mask - default 0, which is none)
-------------------------------------------------------------------------------
Select 
	cast(cast(value as varbinary) as bigint)
from 
	master..syscurconfigs 
where config = 1535 

--------------------------------------------------------------------------------
--  QUERY: Max user connections
--  Tables: master..syscurconfigs 
--  Returns:
--    Max user connections from syscurconfigs
--    Max user connections from @@max_connections
--  Notes:
--  (1) Use the first if non-zero, else the second
--------------------------------------------------------------------------------
Select 
	value, 
	@@max_connections 
from 
	master..syscurconfigs 
where config = 103 

--------------------------------------------------------------------------------
--  QUERY: Throughput
--------------------------------------------------------------------------------
set arithabort on

declare @timestamp bigint

select @timestamp = cpu_ticks / (cpu_ticks/ms_ticks) from sys.dm_os_sys_info
;with SystemHealthRingBuffer(record,timestamp)
as
(
select top 30
record = convert(xml,record),
@timestamp - timestamp
from sys.dm_os_ring_buffers
where ring_buffer_type = 'RING_BUFFER_SCHEDULER_MONITOR'
and record like '%<SystemHealth>%'
order by timestamp desc
)
select 
	dateadd(ms, timestamp * -1, getutcdate()),
	SqlCpu = record.value('(./Record/SchedulerMonitorEvent/SystemHealth/ProcessUtilization)[1]', 'int')
from SystemHealthRingBuffer

select 
	@@cpu_busy, 
	@@idle, 
	@@io_busy, 
	@@pack_received, 
	@@pack_sent, 
	@@packet_errors, 
	@@total_read, 
	@@total_write, 
	@@total_errors, 
	@@connections,
	@@timeticks

--------------------------------------------------------------------------------
--  Query: Procedure Cache
--  DBCC: ProcCache
--  Returns:
--		num proc buffs - Total number of pages used by all entries in the 
--			procedure cache.
--		num proc buffs used - Total number of pages used by all entries that 
--			are currently being used.
--		num proc buffs active - For backward compatibility only. Total number of 
--			pages used by all entries that are currently being used.
--		proc cache size - Total number of entries in the procedure cache.
--		proc cache used -  Total number of entries that are currently being used.
--		proc cache active -  For backward compatibility only. Total number of 
--			entries that are currently being used.
--------------------------------------------------------------------------------
dbcc proccache 

--------------------------------------------------------------------------------
--  QUERY: IsClustered
--  Returns:
--		"isclustered"
--		Physical computer (node) name
--------------------------------------------------------------------------------
select serverproperty('isclustered') , ServerProperty('ComputerNamePhysicalNetBIOS')
exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'

declare 
	@servername varchar(255), 
	@sysperfinfoname varchar(255), 
	@slashpos int

select @servername = cast(serverproperty('servername') as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 
		select @sysperfinfoname = 'SQLSERVER'
	end 


declare @status int 

--------------------------------------------------------------------------------
--  QUERY: Total Locks
--  Tables: master..sysperfinfo
--  Returns: Lock Count
--------------------------------------------------------------------------------
select
  case
    when
      cntr_value >= 0
    then
      convert(dec(20,0),cntr_value)
    else
      convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1
  end
from
  master..sysperfinfo (nolock)
where
  lower(object_name) = lower(@sysperfinfoname + ':locks')
  and lower(instance_name) = '_total'
  and lower(counter_name) = lower('lock requests/sec')

--------------------------------------------------------------------------------
--  XP: Login mode
--  Returns:  Login Mode
--------------------------------------------------------------------------------
exec @status = master..xp_loginconfig 'login mode' 

if @status <> 0 
	select 'cannot find login mode' 
	
--------------------------------------------------------------------------------
--  XP: Audit Level
--  Returns:  Audit Level
--------------------------------------------------------------------------------
exec @status = master..xp_loginconfig 'audit level' 

if @status <> 0 
	select 'cannot find audit level' 

{1}

{2}

--------------------------------------------------------------------------------
--  QUERY: Performance Counters
--  Tables: master..sysperfinfo
--  Returns:
--		batch requests/sec
--		buffer cache hit ratio
--		buffer cache hit ratio base
--		cache hit ratio
--		cache hit ratio base
--		checkpoint pages/sec
--		full scans/sec
--		lazy writes/sec
--		lock waits/sec
--		log flushes/sec
--		page life expectancy
--		page lookups/sec
--		page reads/sec
--		page splits/sec
--		page writes/sec
--		readahead pages/sec
--		sql compilations/sec
--		sql re-compilations/sec
--		table lock escalations/sec
--		target server memory(kb)
--		total server memory (kb)
--		workfiles created/sec
--		worktables created/sec
--------------------------------------------------------------------------------
select 
	lower(RTRIM(counter_name)), 
	isnull(sum(convert(bigint,cntr_value)),0)
from 
	master.sys.dm_os_performance_counters 
where 
	(lower(object_name) = lower(@sysperfinfoname + ':SQL Statistics')
		and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
	or (Lower(object_name) = Lower(@sysperfinfoname + ':Buffer Manager') 
		and (Lower(counter_name) like 'buffer cache hit ratio%'
		or Lower(counter_name) = 'total pages')) 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':plan cache') 
		and Lower(counter_name) like 'cache hit ratio%' 
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':databases') 
		and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':memory manager') 
		and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':access methods') 
		and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':locks') 
		and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname + ':buffer manager') 
		and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
group by 
	counter_name



declare 
	@command nvarchar(2048), 
	@dbname nvarchar(255), 
	@mode smallint

if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
	create table #totaldbsizes (usedsize dec(15,0)) 
else 
	truncate table #totaldbsizes 


declare read_db_sizes insensitive cursor 
for 
	select 
		name, 
		mode
	from 
		master..sysdatabases d (nolock) 
	where
		lower(name) <> 'mssqlsystemresource'
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databaseproperty(name, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(name, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(name, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(name, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(name, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(name, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = d.dbid and l.request_session_id <> @@spid)
				)
			)
		and status & 32 <> 32 
		and status & 64 <> 64 
		and status & 128 <> 128 
		and status & 256 <> 256 
		and status & 512 <> 512 
for read only
set nocount on 
open read_db_sizes 
fetch read_db_sizes into @dbname, @mode
while @@fetch_status = 0 
begin 
	-- Double check to make sure the status hasn't changed
	if has_dbaccess (@dbname) = 1 
		and isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = db_id(@dbname) and l.request_session_id <> @@spid)
				)
			)
	begin
		select @command = 'insert into #totaldbsizes select sum(convert(dec(15,0),used_pages))*8 from [' 
		+ replace(@dbname,char(93),char(93)+char(93)) 
		+ '].sys.allocation_units'

		execute(@command)
	end
	
	fetch read_db_sizes into @dbname, @mode
End 
Close read_db_sizes 
deallocate read_db_sizes 


declare @templogsize dec(12,0), @tempdevsize dec(12,0)

select @templogsize = (cast(size as dec(12,0)) * 8) 
from tempdb.sys.database_files
where type = 1

select @tempdevsize = (cast(size as dec) * 8)
from tempdb.sys.database_files
where type = 0

----------------------------------------------------------------------------------
---- QUERY: Database summary information
---- Tables: master..sysdatabases, tempdb..#totaldbsizes
---- Returns:
---- Total number of databases
---- Number of data files
---- Number of log files
---- Total data file space allocated
---- Total log file space allocated
---- Total data file space used
----------------------------------------------------------------------------------
select
'summary',
(select
count(*)
from
master..sysdatabases)
,sum(case when type = 0 then 1 else 0 end )
,sum(case when type = 1 then 1 else 0 end )
,sum(case
when type = 0 and database_id <> db_id('tempdb')
then isnull(convert(dec(12,0),size) * 8,0)
when type = 0 and database_id = db_id('tempdb')
then @tempdevsize
else 0 end )
,sum(case
when type = 1 and database_id <> db_id('tempdb')
then isnull(convert(dec(12,0),size) * 8,0)
when type = 1 and database_id = db_id('tempdb')
then @templogsize
else 0 end )
,(select sum(usedsize) from #totaldbsizes)
from master.sys.master_files
drop table #totaldbsizes


DBCC SQLPERF (LOGSPACE) 


select @servername = cast(serverproperty('servername') as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 
		select @sysperfinfoname = 'SQLSERVER'
	end  

select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
from master.sys.dm_exec_cached_plans
union
select
 case
   when counter_name = 'Database pages'
     then 'Committed Pages'
   else
     counter_name
 end,
 cntr_value
from
 master..sysperfinfo
where
 counter_name in
   (
     'Procedure cache pages',
     'Total Server Memory (KB)',
     'Connection Memory (KB)',
     'Lock Memory (KB)',
     'Optimizer Memory (KB)',
     'Granted Workspace Memory (KB)'
   )
 or
   (
     counter_name = 'Database pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
 or
   (
     counter_name = 'Free pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
or
   (
     counter_name = 'Total pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
order by 1

select 
	cast(database_id as bigint), 
	name
from
	master.sys.databases

select 
	counter_name, 
	convert(bigint,isnull(cntr_value,0)), 
	instance_name 
from 
	master.sys.dm_os_performance_counters
where 
	Lower(object_name) = Lower(@sysperfinfoname + ':databases') 
	and Lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
	and Lower(instance_name) <> '_total' 
	and lower(instance_name) <> 'mssqlsystemresource'
order by 
	instance_name 

select 
	db_name(database_id),
	NumberReads = sum(cast(num_of_reads as dec(38,0))),
	NumberWrites = sum(cast(num_of_writes as dec(38,0))),
	BytesRead = sum(cast(num_of_bytes_read as dec(38,0))),
	BytesWritten = sum(cast(num_of_bytes_written as dec(38,0))),
	IoStallMS = sum(cast(io_stall as dec(38,0)))
from 
	sys.dm_io_virtual_file_stats(null,null)
group by
	database_id
