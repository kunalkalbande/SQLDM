--------------------------------------------------------------------------------
--  Batch: Query Monitor Batch 2012 ( ExtENDed Events) for Azure Managed Instance
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--  Variables: --  [0] - show_plan predicates
-- [1] - File Name
-- [2] - swicth for altering WHERE clause
-- [3] - max_dispatch_latency
-- [4] - rolloverFiles
-- [5] - max_memory
-- [6] - file size
-- [7] - event_retention_mode
-- [8] - max_event_size
-- [9] - memory_partition_mode
-- [10] - startup_state
-- [11] - track_causality
-- [12] - ExtENDed Events
-- [13] - ReadQuery
-- [14] - CollectQueryPlan
		
--------------------------------------------------------------------------------

DECLARE @session nvarchar(255), @delayTime as nvarchar(20) --@xefilename as nvarchar(4000), 
 DECLARE @currenttime datetime, @rc varchar, @flagtime nvarchar(256)
DECLARE @path nvarchar(260), @mdpath nvarchar(260) --, @file nvarchar(260), @currentfile nvarchar(260)
DECLARE @sql nvarchar(max),@sqlOpt nvarchar(max),@sql1 nvarchar(max),@sql2 nvarchar(max);
DECLARE @StartTime datetime
DECLARE @name nvarchar(128), @event_retention_mode nvarchar(120), @max_dispatch_latency int, 
    @max_memory int, @max_event_size int, @memory_partition_mode nvarchar(120), 
    @track_causality bit, @startup_state bit --,@fileSize int, @rolloverFiles int
DECLARE @CurrentMaxMemoryKb INT, @MaxMemoryKb INT = (1024 * {4} * {6})/8;
DECLARE @SessionCreationSql nvarchar(max)

SELECT @session = '1' + replace(host_name(),' ','') + 'SQLdmQMEXevents'
SET @StartTime = getUTCDate();
SELECT @currenttime = getdate()

SELECT @name=name, 
    @event_retention_mode=event_retention_mode_desc, 
    @max_dispatch_latency = max_dispatch_latency, 
    @max_memory=max_memory, 
    @max_event_size=max_event_size, 
    @memory_partition_mode=memory_partition_mode_desc, 
    @track_causality=track_causality, 
    @startup_state=startup_state
FROM sys.server_event_sessions
WHERE name = @session

IF @name is not null
BEGIN
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
	if (@max_dispatch_latency <> {3}*1000 
		or @event_retention_mode <> '{7}'
		or @max_memory <> {5} * 1024
		or @max_event_size <> {8} * 1024
		or @memory_partition_mode <> '{9}'
		or @track_causality <> case '{11}' when 'ON' then 1 ELSE 0 END
		or @startup_state <> case '{10}' when 'ON' then 1 ELSE 0 END
		or isnull(@CurrentMaxMemoryKb, @MaxMemoryKb) <> @MaxMemoryKb)

		EXEC ('DROP EVENT SESSION [' + @session +'] ON SERVER')
END

--IF NOT EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE name = @session)
--BEGIN
	SELECT @sql = 'CREATE EVENT SESSION [' + @session +'] ON SERVER 
					ADD EVENT sqlserver.user_event, '
	if( 1= {14})
	BEGIN
		SELECT @sqlOpt = 'ADD EVENT sqlserver.query_post_EXECution_showplan(
					       ACTION(sqlserver.plan_handle){0}), ';
	END
	ELSE
	BEGIN
		SELECT @sqlOpt='' ;
	END

	SELECT @sql1={12}
	
	/*SELECT @sql2=' ADD TARGET package0.ring_buffer(SET max_memory =(' + CAST(@MaxMemoryKb/4 as varchar(MAX)) + '))
	 WITH (MAX_MEMORY={5} MB,EVENT_RETENTION_MODE={7},MAX_DISPATCH_LATENCY={3} SECONDS,MAX_EVENT_SIZE={8} MB,
		MEMORY_PARTITION_MODE={9},TRACK_CAUSALITY={10},STARTUP_STATE={11})';*/
	SELECT @sql2= 'ADD TARGET package0.ring_buffer(SET max_memory= ' + CAST(@MaxMemoryKb as varchar(MAX)) + ')
	WITH (EVENT_RETENTION_MODE={7},MAX_DISPATCH_LATENCY={3} SECONDS,
	MEMORY_PARTITION_MODE={9},TRACK_CAUSALITY={10},STARTUP_STATE={11})'
SET @SessionCreationSql = @sql+@sqlOpt+@sql1+@sql2;
IF NOT EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE name = @session)
BEGIN
	EXEC(@SessionCreationSql);

	EXEC('ALTER EVENT SESSION [' + @session + '] ON SERVER state = start')
END
ELSE
BEGIN --we only get here if the extENDed event session exists
	if 1={2} --we do this if we need to alter an existing extENDed event session. This also flushes mem to disk
	BEGIN
		SELECT @sql = '    
            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                WHERE sess.name = ''' + @session + ''' and events.name=''query_post_EXECution_showplan'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.query_post_EXECution_showplan
            END

            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                    join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                    WHERE sess.name = ''' + @session + ''' and events.name=''module_END'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.module_END
            END

            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                    join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                    WHERE sess.name = ''' + @session + ''' and events.name=''sp_statement_completed'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.sp_statement_completed
            END
                        
            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                    join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                    WHERE sess.name = ''' + @session + ''' and events.name=''sql_batch_completed'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.sql_batch_completed
            END
                        
            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                    join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                    WHERE sess.name = ''' + @session + ''' and events.name=''sql_statement_completed'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.sql_statement_completed
            END

            IF EXISTS(SELECT * FROM sys.server_event_sessions sess
                    join sys.server_event_session_events events on sess.event_session_id = events.event_session_id
                    WHERE sess.name = ''' + @session + ''' and events.name=''user_event'')
            BEGIN
                ALTER EVENT SESSION [' + @session +'] ON SERVER
                DROP EVENT sqlserver.user_event
            END        

            ALTER EVENT SESSION [' + @session +'] ON SERVER 
            ADD EVENT sqlserver.user_event, ';

		IF( 1= {14})
		BEGIN
			SELECT @sqlOpt=' ADD EVENT sqlserver.query_post_EXECution_showplan(
							  ACTION(sqlserver.plan_handle){0}), ';
		END
		ELSE
		BEGIN
			SELECT @sqlOpt='' ;
		END

		SELECT @sql1= {12}
		EXEC(@sql+@sqlOpt+@sql1);

	END
	
	--If it does not exist with a started state
	IF NOT EXISTS (SELECT * FROM sys.dm_xe_sessions WHERE name = @session)
        EXEC('ALTER EVENT SESSION [' + @session + '] ON SERVER state = start')
END

--Read Query will be here
{13}
