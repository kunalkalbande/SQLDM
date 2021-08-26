if (object_id('p_InsertDatabaseStatistics') is not null)
begin
drop procedure p_InsertDatabaseStatistics
end
go
create procedure p_InsertDatabaseStatistics
	@SQLServerID int,
	@DatabaseName nvarchar(255),
	@SystemDatabase bit,
	@UTCCollectionDateTime datetime,
	@DatabaseStatus int,
	@Transactions bigint,
	@LogFlushWaits bigint,
	@LogFlushes bigint,
	@LogKilobytesFlushed bigint,
	@LogCacheReads bigint,
	@LogCacheHitRatio float,
	@TimeDeltaInSeconds float,
	@ReturnDatabaseID int output,
	@NumberReads decimal,
	@NumberWrites decimal,
	@BytesRead decimal,
	@BytesWritten decimal,
	@IoStallMS decimal,  
	@DatabaseCreateDate datetime,
	@LastBackupDate datetime, -- SQLdm 10.0 (Gaurav Karwal) - Database backup alerts
	@AverageDataIO decimal(18,4),--SQLdm 11.0 -Azure Support
	@AverageLogIO decimal(18,4),
	@MaxWorker decimal(18,4),
	@MaxSession decimal(18,4),
	@DatabaseAverageMemoryUsage decimal(18,4),
	@InMemoryStorageUsage decimal (18,4),
	@ReturnMessage nvarchar(128) output,
	@AvgCpuPercent decimal,-- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	@AvgDataIoPercent decimal,-- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	@AvgLogWritePercent decimal, -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	@DtuLimit int,
	@AzureCloudAllocatedMemory decimal,
	@AzureCloudUsedMemory decimal,
	@AzureCloudStorageLimit decimal,
	@ElasticPool varchar --SQLdm 11.0 Elastic Pool Support-Nikhil
as
begin

declare @DatabaseID int,
		@InnerReturnMessage nvarchar(128),
		@DatabaseSizeTime datetime

execute [p_InsertDatabaseName] 
   @SQLServerID
  ,@DatabaseName
  ,@SystemDatabase
  ,@DatabaseCreateDate
  ,@DatabaseID output
  ,@InnerReturnMessage output

select @DatabaseSizeTime = max(UTCCollectionDateTime)
from DatabaseSize (nolock)
where DatabaseID = @DatabaseID

IF NOT EXISTS(Select [DatabaseID] from [DatabaseStatistics] where [DatabaseID] = @DatabaseID and [UTCCollectionDateTime] = @UTCCollectionDateTime)
	BEGIN

insert into [DatabaseStatistics]
	([DatabaseID]
	,[UTCCollectionDateTime]
	,[DatabaseStatus]
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
	,[LastBackupDateTime] -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
	,[AverageDataIO]--SQLdm 11.0 -Azure Support
	,[AverageLogIO]
	,[MaxWorker]
	,[MaxSession]
	,[DatabaseAverageMemoryUsage]
	,[InMemoryStorageUsage]
	,[AvgCpuPercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,[AvgDataIoPercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,[AvgLogWritePercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,[DtuLimit] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,[AzureCloudAllocatedMemory]
	,[AzureCloudUsedMemory]
	,[AzureCloudStorageLimit]
	,[ElasticPool]
)
 values
	(@DatabaseID
	,@UTCCollectionDateTime
	,@DatabaseStatus
	,@Transactions
	,@LogFlushWaits
	,@LogFlushes
	,@LogKilobytesFlushed
	,@LogCacheReads
	,@LogCacheHitRatio
	,@TimeDeltaInSeconds
	,@NumberReads 
	,@NumberWrites 
	,@BytesRead 
	,@BytesWritten 
	,@IoStallMS
	,@DatabaseSizeTime
	,@LastBackupDate -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
	,@AverageDataIO --SQLdm 11.0 -Azure Support
	,@AverageLogIO
	,@MaxWorker
	,@MaxSession
	,@DatabaseAverageMemoryUsage
	,@InMemoryStorageUsage
	,@AvgCpuPercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,@AvgDataIoPercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,@AvgLogWritePercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
	,@DtuLimit -- SQLdm 11.0 (Rajat) - Azure Cpu Chart) 
	
	,@AzureCloudAllocatedMemory
	,@AzureCloudUsedMemory
	,@AzureCloudStorageLimit
	,@ElasticPool
)

	END
	ELSE
	BEGIN

UPDATE [DatabaseStatistics] 
	set [DatabaseStatus] = @DatabaseStatus, [Transactions] = @Transactions, 
		[LogFlushWaits] = @LogFlushWaits, [LogFlushes] = @LogFlushes, 
		[LogKilobytesFlushed] = @LogKilobytesFlushed, [LogCacheReads] = @LogCacheReads, 
		[LogCacheHitRatio] = @LogCacheHitRatio, [TimeDeltaInSeconds] = @TimeDeltaInSeconds, 
		[NumberReads] = @NumberReads, [NumberWrites] = @NumberWrites, 
		[BytesRead] = @BytesRead, [BytesWritten] = @BytesWritten, 
		[IoStallMS] = @IoStallMS, [DatabaseSizeTime] = @DatabaseSizeTime, 
		[LastBackupDateTime] = @LastBackupDate,
		[AverageDataIO]=@AverageDataIO,--SQLdm 11.0 -Azure Support
		[AverageLogIO]=@AverageLogIO,
		[MaxWorker]=@MaxWorker,
		[MaxSession]=@MaxSession,
		[DatabaseAverageMemoryUsage]=@DatabaseAverageMemoryUsage,
		[InMemoryStorageUsage]=@InMemoryStorageUsage,
		[AvgCpuPercent]  = @AvgCpuPercent,[AvgDataIoPercent] = @AvgDataIoPercent,
		[AvgLogWritePercent] = @AvgLogWritePercent,[DtuLimit] = @DtuLimit, 
		[AzureCloudAllocatedMemory]=@AzureCloudAllocatedMemory,
		[AzureCloudUsedMemory] = @AzureCloudUsedMemory, [AzureCloudStorageLimit] =@AzureCloudStorageLimit,
	    [ElasticPool]=@ElasticPool
	where [DatabaseID] = @DatabaseID and [UTCCollectionDateTime] = @UTCCollectionDateTime

	End

set @ReturnDatabaseID = @DatabaseID

end