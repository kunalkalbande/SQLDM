--------------------------------------------------------------------------------
--  Batch: Database File Activity 2005
--  Variables: 
--	[0] - Autodiscover Drives (1 to autodiscover)
--  [1] - Specified Drives
--  [2] - Disable OLE	--SQLdm 10.0 (Tarun Sapra)- In case of azure hosted database, ole is disabled by default
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

	if NOT(1={0}) -- Autodiscover flag
	--begin
	-- Populate unused disk space stats

	--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Following cmds can not be executed in azure db
	--insert into #disk_drives(drive_letter, discardData)  exec xp_fixeddrives   -- Fixed
	--insert into #disk_drives(drive_letter, discardData)  exec xp_fixeddrives 1 -- Remote
	
	--insert into #disk_drives 
	--	select distinct upper(left(physical_name,1)), 
	--		null, null, null, null, null, null
	--	from master.sys.master_files
	--	where upper(left(physical_name,1)) collate database_default not in (select upper(drive_letter) collate database_default from #disk_drives)
	--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Following cmds can not be executed in azure db

	--end
	--else
	begin
		{1}
	end

	select @disableole = {2}

	--SQLdm 10.0 (Tarun Sapra)- In case of azure db, ole is disabled by default. So this block wont get executed ever
	if (@disableole = 0)
	begin

		select @curconfig = value
		from 
			syscurconfigs 
		where 
			config = 16388 
		
		if (isnull(@curconfig,1) > 0)
		begin
			select 
				@curconfig = (isnull(value,0) * -1) + 1
			from 
				syscurconfigs 
			where 
				config = 1546 

			if (@curconfig = 0)
				select 'unavailable due to lightweight pooling'
		end

		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('SQLdmDisableOLE') is not null then 0 else 1 end

		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('sp_OACreate') is not null then 1 else 0 end

	end

	--SQLdm 10.0 (Tarun Sapra)- In case of azure db, ole is disabled by default. So this block wont get executed ever
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

declare @MatchMountPoints table(dbid int, fileid bigint, drive_letter nvarchar(256), length int, name nvarchar(256), filename nvarchar(256), filetype int)

--START:SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'master.sys.master_files' is not avlbl in azure db
--insert into @MatchMountPoints
--select
--	database_id,
--	file_id,
--	drive_letter,
--	length = len(drive_letter),
--	name,
--	physical_name,
--	type
--from 
--	master.sys.master_files
--	left join
--	#disk_drives
--	on lower(substring(physical_name,1,len(drive_letter))) collate database_default = lower(drive_letter) collate database_default
--where
--	[state] not in (6,7) -- excluding offline and defunct files]
--END:SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'master.sys.master_files' is not avlbl in azure db

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

--START: SQLdm 10.0 (Tarun Sapra)- fn_virtualfilestats(,) is not supported in azure db
DECLARE @tempVar_FileActivity_1 TABLE
(
	DatabaseName nvarchar(max),
	NumberReads int,
	NumberWrites int,
	drive_letter nvarchar(max),
	[filename] nvarchar(max),
	filetype nvarchar(max),
	filepath nvarchar(max) 
)
SELECT * FROM @tempVar_FileActivity_1
--END: SQLdm 10.0 (Tarun Sapra)- fn_virtualfilestats(,) is not supported in azure db

--select
--	DatabaseName = db_name(f.dbid),
--	NumberReads = cast(vf.NumberReads as dec(38,0)),
--	NumberWrites = cast(vf.NumberWrites as dec(38,0)),
--	drive_letter,
--	f.filename,
--	filetype,
--	filepath
--from
--		#DatabaseFiles f
--	left join
--	::fn_virtualfilestats(null, null) vf
--	on vf.DbId = f.dbid
--	and vf.FileId = f.fileid	


drop table #disk_drives
drop table #DatabaseFiles
