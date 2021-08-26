if (object_id('p_AddDefaultMetricThreshold') is not null)
begin
drop procedure [p_AddDefaultMetricThreshold]
end
go

create procedure [p_AddDefaultMetricThreshold]
	@UserViewID int,
	@Metric int,
	@Enabled bit,
	@WarningThreshold nvarchar(2048),
	@CriticalThreshold nvarchar(2048),
	@Data nvarchar(max),
	@InfoThreshold nvarchar(2048)
as
begin
	DECLARE @metricType INT;
	DECLARE @defaultID int
	DECLARE @err int
	declare @tempID int
	-- adding default baseline values to defaultmetricthreshold
	-- 10.0 SQLdm srishti purohit
	DECLARE @BaselineWarningThreshold nvarchar(2048)
	DECLARE @BaselineCriticalThreshold nvarchar(2048)
	DECLARE @BaselineInfoThreshold nvarchar(2048)
	DECLARE @xmlValueForBaselineToCastWarning xml
	DECLARE @xmlValueForBaselineToCastCritical xml
	DECLARE @xmlValueForBaselineToCastInfo xml
	DECLARE @WarningThresholdXML10 nvarchar(512),
				@CriticalThresholdXML10 nvarchar(512),
				@InfoThresholdXML10 nvarchar(512),
				@BaselineDefaultWarningValue10 nvarchar(256),
				@BaselineDefaultCriticalValue10 nvarchar(256),
				@BaselineDefaultInfoValue10 nvarchar(256),
				@xmSignature nvarchar(256)
	set @err = 0

	select @tempID = MIN(TemplateID)-1 from AlertTemplateLookup
	
	while (Select MAX(TemplateID) from AlertTemplateLookup) > @tempID
	begin
		select @tempID = MIN(TemplateID) from AlertTemplateLookup where TemplateID > @tempID

		if not exists (select [Metric] from [DefaultMetricThresholds] where [UserViewID] = @tempID and [Metric] = @Metric)
		begin
		--10.0 SQLdm 
		-- START Baseline support
		
				--To get operation type from xml of thresholds
				select @xmlValueForBaselineToCastWarning = CAST(@WarningThreshold as XML) ,
				 @xmlValueForBaselineToCastCritical = CAST(@CriticalThreshold as XML) ,
				@xmlValueForBaselineToCastInfo = CAST(@InfoThreshold as XML)
				-- append value			
				select @BaselineDefaultWarningValue10 = [BaselineDefaultWarningValue],
						@BaselineDefaultCriticalValue10 = [BaselineDefaultCriticalValue],
						@BaselineDefaultInfoValue10 = [BaselineDefaultInfoValue]
				from [dbo].[MetricMetaData] 
				where Metric = @Metric
				
			--ANSI_WARNINGS is not honored when passing parameters in a stored procedure , so setting it on	
				SET @WarningThresholdXML10 = CONVERT( nvarchar(512), @xmlValueForBaselineToCastWarning.query('data(//@Op)'))
				SET @CriticalThresholdXML10 = CONVERT(nvarchar(512), @xmlValueForBaselineToCastCritical.query('data(//@Op)'))
				SET @InfoThresholdXML10 = CONVERT(nvarchar(512), @xmlValueForBaselineToCastInfo.query('data(//@Op)'))
	
				-- START --Srishti Purohit -- To accomadate baseline alert feature
				
				SET @xmSignature ='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Op="'

				SET @xmSignature = @xmSignature 
			
				set	@BaselineWarningThreshold = @xmSignature + @WarningThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' + @BaselineDefaultWarningValue10 + '</Value></Threshold>'
				set	@BaselineCriticalThreshold = @xmSignature + @CriticalThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' +  @BaselineDefaultCriticalValue10 + '</Value></Threshold>'
				set	@BaselineInfoThreshold = @xmSignature + @InfoThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' +  @BaselineDefaultInfoValue10 + '</Value></Threshold>'
				-- END --Srishti Purohit -- To accomadate baseline alert feature
		--END Baseline support
		SET @metricType = (
				SELECT MetricType
				FROM [CustomCounterDefinition]
				WHERE [Metric] = @Metric
				)

		INSERT	into DefaultMetricThresholds (UserViewID, Metric, Enabled, WarningThreshold, CriticalThreshold, Data, InfoThreshold, IsBaselineEnabled, [BaselineWarningThreshold] ,[BaselineCriticalThreshold], [BaselineInfoThreshold]) 
		VALUES (@tempID, @Metric, @Enabled, @WarningThreshold, @CriticalThreshold, @Data, @InfoThreshold, 
			CASE WHEN @Metric > 145 AND @metricType <> 4 THEN 1 -- Set Baseline for Azure Metric - Metric Type 4
				 ELSE 0 END, 
			@BaselineWarningThreshold ,@BaselineCriticalThreshold ,@BaselineInfoThreshold)
			SELECT @err = @@error
		
			if (@err = 0)
			begin
				-- add the metric to all defined servers
				insert into MetricThresholds (SQLServerID, Metric, Enabled, WarningThreshold, CriticalThreshold, Data, InfoThreshold , [IsBaselineEnabled], [BaselineWarningThreshold] ,[BaselineCriticalThreshold], [BaselineInfoThreshold]) 
					select S.SQLServerID, D.Metric, D.Enabled, D.WarningThreshold, D.CriticalThreshold, D.Data, D.InfoThreshold, D.IsBaselineEnabled,  D.BaselineWarningThreshold , D.BaselineCriticalThreshold , D.BaselineInfoThreshold			
						from MonitoredSQLServers S, DefaultMetricThresholds D
							where D.UserViewID = @tempID
								and D.Metric = @Metric 
								and D.Metric not in (select T.Metric from MetricThresholds T where T.SQLServerID = S.SQLServerID)
								and D.Metric not in (select Metric from CustomCounterDefinition)

				SELECT @err = @@error
			end
		end
	end

	RETURN @err
end

