--------------------------------------------------------------------------------
--  Batch: Database Size 2005
--  Variables: None
--------------------------------------------------------------------------------
-- Depends on these variables, previously declared:
--	@command nvarchar(2048), 
--	@dbname nvarchar(255), 
-- 	@cmptlevel int, 

use master

declare 
	@tabid int, 
	@lock_type smallint,
	@dbid smallint, 
	@mode smallint, 
	@log_size varchar(20), 
	@total_db_size varchar(20), 
	@category int, 
	@dumptrdate varchar(18), 
	@spid int,
	@systemdatabase bit,
	@databasestatus int,
	@collectsize bit,
	@logexpansion dec(38,0),
	@dbexpansion dec(38,0),
	@crdate datetime
 
if (select isnull(object_id('tempdb..#intermediate_sysfiles'), 0)) = 0  
	create table #intermediate_sysfiles (
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize integer, 
		size integer, 
		growth integer,
		name nvarchar(256), 
		filename nvarchar(1000),
		fileid int) 
else	
	truncate table #intermediate_sysfiles 

if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
	create table #totaldbsizes (usedsize dec(38,0)) 
else 
	truncate table #totaldbsizes 

declare @grouped_devices table(
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize integer, 
		size integer, 
		growth integer, 
		expansion dec(38,0)) 

declare read_db_status insensitive cursor 
for 
	select 
		d.name,
		d.dbid, 
		d.mode, 
		case when d.name in ('master','model','msdb','tempdb') then cast(1 as bit) when d.category & 16 = 16 then cast(1 as bit) else cast(0 as bit) end,
		crdate,
		cmptlevel
	from 
		master..sysdatabases d (nolock) 
		left join master.sys.databases db (nolock) on d.dbid = db.database_id
	where lower(d.name) <> 'mssqlsystemresource'
	order by user_access, status
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @cmptlevel
while @@fetch_status = 0 
begin 
	
	select @databasestatus  = 0, @collectsize = 0

	-- Add status bits
	select @databasestatus = 
		case when isnull(mirr.mirroring_role,0) = 2 and isnull(db.state,0) = 1 then 8 else 0 end								--Restoring Mirror
		+ case when isnull(databasepropertyex(@dbname,'IsInStandby'),0) = 1 or db.is_in_standby = 1 then 16 else 0 end 			--Standby
		+ case when isnull(mirr.mirroring_role,0) <> 2 and (isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 1 
			or isnull(db.state,0) = 1) then 32 else 0 end																		--Restoring (non-mirror)
		+ case when isnull(db.state,0) = 3 then 64 else 0 end																	--Pre-Recovery
		+ case when isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 1 or isnull(db.state,0) = 2 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 1 or isnull(db.state,0) = 4 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@dbname, 'IsOffline'),0) = 1 or isnull(db.state,0) = 6 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@dbname, 'IsReadOnly'),0) = 1 or isnull(db.is_read_only,0) = 1 then 1024 else 0 end --Read Only
		+ case when isnull(databaseproperty(@dbname, 'IsDboOnly'),0) = 1 then 2048 else 0 end									--DBO Use
		+ case when isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 or db.user_access = 1 then 4096 else 0 end			--Single User
		+ case when isnull(databaseproperty(@dbname, 'IsEmergencyMode'),0) = 1 or isnull(db.state,0) = 5 then 32768 else 0 end	--Emergency Mode	
		+ case when isnull(db.is_cleanly_shutdown,0) = 1 then 1073741824 else 0 end												--Cleanly Shutdown
	from 
		master.sys.databases db
		left outer join sys.database_mirroring mirr 
		on mirr.database_id = db.database_id
	where 
		db.database_id = @dbid

	-- Decide whether to collect size information
	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
			or (@databasestatus & 4096 = 4096 
				and not exists 
				(select * from master..sysprocesses p where dbid = @dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = @dbid and l.request_session_id <> @@spid)
			))  --Single User
		)
			select @collectsize = 1

	-- If we cannot access database, but there is no known reason, set database to Inacessible
	if	@collectsize = 1 and (has_dbaccess (@dbname) <> 1 or @mode <> 0)
		select @databasestatus = @databasestatus + 8192, @collectsize = 0
	
	if @collectsize = 1
	begin 
		select @command = 'insert into #totaldbsizes select sum(convert(dec(38,0),used_pages))*8 from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ '].sys.allocation_units'
			
			execute(@command)

		if (exists(select * from #disk_drives where len(drive_letter) > 1))
		begin
			select @command = 'use [' + replace(@dbname,char(93),char(93)+char(93)) + ']
			truncate table #intermediate_sysfiles
			declare @MatchMountPoints table(fileid bigint, drive_letter nvarchar(256), length int, status int,  maxsize dec(38,0),size dec(38,0), growth int, name nvarchar(256), filename nvarchar(256))

			insert into @MatchMountPoints
			select
				fileid,
				drive_letter,
				length = len(drive_letter),
				status,
				maxsize,
				size, 
				growth,
				name,
				filename
			from 
				sysfiles
				left join
				#disk_drives
				on lower(substring(filename,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default

			insert into #intermediate_sysfiles 
			select 
					upper(drive_letter) as driveletter, 
					case when status&1000000 = 0 then 1 else 0 end as datafile, 
					maxsize, 
					size, 
					growth,
					name, 
					filename, 
					fileid
			from @MatchMountPoints m1
			where 
				length  = (select max(length) from @MatchMountPoints m2 where m2.fileid = m1.fileid)'
		end
		else
		begin

			select @command = 'truncate table #intermediate_sysfiles insert into #intermediate_sysfiles 
			select upper(substring(filename, 1, 1)) as driveletter, case when status&1000000 = 0 then 1 else 0 end as datafile, 
			maxsize, size, growth, name, filename, fileid from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysfiles'
		end
	
	execute(@command)

	delete from @grouped_devices 
		insert into @grouped_devices 
			select 
				driveletter, 
				filetypeflag, 
				maxsize, 
				size, 
				growth, 
				0
			from 
				#intermediate_sysfiles
			where 
				maxsize <> -1
			UNION 
			select 
				driveletter, 
				filetypeflag, 
				-1, 
				sum(convert(dec(38,0),size)), 
				sum(convert(dec(38,0),growth)), 
				0
			from 
				#intermediate_sysfiles
			where 
				maxsize = -1 
				and growth <> 0 
			group by 
				driveletter, filetypeflag
			UNION 
			select 
				driveletter, 
				filetypeflag, 
				-1, 
				sum(convert(dec(38,0),size)), 
				0, 
				0
			from 
				#intermediate_sysfiles
			where 
				maxsize = 0 
				or (maxsize = -1 and growth = 0) 
			group by 
				driveletter, filetypeflag

			update @grouped_devices 
				set expansion = 
					isnull(
						CASE 
						when maxsize > 0 and (convert(dec(38,0),maxsize)*8)-(convert(dec(38,0),size)*8) <= (convert(dec(38,0),d.unused_size)/1024) 
							then (convert(dec(38,0),maxsize)*8)-(convert(dec(38,0),size)*8)
						when growth = 0 or maxsize = 0 or (maxsize = -1 and growth = 0) 
							then 0 
						else (convert(dec(38,0),d.unused_size)/1024) 
						end , 0)
					from 
						#disk_drives d
					where 
						driveletter collate database_default = upper(d.drive_letter collate database_default)


			if exists(select driveletter from @grouped_devices where driveletter is null and filetypeflag = 0)
			begin
						select @logexpansion = null
			end			
			else
			begin
				
				select 
					@logexpansion = sum(convert(dec(38,0),expansion))
				from 
					(
					select expansion = 
						case when (sum(convert(dec(38,0),expansion)) > sum(convert(dec(38,0),d.unused_size)/1024))
							then min(convert(dec(38,0),d.unused_size)/1024)
						else
							sum(convert(dec(38,0),expansion)) 
						end
					from 
						@grouped_devices 
						left join #disk_drives d
						on driveletter collate database_default = upper(d.drive_letter collate database_default)
					where filetypeflag = 0 
					group by driveletter, d.drive_letter) g
			end


			if exists(select driveletter from @grouped_devices where driveletter is null and filetypeflag <> 0)
			begin
						select @dbexpansion = null
			end			
			else
			begin

				select 
					@dbexpansion = sum(convert(dec(38,0),expansion))
				from 
					(
					select expansion = 
						case when (sum(convert(dec(38,0),expansion)) > sum(convert(dec(38,0),d.unused_size)/1024))
							then min(convert(dec(38,0),d.unused_size)/1024)
						else
							sum(convert(dec(38,0),expansion)) 
						end
					from 
						@grouped_devices 
						left join #disk_drives d
						on driveletter collate database_default = upper(d.drive_letter collate database_default)
					where filetypeflag <> 0 
					group by driveletter, d.drive_letter) g
			end
			--SQLDM-30391

					Declare @count int,
					@currentRow int

					Select @count = Count(*) from #intermediate_sysfiles
					Select @currentRow = 1
					while @currentRow <= @count
					Begin
						if(exists(select * from sys.dm_os_volume_stats(1,@currentRow) where len(volume_mount_point) > 3))
						BEGIN
						Select @dbexpansion = isnull(
									CASE 
									when max_size > 0 and (convert(dec(38,0),max_size)*8)-(convert(dec(38,0),size)*8) <= (convert(dec(38,0),available_bytes)/1024) 
										then (convert(dec(38,0),max_size)*8)-(convert(dec(38,0),size)*8)
									when growth = 0 or max_size = 0 or (max_size = -1 and growth = 0) 
										then 0 
									else (convert(dec(38,0),available_bytes)/1024) 
									end , 0)
									from sys.database_files f join Sys.dm_os_volume_stats(@dbid, @currentRow) v on f.file_id =  v.file_id;
								END
						set @currentRow = @currentRow + 1
					End
			select @command = case when @cmptlevel >= 80 then 'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] ' else ' ' end 
			+';with SizeStats(Data_Size,TextImage_Size,Used_Pages) as (select 
			Data_Size = sum(case when it.internal_type in (202,204) then 0 when p.index_id < 2 then convert(dec(38,0),au.data_pages) else 0 end) * 8,   
			TextImage_Size = sum(case when it.internal_type in (202,204) then 0 when au.type != 1 then convert(dec(38,0),au.used_pages) else 0 end) * 8, 
			Used_Pages = sum(convert(dec(38,0),au.used_pages)) * 8 
			from ['	+ replace(@dbname,char(93),char(93)+char(93)) + '].sys.partitions p 
			left join ['	+ replace(@dbname,char(93),char(93)+char(93)) + '].sys.allocation_units au on p.partition_id = au.container_id  
			left join ['	+ replace(@dbname,char(93),char(93)+char(93)) + '].sys.internal_tables it on p.object_id = it.object_id )
			select [Database Name] = @dbnameIN,
			[Database Status] = @databasestatusIN,
			[Allocated DBSize] = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from [' 
				+ replace(@dbname,char(93),char(93)+char(93)) + ']..sysfiles (nolock) where status&1000000 = 0), 
			[Data Size] = convert(dec(38,0),Data_Size),
			[Text Size] = convert(dec(38,0),TextImage_Size),
			[Index Size] = convert(dec(38,0),(Used_Pages - Data_Size - TextImage_Size)), 
			[System Database] = cast(@systemdatabaseIN as bit), 
			[Log Expansion] = convert(dec(38,0),@logexpansionIN), 
			[Db Expansion] = convert(dec(38,0),@dbexpansionIN),
			[Total Used]= ' +  case when @cmptlevel >= 80 then '(select sum(convert(dec(38,0), fileproperty(name,''SpaceUsed'') ) * 8) from [' 
				+ replace(@dbname,char(93),char(93)+char(93)) 
				+ ']..sysfiles where status&1000000 = 0),'
				else 'null, ' end 
			+ ' [crdate] = cast( @crdateIN as datetime)
			from SizeStats'

			declare @parmdef nvarchar(1000)

			set @parmdef = '@dbnameIN sysname, @databasestatusIN int, @systemdatabaseIN bit, @logexpansionIN dec(38,0), @dbexpansionIN dec(38,0), @crdateIN datetime'

			exec sp_executesql @command, @parmdef, @dbnameIN = @dbname, @databasestatusIN= @databasestatus, @systemdatabaseIN = @systemdatabase, @logexpansionIN = @logexpansion, @dbexpansionIN = @dbexpansion, @crdateIN = @crdate

			select 
				DatabaseName = @dbname,
				NumberReads = cast(vf.NumberReads as dec(38,0)),
				NumberWrites = cast(vf.NumberWrites as dec(38,0)),
				driveletter,
				f.name,
				cast(filetypeflag as int),
				filename,
				driveletter
			from 
				#intermediate_sysfiles f
				left join ::fn_virtualfilestats(-1,-1) vf
				on vf.FileId = f.fileid	
			where 
				vf.DbId = @dbid

	end --End if @collectsize = 1
	else 
	begin -- if @collectsize <> 1
			select 
				@dbname,
				@databasestatus, 
				null,
				null,
				null,
				null,
				@systemdatabase,
				null,
				null,
				null,
				@crdate
	end 
	fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @cmptlevel
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 

select 'end of db scan'

declare @templogsize dec(38,0), @tempdevsize dec(38,0)

select @templogsize = (cast(size as dec(38,0)) * 8) 
from tempdb.sys.database_files
where type = 1

select @tempdevsize = (cast(size as dec(38,0)) * 8)
from tempdb.sys.database_files
where type = 0

----------------------------------------------------------------------------------
---- QUERY: Database summary information
---- Tables: master..sysdatabases, tempdb..#totaldbsizes
---- Returns:
---- Total number of databases
---- Number of data files
---- Number of log files
---- Total data file space allocated
---- Total log file space allocated
---- Total data file space used
----------------------------------------------------------------------------------
select
'summary',
(select
count(*)
from
master..sysdatabases)
,sum(case when type = 0 then 1 else 0 end )
,sum(case when type = 1 then 1 else 0 end )
,sum(case
when type = 0 and database_id <> db_id('tempdb')
then isnull(convert(dec(38,0),size) * 8,0)
when type = 0 and database_id = db_id('tempdb')
then @tempdevsize
else 0 end )
,sum(case
when type = 1 and database_id <> db_id('tempdb')
then isnull(convert(dec(38,0),size) * 8,0)
when type = 1 and database_id = db_id('tempdb')
then @templogsize
else 0 end )
,(select sum(usedsize) from #totaldbsizes)
from master.sys.master_files

drop table #totaldbsizes

drop table #disk_drives

DBCC SQLPERF (LOGSPACE) 

