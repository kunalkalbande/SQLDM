IF (object_id('p_PostInstallUpgrade') IS NOT NULL)
BEGIN
DROP PROCEDURE p_PostInstallUpgrade
END
GO

CREATE PROCEDURE [dbo].[p_PostInstallUpgrade]
AS
BEGIN
	set ansi_warnings on
	if (select isnull(count([Internal_Value]),0) from [dbo].[RepositoryInfo] where [Name] = 'PostInstallUpgradeTo7.2') = 0
	begin
		declare @SQLVersion int
		select @SQLVersion = Internal_Value from RepositoryInfo where Name = 'SQLVersionAtUpgrade'

		if @SQLVersion < 10
		begin
			update DefaultMetricThresholds set Data = null where Data = ''
			update MetricThresholds set Data = null where Data = ''
			update MonitoredSQLServers set QueryMonitorAdvancedConfiguration = null where QueryMonitorAdvancedConfiguration = ''
			update CustomCounterDefinition set Batch = null where Batch = ''
			update MonitoredSQLServers set DiskCollectionSettings = null where DiskCollectionSettings = ''
			update MonitoredSQLServers set ActiveWaitAdvancedConfiguration = null where ActiveWaitAdvancedConfiguration = ''
			update MonitoredSQLServers set TableStatisticsExcludedDatabases = null where TableStatisticsExcludedDatabases = ''
			update MetricInfo set Comments = null where Comments = ''
			
			insert into [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) values ('PostInstallUpgradeTo7.2', 1, Convert(nvarchar(30), GetDate(), 121))
		end
	end
	
	set ansi_warnings on
	--if (select isnull(count([Internal_Value]),0) from [dbo].[RepositoryInfo] where [Name] = 'PostInstallUpgradeTo7.5') = 0
	--begin
	--		update DiskDrives set DatabaseSizeTime = UTCCollectionDateTime where DatabaseSizeTime is null
			
	--		insert into [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) values ('PostInstallUpgradeTo7.5', 1, Convert(nvarchar(30), GetDate(), 121))
	--end
	if not exists (select name from sysindexes where name = 'IXAlerts2')
	begin
		print 'Creating IXAlerts2'
		create nonclustered index [IXAlerts2] ON [Alerts]([ServerName],[UTCOccurrenceDateTime] DESC,[Active])
	end
	
	if not exists (select name from sysindexes where name = 'IXAlerts4')
	begin
		-- IXAlerts4 added to improve selection of active alerts and alert summary
		print 'Creating IXAlerts4'
		create nonclustered index [IXAlerts4] ON [Alerts]([UTCOccurrenceDateTime] DESC,[ServerName],[Severity])
	end
	
	if not exists (select name from sysindexes where name = 'IXAlerts5')
	begin
		print 'Creating IXAlerts5'
		create nonclustered index [IXAlerts5] ON [Alerts] ([ServerName], [UTCOccurrenceDateTime]) INCLUDE ([AlertID]) ON [PRIMARY]
	end

	if exists (select name from sysindexes where name = 'IXAlerts3')
	begin
		-- 6.0.1 - get rid of useless index
		print 'Dropping IXAlerts3'
		drop index [Alerts].[IXAlerts3]
	end


	if not exists (select name from sys.indexes where name = 'IX_Alerts_ViewSupport')
	begin
		create index IX_Alerts_ViewSupport on Alerts(ServerName, Metric, UTCOccurrenceDateTime, AlertID) include (DatabaseName,TableName,Severity, Active, StateEvent,Value,Heading,Message) 
	end

	--if not exists (select name from sys.indexes where name = 'IX_Alerts_ViewSupport_Operational')
	--begin
	--	create index IX_Alerts_ViewSupport_Operational on Alerts([ServerName], [UTCOccurrenceDateTime])  include (Metric,DatabaseName,TableName,Severity, Active, StateEvent,Value,Heading,Message) 
	--end

	
	if not exists (select name from sys.indexes where name = 'IX_Alerts_PredictiveTrainingIndex')
	begin
		create index IX_Alerts_PredictiveTrainingIndex on Alerts([ServerName],[Metric],[Severity],[UTCOccurrenceDateTime]) include ([StateEvent])
	end
	


	if not exists (select name from sysindexes where name = 'IXTasks')
	begin
		print 'Creating IXTasks'
		create nonclustered index [IXTasks] ON [Tasks]([ServerName],[CreatedOn],[Status],[Severity])
	end

	-- create index IXServerStatistics if it doesn't already exist
	IF (not exists(select id from sysindexes where name = 'IXServerStatistics' collate database_default)) 
	begin
		-- 6.0 new index - adding here rather than in the install script
		print 'Creating IXServerStatistics'
		create nonclustered index [IXServerStatistics] on [ServerStatistics]([SQLServerID],[UTCCollectionDateTime]) 
	end
	
--	-- drop index IXQueryMonitorSQLServerID (clustered) 
--	if (exists(select id from sysindexes where name = 'IXQueryMonitorSQLServerID' collate database_default)) 
--	begin
--		print 'Dropping IXQueryMonitorSQLServerID'
--		drop index QueryMonitor.IXQueryMonitorSQLServerID
--	end
--	
--	-- drop index IXQueryMonitorCompletionTime
--	IF (exists(select id from sysindexes where name = 'IXQueryMonitorCompletionTime' collate database_default)) 
--	begin
--		print 'Dropping IXQueryMonitorCompletionTime'
--		drop index QueryMonitor.IXQueryMonitorCompletionTime
--	end
--	
--	-- add clustered index IXQueryMonitorCompletionTime2
--	if (not exists(select id from sysindexes where name = 'IXQueryMonitorCompletionTime2' collate database_default)) 
--	begin
--		-- 6.0 replace index - adding here rather than in the install script
--		print 'Creating IXQueryMonitorCompletionTime2'
--		create clustered index [IXQueryMonitorCompletionTime2] ON [QueryMonitor]([SQLServerID],[CompletionTime]) 
--	end
--
--	if (not exists(select id from sysindexes where name = 'IXQueryMonitorSnapshot' collate database_default)) 
--	begin
--		-- 6.0 new index - adding here rather than in the install script
--		print 'Creating IXQueryMonitorSnapshot'
--		create nonclustered index [IXQueryMonitorSnapshot] on [QueryMonitor]([SQLServerID],[UTCCollectionDateTime]) 
--	end

	if (not exists(select id from sysindexes where name = 'IXTableGrowth' collate database_default)) 
	begin
		-- 6.0 new index - adding here rather than in the install script
		print 'Creating IXTableGrowth'
		create nonclustered index [IXTableGrowth] on [TableGrowth]([TableID],[UTCCollectionDateTime]) 
	end

	if (not exists(select id from sysindexes where name = 'IXTableReorganization' collate database_default)) 
	begin
		-- 6.0 new index - adding here rather than in the install script
		print 'Creating IXTableReorganization'
		create nonclustered index [IXTableReorganization] on [TableReorganization]([TableID],[UTCCollectionDateTime]) 
	end

	--START--Srishti--4.1.5 Index Suitability Fixes--Duplicate Index
	--if (not exists(select id from sysindexes where name = 'IXSQLServerDatabaseNames2' collate database_default)) 
	--begin
	--	-- 6.0 new index - adding here rather than in the install script
	--	print 'Creating IXSQLServerDatabaseNames2'
	--	create nonclustered index [IXSQLServerDatabaseNames2] on [SQLServerDatabaseNames]([SQLServerID],[DatabaseID]) 
	--end
	--END--Srishti--4.1.5 Index Suitability Fixes--Duplicate Index
	if (not exists(select id from sysindexes where name = 'IXSQLServerTableNames2' collate database_default)) 
	begin
		-- 6.0 new index - adding here rather than in the install script
		print 'Creating IXSQLServerTableNames2'
		create nonclustered index [IXSQLServerTableNames2] on [SQLServerTableNames]([DatabaseID],[TableID]) 
	end

	if (not exists(select id from sysindexes where name = 'IXSQLServerTableNames1' collate database_default)) 
	begin
		-- 6.1 new index 
		print 'Creating IXSQLServerTableNames1'
		CREATE INDEX [IXSQLServerTableNames1] ON [SQLServerTableNames]([DatabaseID],[TableName])
	end

	if (exists(select id from sysindexes where name = 'IXSQLServerTableNames' collate database_default)) 
	begin
		-- 6.1 drop index IXSQLServerTableNames 
		print 'Dropping IXSQLServerTableNames'
		drop index SQLServerTableNames.IXSQLServerTableNames
	end


	if (not exists(select id from sysindexes where name = 'IXQueryMonitorStatisticsSQLServerID' collate database_default)) 
	begin
		CREATE INDEX [IXQueryMonitorStatisticsSQLServerID] ON [QueryMonitorStatistics] ([SQLServerID],[DurationMilliseconds], [StatementType]) 
		INCLUDE ([StatementUTCStartTime], [CPUMilliseconds], [Reads], [Writes], [HostNameID], [ApplicationNameID], [LoginNameID], [DatabaseID], [SQLStatementID], [SQLSignatureID], [SessionID])
	end
	
	--Srishti--4.1.5 Index Suitability Fixes -- DROPPING INDEX	
	--if (not exists(select id from sysindexes where name = 'IXDatabaseStatisticsDatabaseID' collate database_default)) 
	--begin
	--	CREATE NONCLUSTERED INDEX IXDatabaseStatisticsDatabaseID ON [dbo].[DatabaseStatistics] ([DatabaseID],[UTCCollectionDateTime])
	--		INCLUDE ([Transactions],[TimeDeltaInSeconds],[BytesRead],[BytesWritten])
	--end
	
	-- Upgrade process for Informational Alerting in 7.0
	if (select count([Internal_Value]) from [dbo].[RepositoryInfo] where [Name] = 'UpgradedTo7.0') <> 0
	begin
		if (select count([Internal_Value]) from [dbo].[RepositoryInfo] where [Name] = 'PostInstallUpgradeTo7.0') = 0
		begin
		
			declare @BatchUpgradeDate datetime
			
			set @BatchUpgradeDate = DATEADD(dd, datediff(dd, 0, GETDATE()-1),0)
			
			-- Alerts
			update [dbo].[Alerts] set [Severity] = [Severity]*2 where [UTCOccurrenceDateTime] >= @BatchUpgradeDate  and [Severity] in (2,4)
			
			-- ServerActivity
			select SQLServerID, UTCCollectionDateTime, cast(StateOverview as XML) as [StateOverview] into #SATempData from ServerActivity where UTCCollectionDateTime >= @BatchUpgradeDate

			while exists (select * from #SATempData where StateOverview.exist('/Servers/Server//State/@Severity[.="4"]') = 1)
				update #SATempData 
					set StateOverview.modify('replace value of (/Servers/Server//State/@Severity[.="4"])[1] with "8"')

			while exists (select * from #SATempData where StateOverview.exist('/Servers/Server//State/@Severity[.="2"]') = 1)
				update #SATempData 
					set StateOverview.modify('replace value of (/Servers/Server//State/@Severity[.="2"])[1] with "4"')

			update [dbo].[ServerActivity] set [ServerActivity].[StateOverview] = convert(nvarchar(max), b.StateOverview) from #SATempData b where ServerActivity.SQLServerID = b.SQLServerID and ServerActivity.UTCCollectionDateTime = b.UTCCollectionDateTime

			-- Tasks
			update [dbo].[Tasks] set [Severity] = [Severity]*2 where [CreatedOn] = @BatchUpgradeDate and [Severity] in (2,4) 

			insert into [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) values ('PostInstallUpgradeTo7.0', 1, Convert(nvarchar(30), @BatchUpgradeDate, 121))
		end
	end				
	if (not exists(select Internal_Value from RepositoryInfo where Name = 'Installed6.6GroomingHotfix'))
	begin

		create index [ActiveWaitStatisticsHostNameID] on [ActiveWaitStatistics]([HostNameID])
		create index [QueryMonitorStatisticsHostNameID] on [QueryMonitorStatistics]([HostNameID]) 
		create index [DeadlockProcessesHostNameID] on [DeadlockProcesses]([HostNameID])
		create index [BlockingSessionStatisticsHostNameID] on [BlockingSessionStatistics]([HostNameID])

		create index [ActiveWaitStatisticsApplicationNameID] on [ActiveWaitStatistics]([ApplicationNameID])
		create index [QueryMonitorStatisticsApplicationNameID] on [QueryMonitorStatistics]([ApplicationNameID]) 
		create index [DeadlockProcessesApplicationNameID] on [DeadlockProcesses]([ApplicationNameID])
		create index [BlockingSessionStatisticsApplicationNameID] on [BlockingSessionStatistics]([ApplicationNameID])
		create index [QuerySignatureAggregationApplicationNameID] on [QuerySignatureAggregation]([ApplicationNameID])
							
		create index [ActiveWaitStatisticsLoginNameID] on [ActiveWaitStatistics]([LoginNameID])
		create index [QueryMonitorStatisticsLoginNameID] on [QueryMonitorStatistics]([LoginNameID]) 
		create index [DeadlockProcessesLoginNameID] on [DeadlockProcesses]([LoginNameID])
		create index [BlockingSessionStatisticsLoginNameID] on [BlockingSessionStatistics]([LoginNameID])

		create index [ActiveWaitStatisticsSQLSignatureID] on [ActiveWaitStatistics]([SQLSignatureID])
		create index [QueryMonitorStatisticsSQLSignatureID] on [QueryMonitorStatistics]([SQLSignatureID]) 
		create index [DeadlockProcessesSQLSignatureID] on [DeadlockProcesses]([SQLSignatureID])
		create index [BlockingSessionStatisticsSQLSignatureID] on [BlockingSessionStatistics]([SQLSignatureID])
		create index [QuerySignatureAggregationSQLSignatureID] on [QuerySignatureAggregation]([SQLSignatureID])

		create index [ActiveWaitStatisticsSQLStatementID] on [ActiveWaitStatistics]([SQLStatementID])
		create index [QueryMonitorStatisticsSQLStatementID] on [QueryMonitorStatistics]([SQLStatementID]) 
		create index [DeadlockProcessesSQLStatementID] on [DeadlockProcesses]([SQLStatementID])
		create index [BlockingSessionStatisticsSQLStatementID] on [BlockingSessionStatistics]([SQLStatementID])

		create index [DatabaseFileActivityFileID] on [DatabaseFileActivity]([FileID],[UTCCollectionDateTime])
		create index [DatabaseFilesDatabaseID] on [DatabaseFiles]([DatabaseID])
		
		CREATE NONCLUSTERED INDEX [QueryMonitorStatisticsAggregationSupport]
			ON [dbo].[QueryMonitorStatistics] ([SQLServerID],[StatementUTCStartTime])
			INCLUDE ([StatementLocalStartTime],[DurationMilliseconds],[CPUMilliseconds],[Reads],[Writes],[ApplicationNameID],[DatabaseID],[StatementType],[SQLSignatureID])
		
		--START--Srishti--4.1.5 Index Suitability Fixes-- DROPPING INDEX
		--CREATE NONCLUSTERED INDEX [DeadlockProcessesAggregationSupport]
		--	ON [dbo].[DeadlockProcesses] ([SQLServerID],[UTCOccurrenceDateTime])
		--	INCLUDE ([LocalOccurrenceDateTime],[ApplicationNameID],[DatabaseID],[SQLSignatureID])
		--END--Srishti--4.1.5 Index Suitability Fixes

		CREATE NONCLUSTERED INDEX [QuerySignatureAggregationAggregationSupport]
			ON [dbo].[QuerySignatureAggregation] ([SQLServerID])
			INCLUDE ([QuerySignatureAggregationID],[EventUTCStartTime],[QueryMonitorOccurrences],[WaitOcurrences],[BlockingOcurrences],[DeadlockOcurrences],[TotalDurationMilliseconds],[MaxDurationMilliseconds],[TotalCPUMilliseconds],[MaxCPUMilliseconds],[TotalReads],[MaxReads],[TotalWrites],[MaxWrites],[StatementType],[TotalBlockingDurationMilliseconds],[MaxBlockingDurationMilliseconds],[TotalWaitDuration],[MaxWaitDuration],[ApplicationNameID],[DatabaseID],[SQLSignatureID])
	
		insert into RepositoryInfo(Name,Internal_Value)
		select 'Installed6.6GroomingHotfix',1
	end
	
	if (not exists(select index_id from sys.indexes where name = 'IXQueryMonitorStatisticsQMViewSupport' collate database_default)) 
	begin
		create index IXQueryMonitorStatisticsQMViewSupport on [QueryMonitorStatistics](SQLServerID,StatementType,DurationMilliseconds)
		include (UTCCollectionDateTime,CPUMilliseconds,Reads,Writes,HostNameID,ApplicationNameID,LoginNameID,DatabaseID,SQLStatementID,SQLSignatureID,SessionID)
	end

	if (not exists(select index_id from sys.indexes where name = 'IXActiveWaitStatisticsQMViewSupport' collate database_default)) 
	begin
		create index IXActiveWaitStatisticsQMViewSupport on [ActiveWaitStatistics](SQLServerID, WaitDuration, StatementUTCStartTime, SQLSignatureID)
		include (SessionID, HostNameID, ApplicationNameID, LoginNameID, DatabaseID, SQLStatementID)
	end

	--START--Srishti--4.1.5 Index Suitability Fixes-- DROPPING INDEX
	--if (not exists(select index_id from sys.indexes where name = 'IXDeadlockProcessesQMViewSupport' collate database_default)) 
	--begin
	--	create index IXDeadlockProcessesQMViewSupport ON [dbo].[DeadlockProcesses] ([SQLServerID],[UTCOccurrenceDateTime],[SQLStatementID])
	--	INCLUDE ([HostNameID],[ApplicationNameID],[LoginNameID],[DatabaseID],[SQLSignatureID],[SessionID])
	--end
	--END--Srishti--4.1.5 Index Suitability Fixes
	
	--Start-(Tushar)--Dropping this index (4.1.4)--Performance improvement for p_GetTopSessionsByCPUUsage stored procedure
	--if (not exists(select index_id from sys.indexes where name = 'IXServerActivityGroomingSupport' collate database_default)) 
	--begin
	--	create index IXServerActivityGroomingSupport ON [dbo].[ServerActivity] (UTCCollectionDateTime)
	--end
	--End-(Tushar)--Dropping this index (4.1.4)--Performance improvement for p_GetTopSessionsByCPUUsage stored procedure

	if (not exists(select index_id from sys.indexes where name = 'IXTableGrowthGroomingSupport' collate database_default)) 
	begin
		create index IXTableGrowthGroomingSupport ON [dbo].[TableGrowth] (UTCCollectionDateTime)
	end
	
	if (not exists(select index_id from sys.indexes where name = 'IXDeadlocksGroomingSupport' collate database_default)) 
	begin
		create index IXDeadlocksGroomingSupport ON [dbo].[Deadlocks] (UTCCollectionDateTime)
	end


	IF  EXISTS (SELECT object_id FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_DropDefaultConstraint]') AND type in (N'P', N'PC'))
		DROP PROCEDURE [dbo].[p_DropDefaultConstraint]

	if not exists(select name from sys.indexes where name = 'DeadlockProcessDeadlockID')
	begin
		create index DeadlockProcessDeadlockID on DeadlockProcesses([DeadlockID])
	end

	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
		Where name = N'ix_ServerActivity_UTCCollectionDateTime_SQLServerID_Includes_StateOverview' collate database_default
		And object_id = OBJECT_ID('dbo.ServerActivity'))
	  Begin
		print 'Creating ix_ServerActivity_UTCCollectionDateTime_SQLServerID_Includes_StateOverview'
		Create Index ix_ServerActivity_UTCCollectionDateTime_SQLServerID_Includes_StateOverview
			On dbo.ServerActivity (UTCCollectionDateTime, SQLServerID)
			Include (StateOverview);
	  End
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix__DatabaseSize__DatabaseId_UTCCollectionDateTime_Includes' collate database_default
			And object_id = OBJECT_ID('dbo.DatabaseSize') )
	  Begin
		print 'Creating ix__DatabaseSize__DatabaseId_UTCCollectionDateTime_Includes'
		Create Index ix__DatabaseSize__DatabaseId_UTCCollectionDateTime_Includes
			On dbo.DatabaseSize
		(
			[DatabaseID] ASC,
			[UTCCollectionDateTime] ASC
		)
		Include (DataFileSizeInKilobytes,
			DataSizeInKilobytes,
			TextSizeInKilobytes,
			IndexSizeInKilobytes,
			DataExpansionInKilobytes,
			TimeDeltaInSeconds);
	  End
	----------------------------------------------------------------------
	-- upgrade to 8.0
	--Srishti--4.1.5 Index Suitability Fixes 
	--If Not Exists (Select 1 from sys.indexes
	--		Where name = N'ix_DatabaseStatistics_UTCCollectionDateTime_Includes' collate database_default
	--		And object_id = OBJECT_ID('dbo.DatabaseStatistics') )
	--  Begin
	--	print 'Creating ix_DatabaseStatistics_UTCCollectionDateTime_Includes'
	--	Create Index ix_DatabaseStatistics_UTCCollectionDateTime_Includes
	--		On dbo.DatabaseStatistics (UTCCollectionDateTime)
	--		Include (DatabaseID, DatabaseStatus, Transactions, LogFlushWaits, LogFlushes,
	--			LogKilobytesFlushed, LogCacheReads, LogCacheHitRatio, TimeDeltaInSeconds,
	--			NumberReads, NumberWrites, BytesRead, BytesWritten, IoStallMS, LastBackupDateTime);
	--  End
	
	----------------------------------------------------------------------
	-- upgrade to 8.0

	-- if lower(cast(serverproperty('edition') as nvarchar(50))) like '%enterprise edition%'
	-- or lower(cast(serverproperty('edition') as nvarchar(50))) like '%developer edition%'
	-- begin

	-- If Exists (Select 1 From sys.indexes
		-- Where object_id = object_id('dbo.TableGrowth')
		-- And name = 'PKTableGrowth'
		-- And fill_factor <> 60)
	  -- Begin
	  -- print 'Changing fillfactor of PKTableGowth to 60'
		-- -- Change FillFactor
		-- Alter Index PKTableGrowth On dbo.TableGrowth Rebuild
			-- With (Online = On, FillFactor = 60);
	  -- End
	-- End  

	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_TempdbFileData_FileID_Includes_UTCCollectionDateTime'
			And object_id = OBJECT_ID('dbo.TempdbFileData'))
	  Begin
		print 'Creating ix_TempdbFileData_FileID_Includes_UTCCollectionDateTime'
		Create Index ix_TempdbFileData_FileID_Includes_UTCCollectionDateTime
			On dbo.TempdbFileData (FileID)
			Include (UTCCollectionDateTime);
	  End
	  
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix__DiskDrives__SQLServerID_DriveName__Includes'
			And object_id = OBJECT_ID('dbo.DiskDrives'))
	  Begin
		print 'Creating ix__DiskDrives__SQLServerID_DriveName__Includes'
		CREATE INDEX ix__DiskDrives__SQLServerID_DriveName__Includes
			ON dbo.DiskDrives (SQLServerID, DriveName)
			INCLUDE (UTCCollectionDateTime,
				AverageDiskMillisecondsPerRead,
				AverageDiskMillisecondsPerTransfer,
				AverageDiskMillisecondsPerWrite,
				DiskReadsPerSecond,
				DiskTransfersPerSecond,
				DiskWritesPerSecond);
	  End
	  
	----------------------------------------------------------------------	
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_BlockingSessionStatistics_UTCCollectionDateTime_Includes_BlockingUTCStartTime'
			And object_id = OBJECT_ID('dbo.BlockingSessionStatistics'))
	  Begin
		print 'Creating ix_BlockingSessionStatistics_UTCCollectionDateTime_Includes_BlockingUTCStartTime'
		Create Index ix_BlockingSessionStatistics_UTCCollectionDateTime_Includes_BlockingUTCStartTime
			On dbo.BlockingSessionStatistics (UTCCollectionDateTime)
			Include (BlockingUTCStartTime);
	  End

	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_BlockingSessionStatistics_SQLServerID_Includes_BlockingUTCStartTime'
			And object_id = OBJECT_ID('dbo.BlockingSessionStatistics'))
	  Begin
		print 'Creating ix_BlockingSessionStatistics_SQLServerID_Includes_BlockingUTCStartTime'
		Create Index ix_BlockingSessionStatistics_SQLServerID_Includes_BlockingUTCStartTime
			On dbo.BlockingSessionStatistics (SQLServerID)
			Include (BlockingUTCStartTime);
	  End

	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_BlockingSessionStatistics__Includes_SQLSignatureID'
			And object_id = OBJECT_ID('dbo.BlockingSessionStatistics'))
	  Begin
		print 'Creating ix_BlockingSessionStatistics__Includes_SQLSignatureID'
		Create Index ix_BlockingSessionStatistics__Includes_SQLSignatureID
			On dbo.BlockingSessionStatistics (SQLServerID, BlockingUTCStartTime, BlockingLocalStartTime, BlockingDurationMilliseconds, ApplicationNameID,  DatabaseID)
			Include (SQLSignatureID);
	  End

	----------------------------------------------------------------------------	
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_QuerySignatureAggregation_EventUTCStartTime_Includes_QuerySignatureAggregationID'
			And object_id = OBJECT_ID('dbo.QuerySignatureAggregation'))
	  Begin
		print 'Creating ix_QuerySignatureAggregation_EventUTCStartTime_Includes_QuerySignatureAggregationID'
		Create Index ix_QuerySignatureAggregation_EventUTCStartTime_Includes_QuerySignatureAggregationID
			On dbo.QuerySignatureAggregation (EventUTCStartTime)
			Include (QuerySignatureAggregationID);
	  End
	  
	--------------------------------------------------------------
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix__BaselineStatistics__SQLServerID_MetricID__Includes'
			And object_id = OBJECT_ID('dbo.BaselineStatistics'))
	  Begin
		print 'Creating ix__BaselineStatistics__SQLServerID_MetricID__Includes'
		CREATE INDEX ix__BaselineStatistics__SQLServerID_MetricID__Includes
			ON dbo.BaselineStatistics (SQLServerID, MetricID)
			INCLUDE (UTCCalculation, Mean);
	  End
	-------------------------------------------------------------------------------------	
	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix_MetricThresholds_Enabled_Includes'
			And object_id = OBJECT_ID('dbo.MetricThresholds'))
	  Begin
		print 'Creating ix_MetricThresholds_Enabled_Includes'
		Create Index ix_MetricThresholds_Enabled_Includes
			On dbo.MetricThresholds (Enabled)
			Include (SQLServerID, Metric);
	  End

	----------------------------------------------------------------------
	-- upgrade to 8.0
	If Not Exists (Select 1 from sys.indexes
			Where name = N'ix__MetricThresholds__ThresholdInstanceID'
			And object_id = OBJECT_ID('dbo.MetricThresholds'))
	  Begin
		print 'Creating ix__MetricThresholds__ThresholdInstanceID'
		Create Index ix__MetricThresholds__ThresholdInstanceID
			On dbo.MetricThresholds (ThresholdInstanceID);
	  End
	  
	----------------------------------------------------------------------	

	--upgrade to 9.1
	-----------------------------------------------------------------------------------------------------
	--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
	if (not exists (SELECT object_id  FROM sys.indexes WHERE object_id = OBJECT_ID('BlockingSessionStatistics') and name = 'IXBlockingSessionStatistics_DurationMilliSeconds'collate database_default)) 
	begin
		CREATE NONCLUSTERED INDEX [IXBlockingSessionStatistics_DurationMilliSeconds] on [BlockingSessionStatistics]([BlockingDurationMilliseconds])
	END
	--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance

    --START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
	--Srishti--4.1.5 Index Suitability Fixes-- DROPPING and RECREATING INDEX	
	--if not exists(select name from sys.indexes where object_id=object_id('DeadlockProcesses') and name = 'DeadlockProcess_NonClustered')
	--begin
	--	create index [DeadlockProcess_NonClustered] on DeadlockProcesses([SQLServerID],[UTCOccurrenceDateTime],[SQLSignatureID])
	--end
	--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding index to improve the query performance
	if not exists(select name from sys.indexes where object_id=object_id('QuerySignatureAggregation') and name = 'QuerySignatureAggregation_SQLServerID')
	begin
		create index [QuerySignatureAggregation_SQLServerID] on QuerySignatureAggregation([SQLServerID])
	end

	--START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Adding index to improve the query performance
	--Srishti--4.1.5 Index Suitability Fixes -- DROPPING and RECREATING INDEX
	--IF not exists(select name from sys.indexes where object_id=object_id('DatabaseStatistics') and name = 'IX_DatabaseStatisticsComposite')
	--begin
	--	CREATE NONCLUSTERED INDEX IX_DatabaseStatisticsComposite ON [dbo].[DatabaseStatistics] ([DatabaseID],[UTCCollectionDateTime])
	--	INCLUDE ([Transactions],[LogFlushWaits],[LogFlushes],[LogKilobytesFlushed],[LogCacheReads],[LogCacheHitRatio],[TimeDeltaInSeconds],[BytesRead],[BytesWritten],[DatabaseSizeTime])
	--end
	--END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43423 -- Adding index to improve the query performance

	--START: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements -add a non-clustered index
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXDatabaseFilesFileGroupName' )
	Begin
		CREATE NONCLUSTERED INDEX [IXDatabaseFilesFileGroupName] ON [dbo].[DatabaseFiles]
		(
			[FileGroupName] ASC
		)
	End
	
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXDiskDriveStatisticsUTCCollectionDateTime' )
	Begin
		CREATE NONCLUSTERED INDEX [IXDiskDriveStatisticsUTCCollectionDateTime] ON [dbo].[DiskDriveStatistics]
		(
			[UTCCollectionDateTime] ASC
		)
	End

	
	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXDiskDriveStatisticsDriveName' )
	Begin
		CREATE NONCLUSTERED INDEX [IXDiskDriveStatisticsDriveName] ON [dbo].[DiskDriveStatistics]
		(
			[DriveName] ASC
		)
	End

	IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IXDiskDriveStatisticsSQLServerID' )
	Begin
		CREATE NONCLUSTERED INDEX [IXDiskDriveStatisticsSQLServerID] ON [dbo].[DiskDriveStatistics]
		(
			[SQLServerID] ASC
		)
	End
--END: SQLdm 9.1 (Abhishek Joshi) -Filegroup improvements -add a non-clustered index
	-----------------------------------------------------------------------------------------------------

	--START SQLdm(10.2)
	--START--Tushar--Adding index on Alerts table for immproving performance of p_GetDatabaseOverview.(If table is already present in case of upgrade)
 	IF (not exists(select id from sysindexes where id = object_id('Alerts') and name = 'IXAlerts_DatabaseName' collate database_default))
 	BEGIN
 		CREATE NONCLUSTERED INDEX [IXAlerts_DatabaseName] ON [Alerts] 
 		(
 			[DatabaseName] ASC
 		)
 	END

	--END--Tushar--Adding index on Alerts table for immproving performance of p_GetDatabaseOverview.(If table is already present in case of upgrade)
	--START--Srishti--4.1.5 Index Suitability Fixes
	--Duplicate Index
	IF (exists(select id from sysindexes where name = 'IXSQLServerDatabaseNames2' collate database_default)) 
	BEGIN
		DROP INDEX [IXSQLServerDatabaseNames2]
		ON [dbo].[SQLServerDatabaseNames]
	END
	--Index Consolidation
	if (exists(select index_id from sys.indexes where name = 'IXDeadlockProcessesQMViewSupport' collate database_default)) 
	begin
		DROP INDEX DeadlockProcesses.IXDeadlockProcessesQMViewSupport 
	END
	if (exists(select index_id from sys.indexes where name = 'DeadlockProcessesAggregationSupport' collate database_default)) 
	begin
		DROP INDEX DeadlockProcesses.DeadlockProcessesAggregationSupport
	END
	if (exists(select index_id from sys.indexes where name = 'DeadlockProcess_NonClustered' collate database_default)) 
	begin
		DROP INDEX DeadlockProcesses.DeadlockProcess_NonClustered
	END
	if not exists(select name from sys.indexes where object_id=object_id('DeadlockProcesses') and name = 'DeadlockProcess_NonClustered')
	begin
		CREATE index [DeadlockProcess_NonClustered] on DeadlockProcesses([SQLServerID],[UTCOccurrenceDateTime],[SQLSignatureID])
		INCLUDE ([LocalOccurrenceDateTime],[HostNameID],[ApplicationNameID],[LoginNameID],[DatabaseID],[SessionID])
	END
	--Overlapping Indexes
	--SQLServerDatabaseNames
	if (exists(select index_id from sys.indexes where name = 'IXSQLServerDatabaseNames' collate database_default)) 
	begin
		DROP INDEX SQLServerDatabaseNames.[IXSQLServerDatabaseNames] 
	END
	if (exists(select index_id from sys.indexes where name = 'IXSQLServerDatabaseNamesServerIDDBID' collate database_default)) 
	begin
		DROP INDEX SQLServerDatabaseNames.IXSQLServerDatabaseNamesServerIDDBID
	END

	if not exists(select * from sys.indexes where name = 'IXSQLServerDatabaseNamesServerIDDBID')
	Begin
		CREATE NONCLUSTERED INDEX [IXSQLServerDatabaseNamesServerIDDBID] ON [dbo].[SQLServerDatabaseNames] 
		(
			[DatabaseID] ASC,
			[SQLServerID] ASC
		) ON [PRIMARY]
	End

	-- DatabaseStatistics
	IF Exists (Select 1 from sys.indexes
			Where name = N'ix_DatabaseStatistics_UTCCollectionDateTime_Includes' collate database_default
			And object_id = OBJECT_ID('dbo.DatabaseStatistics') )
	BEGIN
		DROP Index dbo.DatabaseStatistics.ix_DatabaseStatistics_UTCCollectionDateTime_Includes
	END	
	IF (exists(select id from sysindexes where name = 'IXDatabaseStatisticsDatabaseID' collate database_default)) 
	BEGIN
		DROP Index dbo.[DatabaseStatistics].IXDatabaseStatisticsDatabaseID
	END
	IF exists(select name from sys.indexes where object_id=object_id('DatabaseStatistics') and name = 'IX_DatabaseStatisticsComposite')
	BEGIN
		-- Prevent the recreation of [DatabaseStatistics].IX_DatabaseStatisticsComposite index everytime Management service starts
		declare @nonIncludeCount int
		SELECT @nonIncludeCount = COUNT(*)
		FROM 
			sys.indexes i
		INNER JOIN 
			sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
		INNER JOIN 
			sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
		WHERE
			i.name like 'IX_DatabaseStatisticsComposite' collate database_default
			and 
			c.name collate database_default NOT in ('Transactions','TimeDeltaInSeconds','BytesRead','BytesWritten', 'DatabaseSizeTime')
			and 
			ic.is_included_column = 1
			
		IF (@nonIncludeCount > 0)
		BEGIN
			DROP Index dbo.[DatabaseStatistics].IX_DatabaseStatisticsComposite
		END
	END
	IF not exists(select name from sys.indexes where object_id=object_id('DatabaseStatistics') and name = 'IX_DatabaseStatisticsComposite')
	begin
		CREATE NONCLUSTERED INDEX IX_DatabaseStatisticsComposite ON [dbo].[DatabaseStatistics] ([DatabaseID],[UTCCollectionDateTime])
		INCLUDE ([Transactions],[TimeDeltaInSeconds],[BytesRead],[BytesWritten],[DatabaseSizeTime])
	End

	--Missing Index
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Alerts]') AND name = N'IX_Alerts_UTCOccurrenceDateTime_Metric_Severity')
		CREATE INDEX [IX_Alerts_UTCOccurrenceDateTime_Metric_Severity]
		ON [dbo].[Alerts]
		( [UTCOccurrenceDateTime] )
		INCLUDE( [Metric], [Severity] )
	--END--Srishti--4.1.5 Index Suitability Fixes
	--END SQLdm(10.2)


	--SQLdm(10.2)--Anshul Aggarwal --Adding covering index on WaitStatistics table for immproving performance of p_GetServerSummary.(If table is already present in case of upgrade)
 	IF (not exists(select id from sysindexes where id = object_id('WaitStatistics') and name = 'IXWaitStatistics_SQLServerID_UTCCollectionDateTime_Includes' collate database_default))
 	BEGIN
 		CREATE NONCLUSTERED INDEX [IXWaitStatistics_SQLServerID_UTCCollectionDateTime_Includes] 
		ON [WaitStatistics] (SQLServerID, UTCCollectionDateTime)
		INCLUDE (WaitStatisticsID, TimeDeltaInSeconds);
 	END
	--Start - Tushar--4.1.6--Adding index on QueryMonitorStatistics table for performace improvement of p_GetQueryDataByQuerySignature stored procedure.
	IF (not exists(select id from sysindexes where id = object_id('QueryMonitorStatistics') and name = 'IXQueryMonitorStatistics_SQLServerID_DatabaseID_SQLSignatureID' collate database_default))
	BEGIN
		CREATE NONCLUSTERED INDEX [IXQueryMonitorStatistics_SQLServerID_DatabaseID_SQLSignatureID] ON [dbo].[QueryMonitorStatistics]
		(
		[SQLServerID] ASC,
		[DatabaseID] ASC,
		[SQLSignatureID] ASC
		)
	END
	--End - Tushar--4.1.6--Adding index on QueryMonitorStatistics table for performace improvement of p_GetQueryDataByQuerySignature stored procedure.

	--Start-(Tushar)--Dropping this index (4.1.4)--Performance improvement for p_GetTopSessionsByCPUUsage stored procedure
	IF (exists(select id from sysindexes where id = object_id('ServerActivity') and name = 'IXServerActivityGroomingSupport' collate database_default))
	BEGIN
		DROP INDEX IXServerActivityGroomingSupport ON ServerActivity;  
	END
	--End-(Tushar)--Dropping this index (4.1.4)--Performance improvement for p_GetTopSessionsByCPUUsage stored procedure
	

	-- SQLdm Upgrade 10.3
	IF (OBJECT_ID('CloudProviders') IS NOT NULL)
	BEGIN 
		DECLARE @CloudProviderId INT;
		SELECT @CloudProviderId = 3;
		
		-- SQLdm 10.3 (Varun Chopra) - Populating the master table for cloud providers with Linux option
		IF((SELECT COUNT(0) FROM CloudProviders WHERE CloudProviderId = @CloudProviderId) = 0)
		BEGIN
			INSERT INTO CloudProviders(CloudProviderId,CloudProviderName) VALUES (@CloudProviderId,'Linux');
		END
	END
	
	--SQLDM 10.3--Updating the Permission value in Permission table for existing user role defined in earlier version
	IF (select count([Internal_Value]) from [dbo].[RepositoryInfo] where [Name] = 'UpgradeToDM10.3') = 0
	BEGIN
	  UPDATE dbo.Permission SET Permission= Permission + 1 where Permission > 0 
	  INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) VALUES ('UpgradeToDM10.3', 1, Convert(nvarchar(30), GetDate(), 121))
	END
	
	---- Updating Grooming Details
	if not exists (select [Internal_Value] from RepositoryInfo where [Name] = 'GroomForecasting')
	begin
		insert into [RepositoryInfo] ([Name],[Internal_Value]) values ('GroomForecasting',1095);
	end
	---- Updating Aggregation Details
	if not exists (select [Internal_Value] from RepositoryInfo where [Name] = 'AggregateForecasting')
	begin
		insert into [RepositoryInfo] ([Name],[Internal_Value]) values ('AggregateForecasting',1095);
	end
	
	IF (OBJECT_ID(N'[dbo].[AlwaysOnReplicas]') IS NOT NULL)
	BEGIN
		IF NOT EXISTS(SELECT * FROM sys.foreign_keys WHERE object_id = object_id(N'[dbo].[fk_AlwaysOnReplicasForServerID]') and parent_object_id = object_id(N'[dbo].[AlwaysOnReplicas]'))
		BEGIN
			ALTER TABLE [dbo].[AlwaysOnReplicas]
				WITH NOCHECK ADD CONSTRAINT [fk_AlwaysOnReplicasForServerID]
				FOREIGN KEY([SQLServerID])
				REFERENCES [dbo].[MonitoredSQLServers] ([SQLServerID])
			ON DELETE CASCADE
			ON UPDATE CASCADE
		END
	END
	--SQLDM 10.3--Updating the Permission value in Permission table for existing user role defined in earlier version
	IF (select count([Internal_Value]) from [dbo].[RepositoryInfo] where [Name] = 'UpgradeToDM11.0') = 0
	BEGIN		
		-- SQLdm 11 - 5.5 Azure Custom Counter changes - AzureProfileId
		IF (NOT EXISTS(SELECT sc.[object_id] FROM sys.columns sc INNER JOIN sys.types st ON sc.[system_type_id] = st.[system_type_id]
			WHERE sc.[object_id]=OBJECT_ID('CustomCounterDefinition') 
			AND sc.[name]='AzureProfileId' AND LOWER(st.[name]) = 'bigint'  COLLATE database_default))
		BEGIN
			ALTER TABLE CustomCounterDefinition ADD [AzureProfileId] BIGINT NULL
		END

		IF (OBJECT_ID('RecommedationClassification') IS NULL)
		BEGIN

		CREATE TABLE RecommedationClassification(
		  RecommendationID VARCHAR(40),
		  AWS BIT,
		  Azure BIT,
		  OnPremisesDb BIT
		)

		INSERT INTO RecommedationClassification VALUES ('SDR-BP1',1,1,1),('SDR-D1',1,0,1),('SDR-D10',1,0,1),('SDR-D11',1,0,1),('SDR-D12',1,0,1),('SDR-D13',1,0,1),('SDR-D14',1,1,1),('SDR-D15',0,0,1),('SDR-D16',1,1,1),('SDR-D17',1,1,1),('SDR-D18',1,1,1),('SDR-D19',1,1,1),('SDR-D2',1,0,1),('SDR-D20',1,1,1),('SDR-D21',1,1,1),('SDR-D22',1,0,1),('SDR-D23',0,0,1),('SDR-D3',1,1,1),('SDR-D4',0,0,1),('SDR-D5',0,0,1),('SDR-D6',0,0,1),('SDR-D7',1,0,1),('SDR-D8',0,0,1),('SDR-D9',1,0,1),('SDR-DC1',1,0,1),('SDR-DC2',1,0,1),('SDR-DC3',1,0,1),('SDR-DC4',1,1,1),('SDR-DC5',0,0,1),('SDR-DL1',1,1,1),('SDR-DL2',1,1,1),('SDR-I1',1,1,1),('SDR-I10',1,1,1),('SDR-I11',1,1,1),('SDR-I12',1,1,1),('SDR-I13',1,1,1),('SDR-I14',1,1,1),('SDR-I15',1,1,1),('SDR-I16',1,1,1),('SDR-I17',1,1,1),('SDR-I18',1,1,1),('SDR-I19',1,1,1),('SDR-I2',1,1,1),('SDR-I20',1,1,1),('SDR-I21',1,1,1),('SDR-I22',1,1,1),('SDR-I23',1,1,1),('SDR-I24',1,1,1),('SDR-I25',0,1,1),('SDR-I26',0,1,1),('SDR-I27',1,1,1),('SDR-I28',0,1,1),('SDR-I29',0,1,1),('SDR-I3',1,1,1),('SDR-I30',1,1,1),('SDR-I31',1,1,1),('SDR-I4',1,1,1),('SDR-I5',1,1,1),('SDR-I6',1,1,1),('SDR-I7',1,1,1),('SDR-I8',1,1,1),('SDR-I9',1,1,1),('SDR-LR1',1,0,1),('SDR-M1',0,0,1),('SDR-M10',0,0,1),('SDR-M11',0,0,1),('SDR-M12',0,0,1),('SDR-M13',1,1,1),('SDR-M14',1,1,1),('SDR-M15',1,1,1),('SDR-M16',0,0,1),('SDR-M17',1,0,1),('SDR-M18',1,0,1),('SDR-M19',1,0,1),('SDR-M2',0,0,1),('SDR-M20',0,0,1),('SDR-M21',0,0,1),('SDR-M22',0,0,1),('SDR-M23',0,0,1),('SDR-M24',0,0,1),('SDR-M25',0,0,1),('SDR-M26',0,0,1),('SDR-M27',0,0,1),('SDR-M28',1,1,1),('SDR-M29',1,0,1),('SDR-M3',1,0,1),('SDR-M30',0,0,1),('SDR-M31',0,0,1),('SDR-M32',0,0,1),('SDR-M33',0,0,1),('SDR-M4',1,0,1),('SDR-M5',1,0,1),('SDR-M6',1,0,1),('SDR-M7',1,1,1),('SDR-M8',0,0,1),('SDR-M9',0,0,1),('SDR-N1',0,0,1),('SDR-N2',0,0,1),('SDR-N3',0,0,1),('SDR-N4',0,0,1),('SDR-N5',0,0,1),('SDR-N6',1,0,1),('SDR-N7',1,1,1),('SDR-N8',0,0,1),('SDR-N9',0,0,1),('SDR-OT1',1,1,1),('SDR-P1',0,0,1),('SDR-P10',0,0,1),('SDR-P11',1,0,1),('SDR-P12',1,0,1),('SDR-P13',0,0,1),('SDR-P14',0,0,1),('SDR-P15',0,0,1),('SDR-P16',0,0,1),('SDR-P17',1,0,1),('SDR-P18',0,0,1),('SDR-P19',0,0,1),('SDR-P2',0,0,1),('SDR-P20',0,0,1),('SDR-P21',0,0,1),('SDR-P22',0,0,1),('SDR-P23',0,0,1),('SDR-P3',0,0,1),('SDR-P4',0,0,1),('SDR-P5',0,0,1),('SDR-P6',1,0,1),('SDR-P7',1,1,1),('SDR-P8',1,1,1),('SDR-P9',1,1,1),('SDR-Q1',1,1,1),('SDR-Q10',1,1,1),('SDR-Q11',1,1,1),('SDR-Q12',1,1,1),('SDR-Q13',1,1,1),('SDR-Q14',1,1,1),('SDR-Q15',1,1,1),('SDR-Q16',1,1,1),('SDR-Q17',1,1,1),('SDR-Q18',1,1,1),('SDR-Q19',1,1,1),('SDR-Q2',1,1,1),('SDR-Q20',1,1,1),('SDR-Q21',1,1,1),('SDR-Q22',1,1,1),('SDR-Q23',1,1,1),('SDR-Q24',1,1,1),('SDR-Q25',1,1,1),('SDR-Q26',1,1,1),('SDR-Q27',1,1,1),('SDR-Q28',1,1,1),('SDR-Q29',1,1,1),('SDR-Q3',1,1,1),('SDR-Q30',1,1,1),('SDR-Q31',1,1,1),('SDR-Q32',1,1,1),('SDR-Q33',1,1,1),('SDR-Q34',1,1,1),('SDR-Q35',1,1,1),('SDR-Q36',1,1,1),('SDR-Q37',0,0,1),('SDR-Q38',0,0,1),('SDR-Q39',0,1,1),('SDR-Q4',1,1,1),('SDR-Q40',0,1,1),('SDR-Q41',0,1,1),('SDR-Q42',0,1,1),('SDR-Q43',1,1,1),('SDR-Q44',1,0,1),('SDR-Q45',0,0,1),('SDR-Q46',1,1,1),('SDR-Q47',1,1,1),('SDR-Q48',1,1,1),('SDR-Q49',1,1,1),('SDR-Q5',1,1,1),('SDR-Q50',1,1,1),('SDR-Q6',1,1,1),('SDR-Q7',0,0,1),('SDR-Q8',1,1,1),('SDR-Q9',1,1,1),('SDR-R1',1,0,1),('SDR-R2',0,0,1),('SDR-R3',1,1,1),('SDR-R4',1,0,1),('SDR-R5',0,0,1),('SDR-R6',0,0,1),('SDR-R7',0,0,1),('SDR-R8',0,0,1),('SDR-S1',0,0,1),('SDR-S10',1,0,1),('SDR-S2',0,0,1),('SDR-S3',1,0,1),('SDR-S4',1,0,1),('SDR-S5',0,0,1),('SDR-S6',1,0,1),('SDR-S7',0,0,1),('SDR-S8',0,0,1),('SDR-S9',1,1,1),('SDR-SC1',1,0,1),('SDR-SC2',0,0,1),('SDR-SC3',1,0,1),('SDR-SC4',1,0,1),('SDR-SC5',1,0,1),('SDR-SC6',1,0,1),('SDR-SC7',1,0,1),('SDR-SC8',0,0,1),('SDR-SC9',1,0,1),('SDR-SF1',0,0,1),('SDR-UG1',0,0,1),('SDR-VL1',0,0,1),('SDR-W1',1,0,1),('SDR-W10',1,0,1),('SDR-W2',1,0,1),('SDR-W3',1,0,1),('SDR-W4',1,0,1),('SDR-W5',1,0,1),('SDR-W6',1,0,1),('SDR-W7',1,0,1),('SDR-W8',1,0,1),('SDR-W9',1,0,1),('SDR-D24',1,0,0),('SDR-DC6',1,0,0),('SDR-DC7',1,1,1),('SDR-DC8',0,1,0),('SDR-DC9',1,1,1),('SDR-DC10',1,1,1),('SDR-DC11',1,1,0),('SDR-DC12',0,0,1),('SDR-DC13',0,0,1),('SDR-M34',1,0,0);
		
		INSERT INTO RecommedationClassification VALUES ('SDR-M35',1,0,0),('SDR-R9',0,0,1),('SDR-R10',0,0,1),('SDR-R11',0,0,1),('SDR-S11',0,1,0),('SDR-S12',0,1,0),('SDR-S13',0,1,0),('SDR-S14',0,1,0),('SDR-S15',0,1,0),('SDR-P24',1,0,0),('SDR-SC10',1,0,0);
		END

		--START SQLdm 11 Add New Table of AzureApplication
		IF (OBJECT_ID('AzureApplication') is null)
		CREATE TABLE [dbo].[AzureApplication](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[Name] [nvarchar](MAX) NOT NULL,
			[TenantId] [nvarchar](MAX) NOT NULL,
			[ClientId] [nvarchar](MAX) NOT NULL,
			[Secret] [nvarchar](MAX) NOT NULL,
			[Description] [nvarchar](MAX) NULL,
			CONSTRAINT [PK_AzureApplication] PRIMARY KEY CLUSTERED ( [ID] ASC )
			)
		 --END

		--START SQLdm 11 Add New Table of AzureSubscription
		IF (OBJECT_ID('AzureSubscription') is null)
		CREATE TABLE [dbo].[AzureSubscription](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[SubscriptionId] [nvarchar](MAX) NOT NULL,
			[Description] [nvarchar](MAX) NULL
			CONSTRAINT [PK_AzureSubscription] PRIMARY KEY CLUSTERED ( [ID] ASC )
			)

		--START SQLdm 11 Add New Table of AzureResourceGroup
		IF (OBJECT_ID('AzureApplicationProfile') is null)
		CREATE TABLE [dbo].[AzureApplicationProfile](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[Name] [nvarchar](MAX) NOT NULL,
			[AzureSubscriptionId] [bigint] NOT NULL,
			[AzureApplicationId] [bigint] NOT NULL,
			[Description] [nvarchar](MAX) NULL
			CONSTRAINT [PK_AzureApplicationProfile] PRIMARY KEY CLUSTERED ( [ID] ASC ),
			CONSTRAINT [fk_AzureApplicationProfile_AzureSubscriptionId] FOREIGN KEY([AzureSubscriptionId])
				REFERENCES [dbo].[AzureSubscription]([ID]) ON DELETE CASCADE,
			CONSTRAINT [fk_AzureApplicationProfile_AzureApplicationId] FOREIGN KEY([AzureApplicationId])
				REFERENCES [dbo].[AzureApplication]([ID]) ON DELETE CASCADE,
			)

		--START SQLdm 11 Add New Table of AzureResourceGroup
		IF (OBJECT_ID('AzureResource') is null)
		CREATE TABLE [dbo].[AzureResource](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[Name] [nvarchar](MAX) NOT NULL,
			[Uri] [nvarchar](MAX) NOT NULL,
			[Type] [nvarchar](MAX) NOT NULL,
			[AzureApplicationProfileId] [bigint] NOT NULL,
			CONSTRAINT [PK_AzureResource] PRIMARY KEY CLUSTERED ( [ID] ASC ),
			CONSTRAINT [fk_AzureResource_AzureApplicationProfileId] FOREIGN KEY([AzureApplicationProfileId])
				REFERENCES [dbo].[AzureApplicationProfile]([ID]) ON DELETE CASCADE
			)

		--START SQLdm 11 Add New Table of AzureProfile
		IF (OBJECT_ID('AzureProfile') is null)
		CREATE TABLE [dbo].[AzureProfile](
			[ID] [bigint] IDENTITY(1,1) NOT NULL,
			[AzureApplicationProfileId] [bigint] NOT NULL,
			[SQLServerID] [int] NOT NULL,
			[Description] [nvarchar](MAX) NULL,
			CONSTRAINT [PK_AzureProfile] PRIMARY KEY CLUSTERED ( [ID] ASC ),
			CONSTRAINT [fk_AzureProfile_AzureApplicationProfileId] FOREIGN KEY([AzureApplicationProfileId])
				REFERENCES [dbo].[AzureApplicationProfile]([ID]) ON DELETE CASCADE,
			CONSTRAINT [fk_AzureProfile_SQLServerID] FOREIGN KEY ([SQLServerID])
				REFERENCES [dbo].[MonitoredSQLServers]([SQLServerID]) ON DELETE CASCADE
			)
		 --END

		 -- SQLdm 11 (Varun Chopra) - Populating the master table for cloud providers with Microsoft Azure SQL Managed Instance option
		 IF (OBJECT_ID('CloudProviders') IS NOT NULL)
		 BEGIN 
			SELECT @CloudProviderId = 5;

			IF (
					(
						SELECT COUNT(0)
						FROM CloudProviders
						WHERE CloudProviderId = @CloudProviderId
						) = 0
					)
			BEGIN
				INSERT INTO CloudProviders (
					CloudProviderId
					,CloudProviderName
					)
				VALUES (
					@CloudProviderId
					,'Microsoft Azure SQL Managed Instance'
					);
			END
		END

		INSERT INTO [dbo].[RepositoryInfo] ([Name], [Internal_Value],[Character_Value]) VALUES ('UpgradeToDM11.0', 1, Convert(nvarchar(30), GetDate(), 121))
	END
END
