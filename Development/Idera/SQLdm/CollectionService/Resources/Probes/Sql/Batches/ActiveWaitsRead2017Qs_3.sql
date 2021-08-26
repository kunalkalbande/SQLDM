-- Batch Read Query Waits Data using Query Store 2017
-- Added new batch for SQLdm 10.4 by Varun Chopra
-- ActiveWaitsRead2017Qs.sql
-- (0) FilterString               -  Filters for Query Store for Waits with generic predicates on SQL Text and Duration / Reads / Writes / CPU
-- (1) Session Filter             -  To apply the Application name and exclude SQLdm Filters
-- (2) Database Filter            -  To add conditions like db in / db not in / db matches / db not matches
-- (3) TopPlanCountFilter         -  Top X Queries to be selected
-- NOTE:
-- [Ticks] calculated using StartTime.Ticks; 
-- [MSTicks] calculated -1 * (long.Parse(row["WaitDuration"].ToString())
--
-- Create the temp table ActiveWaitsQsState if not exists for latest query data and offset to continue reading from LastStartTime
-- LastStartTime defaults to UTC Time in the begining
-- Generally, ActiveWaitsQsState gets refreshed during restart and older values will persist if query store is disabled currently
 IF (object_id('tempdb..ActiveWaitsQsState') IS NULL)  
BEGIN
	CREATE TABLE tempdb..ActiveWaitsQsState(DatabaseID INT, DBName nvarchar(max),LastStartTime DATETIMEOFFSET)
END
--
-- [STEP 1]
-- Find out Databases where Query Store is Enabled
--
-- #QsWaitsEnabledTable - Stores Database and Actual State of the database
IF OBJECT_ID('tempdb..#QsWaitsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsWaitsEnabledTable
END
CREATE TABLE #QsWaitsEnabledTable
(
	DatabaseID INT,
	DBName NVARCHAR(MAX),
	ActualState SMALLINT  -- actual_state	smallint - Indicates the operation mode of Query Store 0 = OFF / 1 = READ_ONLY / 2 = READ_WRITE / 3 = ERROR
)

-- Get Databases where QS is enabled
DECLARE @QsEnabledCommand nvarchar(2000);
SET @QsEnabledCommand = 'USE [?]
IF {2}
BEGIN
  SELECT DB_ID(), DB_NAME(DB_ID()), actual_state 
  FROM sys.database_query_store_options
  WHERE actual_state = 1 OR actual_state = 2
END'

INSERT INTO #QsWaitsEnabledTable
EXEC sp_MSforeachdb @QsEnabledCommand

-- Create Temporary Table to store data
IF OBJECT_ID('tempdb..WaitsStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..WaitsStoreData
END
CREATE TABLE tempdb..WaitsStoreData (
[WaitDuration] BIGINT,
[session_id] SMALLINT,
[Wait Type] NVARCHAR(MAX),
[HostName] NVARCHAR(MAX),
[program_name] NVARCHAR(MAX),
[LoginName] NVARCHAR(MAX),
[DatabaseName] NVARCHAR(MAX),
[statement_txt] NVARCHAR(MAX),
[StartTime] DATETIMEOFFSET,
[DatabaseID] INT
);

-- Create new clustered index on [WaitDuration]
CREATE CLUSTERED INDEX IX_WaitsStoreData_WaitDuration
ON tempdb..WaitsStoreData([WaitDuration] DESC);

-- Return if no database got query store enabled
IF NOT EXISTS (SELECT * FROM #QsWaitsEnabledTable)
BEGIN

SELECT
[WaitDuration],
[session_id],
[Wait Type],
[HostName],
[program_name],
[LoginName],
[DatabaseName],
[statement_txt],
[StartTime]
FROM tempdb..WaitsStoreData

-- Clear TempTables
IF OBJECT_ID('tempdb..#QsWaitsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsWaitsEnabledTable
END
IF OBJECT_ID('tempdb..WaitsStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..WaitsStoreData
END
-- Drop All data from ActiveWaitsQsState
IF OBJECT_ID('tempdb..ActiveWaitsQsState') IS NOT NULL
BEGIN
	DROP TABLE tempdb..ActiveWaitsQsState
END
RETURN;
END

--
-- [STEP 2]
-- Read all watis data from query store
-- Note: we use Top X Filter to obtain Query Plan for only Top X Queries
--

DECLARE @QsReadCommand nvarchar(MAX);
SET @QsReadCommand = '
USE [?]
DECLARE @s SMALLINT
SELECT @s = actual_state FROM sys.database_query_store_options WHERE actual_state = 1 OR actual_state = 2
IF @s IS NOT NULL
BEGIN
DECLARE @DBName NVARCHAR(MAX);
SELECT @DBName = DB_NAME(DB_ID());

DECLARE @DatabaseID INT;
SELECT @DatabaseID = DB_ID();

DECLARE @LastStartTime DATETIMEOFFSET;
SELECT @LastStartTime = LastStartTime FROM tempdb..ActiveWaitsQsState WHERE DatabaseID = @DatabaseID;

-- Set LastStartTime to UTC for newly added entries or null entries in tempdb..ActiveWaitsQsState 
IF NOT EXISTS (SELECT DatabaseID FROM tempdb..ActiveWaitsQsState WHERE DatabaseID = @DatabaseID)
BEGIN
 SELECT @LastStartTime = GETUTCDATE();
 INSERT INTO tempdb..ActiveWaitsQsState (DatabaseID,DBName,LastStartTime)
  VALUES (@DatabaseID,@DBName, @LastStartTime)
END
ELSE IF @LastStartTime IS NULL
BEGIN 
 SELECT @LastStartTime = GETUTCDATE();
 UPDATE tempdb..ActiveWaitsQsState
  SET LastStartTime = GETUTCDATE()
 WHERE DatabaseID = @DatabaseID;
END

;WITH qd AS (
SELECT
    qsws.last_query_wait_time_ms AS [WaitDuration],
	qsws.wait_category_desc AS [Wait Type],
    qsqt.query_sql_text AS [statement_txt],
    DATEADD(microsecond, -qsrs.last_duration, qsrs.last_execution_time) AS [StartTime],
    qsq.query_hash -- For merging with Sessions
FROM sys.query_store_wait_stats qsws
INNER JOIN sys.query_store_plan qsp ON qsp.plan_id = qsws.plan_id
INNER JOIN sys.query_store_query qsq ON  qsp.query_id = qsq.query_id
INNER JOIN sys.query_store_query_text qsqt ON qsqt.query_text_id = qsq.query_text_id
INNER JOIN sys.query_store_runtime_stats_interval qsrsi ON qsws.runtime_stats_interval_id = qsrsi.runtime_stats_interval_id
INNER JOIN sys.query_store_runtime_stats qsrs ON qsrs.plan_id = qsp.plan_id
LEFT JOIN sys.objects so ON so.object_id = qsq.object_id
{0}
),
-- session data
sd as (SELECT
         des.session_id AS [session_id],
		 des.host_name AS [HostName], 
         des.program_name AS [program_name], 
	     des.login_name AS [LoginName],
	     der.query_hash
       FROM sys.dm_exec_requests der 
	   LEFT JOIN sys.dm_exec_sessions des ON der.session_id = des.session_id
       {1}
)
SELECT
           qd.[WaitDuration],
	   sd.[session_id],
	   qd.[Wait Type],
	   sd.HostName,
	   sd.[program_name],
	   sd.[LoginName],
           @DBName AS [DatabaseName],
	   qd.[statement_txt],
	   qd.[StartTime],
           @DatabaseID AS DatabaseID
FROM qd
LEFT OUTER JOIN sd ON qd.query_hash = sd.query_hash
END'

-- Loop and read Query Data except Query Plan Information
DECLARE @sqlQsReadCommand NVARCHAR(MAX);
DECLARE @replace_character NCHAR(1) = N'?';
DECLARE QsDbCursor CURSOR 
LOCAL FORWARD_ONLY STATIC READ_ONLY
FOR SELECT DatabaseID, DBName
FROM #QsWaitsEnabledTable

OPEN QsDbCursor;

DECLARE @DBName NVARCHAR(300);
DECLARE @DatabaseID INT;
DECLARE @LastStartTime DATETIMEOFFSET;

-- Create Temporary Table to temporarily store query store data per database
IF OBJECT_ID('tempdb..TempWaitsStoreData') IS NOT NULL
BEGIN
    DROP TABLE tempdb..TempWaitsStoreData
END
CREATE TABLE tempdb..TempWaitsStoreData(
[WaitDuration] BIGINT,
[session_id] SMALLINT,
[Wait Type] NVARCHAR(MAX),
[HostName] NVARCHAR(MAX),
[program_name] NVARCHAR(MAX),
[LoginName] NVARCHAR(MAX),
[DatabaseName] NVARCHAR(MAX),
[statement_txt] NVARCHAR(MAX),
[StartTime] DATETIMEOFFSET,
[DatabaseID] INT
);
-- Create new clustered index on StartTime
CREATE CLUSTERED INDEX TempWaitsStoreData_StartTime
ON tempdb..TempWaitsStoreData([StartTime] DESC);

FETCH NEXT FROM QsDbCursor into @DatabaseID,@DBName;
WHILE @@FETCH_STATUS = 0
BEGIN 
 -- Create dynamicSQL Query Store Read Command
 SET @sqlQsReadCommand = REPLACE(@QsReadCommand, @replace_character, @DBName);

 -- Insert into Temporary Query Waits Data Table
 INSERT INTO tempdb..TempWaitsStoreData( [WaitDuration], [session_id], [Wait Type], [HostName], [program_name], [LoginName], 
                                 [DatabaseName], [statement_txt], [StartTime], [DatabaseID])
 EXEC sp_executesql @sqlQsReadCommand;
 
 -- Data Present
 IF EXISTS(SELECT DatabaseID FROM tempdb..TempWaitsStoreData)
 BEGIN
  -- Insert data into main Query Store Data Table
  INSERT INTO tempdb..WaitsStoreData ( [WaitDuration], [session_id], [Wait Type], [HostName], [program_name], [LoginName], [DatabaseName], 
                                        [statement_txt], [StartTime], [DatabaseID])
  SELECT  [WaitDuration], [session_id], [Wait Type], [HostName], [program_name], [LoginName], [DatabaseName], 
                                         [statement_txt], [StartTime], [DatabaseID]
  FROM tempdb..TempWaitsStoreData

  -- Get Last Execution Time from newly added data from tempdb..TempWaitsStoreData
  SELECT TOP (1) @LastStartTime = StartTime FROM tempdb..TempWaitsStoreData ORDER BY StartTime DESC

  -- Check Not Null since maybe we don't have any data based on filters
  IF @LastStartTime IS NOT NULL
  BEGIN
   -- Store Last Execution Time to tempdb..ActiveWaitsQsState 
   IF EXISTS (SELECT DatabaseID FROM tempdb..ActiveWaitsQsState where DatabaseID = @DatabaseID)
   BEGIN
    UPDATE tempdb..ActiveWaitsQsState SET LastStartTime = @LastStartTime
     WHERE DatabaseID = @DatabaseID
   END
   ELSE
   BEGIN
    INSERT INTO tempdb..ActiveWaitsQsState (DatabaseID,DBName,LastStartTime)
     VALUES (@DatabaseID,@DBName, @LastStartTime)
   END
  END
 END
 TRUNCATE TABLE tempdb..TempWaitsStoreData;

 FETCH NEXT FROM QsDbCursor INTO @DatabaseID,@DBName;
END
CLOSE QsDbCursor;
DEALLOCATE QsDbCursor;

-- Clear tempdb..TempWaitsStoreData
IF OBJECT_ID('tempdb..TempWaitsStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..TempWaitsStoreData
END

-- Clear #QsWaitsEnabledTable
IF OBJECT_ID('tempdb..#QsWaitsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsWaitsEnabledTable
END

-- Fetch Query Store data
SELECT {3}
 [WaitDuration],
 [session_id],
 [Wait Type],
 [HostName],
 [program_name],
 [LoginName],
 [DatabaseName],
 [statement_txt],
 [StartTime] 
FROM tempdb..WaitsStoreData
GROUP BY
	[session_id],
	[Wait Type],
	[WaitDuration],
	[HostName],
	[program_name],
	[LoginName],
	[DatabaseName],
	[statement_txt],
	[StartTime]

-- Drop WaitsStoreData
IF OBJECT_ID('tempdb..WaitsStoreData') IS NOT NULL
BEGIN
  DROP TABLE tempdb..WaitsStoreData
END