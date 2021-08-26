IF (object_id('p_GetTopInstancesByCPUUsage') is not null)
BEGIN
drop procedure p_GetTopInstancesByCPUUsage
END
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- CPU Load Usage
EXEC  [p_GetTopInstancesByCPUUsage] 100
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesByCPUUsage](
	@TopX int=NULL
)
AS
BEGIN
DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC, InstanceName nvarchar(256),FriendlyServerName  nvarchar(256))
DECLARE @MetricID INT;
SET @MetricID = 30; -- SQL Server CPU Usage (Percent)

INSERT INTO @Threshold EXEC p_PopulateMetricThresholdsNew @MetricID;

IF @TopX IS NOT null AND @TopX > 0
	SET ROWCOUNT @TopX;

;with MaxCollectionDate (SQLServerID, MaxCollectionTime, InstanceName) as 
(select MS.SQLServerID, MAX(UTCCollectionDateTime),  COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		from @Threshold MS
			Join [ServerStatistics] SS (nolock) 
                  on MS.SQLServerID = SS.SQLServerID 
            group by MS.SQLServerID, MS.InstanceName,MS.[FriendlyServerName]
)		

SELECT O.SQLServerID,O.InstanceName, O.CPUActivityPercentage, dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.CPUActivityPercentage,default,default,default,default,default) Severity 
FROM(
 SELECT mcd.SQLServerID,mcd.InstanceName,CPUActivityPercentage
 FROM MaxCollectionDate mcd
 INNER JOIN ServerStatistics ss WITH (NOLOCK) ON mcd.SQLServerID = ss.SQLServerID AND mcd.MaxCollectionTime = ss.UTCCollectionDateTime
 --GROUP BY mss.SQLServerID,mss.InstanceName, mcd.UTCCollectionDateTime,CPUActivityPercentage
 ) O
 INNER JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
 order by CPUActivityPercentage desc

End
