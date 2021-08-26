----------------------------------------------------------------------------------------------
-- //SQLDM 10.0 - Srishti Purohit - New Recommendations - SDR-I24 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------


Select
	dbname = DB_NAME(),
	SchemaName =  OBJECT_SCHEMA_NAME(S.object_id),
	TableName = OBJECT_NAME(S.object_id),
	S.name As StatsName,
	SP.rows,
	SP.modification_counter
From sys.stats S
Cross Apply sys.dm_db_stats_properties(S.object_id, S.stats_id) SP
Where SP.rows >= 1000000
And S.no_recompute = 0
And SP.modification_counter > 100000;


