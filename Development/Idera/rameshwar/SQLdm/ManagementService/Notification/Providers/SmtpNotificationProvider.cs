//------------------------------------------------------------------------------
// <copyright file="SmtpNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Notification;

namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using Idera.SQLdm.Common.Events;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;
    using System.Collections.Generic;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.ManagementService.Monitoring;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    public class SmtpNotificationProvider : INotificationProvider, IBulkNotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SmtpNotificationProvider");

        private SmtpClient client;
        private SmtpNotificationProviderInfo info;
        private object sync = new object();
        private EventLog eventLog;

        /// <summary>
        /// Maximum Length for subject in mail message
        /// </summary>
        private const int MAXSUBJECTLENGTH = 160;

        public SmtpNotificationProvider()
        {
        }

        public SmtpNotificationProvider(NotificationProviderInfo info)
        {
            NotificationProviderInfo = info;
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public SmtpClient GetClient()
        {
            if (client != null)
            {
                if (!client.Host.Equals(info.SmtpServer) ||
                    client.Port != info.SmtpServerPort || 
                    info.RequiresAuthentication != (client.Credentials != null))
                {
                    client = null;
                } else
                {
                    if (info.RequiresAuthentication)
                    {
                        NetworkCredential creds = (NetworkCredential)client.Credentials;
                        if (!creds.UserName.Equals(info.UserName) ||
                            !creds.Password.Equals(info.Password) || 
                            client.Timeout != info.LoginTimeout * 1000)
                            client = null;
                    }   
                }
            }

            if (client == null)
            {
                client = new SmtpClient();
                client.Host = info.SmtpServer;
                client.EnableSsl = info.RequiresSSL;
                if (info.SmtpServerPort > 0)
                    client.Port = info.SmtpServerPort;
                if (info.RequiresAuthentication)
                {
                    client.Timeout = info.LoginTimeout * 1000;
                    client.Credentials = new NetworkCredential(info.UserName, info.Password);
                }
            }
            return client;
        }

        public void ParseAddressesInto(string addresses, MailAddressCollection addressCollection)
        {
            StringBuilder builder = new StringBuilder();
            string[] chunks = addresses.Split(new char[] { '@' });
            if (chunks.Length > 0)
                builder.Append(chunks[0].Trim());

            for (int i = 1; i < chunks.Length; i++)
            {
                try
                {
                    builder.Append("@");
                    int j = chunks[i].IndexOfAny(new char[] {',', ';'});
                    if (j == -1)
                    {
                        if (i + 1 < chunks.Length)
                            throw new InvalidDataException();
                        builder.Append(chunks[i]);
                        addressCollection.Add(new MailAddress(builder.ToString()));
                        break;
                    }
                    else
                    {
                        builder.Append(chunks[i].Substring(0, j));
                        addressCollection.Add(new MailAddress(builder.ToString()));
                        builder.Length = 0;
                        builder.Append(chunks[i].Substring(j + 1));
                    }
                }
                catch (Exception)
                {
                    LogSmtpError(String.Format("Error parsing SMTP To address: {0}", addresses));
                }
            }
        }

        private void LogSmtpError(string message)
        {
            LOG.ErrorFormat(message);

            if (eventLog != null)
            {
                try
                {
                    long eventId  = (long) Idera.SQLdm.Common.Messages.Status.ErrorSmtpProviderError;
                    int category = (int) Idera.SQLdm.Common.Messages.Category.General;

                    EventInstance instance = new EventInstance(eventId, category);
                    instance.EntryType = EventLogEntryType.Error;

                    eventLog.WriteEvent(instance, "SMTP Notification Provider: " + message);    
                } catch (Exception e)
                {
                    LOG.Error("Error writing to event log: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Sends the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public void SendMessage(SmtpDestination destination, string subject, string body, bool IsBodyHTML = true)
        {
            bool exceptionLogged = false;
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(destination.From);
                ParseAddressesInto(destination.To, message.To);
                // SQLDM-28044 Format Mail Subject to be in line with MailClient
                message.Subject = FormatMailSubject(subject);
                //DE46107 defect fix for html text not taking \n as line change
                //10.0 SQLdm Srishti Purohit
                body = body.Replace("\n\n", "<br/><br/>");
                message.Body = body;
                message.IsBodyHtml = IsBodyHTML;
                LOG.DebugFormat("Smtp notification - sending to: {0}", destination.To);
                LOG.DebugFormat("Smtp notification - subject: {0}", subject);
                SmtpClient client = GetClient();
                client.Send(message);
                LOG.DebugFormat("Smtp notification - server: {0}", info.SmtpServer);
            }

            catch (SmtpFailedRecipientsException sfre)
            {
                LOG.Error("Smtp notification - send failed", sfre);
                foreach (SmtpFailedRecipientException inner in sfre.InnerExceptions)
                {
                    LOG.Debug(inner.Message);
                }
                exceptionLogged = true;
                throw;
            }
            catch (Exception e)
            {
                if (!exceptionLogged)
                    LOG.Error("Smtp notification - send failed", e);
                throw;
            }
        }

        /// <summary>
        /// Format Mail Subject - Replaces all CR and LF and 
        /// also check for Maximum Length <seealso cref="MAXSUBJECTLENGTH"/>
        /// </summary>
        /// <param name="subject">Input Subject</param>
        /// <returns>Valid Subject</returns>
        /// <remarks>
        /// SQLDM-28044 Format Mail Subject to be in line with MailClient
        /// </remarks>
        private static string FormatMailSubject(string subject)
        {
            LOG.DebugFormat("Smtp notification FormatMailSubject- Input subject : {0}", subject);
            // Remove presence of Carriage Returns and Line Feeds from the subject
            if (!string.IsNullOrEmpty(subject))
            {
                // Remove CR and LF
                subject = subject.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                // Restricting subject length
                subject = subject.Substring(0, Math.Min(subject.Length, MAXSUBJECTLENGTH));
                LOG.DebugFormat("Smtp notification FormatMailSubject- Formatted subject to: {0}", subject);
            }

            return subject;
        }

        public bool Send(NotificationContext context)
        {
            // Construct SMTP items - destination, subject and body.
            SmtpDestination destination = null;
            MessageMap messageMap = null;
            string subject = null;
            string body = null;
            try
            {
                destination = (SmtpDestination)context.Destination;
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
                                (CustomCounterSnapshot) baseEvent.AdditionalData,
                                metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                    }

                    messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                    subject =
                        NotificationMessageFormatter.FormatMessage(destination.Subject, context.Refresh, baseEvent,
                                                                   false);
                    body =
                        NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false);
                }

                if (string.IsNullOrEmpty(subject) && messageMap != null)
                    subject = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
                if (string.IsNullOrEmpty(subject))
                    subject = destination.Subject;

                if (string.IsNullOrEmpty(body) && messageMap != null)
                    body = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                if (string.IsNullOrEmpty(body))
                    body = destination.Body;

                LOG.DebugFormat("Smtp notification - sending to: {0}", destination.To);
                LOG.DebugFormat("Smtp notification - subject: {0}", subject);
                LOG.DebugFormat("Smtp notification - server: {0}", info.SmtpServer);
            }
            catch (Exception e) // Handle any exception raised during construction of SMTP items
            {
                context.LastSendException = e;
                LOG.Error("Smtp notification - construct message destination and body failed", e);
                return false;
            }

            // Send SMTP message
            try
            {
                SmtpClient client = GetClient();

                MailMessage message = new MailMessage();
                message.From = new MailAddress(destination.From);
                ParseAddressesInto(destination.To, message.To);
                // SQLDM-28044 Format Mail Subject to be in line with MailClient
                message.Subject = FormatMailSubject(subject);
                message.Body = body;

                client.Send(message);                    
                LOG.DebugFormat("Smtp notification sent to {0}", info.SmtpServer);
            }
            catch (SmtpException se) // If its SMTP exception, see if retry is needed
            {
                context.LastSendException = se;
                if (HandleFailedSend(se)) // throwing this to the caller causes a retry
                    throw;
                return false;
            }
            catch (Exception e) // Any other exception log and move on
            {
                context.LastSendException = e;
                LOG.Error("Smtp notification - send failed", e);
                return false;
            }

            return true;
        }

        //START SQLdm 10.0 (Swati Gogia)-- added code to concatenate body of different events for multi-metric alert to send a consolidated mail
        public int Send(NotificationContext[] notifications)
        {
             // Construct SMTP items - destination, subject and body.
            
            string finalEmailBody = string.Empty;
            string individualSubject = string.Empty;
            string bodyForEvent = string.Empty;
            const string subjectMultiMetric = "Multi-Metric Alert";//
            List<NotificationContext> allContexts = new List<NotificationContext>(notifications);
            List<NotificationContext> combinedNotificationContextList = null;
            List<NotificationContext> atomicNotificationContextList = null;
            SmtpDestination destination = new SmtpDestination();
            IEvent baseEvent = null;
            MessageMap messageMap = null;
            MetricDefinitions metricDefinitions = null;
            Guid ruleId = Guid.NewGuid(); //generating a dummy till a new one gets assigned
            int numberOfProcessedContexts = 0;
            try
            {
                metricDefinitions = SharedMetricDefinitions.MetricDefinitions;
                //searching all the context that need to be combined
                combinedNotificationContextList = allContexts.FindAll(x => x.Rule.IsMetricsWithAndChecked == true);
                //sorting on the rule id
                combinedNotificationContextList.Sort(delegate(NotificationContext n1, NotificationContext n2)
                {
                    return n1.Rule.Id.CompareTo(n2.Rule.Id);
                });

                atomicNotificationContextList = allContexts.FindAll(x => x.Rule.IsMetricsWithAndChecked == false);

                

                if (combinedNotificationContextList.Count > 0)
                    {
                        ruleId = combinedNotificationContextList[0].Rule.Id;
                        foreach (NotificationContext context in combinedNotificationContextList)
                        {
                        numberOfProcessedContexts++;
                            if (context.Rule.Id.Equals(ruleId))
                            {
                            bodyForEvent = string.Empty;
                            individualSubject = string.Empty;
                            destination = (SmtpDestination)context.Destination;
                            baseEvent = context.SourceEvent;
                            messageMap = null;
                                if (baseEvent != null)
                                {
                                    if (baseEvent.AdditionalData is CustomCounterSnapshot)
                                    {
                                        // add in the metric description info to the additional 
                                        // data if this is a custom counter snapshot
                                        baseEvent.AdditionalData =
                                            new Pair<CustomCounterSnapshot, MetricDescription?>(
                                                (CustomCounterSnapshot)baseEvent.AdditionalData,
                                                metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                                    }

                                    messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                                    individualSubject =
                                     NotificationMessageFormatter.FormatMessage(destination.Subject, context.Refresh, baseEvent,
                                                                   false);
                                    if (string.IsNullOrEmpty(individualSubject) && messageMap != null)
                                        individualSubject = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
                                    if (string.IsNullOrEmpty(individualSubject))
                                        individualSubject = destination.Subject;

                                bodyForEvent = NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false);

                                if (string.IsNullOrEmpty(bodyForEvent) && messageMap != null)
                                    bodyForEvent = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                                if (string.IsNullOrEmpty(bodyForEvent))
                                    bodyForEvent = destination.Body;

                                finalEmailBody = finalEmailBody + "<br/><br/>" + "<b>" + individualSubject + "</b>" + "<br/><br/>" + bodyForEvent;
                                }
                            }
                            else
                            {
                                try
                                {
                                // Send SMTP message
                                SendMessage((SmtpDestination)combinedNotificationContextList[0].Destination, subjectMultiMetric, finalEmailBody);
                            }
                            catch (SmtpException se) // If its SMTP exception, see if retry is needed
                            {
                                context.LastSendException = se;
                                
                                if (HandleFailedSend(se)) // throwing this to the caller causes a retry
                                    throw;
                                LOG.Error("Smtp notification - send failed", se);
                                //return 0;
                            }
                            catch (Exception e) // Any other exception log and move on
                            {
                                context.LastSendException = e;
                                LOG.Error("Smtp notification - send failed", e);

                            }

                            
                            finalEmailBody = null;
                                    individualSubject = null;
                                    ruleId = context.Rule.Id;
                            bodyForEvent = null;
                            destination = (SmtpDestination)context.Destination;
                            baseEvent = context.SourceEvent;
                            messageMap = null;
                                    if (baseEvent != null)
                                    {
                                        if (baseEvent.AdditionalData is CustomCounterSnapshot)
                                        {
                                            // add in the metric description info to the additional 
                                            // data if this is a custom counter snapshot
                                            baseEvent.AdditionalData =
                                                new Pair<CustomCounterSnapshot, MetricDescription?>(
                                                    (CustomCounterSnapshot)baseEvent.AdditionalData,
                                                    metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                                        }

                                        messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);

                                        individualSubject =
                                     NotificationMessageFormatter.FormatMessage(destination.Subject, context.Refresh, baseEvent,
                                                                   false);
                                        if (string.IsNullOrEmpty(individualSubject) && messageMap != null)
                                            individualSubject = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
                                        if (string.IsNullOrEmpty(individualSubject))
                                            individualSubject = destination.Subject;

                                bodyForEvent = NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false);

                                if (string.IsNullOrEmpty(bodyForEvent) && messageMap != null)
                                    bodyForEvent = messageMap.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                                if (string.IsNullOrEmpty(bodyForEvent))
                                    bodyForEvent = destination.Body;

                                finalEmailBody = finalEmailBody + "<br/><br/>" + "<b>" + individualSubject + "</b>" + "<br/><br/>" + bodyForEvent;
                            }
                        }
                    }
                    try
                    {
                        // Send SMTP message
                        SendMessage((SmtpDestination)combinedNotificationContextList[0].Destination, subjectMultiMetric, finalEmailBody);
                                }
                                catch (SmtpException se) // If its SMTP exception, see if retry is needed
                                {
                        combinedNotificationContextList[combinedNotificationContextList.Count-1].LastSendException = se;
                        
                                    if (HandleFailedSend(se)) // throwing this to the caller causes a retry
                                        throw;
                        LOG.Error("Smtp notification - send failed", se);
                        //return 0;
                    }
                    catch (Exception e) // Any other exception log and move on
                    {
                        combinedNotificationContextList[combinedNotificationContextList.Count - 1].LastSendException = e;
                        LOG.Error("Smtp notification - send failed", e);

                    }
                    
                }
                
                if (atomicNotificationContextList.Count > 0)
                {
                        foreach (NotificationContext context in atomicNotificationContextList)
                        {
                        numberOfProcessedContexts++;
                        if (context != null)
                        {
                            try
                            {
                                SendContext(context);
                            }
                            catch (SmtpException se) // If its SMTP exception, see if retry is needed
                            {
                                context.LastSendException = se;
                                
                                if (HandleFailedSend(se)) // throwing this to the caller causes a retry
                                    throw;
                                LOG.Error("Smtp notification - send failed", se);
                                //return 0; 
                            }
                            catch (Exception e) // Any other exception log and move on
                            {
                                context.LastSendException = e;
                                LOG.Error("Smtp notification - send failed", e);

                            }
                        }
                    }
                  
                }
                
            }

            catch (SmtpException se) // If its SMTP exception, see if retry is needed
            {
               throw;
            }

                return allContexts.Count;
        }

        //calls Send function with a Notification Context to send an Smtp message.
        void SendContext(NotificationContext context)
        {

            int maxEventOccurrenceTimeInHrs = 8;
            try
            {
                if (context.SourceEvent != null && context.SourceEvent.OccuranceTime != null)
                {
                    // If the event was raised before 8 hours, skip notification.
                    TimeSpan timediff = DateTime.Now.ToUniversalTime() - context.SourceEvent.OccuranceTime;
                    if (timediff.Hours < maxEventOccurrenceTimeInHrs)
                    {
                        if (!Send(context))
                        {
                            string message =
                                String.Format("Notification provider error: {0}", context.LastSendException.Message);
                            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                          new MonitoredObjectName((string)null, (string)null),
                                                                  MonitoredState.Warning,
                                                                  context.LastSendException == null
                                                                      ? message
                                                                      : context.LastSendException.Message,
                                                                  message);
                        }
                    }
                    else
                    {
                        LOG.Verbose("Source event is older than 8 hours, not sending this notification");
                    }
                }
                else
                {
                    LOG.Error("Source event occurrence time is undefined, not sending this notification");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //end SQLdm 10.0 (Swati Gogia)


        private bool HandleFailedSend(SmtpException se)
        {
            bool retry = false;
            if (se is SmtpFailedRecipientsException) // If there are multiple recipient exceptions process them
            {                                       // individually for a retry
                SmtpFailedRecipientsException me = se as SmtpFailedRecipientsException;
                if (me.InnerExceptions.Length > 0)
                {
                    foreach (SmtpFailedRecipientException inner in me.InnerExceptions)
                    {
                        LogSmtpError(inner.Message);
                        if (IsRetryCode(inner.StatusCode))
                        {
                            retry = true;
                        }
                    }
                }
                else
                {
                    LogSmtpError(me.Message);
                }
            }
            else // Single exception check if retry is needed.
            {
                LogSmtpError(se.Message);
                if (IsRetryCode(se.StatusCode))
                {
                    retry = true;
                }
            }
            return retry;
        }

        private bool IsRetryCode(SmtpStatusCode code)
        {
            bool flag = false;
            switch (code)
            {
                case SmtpStatusCode.GeneralFailure:
                case SmtpStatusCode.InsufficientStorage:
                case SmtpStatusCode.MailboxBusy:
                case SmtpStatusCode.ServiceNotAvailable:
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
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
                        this.info = value as SmtpNotificationProviderInfo;
                    else
                    {
                        this.info = new SmtpNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Smtp notification provider {0}: {1}", operation, this.info.SmtpServer);
                }
            }
        }
    }
}
