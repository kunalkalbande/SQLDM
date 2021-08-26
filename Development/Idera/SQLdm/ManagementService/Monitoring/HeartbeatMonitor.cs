//------------------------------------------------------------------------------
// <copyright file="HeartbeatMonitor.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.ManagementService.Configuration;
    using BBS.TracerX;
    using Idera.SQLdm.ManagementService.Helpers;
    using Microsoft.ApplicationBlocks.Data;

    /// <summary>
    /// This clas wakes up periodically to check on collection service sessions to make sure they're
    /// still responsive and sending heartbeats.
    /// </summary>
    internal class HeartbeatMonitor : IDisposable
    {
        #region fields

        public event EventHandler CheckHeartbeats;

        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("HeartbeatMonitor");
        public static readonly TimeSpan HEARTBEAT_INTERVAL = ManagementServiceConfiguration.HeartbeatInterval;
        private static readonly TimeSpan HEARTBEAT_CHECK_MAX_ELAPSED = TimeSpan.FromSeconds(ManagementServiceConfiguration.HeartbeatInterval.TotalSeconds * 1.5);
        private static readonly TimeSpan HEARTBEAT_CHECK_INTERVAL = new TimeSpan(0, 0, 30);
        private static readonly TimeSpan HEARTBEAT_CHECK_MAXDELIVERY_TIME = new TimeSpan(0, 15, 0);

        private Timer heartbeatCheckTimer;

        #endregion

        #region constructors

        public HeartbeatMonitor()
        {
            // Tolga K - to fix memory leak begins
            if (heartbeatCheckTimer != null)
            {
                heartbeatCheckTimer.Dispose();
            }
            // Tolga K - to fix memory leak ends

            heartbeatCheckTimer = new Timer(new TimerCallback(FireCheckHeartbeats), null, HEARTBEAT_CHECK_INTERVAL, HEARTBEAT_CHECK_INTERVAL);
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Updates the heartbeat.
        /// </summary>
        /// <param name="context">The session.</param>
        public void UpdateHeartbeat(CollectionServiceContext context, DateTime lastHeartbeat, DateTime? lastSnapshotDeliveryAttempt, TimeSpan? lastSnapshotDeliveryAttemptTime, Exception lastSnapshotDeliveryException, int scheduledRefreshDeliveryTimeoutCount)
        {
            // update the next expected time for a heartbeat to be received
            context.LastHeartbeatReceived = lastHeartbeat;
            // update info about scheduled refresh deliveries
            context.SetLastScheduledDeliveryInfo(lastSnapshotDeliveryAttempt, lastSnapshotDeliveryAttemptTime, lastSnapshotDeliveryException, scheduledRefreshDeliveryTimeoutCount);
            try
            {
                // persist the value
                SqlHelper.ExecuteNonQuery(ManagementServiceConfiguration.ConnectionString,
                                          "p_HeartbeatCollectionService",
                                           context.ServiceId,
                                          lastHeartbeat);
            } catch (Exception e)
            {
                LOG.Error("Unable to update last heartbeat for collection service: ", e);

                // if the connection failed then test the current repository connection so that issues get logged
                RepositoryHelper.TestRepositoryConnection(ManagementServiceConfiguration.ConnectionString, Management.EventLog);
            } 
        }

        /// <summary>
        /// Fires the check heartbeats.
        /// </summary>
        /// <param name="state">The state.</param>
        private void FireCheckHeartbeats(object state)
        {
            if (CheckHeartbeats != null)
                CheckHeartbeats(this, null);
        }

        public void CheckHeartbeart(CollectionServiceContext context)
        {
            try
            {
                ThresholdViolationEvent anevent = PersistenceManager.Instance.GetOperationalEvent(Metric.SQLdmCollectionServiceStatus, context.ServiceId);

                List<IEvent> eventList = null;

                if (!IsSessionCurrent(context))
                {   // generate events for heartbeat timeouts
                    IEnumerable<IEvent> events = RaiseHeartbeatMissedAlert(context, anevent);
                    if (events != null)
                    {
                        eventList = new List<IEvent>(events);
                    }
                }
                else
                    if (IsHavingDeliveryProblems(context))
                    {   // generate events for delivery issues reported via heartbeat interface
                        IEnumerable<IEvent> events = RaiseDeliveryProblemsAlert(context, anevent);
                        if (events != null)
                        {
                            eventList = new List<IEvent>(events);
                        }
                    }
                    else
                    {
                        if (anevent != null)
                        {   // create clear events
                            LOG.WarnFormat("Collection service status alert cleared ({0}${1})", context.CollectionService.MachineName, context.CollectionService.InstanceName);
                            // clear the event from state
                            PersistenceManager.Instance.ClearOperationalEvent(Metric.SQLdmCollectionServiceStatus, context.ServiceId);
                            // create deviation event
                            StateDeviationEvent deviationEvent = new StateDeviationEvent(
                                            new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                                            (int)Metric.SQLdmCollectionServiceStatus,
                                            MonitoredState.Critical,
                                            anevent.Value,
                                            anevent.OccuranceTime,
                                            anevent.AdditionalData,
                                            anevent.IsBaselineEnabled,
                                            anevent.MetricValue);
                            // change event value to running
                            deviationEvent.Value = SQLdmServiceState.Running;
                            // create the clear event
                            StateDeviationClearEvent sdce = new StateDeviationClearEvent(deviationEvent, DateTime.UtcNow);
                            if (eventList == null)
                                eventList = new List<IEvent>();
                            eventList.Add(sdce);
                        }
                    }

                if (eventList != null && eventList.Count > 0)
                {
                    AlertTableWriter.LogThresholdViolations(eventList);
                    Management.Notification.Process(eventList, null);
                }
            }
            catch (Exception ex)
            {
                
                LOG.Error("Exception checking collection service heartbeat.",ex);
            }
        }

        private IEnumerable<IEvent> RaiseDeliveryProblemsAlert(CollectionServiceContext context, ThresholdViolationEvent anevent)
        {
            StateDeviationEvent sde = null;
            StateDeviationUpdateEvent sdue = null;
            DateTime now = DateTime.UtcNow;

            OperationalThresholdViolationEvent trackedEvent = anevent as OperationalThresholdViolationEvent;

            LOG.ErrorFormat("Heartbeat reporting delivery problems from Collection Service ({0}${1}) LastDelivery={2} LastDeliveryTime={3} SnapshotTimeouts={4} LastDeliveryException={5}",
                context.CollectionService.MachineName,
                context.CollectionService.InstanceName,
                context.LastScheduledRefreshDeliveryTime.HasValue ? (object)context.LastScheduledRefreshDeliveryTime : "None",
                context.LastScheduledRefreshDeliveryDuration.HasValue ? (object)context.LastScheduledRefreshDeliveryDuration : "None",
                context.ScheduledRefreshDeliveryTimeoutCount,
                context.LastScheduledRefreshDeliveryException != null ? (object)context.LastScheduledRefreshDeliveryException : "None");

            String addlInfo = String.Format("The Collection Service is reporting that it is having problems delivering scheduled refresh data.  The last attempt was {0} and it took {1} for the remote call to complete.", context.LastScheduledRefreshDeliveryTime, context.LastScheduledRefreshDeliveryDuration);
            if (context.ScheduledRefreshDeliveryTimeoutCount > 0)
                addlInfo += String.Format("  The number of call timeouts since a successful delivery is {0}", context.ScheduledRefreshDeliveryTimeoutCount);
            else
                if (context.LastScheduledRefreshDeliveryException != null)
                    addlInfo += String.Format("  The last exception is: {0}", context.LastScheduledRefreshDeliveryException);

            object[] addlData = new object[] { "Suspect", addlInfo };

            if (anevent == null)
            {

                sde = new StateDeviationEvent(
                    new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                    (int)Metric.SQLdmCollectionServiceStatus,
                    MonitoredState.Critical,
                    200,
                    now,
                    addlData);

                trackedEvent = new OperationalThresholdViolationEvent(
                    new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                    (int)Metric.SQLdmCollectionServiceStatus,
                    MonitoredState.Critical,
                    200,    // value will map to message format 2
                    now,
                    addlData);
            }
            else
            {
                if (GetLastUpdate(anevent, now) + TimeSpan.FromMinutes(6) > now)
                    return null;

                StateDeviationEvent deviationEvent = new StateDeviationEvent(
                        new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                        (int)Metric.SQLdmCollectionServiceStatus,
                        MonitoredState.Critical,
                        200,
                        anevent.OccuranceTime,
                        addlData);

                sdue = new StateDeviationUpdateEvent(deviationEvent, now);
                sdue.PreviousState = anevent.MonitoredState;

                if (trackedEvent == null)
                    trackedEvent = new OperationalThresholdViolationEvent(trackedEvent);
                trackedEvent.LastUpdate = now;
            }

            PersistenceManager.Instance.AddUpdateOperationalEvent(Metric.SQLdmCollectionServiceStatus,
                                                                  context.ServiceId,
                                                                  trackedEvent);

            List<IEvent> events = new List<IEvent>();

            if (sde != null)
                events.Add(sde);
            if (sdue != null)
                events.Add(sdue);

            return events;
        }
       

        private IEnumerable<IEvent> RaiseHeartbeatMissedAlert(CollectionServiceContext context, ThresholdViolationEvent anevent)
        {
            StateDeviationEvent sde = null;
            StateDeviationUpdateEvent sdue = null;
            DateTime now = DateTime.UtcNow;

            OperationalThresholdViolationEvent trackedEvent = anevent as OperationalThresholdViolationEvent;

            LOG.WarnFormat("Heartbeat missed from Collection Service ({0}${1})", context.CollectionService.MachineName, context.CollectionService.InstanceName);
            if (anevent == null)
            {
                sde = new StateDeviationEvent(
                    new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                    (int)Metric.SQLdmCollectionServiceStatus,
                    MonitoredState.Critical,
                    SQLdmServiceState.Undetermined,
                    now,
                    context.ServiceId);

                trackedEvent = new OperationalThresholdViolationEvent(
                    new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                    (int)Metric.SQLdmCollectionServiceStatus,
                    MonitoredState.Critical,
                    SQLdmServiceState.Undetermined,
                    now,
                    context.ServiceId);
            }
            else
            {
                if (GetLastUpdate(anevent, now) + TimeSpan.FromMinutes(6) > now)
                    return null;

                StateDeviationEvent deviationEvent = new StateDeviationEvent(
                            new MonitoredObjectName(String.Format("{0}\\{1}", context.CollectionService.MachineName.Trim(), context.CollectionService.InstanceName.Trim()), "", ""),
                            (int)Metric.SQLdmCollectionServiceStatus,
                            MonitoredState.Critical,
                            SQLdmServiceState.Undetermined,
                            anevent.OccuranceTime,
                            context.ServiceId);

                sdue = new StateDeviationUpdateEvent(deviationEvent, now);
                sdue.PreviousState = anevent.MonitoredState;

                if (trackedEvent == null)
                    trackedEvent = new OperationalThresholdViolationEvent(trackedEvent);
                trackedEvent.LastUpdate = now;
            }

            PersistenceManager.Instance.AddUpdateOperationalEvent(Metric.SQLdmCollectionServiceStatus, context.ServiceId, trackedEvent);

            List<IEvent> events = new List<IEvent>();
            
            if (sde != null)
                events.Add(sde);
            if (sdue != null)
                events.Add(sdue);

            return events;
        }

        private DateTime GetLastUpdate(ThresholdViolationEvent anevent, DateTime now)
        {
            if (anevent is OperationalThresholdViolationEvent)
                return ((OperationalThresholdViolationEvent)anevent).LastUpdate;

            if (anevent == null) return now;
            
            return anevent.OccuranceTime;
        }

        private bool IsHavingDeliveryProblems(CollectionServiceContext context)
        {
            if (context.LastScheduledRefreshDeliveryTime.HasValue)
                return (context.LastScheduledRefreshDeliveryTime.Value + HEARTBEAT_CHECK_MAXDELIVERY_TIME > DateTime.Now && context.ScheduledRefreshDeliveryTimeoutCount > 0);
            return false;
        }

        /// <summary>
        /// Checks the heartbeat.
        /// </summary>
        /// <param name="context">The session.</param>
        public bool IsSessionCurrent(CollectionServiceContext context)
        {
            return (context.NextHeartbeatExpected > DateTime.UtcNow);
        }

        #region IDisposable Members

        public void Dispose()
        {
            heartbeatCheckTimer.Dispose();
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
