//------------------------------------------------------------------------------
// <copyright file="MonitoredSQLServerWrapper.cs" company="Idera, Inc.">
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
    using System.Security.Principal;
    using Common;
    using Configuration;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Used to cache a copy of a monitored sql server object.  You should cache the wrapper
    /// so that we can slip in a changed server object.  Other properties that are server
    /// bases may also be stored in the wrapper.
    /// </summary>
    public class MonitoredSqlServerState
    {
        private MonitoredSqlServer monitoredSqlServer;
        private IDictionary<int, MetricThresholdEntry> metricThresholds;
        private Set<int> assignedCounters;

        private MonitoredSqlServerStateGraph stateGraph;
        private object sync = new object();

        public MonitoredSqlServerState(MonitoredSqlServer server)
        {
            this.monitoredSqlServer = server;

            IList<OutstandingEventEntry> events = null;
            IDictionary<int, List<MetricThresholdEntry>> thresholds;
            Dictionary<int, bool> metricCompatibilityForSqlExpress; //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
            BaselineMetricMeanCollection baselineMetricMean = null; //SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
            int? cloudProviderId = null;    

            RepositoryHelper.GetMonitoredSqlServerWorkload(
                ManagementServiceConfiguration.ConnectionString,
                monitoredSqlServer.Id,
                out thresholds,
                out events,
                out assignedCounters,
                out metricCompatibilityForSqlExpress,
                out baselineMetricMean,
                out cloudProviderId//SQLdm 10.0 (Tarun Sapra)- Added an out param for getting the cloud provider
                );


            metricThresholds = new Dictionary<int, MetricThresholdEntry>();
            // Just concerned with the Default Thresholds right now
            foreach (List<MetricThresholdEntry> mteList in thresholds.Values)
            {
                foreach (MetricThresholdEntry mte in mteList)
                {
                    if (mte.IsDefaultThreshold && !metricThresholds.ContainsKey(mte.MetricID))
                        metricThresholds.Add(mte.MetricID, mte);
                }
            }
        }

        public void Update(MonitoredServerWorkload workload)
        {
            lock (sync)
            {
                monitoredSqlServer = workload.MonitoredServer;
                metricThresholds = workload.Thresholds;
            }
        }

        public MonitoredSqlServer WrappedServer
        {
            get
            {
                lock (sync)
                {
                    return monitoredSqlServer;
                }
            }
            set
            {
                lock (sync)
                {
                    monitoredSqlServer = value;
                }
            }

        }

        public MonitoredSqlServerStateGraph StateGraph
        {
            get { return stateGraph; }
            set { stateGraph = value; }
        }

        /// <summary>
        /// Add threshold to the server and return true if this is a new metric.
        /// </summary>
        /// <param name="thresholdEntry"></param>
        /// <returns></returns>
        public bool AddMetricThresholdEntry(MetricThresholdEntry thresholdEntry)
        {
            bool result = true;
            int metricId = thresholdEntry.MetricID;
            MetricThresholdEntry entry = null;
            lock (sync)
            {
                if (metricThresholds.TryGetValue(metricId, out entry))
                {
                    metricThresholds.Remove(metricId);
                    result = !entry.IsEnabled;
                }

                metricThresholds.Add(metricId, thresholdEntry);
            }
            return result;
        }

        public MetricThresholdEntry GetMetricThresholdEntry(int metricId)
        {
            MetricThresholdEntry result = null;
            lock (sync)
            {
                metricThresholds.TryGetValue(metricId, out result);
            }
            return result;
        }

        /// <summary>
        /// Disables the threshold and returns true if the threshold used to be enabled.
        /// </summary>
        /// <param name="metricId"></param>
        /// <returns></returns>
        internal bool RemoveMetricThresholdEntry(int metricId)
        {
            // we don't actually remove the threshold in case there are queued collections that need to be processed
            MetricThresholdEntry entry = GetMetricThresholdEntry(metricId);
            if (entry != null && entry.IsEnabled)
            {
                entry.IsEnabled = false;
                return true;
            }
            return false;
        }

        public void UpdateSnoozeInfo(int[] metrics, SnoozeInfo snoozeInfo)
        {
            if (metrics != null && metrics.Length > 0)
            {
                foreach (int metricId in metrics)
                {
                    MetricThresholdEntry entry = GetMetricThresholdEntry(metricId);
                    if (entry != null)
                    {
                        AdvancedAlertConfigurationSettings settings = entry.Data as AdvancedAlertConfigurationSettings;
                        if (settings == null)
                        {
                            settings = new AdvancedAlertConfigurationSettings((Metric) metricId, entry.Data);
                            entry.Data = settings;
                        }
                        UpdateSnoozeInfo(settings, snoozeInfo);
                    }
                }
            } else
            {
                foreach (MetricThresholdEntry entry in metricThresholds.Values)
                {
                    AdvancedAlertConfigurationSettings settings = entry.Data as AdvancedAlertConfigurationSettings;
                    if (settings == null)
                    {
                        settings = new AdvancedAlertConfigurationSettings((Metric)entry.MetricID, entry.Data);
                        entry.Data = settings;
                    }
                    UpdateSnoozeInfo(settings, snoozeInfo);
                }
            }
        }

        public void UpdateSnoozeInfo(AdvancedAlertConfigurationSettings settings, SnoozeInfo newSnoozeInfo)
        {
            SnoozeInfo snoozeInfo = settings.SnoozeInfo;
            if (snoozeInfo == null)
            {
                settings.SnoozeInfo = (SnoozeInfo)newSnoozeInfo.Clone();
            } else
            {
                snoozeInfo.Change(newSnoozeInfo.StartSnoozing, newSnoozeInfo.StopSnoozing, newSnoozeInfo.UnsnoozedBy);
            }
        }

    }
    public class MonitoredSqlServerStateGraph
    {
        private readonly MonitoredSqlServerState server;
        private DateTime? lastScheduledCollectionDateTime;
        private DateTime? lastDatabaseCollectionTime;

        private Dictionary<int,IssueContainer> eventStatusMap = new Dictionary<int,IssueContainer>();

        private readonly object statusLock = new object();
        private readonly object snoozeLock = new object();

        public MonitoredSqlServerStateGraph(MonitoredSqlServerState server)
        {
            this.server = server;
            server.StateGraph = this;
        }

        public MonitoredSqlServerState MonitoredSqlServer
        {
            get { return server; }
        }

      
        public DateTime? LastAlertRefreshTime
        {
            get
            {
                if (LastDatabaseCollectionTime == null && LastScheduledRefreshTime == null)
                    return null;
                if (LastDatabaseCollectionTime == null)
                    return LastScheduledRefreshTime;
                if (LastScheduledRefreshTime == null)
                    return LastDatabaseCollectionTime;
                if (LastDatabaseCollectionTime > LastScheduledRefreshTime)
                    return LastDatabaseCollectionTime;
                if (LastScheduledRefreshTime > LastDatabaseCollectionTime)
                    return LastScheduledRefreshTime;
                return LastScheduledRefreshTime;
            }
            set
            {
                LastDatabaseCollectionTime = LastScheduledRefreshTime = value;
            }
        }

        private DateTime? LastScheduledRefreshTime
        {
            get
            {
                lock (statusLock)
                {
                    return lastScheduledCollectionDateTime;
                }
            }
            set
            {
                lock (statusLock)
                {
                    lastScheduledCollectionDateTime = value;
                }
            }
        }


        private DateTime? LastDatabaseCollectionTime
        {
            get
            {
                lock (statusLock)
                {
                    return lastDatabaseCollectionTime;
                }
            }
            set
            {
                lock (statusLock)
                {
                    lastDatabaseCollectionTime = value;
                }
            }
        }


        public DateTime? GetLastRefreshTime(Type t)
        {
            if (t == typeof(DatabaseSizeSnapshot))
                return LastDatabaseCollectionTime;
            else
                return LastScheduledRefreshTime;
        }


        public void SetLastRefreshTime(Type t,DateTime? time)
        {
            if (t == typeof(DatabaseSizeSnapshot))
                LastDatabaseCollectionTime = time;
            else
                LastScheduledRefreshTime = time;
        }


//        public Set<Pair<Triple<string,string,string>,int>> CurrentStateKeys
//        {
//            get { return new Set<Pair<Triple<string,string,string>,int>>(eventStatusMap.Keys); }
//        }

        public bool TryGetStatus(int metricId, out IssueContainer issue)
        {
            return eventStatusMap.TryGetValue(metricId, out issue);
        }

        public bool AddOrUpdateStatus(IEvent ievent, string alertHeader)
        {
            if (ievent.MetricID == (int)Metric.Deadlock)
            {
                return false;
            }

            if (ievent is StateDeviationEvent)
                return AddStatus((StateDeviationEvent) ievent, alertHeader);
            if (ievent is StateDeviationClearEvent)
                return ClearStatus(ievent);
            if (ievent is StateDeviationUpdateEvent)
                return UpdateStatus((StateDeviationUpdateEvent)ievent, alertHeader);
            return false;
        }

        public bool AddStatus(StateDeviationEvent addEvent, string alertHeader)
        {
            lock (statusLock)
            {
                MonitoredObjectName name = addEvent.MonitoredObject;

                IssueContainer container = null;
                if (!eventStatusMap.TryGetValue(addEvent.MetricID, out container))
                {
                    container = new IssueContainer();
                    eventStatusMap.Add(addEvent.MetricID, container);
                }

                ServerIssue issue = container.AddIssue(addEvent);
//                    if (addEvent.OccuranceTime >= issue.UTCLastUpdateDateTime)
//                    {
//                        issue.Update(addEvent);
//                    }
//                }

//                Pair<Triple<string,string,string>, int> eventKey = CreateKey(name, addEvent.MetricID);
//
//                ServerIssue issue = null;
//                if (eventStatusMap.TryGetValue(eventKey, out issue))
//                {
//                    if (addEvent.OccuranceTime >= issue.UTCLastUpdateDateTime)
//                    {
//                        issue.Update(addEvent);
//                    }
//                }
//                else
//                {
//                    issue = AddIssue(ref eventKey, name, addEvent);
//                }
                // save the alert header for use generating status xml document

                issue.AlertHeader = alertHeader;
            }


        return false;
        }

        public bool ClearStatus(IEvent iEvent)
        {
            lock (statusLock)
            {
                IssueContainer container = null;
                if (eventStatusMap.TryGetValue(iEvent.MetricID, out container))
                {
                    container.ClearIssue(iEvent);
                    return true;
                }
                return false;
//                
//                
//                MonitoredObjectName name = iEvent.MonitoredObject;
//                Pair<Triple<string,string,string>, int> eventKey = CreateKey(name, iEvent.MetricID);
//
//                ServerIssue issue = null;
//                if (eventStatusMap.TryGetValue(eventKey, out issue))
//                {
//                    if (iEvent.OccuranceTime >= issue.UTCLastUpdateDateTime)
//                    {
//                        issue.Update(iEvent);
//                    }
//                }
            }
        }

        private bool ClearStatus(ServerIssue issue, DateTime occurranceTime)
        {
            if (occurranceTime >= issue.UTCLastUpdateDateTime)
            {
                return issue.Clear(occurranceTime);
            }
            return false;
        }

        public bool UpdateStatus(StateDeviationUpdateEvent updateEvent, string alertHeader)
        {
            lock (statusLock)
            {
                ServerIssue issue = null;
                IssueContainer container = null;
                if (eventStatusMap.TryGetValue(updateEvent.MetricID, out container))
                {
                    issue = container.UpdateIssue(updateEvent);

//                    if (updateEvent.OccuranceTime >= issue.UTCLastUpdateDateTime)
//                    {
//                        issue.Update(updateEvent);
//                    }
                }
                else
                {
                    container = new IssueContainer();
                    eventStatusMap.Add(updateEvent.MetricID, container);
                    issue = container.AddIssue(updateEvent);
                }

                // save the alert header for use generating status xml document
                issue.AlertHeader = alertHeader;
            }
            return false;
        }

//        private ServerIssue AddIssue(ref Pair<Triple<string,string,string>, int> eventKey, MonitoredObjectName name, IEvent iEvent)
//        {
//            ServerIssue issue = null;
//
//            Pair<MetricDefinition, MetricDescription?> metricInfo = GetMetricInfo(iEvent.MetricID);
//            if (name.IsDatabase)
//            {
//                Metric metric = MetricDefinition.GetMetric(iEvent.MetricID);
//                if (MetricDefinition.IsDatabaseAlert(metric))
//                    issue = new DatabaseIssue(iEvent);
//                else
//                    issue = new ServerIssue(iEvent);
//            }
//            else
//                issue = new ServerIssue(iEvent);
//
//            issue.Rank = metricInfo.First.Rank;
//
//            // add the issue to the event status map
//            eventStatusMap.Add(eventKey, issue);
//
//            return issue;
//        }


        private static Pair<MetricDefinition, MetricDescription?> GetMetricInfo(int metricId)
        {
            MetricDefinitions defs = Management.GetMetricDefinitions();
            MetricDefinition def = defs.GetMetricDefinition(metricId);
            MetricDescription? desc = defs.GetMetricDescription(metricId);
            return new Pair<MetricDefinition, MetricDescription?>(def, desc);
        }

//        private static MetricCategory GetMetricCategory(ref Pair<MetricDefinition, MetricDescription?> metricInfo)
//        {
//            if (metricInfo.First.IsCustom)
//                return MetricCategory.Custom;
//
//            if (metricInfo.Second.HasValue)
//            {
//                try
//                {
//                    return (MetricCategory)Enum.Parse(typeof(MetricCategory), metricInfo.Second.Value.Category);
//                }
//                catch (Exception e)
//                {
////                    LOG.ErrorFormat("Error parsing metric category ({0}): {1}", metricInfo.Second.Value.Category, e.Message);
//                }
//            }
//            return MetricCategory.General;
//        }

        public override string ToString()
        {
            MonitoredSqlServer server = MonitoredSqlServer.WrappedServer;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<Server SQLServerID=\"{0}\" InstanceName=\"{1}\" MaintenanceModeEnabled=\"{2}\" ServerVersion=\"{3}\" ServerEdition=\"{4}\" LastScheduledCollectionDateTime=\"{5}\" LastReorgStatisticsRunTime=\"{6}\" ActiveWarningAlerts=\"{7}\" ActiveCriticalAlerts=\"{8}\" CustomCounterCount=\"{9}\">", 
                server.Id,
                server.InstanceName, 
                server.MaintenanceModeEnabled ? 1 : 0,
                "",
                "",
                LastAlertRefreshTime,
                server.TableFragmentationConfiguration.PreviousFragmentationStatisticsRunTime,
                0,
                0,
                server.CustomCounters.Count
                );

            builder.Append("</Server>");
            return builder.ToString();
        }

        internal void ManualClearEvent(int metricId, string databaseName, string qualifierHash, bool clearAll)
        {
            lock (statusLock)
            {
                IssueContainer issueContainer;
                if (!TryGetStatus(metricId, out issueContainer))
                {
                    issueContainer = new IssueContainer();
                    eventStatusMap.Add(metricId, issueContainer);
                }

                // if we got here then we matched
                DateTime clearTime = LastAlertRefreshTime.HasValue
                                         ? LastAlertRefreshTime.Value
                                         : DateTime.UtcNow;

                issueContainer.ManualClear(databaseName, qualifierHash, clearAll, clearTime);
            }
        }
    }

    public class IssueContainer
    {
        private static Triple<string,string,string> NULLKEY = new Triple<string,string,string>(null, null, null);

        private DateTime? lastClearAll;
        private ServerIssue singleIssue;
        private Dictionary<Triple<string,string,string>, ServerIssue> issues;
        private object sync = new object();

        public DateTime? LastClearAllDateTime
        {
            get { return lastClearAll; }
        }

        public static Triple<string, string, string> CreateKey(MonitoredObjectName mon)
        {
            return CreateKey(mon.ResourceName, mon.TableName, mon.GetQualifierHash());
        }

        public static Triple<string, string, string> CreateKey(string databaseName, string tableName, string qualifierHash)
        {
            if (databaseName == null && qualifierHash == null && tableName == null)
                return NULLKEY;

            return new Triple<string,string,string>(databaseName, tableName, qualifierHash);
        }

        public void ManualClear(string databaseName, string qualifierHash, bool clearAll, DateTime lastRefreshDateTime)
        {
            if (clearAll)
            {
                lastClearAll = lastRefreshDateTime;
                if (singleIssue != null)
                {
                    singleIssue.ClearedEventDateTime = lastRefreshDateTime;
                    singleIssue.BaseEvent.AdditionalData = null;
                }
                if (issues != null)
                {
                    foreach (ServerIssue issue in issues.Values)
                    {
                        issue.ClearedEventDateTime = lastRefreshDateTime;
                        issue.BaseEvent.AdditionalData = null;
                    }
                }
            } else
            {
                Triple<string, string, string> key = CreateKey(databaseName, null, qualifierHash);
                if (key == NULLKEY)
                {
                    if (singleIssue == null)
                    {
                        lastClearAll = lastRefreshDateTime;
                    }
                    else
                    {
                        singleIssue.ClearedEventDateTime = lastRefreshDateTime;
                        singleIssue.BaseEvent.AdditionalData = null;
                    }
                } else
                {
                    if (issues == null)
                    {
                        lastClearAll = lastRefreshDateTime;
                    } else
                    {
                        ServerIssue issue;
                        if (issues.TryGetValue(key, out issue))
                        {
                            issue.ClearedEventDateTime = lastRefreshDateTime;
                            issue.BaseEvent.AdditionalData = null;
                        } else
                        {
                            // have to add a dummy event to act as a placeholder
                            issue = new ServerIssue();
                            issue.ClearedEventDateTime = lastRefreshDateTime;
                            InsertIssue(key, issue);
                        }
                    }
                }
            }
        }

        public ServerIssue AddIssue(IEvent ievent)
        {
            ServerIssue issue = null;
            Triple<string, string, string> issueKey = CreateKey(ievent.MonitoredObject);
            lock (sync)
            {

                if (issueKey == NULLKEY)
                    issue = singleIssue;
                else
                    TryGetValue(issueKey, out issue);

                if (issue == null)
                {
                    issue = new ServerIssue(ievent);
                    if (issueKey == NULLKEY)
                        singleIssue = new ServerIssue(ievent);
                    else
                        InsertIssue(issueKey, issue);
                } else
                {
                    if (ievent.OccuranceTime >= issue.UTCLastUpdateDateTime)
                    {
                        issue.Update(ievent);
                    }
                }
            }
            return issue;
        }

        internal ServerIssue UpdateIssue(StateDeviationUpdateEvent ievent)
        {
            ServerIssue issue = null;
            Triple<string, string, string> issueKey = CreateKey(ievent.MonitoredObject);
            lock (sync)
            {

                if (issueKey == NULLKEY)
                {
                    issue = singleIssue;
                }
                else
                    TryGetValue(issueKey, out issue);

                if (issue == null)
                {
                    issue = new ServerIssue(ievent);
                    if (issueKey == NULLKEY)
                        singleIssue = new ServerIssue(ievent);
                    else
                        InsertIssue(issueKey, issue);
                } else
                {
                    if (ievent.OccuranceTime >= issue.UTCLastUpdateDateTime)
                    {
                        issue.Update(ievent);
                    }
                }
            }
            return issue;
        }

        internal void ClearIssue(IEvent ievent)
        {
            ServerIssue issue = null;
            Triple<string, string, string> issueKey = CreateKey(ievent.MonitoredObject);
            lock (sync)
            {
                if (issueKey == NULLKEY)
                {
                    issue = singleIssue;
                }
                else
                    TryGetValue(issueKey, out issue);

                if (issue == null)
                {
                    issue = new ServerIssue(ievent);
                    if (issueKey == NULLKEY)
                        singleIssue = new ServerIssue(ievent);
                    else
                        InsertIssue(issueKey, issue);
                }
                else
                {
                    if (ievent.OccuranceTime >= issue.UTCLastUpdateDateTime)
                    {
                        issue.Update(ievent);
                    }
                }
            }
        }


        private void InsertIssue(Triple<string,string,string> key, ServerIssue value)
        {
            if (issues == null)
                issues = new Dictionary<Triple<string, string, string>, ServerIssue>();
            issues.Add(key, value);
        }

        public bool TryGetValue(string databaseName, string tableName, string qualifierHash, out ServerIssue issue)
        {
            return TryGetValue(CreateKey(databaseName, tableName, qualifierHash), out issue);
        }

        public bool TryGetValue(MonitoredObjectName mon, out ServerIssue issue)
        {
            return TryGetValue(CreateKey(mon), out issue);
        }

        public bool TryGetValue(Triple<string,string,string> key, out ServerIssue issue)
        {
            if (issues == null)
            {
                issue = null;
                return false;
            }
            return issues.TryGetValue(key, out issue);
        }

    }

    public class ServerIssue
    {
        private DateTime utcLastUpdateDateTime;
        private BaseEvent baseEvent;
        private string alertHeader;
        private int rank = 50;

        // time that the cleared event was generated
        private DateTime? clearedEventOccurenceDateTime;

        public ServerIssue()
        {
            
        }

        public ServerIssue(IEvent iEvent)
        {
            if (iEvent is StateDeviationEvent)
                baseEvent = (StateDeviationEvent)iEvent;
            else
                if (iEvent is StateDeviationUpdateEvent)
                    baseEvent = ((StateDeviationUpdateEvent)iEvent).DeviationEvent;
                else
                    if (iEvent is StateDeviationClearEvent)
                    {
                        StateDeviationClearEvent clearEvent = (StateDeviationClearEvent)iEvent;
                        baseEvent = new StateDeviationEvent(clearEvent.DeviationEvent.MonitoredObject,
                                                            iEvent.MetricID,
                                                            iEvent.MonitoredState,
                                                            clearEvent.DeviationEvent.Value,
                                                            iEvent.OccuranceTime,
                                                            clearEvent.DeviationEvent.AdditionalData,
                                                            clearEvent.DeviationEvent.MetricValue);
                    }

            utcLastUpdateDateTime = iEvent.OccuranceTime;
        }

        internal BaseEvent BaseEvent
        {
            get { return baseEvent; }
        }

        internal DateTime UTCLastUpdateDateTime
        {
            get { return utcLastUpdateDateTime; }
            set { utcLastUpdateDateTime = value; }
        }

        public DateTime? ClearedEventDateTime
        {
            get { return clearedEventOccurenceDateTime;  }
            set { clearedEventOccurenceDateTime = value; }
        }

        internal void Update(IEvent iEvent)
        {
            // update the base event with the new status
            baseEvent.MonitoredState = iEvent.MonitoredState;
            baseEvent.Value = GetValue(iEvent);
            UTCLastUpdateDateTime = iEvent.OccuranceTime;
        }

        internal bool Clear(DateTime occuranceTime)
        {
            baseEvent.MonitoredState = MonitoredState.OK;
            baseEvent.Value = 0;
            UTCLastUpdateDateTime = occuranceTime;
            return false;
        }

        private static object GetValue(IEvent iEvent)
        {
            if (iEvent is StateDeviationUpdateEvent)
                iEvent = ((StateDeviationUpdateEvent)iEvent).DeviationEvent;
            else
                if (iEvent is StateDeviationClearEvent)
                    iEvent = ((StateDeviationClearEvent)iEvent).DeviationEvent;

            if (iEvent is BaseEvent)
                return ((BaseEvent)iEvent).Value;

            return 0;
        }

        public string AlertHeader
        {
            get { return alertHeader ?? String.Empty; }
            set { alertHeader = value; }
        }

        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        public static DateTime? GetIssueStart(IEvent ievent)
        {
            AgentJobFailure failedJob = ievent.AdditionalData as AgentJobFailure;
            if (failedJob != null)
                return failedJob.RunTime;

            AgentJobRunning runningJob = ievent.AdditionalData as AgentJobRunning;
            if (runningJob != null)
                return runningJob.StartedAt;

            if (ievent.MetricID == (int)Metric.ReorganisationPct)
            {
                return ievent.OccuranceTime;
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("<State Rank=\"{0}\" Metric=\"{1}\" Severity=\"{2}\" OccurrenceTime=\"{3}\" BeginTime=\"{4}\" Subject=\"{5}\" />", Rank, baseEvent.MetricID, baseEvent.MonitoredState, this.utcLastUpdateDateTime, this.baseEvent.OccuranceTime, AlertHeader);

            return builder.ToString();
        }
    }

    public class DatabaseIssue : ServerIssue
    {
        public DatabaseIssue(IEvent iEvent)
            : base(iEvent)
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("<State Database=\"{6}\" Rank=\"{0}\" Metric=\"{1}\" Severity=\"{2}\" OccurrenceTime=\"{3}\" BeginTime=\"{4}\" Subject=\"{5}\" />", Rank, BaseEvent.MetricID, BaseEvent.MonitoredState, UTCLastUpdateDateTime, BaseEvent.OccuranceTime, AlertHeader, BaseEvent.MonitoredObject.DatabaseName);

            return builder.ToString();
        }
    }
}
