//------------------------------------------------------------------------------
// <copyright file="IOnDemandServer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   04-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Snapshots;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.Common.Services
{
    /// <summary>
    /// The interface that both the collection service and management service implement to accept
    /// on-demand requests for snapshots.
    /// </summary>
    public interface IOnDemandServer
    {
        #region properties

        #endregion

        #region events

        #endregion

        #region methods


        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        void GetPrescriptiveAnalysisSnapshots(int monitoredSqlServerId, ISnapshotSink sink, object state, AnalysisConfiguration config, AnalysisCollectorType analysisCollectorType);

         /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetPrescriptiveOptimizeScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);

        void GetPrescriptiveAnalysisDbSnapshots(int monitoredSqlServerId, ISnapshotSink sink, object state, string db);

        /// <summary>
        /// 10.0 doctor integration --GetMessages
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        List<string> GetPrescriptiveOptimizeMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);
        
        /// <summary>
        /// 10.0 doctor integration --GetUndoMessages
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        List<string> GetPrescriptiveUndoMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetPrescriptiveUndoScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Result ExecutePrescriptiveOptimization(int monitoredSqlServerId, ISnapshotSink sink, object state, PrescriptiveScriptConfiguration configuration);

        /// <summary>
        /// 10.0 Srishti Purohit - doctor integration
        /// </summary>
        /// <param name="Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName"></param>
        Result GetTableofDependentObject(int monitoredSqlServerId, ISnapshotSink sink, object state, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName objectName);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        string GetConnectionStringForServer(int monitoredSqlServerId);

        /// <summary>
        /// GetCloudProvider
        /// </summary>
        /// <param name="monitoredSqlServerId">Monitored SqlServer Id</param>
        int? GetCloudProvider(int monitoredSqlServerId);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Result GetDatabasesForServer(int serverId, ISnapshotSink sink, object state);

        /// <summary>
        /// 10.0 doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        Result GetMachineName(int serverId, ISnapshotSink sink, object state);


        /// <summary>
        /// Collects data from the ActiveWaits probe and starts it if it is not running.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <returns></returns>
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

		// SQLdm Minimum Privileges - Varun Chopra - Read sever permissions with active waits status
        Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions> GetActiveWaitCollectorStatus(int MonitoredServerId);

        /// <summary>
        /// Starts the AgentJobHistory probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetAgentJobHistory(AgentJobHistoryConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the AzureSQLMetric probe for a monitored server.  
        /// The results are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        //void GetAzureSQLMetric(AzureSQLMetricConfiguration configuration, ISnapshotSink sink, object state);

        //void GetAmazonRDSMetric(AmazonRDSMetricConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the AgentJobSummary probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetAgentJobSummary(AgentJobSummaryConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the BackupRestoreHistory probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetBackupRestoreHistory(BackupRestoreHistoryConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the configuration probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetConfiguration(OnDemandConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the CustomCounter probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetCustomCounter(CustomCounterConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the CustomCounter probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetCustomCounter(List<CustomCounterConfiguration> configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the DatabaseConfiguration probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDatabaseConfiguration(DatabaseProbeConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the DatabaseFiles probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDatabaseFiles(DatabaseFilesConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the DatabaseSummary probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDatabaseSummary(DatabaseSummaryConfiguration configuration, ISnapshotSink sink, object state);
        
        /// <summary>
        /// Starts the DatabaseAlwaysOnStatistics probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDatabaseAlwaysOnStatistics(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the DatabaseAlwaysOnTopology probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDatabaseAlwaysOnTopology(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the Distributor details probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDistributorDetails(DistributorDetailsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the DistributorQueue probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetDistributorQueue(DistributorQueueConfiguration configuration, ISnapshotSink sink, object state);
        
        /// <summary>
        /// Starts the ErrorLog probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetErrorLog(ErrorLogConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the FullTextCatalogs probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetFullTextCatalogs(OnDemandConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the FullTextColumns probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetFullTextColumns(FullTextColumnsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the FullTextTables probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetFullTextTables(FullTextTablesConfiguration configuration, ISnapshotSink sink, object state);


        /// <summary>
        /// Starts the IndexStatistics probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetIndexStatistics(IndexStatisticsConfiguration configuration, ISnapshotSink sink, object state);
        
        /// <summary>
        /// Starts the LockDetails probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetLockDetails(LockDetailsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="wmiConfig"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        void GetDiskSizeDetails(DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the LogList probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetLogList(OnDemandConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the real-time mirror monitoring probe
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        void GetMirrorMonitoringRealtime(MirrorMonitoringRealtimeConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the MirrorMonitoringHistory probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetMirrorMonitoringHistory(MirrorMonitoringHistoryConfiguration configuration, ISnapshotSink sink, object state);
        
        /// <summary>
        /// Starts the ProcedureCache probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetProcedureCache(ProcedureCacheConfiguration configuration, ISnapshotSink sink, object state);

        ///// <summary>
        ///// Starts the PublisherQueue probe for a monitored server.  The results
        ///// are delivered back to the management service through the CAO sink.
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        ///// <param name="sink">The sink.</param>
        ///// <param name="state">The state.</param>
        ///// <returns></returns>
        //void GetPublisherQueue(PublisherQueueConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the Publisher Details probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetPublisherDetails(PublisherDetailsConfiguration configuration, ISnapshotSink sink, object state);
        /// <summary>
        /// Starts the Subscriber Details probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetSubscriberDetails(SubscriberDetailsConfiguration configuration, ISnapshotSink sink, object state);


        ///// <summary>
        ///// Starts the Resource probe for a monitored server.  The results
        ///// are delivered back to the management service through the CAO sink.
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        ///// <param name="sink">The sink.</param>
        ///// <param name="state">The state.</param>
        ///// <returns></returns>
        //void GetResource(ResourceConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the server overview probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetServerOverview(ServerOverviewConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the Services probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetServices(OnDemandConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the sessions probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetSessions(SessionsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SessionDetails probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetSessionDetails(SessionDetailsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SessionSummary probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetSessionSummary(SessionSummaryConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the TableDetails probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetTableDetails(TableDetailConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the TableSummary probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetTableSummary(TableSummaryConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the WaitStats probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetWaitStatistics(WaitStatisticsConfiguration configuration, ISnapshotSink sink, object state);

        #region Server Actions

        /// <summary>
        /// Starts the server action probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendServerAction<T>(T configuration, ISnapshotSink sink, object state)
            where T : OnDemandConfiguration, IServerActionConfiguration;


        /// <summary>
        /// Starts the Free Procedure Cache probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendFreeProcedureCache(FreeProcedureCacheConfiguration configuration, ISnapshotSink sink, object state);


        /// <summary>
        /// Starts the FileActivity probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetFileActivity(FileActivityConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the FullTextAction probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void GetFullTextAction(FullTextActionConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the kill session probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendKillSession(KillSessionConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the configuration probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendReconfiguration(ReconfigurationConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SendShutdownSQLServer probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendShutdownSQLServer(ShutdownSQLServerConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the kill session probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendSetNumberOfLogs(SetNumberOfLogsConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SendStopSessionDetailsTrace probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendStopSessionDetailsTrace(StopSessionDetailsTraceConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SendStopQueryMonitorTrace probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendStopQueryMonitorTrace(StopQueryMonitorTraceConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the SendStopActivityMonitorTrace probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendStopActivityMonitorTrace(StopActivityMonitorTraceConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the ServiceControl probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendServiceControl(ServiceControlConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the recycle log probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendRecycleLog(RecycleLogConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the recycle log probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendRecycleAgentLog(RecycleAgentLogConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the reindex probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendReindex(ReindexConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the UpdateStatistics probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        void SendUpdateStatistics(UpdateStatisticsConfiguration configuration, ISnapshotSink sink, object state);


        void SendWmiConfigurationTest(TestWmiConfiguration configuration, ISnapshotSink sink, object state);

        /// <summary>
        /// Starts the Jobs and Steps probe for a monitored server.  The results
        /// are delivered back to the management service through the CAO sink.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        void GetJobsAndSteps(JobsAndStepsConfiguration configuration, ISnapshotSink sink, object state);
        #endregion

        /// <summary>
        /// Forces a scheduled refresh to start, waits for completion, returns the status document
        /// for the server.
        /// </summary>
        void ForceScheduledCollection(int monitoredSqlServerID, ISnapshotSink sink, object state);

        #region QueryMonitor

        /// <summary>
        /// SQLdm 10.4 (Nikhil Bansal) - Get the OnDemand Estimated Query Plan 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns>Estimated Query Plan</returns>
        void GetEstimatedQueryPlan(EstimatedQueryPlanConfiguration configuration, ISnapshotSink sink, object state);

        #endregion

        #endregion
    }
}
