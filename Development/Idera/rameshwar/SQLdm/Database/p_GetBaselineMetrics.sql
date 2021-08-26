if (object_id('p_GetBaselineMetrics') is not null)
begin
drop procedure p_GetBaselineMetrics
end
go
CREATE PROCEDURE [dbo].[p_GetBaselineMetrics]
	@SendDefault tinyint = 1-- 0 No, 1 Yes
AS
BEGIN
--We need to match the UNIT of BaselineMetaData with UNIT on p_GetBaselineStatisticsReport (KB to MB, MB to GB)
DECLARE @UpdatedMetricUnit TABLE(
    [Name] nvarchar(128),
    [MetricValue] nvarchar(128),
    [Unit] nvarchar(32),
	[MetricID] int
);
IF(@SendDefault = 1)
BEGIN
	INSERT INTO @UpdatedMetricUnit VALUES('< Select a Value >',NULL,NULL,-1)
END;
INSERT @UpdatedMetricUnit
SELECT Name, 
		MetricValue,
		Unit = (SELECT CASE [Unit]
					When 'KB' THEN 'MB'
					When 'MB' THEN 'GB'
					When 'Milliseconds' THEN CASE When [MetricID] = 22 OR [MetricID] = -93 OR [MetricID] = -100 OR [MetricID] = -101 THEN 'Milliseconds' ELSE 'Seconds' END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When [MetricID] = 76 OR [MetricID] = 17 THEN 'Seconds' ELSE 'Minutes' END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE [Unit]
					END),
		MetricID FROM BaselineMetaData
--Now we concat the metric name and unit to avoid display duplicated metric names
SELECT [MetricID], isnull([Name], [MetricValue]) + isnull(' ('+[Unit]+ ')', '') AS Name
	FROM @UpdatedMetricUnit
	WHERE [MetricID] IS NOT NULL
	ORDER BY [Name] ASC
END
 
