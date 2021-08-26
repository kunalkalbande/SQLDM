if (object_id('p_GetMonitoredSQLServerWorkload') is not null)
begin
drop procedure p_GetMonitoredSQLServerWorkload
end
go
CREATE PROCEDURE [dbo].[p_GetMonitoredSQLServerWorkload]
(
	@SQLServerID int
)
AS
BEGIN
	DECLARE @e INT

	exec @e = p_GetMetricThresholds NULL, @SQLServerID
	if (@e = 0)
	begin
		exec @e = p_GetMonitoredSQLServerCounters @SQLServerID
	end

	--SQLdm 8.6 -- (Ankit Srivastava) -- Fro Supressing SQL Express Alert which are not required
	SELECT [Metric],[IsValidForSqlExpress]
			FROM [MetricMetaData]	

	if (@e = 0)
	begin
		exec @e = p_GetBaselineCheckThresholdDeviation @SQLServerID
	end

	--START: SQLdm 10.0 (Tarun Sapra)- Get the cloud provider id for the monitored sql server
	SELECT CloudProviderId AS CloudProviderId FROM MonitoredSQLServers WHERE SQLServerID = @SQLServerID 
	--END: SQLdm 10.0 (Tarun Sapra)- Get the cloud provider id for the monitored sql server

	RETURN @e
END