if (object_id('p_UpdateDashboardLayoutViewed') is not null)
begin
drop procedure [p_UpdateDashboardLayoutViewed]
end
go

create procedure [p_UpdateDashboardLayoutViewed] 
	@DashboardLayoutID int
as
begin

	update [DashboardLayouts]
		set LastViewed = GETUTCDATE()
		where
			DashboardLayoutID = @DashboardLayoutID

end
