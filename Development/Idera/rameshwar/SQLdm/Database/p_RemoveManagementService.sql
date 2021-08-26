if (object_id('p_RemoveManagementService') is not null)
begin
drop procedure p_RemoveManagementService
end
go
CREATE PROCEDURE [dbo].[p_RemoveManagementService](
	@ServiceId UNIQUEIDENTIFIER
)
AS
BEGIN
	DECLARE @e INT

	DELETE FROM [ManagementServices] WHERE ([ManagementServiceID] = @ServiceId)

	SELECT @e = @@error
	if (@e = 0)
	begin
		-- remove the default management service record (if this was the default)
		DELETE FROM [RepositoryInfo] WHERE (LOWER([Character_Value]) = LOWER(@ServiceId))
	end

	RETURN @e
END
