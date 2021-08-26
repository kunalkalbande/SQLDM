if (object_id('p_GetCustomCounterStatistics') is not null)
begin
drop procedure [p_GetCustomCounterStatistics]
end
go
create procedure [dbo].[p_GetCustomCounterStatistics]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null,
	@CurrentOnly bit = 1
as
begin
set transaction isolation level read uncommitted
declare @err int
declare @BeginDateTime datetime
declare @EndDateTime datetime
declare @InstanceName nvarchar(255)
declare @LinkedCounters table (Metric int)

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [CustomCounterStatistics] where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(minute, -@HistoryInMinutes, @EndDateTime)

-- get the instance name
select @InstanceName = [InstanceName]
from 
	[MonitoredSQLServers]
where
	[SQLServerID] = @SQLServerID

if (@CurrentOnly = 1)
begin
	insert into @LinkedCounters
	select Metric from CustomCounterMap 
		where SQLServerID = @SQLServerID

	insert into @LinkedCounters
	select Metric from CustomCounterTags ct
		join ServerTags st on st.SQLServerId = @SQLServerID and ct.TagId = st.TagId
		where Metric not in (select Metric from @LinkedCounters)
end

-- get the trend data
select 
	[InstanceName]
	,[UTCCollectionDateTime] as [CollectionDateTime]
	,[MetricID]
	,[TimeDeltaInSeconds]
	,[RawValue]
	,[DeltaValue]
	,[ErrorMessage] 
from 
	[CustomCounterStatistics] [CS]
	join [MonitoredSQLServers] [S] on [S].[SQLServerID] = [CS].[SQLServerID]
where 
	[CS].[SQLServerID] = @SQLServerID
	and [CS].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
	and (@CurrentOnly = 0 or [CS].[MetricID] in (select [Metric] from @LinkedCounters))
order by 
	[CS].[UTCCollectionDateTime]

select @err = @@error
return @err
end