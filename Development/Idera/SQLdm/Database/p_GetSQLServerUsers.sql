--SQLdm 9.0 (Ankit Srivastava) - User Sync with CWF - Created new procedure to get the users in accoradance with the CWF Sync
-- EXEC [p_GetSQLServerUsers] 1
if (object_id('p_GetSQLServerUsers') is not null)
begin
drop procedure [p_GetSQLServerUsers]
end
go
CREATE PROCEDURE [dbo].[p_GetSQLServerUsers]
@ShouldBeSysAdmin BIT
AS
BEGIN
	declare @e int

	-- Get all users belonging to sysadmin role, these permissions are system defined
	-- and cannot be edited.   These users have administrator power in SQLdm.
	declare @securityEnabled bit

	-- check if security is enabled, and return flags.
	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled' and [Internal_Value] != 0) = 1)
		set @securityEnabled = 1
						else 
		set @securityEnabled = 0
	

		Declare @Users Table
		(
			[Login] nvarchar(100),
			[LoginSID] varbinary(85),
			[LoginType] nvarchar(1),
			IsAdmin bit,
			ServerName nvarchar(max)
		)

		Insert into @Users
		select
			sl.[name] as [Login],
			sl.sid  as [LoginSID],
			case sl.[isntgroup]
				when 0 then 'U' -- windows user
				else 'G' -- windows group
			end,
			--SQLDM(10.1) Barkha khatri 
			--SQLDM 26601 fix ,Using SQLDM permissions as well-avoiding multiple rows in case of union by having the same permissions in users table
			IsAdmin = case @securityEnabled
					when 0 then sl.sysadmin
					else
						CASE p.Permission
						WHEN 3 THEN 1
						ELSE sl.sysadmin
						end
					end,
					(SELECT STUFF((SELECT ',' +  MSS.InstanceName [text()]
			 FROM PermissionServers PS
			 JOIN MonitoredSQLServers MSS ON PS.SQLServerID = MSS.SQLServerID
         WHERE PermissionID = ps.PermissionID
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,1,'') List_Output
		FROM PermissionServers ps
		GROUP BY PermissionID
		HAVING p.PermissionID = PermissionID) AS ServerName
	
		from [master]..[syslogins] as sl
		LEFT JOIN [dbo].[Permission] as p 
			ON (p.[LoginSID] = sl.[sid])
		where sl.sysadmin = CASE 
								WHEN @ShouldBeSysAdmin=1
									THEN 1
								ELSE
									sl.sysadmin
							END
					and sl.isntname=1

		if(@securityEnabled = 0)
		BEGIN
			Select [Login]COLLATE DATABASE_DEFAULT,[LoginSID],[LoginType],IsAdmin, ServerName  from @Users -- SQLdm 9.0 (Ankit Srivastava) Added Collation 
		END
		ELSE
		BEGIN
			Select [Login]COLLATE DATABASE_DEFAULT,[LoginSID],[LoginType],IsAdmin, ServerName from @Users -- SQLdm 9.0 (Ankit Srivastava) Added Collation 
			union
			Select 
			sl.[name] as [Login],
			p.[LoginSID] as [LoginSID],
			[LoginType] = case sl.[isntgroup]
								when 0 then 'U' -- windows user
								else 'G' -- windows group
						end,
						--SQLDM(10.1) Barkha khatri 
						--SQLDM 26601 fix Using SQLDM permissions as well
						IsAdmin = CASE p.Permission
								WHEN 3
									THEN 1
								ELSE
									sl.sysadmin
									end,
			(SELECT STUFF((SELECT ',' + MSS.InstanceName [text()]
			 FROM PermissionServers PS
			 JOIN MonitoredSQLServers MSS ON PS.SQLServerID = MSS.SQLServerID
			 WHERE PermissionID = ps.PermissionID
			 FOR XML PATH(''), TYPE)
			.value('.','NVARCHAR(MAX)'),1,1,'') List_Output
			FROM PermissionServers ps
			GROUP BY PermissionID
			HAVING p.PermissionID = PermissionID) AS ServerName
							
			from [dbo].[Permission] as p 
			join [master]..[syslogins] as sl on (p.[LoginSID] = sl.[sid])
			where p.[Enabled]=1 and p.[WebAppPermission]=1
			and sl.isntname=1
		END

	select @e = @@error
	return @e
END

