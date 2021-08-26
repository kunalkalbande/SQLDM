if (object_id('[p_GetTopTablesFragmentationChart]') is not null)
begin
drop procedure p_GetTopTablesFragmentationChart
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
--  5 - All Time
-- @OrderBy
-- 0 - Scan Density
-- 1 - Logical Fragmentation
-- 2 - Data Size
create procedure [dbo].p_GetTopTablesFragmentationChart
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MaxSizeMB decimal = null,
	@MinLogicalFragmentation float = null,
	@MaxLogicalFragmentation float = null,
	@TopN bigint = 10,
	@ReturnData tinyint = 0,
	@Interval int
as
begin

set ansi_warnings off
set rowcount 0

create table #PassthroughData (DBName sysname, TableName nvarchar(200), Id bigint)

insert into #PassthroughData
	exec p_GetTopTablesFragmentation
	@ServerId = @ServerId,
	@DatabaseNameFilter = @DatabaseNameFilter,
	@UTCStart = @UTCStart,
	@UTCEnd = @UTCEnd,
	@UTCOffset = @UTCOffset,
	@MinSizeMB = @MinSizeMB,
	@MaxSizeMB = @MaxSizeMB,
	@MinLogicalFragmentation = @MinLogicalFragmentation,
	@MaxLogicalFragmentation = @MaxLogicalFragmentation,
	@TopN =  @TopN,
	@OrderBy = @ReturnData,
	@PassthroughOnly = 1

	select
		ObjectName = DBName + '.' + TableName,
		[LastCollectioninInterval] = dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime]))),
		Data =
			case
			when @ReturnData = 0
				then 100 - sum(ScanDensity * tr.TimeDeltaInSeconds) / nullif(sum(tr.TimeDeltaInSeconds),0)
			when @ReturnData = 1
				then sum(LogicalFragmentation * tr.TimeDeltaInSeconds) / nullif(sum(tr.TimeDeltaInSeconds),0)
			when @ReturnData = 2
				then sum((DataSize + [TextSize] + IndexSize) * growth.TimeDeltaInSeconds) / nullif(sum(growth.TimeDeltaInSeconds),0)  /1024
			end
	from
		#PassthroughData p
		join [TableReorganization] tr (nolock)
		on tr.TableID = p.Id
		join [TableGrowth] growth (nolock)
		on p.Id = growth.[TableID]
	where
		tr.[UTCCollectionDateTime] between @UTCStart and @UTCEnd and
		growth.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
	group by
	DBName + '.' + TableName
	,case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) else 1 end
	,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])) end
	order by 
	ObjectName,
	dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime])))

drop table #PassthroughData

end

Go

grant EXECUTE on [p_GetTopTablesFragmentationChart] to [SQLdmConsoleUser]
