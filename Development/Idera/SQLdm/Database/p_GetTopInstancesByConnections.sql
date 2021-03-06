IF (OBJECT_ID('p_GetTopInstancesByConnections') IS NOT NULL)
BEGIN
  DROP PROCEDURE [p_GetTopInstancesByConnections]
END
GO
-- [p_GetTopInstancesByConnections] 30
CREATE PROCEDURE [dbo].[p_GetTopInstancesByConnections]
@TopX INT = NULL
AS
BEGIN
DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC,InstanceName nvarchar(256),FriendlyServerName nvarchar(256))		
	DECLARE @MetricID INT;
	SET @MetricID = 57; --this is the metric id for database connections
	
	INSERT INTO @Threshold EXEC p_PopulateMetricThresholdsNew @MetricID;
	
	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;
	
	
	with MaxCollTime (SQLServerID, MaxCollectionTime, InstanceName) as 
		(select MS.SQLServerID, MAX(UTCCollectionDateTime),COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
			from @Threshold MS
				Join [ServerStatistics] SS (nolock) 
                    on MS.SQLServerID = SS.SQLServerID 
                    group by MS.SQLServerID, MS.InstanceName,MS.[FriendlyServerName]
        )
				
	SELECT O.SQLServerID,O.InstanceName, O.MaxCollectionTime as UTCCollectionDateTime,O.Logins,dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.Logins,default,default,default,default,default) Severity 
	FROM (
	SELECT mct.MaxCollectionTime,
			mct.SQLServerID, 
			mct.InstanceName, 
			SS.Logins
	FROM MaxCollTime mct 
		JOIN ServerStatistics SS (nolock) ON mct.SQLServerID = SS.SQLServerID
		    AND mct.MaxCollectionTime = SS.UTCCollectionDateTime		
	) O 
	JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
	ORDER BY O.Logins DESC
END


