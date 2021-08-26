if (object_id('p_AggregateDatabaseStatistics') is not null)
begin
drop procedure p_AggregateDatabaseStatistics
end
go
CREATE procedure p_AggregateDatabaseStatistics
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime, 
		@SQLServerID int,  
		@AggregateDatabaseStatisticsMonitorDays int,
		@Now datetime, 
		@InstanceName nvarchar(256), 
		@RC int,
		@RowsAffected int,
		@run_id uniqueidentifier,
		@Sequence int,
		@TimeoutTime datetime,
		@RecordsToDelete int

set @Now = getdate()
set @TimeoutTime = dateadd(mi,120,getutcdate())
set @run_id = newid()
set @Sequence = 0		
set @RecordsToDelete = 5000

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Database Statistics', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDatabaseStatisticsMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDatabaseStatisticsMonitorDays is null)
	set @AggregateDatabaseStatisticsMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDatabaseStatisticsMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin DatabaseSize Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
	;with DatabaseStatisticsAggregationStepCte
    (
        [DatabaseID]
        ,[DatabaseStatus]
        ,[UTCCollectionDateTime]
        ,[Transactions]
        ,[LogFlushWaits]
        ,[LogFlushes]
        ,[LogKilobytesFlushed]
        ,[LogCacheReads]
        ,[LogCacheHitRatio]
        ,[TimeDeltaInSeconds]
        ,[NumberReads]
        ,[NumberWrites]
        ,[BytesRead]
        ,[BytesWritten]
        ,[IoStallMS]
        ,[DatabaseSizeTime]
        ,[LastBackupDateTime]
		,[AverageDataIO]
		,[AverageLogIO]
		,[MaxWorker]
		,[MaxSession]
		,[DatabaseAverageMemoryUsage]
        ,[InMemoryStorageUsage]
		,[AvgCpuPercent] 
		,[AvgDataIoPercent]
		,[AvgLogWritePercent] 
		,[DtuLimit]
		,[AzureCloudAllocatedMemory]
		,[AzureCloudUsedMemory]
		,[AzureCloudStorageLimit]
		,[ElasticPool]
    ) as
    ( select top (@RecordsToDelete)
         ds.[DatabaseID]
        ,ds.[DatabaseStatus]
        ,ds.[UTCCollectionDateTime]
        ,ds.[Transactions]
        ,ds.[LogFlushWaits]
        ,ds.[LogFlushes]
        ,ds.[LogKilobytesFlushed]
        ,ds.[LogCacheReads]
        ,ds.[LogCacheHitRatio]
        ,ds.[TimeDeltaInSeconds]
        ,ds.[NumberReads]
        ,ds.[NumberWrites]
        ,ds.[BytesRead]
        ,ds.[BytesWritten]
        ,ds.[IoStallMS]
        ,ds.[DatabaseSizeTime]
        ,ds.[LastBackupDateTime]
		,ds.[AverageDataIO]
		,ds.[AverageLogIO]
		,ds.[MaxWorker]
		,ds.[MaxSession]
		,ds.[DatabaseAverageMemoryUsage]
        ,ds.[InMemoryStorageUsage]
		,ds.[AvgCpuPercent] 
		,ds.[AvgDataIoPercent] 
		,ds.[AvgLogWritePercent] 
		,ds.[DtuLimit]	
		,ds.[AzureCloudAllocatedMemory]
		,ds.[AzureCloudUsedMemory]
		,ds.[AzureCloudStorageLimit]
		,ds.[ElasticPool]
	    from 
		[DatabaseStatistics] ds with (nolock)
        inner join dbo.SQLServerDatabaseNames dn with (nolock)
        on dn.[DatabaseID] = ds.[DatabaseID]
        where [SQLServerID] = @SQLServerID
        and ds.[UTCCollectionDateTime] <= @ActivityCutoff
	)
	insert into [DatabaseStatisticsAggregation]
	(
	    [DatabaseID],
		[DatabaseStatus],
		[MaxUTCCollectionDateTime],
		[MinUTCCollectionDateTime],
		[MaxTransactions],
		[MinTransactions],
		[TotalTransactions],
		[MaxLogFlushWaits],
		[MinLogFlushWaits],
		[TotalLogFlushWaits],
		[MaxLogFlushes],
		[MinLogFlushes],
		[TotalLogFlushes],
		[MaxLogKilobytesFlushed],
		[MinLogKilobytesFlushed],
		[TotalLogKilobytesFlushed],
		[MaxLogCacheReads],
		[MinLogCacheReads],
		[TotalLogCacheReads],
		[MaxLogCacheHitRatio],
		[MinLogCacheHitRatio],
		[TotalLogCacheHitRatio],
		[MaxTimeDeltaInSeconds],
		[MinTimeDeltaInSeconds],
		[TotalTimeDeltaInSeconds],
		[MaxNumberReads],
		[MinNumberReads],
		[TotalNumberReads],
		[MaxNumberWrites],
		[MinNumberWrites],
		[TotalNumberWrites],
		[MaxBytesRead],
		[MinBytesRead],
		[TotalBytesRead],
		[MaxBytesWritten],
		[MinBytesWritten],
		[TotalBytesWritten],
		[MaxIoStallMS],
		[MinIoStallMS],
		[TotalIoStallMS],
		[MaxDatabaseSizeTime],
		[MinDatabaseSizeTime],
		[MaxLastBackupDateTime],
		[MinLastBackupDateTime],
		[MaxAverageDataIO], --SQLdm 11.0 (Azure Support)
	    [MinAverageDataIO],
	    [MaxAverageLogIO],
        [MinAverageLogIMinO],
	    [MaxMaxWorker],
	    [MinMaxWorker],
	    [MaxMaxSession],
	    [MinMaxSession],
	    [MaxDatabaseAverageMemoryUsage],
	    [MinDatabaseAverageMemoryUsage],
	    [MaxInMemoryStorageUsage],
	    [MinInMemoryStorageUsage],
		[MaxAvgCpuPercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MinAvgCpuPercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MaxAvgDataIoPercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MinAvgDataIoPercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MaxAvgLogWritePercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MinAvgLogWritePercent], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MaxDtuLimit], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MinDtuLimit], -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
		[MaxAzureCloudAllocatedMemory],
		[MinAzureCloudAllocatedMemory],
		[MaxAzureCloudUsedMemory],
		[MinAzureCloudUsedMemory],
		[MaxAzureCloudStorageLimit],
		[MinAzureCloudStorageLimit],
		[ElasticPool]
	)
	select
	    [DatabaseID],
		[DatabaseStatus],
		max([UTCCollectionDateTime]),
		min([UTCCollectionDateTime]),
		max([Transactions]),
		min([Transactions]),
		sum([Transactions]),
		max([LogFlushWaits]),
		min([LogFlushWaits]),
		sum([LogFlushWaits]),
		max([LogFlushes]),
		min([LogFlushes]),
		sum([LogFlushes]),
		max([LogKilobytesFlushed]),
		min([LogKilobytesFlushed]),
		sum([LogKilobytesFlushed]),
		max([LogCacheReads]),
		min([LogCacheReads]),
		sum([LogCacheReads]),
		max([LogCacheHitRatio]),
		min([LogCacheHitRatio]),
		sum([LogCacheHitRatio]),
		max([TimeDeltaInSeconds]),
		min([TimeDeltaInSeconds]),
		sum([TimeDeltaInSeconds]),
		max([NumberReads]),
		min([NumberReads]),
		sum([NumberReads]),
		max([NumberWrites]),
		min([NumberWrites]),
		sum([NumberWrites]),
		max([BytesRead]),
		min([BytesRead]),
		sum([BytesRead]),
		max([BytesWritten]),
		min([BytesWritten]),
		sum([BytesWritten]),
		max([IoStallMS]),
		min([IoStallMS]),
		sum([IoStallMS]),
		max([DatabaseSizeTime]),
		min([DatabaseSizeTime]),
		max([LastBackupDateTime]),
		min([LastBackupDateTime]),
		max([AverageDataIO]),
		min([AverageDataIO]),
		max([AverageLogIO]),
		min([AverageLogIO]),
		max([MaxWorker]),
		min([MaxWorker]),
		max([MaxSession]),
		min([MaxSession]),
		max([DatabaseAverageMemoryUsage]),
		min([DatabaseAverageMemoryUsage]),
        max([InMemoryStorageUsage]),
        min([InMemoryStorageUsage]),
		max([AvgCpuPercent]),
		min([AvgCpuPercent]),
		max([AvgDataIoPercent]),
		min([AvgDataIoPercent]),
		max([AvgLogWritePercent]),
		min([AvgLogWritePercent]),
		max([DtuLimit]),
		min([DtuLimit]),
		max([AzureCloudAllocatedMemory]),
        min([AzureCloudAllocatedMemory]),
		max([AzureCloudUsedMemory]),
        min([AzureCloudUsedMemory]),
		max([AzureCloudStorageLimit]),
        min([AzureCloudStorageLimit]),
		[ElasticPool]
	from DatabaseStatisticsAggregationStepCte
	group by [DatabaseID], [DatabaseStatus],[ElasticPool]

	---------------------------------------------
	-- End DatabaseStatistics Aggregation
	---------------------------------------------

	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom Database Size
	exec Grooming.p_GroomDatabaseStatistics
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = @RecordsToDelete,
		@CutoffDateTime = @ActivityCutoff,
		@SQLServerID = @SQLServerID,
		@InstanceName = @InstanceName,
		@Deleted = 0,
		@AggregationGrooming = 1

	----------------------------------------------
	-- End DatabaseStatistics Grooming
	----------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Database Statistics', 0, null

end	



