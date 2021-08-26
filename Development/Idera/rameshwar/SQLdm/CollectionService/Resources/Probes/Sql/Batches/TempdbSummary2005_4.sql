--------------------------------------------------------------------------------
--  Batch: Tempdb Monitoring Batch
--------------------------------------------------------------------------------

-- Tempdb file space usage
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
select
	rtrim(counter_name),
	cast(cntr_value as bigint)
from
	sys.dm_os_performance_counters
where 
	counter_name in( 'Version Generation rate (KB/s)', 'Version Cleanup rate (KB/s)')
	
;With Tasks
As (Select session_id,
        wait_type,
        cast(wait_duration_ms as dec(38,0)) as wait_duration_ms,
        blocking_session_id,
        resource_description,
        PageID = Right(resource_description, Len(resource_description)
                - Charindex(':', resource_description, 3))
    From sys.dm_os_waiting_tasks
    Where wait_type Like 'PAGE%LATCH_%'
    And resource_description Like '2:%')
, wait_page_descriptions
As (Select wait_duration_ms,
              wait_type= Case IsNumeric(PageID)
					When 1 Then Case
						 When PageID = 1 Or PageID % 8088 = 0 Then 'PFS'
						 When PageID = 2 Or PageID % 511232 = 0 Then 'GAM'
						 When PageID = 3 Or (PageID - 1) % 511232 = 0 Then 'SGAM'
	                     Else 'Error: ' + cast(PageID as nvarchar(20))
					End
				Else  'Error: ' + PageID
              End
       From Tasks)
select
       wait_duration = sum(wait_duration_ms),
       wait_type
from
       wait_page_descriptions
where wait_type in ('PFS', 'GAM', 'SGAM')
group by
       wait_type