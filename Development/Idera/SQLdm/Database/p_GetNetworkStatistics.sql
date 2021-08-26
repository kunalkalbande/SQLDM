-- select PacketsSent,PacketsReceived,UTCCollectionDateTime,TimeDeltaInSeconds from ServerStatistics order by UTCCollectionDateTime desc
-- EXEC p_GetNetworkStatistics @SQLServerID=1,@HistoryInMinutes=10

if (object_id('p_GetNetworkStatistics') is not null)
begin
drop procedure p_GetNetworkStatistics
end
go
create procedure [dbo].[p_GetNetworkStatistics]
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

	select PacketsSent,
	PacketsReceived,
	UTCCollectionDateTime,
	TimeDeltaInSeconds 
	from ServerStatistics
	where SQLServerID = @SQLServerID and UTCCollectionDateTime between @BeginDateTime and @EndDateTime
	order by UTCCollectionDateTime desc;
	
	
End

GO