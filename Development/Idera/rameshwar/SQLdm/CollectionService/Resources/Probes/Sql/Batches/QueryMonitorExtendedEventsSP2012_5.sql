--------------------------------------------------------------------------------
--  Batch Segment: Query Monitor Extended Event SP 2012 For Azure Managed Instance
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Tables: none
--  XSP:  none
--	  Variables:  [0] - predicates
-- [1] - if (CollectQueryPlan) then sqlserver.plan_handle,
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------
--  Set up trace events for stored procedures (Event ID 43 - SP:Completed sqlserver.module_end)
--------------------------------------------------------------------------------
' ADD EVENT sqlserver.module_end(SET collect_statement=(1)
	    ACTION(sqlos.task_time,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.nt_username,{1} 
		sqlserver.username,sqlserver.session_id){0}), '

