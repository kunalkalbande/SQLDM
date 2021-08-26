//------------------------------------------------------------------------------
// <copyright file="PowerShellNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
//File added to support Alert Power Shell Provider
//SQLdm 10.1 Srishti Purohit
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
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using Wintellect.PowerCollections;

    public class PowerShellNotificationProvider : INotificationProvider
    {
        private static readonly Logger LOG = Logger.GetLogger("ProgramNotificationProvider");
        private PowerShellNotificationProviderInfo info;
        private EventLog eventLog;
        private object sync = new object();
        string exCmdletResult = "";

        public PowerShellNotificationProvider()
        {
        }

        public PowerShellNotificationProvider(NotificationProviderInfo info)
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
                        this.info = value as PowerShellNotificationProviderInfo;
                    else
                    {
                        this.info = new PowerShellNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Power shell notification provider {0}: {1}", operation, this.info.Name);
                }
            }
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                string description = String.Empty;
                //string serverName = null;
                string command = null;
                string formattedSingleCommand = string.Empty;
                PowerShellDestination destination = null;
                using (PowerShell ps = PowerShell.Create())
                {
                    try
                    {
                        destination = (PowerShellDestination)context.Destination;
                        description = destination.Description;
                        //serverName = destination.Server;
                        command = destination.Command.Trim();
                        IEvent baseEvent = context.SourceEvent;
                        MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;

                        if (baseEvent.AdditionalData is CustomCounterSnapshot)
                        {
                            // add in the metric description info to the additional 
                            // data if this is a custom counter snapshot
                            baseEvent.AdditionalData =
                                new Pair<CustomCounterSnapshot, MetricDescription?>(
                                    (CustomCounterSnapshot)baseEvent.AdditionalData,
                                    metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                        }

                        if (string.IsNullOrEmpty(command))
                        {
                            LogAndAddAlert(String.Format("Command for power shell action '{0}' is invalid [{1}]", description, destination.Command), null);
                            return true;
                        }

                        LOG.DebugFormat("Power shell command action - {0}='{1}' .", description, command);

                        formattedSingleCommand = command;
                        if (command.Contains("$"))
                        {
                            //TODO: The FormatMessage method would be refactored for a new NotificationMessageFormatter.EscapeFormat.PS enum value. 
                            formattedSingleCommand = NotificationMessageFormatter.FormatMessage(command, context.Refresh, baseEvent, false, NotificationMessageFormatter.EscapeFormat.Sql);
                            LOG.InfoFormat("Power shell command: [{0}]{2}Translated to: [{1}]", command, formattedSingleCommand, System.Environment.NewLine);
                        }

                        // Execute the resulting powershell script
                        CmdletResults(formattedSingleCommand, ps);

                        // Trace for errors during the execution
                        foreach (var psError in ps.Streams.Error)
                        {
                            exCmdletResult = "Error: " + ps.Streams.Error[0].Exception.Message;
                            throw ps.Streams.Error[0].Exception;
                        }

                        LOG.InfoFormat("Power shell alert script [{0}] executed successfully.", description);
                    }
                    catch (Exception e)
                    {
                        LogAndAddAlert(String.Format("Error excecuting power shell alert script [{0}] on [{1}]", description, formattedSingleCommand), e);
                        return true;
                    }

                    return true;
                }
            }
        }

        private Collection<PSObject> CmdletResults(string PSScriptText, PowerShell ps)

        {
            exCmdletResult = "";
            Collection<PSObject> PSOutput = null;
            ps.AddScript(PSScriptText);
            try
            {
                PSOutput = ps.Invoke();
            }
            catch (Exception ex) { exCmdletResult = "Error: " + ex.Message; }

            return PSOutput;
        }

        private void LogAndAddAlert(string message, Exception e)
        {
            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                new MonitoredObjectName((string)null, (string)null),
                                MonitoredState.Warning,
                                message,
                                (e == null) ? message : message + "[" + e.Message + "]");

            LOG.ErrorFormat("{0}{2}{1}", message, e, Environment.NewLine);
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

    }
}
