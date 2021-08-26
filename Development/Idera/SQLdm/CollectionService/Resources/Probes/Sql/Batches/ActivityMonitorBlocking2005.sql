--------------------------------------------------------------------------------
--  Batch Segment: Activity Monitor Trace SQL Blocking processes
--  Tables: none
--  XSP:  none
--	Variables:  none
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for SQL blocking
--  Columns:
--  1 - TextData
--  13 - Duration
--  14 - StartTime
--  15 - EndTime
--  22 - objectid
--------------------------------------------------------------------------------
exec @rc = sp_trace_setevent @P1, 137, 1, @on 
exec @rc = sp_trace_setevent @P1, 137, 13, @on
exec @rc = sp_trace_setevent @P1, 137, 14, @on 
exec @rc = sp_trace_setevent @P1, 137, 15, @on
exec @rc = sp_trace_setevent @P1, 137, 22, @on