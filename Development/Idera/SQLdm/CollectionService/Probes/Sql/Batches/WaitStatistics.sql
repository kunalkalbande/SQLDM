--------------------------------------------------------------------------------
--  QUERY: Wait Statistics
--  Tables: sys.dm_os_wait_stats
--------------------------------------------------------------------------------

declare @ExcludedWaitTypes table(ExcludedType nvarchar(256))

{0}

select 
	WaitType = wait_type, 
	WaitingTasksCountTotal = waiting_tasks_count, 
	WaitTimeMSTotal = wait_time_ms, 
	MaxWaitTimeMS = max_wait_time_ms, 
	ResourceWaitMSTotal = wait_time_ms - signal_wait_time_ms
from 
	sys.dm_os_wait_stats
	left join @ExcludedWaitTypes
	on wait_type collate database_default = ExcludedType collate database_default 
where 
	ExcludedType is null
	and wait_time_ms > 0