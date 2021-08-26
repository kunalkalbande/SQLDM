//------------------------------------------------------------------------------
// <copyright file="OnDemandCollectionManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   04-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------

using System.Linq;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Sql.Batches;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.VMware;
using Idera.SQLdm.Services.Common.Probes.Azure;
using Microsoft.Azure.Management.Monitor;
using Vim25Api;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Management;
    using System.Text;
    using BBS.TracerX;
    using Configuration;
    using Idera.SQLdm.CollectionService.Helpers;
    using Idera.SQLdm.CollectionService.Probes;
    using Idera.SQLdm.CollectionService.Probes.Sql;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.VMware;
    using Microsoft.ApplicationBlocks.Data;
    using Wintellect.PowerCollections;
    using System.Threading;
    using Idera.SQLdm.Common.HyperV;


    /// <summary>
    /// Manages all on-demand collection for the collection service.
    /// </summary>
    public partial class OnDemandCollectionManager
    {
        #region fields

        private Logger LOG = Logger.GetLogger("OnDemandCollectionManager");

        private Dictionary<int, MonitoredSqlServer> monitoredServers = new Dictionary<int, MonitoredSqlServer>();

        #endregion

        #region constructors

        public OnDemandCollectionManager()
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Clear the map of monitored servers.
        /// </summary>
        public void Clear()
        {
            lock (monitoredServers)
            {
                monitoredServers.Clear();
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Adds the monitored server.
        /// </summary>
        /// <param name="monitoredServer">The server.</param>
        public void AddMonitoredServer(MonitoredSqlServer monitoredServer)
        {
            lock (monitoredServers)
            {
                monitoredServers[monitoredServer.Id] = monitoredServer;
            }
        }

        /// <summary>
        /// Removes the monitored server.
        /// </summary>
        /// <param name="monitoredServerId">The server.</param>
        public void RemoveMonitoredServer(int monitoredServerId)
        {
            lock (monitoredServers)
            {
                LOG.DebugFormat("Stopping on demand collection for instance ID:({0})", monitoredServerId);
                monitoredServers.Remove(monitoredServerId);
            }
        }

        public void ReplaceMonitoredServer(MonitoredSqlServer monitoredServer)
        {
            int id = monitoredServer.Id;

            lock (monitoredServers)
            {
                if (monitoredServers.ContainsKey(id))
                {
                    monitoredServer.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(id, monitoredServers[id].ActiveWaitsConfiguration, monitoredServer.ActiveWaitsConfiguration);
                    monitoredServers.Remove(id);
                }

                monitoredServers[id] = monitoredServer;
            }
        }


        /// <summary>
        /// Gets the monitored server info.
        /// </summary>
        /// <param name="monitoredServerId">The monitored server.</param>
        /// <returns></returns>
        protected MonitoredSqlServer GetMonitoredServerInfo(int monitoredServerId)
        {
            if (Collection.IsPaused)
                throw new CollectionServiceException(
                    "Unable to process your request.  The collection service is currently paused.");

            lock (monitoredServers)
            {
                MonitoredSqlServer ret;
                monitoredServers.TryGetValue(monitoredServerId, out ret);

                return ret;
            }
        }
        ///Ankit Nagpal --Sqldm10.0.0
        /// <summary>
        /// Return connection info of monitored Server
        /// </summary>
        /// <param name="monitoredServerId"></param>
        /// <returns>SqlConnectionInfo</returns>
        public SqlConnectionInfo GetSqlServerConnectionDetails(int monitoredServerId)
        {

            return GetMonitoredServerInfo(monitoredServerId).ConnectionInfo;

        }


        /// <summary>
        /// Collects the ActiveWaits data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <returns></returns>
        public ActiveWaitsSnapshot CollectActiveWaits(ActiveWaitsConfiguration configuration)
        {
            return Collection.Scheduled.GetActiveWaits(configuration);
        }

        /// <summary>
        /// Collects the AgentJobHistory data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectAgentJobHistory(AgentJobHistoryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildAgentJobHistoryProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        // Modification Start ID: M1
        /// <summary>
        /// Collects the AzureSQLMetric data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        //public Result CollectAzureSQLMetric(AzureSQLMetricConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
        //    if (msi == null)
        //        return Result.Failure;
        //    IProbe probe = ProbeFactory.BuildAzureSQLMetric(msi.ConnectionInfo, msi.CloudProviderId);
        //    OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
        //    return ctxt.Start();
        //}
        // Modification End ID: M1

        //public Result CollectAmazonRDSMetric(AmazonRDSMetricConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
        //    if (msi == null)
        //        return Result.Failure;
        //    IProbe probe = ProbeFactory.BuildAmazonRDSMetric(msi.ConnectionInfo, msi.CloudProviderId);
        //    OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
        //    return ctxt.Start();
        //}

        /// <summary>
        /// Collects the AgentJobSummary data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectAgentJobSummary(AgentJobSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId); // SQLdm 8.6 (Ankit Srivastava) -- getting workload  - solved defect DE43661

            IProbe probe = ProbeFactory.BuildAgentJobSummaryProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.ClusterCollectionSetting, workload, msi.CloudProviderId); // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the BackupRestoreHistory data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectBackupRestoreHistory(BackupRestoreHistoryConfiguration
            configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildBackupRestoreHistoryProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the configuration data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectConfiguration(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildConfigurationProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Collects the CustomCounter data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectCustomCounter(List<CustomCounterConfiguration> configuration, ISnapshotSink sink, object state)
        {
            if (configuration.Count == 0)
                return Result.Failure;

            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration[0].MonitoredServerId);
            if (msi == null)
                throw new ArgumentException("Monitored instance not currently being monitored");

            IProbe probe = ProbeFactory.BuildCustomCounterProbe(msi.ConnectionInfo, configuration, msi.CustomCounterTimeout,
                                                                    msi.WmiConfig,
                                                                    msi.VirtualizationConfiguration, msi.CloudProviderId, msi.ScheduledCollectionInterval);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the DatabaseConfiguration data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseConfiguration(DatabaseProbeConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseConfigurationProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Disk size data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDiskSize(DatabaseProbeConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseConfigurationProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details on demand
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="wmiConfig"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result CollectDiskSizeDetails(DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDiskSizeDetailsProbe(msi.ConnectionInfo, configuration, wmiConfig, msi.DiskCollectionSettings, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the DatabaseFiles data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseFiles(DatabaseFilesConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseFilesProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.DiskCollectionSettings, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the DatabaseSummary data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseSummary(DatabaseSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseSummaryProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.DiskCollectionSettings, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Database AlwaysOn Statistics data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseAlwaysOnStatistics(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseAlwaysOnStatisticsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Database AlwaysOn Topology data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseAlwaysOnTopology(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseAlwaysOnTopologyProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the DistributorQueue data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDistributorQueue(DistributorQueueConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDistributorQueueProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Distributor Details data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDistributorDetails(DistributorDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;
            if (msi.ReplicationMonitoringDisabled)
            {
                DistributorDetails dd = new DistributorDetails(msi.ConnectionInfo);
                dd.ReplicationStatus = ReplicationState.CollectionDisabled;
                sink.Process(new Serialized<DistributorDetails>(dd), state);
                return Result.Success;
            }

            IProbe probe = ProbeFactory.BuildDistributorDetailsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the ErrorLog data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectErrorLog(ErrorLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId);

            IProbe probe = ProbeFactory.BuildErrorLogProbe(msi.ConnectionInfo, configuration, workload, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state, true);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the FileActivity data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectFileActivity(FileActivityConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildFileActivityProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.DiskCollectionSettings, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the FullTextCatalogs data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectFullTextCatalogs(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildFullTextCatalogsProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.ClusterCollectionSetting, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the FullTextColumns data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectFullTextColumns(FullTextColumnsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildFullTextColumnsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the FullTextTables data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectFullTextTables(FullTextTablesConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildFullTextTablesProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Collects the IndexStatistics data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectIndexStatistics(IndexStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildIndexStatisticsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the LockDetails data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectLockDetails(LockDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;
            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added another param 
            IProbe probe = ProbeFactory.BuildLockDetailsProbe(msi.ConnectionInfo, configuration, workload.MonitoredServer.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the LogList data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectLogList(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId); // SQLdm 8.6 (Ankit Srivastava) -- getting workload  - solved defect DE43661

            IProbe probe = ProbeFactory.BuildLogListProbe(msi.ConnectionInfo, workload, msi.CloudProviderId); // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the ProcedureCache data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectProcedureCache(ProcedureCacheConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildProcedureCacheProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the MirrorMonitoringRealtime data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectMirrorMonitoringRealtime(MirrorMonitoringRealtimeConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildMirrorMonitoringRealtimeProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the MirrorMonitoringHistory data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectMirrorMonitoringHistory(MirrorMonitoringHistoryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildMirrorMonitoringHistoryProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Jobs And Steps data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectJobsAndSteps(JobsAndStepsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildJobsAndStepsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }
        ///// <summary>
        ///// Collects the PublisherQueue data.
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        ///// <param name="sink">The sink.</param>
        ///// <param name="state">The state.</param>
        ///// <returns></returns>
        //public Result CollectPublisherQueue(PublisherQueueConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
        //    if (msi == null)
        //        return Result.Failure;

        //    if (msi.ReplicationMonitoringDisabled)
        //    {
        //        PublisherQueue pq = new PublisherQueue(msi.ConnectionInfo);
        //        pq.ReplicationStatus = ReplicationState.CollectionDisabled;
        //        sink.Process(new Serialized<PublisherQueue>(pq), state);
        //        return Result.Success;
        //    }

        //    IProbe probe = ProbeFactory.BuildPublisherQueueProbe(msi.ConnectionInfo, configuration);
        //    OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
        //    return ctxt.Start();
        //}
        /// <summary>
        /// Collects the Publisher Details data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectPublisherDetails(PublisherDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            if (msi.ReplicationMonitoringDisabled)
            {
                PublisherDetails pd = new PublisherDetails(msi.ConnectionInfo);
                pd.ReplicationStatus = ReplicationState.CollectionDisabled;
                sink.Process(new Serialized<PublisherDetails>(pd), state);
                return Result.Success;
            }

            IProbe probe = ProbeFactory.BuildPublisherDetailsProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.ClusterCollectionSetting, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Subscriber Details data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSubscriberDetails(SubscriberDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            if (msi.ReplicationMonitoringDisabled)
            {
                SubscriberDetails sd = new SubscriberDetails(msi.ConnectionInfo);
                sd.ReplicationStatus = ReplicationState.CollectionDisabled;
                sink.Process(new Serialized<SubscriberDetails>(sd), state);
                return Result.Success;
            }

            IProbe probe = ProbeFactory.BuildSubscriberDetailsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        ///// <summary>
        ///// Collects the Resource data.
        ///// </summary>
        ///// <param name="configuration">The configuration object.</param>
        ///// <param name="sink">The sink.</param>
        ///// <param name="state">The state.</param>
        ///// <returns></returns>
        //public Result CollectResource(ResourceConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
        //    if (msi == null)
        //        return Result.Failure;

        //    IProbe probe = ProbeFactory.BuildResourceProbe(msi.ConnectionInfo, configuration, (int) msi.OleAutomationContext, msi.OleAutomationUseDisabled, msi.DiskCollectionSettings);
        //    OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
        //    return ctxt.Start();
        //}

        /// <summary>
        /// Collects the server overview.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectServerOverview(ServerOverviewConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;
            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId);//SQLdm 10.0 (Tarun Sapra)- Get the workload, such that we can have the cloud provider id
            IProbe probe = ProbeFactory.BuildServerOverviewProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.ClusterCollectionSetting, msi.DiskCollectionSettings, msi.VirtualizationConfiguration, msi, workload.MonitoredServer.CloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support:Added a new optional param

            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the Services data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectServices(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildServicesProbe(msi.ConnectionInfo, configuration, msi.WmiConfig, msi.ClusterCollectionSetting, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the session data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSessions(SessionsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            if (msi.InputBufferLimited && msi.InputBufferLimiter > 0)
                configuration.InputBufferLimiter = msi.InputBufferLimiter;
            MonitoredServerWorkload worklaod = Collection.Scheduled.GetWorkload(configuration.MonitoredServerId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: added another param to select the batch according to cloud provider id
            IProbe probe = ProbeFactory.BuildSessionsProbe(msi.ConnectionInfo, configuration, worklaod.MonitoredServer.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the SessionDetails data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSessionDetails(SessionDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildSessionDetailsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the SessionSummary data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSessionSummary(SessionSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildSessionSummaryProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the TableDetails data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectTableDetails(TableDetailConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildTableDetailsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }
        /// <summary>
        /// Collects the TableSummary data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectTableSummary(TableSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildTableSummaryProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Collects the WaitStats data.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWaitStatistics(WaitStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWaitStatisticsProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        #endregion

        #region Server Actions

        /// <summary>
        /// Sends server action probe and executes code path based on configuration object
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendServerActionProbe<T>(T configuration, ISnapshotSink sink, object state) where T : OnDemandConfiguration, IServerActionConfiguration
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildServerActionProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Frees procedure cache
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendFreeProcedureCache(FreeProcedureCacheConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends a full text action
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendFullTextAction(FullTextActionConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends new configuration data 
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendReconfigurationProbe(ReconfigurationConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends a new Blocked Process Threshold
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result SendBlockedProcessThresholdChangeProbe(ReconfigurationConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends job start command
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendJobControlProbe(JobControlConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends kill command
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendKillSessionProbe(KillSessionConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Sends kill command
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendShutdownSQLServer(ShutdownSQLServerConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Set number of SQL Server logs
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendSetNumberOfLogs(SetNumberOfLogsConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Stops a session details trace
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendStopSessionDetailsTrace(StopSessionDetailsTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Stops a query monitortrace
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendStopQueryMonitorTrace(StopQueryMonitorTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Stops a Activity monitor trace
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendStopActivityMonitorTrace(StopActivityMonitorTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Controls a monitored server's services
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendServiceControl(ServiceControlConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }


        /// <summary>
        /// Recycles a SQL Server log
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendRecycleLog(RecycleLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Recycles a SQL Agent log
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendRecycleAgentLog(RecycleAgentLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Reindexes a table
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendReindex(ReindexConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        /// <summary>
        /// Updates statistics for a table
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result SendUpdateStatistics(UpdateStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            return SendServerActionProbe(configuration, sink, state);
        }

        internal void SendStartQueryMonitorTrace(int monitoredSqlServerId, QueryMonitorConfiguration currentConfig, QueryMonitorConfiguration previousConfig, int? cloudProviderId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            StartQueryMonitorTraceConfiguration config =
                new StartQueryMonitorTraceConfiguration(monitoredSqlServerId, currentConfig, previousConfig);

            using (SqlConnection conn = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                conn.Open();
                ServerVersion ver = new ServerVersion(conn.ServerVersion);
                SqlCommand cmd = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildQueryMonitorStartCommand(
                            conn,
                            ver,
                            config,
                            cloudProviderId);

                using (cmd)
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring improvement with Extended Event Session -- added new method for starting Extended event session on demand
        // SQLdm 10.3 (Varun Chopra) Linux Support for Query Monitor
        internal void SendStartQueryMonitorExtendedEventSession(int monitoredSqlServerId, QueryMonitorConfiguration currentConfig, QueryMonitorConfiguration previousConfig, ActiveWaitsConfiguration waitConfig,
            int? cloudProviderId = null)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            StartQueryMonitorTraceConfiguration config =
                new StartQueryMonitorTraceConfiguration(monitoredSqlServerId, currentConfig, previousConfig);

            using (SqlConnection conn = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                conn.Open();
                ServerVersion ver = new ServerVersion(conn.ServerVersion);
                SqlCommand cmd = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildQueryMonitorStartCommandEX(
                            conn,
                            ver,
                            config,
                            waitConfig,
                            cloudProviderId);

                using (cmd)
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
        internal void SendStartActivityMonitorTrace(int monitoredSqlServerId, ActivityMonitorConfiguration currentConfig, ActivityMonitorConfiguration previousConfig, int? cloudProviderId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            StartActivityMonitorTraceConfiguration config =
                new StartActivityMonitorTraceConfiguration(monitoredSqlServerId, currentConfig, previousConfig);

            if (previousConfig != null && currentConfig.BlockedProcessThreshold != previousConfig.BlockedProcessThreshold)
            {

            }

            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                var databases = GetDatabases(monitoredSqlServerId, false, true);
                foreach (var database in databases.Keys)
                {
                    ActivityMonitorStart(monitoredSqlServerId, cloudProviderId, connectionInfo, msi, config, database);
                }
            }
            else
            {
                ActivityMonitorStart(monitoredSqlServerId, cloudProviderId, connectionInfo, msi, config);
            }
        }

        private void ActivityMonitorStart(int monitoredSqlServerId, int? cloudProviderId, SqlConnectionInfo connectionInfo,
            MonitoredSqlServer msi, StartActivityMonitorTraceConfiguration config, string database = null)
        {
            using (SqlConnection conn = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName, database))
            {
                conn.Open();
                ServerVersion ver = new ServerVersion(conn.ServerVersion);
                // Validate Probe Permissions by Passing Permissions Command to generate permissions required
                MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(monitoredSqlServerId);
                // create and pass the probe error argument to handle and store errors
                var collectorName = "Activity Monitor";
                var probeError = new ProbePermissionHelpers.ProbeError() {Name = collectorName};
                // Allow Permission Skip for Cloud Providers
                var hasPermissions = cloudProviderId == Constants.AmazonRDSId || (ver != null && ver.Major < 11) ||
                                     ProbePermissionHelpers.ValidateProbePermissions(
                                         cloudProviderId == Constants.MicrosoftAzureId
                                             ? Probes.Sql.SqlCommandBuilder.BuildMasterPermissionsCommand(
                                                 connectionInfo.GetConnectionDatabase("master"), connectionInfo,
                                                 cloudProviderId)
                                             : null,
                                         Probes.Sql.SqlCommandBuilder.BuildPermissionsCommand(conn, connectionInfo,
                                             cloudProviderId), ver,
                                         collectorName, cloudProviderId,
                                         new object[]
                                         {
                                             workload.MonitoredServer.ActivityMonitorConfiguration
                                                 .TraceMonitoringEnabled,
                                             probeError
                                         });

                if (!hasPermissions)
                {
                    var message =
                        string.Format(
                            "The user account used by the collection service on the monitored server {0} (with SQL Server Version {1}) does not have minimum rights required to start Activity Monitor {2}{3}",
                            msi.InstanceName, ver, Environment.NewLine, probeError);
                    LOG.Error("SendStartActivityMonitor:: " + message);
                    throw new ApplicationException(message);
                }

                //START SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - modifying the on demand call to activity monitor collection
                SqlCommand cmd = conn.CreateCommand();
                if (config.CurrentActivityMonitorConfig.TraceMonitoringEnabled)
                    cmd = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildActivityMonitorStartCommand(
                        conn,
                        ver,
                        config, cloudProviderId);
                else
                    cmd = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildActivityMonitorStartCommandEX(
                        conn,
                        ver,
                        config, cloudProviderId);
                //END  SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - modifying the on demand call to activity monitor collection

                using (cmd)
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Result SendWmiConfigurationTest(TestWmiConfiguration configuration, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(configuration.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiConfigurationTestProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        private void CheckSupportedServer(SqlConnection connection, string instanceName)
        {
            ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);
            //Check the server version
            // Add SQL Server 2008
            if (serverVersion.Major >= 8)
            {
                string productEdition = null;
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetEdition;
                    productEdition = (string)command.ExecuteScalar();
                }

                //SQLdm 8.6 (Ankit Srivastava) : Commented the code which stopped SQL express monitoring 
                // Check edition
                //if (productEdition.ToLower().Trim() != "express edition")
                //{
                LOG.Verbose("Monitored server " + instanceName + " is running a supported SQL Server edition.");
                //}
                //else
                //{
                //    StringBuilder builder = new StringBuilder();
                //    builder.AppendFormat("Monitored server {0} is running an unsupported SQL Server edition.  ", instanceName);
                //    builder.AppendFormat("The reported version is {0} {1}.", serverVersion.Version, productEdition);
                //    LOG.Debug(builder.ToString());
                //    throw new CollectionServiceException(builder.ToString());
                //}
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Monitored server {0} is running an unsupported SQL Server edition.  ", instanceName);
                builder.AppendFormat("The reported version is {0}.", serverVersion.Version);
                LOG.Debug(builder.ToString());
                throw new CollectionServiceException(builder.ToString());
            }
        }

        /// <summary>
        /// Get Server Permissions
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns>Tuple of Permissions</returns>
        public Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> GetServerPermissions(int monitoredSqlServerId)
        {
            ServerVersion serverVersion = null;

            // Get Monitored Server
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            // Execute Permissions Command
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                serverVersion = new ServerVersion(connection.ServerVersion);

                // Check Supported Server
                CheckSupportedServer(connection, msi.InstanceName);

                // Allow Permission Skip for Cloud Providers
                if (msi.CloudProviderId == Constants.AmazonRDSId || (serverVersion != null && serverVersion.Major < 11))
                {
                    // Return Empty Permissions
                    return new Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                        MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
                }
                try
                {
                    CollectionPermissions collectionPermissions = CollectionPermissions.None;
                    if (msi.CloudProviderId == Constants.MicrosoftAzureId)
                    {
                        using (SqlCommand permissionsCommand =
                            Probes.Sql.SqlCommandBuilder.BuildMasterPermissionsCommand(connection, connectionInfo,
                                msi.CloudProviderId))
                        {
                            // To Ensure Closing of DataReader
                            using (SqlDataReader permissionsReader = permissionsCommand.ExecuteReader())
                            {
                                // Read Required Permissions
                                collectionPermissions =
                                    ServerOverviewInterpreter
                                        .ReadPermissionsToEnum<CollectionPermissions>(
                                            permissionsReader, LOG);
                            }
                        }
                    }

                    using (SqlCommand permissionsCommand =
                        Probes.Sql.SqlCommandBuilder.BuildPermissionsCommand(connection, connectionInfo,
                            msi.CloudProviderId))
                    {
                        // To Ensure Closing of DataReader
                        using (SqlDataReader permissionsReader = permissionsCommand.ExecuteReader())
                        {
                            // Read Required Permissions
                            var minimumPermissions =
                                ServerOverviewInterpreter.ReadPermissionsToEnum<MinimumPermissions>(
                                    permissionsReader, LOG);
                            var metadataPermissions =
                                ServerOverviewInterpreter
                                    .ReadPermissionsToEnum<MetadataPermissions>(
                                        permissionsReader, LOG);
                            if (msi.CloudProviderId == Constants.MicrosoftAzureId)
                            {
                                collectionPermissions |=
                                    ServerOverviewInterpreter
                                        .ReadPermissionsToEnum<CollectionPermissions>(
                                            permissionsReader, LOG);
                            }
                            else
                            {
                                collectionPermissions =
                                    ServerOverviewInterpreter
                                        .ReadPermissionsToEnum<CollectionPermissions>(
                                            permissionsReader, LOG);
                            }

                            // Read Replication Permissions
                            collectionPermissions |=
                                ServerOverviewInterpreter
                                    .ReadPermissionsToEnum<CollectionPermissions>(
                                        permissionsReader, LOG);
                            // Returns Permissions
                            return new Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                                minimumPermissions, metadataPermissions, collectionPermissions);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error(string.Format(
                        "Exception occured while reading permisssions for Monitored server {0} with SQL Server Version {1}. Exception : {2}",
                        msi.InstanceName, serverVersion, exception));
                }

            }
            // Return Empty Permissions
            return new Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
        }

        public Triple<ServerVersion, DateTime, DateTime> GetServerTimeAndVersion(int monitoredSqlServerId)
        {
            ServerVersion serverVersion = null;
            DateTime timeStampLocal = DateTime.MinValue;

            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                serverVersion = new ServerVersion(connection.ServerVersion);

                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand timeStampLocalCmd = connection.CreateCommand())
                {
                    timeStampLocalCmd.CommandText = BatchConstants.GetDateTimeLocal;
                    timeStampLocal = (DateTime)timeStampLocalCmd.ExecuteScalar();
                }

            }
            return new Triple<ServerVersion, DateTime, DateTime>(serverVersion, timeStampLocal, DateTime.Now);
        }


		//SQLDM-30197
		public string GetPreferredClusterNode(int monitoredSqlServerId)
		{
			MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand command = new SqlCommand(BatchConstants.PreferredClusterNode, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        if (msi.ClusterCollectionSetting == ClusterCollectionSetting.Default && !reader.IsDBNull(0) && !(reader.GetInt32(0) > 0))
                        {
                            throw new ApplicationException("Monitored SQL Server is not clustered");
                        }
                        else
                        {
                            if (msi.ClusterCollectionSetting != ClusterCollectionSetting.ForceClusteredWithRegread && !reader.IsDBNull(1))
                            {
                                return reader.GetString(1);
                            }
                            if (reader.NextResult() && reader.Read() && !reader.IsDBNull(1))
                            {
                                return reader.GetString(1);
                            }
                        }
                    }
                }
            }
            return null;
		}
		
        public string GetCurrentClusterNode(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                CheckSupportedServer(connection, msi.InstanceName);


                using (SqlCommand command = new SqlCommand(BatchConstants.CurrentClusterNode, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        if (msi.ClusterCollectionSetting == ClusterCollectionSetting.Default && !reader.IsDBNull(0) && !(reader.GetInt32(0) > 0))
                        {
                            throw new ApplicationException("Monitored SQL Server is not clustered");
                        }
                        else
                        {
                            if (msi.ClusterCollectionSetting != ClusterCollectionSetting.ForceClusteredWithRegread && !reader.IsDBNull(1))
                            {
                                return reader.GetString(1);
                            }
                            if (reader.NextResult() && reader.Read() && !reader.IsDBNull(1))
                            {
                                return reader.GetString(1);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<string> GetDisks(int instanceID)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(instanceID);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored,");

            List<string> result = new List<string>();
            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                var serverVersion = new ServerVersion(connection.ServerVersion);

                // Fixed Drive if SQL Server version >= 2008
                var fixedDrives = serverVersion.IsGreaterThanSql2008Sp1R2()
                                      ? string.Format(Constants.FixedDrives2008, Constants.ExecMasterDboXpFixedDrives)
                                      : Constants.ExecMasterDboXpFixedDrives;

                // Remote Drive if SQL Server version >= 2008
                var remoteDrives = serverVersion.IsGreaterThanSql2008Sp1R2()
                                       ? string.Format(
                                           Constants.FixedDrives2008,
                                           Constants.ExecMasterDboXpFixedDrivesRemote)
                                       : Constants.ExecMasterDboXpFixedDrivesRemote;

                using (SqlCommand command = new SqlCommand(fixedDrives, connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string drive = reader.GetString(0);
                            if (!result.Contains(drive))
                            {
                                result.Add(drive);
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(remoteDrives, connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string drive = reader.GetString(0);
                            if (!result.Contains(drive))
                            {
                                result.Add(drive);
                            }
                        }
                    }
                }


            }
            return result;
        }


        public IDictionary<string, bool> GetDatabases(int monitoredSqlServerId, bool includeSystemDatabases, bool includeUserDatabases)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            Dictionary<string, bool> result = new Dictionary<string, bool>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                CheckSupportedServer(connection, msi.InstanceName);

                ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);

                StringBuilder builder = new StringBuilder();
                string isSysDB;
                if (serverVersion.Major < 9)
                {
                    isSysDB = "cast(case when name in ('master','model','msdb','tempdb') then 1 else category & 16 end as bit)";
                    builder.AppendFormat("select name, {0} as [is_distributor] from sysdatabases", isSysDB);
                }
                else
                {
                    isSysDB = "cast(case when name in ('master','model','msdb','tempdb') then 1 else is_distributor end as bit)";
                    builder.AppendFormat("select name, {0} from master.sys.databases", isSysDB);
                }
                if (!(includeSystemDatabases && includeUserDatabases))
                {
                    int bit = includeSystemDatabases ? 1 : 0;
                    builder.AppendFormat(" where {0} = {1}", isSysDB, bit);
                }

                using (SqlCommand command = new SqlCommand(builder.ToString(), connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            result.Add(name, reader.GetBoolean(1));
                        }
                    }
                }
            }
            return result;
        }

        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --Get all the filegroups for a SQL server
        public IList<string> GetFilegroupsOfInstance(int monitoredSqlServerId, string databaseName, bool isDefaultThreshold)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            List<string> result = new List<string>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();

                CheckSupportedServer(connection, msi.InstanceName);

                ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);
                List<string> databases = new List<string>();

                if (isDefaultThreshold)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("select distinct name from sysdatabases");

                    using (SqlCommand command = new SqlCommand(builder.ToString(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                if (name != null && name != "")
                                    databases.Add(name);
                            }
                        }
                    }

                    foreach (string database in databases)
                    {
                        StringBuilder filegroupQuery = new StringBuilder();
                        filegroupQuery.AppendFormat("use {0}; ", database);
                        filegroupQuery.Append("select groupname from sysfilegroups");

                        using (SqlCommand command = new SqlCommand(filegroupQuery.ToString(), connection))
                        {
                            command.CommandType = CommandType.Text;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    bool flag = true;
                                    string name = reader.GetString(0);
                                    foreach (string filegroup in result)
                                    {
                                        if (name.Equals(filegroup, StringComparison.OrdinalIgnoreCase))
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag)
                                        result.Add(name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("use {0}; ", databaseName);
                    builder.Append("select groupname from sysfilegroups");

                    using (SqlCommand command = new SqlCommand(builder.ToString(), connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool flag = true;
                                string name = reader.GetString(0);
                                foreach (string filegroup in result)
                                {
                                    if (name.Equals(filegroup, StringComparison.OrdinalIgnoreCase))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                    result.Add(name);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<Triple<string, string, bool>> GetTables(int monitoredSqlServerId, string database, bool includeSystemTables, bool includeUserTables)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            List<Triple<string, string, bool>> result = new List<Triple<string, string, bool>>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = database;

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);
                // SQL 2005 supports schemas so we have to do something different to convert the uid to a name
                // if we get null for schema name then use 'dbo' as the value
                string schemaFunction = serverVersion.Major >= 9 ? "schema_name" : "user_name";
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("select coalesce({0}(uid),'dbo') as [owner], name, xtype, category from sysobjects where ", schemaFunction);
                if (includeUserTables)
                {
                    if (includeSystemTables)
                        builder.Append("xtype in ('U','S')");
                    else
                        builder.Append("xtype='U' and category <> 2");
                }
                else
                    builder.Append("xtype='S' or (xtype='U' and category=2)");

                using (SqlCommand command = new SqlCommand(builder.ToString(), connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bool system = (reader.GetString(2).Trim() == "S") || (2 & reader.GetInt32(3)) == 2;
                            result.Add(new Triple<string, string, bool>(reader.GetString(0), reader.GetString(1), system));
                            //result.Add(reader.GetString(0) + "." + reader.GetString(1), system);
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<string> GetAgentJobNames(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            List<string> result = new List<string>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "msdb";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlDataReader reader = SqlHelper.ExecuteReader(
                            connection,
                            "sp_help_job",
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null))
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(reader.GetOrdinal("name")));
                    }
                }
            }
            return result;
        }

        public IEnumerable<string> GetAgentJobCategories(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            List<string> result = new List<string>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "msdb";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand command = new SqlCommand("sp_help_category", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@class", "JOB");
                    command.Parameters.AddWithValue("@type", "LOCAL");
                    command.Parameters.AddWithValue("@name", DBNull.Value);
                    command.Parameters.AddWithValue("@suffix", DBNull.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(2));
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<CategoryJobStep> GetAgentJobStepList(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            List<CategoryJobStep> result = new List<CategoryJobStep>();

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "msdb";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlDataReader reader =

                            SqlHelper.ExecuteReader(connection, CommandType.Text, BatchConstants.GetCategoryJobStepList))
                {
                    while (reader.Read())
                    {
                        CategoryJobStep stepDetail = new CategoryJobStep();
                        if (!reader.IsDBNull(0)) stepDetail.Category = reader.GetString(0);
                        if (!reader.IsDBNull(1)) stepDetail.JobName = reader.GetString(1);
                        if (!reader.IsDBNull(2)) stepDetail.StepName = reader.GetString(2);
                        result.Add(stepDetail);
                    }
                }
            }
            return result;
        }


        #region SysPerfInfo Counter Lists

        public DataTable GetSysPerfInfoObjectList(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            DataTable result = new DataTable("Object Names");
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Singleton", typeof(bool));

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select object_name, sum(case when rtrim(instance_name) = '' then 0 else 1 end) from master..sysperfinfo group by object_name order by object_name";
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string objectName = reader.GetString(0).TrimEnd();
                            // lop off the instance name
                            int i = objectName.LastIndexOf(':');
                            if (i >= 0)
                                objectName = objectName.Substring(i + 1);

                            DataRow row = result.NewRow();
                            row[0] = objectName;
                            row[1] = reader.GetInt32(1) == 0;

                            result.Rows.Add(row);
                        }
                    }
                }
            }
            return result;
        }

        #region SysPerfInfo Constants

        private const string SYSPERFINFO_OBJECTNAME =
        @"declare " +
        @"    @servername varchar(255), " +
        @"    @sysperfinfoname varchar(255), " +
        @"    @slashpos int " +
        @"select @servername = cast(serverproperty('servername') as nvarchar(255)) " +
        @"select @servername = upper(@servername) " +
        @"select @slashpos = charindex('\', @servername) " +
        @"if @slashpos <> 0 " +
        @"begin " +
            @"select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) " +
        @"end  " +
        @"else " +
        @"begin " +
            @"select @sysperfinfoname = 'SQLSERVER'" +
        @"end  " +
        @"select @sysperfinfoname = lower(@sysperfinfoname + ':' + @object)";

        #endregion

        public DataTable GetSysPerfInfoCounterList(int monitoredSqlServerId, string objectName)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            DataTable result = new DataTable("Counter Names");
            result.Columns.Add("Name", typeof(string));

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = SYSPERFINFO_OBJECTNAME + " select distinct counter_name from master..sysperfinfo where lower(object_name) = @sysperfinfoname";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@object", objectName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = result.NewRow();
                            row[0] = reader.GetString(0).TrimEnd();
                            result.Rows.Add(row);
                        }
                    }
                }
            }
            return result;
        }

        public DataTable GetSysPerfInfoInstanceList(int monitoredSqlServerId, string objectName)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            DataTable result = new DataTable("Instance Names");
            result.Columns.Add("Name", typeof(string));

            SqlConnectionInfo connectionInfo = msi.ConnectionInfo.Clone();
            connectionInfo.DatabaseName = "master";

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                CheckSupportedServer(connection, msi.InstanceName);

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = SYSPERFINFO_OBJECTNAME + " select distinct instance_name from master..sysperfinfo where lower(object_name) = @sysperfinfoname";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@object", objectName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = result.NewRow();
                            row[0] = reader.GetString(0).TrimEnd();
                            result.Rows.Add(row);
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region VM Counter Lists

        public DataTable GetVmCounterObjectList(int monitoredSqlServerId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(monitoredSqlServerId);
            if (msi == null)
                throw new ApplicationException("Monitored SQL Server Instance not being monitored.");

            if (msi.IsVirtualized == false)
                throw new ApplicationException("Monitored SQL Server Instance is not configued for monitoring virtualization.");

            string serverType = msi.VirtualizationConfiguration.VCServerType;

            DataTable result = new DataTable("Object Names");
            result.Columns.Add("Source", typeof(string));
            result.Columns.Add("Group", typeof(string));
            result.Columns.Add("GroupLabel", typeof(string));
            result.Columns.Add("GroupSummary", typeof(string));
            result.Columns.Add("CounterKey", typeof(int));
            result.Columns.Add("Counter", typeof(string));
            result.Columns.Add("CounterLabel", typeof(string));
            result.Columns.Add("CounterSummary", typeof(string));
            result.Columns.Add("Instance", typeof(string));
            result.Columns.Add("UOM", typeof(string));
            result.Columns.Add("Singleton", typeof(bool));
            if (serverType == "HyperV")
            {
                try
                {
                    HyperVCustomCounter hyperVCustomCounter = new HyperVCustomCounter();
                    Dictionary<string, CustomCounterBasicInfo> hyperVCustomCounterObjects = hyperVCustomCounter.HyperVCustomCounterObjects;
                    foreach (KeyValuePair<string, CustomCounterBasicInfo> item in hyperVCustomCounterObjects)
                    {
                        DataRow row = result.NewRow();
                        Dictionary<string, List<string>> counterList = item.Value.CounterList;
                        if (counterList.ContainsKey("Type"))
                        {
                            List<string> sList = counterList["Type"];
                            if (sList.Contains("VM"))
                            {
                                row[0] = "VM";
                                row[1] = item.Value.Group;
                                row[2] = item.Key;
                                row[3] = item.Value.GroupSummary;
                                row[4] = item.Value.CounterKey;
                                row[5] = item.Value.Counter;
                                row[6] = item.Value.CounterLabel;
                                row[7] = item.Value.CounterSummary;
                                row[8] = item.Value.Instance;
                                row[9] = item.Value.UOM;
                                row[10] = item.Value.Singleton;
                                result.Rows.Add(row);
                            }
                        }
                    }

                    foreach (KeyValuePair<string, CustomCounterBasicInfo> item in hyperVCustomCounterObjects)
                    {
                        DataRow row = result.NewRow();
                        Dictionary<string, List<string>> counterList = item.Value.CounterList;
                        if (counterList.ContainsKey("Type"))
                        {
                            List<string> sList = counterList["Type"];
                            if (sList.Contains("Host"))
                            {
                                row[0] = "Host";
                                row[1] = item.Value.Group;
                                row[2] = item.Key;
                                row[3] = item.Value.GroupSummary;
                                row[4] = item.Value.CounterKey;
                                row[5] = item.Value.Counter;
                                row[6] = item.Value.CounterLabel;
                                row[7] = item.Value.CounterSummary;
                                row[8] = item.Value.Instance;
                                row[9] = item.Value.UOM;
                                row[10] = item.Value.Singleton;
                                result.Rows.Add(row);
                            }
                        }

                    }

                    return result;
                }
                catch (Exception exp)
                {
                    LOG.Error("Hyper - Error retrieving VM counter List " + exp.Message);
                }
            }


            Dictionary<int, PerfCounterInfo> perfInfo = null;
            PerfMetricId[] vmMetricIds = null;
            PerfMetricId[] esxMetricIds = null;

            // connect to vcenter server
            VirtualizationConfiguration config = msi.VirtualizationConfiguration;
            ServiceUtil serviceUtil = new ServiceUtil(config.VCAddress);
            try
            {
                serviceUtil.Connect(config.VCUser, config.VCPassword);

                // get a list of all the available perf counter metadata
                perfInfo = serviceUtil.GetPerfCounterInfo();

                // get a reference to the vm managed object
                ManagedObjectReference vmMoRef = serviceUtil.getManagedObject(config.InstanceUUID, true,
                                                                              VIMSearchType.UUID);
                // get a reference to the esx server hosting the vm 
                ManagedObjectReference esxMoRef = serviceUtil.getESXHost(vmMoRef);

                // get all the perf counters available for the vm
                vmMetricIds = serviceUtil.getPerfMetricIDs(vmMoRef, perfInfo, PerfMetricIdComparer);
                // get all the perf counters available for the esx host
                esxMetricIds = serviceUtil.getPerfMetricIDs(esxMoRef, perfInfo, PerfMetricIdComparer);
            }
            finally
            {
                // disconnect from vCenter
                serviceUtil.Disconnect();
            }

            int last = -1;
            bool singleton = true;
            PerfCounterInfo perfCounterInfo = null;

            foreach (PerfMetricId id in FilterSingleInstance(vmMetricIds))
            {
                if (perfInfo.TryGetValue(id.counterId, out perfCounterInfo))
                {
                    DataRow row = result.NewRow();
                    row[0] = "VM";
                    row[1] = perfCounterInfo.groupInfo.key;
                    row[2] = perfCounterInfo.groupInfo.label;
                    row[3] = perfCounterInfo.groupInfo.summary;
                    row[4] = perfCounterInfo.key;
                    row[5] = perfCounterInfo.nameInfo.key;
                    row[6] = perfCounterInfo.nameInfo.label;
                    row[7] = perfCounterInfo.nameInfo.summary;
                    row[8] = String.IsNullOrEmpty(id.instance) ? "_Total" : id.instance;
                    row[9] = perfCounterInfo.unitInfo.label;

                    if (last != id.counterId)
                    {
                        last = id.counterId;
                        singleton = String.IsNullOrEmpty(id.instance);
                        if (singleton)
                            row[8] = id.instance;
                    }

                    row[10] = singleton;
                    result.Rows.Add(row);

                }
            }

            last = -1;
            singleton = true;
            foreach (PerfMetricId id in FilterSingleInstance(esxMetricIds))
            {
                if (perfInfo.TryGetValue(id.counterId, out perfCounterInfo))
                {
                    DataRow row = result.NewRow();
                    row[0] = "ESX";
                    row[1] = perfCounterInfo.groupInfo.key;
                    row[2] = perfCounterInfo.groupInfo.label;
                    row[3] = perfCounterInfo.groupInfo.summary;
                    row[4] = perfCounterInfo.key;
                    row[5] = perfCounterInfo.nameInfo.key;
                    row[6] = perfCounterInfo.nameInfo.label;
                    row[7] = perfCounterInfo.nameInfo.summary;
                    row[8] = String.IsNullOrEmpty(id.instance) ? "_Total" : id.instance;
                    row[9] = perfCounterInfo.unitInfo.label;

                    if (last != id.counterId)
                    {
                        last = id.counterId;
                        singleton = String.IsNullOrEmpty(id.instance);
                        if (singleton)
                            row[8] = id.instance;
                    }
                    row[10] = singleton;
                    result.Rows.Add(row);
                }
            }

            return result;
        }

        static List<PerfMetricId> FilterSingleInstance(PerfMetricId[] metrics)
        {
            List<PerfMetricId> result = new List<PerfMetricId>();

            int lastChecked = -1;
            for (int i = 0; i < metrics.Length; i++)
            {
                PerfMetricId id = metrics[i];
                if (lastChecked != id.counterId)
                {
                    lastChecked = id.counterId;

                    if (!String.IsNullOrEmpty(id.instance) && (i + 1) < metrics.Length)
                    {
                        PerfMetricId next = metrics[i + 1];
                        if (next.counterId == id.counterId && String.IsNullOrEmpty(next.instance))
                            continue;
                    }
                }
                result.Add(id);
            }

            return result;
        }

        static int PerfMetricIdComparer(PerfMetricId left, PerfMetricId right)
        {
            int rc = left.counterId.CompareTo(right.counterId);
            if (rc == 0) rc = right.instance.CompareTo(left.instance);
            return rc;
        }

        #endregion

        public string GetMachineName(int instanceId)
        {
            string serverName = null;
            MonitoredSqlServer msi = GetMonitoredServerInfo(instanceId);
            if (msi == null)
                throw new ApplicationException("Instance not found for given id");

            using (SqlConnection connection = msi.ConnectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
            {
                connection.Open();
                serverName = SqlHelper.ExecuteScalar(connection, CommandType.Text, "select cast(serverproperty('MachineName') as varchar(255))") as string;
            }
            return serverName;
        }

        private string GetMachineName(SqlConnectionInfo connectionInfo)
        {
            connectionInfo.ApplicationName = Constants.CollectionServceConnectionStringApplicationName;
            return SqlHelper.ExecuteScalar(connectionInfo.ConnectionString, CommandType.Text, "select cast(serverproperty('MachineName') as varchar(255))") as string;
        }

        #region Azure Monitor Counter

        public Serialized<DataTable> GetAzureMonitorDefinitions(IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorDefinitions"))
            {
                var result = new DataTable("Metric Definitions");
                result.Columns.Add("Id", typeof(string));
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("DisplayName", typeof(string));
                result.Columns.Add("ResourceId", typeof(string));
                result.Columns.Add("Unit", typeof(string));
                result.Columns.Add("NamespaceProperty", typeof(string));
                result.Columns.Add("PrimaryAggregationType", typeof(string));
                result.BeginLoadData();
                try
                {
                    var client = new AzureManagementClient();
                    var metricDefinitions = client.GetMetricDefinitions(monitorConfiguration).GetAwaiter().GetResult();
                    if (metricDefinitions != null)
                    {
                        var metricDefinitionsSorted = metricDefinitions.ToList();
                        metricDefinitionsSorted.Sort((m1, m2) =>
                            string.Compare(m1.Name.LocalizedValue, m2.Name.LocalizedValue));
                        foreach (var metricDefinition in metricDefinitionsSorted)
                        {
                            // Dimension Metric Definitions are collected using the particular dimension
                            if (metricDefinition.IsDimensionRequired == true)
                            {
                                continue;
                            }

                            var row = result.NewRow();
                            row["Id"] = metricDefinition.Id;
                            row["Name"] = metricDefinition.Name.Value;
                            row["DisplayName"] = metricDefinition.Name.LocalizedValue;
                            row["ResourceId"] = metricDefinition.ResourceId;
                            row["Unit"] = metricDefinition.Unit;
                            row["NamespaceProperty"] = metricDefinition.NamespaceProperty;
                            row["PrimaryAggregationType"] = metricDefinition.PrimaryAggregationType;
                            result.Rows.Add(row);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error("Error retrieving Azure Monitor Namespaces list for " + monitorConfiguration.Profile.ApplicationProfile.Name + ": ", exception);
                    throw;
                }
                finally
                {
                    result.EndLoadData();
                }
                return result;
            }
        }
        public Serialized<DataTable> GetAzureMonitorNamespaces(IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorNamespaces"))
            {
                var result = new DataTable("Monitor Namespaces");
                result.Columns.Add("Id", typeof(string));
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("Type", typeof(string));
                result.Columns.Add("MetricNamespaceName", typeof(string));
                result.BeginLoadData();
                try
                {
                    var client = new AzureManagementClient();
                    foreach (var metricNamespace in client.GetMetricNamespaces().GetAwaiter().GetResult())
                    {
                        var row = result.NewRow();
                        row["Id"] = metricNamespace.Id;
                        row["Name"] = metricNamespace.Name;
                        row["Type"] = metricNamespace.Type;
                        row["MetricNamespaceName"] = metricNamespace.Properties.MetricNamespaceNameProperty;
                        result.Rows.Add(row);
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error("Error retrieving Azure Monitor Namespaces list for " + monitorConfiguration.Profile.ApplicationProfile.Name + ": ", exception);
                    throw;
                }
                finally
                {
                    result.EndLoadData();
                }
                return result;
            }
        }

        #endregion

        public DataTable GetAzureDatabase(int instanceId)
        {
            using (LOG.InfoCall("GetAzureDatabase"))
            {
                var result = new DataTable("Azure Databases");
                result.Columns.Add("DatabaseName", typeof(string));
                result.Columns.Add("ElasticPoolName", typeof(string));
                result.BeginLoadData();

                var msi = GetMonitoredServerInfo(instanceId);
                if (msi == null)
                {
                    throw new ApplicationException("Monitored SQL Server Instance not being monitored.");
                }

                try
                {
                    var connectionInfo = msi.ConnectionInfo.Clone();
                    connectionInfo.DatabaseName = "master";

                    using (var connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
                    {
                        connection.Open();

                        var builder = new StringBuilder();
                        builder.AppendLine(@"
-- Azure database names with elastic pool details
SELECT d.name, dso.elastic_pool_name 
FROM sys.databases d (NOLOCK)
LEFT JOIN sys.database_service_objectives dso (NOLOCK)
ON d.database_id = dso.database_id AND dso.service_objective = 'ElasticPool';
");
                        using (var command = new SqlCommand(builder.ToString(), connection))
                        {
                            command.CommandType = CommandType.Text;
                            using (var reader = command.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    return result;
                                }
                                var nameOrdinal = reader.GetOrdinal("name");
                                var elasticPoolNameOrdinal = reader.GetOrdinal("elastic_pool_name");
                                while (reader.Read())
                                {
                                    var row = result.NewRow();
                                    if (reader.IsDBNull(nameOrdinal))
                                    {
                                        row["DatabaseName"] = reader.GetString(nameOrdinal);
                                    }
                                    if (reader.IsDBNull(elasticPoolNameOrdinal))
                                    {
                                        row["ElasticPoolName"] = reader.GetString(elasticPoolNameOrdinal);
                                    }

                                    result.Rows.Add(row);
                                }
                            }
                        }
                    }
                    return result;
                }
                catch (Exception me)
                {
                    LOG.Error("Error retrieving Azure Databases for " + msi.DisplayInstanceName + ": ", me);
                    throw;
                }
                finally
                {
                    result.EndLoadData();
                }
            }
        }

        #region WMI Counter Lists

        public DataTable GetWmiObjectList(int instanceId, out string serverName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiObjectList"))
            {
                DataTable result = new DataTable("WMI Objects");
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("Description", typeof(string));
                result.Columns.Add("DisplayName", typeof(string));
                result.Columns.Add("Singleton", typeof(bool));
                result.BeginLoadData();

                serverName = GetMachineName(instanceId);

                WqlObjectQuery query = new WqlObjectQuery("select * from meta_class");
                ManagementScope scope = GetManagementScope(serverName);
                scope.Options.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    searcher.Options.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
                    searcher.Options.UseAmendedQualifiers = true;
                    searcher.Options.ReturnImmediately = true;

                    ManagementObjectCollection queryResult = searcher.Get();
                    if (queryResult != null)
                    {
                        using (queryResult)
                        {
                            try
                            {
                                foreach (ManagementClass mc in queryResult)
                                {
                                    bool dynamicOrStatic = false;
                                    string friendlyName = null;
                                    string description = String.Empty;
                                    bool singleton = false;
                                    using (mc)
                                    {
                                        // only return dynamic or static classes
                                        foreach (QualifierData qd in mc.Qualifiers)
                                        {
                                            switch (qd.Name.ToLower())
                                            {
                                                case "dynamic":
                                                case "static":
                                                    dynamicOrStatic = true;
                                                    break;
                                                case "description":
                                                    description = qd.Value.ToString();
                                                    break;
                                                case "displayname":
                                                    friendlyName = qd.Value.ToString();
                                                    break;
                                                case "singleton":
                                                    singleton = (bool)qd.Value;
                                                    break;
                                            }
                                        }
                                        if (dynamicOrStatic && HasSupportedProperty(mc))
                                        {
                                            DataRow row = result.NewRow();
                                            row[0] = mc["__CLASS"].ToString();
                                            row[1] = description;
                                            if (!String.IsNullOrEmpty(friendlyName))
                                                row[2] = friendlyName;
                                            row[3] = singleton;
                                            result.Rows.Add(row);
                                        }
                                    }
                                }
                            }
                            catch (ManagementException me)
                            {
                                LOG.Error("Error retrieving wmi object list for " + serverName + ": ", me);
                                if (me.ErrorCode == ManagementStatus.Timedout)
                                {
                                    result.EndLoadData();
                                    CollectionServiceException cse = new CollectionServiceException("The object list request timed out.", me);
                                    cse.Data.Add("DataTable", result);
                                }
                                throw;
                            }
                            finally
                            {
                                result.EndLoadData();
                            }
                        }
                    }
                }
                return result;
            }
        }

        private static bool HasSupportedProperty(ManagementClass mc)
        {
            try
            {
                foreach (PropertyData pd in mc.Properties)
                {
                    if (IsSupportedProperty(pd))
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool IsSupportedProperty(PropertyData pd)
        {
            if (pd.IsArray)
                return false;

            // weed out unsupported types
            switch (pd.Type)
            {
                case CimType.DateTime:
                case CimType.None:
                case CimType.Object:
                case CimType.Reference:
                case CimType.String:
                    return false;
            }
            return true;
        }

        private static bool IsSingleton(ManagementClass managementClass)
        {
            try
            {
                object value = managementClass.GetQualifierValue("singleton");
                if (value is bool)
                    return (bool)value;
            }
            catch
            {
                /* */
            }

            return false;
        }

        private static ManagementScope GetManagementScope(string serverName)
        {
            return GetManagementScope(serverName, new ConnectionOptions());
        }

        private static ManagementScope GetManagementScope(string serverName, ConnectionOptions connectOptions)
        {
            return new ManagementScope(String.Format(@"\\{0}\root\cimv2", serverName), connectOptions);
        }

        public DataTable GetWmiCounterList(string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiCounterList"))
            {
                DataTable result = new DataTable("WMI Objects");
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("Description", typeof(string));
                result.Columns.Add("DisplayName", typeof(string));
                result.BeginLoadData();

                StringBuilder description = new StringBuilder();

                ManagementScope scope = GetManagementScope(serverName);
                scope.Options.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
                ManagementPath path = new ManagementPath(objectName);
                ObjectGetOptions getOptions = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
                try
                {
                    using (ManagementClass mc = new ManagementClass(scope, path, getOptions))
                    {
                        foreach (PropertyData pd in mc.Properties)
                        {
                            if (!IsSupportedProperty(pd))
                                continue;

                            // build a description
                            description.Length = 0;
                            string friendlyName = null;
                            foreach (QualifierData qd in pd.Qualifiers)
                            {
                                if (qd.Name.Equals("Description"))
                                {
                                    description.Append(mc.GetPropertyQualifierValue(pd.Name, qd.Name));
                                    description.Append(System.Environment.NewLine);
                                }
                                else if (qd.Name.Equals("DisplayName"))
                                    friendlyName = qd.Value.ToString();
                            }

                            DataRow row = result.NewRow();
                            row[0] = pd.Name;
                            row[1] = description.ToString();
                            if (!String.IsNullOrEmpty(friendlyName))
                                row[2] = friendlyName;
                            result.Rows.Add(row);
                        }
                    }
                }
                catch (ManagementException me)
                {
                    LOG.Error("Error retrieving wmi counter list for " + serverName + ": ", me);
                    if (me.ErrorCode == ManagementStatus.Timedout)
                    {
                        CollectionServiceException cse = new CollectionServiceException("The counter list request timed out.", me);
                        cse.Data.Add("DataTable", result);
                        throw cse;
                    }

                    throw;
                }
                finally
                {
                    result.EndLoadData();
                }
                return result;
            }
        }

        public DataTable GetWmiInstanceList(string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiInstanceList"))
            {
                DataTable result = new DataTable("WMI Objects");
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("DisplayName", typeof(string));

                ManagementScope scope = GetManagementScope(serverName);
                scope.Options.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;

                ManagementPath path = new ManagementPath(objectName);
                ObjectGetOptions getOptions =
                    new ObjectGetOptions(null, TimeSpan.FromSeconds(SqlHelper.CommandTimeout), true);

                using (ManagementClass mc = new ManagementClass(scope, path, getOptions))
                {
                    if (!IsSingleton(mc))
                    {
                        result.BeginLoadData();
                        try
                        {
                            GetWmiInstanceList(result, mc);
                        }
                        catch (ManagementException me)
                        {
                            LOG.Error(String.Format("Error retrieving wmi instance list from {0} for {1}", serverName, objectName), me);
                            if (me.ErrorCode == ManagementStatus.Timedout)
                            {
                                if (result.Rows.Count == 0)
                                    AddWmiInstanceExample(mc, result);
                                CollectionServiceException cse =
                                    new CollectionServiceException("The instance list request timed out.", me);
                                cse.Data.Add("DataTable", result);
                                throw cse;
                            }
                            throw;
                        }
                        catch (TimeoutException te)
                        {
                            LOG.Error(
                                String.Format("Timeout retrieving wmi instance list from {0} for {1}", serverName, objectName), te);
                            if (result.Rows.Count == 0)
                                AddWmiInstanceExample(mc, result);
                            CollectionServiceException cse =
                                new CollectionServiceException("The instance list request timed out.");
                            cse.Data.Add("DataTable", result);
                            throw cse;
                        }
                        finally
                        {
                            result.EndLoadData();
                        }
                    }
                }
                return result;
            }
        }

        private void AddWmiInstanceExample(ManagementClass managementClass, DataTable dataTable)
        {
            using (LOG.InfoCall("AddWmiInstanceExample"))
            {
                StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

                EnumerationOptions enumOptions = new EnumerationOptions();
                enumOptions.Timeout = TimeSpan.FromSeconds(10);
                enumOptions.ReturnImmediately = true;
                enumOptions.UseAmendedQualifiers = true;

                DataRow exampleRow = dataTable.NewRow();
                try
                {
                    foreach (string className in managementClass.Derivation)
                    {
                        if (comparer.Compare(className, "CIM_LogicalFile") == 0)
                        {
                            dataTable.Rows.Add("Name=\"<fully qualified file name>\"",
                                               "<fully qualified file name>");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error getting subclasses for class: ", e);
                }
            }
        }

        private void GetWmiInstanceList(DataTable dataTable, ManagementClass managementClass)
        {
            EnumerationOptions enumOptions = new EnumerationOptions();
            enumOptions.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
            enumOptions.ReturnImmediately = true;
            enumOptions.UseAmendedQualifiers = true;

            DateTime quittingTime = DateTime.Now + enumOptions.Timeout;

            using (ManagementObjectCollection instances = managementClass.GetInstances(enumOptions))
            {
                using (ManagementObjectCollection.ManagementObjectEnumerator semiSyncEnumerator =
                    instances.GetEnumerator())
                {
                    while (semiSyncEnumerator.MoveNext())
                    {
                        ManagementObject instance = semiSyncEnumerator.Current as ManagementObject;
                        if (instance != null)
                        {
                            ManagementPath instancePath = instance.Path;
                            string relativePath = instancePath.RelativePath;
                            string className = instancePath.ClassName;
                            if (className.Length > 0 && relativePath.StartsWith(relativePath))
                                relativePath = relativePath.Substring(className.Length + 1);

                            if (!String.IsNullOrEmpty(relativePath))
                            {
                                DataRow row = dataTable.NewRow();
                                row[0] = relativePath;
                                string[] parts = relativePath.Split('=');
                                if (parts.Length == 2)
                                {
                                    relativePath = parts[1].Replace('"', ' ');
                                    row[1] = relativePath.Trim();
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                        if (DateTime.Now > quittingTime)
                        {
                            throw new TimeoutException();
                        }
                    }
                }
            }
        }

        #endregion

        public object TestCustomCounter(int monitoredSqlServer, Idera.SQLdm.Common.Events.CustomCounterDefinition counterDefinition)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.DataTable GetDriveConfiguration(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetDriveConfiguration"))
            {
                string machineName = GetMachineName(connectionInfo);
                if (string.IsNullOrEmpty(machineName))
                    throw new ApplicationException(String.Format(
                        "Unable to get MachineName property from server '{0}'.",
                        connectionInfo.InstanceName));

                DataTable result = new DataTable("Win32_Volumes");
                result.Columns.Add("Name", typeof(string));
                result.Columns.Add("DriveLetter", typeof(string));
                result.Columns.Add("Label", typeof(string));
                result.Columns.Add("DeviceID", typeof(string));
                result.Columns.Add("DriveType", typeof(UInt32));
                result.Columns.Add("FileSystem", typeof(string));
                result.Columns.Add("Capacity", typeof(UInt64));
                result.Columns.Add("FreeSpace", typeof(UInt64));

                ImpersonationContext impersonation;
                var connectOptions = WmiCollector.CreateConnectionOptions(machineName, wmiConfiguration,
                                                                          out impersonation);

                ManagementScope scope = GetManagementScope(machineName, connectOptions);
                scope.Options.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;

                try
                {
                    if (impersonation == null)
                        GetDriveConfiguration(impersonation, scope, result, machineName);
                    else
                    {
                        // login user and get windows identity
                        if (!impersonation.IsLoggedOn)
                            impersonation.LogonUser();

                        // run delegate at user
                        impersonation.RunAs(() => GetDriveConfiguration(impersonation, scope, result, machineName));
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

                return result;
            }
        }

        private void GetDriveConfiguration(ImpersonationContext context, ManagementScope scope, DataTable result, string machineName)
        {
            ManagementPath path = new ManagementPath("Win32_Volume");
            ObjectGetOptions getOptions = new ObjectGetOptions(null, TimeSpan.FromSeconds(SqlHelper.CommandTimeout), true);

            ManagementClass mc = null;
            try
            {
                mc = new ManagementClass(scope, path, getOptions);
                ManagementPath classPath = mc.ClassPath;
            }
            catch (Exception e)
            {
                mc.Dispose();
                path = new ManagementPath("Win32_LogicalDisk");
                mc = new ManagementClass(scope, path, getOptions);
            }

            using (mc)
            {
                if (!IsSingleton(mc))
                {
                    result.BeginLoadData();
                    try
                    {
                        GetDriveConfiguration(result, mc);
                    }
                    catch (ManagementException me)
                    {
                        LOG.Error(String.Format("Error retrieving disk drive list from {0}", machineName), me);
                        if (me.ErrorCode == ManagementStatus.Timedout)
                        {
                            if (result.Rows.Count == 0)
                                AddWmiInstanceExample(mc, result);
                            CollectionServiceException cse =
                                new CollectionServiceException("The disk drive list request timed out.", me);
                            cse.Data.Add("DataTable", result);
                            throw cse;
                        }
                        throw;
                    }
                    catch (TimeoutException te)
                    {
                        LOG.Error(
                            String.Format("Timeout retrieving disk drive list from {0}", machineName), te);
                        if (result.Rows.Count == 0)
                            AddWmiInstanceExample(mc, result);
                        CollectionServiceException cse =
                            new CollectionServiceException("The disk drive list request timed out.");
                        cse.Data.Add("DataTable", result);
                        throw cse;
                    }
                    finally
                    {
                        result.EndLoadData();
                    }
                }
            }
        }

        private void GetDriveConfiguration(DataTable dataTable, ManagementClass managementClass)
        {
            EnumerationOptions enumOptions = new EnumerationOptions();
            enumOptions.Timeout = CollectionServiceConfiguration.WMIQueryTimeout;
            enumOptions.ReturnImmediately = true;
            enumOptions.UseAmendedQualifiers = true;

            DateTime quittingTime = DateTime.Now + enumOptions.Timeout;

            using (ManagementObjectCollection instances = managementClass.GetInstances(enumOptions))
            {
                using (ManagementObjectCollection.ManagementObjectEnumerator semiSyncEnumerator =
                    instances.GetEnumerator())
                {
                    while (semiSyncEnumerator.MoveNext())
                    {
                        ManagementObject instance = semiSyncEnumerator.Current as ManagementObject;
                        if (instance != null)
                        {
                            ManagementPath instancePath = instance.Path;
                            DataRow row = dataTable.NewRow();
                            row[0] = instance.GetPropertyValue("Name") ?? String.Empty;
                            row[4] = instance.GetPropertyValue("DriveType") ?? DBNull.Value;
                            row[5] = instance.GetPropertyValue("FileSystem") ?? DBNull.Value;
                            row[7] = instance.GetPropertyValue("FreeSpace") ?? DBNull.Value;

                            if (String.Equals(instancePath.ClassName, "Win32_Volume", StringComparison.Ordinal))
                            {
                                row[1] = instance.GetPropertyValue("DriveLetter") ?? DBNull.Value;
                                row[2] = instance.GetPropertyValue("Label") ?? DBNull.Value;
                                row[3] = instance.GetPropertyValue("DeviceID") ?? DBNull.Value;
                                row[6] = instance.GetPropertyValue("Capacity") ?? DBNull.Value;
                            }
                            else
                            {
                                if (row[4] is uint && 2 == (uint)row[4])
                                {
                                    // skip floppies
                                    object mt = instance.GetPropertyValue("MediaType");
                                    if (mt == null)
                                        continue;
                                    if (11 != (uint)mt)
                                        continue;
                                }
                                row[1] = instance.GetPropertyValue("DeviceID") ?? DBNull.Value;
                                row[2] = instance.GetPropertyValue("VolumeName") ?? DBNull.Value;
                                row[3] = DBNull.Value;
                                row[6] = instance.GetPropertyValue("Size") ?? DBNull.Value;
                            }
                            dataTable.Rows.Add(row);
                        }
                        if (DateTime.Now > quittingTime)
                        {
                            throw new TimeoutException();
                        }
                    }
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDisabledIndexes(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDisabledIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSqlModuleOptions(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildSqlModuleOptionsProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectSampleServerResources(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildSampleServerResourcesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectBackupAndRecovery(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildBackupAndRecoveryProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectQueryPlanEstRows(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildQueryPlanEstRowsProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectOutOfDateStats(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildOutOfDateStatsProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectNUMANodeCounters(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildNUMANodeCountersProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectMasterFiles(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildMasterFilesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDBSecurity(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDBSecurityProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiVolume(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiVolumeProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiProcess(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiProcessProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiTCP(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiTCPProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiTCPv4(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiTCPv4Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiTCPv6(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiTCPv6Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPerfOSSystem(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPerfOSSystemProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPerfOSMemory(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPerfOSMemoryProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiNetworkRedirector(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiNetworkRedirectorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiEncryptableVolume(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiEncryptableVolumeProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiComputerSystem(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiComputerSystemProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiProcessor(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiProcessorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPerfOSProcessor(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPerfOSProcessorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiNetworkInterface(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiNetworkInterfaceProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPageFile(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPageFileProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPhysicalMemory(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPhysicalMemoryProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPerfDiskLogicalDisk(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPerfDiskLogicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiPerfDiskPhysicalDisk(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiPerfDiskPhysicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWmiService(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWmiServiceProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWorstFillFactorIndexes(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildWorstFillFactorIndexesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectDatabaseRanking(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseRankingProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectQueryPlan(int serverId, ISnapshotSink sink, object state, string query)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildQueryPlanProbe(msi.ConnectionInfo, query, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// SQLdm 10.4 Nikhil Bansal - Get the Estimated Query Plan on demand
        /// </summary>
        /// <param name="config">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectEstimatedQueryPlan(EstimatedQueryPlanConfiguration config, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(config.MonitoredServerId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildEstimatedQueryPlanProbe(msi.ConnectionInfo, config, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectOverlappingIndexes(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildOverlappingIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectIndexContention(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildIndexContentionProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectHighIndexUpdates(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildHighIndexUpdatesProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectFragmentedIndexes(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildFragmentedIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectAdhocCachedPlanBytes(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildAdhocCachedPlanBytesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectLockedPageKB(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildLockedPageKBProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectServerConfiguration(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildServerConfigurationProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public Result CollectWaitingBatches(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;
            IProbe probe = ProbeFactory.BuildWaitingBatchesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        public Result CollectDatabaseNames(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDatabaseNamesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// 10.0 vineet-doctor integration. Executes optimization script for a set of recommendation
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result ExecutePrescriptiveOptimizationScript(int serverId, ISnapshotSink sink, object state, PrescriptiveScriptConfiguration configuration)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildPrescriptiveOptimizationProbe(msi.ConnectionInfo, configuration, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// 10.0 srishti purohit-doctor integration -- Gives the dependent objects of a particular table.
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result GetTableofDependentObject(int serverId, ISnapshotSink sink, object state, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName ObjectName)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;

            IProbe probe = ProbeFactory.BuildDependentObjectProbe(msi.ConnectionInfo, ObjectName, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }


        public List<Result> GetPrescriptiveAnalysisDbSnapshots(int serverId, ISnapshotSink sink, object state, string db)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return new List<Result> { Result.Failure };
            List<IProbe> lstProbes = new List<IProbe>();
            LOG.Info("Prescriptive Analysis : Adding Database Probes");
            lstProbes.Add(ProbeFactory.BuildBackupAndRecoveryProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildDBSecurityProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildDisabledIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildFragmentedIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildHighIndexUpdatesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildIndexContentionProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildOutOfDateStatsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildOverlappingIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildSqlModuleOptionsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));

            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(lstProbes, sink, state);
            return ctxt.StartMultiple();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration -- Run Prescriptive Analysis for startup snapshots. Add all the required probes in a list and start execution by calling StartMultiple method
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public List<Result> RunStartupPrescriptiveAnalysis(int serverId, ISnapshotSink sink, object state, AnalysisConfiguration config)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return new List<Result> { Result.Failure };

            List<IProbe> lstProbes = new List<IProbe>();
            LOG.Info("Prescriptive Analysis : Adding Startup Probes");
            lstProbes.Add(ProbeFactory.BuildMasterFilesProbe(msi.ConnectionInfo, msi.CloudProviderId));//start
            lstProbes.Add(ProbeFactory.BuildServerConfigurationProbe(msi.ConnectionInfo, msi.CloudProviderId));//start
            lstProbes.Add(ProbeFactory.BuildWaitingBatchesProbe(msi.ConnectionInfo, msi.CloudProviderId));//start
            lstProbes.Add(ProbeFactory.BuildWaitStatisticsProbe(msi.ConnectionInfo, new WaitStatisticsConfiguration(serverId, null), msi.CloudProviderId));//start
            lstProbes.Add(ProbeFactory.BuildConfigurationProbe(msi.ConnectionInfo, msi.CloudProviderId));//start
            //DM 10.0.0.2 -- Vineet -- Fix for analysis issue. Skipping wmi probes if wmi is not enabled
            if (msi.WmiConfig != null && msi.WmiConfig.DirectWmiEnabled)
            {
                lstProbes.Add(ProbeFactory.BuildWmiTCPProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiTCPv4Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiVolumeProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiPerfOSMemoryProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiNetworkRedirectorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiTCPv6Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiPerfOSProcessorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiPerfDiskLogicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiPerfDiskPhysicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiPageFileProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildWmiNetworkInterfaceProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//start
                lstProbes.Add(ProbeFactory.BuildSampleServerResourcesProbe(msi.ConnectionInfo, msi.CloudProviderId));//start
            }
            else
            {
                LOG.Warn("Startup Probes : Skipping wmi probes because wmi is disabled from server properties.");
            }
            LOG.VerboseFormat("Prescriptive Analysis Startup list probes count:{0}", lstProbes.Count);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(lstProbes, sink, state);
            return ctxt.StartMultiple();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration -- Run Prescriptive Analysis for interval snapshots. Add all the required probes in a list and start execution by calling StartMultiple method
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public List<Result> RunIntervalPrescriptiveAnalysis(int serverId, ISnapshotSink sink, object state, AnalysisConfiguration config)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return new List<Result> { Result.Failure };

            List<IProbe> lstProbes = new List<IProbe>();
            LOG.Info("Prescriptive Analysis : Adding Interval Probes");
            //DM 10.0.0.2 -- Vineet -- Fix for analysis issue. Skipping wmi probes if wmi is not enabled
            if (msi.WmiConfig != null && msi.WmiConfig.DirectWmiEnabled)
            {
                lstProbes.Add(ProbeFactory.BuildWmiPerfOSSystemProbe(msi.ConnectionInfo, msi.WmiConfig, true, 10, 0, msi.CloudProviderId));//interval
                lstProbes.Add(ProbeFactory.BuildWmiProcessorProbe(msi.ConnectionInfo, msi.WmiConfig, true, 10, 0, msi.CloudProviderId));//interval
            }
            else
            {
                LOG.Warn("Interval Probes : Skipping wmi probes because wmi is disabled from server properties.");
            }
            lstProbes.Add(ProbeFactory.BuildWaitingBatchesProbe(msi.ConnectionInfo, true, 2, 0, msi.CloudProviderId));//interval
            lstProbes.Add(ProbeFactory.BuildSampleServerResourcesProbe(msi.ConnectionInfo, true, 30, 5, msi.CloudProviderId));//interval

            if (msi.MostRecentSQLVersion != null && msi.MostRecentSQLVersion.Major >= 12)
            {
                lstProbes.Add(ProbeFactory.BuildBufferPoolExtIOProbe(msi.ConnectionInfo, 5, 0, msi.CloudProviderId));
            }
            LOG.VerboseFormat("Prescriptive Analysis Interval list probes count:{0}", lstProbes.Count);

            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(lstProbes, sink, state);
            return ctxt.StartMultiple();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration -- Run Prescriptive Analysis for shutdown snapshots. Add all the required probes in a list and start execution by calling StartMultiple method
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public List<Result> RunShutdownPrescriptiveAnalysis(int serverId, ISnapshotSink sink, object state, AnalysisConfiguration config)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return new List<Result> { Result.Failure };

            List<IProbe> lstProbes = new List<IProbe>();
            LOG.Info("Prescriptive Analysis : Adding Shutdown Probes");
            lstProbes.Add(ProbeFactory.BuildSampleServerResourcesProbe(msi.ConnectionInfo, msi.CloudProviderId));//shut
            lstProbes.Add(ProbeFactory.BuildWorstFillFactorIndexesProbe(msi.ConnectionInfo, msi.CloudProviderId));//shut
            lstProbes.Add(ProbeFactory.BuildNUMANodeCountersProbe(msi.ConnectionInfo, msi.CloudProviderId));//shut

            //DM 10.0.0.2 -- Vineet -- Fix for analysis issue. Skipping wmi probes if wmi is not enabled
            if (msi.WmiConfig != null && msi.WmiConfig.DirectWmiEnabled)
            {
                lstProbes.Add(ProbeFactory.BuildWmiTCPProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiTCPv4Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPerfOSMemoryProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiNetworkRedirectorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiTCPv6Probe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPerfOSProcessorProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPerfDiskLogicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPerfDiskPhysicalDiskProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPageFileProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiNetworkInterfaceProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiProcessProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiEncryptableVolumeProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiComputerSystemProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiPhysicalMemoryProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
                lstProbes.Add(ProbeFactory.BuildWmiServiceProbe(msi.ConnectionInfo, msi.WmiConfig, msi.CloudProviderId));//shut
            }
            else
            {
                LOG.Warn("Shutdown Probes : Skipping wmi probes because wmi is disabled from server properties.");
            }
            LOG.VerboseFormat("Prescriptive Analysis Shutdown list probes count:{0}", lstProbes.Count);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(lstProbes, sink, state);
            return ctxt.StartMultiple();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration -- Run Prescriptive Analysis for database snapshots. Add all the required probes in a list and start execution by calling StartMultiple method
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public List<Result> RunDatabasePrescriptiveAnalysis(int serverId, ISnapshotSink sink, object state, AnalysisConfiguration config)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return new List<Result> { Result.Failure };

            List<string> dbs = GetDatabasesToInclude(msi.ConnectionInfo, sink, state, config, msi.CloudProviderId);

            List<IProbe> lstProbes = new List<IProbe>();

            lstProbes.Add(ProbeFactory.BuildDatabaseRankingProbe(msi.ConnectionInfo, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildAdhocCachedPlanBytesProbe(msi.ConnectionInfo, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildLockedPageKBProbe(msi.ConnectionInfo, msi.CloudProviderId));
            lstProbes.Add(ProbeFactory.BuildQueryPlanEstRowsProbe(msi.ConnectionInfo, msi.CloudProviderId));
            DatabaseProbeConfiguration dbConfig = new DatabaseProbeConfiguration(serverId);
            dbConfig.IncludeSystemDatabases = true;
            lstProbes.Add(ProbeFactory.BuildDatabaseConfigurationProbe(msi.ConnectionInfo, dbConfig, msi.CloudProviderId));
            foreach (string db in dbs)
            {
                lstProbes.Add(ProbeFactory.BuildBackupAndRecoveryProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildDBSecurityProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildDisabledIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildFragmentedIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildHighIndexUpdatesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildIndexContentionProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildOutOfDateStatsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildOverlappingIndexesProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildSqlModuleOptionsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildNonIncrementalColStatsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildHashIndexProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildQueryStoreProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildRarelyUsedIndexOnInMemoryTableProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildQueryAnalyzerProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildColumnStoreIndexProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildFilteredColumnNotInKeyOfFilteredIndexProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildHighCPUTimeProcedureProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
                lstProbes.Add(ProbeFactory.BuildLargeTableStatsProbe(msi.ConnectionInfo, db, msi.CloudProviderId));
            }
            LOG.VerboseFormat("Prescriptive Analysis Database list probes count:{0}", lstProbes.Count);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(lstProbes, sink, state);
            return ctxt.StartMultiple();
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration -- Run Prescriptive Analysis. Add all the required probes in a list and start execution by calling StartMultiple method
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public List<Result> RunPrescriptiveAnalysis(int serverId, ISnapshotSink sink, object state, AnalysisConfiguration config, AnalysisCollectorType analysisCollectorType)
        {
            switch (analysisCollectorType)
            {
                case AnalysisCollectorType.DatabaseSpecific:
                    return RunDatabasePrescriptiveAnalysis(serverId, sink, state, config);
                case AnalysisCollectorType.Interval:
                    return RunIntervalPrescriptiveAnalysis(serverId, sink, state, config);
                case AnalysisCollectorType.Shutdown:
                    return RunShutdownPrescriptiveAnalysis(serverId, sink, state, config);
                case AnalysisCollectorType.Startup:
                    return RunStartupPrescriptiveAnalysis(serverId, sink, state, config);
            }
            return new List<Result> { Result.Failure };
        }

        /// <summary>
        /// SQLdm 10.0 Vineet -- It gives the databases to be included in database snapshots for analysis based on the configuration passed
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <param name="config"></param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public List<string> GetDatabasesToInclude(SqlConnectionInfo con, ISnapshotSink sink, object state, AnalysisConfiguration config, int? cloudProviderId)
        {
            //Start : Get list of databases
            Idera.SQLdm.CollectionService.OnDemandClient.OnDemandCollectionContext<DatabaseNamesSnapshot> clientCtxt =
       new Idera.SQLdm.CollectionService.OnDemandClient.OnDemandCollectionContext<DatabaseNamesSnapshot>();
            IProbe probe1 = ProbeFactory.BuildDatabaseNamesProbe(con, cloudProviderId);
            OnDemandCollectionContext ctxt1 = new OnDemandCollectionContext(probe1, clientCtxt, state);
            ctxt1.Start();
            var snapshot = clientCtxt.Wait();
            var dbsnapshot = (DatabaseNamesSnapshot)snapshot;
            //End : Get list of databases
            List<string> dbs = new List<string>();
            if (dbsnapshot == null || dbsnapshot.Error != null)
            {
                return dbs;
            }
            foreach (string db in dbsnapshot.Databases.Values)
            {
                if (config != null)
                {
                    if (!string.IsNullOrEmpty(config.IncludeDatabaseName))
                    {
                        if (db == config.IncludeDatabaseName) { dbs.Add(db); break; }
                    }
                    else
                    {
                        if (config.BlockedDatabases != null)
                        {
                            //if (!config.BlockedDatabases.ContainsValue(db)) dbs.Add(db);
                            bool contains = false;
                            foreach (string strDB in config.BlockedDatabases.Values)
                            {
                                if (strDB.Trim().ToLower() == db.Trim().ToLower())
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (!contains)
                            { dbs.Add(db); }

                        }
                        else
                        {
                            dbs.Add(db);
                        }
                    }
                }
                else
                { dbs.Add(db); }
            }
            return dbs;
        }

        /// <summary>
        /// SQLdm 10.0 Vineet -- Gets the optimize script for a recommendation
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public string GetPrescriptiveOptimizeScript(int serverId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return string.Empty;
            return Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.GetPrescriptiveOptimizeScript(msi.ConnectionInfo, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Vineet -- Gets the undo script for a recommendation
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public List<string> GetPrescriptiveOptimizeMessages(int serverId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return null;
            return Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.GetPrescriptiveOptimizeMessages(msi.ConnectionInfo, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Srishti -- Gets the undo message for a recommendation
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public List<string> GetPrescriptiveUndoMessages(int serverId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return null;
            return Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.GetPrescriptiveUndoMessages(msi.ConnectionInfo, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Srishti -- Gets the optimize message for a recommendation
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public string GetPrescriptiveUndoScript(int serverId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return string.Empty;
            return Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.GetPrescriptiveUndoScript(msi.ConnectionInfo, recommendation);
        }

        /// <summary>
        /// GetMachineInfo
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public int? GetMachineInfo(int serverId)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return null;

            return msi.CloudProviderId;
        }

        /// <summary>
        /// SQLdm 10.0 Praveen -- Gets the connection string for monitored server. It is required in PredictiveAnalytics service to run workload analysis
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public string GetConnectionStringForServer(int serverId)
        {
            string connectionString = string.Empty;
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi != null)
            { connectionString = msi.ConnectionInfo.ConnectionString; }
            return connectionString;
        }

        /// <summary>
        /// SQLdm 10.0 Vineet -- Gets the Real time Databases for monitored server
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result GetDatabasesForServer(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;
            IProbe probe = ProbeFactory.BuildDatabaseNamesProbe(msi.ConnectionInfo, msi.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }

        /// <summary>
        /// SQLdm 10.0 Praveen -- Gets the machine for monitored server.
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="sink"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result GetMachineName(int serverId, ISnapshotSink sink, object state)
        {
            MonitoredSqlServer msi = GetMonitoredServerInfo(serverId);
            if (msi == null)
                return Result.Failure;
            MonitoredServerWorkload workload = Collection.Scheduled.GetWorkload(serverId);
            IProbe probe = ProbeFactory.BuildMachineNameProbe(msi.ConnectionInfo, workload.MonitoredServer.CloudProviderId);
            OnDemandCollectionContext ctxt = new OnDemandCollectionContext(probe, sink, state);
            return ctxt.Start();
        }
    }
}
