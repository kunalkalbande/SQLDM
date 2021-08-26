--  Batch: OS Metrics
--  Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Waitfor time
--  [3] - Direct WMI
--------------------------------------------------------------------------------

declare @curconfig int, @disableole int, @directwmi int
declare @MachineName varchar(255) 

select @MachineName = cast(serverproperty('MachineName') as varchar(255)) 

select @directwmi={3}
if (@directwmi = 1)
begin
	select @MachineName, cast(serverproperty('servername')  as nvarchar(255)), getutcdate() as UTCTimeStamp
end
else
begin
	select @disableole = {1}
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
			@WmiServiceLocator int, 
			@WmiService int,
			@CounterCollection int, 
			@CounterObject int, 
			@CounterName varchar(255), 
			@TotalPhysicalMemory varchar(21),  
			@AvailableBytes varchar(21), 
			@PagesPersec varchar(21),
			@PercentProcessorTime varchar(21),
			@PercentSQLProcessorTime varchar(21),  
			@PercentPrivilegedTime varchar(21), 
			@PercentUserTime varchar(21), 
			@ProcessorQueueLength varchar(21), 
			@PercentIdleTime varchar(21), 
			@AvgDiskQueueLength varchar(21), 
			@TimeStamp_Sys100NS varchar(21), 
			@TimeStamp_PerfTime varchar(21), 
			@PercentIdleTime_Base varchar(21),	
			@Frequency_PerfTime varchar(21) 
		
		exec sp_OACreate 'WbemScripting.SWbemLocator', 
			@WmiServiceLocator output, {0}
	
		exec sp_OAMethod @WmiServiceLocator, 
			'ConnectServer', 
			@WmiService output, 
			'.', 
			'root\cimv2' 

		if isnull(@WmiService,-1) > 0 
		begin 

			create table #tempversion(i int, Name nvarchar(100), Internal_Value bigint, Character_Value nvarchar(100))

			insert into #tempversion
				execute xp_msver 'WindowsVersion'

			declare @windowsversion int
			select @windowsversion = cast(substring(Character_Value,0,3) as float) from #tempversion

			drop table #tempversion 

			select 'available' 
		
			set @CounterName = 'Win32_ComputerSystem.Name="' + @MachineName + '"' 
		
			exec sp_OAMethod @WmiService, 
				'Get', 
				@CounterObject output, 
				@CounterName 

			exec sp_OAGetProperty @CounterObject, 
				'TotalPhysicalMemory', 
				@TotalPhysicalMemory output 
		
			exec sp_OAMethod @WmiService, 
				'Get', 
				@CounterObject output, 
				'Win32_PerfRawData_PerfOS_Memory=@' 

			exec sp_OAGetProperty @CounterObject, 
				'AvailableBytes', 
				@AvailableBytes output 

			exec sp_OAGetProperty @CounterObject, 
				'PagesPersec', 
				@PagesPersec output 

			exec sp_OAGetProperty @CounterObject, 
				'TimeStamp_Sys100NS', 
				@TimeStamp_Sys100NS output 

			exec sp_OAGetProperty @CounterObject, 
				'TimeStamp_PerfTime', 
				@TimeStamp_PerfTime output 

			exec sp_OAGetProperty @CounterObject, 
				'Frequency_PerfTime', 
				@Frequency_PerfTime output 

			 exec sp_OAMethod @WmiService, 
		 		'Get', 
		 		@CounterObject output, 
		 		'Win32_PerfRawData_PerfOS_Processor="_Total"' 

			exec sp_OAGetProperty @CounterObject, 
				'PercentProcessorTime', 
				@PercentProcessorTime output 

			exec sp_OAGetProperty @CounterObject, 
				'PercentPrivilegedTime', 
				@PercentPrivilegedTime output 

			exec sp_OAGetProperty @CounterObject, 
				'PercentUserTime', 
				@PercentUserTime output 

			exec sp_OAMethod @WmiService, 
		 		'Get', 
		 		@CounterObject output, 
		 		'Win32_PerfRawData_PerfOS_System=@' 

			exec sp_OAGetProperty @CounterObject, 
				'ProcessorQueueLength', 
				@ProcessorQueueLength output 

			exec sp_OAMethod @WmiService, 
		 		'Get', 
		 		@CounterObject output, 
		 		'Win32_PerfRawData_PerfDisk_PhysicalDisk.Name="_Total"' 

			exec sp_OAGetProperty @CounterObject, 
				'PercentIdleTime', 
				@PercentIdleTime output 

			exec sp_OAGetProperty @CounterObject, 
				'PercentIdleTime_Base', 
				@PercentIdleTime_Base output 

			exec sp_OAGetProperty @CounterObject, 
				'AvgDiskQueueLength', 
				@AvgDiskQueueLength output
 				
 			exec sp_OAMethod @WmiService, 
 		 		'execQuery', 
 		 		@CounterCollection output, 
 		 		'select * from win32_PerfRawData_PerfProc_Process where IDProcess={4}'
 				
 			exec sp_OAMethod @CounterCollection,
 				'ItemIndex(0)', 
 				@CounterObject output				 
 
 			exec sp_OAGetProperty @CounterObject, 
 				'PercentProcessorTime', 
 				@PercentSQLProcessorTime output 

			if  ((@windowsversion >= 6.0))-- and (isnull(cast(@PercentIdleTime_Base as float),0) = 0))
			begin

				exec sp_OADestroy @CounterObject

				select @CounterName = 'Win32_PerfFormattedData_PerfDisk_PhysicalDisk.Name="_Total"'  

				exec sp_OAMethod @WmiService, 
					'Get', 
					@CounterObject output, 
					@CounterName

				exec sp_OAMethod @CounterObject, 'Refresh_'

				waitfor delay '{2}'

				exec sp_OAMethod @CounterObject, 'Refresh_'

		
				exec sp_OAGetProperty @CounterObject, 
					'PercentIdleTime', 
					@PercentIdleTime output 
				set @PercentIdleTime_Base = -1
		
			end

			select 
				@TotalPhysicalMemory as TotalPhysicalMemory, 
				@AvailableBytes as AvailableBytes, 
				@PagesPersec as PagesPersec, 
				@PercentProcessorTime as PercentProcessorTime, 
				@PercentPrivilegedTime as PercentPrivilegedTime,  
				@PercentUserTime as PercentUserTime,  
				@ProcessorQueueLength as ProcessorQueueLength,  
				@PercentIdleTime as PercentIdleTime,  
				@AvgDiskQueueLength as AvgDiskQueueLength,  
				@TimeStamp_Sys100NS as TimeStamp_Sys100NS,  
				@TimeStamp_PerfTime as Timestamp_PerfTime,  
				@PercentIdleTime_Base as PercentIdleTime_Base,  
				@Frequency_PerfTime as Frequency_PerfTime,
				getutcdate() as UTCTimeStamp,
				@PercentSQLProcessorTime as PercentSQLProcessorTime 			

			exec master..xp_msver 'physicalmemory' 

		end 
		else 

		select 'service unavailable' 
	end 
	else 
	begin
		if @disableole = 1
			select 'ole automation disabled',total_physical_memory_kb from sys.dm_os_sys_memory;
		else
			select 'procedure unavailable' 
	end
	if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1)
	begin 
		exec sp_OADestroy @CounterObject 
		exec sp_OADestroy @CounterCollection 
		exec sp_OADestroy @WmiService 
		exec sp_OADestroy @WmiServiceLocator  
	end
end


declare @sysperfinfoname_trans sysname
Set @sysperfinfoname_trans = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
select @sysperfinfoname_trans = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname_trans = SUBSTRING(@sysperfinfoname_trans, 1, PATINDEX('%:%',@sysperfinfoname_trans)-1)

select 
	'total pages', 
	isnull(sum(convert(bigint,cntr_value/8)),0)
from 
	sys.dm_os_performance_counters 
where (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
	and Lower(counter_name) = 'total server memory (kb)' )
union
select 
	lower(RTRIM(counter_name)), 
	isnull(sum(convert(bigint,cntr_value)),0)
from 
	sys.dm_os_performance_counters 
where 
	(lower(object_name) = lower(@sysperfinfoname_trans + ':SQL Statistics')
		and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':Buffer Manager') 
		and (Lower(counter_name) like 'buffer cache hit ratio%'
		or Lower(counter_name) = 'total pages')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':plan cache') 
		and Lower(counter_name) like 'cache hit ratio%' 
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':databases') 
		and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
		and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':access methods') 
		and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':locks') 
		and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':buffer manager') 
		and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
group by 
	counter_name 
	
select NULL AS avg_cpu_percent

