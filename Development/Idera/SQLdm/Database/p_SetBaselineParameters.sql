if (object_id('p_SetBaselineParameters') is not null)
begin
drop procedure p_SetBaselineParameters
end
go
CREATE PROCEDURE [dbo].[p_SetBaselineParameters](
	@SQLServerID int,
	@UseDefaults bit,
	@StartDate datetime,
	@EndDate datetime,
	@Days tinyint
)
AS
BEGIN
	declare @err int

	update [MonitoredSQLServers] Set
		   [RefRangeUseDefaults] = @UseDefaults,
  		   [RefRangeStartTimeUTC] = @StartDate,
		   [RefRangeEndTimeUTC] = @EndDate,
		   [RefRangeDays] = @Days
	where [SQLServerID] = @SQLServerID

	select @err = @@error

	return @err
END
		

