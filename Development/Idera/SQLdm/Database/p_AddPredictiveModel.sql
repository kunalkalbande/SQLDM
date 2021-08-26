
IF (OBJECT_ID('p_AddPredictiveModel') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_AddPredictiveModel]
END
GO

CREATE PROCEDURE [dbo].[p_AddPredictiveModel] 
	@SQLServerID int,
	@Metric      int,
	@Severity    int,
	@Timeframe   int,
	@Model       image
AS
BEGIN	
	
	IF NOT EXISTS(SELECT SQLServerID FROM PredictiveModels WHERE SQLServerID = @SQLServerID AND Metric = @Metric AND Severity = @Severity AND Timeframe = @Timeframe)
	BEGIN
	
		INSERT INTO PredictiveModels VALUES (@SQLServerID, @Metric, @Severity, @Timeframe, @Model)
	
	END
	ELSE
	BEGIN
		
		UPDATE PredictiveModels SET Model = @Model WHERE SQLServerID = @SQLServerID AND Metric = @Metric AND Severity = @Severity AND Timeframe = @Timeframe
		
	END	

	DECLARE @err INT
	SELECT  @err = @@error
	RETURN  @err
	
END