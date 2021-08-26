if (object_id('p_DeleteMirroringSessionFromServer') is not null)
begin
drop procedure [p_DeleteMirroringSessionFromServer]
end
go

Create procedure p_DeleteMirroringSessionFromServer
@serverID int,
@guid uniqueidentifier
as

begin
delete from MirroringParticipants
where @serverID = case role when 1 then principal_instanceID else mirror_instanceID end
and mirroring_guid = @guid
end