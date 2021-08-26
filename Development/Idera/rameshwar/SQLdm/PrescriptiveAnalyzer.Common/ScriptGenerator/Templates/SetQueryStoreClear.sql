--There are many ways this particular problem can be addressed. Here are some options for addressing it.
--Clear the Query Store:

ALTER DATABASE {0} SET QUERY_STORE CLEAR;

--Clear select query plans out of the Query Store:
--Select data can be removed y using one of the following system stored procedures: 
--sp_query_store_remove_query (removes all plans for a specific query), 
--sp_query_store_reset_exec_stats (removes execution stats for a specific plan), 
--sp_query_store_remove_plan (removes a specific plan).

--Lower retention policy of the cleanup process:
--ALTER DATABASE <Database name> SET QUERY_STORE (
--	CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = <New value>)
--	);


--Increase allowed storage for Query store:
--ALTER DATABASE <Database name> SET QUERY_STORE (
--	MAX_STORAGE_SIZE_MB = <New value>);