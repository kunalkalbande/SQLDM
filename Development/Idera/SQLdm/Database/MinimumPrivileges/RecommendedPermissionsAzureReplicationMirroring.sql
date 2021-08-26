﻿---------- RECOMMENDEDPERMISSIONSONPREMISESREPLICATIONMIRRORING.SQL
/*----------------------------- Idera SQL diagnostic manager -------------------------------
**
**	Copyright Idera, Inc. 2005-2012
**		All rights reserved
**
**------------------------------------------------------------------------------------------
**
**	Description:  SQL script to create new user with minimum permissions necessary for​ the product​​ to​​ function​​​ and additional permissions​​ required for​​ trace​​, xevent​ manipulation, mirror and replication
**
**------------------------------------------------------------------------------------------
**
**	Instructions: 
**		1. Use the Find/Replace function to locate and replace all references to 
**			'SQLdmConsoleUser' with the name of your SQLdm Monitor User.
**		2. Connect to the Azure SQL Server and execute the first section against the master database 
**			and second section against the target databases.
**	
**------------------------------------------------------------------------------------------
*/
/*----------------------------- Start of First Section -------------------------------
**
**	Description:  This section should be executed against the master database
**
**------------------------------------------------------------------------------------------
**
**	Instructions:
**		1. Connect to the Azure SQL Server master database.
**		2. Comment the second section and execute the first section against the master database.
**	
**------------------------------------------------------------------------------------------
*/
USE [master]
GO

/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [SQLdmConsoleUser]    Script Version: 10.3            ******/
CREATE LOGIN [SQLdmConsoleUser]
	WITH PASSWORD = N'eAP3rYMLRCtCAWKh6mqDgGIuaaS8Umqm1Go0xCGD6GU='
GO

-- Uncomment to disable newly created user for extra security
--ALTER LOGIN [SQLdmConsoleUser] DISABLE
--GO

IF EXISTS (
		SELECT 1
		FROM sys.database_principals
		WHERE (
				type = 'S'
				OR type = 'U'
				)
			AND name = 'SQLdmConsoleUser'
		)
BEGIN
	DROP USER [SQLdmConsoleUser]
END

CREATE USER [SQLdmConsoleUser]
FROM LOGIN [SQLdmConsoleUser];
GO

-- dbmanager (similar to dbcreator) that can create and drop databases
ALTER ROLE dbmanager ADD MEMBER [SQLdmConsoleUser]

-- loginmanager (similar to securityadmin) that can create new logins
ALTER ROLE loginmanager ADD MEMBER [SQLdmConsoleUser]

----------------------------- End of First Section -------------------------------
/*----------------------------- Start of Second Section -------------------------------
**
**	Description:  This section should be executed against the target databases
**
**------------------------------------------------------------------------------------------
**
**	Instructions:
**		1. Connect to the Azure SQL Server target database.
**		2. Comment the first section and execute the below second section against the target database.
**		3. Repeat for all other target database
**	
**------------------------------------------------------------------------------------------
*/
IF EXISTS (
		SELECT 1
		FROM sys.database_principals
		WHERE (
				type = 'S'
				OR type = 'U'
				)
			AND name = 'SQLdmConsoleUser'
		)
BEGIN
	DROP USER [SQLdmConsoleUser]
END

CREATE USER [SQLdmConsoleUser]
FROM LOGIN [SQLdmConsoleUser];

/****** GRANT DATABASE LEVEL PERMISSIONS******/
/** Assign Reader Permissions to SQLdmConsoleUser **/
ALTER ROLE db_datareader ADD MEMBER [SQLdmConsoleUser]

/** Assign VIEW ANY DEFINITION Permissions to SQLdmConsoleUser **/
GRANT VIEW DEFINITION
	TO [SQLdmConsoleUser]

/** Assign VIEW DATABASE STATE Permissions to SQLdmConsoleUser **/
GRANT VIEW DATABASE STATE
	TO [SQLdmConsoleUser]

/** Assign Execute Permissions to SQLdmConsoleUser **/
GRANT EXECUTE
	TO [SQLdmConsoleUser]

/** Assign SELECT Permissions to SQLdmConsoleUser **/
GRANT SELECT
	TO [SQLdmConsoleUser]

/** Assign Connect Permissions to SQLdmConsoleUser **/
GRANT CONNECT
	TO [SQLdmConsoleUser]

/****** GRANT ADDITIONAL PERMISSIONS******/
/** Assign CONTROL Permissions to SQLdmConsoleUser **/
GRANT CONTROL
	TO [SQLdmConsoleUser]

-- /** Assign Data Writer Permissions to SQLdmConsoleUser **/
-- ALTER ROLE db_datawriter ADD MEMBER [SQLdmConsoleUser]

/****** GRANT ADDITIONAL REPLICATION AND MIRROR PERMISSIONS******/
/** Assign DB Owner Permissions to SQLdmConsoleUser **/
EXEC sp_addrolemember N'db_owner'
	,N'SQLdmConsoleUser'
GO
----------------------------- End of Second Section -------------------------------


