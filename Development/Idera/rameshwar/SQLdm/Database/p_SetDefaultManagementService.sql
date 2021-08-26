if (object_id('[p_SetDefaultManagementService]') is not null)
begin
drop procedure [p_SetDefaultManagementService]
end
go
create procedure [dbo].[p_SetDefaultManagementService]
(@ManagementServiceID [uniqueidentifier] output)
as
begin
	declare @ManagementServiceIDTemp uniqueidentifier

	if @ManagementServiceID is null
	begin
		select top 1
			@ManagementServiceIDTemp = ManagementServiceID
		from
			[ManagementServices]
		order by [MachineName] asc
	end
	else
	begin
		select @ManagementServiceIDTemp = @ManagementServiceID
	end

	if @ManagementServiceIDTemp is null
		return 1

	if exists(select [Name] from [RepositoryInfo] where [Name] = 'Default Management Service')
	begin
		update [RepositoryInfo]
		set [Character_Value] = @ManagementServiceIDTemp, [Internal_Value] = NULL
		where [Name] = 'Default Management Service'
	end
	else
		insert into [RepositoryInfo]
			   ([Name]
			   ,[Character_Value])
		 values
			   ('Default Management Service'
			   ,@ManagementServiceIDTemp)

	set @ManagementServiceID = @ManagementServiceIDTemp

	return 0
end
