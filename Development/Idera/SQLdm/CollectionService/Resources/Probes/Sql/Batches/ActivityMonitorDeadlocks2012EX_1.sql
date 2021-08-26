--------------------------------------------------------------------------------
--  Batch Segment: Activity Monitor Trace Extended Events Deadlocks 2012
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	Variables:  [0] - predicates
--  Set up trace events for SQL batches (Event ID 148 - Deadlocks )
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.xml_deadlock_report(
    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.nt_username,sqlserver.session_id,sqlserver.sql_text,sqlserver.username){0})'