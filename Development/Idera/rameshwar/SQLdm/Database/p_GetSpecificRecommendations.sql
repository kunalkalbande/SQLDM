IF (OBJECT_ID('p_GetSpecificRecommendations') IS NOT NULL)
BEGIN
DROP PROCEDURE [dbo].[p_GetSpecificRecommendations]
END
GO
CREATE PROCEDURE [dbo].[p_GetSpecificRecommendations]
	@sqlServerId INT = NULL
AS
BEGIN
	-- Get cloud provider id for the SQL Server Id
	DECLARE @cloudProviderId INT;
	SELECT 
		@cloudProviderId = CloudProviderId 
	FROM
		MonitoredSQLServers mss (NOLOCK)
	WHERE 
		mss.SQLServerID = @sqlServerId

	SELECT
		RC.RecommendationID
	FROM 
		RecommedationClassification RC (NOLOCK)
	WHERE
		@sqlServerId IS NULL OR
		@cloudProviderId IS NULL OR
		(RC.AWS = 1 AND @cloudProviderId = 1) OR
		(RC.Azure = 1 AND @cloudProviderId = 2) OR
		(RC.OnPremisesDb = 1 AND (@cloudProviderId = 3 OR @cloudProviderId = 4) OR
		@cloudProviderId > 4)

	if(@@ERROR <> 0)
	BEGIN
		 Print 'Error occured while getting the recommendations for the server.'
		  -- Any Error Occurred during Transaction. Rollback
		   RAISERROR ('Error occured while getting the recommendations for the server.',
				 16,
				 1)
	END
END
GO


