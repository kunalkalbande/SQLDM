//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common.Services;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using Idera.SQLdm.ManagementService.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using Wintellect.PowerCollections;
using Idera.SQLdm.ManagementService.Helpers;
using System.Threading;

namespace Idera.SQLdm.ManagementService.Monitoring.Data
{
    /// <summary>
    /// Scheduled collection enqueues all collected data to this queue.  This queue
    /// is backed by bamboo to ensure consistency.  When the queue fills to a predetermined
    /// level it will start serializing contents to disk to reduce the amount of memory
    /// being consumed.
    /// </summary>
    [Serializable]
    public class ScheduledCollectionQueue
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionQueue");

        private const int STREAM_TO_DISK_THRESHOLD = 10;

        private ScheduledCollectionQueues collectionQueues;
//        private PersistenceManager persistenceManagerInstance; 
        private Deque<int> waitingQueues;
        private Set<int> inuseQueues;

        public readonly object SyncRoot = new object();
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ScheduledCollectionDataSendQueue"/> class.
        /// </summary>
        public ScheduledCollectionQueue(ScheduledCollectionQueues queues)
        {
            collectionQueues = queues;
//            persistenceManagerInstance = PersistenceManager.Instance;
            waitingQueues = new Deque<int>();
            inuseQueues = new Set<int>();
            count = 0;

            List<int> serversWithData = 
                collectionQueues.GetServersWithWaitingScheduledCollectionData();

            waitingQueues.AddManyToFront(serversWithData);
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return count;
                }
            }
        }

        public int WaitingQueueCount
        {
            get
            {
                lock (SyncRoot)
                {
                    return waitingQueues.Count;
                }
            }
        }

        public void Enqueue(int instanceId, ScheduledCollectionDataMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            lock (SyncRoot)
            {
                
                // add the message to the persistent queue
                collectionQueues.EnqueueScheduledCollectionMessage(instanceId, message);
           //     persistenceManagerInstance.EnqueueScheduledCollectionDataMessage(instanceId, wrapper);
                // if the queue is not already in use then flag is as having data
                SetQueueWaiting(instanceId);
            }            
        }

        /// <summary>
        /// Enqueues the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Enqueue(int instanceId, ScheduledCollectionData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            lock (SyncRoot)
            {
                // add the message to the persistent queue
                collectionQueues.EnqueueScheduledCollectionData(instanceId, data);
      //          persistenceManagerInstance.EnqueueScheduledCollectionDataMessage(instanceId, wrapper);
                // if the queue is not already in use then flag is as having data
                SetQueueWaiting(instanceId);
            }
        }

        private void SetQueueWaiting(int instanceId)
        {
            if (!inuseQueues.Contains(instanceId) && !waitingQueues.Contains(instanceId))
            {
                waitingQueues.AddToBack(instanceId);
                Monitor.Pulse(SyncRoot);
            }
        }

        public int ReserveFirstWaitingQueue()
        {
            int result = -1;
            lock (SyncRoot)
            {
                while (result == -1 && waitingQueues.Count > 0) 
                {
                    result = waitingQueues.RemoveFromFront();
                    if (inuseQueues.Contains(result))
                    {
                        LOG.Debug("Waiting queue appears to be in use");
                        result = -1;
                    }
                }

                if (result == -1)
                    throw new InvalidOperationException("No free queue found to reserve");

                inuseQueues.Add(result);
            }

            return result;
        }

        public void Release(int instanceId)
        {
            lock (SyncRoot)
            {
                if (!inuseQueues.Contains(instanceId))
                    throw new InvalidOperationException("Queue is not reserved");

                inuseQueues.Remove(instanceId);

                int queueCount = collectionQueues.GetScheduledCollectionDataMessageCount(instanceId);
      //          int queueCount = persistenceManagerInstance.GetScheduledCollectionDataMessageCount(instanceId);
                if (queueCount > 0)
                    SetQueueWaiting(instanceId);
            }
        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionData Dequeue(int instanceId)
        {
            ScheduledCollectionData message = null;
            lock (SyncRoot)
            {
                if (!inuseQueues.Contains(instanceId))
                    throw new InvalidOperationException("Queue has not been reserved");

                message = collectionQueues.DequeueScheduledCollectionDataMessage(instanceId);
//                persistenceManagerInstance.DequeueScheduledCollectionDataMessage(instanceId);
            }
            return message;
        }

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionData Peek(int instanceId)
        {
            ScheduledCollectionData data = null;
            lock (SyncRoot)
            {
                if (!inuseQueues.Contains(instanceId))
                    throw new InvalidOperationException("Queue has not been reserved");

                try
                {
                    data = collectionQueues.PeekScheduledCollectionDataMessage(instanceId);
         //           data = persistenceManagerInstance.PeekScheduledCollectionDataMessage(instanceId);
                }
                catch (Exception e)
                {
                    LOG.Error("Unable to dequeue (peek) scheduled refresh snapshot (snapshot discarded):", e);
                }
            }
            return data;
        }

        public void Remove(int instanceId, ScheduledCollectionData item)
        {
            lock (SyncRoot)
            {
                if (!inuseQueues.Contains(instanceId))
                    throw new InvalidOperationException("Queue has not been reserved");

                try
                {
                    collectionQueues.RemoveScheduledCollectionDataMessage(instanceId, item);
                }
                catch (Exception)
                {
                    /* eat it */
                }
            }
        }

        [Serializable]
        public class ScheduledCollectionDataWrapper : ISerializable
        {
            private static DateTime fileControl = DateTime.UtcNow.Date;
            private static int fileSequence = 0;
            private object sync;

            private DateTime created;
            private ScheduledCollectionData message;
            private string filename;

            public ScheduledCollectionDataWrapper(ScheduledCollectionData message)
            {
                sync = new object();

                created = DateTime.Now;
                this.message = message;
                filename = null;
            }

            public ScheduledCollectionDataWrapper(SerializationInfo info, StreamingContext context)
            {
                sync = new object();
                created = info.GetDateTime("created");
                filename = info.GetString("filename");
                if (filename == null)
                    message = (ScheduledCollectionData)info.GetValue("message", typeof(ScheduledCollectionData));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                lock (sync)
                {
                    info.AddValue("created", created);
                    info.AddValue("message", message);
                    info.AddValue("filename", filename);
                }
            }

            private FileInfo GetNextFileName(string path)
            {
                DateTime now = DateTime.UtcNow.Date;
                if (now != fileControl)
                {
                    fileControl = now;
                    fileSequence = 0;
                }
                string name = null;
                do
                {
                    name = path + String.Format("\\{0:yyyyMMdd}{1:00000000}.bin", now, ++fileSequence);
                }
                while (File.Exists(name));

                return new FileInfo(name);
            }


            public void Store()
            {
                lock (sync)
                {
                    if (message == null)
                        return;

                    string dataPath = Path.Combine(ManagementServiceConfiguration.DataPath, "Queued");
                    if (!Directory.Exists(dataPath))
                        Directory.CreateDirectory(dataPath);

                    FileInfo file = GetNextFileName(dataPath);

                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fileStream = file.Create())
                    {
                        DeflateStream deflater = new DeflateStream(fileStream, CompressionMode.Compress, true);
                        BufferedStream stream = new BufferedStream(deflater);
                        formatter.Serialize(stream, message);
                        stream.Close();
                        deflater.Close();
                    }
                    filename = file.FullName;
                    message = null;
                }
            }

            public ScheduledCollectionData Message
            {
                get
                {
                    lock (sync)
                    {
                        if (message == null)
                            Load();

                        return message;
                    }
                }
            }

            private void Load()
            {
                if (filename == null)
                    return;

                FileInfo file = new FileInfo(filename);
                if (file.Exists)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fileStream = new FileInfo(filename).OpenRead())
                    {
                        BufferedStream stream = new BufferedStream(fileStream);
                        DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress);
                        message = (ScheduledCollectionData)formatter.Deserialize(inflater);
                        inflater.Close();
                        stream.Close();
                    }
                    file.Delete();
                }
                filename = null;
            }
        }

        [Serializable]
        public class ScheduledCollectionDataState
        {
            private bool statisticsWritten;
            private bool alertsWritten;
            private bool notificationsProcessed;
            private int attempts;
            private int alertRestartPoint;

            public int ProcessingAttempts
            {
                get { return attempts; }
                set { attempts = value; }
            }
            public bool StatisticsWritten
            {
                get { return statisticsWritten; }
                set
                {
                    statisticsWritten = value;
                    if (value)
                        attempts = 0;
                }
            }
            public bool AlertsWritten
            {
                get { return alertsWritten; }
                set
                {
                    alertsWritten = value;
                    if (value)
                        attempts = 0;
                }
            }
            public int AlertRestartPoint
            {
                get { return alertRestartPoint; }
                set { alertRestartPoint = value; }
            }

            public bool NotificationsProcessed
            {
                get { return notificationsProcessed; }
                set
                {
                    notificationsProcessed = value;
                    if (value)
                        attempts = 0;
                }
            }
        }


        [Serializable]
        public class ScheduledCollectionData
        {
            public readonly ScheduledCollectionDataMessage Message;
            private ScheduledCollectionDataState state;

            public ScheduledCollectionData(ScheduledCollectionDataMessage message)
            {
                Message = message;
                state = new ScheduledCollectionDataState();
            }

            public int ProcessingAttempts
            {
                get { return state.ProcessingAttempts; }
                set { state.ProcessingAttempts = value; }
            }
            public bool StatisticsWritten
            {
                get { return state.StatisticsWritten; }
                set { state.StatisticsWritten = value; }
            }
            public bool AlertsWritten
            {
                get { return state.AlertsWritten; }
                set { state.AlertsWritten = value; }
            }
            public int AlertRestartPoint
            {
                get { return state.AlertRestartPoint; }
                set { state.AlertRestartPoint = value; ; }
            }

            public bool NotificationsProcessed
            {
                get { return state.NotificationsProcessed; }
                set { state.NotificationsProcessed = value; }
            }
        }


    }
}
