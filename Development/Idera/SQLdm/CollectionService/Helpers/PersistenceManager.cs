//------------------------------------------------------------------------------
// <copyright file="PersistenceManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Linq;
using Amazon.Runtime.Internal;

namespace Idera.SQLdm.CollectionService.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Bamboo.Prevalence;
    using Bamboo.Prevalence.Attributes;
    using Bamboo.Prevalence.Configuration;
    using Bamboo.Prevalence.Util;
    using Configuration;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Wintellect.PowerCollections;

    public enum AzureQsType
    {
        QueryMonitor,
        ActiveWaits
    }

    /// <summary>
    /// This object uses Bamboo Prevalence to persist the current state of this object for
    /// recovery purposes.  This recovery mechanism is effectively just snapshot of what is
    /// currently in memory.  A separate task is used to store snapshot data to disk so that 
    /// the memory necessary to hold the object is freed.
    /// </summary>
    [Serializable]
    public class PersistenceManager : MarshalByRefObject, ISerializable
    {
        private class AzureStartTimeQs
        {
            public string QueryMonitor { get; set; }
            public string ActiveWaits { get; set; }
        }
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("PersistenceManager");
        private static PersistenceManager instance = null;
        private static PrevalenceEngine persistenceEngine;
        private static SnapshotTaker persistenceSnapshoter;
        private static bool initialized;
        private const int serializedVersion = 3;

        private object sync = new object();

        // persisted activity profiler settings (key is instance name)
        private Dictionary<string, ActivityMonitorConfiguration> activityMonitorSettings;
        // persisted query monitor settings (key is instance name)
        private Dictionary<string, QueryMonitorConfiguration> queryMonitorSettings;
        //persisted high water mark for failed job alerting
        private Dictionary<string, int> failedJobInstanceIds;
        //persisted list of jobs we have detected as failed
        private Dictionary<string, List<Guid>> failedJobGuids;
        //persisted list of failed jobs/steps
        private Dictionary<string, List<Pair<Guid, int>>> failedJobSteps;
        //persisted high water mark for completed jobs.  
        private Dictionary<string, int> completedJobInstanceIds;
        // our collection service id
        private Guid? collectionServiceID;
        // persisted last-read mark for error log scan
        private DateTime? logScanDate;

        // Azure QS Start Time
        private Dictionary<int, Dictionary<string, AzureStartTimeQs>> azureQsStartTimes;


        static PersistenceManager() {
            initialized = false;
            InitializePersistance();
            if (persistenceEngine == null)
            {
                LOG.Error("Giving up on prevalence engine.  Using non-persistent object.");
                instance = new PersistenceManager();
            } else
            {
                instance = persistenceEngine.PrevalentSystem as PersistenceManager;
                
                persistenceSnapshoter = 
                    new SnapshotTaker(persistenceEngine, 
                        TimeSpan.FromMinutes(30), 
                        Bamboo.Prevalence.Util.CleanUpAllFilesPolicy.Default);
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
        private void InitializeInstance()
        {
            queryMonitorSettings = new Dictionary<string, QueryMonitorConfiguration>();
            activityMonitorSettings = new Dictionary<string, ActivityMonitorConfiguration>();
            failedJobInstanceIds = new Dictionary<string, int>();
            collectionServiceID = null;
            //failedJobGuids = new Dictionary<string,List<Guid>>();
            failedJobSteps = new Dictionary<string, List<Pair<Guid, int>>>();
            logScanDate = null;
            completedJobInstanceIds = new Dictionary<string, int>();
            azureQsStartTimes = new Dictionary<int, Dictionary<string, AzureStartTimeQs>>();
        }

        #region ISerializable Members

        public PersistenceManager(SerializationInfo info, StreamingContext context)
        {
            // save the serialized version to the 
            int objectVersion = -1;
            try
            {
                objectVersion = info.GetInt32("serializedVersion");
            }
            catch (Exception e)
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

            queryMonitorSettings = (Dictionary<string, QueryMonitorConfiguration>)
                info.GetValue("queryMonitorSettings", typeof(Dictionary<string, QueryMonitorConfiguration>));
            activityMonitorSettings = (Dictionary<string, ActivityMonitorConfiguration>)
                info.GetValue("activityMonitorSettings", typeof(Dictionary<string, ActivityMonitorConfiguration>));
            failedJobInstanceIds = (Dictionary<string,int>)
                info.GetValue("failedJobInstanceIds", typeof(Dictionary<string,int>));
            completedJobInstanceIds = (Dictionary<string, int>)
                info.GetValue("completedJobInstanceIds", typeof(Dictionary<string, int>));
            collectionServiceID = Serialized<object>.TryGetSerializedStruct<Guid>(info, "collectionServiceID");

            try
            {
                failedJobGuids =
                    (Dictionary<string, List<Guid>>)
                    info.GetValue("failedJobGuids", typeof (Dictionary<string, List<Guid>>));
                logScanDate = (DateTime?) info.GetValue("logScanDate", typeof (DateTime?));
            }
            catch
            {
                failedJobGuids = new Dictionary<string, List<Guid>>();
                logScanDate = null;
            }

            try
            {
                failedJobSteps = (Dictionary<string, List<Pair<Guid, int>>>)info.GetValue("failedJobSteps", typeof(Dictionary<string, List<Pair<Guid, int>>>));
                logScanDate = (DateTime?)info.GetValue("logScanDate", typeof(DateTime?));

                if (failedJobGuids.Count == 0)
                {
                    try
                    {
                        failedJobSteps = (Dictionary<string, List<Pair<Guid,int>>>)info.GetValue("failedJobSteps", typeof (Dictionary<string, List<Pair<Guid,int>>>));
                    }
                    catch
                    {
                        failedJobSteps = new Dictionary<string,List<Pair<Guid,int>>>();
                        logScanDate = null;
                    }
                }
                else if (failedJobGuids.Count > 0)
                {
                    foreach (KeyValuePair<string,List<Guid>> instFailedJobs in failedJobGuids)
                    {
                        string instance = (string)instFailedJobs.Key;
                        List<Guid> tmpFailedJobs = (List<Guid>)instFailedJobs.Value;
                        List<Pair<Guid,int>> tmpFailedSteps = new List<Pair<Guid, int>>();
                        foreach (Guid g in tmpFailedJobs)
                        {
                            tmpFailedSteps.Add(new Pair<Guid,int>(g, 0));
                        }
                        
                        if (failedJobSteps.ContainsKey(instance))
                            failedJobSteps.Remove(instance);
                        failedJobSteps.Add(instance, tmpFailedSteps);
                    }
                    failedJobGuids.Clear();
                }
                else
                {
                    failedJobSteps = new Dictionary<string,List<Pair<Guid,int>>>();
                    logScanDate = null;
                }
            }
            catch
            {
                failedJobSteps = new Dictionary<string,List<Pair<Guid,int>>>();
                logScanDate = null;
            }
        }

        [PassThrough]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // save the serialized version to the 
            info.AddValue("serializedVersion", serializedVersion);
            info.AddValue("queryMonitorSettings", queryMonitorSettings);

            info.AddValue("activityMonitorSettings", activityMonitorSettings);
            
            info.AddValue("failedJobInstanceIds", failedJobInstanceIds);
            //info.AddValue("failedJobGuids", failedJobGuids);
            info.AddValue("failedJobSteps", failedJobSteps);
            info.AddValue("logScanDate", logScanDate);
            if (collectionServiceID != null)
                info.AddValue("collectionServiceID", collectionServiceID);
        }

        #endregion

        [PassThrough]
        private static void InitializePersistance()
        {
            using (LOG.InfoCall( "Initialize"))
            {
                PrevalenceSettings.FlushAfterCommand = false;

                if (!CreatePrevalenceEngine())
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
                    result = true;
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Exception initializing prevalence engine");
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
        public static void SyncPointPrevalenceData(bool cleanlyShutdown)
        {
            if (persistenceEngine != null)
            {
                try
                {
                    // serialize the current state to disk
                    persistenceEngine.TakeSnapshot();
                    // select and delete old snapshots and log files
                    FileInfo[] uselessFiles = CleanUpAllFilesPolicy.Default.SelectFiles(persistenceEngine);
                    foreach (FileInfo uselessFile in uselessFiles)
                    {
                        uselessFile.Delete();
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error cleaning up cached objects: " + e.Message);
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
                Directory.Delete(path, true);
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
            return Path.Combine(CollectionServiceConfiguration.DataPath, "Cache");
        }

        [PassThrough]
        private static string GetPrevalenceQueueDirectory()
        {
            return Path.Combine(CollectionServiceConfiguration.DataPath, "Queued");
        }

        [PassThrough]
        public static PersistenceManager Instance
        {
            get { return instance; }
        }

        [Query]
        public Guid? GetCollectionServiceID()
        {
            return collectionServiceID;
        }

        public void SetCollectionServiceID(Guid? value)
        {
            collectionServiceID = value;
        }

        [Query]
        public QueryMonitorConfiguration GetQueryMonitorConfiguration(string instanceName)
        {
            QueryMonitorConfiguration result = null;
            lock (sync)
            {
                queryMonitorSettings.TryGetValue(instanceName, out result);
            }
            return result;
        }
        public void SetQueryMonitorConfiguration(string instanceName, QueryMonitorConfiguration config)
        {
            lock (sync)
            {
                if (queryMonitorSettings.ContainsKey(instanceName))
                    queryMonitorSettings.Remove(instanceName);
                queryMonitorSettings.Add(instanceName, config);
            }
        }
        
        [Query]
        public ActivityMonitorConfiguration GetActivityMonitorConfiguration(string instanceName)
        {
            ActivityMonitorConfiguration result = null;
            lock (sync)
            {
                activityMonitorSettings.TryGetValue(instanceName, out result);
            }
            return result;
        }
        public void SetActivityMonitorConfiguration(string instanceName, ActivityMonitorConfiguration config)
        {
            lock (sync)
            {
                if (activityMonitorSettings.ContainsKey(instanceName))
                    activityMonitorSettings.Remove(instanceName);
                activityMonitorSettings.Add(instanceName, config);
            }
        }

        [Query]
        public int GetFailedJobInstanceId(string instanceName)
        {
            int result = 0;
            lock (sync)
            {
                failedJobInstanceIds.TryGetValue(instanceName, out result);
            }
            return result;
        }

        public void SetFailedJobInstanceId(string instanceName, int jobId)
        {
            lock (sync)
            {
                if (failedJobInstanceIds.ContainsKey(instanceName))
                    failedJobInstanceIds.Remove(instanceName);
                failedJobInstanceIds.Add(instanceName, jobId);
            }
        }

        [Query]
        public List<Guid> GetFailedJobGuids(string instanceName)
        {
            List<Guid> result;
            lock (sync)
            {
                failedJobGuids.TryGetValue(instanceName, out result);
            }
            if (result == null) result = new List<Guid>();
            
            return result;
        }

        public void SetFailedJobGuids(string instanceName, List<Guid> jobIds)
        {
            using (LOG.VerboseCall("SetFailedJobGuids"))
            {
                lock (sync)
                {
                    if (failedJobGuids.ContainsKey(instanceName))
                        failedJobGuids.Remove(instanceName);
                    failedJobGuids.Add(instanceName, jobIds);
                }
                LOG.VerboseFormat("Tracking {0} failed jobs for {1}", jobIds.Count, instanceName);
            }
        }

        [Query]
        public List<Pair<Guid, int>> GetFailedJobSteps(string instanceName)
        {
            List<Pair<Guid, int>> result;

            lock (sync)
            {
                failedJobSteps.TryGetValue(instanceName, out result);
            }
            if (result == null) result = new List<Pair<Guid, int>>();

            return result;
        }

        public void SetFailedJobSteps(string instanceName, List<Pair<Guid, int>> jobSteps)
        {
            using (LOG.VerboseCall("SetFailedJobSteps"))
            {
                lock (sync)
                {
                    if (failedJobSteps.ContainsKey(instanceName))
                        failedJobSteps.Remove(instanceName);
                    failedJobSteps.Add(instanceName, jobSteps);
                }
                LOG.VerboseFormat("Tracking {0} failed job steps for {1}", jobSteps.Count, instanceName);
            }
        }

        [Query]
        public int GetCompletedJobInstanceId(string instanceName)
        {
            int result = 0;
            lock (sync)
            {
                completedJobInstanceIds.TryGetValue(instanceName, out result);
            }
            return result;
        }

        public void SetCompletedJobInstanceId(string instanceName, int jobId)
        {
            lock (sync)
            {
                if (completedJobInstanceIds.ContainsKey(instanceName))
                    completedJobInstanceIds.Remove(instanceName);
                completedJobInstanceIds.Add(instanceName, jobId);
            }
        }

        [Query]
        public DateTime? GetLogScanDate()
        {
            return logScanDate;
        }

        public void SetLogScanDate(DateTime? value)
        {
            logScanDate = value;
        }

        public string GetAzureQsStartTime(int serverId, string databaseName, AzureQsType azureType)
        {
            try
            {
                if (!azureQsStartTimes.ContainsKey(serverId))
                {
                    azureQsStartTimes.Add(serverId, new Dictionary<string, AzureStartTimeQs>());
                }

                if (!azureQsStartTimes[serverId].ContainsKey(databaseName))
                {
                    azureQsStartTimes[serverId].Add(databaseName, new AzureStartTimeQs());
                }

                var azureStartTime = azureQsStartTimes[serverId][databaseName];
                return azureType == AzureQsType.QueryMonitor ? azureStartTime.QueryMonitor : azureStartTime.ActiveWaits;
            }
            catch (Exception ex)
            {
                LOG.Warn("GetAzureQsStartTime: Exception encountered {0}", ex);
                return null;
            }
        }

        public void SetAzureQsStartTime(int serverId, string databaseName, AzureQsType azureType, string value)
        {
            try
            {
                if (!azureQsStartTimes.ContainsKey(serverId))
                {
                    azureQsStartTimes.Add(serverId, new Dictionary<string, AzureStartTimeQs>());
                }

                if (!azureQsStartTimes[serverId].ContainsKey(databaseName))
                {
                    azureQsStartTimes[serverId].Add(databaseName, new AzureStartTimeQs());
                }

                var azureStartTime = azureQsStartTimes[serverId][databaseName];
                switch (azureType)
                {
                    case AzureQsType.QueryMonitor:
                        azureStartTime.QueryMonitor = value;
                        break;
                    case AzureQsType.ActiveWaits:
                        azureStartTime.ActiveWaits = value;
                        break;
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("SetAzureQsStartTime: Exception encountered {0}", ex);
            }
        }

        public void ClearAzureQsStartTime(int serverId, string databaseName, AzureQsType azureType)
        {
            try
            {
                if (!azureQsStartTimes.ContainsKey(serverId) ||
                        !azureQsStartTimes[serverId].ContainsKey(databaseName))
                {
                    return;
                }

                var azureStartTime = azureQsStartTimes[serverId][databaseName];
                switch (azureType)
                {
                    case AzureQsType.QueryMonitor:
                        azureStartTime.QueryMonitor = null;
                        if (azureStartTime.ActiveWaits == null)
                        {
                            azureQsStartTimes[serverId].Remove(databaseName);
                        }
                        break;
                    case AzureQsType.ActiveWaits:
                        azureStartTime.ActiveWaits = null;
                        if (azureStartTime.QueryMonitor == null)
                        {
                            azureQsStartTimes[serverId].Remove(databaseName);
                        }
                        break;
                }

                if (azureQsStartTimes[serverId].Count == 0)
                {
                    azureQsStartTimes.Remove(serverId);
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("ClearAzureQsStartTime: Exception encountered {0}", ex);
            }
        }

        public void ClearAzureQsStartTime(int serverId,AzureQsType azureType)
        {
            try
            {
                if (!azureQsStartTimes.ContainsKey(serverId))
                {
                    return;
                }

                var toClearDbNames = new List<string>();
                foreach (var azureStartTimeQse in azureQsStartTimes[serverId])
                {
                    var azureStartTime = azureStartTimeQse.Value;
                    switch (azureType)
                    {
                        case AzureQsType.QueryMonitor:
                            azureStartTime.QueryMonitor = null;
                            if (azureStartTime.ActiveWaits == null)
                            {
                                toClearDbNames.Add(azureStartTimeQse.Key);
                            }
                            break;
                        case AzureQsType.ActiveWaits:
                            azureStartTime.ActiveWaits = null;
                            if (azureStartTime.QueryMonitor == null)
                            {
                                toClearDbNames.Add(azureStartTimeQse.Key);
                            }
                            break;
                    }
                }

                foreach (var clearDbName in toClearDbNames)
                {
                    azureQsStartTimes[serverId].Remove(clearDbName);
                }

                if (azureQsStartTimes[serverId].Count == 0)
                {
                    azureQsStartTimes.Remove(serverId);
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("ClearAzureQsStartTime Server level: Exception encountered {0}", ex);
            }
        }
    }
}
