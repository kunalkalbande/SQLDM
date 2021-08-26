--------------------------------------------------------------------------------
--  Batch: Active Waits Batch with extended events without extended events api for Azure Managed Instance
--  Variables: [0] - Runtime in Seconds
--  [1] - File Name
--  [2] - Filter statements Level 1
--  [3] - Filter statements Level 2
--  [4] - swicth for altering where clause
--  [5] - top (@Rowcountmax) or null
--  [6] - Excluded wait types
--  [7] - Use tempdb for lower compatibility level databases
--------------------------------------------------------------------------------

DECLARE @session NVARCHAR(255), @delayTime AS NVARCHAR(20)

select @session = replace(host_name(),' ','') + 'SQLdmResourceWaits'

declare @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
declare @sql nvarchar(max);
declare @sql1 nvarchar(max);
declare @StartTime datetime

set @StartTime = getUTCDate();

declare @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
@max_memory int, 
--@max_event_size int, 
@memory_partition_mode nvarchar(120), 
@track_causality bit, @startup_state bit,@fileSize int, @rolloverFiles int
-- Used for Ring Buffer
-- max_file_size=(5),max_rollover_files=(3)
-- max_file_size=({11}),max_rollover_files=({9})
DECLARE @CurrentMaxMemoryKb INT, @MaxMemoryKb INT = (1024 * {11} * {9}) / 2;
DECLARE @SessionTargetCreationSql nvarchar(max)
select @currenttime = getdate()

-- Set up the trace file as [HostName]dm5wptrace in the location of the SQL Server error log
--if the user has entered a whole path then don't just put the file in the log directory

--session parameters (not file)
SELECT @name=name, 
@event_retention_mode=event_retention_mode_desc, 
@max_dispatch_latency = max_dispatch_latency, 
@max_memory=max_memory, 
@memory_partition_mode=memory_partition_mode_desc, 
@track_causality=track_causality, 
@startup_state=startup_state 
FROM sys.server_event_sessions
where name = @session

if @name is not null
begin
	-- file size and rollover files
	SELECT @CurrentMaxMemoryKb = convert(int, max_memory)
	FROM (
		SELECT  sess.name as sessionName,fields.name as fieldName, fields.value
		FROM sys.server_event_sessions sess 
		inner join sys.server_event_session_fields fields on sess.event_session_id = fields.event_session_id
		WHERE sess.name = @session) as d
		pivot(max(value) for fieldName in([max_memory])
	) as maxval

	--if the session parameters have changed then drop it so it will be recreated
	if (@max_dispatch_latency <> 300*1000 
		or @event_retention_mode <> 'ALLOW_MULTIPLE_EVENT_LOSS'
		or @memory_partition_mode <> 'NONE'
		or @track_causality <> case 'OFF' when 'ON' then 1 else 0 end
		or @startup_state <> case 'OFF' when 'ON' then 1 else 0 end
		or isnull(@CurrentMaxMemoryKb, @MaxMemoryKb) <> @MaxMemoryKb
		)
	BEGIN
		exec ('drop event session [' + @session +'] ON SERVER')
		PRINT 'DELETED Changed Properties'
	END
end

-- Trigger user-generated flag event to mark the point where we read back the data from the file
set @flagtime = 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
--exec @rc = sp_trace_generateevent 82, @flagtime, NULL 

select @flagtime, ms_ticks
from sys.dm_os_sys_info;

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
	ADD TARGET package0.ring_buffer(SET max_memory= ' + CAST(@MaxMemoryKb as varchar(MAX)) + ')
	WITH (EVENT_RETENTION_MODE={12},MAX_DISPATCH_LATENCY={8} SECONDS,
	MEMORY_PARTITION_MODE={14},TRACK_CAUSALITY={15},STARTUP_STATE={16})'
	
	exec(@sql + @sql1);

	exec('Alter event session [' + @session + '] ON SERVER state = start')
	
	PRINT 'Not Exists'

end
else
begin --we only get here if the trace exists
	if 1={4} --we do this if we need to alter an existing trace. This also flushes mem to disk
	begin
		PRINT 'Flush Force Alter'
		
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
	
		PRINT 'Alter user_event in session'

		exec ('alter event session [' + @session +'] ON SERVER drop event user_event')
		exec ('alter event session [' + @session +'] ON SERVER add event user_event')
	end
	--If it does not exist with a started state
	if not exists (select * from sys.dm_xe_sessions where name = @session)
	BEGIN
		PRINT 'Start Session'
		exec('Alter event session [' + @session + '] ON SERVER state = start')
	END
end

SELECT target_data AS event_data
FROM sys.dm_xe_session_targets dxdst
JOIN sys.dm_xe_sessions AS dxds ON dxds.address = dxdst.event_session_address
WHERE dxds.name = @session;


	;with dmvwaits as (select (
			select ms_ticks from sys.dm_os_sys_info
		) as Ticks,
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

IF EXISTS(SELECT 1 FROM sys.database_event_sessions WHERE name = @session)
	BEGIN

		EXEC('ALTER EVENT SESSION [' + @session + ']
				ON DATABASE
				DROP TARGET package0.ring_buffer;'
		)
		SELECT @SessionTargetCreationSql = 'ALTER EVENT SESSION [' + @session + '] ON DATABASE '
			+ 'ADD TARGET package0.ring_buffer(SET max_memory= ' +
			CAST(@MaxMemoryKb as varchar(MAX)) +
			')'
		EXEC(@SessionTargetCreationSql)
	END