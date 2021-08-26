---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

--------------------------------------------------------------------------------
--  Batch: Server Details 2012
--  Variables: 
--		[0] - Sysperfinfo pipe prefix
--            format needs to comply for Azure SQL DB
--            See ServerOverview2012_2 script for how to get this
--		[1] - Session Count Segment
--		[2] - Resource Check Segment
--		[3] - Blocking Check Segment
--		[4] - Memory Segment
--		[5] - Wait Statistics Segment
--		[6] - Tempdb Summary Segment
--------------------------------------------------------------------------------
set nocount on

declare @sysperfinfoname_trans sysname
Set @sysperfinfoname_trans = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
select @sysperfinfoname_trans = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname_trans = SUBSTRING(@sysperfinfoname_trans, 1, PATINDEX('%:%',@sysperfinfoname_trans)-1)

--------------------------------------------------------------------------------
--  QUERY: Max user connections
--  Tables:sys.configurations
--  Returns:
--    Max user connections from sys.configurations
--    Max user connections from @@max_connections
--  Notes:
--  (1) Use the first if non-zero, else the second
--------------------------------------------------------------------------------
--START (RRG): master..syscurconfigs is available in Azure SQL Database and Managed Instance
Select 
	value, 
	MaxConnections = @@max_connections 
from 
	syscurconfigs 
where config = 103 
--END (RRG): master..syscurconfigs is available in Azure SQL Database and Managed Instance

--------------------------------------------------------------------------------
--  QUERY: CPU Affinity
--  Tables: sys.configurations
--  Returns:
--    Configured CPU affinity mask
--------------------------------------------------------------------------------
--START RRG: Modify Logic
Select 
	cast(cast(value_in_use as varbinary) as bigint)
from 
	sys.configurations
where configuration_id = 1535 
--END RRG: Modify logic

--------------------------------------------------------------------------------
--  QUERY: Throughput
--------------------------------------------------------------------------------
set arithabort on

--START (RRG): Should work in Azure SQL Database Managed Instance
--declare @timestamp bigint
--select @timestamp = cpu_ticks / (cpu_ticks/ms_ticks) from sys.dm_os_sys_info

--;with SystemHealthRingBuffer(record,timestamp) as
--(
--	select top 30
--		record = convert(xml,record),
--		@timestamp - timestamp
--	from sys.dm_os_ring_buffers
--	where ring_buffer_type = 'RING_BUFFER_SCHEDULER_MONITOR'
--		and record like '%<SystemHealth>%'
--	order by timestamp desc
--)
--select 
--	DateStamp = dateadd(ms, (timestamp * -1) % 1000, dateadd(s, (timestamp * -1) / 1000, getutcdate())),
--	SqlCpu = record.value('(./Record/SchedulerMonitorEvent/SystemHealth/ProcessUtilization)[1]', 'int')
--from SystemHealthRingBuffer
--START (RRG): Should work in Azure SQL Database Managed Instance

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
--      total buffer cache pages. In 2012 this is returned as kb while prev. 
--      It was pages. I am unioning and converting to avoid downstream issues.
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
--START RRG: Modify logic
--START RRG: Modify logic
select 
	'total pages', 
	isnull(sum(convert(bigint,cntr_value/8)),0)
from 
	sys.dm_os_performance_counters 
where (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
	and Lower(counter_name) = 'total server memory (kb)' )
union
select 
	lower(RTRIM(counter_name)), 
	isnull(sum(convert(bigint,cntr_value)),0)
from 
	sys.dm_os_performance_counters 
where 
	(lower(object_name) = lower(@sysperfinfoname_trans + ':SQL Statistics')
		and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':Buffer Manager') 
		and (Lower(counter_name) like 'buffer cache hit ratio%'
		or Lower(counter_name) = 'total pages')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':plan cache') 
		and Lower(counter_name) like 'cache hit ratio%' 
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':databases') 
		and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
		and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':access methods') 
		and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':locks') 
		and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':buffer manager') 
		and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
group by 
	counter_name 
--END RRG: Modify Logic

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

select ServerProperty('IsClustered')  , ServerProperty('ComputerNamePhysicalNetBIOS')

--exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'
SELECT 'ComputerName' AS Value, 'CloudServer' AS Data

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
  sys.dm_os_performance_counters
where
  lower(object_name) = lower('{0}:locks')
  and lower(instance_name) = '_total'
  and lower(counter_name) = lower('lock requests/sec')


--------------------------------------------------------------------------------
--  QUERY: SQL 2005 Configuration Options for Alerting
--  Tables: master.sys.syscurconfigs
--  Returns: CLR enabled, agent XPs enabled, command shell enabled
--------------------------------------------------------------------------------

select 
	configuration_id,
	cast(value_in_use as integer)
from 
	sys.configurations
where
	configuration_id in (1562,16384,16390)

{4}

-- Valid states for the database
declare @readIntentOnlyAvailabilityReplica int = 65536 -- Read Intent Only availability replica
declare @unreadableAvailabilityReplica int = 131072 -- No Readable availability replica


/*
select
	cast(db.database_id as bigint), 
	name,
	DatabaseStatus = 
		case when isnull(mirr.mirroring_role,0) = 2 and isnull(db.state,0) = 1
			then 8
			else 0 end								--Restoring Mirror
		+ case when isnull(databasepropertyex(db.name,'IsInStandby'),0) = 1 or db.is_in_standby = 1 then 16 else 0 end 			--Standby
		+ case when isnull(db.state,0) = 1 and isnull(mirr.mirroring_role,0) <> 2 then 32 else 0 end							--Restoring (non-mirror)
		+ case when isnull(db.state,0) = 3 then 64 else 0 end																	--Pre-Recovery
		+ case when isnull(db.state,0) = 2 then 128 else 0 end																	--Recovering
		+ case when isnull(db.state,0) = 4 then 256 else 0 end																	--Suspect
		+ case when isnull(db.state,0) = 6 then 512 else 0 end																	--Offline
		+ case when isnull(db.is_read_only,0) = 1 then 1024 else 0 end															--Read Only
		+ case when db.user_access = 1 then 4096 else 0 end																		--Single User
		+ case when isnull(db.state,0) = 5 then 32768 else 0 end																--Emergency Mode
		
		+ case when isnull(db.state, 0) = 0 and isnull(r.secondary_role_allow_connections, 2) = 0
			then @unreadableAvailabilityReplica
			else 0 end -- No Readable availability replica
		+ case when isnull(db.state, 0) = 0 and isnull(r.secondary_role_allow_connections, 2) = 1
			then @readIntentOnlyAvailabilityReplica
			else 0 end -- Read Intent Only availability replica
			
		+ case when isnull(db.is_cleanly_shutdown,0) = 1 then 1073741824 else 0 end	
from
	master.sys.databases db
	left outer join sys.database_mirroring mirr 
		on mirr.database_id = db.database_id
		left outer join sys.dm_hadr_database_replica_states s on s.database_id = db.database_id
				left outer join sys.availability_replicas r on r.replica_id = s.replica_id
				and r.group_id = s.group_id*/
SELECT cast(database_id AS BIGINT)
	,name
	,0 AS DatabaseStatus
FROM sys.databases

--START RRG: Modify logic

select 
	counter_name, 
	convert(bigint,isnull(cntr_value,0)), 
	instance_name 
from 
	sys.dm_os_performance_counters
where 
	Lower(object_name) = Lower('{0}:databases') 
	and Lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
	and Lower(instance_name) <> '_total' 
	and lower(instance_name) <> 'mssqlsystemresource'
order by 
	instance_name 

--END RRG: Modify logic

select 
	DBName = db_name(database_id),
	NumberReads = sum(cast(num_of_reads as dec(38,0))),
	NumberWrites = sum(cast(num_of_writes as dec(38,0))),
	BytesRead = sum(cast(num_of_bytes_read as dec(38,0))),
	BytesWritten = sum(cast(num_of_bytes_written as dec(38,0))),
	IoStallMS = sum(cast(io_stall as dec(38,0)))
from 
	sys.dm_io_virtual_file_stats(null,null)
group by
	database_id


{5}


{6}

-- START: SQLdm 11.0 - Azure Support: Managed Instance Memory Chart
			SELECT top (1) * FROM sys.server_resource_stats
				ORDER BY end_time desc
		-- END