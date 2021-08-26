if (object_id('p_GetMetricsHistoryForCustomDashboard') is not null)
begin
drop procedure [p_GetMetricsHistoryForCustomDashboard]
end
go

create procedure [p_GetMetricsHistoryForCustomDashboard] 
@dashboardId bigint,        
@widgetId bigint,
@startTime DateTime,
@endTime DateTime
as
BEGIN

DECLARE @matchId INT
DECLARE @metricId INT

SELECT @matchId = 0, @metricId = -1

DECLARE @errorFound nvarchar(250)
declare @e int 
-- Table and Column 
DECLARE @TableName nvarchar(256) 
SELECT @TableName= null;

DECLARE @ColumnName nvarchar(256) 
SELECT @ColumnName= null;

--To make query for random table for metric values
DECLARE @SQLString nvarchar(MAX);

DECLARE @SQLStringCombined nvarchar(MAX);
BEGIN TRY
   -- Start A Transaction
  

-- Get match type for widget
SELECT @matchId=[MatchId], @metricId = [MetricID] FROM CustomDashboardWidgets  WITH(NOLOCK) WHERE [WidgetID] = @widgetId AND [DashboardID] = @dashboardId

if(@matchId != 0 OR @metricId >= 0)
BEGIN
SELECT	@TableName = mmd.TableName, @ColumnName = mmd.ColumnName  
	FROM MetricMetaData mmd WITH(NOLOCK) WHERE mmd.Metric = @metricId
	
SELECT @SQLString = '

--Create temp table to store list of serverId and instance names
DECLARE @serverDetails TABLE(
	SQLServerId INT NOT NULL,
	InstanceName NVARCHAR(256) NOT NULL
	)
IF(@matchId = 1 OR @matchId = 2)
BEGIN
	INSERT INTO @serverDetails (SQLServerId,
	InstanceName
	)
	SELECT 
		WS.SourceServerID,
		COALESCE(mss.[FriendlyServerName],mss.[InstanceName]) AS [InstanceName] --SQLdm 10.1 (Pulkit Puri): For adding Friendly server name
		FROM WidgetSourceMapping WS WITH(NOLOCK) 
		JOIN MonitoredSQLServers mss WITH(NOLOCK) ON WS.SourceServerID = mss.SQLServerID
		WHERE [WidgetID] = @widgetId
	
END
ELSE IF(@matchId = 3)
BEGIN
	--Insert record from table to get list of serverids and instance name
	INSERT INTO @serverDetails (SQLServerId,
	InstanceName
	)
	SELECT 
		ST.SQLServerId,
		mss.InstanceName
		FROM ServerTags ST WITH(NOLOCK) 
		JOIN MonitoredSQLServers mss WITH(NOLOCK) ON ST.SQLServerId = mss.SQLServerID
		WHERE ST.TagId IN (SELECT TagId FROM WidgetTagMapping WHERE [WidgetID] = @widgetId )
	
	--SELECT SQLServerId FROM ServerTags WHERE TagId IN (SELECT TagId FROM WidgetTagMapping WHERE [WidgetID] = 3 )
END
ELSE IF(@matchId = 4)
BEGIN
	--Insert record from table to get list of serverids and instance name
	INSERT INTO @serverDetails (SQLServerId,
	InstanceName
	)
	SELECT SQLServerID, InstanceName FROM MonitoredSQLServers WITH(NOLOCK) 
END
ELSE
BEGIN
	SET @errorFound = ''Invalid WidgetID and dashboardID.''
END
';


	IF (@TableName IN ('ServerStatistics','OSStatistics','DiskDrives','VMStatistics','ESXStatistics','BlockingSessionStatistics'))
	BEGIN 			
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + 
							' AS Value FROM '+ @TableName +
							' AS TN WITH(NOLOCK) JOIN @serverDetails SD ON  SD.SQLServerId = TN.SQLServerID
							WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
							ORDER BY UTCCollectionDateTime DESC';
		
	END
	
	
	ELSE IF (@TableName ='Deadlocks')
	BEGIN 			
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + ' AS Value FROM '+ @TableName +
							' AS TN WITH(NOLOCK) JOIN @serverDetails SD ON  SD.SQLServerId = TN.SQLServerID WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
							GROUP BY SD.SQLServerID, SD.InstanceName , UTCCollectionDateTime,'+@ColumnName+'
							ORDER BY UTCCollectionDateTime DESC';
		
	END
	ELSE IF (@TableName = 'TempdbFileData' )
	BEGIN
	
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName,  UTCCollectionDateTime,(CONVERT(DECIMAL(16,2),SUM('+ @ColumnName +')/1000.0)) AS Value  FROM '+@TableName+
							' AS TD WITH(NOLOCK) LEFT JOIN DatabaseFiles DF ON TD.FileID = DF.FileID
							JOIN SQLServerDatabaseNames ssdn WITH(NOLOCK)  ON ssdn.DatabaseID = DF.DatabaseID
							JOIN @serverDetails SD  ON  SD.SQLServerId =ssdn.SQLServerID 
							WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
							group by SD.SQLServerId, SD.InstanceName, UTCCollectionDateTime
							ORDER BY TD.UTCCollectionDateTime DESC'
							--PRINT @SQLStringCombined
		
	END

	ELSE IF (@TableName IN ('DatabaseSize','MirroringStatistics'))
	BEGIN
	
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' AS TD JOIN SQLServerDatabaseNames ssdn WITH(NOLOCK) ON ssdn.DatabaseID = TD.DatabaseID
							JOIN @serverDetails SD ON  SD.SQLServerId =ssdn.SQLServerID
							WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
							ORDER BY UTCCollectionDateTime DESC';
		
	END                    
	ELSE IF (@TableName = 'TableReorganization' )
	BEGIN
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' AS TR JOIN SQLServerTableNames SST WITH(NOLOCK) ON SST.TableID=TR.TableID '+
							'JOIN SQLServerDatabaseNames ssdn WITH(NOLOCK) ON ssdn.DatabaseID = SST.DatabaseID
							JOIN @serverDetails SD ON  SD.SQLServerId =ssdn.SQLServerID 
							 WHERE TR.UTCCollectionDateTime BETWEEN @startTime and @endTime
							ORDER BY UTCCollectionDateTime DESC';
							
	END
	ELSE IF (@TableName = 'AlwaysOnStatistics')
	BEGIN
		SELECT @SQLStringCombined = @SQLString + ' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							--' AS TD JOIN SQLServerDatabaseNames ssdn WITH(NOLOCK) ON ssdn.DatabaseID = TD.DatabaseId
							' AS TD
							JOIN @serverDetails SD  ON  SD.SQLServerId =TD.SQLServerID
							WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
							ORDER BY UTCCollectionDateTime DESC';
		
		
	END

	--ELSE IF (@TableName = 'MirroringParticipants')
	--BEGIN
	--	SELECT @SQLString = 'SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, dateadd(n,-330,last_updated) AS UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
	--						' WHERE dateadd(n,-330,last_updated) BETWEEN @startTime and @endTime
	--						AND DatabaseID = @DatabaseID
	--						ORDER BY UTCCollectionDateTime DESC';
	--	EXEC sp_executesql @SQLString, N'@startTime datetime, @endTime datetime, @DatabaseID int', @startTime, @endTime, @DatabaseID
	--END
	ELSE IF (@TableName = 'MonitoredSQLServers')
	BEGIN
		SELECT @SQLStringCombined = @SQLString + ' SELECT SQLServerID AS ServerId, InstanceName As InstanceName, dateadd(n,-330,LastDatabaseCollectionTime) AS UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' WITH(NOLOCK) WHERE dateadd(n,-330,LastDatabaseCollectionTime) BETWEEN @startTime and @endTime
							AND SQLServerID IN (SELECT SQLServerId FROM @serverDetails)
							ORDER BY UTCCollectionDateTime DESC';
		
	END

	-- For Custom Counter Metric Value
	ELSE IF (@TableName = 'CustomCounterStatistics')
	BEGIN
	SELECT @SQLStringCombined = @SQLString +' SELECT SD.SQLServerId AS ServerId, SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + 
						' AS Value FROM '+ @TableName +
						' AS TN WITH(NOLOCK) JOIN @serverDetails SD ON  SD.SQLServerId = TN.SQLServerID WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
						GROUP BY SD.SQLServerID, SD.InstanceName , UTCCollectionDateTime,'+@ColumnName+'
						ORDER BY UTCCollectionDateTime DESC';
	END

	--SQLdm 10.0 (Srishti Purohit) -- for Query Monitor Events counts
	ELSE IF (@TableName = 'QueryMonitorStatistics')
	BEGIN
	SELECT @SQLStringCombined = @SQLString +'SELECT SD.SQLServerId AS ServerId ,SD.InstanceName As InstanceName, UTCCollectionDateTime, ' + @ColumnName + 
						' AS Value FROM '+ @TableName +
						' AS TN WITH(NOLOCK) JOIN @serverDetails SD ON  SD.SQLServerId = TN.SQLServerID WHERE UTCCollectionDateTime BETWEEN @startTime and @endTime
						GROUP BY SD.SQLServerId,SD.InstanceName,UTCCollectionDateTime';
						
	END
	--Excute @SQLString for all type of tables
	EXEC sp_executesql @SQLStringCombined, N'@startTime datetime, @endTime datetime,@matchId INT,@widgetId bigint,@errorFound nvarchar(250) ', @startTime, @endTime,@matchId,@widgetId,@errorFound
END
ELSE
BEGIN
	SELECT 0 AS ServerId
END


END TRY
BEGIN CATCH
 SELECT @@ERROR AS ErrorCode;
END CATCH

	
END