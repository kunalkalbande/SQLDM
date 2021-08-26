--------------------------------------------------------------------------------
--  Batch: Services 2005
--  Tables: None
--  Variables: 
--  [0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--  [3] - Direct WMI
--------------------------------------------------------------------------------

declare 
	@servername varchar(255),
	@service_status int,
	@slashpos int, 
	@sqlservicename varchar(50), 
	@agentservicename varchar(50),
	@fulltextservicename varchar(50),
	@directwmi int

select @servername = cast(serverproperty('servername')  as nvarchar(255))

set @directwmi={3}
if (@directwmi = 1)
begin
	select cast(serverproperty('MachineName') as varchar(255)), @servername
end
else
begin
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
	--exec @service_status = master..xp_servicecontrol N'querystate', @agentservicename

	--if @service_status = 1
	--begin
	--	select 'not installed' 
	--end
	select 'Unable to monitor' 

	--------------------------------------------------------------------------------
	--  XP: SQL Server Status
	--  Returns:  SQL Server service status
	--------------------------------------------------------------------------------
	--exec @service_status = master..xp_servicecontrol N'querystate', @sqlservicename 

	--if @service_status = 1 
	--	select 'not installed' 

	--end
	select 'Unable to monitor' 

	end

	declare @curconfig int, @disableole int

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
				select 'unavailable due to lightweight pooling','true'
		end


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
		@ProcessId varchar(255),
		@CreationDate varchar(255),
		@StartMode varchar(255),
		@StartName varchar(255),
		@Description varchar(1000),
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

				exec sp_OAMethod @WmiService, 
					'get', 
					@Win32_Service output, 
					@QueryString 


				if @Win32_Service > -1
				begin
					exec sp_OAGetProperty @Win32_Service, 
						'State', 
						@State output

					exec sp_OAGetProperty @Win32_Service, 
						'StartMode', 
						@StartMode output 

					exec sp_OAGetProperty @Win32_Service, 
						'StartName', 
						@StartName output 

					exec sp_OAGetProperty @Win32_Service, 
						'Description', 
						@Description output 

					exec sp_OAGetProperty @Win32_Service, 
						'ProcessId', 
						@ProcessId output 
				
					-- set @QueryString = 'Select * from Win32_Process Where ProcessID = ' + @ProcessId

					-- exec sp_OAMethod @WmiService, 
					-- 	'execQuery', 
					-- 	@Win32_Process_Collection output, 
					-- 	@QueryString

					-- set @QueryString = 'Win32_Process.Handle="' + @ProcessId + '"'

					-- exec sp_OAGetProperty 
					-- 	@Win32_Process_Collection, 
					-- 	'Item', 
					-- 	@Win32_Process output,
					-- 	@QueryString

					set @QueryString = 'Win32_Process.Handle="' + @ProcessId + '"'

					exec sp_OAGetProperty @WmiService, 
				 		'Get', 
				 		@Win32_Process output,
				 		@QueryString

					exec sp_OAGetProperty @Win32_Process, 
						'CreationDate', 
						@CreationDate output

					begin try 
						-- Assuming that the date returned is in the WMI Date format, so attempting to convert from WMI Date Format to just a datetime
						select @CreationDate = 
							cast(stuff(stuff(stuff(stuff(stuff(left(@CreationDate,isnull(Charindex('.',@CreationDate),1)-1),13,0,':'),11,0,':'),9,0,' '),7,0,'-'),5,0,'-') as datetime)
						
					end try
					begin catch 
						-- For at least one customer there seems to be a bug in WMI where sometimes the format of the CreationDate property is not returned 
						--	in WMI Date format but in just a standard datetime format.  When that is the case, we do not need to convert the value returned
						--	from the CreationDate property
					end catch
			
				end
				else
				begin
						select 
							@State = 'Not Installed',
							@StartMode = null,
							@StartName = null,
							@Description = null,
							@ProcessId = null,
							@CreationDate = null
				end

				select 
					@ServiceNameEnumerationControl,
					@ActiveService,
					cast(nullif(@ProcessId,0) as bigint),
					dateadd(mi,datediff(mi,getdate(),getutcdate()),@CreationDate),
					@StartMode,
					@StartName,
					@Description,
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
					null,
					null,
					null,
					null,
					null,
					null,
					'Unable to monitor' 

			set @ServiceNameEnumerationControl = @ServiceNameEnumerationControl + 1
		end
		select 
			4,
			null,
			null,
			null,
			null,
			null,
			null,
			'Running' 
	end


	select '1'

	if serverproperty('IsClustered') = 0
	begin
	--------------------------------------------------------------------------------
	--  XP: MS Search Status
	--  Returns:  MS Search status
	--------------------------------------------------------------------------------
	--exec @service_status = master..xp_servicecontrol N'QUERYSTATE',@fulltextservicename
	--if @@rowcount = 0 and @service_status = 1 
	--	select 'not installed' 

	--end
	select 'Unable to monitor' 

	end
end