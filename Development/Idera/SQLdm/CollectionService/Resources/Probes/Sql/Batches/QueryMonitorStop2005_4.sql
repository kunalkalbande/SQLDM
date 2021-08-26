--------------------------------------------------------------------------------
--  Batch: Query Monitor Stop Trace 2005
--  Tables: none
--  XSP:  none
--	Variables: [0] - Restart variable name
--------------------------------------------------------------------------------\
declare 
	@{0} int, 
	@tracefilenamedelete{0} nvarchar(4000) 

set @tracefilenamedelete{0} = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @tracefilenamedelete{0} = left(@tracefilenamedelete{0},len(@tracefilenamedelete{0}) - charindex('\',reverse(@tracefilenamedelete{0}))) + '\' + replace(host_name(),' ','') + 'dm5wptrace'   

select 
	@{0} = traceid 
from ::fn_trace_getinfo(0) 
	where property = 2 
	and convert(nvarchar(4000),value) like @tracefilenamedelete{0} + '%' 

if @@rowcount > 0 
begin 
	exec sp_trace_setstatus @{0}, 0 
	exec sp_trace_setstatus @{0}, 2 
end 