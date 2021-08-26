--------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra) Linux Support
--  Batch: Query Monitor Trace
--  XSP:  none
--	Variables: 0 - Trace File Size
--  1 - Trace File Rollovers
--  2 - Query Monitor Trace Batch Segments
--  3 - Query Monitor Filter Statements
--	4 - Records Per Refresh
--  5 - Query Stop Time
--------------------------------------------------------------------------------\
set nocount on 

declare 
	@P1 int, 
	@on bit, 
	@counter int, 
	@rowcount int, 
	@rc varchar, 
	@eventclass int, 
	@previouseventclass int, 
	@sqltext varchar(3061), 
	@duration int, 
	@reads int, 
	@writes int, 
	@cpu int, 
	@date varchar(4), 
	@time varchar(8), 
	@mm varchar(2), 
	@dd varchar(2), 
	@hh varchar(2), 
	@mi varchar(2), 
	@ss varchar(2), 
	@hex varbinary(30), 
	@ntusername varchar(128), 
	@ntdomainname varchar(128), 
	@hostname varchar(128), 
	@appname varchar(128), 
	@sqlname varchar(255), 
	@DBID int, 
	@currenttime datetime, 
	@tracefilename as nvarchar(4000),
	@retrycounter int

set @retrycounter = 0

select @on = 1 

select @currenttime = getdate() 

-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
set @tracefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 

-- SQLdm 10.3 (Varun Chopra) Linux Support using Path with forward slashes
set @tracefilename = left(@tracefilename,len(@tracefilename) - charindex('/',reverse(@tracefilename))) + '/' + replace(host_name(),' ','') + 'dm7ActivityMonitor'

recreate_trace: 

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
	goto correct_trace

declare 
	@maxfilesize bigint, 
	@filecount integer,
	@stoptime datetime

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
		goto recreate_trace 
	end
	else
	begin
		goto end_of_batch
	end
end

--user configured events are in
exec @rc = sp_trace_setevent @P1, 82, 1, @on 

-- Begin Activity Profiler Monitor Trace Segments

{2}

-- End Activity Profiler Trace Segments

--declare 
--	@cpu_filter int, 
--	@reads_filter bigint, 
--	@writes_filter bigint,
--	@duration_filter bigint

-- Begin Filter Statements

{3} 

-- End Filter Statements
--exec sp_trace_setfilter @P1,1,1,6,N'SQLdm3 - %' 

correct_trace: 

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

end_of_batch: