--------------------------------------------------------------------------------
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Batch Segment: Query Monitor Extended Events Single SQL Statements 2008
--  Tables: none
--  XSP:  none
--	Variables:  [0] - predicates
-- [1] - if (CollectQueryPlan) then sqlserver.plan_handle,
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for single SQL statements (Event ID 41 - SQL:StmtCompleted (sqlserver.sql_statement_completed)
--  and SP:StmtCompleted (sqlserver.sp_statement_completed ))
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.sp_statement_completed(
	    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,
		--sqlserver.database_name,
		sqlserver.nt_username,{1} sqlserver.username,sqlserver.session_id,sqlserver.sql_text){0}),
	ADD EVENT sqlserver.sql_statement_completed(
	    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,
		--sqlserver.database_name,
		sqlserver.nt_username,{1} sqlserver.username,sqlserver.session_id,sqlserver.sql_text){0}) '

