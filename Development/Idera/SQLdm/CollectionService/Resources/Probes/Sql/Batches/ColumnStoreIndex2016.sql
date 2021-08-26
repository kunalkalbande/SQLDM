----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-I30 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------


Select serverproperty('Edition');

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
