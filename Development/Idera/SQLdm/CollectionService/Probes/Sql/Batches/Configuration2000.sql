select 
	name, 
	c.comment,
	minimum = low, 
	maximum = high,  
	config_value = c.value,
	run_value = master.dbo.syscurconfigs.value,
	restart_required = case when (c.status = 0) or (c.status = 2) then 1 else 0 end
from 
	master.dbo.spt_values, 
	master.dbo.sysconfigures c, 
	master.dbo.syscurconfigs
where 
	type = 'C'
	and number = c.config
    and number = master.dbo.syscurconfigs.config
