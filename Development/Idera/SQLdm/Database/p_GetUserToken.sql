

IF (object_id('p_GetUserTokenInternal') is not null)
BEGIN
drop procedure p_GetUserTokenInternal
END
GO

CREATE PROCEDURE [dbo].[p_GetUserTokenInternal]
	@IsSQLLogin bit,
	@SQLLoginName nvarchar(128),
	@WindowsSID varbinary(85),
	@ForReporting bit = 0,
	@VMOnly bit = 0,
	@MinimumServerVersion int = 8
AS
BEGIN
	declare @e int
	declare @securityEnabled bit
	declare @sysadmin bit
	declare @sqldmAdministrator bit
	declare @repositoryVersion int
	
	----------------------------------------------------------------------------------------------------
	-- create temp table to hold the list of assigned servers & matching logins
	----------------------------------------------------------------------------------------------------
	create table #temp_AssignedServers 
	(
		SQLServerID int not null,
		InstanceName nvarchar(256) not null,
		Active bit not null,
		Deleted bit not null,
		PermissionType int not null,
		Virtualized bit not null,
		ServerVersion nvarchar(30)
	)
	create table #temp_MatchingLogins
	(
		MatchLoginSID varbinary(85) not null
	)
	
	----------------------------------------------------------------------------------------------------
	-- initialize flags
	----------------------------------------------------------------------------------------------------
	set @securityEnabled = 0
	set @sysadmin = 0
	set @sqldmAdministrator = 0

	----------------------------------------------------------------------------------------------------
	-- check if security is enabled
	-- if ApplicationSecurityEnabled flag is there, check if its set, else create the flag and clear it
	----------------------------------------------------------------------------------------------------
	if ((select count([Internal_Value]) 
		from dbo.[RepositoryInfo] 
			where [Name] = 'ApplicationSecurityEnabled') = 1)
	begin
		if ((select count([Internal_Value]) 
			from dbo.[RepositoryInfo] 
				where [Name] = 'ApplicationSecurityEnabled' and [Internal_Value] != 0) = 1)
			set @securityEnabled = 1
		else
			set @securityEnabled = 0
	end
	else
	begin
		-- insert row for application security enabled flag and clear it
		insert into dbo.[RepositoryInfo]
		values ('ApplicationSecurityEnabled',0,'')
		
		set @securityEnabled = 0
	end

	----------------------------------------------------------------------------------------------------
	-- check if user is sysadmin
	----------------------------------------------------------------------------------------------------
	if IS_SRVROLEMEMBER ('sysadmin') = 1
		set @sysadmin = 1
	else
		set @sysadmin = 0
	----------------------------------------------------------------------------------------------------
	-- if security is not enabled set administrator flag because everyone is an administrator
	-- else get the permissions assigned to the user.
	----------------------------------------------------------------------------------------------------
	if @securityEnabled = 0
	begin
		set @sqldmAdministrator = 1
		
		-- fill temp assigned servers table with all monitored SQL Servers
		-- with admin (2) permission
		insert into #temp_AssignedServers
		select [SQLServerID], [InstanceName], [Active], [Deleted], cast (3 as int) as [PermissionType], case when [VmUID] is null then 0 else 1 end, [ServerVersion]
		from dbo.[MonitoredSQLServers] 
		where [Deleted] = 0
	end
	else
	begin
		-- if user is sysadmin they are administrator 
		-- else check the Permisssion table for administrator powers.
		if @sysadmin = 1
		begin
			-- set SQLdm admin flag
			set @sqldmAdministrator = 1
			
			-- fill temp assigned servers table with all monitored SQL Servers
			-- with admin (2) permission
			insert into #temp_AssignedServers
			select [SQLServerID], [InstanceName], [Active], [Deleted], cast (3 as int) as [PermissionType], case when [VmUID] is null then 0 else 1 end, [ServerVersion]
			from dbo.[MonitoredSQLServers] 
			where [Deleted] = 0
		end
		else
		begin
			-- determine matching logins
			if @IsSQLLogin = 1
			begin
				-- insert the matching login from syslogins table
				insert into #temp_MatchingLogins
				select sid from master..syslogins where name = @SQLLoginName and isntname = 0
			end
			else
			begin
				insert into #temp_MatchingLogins
					select sid from sys.login_token where type = 'WINDOWS LOGIN' or type = 'WINDOWS GROUP'			
			end
			
			-- if matching logins are available get token information
			if (select count(MatchLoginSID) from #temp_MatchingLogins) > 0
			begin

				-- check if user has administrator power
				if (select count(Permission) 
					from [dbo].[Permission]
						where Permission = 3 -- admin power
							and LoginSID in (select MatchLoginSID from #temp_MatchingLogins)
								and Enabled = 1) > 0
					set @sqldmAdministrator = 1
				else
					set @sqldmAdministrator = 0

				-- if user is administrator assign all servers with admin powers
				-- else get only those servers assigned to the user
				if @sqldmAdministrator = 1
				begin
					insert into #temp_AssignedServers
					select [SQLServerID], [InstanceName], [Active], [Deleted], cast (3 as int) as [PermissionType], case when [VmUID] is null then 0 else 1 end, [ServerVersion]
					from dbo.[MonitoredSQLServers]
					where [Deleted] = 0
				end
				else
				begin
					insert into #temp_AssignedServers
					select mss.SQLServerID, mss.InstanceName, mss.Active, mss.Deleted, p.Permission, case when mss.[VmUID] is null then 0 else 1 end, mss.ServerVersion
					from [dbo].[Permission] as p 
						join [dbo].[PermissionServers] as s on (p.[PermissionID] = s.[PermissionID])
							join [dbo].[MonitoredSQLServers] as mss on (s.[SQLServerID] = mss.[SQLServerID])
								where p.LoginSID in (select MatchLoginSID from #temp_MatchingLogins)
									and p.Enabled = 1 and mss.Deleted = 0
					-- get servers assigned via tags
					insert into #temp_AssignedServers
					select distinct mss.SQLServerID, mss.InstanceName, mss.Active, mss.Deleted, p.Permission, case when mss.[VmUID] is null then 0 else 1 end, mss.ServerVersion
					from [dbo].[Permission] as p 
						left outer join [dbo].[PermissionTags] pt on (p.[PermissionID] = pt.[PermissionId])
						left outer join [dbo].[ServerTags] st on (pt.[TagId] = st.[TagId])
						left outer join [dbo].[PermissionServers] s on (p.[PermissionID] = s.[PermissionID] and st.[SQLServerId] = s.[SQLServerID])
						join [dbo].[MonitoredSQLServers] as mss on (st.[SQLServerId] = mss.[SQLServerID])
								where p.LoginSID in (select MatchLoginSID from #temp_MatchingLogins)
									and p.Enabled = 1 and mss.Deleted = 0 
									and s.SQLServerID is null
				end
			end
		end
	end

	----------------------------------------------------------------------------------------------------
	-- select flags & assigned servers
	----------------------------------------------------------------------------------------------------
	if (isnull(@ForReporting,0) = 0)
	begin
		select @securityEnabled as SecurityEnabled, @sysadmin as SysAdmin, @sqldmAdministrator as SQLdmAdministrator
		select [SQLServerID], [InstanceName], [Active], [Deleted], [PermissionType]
		from #temp_AssignedServers where [Virtualized] >= @VMOnly
	end
	else
	begin
		select [InstanceName], [SQLServerID]
		from #temp_AssignedServers where [Virtualized] >= @VMOnly 
			and SUBSTRING(ServerVersion, 0, CHARINDEX('.', ServerVersion)) >= @MinimumServerVersion
	end
	
	select @e = @@error
	return @e
END
GO


IF (object_id('p_GetUserToken') is not null)
BEGIN
drop procedure p_GetUserToken
END
GO

CREATE PROCEDURE [dbo].[p_GetUserToken]
	@IsSQLLogin bit,
	@SQLLoginName nvarchar(128),
	@WindowsSID varbinary(85)
AS
BEGIN
	exec [p_GetUserTokenInternal] @IsSQLLogin, @SQLLoginName, @WindowsSID, 0
END

GO
