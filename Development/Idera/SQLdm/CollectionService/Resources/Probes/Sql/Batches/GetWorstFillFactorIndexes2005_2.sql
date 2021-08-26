declare @dbname sysname
declare @dbid int
declare @dbio bigint
declare @dbsize bigint
declare @dynsql nvarchar(4000)

create table #result ([database_name] sysname,
					  [table_name] sysname, 
					  [index_name] sysname,
		  			  [schema_name] sysname, 
					  [fillfactor] int,
					  [datasizeinmb] int,
					  [indexsizeinmb] int);

IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
declare dblist cursor for 
	select top 10  
		vfs.database_id, 
		sum(vfs.num_of_bytes_read + vfs.num_of_bytes_written), 
		sum(0) 
		from sys.dm_io_virtual_file_stats(null, null) vfs
		--join sys.master_files m on vfs.database_id = m.database_id and vfs.file_id = m.file_id
		join sys.databases d on d.database_id = vfs.database_id
		where vfs.database_id > 4 and
			  d.compatibility_level > 65 and
			  d.state = 0
		group by vfs.database_id
		order by 2 desc

open dblist

fetch next from dblist into @dbid, @dbio, @dbsize
while @@FETCH_STATUS = 0
begin
	if (@dbsize > 10485760)
	begin		
		select @dbname = name from sys.databases where database_id = @dbid

		set @dynsql = 
			'declare @work table(table_name sysname, 
						object_id int,
						index_name sysname,  
						index_size_kb int,
						data_size_kb int, 
						type char(2), 
						owner sysname,
						[fillfactor] int);

		declare @dbsizemb bigint
		set @dbsizemb = ' + CONVERT(nvarchar(32), @dbsize / 1048576) + '  
		
		use [' + replace(@dbname,char(93),char(93)+char(93)) + '];
		with dailystats(TableName,TableID,DataSizeKB,TextSizeKB,used_pages,NumRows,TableType,TableOwner) as 
		(
			  select 
					o.name,
					o.object_id,
					''dataspace'' = sum(case when it.internal_type IN (202,204) then 0 when p.index_id < 2 then convert(dec(12,0),au.data_pages) else 0 end) * 8,
					''textused'' = sum(case when it.internal_type IN (202,204) then 0 when au.type != 1 then convert(dec(12,0),au.used_pages) else 0 end) * 8, 
					used_pages = sum(convert(dec(12,0),au.used_pages)) * 8, 
					sum(case when (p.index_id < 2) and (au.type = 1) then p.rows else 0 end),case when o.type = ''S'' then 1 else 0 end, ISNULL(s.name,''unknown'') 
			  from sys.allocation_units au 
			  left join sys.partitions p on (au.type = 2 and p.partition_id = au.container_id) OR (au.type IN (1,3) AND p.hobt_id = au.container_id) 
			  inner join sys.objects o on o.object_id = p.object_id 
			  left join sys.schemas s on o.schema_id = s.schema_id 
			  left join sys.internal_tables it on p.object_id = it.object_id 
			  where 
					o.type = ''U'' and o.is_ms_shipped = 0
			  group by 
					o.name, o.object_id, o.type, s.name
		)
		insert into @work
		select top 10
			  isnull(TableName,''''),
			  isnull(TableID, 0),
			  isnull(i.name, ''''),
			  case when used_pages < 0 then -1 else isnull((used_pages - DataSizeKB - TextSizeKB),0) end as IndexSizeKB,
			  case when used_pages < 0 then -1 else isnull(DataSizeKB,0) end as DataSizeKB,
			  isnull(TableType,''U''),
			  isnull(TableOwner,''unknown''),
			  OrigFillFactor
		from 
			  dailystats d 
			  left join sys.sysindexes i on d.TableID = i.id 
		where 
			i.OrigFillFactor > 0 and i.OrigFillFactor < 100 and indexproperty(TableID, i.name, ''IsHypothetical'') <> 1 and indexproperty(TableID, i.name, ''IsDisabled'') <> 1
		group by TableName,TableID,i.name,DataSizeKB,TextSizeKB,used_pages,NumRows,TableType,TableOwner,OrigFillFactor 
		order by DataSizeKB DESC

		insert into #result
		select db_name(), table_name, index_name, owner,  [fillfactor], data_size_kb / 1024, index_size_kb / 1024
			from @work'
			
		execute (@dynsql)
	end 
	fetch next from dblist into @dbid, @dbio, @dbsize
end

close dblist
deallocate dblist
END
select top 10 * from #result
where [indexsizeinmb] >= 10
order by [indexsizeinmb] desc

drop table #result
