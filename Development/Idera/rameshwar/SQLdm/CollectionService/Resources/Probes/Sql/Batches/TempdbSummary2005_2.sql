--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  Batch: Tempdb Monitoring Batch
--  Tables: tempdb.sys.dm_db_file_space_usage, sys.dm_os_performance_counters
--          sys.dm_os_waiting_tasks
--------------------------------------------------------------------------------

-- Tempdb file space usage
--START (RRG): This works in Azure SQL Database 

IF DB_NAME() = 'master' 
BEGIN
IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
select
	name,
	physical_name,
	size,	
	user_object_reserved_page_count,
	internal_object_reserved_page_count,
	version_store_reserved_page_count,
	mixed_extent_page_count,
	unallocated_extent_page_count
from 
	tempdb.sys.dm_db_file_space_usage fsu
	inner join tempdb.sys.database_files df
		on fsu.file_id = df.file_id
where fsu.database_id = 2 

-- Version store generation and cleanup rate

--START (RRG): This works in Azure SQL Database 
select
	VSCounterName = rtrim(counter_name),
	CounterValue = cast(cntr_value as bigint)
from
	sys.dm_os_performance_counters
where 
	counter_name in( 'Version Generation rate (KB/s)', 'Version Cleanup rate (KB/s)')

--START (RRG): This works in both Azure SQL Database and Managed Instance
;with Tasks as 
(
	select 
		session_id,
        wait_type,
        cast(wait_duration_ms as dec(38,0)) as wait_duration_ms,
        blocking_session_id,
        resource_description,
        PageID = right(resource_description, len(resource_description) - charindex(':', resource_description, 3))
    from sys.dm_os_waiting_tasks
    where wait_type Like 'PAGE%LATCH_%' and resource_description like '2:%'
), 
wait_page_descriptions as 
(
	select 
		wait_duration_ms,
        wait_type = case IsNumeric(PageID)
					when 1 then case
						 when PageID = 1 Or PageID % 8088 = 0 then 'PFS'
						 when PageID = 2 Or PageID % 511232 = 0 then 'GAM'
						 when PageID = 3 Or (PageID - 1) % 511232 = 0 then 'SGAM'
	                     else 'Error: ' + cast(PageID as nvarchar(20)) end
					else  'Error: ' + PageID
					end
       from Tasks
)
select
	wait_duration = sum(wait_duration_ms),
    wait_type
from
    wait_page_descriptions
where wait_type in ('PFS', 'GAM', 'SGAM')
group by wait_type
END
ELSE
BEGIN
	select
		NULL AS name,
		NULL AS physical_name,
		NULL AS size,	
		NULL AS user_object_reserved_page_count,
		NULL AS internal_object_reserved_page_count,
		NULL AS version_store_reserved_page_count,
		NULL AS mixed_extent_page_count,
		NULL AS unallocated_extent_page_count

		select
			NULL AS VSCounterName,
			NULL AS CounterValue

		select
			NULL AS wait_duration,
			NULL AS wait_type
END
END