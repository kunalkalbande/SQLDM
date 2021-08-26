//------------------------------------------------------------------------------
// <copyright file="ValueListHelpers.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Helpers
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Infragistics.Win;
    using Idera.SQLdm.DesktopClient.Properties;

    public static class ValueListHelpers
    {

        /// <summary>
        /// Copy the source value list to the target value list.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyValueList(ValueList source, ValueList target)
        {
            ValueListItemsCollection targetList = target.ValueListItems;
            foreach (ValueListItem item in source.ValueListItems)
            {
                ValueListItem newitem = new ValueListItem(item.DataValue, item.DisplayText);
                if (item.HasAppearance)
                    newitem.Appearance = item.Appearance.Clone() as Appearance;

                targetList.Add(newitem);
            }
        }

        public static ValueListItem[] GetSeverityValueListItems()
        {
            List<ValueListItem> listItems = new List<ValueListItem>();

            foreach (MonitoredState severity in Enum.GetValues(typeof (MonitoredState)))
            {
                ValueListItem listItem = new ValueListItem((byte) severity, severity.ToString());
                switch (severity)
                {
                    case MonitoredState.OK:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
                        listItem.DisplayText = "OK";
                        break;
                    case MonitoredState.Informational:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusInfoSmall;
                        listItem.DisplayText = "Info";
                        break;
                    case MonitoredState.Warning:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
                        listItem.DisplayText = "Warning";
                        break;
                    case MonitoredState.Critical:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
                        listItem.DisplayText = "Critical";
                        break;
                }
                listItems.Add(listItem);
            }
            return listItems.ToArray();
        }

        public static ValueListItem[] GetTaskStatusValueListItems(bool forFilter)
        {
            List<ValueListItem> listItems = new List<ValueListItem>();

            foreach (TaskStatus status in Enum.GetValues(typeof(TaskStatus)))
            {
                ValueListItem listItem = new ValueListItem((byte)status, status.ToString());
                switch (status)
                {
                    case TaskStatus.NotStarted:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskNotStarted2;
                        listItem.DisplayText = "Not Started";
                        break;
                    case TaskStatus.InProgress:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                        global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskInProgress;
                        listItem.DisplayText = "In Progress";
                        break;
                    case TaskStatus.OnHold:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskOnHold2;
                        listItem.DisplayText = "On Hold";
                        break;
                    case TaskStatus.Completed:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskCompleted;
                        listItem.DisplayText = "Completed";
                        break;
                    case TaskStatus.AnyAll:
                        if (forFilter) {
                            listItem.DisplayText = "All";
                            break;
                        } else {
                            continue;
                        }
                    case TaskStatus.NotCompleted:
                        if (forFilter) {
                            listItem.DisplayText = "Not Completed";
                            break;
                        } else {
                            continue;
                        }
                    case TaskStatus.None:
                        continue;
                }
                listItems.Add(listItem);
            }
            return listItems.ToArray();
        }

        private static int ValueListItemTextComparer(ValueListItem left, ValueListItem right)
        {
            string l = left.DisplayText ?? left.ToString();
            string r = right.DisplayText ?? right.ToString();
            return l.CompareTo(r);
        }

        public static ValueListItem[] GetMetricValueListItems()
        {
            return GetMetricValueListItems(true);
        }

        public static ValueListItem[] GetMetricValueListItems(bool sort)
        {
            List<ValueListItem> listItems = new List<ValueListItem>();

            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                MetricDefinitions metrics = ApplicationModel.Default.MetricDefinitions;
                metrics.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

                MetricDescription? metricDescription;

                foreach (int metricID in metrics.GetMetricDescriptionKeys())
                {
                    if (metricID == (int)Metric.IndexRowHits || metricID == (int)Metric.FullTextRefreshHours)
                        continue;

                    metricDescription = metrics.GetMetricDescription(metricID);
                    if (metricDescription.HasValue)
                    {
                        ValueListItem listItem = new ValueListItem(metricID, metricDescription.Value.Name);
                        listItems.Add(listItem);
                    }
                }
            }
            if (sort)
            {
                // order the list alphabetically
                listItems.Sort(ValueListItemTextComparer);
            }

            return listItems.ToArray();
        }

        //public const string RAISED_TO_CRITICAL = "Raised to critical";
        //public const string LOWERED_TO_OK = "Lowered to OK";

        public const string REMAINED = "Remained {0}";
        public const string RAISED_TO = "Raised to {0}";
        public const string LOWERED_TO = "Lowered to {0}";

        public static ValueListItem[] GetTransitionValueListItems()
        {   
            List<ValueListItem> listItems = new List<ValueListItem>();

            foreach (Transition transition in Enum.GetValues(typeof(Transition)))
            {
                ValueListItem listItem = new ValueListItem((byte)transition);
                switch (transition)
                {
                    case Transition.OK_Info:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image = 
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToInfo;
                        listItem.DisplayText = String.Format(RAISED_TO, MonitoredState.Informational.ToString());
                        break;
                    case Transition.OK_Warning:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToWarning;
                        listItem.DisplayText = String.Format(RAISED_TO, MonitoredState.Warning.ToString());
                        break;
                    case Transition.OK_Critical:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical;
                        listItem.DisplayText = String.Format(RAISED_TO, MonitoredState.Critical.ToString());
                        break;
                    case Transition.Info_OK:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK;
                        listItem.DisplayText = String.Format(LOWERED_TO, MonitoredState.OK.ToString());
                        break;
                    case Transition.Info_Info:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedInfo;
                        listItem.DisplayText = String.Format(REMAINED, MonitoredState.Informational.ToString());
                        break;
                    case Transition.Info_Warning:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToWarning;
                        listItem.DisplayText = String.Format(RAISED_TO, MonitoredState.Warning.ToString());
                        break;
                    case Transition.Info_Critical:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical;
                        listItem.DisplayText = String.Format(RAISED_TO, MonitoredState.Critical.ToString());
                        break;
                    case Transition.Warning_OK:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK;
                        listItem.DisplayText = String.Format(LOWERED_TO, MonitoredState.OK.ToString());
                        break;
                    case Transition.Warning_Info:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToInfo;
                        listItem.DisplayText = String.Format(LOWERED_TO, MonitoredState.Informational.ToString());
                        break;
                    case Transition.Warning_Warning:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedWarning;
                        listItem.DisplayText = String.Format(REMAINED, MonitoredState.Warning.ToString());
                        break;
                    case Transition.Warning_Critical:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RaisedToCritical;
                        listItem.DisplayText = string.Format(RAISED_TO, MonitoredState.Critical.ToString());
                        break;
                    case Transition.Critical_OK:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToOK;
                        listItem.DisplayText = String.Format(LOWERED_TO, MonitoredState.OK.ToString());
                        break;
                    case Transition.Critical_Info:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToInfo;
                        listItem.DisplayText = string.Format(LOWERED_TO, MonitoredState.Informational.ToString()); ;
                        break;
                    case Transition.Critical_Warning:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.LoweredToWarning;
                        listItem.DisplayText = string.Format(LOWERED_TO, MonitoredState.Warning.ToString());
                        break;
                    case Transition.Critical_Critical:
                        listItem.Appearance = new Infragistics.Win.Appearance();
                        listItem.Appearance.Image =
                            global::Idera.SQLdm.DesktopClient.Properties.Resources.RemainedCritical;
                        listItem.DisplayText = string.Format(REMAINED, MonitoredState.Critical.ToString());
                        break;
                }
                listItems.Add(listItem);
            }
            return listItems.ToArray();
        }

    }
}
