IF IS_SRVROLEMEMBER('sysadmin') = 1
BEGIN
  -- Batch  Read Activity Monitor Extended Events 2012
--  SQLdm 10.3 (Varun Chopra) Linux Support
  -- Added new batch for SQLdm 9.1 by Ankit Srivastava
  --Variables
  --  [0] - ReadQueryFilter
  
  declare @LastFileName nvarchar(max)
  declare @LastRecordCount bigint
  
  BEGIN TRY
  
  IF (object_id('tempdb..AMExtendedEvent') is not null)  
  BEGIN
  	DROP TABLE tempdb..AMExtendedEvent
  END
  
  IF (object_id('tempdb..AMExtendedEventsState') is not null)  
  BEGIN
  	SELECT @LastFileName = LastFileName, @LastRecordCount = LastRecordCount from tempdb..AMExtendedEventsState
  END
  
  select @LastFileName, @LastRecordCount
  
  declare @fileStart nvarchar(260)=null
  
-- SQLdm 10.3 (Varun Chopra) Linux Support using Path with forward slashes
SELECT 
    @path = LEFT(column_value, LEN(column_value)-CHARINDEX('/', REVERSE(column_value))),
    @fileStart = RIGHT(column_value, CHARINDEX('/', REVERSE(column_value))-1)
  FROM sys.dm_xe_sessions s
  JOIN sys.dm_xe_session_object_columns soc
      ON s.address = soc.event_session_address
  WHERE s.name = @session
    AND soc.column_name = 'filename'
    
    -- Getting data before extension .xel
    set @fileStart = substring(@fileStart, 0,len(@fileStart)- CHARINDEX('.', reverse(@fileStart))+1)
  
    IF OBJECT_ID('tempdb..AMExtendedEventsDirectoryTree') IS NOT NULL
        DROP TABLE tempdb..AMExtendedEventsDirectoryTree;
  
  CREATE TABLE tempdb..AMExtendedEventsDirectoryTree (
         id int IDENTITY(1,1)
        ,subdirectory nvarchar(512)
        ,depth int
        ,isfile bit);
  
  INSERT tempdb..AMExtendedEventsDirectoryTree (subdirectory,depth,isfile)
  EXEC sys.xp_dirtree @path,1,1;
  
  select subdirectory, FileNameInMilliseconds
  from (
  	SELECT subdirectory,
  	SUBSTRING(subdirectory, LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 2, 
  	LEN(subdirectory) - CHARINDEX('.', REVERSE(subdirectory)) - (LEN(subdirectory) - CHARINDEX('_', REVERSE(subdirectory)) + 1)) as FileNameInMilliseconds
  	FROM tempdb..AMExtendedEventsDirectoryTree
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
END
ELSE -- Read Extended Events data from the batch itself
BEGIN
    -- Batch  Read Activity Monitor Extended Events 2012
    -- Added new batch for SQLdm 9.1 by Ankit Srivastava
    --Variables
    --  [0] - ReadQueryFilter
    declare @prevFileName nvarchar(max)=null, @prevFileOffset bigint=null;
    
    --Create the temp table AMExtendedEvents if not exists for latest filename and offset
    IF (object_id('tempdb..AMExtendedEvents') is not null)  
    BEGIN
    	SELECT @prevFileName=XELFileName from  tempdb..AMExtendedEvents
    	SELECT @prevFileOffset=XELFileOffset from  tempdb..AMExtendedEvents
    END
    ELSE
    	
    BEGIN
    CREATE TABLE tempdb..AMExtendedEvents(XELFileName nvarchar(max),XELFileOffset bigint)
    END
    
    --Create a temp table if not exists for read data to be stored temporarily
    
    IF (object_id('tempdb..#AMReaderDataTable') IS null) 
    BEGIN
    	CREATE TABLE #AMReaderDataTable
    (
    	EventClass int,
    	Duration bigint,
    	DBName nvarchar(max),
    	CompletionTime datetime,
    	NTUserName nvarchar(max),
    	HostName nvarchar(max),
    	ApplicationName nvarchar(max),
    	LoginName varchar(max),
    	ObjectID int,
    	[TextData] nvarchar(max),
    	ObjectName nvarchar(max),
    	SPID int,
    	StartTime datetime,
    	DatabaseID int,
    	Fname nvarchar(max),
    	Foffset bigint
    )
    END
    ELSE
    BEGIN
    	DELETE from #AMReaderDataTable
    END
    
    -- Get the log file name and substitute * wildcard in
    SELECT 
        @path = LEFT(column_value, LEN(column_value)-CHARINDEX('.', REVERSE(column_value))) 
            + '.' 
            + RIGHT(column_value, CHARINDEX('.', REVERSE(column_value))-1)
    FROM sys.dm_xe_sessions s
    JOIN sys.dm_xe_session_object_columns soc
        ON s.address = soc.event_session_address
    WHERE s.name = @session
      AND soc.column_name = 'filename'
    
      -- Set the metadata filename if it is NULL to the log file name with xem extension
    	if(@path is not null)
    	begin
    	select @path=REPLACE(@path,'.xel','') +'*.xel';
    
    BEGIN TRY
      BEGIN TRY
      ;WITH raw_data(t,fname,foffset) AS
        (
      	Select  CAST(event_data AS XML),file_name,file_offset
      	FROM sys.fn_xe_file_target_read_file(@path, null, @prevFileName, @prevFileOffset)
        ),
        xml_data (ed,fname,foffset) AS
      (
        SELECT e.query('.') ,fname,foffset
        FROM raw_data 
        CROSS APPLY t.nodes('/event') AS x(e)
      )
      INSERT INTO #AMReaderDataTable
      SELECT			CASE WHEN event_name ='database_file_size_change' THEN (CASE WHEN FileType='0' THEN 92 ELSE  93 END)
      				ELSE Convert(int,(Select trace_event_id from sys.trace_xe_event_map where xe_event_name=event_name))
      				END as EventClass,
      				ISNULL(duration,taskTime) as [Duration],
      				ISNULL(DBName,DBName1) as DBName ,
      				QueryTimeStamp as CompletionTime, -- The stamp is already in UTC (	so no conversion needed)
      				NTUserName,
      				 HostName,
      				 ApplicationName = isnull(ApplicationName,''),
      				 LoginName,
      				 ISNULL(Convert(int,ObjectID,1),0),
      				 Case when (event_name='blocked_process_report') 
      					  then REPLACE((REPLACE((Convert(nvarchar(max),[blocked_process])),'<value>','')),'</value>','')
      					  when (event_name='xml_deadlock_report')
      					  then REPLACE((REPLACE((Convert(nvarchar(max),[xml_report])),'<value>','')),'</value>','')
      					  else
      					  [TextData]
      				 end as [TextData] ,
      				 Object_Name(Convert(int,ObjectID,1),Convert(int,ISNULL(DatabaseID,DatabaseID1))) as ObjectName,
      				 Convert(int,SPID) as SPID,
      				 dateadd(MS,[taskTime]/1000,dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime,
      				 Convert(int,ISNULL(DatabaseID,DatabaseID1)) DatabaseID,
      				 fname,
      				 foffset
      
      
      FROM
      (
        SELECT 
          ed.value('event[1]/@name','varchar(50)') AS event_name	
      	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
      	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
      	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS [ObjectID]
      	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
      	,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
      	,ed.value('(event/data[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName1
      	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
      	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
      	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
      	,ed.value('(event/data[@name="duration"]/value)[1]','bigint') as duration
      	,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as taskTime
      	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
      	,ed.value('(event/data[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID1
      	,ed.query('(event/data[@name="blocked_process"]/value)[1]') AS [blocked_process]
      	,ed.query('(event/data[@name="xml_report"]/value)[1]') AS [xml_report]
      	,ed.value('(event/action[@name="sql_text"]/value)[1]', 'varchar(max)') AS TextData
      	,ed.value('(event/data[@name="file_type"]/value)[1]', 'varchar(max)') AS FileType
      	,fname
      	,foffset
      	
        FROM xml_data  
      ) AS Queryresult 
      {0}
    
    END TRY
    BEGIN CATCH
      DELETE FROM tempdb..AMExtendedEvents -- deleting the old xel file name which doesn't exist anymore 
      SELECT @prevFileName =null;
      SELECT @prevFileOffset =null;

      ;WITH raw_data(t,fname,foffset) AS
      (
      	Select  CAST(event_data AS XML),file_name,file_offset
      	FROM sys.fn_xe_file_target_read_file(@path, null, @prevFileName, @prevFileOffset)
      ),
      xml_data (ed,fname,foffset) AS
      (
        SELECT e.query('.') ,fname,foffset
        FROM raw_data 
        CROSS APPLY t.nodes('/event') AS x(e)
      )
      INSERT INTO #AMReaderDataTable
      SELECT			CASE WHEN event_name ='database_file_size_change' THEN (CASE WHEN FileType='0' THEN 92 ELSE  93 END)
      				ELSE Convert(int,(Select trace_event_id from sys.trace_xe_event_map where xe_event_name=event_name))
      				END as EventClass,
      				ISNULL(duration,taskTime) as [Duration],
      				ISNULL(DBName,DBName1) as DBName ,
      				QueryTimeStamp as CompletionTime, -- The stamp is already in UTC (	so no conversion needed)
      				NTUserName,
      				 HostName,
      				 ApplicationName = isnull(ApplicationName,''),
      				 LoginName,
      				 ISNULL(Convert(int,ObjectID,1),0),
      				 Case when (event_name='blocked_process_report') 
      					  then REPLACE((REPLACE((Convert(nvarchar(max),[blocked_process])),'<value>','')),'</value>','')
      					  when (event_name='xml_deadlock_report')
      					  then REPLACE((REPLACE((Convert(nvarchar(max),[xml_report])),'<value>','')),'</value>','')
      					  else
      					  [TextData]
      				 end as [TextData] ,
      				 Object_Name(Convert(int,ObjectID,1),Convert(int,ISNULL(DatabaseID,DatabaseID1))) as ObjectName,
      				 Convert(int,SPID) as SPID,
      				 dateadd(MS,[taskTime]/1000,dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(QueryTimeStamp,0))) as StartTime,
      				 Convert(int,ISNULL(DatabaseID,DatabaseID1)) DatabaseID,
      				 fname,
      				 foffset
      
      
      FROM
      (
        SELECT 
          ed.value('event[1]/@name','varchar(50)') AS event_name	
      	,ed.value('event[1]/@timestamp','datetime') AS QueryTimeStamp
      	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName
      	,ed.value('(event/data[@name="object_id"]/value)[1]', 'nvarchar(max)') AS [ObjectID]
      	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName
      	,ed.value('(event/action[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName
      	,ed.value('(event/data[@name="database_name"]/value)[1]', 'nvarchar(max)') AS DBName1
      	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName
      	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName
      	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID
      	,ed.value('(event/data[@name="duration"]/value)[1]','bigint') as duration
      	,ed.value('(event/action[@name="task_time"]/value)[1]','bigint') as taskTime
      	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID
      	,ed.value('(event/data[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID1
      	,ed.query('(event/data[@name="blocked_process"]/value)[1]') AS [blocked_process]
      	,ed.query('(event/data[@name="xml_report"]/value)[1]') AS [xml_report]
      	,ed.value('(event/action[@name="sql_text"]/value)[1]', 'varchar(max)') AS TextData
      	,ed.value('(event/data[@name="file_type"]/value)[1]', 'varchar(max)') AS FileType
      	,fname
      	,foffset
      	
        FROM xml_data  
      ) AS Queryresult 
      {0}
	END CATCH
    --Insert the latest filename and offset ( delete only if there is a new row to be inserted)
    IF EXISTS(Select  Fname,Foffset from #AMReaderDataTable)
    BEGIN
    DELETE FROM tempdb..AMExtendedEvents
    INSERT INTO tempdb..AMExtendedEvents Select TOP 1 Fname,Foffset from #AMReaderDataTable order by CompletionTime desc
    END
    
    Select EventClass ,Duration ,DBName ,CompletionTime ,NTUserName ,HostName ,ApplicationName ,LoginName ,ObjectID ,[TextData] ,ObjectName ,SPID ,StartTime ,DatabaseID from     #AMReaderDataTable
    
    DROP TABLE #AMReaderDataTable
    -- SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario
    END TRY
    BEGIN CATCH
    	DELETE FROM tempdb..AMExtendedEvents -- deleting the old xel file name which doesn't exist anymore 
        --declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
        select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
    END CATCH
    end
END