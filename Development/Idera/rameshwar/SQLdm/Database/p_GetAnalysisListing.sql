if (object_id('p_GetAnalysisListing') is not null)
begin
drop procedure p_GetAnalysisListing
end
go
/****** Object:  StoredProcedure [dbo].[p_GetAnalysisListing]    Script Date: 01/16/2019 05:42:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[p_GetAnalysisListing]
@sqlServerId int
AS
BEGIN
	
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint
		-- check if records for recommendation

BEGIN
SELECT PA.SQLServerID, PA.UTCAnalysisCompleteTime, PA.AnalysisID, PA.UTCAnalysisStartTime AS Started, CONVERT(CHAR(8), CAST(CONVERT(VARCHAR(23),PA.UTCAnalysisCompleteTime,121) AS DATETIME) - CAST(CONVERT(VARCHAR(23),PA.UTCAnalysisStartTime,121)AS DATETIME),8) AS Duration, PA.RecommendationCount AS Recommendations, PAT.AnalysisType AS TaskType, 
ISNULL(MAX(PAR.ComputedRankFactor),0) Priority 
FROM PrescriptiveAnalysis PA
LEFT JOIN PrescriptiveAnalysisDetails PAD ON PAD.AnalysisID = PA.AnalysisID
LEFT JOIN PrescriptiveAnalysisType PAT ON PAT.AnalysisTypeID = PA.AnalysisTypeID
LEFT JOIN PrescriptiveAnalysisRecommendation PAR ON PAR.PrescriptiveAnalysisDetailsID = PAD.PrescriptiveAnalysisDetailsID
WHERE PA.SQLServerID = @sqlServerId
GROUP BY PA.SQLServerID, PA.UTCAnalysisCompleteTime, PA.AnalysisID, PA.UTCAnalysisStartTime, CONVERT(CHAR(8), CAST(CONVERT(VARCHAR(23),PA.UTCAnalysisCompleteTime,121) AS DATETIME) - CAST(CONVERT(VARCHAR(23),PA.UTCAnalysisStartTime,121)AS DATETIME),8), PA.RecommendationCount, PAT.AnalysisType
ORDER BY PA.AnalysisID DESC
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


