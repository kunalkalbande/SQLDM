IF (object_id('p_GetPermissions') is not null)
BEGIN
drop procedure p_GetPermissions
END
GO

CREATE PROCEDURE [dbo].[p_GetPermissions]
AS
BEGIN
	declare @e int
	declare @securityEnabled bit

	-- check if security is enabled, and return flags.
	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled' and [Internal_Value] != 0) = 1)
		set @securityEnabled = 1
	else
		set @securityEnabled = 0
		
	select @securityEnabled as SecurityEnabled

	-- Get all permissions defined in SQLdm if security is enabled.
	-- Get all users belonging to sysadmin role, these permissions are system defined
	-- and cannot be edited.   These users have administrator power in SQLdm.
	if @securityEnabled = 1
	begin
		select
			[PermisisonID] = -1, -- no identity column, set to magic number -1
			[System] = 1, -- system permission
			[Enabled] = 1,
			sl.sid as [LoginSID],
			sl.[name] as [Login],
			[LoginType] = 	case sl.isntname
						when 1 then -- isntname
							case sl.[isntgroup]
								when 0 then 1 -- windows user
								else 2 -- windows group
							end
						when 0 then 3 -- sql login
						else 
							0 -- unknown
					end,
			[PermissionType] = 3, -- administrator
			[SQLServerID] = null,
			[InstanceName] = null,
			[Active] = cast(null as binary),
			[Deleted] = cast(null as binary),
			[Comment] = ''		,
			[WebAppPermission] =1  -- SQLdm 8.5 <Ankit Srivastava> added new columns
		from [master]..[syslogins] as sl
		where sl.sysadmin = 1 
		union
		-- Get all permissions defined in Permission table.   Do a join with sysusers to
		-- get login name and type.   If there is no corresponding login in sysusers table
		-- then set login type to UNKNOWN.   Do a join with PermissionServers/MonitoredSQLServers
		-- tables to get a list of instances assigned.
		select 
			p.[PermissionID] as [PermissionID], 
			[System] = 0, -- user defined permission
			p.[Enabled] as [Enabled], 
			p.[LoginSID] as [LoginSID], 
			sl.[name] as [Login],
			[LoginType] = 	case sl.isntname
						when 1 then -- isntname
							case sl.[isntgroup]
								when 0 then 1 -- windows user
								else 2 -- windows group
							end
						when 0 then 3 -- sql login
						else 
							0 -- unknown
					end,
			p.[Permission] as [PermissionType], 
			s.[SQLServerID] as [SQLServerID], 
			mss.[InstanceName] as [InstanceName],
			mss.[Active] as [Active],
			mss.[Deleted] as [Deleted],
			p.[Comment] as [Comment],
			p.[WebAppPermission] as [WebAppPermission]  -- SQLdm 8.5 <Ankit Srivastava> added new columns
		from [dbo].[Permission] as p 
			left join [dbo].[PermissionServers] as s on (p.[PermissionID] = s.[PermissionID])
			left join [dbo].[MonitoredSQLServers] as mss on (s.[SQLServerID] = mss.[SQLServerID])
			join [master]..[syslogins] as sl on (p.[LoginSID] = sl.[sid])
		UNION -- pick up server assignments from permission tags
		select 
			p.[PermissionID] as [PermissionID], 
			[System] = 0, -- user defined permission
			p.[Enabled] as [Enabled], 
			p.[LoginSID] as [LoginSID], 
			sl.[name] as [Login],
			[LoginType] = 	case sl.isntname
						when 1 then -- isntname
							case sl.[isntgroup]
								when 0 then 1 -- windows user
								else 2 -- windows group
							end
						when 0 then 3 -- sql login
						else 
							0 -- unknown
					end,
			p.[Permission] as [PermissionType], 
			st.[SQLServerId] as [SQLServerID], 
			mss.[InstanceName] as [InstanceName],
			mss.[Active] as [Active],
			mss.[Deleted] as [Deleted],
			p.[Comment] as [Comment],
			p.[WebAppPermission] as [WebAppPermission]  -- SQLdm 8.5 <Ankit Srivastava> added new columns
		from [dbo].[Permission] as p 
			left join [dbo].[PermissionTags] as pt on (p.[PermissionID] = pt.[PermissionId])
			left join [dbo].[ServerTags] as st on (pt.[TagId] = st.[TagId])
			left join [dbo].[MonitoredSQLServers] as mss on (st.[SQLServerId] = mss.[SQLServerID])
			join [master]..[syslogins] as sl on (p.[LoginSID] = sl.[sid])
		where 
			st.[SQLServerId] is not null and
			st.[SQLServerId] not in
				(select [SQLServerID] 
					from [dbo].[PermissionServers] s 
					where p.[PermissionID] = s.[PermissionID])
	end
	
	select @e = @@error
	return @e
END
GO
