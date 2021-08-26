if (object_id('p_AddMonitoredSqlServer') is not null)
begin
drop procedure p_AddMonitoredSqlServer
end
go
CREATE PROCEDURE [dbo].[p_AddMonitoredSqlServer](
	@InstanceName nvarchar(256),
	@CollectionServiceID uniqueidentifier = NULL,
	@UseIntegratedSecurity bit = 1,
	@Username nvarchar(128) = NULL,
	--  SQLdm kit1(Barkha Khatri) alter password length (DE44487 fix)
	@Password nvarchar(256) = NULL,
	@ScheduledCollectionIntervalInSeconds int = 360,
	@QueryMonitorEnabled bit = 0,
	@QueryMonitorSqlBatchEventsEnabled bit = 0,
	@QueryMonitorSqlStatementEventsEnabled bit = 0,
	@QueryMonitorStoredProcedureEventsEnabled bit = 0,
	@ActivityMonitorDeadlockEventsEnabled bit = 0,
	@QueryMonitorDurationFilterInMilliseconds int = 5000,
	@QueryMonitorCpuUsageFilterInMilliseconds int = 0,
	@QueryMonitorLogicalDiskReadsFilter int = 0,
	@QueryMonitorPhysicalDiskWritesFilter int = 0,
	@QueryMonitorTraceFileSizeKB int = 1024,
	@QueryMonitorTraceFileRollovers int = 2,
	@QueryMonitorTraceRecordsPerRefresh int = 1000,
	@GrowthStatisticsStartTime datetime = '1900-01-01 3:00:00 AM',
	@ReorgStatisticsStartTime datetime = '1900-01-01 3:00:00 AM',
	@GrowthStatisticsDays tinyint = 124,
	@ReorgStatisticsDays tinyint = 124,
	@TableStatisticsExcludedDatabases nvarchar(max) = null,
	@EncryptData bit = 0,
	@TrustServerCert bit = 0,
	@ReorgMinTableSizeKB int = 200,
	@DisableReplicationMonitoring bit = 0,
	@QueryMonitorAdvancedConfiguration nvarchar(max) = NULL,
	@DisableExtendedHistoryCollection bit = 0,
	@DisableOleAutomation bit = 0,
	@DiskCollectionSettings nvarchar(max) = NULL,
	@QueryMonitorStopTimeUTC datetime = null,
	@InputBufferLimiter int = 500,
	@InputBufferLimited bit = 0,
	@ActiveWaitCollectorStartTimeRelative datetime = null,
	@ActiveWaitCollectorRunTimeSeconds int = 0, 
	@ActiveWaitCollectorCollectionTimeSeconds int = 30,
	@ActiveWaitCollectorEnabled bit = 0,
	@ActiveWaitLoopTimeMilliseconds int = 500,
	@ActiveWaitAdvancedConfiguration nvarchar(max) = null,
	@ActiveWaitXEEnable bit = 0,
	@ActiveWaitXEFileSizeMB int = 5,
	@ActiveWaitXEFilesRollover int = 1,
	@ActiveWaitXERecordsPerRefresh int = 1000,
	@ActiveWaitXEMaxMemoryMB int = 4,
	@ActiveWaitXEEventRetentionMode tinyint = 1,
	@ActiveWaitXEMaxDispatchLatencySecs int = 300,
	@ActiveWaitXEMaxEventSizeMB int = 8,
	@ActiveWaitXEMemoryPartitionMode int = 0,
	@ActiveWaitXETrackCausality bit = 0,
	@ActiveWaitXEStartupState bit = 0,
	@ActiveWaitsXEFileName nvarchar(1024) = 'dm7SessionOut.xel',
	@ServerPingInterval int = 30,
	@AlertTemplateID int = -1,
	@VHostID int = null,
	@VmUID nvarchar(256) = null,
	@VmName nvarchar(256) = null,
	@VmDomainName nvarchar(256) = null,
	@BaselineTemplate nvarchar(1024) = null,
	@DatabaseStatisticsRefreshIntervalInSeconds int = 3600,
	@WmiCollectionEnabled bit = 0,
	@WmiConnectAsService bit = 0,
	@WmiUserName nvarchar(256) = null,
	@WmiPassword nvarchar(256) = null,
	@ActivityMonitorEnabled bit = 1,
	@ActivityMonitorAutogrowEventsEnabled bit = 0,
	@ActivityMonitorBlockingEventsEnabled bit = 1,
	@ActivityMonitorBlockedProcessThreshold int = 30,
	--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added two newly added parameters
	@QueryMonitorTraceMonitoringEnabled bit,
	@QueryMonitorCollectQueryPlan bit,

	--START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new parameters
	@ActivityMonitorTraceMonitoringEnabled bit,
	@ActivityMonitorXEFileSizeMB int =1,
	@ActivityMonitorXEFilesRollover int =3,
	@ActivityMonitorXERecordsPerRefresh int =1000,
	@ActivityMonitorXEMaxMemoryMB int =1,
	@ActivityMonitorXEEventRetentionMode int =1,
	@ActivityMonitorXEMaxDispatchLatencySecs int =300,
	@ActivityMonitorXEMaxEventSizeMB int =1,
	@ActivityMonitorXEMemoryPartitionMode int =0,
	@ActivityMonitorXETrackCausality bit =0,
	@ActivityMonitorXEStartupState bit =0,
	@ActivityMonitorXEFileName nvarchar(1024)= 'AMExtendedEventLog.xel',
	 --END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new parameters
	 
	--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback -adding new parameters for uery monitoring extended event session configuration
	@QueryMonitorXEFileSizeMB int=20,
	@QueryMonitorXEFilesRollover int=5
	--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for uery monitoring extended event session configuration
-- START 10.0 (srishti purohit) : To handle analysis configuration
	
	,@productionServer bit =0,
	@OLTP bit =0,
	@analysisStartTime DATETIME = NULL,
	@duration INT = 10,
	@scheduledDays smallint = 253,
	@isActive bit = 1,
	@includeDatabase int = 0,
	@filterApplication nvarchar(max) = NULL,
	@categoryID xml = '<Category></Category>',
	@databaseID xml = '<Database></Database>',
	@recommendationID xml = '<Recommendation></Recommendation>',
	-- END 10.0 (srishti purohit) : To handle analysis configuration
	@cloudProviderId int = 4,--SQLdm 10.0 (Tarun Sapra)-Minimal Cloud Support
	@schedulingStatus bit = 0, --SQLdm 10.0 (Praveen Suhalka)-Scheduling Status	
	@QueryMonitorCollectEstimatedQueryPlan bit = 1 ,--10.0 SQLdm Srishti Purohit Fixing defect DE46150. Setting Estimated query plan on by default.
	@isUserSysAdmin bit = 0, --SQLdm 10.1 (Barkha Khatri) adding sysAdmin flag
	@queryMonitorTopPlanCountFilter int = 75,  -- SQLdm 10.4 Varun Chopra adding query monitor top count filter
	@queryMonitorTopPlanCategoryFilter int = 0,  -- SQLdm 10.4 Varun Chopra adding query monitor top category filter
	@QueryMonitorQueryStoreMonitoringEnabled bit = 0,
	@ActiveWaitQsEnable bit = 0, -- SQLdm 10.4 Varun Chopra adding query waits for query store
	@AWSAccessKey nvarchar(max) = NULL, -- SQLdm 10.5 AWS RDS
	@AWSSecretKey nvarchar(max) = NULL, -- SQLdm 10.5 AWS RDS
	@AWSRegionEndpoint nvarchar(max) = NULL -- SQLdm 10.5 AWS RDS
)
as 
begin
	declare @error int
	declare @ExistingSQLServerID int
	declare @CollectionServiceIDTemp uniqueidentifier
	declare @ManagementServiceIDTemp uniqueidentifier
		
	select @CollectionServiceIDTemp = @CollectionServiceID

	if @CollectionServiceIDTemp is null
	begin
		select @ManagementServiceIDTemp = null
		exec p_GetDefaultManagementServiceID @ManagementServiceIDTemp output
		if @ManagementServiceIDTemp is not null
		    exec p_SetDefaultCollectionService @ManagementServiceIDTemp, @CollectionServiceIDTemp output 
	end

	if (@cloudProviderId IS NULL)
	BEGIN
		select @cloudProviderId = 4;
	END
	
	SELECT @ExistingSQLServerID = SQLServerID
	FROM MonitoredSQLServers
	WHERE lower(InstanceName) = lower(@InstanceName)

	IF @@rowcount > 0 BEGIN
		UPDATE [dbo].[MonitoredSQLServers]
		SET
			[Deleted] = 0,
			[Active] = 1,
			[CollectionServiceID] = @CollectionServiceIDTemp,
			[UseIntegratedSecurity] = @UseIntegratedSecurity,
			[Username] = @Username,
			[Password] = @Password,
			[ScheduledCollectionIntervalInSeconds] = @ScheduledCollectionIntervalInSeconds,
			[QueryMonitorEnabled] = @QueryMonitorEnabled,
			[QueryMonitorSqlBatchEventsEnabled] = @QueryMonitorSqlBatchEventsEnabled,
			[QueryMonitorSqlStatementEventsEnabled] = @QueryMonitorSqlStatementEventsEnabled,
			[QueryMonitorStoredProcedureEventsEnabled] = @QueryMonitorStoredProcedureEventsEnabled,
			[QueryMonitorDurationFilterInMilliseconds] = @QueryMonitorDurationFilterInMilliseconds,
			[QueryMonitorCpuUsageFilterInMilliseconds] = @QueryMonitorCpuUsageFilterInMilliseconds,
			[QueryMonitorLogicalDiskReadsFilter] = @QueryMonitorLogicalDiskReadsFilter,
			[QueryMonitorPhysicalDiskWritesFilter] = @QueryMonitorPhysicalDiskWritesFilter,
			[QueryMonitorTraceFileSizeKB] = @QueryMonitorTraceFileSizeKB,
			[QueryMonitorTraceFileRollovers] = @QueryMonitorTraceFileRollovers,
			[QueryMonitorTraceRecordsPerRefresh] = @QueryMonitorTraceRecordsPerRefresh,
			[ActivityMonitorEnabled] = @ActivityMonitorEnabled,
			[ActivityMonitorDeadlockEventsEnabled] = @ActivityMonitorDeadlockEventsEnabled,
			[ActivityMonitorBlockingEventsEnabled] = @ActivityMonitorBlockingEventsEnabled,
			[ActivityMonitorAutoGrowEventsEnabled] = @ActivityMonitorAutogrowEventsEnabled,
			[ActivityMonitorBlockedProcessThreshold] = @ActivityMonitorBlockedProcessThreshold,
			[GrowthStatisticsStartTime] = @GrowthStatisticsStartTime,
			[ReorgStatisticsStartTime] = @ReorgStatisticsStartTime,
			[GrowthStatisticsDays] = @GrowthStatisticsDays,
			[ReorgStatisticsDays] = @ReorgStatisticsDays,
			[TableStatisticsExcludedDatabases] = @TableStatisticsExcludedDatabases,
			[EncryptData] = @EncryptData,
			[TrustServerCert] = @TrustServerCert,
			[ReorgMinTableSizeKB] = @ReorgMinTableSizeKB,
			[DisableReplicationMonitoring] = @DisableReplicationMonitoring,
			[QueryMonitorAdvancedConfiguration] = @QueryMonitorAdvancedConfiguration,
			[DisableExtendedHistoryCollection] = @DisableExtendedHistoryCollection,
			[DisableOleAutomation] = @DisableOleAutomation,
			[DiskCollectionSettings] = @DiskCollectionSettings,
			[QueryMonitorStopTimeUTC] = @QueryMonitorStopTimeUTC,
			[InputBufferLimiter] = @InputBufferLimiter,
			[InputBufferLimited] = @InputBufferLimited,
			[ActiveWaitCollectorStartTimeRelative] = @ActiveWaitCollectorStartTimeRelative,
			[ActiveWaitCollectorRunTimeSeconds] = @ActiveWaitCollectorRunTimeSeconds,
			[ActiveWaitCollectorCollectionTimeSeconds] = @ActiveWaitCollectorCollectionTimeSeconds,
			[ActiveWaitCollectorEnabled] = @ActiveWaitCollectorEnabled,
			[ActiveWaitLoopTimeMilliseconds] = @ActiveWaitLoopTimeMilliseconds,
			[ActiveWaitAdvancedConfiguration] = @ActiveWaitAdvancedConfiguration,
			[ActiveWaitXEEnable] = @ActiveWaitXEEnable,
			[ActiveWaitXEFileSizeMB] = @ActiveWaitXEFileSizeMB,
			[ActiveWaitXEFilesRollover] = @ActiveWaitXEFilesRollover,
			[ActiveWaitXERecordsPerRefresh] = @ActiveWaitXERecordsPerRefresh,
			[ActiveWaitXEMaxMemoryMB] = @ActiveWaitXEMaxMemoryMB,
			[ActiveWaitXEEventRetentionMode] = @ActiveWaitXEEventRetentionMode,
			[ActiveWaitXEMaxDispatchLatencySecs] = @ActiveWaitXEMaxDispatchLatencySecs,
			[ActiveWaitXEMaxEventSizeMB] = @ActiveWaitXEMaxEventSizeMB,
			[ActiveWaitXEMemoryPartitionMode] = @ActiveWaitXEMemoryPartitionMode,
			[ActiveWaitXETrackCausality] = @ActiveWaitXETrackCausality,
			[ActiveWaitXEStartupState] = @ActiveWaitXEStartupState,
			[ActiveWaitsXEFileName] = @ActiveWaitsXEFileName,
			[ServerPingInterval] = @ServerPingInterval,
			[VHostID] = @VHostID,
			[VmUID] = @VmUID,
			[VmName] = @VmName,
			[VmDomainName] = @VmDomainName,
			[DatabaseStatisticsRefreshIntervalInSeconds] = @DatabaseStatisticsRefreshIntervalInSeconds,
			[WmiCollectionEnabled] = @WmiCollectionEnabled,
			[WmiConnectAsService] = @WmiConnectAsService,
			[WmiUserName] = @WmiUserName,
			[WmiPassword] = @WmiPassword,
			--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added two newly added column and values
			[QueryMonitorTraceMonitoringEnabled] =@QueryMonitorTraceMonitoringEnabled,
			[QueryMonitorCollectQueryPlan]=@QueryMonitorCollectQueryPlan,
			[QueryMonitorCollectEstimatedQueryPlan]=@QueryMonitorCollectEstimatedQueryPlan,
			--START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new columns and values 
			[ActivityMonitorTraceMonitoringEnabled]=@ActivityMonitorTraceMonitoringEnabled,
			[ActivityMonitorXEFileSizeMB]=@ActivityMonitorXEFileSizeMB,
			[ActivityMonitorXEFilesRollover]=@ActivityMonitorXEFilesRollover,
			[ActivityMonitorXERecordsPerRefresh]=@ActivityMonitorXERecordsPerRefresh,
			[ActivityMonitorXEMaxMemoryMB]=@ActivityMonitorXEMaxMemoryMB,
			[ActivityMonitorXEEventRetentionMode]=@ActivityMonitorXEEventRetentionMode,
			[ActivityMonitorXEMaxDispatchLatencySecs]=@ActivityMonitorXEEventRetentionMode,
			[ActivityMonitorXEMaxEventSizeMB]=@ActivityMonitorXEMaxEventSizeMB,
			[ActivityMonitorXEMemoryPartitionMode]=@ActivityMonitorXEMemoryPartitionMode,
			[ActivityMonitorXETrackCausality]=@ActivityMonitorXETrackCausality,
			[ActivityMonitorXEStartupState]=@ActivityMonitorXEStartupState,
			[ActivityMonitorXEFileName]=@ActivityMonitorXEFileName,
			 --END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new columns and values 
	--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			[QueryMonitorXEFileSizeMB] =@QueryMonitorXEFileSizeMB,
			[QueryMonitorXEFilesRollover]=@QueryMonitorXEFilesRollover,
	--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			[CloudProviderId] = @cloudProviderId,
			[IsUserSysAdmin]=@isUserSysAdmin, --SQLdm 10.1 (Barkha Khatri) adding sysAdmin flag to MonitoredSQLServers table
			-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
			[QueryMonitorTopPlanCountFilter]=@queryMonitorTopPlanCountFilter,
			[QueryMonitorTopPlanCategoryFilter]=@queryMonitorTopPlanCategoryFilter,
			[QueryMonitorQueryStoreMonitoringEnabled] = @QueryMonitorQueryStoreMonitoringEnabled,
			[ActiveWaitQsEnable] = @ActiveWaitQsEnable			
		WHERE
			[SQLServerID] = @ExistingSQLServerID

		SELECT @error = @@error

		SELECT 
			[SQLServerID],
			[RegisteredDate],
			[CollectionServiceID],
			[OutOfProcOleAutomation]
		FROM [dbo].[MonitoredSQLServers]
		WHERE [SQLServerID] = @ExistingSQLServerID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[MonitoredSQLServers]
			([InstanceName]
			,[CollectionServiceID]
			,[UseIntegratedSecurity]
			,[Username]
			,[Password]
			,[ScheduledCollectionIntervalInSeconds]
			,[QueryMonitorEnabled]
			,[QueryMonitorSqlBatchEventsEnabled]
			,[QueryMonitorSqlStatementEventsEnabled]
			,[QueryMonitorStoredProcedureEventsEnabled]
			,[QueryMonitorDurationFilterInMilliseconds]
			,[QueryMonitorCpuUsageFilterInMilliseconds]
			,[QueryMonitorLogicalDiskReadsFilter]
			,[QueryMonitorPhysicalDiskWritesFilter]
			,[QueryMonitorTraceFileSizeKB]
			,[QueryMonitorTraceFileRollovers]
			,[QueryMonitorTraceRecordsPerRefresh]
			,[ActivityMonitorEnabled]
			,[ActivityMonitorDeadlockEventsEnabled]
			,[ActivityMonitorBlockingEventsEnabled]
			,[ActivityMonitorAutoGrowEventsEnabled]
			,[ActivityMonitorBlockedProcessThreshold]
			,[GrowthStatisticsStartTime]
			,[ReorgStatisticsStartTime]
			,[GrowthStatisticsDays]
			,[ReorgStatisticsDays]
			,[TableStatisticsExcludedDatabases]
			,[EncryptData]
			,[TrustServerCert]
			,[ReorgMinTableSizeKB]
			,[DisableReplicationMonitoring]
			,[QueryMonitorAdvancedConfiguration]
			,[DisableExtendedHistoryCollection]
			,[DisableOleAutomation]
			,[DiskCollectionSettings]
			,[QueryMonitorStopTimeUTC]
			,[InputBufferLimiter]
			,[InputBufferLimited]
			,[ActiveWaitCollectorStartTimeRelative]
			,[ActiveWaitCollectorRunTimeSeconds]
			,[ActiveWaitCollectorCollectionTimeSeconds]
			,[ActiveWaitCollectorEnabled]
			,[ActiveWaitLoopTimeMilliseconds]
			,[ActiveWaitAdvancedConfiguration]
			,[ActiveWaitXEEnable]
			,[ActiveWaitXEFileSizeMB]
			,[ActiveWaitXEFilesRollover]
			,[ActiveWaitXERecordsPerRefresh]
			,[ActiveWaitXEMaxMemoryMB]
			,[ActiveWaitXEEventRetentionMode]
			,[ActiveWaitXEMaxDispatchLatencySecs]
			,[ActiveWaitXEMaxEventSizeMB]
			,[ActiveWaitXEMemoryPartitionMode]
			,[ActiveWaitXETrackCausality]
			,[ActiveWaitXEStartupState]
			,[ActiveWaitsXEFileName]
			,[ServerPingInterval]
			,[VHostID]
			,[VmUID]
			,[VmName]
			,[VmDomainName]
			,[DatabaseStatisticsRefreshIntervalInSeconds]
			,[WmiCollectionEnabled]
			,[WmiConnectAsService]
			,[WmiUserName]
			,[WmiPassword]
			--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Added two newly added columns
			,[QueryMonitorTraceMonitoringEnabled]
			,[QueryMonitorCollectQueryPlan]
			,[QueryMonitorCollectEstimatedQueryPlan]
			--START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new columns 
			,[ActivityMonitorTraceMonitoringEnabled]
			,[ActivityMonitorXEFileSizeMB]
			,[ActivityMonitorXEFilesRollover]
			,[ActivityMonitorXERecordsPerRefresh]
			,[ActivityMonitorXEMaxMemoryMB]
			,[ActivityMonitorXEEventRetentionMode]
			,[ActivityMonitorXEMaxDispatchLatencySecs]
			,[ActivityMonitorXEMaxEventSizeMB]
			,[ActivityMonitorXEMemoryPartitionMode]
			,[ActivityMonitorXETrackCausality]
			,[ActivityMonitorXEStartupState]
			,[ActivityMonitorXEFileName]
			 --END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new columns  
			 
			--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			,[QueryMonitorXEFileSizeMB]
			,[QueryMonitorXEFilesRollover]
			--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			,[CloudProviderId],
			--SQLdm 10.1 (Barkha Khatri) adding sysAdmin flag to MonitoredSQLServers table
			[IsUserSysAdmin],
			-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
			[QueryMonitorTopPlanCountFilter],
			[QueryMonitorTopPlanCategoryFilter],
			[QueryMonitorQueryStoreMonitoringEnabled],
			[ActiveWaitQsEnable],
			[aws_access_key],
			[aws_secret_key],
			[aws_region_endpoint])
		VALUES
			(@InstanceName,
			@CollectionServiceIDTemp,
			@UseIntegratedSecurity,
			@Username,
			@Password,
			@ScheduledCollectionIntervalInSeconds,
			@QueryMonitorEnabled,
			@QueryMonitorSqlBatchEventsEnabled,
			@QueryMonitorSqlStatementEventsEnabled,
			@QueryMonitorStoredProcedureEventsEnabled,
			@QueryMonitorDurationFilterInMilliseconds,
			@QueryMonitorCpuUsageFilterInMilliseconds,
			@QueryMonitorLogicalDiskReadsFilter,
			@QueryMonitorPhysicalDiskWritesFilter,
			@QueryMonitorTraceFileSizeKB,
			@QueryMonitorTraceFileRollovers,
			@QueryMonitorTraceRecordsPerRefresh,
			@ActivityMonitorEnabled,
			@ActivityMonitorDeadlockEventsEnabled,
			@ActivityMonitorBlockingEventsEnabled,
			@ActivityMonitorAutogrowEventsEnabled,
			@ActivityMonitorBlockedProcessThreshold,
			@GrowthStatisticsStartTime,
			@ReorgStatisticsStartTime,
			@GrowthStatisticsDays,
			@ReorgStatisticsDays,
			@TableStatisticsExcludedDatabases,
			@EncryptData,
			@TrustServerCert,
			@ReorgMinTableSizeKB,
			@DisableReplicationMonitoring,
			@QueryMonitorAdvancedConfiguration,
			@DisableExtendedHistoryCollection,
			@DisableOleAutomation,
			@DiskCollectionSettings,
			@QueryMonitorStopTimeUTC,
			@InputBufferLimiter,
			@InputBufferLimited,
			@ActiveWaitCollectorStartTimeRelative,
			@ActiveWaitCollectorRunTimeSeconds, 
			@ActiveWaitCollectorCollectionTimeSeconds,
			@ActiveWaitCollectorEnabled,
			@ActiveWaitLoopTimeMilliseconds,
			@ActiveWaitAdvancedConfiguration,
			@ActiveWaitXEEnable,
			@ActiveWaitXEFileSizeMB,
			@ActiveWaitXEFilesRollover,
			@ActiveWaitXERecordsPerRefresh,
			@ActiveWaitXEMaxMemoryMB,
			@ActiveWaitXEEventRetentionMode,
			@ActiveWaitXEMaxDispatchLatencySecs,
			@ActiveWaitXEMaxEventSizeMB,
			@ActiveWaitXEMemoryPartitionMode,
			@ActiveWaitXETrackCausality,
			@ActiveWaitXEStartupState,
			@ActiveWaitsXEFileName,
			@ServerPingInterval,
			@VHostID,
			@VmUID,
			@VmName,
			@VmDomainName,
			@DatabaseStatisticsRefreshIntervalInSeconds,
			@WmiCollectionEnabled,
			@WmiConnectAsService,
			@WmiUserName,
			@WmiPassword,
			--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added two new column values
			@QueryMonitorTraceMonitoringEnabled,
			@QueryMonitorCollectQueryPlan,
			@QueryMonitorCollectEstimatedQueryPlan,
			--START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new column values
			@ActivityMonitorTraceMonitoringEnabled,
			@ActivityMonitorXEFileSizeMB,
			@ActivityMonitorXEFilesRollover,
			@ActivityMonitorXERecordsPerRefresh,
			@ActivityMonitorXEMaxMemoryMB,
			@ActivityMonitorXEEventRetentionMode,
			@ActivityMonitorXEMaxDispatchLatencySecs,
			@ActivityMonitorXEMaxEventSizeMB,
			@ActivityMonitorXEMemoryPartitionMode,
			@ActivityMonitorXETrackCausality,
			@ActivityMonitorXEStartupState,
			@ActivityMonitorXEFileName,
			--END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Event Session  -- Added new column values
	 
			--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			@QueryMonitorXEFileSizeMB,
			@QueryMonitorXEFilesRollover
			--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			,@cloudProviderId,
			--SQLdm 10.1 (Barkha Khatri) adding sysAdmin flag to MonitoredSQLServers table
			@isUserSysAdmin,
			-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
			@queryMonitorTopPlanCountFilter,
			@queryMonitorTopPlanCategoryFilter,
			@QueryMonitorQueryStoreMonitoringEnabled,
			@ActiveWaitQsEnable,
			@AWSAccessKey,
			@AWSSecretKey,
			@AWSRegionEndpoint)
		SELECT @error = @@error

		if (@error = 0)
		BEGIN
			--DECLARE @defaultID int

			SELECT @ExistingSQLServerID = scope_identity()	
			if (@AlertTemplateID < 0)
			begin		
				SELECT @AlertTemplateID = [TemplateID] FROM [AlertTemplateLookup] where [Default] = 1
			end

			insert into MetricThresholds (SQLServerID, Metric, Enabled, InfoThreshold, WarningThreshold, CriticalThreshold, Data, ThresholdInstanceID, IsBaselineEnabled, BaselineWarningThreshold , BaselineCriticalThreshold , BaselineInfoThreshold)
				select @ExistingSQLServerID, Metric, Enabled, InfoThreshold, WarningThreshold, CriticalThreshold, Data, ThresholdInstanceID,  IsBaselineEnabled, BaselineWarningThreshold , BaselineCriticalThreshold , BaselineInfoThreshold 			
					from DefaultMetricThresholds
					where UserViewID = @AlertTemplateID
						and Metric not in 
							(select Metric from CustomCounterDefinition)

			SELECT 
				[SQLServerID],
				[RegisteredDate],
				[CollectionServiceID],
				[OutOfProcOleAutomation],
				[DisableOleAutomation]
			FROM [dbo].[MonitoredSQLServers]
			WHERE [SQLServerID] = @ExistingSQLServerID
		END
	END
	
	IF @error=0
	BEGIN
	  IF NOT EXISTS(SELECT SQLServerID FROM AlertInstanceTemplate WHERE SQLServerID = @ExistingSQLServerID)
		  BEGIN
				INSERT INTO AlertInstanceTemplate(SQLServerID,TemplateID)VALUES(@ExistingSQLServerID,@AlertTemplateID)
		  END		
	END
	
	IF @error = 0 AND @BaselineTemplate IS NOT NULL
	BEGIN		
		exec p_AddBaselineTemplate @ExistingSQLServerID, @BaselineTemplate

		IF @error <> 0 BEGIN
			RAISERROR('An error occurred while add the new instance.', 10, 1, @error)
			RETURN(@error)
		END
	END

	IF @error = 0
	BEGIN	
	-- START 10.0 (srishti purohit) : To handle analysis configuration
	exec p_SaveAnalysisConfiguration @ExistingSQLServerID,@productionServer,@OLTP,
	@analysisStartTime ,
	@duration ,@scheduledDays ,@isActive ,
	@includeDatabase ,
	@filterApplication ,
	@categoryID ,
	@databaseID ,
	@recommendationID,
	@schedulingStatus --SQLdm 10.0 (Praveen Suhalka)-Scheduling Status

	IF @error != 0 --GOTO HANDLE_ERROR
	BEGIN
	RAISERROR('An error occurred while updating instance ANALYSIS %d.', 10, 1, @error)
        RETURN(@error)
		END
		END

	-- END 10.0 (srishti purohit) : To handle analysis configuration

	RETURN(0)
end




