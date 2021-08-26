--SQLDm 10.2 Nishant Adhikari
-- example
-- EXEC p_GetCPUStatistics @SQLServerID=1,@HistoryInMinutes=10

if (object_id('p_GetCPUStatistics') is not null)
begin
drop procedure p_GetCPUStatistics
end
go
create procedure [dbo].[p_GetCPUStatistics]
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
	select SqlCompilations,
		SqlRecompilations,
		Batches,
		Transactions,
		UTCCollectionDateTime,
		TimeDeltaInSeconds,
		SQLServerID
		from ServerStatistics
	where [ServerStatistics].[SQLServerID] = @SQLServerID
	and [ServerStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime;
End

GO


