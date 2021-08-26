if (object_id('p_AssignCounterToServers') is not null)
begin
drop procedure [p_AssignCounterToServers]
end
go

create procedure [p_AssignCounterToServers]
	@Metric int,
	@TagsXML nvarchar(max),
	@ServerXML nvarchar(max),
	@SynchronizeToList bit
as
begin
	DECLARE @err int
	declare @xmlDoc int
	declare @Servers table(
		SQLServerID int,
		InstanceName nvarchar(256),
		LastAlertRefreshTime datetime
	) 

	exec p_UpdateCustomCounterTags @Metric, @TagsXML

	exec sp_xml_preparedocument @xmlDoc output, @ServerXML

	insert into @Servers
		select ms.SQLServerID, ms.InstanceName, ms.LastAlertRefreshTime
			from openxml(@xmlDoc, '//SQLServer', 1) with (SQLServerID int) x
			join MonitoredSQLServers ms (NOLOCK) on ms.SQLServerID = x.SQLServerID
			where ms.Active = 1	

	exec sp_xml_removedocument @xmlDoc

	if (@SynchronizeToList = 1)
	begin
		-- remove mapping of server->counter mapping
		delete from CustomCounterMap 
			where Metric = @Metric and
				SQLServerID not in (select SQLServerID from @Servers)
	end

	-- map counter to instance for statically linked servers
	insert into CustomCounterMap
		select SQLServerID, @Metric from @Servers
			where SQLServerID not in 
					(select SQLServerID from CustomCounterMap (nolock) where Metric = @Metric)

	-- add in servers associated with tags that are linked by tag
	insert into @Servers 
		select distinct MS.SQLServerID, MS.InstanceName, MS.LastAlertRefreshTime 
			from ServerTags ST 
			join CustomCounterTags CCT (nolock) on ST.TagId = CCT.TagId and CCT.Metric = @Metric
			join MonitoredSQLServers MS (nolock) on ST.SQLServerId = MS.SQLServerID
			where ST.SQLServerId not in (select SQLServerID from @Servers)

	if (@SynchronizeToList = 1)
	begin
		-- remove the alert configuration
		delete from MetricThresholds 
			where Metric = @Metric and
				SQLServerID not in (select SQLServerID from @Servers)

		-- deactivate alerts
		update Alerts with (UPDLOCK) set Active = 0
		from Alerts A 
		Where Exists (Select 1 From dbo.MonitoredSQLServers with(nolock)
					Where InstanceName = A.ServerName
					And LastAlertRefreshTime = A.UTCOccurrenceDateTime
					And SQLServerID not in (select SQLServerID from @Servers))
		And A.Metric = @Metric 
		And A.Active = 1
	end

	-- remove counters already mapped from the temp table
	delete from @Servers
		where SQLServerID in (select SQLServerID from MetricThresholds (nolock) where Metric = @Metric)

	-- create a threshold entry from the default for all newly mapped instances
	insert into MetricThresholds 
		select SQLServerID, Metric, [Enabled], WarningThreshold, CriticalThreshold, Data, null, null, null, null, InfoThreshold,[ThresholdInstanceID], [ThresholdEnabled], [IsBaselineEnabled], [BaselineWarningThreshold],
		[BaselineCriticalThreshold], [BaselineInfoThreshold]
			from @Servers left join DefaultMetricThresholds (nolock) on 
				UserViewID = 0 and Metric = @Metric
			where Metric is not null

	SELECT @err = @@error
	RETURN @err
end

