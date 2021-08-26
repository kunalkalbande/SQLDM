//------------------------------------------------------------------------------
// <copyright file="SCOMAlertNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Diagnostics;
    using Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Analysis;
    using System.Xml;
    /// <summary>
    /// Notification Provider for sending alert to SCOM either as Alert or as Event.
    /// </summary>
    public class SCOMAlertNotificationProvider : INotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SCOMAlertNotificationProvider");
        private EventLogNotificationProviderInfo info;
        private EventLog eventLog;

        private object sync = new object();

        public SCOMAlertNotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                try
                {
                    int alertId = context.Refresh.Id;
                    if (context.Refresh != null)
                    {
                        SCOMAlertDestination destination = context.Destination as SCOMAlertDestination;
                        ManagementService mgmtService = new ManagementService();

                        mgmtService.UpdateSCOMAlertEvent(context.SourceEvent.MetricID, true, context.Rule.Id);
                    }
                }
                catch(Exception ex)
                {
                    LOG.Debug("SCOMAlertNotificationProvider Send function: "+ ex.Message);
                }
                return true;
            }
        }
        private void LogAndAddAlert(string message, Exception e)
        {
            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                new MonitoredObjectName((string)null, (string)null),
                                e != null ? MonitoredState.Warning : MonitoredState.OK,
                                message,
                                (e == null) ? message : message + "[" + e.Message + "]");

            if (e == null)
                LOG.Info(message);
            else
                LOG.Error(message, e);
        }

        public NotificationProviderInfo NotificationProviderInfo
        {
            get
            {
                lock (sync)
                {
                    return this.info;
                }
            }

            set
            {
                lock (sync)
                {
                    string operation = this.info == null ? "created" : "updated";

                    if (value is EventLogNotificationProviderInfo)
                        this.info = value as EventLogNotificationProviderInfo;
                    else
                    {
                        this.info = new EventLogNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Send to SCOM as Alert provider {0}", operation);
                }
            }
        }


    }
}
