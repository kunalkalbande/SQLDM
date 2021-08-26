--------------------------------------------------------------------------------
--  Batch: Replication Status
--  [0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Cluster setting
--  [3] - Direct WMI
--------------------------------------------------------------------------------
declare 
@distributor nvarchar(255), 
@DistributionDB nvarchar(255), 
@retcode int, 
@servername nvarchar(255),
@service_status int,
@slashpos int, 
@agentservicename nvarchar(50),
@directwmi int

select @servername = cast(serverproperty('servername') as nvarchar)

exec @retcode = sp_helpdistributor @distributor = @distributor output, @distribdb = @DistributionDB output 

if @@error <> 0 OR @retcode <> 0 OR @distributor IS NULL 
begin
	if @DistributionDB IS NULL 
	begin 
		select 'replicationstate',1
	end 
	else 
	begin 
		select @distributor = @servername 
		select 'replicationstate',3
	end 
end 
else 
begin 
	if @distributor <> @servername 
	begin
		select 'replicationstate',2
	end 
	else 
	begin 
		if @DistributionDB IS NULL 
			begin 
				select 'replicationstate',0
			end 
			else 
				select 'replicationstate',3
			end 
	end 


select 
	'sp_helpdistributor', 
	distributor = @distributor, 
	distributiondb = @DistributionDB

select @servername = cast(serverproperty('servername')  as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @agentservicename = 'SQLAGENT$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 

		select @agentservicename = 'SQLSERVERAGENT' 
	end  

select @directwmi={3}
if (@directwmi = 1)
begin
	select cast(serverproperty('MachineName') as varchar(255)), @agentservicename
end
else
if serverproperty('IsClustered') = 0 {2}
begin
	--------------------------------------------------------------------------------
	--  XP: Agent Status
	--  Returns:  SQL Server Agent service status
	--------------------------------------------------------------------------------
    IF IS_SRVROLEMEMBER('sysadmin') = 1 
    BEGIN	
	    exec @service_status = master..xp_servicecontrol N'querystate', @agentservicename
    END
	ELSE
	BEGIN
        SELECT @service_status = NULL
    END
	
	if @service_status = 1
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
		@MachineName nvarchar(255),
		@QueryString nvarchar(255),
		@State nvarchar(255)

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
			
				select @QueryString = N'Win32_Service.Name=''' + @agentservicename + '''' 

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
