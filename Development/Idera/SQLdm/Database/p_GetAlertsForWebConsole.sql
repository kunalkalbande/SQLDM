if (object_id('p_GetAlertsForWebConsole') is not null)
begin
drop procedure p_GetAlertsForWebConsole
end
go
CREATE PROCEDURE [dbo].[p_GetAlertsForWebConsole](
    @StartDate datetime,
    @EndDate datetime,
    @InstanceId nvarchar(64),
    @Severity nvarchar(64),
    @Metric nvarchar(64),
    @Category nvarchar(64),
    @OrderBy nvarchar(256),
    @OrderType nvarchar(4),
    @Limit int ,
    @ActiveOnly bit = 1,
    @InstanceLogicalparam tinyint = 1,
    @MetricLogicalparam tinyint = 1,
    @SeverityLogicalparam tinyint = 1
)
AS
begin
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    declare @now datetime
    Select @now= DateAdd(second,10,GetUTCDate())
    declare @starting datetime 
    select @starting =coalesce(@StartDate, DATEADD(year, -10, GETUTCDATE()))
    
    declare @ending datetime
    select @ending= coalesce(@EndDate, @now)
    
    select @OrderType = coalesce(@OrderType, 'ASC')
    declare @e int
    DECLARE @SQLString nvarchar(MAX)
	DECLARE @SQLString1 nvarchar(MAX)
    
    --[START]SQLdm 10.0 (Gaurav Karwal): for implementing sub categories of resources and finding the previous alert severity
--  DECLARE @ResourceMappingList TABLE(Metric INT,Category NVARCHAR(128))
    DECLARE @CPUCategoryLiteral VARCHAR(10),@MemoryCategoryLiteral VARCHAR(10),@IOCategoryLiteral VARCHAR(10),@ResoucesCatLiteral VARCHAR(10);
    
    SELECT @CPUCategoryLiteral = 'CPU',@MemoryCategoryLiteral = 'Memory', @IOCategoryLiteral = 'IO',@ResoucesCatLiteral = 'Resources';
    
    
    
--  INSERT INTO @ResourceMappingList VALUES(0,@CPUCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(26,@CPUCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(27,@CPUCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(28,@CPUCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(29,@CPUCategoryLiteral);t
--  INSERT INTO @ResourceMappingList VALUES(30,@CPUCategoryLiteral);
    
    -- INSERT INTO @ResourceMappingList VALUES(25,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(30,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(31,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(62,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(63,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(74,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(76,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(81,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(82,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(83,@IOCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(84,@IOCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(85,@IOCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(86,@IOCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(87,@IOCategoryLiteral);
--  INSERT INTO @ResourceMappingList VALUES(87,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(111,@IOCategoryLiteral);
    
    -- INSERT INTO @ResourceMappingList VALUES(13,@IOCategoryLiteral);
    -- INSERT INTO @ResourceMappingList VALUES(24,@IOCategoryLiteral);
    
    --[END]SQLdm 10.0 (Gaurav Karwal): for implementing sub categories of resources and finding the previous alert severity
    --Changes for Limit not passed by user send all record not only top 100
    DECLARE @checkLimit nvarchar(100)
    SELECT @checkLimit = '';
    if(@Limit is NOT Null AND @Limit != 0) --SQLdm 8.6 -(Ankit Srivastava) - Fixed for automation testing -- Setting the limit variable to the default
        Select @checkLimit=' TOP ('+CAST(@Limit as nvarchar(20))+') ';
    ELSE
        SELECT @checkLimit = '';
    if(@ActiveOnly is null) --SQLdm 8.6 -(Ankit Srivastava) - Fixed for automation testing -- Setting the ActiveOnly variable to the default
        Select @ActiveOnly = 1 
    --Commenting Out the following Code as Ending is getting delayed by 1 day
/*  if(@ending <> @now) --SQLdm 8.6 -(Ankit Srivastava) - Fixed for automation testing -- Setting the time to 23:59:59 if not mentioned 
     begin
        declare @newEnd datetime;
        select @newEnd = CONVERT(datetime,@ending,104);
        if( @ending = @newEnd)
        select @ending=DATEADD(MILLISECOND,-2,(DATEADD(DAY,1,@ending)))
    end
*/
    create table #IntermediateTable (InstanceId int, 
                                    InstanceName nvarchar(255) collate database_default, 
                                     FriendlyServerName nvarchar(255) collate database_default,--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
                                     Metric int, 
                                     LastScheduledCollectionTime datetime, 
                                     LastDatabaseCollectionTime datetime, 
                                     LastAlertRefreshTime datetime,
                                     IsSnoozed bit,
                                     Category nvarchar(64) collate database_default,
                                     IsDBNetric bit,
                                     SubCategory nvarchar(64) collate database_default,
                                     IsMaintenanceMode bit)
    
    
    
    -- create filtered table of selected servers and metrics to include
    insert into #IntermediateTable
    select
            MS.SQLServerID, 
            MS.[InstanceName],
            MS.[FriendlyServerName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
            M.Metric, 
            MS.LastScheduledCollectionTime, 
            MS.LastDatabaseCollectionTime,
            MS.LastAlertRefreshTime,
            case when T.UTCSnoozeEnd > @now then 1 else 0 end,
            MI.Category,
            case
                when DBM.MetricID is null then
                    0
                else
                    1
            end,
            CASE WHEN (LOWER(MI.Category) = LOWER(@ResoucesCatLiteral) AND M.Metric IN (0,26,27,28,29,30)) THEN LOWER(@CPUCategoryLiteral) 
            ELSE 
            CASE WHEN (LOWER(MI.Category) = LOWER(@ResoucesCatLiteral) AND M.Metric IN (25,30,31,62,63,74,76,81,82,83,84,85,86,87,111) ) THEN LOWER(@IOCategoryLiteral) ELSE
                CASE WHEN (LOWER(MI.Category) = LOWER(@ResoucesCatLiteral) AND M.Metric IN (13,24)) THEN LOWER(@MemoryCategoryLiteral) 
                ELSE
                MI.Category
                END
            END
        
        END
        SubCategory,
        0
        FROM MetricMetaData M (NOLOCK)
        cross join MonitoredSQLServers MS (NOLOCK)
        join MetricInfo as MI on M.Metric = MI.Metric
        left outer join MetricThresholds T (NOLOCK) on
                MS.[SQLServerID] = T.[SQLServerID] and
                M.[Metric] = T.[Metric]     
        left outer join DBMetrics DBM (NOLOCK) on
                DBM.MetricID = M.[Metric]
            WHERE 
                MS.Active = 1 and M.Deleted = 0 and
                (@Metric is null or ((@MetricLogicalparam = 1 and M.[Metric] IN (SELECT Value FROM fn_Split(@Metric,',')))) OR (@MetricLogicalparam = 2 and M.[Metric] NOT IN (SELECT Value FROM fn_Split(@Metric,',')))) and
                (@Category is null or MI.Category = @Category) and
                (T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @now)
                and (@InstanceId is null or ((@InstanceLogicalparam = 1 and MS.[SQLServerID] IN (SELECT Value FROM fn_Split(@InstanceId,',')))) OR (@InstanceLogicalparam = 2 and MS.[SQLServerID] NOT IN (SELECT Value FROM fn_Split(@InstanceId,','))))
                --select * from #IntermediateTable where Metric = 52
    create index IDX_TEMP ON #IntermediateTable (InstanceName, Metric)
    --set rowcount @Limit
    
    --START SQldm srishti purohit --To provide previous severity
    
    CREATE TABLE #PreviousAlertSEVERITY (Severity TINYINT,
    Category NVARCHAR(128) collate database_default,SubCategory NVARCHAR(128) collate database_default)
    INSERT INTO #PreviousAlertSEVERITY 
    SELECT T.Severity, 
        T.Category, T.SubCategory
        FROM(SELECT Severity,M.Category,
        CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral) AND A.Metric IN ( 0, 26,27,28,29,30)) THEN LOWER(@CPUCategoryLiteral) 
            ELSE 
            CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral) AND A.Metric IN ( 25,30,31,62,63,74,76,81,82,83,84,85,86,87,111)) THEN LOWER(@IOCategoryLiteral) ELSE
                CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral) AND A.Metric IN (13, 24)) THEN LOWER(@MemoryCategoryLiteral) 
                ELSE
                M.Category
                END
            END
        
        END
        SubCategory, 
        ROW_NUMBER() 
            OVER(PARTITION BY 
                CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral) AND A.Metric IN (0,26,27,28,29,30)) THEN LOWER(@CPUCategoryLiteral) 
                ELSE 
                    CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral) AND A.Metric IN (25,30,31,62,63,74,76,81,82,83,84,85,86,87,111)) THEN LOWER(@IOCategoryLiteral) 
                    ELSE
                        CASE WHEN (LOWER(M.Category) = LOWER(@ResoucesCatLiteral)AND A.Metric IN ( 13,24)) THEN LOWER(@MemoryCategoryLiteral) 
                        ELSE
                        M.Category
                        END
                    END
                END
                ORDER BY A.UTCOccurrenceDateTime DESC)  RowNum 
        
        FROM Alerts A JOIN MetricInfo M ON A.Metric = M.Metric WHERE A.UTCOccurrenceDateTime < @starting) T WHERE T.RowNum = 1
    
    update #IntermediateTable
    set IsMaintenanceMode = 1
    from
    #IntermediateTable I inner join Alerts A (NOLOCK) on A.ServerName collate database_default = I.InstanceName collate database_default 
    where A.Metric = 48 and I.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
    if @ActiveOnly = 1 begin
        select @SQLString = 'SELECT '+@checkLimit+' A.[AlertID],A.[UTCOccurrenceDateTime],I.[InstanceId],COALESCE(I.[FriendlyServerName],I.[InstanceName]) AS ServerName,A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],ISNULL(PAD.[Severity],1) PreviousAlertSeverity,A.[StateEvent],A.[Value],A.[Heading],A.[Message]
                
                from #IntermediateTable I               
                    inner join Alerts A (nolock) on 
                        A.[ServerName] = I.InstanceName and
                        ((A.UTCOccurrenceDateTime = I.LastScheduledCollectionTime and I.IsDBNetric = 0) or (A.UTCOccurrenceDateTime = I.LastDatabaseCollectionTime and I.IsDBNetric = 1)) and
                        A.Metric = I.Metric 
                        LEFT OUTER JOIN #PreviousAlertSEVERITY PAD ON  I.SubCategory = PAD.SubCategory
                where
                    (I.IsMaintenanceMode = 0 or (I.IsMaintenanceMode = 1 and A.Metric = 48))
                    and I.IsSnoozed = 0
                    and (A.[UTCOccurrenceDateTime] between @starting and @ending)' -- --SQLdm 8.6 -(Ankit Srivastava) - Fixed for automation testing -- Setting the end and start date filters for ActiveOnly case too
    end else
        select @SQLString = 'SELECT '+@checkLimit+' A.[AlertID],A.[UTCOccurrenceDateTime],I.[InstanceId],COALESCE(I.[FriendlyServerName],I.[InstanceName]) AS ServerName,A.[DatabaseName],A.[TableName],
                case 
                    when A.Metric = 48 and I.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
                        then 1
                    when A.[Active] = 1 and 
                            A.[UTCOccurrenceDateTime] = I.[LastScheduledCollectionTime] and
                            I.IsDBNetric = 0 and
                            I.IsSnoozed = 0
                    then 1
                        when A.[Active] = 1 and 
                            A.[UTCOccurrenceDateTime] = I.[LastDatabaseCollectionTime] and
                            I.IsDBNetric = 1 and
                            I.IsSnoozed = 0
                    then 1                           
                    else 0 
                end AS Active,
                A.[Metric],A.[Severity],ISNULL(PAD.[Severity],1) PreviousAlertSeverity,A.[StateEvent],A.[Value],A.[Heading],A.[Message]
        FROM #IntermediateTable I 
            left join [Alerts] A (NOLOCK) 
                on I.InstanceName = A.ServerName
                    and I.Metric = A.Metric
                    LEFT OUTER JOIN #PreviousAlertSEVERITY PAD ON  I.SubCategory = PAD.SubCategory
        WHERE 
            (A.[UTCOccurrenceDateTime] between @starting and @ending)'

	SELECT @SQLString1 = (SELECT Value FROM fn_Split(@Severity,','))

    SELECT @SQLString = @SQLString + 
        case when @Severity is not null and @SeverityLogicalparam = 1 then ' and A.[Severity] in  (' + @SQLString1 +  ')' when @Severity is not null and @SeverityLogicalparam = 2 then ' and A.[Severity] not in  (' + @SQLString1 +  ')'   else ' ' end + 
        case when @OrderBy is not null then ' ORDER BY '+(CASE WHEN UPPER(@OrderBy) = 'CATEGORY' THEN +'I.'+ @OrderBy ELSE @OrderBy END )+' '+ @OrderType else ' ' end;
    if @ActiveOnly = 1
        EXEC sp_executesql @SQLString, N'@Limit int, @starting datetime, @ending datetime, @Severity nvarchar(64)', @Limit, @starting, @ending, @Severity --SQLdm 8.6 -(Ankit Srivastava) - Fixed for automation testing -- Added new parameter
    else 
        EXEC sp_executesql @SQLString, N'@Limit int, @starting datetime, @ending datetime, @Severity nvarchar(64)', @Limit, @starting, @ending, @Severity
    
    SELECT @e = @@error
    RETURN @e
END
-- set statistics time off
-- active
/**
select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null,null, 'Severity', 'desc', 1000, 1
select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null,null,  'DatabaseName', 'desc', 1000, 1
select GETDATE()                                         
exec p_GetAlertsForWebConsole null, null, null, null, null,null,  'UTCOccurrenceDateTime', 'desc', 1000, 1
select GETDATE()                                          
exec p_GetAlertsForWebConsole null, null, null, null, null,null,  'ServerName', 'desc', 1000, 1
select GETDATE()                                    
exec p_GetAlertsForWebConsole null, null, null, null, null,null,  'Category', 'desc', 1000, 1
select GETDATE()
*/
-- all
/**
select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null, 'Severity', 'desc', 100, 0
--select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null, 'DatabaseName', 'desc', 100, 0
--select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null, 'UTCOccurrenceDateTime', 'desc', 100, 0
--select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null, 'ServerName', 'desc', 100, 0
--select GETDATE()
exec p_GetAlertsForWebConsole null, null, null, null, null, 'Category', 'desc', 100, 0
select GETDATE()
*/
