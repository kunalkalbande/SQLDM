--------------------------------------------------------------------------------
--  QUERY: Lock Counter Statistics
--  Tables: master..sysperfinfo
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
select 
	counter_name, 
	instance_name, 
	case 
		when cntr_value >= 0 then convert(dec(20,0),cntr_value)  
		else convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1 
	end 
from master..sysperfinfo (nolock) 
where lower(object_name) in (lower(@sysperfinfoname + ':locks'), lower(@sysperfinfoname + ':latches'))
