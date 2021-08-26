--------------------------------------------------------------------------------
--  Batch: Session Details Trace
--  Tables: tempdb..SQLdm_Session_Traces, #TraceEvents 
--  Variables:  [0] - Session ID to filter upon
--  [1] - Client session ID - identifier for a given client session
--	[2] - Default trace length in minutes 
--  [3] - Threshold to restart trace - restart if less than [3] minutes remain
--------------------------------------------------------------------------------

set nocount on 
declare 
	@P1 int, 
	@on bit, 
	@counter int, 
	@tabspid int, 
	@rowcount int, 
	@spid int, 
	@rc varchar, 
	@eventclass int, 
	@sqltext varchar(512), 
	@duration bigint, 
	@reads bigint, 
	@writes bigint, 
	@cpu bigint, 
	@date varchar(15), 
	@time varchar(15), 
	@mm varchar(2), 
	@dd varchar(2), 
	@hh varchar(2), 
	@mi varchar(2), 
	@ss varchar(2), 
	@datetime datetime,
	@hex varbinary(4000), 
	@stoptime datetime,
	@alreadyread int,
	@client_session_id nvarchar(255)

select @on = 1, @spid = {0}, @alreadyread = 0, @client_session_id = '{1}'

begin transaction 

-- Set up temp table
if not exists 
	(
	select * 
	from tempdb..sysobjects 
	where id = object_id('tempdb..SQLdm_Session_Traces') 
	and type = 'U') 
	create table tempdb..SQLdm_Session_Traces (host_name varchar(255), desktop_id varchar(255), stoptime datetime, queue_number int, spid int) 

-- Get saved data from temp table
select 
	@P1 = queue_number, 
	@stoptime = stoptime,
	@tabspid = spid 
from 
	tempdb..SQLdm_Session_Traces 
where 
	host_name = host_name() 
	and desktop_id = @client_session_id

select @rowcount = @@rowcount 

-- If we have data from the table that seems up to date, try to go ahead and read it
if @rowcount > 0 and @tabspid = @spid and @stoptime > getdate()
goto correct_trace 

-- You can get here two ways.
-- (1) The data from the temp table is either not present or is not up to date
-- (2) The trace has already been read and is about to expire
-- Either way, stop and delete the trace if we have a trace ID.  If we do not have a trace ID
-- just move on.
stop_and_restart:

if	(@rowcount > 0 and @tabspid <> @spid) 
	or (@rowcount > 0 and @stoptime < getdate()) 
	or (@alreadyread > 0 and datediff(mi,getdate(),@stoptime) < 5)
begin 
	exec sp_trace_setstatus @P1, 0 
	exec sp_trace_setstatus @P1, 2 
end 

-- Whether we have no trace to begin with or just deleted an old one, 
-- this is where we recreate the trace and get a new trace ID
recreate_trace: 
delete from tempdb..SQLdm_Session_Traces where host_name = host_name() and desktop_id = @client_session_id

-- Set the stop time based on the variable passed in from the collection service
-- This prevents abandoned traces from staying on the server indefinitely
set @stoptime = dateadd(mi,{2},getdate())
exec @rc = sp_trace_create @P1 output, 1, null, null, @stoptime 

-- Update the temp table
if @P1 <> 0 
	insert into tempdb..SQLdm_Session_Traces values (host_name(), @client_session_id, @stoptime, @P1, @spid) 

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

exec @rc = sp_trace_setfilter @P1, 12, 0, 0, @spid 

-- Make sure our trace is running, whether it was just created or we looked it up
correct_trace: 
exec @rc = sp_trace_setstatus @P1, 1 

-- If there was a problem starting the trace try to recreate the trace
if @rc = 9 goto recreate_trace 

commit transaction 

-- If we were recreating the batch because it was about to time out, exit
-- now to prevent a re-read
if @alreadyread > 0 goto exitbatch

-- Set up our temp table for the trace data
if (select isnull(object_id('tempdb..#TraceEvents'), 0)) = 0 
	create table #TraceEvents (colid int, datlength int, datatext image) 

select @counter = 0 

Loop: 

truncate table #TraceEvents 
insert into #TraceEvents 
	exec sp_trace_getdata @P1, 1 

if @@rowcount <> 0 and (select count(*) from #TraceEvents where datlength = 0) = 0  
begin 
	select @duration = 0, @reads = 0, @writes = 0, @cpu = 0 
	select @eventclass = ascii(convert(nvarchar(1),convert(varbinary(1), datatext))) from #TraceEvents where colid = 65526 


	select 
		@sqltext = rtrim(convert(nvarchar(512),convert(varbinary(1024), datatext))) 
	from 
		#TraceEvents 
	where 
		colid = 1 

	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 13 

	if @@rowcount > 0 
		select 
			@duration = convert(int,substring(@hex, 1, 1)) 
						+ (convert(int,substring(@hex, 2, 1)) * 255) 
						+ (convert(int,substring(@hex, 3, 1)) * 65025) 
						+ (convert(int,substring(@hex, 4, 1)) * 16581375) 

	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 16 

	if @@rowcount > 0 
		select 
			@reads = convert(int,substring(@hex, 1, 1)) 
			+ (convert(int,substring(@hex, 2, 1)) * 255) 
			+ (convert(int,substring(@hex, 3, 1)) * 65025) 
			+ (convert(int,substring(@hex, 4, 1)) * 16581375) 

	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 17 

		
	if @@rowcount > 0 
		select 
			@writes = convert(int,substring(@hex, 1, 1)) 
					+ (convert(int,substring(@hex, 2, 1)) * 255) 
					+ (convert(int,substring(@hex, 3, 1)) * 65025) 
					+ (convert(int,substring(@hex, 4, 1)) * 16581375) 

	-- Set cpu from hex data
	select 
		@hex = convert(varbinary(10), datatext) 
	from 
		#TraceEvents 
	where 
		colid = 18 

	if @@rowcount > 0 
		select 
			@cpu = convert(int,substring(@hex, 1, 1)) 
					+ (convert(int,substring(@hex, 2, 1)) * 255) 
					+ (convert(int,substring(@hex, 3, 1)) * 65025) 
					+ (convert(int,substring(@hex, 4, 1)) * 16581375) 


	--Set date/time from hex data
	if @eventclass <> 40 
	begin
		select 
			@hex = convert(varbinary(30), datatext) 
		from 
			#TraceEvents 
		where 
			colid = 15 
	end
	Else 
	begin
		select 
			@hex = convert(varbinary(30), datatext) 
		from 
			#TraceEvents 
		where colid = 14 
	end
		

	if @@rowcount > 0 
	begin 
		select @mm = RTRIM(convert(varchar(2),convert(int,substring(@hex, 3, 1)))) 
		select @dd = RTRIM(convert(varchar(2),convert(int,substring(@hex, 7, 1)))) 
		select @hh = RTRIM(convert(varchar(2),convert(int,substring(@hex, 9, 1)))) 
		select @mi = RTRIM(convert(varchar(2),convert(int,substring(@hex, 11, 1)))) 
		select @ss = RTRIM(convert(varchar(2),convert(int,substring(@hex, 13, 1)))) 
		select @date = datepart(year,getdate())
		select @date = @date + '-' + REPLICATE('0', 2 - LEN(@mm)) + @mm + '-' + REPLICATE('0', 2 - LEN(@dd)) + @dd + ' ' 
		select @time = REPLICATE('0', 2 - LEN(@hh)) + @hh + ':' + REPLICATE('0', 2 - LEN(@mi)) + @mi + ':' + REPLICATE('0', 2 - LEN(@ss)) + @ss 
		select @datetime = convert(datetime,@date + @time)
		select @datetime = dateadd(mi,datediff(mi,getdate(),getutcdate()),@datetime)
	end


	if @eventclass is not null 
		if @eventclass <> 40 
		begin 
			select @eventclass, @duration, @datetime, @sqltext, @reads, @writes, @cpu 
			select @counter = @counter + 1 
		end 
		else
		begin
			select 40, cast(0 as bigint), @datetime, @sqltext, cast(0 as bigint), cast(0 as bigint), cast(0 as bigint)
		end
	if @counter < 1000 goto Loop 
End 

if @counter > 999 
	select 40, cast(0 as bigint), @datetime, 'Trace too long! Refresh for more trace', '', 0, 0, 0 

drop table #TraceEvents 

set @alreadyread = 1
print @alreadyread

if (datediff(mi,getdate(),@stoptime) < 1)
begin
	begin transaction
	goto stop_and_restart
end

exitbatch:

-- Clean up any stopped traces
delete from tempdb..SQLdm_Session_Traces where datediff(mi,stoptime,getdate()) > 0
