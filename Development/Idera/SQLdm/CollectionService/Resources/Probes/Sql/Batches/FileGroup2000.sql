--------------------------------------------------------------------------------
--  Batch: File group Batch 2000 --SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new Batch
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
		-- 10.0 (vineet) --   FilePath column was missing in create table. 
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
		and cmptlevel > 60
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databaseproperty(name, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(name, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(name, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(name, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(name, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(name, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
				and not exists
				(select * from master..syslockinfo l where rsc_dbid = d.dbid and l.req_spid <> @@spid)
				)
			)
		and status & 32 <> 32 
		and status & 64 <> 64 
		and status & 128 <> 128 
		and status & 256 <> 256 
		and status & 512 <> 512 
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname,   @cmptlevel
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
--		growth (pages or percent)
--		total current size in kb
--		total current used size in kb
--		configured maximum size in pages
--		size available for expansion
--		free space on disk
--		file path
--		status bits
--------------------------------------------------------------------------------	
select @command = ' use ' + quotename(@dbname) 
+ ' insert #filestats EXEC (''dbcc showfilestats'')
 declare @TempSysfiles table(drivename nvarchar(256), length int, filename nvarchar(512), fileid smallint, name sysname, status int, growth int, maxsize int, size int, groupid smallint)
 declare @Sysfiles table(drivename nvarchar(256), filename nvarchar(512), fileid smallint, name sysname, status int, growth int, maxsize int, size int, groupid smallint)
 insert into @TempSysfiles select drivename = drive_letter,length = len(drive_letter),filename,fileid,name,status,growth,maxsize,size,groupid from sysfiles 
 '
 	
-- Protect againt using "collate" on SQL 7 compatible dbs
 -- 10.0 (vineet) if cmptlevel is <80 replace joins as joins does not work int this version
if (@cmptlevel > 70)
	select @command = @command + ' left join	#disk_drives on lower(substring(filename,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default '
else
	select @command = @command + ' ,	#disk_drives where lower(substring(filename,1,len(drive_letter))) = lower(drive_letter) '

 select @command = @command + 'insert into @Sysfiles
 select	drivename,filename,fileid,name,status,growth,maxsize,size,groupid from @TempSysfiles s1 where
length is null or (length = (select max(length) from @TempSysfiles s2 where s2.fileid = s1.fileid))
 insert into #filelistNew 
select ' +  
	quotename(@dbname,'''') + ', 
	name, 
	case when f.status&1000000=0 then 1 else 0 end, 
	case when f.status&1000000=0 then groupname else ''n/a'' end, 
	TotalSize = case when f.status&1000000=0 then (convert(dec(38,5),TotalExtents) * 64) else 
		case when (select count(fileid) from sysfiles where status&1000000<>0) > 1 then convert(dec(38,5), size) * 8 else'
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename) collate database_default = rtrim(db_name()) collate database_default) end '
else
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename) = rtrim(db_name())) end '

select @command = @command + '	end, 
		UsedSize = case when f.status&1000000=0 then (convert(dec(38,5),UsedExtents) * convert(dec(38,5),64)) else (select max(convert(dec(38,5),logsize) * (convert(dec(38,5),logspaceused))/100 * 1024) from #sqlperf where rtrim(databasename) = rtrim(' + quotename(@dbname,'''') + ')) end, 
		ConfiguredMaxSize = convert(decimal(35,5),round(maxsize/128,2)), 
		diskfree = (select (convert(dec(38,5),d.unused_size)/1024) from #disk_drives d where  '

-- 10.0 (vineet) if cmptlevel is <80 replace joins as joins does not work int this version
if (@cmptlevel > 70)
	select @command = @command +  '	upper(drivename) collate database_default = upper(d.drive_letter) collate database_default ), 
	filename, 
	drivename from @Sysfiles f left join sysfilegroups fg on f.groupid = fg.groupid left join #filestats fs on f.fileid = fs.Fileid'
else
	select @command = @command + ' upper(drivename) = upper(d.drive_letter) ), 
	filename, 
	drivename from @Sysfiles f , sysfilegroups fg, #filestats fs where f.groupid = fg.groupid  and f.fileid = fs.Fileid'		
select @command = @command + '	
	 
 truncate table #filestats 
insert into #filegroupsizes 
select db_name(), sysfilegroups.groupname, Data_Size = (sum(case when indid in (0, 1) then convert(dec(38,5),dpages) else 0 end) * 8), TextImage_Size = (sum(case when indid = 255 then convert(dec(38,5),used) else 0 end) * 8),   
Index_Size = (sum(case when indid in (0, 1) then convert(dec(38,5),used) - convert(dec(38,5),dpages) else 0 end) * 8) '

-- 10.0 (vineet) if cmptlevel is <80 replace joins as joins does not work int this version
if (@cmptlevel > 70)
	select @command = @command + ' from sysindexes left join sysfilegroups on sysindexes.groupid = sysfilegroups.groupid 
	where sysfilegroups.groupname is not null group by sysindexes.groupid, sysfilegroups.groupname '
else
	select @command = @command + ' from sysindexes , sysfilegroups where sysindexes.groupid = sysfilegroups.groupid 
	and sysfilegroups.groupname is not null group by sysindexes.groupid, sysfilegroups.groupname '

execute (@command)

	fetch read_db_status into @dbname,   @cmptlevel
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
