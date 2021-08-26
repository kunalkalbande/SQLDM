--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  QUERY: Wait Statistics
--  Tables: sys.dm_db_wait_stats
--------------------------------------------------------------------------------

--START (RRG): This works in Azure SQL Database
--------------------------------------------------------------------------------
--  QUERY: Wait Statistics
--  Tables: sys.dm_os_wait_stats
--------------------------------------------------------------------------------

IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
	SELECT WaitType = wait_type
		,WaitingTasksCountTotal = waiting_tasks_count
		,WaitTimeMSTotal = wait_time_ms
		,MaxWaitTimeMS = max_wait_time_ms
		,ResourceWaitMSTotal = wait_time_ms - signal_wait_time_ms
	FROM sys.dm_os_wait_stats
	WHERE wait_type NOT IN (
			'BAD_PAGE_PROCESS'
			,'BROKER_EVENTHANDLER'
			,'BROKER_RECEIVE_WAITFOR'
			,'BROKER_TASK_STOP'
			,'BROKER_TO_FLUSH'
			,'BROKER_TRANSMITTER'
			,'CHECKPOINT_QUEUE'
			,'CHKPT'
			,'CLR_SEMAPHORE'
			,'DBMIRROR_EVENTS_QUEUE'
			,'DISPATCHER_QUEUE_SEMAPHORE'
			,'FILESTREAM_WORKITEM_QUEUE'
			,'FT_IFTS_SCHEDULER_IDLE_WAIT'
			,'FT_IFTSHC_MUTEX'
			,'KSOURCE_WAKEUP'
			,'LAZYWRITER_SLEEP'
			,'LOGMGR_QUEUE'
			,'MISCELLANEOUS'
			,'ONDEMAND_TASK_QUEUE'
			,'REQUEST_FOR_DEADLOCK_SEARCH'
			,'RESOURCE_QUEUE'
			,'SLEEP_SYSTEMTASK'
			,'SLEEP_TASK'
			,'SP_SERVER_DIAGNOSTICS_SLEEP'
			,'SQLTRACE_BUFFER_FLUSH'
			,'WAITFOR'
			,'XE_DISPATCHER_WAIT'
			,'XE_TIMER_EVENT'
			)
		AND wait_time_ms > 0
ELSE
BEGIN
	DECLARE @dummy_dm_os_wait_stats TABLE (
		WaitType NVARCHAR(120)
		,WaitingTasksCountTotal BIGINT
		,WaitTimeMSTotal BIGINT
		,MaxWaitTimeMS BIGINT
		,ResourceWaitMSTotal BIGINT
		);

	SELECT WaitType
		,WaitingTasksCountTotal
		,WaitTimeMSTotal
		,MaxWaitTimeMS
		,ResourceWaitMSTotal
	FROM @dummy_dm_os_wait_stats
END
