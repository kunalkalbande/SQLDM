select 
	name, 
	description,
	minimum, 
	maximum,  
	config_value = value,
	run_value = value_in_use,
	restart_required = case when is_dynamic = 1 then 0 else 1 end
from 
	sys.configurations