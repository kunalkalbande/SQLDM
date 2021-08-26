----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I31 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------

Select 
dbname = DB_NAME(),
SchemaName = OBJECT_SCHEMA_NAME(I.object_id),
TableName =  OBJECT_NAME(I.object_id), 
IndexName = I.name, 
I.filter_definition
From sys.indexes I
Where has_filter = 1
And Not Exists(Select 1 From sys.index_columns IC 
Inner Join sys.columns C On C.object_id = IC.object_id
Where IC.object_id = I.object_id
And IC.index_id = I.index_id 
And CharIndex(quotename(C.name) COLLATE DATABASE_DEFAULT,I.filter_definition COLLATE DATABASE_DEFAULT) > 1
And C.column_id = IC.column_id)