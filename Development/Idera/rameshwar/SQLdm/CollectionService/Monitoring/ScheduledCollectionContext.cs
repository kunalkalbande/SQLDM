//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Diagnostics;
using System.Runtime.Remoting.Lifetime;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Messages;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    using System;
    using System.Threading;
    using Idera.SQLdm.CollectionService.Helpers;
    using Idera.SQLdm.CollectionService.Probes;
    using Idera.SQLdm.CollectionService.Probes.Sql;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.VMware;
    using Idera.SQLdm.Common.Snapshots.State;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    using System.Collections.Generic;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.CollectionService.Configuration;

    partial class ScheduledCollectionManager
    {

        

        /// <summary>
        /// This class is used for on-demand collection.  When a client requests on-demand data from the
        /// management service, it creates a new instance of this class and passes it to the collection service.
        /// As data is returned to the management service, it is forwarded back to the client.
        /// </summary>
        protected class OnDemandCollectionContext<T> : MarshalByRefObject, ISnapshotSink, IDisposable, ISponsor
        {
            private static readonly TimeSpan DEFAULT_WAITTIME = TimeSpan.FromSeconds(90);

            private bool disposed = false;
            private bool cancelled = false;
            private Serialized<T> snap = null;
            private ManualResetEvent readyEvent = new ManualResetEvent(false);
            private Stopwatch stopwatch = new Stopwatch();
            private TimeSpan waitTime = DEFAULT_WAITTIME;
            

            private Guid sessionId;

            public OnDemandCollectionContext()
            {
            }

            public OnDemandCollectionContext(TimeSpan waitTime)
            {
                this.waitTime = waitTime;
            }

            /// <summary>
            /// Sets the session id of this context in order to support cancelling the request.
            /// </summary>
            /// <param name="sessionId"></param>
            public void SetSessionId(Guid sessionId)
            {
                this.sessionId = sessionId;
                Dictionary<Guid, ISnapshotSink> sinks = Collection.SnapshotSinks;
                ISnapshotSink sink = null;
                lock (sinks)
                {
                    // cancel an existing request with the same id
                    if (sinks.TryGetValue(sessionId, out sink))
                    {
                        sink.Cancelled = true;
                        sinks.Remove(sessionId);
                    }
                    // keep track of this session 
                    sinks.Add(sessionId, this);
                }
            }

            

            TimeSpan ISponsor.Renewal(ILease lease)
            {
                lock (readyEvent)
                {
                    if (disposed)
                        return TimeSpan.Zero;

                    TimeSpan elapsed = stopwatch.Elapsed;
                    TimeSpan ttl = waitTime + TimeSpan.FromSeconds(15);
                    return elapsed > ttl ? TimeSpan.Zero : ttl - elapsed;
                }
            }

            public void Dispose()
            {
                lock (readyEvent)
                {
                    if (!disposed)
                    {
                        disposed = true;
                        snap = null;

                        // make sure the event handle gets closed
                        readyEvent.Close();

                       if (sessionId != default(Guid))
                        {
                            Dictionary<Guid, ISnapshotSink> sinks = Collection.SnapshotSinks;
                            ISnapshotSink sink = null;
                            lock (sinks)
                            {
                                if (sinks.TryGetValue(sessionId, out sink))
                                {
                                    // only remove ourselves from the map
                                    if (sink == this)
                                        sinks.Remove(sessionId);
                                }
                            }
                        }
                    }
                }
            }

            public static ISnapshotSink GetSink(Guid sessionId)
            {
                ISnapshotSink sink = null;
                if (sessionId != default(Guid))
                {
                    Dictionary<Guid, ISnapshotSink> sinks = Collection.SnapshotSinks;
                    lock (sinks)
                    {
                        sinks.TryGetValue(sessionId, out sink);
                    }
                }
                return sink;
            }

            public Serialized<T> Wait()
            {
                stopwatch.Start();

                for (TimeSpan timeout = waitTime; waitTime > stopwatch.Elapsed; timeout = waitTime - stopwatch.Elapsed)
                {
                    if (readyEvent.WaitOne(timeout, false))
                    {
                        stopwatch.Stop();
                        return snap;
                    }
                }

                throw new ServiceException(Status.ErrorRequestTimeout, "");
            }

            /// <summary>
            /// Callback from the Collection Service with the results of a client on-demand request.
            /// </summary>
            /// <param name="snapshot">The collected snapshot</param>
            /// <param name="state">Object passed to the collection service for correlation</param>
            public void Process(ISerialized snapshot, object state)
            {
                if (!disposed)
                {
                    try
                    {
                        this.snap = (Serialized<T>)snapshot;
                    }
                    finally
                    {
                        // make sure to signal that the request is complete
                        lock (readyEvent)
                        {
                            readyEvent.Set();
                        }
                    }
                }
            }

            #region ISnapshotSink Members

            public TimeSpan ManagementServiceWaitTime
            {
                get
                {
                    return waitTime;
                }
                set
                {
                    waitTime = value;
                }
            }

            public bool Cancelled
            {
                get
                {
                    return cancelled;
                }
                set
                {
                    cancelled = value;
                }
            }

            /// <summary>
            /// Registers the sink so that it can be retrieved again using its id.
            /// </summary>
            /// <returns></returns>
            public Guid RegisterSink()
            {
                if (sessionId == Guid.Empty)
                {
                    sessionId = Guid.NewGuid();
                    SetSessionId(sessionId);
                }

                return sessionId;
            }

            #endregion

            /// <summary>
            /// SQLdm 10.0 Vineet -- Added to support multiple probes
            /// </summary>
            /// <param name="listSnapshot"></param>
            /// <param name="state"></param>
            public void ProcessMultiple(List<ISerialized> listSnapshot, object state)
            {
                throw new NotImplementedException();
            }
        }

        internal class CollectNowHelper
        {
            private List<ISnapshotSink> waitingRequests = new List<ISnapshotSink>();
            private int waitingCollectors = 0;
            private List<Type> waitingFor = new List<Type>();
            

            public CollectNowHelper()
            {
            }

            public List<ISnapshotSink> WaitingRequests
            {
                get { return waitingRequests; }
                set { waitingRequests = value; }
            }

            public List<Type> WaitingFor
            {
                get { return waitingFor; }
                set { waitingFor = value; }
            }


            public void AddSink(ScheduledCollectionDataMessage message)
            {
                lock (WaitingRequests)
                {
                    if (WaitingRequests.Count > 0)
                    {
                        if (WaitingFor.Contains(message.Snapshot.GetType()))
                            WaitingFor.Remove(message.Snapshot.GetType());

                        if (WaitingRequests.Count > 0 && WaitingFor.Count == 0)
                        {
                            foreach (ISnapshotSink sink in WaitingRequests)
                                message.AddSink(sink);

                            WaitingRequests.Clear();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// An instance of this class is used by ScheduledCollectionManager
        /// for each monitored server's scheduled collection.  It contains
        /// context info such as connection information, state information
        /// used to determine events, and a previous value cache for computing
        /// delta values.
        /// </summary>
        private class ScheduledCollectionContext : IDisposable
        {
            #region fields

            private BBS.TracerX.Logger LOG;
            private ScheduledCollectionManager manager;
            private MonitoredServerWorkload monitoredServerWorkload;
            private QueryMonitorCollectionState qmCollectionState;
            private ActivityMonitorCollectionState activityMonitorCollectionState;
            private ReaderWriterLock sync = new ReaderWriterLock();

            private Timer normalCollectionTimer;
            private Semaphore normalCollectionSemaphore;
            private volatile bool disposed = false;
            private object syncRoot = new object();
            private object syncRoot_ping = new object();
            //private object syncRoot_db = new object();

            private bool inScheduledRefreshSnapshotCallback = false;
            private bool snapshotSent = false;
            private CollectNowHelper collectNowHelper = new CollectNowHelper();
            

            
            private Timer serverPingTimer;
            private ServerPingProbe serverPingProbe;
            private bool pingRunning = false;

            //private Timer databaseStatisticsTimer;
            //private DatabaseSummaryProbe databaseStatisticsProbe;
            //private bool databaseStatisticsRunning;

            private PeriodicCollectionContext<DatabaseConfigurationSnapshot> databasePeriodicCollector;

            #endregion

            #region constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="T:ScheduledCollectionInfo"/> class.
            /// </summary>
            /// <param name="manager">The manager.</param>
            /// <param name="monitoredServerWorkload">The monitored server workload.</param>
            public ScheduledCollectionContext(ScheduledCollectionManager manager, MonitoredServerWorkload monitoredServerWorkload)
            {
                if (manager == null)
                    throw new ArgumentNullException("manager");
                if (monitoredServerWorkload == null)
                    throw new ArgumentNullException("monitoredServerWorkload");

                LOG = Logger.GetLogger("ScheduledCollectionContext:" + monitoredServerWorkload.Name);

                normalCollectionSemaphore = new Semaphore(1, 1);

                this.manager = manager;
                this.monitoredServerWorkload = monitoredServerWorkload;

                collectNowHelper = new CollectNowHelper();

            }

            #endregion

            #region properties

            public MonitoredServerWorkload Workload
            {
                get
                {
                    sync.AcquireReaderLock(-1);
                    MonitoredServerWorkload result = monitoredServerWorkload;
                    sync.ReleaseReaderLock();
                    return result;
                }
            }

            #endregion

            #region events

            #endregion

            #region methods

            /// <summary>
            /// Starts the Scheduled Collection Timer.
            /// </summary>
            /// <param name="initialDelaySeconds">The initial delay in minutes.</param>
            public void Start(int initialDelaySeconds)
            {
                lock (syncRoot)
                {
                    if (normalCollectionTimer != null)
                        normalCollectionTimer.Dispose();

                    LOG.DebugFormat("Next refresh scheduled in {0} seconds", initialDelaySeconds);

                    // Tolga K - to fix memory leak begins
                    if (normalCollectionTimer != null)
                    {
                        normalCollectionTimer.Dispose();
                    }
                    // Tolga K - to fix memory leak ends

                    normalCollectionTimer = new Timer(
                        new TimerCallback(CollectNormal),
                        null,
                        initialDelaySeconds * 1000,
                        Timeout.Infinite);

                    if (!pingRunning)
                        StartPing();

                    // 30 second delay on first starting the database collector
                    // Dual purpose - slightly reduce startup monitoring hit and increase chance that scheduled refresh finishes first
                    // Scheduled refresh does not *have* to finish first but it feels a little more natural alerting-wise
                    if (databasePeriodicCollector == null)
                        StartDatabasePeriodicCollector(30);
                }
            }

            public void StartPing()
            {
                pingRunning = true;
                if (serverPingTimer != null)
                    serverPingTimer.Dispose();

                int pingInterval = 30000;
                if (monitoredServerWorkload != null && monitoredServerWorkload.MonitoredServer != null)
                {
                    pingInterval = (int)monitoredServerWorkload.MonitoredServer.ServerPingInterval.TotalMilliseconds;
                }

                // Tolga K - to fix memory leak begins
                if (serverPingTimer != null)
                {
                    serverPingTimer.Dispose();
                }
                // Tolga K - to fix memory leak ends

                serverPingTimer = new Timer(new TimerCallback(PingServer), TimeSpan.FromMilliseconds(pingInterval >= 1000 ? pingInterval - 500: 500), 0, pingInterval);
            }

                
            

            public void Reconfigure(MonitoredServerWorkload monitoredServerWorkload)
            {
                sync.AcquireWriterLock(-1);
                try
                {
                    bool restartTimer = (this.monitoredServerWorkload.NormalCollectionInterval != monitoredServerWorkload.NormalCollectionInterval);
                    bool restartPing = (this.monitoredServerWorkload.MonitoredServer.ServerPingInterval !=
                                        monitoredServerWorkload.MonitoredServer.ServerPingInterval);
                    bool restartDatabaseStatistics = databasePeriodicCollector == null || 
                        (this.monitoredServerWorkload.MonitoredServer.DatabaseStatisticsInterval != monitoredServerWorkload.MonitoredServer.DatabaseStatisticsInterval);
                    
                    LOG.Debug(String.Format("Reconfigure restart bools: restartTimer - {0}, restartPing - {1}, restartDatabaseStatistics - {2}",restartTimer,restartPing,restartDatabaseStatistics));
                    // retain the previous refresh and the current state graph
                    monitoredServerWorkload.PreviousRefresh = this.monitoredServerWorkload.PreviousRefresh;
                    monitoredServerWorkload.PeriodicRefreshPreviousValues = this.monitoredServerWorkload.PeriodicRefreshPreviousValues;
                    monitoredServerWorkload.StateGraph = this.monitoredServerWorkload.StateGraph;


                    monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime = this.monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime;
                    monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime = this.monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime;

                    if (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.EqualsLimited(this.monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration))
                    {
                        monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus =
                            this.monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus;
                        monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.
                            PresentFragmentationStatisticsRunTime =
                            this.monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.
                                PresentFragmentationStatisticsRunTime;
                    }

                    monitoredServerWorkload.MonitoredServer.ActiveClusterNode = this.monitoredServerWorkload.MonitoredServer.ActiveClusterNode;

                    //flag this configuration as containing the latest workload
                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.IsMaster = true;

                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(this.monitoredServerWorkload.MonitoredServer.Id,
                        this.monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration, monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                    
                    this.monitoredServerWorkload = monitoredServerWorkload;
                    if (this.databasePeriodicCollector != null)
                        this.databasePeriodicCollector.Reconfigure(monitoredServerWorkload);

                    if (ServerInMaintenanceMode(monitoredServerWorkload.MonitoredServer,LOG))
                    {
                        PersistenceManager.Instance.SetFailedJobInstanceId(this.monitoredServerWorkload.MonitoredServer.InstanceName, 0);
                        PersistenceManager.Instance.SetCompletedJobInstanceId(this.monitoredServerWorkload.MonitoredServer.InstanceName, 0);
                        this.monitoredServerWorkload.PreviousRefresh = null;
                        this.monitoredServerWorkload.PeriodicRefreshPreviousValues.Clear();
                        this.monitoredServerWorkload.MonitoredServer.ActiveClusterNode = null;
                        Collection.Scheduled.StopWaitingForActiveWaits(monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                        
                        LOG.Debug("Stopping database statistics collector because of maintenance mode");
                        StopDatabasePeriodicCollector();
                    }
                    else
                    {
                        //if no longer enabled then stop the waiter
                        //when in the pickup window it is still enabled
                        if (!monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.Enabled)
                        {
                            Collection.Scheduled.StopWaitingForActiveWaits(
                                monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                        }

                        if (restartDatabaseStatistics)
                        {
                            LOG.Debug("Restarting database statistics collector");
                            StopDatabasePeriodicCollector();
                            StartDatabasePeriodicCollector(0);
                        }
                    }
    
                    if (monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.Enabled && monitoredServerWorkload.MonitoredServer.CloudProviderId != Constants.MicrosoftAzureId)
                        StartQueryMonitorTrace();

                    if (monitoredServerWorkload.MonitoredServer.ActivityMonitorConfiguration != null)
                    {
                        if (monitoredServerWorkload.MonitoredServer.ActivityMonitorConfiguration.Enabled && monitoredServerWorkload.MonitoredServer.CloudProviderId != Constants.MicrosoftAzureId)
                        {
                            StartActivityMonitorTrace();
                        }
                    }

                    if (restartTimer)
                        Start(0);

                    if (restartPing)
                    {
                        if (serverPingTimer != null)
                            serverPingTimer.Dispose();
                        StartPing();
                    }

                    
                }
                finally
                {
                    sync.ReleaseWriterLock();
                }
            }

            public void UpdateThresholdEntries(IEnumerable<MetricThresholdEntry> thresholdEntries)
            {                
                sync.AcquireWriterLock(-1);
                try
                {
                    IDictionary<int, List<MetricThresholdEntry>> thresholdMap = monitoredServerWorkload.ThresholdInstances;
                    foreach(MetricThresholdEntry entry in thresholdEntries)
                    {
                        if (thresholdMap.ContainsKey(entry.MetricID))
                        {
                            List<MetricThresholdEntry> mteList = thresholdMap[entry.MetricID];
                            foreach (MetricThresholdEntry mte in mteList)
                            {
                                if (mte.MetricInstanceName == entry.MetricInstanceName)
                                {
                                    mteList.Remove(mte);
                                    break;
                                }
                            }
                            if (entry.State != ThresholdState.Deleted)
                                thresholdMap[entry.MetricID].Add(entry);
                        }
                        else
                        {
                            if (entry.State != ThresholdState.Deleted)
                                thresholdMap.Add(entry.MetricID, new List<MetricThresholdEntry>() { entry });
                        }
                        monitoredServerWorkload.ReloadDefaultThresholds = true;
                    }
                    LOG.DebugFormat("Thresholds updated for instance {0}", monitoredServerWorkload.Name);
                }
                finally
                {
                    sync.ReleaseWriterLock();
                }
            }

            #region Database Statistics Periodic Refresh

            private void StartDatabasePeriodicCollector(int initialDelaySeconds, object state = null)
            {
                if (databasePeriodicCollector != null && state == null) return;
                if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled) return;

                if (databasePeriodicCollector == null)
                {
                    databasePeriodicCollector =
                    new PeriodicCollectionContext<DatabaseConfigurationSnapshot>(
                        new PeriodicProbe(DatabasePeriodicCollectorProbe), new CallbackAction(DatabasePeriodicCallbackAction),
                        new Interval(DatabasePeriodicCollectorInterval), monitoredServerWorkload,
                        "DatabaseStatistics", collectNowHelper);
                }

                if (state is ISnapshotSink)
                {
                    // Start collector in response to "Collect Now" request
                    databasePeriodicCollector.Start(state);
                }
                else
                {
                    // Regular collector start
                    databasePeriodicCollector.Start(initialDelaySeconds);
                }
            }

            private void StopDatabasePeriodicCollector()
            {
                if (databasePeriodicCollector != null)
                {
                    databasePeriodicCollector.Dispose();
                }

                databasePeriodicCollector = null;
            }

            internal SqlBaseProbe DatabasePeriodicCollectorProbe()
            {
                MonitoredSqlServer server = monitoredServerWorkload.MonitoredServer;
                DatabaseSizeSnapshot previousValues = null;
                if (monitoredServerWorkload.PeriodicRefreshPreviousValues.ContainsKey(typeof(DatabaseSizeSnapshot)))
                {
                    previousValues = (DatabaseSizeSnapshot)monitoredServerWorkload.PeriodicRefreshPreviousValues[typeof (DatabaseSizeSnapshot)];
                }
                var config = new DatabaseSizeConfiguration(server.Id, previousValues);
                return (SqlBaseProbe)ProbeFactory.BuildDatabaseSizeProbe(server.ConnectionInfo, config, server.WmiConfig, server.DiskCollectionSettings,monitoredServerWorkload.MonitoredServer.CloudProviderId);
            }

            internal void DatabasePeriodicCallbackAction(Snapshot snapshot)
            {
                // Set previous values for next refresh, if server is not in maintenance mode
                if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == false)
                {
                    if (monitoredServerWorkload.PeriodicRefreshPreviousValues.ContainsKey(typeof (DatabaseSizeSnapshot)))
                    {
                        monitoredServerWorkload.PeriodicRefreshPreviousValues[typeof (DatabaseSizeSnapshot)] = snapshot;
                    }
                    else
                    {
                        monitoredServerWorkload.PeriodicRefreshPreviousValues.Add(typeof (DatabaseSizeSnapshot),
                                                                                  snapshot);
                    }
                }
            }

            internal TimeSpan DatabasePeriodicCollectorInterval()
            {
                int databaseStatisticsInterval = 60 * 60 * 1000;
                if (monitoredServerWorkload != null && monitoredServerWorkload.MonitoredServer != null)
                {
                    return monitoredServerWorkload.MonitoredServer.DatabaseStatisticsInterval;
                }
                else
                {
                    return TimeSpan.FromSeconds(databaseStatisticsInterval);
                }
            }

           

            #endregion

            #region Periodic Collectors



            #endregion

            public bool IsDisposed
            {
                get { 
                    //This lock was causing a deadlock
                    //with ServerPingCallback and ScheduledCollectionCallback
                    //lock (syncRoot)
                    //{
                        return disposed;    
                    //}
                }
            }

            private void PingServer(object state)
            {
                if (IsDisposed)
                    return;

                using (LOG.DebugCall("PingServer"))
                {
                    if (!ServerInMaintenanceMode(monitoredServerWorkload.MonitoredServer, LOG))
                    {
                        try
                        {
                           
                            sync.AcquireReaderLock(-1);

                            MonitoredSqlServer server = monitoredServerWorkload.MonitoredServer;
                            serverPingProbe = 
                                (ServerPingProbe) ProbeFactory.BuildServerPingProbe(server, monitoredServerWorkload, server.CloudProviderId);
                            
                            if (IsDisposed)
                                return;
                            
                            serverPingProbe.BeginProbe(
                                new EventHandler<SnapshotCompleteEventArgs>(ServerPingCallback));
                                          
                        }
                        catch (Exception exception)
                        {
                            LOG.WarnFormat("Error starting server ping: {0}", exception.Message);

                            if (serverPingProbe != null)
                            {
                                serverPingProbe.IsCancelled = true;
                                serverPingProbe.Dispose();
                                serverPingProbe = null;
                            }


                        }
                        finally
                        {
                            if (sync.IsReaderLockHeld)
                                sync.ReleaseReaderLock();
                        }
                    }
                }
            }

            private void ServerPingCallback(object sender, SnapshotCompleteEventArgs e)
            {

                using (LOG.DebugCall("ServerPingCallback"))
                {
                    try
                    {
                        using (sender as IDisposable)
                        {
                            if (IsDisposed)
                                return;

                            // This means we cancelled out the probe
                            if (e.Result == Result.Failure)
                                return;

                            lock (syncRoot_ping)
                            {
                                if (serverPingProbe != null)
                                {
                                    serverPingProbe.IsCancelled = true;
                                    serverPingProbe.Dispose();
                                    serverPingProbe = null;
                                }
                            }

                            

                            // Only alert if the server is not in maintenance mode.
                            if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == false)
                            {
                                ScheduledRefresh snapshot = (ScheduledRefresh) e.Snapshot;

                                if (snapshot.Server.SqlServiceStatus != ServiceState.Running)
                                {
                                    // acquire a read lock if we don't already hold one
                                    if (!sync.IsReaderLockHeld)
                                        sync.AcquireReaderLock(-1);
                                    try
                                    {
                                        // generate events


                                        snapshot.TimeStamp = System.DateTime.Now.ToUniversalTime();
                                        snapshot.IsConnectionTestSnapshot = true;

                                        ScheduledCollectionEventProcessor eventProcessor =
                                            new ScheduledCollectionEventProcessor(snapshot, Workload);

                                        eventProcessor.Process();

                                        int numberEvents = ((IEventContainer) e.Snapshot).NumberOfEvents;
                                        if (numberEvents > 0 && LOG.IsVerboseEnabled)
                                        {
                                            foreach (IEvent ievent in ((IEventContainer) e.Snapshot).Events)
                                            {
                                                LOG.VerboseFormat("EventArgs: {0}", ievent);
                                            }
                                        }

                                        LOG.DebugFormat("Added {0} events to the snapshot", numberEvents);

                                    }
                                    catch (Exception ex)
                                    {
                                        LOG.Error("Exception while creating events", ex);
                                    }
                                    finally
                                    {
                                        if (sync.IsReaderLockHeld)
                                            sync.ReleaseReaderLock();
                                    }

                                    //send the snapshot to the management service
                                    lock (syncRoot)
                                    {
                                        if (!disposed)
                                        {
                                            ScheduledCollectionDataMessage message =
                                                new ScheduledCollectionDataMessage(Workload.MonitoredServer, e.Snapshot);

                                            Collection.Scheduled.EnqueueScheduledRefresh(message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        LOG.Error(exception);
                    }
                }
            }


            
          
           

            /// <summary>
            /// Kicks off the normal collection.
            /// </summary>
            /// <param name="state">The state.</param>
            private void CollectNormal(object state)
            {
                if (IsDisposed)
                    return;

                using (LOG.DebugCall("CollectNormal"))
                {
                    if (state is ISnapshotSink)
                    {
                        lock (syncRoot)
                        {
                            collectNowHelper.WaitingRequests.Add((ISnapshotSink)state);

                            // add to list of waiting on-demand requests
                            if (normalCollectionSemaphore.WaitOne(0, false) == false)
                            {
                                // collection already in progress -  see if results have been sent 
                                if (!inScheduledRefreshSnapshotCallback || (inScheduledRefreshSnapshotCallback && !snapshotSent))
                                {
// KVG - this looks wrong           // add the sink so it will get added to the in-progress collection
//       so I commented it out.     waitingRequests.Add((ISnapshotSink)state);
                                    LOG.Debug("scheduled collection already in progress");
                                    return;
                                }
                            }
                        }
                    }
                    else if (normalCollectionSemaphore.WaitOne(0, false) == false)
                    {
                        LOG.Debug(
                            "ScheduledCollectionContext.CollectNormal called but there is currently a refresh running.  Timer to next collection interval.");
                        // a scheduled refresh is currently running so schedule so set the timer to try again later.
                        sync.AcquireReaderLock(-1);
                        Start(Convert.ToInt32(monitoredServerWorkload.NormalCollectionInterval.TotalSeconds));
                        sync.ReleaseReaderLock();
                        return;
                    }

                    // skip this server if it is in maintenance mode
					//SQLDM-29410. 
                    if (ServerInMaintenanceMode(monitoredServerWorkload.MonitoredServer, LOG))
                    {
                        monitoredServerWorkload.MonitoredServer.ActiveClusterNode = null;
                        MonitoredSqlServer server = monitoredServerWorkload.MonitoredServer;
                        ScheduledRefresh refresh = new ScheduledRefresh(server);
                        MonitoredObjectName serverName = new MonitoredObjectName(monitoredServerWorkload.MonitoredServer.InstanceName, String.Empty, String.Empty);
                        ThresholdViolationEvent tve = new ThresholdViolationEvent(serverName, (int)Metric.MaintenanceMode, MonitoredState.Informational, MonitoredState.OK, DateTime.UtcNow, null);
                        StateDeviationEvent sde = new StateDeviationEvent(serverName,               (int) Metric.MaintenanceMode,
                                                                          tve.MonitoredState,       OptionStatus.Enabled,
                                                                          DateTime.UtcNow,          monitoredServerWorkload.MonitoredServer);
                        refresh.AddEvent(sde);
                        SnapshotCompleteEventArgs snapshotComplete = new SnapshotCompleteEventArgs(refresh, Result.Success);
                        ScheduledRefreshSnapshotCallback(ProbeFactory.BuildScheduledRefreshProbe(server, monitoredServerWorkload, server.CloudProviderId), snapshotComplete);
                        PersistenceManager.Instance.SetFailedJobInstanceId(server.InstanceName,0);
                        PersistenceManager.Instance.SetCompletedJobInstanceId(this.monitoredServerWorkload.MonitoredServer.InstanceName, 0);
                        Collection.Scheduled.StopWaitingForActiveWaits(monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                        StopDatabasePeriodicCollector();
                        
                        return;
                    }
                    
                    //TODO: Something else to get the normal collection probes
                    ScheduledRefreshProbe probe = null;
                    try
                    {
                        sync.AcquireReaderLock(-1);

                        MonitoredSqlServer server = monitoredServerWorkload.MonitoredServer;
                        // Tolga K - to fix memory leak begins
                        if (probe != null)
                        {
                            probe.Dispose();
                        }
                        // Tolga K - to fix memory leak ends

                        probe = (ScheduledRefreshProbe)ProbeFactory.BuildScheduledRefreshProbe(server, monitoredServerWorkload, server.CloudProviderId);
                        
                        // object used to synchronize changes to the query monitor configuration
                        qmCollectionState = probe.CreateQueryMonitorCollectionState();
                        activityMonitorCollectionState = probe.CreateActivityMonitorCollectionState();

                        if (IsDisposed)
                            return;

                        LOG.Debug("Staring scheduled refresh");
                        probe.BeginProbe(new EventHandler<SnapshotCompleteEventArgs>(ScheduledRefreshSnapshotCallback));
                    }
                    catch (Exception exception)
                    {
                        LOG.WarnFormat("Error starting server probe: {0}", exception.Message);

                        if (probe != null)
                            probe.Dispose();

                        try
                        {
                            // release the semaphore and dispose of the probe
                            if (!IsDisposed)
                            {
                                normalCollectionSemaphore.Release();
                            }
                        }
                        catch (Exception e)
                        {
                            LOG.WarnFormat("Error releasing scheduled collection semaphore: {0}", exception.Message);
                            return;
                        }
                    }
                    finally
                    {
                        // we should only get here if we are holding a read lock.  under certain cirumstances
                        // though we could have processed the snapshot callback in this thread.  In this situation
                        // the callback would have needed to release the lock in order to function.
                        if (sync.IsReaderLockHeld)
                            sync.ReleaseReaderLock();
                    }
                }
            }

            /// <summary>
            /// Callback called when the Server Refresh probe completes.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="T:Idera.SQLdm.Probes.SnapshotCompleteEventArgs"/> instance containing the event data.</param>
            private void ScheduledRefreshSnapshotCallback(object sender, SnapshotCompleteEventArgs e)
            {
                using (LOG.DebugCall("ScheduledRefreshSnapshotCallback"))
                {
                    inScheduledRefreshSnapshotCallback = true;
                    snapshotSent = false;

                    ScheduledRefreshProbe probe = sender as ScheduledRefreshProbe;
                    if (probe != null)
                    {
                        probe.CompletionCallbackCount += 1;
                        if (probe.CompletionCallbackCount != 1)
                        {
                            LOG.DebugFormat(
                                "Scheduled refresh completion callback entered multiple times for the same refresh {0}",
                                probe.CompletionCallbackCount);
                            return;
                        }

                        probe.Dispose();
                    }

                    try
                    {
                        using (sender as IDisposable)
                        {
                            if (IsDisposed)
                                return;

                            // Only collect history data if the server is not in maintenance mode.
                            if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == false)
                            {
                                CollectCustomCounters((ScheduledRefresh)e.Snapshot);

                                CollectHistoryBrowserData((ScheduledRefresh)e.Snapshot);

                                if (e.Snapshot != null && e.Snapshot.ProductVersion != null &&
                                    e.Snapshot.ProductVersion.Major > 8 &&
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration != null &&
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.Enabled)
                                {
                                    CollectActiveWaits((ScheduledRefresh) e.Snapshot);
                                }
                                else
                                {
                                    PersistenceManager.Instance.ClearAzureQsStartTime(monitoredServerWorkload.MonitoredServer.Id, AzureQsType.ActiveWaits);
                                    Collection.Scheduled.CleanupActiveWaits(monitoredServerWorkload.MonitoredServer.Id);
                                }

                                // Table statistics
                                if (e.Snapshot != null && e.Snapshot.TimeStampLocal != null && monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration != null)
                                {
                                    CollectTableGrowth((ScheduledRefresh) e.Snapshot);
                                }

                                // Table statistics
                                if (e.Snapshot != null && e.Snapshot.TimeStampLocal != null && monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration != null)
                                {
                                    CollectTableFragmentation((ScheduledRefresh)e.Snapshot);
                                }

                                // TODO: MOVE THIS VM STUFF TO THE ServerDetailsCallback on the ScheduledRefreshProbe
                                // VM Information
                                //if (e.Snapshot != null && e.Snapshot.TimeStampLocal != null)
                                //{
                                //    if (monitoredServerWorkload.MonitoredServer.IsVirtualized)
                                //    {
                                //        int iterations = 1;
                                //        if (monitoredServerWorkload.PreviousRefresh != null)
                                //        {
                                //            TimeSpan? timeDelta = e.Snapshot.TimeStamp - monitoredServerWorkload.PreviousRefresh.TimeStamp;
                                //            iterations = timeDelta.HasValue ? timeDelta.Value.Seconds % 20 : 1;
                                //        }
                                //        CollectVMInfo((ScheduledRefresh)e.Snapshot, iterations);
                                //    }
                                //}

                                // acquire a read lock if we don't already hold one
                                if (!sync.IsReaderLockHeld)
                                    sync.AcquireReaderLock(-1);
                                try
                                {   // generate events
                                    ScheduledCollectionEventProcessor eventProcessor =
                                        new ScheduledCollectionEventProcessor((ScheduledRefresh) e.Snapshot, Workload);
                                 
                                    eventProcessor.Process();

                                    int numberEvents = ((IEventContainer) e.Snapshot).NumberOfEvents;
                                    if (numberEvents > 0 && LOG.IsVerboseEnabled)
                                    {
                                        foreach (IEvent ievent in ((IEventContainer)e.Snapshot).Events)
                                        {
                                            LOG.VerboseFormat("EventArgs: {0}", ievent);
                                        }
                                    }
                                    //SQLdm 10.0 (Gaurav Karwal) : Small Features : Updating '# Alerts Raised' counter
                                    Statistics.SetAlertsRaised(numberEvents, monitoredServerWorkload.Name,true);

                                    LOG.DebugFormat("Added {0} events to the snapshot", numberEvents);
                                }
                                catch (Exception ex)
                                {
                                    LOG.Error("Exception while creating events", ex);
                                }
                                finally
                                {
                                    if (sync.IsReaderLockHeld)
                                        sync.ReleaseReaderLock();
                                }


                                sync.AcquireWriterLock(-1);

                                monitoredServerWorkload.PreviousRefresh = e.Snapshot;

                                monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime =
                                    ((ScheduledRefresh)e.Snapshot).MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime;
                                monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime =
                                    ((ScheduledRefresh) e.Snapshot).MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime;
                                if (((ScheduledRefresh)e.Snapshot).Server.ClusterNodeName != null)
                                {
                                    monitoredServerWorkload.MonitoredServer.ActiveClusterNode =
                                        ((ScheduledRefresh) e.Snapshot).Server.ClusterNodeName;
                                }

                                // Reconfigure query monitor if necessary
                                if (monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.StopTimeUTC.HasValue &&
                                    monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.StopTimeUTC.Value <=
                                    e.Snapshot.TimeStamp)
                                {
                                    ((ScheduledRefresh) e.Snapshot).RequestReconfiguration = true;
                                }


                                sync.ReleaseWriterLock();
                            }
                            else
                            {
                                // Clear out old refreshes
                                sync.AcquireWriterLock(-1);

                                monitoredServerWorkload.DropQueryDataFromPreviousRefresh(); // Drop query data to clear space
                                monitoredServerWorkload.PreviousRefresh = null;

                                sync.ReleaseWriterLock(); 
                            }

                            //send the snapshot to the management service
                            lock (syncRoot)
                            {
                                if (!disposed)
                                {
                                    ScheduledCollectionDataMessage message =
                                        new ScheduledCollectionDataMessage(Workload.MonitoredServer, e.Snapshot);

                                    collectNowHelper.AddSink(message);

                                    Collection.Scheduled.EnqueueScheduledRefresh(message);
//                                    PersistenceManager pm = PersistenceManager.Instance;
//                                    pm.EnqueueScheduledCollectionDataMessage(pm.WrapMessage(message));
                                    snapshotSent = true;
                                    monitoredServerWorkload.DropQueryDataFromPreviousRefresh(); // Drop query data to clear space
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        LOG.Error(exception);
                    }
                    finally
                    {
                        // Normal collection is done for this server.
                        // Let new normal collections start.
                        lock (syncRoot)
                        {
                            // restart the timer to kick off the next collection cycle
                            sync.AcquireReaderLock(-1);

                            if (collectNowHelper.WaitingRequests.Count > 0 && collectNowHelper.WaitingFor.Contains(typeof(ScheduledRefresh)))
                                Start(0);
                            else
                                Start((int)this.monitoredServerWorkload.NormalCollectionInterval.TotalSeconds);

                            sync.ReleaseReaderLock();
                            inScheduledRefreshSnapshotCallback = false;
                            if (!disposed)
                                normalCollectionSemaphore.Release();
                        }
                    }
                }
            }

          
            internal void CollectHistoryBrowserData(ScheduledRefresh refresh)
            {
                using (LOG.DebugCall("CollectHistoryBrowserData"))
                {
                    if (refresh.MonitoredServer.CloudProviderId==null && refresh.CollectionFailed)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: If sql server is hosted on cloud,then no need to check collection failed check
                    {
                        LOG.Info("Skipping history browser collection because scheduled collection failed");
                        return;
                    }

                    if (refresh.MonitoredServer.ExtendedHistoryCollectionDisabled)
                    {
                        LOG.Info("Skipping history browser collection because it has been disabled");
                        return;
                    }

                    try
                    {
                        using (
                            OnDemandCollectionContext<SessionSnapshot> sessionListContext =
                                new OnDemandCollectionContext<SessionSnapshot>(TimeSpan.FromSeconds(CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlCommandTimeout)))
                        {
                            Collection.OnDemand.CollectSessions(
                                monitoredServerWorkload.HistoryBrowserSessionsConfiguration, sessionListContext, null);
                            refresh.SessionList = sessionListContext.Wait();
                        }


                    }
                    catch (Exception ex)
                    {
                        LOG.Error("Exception collecting Session List for scheduled collection.  Exiting history browser collection.", ex);
                        return;
                    }

                    try
                    {
                        using (
                            OnDemandCollectionContext<LockDetails> lockDetailsContext =
                                new OnDemandCollectionContext<LockDetails>(TimeSpan.FromSeconds(CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlCommandTimeout)))
                        {
                            Collection.OnDemand.CollectLockDetails(
                                monitoredServerWorkload.HistoryBrowserLockDetailsConfiguration, lockDetailsContext, null);
                            refresh.LockList = lockDetailsContext.Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("Exception collecting Lock List for scheduled collection", ex);
                    }
                }
            }

            internal void CollectActiveWaits(ScheduledRefresh refresh)
            {
                using (LOG.DebugCall("CollectActiveWaits"))
                {
                    try
                    {
                        // Update the configuration with the server's local timestamp in case it has changed, and set the pickup interval to twice the refresh interval
                        if (monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration != null && refresh.TimeStampLocal.HasValue && refresh.TimeStamp.HasValue)
                        {
                            monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.UpdateStartTime(refresh.TimeStampLocal.Value, refresh.TimeStamp.Value);
                            monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.WaitForPickupTime =
                                TimeSpan.FromMinutes(monitoredServerWorkload.NormalCollectionInterval.Minutes*2);
                        }
                        // Try to pick up active waits data even if the window has closed, in case the collection window ended between refreshes
                        if (monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration != null
                                        && (monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.ReadyForCollection
                                            ||
                                            monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.InPickupWindow))
                        {
                            //Flag this as a master. The filter criteria cannot be ignored in the combine rules.
                            monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.IsMaster = true;
                            refresh.ActiveWaits =
                                Collection.Scheduled.GetActiveWaits(
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                            // If the configuration is not ready for collection it means we're doing the final pickup before shutdown
                            // so go ahead and remove the waiter in case we're the last one waiting
                            if (!monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.ReadyForCollection)
                            {
                                Collection.Scheduled.StopWaitingForActiveWaits(
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                                Collection.Scheduled.CleanupActiveWaits(monitoredServerWorkload.MonitoredServer.Id);
                            }
                        }
                        else
                        {
                            if (monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration != null &&
                                !monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration.ReadyForCollection)
                                Collection.Scheduled.StopWaitingForActiveWaits(monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration);
                            Collection.Scheduled.CleanupActiveWaits(monitoredServerWorkload.MonitoredServer.Id);
                        }

                        
                    }
                    catch (Exception e)
                    {
                        
                        LOG.Error("Error collecting active waits.",e);
                    }
                }
            }

            internal void CollectTableGrowth(ScheduledRefresh refresh)
            {
                using (LOG.DebugCall("CollectTableGrowth"))
                {
                    try
                    {
                        // Update the configuration with the server's local timestamp in case it has changed, and set the pickup interval to twice the refresh interval
                        if (monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration != null && refresh.TimeStampLocal.HasValue && refresh.TimeStamp.HasValue)
                        {
                            monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.TimeStampLocal = refresh.TimeStampLocal.Value;
                            monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.WaitForPickupTime =
                                TimeSpan.FromMinutes(monitoredServerWorkload.NormalCollectionInterval.Minutes * 3);
                        }
                        // Try to pick up data even if the window has closed, in case the collection window ended between refreshes
                        if (monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration != null
                                        && (monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.ReadyForCollection
                                            ||
                                            monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.InPickupWindow))
                        {
                            TableGrowthSnapshot tableGrowth =
                                Collection.Scheduled.GetTableGrowth(
                                    monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration);
                            foreach (DatabaseStatistics dbstats in tableGrowth.DbStatistics.Values)
                            {
                                if (!refresh.DbStatistics.ContainsKey(dbstats.Name))
                                {
                                    refresh.DbStatistics.Add(dbstats.Name,dbstats);
                                }
                                else
                                {
                                    refresh.DbStatistics[dbstats.Name].TableSizes = new Dictionary<string, TableSize>(dbstats.TableSizes.Count);
                                    foreach(TableSize tableSize in dbstats.TableSizes.Values)
                                    {
                                        refresh.DbStatistics[dbstats.Name].TableSizes.Add(tableSize.Schema + "." + tableSize.Name, (TableSize)tableSize.Clone());
                                    }
                                }
                            }
                            // If the configuration is not ready for collection it means we're doing the final pickup before shutdown
                            // so go ahead and remove the waiter in case we're the last one waiting
                            if (!monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.ReadyForCollection)
                            {
                                Collection.Scheduled.StopWaitingForTableGrowth(
                                    monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration);
                                Collection.Scheduled.CleanupTableGrowth(monitoredServerWorkload.MonitoredServer.Id);
                            }
                        }
                        else
                        {
                            if (monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration != null &&
                                !monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration.ReadyForCollection)
                                Collection.Scheduled.StopWaitingForTableGrowth(monitoredServerWorkload.MonitoredServer.TableGrowthConfiguration);
                            Collection.Scheduled.CleanupTableGrowth(monitoredServerWorkload.MonitoredServer.Id);
                        }


                    }
                    catch (Exception e)
                    {

                        LOG.Error("Error collecting table growth.", e);
                    }
                }
            }

            internal void CollectTableFragmentation(ScheduledRefresh refresh)
            {
                using (LOG.DebugCall("CollectTableFragmentation"))
                {
                    try
                    {
                        // Update the configuration with the server's local timestamp in case it has changed, and set the pickup interval to twice the refresh interval
                        if (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration != null && refresh.TimeStampLocal.HasValue && refresh.TimeStamp.HasValue)
                        {
                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.TimeStampLocal = refresh.TimeStampLocal.Value;
                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.WaitForPickupTime =
                                TimeSpan.FromMinutes(monitoredServerWorkload.NormalCollectionInterval.Minutes * 3);
                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.ForceFinish = false;
                        }
                        // Try to pick up data even if the window has closed, in case the collection window ended between refreshes
                        if (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration != null
                                        && (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.ReadyForCollection
                                            ||
                                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.InPickupWindow))
                        {
                           //need to clean the desviation map to update the alerts every time we runs table fragmentation 
                            if (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.ReadyForCollection)
                            {
                                monitoredServerWorkload.StateGraph.ClearAllEvents(new int[] { (int)Metric.ReorganisationPct }, null);
                            }
                            

                            TableFragmentationSnapshot tableFragmentation =
                                Collection.Scheduled.GetTableFragmentation(
                                    monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration);
                            foreach (DatabaseStatistics dbstats in tableFragmentation.DbStatistics.Values)
                            {
                                if (!refresh.DbStatistics.ContainsKey(dbstats.Name))
                                {
                                    refresh.DbStatistics.Add(dbstats.Name, dbstats);
                                }
                                else
                                {
                                    refresh.DbStatistics[dbstats.Name].TableReorganizations = new Dictionary<string, TableReorganization>(dbstats.TableReorganizations.Count);
                                    foreach (TableReorganization tableFrag in dbstats.TableReorganizations.Values)
                                    {
                                        refresh.DbStatistics[dbstats.Name].TableReorganizations.Add(tableFrag.Name, (TableReorganization)tableFrag.Clone());
                                    }
                                }
                            }
                            
                            // If the configuration is not ready for collection it means we're doing the final pickup before shutdown
                            // so go ahead and remove the waiter in case we're the last one waiting
                            if (!monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.ReadyForCollection)
                            {
                                Collection.Scheduled.StopWaitingForTableFragmentation(
                                    monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration);
                                Collection.Scheduled.CleanupTableFragmentation(monitoredServerWorkload.MonitoredServer.Id);

                                // On first stop attempt set to "Stopping" to give time for alerting cleanup
                                monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus
                                    = TableFragmentationCollectorStatus.Stopping;

                            }
                        }
                        else
                        {
                            if (monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration != null &&
                                !monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.ReadyForCollection)
                                Collection.Scheduled.StopWaitingForTableFragmentation(monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration);
                            Collection.Scheduled.CleanupTableFragmentation(monitoredServerWorkload.MonitoredServer.Id);

                            // On first stop attempt set to "Stopping" to give time for alerting cleanup
                            // Subsequently set to Stopped
                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus =
                                monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus >
                                TableFragmentationCollectorStatus.Stopped
                                    ? TableFragmentationCollectorStatus.Stopping
                                    : TableFragmentationCollectorStatus.Stopped;
                        }


                    }
                    catch (Exception e)
                    {

                        LOG.Error("Error collecting table fragmentation.", e);
                    }
                    finally
                    {
                        refresh.TableFragmentationStatus =
                            monitoredServerWorkload.MonitoredServer.TableFragmentationConfiguration.CollectorStatus;
                    }
                }
            }

            internal void CollectCustomCounters(ScheduledRefresh refresh)
            {
                using (LOG.DebugCall("CollectCustomCounters"))
                {
                    if (refresh.CollectionFailed)
                    {
                        LOG.Info("Skipping custom counter collection because scheduled collection failed");
                        return;
                    }

                    Set<int> customCounterIds = GetCustomCounters();
                    if (customCounterIds.Count == 0)
                    {
                        LOG.Info("No custom counters assigned/tagged for collection");
                        return;
                    }

                    try
                    {
                        //SQLDM-29410
                        if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == false) {
                            ScheduledRefresh prev = ((ScheduledRefresh)monitoredServerWorkload.PreviousRefresh);
                            List<CustomCounterConfiguration> configurations = new List<CustomCounterConfiguration>();

                            foreach (int counterID in customCounterIds)
                            {
                                CustomCounterDefinition counterDefinition = Collection.GetCustomCounter(counterID);
                                if (counterDefinition == null || !counterDefinition.IsEnabled)
                                    continue;

                                refresh.AlertableMetrics.Add((Metric)counterDefinition.MetricID);

                                if (prev != null
                                    && prev.CustomCounters.ContainsKey(counterID)
                                    && prev.CustomCounters[counterID] != null
                                    && !prev.CustomCounters[counterID].CollectionFailed
                                    &&
                                    prev.CustomCounters[counterID].Definition.EqualsMinusEnabled(counterDefinition))
                                {   // There was a successful previous collection for this counter

                                    // update scale in the previous snapshot if it is different
                                    CustomCounterSnapshot previousSnapshot = prev.CustomCounters[counterID];
                                    if (previousSnapshot.Definition.Scale != counterDefinition.Scale)
                                        previousSnapshot.Definition.Scale = counterDefinition.Scale;

                                    configurations.Add(
                                        new CustomCounterConfiguration(
                                            monitoredServerWorkload.MonitoredServer.Id,
                                            prev.CustomCounters[counterID]));
                                }
                                else
                                {
                                    // No previous collection, or the definition has changed 
                                    configurations.Add(
                                        new CustomCounterConfiguration(
                                            monitoredServerWorkload.MonitoredServer.Id,
                                            counterDefinition));
                                }
                            }

                            using (
                                OnDemandCollectionContext<CustomCounterCollectionSnapshot> customCounterContext =
                                    new OnDemandCollectionContext<CustomCounterCollectionSnapshot>(monitoredServerWorkload.MonitoredServer.CustomCounterTimeout.Add(TimeSpan.FromSeconds(SqlHelper.CommandTimeout))))
                            {
                                Collection.OnDemand.CollectCustomCounter(
                                               configurations,
                                               customCounterContext, null);
                                CustomCounterCollectionSnapshot counters = customCounterContext.Wait();
                                refresh.CustomCounters = counters.CustomCounterList;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("Unrecoverable exception collecting custom counters for scheduled collection.  Exiting custom counter collection.", ex);
                    }
                }
            }

            internal bool CollectNow(ISnapshotSink sink)
            {
                if (!collectNowHelper.WaitingFor.Contains(typeof(ScheduledRefresh)))
                    collectNowHelper.WaitingFor.Add(typeof (ScheduledRefresh));
                if (!collectNowHelper.WaitingFor.Contains(typeof(DatabaseSizeSnapshot)))
                    collectNowHelper.WaitingFor.Add(typeof(DatabaseSizeSnapshot));
                CollectNormal(sink);
                StartDatabasePeriodicCollector(0,sink);
                return true;
            }
         
            private bool StartQueryMonitorTrace()
            {   
                // *** Do not call this without holding the context writer lock
                using (LOG.InfoCall("StartQueryMonitorTrace"))
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support for Active Waits
                    if (qmCollectionState != null)
                    {
                        lock (qmCollectionState)
                        {
                            if (qmCollectionState.IsQMAlreadyEnabled)
                            {
                                LOG.Info("Query monitor is already enabled");
                                return true;
                            }
                            //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - Added extended event session collection as well
                            if (monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.TraceMonitoringEnabled) 
                            Collection.OnDemand.SendStartQueryMonitorTrace(
                                monitoredServerWorkload.Id,
                                monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration,
                                qmCollectionState.QueryMonitorCollectorStarted
                                    ? qmCollectionState.ProbeQMConfig
                                    : qmCollectionState.ProbePreviousQMConfig,
                                monitoredServerWorkload.MonitoredServer.CloudProviderId);
                            else if (!monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.QueryStoreMonitoringEnabled)
                            {
                                Collection.OnDemand.SendStartQueryMonitorExtendedEventSession(
                                    monitoredServerWorkload.Id,
                                    monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration,
                                    qmCollectionState.QueryMonitorCollectorStarted
                                        ? qmCollectionState.ProbeQMConfig
                                        : qmCollectionState.ProbePreviousQMConfig,
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration,
                                    monitoredServerWorkload.MonitoredServer.CloudProviderId);
                            }
                            // Note: Requires code when Query Store is configurable
                        }
                    }
                    else
                    {
						//SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - Added extended event session collection as well
                        if (monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.TraceMonitoringEnabled) 
                        Collection.OnDemand.SendStartQueryMonitorTrace(
                            monitoredServerWorkload.Id,
                            monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration,
                            PersistenceManager.Instance.GetQueryMonitorConfiguration(
                                monitoredServerWorkload.MonitoredServer.InstanceName),
                            monitoredServerWorkload.MonitoredServer.CloudProviderId
                            );
                        else if (!monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration.QueryStoreMonitoringEnabled)
                        {
                            Collection.OnDemand.SendStartQueryMonitorExtendedEventSession(
                            monitoredServerWorkload.Id,
                            monitoredServerWorkload.MonitoredServer.QueryMonitorConfiguration,
                            PersistenceManager.Instance.GetQueryMonitorConfiguration(
                                monitoredServerWorkload.MonitoredServer.InstanceName),
                                    monitoredServerWorkload.MonitoredServer.ActiveWaitsConfiguration,
                                    monitoredServerWorkload.MonitoredServer.CloudProviderId
                            );
                        }
                        // Note: Requires code when Query Store is configurable 
                    }
                    return true;
                }
            }
            
            public void StopActivityMonitorTrace()
            {
                Collection.OnDemand.SendStopActivityMonitorTrace(
                    new StopActivityMonitorTraceConfiguration(monitoredServerWorkload.Id), null, null);
            }

            public void StopQueryMonitorTrace()
            {
                Collection.OnDemand.SendStopQueryMonitorTrace(
                    new StopQueryMonitorTraceConfiguration(monitoredServerWorkload.Id), null, null);
            }

            private bool StartActivityMonitorTrace()
            {
                // *** Do not call this without holding the context writer lock
                using (LOG.InfoCall("StartActivityMonitorTrace"))
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor
                    if (activityMonitorCollectionState != null)
                    {
                        lock (activityMonitorCollectionState)
                        {
                            if (activityMonitorCollectionState.IsAMAlreadyEnabled)
                            {
                                LOG.Info("ActivityMonitor is already enabled");
                                return true;
                            }
                            Collection.OnDemand.SendStartActivityMonitorTrace(
                                monitoredServerWorkload.Id,
                                monitoredServerWorkload.MonitoredServer.ActivityMonitorConfiguration,
                                activityMonitorCollectionState.ActivityMonitorCollectorStarted
                                    ? activityMonitorCollectionState.ProbeAMConfig
                                    : activityMonitorCollectionState.ProbePreviousAMConfig,
                                monitoredServerWorkload.MonitoredServer.CloudProviderId);
                        }
                    }
                    else
                    {
                        Collection.OnDemand.SendStartActivityMonitorTrace(
                            monitoredServerWorkload.Id,
                            monitoredServerWorkload.MonitoredServer.ActivityMonitorConfiguration,
                            PersistenceManager.Instance.GetActivityMonitorConfiguration(
                                monitoredServerWorkload.MonitoredServer.InstanceName),
                            monitoredServerWorkload.MonitoredServer.CloudProviderId
                            );
                    }
                    return true;
                }
            }

            internal Set<int> GetCustomCounters()
            {
                // get statically linked counter list
                Set<int> counters = new Set<int>(monitoredServerWorkload.CustomCounters);

                // get all the tags for the server
                Set<int> serverTags = Workload.ServerTags;
                if (serverTags.Count > 0)
                {
                    // for each defined custom counter
                    foreach (int metricId in Collection.GetCustomCounterKeySet())
                    {
                        // is counter already in the list
                        if (counters.Contains(metricId))
                            continue;
                        // get tags for the custom counter
                        Set<int> counterTags = Collection.GetCustomCounterTags(metricId);
                        if (counterTags.Count > 0)
                        {
                            // do any of the items in server & counter tags match
                            if (!Algorithms.DisjointSets(serverTags, counterTags))
                                counters.Add(metricId);
                        }
                    }        
                }

                return counters;
            }

            internal void RemoveCustomCounter(int metricId)
            {
                List<int> counters = monitoredServerWorkload.CustomCounters;
                lock (counters)
                {
                    if (counters.Contains(metricId))
                        counters.Remove(metricId);
                }
            }

            internal void AddCustomCounter(int metricId)
            {
                List<int> counters = monitoredServerWorkload.CustomCounters;
                lock (counters)
                {
                    if (!counters.Contains(metricId))
                        counters.Add(metricId);
                }
            }

            internal void RemoveServerTag(int tagId)
            {
                Workload.RemoveServerTag(tagId);
            }

            internal void RemoveServerTags(IEnumerable<int> tagIds)
            {
                Workload.RemoveServerTags(tagIds);
            }

            internal void AddServerTag(int tagId)
            {
                Workload.AddServerTag(tagId);
            }

            /// <summary>
            /// Save the mirroring preferred role for the guid
            /// </summary>
            /// <param name="guid"></param>
            /// <param name="preferredConfig"></param>
            internal void UpdateMirroringPreferredConfig(Guid guid, ServerPreferredMirrorConfig preferredConfig)
            {
                Workload.UpdateMirroringSessions(guid, preferredConfig);
            }

            internal void ClearEventState(int metricId, DateTime? cutoffTime, MonitoredObjectName objectName)
            {
                sync.AcquireWriterLock(-1);
                try
                {
                    if (objectName == null)
                    {   // clear all
                        monitoredServerWorkload.StateGraph.ClearAllEvents(new int[] {metricId}, cutoffTime);
                    }
                    else
                    {
                        MonitoredObjectStateGraph stateGraph = monitoredServerWorkload.StateGraph;
                        MetricState state = stateGraph.GetEvent(objectName, metricId);
                        if (cutoffTime == null || cutoffTime.Value >= state.StartTime) 
                            monitoredServerWorkload.StateGraph.ClearEvent(state);
                    }
                }
                finally
                {
                    sync.ReleaseWriterLock();
                }
            }

            internal static bool ServerInMaintenanceMode(MonitoredSqlServer monitoredServer, Logger LOG)
            {
                MaintenanceMode mmMode = monitoredServer.MaintenanceMode;
                MaintenanceModeType mmType = mmMode.MaintenanceModeType;
                IManagementService mgmtSvc = null;
                MonitoredSqlServerConfiguration config = new MonitoredSqlServerConfiguration();
                bool inMaintenanceMode = false;

                try
                {
                    if (mmType != MaintenanceModeType.Never)
                    {
                        // the server is in maintenance mode.
                        if (mmMode.MaintenanceModeOnDemand.HasValue && mmMode.MaintenanceModeOnDemand.Value)
                        {
                            LOG.InfoFormat("Server {0} is in Maintenance Mode OnDemand: {1}", monitoredServer.InstanceName, mmType);
                            return true;
                        }

                        switch (mmType)
                        {
                            case MaintenanceModeType.Once:
                                {
                                    if (monitoredServer.MaintenanceModeEnabled)
                                    {
                                        // see if it is time to remove the server from maintenance mode
                                        if (DateTime.Now >= mmMode.MaintenanceModeStop)
                                        {
                                            mgmtSvc = RemotingHelper.GetObject<IManagementService>();

                                            monitoredServer.MaintenanceModeEnabled = false;
                                            monitoredServer.MaintenanceMode.MaintenanceModeType
                                                = MaintenanceModeType.Never;
                                            monitoredServer =
                                                mgmtSvc.UpdateMonitoredSqlServer(
                                                    monitoredServer.Id,
                                                    monitoredServer.GetConfiguration());
                                            LOG.InfoFormat("Removing server {0} from maintenance mode.",
                                                           monitoredServer.InstanceName);

                                            //Creates an AuditableEntity and log it to the AuditingEngine
                                            var entity = GetMaintenanceModeEntity(monitoredServer);
                                            entity.AddMetadataProperty("Maintenance Mode status", "Maintenance Mode stopped");
                                            AuditingEngine.Instance.ManagementService = mgmtSvc;
                                            AuditingEngine.Instance.SQLUser =
                                                mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication
                                                ? AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                            AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStop);
                                        }
                                        else
                                        {
                                            // the server is still in maintenance mode.
                                            LOG.InfoFormat("Server {0} is in Maintenance Mode: {1}",
                                                           monitoredServer.InstanceName, mmType);
                                            inMaintenanceMode = true;
                                        }
                                    }
                                    else
                                    {
                                        // see if it is time to put the server in maintenance mode
                                        if (DateTime.Now >= mmMode.MaintenanceModeStart)
                                        {
                                            //put server in maintenance mode
                                            monitoredServer.MaintenanceModeEnabled = true;
                                            mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                            monitoredServer =
                                                mgmtSvc.UpdateMonitoredSqlServer(
                                                    monitoredServer.Id,
                                                    monitoredServer.GetConfiguration());
                                            LOG.InfoFormat("Putting server {0} into maintenace mode.",
                                                           monitoredServer.InstanceName);
                                            inMaintenanceMode = true;

                                            //Creates an AuditableEntity and log it to the AuditingEngine
                                            var entity = GetMaintenanceModeEntity(monitoredServer);
                                            AuditingEngine.Instance.ManagementService = mgmtSvc;
                                            AuditingEngine.Instance.SQLUser = 
                                                mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication ?
                                                AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                            AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStart);
                                        }
                                    }
                                    break;
                                }
                            case MaintenanceModeType.Recurring:
                                {
                                    if (monitoredServer.MaintenanceModeEnabled)
                                    {
                                        // The date part of the date time object is not set for the start time. I added the current date part for an accurrate comparison
                                        DateTime mmModeStart = new DateTime(1900, 1, 1,
                                                                            mmMode.MaintenanceModeRecurringStart.Value.Hour,
                                                                            mmMode.MaintenanceModeRecurringStart.Value.Minute,
                                                                            mmMode.MaintenanceModeRecurringStart.Value.Second);
                                        DateTime endOfDay;

                                        if (DateTime.Now.TimeOfDay > mmModeStart.TimeOfDay)
                                        {
                                            if ((DateTime.Now.TimeOfDay - mmModeStart.TimeOfDay) >=
                                                mmMode.MaintenanceModeDuration)
                                            {
                                                inMaintenanceMode = false;
                                            }
                                            else
                                            {
                                                inMaintenanceMode = true;
                                            }
                                        }
                                        else
                                        {
                                            endOfDay = new DateTime(1900, 1, 1, 23, 59, 59, 999);
                                            if (((endOfDay.TimeOfDay - mmModeStart.TimeOfDay) + DateTime.Now.TimeOfDay) >=
                                                mmMode.MaintenanceModeDuration)
                                            {
                                                inMaintenanceMode = false;
                                            }
                                            else
                                            {
                                                inMaintenanceMode = true;
                                            }
                                        }

                                        if (inMaintenanceMode)
                                        {
                                            // the server is still in maintenance mode.
                                            LOG.InfoFormat("Server {0} is in Maintenance Mode: {1}",
                                                           monitoredServer.InstanceName, mmType);
                                        }
                                        else
                                        {
                                            //remove the server from maintenance mode 
                                            monitoredServer.MaintenanceModeEnabled = false;
                                            mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                            monitoredServer =
                                                mgmtSvc.UpdateMonitoredSqlServer(
                                                    monitoredServer.Id,
                                                    monitoredServer.GetConfiguration());
                                            LOG.InfoFormat("Removing server {0} from maintenance mode",
                                                           monitoredServer.InstanceName);

                                            //Creates an AuditableEntity and log it to the AuditingEngine
                                            var entity = GetMaintenanceModeEntity(monitoredServer);
                                            entity.AddMetadataProperty("Maintenance Mode status", "Maintenance Mode stopped");
                                            AuditingEngine.Instance.ManagementService = mgmtSvc;
                                            AuditingEngine.Instance.SQLUser =
                                                mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication ?
                                                AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                            AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStop);
                                        }
                                    }
                                    else
                                    {
                                        bool mmModeForToday = false;

                                        //See if today is of of the day for recurring maintenance mode.
                                        foreach (int val in Enum.GetValues(typeof (DayOfWeek)))
                                        {
                                            if (MonitoredSqlServer.MatchDayOfWeek((DayOfWeek) val,
                                                                                  mmMode.MaintenanceModeDays))
                                            {
                                                if (val == (int) DateTime.Now.DayOfWeek)
                                                {
                                                    mmModeForToday = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (mmModeForToday)
                                        {
                                            // see if it is time to put the server in maintenance mode
                                            if ((DateTime.Now.TimeOfDay >=
                                                 mmMode.MaintenanceModeRecurringStart.Value.TimeOfDay) &&
                                                (DateTime.Now.TimeOfDay <
                                                 (mmMode.MaintenanceModeRecurringStart.Value.TimeOfDay +
                                                  mmMode.MaintenanceModeDuration)))
                                            {
                                                //put server in maintenance mode
                                                monitoredServer.MaintenanceModeEnabled = true;
                                                mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                                monitoredServer =
                                                    mgmtSvc.UpdateMonitoredSqlServer(
                                                        monitoredServer.Id,
                                                        monitoredServer.GetConfiguration());
                                                LOG.InfoFormat("Putting server {0} into maintenace mode",
                                                               monitoredServer.InstanceName);
                                                inMaintenanceMode = true;

                                                //Creates an AuditableEntity and log it to the AuditingEngine
                                                var entity = GetMaintenanceModeEntity(monitoredServer);
                                                AuditingEngine.Instance.ManagementService = mgmtSvc;
                                                AuditingEngine.Instance.SQLUser =
                                                    mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication ?
                                                    AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                                AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStart);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case MaintenanceModeType.Monthly:
                                {
                                    if (monitoredServer.MaintenanceModeEnabled)
                                    {
                                        // The date part of the date time object is not set for the start time. I added the current date part for an accurrate comparison
                                        DateTime mmModeStart = new DateTime(1900, 1, 1,
                                                                            mmMode.MaintenanceModeMonthRecurringStart.Value.Hour,
                                                                            mmMode.MaintenanceModeMonthRecurringStart.Value.Minute,
                                                                            mmMode.MaintenanceModeMonthRecurringStart.Value.Second);
                                        DateTime endOfDay;

                                        if (DateTime.Now.TimeOfDay > mmModeStart.TimeOfDay)
                                        {
                                            if ((DateTime.Now.TimeOfDay - mmModeStart.TimeOfDay) >=
                                                mmMode.MaintenanceModeMonthDuration)
                                            {
                                                inMaintenanceMode = false;
                                            }
                                            else
                                            {
                                                inMaintenanceMode = true;
                                            }
                                        }
                                        else
                                        {
                                            endOfDay = new DateTime(1900, 1, 1, 23, 59, 59, 999);
                                            if (((endOfDay.TimeOfDay - mmModeStart.TimeOfDay) + DateTime.Now.TimeOfDay) >=
                                                mmMode.MaintenanceModeMonthDuration)
                                            {
                                                inMaintenanceMode = false;
                                            }
                                            else
                                            {
                                                inMaintenanceMode = true;
                                            }
                                        }

                                        if (inMaintenanceMode)
                                        {
                                            // the server is still in maintenance mode.
                                            LOG.InfoFormat("Server {0} is in Maintenance Mode: {1}",
                                                           monitoredServer.InstanceName, mmType);
                                        }
                                        else
                                        {
                                            //remove the server from maintenance mode 
                                            monitoredServer.MaintenanceModeEnabled = false;
                                            mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                            monitoredServer =
                                                mgmtSvc.UpdateMonitoredSqlServer(
                                                    monitoredServer.Id,
                                                    monitoredServer.GetConfiguration());
                                            LOG.InfoFormat("Removing server {0} from maintenance mode",
                                                           monitoredServer.InstanceName);

                                            //Creates an AuditableEntity and log it to the AuditingEngine
                                            var entity = GetMaintenanceModeEntity(monitoredServer);
                                            entity.AddMetadataProperty("Maintenance Mode status", "Maintenance Mode stopped");
                                            AuditingEngine.Instance.ManagementService = mgmtSvc;
                                            AuditingEngine.Instance.SQLUser =
                                                mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication ?
                                                AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                            AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStop);
                                        }
                                    }
                                    else
                                    {
                                        bool mmModeForToday = false;

                                        short mmMonth = (short)mmMode.MaintenanceModeMonth;
                                        short maintenanceModeSpecificDay = (short)mmMode.MaintenanceModeSpecificDay;
                                        short mmWeekOrdinal = (short)mmMode.MaintenanceModeWeekOrdinal;
                                        short mmWeekDay = (short)mmMode.MaintenanceModeWeekDay;

                                        DateTime dt = DateTime.Now;
                                        int currentMonth = dt.Month;
                                        int currentDay = dt.Day;
                                        int selectedWeek = currentDay / 7;
                                        int remForselectedWeek = currentDay % 7;
                                        if (remForselectedWeek > 0)
                                            selectedWeek = selectedWeek + 1;

                                        // if the current month is devisible by mmMonth, then we can say that Maintenance is scheduled for current month.
                                        if (currentMonth % mmMonth == 0)
                                        {
                                            if (maintenanceModeSpecificDay > 0)
                                            {
                                                if (currentDay.ToString().Equals(maintenanceModeSpecificDay.ToString()))
                                                {
                                                    mmModeForToday = true;
                                                }
                                            }
                                            else if (mmWeekOrdinal > 0 && mmWeekDay > 0)
                                            {
                                                if ((selectedWeek == mmWeekOrdinal) && ((short)dt.DayOfWeek == (mmWeekDay - 1)))
                                                {
                                                    mmModeForToday = true;
                                                }
                                            }
                                        }

                                        if (mmModeForToday)
                                        {
                                            // see if it is time to put the server in maintenance mode
                                            if ((DateTime.Now.TimeOfDay >=
                                                 mmMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay) &&
                                                (DateTime.Now.TimeOfDay <
                                                 (mmMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay +
                                                  mmMode.MaintenanceModeMonthDuration)))
                                            {
                                                //put server in maintenance mode
                                                monitoredServer.MaintenanceModeEnabled = true;
                                                mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                                                monitoredServer =
                                                    mgmtSvc.UpdateMonitoredSqlServer(
                                                        monitoredServer.Id,
                                                        monitoredServer.GetConfiguration());
                                                LOG.InfoFormat("Putting server {0} into maintenace mode",
                                                               monitoredServer.InstanceName);
                                                inMaintenanceMode = true;

                                                //Creates an AuditableEntity and log it to the AuditingEngine
                                                var entity = GetMaintenanceModeEntity(monitoredServer);
                                                AuditingEngine.Instance.ManagementService = mgmtSvc;
                                                AuditingEngine.Instance.SQLUser =
                                                    mgmtSvc.GetManagementServiceConfiguration().WindowsAuthentication ?
                                                    AuditingEngine.GetWorkstationUser() : mgmtSvc.GetManagementServiceConfiguration().RepositoryUsername;
                                                AuditingEngine.Instance.LogAction(entity, AuditableActionType.MaintenanceModeScheduledStart);
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    LOG.InfoFormat("Server {0} is in Maintenance Mode: {1}",
                                                   monitoredServer.InstanceName, mmType);
                                    inMaintenanceMode = true;
                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error determining if server is in maintenance mode.",e);
                }
                return (inMaintenanceMode);
            }

            /// <summary>
            /// This method creates a new AuditableEntity that contains Changed Maintenance Mode to and Event type in its metadata
            /// </summary>
            /// <param name="monitoredServer">The MonitoredSqlServer that is selected now</param>
            /// <returns></returns>
            private static AuditableEntity GetMaintenanceModeEntity(MonitoredSqlServer monitoredServer)
            {
                AuditableEntity entity = monitoredServer.GetAuditableEntity();
                string maintenanceModeType = String.Empty;
                switch (monitoredServer.MaintenanceMode.MaintenanceModeType)
                {
                    case MaintenanceModeType.Always:
                        maintenanceModeType = Constants.Always;
                        break;
                    case MaintenanceModeType.Never:
                        maintenanceModeType = Constants.Never;
                        break;
                    case MaintenanceModeType.Once:
                        maintenanceModeType = Constants.Once;
                        break;
                    case MaintenanceModeType.Recurring:
                        maintenanceModeType = Constants.Recurring;
                        break;
                    case MaintenanceModeType.Monthly:
                        maintenanceModeType = Constants.Monthly;
                        break;
                    default:
                        maintenanceModeType = "None";
                        break;
                }

                entity.AddMetadataProperty("Changed Maintenance Mode to", maintenanceModeType);
                entity.AddMetadataProperty("Event type", "Automatic system event");

                return entity;
            }

            //private MonitoredSqlServerConfiguration CreateSqlServerConfig(MonitoredSqlServer sqlServer)
            //{
            //    MonitoredSqlServerConfiguration config = new MonitoredSqlServerConfiguration(sqlServer.ConnectionInfo);

            //    config.IsActive = sqlServer.IsActive;
            //    config.MaintenanceMode = sqlServer.MaintenanceMode;
            //    config.CollectionServiceId = sqlServer.CollectionServiceId;
            //    config.CustomCounters = sqlServer.CustomCounters;
            //    config.GrowthStatisticsDays = sqlServer.GrowthStatisticsDays;
            //    config.GrowthStatisticsStartTime = sqlServer.GrowthStatisticsStartTime;
            //    config.InstanceName = sqlServer.InstanceName;
            //    config.LastGrowthStatisticsRunTime = sqlServer.LastGrowthStatisticsRunTime;
            //    config.LastReorgStatisticsRunTime = sqlServer.LastReorgStatisticsRunTime;
            //    config.MaintenanceModeEnabled = sqlServer.MaintenanceModeEnabled;
            //    config.QueryMonitorConfiguration = sqlServer.QueryMonitorConfiguration;
            //    config.ReorganizationMinimumTableSize = sqlServer.ReorganizationMinimumTableSize;
            //    config.ReorgStatisticsDays = sqlServer.ReorgStatisticsDays;
            //    config.ReorgStatisticsStartTime = sqlServer.ReorgStatisticsStartTime;
            //    config.ReplicationMonitoringDisabled = sqlServer.ReplicationMonitoringDisabled;
            //    config.ScheduledCollectionInterval = sqlServer.ScheduledCollectionInterval;
            //    config.TableStatisticsExcludedDatabases = sqlServer.TableStatisticsExcludedDatabases;
            //    config.ExtendedHistoryCollectionDisabled = sqlServer.ExtendedHistoryCollectionDisabled;
            //    config.ActiveClusterNode = sqlServer.ActiveClusterNode;
            //    config.PreferredClusterNode = sqlServer.PreferredClusterNode;
            //    return (config);
            //}

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                lock (syncRoot)
                {
                    disposed = true;

                    if (normalCollectionTimer != null)
                    {
                        normalCollectionTimer.Dispose();
                        normalCollectionTimer = null;
                    }

                    normalCollectionSemaphore.Close();

                    if (serverPingTimer != null)
                    {
                        serverPingTimer.Dispose();
                        serverPingTimer = null;
                    }

                    StopDatabasePeriodicCollector();

                }
            }

            #endregion

            #endregion

            #region interface implementations

            #endregion

            #region nested types

            #endregion

        }



    }
}
