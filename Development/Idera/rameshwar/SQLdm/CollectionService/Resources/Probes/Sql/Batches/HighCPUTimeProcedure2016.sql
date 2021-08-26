----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q43 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------


Select serverproperty('Edition');

Select 
	dbname = DB_NAME(),
	ProcedureName = OBJECT_SCHEMA_NAME(object_id) + N'.' + OBJECT_NAME(object_id),
	AverageWorkerTime = total_worker_time/execution_count,
	AverageElapsedTime = total_elapsed_time/execution_count,
	WorkerToCPUTimePercent = floor((total_worker_time*1.0)/total_elapsed_time * 100)
From sys.dm_exec_procedure_stats
Where type = 'P'
And execution_count > 500
And database_id = db_id();

