-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup and Mount Point Monitoring Improvements
-- get the disk drives information

-- exec p_GetDiskSizeDetails @SQLServerID

if (object_id('p_GetDiskSizeDetails') is not null)
begin
	drop procedure p_GetDiskSizeDetails
end
go

create procedure [dbo].[p_GetDiskSizeDetails]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime =null,
	@HistoryInMinutes int =null
as
begin
	
	set transaction isolation level read uncommitted
declare @err int

declare @BeginDateTime datetime
declare @EndDateTime datetime

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [DiskDriveStatistics] where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

	Select  @UTCSnapshotCollectionDateTime=max([UTCCollectionDateTime]) from DiskDriveStatistics 
	where
		SQLServerID = @SQLServerID
		and [UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
	
	select 
		DiskDriveStatisticsID,
		SQLServerID,
		UTCCollectionDateTime, 
		DriveName,
		UnusedSizeKB,
		TotalSizeKB
	from
		DiskDriveStatistics
	where
		SQLServerID = @SQLServerID
		and [UTCCollectionDateTime] =@UTCSnapshotCollectionDateTime 

	select 
		DFS.DriveName,
		DFS.InitialSize,
		DFS.UsedSpace,
		DFS.AvailableSpace,
		CASE when DF.FileGroupName = 'n/a' THEN 0
			ELSE 1
		END AS IsDataFile
	from  DatabaseFileStatistics  DFS
	JOIN DatabaseFiles DF ON DF.FileID=DFS.FileID
	JOIN SQLServerDatabaseNames SSDN ON SSDN.DatabaseID=DF.DatabaseID
	WHERE SSDN.SQLServerID = @SQLServerID
	AND DFS.[UTCCollectionDateTime] =@UTCSnapshotCollectionDateTime


end
go
