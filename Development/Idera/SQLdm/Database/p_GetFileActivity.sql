if (object_id('p_GetFileActivity') is not null)
begin
drop procedure p_GetFileActivity
end
go 
create procedure p_GetFileActivity
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
begin

declare @BeginDateTime datetime
declare @EndDateTime datetime

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [DiskDrives] (NOLOCK) where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

select
	UTCCollectionDateTime = da.UTCCollectionDateTime,	
	DriveName = dd.DriveName,
	DatabaseName,
	[FileName]= rtrim([FileName]),
	FileType = rtrim(FileType),
	FilePath = rtrim(FilePath),
	DiskReadsPerSecond,	
	DiskWritesPerSecond,
	FileReadsPerSecond = da.Reads / CASE WHEN TimeDeltaInSeconds = 0 THEN 1 ELSE TimeDeltaInSeconds END,
	FileWritesPerSecond = da.Writes / CASE WHEN TimeDeltaInSeconds = 0 THEN 1 ELSE TimeDeltaInSeconds END,
	DiskTransfersPerSecond,
	FileTransfersPerSecond = (da.Reads + da.Writes) / CASE WHEN TimeDeltaInSeconds = 0 THEN 1 ELSE TimeDeltaInSeconds END
from
	DiskDrives dd (NOLOCK)
	left join SQLServerDatabaseNames dn (NOLOCK)
	on dd.SQLServerID = dn.SQLServerID
	left join DatabaseFiles df (NOLOCK)
	on dn.DatabaseID = df.DatabaseID
	and lower(df.DriveName) = lower(dd.DriveName)
	left join DatabaseFileActivity da (NOLOCK)
	on df.FileID = da.FileID
	and (da.UTCCollectionDateTime = ISNULL(dd.DatabaseSizeTime,dd.UTCCollectionDateTime))
where
	dd.SQLServerID = @SQLServerID
	and dn.SQLServerID = @SQLServerID
	and dd.[UTCCollectionDateTime] >= @BeginDateTime 
	and dd.[UTCCollectionDateTime] <= @EndDateTime
	and da.[UTCCollectionDateTime] >= @BeginDateTime 
	and da.[UTCCollectionDateTime] <= @EndDateTime
	and dn.IsDeleted = 0 -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 
	
end