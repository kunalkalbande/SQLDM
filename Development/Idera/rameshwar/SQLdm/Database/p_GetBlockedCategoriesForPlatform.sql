IF (OBJECT_ID('p_GetBlockedCategoriesForPlatform') IS NOT NULL)
BEGIN
DROP PROCEDURE [dbo].[p_GetBlockedCategoriesForPlatform]
END
GO
CREATE PROCEDURE [dbo].[p_GetBlockedCategoriesForPlatform]
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

	SELECT DISTINCT 
		prc.[CategoryID],
		prc.[Name]
	FROM 
		RecommedationClassification rc (NOLOCK)
		JOIN PrescriptiveRecommendation pr (NOLOCK)
			ON pr.RecommendationID = rc.RecommendationID
		JOIN PrescriptiveRecommendationCategory prc (NOLOCK)
			ON prc.CategoryID = pr.CategoryID
	WHERE 
		@sqlServerId IS NOT NULL AND 
		@cloudProviderId IS NOT NULL AND 
		prc.CategoryID NOT IN
			(
				SELECT DISTINCT prc.CategoryID
				FROM 
					RecommedationClassification rc
					JOIN PrescriptiveRecommendation pr
						ON pr.RecommendationID = rc.RecommendationID
					JOIN PrescriptiveRecommendationCategory prc
						ON prc.CategoryID = pr.CategoryID
				WHERE
					(rc.AWS = 1 AND @cloudProviderId = 1) OR
					(rc.Azure = 1 AND @cloudProviderId = 2) OR
					(rc.OnPremisesDb = 1 AND (@cloudProviderId = 3 OR @cloudProviderId = 4) OR
					@cloudProviderId > 4)
			)

	if(@@ERROR <> 0)
	BEGIN
		 Print 'Error occured while getting the blocked categories for the server.'
		  -- Any Error Occurred during Transaction. Rollback
		   RAISERROR ('Error occured while getting the blocked categories for the server.',
				 16,
				 1)
	END
END
GO


