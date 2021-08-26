if (object_id('p_GetInstancesMonitoringCustomCounter') is not null)
begin
drop procedure p_GetInstancesMonitoringCustomCounter
end
go

create procedure [dbo].p_GetInstancesMonitoringCustomCounter(
	@Metric int
)
AS
begin
	declare @e int

	select [SQLServerID] from [CustomCounterMap] where [Metric] = @Metric

	set @e = @@ERROR

	return @e
end	

