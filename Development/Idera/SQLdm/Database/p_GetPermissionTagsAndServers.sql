IF (object_id('p_GetPermissionTagsAndServers') is not null)
BEGIN
drop procedure p_GetPermissionTagsAndServers
END
GO

CREATE PROCEDURE [dbo].[p_GetPermissionTagsAndServers]
(
	@PermissionId INT
)
AS
BEGIN
	declare @e int
	declare @securityEnabled bit

	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled' and [Internal_Value] != 0) = 1)
		set @securityEnabled = 1
	else
		set @securityEnabled = 0
		
	select @securityEnabled as SecurityEnabled

	if @securityEnabled = 1
	begin
		select TagId from PermissionTags where PermissionId = @PermissionId
		select SQLServerID from PermissionServers where PermissionID = @PermissionId
	end
	
	select @e = @@error
	return @e
END