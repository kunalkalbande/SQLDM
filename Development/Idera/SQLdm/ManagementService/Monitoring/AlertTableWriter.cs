//------------------------------------------------------------------------------
// <copyright file="AlertTableWriter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System.Data;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Threading;
    using Data;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;
    using Microsoft.ApplicationBlocks.Data;
    using Wintellect.PowerCollections;
    using System.Xml;
    using System.Linq;

    public class AlertQueueProcessor
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AlertQueueProcessor");

        private ManualResetEvent connectionWaitEvent;
        private AlertWriterQueue queue;
        private Thread writerThread;
        // Management Service Memory Issue: Adding additional 4 dequeue threads to avoid throttling issue.
        private Thread writerThread1;
        private Thread writerThread2;
        private Thread writerThread3;
        private Thread writerThread4;
        private bool running;

        /// <summary>
        /// Object used to reserve the use of the alerts table.
        /// </summary>
        public readonly object AlertsTableLock = new object();


        public AlertQueueProcessor(ScheduledCollectionQueues collectionQueues)
        {
            queue = new AlertWriterQueue(collectionQueues);
            connectionWaitEvent = new ManualResetEvent(true);
        }

        //        public void AddFromScheduledCollectionQueue(int instanceId)
        //        {
        //            lock(queue.SyncRoot)
        //            {
        //                queue.AddFromScheduledCollectionQueue(instanceId);
        //            }
        //        }

        public void AddScheduledRefresh(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            queue.AddScheduledRefresh(data);
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

                if (writerThread == null)
                {
                    // start worker threads
                    writerThread = new Thread(Run);
                    // Management Service Memory Issue: Assigning highest priority for dequeue thread.
                    writerThread.Priority = ThreadPriority.Highest;
                    writerThread.IsBackground = true;
                    writerThread.Name = "AlertQueue-1";
                    writerThread.Start();
                    LOG.Verbose("Started alert queue processor thread.");
                }
                if (writerThread1 == null)
                {
                    // start worker threads
                    writerThread1 = new Thread(Run);
                    writerThread1.Priority = ThreadPriority.Highest;
                    writerThread1.IsBackground = true;
                    writerThread1.Name = "AlertQueue-2";
                    writerThread1.Start();
                    LOG.Verbose("Started alert queue processor thread.");
                }
                if (writerThread2 == null)
                {
                    // start worker threads
                    writerThread2 = new Thread(Run);
                    writerThread2.Priority = ThreadPriority.Highest;
                    writerThread2.IsBackground = true;
                    writerThread2.Name = "AlertQueue-3";
                    writerThread2.Start();
                    LOG.Verbose("Started alert queue processor thread.");
                }
                if (writerThread3 == null)
                {
                    // start worker threads
                    writerThread3 = new Thread(Run);
                    writerThread3.Priority = ThreadPriority.Highest;
                    writerThread3.IsBackground = true;
                    writerThread3.Name = "AlertQueue-4";
                    writerThread3.Start();
                    LOG.Verbose("Started alert queue processor thread.");
                }
                if (writerThread4 == null)
                {
                    // start worker threads
                    writerThread4 = new Thread(Run);
                    writerThread4.Priority = ThreadPriority.Highest;
                    writerThread4.IsBackground = true;
                    writerThread4.Name = "AlertQueue-5";
                    writerThread4.Start();
                    LOG.Verbose("Started alert queue processor thread.");
                }
            }
        }

        public bool Stop(TimeSpan waitTime)
        {
            using (LOG.DebugCall("Stop"))
            {
                lock (queue.SyncRoot)
                {
                    if (!running)
                        return true;

                    running = false;
                    LOG.DebugFormat("Interrupting all threads waiting for work.");
                    writerThread.Interrupt();
                    writerThread1.Interrupt();
                    writerThread2.Interrupt();
                    writerThread3.Interrupt();
                    writerThread4.Interrupt();
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                System.Threading.ThreadState state = writerThread.ThreadState;
                try
                {
                    if (writerThread.Join(waitTime))
                    {
                        LOG.VerboseFormat("Background thread {0} has ended.", writerThread.Name);
                    }
                    else
                    {
                        LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", writerThread.Name);
                        return false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", writerThread.Name, state);
                }

                state = writerThread1.ThreadState;
                try
                {
                    if (writerThread1.Join(waitTime))
                    {
                        LOG.VerboseFormat("Background thread {0} has ended.", writerThread1.Name);
                    }
                    else
                    {
                        LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", writerThread1.Name);
                        return false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", writerThread1.Name, state);
                }

                state = writerThread2.ThreadState;
                try
                {
                    if (writerThread2.Join(waitTime))
                    {
                        LOG.VerboseFormat("Background thread {0} has ended.", writerThread2.Name);
                    }
                    else
                    {
                        LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", writerThread2.Name);
                        return false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", writerThread2.Name, state);
                }

                state = writerThread3.ThreadState;
                try
                {
                    if (writerThread3.Join(waitTime))
                    {
                        LOG.VerboseFormat("Background thread {0} has ended.", writerThread3.Name);
                    }
                    else
                    {
                        LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", writerThread3.Name);
                        return false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", writerThread3.Name, state);
                }

                state = writerThread4.ThreadState;
                try
                {
                    if (writerThread4.Join(waitTime))
                    {
                        LOG.VerboseFormat("Background thread {0} has ended.", writerThread4.Name);
                    }
                    else
                    {
                        LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", writerThread4.Name);
                        return false;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", writerThread4.Name, state);
                }

                stopwatch.Stop();
                return true;
            }
        }

        private void Run()
        {
            PersistenceManager pm = PersistenceManager.Instance;

            for (; ; )
            {
                ScheduledCollectionQueue.ScheduledCollectionData data = null;

                lock (queue.SyncRoot)
                {
                    if (!running)
                        break;

                    // wait for something to do or shutdown
                    while (queue.Count == 0 && running)
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

                    if (!running)
                        break;
                    // get the item out of the queue
                    data = queue.Dequeue();
                    // Memory Management Issue : Log to see the queue count details 
                    LOG.ErrorFormat("Memory Management in alertWriterQueue - performing dequeue and the queue count is {0}", queue.Count);
                }
                if (data != null)
                {
                    data.ProcessingAttempts++;

                    // process alerts event if the statistics didn't get stored
                    // only process notifications if the alerts get stored
                    if (!data.AlertsWritten)
                    {
                        ProcessAlerts(data);
                        if (data.AlertsWritten)
                        {
                            ProcessNotifications(data);
                            UpdateStatus(data);
                            // set waiting requesters free!
                            List<Guid> sinks = data.Message.GetSinks();
                            if (sinks != null)
                            {
                                foreach (Guid sinkid in sinks)
                                {
                                    ISnapshotSink sink = OnDemandCollectionContext<object>.GetSink(sinkid);
                                    if (sink != null)
                                    {
                                        try
                                        {
                                            sink.Process(null, null);
                                        }
                                        catch (Exception ex)
                                        {
                                            LOG.Error("Error processing snapshot sink",ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!data.NotificationsProcessed)
                    {
                        ProcessNotifications(data);
                    }

                    if (!data.AlertsWritten || !data.NotificationsProcessed)
                    {
                        if (data.ProcessingAttempts >= 5)
                        {
                            LOG.DebugFormat("Dropping scheduled collection (Alerts {0}:Notifications {1})",
                                            data.AlertsWritten,
                                            data.NotificationsProcessed);
                        }
                        else
                        {
                            // if not fully processed requeue it for another try
                            queue.RequeueScheduledRefresh(data);
                            Thread.Sleep(500);
                        }
                    }
                }
            }
        }

        private void ProcessNotifications(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            AlertableSnapshot refresh = data.Message.Snapshot as AlertableSnapshot;

            if (refresh != null)
                Management.Notification.Process(refresh);

            data.NotificationsProcessed = true;
        }


        private void ProcessAlerts(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            Stopwatch stopwatch = new Stopwatch();

            AlertableSnapshot refresh = data.Message.Snapshot as AlertableSnapshot;

            if (refresh != null)
            {
                try
                {
                    // wait for an open connection
                    using (SqlConnection connection = GetOpenConnection())
                    {
                        if (connection == null)
                            return;

                        lock (AlertsTableLock)
                        {
                            stopwatch.Start();
                            // process the alert events
                            AlertTableWriter.LogThresholdViolations(connection, data);
                            //AlertTableWriter.LogThresholdViolations(connection, scheduledRefresh, data.AlertRestartPoint);
                            stopwatch.Stop();
                            LOG.VerboseFormat("Alert processing time: {0} ms", stopwatch.ElapsedMilliseconds);
                        }
                    }
                    // set that we are through
                    data.AlertsWritten = true;
                }
                catch (AlertTableWriter.AlertProcessingException alertProcessingException)
                {
                    // we had a failure somewhere - record where we left off for a retry
                    data.AlertRestartPoint = alertProcessingException.NumberProcessed;
                }
                catch (Exception e)
                {
                    // who knows what happened - abort the processing of alerts
                    LOG.Error("Unexpected error processing alert events: ", e);
                    data.AlertsWritten = true;
                }
            }
        }

        private void UpdateStatus(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            using (LOG.InfoCall("UpdateStatus"))
            {
                try
                {
                    using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
                    {
                        // Management Service Memory Issue: Get Cached Status Document and move the connection open statment to bottom portion of the code.
                        // SQLDM-29437: Reverting SQLDM-28938 due the side effect of high amount of memory 
                        XmlDocument updateDocument = Management.ScheduledCollection.GetCachedStatusDocument(data.Message.MonitoredServer.Id);

                        if (updateDocument != null)
                        {
                            // we must be able to determine the timestamp of the snapshot
                            DateTime timestamp;
                            AlertableSnapshot refresh = (AlertableSnapshot)data.Message.Snapshot;
                            DateTime? serverTime = refresh.TimeStamp;
                            if (serverTime.HasValue)
                            {
                                timestamp = serverTime.Value;
                            }
                            else
                            {
                                IEvent firstEvent = refresh.GetEvent(0);
                                if (firstEvent == null)
                                {
                                    LOG.Info("Unable to determine refresh time - skipping update status for refresh");
                                    return;
                                }
                                timestamp = firstEvent.OccuranceTime;
                            }
                            // Management Service Memory Issue: Comment this code as we already collecting the document from cache and no need to replace/append nodes
                            //try
                            //{
                            //   Management.ScheduledCollection.UpdateStatusDocument(updateDocument);
                            //} catch (Exception e)
                            //{
                            //    LOG.ErrorFormat("Unable to update cached status document for {0}: {1} ", 
                            //        data.Message.MonitoredServer.InstanceName,
                            //        e.ToString());    
                            //}

                            // SQLDM-28317 (10.4.1-Sprint1): Initial code to sync instance status for Dashboard and Desktop
                            // Allow saving the status document for the maintenance mode
                            if (refresh.CollectionFailed ||
                                (refresh.ProductVersion == null ||
                                refresh.ProductVersion.Major <= 7 ||
                                refresh.TimeStamp == null) && 
                                !HasMaintenanceModeEvent(refresh.Events))
                                return;

                            // strip out the database nodes
                            XmlNodeList uselessNodes =
                                updateDocument.SelectNodes("/Servers/*/Category[@Name = \"Databases\"]/Database");
                            if (uselessNodes != null)
                            {
                                foreach (XmlNode uselessNode in uselessNodes)
                                {
                                    XmlNode parent = uselessNode.ParentNode;
                                    if (parent != null)
                                        parent.RemoveChild(uselessNode);
                                }
                            }

                            if (updateDocument.ChildNodes.Count > 0)
                            {
                                // poke in the collection date time 
                                XmlNode serversNode = updateDocument.ChildNodes[0];
                                if (serversNode.ChildNodes.Count > 0)
                                {
                                    XmlNode serverNode = serversNode.ChildNodes[0];
                                    XmlAttribute timeAttr = serverNode.Attributes["LastScheduledCollectionTime"];
                                    if (timeAttr == null)
                                    {
                                        timeAttr = updateDocument.CreateAttribute("LastScheduledCollectionTime");
                                        serverNode.Attributes.Append(timeAttr);
                                    }
                                    timeAttr.Value = XmlConvert.ToString(timestamp, "yyyy-MM-ddTHH:mm:ss.FFF");

                                    timeAttr = serverNode.Attributes["LastAlertRefreshTime"];
                                    if (timeAttr == null)
                                    {
                                        timeAttr = updateDocument.CreateAttribute("LastAlertRefreshTime");
                                        serverNode.Attributes.Append(timeAttr);
                                    }
                                    timeAttr.Value = XmlConvert.ToString(timestamp, "yyyy-MM-ddTHH:mm:ss.FFF");

                                    timeAttr = serverNode.Attributes["LastDatabaseRefreshTime"];
                                    if (timeAttr == null)
                                    {
                                        timeAttr = updateDocument.CreateAttribute("LastDatabaseRefreshTime");
                                        serverNode.Attributes.Append(timeAttr);
                                    }
                                    timeAttr.Value = XmlConvert.ToString(timestamp, "yyyy-MM-ddTHH:mm:ss.FFF");
                                }
                            }

                            connection.Open();
                            // store the document in the server activity table
                            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertServerActivity"))
                            {
                                SqlHelper.AssignParameterValues(command.Parameters,
                                                                refresh.Id,
                                                                timestamp,
                                                                updateDocument.OuterXml, // @StateOverview 
                                                                null,
                                                                // @SystemProcesses (saved when statistics were saved
                                                                null,
                                                                // @SessionList   (saved when statistics were saved
                                                                null,
                                                                // @LockStatistics   (saved when statistics were saved
                                                                null, // @LockList   (saved when statistics were saved
                                                                ((AlertableSnapshot)data.Message.Snapshot).SnapshotType
                                    );
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Exception trying to update the status document for {0}: {1}",
                                    data.Message.MonitoredServer.InstanceName,
                                    e.ToString());
                }
            }
        }

        /// <summary>
        /// Returns true if events contains a <see cref="Metric.MaintenanceMode"/> event
        /// </summary>
        /// <param name="events">events generated in refresh</param>
        /// <returns>
        /// True if <see cref="Metric.MaintenanceMode"/> found
        /// </returns>
        /// <remarks>
        /// SQLDM-28317 (10.4.1-Sprint1): code changes to sync instance status for Dashboard and Desktop
        /// </remarks>
        private bool HasMaintenanceModeEvent(IEnumerable<IEvent> events)
        {
            if (events != null)
            {
                for (var i = 0; i < events.Count(); i++)
                {
                    if (events.ElementAt(i).MetricID == (int)Metric.MaintenanceMode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

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
    }

    public class AlertTableWriter : IDisposable
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AlertTableWriter");

        private const int SubjectColumnLength = 256;
        private const int BodyColumnLength = 1024;

        private SqlTransaction transaction;
        private SqlConnection connection;
        private SqlCommand addAlertCommand;
        private MonitoredSqlServerState serverState;
        private MonitoredSqlServerStateGraph stateGraph;
        private AlertableSnapshot refresh;
        private IEnumerable<IEvent> events;
        private int eventsProcessed;
        private int eventsToSkip;
        private bool autoDispose;


        private AlertTableWriter()
        {
            eventsProcessed = 0;
            eventsToSkip = 0;
            autoDispose = true;
        }

        private AlertTableWriter(IEnumerable<IEvent> events) : this()
        {
            this.events = events;
        }


        private AlertTableWriter(AlertableSnapshot refresh, int eventsToSkip) : this()
        {
            this.refresh = refresh;
            this.events = this.refresh.Events;
            this.eventsToSkip = eventsToSkip;
        }

        private SqlTransaction GetTransaction()
        {
            // start a new transaction every time called
            if (transaction != null)
            {
                // handle case where transaction is still active
                LOG.Debug("GetTransaction() called when transaction not null.");
                transaction = null;
            }

            if (connection == null)
            {   // open the connection if necessary
                connection = ManagementServiceConfiguration.GetRepositoryConnection();
                this.autoDispose = true;
            }

            // ensure the connection is open before using it
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable, "AlertTableWriter");

            return transaction;
        }

        public static void LogThresholdViolations(SqlConnection connection, ScheduledCollectionQueue.ScheduledCollectionData data)
        {

            // if none of the alerts from the refresh have been added then try to bulk insert
            if (data.AlertRestartPoint == 0)
            {
                BulkAlertTableWriter.ProcessAlerts(connection, data);
                if (data.AlertsWritten)
                {
                    return;
                }
            }
            // if all alerts have not been written the n do it the old way - one by one
            LogThresholdViolations(connection, (AlertableSnapshot) data.Message.Snapshot, data.AlertRestartPoint);

        }

        public static void LogThresholdViolations(SqlConnection connection, AlertableSnapshot scheduledRefresh, int eventsToSkip)
        {
            if (scheduledRefresh != null)
            {
                AlertTableWriter writer = new AlertTableWriter(scheduledRefresh, eventsToSkip);
                try
                {
                    writer.connection = connection;
                    writer.autoDispose = false;
                    writer.LogThresholdViolations();
                }
                catch (Exception e)
                {
                    int numberProcessed = writer.eventsProcessed > writer.eventsToSkip
                                 ? writer.eventsProcessed
                                 : writer.eventsToSkip;

                    throw new AlertProcessingException(e, numberProcessed);
                }
                finally
                {
                    writer.Dispose();
                }
            }
        }

        public static void LogThresholdViolations(List<IEvent> events)
        {
            Management.QueueDelegate(delegate()
            {
                AlertTableWriter writer = new AlertTableWriter(events);
                try
                {
                    writer.LogThresholdViolations();
                } catch (Exception)
                {
                    /* */
                }
                finally
                {
                    writer.Dispose();
                }
            });
        }

        public static void LogOperationalAlerts(Metric metric, MonitoredObjectName name, MonitoredState severity, string heading, string message)
        {
            using (AlertTableWriter writer = new AlertTableWriter())
            {
                // serialize access to the alerts table
                lock (Management.ScheduledCollection.AlertTableSyncRoot)
                {
                    writer.InternalLogOperationalAlerts(metric, name, severity, heading, message);
                }
            }
        }

        private void InternalLogOperationalAlerts(Metric metric, MonitoredObjectName name, MonitoredState severity, string header, string body)
        {
            try
            {
                Transition transition = Transition.OK_Critical;
                if (severity == MonitoredState.Warning)
                    transition = Transition.OK_Warning;
                if (severity == MonitoredState.Informational)
                    transition = Transition.OK_Info;
                if (severity == MonitoredState.OK)
                    transition = Transition.Warning_OK;

                if (header.Length > SubjectColumnLength)
                    header = header.Substring(0, SubjectColumnLength);
                if (body.Length > BodyColumnLength)
                    body = body.Substring(0, BodyColumnLength);


                AddAlertLogRecord(DateTime.UtcNow, name.ServerName, name.DatabaseName, name.TableName, (int)metric, severity, transition, null, header, body, null, null);

                //AddAdvanceAlertFiletringLogRecord(DateTime.UtcNow, name.ServerName, (int)metric, severity);
            }
            catch (Exception e)
            {
                LOG.Error("Exception writing operational alerts to the alert log", e);
            }
        }

        public static double? ToDouble(object value)
        {
            double? result = null;

            try
            {
                if (value != null)
                    result = Convert.ToDouble(value);
            }
            catch (Exception)
            {
                LOG.ErrorFormat("Exception converting value to double: {0}", value.ToString());
            }
            return result;
        }

        private MonitoredSqlServerStateGraph GetStateGraph(int instanceId)
        {
            if (stateGraph == null || stateGraph.MonitoredSqlServer.WrappedServer.Id != instanceId)
            {
                serverState = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId);
                stateGraph = serverState != null ? serverState.StateGraph : null;
            }
            return stateGraph;
        }

        private int? GetMonitoredServerID(string instanceName)
        {
            MonitoredSqlServerState server = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceName);
            if (server != null)
                return server.WrappedServer.Id;
            return null;
        }

        public void LogThresholdViolations()
        {


            Exception innerException = null;
            DateTime? scheduledCollectionDateTime = null;

            if (events == null)
            {
                if (refresh != null)
                    LOG.Debug("Snapshot had no events to process...");
                return;
            }

            LOG.Debug("Writing threshold violations to Alerts table...");

            Transition transition = Transition.Warning_Warning;
            MonitoredState previousState = MonitoredState.None;

            IEvent baseEvent = null;
            bool stateChanged = false;

            MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();

            int debugStep = 0;
            int debugStep2 = 0;

            try
            {
                string refreshServerName = refresh != null
                                                 ? refresh.ServerName
                                                 : null;
                string monitoredServerName = refreshServerName;

                debugStep = 1;
                if (refresh != null)
                {
                    // figure out the timestamp for this scheduled collection
                    scheduledCollectionDateTime = refresh.TimeStamp;
                    if (!scheduledCollectionDateTime.HasValue)
                    {
                        IEvent firstEvent = refresh.GetEvent(0);
                        if (firstEvent != null)
                            scheduledCollectionDateTime = firstEvent.OccuranceTime;
                    }
                }
                debugStep = 2;

                foreach (IEvent ievent in events)
                {
                    if (BulkAlertTableWriter.AddAdvanceFilteringAlert(connection, monitoredServerName, ievent))
                    {
                        debugStep = 3;
                        debugStep2 = 0;
                        // bulk insert processing
                        if (eventsProcessed < eventsToSkip)
                        {   // skip event already processed
                            LOG.DebugFormat("Skipping event (already processed): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                            eventsProcessed++;
                            continue;
                        }
                        // write all events to the program log 
                        LOG.DebugFormat("Handling event: [{0}]{1}", monitoredServerName ?? "no server", ievent);

                        int metricID = ievent.MetricID;
                        Metric metric = MetricDefinition.GetMetric(metricID);

                        if (scheduledCollectionDateTime == null && metric == Metric.MaintenanceMode)
                        {
                            scheduledCollectionDateTime = ievent.OccuranceTime;
                        }
                        debugStep = 4;
                        MetricDefinition metricDefinition = metricDefinitions.GetMetricDefinition(metricID);
                        if (metricDefinition == null || metricDefinition.IsDeleted)
                        {   // metric definition must exist and not be marked as deleted
                            eventsProcessed++;
                            continue;
                        }
                        debugStep = 5;
                        CustomCounterDefinition counterDefinition = metricDefinitions.GetCounterDefinition(metricID);
                        if (counterDefinition != null && !counterDefinition.IsEnabled)
                        {   // counter definition (custom counters only) must be enabled
                            eventsProcessed++;
                            continue;
                        }

                        // get the threshold entry for this metric
                        AdvancedAlertConfigurationSettings advancedSettings = null;
                        debugStep = 6;
                        if (serverState != null)
                        {
                            MetricThresholdEntry thresholdEntry = serverState.GetMetricThresholdEntry(metricID);
                            if (thresholdEntry.Data == null)
                            {
                                advancedSettings = thresholdEntry.Data as AdvancedAlertConfigurationSettings;
                                if (advancedSettings == null)
                                    advancedSettings = new AdvancedAlertConfigurationSettings(metric, thresholdEntry.Data);
                            }
                        }
                        debugStep = 7;
                        if (advancedSettings != null)
                        {
                            SnoozeInfo snoozeInfo = advancedSettings.SnoozeInfo;
                            if (snoozeInfo != null && snoozeInfo.IsSnoozed(ievent.OccuranceTime))
                            {
                                LOG.DebugFormat("Skipping event (snoozed): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                                eventsProcessed++;
                                continue;
                            }
                        }

                        debugStep = 8;
                        stateChanged = ievent.StateChanged;

                        //TODO: Need to figure out how to work Informational state into this... 
                        //   I'm thinking that since there are so many combinations that a dictate how an event can 
                        //   change now, that we need to add Transition object to the ievent object so that we can determine
                        //   what the transition was for the event.
                        if (ievent is StateDeviationEvent)
                            previousState = MonitoredState.OK;
                        else
                        if (ievent is StateDeviationClearEvent)
                            previousState = ((StateDeviationClearEvent)ievent).DeviationEvent.MonitoredState;
                        else
                            previousState = ((StateDeviationUpdateEvent)ievent).PreviousState;

                        debugStep = 9;
                        // sanity check
                        if (!stateChanged && previousState == MonitoredState.OK)
                        {
                            LOG.DebugFormat("Skipping event (OK->OK): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                            eventsProcessed++;
                            continue;
                        }

                        double? value = null;

                        try
                        {
                            debugStep2 = 1;
                            baseEvent = ievent;
                            if (baseEvent != null)
                            {
                                //                                bool active = GetActive(baseEvent.MetricID, baseEvent.MonitoredState);
                                //                                if (!active)
                                //                                {
                                //                                    switch (baseEvent.MetricID)
                                //                                    {
                                //                                        case (int)Metric.BombedJobs:
                                //                                        case (int)Metric.LongJobs:
                                //                                            // these events do not add clearing entries to the alerts table
                                //                                            // they just need to get their active flags cleared when they expire.
                                //                                            nonClearingAlerts.Add(baseEvent);
                                //                                            stateChanged = false;
                                //                                            eventsProcessed++;
                                //                                            continue;
                                //                                    }
                                //                                    if (baseEvent is StateChangeEvent && ((StateChangeEvent) baseEvent).InvalidObject)
                                //                                    {
                                //                                            nonClearingAlerts.Add(baseEvent);
                                //                                            stateChanged = false;
                                //                                            eventsProcessed++;
                                //                                            continue;
                                //                                    }
                                //                                }

                                //                                if (!clearAll)
                                //                                    clearAll = GetClearAllAlerts(baseEvent.MetricID, baseEvent.MonitoredState);

                                MonitoredObjectName monitoredObject = baseEvent.MonitoredObject;
                                /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
                                value = ToDouble(baseEvent.MetricValue);
                                debugStep2 = 2;
                                if (baseEvent.AdditionalData is CustomCounterSnapshot)
                                {
                                    // add in the metric description info to the additional 
                                    // data if this is a custom counter snapshot
                                    baseEvent.AdditionalData =
                                        new Pair<CustomCounterSnapshot, MetricDescription?>(
                                            (CustomCounterSnapshot)baseEvent.AdditionalData,
                                            metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                                }

                                string header = null;
                                string body = null;

                                debugStep2 = 3;
                                MessageMap messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);
                                if (messageMap != null)
                                {
                                    header = messageMap.FormatMessage(refresh, baseEvent, MessageType.Header);
                                    body = messageMap.FormatMessage(refresh, baseEvent, MessageType.Body);
                                }
                                else
                                {
                                    header = "Unknown metric";
                                    body = "";
                                }

                                // make sure we stay within the size of the column
                                if (header.Length > SubjectColumnLength)
                                    header = header.Substring(0, SubjectColumnLength);
                                if (body.Length > BodyColumnLength)
                                    body = body.Substring(0, BodyColumnLength);

                                // Determine the transition of the alert
                                debugStep2 = 4;
                                if (baseEvent is StateDeviationEvent)
                                    transition = ((StateDeviationEvent)baseEvent).GetTransition();
                                else
                                    if (baseEvent is StateDeviationClearEvent)
                                    transition = ((StateDeviationClearEvent)baseEvent).GetTransition();
                                else
                                    transition = ((StateDeviationUpdateEvent)baseEvent).GetTransition();

                                debugStep2 = 5;
                                // now that we have the messages formatted - ensure the state graph gets updated 
                                if (stateGraph != null)
                                {
                                    if (ievent.MonitoredState == MonitoredState.OK || ievent is StateDeviationClearEvent)
                                        stateGraph.ClearStatus(ievent);
                                    else
                                        stateGraph.AddOrUpdateStatus(ievent, header);
                                }
                                debugStep2 = 6;
                                if (refreshServerName == null)
                                {
                                    if (metricDefinition.MetricClass != MetricClass.Processes)
                                    {
                                        monitoredServerName = monitoredObject.ServerName;
                                    }
                                }

                                string qualifierHash = monitoredObject.GetQualifierHash();
                                Guid? linkId = null;

                                if (baseEvent.MetricID == (int)Metric.Deadlock)
                                {
                                    DeadlockInfo deadlock = baseEvent.AdditionalData as DeadlockInfo;
                                    if (deadlock != null)
                                        linkId = deadlock.GetId();
                                }

                                if (baseEvent.MetricID == (int)Metric.BlockingAlert)
                                {
                                    var block = baseEvent.AdditionalData as BlockingSession;
                                    if (block != null)
                                    {
                                        linkId = block.BlockID;
                                    }
                                }

                                debugStep2 = 7;
                                AddAlertLogRecord(
                                    baseEvent.OccuranceTime,
                                    //monitoredObject.ServerName,
                                    monitoredServerName,
                                    monitoredObject.DatabaseName,
                                    monitoredObject.TableName,
                                    baseEvent.MetricID,
                                    baseEvent.MonitoredState,
                                    transition,
                                    value,
                                    header,
                                    body,
                                    qualifierHash,
                                    linkId);

                                eventsProcessed++;
                                //SQLdm 10.1 (GK): adding to the alert list for CWF sync
                                //AlertTableWriter.listOfAlertsToSyncWithCWF.Add(CreateCWFAlertObject(baseEvent, monitoredServerName, CommonWebFramework.GetInstance().ProductID, ievent.OccuranceTime, value, monitoredObject,body, metricDefinition.MetricCategory.ToString()));
                            }
                            //AddAdvanceAlertFiletringLogRecord(baseEvent.OccuranceTime, monitoredServerName, baseEvent.MetricID,  baseEvent.MonitoredState);


                        }
                        catch (Exception e)
                        {
                            LOG.ErrorFormat("Exception writing threshold violations to the alert log (debugStep2={0}) {1}", debugStep2, e);
                            innerException = e;
                            throw;
                        }
                    }
                }


                debugStep = 10;
                if (refresh != null)
                {
                    UpdateLastScheduledCollectionTime(refresh.Id, scheduledCollectionDateTime.HasValue ? scheduledCollectionDateTime.Value : DateTime.UtcNow, refresh.GetType());
                }
                else
                if (scheduledCollectionDateTime.HasValue)
                {
                    int? instanceId = GetMonitoredServerID(monitoredServerName);
                    if (instanceId != null)
                    {
                        UpdateLastScheduledCollectionTime(instanceId.Value,
                                                          scheduledCollectionDateTime.Value,
                                                          refresh != null ? refresh.GetType() : null);  // refresh can be null in certain situations
                    }
                }
            }
            catch (Exception e)
            {
                if (innerException == null) // log if it hasn't already been logged
                    LOG.ErrorFormat("Exception writing threshold violations to the alert log: (debugStep={0}) {1}", debugStep, e);
                throw;
            }
        }




        private void AddAlertLogRecord(DateTime dateTime, string server, string database, string table, int metricID, MonitoredState severity, Transition transition, double? value, string heading, string message, string qualifier, Guid? linkId)
        {
            SqlCommand command = GetAddAlertCommand();

            Exception innerException = null;

            bool rollback = false;
            try
            {
                SqlHelper.AssignParameterValues(
                    command.Parameters,
                    dateTime,
                    server,
                    database,
                    table,
                    metricID,
                    (byte) severity,
                    (byte) transition,
                    value.HasValue ? value.Value : (object) DBNull.Value,
                    heading,
                    message,
                    qualifier,
                    linkId.HasValue && linkId.Value != Guid.Empty ? linkId.Value : (object)DBNull.Value);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                rollback = true;
                innerException = e;
                throw e;
            }
            finally
            {
                try
                {
                    if (transaction != null)
                    {
                        if (rollback)
                            transaction.Rollback();
                        else
                            transaction.Commit();

                        if (autoDispose)
                        {
                            //SQlDM-28022 - Handling connection object to avoid leakage and Ensure connection is closed
                            if (command.Connection.State != ConnectionState.Closed)
                            {
                                command.Connection.Close();
                                command.Connection.Dispose();
                            }
                        }

                        transaction.Dispose();
                        transaction = null;
                    }
                }
                catch (Exception)
                {
                    transaction = null;
                    if (autoDispose)
                    {
                        connection = null;
                        using (command.Connection)
                        {
                            command.Connection.Close();
                        }
                    }
                    if (innerException == null)
                        throw;
                }
            }
        }

        private void AddAdvanceAlertFiletringLogRecord(DateTime OccuranceTime, string monitoredServerName, int MetricID, MonitoredState MonitoredState)
        {
            SqlCommand command = GetAddAdvanceAlertFilteringAlertCommand();

            Exception innerException = null;

            bool rollback = false;
            try
            {
                SqlHelper.AssignParameterValues(command.Parameters, OccuranceTime, monitoredServerName, MetricID, MonitoredState);
            }
            catch (Exception e)
            {
                rollback = true;
                innerException = e;
                throw e;
            }
            finally
            {
                try
                {
                    if (transaction != null)
                    {
                        if (rollback)
                            transaction.Rollback();
                        else
                            transaction.Commit();

                        if (autoDispose)
                        {
                            //SQlDM-28022 - Handling connection object to avoid leakage and Ensure connection is closed
                            if (command.Connection.State != ConnectionState.Closed)
                            {
                                command.Connection.Close();
                                command.Connection.Dispose();
                            }
                        }

                        transaction.Dispose();
                        transaction = null;
                    }
                }
                catch (Exception)
                {
                    transaction = null;
                    if (autoDispose)
                    {
                        connection = null;
                        using (command.Connection)
                        {
                            command.Connection.Close();
                        }
                    }
                    if (innerException == null)
                        throw;
                }
            }
        }

        private SqlCommand GetAddAlertCommand()
        {
            SqlTransaction xa = GetTransaction();

            if (addAlertCommand == null)
            {
                addAlertCommand = SqlHelper.CreateCommand(xa.Connection, "p_AddAlert");
            }

            // ensure the command is using the current connection
            if (addAlertCommand.Connection != xa.Connection)
                addAlertCommand.Connection = xa.Connection;

            // ensure the command is using the currect transaction
            if (addAlertCommand.Transaction != xa)
                addAlertCommand.Transaction = xa;

            return addAlertCommand;
        }

        private SqlCommand GetAddAdvanceAlertFilteringAlertCommand()
        {
            SqlTransaction xa = GetTransaction();

            //     if (addAlertCommand == null)
            //{
            addAlertCommand = SqlHelper.CreateCommand(xa.Connection, "p_AddAdvanceFilteringAlert");
            //  }

            // ensure the command is using the current connection
            if (addAlertCommand.Connection != xa.Connection)
                addAlertCommand.Connection = xa.Connection;

            // ensure the command is using the currect transaction
            if (addAlertCommand.Transaction != xa)
                addAlertCommand.Transaction = xa;

            return addAlertCommand;
        }

        internal void UpdateLastScheduledCollectionTime(int instanceId, DateTime collectionTime, Type refreshType)
        {
            using (LOG.InfoCall("UpdateLastScheduledCollectionTime"))
            {
                bool rollback = false;

                SqlTransaction xa = GetTransaction();
                try
                {
                    RepositoryHelper.UpdateLastRefreshTime(xa, instanceId, collectionTime, refreshType);
                } catch (Exception)
                {
                    rollback = true;
                    throw;
                } finally
                {
                    try
                    {
                        if (transaction != null)
                        {
                            if (rollback)
                                transaction.Rollback();
                            else
                                transaction.Commit();

                            if (autoDispose)
                            {
                                //SQlDM-28022 - Handling connection object to avoid leakage and Ensure connection is closed
                                if (xa.Connection.State != ConnectionState.Closed)
                                {
                                    xa.Connection.Close();
                                    xa.Connection.Dispose();
                                }
                                transaction.Dispose();
                                transaction = null;
                            }
                        }

                    }
                    catch (Exception)
                    {
                        transaction = null;
                        if (autoDispose)
                        {
                            if (connection != null)
                            {
                                try
                                {
                                    connection.Close();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            connection = null;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (addAlertCommand != null)
            {
                addAlertCommand.Dispose();
                addAlertCommand = null;
            }
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }
            if (connection != null && this.autoDispose)
            {
                connection.Dispose();
                connection = null;
            }

            refresh = null;
            events = null;
        }

        public static void HandleMaintenanceModeAlerts(MonitoredSqlServer instance)
        {
            using (LOG.DebugCall("HandleMaintenanceModeAlerts"))
            {
                ThresholdViolationEvent tve = null;

                DateTime now = DateTime.UtcNow;

                MonitoredObjectName mon = new MonitoredObjectName(instance.InstanceName, String.Empty, String.Empty);

                ThresholdViolationEvent anevent = PersistenceManager.Instance.GetOperationalEvent(Metric.MaintenanceMode, instance.Id);
                if (instance.MaintenanceModeEnabled)
                {
                    LOG.WarnFormat("Instance {0} put into maintenance mode", instance.InstanceName);
                    if (anevent == null)
                    {
                        MonitoredState state = MonitoredState.Informational;
                        // get the alert threshold for this instance
                        MetricThresholdEntry entry =
                            RepositoryHelper.GetMetricThreshold(ManagementServiceConfiguration.ConnectionString,
                                                                instance.Id, (int)Metric.MaintenanceMode);
                        if (entry != null)
                        {
                            if (!entry.IsEnabled)
                                return;
                            if (entry.WarningThreshold.Enabled)
                                state = MonitoredState.Warning;
                        }

                        tve = new ThresholdViolationEvent(mon, (int) Metric.MaintenanceMode, state, MonitoredState.OK, now, null);

                    }
                    PersistenceManager.Instance.AddUpdateOperationalEvent(Metric.MaintenanceMode,
                                                                          instance.Id,
                                                                          tve);
                } else
                {
                    LOG.WarnFormat("Instance {0} removed from maintenance mode", instance.InstanceName);
                    PersistenceManager.Instance.ClearOperationalEvent(Metric.MaintenanceMode, instance.Id);
                }


                List<IEvent> events = new List<IEvent>();
                if (tve != null)
                {
                    events.Add(new StateDeviationEvent(mon, (int) Metric.MaintenanceMode, tve.MonitoredState, OptionStatus.Enabled, now, instance));
                } else
                {
                    StateDeviationEvent sde = new StateDeviationEvent(
                        mon,
                        (int) Metric.MaintenanceMode,
                        anevent != null ? anevent.MonitoredState : MonitoredState.Critical,
                        OptionStatus.Disabled,
                        now,
                        instance);

                    events.Add(new StateDeviationClearEvent(sde, now));
                }

                try
                {
                    AlertTableWriter.LogThresholdViolations(events);
                } catch (Exception)
                {
                    LOG.Error("Error adding maintenance mode alert to the alert table continuing with notifications.");
                }
                Management.Notification.Process(events, null);
            }
        }

        public class AlertProcessingException : Exception
        {
            public readonly int NumberProcessed;
            public AlertProcessingException(Exception inner, int numberProcessed) :
                base(String.Format("Alert processing failed after processing {0} events.", numberProcessed), inner)
            {
                this.NumberProcessed = numberProcessed;
            }
        }

        // SQLDM - 26298 - Code changes to Update the AG Role change alerts
        public static void UpdateAGAlertLogRecord(DateTime dateTime, string server, int metricID, int MinutesAgeAlerts)
        {
            using (AlertTableWriter writer = new AlertTableWriter())
            {
                writer.UpdAGAlertLogRecord(dateTime, server, metricID, MinutesAgeAlerts);
            }
        }

        private void UpdAGAlertLogRecord(DateTime dateTime, string server, int metricID, int MinutesAgeAlerts)
        {
            SqlCommand command = GetUpdateAGAlertCommand();

            Exception innerException = null;

            bool rollback = false;
            try
            {
                SqlHelper.AssignParameterValues(
                    command.Parameters,
                    dateTime,
                    server,
                    metricID,
                    MinutesAgeAlerts);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                rollback = true;
                innerException = e;
                throw e;
            }
            finally
            {
                try
                {
                    if (rollback)
                        transaction.Rollback();
                    else
                        transaction.Commit();

                    transaction.Dispose();
                    transaction = null;
                }
                catch (Exception)
                {
                    transaction = null;
                    if (autoDispose)
                    {
                        connection = null;
                        using (command.Connection)
                        {
                            command.Connection.Close();
                        }
                    }
                    if (innerException == null)
                        throw;
                }
            }
        }

        private SqlCommand GetUpdateAGAlertCommand()
        {
            SqlTransaction xa = GetTransaction();
            SqlCommand updatAGAlertCommand = SqlHelper.CreateCommand(xa.Connection, "p_UpdateAGAlert");

            // ensure the command is using the current connection
            if (updatAGAlertCommand.Connection != xa.Connection)
                updatAGAlertCommand.Connection = xa.Connection;

            // ensure the command is using the currect transaction
            if (updatAGAlertCommand.Transaction != xa)
                updatAGAlertCommand.Transaction = xa;

            return updatAGAlertCommand;
        }
    }
}
