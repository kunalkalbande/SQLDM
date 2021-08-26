--------------------------------------------------------------------------------
--  Batch: Query Monitor Stop Trace 2000
--  Tables: none
--  XSP:  none
--	Variables: [0] - Restart variable name
--------------------------------------------------------------------------------\ 
declare @{0} int

if exists (select * from tempdb..sysobjects where id = object_id('tempdb..SQLdm_Query_Monitor_Traces') and type = 'U') 
begin 
set @{0} = 1
	while (@{0} is not null)
	begin
		set @{0} = null
		select 
			@{0} = min(queue_number)
		from 
			tempdb..SQLdm_Query_Monitor_Traces 
		where 
			host_name = HOST_NAME() 

		if @{0} is not null
		begin 
			exec sp_trace_setstatus @{0}, 0 
			exec sp_trace_setstatus @{0}, 2 
			delete from tempdb..SQLdm_Query_Monitor_Traces where queue_number = @{0}
		end 
	end
end 