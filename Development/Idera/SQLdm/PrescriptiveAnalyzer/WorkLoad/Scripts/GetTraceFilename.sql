----------------------------------------------------------------------------------------
--  Parms:
--    {0} - Trace file prefix
--

DECLARE @TracePath nvarchar(1024)
DECLARE @traceID int 
DECLARE @rc int 

----------------------------------------------------------------------------------------
--  The IO path to tempdb on high-end systems will be SAN, hence we should put the trace data files there too rather than on a local drive  
--
USE tempdb
select @TracePath = left(physical_name, len(physical_name) - charindex('\',reverse(physical_name))) + '\' from sys.database_files WHERE file_id = 1 
select @TracePath = @TracePath + '{0}'

----------------------------------------------------------------------------------------
--  Make sure that our trace is not already running.
--
SELECT @traceID = traceid FROM sys.fn_trace_getinfo(0) WHERE property = 2 and convert(nvarchar(1024),value) like '%{0}%'  
IF @@ROWCOUNT <> 0
BEGIN
	exec @rc = sp_trace_setstatus @traceID, 0 -- stop the trace
	exec @rc = sp_trace_setstatus @traceID, 2 -- close the trace
	waitfor delay '00:00:01'
END


select @TracePath + '.trc'
