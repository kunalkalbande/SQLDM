//------------------------------------------------------------------------------
// <copyright file="CollectionService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   04-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Snapshots;
using Microsoft.ApplicationBlocks.Data;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.CollectionService
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Management;
    using Idera.SQLdm.CollectionService.Configuration;
    using Idera.SQLdm.CollectionService.Helpers;
    using Idera.SQLdm.CollectionService.Monitoring;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Thresholds;
    using Microsoft.SqlServer.Management.Smo;
    using System.Collections.Generic;

    /// <summary>
    /// Class that implements the collection service's hosted remotable object.
    /// </summary>
    class CollectionService : MarshalByRefObject, ICollectionService
    {
        #region fields

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CollectionService");

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        #region ICollectionService Members

        /// <summary>
        /// Starts the monitoring server.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server workload.</param>
        /// <returns></returns>
        public Result StartMonitoringServer(MonitoredServerWorkload monitoredServerWorkload)
        {
            using (LOG.InfoCall("StartMonitoringServer"))
            {
                try
                {
                    if (monitoredServerWorkload == null)
                        throw new ArgumentNullException("monitoredServer");
                    if (monitoredServerWorkload.MonitoredServer.ConnectionInfo == null)
                        throw new ArgumentException("Monitored server has no connection info");

                    // Add the server to the on-demand context
                    Collection.OnDemand.AddMonitoredServer(monitoredServerWorkload.MonitoredServer);

                    // start scheduled monitoring 
                    Collection.Scheduled.StartMonitoring(monitoredServerWorkload);

                    return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                    return Result.Failure;
                }
            }
        }
        //sqldm-30244 start
        public DateTime GetDateTime(DateTime dt)
        {
            return dt.ToLocalTime();
        }
        //sqldm-30244 end
        /// <summary>
        /// Stops the monitoring server.
        /// </summary>
        /// <param name="monitoredSqlServerId">The id monitored server.</param>
        /// <returns></returns>
        public Result StopMonitoringServer(int monitoredSqlServerId)
        {
            using (LOG.InfoCall("StopMonitoringServer"))
            {
                try
                {
                    // remove scheduled collection context
                    Collection.Scheduled.StopMonitoring(monitoredSqlServerId);
                    // remove from on-demand context
                    Collection.OnDemand.RemoveMonitoredServer(monitoredSqlServerId);
                    return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                    return Result.Failure;
                }
            }
        }

        /// <summary>
        /// Reconfigures the monitored server.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server workload.</param>
        /// <returns></returns>
        public Result ReconfigureMonitoredServer(MonitoredServerWorkload monitoredServerWorkload)
        {
            using (LOG.InfoCall("ReconfigureMonitoredServer"))
            {
                try
                {
                    if (monitoredServerWorkload == null)
                        throw new ArgumentNullException("workload");
                    if (monitoredServerWorkload.MonitoredServer.ConnectionInfo == null)
                        throw new ArgumentException("Monitored server has no connection info");

                    // update the scheduled collection context
                    Collection.Scheduled.ReconfigureMonitoring(monitoredServerWorkload);

                    // update the on-demand context
                    Collection.OnDemand.ReplaceMonitoredServer(monitoredServerWorkload.MonitoredServer);

                    return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                    return Result.Failure;
                }
            }
        }

        public Result UpdateThresholdEntries(int monitoredSqlServerId, IEnumerable<MetricThresholdEntry> metricThresholdEntries)
        {
            using (LOG.InfoCall("UpdateThresholdEntries"))
            {
                try
                {
                    // update the scheduled collection context
                    if (Collection.Scheduled.UpdateThresholdEntries(monitoredSqlServerId, metricThresholdEntries))
                        return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                }
                return Result.Failure;
            }
        }

        public Result UpdateThresholdEntries(IEnumerable<MetricThresholdEntry> metricThresholdEntries)
        {
            using (LOG.InfoCall("UpdateThresholdEntries"))
            {
                try
                {
                    MetricThresholdEntry[] entryArray = new MetricThresholdEntry[1];

                    foreach (MetricThresholdEntry entry in metricThresholdEntries)
                    {
                        entryArray[0] = entry;
                        Collection.Scheduled.UpdateThresholdEntries(entry.MonitoredServerID, entryArray);
                    }

                    return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                }
                return Result.Failure;
            }
        }

        #region IOnDemandServer Members
        ///Ankit Nagpal -- Sqldm10.0
        /// <summary>
        /// Checks if registered user of monitored server has sysadmin rights or not
        /// </summary>
        /// <param name="monitoredSqlServer">monitoredSqlServer</param>
        /// <returns>boolean</returns>
        public TestSqlConnectionResult IsSysAdmin(SqlConnectionInfo sqlConnectionInfo)
        {
            using (LOG.InfoCall("IsSysAdmin"))
            {
                try
                {
                    using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
                    {
                        LOG.DebugFormat("Connection to get permissions started for {0}.", sqlConnectionInfo.InstanceName);
                        connection.Open();

                        bool isAdmin = Convert.ToBoolean(SqlHelper.ExecuteScalar(connection, CommandType.Text,
                                                                             "select is_srvrolemember('sysadmin')"));
                        return new TestSqlConnectionResult(sqlConnectionInfo, connection.ServerVersion, isAdmin);
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error(exception + " Instance Name : " + sqlConnectionInfo.InstanceName);
                    //Make sysadmin permissions true for user whose connection can't be established
                    return new TestSqlConnectionResult(sqlConnectionInfo, exception);
                }
            }
        }
        /// <summary>
        /// SQLdm 10.0.2 (Barkha Khatri)
        /// gets the product vesion of an SQL Server given the  monitoredSqlServer id
        /// </summary>
        /// <param name="monitoredSqlServer"></param>
        /// <returns>Server version</returns>
        public ServerVersion GetProductVersion(int monitoredSqlServer)
        {
            using (LOG.InfoCall("GetProductVersion"))
            {
                string GetVersion = " select serverproperty('productversion') ";
                try
                {
                    SqlConnectionInfo connInfo = Collection.OnDemand.GetSqlServerConnectionDetails(monitoredSqlServer);
                    if (connInfo.ConnectionString == null)
                    {
                        throw new ArgumentNullException("connectionString");
                    }
                    using (SqlConnection connection = new SqlConnection(connInfo.ConnectionString))
                    {

                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = GetVersion;
                            object productVersion = command.ExecuteScalar();
                            if (productVersion != null)
                            {
                                LOG.InfoFormat("GetProductVersion -- This is the server version {0}:", productVersion.ToString());
                                return new ServerVersion((string)productVersion);
                            }
                            else
                            {
                                LOG.Error(String.Format("GetProductVersion--the query returns null for the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty));
                                return null;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    LOG.Error(String.Format("GetProductVersion--Error while accessing the SQL Server \n(" + e + " Instance Id: " + monitoredSqlServer + ")"));
                    return null;
                }

            }
        }
        /// <summary>
        /// Starts data collection and returns snapshot.  This differs from other IOnDemandServer members.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <returns></returns>
        public Serialized<ActiveWaitsSnapshot> GetActiveWaits(ActiveWaitsConfiguration configuration)
        {
            using (LOG.InfoCall("GetActiveWaits"))
            {
                try
                {
                    configuration.StartTimeUTC = DateTime.Now.ToUniversalTime();
                    return Collection.OnDemand.CollectActiveWaits(configuration);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void StopWaitingForActiveWaits(ActiveWaitsConfiguration configuration)
        {
            using (LOG.InfoCall("UnregisterSingleActiveWaitsWaiter"))
            {
                try
                {
                    if (configuration == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "configuration");

                    Collection.Scheduled.StopWaitingForActiveWaits(configuration);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void StopActiveWaitCollector(int MonitoredServerId)
        {
            using (LOG.InfoCall("StopActiveWaitCollector"))
            {
                try
                {

                    Collection.Scheduled.StopActiveWaitCollector(MonitoredServerId);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        // SQLdm Minimum Privileges - Varun Chopra - Allow permissions collection when refresh button clicked on server properties
        public Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions> GetActiveWaitCollectorStatus(int MonitoredServerId)
        {
            using (LOG.InfoCall("GetActiveWaitStatus"))
            {
                try
                {
                    return Collection.Scheduled.GetActiveWaitCollectorStatus(MonitoredServerId);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }


        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetAgentJobHistory(AgentJobHistoryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetAgentJobHistory"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectAgentJobHistory(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        // Modification Start ID: M1
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        //public void GetAzureSQLMetric(AzureSQLMetricConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    using (LOG.InfoCall("GetAzureSQLMetric"))
        //    {
        //        try
        //        {
        //            if (sink == null)
        //                throw new ServiceException(Status.ErrorArgumentRequired, "sink");

        //            Collection.OnDemand.CollectAzureSQLMetric(configuration, sink, state);
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}
        // Modification End ID: M1


        //public void GetAmazonRDSMetric(AmazonRDSMetricConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    using (LOG.InfoCall("GetAmazonRDSMetric"))
        //    {
        //        try
        //        {
        //            if (sink == null)
        //                throw new ServiceException(Status.ErrorArgumentRequired, "sink");

        //            Collection.OnDemand.CollectAmazonRDSMetric(configuration, sink, state);
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetAgentJobSummary(AgentJobSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetAgentJobSummary"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectAgentJobSummary(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetBackupRestoreHistory(BackupRestoreHistoryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetBackupRestoreHistory"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectBackupRestoreHistory(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetConfiguration(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetConfiguration"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectConfiguration(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetCustomCounter(CustomCounterConfiguration configuration, ISnapshotSink sink, object state)
        {
            List<CustomCounterConfiguration> collection = new List<CustomCounterConfiguration>();
            collection.Add(configuration);
            GetCustomCounter(collection, sink, state);
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetCustomCounter(List<CustomCounterConfiguration> configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetCustomCounter"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectCustomCounter(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDatabaseConfiguration(DatabaseProbeConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabaseConfiguration"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDatabaseConfiguration(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDatabaseFiles(DatabaseFilesConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabaseFiles"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDatabaseFiles(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDatabaseSummary(DatabaseSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabaseSummary"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDatabaseSummary(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDatabaseAlwaysOnStatistics(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabaseAlwaysOnStatistics"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDatabaseAlwaysOnStatistics(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDatabaseAlwaysOnTopology(AlwaysOnAvailabilityGroupsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabaseAlwaysOnTopology"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDatabaseAlwaysOnTopology(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDistributorQueue(DistributorQueueConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDistributorQueue"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDistributorQueue(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetDistributorDetails(DistributorDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDistributorDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDistributorDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetErrorLog(ErrorLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetErrorLog"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectErrorLog(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetFullTextCatalogs(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetFullTextCatalogs"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectFullTextCatalogs(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetFullTextColumns(FullTextColumnsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetFullTextColumns"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectFullTextColumns(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetFullTextTables(FullTextTablesConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetFullTextTables"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectFullTextTables(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetIndexStatistics(IndexStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetIndexStatistics"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectIndexStatistics(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetLockDetails(LockDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetLockDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectLockDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetLogList(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetLogList"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectLogList(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetMirrorMonitoringRealtime(MirrorMonitoringRealtimeConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetMirrorMonitoringRealtime"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectMirrorMonitoringRealtime(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetMirrorMonitoringHistory(MirrorMonitoringHistoryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetMirrorMonitoringHistory"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectMirrorMonitoringHistory(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetProcedureCache(ProcedureCacheConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetProcedureCache"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectProcedureCache(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetJobsAndSteps(JobsAndStepsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetJobsAndSteps"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectJobsAndSteps(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        ///// <summary>
        ///// Starts data collection and returns.  The snapshot will be returned through the sink.
        ///// Any exceptions during this process will be percolated to the caller.
        ///// </summary>
        ///// <param name="configuration">configuration</param>
        ///// <param name="sink">The sink</param>
        ///// <param name="state"></param>
        ///// <returns></returns>
        //public void GetPublisherQueue(PublisherQueueConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    using (LOG.InfoCall("GetPublisherQueue"))
        //    {
        //        try
        //        {
        //            if (sink == null)
        //                throw new ServiceException(Status.ErrorArgumentRequired, "sink");

        //            Collection.OnDemand.CollectPublisherQueue(configuration, sink, state);
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetPublisherDetails(PublisherDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetPublisherDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectPublisherDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetSubscriberDetails(SubscriberDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetSubscriberDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectSubscriberDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        ///// <summary>
        ///// Starts data collection and returns.  The snapshot will be returned through the sink.
        ///// Any exceptions during this process will be percolated to the caller.
        ///// </summary>
        ///// <param name="configuration">configuration</param>
        ///// <param name="sink">The sink</param>
        ///// <param name="state"></param>
        ///// <returns></returns>
        //public void GetResource(ResourceConfiguration configuration, ISnapshotSink sink, object state)
        //{
        //    using (LOG.InfoCall("GetResource"))
        //    {
        //        try
        //        {
        //            if (sink == null)
        //                throw new ServiceException(Status.ErrorArgumentRequired, "sink");

        //            Collection.OnDemand.CollectResource(configuration, sink, state);
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetServerOverview(ServerOverviewConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetServerOverview"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectServerOverview(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        // SQLdm Minimum Privileges - Varun Chopra - Read permissions collections on demand
        public Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> GetServerPermissions(
            int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetServerPermissions(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve server time and version.", e);
            }
        }

        public Triple<ServerVersion, System.DateTime, System.DateTime> GetServerTimeAndVersion(int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetServerTimeAndVersion(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve server time and version.", e);
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetServices(OnDemandConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetServices"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectServices(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetSessions(SessionsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetSessions"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectSessions(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetSessionDetails(SessionDetailsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetSessionDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectSessionDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetSessionSummary(SessionSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetSessionSummary"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectSessionSummary(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetTableDetails(TableDetailConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetTableDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectTableDetails(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetTableSummary(TableSummaryConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetTableSummary"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectTableSummary(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetWaitStatistics(WaitStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetWaitStatistics"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectWaitStatistics(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.4 (Nikhil Bansal) - Get the OnDemand Estimated Query Plan 
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetEstimatedQueryPlan(EstimatedQueryPlanConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetEstimatedQueryPlan"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectEstimatedQueryPlan(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }

        }

        #endregion

        #region Server Actions


        /// <summary>
        /// Sends server action probe and executes code path based on configuration object
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendServerAction<T>(T configuration, ISnapshotSink sink, object state) where T : OnDemandConfiguration, IServerActionConfiguration
        {
            using (LOG.InfoCall("ServerAction"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendServerActionProbe(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendFreeProcedureCache(FreeProcedureCacheConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("FreeProcedureCache"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendFreeProcedureCache(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetFileActivity(FileActivityConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetFileActivity"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectFileActivity(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void GetFullTextAction(FullTextActionConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetFullTextAction"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendFullTextAction(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendJobControlSession(JobControlConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("JobControl"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendJobControlProbe(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendKillSession(KillSessionConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("KillSession"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendKillSessionProbe(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendReconfiguration(ReconfigurationConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendReconfiguration"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendReconfigurationProbe(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        public void SendBlockedProcessThresholdChange(ReconfigurationConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendBlockedProcessThresholdChange"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendBlockedProcessThresholdChangeProbe(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendShutdownSQLServer(ShutdownSQLServerConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("ShutdownSQLServer"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendShutdownSQLServer(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendSetNumberOfLogs(SetNumberOfLogsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendSetNumberOfLogs"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendSetNumberOfLogs(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendStopSessionDetailsTrace(StopSessionDetailsTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendStopSessionDetailsTrace"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendStopSessionDetailsTrace(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendStopQueryMonitorTrace(StopQueryMonitorTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendStopQueryMonitorTrace"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendStopQueryMonitorTrace(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendStopActivityMonitorTrace(StopActivityMonitorTraceConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendStopActivityMonitorTrace"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendStopActivityMonitorTrace(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendServiceControl(ServiceControlConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetServiceControl"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendServiceControl(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendRecycleLog(RecycleLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendRecycleLog"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendRecycleLog(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendRecycleAgentLog(RecycleAgentLogConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendRecycleAgentLog"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendRecycleAgentLog(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendReindex(ReindexConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendReindex"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendReindex(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Starts data collection and returns.  The snapshot will be returned through the sink.
        /// Any exceptions during this process will be percolated to the caller.
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="sink">The sink</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public void SendUpdateStatistics(UpdateStatisticsConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendUpdateStatistics"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendUpdateStatistics(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void SendWmiConfigurationTest(TestWmiConfiguration configuration, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("SendWmiConfigurationTest"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.SendWmiConfigurationTest(configuration, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }


        public void ForceScheduledCollection(int monitoredSqlServerId, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("ForceScheduledCollection"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.Scheduled.CollectNow(monitoredSqlServerId, sink);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public CollectionServiceConfigurationMessage GetCollectionServiceConfiguration()
        {
            return new ConfigurationService().GetCollectionServiceConfiguration();
        }

        public Result SetCollectionServiceConfiguration(CollectionServiceConfigurationMessage message)
        {
            return new ConfigurationService().SetCollectionServiceConfiguration(message);
        }

        //        public System.Collections.Generic.IDictionary<SqlConnectionInfo, TestSqlConnectionResult> TestServerConnections(System.Collections.Generic.IEnumerable<SqlConnectionInfo> connections)
        //        {
        //            throw new Exception("The method or operation is not implemented.");
        //        }
        //
        //        public System.Collections.Generic.IDictionary<SqlConnectionInfo, TestSqlConnectionResult> TestServerConnections(Guid collectionServiceID, System.Collections.Generic.IEnumerable<SqlConnectionInfo> connections)
        //        {
        //            throw new Exception("The method or operation is not implemented.");
        //        }

        public DataTable GetAvailableSqlServerInstances()
        {
            try
            {
                return SmoApplication.EnumAvailableSqlServers();
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve available SQL Servers instances.", e);
            }
        }

        public TestSqlConnectionResult TestSqlConnection(SqlConnectionInfo connectionInfo)
        {
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.CollectionServceConnectionStringApplicationName))
                {
                    LOG.DebugFormat("Connection test started for {0}.", connectionInfo.InstanceName);

                    connection.Open();

                    ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);

                    if (!(serverVersion.IsSupported))
                    {
                        return
                        new TestSqlConnectionResult(connectionInfo,
                               new Exception(
                               "The monitored SQL Server instance is running an unsupported SQL Server version.  The reported version is " +
                                serverVersion.Version));
                    }

                    //int? masterCompatibility = null;

                    //using (SqlCommand masterCmd = connection.CreateCommand())
                    //{
                    //    masterCmd.CommandText =
                    //        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.
                    //            GetMasterCompatibility;
                    //    masterCompatibility = Int32.Parse((string) masterCmd.ExecuteScalar());
                    //}

                    //if (serverVersion.Major > 8 && masterCompatibility.HasValue && masterCompatibility.Value < 90)
                    //{
                    //    return
                    //        new TestSqlConnectionResult(connectionInfo,
                    //        new Exception(
                    //            "The monitored SQL Server instance is running an unsupported SQL Server compatibility level.  The reported compatibility of the master database is " +
                    //            masterCompatibility +
                    //            " on a server with a reported version of " +
                    //            serverVersion.Version +
                    //            ".  For SQL 2005 and later SQLdm requires the master database compatibility level to be at least 90 in order"
                    //            + " to use certain table-valued functions."));
                    //}

                    string productEdition = null;

                    using (SqlCommand editionCmd = connection.CreateCommand())
                    {
                        editionCmd.CommandText =
                            Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetEdition;
                        productEdition = (string)editionCmd.ExecuteScalar();
                    }

                    //SQLdm 8.6 -- (Ankit Srivastava) -- Commented the code which stopped SQL Express monitoring
                    //if (productEdition.ToLower().Trim() == "express edition" )
                    //{
                    //    return
                    //        new TestSqlConnectionResult(connectionInfo,
                    //        new Exception(
                    //            "The monitored SQL Server instance is running an unsupported SQL Server edition.  The reported version is " +
                    //                                          serverVersion.Version + " " + productEdition));
                    //}

                    ///Ankit Nagpal -Sqldm10.0
                    ///Making changes for cloud server support , removing isAdmin check
                    bool isAdmin = true;//Convert.ToBoolean(SqlHelper.ExecuteScalar(connection, CommandType.Text,
                                        //"select is_srvrolemember('sysadmin')"));

                    LOG.DebugFormat("Connection test completed for {0}.", connectionInfo.InstanceName);

                    if (isAdmin)
                    {
                        return new TestSqlConnectionResult(connectionInfo, connection.ServerVersion);
                    }
                    else
                    {
                        return
                            new TestSqlConnectionResult(connectionInfo,
                                                        new Exception(
                                                            "The connection credentials do not have system adminstrator privileges on the monitored SQL Server instance."));
                    }
                }
            }
            catch (Exception e)
            {
                return new TestSqlConnectionResult(connectionInfo, e);
            }
        }

        public System.Collections.Generic.IEnumerable<string> GetAgentJobNames(int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetAgentJobNames(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve available Agent Job Names.", e);
            }
        }

        public System.Collections.Generic.IEnumerable<string> GetAgentJobCategories(int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetAgentJobCategories(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve available Agent Job Categories.", e);
            }
        }

        public System.Collections.Generic.IEnumerable<CategoryJobStep> GetAgentJobStepList(int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetAgentJobStepList(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unabled to retrieve Agent Job Step Information");
            }
        }

        public string GetCurrentClusterNode(int monitoredSqlServerId)
        {
            try
            {
                return Collection.OnDemand.GetCurrentClusterNode(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current cluster node.", e);
            }
        }
		

		//SQLDM-30197
		public string GetPreferredClusterNode(int monitoredSqlServerId)
		{
			try
            {
                return Collection.OnDemand.GetPreferredClusterNode(monitoredSqlServerId);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current cluster node.", e);
            }
		}

        public List<string> GetDisks(int instanceID)
        {
            try
            {
                return Collection.OnDemand.GetDisks(instanceID);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current disk list", e);
            }
        }

        public IDictionary<string, bool> GetDatabases(int monitoredSqlServerId, bool includeSystemDatabases, bool includeUserDatabases)
        {
            try
            {
                return Collection.OnDemand.GetDatabases(monitoredSqlServerId, includeSystemDatabases, includeUserDatabases);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current database list.", e);
            }
        }

        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --Get all the filegroups for a SQL server
        public IList<string> GetFilegroups(int monitoredSqlServerId, string databaseName, bool isDefaultThreshold)
        {
            try
            {
                return Collection.OnDemand.GetFilegroupsOfInstance(monitoredSqlServerId, databaseName, isDefaultThreshold);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current filegroups list.", e);
            }
        }

        public List<Triple<string, string, bool>> GetTables(int monitoredSqlServerId, string database, bool includeSystemTables, bool includeUserTables)
        {
            try
            {
                return Collection.OnDemand.GetTables(monitoredSqlServerId, database, includeSystemTables, includeUserTables);
            }
            catch (Exception e)
            {
                throw new CollectionServiceException("Unable to retrieve current table list.", e);
            }
        }

        public Result PauseService()
        {
            Collection.Pause();
            return Result.Success;
        }

        public Result ResumeService()
        {
            Collection.IsPaused = false;
            return Result.Success;
        }

        public CollectionServiceStatus GetServiceStatus()
        {
            return new ConfigurationService().GetServiceStatus();
        }

        public Result ReinitializeService()
        {
            // cause collection to cease
            Collection.Pause();

            Uri uri = CollectionServiceConfiguration.ManagementServiceUri;
            CollectionServiceConfiguration.FireOnManagementServiceChanged(uri, uri);

            return Result.Success;
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetSysPerfInfoObjectList(int monitoredSqlServerId)
        {
            using (LOG.InfoCall("GetSysPerfInfoObjectList"))
            {
                try
                {
                    DataTable objects = Collection.OnDemand.GetSysPerfInfoObjectList(monitoredSqlServerId);
                    return new Serialized<DataTable>(objects);
                }
                catch (Exception e)
                {
                    throw new CollectionServiceException("Unable to retrieve list of objects from sysperfinfo.", e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetSysPerfInfoCounterList(int monitoredSqlServerId, string objectName)
        {
            using (LOG.InfoCall("GetSysPerfInfoCounterList"))
            {
                try
                {
                    DataTable dataTable =
                        Collection.OnDemand.GetSysPerfInfoCounterList(monitoredSqlServerId, objectName);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable);
                }
                catch (Exception e)
                {
                    throw new CollectionServiceException(
                        String.Format("Unable to retrieve list of counters from sysperfinfo for object {0}.", objectName),
                        e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetSysPerfInfoInstanceList(int monitoredSqlServerId, string objectName)
        {
            using (LOG.InfoCall("GetSysPerfInfoInstanceList"))
            {
                try
                {
                    DataTable dataTable =
                        Collection.OnDemand.GetSysPerfInfoInstanceList(monitoredSqlServerId, objectName);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable);
                }
                catch (Exception e)
                {
                    throw new CollectionServiceException(
                        String.Format("Unable to retrieve list of instance from sysperfinfo for object {0}.",
                                      objectName), e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetVmCounterObjectList(int monitoredSqlServerId)
        {
            using (LOG.InfoCall("GetVmCounterObjectList"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetVmCounterObjectList(monitoredSqlServerId);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable);
                }
                catch (Exception e)
                {
                    throw new CollectionServiceException("Unable to retrieve list of objects from Virtualization Host.", e);
                }
            }
        }

        public Serialized<DataTable> GetAzureMonitorNamespaces(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorNamespaces"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetAzureMonitorNamespaces(monitorConfiguration);

                    dataTable.RemotingFormat = SerializationFormat.Binary;

                    return dataTable;
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new CollectionServiceException(
                        string.Format(
                            "Unable to retrieve list of Azure Monitor namespaces for instance {0}{1} ({2}) using {3}",
                            instanceId, monitorConfiguration.Profile.ApplicationProfile.Name,
                            monitorConfiguration.MonitorParameters.Resource.Type,
                            monitorConfiguration.MonitorParameters.Resource.Uri),
                        e);
                }
            }
        }
        public Serialized<DataTable> GetAzureMonitorDefinitions(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorDefinitions"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetAzureMonitorDefinitions(monitorConfiguration);

                    dataTable.RemotingFormat = SerializationFormat.Binary;

                    return dataTable;
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    LOG.Error("GetAzureMonitorDefinitions:" + e);
                    throw new CollectionServiceException(
                        string.Format(
                            "Unable to retrieve list of Azure Monitor Definitions for instance {0}{1} ({2}) using {3}",
                            instanceId, monitorConfiguration.Profile.ApplicationProfile.Name,
                            monitorConfiguration.MonitorParameters.Resource.Type,
                            monitorConfiguration.MonitorParameters.Resource.Uri),
                        new Exception(e.Message));
                }
            }
        }
        public Serialized<DataTable> GetAzureDatabase(int instanceId)
        {
            using (LOG.InfoCall("GetAzureDatabase"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetAzureDatabase(instanceId);
                    return dataTable;
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new CollectionServiceException("Unable to retrieve list of wmi classes from \\root\\cimv2.", e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<Pair<string, DataTable>> GetWmiObjectList(int instanceId, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiObjectList"))
            {
                try
                {
                    Pair<string, DataTable> result = new Pair<string, DataTable>();
                    DataTable dataTable = Collection.OnDemand.GetWmiObjectList(instanceId, out result.First, wmiConfiguration);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    result.Second = dataTable;

                    return new Serialized<Pair<string, DataTable>>(result, true);
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new CollectionServiceException("Unable to retrieve list of wmi classes from \\root\\cimv2.", e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetWmiCounterList(string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiCounterList"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetWmiCounterList(serverName, objectName, wmiConfiguration);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable, true);
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new CollectionServiceException(
                        String.Format("Unable to retrieve list of properties from \\root\\cimv2\\{0}.", objectName), e);
                }
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetWmiInstanceList(string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiInstanceList"))
            {

                try
                {
                    DataTable dataTable = Collection.OnDemand.GetWmiInstanceList(serverName, objectName, wmiConfiguration);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable, true);
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;

                    throw new CollectionServiceException(
                        String.Format("Unable to retrieve list of instances from \\root\\cimv2\\{0}.", objectName), e);
                }
            }
        }

        public object TestCustomCounter(int monitoredSqlServer, CustomCounterDefinition counterDefinition)
        {
            using (LOG.InfoCall("TestCustomCounter"))
            {
                return Collection.OnDemand.TestCustomCounter(monitoredSqlServer, counterDefinition);
            }
        }

        public void AddCustomCounter(CustomCounterDefinition counterDefinition)
        {
            using (LOG.InfoCall("AddCustomCounter"))
            {
                Collection.ReplaceCustomCounter(counterDefinition);
            }
        }

        public void UpdateCustomCounter(CustomCounterDefinition counterDefinition)
        {
            using (LOG.InfoCall("UpdateCustomCounter"))
            {
                Collection.ReplaceCustomCounter(counterDefinition);
            }
        }

        public void UpdateCustomCounterStatus(int metricID, bool enabled)
        {
            using (LOG.InfoCall("UpdateCustomCounterStatus"))
            {
                CustomCounterDefinition ccd = Collection.GetCustomCounter(metricID);
                if (ccd != null)
                {
                    ccd.IsEnabled = enabled;
                }
                else
                    LOG.Error("Unable to locate counter definition for metrid id={0}", metricID);
            }
        }

        public void DeleteCustomCounter(int metricID)
        {
            using (LOG.InfoCall("DeleteCustomCounter"))
            {
                Collection.Scheduled.RemoveCustomCounterFromAllServers(metricID);
                Collection.RemoveCustomCounter(metricID);
            }
        }

        public void AddCounterToServers(IEnumerable<MetricThresholdEntry> thresholds, bool syncWithList)
        {
            using (LOG.InfoCall("AddCounterToServers"))
            {
                MetricThresholdEntry[] thresholdArray = new MetricThresholdEntry[1];
                Dictionary<int, Set<int>> serverMap = new Dictionary<int, Set<int>>();
                Set<int> serverList;
                foreach (MetricThresholdEntry threshold in thresholds)
                {
                    // add the threshold entry
                    thresholdArray[0] = threshold;
                    Collection.Scheduled.UpdateThresholdEntries(threshold.MonitoredServerID, thresholdArray);
                    // add the custom counter to the configured list
                    Collection.Scheduled.AddCustomCounterToServer(threshold.MonitoredServerID, threshold.MetricID);
                    if (!serverMap.TryGetValue(threshold.MetricID, out serverList))
                    {
                        serverList = new Set<int>();
                        serverMap.Add(threshold.MetricID, serverList);
                    }
                    serverList.Add(threshold.MonitoredServerID);
                }
                if (syncWithList)
                {
                    Set<int> monitoredServers = Collection.Scheduled.GetServersBeingMonitored();
                    foreach (int metricID in serverMap.Keys)
                    {
                        Set<int> difference = monitoredServers.Difference(serverMap[metricID]);
                        foreach (int monitoredSqlServerId in difference)
                        {
                            Collection.Scheduled.RemoveCustomCounterFromServer(monitoredSqlServerId, metricID);
                        }
                    }
                }
            }
        }

        public void RemoveCounterFromServers(int metricId, int[] monitoredSqlServers)
        {
            using (LOG.InfoCall("RemoveCounterFromServers"))
            {

                foreach (int serverID in monitoredSqlServers)
                {
                    Collection.Scheduled.RemoveCustomCounterFromServer(serverID, metricId);
                }
            }
        }

        public void ClearEventState(int monitoredSqlServerId, int metricId, DateTime? cutoffTime, MonitoredObjectName objectName)
        {
            using (LOG.InfoCall("ClearEventState"))
            {
                Collection.Scheduled.ClearEventState(monitoredSqlServerId, metricId, cutoffTime, objectName);
            }
        }

        /// <summary>
        /// Remove tag (from custom counters and servers)
        /// </summary>
        public void RemoveTags(IEnumerable<int> tagIds)
        {
            using (LOG.InfoCall("RemoveTag"))
            {
                Collection.RemoveTags(tagIds);
                Collection.Scheduled.RemoveServerTags(tagIds);
            }
        }

        public void UpdateTagConfiguration(int tagId, IList<int> serverIds, IList<int> customCounterIds)
        {
            using (LOG.InfoCall("AddTagToServers"))
            {
                Collection.Scheduled.UpdateTagConfiguration(tagId, serverIds);
                Collection.UpdateTagConfiguration(tagId, customCounterIds);
            }
        }


        public void AddTagToServers(int tagId, IEnumerable<int> servers)
        {
            using (LOG.InfoCall("AddTagToServers"))
            {
                foreach (int monitoredSqlServerId in servers)
                {
                    Collection.Scheduled.AddServerTag(monitoredSqlServerId, tagId);
                }
            }
        }

        public void RemoveTagFromServers(int tagId, IEnumerable<int> servers)
        {
            using (LOG.InfoCall("RemoveTagFromServers"))
            {
                foreach (int monitoredSqlServerId in servers)
                {
                    Collection.Scheduled.RemoveServerTag(monitoredSqlServerId, tagId);
                }
            }
        }

        public void AddTagToCustomCounters(int tagId, IEnumerable<int> metrics)
        {
            using (LOG.InfoCall("AddTagToCustomCounters"))
            {
                Collection.AddTagToCustomCounters(tagId, metrics);
            }
        }

        public void RemoveTagFromCustomCounters(int tagId, IEnumerable<int> metrics)
        {
            using (LOG.InfoCall("RemoveTagFromCustomCounters"))
            {
                Collection.RemoveTagFromCustomCounters(tagId, metrics);
            }
        }

        public void SaveMirrorPreferredConfig(MirroringSession session)
        {
            using (LOG.InfoCall("SaveMirrorPreferredConfig"))
            {
                Collection.Scheduled.SaveMirroringPreferredConfig(session);
            }
        }

        public Idera.SQLdm.Common.Data.Serialized<DataTable> GetDriveConfiguration(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetDiskConfiguration"))
            {
                try
                {
                    DataTable dataTable = Collection.OnDemand.GetDriveConfiguration(connectionInfo, wmiConfiguration);
                    dataTable.RemotingFormat = SerializationFormat.Binary;
                    return new Serialized<DataTable>(dataTable, true);
                }
                catch (Exception e)
                {
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new CollectionServiceException(
                        String.Format("Unable to retrieve disk configuration from \\root\\cimv2 for instance {0}.", connectionInfo.InstanceName), e);
                }
            }
        }

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details
        public void GetDiskSizeDetails(DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetLockDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    Collection.OnDemand.CollectDiskSizeDetails(configuration, wmiConfig, sink, state);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details

        //10.0 doctor integration

        public void GetPrescriptiveAnalysisSnapshots(int monitoredSqlServerId, ISnapshotSink sink, object state, AnalysisConfiguration config, AnalysisCollectorType analysisCollectorType)
        {
            using (LOG.InfoCall("GetLockDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    List<Result> rslts = Collection.OnDemand.RunPrescriptiveAnalysis(monitoredSqlServerId, sink, state, config, analysisCollectorType);
                    // return rslts;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void GetPrescriptiveAnalysisDbSnapshots(int monitoredSqlServerId, ISnapshotSink sink, object state, string db)
        {
            using (LOG.InfoCall("GetLockDetails"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    List<Result> rslts = Collection.OnDemand.GetPrescriptiveAnalysisDbSnapshots(monitoredSqlServerId, sink, state, db);
                    // return rslts;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }


        /// <summary>
        /// SQLdm 10.0 Vineet- doctor integration
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        string IOnDemandServer.GetPrescriptiveOptimizeScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            return Collection.OnDemand.GetPrescriptiveOptimizeScript(monitoredSqlServerId, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Srishti- doctor integration --GetMessages
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        List<string> IOnDemandServer.GetPrescriptiveOptimizeMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            return Collection.OnDemand.GetPrescriptiveOptimizeMessages(monitoredSqlServerId, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Srishti- doctor integration --GetUndoMessages
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        List<string> IOnDemandServer.GetPrescriptiveUndoMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            return Collection.OnDemand.GetPrescriptiveUndoMessages(monitoredSqlServerId, recommendation);
        }
        /// <summary>
        /// SQLdm 10.0 Vineet- doctor integration
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        string IOnDemandServer.GetPrescriptiveUndoScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            return Collection.OnDemand.GetPrescriptiveUndoScript(monitoredSqlServerId, recommendation);
        }

        /// <summary>
        /// SQLdm 10.0 Vineet- doctor integration
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public Result ExecutePrescriptiveOptimization(int monitoredSqlServerId, ISnapshotSink sink, object state, PrescriptiveScriptConfiguration configuration)
        {
            return Collection.OnDemand.ExecutePrescriptiveOptimizationScript(monitoredSqlServerId, sink, state, configuration);
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit- doctor integration
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName"></param>
        /// <returns></returns>
        public Result GetTableofDependentObject(int monitoredSqlServerId, ISnapshotSink sink, object state, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName ObjectName)
        {
            return Collection.OnDemand.GetTableofDependentObject(monitoredSqlServerId, sink, state, ObjectName);
        }

        /// <summary>
        /// GetCloudProvider
        /// </summary>
        /// <param name="monitoredSqlServerId">Monitored SqlServer Id</param>
        int? IOnDemandServer.GetCloudProvider(int monitoredSqlServerId)
        {
            return Collection.OnDemand.GetMachineInfo(monitoredSqlServerId);
        }

        /// <summary>
        /// SQLdm 10.0 Praveen- doctor integration
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        string IOnDemandServer.GetConnectionStringForServer(int monitoredSqlServerId)
        {
            return Collection.OnDemand.GetConnectionStringForServer(monitoredSqlServerId);
        }

        public Result GetDatabasesForServer(int monitoredSqlServerId, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetDatabasesForServer"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    return Collection.OnDemand.GetDatabasesForServer(monitoredSqlServerId, sink, state);
                    // return rslts;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Result GetMachineName(int monitoredSqlServerId, ISnapshotSink sink, object state)
        {
            using (LOG.InfoCall("GetMachineName"))
            {
                try
                {
                    if (sink == null)
                        throw new ServiceException(Status.ErrorArgumentRequired, "sink");

                    return Collection.OnDemand.GetMachineName(monitoredSqlServerId, sink, state);
                    // return rslts;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

    }
}
