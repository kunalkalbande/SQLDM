IF (object_id('p_IsUserAdministrator') is not null)
BEGIN
drop procedure p_IsUserAdministrator
END
GO

CREATE PROCEDURE [dbo].[p_IsUserAdministrator]
	@IsSQLLogin bit,
	@SQLLoginName nvarchar(128),
	@WindowsSID varbinary(85),
	@PermissionID int
AS
BEGIN
	declare @e int
	declare @securityEnabled bit
	declare @sysadmin bit
	declare @sqldmAdministrator bit
	declare @repositoryVersion int
	
	----------------------------------------------------------------------------------------------------
	-- create temp table to hold the list of matching logins
	----------------------------------------------------------------------------------------------------
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
		set @sqldmAdministrator = 1
	else
	begin
		-- if user is sysadmin they are administrator 
		-- else check the Permisssion table for administrator powers.
		if @sysadmin = 1
		begin
			-- set SQLdm admin flag
			set @sqldmAdministrator = 1
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
				-- determine the version of SQLdm repository.
				select @repositoryVersion = CAST(SUBSTRING(CONVERT(char(20), SERVERPROPERTY('ProductVersion')),1,1) as int)
				
				-- if SQL Server 2000, just insert the input windows user sid into temp table because
				-- SQL Server 2000 does not have support for getting the user token.
				-- if SQL Server 2005 or higher get the sids from the login token.
				if @repositoryVersion = 8
					insert into #temp_MatchingLogins values(@WindowsSID)
				else				
					insert into #temp_MatchingLogins
					exec ('select sid from sys.login_token where type = ''WINDOWS LOGIN'' or type = ''WINDOWS GROUP''')				
			end
			
			-- if matching logins are available get token information
			if (select count(MatchLoginSID) from #temp_MatchingLogins) > 0
			begin

				-- check if user has administrator power with specified permisison ID.
				if (select count(Permission) 
					from [dbo].[Permission]
						where Permission = 3 -- admin power
							and LoginSID in (select MatchLoginSID from #temp_MatchingLogins)
								and Enabled = 1
									and PermissionID != @PermissionID) > 0
					set @sqldmAdministrator = 1
				else
					set @sqldmAdministrator = 0
			end
		end
	end

	----------------------------------------------------------------------------------------------------
	-- select flags & assigned servers
	----------------------------------------------------------------------------------------------------
	select @sqldmAdministrator as SQLdmAdministrator
	
	select @e = @@error
	return @e
END
GO
