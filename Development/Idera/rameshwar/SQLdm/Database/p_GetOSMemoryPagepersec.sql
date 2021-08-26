-- Get OS Memory Pages Per Second
-- example
-- EXEC p_GetOSMemoryPagesPerSecond @SQLServerID=1,@HistoryInMinutes=75

if (object_id('p_GetOSMemoryPagesPerSecond') is not null)
begin
drop procedure p_GetOSMemoryPagesPerSecond
end
go
create procedure [dbo].[p_GetOSMemoryPagesPerSecond]
	(
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
	)
as
Begin
set transaction isolation level read uncommitted
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

	select  UTCCollectionDateTime,PagesPerSecond from OSStatistics 
	where [OSStatistics].[SQLServerID] = @SQLServerID
	and [OSStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime;
End

GO


