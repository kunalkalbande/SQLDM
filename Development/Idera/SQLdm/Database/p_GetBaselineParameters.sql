if (object_id('p_GetBaselineParameters') is not null)
begin
drop procedure p_GetBaselineParameters
end
go
CREATE PROCEDURE [dbo].[p_GetBaselineParameters](
	@SQLServerID int,
	@UseDefaults bit output,
	@StartDate datetime output,
	@EndDate datetime output,
	@Days tinyint output,
	@EarliestStatisticsAvailable datetime output
)
AS
BEGIN
	declare @RefRangeUseDefaults bit,
			@RefRangeStartTimeUTC datetime,
			@RefRangeEndTimeUTC datetime,
			@RefRangeDays tinyint,
			@EarliestData datetime,
			@err int

	select  @RefRangeUseDefaults = [RefRangeUseDefaults],
  			@RefRangeStartTimeUTC = [RefRangeStartTimeUTC],
			@RefRangeEndTimeUTC = [RefRangeEndTimeUTC],
			@RefRangeDays = [RefRangeDays]
	from [MonitoredSQLServers] 
	where [SQLServerID] = @SQLServerID

	select @err = @@error
	if (@err = 0)
	begin
		select top 1 @EarliestData = [UTCCollectionDateTime] 
		from ServerStatistics
		where [SQLServerID] = @SQLServerID
		order by [UTCCollectionDateTime] ASC
	
		select @err = @@error
	end

	if (@err = 0)
	begin
		select @UseDefaults = case when @RefRangeUseDefaults is null then 1 else @RefRangeUseDefaults end
		select @StartDate = @RefRangeStartTimeUTC
		select @EndDate = @RefRangeEndTimeUTC
		select @Days = @RefRangeDays
		select @EarliestStatisticsAvailable = @EarliestData
	end

	return @err
END
		

