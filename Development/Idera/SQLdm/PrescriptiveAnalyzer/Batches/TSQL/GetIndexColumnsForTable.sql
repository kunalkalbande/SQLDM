----------------------------------------------------------------------------------------------
--  Return index column information.
--
--  Variables:
--    Table - The table to return index column information for.
--	
----------------------------------------------------------------------------------------------
use {Database};
declare @objid int;
set @objid = object_id({SchemaTable});
select 
	[IndexName]=i.name, 
	[IndexId]=i.index_id,
	[Columns]=(select c.name [data()] from sys.index_columns as ic
			left outer join sys.columns c on c.object_id = ic.object_id and ic.column_id = c.column_id 
			where ic.is_included_column = 0
			and ic.object_id = i.object_id
			and ic.index_id = i.index_id
			order by ic.key_ordinal
			for xml path('Column')),
	[Included]=(select c.name [data()] from sys.index_columns as ic
			left outer join sys.columns c on c.object_id = ic.object_id and ic.column_id = c.column_id 
			where ic.is_included_column = 1
			and ic.object_id = i.object_id
			and ic.index_id = i.index_id
			order by ic.key_ordinal
			for xml path('Column')),
	[IndexSize]=(select (isnull(sum(ps.used_page_count), 0) * 8192) from sys.dm_db_partition_stats ps where ps.object_id = i.object_id and ps.index_id = i.index_id)			
 from sys.indexes i 
		where i.object_id = @objid
		and i.is_hypothetical = 0
		and i.is_disabled = 0		
		and i.index_id > 0;
