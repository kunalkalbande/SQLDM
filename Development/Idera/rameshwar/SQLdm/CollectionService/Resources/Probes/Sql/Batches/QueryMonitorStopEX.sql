--------------------------------------------------------------------------------
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Batch: Query Monitor Stop Extended Event Session
--  Tables: none
--  XSP:  none
--------------------------------------------------------------------------------\

declare @sessionName nvarchar(128)

select @sessionName = replace(host_name(),' ','') + 'SQLdmQMEXevents'


if  EXISTS (select * from sys.dm_xe_sessions where name = @sessionName) -- SQLdm 9.0 (Ankit Srivastava) -- changed the condition to check among only the started sessions
begin
	exec('Alter event session [' + @sessionName + '] on server state = stop')
end

