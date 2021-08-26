--Read Query will be here
-- Batch  Read Activity Monitor Extended Events 2012
-- Added new batch for SQLdm 9.1 by Ankit Srivastava
--Variables
--  [0] - Readng time for Session Data
--Readng Existing Session

IF @Existsession IS not null
Begin
    Print 'Existing Session Read';
	DECLARE @startDateExist DATETIME;
	SELECT @startDateExist = GETUTCDATE();
	--DECLARE @RingBufferExistXML XML
	DECLARE @RingBufferExistXML nvarchar(max)
	-- Consider Command Timeout for completion of query collection

	WHILE DATEDIFF(SECOND, @startDateExist, GETUTCDATE()) < {0}
		BEGIN
			BEGIN TRY
			Select  @RingBufferExistXML = target_data
							FROM sys.dm_xe_database_session_targets dxdst
							JOIN sys.dm_xe_database_sessions AS dxds ON dxds.address = dxdst.event_session_address
							WHERE dxds.name = @Existsession
			END TRY
			BEGIN CATCH
				declare @Error nvarchar(max), @ErrorSeverityOld int, @ErrorStateOld int;
				SELECT @Error = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverityOld = ERROR_SEVERITY(), @ErrorStateOld = ERROR_STATE();
				RAISERROR (@Error, @ErrorSeverityOld, @ErrorStateOld); -- raising the error and logging it as warning 
			END CATCH
		END
			
    SELECT  DB_NAME()
	-- Select Final output
	SELECT @RingBufferExistXML
	EXEC ('DROP EVENT SESSION [' + @Existsession+'] ON DATABASE')
END
Else
Begin
   SELECT  DB_NAME()
   --SELECT Null
END
-- DATA READ END