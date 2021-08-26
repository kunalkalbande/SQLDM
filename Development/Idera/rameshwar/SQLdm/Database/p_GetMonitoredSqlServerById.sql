IF (object_id('[p_GetMonitoredSqlServerById]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetMonitoredSqlServerById]
END
GO

CREATE PROCEDURE [dbo].[p_GetMonitoredSqlServerById](
	@Id int,
	@IncludeCounters bit = 0
)
AS
BEGIN
	DECLARE @error int

	SELECT 
		a.[SQLServerID],
		a.[InstanceName],
		a.[FriendlyServerName],
	
		a.[Active],
		a.[RegisteredDate],
		a.[CollectionServiceID],
		a.[UseIntegratedSecurity],
		a.[Username],
		a.[Password],
		a.[ScheduledCollectionIntervalInSeconds],
		a.[MaintenanceModeEnabled],
		a.[QueryMonitorEnabled],
		a.[QueryMonitorSqlBatchEventsEnabled],
		a.[QueryMonitorSqlStatementEventsEnabled],
		a.[QueryMonitorStoredProcedureEventsEnabled],
		a.[QueryMonitorDurationFilterInMilliseconds],
		a.[QueryMonitorCpuUsageFilterInMilliseconds],
		a.[QueryMonitorLogicalDiskReadsFilter],
		a.[QueryMonitorPhysicalDiskWritesFilter],
		a.[QueryMonitorTraceFileSizeKB],
		a.[QueryMonitorTraceFileRollovers],
		a.[QueryMonitorTraceRecordsPerRefresh],
		--SQLdm 9.0 (Ankit Srivastava) -- Query Monitor Improvement with Extended event -- fetching newly added columns -start
		a.[QueryMonitorTraceMonitoringEnabled],
		a.[QueryMonitorCollectQueryPlan],
		--SQLdm 9.0 (Ankit Srivastava) -- Query Monitor Improvement with Extended event -- fetching newly added columns -end
		a.[QueryMonitorCollectEstimatedQueryPlan],
		--SQLdm 10.0 (Tarun Sapra) - Modified the stored procedure to get the new column for estimated query plan flag
		a.[GrowthStatisticsStartTime],
		a.[ReorgStatisticsStartTime],
		a.[LastGrowthStatisticsRunTime],
		a.[LastReorgStatisticsRunTime],
		a.[GrowthStatisticsDays],
		a.[ReorgStatisticsDays],
		a.[TableStatisticsExcludedDatabases],
		a.[EncryptData],
		a.[TrustServerCert],
		(SELECT min(ServerStatistics.UTCCollectionDateTime)
		FROM ServerStatistics (NOLOCK)
		WHERE ServerStatistics.SQLServerID = a.SQLServerID) AS EarliestData,
		a.[ReorgMinTableSizeKB],
		a.[CustomCounterTimeoutInSeconds],
		a.[OutOfProcOleAutomation],
		a.[DisableReplicationMonitoring],
		a.[MaintenanceModeType],
		a.[MaintenanceModeStart],
		a.[MaintenanceModeStop],
		a.[MaintenanceModeDurationSeconds],
		a.[MaintenanceModeDays],
		a.[MaintenanceModeRecurringStart],

		a.[MaintenanceModeMonth],

		a.[MaintenanceModeSpecificDay],

		a.[MaintenanceModeWeekOrdinal],

		a.[MaintenanceModeWeekDay],

		a.[MaintenanceModeMonthDuration],

		a.[MaintenanceModeMonthRecurringStart],
		a.[MaintenanceModeOnDemand],

		a.[QueryMonitorAdvancedConfiguration],
		a.[DisableExtendedHistoryCollection],
		a.[DisableOleAutomation],
		a.[DiskCollectionSettings],
		a.[QueryMonitorStopTimeUTC],
		a.[InputBufferLimiter],
		a.[InputBufferLimited],
		a.[ActiveClusterNode],
		a.[PreferredClusterNode],
		a.[ActiveWaitCollectorStartTimeRelative],
		a.[ActiveWaitCollectorRunTimeSeconds],
		a.[ActiveWaitCollectorCollectionTimeSeconds],
		a.[ActiveWaitCollectorEnabled],	
		a.[ActiveWaitLoopTimeMilliseconds],
		a.[ActiveWaitAdvancedConfiguration],
		a.[ActiveWaitXEEnable],
		a.[ActiveWaitXEFileSizeMB],
		a.[ActiveWaitXEFilesRollover],
		a.[ActiveWaitXERecordsPerRefresh],
		a.[ActiveWaitXEMaxMemoryMB],
		a.[ActiveWaitXEEventRetentionMode],
		a.[ActiveWaitXEMaxDispatchLatencySecs],
		a.[ActiveWaitXEMaxEventSizeMB],
		a.[ActiveWaitXEMemoryPartitionMode],
		a.[ActiveWaitXETrackCausality],
		a.[ActiveWaitXEStartupState],
		a.[ActiveWaitsXEFileName],
		a.[ClusterCollectionSetting],
		a.[ServerPingInterval],
		a.[VHostID],
		a.[VmUID],
		a.[VmName],
		a.[VmDomainName],
		b.[VHostName],
		b.[VHostAddress],
		b.[ServerType],
		b.[Username] as [VCUserName],
		b.[Password] as [VCPassword],
		a.[AlertRefreshInMinutes],
		c.[Template] as [BaselineTemplate],		
		c.[TemplateID] as [BaselineTemplateID],
		c.[BaselineName] as [BaselineName],	
		--START -- SQLdm10.1 (srishti Purohit) -- Get all custom baselines
		(SELECT STUFF((SELECT ', ' + CAST(BT.TemplateID AS VARCHAR(10)) +', '+ BT.Template +', '+ BT.BaselineName [text()]
		FROM BaselineTemplates BT
         WHERE SQLServerID = BTS.SQLServerID AND Active = '1' AND BaselineName != 'Default'
		 GROUP BY BT.TemplateID, BT.Template, BT.BaselineName
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ')List_Output
		FROM BaselineTemplates BTS
		WHERE BTS.SQLServerID = @Id AND BTS.Active = '1' and BTS.BaselineName != 'Default'
		GROUP BY SQLServerID
		HAVING @Id = SQLServerID) AS CustomBaselineTemplates,
		--END --SQLdm10.1 (srishti Purohit) -- Get all custom baselines
		c.[Active] as [BaselineActive],		
		ServerVersion,
		ServerEdition,
		JobAlertEnabled,
		LogAlertEnabled,
		DatabaseStatisticsRefreshIntervalInSeconds,
		WmiCollectionEnabled,
		WmiConnectAsService,
		WmiUserName,
		WmiPassword,
		a.[ActivityMonitorEnabled],
		--START SQLdm 9.1 (Ankit Srivastava) -- Activity Monitor with Extended Events Improvement with Extended event -- fetching newly added columns
		a.[ActivityMonitorTraceMonitoringEnabled],
		a.[ActivityMonitorXEFileSizeMB],
		a.[ActivityMonitorXEFilesRollover],
		a.[ActivityMonitorXERecordsPerRefresh],
		a.[ActivityMonitorXEMaxMemoryMB],
		a.[ActivityMonitorXEEventRetentionMode],
		a.[ActivityMonitorXEMaxDispatchLatencySecs],
		a.[ActivityMonitorXEMaxEventSizeMB],
		a.[ActivityMonitorXEMemoryPartitionMode],
		a.[ActivityMonitorXETrackCausality],
		a.[ActivityMonitorXEStartupState],
		a.[ActivityMonitorXEFileName],
		--END SQLdm 9.1 (Ankit Srivastava) -- Activity Monitor with Extended Events Improvement with Extended event -- fetching newly added columns
		a.[ActivityMonitorDeadlockEventsEnabled],
		a.[ActivityMonitorBlockingEventsEnabled],
		a.[ActivityMonitorAutoGrowEventsEnabled],
		a.[ActivityMonitorBlockedProcessThreshold],
		f.[InstanceName] as [SS_InstanceName],
		f.[DatabaseName] as [SS_DatabaseName],
		f.[SecurityMode] as [SS_SecurityMode],
		f.[UserName] as [SS_UserName],
		f.[EncryptedPassword] as [SS_EncryptedPassword],
		d.[RelatedInstanceId] as [SS_RelatedInstanceId],
		d.[LastBackupActionId] as [SS_LastBackupActionId],
		d.[LastDefragActionId] as [SS_LastDefragActionId],
		--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - Fetching new columns for uery monitoring extended event session configuration
		[QueryMonitorXEFileSizeMB],
		[QueryMonitorXEFilesRollover],
		--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - Fetching new columns for uery monitoring extended event session configuration
		
		-- START 10.0 (srishti purohit) : To handle analysis configuration
		analysisConfig.[ProductionServer] AS [AnalysisProductionServer],
		analysisConfig.[OLTP],
		analysisConfig.[StartTime],
		analysisConfig.[Duration],
		analysisConfig.[ScheduledDays],
		analysisConfig.[SchedulingStatus],--SQLDM10.0 (praveen suhalka) : scheduling status
		analysisConfig.[IncludeDatabase],
		(SELECT SSDN.DatabaseName FROM SQLServerDatabaseNames SSDN WHERE SSDN.DatabaseID = analysisConfig.[IncludeDatabase] 
		AND SSDN.IsDeleted = 0) -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 
		AS IncludeDatabaseName,
		analysisConfig.[IsActive],
		analysisConfig.[FilterApplication]
		,(SELECT STUFF((SELECT ', ' + CAST(CategoryID AS VARCHAR(10)) [text()]
         FROM AnalysisConfigCategories (NOLOCK)
         WHERE AnalysisConfigurationID = acc.AnalysisConfigurationID AND  analysisConfig.ID = AnalysisConfigurationID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM AnalysisConfigCategories acc (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedCategories
		,(SELECT STUFF((SELECT ', ' + CAST(CC.CategoryID AS VARCHAR(10)) +', '+ PRC.Name [text()]
		FROM AnalysisConfigCategories CC (NOLOCK)
		JOIN PrescriptiveRecommendationCategory PRC ON PRC.CategoryID = CC.CategoryID
         WHERE AnalysisConfigurationID = acc.AnalysisConfigurationID AND  analysisConfig.ID = AnalysisConfigurationID
		 AND CC.CategoryID = PRC.CategoryID
		 GROUP BY CC.CategoryID, PRC.Name
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ')List_Output
		FROM AnalysisConfigCategories acc (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedCategoriesWithName
      ,(SELECT STUFF((SELECT ', ' + CAST(DatabaseID AS VARCHAR(10)) [text()]
         FROM AnalysisConfigBlockedDatabases (NOLOCK)
         WHERE AnalysisConfigurationID = acbd.AnalysisConfigurationID AND  analysisConfig.ID = AnalysisConfigurationID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM AnalysisConfigBlockedDatabases acbd (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedDatabases
		,(SELECT STUFF((SELECT ', ' + CAST(BD.DatabaseID AS VARCHAR(10)) +', '+ SSDN.DatabaseName [text()]
         FROM AnalysisConfigBlockedDatabases  BD (NOLOCK)

				JOIN SQLServerDatabaseNames SSDN ON SSDN.DatabaseID = BD.DatabaseID
         WHERE AnalysisConfigurationID = acbd.AnalysisConfigurationID AND  analysisConfig.ID = AnalysisConfigurationID
		 AND BD.DatabaseID = SSDN.DatabaseID 
		 AND SSDN.IsDeleted = 0 -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 
		 GROUP BY BD.DatabaseID, SSDN.DatabaseName
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM AnalysisConfigBlockedDatabases acbd (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedDatabasesWithName
		,(SELECT STUFF((SELECT ',' + CAST(RecommendationID AS VARCHAR(10)) [text()]
         FROM AnalysisConfigBlockedRecommendation (NOLOCK)
         WHERE AnalysisConfigurationID = acbr.AnalysisConfigurationID AND  analysisConfig.ID = AnalysisConfigurationID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,1,'') List_Output
		FROM AnalysisConfigBlockedRecommendation acbr (NOLOCK)
		GROUP BY AnalysisConfigurationID
		HAVING analysisConfig.ID = AnalysisConfigurationID) AS BlockedRecommendations,
		-- END 10.0 (srishti purohit) : To handle analysis configuration
		--SQLdm 10.1 sys admin flag 
		a.[IsUserSysAdmin],
		a.[CloudProviderId] as [CloudProviderId],
		-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
		a.[QueryMonitorTopPlanCountFilter],
		a.[QueryMonitorTopPlanCategoryFilter],
		a.[QueryMonitorQueryStoreMonitoringEnabled],
		a.[ActiveWaitQsEnable]

	FROM [MonitoredSQLServers] a (NOLOCK)
		LEFT JOIN [VirtualHostServers] b 		(NOLOCK)
			on a.VHostID = b.VHostID 
		LEFT JOIN [BaselineTemplates] c (NOLOCK)
				on a.SQLServerID = c.SQLServerID and c.TemplateID = (select MAX(TemplateID) from BaselineTemplates (NOLOCK) where SQLServerID = a.SQLServerID and BaselineName = 'Default' and Active='1')
		LEFT JOIN [MonitoredServerSQLsafeInstance] d (NOLOCK)
			on a.SQLServerID = d.SQLServerID
		LEFT JOIN [SQLsafeConnections] f (NOLOCK)
			on d.RepositoryId = f.RepositoryId and f.Deleted <> 1
		LEFT JOIN AnalysisConfiguration analysisConfig (NOLOCK)
			on a.SQLServerID = analysisConfig.MonitoredServerID AND analysisConfig.IsActive = 1
		outer apply
		(
		select top 1 Enabled as JobAlertEnabled
		from MetricThresholds mt (NOLOCK)
		where Enabled = 1
		and mt.SQLServerID = a.SQLServerID
		-- Job related alerts:
		-- 34 - SQL Server Agent Long Running Job (Percent)
		-- 35 - SQL Server Agent Job Failure
		-- 65 - SQL Server Agent Long Running Job (Minutes)
		-- 88 - SQL Server Agent Job Completion
		and Metric in (34,35,65,88)
		) j
	outer apply
	(
		select top 1 Enabled as LogAlertEnabled
		from MetricThresholds mt (NOLOCK)
		where Enabled = 1
		and mt.SQLServerID = a.SQLServerID
		-- Job related alerts:
		-- 66 - SQL Server Error Log
		-- 67 - SQL Server Agent Log
		and Metric in (66,67)
		) e
	WHERE a.[SQLServerID] = @Id

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	SELECT [Metric] 
	FROM [CustomCounterMap] (NOLOCK)
	WHERE [SQLServerID] = @Id

	SELECT [TagId] 
	FROM [ServerTags] (NOLOCK)
	WHERE [SQLServerId] = @Id

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server instance %d.', 10, 1, @Id)
        RETURN(@error)
END
 

GO


