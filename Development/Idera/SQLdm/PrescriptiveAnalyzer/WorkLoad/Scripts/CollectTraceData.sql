----------------------------------------------------------------------------------------
--  Parms:
--    0 - The id of the trace
--    1 - End time of the last collected trace item
--    2 - Allows the trace to be started/stopped for specific sampling
--

set transaction isolation level read uncommitted 
set lock_timeout 20000 
set implicit_transactions off 
if @@trancount > 0 commit transaction 
set language us_english 
set cursor_close_on_commit off 
set query_governor_cost_limit 0 
set numeric_roundabort off
set deadlock_priority low 
set nocount on

DECLARE @TracePath nvarchar(1024)
DECLARE @TraceID int
DECLARE @TraceStatus bit
DECLARE @EventSequence bigint

SET @TraceID = {0}
SET @EventSequence =  {1}

{2}

SELECT @TracePath = convert(nvarchar(1024),value) FROM sys.fn_trace_getinfo(0) WHERE property = 2 and traceid = @TraceID

IF @@ROWCOUNT <> 0
BEGIN
	SELECT EventSequence, EventClass, Duration, Reads, Writes, CPU, DatabaseID, NTUserName, HostName, ApplicationName, LoginName, ObjectName, TextData
		FROM fn_trace_gettable(@TracePath, default) 
		WHERE EventSequence > @EventSequence 
		AND (Reads > 0 OR Writes > 0 OR CPU > 0 OR EventClass = 43) -- OR EventClass = 12 
	SELECT @TraceStatus = convert(bit,value) FROM sys.fn_trace_getinfo(0) WHERE property = 5 and traceid = @TraceID
	IF @@ROWCOUNT <> 0 if @TraceStatus <> 1 SELECT @TraceStatus
END
