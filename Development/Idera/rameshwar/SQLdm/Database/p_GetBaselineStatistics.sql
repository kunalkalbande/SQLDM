-- SQLdm 9.1 (Sanjali Makkar)

-- Get Baseline For A Particular Metric
-- retrieves the Baseline by Metric ID and Instance ID
-- according to the set filters

-- exec p_GetBaselineStatistics		        @InstanceID = '1',
--											@MetricID = '81',
--											@NumHistoryMin = '4320'

IF (object_id('p_GetBaselineStatistics') is not null)
BEGIN
	DROP PROCEDURE [p_GetBaselineStatistics]
END
GO


Create PROCEDURE [dbo].[p_GetBaselineStatistics] (@InstanceID INT, @MetricID INT = NULL, @NumHistoryMin INT = 60, @EndTimeUTC DATETIME = NULL)
AS

BEGIN

IF(@NumHistoryMin = -1)
BEGIN
	SET @NumHistoryMin = 60;
END
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @EndTime DateTime 
if(@EndTimeUTC is null)
	SELECT @EndTime = MAX(UTCCalculation) FROM BaselineStatistics;
else
	select @EndTime = @EndTimeUTC;

	DECLARE @StartTime DateTime 
	SELECT @StartTime = DATEADD(MINUTE,-@NumHistoryMin, @EndTime)
	
-- Read metric data from MetricInfo table.
-- Read MetricMetaData to determine info where Metric is stored.

;WITH BaselineData(UTCCalculation,
SQLServerID,
TemplateID,
MetricID,
Mean,
StdDeviation,
Min,
Max,
Count)
AS ((SELECT TOP 1 UTCCalculation,
SQLServerID,
TemplateID,
MetricID,
Mean,
StdDeviation,
Min,
Max,
Count FROM BaselineStatistics WHERE UTCCalculation<=@StartTime AND BaselineStatistics.SQLServerID = @InstanceID AND (@MetricID IS NULL OR BaselineStatistics.MetricID = @MetricID) ORDER BY UTCCalculation DESC)
UNION
SELECT UTCCalculation,
SQLServerID,
TemplateID,
MetricID,
Mean,
StdDeviation,
Min,
Max,
Count FROM BaselineStatistics WHERE UTCCalculation BETWEEN @StartTime AND @EndTime)

SELECT 
	BD.SQLServerID AS ServerID, 
	MSS.InstanceName AS ServerName, 
	BD.MetricID AS MetricID, 
	MI.Name AS MetricName, 
	BD.UTCCalculation AS UTCCalculation,
	BD.Mean AS Mean,
	BD.StdDeviation AS StandardDeviation,
	BD.Min AS Min,
	BD.Max AS Max,
	BD.Count AS Count
	
FROM 
	BaselineData BD LEFT OUTER JOIN MetricInfo MI ON 
	BD.MetricID = MI.Metric INNER JOIN 
	MonitoredSQLServers MSS ON BD.SQLServerID = MSS.SQLServerID
	 
WHERE BD.SQLServerID = @InstanceID AND (@MetricID IS NULL OR BD.MetricID = @MetricID)
ORDER BY BD.UTCCalculation ASC;

END
 
GO