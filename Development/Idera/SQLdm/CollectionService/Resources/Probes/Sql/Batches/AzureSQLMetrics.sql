if HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE') = 1 
BEGIN
SELECT DISTINCT avg(avg_data_io_percent) AS AverageDataIOPct, avg(avg_log_write_percent) AS AverageLogWritePct, 
max(max_worker_percent) AS MaxWorkerPct ,max(max_session_percent) as MaxSessionPct, avg(dtu_limit) AS DTU_Limit, 
avg(avg_memory_usage_percent) as AvgMemoryUsagePct, AVG(xtp_storage_percent) AS InMemoryStoragePct
FROM sys.dm_db_resource_stats
WHERE end_time >= dateadd(hh,-1,current_timestamp) 
END
ELSE
BEGIN
SELECT NULL AS AverageDataIOPct, NULL AS AverageLogWritePct, 
NULL AS MaxWorkerPct ,NULL as MaxSessionPct, NULL AS DTU_Limit, 
NULL as AvgMemoryUsagePct, NULL AS InMemoryStoragePct
END