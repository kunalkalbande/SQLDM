if (object_id('[p_GetSessionsDetails]') is not null)
begin
drop procedure [p_GetSessionsDetails]
end
go

create procedure [dbo].[p_GetSessionsDetails]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null
as
begin
set transaction isolation level read uncommitted
declare @err int

-- SqlDM 10.2 (Anshul Aggarwal) History Browser - Fetch the latest snapshot in  the given start and end range.
if (@UTCSnapshotCollectionDateTime is null)
	select @UTCSnapshotCollectionDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)
else if  (@HistoryInMinutes is not null)
		select @UTCSnapshotCollectionDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID 
			AND isnull(RefreshType,0) = 0
			AND (UTCCollectionDateTime between dateadd(n, -@HistoryInMinutes, @UTCSnapshotCollectionDateTime) AND @UTCSnapshotCollectionDateTime))

select
	[InstanceName]
	,[UTCCollectionDateTime]
	,[SessionList]
from
	[ServerActivity]
	left join [MonitoredSQLServers]
	on [ServerActivity].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
where
	[ServerActivity].[SQLServerID] = @SQLServerID
	and [ServerActivity].[UTCCollectionDateTime] = @UTCSnapshotCollectionDateTime

select @err = @@error
return @err
end
go