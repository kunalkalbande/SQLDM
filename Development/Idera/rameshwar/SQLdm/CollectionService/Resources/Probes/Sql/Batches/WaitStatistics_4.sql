--------------------------------------------------------------------------------
--  QUERY: Wait Statistics
--  Tables: sys.dm_os_wait_stats
--------------------------------------------------------------------------------

select 
	WaitType = wait_type, 
	WaitingTasksCountTotal = waiting_tasks_count, 
	WaitTimeMSTotal = wait_time_ms, 
	MaxWaitTimeMS = max_wait_time_ms, 
	ResourceWaitMSTotal = wait_time_ms - signal_wait_time_ms
from 
	sys.dm_os_wait_stats
where 
	wait_type Not In ({0})
	and wait_time_ms > 0