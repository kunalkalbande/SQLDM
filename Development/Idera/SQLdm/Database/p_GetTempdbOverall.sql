if (object_id('p_GetTempdbOverall') is not null)
begin
drop procedure [p_GetTempdbOverall]
end
go

-- @OrderBy
--	0 - Size
--	1 - Growth Rate
--	2 - Reads 
--	3 - Writes
--	4 - Transactions
CREATE procedure [dbo].[p_GetTempdbOverall]
	@ServerId int,	
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@OrderBy tinyint = 0,
	@IncludeSystem bit = 1
as
begin

set rowcount 0

declare @TempdbDatabaseID int 

select @TempdbDatabaseID = DatabaseID
from
      [SQLServerDatabaseNames] names (nolock)   
where
      names.SQLServerID = @ServerId
      and names.DatabaseName = 'tempdb'   


declare @StartSize decimal, @EndSize decimal

select	
	@StartSize = sum(tdb.FileSizeInKilobytes)
from
	DatabaseFiles files 
	left join TempdbFileData tdb on (tdb.FileID = files.FileID)	
where
	files.DatabaseID = @TempdbDatabaseID
	and
	tdb.[UTCCollectionDateTime] =
	(		
		select 
			min([UTCCollectionDateTime]) 
		from 
			DatabaseFiles dfiles (nolock)
			inner join TempdbFileData tdb on (dfiles.FileID = tdb.FileID)
		where 
			tdb.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
			and dfiles.DatabaseID = @TempdbDatabaseID
	)			

select	
	@EndSize = sum(tdb.FileSizeInKilobytes )
from	
	DatabaseFiles files 
	left join TempdbFileData tdb on (tdb.FileID = files.FileID)	
where
	files.DatabaseID = @TempdbDatabaseID
	and
	tdb.[UTCCollectionDateTime] =
	(		
		select 
			max([UTCCollectionDateTime]) 
		from 
			DatabaseFiles dfiles (nolock)
			inner join TempdbFileData tdb on (dfiles.FileID = tdb.FileID)
		where 
			tdb.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
			and dfiles.DatabaseID = @TempdbDatabaseID
	)			

SELECT * INTO #DS FROM
(
	SELECT DatabaseID,
		UTCCollectionDateTime,
		Transactions,
		TimeDeltaInSeconds,
		BytesRead,
		BytesWritten				
	FROM DatabaseStatistics ds (nolock)
	where ds.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
	and ds.DatabaseID = @TempdbDatabaseID
	
	UNION ALL
	
	SELECT DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalTransactions AS Transactions,
		TotalTransactions AS Transactions,
		TotalBytesRead AS BytesRead,
		TotalBytesWritten AS BytesWritten		
	FROM DatabaseStatisticsAggregation ds (nolock)
	where ds.MaxUTCCollectionDateTime between @UTCStart and @UTCEnd
	and ds.DatabaseID = @TempdbDatabaseID

) AS DSAggregated; 

set rowcount @TopN

select 
	DatabaseName = 'tempdb',
	[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])),	
	DataSizeMb = max(@EndSize) / 1024,
	GrowthOverPeriod = max(@EndSize / nullif(@StartSize,0)) - 1,
	KBReadPerSecond = sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),
	KBWrittenPerSecond = sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then ds.TimeDeltaInSeconds else 0 end) ,0),
	TransactionsPerSecond = sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0)
from
	#DS ds (nolock)
where 
	isnull(@EndSize,0) / 1024 >= isnull(@MinSizeMB,0)
having
	(@MinGrowthPercent is null or isnull(max(@EndSize / nullif(@StartSize,0)),0) >= isnull((cast(@MinGrowthPercent as float) / 100) + case @MinGrowthPercent when 0 then 0 else 1 end,1)) 
	and isnull(sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0),0) >= isnull(@MinTransactionsPerSecond,0)
	and isnull(sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),0) >= isnull(@MinReadsPerSecond,0)
	and isnull(sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then ds.TimeDeltaInSeconds else 0 end) ,0),0) >= isnull(@MinWritesPerSecond,0)

DROP TABLE #DS;
set rowcount 0

end

