//------------------------------------------------------------------------------
// <copyright file="PersistenceManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Bamboo.Prevalence;
    using Bamboo.Prevalence.Attributes;
    using Bamboo.Prevalence.Configuration;
    using Bamboo.Prevalence.Util;
    using Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.ManagementService.Monitoring.Data;
    using Wintellect.PowerCollections;

    [Serializable]
    public class PersistenceManager : MarshalByRefObject, ISerializable
    {
        private delegate void Deque_Add<T>(Deque<T> deque, T value);
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("PersistenceManager");
        private static PersistenceManager instance = null;
        private static PrevalenceEngine persistenceEngine;
        private static SnapshotTaker persistenceSnapshoter;
        private static bool initialized = false;
        private const int serializedVersion = 6;

        private object sync = new object();

        private bool cleanlyShutdown;
        // persisted queue used by notification providers
        private Dictionary<Guid, Deque<NotificationContext>> notificationQueues;
        private Dictionary<Pair<Metric, object>,ThresholdViolationEvent> operationalEvents;
        private Dictionary<int, Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>> scheduledCollectionQueue;
        private Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> alertWriterQueue;
        private Guid? managementServiceID;

        private QMode queueMode;
        private Deque_Add<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queueAddMethod;

        static PersistenceManager()
        {
            InitializePersistance();
            if (persistenceEngine == null)
            {
                LOG.Error("Giving up on prevalence engine.  Using non-persistent object.");
                instance = new PersistenceManager();
            }
            else
            {
                instance = persistenceEngine.PrevalentSystem as PersistenceManager;
                persistenceSnapshoter =
                    new SnapshotTaker(persistenceEngine,
                        TimeSpan.FromMinutes(10),
                        Bamboo.Prevalence.Util.CleanUpAllFilesPolicy.Default);

                // for us to have gotten here we have either gotten a clean shutdown or reinitialized the cache
                // set the clean shutdown flag to false and do a syncpoint.
                SyncPointPrevalenceData(false);
            }
            initialized = true;
        }

        /// <summary>
        /// Don't construct this object on your own!  It is meant to be a singleton but
        /// its constructor is accessible so that the prevalence engine can use it.
        /// </summary>
        public PersistenceManager()
        {
            InitializeInstance();
        }

        [PassThrough]
        public void InitializeInstance()
        {
            notificationQueues = new Dictionary<Guid, Deque<NotificationContext>>();
            operationalEvents = new Dictionary<Pair<Metric, object>, ThresholdViolationEvent>();
            scheduledCollectionQueue = new Dictionary<int, Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>>();
            alertWriterQueue = new Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>();
            cleanlyShutdown = false;
            managementServiceID = null;

            // added in 5.6 to switch from FIFO to LIFO queue;
            queueMode = QMode.LIFO;
            queueAddMethod = Deque_AddToFront<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>;
        }

        #region ISerializable Members

        public PersistenceManager(SerializationInfo info, StreamingContext context)
        {
            // save the serialized version to the 
            int objectVersion = -1;
            try
            {
                objectVersion = info.GetInt32("serializedVersion");
            } catch (Exception)
            {
                /* we'll upchuck later if needed */
            }

            if (objectVersion != serializedVersion)
            {
                throw new SerializationException(
                    String.Format("The serialized object version ({0}) is not compatible with this version ({1}) of PersistenceManager.",
                        objectVersion,
                        serializedVersion));
            }
            cleanlyShutdown = info.GetBoolean("cleanlyShutdown");
            notificationQueues = (Dictionary<Guid,Deque<NotificationContext>>)
                info.GetValue("notificationQueues", typeof(Dictionary<Guid,Deque<NotificationContext>>));
            operationalEvents = (Dictionary<Pair<Metric,object>,ThresholdViolationEvent>)
                info.GetValue("operationalEvents", typeof(Dictionary<Pair<Metric,object>,ThresholdViolationEvent>));
            scheduledCollectionQueue = (Dictionary<int,Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>>)
                info.GetValue("scheduledCollectionQueue", typeof(Dictionary<int,Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>>));
            alertWriterQueue = (Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>)
                               info.GetValue("alertWriterQueue", typeof(Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>));
            managementServiceID = Serialized<object>.TryGetSerializedStruct<Guid>(info, "managementServiceID");

            try
            {
                queueMode = (QMode)info.GetValue("queueMode", typeof(QMode));
            }
            catch (Exception)
            {
                queueMode = QMode.FIFO; 
            }

            if (queueMode == QMode.LIFO)
                queueAddMethod = Deque_AddToFront<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>;
            else
                queueAddMethod = Deque_AddToBack<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>;
        }

        [PassThrough]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // save the serialized version to the 
            info.AddValue("serializedVersion", serializedVersion);
            info.AddValue("cleanlyShutdown", cleanlyShutdown);
            info.AddValue("notificationQueues", notificationQueues);
            info.AddValue("operationalEvents", operationalEvents);
            info.AddValue("scheduledCollectionQueue", scheduledCollectionQueue);
            info.AddValue("alertWriterQueue", alertWriterQueue);
            info.AddValue("managementServiceID", managementServiceID);
            info.AddValue("queueMode", queueMode);
        }

        #endregion

        /// <info>
        /// The queue was changed from FIFO to LIFO in 5.6.  In order to 
        /// sequence stuff correctly after upgrage, this method needs to 
        /// get called after converting the queue object.  It also needs 
        /// to get replayed so that new stuff added gets replayed into the 
        /// right spot.
        /// </info>
        public void UpgradeScheduledDataSendQueue()
        {
            if (queueMode != QMode.LIFO)
            {
                using (LOG.InfoCall("UpgradeScheduledDataSendQueue"))
                {
                    // switch to LIFO mode
                    queueMode = QMode.LIFO;
                    queueAddMethod = Deque_AddToFront<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>;

                    lock (alertWriterQueue)
                    {
                        // reverse the order of the alert writer queue
                        ScheduledCollectionQueue.ScheduledCollectionDataWrapper[] items = alertWriterQueue.ToArray();
                        alertWriterQueue.Clear();
                        alertWriterQueue.AddManyToFront(items);
                    }
                    lock (scheduledCollectionQueue)
                    {
                        // reverse the order of the scheduled collection data queues
                        foreach (
                            Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> deque in
                                scheduledCollectionQueue.Values)
                        {
                            lock (deque)
                            {
                                ScheduledCollectionQueue.ScheduledCollectionDataWrapper[] items =
                                    alertWriterQueue.ToArray();
                                deque.Clear();
                                deque.AddManyToFront(items);
                            }
                        }
                    }
                }
            }
        }

        [PassThrough]
        private static void Verify()
        {
            foreach (string fname in Directory.GetFiles(GetPrevalenceDirectory(), "*.commandlog"))
            {
                Verify(fname);
            }
        }

        [PassThrough]
        private static void Verify(string fname)
        {
            using (FileStream stream = File.OpenRead(fname))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                while (stream.Position < stream.Length)
                {
                    long position = stream.Position;

                    try
                    {
                        object command = formatter.Deserialize(stream);
                        if (null != command)
                        {
                            LOG.VerboseFormat("{0} command successfully deserialized.", command);
                        }
                        else
                            LOG.Verbose(command.ToString());
                    }
                    catch (Exception x)
                    {
                        LOG.ErrorFormat("{0}:{1}: {2}", fname, position, x);
                        break;
                    }
                }
            }
        }

        [PassThrough]
        private static void InitializePersistance()
        {
            using (LOG.InfoCall( "Initialize"))
            {
                PrevalenceSettings.FlushAfterCommand = false;
                
                // debug code to log the contents of the .commandlog files
                // Verify();

                bool valid = CreatePrevalenceEngine(); 
                if (valid)
                {
                    PersistenceManager pm = persistenceEngine.PrevalentSystem as PersistenceManager;
                    if (pm == null || !pm.GetCleanlyShutdown())
                    {
                        valid = false;   
                    }
                }
                if (!valid)
                {
                    // assume failure due to corrupt data
                    string datapath = GetPrevalenceDirectory();
                    try
                    {
                        LOG.DebugFormat("Clearing prevalence engine directory: {0}", datapath);
                        Directory.Delete(datapath, true);
                    }
                    catch (Exception e)
                    {
                        LOG.DebugFormat("Exception clearing prevalence engine directory", e);
                    }
                    LOG.Debug("Retry prevalence engine instantiation");
                    CreatePrevalenceEngine();
                }
            }
        }

        [PassThrough]
        private static bool CreatePrevalenceEngine()
        {
            bool result = false;
            using (LOG.InfoCall( "CreatePrevalenceEngine"))
            {
                string datapath = GetPrevalenceDirectory();
                try
                {
                    persistenceEngine =
                        PrevalenceActivator.CreateTransparentEngine(typeof(PersistenceManager), datapath);
                    result = persistenceEngine != null && persistenceEngine.PrevalentSystem is PersistenceManager;
                    if (!result)
                        LOG.Error("Persistence engine is null or not wrapping the expected type");
                }
                catch (Exception)
                {
                    LOG.Error("Exception initializing prevalence engine");
                }
            }
            return result;
        }

        [PassThrough]
        public void ReinitializePersistence()
        {
            using (LOG.InfoCall("ReinitializePersistence"))
            {
                lock (sync)
                {
                    // Syncpoint to get a consistent state
                    SyncPointPrevalenceData(false);
                    // delete all persistence data
                    string datapath = GetPrevalenceQueueDirectory();
                    LOG.DebugFormat("Clearing queue serialization directory: {0}", datapath);
                    WipeDirectory(datapath);
                    // clear in-memory persisted data
                    InitializeInstance();
                    // Syncpoint again to persist the reinitialized persistence object
                    SyncPointPrevalenceData(false);
                }
            }
        }

        [PassThrough] 
        public static void WipePersistenceDirectories()
        {
            string datapath = GetPrevalenceDirectory();
            LOG.DebugFormat("Clearing prevalence engine directory: {0}", datapath);
            if (WipeDirectory(datapath))
            {
                datapath = GetPrevalenceQueueDirectory();
                LOG.DebugFormat("Clearing queue serialization directory: {0}", datapath);
                WipeDirectory(datapath);
            }
        }

        [PassThrough]
        private static bool WipeDirectory(string path)
        {
            try
            {
                if(Directory.Exists(path)) Directory.Delete(path, true);
                return true;
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error wiping directory '{0}' - {1}", path, e.Message);
            }
            return false;
        }

        [PassThrough]
        private static string GetPrevalenceDirectory()
        {
            return Path.Combine(ManagementServiceConfiguration.DataPath, "Cache");
        }

        [PassThrough]
        private static string GetPrevalenceQueueDirectory()
        {
            return Path.Combine(ManagementServiceConfiguration.DataPath, "Queued");
        }

        /// <summary>
        /// Force prevalence to clean up after itself so that it has a 
        /// shorter startup time the next time the service starts.
        /// </summary>
        [PassThrough]
        public static void SyncPointPrevalenceData(bool cleanlyShutdown)
        {
            if (persistenceEngine != null)
            {
                try
                {
                    // update the clean shutdown flag
                    instance.SetCleanlyShutdown(cleanlyShutdown);

                    // serialize the current state to disk
                    persistenceEngine.TakeSnapshot();
                    // select and delete old snapshots and log files
                    FileInfo[] uselessFiles = CleanUpAllFilesPolicy.Default.SelectFiles(persistenceEngine);
                    foreach (FileInfo uselessFile in uselessFiles)
                    {
                        uselessFile.Delete();
                    }
                } catch (Exception e)
                {
                    LOG.Error("Error cleaning up cached objects: " + e.Message); 
                }
            }
        }

        [PassThrough]
        public static PersistenceManager Instance
        {
            get { return instance; }
        }

        [Query]
        public bool GetCleanlyShutdown()
        {
            return cleanlyShutdown;  
        }

        public void SetCleanlyShutdown(bool value)
        {
            cleanlyShutdown = value;
        }

        [Query]
        public Guid? GetManagementServiceID()
        {
            return managementServiceID;
        }

        public void SetManagementServiceID(Guid? id)
        {
            managementServiceID = id;    
        }

        [Query]
        public IQueue<NotificationContext> GetNotificationQueue(Guid notificationProviderId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (!notificationQueues.TryGetValue(notificationProviderId, out queue))
                {
                    queue = new Deque<NotificationContext>();
                    notificationQueues.Add(notificationProviderId, queue);
                }
            }
            return new NotificationQueueHelper(notificationProviderId);
        }

        [Query]
        public int GetNotificationQueueLength(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    return queue.Count;
                }
            }
            return 0;
        }

        [Query]
        public NotificationContext PeekNotificationItem(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    if (queue.Count > 0)
                    {
                        return queue.GetAtFront();
                    }
                }
            }
            return null;
        }

        [Query] 
        public NotificationContext[] PeekNotificationItems(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    return queue.ToArray();
                }
            }
            return null;
        }

        public NotificationContext DequeueNotificationItem(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    if (queue.Count > 0)
                    {
                        return queue.RemoveFromFront();
                    }
                }
            }
            return null;
        }

        public void EnqueueNotificationItem(Guid queueId, NotificationContext item)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (!notificationQueues.TryGetValue(queueId, out queue))
                {
                    queue = new Deque<NotificationContext>();
                    notificationQueues.Add(queueId, queue);
                }
                queue.Add(item);
            }
        }

        public void EnqueueNotificationItems(List<NotificationContext> items)
        {
            List<Guid> providers = new List<Guid>();
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                foreach (NotificationContext item in items)
                {
                    Guid queueId = item.Destination.ProviderID;
                    if (!notificationQueues.TryGetValue(queueId, out queue))
                    {
                        queue = new Deque<NotificationContext>();
                        notificationQueues.Add(queueId, queue);
                    }
                    if (queue.Count == 0 && !providers.Contains(queueId))
                        providers.Add(queueId);

                    queue.Add(item);
                }
                if (initialized)
                {
                    foreach (Guid id in providers)
                    {
                        Management.Notification.TryStartProvider(id);
                    }
                }
            }
        }

        public NotificationContext[] DequeueNotificationItems(Guid queueId, int count)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    // dequeue smaller of queue count or the number requested
                    int limit = Math.Min(count, queue.Count);
                    
                    NotificationContext[] result = new NotificationContext[limit];
                    queue.CopyTo(0, result, 0, limit);
                    // remove the copied items from the queue
                    queue.RemoveRange(0, limit);
                }
            }
            return null;
        }

        public void ClearQueue(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (sync)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    queue.Clear();
                }
            }
        }

        [Query]
        public ThresholdViolationEvent GetOperationalEvent(Metric metric, object id)
        {
            return GetOperationalEvent(new Pair<Metric,object>(metric,id));    
        }

        [Query]
        public ThresholdViolationEvent GetOperationalEvent(Pair<Metric,object> key)
        {
            ThresholdViolationEvent result = null;
            operationalEvents.TryGetValue(key, out result);
            return result;
        }

        public bool AddUpdateOperationalEvent(Metric metric, object id, ThresholdViolationEvent anevent)
        {
            return AddUpdateOperationalEvent(new Pair<Metric,object>(metric,id), anevent);       
        }
            
        public bool AddUpdateOperationalEvent(Pair<Metric, object> key, ThresholdViolationEvent anevent)
        {
            bool result = false;
            if (operationalEvents.ContainsKey(key))
            {
                result = true;
                operationalEvents.Remove(key);
            }
            operationalEvents.Add(key, anevent);
            return result;
        }

        public void ClearOperationalEvent(Metric metric, object id)
        {
            ClearOperationalEvent(new Pair<Metric, object>(metric, id));
        }

        public void ClearOperationalEvent(Pair<Metric, object> key)
        {
            if (operationalEvents.ContainsKey(key))
            {
                operationalEvents.Remove(key);
            }
        }

        [Query]
        public List<int> GetServersWithWaitingScheduledCollectionData()
        {
            List<int> result = new List<int>();

            foreach (int serverId in scheduledCollectionQueue.Keys)
            {
                ICollection collection = scheduledCollectionQueue[serverId];
                if (collection.Count > 0)
                    result.Add(serverId);
            }

            return result;
        }

        public ScheduledCollectionQueue.ScheduledCollectionData DequeueScheduledCollectionDataMessage(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queue = null;
            if (scheduledCollectionQueue.TryGetValue(instanceId, out queue)) 
            {
                if (queue.Count > 0)
                {
                    ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapped = queue.RemoveFromFront();
                    return wrapped.Message;
                }
            }

            return null;
        }

        [Query]
        public ScheduledCollectionQueue.ScheduledCollectionData PeekScheduledCollectionDataMessage(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queue = null;
            if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
            {
                if (queue.Count > 0)
                {
                    ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapped = queue.GetAtFront();
                    return wrapped.Message;
                }
            }

            return null;
        }

        [Query]
        public int GetScheduledCollectionDataMessageCount(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queue = null;
            if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
            {
                return queue.Count;
            }
            return 0;
        }

        [PassThrough]
        public ScheduledCollectionQueue.ScheduledCollectionDataWrapper WrapMessage(ScheduledCollectionDataMessage message)
        {
            ScheduledCollectionQueue.ScheduledCollectionData data =
                new ScheduledCollectionQueue.ScheduledCollectionData(message);

            ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapper =
                new ScheduledCollectionQueue.ScheduledCollectionDataWrapper(data);

            try
            {
                if (scheduledCollectionQueue.Count > 10)
                    wrapper.Store();
            }
            catch (Exception e)
            {
                LOG.Error("Error storing scheduled collection data message", e);
            }
            return wrapper;
        }

        public void EnqueueScheduledCollectionDataMessage(int instanceId, ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapper)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queue = null;
            if (!scheduledCollectionQueue.TryGetValue(instanceId, out queue))
            {
                queue = new Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper>();
                scheduledCollectionQueue.Add(instanceId, queue);
            }
            // use delegate to add the queue entry
            queueAddMethod(queue, wrapper);
        }

        [Query]
        public int GetAlertWriterDataMessageCount()
        {
            return alertWriterQueue.Count;
        }

        /// <summary>
        /// Removes the first entry in the scheduled collection queue for the server and 
        /// adds it to the alert writer queue.
        /// </summary>
        /// <param name="instanceId"></param>
        public void MoveScheduledCollectionQueueEntryToAlertWriterQueue(int instanceId)
        {
            using (LOG.DebugCall("MoveScheduledCollectionQueueEntryToAlertWriterQueue"))
            {
                Deque<ScheduledCollectionQueue.ScheduledCollectionDataWrapper> queue = null;
                if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    if (queue.Count > 0)
                    {
                        ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapped = queue.RemoveFromFront();
                        if (wrapped != null)
                        {
                            // reset the number of processing attempts to zero
                            try
                            {
                                wrapped.Message.ProcessingAttempts = 0;
                                // start persisting to disk if the queue is getting backed up
                                if (alertWriterQueue.Count > 10)
                                {
                                    try
                                    {
                                        wrapped.Store();
                                    }
                                    catch (Exception)
                                    {
                                        /* */
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                // more than likely it failed trying to deserialize wrapped.Message.  Allow it to 
                                // continue.  Maybe the alert processing code will be a little more graceful in handling
                                // the exception it if fails again.
                                LOG.ErrorFormat("Error resetting processing attempts to zero or storing wrapper guts (allow alert processing deal with the issue (instanceId={0})): {1}", instanceId, e);
                            }
                            // use delegate to add queue entry
                            queueAddMethod(alertWriterQueue, wrapped);
                        }
                    }
                }
            }
        }

        public ScheduledCollectionQueue.ScheduledCollectionData DequeueAlertWriterDataMessage()
        {
            if (alertWriterQueue.Count > 0)
            {
                ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapped = alertWriterQueue.RemoveFromFront();
                return wrapped.Message;
            }

            return null;
        }

        public void Internal_DequeueAlertWriterDataMessage()
        {
            using (LOG.WarnCall("Internal_DequeueAlertWriterDataMessage"))
            {
                if (alertWriterQueue.Count > 0)
                {
                    alertWriterQueue.RemoveFromFront();
                    LOG.Warn("Removing first item from the alert writer queue");
                }
                else
                    LOG.Warn("Alert writer queue is empty");
            }
        }

        [Query]
        public ScheduledCollectionQueue.ScheduledCollectionData PeekAlertWriterDataMessage()
        {
            using (LOG.DebugCall("PeekAlertWriterDataMessage"))
            {
                if (alertWriterQueue.Count > 0)
                {
                    ScheduledCollectionQueue.ScheduledCollectionDataWrapper wrapped = alertWriterQueue.GetAtFront();
                    try
                    {
                        return wrapped.Message;
                    }
                    catch (Exception e)
                    {
                        // probably a deserialization issue
                        LOG.ErrorFormat("Error meterializing alert writer message (Peek count={0}) {1}: ",
                                        alertWriterQueue, e);

                        Internal_DequeueAlertWriterDataMessage();
                    }
                }
                return null;
            }
        }

        [PassThrough]
        private static void Deque_AddToFront<T>(Deque<T> deque, T item)
        {
            deque.AddToFront(item);
        }

        [PassThrough]
        private static void Deque_AddToBack<T>(Deque<T> deque, T item)
        {
            deque.AddToBack(item);
        }

        public void SetScheduledCollectionDataProcessingAttempts(ScheduledCollectionQueue.ScheduledCollectionDataState state, int value)
        {
            state.ProcessingAttempts = value;
        }
        public void SetScheduledCollectionDataStatisticsWritten(ScheduledCollectionQueue.ScheduledCollectionDataState state, bool value)
        {
            state.StatisticsWritten = value;
        }
        public void SetScheduledCollectionDataAlertsWritten(ScheduledCollectionQueue.ScheduledCollectionDataState state, bool value)
        {
            state.AlertsWritten = value;
        }
        public void SetScheduledCollectionDataAlertRestartPoint(ScheduledCollectionQueue.ScheduledCollectionDataState state, int value)
        {
            state.AlertRestartPoint = value;
        }

        public void SetScheduledCollectionDataNotificationsProcessed(ScheduledCollectionQueue.ScheduledCollectionDataState state, bool value)
        {
            state.NotificationsProcessed = value;
        }

        public sealed class NotificationQueueHelper : IQueue<NotificationContext>
        {
            private Guid providerId;

            public NotificationQueueHelper(Guid providerId)
            {
                this.providerId = providerId;
            }

            public int Count
            {
                get { return PersistenceManager.Instance.GetNotificationQueueLength(providerId); }
            }

            public void Clear()
            {
                PersistenceManager.Instance.ClearQueue(providerId);
            }

            public NotificationContext Dequeue()
            {
                return PersistenceManager.Instance.DequeueNotificationItem(providerId);
            }

            public void Enqueue(NotificationContext item)
            {
                PersistenceManager.Instance.EnqueueNotificationItem(providerId, item);
            }

            public NotificationContext Peek()
            {
                return PersistenceManager.Instance.PeekNotificationItem(providerId);
            }

            public NotificationContext[] PeekAll()
            {
                return PersistenceManager.Instance.PeekNotificationItems(providerId);
            }

            public NotificationContext[] Dequeue(int count)
            {
                return PersistenceManager.Instance.DequeueNotificationItems(providerId, count);
            }
        }
    }
}
