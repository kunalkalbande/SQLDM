--------------------------------------------------------------------------------
--  Batch: Query Monitor Trace Restart
--  Tables: tempdb..SQLdm_Query_Monitor 
--  XSP:  none
--	Variables:  [0] - Query Monitor Trace Batch Segments
--  [1] - Query Monitor Filter Statements
--  [2] - Query Stop Time
--------------------------------------------------------------------------------\
declare 
	@stoptimerestart datetime

set @stoptimerestart = {2}
set @stoptimerestart = dateadd(hh,-datediff(hh,getdate(),getutcdate()),@stoptimerestart)


exec @rc = sp_trace_create @P1 output, 1, NULL, NULL, @stoptimerestart 

if isnull(@P2,0) <> 0
	exec @rc = sp_trace_create @P2 output, 1, NULL, NULL, @stoptimerestart 

if @P1 <> 0 
	insert into tempdb..SQLdm_Query_Monitor_Traces 
	values (HOST_NAME(), @P1, 0) 

if isnull(@P2,0) <> 0 
	insert into tempdb..SQLdm_Query_Monitor_Traces 
	values (HOST_NAME(), @P2, 1) 	
	
-- Begin Query Monitor Trace Segments

{0}

-- End Query Monitor Trace Segments


-- Begin Filter Statements

{1} 

-- End Filter Statements

exec @rc = sp_trace_setstatus @P1, 1 

if isnull(@P2,0) <> 0
	exec @rc = sp_trace_setstatus @P2, 1 