if (object_id('p_DeletePermission') is not null)
begin
drop procedure [p_DeletePermission]
end
go

create procedure [p_DeletePermission]
	@PermissionID int
as
begin
	DECLARE @err int
	
	-- delete from Permission table
	delete from [dbo].[Permission]
	where PermissionID = @PermissionID
	
	SELECT @err = @@error
	RETURN @err
end
