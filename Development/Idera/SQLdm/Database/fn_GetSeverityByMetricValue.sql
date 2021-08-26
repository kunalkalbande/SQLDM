IF (OBJECT_ID('fn_GetSeverityByMetricValue') IS NOT NULL)
BEGIN
DROP FUNCTION [fn_GetSeverityByMetricValue]
END
GO
CREATE FUNCTION [dbo].[fn_GetSeverityByMetricValue]
(
	  @CriticalThreshold NUMERIC
	 ,@WarningThreshold NUMERIC
	 ,@InfoThreshold NUMERIC
	 ,@MetricValue NUMERIC
   	 ,@WarningState INT = 4
	 ,@CriticalState INT = 8
	 ,@InfoState INT = 2
	 ,@NormalState INT = 1
	 ,@NoneState INT = 0
)
RETURNS INT
AS
BEGIN
	DECLARE @retVal INT,@Direction INT;
	
	SELECT @Direction = CASE WHEN @CriticalThreshold = @WarningThreshold AND @InfoThreshold = @WarningThreshold THEN
							0
						ELSE
							CASE WHEN @CriticalThreshold < @WarningThreshold THEN
								-1
							ELSE
								CASE WHEN @CriticalThreshold = @WarningThreshold AND @WarningThreshold < @InfoThreshold THEN
									-1
								ELSE
									1
								END
							END
						
						END
	
	IF @Direction = 1 OR @Direction = 0 BEGIN
		SELECT @retVal = 
			CASE WHEN ISNUMERIC(@MetricValue) = 1 THEN
			  CASE WHEN (@MetricValue >= @InfoThreshold AND @MetricValue < @WarningThreshold) THEN @InfoState ELSE
				CASE WHEN (@MetricValue >= @WarningThreshold AND @MetricValue < @CriticalThreshold) THEN @WarningState ELSE
					CASE WHEN (@MetricValue >= @CriticalThreshold) THEN @CriticalState ELSE @NormalState END
				END
			  END
			ELSE
				@NoneState
			END;
	END
	ELSE BEGIN
		SELECT @retVal = 
			CASE WHEN ISNUMERIC(@MetricValue) = 1 THEN
			  CASE WHEN (@MetricValue <= @InfoThreshold AND @MetricValue > @WarningThreshold) THEN @InfoState ELSE
				CASE WHEN (@MetricValue <= @WarningThreshold AND @MetricValue > @CriticalThreshold) THEN @WarningState ELSE
					CASE WHEN (@MetricValue <= @CriticalThreshold) THEN @CriticalState ELSE @NormalState END
				END
			  END
			ELSE
				@NoneState
			END;
	END
	RETURN @retVal;
END
GO
--SELECT dbo.[fn_GetSeverityByMetricValue](85,90,95,100,default,default,default,default,default)
