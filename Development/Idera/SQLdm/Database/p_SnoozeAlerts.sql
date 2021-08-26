if (object_id('p_SnoozeAlerts') is not null)
begin
drop procedure [p_SnoozeAlerts]
end
go

create procedure [p_SnoozeAlerts]
	@SQLServerID int,
	@Metric int,
	@SnoozeMinutes int,
	@SnoozeStart datetime output,
	@SnoozeEnd datetime output,
	@SnoozeStartUser nvarchar(255) output,
	@SnoozeEndUser nvarchar(255) output
as
begin
	DECLARE @err int
	DECLARE @Now datetime
	DECLARE @NotNow datetime
	DECLARE @End datetime
	DECLARE @Start datetime
	DECLARE @StartUser nvarchar(255)
	DECLARE @EndUser nvarchar(255)
	DECLARE @InstanceName nvarchar(256)
	DECLARE @LastAlertRefreshTime datetime

	set @Now = GetUTCDate()
	set @End = DateAdd(minute, @SnoozeMinutes, @Now)	
	set @NotNow = DateAdd(d,-5,@Now)

	select	@InstanceName = [InstanceName], 
			@LastAlertRefreshTime = [LastAlertRefreshTime]
		from MonitoredSQLServers
		where SQLServerID = @SQLServerID

	set @StartUser = @SnoozeStartUser

	set @EndUser = @SnoozeEndUser
	if (@EndUser is null)
		set @EndUser = @SnoozeStartUser

	if (@Start is null)
		set @Start = @Now

	UPDATE MetricThresholds SET
			[UTCSnoozeStart] = 
				case when coalesce([UTCSnoozeEnd],@NotNow) < @Now then @Now else [UTCSnoozeStart] end,
			[SnoozeStartUser] = 
				case when coalesce([UTCSnoozeEnd],@NotNow) < @Now then @StartUser else [SnoozeStartUser] end,
			[UTCSnoozeEnd] = @End,
			[SnoozeEndUser] = @EndUser
		WHERE [SQLServerID] = @SQLServerID 
			and (@Metric is null or [Metric] = @Metric) 
			and ([Metric] not in (48,56))
	-- kill the active flag for existing alerts
	update Alerts set [Active] = 0 
	where [ServerName] = @InstanceName and 
	  	  [UTCOccurrenceDateTime] = @LastAlertRefreshTime and
		  [Active] = 1 and 
		  (@Metric is null or [Metric] = @Metric) and
		  ([Metric] not in (48,56))


	SELECT @err = @@error
	
	SELECT @SnoozeEnd = @End	
	SELECT @SnoozeEndUser = @EndUser	
	SELECT @SnoozeStart = @Now
	SELECT @SnoozeStartUser = @StartUser	

	RETURN @err
end
