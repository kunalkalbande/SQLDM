if (object_id('p_SavePrescriptiveAnalysisDetails') is not null)
begin
drop procedure [p_SavePrescriptiveAnalysisDetails]
end
go

create procedure [p_SavePrescriptiveAnalysisDetails] 
@analysisID INT,
@analyzerID INT,
@status INT ,
@recommendationCount INT,
@prescriptiveAnalysisDetailsID INT = 0 OUT
AS
begin
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint


-- Save analysis details
-- insert analysis record in table
			-- insert new record in [PrescriptiveAnalysisDetails] table
			if(@analysisID > 0)
		BEGIN
			INSERT INTO [dbo].[PrescriptiveAnalysisDetails]
           ([AnalysisID]
           ,[AnalyzerID]
           ,[Status]
           ,[RecommendationCount]
           ,[RecordCreatedTimestamp]
           ,[RecordUpdateDateTimestamp])
			
			(SELECT @analysisID,
			@analyzerID,
			@status[1],
			@recommendationCount
			,CURRENT_TIMESTAMP
			,CURRENT_TIMESTAMP)

		SET @prescriptiveAnalysisDetailsID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
		if(@prescriptiveAnalysisDetailsID <= 0)
		BEGIN
			Print 'Error while saving anaysis in [PrescriptiveAnalysis].'
		  -- Any Error Occurred during Transaction. Rollback
		  ROLLBACK  -- Roll back
  		  
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR('Unable to save analysis in PrescriptiveAnalysis.', @ErrorSeverity, @ErrorState)
			RETURN 0
		END
		ELSE 
		BEGIN
		-- return analysis id 
		RETURN @analysisID
		END
	END

end
 
GO 
