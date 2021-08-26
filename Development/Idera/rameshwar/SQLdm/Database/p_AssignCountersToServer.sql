if (object_id('p_AssignCountersToServer') is not null)
begin
drop procedure [p_AssignCountersToServer]
end
go

create procedure [p_AssignCountersToServer]
	@SQLServerID int,
	@MetricXML nvarchar(max),
	@SynchronizeToList bit
as
begin
	DECLARE @err int
	declare @xmlDoc int
	declare @Metrics table(
		Metric int
	) 
	declare @InstanceName nvarchar(256)


	exec sp_xml_preparedocument @xmlDoc output, @MetricXML

	insert into @Metrics	
	select distinct MetricID
		from openxml(@xmlDoc, '//Metric', 1) with (MetricID int) 

	exec sp_xml_removedocument @xmlDoc

	-- map the new counter to the instance
	insert into CustomCounterMap
		select @SQLServerID, Metric from @Metrics
			where Metric not in 
				(select Metric from CustomCounterMap where SQLServerID = @SQLServerID)

	if (@SynchronizeToList = 1) 
	begin
		-- remove mappings for metrics not in the new list
		delete from CustomCounterMap 
			where SQLServerID = @SQLServerID and
				Metric not in (select Metric from @Metrics)
	end

	-- add all custom counters assigned by tag
	insert into @Metrics 
		select distinct CCT.Metric from CustomCounterTags CCT
			join ServerTags ST on ST.TagId = CCT.TagId
			where ST.SQLServerId = @SQLServerID and
				CCT.Metric not in (select Metric from @Metrics)

	if (@SynchronizeToList = 1) 
	begin
		-- remove thresholds for custom counters not mapped to the server
		delete from MetricThresholds 
			where SQLServerID = @SQLServerID and
				Metric in (select Metric from CustomCounterDefinition) and
				Metric not in (select Metric from @Metrics)
		-- deactivate alerts for custom counters not mapped to the server
		select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID
		if (@InstanceName is not null) 
		begin
			update Alerts set Active = 0
				where Active = 1 and
					ServerName = @InstanceName and
					Metric in (select Metric from CustomCounterDefinition) and
					Metric not in (select Metric from @Metrics)
		end			
	end

	-- remove metrics already mapped from the new list
	delete from @Metrics 
		where Metric in (select Metric from MetricThresholds where SQLServerID = @SQLServerID)

	--lookup the default alert template
	DECLARE @defaultID int
	SELECT @defaultID = [TemplateID] FROM [AlertTemplateLookup] where [Default] = 1
		
	-- create a threshold entry from the default for all newly mapped instances
	insert into MetricThresholds 
		select @SQLServerID, M.Metric, [Enabled], WarningThreshold, CriticalThreshold, Data, null, null, null, null, InfoThreshold,ThresholdInstanceID,ThresholdEnabled, DMT.IsBaselineEnabled, DMT.[BaselineWarningThreshold],
		DMT.[BaselineCriticalThreshold], DMT.[BaselineInfoThreshold]
			from @Metrics M left join DefaultMetricThresholds DMT on 
				UserViewID = @defaultID and M.Metric = DMT.Metric
			where DMT.Metric is not null

	-- return a list of all metric IDs mapped to the server
	select Metric from CustomCounterMap where SQLServerID = @SQLServerID

	SELECT @err = @@error
	RETURN @err
end

