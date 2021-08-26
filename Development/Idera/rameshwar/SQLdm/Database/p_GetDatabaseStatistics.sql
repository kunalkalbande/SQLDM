if (object_id('p_GetDatabaseStatistics') is not null)
begin
drop procedure p_GetDatabaseStatistics
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
--  5 - All Time
create procedure [dbo].[p_GetDatabaseStatistics]
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@CompareDatabaseNameFilter sysname = null,
	@CompareStartRange DateTime = null,
	@UTCOffset int = 0,
	@Interval int
as
begin
set ansi_warnings off
set ansi_nulls off


set rowcount 0

declare @CompareEndRange DateTime
set @CompareStartRange = isnull(@CompareStartRange,@UTCStart)
set @CompareEndRange = dateadd(mi, datediff(mi, @UTCStart, @UTCEnd), @CompareStartRange)
declare @StartDateValue datetime = dbo.fn_RoundDateTime(@Interval,@CompareStartRange) 
declare @EndDateValue datetime = dbo.fn_RoundDateTime(@Interval,@CompareEndRange)

SELECT * INTO #DS FROM
(
	SELECT DatabaseStatisticsID,
		ds.DatabaseID,
		UTCCollectionDateTime,
		Transactions,
		LogFlushWaits,
		LogFlushes,
		LogKilobytesFlushed,
		LogCacheReads,
		LogCacheHitRatio,
		TimeDeltaInSeconds,
		BytesRead,
		BytesWritten,
		DatabaseSizeTime,
		LastBackupDateTime		     
  FROM [SQLServerDatabaseNames] names (nolock)

  LEFT JOIN DatabaseStatistics ds (nolock) ON names.[DatabaseID] = ds.[DatabaseID]
	WHERE
	--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Applying roundFunction to variables instead of column fields and added where caluse to reduce the scope
	ds.[UTCCollectionDateTime] BETWEEN @StartDateValue and @EndDateValue
	and LogFlushes is not null
	AND names.[SQLServerID] = @ServerId   AND names.[DatabaseName] = @DatabaseNameFilter
	--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Applying roundFunction to variables instead of column fields and added where caluse to reduce the scope

	UNION ALL

	SELECT AggregatedDatabaseStatisticsID AS DatabaseStatisticsID, -- Adding AggregatedDatabaseStatisticsID used only for not null condition
		ds.DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalTransactions AS Transactions,
		TotalLogFlushWaits AS LogFlushWaits,
		TotalLogFlushes AS LogFlushes,
		TotalLogKilobytesFlushed AS LogKilobytesFlushed,
		TotalLogCacheReads AS LogCacheReads,
		TotalLogCacheHitRatio AS LogCacheHitRatio,
		TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
		TotalBytesRead AS BytesRead,
		TotalBytesWritten AS BytesWritten,
		MaxDatabaseSizeTime AS DatabaseSizeTime,
		MaxLastBackupDateTime AS LastBackupDateTime
	FROM [SQLServerDatabaseNames] names (nolock)
	LEFT JOIN DatabaseStatisticsAggregation ds (nolock) ON names.[DatabaseID] = ds.[DatabaseID]
	WHERE
	--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Applying roundFunction to variables instead of column fields and added where caluse to reduce the scope
	ds.[MaxUTCCollectionDateTime] BETWEEN @StartDateValue and @EndDateValue
	and TotalLogFlushes is not null
	AND names.[SQLServerID] = @ServerId   AND names.[DatabaseName] = @DatabaseNameFilter
	--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Applying roundFunction to variables instead of column fields and added where caluse to reduce the scope

) AS DSAggregation;

if (select isnull(object_id('tempdb..#DBStatistics'), 0)) <> 0 
begin
	drop table #DBStatistics
end

create table #DBStatistics(
	DatabaseName nvarchar(257),
	LastCollectioninInterval datetime,
	IntervalNumber int,
	AverageDataFileSizeMb float,
	AverageLogFileSizeMb float,
	AverageDataSizeMb float,
	GrowthOverPeriod decimal(38, 19),
	KBReadPerSecond float,
	KBWrittenPerSecond float,
	TransactionsPerSecond float,
	W_LogCacheHitRatio float,
	W_LogCacheReads bigint,
	W_LogFileSizeInKilobytes float,
	W_LogFlushWaits bigint,
	W_LogFlushes bigint,
	W_LogKilobytesFlushed bigint,
	W_PercentLogSpace float,
	RowNumber int identity,
	IntervalTemp bigint,
	LastBackupDateTime datetime -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
) 

insert into #DBStatistics(DatabaseName, LastCollectioninInterval, IntervalTemp, AverageDataFileSizeMb, AverageLogFileSizeMb, AverageDataSizeMb, KBReadPerSecond,
			KBWrittenPerSecond, TransactionsPerSecond, W_LogCacheHitRatio, W_LogCacheReads, W_LogFileSizeInKilobytes, W_LogFlushWaits,
			W_LogFlushes, W_LogKilobytesFlushed, W_PercentLogSpace, LastBackupDateTime)

select 
	[DatabaseName] = 
		case 
			when @DatabaseNameFilter = @CompareDatabaseNameFilter
		then 
			DatabaseName + ' (Base)'
		else DatabaseName end,
	[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])),
	IntervalTemp = case
				 when isnull(@Interval,5) = 5
						then 0
				 when isnull(@Interval,5) = 4
						then datediff(yyyy, @UTCStart, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 3
						then datediff(mm, @UTCStart, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 2
						then datediff(dd, @UTCStart, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 1
						then datediff(hh, @UTCStart, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) =  0
						then datediff(mi, @UTCStart, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
				 end,
	-- Note - isnull(nullif(dz.TimeDeltaInSeconds,0),1)  is used to cause 1 to be returned for the weighted average for size data if the time delta is 0
	-- This is to prevent division by zero and prevent loss of valid data (size data does not have to be converted to a rate)				 
	AverageDataFileSizeMb = (sum(DataFileSizeInKilobytes * isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when DataFileSizeInKilobytes is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	AverageLogFileSizeMb = (sum(LogFileSizeInKilobytes * isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when LogFileSizeInKilobytes is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	AverageDataSizeMb = (sum((DataSizeInKilobytes)* isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when (DataSizeInKilobytes) is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	KBReadPerSecond = sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),
	KBWrittenPerSecond = sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then ds.TimeDeltaInSeconds else 0 end) ,0),
	TransactionsPerSecond = sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0),
	W_LogCacheHitRatio =		sum(LogCacheHitRatio * ds.TimeDeltaInSeconds) / nullif(sum(case when LogCacheHitRatio is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_LogCacheReads =			sum(convert(float, LogCacheReads)) / nullif(sum(case when LogCacheReads is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFileSizeInKilobytes =	sum(LogFileSizeInKilobytes * ds.TimeDeltaInSeconds) / nullif(sum(case when LogFileSizeInKilobytes is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFlushWaits =			sum(LogFlushWaits * ds.TimeDeltaInSeconds) / nullif(sum(case when LogFlushWaits is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFlushes =				sum(convert(float, LogFlushes)) / nullif(sum(case when LogFlushes is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_LogKilobytesFlushed =		sum(LogKilobytesFlushed * ds.TimeDeltaInSeconds) / nullif(sum(case when LogKilobytesFlushed is not null then ds.TimeDeltaInSeconds else 0 end),0),
	W_PercentLogSpace =			sum(PercentLogSpace * ds.TimeDeltaInSeconds) / nullif(sum(case when PercentLogSpace is not null then ds.TimeDeltaInSeconds else 0 end),0),
	[LastBackupDateTime]  = max(dateadd(mi, @UTCOffset, ds.[LastBackupDateTime]))
from
	[SQLServerDatabaseNames] names (nolock)
	left join #DS ds (nolock)
	on names.[DatabaseID] = ds.[DatabaseID]
	left join [DatabaseSize] dz (nolock)
	on names.[DatabaseID] = dz.[DatabaseID]
	and ds.DatabaseSizeTime = dz.UTCCollectionDateTime
where 
	names.[SQLServerID] = @ServerId
	and 
	names.[DatabaseName] = @DatabaseNameFilter
group by
	[DatabaseName], 
--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Implementing bucketing to avoid too many function calls
case @Interval 
when 4 then convert(char(4), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 3 then convert(char(7), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 2 then convert(char(10), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 1 then convert(char(13), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 0 then convert(char(16), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
else '1' end
--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Implementing bucketing to avoid too many function calls
union
select 
	[DatabaseName] = 
		case 
			when @DatabaseNameFilter = @CompareDatabaseNameFilter 
		then 
			DatabaseName + ' (Compare)'
		else DatabaseName end,
	[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])),
	IntervalTemp = case
					 when isnull(@Interval,5) = 5
							then 0
					 when isnull(@Interval,5) = 4
							then datediff(yyyy, @CompareStartRange, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 3
							then datediff(mm, @CompareStartRange, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 2
							then datediff(dd, @CompareStartRange, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 1
							then datediff(hh, @CompareStartRange, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 0
							then datediff(mi, @CompareStartRange, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])))
					 end,
	-- Note - isnull(nullif(dz.TimeDeltaInSeconds,0),1)  is used to cause 1 to be returned for the weighted average for size data if the time delta is 0
	-- This is to prevent division by zero and prevent loss of valid data (size data does not have to be converted to a rate)					 
	AverageDataFileSizeMb = (sum(DataFileSizeInKilobytes * isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when DataFileSizeInKilobytes is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	AverageLogFileSizeMb = (sum(LogFileSizeInKilobytes * isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when LogFileSizeInKilobytes is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	AverageDataSizeMb = (sum((DataSizeInKilobytes)* isnull(nullif(dz.TimeDeltaInSeconds,0),1)  ) / 
		nullif(sum(case when (DataSizeInKilobytes) is not null then isnull(nullif(dz.TimeDeltaInSeconds,0),1)   else 0 end),0)) / 1024,
	KBReadPerSecond = sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),
	KBWrittenPerSecond = sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then  ds.TimeDeltaInSeconds else 0 end) ,0),
	TransactionsPerSecond = sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then  ds.TimeDeltaInSeconds else 0 end)  ,0),
	W_LogCacheHitRatio =		sum(LogCacheHitRatio *  ds.TimeDeltaInSeconds) / nullif(sum(case when LogCacheHitRatio is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_LogCacheReads =			sum(convert(float, LogCacheReads)) / nullif(sum(case when LogCacheReads is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFileSizeInKilobytes =	sum(LogFileSizeInKilobytes *  ds.TimeDeltaInSeconds) / nullif(sum(case when LogFileSizeInKilobytes is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFlushWaits =			sum(LogFlushWaits *  ds.TimeDeltaInSeconds) / nullif(sum(case when LogFlushWaits is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_LogFlushes =				sum(convert(float, LogFlushes)) / nullif(sum(case when LogFlushes is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_LogKilobytesFlushed =		sum(LogKilobytesFlushed *  ds.TimeDeltaInSeconds) / nullif(sum(case when LogKilobytesFlushed is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	W_PercentLogSpace =			sum(PercentLogSpace *  ds.TimeDeltaInSeconds) / nullif(sum(case when PercentLogSpace is not null then  ds.TimeDeltaInSeconds else 0 end),0),
	[LastBackupDateTime]  = max(dateadd(mi, @UTCOffset, ds.[LastBackupDateTime]))
from
	[SQLServerDatabaseNames] names (nolock)
	left join #DS ds (nolock)
	on names.[DatabaseID] = ds.[DatabaseID]
	left join [DatabaseSize] dz (nolock)
	on names.[DatabaseID] = dz.[DatabaseID]
	and ds.DatabaseSizeTime = dz.UTCCollectionDateTime
where 
	names.[SQLServerID] = @ServerId
	and 
	names.[DatabaseName] = @CompareDatabaseNameFilter
group by
	[DatabaseName], 
--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Implementing bucketing to avoid too many function calls
case @Interval 
when 4 then convert(char(4), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 3 then convert(char(7), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 2 then convert(char(10), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 1 then convert(char(13), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
when 0 then convert(char(16), dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]), 120) 
else '1' end
--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Implementing bucketing to avoid too many function calls
order by
	IntervalTemp asc, 
	[DatabaseName] asc;

With Ranks
As (Select RowNumber, AverageDataSizeMb,
		MainPrev = Cast(Null as float),
		ComparePrev = Cast(Null as float)
    From #DBStatistics
	Where RowNumber = 1
	Union All
	Select R.RowNumber + 1, DBS.AverageDataSizeMb,
		MainPrev = case when [DatabaseName] = @DatabaseNameFilter or [DatabaseName] = @DatabaseNameFilter + ' (Base)' then R.AverageDataSizeMb
			else Null
			end,
		ComparePrev = case when [DatabaseName] = @CompareDatabaseNameFilter or [DatabaseName] = @CompareDatabaseNameFilter + ' (Compare)' then R.AverageDataSizeMb
			else Null
			end
    From #DBStatistics DBS
	Inner Join Ranks R On DBS.RowNumber = R.RowNumber + 1)
select 
	DatabaseName,
	[LastCollectioninInterval] = dbo.fn_RoundDateTime(@Interval, LastCollectioninInterval),
	IntervalNumber = dense_rank() Over (Order By IntervalTemp),
	AverageDataFileSizeMb,
	AverageLogFileSizeMb,
	DBS.AverageDataSizeMb,
	GrowthOverPeriod = Cast((DBS.AverageDataSizeMb / 
		nullif(case when [DatabaseName] = @DatabaseNameFilter then R.MainPrev else R.ComparePrev end,0) ) - 1
		As decimal(38, 19)),
	KBReadPerSecond,
	KBWrittenPerSecond,
	TransactionsPerSecond,
	W_LogCacheHitRatio,
	W_LogCacheReads,
	W_LogFileSizeInKilobytes,
	W_LogFlushWaits,
	W_LogFlushes,
	W_LogKilobytesFlushed,
	W_PercentLogSpace,
	LastBackupDateTime
from #DBStatistics DBS
Inner Join Ranks R On R.RowNumber = DBS.RowNumber
order by
	IntervalNumber asc, 
	[DatabaseName] asc
OPTION (MAXRECURSION 0);

DROP TABLE #DS;
end
Go

grant EXECUTE on [p_GetDatabaseStatistics] to [SQLdmConsoleUser]