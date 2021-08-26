declare @P1 int, @rowcount int

if not exists (select * from tempdb..sysobjects where id = object_id('tempdb..SQLdm_Query_Monitor') and type = 'U') 
	create table tempdb..SQLdm_Query_Monitor (
		host_name varchar(255), 
		queue_number int) 

select 
	@P1 = queue_number 
from 
	tempdb..SQLdm_Query_Monitor 
where 
	host_name = HOST_NAME() 

set @rowcount = @@rowcount

select @rowcount

if @rowcount > 0 
begin
	select 
		columnid,	
		value 
	from 
		::fn_trace_getfilterinfo(@P1)

	select distinct 
		eventid 
	from 
		::fn_trace_geteventinfo(@P1) 
end

