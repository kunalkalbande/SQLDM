//------------------------------------------------------------------------------
// <copyright file="EnableQWaitsNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//File added in SQLdm10.1 (Srishti Purohit) 
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
    /// Notification Provider for enabline the query waits.
    /// </summary>
    public class EnableQWaitsNotificationProvider : INotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("EnableQWaitsNotificationProvider");
        private EventLogNotificationProviderInfo info;
        private EventLog eventLog;

        private object sync = new object();

        public EnableQWaitsNotificationProvider(NotificationProviderInfo info)
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
                //SQLdm 10.1 (Srishti Purohit) -- Fixing defect SQLDM-26103 -- checking if server version is greater or equal 2005
                if (context.Refresh != null && context.Refresh.ProductVersion != null && context.Refresh.ProductVersion.Major >= (int)Common.SQLVersionMajor.SQL2005)
                {
                    EnableQWaitsDestination destination = context.Destination as EnableQWaitsDestination;
                    MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(context.Refresh.Id);

                    bool alreadyEnabled = false;
                    if (state.WrappedServer.ActiveWaitsConfiguration.Enabled)
                    {
                        state.WrappedServer.ActiveWaitsConfiguration.UpdateStartTime(DateTime.Now, DateTime.UtcNow);
                        if (state.WrappedServer.ActiveWaitsConfiguration.StopTimeUTC.HasValue)
                        {
                            if (destination != null && destination.DurationInMinutes > 0)
                            {
                                DateTime stop = state.WrappedServer.ActiveWaitsConfiguration.StopTimeUTC.Value;
                                if (stop >= DateTime.MinValue + TimeSpan.FromSeconds(59))
                                    stop -= TimeSpan.FromSeconds(59);

                                if (stop > DateTime.UtcNow)
                                {
                                    if ((DateTime.UtcNow + TimeSpan.FromMinutes(destination.DurationInMinutes - 2)) <= stop)
                                    {
                                        alreadyEnabled = true;
                                    }
                                }
                            }
                        }
                        else
                            alreadyEnabled = true;
                    }

                    if (alreadyEnabled)
                    {
                        LOG.InfoFormat("Query waits already enabled for '{0}'", context.Refresh.ServerName);
                        return true;
                    }

                    try
                    {
                        MonitoredSqlServerConfiguration mssc = state.WrappedServer.GetConfiguration();
                        ActiveWaitsConfiguration qwc = mssc.ActiveWaitsConfiguration;

                        int duration = destination == null ? 0 : destination.DurationInMinutes;
                        qwc.StartTimeRelative = DateTime.Now;
                        if (duration == 0)
                            qwc.RunTime = null;
                        else
                            qwc.RunTime = TimeSpan.FromMinutes(duration);

                        qwc.Enabled = true;
                        //SQLDM-29208.
                        mssc.ActiveWaitsConfiguration = qwc;


                        ManagementService.InternalUpdateMonitoredSqlServer(state.WrappedServer.Id, mssc, false);

                        if (duration < 1)
                            LogAndAddAlert(String.Format("Query waits trace started for server '{0}'", context.Refresh.ServerName, duration), null);
                        else
                            LogAndAddAlert(String.Format("Query waits trace started for server '{0}' for {1} minutes.", context.Refresh.ServerName, duration), null);

                    }
                    catch (Exception e)
                    {
                        LogAndAddAlert(String.Format("Error starting the query waits for server '{0}':", context.Refresh.ServerName), e);
                    }
                }
                else
                {
                    if(context.Refresh.ProductVersion == null)
                    {
                        LOG.Error(" For server {0} Product version is null.", context.Refresh.ServerName);
                    }
                    else
                    {
                        LOG.Error(" For server {0} Product version is less than 2005 {1}.", context.Refresh.ServerName, context.Refresh.ProductVersion);
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
                    LOG.DebugFormat("Enable query waits notification provider {0}", operation);
                }
            }
        }


    }
}
