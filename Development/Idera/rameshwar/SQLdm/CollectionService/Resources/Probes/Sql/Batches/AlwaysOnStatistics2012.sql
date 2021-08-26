--  Batch: Always On Statistics
--  Tables: sys.dm_hadr_database_replica_states
--			sys.dm_hadr_database_replica_cluster_states
--			sys.dm_hadr_availability_replica_states
--------------------------------------------------------------------------------
     
select replica_id,group_database_id,is_database_joined,is_failover_ready into #tmpdbr_database_replica_cluster_states from master.sys.dm_hadr_database_replica_cluster_states
       
select * into #tmpdbr_database_replica_states from master.sys.dm_hadr_database_replica_states
      
select replica_id,role into #tmpdbr_availability_replica_states from master.sys.dm_hadr_availability_replica_states
       
select drs.database_id, drs.last_commit_time into #tmpdbr_database_replica_states_primary_LCT from  #tmpdbr_database_replica_states as drs 
left join #tmpdbr_availability_replica_states ars on drs.replica_id = ars.replica_id 
where ars.role = 1

select	dbr.replica_id
		,dbr.group_id
		,dbr.database_id
		,drcs.is_failover_ready
		,synchronization_state
		,dbr.synchronization_health
		,database_state
		,is_suspended
		,last_hardened_time
		,log_send_queue_size
		,log_send_rate
		,redo_queue_size
		,redo_rate
		,role as 'ReplicaRole'
		,operational_state
		,connected_state
		,last_connect_error_number
		,last_connect_error_description
		,last_connect_error_timestamp
		,filestream_send_rate
		,CONVERT(int,CEILING(ISNULL(CASE dbr.log_send_rate WHEN 0 THEN -1 ELSE CAST(dbr.log_send_queue_size AS float) / dbr.log_send_rate END, -1))) AS [SynchronizationPerformance]
		,CASE dbcs.is_failover_ready WHEN 1 THEN 0 ELSE ISNULL(DATEDIFF(ss, dbr.last_commit_time, dbrp.last_commit_time), 0) END  AS [EstimatedDataLoss]
		,CONVERT(int,CEILING(ISNULL(CASE dbr.redo_rate WHEN 0 THEN -1 ELSE CAST(dbr.redo_queue_size AS float) / dbr.redo_rate END, -1))) AS [EstimatedRecoveryTime]
		, dbr.group_database_id
from #tmpdbr_database_replica_cluster_states AS dbcs
LEFT OUTER JOIN #tmpdbr_database_replica_states AS dbr ON dbcs.replica_id = dbr.replica_id AND dbcs.group_database_id = dbr.group_database_id
LEFT OUTER JOIN #tmpdbr_database_replica_states_primary_LCT AS dbrp ON dbr.database_id = dbrp.database_id
join master.sys.dm_hadr_database_replica_cluster_states drcs on drcs.replica_id = dbr.replica_id and drcs.group_database_id = dbr.group_database_id
join master.sys.dm_hadr_availability_replica_states ars on ars.replica_id = dbr.replica_id


DROP TABLE #tmpdbr_database_replica_cluster_states
DROP TABLE #tmpdbr_database_replica_states
DROP TABLE #tmpdbr_database_replica_states_primary_LCT
DROP TABLE #tmpdbr_availability_replica_states
