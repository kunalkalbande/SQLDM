if (object_id('p_GetSessionStatisticsForWebConsole') is not null)
begin
drop procedure p_GetSessionStatisticsForWebConsole
end
GO

create procedure [dbo].[p_GetSessionStatisticsForWebConsole]
	@SQLServerID int,
	@HistoryInMinutes int = null,
	@EndTimeUTC datetime = null
as
begin
set transaction isolation level read uncommitted
declare @err int

declare @BeginDateTime datetime
declare @EndDateTime datetime  
--SELECT @EndDateTime= (select max(UTCCollectionDateTime) from [ServerActivity] (NOLOCK) where [SQLServerID] = @SQLServerID)
if (@EndTimeUTC is null)
	SELECT @EndDateTime= (select max(UTCCollectionDateTime) from [ServerActivity] (NOLOCK) where [SQLServerID] = @SQLServerID)
else
	select @EndDateTime = @EndTimeUTC
if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

-- Get session and lock list for requested snapshot
select
	ss.SQLServerID
	,mss.InstanceName
	,ss.UTCCollectionDateTime
	,ss.ResponseTimeInMilliseconds
	,ss.UserProcesses
	,sa.SessionList
	,sa.LockStatistics
from 
	[ServerActivity] sa (NOLOCK)
	LEFT JOIN ServerStatistics ss (NOLOCK) ON sa.SQLServerID = ss.SQLServerID 
		AND sa.UTCCollectionDateTime = ss.UTCCollectionDateTime
	INNER join [MonitoredSQLServers] mss (NOLOCK)
	on sa.SQLServerID = mss.SQLServerID
where 
	sa.[SQLServerID] = @SQLServerID
	and sa.[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime

select @err = @@error
return @err
end
GO


