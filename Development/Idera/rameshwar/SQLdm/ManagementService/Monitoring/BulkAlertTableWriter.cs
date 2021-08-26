//------------------------------------------------------------------------------
// <copyright file="BulkAlertTableWriter.cs" company="Idera, Inc.">
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
    using System.Data;
    using System.Data.SqlClient;
    using Common;
    using Data;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Thresholds;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.Common.CWFDataContracts;

    public static class BulkAlertTableWriter
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("BulkAlertTableWriter");
        private static readonly DataTable alertTable = new DataTable("Alerts");

        private const int SubjectColumnLength = 256;
        private const int BodyColumnLength = 2048;

        static BulkAlertTableWriter()
        {
            DataColumn idColumn = alertTable.Columns.Add("AlertID", typeof(int));
            alertTable.Columns.Add("UTCOccurrenceDateTime", typeof(DateTime));
            alertTable.Columns.Add("ServerName", typeof(string));
            alertTable.Columns.Add("DatabaseName", typeof(string));
            alertTable.Columns.Add("TableName", typeof(string));
            alertTable.Columns.Add("Active", typeof(bool));
            alertTable.Columns.Add("Metric", typeof(int));
            alertTable.Columns.Add("Severity", typeof(byte));
            alertTable.Columns.Add("StateEvent", typeof(byte));
            alertTable.Columns.Add("Value", typeof(double));
            alertTable.Columns.Add("Heading", typeof(string));
            alertTable.Columns.Add("Message", typeof(string));
            alertTable.Columns.Add("QualifierHash", typeof(string));
            alertTable.Columns.Add("LinkedData", typeof(Guid));
        }

        public static void ProcessAlerts(SqlConnection connection, ScheduledCollectionQueue.ScheduledCollectionData data)
        {
            int result = 0;
            DateTime startTime = DateTime.UtcNow;


            AlertableSnapshot snapshot = data.Message.Snapshot as AlertableSnapshot;
            if (snapshot == null)
                throw new ArgumentException("Snapshot in the message is not the correct type", "data.Message.Snapshot");

            // figure out the timestamp for this scheduled collection
            DateTime? serverTime = snapshot.TimeStamp;
            if (!serverTime.HasValue)
            {
                IEvent firstEvent = snapshot.GetEvent(0);
                if (firstEvent != null)
                    serverTime = firstEvent.OccuranceTime;
            }
            DateTime timestamp = serverTime.HasValue ? serverTime.Value : DateTime.UtcNow;

            string refreshServerName = snapshot.ServerName;
            string monitoredServerName = refreshServerName;

            DataTable dataTable = null;
            Exception innerException = null;
            string qualifierHash = null;

            using (LOG.DebugCall(String.Format("ProcessAlerts [{0}]", refreshServerName)))
            {
                if (snapshot.Events == null || snapshot.NumberOfEvents == 0)
                {
                    LOG.Debug("Snapshot had no events to process...");
                    UpdateLastScheduledCollectionTime(connection, snapshot.Id, timestamp, snapshot.GetType());
                    return;
                }

                MonitoredSqlServerState serverState =
                    Management.ScheduledCollection.GetCachedMonitoredSqlServer(snapshot.Id);
                MonitoredSqlServerStateGraph stateGraph = serverState.StateGraph;
                DateTime? lastRefreshTime = stateGraph.GetLastRefreshTime(snapshot.GetType());

                if (serverState.WrappedServer.MaintenanceModeEnabled)
                {
                    lastRefreshTime = stateGraph.GetLastRefreshTime(snapshot.GetType());
                    if (lastRefreshTime == null || timestamp > lastRefreshTime.Value)
                    {
                        LOG.InfoFormat("Skipping alerts for scheduled refresh because '{0} is in maintenance mode.", monitoredServerName);
                        return;
                    }
                }

                int eventsToSkip = data.AlertRestartPoint;
                int eventsProcessed = 0;
                //bool clearAll = false;

                List<IEvent> nonClearingAlerts = new List<IEvent>();

                LOG.Debug("Writing threshold violations to Alerts table...");

                Transition transition = Transition.Warning_Warning;
                MonitoredState previousState = MonitoredState.None;

                bool stateChanged = false;
                IEvent baseEvent = null;

                MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();

                int debugStep = 0;
                int debugStep2 = 0;
                try
                {
                    //Dictionary<int, bool> dictOfBaselineFlag = RepositoryHelper.GetBaselineFlagForAllAlertableMetrics(ManagementServiceConfiguration.ConnectionString, snapshot.Id);//SQLdm 10.0 (Tarun Sapra)- Alert msg when baseline alerts are enabled
                    foreach (IEvent ievent in snapshot.Events)
                    {
                        if (AddAdvanceFilteringAlert(connection, monitoredServerName, ievent))
                        {
                            debugStep = debugStep2 = 0;
                            // bulk insert processing
                            if (eventsProcessed < eventsToSkip)
                            {   // skip event already processed
                                LOG.DebugFormat("Skipping event (already processed): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                                eventsProcessed++;
                                continue;
                            }

                            // write all events to the program log 
                            LOG.DebugFormat("Handling event: [{0}]{1}", monitoredServerName ?? "no server", ievent);

                            debugStep = 1;
                            int metricID = ievent.MetricID;
                            Metric metric = MetricDefinition.GetMetric(metricID);

                            debugStep = 2;
                            MetricDefinition metricDefinition = metricDefinitions.GetMetricDefinition(metricID);

                            if (metricDefinition == null || metricDefinition.IsDeleted)
                            {   // metric definition must exist and not be marked as deleted
                                eventsProcessed++;
                                continue;
                            }

                            debugStep = 3;
                            CustomCounterDefinition counterDefinition = metricDefinitions.GetCounterDefinition(metricID);
                            if (counterDefinition != null && !counterDefinition.IsEnabled)
                            {   // counter definition (custom counters only) must be enabled
                                eventsProcessed++;
                                continue;
                            }

                            // get the threshold entry for this metric
                            debugStep = 4;
                            AdvancedAlertConfigurationSettings advancedSettings = null;
                            MetricThresholdEntry thresholdEntry = serverState != null ? serverState.GetMetricThresholdEntry(metricID) : null;
                            if (thresholdEntry != null && thresholdEntry.Data != null)
                            {
                                advancedSettings = thresholdEntry.Data as AdvancedAlertConfigurationSettings;
                                if (advancedSettings == null)
                                    advancedSettings = new AdvancedAlertConfigurationSettings(metric, thresholdEntry.Data);
                            }

                            debugStep = 5;
                            if (advancedSettings != null)
                            {
                                SnoozeInfo snoozeInfo = advancedSettings.SnoozeInfo;
                                if (snoozeInfo != null && snoozeInfo.IsSnoozed(timestamp))
                                {
                                    LOG.DebugFormat("Skipping event (snoozing {2} to {3}): [{0}]{1} ", monitoredServerName ?? "no server", ievent, snoozeInfo.StartSnoozing, snoozeInfo.StopSnoozing);
                                    eventsProcessed++;
                                    continue;
                                }
                            }


                            if (ievent.OccuranceTime > lastRefreshTime)
                            {
                                // special handling for job alerts
                                switch (metric)
                                {
                                    case Metric.BombedJobs:
                                    case Metric.LongJobs:
                                    case Metric.LongJobsMinutes:
                                    case Metric.ReorganisationPct:
                                    case Metric.JobCompletion:
                                        {
                                            ServerIssue serverIssue = null;
                                            IssueContainer issueContainer = null;

                                            DateTime originationDateTime = GetEventOriginationDateTime(ievent);

                                            if (stateGraph.TryGetStatus(metricID, out issueContainer))
                                            {
                                                if (issueContainer.LastClearAllDateTime.HasValue)
                                                {
                                                    if (originationDateTime <= issueContainer.LastClearAllDateTime.Value)
                                                    {
                                                        eventsProcessed++;
                                                        LOG.DebugFormat("Skipping event (manually cleared): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                                                        continue;
                                                    }
                                                }
                                                if (issueContainer.TryGetValue(ievent.MonitoredObject, out serverIssue))
                                                {
                                                    if (originationDateTime <= serverIssue.ClearedEventDateTime)
                                                    {
                                                        eventsProcessed++;
                                                        LOG.DebugFormat("Skipping event (manually cleared): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                                                        continue;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                            }

                            debugStep = 6;
                            stateChanged = ievent.StateChanged;

                            if (ievent is StateDeviationEvent)
                                previousState = MonitoredState.OK;
                            else
                            if (ievent is StateDeviationClearEvent)
                                previousState = ((StateDeviationClearEvent)ievent).DeviationEvent.MonitoredState;
                            else
                                if (ievent is StateDeviationUpdateEvent)
                                previousState = ((StateDeviationUpdateEvent)ievent).PreviousState;

                            debugStep = 7;
                            // sanity check
                            if (!stateChanged && previousState == MonitoredState.OK)
                            {
                                LOG.DebugFormat("Skipping event (OK->OK): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                                eventsProcessed++;
                                continue;
                            }

                            // get the stored state for this alert
                            //                        issue = null;
                            //                        if (stateGraph.TryGetStatus(ievent.MonitoredObject, ievent.MetricID, out issue))
                            //                        {
                            //                            bool isSnoozed = AlertFilterHelper.IsSnoozed(advancedSettings, ievent.OccuranceTime);
                            //                            if (isSnoozed)
                            //                            {
                            //                                stateGraph.ClearStatus(ievent);
                            //                                LOG.DebugFormat("Skipping event (snoozed): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                            //                                eventsProcessed++;
                            //                                continue;
                            //                            }
                            //
                            //                            if (ievent is StateDeviationUpdateEvent)
                            //                            {
                            //                                bool isSmoothed = AlertFilterHelper.IsSmoothed(advancedSettings, (StateDeviationUpdateEvent)ievent);
                            //                                if (isSmoothed)
                            //                                {
                            //                                    stateGraph.ClearStatus(ievent);
                            //                                    LOG.DebugFormat("Skipping event (smoothed): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                            //                                    eventsProcessed++;
                            //                                    continue;
                            //                                }
                            //                            }
                            //
                            //                            AlertFilterReason filterReason = 
                            //                                AlertFilterHelper.IsFiltered(advancedSettings, 
                            //                                    ievent.MonitoredObject,
                            //                                    ievent.AdditionalData); 
                            //                            if (filterReason != AlertFilterReason.NotFiltered)
                            //                            {
                            //                                stateGraph.ClearStatus(ievent);
                            //                                LOG.DebugFormat("Skipping event (filtered): [{0}]{1} ", monitoredServerName ?? "no server", ievent);
                            //                                eventsProcessed++;
                            //                                continue;
                            //                            }
                            //                        }


                            debugStep = 8;
                            double? value = null;
                            try
                            {
                                baseEvent = ievent;
                                if (baseEvent != null)
                                {
                                    debugStep2 = 1;
                                    bool active = GetActive(baseEvent.MetricID, baseEvent.MonitoredState);
                                    if (!active)
                                    {
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
                                    }

                                    //                                if (!clearAll)
                                    //                                    clearAll = GetClearAllAlerts(baseEvent.MetricID, baseEvent.MonitoredState);

                                    MonitoredObjectName monitoredObject = baseEvent.MonitoredObject;
                                    value = ToDouble(baseEvent.Value);
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
                                        header = messageMap.FormatMessage(snapshot, baseEvent, MessageType.Header);
                                        body = messageMap.FormatMessage(snapshot, baseEvent, MessageType.Body);
                                        if (baseEvent.IsBaselineEnabled)
                                        {
                                            LOG.Info("For metric " + baseEvent.MetricID + " alert message text is changed.");
                                            header = header.Insert(0, Common.Constants.BaselineAlertText);
                                            body = body.Insert(0, Common.Constants.BaselineAlertText);
                                        }
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
                                    if (ievent.MonitoredState == MonitoredState.OK || ievent is StateDeviationClearEvent)
                                        stateGraph.ClearStatus(ievent);
                                    else
                                        stateGraph.AddOrUpdateStatus(ievent, header);

                                    if (refreshServerName == null)
                                    {
                                        if (metricDefinition.MetricClass != MetricClass.Processes)
                                        {
                                            monitoredServerName = monitoredObject.ServerName;
                                        }
                                    }
                                    debugStep2 = 6;
                                    qualifierHash = monitoredObject.GetQualifierHash();

                                    if (dataTable == null)
                                        dataTable = alertTable.Clone();

                                    debugStep2 = 7;
                                    DataRow row = dataTable.NewRow();
                                    //SQLdm (Tushar)--Fix for issue SQLDM-28167
                                    BlockingSession bs = baseEvent.AdditionalData as BlockingSession;
                                    row["AlertID"] = DBNull.Value;
                                    row["UTCOccurrenceDateTime"] = baseEvent.OccuranceTime;
                                    row["ServerName"] = monitoredServerName;
                                    row["DatabaseName"] = String.IsNullOrEmpty(monitoredObject.DatabaseName)
                                                               ? (object)DBNull.Value
                                                               : monitoredObject.DatabaseName;
                                    //Start-SQLdm (Tushar)--Fix for issue SQLDM-28167 -- Saving UTC time of start of blocking session in the TableName field of 
                                    //Alerts table, so that desktop client can retrieve this time and update the message header and body when displaying the alert on UI.
                                    row["TableName"] = baseEvent.MetricID == 33 ? bs.BlockingStartTimeUTC.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : String.IsNullOrEmpty(monitoredObject.TableName)
                                                               ? (object)DBNull.Value
                                                               : monitoredObject.DatabaseName;
                                    debugStep2 = 8;
                                    row["Active"] = GetActive(metricID, baseEvent.MonitoredState);
                                    row["Metric"] = baseEvent.MetricID;
                                    row["Severity"] = baseEvent.MonitoredState;
                                    row["StateEvent"] = transition;
                                    row["Value"] = value == null ? (object)DBNull.Value : value.Value;
                                    row["Heading"] = header;
                                    row["Message"] = body;
                                    row["QualifierHash"] = String.IsNullOrEmpty(qualifierHash) ? (object)DBNull.Value : qualifierHash;

                                    switch (baseEvent.MetricID)
                                    {
                                        case ((int)Metric.Deadlock):
                                            var deadlock = baseEvent.AdditionalData as DeadlockInfo;
                                            if (deadlock != null && deadlock.GetId() != Guid.Empty)
                                                row["LinkedData"] = deadlock.GetId();
                                            else
                                                row["LinkedData"] = DBNull.Value;
                                            break;
                                        case ((int)Metric.BlockingAlert):
                                            var block = baseEvent.AdditionalData as BlockingSession;
                                            if (block != null && block.BlockID != Guid.Empty)
                                                row["LinkedData"] = block.BlockID;
                                            else
                                                row["LinkedData"] = DBNull.Value;
                                            break;
                                        default:
                                            row["LinkedData"] = DBNull.Value;
                                            break;
                                    }


                                    debugStep2 = 9;
                                    dataTable.Rows.Add(row);

                                    eventsProcessed++;

                                    //AddAdvanceFilteringAlert(connection, baseEvent.OccuranceTime, monitoredServerName, baseEvent.MetricID, baseEvent.PreviousState, baseEvent.MonitoredState);
                                }
                            }
                            catch (Exception e1)
                            {
                                LOG.ErrorFormat("Exception building bulk copy row list (debugStep2={0}) {1}", debugStep2, e1);
                                innerException = e1;
                                throw;
                            }

                            debugStep = debugStep2 = 0;
                            if (dataTable != null && dataTable.Rows.Count > 0)
                            {
                                ServerVersion version = new ServerVersion(connection.ServerVersion);
                                debugStep = 1;
                                using (SqlTransaction xa = connection.BeginTransaction(IsolationLevel.Serializable))
                                {
                                    //                            long highestAlertId = RepositoryHelper.GetHighestAlertID(xa, null);
                                    //                            LOG.VerboseFormat("High alertID prior to bulk copy is {0}", highestAlertId);
                                    try
                                    {
                                        LOG.VerboseFormat("About to bulk copy {0} alerts", dataTable.Rows.Count);
                                        SqlBulkCopyOptions options = version.Major >= 9
                                                                         ? SqlBulkCopyOptions.Default
                                                                         : SqlBulkCopyOptions.Default |
                                                                           SqlBulkCopyOptions.CheckConstraints;
                                        debugStep2 = 1;
                                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, options, xa))
                                        {
                                            bulkCopy.DestinationTableName = "Alerts";
                                            bulkCopy.WriteToServer(dataTable);
                                        }
                                        debugStep2 = 2;
                                        if (lastRefreshTime == null || timestamp > lastRefreshTime)
                                        {
                                            if (serverState.WrappedServer.MaintenanceModeEnabled)
                                            {
                                                LOG.WarnFormat("Skipping update of last refresh time because '{0}' is now in maintenance mode.",
                                                    monitoredServerName);
                                            }
                                            else
                                            {
                                                RepositoryHelper.UpdateLastRefreshTime(xa, snapshot.Id, timestamp, snapshot.GetType());
                                            }
                                            // update the last scheduled refresh time in the state graph
                                            stateGraph.SetLastRefreshTime(snapshot.GetType(), timestamp);
                                        }

                                        //                                if (clearAll)
                                        //                                {
                                        //                                    ClearAlerts(xa, refreshServerName, highestAlertId);
                                        //                                }
                                        //                                else
                                        //                                {
                                        //                                   ClearLikeAlerts(xa, refreshServerName, highestAlertId);
                                        //                                   if (nonClearingAlerts.Count > 0)
                                        //                                       ClearNonClearingAlerts(xa, nonClearingAlerts, highestAlertId);
                                        //                                }
                                        debugStep2 = 3;
                                        xa.Commit();
                                        data.AlertsWritten = true;
                                        dataTable = null;
                                    }
                                    catch (Exception e3)
                                    {
                                        try
                                        {
                                            xa.Rollback();
                                        }
                                        catch
                                        {
                                            /* */
                                        }
                                        LOG.ErrorFormat("Exception doing bulk copy to the alert log (debugStep2={0}) {1}", debugStep2, e3);
                                        innerException = e3;
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                LOG.Debug("All alerts were suppressed or filtered out... Updating last collection timestamp.");
                                UpdateLastScheduledCollectionTime(connection, snapshot.Id, timestamp, snapshot.GetType());
                                data.AlertsWritten = true;
                            }
                        }
                        else
                        {
                            data.AlertsWritten = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (innerException != null)
                        LOG.ErrorFormat("Exception writing threshold violations to the alert log: (debugStep={0}) {1}", debugStep, e);
                    else
                        LOG.ErrorFormat("Unhandled exception writing threshold violations to the alert log: (debugStep={0}) {1}", debugStep, e);

                    throw;
                }
                finally
                {
                    if (dataTable != null)
                        dataTable.Dispose();
                }

                LOG.VerboseFormat("State Graph: [{0}] {1}", monitoredServerName ?? "no server", stateGraph);
            }
        }
        internal static bool AddAdvanceFilteringAlert(SqlConnection connection, string monitoredServerName, IEvent ievent )
        {
            bool rollback = false;
            bool result = false;
            int MetricID;
            MonitoredState MonitoredState;
            string DataBaseName;
            if (connection != null)
            {
                using (SqlTransaction xa = connection.BeginTransaction())
                {
                    try
                    {
                        if (ievent != null && ievent.MonitoredObject != null)
                        {
                            MetricID = ievent.MetricID;
                            MonitoredState = ievent.MonitoredState;
                            DataBaseName = ievent.MonitoredObject.DatabaseName;

                            result = RepositoryHelper.AddRecoredAdvanceFilteringAlert(xa, monitoredServerName, MetricID, MonitoredState, DataBaseName);
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat("Error Add Advance Filtering Alert: ", e);
                        rollback = true;
                    }
                    finally
                    {
                        if (xa != null)
                        {
                            try
                            {
                                if (rollback)
                                    xa.Rollback();
                                else
                                    xa.Commit();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            return result;
        }

        internal static void UpdateLastScheduledCollectionTime(SqlConnection connection, int instanceId, DateTime collectionTime, Type refreshType)
        {
            using (LOG.InfoCall("UpdateLastScheduledCollectionTime"))
            {
                bool rollback = false;

                using (SqlTransaction xa = connection.BeginTransaction())
                {
                    try
                    {
                        RepositoryHelper.UpdateLastRefreshTime(xa, instanceId, collectionTime, refreshType);
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat("Error updating last scheduled collection time: ", e);
                        rollback = true;
                    }
                    finally
                    {
                        if (xa != null)
                        {
                            try
                            {
                                if (rollback)
                                    xa.Rollback();
                                else
                                    xa.Commit();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        private static void ClearNonClearingAlerts(SqlTransaction xa, List<IEvent> nonClearingAlerts, long highestAlertId)
        {
            try
            {
                foreach (BaseEvent baseEvent in nonClearingAlerts)
                {
                    RepositoryHelper.ClearAlerts(
                        xa,
                        baseEvent.MonitoredObject.ServerName,
                        highestAlertId,
                        true,
                        baseEvent.MetricID,
                        null);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DateTime GetEventOriginationDateTime(IEvent ievent)
        {
            if (ievent is StateDeviationUpdateEvent)
                return ((StateDeviationUpdateEvent)ievent).DeviationEvent.OccuranceTime;
            return ievent.OccuranceTime;
        }
        private static void ClearLikeAlerts(SqlTransaction xa, string refreshServerName, long highestAlertId)
        {
            RepositoryHelper.ClearAlerts(xa, refreshServerName, highestAlertId, false, null, null);
        }

        private static void ClearAlerts(SqlTransaction xa, string refreshServerName, long highestAlertId)
        {
            RepositoryHelper.ClearAlerts(xa, refreshServerName, highestAlertId, true, null, null);
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

        private static bool GetActive(int metricID, MonitoredState severity)
        {
            return (metricID != (int)Metric.Operational) && (severity > MonitoredState.OK);
        }

        public static bool GetClearAllAlerts(int metricID, MonitoredState severity)
        {
            switch (metricID)
            {
                case (int)Metric.SqlServiceStatus:
                    return severity > MonitoredState.OK;
                case (int)Metric.MaintenanceMode:
                    return true;
            }
            return false;
        }
    }
}
