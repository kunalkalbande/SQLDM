--------------------------------------------------------------------------------
--  Batch Segment: Activity Monitor Trace Extended Events Autogrow 2012
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	Variables:  [0] - predicates
--  Set up trace events for SQL batches (Event ID 92 - Data File Auto Grow
--  and 93 - Log File Auto Grow )
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.database_file_size_change(SET collect_database_name=(1)
    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.nt_username,sqlserver.session_id,sqlserver.sql_text,sqlserver.username){0}) '

