if (object_id('p_UpdateWebAppPermissionStatus') is not null)
begin
drop procedure [p_UpdateWebAppPermissionStatus]
end

GO
--SQLdm 8.5 <Ankit Srivastava> - created new procedure for updating only the web app access
Create procedure [dbo].[p_UpdateWebAppPermissionStatus]
	@PermissionID int,
	@WebAppPermission bit
as
begin
	DECLARE @err int
	
	-- update Permission table
	update [dbo].[Permission]
	set WebAppPermission = @WebAppPermission
	where PermissionID = @PermissionID
	
	SELECT @err = @@error
	RETURN @err
end
 
