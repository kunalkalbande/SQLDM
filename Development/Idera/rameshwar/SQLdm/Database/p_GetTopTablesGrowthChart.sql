if (object_id('[p_GetTopTablesGrowthChart]') is not null)
begin
drop procedure p_GetTopTablesGrowthChart
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
-- 0 - Number of Rows
-- 1 - Data Size
-- 2 - Text Size
-- 3 - Index Size
-- 4 - Total Size
-- 5 - Table Growth
-- 6 - Row Growth
create procedure [dbo].p_GetTopTablesGrowthChart
	@ServerId int,
	@DatabaseNameFilter sysname = null,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@UTCOffset int = 0,
	@MinSizeMB decimal = null,
	@MaxSizeMB decimal = null,
	@MinRows bigint = null,
	@MaxRows bigint = null,
	@TopN bigint = 10,
	@ReturnData tinyint = 0,
	@Interval int
as
begin

set ansi_warnings off
set rowcount 0

create table #PassthroughData (DBName sysname, TableName nvarchar(200), Id bigint)

insert into #PassthroughData
	exec p_GetTopTablesGrowth
	@ServerId = @ServerId,
	@DatabaseNameFilter = @DatabaseNameFilter,
	@UTCStart = @UTCStart,
	@UTCEnd = @UTCEnd,
	@UTCOffset = @UTCOffset,
	@MinSizeMB = @MinSizeMB,
	@MaxSizeMB = @MaxSizeMB,
	@MinRows = @MinRows,
	@MaxRows = @MaxRows,
	@TopN =  @TopN,
	@OrderBy = @ReturnData,
	@PassthroughOnly = 1

declare @OutputTable table(ObjectName nvarchar(200),LastCollectioninInterval datetime, Data dec(38,10), RowNumber int identity)

insert into @OutputTable (ObjectName, LastCollectioninInterval, Data)
	select
		ObjectName = DBName + '.' + TableName,
		[LastCollectioninInterval] = dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, [UTCCollectionDateTime]))),
		Data =
			case
			when @ReturnData = 0 or @ReturnData = 6
				then ceiling(sum(NumberOfRows * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0))
			when @ReturnData = 1
				then sum(DataSize * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) / 1024
			when @ReturnData = 2
				then sum([TextSize] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) / 1024
			when @ReturnData = 3
				then sum(IndexSize * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) / 1024
			when @ReturnData = 4 or @ReturnData = 5
				then sum((DataSize + [TextSize] + IndexSize) * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) / 1024
			end
	from
		#PassthroughData p
		join [TableGrowth] growth (nolock)
		on p.Id = growth.[TableID]
	where
		[UTCCollectionDateTime] between @UTCStart and @UTCEnd
	group by
	DBName + '.' + TableName
	,case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) else 1 end
	,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, [UTCCollectionDateTime])) end
	order by 
	ObjectName,
	dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, [UTCCollectionDateTime])))

if @ReturnData >= 5
begin
	declare @PrevValue float, @CurValue float, @rownum int, @CurObj nvarchar(255),@PrevObj nvarchar(255)

	select 
		@PrevValue = null,
		@rownum = isnull(min(RowNumber),-1),
		@PrevObj = null,
		@CurObj = null
	from @OutputTable

	while @rownum > 0
	begin
	
		select @CurValue = Data,
			   @CurObj = ObjectName
		from @OutputTable
		where RowNumber = @rownum	

		if @CurObj <> @PrevObj
			set @PrevValue = null

		update @OutputTable
			set Data = isnull((Data /  nullif(@PrevValue,0) ) - 1,0) * 100
		where RowNumber = @rownum

		select @PrevValue = @CurValue,
			   @PrevObj = @CurObj

		select @rownum = isnull(min(RowNumber),-1)
		from @OutputTable
			where RowNumber > @rownum
	end
end

select 
	ObjectName,
	LastCollectioninInterval, 
	Data
from @OutputTable

end

Go

grant EXECUTE on [p_GetTopTablesGrowthChart] to [SQLdmConsoleUser]
