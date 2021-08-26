if (object_id('[p_GetTempdbSummaryData]') is not null)
begin
drop procedure [p_GetTempdbSummaryData]
end
go
create procedure [dbo].[p_GetTempdbSummaryData]
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
	
;with cte_FileData (UTCCollectionDateTime, UserObjectsInKilobytes, InternalObjectsInKilobytes,VersionStoreInKilobytes,MixedExtentsInKilobytes,UnallocatedSpaceInKilobytes)as
(
select
TempdbFileData.UTCCollectionDateTime,
sum(UserObjectsInKilobytes),
sum(InternalObjectsInKilobytes),
sum(VersionStoreInKilobytes),
sum(MixedExtentsInKilobytes),
sum(UnallocatedSpaceInKilobytes)
from TempdbFileData	 (NOLOCK)
inner join DatabaseFiles (NOLOCK) on TempdbFileData.FileID = DatabaseFiles.FileID
inner join SQLServerDatabaseNames (NOLOCK) on SQLServerDatabaseNames.DatabaseID = DatabaseFiles.DatabaseID
where
	SQLServerID = @SQLServerID and
	TempdbFileData.UTCCollectionDateTime between @BeginDateTime and @EndDateTime
	and SQLServerDatabaseNames.IsDeleted = 0 -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 	
group by
UTCCollectionDateTime
)
select
	ServerStatistics.UTCCollectionDateTime, 
	UserObjectsInMB = UserObjectsInKilobytes / 1024.0, 
	InternalObjectsInMB = InternalObjectsInKilobytes / 1024.0,
	VersionStoreInMB = VersionStoreInKilobytes / 1024.0,
	MixedExtentsInMB = MixedExtentsInKilobytes / 1024.0,
	UnallocatedSpaceInMB = UnallocatedSpaceInKilobytes / 1024.0,
	TempdbPFSWaitTimeMilliseconds = TempdbPFSWaitTimeMilliseconds,
	TempdbGAMWaitTimeMilliseconds =	TempdbGAMWaitTimeMilliseconds,
	TempdbSGAMWaitTimeMilliseconds = TempdbSGAMWaitTimeMilliseconds,
	VersionStoreGenerationKilobytesPerSec = VersionStoreGenerationKilobytes / nullif(TimeDeltaInSeconds,0),
	VersionStoreCleanupKilobytesPerSec = VersionStoreCleanupKilobytes  / nullif(TimeDeltaInSeconds,0)
from	
	ServerStatistics (NOLOCK)
	left join cte_FileData fd
	on [ServerStatistics].UTCCollectionDateTime = fd.UTCCollectionDateTime
where
	[ServerStatistics].[SQLServerID] = @SQLServerID
	and [ServerStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
order by 
	[ServerStatistics].[UTCCollectionDateTime]
		
end	
