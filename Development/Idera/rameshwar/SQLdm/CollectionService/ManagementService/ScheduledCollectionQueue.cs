//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.CollectionService.Monitoring;

namespace Idera.SQLdm.CollectionService.ManagementService
{
    using System.Collections.Generic;
    using System.Threading;
    using Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;

    public class ScheduledCollectionQueue
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionQueue");
        private static int MaxQueueLengthPerServer = 5;
        private Set<int> queueIds = new Set<int>();
        private Deque<int> queueOrder = new Deque<int>();
        private Dictionary<int, Deque<Serialized<ScheduledCollectionDataMessage>>> queues;
        public readonly object SyncRoot = new object();

        private int count = 0;

        //SQLdm 10.0 (Sanjali Makkar): Small Features : declaring local variable to get '# Dropped Scheduled Refreshes'
        private int droppedScheduledRefreshes = 0;
        
        public ScheduledCollectionQueue()
        {
            queues = new Dictionary<int, Deque<Serialized<ScheduledCollectionDataMessage>>>();
            MaxQueueLengthPerServer =
                CollectionServiceConfiguration.GetCollectionServiceElement().ScheduledCollectionMaxQueueLength;
        }

        public void Enqueue(ScheduledCollectionDataMessage scheduledRefresh)
        {
            using (LOG.VerboseCall("Schedule Collection Enqueue"))
            {
                Stopwatch stopwatch = new Stopwatch();

                int id = scheduledRefresh.MonitoredServer.Id;
                Deque<Serialized<ScheduledCollectionDataMessage>> queue = null;

                //firt we need serialize before lock the common resources Dequeue's and dictionary too
                Serialized<ScheduledCollectionDataMessage> serialized = new Serialized<ScheduledCollectionDataMessage>(scheduledRefresh, true);
                serialized.Serialize(true);

                stopwatch.Start();
                lock (SyncRoot)
                {
                    if (!queues.TryGetValue(id, out queue))
                    {
                        queue = new Deque<Serialized<ScheduledCollectionDataMessage>>();
                        queues.Add(id, queue);
                    }
                    if (scheduledRefresh.Snapshot != null)
                        LOG.DebugFormat("Adding snapshot for '{0}' to scheduled refresh delivery queue at the {1} time stamp",
                                       scheduledRefresh.MonitoredServer.InstanceName, scheduledRefresh.Snapshot.TimeStampLocal);
                   
                    queue.AddToFront(serialized);
                    count++;

                    LOG.DebugFormat("The size of the Collection Queue length is {0} ", queue.Count);

                    while (queue.Count > MaxQueueLengthPerServer)
                    {
                        queue.RemoveFromBack();
                        count--;

                        //SQLdm 10.0 (Sanjali Makkar): Small Features : Updating local variable for '# Dropped Scheduled Refreshes' Performance Counter
                        droppedScheduledRefreshes++;
                        
                        LOG.ErrorFormat("Dropping scheduled collection for {0} to scheduled refresh delivery from the last from queue.",
                                scheduledRefresh.MonitoredServer.InstanceName);
                    }

                    //SQLdm 10.0 (Sanjali Makkar): Small Features : Updating '# Dropped Scheduled Refreshes' Performance Counter
                    Statistics.SetDroppedScheduledRefreshes(droppedScheduledRefreshes, scheduledRefresh.MonitoredServer.InstanceName);
                    droppedScheduledRefreshes = 0;

                    //SQLdm 10.0 (Sanjali Makkar): Small Features : Updating '# Queued Scheduled Refreshes' Performance Counter
                    Statistics.SetQueuedScheduledRefreshes(queue.Count, scheduledRefresh.MonitoredServer.InstanceName);

                    if (!queueIds.Contains(id))
                    {
                        queueIds.Add(id);           // indication that server has entry queued
                        queueOrder.AddToBack(id);   // put in order queue so items are fairly dequeued
                    }
                }
                stopwatch.Stop();
                LOG.DebugFormat("Schedule Collection Enqueue method took {0} milliseconds.", stopwatch.ElapsedMilliseconds);
            }
        }

        public int Count
        {
            get
            {
                lock(SyncRoot) { return count; }
            }
        }

        public bool Peek(out int monitoredServerId, out Serialized<ScheduledCollectionDataMessage> scheduledRefresh)
        {
            using (LOG.VerboseCall("Schedule Collection Peek"))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                // remove the object from the queue
                Deque<Serialized<ScheduledCollectionDataMessage>> queue = null;
                lock (SyncRoot)
                {
                    LOG.DebugFormat("The QueueOrder Collection size is {0} at the Peek Method ", queueOrder.Count);
                    while (queueOrder.Count > 0)
                    {
                        int serverId = queueOrder.GetAtFront();
                        if (queues.TryGetValue(serverId, out queue))
                        {
                            LOG.DebugFormat("The Queue Collection size is {0} at the Peek Method ", queue.Count);
                            if (queue.Count > 0)
                            {
                                scheduledRefresh = queue.GetAtFront();
                                monitoredServerId = serverId;
                                
                                stopwatch.Stop();
                                LOG.DebugFormat("TRUE return in Schedule Collection Peek method took {0} milliseconds.", stopwatch.ElapsedMilliseconds);
                                return true;
                            }
                        }
                    }
                }
                scheduledRefresh = null;
                monitoredServerId = -1;
                LOG.Debug("The Queue Collection is empty, we return ScheduleRefresh as NUll aand ID is -1");

                stopwatch.Stop();
                LOG.DebugFormat("FALSE return in Schedule Collection Peek method took {0} milliseconds.", stopwatch.ElapsedMilliseconds);
                return false;
            }
        }

        
        public void Remove(int serverId, Serialized<ScheduledCollectionDataMessage> serializedRefresh)
        {
            using (LOG.VerboseCall("Schedule Collection Remove"))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                LOG.DebugFormat("Removing scheduled collection queue for serverId: {0}",serverId);
                Deque<Serialized<ScheduledCollectionDataMessage>> queue = null;
                lock (SyncRoot)
                {
                    if (queues.TryGetValue(serverId, out queue))
                    {
                        // remove scheduled refresh from queue
                        queue.Remove(serializedRefresh);
                        // if queue empty
                        if (queue.Count == 0)
                        {
                            // remove server from queue ids
                            queueIds.Remove(serverId);
                           
                        }
                        else
                        {
                            // queue not empty
                            // add to queue ids
                            queueIds.Add(serverId);
                            // send server to back of dequeue line
                            queueOrder.AddToBack(serverId);
                        }
                        // if head of queueOrder is server remove it
                        if (queueOrder.GetAtFront() == serverId)
                        {
                            queueOrder.RemoveFromFront();
                        }
                    }
                }
                stopwatch.Stop();
                LOG.DebugFormat("Schedule Collection Remove method took {0} milliseconds.", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
