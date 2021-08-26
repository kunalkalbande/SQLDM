if (object_id('p_GetCounters') is not null)
begin
drop procedure p_GetCounters
end
go

CREATE PROCEDURE [dbo].p_GetCounters(
	@Metric int,
	@IncludeMetricInfo bit = 1,
	@IncludeMessages bit = 1,
	@IncludeCustomDefinitions bit = 1,
	@CustomCountersOnly bit = 0,
	@UTCChangedAfterDateTime datetime,
	@nonAzureOnly BIT = 0,	-- Returns only non azure custom counters (used to restrict in tags)
	@cloudProviderId INT = NULL
)
AS
begin
	declare @e int
	
	if (@IncludeMetricInfo = 0)
		SELECT MMD.[Metric],MMD.[UTCLastChangeDateTime],[Deleted],[Class],[Flags],[MinValue],[MaxValue],[DefaultWarningValue],[DefaultCriticalValue],[DoNotifications],[EventCategory],[DefaultMessageID],[AlertEnabledDefault],[ValueComparison],[ValueType],[Rank],[DefaultInfoValue]
		--Getting default aand max value for baseline alert
		,[BaselineMaxValue],[BaselineDefaultWarningValue],[BaselineDefaultCriticalValue], [BaselineDefaultInfoValue]
			FROM [MetricMetaData] MMD
			LEFT JOIN [CustomCounterDefinition] CCD on CCD.Metric = MMD.Metric
			WHERE (@Metric is null or MMD.[Metric] = @Metric) 
				AND (@CustomCountersOnly = 0 and exists(select 1 from [CustomCounterDefinition] CC where CC.[Metric] = MMD.[Metric]))
				AND (@UTCChangedAfterDateTime is null or MMD.[UTCLastChangeDateTime] > @UTCChangedAfterDateTime)
				AND ([Deleted] = 0)
				AND (@nonAzureOnly IS NULL OR @nonAzureOnly = 0 OR (@nonAzureOnly = 1 AND CCD.MetricType <> 4)) -- When Non Azure is required allow non azure metrics
				AND (@cloudProviderId IS NULL OR (CCD.MetricType <> 4 AND @cloudProviderId <> 2) -- Allow non azure metric for non azure servers
				   OR (@cloudProviderId = 2) OR (@cloudProviderId = 5)) -- Allow same behaviour for azure server
				   
				 
	else
		SELECT  MMD.[Metric], 
			    'UTCLastChangeDateTime' =
					CASE WHEN MMD.[UTCLastChangeDateTime] > MI.[UTCLastChangeDateTime] then MMD.[UTCLastChangeDateTime] else MI.[UTCLastChangeDateTime] end,
			    [Deleted],[Class],[Flags],[MinValue],[MaxValue],[DefaultWarningValue],[DefaultCriticalValue],[DoNotifications],[EventCategory],[DefaultMessageID],[AlertEnabledDefault],[ValueComparison],[ValueType],MI.[Rank],[Category],[Name],[Description],[Comments],[DefaultInfoValue]
			--Getting default aand max value for baseline alert
		,[BaselineMaxValue],[BaselineDefaultWarningValue],[BaselineDefaultCriticalValue], [BaselineDefaultInfoValue]
		FROM [MetricMetaData] MMD 
		INNER JOIN [MetricInfo] MI  on MMD.Metric = MI.Metric
		LEFT OUTER JOIN [CustomCounterDefinition] CCD on CCD.Metric = MI.Metric
			WHERE MMD.[Metric] = MI.[Metric]
				AND (@Metric is null or MMD.[Metric] = @Metric)
				AND (@CustomCountersOnly = 0 or exists(select 1 from [CustomCounterDefinition] CC where CC.[Metric] = MMD.[Metric]))
				AND (@UTCChangedAfterDateTime is null or MMD.[UTCLastChangeDateTime] > @UTCChangedAfterDateTime or MI.[UTCLastChangeDateTime] > @UTCChangedAfterDateTime)
				AND ([Deleted] = 0) 
				AND (@nonAzureOnly IS NULL OR @nonAzureOnly = 0 OR (@nonAzureOnly = 1 AND CCD.MetricType <> 4)) -- When Non Azure is required allow non azure metrics
				AND ( @cloudProviderId IS NULL OR (CCD.MetricType IS NULL OR CCD.MetricType <> 4 AND @cloudProviderId <> 2) -- Allow non azure metric for non azure servers
				   OR (@cloudProviderId = 2) OR (@cloudProviderId = 5)) -- Allow same behaviour for azure server
	
	SELECT @e = @@error

	if (@e = 0)
	begin
		if (@IncludeMessages <> 0)
		begin 
			SELECT MMDM.[Metric],[MessageID],[EventID],[HeaderTemplate],[BodyTemplate],[TodoTemplate], 
			[PulseTemplate], [HeaderTemplate_Baseline], [BodyTemplate_Baseline], [TodoTemplate_Baseline], [PulseTemplate_Baseline]
				FROM [MetricMetaDataMessages] MMDM
				LEFT JOIN [CustomCounterDefinition] CCD ON CCD.Metric = MMDM.Metric 
				WHERE (@Metric is null or MMDM.[Metric] = @Metric)
					AND (@nonAzureOnly IS NULL OR @nonAzureOnly = 0 OR (@nonAzureOnly = 1 AND CCD.MetricType <> 4)) -- When Non Azure is required allow non azure metrics
				   -- Metric Type is not Azure Metric and cloud provider is not azure server
				   AND (@cloudProviderId IS NULL OR (CCD.MetricType IS NULL OR CCD.MetricType <> 4 AND @cloudProviderId <> 2) -- Allow non azure metric for non azure servers
				   OR (@cloudProviderId = 2) OR (@cloudProviderId = 5)) -- Allow same behaviour for azure server
				ORDER BY MMDM.[Metric],[MessageID]
			
			SELECT [Metric],[Value],[MessageID] 
				FROM [MetricMetaDataMessageMap]
				WHERE (@Metric is null or [Metric] = @Metric)
				ORDER BY [Metric]
		end	

	if (@IncludeCustomDefinitions <> 0)
		begin
			SELECT [Metric],[UTCLastChangeDateTime],[Enabled],[MetricType],[CalculationType],[Scale],[Object],[Counter],[Instance],[Batch] ,[ServerType], [AzureProfileId]
				FROM [CustomCounterDefinition] CCD
				WHERE (@Metric is null or [Metric] = @Metric)
					AND (@UTCChangedAfterDateTime is null or [UTCLastChangeDateTime] > @UTCChangedAfterDateTime) 
					AND ([Metric] not in (SELECT [Metric] from [MetricMetaData] where [Deleted] = 1))
					AND (@nonAzureOnly IS NULL OR @nonAzureOnly = 0 OR (@nonAzureOnly = 1 AND CCD.MetricType <> 4)) -- When Non Azure is required allow non azure metrics
					AND (@cloudProviderId IS NULL OR (CCD.MetricType IS NULL OR CCD.MetricType <> 4 AND @cloudProviderId <> 2) -- Allow non azure metric for non azure servers
				   OR (@cloudProviderId = 2) OR (@cloudProviderId = 5)) -- Allow same behaviour for azure server
				ORDER BY [Metric]
		end
	end

	RETURN @e
END	

