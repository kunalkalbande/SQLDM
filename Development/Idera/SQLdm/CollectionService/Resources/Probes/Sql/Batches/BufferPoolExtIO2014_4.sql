----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-M33 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------


select state from sys.dm_os_buffer_pool_extension_configuration;

Select cntr_value from sys.dm_os_performance_counters
where object_name LIKE '%Buffer Manager%'
and counter_name = 'Extension outstanding IO counter';




