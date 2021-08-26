----------------------------------------------------------------------------------------------
--
--	collect backup and recovery information for analysis.
--
--	Collection boot page information for the database.
--
DECLARE @outputTable1 table (Name varchar(64), Value varchar(32));
DECLARE @outputTable2 table (FileName NVARCHAR(260), StartDateTime DATETIME, FileExists INT);
DECLARE @outputTable3 table (physical_name NVARCHAR(520), Value varchar(32));
DECLARE @outputTable4 table (DaysOld INT, BackupStartDate DATETIME);

SELECT [Name], [Value] FROM @outputTable1;
SELECT [FileName], [StartDateTime], [FileExists] FROM @outputTable2;
SELECT [physical_name] FROM @outputTable3;
SELECT [DaysOld], [BackupStartDate] FROM @outputTable4;
