--------------------------------------------------------------------------------
--  Batch: Query Monitor Batch 2012 ( Extended Events)
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Variables: --  [0] - show_plan predicates
--  [1] - File Name
--  [2] - swicth for altering where clause
-- [3] - max_dispatch_latency
-- [4] - rolloverFiles
-- [5] - max_memory
-- [6] - file size
-- [7] - event_retention_mode
--  [8]- max_event_size
-- [9]- memory_partition_mode
--  [10]- startup_state
-- [11]- track_causality
--  [12] - Extended Events
-- [13] - ReadQuery
-- [14] - CollectQueryPlan
		
--------------------------------------------------------------------------------

declare @session nvarchar(255),@xefilename as nvarchar(4000), @delayTime as nvarchar(20) 


select @session = replace(host_name(),' ','') + 'SQLdmQMEXevents'


declare @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
declare @path nvarchar(260), @mdpath nvarchar(260), @file nvarchar(260), @currentfile nvarchar(260)
declare @sql nvarchar(max),@sqlOpt nvarchar(max),@sql1 nvarchar(max),@sql2 nvarchar(max);
declare @StartTime datetime

set @StartTime = getUTCDate();

declare @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
@max_memory int, @max_event_size int, @memory_partition_mode nvarchar(120), 
@track_causality bit, @startup_state bit,@fileSize int, @rolloverFiles int

select @currenttime = getdate()
set @file = '{1}'

-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
--if the user has entered a whole path then don't just put the file in the log directory
if charindex('\',@file,0) > 0
	set @xefilename = @file
else
begin
	set @xefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
	Select @xefilename = left(@xefilename,len(@xefilename) - charindex('\',reverse(@xefilename))) + '\' + replace(Host_name(),' ','') + ISNULL(Convert(nvarchar(100),SERVERPROPERTY ('InstanceName')),'Default')+ @file
end


--session parameters (not file)
SELECT @name=name, 
@event_retention_mode=event_retention_mode_desc, 
@max_dispatch_latency = max_dispatch_latency, 
@max_memory=max_memory, 
@max_event_size=max_event_size, 
@memory_partition_mode=memory_partition_mode_desc, 
@track_causality=track_causality, 
@startup_state=startup_state 
FROM sys.server_event_sessions
where name = @session

if @name is not null
begin
	-- file size and rollover files
	select @fileSize = convert(int, max_file_size), 
	@rolloverFiles = convert(int,max_rollover_files), 
	@currentfile = convert(nvarchar(260),filename)
	from (
	select  sess.name as sessionName,fields.name as fieldName, fields.value
	from sys.server_event_sessions sess 
	inner join sys.server_event_session_fields fields on sess.event_session_id = fields.event_session_id
	where sess.name = @session) as d
	pivot(max(value) for fieldName in([filename], [max_file_size], [max_rollover_files])) as maxval

	--if the session parameters have changed then drop it so it will be recreated
	if (@max_dispatch_latency <> {3}*1000 
		or @event_retention_mode <> '{7}'
		or @max_memory <> {5} * 1024
		or @max_event_size <> {8} * 1024
		or @memory_partition_mode <> '{9}'
		or @track_causality <> case '{11}' when 'ON' then 1 else 0 end
		or @startup_state <> case '{10}' when 'ON' then 1 else 0 end
		or isnull(@fileSize, 20) <> {6} 
		or isnull(@rolloverFiles, 5) <> {4} )
		exec ('drop event session [' + @session +'] ON SERVER')
end

-- Trigger user-generated flag event to mark the point where we read back the data from the file
set @flagtime = 'SQLdmQM - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
-- SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- Rplaced Bookmark approach with Offset approach
select @flagtime, ms_ticks
from sys.dm_os_sys_info;

-- SQL DM 10.5 - DROP session if it contains ERRORLOG in path

IF EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE name = @session)
BEGIN
	DECLARE @filepathcheck NVARCHAR(4000)

	SELECT @filepathcheck = CAST(current_session_fields.value AS NVARCHAR(4000))
	FROM sys.server_event_sessions current_sessions 
	INNER JOIN sys.server_event_session_fields current_session_fields
	ON current_sessions.event_session_id = current_session_fields.event_session_id
	WHERE current_session_fields.name = 'filename' and current_sessions.name = @session

	IF  CHARINDEX('\ERRORLOG', @filepathcheck ) > 0 
	BEGIN
		EXEC('DROP EVENT SESSION [' + @session +'] ON SERVER')
	END
END

-- SQL DM 10.5 - DROP session if it contains ERRORLOG in path

IF Not EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE name = @session)
begin
	select @sql = 'CREATE EVENT SESSION [' + @session +'] ON SERVER 
					ADD EVENT sqlserver.user_event, '
	if( 1= {14})
	begin
		select @sqlOpt='ADD EVENT sqlserver.query_post_execution_showplan(
					ACTION(sqlserver.plan_handle){0}), ';
	end
	else
	begin
		select @sqlOpt='' ;
	end

	select @sql1={12}
	
	select @sql2=' ADD TARGET package0.event_file(SET filename= ''' + @xefilename + ''',max_file_size=({6}),max_rollover_files=({4}))
	 WITH (MAX_MEMORY={5} MB,EVENT_RETENTION_MODE={7},MAX_DISPATCH_LATENCY={3} SECONDS,MAX_EVENT_SIZE={8} MB,
		MEMORY_PARTITION_MODE={9},TRACK_CAUSALITY={10},STARTUP_STATE={11})';
	
	exec(@sql+@sqlOpt+@sql1+@sql2);

	exec('Alter event session [' + @session + '] on server state = start')
end
else
begin --we only get here if the extended event session exists
	if 1={2} --we do this if we need to alter an existing extended event session. This also flushes mem to disk
	begin
		select @sql = '	if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''query_post_execution_showplan'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.query_post_execution_showplan
						end

						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''module_end'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.module_end
						end

						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''sp_statement_completed'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.sp_statement_completed
						end
						
						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''sql_batch_completed'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.sql_batch_completed
						end
						
						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''sql_statement_completed'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.sql_statement_completed
						end

						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''user_event'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.user_event
						end		

						Alter EVENT SESSION [' + @session +'] ON SERVER 
						ADD EVENT sqlserver.user_event, ';

	if( 1= {14})
	begin
		select @sqlOpt=' ADD EVENT sqlserver.query_post_execution_showplan(
						ACTION(sqlserver.plan_handle){0}), ';
	end
	else
	begin
		select @sqlOpt='' ;
	end

		select @sql1= {12}
	
	exec(@sql+@sqlOpt+@sql1);

	end
	
	--If it does not exist with a started state
	if not exists (select * from sys.dm_xe_sessions where name = @session)
		exec('Alter event session [' + @session + '] on server state = start')
end

--Read Query will be here
{13}
