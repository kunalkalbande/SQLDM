
IF (object_id('p_GetTopXServerByTempDbUtilization') is not null)
BEGIN
drop procedure p_GetTopXServerByTempDbUtilization
END
GO

/*-- 
SQLdm 8.5 (Ashutosh Upadhyay): for Top X API- Tempdb Utilization
EXEC  [p_GetTopXServerByTempDbUtilization] '2013-05-14 10:24:29.587','2014-05-14 10:24:29.587',1
--*/

Create PROCEDURE [dbo].[p_GetTopXServerByTempDbUtilization](
	@TopX int=NULL
)
AS
BEGIN

	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;
	
	WITH MaxCollectionDate (SQLServerID, UTCCollectionDateTime)
	AS (
		SELECT ss.SQLServerID, MAX(ss.UTCCollectionDateTime)
		FROM ServerStatistics ss (nolock)
		GROUP BY ss.SQLServerID
	)
	SELECT SS.UTCCollectionDateTime,MS.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	,SS.TempDBSizeInKilobytes
	FROM MonitoredSQLServers MS  (nolock)
	INNER JOIN ServerStatistics SS (nolock) ON MS.SQLServerID = SS.SQLServerID
	INNER JOIN MaxCollectionDate mcd (nolock) ON SS.SQLServerID = mcd.SQLServerID AND SS.UTCCollectionDateTime = mcd.UTCCollectionDateTime
	WHERE MS.Active = 1
	ORDER BY SS.TempDBSizeInKilobytes DESC
END

GO


