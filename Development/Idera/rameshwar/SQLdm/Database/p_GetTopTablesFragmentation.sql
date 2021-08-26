if (object_id('[p_GetTopTablesFragmentation]') is not null)
begin
drop procedure p_GetTopTablesFragmentation
end
go
-- @OrderBy
-- 0 - Scan Density
-- 1 - Logical Fragmentation
-- 2 - Total Size
create procedure [dbo].p_GetTopTablesFragmentation
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
	@OrderBy tinyint = 0,
	@PassthroughOnly bit = 0
as
begin

if len(@DatabaseNameFilter) = 0
	set @DatabaseNameFilter = null

set rowcount @TopN

if @PassthroughOnly = 1
begin
	select
		DatabaseName
		,[TableName] = isnull(SchemaName,'') + '.' + isnull(TableName,'')
		,[Id] = tnames.TableID
	from
		[SQLServerDatabaseNames] dbnames (nolock)
		join [SQLServerTableNames] tnames (nolock)
		on dbnames.[DatabaseID] = tnames.[DatabaseID]
		join [TableReorganization] tr (nolock)
		on tr.TableID = tnames.TableID
		join [TableGrowth] growth (nolock)
		on tnames.[TableID] = growth.[TableID]
	where 
		dbnames.SQLServerID = @ServerId and
		DatabaseName = coalesce(@DatabaseNameFilter,DatabaseName)
		and tr.UTCCollectionDateTime = 
			(
				select max(tr2.UTCCollectionDateTime) 
				from TableReorganization tr2 (nolock)
				where 
					tr2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd and
					tr.TableID = tr2.TableID
			)
		and
		growth.[UTCCollectionDateTime] =
			(
				select max(growth2.UTCCollectionDateTime) 
				from [TableGrowth] growth2 (nolock)
				where 
					growth2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd and
					growth.TableID = growth2.TableID
			)
		and isnull(LogicalFragmentation,0) between isnull(@MinLogicalFragmentation,-1) and isnull(@MaxLogicalFragmentation,100)
		and ( DataSize + [TextSize] + IndexSize )  / 1024 between isnull(@MinSizeMB,-1) and isnull(@MaxSizeMB,(DataSize + [TextSize] + IndexSize ) / 1024 + 1)
	order by
		case 
		when @OrderBy = 0 then 100 - ScanDensity
		when @OrderBy = 1 then LogicalFragmentation
		else  (DataSize + [TextSize] + IndexSize) / 1024
		end 
		desc
end
else
begin

	select 
		DatabaseName
		,[TableName] = isnull(SchemaName,'') + '.' + isnull(TableName,'')
		,dateadd(mi, @UTCOffset, tr.[UTCCollectionDateTime]) as LastCollectioninInterval
		,ScanDensity = 100 - ScanDensity
		,LogicalFragmentation = LogicalFragmentation
		,TotalSize = (DataSize + [TextSize] + IndexSize) / 1024
	from
		[SQLServerDatabaseNames] dbnames (nolock)
		join [SQLServerTableNames] tnames (nolock)
		on dbnames.[DatabaseID] = tnames.[DatabaseID]
		join [TableReorganization] tr (nolock)
		on tr.TableID = tnames.TableID
		join [TableGrowth] growth (nolock)
		on tnames.[TableID] = growth.[TableID]
	where 
		dbnames.SQLServerID = @ServerId and
		DatabaseName = coalesce(@DatabaseNameFilter,DatabaseName)
		and tr.UTCCollectionDateTime = 
			(
				select max(tr2.UTCCollectionDateTime) 
				from TableReorganization tr2 (nolock)
				where 
					tr2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd and
					tr.TableID = tr2.TableID
			)
		and
		growth.[UTCCollectionDateTime] =
			(
				select max(growth2.UTCCollectionDateTime) 
				from [TableGrowth] growth2 (nolock)
				where 
					growth2.[UTCCollectionDateTime] between @UTCStart and @UTCEnd and
					growth.TableID = growth2.TableID
			)
		and (@OrderBy > 0 or ScanDensity is not null)
		and isnull(LogicalFragmentation,0) between isnull(@MinLogicalFragmentation,-1) and isnull(@MaxLogicalFragmentation,100)
		and ( DataSize + [TextSize] + IndexSize )  / 1024 between isnull(@MinSizeMB,-1) and isnull(@MaxSizeMB,(DataSize + [TextSize] + IndexSize ) / 1024 + 1)
	order by
		case 
		when @OrderBy = 0 then 100 - ScanDensity
		when @OrderBy = 1 then LogicalFragmentation
		else (DataSize + [TextSize] + IndexSize) / 1024
		end 
		desc

end

end

Go

grant EXECUTE on [p_GetTopTablesFragmentation] to [SQLdmConsoleUser]

