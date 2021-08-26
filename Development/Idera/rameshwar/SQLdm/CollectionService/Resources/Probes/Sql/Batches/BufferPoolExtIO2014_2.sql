----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-M33 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------

DECLARE @BufferPoolExt1 TABLE([state] INT);
SELECT [state] FROM @BufferPoolExt1;

if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
	Select cntr_value from sys.dm_os_performance_counters
	where object_name LIKE '%Buffer Manager%'
	and counter_name = 'Extension outstanding IO counter';
ELSE
	SELECT [state] AS cntr_value FROM @BufferPoolExt1;

