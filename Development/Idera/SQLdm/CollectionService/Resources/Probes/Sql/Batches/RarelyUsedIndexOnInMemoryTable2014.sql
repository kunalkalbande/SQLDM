----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I29 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------
select serverproperty('Edition');

Select
	dbname = DB_NAME(),
	SchemaName = OBJECT_SCHEMA_NAME(I.object_id),
	TableName = OBJECT_NAME(I.object_id),
	IndexName = I.name,
	XIS.rows_returned,
	XIS.scans_started
From sys.indexes I
Inner Join sys.dm_db_xtp_index_stats XIS On XIS.object_id = I.object_id And XIS.index_id = I.index_id
Where XIS.rows_returned < XIS.scans_started;