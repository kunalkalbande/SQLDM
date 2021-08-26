if (object_id('p_GetServerOverviewHistory') is not null)
begin
drop procedure p_GetServerOverviewHistory
end
go

CREATE PROCEDURE [dbo].p_GetServerOverviewHistory(
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
)
AS
begin
declare @BeginDateTime datetime
declare @EndDateTime datetime

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max([UTCCollectionDateTime]) from [ServerStatistics] (NOLOCK) where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)
	
	select

		s.[UTCCollectionDateTime] as [UTCCollectionDateTime]

		,s.[SQLServerID] as [SQLServerID]

		,[InstanceName]

		,[BlockedProcesses]

		,[BufferCacheHitRatioPercentage]

		,[BufferCacheSizeInKilobytes]

		,[ClientComputers]

		,[CPUActivityPercentage]

		,[IdleTimePercentage]

		,[DatabaseCount]

		,[DataFileCount]

		,[DataFileSpaceAllocatedInKilobytes]

		,[DataFileSpaceUsedInKilobytes]
		
		,[DiskQueueLength]

		,[LogFileCount]

		,[LogFileSpaceAllocatedInKilobytes]

		,[LogFileSpaceUsedInKilobytes]

		,[LogFlushes]

		,[LogFlushes] / nullif(TimeDeltaInSeconds,0) as [LogFlushesPerSecond]

		,[PacketErrors] 

		,[PacketErrors] / nullif(TimeDeltaInSeconds,0) as [PacketErrorsPerSecond]

		,[PacketsReceived] 

		,[PacketsReceived] / nullif(TimeDeltaInSeconds,0) as [PacketsReceivedPerSecond]

		,[PacketsSent]

		,[PacketsSent] / nullif(TimeDeltaInSeconds,0) as [PacketsSentPerSecond]

		,[PagesPerSecond]
		
		,[PageReads]
				
		,[PageReads] / nullif(TimeDeltaInSeconds,0) as [PageReadsPerSecond]
		
		,[PageWrites]
		
		,[PageWrites] / nullif(TimeDeltaInSeconds,0) as [PageWritesPerSecond]
		
		,[ProcedureCacheHitRatioPercentage]
				
		,[ProcedureCacheSizeInKilobytes]
		
		,[ProcessorQueueLength]

		,[ResponseTimeInMilliseconds]

		,[SqlCompilations]

		,[SqlCompilations] / nullif(TimeDeltaInSeconds,0) as [SqlCompilationsPerSecond]
				
		,[SqlRecompilations]
		
		,[SqlRecompilations] / nullif(TimeDeltaInSeconds,0) as [SqlRecompilationsPerSecond]
		
		,[SqlMemoryAllocatedInKilobytes]
		
		,[SqlMemoryUsedInKilobytes]
		
		,[SystemProcesses]
		
		,[SystemProcessesConsumingCPU]
		
		,s.[ServerVersion]
		
		,[Transactions]
		
		,[Transactions] / nullif(TimeDeltaInSeconds,0) as [TransactionsPerSecond]
		
		,[UserProcesses]
		
		,[UserProcessesConsumingCPU]
		
		,[Batches]
		
		,[Batches] / nullif(TimeDeltaInSeconds,0) as [BatchesPerSecond]

		,[DiskTimePercent] 

		,[OSAvailableMemoryInKilobytes]

		,[OSTotalPhysicalMemoryInKilobytes]
		
		,[FriendlyServerName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name

	from

		ServerStatistics s (NOLOCK)

		left join [MonitoredSQLServers] m (NOLOCK)

		on m.[SQLServerID] = s.[SQLServerID]

		left join [OSStatistics] o (NOLOCK)

		on o.[SQLServerID] = s.[SQLServerID]  and o.[UTCCollectionDateTime] = s.[UTCCollectionDateTime]

	where 

		s.[SQLServerID] = @SQLServerID
		
		and s.[UTCCollectionDateTime] between coalesce(@BeginDateTime,s.[UTCCollectionDateTime]) and coalesce(@EndDateTime,s.[UTCCollectionDateTime])

	order by

		s.[UTCCollectionDateTime] desc
			
END	

