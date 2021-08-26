if (object_id('p_UpdateDashboardLayout') is not null)
begin
drop procedure [p_UpdateDashboardLayout]
end
go

create procedure [p_UpdateDashboardLayout] 
	@DashboardLayoutID int,
	@Name nvarchar(128),
	@Configuration nvarchar(max),
	@Image image,
	@UseAsDefault bit
as
begin

	declare @LastUpdated datetime
	declare @LoginName nvarchar(256)
	
	select @LastUpdated = GETUTCDATE()

	update [DashboardLayouts]
		set Name = @Name,
			LastUpdated = @LastUpdated,
			LastViewed = @LastUpdated,
			Configuration = isnull(@Configuration, Configuration),
			LayoutImage = isnull(@Image, LayoutImage)
		where
			DashboardLayoutID = @DashboardLayoutID

	if (@UseAsDefault = 1)
	begin
		select @LoginName = LoginName from [DashboardLayouts] where DashboardLayoutID = @DashboardLayoutID
		exec p_SetDefaultDashboardLayout @LoginName, null, @DashboardLayoutID
	end

end
