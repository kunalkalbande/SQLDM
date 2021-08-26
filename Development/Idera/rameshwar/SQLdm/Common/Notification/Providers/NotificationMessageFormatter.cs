//------------------------------------------------------------------------------
// <copyright file="NotificationMessageFormatter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification.Providers
{
    using System;
    using System.Text;
    using Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;

    public static class NotificationMessageFormatter
    {
        private const string STANDARD_STUFF =
            "Metric: $(Metric)  Severity: $(Severity)  Instance: $(Instance) {0}{1}Occurred at: $(Timestamp)";

        /// <summary>
        /// Not Available Value - N/A
        /// </summary>
        private const string NOT_AVAILABLE = "N/A";

        /// <summary>
        /// Hostname Placeholder to format message $(Hostname)
        /// </summary>
        private const string HOSTNAME_PLACEHOLDER = "$(Hostname)";

        public enum EscapeFormat
        {
            None,
            Sql,
            News
        }

        public static string FormatMessage(string template, AlertableSnapshot refresh, IEvent ievent, bool insertStandardStuff)
        {
            return FormatMessage(template, refresh, ievent, insertStandardStuff, EscapeFormat.None);
        }

        public static string FormatMessage(string template, AlertableSnapshot refresh, IEvent ievent, bool insertStandardStuff, EscapeFormat escapeFormat)
        {
            MonitoredObjectName mo = ievent.MonitoredObject;
                //escapeFormat != EscapeFormat.News
                //                         ? ievent.MonitoredObject
                //                         : new WrappedObjectName(ievent.MonitoredObject);

            StringBuilder builder = new StringBuilder();

            if (insertStandardStuff)
            {
                builder.AppendFormat(STANDARD_STUFF,
                                     mo != null && !String.IsNullOrEmpty(mo.DatabaseName) ? "Database: [$(Database)]  " : "",
                                     mo != null && mo.IsTable ? "Table: $(Table)  " : "");

                builder.Append("\n\n");
            }
            builder.Append(template);

            builder.Replace("$(Severity)", ievent.MonitoredState.ToString());
            
            string serverText = mo != null && !String.IsNullOrEmpty(mo.ServerName) ? mo.ServerName : NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)           
                serverText = EscapeSql(serverText);
            builder.Replace("$(Instance)", serverText);

            // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
            var serverHostName = refresh != null && !string.IsNullOrEmpty(refresh.ServerHostName) ? refresh.ServerHostName : NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                serverText = EscapeSql(serverHostName);
            builder.Replace(HOSTNAME_PLACEHOLDER, serverHostName);

            string vartext = mo != null && !String.IsNullOrEmpty(mo.DatabaseName) ? mo.DatabaseName : NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                vartext = EscapeSql(vartext);
            builder.Replace("$(Database)", vartext);

            vartext = mo != null && mo.IsTable ? mo.TableName : NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                vartext = EscapeSql(vartext);
            builder.Replace("$(Table)", vartext);

            vartext = GetValueText(ievent.Value);
            if (escapeFormat == EscapeFormat.Sql)
                vartext = EscapeSql(vartext);
            builder.Replace("$(Value)", vartext);

            vartext = mo != null && mo.ResourceType != InstanceType.Unknown ? mo.ResourceName : NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                vartext = EscapeSql(vartext);
            builder.Replace("$(Resource)", vartext);

            builder.Replace("$(Timestamp)", ievent.OccuranceTime.ToLocalTime().ToString("G"));

            // process the alert header & text
            string header = "";
            string body = "";

            MessageMap messageMap = SharedMetricDefinitions.MetricDefinitions.GetMessages(ievent.MetricID);
            if (messageMap != null)
            {
                header = messageMap.FormatMessage(refresh, ievent, 
                    escapeFormat != EscapeFormat.News ? MessageType.Header : MessageType.Pulse);
                
                body = messageMap.FormatMessage(refresh, ievent,
                    escapeFormat != EscapeFormat.News ? MessageType.Body : MessageType.Pulse);
            }
            if (String.IsNullOrEmpty(header))
                header = NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                    header = EscapeSql(header);
            builder.Replace("$(AlertSummary)", header);

            if (String.IsNullOrEmpty(body))
                body = NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                body = EscapeSql(body);

            builder.Replace("$(AlertText)", body);

            

            // comments and metric name come from the metric info table
            MetricDescription? metricDescription = SharedMetricDefinitions.MetricDefinitions.GetMetricDescription(ievent.MetricID);
            string metricName = (metricDescription == null) ?
                MetricDefinition.GetMetric(ievent.MetricID).ToString() :
                metricDescription.Value.Name;

            switch (escapeFormat)
            {
                case EscapeFormat.Sql:
                    metricName = EscapeSql(metricName);
                    break;
                case EscapeFormat.News:
                    ReplaceTextWithReference(builder, "$Metric", metricName);
                    metricName = EscapeNewsVariable("$Metric", metricName);
                    break;
            }

            builder.Replace("$(Metric)", metricName);

            string comments = (metricDescription == null) ? NOT_AVAILABLE : metricDescription.Value.Comments;
            if (String.IsNullOrEmpty(comments))
                comments = NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                comments = EscapeSql(comments);
            builder.Replace("$(Comments)", comments);

            string description = (metricDescription == null) ? NOT_AVAILABLE : metricDescription.Value.Description;
            if (String.IsNullOrEmpty(description))
                description = NOT_AVAILABLE;
            if (escapeFormat == EscapeFormat.Sql)
                description = EscapeSql(description);

            builder.Replace("$(Description)", description);

            if (escapeFormat == EscapeFormat.News && !serverText.Equals(NOT_AVAILABLE))
                ReplaceTextWithReference(builder, "*", serverText);

            return builder.ToString();
        }

        private static void ReplaceTextWithReference(StringBuilder builder, string varName, string metricName)
        {
            string replacement = EscapeNewsVariable(varName, metricName);
            builder.Replace(metricName, replacement);
        }

        private static string EscapeNewsVariable(string varName, string text)
        {
            return String.Format("@[{0}:{1}]", varName, text);
        }

        public static String EscapeSql(string sql)
        {
            StringBuilder builder = new StringBuilder(sql);
            builder.Replace("'", "''");
            return builder.ToString();
        }

        private static string GetValueText(object value)
        {
            if (value == null)
                return "<No Value>";
            if (value is Enum)
            {
                return EnumDescriptionConverter.GetFlagEnumDescription((Enum)value);
            }
            return value.ToString();
        }

        internal class WrappedObjectName : MonitoredObjectName
        {
            public WrappedObjectName(MonitoredObjectName copy)
                : base(TagServer(copy.ServerName), copy.DatabaseName, copy.TableName)
            {
                if (copy.HasAdditionalQualifiers)
                    AdditionalQualifiers = copy.AdditionalQualifiers;
            }

            private static string TagServer(string server)
            {
                return "@[*:" + server + "]";
            }
        }
    }
}
