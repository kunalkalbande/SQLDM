--------------------------------------------------------------------------------
--  Batch: Session Details Trace 2005
--  SQLdm 10.3 (Varun Chopra) Linux Support
--  Tables: tempdb..SQLdm_Session_Traces, #DMTraceEvents 
--  Variables:  [0] - Session ID to filter upon
--  [1] - Client session ID - identifier for a given client session
--	[2] - Default trace length in minutes 
--  [3] - Threshold to restart trace - restart if less than [3] minutes remain
--  [4] - Trace File Size
--  [5] - Trace File Rollovers
--------------------------------------------------------------------------------

set nocount on 
declare 
	@P1 int, 
	@on bit, 
	@counter int, 
	@tabspid int, 
	@spid int, 
	@rc varchar, 
	@literal_stoptime datetime,
	@internal_stoptime datetime,
	@alreadyread int,
	@client_session_id nvarchar(255),
	@tracefilename as nvarchar(4000),
	@currenttime datetime

select @currenttime = getdate() 

select @on = 1, @spid = {0}, @client_session_id = '{1}'

-- It's important to begin this transaction because we actually do want to
-- block any of our own batches which are running simultaneously
begin transaction 

-- Set up temp table
if not exists 
	(
	select * 
	from tempdb..sysobjects 
	where id = object_id('tempdb..SQLdm_Session_Traces') 
	and type = 'U') 
	create table tempdb..SQLdm_Session_Traces (host_name varchar(255), desktop_id varchar(255), internal_stoptime datetime, spid int) 


-- Get saved data from temp table
select 
	@tabspid = spid
from 
	tempdb..SQLdm_Session_Traces
where 
	host_name = host_name() 
	and desktop_id = @client_session_id

-- SQLdm 10.3 (Varun Chopra) Linux Support using Path with forward slashes
set @tracefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @tracefilename = left(@tracefilename,len(@tracefilename) - charindex('/',reverse(@tracefilename))) + '/' + replace(host_name(),' ','') + 'dmsestrace' 

-- Check to see if this trace file is already being used
select 
	@P1 = id,
	@literal_stoptime = stop_time
from 
	master.sys.traces
where 
	path like @tracefilename + '%'


-- If it has already timed out, go restart it
if (@literal_stoptime is null)
	goto stop_and_restart

-- If it's about to timeout, restart it
if (datediff(mi,getdate(),@literal_stoptime) < {3})
	goto stop_and_restart

-- If we have data from the table that seems up to date, try to go ahead and read it
if @tabspid = @spid
	goto correct_trace 

-- You can get here two ways.
-- (1) The data from the temp table is either not present or is not up to date
-- (2) The trace is about to expire
-- Either way, stop and delete the trace if we have a trace ID.  If we do not have a trace ID
-- just move on.
stop_and_restart:

if (@literal_stoptime is not null) and (@P1 is not null)
begin
	-- If we're pretty sure it's already running we just need to change it
	-- But check to make sure it hasn't disappeared in the meantime
	if exists(select id from master.sys.traces where id = @P1)
	begin
		exec sp_trace_setstatus @P1, 0 
		exec sp_trace_setstatus @P1, 2 
	end
end

-- Whether we have no trace to begin with or just deleted an old one, 
-- this is where we recreate the trace and get a new trace ID
recreate_trace: 

delete from tempdb..SQLdm_Session_Traces where host_name = host_name() and desktop_id = @client_session_id

declare 
	@maxfilesize bigint, 
	@filecount integer 

set @maxfilesize = {4}
set @filecount = {5}

-- Set the stop time based on the variable passed in from the collection service
-- This prevents abandoned traces from staying on the server indefinitely
set @literal_stoptime = dateadd(mi,{2},getdate())
exec @rc = sp_trace_create @P1 output, 2, @tracefilename, @maxfilesize, @literal_stoptime, @filecount
exec @rc = sp_trace_setevent @P1, 82, 1, @on 

-- Update the temp table
if @P1 <> 0 
begin
	insert into tempdb..SQLdm_Session_Traces values (host_name(), @client_session_id, @literal_stoptime, @spid) 
end
	

exec @rc = sp_trace_setevent @P1, 40, 1, @on 
exec @rc = sp_trace_setevent @P1, 40, 12, @on 
exec @rc = sp_trace_setevent @P1, 40, 14, @on 
exec @rc = sp_trace_setevent @P1, 42, 1, @on 
exec @rc = sp_trace_setevent @P1, 42, 12, @on 
exec @rc = sp_trace_setevent @P1, 42, 14, @on 
exec @rc = sp_trace_setevent @P1, 41, 1, @on 
exec @rc = sp_trace_setevent @P1, 41, 12, @on 
exec @rc = sp_trace_setevent @P1, 41, 13, @on 
exec @rc = sp_trace_setevent @P1, 41, 15, @on 
exec @rc = sp_trace_setevent @P1, 41, 16, @on 
exec @rc = sp_trace_setevent @P1, 41, 17, @on 
exec @rc = sp_trace_setevent @P1, 41, 18, @on 
exec @rc = sp_trace_setevent @P1, 43, 1, @on 
exec @rc = sp_trace_setevent @P1, 43, 12, @on 
exec @rc = sp_trace_setevent @P1, 43, 13, @on 
exec @rc = sp_trace_setevent @P1, 43, 15, @on 
exec @rc = sp_trace_setevent @P1, 43, 16, @on 
exec @rc = sp_trace_setevent @P1, 43, 17, @on 
exec @rc = sp_trace_setevent @P1, 43, 18, @on 
exec @rc = sp_trace_setevent @P1, 33, 1, @on 
exec @rc = sp_trace_setevent @P1, 33, 12, @on 
exec @rc = sp_trace_setevent @P1, 33, 13, @on 
exec @rc = sp_trace_setevent @P1, 33, 15, @on 
exec @rc = sp_trace_setevent @P1, 33, 16, @on 
exec @rc = sp_trace_setevent @P1, 33, 17, @on 
exec @rc = sp_trace_setevent @P1, 33, 18, @on 

exec sp_trace_setfilter @P1,1,1,6,N'SQLdmSess - %' 

update 
	tempdb..SQLdm_Session_Traces 
set spid = @spid 
where 
	host_name = host_name() 
	and desktop_id = @client_session_id
 
declare @cursorspid int
declare spidcursor insensitive cursor for
	select distinct spid 
	from 
		tempdb..SQLdm_Session_Traces
	where
		internal_stoptime > getdate()
open spidcursor
fetch next from spidcursor into @cursorspid
while (@@fetch_status = 0)
begin
	exec @rc = sp_trace_setfilter @P1, 12, 1, 0, @cursorspid
	fetch next from spidcursor into @cursorspid
end
close spidcursor
deallocate spidcursor

-- Make sure our trace is running, whether it was just created or we looked it up
correct_trace: 
exec @rc = sp_trace_setstatus @P1, 1 

-- If there was a problem starting the trace try to recreate the trace
if @rc = 9 goto recreate_trace 

commit transaction 

declare @flagtime nvarchar(256) 

-- Trigger user-generated flag event
set @flagtime = 'SQLdmSess - ' + host_name() + @client_session_id + ' - ' + convert(nvarchar(50),@currenttime,121) 
exec @rc = sp_trace_generateevent 82, @flagtime, NULL 


if (select isnull(object_id('tempdb..#DMTraceEvents'), 0)) <> 0 
	drop table #DMTraceEvents 

select @tracefilename = path from sys.traces where id = @P1

-- Populate temporary table
select 
	identity(int, 1, 1) as RowNumber,
	EventClass, 
	Duration = cast(isnull(Duration,0) / 1000 as bigint),	
	EndTime,	
	Reads,
	Writes,
	CPU,
	Spid,
	substring(TextData, 1, 3060) as TextData 
into 
	#DMTraceEvents 
from 
	::fn_trace_gettable(@tracefilename, default) 
where 
	EventClass <= 82  

declare 
	@LastReadRowNumber int, 
	@CurrentReadRowNumber int 

-- Find the flag we just raised as our endpoint
select 
	@CurrentReadRowNumber = RowNumber 
from 
	#DMTraceEvents 
where 
	EventClass = 82 
	and TextData = @flagtime 

-- Find the flag we may have raised on a previous refresh as our startpoint
select 
	@LastReadRowNumber = max(RowNumber) 
from 
	#DMTraceEvents 
where
 	lower(TextData) like lower('SQLdmSess - ' + host_name() + @client_session_id + ' - %')
	and EventClass = 82
	and RowNumber < @CurrentReadRowNumber 

select 
	EventClass,
	Duration,
	dateadd(mi,datediff(mi,getdate(),getutcdate()),EndTime),
	TextData,
	Reads,
	Writes,
	cast(CPU as bigint)
from 
	#DMTraceEvents 
where 
	Spid = @spid
	and RowNumber > isnull(@LastReadRowNumber,0) 
	and RowNumber < isnull(@CurrentReadRowNumber,10001) 
	and TextData is not null

if (select isnull(object_id('tempdb..#DMTraceEvents'), 0)) <> 0 
	drop table #DMTraceEvents 

exitbatch:
-- Clean up any stopped traces
delete from tempdb..SQLdm_Session_Traces where datediff(mi,internal_stoptime,getdate()) > 0


