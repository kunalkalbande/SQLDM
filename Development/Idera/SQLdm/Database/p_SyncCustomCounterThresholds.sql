IF (object_id('p_SyncCustomCounterThresholds') IS NOT NULL)
BEGIN
DROP PROCEDURE p_SyncCustomCounterThresholds
END
GO

CREATE PROCEDURE [dbo].[p_SyncCustomCounterThresholds]
AS
BEGIN
	declare @MissingMetrics table(SQLServerID int not null, Metric int not null)

	-- thresholds that need creating
	insert into @MissingMetrics
	select distinct ST.SQLServerId, CCT.Metric
		from ServerTags ST
			left outer join CustomCounterTags CCT on ST.TagId = CCT.TagId
			left outer join MetricThresholds MT on ST.SQLServerId = MT.SQLServerID and CCT.Metric = MT.Metric
		where
			MT.Metric is null and CCT.Metric is not null

	--lookup the default alert template
	DECLARE @defaultID int
	SELECT @defaultID = [TemplateID] FROM [AlertTemplateLookup] where [Default] = 1

	insert into MetricThresholds
		select MM.SQLServerID, MM.Metric, DMT.Enabled, DMT.WarningThreshold, DMT.CriticalThreshold, DMT.Data, NULL, NULL, NULL, NULL, DMT.InfoThreshold, DMT.ThresholdInstanceID,DMT.ThresholdEnabled, DMT.IsBaselineEnabled, DMT.[BaselineWarningThreshold],
		DMT.[BaselineCriticalThreshold], DMT.[BaselineInfoThreshold]
			from @MissingMetrics MM 
				left join DefaultMetricThresholds DMT on DMT.UserViewID = @defaultID and MM.Metric = DMT.Metric
			where DMT.Metric is not null

	-- remove custom counter thresholds that are 
	-- no longer linked or associated by tag
    delete MT
        from MetricThresholds MT
            join CustomCounterDefinition CCD on MT.Metric = CCD.Metric
            left join CustomCounterTags CCT on MT.Metric = CCT.Metric
            left join ServerTags ST on MT.SQLServerID = ST.SQLServerId and CCT.TagId = ST.TagId 
        where 
            CCT.TagId is null and ST.SQLServerId is null

END