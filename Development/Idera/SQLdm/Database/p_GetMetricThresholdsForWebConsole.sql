IF (OBJECT_ID('p_GetMetricThresholdsForWebConsole') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMetricThresholdsForWebConsole
END
GO
CREATE PROCEDURE [dbo].p_GetMetricThresholdsForWebConsole		
	  @MetricID BIGINT =  NULL,
		@SQLServerID INT = NULL
AS
BEGIN
	with T(SQLServerID, MetricID, InfoThreshold) AS 
	(SELECT mi.SQLServerID, mi.Metric, 
		CONVERT(XML, mi.InfoThreshold)
	FROM MetricThresholds mi
	WHERE Metric = ISNULL(@MetricID, Metric)
		AND SQLServerID = ISNULL(@SQLServerID, SQLServerID)
	)		
	SELECT t.SQLServerID, t.MetricID, T2.InfoThreshold.query('.') AS InfoThreshold FROM T t
	CROSS APPLY t.InfoThreshold.nodes('/Threshold/Value/anyType/text()') AS T2(InfoThreshold)
	UNION ALL
	SELECT t.SQLServerID, t.MetricID, T2.InfoThreshold.query('.') AS InfoThreshold FROM T t
	CROSS APPLY t.InfoThreshold.nodes('/Threshold/Value/text()') AS T2(InfoThreshold);

	with T(SQLServerID, MetricID, WarningThreshold) AS 
	(SELECT mi.SQLServerID, mi.Metric, 
		CONVERT(XML, mi.WarningThreshold)
	FROM MetricThresholds mi
	WHERE Metric = ISNULL(@MetricID, Metric)
		AND SQLServerID = ISNULL(@SQLServerID, SQLServerID)
	)		
	SELECT t.SQLServerID, t.MetricID, T2.WarningThreshold.query('.') AS WarningThreshold FROM T t
	CROSS APPLY t.WarningThreshold.nodes('/Threshold/Value/anyType/text()') AS T2(WarningThreshold)
	UNION ALL
	SELECT t.SQLServerID, t.MetricID, T2.WarningThreshold.query('.') AS WarningThreshold FROM T t
	CROSS APPLY t.WarningThreshold.nodes('/Threshold/Value/text()') AS T2(WarningThreshold);
	
	with T(SQLServerID, MetricID, CriticalThreshold) AS 
	(SELECT mi.SQLServerID, mi.Metric, 
		CONVERT(XML, mi.CriticalThreshold)
	FROM MetricThresholds mi
	WHERE Metric = ISNULL(@MetricID, Metric)
		AND SQLServerID = ISNULL(@SQLServerID, SQLServerID)
	)
	SELECT t.SQLServerID, t.MetricID, T2.CriticalThreshold.query('.') AS CriticalThreshold FROM T t
	CROSS APPLY t.CriticalThreshold.nodes('/Threshold/Value/anyType/text()') AS T2(CriticalThreshold)
	UNION ALL
	SELECT t.SQLServerID, t.MetricID, T2.CriticalThreshold.query('.') AS CriticalThreshold FROM T t
	CROSS APPLY t.CriticalThreshold.nodes('/Threshold/Value/text()') AS T2(CriticalThreshold);
END