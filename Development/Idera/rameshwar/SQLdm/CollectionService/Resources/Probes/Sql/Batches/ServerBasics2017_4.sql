--------------------------------------------------------------------------------
--  Batch: Server Basics - 2005
--  Tables: None
--  XSP: xp_msver, xp_servicecontrol
--	Variables: @@servername or serverproperty('servername')
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--------------------------------------------------------------------------------

--------------------------------------------------------------------------------
--  Query: Server Access
--  Returns: "SA" if administrator, "Not Administrator" otherwise
--------------------------------------------------------------------------------
if is_srvrolemember('sysadmin') = 1 
	select 'sa' 
else 
	begin 
		select 'not administrator' 
		--return 
	end 

--------------------------------------------------------------------------------
--  Query: Server Name
--  Returns: Real server name from the monitored server
--------------------------------------------------------------------------------
declare @servername varchar(255)
select @servername = cast(serverproperty('servername') as nvarchar(255))
select @servername


--------------------------------------------------------------------------------
--  XP: Product version, Language, Processor Count and Physical Memory
--  Returns:
--		Index 
--		Name 
--		Internal_Value 
--		Character_Value  
--------------------------------------------------------------------------------
exec master..xp_msver 

IF IS_SRVROLEMEMBER('sysadmin') = 1 
BEGIN
  EXEC master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'SOFTWARE\MICROSOFT\Windows NT\CurrentVersion', N'ProductName' 
END
ELSE
BEGIN
  -- Available from SQL 2017+
  SELECT 'ProductName' AS 'Value', host_distribution AS 'Data' FROM sys.dm_os_host_info
END

select serverproperty('MachineName')
select serverproperty('Edition')


--------------------------------------------------------------------------------
--  Query: Show Advanced Options
--  Returns: Nothing
--  Notes: Turn on "show advanced options" if it is not already enabled
--------------------------------------------------------------------------------
if (select count(*) from master..syscurconfigs where value <> 1 and config = 518) > 0 AND IS_SRVROLEMEMBER('sysadmin') = 1
begin 
    execute master..sp_configure 'show advanced options', 1  
    reconfigure with override 
end 

--Query: Default sql server instances
--Returns: List of default server instances 
IF IS_SRVROLEMEMBER('sysadmin') = 1 
  EXEC xp_regread 'HKEY_LOCAL_MACHINE', 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL', 'MSSQLSERVER' 
ELSE
  SELECT 'MSSQLSERVER' Value, 'UNKNOWN' Data

declare @directwmi int
set @directwmi={3}
if (@directwmi = 0)
begin

	declare 
		@service_status int,
		@slashpos int, 
		@sqlservicename varchar(50), 
		@agentservicename varchar(50),
		@fulltextservicename varchar(50)

	select @servername = upper(@servername) 

	select @slashpos = charindex('\', @servername)  

	if @slashpos <> 0 
		begin 
			select @sqlservicename = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) 
			select @agentservicename = 'SQLAGENT$' + substring(@servername, @slashpos + 1, 30) 
			select @fulltextservicename = 'MSFTESQL$' + substring(@servername, @slashpos + 1, 30) 
		end  
	else 
		begin 
			select @sqlservicename = 'MSSQLSERVER' 
			select @agentservicename = 'SQLSERVERAGENT' 
			select @fulltextservicename = 'MSFTESQL' 
		end  


	if serverproperty('IsClustered') = 0 {2}
	begin

	--------------------------------------------------------------------------------
	--  XP: Agent Status
	--  Returns:  SQL Server Agent service status
	--------------------------------------------------------------------------------
	-- Modified for reduced permissions instead of master..xp_servicecontrol N'querystate', @agentservicename 
	SELECT @service_status = sys.dm_server_services.status FROM sys.dm_server_services where servicename = @agentservicename 
	
	if @service_status is null
	begin
		if @slashpos <> 0
			select @agentservicename = 'SQL Server Agent (' + substring(@servername, @slashpos + 1, 30) + ')' 
		else
			select @agentservicename = 'SQL Server Agent (MSSQLSERVER)' 

		SELECT @service_status = sys.dm_server_services.status FROM sys.dm_server_services where servicename = @agentservicename
	end

	if @service_status is null
		select 'unable to monitor'
	-- TODO: It is needed to find out the code that is related to 'not installed' service from sys.dm_server_services
	--else if @service_status = 1
	--	select 'not installed' 
	else
		SELECT sys.dm_server_services.status_desc FROM sys.dm_server_services where servicename = @agentservicename 

	select 0


	--------------------------------------------------------------------------------
	--  XP: MS Search Status
	--  Returns:  MS Search status
	--------------------------------------------------------------------------------
    IF IS_SRVROLEMEMBER('sysadmin') = 1 
    BEGIN
    	exec @service_status = master..xp_servicecontrol N'QUERYSTATE',@fulltextservicename
    END
    ELSE
    BEGIN
        SELECT @service_status = NULL
    END
	if @@rowcount = 0 and @service_status = 1 
		select 'not installed' 
    ELSE IF @service_status IS NULL
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
			@WmiServiceLocator int, 
			@WmiService int,
			@Win32_Service int, 
			@Win32_Process int,
			@Win32_Process_Collection int,
			@MachineName varchar(255),
			@QueryString varchar(255),
			@ServiceNameEnumerationControl int,
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
			
				set @ServiceNameEnumerationControl = 0
			
				while @ServiceNameEnumerationControl < 4
				begin
					select @ActiveService = 
						case
							when @ServiceNameEnumerationControl = 0
								then @agentservicename 
							when @ServiceNameEnumerationControl = 2
								then @fulltextservicename
							when @ServiceNameEnumerationControl = 3 
								then @sqlservicename
							else
								'MSDTC'
						end

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
			
					select 
						@ServiceNameEnumerationControl,
						@State

					set @ServiceNameEnumerationControl = @ServiceNameEnumerationControl + 1
				end
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
		begin
			select @ServiceNameEnumerationControl = 0
			while @ServiceNameEnumerationControl < 3
			begin
				select 
						@ServiceNameEnumerationControl,
						'Unable to monitor' 

				set @ServiceNameEnumerationControl = @ServiceNameEnumerationControl + 1
			end
		end


	end
end