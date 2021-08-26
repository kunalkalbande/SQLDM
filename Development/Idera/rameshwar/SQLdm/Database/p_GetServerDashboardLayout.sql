if (object_id('p_GetServerDashboardLayout') is not null)
begin
drop procedure [p_GetServerDashboardLayout]
end
go

create procedure [p_GetServerDashboardLayout] 
	@LoginName nvarchar(256),
	@SQLServerID int
as
begin
	declare @ID int
	
	select @ID = 0

	-- get the default dashboard for the server for this user
	select @ID = l.DashboardLayoutID
		from [DashboardLayouts] l
			inner join [DashboardDefaults] d on l.DashboardLayoutID = d.DashboardLayoutID
		where d.LoginName = @LoginName
			and d.SQLServerID = @SQLServerID
	
	-- otherwise get the users default
	if (@ID = 0)
		select @ID = l.DashboardLayoutID
			from [DashboardLayouts] l
				inner join [DashboardDefaults] d on l.DashboardLayoutID = d.DashboardLayoutID
			where d.LoginName = @LoginName
				and d.SQLServerID is null

	-- otherwise get the system default
	if (@ID = 0)
	begin
		declare @version nvarchar(30)
		select @version = ServerVersion from MonitoredSQLServers where SQLServerID = @SQLServerID
		select @ID = case when @version is not null and left(@version, 1) = '8' then 2 else 1 end
	end

	-- return the configuration
	select DashboardLayoutID, LoginName, Name, Configuration
		from [DashboardLayouts]
		where DashboardLayoutID = @ID

end	
