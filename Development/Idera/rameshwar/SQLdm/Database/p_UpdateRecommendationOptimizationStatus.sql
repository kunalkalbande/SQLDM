if (object_id('p_UpdateRecommendationOptimizationStatus') is not null)
begin
drop procedure [p_UpdateRecommendationOptimizationStatus]
end
go

create procedure [p_UpdateRecommendationOptimizationStatus] 
@analysisRecommendationID INT,
@status int,
@errorMessageText nvarchar(max),
@isStatusChanged BIT OUT
AS
begin

--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint


SET @isStatusChanged =0

UPDATE [dbo].[PrescriptiveAnalysisRecommendation]
   SET [OptimizationStatusID] = @status,
	[OptimizationErrorMessage] = @errorMessageText
 WHERE [ID] = @analysisRecommendationID

SET @isStatusChanged = 1
IF(@@ERROR <> 0)
BEGIN
SET @isStatusChanged = 0
 Print 'Transaction Failed .'
  SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
			
END

end
 
GO 
