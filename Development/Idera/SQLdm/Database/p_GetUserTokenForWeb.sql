IF (object_id('p_GetUserTokenForWebInternal') is not null)
BEGIN
drop procedure p_GetUserTokenForWebInternal
END
GO

CREATE PROCEDURE [dbo].[p_GetUserTokenForWebInternal]
	@IsSQLLogin bit,
	@SQLLoginName nvarchar(128),
	@ForReporting bit = 0,
	@VMOnly bit = 0,
	@MinimumServerVersion int = 8
AS
BEGIN
  DECLARE @e int
  DECLARE @securityEnabled bit
  DECLARE @sysadmin bit
  DECLARE @sqldmAdministrator bit
  DECLARE @repositoryVersion int

  ----------------------------------------------------------------------------------------------------
  -- create temp table to hold the list of assigned servers & matching logins
  ----------------------------------------------------------------------------------------------------
  CREATE TABLE #temp_AssignedServers (
    SQLServerID int NOT NULL,
    InstanceName nvarchar(256) NOT NULL,
    Active bit NOT NULL,
    Deleted bit NOT NULL,
    PermissionType int NOT NULL,
    Virtualized bit NOT NULL,
    ServerVersion nvarchar(30)
  )
  CREATE TABLE #temp_MatchingLogins (
    MatchLoginSID varbinary(85) NOT NULL
  )
  CREATE TABLE #temp_Login (
    [account name] sysname,
    [type] char(8),
    [privilege] char(9),
    [mapped login name] sysname,
    [permission path] sysname
  )
  ----------------------------------------------------------------------------------------------------
  -- initialize flags
  ----------------------------------------------------------------------------------------------------
  SET @securityEnabled = 0
  SET @sysadmin = 0
  SET @sqldmAdministrator = 0

  ----------------------------------------------------------------------------------------------------
  -- check if security is enabled
  -- if ApplicationSecurityEnabled flag is there, check if its set, else create the flag and clear it
  ----------------------------------------------------------------------------------------------------
  IF ((SELECT
      COUNT([Internal_Value])
    FROM dbo.[RepositoryInfo]
    WHERE [Name] = 'ApplicationSecurityEnabled')
    = 1)
  BEGIN
    IF ((SELECT
        COUNT([Internal_Value])
      FROM dbo.[RepositoryInfo]
      WHERE [Name] = 'ApplicationSecurityEnabled'
      AND [Internal_Value] != 0)
      = 1)
      SET @securityEnabled = 1
    ELSE
      SET @securityEnabled = 0
  END
  ELSE
  BEGIN
    -- insert row for application security enabled flag and clear it
    INSERT INTO dbo.[RepositoryInfo]
      VALUES ('ApplicationSecurityEnabled', 0, '')

    SET @securityEnabled = 0
  END

  --SQLDM-29173/Insert login data to get user group permissions from his group
  BEGIN TRY
    IF @IsSQLLogin = 0
    BEGIN
      INSERT INTO #temp_Login
      EXEC master..xp_logininfo @acctname = @SQLLoginName
      SET @SQLLoginName = ISNULL((SELECT
        [permission path]
      FROM #temp_Login)
      , @SQLLoginName)
    END
  END TRY
  BEGIN CATCH
  END CATCH

  ----------------------------------------------------------------------------------------------------
  -- check if user is sysadmin
  ----------------------------------------------------------------------------------------------------
  --select * from sys.login_token
  SELECT
    @sysadmin = ISNULL(sysadmin, 0)
  FROM master..syslogins
  WHERE LOWER(name) = LOWER(@SQLLoginName);

  --if IS_SRVROLEMEMBER ('sysadmin') = 1
  --	set @sysadmin = 1
  --else
  --	set @sysadmin = 0
  ----------------------------------------------------------------------------------------------------
  -- if security is not enabled set administrator flag because everyone is an administrator
  -- else get the permissions assigned to the user.
  ----------------------------------------------------------------------------------------------------
  IF @securityEnabled = 0
  BEGIN
    SET @sqldmAdministrator = 1

    -- fill temp assigned servers table with all monitored SQL Servers
    -- with admin (2) permission
    INSERT INTO #temp_AssignedServers
      SELECT
        [SQLServerID],
        [InstanceName],
        [Active],
        [Deleted],
        CAST(3 AS int) AS [PermissionType],
        CASE
          WHEN [VmUID] IS NULL THEN 0
          ELSE 1
        END,
        [ServerVersion]
      FROM dbo.[MonitoredSQLServers]
      WHERE [Deleted] = 0
  END
  ELSE
  BEGIN
    -- if user is sysadmin they are administrator 
    -- else check the Permisssion table for administrator powers.
    IF @sysadmin = 1
    BEGIN
      -- set SQLdm admin flag
      SET @sqldmAdministrator = 1

      -- fill temp assigned servers table with all monitored SQL Servers
      -- with admin (2) permission
      INSERT INTO #temp_AssignedServers
        SELECT
          [SQLServerID],
          [InstanceName],
          [Active],
          [Deleted],
          CAST(3 AS int) AS [PermissionType],
          CASE
            WHEN [VmUID] IS NULL THEN 0
            ELSE 1
          END,
          [ServerVersion]
        FROM dbo.[MonitoredSQLServers]
        WHERE [Deleted] = 0
    END
    ELSE
    BEGIN
      -- determine matching logins
      IF @IsSQLLogin = 1
      BEGIN
        -- insert the matching login from syslogins table
        INSERT INTO #temp_MatchingLogins
          SELECT
            sid
          FROM master..syslogins
          WHERE LOWER(name) = LOWER(@SQLLoginName)
          AND isntname = 0
      END
      ELSE
      BEGIN
		-- insert the matching login from syslogins table
		INSERT INTO #temp_MatchingLogins
          SELECT
            sid
          FROM master..syslogins
          WHERE LOWER(name) = LOWER(@SQLLoginName)
          AND (isntname = 1
          OR isntgroup = 1) --checking only for windows logins
      END

      -- if matching logins are available get token information
      IF (SELECT
          COUNT(MatchLoginSID)
        FROM #temp_MatchingLogins)
        > 0
      BEGIN

        -- check if user has administrator power
        IF (SELECT
            COUNT(Permission)
          FROM [dbo].[Permission]
          WHERE Permission = 3 -- admin power
          AND LoginSID = (SELECT TOP 1
            MatchLoginSID
          FROM #temp_MatchingLogins)
          AND Enabled = 1)
          > 0
        BEGIN

          SET @sqldmAdministrator = 1
        END
        ELSE
        BEGIN

          SET @sqldmAdministrator = 0
        END

        -- if user is administrator assign all servers with admin powers
        -- else get only those servers assigned to the user
        IF @sqldmAdministrator = 1
        BEGIN
          INSERT INTO #temp_AssignedServers
            SELECT
              [SQLServerID],
              [InstanceName],
              [Active],
              [Deleted],
              CAST(3 AS int) AS [PermissionType],
              CASE
                WHEN [VmUID] IS NULL THEN 0
                ELSE 1
              END,
              [ServerVersion]
            FROM dbo.[MonitoredSQLServers]
            WHERE [Deleted] = 0
        END
        ELSE
        BEGIN
          INSERT INTO #temp_AssignedServers
            SELECT
              mss.SQLServerID,
              mss.InstanceName,
              mss.Active,
              mss.Deleted,
              p.Permission,
              CASE
                WHEN mss.[VmUID] IS NULL THEN 0
                ELSE 1
              END,
              mss.ServerVersion
            FROM [dbo].[Permission] AS p
            JOIN [dbo].[PermissionServers] AS s
              ON (p.[PermissionID] = s.[PermissionID])
            JOIN [dbo].[MonitoredSQLServers] AS mss
              ON (s.[SQLServerID] = mss.[SQLServerID])
            WHERE p.LoginSID IN (SELECT
              MatchLoginSID
            FROM #temp_MatchingLogins)
            AND p.Enabled = 1
            AND mss.Deleted = 0
          -- get servers assigned via tags
          INSERT INTO #temp_AssignedServers
            SELECT DISTINCT
              mss.SQLServerID,
              mss.InstanceName,
              mss.Active,
              mss.Deleted,
              p.Permission,
              CASE
                WHEN mss.[VmUID] IS NULL THEN 0
                ELSE 1
              END,
              mss.ServerVersion
            FROM [dbo].[Permission] AS p
            LEFT OUTER JOIN [dbo].[PermissionTags] pt
              ON (p.[PermissionID] = pt.[PermissionId])
            LEFT OUTER JOIN [dbo].[ServerTags] st
              ON (pt.[TagId] = st.[TagId])
            LEFT OUTER JOIN [dbo].[PermissionServers] s
              ON (p.[PermissionID] = s.[PermissionID]
              AND st.[SQLServerId] = s.[SQLServerID])
            JOIN [dbo].[MonitoredSQLServers] AS mss
              ON (st.[SQLServerId] = mss.[SQLServerID])
            WHERE p.LoginSID IN (SELECT
              MatchLoginSID
            FROM #temp_MatchingLogins)
            AND p.Enabled = 1
            AND mss.Deleted = 0
            AND s.SQLServerID IS NULL
        END
      END
    END
  END

  ----------------------------------------------------------------------------------------------------
  -- select flags & assigned servers
  ----------------------------------------------------------------------------------------------------
  IF (ISNULL(@ForReporting, 0) = 0)
  BEGIN
    SELECT
      @securityEnabled AS SecurityEnabled,
      @sysadmin AS SysAdmin,
      @sqldmAdministrator AS SQLdmAdministrator
    SELECT
      [SQLServerID],
      [InstanceName],
      [Active],
      [Deleted],
      [PermissionType]
    FROM #temp_AssignedServers
    WHERE [Virtualized] >= @VMOnly
  END
  ELSE
  BEGIN
    SELECT
      [InstanceName],
      [SQLServerID]
    FROM #temp_AssignedServers
    WHERE [Virtualized] >= @VMOnly
    AND SUBSTRING(ServerVersion, 0, CHARINDEX('.', ServerVersion)) >= @MinimumServerVersion
  END

  SELECT
    @e = @@error
  RETURN @e
END
GO

IF (object_id('p_GetUserTokenForWeb') is not null)
BEGIN
drop procedure p_GetUserTokenForWeb
END
GO

CREATE PROCEDURE [dbo].[p_GetUserTokenForWeb]
	@IsSQLLogin bit,
	@SQLLoginName nvarchar(128)
AS
BEGIN
	exec [p_GetUserTokenForWebInternal] @IsSQLLogin, @SQLLoginName, 0
END

GO