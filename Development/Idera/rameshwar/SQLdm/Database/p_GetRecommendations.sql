if (object_id('p_GetRecommendations') is not null)
begin
drop procedure [p_GetRecommendations]
end
go

-- SQLdm 10.0 Srishti Purohit Doctor implementation in DM
create procedure [p_GetRecommendations] 

@sqlServerId int,
@analysisCompletionTime datetime
AS
BEGIN
	
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		-- check if records for recommendation

if @analysisCompletionTime IS NULL
BEGIN
SELECT NULL
END
		ELSE
		BEGIN

		
		SELECT PARP.AnalysisRecommendationID, PropertyName, Value FROM PrescriptiveAnalysisRecommendationProperty PARP
		JOIN PrescriptiveRecommendationProperty PRP ON PARP.PropertyID = PRP.ID
		JOIN PrescriptiveAnalysisRecommendation PAR ON PARP.AnalysisRecommendationID = PAR.ID
		JOIN PrescriptiveAnalysisDetails PAD ON PAD.PrescriptiveAnalysisDetailsID = PAR.PrescriptiveAnalysisDetailsID
		JOIN PrescriptiveAnalysis PA ON PA.AnalysisID = PAD.AnalysisID
		WHERE PA.SQLServerID = @sqlServerId AND PA.[UTCAnalysisCompleteTime] = @analysisCompletionTime

		SELECT PAR.[RecommendationID]
      --,PR.[AdditionalConsiderations]
   --   ,PR.[bitly]
	  --,PRC.Name
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
		WHERE PA.SQLServerID = @sqlServerId	AND PA.[UTCAnalysisCompleteTime] = @analysisCompletionTime
		--Check for only active master recommendations
		AND PR.IsActive = 1
	END

IF(@@ERROR <>0)
BEGIN
	 Print 'Error while getting recommendations.'
	  -- Any Error Occurred during Transaction. Rollback
	  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to save analysis in PrescriptiveAnalysis.', @ErrorSeverity, @ErrorState)

END
END
 
GO 
