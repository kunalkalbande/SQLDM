if (object_id('p_AddCollectionServices') is not null)
begin
drop procedure p_AddCollectionServices
end
go
CREATE PROCEDURE [dbo].[p_AddCollectionServices](
	@InstanceName NVARCHAR(15),
	@MachineName NVARCHAR(15),
	@Address NVARCHAR(256),
	@Port int,
	@Enabled bit,
	@ManagementServiceId UNIQUEIDENTIFIER,
	@ReturnServiceId UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	DECLARE @id UNIQUEIDENTIFIER
	DECLARE @e INT

	BEGIN TRANSACTION

	-- see if this collection service is already registered (possible upgrade)
	SELECT @id = [CollectionServiceID] from CollectionServices
		WHERE [InstanceName] = @InstanceName and
			[MachineName] = @MachineName
	
	SELECT @e = @@error

	if (@id is null)
	begin
		-- does not exist so mark others not enabled and add the new collection service
		SELECT @id = NEWID()

		UPDATE [CollectionServices] SET [Enabled] = 0

		INSERT INTO [CollectionServices] ([CollectionServiceID],[InstanceName],[MachineName],[Address],[Port],[Enabled],[ManagementServiceID])
			VALUES (@id, @InstanceName, @MachineName, @Address, @Port, @Enabled, @ManagementServiceId)

		SELECT @e = @@error
	end
	else
	begin
		-- make sure this collection service is the only one enabled
		UPDATE [CollectionServices] SET [Enabled] = 0
			WHERE [CollectionServiceID] <> @id
		UPDATE [CollectionServices] SET [Enabled] = 1
			WHERE [CollectionServiceID] = @id
	end
	-- set return collection service id
	SELECT @ReturnServiceId = @id	

	IF (@e = 0)
	BEGIN
		-- make this the default collection service
		exec [p_SetDefaultCollectionService] @ManagementServiceId, @id
	END

	COMMIT		

	RETURN @e
END