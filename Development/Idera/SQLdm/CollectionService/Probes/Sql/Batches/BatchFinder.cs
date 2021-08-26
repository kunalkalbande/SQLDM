//------------------------------------------------------------------------------
// <copyright file="BatchFinder.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   31-Jan-2019
// Description          :   Done changes for New Azure SQL DB Alerts.
//----------------------------------------------------------------------------
using System;
using BBS.TracerX;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.CollectionService.Probes.Sql.Batches
{
    /// <summary>
    /// Locates the correct embedded assembly for a query on a given server version
    /// and returns the sql text
    /// </summary>
    class BatchFinder
    {
        #region constants
       
        private const string ActivityMonitor2005 = "ActivityMonitor2005.sql";
        private const string ActivityMonitor2000 = "ActivityMonitor2000.sql";
        private const string ActivityMonitorStop2005 = "ActivityMonitorStop2005.sql";
        private const string ActivityMonitorStop2000 = "ActivityMonitorStop2000.sql";
        private const string ActivityMonitorRead2005 = "ActivityMonitorRead2005.sql";
        private const string ActivityMonitorRead2000 = "ActivityMonitorRead2000.sql";
        private const string ActivityMonitorRestart2000 = "ActivityMonitorRestart2000.sql";
        private const string ActivityMonitorRestart2005 = "ActivityMonitorRestart2005.sql";
        private const string ActivityMonitorBlocking2005 = "ActivityMonitorBlocking2005.sql";
        private const string ActivityMonitorTraceDeadlocks2005 = "ActivityMonitorTraceDeadlocks.sql";
        private const string ActivityMonitorTraceAutogrow2000 = "ActivityMonitorTraceAutogrow.sql";
        private const string ActivityMonitorTraceAutogrow2005 = "ActivityMonitorTraceAutogrow.sql";
        private const string ActiveWaits2005 = "ActiveWaits2005.sql";
        private const string ActiveWaits2012 = "ActiveWaits2012.sql";
        private const string ActiveWaits2012EXN = "ActiveWaits2012EXN.sql";
        private const string ActiveWaitsRead2017Qs = "ActiveWaitsRead2017Qs.sql";
        //Start--SQLdm 10.3 (Tushar)--Support of extended events LINQ assembly for active waits. 
        private const string ActiveWaitsRead2012EX = "ActiveWaitsRead2012EX.sql";
        private const string ActiveWaitsWrite2012EX = "ActiveWaitsWrite2012EX.sql";
        //End--SQLdm 10.3 (Tushar)--Support of extended events LINQ assembly for active waits. 
        private const string AgentJobHistory2000 = "AgentJobHistory.sql";
        private const string AgentJobHistory2005 = "AgentJobHistory.sql";
        private const string AgentJobSummary2000 = "AgentJobSummary2000.sql";
        private const string AgentJobSummary2008 = "AgentJobSummary2008.sql";
        private const string AgentJobSummary2008Sp1R2 = "AgentJobSummary2008Sp1R2.sql";
        private const string AlwaysOnTopology2012 = "AlwaysOnTopology2012.sql";
        private const string AlwaysOnToplogySummary2012 = "AlwaysOnToplogySummary2012.sql";
        private const string AlwaysOnToplogyDetail2012 = "AlwaysOnToplogyDetail2012.sql";
        private const string AlwaysOnStatistics2012 = "AlwaysOnStatistics2012.sql";
        private const string BackupHistoryFull2000 = "BackupHistoryFull.sql";
        private const string BackupHistoryFull2005 = "BackupHistoryFull.sql";
        private const string BackupHistorySmall2000 = "BackupHistorySmall.sql";
        private const string BackupHistorySmall2005 = "BackupHistorySmall.sql";
        private const string BlockingCheck2000 = "BlockingCheck2000.sql";
        private const string BlockingCheck2005 = "BlockingCheck2005.sql";
        private const string BlockingCheck2014sp2 = "BlockingCheck2014sp2.sql";
        private const string Configuration2000 = "Configuration2000.sql";
        private const string Configuration2005 = "Configuration2005.sql";
        private const string CustomCounterOS2000 = "CustomCounterOS.sql";
        private const string CustomCounterOS2005 = "CustomCounterOS.sql";
        private const string CustomCounterSQL2000 = "CustomCounterSQL.sql";
        private const string CustomCounterSQL2005 = "CustomCounterSQL.sql";
        private const string DatabaseLastBackupDateTime = "DatabaseLastBackupDateTime.sql"; //SQLdm 10.1 (Tolga K) -- to get the last backup date and time for a database
        private const string DatabaseConfiguration2000 = "DatabaseConfiguration2000.sql";
        private const string DatabaseConfiguration2005 = "DatabaseConfiguration2005.sql";
        private const string DatabaseConfiguration2012 = "DatabaseConfiguration2012.sql"; //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- added name of new batch
        private const string DatabaseFiles2000 = "DatabaseFiles2000.sql";
        private const string DatabaseFiles2005 = "DatabaseFiles2005.sql";
        private const string DatabaseSize2000 = "DatabaseSize2000.sql";
        private const string DatabaseSize2005 = "DatabaseSize2005.sql";
        private const string DatabaseSize2012 = "DatabaseSize2012.sql";
        private const string DatabaseSummary2000 = "DatabaseSummary2000.sql";
        private const string DatabaseSummary2005 = "DatabaseSummary2005.sql";
        private const string DatabaseSummary2012 = "DatabaseSummary2012.sql";
        private const string DiskDrives2000 = "DiskDrives2000.sql";
        private const string DiskDrives2005 = "DiskDrives2005.sql";
        private const string DiskDriveHeader2000 = "DiskDriveHeader2000.sql";
        private const string DiskDriveHeader2005 = "DiskDriveHeader.sql";
        private const string DiskSize2000 = "DiskSize2000.sql";
        private const string DiskSize2005 = "DiskSize2005.sql";
        private const string DiskSize2008 = "DiskSize2008.sql";
        private const string DiskSize2008Sp1R2 = "DiskSize2008Sp1R2.sql";
        private const string DistributorQueue2000 = "DistributorQueue.sql";
        private const string DistributorQueue2005 = "DistributorQueue.sql";
        //SQLDM-30114.
        private const string DistributorQueue2016 = "DistributorQueue2016.sql";
        private const string DropPlansFromCache2008 = "DropPlansFromCache2008.sql";
        private const string DTCStatus2000 = "DTCStatus.sql";
        private const string DTCStatus2005 = "DTCStatus.sql";
        private const string DtcStatusServiceControlAccess2000 = "DTCStatusServiceControl.sql";
        private const string DtcStatusServiceControlAccess2005 = "DTCStatusServiceControl.sql";
        private const string FailedJobs2000 = "FailedJobs2000.sql";
        private const string FailedJobs2005 = "FailedJobs2005.sql";
        private const string FailedJobSteps2000 = "FailedJobSteps2000.sql";
        private const string FailedJobSteps2005 = "FailedJobSteps2005.sql";
        private const string CompletedJobs2000 = "CompletedJobs.sql";
        private const string CompletedJobs2005 = "CompletedJobs.sql";
        private const string FailedJobVariables2000 = "FailedJobVariables.sql";
        private const string FailedJobVariables2005 = "FailedJobVariables.sql";
        private const string FileActivity2000 = "FileActivity2000.sql";
        private const string FileActivity2005 = "FileActivity2005.sql";
        private const string FileActivity2008 = "FileActivity2008.sql";
        private const string FileActivity2008Sp1R2 = "FileActivity2008Sp1R2.sql";
        private const string Fragmentation2000 = "Fragmentation2000.sql";
        private const string Fragmentation2005 = "Fragmentation2005.sql";
        private const string FragmentationWorkload2000 = "FragmentationWorkload2000.sql";
        private const string FragmentationWorkload2005 = "FragmentationWorkload2005.sql";
        private const string FullTextCatalogs2000 = "FullTextCatalogs2000.sql";
        private const string FullTextCatalogs2005 = "FullTextCatalogs2005.sql";
        private const string FullTextColumns2000 = "FullTextColumns.sql";
        private const string FullTextColumns2005 = "FullTextColumns.sql";
        private const string FullTextSearchService2000 = "FullTextSearchService2000.sql";
        private const string FullTextSearchService2005 = "FullTextSearchService2005.sql";
        private const string FullTextSearchServiceServiceControl2000 = "FullTextSearchServiceServiceControl2000.sql";
        private const string FullTextSearchServiceServiceControl2005 = "FullTextSearchServiceServiceControl2005.sql";
        private const string FullTextTables2000 = "FullTextTables.sql";
        private const string FullTextTables2005 = "FullTextTables.sql";
        private const string IndexStatistics2000 = "IndexStatistics.sql";
        private const string IndexStatistics2005 = "IndexStatistics.sql";
        private const string LockCounterStatistics2000 = "LockCounterStatistics.sql";
        private const string LockCounterStatistics2005 = "LockCounterStatistics2005.sql";
        private const string LockDetails2000 = "LockDetails2000.sql";
        private const string LockDetails2005 = "LockDetails2005.sql";
        private const string LockDetails2014sp2 = "LockDetails2014sp2.sql";
        private const string LogList2000 = "LogList2000.sql";
        private const string LogList2005 = "LogList2005.sql";
        private const string LogScan2000 = "LogScan2000.sql";
        private const string LogScan2005 = "LogScan2005.sql";
        private const string LongJobs2000 = "LongJobs2000.sql";
        private const string LongJobs2005 = "LongJobs2005.sql";
        private const string LongJobsWithSteps2000 = "LongJobsWithSteps2000.sql";
        private const string LongJobsWithSteps2005 = "LongJobsWithSteps2005.sql";
        private const string LongJobsByPercent2000 = "LongJobsByPercent.sql";
        private const string LongJobsByPercent2005 = "LongJobsByPercent.sql";
        private const string LongJobsByRuntime2000 = "LongJobsByRuntime.sql";
        private const string LongJobsByRuntime2005 = "LongJobsByRuntime.sql";
        private const string Memory2000 = "Memory2000.sql";
        private const string Memory2005 = "Memory2005.sql";
        private const string Memory2012 = "Memory2012.sql";
        private const string OSMetrics2000 = "OSMetrics.sql";
        private const string OSMetrics2005 = "OSMetrics.sql";
        private const string OSMetrics2012 = "OSMetrics2012.sql";
        private const string OldestOpenTransaction2000 = "OldestOpenTransaction2000.sql";
        private const string OldestOpenTransaction2005 = "OldestOpenTransaction2005.sql";
        private const string PinnedTables2000 = "PinnedTables.sql";
        private const string PinnedTables2005 = "PinnedTables.sql";
        private const string ProcedureCache2000 = "ProcedureCache2000.sql";
        private const string ProcedureCache2005 = "ProcedureCache2005.sql";
        private const string ProcedureCacheList2000 = "ProcedureCacheList2000.sql";
        private const string ProcedureCacheList2005 = "ProcedureCacheList2005.sql";
        private const string QueryMonitor2000 = "QueryMonitor2000.sql";
        private const string QueryMonitor2005 = "QueryMonitor2005.sql";
        //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -added new batch names -start
        private const string QueryMonitor2008EX = "QueryMonitor2008EX.sql";
        private const string QueryMonitor2012EX = "QueryMonitor2012EX.sql";
        private const string QueryMonitorRestart2008EX = "QueryMonitorRestart2008EX.sql";
        private const string QueryMonitorRestart2012EX = "QueryMonitorRestart2012EX.sql";
        private const string QueryMonitorRead2008EX = "QueryMonitorRead2008EX.sql";
        private const string QueryMonitorRead2012EX = "QueryMonitorRead2012EX.sql";
        private const string QueryMonitorStopEX2008 = "QueryMonitorStopEX.sql";
        private const string QueryMonitorStopEX2012 = "QueryMonitorStopEX.sql";
        private const string QueryMonitorExtendedEventsBatches2008 = "QueryMonitorExtendedEventsBatches2008.sql";
        private const string QueryMonitorExtendedEventsBatches2012 = "QueryMonitorExtendedEventsBatches2012.sql";
        private const string QueryMonitorExtendedEventsSingleStmt2008 = "QueryMonitorExtendedEventsSingleStmt2008.sql";
        private const string QueryMonitorExtendedEventsSingleStmt2012 = "QueryMonitorExtendedEventsSingleStmt2012.sql";
        private const string QueryMonitorExtendedEventsSP2008 = "QueryMonitorExtendedEventsSP2008.sql";
        private const string QueryMonitorExtendedEventsSP2012 = "QueryMonitorExtendedEventsSP2012.sql";
        //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -added new batch names -end

        // START SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - added new batch names
        private const string QueryMonitorRead2016Qs = "QueryMonitorRead2016Qs.sql";
        private const string QueryMonitorStop2016Qs = "QueryMonitorStop2016Qs.sql";
        // END SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - added new batch names

        private const string QueryMonitorWrite2012EX = "QueryMonitorWrite2012EX.sql"; // SQLdm 10.2 (Anshul Aggarwal) - Batch to write last filename, record count
        private const string QueryMonitorReadEstimatedPlan2005 = "QueryMonitorReadEstimatedPlan2005.sql";//SQLdm 10.0 (Tarun Sapra) -- Added new batch for getting estimated query plan

        private const string ActivityMonitorWrite2012EX = "ActivityMonitorWrite2012EX.sql";// SQLdm 10.3 (Tushar) - Batch to write last filename, record count

        //START SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events -added new batch names 
        private const string ActivityMonitor2012EX = "ActivityMonitor2012EX.sql";
        private const string ActivityMonitorRestart2012EX = "ActivityMonitorRestart2012EX.sql";
        private const string ActivityMonitorRead2012EX = "ActivityMonitorRead2012EX.sql";
        private const string ActivityMonitorStopEX = "ActivityMonitorStopEX.sql";
        private const string ActivityMonitorDeadlocks2012EX = "ActivityMonitorDeadlocks2012EX.sql";
        private const string ActivityMonitorAutogrow2012EX = "ActivityMonitorAutogrow2012EX.sql";
        private const string ActivityMonitorBlocking2012EX = "ActivityMonitorBlocking2012EX.sql";
        //END SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events -added new batch names 

        //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new batches
        private const string FileGroup2000 = "FileGroup2000.sql";
        private const string FileGroup2005 = "FileGroup2005.sql";
        private const string DiskDriveStatisticsBatch = "DiskDriveStatistics.sql";
        //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new batches

        private const string QueryMonitorRestart2005 = "QueryMonitorRestart2005.sql";
        private const string QueryMonitorCheckSettings2000 = "QueryMonitorCheckSettings2000.sql";
        private const string QueryMonitorCheckSettings2005 = "QueryMonitorCheckSettings2005.sql";
        private const string QueryMonitorRestart2000 = "QueryMonitorRestart2000.sql";
        private const string QueryMonitorRead2000 = "QueryMonitorRead2000.sql";
        private const string QueryMonitorRead2005 = "QueryMonitorRead2005.sql";
        private const string QueryMonitorStop2000 = "QueryMonitorStop2000.sql";
        private const string QueryMonitorStop2005 = "QueryMonitorStop2005.sql";
        private const string QueryMonitorTraceBatches2000 = "QueryMonitorTraceBatches.sql";
        private const string QueryMonitorTraceBatches2005 = "QueryMonitorTraceBatches.sql";
        private const string QueryMonitorTraceSingleStmt2000 = "QueryMonitorTraceSingleStmt.sql";
        private const string QueryMonitorTraceSingleStmt2005 = "QueryMonitorTraceSingleStmt.sql";
        private const string QueryMonitorTraceSP2000 = "QueryMonitorTraceSP.sql";
        private const string QueryMonitorTraceSP2005 = "QueryMonitorTraceSP.sql";
        private const string FilteredMonitoredDatabasesAzure = "QueryMonitorDatabasesAzure.sql";
        private const string Reindex2000 = "Reindex2000.sql";
        private const string Reindex2005 = "Reindex2005.sql";
        //private const string Reorganization2000 = "Reorganization2000.sql";
        //private const string Reorganization2005 = "Reorganization2005.sql";
		// Permissions batch for replication check
        private const string ReplicationCheckPermissions = "ReplicationCheckPermissions.sql";
        private const string ReplicationCheck2000 = "ReplicationCheck.sql";
        private const string ReplicationCheck2005 = "ReplicationCheck2005.sql";
        //SQLDM-30114
        private const string ReplicationCheck2016 = "ReplicationCheck2016.sql";

        private const string ReplicationStatus2000 = "ReplicationStatus2000.sql";
        private const string ReplicationStatus2008 = "ReplicationStatus2008.sql";
        private const string ReplicationStatus2008Sp1R2 = "ReplicationStatus2008Sp1R2.sql";
        private const string ResourceCheck2000 = "ResourceCheck2000.sql";
        private const string ResourceCheck2005 = "ResourceCheck2005.sql";
        //private const string Resources2000 = "Resources2000.sql";
        //private const string Resources2005 = "Resources2005.sql";
        private const string RestoreHistoryFull2000 = "RestoreHistoryFull.sql";
        private const string RestoreHistoryFull2005 = "RestoreHistoryFull.sql";
        private const string RestoreHistorySmall2000 = "RestoreHistorySmall.sql";
        private const string RestoreHistorySmall2005 = "RestoreHistorySmall.sql";
        private const string ServerBasics2000 = "ServerBasics2000.sql";
        private const string ServerBasics2005 = "ServerBasics2005.sql";
        private const string ServerBasics2008 = "ServerBasics2008.sql";
        private const string ServerBasics2008Sp1R2 = "ServerBasics2008Sp1R2.sql";
        private const string ServerBasics2017 = "ServerBasics2017.sql";
        private const string CollectionPermissions2000 = "CollectionPermissions2000.sql";
        private const string CollectionPermissions2008 = "CollectionPermissions2008.sql";
        private const string MetadataPermissions = "MetadataPermissions.sql";
        private const string MinimumPermissions = "MinimumPermissions.sql";
        private const string CollectionPermissionsMaster = "CollectionPermissionsMaster.sql";
        private const string ServerDetails2000 = "ServerDetails2000.sql";
        private const string ServerDetails2005 = "ServerDetails2005.sql";
        private const string ServerDetails2012 = "ServerDetails2012.sql";
        private const string ServerOverview2000 = "ServerOverview2000.sql";
        private const string ServerOverview2005 = "ServerOverview2005.sql";
        private const string ServerOverview2012 = "ServerOverview2012.sql";
        private const string ServerOverview2008 = "ServerOverview2008.sql";
        private const string ServerOverview2008Sp1R2 = "ServerOverview2008Sp1R2.sql";
        private const string ServerOverview2017 = "ServerOverview2017.sql";
        private const string Services2000 = "Services2000.sql";
        private const string Services2005 = "Services2005.sql";
        private const string Services2008 = "Services2008.sql";
        private const string Services2008Sp1R2 = "Services2008Sp1R2.sql";
        private const string ServicesServiceControlAccess2000 = "ServicesServiceControlAccess2000.sql";
        private const string ServicesServiceControlAccess2005 = "ServicesServiceControlAccess2005.sql";
        private const string ServicesServiceControlAccess2008 = "ServicesServiceControlAccess2008.sql";
        private const string ServicesServiceControlAccess2008Sp1R2 = "ServicesServiceControlAccess2008Sp1R2.sql";
        private const string SessionCount2000 = "SessionCount.sql";
        private const string SessionCount2005 = "SessionCount2005.sql";
        private const string SessionDetails2000 = "SessionDetails2000.sql";
        private const string SessionDetails2005 = "SessionDetails2005.sql";
        private const string SessionDetails2014sp2 = "SessionDetails2014sp2.sql";
        private const string SessionDetailsTrace2000 = "SessionDetailsTrace2000.sql";
        private const string SessionDetailsTrace2005 = "SessionDetailsTrace2005.sql";
        private const string Sessions2000 = "Sessions2000.sql";
        private const string Sessions2005 = "Sessions2005.sql";
        // SQLDM 10.2.2 (Varun Chopra) SQLDM-27231 Need to include Deferred Deallocations in the calculation of tempdb session space usage
        private const string Sessions2014 = "Sessions2014.sql";
        private const string Session2014sp2 = "Sessions2014sp2.sql";
        private const string SessionSummary2000 = "SessionSummary2000.sql";
        private const string SessionSummary2005 = "SessionSummary2005.sql";
        private const string StopSessionDetailsTrace2000 = "StopSessionDetailsTrace2000.sql";
        private const string StopSessionDetailsTrace2005 = "StopSessionDetailsTrace2005.sql";
        private const string TableDetails2000 = "TableDetails.sql";
        private const string TableDetails2005 = "TableDetails.sql";
        private const string TableGrowth2000 = "TableGrowth2000.sql";
        private const string TableGrowth2005 = "TableGrowth2005.sql";
        private const string TableSummary2000 = "TableSummary2000.sql";
        private const string TableSummary2005 = "TableSummary2005.sql";
        private const string TempdbSummary2005 = "TempdbSummary2005.sql";
        private const string UpdateStatistics2000 = "UpdateStatistics.sql";
        private const string UpdateStatistics2005 = "UpdateStatistics.sql";
        // UpdateStatistics Permissions Batch
        private const string UpdateStatisticsPermissions2000 = "UpdateStatisticsPermissions.sql";
        //Fragmentation Permissions Batch
        private const string FragmentationsPermissions = "Fragmentation2005Permissions.sql";
        //Reindex Permissions Batch
        private const string ReindexPermissions2005 = "ReindexPermissions.sql";
        private const string MirrorMonitoringRealtime2000 = "MirrorMonitoringRealtime2005.sql";
        private const string MirrorMonitoringRealtime2005 = "MirrorMonitoringRealtime2005.sql";
        private const string MirrorMonitoringHistory2005 = "MirrorMonitoringHistory2005.sql";
        private const string MirrorMonitoringRealtime2008 = "MirrorMonitoringRealtime2005.sql";
        private const string MirrorDetails2005 = "MirroredDatabaseScheduled2005.sql";
        private const string MirrorMetricsUpdate2005 = "MirrorMetricsUpdate2005.sql";
        private const string MirroringPartnerAction2005 = "MirroringPartnerAction.sql";
        private const string MirroringPartnerActionPermissions2005 = "MirroringPartnerActionPermissions.sql";
        private const string DistributorDetails2000 = "ReplicationDistributorDetails2000.sql";
        private const string DistributorDetails2005 = "ReplicationDistributorDetails2005.sql";
        //SQLDM-30114
        private const string DistributorDetails2016 = "ReplicationDistributorDetails2016.sql";
        private const string DistributorDetailsPermissions2000 = "ReplicationDistributorDetailsPermissions.sql";
        private const string ReplicationSubscriberDetails2005 = "ReplicationSubscriberDetails2005.sql";
        private const string WaitStatistics2005 = "WaitStatistics.sql";
        private const string WmiConfigurationTest2000 = "WmiConfigurationTest.sql";
        private const string WmiConfigurationTest2005 = "WmiConfigurationTest.sql";
        
        //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added names for new batches.
        private const string Configuration2012 = "Configuration2012.sql";
        private const string DisabledIndexes2005 = "DisabledIndexes2005.sql";
        private const string DatabaseNames2000 = "DatabaseNames2000.sql";
        private const string IndexContention2005 = "IndexContention2005.sql";
        private const string HighIndexUpdates2005 = "HighIndexUpdates2005.sql";
        private const string FragmentedIndexes2005 = "FragmentedIndexes2005.sql";
        private const string OverlappingIndexes2005 = "OverlappingIndexes2005.sql";
        private const string OverlappingIndexes2008 = "OverlappingIndexes2008.sql";
        private const string GetWorstFillFactorIndexes2005 = "GetWorstFillFactorIndexes2005.sql";
        private const string BackupAndRecovery2005 = "BackupAndRecovery2005.sql";
        private const string OutOfDateStats2005 = "OutOfDateStats2005.sql";
        private const string SQLModuleOptions2005 = "SQLModuleOptions2005.sql";
        private const string DBSecurity2005 = "DBSecurity2005.sql";
        private const string GetMasterFiles2005 = "GetMasterFiles2005.sql";
        private const string NUMANodeCounters2005 = "NUMANodeCounters2005.sql";
        private const string QueryPlanEstRows2005 = "QueryPlanEstRows2005.sql";
        private const string SampleServerResources2005 = "SampleServerResources2005.sql";
        private const string GetAdhocCachedPlanBytes2005 = "GetAdhocCachedPlanBytes2005.sql";
        private const string GetLockedPageKB2008 = "GetLockedPageKB2008.sql";
        private const string GetLockedPageKB2005 = "GetLockedPageKB2005.sql";
        private const string ServerConfiguration2005 = "ServerConfiguration2005.sql";
        private const string WaitingBatches2005 = "WaitingBatches2005.sql";
        private const string DatabaseRanking2005 = "DatabaseRanking2005.sql";
        //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added names for new batches.
        //START: SQLdm 10.0 (Srishti Purohit) (DependentObjectDialog) -- Added properties for new probes to get depend object for links.
        private const string GetTableDependentObjects = "DependentObjectsPrescriptiveRecommendation.sql";
        //END: SQLdm 10.0 (Srishti Purohit) (DependentObjectDialog) -- Added properties for new probes to get depend object for links.
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I23 Adding 
        private const string NonIncrementalColumnStat2014 = "NonIncrementalColumnStatOnPartitionedTable2014.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I23 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I25, 26, 28 Adding 
        private const string HashIndex2014 = "HashIndex2014.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I25, 26, 28 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q39,Q40,Q41, Q42 Adding 
        private const string QueryStore2016 = "QueryStore2016.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q39,Q40,Q41, Q42 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q37,Q38,SDR-M31,SDR-M32, D23 Adding 
        private const string ServerConfiguration2014 = "ServerConfiguration2014.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q37,Q38,SDR-M31,SDR-M32, D23 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q37,Q38,SDR-M31,SDR-M32, D23, R8 Adding 
        private const string ServerConfiguration2016 = "ServerConfiguration2016.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q37,Q38,SDR-M31,SDR-M32, D23, R8 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I29 Adding 
        private const string RarelyUsedIndexOnInMemoryTable2014 = "RarelyUsedIndexOnInMemoryTable2014.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I29 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50 Adding 
        private const string QueryAnalyzer2016 = "QueryAnalyzer2016.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I30 Adding 
        private const string ColumnStoreIndex2016 = "ColumnStoreIndex2016.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I30 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I31 Adding 
        private const string FilteredIndex2008 = "FilteredIndex2008.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I31 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q43 Adding 
        private const string HighCPUTimeProcedure2016 = "HighCPUTimeProcedure2016.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-Q43 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I24 Adding 
        private const string LargeTableStats2008 = "LargeTableStats2008.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-I24 Adding 
        //START: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-M33 Adding Batch
        private const string BufferPoolExtIO2014 = "BufferPoolExtIO2014.sql";
        //END: SQLdm 10.0  Srishti Purohit - New Recommendations - SDR-M33 Adding Batch

        private const string AzureSQLMetrics = "AzureSQLMetrics.sql";       //M1
        private const string CloudSQLDatabases = "CloudSQLDatabases.sql";       //M1
        private const string AzureServiceTierChanged = "AzureServiceTierChanged.sql";       //
        private const string AzureElasticPools = "AzureElasticPool.sql";
        
        #endregion

        #region fields

        private static Logger LOG = Logger.GetLogger("BatchFinder");

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        private static string GetBatch(string BatchName)
        {
            return GetBatch(BatchName, false);            
        }

        /// <summary>
        /// SQLdm 10.0 (vineet kumar) - Added this method to make it accessible to Nunit test cases
        /// </summary>
        /// <param name="BatchName"></param>
        /// <returns></returns>
        internal static string GetBatchForTest(string BatchName)
        {
            return GetBatch(BatchName);
        }

        private static string GetBatch(string BatchName, bool suppressCopyright)
        {
            using (LOG.DebugCall("GetBatch"))
            {
                try
                {
                    LOG.Debug(string.Format("Fetching batch: {0}", BatchName));
                    return new BatchResourceReader().GetBatch(BatchName, suppressCopyright);
                }
                catch (Exception e)
                {
                    LOG.Error("Unable to build batch " + BatchName, e);
                    throw e;
                }
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Active Waits
        internal static string ActiveWaits(ServerVersion ver, int? cloudProviderId, bool isSysadminUser = false)
        {
            if (ver.Major < 9)
                return null;

            if (ver.Major < 11)
                return GetBatch(ActiveWaits2005);
            else
            {
                if (isSysadminUser)
                {
                    return GetBatch(GetBatchForCloud(ActiveWaits2012, cloudProviderId));
                }
                else
                {
                    return GetBatch(GetBatchForCloud(ActiveWaits2012EXN, cloudProviderId));
                }
            }
        }

        //SQLdm 10.3 (Tushar)--Support of LINQ assembly for active waits.
        /// <summary>
        /// SQLdm 10.3 (Tushar)--This method returns read query batch for LINQ assembly supported extended events.
        /// </summary>
        /// <returns></returns>
        internal static string ActiveWaitsReadEX(int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Active Waits
            return GetBatch(GetBatchForCloud(ActiveWaitsRead2012EX, cloudProviderId));
        }

        /// <summary>
        /// SQLdm 10.4 (Varun)--This method returns read query batch for reading waits data from Query Store.
        /// </summary>
        /// <returns></returns>
        internal static string ActiveWaitsReadQs(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 14 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId ||
                cloudProviderId == Constants.MicrosoftAzureId) // SQL Server 2017+
            {
                return GetBatch(GetBatchForCloud(ActiveWaitsRead2017Qs, cloudProviderId));
            }
            return string.Empty;
        }

        /// <summary>
        /// SQLdm 10.3 (Tushar)--This method returns write query batch for LINQ assembly supported extended events.
        /// </summary>
        /// <returns></returns>
        internal static string ActiveWaitsWriteEX()
        {
            return GetBatch(ActiveWaitsWrite2012EX);
        }

        #region Activity Monitor

        // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
        //START SQLdm 9.1 (Ankit Srivastava) - Activity Monitor with extended events - batch finder methods for new batches
        internal static string ActivityMonitorEx(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(ActivityMonitor2012EX, cloudProviderId));
            }
            else
                return String.Empty;

        }

        internal static string ActivityMonitorStopEx(ServerVersion ver,int ?cloudProviderId)
        {
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
                if(cloudProviderId==2 ||cloudProviderId==5)
                    return GetBatch(GetBatchForCloud(ActivityMonitorStopEX, cloudProviderId), true);

                return GetBatch(ActivityMonitorStopEX, true);
            }
            else
                return String.Empty;
        }

        internal static string ActivityMonitorReadEx(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(ActivityMonitorRead2012EX, cloudProviderId), true);
            }
            else
                return String.Empty;
        }

        internal static string ActivityMonitorRestartEx(ServerVersion ver)
        {
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(ActivityMonitorRestart2012EX, true);
            }
            else
                return string.Empty;
        }

        internal static string ActivityMonitorDeadlocksEx(ServerVersion ver,int ?cloudProviderId)
        {
           
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
                if(cloudProviderId==2||cloudProviderId==5)
                   return GetBatch(GetBatchForCloud(ActivityMonitorDeadlocks2012EX,cloudProviderId), true);
                return GetBatch(ActivityMonitorDeadlocks2012EX, true);
                
            }
            else
                return String.Empty;
        }

        
        internal static string ActivityMonitorAutogrowEx(ServerVersion ver ,int ?cloudProviderId)
        {
            
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
             
                if(cloudProviderId==2)
                    return String.Empty;
                if(cloudProviderId==5)
                    return GetBatch(GetBatchForCloud(ActivityMonitorAutogrow2012EX, cloudProviderId),true);
                return GetBatch(ActivityMonitorAutogrow2012EX, true);
            }
            else
                return String.Empty;
        }

       
        internal static string ActivityMonitorBlockingEx(ServerVersion ver, int? cloudProviderId)
        {
       
            if (ver.Major > 10) // Use same batch for SQL Server 2012 and above
            {
               
                if(cloudProviderId==2||cloudProviderId==5)
                   return GetBatch(GetBatchForCloud(ActivityMonitorBlocking2012EX,cloudProviderId), true);
                return GetBatch(ActivityMonitorBlocking2012EX, true);
            }
            else// Not permitted for SQL 2008 and below
                return String.Empty;

        }
        //END SQLdm 9.1 (Ankit Srivastava) - Activity Monitor with extended events - batch finder methods for new batches

        /// <summary>
        /// SQL batch for activity profiler
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="cloudProviderId"></param>
        /// <returns></returns>
        internal static string ActivityMonitor(ServerVersion ver, int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ActivityMonitor2005, cloudProviderId));
            }
            else
            {
                return GetBatch(ActivityMonitor2000);
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
        internal static string ActivityMonitorStop(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ActivityMonitorStop2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(ActivityMonitorStop2000, true);
            }
        }

        internal static string ActivityMonitorRead(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(ActivityMonitorRead2005, true);
            }
            else
            {
                return GetBatch(ActivityMonitorRead2000, true);
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
        internal static string ActivityMonitorRestart(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ActivityMonitorRestart2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(ActivityMonitorRestart2000, true);
            }
        }

        internal static string ActivityMonitorTraceDeadlocks(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(ActivityMonitorTraceDeadlocks2005, true);
            }
            else
            {
                // Not permitted for SQL 2000
                return null;
            }
        }

        internal static string ActivityMonitorBlocking(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(ActivityMonitorBlocking2005, true);
            }
            else
            {
                // Not permitted for SQL 2000
                return null;
            }
        }
        #endregion

        internal static string AgentJobHistory(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(AgentJobHistory2005);
            }
            else
            {
                return GetBatch(AgentJobHistory2000);
            }
        }

        // Start ID: M1
        /// <summary>
        /// Fetch batch file and return result in string format  
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        internal static string AzureSQLMetric()
        {
            return GetBatch(AzureSQLMetrics);
        }

        internal static string AzureDatabaseServiceTierChanged()
        {
            return GetBatch(AzureServiceTierChanged);
        }

        internal static string CloudSQLDatabase()
        {
            return GetBatch(CloudSQLDatabases);
        }
        // End ID: M1

        internal static string AzureElasticPool()
        {
            return GetBatch(AzureElasticPools);
        }
        internal static string AgentJobSummary(ServerVersion ver)
        {
            if (ver.Major >= 10) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(ver.IsGreaterThanSql2008Sp1R2() ? AgentJobSummary2008Sp1R2 : AgentJobSummary2008);
            }
            else
            {
                return GetBatch(AgentJobSummary2000);
            }
        }

        internal static string AlwaysOnToplogySummary()
        {
            return GetBatch(AlwaysOnToplogySummary2012);
        }

        internal static string AlwaysOnToplogyDetail()
        {
            return GetBatch(AlwaysOnToplogyDetail2012);
        }

        internal static string AlwaysOnTopology(ServerVersion ver)
        {
            if (ver.Major >= 11) //This is only supported for SQL Server 2012+
            {
                return GetBatch(AlwaysOnTopology2012);
            }
            else
                return null;
        }

        internal static string AlwaysOnStatistics(ServerVersion ver)
        {
            if (ver.Major >= 11) //This is only supported for SQL Server 2012+
            {
                return GetBatch(AlwaysOnStatistics2012);
            }
            else
                return null;
        }

        internal static string BackupHistoryFull(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(BackupHistoryFull2005);
            }
            else
            {
                return GetBatch(BackupHistoryFull2000);
            }
        }

        internal static string BackupHistorySmall(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(BackupHistorySmall2005);
            }
            else
            {
                return GetBatch(BackupHistorySmall2000);
            }
        }


        internal static string FailedJobs(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(FailedJobs2005, true);
            }
            else
            {
                return GetBatch(FailedJobs2000, true);
            }
        }

        internal static string FailedJobSteps(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(FailedJobSteps2005, true);
            }
            else
            {
                return GetBatch(FailedJobSteps2000, true);
            }
        }

        internal static string Fragmentation(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(Fragmentation2005, cloudProviderId), false);
            }
            else
            {
                return GetBatch(Fragmentation2000, false);
            }
        }


        internal static string FragmentationWorkload(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(FragmentationWorkload2005, cloudProviderId), false);
            }
            else
            {
                return GetBatch(FragmentationWorkload2000, false);
            }
        }

        internal static string CompletedJobs(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(CompletedJobs2005, true);
            }
            else
            {
                return GetBatch(CompletedJobs2000, true);
            }
        }

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new param and calling batch according to cloud privider id
        internal static string FileActivity(ServerVersion ver, int? cloudProviderId = null)
        {
            if (ver.Major >= 10) // SQL Server 2008+
            {
                return ver.IsGreaterThanSql2008Sp1R2()
                           ? GetBatch(
                               cloudProviderId == null
                                   ? FileActivity2008Sp1R2
                                   : FileActivity2008Sp1R2.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql")
                           : GetBatch(
                               cloudProviderId == null
                                   ? FileActivity2008
                                   : FileActivity2008.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else if (ver.Major >= 9) // SQL Server 2005+
            {
                return GetBatch(cloudProviderId == null ? FileActivity2005 : FileActivity2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                        
                //return GetBatch(FileActivity2005);
            }
            else
            {
                return GetBatch(cloudProviderId == null ? FileActivity2000 : FileActivity2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                        
                //return GetBatch(FileActivity2000);
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new param and calling batch according to cloud privider id

        internal static string BlockingCheck(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.IsGreaterThanSql2014sp2())
            {
                return GetBatch(GetBatchForCloud(BlockingCheck2014sp2, cloudProviderId), true);
            }
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(BlockingCheck2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(BlockingCheck2000, true);
            }
        }

        internal static string Configuration(ServerVersion ver, int? cloudProviderId)
        {
            //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- use Configuration2012.sql for version 2012 and above
            if (ver.Major >= 11)
            {
                return GetBatch(GetBatchForCloud(Configuration2012, cloudProviderId));
            }
            //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- use Configuration2012.sql for version 2012 and above
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(Configuration2005);
            }
            else
            {
                return GetBatch(Configuration2000);
            }
        }

        //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added properties for new probes.
        internal static string IndexContention(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(IndexContention2005);
            }
            else
            {
                return null;
            }
        }

        //START: SQLdm 10.0 (Srishti Purohit) (DependentObjectDialog) -- Added properties for new probes to get depend object for links.
        internal static string DependentObjectBatch(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetTableDependentObjects);
            }
            else
            {
                return null;
            }
        }

        internal static string OverlappingIndexes(ServerVersion ver)
        {
            if (ver.Major >= 10)
            {
                return GetBatch(OverlappingIndexes2008);
            }
            if (ver.Major >= 9)
            {
                return GetBatch(OverlappingIndexes2005);
            }
            else
            {
                return null;
            }
        }

        internal static string DisabledIndexes(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(DisabledIndexes2005);
            }
            else
            {
                return null;
            }
        }

        internal static string DatabaseNames(ServerVersion ver, int? cloudProviderId)
        {
            return GetBatch(GetBatchForCloud(DatabaseNames2000, cloudProviderId));
        }


        internal static string FragmentedIndexes(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(FragmentedIndexes2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string GetWorstFillFactorIndexes(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(GetWorstFillFactorIndexes2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string HighIndexUpdates(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(HighIndexUpdates2005);
            }
            else
            {
                return null;
            }
        }

        internal static string BackupAndRecovery(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(BackupAndRecovery2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string OutOfDateStats(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(OutOfDateStats2005);
            }
            else
            {
                return null;
            }
        }

        internal static string SQLModuleOptions(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(SQLModuleOptions2005);
            }
            else
            {
                return null;
            }
        }

        internal static string DBSecurity(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(DBSecurity2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string GetMasterFiles(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(GetMasterFiles2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string NUMANodeCounters(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(NUMANodeCounters2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }


        internal static string QueryPlanEstRows(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(QueryPlanEstRows2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string SampleServerResources(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(SampleServerResources2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string GetAdhocCachedPlanBytes(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(GetAdhocCachedPlanBytes2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string GetLockedPageKB(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 10)
            {
                return GetBatch(GetBatchForCloud(GetLockedPageKB2008, cloudProviderId));
            }
            if (ver.Major >= 9)
            {
                return GetBatch(GetLockedPageKB2005);
            }
            else
            {
                return null;
            }
        }

        internal static string ServerConfiguration(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 13)
            {
                return GetBatch(GetBatchForCloud(ServerConfiguration2016, cloudProviderId));
            }
            else if (ver.Major >= 12)
            {
                return GetBatch(GetBatchForCloud(ServerConfiguration2014, cloudProviderId));
            }
            else if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(ServerConfiguration2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string WaitingBatches(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(WaitingBatches2005, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        internal static string DatabaseRanking(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(DatabaseRanking2005);
            }
            else
            {
                return null;
            }
        }

        //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added properties for new probes.


        internal static string CustomCounterOS(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(CustomCounterOS2005, true);
            }
            else
            {
                return GetBatch(CustomCounterOS2000, true);
            }
        }

        internal static string CustomCounterSQL(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(GetBatchForCloud(CustomCounterSQL2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(GetBatchForCloud(CustomCounterSQL2005, cloudProviderId), true);
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
        internal static string NonIncrementalColumnStats(ServerVersion ver)
        {
            if (ver.Major >= 12)
            {
                return GetBatch(NonIncrementalColumnStat2014);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I25, 26,28 Adding new analyzer 
        internal static string HashIndex(ServerVersion ver)
        {
            if (ver.Major >= 12)
            {
                return GetBatch(HashIndex2014);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I25, 26,28 Adding new analyzer 
        internal static string QueryStore(ServerVersion ver)
        {
            if (ver.Major >= 13)
            {
                return GetBatch(QueryStore2016);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I29 Adding new batch 
        internal static string RarelyUsedIndexOnInMemoryTable(ServerVersion ver)
        {
            if (ver.Major >= 12)
            {
                return GetBatch(RarelyUsedIndexOnInMemoryTable2014);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50 Adding new batch 
        internal static string QueryAnalyzer(ServerVersion ver)
        {
            if (ver.Major >= 13)
            {
                return GetBatch(QueryAnalyzer2016);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I30 Adding new analyzer 
        internal static string ColumnStoreIndex(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 13)
            {
                return GetBatch(GetBatchForCloud(ColumnStoreIndex2016, cloudProviderId));
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I31 Adding new analyzer 
        internal static string FilteredColumnNotInKeyOfFilteredIndex(ServerVersion ver)
        {
            if (ver.Major >= 10)
            {
                return GetBatch(FilteredIndex2008);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q43 Adding new batch 
        internal static string HighCPUTimeProcedure(ServerVersion ver)
        {
            if (ver.Major >= 13)
            {
                return GetBatch(HighCPUTimeProcedure2016);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I24 Adding new batch 
        internal static string LargeTableStats(ServerVersion ver)
        {
            //SQL Server build level is SQL Server 2008 R2 SP2+, SQL Server 2012 SP1+, or newer
            if ((ver.Major == 10 && ver.Minor == 50 && ver.Build >= 4000) || (ver.Major == 11 && ver.Build >= 3000) || (ver.Major >= 12))
            {
                return GetBatch(LargeTableStats2008);
            }
            else
            {
                return null;
            }
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-M33 Adding new Batch 
        internal static string BufferPoolExtIO(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 12)
            {
                return GetBatch(GetBatchForCloud(BufferPoolExtIO2014, cloudProviderId));
            }
            else
            {
                return null;
            }
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - New batch finder method
        internal static string FileGroup(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(FileGroup2005, true);
            }
            else
            {
                return GetBatch(FileGroup2000, true);
            }
        }

        internal static string DiskDriveStatistics(ServerVersion ver)
        {
            return GetBatch(DiskDriveStatisticsBatch, true);
        }
        //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - New batch finder method

        internal static string DatabaseConfiguration(ServerVersion ver, int? cloudProviderId)
        {
            //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- use new batch for 2012
            if (ver.Major >= 11)
            {
                return GetBatch(GetBatchForCloud(DatabaseConfiguration2012, cloudProviderId));
            }
            //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- use new batch for 2012
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(DatabaseConfiguration2005, cloudProviderId));
            }
            else
            {
                return GetBatch(DatabaseConfiguration2000);
            }
        }

        internal static string DatabaseFiles(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(DatabaseFiles2005, true);
            }
            else
            {
                return GetBatch(DatabaseFiles2000, true);
            }
        }

        internal static string DatabaseSize(ServerVersion serverVersion, int? cloudProviderId)
        {
            if (serverVersion.Major >= 11)
            {
                // Use batch for SQL Server 2012
                return GetBatch(GetBatchForCloud(DatabaseSize2012, cloudProviderId), true);
            }

            if (serverVersion.Major >= 9)
            {
                // Use same batch for SQL Server 2005/2008
                return GetBatch(GetBatchForCloud(DatabaseSize2005, cloudProviderId), true);
            }

            return GetBatch(DatabaseSize2000, true);
        }


        internal static string DatabaseSummary(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            if (serverVersion.Major >= 11)
            {
                // Use batch for SQL Server 2012
                return GetBatch(GetBatchForCloud(DatabaseSummary2012, cloudProviderId), true);
            }

            if (serverVersion.Major >= 9)
            {
                // Use same batch for SQL Server 2005/2008
                return GetBatch(DatabaseSummary2005);
            }

            return GetBatch(DatabaseSummary2000);
        }

        internal static string DiskDrives(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(DiskDrives2005, true);
            }
            else
            {
                return GetBatch(DiskDrives2000, true);
            }
        }

        internal static string DiskDriveHeader(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(DiskDriveHeader2005);
            }
            else
            {
                return GetBatch(DiskDriveHeader2000);
            }
        }

        internal static string DiskSize(ServerVersion ver)
        {
            if (ver.Major >= 10) // Use same batch for SQL Server 2008+
            {
                return String.Format(
                    ver.IsGreaterThanSql2008Sp1R2() ? GetBatch(DiskSize2008Sp1R2, true) : GetBatch(DiskSize2008, true),
                    ver.MasterDatabaseCompatibility > 7 ? " collate database_default " : "");
            }
            else if (ver.Major >= 9) // Use batch for SQL Server 2005
            {
                return String.Format(GetBatch(DiskSize2005, true), ver.MasterDatabaseCompatibility > 7 ? " collate database_default " : "");
            }
            else
            {
                return String.Format(GetBatch(DiskSize2000, true), ver.MasterDatabaseCompatibility > 7 ? " collate database_default " : "");
            }
        }

        internal static string DistributorQueue(ServerVersion ver)
        {
            //SQLDM-30114
            if (ver.Major == 13 && ver.Build >= 5216)
            {
                return GetBatch(DistributorQueue2016);
            }
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(DistributorQueue2005);
            }
            else
            {
                return GetBatch(DistributorQueue2000);
            }
        }

        internal static string DropPlansFromCache(ServerVersion ver)
        {
            if (ver.Major >= 10) // This is a SQL 2008 only batch
            {
                return GetBatch(DropPlansFromCache2008);
            }
            else
            {
                return null;
            }
        }

        internal static string DistributorDetailsPermissions(ServerVersion ver)
        {
            return GetBatch(DistributorDetailsPermissions2000);
        }

        internal static string DistributorDetails(ServerVersion ver)
        {
            //SQLDM-30114
            if (ver.Major == 13 && ver.Build >= 5216)
            {
                return GetBatch(DistributorDetails2016);
            }
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(DistributorDetails2005);
            }
            else
            {
                return GetBatch(DistributorDetails2000);
            }
        }

        internal static string DTCStatus(ServerVersion ver, bool hasServiceControlExecuteAccess, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                if (hasServiceControlExecuteAccess && cloudProviderId != Constants.MicrosoftAzureId)
                {
                    return GetBatch(DtcStatusServiceControlAccess2005);
                }
                return GetBatch(GetBatchForCloud(DTCStatus2005, cloudProviderId));
            }
            else
            {
                return GetBatch(hasServiceControlExecuteAccess ? DtcStatusServiceControlAccess2000 : DTCStatus2000);
            }
        }

        internal static string FailedJobVariables(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(FailedJobVariables2005);
            }
            else
            {
                return GetBatch(FailedJobVariables2000);
            }
        }


        internal static string FullTextCatalogs(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(FullTextCatalogs2005);
            }
            else
            {
                return GetBatch(FullTextCatalogs2000);
            }
        }

        internal static string FullTextColumns(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(FullTextColumns2005);
            }
            else
            {
                return GetBatch(FullTextColumns2000);
            }
        }

        internal static string FullTextSearchService(ServerVersion ver, bool hasServiceControlExecuteAccess)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(hasServiceControlExecuteAccess ? FullTextSearchServiceServiceControl2005 : FullTextSearchService2005);
            }
            else
            {
                return GetBatch(hasServiceControlExecuteAccess ? FullTextSearchServiceServiceControl2000 : FullTextSearchService2000);
            }
        }

        internal static string FullTextTables(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(FullTextTables2005);
            }
            else
            {
                return GetBatch(FullTextTables2000);
            }
        }



        internal static string IndexStatistics(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(IndexStatistics2005);
            }
            else
            {
                return GetBatch(IndexStatistics2000);
            }
        }

        internal static string LockCounterStatistics(ServerVersion ver, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(cloudProviderId == null ? LockCounterStatistics2005 : LockCounterStatistics2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                
            }
            else
            {
                return GetBatch(cloudProviderId == null ? LockCounterStatistics2000 : LockCounterStatistics2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                
            }
        }

        internal static string LockDetails(ServerVersion ver, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        {
            if (ver.IsGreaterThanSql2014sp2())
            {
                return GetBatch(
                    cloudProviderId == null
                        ? LockDetails2014sp2
                        : LockDetails2014sp2.Replace(".sql", "_" + cloudProviderId) + ".sql");
            }
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(cloudProviderId == null ? LockDetails2005 : LockDetails2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                
                //return GetBatch(LockDetails2005);
            }
            else
            {
                return GetBatch(cloudProviderId == null ? LockDetails2000 : LockDetails2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                
                //return GetBatch(LockDetails2000);
            }
        }

        internal static string LogList(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(LogList2005);
            }
            else
            {
                return GetBatch(LogList2000);
            }
        }

        internal static string LogScan(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(cloudProviderId == null ? LogScan2005 : LogScan2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else
            {
                return GetBatch(LogScan2000);
            }
        }


        internal static string LongJobs(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(LongJobs2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(LongJobs2000, true);
            }
        }


        internal static string LongJobsWithSteps(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(LongJobsWithSteps2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(LongJobsWithSteps2000, true);
            }
        }

        internal static string LongJobsByPercent(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(LongJobsByPercent2005, true);
            }
            else
            {
                return GetBatch(LongJobsByPercent2000, true);
            }
        }

        internal static string LongJobsByRuntime(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(LongJobsByRuntime2005, true);
            }
            else
            {
                return GetBatch(LongJobsByRuntime2000, true);
            }
        }

        internal static string MirrorMonitoringHistory(ServerVersion ver)
        {
            return GetBatch(MirrorMonitoringHistory2005);
        }

        internal static string MirroringPartnerAction(ServerVersion ver)
        {
            return GetBatch(MirroringPartnerAction2005);
        }

        internal static string MirroringPartnerActionPermissions(ServerVersion ver, int? cloudProviderId)
        {
            return GetBatch(GetBatchForCloud(MirroringPartnerActionPermissions2005, cloudProviderId));
        }

        internal static string MirrorMonitoringRealtime(ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                if (ver.Major == 10)
                {
                    return GetBatch(MirrorMonitoringRealtime2005);
                }
                else
                {
                    return GetBatch(MirrorMonitoringRealtime2008);
                }
            }
            else
            {
                return GetBatch(MirrorMonitoringRealtime2000);
            }
        }

        internal static string MirrorDetails(ServerVersion ver)
        {
            return GetBatch(MirrorDetails2005);
        }

        internal static string MirrorMetricsUpdate(ServerVersion ver)
        {
            return GetBatch(MirrorMetricsUpdate2005);
        }


        internal static string Memory(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major < 9)
                return GetBatch(Memory2000);
            else if (ver.Major < 11)
                return GetBatch(Memory2005);
            else
                return GetBatch(GetBatchForCloud(Memory2012, cloudProviderId));
        }

        internal static string DatabaseLastBakcupDateTime()
        {
            return GetBatch(DatabaseLastBackupDateTime);
        }

        internal static string OSMetrics(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(OSMetrics2005, cloudProviderId));
            }
            else
            {
                return GetBatch(OSMetrics2000);
            }
        }

        internal static string OldestOpenTransaction(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(OldestOpenTransaction2005, cloudProviderId));
            }
            else
            {
                return GetBatch(OldestOpenTransaction2000);
            }
        }


        internal static string PinnedTables(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(PinnedTables2005);
            }
            else
            {
                return GetBatch(PinnedTables2000);
            }
        }

        internal static string ProcedureCache(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ProcedureCache2005, cloudProviderId));
            }
            else
            {
                return GetBatch(ProcedureCache2000);
            }
        }

        internal static string ProcedureCacheList(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ProcedureCacheList2005, cloudProviderId));
            }
            else
            {
                return GetBatch(ProcedureCacheList2000);
            }
        }

        /// <summary>
        /// Creates a new trace
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        internal static string QueryMonitor(ServerVersion ver, int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(QueryMonitor2005, cloudProviderId));
            }
            else
            {
                return GetBatch(QueryMonitor2000);
            }
        }

        /// <summary>
        /// Creates a new extended event session  //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>
        internal static string QueryMonitorEX(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
            if (serverVersion.Major >= 11) // Use for SQL Server 2012
            {
                return GetBatch(GetBatchForCloud(QueryMonitor2012EX, cloudProviderId));
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(GetBatchForCloud(QueryMonitor2008EX, cloudProviderId));
            }
            else return String.Empty;
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) - Batch to write last filename, record count
        /// </summary>
        internal static string QueryMonitorWriteEX()
        {
            return GetBatch(QueryMonitorWrite2012EX);
        }

        internal static string GetFilteredMonitoredDatabasesAzure()
        {
            return GetBatch(FilteredMonitoredDatabasesAzure);
        }

        /// <summary>
        /// SQLdm 10.3 (Tushar) - Batch to write last filename, record count
        /// </summary>
        /// <returns></returns>
        internal static string ActivityMonitorWriteEX()
        {
            return GetBatch(ActivityMonitorWrite2012EX);
        }

        /// <summary>
        /// restarts the extended event session  //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <param name="cloudProviderId"></param>
        /// <returns></returns>
        internal static string QueryMonitorRestartEX(ServerVersion serverVersion, int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012
            {
                return GetBatch(GetBatchForCloud(QueryMonitorRestart2012EX, cloudProviderId));
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(GetBatchForCloud(QueryMonitorRestart2008EX, cloudProviderId));
            }
            else return String.Empty;
        }

        /// <summary>
        /// Reads Query Store Data from all the databases where query store is enabled
        /// </summary>
        /// <param name="serverVersion">SQL 2016+</param>
        /// <param name="cloudProviderId">Handle Cloud servers</param>
        /// <returns>Batch Value</returns>
        internal static string QueryMonitorReadQs(ServerVersion serverVersion, int? cloudProviderId)
        {
            if (serverVersion.Major >= 13 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId ||
                cloudProviderId == Constants.MicrosoftAzureId) // Use same batch for SQL Server 2016
            {
                return GetBatch(GetBatchForCloud(QueryMonitorRead2016Qs, cloudProviderId));
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// reads the extended event session XEL file  //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <param name="cloudProviderId"></param>
        /// <returns></returns>
        internal static string QueryMonitorReadEX(ServerVersion serverVersion, int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012
            {
                return GetBatch(GetBatchForCloud(QueryMonitorRead2012EX, cloudProviderId), true);
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(QueryMonitorRead2008EX, true);
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(QueryMonitorRead2008EX, true);
            }
            else return String.Empty;
        }

        internal static string QueryMonitorCheckSettings(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorCheckSettings2005, true);
            }
            else
            {
                return GetBatch(QueryMonitorCheckSettings2000, true);
            }
        }

        internal static string QueryMonitorRead(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorRead2005, true);
            }
            else
            {
                return GetBatch(QueryMonitorRead2000, true);
            }
        }

        //START SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available
        internal static string QueryMonitorReadEstimatedPlan(ServerVersion ver)
        {
            if (ver.Major > 8) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorReadEstimatedPlan2005, true);
            }
            //This feature won't support SQL server version 2000 or before
            return string.Empty;
        }
        //END SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available

        //internal static string QueryMonitorNoDeclare(ServerVersion ver)
        //{
        //    if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
        //    {
        //        return GetBatch(QueryMonitorRestart2005);
        //    }
        //    else
        //    {
        //        return GetBatch(QueryMonitorRestart2000);
        //    }
        //}

        // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
        internal static string QueryMonitorRestart(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(QueryMonitorRestart2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(QueryMonitorRestart2000, true);
            }
        }

        // SQLdm 10.3 (Varun Chopra) Get Batch for Cloud based on Cloud provider id to support linux
        private static string GetBatchForCloud(string batchName, int? cloudProviderId)
        {
            if (cloudProviderId == Constants.Windows)
            {
                cloudProviderId = null;
            }
            return cloudProviderId == null ? batchName : batchName.Replace(".sql", "_" + cloudProviderId) + ".sql";
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
        internal static string QueryMonitorStop(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(QueryMonitorStop2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(QueryMonitorStop2000, true);
            }
        }

        internal static string ActivityProfilerTraceAutogrow(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005 
            {
                return GetBatch(ActivityMonitorTraceAutogrow2005, true);
            }
            else
            {
                return GetBatch(ActivityMonitorTraceAutogrow2000, true);
            }
        }


        internal static string QueryMonitorTraceBatches(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorTraceBatches2005, true);
            }
            else
            {
                return GetBatch(QueryMonitorTraceBatches2000, true);
            }
        }

        internal static string QueryMonitorTraceSingleStmt(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorTraceSingleStmt2005, true);
            }
            else
            {
                return GetBatch(QueryMonitorTraceSingleStmt2000, true);
            }
        }

        internal static string QueryMonitorTraceSP(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(QueryMonitorTraceSP2005, true);
            }
            else
            {
                return GetBatch(QueryMonitorTraceSP2000, true);
            }
        }

        /// <summary>
        /// stops the extended event session  //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>

        internal static string QueryMonitorStopEx(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(QueryMonitorStopEX2012, cloudProviderId));
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(QueryMonitorStopEX2008);
            }
            else return String.Empty;
        }

        /// <summary>
        /// Stops Query Store - Essentially drops the temporary table used for state storage
        /// </summary>
        /// <param name="serverVersion">SQL 2016+</param>
        /// <returns>Batch Value</returns>
        internal static string QueryMonitorStopQs(ServerVersion serverVersion, int? cloudProviderId)
        {
            if (serverVersion.Major >= 13 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId) // Use same batch for SQL Server 2016
            {
                return GetBatch(QueryMonitorStop2016Qs);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// gets the sql batch part of the extended event session query //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>
        internal static string QueryMonitorExtendedEventsBatches(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(QueryMonitorExtendedEventsBatches2012, cloudProviderId), true);
            }
            else
                return String.Empty; // sql_batch_completed event not available for 2008 and below
        }

        /// <summary>
        /// gets the Single statement part of the extended event session query //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>
        internal static string QueryMonitorExtendedEventsSingleStmt(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(QueryMonitorExtendedEventsSingleStmt2012, cloudProviderId), true);
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(QueryMonitorExtendedEventsSingleStmt2008, true);
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// gets the SP completed part of the the extended event session query  //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- new batch method
        /// </summary>
        /// <param name="serverVersion"></param>
        /// <returns></returns>
        internal static string QueryMonitorExtendedEventsSP(ServerVersion serverVersion, int? cloudProviderId = null)
        {
            if (serverVersion.Major >= 11) // Use same batch for SQL Server 2012 and above
            {
                return GetBatch(GetBatchForCloud(QueryMonitorExtendedEventsSP2012, cloudProviderId), true);
            }
            else if (serverVersion.Major >= 10) // Use for SQL Server 2008
            {
                return GetBatch(QueryMonitorExtendedEventsSP2008, true);
            }
            else return String.Empty;
        }


        internal static string Reindex(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(Reindex2005, cloudProviderId));
            }
            else
            {
                return GetBatch(Reindex2000);
            }
        }


        //internal static string Reorganization(ServerVersion ver)
        //{
        //    if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
        //    {
        //        return GetBatch(Reorganization2005);
        //    }
        //    else
        //    {
        //        return GetBatch(Reorganization2000);
        //    }
        //}

        internal static string ReplicationCheck(ServerVersion ver, int? cloudProviderId)
        {
            //SQLDM-30114
            if(ver.Major == 13 && ver.Build >= 5216)
            {
                return GetBatch(ReplicationCheck2016);
            }
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ReplicationCheck2005, cloudProviderId));
            }
            else
            {
                return GetBatch(ReplicationCheck2000);
            }
        }

        

        internal static string ReplicationStatus(ServerVersion ver)
        {
            if (ver.Major >= 10) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(ver.IsGreaterThanSql2008Sp1R2() ? ReplicationStatus2008Sp1R2 : ReplicationStatus2008);
            }
            else
            {
                return GetBatch(ReplicationStatus2000);
            }
        }

        internal static string ResourceCheck(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ResourceCheck2005, cloudProviderId), true);
            }
            else
            {
                return GetBatch(ResourceCheck2000, true);
            }
        }

        //internal static string Resources(ServerVersion ver)
        //{
        //    if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
        //    {
        //        return GetBatch(Resources2005);
        //    }
        //    else
        //    {
        //        return GetBatch(Resources2000);
        //    }
        //}

        internal static string RestoreHistoryFull(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(RestoreHistoryFull2005);
            }
            else
            {
                return GetBatch(RestoreHistoryFull2000);
            }
        }

        internal static string RestoreHistorySmall(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(RestoreHistorySmall2005);
            }
            else
            {
                return GetBatch(RestoreHistorySmall2000);
            }
        }

        internal static string ReadCollectionPermissions(ServerVersion ver, int? cloudProviderId = null)
        {
            // Added cases for Cloud / Linux and various SQL versions            
            if (ver.Major >= 10) // Use batch for SQL Server 2008
            {
                return GetBatch(cloudProviderId == null ? CollectionPermissions2008 : CollectionPermissions2008.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else
            {
                return GetBatch(cloudProviderId == null ? CollectionPermissions2000 : CollectionPermissions2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
        }

        internal static string ReadReplicationPermissions(ServerVersion ver, int? cloudProviderId = null)
        {
            // TODO: Add cases for Cloud / Linux and various SQL versions            
            return GetBatch(cloudProviderId == null ? ReplicationCheckPermissions : ReplicationCheckPermissions.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
        }

        internal static string ReadMinimumPermissions(ServerVersion ver, int? cloudProviderId = null)
        {
            // Added cases for Cloud / Linux and various SQL versions            
            return GetBatch(cloudProviderId == null ? MinimumPermissions : MinimumPermissions.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
        }

        internal static string ReadMasterPermissions(ServerVersion ver, int? cloudProviderId = null)
        {
            // Added cases for Cloud / Linux and various SQL versions            
            return GetBatch(CollectionPermissionsMaster);
        }

        internal static string ReadMetadataPermissions(ServerVersion ver, int? cloudProviderId = null)
        {
            // Added cases for Cloud / Linux and various SQL versions            
            return GetBatch(cloudProviderId == null ? MetadataPermissions : MetadataPermissions.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
        }
        //START: SQLdm 10.0 (Tarun Sapra)- Added a new batch for cloud instances
        internal static string ServerBasics(ServerVersion ver, int? cloudProviderId = null)
        {
            if (ver.Major >= 14) // Use batch for SQL Server 2017+
            {
                return GetBatch(cloudProviderId == null ? ServerBasics2017 : ServerBasics2017.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            if (ver.IsGreaterThanSql2008Sp1R2()) // Use batch for SQL Server 2008
            {
                return GetBatch(cloudProviderId == null ? ServerBasics2008Sp1R2 : ServerBasics2008Sp1R2.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else if (ver.Major >= 10) // Use batch for SQL Server 2008
            {
                return GetBatch(cloudProviderId == null ? ServerBasics2008 : ServerBasics2008.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {                
                return GetBatch(cloudProviderId == null ? ServerBasics2005 : ServerBasics2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else
            {
                return GetBatch(cloudProviderId == null ? ServerBasics2000 : ServerBasics2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
        }

        //END: SQLdm 10.0 (Tarun Sapra)- Added a new batch for cloud instances

        //START: SQLdm 10.0 (Tarun Sapra)- Added a new batch for cloud instances
        internal static string ServerDetails(ServerVersion ver, int? cloudProviderId = null)
        {
            if (ver.Major < 9)
                return GetBatch(cloudProviderId == null ? ServerDetails2000 : ServerDetails2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                
            else if (ver.Major < 11)
                return GetBatch(cloudProviderId == null ? ServerDetails2005 : ServerDetails2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                
            else
                return GetBatch(cloudProviderId == null ? ServerDetails2012 : ServerDetails2012.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Added a new batch for cloud instances

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param and calling batches accordingly
        internal static string ServerOverview(ServerVersion ver, int? cloudProviderId = null)
        {    
            if (ver.Major < 9)
                return GetBatch(cloudProviderId == null ? ServerOverview2000 : ServerOverview2000.Replace(".sql","_"+ cloudProviderId.ToString())+".sql");
            else if (ver.Major < 10)
                return GetBatch(cloudProviderId == null ? ServerOverview2005 : ServerOverview2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            else if (ver.Major < 11)
            {
                return ver.IsGreaterThanSql2008Sp1R2()
                           ? GetBatch(
                               cloudProviderId == null
                                   ? ServerOverview2008Sp1R2
                                   : ServerOverview2008Sp1R2.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql")
                           : GetBatch(
                               cloudProviderId == null
                                   ? ServerOverview2008
                                   : ServerOverview2008.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            }
            else if (ver.Major < 14)
                return GetBatch(cloudProviderId == null ? ServerOverview2012 : ServerOverview2012.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
            else
            return GetBatch(cloudProviderId == null ? ServerOverview2017 : ServerOverview2017.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param and calling batches accordingly

        internal static string Services(ServerVersion ver, bool hasServiceControlExecuteAccess)
        {
            if (ver.Major >= 10) // Use batch for SQL Server 2008
            {
                return ver.IsGreaterThanSql2008Sp1R2()
                           ? GetBatch(
                               hasServiceControlExecuteAccess
                                   ? ServicesServiceControlAccess2008Sp1R2
                                   : Services2008Sp1R2)
                           : GetBatch(hasServiceControlExecuteAccess ? ServicesServiceControlAccess2008 : Services2008);
            }
            else if (ver.Major >= 9) // Use batch for SQL Server 2005
            {
                return GetBatch(hasServiceControlExecuteAccess ? ServicesServiceControlAccess2005 : Services2005);
            }
            else
            {
                return GetBatch(hasServiceControlExecuteAccess ? ServicesServiceControlAccess2000 : Services2000);
            }
        }

        internal static string SessionCount(ServerVersion ver, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- Added a new param
        {
            return SessionCount(ver, "", cloudProviderId);
        }

        internal static string SessionCount(ServerVersion ver, string p1, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- Added a new param
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return String.Format(
                    GetBatch(cloudProviderId == null ? SessionCount2005 : SessionCount2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql")
                    , ",sql_handle", p1);
                //return String.Format(GetBatch(SessionCount2005, true), ",sql_handle", p1);
            }
            else
            {
                return String.Format(
                    GetBatch(cloudProviderId == null ? SessionCount2000 : SessionCount2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql")
                    , "", p1);
                //return String.Format(GetBatch(SessionCount2000, true), "", p1);
            }
        }

        internal static string SessionDetails(ServerVersion ver)
        {
            if (ver.IsGreaterThanSql2014sp2())
            {
                return GetBatch(SessionDetails2014sp2);
            }
            else if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(SessionDetails2005);
            }
            else
            {
                return GetBatch(SessionDetails2000);
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Session Details
        internal static string SessionDetailsTrace(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(SessionDetailsTrace2005, cloudProviderId));
            }
            else
            {
                return GetBatch(SessionDetailsTrace2000);
            }
        }

        internal static string Sessions(ServerVersion ver, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        {
            // SQLDM 10.2.2 (Varun Chopra) SQLDM-27231 Need to include Deferred Deallocations in the calculation of tempdb session space usage
            if (ver.Major >= 12) // Batch for SQL Server 2014 onwards
            {
                if (ver.IsGreaterThanSql2014sp2())
                {
                    return GetBatch(cloudProviderId == null ? Session2014sp2 : Session2014sp2.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
                }
                else
                {
                    return GetBatch(cloudProviderId == null ? Sessions2014 : Sessions2014.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
                }
            }
            else if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(cloudProviderId == null ? Sessions2005 : Sessions2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
                //return GetBatch(Sessions2005);
            }
            else
            {
                return GetBatch(cloudProviderId == null ? Sessions2000 : Sessions2000.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");
                //return GetBatch(Sessions2000);
            }
        }

        internal static string SessionSummary(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(SessionSummary2005, cloudProviderId));
            }
            else
            {
                return GetBatch(SessionSummary2000);
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Session Details
        internal static string StopSessionDetailsTrace(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(StopSessionDetailsTrace2005, cloudProviderId));
            }
            else
            {
                return GetBatch(StopSessionDetailsTrace2000);
            }
        }

        internal static string TableDetails(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(TableDetails2005, cloudProviderId));
            }
            else
            {
                return GetBatch(TableDetails2000);
            }
        }


        internal static string TableGrowth(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(TableGrowth2005, cloudProviderId));
            }
            else
            {
                return GetBatch(TableGrowth2000);
            }
        }

        internal static string TableSummary(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(TableSummary2005, cloudProviderId));
            }
            else
            {
                return GetBatch(TableSummary2000);
            }
        }

        internal static string TempdbSummary(ServerVersion ver, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support:Added a new param for cloud provider id
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005+ 
            {
                return GetBatch(cloudProviderId == null ? TempdbSummary2005 : TempdbSummary2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                        
                //return GetBatch(TempdbSummary2005); 
            }
            else
            {
                return null;
            }
        }

        internal static string UpdateStatistics(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(UpdateStatistics2005, cloudProviderId));
            }
            else
            {
                return GetBatch(UpdateStatistics2000);
            }
        }

        /// <summary>
        /// Returns the Permissions Batch which checks permissions on particular table in database
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="cloudProviderId"></param>
        /// <returns></returns>
        internal static string UpdateStatisticsPermissions(ServerVersion ver, int? cloudProviderId)
        {
            return GetBatch(GetBatchForCloud(UpdateStatisticsPermissions2000, cloudProviderId));
        }

        /// <summary>
        /// Returns the Permissions Batch which checks permissions on particular table in database
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        internal static string FragmentationPermissions(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(FragmentationsPermissions, cloudProviderId));
            }
            else return null;
        }

        internal static string ReindexPermissions(ServerVersion ver, int? cloudProviderId)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(GetBatchForCloud(ReindexPermissions2005, cloudProviderId));
            }
            else return null;
        }
        

        internal static string WaitStatistics(ServerVersion ver,int? cloudProviderId = null)
        {
            if (ver.Major >= 9)
            {
                return GetBatch(cloudProviderId == null ? WaitStatistics2005 : WaitStatistics2005.Replace(".sql", "_" + cloudProviderId.ToString()) + ".sql");                                                        
            }
            else
            {
                return null;
            }
        }

        internal static string WmiConfigurationTest(ServerVersion ver)
        {
            if (ver.Major >= 9) // Use same batch for SQL Server 2005/2008
            {
                return GetBatch(WmiConfigurationTest2005);
            }
            else
            {
                return GetBatch(WmiConfigurationTest2000);
            }
        }

        internal static string DatabaseAlwaysOnStatistics(ServerVersion ver)
        {
            if (ver.Major >= 11)
            {
                return GetBatch(AlwaysOnStatistics2012);
            }
            else  //AlwaysOn is only available for SQL Server 2012.
            {
                return null;
            }
        }

        internal static string DatabaseAlwaysOnTopology(ServerVersion ver)
        {
            if (ver.Major >= 11)
            {
                return GetBatch(AlwaysOnTopology2012);
            }
            else  //AlwaysOn is only available for SQL Server 2012.
            {
                return null;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion



    }
}
