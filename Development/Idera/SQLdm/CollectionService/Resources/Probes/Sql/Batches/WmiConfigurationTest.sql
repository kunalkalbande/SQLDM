--  Batch: WmiConfigurationTest
--  Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Enable direct WMI
--------------------------------------------------------------------------------

declare @curconfig int, @disableole int, @enabledirect int
declare	@MachineName varchar(255) 

select @MachineName = cast(serverproperty('MachineName') as varchar(255)) 

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
		@TotalPhysicalMemory varchar(21)

	exec sp_OACreate 'WbemScripting.SWbemLocator', 
		@WmiServiceLocator output, {0}
	
	exec sp_OAMethod @WmiServiceLocator, 
		'ConnectServer', 
		@WmiService output, 
		'.', 
		'root\cimv2' 

	if isnull(@WmiService,-1) > 0 
	begin 
		select 'available' 
		
		set @CounterName = 'Win32_ComputerSystem.Name="' + @MachineName + '"' 
		
		exec sp_OAMethod @WmiService, 
			'Get', 
			@CounterObject output, 
			@CounterName 

		exec sp_OAGetProperty @CounterObject, 
			'TotalPhysicalMemory', 
			@TotalPhysicalMemory output 
		
		select 
			@TotalPhysicalMemory as TotalPhysicalMemory

	end 
	else 
	select 'service unavailable' 
end 
else 
begin
	if @disableole = 1
		select 'ole automation disabled'
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

select @enabledirect={2}
if (@enabledirect = 1)
begin
	select @MachineName, cast(serverproperty('servername')  as nvarchar(255))
end

