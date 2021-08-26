if (object_id('p_DatabaseStatisticsFromStaging') is not null)
begin
drop procedure [p_DatabaseStatisticsFromStaging]
end
go

create proc p_DatabaseStatisticsFromStaging(@sourceID uniqueidentifier) 
as
begin
	declare @dbSizeTime table (DatabaseID int, maxTime datetime)
	declare @dbNames table (ServerID int, DatabaseName nvarchar(255), SystemDatabase bit, CreationDateTime datetime)

	--get all new database names for this server
	insert @dbNames
	select dbs.ServerID, dbs.DatabaseName, dbs.SystemDatabase, dbs.DatabaseCreateDate
	from [dbo].[stageDatabaseStatistics] dbs with (nolock)
	where dbs.SourceID = @sourceID 
	and not exists (select 1 from SQLServerDatabaseNames names 
						where names.SQLServerID = dbs.ServerID 
						and names.DatabaseName = dbs.DatabaseName collate database_default)

	--get all databases that are staged but not in SQLServerDatabaseNames
	--insert the new ones into SQLServerDatabaseNames
	insert SQLServerDatabaseNames
			([SQLServerID]
		   ,[DatabaseName]
		   ,[SystemDatabase]
		   ,[CreationDateTime])
	select dbs.ServerID, dbs.DatabaseName, dbs.SystemDatabase, dbs.CreationDateTime
	from @dbNames dbs

	--SQLdm kit1 Barkha Khatri updating IsDeleted flag and time for new databases with names same as old one
	/*update [SQLServerDatabaseNames]
	set [CreationDateTime] = dbs.DatabaseCreateDate,[IsDeleted]=0
	from SQLServerDatabaseNames, stageDatabaseStatistics dbs
	Where  SQLServerDatabaseNames.SQLServerID = dbs.ServerID
		and SQLServerDatabaseNames.DatabaseName = dbs.DatabaseName and dbs.SourceID = @sourceID and IsDeleted=1*/
		
	--SQLdm kit1 Barkha Khatri updating IsDeleted flag to 1 for deleted databases	
	/*update [SQLServerDatabaseNames]
	 set [IsDeleted]=1
	 from SQLServerDatabaseNames names
	 INNER JOIN stageDatabaseStatistics dbs ON names.SQLServerID = dbs.ServerID
	 Where dbs.SourceID = @sourceID and IsDeleted=0 and NOT EXISTS 
	 (SELECT names.SQLServerID 
	 FROM stageDatabaseStatistics dbs
	 WHERE names.SQLServerID = dbs.ServerID and names.DatabaseName = dbs.DatabaseName )*/
	 
	 
		
		
		
	--get the most recent collection per database
	--insert @dbSizeTime
	--select DatabaseID, max(UTCCollectionDateTime)
	--from DatabaseSize with (nolock)
	--group by DatabaseID

	--insert set into database statistics
	INSERT INTO [dbo].[DatabaseStatistics]
			   ([DatabaseID]
			   ,[UTCCollectionDateTime]
			   ,[DatabaseStatus]
			   ,[Transactions]
			   ,[LogFlushWaits]
			   ,[LogFlushes]
			   ,[LogKilobytesFlushed]
			   ,[LogCacheReads]
			   ,[LogCacheHitRatio]
			   ,[TimeDeltaInSeconds]
			   ,[NumberReads]
			   ,[NumberWrites]
			   ,[BytesRead]
			   ,[BytesWritten]
			   ,[IoStallMS]
			   ,[DatabaseSizeTime]
			   ,[LastBackupDateTime] -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
			   ,[AverageDataIO] --SQLdm 11.0 (Azure Support)
               ,[AverageLogIO]
               ,[MaxWorker]
               ,[MaxSession]
               ,[DatabaseAverageMemoryUsage]
               ,[InMemoryStorageUsage] 
			   
			   ,[AvgCpuPercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,[AvgDataIoPercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,[AvgLogWritePercent] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,[DtuLimit] -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,[AzureCloudAllocatedMemory]
			   ,[AzureCloudUsedMemory]
			   ,[AzureCloudStorageLimit]
			   ,[ElasticPool]) --SQLdm 11.0 ElasticPool Support -Nikhil
	select names.DatabaseID, s.UTCCollectionDateTime, s.DatabaseStatus, s.[Transactions] ,s.[LogFlushWaits],s.[LogFlushes],s.[LogKilobytesFlushed]
			   ,s.[LogCacheReads]
			   ,s.[LogCacheHitRatio]
			   ,s.[TimeDeltaInSeconds]
			   ,s.[NumberReads]
			   ,s.[NumberWrites]
			   ,s.[BytesRead]
			   ,s.[BytesWritten]
			   ,s.[IoStallMS]
			   ,size.UTCCollectionDateTime
			   ,s.LastBackupDateTime -- SQLdm 10.0 (Vandana Gogna) - Database backup alerts
			   ,s.AverageDataIO--SQLdm 11.0 (Azure Support)
			   ,s.AverageLogIO
			   ,s.MaxWorker
			   ,s.MaxSession
			   ,s.DatabaseAverageMemoryUsage
			   ,s.InMemoryStorageUsage
			   ,s.AvgCpuPercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,s.AvgDataIoPercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,s.AvgLogWritePercent -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,s.DtuLimit -- SQLdm 11.0 (Rajat) - Azure Cpu Chart
			   ,s.AzureCloudAllocatedMemory
			   ,s.AzureCloudUsedMemory
			   ,s.AzureCloudStorageLimit
			   ,s.ElasticPool
	from [dbo].[stageDatabaseStatistics] s with (nolock)
	inner join SQLServerDatabaseNames names on s.ServerID = names.SQLServerID and s.DatabaseName = names.DatabaseName
	inner join DatabaseSizeDateTime size on size.DatabaseID = names.DatabaseID
	where s.SourceID = @sourceID
	and not exists(select 1 from [DatabaseStatistics] stat where stat.UTCCollectionDateTime = s.UTCCollectionDateTime and stat.DatabaseID = names.DatabaseID);
	--return the new databases with their new id's
	select db.DatabaseName, db.DatabaseID
	from stageDatabaseStatistics sdb with (nolock)
	inner join SQLServerDatabaseNames db on sdb.ServerID = db.SQLServerID and sdb.DatabaseName = db.DatabaseName collate database_default
	where sdb.SourceID = @sourceID

	--delete from staging for this process
	delete from [dbo].[stageDatabaseStatistics]
	where SourceID = @sourceID;

end