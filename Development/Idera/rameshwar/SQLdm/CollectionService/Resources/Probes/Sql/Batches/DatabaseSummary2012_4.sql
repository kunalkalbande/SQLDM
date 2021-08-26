
--------------------------------------------------------------------------------
--  Batch: Database Summary 2005
--  Tables: #Open_Tran,#disk_drives,#totaldbsizes,msdb..backupset,
--		master..sysdatabases,sys.partitions,sys.allocation_units,sys.internal_tables,
--		sysobjects,sysfiles,sysfilegroups,sysprocesses,sys.master_files
--  Variables: [0] - Exclude System Databases
--		[1] - Session count
--		[2] - Oldest open transactions
--------------------------------------------------------------------------------
set concat_null_yields_null off
use master

declare 
	--@command nvarchar(4000), 
	--@dbname nvarchar(255), 
	@dbid smallint, 
	@mode smallint,
	@status int,
	@status2 int,
	@category int,
	@crdate datetime,
	@lastbackup datetime,
	@compatibility tinyint,
	@databasestatus int,
	@collectsize bit,
	@recovery nvarchar(120)


if (select isnull(object_id('tempdb..#Open_Tran'),0)) = 0 
Create table #Open_Tran (RecID varchar(35), RecValue varchar(255)) 
else truncate table #Open_Tran 
 
declare @intermediate_sysfiles table(
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize dec(38,0), 
		size dec(38,0), 
		growth dec(38,0))

declare @MatchMountPoints table(fileid bigint, drive_letter nvarchar(256), length int, type int,  maxsize dec(38,0),size dec(38,0), growth dec(38,0))

if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
	create table #totaldbsizes (usedsize dec(38,0)) 
else 
	truncate table #totaldbsizes 

declare @grouped_devices table(
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize dec(38,0), 
		size dec(38,0), 
		growth dec(38,0), 
		expansion dec(38,0)) 

declare read_db_status insensitive cursor 
for 
	select 
		d.name,
		d.dbid,
		d.mode, 
		isnull(convert(integer,d.status),-999),
		isnull(convert(integer,d.status2),-999), 
		d.category, 
		d.crdate, 
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
		d.cmptlevel,
		d2.recovery_model_desc
	from 
		master..sysdatabases d (nolock) 
		left join
		master.sys.databases d2(nolock) 
		on d.dbid = d2.database_id
	where 
		lower(d.name) <> 'mssqlsystemresource'
		{0}
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid, @mode, @status, @status2, @category, @crdate,	@lastbackup, @compatibility, @recovery  
while @@fetch_status = 0 
begin 
		-- Valid states for the database
		declare @readIntentOnlyAvailabilityReplica int = 65536 -- Read Intent Availbility Replica
		declare @unreadableAvailabilityReplica int = 131072 -- No Readable Availbility Replica

		-- Add status bits
		declare @databaseName nvarchar(255) = @dbname
		select @databasestatus  = 0, @collectsize = 0

		select @databasestatus = case when isnull(mirr.mirroring_role,0) = 2 and isnull(db.state,0) = 1
			then 8 
			else 0 end  --Restoring Mirror
		+ case when isnull(databasepropertyex(@databaseName,'IsInStandby'),0) = 1 or db.is_in_standby = 1 then 16 else 0 end 	--Standby
		+ case when (isnull(db.state,0) = 1 and isnull(mirr.mirroring_role,0) <> 2) then 32 else 0 end  --Restoring (non-mirror)
		+ case when isnull(db.state,0) = 3 then 64 else 0 end  --Pre-Recovery
		+ case when isnull(db.state,0) = 2 then 128 else 0 end	--Recovering
		+ case when isnull(db.state,0) = 4 then 256 else 0 end	--Suspect
		+ case when isnull(db.state,0) = 6 then 512 else 0 end	--Offline
		+ case when isnull(db.is_read_only,0) = 1 then 1024 else 0 end --Read Only
		+ case when db.user_access = 1 then 4096 else 0 end	--Single User
		+ case when isnull(db.state,0) = 5 then 32768 else 0 end  --Emergency Mode	

		+ case when isnull(db.state, 0) = 0 and ars.role = 2 and isnull(r.secondary_role_allow_connections, 2) = 0
			then @unreadableAvailabilityReplica
			else 0 end -- No Readable Availbility Replica
		+ case when isnull(db.state, 0) = 0 and ars.role = 2 and isnull(r.secondary_role_allow_connections, 2) = 1
			then @readIntentOnlyAvailabilityReplica
			else 0 end -- Read Intent Availbility Replica

		+ case when isnull(db.is_cleanly_shutdown,0) = 1 then 1073741824 else 0 end  --Cleanly Shutdown
	from 
		master.sys.databases db
		left outer join sys.database_mirroring mirr on mirr.database_id = db.database_id
		left outer join sys.dm_hadr_database_replica_states s on s.database_id = db.database_id
			left outer join sys.availability_replicas r on r.replica_id = s.replica_id
			left outer join sys.dm_hadr_availability_replica_states ars on r.replica_id = ars.replica_id
			and r.group_id = s.group_id
	where db.database_id = db_id(@databaseName) 
	and isnull(ars.is_local, 1)  = 1

	-- Decide whether to collect size information
	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
		and (@databasestatus & @unreadableAvailabilityReplica) <> @unreadableAvailabilityReplica -- Un-readable availability replica
	
			or (@databasestatus & 4096 = 4096 
				and not exists 
				(select * from master..sysprocesses p where dbid = @dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = @dbid and l.request_session_id <> @@spid)
			))  --Single User
		)
			select @collectsize = 1

	-- If we cannot access database, but there is no known reason, set database to Inacessible
	if	(@collectsize = 1 and has_dbaccess (@dbname) <> 1 or @mode <> 0)
		select @databasestatus = @databasestatus + 8192, @collectsize = 0
	
	if @collectsize = 1
	begin 
			select @command = 'insert into #totaldbsizes select sum(convert(dec(38,0),used_pages))*8 from [' 
			+ replace(@dbname,char(93),char(93)+char(93)) 
			+ '].sys.allocation_units'
			
			execute(@command)

			-- Autogrow Flags
			-- 1 = data autogrow on
			-- 4 = data autogrow off
			-- 8 = log autogrow on
			-- 16 = log autogrow off

			select @command =  case when @compatibility >= 80 then 'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] ' else ' ' end 
			+' ;with SizeStats(Data_Size,TextImage_Size,Used_Pages) as (select 
			Data_Size = sum(case when it.internal_type in (202,204) then 0 when p.index_id < 2 then convert(dec(38,0),au.data_pages) else 0 end) * 8,   
			TextImage_Size = sum(case when it.internal_type in (202,204) then 0 when au.type != 1 then convert(dec(38,0),au.used_pages) else 0 end) * 8, 
			Used_Pages = sum(convert(dec(38,0),au.used_pages)) * 8 
			from ' + quotename(@dbname) + '.sys.partitions p 
			left join ' + quotename(@dbname) + '.sys.allocation_units au on p.partition_id = au.container_id  ' 
			+ ' left join ' + quotename(@dbname) + '.sys.internal_tables it on p.object_id = it.object_id )'
			+'select ''Database'', ''' + replace(@dbname,char(39),char(39)+char(39)) + ''', ' 
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
			+ 'Allocated_DBSize = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from ' + quotename(@dbname) 
			+ '..sysfiles (nolock) where status&1000000 = 0), 
			Data_Size,TextImage_Size,(Used_Pages - Data_Size - TextImage_Size) as Index_Size,
			{1}
			+ 'autogrow = (select sum(distinct(case 
				when growth <> 0 and status&1000000 = 0 then 1 
				when growth = 0 and status&1000000 = 0 then 4
				when growth <> 0 and status&1000000 <> 0 then 8 
				when growth = 0 and status&1000000 <> 0 then 16
				else 0 end)) from ' + quotename(@dbname) + '..sysfiles), '
			+ 'recovery = ' + (select case @recovery when 'SIMPLE' then cast(3 as char) when 'BULK_LOGGED' then cast(2 as char) else cast(1 as char) end)
			+ ',' 
			+ case when @compatibility >= 80 then '[Total Used]= (select sum(convert(dec(38,0), fileproperty(name,''SpaceUsed'') ) * 8) from [' 
				+ replace(@dbname,char(93),char(93)+char(93)) 
				+ ']..sysfiles where status&1000000 = 0) '
				else ' null ' end
			+ ' from SizeStats'

		execute(@command)

		{2}

		delete from @intermediate_sysfiles 

	if (exists(select * from #disk_drives where len(drive_letter) > 1))
		begin
			delete from @MatchMountPoints
			
			insert into @MatchMountPoints
			select
				file_id,
				drive_letter,
				length = len(drive_letter),
				type,
				max_size,
				size, 
				growth
			from 
				master.sys.master_files
				left join
				#disk_drives
				on lower(substring(physical_name,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default
			where
				type < 3
				and database_id = @dbid

			insert into @intermediate_sysfiles 
			select 
					upper(drive_letter) as driveletter, 
					case when type = 1 then 0 else 1 end, 
					maxsize, 
					size, 
					growth 
			from @MatchMountPoints m1
			where 
				length  = (select max(length) from @MatchMountPoints m2 where m2.fileid = m1.fileid)
		end
		else
		begin

			insert into @intermediate_sysfiles 
				select upper(substring(physical_name, 1, 1)) as driveletter, 
				case when type = 1 then 0 else 1 end,  
				max_size, 
				size, 
				growth 
			from 
				master.sys.master_files
			where
				type < 3
				and database_id = @dbid
		end

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
				@intermediate_sysfiles
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
				@intermediate_sysfiles
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
				@intermediate_sysfiles
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
				case @recovery when 'SIMPLE' then 3 when 'BULK_LOGGED' then 2 else 1 end

	end 
	fetch read_db_status into @dbname, @dbid, @mode, @status, @status2, @category, @crdate,	@lastbackup, @compatibility, @recovery 
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 


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


DBCC SQLPERF (LOGSPACE) 

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
