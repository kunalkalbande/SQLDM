--------------------------------------------------------------------------------
--  Batch: Active Waits Batch with extended events without extended events api
--  Variables: [0] - Runtime in Seconds
--  [1] - File Name
--  [2] - Filter statements Level 1
--  [3] - Filter statements Level 2
--  [4] - swicth for altering where clause
--  [5] - top (@Rowcountmax) or null
--  [6] - Excluded wait types
--  [7] - Use tempdb for lower compatibility level databases
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

-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
--if the user has entered a whole path then don't just put the file in the log directory
if charindex('\',@file,0) > 0
	set @xefilename = @file
else
begin
	set @xefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
	set @xefilename = left(@xefilename,len(@xefilename) - charindex('\',reverse(@xefilename))) + '\' + replace(host_name(),' ','') + @file
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
		or @track_causality <> case '{16}' when 'ON' then 1 else 0 end
		or @startup_state <> case '{15}' when 'ON' then 1 else 0 end
		or isnull(@fileSize, 5) <> {11} 
		or isnull(@rolloverFiles, 1) <> {9}
		or isnull(@currentfile,'dm7XEWaits.xel') <> @xefilename)
		exec ('drop event session [' + @session +'] ON SERVER')
end

-- Trigger user-generated flag event to mark the point where we read back the data from the file
set @flagtime = 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
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

-- Get the log file name and substitute * wildcard in
SELECT 
    @path = LEFT(column_value, LEN(column_value)-CHARINDEX('.', REVERSE(column_value))) 
        + '*' 
        + RIGHT(column_value, CHARINDEX('.', REVERSE(column_value))-1)
FROM sys.dm_xe_sessions s
JOIN sys.dm_xe_session_object_columns soc
    ON s.address = soc.event_session_address
WHERE s.name = @session
  AND soc.column_name = 'filename'

  -- Set the metadata filename if it is NULL to the log file name with xem extension
SELECT @mdpath = ISNULL(@mdpath, 
                        LEFT(@path, LEN(@path)-CHARINDEX('*', REVERSE(@path))) 
                        + '*xem')

SELECT {5} 
    object_name, CAST(event_data AS XML) AS event_data, file_offset
 FROM sys.fn_xe_file_target_read_file(@path, @mdpath, null, null)
	;with dmvwaits as (select (select ms_ticks from sys.dm_os_sys_info) as Ticks,
		wait_duration_ms, 
		wt.session_id, 
		wt.wait_type,
		host_name,
		program_name,
		login_name,
		db_name(r.database_id) as db_name,
		statement_txt = case when isnull(is_user_process,0) = 0 then null else 
			(select substring(text, statement_start_offset/2 + 1,
			(case when statement_end_offset < 1
			then len(convert(nvarchar(max),text)) * 2
			else statement_end_offset
			end - statement_start_offset)/2 + 1)
			from sys.dm_exec_sql_text(sql_handle)) end,
		MSTicks = wait_started_ms_ticks,
		case when wait_duration_ms > 2000000000 then Dateadd(s,-wait_duration_ms/1000,getutcdate()) else Dateadd(ms,-wait_duration_ms,getutcdate()) end as start_time,
		StartTime = @StartTime,
		r.database_id
	from 
		sys.dm_os_waiting_tasks wt left join sys.dm_os_workers w on wt.waiting_task_address = w.task_address	
		left join sys.dm_exec_sessions s on wt.session_id = s.session_id	
		left join sys.dm_exec_requests r on r.session_id = s.session_id
	where
		wt.wait_type not in ({6}) 
		and r.status <> 'sleeping'
		and r.session_id >= 50
		and s.session_id >= 50
		and wait_duration_ms > 50
		and isnull(is_user_process,0) = 1
		and r.session_id <> @@spid)
	select {5} * from (
	select * 
		from dmvwaits r 
		where r.statement_txt is not null and len(rtrim(r.statement_txt )) > 0 {2}) as d {3}


select @delayTime = convert(nvarchar(20), dateadd(s,{0},0), 114)

exec('waitfor delay ''' + @delayTime + '''')