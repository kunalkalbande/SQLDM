if (object_id('p_GetTopSessionsByCpuUsage') is not null)
begin
	drop procedure [p_GetTopSessionsByCpuUsage]
end
go

-- [p_GetTopSessionsByCpuUsage] 30
CREATE PROCEDURE [dbo].[p_GetTopSessionsByCpuUsage]
@TopX INT = NULL, 
@SQLServerID INT = NULL --SQLdm 9.1 (Sanjali Makkar) : Adding the filter of InstanceID
AS
BEGIN


	;with MaxCollTime (SQLServerID, UTCCollectionDateTime) as (
	  SELECT MS.SQLServerID, MAX(sa.UTCCollectionDateTime)
	  FROM [ServerActivity] sa (nolock)
		JOIN MonitoredSQLServers MS (nolock)
		  ON sa.SQLServerID = MS.SQLServerID
	  WHERE MS.Active = 1 AND (@SQLServerID IS NULL OR sa.SQLServerID = @SQLServerID) AND sa.SessionList IS NOT NULL
	  GROUP BY MS.SQLServerID
		)
	SELECT
		sa.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		, sa.UTCCollectionDateTime, sa.SessionList
	FROM
	 	MaxCollTime mct (nolock) INNER JOIN  [ServerActivity] sa (nolock)
		ON  mct.SQLServerID = sa.SQLServerID
			AND mct.UTCCollectionDateTime = sa.UTCCollectionDateTime
		JOIN MonitoredSQLServers MS ON MS.SQLServerID = mct.SQLServerID WHERE sa.SessionList IS NOT NULL
END
 
GO