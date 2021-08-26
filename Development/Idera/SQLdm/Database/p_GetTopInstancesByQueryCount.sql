IF (OBJECT_ID('p_GetTopInstancesByQueryCount') IS NOT NULL)
BEGIN
  DROP PROC [p_GetTopInstancesByQueryCount]
END
GO
-- [p_GetTopInstancesByQueryCount] 30
CREATE PROCEDURE [dbo].[p_GetTopInstancesByQueryCount]
@TopX INT = NULL
AS
BEGIN

DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC)
	DECLARE @MetricID INT;
	SET @MetricID = 51; --this is the metric id for query count
	
	DECLARE @MaxCollectionDate TABLE(sqlserverId int, UTCCollectionDateTime Datetime)
	insert into @MaxCollectionDate
	select q.SQLServerID, max(q.UTCCollectionDateTime)
	FROM QueryMonitorStatistics q (nolock)
	group by q.SQLServerID	

	INSERT INTO @Threshold EXEC p_PopulateMetricThresholds @MetricID;	

	IF @TopX IS NOT null AND @TopX > 0
	SET ROWCOUNT @TopX;
  

	SELECT O.InstanceName, O.UTCCollectionDateTime,O.SQLServerID,  O.SqlQueryCount,
	dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.SqlQueryCount,default,default,default,default,default) Severity 
	FROM (SELECT qm.UTCCollectionDateTime,MS.SQLServerID,
	COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	,count(qm.SQLStatementID) as SqlQueryCount
	FROM MonitoredSQLServers MS (nolock)
	INNER JOIN [QueryMonitorStatistics] qm (nolock) ON qm.SQLServerID = MS.SQLServerID
	INNER JOIN @MaxCollectionDate m on m.sqlserverId = MS.SQLServerID
			AND qm.UTCCollectionDateTime = m.UTCCollectionDateTime
	WHERE MS.Active = 1	
	GROUP BY qm.UTCCollectionDateTime,MS.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) --SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	) O 	
	JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
	ORDER BY O.SqlQueryCount DESC
END