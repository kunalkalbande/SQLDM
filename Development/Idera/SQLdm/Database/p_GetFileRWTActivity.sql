-- Get File Read Write Transfer Activity
-- example
-- EXEC [dbo].[p_GetFileRWTActivity] @SQLServerID=1,@HistoryInMinutes=120
if (object_id('p_GetFileRWTActivity') is not null)
begin
drop procedure p_GetFileRWTActivity
end
go 
create procedure p_GetFileRWTActivity
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
	DatabaseFiles.FileID,UTCCollectionDateTime,Reads,Writes,TimeDeltaInSeconds,DatabaseFiles.DatabaseID,FileName,FileType,FilePath,
	DriveName,FileGroupName,SQLServerID,DatabaseName,CreationDateTime,IsDeleted
from
	DatabaseFileActivity (NOLOCK)
	join DatabaseFiles (NOLOCK)
	on DatabaseFileActivity.FileID=DatabaseFiles.FileID 
	left join SQLServerDatabaseNames (NOLOCK) 
	on DatabaseFiles.DatabaseID=SQLServerDatabaseNames.DatabaseID
where
	DriveName in (
		select distinct DriveName from DiskDrives 
		where SQLServerID=@SQLServerID 
		and DiskDrives.UTCCollectionDateTime between @BeginDateTime and @EndDateTime
	) 
	and DatabaseFileActivity.UTCCollectionDateTime between @BeginDateTime and @EndDateTime
	and SQLServerDatabaseNames.IsDeleted = 0
	and SQLServerID=@SQLServerID
end