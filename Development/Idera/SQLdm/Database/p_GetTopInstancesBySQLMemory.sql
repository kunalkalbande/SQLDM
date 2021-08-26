IF (object_id('p_GetTopInstancesBySQLMemory') is not null)
BEGIN
drop procedure p_GetTopInstancesBySQLMemory
END
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- Instance by SQL Memory Usage
EXEC  [p_GetTopInstancesBySQLMemory] 100
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesBySQLMemory](
	@TopX int=NULL
)
AS
BEGIN

DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC,InstanceName varchar(255),FriendlyServerName nvarchar(256))--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
DECLARE @MetricID INT;
SET @MetricID = 13; -- SQL Server Memory Usage (Percent)
INSERT INTO @Threshold EXEC p_PopulateMetricThresholdsNew @MetricID;

IF @TopX IS NOT null AND @TopX > 0
	SET ROWCOUNT @TopX;

	
with MaxCollectionDate (SQLServerID, MaxCollectionTime, InstanceName) as (
		select MS.SQLServerID, MAX(UTCCollectionDateTime), COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		from @Threshold MS
			Join ServerStatistics (nolock) ST ON MS.SQLServerID = ST.SQLServerID
	    group by MS.SQLServerID, MS.InstanceName,MS.[FriendlyServerName]
	)	
	
SELECT O.SQLServerID,O.InstanceName, O.SqlMemoryUsedInMB, 
	O.SqlMemoryAllocatedInMB, dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,
		O.SqlMemoryUsedInMB,default,default,default,default,default) Severity 
FROM(
SELECT mcd.SQLServerID,
	mcd.InstanceName,
	max((ss.SqlMemoryUsedInKilobytes/1024)) as SqlMemoryUsedInMB,
	max((SqlMemoryAllocatedInKilobytes/1024)) as SqlMemoryAllocatedInMB, 
	3 as Severity
FROM MaxCollectionDate mcd
	INNER JOIN ServerStatistics ss (NOLOCK) ON mcd.SQLServerID = ss.SQLServerID AND mcd.MaxCollectionTime = ss.UTCCollectionDateTime
GROUP BY mcd.SQLServerID,mcd.InstanceName, mcd.MaxCollectionTime, SqlMemoryUsedInKilobytes
 ) O 
 INNER JOIN @Threshold T ON T.SQLServerID = O.SQLServerID 
 order by SqlMemoryUsedInMB desc

End