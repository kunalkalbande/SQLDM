if (object_id('p_UpdateRecommendationFlagStatus') is not null)
begin
drop procedure [p_UpdateRecommendationFlagStatus]
end
go

create procedure [p_UpdateRecommendationFlagStatus] 
@analysisRecommendationID INT,
@flag BIT,
@isFlagChanged BIT OUT
AS
begin

--For excepttion handling
DECLARE
  @ErrorMessage   varchar(2000)
 ,@ErrorSeverity  tinyint
 ,@ErrorState     tinyint

SET @isFlagChanged =0

UPDATE [dbo].[PrescriptiveAnalysisRecommendation]
   SET [IsFlagged] = @flag
 WHERE [ID] = @analysisRecommendationID

SET @isFlagChanged = 1

IF(@@ERROR <> 0)
BEGIN
SET @isFlagChanged = 0
 Print 'Transaction Failed .'
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
			
END

end
 
GO 
