if (object_id('p_GetTempdbStatistics') is not null)
begin
drop procedure [p_GetTempdbStatistics]
end
go

-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
--  5 - All Time
CREATE procedure [dbo].[p_GetTempdbStatistics]
	@ServerId int,	
	@UTCStart DateTime,
	@UTCEnd DateTime,	
	@UTCOffset int = 0,
	@Interval int
as
begin
set ansi_warnings off
set ansi_nulls off

set rowcount 0

declare @TempdbDatabaseID int

select @TempdbDatabaseID = DatabaseID
from
	[SQLServerDatabaseNames] names (nolock)	
where
	names.SQLServerID = @ServerId
	and names.DatabaseName = 'tempdb' 	

SELECT * INTO #DS FROM
(
	SELECT DatabaseID,
		UTCCollectionDateTime,
		Transactions,
		TimeDeltaInSeconds,
		BytesRead,
		BytesWritten
	FROM DatabaseStatistics ds
	WHERE ds.DatabaseID = @TempdbDatabaseID
	
	UNION ALL

	SELECT DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalTransactions AS Transactions,
		TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
		TotalBytesRead AS BytesRead,
		TotalBytesWritten AS BytesWritten
	 FROM DatabaseStatisticsAggregation ds
	WHERE ds.DatabaseID = @TempdbDatabaseID
) AS DSAggregation;

create table #DbStatistics(	
	LastCollectioninInterval datetime,
	IntervalNumber int,
	AverageDataFileSizeMb float,
	GrowthOverPeriod decimal(38, 2),
	KBReadPerSecond float,
	KBWrittenPerSecond float,
	TransactionsPerSecond float,
	RowNumber int identity primary key clustered,
	IntervalTemp bigint,
	UserObjectsMB float,
	InternalObjectsMB float,
	VersionStoreMB float,
	MixedExtentsMB float,
	CountDataFiles int
) 

;with
FileCountCTE
as
(
	select
		FileCount = count(tdb.FileID),
		UTCCollectionDateTime,
		FileSizeInKilobytes = sum(FileSizeInKilobytes),
		TimeDeltaInSeconds = min(TimeDeltaInSeconds),
		UserObjectsInKilobytes = sum(UserObjectsInKilobytes),
		InternalObjectsInKilobytes = sum(InternalObjectsInKilobytes),
		VersionStoreInKilobytes = sum(VersionStoreInKilobytes),
		MixedExtentsInKilobytes = sum(MixedExtentsInKilobytes)
	from
		DatabaseFiles files
		inner join TempdbFileData tdb
		on tdb.FileID = files.FileID
	where 
		files.DatabaseID = @TempdbDatabaseID	
		and tdb.[UTCCollectionDateTime] BETWEEN @UTCStart and @UTCEnd
	group by
		UTCCollectionDateTime
)
insert into #DbStatistics(LastCollectioninInterval, IntervalTemp, AverageDataFileSizeMb, KBReadPerSecond, KBWrittenPerSecond, TransactionsPerSecond, UserObjectsMB, InternalObjectsMB, VersionStoreMB, MixedExtentsMB, CountDataFiles)
select 	
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
	AverageDataFileSizeMb = (sum(fc.FileSizeInKilobytes * (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end)) / 
		nullif(sum(case when fc.FileSizeInKilobytes is not null then (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end) else 0 end),0)) / 1024,
	KBReadPerSecond = sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then (case ds.TimeDeltaInSeconds when 0 then 1 else ds.TimeDeltaInSeconds end) else 0 end),0),
	KBWrittenPerSecond = sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then (case ds.TimeDeltaInSeconds when 0 then 1 else ds.TimeDeltaInSeconds end) else 0 end) ,0),
	TransactionsPerSecond = sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then (case ds.TimeDeltaInSeconds when 0 then 1 else ds.TimeDeltaInSeconds end) else 0 end)  ,0),
	UserObjectsMB = round((sum(fc.UserObjectsInKilobytes * (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end)) / 
		nullif(sum(case when fc.UserObjectsInKilobytes is not null then (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end) else 0 end),0)) / 1024, 2),
	InternalObjectsMB = round((sum(fc.InternalObjectsInKilobytes * (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end)) / 
		nullif(sum(case when fc.InternalObjectsInKilobytes is not null then (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end) else 0 end),0)) / 1024, 2),
	VersionStoreMB = round((sum(fc.VersionStoreInKilobytes * (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end)) / 
		nullif(sum(case when fc.VersionStoreInKilobytes is not null then (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end) else 0 end),0)) / 1024, 2),
	MixedExtentsMB = round((sum(fc.MixedExtentsInKilobytes * (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end)) / 
		nullif(sum(case when fc.MixedExtentsInKilobytes is not null then (case fc.TimeDeltaInSeconds when 0 then 1 else fc.TimeDeltaInSeconds end) else 0 end),0)) / 1024, 2),
	CountDataFiles = max(fc.FileCount)
from	
	FileCountCTE fc left join 
	#DS (nolock) ds on (ds.UTCCollectionDateTime = fc.UTCCollectionDateTime)
group by
	 case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else 1 end
	,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end

order by
	IntervalTemp asc	

DROP TABLE #DS;
	
declare @MainPrev float, @rownum int, @intervalcounter int, @previnterval bigint
select 
	@MainPrev = null, 
	@rownum = isnull(min(RowNumber),-1) ,
	@intervalcounter = 0,
	@previnterval = 0
from #DbStatistics
while @rownum > 0
begin

	update #DbStatistics
		set GrowthOverPeriod = (AverageDataFileSizeMb /  nullif(@MainPrev,0) ) - 1,
			IntervalNumber = case when IntervalTemp = @previnterval then @intervalcounter else @intervalcounter + 1 end
	where RowNumber = @rownum

	select
		@MainPrev = AverageDataFileSizeMb,		
		@intervalcounter = case when IntervalTemp = @previnterval then @intervalcounter else @intervalcounter + 1 end,
		@previnterval = IntervalTemp
	from #DbStatistics
	where RowNumber = @rownum

	select @rownum = isnull(min(RowNumber),-1) 
	from #DbStatistics
	where RowNumber > @rownum
end


select 	
	LastCollectioninInterval,
	IntervalNumber,
	AverageDataFileSizeMb,
	GrowthOverPeriod,
	KBReadPerSecond,
	KBWrittenPerSecond,
	TransactionsPerSecond,
	UserObjectsMB,
	InternalObjectsMB,
	VersionStoreMB,
	MixedExtentsMB,
	CountDataFiles
from #DbStatistics
order by
	IntervalNumber asc

end
