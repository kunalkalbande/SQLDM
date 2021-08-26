--Read Query will be here
-- Batch  Read Activity Monitor Extended Events 2012
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--Variables
--  [0] - Readng time for Session Data
--Readng Existing Session
IF @Existsession IS not null
Begin
    Print 'Existing Session Read';
	DECLARE @startDate DATETIME;
	SELECT @startDate = GETUTCDATE();
	--DECLARE @RingBufferXML XML
	DECLARE @RingBufferXML nvarchar(max)
	-- Consider Command Timeout for completion of query collection

	WHILE DATEDIFF(SECOND, @startDate, GETUTCDATE()) < {0}
		BEGIN
			BEGIN TRY
			Select  @RingBufferXML = target_data
							FROM sys.dm_xe_session_targets dxdst
							JOIN sys.dm_xe_sessions AS dxds ON dxds.address = dxdst.event_session_address
							WHERE dxds.name = @Existsession
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
	EXEC ('DROP EVENT SESSION [' + @Existsession+'] ON SERVER')
	
END
Else
Begin
   SELECT  DB_NAME()
   --SELECT Null
END
