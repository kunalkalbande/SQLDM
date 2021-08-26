if (object_id('[p_GetServerSummary]') is not null)
begin
drop procedure [p_GetServerSummary]
end
go
create procedure [dbo].[p_GetServerSummary]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
begin
set transaction isolation level read uncommitted
declare @err int
declare @BeginDateTime datetime
declare @EndDateTime datetime

declare @IsSnapshotSelected bit
SET @IsSnapshotSelected = 0

if (@UTCSnapshotCollectionDateTime is null)
begin
	select @EndDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)
	SET @IsSnapshotSelected = 1
end
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

-- SqlDM 10.2 (Anshul Aggarwal) History Browser - Fetch the latest snapshot in  the given start and end range.
if (@IsSnapshotSelected = 0)
		select @EndDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID 
			AND isnull(RefreshType,0) = 0
			AND (UTCCollectionDateTime between @BeginDateTime AND @EndDateTime))

-- Get state overview data
select 
	[StateOverview] 
from 
	[ServerActivity] 
where 
	[SQLServerID] = @SQLServerID
	and [UTCCollectionDateTime] = @EndDateTime

-- Get trend data

;with cte_waitdata as
(
select
	pvt.UTCCollectionDateTime,
	[I/O],
	[Lock],
	[Memory],
	[Transaction Log],
	[Other],
	[Signal] = sum(WaitTimeInMilliseconds-ResourceWaitTimeInMilliseconds) / nullif(TimeDeltaInSeconds,0)
from
	(
	select
		UTCCollectionDateTime,
		Category = case when Category in ('I/O','Lock','Memory','Transaction Log','Backup','Non-I/O Page Latch','Non-Page Latch') then Category else 'Other' end,
		ResourceWaitTimeMSPerSec = sum(ResourceWaitTimeInMilliseconds) / nullif(TimeDeltaInSeconds,0)
	from
		WaitStatisticsDetails wsd
		inner join WaitStatistics ws
		on wsd.WaitStatisticsID = ws.WaitStatisticsID
		inner join WaitTypes wt
		on wsd.WaitTypeID = wt.WaitTypeID
		inner join WaitCategories wc
		on wt.CategoryID = wc.CategoryID
	where
		wc.ExcludeFromCollection = 0 and
		[SQLServerID] = @SQLServerID and
		[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
	group by
		case when Category in ('I/O','Lock','Memory','Transaction Log','Backup','Non-I/O Page Latch','Non-Page Latch') then Category else 'Other' end,
		UTCCollectionDateTime,
		TimeDeltaInSeconds
	) as PivotSource
pivot
	(
	sum(ResourceWaitTimeMSPerSec)
	for Category 
	in([I/O],[Lock],[Memory],[Transaction Log],[Other],[Backup],[Non-I/O Page Latch],[Non-Page Latch])
	) as pvt
inner join WaitStatistics ws2
on pvt.UTCCollectionDateTime = ws2.UTCCollectionDateTime
inner join WaitStatisticsDetails wsd2
on ws2.WaitStatisticsID = wsd2.WaitStatisticsID
where ws2.SQLServerID = @SQLServerID	-- SqlDM 10.2 (Anshul Aggarwal) SQLDM-27658 - SP performance improvement
group by
	ws2.UTCCollectionDateTime, 
	ws2.TimeDeltaInSeconds,
	pvt.UTCCollectionDateTime,
	[I/O],
	[Lock],
	[Memory],
	[Transaction Log],
	[Other]
), cte_TempdbFileData (UTCCollectionDateTime, UserObjectsInKilobytes, InternalObjectsInKilobytes,VersionStoreInKilobytes,MixedExtentsInKilobytes,UnallocatedSpaceInKilobytes)as
(
select
TempdbFileData.UTCCollectionDateTime,
sum(UserObjectsInKilobytes),
sum(InternalObjectsInKilobytes),
sum(VersionStoreInKilobytes),
sum(MixedExtentsInKilobytes),
sum(UnallocatedSpaceInKilobytes)
from TempdbFileData	
inner join DatabaseFiles on TempdbFileData.FileID = DatabaseFiles.FileID
inner join SQLServerDatabaseNames on SQLServerDatabaseNames.DatabaseID = DatabaseFiles.DatabaseID
where
	SQLServerID = @SQLServerID and
	TempdbFileData.UTCCollectionDateTime between @BeginDateTime and @EndDateTime
	and SQLServerDatabaseNames.IsDeleted = 0 -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 	
group by
UTCCollectionDateTime
)
select
	[InstanceName]
	,[ServerStatistics].[UTCCollectionDateTime] as [CollectionDateTime]
	,[ActiveProcesses]
	,[AgentServiceStatus]
	,[DataIOUsage]
	,[LogIOUsage]
	,[DataIORate]
	,[LogIORate]
	,[BlockedProcesses]
	,[BufferCacheHitRatioPercentage]
	,[BufferCacheSizeInKilobytes]
	,[CheckpointWrites] = isnull([CheckpointWrites],0)
	,[ClientComputers]
	,[CommittedInKilobytes]
	,[ConnectionMemoryInKilobytes]
	,[CPUActivityPercentage]
	,[CPUTimeDelta]
	,[CPUTimeRaw]
	,[DatabaseCount]
	,[DataFileCount]
	,[DataFileSpaceAllocatedInKilobytes]
	,[DataFileSpaceUsedInKilobytes]
	,[DiskTimePercent]
	,[DiskQueueLength]
	,[DistributionLatencyInSeconds]
	,[DTCServiceStatus]
	,[FreeCachePagesInKilobytes]
	,[CachePagesInKilobytes]
	,[FreePagesInKilobytes]
	,[FullScans]
	,[FullTextSearchStatus]
	,[GrantedWorkspaceMemoryInKilobytes]
	,[IdleTimeDelta]
	,[IdleTimePercentage]
	,[IdleTimeRaw]
	,[IOActivityPercentage]
	,[IOTimeDelta]
	,[IOTimeRaw]
	,[IsClustered]
	,[ClusterNodeName]
	,[LazyWriterWrites] = isnull([LazyWriterWrites],0)
	,[LeadBlockers]
	,[LockMemoryInKilobytes]
	,[LockStatistics]
	,[LockWaits]
	,[Logins]
	,[LogFileCount]
	,[LogFileSpaceAllocatedInKilobytes]
	,[LogFileSpaceUsedInKilobytes]
	,[LogFlushes]
	,[MaxConnections]
	,[OldestOpenTransactionsInMinutes]
	,[OpenTransactions]
	,[OptimizerMemoryInKilobytes]
	,[OSAvailableMemoryInKilobytes]
	,[OsStatisticAvailability]
	,[OSTotalPhysicalMemoryInKilobytes]
	,[PacketErrors]
	,[PacketsReceived]
	,[PacketsSent]
	,[PageErrors]
	,[PageLifeExpectancy]
	,[PageLookups]
	,[PageReads] = isnull([PageReads],0)
	,[PagesPerSecond]
	,[PageSplits]
	,[PageWrites] = isnull([PageWrites],0)
	,[PrivilegedTimePercent]
	,[ProcedureCacheHitRatioPercentage]
	,[ProcedureCacheSizeInKilobytes]
	,[ProcedureCacheSizePercent]
	,[ProcessorCount]
	,[ProcessorsUsed]
	,[ProcessorTimePercent]
	,[ProcessorQueueLength]
	,[ReadAheadPages] = isnull([ReadAheadPages],0)
	,[ServerHostName]
	,[ServerStatistics].[RealServerName]
	,[ReplicationLatencyInSeconds]
	,[ReplicationSubscribed]
	,[ReplicationUndistributed]
	,[ReplicationUnsubscribed]
	,[ResponseTimeInMilliseconds]
	,[RunningSince]
	,[ServerStatistics].[ServerVersion]
	,[SqlCompilations]
	,[SqlMemoryAllocatedInKilobytes]
	,[SqlMemoryUsedInKilobytes]
	,[SqlRecompilations]
	,[SqlServerEdition]
	,[SqlServerServiceStatus]
	,[ServerStatistics].[SystemProcesses]
	,[SystemProcessesConsumingCPU]
	,[TableLockEscalations]
	,[TempDBSizeInKilobytes]
	,[TempDBSizePercent]
	,[TimeDeltaInSeconds]
	,[TotalLocks]
	,[Transactions]
	,[Batches]
	,[UserProcesses]
	,[UserProcessesConsumingCPU]
	,[UserTimePercent]
	,[WindowsVersion]
	,[WorkFilesCreated]
	,[WorkTablesCreated]
	,[I/O]
	,[Lock]
	,[Memory]
	,[Transaction Log]
	,[Other]
	,[Signal]
	,[VMConfigData].UUID as [vmUUID]
	,[VMConfigData].VMHeartBeat as [vmHeartBeat]
	,[VMConfigData].VMName as [vmName] 
	,[VMConfigData].BootTime as [vmBootTime]
	,[VMConfigData].CPULimit as [vmCPULimit] 
	,[VMConfigData].CPUReserve as [vmCPUReserve]
	,[VMConfigData].DomainName as [vmDomainName]
	,[VMConfigData].MemLimit as [vmMemLimit]
	,[VMConfigData].MemReserve as [vmMemReserve]
	,[VMConfigData].MemSize as [vmMemSize]
	,[VMConfigData].NumCPUs as [vmNumCPUs]
	,[VMStatistics].CPUReady as [vmCPUReady]
	,[VMStatistics].CPUSwapWait as [vmCPUSwapWait]
	,[VMStatistics].CPUUsage as [vmCPUUsage]
	,[VMStatistics].CPUUsageMHz as [vmCPUUsageMHz]
	,[VMStatistics].DiskRead as [vmDiskRead]
	,[VMStatistics].DiskUsage as [vmDiskUsage]
	,[VMStatistics].DiskWrite as [vmDiskWrite]
	,[VMStatistics].MemActive as [vmMemActive]
	,[VMStatistics].MemBalooned as [vmMemBallooned]
	,[VMStatistics].MemConsumed as [vmMemConsumed]
	,[VMStatistics].MemGranted as [vmMemGranted]
	,[VMStatistics].MemSwapInRate as [vmMemSwapInRate]
	,[VMStatistics].MemSwapOutRate as [vmMemSwapOutRate]
	,[VMStatistics].MemSwapped as [vmMemSwapped]
	,[VMStatistics].MemUsage as [vmMemUsage]
	,[VMStatistics].NetReceived as [vmNetReceived]
	,[VMStatistics].NetTransmitted as [vmNetTransmitted]
	,[VMStatistics].NetUsage as [vmNetUsage]
	,[VMStatistics].PagePerSecVM as [PagePerSecVM]
	,[VMStatistics].AvailableByteVm as [AvailableByteVm]
	,[ESXConfigData].HostName as [esxHostName]
	,[ESXConfigData].Status as [esxStatus]
	,[ESXConfigData].BootTime as [esxBootTime]
	,[ESXConfigData].CPUMHz as [esxCPUMHz]
	,[ESXConfigData].DomainName as [esxDomainName]
	,[ESXConfigData].MemorySize as [esxMemSize]
	,[ESXConfigData].NumCPUCores as [esxNumCPUCores]
	,[ESXConfigData].NumCPUPkgs as [esxNumCPUPkgs]
	,[ESXConfigData].NumCPUThreads as [esxNumCPUThreads]
	,[ESXConfigData].NumNICs as [esxNumNICs]
	,[ESXStatistics].CPUUsage as [esxCPUUsage]
	,[ESXStatistics].CPUUsageMHz as [esxCPUUsageMHz]
	,[ESXStatistics].DiskDeviceLatency as [esxDeviceLatency]
	,[ESXStatistics].DiskKernelLatency as [esxKernelLatency]
	,[ESXStatistics].DiskQueueLatency as [esxQueueLatency]
	,[ESXStatistics].DiskRead as [esxDiskRead]
	,[ESXStatistics].DiskTotalLatency as [esxTotalLatency]
	,[ESXStatistics].DiskUsage as [esxDiskUsage]
	,[ESXStatistics].DiskWrite as [esxDiskWrite]
	,[ESXStatistics].MemActive as [esxMemActive]
	,[ESXStatistics].MemBalooned as [esxMemBallooned]
	,[ESXStatistics].MemConsumed as [esxMemConsumed]
	,[ESXStatistics].MemGranted as [esxMemGranted]
	,[ESXStatistics].MemSwapInRate as [esxMemSwapInRate]
	,[ESXStatistics].MemSwapOutRate as [esxMemSwapOutRate]
	,[ESXStatistics].MemUsage as [esxMemUsage]
	,[ESXStatistics].NetReceived as [esxNetReceived]
	,[ESXStatistics].NetTransmitted as [esxNetTransmitted]
	,[ESXStatistics].NetUsage as [esxNetUsage]
	,[ESXStatistics].MemPagePerSec as [pagePerSecHost]
	,[ESXStatistics].AvailableMemBytes as [availableByteHost]
	,TempdbUserObjectsInMegabytes = [cte_TempdbFileData].UserObjectsInKilobytes / 1024.0
	,TempdbInternalObjectsInMegabytes = [cte_TempdbFileData].InternalObjectsInKilobytes / 1024.0
	,TempdbVersionStoreInMegabytes = [cte_TempdbFileData].VersionStoreInKilobytes / 1024.0
	,TempdbMixedExtentsInMegabytes = [cte_TempdbFileData].MixedExtentsInKilobytes / 1024.0
	,TempdbUnallocatedSpaceInMegabytes = [cte_TempdbFileData].UnallocatedSpaceInKilobytes / 1024.0
	,TempdbPFSWaitTimeMilliseconds
	,TempdbGAMWaitTimeMilliseconds
	,TempdbSGAMWaitTimeMilliseconds
	,VersionStoreGenerationKilobytesPerSec = VersionStoreGenerationKilobytes / nullif(TimeDeltaInSeconds,0)
	,VersionStoreCleanupKilobytesPerSec = VersionStoreCleanupKilobytes  / nullif(TimeDeltaInSeconds,0)
	,[ManagedInstanceStorageLimit]
	,CPUCreditBalance
    ,CPUCreditUsage
    ,DiskQueueDepth
    ,ReadLatency
    ,ReadThroughput
    ,SwapUsage
    ,WriteLatency
    ,WriteThroughput
from
	[ServerStatistics]
	left join [MonitoredSQLServers]
	on [ServerStatistics].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
	left join [OSStatistics]
	on [ServerStatistics].[SQLServerID] = [OSStatistics].[SQLServerID]
	and [ServerStatistics].[UTCCollectionDateTime] = [OSStatistics].[UTCCollectionDateTime]
	left join [ServerActivity]
	on [ServerStatistics].[SQLServerID] = [ServerActivity].[SQLServerID]		-- SqlDM 10.2 (Anshul Aggarwal) SQLDM-27658 - SP is slow.
	and [ServerStatistics].[UTCCollectionDateTime] = [ServerActivity].[UTCCollectionDateTime]
	left join cte_waitdata
	on [ServerStatistics].[UTCCollectionDateTime] = cte_waitdata.[UTCCollectionDateTime]
	left join [VMConfigData]
		on [ServerStatistics].[SQLServerID] = [VMConfigData].[SQLServerID] 
		and [ServerStatistics].[UTCCollectionDateTime] = [VMConfigData].[UTCCollectionDateTime] 
	left join [VMStatistics] 
		on [ServerStatistics].SQLServerID = [VMStatistics].SQLServerID 
		and [ServerStatistics].UTCCollectionDateTime = [VMStatistics].UTCCollectionDateTime 
	left join [ESXConfigData]
		on [ServerStatistics].SQLServerID = [ESXConfigData].SQLServerID 
		and [ServerStatistics].UTCCollectionDateTime = [ESXConfigData].UTCCollectionDateTime 
	left join [ESXStatistics] 
		on [ServerStatistics].SQLServerID = [ESXStatistics].SQLServerID 
		and [ServerStatistics].UTCCollectionDateTime = [ESXStatistics].UTCCollectionDateTime 
	left join cte_TempdbFileData
		on [ServerStatistics].[UTCCollectionDateTime] = cte_TempdbFileData.UTCCollectionDateTime
where
	[ServerStatistics].[SQLServerID] = @SQLServerID
	and [ServerStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
order by 
	[ServerStatistics].[UTCCollectionDateTime]

-- Get distinct disk drives
select distinct [DriveName] from [DiskDrives] where [SQLServerID] = @SQLServerID

-- Get per disk metrics
select
	[UTCCollectionDateTime] as [CollectionDateTime],
	[DriveName],
	[UnusedSizeKB],
	[TotalSizeKB],
	[DiskIdlePercent],
	[AverageDiskQueueLength],
	[AverageDiskMillisecondsPerRead],
	[AverageDiskMillisecondsPerTransfer],
	[AverageDiskMillisecondsPerWrite],
	[DiskReadsPerSecond],
	[DiskTransfersPerSecond],
	[DiskWritesPerSecond],
	(100 - [DiskIdlePercent]) as [DiskBusyPercent]
from 
	[DiskDrives]
where
	[SQLServerID] = @SQLServerID and
	[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
order by 
	[UTCCollectionDateTime]


select @err = @@error
return @err
end
