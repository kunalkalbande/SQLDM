
-- Read Query Monitor Extended Events 2008
-- Added new batch for SQLdm 9.0 by Ankit Srivastava
--Variables
--  [0] - ReadQueryFilter
--  [1] - Top Plan Filter Count for query monitoring performance improvement
--  [2] - Top Plan Filter Category for query monitoring performance improvement
-- Get the log file name and substitute * wildcard in

--START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- Replaced Bookmark approach with Offset approach
declare @prevFileName nvarchar(max)=null, @prevFileOffset bigint=null;

declare @topPlanCategory int -- SQLdm 10.4 (Nikhil Bansal) : For Query Monitoring Performance Improvement
set @topPlanCategory = {2} -- SQLdm 10.4 (Nikhil Bansal) : For Query Monitoring Performance Improvement

--Create the temp table QMExtendedEvents if not exists for latest filename and offset
IF (object_id('tempdb..NewQMExtendedEvents') is not null)  
BEGIN
	SELECT @prevFileName=XELFileName from  tempdb..NewQMExtendedEvents
	SELECT @prevFileOffset=XELFileOffset from  tempdb..NewQMExtendedEvents
END
ELSE
	
BEGIN
CREATE TABLE tempdb..NewQMExtendedEvents(XELFileName nvarchar(max),XELFileOffset bigint)
END

IF (object_id('tempdb..#QMReaderDataTable') IS null) 
BEGIN
	CREATE TABLE #QMReaderDataTable
(
	EventClass int,
	Duration bigint,
	DBName nvarchar(max),
	CompletionTime datetime,
	NTUserName nvarchar(max),
	HostName nvarchar(max),
	ApplicationName nvarchar(max),
	LoginName varchar(max),
	[Reads] bigint,
	[Writes] bigint,
	[CPU] bigint,
	[TextData] nvarchar(max),
	ObjectName nvarchar(max),
	SPID int,
	StartTime datetime,
	DatabaseID int,
	PlanHandle varbinary(64),		--SQLDM 10.4 (Nikhil Bansal) - To store the Query Plan Handle
	[QueryPlan] nvarchar(max),
	Fname nvarchar(max),
	Foffset bigint
)
END
ELSE
BEGIN
	DELETE from #QMReaderDataTable
END

--Create a temp table if not exists for read data to be stored temporarily
--END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- Replaced Bookmark approach with Offset approach

SELECT 
    @path = LEFT(column_value, LEN(column_value)-CHARINDEX('.', REVERSE(column_value))) 
        + '.' 
        + RIGHT(column_value, CHARINDEX('.', REVERSE(column_value))-1)
FROM sys.dm_xe_sessions s
JOIN sys.dm_xe_session_object_columns soc
    ON s.address = soc.event_session_address
WHERE s.name = @session
  AND soc.column_name = 'filename'

  

  if( @path is not null)
  begin

  -- Set the metadata filename if it is NULL to the log file name with xem extension
SELECT @mdpath =substring(@path,0,len(@path) -3 ) + '*.xem'; -- SQLdm 9.0 (Ankit Srivastava) -- modification in trimmed length from 4 to 3 to get the exact name of the file
Select @path =substring(@path,0,len(@path) -3 ) +'*.xel'; -- SQLdm 9.0 (Ankit Srivastava) -- modification in trimmed length from 4 to 3 to get the exact name of the file

--START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- added offset and file name into the result
BEGIN TRY
    BEGIN TRY
    ;WITH raw_data(t,fname,foffset) AS
      (
    	Select  CAST(event_data AS XML),file_name,file_offset
    	FROM sys.fn_xe_file_target_read_file(@path, @mdpath, @prevFileName, @prevFileOffset)
      ),
      xml_data (ed,fname,foffset) AS
    (
      SELECT e.query('.') ,fname,foffset
      FROM raw_data 
      CROSS APPLY t.nodes('/event') AS x(e)
    )
    INSERT INTO #QMReaderDataTable
    SELECT Convert(int,(CASE
    			WHEN event_name='sql_statement_completed'
    			THEN 41
    			WHEN event_name='sp_statement_completed'
    			THEN 45
    			WHEN event_name='module_end'
    			THEN 43
    	    END
    		)) as EventClass,
    				[Duration],
    				db_name(DatabaseID) as DBName,
    				QueryTimeStamp as CompletionTime, --SQLdm 9.0 (	Ankit Srivastava) The stamp is already in UTC (	so no conversion needed)
    				 NTUserName,
    				 HostName,
    				 ApplicationName = isnull(ApplicationName,''),
    				 LoginName,
    				 [Reads],
    				 [Writes],
    				 [CPU],
    				 TextData,
    				 ISNULL(ObjectName,Object_Name(Convert(int,ObjctId,1),Convert(int,DatabaseID))) as ObjectName, -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to     avoid ambiguity
    				 Convert(int,SPID) as SPID,
    				 dateadd(MS,-1*([Duration]/1000),dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime, --//SQLdm 9.0 modified the start time     calculation
    				 Convert(int,DatabaseID) as DatabaseID,
				 -- Convert(nvarchar(max),query_plan) as QueryPlan,	// SQLdm 10.4 (Nikhil Bansal) - Here, Store Plan handle instead of the Plan itself
				 PlanHandle,
				 null AS QueryPlan,
    				 fname,
    				 foffset
    				 
    FROM
    (
      SELECT 
        ed.value('event[1]/@name','varchar(50)') AS event_name
    	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
        --,ed.value('(event/action[@name="server_principal_name"]/value)[1]', 'nvarchar(128)') as LoginName
    	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
    	,ed.value('(event/data[@name="duration"]/value)[1]', 'bigint')	as [Duration] --- SQLdm 9.0 (Ankit Srivastava) -- removed conditiond for user_settable event since its not     required anymore
    	,ed.value('(event/data[@name="cpu"]/value)[1]', 'bigint') AS [CPU]
    	,ed.value('(event/data[@name="reads"]/value)[1]', 'bigint') AS [Reads]
    	,ed.value('(event/data[@name="writes"]/value)[1]', 'bigint') AS [Writes]
	,Convert(varbinary(64),((Convert(XML,(ed.value('(event/action[@name="plan_handle"]/value)[1]','nvarchar(max)')))).value('plan[1]/@handle','nvarchar(max)')),1) AS PlanHandle
    	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
    	--,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
    	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
    	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
    	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS ObjctId -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to avoid ambiguity
    	,ed.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(max)') AS ObjectName
    	,ed.value('(event/action[@name="sql_text"]/value)[1]', 'varchar(max)') AS TextData
    	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
    	--,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as tastTime
    	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
    	,fname
    	,foffset
    	
      FROM xml_data
    ) AS Queryresult 
    {0}
  -- START - SQLdm 10.4 (Nikhil Bansal) : Query Monitor Performance Improvement
	ORDER BY
		CASE @topPlanCategory WHEN 0 THEN [Duration] END DESC,
		CASE @topPlanCategory WHEN 1 THEN [Reads] END DESC,
		CASE @topPlanCategory WHEN 2 THEN [CPU] END DESC,
		CASE @topPlanCategory WHEN 3 THEN [Writes] END DESC

  --Update the Query Plans for the Top X Queries
  UPDATE #QMReaderDataTable 
  SET QueryPlan = Convert(nvarchar(max),QP.query_plan)
		FROM #QMReaderDataTable AS Queries
			OUTER APPLY sys.dm_exec_text_query_plan(Queries.PlanHandle, DEFAULT, DEFAULT) AS QP	-- SQLDM 10.2.2 (Varun Chopra) SQLDM-27780 Query plan information cannot be viewed in the dashboard
			WHERE Queries.PlanHandle IN (SELECT 
												TOP {1} 
													#QMReaderDataTable.PlanHandle
												FROM #QMReaderDataTable
												)
	-- END - SQLdm 10.4 (Nikhil Bansal) : Query Monitor Performance Improvement 

END TRY
BEGIN CATCH
	DELETE FROM tempdb..NewQMExtendedEvents -- deleting the old xel file name which doesn't exist anymore 
	SELECT @prevFileName =null;
	SELECT @prevFileOffset =null;

	-- Consuming exception in an attempt to retry reading because of rollover limit
        ;WITH raw_data(t,fname,foffset) AS
      (
    	Select  CAST(event_data AS XML),file_name,file_offset
    	FROM sys.fn_xe_file_target_read_file(@path, @mdpath, @prevFileName, @prevFileOffset)
      ),
      xml_data (ed,fname,foffset) AS
    (
      SELECT e.query('.') ,fname,foffset
      FROM raw_data 
      CROSS APPLY t.nodes('/event') AS x(e)
    )
    INSERT INTO #QMReaderDataTable
    SELECT Convert(int,(CASE
    			WHEN event_name='sql_statement_completed'
    			THEN 41
    			WHEN event_name='sp_statement_completed'
    			THEN 45
    			WHEN event_name='module_end'
    			THEN 43
    	    END
    		)) as EventClass,
    				[Duration],
    				db_name(DatabaseID) as DBName,
    				QueryTimeStamp as CompletionTime, --SQLdm 9.0 (	Ankit Srivastava) The stamp is already in UTC (	so no conversion needed)
    				 NTUserName,
    				 HostName,
    				 ApplicationName = isnull(ApplicationName,''),
    				 LoginName,
    				 [Reads],
    				 [Writes],
    				 [CPU],
    				 TextData,
    				 ISNULL(ObjectName,Object_Name(Convert(int,ObjctId,1),Convert(int,DatabaseID))) as ObjectName, -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to     avoid ambiguity
    				 Convert(int,SPID) as SPID,
    				 dateadd(MS,-1*([Duration]/1000),dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime, --//SQLdm 9.0 modified the start time     calculation
    				 Convert(int,DatabaseID) as DatabaseID,
				 -- Convert(nvarchar(max),query_plan) as QueryPlan,	// SQLdm 10.4 (Nikhil Bansal) - Here, Store Plan handle instead of the Plan itself
				 PlanHandle,
				 null AS QueryPlan,
    				 fname,
    				 foffset
    				 
    FROM
    (
      SELECT 
        ed.value('event[1]/@name','varchar(50)') AS event_name
    	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
        --,ed.value('(event/action[@name="server_principal_name"]/value)[1]', 'nvarchar(128)') as LoginName
    	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
    	,ed.value('(event/data[@name="duration"]/value)[1]', 'bigint')	as [Duration] --- SQLdm 9.0 (Ankit Srivastava) -- removed conditiond for user_settable event since its not     required anymore
    	,ed.value('(event/data[@name="cpu"]/value)[1]', 'bigint') AS [CPU]
    	,ed.value('(event/data[@name="reads"]/value)[1]', 'bigint') AS [Reads]
    	,ed.value('(event/data[@name="writes"]/value)[1]', 'bigint') AS [Writes]
	,Convert(varbinary(64),((Convert(XML,(ed.value('(event/action[@name="plan_handle"]/value)[1]','nvarchar(max)')))).value('plan[1]/@handle','nvarchar(max)')),1) AS PlanHandle
    	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
    	--,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
    	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
    	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
    	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS ObjctId -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to avoid ambiguity
    	,ed.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(max)') AS ObjectName
    	,ed.value('(event/action[@name="sql_text"]/value)[1]', 'varchar(max)') AS TextData
    	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
    	--,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as tastTime
    	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
    	,fname
    	,foffset
    	
      FROM xml_data
    ) AS Queryresult 
      OUTER APPLY sys.dm_exec_text_query_plan(PlanHandle, DEFAULT, DEFAULT) -- SQLDM 10.2.2 (Varun Chopra) SQLDM-27780 Query plan information cannot be viewed in the dashboard
    {0}
  -- START - SQLdm 10.4 (Nikhil Bansal) : Query Monitor Performance Improvement
	ORDER BY
		CASE @topPlanCategory WHEN 0 THEN [Duration] END DESC,
		CASE @topPlanCategory WHEN 1 THEN [Reads] END DESC,
		CASE @topPlanCategory WHEN 2 THEN [CPU] END DESC,
		CASE @topPlanCategory WHEN 3 THEN [Writes] END DESC

  --Update the Query Plans for the Top X Queries
  UPDATE #QMReaderDataTable 
  SET QueryPlan = Convert(nvarchar(max),QP.query_plan)
		FROM #QMReaderDataTable AS Queries
			OUTER APPLY sys.dm_exec_text_query_plan(Queries.PlanHandle, DEFAULT, DEFAULT) AS QP	-- SQLDM 10.2.2 (Varun Chopra) SQLDM-27780 Query plan information cannot be viewed in the dashboard
			WHERE Queries.PlanHandle IN (SELECT 
												TOP {1} 
													#QMReaderDataTable.PlanHandle
												FROM #QMReaderDataTable
												)
	-- END - SQLdm 10.4 (Nikhil Bansal) : Query Monitor Performance Improvement 


END CATCH    
--END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- added offset and file name into the result

--START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback --Insert the latest filename and offset ( delete only if there is a new row to be inserted)
IF EXISTS(Select  Fname,Foffset from #QMReaderDataTable)
BEGIN
DELETE FROM tempdb..NewQMExtendedEvents
INSERT INTO tempdb..NewQMExtendedEvents Select TOP 1 Fname,Foffset from #QMReaderDataTable order by CompletionTime desc
END

Select EventClass ,Duration ,DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU] ,[TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan from #QMReaderDataTable

DROP TABLE #QMReaderDataTable
-- SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario
END TRY
BEGIN CATCH
	DELETE FROM tempdb..NewQMExtendedEvents -- deleting the old xel file name which doesn't exist anymore 
    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH
end
--END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback --Insert the latest filename and offset ( delete only if there is a new row to be inserted)
