--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Trace Extended Events Batches 2012 for Azure Managed Instance
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	  Variables:  [0] - predicates
-- [1] - if (CollectQueryPlan) then sqlserver.plan_handle,
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for SQL batches (Event ID 41 - SQL:BatchCompleted - (sqlserver.sql_batch_completed))
--------------------------------------------------------------------------------
	' ADD EVENT sqlserver.sql_batch_completed(SET collect_batch_text=(1)
	    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.nt_username,{1} sqlserver.username,
		sqlserver.server_principal_name,sqlserver.session_id){0}) '

