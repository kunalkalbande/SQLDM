-- Batch  Read Query Monitor Query Store 2016
-- Added new batch for SQLdm 10.4 by Varun Chopra
-- QueryMonitorRead2016Qs.sql
-- (0) topPlanCategory            -  Top Plan Category i.e Duration / Reads / Writes / CPU
-- (1) FilterString               -  Filters for Query Store with generic predicates on SQL Text and Duration / Reads / Writes / CPU
-- (2) TopPlanCountFilter         -  Top X Queries to be selected
-- (3) Session Filter             -  To apply the Application name and exclude SQLdm Filters
-- (4) Database Filter            -  To add conditions like db in / db not in / db matches / db not matches
-- (5) Collect Actual Plans       -  Set to 1 if we need to collect actual plan, 0 otherwise
--
-- Create the temp table QMQueryStoreState if not exists for latest query data and offset to continue reading from LastStartTime
-- LastStartTime defaults to UTC Time in the begining
-- Generally, QueryMonitorQsState gets refreshed during restart and older values will persist if query store is disabled currently
 IF (object_id('tempdb..QueryMonitorQsState') IS NULL)  
BEGIN
	CREATE TABLE tempdb..QueryMonitorQsState(DatabaseID INT, DBName nvarchar(max),LastStartTime DATETIMEOFFSET)
END
--
-- [STEP 1]
-- Find out Databases where Query Store is Enabled
--

-- #QsEnabledTable - Stores Database and Actual State of the database
IF OBJECT_ID('tempdb..#QsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsEnabledTable
END
CREATE TABLE #QsEnabledTable
(
	DatabaseID INT,
	DBName NVARCHAR(MAX),
	ActualState SMALLINT  -- actual_state	smallint - Indicates the operation mode of Query Store 0 = OFF / 1 = READ_ONLY / 2 = READ_WRITE / 3 = ERROR
)

-- Get Databases where QS is enabled
DECLARE @QsEnabledCommand nvarchar(2000);
SET @QsEnabledCommand = 'USE [?]
IF {4}
BEGIN
  SELECT DB_ID(), DB_NAME(DB_ID()), actual_state 
  FROM sys.database_query_store_options
  WHERE actual_state = 1 OR actual_state = 2
END'

INSERT INTO #QsEnabledTable
EXEC sp_MSforeachdb @QsEnabledCommand

-- Create Temporary Table to store data
IF OBJECT_ID('tempdb..QueryStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..QueryStoreData
END
CREATE TABLE tempdb..QueryStoreData (
 StatementType CHAR(2),
 Duration BIGINT,
 DBName NVARCHAR(MAX),
 NTUserName NVARCHAR(MAX),
 HostName NVARCHAR(MAX),
 ApplicationName NVARCHAR(MAX),
 LoginName NVARCHAR(MAX),
 Reads BIGINT,
 Writes BIGINT,
 CPU BIGINT,
 TextData NVARCHAR(MAX),
 ObjectName NVARCHAR(MAX),
 SPID INT,
 StartTime DATETIMEOFFSET,
 QueryPlan NVARCHAR(MAX),
 DatabaseID INT,
 plan_id BIGINT
);

-- Create new clustered index on {0}
CREATE CLUSTERED INDEX IX_QueryStoreData_{0}
ON tempdb..QueryStoreData({0} DESC);

-- Return if no database got query store enabled
IF NOT EXISTS (SELECT * FROM #QsEnabledTable)
BEGIN

SELECT
 StatementType,
 Duration,
 DBName,
 NTUserName,
 HostName,
 ApplicationName,
 LoginName,
 Reads,
 Writes,
 CPU,
 TextData,
 ObjectName,
 SPID,
 StartTime,	-- Used to calculate completion time
 QueryPlan
FROM tempdb..QueryStoreData

-- Clear TempTables
IF OBJECT_ID('tempdb..#QsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsEnabledTable
END
IF OBJECT_ID('tempdb..QueryStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..QueryStoreData
END
-- Drop All data from QueryMonitorQsState
IF OBJECT_ID('tempdb..QueryMonitorQsState') IS NOT NULL
BEGIN
	DROP TABLE tempdb..QueryMonitorQsState
END
RETURN;
END

--
-- [STEP 2]
-- Read all query store data except Query Plan 
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
SELECT @LastStartTime = LastStartTime FROM tempdb..QueryMonitorQsState WHERE DatabaseID = @DatabaseID;

-- Set LastStartTime to UTC for newly added entries or null entries in tempdb..QueryMonitorQsState 
IF NOT EXISTS (SELECT DatabaseID FROM tempdb..QueryMonitorQsState WHERE DatabaseID = @DatabaseID)
BEGIN
 SELECT @LastStartTime = GETUTCDATE();
 INSERT INTO tempdb..QueryMonitorQsState (DatabaseID,DBName,LastStartTime)
  VALUES (@DatabaseID,@DBName, @LastStartTime)
END
ELSE IF @LastStartTime IS NULL
BEGIN 
 SELECT @LastStartTime = GETUTCDATE();
 UPDATE tempdb..QueryMonitorQsState
  SET LastStartTime = GETUTCDATE()
 WHERE DatabaseID = @DatabaseID;
END

;WITH qd AS (
SELECT
    so.type AS StatementType,
    qsrs.last_duration AS Duration,
    qsrs.last_logical_io_reads AS Reads,
    qsrs.last_logical_io_writes AS Writes,
    qsrs.last_cpu_time AS CPU,
    qsqt.query_sql_text AS TextData,
    so.name AS ObjectName,
    qsrs.last_execution_time AS StartTime,
    NULL AS QueryPlan,
    qsp.plan_id,  --  For merging with non top query data
    qsq.query_hash -- For merging with Sessions
FROM sys.query_store_query qsq
INNER JOIN sys.query_store_query_text qsqt ON qsqt.query_text_id = qsq.query_text_id
INNER JOIN sys.query_store_plan qsp ON qsp.query_id = qsq.query_id
INNER JOIN sys.query_store_runtime_stats qsrs ON qsrs.plan_id = qsp.plan_id
LEFT JOIN sys.objects so ON so.object_id = qsq.object_id
{1}
),
-- session data
sd as (SELECT
         des.nt_user_name AS NTUserName, 
         des.host_name AS HostName, 
         des.program_name AS ApplicationName, 
	     des.login_name AS LoginName, 
	     des.session_id AS SPID,
		 der.query_hash
       FROM sys.dm_exec_sessions des
       INNER JOIN sys.dm_exec_requests der on der.session_id = des.session_id
       {3}
)
SELECT
       qd.StatementType,
       qd.Duration,
       @DBName AS DBName,
       sd.NTUserName,
       sd.HostName,
       sd.ApplicationName,
       sd.LoginName,
       qd.Reads,
       qd.Writes,
       qd.CPU,
       qd.TextData,
       qd.ObjectName,
       sd.SPID,
       qd.StartTime,
       qd.QueryPlan,
       @DatabaseID AS DatabaseID,
	   qd.plan_id
FROM qd
left outer join sd on qd.query_hash = sd.query_hash
END'

-- Loop and read Query Data except Query Plan Information
DECLARE @sqlQsReadCommand NVARCHAR(MAX);
DECLARE @replace_character NCHAR(1) = N'?';
DECLARE QsDbCursor CURSOR 
LOCAL FORWARD_ONLY STATIC READ_ONLY
FOR SELECT DatabaseID, DBName
FROM #QsEnabledTable

OPEN QsDbCursor;

DECLARE @DBName NVARCHAR(300);
DECLARE @DatabaseID INT;
DECLARE @LastStartTime DATETIMEOFFSET;

-- Create Temporary Table to temporarily store query store data per database
IF OBJECT_ID('tempdb..TempQueryStoreData') IS NOT NULL
BEGIN
    DROP TABLE tempdb..TempQueryStoreData
END
CREATE TABLE tempdb..TempQueryStoreData(
StatementType CHAR(2),
Duration BIGINT,
DBName NVARCHAR(MAX),
NTUserName NVARCHAR(MAX),
HostName NVARCHAR(MAX),
ApplicationName NVARCHAR(MAX),
LoginName NVARCHAR(MAX),
Reads BIGINT,
Writes BIGINT,
CPU BIGINT,
TextData NVARCHAR(MAX),
ObjectName NVARCHAR(MAX),
SPID INT,
StartTime DATETIMEOFFSET,
QueryPlan NVARCHAR(MAX),
DatabaseID INT,
plan_id BIGINT
);
-- Create new clustered index on StartTime
CREATE CLUSTERED INDEX TempQueryStoreData_StartTime
ON tempdb..TempQueryStoreData(StartTime DESC);

FETCH NEXT FROM QsDbCursor into @DatabaseID,@DBName;
WHILE @@FETCH_STATUS = 0
BEGIN 
 -- Create dynamicSQL Query Store Read Command
 SET @sqlQsReadCommand = REPLACE(@QsReadCommand, @replace_character, @DBName);

 -- Insert into Temporary Query Store Data Table
 INSERT INTO tempdb..TempQueryStoreData( StatementType, Duration, DBName, NTUserName, HostName, ApplicationName, LoginName, 
                   Reads, Writes, CPU, TextData, ObjectName, SPID, StartTime, QueryPlan, DatabaseID, plan_id )
 EXEC sp_executesql @sqlQsReadCommand;
 
 -- Data Present
 IF EXISTS(SELECT DatabaseID FROM tempdb..TempQueryStoreData)
 BEGIN
  -- Insert data into main Query Store Data Table
  INSERT INTO tempdb..QueryStoreData ( StatementType, Duration, DBName, NTUserName, HostName, ApplicationName, LoginName, 
   Reads, Writes, CPU, TextData, ObjectName, SPID, StartTime, QueryPlan, DatabaseID, plan_id)
  SELECT StatementType, Duration, DBName, NTUserName, HostName, ApplicationName, LoginName, 
   Reads, Writes, CPU, TextData, ObjectName, SPID, StartTime, QueryPlan, DatabaseID, plan_id
  FROM tempdb..TempQueryStoreData

  -- Get Last Execution Time from newly added data from tempdb..TempQueryStoreData
  SELECT TOP (1) @LastStartTime = StartTime FROM tempdb..TempQueryStoreData ORDER BY StartTime DESC

  -- Check Not Null since maybe we don't have any data based on filters
  IF @LastStartTime IS NOT NULL
  BEGIN
   -- Store Last Execution Time to tempdb..QueryMonitorQsState 
   IF EXISTS (SELECT DatabaseID FROM tempdb..QueryMonitorQsState where DatabaseID = @DatabaseID)
   BEGIN
    UPDATE tempdb..QueryMonitorQsState SET LastStartTime = @LastStartTime
     WHERE DatabaseID = @DatabaseID
   END
   ELSE
   BEGIN
    INSERT INTO tempdb..QueryMonitorQsState (DatabaseID,DBName,LastStartTime)
     VALUES (@DatabaseID,@DBName, @LastStartTime)
   END
  END
 END
 TRUNCATE TABLE tempdb..TempQueryStoreData;

 FETCH NEXT FROM QsDbCursor INTO @DatabaseID,@DBName;
END
CLOSE QsDbCursor;
DEALLOCATE QsDbCursor;

-- Clear tempdb..TempQueryStoreData
IF OBJECT_ID('tempdb..TempQueryStoreData') IS NOT NULL
BEGIN
	DROP TABLE tempdb..TempQueryStoreData
END

-- Clear #QsEnabledTable
IF OBJECT_ID('tempdb..#QsEnabledTable') IS NOT NULL
BEGIN
	DROP TABLE #QsEnabledTable
END

--
-- [STEP 3]
-- Read Top X Query Plan data from Query Store
-- Note: For estimated plans return without query plan

-- Ensure Query Store Contains Data and collect actual plans
IF (EXISTS (SELECT * FROM tempdb..QueryStoreData)) AND 1 = {5}  -- set to collect actual plan
BEGIN
 -- Command to read Plan_Id and Query_Plan information
 DECLARE @QsReadQpCommand nvarchar(MAX);
 SET @QsReadQpCommand = '
 USE [?]
 SELECT plan_id, query_plan FROM sys.query_store_plan
 WHERE ';
 
 -- Table to store Distinct Plan Id Information per database
 IF OBJECT_ID('tempdb..#TopQueryPlanTable') IS NOT NULL
 BEGIN
 	DROP TABLE #TopQueryPlanTable
 END
 CREATE TABLE #TopQueryPlanTable(
 DatabaseID INT,
 DBName NVARCHAR(MAX),
 plan_id BIGINT
 );
 
 -- Insert Data into Top Query Plan Table
 WITH TopQueryStoreData AS 
 ( 
  SELECT TOP ({2}) DatabaseID, DBName, plan_id
   FROM tempdb..QueryStoreData
  ORDER BY {0} DESC
 )
 INSERT INTO #TopQueryPlanTable
 SELECT DISTINCT DatabaseID, DBName, plan_id
 FROM TopQueryStoreData
 
 -- Get all the plan_id in CSV form so that we fire only single query per database
 -- e.g. db1 contains 1 and 2 plan_id, they will be stored as single entry against db1 as 1,2
 
 -- Define Variables for Database and Plan Condition
 DECLARE @intMax INT = 2147483647;
 DECLARE @csv_op NVARCHAR(1)= ',';
 DECLARE @csv_opLength INT = 2 -- LEN(@csv_op) + 1
 
 -- Stores Query Store Plan Condition in Csv form Per databse
 IF OBJECT_ID('tempdb..#QsPlanConditionsPerDb') IS NOT NULL
 BEGIN
 	DROP TABLE #QsPlanConditionsPerDb
 END
 CREATE TABLE #QsPlanConditionsPerDb(
 DatabaseID INT,
 DBName NVARCHAR(MAX),
 PlanCondition NVARCHAR(MAX)
 );
 
 -- Select Database and Plan Condition to optimize single call per database for collecting all the plans as Csv Format
 INSERT INTO #QsPlanConditionsPerDb
 SELECT 
  DatabaseID,
  DBName,
  SUBSTRING((SELECT CONCAT(@csv_op, CAST(plan_id AS NVARCHAR))
    FROM #TopQueryPlanTable WHERE DatabaseID = a.DatabaseID ORDER BY DatabaseID FOR XML PATH('')), @csv_opLength, @intMax) AS PlanCondition
 FROM  
    (
    select DatabaseID, DBName, ROW_NUMBER() over (partition by DatabaseID order by DatabaseID) rownum
    from #TopQueryPlanTable) a
 WHERE a.rownum = 1
 
 -- Clear TopQueryPlanTable not used anymore
 IF OBJECT_ID('tempdb..#TopQueryPlanTable') IS NOT NULL
 BEGIN
 	DROP TABLE #TopQueryPlanTable
 END
 
 -- For Storing Query Plan against plan_id
 IF OBJECT_ID('tempdb..#QueryPlanStorage') IS NOT NULL
 BEGIN
 	DROP TABLE #QueryPlanStorage
 END
 CREATE TABLE #QueryPlanStorage(
 plan_id BIGINT,
 query_plan NVARCHAR(MAX)
 );
 
 -- For Storing PlanId and Query Plan from Cursor
 DECLARE @plan_id_cond NVARCHAR(MAX);
 DECLARE @queryPlan NVARCHAR(MAX);
 
 DECLARE QsQpCursor CURSOR LOCAL FAST_FORWARD FOR
  SELECT DatabaseID, DBName, PlanCondition 
   FROM #QsPlanConditionsPerDb
 
 OPEN QsQpCursor;
 
 FETCH NEXT FROM QsQpCursor INTO @DatabaseID, @DBName, @plan_id_cond;
 WHILE @@FETCH_STATUS = 0
 BEGIN
  -- Read Query Plan information per database
  SELECT @sqlQsReadCommand = CONCAT(REPLACE(@QsReadQpCommand, @replace_character, @DBName), ' plan_id IN(', @plan_id_cond, ')');

  INSERT INTO #QueryPlanStorage
   EXEC sp_executesql @sqlQsReadCommand;

  -- Update Top X Query Plan Data with collected plans
  WITH TopQueryStoreData AS 
  ( 
   SELECT TOP ({2}) plan_id, QueryPlan
   FROM tempdb..QueryStoreData
   WHERE DatabaseID = @DatabaseID
   ORDER BY {0} DESC
  )
  UPDATE tqsd
   SET tqsd.QueryPlan = qps.query_plan
     FROM TopQueryStoreData tqsd
    JOIN #QueryPlanStorage qps
     ON tqsd.plan_id = qps.plan_id
 
  -- Delete Data from Query Plan Storage to be used in next iteration
  TRUNCATE TABLE #QueryPlanStorage;
  
  FETCH NEXT FROM QsQpCursor INTO @DatabaseID,@DBName,@plan_id_cond;
 END
 CLOSE QsQpCursor;
 DEALLOCATE QsQpCursor;
 
 -- Drop Query Plan Storage
 IF OBJECT_ID('tempdb..#QueryPlanStorage') IS NOT NULL
 BEGIN
 	DROP TABLE #QueryPlanStorage
 END
 
 -- Drop Query Plan Conditions Per Db
 IF OBJECT_ID('tempdb..#QsPlanConditionsPerDb') IS NOT NULL
 BEGIN
 	DROP TABLE #QsPlanConditionsPerDb
 END
END

-- Fetch Query Store data
SELECT
 StatementType,
 Duration,
 DBName,
 NTUserName,
 HostName,
 ApplicationName,
 LoginName,
 Reads,
 Writes,
 CPU,
 TextData,
 ObjectName,
 SPID,
 StartTime,	-- Used to calculate completion time
 QueryPlan
FROM tempdb..QueryStoreData
ORDER BY {0} DESC

-- Drop QueryStoreData
IF OBJECT_ID('tempdb..QueryStoreData') IS NOT NULL
BEGIN
  DROP TABLE tempdb..QueryStoreData
END