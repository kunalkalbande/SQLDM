if (object_id('p_UnsnoozeAlerts') is not null)
begin
drop procedure [p_UnsnoozeAlerts]
end
go

create procedure [p_UnsnoozeAlerts]
	@SQLServerID int,
	@MetricXML nvarchar(max),
	@SnoozeStart datetime output,
	@SnoozeEnd datetime output,
	@SnoozeStartUser nvarchar(255) output,
	@SnoozeEndUser nvarchar(255) output
as
begin
	DECLARE @err int
	DECLARE @xmlDoc int
	DECLARE @Now datetime
	DECLARE @Start datetime
	DECLARE @StartUser nvarchar(255)
	DECLARE @EndUser nvarchar(255)
	
	DECLARE @Metrics table(Metric int) 

	if (@MetricXML is not null)
	begin
		exec sp_xml_preparedocument @xmlDoc output, @MetricXML

		insert into @Metrics	
		select MetricID
			from openxml(@xmlDoc, '//Metric', 1) with (MetricID int) 

		exec sp_xml_removedocument @xmlDoc
	end

	set @Now = GetUTCDate()

	if (@MetricXML is not null)
	begin
		select @Start = [UTCSnoozeStart], @StartUser = [SnoozeStartUser] 
		from MetricThresholds 
			where SQLServerID = @SQLServerID and Metric = (select min(Metric) from @Metrics)
	end

	if (@StartUser is null) 
	begin
		set @StartUser = SYSTEM_USER
	end

	set @EndUser = @SnoozeEndUser
	if (@EndUser is null)
		set @EndUser = SYSTEM_USER

	if (@Start is null)
		set @Start = @Now

	UPDATE MetricThresholds SET
			[UTCSnoozeEnd] = @Now,
			[SnoozeEndUser] = @EndUser
		WHERE [SQLServerID] = @SQLServerID 
			and (@MetricXML is null or [Metric] in (select [Metric] from @Metrics))

	SELECT @err = @@error
	
	SELECT @SnoozeEnd = @Now	
	SELECT @SnoozeEndUser = @EndUser	
	SELECT @SnoozeStart = @Start
	SELECT @SnoozeStartUser = @StartUser	

	RETURN @err
end
