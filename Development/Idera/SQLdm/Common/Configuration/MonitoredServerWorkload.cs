//------------------------------------------------------------------------------
// <copyright file="MonitoredServerWorkload.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using BBS.TracerX;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Contains connection information and scheduled collection configuration
    /// for a given monitored server.
    /// </summary>
    [Serializable]
    public class MonitoredServerWorkload : ISerializable
    {
        #region fields

        private MonitoredSqlServer monitoredServer;
        private MonitoredObjectStateGraph stateGraph;
        private List<int> customCounters;
        private IDictionary<int, MetricThresholdEntry> thresholds = null;
        private IDictionary<int, List<MetricThresholdEntry>> thresholdInstances;
        private IDictionary<int, bool> metricCompatibilityForSqlExpress = null; //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
        //Start - SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
        BaselineMetricMeanCollection baselineMetricMeanCollection;
        //End - SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values

        private Snapshot previousRefresh;
        private SessionsConfiguration historyBrowserSessionsConfiguration;
        private LockDetailsConfiguration historyBrowserLockDetailsConfiguration;
        private IDictionary<Guid, ServerPreferredMirrorConfig> mirroringRoles;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MonitoredServerWorkload");

        private object sync = new object();
        private bool reloadDefaultThresholds = true;
        private object syncThresholds = new object();

        public Dictionary<Type, Snapshot> periodicRefreshPreviousValues = new Dictionary<Type, Snapshot>();

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServerWorkload"/> class.
        /// </summary>
        public MonitoredServerWorkload()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServerWorkload"/> class.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <param name="stateGraph">The state graph.</param>
        /// <param name="thresholds">The thresholds.</param>
        public MonitoredServerWorkload(MonitoredSqlServer monitoredServer, MonitoredObjectStateGraph stateGraph, IDictionary<int, List<MetricThresholdEntry>> thresholds)
            : this()
        {
            // UNUSED??
            MonitoredServer = monitoredServer;
            StateGraph = stateGraph;
            thresholdInstances = thresholds;
            PeriodicRefreshPreviousValues = new Dictionary<Type, Snapshot>();
            if (MonitoredServer != null)
            {
                HistoryBrowserLockDetailsConfiguration = new LockDetailsConfiguration(MonitoredServer.Id, null);
                HistoryBrowserSessionsConfiguration = new SessionsConfiguration(MonitoredServer.Id, null);
                HistoryBrowserSessionsConfiguration.ExcludeSystemProcesses = false;
                HistoryBrowserSessionsConfiguration.ExcludeDiagnosticManagerProcesses = false;
            }
            if (monitoredServer != null)
                baselineMetricMeanCollection = new BaselineMetricMeanCollection(monitoredServer.Id);
            else
                LOG.Error("BaselineMetricMeanCollection object is not initialized, due to MonitoredSqlServer object is null.");
        }

        protected MonitoredServerWorkload(SerializationInfo info, StreamingContext context)
        {
            monitoredServer = (MonitoredSqlServer)info.GetValue("monitoredServer", typeof(MonitoredSqlServer));
            stateGraph = (MonitoredObjectStateGraph)info.GetValue("stateGraph", typeof(MonitoredObjectStateGraph));
            customCounters = (List<int>)info.GetValue("customCounters", typeof(List<int>));
            thresholdInstances = (IDictionary<int, List<MetricThresholdEntry>>)info.GetValue("thresholdInstances", typeof(IDictionary<int, List<MetricThresholdEntry>>));
            metricCompatibilityForSqlExpress = (IDictionary<int, bool>)info.GetValue("metricCompatibilityForSqlExpress", typeof(IDictionary<int, bool>)); //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
            baselineMetricMeanCollection = (BaselineMetricMeanCollection)info.GetValue("baselineMetricMean", typeof(BaselineMetricMeanCollection));//SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
            previousRefresh = (Snapshot)info.GetValue("previousRefresh", typeof(Snapshot));
            historyBrowserSessionsConfiguration = (SessionsConfiguration)info.GetValue("historyBrowserSessionsConfiguration", typeof(SessionsConfiguration));
            historyBrowserLockDetailsConfiguration = (LockDetailsConfiguration)info.GetValue("historyBrowserLockDetailsConfiguration", typeof(LockDetailsConfiguration));
            mirroringRoles = (IDictionary<Guid, ServerPreferredMirrorConfig>)info.GetValue("mirroringRoles", typeof(IDictionary<Guid, ServerPreferredMirrorConfig>));
            periodicRefreshPreviousValues = (Dictionary<Type, Snapshot>)info.GetValue("periodicRefreshPreviousValues", typeof(Dictionary<Type, Snapshot>));
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("monitoredServer", monitoredServer);
            info.AddValue("stateGraph", stateGraph);
            info.AddValue("customCounters", customCounters);
            info.AddValue("thresholdInstances", thresholdInstances);
            info.AddValue("metricCompatibilityForSqlExpress", metricCompatibilityForSqlExpress); //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
            info.AddValue("baselineMetricMean", baselineMetricMeanCollection); //SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
            info.AddValue("previousRefresh", previousRefresh);
            info.AddValue("historyBrowserSessionsConfiguration", historyBrowserSessionsConfiguration);
            info.AddValue("historyBrowserLockDetailsConfiguration", historyBrowserLockDetailsConfiguration);
            info.AddValue("mirroringRoles", mirroringRoles);
            info.AddValue("periodicRefreshPreviousValues", periodicRefreshPreviousValues);
        }

        #region properties

        public MonitoredSqlServer MonitoredServer
        {
            get { return monitoredServer; }
            set { monitoredServer = value; }
        }


        public int Id
        {
            get { return MonitoredServer.Id; }
        }

        public string Name
        {
            get { return MonitoredServer.ConnectionInfo.InstanceName; }
        }

        /// <summary>
        /// Gets or sets the normal collection interval.
        /// </summary>
        /// <value>The normal collection interval.</value>
        public TimeSpan NormalCollectionInterval
        {
            get { return monitoredServer.ScheduledCollectionInterval; }
        }

        /// <summary>
        /// Gets or sets the state graph.
        /// </summary>
        /// <value>The state graph.</value>
        public MonitoredObjectStateGraph StateGraph
        {
            get { return stateGraph; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("stateGraph");
                stateGraph = value;
            }
        }

        public bool ReloadDefaultThresholds
        {
            set { reloadDefaultThresholds = true; }
        }

        /// <summary>
        /// Gets the default thresholds for each metric.
        /// </summary>
        /// <value>The thresholds.</value>
        public IDictionary<int, MetricThresholdEntry> Thresholds
        {
            get
            {
                lock (syncThresholds)
                {
                    if (reloadDefaultThresholds)
                    {
                        if (thresholds == null)
                            thresholds = new Dictionary<int, MetricThresholdEntry>();
                        else
                            thresholds.Clear();

                        foreach (List<MetricThresholdEntry> mteList in thresholdInstances.Values)
                        {
                            foreach (MetricThresholdEntry mte in mteList)
                            {
                                if (mte.IsDefaultThreshold)
                                {
                                    thresholds.Add(mte.MetricID, mte);
                                }
                            }
                        }
                        reloadDefaultThresholds = false;
                    }
                    return thresholds;
                }
            }

            //set { thresholds = value; }
        }

        /// <summary>
        /// Gets all of the threshold instances grouped by MetricID
        /// </summary>
        public IDictionary<int, List<MetricThresholdEntry>> ThresholdInstances
        {
            get { return thresholdInstances; }
            set { thresholdInstances = value; }
        }

        /// <summary>
        /// SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
        /// </summary>
        public IDictionary<int, bool> MetricCompatibilityForSqlExpress
        {
            get { return metricCompatibilityForSqlExpress; }
            set { metricCompatibilityForSqlExpress = value; }
        }

        /// <summary>
        ///SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
        /// </summary>
        public BaselineMetricMeanCollection BaselineMetricMeanCollection
        {
            get { return baselineMetricMeanCollection; }
            set { baselineMetricMeanCollection = value; }
        }


        /// <summary>
        /// Gets or sets the outstanding events.  
        /// </summary>
        /// <value>The outstanding events.</value>
        public IList<OutstandingEventEntry> OutstandingEvents
        {
            get { return null; }
            set
            {
                StateGraph = new MonitoredObjectStateGraph(MonitoredServer);
                if (value == null)
                    return;

                //                foreach (OutstandingEventEntry outstandingEvent in value)
                //                {
                //
                //                    StateGraph.AddEvent(new StateDeviationEvent(outstandingEvent));
                //                }
            }
        }

        /// <summary>
        /// Stores the previous refresh on a monitored server
        /// </summary>
        public Snapshot PreviousRefresh
        {
            get
            {
                return previousRefresh;
            }
            set
            {
                previousRefresh = value;
            }
        }

        /// <summary>
        /// Store previous data for periodic collectors
        /// </summary>
        public Dictionary<Type, Snapshot> PeriodicRefreshPreviousValues
        {
            get { return periodicRefreshPreviousValues; }
            set { periodicRefreshPreviousValues = value; }
        }

        /// <summary>
        /// Stores the history browser session configuratio
        /// </summary>
        public SessionsConfiguration HistoryBrowserSessionsConfiguration
        {
            get
            {
                // Ensure we have a session configuration
                if (historyBrowserSessionsConfiguration == null)
                    if (MonitoredServer != null)
                    {
                        historyBrowserSessionsConfiguration = new SessionsConfiguration(MonitoredServer.Id, null);
                        historyBrowserSessionsConfiguration.ExcludeSystemProcesses = false;
                        historyBrowserSessionsConfiguration.ExcludeDiagnosticManagerProcesses = false;
                    }

                // Update previous values, if any
                if (PreviousRefresh != null && historyBrowserSessionsConfiguration != null)
                    historyBrowserSessionsConfiguration.SetPreviousValues(((ScheduledRefresh)previousRefresh).SessionList);

                return historyBrowserSessionsConfiguration;
            }
            set { historyBrowserSessionsConfiguration = value; }
        }

        /// <summary>
        /// Stores the history browser lock configuration
        /// </summary>
        public LockDetailsConfiguration HistoryBrowserLockDetailsConfiguration
        {
            get
            {
                // Ensure we have a lock configuration
                if (historyBrowserLockDetailsConfiguration == null)
                    if (MonitoredServer != null)
                    {
                        historyBrowserLockDetailsConfiguration = new LockDetailsConfiguration(MonitoredServer.Id, null);
                    }

                // Update previous values, if any
                if (PreviousRefresh != null && historyBrowserLockDetailsConfiguration != null)
                    historyBrowserLockDetailsConfiguration.SetPreviousSnapshot(((ScheduledRefresh)previousRefresh).LockList);

                return historyBrowserLockDetailsConfiguration;
            }
            set { historyBrowserLockDetailsConfiguration = value; }
        }

        public List<int> CustomCounters
        {
            get
            {
                if (customCounters == null)
                    customCounters = new List<int>();
                return customCounters;
            }
            set
            {
                customCounters = value;
            }
        }

        public Set<int> ServerTags
        {
            get
            {
                lock (sync)
                {
                    return new Set<int>(monitoredServer.Tags);
                }
            }
            set
            {
                lock (sync)
                {
                    monitoredServer.Tags.Clear();
                    monitoredServer.Tags.AddRange(value);
                }
            }
        }

        public void AddServerTag(int tagId)
        {
            lock (sync)
            {
                if (!monitoredServer.Tags.Contains(tagId))
                    monitoredServer.Tags.Add(tagId);
            }
        }

        public void RemoveServerTag(int tagId)
        {
            lock (sync)
            {
                monitoredServer.Tags.Remove(tagId);
            }
        }

        public void RemoveServerTags(IEnumerable<int> tagIds)
        {
            lock (sync)
            {
                foreach (int tagId in tagIds)
                {
                    monitoredServer.Tags.Remove(tagId);
                }
            }
        }
        /// <summary>
        /// Update the mirroring sessions that are present on this server
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="serverConfig"></param>
        public void UpdateMirroringSessions(Guid guid, ServerPreferredMirrorConfig serverConfig)
        {
            lock (sync)
            {
                if (mirroringRoles == null) mirroringRoles = new Dictionary<Guid, ServerPreferredMirrorConfig>();

                if (mirroringRoles.ContainsKey(guid))
                {
                    if (serverConfig == null)
                    {
                        mirroringRoles.Remove(guid);
                    }
                    else
                    {
                        mirroringRoles[guid] = serverConfig;
                    }
                }
                else
                {
                    mirroringRoles.Add(guid, serverConfig);
                }
            }
        }

        /// <summary>
        /// Gets the last role that was set as the preferred role for this server in the session represented bu the guid param
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ServerPreferredMirrorConfig GetMirroringPreferredConfigForServer(Guid guid)
        {
            if (mirroringRoles == null)
            {
                return null;
            }
            if (mirroringRoles.ContainsKey(guid))
            {
                if (mirroringRoles[guid] != null)
                {
                    return mirroringRoles[guid];
                }
            }
            return null;
        }
        /// <summary>
        /// get the all preferred config for mirroring sessions on this server
        /// </summary>
        public IDictionary<Guid, ServerPreferredMirrorConfig> GetPreferredConfig
        {
            get { return mirroringRoles; }
        }
        #endregion

        #region events

        #endregion

        #region methods

        public bool GetMetricThresholdEnabled(Metric metric)
        {
            return GetMetricThresholdEnabled((int)metric);
        }

        public bool GetMetricThresholdEnabled(int metricID)
        {
            bool result = false;
            if (thresholds == null)
            {
                thresholds = Thresholds;
            }
            if (thresholds.ContainsKey(metricID))
                result = thresholds[metricID].IsEnabled;
            return result;
        }

        internal void DropQueryDataFromPreviousRefresh()
        {
            try
            {
                if (previousRefresh != null && previousRefresh.GetType() == typeof(ScheduledRefresh))
                {
                    // Clear large data we no longer need to save space
                    ((ScheduledRefresh)previousRefresh).QueryMonitorStatements.Clear();
                    ((ScheduledRefresh)previousRefresh).QueryMonitorSignatures.Clear();
                    ((ScheduledRefresh)previousRefresh).Deadlocks.Clear();
                    ((ScheduledRefresh)previousRefresh).BlockingSessions.Clear();
                }
            }
            catch (Exception e)
            {
                if (monitoredServer != null)
                {
                    LOG.Warn(String.Format("Error setting previous refresh for {0}: ", monitoredServer.InstanceName), e);
                }
                else
                {
                    LOG.Warn("Error setting previous refresh: ", e);
                }
            }
        }

        #endregion




    }
}
