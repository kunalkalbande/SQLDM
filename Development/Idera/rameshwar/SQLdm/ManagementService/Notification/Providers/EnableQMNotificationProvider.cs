//------------------------------------------------------------------------------
// <copyright file="EnableQMNotificationProvider.cs" company="Idera, Inc.">
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

    /// <summary>
    /// Notification Provider for enabline the query monitor.
    /// </summary>
    public class EnableQMNotificationProvider : INotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("EnableQMNotificationProvider");
        private EventLogNotificationProviderInfo info;
        private EventLog eventLog;
        
        private object sync = new object();

        public EnableQMNotificationProvider(NotificationProviderInfo info)
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
                //MonitoredSqlServer collectionServer = context.Refresh.MonitoredServer;
                if (context.Refresh != null)
                {
                    EnableQMDestination destination = context.Destination as EnableQMDestination;
                    MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(context.Refresh.Id);

                    bool alreadyEnabled = false;
                    if (state.WrappedServer.QueryMonitorConfiguration.Enabled)
                    {
                        if (state.WrappedServer.QueryMonitorConfiguration.StopTimeUTC.HasValue)
                        {
                            if (destination != null && destination.DurationInMinutes > 0)
                            {
                                DateTime stop = state.WrappedServer.QueryMonitorConfiguration.StopTimeUTC.Value;
                                stop -= TimeSpan.FromSeconds(59);

                                if (stop > DateTime.UtcNow)
                                {
                                    if ((DateTime.UtcNow + TimeSpan.FromMinutes(destination.DurationInMinutes - 2)) <= stop)
                                    {
                                        alreadyEnabled = true;
                                    }
                                }
                            }
                        } else
                            alreadyEnabled = true;
                    }

                    if (alreadyEnabled)
                    {
                        LOG.InfoFormat("Query monitor already enabled for '{0}'", context.Refresh.ServerName);
                        return true;
                    }

                    try
                    {
                        MonitoredSqlServerConfiguration mssc = state.WrappedServer.GetConfiguration();
                        QueryMonitorConfiguration qmc = mssc.QueryMonitorConfiguration;

                        int duration = destination == null ? 15 : destination.DurationInMinutes;
                        if (duration < 1)
                        {
                            qmc.IsAlertResponseQueryTrace = false;
                        }
                        else
                        {
                            qmc.StopTimeUTC = DateTime.UtcNow + TimeSpan.FromMinutes(duration);
                            qmc.IsAlertResponseQueryTrace = true;
                        }
                        
                        qmc.Enabled = true;
                        //SQLDM-29208. Update the monitored server configuration.
                        mssc.QueryMonitorConfiguration = qmc;
                        ManagementService.InternalUpdateMonitoredSqlServer(state.WrappedServer.Id, mssc, false);
                        
                        if (duration < 1)
                            LogAndAddAlert(String.Format("Query monitor trace started for server '{0}'", context.Refresh.ServerName, duration), null);
                        else
                            LogAndAddAlert(String.Format("Query monitor trace started for server '{0}' for {1} minutes.", context.Refresh.ServerName, duration), null);

                    } catch (Exception e)
                    {
                        LogAndAddAlert(String.Format("Error starting the query monitor for server '{0}':", context.Refresh.ServerName), e);
                    }
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
                    LOG.DebugFormat("Enable query monitor notification provider {0}", operation);
                }
            }
        }


    }
}
