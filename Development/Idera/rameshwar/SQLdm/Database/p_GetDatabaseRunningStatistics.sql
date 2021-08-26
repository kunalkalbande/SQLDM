-- SQLDm 10.2 Nishant Adhikari
-- example
-- EXEC p_GetDatabaseRunningStatistics @SQLServerID=1,@HistoryInMinutes=10
if (object_id('p_GetDatabaseRunningStatistics') is not null)
begin
drop procedure p_GetDatabaseRunningStatistics
end
go
create procedure [dbo].[p_GetDatabaseRunningStatistics]
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
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime);

SELECT * INTO #DS FROM
(
	SELECT ds.DatabaseID,
		UTCCollectionDateTime,
		Transactions,
		LogFlushes,
		NumberReads,
		NumberWrites,
		IoStallMS,
		TimeDeltaInSeconds,
		DatabaseName
	FROM DatabaseStatistics ds (nolock)
	join SQLServerDatabaseNames ssdn (nolock) on 
		ds.DatabaseID=ssdn.DatabaseID 
	where ssdn.SQLServerID= @SQLServerID
		and UTCCollectionDateTime between @BeginDateTime and @EndDateTime

	UNION ALL

	SELECT ds.DatabaseID,
	MaxUTCCollectionDateTime AS UTCCollectionDateTime,
	TotalTransactions AS Transactions,
	TotalLogFlushes AS LogFlushes,
	TotalNumberReads AS NumberReads,
	TotalNumberWrites AS NumberWrites,
	TotalIoStallMS AS IoStallMS,
	TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
	DatabaseName
	FROM DatabaseStatisticsAggregation ds (nolock)
	join SQLServerDatabaseNames ssdn (nolock) on 
		ds.DatabaseID=ssdn.DatabaseID 
	where ssdn.SQLServerID= @SQLServerID
		and MaxUTCCollectionDateTime between @BeginDateTime and @EndDateTime
) AS DSAggregated;


select  UTCCollectionDateTime,
		Transactions,
		LogFlushes,
		NumberReads,
		NumberWrites,
		IoStallMS,
		TimeDeltaInSeconds,
		DatabaseID,
		@SQLServerID,
		DatabaseName
		from #DS 
End

DROP TABLE #DS
GO
