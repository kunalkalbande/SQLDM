//------------------------------------------------------------------------------
// <copyright file="SqlNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using BBS.TracerX;
    using Common;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;
    using Wintellect.PowerCollections;

    public class SqlNotificationProvider : INotificationProvider
    {
        private static readonly Logger LOG = Logger.GetLogger("ProgramNotificationProvider");
        private SqlNotificationProviderInfo info;
        private EventLog eventLog;
        private object sync = new object();

        public SqlNotificationProvider()
        {
        }

        public SqlNotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
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

                    if (value is SmtpNotificationProviderInfo)
                        this.info = value as SqlNotificationProviderInfo;
                    else
                    {
                        this.info = new SqlNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Sql notification provider {0}: {1}", operation, this.info.Name);
                }
            }
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                string description = String.Empty;
                string serverName = null;
                string sql = null;
                SqlDestination destination = null;
                AdhocQueryConfiguration config = null;
                try
                {
                    destination = (SqlDestination) context.Destination;
                    description = destination.Description;
                    serverName = destination.Server;
                    sql = destination.Sql.Trim();

                    IEvent baseEvent = context.SourceEvent;
                    MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;
                    if (baseEvent.AdditionalData is CustomCounterSnapshot)
                    {
                        // add in the metric description info to the additional 
                        // data if this is a custom counter snapshot
                        baseEvent.AdditionalData =
                            new Pair<CustomCounterSnapshot, MetricDescription?>(
                                (CustomCounterSnapshot) baseEvent.AdditionalData,
                                metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                    }

                    if (serverName.Contains("$"))
                    {
                        serverName =
                            NotificationMessageFormatter.FormatMessage(serverName, context.Refresh, baseEvent, false);
                        LOG.InfoFormat("Server name '{0}' translated to '{1}'", destination.Server, serverName);
                    }
                    if (sql.Contains("$"))
                    {
                        sql = NotificationMessageFormatter.FormatMessage(sql, context.Refresh, baseEvent, false, NotificationMessageFormatter.EscapeFormat.Sql);
                        LOG.InfoFormat("Sql '{0}' translated to '{1}'", destination.Sql, sql);
                    }

                    if (string.IsNullOrEmpty(serverName))
                    {
                        LogAndAddAlert(String.Format("Target server for sql script action '{0}' is invalid [{1}]", description, destination.Server), null);
                        return true;
                    }
                    if (string.IsNullOrEmpty(sql))
                    {
                        LogAndAddAlert(String.Format("Sql for sql script action '{0}' is invalid [{1}]", description, destination.Sql), null);
                        return true;
                    }

                    LOG.DebugFormat("Sql action - {0}='{1}' on {2}", description, sql, serverName);

                    // translate any variables in the server, job, step names
                    MonitoredSqlServerState state =
                        Management.ScheduledCollection.GetCachedMonitoredSqlServer(serverName,
                                                                                   StringComparison.
                                                                                       CurrentCultureIgnoreCase);
                    if (state == null)
                    {
                        LogAndAddAlert(String.Format("Target for sql script action '{0}' not found [{1}]", description, serverName), null);
                        return true;
                    }
                    int monitoredServerId = state.WrappedServer.Id;
                    config = new AdhocQueryConfiguration(
                        state.WrappedServer.Id,
                        sql,
                        false);
                }
                catch (Exception e)
                {
                    LogAndAddAlert(String.Format("Error initializing sql script action '{0}' on {1}", description, serverName), e);
                    return true;
                }

                ManagementService ms = new ManagementService();
                try
                {
                    AdhocQuerySnapshot result = ms.SendAdhocQuery(config);
                    if (result.CollectionFailed)
                        throw result.Error;
                    else
                    {
                        if (result.RowsAffected == null || result.RowsAffected < 0)
                            LOG.InfoFormat("{0}:{1} {2} rowsets returned. {3}", serverName, description,
                                           result.RowSetCount, sql);
                        else
                            LOG.InfoFormat("Sql executed on {0}:{1} {2} rows affected. {2}", serverName, description,
                                           result.RowsAffected, sql);
                    }
                }
                catch (Exception e)
                {
                    LogAndAddAlert(String.Format("Error executing sql script action '{0}' on {1}", description, serverName), e);
                }
                return true;
            }
        }


        private void LogAndAddAlert(string message, Exception e)
        {
            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                new MonitoredObjectName((string)null, (string)null),
                                MonitoredState.Warning,
                                message,
                                (e == null) ? message : message + "[" + e.Message + "]");

            LOG.Error(message, e);
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

    }
}
