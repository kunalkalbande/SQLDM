//------------------------------------------------------------------------------
// <copyright file="HeartbeatSender.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.ManagementService
{
    using System;
    using System.Threading;

    using BBS.TracerX;

    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.CollectionService.Configuration;

    /// <summary>
    /// Class responsible for sending heartbeats to the management service for the duration of a session.
    /// </summary>
    internal class HeartbeatSender : IDisposable
    {
        #region fields

        private Timer     heartbeatTimer;
        private DateTime  lastHeartbeat;
        private DateTime  lastScheduledRefreshDelivered;
        private TimeSpan  lastScheduledRefreshDeliveryTime;
        private Exception lastScheduledRefreshDeliveryException;
        private int scheduledRefreshDeliveryTimeoutCount;

        private object sync = new object();

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("HeartbeatSender");

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts sending heartbeats on a regular schedule.
        /// </summary>
        public void Start()
        {
            TimeSpan interval = CollectionServiceConfiguration.HeartbeatInterval;

            // Tolga K - to fix memory leak begins
            if (heartbeatTimer != null)
            {
                heartbeatTimer.Dispose();
            }
            // Tolga K - to fix memory leak ends

            heartbeatTimer = new Timer(new TimerCallback(SendHeartbeat), null, TimeSpan.Zero, interval);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Reconfigure()
        {
            if (heartbeatTimer == null)
                return;

            TimeSpan interval = CollectionServiceConfiguration.HeartbeatInterval;
            // time since last heartbeat attempt
            TimeSpan span = DateTime.Now - lastHeartbeat;
            // remaining untin next heartbeat
            span = span - interval;
            // as milliseconds
            long nextHeartbeat = (long)span.TotalMilliseconds;
            if (nextHeartbeat < 0)
                nextHeartbeat = 0;           
            // update the timer
            heartbeatTimer.Change(nextHeartbeat, (long)interval.TotalMilliseconds);
        }

        public void UpdateScheduledRefreshData(DateTime lastRefreshDelivered, TimeSpan lastRefreshTime, Exception lastRefreshException)
        {
            lock (sync)
            {
                lastScheduledRefreshDelivered = lastRefreshDelivered;
                lastScheduledRefreshDeliveryTime = lastRefreshTime;
                lastScheduledRefreshDeliveryException = lastRefreshException;

                if (lastRefreshException is TimeoutException)
                    scheduledRefreshDeliveryTimeoutCount++;
                else
                    scheduledRefreshDeliveryTimeoutCount = 0;
            }
        }

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SendHeartbeat(object state)
        {
            LOG.Debug("Sending heartbeat to Management Service");

            IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();

            TimeSpan nextHeartbeat = CollectionServiceConfiguration.HeartbeatInterval;

            DateTime lastRefreshDelivered;
            TimeSpan lastRefreshTime;
            Exception lastRefreshException;
            int lastRefreshTimeoutCount;

            lock (sync)
            {
                lastRefreshTime = lastScheduledRefreshDeliveryTime;
                lastRefreshDelivered = lastScheduledRefreshDelivered;
                lastRefreshException = lastScheduledRefreshDeliveryException;
                lastRefreshTimeoutCount = scheduledRefreshDeliveryTimeoutCount;
            }

            try
            {
                Result res = mgmtSvc.ProcessCollectionServiceHeartbeat(CollectionServiceConfiguration.CollectionServiceId, nextHeartbeat, lastRefreshDelivered, lastRefreshTime, lastRefreshException, lastRefreshTimeoutCount);
                if (res == Result.Failure)
                {
                    LOG.ErrorFormat("Error sending heartbeat to Management Service ({0})", res);
                }
            } catch(Exception e)
            {
                LOG.Error("Exception sending heartbeat - Management Service possibly down.", e);
            }
            lastHeartbeat = DateTime.Now;
        }

        #region IDisposable Members

        public void Dispose()
        {
            heartbeatTimer.Dispose();
        }

        public void TimerIntervalChanged(TimeSpan oldTime, TimeSpan newTime)
        {
            LOG.DebugFormat("Heartbeat interval changed old={0} new={1}", oldTime, newTime);
            Reconfigure();
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
