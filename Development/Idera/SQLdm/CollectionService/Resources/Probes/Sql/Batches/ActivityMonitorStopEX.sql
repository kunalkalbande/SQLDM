--------------------------------------------------------------------------------
--  Batch: Activity Monitor Stop Extended Event Session
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--------------------------------------------------------------------------------\

declare @sessionName nvarchar(128)

select @sessionName = replace(host_name(),' ','') + 'SQLdmAMEXevents'


if  EXISTS (select * from sys.dm_xe_sessions where name = @sessionName) 
begin
	exec('Alter event session [' + @sessionName + '] on server state = stop')
end

