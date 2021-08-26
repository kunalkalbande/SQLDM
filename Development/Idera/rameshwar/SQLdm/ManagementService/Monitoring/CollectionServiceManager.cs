//------------------------------------------------------------------------------
// <copyright file="CollectionServiceManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;
    using BBS.TracerX;
    using Wintellect.PowerCollections;
    
    /// <summary>
    /// Manages all collection services and sessions for the management service.
    /// </summary>
    public class CollectionServiceManager
    {
        #region fields

        private IDictionary<Guid,CollectionServiceContext> contexts;
//        private IDictionary<int, MonitoredSqlServerWrapper> monitoredServers;
        private HeartbeatMonitor heartbeatMonitor;
//        private Dictionary<object, Triple<Metric,object,object>> msEvents;
        private object sync = new object();

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CollectionServiceManager");

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceSessionManager"/> class.
        /// </summary>
        internal CollectionServiceManager()
        {            
            contexts = new Dictionary<Guid,CollectionServiceContext>();
//            monitoredServers = new Dictionary<int, MonitoredSqlServerWrapper>();
            heartbeatMonitor = new HeartbeatMonitor();
            heartbeatMonitor.CheckHeartbeats += new EventHandler(heartbeatMonitor_CheckHeartbeats);
        }

        public void Initialize()
        {
            lock (sync)
            {
                                
                try
                {
                    // attempt to pause collection for collection services we know about
                    PauseAllCollection(true);
                }
                catch
                {
                    /* */
                }

                // clear out all the collection contexts
                contexts.Clear();

                // get the management service we know about
                ManagementServiceInfo msi = Management.ManagementService;
                if (msi == null)
                {
                    throw new ServiceException(Status.ErrorInvalidManagementServiceId, ManagementServiceConfiguration.InstanceName);
                }

                // create a context for each collection service
                string connectString = ManagementServiceConfiguration.ConnectionString;
                foreach (
                    CollectionServiceInfo csi in RepositoryHelper.GetCollectionServices(connectString, null, msi.Id))
                {
                    csi.ManagementService = msi;
                    CollectionServiceContext context = new CollectionServiceContext(csi);
                    contexts.Add(csi.Id, context);
                }
//                // load up all the monitored servers and their state graphs
//                foreach (MonitoredSqlServer server in RepositoryHelper.GetMonitoredSqlServers(connectString, null, true))
//                {
//                    if (monitoredServers.ContainsKey(server.Id))
//                        monitoredServers.Remove(server.Id);
//                    AddMonitoredSqlServerWrapper(server);
//                }
            }
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

//        public MonitoredSqlServerWrapper AddMonitoredSqlServerWrapper(MonitoredSqlServer server)
//        {
//            MonitoredSqlServerWrapper wrapper = null;
//            lock (sync)
//            {
//                if (!monitoredServers.TryGetValue(server.Id, out wrapper))
//                {
//                    wrapper = new MonitoredSqlServerWrapper(server);
//                    new MonitoredSqlServerStateGraph(wrapper);
//                    monitoredServers.Add(server.Id, wrapper);
//                }
//            }
//            return wrapper;
//        }

        /// <summary>
        /// Gets the copy of the list of available collection services.
        /// </summary>
        /// <returns></returns>
        public IList<CollectionServiceInfo> GetCollectionServices()
        {
            List<CollectionServiceInfo> result = new List<CollectionServiceInfo>();
            lock (sync)
            {
                foreach (CollectionServiceContext context in contexts.Values)
                {
                    result.Add(context.CollectionService);
                }
            }
            return result;
        }

//        public MonitoredSqlServerWrapper GetCachedMonitoredSqlServer(int id)
//        {
//            MonitoredSqlServerWrapper wrapper = null;
//            lock (sync)
//            {
//                monitoredServers.TryGetValue(id, out wrapper);
//            }
//            return wrapper;
//        }
//
//        public MonitoredSqlServerWrapper GetCachedMonitoredSqlServer(string instanceName)
//        {
//            MonitoredSqlServerWrapper wrapper = null;
//
//            lock (sync)
//            {
//                foreach (MonitoredSqlServerWrapper server in monitoredServers.Values)
//                {
//                    if (server.WrappedServer.InstanceName.Equals(instanceName))
//                    {
//                        wrapper = server;
//                        break;
//                    }
//                }
//            }
//
//            return wrapper;
//        }

        public CollectionServiceContext this[Guid id]
        {
            get
            {
                CollectionServiceContext result = null;
                lock (sync)
                {
                    contexts.TryGetValue(id, out result);
                }
                return result;
            }
        }

        public Guid DefaultCollectionServiceID
        {
            get
            {
                ManagementServiceInfo msi = Management.ManagementService;
                if (msi != null)
                {
                    if (msi.DefaultCollectionServiceID.HasValue)
                    {
                        return msi.DefaultCollectionServiceID.Value;
                    }
                }
                return Guid.Empty;
            }
        }

        public bool GetCollectionServiceId(string collectionServiceInstance, string collectionServiceMachine, out Guid? collectionServiceId)
        {
            foreach (CollectionServiceContext context in contexts.Values)
            {
                CollectionServiceInfo csi = context.CollectionService;
                if (csi.InstanceName.Trim().Equals(collectionServiceInstance) &&
                    csi.MachineName.Trim().Equals(collectionServiceMachine))
                {
                    collectionServiceId = csi.Id;
                    return true;
                }
            }
            collectionServiceId = default(Guid);
            return false;
        }

        public CollectionServiceWorkload GetCollectionServiceWorkload(Guid collectionServiceId)
        {
            using (LOG.InfoCall("GetCollectionServiceWorkload"))
            {
                CollectionServiceContext csi = this[collectionServiceId];
                
                string connection = ManagementServiceConfiguration.ConnectionString;

                IList<MonitoredServerWorkload> serverWork = new List<MonitoredServerWorkload>();
                IList<MonitoredSqlServer> servers =
                    RepositoryHelper.GetMonitoredSqlServers(connection, collectionServiceId, true);

                Triple<MultiDictionary<int, int>,
                    MultiDictionary<int, int>,
                    MultiDictionary<int, int>> tagAssociations =
                        RepositoryHelper.GetTagAssociations(connection, null, true, true, false);

                //Fetch whatever mirroring relationships are being monitored
                Dictionary<Guid, MirroringSession> mirroringConfig = RepositoryHelper.GetMirroringPreferredConfig(connection);

                foreach (MonitoredSqlServer server in servers)
                {
                    MonitoredServerWorkload serverWorkload = GetMonitoredServerWorkload(server);
                    if (tagAssociations.First != null)
                    {
                        ICollection<int> tags = tagAssociations.First[server.Id];
                        if (tags != null && tags.Count > 0)
                        {
                            server.Tags = new List<int>(tags);
                        }
                    }
                    Dictionary<Guid, ServerPreferredMirrorConfig> localRoles = getPreferredConfigForThisServer(server.Id, mirroringConfig);

                    foreach (KeyValuePair<Guid, ServerPreferredMirrorConfig> role in localRoles)
                    {
                        serverWorkload.UpdateMirroringSessions(role.Key, role.Value);
                    }
                    LOG.DebugFormat("ServerWorkload\n id: {0}\n name: {1}\n monitoredServer: {2}\n normalCollectionInterval: {3}", serverWorkload.Id, serverWorkload.Name, serverWorkload.MonitoredServer, serverWorkload.NormalCollectionInterval);
                    serverWork.Add(serverWorkload);
                }

                MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();

                CollectionServiceWorkload workload =
                    new CollectionServiceWorkload(csi.CollectionService, serverWork, metricDefinitions);
                
                workload.CustomCounterTags = tagAssociations.Second;

                List<Pair<String, int?>> excludedWaitTypes = RepositoryHelper.GetExcludedWaitTypes(connection);

                workload.ExcludedWaitTypes = excludedWaitTypes;
                
                return workload;
            }
        }

        /// <summary>
        /// Build the workload for a single monitored sql server.
        /// </summary>
        /// <param name="monitoredSqlServer"></param>
        /// <returns></returns>
        public MonitoredServerWorkload GetMonitoredServerWorkload(MonitoredSqlServer monitoredSqlServer)
        {
            using (LOG.InfoCall("GetMonitoredServerWorkload"))
            {
                MonitoredServerWorkload workload = new MonitoredServerWorkload();
                workload.MonitoredServer = monitoredSqlServer;

                IDictionary<int,List<MetricThresholdEntry>> thresholds = null;
                IList<OutstandingEventEntry> events = null;
                Set<int> counters = null;
                Dictionary<int, bool> metricCompatibilityForSqlExpress = null; //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
                BaselineMetricMeanCollection baselineMetricMean = null; //SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
                int? cloudProviderId = null;
                RepositoryHelper.GetMonitoredSqlServerWorkload(
                    ManagementServiceConfiguration.ConnectionString,
                    monitoredSqlServer.Id,
                    out thresholds,
                    out events,
                    out counters,
                    out metricCompatibilityForSqlExpress,
                    out baselineMetricMean,
                    out cloudProviderId//SQLdm 10.0 (Tarun Sapra)- Added an out param for getting the cloud provider
                    );

                workload.ThresholdInstances = thresholds;
                workload.OutstandingEvents = events;
                workload.CustomCounters.AddRange(counters);
                workload.MetricCompatibilityForSqlExpress = metricCompatibilityForSqlExpress;
                workload.BaselineMetricMeanCollection = baselineMetricMean;
                workload.MonitoredServer.CloudProviderId = cloudProviderId;//SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support
                return workload;
            }
        }

        /*
        public CollectionServiceWorkloadMessage ReopenSession(Guid sessionId)
        {
            LOG.InfoFormat("Reopening Collection Service session ({0})", sessionId);
            lock (sessions)
            {
                CollectionServiceContext session = sessions.GetBySessionId(sessionId);
                if (session == null)
                    return null;

                IList<MonitoredServerWorkload> workload = null;

                if (session.ConfigurationChanged)
                {
                    using (IDataSession s = dataManager.GetSession())
                    {
                        CollectionServiceWorkload csw = dataManager.GetCollectionServiceWorkload(session.ServiceId);
                        if (csw != null)
                            workload = csw.MonitoredServerWorkloads;
                    }
                }
                return new CollectionServiceWorkloadMessage(sessionId, Result.Success, ManagementServiceConfiguration.HeartbeatInterval, workload);
            }
            return null;
        }
         **/

        /// <summary>
        /// Notification from a collection service that it is shutting down.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public bool CloseSession(Guid sessionId)
        {
            LOG.InfoFormat("Closing Collection Service session for ({0})", sessionId);
            return true;
        }

        public bool StartMonitoringServer(MonitoredSqlServer server)
        {
            if (server.CollectionServiceId == Guid.Empty || !server.IsActive)
                return false;

            MonitoredServerWorkload workload = GetMonitoredServerWorkload(server);
            return StartMonitoringServer(workload);
        }

        /// <summary>
        /// Starts the monitoring server.
        /// </summary>
        /// <param name="workload">The monitored server.</param>
        /// <returns></returns>
        public bool StartMonitoringServer(MonitoredServerWorkload workload)
        {
            Guid collectionServiceId = workload.MonitoredServer.CollectionServiceId;
            if (collectionServiceId == Guid.Empty)
                return false;

            CollectionServiceContext session = this[collectionServiceId];
            if (session == null)
                return false;

            ICollectionService collsvc = session.GetService();

            return (collsvc != null && collsvc.StartMonitoringServer(workload) == Result.Success);
        }

        /// <summary>
        /// Stops the monitoring server.
        /// </summary>
        /// <param name="monitoredSqlServerId">The monitored server.</param>
        /// <returns></returns>
        public bool StopMonitoringServer(int monitoredSqlServerId)
        {
            LOG.DebugFormat("Sending stop request to collection service for server: {0}", monitoredSqlServerId);
            ICollectionService collsvc = GetCollectionServiceForServer(monitoredSqlServerId);
            
            return (collsvc != null && collsvc.StopMonitoringServer(monitoredSqlServerId) == Result.Success);
        }

        /// <summary>
        /// Sends an updated workload to the collection service for the specified server.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public bool UpdateMonitoredServer(MonitoredSqlServer server)
        {
            if (server.CollectionServiceId == Guid.Empty || !server.IsActive)
                return false;
            LOG.DebugFormat("Updating collection service for server Id: {0}, Name: {1}" ,server.Id, server.InstanceName);
            MonitoredServerWorkload workload = GetMonitoredServerWorkload(server);
            return UpdateMonitoredServer(workload);
        }
             
        /// <summary>
        /// Sends an updated workload to the collection service for the server specified 
        /// in the given workload.
        /// </summary>
        /// <param name="workload"></param>
        /// <returns></returns>    
        public bool UpdateMonitoredServer(MonitoredServerWorkload workload)
        {
            Guid collectionServiceId = workload.MonitoredServer.CollectionServiceId;
            if (collectionServiceId == Guid.Empty)
                return false;

            CollectionServiceContext session = this[collectionServiceId];
            if (session == null)
                return false;

            ICollectionService collsvc = session.GetService();
            return (collsvc != null && collsvc.ReconfigureMonitoredServer(workload) == Result.Success);
        }

        #region Heartbeat methods

        /// <summary>
        /// Updates the heartbeat.
        /// </summary>
        public void UpdateHeartbeat(Guid collectionServiceId, TimeSpan nextExpectedHeartbeat, DateTime? lastSnapshotDeliveryAttempt, TimeSpan? lastSnapshotDeliveryAttemptTime, Exception lastSnapshotDeliveryException, int scheduledRefreshDeliveryTimeoutCount)
        {
            CollectionServiceContext context = this[collectionServiceId];
            if (context != null)
            {
                Management.QueueDelegate(delegate() {
                    CollectionServiceInfo csi = context.CollectionService;
                    LOG.DebugFormat("Heartbeat received from Collection Service ({0}${1})", csi.MachineName, csi.InstanceName);
                    heartbeatMonitor.UpdateHeartbeat(context, DateTime.UtcNow, lastSnapshotDeliveryAttempt, lastSnapshotDeliveryAttemptTime, lastSnapshotDeliveryException, scheduledRefreshDeliveryTimeoutCount);                             
                }); 
            }
        }
        
        /// <summary>
        /// Handles the CheckHeartbeats event of the heartbeatMonitor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void heartbeatMonitor_CheckHeartbeats(object sender, EventArgs e)
        {
            foreach (CollectionServiceContext context in Collections.ToArray(contexts.Values, sync))
            {
                heartbeatMonitor.CheckHeartbeart(context);
                if (!heartbeatMonitor.IsSessionCurrent(context))
                {
                    CollectionServiceInfo csi = context.CollectionService;
                    LOG.WarnFormat("Heartbeat missed from Collection Service ({0}${1})", csi.MachineName, csi.InstanceName);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the collection service by id.
        /// </summary>
        /// <param name="collectionServiceId">The collection service id.</param>
        /// <returns></returns>
        public ICollectionService GetCollectionServiceById(Guid collectionServiceId)
        {
            CollectionServiceContext csc = this[collectionServiceId];
            return (csc != null) ? csc.GetService() : null;
        }

        public ICollectionService DefaultCollectionService
        {
            get
            {
                Guid csid = DefaultCollectionServiceID;
                if (csid != Guid.Empty)
                    return GetCollectionServiceById(csid);

                return null;
            }
        }

        public Guid RegisterCollectionService(string machineName, string instanceName, string address, int servicePort, bool force)
        {
            ManagementServiceInfo managementService = Management.ManagementService;

            CollectionServiceInfo csi = new CollectionServiceInfo();
            csi.InstanceName = instanceName;
            csi.MachineName = machineName;
            csi.Address = address;
            csi.Port = servicePort;
            csi.Enabled = true;
            csi.ManagementService = managementService;

            RepositoryHelper.AddCollectionService(
                ManagementServiceConfiguration.ConnectionString,
                csi);

            lock (sync)
            {
                if (!managementService.DefaultCollectionServiceID.HasValue)
                    managementService.DefaultCollectionServiceID = csi.Id;
                CollectionServiceContext context = new CollectionServiceContext(csi);
                contexts.Add(csi.Id, context);
            }

            return csi.Id;
        }

        /// <summary>
        /// Gets the collection service for server.
        /// </summary>
        /// <param name="instanceId">The ID of the Monitored Server.</param>
        /// <returns></returns>
        public ICollectionService GetCollectionServiceForServer(int instanceId)
        {
            string instanceName;
            return GetCollectionServiceForServer(instanceId, out instanceName);
        }

        public ICollectionService GetCollectionServiceForServer(int instanceId, out string instanceName)
        {
            Guid? collectionServiceId;

            RepositoryHelper.GetCollectionServiceIdForServer(
                ManagementServiceConfiguration.ConnectionString,
                instanceId,
                out collectionServiceId,
                out instanceName);

            if (collectionServiceId.HasValue)
            {
                return GetCollectionServiceById(collectionServiceId.Value);
            }
            return null;
        }


        public void PauseAllCollection(bool reinitialize)
        {
            using (LOG.DebugCall()) {
                foreach (Guid collectionServiceId in contexts.Keys) {
                    PauseCollection(collectionServiceId, reinitialize);
                }
            }
        }

        public void PauseCollection(Guid collectionServiceId, bool reinitialize)
        {
            ICollectionService collectionService = GetCollectionServiceById(collectionServiceId);
            try
            {
                if (!reinitialize)
                    collectionService.PauseService();
                else
                    collectionService.ReinitializeService();
            } catch (Exception)
            {
                /* silence! */
            }
        }

        public void ResumeAllCollection()
        {
            using (LOG.DebugCall()) {
                foreach (Guid collectionServiceId in contexts.Keys) {
                    ResumeCollection(collectionServiceId);
                }
            }
        }

        public void ResumeCollection(Guid collectionServiceId)
        {
            ICollectionService collectionService = GetCollectionServiceById(collectionServiceId);
            try
            {
                if (collectionService != null)
                    collectionService.ResumeService();
            } catch (Exception e)
            {
                LOG.Error("Error resuming collection service", e);
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        internal bool Close(Guid collectionServiceId)
        {
            using (LOG.InfoCall( "Close"))
            {
                LOG.DebugFormat("Collection Service {0} close notification.", collectionServiceId.ToString());
            }
            return true;
        }
        /// <summary>
        /// Select out the subset of preferred mirroring configurations that apply to this server
        /// </summary>
        /// <param name="ServerID"></param>
        /// <param name="allSessions"></param>
        /// <returns></returns>
        private Dictionary<Guid, ServerPreferredMirrorConfig>  getPreferredConfigForThisServer(int ServerID, Dictionary<Guid, MirroringSession> allSessions)
        {
            Dictionary<Guid, ServerPreferredMirrorConfig> result = new Dictionary<Guid, ServerPreferredMirrorConfig>();
            ServerPreferredMirrorConfig preference = null;
            foreach(MirroringSession session in allSessions.Values)
            {

                //if this server is involved in mirroring
                if(session.PreferredMirrorID == ServerID)
                {
                    preference = new ServerPreferredMirrorConfig(
                            Idera.SQLdm.Common.Snapshots.MirroringMetrics.MirroringRoleEnum.Mirror, session.WitnessName,
                            session.Database);
                    result.Add(session.MirroringGuid, preference);
                }
                if (session.PreferredPrincipalID == ServerID)
                {
                    preference = new ServerPreferredMirrorConfig(
                        Idera.SQLdm.Common.Snapshots.MirroringMetrics.MirroringRoleEnum.Principal, session.WitnessName,
                        session.Database);
                    result.Add(session.MirroringGuid, preference);
                }
            }
            return result;
        }
    }
}
