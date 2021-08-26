----------------------------------------------------------------------------------------
--  Parms:
--    0 - Trace file prefix
--    1 - Maximum minutes that the trace can run
--    2 - Min query duration to trace
--    3 - Max query duration to trace (applied when > 0)
--    4 - Trace status (1 for start)
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
DECLARE @errorText nvarchar(1024)
DECLARE @traceID int 
DECLARE @on bit 
DECLARE @rc int 
DECLARE @maxMinutes int
DECLARE @minDuration bigint
DECLARE @maxDuration bigint
DECLARE @sysSPID int
DECLARE @applicationName nvarchar(1024)
DECLARE @dbid int 
DECLARE @MaxTraceFileSize bigint
DECLARE @stopTime datetime

SET @maxMinutes = {1}
SET @minDuration = {2}
SET @maxDuration = {3}
SET @applicationName = N{5}
SET @dbid = isnull(db_id({6}),0)
SET @sysSPID = 50
SET @on = 1
SET @MaxTraceFileSize = 25
SET @stopTime = dateadd(minute, @maxMinutes, getdate())

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
	if @rc <> 0 begin set @errorText = 'sp_trace_setstatus @traceID, 0' goto ReportError end
	exec @rc = sp_trace_setstatus @traceID, 2 -- close the trace
	if @rc <> 0 begin set @errorText = 'sp_trace_setstatus @traceID, 2' goto ReportError end
END
	
----------------------------------------------------------------------------------------
--  Create our trace.
--
exec @rc = sp_trace_create @traceID output, 0, @TracePath, @MaxTraceFileSize, @stopTime
if @rc <> 0 begin set @errorText = 'sp_trace_create' goto ReportError end

----------------------------------------------------------------------------------------
--  SQL:BatchCompleted
--   
--  Select columns:
--    1  - TextData
--    3  - DatabaseID
--    6  - NTUserName
--    8  - HostName
--    10 - ApplicationName
--    11 - LoginName
--    12 - SPID
--    13 - Duration
--    16 - Reads
--    17 - Writes
--    18 - CPU
--    27 - EventClass
--    51 - EventSequence
--
exec @rc = sp_trace_setevent @traceID, 12, 1, @on -- TextData 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 1, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 3, @on -- DatabaseID
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 3, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 6, @on -- NTUserName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 6, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 8, @on -- HostName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 8, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 10, @on -- ApplicationName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 10, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 11, @on -- LoginName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 11, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 12, @on -- SPID
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 12, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 13, @on -- Duration
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 13, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 16, @on --DiskReadsLogical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 16, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 17, @on --DiskWritesPhysical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 17, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 18, @on --CPU Time
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 18, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 27, @on --EventClass
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 27, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 12, 51, @on --EventSequence
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 12, 51, @on' goto ReportError end

----------------------------------------------------------------------------------------
--  SP:Completed
--   
--  Select columns:
--    1  - TextData
--    3  - DatabaseID
--    6  - NTUserName
--    8  - HostName
--    10 - ApplicationName
--    11 - LoginName
--    12 - SPID
--    13 - Duration
--    16 - Reads
--    17 - Writes
--    18 - CPU
--    27 - EventClass
--    51 - EventSequence
--
/*
exec @rc = sp_trace_setevent @traceID, 43, 1, @on -- TextData 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 1, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 3, @on -- DatabaseID  
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 3, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 6, @on -- NTUserName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 6, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 8, @on -- HostName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 8, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 10, @on -- ApplicationName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 10, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 11, @on -- LoginName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 11, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 12, @on -- SPID
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 12, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 13, @on -- Duration
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 13, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 16, @on --DiskReadsLogical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 16, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 17, @on --DiskWritesPhysical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 17, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 18, @on --CPU Time
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 18, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 27, @on --EventClass
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 27, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 43, 51, @on --EventSequence
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 43, 51, @on' goto ReportError end
*/

----------------------------------------------------------------------------------------
--  RPC:Completed
--   
--  Select columns:
--    1  - TextData
--    3  - DatabaseID
--    6  - NTUserName
--    8  - HostName
--    10 - ApplicationName
--    11 - LoginName
--    12 - SPID
--    13 - Duration
--    16 - Reads
--    17 - Writes
--    18 - CPU
--    27 - EventClass
--    34 - ObjectName
--    51 - EventSequence
--
exec @rc = sp_trace_setevent @traceID, 10, 1, @on -- TextData 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 1, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 3, @on -- DatabaseID  
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 3, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 6, @on -- NTUserName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 6, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 8, @on -- HostName 
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 8, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 10, @on -- ApplicationName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 10, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 11, @on -- LoginName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 11, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 12, @on -- SPID
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 12, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 13, @on -- Duration
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 13, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 16, @on --DiskReadsLogical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 16, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 17, @on --DiskWritesPhysical
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 17, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 18, @on --CPU Time
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 18, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 27, @on --EventClass
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 27, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 34, @on --ObjectName
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 34, @on' goto ReportError end
exec @rc = sp_trace_setevent @traceID, 10, 51, @on --EventSequence
if @rc <> 0 begin set @errorText = 'sp_trace_setevent @traceID, 10, 51, @on' goto ReportError end


----------------------------------------------------------------------------------------
--  Set filters
--
exec @rc = sp_trace_setfilter @traceID, 13, 0, 4, @minDuration -- set min query duration filter >=
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 13, 0, 4, @minDuration' goto ReportError end
if (@maxDuration > 0) exec @rc = sp_trace_setfilter @traceID, 13, 0, 3, @maxDuration -- set max query duration filter <
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 13, 0, 3, @maxDuration' goto ReportError end

----------------------------------------------------------------------------------------
--  Set filter for SPID.  Don't include activity from system SPID's <=50
--
exec @rc = sp_trace_setfilter @traceID, 12, 0, 2, @sysSPID -- set SPID filter > 50
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 12, 0, 2, @sysSPID' goto ReportError end

----------------------------------------------------------------------------------------
--  Set filter for application name if it has been provided.
--
if @applicationName <> N'' exec @rc = sp_trace_setfilter @traceID, 10, 0, 6, @applicationName -- set application name filter like
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 10, 0, 6, @applicationName' goto ReportError end

exec @rc = sp_trace_setfilter @traceID, 10, 0, 7, N'Idera SQL doctor%' -- set application name filter notlike
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 10, 0, 7, Idera SQL doctor' goto ReportError end
exec @rc = sp_trace_setfilter @traceID, 10, 0, 7, N'SQL diagnostic manager%' -- set application name filter notlike
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 10, 0, 7, SQL diagnostic manager' goto ReportError end

----------------------------------------------------------------------------------------
--  Set filter for database if it has been provided.
--
if @dbid <> 0 exec @rc = sp_trace_setfilter @traceID, 3, 0, 0, @dbid -- set database filter =
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 3, 0, 0, @dbid' goto ReportError end

exec @rc = sp_trace_setfilter @traceID, 3, 0, 1, 32767 -- set database filter to exclude the resource DB
if @rc <> 0 begin set @errorText = 'sp_trace_setfilter @traceID, 3, 0, 1, 32767' goto ReportError end

----------------------------------------------------------------------------------------
--  Set the trace status (Start the trace)
--
exec sp_trace_setstatus @traceID, {4} 
if @rc <> 0 begin set @errorText = 'sp_trace_setstatus @TraceID, 1' goto ReportError end

select @traceID
goto ExitNow

ReportError:
SET @errorText = convert(nvarchar(10), @rc) + ' - ' + @errorText
RAISERROR(@errorText, 16, 1)
ExitNow:
