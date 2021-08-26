--------------------------------------------------------------------------------
--  Batch: Disk Drives 2005
--  Tables: #disk_drives
--  Variables:
--  [0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cooked disk drive segment (if applicable)
--  [3] disk drive filter 
--------------------------------------------------------------------------------

select @disableole = {1}

if (@disableole = 0)
begin

	-- Check for ole automation
	select @curconfig = cast(value_in_use as integer) from sys.configurations where name = N'Ole Automation Procedures'

	if (@curconfig is null) 
		select @curconfig = case when object_id('master..sp_OACreate') is not null then 1 else 0 end


	if (isnull(@curconfig,1) > 0)
		select @curconfig = case when object_id('master..SQLdmDisableOLE') is not null then 0 else 1 end

	if (isnull(@curconfig,1) > 0)
		select 
			@curconfig = (isnull(value,0) * -1) + 1
		from 
			master..syscurconfigs 
		where 
			config = 1546 
end

if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1) 
begin 
	declare 
		@hr int,
		@FileSystemObject int,
		@FsoDrive int,
		@driveletter nvarchar(256),
		@TotalDiskSizeBytes nvarchar(50),
		@WmiServiceLocator int, 
		@WmiService int,
		@CounterCollection int, 
		@CounterObject int, 
		@CounterName varchar(255),
		@PercentIdleTime varchar(50), 
		@PercentFreeSpace varchar(50), 
		@PercentFreeSpace_Base varchar(50),
		@FreeDiskSizeBytes varchar(50),
		@AvgDiskQueueLength varchar(50), 
		@PercentIdleTime_Base varchar(50),
		@TimeStamp_Sys100NS varchar(50),
		@AvgDisksecPerRead varchar(50),
		@AvgDisksecPerRead_Base varchar(50),
		@AvgDisksecPerTransfer varchar(50),
		@AvgDisksecPerTransfer_Base varchar(50),
		@AvgDisksecPerWrite varchar(50),
		@AvgDisksecPerWrite_Base varchar(50),
		@Frequency_Perftime varchar(50),
		@DiskReadsPerSec varchar(50),
		@DiskTransfersPerSec varchar(50),
		@DiskWritesPerSec varchar(50),
		@TimeStamp_PerfTime varchar(50)

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
				select drive_letter from #disk_drives order by drive_letter 
			for read only
			open read_disk_size
			fetch next from read_disk_size into @driveletter
			while @@fetch_status = 0
			begin
				select @FsoDrive = null, @TotalDiskSizeBytes = null, @CounterObject = null, @PercentIdleTime = null, @AvgDiskQueueLength=null, @FreeDiskSizeBytes = null, @PercentFreeSpace = null, @PercentIdleTime_Base = null, @TotalDiskSizeBytes = null,
				@DiskReadsPerSec = null, @DiskTransfersPerSec = null, @DiskWritesPerSec = null

				-- Get size information from FSO

				exec sp_OAMethod @FileSystemObject,'GetDrive', @FsoDrive out, @driveletter

				exec sp_OAGetProperty @FsoDrive,'TotalSize', @TotalDiskSizeBytes out

				exec sp_OAGetProperty @FsoDrive,'FreeSpace', @FreeDiskSizeBytes out

				update #disk_drives 
					set total_size = case when @TotalDiskSizeBytes > isnull(total_size,0) then @TotalDiskSizeBytes else total_size end,
					unused_size = case when @FreeDiskSizeBytes > isnull(unused_size,0) then @FreeDiskSizeBytes else unused_size end
				where drive_letter = @driveletter

				-- Get throughput information from WMI
				if isnull(@WmiService,-1) > 0 
				begin 
					select @TotalDiskSizeBytes = null, @CounterObject = null, @AvgDiskQueueLength=null, @FreeDiskSizeBytes = null, @AvgDisksecPerRead  = null, 
					@AvgDisksecPerRead_Base = null, @AvgDisksecPerTransfer = null, @AvgDisksecPerTransfer_Base = null, @AvgDisksecPerWrite = null, 
					@AvgDisksecPerWrite_Base = null, @Frequency_Perftime = null

					select @CounterName = 'Win32_PerfRawData_PerfDisk_LogicalDisk.Name=''' +  rtrim(case when charindex(':',@driveletter) > 0 then @driveletter else @driveletter + ':' end) + '''' 

					exec sp_OAMethod @WmiService, 
		 				'Get', 
		 				@CounterObject output, 
		 				@CounterName

					exec sp_OAGetProperty @CounterObject, 
						'TimeStamp_Sys100NS', 
						@TimeStamp_Sys100NS output 

					-- The Percent Free Space base is equal to Total Disk Size
					-- However, this value is in megabytes and the FSO value is in bytes
					exec sp_OAGetProperty @CounterObject, 
						'PercentFreeSpace_Base', 
						@TotalDiskSizeBytes output 

					select @TotalDiskSizeBytes = cast(@TotalDiskSizeBytes as dec(38,18)) * 1024 * 1024

					exec sp_OAGetProperty @CounterObject, 
						'FreeMegabytes', 
						@FreeDiskSizeBytes output 

					select @FreeDiskSizeBytes = cast(@FreeDiskSizeBytes as dec(38,18)) * 1024 * 1024

					exec sp_OAGetProperty @CounterObject, 
						'PercentIdleTime', 
						@PercentIdleTime output 

					exec sp_OAGetProperty @CounterObject, 
						'PercentIdleTime_Base', 
						@PercentIdleTime_Base output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDiskQueueLength', 
						@AvgDiskQueueLength output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerRead', 
						@AvgDisksecPerRead output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerRead_Base', 
						@AvgDisksecPerRead_Base output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerTransfer', 
						@AvgDisksecPerTransfer output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerTransfer_Base', 
						@AvgDisksecPerTransfer_Base output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerWrite', 
						@AvgDisksecPerWrite output 

					exec sp_OAGetProperty @CounterObject, 
						'AvgDisksecPerWrite_Base', 
						@AvgDisksecPerWrite_Base output 

					exec sp_OAGetProperty @CounterObject, 
						'DiskReadsPerSec', 
						@DiskReadsPerSec output 

					exec sp_OAGetProperty @CounterObject, 
						'DiskTransfersPerSec', 
						@DiskTransfersPerSec output 

					exec sp_OAGetProperty @CounterObject, 
						'DiskWritesPerSec', 
						@DiskWritesPerSec output 

					exec sp_OAGetProperty @CounterObject, 
						'Frequency_Perftime', 
						@Frequency_Perftime output 

					exec sp_OAGetProperty @CounterObject, 
						'TimeStamp_PerfTime', 
						@TimeStamp_PerfTime output 

					{2}

					update #disk_drives 
						set 
						disk_idle = nullif(@PercentIdleTime,'0'),
						disk_idle_base = nullif(@PercentIdleTime_Base,'0'),
						disk_queue = nullif(@AvgDiskQueueLength,'0'),
						total_size = case 
							when isnull(total_size,0) = 0 then cast(@TotalDiskSizeBytes as  dec(38,0))
							when cast(@TotalDiskSizeBytes as dec(38,0)) between isnull(total_size,0) and isnull(total_size,0)*1.25 then @TotalDiskSizeBytes 
							else total_size end,
						unused_size = case when @FreeDiskSizeBytes > isnull(unused_size,0) then @FreeDiskSizeBytes else unused_size end,
						timestamp_sys100ns = nullif(@TimeStamp_Sys100NS,'0'),
						AvgDisksecPerRead  = nullif(@AvgDisksecPerRead ,'0'),
						AvgDisksecPerRead_Base = nullif(@AvgDisksecPerRead_Base,'0'),
						AvgDisksecPerTransfer = nullif(@AvgDisksecPerTransfer,'0'),
						AvgDisksecPerTransfer_Base = nullif(@AvgDisksecPerTransfer_Base,'0'),
						AvgDisksecPerWrite = nullif(@AvgDisksecPerWrite,'0'),
						AvgDisksecPerWrite_Base = nullif(@AvgDisksecPerWrite_Base,'0'),
						Frequency_Perftime = nullif(@Frequency_Perftime,'0'),
						DiskReadsPerSec = nullif(@DiskReadsPerSec,'0'),
						DiskTransfersPerSec = nullif(@DiskTransfersPerSec,'0'),
						DiskWritesPerSec = nullif(@DiskWritesPerSec,'0'),
						TimeStamp_PerfTime = nullif(@TimeStamp_PerfTime,'0')
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

if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1) 
begin 
	exec sp_OADestroy @FileSystemObject
	exec sp_OADestroy @WmiService 
	exec sp_OADestroy @WmiServiceLocator  
end

select *, getutcdate() from #disk_drives {3}

