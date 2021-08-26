if (object_id('p_GetIdManagementServices') is not null)
begin
drop procedure p_GetIdManagementServices
end
go
CREATE PROCEDURE [dbo].[p_GetIdManagementServices](
	@InstanceName NVARCHAR(15),
	@MachineName NVARCHAR(15),
	@ReturnServiceId UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	DECLARE @id UNIQUEIDENTIFIER
	DECLARE @e INT

	SELECT @id = [ManagementServiceID] from [ManagementServices] 
		WHERE ([InstanceName] = @InstanceName) AND ([MachineName] = @MachineName)

	SELECT @e = @@error

	SELECT @ReturnServiceId = @id

	RETURN @e
END
