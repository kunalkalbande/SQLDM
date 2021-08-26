--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--(RRG): Call within the context of User Database not tempdb
--use tempdb;
IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
select 
      t.wait_type, 
      t.wait_duration_ms,
      t.session_id, 
      t.resource_description,
      s.program_name,
      b.sql_handle,
      b.statement_start_offset,
      b.statement_end_offset,
      st.text
      from sys.dm_os_waiting_tasks t
      left join sys.dm_exec_requests b on t.waiting_task_address = b.task_address 
      left join sys.dm_exec_sessions s on b.session_id = s.session_id
      outer apply sys.dm_exec_sql_text (b.sql_handle) st
      where b.sql_handle is not null
			and t.wait_type not in (
								'CLR_SEMAPHORE', 'LAZYWRITER_SLEEP', 'RESOURCE_QUEUE', 'SLEEP_TASK',
								'SLEEP_SYSTEMTASK', 'SQLTRACE_BUFFER_FLUSH', 'WAITFOR', 'LOGMGR_QUEUE',
								'CHECKPOINT_QUEUE', 'REQUEST_FOR_DEADLOCK_SEARCH', 'XE_TIMER_EVENT', 'BROKER_TO_FLUSH',
								'BROKER_TASK_STOP', 'CLR_MANUAL_EVENT', 'CLR_AUTO_EVENT', 'DISPATCHER_QUEUE_SEMAPHORE',
								'FT_IFTS_SCHEDULER_IDLE_WAIT', 'XE_DISPATCHER_WAIT', 'XE_DISPATCHER_JOIN', 'BROKER_EVENTHANDLER',
								'TRACEWRITE', 'FT_IFTSHC_MUTEX', 'SQLTRACE_INCREMENTAL_FLUSH_SLEEP',
								'BROKER_RECEIVE_WAITFOR', 'ONDEMAND_TASK_QUEUE', 'DBMIRROR_EVENTS_QUEUE',
								'DBMIRRORING_CMD', 'BROKER_TRANSMITTER', 'SQLTRACE_WAIT_ENTRIES',
								'SLEEP_BPOOL_FLUSH', 'SQLTRACE_LOCK');
            --and t.wait_type in ('ASYNC_NETWORK_IO', 'CXPACKET');
ELSE
	SELECT 
	  NULL AS wait_type, 
      NULL AS wait_duration_ms,
      NULL AS session_id, 
      NULL AS resource_description,
      NULL AS program_name,
      NULL AS sql_handle,
      NULL AS statement_start_offset,
      NULL AS statement_end_offset,
      NULL AS text