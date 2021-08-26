--------------------------------------------------------------------------------
--  Batch: Server Details 2000
--  SQLdm 10.3 (Varun Chopra) Linux Support
--  Tables: master..syscurconfigs, master..sysprocesses, master..sysperfinfo
--	System Variables: @@max_connections, @@cpu_busy, @@idle, @@io_busy, 
--		@@pack_received, @@pack_sent, @@packet_errors, @@total_read, 
--		@@total_write, @@total_errors, @@connections 
--  Variables: 
--		[0] - Sysperfinfo pipe prefix
--		[1] - Session Count Segment
--		[2] - Resource Check Segment
--		[3] - Blocking Check Segment
--		[4] - Memory Segment
--------------------------------------------------------------------------------
set nocount on

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
--  QUERY: CPU Affinity
--  Tables: master..syscurconfigs 
--  Returns:
--    Configured CPU affinity mask
--------------------------------------------------------------------------------
Select 
	cast(cast(value as varbinary) as bigint)
from 
	master..syscurconfigs 
where config = 1535 

--------------------------------------------------------------------------------
--  QUERY: Throughput
--  Tables: none
--  Returns:
--		CPU Busy
--		Idle Time
--		IO Busy
--		Packets Received
--		Packets Sent
--		Packet Errors
--		Total Read
--		Total Write
--		Total Errors
--		Total Connections
--------------------------------------------------------------------------------
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

{1}

declare 
	@command nvarchar(2048), 
	@dbname nvarchar(255), 
	@tabid int, 
	@lock_type smallint,
	@dbid smallint, 
	@mode smallint, 
	@status integer, 
	@status2 integer, 
	@log_size varchar(20), 
	@total_db_size varchar(20), 
	@cmptlevel int, 
	@category int, 
	@dumptrdate varchar(18), 
	@spid int,
	@systemdatabase bit


{2}

{3}

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
	master..sysperfinfo 
where 
	(lower(object_name) = lower('{0}:SQL Statistics')
		and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
	or (Lower(object_name) = Lower('{0}:Buffer Manager') 
		and (Lower(counter_name) like 'buffer cache hit ratio%'
		or Lower(counter_name) = 'total pages')) 
	or (Lower(object_name) = Lower('{0}:cache manager') 
		and Lower(counter_name) like 'cache hit ratio%' 
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower('{0}:databases') 
		and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower('{0}:memory manager') 
		and Lower(counter_name) in ('target server memory(kb)', 'total server memory (kb)')) 
	or (Lower(object_name) = Lower('{0}:access methods') 
		and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
	or (Lower(object_name) = Lower('{0}:locks') 
		and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower('{0}:buffer manager') 
		and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
group by 
	counter_name 


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
--SQLdm 10.0 (Tarun Sapra)- following command isnt available, thus we are sending empty table 
		--dbcc proccache 
		DECLARE @tempVar table (
		[num proc buffs] int,
		[num proc buffs used] int,
		[num proc buffs active] int,
		[proc cache size] int,
		[proc cache used] int,
		[proc cache active] int
		)
		select * from @tempVar

select ServerProperty('IsClustered') 

		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - cloud server doesnt support registry reading
		--exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'
		select 'ComputerName' as Value,'CloudServer' as Data
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

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
  lower(object_name) = lower('{0}:locks')
  and lower(instance_name) = '_total'
  and lower(counter_name) = lower('lock requests/sec')

{4}

select 
	cast(dbid as bigint),
	name,
	case when isnull(databasepropertyex(name,'IsInStandby'),0) = 1 then 16 else 0 end 									--Standby
	+ case when isnull(databaseproperty(name, 'IsInLoad'),0) = 1 or db.status & 32 = 32 then 32 else 0 end				--Restoring (non-mirror)
	+ case when db.status & 64 = 64 then 64 else 0 end																	--Pre-Recovery
	+ case when isnull(databaseproperty(name, 'IsInRecovery'),0) = 1 or db.status & 128 = 128 then 128 else 0 end		--Recovering
	+ case when isnull(databaseproperty(name, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(name, 'IsSuspect'),0) = 1 
		or isnull(databaseproperty(name, 'IsShutDown'),0) = 1 or db.status & 256 = 256 then 256 else 0 end				--Suspect
	+ case when isnull(databaseproperty(name, 'IsOffline'),0) = 1 or db.status & 512 = 512 then 512 else 0 end			--Offline
	+ case when isnull(databaseproperty(name, 'IsReadOnly'),0) = 1 or db.status & 1024 = 1024 then 1024 else 0 end		--Read Only
	+ case when isnull(databaseproperty(name, 'IsDboOnly'),0) = 1 or db.status & 2048 = 2048 then 2048 else 0 end		--DBO Use
	+ case when isnull(databaseproperty(name, 'IsSingleUser'),0) = 1 or db.status & 4096 = 4096 then 4096 else 0 end		--Single User
	+ case when isnull(databaseproperty(name, 'IsEmergencyMode'),0) = 1 or db.status & 32768 = 32768 then 32768 else 0 end	--Emergency Mode	
	+ case when db.status & 1073741824 = 1073741824 then 1073741824 else 0 end												--Cleanly Shutdown
from 
	master..sysdatabases db

select 
	counter_name, 
	convert(bigint,isnull(cntr_value,0)), 
	instance_name 
from 
	master..sysperfinfo (nolock)
where 
	--Lower(object_name) = Lower('{0}:databases') 
	--and 
	Lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
	and Lower(instance_name) <> '_total' 
	and lower(instance_name) <> 'mssqlsystemresource'
order by 
	instance_name 

select 
	db_name(DbId),
	NumberReads = sum(cast(NumberReads as dec(38,0))),
	NumberWrites = sum(cast(NumberWrites as dec(38,0))),
	BytesRead = sum(cast(BytesRead as dec(38,0))),
	BytesWritten = sum(cast(BytesWritten as dec(38,0))),
	IoStallMS = sum(cast(IoStallMS as dec(38,0)))
from 
	::fn_virtualfilestats(-1,-1)
group by
	DbId



