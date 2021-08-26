declare @dbid int;
select @dbid = db_id();

------------------------------------------------
-- return duplicate index information
--
;with indexcols as
(
	select 
		object_id, 
		index_id, 
		name,
		is_unique,
		is_primary_key,
		(select cast(c.column_id as varchar) + case c.is_descending_key when 1 then 'D' else 'A' end  as [data()] -- pull key columns
			from sys.index_columns as c
			where c.object_id = i.object_id
			and c.index_id = i.index_id
            and c.is_included_column = 0
			order by c.key_ordinal, c.column_id
			for xml path('')) as cols,
		isnull((select c.column_id as [data()] -- pull included columns
			from sys.index_columns as c
			where c.object_id = i.object_id
			and c.index_id = i.index_id
            and c.is_included_column = 1
			order by c.column_id
			for xml path('')), '') as inc
		from sys.indexes as i 
			where is_disabled = 0 
			and is_hypothetical = 0
			and i.type < 3 -- do not include xml/spatial indexes
			and i.type > 0 -- do not include heaps
			and objectproperty(i.object_id,'IsMSShipped') = 0
			and objectproperty(i.object_id,'IsUserTable') = 1
			and (indexproperty(i.object_id, i.name, 'IsFulltextKey') <> 1)
)
select
	[DupIndex]='true',
	[DatabaseName]=db_name(),
	[Schema]= (Select s.name from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id where o.object_id = c1.object_id),
	[TableName]=object_name(c1.object_id),
	[IndexName]=c1.name,
	[IndexId]=c1.index_id,
	[IndexUnique]=c1.is_unique,
	[IndexPrimaryKey]=c1.is_primary_key,
	[IndexUsage]=(select user_seeks+user_scans+user_lookups from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c1.object_id and index_id = c1.index_id),
	[IndexUpdates]=(select user_updates from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c1.object_id and index_id = c1.index_id),
	[IndexKeySize]=0,
	[IndexForeignKeys]=(select count(*) from sys.foreign_keys fk where fk.referenced_object_id = c1.object_id and fk.key_index_id = c1.index_id),
	[DupIndexName]=c2.name,
	[DupIndexId]=c2.index_id,
	[DupIndexUnique]=c2.is_unique,
	[DupIndexPrimaryKey]=c2.is_primary_key,
	[DupIndexUsage]=(select user_seeks+user_scans+user_lookups from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c2.object_id and index_id = c2.index_id),
	[DupIndexUpdates]=(select user_updates from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c2.object_id and index_id = c2.index_id),
	[DupIndexKeySize]=0,
	[DupIndexForeignKeys]=(select count(*) from sys.foreign_keys fk where fk.referenced_object_id = c2.object_id and fk.key_index_id = c2.index_id)
	from indexcols as c1
	join indexcols as c2 
		on c1.object_id = c2.object_id 
		and c1.index_id < c2.index_id 
		and c1.cols = c2.cols and c1.inc = c2.inc;

------------------------------------------------
-- return partial duplicate index information
--
;with indexcols as
(
	select 
		object_id, 
		index_id, 
		name,
		is_unique,
		is_primary_key,
		(select cast(c.column_id as varchar) + case c.is_descending_key when 1 then 'D' else 'A' end  as [data()] -- pull key columns
			from sys.index_columns as c
			where c.object_id = i.object_id
			and c.index_id = i.index_id
            and c.is_included_column = 0
			order by c.key_ordinal, c.column_id
			for xml path('')) as cols,
		isnull((select c.column_id as [data()] -- pull included columns
			from sys.index_columns as c
			where c.object_id = i.object_id
			and c.index_id = i.index_id
            and c.is_included_column = 1
			order by c.column_id
			for xml path('')), '') as inc
		from sys.indexes as i 
			where is_disabled = 0 
			and is_hypothetical = 0
			and i.type < 3 -- do not include xml/spatial indexes
			and i.type > 0 -- do not include heaps
			and objectproperty(i.object_id,'IsMSShipped') = 0
			and objectproperty(i.object_id,'IsUserTable') = 1
			and (indexproperty(i.object_id, i.name, 'IsFulltextKey') <> 1)
)
select
	[DupIndex]='false',
	[DatabaseName]=db_name(),
	[Schema]= (Select s.name from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id where o.object_id = c1.object_id),
	[TableName]=object_name(c1.object_id),
	[IndexName]=c1.name,
	[IndexId]=c1.index_id,
	[IndexUsage]=(select user_seeks+user_scans+user_lookups from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c1.object_id and index_id = c1.index_id),
	[IndexUpdates]=(select user_updates from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c1.object_id and index_id = c1.index_id),
	[IndexKeySize]=LEN(c1.cols),
	[IndexUnique]=c1.is_unique,
	[IndexPrimaryKey]=c1.is_primary_key,
    [IndexForeignKeys]=(select count(*) from sys.foreign_keys fk where fk.referenced_object_id = c1.object_id and fk.key_index_id = c1.index_id),
	[IndexIncludeCols]=c1.inc,
	[IndexCols]=c1.cols,
	[DupIndexName]=c2.name,
	[DupIndexUsage]=(select user_seeks+user_scans+user_lookups from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c2.object_id and index_id = c2.index_id),
	[DupIndexUpdates]=(select user_updates from sys.dm_db_index_usage_stats where database_id = @dbid and object_id = c2.object_id and index_id = c2.index_id),
	[DupIndexId]=c2.index_id,
	[DupIndexUnique]=c2.is_unique,
	[DupIndexPrimaryKey]=c2.is_primary_key,
	[DupIndexKeySize]=LEN(c2.cols),
	[DupIndexForeignKeys]=(select count(*) from sys.foreign_keys fk where fk.referenced_object_id = c2.object_id and fk.key_index_id = c2.index_id),
	[DupIndexIncludeCols]=c2.inc,
	[DupIndexCols]=c2.cols
	from indexcols as c1
	join indexcols as c2 
		on c1.object_id = c2.object_id 
		and c1.is_unique = c2.is_unique
		and c1.index_id < c2.index_id
        and ((c1.cols <> c2.cols) or (c1.inc <> c2.inc))
		and ((c1.cols like c2.cols + '%' and SUBSTRING(c1.cols,LEN(c2.cols)+1,1) = ' ')
		or (c2.cols like c1.cols + '%' and SUBSTRING(c2.cols,LEN(c1.cols)+1,1) = ' '));
