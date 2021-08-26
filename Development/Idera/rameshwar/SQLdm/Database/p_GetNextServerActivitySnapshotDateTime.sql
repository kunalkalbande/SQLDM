if (object_id('p_GetNextServerActivitySnapshotDateTime') is not null)
begin
drop procedure [p_GetNextServerActivitySnapshotDateTime]
end
go
create procedure [dbo].[p_GetNextServerActivitySnapshotDateTime]
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime,
	@RefreshType int = 0
as
begin
set transaction isolation level read uncommitted
declare @err int

select 
	min(UTCCollectionDateTime)
from 
	[ServerActivity]
where 
	[UTCCollectionDateTime] > @UTCSnapshotCollectionDateTime 
	and [SQLServerID] = @SQLServerID
	and isnull(RefreshType, 0) = @RefreshType

select @err = @@error
return @err
end