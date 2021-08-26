if (object_id('p_InsertDiskDrive') is not null)
begin
drop procedure p_InsertDiskDrive
end
go
create procedure p_InsertDiskDrive
	@SQLServerID int,
	@DriveName nvarchar(256),
	@UTCCollectionDateTime datetime,
	@UnusedSizeKB dec(18,0),
	@TotalSizeKB dec(18,0),
	@DiskIdlePercent bigint,
	@AverageDiskQueueLength bigint,
	@AverageDiskMillisecondsPerRead bigint,
	@AverageDiskMillisecondsPerTransfer bigint,
	@AverageDiskMillisecondsPerWrite bigint,
	@DiskReadsPerSecond bigint,
	@DiskTransfersPerSecond bigint,
	@DiskWritesPerSecond bigint,
	@ReturnMessage nvarchar(128) output
as
begin

	declare @DatabaseSizeTime datetime
	
	select 	@DatabaseSizeTime = LastDatabaseCollectionTime
	from MonitoredSQLServers
	where SQLServerID = @SQLServerID
	
	insert into [dbo].[DiskDrives]
	(
		[SQLServerID],
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
	)
	values
	(	
		@SQLServerID,
		@UTCCollectionDateTime,
		@DriveName,
		@UnusedSizeKB,
		@TotalSizeKB,
		@DiskIdlePercent,
		@AverageDiskQueueLength,
		@AverageDiskMillisecondsPerRead,
		@AverageDiskMillisecondsPerTransfer,
		@AverageDiskMillisecondsPerWrite,
		@DiskReadsPerSecond,
		@DiskTransfersPerSecond,
		@DiskWritesPerSecond,
		@DatabaseSizeTime
	)

end