IF (object_id('p_GetTopInstancesByIO') is not null)
BEGIN
drop procedure p_GetTopInstancesByIO
END
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- IO
EXEC  [p_GetTopInstancesByIO] 100
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesByIO](
  @TopX int=NULL
)
AS
BEGIN

SELECT * INTO #DS FROM
(
	SELECT ds.DatabaseID,
		UTCCollectionDateTime,
		TimeDeltaInSeconds,
		NumberReads,
		NumberWrites,
		names.SQLServerID,
		COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]
	FROM DatabaseStatistics ds (nolock)  
	JOIN [SQLServerDatabaseNames] names (nolock) ON names.DatabaseID = ds.DatabaseID
    JOIN MonitoredSQLServers MS (nolock) ON MS.SQLServerID = names.SQLServerID
  WHERE MS.Active = 1
	
	UNION ALL
	
	SELECT ds.DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
		TotalNumberReads AS NumberReads,
		TotalNumberWrites AS NumberWrites,
		names.SQLServerID,
		COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]
	FROM DatabaseStatisticsAggregation ds (nolock)  
	JOIN [SQLServerDatabaseNames] names (nolock) ON names.DatabaseID = ds.DatabaseID
    JOIN MonitoredSQLServers MS (nolock) ON MS.SQLServerID = names.SQLServerID
  WHERE MS.Active = 1
) AS DSAggregation;

IF @TopX IS NOT null AND @TopX > 0
  SET ROWCOUNT @TopX;

with DBMaxCollTime (SQLServerID, MaxCollectionTime, InstanceName) as (
  select SQLServerID, MAX(DS.UTCCollectionDateTime), InstanceName
--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
  from #DS DS
  GROUP BY SQLServerID,InstanceName
)

SELECT mct.SQLServerID,
					mct.InstanceName,
					SUM((NumberReads+NumberWrites) / TimeDeltaInSeconds) as SQLPhysicalIO,
					3 as Severity
  FROM DBMaxCollTime mct 
			JOIN #DS ds (nolock) ON mct.MaxCollectionTime = ds.UTCCollectionDateTime
			INNER JOIN [SQLServerDatabaseNames] names (NOLOCK) ON  names.DatabaseID = ds.DatabaseID 
			group by mct.SQLServerID,mct.InstanceName

ORDER BY SQLPhysicalIO desc
End
