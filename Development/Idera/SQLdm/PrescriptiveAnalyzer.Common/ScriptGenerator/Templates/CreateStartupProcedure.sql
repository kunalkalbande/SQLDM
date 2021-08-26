USE master
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- first set the server to show advanced options
EXEC sp_configure 'show advanced option', '1';
RECONFIGURE
-- then set the scan for startup procs to 1
EXEC sp_configure 'scan for startup procs', '1';
RECONFIGURE
GO 

if (object_id('sp_CreateSQLdoctorDeadlockTrace') is not null)
begin
	drop procedure [dbo].[sp_CreateSQLdoctorDeadlockTrace]
end
GO

create procedure [dbo].[sp_CreateSQLdoctorDeadlockTrace]
as
begin
	declare @traceid int
	declare @traceopts int
	declare @tracemax bigint
	declare @tracefile nvarchar(512)
	declare @tracestop datetime
	declare @on bit
	declare @rc int

	set @on = 1
	set @traceopts = 0
	set @tracemax = 5
	
	select @traceid = traceid 
		from sys.fn_trace_getinfo(0) 
		where property = 2 and convert(nvarchar(1024),value) like '%\SQLdrDeadlocks_%.trc'  
	
	IF (@traceid is not null)	
	begin
		exec sp_trace_setstatus @traceid, 0 -- stop the trace
		exec sp_trace_setstatus @traceid, 2 -- close the trace
		waitfor delay '00:00:01'
		set @traceid = null
	END
	
	-- place tracefile in qtemp log directory
	select @tracefile = left(physical_name, len(physical_name) - charindex('\',reverse(physical_name))) 
		from sys.master_files where database_id = 2 and file_id = 1 
	set @tracefile = @tracefile + '\SQLdrDeadlocks_0' -- trace file name

	BEGIN TRY
		exec @rc = sp_trace_create @traceid=@traceid output, 
							 @options=@traceopts, 
							 @tracefile=@tracefile, 
							 @maxfilesize=@tracemax,
							 @stoptime=@tracestop 
	END TRY
	BEGIN CATCH
		declare @cmd sysname
		declare @curconfig int 

		print 'Please ignore the preceeding error.  We will delete the trace file and try to configure the trace again.'

		-- assume that an old file exists - delete it and try again
		select @curconfig = cast(value_in_use as int)
			from sys.configurations 
			where configuration_id = 16390 -- command shell 
		if (@curconfig = 1)
		BEGIN
			SET @cmd = 'del "' + @tracefile + '.trc"'
			EXEC master.dbo.xp_cmdshell @cmd
		END
		ELSE	
		BEGIN		
			-----------------------------------------------------------------------------
			-- without cmd shell being enabled we will try to use ole automation.
			--
			DECLARE @hr int
			DECLARE @FSO_Token int
			DECLARE @showAdvOpts bigint;
			DECLARE @oleAuto bigint;
			select @showAdvOpts = cast(value_in_use as bigint) from sys.configurations where configuration_id = 518; -- show advanced options
			select @oleAuto = cast(value_in_use as bigint) from sys.configurations where configuration_id = 16388; -- Ole Automation Procedures
			if 0 = @showAdvOpts
			begin
				exec sp_configure 'show advanced options', 1;
				reconfigure with override;
			end;
			if 0 = @oleAuto
			begin
				exec sp_configure 'Ole Automation Procedures', 1;
				reconfigure with override;
			end;
			-----------------------------------------------------------------------------
			-- Ensure that the stored procedure exists or we will not be able to call it
			--
			if (object_id('sp_OACreate') is null)
			begin
			   RAISERROR('The sp_OACreate stored procedure was not found on the server', 16, 1)
			   return
			end

			-----------------------------------------------------------------------------
			-- Ensure that the stored procedure exists or we will not be able to call it
			--
			if (object_id('sp_OAMethod') is null)
			begin
			   RAISERROR('The sp_OAMethod stored procedure was not found on the server', 16, 1)
			   return
			end

			-----------------------------------------------------------------------------
			-- Ensure that we can get access the object for locating the service.
			--
			exec @hr = sp_OACreate 'Scripting.FileSystemObject', @FSO_Token output
			if @hr <> 0
			BEGIN
			   RAISERROR('Failed to use OLE for FileSystem access when cleaning up trace file', 16, 1)
			   return
			END
			SET @cmd = @tracefile + '.trc'
			exec @hr = sp_OAMethod @FSO_Token, 'DeleteFile', NULL, @cmd
			exec @hr = sp_OADestroy @FSO_Token
			
			if 0 = @showAdvOpts
			begin
				exec sp_configure 'show advanced options', 0;
				reconfigure with override;
			end;
			if 0 = @oleAuto
			begin
				exec sp_configure 'Ole Automation Procedures', 0;
				reconfigure with override;
			end;
		END

		exec sp_trace_create @traceid=@traceid output, 
						 @options=@traceopts, 
						 @tracefile=@tracefile, 
						 @maxfilesize=@tracemax,
						 @stoptime=@tracestop 
	END CATCH

	if (@traceid >0)
	begin
		exec sp_trace_setevent @traceid, 148,  1, @on	-- TextData
		exec sp_trace_setevent @traceid, 148, 12, @on	-- SPID
		exec sp_trace_setevent @traceid, 148, 14, @on	-- StartTime
		exec sp_trace_setevent @traceid, 148, 27, @on	-- EventClass
		exec sp_trace_setevent @traceid, 148, 51, @on	-- EventSequence
		exec sp_trace_setstatus @traceid, 1 -- start the trace
	end
end
GO
	
-- set it to run at sql server start-up
exec sp_procoption N'sp_CreateSQLdoctorDeadlockTrace', 'startup', 'on'

-- go ahead and start the trace now
exec sp_CreateSQLdoctorDeadlockTrace