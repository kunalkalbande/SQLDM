--------------------------------------------------------------------------------
--  Batch: Database Summary 2000
--  Tables: #Open_Tran,#disk_drives,#totaldbsizes,#intermediate_sysfiles,
--		#totaldbsizes,msdb..backupset,master..sysdatabases,sysindexes,
--		sysobjects,sysfiles,sysfilegroups
--  Variables: [0] - Exclude System Databases
--		[1] - Session count
--------------------------------------------------------------------------------
set concat_null_yields_null off
use master

declare 
--	@command nvarchar(4000), 
--	@dbname nvarchar(255), 
	@dbid smallint, 
	@mode smallint,
	@status int,
	@status2 int,
	@category int,
	@crdate datetime,
	@lastbackup datetime,
	@compatibility tinyint,
	@databasestatus int,
	@collectsize bit


if (select isnull(object_id('tempdb..#Open_Tran'),0)) = 0 
Create table #Open_Tran (RecID varchar(35), RecValue varchar(255)) 
else truncate table #Open_Tran 

if (select isnull(object_id('tempdb..#intermediate_sysfiles'), 0)) = 0  
	create table #intermediate_sysfiles (
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize integer, 
		size integer, 
		growth integer) 
else 
	truncate table #intermediate_sysfiles 

declare @MatchMountPoints table(fileid bigint, drive_letter nvarchar(256), length int, type int,  maxsize dec(38,0),size dec(38,0), growth int)

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
		isnull(convert(integer,status),-999),
		isnull(convert(integer,status2),-999), 
		category, 
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
		cmptlevel
	from 
		master..sysdatabases d(nolock) 
	where
		1=1 
		{0}
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid, @mode, @status, @status2, @category, @crdate,	@lastbackup, @compatibility  
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

		select @command =  case when @compatibility >= 80 then 'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] ' else ' ' end 
			+ 'select  ''Database'', ''' + replace(@dbname,char(39),char(39)+char(39)) + ''', ' 
			+ 'dbstatus = ' + convert(varchar(30),@databasestatus,0) + ', '  			
			+ 'status = ' + convert(varchar(30),@status) + ', ' 
			+ 'status2 = ' + convert(varchar(30),@status2) + ', ' 
			+ 'category = ' + convert(varchar(30),@category) + ', ' 
			+ 'crdate = cast(''' + convert(varchar(30),@crdate,121) + ''' as datetime), ' 
			+ 'lastbackup = ' + case when @lastbackup is null then ' NULL' else 'cast(''' + convert(varchar(30),dateadd(mi,datediff(mi,getdate(),getutcdate()),@lastbackup),121) + ''' as datetime) '  end + ', '
			+ 'compatibility = ' + convert(varchar(30),@compatibility) + ', ' 
			+ 'systemTables = (select count(*) from ' + quotename(@dbname) + '..sysobjects where xtype = ''S''), '
			+ 'userTables = (select count(*) from ' + quotename(@dbname) + '..sysobjects where xtype = ''U''), '
			+ 'filecount = (select count(*) from ' + quotename(@dbname) + '..sysfiles), '
			+ 'filegroupcount = (select count(*) from ' + quotename(@dbname) + '..sysfilegroups), '
			+ 'Allocated_DBSize = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ ']..sysfiles (nolock) where status&1000000 = 0), Data_Size = (sum(case when indid in (0, 1) then convert(dec(38,0),dpages) else 0 end) * 8), '
				+ 'TextImage_Size = (sum(case when indid = 255 then convert(dec(38,0),used) else 0 end) * 8), ' 
				+ 'Index_Size = (sum(case when indid in (0, 1) then convert(dec(38,0),used) - convert(dec(38,0),dpages) else 0 end) * 8), '
				+ '{1}
				+ 'autogrow = (select sum(distinct(case 
				when growth <> 0 and status&1000000 = 0 then 1 
				when growth = 0 and status&1000000 = 0 then 4
				when growth <> 0 and status&1000000 <> 0 then 8 
				when growth = 0 and status&1000000 <> 0 then 16
				else 0 end)) from ' + quotename(@dbname) + '..sysfiles), '
			+ 'recovery = (select case databasepropertyex(''' + replace(@dbname,char(39),char(39)+char(39)) + ''',''recovery'') when ''SIMPLE'' then 3 when ''BULK_LOGGED'' then 2 else 1 end) '
			+ ',' 
			+ case when @compatibility >= 80 then '[Total Used]= (select sum(convert(dec(38,0), fileproperty(name,''SpaceUsed'') ) * 8) from [' 
				+ replace(@dbname,char(93),char(93)+char(93)) 
				+ ']..sysfiles where status&1000000 = 0) '
				else ' null ' end
				+ ' from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysindexes (nolock)'

		execute(@command)

		{2}

		if (exists(select * from #disk_drives where len(drive_letter) > 1))
			begin
				select @command = 'use [' + replace(@dbname,char(93),char(93)+char(93)) + ']
				truncate table #intermediate_sysfiles
				declare @MatchMountPoints table(fileid bigint, drive_letter nvarchar(256), length int, status int,  maxsize dec(38,0),size dec(38,0), growth int)

				insert into @MatchMountPoints
				select
					fileid,
					drive_letter,
					length = len(drive_letter),
					status,
					maxsize,
					size, 
					growth
				from 
					sysfiles
					left join
					#disk_drives
					on lower(substring(filename,1,len(drive_letter))) ' + case when @compatibility >= 80  then ' collate database_default ' else ' ' end 
					+ '= lower(drive_letter) ' + case when @compatibility >= 80  then  ' collate database_default ' else ' ' end + '

				insert into #intermediate_sysfiles 
				select 
						upper(drive_letter) as driveletter, 
						case when status&1000000 = 0 then 1 else 0 end as datafile, 
						maxsize, 
						size, 
						growth 
				from @MatchMountPoints m1
				where 
					length  = (select max(length) from @MatchMountPoints m2 where m2.fileid = m1.fileid)'
			end
			else
			begin

				select @command = 'truncate table #intermediate_sysfiles insert into #intermediate_sysfiles 
				select upper(substring(filename, 1, 1)) as driveletter, case when status&1000000 = 0 then 1 else 0 end as datafile, 
				maxsize, size, growth from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysfiles'
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

			
				select 
					'Log Expansion', case when min(convert(dec(38,0),minexpansion)) > -1 then sum(convert(dec(38,0),maxexpansion)) else null end 
				from 
					(
					select maxexpansion = 
						case when (sum(convert(dec(38,0),expansion)) > sum(convert(dec(38,0),d.unused_size)/1024))
							then min(convert(dec(38,0),d.unused_size)/1024)
						else
							sum(convert(dec(38,0),expansion)) 
						end,
						minexpansion = min(isnull(expansion,-1)) 
					from 
						@grouped_devices 
						left join #disk_drives d
						on driveletter collate database_default = upper(d.drive_letter collate database_default)
					where filetypeflag = 0 
					group by driveletter, d.drive_letter) g
				union
				select 
					'DB Expansion', case when min(convert(dec(38,0),minexpansion)) > -1 then sum(convert(dec(38,0),maxexpansion)) else null end 
				from 
					(
					select maxexpansion = 
						case when (sum(convert(dec(38,0),expansion)) > sum(convert(dec(38,0),d.unused_size)/1024))
							then min(convert(dec(38,0),d.unused_size)/1024)
						else
							sum(convert(dec(38,0),expansion)) 
						end,
						minexpansion = min(isnull(expansion,-1)) 
					from 
						@grouped_devices 
						left join #disk_drives d
						on driveletter collate database_default = upper(d.drive_letter collate database_default)
					where filetypeflag <> 0 
					group by driveletter, d.drive_letter) g
			
	end 
	else 
	begin  
			select 
				'Database', 
				@dbname, 
				@databasestatus,
				case databasepropertyex(@dbname,'recovery') when 'SIMPLE' then 3 when 'BULK_LOGGED' then 2 else 1 end
	end 
	fetch read_db_status into @dbname, @dbid, @mode, @status, @status2, @category, @crdate,	@lastbackup, @compatibility  
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 


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

drop table #totaldbsizes

dbcc sqlperf (logspace) 


select 
	counter_name, 
	convert(bigint,isnull(cntr_value,0)), 
	instance_name 
from 
	master..sysperfinfo (nolock)
where 
	rtrim(Lower(object_name)) like Lower('%:databases') 
	and Lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
	and Lower(instance_name) <> '_total' 
	and lower(instance_name) <> 'mssqlsystemresource'
order by 
	instance_name 
