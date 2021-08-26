//------------------------------------------------------------------------------
// <copyright file="AlertWriterQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.ManagementService.Monitoring.Data
{
    using System.Threading;
    using Helpers;
    using Idera.SQLdm.Common.Services;
    using Wintellect.PowerCollections;

    public class AlertWriterQueue
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionQueue");

        private const int STREAM_TO_DISK_THRESHOLD = 10;

 //       private PersistenceManager persistenceManagerInstance; 

        private ScheduledCollectionQueues collectionQueues;

        public readonly object SyncRoot = new object();

        public AlertWriterQueue(ScheduledCollectionQueues collectionQueues)
        {
//            persistenceManagerInstance = PersistenceManager.Instance;
            this.collectionQueues = collectionQueues;
        }

        public void AddScheduledRefresh(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            lock (SyncRoot)
            {
                data.ProcessingAttempts = 0;
                collectionQueues.EnqueueAlertWriterDataMessage(data);
//                persistenceManagerInstance.MoveScheduledCollectionQueueEntryToAlertWriterQueue(instanceId);
                Monitor.Pulse(SyncRoot);
            }
        }

        public void RequeueScheduledRefresh(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            lock (SyncRoot)
            {
                collectionQueues.EnqueueAlertWriterDataMessage(data);
                Monitor.Pulse(SyncRoot);
            }
        }

        public int Count
        {
            get
            {
                return collectionQueues.GetAlertWriterDataMessageCount();             
//                return persistenceManagerInstance.GetAlertWriterDataMessageCount();
            }
        }


        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionQueue.ScheduledCollectionData Dequeue()
        {
//            ScheduledCollectionQueue.ScheduledCollectionData data = null;
//               data = persistenceManagerInstance.DequeueAlertWriterDataMessage();
            return collectionQueues.DequeueAlertWriterDataMessage();
        }

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns></returns>
        public ScheduledCollectionQueue.ScheduledCollectionData Peek()
        {
//            ScheduledCollectionQueue.ScheduledCollectionData data = null;
//            lock (SyncRoot)
//            {
//                data = persistenceManagerInstance.PeekAlertWriterDataMessage();
//            }
            return collectionQueues.PeekAlertWriterDataMessage();
        }

    }
}
