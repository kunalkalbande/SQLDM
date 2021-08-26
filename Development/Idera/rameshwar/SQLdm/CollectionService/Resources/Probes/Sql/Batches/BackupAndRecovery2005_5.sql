----------------------------------------------------------------------------------------------
--
--	collect backup and recovery information for analysis.
--
--	Collection boot page information for the database.
--
declare @dbinfo table ([ParentObject] varchar(255), [Object] varchar(255), [Field] varchar(255), [Value] varchar(255));
declare @outputTable table (Name varchar(64), Value varchar(32));

insert into @dbinfo exec sp_executesql N'dbcc dbinfo with tableresults';

insert into @outputTable(Name, Value) select top 1 'dbi_crdate', convert(varchar,[Value],21) from @dbinfo where [Field] in ('dbi_crdate');
insert into @outputTable(Name, Value) select top 1 'dbi_dbccLastKnownGood', convert(varchar,[Value],21) from @dbinfo where [Field] in ('dbi_dbccLastKnownGood');
insert into @outputTable(Name, Value) select top 1 'dbid_suspectpages', database_id from msdb..suspect_pages where database_id = DB_ID() and event_type between 1 and 3;
insert into @outputTable(Name, Value) select 'is_read_only', is_read_only from sys.databases where database_id = db_id();
insert into @outputTable(Name, Value) select 'Recovery_Mode', CONVERT(VARCHAR(32),DATABASEPROPERTYEX(DB_NAME(), 'RECOVERY'))

select Name, Value from @outputTable; 

DECLARE @ReturnStartDateTime DATETIME
DECLARE @ReturnFileName NVARCHAR(260)
DECLARE @MediaSetId int
DECLARE @FileName NVARCHAR(260)
DECLARE @MirrorFile INT
DECLARE @DeviceType TINYINT
DECLARE @FileExistsResults TABLE ([ID] INT IDENTITY(1,1) NOT NULL, [FileExists] INT, [FileIsADirecectory] INT, [ParentDireectoryExists] INT, [Mirror] INT)

SELECT TOP 1 @MediaSetId = [media_set_id], @ReturnStartDateTime = backup_start_date
FROM [msdb]..[backupset]
WHERE ([database_name] = DB_NAME()) 
ORDER BY [backup_start_date] DESC;
	
SELECT [physical_device_name], [mirror], [device_type] INTO #Backupdevices 
FROM [msdb]..[backupmediafamily]
WHERE [media_set_id] = @MediaSetId

DECLARE file_cursor CURSOR FAST_FORWARD FOR
	SELECT [physical_device_name], [mirror], [device_type] FROM #Backupdevices

OPEN file_cursor
	FETCH NEXT FROM file_cursor INTO @FileName, @MirrorFile, @DeviceType
	IF @@FETCH_STATUS = 0
	BEGIN
		SET @ReturnFileName = @FileName
		IF (@DeviceType = 2) OR (@DeviceType = 102)
		BEGIN
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO @FileExistsResults ([FileExists], [FileIsADirecectory], [ParentDireectoryExists]) EXEC [master]..xp_fileexist @FileName
				UPDATE @FileExistsResults SET [Mirror] = @MirrorFile WHERE [ID] = SCOPE_IDENTITY()
				FETCH NEXT FROM file_cursor INTO @FileName, @MirrorFile, @DeviceType
			END 
		END
		ELSE INSERT INTO @FileExistsResults ([FileExists]) VALUES (1)
	END
	ELSE INSERT INTO @FileExistsResults ([FileExists]) VALUES (1)
CLOSE file_cursor
DEALLOCATE file_cursor
DROP TABLE #Backupdevices

SELECT @ReturnFileName AS 'FileName', @ReturnStartDateTime AS 'StartDateTime', MAX(MirrorSetsQuery.MirrorSetExists) AS 'FileExists'
FROM	(SELECT MIN([FileExists]) AS 'MirrorSetExists' 
		FROM @FileExistsResults 
		GROUP BY [Mirror]) 
		AS MirrorSetsQuery
			
select physical_name from sys.master_files where database_id = db_id();

IF UPPER(CAST(DATABASEPROPERTYEX(DB_NAME(), 'RECOVERY') AS CHAR(1))) <> 'S' 
BEGIN
	SELECT TOP 1 DATEDIFF(DAY, [backup_start_date], GETDATE()) AS 'DaysOld', [backup_start_date] AS 'BackupStartDate'
	FROM [msdb]..[backupset]
	WHERE (([type] = 'L') AND ([database_name] = DB_NAME()))
	ORDER BY [backup_start_date] DESC
END
ELSE
	SELECT -1 AS 'DaysOld', null AS 'BackupStartDate'