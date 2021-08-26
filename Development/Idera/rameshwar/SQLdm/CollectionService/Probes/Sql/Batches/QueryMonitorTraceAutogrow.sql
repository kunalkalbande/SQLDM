--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace Autogrow
--  Tables: none
--  XSP:  none
--	Variables: [0] - Trace ID (@P1 or @P2)
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for Autogrow (Event ID 92 - Data File Auto Grow
--  and 93 - Log File Auto Grow)
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
exec @rc = sp_trace_setevent {0}, 92, 1, @on 
exec @rc = sp_trace_setevent {0}, 92, 3, @on 
exec @rc = sp_trace_setevent {0}, 93, 1, @on 
exec @rc = sp_trace_setevent {0}, 93, 3, @on 


