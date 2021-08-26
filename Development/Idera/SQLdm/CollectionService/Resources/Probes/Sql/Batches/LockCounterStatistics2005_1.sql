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

Declare @objectnamelocks nchar(128), @objectnamelatches nchar(128)

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
If @sysperfinfoname Is Null
       Set @sysperfinfoname = N'SQLServer';
Else
       Set @sysperfinfoname = N'MSSQL$' + @sysperfinfoname;

Set @objectnamelocks = @sysperfinfoname + ':Locks'
Set @objectnamelatches = @sysperfinfoname + ':Latches'

Select
	counter_name, 
	instance_name,
	case 
		when cntr_value >= 0 then convert(dec(20,0),cntr_value)  
		else convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1 
	end 
From sys.dm_os_performance_counters with(nolock)
where object_name in (@objectnamelocks, @objectnamelatches);
