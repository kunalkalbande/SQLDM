--SQLdm 9.1 (Sanjali Makkar)
--To get Session List

IF (object_id('p_GetSessionList') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_GetSessionList]
END
GO

-- EXEC [p_GetSessionList] 1

CREATE PROCEDURE [dbo].[p_GetSessionList]
@SQLServerID INT = NULL 
AS
BEGIN

	;WITH MaxCollTime (SQLServerID,UTCCollectionDateTime) AS (
	  SELECT MS.SQLServerID, MAX(SA.UTCCollectionDateTime)
	  FROM [ServerActivity] SA (nolock)
		JOIN MonitoredSQLServers MS (nolock)
		  ON SA.SQLServerID = MS.SQLServerID
	  WHERE MS.Active = 1 AND (@SQLServerID IS NULL OR SA.SQLServerID = @SQLServerID)
	  GROUP BY MS.SQLServerID
		)
	SELECT
		SA.SQLServerID, 
		 COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		SA.UTCCollectionDateTime, SA.SessionList
	FROM
	 	MaxCollTime MCT (nolock) INNER JOIN  [ServerActivity] SA (nolock) 
		ON  MCT.SQLServerID = SA.SQLServerID AND MCT.UTCCollectionDateTime = SA.UTCCollectionDateTime
		JOIN MonitoredSQLServers MS ON MS.SQLServerID = MCT.SQLServerID 
END
 
GO


