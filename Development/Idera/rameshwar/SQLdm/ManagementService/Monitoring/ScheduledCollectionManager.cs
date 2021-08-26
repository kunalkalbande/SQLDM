//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionDataProcessor.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
    using System.Data.SqlClient;
    using System.Text;
    using System.Xml;
    using Configuration;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.ManagementService.Monitoring.Data;
    using BBS.TracerX;
    using System.Threading;
    using Idera.SQLdm.ManagementService.Helpers;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Wintellect.PowerCollections;
    using Health;

    /// <summary>
    /// This class processes incoming scheduled collection data.  Persisting snapshots and
    /// events, as well as kicking off notification when needed.
    /// </summary>
    public class ScheduledCollectionManager 
    {
        #region fields

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionManager");
        private const int WriterThreads = 5;
        private const int MaxRetries = 5;

        private readonly object sync = new object();

        private ScheduledCollectionQueues collectionQueues;
        private ScheduledCollectionQueue queue;
        private List<Thread>        workThreads;
        private Set<Thread>         waitingThreads;
        private ManualResetEvent    connectionWaitEvent;
        private AlertQueueProcessor alertProcessor;

        private IDictionary<int, MonitoredSqlServerState> monitoredServers;

        private DateTime?           lastStatisticsFailureEventUTC;
        private int                 writeFailuresSinceLastEvent;

        private Timer statusTimer;
        private XmlDocument statusDocument;

        private Dictionary<string,long> cachedWaitTypes = new Dictionary<string, long>();
        private Dictionary<long, long> cachedSQLStatements = new Dictionary<long, long>();
        private Dictionary<long, long> cachedSQLSignatures = new Dictionary<long, long>();
        private Dictionary<long, long> cachedApplicationNames = new Dictionary<long, long>();
        private Dictionary<long, long> cachedHostNames = new Dictionary<long, long>();
        private Dictionary<long, long> cachedLoginNames = new Dictionary<long, long>();

        private bool running = false;

        #endregion

        #region constructors

        public ScheduledCollectionManager()
        {
            collectionQueues = new ScheduledCollectionQueues();
            queue = new ScheduledCollectionQueue(collectionQueues);
            workThreads = new List<Thread>();
            waitingThreads = new Set<Thread>();
            alertProcessor = new AlertQueueProcessor(collectionQueues);
            monitoredServers = new Dictionary<int, MonitoredSqlServerState>();

            connectionWaitEvent = new ManualResetEvent(true);
            statusTimer = new Timer(GetStatusDocument, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region properties

        public string MonitoredSQLServerStatusDocument
        {
            get
            {
                lock(sync)
                {
                   if (statusDocument == null)
                        GetStatusDocument(null);

                    return statusDocument != null ? statusDocument.OuterXml : null;
                }
            }
        }


        //SQLDM-28938. Update Status Document Regularly
        public string MonitoredSQLServerStatusDocumentForceUpdate
        {
            get
            {
                lock (sync)
                    GetStatusDocument(null);
                return statusDocument != null ? statusDocument.OuterXml : null;
            }
        }

        public object AlertTableSyncRoot
        {
            get { return alertProcessor.AlertsTableLock; }
        }

        public ScheduledCollectionQueues ScheduledCollectionQueues
        {
            get { return collectionQueues; }
        }

        public Dictionary<string, long> CachedWaitTypes
        {
            get
            {
                lock (sync)
                {
                    if (cachedWaitTypes == null)
                        cachedWaitTypes = new Dictionary<string, long>();
                    return cachedWaitTypes;
                }
            }
        }


        public Dictionary<long, long> CachedSQLStatements
        {
            get
            {
                lock (sync)
                {
                    if (cachedSQLStatements == null)
                        cachedSQLStatements = new Dictionary<long, long>();
                    return cachedSQLStatements;
                }
            }
        }

        public Dictionary<long, long> CachedSQLSignatures
        {
            get
            {
                lock (sync)
                {
                    if (cachedSQLSignatures == null)
                        cachedSQLSignatures = new Dictionary<long, long>();
                    return cachedSQLSignatures;
                }
            }
        }

        public Dictionary<long, long> CachedApplicationNames
        {
            get
            {
                lock (sync)
                {
                    if (cachedApplicationNames == null)
                        cachedApplicationNames = new Dictionary<long, long>();
                    return cachedApplicationNames;
                }
            }
        }

        public Dictionary<long, long> CachedLoginNames
        {
            get
            {
                lock (sync)
                {
                    if (cachedLoginNames == null)
                        cachedLoginNames = new Dictionary<long, long>();
                    return cachedLoginNames;
                }
            }
        }

        public Dictionary<long, long> CachedHostNames
        {
            get
            {
                lock (sync)
                {
                    if (cachedHostNames == null)
                        cachedHostNames = new Dictionary<long, long>();
                    return cachedHostNames;
                }
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public MonitoredSqlServerState AddMonitoredSqlServerState(MonitoredSqlServer server)
        {
            MonitoredSqlServerState state = null;
            lock (sync)
            {
                if (!monitoredServers.TryGetValue(server.Id, out state))
                {
                    state = new MonitoredSqlServerState(server);
                    new MonitoredSqlServerStateGraph(state);
                    monitoredServers.Add(server.Id, state);
                }
            }
            return state;
        }

        public MonitoredSqlServerState UpdateMonitoredSqlServerState(MonitoredSqlServer server)
        {
            MonitoredSqlServerState state = null;
            lock (sync)
            {
                if (!monitoredServers.TryGetValue(server.Id, out state))
                {
                    return AddMonitoredSqlServerState(server);
                }
                state.WrappedServer = server;
            }
            return state;
        }

        public void UpdateSnoozeInfo(int instanceId, int? metricId, SnoozeInfo snoozeInfo)
        {
            MonitoredSqlServerState state = GetCachedMonitoredSqlServer(instanceId);
            if (state != null)
            {
                state.UpdateSnoozeInfo(metricId == null ? null : new int[] { metricId.Value }, snoozeInfo);
            }
        }

        public MonitoredSqlServerState GetCachedMonitoredSqlServer(int id)
        {
            MonitoredSqlServerState state = null;
            lock (sync)
            {
                monitoredServers.TryGetValue(id, out state);
            }
            return state;
        }

        public MonitoredSqlServerState GetCachedMonitoredSqlServer(string instanceName)
        {
            return GetCachedMonitoredSqlServer(instanceName, StringComparison.CurrentCulture);
        }

        public MonitoredSqlServerState GetCachedMonitoredSqlServer(string instanceName, StringComparison comparison)
        {
            MonitoredSqlServerState state = null;

            lock (sync)
            {
                foreach (MonitoredSqlServerState server in monitoredServers.Values)
                {
                    if (server.WrappedServer.InstanceName.Equals(instanceName, comparison))
                    {
                        state = server;
                        break;
                    }
                }
            }

            return state;
        }

        public  Set<int> GetServerIds()
        {
            lock (sync)
            {
                return new Set<int>(monitoredServers.Keys);
            }
        }

        public List<string> GetServerNames()
        {
            lock (sync)
            {
                List<string> serverNames = new List<string>(monitoredServers.Count);
                foreach (MonitoredSqlServerState state in monitoredServers.Values)
                {
                    serverNames.Add(state.WrappedServer.InstanceName);
                }
                return serverNames;
            }
        }


        internal Dictionary<string, long> AddCachedWaitStats(Dictionary<string, long> value)
        {
            lock (sync)
            {
                if (value != null && value.Count > 0)
                {
                    if (cachedWaitTypes.Count == 0)
                    {
                        cachedWaitTypes = value;
                    }
                    else
                    {
                        foreach (string waitType in value.Keys)
                        {
                            if (!cachedWaitTypes.ContainsKey(waitType) && value[waitType] > 0)
                            {
                                cachedWaitTypes.Add(waitType, value[waitType]);
                            }
                        }
                    }
                }

                return cachedWaitTypes;
            }
        }

        internal Dictionary<long, long> AddCachedSqlStatement(Dictionary<long, long> value)
        {
            lock (sync)
            {
                if (value == null || cachedSQLStatements.Count > 1000)
                {
                    cachedSQLStatements.Clear();
                }
                else
                if (value.Count > 0)
                {
                    if (cachedSQLStatements.Count == 0)
                    {
                        cachedSQLStatements = value;
                    }
                    else
                    {
                        try
                        {
                            foreach (long hashKey in value.Keys)
                            {
                                if (!cachedSQLStatements.ContainsKey(hashKey) && value[hashKey] > 0)
                                {
                                    cachedSQLStatements.Add(hashKey, value[hashKey]);
                                }
                            }
                        }
                        catch (InvalidOperationException exception)
                        {
                            LOG.Error("AddCachedSqlStatement: " + exception);
                            // Retry
                            foreach (long hashKey in value.Keys)
                            {
                                if (!cachedSQLStatements.ContainsKey(hashKey) && value[hashKey] > 0)
                                {
                                    cachedSQLStatements.Add(hashKey, value[hashKey]);
                                }
                            }
                        }
                    }
                }

                return cachedSQLStatements;
            }
        }

        internal Dictionary<long, long> AddCachedSqlSignature(Dictionary<long, long> value)
        {
            lock (sync)
            {
                if (value == null || cachedSQLSignatures.Count > 1000)
                {
                    cachedSQLSignatures.Clear();
                }
                else
                    if (value.Count > 0)
                {
                    if (cachedSQLSignatures.Count == 0)
                    {
                        cachedSQLSignatures = value;
                    }
                    else
                    {
                        foreach (long hashKey in value.Keys)
                        {
                            if (!cachedSQLSignatures.ContainsKey(hashKey) && value[hashKey] > 0)
                            {
                                cachedSQLSignatures.Add(hashKey, value[hashKey]);
                            }
                        }
                    }
                }

                return cachedSQLSignatures;
            }
        }

        internal Dictionary<long, long> AddCachedApplicationName(Dictionary<long, long> value)
        {
            lock (sync)
            {
                if (value == null || cachedApplicationNames.Count > 1000)
                {
                    cachedApplicationNames.Clear();
                }
                else
                    if (value.Count > 0)
                {
                    if (cachedApplicationNames.Count == 0)
                    {
                        cachedApplicationNames = value;
                    }
                    else
                    {
                        foreach (long hashKey in value.Keys)
                        {
                            if (!cachedApplicationNames.ContainsKey(hashKey) && value[hashKey] > 0)
                            {
                                cachedApplicationNames.Add(hashKey, value[hashKey]);
                            }
                        }
                    }
                }

                return cachedApplicationNames;
            }
        }

        internal Dictionary<long, long> AddCachedHostName(Dictionary<long, long> value)
        {
            lock (sync)
            {
                if (value == null || cachedHostNames.Count > 1000)
                {
                    cachedHostNames.Clear();
                }
                else
                    if (value.Count > 0)
                {
                    if (cachedHostNames.Count == 0)
                    {
                        cachedHostNames = value;
                    }
                    else
                    {
                        foreach (long hashKey in value.Keys)
                        {
                            if (!cachedHostNames.ContainsKey(hashKey) && value[hashKey] > 0)
                            {
                                cachedHostNames.Add(hashKey, value[hashKey]);
                            }
                        }
                    }
                }

                return cachedHostNames;
            }
        }

        internal Dictionary<long, long> AddCachedLoginName(Dictionary<long, long> value)
        {
            lock (sync)
            {
                if (value == null || cachedLoginNames.Count > 1000)
                {
                    cachedLoginNames.Clear();
                }
                else
                    if (value.Count > 0)
                {
                    if (cachedLoginNames.Count == 0)
                    {
                        cachedLoginNames = value;
                    }
                    else
                    {
                        foreach (long hashKey in value.Keys)
                        {
                            if (!cachedLoginNames.ContainsKey(hashKey) && value[hashKey] > 0)
                            {
                                cachedLoginNames.Add(hashKey, value[hashKey]);
                            }
                        }
                    }
                }

                return cachedLoginNames;
            }
        }

        internal void RemoveServerTags(ICollection<int> tagIds)
        {
            lock (sync)
            {
                foreach (MonitoredSqlServerState serverState in monitoredServers.Values)
                {
                    MonitoredSqlServer server = serverState.WrappedServer;
                    server.RemoveTags(tagIds);
                }
            }
        }

        internal void SyncServerTags(int tagId, IList<int> serverIds, IList<int> customCounterIds)
        {
            List<MonitoredSqlServerState> pushList = new List<MonitoredSqlServerState>();
            lock (sync)
            {
                foreach (MonitoredSqlServerState serverState in monitoredServers.Values)
                {
                    MonitoredSqlServer server = serverState.WrappedServer;
                    if (serverIds.Contains(server.Id))
                    {
                        server.AddTag(tagId);
                        foreach (int metricId in customCounterIds)
                        {
                            MetricThresholdEntry threshold = serverState.GetMetricThresholdEntry(metricId);
                            if (threshold == null)
                            {
                                pushList.Add(serverState);
                                break;
                            }
                        }
                    }
                    else
                        server.RemoveTag(tagId);
                } 
            }
            foreach (MonitoredSqlServerState serverState in pushList)
            {   // need to sync cached metric thresholds and push workload
                UpdateAndPushWorkload(serverState);
            }
        }

        private void UpdateAndPushWorkload(MonitoredSqlServerState state)
        {
            // get a new workload for this here server from the repository
            string connection = ManagementServiceConfiguration.ConnectionString;
            MonitoredSqlServer server = RepositoryHelper.GetMonitoredSqlServer(connection, state.WrappedServer.Id);
            if (server != null)
            {
                MonitoredServerWorkload workload = Management.CollectionServices.GetMonitoredServerWorkload(server);
                // update internal state with workload
                state.Update(workload);

                Management.CollectionServices.UpdateMonitoredServer(workload);
            }
        }
   
        private void GetStatusDocument(object arg)
        {
            // wait for an open connection
            SqlConnection connection = GetOpenConnection();
            if (connection == null)
                return;

            XmlDocument document = null;
            try
            {
                // ensure the alertProcessor and this query are not hitting the repository concurrently
                // Management Service Memory Issue: Comment this code as we dont need lock here
                //lock (alertProcessor.AlertsTableLock)
                {
                    document = RepositoryHelper.GetMonitoredSqlServerStatus(connection, null);
                }
                // swap out the visible status xml with its new value
                lock (sync)
                {
                    statusDocument = document;
                }
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
            finally
            {
                try { connection.Dispose(); } catch { /* */ }
            }
        }

        /// <summary>
        /// Get the status document and extract the status for the given instance.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>New status document created from cached status</returns>
        public XmlDocument GetCachedStatusDocument(int instanceId)
        {
            XmlDocument document = null;

            try
            {
                lock (sync)
                {
                    // find the matching node in the status document
                    string nodequery = String.Format("/Servers/Server[@SQLServerID={0}]", instanceId);
                    XmlNode statusNode = statusDocument.SelectSingleNode(nodequery);
                    if (statusNode != null)
                    {
                        document = new XmlDocument();
                        XmlElement root = document.CreateElement("Servers");
                        document.AppendChild(root);
                        XmlNode serverStatus = document.ImportNode(statusNode, true);
                        root.AppendChild(serverStatus);
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
            return document;
        }

        public XmlDocument UpdateStatusDocument(int monitoredSqlServerID)
        {
            try
            {
                //SQlDM-28022 - Handling connection object to avoid leakage
                // Stores the status document from the repository
                XmlDocument doc;
                using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
                {
                    connection.Open();
                    // get the status document from the repository
                    doc = RepositoryHelper.GetMonitoredSqlServerStatus(connection, monitoredSqlServerID);
                }

                // update the shared copy
                UpdateStatusDocument(doc);
                
                // return the server status document
                return doc;
            }
            catch (Exception e)
            {
                LOG.Error("Error updating status document. ", e);
                throw;
            }
        }

        /// <summary>
        /// Update the existing status document with entries in updateDocument.
        /// </summary>
        /// <param name="updateDocument"></param>
        /// <returns></returns>
        public XmlDocument UpdateStatusDocument(XmlDocument updateDocument)
        {
            using (LOG.InfoCall("UpdateStatusDocument"))
            {
                lock (sync)
                {
                    // serverStatus has same schema as statusDocument
                    foreach (XmlNode updateNode in updateDocument.DocumentElement.ChildNodes)
                    {
                        // clone the replacement with the status document as the owner
                        XmlNode newNode = statusDocument.ImportNode(updateNode, true);

                        // get the id of the server to update
                        XmlAttribute serverIdAttribute = updateNode.Attributes["SQLServerID"];
                        if (serverIdAttribute == null)
                        {
                            LOG.Warn("Unable to identify server status node: ", updateNode.OuterXml);
                            continue;
                        }

                        string serverId = serverIdAttribute.Value;
                        // find the matching node in the status document
                        string nodequery = String.Format("/Servers/Server[@SQLServerID={0}]", serverId);
                        XmlNode statusNode = statusDocument.SelectSingleNode(nodequery);

                        if (statusNode != null)
                        {
                            // replace the old node with the new node
                            statusNode.ParentNode.ReplaceChild(newNode, statusNode);
                        }
                        else
                        {
                            // append the new status node to the document
                            statusDocument.DocumentElement.AppendChild(newNode);
                        }
                    }

                    return statusDocument;
                }
            }
        }

        public void UpdateStatusDocumentSnoozeInfo(int monitoredSqlServerID, bool snoozing, int[] metrics)
        {
            using (LOG.VerboseCall("UpdateStatusDocumentSnoozeInfo"))
            {
                try
                {
                    lock (sync)
                    {
                        // find the matching node in the status document
                        string nodequery = String.Format("/Servers/Server[@SQLServerID={0}]", monitoredSqlServerID);
                        XmlElement statusNode = statusDocument.SelectSingleNode(nodequery) as XmlElement;
                        if (statusNode != null)
                        {
                            bool allMetrics = metrics == null || metrics.Length == 0;
                            int nAlertsSnoozing = 0;
                            int nThresholds = 46;

                            XmlAttribute nThresholdsAttribute = statusNode.Attributes["ThresholdCount"];
                            XmlAttribute nSnoozingAttribute = statusNode.Attributes["AlertsSnoozing"];
                            if (nThresholdsAttribute != null)
                                Int32.TryParse(nThresholdsAttribute.Value, out nThresholds);
                            if (nSnoozingAttribute != null)
                                Int32.TryParse(nSnoozingAttribute.Value, out nAlertsSnoozing);

                            if (snoozing)
                            {
                                if (allMetrics)
                                    nAlertsSnoozing = nThresholds;
                                 else
                                    nAlertsSnoozing = Math.Min(nAlertsSnoozing + metrics.Length, nThresholds);
                            }
                            else
                            {
                                if (allMetrics)
                                    nAlertsSnoozing = 0;
                                else
                                    nAlertsSnoozing = Math.Max(nAlertsSnoozing - metrics.Length, 0);
                            }

                            // Set AlertsSnoozing to its new value
                            SetXmlAttribute((XmlElement) statusNode,
                                            "AlertsSnoozing",
                                            nAlertsSnoozing.ToString());

                            if (snoozing && allMetrics)
                            {
                                // Reset warning and critical alert counts
                                SetXmlAttribute((XmlElement) statusNode, "ActiveWarningAlerts", "0");
                                SetXmlAttribute((XmlElement) statusNode, "ActiveCriticalAlerts", "0");
                                // if snoozed - clear out all issues and set severity to OK
                                XmlNodeList stateNodes = statusNode.SelectNodes(".//State");
                                if (stateNodes != null)
                                {
                                    foreach (XmlElement element in stateNodes)
                                    {
                                        element.ParentNode.RemoveChild(element);
                                    }
                                }
                            }
                            LOG.VerboseFormat("Updating AlertsSnoozing count to {0} (all={1} metrics.Length={2})",
                                            nAlertsSnoozing, allMetrics, metrics == null ? -1 : metrics.Length);
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                }
            }
        }



        private static void SetXmlAttribute(XmlElement element, string attName, string attValue)
        {
            XmlAttribute attribute = element.Attributes[attName];
            if (attribute == null)
            {
                attribute = element.Attributes.Append(element.OwnerDocument.CreateAttribute(attName));
            }
            attribute.Value = attValue;
        }

        public void Add(int instanceId, ScheduledCollectionDataMessage data)
        {
            queue.Enqueue(data.MonitoredServer.Id, data);
        }

        public void Start()
        {
            using (LOG.VerboseCall("Start"))
            {
                // set running flag on
                lock (queue.SyncRoot)
                {
                    running = true;
                }

                lock (sync)
                {
                    // load up all the monitored servers and their state graphs
                    using (SqlConnection repositoryConnection = GetOpenConnection())
                    {
                        monitoredServers.Clear();
                        foreach (MonitoredSqlServerState state in RepositoryHelper.GetMonitoredSqlServersState(repositoryConnection, null, true)) 
                        {
                            monitoredServers.Add(state.WrappedServer.Id, state);
                        }
                    }
                }

                // start worker threads
                for (int count = workThreads.Count; count < WriterThreads; count++)
                {
                    Thread writerThread = new Thread(Run);
                    writerThread.IsBackground = true;
                    // Management Service Memory Issue: Assigning the below normal priority to enqueue
                    writerThread.Priority = ThreadPriority.BelowNormal;
                    writerThread.Name = String.Format("ScheduledRefresh-{0}", count + 1);
                    // keep track of them
                    workThreads.Add(writerThread);
                    // start the thread
                    writerThread.Start();
                }
                LOG.VerboseFormat("Started {0} scheduled refresh writer threads.", WriterThreads);

                alertProcessor.Start();

                statusTimer.Change(0, 30000);
            }
        }

        public bool Stop(TimeSpan waitTime)
        {
            using (LOG.DebugCall("Stop"))
            {
                // stop the status timer
                statusTimer.Change(Timeout.Infinite, Timeout.Infinite);

                lock (queue.SyncRoot)
                {
                    running = false;
                    LOG.DebugFormat("Interrupting {0} threads waiting for work.", waitingThreads.Count);
                    foreach (Thread thread in waitingThreads)
                    {
                        thread.Interrupt();
                    }
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (Thread thread in workThreads.ToArray())
                {
                    TimeSpan timeLeft = waitTime - stopwatch.Elapsed;
                    if (timeLeft.Ticks < 0)
                    {
                        stopwatch.Stop();
                        LOG.Debug("Ran out of time waiting for threads to end.");
                        return false;
                    }
                    System.Threading.ThreadState state = thread.ThreadState;
                    try
                    {
                        if (thread.Join(timeLeft))
                        {
                            workThreads.Remove(thread);
                            LOG.VerboseFormat("Background thread {0} has ended.", thread.Name);
                        }
                        else
                        {
                            LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", thread.Name);
                            return false;
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", thread.Name, state);
                    }
                }
                Statistics.Dispose();
                alertProcessor.Stop(waitTime - stopwatch.Elapsed);

                stopwatch.Stop();
                return true;
            }
        }

        private void Run()
        {
            int instanceId = -1;
//            PersistenceManager pm = PersistenceManager.Instance;

            for ( ; ; )
            {
                lock (queue.SyncRoot)
                {
                    if (!running)
                        break;

                    // add us to the list of waiting threads
                    waitingThreads.Add(Thread.CurrentThread);

                    // wait for something to do or shutdown
                    while (queue.WaitingQueueCount == 0 && running)
                    {
                        try
                        {
                            Monitor.Wait(queue.SyncRoot);
                        }
                        catch (ThreadInterruptedException)
                        {
                            /* ignore */
                        }
                    }

                    // remove us from the list of waiting threads
                    waitingThreads.Remove(Thread.CurrentThread);

                    if (!running)
                        break;
                   
                    instanceId = queue.ReserveFirstWaitingQueue();
                }
                try
                {
                    var data = queue.Peek(instanceId);
                    if (data != null)
                    {
                        var message = data.Message;
                        if (message != null)
                        {
                            var snapshot = data.Message.Snapshot;
                            if (snapshot != null)
                            {
                                try
                                {
                                    // should be ok to process
                                    Process(instanceId, data);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error("Unhandled exception in Process(instanceId, data) for instance id: ", instanceId);
                                    // unknown problem remove from queue so it doesn't happen again
                                    queue.Remove(instanceId, data);
                                }
                            }
                            else
                            {
                                LOG.Info("Snapshot is null for periodic refresh for instance id: ", instanceId);
                                // useless without a snapshot - remove from the queue
                                queue.Remove(instanceId, data);
                            }
                        }
                        else
                        {
                            LOG.Info("Data is null for periodic refresh for instance id: ", instanceId);
                            // useless without a message - shouldn't happen 
                            queue.Remove(instanceId, data);
                        }
                    } else // shouldn't happen
                        LOG.Info("Peek returned a null message for instance id: ", instanceId);
                }
                finally
                {
                    lock (queue.SyncRoot)
                    {
                        queue.Release(instanceId);
                    }
                }
            }

            // make sure we are not in the list of waiting threads
            lock (queue.SyncRoot)
            {
                waitingThreads.Remove(Thread.CurrentThread);
            }
        }

        private void Process(int instanceId, ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            // Check to see if this is a Scheduled Refresh object - could be something else
            var scheduledRefresh = data.Message.Snapshot as ScheduledRefresh;
            if (scheduledRefresh != null)
            {
                // See if the server need reconfiguring
                bool reconfigure = scheduledRefresh.RequestReconfiguration;
                try
                {
                    if (scheduledRefresh.Server.IsClustered.HasValue && scheduledRefresh.Server.IsClustered.Value)
                    {
                        var server = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId).WrappedServer;
                        if ((server.PreferredClusterNode == null || server.PreferredClusterNode.Trim().Length == 0) &&
                            scheduledRefresh.Server.ClusterNodeName != null)
                        {
                            server.PreferredClusterNode = scheduledRefresh.Server.ClusterNodeName;
                            reconfigure = true;
                        }
                    }
                }
                catch (Exception)
                {
                    LOG.Error("Error setting reconfigure option for scheduled collection.");
                }

                // Update workload if requested or if this is a cluster that needs a preferred cluster node
                if (reconfigure)
                {
                    try
                    {
                        ManagementService.InternalUpdateMonitoredSqlServer(instanceId,
                                                                           Management.ScheduledCollection.
                                                                               GetCachedMonitoredSqlServer(
                                                                                   instanceId).WrappedServer
                                                                               .
                                                                               GetConfiguration(),
                                                                           true);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error reconfiguring workload. Retrying.", e);
                        try
                        {
                            ManagementService.InternalUpdateMonitoredSqlServer(instanceId,
                                                                               Management.
                                                                                   ScheduledCollection.
                                                                                   GetCachedMonitoredSqlServer
                                                                                   (
                                                                                       instanceId).
                                                                                   WrappedServer.
                                                                                   GetConfiguration(),
                                                                               true);
                        }
                        catch (Exception e2)
                        {
                            LOG.Error("Unable to reconfigure workload.", e2);
                        }
                    }
                }

            }

            data.ProcessingAttempts++;

            if (!data.StatisticsWritten)
            {
                ProcessStatistics(data);
            }

            if (data.StatisticsWritten)
            {
                lock (queue.SyncRoot)
                {   // move the data to the alert writer queue
                    this.alertProcessor.AddScheduledRefresh(data);
                    this.queue.Remove(instanceId, data);
                }
            }
            else
            {
                if (data.ProcessingAttempts >= MaxRetries)
                {
                    LOG.DebugFormat("Dropping scheduled collection (Stats {0})", data.StatisticsWritten);
                    lock (queue.SyncRoot)
                    {
                        queue.Remove(instanceId, data);
                    }
                }
            }
        }

        //private void ProcessNotifications(ScheduledCollectionQueue.ScheduledCollectionData data)
        //{
        //    ScheduledRefresh scheduledRefresh = data.Message.Snapshot as ScheduledRefresh;

        //    if (scheduledRefresh != null)
        //        Management.Notification.Process(scheduledRefresh);

        //    data.NotificationsProcessed = true;
        //}


        //private void ProcessAlerts(ScheduledCollectionQueue.ScheduledCollectionData data)
        //{
        //    ScheduledRefresh scheduledRefresh = data.Message.Snapshot as ScheduledRefresh;

        //    if (scheduledRefresh != null)
        //    {
        //        try
        //        {
        //            // wait for an open connection
        //            SqlConnection connection = GetOpenConnection();
        //            if (connection == null)
        //                return;

        //            // process the alert events
        //            AlertTableWriter.LogThresholdViolations(connection, scheduledRefresh, data.AlertRestartPoint);
        //            // set that we are through
        //            data.AlertsWritten = true;
        //        } 
        //        catch (AlertTableWriter.AlertProcessingException alertProcessingException)
        //        {
        //            // we had a failure somewhere - record where we left off for a retry
        //            data.AlertRestartPoint = alertProcessingException.NumberProcessed;    
        //        }
        //        catch (Exception e)
        //        {
        //            // who knows what happened - abort the processing of alerts
        //            LOG.Error("Unexpected error processing alert events: ", e);
        //            data.AlertsWritten = true;
        //        }
        //    }
        //}

        private void ProcessStatistics(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            using (LOG.InfoCall("ProcessStatistics"))
            {
                if (data.Message == null || data.Message.Snapshot == null)
                {
                    LOG.Warn("Message or snapshot is null - skipping save");
                    data.StatisticsWritten = true;
                    return;
                }

                var scheduledRefresh = data.Message.Snapshot as ScheduledRefresh;
                if (scheduledRefresh != null)
                {
                    try
                    {

                        LOG.InfoFormat("Processing server {0} statistics collected at {1}. (attempt={2})",
                                        scheduledRefresh.MonitoredServer.InstanceName,
                                        scheduledRefresh.Server != null
                                            ? scheduledRefresh.TimeStamp
                                            : null,
                                        data.ProcessingAttempts);

                        //SQlDM-28022 - Handling connection object to avoid leakage and Ensure Connection is closed
                        using (SqlConnection connection = GetOpenConnection())
                        {
                            if (connection == null)
                                return;

                            using (var scdm = new ScheduledCollectionDataManager(connection))
                            {
                                scdm.SaveScheduledRefresh(scheduledRefresh);
                            }
                        }

                        data.StatisticsWritten = true;
                    }
                    catch (Exception e)
                    {
                        String logMessage = "Save scheduled refresh failed.";
                        LOG.Error(logMessage);

                        lock (sync)
                        {
                            writeFailuresSinceLastEvent++;
                        }

                        if (e is SqlException && data.ProcessingAttempts >= MaxRetries)
                        {
                            lock (sync)
                            {
                                if (lastStatisticsFailureEventUTC == null ||
                                    (DateTime.UtcNow - lastStatisticsFailureEventUTC.Value).TotalMinutes > 15)
                                {
                                    // write this sucker to the event log
                                    StringBuilder builder = new StringBuilder(logMessage);
                                    builder.AppendFormat(
                                        "  {0} attempts to write statistics to the repository have failed",
                                        writeFailuresSinceLastEvent);
                                    if (lastStatisticsFailureEventUTC.HasValue)
                                        builder.AppendFormat(" since {0}",
                                                                lastStatisticsFailureEventUTC.Value.ToLocalTime());
                                    builder.Append(
                                        ".  Collected data will be cached on disk until it can be written to the repository.\n\n")
                                        .Append(e.ToString());
                                    if (builder.Length > 512)
                                        builder.Length = 512;
                                    Management.WriteEvent((int) EventLogEntryType.Error, Status.ErrorRepositoryError,
                                                            Category.General, builder.ToString());

                                    lastStatisticsFailureEventUTC = DateTime.UtcNow;
                                    writeFailuresSinceLastEvent = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var databaseSizeSnapshot = data.Message.Snapshot as DatabaseSizeSnapshot;
                    if (databaseSizeSnapshot != null)
                    {
                        try
                        {
                            //SQlDM-28022 - Handling connection object to avoid leakage and Ensure Connection is closed
                            using (var connection = GetOpenConnection())
                            {
                                if (connection == null)
                                    return;

                            using (var scdm = new ScheduledCollectionDataManager(connection))
                            {
                                scdm.SaveDatabaseSize(databaseSizeSnapshot);
                               //SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Saving DiskDrive Statistics 
								scdm.SaveDiskDriveStatistics(databaseSizeSnapshot);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error saving database statistics",e);
                        }
                    }
                    data.StatisticsWritten = true;
                    LOG.Info(String.Format("Processing {0} from {1}",data.Message.Snapshot.GetType(),data.Message.MonitoredServer.InstanceName));   
                }
            }
        }
/*

        /// <summary>
        /// Processes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        private bool Process(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            // make sure the message has enough stuff to process
            if (data == null || data.Message == null || data.Message.Snapshot == null)
                return true;

            if (!data.StatisticsWritten)
            {
                ScheduledRefresh scheduledRefresh = data.Message.Snapshot as ScheduledRefresh;
                if (scheduledRefresh != null)
                {
                    try
                    {
                        SqlConnection connection = GetOpenConnection();
                        if (connection == null)
                            return false;

                        using (ScheduledCollectionDataManager scdm = new ScheduledCollectionDataManager(connection))
                        {
                            scdm.SaveScheduledRefresh(scheduledRefresh);
                        }
                        data.StatisticsWritten = true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
            if (!data.AlertsWritten)
            {
                // process the alerts in this thread
                AlertTableWriter.LogThresholdViolations(scheduledRefresh, false);
            }
            if (!data.NotificationsProcessed)
            {
                // process notifications for any new events in a background thread
                Management.Notification.Process(scheduledRefresh);
            }
       
            return true;
        }

*/
        private SqlConnection GetOpenConnection()
        {
            bool eventLogged = false;
            SqlConnection connection = null;

            for ( ; connection == null && IsRunning ; )
            {
                try 
                {
                    // causes the thread to wait 60 seconds when one of 
                    // the other theads can't connect to the repository
                    connectionWaitEvent.WaitOne(TimeSpan.FromSeconds(60), false);
                }
                catch (Exception)
                {
                    /* */
                }
                
                if (!IsRunning)
                    return null;

                connection = ManagementServiceConfiguration.GetRepositoryConnection();

                try
                {
                    // try to open the connection
                    connection.Open();

                    // if we logged an error then test the new connection
                    if (eventLogged)
                    {
                        if (RepositoryHelper.TestRepositoryConnection(connection, Management.EventLog))
                        {
                            // log that we got connected to the repository
                            MainService.WriteEvent(Management.EventLog,
                                                   EventLogEntryType.Information,
                                                   Status.RepositoryTestPassed,
                                                   Category.General,
                                                   "Connection to the SQLDM Repository was successful.");
                            eventLogged = false;
                        }
                    }

                    // signal other possibly waiting threads that connections are working again
                    connectionWaitEvent.Set();
                }
                catch (Exception e)
                {
                    // cause all future connection requests to block
                    connectionWaitEvent.Reset();

                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                        connection = null;
                    }

                    if (!eventLogged)
                    {
                        RepositoryHelper.
                            TestRepositoryConnection(ManagementServiceConfiguration.ConnectionString, Management.EventLog);
                        eventLogged = true;
                    }
                    LOG.Error("Unable to open connection to repository: ", e);
                }
            }

            return connection;
        }

        private bool IsRunning
        {
            get
            {
                lock (queue.SyncRoot)
                {
                    return running;    
                }    
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        #region IDisposable Members

        public void  Dispose()
        {

        }

        #endregion


        internal void SyncThresholds(int metricId, List<Idera.SQLdm.Common.Thresholds.MetricThresholdEntry> thresholds)
        {
            MonitoredSqlServerState serverState = null;
            Set<int> serverKeys = GetServerIds();

            foreach (MetricThresholdEntry threshold in thresholds)
            {
                int serverId = threshold.MonitoredServerID;
                serverState = GetCachedMonitoredSqlServer(serverId);
                if (serverState == null)
                    continue;

                // add the metric to the server
                if (serverState.AddMetricThresholdEntry(threshold))
                    AdjustStatusCustomCounterCount(serverId, 1);
                // remove server from diff set
                serverKeys.Remove(serverId);
               
            }

            foreach (int serverId in serverKeys)
            {
                serverState = GetCachedMonitoredSqlServer(serverId);
                if (serverState == null)
                    continue;

                if (serverState.RemoveMetricThresholdEntry(metricId))
                {
                    AdjustStatusCustomCounterCount(serverId, -1);
                }
            }
        }

        private void AdjustStatusCustomCounterCount(int monitoredSqlServerID, int adjustment)
        {
            using (LOG.VerboseCall("AdjustStatusCustomCounterCount"))
            {
                try
                {
                    lock (sync)
                    {
                        int nCustomCounters = 0;
                        // find the matching node in the status document
                        string nodequery = String.Format("/Servers/Server[@SQLServerID={0}]", monitoredSqlServerID);
                        XmlElement statusNode = statusDocument.SelectSingleNode(nodequery) as XmlElement;
                        if (statusNode != null)
                        {
                            XmlAttribute nCustomCountersAttribute = statusNode.Attributes["CustomCounterCount"];
                            if (nCustomCountersAttribute != null)
                                Int32.TryParse(nCustomCountersAttribute.Value, out nCustomCounters);

                            nCustomCounters += adjustment;

                            if (nCustomCounters < 0)
                                nCustomCounters = 0;
                            
                            // Set CustomCounterCount to its new value
                            SetXmlAttribute((XmlElement) statusNode,
                                            "CustomCounterCount",
                                            nCustomCounters.ToString());

                            LOG.VerboseFormat("Updating custom counter count to {0} for server {0}",
                                                nCustomCounters, monitoredSqlServerID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                }
            }
        }
    }
}
