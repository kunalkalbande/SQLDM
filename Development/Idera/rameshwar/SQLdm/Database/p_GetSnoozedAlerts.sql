if (object_id('p_GetSnoozedAlerts') is not null)
begin
drop procedure [p_GetSnoozedAlerts]
end
go

create procedure [p_GetSnoozedAlerts]
	@SQLServerID int
as
begin
	DECLARE @err int
	DECLARE @Now datetime

	set @Now = GetUTCDate()

	select MT.[Metric],MI.[Name] 
		from MetricThresholds MT (nolock) 
		inner join MetricInfo MI (nolock) on MI.[Metric] = MT.[Metric]
	where
	(@SQLServerID = MT.SQLServerID) and
	(UTCSnoozeEnd > @Now)

	select @err = @@error

	return @err
end