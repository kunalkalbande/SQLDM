-- Batch  Read Active Waits Extended Events 2012
-- Added new batch for SQLdm 10.3 by Tushar Saini
--Variables
--  [0] - ReadQueryFilter

declare @LastFileName nvarchar(max)
declare @LastRecordCount bigint

BEGIN TRY

IF (object_id('tempdb..AWExtendedEvent') is not null)  
BEGIN
	DROP TABLE tempdb..AWExtendedEvent
END

IF (object_id('tempdb..AWExtendedEventsState') is not null)  
BEGIN
	SELECT @LastFileName = LastFileName, @LastRecordCount = LastRecordCount from tempdb..AWExtendedEventsState
END

select @LastFileName, @LastRecordCount

declare @fileStart nvarchar(260)=null

SELECT 
    @path = LEFT(column_value, LEN(column_value)-CHARINDEX('\', REVERSE(column_value))),
    @fileStart = RIGHT(column_value, CHARINDEX('\', REVERSE(column_value))-1)
FROM sys.dm_xe_sessions s
JOIN sys.dm_xe_session_object_columns soc
    ON s.address = soc.event_session_address
WHERE s.name = @session
  AND soc.column_name = 'filename'

-- Getting data before extension .xel
--SQLDM 10.3 (Tushar) -- Fix for SQLDM-28737--Handling case insensitive sql server instances.
set @fileStart = substring(@fileStart, 0,len(@fileStart)- CHARINDEX('.', reverse(@fileStart))+1)
  
  IF OBJECT_ID('tempdb..AWExtendedEventsDirectoryTree') IS NOT NULL
      DROP TABLE tempdb..AWExtendedEventsDirectoryTree;

CREATE TABLE tempdb..AWExtendedEventsDirectoryTree (
       id int IDENTITY(1,1)
      ,subdirectory nvarchar(512)
      ,depth int
      ,isfile bit);

INSERT tempdb..AWExtendedEventsDirectoryTree (subdirectory,depth,isfile)
EXEC sys.xp_dirtree @path,1,1;

select subdirectory, FileNameInMilliseconds
from (
	SELECT subdirectory,
	SUBSTRING(subdirectory, LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 2, 
	LEN(subdirectory) - CHARINDEX('.', REVERSE(subdirectory)) - (LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 1)) as FileNameInMilliseconds
	FROM tempdb..AWExtendedEventsDirectoryTree
	where subdirectory like @fileStart + '%.xel' 
) as fileData
where FileNameInMilliseconds >= ISNULL(@LastFileName, 0)
order by FileNameInMilliseconds asc

END TRY

BEGIN CATCH
    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH