
-- Query Monitor Read 2012 For Azure
-- (0) Set to capture Actual Query Plan
-- (1) Cutoff Time in Seconds - 2/3 rd of Command / Collection Timeout in Seconds
-- (2) Set to collect top X queries at single go
-- Loops untill has time and collects X queries in each go and returns the result when timedout
-- Above parameters applicable for non sysadmin user only to allow read data from the batch

BEGIN
	DECLARE @startDate DATETIME;
	SELECT @startDate = GETUTCDATE();
	DECLARE @RingBufferXML nvarchar(max)
	DECLARE @SessionTargetCreationSql nvarchar(max)
	-- Consider Command Timeout for completion of query collection

	WHILE DATEDIFF(SECOND, @startDate, GETUTCDATE()) < {1}
		BEGIN
			BEGIN TRY
			Select  @RingBufferXML = target_data 
							FROM sys.dm_xe_database_session_targets dxdst
							JOIN sys.dm_xe_database_sessions AS dxds ON dxds.address = dxdst.event_session_address
							WHERE dxds.name = @session
				--START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- added offset and file name into the result
			END TRY
			BEGIN CATCH
				declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
				SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
				RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
			END CATCH
		END
			
    SELECT  DB_NAME()
	-- Select Final output
	SELECT @RingBufferXML

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
	
END
