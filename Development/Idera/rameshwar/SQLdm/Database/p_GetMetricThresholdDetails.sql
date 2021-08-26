if (object_id('p_GetMetricThresholdDetails') is not null)
begin
drop procedure [p_GetMetricThresholdDetails]
end
go
create procedure [dbo].[p_GetMetricThresholdDetails]
				@ServerID int,
				@InMetric int = null
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	declare @metricID int
	declare @AlertThresholds table (
			MetricID int, 
			WarningThreshold nvarchar(200), 
			CriticalThreshold nvarchar(200),
			InfoThreshold nvarchar(200),
			criticalThresholdEnabled nvarchar(256),
			warningThresholdEnabled nvarchar(256),
			infoThresholdEnabled nvarchar(256) ,
			thresholdInstanceId int)

	declare @MetricStatus table (
			MetricID int,
			MetricStatus bit)			
			
	declare @InfoThresholdXML nvarchar(2048), 
			@WarningThresholdXML nvarchar(2048), 
			@CriticalThresholdXML nvarchar(2048),
			@enabled bit,
			@thresholdInstanceId int,
			@thresholdEnabled bit,
			@xmlDoc int, 
			@warningValueArray nvarchar(4000), 		
			@criticalValueArray nvarchar(4000), 		
			@infoValueArray nvarchar(4000), 		
			@serviceState nvarchar(256),
			@criticalThresholdEnabled nvarchar(256),
			@warningThresholdEnabled nvarchar(256),
			@infoThresholdEnabled nvarchar(256)
	
	SET @infoValueArray = ''
	SET @warningValueArray = ''
	SET @criticalValueArray = ''
	
	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]

	insert into @MetricStatus select Metric, Enabled from MetricThresholds where SQLServerID = @ServerID and ThresholdInstanceID < 0
	
	--Get the thresholds for each metric		
	declare read_metricId insensitive cursor 
	for
		select Metric from MetricMetaData where @InMetric is null or Metric = @InMetric

	for read only
		set nocount on 
		open read_metricId 
		fetch read_metricId into @metricID

	while @@fetch_status = 0 
	begin

		declare  read_threshold insensitive cursor
		 for
			select  WarningThreshold,
					CriticalThreshold,
					InfoThreshold,
					MT.Enabled,
					MT.ThresholdInstanceID
			from MetricThresholds MT (nolock)
			where Metric = @metricID and SQLServerID = @ServerID
		
		for read only
			set nocount on
			open read_threshold
			fetch read_threshold into @WarningThresholdXML, @CriticalThresholdXML, @InfoThresholdXML, @enabled, @thresholdInstanceId
			
		while @@FETCH_STATUS = 0
		begin	
		
			exec sp_xml_preparedocument @xmlDoc output, @WarningThresholdXML
			select @warningThresholdEnabled = [Enabled], @warningValueArray = Value from openxml(@xmlDoc, '/Threshold', 3) with (Enabled nvarchar(256), Value nvarchar(2048))
			
			declare read_threshold_entry cursor
			for
				select servicestate from openxml(@xmlDoc, '//anyType', 2) with (servicestate nvarchar(256) 'text()')

			open read_threshold_entry
			fetch read_threshold_entry into @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				set @warningValueArray = ''
			END
			
			while @@fetch_status = 0
			begin
				set @warningValueArray = @warningValueArray + @serviceState + ', '
				fetch read_threshold_entry into @serviceState
			end
			Close read_threshold_entry
			deallocate read_threshold_entry
			exec sp_xml_removedocument @xmlDoc
			
			exec sp_xml_preparedocument @xmlDoc output, @CriticalThresholdXML
			select @criticalValueArray = Value, @criticalThresholdEnabled = [Enabled] from openxml(@xmlDoc, '/Threshold', 3) with (Value nvarchar(2048), Enabled nvarchar(256))

			declare read_threshold_entry cursor
			for
				select servicestate from openxml(@xmlDoc, '//anyType', 2) with (servicestate nvarchar(256) 'text()')

			open read_threshold_entry
			fetch read_threshold_entry into @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				set @criticalValueArray = ''
			END
			
			while @@fetch_status = 0
			begin
				set @criticalValueArray = @criticalValueArray + @serviceState + ', '
				fetch read_threshold_entry into @serviceState
			end
	
			Close read_threshold_entry
			deallocate read_threshold_entry
			exec sp_xml_removedocument @xmlDoc			

			exec sp_xml_preparedocument @xmlDoc output, @InfoThresholdXML
			select @infoValueArray = Value, @infoThresholdEnabled = [Enabled] from openxml(@xmlDoc, '/Threshold', 3) with (Value nvarchar(2048), Enabled nvarchar(256))
			
			declare read_threshold_entry cursor
			for
				select servicestate from openxml(@xmlDoc, '//anyType', 2) with (servicestate nvarchar(256) 'text()')

			open read_threshold_entry
			fetch read_threshold_entry into @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				set @infoValueArray = ''
			END
			
			while @@fetch_status = 0
			begin
				set @infoValueArray = @infoValueArray + @serviceState + ', '
				fetch read_threshold_entry into @serviceState
			end
	
			Close read_threshold_entry
			deallocate read_threshold_entry
			exec sp_xml_removedocument @xmlDoc						
			
			insert into @AlertThresholds ([MetricID], [WarningThreshold], [CriticalThreshold], [InfoThreshold], [criticalThresholdEnabled], [warningThresholdEnabled], [infoThresholdEnabled], [thresholdInstanceId])
								 VALUES (@metricID, @warningValueArray, @criticalValueArray, @infoValueArray, @criticalThresholdEnabled, @warningThresholdEnabled, @infoThresholdEnabled, @thresholdInstanceId)
		
			fetch read_threshold into @WarningThresholdXML, @CriticalThresholdXML, @InfoThresholdXML, @enabled, @thresholdInstanceId
		end
		close read_threshold
		deallocate read_threshold
		fetch read_metricId into @metricID
	end
	Close read_metricId 
	deallocate read_metricId 
	
	select	mi.Metric,
			mi.Name, 
			ms.InstanceName, 
			mi.Description, 
			stat.MetricStatus as [Enabled], 
			mt.ThresholdEnabled,
			Category, 
			ISNULL(mti.ThresholdInstanceName,'') as [ThresholdInstanceName],
			at.InfoThreshold, 
			at.CriticalThreshold, 
			at.WarningThreshold,
			at.criticalThresholdEnabled,
			at.warningThresholdEnabled,
			at.infoThresholdEnabled
	from MetricThresholds mt 
	left join MetricThresholdInstances mti on mt.ThresholdInstanceID = mti.InstanceID
	left join MetricInfo mi on mi.Metric = mt.Metric
	left join #SecureMonitoredSQLServers ms on ms.SQLServerID = @ServerID
	left join @AlertThresholds at on at.MetricID = mt.Metric and at.thresholdInstanceId = mt.ThresholdInstanceID
	left join @MetricStatus stat on mt.Metric = stat.MetricID
	where mt.SQLServerID = @ServerID and at.MetricID is not null
	ORDER BY ms.InstanceName ASC, mi.Name ASC
end