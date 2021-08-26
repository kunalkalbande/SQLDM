if (object_id('p_UpdatePermissionStatus') is not null)
begin
drop procedure [p_UpdatePermissionStatus]
end
go

create procedure [p_UpdatePermissionStatus]
	@PermissionID int,
	@Enabled bit
as
begin
	DECLARE @err int
	
	-- update Permission table
	update [dbo].[Permission]
	set Enabled = @Enabled
	where PermissionID = @PermissionID
	
	SELECT @err = @@error
	RETURN @err
end
