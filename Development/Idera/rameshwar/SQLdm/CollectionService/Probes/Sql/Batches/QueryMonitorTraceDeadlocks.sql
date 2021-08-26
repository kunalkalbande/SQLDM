--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace SQL Deadlocks
--  Tables: none
--  XSP:  none
--	Variables:  none
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for SQL deadlocks
--  Columns:
--  1 - TextData
--  14 - StartTime
--------------------------------------------------------------------------------
exec @rc = sp_trace_setevent @P1, 148, 1, @on 
exec @rc = sp_trace_setevent @P1, 148, 14, @on 
