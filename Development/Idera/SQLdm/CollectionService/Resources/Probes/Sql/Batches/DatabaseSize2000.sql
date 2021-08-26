--------------------------------------------------------------------------------
--  Batch: Database Size 2000
--  Variables: 
-- [0] -- #disk_drive filter -WHERE
-- [1] -- #disk_drive dynamic filter -WHERE
-- [2] -- #disk_drive filter -AND 
--------------------------------------------------------------------------------
-- Depends on these variables, previously declared:
--	@command nvarchar(2048), 
--	@dbname nvarchar(255), 
--	@cmptlevel int, 

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
	@crdate datetime,
	@lastbackup datetime,
	@roleAG int

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
	create table #totaldbsizes (
		datasize dec(38,0),
		logsize dec(38,0),
		datafilecount int,
		logfilecount int,
		usedsize dec(38,0)) 
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
		name, 
		dbid, 
		mode, 
		case when name in ('master','model','msdb','tempdb') then cast(1 as bit) when category & 16 = 16 then cast(1 as bit) else cast(0 as bit) end,
		crdate,
		( 
		select 
		 max(backup_finish_date) 
		from 
		 msdb..backupset 
		where 
		 type <> 'F' 
		 and database_name = d.name
		 collate database_default
		) as lastbackup, 
		cmptlevel,
		NULL as roleAG
	from 
		master..sysdatabases d (nolock) 
	order by
		isnull(databaseproperty(name, 'IsSingleUser'),0),
		status
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @lastbackup, @cmptlevel, @roleAG
while @@fetch_status = 0 
begin 
	
	select @databasestatus  = 0, @collectsize = 0

	-- Add status bits
	select @databasestatus = 
		case when isnull(databasepropertyex(@dbname,'IsInStandby'),0) = 1 then 16 else 0 end 									--Standby
		+ case when isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 1 or db.status & 32 = 32 then 32 else 0 end				--Restoring (non-mirror)
		+ case when db.status & 64 = 64 then 64 else 0 end																			--Pre-Recovery
		+ case when isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 1 or db.status & 128 = 128 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 1 or db.status & 256 = 256 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@dbname, 'IsOffline'),0) = 1 or db.status & 512 = 512 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@dbname, 'IsReadOnly'),0) = 1 or db.status & 1024 = 1024 then 1024 else 0 end		--Read Only
		+ case when isnull(databaseproperty(@dbname, 'IsDboOnly'),0) = 1 or db.status & 2048 = 2048 then 2048 else 0 end		--DBO Use
		+ case when isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 or db.status & 4096 = 4096 then 4096 else 0 end		--Single User
		+ case when isnull(databaseproperty(@dbname, 'IsEmergencyMode'),0) = 1 or db.status & 32768 = 32768 then 32768 else 0 end	--Emergency Mode	
		+ case when db.status & 1073741824 = 1073741824 then 1073741824 else 0 end												--Cleanly Shutdown
	from 
		master..sysdatabases db
	where 
		db.dbid = @dbid

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
				(select * from master..syslockinfo l where rsc_dbid = @dbid and l.req_spid <> @@spid)
			))  --Single User
		)
			select @collectsize = 1

	-- If we cannot access database, but there is no known reason, set database to Inacessible
	if	@collectsize = 1 and (has_dbaccess (@dbname) <> 1 or @mode <> 0)
		select @databasestatus = @databasestatus + 8192, @collectsize = 0
	
	if @collectsize = 1
	begin 
		select @command = 'insert into #totaldbsizes select (select isnull(sum(convert(dec(38,0),size) * 8), 0) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 = 0), (select isnull(sum(convert(dec(38,0),size) * 8), 0) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 <> 0),'
			+'(select count(*) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 = 0), (select count(*) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 <> 0),'
			+ '(select isnull(sum(convert(dec(38,0),used) * 8), 0) from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysindexes (nolock) where indid in (0, 1, 255))'
		execute(@command)

		declare @databaseindicator varchar(10)
		select @databaseindicator = case when databaseproperty(@dbname, 'IsInStandBy') = 1 then 'Standby' else 'Database' end

		if (exists(select * from #disk_drives {0} ))
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
					sysfiles ' + case when @cmptlevel = 60 then ' , ' else ' left join ' end +
					
					' #disk_drives ' +
					case when @cmptlevel = 60 then ' where ' else ' on ' end +
					' lower(substring(filename,1,len(drive_letter))) ' + case when @cmptlevel > 70 then ' collate database_default ' else ' ' end + ' = lower(drive_letter) ' + case when @cmptlevel > 70 then ' collate database_default ' else ' ' end + '
					{1}
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
						{2}

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
					where filetypeflag = 0 {2}
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
					where filetypeflag <> 0 {2}
					group by driveletter, d.drive_letter) g
			end

		select @command = case when @cmptlevel >= 80 then 'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] ' else ' ' end 
			+ 'select [Database Name] = ''' + replace(@dbname,char(39),char(39)+char(39)) + ''', 
			[Database Status] = ' + convert(varchar(10),@databasestatus) + ', 
			[Allocated DBSize] = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 = 0), 
			[Data Size] = (sum(case when indid in (0, 1) then convert(dec(38,0),dpages) else 0 end) * 8), 
			[Text Size] = (sum(case when indid = 255 then convert(dec(38,0),used) else 0 end) * 8), 
			[Index Size] = (sum(case when indid in (0, 1) then convert(dec(38,0),used) - convert(dec(38,0),dpages) else 0 end) * 8), 
			[System Database] = cast(' + cast(@systemdatabase as nvarchar(1)) + ' as bit), 
			[Log Expansion] = convert(dec(38,0),' + isnull(cast(@logexpansion as nvarchar(30)), 'null') + '), 
			[Db Expansion] = convert(dec(38,0),' + isnull(cast(@dbexpansion as nvarchar(30)), 'null') + '),
			[Total Used]= '
				+ case when @cmptlevel >= 80 then '(select sum(convert(dec(38,0), fileproperty(name,''SpaceUsed'') ) * 8) from [' 
				+ replace(@dbname,char(93),char(93)+char(93)) 
				+ ']..sysfiles where status&1000000 = 0), '
				else 'null, ' end 
			+ ' [crdate] = cast(''' + convert(nvarchar(30), @crdate, 121) + ''' as datetime),
			 [lastbackup] = cast(''' + convert(nvarchar(30), @lastbackup, 121) + ''' as datetime), '
			 + '[isPrimary] = NULL '
			 + 'from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysindexes (nolock)'

		execute(@command)

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
				@crdate,
				@lastbackup,
				@roleAG
	end 
	fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @lastbackup, @cmptlevel, @roleAG
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 

select 'end of db scan'

----------------------------------------------------------------------------------
----  QUERY: Database summary information
----  Tables: master..sysdatabases, tempdb..#totaldbsizes
----  Returns: 
----	Total number of databases
----	Number of data files
----	Number of log files
----	Total data file space allocated
----	Total log file space allocated
----	Total data file space used
----------------------------------------------------------------------------------
select 
	'summary',
	(select 
		count(*)
	from
		master..sysdatabases)
	,sum(datafilecount)
	,sum(logfilecount)
	,sum(datasize)
	,sum(logsize)
	,sum(usedsize) 
from #totaldbsizes

	--START SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - Dropping the tables later since Appending DiskDriveStats and FileGroup batches
--drop table #totaldbsizes

--drop table #disk_drives
	--END SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - Dropping the tables later since Appending DiskDriveStats and FileGroup batches

DBCC SQLPERF (LOGSPACE) 