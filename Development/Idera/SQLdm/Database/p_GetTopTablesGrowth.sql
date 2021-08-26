if (object_id('[p_GetTopTablesGrowth]') is not null)
begin
drop procedure p_GetTopTablesGrowth
end
go
-- @OrderBy
-- 0 - Number of Rows
-- 1 - Data Size
-- 2 - Text Size
-- 3 - Index Size
-- 4 - Total Size
-- 5 - Table Growth
-- 6 - Row Growth
create procedure [dbo].p_GetTopTablesGrowth
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
	@OrderBy tinyint = 0,
	@PassthroughOnly bit = 0
as
begin
set ansi_warnings off
set rowcount 0

if len(@DatabaseNameFilter) = 0
	set @DatabaseNameFilter = null

declare @StartSizeTable table(TName nvarchar(500), DBName sysname, StartSizeId int, StartRows bigint, StartSize float)

insert into @StartSizeTable
select
	TName = isnull(SchemaName,'') + '.' + isnull(TableName,''),
	DBName = DatabaseName,
	StartSizeId = tnames.TableID,
	StartRows = NumberOfRows,
	StartSize = (DataSize + [TextSize] + IndexSize)
from
	[SQLServerDatabaseNames] dbnames (nolock)
	join [SQLServerTableNames] tnames (nolock)
	on dbnames.[DatabaseID] = tnames.[DatabaseID]
	join [TableGrowth] growth (nolock)
	on tnames.[TableID] = growth.[TableID]
where
	dbnames.SQLServerID = @ServerId and
	DatabaseName = coalesce(@DatabaseNameFilter,DatabaseName) and
	[UTCCollectionDateTime] =
	(
		select min([UTCCollectionDateTime]) 
		from [TableGrowth] growth2 (nolock)
		where 
		[UTCCollectionDateTime] between @UTCStart and @UTCEnd
		and growth.TableID = growth2.TableID)


set rowcount @TopN

if @PassthroughOnly = 1
begin
	select
		[DatabaseName] = DBName
		,[TableName] = TName
		,[Id] = ss.StartSizeId
	from
	@StartSizeTable ss
	join [TableGrowth] growth (nolock)
	on ss.StartSizeId = growth.[TableID]
where 
	growth.[UTCCollectionDateTime] = 
	(
		select max(growth2.UTCCollectionDateTime) 
		from [TableGrowth] growth2 (nolock)
		where 
			growth2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
			and growth.TableID = growth2.TableID
	)
	and NumberOfRows between isnull(@MinRows,-1) and isnull(@MaxRows,NumberOfRows + 1)
	and (DataSize + [TextSize] + IndexSize) / 1024between isnull(@MinSizeMB,-1) and isnull(@MaxSizeMB,(DataSize + [TextSize] + IndexSize) / 1024 + 1)
order by
	case 
	when @OrderBy = 0 then NumberOfRows
	when @OrderBy = 1 then DataSize / 1024
	when @OrderBy = 2 then [TextSize] / 1024
	when @OrderBy = 3 then IndexSize / 1024
	when @OrderBy = 4 then (DataSize + [TextSize] + IndexSize) / 1024
	when @OrderBy = 5 then isnull(( (DataSize + [TextSize] + IndexSize) / nullif(StartSize,0)) - 1,0)
	when @OrderBy = 6 then isnull((cast(NumberOfRows as float)/ nullif(StartRows,0)) - 1,0)
	else NumberOfRows
	end 
	desc

end
else
begin


select 
	[DatabaseName] = DBName
	,[TableName] = TName
	,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime]) as [LastCollectioninInterval]
	,NumberOfRows 
	,DataSize = DataSize / 1024
	,[TextSize] = [TextSize] / 1024
	,IndexSize = IndexSize  / 1024
	,TotalSize = (DataSize + [TextSize] + IndexSize) / 1024
	,TableGrowth = isnull(( (DataSize + [TextSize] + IndexSize) / nullif(StartSize,0)) - 1,0)
	,RowGrowth = isnull((cast(NumberOfRows as float)/ nullif(StartRows,0)) - 1,0)
from
	@StartSizeTable ss
	join [TableGrowth] growth (nolock)
	on ss.StartSizeId = growth.[TableID]
where 
	growth.[UTCCollectionDateTime] = 
	(
		select max(growth2.UTCCollectionDateTime) 
		from [TableGrowth] growth2 (nolock)
		where 
			growth2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
			and growth.TableID = growth2.TableID
	)
	and NumberOfRows between isnull(@MinRows,-1) and isnull(@MaxRows,NumberOfRows + 1)
	and (DataSize + [TextSize] + IndexSize) / 1024between isnull(@MinSizeMB,-1) and isnull(@MaxSizeMB,(DataSize + [TextSize] + IndexSize) / 1024 + 1)
order by
	case 
	when @OrderBy = 0 then NumberOfRows
	when @OrderBy = 1 then DataSize / 1024
	when @OrderBy = 2 then [TextSize] / 1024
	when @OrderBy = 3 then IndexSize / 1024
	when @OrderBy = 4 then (DataSize + [TextSize] + IndexSize) / 1024
	when @OrderBy = 5 then isnull(( (DataSize + [TextSize] + IndexSize) / nullif(StartSize,0)) - 1,0)
	when @OrderBy = 6 then isnull((cast(NumberOfRows as float)/ nullif(StartRows,0)) - 1,0)
	else NumberOfRows
	end 
	desc

end

end

Go

grant EXECUTE on [p_GetTopTablesGrowth] to [SQLdmConsoleUser]


