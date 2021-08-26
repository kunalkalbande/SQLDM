--------------------------------------------------------------------------------
--  Batch: Full Text Search Service 2005
--  Tables: None
--  Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--  [3] - Direct WMI
--------------------------------------------------------------------------------
declare 
	@servername varchar(255),
	@service_status int,
	@slashpos int, 
	@fulltextservicename varchar(50),
	@Version varchar(20),
	@Major int,
	@directwmi int

-- Starting with SQL Server 2008 FTE was merged with the SQL Server process
--  so just returing 'not installed' here so that the option to stop the 
--  service does not appear.	
-- This section for SQL Server 2008 and above should really be put into a 
--  a new batch on its own and the BatchFinder.cs updated to pull the right 
--  batch.  Need to talk to Vicky about this.
select @Version = convert(varchar, serverproperty('ProductVersion'))
select @Major = convert(int, substring(@Version, 1, (charindex('.', @Version,1)-1)))

select @servername = cast(serverproperty('servername')  as nvarchar(255))
select @servername = upper(@servername) 
select @slashpos = charindex('\', @servername) 

if @slashpos <> 0 
begin 
	select @fulltextservicename = 'MSFTESQL$' + substring(@servername, @slashpos + 1, 30) 
end  
else 
begin 
	select @fulltextservicename = 'MSFTESQL' 
end  

select @directwmi={3}
if (@directwmi=1)
begin
	select cast(serverproperty('MachineName') as varchar(255)) as [MachineName], @fulltextservicename as [ServiceName], @Major as [MajorVersion] 
end
else
if @Major >= 10
	begin
		select 'unable to monitor'
	end
else
begin

if serverproperty('IsClustered') = 0 {2}
begin
	--------------------------------------------------------------------------------
	--  XP: MS Search Status
	--  Returns:  MS Search status
	--------------------------------------------------------------------------------
	--exec @service_status = master..xp_servicecontrol N'QUERYSTATE',@fulltextservicename
	--if @@rowcount = 0 and @service_status = 1 
	--	select 'not installed' 
	SELECT 'Unable to monitor'
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
			
				select @QueryString = 'Win32_Service.Name=''' + @fulltextservicename + '''' 

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
					@State

			
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
end