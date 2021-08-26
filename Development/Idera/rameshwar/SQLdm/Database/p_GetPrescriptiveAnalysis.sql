if (object_id('p_GetPrescriptiveAnalysis') is not null)
begin
drop procedure p_GetPrescriptiveAnalysis
end
go
CREATE PROCEDURE [dbo].[p_GetPrescriptiveAnalysis]
	@SQLServerID int 
AS
BEGIN
	SELECT [AnalysisID]
      ,[SQLServerID]
      ,[UTCAnalysisStartTime]
      ,[UTCAnalysisCompleteTime]
      ,[RecommendationCount]
	FROM [dbo].[PrescriptiveAnalysis]
	where
		SQLServerID = @SQLServerID
END

GO
