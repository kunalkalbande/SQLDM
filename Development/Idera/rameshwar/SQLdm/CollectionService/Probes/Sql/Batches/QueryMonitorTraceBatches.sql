--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace SQL Batches
--  Tables: none
--  XSP:  none
--	Variables:  none
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for SQL batches (Event ID 41 - SQL:StmtCompleted 
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
exec @rc = sp_trace_setevent @P1, 12, 1, @on 
exec @rc = sp_trace_setevent @P1, 12, 3, @on 
exec @rc = sp_trace_setevent @P1, 12, 6, @on 
exec @rc = sp_trace_setevent @P1, 12, 7, @on 
exec @rc = sp_trace_setevent @P1, 12, 8, @on 
exec @rc = sp_trace_setevent @P1, 12, 10, @on 
exec @rc = sp_trace_setevent @P1, 12, 11, @on 
exec @rc = sp_trace_setevent @P1, 12, 12, @on 
exec @rc = sp_trace_setevent @P1, 12, 13, @on 
exec @rc = sp_trace_setevent @P1, 12, 15, @on 
exec @rc = sp_trace_setevent @P1, 12, 16, @on 
exec @rc = sp_trace_setevent @P1, 12, 17, @on 
exec @rc = sp_trace_setevent @P1, 12, 18, @on 

