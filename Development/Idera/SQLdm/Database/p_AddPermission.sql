if (object_id('p_AddPermission') is not null)
begin
drop procedure [p_AddPermission]
end
go

create procedure [p_AddPermission]
	@LoginSID varbinary(256),
	@Permission int,
	@TagsXML nvarchar(max),
	@ServerXML nvarchar(max),
	@Comment nvarchar(1024),
	@WebAppPermission bit-- SQLdm 8.5 <Ankit Srivastava> added new columns
as
begin
	declare @err int
	declare @ruleid int
	declare @xmlDoc int
	
	-- Add entry to permission table and get the rule ID.
	insert into [dbo].[Permission] (LoginSID, Permission, Enabled, Comment, WebAppPermission)
	values (@LoginSID, @Permission, 1, @Comment, @WebAppPermission)
	select @ruleid = @@identity

	-- Add tags to permission
	exec p_UpdatePermissionTags @ruleid, @TagsXML
	
	-- Add all servers to the permission server table for the rule ID.
	exec sp_xml_preparedocument @xmlDoc output, @ServerXML

	insert into [dbo].[PermissionServers] (PermissionID, SQLServerID)
	select @ruleid, SQLServerID
		from openxml(@xmlDoc, '//Server', 1) with (SQLServerID int) 

	exec sp_xml_removedocument @xmlDoc
	

	select @err = @@error
	return @err
end

