use {Database};

declare @dbid int;
declare @objid int;
set @dbid = db_id();
set @objid = object_id({SchemaTable});

-- We will not make the index recommendation for system tables.
if (objectproperty(@objid, 'isusertable') = 0) 
begin 
	select [AllowRecommendation]='Not a user table'; 
	return; 
end;

-- We will not make the index recommendation for ms tables.
if (objectproperty(@objid, 'IsMSShipped') = 1) 
begin 
	select [AllowRecommendation]='MSShipped table'; 
	return; 
end;

-- if the table writes are more than reads we do not want to make an index recommendation.
if (select count(*) 
	from sys.dm_db_index_usage_stats s
	where s.database_id = @dbid
	and s.object_id = @objid
	and s.index_id <= 1
	and (s.user_seeks + s.user_scans + s.user_lookups) < s.user_updates) > 0
begin
	select [AllowRecommendation]='More updates than reads for table.'; 
	return; 
end;

-- If the tables has columns with datatypes for which indexing fails we donot want to make an index recommendation
if(select count(DATA_TYPE) from 
	INFORMATION_SCHEMA.COLUMNS 
	where table_schema = {SafeSchemaName}
	and table_name = {SafeTableName}
	and column_name in ({SafeAllColumnsList})
	and ((data_type in ('ntext', 'text', 'xml', 'image'))
	  or (data_type in ('nvarchar', 'varchar', 'varbinary') and CHARACTER_MAXIMUM_LENGTH=-1))) > 0
 begin
	select [AllowReocmmendation]='Some columns cannot be used in key columns';
 end;

select [AllowRecommendation]=1; 
return; 