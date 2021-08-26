--------------------------------------------------------------------------------
--  Batch: File group Batch 2005 --SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new Batch
--  Tables: #disk_drives,#filestats,#sqlperf,sysfiles,sysfilegroups,
--  Variables: [0] - List of databases
--------------------------------------------------------------------------------

if (select isnull(object_id('tempdb..#filestats'), 0)) = 0 
begin 
	create table #filestats (Fileid int, FSFileGroup int, TotalExtents int, UsedExtents int, FSName sysname, FSFileName nchar(520))
end
else
begin
	truncate table  #filestats
end

if (select isnull(object_id('tempdb..#sqlperf'), 0)) = 0 
begin 
	create table #sqlperf (
		databasename nvarchar(520),
		logsize float,
		logspaceused float,
		status int) 
	
    insert into #sqlperf exec('dbcc sqlperf (logspace)')
end

if (select isnull(object_id('tempdb..#filelistNew'), 0)) = 0 
begin 
	create table #filelistNew (
		databasename nvarchar(520),
		LogicalFilename sysname,
		IsDataFile bit,
		FilegroupName nvarchar(256),
		CurrentSize decimal(38,5),
		UsedSize decimal(38,5),
		ConfiguredMaxSize int,
		FreeSpaceOnDisk decimal(38,5),
		FilePath nvarchar(520),
		drivename nvarchar(520)) 
end
else
	truncate table #filelistNew


if (select isnull(object_id('tempdb..#filegroupsizes'), 0)) = 0 
begin 
	create table #filegroupsizes (
		databasename nvarchar(520),
		FilegroupName nvarchar(256),
		DataSize decimal(38,5),
		TextImageSize decimal(38,5),
		IndexSize decimal(38,5)) 
end
else
	truncate table #filegroupsizes



declare read_db_status insensitive cursor 
for 
	select 
		name,
		cast(cmptlevel as smallint)
	from 
		master..sysdatabases d(nolock) 
	where 
		lower(name) <> 'mssqlsystemresource'
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databasepropertyex(name, 'IsInLoad'),0) = 0 
		and isnull(databasepropertyex(name, 'IsSuspect'),0) = 0 
		and isnull(databasepropertyex(name, 'IsInRecovery'),0) = 0 
		and isnull(databasepropertyex(name, 'IsNotRecovered'),0) = 0 
		and isnull(databasepropertyex(name, 'IsOffline'),0) = 0 
		and isnull(databasepropertyex(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databasepropertyex(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databasepropertyex(name, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = d.dbid and l.request_session_id <> @@spid)
				)
			)
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @cmptlevel
while @@fetch_status = 0 
begin 

--------------------------------------------------------------------------------
--  DYNAMIC QUERY: Database Files
--  Tables: #disk_drives,#filestats,#sqlperf,sysfiles,sysfilegroups,
--  Returns:	
--		database name
--		file name
--		is data file
--		filegroup name
--		total current size in kb
--		total current used size in kb
--		configured maximum size in pages
--		free space on disk
--		file path
--		status bits
--------------------------------------------------------------------------------	
select @command = ' use ' + quotename(@dbname) 
+ '  
 IF ((IS_SRVROLEMEMBER(''sysadmin'') = 1) OR (IS_ROLEMEMBER(''db_owner'') = 1))
 BEGIN
   insert #filestats EXEC (''dbcc showfilestats'') 
 END
 ELSE
 BEGIN
   INSERT #filestats 
   SELECT f.fileid AS Fileid, f.groupid AS FileGroup, f.size/8 AS TotalExtents, fileproperty(f.name,''SpaceUsed'')/8 AS UsedExtents, 
          f.name AS Name, f.filename AS FileName
   FROM sys.sysfiles f
   JOIN sys.database_files db_f ON f.fileid = db_f.file_id
   JOIN sys.data_spaces ds ON ds.data_space_id = db_f.data_space_id
 END
 declare @TempSysfiles table(drivename nvarchar(256), length int, filename nvarchar(512), fileid smallint, name sysname, status int, growth int, maxsize int, size int, groupid smallint)
 declare @Sysfiles table(drivename nvarchar(256), filename nvarchar(512), fileid smallint, name sysname, status int, growth int, maxsize int, size int, groupid smallint)
 insert into @TempSysfiles select drivename = drive_letter, length = len(drive_letter),filename,fileid,name,status,growth,maxsize,size,groupid from sysfiles left join #disk_drives'
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' on lower(substring(filename,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default '
else
	select @command = @command + ' on lower(substring(filename,1,len(drive_letter))) = lower(drive_letter) '	
select @command = @command + ' insert into @Sysfiles
 select	drivename,filename,fileid,name,status,growth,maxsize,size,groupid from @TempSysfiles s1 where 
 length is null or (length = (select max(length) from @TempSysfiles s2 where s2.fileid = s1.fileid))
 insert into #filelistNew 
 select '+
	quotename(@dbname,'''') + ', 
	name, 
	case when f.status&1000000=0 then 1 else 0 end, 
	case when f.status&1000000=0 then groupname else ''n/a'' end,  
	TotalSize = case when f.status&1000000=0 then (convert(dec(38,5),TotalExtents) * 64) else 
		case when (select count(fileid) from sysfiles where status&1000000<>0) > 1 then convert(dec(38,5), size) * 8 else'
	
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename)  collate database_default = rtrim(db_name())  collate database_default) end '
else
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename) = rtrim(db_name())) end '	
select @command = @command + 'end, 
    UsedSize = case when f.status&1000000=0 then (convert(dec(38,5),UsedExtents) * convert(dec(38,5),64)) else (select max(convert(dec(38,5),logsize) * (convert(dec(38,5),logspaceused))/100 * 1024) from #sqlperf where rtrim(databasename) = rtrim(' + quotename(@dbname,'''') + ')) end, 
	ConfiguredMaxSize = convert(decimal(35,5),round(maxsize/128,2)), 
	diskfree = (select (convert(dec(38,5),d.unused_size)/1024) from #disk_drives d where  '
if (@cmptlevel > 70)
	select @command = @command +  ' upper(drivename) collate database_default = upper(d.drive_letter) collate database_default ), '
else
	select @command = @command + ' upper(drivename) = upper(d.drive_letter) ), '	
select @command = @command + '	
	filename, 
	drivename from @Sysfiles f left join sysfilegroups fg on f.groupid = fg.groupid left join #filestats fs on f.fileid = fs.Fileid 
 truncate table #filestats 
 ;with SizeStats(groupid,Data_Size,TextImage_Size,Used_Pages) as (select  data_space_id, Data_Size = sum(case when it.internal_type in (202,204) then 0 when p.index_id < 2 then convert(dec(38,5),au.data_pages) else 0 end) * 8,    
 TextImage_Size = sum(case when it.internal_type in (202,204) then 0 when au.type != 1 then convert(dec(38,5),au.used_pages) else 0 end) * 8,  Used_Pages = sum(convert(dec(38,5),au.used_pages)) * 8  
 from sys.partitions p left join sys.allocation_units au on p.partition_id = au.container_id left join sys.internal_tables it on p.object_id = it.object_id group by data_space_id) 
 insert into #filegroupsizes  select db_name(), sysfilegroups.groupname, DataSize = Data_Size, TextImageSize = TextImage_Size, Index_Size = (Used_Pages - Data_Size - TextImage_Size) from SizeStats left join sysfilegroups  
 on SizeStats.groupid = sysfilegroups.groupid where sysfilegroups.groupname is not null '

execute (@command)

fetch read_db_status into @dbname, @cmptlevel
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 



SELECT 
		databasename AS DatabaseName,
		LogicalFilename AS [FileName],
		IsDataFile,
		FilegroupName,
		CurrentSize/1024 AS InitialSizeMB,
		UsedSize/1024 AS SpaceUsedMB,
		(CurrentSize-UsedSize)/1024 AS AvailableSpaceMB,
		ConfiguredMaxSize AS MaxSizeMB,
		FreeSpaceOnDisk/1024 AS FreeDiskSpaceMB,
		FilePath,
		drivename AS DriveName
FROM #filelistNew

 -- drop the temporary tables 

if (OBJECT_ID('#totaldbsizes') IS NOT NULL)
	BEGIN 
		drop table #totaldbsizes
	END

if (OBJECT_ID('#disk_drives') IS NOT NULL)
	BEGIN 
		drop table #disk_drives
	END

