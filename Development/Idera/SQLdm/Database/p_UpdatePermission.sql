if (object_id('p_UpdatePermission') is not null)
begin
drop procedure [p_UpdatePermission]
end
go

create procedure [p_UpdatePermission]
	@PermissionID int,
	@Permission int,
	@Enabled bit,
	@TagsXML nvarchar(max),
	@ServerXML nvarchar(max),
	@Comment nvarchar(1024),
	@WebAppPermission bit-- SQLdm 8.5 <Ankit Srivastava> added new columns
as
begin
	DECLARE @err int
	DECLARE @permissionExists int
	declare @xmlDoc int
	
	-- check if permission exists
	select @permissionExists = count(PermissionID) 
	from [dbo].[Permission] 
	where PermissionID = @PermissionID
	
	-- update if permission exists.
	if (@permissionExists != 0)
	begin
		-- update Permission table
		update [dbo].[Permission]
		set Permission = @Permission, Enabled = @Enabled, Comment = @Comment , WebAppPermission=@WebAppPermission
		where PermissionID = @PermissionID

		-- Add tags to permission
		exec p_UpdatePermissionTags @PermissionID, @TagsXML

		-- delete servers from PermissionServers table for input PermissionID.
		delete from [dbo].[PermissionServers] where PermissionID = @PermissionID

		-- re-add servers to PermissionServers table
		exec sp_xml_preparedocument @xmlDoc output, @ServerXML

		insert into [dbo].[PermissionServers] (PermissionID, SQLServerID)
		select @PermissionID, SQLServerID
			from openxml(@xmlDoc, '//Server', 1) with (SQLServerID int) 

		exec sp_xml_removedocument @xmlDoc
	end
	
	SELECT @err = @@error
	RETURN @err
end
