-- //Ashu: Should consider creating following indexes
-- create index idx_alerts_data ON Alerts(DatabaseName, Active, AlertID)
-- Indexes on DatabaseStatistics and DatabaseSize tables will be assessed when we have data for those tables.
--p_GetDatabaseByInstance @ServerId = 6
IF (OBJECT_ID('p_GetDatabaseByInstance') IS NOT NULL)
BEGIN
  DROP PROC p_GetDatabaseByInstance
END
GO
CREATE PROCEDURE [dbo].[p_GetDatabaseByInstance]
    @ServerId INT,
	@IsDeletedDBs bit=0
AS
BEGIN

Create  table #tempDatabase
(
DatabaseName nvarchar(100),
UserTables int
)

DECLARE @SQL NVARCHAR(max)
 
SET @SQL = stuff((
            SELECT '
UNION
SELECT ' + quotename(name, '''') + ' collate database_default as Db_Name, t.Name collate database_default as Table_Name
FROM ' + quotename(name) + '.sys.tables t'
            FROM sys.databases d (NOLOCK) where state = 0
            ORDER BY name
            FOR XML PATH('')
                ,type
            ).value('.', 'nvarchar(max)'), 1, 8, '') 
			 
SET @SQL= 'Insert into #tempDatabase (DatabaseName,UserTables) Select Db_Name,Count(Table_Name) from ( ' +@SQL+ ' ) db group by Db_Name '

EXECUTE sp_executesql @SQL ;

WITH LatestDatabaseStats (DatabaseID, UTCCollectionDateTime)
AS 
(
	SELECT ds.DatabaseID, MAX(ds.UTCCollectionDateTime)
	FROM SQLServerDatabaseNames dbn (NOLOCK) 
	INNER JOIN DatabaseStatistics ds (NOLOCK) ON dbn.DatabaseID = ds.DatabaseID
	WHERE dbn.SQLServerID = @ServerId and dbn.IsDeleted=@IsDeletedDBs -- SQLdm Kit1 Barkha khatri
	GROUP BY ds.DatabaseID	
), 

LatestDatabaseSize (DatabaseID, UTCCollectionDateTime)
AS
(
	SELECT ds.DatabaseID, MAX(ds.UTCCollectionDateTime)
	FROM SQLServerDatabaseNames dbn  (NOLOCK) INNER JOIN DatabaseSize ds (NOLOCK) ON dbn.DatabaseID = ds.DatabaseID
	WHERE dbn.SQLServerID = @ServerId and dbn.IsDeleted=@IsDeletedDBs -- SQLdm Kit1 Barkha khatri
	GROUP BY ds.DatabaseID	
),
LatestDatabaseBackUpStats(DatabaseName,LastBackupDateTime)
AS
(
	SELECT database_name DatabaseName, MAX(backup_finish_date) BackUpFinishDate
	FROM msdb..backupset (NOLOCK) WHERE type <> 'F'
	GROUP BY database_name
)

SELECT 
DB.DatabaseID, 
DB.SQLServerID InstanceID, 
DB.DatabaseName, 
DB.SystemDatabase IsSystemDatabase,
DB.CreationDateTime, 
mss.Active IsInstanceEnabled, 
ISNULL(dstats.Transactions, 0) AS LatestTransactions, 
ISNULL(dstats.DatabaseStatus, 4) AS LatestDatabaseStatus,
ISNULL(dsize.DataFileSizeInKilobytes/1024, 0) AS DataFileSizeMb,
ISNULL(dsize.LogFileSizeInKilobytes/1024, 0) AS LogFileSizeMb,
ISNULL((dsize.DataFileSizeInKilobytes + dsize.LogFileSizeInKilobytes)/1024, 0) AS TotalFileSizeMb,
ISNULL(dsize.DataSizeInKilobytes/1024, 0) AS DataSizeMb,
ISNULL(dsize.LogSizeInKilobytes/1024, 0) AS LogSizeMb,
ISNULL((dsize.DataSizeInKilobytes + dsize.LogSizeInKilobytes)/1024, 0) AS TotalSizeMb,
--START SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters
ISNULL(dsize.IndexSizeInKilobytes/1024, 0) AS IndexSizeMb,
ISNULL(dsize.TextSizeInKilobytes/1024, 0) AS TextSizeMb,
--END SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters
dsize.UTCCollectionDateTime AS LatestSizeCollectionTime,
dstats.UTCCollectionDateTime AS LatestStatsCollectionTime,
(SELECT COUNT(0) FROM [DatabaseFiles] DF WHERE DF.DatabaseID = DB.DatabaseID AND DF.FileType = 0) DataFileCount,
(SELECT COUNT(0) FROM [DatabaseFiles] DF WHERE DF.DatabaseID = DB.DatabaseID AND DF.FileType = 1) LogFileCount,
--ISNULL(SY.recovery_model_desc,'') RecoveryModel,
DB_BK_UP.LastBackupDateTime,
ISNULL(db.UserTables , 0) UserTables
FROM SQLServerDatabaseNames DB (NOLOCK)
INNER JOIN MonitoredSQLServers mss (NOLOCK) 
	ON mss.SQLServerID = DB.SQLServerID
LEFT OUTER JOIN LatestDatabaseSize ldsize (NOLOCK) ON ldsize.DatabaseID = DB.DatabaseID
LEFT OUTER JOIN DatabaseSize dsize (NOLOCK) 
	ON ldsize.DatabaseID = dsize.DatabaseID 
	AND ldsize.UTCCollectionDateTime = dsize.UTCCollectionDateTime
LEFT OUTER JOIN LatestDatabaseStats ldstats (NOLOCK) 
	ON ldstats.DatabaseID = DB.DatabaseID
LEFT OUTER JOIN DatabaseStatistics dstats (NOLOCK) 
	ON ldstats.DatabaseID = dstats.DatabaseID 
	AND ldstats.UTCCollectionDateTime = dstats.UTCCollectionDateTime
LEFT OUTER JOIN LatestDatabaseBackUpStats DB_BK_UP (NOLOCK) 
	ON LOWER(DB_BK_UP.DatabaseName) COLLATE DATABASE_DEFAULT = LOWER(DB.DatabaseName) COLLATE DATABASE_DEFAULT
LEFT OUTER JOIN #tempDatabase db (NOLOCK) ON DB.DatabaseName COLLATE DATABASE_DEFAULT=db.DatabaseName COLLATE DATABASE_DEFAULT
WHERE DB.SQLServerID = @ServerId and DB.IsDeleted=@IsDeletedDBs -- SQLdm Kit1 Barkha khatri
AND DB.DatabaseName IS NOT NULL
GROUP BY DB.DatabaseID,DB.SQLServerID, DB.DatabaseName, DB.SystemDatabase, DB.CreationDateTime,
mss.Active, 
dstats.Transactions, dstats.DatabaseStatus,
dsize.DataFileSizeInKilobytes, dsize.LogFileSizeInKilobytes, 
dsize.DataSizeInKilobytes, dsize.LogSizeInKilobytes, 
dsize.TextSizeInKilobytes, dsize.IndexSizeInKilobytes,
dsize.UTCCollectionDateTime, dstats.UTCCollectionDateTime,
DB_BK_UP.LastBackupDateTime,
db.UserTables

drop table #tempDatabase

END
 

GO
