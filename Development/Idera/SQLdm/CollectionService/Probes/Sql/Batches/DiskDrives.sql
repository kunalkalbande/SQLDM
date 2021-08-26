--------------------------------------------------------------------------------
--  Batch: Disk Drives
--  Tables: #disk_drives
--  Variables:
-- [0] - sp_OACreate context
--------------------------------------------------------------------------------
declare 
	@command nvarchar(4000), 
	@dbname nvarchar(255), 
	@cmptlevel smallint,
	@curconfig int 
	
if (select isnull(object_id('tempdb..#disk_drives'), 0)) = 0 
begin 
	create table #disk_drives (
		drive_letter char(1), 
		unused_size dec(18,0),
		total_size dec(18,0),
		disk_idle bigint,
		disk_queue bigint) 
end
else
	truncate table #disk_drives

-- Populate unused disk space stats
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives   -- Fixed
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 1 -- Remote
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 2 -- Removable

update #disk_drives 
	set unused_size = unused_size * 1024.0 * 1024.0,
		drive_letter = upper(drive_letter)

declare read_db_drives insensitive cursor 
for 
	select 
		name
	from 
		master..sysdatabases d(nolock) 
	where 
		lower(name) <> 'mssqlsystemresource'
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
				)
			)
		and status & 32 <> 32 
		and status & 64 <> 64 
		and status & 128 <> 128 
		and status & 256 <> 256 
		and status & 512 <> 512 
for read only
set nocount on 
open read_db_drives 
fetch read_db_drives into @dbname
while @@fetch_status = 0 
begin 
	set @command = 'insert into #disk_drives 
						select distinct upper(left(filename,1)), 
							null, null, null, null
						from ' + quotename(@dbname) + '..sysfiles
						where upper(left(filename,1)) collate database_default not in (select upper(drive_letter) collate database_default from #disk_drives)'
	exec (@command)
	fetch read_db_drives into @dbname
End 
Close read_db_drives 
deallocate read_db_drives 

-- Check for ole automation
select @curconfig = value from master..syscurconfigs where config = 16388 

if (@curconfig is null) 
	select @curconfig = case when object_id('master..sp_OACreate') is not null then 1 else 0 end

if (isnull(@curconfig,1) > 0)
	select 
		@curconfig = (isnull(value,0) * -1) + 1
	from 
		master..syscurconfigs 
	where 
		config = 1546 

if (isnull(@curconfig,-1) > 0) 
begin 
	declare 
		@hr int,
		@FileSystemObject int,
		@FsoDrive int,
		@driveletter char(1),
		@totalSize nvarchar(50),
		@WmiServiceLocator int, 
		@WmiService int,
		@CounterCollection int, 
		@CounterObject int, 
		@CounterName varchar(255),
		@PercentIdleTime varchar(21), 
		@FreeSpace varchar(21),
		@AvgDiskQueueLength varchar(21), 
		@PercentIdleTime_Base varchar(21)

		-- Create FSO object
		exec @hr=sp_OACreate 'Scripting.FileSystemObject',@FileSystemObject output, {0}

		-- Create WMI object
		exec sp_OACreate 'WbemScripting.SWbemLocator', @WmiServiceLocator output, {0}
		
		exec sp_OAMethod @WmiServiceLocator, 
			'ConnectServer', 
			@WmiService output, 
			'.', 
			'root\cimv2' 

		if (@hr = 0 )
		begin
			declare read_disk_size insensitive cursor 
			for 
				select drive_letter from #disk_drives
			for read only
			open read_disk_size
			fetch next from read_disk_size into @driveletter
			while @@fetch_status = 0
			begin
				select @FsoDrive = null, @totalSize = null, @CounterObject = null, @PercentIdleTime = null, @AvgDiskQueueLength=null

				-- Get size information from FSO

				exec sp_OAMethod @FileSystemObject,'GetDrive', @FsoDrive out, @driveletter

				exec sp_OAGetProperty @FsoDrive,'TotalSize', @totalSize out

				exec sp_OAGetProperty @FsoDrive,'FreeSpace', @FreeSpace out
		
				update #disk_drives 
					set total_size = convert(dec(18,0),@totalSize),
					unused_size = convert(dec(18,0),@FreeSpace) 
				where drive_letter = @driveletter

				-- Get throughput information from WMI
				if isnull(@WmiService,-1) > 0 
				begin 
					select @CounterName = 'Win32_PerfFormattedData_PerfDisk_LogicalDisk.Name="' + @driveletter + ':"' 

					exec sp_OAMethod @WmiService, 
		 				'Get', 
		 				@CounterObject output, 
		 				@CounterName

					exec sp_OAMethod @CounterObject, 'Refresh_'

					waitfor delay '00:00:01'

					exec sp_OAMethod @CounterObject, 'Refresh_'
		
					exec sp_OAGetProperty @CounterObject, 
						'PercentIdleTime', 
						@PercentIdleTime output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDiskQueueLength', 
						@AvgDiskQueueLength output 

					update #disk_drives 
						set 
						disk_idle = convert(bigint,@PercentIdleTime), 
						disk_queue = convert(bigint,@AvgDiskQueueLength)
					where 
						drive_letter = @driveletter
				end

				exec sp_OADestroy @CounterObject 
				exec sp_OADestroy @FsoDrive

				fetch next from read_disk_size into @driveletter
			end
			close read_disk_size
			deallocate read_disk_size
	end
end

if (@curconfig > 0) 
begin 
	exec sp_OADestroy @FileSystemObject
	exec sp_OADestroy @WmiService 
	exec sp_OADestroy @WmiServiceLocator  
end

select * from #disk_drives

