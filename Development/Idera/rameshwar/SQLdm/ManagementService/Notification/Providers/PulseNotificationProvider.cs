//------------------------------------------------------------------------------
// <copyright file="PulseNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using BBS.TracerX;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.ManagementService.Monitoring;
    using Idera.SQLdm.Common;
    using Idera.Newsfeed.Shared.DataModel;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.Newsfeed.Shared.Interface;
    using System.Reflection;
    using System.IO;
    using System.Xml;

    public class PulseNotificationProvider : INotificationProvider
    {
        private static readonly Logger LOG = Logger.GetLogger("PulseNotificationProvider");
        private PulseNotificationProviderInfo info;
        private EventLog eventLog;
        private object sync = new object();

        public PulseNotificationProvider()
        {
        }

        public PulseNotificationProvider(NotificationProviderInfo info)
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

                    this.info = value as PulseNotificationProviderInfo;
                    if (this.info == null)
                        this.info = new PulseNotificationProviderInfo(value);

                    LOG.DebugFormat("Newsfeed notification provider {0}: {1}", operation, this.info.Name);
                }
            }
        }

        public Application GetApplication()
        {
            Application app = null;

            try
            {
                app = WebClient.WebClient.Application;
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }

            if (app == null)
            {
                // try to refresh the notification provider registration
                if (info == null)
                    WebClient.WebClient.RefreshPulseRegistration(info);

                app = WebClient.WebClient.Application;
            }

            return app;
        }

        public bool Send(NotificationContext context)
        {
            using (LOG.InfoCall("Send"))
            {
                IEvent ievent = context.SourceEvent;

                Application app;

                try
                {
                    app = WebClient.WebClient.Application;
                    if (app == null)
                        throw new ApplicationException("Unable to find news feed application id.  Registration with the news service may be invalid or the news service may be down.");
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }

                PulseDestination destination = context.Destination as PulseDestination ?? new PulseDestination(context.Destination);
                IEvent baseEvent = context.SourceEvent;

                string correlationId = GetCorrelationId(app.Id, ievent);
                LOG.DebugFormat("CorrelationId: {0}", correlationId);

                MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;
                MetricDescription? metricDescription = metricDefinitions.GetMetricDescription(ievent.MetricID);

                if (baseEvent.AdditionalData is CustomCounterSnapshot)
                {
                    // add in the metric description info to the additional 
                    // data if this is a custom counter snapshot
                    baseEvent.AdditionalData =
                        new Pair<CustomCounterSnapshot, MetricDescription?>(
                            (CustomCounterSnapshot)baseEvent.AdditionalData,
                            metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                }

                string body = NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false, NotificationMessageFormatter.EscapeFormat.News);

                //MessageMap messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);
                //Message? message = null;
                //if (messageMap != null)
                //    message = messageMap.GetMessageForValue(baseEvent.Value);

                //if (string.IsNullOrEmpty(body) && message != null)
                //    body = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                //if (string.IsNullOrEmpty(body))
                //{
                //    body = destination.Body;
                //}

                //if (String.IsNullOrEmpty(body))
                //{
                //    if (message != null)
                //    {
                //        body = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
                //    }
                //    if (string.IsNullOrEmpty(body))
                //    {
                //        body = destination.Subject;
                //    }
                //}

          
                object value = null;
                try
                {
                    if (baseEvent.Value != null)
                        value = Convert.ToDouble(value);
                }
                catch (Exception)
                {
                    /* */
                }

                StringBuilder vartext = new StringBuilder();
                StringWriter sw = new StringWriter();
                XmlTextWriter varwriter = new XmlTextWriter(new StringWriter(vartext));

                varwriter.WriteStartDocument();
                varwriter.WriteStartElement("Vars");

                AppendVar(varwriter, "$Metric", baseEvent.MetricID);

                varwriter.WriteEndElement();
                varwriter.WriteEndDocument();

                string metricName = metricDescription.HasValue
                    ? metricDescription.Value.Name
                    : "N/A";

                Importance newImportance = GetImportance(ievent.MonitoredState);

                MonitoredState prevState = ievent.StateChanged ? ievent.PreviousState : ievent.MonitoredState;
                Importance prevImportance = GetImportance(prevState);

                int senderId = 1;
                string sender = ievent.MonitoredObject.ServerName;
                switch (ievent.MetricID)
                {
                    case (int)Metric.SQLdmCollectionServiceStatus:
                        sender = null;  // sender is application
                        senderId = 0;   // this is from the application
                        break;
                }
                int postId = WebClient.WebClient.PostStatus(sender, senderId, correlationId, prevImportance, newImportance, metricName, body, vartext.ToString());
                LOG.VerboseFormat("Newsfeed notification: {0}", body);
                return true;
            }
        }

        private static Importance GetImportance(MonitoredState state)
        {
            switch (state)
            {
                case MonitoredState.Critical:
                    return Importance.Critical;
                case MonitoredState.Warning:
                    return Importance.Warning;
                case MonitoredState.Informational:
                    return Importance.Info;
                case MonitoredState.OK:
                    return Importance.Good;
                default:
                    return Importance.Unknown;
            }
        }

        public static void Test(PulseNotificationProviderInfo context)
        {
            WebClient.WebClient.ValidatePulseConnection(context);
        }

        private static void AppendVar(XmlTextWriter writer, string varName, object value)
        {
            if (value == null)
                value = String.Empty;

            writer.WriteStartElement("Var");
            // name attribute
            writer.WriteStartAttribute("name");
            writer.WriteString(varName);
            writer.WriteEndAttribute();
            // type attribute
            writer.WriteStartAttribute("type");
            writer.WriteString(value.GetType().Name);
            writer.WriteEndAttribute();
            // text value
            writer.WriteString(value.ToString());

            writer.WriteEndElement();
        }

        private static string GetCorrelationId(int applicationId, IEvent ievent)
        {
            DateTime startTime;

            if (ievent is StateDeviationEvent)
            {
                StateDeviationEvent sde = (StateDeviationEvent)ievent;
                startTime = sde.OccuranceTime;
                LOG.DebugFormat("Deviation @{0} mon={1} metric={3} state={2}", sde.OccuranceTime, sde.MonitoredObject, sde.MonitoredState,sde.MetricID);

            }
            else
            if (ievent is StateDeviationClearEvent)
            {
                StateDeviationClearEvent sdce = (StateDeviationClearEvent)ievent;
                StateDeviationEvent sde = sdce.DeviationEvent;
                startTime = sde.OccuranceTime;
                LOG.DebugFormat("Cleared @{0} Start @{3} mon={1} metric={4} state={2}", sdce.OccuranceTime, sdce.MonitoredObject, sdce.MonitoredState, sde.OccuranceTime, sdce.MetricID);
            }
            else
            if (ievent is StateDeviationUpdateEvent)
            {
                StateDeviationUpdateEvent sdue = (StateDeviationUpdateEvent)ievent;
                StateDeviationEvent sde = sdue.DeviationEvent;
                startTime = sde.OccuranceTime;
                LOG.DebugFormat("Updated @{0} Start @{3} mon={1} metric={4} state={2}", sdue.OccuranceTime, sdue.MonitoredObject, sdue.MonitoredState, sde.OccuranceTime, sdue.MetricID);
            }
            else
            {
                LOG.Debug("ievent is unknown: ", ievent.GetType().FullName);
                startTime = ievent.OccuranceTime;
            }

            return String.Format("{0}.{1}.{2}.{3}", startTime.Ticks,  applicationId, ievent.MetricID, ievent.MonitoredObject.GetHashCode());
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
