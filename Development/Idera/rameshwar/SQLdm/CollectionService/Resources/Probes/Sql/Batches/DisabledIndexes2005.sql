select 
	[DatabaseName]=db_name(),
	[Schema]= (Select top 1 s.name from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id where o.object_id = object_id), 
	[TableName]=object_name(object_id), 
	[IndexName]=name, 
	index_id, 
	is_disabled, 
	is_hypothetical  
	from sys.indexes 
		where (objectproperty(object_id, 'IsMsShipped') = 0)
		and (indexproperty(object_id, name, 'IsFulltextKey') <> 1)
		and (is_disabled = 1 or is_hypothetical = 1)