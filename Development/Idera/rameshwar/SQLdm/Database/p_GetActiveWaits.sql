if (object_id('p_GetActiveWaits') is not null)
begin
drop procedure p_GetActiveWaits
end
go
create procedure p_GetActiveWaits
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null,
	@CategoryName varchar(120) = null,
	--START SQLdm 10.0 (Sanjali Makkar) : To Add filters of IDs of various parameters
	@WaitTypeID int = null,
	@CategoryID int = null,
	@SQLStatementID int = null,
	@ApplicationNameID int = null,
	@DatabaseID int = null,
	@HostNameID int = null,
	@SessionID  int = null,
	@LoginNameID  int = null,
	--END SQLdm 10.0 (Sanjali Makkar) : To Add filters of IDs of various parameters
	@BeginDateTime datetime = null
as

begin
set transaction isolation level read uncommitted
declare @err int
--declare @BeginDateTime datetime
declare @EndDateTime datetime
declare @secondsPerBucket int
--declare @HistoryInMinutes int

-- Start : SQLdm 8.6(Ankit Srivastava) - Fixing defect for Query Wait Statistics which has DBNULL to string conversion error which occurs when the result contains unnecessary wait_types. 
select @secondsPerBucket = ms.ActiveWaitCollectorCollectionTimeSeconds from MonitoredSQLServers ms (NOLOCK) where [SQLServerID] = @SQLServerID

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL([WaitDuration],0))/1000),[StatementUTCStartTime])) from [ActiveWaitStatistics] (NOLOCK) where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

--print CONVERT(VARCHAR,@EndDateTime,114)
--SQLdm 10.0 (Sanjali Makkar) 
--Commenting out the following code as HistoryInMinutes is not given as input parameter to stored proc now
/*if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)*/

if(@BeginDateTime is null)
	SELECT @BeginDateTime = dateadd(n, -ISNULL(@HistoryInMinutes,30), @EndDateTime)

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
;with 
idr2 as (
select d2.labelStartTime as start, d2.labelEndTime as endtime
 from dbo.fn_GetChartStartAndEndTimeLabels --SQL DM 8.6(Vineet Kumar) - Fixing defect DE41829 - Using this function to get start and end time labels in one function. It reduces the no of CTEs used from two to one.
(@EndDateTime, @secondsPerBucket, datediff(s,@BeginDateTime, @EndDateTime)) d2 ),
 waits as(
select
	idr2.start/* dbo.GetChartBucket([StatementUTCStartTime]) */ as StatementUTCStartTime
	,dateadd(ss, CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL([WaitDuration],0))/1000), [StatementUTCStartTime]) as endtime
	,aw.[WaitTypeID]
	,[WaitType]
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
	,[SessionID]
	,aw.[HostNameID]
	,[HostName]
	,aw.[ApplicationNameID]
	,[ApplicationName]
	,aw.[LoginNameID]
	,[LoginName]
	,aw.DatabaseID
	,[DatabaseName]
	,aw.[SQLStatementID]
	,[SQLStatement]
from idr2 left join [ActiveWaitStatistics] aw  (NOLOCK) on aw.StatementUTCStartTime >= idr2.start 
and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) < idr2.endtime -- no brainer fit in the range
			or (aw.StatementUTCStartTime <= idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= idr2.endtime) -- starts before this bucket so whole bucket in this bucket (bucket start to bucket end)
			or (aw.StatementUTCStartTime <= idr2.start and  (dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) > idr2.start and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) < idr2.endtime)) -- starts before this bucket but ends in the bucket (start of bucket to endtime)
			or ((aw.StatementUTCStartTime >= idr2.start and aw.StatementUTCStartTime < idr2.endtime) and dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= idr2.endtime) -- starts in this bucket but ends after the bucket (some time in bucket till end of bucket)
			or aw.WaitDuration is null
	inner join [WaitTypes] wt (NOLOCK)
	on aw.[WaitTypeID] = wt.[WaitTypeID] inner join [WaitCategories] wc
	on wt.[CategoryID] = wc.[CategoryID] inner join [HostNames] hn
	on aw.[HostNameID] = hn.[HostNameID] inner join [ApplicationNames] an
	on aw.[ApplicationNameID] = an.[ApplicationNameID] inner join [LoginNames] ln
	on aw.[LoginNameID] = ln.[LoginNameID] left join [SQLServerDatabaseNames] dn
	on aw.DatabaseID = dn.DatabaseID left join [AllSQLStatements] sq
	on aw.[SQLStatementID] = sq.[SQLStatementID] 
where aw.[SQLServerID] = @SQLServerID --or aw.SQLServerID is null or aw.SQLServerID = ''
	and (dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL(aw.[WaitDuration],0))/1000),aw.[StatementUTCStartTime]) >= @BeginDateTime 
	and aw.[StatementUTCStartTime] <= @EndDateTime) --or aw.[WaitDuration] is null
	and wc.[Category] = isnull(@CategoryName,wc.[Category])
	and aw.[WaitTypeID] = isnull(@WaitTypeID,aw.[WaitTypeID])
	and wt.[CategoryID] = isnull(@CategoryID,wt.[CategoryID])
	and aw.[SessionID] = isnull(@SessionID,aw.[SessionID])
	and aw.[HostNameID] = isnull(@HostNameID,aw.[HostNameID])
	and aw.[ApplicationNameID] = isnull(@ApplicationNameID,aw.[ApplicationNameID])
	and aw.[LoginNameID] = isnull(@LoginNameID,aw.[LoginNameID])
	and aw.[DatabaseID] = isnull(@DatabaseID,aw.[DatabaseID])
	and aw.[SQLStatementID] = isnull(@SQLStatementID,aw.[SQLStatementID])
	
	and wc.ExcludeFromCollection =0 -- Start : SQLdm 8.6(Ankit Srivastava) - Fixing defect -Excluding the wait_types which belong to Excluded wait_category
	and dn.IsDeleted=0 -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 
	)
	--select * from waits order by 1 asc
select w.StatementUTCStartTime as StatementUTCStartTime
--START SQLdm 10.0 (Sanjali Makkar) : To Add filters of IDs of various parameters
	,isnull([WaitTypeID],'na') as WaitTypeID
	,isnull([WaitType],'na') as WaitType
	,isnull([CategoryID],'na') as CategoryID
	,isnull([Category],'na') as Category
	,isnull([SQLStatementID],'na') as SQLStatementID
	,isnull([SQLStatement],'na') as SQLStatement
	,isnull([ApplicationNameID],'na') as ApplicationNameID
	,isnull([ApplicationName],'na') as ApplicationName
	,isnull([DatabaseID],'na') as DatabaseID
	,isnull([DatabaseName],'na') as DatabaseName
	,isnull([HostNameID],'na') as HostNameID
	,isnull([HostName],'na') as HostName
	,isnull([SessionID],0) as SessionID
	,isnull([LoginNameID],'na') as LoginNameID
	,isnull([LoginName],'na') as LoginName
	--END SQLdm 10.0 (Sanjali Makkar) : To Add filters of IDs of various parameters
	,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),isnull(w.[WaitDuration],0))/@secondsPerBucket) as WaitDuration
	 from waits w (NOLOCK)
	order by w.StatementUTCStartTime  asc
select @err = @@error
return @err
end



