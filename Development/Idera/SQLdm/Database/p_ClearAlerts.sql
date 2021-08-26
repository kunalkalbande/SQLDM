if (object_id('p_ClearAlerts') is not null)
begin
drop procedure [p_ClearAlerts]
end
go

create procedure [p_ClearAlerts] 
	@ServerName nvarchar(256),
	@HighestAlertID bigint,
	@ClearAll bit,
	@Metric int,
	@AlertID bigint
as
begin
	
declare @hash nvarchar(28)
declare @server nvarchar(256)
declare @database nvarchar(256)
declare @table nvarchar(256)
declare @msid int
declare @dbid int
declare @tbid int
declare @type int
declare @e int


	if (@AlertID is not null)
	begin
		-- @Metric is not allowed if @AlertID is specified
		if (@Metric is not null)
		begin
			RAISERROR('@Metric is not allowed when @AlertID is specified.', 10, 1);
			RETURN -1;
		end
		-- get server name, metric and hash using the alert id
		select @server = [ServerName],
			   @database = [DatabaseName],
			   @table = [TableName],
			   @type = [Metric],
			   @hash = [QualifierHash]
		from Alerts where [AlertID] = @AlertID

		if (@@ROWCOUNT = 0)
		begin
			RAISERROR('Alert not found to clear.', 10, 1);
			RETURN -2;
		end
		
		select @msid = [SQLServerID] from MonitoredSQLServers where [InstanceName] = @server

		if (@ClearAll = 1) 
		begin
			-- clear all alerts
			update Alerts set [Active] = 0
				where [Active] = 1 
					and [ServerName] = @server 
					and [Metric] = @type
		end
		else
		begin
			-- clear single alert
			update Alerts set [Active] = 0
				where [AlertID] = @AlertID			
		end

		select @msid, @type, @server, @database, @hash

		select @e = @@error
		return @e
	end

	
	if (@ClearAll = 1) 
	begin
		-- clear all alerts
		update Alerts set Active = 0
			where [Active] = 1
				and not ([AlertID] > @HighestAlertID) 
				and [ServerName] = @ServerName 
				and (@Metric is null or @Metric = [Metric])
		select @e = @@error
	end
	else
	begin
		-- clear matching alerts
		update Alerts set Active = 0 
			where [AlertID] in 
				(select S.[AlertID] from Alerts P
					left join Alerts S  
					on  S.[ServerName] = P.[ServerName]
					and S.[Metric] = P.[Metric]
					and (S.[DatabaseName] = P.[DatabaseName] or (S.[DatabaseName] is null and P.[DatabaseName] is null))
					and (S.[TableName] = P.[TableName] or (S.[TableName] is null and P.[TableName] is null))
					and (S.[QualifierHash] = P.[QualifierHash] or (S.[QualifierHash] is null and P.[QualifierHash] is null))
					where P.[AlertID] > @HighestAlertID
						and P.[ServerName] = @ServerName
						and not S.[AlertID] > @HighestAlertID 
						and S.[Active] = 1
				)	
		select @e = @@error			
	end

	return @e
end