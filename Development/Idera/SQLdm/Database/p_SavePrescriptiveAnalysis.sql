if (object_id('p_SavePrescriptiveAnalysis') is not null)
begin
drop procedure [p_SavePrescriptiveAnalysis]
end
go

create procedure [p_SavePrescriptiveAnalysis] 
@sqlServerID INT,
@analysisStartTime DATETIME,
@analysisCompleteTime DATETIME ,
@recommendationCount INT,
@analysisTypeId INT,
@analysisID INT = 0 OUT
AS
begin
--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint


-- Save analysis
-- insert analysis record in table
			-- insert new record in [PrescriptiveAnalysis] table
			INSERT INTO [dbo].[PrescriptiveAnalysis] WITH(TABLOCK)
				   ([SQLServerID]
           ,[UTCAnalysisStartTime]
           ,[UTCAnalysisCompleteTime]
           ,[RecommendationCount]
		   ,[AnalysisTypeID]
           ,[RecordCreatedTimestamp]
           ,[RecordUpdateDateTimestamp])
			 VALUES ( @sqlServerID
			 ,@analysisStartTime
			 ,@analysisCompleteTime
			 ,@recommendationCount	
			 ,@analysisTypeId
			  ,CURRENT_TIMESTAMP
			  ,CURRENT_TIMESTAMP)
	
			
			SET @analysisID = (CASE WHEN @@ROWCOUNT > 0 THEN SCOPE_IDENTITY() ELSE -1 END)
			PRINT @analysisID
		if(@analysisID <= 0)
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

end
 
GO 
