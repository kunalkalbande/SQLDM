if (object_id('[p_GetSessionsDetailsRanged]') is not null)
begin
drop procedure [p_GetSessionsDetailsRanged]
end
go

create procedure [dbo].[p_GetSessionsDetailsRanged]
	@SQLServerID int,
	@UTCSnapshotStartRange datetime = null,
	@UTCSnapshotEndRange datetime = null
as
begin
set transaction isolation level read uncommitted
declare @err int

-- SqlDM 10.2 (Anshul Aggarwal) History Browser - Fetch the latest snapshot in  the given start and end range.
if (@UTCSnapshotStartRange is null)
	select @UTCSnapshotStartRange = (select min(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)
if  (@UTCSnapshotEndRange is null)
    select @UTCSnapshotEndRange = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)

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
	and [ServerActivity].[UTCCollectionDateTime] >= @UTCSnapshotStartRange
	and [ServerActivity].[UTCCollectionDateTime] <= @UTCSnapshotEndRange
select @err = @@error
return @err
end
go