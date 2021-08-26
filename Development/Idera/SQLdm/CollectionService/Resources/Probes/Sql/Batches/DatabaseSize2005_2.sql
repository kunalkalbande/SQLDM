----------------------------------------------------------------------------------------------
-- SQLDm 10.6 - Azure Database Size
--
--	
----------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------
---- QUERY: Database summary information
---- Returns:
---- Total number of databases
---- Number of data files
---- Number of log files
---- Total data file space allocated
---- Total log file space allocated
---- Total data file space used
----------------------------------------------------------------------------------
DECLARE @filelistNew TABLE (
	databasename NVARCHAR(520)
	,LogicalFilename SYSNAME
	,IsDataFile BIT
	,FilegroupName NVARCHAR(256)
	,CurrentSize DECIMAL(38, 5)
	,UsedSize DECIMAL(38, 5)
	,ConfiguredMaxSize INT
	,FreeSpaceOnDisk DECIMAL(38, 5)
	,FilePath NVARCHAR(520)
	,drivename NVARCHAR(520)
	);
DECLARE @sqlperf TABLE (
	databasename NVARCHAR(520)
	,logsize REAL
	,logspaceused REAL
	,[status] INT
	);
DECLARE @filestats TABLE (
	Fileid INT
	,FSFileGroup INT
	,TotalExtents INT
	,UsedExtents INT
	,FSName SYSNAME
	,FSFileName NCHAR(520)
	);
DECLARE @logexpansion DEC(38, 0)
	,@dbexpansion DEC(38, 0);

DECLARE @TempSysfiles TABLE (
	filename NVARCHAR(512)
	,fileid SMALLINT
	,name SYSNAME
	,STATUS INT
	,growth INT
	,maxsize INT
	,size INT
	,groupid SMALLINT
	)
DECLARE @Sysfiles TABLE (
	filename NVARCHAR(512)
	,fileid SMALLINT
	,name SYSNAME
	,STATUS INT
	,growth INT
	,maxsize INT
	,size INT
	,groupid SMALLINT
	)

INSERT INTO @sqlperf
SELECT DB_NAME() AS databasename
	,total_log_size_in_bytes*1.0/1024/1024 AS logsize 
	,used_log_space_in_percent AS logspaceused
	,0 AS [status]
FROM sys.dm_db_log_space_usage;

INSERT @filestats
SELECT f.fileid AS Fileid
	,f.groupid AS FileGroup
	,f.size / 8 AS TotalExtents
	,fileproperty(f.name, 'SpaceUsed') / 8 AS UsedExtents
	,f.name AS Name
	,f.filename AS FileName
FROM sys.sysfiles f
JOIN sys.database_files db_f ON f.fileid = db_f.file_id
JOIN sys.data_spaces ds ON ds.data_space_id = db_f.data_space_id

INSERT INTO @TempSysfiles
SELECT filename
	,fileid
	,name
	,STATUS
	,growth
	,maxsize
	,size
	,groupid
FROM sysfiles

INSERT INTO @Sysfiles
SELECT filename
	,fileid
	,name
	,STATUS
	,growth
	,maxsize
	,size
	,groupid
FROM @TempSysfiles s1

INSERT INTO @filelistNew
SELECT DB_NAME()
	,name
	,CASE 
		WHEN f.STATUS & 1000000 = 0
			THEN 1
		ELSE 0
		END
	,CASE 
		WHEN f.STATUS & 1000000 = 0
			THEN groupname
		ELSE 'n/a'
		END
	,TotalSize = CASE 
		WHEN f.STATUS & 1000000 = 0
			THEN (convert(DEC(38, 5), TotalExtents) * 64)
		ELSE CASE 
				WHEN (
						SELECT count(fileid)
						FROM sysfiles
						WHERE STATUS & 1000000 <> 0
						) > 1
					THEN convert(DEC(38, 5), size) * 8
				ELSE (
						SELECT max(convert(DEC(38, 5), logsize)) * 1024
						FROM @sqlperf
						WHERE rtrim(databasename) collate database_default = rtrim(db_name()) collate database_default
						)
				END
		END
	,UsedSize = CASE 
		WHEN f.STATUS & 1000000 = 0
			THEN (convert(DEC(38, 5), UsedExtents) * convert(DEC(38, 5), 64))
		ELSE (
				SELECT max(convert(DEC(38, 5), logsize) * (convert(DEC(38, 5), logspaceused)) / 100 * 1024)
				FROM @sqlperf
				)
		END
	,ConfiguredMaxSize = convert(DECIMAL(35, 5), round(maxsize / 128, 2))
	,diskfree = NULL
	,filename
	,drivename = NULL
FROM @Sysfiles f
LEFT JOIN sysfilegroups fg ON f.groupid = fg.groupid
LEFT JOIN @filestats fs ON f.fileid = fs.Fileid

;WITH SizeStats (
	Data_Size
	,TextImage_Size
	,Used_Pages
	)
AS (
	SELECT Data_Size = sum(CASE 
				WHEN it.internal_type IN (
						202
						,204
						)
					THEN 0
				WHEN p.index_id < 2
					THEN convert(DEC(38, 0), au.data_pages)
				ELSE 0
				END) * 8
		,TextImage_Size = sum(CASE 
				WHEN it.internal_type IN (
						202
						,204
						)
					THEN 0
				WHEN au.type != 1
					THEN convert(DEC(38, 0), au.used_pages)
				ELSE 0
				END) * 8
		,Used_Pages = sum(convert(DEC(38, 0), au.used_pages)) * 8
	FROM sys.partitions p
	LEFT JOIN sys.allocation_units au ON p.partition_id = au.container_id
	LEFT JOIN sys.internal_tables it ON p.object_id = it.object_id
	)
SELECT [Database Name] = d.name
	,[Database Status] = 0
	,[Allocated DBSize] = (
		SELECT isnull(sum(convert(DEC(38, 0), size) * 8), 0)
		FROM sysfiles(NOLOCK)
		WHERE STATUS & 1000000 = 0
		)
	,[Data Size] = convert(DEC(38, 0), Data_Size)
	,[Text Size] = convert(DEC(38, 0), TextImage_Size)
	,[Index Size] = convert(DEC(38, 0), (Used_Pages - Data_Size - TextImage_Size))
	,[System Database] = cast(CASE 
			WHEN d.name IN (
					'master'
					,'model'
					,'msdb'
					,'tempdb'
					)
				THEN cast(1 AS BIT)
			WHEN d.category & 16 = 16
				THEN cast(1 AS BIT)
			ELSE cast(0 AS BIT)
			END AS BIT)
	,[Log Expansion] = NULL
	,[Db Expansion] = NULL
	,[Total Used] = (
		SELECT sum(convert(DEC(38, 0), fileproperty(name, 'SpaceUsed')) * 8)
		FROM sysfiles
		WHERE STATUS & 1000000 = 0
		)
	,[crdate] = cast(crdate AS DATETIME)
	,[lastbackup] = logStats.log_backup_time
	,[isPrimary] = NULL
FROM SizeStats
CROSS JOIN sysdatabases d(NOLOCK)
CROSS APPLY sys.dm_db_log_stats(d.dbid) logStats
LEFT JOIN sys.databases db(NOLOCK) ON d.dbid = db.database_id
WHERE lower(d.name) <> 'mssqlsystemresource'
	AND d.name = DB_NAME()
ORDER BY user_access
	,STATUS

SELECT DatabaseName = DB_NAME()
	,NumberReads = cast(vf.NumberReads AS DEC(38, 0))
	,NumberWrites = cast(vf.NumberWrites AS DEC(38, 0))
	, NULL as driveletter
	,f.name
	,CASE 
		WHEN STATUS & 1000000 = 0
			THEN 1
		ELSE 0
		END AS filetypeflag
	,filename
	, NULL as driveletter
FROM sysfiles f
LEFT JOIN::fn_virtualfilestats(NULL, NULL) vf ON vf.FileId = f.fileid
WHERE vf.DbId = DB_ID()

select 'end of db scan'

SELECT databasename AS [Database Name]
	,logsize AS [Log Size (MB)]
	,logspaceused AS [Log Space Used (%)]
	, [status] AS [Status]
FROM @sqlperf;

SELECT 
		databasename AS DatabaseName,
		LogicalFilename AS [FileName],
		IsDataFile,
		FilegroupName,
		CurrentSize/1024 AS InitialSizeMB,
		UsedSize/1024 AS SpaceUsedMB,
		(CurrentSize-UsedSize)/1024 AS AvailableSpaceMB,
		ConfiguredMaxSize AS MaxSizeMB,
		FreeSpaceOnDisk/1024 AS FreeDiskSpaceMB,
		FilePath,
		drivename AS DriveName
FROM @filelistNew
