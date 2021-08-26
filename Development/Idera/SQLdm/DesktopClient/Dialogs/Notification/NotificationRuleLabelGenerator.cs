//------------------------------------------------------------------------------
// <copyright file="NotificationRuleLabelGenerator.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Events;
using System.Collections.Generic;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common;
    using Wintellect.PowerCollections;

    public static class NotificationRuleLabelGenerator
    {
        public static int Rvalue { get; set; }
        public static int SelectedSeverityValue { get; set; }
        public static string RankSelectedValue { get; set; }
        public static string SelectedCmbValues { get; set; }
        static string[] AddEditValidate;
        private const string TOP_EDIT = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" +
                                        "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                                        "<head>" +
                                        "<title></title>" +
                                        "<style type='text/css'> " +
                                        "A:link     { color: blue; }" +
                                        "A:visited  { color: blue; }" +
                                        "BODY       { font-family: 'Microsoft Sans Serif'; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px; }" +
                                        "TD         { font-size: 8.25pt;     } " +
                                        "TD.dest    { white-space: nowrap; } " +
                                        "TD.rule1   { padding-left: 0.25in;  } " +
                                        "TD.rule2   { padding-left: 0.50in;  } " +
                                        "TD.action1 { padding-left: 0.25in;  } " +
                                        "TD.action2 { padding-left: 0.50in;  } " +
                                        "input.button { font-size: 6pt; width=15; height=15; } " +
                                        "input.checkbox { height: 15; white-space: nowrap; overflow-x: hidden; } " +
                                        "</style>" +
                                        "</head>" +
                                        "<body>" +
                                            "<table width='100%' border='0'>";

        private const string TOP_DISPLAY = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" +
                                    "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                                        "<head>" +
                                            "<title></title>" +
                                            "<style type='text/css'> " +
                                                "A:link     { font-family: 'Microsoft Sans Serif'; font-size: 8.25pt; font-weight: bold; cursor: default; color: black; text-decoration: underline }" +
                                                "A:visited  { font-family: 'Microsoft Sans Serif'; font-size: 8.25pt; font-weight: bold; cursor: default; color: black; text-decoration: underline }" +
                                                "BODY       { font-family: 'Microsoft Sans Serif'; padding-right: 0px; padding-left: 0px; padding-bottom: 0px; margin: 0px; padding-top: 0px; }" +
                                                "TD         { font-size: 8.25pt;     } " +
                                                "TD.rule1   { padding-left: 0.25in;  } " +
                                                "TD.rule2   { padding-left: 0.50in;  } " +
                                                "TD.action1 { padding-left: 0.25in;  } " +
                                                "TD.action2 { padding-left: 0.50in;  } " +
                                            "</style>" +
                                        "</head>" +
                                        "<body>" +
                                            "<table width='100%' border='0'>";


        private const string NOSERVER_ROW = "<tr><td class='rule1'>for any monitored SQL Server Instance</td></tr>";
        private const string FIRST_ROW = "<tr><td>For each scheduled refresh {0}</td></tr>";

        private const string RULE1_ROW = "<tr><td class='rule1'>and {0}</td></tr>";
        private const string RULE1_ROW_RANK = "<tr><td class='rule2'>and {0}</td></tr>";
        private const string RULE2_ROW_RANK = "<tr><td class='rule2'>{0}</td></tr>";
        private const string RULE2_ROW = "<tr><td class='rule2'>{1} {0}</td></tr>";
        private const string ACTION1_ROW = "<tr><td class='action1'>{0}</td></tr>";
        private const string ACTION2_ROW = "<tr><td class='action2'>and{0}</td></tr>";
        private const string BOTTOM = "</table></body></html>";
        private const string DESTINATION_ROW = "<tr>"
                                                + "<td class='dest'>{0}</td>"
                                                + "<td class='dest'>{1}</td>"
                                                + "<td class='dest'>"
                                                + "<a href='internal:RemoveDestination?{2}'>Remove</a>"
                                                + "</td>"
                                                + "</tr>";
        private const string AND = "and";
        private const string OR = "or";

        private const string NO_SERVER =
            "SQL Server Instance is in <a href='internal:Servers'><font color='red'>specified list</font></a>";

        private const string HAS_SERVER =
            "SQL Server Instance is <a href='internal:Servers'>{0}</a>";

        private const string NO_TAGS =
            "SQL Server Instance has tags in <a href='internal:ServerTags'><font color='red'>specified list</font></a>";

        private const string HAS_TAGS =
            "SQL Server Instance has tags <a href='internal:ServerTags'>{0}</a>";

        private const string ALL_METRICS =
            "any metric";

        private const string NO_METRIC =
            "the <a href='internal:Metrics'><font color='red'>metric</font></a>";

        //SQLDM-26122 - Code change for rule description when "Where all metrics.."is selected
        private const string NO_METRIC_ALL =
          "all the <a href='internal:Metrics'><font color='red'>metrics</font></a>";

        private const string RANK_VALUE = "<a href='internal:RankValue'><font color='red'>Alert Rank is</font></a>";
        private const string SELECTED_RANK_VALUE = "<a href='internal:RankValue'><font color='blue'>{0}</font></a>";

        private const string SELECTED_RANK_VALUE_BLACK = "<a href='internal:RankValue'><font color='black'>{0}</font></a>";

        private const string METRIC_SEVERITY = "<a href='internal:MetricSeverityValue'><font color='red'>specific time frame</font></a>";
        private const string METRIC_SEVERITY_MINUTES_BLUE = "<a href='internal:MetricSeverityValue'><font color='blue'>minutes</font></a>";
        private const string METRIC_SEVERITY_MINUTES_BLACK = "<a href='internal:MetricSeverityValue'><font color='black'>minutes</font></a>";

        private const string SELECTED_METRIC_SEVERITY = "<a href='internal:MetricSeverityValue'><font color='blue'>{0}</font></a>";
        private const string SELECTED_METRIC_SEVERITY_BLCAK = "<a href='internal:MetricSeverityValue'><font color='black'>{0}</font></a>";

        private const string HAS_METRIC =
            "<a href='internal:Metrics'>{0}</a>";

        private const string METRIC_LINE = "{0} {1}";

        private const string NO_STATE =
            "severity is <a href='internal:MetricStateDeviation'><font color='red'>equal to</font></a>";

        private const string HAS_STATE =
            "severity is <a href='internal:MetricStateDeviation'>{0}</a>";

        private const string NO_STATECHANGE = "";

        private const string HAS_STATECHANGE = "metric severity has changed";
        private const string HAS_STATEUNCHANGE = "metric severity has unchanged";

        private const string NO_TIME =
            "occuring <a href='internal:SnapshotTime'><font color='red'>during this time</font></a>";

        private const string HAS_TIME =
            "occurring <a href='internal:SnapshotTime'>{0}</a>";

        private static string BuildServerNameRule(ServerNameRule snr)
        {
            if (snr == null || !snr.Enabled)
                return String.Empty;

            if (snr.ServerNames.Count == 0)
                return NO_SERVER;

            // build a list of server names 
            StringBuilder builder = new StringBuilder();
            foreach (string name in snr.ServerNames)
            {
                if (builder.Length > 0)
                    builder.Append(" or ");
                builder.Append("'").Append(name).Append("'");
            }

            return String.Format(HAS_SERVER, builder);
        }

        private static string BuildServerTagRule(ServerTagRule str, Dictionary<int, Pair<string, int>> tagMap)
        {
            if (str == null || !str.Enabled)
                return String.Empty;

            if (str.ServerTagIds.Count == 0)
                return NO_TAGS;

            // build a list of server names 
            Pair<string, int> tagPair;
            StringBuilder builder = new StringBuilder();
            foreach (int tagId in str.ServerTagIds)
            {
                if (tagMap.TryGetValue(tagId, out tagPair))
                {
                    if (builder.Length > 0)
                        builder.Append(" or ");
                    builder.Append("'").Append(tagPair.First).Append("'");
                }
            }
            if (builder.Length > 0)
                return String.Format(HAS_TAGS, builder);

            // if you get here it means that there are tags assigned to the rule but they no longer exist.
            return NO_TAGS;
        }

        private static string BuildMetricLink(NotificationRule rule, MetricDefinitions metricDefinitions)
        {
            //SQLDM-26122 - Change to display "Where any metric.." in rule description when "Where all metrics.."is selected
            if (rule.MetricIDs == null)
                return ALL_METRICS;
            if (rule.MetricIDs.Count == 0 && rule.IsMetricsWithAndChecked == true)
                return NO_METRIC_ALL;
            if (rule.MetricIDs.Count == 0)
                return NO_METRIC;

            MetricDescription? metricDescription;
            StringBuilder builder = new StringBuilder();

            // Adding the code to write the metric description based on the and/or checkbox check condition
            if (!rule.IsMetricsWithAndChecked)
            {
                // for metric rules with 'or' clause
                foreach (int metricID in rule.MetricIDs)
                {
                    metricDescription = metricDefinitions.GetMetricDescription(metricID);
                    if (metricDescription.HasValue)
                    {
                        if (builder.Length > 0)
                            // reverting this change - metric rules with 'and' clause instead of 'or' clause
                            builder.Append(" or ");
                        builder.AppendFormat("'{0}'", metricDescription.Value.Name);
                    }
                }


            }
            else
            {
                // for metric rules with 'and' clause
                foreach (int metricID in rule.MetricIDs)
                {
                    metricDescription = metricDefinitions.GetMetricDescription(metricID);
                    if (metricDescription.HasValue)
                    {
                        if (builder.Length > 0)
                            // metric rules with 'and' clause instead of 'or' clause
                            builder.Append(" and ");
                        builder.AppendFormat("'{0}'", metricDescription.Value.Name);
                    }
                }
            }

            return String.Format(HAS_METRIC, builder.ToString());
        }

        private static string BuildRankValuesLink(NotificationRule rule)
        {
            string retVal = string.Empty;
            string rankValue = string.Empty;
            string rankCondition = string.Empty;
            string selectedRankValue = string.Empty;
            rankCondition = rule.CmbRankValue;
            rankValue = rule.RankValue;
            SelectedCmbValues = rankCondition;
            RankSelectedValue = rankValue;
            retVal = GetAlertRankResponseText(rankCondition, rankValue, rule.IsRankValueChecked, rule.IsEditCheckAlertRankResponse, rule.IsAlertResponseDailogClick);
            return retVal;
        }
        private static String GetAlertRankResponseText(String rankCondition, String strRankValue, Boolean IsRankValueChecked, bool IsEditCheckAlertRankResponse, bool IsAlertResponseDailogClick)
        {
            String retVal = String.Empty;
            string cmbrankvalue = String.Empty;
            bool IsEditMode = false;

            if (!String.IsNullOrEmpty(strRankValue))
            {
                AddEditValidate = strRankValue.Trim().Split();
                if (AddEditValidate.Length > 1)
                {
                    rankCondition = AddEditValidate[0];
                    strRankValue = AddEditValidate[1];
                    RankSelectedValue = strRankValue;
                    SelectedCmbValues = rankCondition;
                    if (!IsEditCheckAlertRankResponse)
                        IsEditMode = true;
                }
            }

            if (!String.IsNullOrEmpty(rankCondition))
            {
                if (rankCondition.ToString() == "0")
                    cmbrankvalue = ">";
                else if (rankCondition.ToString() == "1")
                    cmbrankvalue = "<";
                else if (rankCondition.ToString() == "2")
                    cmbrankvalue = "=";
            }

            if (IsEditMode || IsAlertResponseDailogClick)
                retVal = string.Format(RULE1_ROW_RANK, string.Format(SELECTED_RANK_VALUE_BLACK, string.Format("Alert Rank is {0} {1}", cmbrankvalue, strRankValue)));
            else if (IsRankValueChecked && string.IsNullOrEmpty(strRankValue) && string.IsNullOrEmpty(cmbrankvalue))
                retVal = string.Format(RULE1_ROW_RANK, RANK_VALUE);
            else if ((IsEditCheckAlertRankResponse) || ((!string.IsNullOrEmpty(strRankValue) && (!string.IsNullOrEmpty(cmbrankvalue)))))
                retVal = string.Format(RULE1_ROW_RANK, string.Format(SELECTED_RANK_VALUE, string.Format("Alert Rank is {0} {1}", cmbrankvalue, strRankValue)));
            return retVal;
        }


        private static string BuildMetricSeverityLink(NotificationRule rule)
        {
            string retVal = string.Empty;
            // string metricseverityvalue = string.IsNullOrWhiteSpace(rule.MetricSeverityValue) ? "4" : rule.MetricSeverityValue;
            string metricseverityvalue = rule.MetricSeverityValue;

            if (!rule.IsMetricSeverityAndUnchecked)
            {
                return string.Empty;
            }

            retVal = GetSeverityLinkText(rule.IsMetricSeverityChecked, metricseverityvalue, rule.IsMetricSheverityDialogOk, rule.IsMetricsSheverityEdit);
            return retVal;
        }
        private static string GetSeverityLinkText(bool IsMetricSeverityChecked, string metricseverityvalue, bool IsMetricSheverityDialogOk, bool IsMetricsSheverityEdit)
        {
            String retVal = String.Empty;
            if (IsMetricSeverityChecked && (IsMetricsSheverityEdit || IsMetricSheverityDialogOk))
                retVal = string.Format(string.Format(SELECTED_METRIC_SEVERITY, string.Format("specific time frame {0} {1}", metricseverityvalue, METRIC_SEVERITY_MINUTES_BLUE)));
            else if (IsMetricSeverityChecked && (!string.IsNullOrEmpty(metricseverityvalue)))
                retVal = string.Format(string.Format(SELECTED_METRIC_SEVERITY_BLCAK, string.Format("specific time frame {0} {1}", metricseverityvalue, METRIC_SEVERITY_MINUTES_BLACK)));
            else if (IsMetricSeverityChecked && string.IsNullOrEmpty(metricseverityvalue))
                retVal = string.Format(METRIC_SEVERITY);
            return retVal;
        }
        private static string BuildStateComparisonRule(MetricStateRule msr)
        {
            if (msr == null || !msr.Enabled)
                return "";

            if (!msr.IsCritical && !msr.IsWarning && !msr.IsOK && !msr.IsInformational)
                return NO_STATE;

            StringBuilder builder = new StringBuilder();
            if (msr.IsOK)
                builder.Append("OK");

            if (msr.IsInformational)
            {
                if (builder.Length > 0)
                    builder.Append(" or ");
                builder.Append("Informational");
            }
            if (msr.IsWarning)
            {
                if (builder.Length > 0)
                    builder.Append(" or ");
                builder.Append("Warning");
            }
            if (msr.IsCritical)
            {
                if (builder.Length > 0)
                    builder.Append(" or ");
                builder.Append("Critical");
            }

            return String.Format(HAS_STATE, builder.ToString());
        }

        public static bool IsValid(MetricStateChangeRule mscr)
        {
            //            // each list must have an entry
            //            if (mscr.PreviousState.Count == 0 || mscr.NewState.Count == 0)
            //                return false;
            //            // if there is only one entry in each list they must be different
            //            if (mscr.PreviousState.Count == 1 && mscr.NewState.Count == 1)
            //            {
            //                if (mscr.PreviousState[0] == mscr.NewState[0])
            //                    return false;
            //            }
            return true;
        }

        private static string BuildStateChangeComparisonRule(MetricStateChangeRule mscr)
        {
            if (mscr == null || !mscr.Enabled)
                return NO_STATECHANGE;

            return HAS_STATECHANGE;
            //            string template = IsValid(mscr) ? HAS_STATECHANGE : NO_STATECHANGE;
            //
            //            StringBuilder builder = new StringBuilder();
            //            if (mscr.PreviousState.Count == 0)
            //            {
            //                builder.Append("?");
            //            } else
            //            {
            //                foreach (MonitoredState state in mscr.PreviousState)
            //                {
            //                    if (builder.Length > 0)
            //                        builder.Append(" or ");
            //                    builder.AppendFormat("{0:G}", state);
            //                }
            //            }
            //            builder.Append(" to ");
            //
            //            if (mscr.NewState.Count == 0)
            //            {
            //                builder.Append("?");
            //            }
            //            else
            //            {
            //                int mark = builder.Length;
            //                foreach (MonitoredState state in mscr.NewState)
            //                {
            //                    if (builder.Length > mark)
            //                        builder.Append(" or ");
            //                    builder.AppendFormat("{0:G}", state);
            //                }
            //            }
            //            return String.Format(template, builder.ToString());
        }

        private static string BuildStateChangeComparisonRuleseverity(MetricStateChangeRule mscr)
        {
            if (mscr == null || !mscr.Enabled)
                return NO_STATECHANGE;
            return HAS_STATEUNCHANGE;
        }

        private static bool IsValid(SnapshotTimeRule str)
        {
            return !(!str.IsStartTimeSet && !str.IsEndTimeSet && !str.AnyDays);
        }

        private static string FormatTime(TimeSpan time)
        {

            string ampm = "AM";
            int hour = time.Hours;
            if (hour >= 12)
            {
                hour = hour - 12;
                ampm = "PM";
            }
            if (hour == 0)
                hour = 12;

            return String.Format("{0}:{1:D2} {2}", hour, time.Minutes, ampm);
        }

        private static string BuildSnapshotTimeComparisonRule(SnapshotTimeRule str)
        {
            if (str == null || !str.Enabled)
                return "";

            if (!IsValid(str))
                return NO_TIME;

            StringBuilder builder = new StringBuilder();

            int mark = builder.Length;
            if (str.StartTime != str.EndTime)
            {
                builder.AppendFormat("between {0} and {1}",
                                    FormatTime(str.StartTime),
                                    FormatTime(str.EndTime));
                if (str.StartTime > str.EndTime)
                    builder.Append(" the following day");
            }

            //            if (str.IsStartTimeSet)
            //            {
            //                if (str.IsEndTimeSet)
            //                {
            //                    builder.AppendFormat("between {0} and {1}",
            //                                         FormatTime(str.StartTime),
            //                                         FormatTime(str.EndTime));
            //                    if (str.StartTime < str.EndTime)
            //                        builder.Append(" the next day");
            //                }
            //                else
            //                    builder.AppendFormat("after {0}", FormatTime(str.StartTime));
            //            }
            //            else
            //            if (str.IsEndTimeSet)
            //            {
            //                builder.AppendFormat("before {0}", FormatTime(str.EndTime));
            //            }

            if (str.AnyDays)
            {
                if (builder.Length > mark)
                    builder.Append(" on ");

                if (str.AllDays)
                {
                    builder.Append("<b>all</b> days");
                }
                else
                {
                    mark = builder.Length;

                    if (str.Sunday)
                    {
                        builder.Append("Sunday");
                    }
                    if (str.Monday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Monday");
                    }
                    if (str.Tuesday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Tuesday");
                    }
                    if (str.Wednesday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Wednesday");
                    }
                    if (str.Thursday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Thursday");
                    }
                    if (str.Friday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Friday");
                    }
                    if (str.Saturday)
                    {
                        if (builder.Length > mark)
                            builder.Append(" or ");
                        builder.Append("Saturday");
                    }
                }
            }
            return String.Format(HAS_TIME, builder.ToString());
        }

        private static string BuildRuleLinks(NotificationRule rule, MetricDefinitions metricDefinitions, Dictionary<int, Pair<string, int>> tagMap)
        {
            StringBuilder builder = new StringBuilder();
            string template = RULE1_ROW;
            string stateChangeComparisonConnector = AND;

            string chunk = BuildServerNameRule(rule.ServerNameComparison);
            if (String.IsNullOrEmpty(chunk))
            {
                if (!rule.ServerTagComparison.Enabled)
                    builder.Append(NOSERVER_ROW);
            }
            else
            {
                builder.Append(String.Format(template, chunk, AND));
                template = RULE2_ROW;
            }

            chunk = BuildServerTagRule(rule.ServerTagComparison, tagMap);
            if (!String.IsNullOrEmpty(chunk))
            {
                builder.Append(String.Format(template, chunk, OR));
                template = RULE2_ROW;
            }

            // we will always have these
            chunk = string.Format(METRIC_LINE, BuildMetricLink(rule, metricDefinitions), BuildStateComparisonRule(rule.StateComparison));
            builder.Append(string.Format(template, chunk, AND));

            template = RULE2_ROW;
            if (!rule.IsMetricSeverityChecked)
                chunk = BuildStateChangeComparisonRule(rule.StateChangeComparison);
            else
            {
                chunk = BuildStateChangeComparisonRuleseverity(rule.StateChangeComparison);
                chunk += " ";
                chunk += BuildMetricSeverityLink(rule);
            }

            if (rule.IsMetricSeverityAndUnchecked == false)
                chunk = string.Empty;

            if (!string.IsNullOrEmpty(chunk))
            {
                builder.Append(string.Format(template, chunk, stateChangeComparisonConnector));
                template = RULE2_ROW;
            }

            if (rule.IsRankValueChecked)
            {
                chunk = BuildRankValuesLink(rule);
                builder.Append(String.Format(chunk));
            }

            return builder.ToString();
        }

        private static List<NotificationDestinationInfo> GetDestinationsForProvider(NotificationRule rule, NotificationProviderInfo provider)
        {
            List<NotificationDestinationInfo> result = new List<NotificationDestinationInfo>();
            foreach (NotificationDestinationInfo destination in rule.Destinations)
            {
                //     if (destination.Provider == null)
                //         FixupDestinationProvider()

                if (destination.Provider.GetType() == provider.GetType())
                    result.Add(destination);
            }

            return result;
        }

        public static void FixupDestinationProvider(IManagementService managementService, NotificationRule rule)
        {
            IList<NotificationProviderInfo> providers = null;

            if (rule.Destinations.Count > 0)
            {
                foreach (NotificationDestinationInfo dest in rule.Destinations)
                {
                    if (dest.Provider == null)
                    {
                        if (providers == null)
                            providers = managementService.GetNotificationProviders();

                        foreach (NotificationProviderInfo npi in providers)
                        {
                            if (npi.Id == dest.ProviderID)
                            {
                                dest.Provider = npi;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static StringBuilder GetDestinationAnchor(NotificationDestinationInfo destination, string linkId)
        {
            NotificationProviderInfo provider = destination.Provider;

            StringBuilder sb = new StringBuilder(provider.ProviderTypeLabel);
            string begin = sb.ToString();
            string text = "";
            string end = "";
            int i = begin.IndexOf("<a");
            if (i >= 0)
            {
                i = begin.IndexOf('>');

                int j = begin.IndexOf("</a>", i);
                if (j > 0)
                    end = begin.Substring(j);
                else
                    end = "</a>";
                text = begin.Substring(i + 1, j - i - 1);
                begin = begin.Substring(0, i + 1);
            }

            begin = String.Format(begin, String.IsNullOrEmpty(linkId) ? "" : "?" + linkId);

            StringBuilder line = new StringBuilder();
            string destinatonText = destination.ToString();
            if (String.IsNullOrEmpty(destinatonText))
            {
                line.Append(begin);
                line.Append("<font color=\"red\">");
                line.Append(text);
                line.Append("</font>");
                line.Append(end);
            }
            else
            {
                line.Append(begin);
                line.Append(destinatonText);
                line.Append(end);
            }
            return line;
        }


        public static string RebuildLinks(IManagementService managementService, NotificationRule rule, List<NotificationProviderInfo> providerTypes, MetricDefinitions metricDefinitions, bool displayOnly, Dictionary<int, Pair<string, int>> tagMap)
        {
            StringBuilder builder = new StringBuilder(displayOnly ? TOP_DISPLAY : TOP_EDIT);
            string chunk = BuildSnapshotTimeComparisonRule(rule.SnapshotTimeComparison);
            builder.Append(String.Format(FIRST_ROW, chunk));
            chunk = BuildRuleLinks(rule, metricDefinitions, tagMap);
            builder.Append(chunk);

            string template = ACTION1_ROW;

            // make sure destinations have the provider set
            FixupDestinationProvider(managementService, rule);

            int destinationRow = 0;

            foreach (NotificationDestinationInfo destination in rule.Destinations)
            {
                if (!destination.Enabled)
                    continue;

                // force first character to lower case
                StringBuilder sb = GetDestinationAnchor(destination, destinationRow.ToString());
                if (Char.IsUpper(sb[0]))
                    sb[0] = Char.ToLower(sb[0]);
                builder.AppendFormat(template, sb);
                template = ACTION2_ROW;
                destinationRow++;
            }

            builder.Append(BOTTOM);
            return builder.ToString();
        }

        public static string GetDestinationCheckList(IManagementService managementService, NotificationRule rule)
        {
            StringBuilder builder = new StringBuilder(TOP_EDIT);

            // make sure destinations have the provider set
            FixupDestinationProvider(managementService, rule);

            StringBuilder line = new StringBuilder();

            int destinationRow = 0;
            foreach (NotificationDestinationInfo destination in rule.Destinations)
            {
                if (line.Length > 0)
                    line.Length = 0;

                string enabledLink = String.Format("<a href='internal:DestinationCheckChanged?{0}'>{1}</a>",
                                                   destinationRow,
                                                   destination.Enabled ? "Disable" : "<font color=\"red\">Enable</font>");

                line.Append(GetDestinationAnchor(destination, destinationRow.ToString()));

                builder.AppendFormat(DESTINATION_ROW, enabledLink, line, destinationRow);
                destinationRow++;
            }

            builder.Append(BOTTOM);

            return builder.ToString();
        }
    }
}
