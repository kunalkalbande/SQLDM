if (object_id('p_InsertServerStatistics') is not null)
begin
drop procedure p_InsertServerStatistics
end
go
create procedure p_InsertServerStatistics
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@TimeDeltaInSeconds float,
	@AgentServiceStatus int,
	@DataIOUsage float,
	@LogIOUsage float,
	@DataIORate float,
	@LogIORate float,
	@SqlServerServiceStatus int,
	@DTCServiceStatus int,
	@FullTextSearchStatus int,
	@BufferCacheHitRatioPercentage float,
	@CheckpointWrites bigint,
	@ClientComputers int,
	@CPUActivityPercentage float,
	@CPUTimeDelta bigint,
	@CPUTimeRaw bigint,
	@DistributionLatencyInSeconds float,
	@FullScans bigint,
	@IdleTimeDelta bigint,
	@IdleTimePercentage float,
	@IdleTimeRaw bigint,
	@IOActivityPercentage float,
	@IOTimeDelta bigint,
	@IOTimeRaw bigint,
	@LazyWriterWrites bigint,
	@LockWaits bigint,
	@Logins bigint,
	@LogFlushes bigint,
	@SqlMemoryAllocatedInKilobytes bigint,
	@SqlMemoryUsedInKilobytes bigint,
	@OldestOpenTransactionsInMinutes bigint,
	@PacketErrors bigint,
	@PacketsReceived bigint,
	@PacketsSent bigint,
	@PageErrors bigint,
	@PageLifeExpectancy bigint,
	@PageLookups bigint,
	@PageReads bigint,
	@PageSplits bigint,
	@PageWrites bigint,
	@ProcedureCacheHitRatioPercentage float,
	@ProcedureCacheSizeInKilobytes bigint,
	@ProcedureCacheSizePercent float,
	@ReadAheadPages bigint,
	@ReplicationLatencyInSeconds float,
	@ReplicationSubscribed bigint,
	@ReplicationUndistributed bigint,
	@ReplicationUnsubscribed bigint,
	@ResponseTimeInMilliseconds int,
	@ServerVersion nvarchar(30),
	@ServerEdition nvarchar(30),
	@SqlCompilations bigint,
	@SqlRecompilations bigint,
	@TableLockEscalations bigint,
	@TempDBSizeInKilobytes bigint,
	@TempDBSizePercent float,
	@Batches bigint,
	@UserProcesses int,
	@WorkFilesCreated bigint,
	@WorkTablesCreated bigint,
	@SystemProcesses int,
	@UserProcessesConsumingCPU int, 
	@SystemProcessesConsumingCPU int, 
	@BlockedProcesses int, 
	@OpenTransactions int, 
	@DatabaseCount int,
	@DataFileCount int,
	@LogFileCount int,
	@DataFileSpaceAllocatedInKilobytes decimal,
	@DataFileSpaceUsedInKilobytes decimal,
	@LogFileSpaceAllocatedInKilobytes decimal,
	@LogFileSpaceUsedInKilobytes decimal,
	@TotalLocks decimal,
	@BufferCacheSizeInKilobytes bigint,
	@ActiveProcesses int,
	@LeadBlockers int,
	@CommittedInKilobytes bigint,
	@ConnectionMemoryInKilobytes bigint,
	@FreePagesInKilobytes bigint,
	@GrantedWorkspaceMemoryInKilobytes bigint,
	@LockMemoryInKilobytes bigint,
	@OptimizerMemoryInKilobytes bigint,
	@TotalServerMemoryInKilobytes bigint,
	@FreeCachePagesInKilobytes bigint,
	@CachePagesInKilobytes bigint,
	@MaxConnections bigint,
	@PhysicalMemoryInKilobytes bigint,
	@ProcessorCount int,
	@ProcessorsUsed int,
	@ProcessorType nvarchar(20),
	@ServerHostName nvarchar(50),
	@RealServerName nvarchar(100),
	@WindowsVersion nvarchar(30),
	@SqlServerEdition nvarchar(30),  
	@RunningSince datetime,
	@IsClustered bit,
	@OsStatisticAvailability nvarchar(50),
	@ClusterNodeName nvarchar(256),
	@VersionStoreGenerationKilobytes bigint,
	@VersionStoreCleanupKilobytes bigint,
	@TempdbPFSWaitTimeMilliseconds bigint,
	@TempdbGAMWaitTimeMilliseconds bigint,
	@TempdbSGAMWaitTimeMilliseconds bigint,
	@Transactions bigint,
	@SQLBrowserServiceStatus int,   --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Browser service column
	@SQLActiveDirectoryHelperServiceStatus int,   --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Active Directory Helper service column
	@ManagedInstanceStorageLimit decimal,
	@ReadThroughput float , --SQLdm 11.1 5.4.1
	@WriteThroughput float ,--SQLdm 11.1 5.4.1
	@SwapUsage float ,--SQLdm 11.1 5.4.1
	@ReadLatency float ,--SQLdm 11.1 5.4.1
	@WriteLatency float ,--SQLdm 11.1 5.4.1
	@CPUCreditBalance float ,--SQLdm 11.1 5.4.1
	@CPUCreditUsage float ,--SQLdm 11.1 5.4.1
	@DiskQueueDepth float ,--SQLdm 11.1 5.4.1
	@ReturnMessage nvarchar(128) output
as
begin
	insert into [ServerStatistics]
			([SQLServerID]
			,[UTCCollectionDateTime]
			,[TimeDeltaInSeconds]
			,[AgentServiceStatus]
			,[DataIOUsage]
			,[LogIOUsage]
			,[DataIORate]
			,[LogIORate]
			,[SqlServerServiceStatus]
			,[DTCServiceStatus]
			,[FullTextSearchStatus]
			,[BufferCacheHitRatioPercentage]
			,[CheckpointWrites]
			,[ClientComputers]
			,[CPUActivityPercentage]
			,[CPUTimeDelta]
			,[CPUTimeRaw]
			,[DistributionLatencyInSeconds]
			,[FullScans]
			,[IdleTimeDelta]
			,[IdleTimePercentage]
			,[IdleTimeRaw]
			,[IOActivityPercentage]
			,[IOTimeDelta]
			,[IOTimeRaw]
			,[LazyWriterWrites]
			,[LockWaits]
			,[Logins]
			,[LogFlushes]
			,[SqlMemoryAllocatedInKilobytes]
			,[SqlMemoryUsedInKilobytes]
			,[OldestOpenTransactionsInMinutes]
			,[PacketErrors]
			,[PacketsReceived]
			,[PacketsSent]
			,[PageErrors]
			,[PageLifeExpectancy]
			,[PageLookups]
			,[PageReads]
			,[PageSplits]
			,[PageWrites]
			,[ProcedureCacheHitRatioPercentage]
			,[ProcedureCacheSizeInKilobytes]
			,[ProcedureCacheSizePercent]
			,[ReadAheadPages]
			,[ReplicationLatencyInSeconds]
			,[ReplicationSubscribed]
			,[ReplicationUndistributed]
			,[ReplicationUnsubscribed]
			,[ResponseTimeInMilliseconds]
			,[ServerVersion]
			,[SqlCompilations]
			,[SqlRecompilations]
			,[TableLockEscalations]
			,[TempDBSizeInKilobytes]
			,[TempDBSizePercent]
			,[Batches]
			,[UserProcesses]
			,[WorkFilesCreated]
			,[WorkTablesCreated]
			,[SystemProcesses]
			,[UserProcessesConsumingCPU]
			,[SystemProcessesConsumingCPU]
			,[BlockedProcesses]
			,[OpenTransactions]
			,[DatabaseCount]
			,[DataFileCount]
			,[LogFileCount]
			,[DataFileSpaceAllocatedInKilobytes]
			,[DataFileSpaceUsedInKilobytes]
			,[LogFileSpaceAllocatedInKilobytes]
			,[LogFileSpaceUsedInKilobytes]
			,[TotalLocks]
			,[BufferCacheSizeInKilobytes]
			,[ActiveProcesses]
			,[LeadBlockers]
			,[CommittedInKilobytes]
			,[ConnectionMemoryInKilobytes]
			,[FreePagesInKilobytes]
			,[GrantedWorkspaceMemoryInKilobytes]
			,[LockMemoryInKilobytes]
			,[OptimizerMemoryInKilobytes]
			,[TotalServerMemoryInKilobytes]
			,[FreeCachePagesInKilobytes]
			,[CachePagesInKilobytes]
			,[MaxConnections]
			,[PhysicalMemoryInKilobytes]
			,[ProcessorCount]
			,[ProcessorsUsed]
			,[ProcessorType]
			,[ServerHostName]
			,[RealServerName]
			,[WindowsVersion]
			,[SqlServerEdition]
			,[RunningSince]
			,[IsClustered]
			,[OsStatisticAvailability]
			,[ClusterNodeName]
			,[VersionStoreGenerationKilobytes]
			,[VersionStoreCleanupKilobytes]
			,[TempdbPFSWaitTimeMilliseconds]
			,[TempdbGAMWaitTimeMilliseconds]
			,[TempdbSGAMWaitTimeMilliseconds]
			,[Transactions]
			,[SQLBrowserServiceStatus]      --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Browser service column
			,[SQLActiveDirectoryHelperServiceStatus]   --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Active Directory Helper service column
			,[ManagedInstanceStorageLimit]
			,[ReadThroughput] 
			,[WriteThroughput] 
			,[SwapUsage] 
			,[ReadLatency] 
			,[WriteLatency] 
			,[CPUCreditBalance]
			,[CPUCreditUsage] 
			,[DiskQueueDepth]
			)
     values
           (@SQLServerID
			,@UTCCollectionDateTime
			,@TimeDeltaInSeconds
			,@AgentServiceStatus
			,@DataIOUsage
			,@LogIOUsage
			,@DataIORate
			,@LogIORate
			,@SqlServerServiceStatus
			,@DTCServiceStatus
			,@FullTextSearchStatus
			,@BufferCacheHitRatioPercentage
			,@CheckpointWrites
			,@ClientComputers
			,@CPUActivityPercentage
			,@CPUTimeDelta
			,@CPUTimeRaw
			,@DistributionLatencyInSeconds
			,@FullScans
			,@IdleTimeDelta
			,@IdleTimePercentage
			,@IdleTimeRaw
			,@IOActivityPercentage
			,@IOTimeDelta
			,@IOTimeRaw
			,@LazyWriterWrites
			,@LockWaits
			,@Logins
			,@LogFlushes
			,@SqlMemoryAllocatedInKilobytes
			,@SqlMemoryUsedInKilobytes
			,@OldestOpenTransactionsInMinutes
			,@PacketErrors
			,@PacketsReceived
			,@PacketsSent
			,@PageErrors
			,@PageLifeExpectancy
			,@PageLookups
			,@PageReads
			,@PageSplits
			,@PageWrites
			,@ProcedureCacheHitRatioPercentage
			,@ProcedureCacheSizeInKilobytes
			,@ProcedureCacheSizePercent
			,@ReadAheadPages
			,@ReplicationLatencyInSeconds
			,@ReplicationSubscribed
			,@ReplicationUndistributed
			,@ReplicationUnsubscribed
			,@ResponseTimeInMilliseconds
			,@ServerVersion
			,@SqlCompilations
			,@SqlRecompilations
			,@TableLockEscalations
			,@TempDBSizeInKilobytes
			,@TempDBSizePercent
			,@Batches
			,@UserProcesses
			,@WorkFilesCreated
			,@WorkTablesCreated
			,@SystemProcesses
			,@UserProcessesConsumingCPU  
			,@SystemProcessesConsumingCPU  
			,@BlockedProcesses 
			,@OpenTransactions			
			,@DatabaseCount
			,@DataFileCount
			,@LogFileCount
			,@DataFileSpaceAllocatedInKilobytes
			,@DataFileSpaceUsedInKilobytes
			,@LogFileSpaceAllocatedInKilobytes
			,@LogFileSpaceUsedInKilobytes
			,@TotalLocks
			,@BufferCacheSizeInKilobytes
			,@ActiveProcesses
			,@LeadBlockers
			,@CommittedInKilobytes
			,@ConnectionMemoryInKilobytes
			,@FreePagesInKilobytes
			,@GrantedWorkspaceMemoryInKilobytes
			,@LockMemoryInKilobytes
			,@OptimizerMemoryInKilobytes
			,@TotalServerMemoryInKilobytes
			,@FreeCachePagesInKilobytes
			,@CachePagesInKilobytes
			,@MaxConnections
			,@PhysicalMemoryInKilobytes
			,@ProcessorCount
			,@ProcessorsUsed
			,@ProcessorType
			,@ServerHostName
			,@RealServerName
			,@WindowsVersion
			,@SqlServerEdition
			,@RunningSince
			,@IsClustered
			,@OsStatisticAvailability
			,@ClusterNodeName
			,@VersionStoreGenerationKilobytes
			,@VersionStoreCleanupKilobytes
			,@TempdbPFSWaitTimeMilliseconds 
			,@TempdbGAMWaitTimeMilliseconds
			,@TempdbSGAMWaitTimeMilliseconds
			,@Transactions
			,@SQLBrowserServiceStatus        --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Browser service column
			,@SQLActiveDirectoryHelperServiceStatus   --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the SQL Active Directory Helper service column
			,@ManagedInstanceStorageLimit
			,@ReadThroughput
			,@WriteThroughput
			,@SwapUsage
			,@ReadLatency 
			,@WriteLatency 
			,@CPUCreditBalance
			,@CPUCreditUsage
			,@DiskQueueDepth
)
exec p_UpdateServerVersion @SQLServerID, @ServerVersion, @ServerEdition

update [MonitoredSQLServers] 
	set [ActiveClusterNode] = @ClusterNodeName,
		[RealServerName] = @RealServerName
	where [SQLServerID] = @SQLServerID

-- exec p_UpdateLastRefreshTime @SQLServerID

select @ReturnMessage = @@error

end
