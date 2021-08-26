--------------------------------------------------------------------------------
--  Batch: Database File Activity 2008
--  Variables: 
--	[0] - Autodiscover Drives (1 to autodiscover)
--  [1] - Specified Drives
--  [2] - Disable OLE
--  [3] - sp_OA context
--  [4] - Direct WMI
--------------------------------------------------------------------------------
declare 
	@curconfig int,
	@disableole int,
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
			TimeStamp_PerfTime nvarchar(30),
			unused_size decimal(38,5)
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
	    IF ((IS_SRVROLEMEMBER('sysadmin') = 1))
	    BEGIN
	        -- Populate unused disk space stats
	        insert into #disk_drives(drive_letter, discardData)  exec master..xp_fixeddrives   -- Fixed
	        insert into #disk_drives(drive_letter, discardData)  exec master..xp_fixeddrives 1 -- Remote
            END
        INSERT INTO #disk_drives(drive_letter, unused_size)  
          SELECT DISTINCT UPPER(LEFT(volume_mount_point, 1)) AS drive, available_bytes/1024/1024 AS 'MB free'
            FROM sys.master_files AS f CROSS APPLY sys.dm_os_volume_stats(f.database_id, f.file_id)
          WHERE UPPER(LEFT(volume_mount_point,1)) collate database_default NOT IN (SELECT UPPER(drive_letter) collate database_default FROM #disk_drives)
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
			  SELECT 'available' 
			  IF ((IS_SRVROLEMEMBER('sysadmin') = 1))
			  BEGIN
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

					exec sp_OADestroy @CounterObject 

					fetch next from read_disk_size into @driveletter
				end
				close read_disk_size
				deallocate read_disk_size
		  END
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

declare @MatchMountPoints table(dbid int, fileid bigint, drive_letter nvarchar(256), length int, name nvarchar(256), filename nvarchar(256), filetype int)

insert into @MatchMountPoints
select
	database_id,
	file_id,
	drive_letter,
	length = len(drive_letter),
	name,
	physical_name,
	type
from 
	master.sys.master_files
	left join
	#disk_drives
	on lower(substring(physical_name,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default
where
	[state] not in (6,7) -- excluding offline and defunct files]

insert into #DatabaseFiles
select 
	dbid,
	fileid,
	drive_letter,
	name,
	filename,
	filetype
from @MatchMountPoints m1
where 
	length  = (select max(length) from @MatchMountPoints m2 where m2.fileid = m1.fileid and m2.dbid = m1.dbid)
	or length is null

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
	::fn_virtualfilestats(null, null) vf
	on vf.DbId = f.dbid
	and vf.FileId = f.fileid	


drop table #disk_drives
drop table #DatabaseFiles
