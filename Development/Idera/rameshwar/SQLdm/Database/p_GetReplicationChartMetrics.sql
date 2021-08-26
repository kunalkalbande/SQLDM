if (object_id('p_GetReplicationChartMetrics') is not null)
begin
drop procedure p_GetReplicationChartMetrics
end
go

create procedure [dbo].[p_GetReplicationChartMetrics]
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
	select @EndDateTime = (select max(UTCCollectionDateTime) 
	from [ServerStatistics] (nolock) 
	where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)


select [UTCCollectionDateTime],
ReplicationLatencyInSeconds, 
DistributionLatencyInSeconds,
ReplicationSubscribed,
ReplicationUnsubscribed,
ReplicationUndistributed
from
	[ServerStatistics] (nolock)
	left join [MonitoredSQLServers] (nolock)
	on [ServerStatistics].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
where
	[ServerStatistics].[SQLServerID] = @SQLServerID
	and [ServerStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
order by 
	[ServerStatistics].[UTCCollectionDateTime]
end	

GO



