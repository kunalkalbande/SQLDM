if (object_id('p_AddManagementService') is not null)
begin
drop procedure p_AddManagementService
end
go
CREATE PROCEDURE [dbo].[p_AddManagementService](
	@InstanceName NVARCHAR(15),
	@MachineName NVARCHAR(15),
	@Address NVARCHAR(256),
	@Port int,
	@ReturnServiceId UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	DECLARE @e int
	DECLARE @id uniqueidentifier

	BEGIN TRANSACTION

	-- see if we are already registered (possible we are doing an upgrade)
	SELECT @id = [ManagementServiceID] from ManagementServices 
			where [InstanceName] = @InstanceName and
				  [MachineName] = @MachineName
	
	SELECT @e = @@error

	if (@id is null)
	begin
		-- assign a new id to the management service
		SELECT @id = NEWID()
		-- remove existing management service records
		DELETE FROM [ManagementServices]
		-- add the new management service record
		INSERT INTO [ManagementServices] ([ManagementServiceID],[InstanceName],[MachineName],[Address],[Port])
			VALUES (@id, @InstanceName, @MachineName, @Address, @Port)
	
		SELECT @e = @@error

		if (@e = 0)
		begin
			declare @c int
			select @c=count(*) from CollectionServices 
			if @c = 0
			begin
				update MonitoredSQLServers set CollectionServiceID=null
			end
		end
	end


	SELECT @ReturnServiceId = @id
	
	IF (@e = 0)
	begin
		-- make sure this is the default management service
		exec p_SetDefaultManagementService @id output
		COMMIT
	END
	else
	begin
		ROLLBACK
	end

	RETURN @e
END

