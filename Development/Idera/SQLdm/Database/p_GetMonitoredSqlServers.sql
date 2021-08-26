IF (object_id('[p_GetMonitoredSqlServers]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_GetMonitoredSqlServers]
END
GO

CREATE PROCEDURE [dbo].[p_GetMonitoredSqlServers]
(
	@CollectionServiceId UNIQUEIDENTIFIER = NULL,
	@ActiveOnly BIT = 0,
	@GetLicenseKeys BIT = 0, -- Indicates if we should also return the license keys.
	@CustomCounters BIT = 0,  -- Indicates if we should also return the custom counters mapped to the server
	@FilterField VARCHAR(256) = NULL, 
	@FilterValue VARCHAR(256) = NULL
)
AS
BEGIN
	DECLARE @error int

	create table #serverIds 
	(
		SQLServerID int not null
	);

	
	
	if (@FilterField is not null and @FilterValue is not null)
	begin

		if (@FilterField = 'TagId')
		begin
			insert into #serverIds
			select DISTINCT mss.[SQLServerID] from dbo.[MonitoredSQLServers] mss (NOLOCK)
			INNER JOIN [ServerTags] st ON st.SQLServerId = mss.SQLServerID
			where [Deleted] = 0 AND st.TagId = @FilterValue
		end

		else if (@FilterField = 'InstanceName')
		begin
			insert into #serverIds
			select DISTINCT [SQLServerID] from dbo.[MonitoredSQLServers] mss (NOLOCK)
			where [Deleted] = 0 AND mss.InstanceName LIKE @FilterValue
		end
		else if (@FilterField = '')
		begin	
			insert into #serverIds
			select DISTINCT [SQLServerID] from dbo.[MonitoredSQLServers] (NOLOCK)
			where [Deleted] = 0
		end
	end
	else 
	begin
		insert into #serverIds
		select DISTINCT [SQLServerID] from dbo.[MonitoredSQLServers] (NOLOCK)
		where [Deleted] = 0
	end

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
		a.[LastAlertRefreshTime],
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
		--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --Get the newly added columns from  the repository
		a.QueryMonitorTraceMonitoringEnabled,
		a.QueryMonitorCollectQueryPlan,
		--SQLdm 10.0 (Tarun Sapra): Get the flag for displaying the estimated query plan only
		a.QueryMonitorCollectEstimatedQueryPlan,
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
		--START -- SQLdm10.1 (srishti Purohit) -- Get all custom baselines
		(SELECT STUFF((SELECT ', ' + CAST(BT.TemplateID AS VARCHAR(10)) +', '+ BT.Template +', '+ BT.BaselineName [text()] 
		FROM BaselineTemplates BT
         WHERE SQLServerID = BTS.SQLServerID AND Active = '1' AND BaselineName != 'Default'
		 GROUP BY BT.TemplateID, BT.Template, BT.BaselineName
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ')List_Output
		FROM BaselineTemplates BTS
		WHERE BTS.SQLServerID = a.SQLServerID AND BTS.Active = '1' and BTS.BaselineName != 'Default'
		GROUP BY SQLServerID
		HAVING a.SQLServerID = SQLServerID) AS CustomBaselineTemplates,
		--END --SQLdm10.1 (srishti Purohit) -- Get all custom baselines
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
		--START SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --Get the newly added columns from  the repository
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
		--END SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  --Get the newly added columns from  the repository
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
		ISNULL(
				(
					SELECT ISNULL(Logins,0) FROM ServerStatistics stat (NOLOCK)
					WHERE stat.SQLServerID = a.SQLServerID 
					AND stat.UTCCollectionDateTime = (SELECT MAX(stat_temp.UTCCollectionDateTime) FROM ServerStatistics stat_temp WHERE stat_temp.SQLServerID = a.SQLServerID)
				),
				0
		) LastRunActiveUserSessions,

		ISNULL(
				(
					SELECT ISNULL(CPUActivityPercentage,0) FROM ServerStatistics stat (NOLOCK)
					WHERE stat.SQLServerID = a.SQLServerID 
					AND stat.UTCCollectionDateTime = (SELECT MAX(stat_temp.UTCCollectionDateTime) FROM ServerStatistics stat_temp WHERE stat_temp.SQLServerID = a.SQLServerID)
				),0
		) LastRunCPUActivityPercentage,

		ISNULL(
				(
					SELECT ISNULL([SqlMemoryAllocatedInKilobytes],0) FROM ServerStatistics stat (NOLOCK)
					WHERE stat.SQLServerID = a.SQLServerID 
					AND stat.UTCCollectionDateTime = (SELECT MAX(stat_temp.UTCCollectionDateTime) FROM ServerStatistics stat_temp WHERE stat_temp.SQLServerID = a.SQLServerID)
				),
		0) LastRunSqlMemoryAllocatedInKilobytes,
		ISNULL(
				(
					SELECT ISNULL([SqlMemoryUsedInKilobytes],0) FROM ServerStatistics stat (NOLOCK)
					WHERE stat.SQLServerID = a.SQLServerID 
					AND stat.UTCCollectionDateTime = (SELECT MAX(stat_temp.UTCCollectionDateTime) FROM ServerStatistics stat_temp WHERE stat_temp.SQLServerID = a.SQLServerID)
				),
		0) LastRunSqlMemoryUsedInKilobytes,

		ISNULL(
				(
					SELECT ISNULL(IOActivityPercentage,0) FROM ServerStatistics stat (NOLOCK)
					WHERE stat.SQLServerID = a.SQLServerID 
					AND stat.UTCCollectionDateTime = (SELECT MAX(stat_temp.UTCCollectionDateTime) FROM ServerStatistics stat_temp WHERE stat_temp.SQLServerID = a.SQLServerID)
				),
		0) LastRunIOActivityPercentage,
		--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - Fetching new columns for uery monitoring extended event session configuration
		[QueryMonitorXEFileSizeMB],
		[QueryMonitorXEFilesRollover]
		--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - Fetching new columns for uery monitoring extended event session configuration

		
		-- START 10.0 (srishti purohit) : To handle analysis configuration
		,analysisConfig.[ProductionServer] AS [AnalysisProductionServer],
		analysisConfig.[OLTP],
		analysisConfig.[SchedulingStatus],--SQLDM10.0 (praveen suhalka) : scheduling status
		analysisConfig.[StartTime],
		analysisConfig.[Duration],
		analysisConfig.[ScheduledDays],
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
		LEFT JOIN [VirtualHostServers] b (NOLOCK)
			on a.VHostID = b.VHostID 
		LEFT JOIN [BaselineTemplates] c (NOLOCK)
			on a.SQLServerID = c.SQLServerID and c.TemplateID = (select MAX(TemplateID) from BaselineTemplates (NOLOCK) where SQLServerID = a.SQLServerID and BaselineName = 'Default' and Active='1')
		LEFT JOIN [MonitoredServerSQLsafeInstance] d (NOLOCK)
			on a.SQLServerID = d.SQLServerID
		LEFT JOIN [SQLsafeConnections] f (NOLOCK)
			on d.RepositoryId = f.RepositoryId and f.Deleted <> 1
		LEFT JOIN AnalysisConfiguration analysisConfig (NOLOCK)
			on a.SQLServerID = analysisConfig.MonitoredServerID AND analysisConfig.IsActive = 1
		
		INNER JOIN #serverIds sids (NOLOCK)
		    on a.SQLServerID = sids.SQLServerID
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
	WHERE
		((@CollectionServiceId IS NULL) OR (a.[CollectionServiceID] = @CollectionServiceId)) AND
		((@ActiveOnly = 0) OR (a.[Active] = 1))
	
	IF (@GetLicenseKeys != 0) 
    BEGIN
		DECLARE @ReturnServerCount int
		DECLARE @ret int
		DECLARE @inst nvarchar(128) 
		EXEC @ret = [p_GetLicenseKeys] 
			@LicenseID = null, 
			@ReturnServerCount = @ReturnServerCount output, 
			@ReturnInstanceName = @inst output
	END

	IF (@CustomCounters = 1)
	BEGIN
		SELECT [CCM].[SQLServerID],[CCM].[Metric] 
			FROM CustomCounterMap CCM (NOLOCK), MonitoredSQLServers MSS (NOLOCK)
			WHERE ([CCM].[SQLServerID] = [MSS].[SQLServerID]) and
				((@ActiveOnly = 0) OR ([MSS].[Active] = 1))
		union
		SELECT ST.SQLServerId, CCT.Metric from ServerTags ST (NOLOCK)
			JOIN CustomCounterTags CCT (NOLOCK) on ST.TagId = CCT.TagId
			LEFT OUTER JOIN CustomCounterMap CCM (NOLOCK) on ST.SQLServerId = CCM.SQLServerID and CCT.Metric = CCM.Metric
			inner join MonitoredSQLServers MSS (NOLOCK) on ST.SQLServerId = MSS.SQLServerID
			WHERE ((@ActiveOnly = 0) OR ([MSS].[Active] = 1))
		ORDER BY SQLServerID
		
		
		SELECT ST.[SQLServerId], ST.[TagId]
			FROM ServerTags ST (NOLOCK), MonitoredSQLServers MSS (NOLOCK)
			WHERE (ST.[SQLServerId] = [MSS].[SQLServerID]) and
				((@ActiveOnly = 0) OR ([MSS].[Active] = 1))
			ORDER BY [ST].[SQLServerId]
	END



	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server instances.', 10, 1)
        RETURN(@error)			
END
 

GO


