if (object_id('p_GetCollectionServices') is not null)
begin
drop procedure p_GetCollectionServices
end
go
CREATE PROCEDURE [dbo].[p_GetCollectionServices](
	@CollectionServiceId uniqueidentifier,
	@ManagementServiceId uniqueidentifier
)
as
begin
	declare @e int

	IF (@ManagementServiceId is null) 
	begin
		if (@CollectionServiceId is null)
		begin
			select [CollectionServiceID],[InstanceName],[MachineName],[Address],[Port],[Enabled],[LastHeartbeatUTC],[ManagementServiceID] from [CollectionServices]
		end
		else
		begin
			select [CollectionServiceID],[InstanceName],[MachineName],[Address],[Port],[Enabled],[LastHeartbeatUTC],[ManagementServiceID] from [CollectionServices]
				where ([CollectionServiceID] = @CollectionServiceId)
		end
	end
	else
	begin
		iF (@CollectionServiceId is null)
		begin
			select [CollectionServiceID],[InstanceName],[MachineName],[Address],[Port],[Enabled],[LastHeartbeatUTC],[ManagementServiceID] from [CollectionServices]
				where ([ManagementServiceID] = @ManagementServiceId)
		end
		else
		begin
			select [CollectionServiceID],[InstanceName],[MachineName],[Address],[Port],[Enabled],[LastHeartbeatUTC],[ManagementServiceID] from [CollectionServices]
				where ([CollectionServiceID] = @CollectionServiceId) and ([ManagementServiceID] = @ManagementServiceId)
		end
	end

	select @e = @@error
	return @e
end
