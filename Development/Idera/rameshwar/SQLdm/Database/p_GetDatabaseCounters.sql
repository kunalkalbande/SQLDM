if (object_id('[p_GetDatabaseCounters]') is not null)
begin
drop procedure [p_GetDatabaseCounters]
end
go
create procedure [dbo].[p_GetDatabaseCounters]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
begin
set transaction isolation level read uncommitted
declare @err int
declare @BeginDateTime datetime
declare @EndDateTime datetime

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [ServerStatistics] where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

SELECT * INTO #DS FROM
(
	SELECT DatabaseName,
		UTCCollectionDateTime,
		DatabaseStatus,
		Transactions,
		LogFlushWaits,
		LogFlushes,
		LogKilobytesFlushed,
		LogCacheReads,
		LogCacheHitRatio,
		TimeDeltaInSeconds,
		NumberReads,
		NumberWrites,
		BytesRead,
		BytesWritten,
		IoStallMS,
		LastBackupDateTime,
		AverageDataIO,  --SQLdm 11.0 (Azure Support)
        AverageLogIO,
        MaxWorker,
        MaxSession,
        DatabaseAverageMemoryUsage,
        InMemoryStorageUsage,
		AvgCpuPercent,
		AvgDataIoPercent,
		AvgLogWritePercent,
		DtuLimit,
		AzureCloudAllocatedMemory,
		AzureCloudUsedMemory,
		AzureCloudStorageLimit,
        ElasticPool		
	FROM DatabaseStatistics ds (nolock)	inner join [SQLServerDatabaseNames] (nolock) on ds.DatabaseID = [SQLServerDatabaseNames].DatabaseID 
	inner join [MonitoredSQLServers] (nolock) on [SQLServerDatabaseNames].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
	where
	[SQLServerDatabaseNames].[SQLServerID] = @SQLServerID
	and ds.[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime	
	
	UNION ALL
	
	SELECT DatabaseName,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		DatabaseStatus,
		TotalTransactions AS Transactions,
		TotalLogFlushWaits AS LogFlushWaits,
		TotalLogFlushes AS LogFlushes,
		TotalLogKilobytesFlushed AS LogKilobytesFlushed,
		TotalLogCacheReads AS LogCacheReads,
		TotalLogCacheHitRatio AS LogCacheHitRatio,
		TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
		TotalNumberReads AS NumberReads,
		TotalNumberWrites AS NumberWrites,
		TotalBytesRead AS BytesRead,
		TotalBytesWritten AS BytesWritten,
		TotalIoStallMS AS IoStallMS,
		MaxLastBackupDateTime AS LastBackupDateTime,
		MaxAverageDataIO AS AverageDataIO,--SQLdm 11.0 (Azure Support) 
        MaxAverageLogIO AS AverageLogIO,
        MaxMaxWorker AS MaxWorker,
        MaxMaxSession AS MaxSession,
        MaxDatabaseAverageMemoryUsage AS DatabaseAverageMemoryUsage,
        MaxInMemoryStorageUsage AS InMemoryStorageUsage,
		
		MaxAvgCpuPercent AS AvgCpuPercent,
		MaxAvgDataIoPercent AS AvgDataIoPercent,
		MaxAvgLogWritePercent AS AvgLogWritePercent,
		MaxDtuLimit AS DtuLimit,

		MaxAzureCloudAllocatedMemory AS AzureCloudAllocatedMemory,
		MaxAzureCloudUsedMemory AS AzureCloudUsedMemory,
		MaxAzureCloudStorageLimit AS AzureCloudStorageLimit	,
		ElasticPool  As ElasticPool
	FROM DatabaseStatisticsAggregation ds(nolock)	inner join [SQLServerDatabaseNames] (nolock) on ds.DatabaseID = [SQLServerDatabaseNames].DatabaseID 
	inner join [MonitoredSQLServers] (nolock) on [SQLServerDatabaseNames].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
	where
	[SQLServerDatabaseNames].[SQLServerID] = @SQLServerID
	and ds.[MaxUTCCollectionDateTime] between @BeginDateTime and @EndDateTime

) AS DSAggregate;

-- Get trend data
select
	[DatabaseName],
	[UTCCollectionDateTime],
	[DatabaseStatus],
	[Transactions],
	[LogFlushWaits],
	[LogFlushes],
	[LogKilobytesFlushed],
	[LogCacheReads],
	[LogCacheHitRatio],
	[TimeDeltaInSeconds],
	[NumberReads],
	[NumberWrites],
	[BytesRead],
	[BytesWritten],
	[IoStallMS],
	[LastBackupDateTime], -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
	[AverageDataIO], --SQLdm 11.0 (Azure Support)
    [AverageLogIO],
    [MaxWorker],
    [MaxSession],
    [DatabaseAverageMemoryUsage],
    [InMemoryStorageUsage],
	
	[AvgCpuPercent],
	[AvgDataIoPercent],
	[AvgLogWritePercent],
	[DtuLimit],
	[AzureCloudAllocatedMemory],
	[AzureCloudUsedMemory],
	[AzureCloudStorageLimit],
	[ElasticPool]
from
	#DS (nolock)
order by 
	#DS.[UTCCollectionDateTime]
	
select @err = @@error
return @err

DROP TABLE #DS;
end
