if (object_id('p_GetCollectionServiceForSQLServer') is not null)
begin
drop procedure p_GetCollectionServiceForSQLServer
end
go
CREATE PROCEDURE [dbo].[p_GetCollectionServiceForSQLServer](
	@SQLServerID INT,
	@ReturnServiceID UNIQUEIDENTIFIER OUTPUT,
	@ReturnInstanceName NVARCHAR(256) OUTPUT
)
AS
BEGIN
	declare @e int
	declare @CollectionServiceIDTemp uniqueidentifier
	declare @ManagementServiceIDTemp uniqueidentifier
	declare @InstanceNameTemp nvarchar(256)

	SELECT @CollectionServiceIDTemp = [CollectionServiceID],
		   @InstanceNameTemp = [InstanceName]
	FROM [MonitoredSQLServers]
	WHERE [SQLServerID] = @SQLServerID
	
	SELECT @e = @@error

	if @CollectionServiceIDTemp is null
	begin
		select @ManagementServiceIDTemp = null
		exec p_GetDefaultManagementServiceID @ManagementServiceIDTemp output
		if @ManagementServiceIDTemp is not null
		    exec p_SetDefaultCollectionService @ManagementServiceIDTemp, @CollectionServiceIDTemp output 
	end

	SELECT @ReturnServiceID = @CollectionServiceIDTemp
	SELECT @ReturnInstanceName = @InstanceNameTemp
	RETURN @e
END


