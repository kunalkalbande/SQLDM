/*
Alters to add new cols needed for this proc
alter table MetricMetaData add TableName nvarchar(256);
alter table MetricMetaData add ColumnName nvarchar(256);

Possible Statistics Tables (tables which have UTCCollectionDateTime column)
   ActiveWaitStatistics, AlwaysOnStatistics, BlockingSessionStatistics, Blocks, CustomCounterStatistics, 
   DatabaseFileActivity, DatabaseSize, DatabaseSizeDateTime, DatabaseStatistics, DatabaseStatistics_upgrade,   
   DeadlockProcesses, Deadlocks, DiskDrives, ESXConfigData, ESXStatistics, MirroringStatistics, OSStatistics, 
   QueryMonitor, QueryMonitorStatistics, SATempData, ServerActivity, ServerStatistics, stageDatabaseStatistics, 
   TableGrowth, TableReorganization, TempdbFileData, VMConfigData, VMStatistics, WaitStatistics
*/

if (object_id('p_GetMetricsHistoryForAlert') is not null)
begin
drop procedure p_GetMetricsHistoryForAlert 
end
go


CREATE PROCEDURE [dbo].[p_GetMetricsHistoryForAlert] (@AlertId INT, @NumHistoryHours INT = 4)
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @SQLServerID int 
	SELECT @SQLServerID= null;

	DECLARE @DatabaseID int 
	SELECT @DatabaseID= null;

	DECLARE @MetricId int 
	SELECT @MetricId= null;

	DECLARE @AlertOccurrenceTime DateTime 
	SELECT @AlertOccurrenceTime= null;

	DECLARE @TableName nvarchar(256) 
	SELECT @TableName= null;

	DECLARE @ColumnName nvarchar(256) 
	SELECT @ColumnName= null;

	-- Read metric from Alerts table.
	-- Read MetricMetaData to determine info where Metric is stored.
	SELECT 
		@SQLServerID = mss.SQLServerID,
		@DatabaseID = ssdn.DatabaseID,
		@MetricId = a.Metric, @AlertOccurrenceTime = a.UTCOccurrenceDateTime, 
		@TableName = mmd.TableName, @ColumnName = mmd.ColumnName  
	FROM Alerts a
	INNER JOIN MetricMetaData mmd ON a.Metric = mmd.Metric
	LEFT OUTER JOIN MonitoredSQLServers mss ON mss.InstanceName = a.ServerName
	LEFT OUTER JOIN SQLServerDatabaseNames ssdn ON ssdn.DatabaseName = a.DatabaseName  AND mss.SQLServerID = ssdn.SQLServerID
	WHERE a.AlertID = @AlertId;
	
	DECLARE @EndDateSQLString nvarchar(MAX);
	DECLARE @SQLString nvarchar(MAX);

	DECLARE @end DateTime 
	SELECT @end= @AlertOccurrenceTime;

	declare @start DateTime 
	select @start= DATEADD(HOUR, -@NumHistoryHours, @end);
	
	IF (@TableName IN ('ServerStatistics','OSStatistics','DiskDrives','VMStatistics','ESXStatistics'))
	BEGIN 			
		SELECT @SQLString = 'SELECT UTCCollectionDateTime, ' + @ColumnName + ' AS Value FROM '+ @TableName +
							' WHERE UTCCollectionDateTime BETWEEN @start and @end
							AND SQLServerID = @SQLServerID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @SQLServerID int', @start, @end, @SQLServerID
	END
	ELSE IF (@TableName ='Deadlocks')
	BEGIN 			
		SELECT @SQLString = 'SELECT UTCCollectionDateTime, ' + @ColumnName + ' AS Value FROM '+ @TableName +
							' WHERE UTCCollectionDateTime BETWEEN @start and @end
							AND SQLServerID = @SQLServerID
							GROUP BY SQLServerID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @SQLServerID int', @start, @end, @SQLServerID
	END
	ELSE IF (@TableName = 'TempdbFileData' )
	BEGIN
		SELECT @SQLString = 'SELECT  UTCCollectionDateTime,SUM('+ @ColumnName +') AS Value  FROM '+@TableName+
							' WHERE UTCCollectionDateTime BETWEEN @start and @end
							group by UTCCollectionDateTime
							ORDER BY UTCCollectionDateTime DESC'
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime', @start, @end
	END

	ELSE IF (@TableName IN ( 'DatabaseStatistics','DatabaseSize','MirroringStatistics'))
	BEGIN
		SELECT @SQLString = 'SELECT UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' WHERE UTCCollectionDateTime BETWEEN @start and @end
							AND DatabaseID = @DatabaseID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @DatabaseID int', @start, @end, @DatabaseID
END
	ELSE IF (@TableName = 'TableReorganization' )
	BEGIN
		SELECT @SQLString = 'SELECT UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' TR LEFT JOIN SQLServerTableNames SST ON SST.TableID=TR.TableID '+
							' WHERE TR.UTCCollectionDateTime BETWEEN @start and @end
							AND SST.DatabaseID = @DatabaseID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @DatabaseID int', @start, @end, @DatabaseID
	END
	
	ELSE IF (@TableName = 'AlwaysOnStatistics')
	BEGIN
		SELECT @SQLString = 'SELECT UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' WHERE UTCCollectionDateTime BETWEEN @start and @end
							AND DatabaseId = @DatabaseID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @DatabaseID int', @start, @end, @DatabaseID
	END

	ELSE IF (@TableName = 'MirroringParticipants')
	BEGIN
		SELECT @SQLString = 'SELECT dateadd(n,-330,last_updated) AS UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' WHERE dateadd(n,-330,last_updated) BETWEEN @start and @end
							AND DatabaseID = @DatabaseID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @DatabaseID int', @start, @end, @DatabaseID
	END

	ELSE IF (@TableName = 'MonitoredSQLServers')
	BEGIN
		SELECT @SQLString = 'SELECT dateadd(n,-330,LastDatabaseCollectionTime) AS UTCCollectionDateTime, ' + @ColumnName + '  AS Value FROM '+ @TableName +
							' WHERE dateadd(n,-330,LastDatabaseCollectionTime) BETWEEN @start and @end
							AND SQLServerID = @SQLServerID
							ORDER BY UTCCollectionDateTime DESC';
		EXEC sp_executesql @SQLString, N'@start datetime, @end datetime, @SQLServerID int', @start, @end, @SQLServerID
	END

	

	
END
 
