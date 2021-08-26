declare 
	@P1 int, 
	@tracefilename as nvarchar(4000),
	@rowcount int

set @tracefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @tracefilename = left(@tracefilename,len(@tracefilename) - charindex('\',reverse(@tracefilename))) + '\' + replace(host_name(),' ','') + 'dmwptrace'

select 
	@P1 = traceid 
from 
	::fn_trace_getinfo(0) 
where 
	property = 2 
	and lower(convert(nvarchar(4000),value)) like lower(@tracefilename) + '%' 

set @rowcount = @@rowcount

select @rowcount

if @rowcount > 0 
begin
	select 
		columnid,	
		value 
	from 
		fn_trace_getfilterinfo(@P1)
	where
		columnid != 1

	select distinct 
		eventid 
	from 
		fn_trace_geteventinfo(@P1) 
	where
		eventid != 82

	select
		value 
	from 
		fn_trace_getinfo(@P1)
	where
		property = 3
end


