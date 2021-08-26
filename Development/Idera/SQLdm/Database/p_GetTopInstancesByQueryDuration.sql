IF (OBJECT_ID('p_GetTopInstancesByQueryDuration') IS NOT NULL)
BEGIN
  DROP PROC [p_GetTopInstancesByQueryDuration]
END
GO
-- [p_GetTopInstancesByQueryDuration] 30
CREATE PROCEDURE [dbo].[p_GetTopInstancesByQueryDuration]
@TopX INT = NULL,
@NumDays INT = 24
AS
BEGIN

	DECLARE @EndDate DATETIME 
	SELECT @EndDate=MAX(qm.UTCCollectionDateTime) FROM QueryMonitorStatistics qm (NOLOCK)
	DECLARE @StartDate DATETIME 
	SELECT @StartDate= DATEADD(DD, -1*@NumDays, @EndDate)
	
	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX
	ELSE
		SET ROWCOUNT 1000
	
	SELECT COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	, qm.UTCCollectionDateTime, MS.SQLServerID, d.DatabaseName, 
		qm.DurationMilliseconds,qm.CPUMilliseconds, qm.Reads,qm.Writes,s.SQLSignature
	FROM MonitoredSQLServers MS (nolock)
	JOIN [SQLServerDatabaseNames] d (nolock) ON d.SQLServerID = MS.SQLServerID
	JOIN [QueryMonitorStatistics] qm (nolock) ON qm.DatabaseID = d.DatabaseID
	JOIN AllSQLSignatures s (nolock) on qm.SQLSignatureID = s.SQLSignatureID
	WHERE MS.Active = 1	
	and qm.UTCCollectionDateTime  between @StartDate and @EndDate
	ORDER BY qm.DurationMilliseconds DESC
END
