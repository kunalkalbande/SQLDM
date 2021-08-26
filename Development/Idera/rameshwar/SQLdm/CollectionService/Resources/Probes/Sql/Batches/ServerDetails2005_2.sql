--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  Batch: Server Details 2005
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
--START (RRG): Get internal for Instance Name when in Azure Platform
declare @sysperfinfoname sysname
Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1
select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
--END (RRG): Get internal for Instance Name when in Azure Platform
	
SELECT DB_NAME();
IF DB_NAME() = 'master'
BEGIN
--------------------------------------------------------------------------------
--  QUERY: Max user connections
--  Tables:sys.configurations
--  Returns:
--    Max user connections from sys.configurations
--    Max user connections from @@max_connections
--  Notes:
--  (1) Use the first if non-zero, else the second
--------------------------------------------------------------------------------
Select 
	value, 
	MaxConnections = @@max_connections 
from 
	syscurconfigs 
where config = 103 

--------------------------------------------------------------------------------
--  QUERY: CPU Affinity
--  Tables: sys.configurations
--  Returns:
--    Configured CPU affinity mask
--------------------------------------------------------------------------------
Select 
	cast(cast(value_in_use as varbinary) as bigint)
from 
	sys.configurations
where configuration_id = 1535 


select ServerProperty('IsClustered')  , ServerProperty('ComputerNamePhysicalNetBIOS')

--exec xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'
-- (RRG): SERVERPROPERTY('ComputerName') returns NULL
--   SELECT 'ComputerName' AS Value, SERVERPROPERTY('ComputerName') AS Data
-- Alternative: SELECT 'ComputerName' AS Value, SERVERPROPERTY('ServerName') AS Data --> returns part1 of the DNS Url
--   for xxx.database.windows.net the above call returns 'xxx' which is the actual Azure SQL Database Server name

SELECT 'ComputerName' AS Value, 'CloudServer' AS Data

--------------------------------------------------------------------------------
--  QUERY: SQL 2005 Configuration Options for Alerting
--  Tables: master.sys.syscurconfigs
--  Returns: CLR enabled, agent XPs enabled, command shell enabled
--------------------------------------------------------------------------------
--START RRG: Modify logic
select 
	configuration_id,
	cast(value_in_use as integer)
from 
	sys.configurations
where
	configuration_id in (1562,16384,16390)

	SELECT cast(database_id AS BIGINT)
	,name
	,0 AS DatabaseStatus
FROM sys.databases

if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
BEGIN
--START RRG: Modify logic
	select 
		counter_name, 
		convert(bigint,isnull(cntr_value,0)), 
		--instance_name,
		DBName = d.name
	from 
		sys.dm_os_performance_counters pc
		inner join sys.databases d on d.physical_database_name = pc.instance_name
	where 			
		lower(object_name) = lower(@sysperfinfoname + ':databases') 
		and lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
		and lower(instance_name) <> '_total' 
		and lower(instance_name) <> 'mssqlsystemresource'
	order by 
		d.name
ELSE
BEGIN
	SELECT NULL, NULL, NULL
END

	-- START: SQLdm 11.0 - Azure Support: Memory Chart
		SELECT *
			FROM sys.resource_stats as rs
			INNER JOIN (
				SELECT database_name
					,MAX(end_time) AS end_time
				FROM sys.resource_stats
				GROUP BY database_name
				) AS grouped_rs ON rs.database_name = grouped_rs.database_name
				AND rs.end_time = grouped_rs.end_time
				inner join sys.databases db
				ON db.name = rs.database_name
		-- END
END
-- END OF master queries

-- BEGIN Per Database Query

-- START: SQLdm 11.0 - Azure Memory Chart : Database size Limit
SELECT DATABASEPROPERTYEX (DB_NAME(DB_ID()), 'MaxSizeInBytes') AS [MaxSizeInBytes], DB_NAME() AS database_name OPTION (RECOMPILE)
-- END

--------------------------------------------------------------------------------
--  QUERY: Throughput
--------------------------------------------------------------------------------
set arithabort on

--START (RRG): sys.dm_os_sys_info DMV is not available in Azure SQL Database
--  so timestamp can not be calculated
declare @timestamp bigint

--select @timestamp = cpu_ticks / (cpu_ticks/ms_ticks) from sys.dm_os_sys_info;
--with SystemHealthRingBuffer(record,timestamp)
--as
--(
--select top 30
--record,
--@timestamp - timestamp
--from sys.dm_os_ring_buffers
--where ring_buffer_type = 'RING_BUFFER_SCHEDULER_MONITOR'
--and record like '%<SystemHealth>%'
--order by timestamp desc
--)
--select 
--       dateadd(ms, (timestamp * -1) % 1000, dateadd(s, (timestamp * -1) / 1000, getutcdate())),
--       SqlCpu = cast(SubString(record, CHARINDEX('<ProcessUtilization>', record) + 20,
--                                  CHARINDEX('<', record, CHARINDEX('<ProcessUtilization>', record)+1)
--                                         - CHARINDEX('<ProcessUtilization>', record) - 20) as int)
--from SystemHealthRingBuffer;
--DECLARE @tempVar_ServerDetails_3 TABLE
--(
--	dummyCol1 datetime,
--	dummyCol2 datetime,
--	dummyCol3 datetime,
--	SqlCpu int,
--	dummyCol4 int
--)
--SELECT * FROM @tempVar_ServerDetails_3

--START RRG: Modify logic

-- cpu usage chart
if HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE') = 1 
 SELECT  top 1 DB_NAME() AS database_name,  avg_cpu_percent, avg_data_io_percent, avg_log_write_percent , 
 dtu_limit, cpu_limit FROM sys.dm_db_resource_stats ORDER by end_time desc;
ELSE
BEGIN
	SELECT DB_NAME() AS database_name, null, null, null, null, null
END

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

/* DECLARE @tempVar_ServerDetails_10 TABLE
(
	dummyCol1 int,
	dummyCol2 int,
	dummyCol3 int,
	dummyCol4 int,
	dummyCol5 int,
	dummyCol6 int,
	dummyCol7 int,
	dummyCol8 int,
	dummyCol9 int,
	dummyCol10 int,
	dummyCol11 int
)
SELECT * FROM @tempVar_ServerDetails_10
*/

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
--  Tables: sysperfinfo
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
IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
select 
	'total pages', 
	isnull(sum(convert(bigint,cntr_value/8)),0)
from 
	sys.dm_os_performance_counters 
where (Lower(object_name) = Lower(@sysperfinfoname + ':memory manager') 
	and Lower(counter_name) = 'total server memory (kb)' )
union
select 
	lower(RTRIM(counter_name)), 
	isnull(sum(convert(bigint,cntr_value)),0)
from 
	sys.dm_os_performance_counters 
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
ELSE
BEGIN
DECLARE @tempVar_ServerDetails_4 TABLE
(
	dummyCol1 nvarchar(max),
	dummyCol2 int
)
SELECT * FROM @tempVar_ServerDetails_4 
END
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
IF EXISTS (
		SELECT dtb.name AS [Database_Name]
		FROM sys.databases AS dtb
		WHERE (
				CAST(CASE 
						WHEN dtb.name IN (
								'master'
								,'model'
								,'msdb'
								,'tempdb'
								)
							THEN 1
						ELSE dtb.is_distributor
						END AS BIT) = 1
				)
			AND dtb.[name] = DB_NAME()
		)
BEGIN
	declare @tempVar_ServerOverview_5 table 
	(
		[num proc buffs] int,
		[num proc buffs used] int,
		[num proc buffs active] int,
		[proc cache size] int,
		[proc cache used] int,
		[proc cache active] int
	) OR 0 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
	select * from @tempVar_ServerOverview_5 
END
ELSE
	dbcc proccache

--------------------------------------------------------------------------------
--  QUERY: Total Locks
--  Tables: sysperfinfo
--  Returns: Lock Count
--------------------------------------------------------------------------------
--START RRG: Modify logic
IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
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
  lower(object_name) = lower(@sysperfinfoname + ':locks')
  and lower(instance_name) = '_total'
  and lower(counter_name) = lower('lock requests/sec')
ELSE
BEGIN

  DECLARE @tempVar_ServerDetails_6 TABLE
  (
	dummyCol1 int
  )
  SELECT * FROM @tempVar_ServerDetails_6
END
--END RRG: Modify logic

{4}
/* SELECT * FROM @temp_ServerDetails_1 */
-- These ResultSet were been expected by the DM interpreters
-- SELECT TOP 0 NULL as configuration_id, NULL as value
-- SELECT TOP 0 NULL , NULL
--END RRG: Modify logic

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

/* 
DECLARE @tempVar_ServerDetails_8 TABLE
(
	counter_name nvarchar(max),
	dummyCol int,
	instance_name nvarchar(max)
)
SELECT * FROM @tempVar_ServerDetails_8
*/
--END RRG: Modify logic

--START (RRG): Needed to reformulate query to accomodate Azure SQL Database returning NULL for database name 
--  by JOINING to sys.databases
if (HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1) 
BEGIN
	select 
		DBName = name,
		NumberReads = sum(cast(num_of_reads as dec(38,0))),
		NumberWrites = sum(cast(num_of_writes as dec(38,0))),
		BytesRead = sum(cast(num_of_bytes_read as dec(38,0))),
		BytesWritten = sum(cast(num_of_bytes_written as dec(38,0))),
		IoStallMS = sum(cast(io_stall as dec(38,0)))
	from 
		sys.dm_io_virtual_file_stats(null,null) AS vfs 
		INNER JOIN sys.databases AS d ON vfs.database_id = d.database_id --JOIN is needed to avoid NULL database names
	WHERE
		d.name = DB_NAME()
	group by
		name
END
ELSE
BEGIN
	SELECT 	DB_NAME() AS DBName, NULL AS NumberReads, NULL AS NumberWrites, NULL AS BytesRead, NULL AS BytesWritten, NULL AS IoStallMS
END


--END (RRG): Need to reformulate query to accomodate Azure SQL Database returning NULL when not a system database

{5}


{6}

-- START: SQLdm 11.0 - Azure Support: Memory Chart
IF DB_NAME() = 'master'
		SELECT *
			FROM sys.resource_stats as rs
			INNER JOIN (
				SELECT database_name
					,MAX(end_time) AS end_time
				FROM sys.resource_stats
				GROUP BY database_name
				) AS grouped_rs ON rs.database_name = grouped_rs.database_name
				AND rs.end_time = grouped_rs.end_time
				inner join sys.databases db
				ON db.name = rs.database_name
ELSE
	SELECT 
		NULL AS name
		, NULL AS database_id
		, NULL AS source_database_id
		, NULL AS owner_sid
		, NULL AS create_date
		, NULL AS compatibility_level
		, NULL AS collation_name
		, NULL AS user_access
		, NULL AS user_access_desc
		, NULL AS is_read_only
		, NULL AS is_auto_close_on
		, NULL AS is_auto_shrink_on
		, NULL AS state
		, NULL AS state_desc
		, NULL AS is_in_standby
		, NULL AS is_cleanly_shutdown
		, NULL AS is_supplemental_logging_enabled
		, NULL AS snapshot_isolation_state
		, NULL AS snapshot_isolation_state_desc
		, NULL AS is_read_committed_snapshot_on
		, NULL AS recovery_model
		, NULL AS recovery_model_desc
		, NULL AS page_verify_option
		, NULL AS page_verify_option_desc
		, NULL AS is_auto_create_stats_on
		, NULL AS is_auto_create_stats_incremental_on
		, NULL AS is_auto_update_stats_on
		, NULL AS is_auto_update_stats_async_on
		, NULL AS is_ansi_null_default_on
		, NULL AS is_ansi_nulls_on
		, NULL AS is_ansi_padding_on
		, NULL AS is_ansi_warnings_on
		, NULL AS is_arithabort_on
		, NULL AS is_concat_null_yields_null_on
		, NULL AS is_numeric_roundabort_on
		, NULL AS is_quoted_identifier_on
		, NULL AS is_recursive_triggers_on
		, NULL AS is_cursor_close_on_commit_on
		, NULL AS is_local_cursor_default
		, NULL AS is_fulltext_enabled
		, NULL AS is_trustworthy_on
		, NULL AS is_db_chaining_on
		, NULL AS is_parameterization_forced
		, NULL AS is_master_key_encrypted_by_server
		, NULL AS is_query_store_on
		, NULL AS is_published
		, NULL AS is_subscribed
		, NULL AS is_merge_published
		, NULL AS is_distributor
		, NULL AS is_sync_with_backup
		, NULL AS service_broker_guid
		, NULL AS is_broker_enabled
		, NULL AS log_reuse_wait
		, NULL AS log_reuse_wait_desc
		, NULL AS is_date_correlation_on
		, NULL AS is_cdc_enabled
		, NULL AS is_encrypted
		, NULL AS is_honor_broker_priority_on
		, NULL AS replica_id
		, NULL AS group_database_id
		, NULL AS resource_pool_id
		, NULL AS default_language_lcid
		, NULL AS default_language_name
		, NULL AS default_fulltext_language_lcid
		, NULL AS default_fulltext_language_name
		, NULL AS is_nested_triggers_on
		, NULL AS is_transform_noise_words_on
		, NULL AS two_digit_year_cutoff
		, NULL AS containment
		, NULL AS containment_desc
		, NULL AS target_recovery_time_in_seconds
		, NULL AS delayed_durability
		, NULL AS delayed_durability_desc
		, NULL AS is_memory_optimized_elevate_to_snapshot_on
		, NULL AS is_federation_member
		, NULL AS is_remote_data_archive_enabled
		, NULL AS is_mixed_page_allocation_on
		, NULL AS is_temporal_history_retention_enabled
		, NULL AS catalog_collation_type
		, NULL AS catalog_collation_type_desc
		, NULL AS physical_database_name
		, NULL AS is_result_set_caching_on
		, NULL AS is_accelerated_database_recovery_on
		, NULL AS is_tempdb_spill_to_remote_store
		, NULL AS is_stale_page_detection_on
		, NULL AS is_memory_optimized_enabled
		-- END

