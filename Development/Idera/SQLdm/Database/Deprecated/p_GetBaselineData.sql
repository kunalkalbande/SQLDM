if (object_id('[p_GetBaselineData]') is not null)
begin
drop procedure p_GetBaselineData
end
go

create procedure [dbo].p_GetBaselineData
	@SQLServerID int,
	@AlertableOnly bit,
	@StartTime datetime output,
	@EndTime datetime output
as
begin
	declare @err int
	declare @Start datetime
	declare @End datetime	
	declare @All bit

	if (@AlertableOnly is null or @AlertableOnly = 0)
		select @All = 1
	else 
		select @All = 0

	select @Start = RefRangeStartTimeUTC,
		   @End = RefRangeEndTimeUTC 
	from MonitoredSQLServers 
	where SQLServerID = @SQLServerID

	set @err = @@ERROR
	
	if (@Start is not null and @End is not null)
	begin
		select D.[SQLServerID],D.[ItemID],D.[UTCCollectionDateTime],D.[RowCount],D.[Average],D.[Deviation] 
			from BaselineData D
			inner join BaselineMetaData MD on D.[ItemID] = MD.[ItemID]
		where SQLServerID = @SQLServerID
		and (@All = 1 or MD.[MetricID] is not null) 
		
		set @err = @@ERROR
	end

	select @StartTime = @Start
	select @EndTime = @End	

	return @err
end