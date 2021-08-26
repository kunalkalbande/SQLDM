select sum(cast(size_in_bytes as bigint)) from sys.dm_exec_cached_plans 
	where usecounts = 1 
	and objtype = 'Adhoc';
