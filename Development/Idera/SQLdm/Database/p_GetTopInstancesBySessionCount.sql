IF (OBJECT_ID('p_GetTopInstancesBySessionCount') IS NOT NULL)
BEGIN
  DROP PROC [p_GetTopInstancesBySessionCount]
END
GO
-- [p_GetTopInstancesBySessionCount] 30
CREATE PROCEDURE [dbo].[p_GetTopInstancesBySessionCount]
AS
BEGIN

	with MaxCollTime (SQLServerID, InstanceName, UTCCollectionDateTime) as (
	  select MS.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	  MAX(SA.UTCCollectionDateTime)
	  from [ServerActivity] SA (nolock)
		JOIN MonitoredSQLServers MS (nolock)
		  ON SA.SQLServerID = MS.SQLServerID
	  WHERE MS.Active = 1
	  GROUP BY MS.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName])
	)
	SELECT mct.SQLServerID, mct.InstanceName, mct.UTCCollectionDateTime,
		SA.SessionList
	FROM MaxCollTime mct (nolock)
	JOIN [ServerActivity] SA (nolock) ON SA.SQLServerID = mct.SQLServerID AND SA.UTCCollectionDateTime = mct.UTCCollectionDateTime

END