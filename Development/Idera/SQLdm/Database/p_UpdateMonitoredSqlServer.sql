IF (object_id('p_UpdateMonitoredSqlServer') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_UpdateMonitoredSqlServer]
END
GO


CREATE PROCEDURE [dbo].[p_UpdateMonitoredSqlServer]
(
	@SqlServerId int,
	@Active bit,
	@CollectionServiceId uniqueidentifier,
	@UseIntegratedSecurity bit,
	@Username nvarchar(128),
	--  SQLdm kit1(Barkha Khatri) alter password length (DE44487 fix)
	@Password nvarchar(256),
	@ScheduledCollectionIntervalInSeconds int,
	@MaintenanceModeEnabled bit,
	@QueryMonitorEnabled bit,
	@QueryMonitorSqlBatchEventsEnabled bit,
	@QueryMonitorSqlStatementEventsEnabled bit,
	@QueryMonitorStoredProcedureEventsEnabled bit,
	@ActivityMonitorDeadlockEventsEnabled bit,
	@QueryMonitorDurationFilterInMilliseconds int,
	@QueryMonitorCpuUsageFilterInMilliseconds int,
	@QueryMonitorLogicalDiskReadsFilter int,
	@QueryMonitorPhysicalDiskWritesFilter int,
	@QueryMonitorTraceFileSizeKB int,
	@QueryMonitorTraceFileRollovers int,
	@QueryMonitorTraceRecordsPerRefresh int,
	@GrowthStatisticsStartTime datetime = '1900-01-01 3:00:00 AM',
	@ReorgStatisticsStartTime datetime = '1900-01-01 3:00:00 AM',
	@GrowthStatisticsDays tinyint = 253,
	@ReorgStatisticsDays tinyint = 253,
	@TableStatisticsExcludedDatabases nvarchar(max) = null,
	@EncryptData bit = 0,
	@TrustServerCert bit = 0,
	@ReorgMinTableSizeKB int = 200,
	@DisableReplicationMonitoring bit = 0,
	@MaintenanceModeType int = 0,
	@MaintenanceModeStart datetime = null,
	@MaintenanceModeStop datetime = null,
	@MaintenanceModeDurationSeconds int = null,
	@MaintenanceModeDays smallint = 0,
	@QueryMonitorAdvancedConfiguration nvarchar(max) = null,
	@DisableExtendedHistoryCollection bit = 0,
	@MaintenanceModeRecurringStart datetime = null,
	@DisableOleAutomation bit = 0,
	@DiskCollectionSettings nvarchar(max) = null,
	@QueryMonitorStopTimeUTC datetime = null,
	@InputBufferLimiter int = 500,
	@InputBufferLimited bit = 0,
	@PreferredClusterNode nvarchar(256) = null,
	@ActiveWaitCollectorStartTimeRelative datetime = null,
	@ActiveWaitCollectorRunTimeSeconds int = 0, 
	@ActiveWaitCollectorCollectionTimeSeconds int = 30,
	@ActiveWaitCollectorEnabled bit = 0,
	@ActiveWaitLoopTimeMilliseconds int = 500,
	@ActiveWaitAdvancedConfiguration nvarchar(max) = null,
	@ServerPingInterval int = 30,
	@VHostID int = -1,
	@VmUID nvarchar(256) = null,
	@VmName nvarchar(256) = null,
	@VmDomainName nvarchar(256) = null,
	@AlertRefreshInMinutes bit = true,
	@BaselineTemplate nvarchar(1024) = null,
	@DatabaseStatisticsRefreshIntervalInSeconds int,
	@ActiveWaitXEEnable bit = 1,
	@ActiveWaitXEFileSizeMB int = 1,
	@ActiveWaitXEFilesRollover int = 3,
	@ActiveWaitXERecordsPerRefresh int = 1000,
	@ActiveWaitXEMaxMemoryMB int = 1,
	@ActiveWaitXEEventRetentionMode tinyint = 1,
	@ActiveWaitXEMaxDispatchLatencySecs int = 300,
	@ActiveWaitXEMaxEventSizeMB int = 1,
	@ActiveWaitXEMemoryPartitionMode int = 0,
	@ActiveWaitXETrackCausality bit = 0,
	@ActiveWaitXEStartupState bit = 0,
	@ActiveWaitsXEFileName nvarchar(1024) = 'dm7XESessionOut.xel',
	@WmiCollectionEnabled bit = 0,
	@WmiConnectAsService bit = 0,
	@WmiUserName nvarchar(256) = null,
	@WmiPassword nvarchar(256) = null,
	@ActivityMonitorEnabled bit,
	@ActivityMonitorBlockingEventsEnabled bit,
	@ActivityMonitorAutogrowEventsEnabled bit,

	@ActivityMonitorBlockedProcessThreshold int,

	@MaintenanceModeMonth int,

	@MaintenanceModeSpecificDay int,

	@MaintenanceModeWeekOrdinal int,

	@MaintenanceModeWeekDay int,

	@MaintenanceModeMonthDuration int,

	@MaintenanceModeMonthRecurringStart datetime = null,
	--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added two new parameters for updating the MonitoredSqlServer
	@QueryMonitorTraceMonitoringEnabled bit,
	@QueryMonitorCollectQueryPlan bit,

	--START SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  -- Added new parameters
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
	 --END SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  -- Added new parameters
	--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback -adding new parameters for uery monitoring extended event session configuration
	@QueryMonitorXEFileSizeMB int=20,
	@QueryMonitorXEFilesRollover int=5,
	--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for uery monitoring extended event session configuration
	@friendlyServerName nvarchar(256) = null,
	@baselineConfigList xml
	-- START 10.0 (srishti purohit) : To handle analysis configuration
	
	,@productionServer bit,
	@OLTP bit,
	@analysisStartTime DATETIME,
	@duration INT,
	@scheduledDays smallint,
	@isActive bit,
	@includeDatabase int,
	@filterApplication nvarchar(max),
	@categoryID xml,
	@databaseID xml,
	@recommendationID xml
	
	-- END 10.0 (srishti purohit) : To handle analysis configuration
	,@QueryMonitorCollectEstimatedQueryPlan bit--SQLdm 10.0 (Tarun Sapra)- Flag for displaying estimated query plan only
	,@schedulingStatus bit --SQLDM10.0 (praveen suhalka) : scheduling status
	-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
	,@queryMonitorTopPlanCountFilter INT
	,@queryMonitorTopPlanCategoryFilter INT
	,@QueryMonitorQueryStoreMonitoringEnabled BIT
	,@ActiveWaitQsEnable bit = 1
	,@MaintenanceModeOnDemand bit = 0
	,@CloudProviderId int
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @error int
	DECLARE @rowcount int

	UPDATE [dbo].[MonitoredSQLServers]
	SET 
		[Active] = @Active,
		[FriendlyServerName] = @friendlyServerName,
		[CollectionServiceID] = @CollectionServiceId,
		[UseIntegratedSecurity] = @UseIntegratedSecurity,
		[Username] = @Username,
		[Password] = @Password,
		[ScheduledCollectionIntervalInSeconds] = @ScheduledCollectionIntervalInSeconds,
		[MaintenanceModeEnabled] = @MaintenanceModeEnabled,
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
		[MaintenanceModeType] = @MaintenanceModeType,
		[MaintenanceModeStart] = @MaintenanceModeStart,
		[MaintenanceModeStop] = @MaintenanceModeStop,
		[MaintenanceModeDurationSeconds] = @MaintenanceModeDurationSeconds,
		[MaintenanceModeDays] = @MaintenanceModeDays,
		[QueryMonitorAdvancedConfiguration] = @QueryMonitorAdvancedConfiguration,
		[DisableExtendedHistoryCollection] = @DisableExtendedHistoryCollection,
		[MaintenanceModeRecurringStart] = @MaintenanceModeRecurringStart,
		[DisableOleAutomation] = @DisableOleAutomation,
		[DiskCollectionSettings] = @DiskCollectionSettings,
		[QueryMonitorStopTimeUTC] = @QueryMonitorStopTimeUTC,
		[InputBufferLimiter] = @InputBufferLimiter,
		[InputBufferLimited] = @InputBufferLimited,
		[PreferredClusterNode] = @PreferredClusterNode,
		[ActiveWaitCollectorStartTimeRelative] = @ActiveWaitCollectorStartTimeRelative,
		[ActiveWaitCollectorRunTimeSeconds] = @ActiveWaitCollectorRunTimeSeconds,
		[ActiveWaitCollectorCollectionTimeSeconds] = @ActiveWaitCollectorCollectionTimeSeconds,
		[ActiveWaitCollectorEnabled] = @ActiveWaitCollectorEnabled,
		[ActiveWaitLoopTimeMilliseconds] = @ActiveWaitLoopTimeMilliseconds,
		[ActiveWaitAdvancedConfiguration] = @ActiveWaitAdvancedConfiguration,
		[ServerPingInterval] = @ServerPingInterval,
		[VHostID] = @VHostID,
		[VmUID] = @VmUID,
		[VmName] = @VmName,
		[VmDomainName] = @VmDomainName,
		[AlertRefreshInMinutes]  = @AlertRefreshInMinutes,
		[DatabaseStatisticsRefreshIntervalInSeconds] = @DatabaseStatisticsRefreshIntervalInSeconds,
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
		[WmiCollectionEnabled] = @WmiCollectionEnabled,
		[WmiConnectAsService] = @WmiConnectAsService,
		[WmiUserName] = @WmiUserName,

		[WmiPassword] = @WmiPassword,
		
		[MaintenanceModeMonth] = @MaintenanceModeMonth,

		[MaintenanceModeSpecificDay] = @MaintenanceModeSpecificDay,

		[MaintenanceModeWeekOrdinal] = @MaintenanceModeWeekOrdinal,

		[MaintenanceModeWeekDay] = @MaintenanceModeWeekDay,

		[MaintenanceModeMonthDuration] = @MaintenanceModeMonthDuration,

		[MaintenanceModeMonthRecurringStart] = @MaintenanceModeMonthRecurringStart,
		[MaintenanceModeOnDemand] = @MaintenanceModeOnDemand,				
		--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Setting the values of newly addded Query Monitor columns
		[QueryMonitorTraceMonitoringEnabled] =@QueryMonitorTraceMonitoringEnabled, 
		[QueryMonitorCollectQueryPlan] =@QueryMonitorCollectQueryPlan,
		--START SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  -- Added new columns and values 
		[QueryMonitorCollectEstimatedQueryPlan] = @QueryMonitorCollectEstimatedQueryPlan,--SQLdm 10.0 (Tarun Sapra) - Sets the flag for getting estimated query plan
			[ActivityMonitorTraceMonitoringEnabled]=@ActivityMonitorTraceMonitoringEnabled,
			[ActivityMonitorXEFileSizeMB]=@ActivityMonitorXEFileSizeMB,
			[ActivityMonitorXEFilesRollover]=@ActivityMonitorXEFilesRollover,
			[ActivityMonitorXERecordsPerRefresh]=@ActivityMonitorXERecordsPerRefresh,
			[ActivityMonitorXEMaxMemoryMB]=@ActivityMonitorXEMaxMemoryMB,
			[ActivityMonitorXEEventRetentionMode]=@ActivityMonitorXEEventRetentionMode,
			[ActivityMonitorXEMaxDispatchLatencySecs]=@ActivityMonitorXEMaxDispatchLatencySecs,
			[ActivityMonitorXEMaxEventSizeMB]=@ActivityMonitorXEMaxEventSizeMB,
			[ActivityMonitorXEMemoryPartitionMode]=@ActivityMonitorXEMemoryPartitionMode,
			[ActivityMonitorXETrackCausality]=@ActivityMonitorXETrackCausality,
			[ActivityMonitorXEStartupState]=@ActivityMonitorXEStartupState,
			[ActivityMonitorXEFileName]=@ActivityMonitorXEFileName,
	--END SQLdm 9.1 (Ankit Srivastava): Activity Monitor with Extended Events  -- Added new columns and values 
	--START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			[QueryMonitorXEFileSizeMB] =@QueryMonitorXEFileSizeMB,
			[QueryMonitorXEFilesRollover]=@QueryMonitorXEFilesRollover,
	--END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new column values for uery monitoring extended event session configuration
			-- SQLdm 10.4 (Varun Chopra) Query Store and Top X Plan
			[QueryMonitorTopPlanCountFilter] = @queryMonitorTopPlanCountFilter,
            [QueryMonitorTopPlanCategoryFilter] = @queryMonitorTopPlanCategoryFilter,
	        [QueryMonitorQueryStoreMonitoringEnabled] = @QueryMonitorQueryStoreMonitoringEnabled,
			
			[ActiveWaitQsEnable] = @ActiveWaitQsEnable,
			[CloudProviderId]=@CloudProviderId
			
	WHERE [SQLServerID] = @SqlServerId 		

	SELECT @error = @@error, @rowcount = @@rowcount
	
	-- insert latest template for this server	
	exec p_AddBaselineTemplate @SqlServerId,@BaselineTemplate
	exec p_UpsertBaselineTemplate @SqlServerId,@baselineConfigList
	IF @error != 0 GOTO HANDLE_ERROR

	-- START 10.0 (srishti purohit) : To handle analysis configuration
	exec p_SaveAnalysisConfiguration @SqlServerId,@productionServer,@OLTP,
	@analysisStartTime ,
	@duration ,@scheduledDays ,@isActive ,
	@includeDatabase ,
	@filterApplication ,
	@categoryID ,
	@databaseID ,
	@recommendationID,
	@schedulingStatus --SQLDM10.0 (praveen suhalka) : scheduling status

	IF @error != 0 --GOTO HANDLE_ERROR
	BEGIN
	RAISERROR('An error occurred while updating instance ANALYSIS %d.', 10, 1, @SqlServerId)
        RETURN(@error)
		END

	-- END 10.0 (srishti purohit) : To handle analysis configuration

	IF @rowcount = 1 BEGIN
		SELECT 
			[SQLServerID], 
			[RegisteredDate],
			[OutOfProcOleAutomation]
		FROM [dbo].[MonitoredSQLServers]
		WHERE [SQLServerID] = @SqlServerId
	END
        
	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while updating instance %d.', 10, 1, @SqlServerId)
        RETURN(@error)
END
 

GO


