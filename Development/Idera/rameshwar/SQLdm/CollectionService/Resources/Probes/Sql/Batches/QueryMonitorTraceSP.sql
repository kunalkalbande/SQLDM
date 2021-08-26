--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace SP
--  Tables: none
--  XSP:  none
--	Variables:  none
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for stored procedures (Event ID 43 - SP:Completed)
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
exec @rc = sp_trace_setevent @P1, 43, 1, @on 
exec @rc = sp_trace_setevent @P1, 43, 3, @on 
exec @rc = sp_trace_setevent @P1, 43, 6, @on 
exec @rc = sp_trace_setevent @P1, 43, 7, @on 
exec @rc = sp_trace_setevent @P1, 43, 8, @on 
exec @rc = sp_trace_setevent @P1, 43, 10, @on 
exec @rc = sp_trace_setevent @P1, 43, 11, @on 
exec @rc = sp_trace_setevent @P1, 43, 12, @on 
exec @rc = sp_trace_setevent @P1, 43, 13, @on 
exec @rc = sp_trace_setevent @P1, 43, 15, @on 
exec @rc = sp_trace_setevent @P1, 43, 16, @on 
exec @rc = sp_trace_setevent @P1, 43, 17, @on 
exec @rc = sp_trace_setevent @P1, 43, 18, @on 

