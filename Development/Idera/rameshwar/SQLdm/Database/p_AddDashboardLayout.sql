if (object_id('p_AddDashboardLayout') is not null)
begin
drop procedure [p_AddDashboardLayout]
end
go

create procedure [p_AddDashboardLayout] 
	@LoginName nvarchar(256),
	@Name nvarchar(128),
	@Configuration nvarchar(max),
	@Image image,
	@UseAsDefault bit,
	@newID int output
as
begin

	declare @ID int

	insert into [DashboardLayouts]
			(LoginName,
			Name,
			LastUpdated,
			LastViewed,
			Configuration,
			LayoutImage
			)
		values (@LoginName,
				@Name,
				GETUTCDATE(),
				GETUTCDATE(),
				@Configuration,
				@Image
			)

	select @ID = SCOPE_IDENTITY()

	if (@UseAsDefault = 1)
		exec p_SetDefaultDashboardLayout @LoginName, null, @ID

	select @newID = @ID
end	

