if (object_id('p_GetManagementServices') is not null)
begin
drop procedure p_GetManagementServices
end
go
CREATE PROCEDURE [dbo].[p_GetManagementServices](
	@ManagementServiceId UNIQUEIDENTIFIER
)
AS
BEGIN
	DECLARE @e INT

	IF (@ManagementServiceId IS NULL) 
	BEGIN
		SELECT [ManagementServiceID],[InstanceName],[MachineName],[Address],[Port],[DefaultCollectionServiceID] FROM [ManagementServices]
	END
	ELSE
	BEGIN
		SELECT [ManagementServiceID],[InstanceName],[MachineName],[Address],[Port],[DefaultCollectionServiceID] FROM [ManagementServices]
			WHERE ([ManagementServiceID] = @ManagementServiceId)
	END

	SELECT @e = @@error
	RETURN @e
END
