if (object_id('[p_GetDefaultManagementService]') is not null)
begin
drop procedure [p_GetDefaultManagementService]
end
go
create procedure [dbo].[p_GetDefaultManagementService]
as
begin
	declare @DefaultManagementServiceID uniqueidentifier
	set @DefaultManagementServiceID = null

	select 
		@DefaultManagementServiceID = cast(Character_Value as uniqueidentifier)
	from 
		[RepositoryInfo] (NOLOCK)
	where 
		Name = 'Default Management Service'

	--DEBUGONLY - If the default management service is not set, set it here
	if @DefaultManagementServiceID is null
		exec p_SetDefaultManagementService @DefaultManagementServiceID output

	select [ManagementServiceID]
	  ,[MachineName]
	  ,[InstanceName]
	  ,[Address]
	  ,[Port]
	  ,[DefaultCollectionServiceID]
	from 
		[ManagementServices] (NOLOCK)
	where [ManagementServiceID] = @DefaultManagementServiceID

	return 0
end
