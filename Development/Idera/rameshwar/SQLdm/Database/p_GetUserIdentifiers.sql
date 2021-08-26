if (object_id('p_GetUserIdentifiers') is not null)
begin
drop procedure p_GetUserIdentifiers
end
go

CREATE PROCEDURE [dbo].[p_GetUserIdentifiers]
AS
BEGIN
	declare @securityEnabled bit
	declare @sysadmin bit
	declare @sqldmAdministrator bit
	declare @repositoryVersion int
	
	----------------------------------------------------------------------------------------------------
	-- initialize flags
	----------------------------------------------------------------------------------------------------
	set @securityEnabled = 0
	set @sysadmin = 0
	set @sqldmAdministrator = 0

	declare @temp_MatchingLogins table (MatchLoginSID varbinary(85) not null)
	
	----------------------------------------------------------------------------------------------------
	-- check if security is enabled
	-- if ApplicationSecurityEnabled flag is there, check if its set, else create the flag and clear it
	----------------------------------------------------------------------------------------------------
	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled') = 1)
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

	if (@securityEnabled = 0 or @sysadmin = 1)
	begin
		set @sqldmAdministrator = 1;
	end

	-- determine the version of SQLdm repository.
	select @repositoryVersion = CAST(SUBSTRING(CONVERT(char(20), SERVERPROPERTY('ProductVersion')),1,1) as int)
				
	select @securityEnabled, @sysadmin, @sqldmAdministrator, @repositoryVersion

	-- if SQL Server 2000, just insert the input windows user sid into temp table because
	-- SQL Server 2000 does not have support for getting the user token.
	-- if SQL Server 2005 or higher get the sids from the login token.
	if @repositoryVersion = 8
		select SYSTEM_USER, SUSER_SID(SYSTEM_USER)
	else				
		exec ('select name, sid from sys.login_token where type like ''% LOGIN'' or type like ''% GROUP''')					
END