IF (object_id('p_GetRecommendationList') is not null)
	BEGIN
		DROP PROCEDURE p_GetRecommendationList
	END
GO

-- SQLdm 10.5 Sushant Kulkarni Doctor implementation in DM
CREATE procedure p_GetRecommendationList
	@SQLServerID int,
	@AnalysisID int
AS
BEGIN	
	set transaction isolation level read uncommitted
	declare @err int
	
	--For excepttion handling
	DECLARE
	  @ErrorMessage   varchar(2000)
	 ,@ErrorSeverity  tinyint
	 ,@ErrorState     tinyint
	 ,@RecommendationCount INT

	
		SELECT PARP.AnalysisRecommendationID, PropertyName, Value FROM PrescriptiveAnalysisRecommendationProperty PARP
		JOIN PrescriptiveRecommendationProperty PRP ON PARP.PropertyID = PRP.ID
		JOIN PrescriptiveAnalysisRecommendation PAR ON PARP.AnalysisRecommendationID = PAR.ID
		JOIN PrescriptiveAnalysisDetails PAD ON PAD.PrescriptiveAnalysisDetailsID = PAR.PrescriptiveAnalysisDetailsID
		JOIN PrescriptiveAnalysis PA ON PA.AnalysisID = PAD.AnalysisID
		WHERE PA.SQLServerID = @SQLServerID 

		-- check if records for recommendation
	
		SELECT PAR.[RecommendationID]
	--   ,PR.[AdditionalConsiderations]
	--   ,PR.[bitly]
		--,PRC.Name AS Category
	--   ,PR.[ConfidenceFactor]
	--   ,PAR.[Description]
	--   ,PAR.[Finding]
	--   ,PR.[ImpactExplanation]
	--   ,PR.[ImpactFactor]
	--   ,PR.[InfoLinks]
	--   ,PR.[PluralFormFinding]
	--   ,PR.[PluralFormImpactExplanation]
	--   ,PR.[PluralFormRecommendation]
	--   ,PAR.[ProblemExplanation]
	--   ,PAR.[Recommendation]
	--   ,PAR.[Relevance]
	--   ,PR.[Tags]
		,PAR.ComputedRankFactor
		,PAD.RecordCreatedTimestamp
		,PAD.AnalyzerID
		,PAR.ID AS AnalysisRecommendationID
		,PAD.Status
		,PA.RecommendationCount
		,PAR.IsFlagged
		,PAR.OptimizationStatusID
		,PAR.OptimizationErrorMessage
		FROM PrescriptiveRecommendation PR
		JOIN PrescriptiveAnalysisRecommendation PAR ON PAR.RecommendationID = PR.RecommendationID
		JOIN PrescriptiveAnalysisDetails PAD ON PAD.PrescriptiveAnalysisDetailsID = PAR.PrescriptiveAnalysisDetailsID
		JOIN PrescriptiveAnalysis PA ON PA.AnalysisID = PAD.AnalysisID
		JOIN PrescriptiveRecommendationCategory PRC ON PRC.CategoryID = PR.[CategoryID]
		WHERE PA.SQLServerID = @SQLServerID
		AND PA.AnalysisID = @AnalysisID
		--Check for only active master recommendations
		AND PR.IsActive = 1
		ORDER BY PAR.ComputedRankFactor DESC

	IF(@@ERROR <>0)
		BEGIN
	
				Print 'Error'
	 			SET @ErrorMessage  = ERROR_MESSAGE()
					SET @ErrorSeverity = ERROR_SEVERITY()
					SET @ErrorState    = ERROR_STATE()
					RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)

		END

END
 
