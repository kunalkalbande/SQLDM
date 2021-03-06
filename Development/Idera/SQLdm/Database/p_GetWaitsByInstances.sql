IF (object_id('p_GetWaitsByInstance') is not null)
BEGIN
drop procedure p_GetWaitsByInstance
END
GO

/*-- 
SQLdm 8.5 (Gaurav Karwal): for Top X API- Get Waits by Instance
EXEC  p_GetWaitsByInstance 10
--*/

CREATE PROCEDURE [dbo].p_GetWaitsByInstance
	@TopX int=NULL
AS
BEGIN

	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;	
	
	WITH MaxCollTimes(SQLServerID, UTCCollectionDateTime) AS 
	(SELECT ws.SQLServerID, MAX(ws.UTCCollectionDateTime) FROM WaitStatistics ws (NOLOCK) GROUP BY ws.SQLServerID)	
	SELECT MS.SQLServerID,
		 COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		WD.WaitTypeID,
		WD.WaitTimeInMilliseconds,
		WT.WaitType,
		A.ApplicationName, 
		WS.UTCCollectionDateTime, 
		WS.TimeDeltaInSeconds
	 FROM WaitStatistics WS (NOLOCK)
	 JOIN WaitStatisticsDetails WD (NOLOCK) ON WD.[WaitStatisticsID] = WS.[WaitStatisticsID]
	 JOIN MonitoredSQLServers MS (NOLOCK) ON MS.SQLServerID = WS.SQLServerID
     JOIN ApplicationNames A (NOLOCK) ON WD.[WaitingTasks] = A.ApplicationNameID
     JOIN WaitTypes WT (NOLOCK) ON WD.WaitTypeID = WT.WaitTypeID 
     INNER JOIN MaxCollTimes mct (NOLOCK) ON mct.SQLServerID = WS.SQLServerID AND mct.UTCCollectionDateTime = WS.UTCCollectionDateTime
	 WHERE MS.Active = 1
	ORDER BY WD.WaitTimeInMilliseconds DESC
END
