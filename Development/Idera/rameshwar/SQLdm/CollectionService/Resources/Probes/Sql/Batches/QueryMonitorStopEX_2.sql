--------------------------------------------------------------------------------
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Batch: Query Monitor Stop Extended Event Session
--  Tables: none
--  XSP:  none
--------------------------------------------------------------------------------\
IF IS_ROLEMEMBER ( 'db_datawriter', USER_NAME()  )  = 1 and HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'CONTROL') = 1
BEGIN
DECLARE @sessionName nvarchar(128)

SELECT @sessionName = replace(host_name(),' ','') + 'SQLdmQMEXevents'

IF EXISTS (SELECT * FROM sys.dm_xe_database_sessions where name = @sessionName) -- SQLdm 9.0 (Ankit Srivastava) -- changed the condition to check among only the started sessions
BEGIN
	EXEC('ALTER EVENT SESSION [' + @sessionName + '] ON DATABASE state = stop')
END
END
