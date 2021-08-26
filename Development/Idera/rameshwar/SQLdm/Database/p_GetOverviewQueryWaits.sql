/*
SQLdm(10.2) srishti purohit : 
New SP to create new API for GetQueryWaitStatisticsForInstanceOverview on instance Overview page
The SP gives Wait duration wrt wait Category for selected instance
THe data will be return data accoridng to summaryLevel/duration(secs) send. Data following under given duration and BeginDateTime, is clubbed to return only one row with startTime stamp for respective duration. SP also support history range select.
*/

if (object_id('p_GetOverviewQueryWaits') is not null)
begin
drop procedure p_GetOverviewQueryWaits
end
go
create procedure p_GetOverviewQueryWaits
	@SQLServerID int,
	@EndDateTime datetime,
	@HistoryInMinutes int = null,
	@BeginDateTime datetime,
	@SummaryLevel int
as

begin
set transaction isolation level read uncommitted
declare @err int
declare @DateTimeToPopulateTempTable datetime
declare @TempEndDateTime datetime
declare @secondsPerBucket int

select @secondsPerBucket = ms.ActiveWaitCollectorCollectionTimeSeconds from MonitoredSQLServers ms (NOLOCK) where [SQLServerID] = @SQLServerID

IF(@HistoryInMinutes IS NULL)
BEGIN
	SELECT @HistoryInMinutes = DATEDIFF(n, @BeginDateTime, @EndDateTime);
END

-- Start : SQL DM 8.6(Vineet Kumar) - Fixing defect DE41829. This code decides the @secondsPerBucket at runtime depending on the amount of data to be fetched. 
		-- @secondsPerBucket is decided on runtime to limit the no of records to fetch so that they can be displayed on graph in readable form.
if(@HistoryInMinutes=24*60)
	select @secondsPerBucket = 600 --If data is required for one day (i.e. 24*60 minutes), It will override the default value of @secondsPerBucket to 600 (i.e. 10 minutes)
else if(@HistoryInMinutes=12*60)
	select @secondsPerBucket = 300 -- For 12 hours, @secondsPerBucket=300 (i.e. 5 minutes)
else if(@HistoryInMinutes=8*60)
	select @secondsPerBucket = 180 -- For 8 hours, @secondsPerBucket=180 (i.e. 3 minutes)
else if(@HistoryInMinutes=4*60)
	select @secondsPerBucket = 120 -- For 4 hours, @secondsPerBucket=120 (i.e. 2 minutes)
else if(@HistoryInMinutes=2*60)
	select @secondsPerBucket = 60 -- For 2 hours, @secondsPerBucket=60 (i.e. 1 minutes)
if(@HistoryInMinutes*60/@secondsPerBucket > 200)
	select @secondsPerBucket = @HistoryInMinutes * 60/600; -- If data is being retrieved for different time period than the above mentioned. If no of distinct time durations is greater than 2000,  @secondsPerBucket will be set to 10 minutes. Otherwise will use the default  @secondsPerBucket
-- End : SQL DM 8.6(Vineet Kumar) - Fixing defect DE41829.


create table #TempDurationForSelfJOin
( 
	GraphStartDate DateTime, 
	GraphEndDate DatetIme, 
	TotalDuration int
)
select @DateTimeToPopulateTempTable = @BeginDateTime;
	
WHILE @DateTimeToPopulateTempTable < @EndDateTime
BEGIN
	SELECT @TempEndDateTime = DATEADD(SECOND, @SummaryLevel, @DateTimeToPopulateTempTable)
	Insert Into #TempDurationForSelfJOin Values(@DateTimeToPopulateTempTable, CASE when @TempEndDateTime > @EndDateTime then @EndDateTime ELSE @TempEndDateTime END, 0) 
	SELECT @DateTimeToPopulateTempTable = @TempEndDateTime;
END

;with 
idr2 as (
select d2.labelStartTime as start, d2.labelEndTime as endtime
 from dbo.fn_GetChartStartAndEndTimeLabels --SQL DM 8.6(Vineet Kumar) - Fixing defect DE41829 - Using this function to get start and end time labels in one function. It reduces the no of CTEs used from two to one.
(@EndDateTime, @secondsPerBucket, datediff(s,@BeginDateTime, @EndDateTime)) d2 ),
 waits as(
select
	idr2.start/* dbo.GetChartBucket([StatementUTCStartTime]) */ as StatementUTCStartTime
	,dateadd(ss, CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL([WaitDuration],0))/1000), [StatementUTCStartTime]) as endtime
	,wt.[CategoryID]
	,[Category]
	,[WaitDuration] = case when WaitDuration is null then 0 
	when aw.StatementUTCStartTime >= idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) <= idr2.endtime then WaitDuration 
		when (aw.StatementUTCStartTime <= idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= idr2.endtime) 
			then DATEDIFF(ss,idr2.start,idr2.endtime)*1000
		when (aw.StatementUTCStartTime <= idr2.start and  (dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) > idr2.start and 
			dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) < idr2.endtime)) 
			then DATEDIFF(ss,idr2.start,dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]))*1000
		when ((aw.StatementUTCStartTime >= idr2.start and aw.StatementUTCStartTime < idr2.endtime) and 
			dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) > idr2.endtime) 
			then  DATEDIFF(ss,aw.StatementUTCStartTime, idr2.endtime)*1000
		else 0 
		end
from idr2 left join [ActiveWaitStatistics] aw  (NOLOCK) on aw.StatementUTCStartTime >= idr2.start 
and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) < idr2.endtime -- no brainer fit in the range
			or (aw.StatementUTCStartTime <= idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= idr2.endtime) -- starts before this bucket so whole bucket in this bucket (bucket start to bucket end)
			or (aw.StatementUTCStartTime <= idr2.start and  (dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) > idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) < idr2.endtime)) -- starts before this bucket but ends in the bucket (start of bucket to endtime)
			or ((aw.StatementUTCStartTime >= idr2.start and aw.StatementUTCStartTime < idr2.endtime) and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= idr2.endtime) -- starts in this bucket but ends after the bucket (some time in bucket till end of bucket)
			or aw.WaitDuration is null
	inner join [WaitTypes] wt (NOLOCK)
	on aw.[WaitTypeID] = wt.[WaitTypeID] inner join [WaitCategories] wc
	on wt.[CategoryID] = wc.[CategoryID]
	where aw.[SQLServerID] = @SQLServerID --or aw.SQLServerID is null or aw.SQLServerID = ''
	and (dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= @BeginDateTime 
	and aw.[StatementUTCStartTime] <= @EndDateTime)	
	and wc.ExcludeFromCollection =0
	)

select a.GraphStartDate as StatementUTCStartTime
	,isnull([CategoryID],'') as CategoryID
	,isnull([Category],'') as Category
	,SUM(CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),isnull(w.[WaitDuration],0))/@secondsPerBucket)) as WaitDuration
	 from waits w (NOLOCK)
	 INNER JOIN #TempDurationForSelfJOin a on w.StatementUTCStartTime > a.GraphStartDate
	 --INNER JOIN #TempDurationForSelfJOin b ON a.GraphEndDate = b.GraphStartDate
	 WHERE w.StatementUTCStartTime BETWEEN a.GraphStartDate AND a.GraphEndDate
	 GROUP BY a.GraphStartDate, CategoryID, Category
	 order by a.GraphStartDate  asc
--Dropping Temp table
DROP TABLE #TempDurationForSelfJOin;

select @err = @@error
return @err
end



