IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
	SELECT cntr_value AS locked_page_allocations_kb
	FROM sys.dm_os_performance_counters
	WHERE counter_name = 'Lock Memory (KB)';
ELSE
	SELECT NULL AS locked_page_allocations_kb
