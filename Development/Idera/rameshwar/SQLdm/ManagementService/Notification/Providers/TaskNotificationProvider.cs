//------------------------------------------------------------------------------
// <copyright file="ODBCNotificationProvider.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using BBS.TracerX;
namespace Idera.SQLdm.ManagementService.Notification.Providers
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Notification.Providers;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Microsoft.ApplicationBlocks.Data;
   
    using Wintellect.PowerCollections;

    public class TaskNotificationProvider : IBulkNotificationProvider
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("TaskNotificationProvider");
        private static readonly DataTable taskTable = new DataTable("Tasks");

        private const int ServerNameColumnLength = 256;
        private const int SubjectColumnLength = 256;
        private const int BodyColumnLength = 1024;

        private TaskNotificationProviderInfo info;
        private object sync = new object();
        private EventLog eventLog;

        static TaskNotificationProvider()
        {
            DataColumn idColumn = taskTable.Columns.Add("TaskID", typeof(int));
            taskTable.Columns.Add("ServerName", typeof(string));
            taskTable.Columns.Add("Subject", typeof (string));
            taskTable.Columns.Add("Message", typeof (string));
            taskTable.Columns.Add("Comments", typeof (string));
            taskTable.Columns.Add("Owner", typeof (string));
            taskTable.Columns.Add("CreatedOn", typeof (DateTime));
            taskTable.Columns.Add("CompletedOn", typeof (DateTime));
            taskTable.Columns.Add("Status", typeof (byte));
            taskTable.Columns.Add("Metric", typeof (int));
            taskTable.Columns.Add("Severity", typeof (byte));
            taskTable.Columns.Add("Value", typeof (double));
            taskTable.Columns.Add("EventID", typeof (int));
            taskTable.Columns.Add("DatabaseName", typeof(string));
        }
        
        public TaskNotificationProvider(NotificationProviderInfo info)
        {
            if (info is TaskNotificationProviderInfo)
                NotificationProviderInfo = info as TaskNotificationProviderInfo;
            else
                this.info = new TaskNotificationProviderInfo(info);
        }

        public void SetEventLog(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public bool Send(NotificationContext context)
        {
            TaskDestination destination = (TaskDestination)context.Destination;
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

            string subject = NotificationMessageFormatter.FormatMessage(destination.Subject, context.Refresh, baseEvent, false);
            string body = NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false);

            MessageMap messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);
            Message? message = null;
            if (messageMap != null)
                message = messageMap.GetMessageForValue(baseEvent.Value);

            if (string.IsNullOrEmpty(subject) && message != null)
            {
                subject = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
            }
            if (string.IsNullOrEmpty(subject))
            {
                subject = destination.Subject;
            }
            if (subject.TrimEnd().Length > SubjectColumnLength)
            {
                subject = subject.Substring(0, SubjectColumnLength - 3) + "...";
            }

            if (string.IsNullOrEmpty(body) && message != null)
                body = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
            if (string.IsNullOrEmpty(body))
            {
                body = destination.Body;
            }
            if (body.TrimEnd().Length > BodyColumnLength)
            {
                body = body.Substring(0, BodyColumnLength - 3) + "...";
            }

            int eventId = 0;
            if (message.HasValue)
                eventId = (int)message.Value.EventId;

            object value = null;
            try
            {
                if (baseEvent.Value != null)
                    value = Convert.ToDouble(value);
            } catch (Exception)
            {
                /* */
            }

            using (SqlConnection connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection())
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddTask"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters,
                                              baseEvent.MonitoredObject.ServerName, // server name
                                              subject,                              // subject
                                              body,                                 // message
                                              null,                                 // comments
                                              null,                                 // assigned to
                                              baseEvent.OccuranceTime,              // create date
                                              null,                                 // completed date
                                              (byte)TaskStatus.NotStarted,          // status
                                              baseEvent.MetricID,                   // metric
                                              baseEvent.MonitoredState,             // severity
                                              value,                                // value
                                              (int)eventId,                         // event id
                                              DBNull.Value                          // return task id
                    );

                    if (destination == null || destination.Users.Length == 0)
                    {   // write unassigned task
                        command.ExecuteNonQuery();   
                    } else
                    {
                        foreach (string user in destination.Users)
                        {   // write a task for each user in the destination
                            command.Parameters[4].Value = user;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            return true;
        }

        public int Send(NotificationContext[] notifications)
        {
            MetricDefinitions metricDefinitions = SharedMetricDefinitions.MetricDefinitions;

            using (DataTable table = taskTable.Clone())
            {
                foreach (NotificationContext context in notifications)
                {
                    TaskDestination destination = (TaskDestination)context.Destination;

                    IEvent baseEvent = (IEvent)context.SourceEvent;
                    if (baseEvent.AdditionalData is CustomCounterSnapshot)
                    {
                        // add in the metric description info to the additional 
                        // data if this is a custom counter snapshot
                        baseEvent.AdditionalData =
                            new Pair<CustomCounterSnapshot, MetricDescription?>(
                                (CustomCounterSnapshot)baseEvent.AdditionalData,
                                metricDefinitions.GetMetricDescription(baseEvent.MetricID));
                    }

                    string subject = NotificationMessageFormatter.FormatMessage(destination.Subject, context.Refresh, baseEvent, false);
                    string body = NotificationMessageFormatter.FormatMessage(destination.Body, context.Refresh, baseEvent, false);
                    string databaseName = baseEvent.MonitoredObject.DatabaseName;

                    MessageMap messageMap = metricDefinitions.GetMessages(baseEvent.MetricID);
                    Message? message = null;
                    if (messageMap != null)
                        message = messageMap.GetMessageForValue(baseEvent.Value);

                    if (string.IsNullOrEmpty(subject) && message != null)
                    {
                        subject = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Header);
                    }
                    if (string.IsNullOrEmpty(subject))
                    {
                        subject = destination.Subject;
                    }
                    if (subject.TrimEnd().Length > SubjectColumnLength)
                    {
                        subject = subject.Substring(0, SubjectColumnLength - 3) + "...";
                    }
                    if (string.IsNullOrEmpty(body) && message != null)
                        body = message.Value.FormatMessage(context.Refresh, baseEvent, MessageType.Body);
                    if (string.IsNullOrEmpty(body))
                    {
                        body = destination.Body;
                    }
                    if (body.TrimEnd().Length > BodyColumnLength)
                    {
                        body = body.Substring(0, BodyColumnLength - 3) + "...";
                    }

                    int eventId = 0;
                    if (message.HasValue)
                        eventId = (int)message.Value.EventId;


                    object value = DBNull.Value;
                    object baseValue = baseEvent.Value;
                    try
                    {
                        if (baseEvent.Value != null)
                        {
                            if (baseEvent.Value.GetType().IsEnum)
                            {
                                Type underlying = Enum.GetUnderlyingType(baseValue.GetType());
                                baseValue = Convert.ChangeType(baseValue, underlying);
                            }  
                            value = Convert.ToDouble(baseValue);
                        } 
                    }
                    catch (Exception)
                    {
                        /* */
                    }

                    string serverName = baseEvent.MonitoredObject.ServerName;
                   
                    object[] valueArray = new object[]
                        {
                            DBNull.Value,                         // task id
                            String.IsNullOrEmpty(serverName) ? (object)DBNull.Value : serverName,  // server name
                            subject,                              // subject
                            body,                                 // message
                            DBNull.Value,                         // comments
                            DBNull.Value,                         // assigned to
                            baseEvent.OccuranceTime,              // create date
                            DBNull.Value,                         // completed date
                            (byte)TaskStatus.NotStarted,          // status
                            baseEvent.MetricID,                   // metric
                            baseEvent.MonitoredState,             // severity
                            value,                                // value
                            (int)eventId,                         // event id
                            String.IsNullOrEmpty(databaseName) ? (object)DBNull.Value : databaseName // database name
                        };

                    if (destination == null || destination.Users.Length == 0)
                    {   // write unassigned task
                        DataRow row = table.NewRow();
                        row.ItemArray = valueArray;
                        table.Rows.Add(row);
                    }
                    else
                    {
                        foreach (string user in destination.Users)
                        {   // write a task for each user in the destination
                            valueArray[5] = user;
                            DataRow row = table.NewRow();
                            row.ItemArray = valueArray;
                            table.Rows.Add(row);
                        }
                    }
                }
                
                if (table.Rows.Count > 0)
                {
                    using (SqlConnection connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection())
                    {
                        connection.Open();
                        try
                        {
                            LOG.VerboseFormat("About to bulk copy {0} tasks", table.Rows.Count);
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.UseInternalTransaction | SqlBulkCopyOptions.FireTriggers, null))
                            {
                                bulkCopy.DestinationTableName = "Tasks";
                                bulkCopy.WriteToServer(table);
                            }
                            return notifications.Length;
                        }
                        catch (Exception e3)
                        {
                            LOG.Error("Exception doing bulk copy to the tasks table", e3);
                        }
                    }
                }
            }
           
            return 0;
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

                    if (value is TaskNotificationProviderInfo)
                        this.info = value as TaskNotificationProviderInfo;
                    else
                    {
                        this.info = new TaskNotificationProviderInfo(value);
                    }
                    LOG.DebugFormat("Task notification provider {0}", operation);
                }
            }
        }



    }
}
