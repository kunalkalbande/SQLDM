--------------------------------------------------------------------------------
--  Batch: Activity Monitor Stop Extended Event Session
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--------------------------------------------------------------------------------\

declare @sessionName nvarchar(128), @deletesessionName nvarchar(128)
select @sessionName = '1'+replace(host_name(),' ','') + 'SQLdmAMEXevents%'


while ( EXISTS (select Top(1) name from sys.dm_xe_sessions where name like @sessionName) )
begin
    print 'Deleted session';
    Set @deletesessionName =(select Top(1)name from sys.dm_xe_sessions where name like @sessionName)
	exec('Alter event session [' +@deletesessionName + '] on server state = stop')
	EXEC ('DROP EVENT SESSION [' + @deletesessionName+'] ON  SERVER')
	
end