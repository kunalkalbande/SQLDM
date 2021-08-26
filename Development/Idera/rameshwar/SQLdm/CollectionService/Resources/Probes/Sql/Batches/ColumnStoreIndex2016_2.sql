--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-I30 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------

Select serverproperty('Edition');

--If condition handles all User Databases which is the normal use case
--  for Azure SQL Database
--Else condition handles system database which are not managed by User
if (select database_id from sys.databases where name = DB_NAME()) > 4 
begin
	Select 
		dbname = DB_NAME(),
		SchemaName = OBJECT_SCHEMA_NAME(P.object_id),
		TableName = OBJECT_NAME(P.object_id),
		Row_Count = P.rows,
		IUS.user_scans
	From sys.partitions P
	Inner Join sys.dm_db_index_usage_stats IUS On IUS.object_id = P.object_id And IUS.index_id = P.index_id
	Where IUS.database_id = db_id()
	And P.index_id In (1, 0)
	And P.rows > 1000000
	And IUS.user_scans > FLOOR(P.rows *.1)
	And Not Exists (Select 1 From sys.column_store_segments
					Where hobt_id = P.hobt_id);
end
else
begin
	DECLARE @tempVar_ColumnStoreIndex2016_2 TABLE
	(
		dbname nvarchar(128),
		SchemaName sysname,
		TableName sysname,
		Row_Count bigint,
		user_scans bigint
	)
	SELECT * FROM @tempVar_ColumnStoreIndex2016_2
end