--------------------------------------------------------------------------------
--  Batch: Query Monitor Trace
--  Similar to QueryMonitor2005.sql just no declarations done in order to use it for Restart trace
--  Tables: tempdb..SQLdm_Query_Monitor 
--  XSP:  none
--	Variables: 0 - Trace File Size
--  1 - Trace File Rollovers
--  2 - Query Monitor Trace Batch Segments
--  3 - Query Monitor Filter Statements
--	4 - Records Per Refresh
--  5 - Query Stop Time
--------------------------------------------------------------------------------\
set @retrycounter = 0

select @on = 1 

select @currenttime = getdate() 

-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
set @tracefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @tracefilename = left(@tracefilename,len(@tracefilename) - charindex('\',reverse(@tracefilename))) + '\' + replace(host_name(),' ','') + 'dm5wptrace'



recreate_trace1: 

-- Look up the ID of the trace currently using that file
select 
	@P1 = traceid 
from 
	::fn_trace_getinfo(0) 
where 
	property = 2 
	and lower(convert(nvarchar(4000),value)) like lower(@tracefilename) + '%' 

select @rowcount = @@rowcount 

if @rowcount > 0 
	goto correct_trace1

set @maxfilesize = {0}
set @filecount = {1}
set @stoptime = {5}
set @stoptime = dateadd(hh,-datediff(hh,getdate(),getutcdate()),@stoptime)

-- Create the trace with given filesize and file count
exec @rc = sp_trace_create @P1 output, 2, @tracefilename, @maxfilesize, @stoptime, @filecount

-- If there is an error creating the trace, try again or exit the batch
if @rc = '*' 
begin
	set @retrycounter = @retrycounter + 1
	if @retrycounter < 10
	begin
		waitfor delay '00:00:01'
		goto recreate_trace1 
	end
	else
	begin
		goto end_of_batch1
	end
end

exec @rc = sp_trace_setevent @P1, 82, 1, @on 

-- Begin Query Monitor Trace Segments

{2}

-- End Query Monitor Trace Segments

-- Begin Filter Statements

{3} 

-- End Filter Statements
exec sp_trace_setfilter @P1,1,1,6,N'SQLdm1 - %' 
exec sp_trace_setfilter @P1,1,0,1,NULL 

correct_trace1: 

exec @rc = sp_trace_setstatus @P1, 1 

if @rc = 9 
begin
	set @retrycounter = @retrycounter + 1
	if @retrycounter < 10
	begin
		waitfor delay '00:00:01'
		goto recreate_trace 
	end
	else
	begin
		goto end_of_batch
	end
end


{4}

end_of_batch1: