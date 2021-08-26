--------------------------------------------------------------------------------
--  Batch: Stop Session Details Trace 2005
--  Tables: tempdb..SQLdm_Session_Traces 
--  Variables: 
--  [0] - Client session ID - identifier for a given client session
--------------------------------------------------------------------------------
set nocount on 
declare 
	@P1 int, 
	@on bit, 
	@rc int,
	@literal_stoptime datetime,
	@internal_stoptime datetime,
	@client_session_id nvarchar(255),
	@tracefilename as nvarchar(4000),
	@maxfilesize bigint, 
	@filecount integer

select @on = 1,  @client_session_id = '{0}'

-- It's important to begin this transaction because we actually do want to
-- block any of our own batches which are running simultaneously
begin transaction 

-- Check for temp table
if exists (
	select * 
	from tempdb..sysobjects 
	where id = object_id('tempdb..SQLdm_Session_Traces') 
	and type = 'U') 
begin

if (select count(*) 
	from 
		tempdb..SQLdm_Session_Traces
	where 
		host_name = host_name() 
		and desktop_id = @client_session_id) <= 0
	goto exitbatch

-- Get saved data from temp table
delete from 
	tempdb..SQLdm_Session_Traces
where 
	host_name = host_name() 
	and desktop_id = @client_session_id

set @tracefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @tracefilename = left(@tracefilename,len(@tracefilename) - charindex('\',reverse(@tracefilename))) + '\' + replace(host_name(),' ','') + 'dmsestrace' 

-- Check to see if this trace file is already being used
select 
	@P1 = id,
	@literal_stoptime = stop_time,
	@filecount = max_size,
	@maxfilesize = max_files
from 
	master.sys.traces
where 
	path like @tracefilename + '%'

-- Stop and delete the trace
if exists(select id from master.sys.traces where id = @P1)
begin
	exec sp_trace_setstatus @P1, 0 
	exec sp_trace_setstatus @P1, 2 
end

-- If we stopped the last trace, exit the batch
if (select count(*) from tempdb..SQLdm_Session_Traces where datediff(mi,internal_stoptime,getdate()) < 0) <= 0
	goto exitbatch

-- Otherwise go on and recreate the batch with the remaining SPIDs

-- We do not want to extend the length of the trace, just recreate
select
	@literal_stoptime = max(internal_stoptime)
from 
	tempdb..SQLdm_Session_Traces

exec @rc = sp_trace_create @P1 output, 2, @tracefilename, @maxfilesize, @literal_stoptime, @filecount
exec @rc = sp_trace_setevent @P1, 82, 1, @on 
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

exec @rc = sp_trace_setstatus @P1, 1 

exitbatch:
-- Clean up any stopped traces
delete from tempdb..SQLdm_Session_Traces where datediff(mi,internal_stoptime,getdate()) > 0


end

commit transaction 