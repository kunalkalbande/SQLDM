//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Messages;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Bamboo.Prevalence;
    using Bamboo.Prevalence.Configuration;
    using Bamboo.Prevalence.Util;
    using Idera.SQLdm.CollectionService.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.CollectionService.Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Services;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Thresholds;
    using ManagementService;
    using Wintellect.PowerCollections;
    
    /// <summary>
    /// Handles all scheduled collection for this collection service.  Responsible for maintaining scheduling,
    /// and handling updates to collection configuration (thresholds, connection information, etc...).
    /// </summary>
    public partial class ScheduledCollectionManager
    {
        #region fields

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionManager");

        private IDictionary<int, ScheduledCollectionContext> scheduledCollections = new Dictionary<int, ScheduledCollectionContext>();
        private PersistenceManager persistenceManager = null;
        private ScheduledCollectionQueue scheduledCollectionQueue = new ScheduledCollectionQueue();

        private IDictionary<int, ContinuousCollectionContext<ActiveWaitsSnapshot>> waitStatisticsCollectors =
            new Dictionary<int, ContinuousCollectionContext<ActiveWaitsSnapshot>>();

        private IDictionary<int, ContinuousCollectionContext<TableGrowthSnapshot>> tableGrowthCollectors =
            new Dictionary<int, ContinuousCollectionContext<TableGrowthSnapshot>>();

        private IDictionary<int, ContinuousCollectionContext<TableFragmentationSnapshot>> tableFragmentationCollectors =
            new Dictionary<int, ContinuousCollectionContext<TableFragmentationSnapshot>>();

        ////SQLdm 10.0 (Sanjali Makkar) : Small Features : Declaring variable for storing the count of Monitored Servers
        //private const int MONITORED_SERVER_COUNTER_INCREMENT = 1;
        //private static IDictionary<int, bool> monitoredServerNames = new Dictionary<int, bool>();
        
        #endregion

        #region constructors

        internal ScheduledCollectionManager()
        {
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server.</param>
        public void StartMonitoring(MonitoredServerWorkload monitoredServerWorkload)
        {
            if (Collection.IsPaused)
            {
                string message = String.Format(
                    "Unable to start monitoring instance {0}.  The collection service is currently paused.",
                    monitoredServerWorkload.Name);
                LOG.Error(message);
                throw new CollectionServiceException(message);
            }

           int id = monitoredServerWorkload.Id;

           //START SQLdm 10.0 (Sanjali Makkar): Small Features : Updating '# Monitored Servers' and '# Servers in Maintainence Mode' perfCounter
           //if (id != null)
           //{
           //    bool inMaintenanceMode = false;
           //    bool newServer = false;

           //    lock (monitoredServerNames)
           //    {
           //        newServer = !monitoredServerNames.TryGetValue(id, out inMaintenanceMode);
           //    }

           //    if (newServer)
           //    {
           //        Statistics.SetMonitoredServers(MONITORED_SERVER_COUNTER_INCREMENT);
           //        lock (monitoredServerNames)
           //        {
           //            monitoredServerNames.Add(id, monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled);
           //        }
           //    }

           //    if (!monitoredServerNames.ContainsKey(id))
           //    {
           //        //increment the server counter for maintenance mode only when the server was not in maintenace mode earlier
           //        if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == true && inMaintenanceMode == false)
           //            Statistics.SetMaintainenceModeServers(MONITORED_SERVER_COUNTER_INCREMENT);
           //    }
           //}

           //END SQLdm 10.0 (Sanjali Makkar): Small Features : Updating '# Monitored Servers' and '# Servers in Maintainence Mode' perfCounter

            ScheduledCollectionContext newContext;
            ScheduledCollectionContext oldContext = null;

            lock (scheduledCollections)
            { 
                if (scheduledCollections.TryGetValue(monitoredServerWorkload.Id, out oldContext))
                {
                    monitoredServerWorkload.PreviousRefresh = oldContext.Workload.PreviousRefresh;
                    LOG.DebugFormat("Removing old scheduled collection context for instance id: {0}", monitoredServerWorkload.Id);
                    scheduledCollections.Remove(monitoredServerWorkload.Id);
                }

                newContext = new ScheduledCollectionContext(this, monitoredServerWorkload);
                LOG.DebugFormat("Adding new scheduled collection context for instance id: {0}", monitoredServerWorkload.Id);
                scheduledCollections[monitoredServerWorkload.Id] = newContext;
            }
           
            // kill the old collection context
            if (oldContext != null)
            {
                LOG.InfoFormat("Stopping scheduled collection for {0}: (config changing)", monitoredServerWorkload.Name);
                oldContext.Dispose();
            }

            // start the new collection context
            LOG.InfoFormat("Starting scheduled collection for {0}", monitoredServerWorkload.Name);
            newContext.Start(0);
        }

        /// <summary>
        /// Stops the monitoring.
        /// </summary>
        /// <param name="monitoredSqlServerId">The monitored server.</param>
        /// <returns></returns>
        public bool StopMonitoring(int monitoredSqlServerId)
        {
            lock (scheduledCollections)
            {
                ScheduledCollectionContext context;
                if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                {
                    using (context)
                    {
                        LOG.InfoFormat("Stopping scheduled collection for ({0})", context.Workload.Name);
                        Collection.Scheduled.StopActiveWaitCollector(context.Workload.Id);
                        scheduledCollections.Remove(monitoredSqlServerId);
                        
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Reconfigures the monitoring.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server.</param>
        /// <returns></returns>
        public void ReconfigureMonitoring(MonitoredServerWorkload monitoredServerWorkload)
        {
            ScheduledCollectionContext context = null;

            lock (scheduledCollections)
            {
                if (!scheduledCollections.TryGetValue(monitoredServerWorkload.Id, out context))
                {
                    StartMonitoring(monitoredServerWorkload);
                    return;
                }
            }

            LOG.InfoFormat("Reconfiguring scheduled collection for ({0})", monitoredServerWorkload.Name);
            context.Reconfigure(monitoredServerWorkload);
        }

        public bool UpdateThresholdEntries(int monitoredSqlServerId, IEnumerable<MetricThresholdEntry> thresholdEntries)
        {
            using (LOG.DebugCall("UpdateThresholdEntries"))
            {
                ScheduledCollectionContext context;
                if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                {
                    context.UpdateThresholdEntries(thresholdEntries);

                    return true;
                }
                LOG.DebugFormat("Context not found for instance id={0}", monitoredSqlServerId);
                return false;
            }
        }

        public void EnqueueScheduledRefresh(ScheduledCollectionDataMessage scheduledRefresh)
        {
            using (LOG.VerboseCall("EnqueueScheduledRefresh"))
            {
                scheduledCollectionQueue.Enqueue(scheduledRefresh);
            }
        }

        /// <summary>
        /// Send any queued data to the management service
        /// </summary>
        /// <returns>retry interval</returns>
        public int FlushSnapshotQueue(int timeoutMs)
        {
            using (LOG.VerboseCall("FlushSnapshotQueue ScheduledRefresh"))
            {
                int retryInterval = 5;

                int serverId;
                Serialized<ScheduledCollectionDataMessage> data;
                try
                {
                    while (scheduledCollectionQueue.Peek(out serverId, out data))
                    {
                        Collection.ManagementService.SendScheduledCollectionData(serverId, data, timeoutMs);
                        scheduledCollectionQueue.Remove(serverId, data); 
                    }
                }
                catch (Exception exception)
                {
                    // bump up the next retry to 60 seconds
                    retryInterval = 60;
                    LOG.Error("Error sending snapshot data", exception);
                }
                return retryInterval*1000;
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            using (LOG.DebugCall("Stop"))
            {
                lock (scheduledCollections)
                {
                    foreach (ScheduledCollectionContext scheduledCollection in scheduledCollections.Values)
                    {
                        LOG.Debug("Stop activity monitor trace");
                        scheduledCollection.StopActivityMonitorTrace();
                        LOG.Debug("Stop query monitor trace");
                        scheduledCollection.StopQueryMonitorTrace();

                        scheduledCollection.Dispose();
                    }
                    scheduledCollections.Clear();
                    Statistics.Dispose();
                }                
            }
        }

        public bool CollectNow(int monitoredSqlServerId, ISnapshotSink sink)
        {
            using (LOG.DebugCall("CollectNow"))
            {
                ScheduledCollectionContext context;
                if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                {
                    context.CollectNow(sink);
                    return true;
                }
                throw new CollectionServiceException(String.Format("Context not found for instance id={0}", monitoredSqlServerId));
            }
        }

        public void AddCustomCounterToServer(int monitoredSqlServerId, int metricID)
        {
            using (LOG.DebugCall("AddCustomCounterFromServer"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                    {
                        context.AddCustomCounter(metricID);
                    }
                    else
                        LOG.DebugFormat("Context not found for instance id={0}", monitoredSqlServerId);
                }
            }
        }

        public Set<int> GetServersBeingMonitored()
        {
            Set<int> result = new Set<int>();
            lock(scheduledCollections)
            {
                result.AddMany(scheduledCollections.Keys);
            }
            return result;
        }

        public void RemoveCustomCounterFromServer(int monitoredSqlServerId, int metricID)
        {
            using (LOG.DebugCall("RemoveCustomCounterFromServer"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                    {
                        context.RemoveCustomCounter(metricID);
                    } else
                        LOG.DebugFormat("Context not found for instance id={0}", monitoredSqlServerId);
                }
            }
        }

        public void RemoveCustomCounterFromAllServers(int metricID)
        {
            using (LOG.DebugCall("RemoveCustomCounterFromAllServers"))
            {
                int[] servers = null;
                lock (scheduledCollections)
                {
                    servers = Algorithms.ToArray<int>(scheduledCollections.Keys);
                }
                foreach (int serverId in servers)
                {
                    RemoveCustomCounterFromServer(serverId, metricID);                               
                }           
            }
        }

        internal void UpdateTagConfiguration(int tagId, IList<int> serverIds)
        {
            using (LOG.DebugCall("UpdateTagConfiguration"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    Set<int> keys = new Set<int>(scheduledCollections.Keys);
                    foreach (int i in Algorithms.SetDifference(keys, serverIds))
                    {
                        context = scheduledCollections[i];
                        context.RemoveServerTag(tagId);
                    }

                    foreach (int i in serverIds)
                    {
                        if (scheduledCollections.TryGetValue(i, out context))
                            context.AddServerTag(tagId);
                    }
                }
            }
        }
 

        internal void AddServerTag(int monitoredSqlServerId, int tagId)
        {
            using (LOG.DebugCall("AddServerTag"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                    {
                        context.AddServerTag(tagId);
                    }
                    else
                        LOG.WarnFormat("Context not found for instance id={0}", monitoredSqlServerId);
                }
            }
        }

        internal void RemoveServerTag(int monitoredSqlServerId, int tagId)
        {
            using (LOG.DebugCall("RemoveServerTag"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                    {
                        context.RemoveServerTag(tagId);
                    }
                    else
                        LOG.WarnFormat("Context not found for instance id={0}", monitoredSqlServerId);
                }
            }
        }

        internal void RemoveServerTags(IEnumerable<int> tagIds)
        {
            using (LOG.DebugCall("RemoveServerTags"))
            {
                lock (scheduledCollections)
                {
                    foreach (ScheduledCollectionContext context in scheduledCollections.Values) 
                    {
                        context.RemoveServerTags(tagIds);
                    }
                }
            }
        }

        internal void ClearEventState(int monitoredSqlServerId, int metricId, DateTime? cutoffTime, MonitoredObjectName objectName)
        {
            using (LOG.DebugCall("ClearEventState"))
            {
                lock (scheduledCollections)
                {
                    ScheduledCollectionContext context;
                    if (scheduledCollections.TryGetValue(monitoredSqlServerId, out context))
                    {
                        context.ClearEventState(metricId, cutoffTime, objectName);
                    }
                    else
                        LOG.WarnFormat("Context not found for instance id={0}", monitoredSqlServerId);
                }
            }
        }


        public MonitoredServerWorkload GetWorkload(int SQLServerID)
        {
            if (scheduledCollections.Keys.Contains(SQLServerID))
            {
                return scheduledCollections[SQLServerID].Workload;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Save preferred mirroring configurations for this server
        /// </summary>
        /// <param name="session"></param>
        internal void SaveMirroringPreferredConfig(MirroringSession session)
        {
            using (LOG.DebugCall("SaveMirroringPreferredConfig"))
            {
                Idera.SQLdm.Common.Snapshots.MirroringMetrics.MirroringRoleEnum role;
                bool noPreference = false;

                lock (scheduledCollections)
                {
                    ScheduledCollectionContext principalContext, mirrorContext;

                    if (session.PreferredConfig == MirroringSession.MirroringPreferredConfig.Delete) noPreference = true;

                    ServerPreferredMirrorConfig preference = null;
                    if (scheduledCollections.TryGetValue(session.PreferredPrincipalID, out principalContext))
                    {
                        if (!noPreference)
                        {
                            preference = new ServerPreferredMirrorConfig(Common.Snapshots.MirroringMetrics.MirroringRoleEnum.Principal, session.WitnessName, session.Database);
                        }
                        principalContext.UpdateMirroringPreferredConfig(session.MirroringGuid, preference);
                    }
                    else
                        LOG.WarnFormat("Context not found for mirror principal instance id={0}", session.PrincipalID);

                    if (scheduledCollections.TryGetValue(session.PreferredMirrorID, out mirrorContext))
                    {
                        if (!noPreference)
                        {
                            preference = new ServerPreferredMirrorConfig(Common.Snapshots.MirroringMetrics.MirroringRoleEnum.Mirror, session.WitnessName, session.Database);
                        }
                        mirrorContext.UpdateMirroringPreferredConfig(session.MirroringGuid, preference);
                    }
                    else
                        LOG.WarnFormat("Context not found for mirror instance id={0}", session.MirrorID);

                }
            }
        }

        #region Waits
        // This method starts a continuous collection if necessary and also allows pickup of waiting snapshots
        public ActiveWaitsSnapshot GetActiveWaits(ActiveWaitsConfiguration configuration)
        {
            try
            {
                ActiveWaitsSnapshot snapshot;
                ScheduledCollectionContext context;
                SqlConnectionInfo connection = null;
                if (scheduledCollections.TryGetValue(configuration.MonitoredServerId, out context))
                {
                    connection = context.Workload.MonitoredServer.ConnectionInfo;
                }

                if (waitStatisticsCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    // Try to pick up existing data
                    snapshot = waitStatisticsCollectors[configuration.MonitoredServerId].GetData(configuration);
                }
                else
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support for Wait Statistics
                    if (configuration.ReadyForCollection)
                    {
                        // Start new collector
                        waitStatisticsCollectors.Add(configuration.MonitoredServerId,
                                                     new ContinuousCollectionContext<ActiveWaitsSnapshot>(
                                                         configuration,
                                                         connection,
                                                         context.Workload.MonitoredServer.InstanceName,
                                                         context.Workload.MonitoredServer.CloudProviderId));
                        snapshot = new ActiveWaitsSnapshot(connection);
                    }
                    else
                    {
                        CleanupActiveWaits(configuration.MonitoredServerId);
                        snapshot = new ActiveWaitsSnapshot(connection);
                    }
                }
                if (snapshot == null)
                {
                     snapshot = new ActiveWaitsSnapshot(connection);
                }

                //SQLdm 10.0 : Small Features : Updating counter '# Wait Statistics Statements'
                Statistics.SetWaitStatisticStatements(snapshot.ActiveWaits.Rows.Count, context.Workload.MonitoredServer.InstanceName);

                return snapshot;
            }
            catch (Exception e)
            {
                LOG.Error("Error getting active waits.",e);
                return null;
            }
        }

		// Allow updating permissions on refresh click in wait monitoring - server properties
        public Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions> GetActiveWaitCollectorStatus(int MonitoredServerId)
        {
            
            if (waitStatisticsCollectors.ContainsKey(MonitoredServerId))
            {
				// Allow updating permissions on refresh click in wait monitoring - server properties
                return new
                    Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                        waitStatisticsCollectors[MonitoredServerId].RunStatus,
                        waitStatisticsCollectors[MonitoredServerId].MinimumPermissions,
                        waitStatisticsCollectors[MonitoredServerId].MetadataPermissions,
                        waitStatisticsCollectors[MonitoredServerId].CollectionPermissions);
            }
            else
            {
                return new Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions>(ContinuousCollectorRunStatus.NotCreated, MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
            }
        }

        // Stop waiting for a particular waiter
        public void StopWaitingForActiveWaits(ActiveWaitsConfiguration configuration)
        {
            if (configuration != null)
            {
                if (waitStatisticsCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    waitStatisticsCollectors[configuration.MonitoredServerId].RemoveWaiter(
                        new ContinuousCollectionWaiter<ActiveWaitsSnapshot>(configuration));
                    CleanupActiveWaits(configuration.MonitoredServerId);
                }
            }
        }

        // Stop waiting for all waiters
        public void StopActiveWaitCollector(int MonitoredServerId)
        {
            if (waitStatisticsCollectors.ContainsKey(MonitoredServerId))
            {
                waitStatisticsCollectors[MonitoredServerId].RemoveAllWaiters();
            }
            CleanupActiveWaits(MonitoredServerId);
            if (waitStatisticsCollectors.ContainsKey(MonitoredServerId))
            {
                waitStatisticsCollectors[MonitoredServerId].Dispose();
                waitStatisticsCollectors.Remove(MonitoredServerId);
            }
        }


        public void CleanupActiveWaits(int MonitoredServerId)
        {
            try
            {
                if (waitStatisticsCollectors.ContainsKey(MonitoredServerId) && waitStatisticsCollectors[MonitoredServerId].ReadyToDispose)
                {
                    LOG.Info(String.Format("Cleaning up active waits collector for {0}", scheduledCollections[MonitoredServerId].Workload.MonitoredServer.InstanceName));
                    waitStatisticsCollectors[MonitoredServerId].Dispose();
                    waitStatisticsCollectors.Remove(MonitoredServerId);
                }
                
            }
            catch (Exception e)
            {
                LOG.Error("Error cleaning up active waits collector", e);
            }
        }

        #endregion

        #region TableGrowth
        // This method starts a continuous collection if necessary and also allows pickup of waiting snapshots
        public TableGrowthSnapshot GetTableGrowth(TableGrowthConfiguration configuration)
        {
            try
            {
                TableGrowthSnapshot snapshot;
                ScheduledCollectionContext context;
                SqlConnectionInfo connection = null;
                if (scheduledCollections.TryGetValue(configuration.MonitoredServerId, out context))
                {
                    connection = context.Workload.MonitoredServer.ConnectionInfo;
                }

                if (tableGrowthCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    // Try to pick up existing data
                    snapshot = tableGrowthCollectors[configuration.MonitoredServerId].GetData(configuration);
                }
                else
                {
                    if (configuration.ReadyForCollection)
                    {
                        // SQLdm 10.3 (Varun Chopra) Linux Support for Table Growth Collector
                        // Start new collector
                        tableGrowthCollectors.Add(configuration.MonitoredServerId,
                                                     new ContinuousCollectionContext<TableGrowthSnapshot>(
                                                         configuration,
                                                         connection,
                                                         context.Workload.MonitoredServer.InstanceName,
                                                         context.Workload.MonitoredServer.CloudProviderId));
                        snapshot = new TableGrowthSnapshot(connection);
                    }
                    else
                    {
                        CleanupTableGrowth(configuration.MonitoredServerId);
                        snapshot = new TableGrowthSnapshot(connection);
                    }
                }
                if (snapshot == null)
                {
                    snapshot = new TableGrowthSnapshot(connection);
                }
                return snapshot;
            }
            catch (Exception e)
            {
                LOG.Error("Error getting table growth.", e);
                return null;
            }
        }

        public ContinuousCollectorRunStatus GetTableGrowthCollectorStatus(int MonitoredServerId)
        {

            if (tableGrowthCollectors.ContainsKey(MonitoredServerId))
            {
                return tableGrowthCollectors[MonitoredServerId].RunStatus;
            }
            else
            {
                return ContinuousCollectorRunStatus.NotCreated;
            }
        }

        // Stop waiting for a particular waiter
        public void StopWaitingForTableGrowth(TableGrowthConfiguration configuration)
        {
            if (configuration != null)
            {
                if (tableGrowthCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    tableGrowthCollectors[configuration.MonitoredServerId].RemoveWaiter(
                        new ContinuousCollectionWaiter<TableGrowthSnapshot>(configuration));
                    CleanupTableGrowth(configuration.MonitoredServerId);
                }
            }
        }

        // Stop waiting for all waiters
        public void StopTableGrowthCollector(int MonitoredServerId)
        {
            if (tableGrowthCollectors.ContainsKey(MonitoredServerId))
            {
                tableGrowthCollectors[MonitoredServerId].RemoveAllWaiters();
            }
            CleanupTableGrowth(MonitoredServerId);
            if (tableGrowthCollectors.ContainsKey(MonitoredServerId))
            {
                tableGrowthCollectors[MonitoredServerId].Dispose();
                tableGrowthCollectors.Remove(MonitoredServerId);
            }
        }


        public void CleanupTableGrowth(int MonitoredServerId)
        {
            try
            {
                if (tableGrowthCollectors.ContainsKey(MonitoredServerId) && tableGrowthCollectors[MonitoredServerId].ReadyToDispose)
                {
                    LOG.Info(String.Format("Cleaning up table growth collector for {0}", scheduledCollections[MonitoredServerId].Workload.MonitoredServer.InstanceName));
                    tableGrowthCollectors[MonitoredServerId].Dispose();
                    tableGrowthCollectors.Remove(MonitoredServerId);
                }

            }
            catch (Exception e)
            {
                LOG.Error("Error cleaning up table growth collector", e);
            }
        }

        #endregion

        #region TableFragmentation
        // This method starts a continuous collection if necessary and also allows pickup of waiting snapshots
        public TableFragmentationSnapshot GetTableFragmentation(TableFragmentationConfiguration configuration)
        {
            try
            {
                TableFragmentationSnapshot snapshot;
                ScheduledCollectionContext context;
                SqlConnectionInfo connection = null;
                if (scheduledCollections.TryGetValue(configuration.MonitoredServerId, out context))
                {
                    connection = context.Workload.MonitoredServer.ConnectionInfo;
                }

                if (tableFragmentationCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    // Try to pick up existing data
                    snapshot = tableFragmentationCollectors[configuration.MonitoredServerId].GetData(configuration);
                }
                else
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support for Table Fragmentation Collector
                    if (configuration.ReadyForCollection)
                    {
                        // Start new collector
                        tableFragmentationCollectors.Add(configuration.MonitoredServerId,
                                                     new ContinuousCollectionContext<TableFragmentationSnapshot>(
                                                         configuration,
                                                         connection,
                                                         context.Workload.MonitoredServer.InstanceName,
                                                         context.Workload.MonitoredServer.CloudProviderId));
                        snapshot = new TableFragmentationSnapshot(connection);
                    }
                    else
                    {
                        CleanupTableFragmentation(configuration.MonitoredServerId);
                        snapshot = new TableFragmentationSnapshot(connection);
                    }
                }
                if (snapshot == null)
                {
                    snapshot = new TableFragmentationSnapshot(connection);
                }
                return snapshot;
            }
            catch (Exception e)
            {
                LOG.Error("Error getting table Fragmentation.", e);
                return null;
            }
        }

        public ContinuousCollectorRunStatus GetTableFragmentationCollectorStatus(int MonitoredServerId)
        {

            if (tableFragmentationCollectors.ContainsKey(MonitoredServerId))
            {
                return tableFragmentationCollectors[MonitoredServerId].RunStatus;
            }
            else
            {
                return ContinuousCollectorRunStatus.NotCreated;
            }
        }

        // Stop waiting for a particular waiter
        public void StopWaitingForTableFragmentation(TableFragmentationConfiguration configuration)
        {
            if (configuration != null)
            {
                if (tableFragmentationCollectors.ContainsKey(configuration.MonitoredServerId))
                {
                    tableFragmentationCollectors[configuration.MonitoredServerId].RemoveWaiter(
                        new ContinuousCollectionWaiter<TableFragmentationSnapshot>(configuration));
                    CleanupTableFragmentation(configuration.MonitoredServerId);
                }
            }
        }

        // Stop waiting for all waiters
        public void StopTableFragmentationCollector(int MonitoredServerId)
        {
            if (tableFragmentationCollectors.ContainsKey(MonitoredServerId))
            {
                tableFragmentationCollectors[MonitoredServerId].RemoveAllWaiters();
            }
            CleanupTableFragmentation(MonitoredServerId);
            if (tableFragmentationCollectors.ContainsKey(MonitoredServerId))
            {
                tableFragmentationCollectors[MonitoredServerId].Dispose();
                tableFragmentationCollectors.Remove(MonitoredServerId);
            }
        }


        public void CleanupTableFragmentation(int MonitoredServerId)
        {
            try
            {
                if (tableFragmentationCollectors.ContainsKey(MonitoredServerId) && tableFragmentationCollectors[MonitoredServerId].ReadyToDispose)
                {
                    LOG.Info(String.Format("Cleaning up table Fragmentation collector for {0}", scheduledCollections[MonitoredServerId].Workload.MonitoredServer.InstanceName));
                    tableFragmentationCollectors[MonitoredServerId].Dispose();
                    tableFragmentationCollectors.Remove(MonitoredServerId);
                }

            }
            catch (Exception e)
            {
                LOG.Error("Error cleaning up table Fragmentation collector", e);
            }
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        // See ScheduledCollectionManager.ScheduledCollectionContext.cs
        // for implementation of ScheduledCollectionContext

        #endregion
    }

}
