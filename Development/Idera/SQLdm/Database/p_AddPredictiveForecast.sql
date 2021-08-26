
IF (OBJECT_ID('p_AddPredictiveForecast') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_AddPredictiveForecast]
END
GO

CREATE PROCEDURE [dbo].[p_AddPredictiveForecast] 
	@SQLServerID int,
	@Metric      int,
	@Severity    int,
	@Timeframe   int,
	@Forecast    int,
	@Accuracy	 decimal(5,2),
	@Expiration  datetime
AS
BEGIN	
	
	IF NOT EXISTS(SELECT SQLServerID FROM PredictiveForecasts WHERE SQLServerID = @SQLServerID AND Metric = @Metric AND Severity = @Severity AND Timeframe = @Timeframe)
	BEGIN
	
		INSERT INTO PredictiveForecasts VALUES (@SQLServerID, @Metric, @Severity, @Timeframe, @Forecast, @Accuracy, @Expiration)
	
	END
	ELSE
	BEGIN
		
		UPDATE PredictiveForecasts SET Forecast = @Forecast, Accuracy = @Accuracy, Expiration = @Expiration WHERE SQLServerID = @SQLServerID AND Metric = @Metric AND Severity = @Severity AND Timeframe = @Timeframe
		
	END	

	DECLARE @err INT
	SELECT  @err = @@error
	RETURN  @err
	
END