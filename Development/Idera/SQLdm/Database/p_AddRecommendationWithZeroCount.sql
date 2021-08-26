if (object_id('p_AddRecommendationWithZeroCount') is not null)
BEGIN
	DROP PROCEDURE p_AddRecommendationWithZeroCount
END
GO
CREATE PROCEDURE [dbo].[p_AddRecommendationWithZeroCount]
@SQLServerID INT
AS
	BEGIN
		-- Insert record in [PrescriptiveAnalysis]
		INSERT INTO [dbo].[PrescriptiveAnalysis]
		(
			 [SQLServerID]
			,[UTCAnalysisStartTime]
			,[UTCAnalysisCompleteTime]
			,[RecommendationCount]
			,[AnalysisTypeID]
			,[RecordCreatedTimestamp]
			,[RecordUpdateDateTimestamp]
		)
		VALUES
		(
			 @SQLServerID
			,GETDATE()
			,GETDATE()
			,0
			,1
			,GETDATE()
			,GETDATE()
		)

		-- Insert record in [PrescriptiveAnalysisDetails]
		INSERT INTO [dbo].[PrescriptiveAnalysisDetails]
		(
			 [AnalysisID]
			,[AnalyzerID]
			,[Status]
			,[RecommendationCount]
			,[RecordCreatedTimestamp]
			,[RecordUpdateDateTimestamp]
		)
		VALUES
		(
			(SELECT MAX([AnalysisID]) FROM [PrescriptiveAnalysis])
			,1
			,0
			,0
			,GETDATE()
			,GETDATE()
		)
	END



