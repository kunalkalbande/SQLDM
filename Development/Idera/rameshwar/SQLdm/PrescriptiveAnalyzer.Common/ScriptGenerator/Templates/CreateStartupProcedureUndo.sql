USE master
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if (object_id('sp_CreateSQLdoctorDeadlockTrace') is not null)
begin
	drop procedure [dbo].[sp_CreateSQLdoctorDeadlockTrace]
end
GO

declare @TraceID INT
select @TraceID = TraceID
	from :: fn_trace_getinfo(0)
	where cast([value] as varchar(256)) like '%SQLdrDeadlocks%'

if (@TraceID IS NOT NULL)
BEGIN
	print 'Terminating Trace ID ' + cast (@TraceID as varchar(5))
	EXEC sp_trace_setstatus @TraceID, 0
	EXEC sp_trace_setstatus @TraceID, 2
END