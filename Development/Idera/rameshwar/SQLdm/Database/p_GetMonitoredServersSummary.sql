IF (object_id('p_GetMonitoredServersSummary') is not null)
BEGIN
	DROP PROCEDURE [p_GetMonitoredServersSummary]
END
GO

--EXEC [p_GetMonitoredServersSummary]

create procedure [dbo].[p_GetMonitoredServersSummary]
	@SQLServerID int = null,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
BEGIN
set transaction isolation level read uncommitted
declare @err int
declare @BeginDateTime datetime
declare @EndDateTime datetime

declare @HealthIndexCoefficientForCriticalAlert bigint
declare @HealthIndexCoefficientForInformationalAlert bigint
declare @HealthIndexCoefficientForWarningAlert bigint

-- SQLdm 10.2 (Anshul Aggarwal) SQLDM-27331 - Grid Overview : 'Health index/Alert' columns state is shown incorrectly.
declare @now datetime
Select @now= DateAdd(second,10,GetUTCDate())

declare @InstancesByMaxCollectionTime TABLE (
InstanceID int, 
UTCCollectionDateTime DateTime)


declare @ServerActivityByMaxCollectionTime TABLE (  
InstanceID int,   
UTCCollectionDateTime DateTime)  
  
INSERT INTO @InstancesByMaxCollectionTime  
SELECT mss.SQLServerID, MAX(ss.UTCCollectionDateTime) FROM MonitoredSQLServers mss (NOLOCK)  
LEFT OUTER JOIN [ServerStatistics] ss (NOLOCK) ON mss.SQLServerID = ss.SQLServerID  
WHERE mss.Active = 1 AND (@SQLServerID IS NULL OR mss.SQLServerID = @SQLServerID)   
GROUP BY mss.SQLServerID  
  
--START SQLdm 9.1 (Sanjali Makkar): Gets Health Index coefficients for specific alerts   
INSERT INTO @ServerActivityByMaxCollectionTime  
SELECT mss.SQLServerID, MAX(sa.UTCCollectionDateTime) UTCCollectionDateTime FROM MonitoredSQLServers mss (NOLOCK)  
LEFT OUTER JOIN ServerActivity sa (NOLOCK) ON mss.SQLServerID = sa.SQLServerID  
WHERE mss.Active = 1 AND (@SQLServerID IS NULL OR mss.SQLServerID = @SQLServerID) AND sa.StateOverview IS NOT NULL
GROUP BY mss.SQLServerID  

--START: SQLdm 10.1 (srisht purohit) -New table to make health index global for all instance
SELECT @HealthIndexCoefficientForCriticalAlert = HealthIndexCoefficientValue FROM HealthIndexCofficients WHERE ID = 1
SELECT @HealthIndexCoefficientForWarningAlert = HealthIndexCoefficientValue FROM HealthIndexCofficients WHERE ID = 2
SELECT @HealthIndexCoefficientForInformationalAlert = HealthIndexCoefficientValue FROM HealthIndexCofficients WHERE ID = 3
--END: SQLdm 10.1 (srisht purohit) -New table to make health index global for all instance

-- Get state overview data  
SELECT ibmct.InstanceID, sa.[StateOverview], @HealthIndexCoefficientForCriticalAlert AS HealthIndexCoefficientForCriticalAlert,  
@HealthIndexCoefficientForWarningAlert AS HealthIndexCoefficientForWarningAlert, 
@HealthIndexCoefficientForInformationalAlert AS HealthIndexCoefficientForInformationalAlert, 
mss.FriendlyServerName as FriendlyServerName,--SQLdm 10.1 (Pulkit Puri) -For adding friendly server name
mss.InstanceScaleFactor AS InstanceScaleFactor,
(
SELECT AVG(TagScaleFactor) FROM Tags t
JOIN ServerTags st ON st.TagId = t.Id WHERE st.SQLServerId = ibmct.InstanceID
) AS AvgTagsScaleFactor --SQLdm10.1 (Srishti Purohit)  
from ServerActivity sa JOIN @ServerActivityByMaxCollectionTime samax ON sa.UTCCollectionDateTime = samax.UTCCollectionDateTime and sa.SQLServerID = samax.InstanceID
INNER JOIN @InstancesByMaxCollectionTime ibmct on ibmct.InstanceID = sa.SQLServerID
INNER JOIN MonitoredSQLServers mss  
ON mss.SQLServerID = ibmct.InstanceID  
--END SQLdm 9.1 (Sanjali Makkar): Gets Health Index coefficients for specific alerts   
-- Get trend data
;with cte_waitdata as
(
select
	pvt.InstanceID,
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
		ibmct.InstanceID,
		ibmct.UTCCollectionDateTime,
		Category = case when Category in ('I/O','Lock','Memory','Transaction Log') then Category else 'Other' end,
		ResourceWaitTimeMSPerSec = sum(ResourceWaitTimeInMilliseconds) / nullif(TimeDeltaInSeconds,0)
	from
		WaitStatisticsDetails wsd (NOLOCK)
		inner join WaitTypes wt (NOLOCK) on wt.WaitTypeID = wsd.WaitTypeID
		inner join WaitStatistics ws (NOLOCK) on ws.WaitStatisticsID = wsd.WaitStatisticsID
		INNER JOIN @InstancesByMaxCollectionTime ibmct on ibmct.InstanceID = ws.SQLServerID
		inner join WaitCategories wc (NOLOCK) on wt.CategoryID = wc.CategoryID
	where
		wc.ExcludeFromCollection = 0 and
		ws.[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	group by
		case when Category in ('I/O','Lock','Memory','Transaction Log') then Category else 'Other' end,
		ibmct.InstanceID,
		ibmct.UTCCollectionDateTime,
		TimeDeltaInSeconds
	) as PivotSource
pivot
	(
	sum(ResourceWaitTimeMSPerSec)
	for Category 
	in([I/O],[Lock],[Memory],[Transaction Log],[Other])
	) as pvt
inner join WaitStatistics ws2 (NOLOCK)
on pvt.UTCCollectionDateTime = ws2.UTCCollectionDateTime
	and pvt.InstanceID = ws2.SQLServerID
inner join WaitStatisticsDetails wsd2 (NOLOCK)
on ws2.WaitStatisticsID = wsd2.WaitStatisticsID
group by
	ws2.UTCCollectionDateTime, 
	ws2.TimeDeltaInSeconds,
	pvt.InstanceID,
	pvt.UTCCollectionDateTime,
	[I/O],
	[Lock],
	[Memory],
	[Transaction Log],
	[Other]
), cte_TempdbFileData (InstanceID, UTCCollectionDateTime, UserObjectsInKilobytes, InternalObjectsInKilobytes,VersionStoreInKilobytes,MixedExtentsInKilobytes,UnallocatedSpaceInKilobytes)as
(
select
ibmct.InstanceID,
ibmct.UTCCollectionDateTime,
sum(UserObjectsInKilobytes),
sum(InternalObjectsInKilobytes),
sum(VersionStoreInKilobytes),
sum(MixedExtentsInKilobytes),
sum(UnallocatedSpaceInKilobytes)
from @InstancesByMaxCollectionTime ibmct 
inner join SQLServerDatabaseNames (NOLOCK) on ibmct.InstanceID = SQLServerDatabaseNames.SQLServerID
inner join DatabaseFiles (NOLOCK) on SQLServerDatabaseNames.DatabaseID = DatabaseFiles.DatabaseID
inner join TempdbFileData(NOLOCK) on TempdbFileData.FileID = DatabaseFiles.FileID
where
	TempdbFileData.UTCCollectionDateTime = ibmct.UTCCollectionDateTime
	and SQLServerDatabaseNames.IsDeleted=0 -- SQLdm Kit1 Barkha khatri
group by
ibmct.InstanceID,
ibmct.UTCCollectionDateTime
)
select
	MonitoredSQLServers.SQLServerID
	,[InstanceName]
	,[ServerStatistics].[UTCCollectionDateTime] as [CollectionDateTime]
	,[ActiveProcesses]
	,[AgentServiceStatus]
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
	,[MonitoredSQLServers].MaintenanceModeEnabled
	--[START] SQLdm 9.1 (Gaurav Karwal): Added after new sql services were monitored
	,ISNULL(ServerStatistics.SQLBrowserServiceStatus,9) SQLBrowserServiceStatus
	,ISNULL(ServerStatistics.SQLActiveDirectoryHelperServiceStatus,9) SQLActiveDirectoryHelperServiceStatus
	--[END]  SQLdm 9.1 (Gaurav Karwal): Added after new sql services were monitored
	,[MonitoredSQLServers].FriendlyServerName-- SQLdm 10.1 (Pulkit Puri) To add friendly name in the overview part
from
	@InstancesByMaxCollectionTime ibmct 
	INNER JOIN [MonitoredSQLServers] (NOLOCK) on ibmct.InstanceID = [MonitoredSQLServers].SQLServerID
	left join [ServerStatistics] (NOLOCK)
		on [ServerStatistics].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
		and [ServerStatistics].[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	left join [OSStatistics] (NOLOCK)
		on [OSStatistics].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
		and [OSStatistics].[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	left join [ServerActivity] (NOLOCK)
		on [ServerActivity].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
		and [ServerActivity].[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	left join cte_waitdata
		on cte_waitdata.InstanceID = MonitoredSQLServers.SQLServerID
		and cte_waitdata.[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	left join [VMConfigData] (NOLOCK)
		on [VMConfigData].[SQLServerID] = MonitoredSQLServers.[SQLServerID] 
		and [VMConfigData].[UTCCollectionDateTime] = ibmct.UTCCollectionDateTime
	left join [VMStatistics] (NOLOCK)
		on [VMStatistics].SQLServerID = MonitoredSQLServers.[SQLServerID] 
		and [VMStatistics].UTCCollectionDateTime =  ibmct.UTCCollectionDateTime
	left join [ESXConfigData] (NOLOCK)
		on [ESXConfigData].SQLServerID  = MonitoredSQLServers.[SQLServerID] 
		and [ESXConfigData].UTCCollectionDateTime  =  ibmct.UTCCollectionDateTime
	left join [ESXStatistics] (NOLOCK)
		on [ESXStatistics].SQLServerID = MonitoredSQLServers.[SQLServerID] 
		and [ESXStatistics].UTCCollectionDateTime  =  ibmct.UTCCollectionDateTime
	left join cte_TempdbFileData
		on cte_TempdbFileData.InstanceID = MonitoredSQLServers.[SQLServerID] 
		and cte_TempdbFileData.UTCCollectionDateTime =  ibmct.UTCCollectionDateTime
--WHERE [MonitoredSQLServers].Active = 1
ORDER BY ServerStatistics.SQLServerID;

SELECT
		AD.Metric,
		AD.SQLServerID,
		AD.MaxSeverity
	FROM
		( SELECT a.Metric, mss.SQLServerID, MAX(a.Severity) AS MaxSeverity
		  from MetricInfo mi (NOLOCK)
		  JOIN Alerts a (NOLOCK)
			ON mi.Metric = a.Metric
		  JOIN MonitoredSQLServers (NOLOCK) AS mss 
			ON mss.InstanceName = a.ServerName
		-- SQLdm 10.2 (Anshul Aggarwal) SQLDM-27331 - Grid Overview : 'Health index/Alert' columns state is shown incorrectly.
		  LEFT OUTER JOIN MetricThresholds T (NOLOCK)	
		    ON mss.[SQLServerID] = T.[SQLServerID] and mi.[Metric] = T.[Metric] 	
		  LEFT OUTER JOIN DBMetrics dbm (NOLOCK) 
			ON dbm.MetricID = mi.Metric
			WHERE a.Active = 1
					and mss.Active = 1  -- SQLdm 10.2 (Anshul Aggarwal) SQLDM-27331 - Not including Maintenance Mode Enabled by default (A.Metric = 48)
					-- SQLdm 10.2 (Anshul Aggarwal) SQLDM-27331 - Grid Overview : 'Health index/Alert' columns state is shown incorrectly.
					and (T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @now)
					and ((a.UTCOccurrenceDateTime = mss.LastScheduledCollectionTime and dbm.MetricID is null) or
						(a.UTCOccurrenceDateTime = mss.LastDatabaseCollectionTime and dbm.MetricID is not null))
			GROUP BY mss.SQLServerID,a.Metric) AS AD;

--SQLdm 10.2.2- Fix for issue SQLDM-28538
-- create filtered table of selected servers and metrics to include
create table #IntermediateTempTable (SQLServerID int,
									 InstanceName nvarchar(255) collate SQL_Latin1_General_CP1_CS_AS , 
								     Metric int, 
									 LastScheduledCollectionTime datetime, 
									 LastDatabaseCollectionTime datetime, 
									 LastAlertRefreshTime datetime,
									 IsSnoozed bit,
									 IsDBNetric bit)
	create index IDX_TEMP ON #IntermediateTempTable (InstanceName, Metric)

	declare @current datetime
	Select @current= DateAdd(second,10,GetUTCDate())
	declare @starting datetime 
	select @starting = DATEADD(year, -10, GETUTCDATE())
	declare @ending datetime
	select @ending= @current
		
	insert into #IntermediateTempTable
	select
			MS.[SQLServerID],
			MS.[InstanceName],
			M.Metric, 
			MS.LastScheduledCollectionTime, 
			MS.LastDatabaseCollectionTime,
			MS.LastAlertRefreshTime,
			case when T.UTCSnoozeEnd > @current then 1 else 0 end,
			case
				when DBM.MetricID is null then
					0
				else
					1
			end
		FROM MetricMetaData M (NOLOCK)
		cross join MonitoredSQLServers MS (NOLOCK)
		join MetricInfo as MI on M.Metric = MI.Metric
		left outer join MetricThresholds T (NOLOCK) on
				MS.[SQLServerID] = T.[SQLServerID] and
				M.[Metric] = T.[Metric] 	
		left outer join DBMetrics DBM (NOLOCK) on
				DBM.MetricID = M.[Metric]
			WHERE 
				MS.Active = 1 and M.Deleted = 0 and
				(T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @current)

create table #TempAlert(
		SQLServerID int,
		ServerName nvarchar(256)collate SQL_Latin1_General_CP1_CS_AS , 
		Severity tinyint
		 )
create index IDX_IMDT ON #TempAlert (ServerName, Severity)

insert into #TempAlert 
		SELECT I.[SQLServerID], I.[InstanceName] AS ServerName,A.[Severity]				
				from #IntermediateTempTable I				
					inner join Alerts A (nolock) on 
						A.[ServerName] = I.InstanceName collate SQL_Latin1_General_CP1_CS_AS  and
						((A.UTCOccurrenceDateTime = I.LastScheduledCollectionTime and I.IsDBNetric = 0) or (A.UTCOccurrenceDateTime = I.LastDatabaseCollectionTime and I.IsDBNetric = 1)) and
						A.Metric = I.Metric
				where
					(A.Metric = 48 or A.[Active] = 1)
					and I.IsSnoozed = 0
					and (A.[UTCOccurrenceDateTime] between @starting and @ending)
select SQLServerID,
		ServerName, 
		Max(Severity) as [Status] from #TempAlert
		Group by SQLServerID,ServerName
drop table #IntermediateTempTable		
drop table #TempAlert
--SQLdm 10.2.2- Fix for issue SQLDM-28538
			
select @err = @@error
return @err

END
