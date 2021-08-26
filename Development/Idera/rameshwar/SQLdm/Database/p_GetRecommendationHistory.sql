if (object_id('p_GetRecommendationHistory') is not null)
begin
drop procedure [p_GetRecommendationHistory]
end
go

-- SQLdm 10.0 Srishti Purohit Doctor implementation in DM
create procedure [p_GetRecommendationHistory] 

    @SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null
	--@HistoryInMinutes int = null
AS
BEGIN
	
set transaction isolation level read uncommitted
declare @err int
--declare @BeginDateTime datetime
declare @AnalysisCompletionDateTime datetime


--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint

if (@UTCSnapshotCollectionDateTime is null)
	select @AnalysisCompletionDateTime = (select max([UTCAnalysisCompleteTime]) from PrescriptiveAnalysis where [SQLServerID] = @SQLServerID)
else
	select @AnalysisCompletionDateTime = @UTCSnapshotCollectionDateTime
	PRINT @AnalysisCompletionDateTime
--if (@HistoryInMinutes is null)
--	select @BeginDateTime = @AnalysisCompletionDateTime
--else
--	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @AnalysisCompletionDateTime)

		-- get properties of recomms
		
		
		SELECT PARP.AnalysisRecommendationID, PropertyName, Value FROM PrescriptiveAnalysisRecommendationProperty PARP
		JOIN PrescriptiveRecommendationProperty PRP ON PARP.PropertyID = PRP.ID
		JOIN PrescriptiveAnalysisRecommendation PAR ON PARP.AnalysisRecommendationID = PAR.ID
		JOIN PrescriptiveAnalysisDetails PAD ON PAD.PrescriptiveAnalysisDetailsID = PAR.PrescriptiveAnalysisDetailsID
		JOIN PrescriptiveAnalysis PA ON PA.AnalysisID = PAD.AnalysisID
		WHERE PA.SQLServerID = @SQLServerID AND PA.[UTCAnalysisCompleteTime] --between @BeginDateTime and @AnalysisCompletionDateTime
		=  @AnalysisCompletionDateTime

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
		WHERE PA.SQLServerID = @SQLServerID	AND PA.[UTCAnalysisCompleteTime] --between @BeginDateTime and @AnalysisCompletionDateTime
		= @AnalysisCompletionDateTime
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
 
GO 
