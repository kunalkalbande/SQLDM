if (object_id('p_GetWaitStatistics') is not null)
begin
drop procedure p_GetWaitStatistics
end
go
create procedure p_GetWaitStatistics
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null,
	@CategoryName varchar(120) = null
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
	,[WaitType]
	,[Category]
	,[WaitingTasks]
	,[WaitTimeInMilliseconds]
	,[MaxWaitTimeInMilliseconds]
	,[ResourceWaitTimeInMilliseconds]
	,[SignalWaitTimeInMilliseconds] = [WaitTimeInMilliseconds] - [ResourceWaitTimeInMilliseconds]
	,[WaitingTasksPerSecond] = cast([WaitingTasks] as float) / nullif([TimeDeltaInSeconds],0)
	,[TotalWaitMillisecondsPerSecond] = [WaitTimeInMilliseconds] / nullif([TimeDeltaInSeconds],0)
	,[SignalWaitMillisecondsPerSecond] = cast(([WaitTimeInMilliseconds] - [ResourceWaitTimeInMilliseconds]) as float) / nullif([TimeDeltaInSeconds],0)
	,[ResourceWaitMillisecondsPerSecond] = cast([ResourceWaitTimeInMilliseconds] as float) / nullif([TimeDeltaInSeconds],0)
	,wt.Description
	,wt.HelpLink
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
	and wc.[Category] = isnull(@CategoryName,wc.[Category])



select @err = @@error
return @err
end

