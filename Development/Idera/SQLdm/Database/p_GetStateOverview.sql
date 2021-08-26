if (object_id('p_GetStateOverview') is not null)
begin
drop procedure [p_GetStateOverview]
end
go
create procedure [dbo].[p_GetStateOverview]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null
as
begin
set transaction isolation level read uncommitted
declare @err int

if (@UTCSnapshotCollectionDateTime is null)
	select @UTCSnapshotCollectionDateTime = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID)

select
	[UTCCollectionDateTime]
	,[StateOverview]
from 
	[ServerActivity]
where 
	[SQLServerID] = @SQLServerID
	and [UTCCollectionDateTime] = @UTCSnapshotCollectionDateTime

select @err = @@error
return @err
end