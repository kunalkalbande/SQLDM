if (object_id('[p_GetTempdbFileData]') is not null)
begin
drop procedure [p_GetTempdbFileData]
end
go
create procedure [dbo].[p_GetTempdbFileData]
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
	

;with cte_max(MaxDateTime) as 
(
	select 
		max(UTCCollectionDateTime)
	from TempdbFileData	
		inner join DatabaseFiles on TempdbFileData.FileID = DatabaseFiles.FileID
		inner join SQLServerDatabaseNames on SQLServerDatabaseNames.DatabaseID = DatabaseFiles.DatabaseID
	where 
		SQLServerID = @SQLServerID and -- SqlDM 10.2 (Anshul Aggarwal) - SQLDM-27822 - Custom range drill down doesn't show data for 'Tempdb Space Used by File' graph
		UTCCollectionDateTime between @BeginDateTime and @EndDateTime
)
select
	TempdbFileData.UTCCollectionDateTime,
	FileName,
	FilePath,
	UserObjectsInMB = UserObjectsInKilobytes / 1024.0, 
	InternalObjectsInMB = InternalObjectsInKilobytes / 1024.0,
	VersionStoreInMB = VersionStoreInKilobytes / 1024.0,
	MixedExtentsInMB = MixedExtentsInKilobytes / 1024.0,
	UnallocatedSpaceInMB = UnallocatedSpaceInKilobytes / 1024.0
	from TempdbFileData	
	inner join DatabaseFiles on TempdbFileData.FileID = DatabaseFiles.FileID
	inner join SQLServerDatabaseNames on SQLServerDatabaseNames.DatabaseID = DatabaseFiles.DatabaseID
	inner join cte_max on TempdbFileData.UTCCollectionDateTime = cte_max.MaxDateTime
where
	SQLServerID = @SQLServerID and
	DatabaseName = 'tempdb' and
	TempdbFileData.UTCCollectionDateTime = MaxDateTime


		
end	
