if (object_id('p_GetDashboardLayoutID') is not null)
begin
drop procedure [p_GetDashboardLayoutID]
end
go

create procedure [p_GetDashboardLayoutID] 
	@LoginName nvarchar(256),
	@Name nvarchar(128)
as
begin

	select DashboardLayoutID from [DashboardLayouts]
		where LoginName = @LoginName
			and Name = @Name

end	
