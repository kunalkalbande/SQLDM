--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

if (select database_id from sys.databases where name = DB_NAME()) > 4 
	select sum(cast(size_in_bytes as bigint)) from sys.dm_exec_cached_plans 
		where usecounts = 1 
		and objtype = 'Adhoc';
else
begin
	DECLARE @tempVar_GetAdhocCachedPlanBytes2005_2 TABLE
	(
		DummyCol bigint
	)
	SELECT * FROM @tempVar_GetAdhocCachedPlanBytes2005_2
end