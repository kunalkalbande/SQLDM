--------------------------------------------------------------------------------
--  SQLdm 10.3 (Tushar)--Modified batch to support extended events api
--  SQLdm 10.3 (Varun Chopra) Linux Support
--  Batch: Active Waits Batch
--  Variables: 
--  [0] - File Name
--  [1] - swicth for altering where clause
--  [2] - Use tempdb for lower compatibility level databases
--  [3] - Read query for extended events
--------------------------------------------------------------------------------

declare @session nvarchar(255),@xefilename as nvarchar(4000), @delayTime as nvarchar(20) 

select @session = replace(host_name(),' ','') + 'SQLdmResourceWaits'

declare @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
declare @path nvarchar(260), @mdpath nvarchar(260), @file nvarchar(260), @currentfile nvarchar(260)
declare @sql nvarchar(max);
declare @sql1 nvarchar(max);
declare @StartTime datetime

set @StartTime = getUTCDate();

declare @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
@max_memory int, @max_event_size int, @memory_partition_mode nvarchar(120), 
@track_causality bit, @startup_state bit,@fileSize int, @rolloverFiles int

select @currenttime = getdate()
set @file = '{1}'

-- SQLdm 10.3 (Varun Chopra) Linux Support using Path with forward slashes
-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
--if the user has entered a whole path then don't just put the file in the log directory
if charindex('/',@file,0) > 0
	set @xefilename = @file
else
begin
	set @xefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
	set @xefilename = left(@xefilename,len(@xefilename) - charindex('/',reverse(@xefilename))) + '/' + replace(host_name(),' ','') + @file
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
	if (@max_dispatch_latency <> {8}*1000 
		or @event_retention_mode <> '{12}'
		or @max_memory <> {10} * 1024
		or @max_event_size <> {13} * 1024
		or @memory_partition_mode <> '{14}'
		or @track_causality <> case '{15}' when 'ON' then 1 else 0 end
		or @startup_state <> case '{16}' when 'ON' then 1 else 0 end
		or isnull(@fileSize, 5) <> {11} 
		or isnull(@rolloverFiles, 1) <> {9}
		or isnull(@currentfile,'dm7XEWaits.xel') <> @xefilename)
		exec ('drop event session [' + @session +'] ON SERVER')
end

-- Trigger user-generated flag event to mark the point where we read back the data from the file
set @flagtime = 'SQLdm2 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
exec @rc = sp_trace_generateevent 82, @flagtime, NULL 

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
	ADD EVENT sqlserver.user_event,
	ADD EVENT sqlos.wait_info(
		ACTION(package0.collect_system_time,
		sqlos.task_elapsed_quantum,
		sqlos.task_time,sqlserver.client_app_name,
		sqlserver.client_hostname,
		sqlserver.database_id,
		sqlserver.database_name,
		sqlserver.is_system,
		sqlserver.nt_username,
		sqlserver.plan_handle,
		sqlserver.session_id,
		sqlserver.sql_text,
		sqlserver.tsql_frame,
		sqlserver.username) {7}),'
		select @sql1 = 'ADD EVENT sqlos.wait_info_external(    
		ACTION(package0.collect_system_time,
		sqlos.task_elapsed_quantum,
		sqlos.task_time,sqlserver.client_app_name,
		sqlserver.client_hostname,
		sqlserver.database_id,
		sqlserver.database_name,
		sqlserver.is_system,
		sqlserver.nt_username,
		sqlserver.plan_handle,
		sqlserver.session_id,
		sqlserver.sql_text,
		sqlserver.tsql_frame,
		sqlserver.username) {7}) 
	ADD TARGET package0.event_file(SET filename= ''' + @xefilename + ''',max_file_size=({11}),max_rollover_files=({9}))
	WITH (MAX_MEMORY={10} MB,EVENT_RETENTION_MODE={12},MAX_DISPATCH_LATENCY={8} SECONDS,MAX_EVENT_SIZE={13} MB,
	MEMORY_PARTITION_MODE={14},TRACK_CAUSALITY={15},STARTUP_STATE={16})'
	
	exec(@sql + @sql1);

	exec('Alter event session [' + @session + '] on server state = start')
end
else
begin --we only get here if the trace exists
	if 1={4} --we do this if we need to alter an existing trace. This also flushes mem to disk
	begin
		select @sql = '	ALTER EVENT SESSION [' + @session +'] ON SERVER
		DROP EVENT sqlos.wait_info,
		DROP EVENT sqlos.wait_info_external

		Alter EVENT SESSION [' + @session +'] ON SERVER 
		ADD EVENT sqlos.wait_info(
			ACTION(package0.collect_system_time,
			sqlos.task_elapsed_quantum,
			sqlos.task_time,sqlserver.client_app_name,
			sqlserver.client_hostname,
			sqlserver.database_id,
			sqlserver.database_name,
			sqlserver.is_system,
			sqlserver.nt_username,
			sqlserver.plan_handle,
			sqlserver.session_id,
			sqlserver.sql_text,
			sqlserver.tsql_frame,
			sqlserver.username) {7}),'
		select @sql1 = 'ADD EVENT sqlos.wait_info_external(    
			ACTION(package0.collect_system_time,
			sqlos.task_elapsed_quantum,
			sqlos.task_time,sqlserver.client_app_name,
			sqlserver.client_hostname,
			sqlserver.database_id,
			sqlserver.database_name,
			sqlserver.is_system,
			sqlserver.nt_username,
			sqlserver.plan_handle,
			sqlserver.session_id,
			sqlserver.sql_text,
			sqlserver.tsql_frame,
			sqlserver.username) {7})'
		
			exec(@sql + @sql1);
	end
	else
	begin
		exec ('alter event session [' + @session +'] ON SERVER drop event user_event')
		exec ('alter event session [' + @session +'] ON SERVER add event user_event')
	end
	--If it does not exist with a started state
	if not exists (select * from sys.dm_xe_sessions where name = @session)
		exec('Alter event session [' + @session + '] on server state = start')
end

--Read Query for extebded events
{17}