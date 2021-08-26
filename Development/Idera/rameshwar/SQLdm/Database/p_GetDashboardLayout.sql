if (object_id('p_GetDashboardLayout') is not null)
begin
drop procedure [p_GetDashboardLayout]
end
go

create procedure [p_GetDashboardLayout] 
	@DashboardLayoutID int
as
begin

	if exists (select 1 from [DashboardLayouts] where DashboardLayoutID = @DashboardLayoutID)
	begin
		select DashboardLayoutID, LoginName, Name, LastUpdated, LastViewed, Configuration, LayoutImage, system_user as CurrentUser 
			from [DashboardLayouts]
			where DashboardLayoutID = @DashboardLayoutID
	end
	else
		raiserror ('The dashboard layout id was not found.', 1, 0)

end
