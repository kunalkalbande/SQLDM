//------------------------------------------------------------------------------
// <copyright file="JobNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using BBS.TracerX;
    using Common;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;
    using Wintellect.PowerCollections;

    public class JobNotificationProvider : INotificationProvider
    {
        private static readonly Logger LOG = Logger.GetLogger("JobNotificationProvider");
        private JobNotificationProviderInfo info;
        private EventLog eventLog;
        private object sync = new object();


        public JobNotificationProvider()
        {
        }

        public JobNotificationProvider(NotificationProviderInfo info)
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

                    if (value is JobNotificationProviderInfo)
                        this.info = value as JobNotificationProviderInfo;
                    else
                    {
                        this.info = new JobNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Job notification provider {0}: {1}", operation, this.info.Name);
                }
            }
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                string serverName = null;
                string jobName = null;
                string stepName = null;
                JobDestination destination = null;
                JobControlConfiguration config = null;
                try
                {
                    destination = (JobDestination) context.Destination;
                    serverName = destination.Server;
                    jobName = destination.Job;
                    stepName = destination.Step;

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
                    if (jobName.Contains("$"))
                    {
                        jobName = NotificationMessageFormatter.FormatMessage(jobName, context.Refresh, baseEvent, false);
                        LOG.InfoFormat("Job name '{0}' translated to '{1}'", destination.Job, jobName);
                    }
                    if (stepName.Contains("$"))
                    {
                        stepName =
                            NotificationMessageFormatter.FormatMessage(stepName, context.Refresh, baseEvent, false);
                        LOG.InfoFormat("Job step {0}' translated to '{1}'", destination.Step, stepName);
                    }

                    if (string.IsNullOrEmpty(serverName))
                    {
                        LogAndAddAlert(String.Format("Target server for job action is invalid '{0}'", destination.Server), null);
                        return true;
                    }
                    if (string.IsNullOrEmpty(jobName))
                    {
                        LogAndAddAlert(String.Format("Job name for job action is invalid '{0}'", destination.Job), null);
                        return true;
                    }

                    LOG.DebugFormat("Job action - start {0} at step {1} on {2}", jobName, stepName, serverName);

                    // translate any variables in the server, job, step names
                    MonitoredSqlServerState state =
                        Management.ScheduledCollection.GetCachedMonitoredSqlServer(serverName,
                                                                                   StringComparison.
                                                                                       CurrentCultureIgnoreCase);
                    if (state == null)
                    {
                        LogAndAddAlert(String.Format("Target server for job action not found '{0}'", serverName), null);
                        return true;
                    }

                    int monitoredServerId = state.WrappedServer.Id;
                    config = new JobControlConfiguration(
                        state.WrappedServer.Id,
                        jobName,
                        stepName,
                        Idera.SQLdm.Common.Snapshots.JobControlAction.Start);
                }
                catch (Exception e)
                {
                    LogAndAddAlert(String.Format("Error initializing job action '{0}' on {1}", jobName, serverName), e);
                    return true;
                }

                ManagementService ms = new ManagementService();
                try
                {
                    AuditingEngine.SetContextData(
                        string.IsNullOrEmpty(ManagementServiceConfiguration.RepositoryUser) ? AuditingEngine.GetWorkstationUser() : ManagementServiceConfiguration.RepositoryUser);
                    Snapshot result = ms.SendJobControl(config);
                    if (result.CollectionFailed)
                        throw result.Error;
                    else
                        LOG.InfoFormat("Job {0} started on {1}", serverName, jobName);
                }
                catch (Exception e)
                {
                    if (String.IsNullOrEmpty(stepName))
                        LogAndAddAlert(String.Format("Error starting job action '{0}' on {1}", jobName, serverName), e);
                    else
                        LogAndAddAlert(String.Format("Error starting job action '{0}' at '{1}' on {2}", jobName, stepName, serverName), e);
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
