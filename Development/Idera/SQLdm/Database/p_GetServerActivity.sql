if (object_id('p_GetServerActivity') is not null)
begin
drop procedure p_GetServerActivity
end
go
create procedure p_GetServerActivity
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@ColumnBitMask int = 7, -- SQLServerID + UTCCollectionDateTime + StateOverview 
	@RefreshType int = 0
as
begin

-- column bit mask
-- 1   = SQLServerID (always returned)
-- 2   = UTCCollectionDateTime (always returned)
-- 4   = StateOverview
-- 8   = SystemProcesses
-- 16  = SessionList
-- 32  = LockStatistics
-- 64  = LockList
-- 128 = ResponseTime

declare @err int
declare @start datetime
declare @end datetime

select @start = @StartDateTime
if (@start is null)
	select @start = min([UTCCollectionDateTime]) from ServerActivity (NOLOCK) where [SQLServerID] = @SQLServerID

select @end = @EndDateTime
if (@end is null)
	select @end = max([UTCCollectionDateTime]) from ServerActivity (NOLOCK)  where [SQLServerID] = @SQLServerID

-- if no timestamp is supplied then return list of available snapshots
if (@ColumnBitMask & 128 = 0)
begin
	SELECT 
		[SQLServerID],
		[UTCCollectionDateTime],
		case when @ColumnBitMask & 4 <> 0 then [StateOverview] else null end,
		case when @ColumnBitMask & 8 <> 0 then [SystemProcesses] else null end,
		case when @ColumnBitMask & 16 <> 0 then [SessionList] else null end,
		case when @ColumnBitMask & 32 <> 0 then [LockStatistics] else null end,
		case when @ColumnBitMask & 64 <> 0 then [LockList] else null end,
		null
	FROM [ServerActivity] (NOLOCK)
	WHERE [SQLServerID] = @SQLServerID and 
		isnull([RefreshType],0) = @RefreshType and
		[UTCCollectionDateTime] >= @start and
		[UTCCollectionDateTime] <= @end
	ORDER BY [UTCCollectionDateTime]
end
else
begin
	SELECT 
		[ServerActivity].[SQLServerID],
		[ServerActivity].[UTCCollectionDateTime],
		case when @ColumnBitMask & 4 <> 0 then [ServerActivity].[StateOverview] else null end,
		case when @ColumnBitMask & 8 <> 0 then [ServerActivity].[SystemProcesses] else null end,
		case when @ColumnBitMask & 16 <> 0 then [ServerActivity].[SessionList] else null end,
		case when @ColumnBitMask & 32 <> 0 then [ServerActivity].[LockStatistics] else null end,
		case when @ColumnBitMask & 64 <> 0 then [ServerActivity].[LockList] else null end,
		[ServerStatistics].[ResponseTimeInMilliseconds]
	FROM [ServerActivity] (NOLOCK)
	LEFT JOIN [ServerStatistics] (NOLOCK) ON 
		[ServerStatistics].[SQLServerID] = [ServerActivity].[SQLServerID] and
		[ServerStatistics].[UTCCollectionDateTime] = [ServerActivity].[UTCCollectionDateTime]
	WHERE [ServerActivity].[SQLServerID] = @SQLServerID and 
		isnull([RefreshType],0) = @RefreshType and
		[ServerActivity].[UTCCollectionDateTime] >= @start and
		[ServerActivity].[UTCCollectionDateTime] <= @end
	ORDER BY [ServerActivity].[UTCCollectionDateTime]
end

SELECT @err = @@error
	
RETURN @err

end
