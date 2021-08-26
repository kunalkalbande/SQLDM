if (object_id('p_SetDefaultDashboardLayout') is not null)
begin
drop procedure [p_SetDefaultDashboardLayout]
end
go

create procedure [p_SetDefaultDashboardLayout] 
	@LoginName nvarchar(256),
	@SQLServerID int,
	@DashboardLayoutID int
as
begin

	if exists (select 1 from DashboardDefaults
					where LoginName = @LoginName
						and (SQLServerID = @SQLServerID
							 or (SQLServerID is null and @SQLServerID is null)))
		update DashboardDefaults
			set DashboardLayoutID = @DashboardLayoutID
			where LoginName = @LoginName
				and (SQLServerID = @SQLServerID
						or (SQLServerID is null and @SQLServerID is null))
	else
		insert into DashboardDefaults
				(LoginName,
				 SQLServerID,
				 DashboardLayoutID
				)
			values (@LoginName,
					@SQLServerID,
					@DashboardLayoutID
				)

end	
