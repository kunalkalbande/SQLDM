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
	sys.dm_db_file_space_usage fsu
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
	
if (select isnull(object_id('tempdb..#wait_pages'), 0)) <> 0  
	drop table #wait_pages	
	
-- Tempdb contention	
;with 
waiting_tasks(wait_duration_ms,resource_description,lastColon,descLength)
as
(
	-- Identify current PAGELATCH and PAGEIOLATCH waits from tempdb
	select
		cast(wait_duration_ms as dec(38,0)),
		resource_description,
		lastColon = isnull(charindex(':',resource_description,3),0),
		descLength = isnull(len(resource_description),0)
	from
		sys.dm_os_waiting_tasks
	where 
		resource_description like '2:%'
		and wait_type like 'PAGE%LATCH%'
)
-- String manipulate to get the page from the description
select
	wait_duration_ms,
	wait_page = case when lastColon > 0 then substring(resource_description,1+lastColon,1+descLength-lastColon) else null end
	into #wait_pages
from
	waiting_tasks
where 
	resource_description like '2:%'
	and lastColon > 0
	

delete from #wait_pages
	where isnumeric(wait_page) = 0


;with wait_page_types(wait_duration_ms,PFS,GAM,SGAM,wait_page)
as
(
	-- Identify PFS, GAM, and SGAM pages
	-- PFS page 1 and every 8088 pages
	-- GAM page 2 and every 511232 pages 
	-- SGAM page 3 and every 511232 pages 
	select 
		wait_duration_ms,
		PFS = cast(case when cast(wait_page as bigint) = 1 then 0 else (cast(wait_page as bigint) % 8088) end as bigint),
		GAM = cast(case when cast(wait_page as bigint) = 2 then 0 else (cast(wait_page as bigint) % 511232) end as bigint),
		SGAM = cast(case when cast(wait_page as bigint) = 3 then 0 else (cast(wait_page as bigint) % 511233) end as bigint),
		wait_page
	from
		#wait_pages
	where
		isnumeric(wait_page) = 1
),
wait_page_descriptions(wait_duration_ms,wait_type)
as
(
select
	wait_duration_ms,
	page = case 
			when PFS = 0 then 'PFS' 
			when GAM = 0 then 'GAM'
			when SGAM = 0 then 'SGAM'
			else 'Error: ' + cast(wait_page as nvarchar(20))
			end
from 
	wait_page_types
where
	PFS = 0 or GAM = 0 or SGAM = 0	
)
select
	wait_duration = sum(wait_duration_ms),
	wait_type
from
	wait_page_descriptions
group by
	wait_type
	

if (select isnull(object_id('tempdb..#wait_pages'), 0)) <> 0  
	drop table #wait_pages	