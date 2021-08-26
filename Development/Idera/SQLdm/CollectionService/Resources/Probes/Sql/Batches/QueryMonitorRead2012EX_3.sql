
-- Query Monitor Read 2012
-- (0) Set to capture Actual Query Plan
-- (1) Cutoff Time in Seconds - 2/3 rd of Command / Collection Timeout in Seconds
-- (2) Set to collect top X queries at single go
-- Loops untill has time and collects X queries in each go and returns the result when timedout
-- Above parameters applicable for non sysadmin user only to allow read data from the batch
IF IS_SRVROLEMEMBER('sysadmin') = 1 
BEGIN
    -- SELECT 'administrator'
    -- Batch  Read Query Monitor Extended Events 2012
    --  SQLdm 10.3 (Varun Chopra) Linux Support
    -- Added new batch for SQLdm 9.0 by Ankit Srivastava
    --START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback --  Capture   the previous filename and offset 
    -- SQLdm 10.2 (Anshul Aggarwal) - Logic changed to support XEReader API
    
    declare @LastFileName nvarchar(max)
    declare @LastRecordCount bigint
    
    BEGIN TRY
    
    --Drop the old tempdb..QMExtendedEvents table used before XEReader API was introduced
    IF (object_id('tempdb..QMExtendedEvents') is not null)  
    BEGIN
    	DROP TABLE tempdb..QMExtendedEvents
    END
    
    IF (object_id('tempdb..QMExtendedEventsState') is not null)  
    BEGIN
    	SELECT @LastFileName = LastFileName, @LastRecordCount = LastRecordCount from     tempdb..QMExtendedEventsState
    END
    	
    select @LastFileName, @LastRecordCount
    
    declare @fileStart nvarchar(260)=null
    
    SELECT 
    @path = LEFT(column_value, LEN(column_value)-CHARINDEX('/', REVERSE(column_value))),
	@fileStart = RIGHT(column_value, CHARINDEX('/', REVERSE(column_value))-1)
    FROM sys.dm_xe_sessions s
    JOIN sys.dm_xe_session_object_columns soc
        ON s.address = soc.event_session_address
    WHERE s.name = @session
      AND soc.column_name = 'filename'
    
    IF OBJECT_ID('tempdb..QMExtendedEventsDirectoryTree') IS NOT NULL
          DROP TABLE tempdb..QMExtendedEventsDirectoryTree;
    
    CREATE TABLE tempdb..QMExtendedEventsDirectoryTree (
           id int IDENTITY(1,1)
          ,subdirectory nvarchar(512)
          ,depth int
          ,isfile bit);
    
    	INSERT tempdb..QMExtendedEventsDirectoryTree (subdirectory,depth,isfile)
    	EXEC sys.xp_dirtree @path,1,1;
    	
    select subdirectory, FileNameInMilliseconds
    from (
    	SELECT subdirectory,
    	SUBSTRING(subdirectory, LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 2, 
    	LEN(subdirectory) - CHARINDEX('.', REVERSE(subdirectory)) - (LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 1)) as FileNameInMilliseconds
    	FROM tempdb..QMExtendedEventsDirectoryTree
    	where subdirectory like @fileStart + '%.xel' 
    ) as fileData
    where FileNameInMilliseconds >= ISNULL(@LastFileName, 0)
    order by FileNameInMilliseconds asc
    
    END TRY
    BEGIN CATCH
        declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
        select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)),     @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging    it  as warning 
    END CATCH
END
ELSE
BEGIN
  DECLARE @startDate DATETIME;
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
  	[QueryPlan] nvarchar(max)
  )
  END
  ELSE
  BEGIN
  	DELETE from #QMReaderDataTable
  END

  SELECT @startDate = GETUTCDATE();
  -- Consider Command Timeout for completion of query collection
  WHILE DATEDIFF(SECOND, @startDate, GETUTCDATE()) < {1}
  BEGIN
    --SELECT 'not administrator'
    -- Read Query Monitor Extended Events 2008
    -- Added new batch for SQLdm 9.0 by Ankit Srivastava
    --Variables
    --  [0] - ReadQueryFilter
    -- Get the log file name and substitute * wildcard in
    
    --START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- Replaced Bookmark approach with Offset approach
    declare @prevFileName nvarchar(max)=null, @prevFileOffset bigint=null;
    
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
    
    IF (object_id('tempdb..#TempQMReaderDataTable') IS null) 
    BEGIN
    	CREATE TABLE #TempQMReaderDataTable
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
    	plan_handle nvarchar(max),
    	[CPU] bigint,
    	[TextData] nvarchar(max),
    	ObjectName nvarchar(max),
    	SPID int,
    	StartTime datetime,
    	DatabaseID int,
    	[QueryPlan] nvarchar(max),
    	Fname nvarchar(max),
    	Foffset bigint
    )
    END
    ELSE
    BEGIN
    	DELETE from #TempQMReaderDataTable
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
          SELECT TOP {2} e.query('.') ,fname,foffset
          FROM raw_data 
          CROSS APPLY t.nodes('/event') AS x(e)
        )
        INSERT INTO #TempQMReaderDataTable (EventClass, Duration, DBName, CompletionTime, NTUserName, HostName, ApplicationName, LoginName, [Reads], [Writes],
		                                plan_handle, [CPU], [TextData], ObjectName, SPID, StartTime, DatabaseID, [QueryPlan], Fname, Foffset)
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
						 plan_handle,
        				 [CPU],
        				 TextData,
        				 ISNULL(ObjectName,Object_Name(Convert(int,ObjctId,1),Convert(int,DatabaseID))) as ObjectName, -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of     column to     avoid ambiguity
        				 Convert(int,SPID) as SPID,
        				 dateadd(MS,-1*([Duration]/1000),dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime, --//SQLdm 9.0 modified the start     time     calculation
        				 Convert(int,DatabaseID) as DatabaseID,
        				 NULL as QueryPlan,
        				 fname,
        				 foffset
        				 
        FROM
        (
          SELECT
            ed.value('event[1]/@name','varchar(50)') AS event_name
        	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
            --,ed.value('(event/action[@name="server_principal_name"]/value)[1]', 'nvarchar(128)') as LoginName
        	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
        	,ed.value('(event/data[@name="duration"]/value)[1]', 'bigint')	as [Duration] --- SQLdm 9.0 (Ankit Srivastava) -- removed conditiond for user_settable event since its     not     required anymore
        	,ed.value('(event/data[@name="cpu_time"]/value)[1]', 'bigint') AS [CPU]
        	,ed.value('(event/data[@name="logical_reads"]/value)[1]', 'bigint') AS [Reads]
        	,ed.value('(event/data[@name="writes"]/value)[1]', 'bigint') AS [Writes]
        	,ed.value('(event/action[@name="plan_handle"]/value)[1]','nvarchar(max)') AS plan_handle
        	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
        	--,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
        	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
        	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
        	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS ObjctId -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to avoid ambiguity
        	,ed.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(max)') AS ObjectName
        	,ISNULL(ISNULL(ed.value('(event/data[@name="statement"]/value)[1]', 'varchar(max)'),
			              ed.value('(event/data[@name="batch_text"]/value)[1]', 'varchar(max)')), 
				    ed.value('(event/data[@name="user_info"]/value)[1]', 'varchar(max)')) AS TextData
        	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
        	--,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as tastTime
        	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
        	,fname
        	,foffset
        	
          FROM xml_data
		  WHERE ed.value('event[1]/@name','varchar(50)') <>'query_post_execution_showplan'
	
        ) AS Queryresult    
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
          SELECT TOP {2} e.query('.') ,fname,foffset
          FROM raw_data 
          CROSS APPLY t.nodes('/event') AS x(e)
        )
        INSERT INTO #TempQMReaderDataTable (EventClass, Duration, DBName, CompletionTime, NTUserName, HostName, ApplicationName, LoginName, [Reads], [Writes],
		                                plan_handle, [CPU], [TextData], ObjectName, SPID, StartTime, DatabaseID, [QueryPlan], Fname, Foffset)
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
					 plan_handle,
        				 [CPU],
        				 TextData,
        				 ISNULL(ObjectName,Object_Name(Convert(int,ObjctId,1),Convert(int,DatabaseID))) as ObjectName, -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of     column to     avoid ambiguity
        				 Convert(int,SPID) as SPID,
        				 dateadd(MS,-1*([Duration]/1000),dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime, --//SQLdm 9.0 modified the start     time     calculation
        				 Convert(int,DatabaseID) as DatabaseID,
        				 null as QueryPlan,
        				 fname,
        				 foffset
        				 
        FROM
        (
          SELECT
            ed.value('event[1]/@name','varchar(50)') AS event_name
        	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
            --,ed.value('(event/action[@name="server_principal_name"]/value)[1]', 'nvarchar(128)') as LoginName
        	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
        	,ed.value('(event/data[@name="duration"]/value)[1]', 'bigint')	as [Duration] --- SQLdm 9.0 (Ankit Srivastava) -- removed conditiond for user_settable event since its     not     required anymore
        	,ed.value('(event/data[@name="cpu_time"]/value)[1]', 'bigint') AS [CPU]
        	,ed.value('(event/data[@name="logical_reads"]/value)[1]', 'bigint') AS [Reads]
        	,ed.value('(event/data[@name="writes"]/value)[1]', 'bigint') AS [Writes]
        	,ed.value('(event/action[@name="plan_handle"]/value)[1]','nvarchar(max)') AS plan_handle
        	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
        	--,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
        	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
        	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
        	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS ObjctId -- SQLdm 9.0 (Ankit Srivastava) -- Changed the name of column to avoid ambiguity
        	,ed.value('(event/data[@name="object_name"]/value)[1]', 'nvarchar(max)') AS ObjectName
        	,ISNULL(ISNULL(ed.value('(event/data[@name="statement"]/value)[1]', 'varchar(max)'),
			              ed.value('(event/data[@name="batch_text"]/value)[1]', 'varchar(max)')), 
				    ed.value('(event/data[@name="user_info"]/value)[1]', 'varchar(max)')) AS TextData
        	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
        	--,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as tastTime
        	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
        	,fname
        	,foffset
        	
          FROM xml_data
		  WHERE ed.value('event[1]/@name','varchar(50)') <>'query_post_execution_showplan'
        
        ) AS Queryresult		
    END CATCH    
    --END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- added offset and file name into the result
    
    --START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback --Insert the latest filename and offset ( delete only if there is a new row to be     inserted)
    IF EXISTS(Select  Fname,Foffset from #TempQMReaderDataTable)
    BEGIN
    DELETE FROM tempdb..NewQMExtendedEvents
    INSERT INTO tempdb..NewQMExtendedEvents Select TOP 1 Fname,Foffset from #TempQMReaderDataTable order by CompletionTime desc
    END
    
	-- Populate Query Plan Information
	IF 1={0} AND EXISTS(SELECT * FROM #TempQMReaderDataTable)
	BEGIN
	    BEGIN TRY
	        -- Read actual plans
		    ;WITH raw_data(t,fname,foffset) AS
              (
            	Select CAST(event_data AS XML),file_name,file_offset
            	FROM sys.fn_xe_file_target_read_file(@path, @mdpath, @prevFileName, @prevFileOffset)
              ),
              xml_data (ed,fname,foffset) AS
            (
              SELECT TOP {2} e.query('.') ,fname,foffset
              FROM raw_data 
              CROSS APPLY t.nodes('/event') AS x(e)
            )
	             INSERT INTO #QMReaderDataTable (EventClass, Duration, DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],        [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan)
		     SELECT EventClass, Duration, DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],        [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlanResultSet.query_plan AS QueryPlan FROM #TempQMReaderDataTable qmrdt LEFT JOIN
		      (
		      SELECT qmrdt.plan_handle,
		      CONVERT(nvarchar(max), ed.query('(event/data[@name="showplan_xml"]/value)[last()]/*')) AS query_plan,
              row_number() over(partition by plan_handle order by ed.value('event[1]/@timestamp','datetime') desc) as rn
		      FROM #TempQMReaderDataTable qmrdt
		      LEFT JOIN xml_data ON 
		      qmrdt.plan_handle = ed.value('(event/action[@name="plan_handle"]/value)[1]','nvarchar(max)')
		      WHERE ed.value('event[1]/@name','varchar(50)') = 'query_post_execution_showplan' -- Use events other thatn query_post_execution_showplan
		      ) AS QueryPlanResultSet
		      On QueryPlanResultSet.plan_handle = qmrdt.plan_handle
		      WHERE rn = 1 OR rn IS NULL -- to ensure to avoid getting the duplicate plans
		END TRY
		BEGIN CATCH
		    -- To handle scenario with nesting > 128
		    -- Try Using Outer Apply now
		    INSERT INTO #QMReaderDataTable (EventClass, Duration, DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],        [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan)
		    -- Try Using Outer Apply now
		    SELECT EventClass, Duration, DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],  [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, query_plan AS QueryPlan FROM #TempQMReaderDataTable qmrdt
		    OUTER APPLY sys.dm_exec_text_query_plan(Convert(varbinary(64),((Convert(XML,(qmrdt.plan_handle))).value('plan[1]/@handle','nvarchar(max)')),1) , DEFAULT, DEFAULT) AS query_plan -- SQLDM 10.2.2 (Varun Chopra) SQLDM-27780 Query plan information cannot be viewedinthe dashboard
		END CATCH
	END
	ELSE
	BEGIN
	    INSERT INTO #QMReaderDataTable (EventClass, Duration, DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],        [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan)
	    Select EventClass ,Duration ,DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],    [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan from #TempQMReaderDataTable
    END

    IF (object_id('tempdb..#TempQMReaderDataTable') IS NOT NULL)
    BEGIN
	    DELETE from #TempQMReaderDataTable
    END
	-- SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario
    END TRY
    BEGIN CATCH
    	DELETE FROM tempdb..NewQMExtendedEvents -- deleting the old xel file name which doesn't exist anymore 
        --declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
        select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
    END CATCH
    END
    --END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback --Insert the latest filename and offset ( delete only if there is a new row to be inserted)
	
  END	  
  -- Select Final output
  SELECT EventClass, Duration,DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,[Reads],[Writes],[CPU],
  [TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID, QueryPlan 
   FROM #QMReaderDataTable

  IF (object_id('tempdb..#TempQMReaderDataTable') IS NOT NULL)
  BEGIN
    DROP TABLE #TempQMReaderDataTable
  END
  IF (object_id('tempdb..#QMReaderDataTable') IS NOT NULL)
  BEGIN
	DROP TABLE #QMReaderDataTable
  END
END