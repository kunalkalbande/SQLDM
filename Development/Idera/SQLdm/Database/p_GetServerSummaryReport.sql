if (object_id('p_GetServerSummaryReport') is not null)
begin
drop procedure [p_GetServerSummaryReport]
end
go
create procedure [dbo].[p_GetServerSummaryReport]
				@ServerID int,
				@UTCOffset int = null,
				@StartTime DateTime = null,
				@EndTime DateTime = null
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

set @EndTime = ISNULL(@EndTime, GetutcDate())
set @UTCOffset = ISNULL(@UTCOffset, datediff(mi, getutcdate(), getdate()))
set @StartTime = ISNULL(@StartTime, CONVERT(CHAR(8),getutcdate(),112))

-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
	select
		@ServerID as ServerID
		,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]) as [LastCollectioninInterval]
		,CPUActivityPercentage
		,DiskTimePercent
		,[OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes] as [MemoryAvailable]
		,[ResponseTimeInMilliseconds]
		,[SystemProcesses] + UserProcesses as SessionCount
	from
		[ServerStatistics] s1 (nolock)
		left join [MonitoredSQLServers] m (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
		left join [OSStatistics] o (nolock)
		on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
	where
		s1.[SQLServerID] = @ServerID
		and m.[Active] = 1
		and s1.[UTCCollectionDateTime] between @StartTime and @EndTime
	order by
		s1.[UTCCollectionDateTime]
end