IF (object_id('p_GetBaselinesForMetric') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetBaselinesForMetric
END
GO

CREATE PROCEDURE [dbo].p_GetBaselinesForMetric
	@SQLServerId INT,
	@MetricId INT,
	@EndDate DATETIME,
	@HistoryInSeconds INT
AS
BEGIN
	
	DECLARE @startTime DATETIME 
	SET @startTime = DATEADD(SECOND,@HistoryInSeconds*-1,@EndDate)
	DECLARE @endTime DATETIME 
	SET @startTime = @EndDate

	SELECT final.value,final.utcdate FROM (SELECT temp1.value,temp1.utcdate FROM (SELECT TOP 1 (Mean+StdDeviation) as value,@startTime as utcdate FROM BaselineStatistics WHERE UTCCalculation<=@startTime AND SQLServerID=@SQLServerId AND MetricID=@MetricId ORDER BY UTCCalculation DESC) temp1
	UNION 
	SELECT (Mean+StdDeviation) as value,UTCCalculation as utcdate FROM BaselineStatistics WHERE UTCCalculation BETWEEN @startTime AND @endTime AND SQLServerID=@SQLServerId AND MetricID=@MetricId
	UNION 
	SELECT temp3.value,temp3.utcdate FROM (SELECT TOP 1 (Mean+StdDeviation) as value,@endTime as utcdate FROM BaselineStatistics WHERE UTCCalculation<=@endTime AND SQLServerID=@SQLServerId AND MetricID=@MetricId ORDER BY UTCCalculation DESC) temp3) final 

END
