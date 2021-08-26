if (object_id('p_DeleteDashboardLayout') is not null)
begin
drop procedure [p_DeleteDashboardLayout]
end
go

create procedure [p_DeleteDashboardLayout] 
	@DashboardLayoutID int
as
begin

	if exists (select 1 from [DashboardLayouts] where DashboardLayoutID = @DashboardLayoutID)
	begin
		delete [DashboardDefaults]
			where DashboardLayoutID = @DashboardLayoutID

		delete [DashboardLayouts]
			where DashboardLayoutID = @DashboardLayoutID
	end
	else
		raiserror ('The dashboard layout id was not found for deletion.', 1, 0)

end
