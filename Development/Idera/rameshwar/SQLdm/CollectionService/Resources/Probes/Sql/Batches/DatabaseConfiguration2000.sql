--------------------------------------------------------------------------------
--  Batch: Database Configuration 2005
--  Tables: master.sys.databases
--	Returns:
--	Database Name
--	Collation
--	IsAnsiNullDefault
--	IsAnsiNullsEnabled
--	IsAnsiPaddingEnabled
--	IsAnsiWarningsEnabled
--	IsArithmeticAbortEnabled 
--	IsAutoClose
--	IsAutoCreateStatistics
--	IsAutoShrink
--	IsAutoUpdateStatistics
--	IsCloseCursorsOnCommitEnabled
--	IsFulltextEnabled
--	IsInStandBy
--	IsLocalCursorsDefault
--	IsMergePublished
--	IsNullConcat
--	IsNumericRoundAbortEnabled
--	IsParameterizationForced
--	IsQuotedIdentifiersEnabled
--	IsPublished
--	IsRecursiveTriggersEnabled
--	IsSubscribed
--	IsSyncWithBackup
--	IsTornPageDetectionEnabled
--	Recovery
--	Status
--	Updateability
--	UserAccess
--	Version
--------------------------------------------------------------------------------
select
	name,
	Collation = (select databasepropertyex(name,'Collation')),
	IsAnsiNullDefault = (select databasepropertyex(name,'IsAnsiNullDefault')),
	IsAnsiNullsEnabled = (select databasepropertyex(name,'IsAnsiNullsEnabled')),
	IsAnsiPaddingEnabled = (select databasepropertyex(name,'IsAnsiPaddingEnabled')),
	IsAnsiWarningsEnabled = (select databasepropertyex(name,'IsAnsiWarningsEnabled')),
	IsArithmeticAbortEnabled  = (select databasepropertyex(name,'IsArithmeticAbortEnabled')),
	IsAutoClose = (select databasepropertyex(name,'IsAutoClose')),
	IsAutoCreateStatistics = (select databasepropertyex(name,'IsAutoCreateStatistics')),
	IsAutoShrink = (select databasepropertyex(name,'IsAutoShrink')),
	IsAutoUpdateStatistics = (select databasepropertyex(name,'IsAutoUpdateStatistics')),
	IsCloseCursorsOnCommitEnabled= (select databasepropertyex(name,'IsCloseCursorsOnCommitEnabled')),
	IsFulltextEnabled = (select databasepropertyex(name,'IsFulltextEnabled')), 
	IsInStandBy = (select databasepropertyex(name,'IsInStandBy')),
	IsLocalCursorsDefault = (select databasepropertyex(name,'IsLocalCursorsDefault')),
	IsMergePublished = (select databasepropertyex(name,'IsMergePublished')),
	IsNullConcat = (select databasepropertyex(name,'IsNullConcat')),
	IsNumericRoundAbortEnabled = (select databasepropertyex(name,'IsNumericRoundAbortEnabled')),
	IsParameterizationForced = (select databasepropertyex(name,'IsParameterizationForced')),
	IsQuotedIdentifiersEnabled = (select databasepropertyex(name,'IsQuotedIdentifiersEnabled')),
	IsPublished = (select databasepropertyex(name,'IsPublished')),
	IsRecursiveTriggersEnabled = (select databasepropertyex(name,'IsRecursiveTriggersEnabled')),
	IsSubscribed = (select databasepropertyex(name,'IsSubscribed')),
	IsSyncWithBackup = (select databasepropertyex(name,'IsSyncWithBackup')),
	IsTornPageDetectionEnabled = (select databasepropertyex(name,'IsTornPageDetectionEnabled')),
	Recovery = (select databasepropertyex(name,'Recovery')),
	Status = (select databasepropertyex(name,'Status')),
	Updateability = (select databasepropertyex(name,'Updateability')),
	UserAccess = (select databasepropertyex(name,'UserAccess')),
	Version = (select databasepropertyex(name,'Version')),
	cmptlevel,	
	is_db_chaining_on = case when status2 & 1024 = 1024 then cast(1 as bit) else cast(0 as bit) end
from 
	master..sysdatabases
where 
	1=1
	{0}