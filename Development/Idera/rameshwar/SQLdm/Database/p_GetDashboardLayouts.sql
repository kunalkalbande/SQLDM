if (object_id('p_GetDashboardLayouts') is not null)
begin
drop procedure [p_GetDashboardLayouts]
end
go

create procedure [p_GetDashboardLayouts] 
	@LoginName nvarchar(256),
	@Filter int
as
begin
	-- get all for the user plus all the defaults
	if (@Filter = 0)
		select DashboardLayoutID, LoginName, Name, LastUpdated, LastViewed, Configuration, LayoutImage, system_user as CurrentUser
			from [DashboardLayouts]
			where LoginName is null
			order by Name
	else if (@Filter = 1)
		select DashboardLayoutID, LoginName, Name, LastUpdated, LastViewed, Configuration, LayoutImage, system_user as CurrentUser
			from [DashboardLayouts]
			where LoginName = @LoginName
				or LoginName is null
			order by LoginName Desc, Name
	else
	-- otherwise get all
		select DashboardLayoutID, LoginName, Name, LastUpdated, LastViewed, Configuration, LayoutImage, system_user as CurrentUser
			from [DashboardLayouts]
			order by LoginName, Name

end	
