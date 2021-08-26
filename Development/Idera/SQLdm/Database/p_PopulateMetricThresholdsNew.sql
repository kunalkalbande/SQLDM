--p_PopulateMetricThresholdsNew 92
IF (OBJECT_ID('p_PopulateMetricThresholdsNew') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_PopulateMetricThresholdsNew]
END
GO
CREATE PROCEDURE [dbo].[p_PopulateMetricThresholdsNew]
	  @MetricID BIGINT
AS
BEGIN
	 SELECT me.SQLServerID, me.Metric, CONVERT(XML,WarningThreshold).value('(/Threshold/Value)[1]','numeric') WarningThreshold,CONVERT(XML,CriticalThreshold).value('(/Threshold/Value)[1]','numeric') CriticalThreshold,CONVERT(XML,InfoThreshold).value('(/Threshold/Value)[1]','numeric') InfoThreshold ,m.InstanceName, m.FriendlyServerName--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
   FROM MonitoredSQLServers m (NOLOCK)
		JOIN MetricThresholds me (NOLOCK) ON m.SQLServerID = me.SQLServerID 
   WHERE m.Active =1 and  me.Metric = @MetricID;	
END
