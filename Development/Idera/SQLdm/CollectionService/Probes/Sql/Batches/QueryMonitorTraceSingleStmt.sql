--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace Single SQL Statements
--  Tables: none
--  XSP:  none
--	Variables:  none
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for single SQL statements (Event ID 41 - SQL:StmtCompleted 
--  and SP:StmtCompleted)
--  Columns:
--  1 - TextData
--  3 - DatabaseID
--  6 - NTUserName
--  7 - NTDomainName
--  8 - HostName
--  10 - ApplicationName
--  11 - LoginName
--  13 - Duration (milliseconds 2000 / microseconds 2005)
--  15 - EndTime
--  16 - Logical Disk Reads
--  17 - Physical disk writes
--  18 - CPU time
--------------------------------------------------------------------------------
exec @rc = sp_trace_setevent @P1, 41, 1, @on 
exec @rc = sp_trace_setevent @P1, 41, 3, @on 
exec @rc = sp_trace_setevent @P1, 41, 6, @on 
exec @rc = sp_trace_setevent @P1, 41, 7, @on 
exec @rc = sp_trace_setevent @P1, 41, 8, @on 
exec @rc = sp_trace_setevent @P1, 41, 10, @on 
exec @rc = sp_trace_setevent @P1, 41, 11, @on 
exec @rc = sp_trace_setevent @P1, 41, 12, @on 
exec @rc = sp_trace_setevent @P1, 41, 13, @on 
exec @rc = sp_trace_setevent @P1, 41, 15, @on 
exec @rc = sp_trace_setevent @P1, 41, 16, @on 
exec @rc = sp_trace_setevent @P1, 41, 17, @on 
exec @rc = sp_trace_setevent @P1, 41, 18, @on 
exec @rc = sp_trace_setevent @P1, 45, 1, @on 
exec @rc = sp_trace_setevent @P1, 45, 3, @on 
exec @rc = sp_trace_setevent @P1, 45, 6, @on 
exec @rc = sp_trace_setevent @P1, 45, 7, @on 
exec @rc = sp_trace_setevent @P1, 45, 8, @on 
exec @rc = sp_trace_setevent @P1, 45, 10, @on 
exec @rc = sp_trace_setevent @P1, 45, 11, @on 
exec @rc = sp_trace_setevent @P1, 45, 12, @on 
exec @rc = sp_trace_setevent @P1, 45, 13, @on 
exec @rc = sp_trace_setevent @P1, 45, 15, @on 
exec @rc = sp_trace_setevent @P1, 45, 16, @on 
exec @rc = sp_trace_setevent @P1, 45, 17, @on 
exec @rc = sp_trace_setevent @P1, 45, 18, @on
exec @rc = sp_trace_setevent @P1, 45, 34, @on 

