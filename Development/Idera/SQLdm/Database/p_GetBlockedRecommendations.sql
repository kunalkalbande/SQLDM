IF (OBJECT_ID('p_GetBlockedRecommendations') is not null)
BEGIN
	DROP PROCEDURE [p_GetBlockedRecommendations]
END

GO

CREATE PROCEDURE p_GetBlockedRecommendations
@InstanceID INT
AS
	BEGIN
		select AR.AnalysisConfigurationID,AR.RecommendationID 
		FROM MonitoredSQLServers MS 
		INNER JOIN AnalysisConfiguration AC ON AC.MonitoredServerID = MS.SQLServerID
		INNER JOIN AnalysisConfigBlockedRecommendation AR ON AR.AnalysisConfigurationID = AC.ID
		WHERE	MS.Active = 1 
			AND AC.IsActive = 1 
			AND MS.SQLServerID = @InstanceID
	END