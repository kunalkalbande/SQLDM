--------------------------------------------------------------------------------
--  Batch: DTC Status With Service Control
--  Tables: None
--  XSP: xp_servicecontrol
--	Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--  [3] - Direct WMI setting
--------------------------------------------------------------------------------
declare @directwmi int
		
set @directwmi={3}
if (@directwmi = 1)
begin
	select cast(serverproperty('MachineName') as varchar(255)) as [MachineName]
end
else
if (serverproperty('IsClustered') = 0 {2})
begin
	--------------------------------------------------------------------------------
	--  XP: DTC Status
	--  Returns:  SQL Server DTC service status
	--------------------------------------------------------------------------------
	declare @service_status int
    IF IS_SRVROLEMEMBER('sysadmin') = 1 
    BEGIN	
	exec @service_status = master..xp_servicecontrol N'QUERYSTATE', N'MSDTC' 
    END
	ELSE
	BEGIN
        SELECT @service_status = NULL
    END

	if @@rowcount = 0 and @service_status = 1 
	  select 'Not Installed' 
	else IF @service_status IS NULL
	begin
		select 'unable to monitor'
	end

end
else
begin

	declare @curconfig int, @disableole int

	select @disableole = {1}

	if (@disableole = 0)
	begin


		select @curconfig = value from master..syscurconfigs where config = 16388 

		if (isnull(@curconfig,1) > 0)
			select 
				@curconfig = (isnull(value,0) * -1) + 1
			from 
				master..syscurconfigs 
			where 
				config = 1546 

		
		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('master..SQLdmDisableOLE') is not null then 0 else 1 end

		if (isnull(@curconfig,1) > 0)
			select @curconfig = case when object_id('master..sp_OACreate') is not null then 1 else 0 end

	end

	declare 
		@MachineName varchar(255),
		@WmiServiceLocator int, 
		@WmiService int,
		@Win32_Service int, 
		@Win32_Process int,
		@Win32_Process_Collection int,
		@QueryString varchar(255),
		@ActiveService varchar(50),
		@State varchar(255)

	if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1) 
	begin 

		select @MachineName = cast(serverproperty('MachineName') as varchar(255)) 

		exec sp_OACreate 'WbemScripting.SWbemLocator', 
			@WmiServiceLocator output, {0}
		
		exec sp_OAMethod @WmiServiceLocator, 
			'ConnectServer', 
			@WmiService output, 
			'.', 
			'root\cimv2' 

		if isnull(@WmiService,-1) > 0 
		begin 
		
			select @ActiveService = 'MSDTC'

			select @QueryString = 'Win32_Service.Name=''' + @ActiveService + '''' 

			set @Win32_Service = -1

			exec sp_OAMethod @WmiService, 
				'get', 
				@Win32_Service output, 
				@QueryString 

			if @Win32_Service > -1
			begin
				exec sp_OAGetProperty @Win32_Service, 
					'State', 
					@State output
				end
			else
				set @State = 'Not Installed'
		
			select @State
		end 
	end 

	if (isnull(@curconfig,-1) > 0 and @disableole = 0 AND IS_SRVROLEMEMBER('sysadmin') = 1) 
	begin 
		exec sp_OADestroy @Win32_Process_Collection 
		exec sp_OADestroy @Win32_Service 
		exec sp_OADestroy @WmiService 
		exec sp_OADestroy @WmiServiceLocator  
	end
	
	if not(isnull(@WmiService,-1) > 0)
		select 'Unable to monitor' 
end