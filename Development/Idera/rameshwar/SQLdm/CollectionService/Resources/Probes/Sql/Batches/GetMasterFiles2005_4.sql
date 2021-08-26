declare @autoshrink table([dbid] int, dbname sysname, is_auto_shrink_on bit, primary key ([dbid]))

insert into @autoshrink
select 
	i.database_id, 
	d.name,
	case when i.num_of_writes > 0 then d.is_auto_shrink_on else 0 end
from sys.dm_io_virtual_file_stats(null, null) i
left join sys.databases d on d.database_id = i.database_id
where i.file_id = 1

select 
m.database_id,
a.dbname,
file_name=m.[name],
m.physical_name,
m.[file_id],
m.[type],
m.type_desc,
m.[growth],
m.[is_percent_growth],
size=i.size_on_disk_bytes,
initial_size=m.[size],
m.max_size,
a.is_auto_shrink_on
from @autoshrink a
left join sys.master_files m on a.[dbid] = m.database_id
join sys.dm_io_virtual_file_stats(null, null) i on m.database_id = i.database_id and m.file_id = i.file_id
where m.database_id is not null
order by m.database_id

