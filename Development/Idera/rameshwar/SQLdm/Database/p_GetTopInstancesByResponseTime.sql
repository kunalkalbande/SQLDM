IF (OBJECT_ID('p_GetTopInstancesByResponseTime') IS NOT NULL)
BEGIN
  DROP PROCEDURE [p_GetTopInstancesByResponseTime]
END
GO
-- [p_GetTopInstancesByResponseTime] 30
CREATE PROCEDURE [dbo].[p_GetTopInstancesByResponseTime]
@TopX INT = NULL
AS
BEGIN
	DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC, InstanceName varchar(255),FriendlyServerName nvarchar(256))--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	DECLARE @MetricID INT;
	SET @MetricID = 22; --this is the metric id for Server Response Time
	
	INSERT INTO @Threshold EXEC p_PopulateMetricThresholdsNew @MetricID;
	
	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX

	SELECT O.InstanceName, O.UTCCollectionDateTime,O.SQLServerID,  O.ResponseTimeInMilliseconds,dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.ResponseTimeInMilliseconds,default,default,default,default,default) Severity 
	FROM (SELECT SS.UTCCollectionDateTime,MS.SQLServerID,
	COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	 ,ResponseTimeInMilliseconds
        FROM MonitoredSQLServers MS (nolock) 
		JOIN ServerStatistics SS (nolock) 
		ON MS.SQLServerID = SS.SQLServerID
        WHERE MS.Active = 1 and MS.LastScheduledCollectionTime = SS.UTCCollectionDateTime
	) O 
	JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
	ORDER BY O.ResponseTimeInMilliseconds DESC
END