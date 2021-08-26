if (object_id('p_AggregateDiskDriveStatisticsData') is not null)
begin
drop procedure p_AggregateDiskDriveStatisticsData
end
go
CREATE procedure p_AggregateDiskDriveStatisticsData
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime,
		@SQLServerID int ,
		@AggregateDiskDriveStatisticsMonitorDays int,
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

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Disk Drive Statistics', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDiskDriveStatisticsMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDiskDriveStatisticsMonitorDays is null)
	set @AggregateDiskDriveStatisticsMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off

	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDiskDriveStatisticsMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin DiskDriveStatistics Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
	;with DiskDriveStatisticsStepCte
    (
     [UTCCollectionDateTime],
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
     [DatabaseSizeTime]
    ) as
    ( select top (@RecordsToDelete)
    	dds.[UTCCollectionDateTime],
		dds.[DriveName],
		dds.[UnusedSizeKB],
	    dds.[TotalSizeKB],
		dds.[DiskIdlePercent],
		dds.[AverageDiskQueueLength],
		dds.[AverageDiskMillisecondsPerRead],
		dds.[AverageDiskMillisecondsPerTransfer],
		dds.[AverageDiskMillisecondsPerWrite],
		dds.[DiskReadsPerSecond],
		dds.[DiskTransfersPerSecond],
		dds.[DiskWritesPerSecond],
		dds.[DatabaseSizeTime]
	from 
		DiskDriveStatistics dds with (nolock)
    	Where dds.SQLServerID = @SQLServerID
    	      and dds.UTCCollectionDateTime <= @ActivityCutoff
	)
	insert into DiskDriveStatisticsAggregation
	(
	   [SQLServerID]
      ,[MinUTCCollectionDateTime]
      ,[MaxUTCCollectionDateTime]
      ,[DriveName]
      ,[MinUnusedSizeKB]
      ,[MaxUnusedSizeKB]
      ,[TotalUnusedSizeKB]
      ,[MinTotalzSieKB]
      ,[MaxTotalSizeKB]
      ,[TotalSizeKBPerDay]
      ,[MinDiskIdlePercent]
      ,[MaxDiskIdlePercent]
      ,[TotalDiskIdlePercent]
      ,[MinDiskReadsPerSecond]
      ,[MaxDiskReadsPerSecond]
      ,[TotalDiskReadsPerSecond]
      ,[MinDiskTransferPerSecond]
      ,[MaxDiskTransferPerSecond]
      ,[TotalTransferPerSecond]
      ,[MinDiskWritesPerSecond]
      ,[MaxDiskWritesPerSecond]
      ,[TotalDiskWritesPerSecond]
      ,[MinDatabaseSizeTime]
      ,[MaxDatabaseSizeTime]
      ,[MinAverageDiskQueueLength]
      ,[MaxAverageDiskQueueLength]
      ,[MinAverageDiskMillisecondsPerRead]
      ,[MaxAverageDiskMillisecondsPerRead]
      ,[MinAverageDiskMillisecondsPerTransfer]
      ,[MaxAverageDiskMillisecondsPerTransfer]
      ,[MinAverageDiskMillisecondsPerWrite]
      ,[MaxAverageDiskMillisecondsPerWrite]
	)
	select
	    @SQLServerID,
        min(UTCCollectionDateTime),
		max(UTCCollectionDateTime),
		[DriveName],
		min([UnusedSizeKB]),
		max([UnusedSizeKB]),
		sum([UnusedSizeKB]),
		min([TotalSizeKB]),
		max([TotalSizeKB]),
		sum([TotalSizeKB]),
	    Min(DiskIdlePercent),
		Max(DiskIdlePercent),
		sum(DiskIdlePercent),
	    Min(DiskReadsPerSecond),
		Max(DiskReadsPerSecond),
		sum(DiskReadsPerSecond),
		Min([DiskTransfersPerSecond]),
		Max([DiskTransfersPerSecond]),
		sum([DiskTransfersPerSecond]),
		Min(DiskWritesPerSecond),
		Max(DiskWritesPerSecond),
		sum(DiskWritesPerSecond),
		Min(DatabaseSizeTime),
		Max(DatabaseSizeTime),
	    min([AverageDiskQueueLength]),
		max([AverageDiskQueueLength]),
	    min([AverageDiskMillisecondsPerRead]),
		max([AverageDiskMillisecondsPerRead]),
	    min([AverageDiskMillisecondsPerTransfer]),
		max([AverageDiskMillisecondsPerTransfer]),
		min([AverageDiskMillisecondsPerWrite]),
		max([AverageDiskMillisecondsPerWrite])
	from DiskDriveStatisticsStepCte
	group by [DriveName]
	
	---------------------------------------------
	-- End DiskDriveStatistics Aggregation
	---------------------------------------------
	
	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom Database Size
	exec Grooming.p_GroomDiskDriveStatistics
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
	-- End DiskDriveStatistics Grooming
	----------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Disk Drive Statistics', 0, null	
	
end	
 

GO


