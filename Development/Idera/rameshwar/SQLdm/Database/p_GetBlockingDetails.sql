if (object_id('[p_GetBlockingDetails]') is not null)
begin
drop procedure [p_GetBlockingDetails]
end
go
create procedure [dbo].[p_GetBlockingDetails]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
begin
set transaction isolation level read uncommitted
declare @err int

declare @BeginDateTime datetime
declare @EndDateTime datetime

declare @IsSnapshotSelected bit
SET @IsSnapshotSelected = 0

if (@UTCSnapshotCollectionDateTime is null)
begin
	select @EndDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)
	SET @IsSnapshotSelected = 1
end
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime)

-- SqlDM 10.2 (Anshul Aggarwal) History Browser - Fetch the latest snapshot in  the given start and end range.
if (@IsSnapshotSelected = 0)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID 
			AND isnull(RefreshType,0) = 0
			AND (UTCCollectionDateTime between @BeginDateTime AND @EndDateTime))

-- Get session and lock list for requested snapshot
select
	[InstanceName]
	,[UTCCollectionDateTime]
	,[SessionList]
	,[LockList]
from 
	[ServerActivity]
	left join [MonitoredSQLServers]
	on [ServerActivity].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
where 
	[ServerActivity].[SQLServerID] = @SQLServerID
	and [UTCCollectionDateTime] = @EndDateTime

-- Get history of blocked processes and lead blockers
select
	[ServerStatistics].[UTCCollectionDateTime] as [Date]
	,[BlockedProcesses] as [Blocked Sessions]
	,[LeadBlockers] as [Lead Blockers]
    ,[LockStatistics] as [Lock Statistics]
from
	[ServerStatistics]
	left join [ServerActivity]
	on [ServerActivity].[SQLServerID] = [ServerStatistics].[SQLServerID] and [ServerActivity].[UTCCollectionDateTime] = [ServerStatistics].[UTCCollectionDateTime]
where
	[ServerStatistics].[SQLServerID] = @SQLServerID
	and [ServerStatistics].[UTCCollectionDateTime] between @BeginDateTime and @EndDateTime
order by 
	[ServerStatistics].[UTCCollectionDateTime]

select @err = @@error
return @err
end