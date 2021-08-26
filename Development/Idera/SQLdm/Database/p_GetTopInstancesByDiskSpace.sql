IF (object_id('p_GetTopInstancesByDiskSpace') is not null)
BEGIN
drop procedure p_GetTopInstancesByDiskSpace
END
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space
EXEC  [p_GetTopInstancesByDiskSpace] 30
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesByDiskSpace]
	@TopX int=NULL
AS
BEGIN
	DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC)
	DECLARE @MetricID INT;
	SET @MetricID = 59; --this is the metric id for OS Disk Free Space
	
	INSERT INTO @Threshold EXEC p_PopulateMetricThresholds @MetricID;
	
	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;

	with MaxCollTime (SQLServerID, InstanceName, UTCCollectionDateTime) as (
	  select MS.SQLServerID, 
	   COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	    MAX(UTCCollectionDateTime)
	  from DiskDrives DS (nolock)
		JOIN MonitoredSQLServers MS (nolock)
		  ON DS.SQLServerID = MS.SQLServerID
	  WHERE MS.Active = 1
	  GROUP BY MS.SQLServerID, MS.InstanceName,MS.[FriendlyServerName]
	)
	SELECT O.InstanceName, O.UTCCollectionDateTime,O.SQLServerID,  O.DiskUtilizationPercentage,	
	dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.DiskUtilizationPercentage,default,default,default,default,default) Severity 
	FROM (	
	SELECT mct.UTCCollectionDateTime, mct.SQLServerID, mct.InstanceName,
		round(avg(((TotalSizeKB - UnusedSizeKB)/TotalSizeKB)*100),2) as DiskUtilizationPercentage
	FROM MaxCollTime mct (nolock)
	JOIN  DiskDrives DD WITH (NOLOCK) ON DD.SQLServerID = mct.SQLServerID AND DD.UTCCollectionDateTime = mct.UTCCollectionDateTime
	GROUP BY mct.UTCCollectionDateTime, mct.SQLServerID, mct.InstanceName
	) O 
	JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
	ORDER BY O.DiskUtilizationPercentage DESC
	--SQLdm 9.0 (Vineet Kumar) (Web Console Improvements) -- Fixing DE42810. Changed order by from UTCCollectionDateTime to DiskUtilizationPercentage
END



