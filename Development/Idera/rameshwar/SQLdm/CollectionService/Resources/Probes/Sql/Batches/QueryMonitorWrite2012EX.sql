-- SQL Diagnostic Manager v10.2.0.0
-- Copyright © IDERA, Inc. 2003-2017 
-- Batch - Write Query Monitor Extended Events 2012 Last Read Offsets
-- Added new batch for SQLdm 10.2 by Anshul Aggarwal
--Variables
--  [0] - file name
--  [1] - last record count for file

BEGIN TRY

declare @LastFileName nvarchar(max)
declare @LastRecordCount bigint

set @LastFileName = '{0}'
set @LastRecordCount = {1}

--Create the temp table QMExtendedEvents if not exists for latest filename and offset
IF (object_id('tempdb..QMExtendedEventsState') is null)  
BEGIN
	CREATE TABLE tempdb..QMExtendedEventsState(LastFileName nvarchar(max),LastRecordCount bigint)
END

IF NULLIF(@LastFileName, '') is not null
BEGIN
	DELETE FROM tempdb..QMExtendedEventsState
	INSERT INTO tempdb..QMExtendedEventsState values (@LastFileName, @LastRecordCount)
END

END TRY
BEGIN CATCH
    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH