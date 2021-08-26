//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionQueues.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System.Collections.Generic;
    using Configuration;
    using Data;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Wintellect.PowerCollections;

    public class ScheduledCollectionQueues
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionQueue");
        private static int MaxQueueLengthPerServer = 5;

        // stuff for scheduled collection data
        private Dictionary<int, Deque<ScheduledCollectionQueue.ScheduledCollectionData>> scheduledCollectionQueue;
        private object scheduledCollectionQueueLock = new object();
        private int scheduledCollectionQueueCount;

        // stuff flows from the scheduled collection queue to the alert writer queue
        private Deque<ScheduledCollectionQueue.ScheduledCollectionData> alertWriterQueue;
        private object alertWriterQueueLock = new object();

        // stuff flows from the alert writers to the notification queues
        private Dictionary<Guid, Deque<NotificationContext>> notificationQueues;
        private object notificationQueueLock = new object();

        public ScheduledCollectionQueues()
        {
            scheduledCollectionQueue = new Dictionary<int, Deque<ScheduledCollectionQueue.ScheduledCollectionData>>();
            alertWriterQueue = new Deque<ScheduledCollectionQueue.ScheduledCollectionData>();
            notificationQueues = new Dictionary<Guid, Deque<NotificationContext>>();
            MaxQueueLengthPerServer = ManagementServiceConfiguration.GetManagementServiceElement().ScheduledCollectionMaxQueueLength;
        }

        public List<int> GetServersWithWaitingScheduledCollectionData()
        {
            List<int> result = new List<int>();
            lock (scheduledCollectionQueueLock)
            {
                foreach (int serverId in scheduledCollectionQueue.Keys)
                {
                    ICollection collection = scheduledCollectionQueue[serverId];
                    if (collection.Count > 0)
                        result.Add(serverId);
                }
            }
            return result;
        }

        public ScheduledCollectionQueue.ScheduledCollectionData DequeueScheduledCollectionDataMessage(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionData> queue = null;
            lock (scheduledCollectionQueueLock)
            {
                if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    if (queue.Count > 0)
                    {
                        scheduledCollectionQueueCount--;
                        return queue.RemoveFromFront();
                    }
                }
            }

            return null;
        }

        public ScheduledCollectionQueue.ScheduledCollectionData PeekScheduledCollectionDataMessage(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionData> queue = null;
            lock (scheduledCollectionQueueLock)
            {
                if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    if (queue.Count > 0)
                    {
                        return queue.GetAtFront();
                    }
                }
            }
            return null;
        }

        public void RemoveScheduledCollectionDataMessage(int instanceId, ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionData> queue = null;
            lock (scheduledCollectionQueueLock)
            {
                if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    queue.Remove(data);
                }
            }
        }

        public int GetScheduledCollectionDataMessageCount(int instanceId)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionData> queue = null;
            lock (scheduledCollectionQueueLock)
            {
                if (scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    return queue.Count;
                }
            }
            return 0;
        }

        public void EnqueueScheduledCollectionMessage(int instanceId, ScheduledCollectionDataMessage message)
        {
            EnqueueScheduledCollectionData(instanceId, new ScheduledCollectionQueue.ScheduledCollectionData(message));
        }

        public void EnqueueScheduledCollectionData(int instanceId, ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            Deque<ScheduledCollectionQueue.ScheduledCollectionData> queue = null;
            lock (scheduledCollectionQueueLock)
            {
                if (!scheduledCollectionQueue.TryGetValue(instanceId, out queue))
                {
                    queue = new Deque<ScheduledCollectionQueue.ScheduledCollectionData>();
                    scheduledCollectionQueue.Add(instanceId, queue);
                }
                // use delegate to add the queue entry
                queue.AddToFront(data);
                scheduledCollectionQueueCount++;

                if (queue.Count > MaxQueueLengthPerServer)
                {
                    scheduledCollectionQueueCount--;
                    ScheduledCollectionQueue.ScheduledCollectionData removedRefresh = queue.RemoveFromBack();

                    LOG.ErrorFormat("Dropping scheduled collection for {0} from {1}",
                                    removedRefresh.Message.MonitoredServer.InstanceName, removedRefresh.Message.Snapshot.TimeStampLocal);

                    AlertTableWriter.LogOperationalAlerts(Idera.SQLdm.Common.Events.Metric.Operational,
                                                          new MonitoredObjectName(removedRefresh.Message.MonitoredServer),
                                                          Idera.SQLdm.Common.MonitoredState.Critical,
                                                          "Scheduled collection dropped",
                                                          String.Format("A scheduled collection was dropped because the maximun number allowed to be queued ({0}) was exceeded.  The data was collected on '{1}' at {2}.",
                                                            MaxQueueLengthPerServer,
                                                            removedRefresh.Message.MonitoredServer.InstanceName,
                                                            removedRefresh.Message.Snapshot.TimeStampLocal)
                                                          );
                }
            }
        }

        public int ScheduledCollectionDataMessageCount
        {
            get
            {
                lock (scheduledCollectionQueueLock)
                {
                    return scheduledCollectionQueueCount;
                }
            }
        }


        public int EnqueueAlertWriterDataMessage(ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            lock (alertWriterQueueLock)
            {
                LOG.VerboseFormat("Adding refresh to queue for '{0}' from [{1}]",
                                  data.Message.MonitoredServer.InstanceName, data.Message.Snapshot.TimeStampLocal);
                alertWriterQueue.Add(data);
                // Memory Management Issue : Log to see the queue count details 
                LOG.ErrorFormat("Memory Management in alertWriterQueue - performing enqueue and the count is {0}",
                    alertWriterQueue.Count);
                return alertWriterQueue.Count;
            }
        }

        public int GetAlertWriterDataMessageCount()
        {
            lock (alertWriterQueueLock)
            {
                return alertWriterQueue.Count;
            }
        }

        public ScheduledCollectionQueue.ScheduledCollectionData DequeueAlertWriterDataMessage()
        {
            lock (alertWriterQueueLock)
            {
                if (alertWriterQueue.Count > 0)
                {
                    return alertWriterQueue.RemoveFromFront();
                }
            }
            return null;
        }

        public ScheduledCollectionQueue.ScheduledCollectionData PeekAlertWriterDataMessage()
        {
            lock (alertWriterQueueLock)
            {
                if (alertWriterQueue.Count > 0)
                {
                    return alertWriterQueue.GetAtFront();
                }
                return null;
            }
        }


       
        public IQueue<NotificationContext> GetNotificationQueue(Guid notificationProviderId)
        {
            Deque<NotificationContext> queue = null;
            lock (notificationQueueLock)
            {
                if (!notificationQueues.TryGetValue(notificationProviderId, out queue))
                {
                    queue = new Deque<NotificationContext>();
                    notificationQueues.Add(notificationProviderId, queue);
                }
            }
            return new NotificationQueueHelper(notificationProviderId, this);
        }

        public int GetNotificationQueueLength(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (notificationQueueLock)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    return queue.Count;
                }
            }
            return 0;
        }

        public NotificationContext PeekNotificationItem(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (notificationQueueLock)
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

        public NotificationContext[] PeekNotificationItems(Guid queueId)
        {
            Deque<NotificationContext> queue = null;
            lock (notificationQueueLock)
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
            lock (notificationQueueLock)
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
            lock (notificationQueueLock)
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
            lock (notificationQueueLock)
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
                foreach (Guid id in providers)
                {
                    Management.Notification.TryStartProvider(id);
                }
            }
        }

        public NotificationContext[] DequeueNotificationItems(Guid queueId, int count)
        {
            Deque<NotificationContext> queue = null;
            lock (notificationQueueLock)
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
            lock (notificationQueueLock)
            {
                if (notificationQueues.TryGetValue(queueId, out queue))
                {
                    queue.Clear();
                }
            }
        }

        public sealed class NotificationQueueHelper : IQueue<NotificationContext>
        {
            private Guid providerId;
            private ScheduledCollectionQueues queues;

            public NotificationQueueHelper(Guid providerId, ScheduledCollectionQueues queues)
            {
                this.providerId = providerId;
                this.queues = queues;
            }

            public int Count
            {
                get { return queues.GetNotificationQueueLength(providerId); }
            }

            public void Clear()
            {
                queues.ClearQueue(providerId);
            }

            public NotificationContext Dequeue()
            {
                return queues.DequeueNotificationItem(providerId);
            }

            public void Enqueue(NotificationContext item)
            {
                queues.EnqueueNotificationItem(providerId, item);
            }

            public NotificationContext Peek()
            {
                return queues.PeekNotificationItem(providerId);
            }

            public NotificationContext[] PeekAll()
            {
                return queues.PeekNotificationItems(providerId);
            }

            public NotificationContext[] Dequeue(int count)
            {
                return queues.DequeueNotificationItems(providerId, count);
            }
        }
    }
}
