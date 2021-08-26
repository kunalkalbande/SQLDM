
IF (OBJECT_ID('p_GetPredictiveModelInput') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_GetPredictiveModelInput]
END
GO

CREATE PROCEDURE [dbo].[p_GetPredictiveModelInput] 
	@SID   int,
	@INT   int,
	@START datetime,
	@END   datetime
AS
BEGIN

select max(UTCCollectionDateTime), AVG( TimeDeltaInSeconds), AVG( BufferCacheHitRatioPercentage), AVG( CheckpointWrites), AVG( ClientComputers), AVG( CPUActivityPercentage), AVG( CPUTimeDelta), AVG( CPUTimeRaw), AVG( FullScans), AVG( IdleTimeDelta), AVG( IdleTimePercentage), AVG( IdleTimeRaw), AVG( IOActivityPercentage), AVG( IOTimeDelta), AVG( IOTimeRaw), AVG( LazyWriterWrites), AVG( LockWaits), AVG( Logins), AVG( LogFlushes), AVG( SqlMemoryAllocatedInKilobytes), AVG( SqlMemoryUsedInKilobytes), AVG( OldestOpenTransactionsInMinutes), AVG( PacketErrors), AVG( PacketsReceived), AVG( PacketsSent), AVG( PageErrors), AVG( PageLifeExpectancy), AVG( PageLookups), AVG( PageReads), AVG( PageSplits), 
AVG( PageWrites), AVG( ProcedureCacheHitRatioPercentage), AVG( ProcedureCacheSizeInKilobytes), AVG( ProcedureCacheSizePercent), AVG( ReadAheadPages), AVG( ReplicationLatencyInSeconds), AVG( ResponseTimeInMilliseconds), AVG( SqlCompilations), AVG( SqlRecompilations), AVG( TableLockEscalations), AVG( TempDBSizeInKilobytes), AVG( TempDBSizePercent), AVG(Batches), AVG( UserProcesses), AVG( WorkFilesCreated), AVG( WorkTablesCreated), AVG( SystemProcesses), AVG( UserProcessesConsumingCPU), AVG( SystemProcessesConsumingCPU), AVG( BlockedProcesses), AVG( OpenTransactions), AVG( DatabaseCount), AVG( DataFileCount), AVG( LogFileCount), AVG( DataFileSpaceAllocatedInKilobytes), AVG( DataFileSpaceUsedInKilobytes), 
AVG( LogFileSpaceAllocatedInKilobytes), AVG( LogFileSpaceUsedInKilobytes), AVG( TotalLocks), AVG( BufferCacheSizeInKilobytes), AVG( ActiveProcesses), AVG( LeadBlockers), AVG( CommittedInKilobytes), AVG( ConnectionMemoryInKilobytes), AVG( FreePagesInKilobytes), AVG( GrantedWorkspaceMemoryInKilobytes), AVG( LockMemoryInKilobytes), AVG( OptimizerMemoryInKilobytes), AVG( TotalServerMemoryInKilobytes), AVG( FreeCachePagesInKilobytes), AVG( CachePagesInKilobytes), AVG( MaxConnections), AVG( PhysicalMemoryInKilobytes), AVG( ProcessorCount), AVG( ProcessorsUsed), AVG( DistributionLatencyInSeconds)
from ServerStatistics with (nolock)
where SQLServerID = @SID and UTCCollectionDateTime between DATEADD(MINUTE, -@INT, @START) and @END

END