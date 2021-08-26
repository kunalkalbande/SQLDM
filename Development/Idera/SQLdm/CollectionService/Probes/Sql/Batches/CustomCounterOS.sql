-- [0] - sp_OACreate context
-- [1] - Object Name
-- [2] - Instance Name String
-- [3] - Counter Name
-- [4] - Wait time
-- [5] - Disable OLE

declare @curconfig int, @disableole int

select @disableole = {5}

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

if (isnull(@curconfig,-1) > 0 and @disableole = 0) 
begin 
	declare 
		@WmiServiceLocator int, 
		@WmiService int,
		@Refresher int, 
		@CounterObject int, 
		@CounterValue varchar(50),
		@MachineName varchar(255),
		-- error stuff
		@ErrObject int,
		@ErrSource nvarchar(1024),
		@ErrDescription nvarchar(256),
		@HResult int 

	select @MachineName = cast(serverproperty('MachineName') as varchar(255)) 

	set @ErrObject = null
	set @ErrSource = 'Create WbemScripting.SWbemLocator'
	exec @HResult = sp_OACreate 'WbemScripting.SWbemLocator', 
		@WmiServiceLocator output, {0}
	
	if (@HResult = 0) 
	begin
		set @ErrObject = @WmiServiceLocator
		set @ErrSource = 'Connect to root\cimv2'
		exec @HResult = sp_OAMethod @WmiServiceLocator, 
			'ConnectServer', 
			@WmiService output, 
			'.', 
			'root\cimv2' 
		
		if (@HResult = 0 and isnull(@WmiService,-1) > 0) 
		begin
			set @ErrObject = @WmiService
			set @ErrSource = 'Get path="{1}{2}"'
			exec @HResult = sp_OAMethod @WmiService, 
				'Get', 
				@Refresher output, 
				'{1}{2}' 
			
			if (@HResult = 0) 
			begin
				set @ErrObject = @Refresher
				set @ErrSource = 'Execute Refresh method (1)'
				exec @HResult = sp_OAMethod @Refresher, 'Refresh_'
				if (@HResult = 0 or @HResult = -2147352570)
				begin
					waitfor delay '{4}'

					set @ErrObject = @Refresher
					set @ErrSource = 'Execute Refresh method (2)'
					exec @HResult = sp_OAMethod @Refresher, 'Refresh_'
					if (@HResult = 0 or @HResult = -2147352570)
					begin
						set @ErrObject = @Refresher
						set @ErrSource = 'Get property {3}'
						exec @HResult = sp_OAGetProperty @Refresher, 
							'{3}', 
							@CounterValue output 
						
						if (@HResult = 0)
						begin
							select 'available'
							select 	@CounterValue as CounterValue
						end
					end
				end
			end
		end
	end	

	if (@HResult <> 0)
	begin
		exec sp_OAGetErrorInfo @ErrObject, null, @ErrDescription output 
		select @HResult, @ErrSource, @ErrDescription
	end
end 
else
	select 'OLE Automation disabled'

if (isnull(@curconfig,-1) > 0 and @disableole = 0) 
begin 
	exec sp_OADestroy @CounterObject 
	exec sp_OADestroy @Refresher 
	exec sp_OADestroy @WmiService 
	exec sp_OADestroy @WmiServiceLocator  
end
