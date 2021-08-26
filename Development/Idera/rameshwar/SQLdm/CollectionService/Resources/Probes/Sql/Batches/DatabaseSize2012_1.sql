--------------------------------------------------------------------------------
--  Batch: Database Size 2012
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
	@crdate datetime,
	@lastbackup datetime,
	@roleAG int
 
if (select isnull(object_id('tempdb..#intermediate_sysfiles'), 0)) = 0  
	create table #intermediate_sysfiles (
		driveletter nvarchar(256), 
		filetypeflag tinyint, 
		maxsize decimal(38,0), 
		size decimal(38,0), 
		growth decimal(38,0),
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
		maxsize decimal(38,0), 
		size decimal(38,0), 
		growth decimal(38,0), 
		expansion dec(38,0)) 

-- SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
IF CURSOR_STATUS('global','read_db_status')>=-1
BEGIN
 DEALLOCATE read_db_status
END

declare read_db_status insensitive cursor 
for 
	select 
		d.name,
		d.dbid, 
		d.mode, 
		case when d.name in ('master','model','msdb','tempdb') then cast(1 as bit) when d.category & 16 = 16 then cast(1 as bit) else cast(0 as bit) end,
		crdate,
		( 
		select 
		 max(backup_finish_date) 
		from 
		 msdb..backupset 
		where 
		 type <> 'F' 
		 and database_name = db.name
		 collate database_default
		) as lastbackup, 
		cmptlevel,
		(
			select case when role is null then NULL when cast(role as int)=1 then 1 else 0 end from sys.dm_hadr_availability_replica_states where replica_id = db.replica_id
		) as roleAG
	from 
		master..sysdatabases d (nolock) 
		left join master.sys.databases db (nolock) on d.dbid = db.database_id
	where lower(d.name) <> 'mssqlsystemresource'
	order by user_access, status
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @lastbackup, @cmptlevel, @roleAG
while @@fetch_status = 0 
begin 
	    -- Latter version than 2012.
		select @databasestatus  = 0, @collectsize = 0
		declare @databaseName varchar(255) = @dbname

		-- Valid states for the database
		declare @readIntentOnlyAvailabilityReplica int = 65536 -- Read Intent Only availability replica
		declare @unreadableAvailabilityReplica int = 131072 -- No Readable availability replica

		-- Add status bit for SQL 2012 or latter
		select @databasestatus =
		  case when isnull(mirr.mirroring_role,0) = 2 and isnull(db.state,0) = 1
		    then 8
			else 0 end  --Restoring Mirror
	      + case when isnull(databasepropertyex(@databaseName,'IsInStandby'), 0) = 1 or db.is_in_standby = 1 then 16 else 0 end 	--Standby
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
			else 0 end-- No Readable availability replica
		  + case when isnull(db.state, 0) = 0 and ars.role = 2 and isnull(r.secondary_role_allow_connections, 2) =1
		    then @readIntentOnlyAvailabilityReplica
			else 0 end-- Read Intent Only availability replica
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

		-- SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
if OBJECT_ID('tempdb..#AvailabilityReplica') IS NULL 
begin 
			create table #AvailabilityReplica
			(
				replica_id uniqueidentifier null,
				group_id uniqueidentifier null,
				replica_server_name nvarchar(max) null
			)
end
else
begin
	truncate table  #AvailabilityReplica
end

-- SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
if (select isnull(object_id('tempdb..#DatabaseReplicaStates'), 0)) = 0 
begin 
			create table #DatabaseReplicaStates
			(
				replica_id uniqueidentifier not null,
				group_id uniqueidentifier null,
				DatabaseName sysname not null,
				database_id int not null,
				group_database_id uniqueidentifier not null
			)
end
else
begin
	truncate table  #DatabaseReplicaStates
end
-- SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
if (select isnull(object_id('tempdb..#DatabaseJoinReplicaStates'), 0)) = 0 
begin 
			create table #DatabaseJoinReplicaStates
			(
				replica_id uniqueidentifier not null,
				group_id uniqueidentifier null,
				DatabaseName sysname not null,
				database_id int not null,
				group_database_id uniqueidentifier not null
	
			)
end
else
begin
	truncate table  #DatabaseJoinReplicaStates
end
-- SQLdm 10.3 (Varun Chopra) SQLDM-28788: SQLdm Collection service is down with 250 servers monitoring
if (select isnull(object_id('tempdb..#DatabaseLastBackupTime'), 0)) = 0 
begin 
			Create table #DatabaseLastBackupTime (DatabaseName sysname not null,LastBackupTime datetime)
end
else
begin
	truncate table  #DatabaseLastBackupTime
end
	
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
			declare @MatchMountPoints table(fileid bigint, drive_letter nvarchar(256), length int, status int,  maxsize dec(38,0),size dec(38,0), growth decimal(38,0), name nvarchar(256), filename nvarchar(256))
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
			[Allocated DBSize] = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from sysfiles (nolock) where status&1000000 = 0), 
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
			+ ' [crdate] = cast( @crdateIN as datetime), 
			  [lastbackup] = case when @lastbackupIN is null then NULL else cast(@lastbackupIN as datetime)   end, '
			+ '[isPrimary] = @roleIN'
			+ ' from SizeStats'

			declare @parmdef nvarchar(1000)

			set @parmdef = '@dbnameIN sysname, @databasestatusIN int, @systemdatabaseIN bit, @logexpansionIN dec(38,0), @dbexpansionIN dec(38,0), @crdateIN datetime, @lastbackupIN datetime, @roleIN int'

			exec sp_executesql @command, @parmdef, @dbnameIN = @dbname, @databasestatusIN= @databasestatus, @systemdatabaseIN = @systemdatabase, @logexpansionIN = @logexpansion, @dbexpansionIN = @dbexpansion, @crdateIN = @crdate, @lastbackupIN = @lastbackup, @roleIN= @roleAG

			--SQLDm10.3 - SQLDM-28411 --merged the batches to populate availability groups 			
			select	ag.group_id
				,name as 'Groupname'
				from master.sys.availability_groups ag

			select  Replicas.replica_id
				,Replicas.group_id
				,replica_server_name as 'Replica_Name'
				from master.sys.availability_replicas as Replicas

			insert into #AvailabilityReplica (replica_id,group_id,replica_server_name)
				select  Replicas.replica_id
				,Replicas.group_id
				,replica_server_name as 'Replica_Name' 
				from master.sys.availability_replicas as Replicas

			insert into #DatabaseReplicaStates(replica_id,group_id,DatabaseName ,database_id,group_database_id )
				select	dbrs.replica_id
				,group_id
				,db.name as 'DatabaseName'
				,db.database_id,
				dbrs.group_database_id
				from master.sys.dm_hadr_database_replica_states dbrs
					join master.sys.databases db on db.database_id = dbrs.database_id

			insert into #DatabaseJoinReplicaStates 
				select t2.replica_id, t2.group_id,DatabaseName,database_id ,group_database_id
					from #DatabaseReplicaStates t1 inner join  #AvailabilityReplica t2 
						 on t1.group_id = t2.group_id and t1.replica_id = t2.replica_id

			insert into #DatabaseLastBackupTime
				select 
					database_name,
					max(backup_finish_date) as lastbackup
					from 
						msdb..backupset 
						where 
							type <> 'F' 
							and database_name in (select distinct DatabaseName from #DatabaseJoinReplicaStates)--= 'TEST_DB1'
							--collate database_default
							group by database_name



			select t1.replica_id, t1.group_id,t1.DatabaseName,t1.database_id ,t1.group_database_id,t2.LastBackupTime
				from #DatabaseJoinReplicaStates t1 inner join  #DatabaseLastBackupTime t2 
					on t1.DatabaseName = t2.DatabaseName
			
			--End
			
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
				left join ::fn_virtualfilestats(NULL,NULL) vf
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

            --SQLDm10.3 - SQLDM-28411 --merged the batches to populate availability groups 			
			select	ag.group_id
				,name as 'Groupname'
				from master.sys.availability_groups ag

			select  Replicas.replica_id
				,Replicas.group_id
				,replica_server_name as 'Replica_Name'
				from master.sys.availability_replicas as Replicas

			insert into #AvailabilityReplica (replica_id,group_id,replica_server_name)
				select  Replicas.replica_id
				,Replicas.group_id
				,replica_server_name as 'Replica_Name' 
				from master.sys.availability_replicas as Replicas

			insert into #DatabaseReplicaStates(replica_id,group_id,DatabaseName ,database_id,group_database_id )
				select	dbrs.replica_id
				,group_id
				,db.name as 'DatabaseName'
				,db.database_id,
				dbrs.group_database_id
				from master.sys.dm_hadr_database_replica_states dbrs
					join master.sys.databases db on db.database_id = dbrs.database_id


			insert into #DatabaseJoinReplicaStates 
				select t2.replica_id, t2.group_id,DatabaseName,database_id ,group_database_id
					from #DatabaseReplicaStates t1 inner join  #AvailabilityReplica t2 
						 on t1.group_id = t2.group_id and t1.replica_id = t2.replica_id


			insert into #DatabaseLastBackupTime
				select 
					database_name,
					max(backup_finish_date) as lastbackup
					from 
						msdb..backupset 
						where 
							type <> 'F' 
							and database_name in (select distinct DatabaseName from #DatabaseJoinReplicaStates)--= 'TEST_DB1'
							--collate database_default
							group by database_name



			select t1.replica_id, t1.group_id,t1.DatabaseName,t1.database_id ,t1.group_database_id,t2.LastBackupTime
				from #DatabaseJoinReplicaStates t1 inner join  #DatabaseLastBackupTime t2 
					on t1.DatabaseName = t2.DatabaseName
			
			--End
			
			--select 
			--	DatabaseName = @dbname,
			--	NumberReads = cast(vf.NumberReads as dec(38,0)),
			--	NumberWrites = cast(vf.NumberWrites as dec(38,0)),
			--	driveletter,
			--	f.name,
			--	cast(filetypeflag as int),
			--	filename,
			--	driveletter
			--from 
			--	#intermediate_sysfiles f
			--	left join ::fn_virtualfilestats(NULL,NULL) vf
			--	on vf.FileId = f.fileid	
			--where 
			--	vf.DbId = @dbid

	end
	Drop table #AvailabilityReplica , #DatabaseReplicaStates , #DatabaseJoinReplicaStates , #DatabaseLastBackupTime
	fetch read_db_status into @dbname, @dbid, @mode, @systemdatabase, @crdate, @lastbackup, @cmptlevel,@roleAG
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

-- commenting below script same as done for 2000 to fix bug DE45770

--START SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - Dropping the tables later since Appending DiskDriveStats and FileGroup batches
--drop table #totaldbsizes

--drop table #disk_drives
--END SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - Dropping the tables later since Appending DiskDriveStats and FileGroup batches

DBCC SQLPERF (LOGSPACE)
