---------- RECOMMENDEDPERMISSIONSONPREMISES.SQL
/*----------------------------- Idera SQL diagnostic manager -------------------------------
**
**	Copyright Idera, Inc. 2005-2012
**		All rights reserved
**
**------------------------------------------------------------------------------------------
**
**	Description:  SQL script to create new user with minimum permissions necessary for​ the product​​ to​​ function​​​ and permissions​​ required for​​ trace​​ and​ xevent​ manipulation
**  Remarks:      The same script is applicable for Managed Azure Managed Instance
**
**------------------------------------------------------------------------------------------
**
**	Instructions: 
**		1. Use the Find/Replace function to locate and replace all references to 
**			'SQLdmConsoleUser' with the name of your SQLdm Monitor User.
**		2. Connect to the SQL Server that you wanted to monitor using your SQLdm and 
**			execute this script.
**	
**------------------------------------------------------------------------------------------

*/
 
USE [master]
GO

/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [SQLdmConsoleUser]    Script Version: 10.3            ******/
CREATE LOGIN [SQLdmConsoleUser] WITH PASSWORD=N'eAP3rYMLRCtCAWKh6mqDgGIuaaS8Umqm1Go0xCGD6GU=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=ON, CHECK_POLICY=ON
GO

-- Uncomment to disable newly created user for extra security
--ALTER LOGIN [SQLdmConsoleUser] DISABLE
--GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_StartGrantTempDB')
	DROP PROCEDURE p_StartGrantTempDB
GO

CREATE PROCEDURE p_StartGrantTempDB 
AS
BEGIN

	SET NOCOUNT ON;

	declare @dmusername varchar(32);
	select @dmusername = 'SQLdmConsoleUser';

	declare @sql varchar(4096);
	select @sql = 'USE tempdb;' + char(13)
	+ 'CREATE USER [' + @dmusername + '];' + char(13)

	+ 'EXECUTE sp_addrolemember @rolename = ''db_datareader'',  @membername = '''+ @dmusername + '''  ;' + char(13)
	+ 'EXECUTE sp_addrolemember @rolename = ''db_datawriter'',  @membername = ''' + @dmusername + '''  ;' + char(13)
	+ 'EXECUTE sp_addrolemember @rolename = ''db_owner'', @membername = ''' + @dmusername + ''' ;' + char(13)
	+ 'GRANT CREATE TABLE to [' + @dmusername + '];' + char(13)
	;

exec(@sql);
END
GO

exec sp_procoption 'p_StartGrantTempDB', 'startup', 'true'
go 

/****** GRANT DATABASE LEVEL PERMISSIONS******/
CREATE USER [SQLdmConsoleUser] FOR LOGIN [SQLdmConsoleUser]
GO
exec sp_addrolemember 'db_datareader', 'SQLdmConsoleUser' 
GO
GRANT EXECUTE on xp_loginconfig to [SQLdmConsoleUser];
GO

GRANT EXECUTE  on xp_regread to [SQLdmConsoleUser];
GO

/****** GRANT SERVER LEVEL PERMISSIONS******/

/** Assign VIEW SERVER STATE Permissions to SQLdmConsoleUser **/
GRANT VIEW SERVER STATE TO [SQLdmConsoleUser]

/** Assign VIEW ANY DEFINITION Permissions to SQLdmConsoleUser **/
GRANT VIEW ANY DEFINITION TO [SQLdmConsoleUser]

/** Assign VIEW ANY DATABASE Permissions to SQLdmConsoleUser **/
GRANT VIEW​ ANY DATABASE TO [SQLdmConsoleUser]

/** Assign CREATE ANY DATABASE Permissions to SQLdmConsoleUser **/
/** TODO:  This could be a problem.  May be able to be replaced with component privs. **/
-- GRANT CREATE ANY DATABASE TO [SQLdmConsoleUser]

/****** GRANT ADDITIONAL XEVENTS AND TRACE PERMISSIONS******/

/** Assign ALTER TRACE Permissions to SQLdmConsoleUser **/
GRANT ALTER TRACE TO [SQLdmConsoleUser]

/** Assign ALTER ANY EVENT SESSION Permissions to SQLdmConsoleUser **/
GRANT ALTER ANY EVENT SESSION TO [SQLdmConsoleUser]

/***************** Set up tempdb **********************/
USE tempdb;
GO
CREATE USER [SQLdmConsoleUser];
GO
EXECUTE sp_addrolemember @rolename = 'db_datareader',  @membername = 'SQLdmConsoleUser'  ;
GO
EXECUTE sp_addrolemember @rolename = 'db_datawriter',  @membername = 'SQLdmConsoleUser'  ;
GO
EXECUTE sp_addrolemember @rolename = 'db_owner', @membername = 'SQLdmConsoleUser' 
GO

-- Added this after observing an error in Collection Service log.
GRANT CREATE TABLE to [SQLdmConsoleUser];
GO