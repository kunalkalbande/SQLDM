----------------------------------------------------------------------------------------------
-- //SQLdm 10.0 - Srishti Puohit - New Recommendations - SDR-Q39, SDR-Q40, SDR-Q41,SDR-Q42 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------
select dbname = DB_NAME();
select actual_state, readonly_reason, remaining_space = (CASE WHEN max_storage_size_mb = 0 or max_storage_size_mb = null THEN 0 else ((max_storage_size_mb - current_storage_size_mb)*100/max_storage_size_mb) END) from sys.database_query_store_options;
select planName = name from sys.plan_guides where is_disabled = 0;
