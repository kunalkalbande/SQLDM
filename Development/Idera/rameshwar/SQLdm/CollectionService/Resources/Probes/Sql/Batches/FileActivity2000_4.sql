--------------------------------------------------------------------------------
--  Batch: Database File Activity 2000
--  Variables: 
--	[0] - Autodiscover Drives (1 to autodiscover)
--  [1] - Specified Drives
--  [2] - Disable OLE
--  [3] - sp_OA context
--  [4] - Direct WMI
--------------------------------------------------------------------------------
declare 
	@fa_dbname nvarchar(255), 
	@cmptlevel smallint,
	@curconfig int,
	@disableole int,
	@dbid smallint, 
	@fa_mode smallint,
	@databasestatus int,
	@proceed bit,
	@fa_command nvarchar(1000),
	@directwmi int


if (select isnull(object_id('tempdb..#disk_drives'), 0)) = 0 
begin 
	create table #disk_drives (
			drive_letter nvarchar(256), 
			discardData dec(38,0),
			DiskReadsPerSec nvarchar(30),
			DiskWritesPerSec nvarchar(30),
			DiskTransfersPerSec nvarchar(30),
			Frequency_Perftime nvarchar(30),
			TimeStamp_PerfTime nvarchar(30)
	)
	end	
else
	truncate table #disk_drives

if (select isnull(object_id('tempdb..#DatabaseFiles'), 0)) = 0 
begin 
	create table #DatabaseFiles (
			dbid int, 
			fileid bigint, 
			drive_letter nvarchar(256), 
			filename nvarchar(256),
			filepath nvarchar(256),
			filetype int
	)
end	
else
	truncate table #DatabaseFiles

select @directwmi={4}
if (@directwmi = 1)
begin
    -- select machine name for wmi connection
	select cast(serverproperty('MachineName') as varchar(255)) as [MachineName]
end
else
begin
	if (1={0}) -- Autodiscover flag
	begin
	-- Populate unused disk space stats
	insert into #disk_drives(drive_letter, discardData)  exec master..xp_fixeddrives   -- Fixed
	insert into #disk_drives(drive_letter, discardData)  exec master..xp_fixeddrives 1 -- Remote

	insert into #disk_drives 
		select distinct upper(left(filename,1)), 
			null, null, null, null, null, null
		from master..sysaltfiles
		where upper(left(filename,1)) collate database_default not in (select upper(drive_letter) collate database_default from #disk_drives)
	end
	else
	begin
		{1}
	end

	select @disableole = {2}

	if (@disableole = 0)
	begin

		select @curconfig = value
		from 
			master..syscurconfigs 
		where 
			config = 16388 
		
		if (isnull(@curconfig,1) > 0)
		begin
			select 
				@curconfig = (isnull(value,0) * -1) + 1
			from 
				master..syscurconfigs 
			where 
				config = 1546 

			if (@curconfig = 0)
				select 'unavailable due to lightweight pooling'
		end

		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('master..SQLdmDisableOLE') is not null then 0 else 1 end

		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('master..sp_OACreate') is not null then 1 else 0 end

	end

	if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1) 
	begin 
		declare 
			@hr int,
			@driveletter nvarchar(256),
			@WmiServiceLocator int, 
			@WmiService int,
			@CounterCollection int, 
			@CounterObject int, 
			@CounterName varchar(255),
			@DiskReadsPerSec varchar(50),
			@DiskWritesPerSec varchar(50),
			@DiskTransfersPerSec varchar(50),
			@Frequency_Perftime varchar(50),
			@TimeStamp_PerfTime varchar(50)

			-- Create WMI object
			exec @hr=sp_OACreate 'WbemScripting.SWbemLocator', @WmiServiceLocator output, {3}
		
			exec sp_OAMethod @WmiServiceLocator, 
				'ConnectServer', 
				@WmiService output, 
				'.', 
				'root\cimv2' 

			if (@hr = 0 and isnull(@WmiService,-1) > 0)
			begin

				select 'available'	

				declare read_disk_size insensitive cursor 
				for 
					select drive_letter from #disk_drives order by drive_letter 
				for read only
				open read_disk_size
				fetch next from read_disk_size into @driveletter
				while @@fetch_status = 0
				begin
					-- Get throughput information from WMI
					if isnull(@WmiService,-1) > 0 
					begin 
						select @DiskReadsPerSec = null, @DiskWritesPerSec = null, @DiskTransfersPerSec = null, @Frequency_Perftime = null, @TimeStamp_PerfTime = null

						select @CounterName = 'Win32_PerfRawData_PerfDisk_LogicalDisk.Name=''' +  rtrim(case when charindex(':',@driveletter) > 0 then @driveletter else @driveletter + ':' end) + '''' 

						exec sp_OAMethod @WmiService, 
							'Get', 
							@CounterObject output, 
							@CounterName

						exec sp_OAGetProperty @CounterObject, 
							'DiskReadsPerSec', 
							@DiskReadsPerSec output 

						exec sp_OAGetProperty @CounterObject, 
							'DiskWritesPerSec', 
							@DiskWritesPerSec output 

						exec sp_OAGetProperty @CounterObject, 
							'DiskTransfersPerSec', 
							@DiskTransfersPerSec output

						exec sp_OAGetProperty @CounterObject, 
							'Frequency_Perftime', 
							@Frequency_Perftime output 

						exec sp_OAGetProperty @CounterObject, 
							'TimeStamp_PerfTime', 
							@TimeStamp_PerfTime output 

						update #disk_drives 
							set 
							DiskReadsPerSec = nullif(@DiskReadsPerSec,'0'),
							DiskWritesPerSec = nullif(@DiskWritesPerSec,'0'),
							DiskTransfersPerSec = nullif(@DiskTransfersPerSec,'0'),
							Frequency_Perftime = nullif(@Frequency_Perftime,'0'),
							TimeStamp_PerfTime = nullif(@TimeStamp_PerfTime,'0')
						where 
							drive_letter = @driveletter
					end
					else

					exec sp_OADestroy @CounterObject 

					fetch next from read_disk_size into @driveletter
				end
				close read_disk_size
				deallocate read_disk_size
		end
		else
		begin
			select 'service unavailable' 
		end
	end
	else
	begin
		if @disableole = 1
			select 'ole automation disabled'
		else
			select 'procedure unavailable' 
	end
end

declare @MatchMountPoints table(dbid int, fileid bigint, drive_letter nvarchar(256), length int, name nvarchar(256), filename nvarchar(256))

insert into @MatchMountPoints
select
	dbid,
	fileid,
	drive_letter,
	length = len(drive_letter),
	name,
	filename
from 
	master..sysaltfiles
	left join
	#disk_drives
	on lower(substring(filename,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default
	where dbid <> 32767

insert into #DatabaseFiles
select 
	dbid,
	fileid,
	drive_letter,
	name,
	filename,
	null
from @MatchMountPoints m1
where 
	length  = (select max(length) from @MatchMountPoints m2 where m2.fileid = m1.fileid and m2.dbid = m1.dbid)
	or length is null



declare read_db_status insensitive cursor 
for 
	select 
		name, 
		dbid, 
		mode
	from 
		master..sysdatabases d (nolock) 
	order by
		isnull(databaseproperty(name, 'IsSingleUser'),0),
		status
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @fa_dbname, @dbid, @fa_mode
while @@fetch_status = 0 
begin 
	
	select @databasestatus  = 0, @proceed = 0

	-- Add status bits
	select @databasestatus = 
		case when isnull(databasepropertyex(@fa_dbname,'IsInStandby'),0) = 1 then 16 else 0 end 									--Standby
		+ case when isnull(databaseproperty(@fa_dbname, 'IsInLoad'),0) = 1 or db.status & 32 = 32 then 32 else 0 end				--Restoring (non-mirror)
		+ case when db.status & 64 = 64 then 64 else 0 end																			--Pre-Recovery
		+ case when isnull(databaseproperty(@fa_dbname, 'IsInRecovery'),0) = 1 or db.status & 128 = 128 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@fa_dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@fa_dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@fa_dbname, 'IsShutDown'),0) = 1 or db.status & 256 = 256 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@fa_dbname, 'IsOffline'),0) = 1 or db.status & 512 = 512 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@fa_dbname, 'IsReadOnly'),0) = 1 or db.status & 1024 = 1024 then 1024 else 0 end		--Read Only
		+ case when isnull(databaseproperty(@fa_dbname, 'IsDboOnly'),0) = 1 or db.status & 2048 = 2048 then 2048 else 0 end		--DBO Use
		+ case when isnull(databaseproperty(@fa_dbname, 'IsSingleUser'),0) = 1 or db.status & 4096 = 4096 then 4096 else 0 end		--Single User
		+ case when isnull(databaseproperty(@fa_dbname, 'IsEmergencyMode'),0) = 1 or db.status & 32768 = 32768 then 32768 else 0 end	--Emergency Mode	
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
			select @proceed = 1

	-- If we cannot access database, but there is no known reason, set database to Inacessible
	if	@proceed = 1 and (has_dbaccess (@fa_dbname) <> 1 or @fa_mode <> 0)
		select @databasestatus = @databasestatus + 8192, @proceed = 0
	
	if @proceed = 1
	begin 
		select @fa_command = 'use [' + replace(@fa_dbname,char(93),char(93)+char(93)) + ']
		update d set filetype = case status & 0x40 when 0x40 then 1 else 0 end
		from
		#DatabaseFiles d,
		sysfiles f
		where d.dbid = db_id()
		and d.fileid = f.fileid'

		exec(@fa_command)
	end
	fetch read_db_status into @fa_dbname, @dbid, @fa_mode
End -- End "while @@fetch_status = 0 " loop
Close read_db_status 
deallocate read_db_status 

if (@directwmi <> 1)
begin
	select 
		drive_letter,
		DiskReadsPerSec,
		DiskWritesPerSec,
		DiskTransfersPerSec,
		Frequency_Perftime,
		TimeStamp_PerfTime
	from 
		#disk_drives
end

select
	DatabaseName = db_name(f.dbid),
	NumberReads = cast(vf.NumberReads as dec(38,0)),
	NumberWrites = cast(vf.NumberWrites as dec(38,0)),
	drive_letter,
	f.filename,
	filetype,
	filepath
from
	 #DatabaseFiles f
	left join
	::fn_virtualfilestats(-1,-1) vf
	on vf.DbId = f.dbid
	and vf.FileId = f.fileid	


drop table #disk_drives
drop table #DatabaseFiles
