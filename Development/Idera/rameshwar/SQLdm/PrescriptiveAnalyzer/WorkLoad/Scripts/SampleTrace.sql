
DECLARE @rc int 

exec @rc = sp_trace_setstatus @TraceID, 1 -- start the trace
if @rc <> 0 begin RAISERROR('sp_trace_setstatus @TraceID, 1', 16, 1) end
waitfor delay '{0}'
exec @rc = sp_trace_setstatus @TraceID, 0 -- stop the trace
if @rc <> 0 begin RAISERROR('sp_trace_setstatus @TraceID, 0', 16, 1) end


