--------------------------------------------------------------------------------
--  Batch: Activity Monitor Batch 2012 ( Extended Events)
--  SQLdm 10.3 (Varun Chopra) Linux Support
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
SELECT @format = replace(host_name(),' ','') + 'SQLdmAMEXevents%'
--select Top(1) name FROM sys.database_event_sessions WHERE name like @format
set @Existsession = (select Top(1) name FROM sys.database_event_sessions WHERE name like @format)

DECLARE @session nvarchar(255), @delayTime as nvarchar(20) --@xefilename as nvarchar(4000), 
 DECLARE @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
DECLARE @path nvarchar(260), @mdpath nvarchar(260) --, @file nvarchar(260), @currentfile nvarchar(260)
DECLARE @sql nvarchar(max),@sqlOpt nvarchar(max),@sql1 nvarchar(max),@sql2 nvarchar(max);
DECLARE @StartTime datetime
DECLARE @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
	@max_memory int, @max_event_size int, @memory_partition_mode nvarchar(120), 
	@track_causality bit, @startup_state bit --,@fileSize int, @rolloverFiles int
DECLARE @CurrentMaxMemoryKb INT, @MaxMemoryKb INT = (1024 * {3} *{5}) / 2;
SELECT @session = replace(host_name(),' ','') + 'SQLdmAMEXevents' + convert(varchar, getUTCdate(), 21)
SELECT @StartTime = getUTCDate();
SELECT @currenttime = getdate()


IF Not EXISTS(SELECT 1 FROM sys.database_event_sessions WHERE name = @session)
BEGIN
	SELECT @sql = 'CREATE EVENT SESSION [' + @session +'] ON DATABASE' 
					--ADD EVENT sqlserver.user_event,'
	SELECT @sql1={11}
	
	SELECT @sql2= 'ADD TARGET package0.ring_buffer(SET max_memory= ' + CAST(@MaxMemoryKb as varchar(MAX)) + ')
	WITH (EVENT_RETENTION_MODE={6},MAX_DISPATCH_LATENCY={2} SECONDS,
	MEMORY_PARTITION_MODE={8},TRACK_CAUSALITY={9},STARTUP_STATE={10})';

	PRINT @sql+@sql1+@sql2;
	
	EXEC(@sql+@sql1+@sql2)
	EXEC('ALTER EVENT SESSION [' + @session + '] ON DATABASE state = start')
END
ELSE
BEGIN --we only get here if the extended event session exists
	IF 1={1} --we do this if we need to alter an existing extended event session.
	BEGIN
		SELECT @sql = '
			IF EXISTS(SELECT * FROM sys.database_event_sessions sess
				JOIN sys.database_event_session_events events on sess.event_session_id = events.event_session_id
				WHERE sess.name = '''+@session+''' and events.name=''blocked_process_report'')
			BEGIN
				ALTER EVENT SESSION [' + @session +'] ON DATABASE
				DROP EVENT sqlserver.blocked_process_report
			END
						
			IF EXISTS(SELECT * FROM sys.database_event_sessions sess
				JOIN sys.database_event_session_events events on sess.event_session_id = events.event_session_id
				WHERE sess.name = '''+@session+''' and events.name=''database_xml_deadlock_report'')
			BEGIN
				ALTER EVENT SESSION [' + @session +'] ON DATABASE
				DROP EVENT sqlserver.database_xml_deadlock_report
			END				

			ALTER EVENT SESSION [' + @session +'] ON DATABASE ';
				--if exists(select * from sys.server_event_sessions sess
				--		join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
				--		where sess.name = '''+@session+''' and events.name=''user_event'')
				--begin
				--	ALTER EVENT SESSION [' + @session +'] ON SERVER
				--	DROP EVENT sqlserver.user_event
				--end
				--ADD EVENT sqlserver.user_event, ';
	
		SELECT @sql1= {11}
		EXEC(@sql+@sql1);

	END
	
	--If it does not exist with a started state
	IF not exists (SELECT * FROM sys.dm_xe_database_sessions WHERE name = @session)
		EXEC('ALTER EVENT SESSION [' + @session + '] ON DATABASE state = start')

END

--Read Query will be here
{12}
