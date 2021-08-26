--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Extended Events Single SQL Statements 2012
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	  Variables:  [0] - predicates
-- [1] - if (CollectQueryPlan) then sqlserver.plan_handle,
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for single SQL statements (Event ID 41 - SQL:StmtCompleted (sqlserver.sql_statement_completed)
--  and 45- SP:StmtCompleted (sqlserver.sp_statement_completed ))
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.sp_statement_completed(SET collect_object_name=(1),collect_statement=(1)
	    ACTION(mdmtargetpkg.mdmget_TimeStampUTC,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.username,{1} 
		sqlserver.session_id){0}), 
	ADD EVENT sqlserver.sql_statement_completed(SET collect_parameterized_plan_handle=(0),collect_statement=(1)
	    ACTION(mdmtargetpkg.mdmget_TimeStampUTC,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.username,{1} 
		sqlserver.session_id){0}), '

