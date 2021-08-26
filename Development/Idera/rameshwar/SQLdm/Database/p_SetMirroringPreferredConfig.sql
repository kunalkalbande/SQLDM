if (object_id('p_SetMirroringPreferredConfig') is not null)
begin
drop procedure [p_SetMirroringPreferredConfig]
end
go

create procedure [dbo].[p_SetMirroringPreferredConfig](
@Guid uniqueidentifier,
@mirror int = null, 
@principal int = null, 
@witnessName nvarchar(128) = null, 
@databaseName nvarchar(128) = null,
@Normal int
) as
--------------------------------------------------------------------------------
--  Batch: MirroringSetPreferredConfig
--  Variables: 
--		[0] - Session GUID
--		[1] - Action integer 0 = Failed Over, 1 = Normal, -1 = Agnostic
--------------------------------------------------------------------------------

if @Normal = -1
delete from MirroringPreferredConfig where MirroringGuid = @Guid
else
begin

if exists(select * from MirroringPreferredConfig where MirroringGuid = @Guid)
begin
update MirroringPreferredConfig 
set MirrorInstanceID = @mirror, 
PrincipalInstanceID = @principal,
NormalConfiguration = @Normal,
DatabaseName = @databaseName,
WitnessAddress = @witnessName
where MirroringGuid = @Guid
end
else
begin
insert into MirroringPreferredConfig (
MirroringGuid, 
MirrorInstanceID, 
PrincipalInstanceID, 
NormalConfiguration, 
DatabaseName, 
WitnessAddress)
values (@Guid, @mirror, @principal, @Normal, @databaseName, @witnessName)
end
end
GO
