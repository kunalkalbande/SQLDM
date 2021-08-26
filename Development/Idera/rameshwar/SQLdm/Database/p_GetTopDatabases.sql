if (object_id('p_GetTopDatabases') is not null)
begin
drop procedure p_GetTopDatabases
end
go
-- @OrderBy
--	0 - Size
--	1 - Growth Rate
--	2 - Reads 
--	3 - Writes
--	4 - Transactions
create procedure [dbo].p_GetTopDatabases
	@ServerId int,
	@DatabaseNameFilter sysname = null,
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

create table #DatabaseInfo
(
	DatabaseID int primary key clustered, 
	DatabaseName nvarchar(255),
	StartTime datetime,
	EndTime datetime,
	StartTotalDataSizeInKilobytes decimal,
	EndDataFileSizeInKilobytes decimal,
	EndLogFileSizeInKilobytes decimal,
	EndTotalDataSizeInKilobytes decimal,
	UTCStart datetime,
	UTCEnd datetime)


if @DatabaseNameFilter is null or len(@DatabaseNameFilter) = 0
begin
	insert into #DatabaseInfo(DatabaseID,DatabaseName, StartTime, EndTime,UTCStart,UTCEnd)
		select 
			names.DatabaseID,
			DatabaseName,
			min(UTCCollectionDateTime),
			max(UTCCollectionDateTime) ,
			@UTCStart,
			@UTCEnd
		from 
			[dbo].[SQLServerDatabaseNames] names (nolock)
			left join DatabaseSize (nolock) ds
			on ds.DatabaseID = names.DatabaseID
		where
			names.[SQLServerID] = @ServerId
			and (@IncludeSystem = 1 or names.SystemDatabase = 0)
			and dateadd(minute, datediff(minute, 0, [UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
		group by
			names.DatabaseID,
			DatabaseName
end		
else
begin
	insert into #DatabaseInfo(DatabaseID,DatabaseName, StartTime, EndTime,UTCStart,UTCEnd)
		select 
			names.DatabaseID,
			DatabaseName,
			min(UTCCollectionDateTime),
			max(UTCCollectionDateTime) ,
			@UTCStart,
			@UTCEnd
		from 
			[dbo].[SQLServerDatabaseNames] names (nolock)
			left join DatabaseSize (nolock) ds
			on ds.DatabaseID = names.DatabaseID
		where
			names.[SQLServerID] = @ServerId
			and names.[DatabaseName] like @DatabaseNameFilter	
			and (@IncludeSystem = 1 or names.SystemDatabase = 0)
			and dateadd(minute, datediff(minute, 0, [UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
		group by
			names.DatabaseID,
			DatabaseName
end		

update #DatabaseInfo
set 
	StartTotalDataSizeInKilobytes = isnull(DataSizeInKilobytes,0)
from
	#DatabaseInfo di inner join 
	[DatabaseSize] ds (nolock)
	on di.DatabaseID = ds.DatabaseID
where
	[UTCCollectionDateTime] = StartTime
	
update #DatabaseInfo
set 
	StartTotalDataSizeInKilobytes = isnull(DataSizeInKilobytes,0)
from
	#DatabaseInfo di inner join 
	[DatabaseSize] ds (nolock)
	on di.DatabaseID = ds.DatabaseID
where
	[UTCCollectionDateTime] = StartTime
	

update #DatabaseInfo
set 
	EndDataFileSizeInKilobytes = DataFileSizeInKilobytes,
	EndLogFileSizeInKilobytes = LogFileSizeInKilobytes,
	EndTotalDataSizeInKilobytes = isnull(DataSizeInKilobytes,0)
from
	#DatabaseInfo di inner join 
	[DatabaseSize] ds (nolock)
	on di.DatabaseID = ds.DatabaseID
where
	[UTCCollectionDateTime] =  EndTime
	
if (@MinSizeMB is not null)
begin	
	delete from #DatabaseInfo
	where
	isnull(EndTotalDataSizeInKilobytes,0) / 1024 < @MinSizeMB	
end

if (@MinGrowthPercent is not null)
begin
	delete from #DatabaseInfo
	where DatabaseID in(
	select DatabaseID 
	from #DatabaseInfo
	group by DatabaseID
	having
	isnull(max(EndTotalDataSizeInKilobytes / nullif(StartTotalDataSizeInKilobytes,0)),0) < isnull((cast(@MinGrowthPercent as float) / 100) + case @MinGrowthPercent when 0 then 0 else 1 end,1))
end

set rowcount @TopN




select 
	[DatabaseName],
	[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])),
	DataFileSizeMb = max(EndDataFileSizeInKilobytes) / 1024,  -- Max here is to avoid duplication
	LogFileSizeMb = max(EndLogFileSizeInKilobytes) /1024, 
	DataSizeMb = max(EndTotalDataSizeInKilobytes) / 1024,
	GrowthOverPeriod = max(EndTotalDataSizeInKilobytes / nullif(StartTotalDataSizeInKilobytes,0)) - 1,
	KBReadPerSecond = sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),
	KBWrittenPerSecond = sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then ds.TimeDeltaInSeconds else 0 end) ,0),
	TransactionsPerSecond = sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0)
from
	#DatabaseInfo di (nolock)
	left join [DatabaseStatistics] ds (nolock)
	on di.DatabaseID = ds.[DatabaseID]
where 
	ds.[UTCCollectionDateTime] between di.UTCStart and di.UTCEnd
group by
	[DatabaseName]
having
	isnull(sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0),0) >= isnull(@MinTransactionsPerSecond,0)
	and isnull(sum(convert(float,ds.BytesRead) / 1024) / nullif(sum(case when ds.BytesRead is not null then ds.TimeDeltaInSeconds else 0 end),0),0) >= isnull(@MinReadsPerSecond,0)
	and isnull(sum(convert(float,ds.BytesWritten) / 1024) / nullif(sum(case when ds.BytesWritten is not null then ds.TimeDeltaInSeconds else 0 end) ,0),0) >= isnull(@MinWritesPerSecond,0)
order by
	case 
	when @OrderBy = 0 then max(EndTotalDataSizeInKilobytes) / 1024
	when @OrderBy = 1 then max(EndTotalDataSizeInKilobytes / nullif(StartTotalDataSizeInKilobytes,0)) - 1
	when @OrderBy = 2 then sum(convert(float,ds.BytesRead)) / nullif((sum(convert(float,ds.TimeDeltaInSeconds)) / 1024 / 60) ,0)
	when @OrderBy = 3 then sum(convert(float,ds.BytesWritten)) / nullif((sum(convert(float,ds.TimeDeltaInSeconds)) / 1024 / 60) ,0)
	when @OrderBy = 4 then sum(convert(float,ds.Transactions)) / nullif((sum(convert(float,ds.TimeDeltaInSeconds)) / 60) ,0)
	else max(EndTotalDataSizeInKilobytes) / 1024
	end 
	desc

drop table #DatabaseInfo

set rowcount 0

end

Go

grant EXECUTE on [p_GetTopDatabases] to [SQLdmConsoleUser]

go

if (object_id('p_GetTopDatabasesBySize') is not null)
begin
drop procedure p_GetTopDatabasesBySize
end
go
create procedure [dbo].p_GetTopDatabasesBySize
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1
as
begin

	exec p_GetTopDatabases
		@ServerId,
		@DatabaseNameFilter,
		@UTCStart,
		@UTCEnd,
		@UTCOffset,
		@MinSizeMB,
		@MinGrowthPercent,
		@MinTransactionsPerSecond,
		@MinReadsPerSecond,
		@MinWritesPerSecond,
		@TopN,
		0,
		@IncludeSystem
end

Go

grant EXECUTE on [p_GetTopDatabasesBySize] to [SQLdmConsoleUser]

go

if (object_id('p_GetTopDatabasesByGrowth') is not null)
begin
drop procedure p_GetTopDatabasesByGrowth
end
go
create procedure [dbo].p_GetTopDatabasesByGrowth
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1
as
begin

	exec p_GetTopDatabases
		@ServerId,
		@DatabaseNameFilter,
		@UTCStart,
		@UTCEnd,
		@UTCOffset,
		@MinSizeMB,
		@MinGrowthPercent,
		@MinTransactionsPerSecond,
		@MinReadsPerSecond,
		@MinWritesPerSecond,
		@TopN,
		1,
		@IncludeSystem
end

Go

grant EXECUTE on [p_GetTopDatabasesByGrowth] to [SQLdmConsoleUser]

go

if (object_id('p_GetTopDatabasesByReads') is not null)
begin
drop procedure p_GetTopDatabasesByReads
end
go
create procedure [dbo].p_GetTopDatabasesByReads
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1
as
begin

	exec p_GetTopDatabases
		@ServerId,
		@DatabaseNameFilter,
		@UTCStart,
		@UTCEnd,
		@UTCOffset,
		@MinSizeMB,
		@MinGrowthPercent,
		@MinTransactionsPerSecond,
		@MinReadsPerSecond,
		@MinWritesPerSecond,
		@TopN,
		2,
		@IncludeSystem
end

Go

grant EXECUTE on [p_GetTopDatabasesByReads] to [SQLdmConsoleUser]

go

if (object_id('p_GetTopDatabasesByWrites') is not null)
begin
drop procedure p_GetTopDatabasesByWrites
end
go
create procedure [dbo].p_GetTopDatabasesByWrites
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1
as
begin

	exec p_GetTopDatabases
		@ServerId,
		@DatabaseNameFilter,
		@UTCStart,
		@UTCEnd,
		@UTCOffset,
		@MinSizeMB,
		@MinGrowthPercent,
		@MinTransactionsPerSecond,
		@MinReadsPerSecond,
		@MinWritesPerSecond,
		@TopN,
		3,
		@IncludeSystem
end

Go

grant EXECUTE on [p_GetTopDatabasesByWrites] to [SQLdmConsoleUser]

go

if (object_id('p_GetTopDatabasesByTransactions') is not null)
begin
drop procedure p_GetTopDatabasesByTransactions
end
go
create procedure [dbo].p_GetTopDatabasesByTransactions
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MinGrowthPercent bigint = null,
	@MinTransactionsPerSecond bigint = null,
	@MinReadsPerSecond bigint = null,
	@MinWritesPerSecond bigint = null,
	@TopN bigint = 10,
	@IncludeSystem bit = 1
as
begin

	exec p_GetTopDatabases
		@ServerId,
		@DatabaseNameFilter,
		@UTCStart,
		@UTCEnd,
		@UTCOffset,
		@MinSizeMB,
		@MinGrowthPercent,
		@MinTransactionsPerSecond,
		@MinReadsPerSecond,
		@MinWritesPerSecond,
		@TopN,
		4,
		@IncludeSystem
end

Go

grant EXECUTE on [p_GetTopDatabasesByTransactions] to [SQLdmConsoleUser]