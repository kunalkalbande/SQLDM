if (object_id('p_GetMirroringParticipantsForServer') is not null)
begin
drop procedure [p_GetMirroringParticipantsForServer]
end
go


Create procedure p_GetMirroringParticipantsForServer
@serverID int
as

begin
select 'ServerID' = coalesce(mirror_instanceID, principal_instanceID),
mirroring_guid
from MirroringParticipants (nolock)
where @serverID = case role when 1 then principal_instanceID else mirror_instanceID end
end