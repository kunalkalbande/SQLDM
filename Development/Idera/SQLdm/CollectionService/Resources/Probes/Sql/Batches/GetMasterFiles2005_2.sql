--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

-- START (RRG): Can not be called in Azure SQL Database

--declare @autoshrink table([dbid] int, dbname sysname, is_auto_shrink_on bit, primary key ([dbid]))

--insert into @autoshrink
--select 
--	i.database_id, 
--	d.name,
--	case when i.num_of_writes > 0 then d.is_auto_shrink_on else 0 end
--from sys.dm_io_virtual_file_stats(null, null) i
--left join sys.databases d on d.database_id = i.database_id
--where i.file_id = 1 and d.name IS NOT NULL

--select 
--m.database_id,
--a.dbname,
--file_name=m.[name],
--m.physical_name,
--m.[file_id],
--m.[type],
--m.type_desc,
--m.[growth],
--m.[is_percent_growth],
--size=i.size_on_disk_bytes,
--initial_size=m.[size],
--m.max_size,
--a.is_auto_shrink_on
--from @autoshrink a
--left join sys.master_files m on a.[dbid] = m.database_id
--join sys.dm_io_virtual_file_stats(null, null) i on m.database_id = i.database_id and m.file_id = i.file_id
--where m.database_id is not null
--order by m.database_id

DECLARE @tempVar_GetMasterFiles2005_2 TABLE
(
	[database_id] int, 
	[dbname] nvarchar(1024),
	[file_name] nvarchar(1024),
	[physical_name] nvarchar(1024),
	[file_id] int,
	[type] tinyint,
	[type_desc] nvarchar(60),
	[growth] int,
	[is_percent_growth] bit,
	[size] int,
	[initial_size] int,
	[max_size] int,
	[is_auto_shrink_on] bit
)
INSERT INTO @tempVar_GetMasterFiles2005_2 VALUES
(
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
    NULL
) 
SELECT * FROM @tempVar_GetMasterFiles2005_2