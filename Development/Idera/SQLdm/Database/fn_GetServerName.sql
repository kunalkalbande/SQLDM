if (object_id('fn_GetServerName') is not null)
begin
drop function fn_GetServerName
end
go
-- =============================================
-- Take the real server name which is replication uses and
-- Returns the name by which sqldm knows this server
-- return the registered name given the realname
-- if there are multiple entries with the same realname return the once where the instancename matches the realname (primary)
-- if there are no cases of instance equal real but rows were returned then the first instance name
-- if no rows were returned then return whatever registered name was passed in
-- =============================================
CREATE FUNCTION [dbo].[fn_GetServerName]
(
	-- Add the parameters for the function here
	@Server sysname
)
RETURNS sysname
AS
BEGIN
declare @returnString sysname

--return the default instance if it exists
Select @returnString = m.InstanceName
from MonitoredSQLServers m  (nolock)
where lower(RealServerName) = lower(@Server)
and lower(InstanceName) = lower(@Server)
and m.Active = 1
--if no default exists, there are only aliases
if @@RowCount = 0
 begin
	--return the last active server to be registered with this real name
	Select @returnString = m.InstanceName
	from MonitoredSQLServers m  (nolock)
	where lower(RealServerName) = lower(@Server) 
	and m.Active = 1
	if @@RowCount = 0
	--if no servers were found with this real name return the name the user passed in
	select @returnString = @Server
 end

return @returnString
END
GO