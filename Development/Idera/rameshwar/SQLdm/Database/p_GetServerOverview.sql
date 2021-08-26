if (object_id('[p_GetServerOverview]') is not null)
begin
drop procedure p_GetServerOverview 
end
go
create procedure [dbo].p_GetServerOverview
	@SQLServerID int,
	@SampleTime datetime = null
as
begin

	select top 1
		s.[UTCCollectionDateTime] as [UTCCollectionDateTime]
		,[InstanceName]
		,[BlockedProcesses]
		,[BufferCacheHitRatioPercentage]
		,[BufferCacheSizeInKilobytes]
		,[ClientComputers]
		,[CPUActivityPercentage]
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
	from
		ServerStatistics s
		left join [MonitoredSQLServers] m
		on m.[SQLServerID] = s.[SQLServerID]
		left join [OSStatistics] o
		on o.[SQLServerID] = s.[SQLServerID]  and o.[UTCCollectionDateTime] = s.[UTCCollectionDateTime]
	where 
		s.[SQLServerID] = @SQLServerID
		and s.[UTCCollectionDateTime] = coalesce(@SampleTime,s.[UTCCollectionDateTime])
	order by
		s.[UTCCollectionDateTime] desc

end
