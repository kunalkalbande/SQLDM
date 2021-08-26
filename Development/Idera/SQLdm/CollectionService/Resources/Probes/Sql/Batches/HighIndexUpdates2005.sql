declare @dbid int
select @dbid = db_id()

select top (100) 
	[DatabaseName]=db_name(), 
	[Schema]= (Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = s.object_id),
	[TableName]=object_name(s.object_id), 
	[IndexName]=i.name,
	[UserReads]=user_seeks + user_scans + user_lookups,
	[UserWrites]=user_updates,
	[WritesPerRead]=case (s.user_seeks + s.user_scans + s.user_lookups) when 0 then s.user_updates else cast((1.0 * s.user_updates) / (s.user_seeks + s.user_scans + s.user_lookups) as numeric(15,2)) end,
	[DaysSinceTableCreated]=cast(isnull((select datediff(d,create_date,getdate()) from sys.tables where object_id = s.object_id), 0) as bigint)
	from sys.dm_db_index_usage_stats s
		join sys.indexes i on s.database_id = @dbid
			and s.index_id = i.index_id
			and s.object_id = i.object_id
		where s.database_id = @dbid
		and s.index_id > 1 
		and s.user_updates > 100
		and objectproperty(s.object_id,'IsMSShipped') = 0
		and objectproperty(s.object_id,'IsUserTable') = 1
		and (indexproperty(i.object_id, i.name, 'IsFulltextKey') <> 1)
		and i.is_unique = 0
		and i.is_primary_key = 0
		and i.is_unique_constraint = 0
		and (s.user_seeks + s.user_scans + s.user_lookups) < s.user_updates
	order by [WritesPerRead] desc

