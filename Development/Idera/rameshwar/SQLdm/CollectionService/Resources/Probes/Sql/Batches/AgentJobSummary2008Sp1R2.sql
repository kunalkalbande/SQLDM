--------------------------------------------------------------------------------
--  Batch: Agent Job Summary
--  Tables:  msdb..sysjobhistory , master..syscurconfigs 
--  Stored Procedures: msdb..sp_get_composite_job_info
--  Variables: 
--  [0] - sp_OACreate context
--	[1] - Filter for running jobs
--	[2] - Disable OLE
--  [3] - Cluster setting
--  [4] - Direct WMI Enabled
--  [5] - Is SQL Server Edition Express -- SQLdm 8.6 (Ankit Srivastava)

--------------------------------------------------------------------------------
declare 
	@service_status int,
	@slashpos int, 
	@agentservicename varchar(50),
	@servername varchar(255),
	@directwmi int,
	@isExpress bit -- SQLdm 8.6 (Ankit Srivastava) --- if the edition is express

select @servername = cast(serverproperty('servername')  as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

select @isExpress = {5}

if(@isExpress = 0) -- SQLdm 8.6 (Ankit Srivastava) --- skip the batch if the edition is express
begin
if @slashpos <> 0 
	begin 
		select @agentservicename = 'SQLAGENT$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 

		select @agentservicename = 'SQLSERVERAGENT' 
	end  

if serverproperty('IsClustered') = 0 {3}
begin
	--------------------------------------------------------------------------------
	--  XP: Agent Status
	--  Returns:  SQL Server Agent service status
	--------------------------------------------------------------------------------
	-- Alternative for reduced permissions instead of master..xp_servicecontrol N'querystate'
	SELECT @service_status = sys.dm_server_services.status FROM sys.dm_server_services where servicename = @agentservicename 

	if @service_status is null and @slashpos <> 0
	begin
		select @agentservicename = 'SQL Server Agent (' + substring(@servername, @slashpos + 1, 30) + ')' 
		SELECT sys.dm_server_services.status_desc FROM sys.dm_server_services where servicename = @agentservicename 
	end
	else if @service_status = 1
	begin
		select 'not installed' 
	end
	else IF @service_status IS NULL
	begin
		select 'unable to monitor'
	end
end
else
begin
	declare @curconfig int, @disableole int

	select @disableole = {2}

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
			
				select @QueryString = 'Win32_Service.Name=''' + @agentservicename + '''' 

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

select @directwmi = {4}
if (@directwmi = 1)
begin
	select cast(serverproperty('MachineName') as varchar(255)), @servername
end

select
 job_id,
 run_duration
from
 msdb..sysjobhistory sj1 (nolock)
where
 sj1.step_id = 0
  and (convert(datetime, stuff(stuff(str(sj1.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(sj1.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)) = 
	(
		select 
			max(convert(datetime, stuff(stuff(str(sj2.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(sj2.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)) 
		from 
			msdb..sysjobhistory sj2 (nolock) 
		where 
			sj2.step_id = 0 
			and sj2.job_id = sj1.job_id
	)

declare @agentxpavailable int 

select 
	@agentxpavailable = value 
from 
	master..syscurconfigs 
where 
	config = 16384 

if (isnull(@agentxpavailable,1) > 0) 
begin
	exec msdb..sp_get_composite_job_info {1}
End 
End