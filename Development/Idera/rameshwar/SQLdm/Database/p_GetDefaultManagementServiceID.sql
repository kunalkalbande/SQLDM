if (object_id('[p_GetDefaultManagementServiceID]') is not null)
begin
drop procedure [p_GetDefaultManagementServiceID]
end
go
create procedure [dbo].[p_GetDefaultManagementServiceID]
	@ReturnServiceID uniqueidentifier output
as
begin
	declare @e int
	declare @DefaultManagementServiceID uniqueidentifier
	set @DefaultManagementServiceID = null

	select 
		@DefaultManagementServiceID = cast(Character_Value as uniqueidentifier)
	from 
		[RepositoryInfo] 
	where 
		Name = 'Default Management Service'

	select @e = @@error

	select @ReturnServiceID = @DefaultManagementServiceID
	
	return @e
end
