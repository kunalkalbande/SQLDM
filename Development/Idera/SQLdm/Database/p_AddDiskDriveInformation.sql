-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup Improvements
-- insert the disk drives information

-- exec p_AddDiskDriveInformation   @SQLServerID = 1,
--									@UTCCollectionDateTime = '2014-07-14 16:04:52.283',
--									@DriveName = 'C',
--									@UnusedSizeKB = 89,
--									@TotalSizeKB = 56,
--									@DiskIdlePercent = 46,
--									@AverageDiskQueueLength = 45,
--									@AverageDiskMillisecondsPerRead = 56,
--									@AverageDiskMillisecondsPerTransfer = 34,
--									@AverageDiskMillisecondsPerWrite = 89,
--									@DiskReadsPerSecond = 34,
--									@DiskTransfersPerSecond = 89,
--									@DiskWritesPerSecond = 23,
--									@DatabaseSizeTime = '2014-07-14 16:04:52.283'

if (object_id('p_AddDiskDriveInformation') is not null)
begin
	drop procedure [p_AddDiskDriveInformation]
end
go

create procedure [dbo].[p_AddDiskDriveInformation]
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@DriveName nvarchar(256),
	@UnusedSizeKB dec(18,0),
	@TotalSizeKB dec(18,0),
	@DiskReadsPerSecond bigint,
	@DiskWritesPerSecond bigint
as
begin
	insert into [DiskDriveStatistics]
	(
		SQLServerID, 
		UTCCollectionDateTime, 
		DriveName, 
		UnusedSizeKB, 
		TotalSizeKB, 
		DiskReadsPerSecond,
		DiskWritesPerSecond
	)
	values
	(
		@SQLServerID,
		@UTCCollectionDateTime,
		@DriveName,
		@UnusedSizeKB,
		@TotalSizeKB,
		@DiskReadsPerSecond,
		@DiskWritesPerSecond
	)
end
go
