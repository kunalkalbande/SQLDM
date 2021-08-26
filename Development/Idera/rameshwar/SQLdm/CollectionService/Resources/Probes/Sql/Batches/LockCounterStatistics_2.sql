--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  QUERY: Lock Counter Statistics 2005
--  Tables: sys.dm_os_performance_counters
--  Returns: 
--    Counter name (counter_name)
--    Instance name (instance_name)
--    Value of counter (calculated)
--  Join on: none
--  Criteria: 
--  (1) For counter names sqlserver:locks and sqlserver:latches
--  Calculations:
--  (1) If the counter value is less than 0, attempt to convert to positive int
--------------------------------------------------------------------------------

declare @sysperfinfoname sysname
declare @objectnamelocks nchar(128), @objectnamelatches nchar(128)

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))

--extract one row that contains the actual instance name and exclude others
--there should be only one instance name
if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
	select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)

Set @objectnamelocks = @sysperfinfoname + ':locks'
Set @objectnamelatches = @sysperfinfoname + ':latches'

if HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1 
BEGIN
Select
	counter_name,
	instance_name,
	case
		when cntr_value >= 0 then convert(dec(20,0),cntr_value)
		else convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1
	end
from sys.dm_os_performance_counters with (nolock)
where object_name in (@objectnamelocks, @objectnamelatches);
END
ELSE
	SELECT NULL, NULL, NULL