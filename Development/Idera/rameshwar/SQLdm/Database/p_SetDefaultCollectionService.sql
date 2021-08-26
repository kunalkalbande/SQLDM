if (object_id('[p_SetDefaultCollectionService]') is not null)
begin
drop procedure [p_SetDefaultCollectionService]
end
go
create procedure [dbo].[p_SetDefaultCollectionService]
(@ManagementServiceID [uniqueidentifier],
 @CollectionServiceID [uniqueidentifier] output)
as
begin
	declare @CollectionServiceIDTemp uniqueidentifier

	select @CollectionServiceIDTemp = @CollectionServiceID

	if @CollectionServiceIDTemp is null
	begin
		--- see if a default is already set
		select 
			@CollectionServiceIDTemp = [DefaultCollectionServiceID]		
		from
			[ManagementServices]
		where @ManagementServiceID = @ManagementServiceID
		
		if (@CollectionServiceIDTemp is not null)
		begin -- return the current default
			select @CollectionServiceID = @CollectionServiceIDTemp		
			return 0
		end
		--- no default, select the first existing 
		select top 1
			@CollectionServiceIDTemp = CollectionServiceID
		from
			[CollectionServices]
		where [ManagementServiceID] = @ManagementServiceID
		order by [MachineName],[InstanceName] asc		
		--- none found, exit
		if @CollectionServiceIDTemp is null
			return 1
	end

	update [ManagementServices]
		set [DefaultCollectionServiceID] = @CollectionServiceIDTemp
		where [ManagementServiceID] = @ManagementServiceID
	
	update [MonitoredSQLServers]
		set [CollectionServiceID] = @CollectionServiceIDTemp
--		where [CollectionServiceID] is null 

	select @CollectionServiceID = @CollectionServiceIDTemp

	return 0
end
