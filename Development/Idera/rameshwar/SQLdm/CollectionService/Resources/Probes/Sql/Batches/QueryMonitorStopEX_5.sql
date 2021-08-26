--------------------------------------------------------------------------------
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Batch: Query Monitor Stop Extended Event Session For Azure Managed Instance
--  Tables: none
--  XSP:  none
--------------------------------------------------------------------------------\

DECLARE @sessionName nvarchar(128)

SELECT @sessionName = '1' + replace(host_name(),' ','') + 'SQLdmQMEXevents'

IF EXISTS (SELECT * FROM sys.dm_xe_sessions where name = @sessionName) -- SQLdm 9.0 (Ankit Srivastava) -- changed the condition to check among only the started sessions
BEGIN
	EXEC('ALTER EVENT SESSION [' + @sessionName + '] ON SERVER state = stop')
END

