-- SQL DM 10.2 Nishant
-- example
-- EXEC p_GetWaitStatisticsForDashboard @SQLServerID=1,@HistoryInMinutes=60

if (object_id('p_GetWaitStatisticsForDashboard') is not null)
begin
drop procedure p_GetWaitStatisticsForDashboard
end
go
create procedure p_GetWaitStatisticsForDashboard
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
	select @EndDateTime = (select max(UTCCollectionDateTime) from [WaitStatistics] (NOLOCK) where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

select
	[UTCCollectionDateTime]
	,[TimeDeltaInSeconds]=sum([TimeDeltaInSeconds])
	,[Category]
	,[WaitingTasks] = sum(WaitingTasks)
	,[WaitMilliseconds] = sum([WaitTimeInMilliseconds])
	,[SignalWaitMilliseconds] = sum([WaitTimeInMilliseconds]) - sum([ResourceWaitTimeInMilliseconds])
	,[ResourceWaitMilliseconds] = sum([ResourceWaitTimeInMilliseconds])
from	
	[WaitStatistics] ws (NOLOCK)
	left join 
	[WaitStatisticsDetails] wsd (NOLOCK)
	on ws.[WaitStatisticsID]  = wsd.[WaitStatisticsID]
	inner join [WaitTypes] wt (NOLOCK)
	on wsd.[WaitTypeID] = wt.[WaitTypeID]
	inner join [WaitCategories] wc (NOLOCK)
	on wt.[CategoryID] = wc.[CategoryID]
where 
	ws.[SQLServerID] = @SQLServerID
	and wsd.[WaitStatisticsID] is not null
	and ws.[UTCCollectionDateTime] > @BeginDateTime 
	and ws.[UTCCollectionDateTime] <= @EndDateTime

group by 
	UTCCollectionDateTime,Category

order by
	UTCCollectionDateTime Desc,Category

select @err = @@error
return @err
end

