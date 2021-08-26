---------- MINIMUMPERMISSIONSONPREMISES.SQL
/*----------------------------- Idera SQL diagnostic manager -------------------------------
**
**	Copyright Idera, Inc. 2005-2012
**		All rights reserved
**
**------------------------------------------------------------------------------------------
**
**	Description:  SQL script to create new user with minimum permissions necessary for​ the product​​ to​​ function​​​ 
**  Remarks:      The same script is applicable for Managed Azure Managed Instance
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

/****** GRANT SERVER LEVEL PERMISSIONS******/

/** Assign VIEW SERVER STATE Permissions to SQLdmConsoleUser **/
GRANT VIEW SERVER STATE TO [SQLdmConsoleUser]

/** Assign VIEW ANY DEFINITION Permissions to SQLdmConsoleUser **/
GRANT VIEW ANY DEFINITION TO [SQLdmConsoleUser]

/** Assign VIEW ANY DATABASE Permissions to SQLdmConsoleUser **/
GRANT VIEW​ ANY DATABASE TO [SQLdmConsoleUser]

GO