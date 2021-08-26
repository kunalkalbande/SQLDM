IF (object_id('p_GetMetricList') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMetricList
END
GO

CREATE PROCEDURE [dbo].p_GetMetricList
AS
BEGIN

	--SELECT mi.Name,mmd.Metric FROM MetricMetaData mmd 
	--INNER JOIN MetricInfo mi ON mmd.Metric=mi.Metric 
	--INNER JOIN BaselineMetaData bmd on mmd.Metric = bmd.MetricID
	--WHERE TableName IS NOT NULL AND ColumnName IS NOT NULL
	SELECT CASE
		WHEN p.Name IN (SELECT Name FROM BaselineMetaData WHERE Name IS NOT NULL GROUP BY Name HAVING COUNT(Name)>1)
			THEN p.Name+'('+p.Unit+')'
		ELSE p.Name
		END AS MetricName,p.ItemID,ISNULL(p.Unit,'') AS Unit
	FROM BaselineMetaData p
	WHERE p.Name IS NOT NULL

END