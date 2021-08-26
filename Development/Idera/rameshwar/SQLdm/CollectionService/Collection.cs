//------------------------------------------------------------------------------
// <copyright file="ManagementServiceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.CollectionService
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Idera.SQLdm.CollectionService.ManagementService;
    using Idera.SQLdm.CollectionService.Monitoring;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Top-level static class used by the collection service.  The primary purpose of this
    /// class is to hold references to the managers for the various subsystems in the collection
    /// service.
    /// </summary>
    public static class Collection
    {
        #region fields
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Collection");

        private static ManagementServiceManager managementService;
        private static ScheduledCollectionManager scheduled;
        private static OnDemandCollectionManager onDemand;
        private static Dictionary<int,CustomCounterDefinition> customCounters;
        private static MultiDictionary<int,int> customCounterTags;  // <metric,tag>
        private static EventLog eventLog;
        private static MessageDll messageDll;
        private static Dictionary<Guid, ISnapshotSink> snapshotSinks;
        private static DelegateWorkQueue workQueue;
        private static DelegateWorkQueue workQueueOnDemand;
        private static bool paused;
        private static bool reset;
        private static List<Pair<string,int?>> excludedWaitTypes;

        private static object sync = new object();

        #endregion

        #region constructors

        /// <summary>
        /// Initializes the <see cref="T:Collection"/> class.
        /// </summary>
        static Collection()
        {
            ManagementService = new ManagementServiceManager();
            Scheduled = new ScheduledCollectionManager();
            OnDemand = new OnDemandCollectionManager();
            snapshotSinks = new Dictionary<Guid, ISnapshotSink>();
            customCounters = new Dictionary<int, CustomCounterDefinition>();
            customCounterTags = new MultiDictionary<int, int>(false);
            excludedWaitTypes = new List<Pair<string,int?>>();

            // configure the work queue similar to the .Net managed thread pool
            int workerThreads, completionPortThreads;
            System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            LOG.InfoFormat("Work queue set to use a maximum of {0} threads.", workerThreads);
            workQueue = new DelegateWorkQueue(new MultipleWorkerStrategy(10, workerThreads));
            workQueueOnDemand = new DelegateWorkQueue(new MultipleWorkerStrategy(10, workerThreads));//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix

            AuditingEngine.Instance.ManagementService = RemotingHelper.GetObject<IManagementService>();
        }

        #endregion

        #region guts
/*
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public static CollectionServiceConfiguration Configuration
        {
            get { return configuration; }
            private set { configuration = value; }
        }
*/
        public static Dictionary<Guid, ISnapshotSink> SnapshotSinks
        {
            get { return snapshotSinks; }
        }

        public static bool IsPaused
        {
            get
            {
                lock (sync)
                {
                    return paused;
                }
            }
            set
            {
                lock (sync)
                {
                    paused = value;
                }
            }
        }

        public static EventLog EventLog
        {
            get { return eventLog; }
            set { eventLog = value; }
        }

        public static void WriteEvent(Status messageId, Category categoryId, params string[] vars)
        {
            try
            {
                EventLog.WriteEvent(new EventInstance((long) messageId, (int) categoryId), vars);
            } 
            catch (Exception e)
            {
                LogWriteEventError(e, messageId, vars);   
            }
        }

        public static void WriteEvent(EventLog eventLog, EventLogEntryType entryType, Status messageId, Category categoryId, params string[] vars)
        {
            try
            {
                EventInstance instance = new EventInstance((long)messageId, (int)categoryId);
                instance.EntryType = entryType;
                eventLog.WriteEvent(instance, vars);
            }
            catch (Exception e)
            {
                LogWriteEventError(e, messageId, vars);
            }
        }

        public static void LogWriteEventError(Exception exception, Status messageId, params string[] vars)
        {
            LOG.ErrorFormat("Error writing to the event log: {0}", exception.Message);
            try
            {
                if (messageDll == null)
                    messageDll = new MessageDll();

                string message = messageDll.Format(messageId, vars);
                LOG.ErrorFormat(message);
            }
            catch (Exception e)
            {
                StringBuilder builder = new StringBuilder();
                if (vars != null)
                {
                    foreach (string var in vars)
                    {
                        if (builder.Length > 0)
                            builder.Append(",");
                        builder.AppendFormat("'{0}'", var);
                    }
                }
                LOG.ErrorFormat("Error formatting event log message: {0:F} - {1}", messageId, builder.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the management service.
        /// </summary>
        /// <value>The management service.</value>
        public static ManagementServiceManager ManagementService
        {
            get { return managementService; }
            private set { managementService = value; }
        }

        /// <summary>
        /// Gets or sets the scheduled.
        /// </summary>
        /// <value>The scheduled.</value>
        public static ScheduledCollectionManager Scheduled
        {
            get { return scheduled; }
            private set { scheduled = value; }
        }
        /// <summary>
        /// Gets or sets the on demand.
        /// </summary>
        /// <value>The on demand.</value>
        public static OnDemandCollectionManager OnDemand
        {
            get { return onDemand; }
            private set { onDemand = value; }
        }

        public static void Pause()
        {
            IsPaused = true;
            OnDemand.Clear();
            Scheduled.Stop();
        }

        public static void QueueDelegate(WorkQueueDelegate method)
        {
            workQueue.Enqueue(method);
        }


        public static void JoinWorkQueue()
        {
            workQueue.Stop(true);
            workQueue.Join();
        }

        //SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
        public static void OnDemandQueueDelegate(WorkQueueDelegate method)
        {
            workQueueOnDemand.Enqueue(method);
        }

        //SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
        public static void OnDemandJoinWorkQueue()
        {
            workQueueOnDemand.Stop(true);
            workQueueOnDemand.Join();
        }

        public static Set<int> GetCustomCounterKeySet()
        {
            lock (sync)
            {
                return new Set<int>(customCounters.Keys);
            }
        }

        public static CustomCounterDefinition GetCustomCounter(int metricID)
        {
            CustomCounterDefinition definition = null;
            lock (sync)
            {
                customCounters.TryGetValue(metricID, out definition);    
            }
            return definition;
        }

        public static void ReplaceCustomCounter(CustomCounterDefinition counterDefinition)
        {
            lock (sync)
            {
                if (customCounters.ContainsKey(counterDefinition.MetricID))
                    customCounters.Remove(counterDefinition.MetricID);
                customCounters.Add(counterDefinition.MetricID, counterDefinition);
            }
        }

        public static void RemoveCustomCounter(int metricID)
        {
            lock (sync)
            {
                if (customCounters.ContainsKey(metricID))
                    customCounters.Remove(metricID);

                // whack any tags we are tracking for the counter
                customCounterTags.Remove(metricID);
            }
        }

        /// <summary>
        /// Used to initialize the list of custom counter tags received in CollectionServiceWorkload
        /// </summary>
        public static void SetCustomCounterTags(MultiDictionary<int,int> allOfEm)
        {
            lock(sync)
            {
                customCounterTags = allOfEm;
            }
        }

        /// <summary>
        /// Get a set of tags for a custom counter
        /// </summary>
        public static Set<int> GetCustomCounterTags(int metricId)
        {
            Set<int> result = new Set<int>();
            lock (sync)
            {
               if (customCounterTags.ContainsKey(metricId))
               {
                   result.AddMany(customCounterTags[metricId]);
               }
            }
            return result;
        }

        /// <summary>
        /// Add the tag to the selected counters
        /// </summary>
        public static void AddTagToCustomCounters(int tagId, IEnumerable<int> metricIds)
        {
            lock (sync)
            {
                foreach (int metricId in metricIds)
                {
                    if (customCounters.ContainsKey(metricId))
                    {
                        customCounterTags.Add(metricId, tagId);
                    }
                }
            }
        }

        /// <summary>
        /// Remove the tag from the selected counters
        /// </summary>
        public static void RemoveTagFromCustomCounters(int tagId, IEnumerable<int> metricIds)
        {
            lock (sync)
            {
                foreach (int metricId in metricIds)
                {
                    customCounterTags.Remove(metricId, tagId);
                }
            }
        }

        /// <summary>
        /// Handle when someone deletes a tag.
        /// </summary>
        public static void RemoveTags(IEnumerable<int> tagIds)
        {
            lock (sync)
            {
                Set<int> customCounterKeySet = GetCustomCounterKeySet();
                if (customCounterKeySet.Count > 0)
                {
                    foreach (int tagId in tagIds)
                    {
                        RemoveTagFromCustomCounters(tagId, customCounterKeySet);
                    }
                }
            }
        }

        internal static void UpdateTagConfiguration(int tagId, IList<int> customCounterIds)
        {
            lock (sync)
            {
                Set<int> customCounterKeySet = GetCustomCounterKeySet();
                if (customCounterKeySet.Count > 0)
                {
                    RemoveTagFromCustomCounters(tagId, Algorithms.SetDifference(customCounterKeySet, customCounterIds));
                    AddTagToCustomCounters(tagId, customCounterIds);

                }
            }
        }

        public static void AddExcludedWaitTypes(List<Pair<string, int?>> newExcludedWaits)
        {
            lock (sync)
            {
                excludedWaitTypes = newExcludedWaits;
            }
        }

        public static List<Pair<string, int?>> GetExcludedWaitTypes()
        {
            lock (sync)
            {
                return excludedWaitTypes;
            }
        }

        #endregion

    }
}
