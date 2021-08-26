//------------------------------------------------------------------------------
// <copyright file="ProgramNotificationProvider.cs" company="Idera, Inc.">
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
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;
    using Wintellect.PowerCollections;

    public class ProgramNotificationProvider : INotificationProvider
    {
        private static readonly Logger LOG = Logger.GetLogger("ProgramNotificationProvider");
        private Process process;
        private ProgramNotificationProviderInfo info;
        private EventLog eventLog;
        private object sync = new object();

        public ProgramNotificationProvider()
        {
        }

        public ProgramNotificationProvider(NotificationProviderInfo info)
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

                    if (value is ProgramNotificationProviderInfo)
                        this.info = value as ProgramNotificationProviderInfo;
                    else
                    {
                        this.info = new ProgramNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Program notification provider {0}: {1}", operation, this.info.Name);
                }
            }
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                bool testMode = false;
                string command = null;
                string startDirectory = null;
                string path = String.Empty;
                string arguments = String.Empty;

                string description = String.Empty;
                ProgramDestination destination = null;
                ProcessStartInfo startInfo = null;
                try
                {
                    destination = (ProgramDestination) context.Destination;
                    description = destination.Description;
                    command = destination.Command.Trim();
                    startDirectory = destination.StartIn.Trim();

                    IEvent baseEvent = context.SourceEvent;
                    if (baseEvent != null)
                    {
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

                        if (command.Contains("$"))
                        {
                            command = NotificationMessageFormatter.FormatMessage(command, context.Refresh, baseEvent, false);
                            LOG.InfoFormat("Server name '{0}' translated to '{1}'", destination.Command, command);
                        }
                        if (startDirectory.Contains("$"))
                        {
                            startDirectory = NotificationMessageFormatter.FormatMessage(startDirectory, context.Refresh, baseEvent, false);
                            LOG.InfoFormat("Start directory '{0}' translated to '{1}'", destination.StartIn,
                                           startDirectory);
                        }
                    }
                    else
                        testMode = true;

                    if (string.IsNullOrEmpty(command))
                    {
                        if (testMode)
                            throw new ApplicationException(String.Format("Cannot execute command: '{0}'", destination.Command));

                        LogAndAddAlert(String.Format("Unable to translate command into something useful: '{0}'", destination.Command), null);
                        return true;
                    }
                    LOG.DebugFormat("Program action - start {0} in {1}", command, startDirectory);

                    path = command;
                    arguments = String.Empty;

                    ProgramDestination.ParseCommandLine(command, out path, out arguments);

                    startInfo = new ProcessStartInfo();
                    // setting WindowStyle to Hidden will cause GUI programs to start with a hidden window.  
                    // If the program does not do something to make the window show the process will run with no
                    // way for the user to interact with it.  Leaving it as Normal will cause little popup command windows
                    // to appear and go away as command-line programs execute.
                    // startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.ErrorDialog = false;
                    startInfo.Arguments = arguments;
                    startInfo.FileName = path.Replace("\"", "");
                    if (!String.IsNullOrEmpty(startDirectory))
                        startInfo.WorkingDirectory = startDirectory.Replace("\"","");
                }
                catch (Exception e)
                {
                    if (testMode)
                    {
                        throw new ApplicationException(
                            String.Format("Error configuring program action '{0}' path: {1} args: {2}",
                                          description, path, arguments), e);
                    }

                    LogAndAddAlert(
                        String.Format("Error configuring program action '{0}' path: {1} args: {2}",
                            description, path, arguments),
                        e);
                    return true;
                }

                try
                {
                    process = Process.Start(startInfo);
                    LOG.InfoFormat("Program action '{0}' started. pid={1} ({2})", description, process.Id, command);
                }
                catch (Exception e)
                {
                    if (testMode)
                    {
                        throw new ApplicationException(
                            String.Format("Error starting program action '{0}' path: {1} args: {2}",
                                          description, path, arguments), e);
                    }

                    LogAndAddAlert(
                        String.Format("Error starting program action '{0}' path: {1} args: {2}",
                                      description, path, arguments),
                        e);
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
