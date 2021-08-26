--------------------------------------------------------------------------------
--  Batch: Full Text Search Service 2000
--  Tables: None
--  Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--------------------------------------------------------------------------------
declare 
	@service_status int


if serverproperty('IsClustered') = 0 {2}
begin
	--------------------------------------------------------------------------------
	--  XP: MS Search Status
	--  Returns:  MS Search status
	--------------------------------------------------------------------------------
	exec @service_status = master..xp_servicecontrol N'QUERYSTATE', N'MSSearch' 
	if @@rowcount = 0 and @service_status = 1 
		select 'not installed' 
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

	if (isnull(@curconfig,-1) > 0 and @disableole = 0) 
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
			
				select @QueryString = 'Win32_Service.Name=''MSSearch''' 

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

	if (isnull(@curconfig,-1) > 0 and @disableole = 0) 
	begin 
		exec sp_OADestroy @Win32_Process_Collection 
		exec sp_OADestroy @Win32_Service 
		exec sp_OADestroy @WmiService 
		exec sp_OADestroy @WmiServiceLocator  
	end
	
	if not(isnull(@WmiService,-1) > 0)
		select 'Unable to monitor' 
end
