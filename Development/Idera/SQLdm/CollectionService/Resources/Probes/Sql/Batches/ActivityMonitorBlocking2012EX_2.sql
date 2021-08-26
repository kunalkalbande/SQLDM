--------------------------------------------------------------------------------
--  Batch Segment: Activity Monitor Trace Extended Events Blockings 2012
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	Variables:  [0] - predicates
--  Set up trace events for SQL batches (Event ID 137 - SQL:Blocking )
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.blocked_process_report(
    ACTION(mdmtargetpkg.mdmget_TimeStampUTC,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.username,sqlserver.session_id,sqlserver.sql_text,sqlserver.username)) '

