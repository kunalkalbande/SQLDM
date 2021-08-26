-- SQLdm 10.0 (Srishti Purohit)

-- Get Baseline Stddeviation and mean to calculated difference from base line threshold
-- retrieves the Baseline by Instance ID
-- according to the set filters

-- exec p_GetBaselineCheckThresholdDeviation		        @InstanceID = '1'

IF (object_id('p_GetBaselineCheckThresholdDeviation') is not null)
BEGIN
	DROP PROCEDURE [p_GetBaselineCheckThresholdDeviation]
END
GO


CREATE PROCEDURE [dbo].[p_GetBaselineCheckThresholdDeviation] (@InstanceID INT)
AS

BEGIN


	
-- All baseline mean related to every template will be captured (including inactive)
-- If we do not have baseline statistics that ran based on the current baseline settings, the last calculated baselines will apply
-- The last calculated baseline may or may not be of the immediately previous baseline setting.
-- If none of the previous custom baselines apply for a given time, the latest calculated default baseline will apply.

;WITH BaselineDataRankWise(Mean, StdDeviation, TemplateID,   MetricID, UTCCalculation,RankMetric)
AS (select Mean, StdDeviation, TemplateID, MetricID, UTCCalculation, ROW_NUMBER() over (Partition BY TemplateID, MetricID order by UTCCalculation DESC) as RankMetric
from 
BaselineStatistics WITH (NOLOCK) WHERE SQLServerID = @InstanceID)

SELECT  
	CASE WHEN BT.BaselineName = 'Default' THEN -1 ELSE BD.TemplateID END As TemplateID,
	BT.Template AS Template,
	BT.Active AS Active,
	M.Metric AS MetricID, 
	M.Name AS MetricName,
	BD.Mean + BD.StdDeviation AS Mean,
	ISNULL(BD.UTCCalculation, GETDATE()) AS UTCCalculation

FROM BaselineMetaData B 
	JOIN MetricInfo M ON B.MetricID = M.Metric
	JOIN BaselineDataRankWise BD ON M.Metric = BD.MetricID
	JOIN BaselineTemplates BT ON BT.TemplateID = BD.TemplateID
		 
WHERE ((RankMetric = 1 OR RankMetric IS NULL) AND (BT.Template LIKE '%ScheduledStartDate%' COLLATE SQL_Latin1_General_CP1_CI_AI OR BT.BaselineName = 'Default' ))
	GROUP BY BD.TemplateID, BT.Template, BT.Active, M.Name, M.Metric, BD.Mean, BD.StdDeviation, UTCCalculation, BT.BaselineName
ORDER BY BT.Active DESC, UTCCalculation DESC
END
 
GO