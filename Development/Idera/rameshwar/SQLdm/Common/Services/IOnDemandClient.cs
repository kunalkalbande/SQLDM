// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   06-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Snapshots;
using System.Collections;
using Wintellect.PowerCollections;
using System.Collections.Generic;
using System;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;

namespace Idera.SQLdm.Common.Services
{
    using System.Data;
    using System.Management;
    using Events;
    using Snapshots.Cloud;
    /// <summary>
    /// Interface that the Management Service exposes for on-demand snapshot requests from 
    /// the client.  
    /// </summary>
    public interface IOnDemandClient
    {
        /// <summary>
        /// Signals a client request, running on a separate thread, as cancelled.
        /// </summary>
        /// <param name="sessionId"></param>
        void CancelOnDemandRequest(Guid sessionId);

        /// <summary>
        /// Gets ActiveWaitsSnapshot
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ActiveWaitsSnapshot> GetActiveWaits(ActiveWaitsConfiguration configuration);

        /// <summary>
        /// Stop waiting for active waits.  Does not necessarily end all collection.
        /// </summary>
        /// <param name="configuration"></param>
        void StopWaitingForActiveWaits(ActiveWaitsConfiguration configuration);

        /// <summary>
        /// Stop all active wait collectors for the monitored server.
        /// </summary>
        void StopActiveWaitCollector(int MonitoredServerId);

		// SQLdm Minimum Privileges - Varun Chopra - Read sever permissions with Active wait status
        Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions> GetActiveWaitCollectorStatus(int MonitoredServerId);

		// SQLdm Minimum Privileges - Varun Chopra - Read on demand permissions collections - Minimum, Metadata and Collection
        Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> GetServerPermissions(int monitoredSqlServerId);

        /// <summary>
        /// Gets AgentJobHistory
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<AgentJobHistorySnapshot> GetAgentJobHistory(AgentJobHistoryConfiguration configuration);

        // Modification Start ID: M1
        /// <summary>
        /// Gets GetAzureSQLMetric
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        //Serialized<AzureSQLMetricSnapshot> GetAzureSQLMetric(AzureSQLMetricConfiguration configuration);
        // Modification End ID: M1
        
        //Serialized<AmazonRDSMetricSnapshot> GetAmazonRDSMetric(AmazonRDSMetricConfiguration configuration);

        /// <summary>
        /// Gets AgentJobSummary
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<AgentJobSummary> GetAgentJobSummary(AgentJobSummaryConfiguration configuration);

        /// <summary>
        /// Gets BackupRestoreHistory
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<BackupRestoreHistory> GetBackupRestoreHistory(BackupRestoreHistoryConfiguration configuration);

        /// <summary>
        /// Gets configuration details
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ConfigurationSnapshot> GetConfiguration(OnDemandConfiguration configuration);

        /// <summary>
        /// Gets CustomCounter
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<CustomCounterCollectionSnapshot> GetCustomCounter(List<CustomCounterConfiguration> configuration);

        /// <summary>
        /// Gets CustomCounter
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<CustomCounterSnapshot> GetCustomCounter(CustomCounterConfiguration configuration);

        /// <summary>
        /// Gets the current cluster node for a clustered SQL Server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        string GetCurrentClusterNode(int monitoredSqlServerId);

		//SQLDM-30197
		/// <summary>
        /// Gets the current cluster node for a clustered SQL Server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        string GetPreferredClusterNode(int monitoredSqlServerId);
		
        /// <summary>
        /// Gets a list of disk drives used by the given sql server
        /// </summary>
        /// <param name="SqlServerId"></param>
        /// <returns></returns>
        List<string> GetDisks(int SqlServerId);

        /// <summary>
        /// Gets a dictionary of database names for the given instance.  The key is the 
        /// database name and the value is a boolean indicating system tables. 
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IDictionary<string, bool> GetDatabases(int monitoredSqlServerId, bool includeSystemDatabases, bool includeUserDatabases);

        DataTable GetAzureDatabases(int monitoredSqlServerId);

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --Get all the filegroups for a SQL server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IList<string> GetFilegroups(int monitoredSqlServerId, string databaseName, bool isDefaultThreshold);

        /// <summary>
        /// Gets DatabaseConfiguration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<DatabaseConfigurationSnapshot> GetDatabaseConfiguration(DatabaseProbeConfiguration configuration);

        /// <summary>
        /// Gets DatabaseFiles
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<DatabaseFilesSnapshot> GetDatabaseFiles(DatabaseFilesConfiguration configuration);

        /// <summary>
        /// Gets DatabaseSummary
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<DatabaseSummary> GetDatabaseSummary(DatabaseSummaryConfiguration configuration);

        /// <summary>
        /// Gets DatabaseAlwaysOnStatistics
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<AlwaysOnAvailabilityGroupsSnapshot> GetDatabaseAlwaysOnStatistics(AlwaysOnAvailabilityGroupsConfiguration configuration);

        /// <summary>
        /// Gets DatabaseAlwaysOnTopology
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<AlwaysOnAvailabilityGroupsSnapshot> GetDatabaseAlwaysOnTopology(AlwaysOnAvailabilityGroupsConfiguration configuration);

        /// <summary>
        /// Gets DistributorQueue
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<DistributorQueue> GetDistributorQueue(DistributorQueueConfiguration configuration);

        /// <summary>
        /// Gets Distributor Details
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<DistributorDetails> GetDistributorDetails(DistributorDetailsConfiguration configuration);

        /// <summary>
        /// Gets ErrorLog
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ErrorLog> GetErrorLog(ErrorLogConfiguration configuration);

        /// <summary>
        /// Gets FileActivity
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<FileActivitySnapshot> GetFileActivity(FileActivityConfiguration configuration);

        /// <summary>
        /// Gets FullTextCatalogs
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<FullTextCatalogs> GetFullTextCatalogs(OnDemandConfiguration configuration);

        /// <summary>
        /// Gets FullTextColumns
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<FullTextColumns> GetFullTextColumns(FullTextColumnsConfiguration configuration);
        
        /// <summary>
        /// Gets FullTextTables
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<FullTextTables> GetFullTextTables(FullTextTablesConfiguration configuration);

        /// <summary>
        /// Gets IndexStatistics
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<IndexStatistics> GetIndexStatistics(IndexStatisticsConfiguration configuration);

        /// <summary>
        /// Gets LockDetails
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<LockDetails> GetLockDetails(LockDetailsConfiguration configuration);

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --on demand details collection for disk size view 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="wmiConfig"></param>
        /// <returns></returns>
        Serialized<DiskSizeDetails> GetDiskSizeDetails(DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig);

        /// <summary>
        /// Gets LogList
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<LogFileList> GetLogList(OnDemandConfiguration configuration);
        /// <summary>
        /// gets Data for the mirroring real-time view
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Serialized<MirrorMonitoringRealtimeSnapshot> GetMirrorMonitoringRealtime(MirrorMonitoringRealtimeConfiguration configuration);

        /// <summary>
        /// Gets data for the history tab of the mirroring realtime view
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        MirrorMonitoringHistorySnapshot GetMirrorMonitoringHistory(MirrorMonitoringHistoryConfiguration configuration);

        /// <summary>
        /// Gets ProcedureCache
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ProcedureCache> GetProcedureCache(ProcedureCacheConfiguration configuration);

        ///// <summary>
        ///// Gets PublisherQueue
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        //Serialized<PublisherQueue> GetPublisherQueue(PublisherQueueConfiguration configuration);
        /// <summary>
        /// Gets Publisher Details
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<PublisherDetails> GetPublisherDetails(PublisherDetailsConfiguration configuration);

        /// <summary>
        /// Gets Subscriber Details
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<SubscriberDetails> GetSubscriberDetails(SubscriberDetailsConfiguration configuration);

        ///// <summary>
        ///// Gets Resource
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        //Serialized<ResourceSnapshot> GetResource(ResourceConfiguration configuration);

        // Get current local server time and version, and collection service local time
        Triple<ServerVersion, DateTime, DateTime> GetServerTimeAndVersion(int monitoredSqlServerId);

        /// <summary>
        /// Gets the server overview.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ServerOverview> GetServerOverview(ServerOverviewConfiguration configuration);

        /// <summary>
        /// Gets several snapshots for the monitored server that comprise a complete server summary.
        /// </summary>
        /// <param name="serverOverviewConfiguration">The Server Overview snapshot configuration.</param>
        /// <returns></returns>
        ServerSummarySnapshots GetServerSummary(ServerOverviewConfiguration serverOverviewConfiguration);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetPrescriptiveOptimizeScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);
        
        /// <summary>
        /// 10.0 doctor integration -- GetMessages
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        List<string> GetPrescriptiveOptimizeMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetPrescriptiveUndoScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);

        /// <summary>
        /// 10.0 doctor integration -- GetUndoMessages
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        List<string> GetPrescriptiveUndoMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);
        
        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        PrescriptiveOptimizationStatusSnapshot ExecutePrescriptiveOptimization(int monitoredSqlServerId, PrescriptiveScriptConfiguration configuration);
        
        /// <summary>
        /// SQLdm 10.0 srishti purohit-- doctor integration-- affected procedures and tables for specific table of database
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        DependentObjectSnapshot GetTableDependentObjects(int monitoredSqlServerId, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName ObjectName);
        
        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetConnectionStringForServer(int monitoredSqlServerId);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Dictionary<int,string> GetDatabasesForServer(int serverId);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        string GetMachineName(int serverId);

        /// <summary>
        /// Gets Services
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ServicesSnapshot> GetServices(OnDemandConfiguration configuration);

        /// <summary>
        /// Gets session details
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<SessionSnapshot> GetSessions(SessionsConfiguration configuration);

        /// <summary>
        /// Gets SessionDetails
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<SessionDetailSnapshot> GetSessionDetails(SessionDetailsConfiguration configuration);

        /// <summary>
        /// Gets session summary 
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<SessionSummary> GetSessionSummary(SessionSummaryConfiguration configuration);

        /// <summary>
        /// Gets TableDetails
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<TableDetail> GetTableDetails(TableDetailConfiguration configuration);

        /// <summary>
        /// Gets TableSummary
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<TableSummary> GetTableSummary(TableSummaryConfiguration configuration);

        /// <summary>
        /// Gets WaitStats
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<WaitStatisticsSnapshot> GetWaitStatistics(WaitStatisticsConfiguration configuration);

        /// <summary>
        /// Gets a list of table names for the specified server.  Each Triple contains
        /// a schema name, table name, and bool indicating if the table is a system table.
        /// </summary>
        List<Triple<string, string, bool>> GetTables(int monitoredSqlServerId, string database, bool includeSystemTables, bool includeUserTables);

        // get list of available objects in sysperfinfo
        Serialized<DataTable> GetSysPerfInfoObjectList(int monitoredSqlServerId);
        Serialized<DataTable> GetSysPerfInfoCounterList(int monitroredSqlServerId, string objectName);
        Serialized<DataTable> GetSysPerfInfoInstanceList(int monitoredSqlServerId, string objectName);

        // Get Azure Monitor Namespaces
        Serialized<DataTable> GetAzureMonitorDefinitions(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration);

        List<IAzureResource> GetAzureApplicationResources(IMonitorManagementConfiguration configuration);

        List<AzureSqlModel> GetFilteredAzureApplicationResources(IMonitorManagementConfiguration configuration);

        Serialized<DataTable> GetAzureMonitorNamespaces(int instanceId, IMonitorManagementConfiguration monitorConfiguration);

        // get list of available objects in the \root\cimv2 namespace
        Serialized<Pair<string, DataTable>> GetWmiObjectList(int monitoredSqlServerId, WmiConfiguration wmiConfiguration);
        Serialized<DataTable> GetWmiCounterList(int monitroredSqlServerId, string serverName, string objectName, WmiConfiguration wmiConfiguration);
        Serialized<DataTable> GetWmiInstanceList(int monitoredSqlServerId, string serverName, string objectName, WmiConfiguration wmiConfiguration);

        // get list of available objects in the \root\cimv2 namespace
        Serialized<DataTable> GetVmCounterObjectList(int monitoredSqlServerId);

        // try to retrieve the custom counter 
        object TestCustomCounter(int monitoredSqlServer, CustomCounterDefinition counterDefinition);

        Serialized<DataTable> GetDriveConfiguration(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration);

        Serialized<WmiConfigurationTestSnapshot> SendWmiConfigurationTest(TestWmiConfiguration configuration);

        #region Server Actions

        /// <summary>
        /// Sends a generic server action 
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
//        Serialized<R> SendServerAction<T,R>(T configuration) where T : OnDemandConfiguration, IServerActionConfiguration
//                                                             where R : Snapshot;

        /// <summary>
        /// Sends FreeProcedureCache
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendFreeProcedureCache(FreeProcedureCacheConfiguration configuration);


        /// <summary>
        /// Sends FullTextAction
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendFullTextAction(FullTextActionConfiguration configuration);

        /// <summary>
        /// Sends Agent job Start
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendJobControl(JobControlConfiguration configuration);

        /// <summary>
        /// Sends KillSession
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendKillSession(KillSessionConfiguration configuration);

        /// <summary>
        /// Sends reconfiguration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendReconfiguration(ReconfigurationConfiguration configuration);

        /// <summary>
        /// Sends blocked process Threshold change
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendBlockedProcessThresholdChange(BlockedProcessThresholdConfiguration configuration);

         /// <summary>
        /// Sends SendShutdownSQLServer
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendShutdownSQLServer(ShutdownSQLServerConfiguration configuration);


        /// <summary>
        /// Sends SendSetNumberOfLogs
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendSetNumberOfLogs(SetNumberOfLogsConfiguration configuration);

        /// <summary>
        /// Sends SendStopSessionDetailsTrace
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendStopSessionDetailsTrace(StopSessionDetailsTraceConfiguration configuration);

        /// <summary>
        /// Sends SendStopQueryMonitorTrace
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendStopQueryMonitorTrace(StopQueryMonitorTraceConfiguration configuration);

        /// <summary>
        /// Sends ServiceControl
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendServiceControl(ServiceControlConfiguration configuration);

        /// <summary>
        /// Sends RecycleLog
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendRecycleLog(RecycleLogConfiguration configuration);
        
        /// <summary>
        /// Sends RecycleAgentLog
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendRecycleAgentLog(RecycleAgentLogConfiguration configuration);

        /// <summary>
        /// Sends Reindex
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<ReindexSnapshot> SendReindex(ReindexConfiguration configuration);

        /// <summary>
        /// Sends UpdateStatistics
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<Snapshot> SendUpdateStatistics(UpdateStatisticsConfiguration configuration);
        
        /// <summary>
        /// Failover, suspend or resume mirroring session
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Serialized<Snapshot> SendMirroringPartnerAction(MirroringPartnerActionConfiguration configuration);
        
        /// <summary>
        /// Sends SQL to be executed on instance
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Serialized<AdhocQuerySnapshot> SendAdhocQuery(AdhocQueryConfiguration configuration);

        /// <summary>
        /// Get list of available jobs or steps for selected instance
        /// </summary>
        /// <param name="configuration">The Jobs And Steps configuration object.</param>
        Serialized<JobsAndStepsSnapshot> GetJobsAndSteps(JobsAndStepsConfiguration configuration);
        #endregion

        #region QueryMonitor

        /// <summary>
        /// SQLdm 10.4 Nikhil Bansal -- Get the OnDemand Estimated Query Plan
        /// </summary>
        /// <param name="estQueryPlanConfig">Configuration of the query for which the plan has to be collected</param>
        /// <returns> Estimated Plan </returns>
        string GetEstimatedQueryPlan(EstimatedQueryPlanConfiguration estQueryPlanConfig);

        #endregion
    }
}
