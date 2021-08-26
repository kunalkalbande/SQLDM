//------------------------------------------------------------------------------
// <copyright file="EventLogNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Diagnostics;
    using Common;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Notification Provider for writing events to the Event Log.
    /// </summary>
    public class EventLogNotificationProvider : INotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("EventLogNotificationProvider");
        private EventLogNotificationProviderInfo info;
        private EventLog eventLog;
        
        private object sync = new object();
        
        public EventLogNotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public bool Send(NotificationContext context)
        {
            MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;
            try
            {
                IEvent baseEvent = context.SourceEvent;

                if (baseEvent.AdditionalData is CustomCounterSnapshot)
                {
                    // add in the metric description info to the additional 
                    // data if this is a custom counter snapshot
                    baseEvent.AdditionalData =
                        new Pair<CustomCounterSnapshot, MetricDescription?>(
                            (CustomCounterSnapshot)baseEvent.AdditionalData,
                            metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                }


                MetricDefinition definition = metricDefinitions.GetMetricDefinition(baseEvent.MetricID);
                if (definition == null || !definition.ProcessNotifications)
                {
                    return true;
                }

                MessageMap messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);
                Message? message = messageMap.GetMessageForValue(baseEvent.Value);
                if (message == null)
                {
                    object data = baseEvent.AdditionalData;
                    if (data is Pair<CustomCounterSnapshot, MetricDescription?>)
                    {
                        Pair<CustomCounterSnapshot, MetricDescription?> pcm = (Pair<CustomCounterSnapshot, MetricDescription?>)data;
                        if (pcm.First.CollectionFailed)
                        {
                            message = messageMap.GetMessage(-1);
                            if (message == null)
                                message = messageMap.GetMessage(0);
                        }
                        else
                            message = messageMap.GetMessage(0);
                    }
                    else
                        message = messageMap.GetMessage(0);
                }
                if (message.HasValue)
                {
                    string body = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                    Idera.SQLdm.Common.Messages.Status status = (Idera.SQLdm.Common.Messages.Status)Enum.ToObject(typeof(Idera.SQLdm.Common.Messages.Status), (int)message.Value.EventId);
                    
                    int eventId = (int)status;
                    uint eventType = 0;

                    EventInstance instance = new EventInstance((long)eventId, (int)definition.EventCategory);
                    switch (baseEvent.MonitoredState)
                    {
                        case MonitoredState.OK:
                        case MonitoredState.Informational:
                            eventType = (int)EventLogEntryType.Information;
                            break;
                        case MonitoredState.Warning:
                            eventType = (int)EventLogEntryType.Warning;
                            break;
                        case MonitoredState.Critical:
                            eventType = (int)EventLogEntryType.Error;
                            break;
                    }
                    if (eventLog != null)
                    {
                        MessageDll.WriteEvent(eventLog.Source, (uint)eventType, 1, (uint)eventId, body);
                    }
                }
            }
            catch (Exception e)
            {
                // attach the exception and return false so that this gets logged but with no retry
                context.LastSendException = e;
                return false;
            }

            return true;
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
                    LOG.DebugFormat("Windows Event Log notification provider {0}", operation);
                }
            }
        }


    }
}
