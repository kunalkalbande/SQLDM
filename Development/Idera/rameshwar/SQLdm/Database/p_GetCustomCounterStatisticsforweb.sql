-- SQLDm 10.2 Nishant Adhikari
-- example
-- EXEC p_GetCustomCounterStatisticsforweb @SQLServerID=1,@HistoryInMinutes=60
if (object_id('p_GetCustomCounterStatisticsforweb') is not null)
begin
drop procedure p_GetCustomCounterStatisticsforweb
end
go
create procedure [dbo].[p_GetCustomCounterStatisticsforweb]
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
	select
	SQLServerID,UTCCollectionDateTime,MetricID,TimeDeltaInSeconds,RawValue,DeltaValue,ErrorMessage,RunTimeInMilliseconds,
	MetricInfo.UTCLastChangeDateTime,MetricType,CalculationType,Enabled,Scale,MetricInfo.Rank,Category,Name,Description,Deleted,ValueType
	from CustomCounterStatistics
	join CustomCounterDefinition on CustomCounterStatistics.MetricID=CustomCounterDefinition.Metric
	join MetricInfo on CustomCounterStatistics.MetricID=MetricInfo.Metric
	join MetricMetaData on CustomCounterStatistics.MetricID=MetricMetaData.Metric
	where SQLServerID=@SQLServerID
	and UTCCollectionDateTime between @BeginDateTime and @EndDateTime;
End

GO