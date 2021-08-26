--------------------------------------------------------------------------------
--  Batch: Activity Monitor Batch 2012 ( Extended Events)
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--  Variables: --   [0] - File Name
-- [1] - swicth for altering where clause
-- [2] - max_dispatch_latency
-- [3] - rolloverFiles
-- [4] - max_memory
-- [5] - file size
-- [6] - event_retention_mode
-- [7]- max_event_size
-- [8]- memory_partition_mode
-- [9]- startup_state
-- [10]- track_causality
-- [11] - Extended Events
-- [12] - ReadQuery

		
--------------------------------------------------------------------------------
DECLARE @Existsession nvarchar(255)
DECLARE @format nvarchar(255)
SELECT @format = '1'+replace(host_name(),' ','') + 'SQLdmAMEXevents%'
--select Top(1) name FROM sys.database_event_sessions WHERE name like @format
set @Existsession = (select Top(1) name FROM sys.server_event_sessions WHERE name like @format)

declare @session nvarchar(255), @delayTime as nvarchar(20) 


select @session = '1'+replace(host_name(),' ','') + 'SQLdmAMEXevents'


declare @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
declare @path nvarchar(260), @mdpath nvarchar(260), @file nvarchar(260), @currentfile nvarchar(260)
declare @sql nvarchar(max),@sql1 nvarchar(max),@sql2 nvarchar(max);
declare @StartTime datetime

select @StartTime = getUTCDate();
select @session = '1'+replace(host_name(),' ','') + 'SQLdmAMEXevents' +convert(varchar, getUTCdate(), 21)
DECLARE @CurrentMaxMemoryKb INT, @MaxMemoryKb INT = (1024 * {3} *{5}) / 4;

declare @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
@max_memory int, @max_event_size int, @memory_partition_mode nvarchar(120), 
@track_causality bit, @startup_state bit,@fileSize int, @rolloverFiles int

select @currenttime = getdate()



-- SQL DM 10.5 - DROP session if it contains ERRORLOG in path

IF Not EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE name = @session)
begin
	select @sql = 'CREATE EVENT SESSION [' + @session +'] ON SERVER '
					--ADD EVENT sqlserver.user_event, 
	select @sql1={11}
	
	SELECT @sql2= 'ADD TARGET package0.ring_buffer(SET max_memory= ' + CAST(@MaxMemoryKb as varchar(MAX)) + ')
	WITH (EVENT_RETENTION_MODE={6},MAX_DISPATCH_LATENCY={2} SECONDS,
	MEMORY_PARTITION_MODE={8},TRACK_CAUSALITY={9},STARTUP_STATE={10})';
	exec(@sql+@sql1+@sql2);

	exec('Alter event session [' + @session + '] on server state = start')
end
else
begin --we only get here if the extended event session exists
	if 1={1} --we do this if we need to alter an existing extended event session.
	begin
		select @sql = '	if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''blocked_process_report'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.blocked_process_report
						end

						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''database_file_size_change'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.database_file_size_change
						end
						
						if exists(select * from sys.server_event_sessions sess
								join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
								where sess.name = '''+@session+''' and events.name=''xml_deadlock_report'')
						begin
							ALTER EVENT SESSION [' + @session +'] ON SERVER
							DROP EVENT sqlserver.xml_deadlock_report
						end				

						Alter EVENT SESSION [' + @session +'] ON SERVER ';
						
	select @sql1= {11}
	
	exec(@sql+@sql1);

	end
	
	--If it does not exist with a started state
	if not exists (select * from sys.dm_xe_sessions where name = @session)
		exec('Alter event session [' + @session + '] on server state = start')
end

--Read Query will be here
{12}
