if (object_id('p_GetCloudByInstanceID') is not null)
begin
drop procedure p_GetCloudByInstanceID
end
go

create procedure p_GetCloudByInstanceID 
(
@SQLServerId int 
)
as
BEGIN
Select CloudProviderId from MonitoredSQLServers where SQLServerID=@SQLServerId
END