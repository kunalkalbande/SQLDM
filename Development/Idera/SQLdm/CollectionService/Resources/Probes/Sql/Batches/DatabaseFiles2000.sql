--------------------------------------------------------------------------------
--  Batch: Database Files
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

if (select isnull(object_id('tempdb..#filelist'), 0)) = 0 
begin 
	create table #filelist (
		databasename nvarchar(520),
		LogicalFilename sysname,
		IsDataFile int,
		FilegroupName nvarchar(256),
		ConfiguredGrowth int,
		CurrentSize decimal(38,5),
		UsedSize decimal(38,5),
		ConfiguredMaxSize int,
		ExpansionSpace decimal(38,5),
		FreeSpaceOnDisk decimal(38,5),
		FilePath nvarchar(520),
		StatusBits int,
		drivename nvarchar(520)) 
end
else
	truncate table #filelist


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
		and name in ({0}) 
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
 insert into @TempSysfiles select drivename = drive_letter,length = len(drive_letter),filename,fileid,name,status,growth,maxsize,size,groupid from sysfiles left join	#disk_drives'
 	
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' on lower(substring(filename,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default '
else
	select @command = @command + ' on lower(substring(filename,1,len(drive_letter))) = lower(drive_letter) '
 select @command = @command + 'insert into @Sysfiles
 select	drivename,filename,fileid,name,status,growth,maxsize,size,groupid from @TempSysfiles s1 where
length is null or (length = (select max(length) from @TempSysfiles s2 where s2.fileid = s1.fileid))
 insert into #filelist 
select '
+  quotename(@dbname,'''') + ', 
	name, 
 case when f.status&1000000=0 then 1 else 0 end, case when f.status&1000000=0 then groupname else ''n/a'' end, growth, 
TotalSize = case when f.status&1000000=0 then (convert(dec(38,5),TotalExtents) * 64) else 
 case when (select count(fileid) from sysfiles where status&1000000<>0) > 1 
		then convert(dec(38,5), size) * 8 
	else'
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename) collate database_default = rtrim(db_name()) collate database_default) end '
else
	select @command = @command + ' (select max(convert(dec(38,5),logsize)) * 1024 from #sqlperf where rtrim(databasename) = rtrim(db_name())) end '

select @command = @command + '	end, 
 UsedSize = case when f.status&1000000=0 then (convert(dec(38,5),UsedExtents) * convert(dec(38,5),64)) else (select max(convert(dec(38,5),logsize) * (convert(dec(38,5),logspaceused))/100 * 1024) from #sqlperf where rtrim(databasename) = rtrim(' + quotename(@dbname,'''') + ')) end, 
 configuredmaxsize = maxsize, 
 expansionsize = ( select convert(dec(38,5),isnull( case when maxsize > 0 and (convert(dec(38,5),maxsize)*8)-(convert(dec(38,5),size)*8) <= (convert(dec(38,5),d.unused_size)/1024) then (convert(dec(38,5),maxsize)*8)-(convert(dec(38,5),size)*8) 
 when growth = 0 or maxsize = 0 or (maxsize = -1 and growth = 0) then 0 else (convert(dec(38,5),d.unused_size)/1024) end , 0)) 
 from #disk_drives d where  '
-- Protect againt using "collate" on SQL 7 compatible dbs
if (@cmptlevel > 70)
	select @command = @command + ' upper(drivename) collate database_default = upper(d.drive_letter) collate database_default ), '
else
	select @command = @command + ' upper(drivename) = upper(d.drive_letter) ), '
select @command = @command + '	diskfree = (select (convert(dec(38,5),d.unused_size)/1024) from #disk_drives d where  '
if (@cmptlevel > 70)
	select @command = @command +  '	upper(drivename) collate database_default = upper(d.drive_letter) collate database_default ), '
else
	select @command = @command + ' upper(drivename) = upper(d.drive_letter) ), '		
select @command = @command + '	filename, f.status,drivename from @Sysfiles f left join sysfilegroups fg on f.groupid = fg.groupid left join #filestats fs on f.fileid = fs.Fileid 
 truncate table #filestats 
insert into #filegroupsizes 
select db_name(), sysfilegroups.groupname, Data_Size = (sum(case when indid in (0, 1) then convert(dec(38,5),dpages) else 0 end) * 8), TextImage_Size = (sum(case when indid = 255 then convert(dec(38,5),used) else 0 end) * 8),   
Index_Size = (sum(case when indid in (0, 1) then convert(dec(38,5),used) - convert(dec(38,5),dpages) else 0 end) * 8) from sysindexes left join sysfilegroups on sysindexes.groupid = sysfilegroups.groupid 
where sysfilegroups.groupname is not null group by sysindexes.groupid, sysfilegroups.groupname '

execute (@command)

	fetch read_db_status into @dbname,   @cmptlevel
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 


select * from #filelist

select
	f.databasename,
	f.FilegroupName,
	files = count(f.FilegroupName),
	CurrentSize = sum(CurrentSize),
	UsedSize = sum(f.UsedSize),
	autogrow = sum(distinct(case 
				when ConfiguredGrowth <> 0 and IsDataFile = 1 then 1 
				when ConfiguredGrowth = 0 and IsDataFile = 1 then 4
				when ConfiguredGrowth <> 0 and IsDataFile = 0 then 8 
				when ConfiguredGrowth = 0 and IsDataFile = 0 then 16
				else 0 end)),
	expansion = sum(distinct(expansion.expansionspace)),
	DataSize = max(isnull(fs.DataSize,0)),
	TextImageSize = max(isnull(fs.TextImageSize,0)),
	IndexSize = max(isnull(fs.IndexSize,0))
from
#filelist f
left join 
#filegroupsizes fs
on f.databasename = fs.databasename
and f.FilegroupName = fs.FilegroupName
left join
(select
	databasename,
	FilegroupName,
	drivename,
	expansionspace = case when sum(ExpansionSpace) > FreeSpaceOnDisk then FreeSpaceOnDisk else sum(ExpansionSpace) end
from
	#filelist f2
where 
	FilegroupName <> 'n/a'
group by
	databasename,
	FilegroupName,
	drivename,
	FreeSpaceOnDisk
) as expansion
on f.databasename = expansion.databasename
and f.FilegroupName = expansion.FilegroupName
and f.drivename = expansion.drivename
where 
	f.FilegroupName <> 'n/a'
group by f.databasename, f.FilegroupName



drop table #disk_drives