using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Alerts
{
    using System.Configuration;
    using Common;
    using Controls;
    using Dialogs;
    using Helpers;
    using Common.Configuration;
    using Common.Events;
    using Common.Services;
    using Common.UI.Dialogs;
    using Views.Alerts;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinGrid;
    using Objects;
    using Properties;
    using Infragistics.Win.UltraWinToolbars;
    using Common.Objects.ApplicationSecurity;

    internal partial class ActiveAlertsView : ServerBaseView
    {
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const char DELTA_CHAR = (char)0x394;

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AlertsView");
        private static string DELTA_STRING = DELTA_CHAR.ToString();

        private static readonly object updateLock = new object();

        private readonly AlertFilter activeAlertsFilter = new AlertFilter();
        private readonly AlertFilter historicalAlertsFilter = new AlertFilter();

        public event EventHandler ForecastPanelVisibleChanged;
        public event EventHandler DetailsPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        // historical viewing support
        private DateTime? historicalSnapshotDateTime;
        private Exception historyModeLoadError = null;

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings = null;
        private bool initialized = false;

        private UltraGridColumn selectedColumn = null;

        private bool showHistoricalView = false;
        private Control focused;
        private int lastSplitterDistance = 0;

        public ActiveAlertsView(int instanceId): base(instanceId)
        {
            InitializeComponent();
            AdaptFontSize();

            alertsGrid.CreationFilter = new NoTooltip();

            noSelectionLabel.BringToFront();
            alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[instanceId];

            activeAlertsFilter.ActiveOnly = true;
            activeAlertsFilter.Instance = instance.InstanceName;

            historicalAlertsFilter.ActiveOnly = false;
            historicalAlertsFilter.Instance = instance.InstanceName;
        }


        public event EventHandler HistoricalSnapshotDateTimeChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                historicalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;

                if (HistoricalSnapshotDateTimeChanged != null)
                {
                    HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                }
            }
        }


        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ActiveAlertsView);
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != null && (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime ||
                HistoricalStartDateTime != currentHistoricalStartDateTime))
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                alertForecastPanel.InHistoryMode = false;
                LoadForecast();
                return GetRealTimeData();
            }
            else
            {
                alertForecastPanel.InHistoryMode = true;
                // SqlDM 10.2 (Anshul Aggarwal) - Fetch data depending on custom/snapshot view mode.
                if (ViewMode == ServerViewMode.Historical)
                {
                    return GetHistoricalData(HistoricalSnapshotDateTime, HistoricalSnapshotDateTime);
                }
                else
                {
                    return GetHistoricalData(HistoricalStartDateTime, HistoricalSnapshotDateTime);
                }
            }
        }

        private void LoadForecast()
        {
            using (Log.DebugCall("LoadForecast"))
            {
                DataTable table = null;
                DateTime start = DateTime.Now;

                try
                {
                    table = RepositoryHelper.GetServerForecasts(this.instanceId);
                }
                catch (Exception e)
                {
                    Log.Error("Caught exception loading forecasts for Active Alerts view.", e);
                }

                TimeSpan span = DateTime.Now - start;
                Log.Debug("ActiveAlertsView.LoadForecasts() took this long: ", span);

                alertForecastPanel.UpdateForecastData(table);
            }
        }

        private object GetRealTimeData()
        {
            return GetAlertData(activeAlertsFilter);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) -Fetches Alerts for the latest snapshot in the selected range.
        /// </summary>
        private object GetHistoricalData(DateTime? startDateTime, DateTime? endDateTime)
        {
            historicalAlertsFilter.BeginDate = startDateTime;
            historicalAlertsFilter.EndDate = endDateTime;
            return GetAlertData(historicalAlertsFilter);
        }

        private object GetAlertData(AlertFilter filter)
        {
            object begin = (filter.BeginDate.HasValue) ? (object)filter.BeginDate.Value.ToUniversalTime() : null;
            object end = (filter.EndDate.HasValue) ? (object)filter.EndDate.Value.ToUniversalTime() : null;
            object metric = (filter.Metric.HasValue) ? (object)((int)filter.Metric.Value) : null;
            object severity = (filter.Severity.HasValue) ? (object)((byte)filter.Severity.Value) : null;
            object activeOnly = (filter.ActiveOnly.HasValue && filter.ActiveOnly.Value)
                                    ? (object)true
                                    : null;

            using (LOG.DebugCall("DoRefreshWork"))
            {
                try
                {
                    // make sure the metric value list contains up-to-date metrics
                    ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
                    if (valueList.ValueListItems.Count == 0)
                        valueList.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());
                    else
                    {
                        MetricDescription? metricDescription;
                        // grab changes to metric definitions
                        MetricDefinitions metrics = ApplicationModel.Default.MetricDefinitions;
                        metrics.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
                        // only custom counters get added/removed while the product is running
                        foreach (int metricID in metrics.GetCounterDefinitionKeys())
                        {
                            ValueListItem listItem = valueList.FindByDataValue(metricID);
                            if (listItem == null)
                            {
                                metricDescription = metrics.GetMetricDescription(metricID);
                                if (metricDescription.HasValue)
                                {
                                    listItem = new ValueListItem(metricID, metricDescription.Value.Name);
                                    valueList.ValueListItems.Add(listItem);
                                }
                            }
                        }
                    }

                    if (!alertsViewDataSource.BindingNotificationsSuspended)
                        alertsViewDataSource.SuspendBindingNotifications();

                    RepositoryHelper
                        .LoadDataSource(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        alertsViewDataSource,
                      RepositoryHelper.GetAlertsForInstanceProcedure,       //SqlDM 10.2 (Anshul Aggarwal) - Fetches Alerts for Server > ActiveAlertsView
                        0, begin, end, instanceId, null, null, severity, metric, null, activeOnly, null);
                }
                catch (Exception e)
                {
                    return e;
                }
                return alertsViewDataSource;
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                //                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                //                fillPanel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            using (Log.VerboseCall())
            {
                if (HistoricalSnapshotDateTime != null)
                {
                    ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                          Properties.Resources.StatusWarningSmall);
                }
                else
                {
                    operationalStatusPanel.Visible = false;
                }

                if (data != null && data is Exception)
                {
                    Exception e = (Exception)data;
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default
                        .OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now, e));
                    //                   alertsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    //                   alertsGridStatusLabel.BringToFront();
                    return;
                }
                //               alertsGridStatusLabel.SendToBack();

                UltraGridHelper.GridState state = UltraGridHelper.GetGridState(alertsGrid, "AlertID");

                alertsGrid.SuspendLayout();
                alertsGrid.SuspendRowSynchronization();
                // resume binding - will cause group by rows to be recreated
                alertsViewDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, alertsGrid);
                        ConfigureView();

                        initialized = true;
                    }
                }

                UltraGridHelper.RestoreGridState(state);
                alertsGrid.ResumeRowSynchronization();
                alertsGrid.ResumeLayout();

                ApplicationController.Default.SetCustomStatus(
                    string.Empty,
                    String.Format("{0} Alert{1}",
                                  ((UltraDataSource)alertsGrid.DataSource).Rows.Count,
                                  ((UltraDataSource)alertsGrid.DataSource).Rows.Count == 1 ? string.Empty : "s")
                    );

                // SqlDM 10.2 (Anshul Aggaral) - Update start/end dates known to this screen.
                currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                currentHistoricalStartDateTime = HistoricalStartDateTime;
                ApplicationController.Default
                    .OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                if (alertsGrid.Selected.Rows.Count == 0 || !alertsGrid.Selected.Rows[0].IsDataRow)
                {
                    noSelectionLabel.BringToFront();
                }
            }
        }

        public void ConfigureView()
        {
            UltraGridBand band = alertsGrid.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();

            band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
            band.SortedColumns.Add(band.Columns["ServerName"], false, false);
            band.SortedColumns.Add(band.Columns["Severity"], true, false);
        }

        private void alertsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            using (Log.VerboseCall())
            {
                if (typeof(Infragistics.Win.UltraWinGrid.UltraGridRow).IsAssignableFrom(e.Type))
                {
                    Log.Verbose("UltraGridRow is assignable from type ", e.Type);
                    Log.Verbose("alertsGrid.Selected.Rows.Count = ", alertsGrid.Selected.Rows.Count);
                    if (alertsGrid.Selected.Rows.Count == 0 || !alertsGrid.Selected.Rows[0].IsDataRow)
                    {
                        Log.Verbose("Showing noSelectionLable");
                        noSelectionLabel.BringToFront();
                    }
                    else
                    {
                        Log.Verbose("Showing detailsContentPanel, alert id = ", alertsGrid.Selected.Rows[0].Cells["AlertID"].Value);

                        if (alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                        {
                            Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                            showRealTimeViewLinkLabel.Visible = m != Metric.Operational && m != Metric.SQLdmCollectionServiceStatus;
                            showHistoricalView = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);

                            showDetailsLinkLabel.Visible = (m == Metric.BlockingAlert);

                            //If we are showing the linklabel allowing drill-in to Block report
                            //move the help link to the bottom for consistency
                            if (showDetailsLinkLabel.Visible)
                            {
                                detailsContentPanel.SetCellPosition(showDetailsLinkLabel, new TableLayoutPanelCellPosition(6, 2));
                                detailsContentPanel.SetCellPosition(helpHistoryLinkLabel2, new TableLayoutPanelCellPosition(6, 3));
                            }
                            else
                            {
                                detailsContentPanel.SetCellPosition(helpHistoryLinkLabel2, new TableLayoutPanelCellPosition(6, 2));
                            }

                            if (showRealTimeViewLinkLabel.Visible)
                            {
                                if (m != Metric.Deadlock)
                                    showRealTimeViewLinkLabel.Text = "Show Real Time View";
                                else
                                    showRealTimeViewLinkLabel.Text = "Show Deadlock View";

                                if (showHistoricalView)
                                {
                                    helpHistoryLinkLabel1.Visible = true;
                                    helpHistoryLinkLabel2.Visible = m != Metric.Custom;
                                    helpHistoryLinkLabel1.Text = "Show Historical View";
                                    helpHistoryLinkLabel2.Text = "Show Alert Help";
                                }
                                else
                                {
                                    helpHistoryLinkLabel1.Visible = m != Metric.Custom;
                                    helpHistoryLinkLabel2.Visible = false;
                                    helpHistoryLinkLabel1.Text = "Show Alert Help";
                                }
                            }
                            else
                            {
                                helpHistoryLinkLabel1.Visible = false;
                                helpHistoryLinkLabel2.Visible = false;
                            }
                        }
                        else
                        {
                            showRealTimeViewLinkLabel.Visible = false;
                            helpHistoryLinkLabel1.Visible = false;
                            helpHistoryLinkLabel2.Visible = false;
                            showDetailsLinkLabel.Visible = false;
                        }

                        detailsContentPanel.BringToFront();
                    }
                }
                else
                {
                    Log.Verbose("UltraGridRow is NOT assignable from type ", e.Type);
                }
            }
        }

        private void alertsGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                NavigateToView(e.Row, false);
            }
        }

        private void NavigateToView(UltraGridRow row, bool showHistoricalSnapshot)
        {
            if (row != null)
            {
                string instanceName = row.Cells["ServerName"].Value as string;
                object argument = null;

                if (row.Cells["Metric"].Value != null)
                {
                    Metric metric = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
                    //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup from filegroup alert message
                    string filegroup = GetFilegroupFromAlertMessage((string)row.Cells["Message"].Value);

                    switch (metric)
                    {
                       case Metric.ReorganisationPct:
                          argument = row.Cells["DatabaseName"].Value as object;
                          break;
                        case Metric.PageLifeExpectancy:
                          argument = Metric.PageLifeExpectancy;
                            break;
                        // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                        case Metric.FilegroupSpaceFullSize:
                        case Metric.FilegroupSpaceFullPct:
                            argument = (object)(new object[] { row.Cells["DatabaseName"].Value, row.Cells["Metric"].Value, filegroup });
                            break;
                        // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                       default:
                          argument = null;
                          break;
                    }
                    
                    if (showHistoricalSnapshot)
                    {
                        DateTime snapshotDateTime = (DateTime)row.Cells["UTCOccurrenceDateTime"].Value;
                        ClickThroughHelper.NavigateToView(instanceName, metric, snapshotDateTime, argument);
                    }
                    else
                    {
                        if (metric == Metric.Deadlock)
                        {
                            ShowDeadlockDetails();
                            return;
                        }

                        ClickThroughHelper.NavigateToView(instanceName, metric, null, argument);
                    }
                }
            }
        }

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message
        private string GetFilegroupFromAlertMessage(string message)
        {
            string filegroup = null;
            string referenceString = "has a file group ";

            if (message.Length > 0)
            {
                int startIdx = message.IndexOf(referenceString) + referenceString.Length + 1;
                int lastIdx = message.LastIndexOf(']');
                if (startIdx < lastIdx)
                    filegroup = message.Substring(startIdx, lastIdx - startIdx);
            }

            return filegroup;
        }
        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message

        private void alertsGrid_InitializeGroupByRow(object sender, InitializeGroupByRowEventArgs e)
        {
            string key = e.Row.Column.Key;
            if (key == "Severity") // || key == "StateEvent")
            {
                ValueList valueList = e.Row.Column.ValueList as ValueList;
                if (valueList != null)
                {
                    ValueListItem item = valueList.FindByDataValue(e.Row.Value);
                    if (item != null)
                    {
                        e.Row.Description = String.Format("{0} ({1} item{2})",
                                                          item.DisplayText,
                                                          e.Row.Rows.Count,
                                                          e.Row.Rows.Count == 1 ? String.Empty : "s");
                    }
                }
            }
        }

        private void alertsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            bool enableClearOptions = false;

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                {
                    Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader =
                        contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                    ConfigureContextMenuItems();
                }
                else
                {
                    toolbarsManager.Tools["showDetailsButton"].SharedProps.Visible = !Settings.Default.AlertsViewDetailsPanelExpanded;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    if (selectedElement.SelectableItem is UltraGridRow)
                    {
                        ButtonTool tool = toolbarsManager.Tools["editAlertConfigurationButton"] as ButtonTool;
                        UltraGridRow row = ((UltraGridRow)selectedElement.SelectableItem);
                        if (row.IsDataRow)
                        {
                            alertsGrid.Selected.Rows.Clear();
                            row.Activate();
                            row.Selected = true;
                            // set the alert configuration edit button enabled if the selected row has a server name
                            UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                            object server = dataRow["ServerName"];
                            toolbarsManager.Tools["editAlertConfigurationButton"]
                                .SharedProps.Visible = (server is string && !String.IsNullOrEmpty((string)server));

                            bool enableSnoozeButton = false;
                            object active = dataRow["Active"];
                            object metric = dataRow["Metric"];
                            if (active is bool && ((bool)active))
                            {
                                if (metric is int)
                                {
                                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                                    switch (m)
                                    {
                                        case Metric.ReorganisationPct:
                                        case Metric.BombedJobs:
                                        case Metric.LongJobs:
                                        case Metric.LongJobsMinutes:
                                        case Metric.ClusterFailover:
                                        case Metric.JobCompletion:
                                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                                            enableClearOptions =
                                                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                                            break;
                                    }
                                }
                            }
                            if (alertsGrid.Selected.Rows.Count == 1)
                            {
                                //Operator Security Role Changes - 10.3 --Added the condition for ReadOnlyPlus Role
                                int id = ApplicationModel.Default.ActiveInstances[server as string].Id;
                                if (metric is int && !((int)metric == (int)Metric.MaintenanceMode || (int)metric == (int)Metric.Operational))
                                    enableSnoozeButton = (ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify)||(ApplicationModel.Default.UserToken.GetServerPermission(id)==PermissionType.ReadOnlyPlus);
                            }
                            toolbarsManager.Tools["snoozeAlertButton"].SharedProps.Visible = enableSnoozeButton;
                        }
                        else
                        {
                            toolbarsManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                        }
                        toolbarsManager.Tools["copyToClipboardButton"].SharedProps.Visible = true;
                    }
                    else
                    {
                        toolbarsManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                        toolbarsManager.Tools["copyToClipboardButton"].SharedProps.Visible = false;
                    }
                }
                toolbarsManager.Tools["clearAlertButton"].SharedProps.Visible = enableClearOptions;
                toolbarsManager.Tools["clearAllAlertsButton"].SharedProps.Visible = enableClearOptions;
            }

        }

        private void ConfigureContextMenuItems()
        {
            ButtonTool tool = toolbarsManager.Tools["toggleColumnTextButton"] as ButtonTool;
            if (tool == null)
                return;

            bool showingText =
                (selectedColumn.ValueList != null && selectedColumn.ValueList.ShouldDisplayText);

            switch (selectedColumn.Key)
            {
                case "Severity":
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.Caption = showingText ? "Hide Severity Description" : "Show Severity Description";
                    break;
                case "StateEvent":
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.Caption = showingText ? "Hide Change Description" : "Show Change Description";
                    break;
                default:
                    tool.SharedProps.Visible = false;
                    break;
            }
        }

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            // Switching to real-time mode is the only supported opertional action at this time
            SwitchToRealTimeMode();
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        #region One Time Stuff

        private void ActiveAlertsView_Load(object sender, EventArgs e)
        {            
            ConfigureAlertsGrid();

            Settings.Default.SettingChanging += Settings_SettingChanging;

            ApplySettings();
        }

        public override void  ApplySettings()
        {
            ForecastPanelVisible = Settings.Default.ActiveAlertsViewForecastPanelVisible;
            DetailsPanelVisible = Settings.Default.ActiveAlertsViewDetailsPanelExpanded;

            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.AlertsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }
        }

        public override void SaveSettings()
        {
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance)
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance = 
                    Settings.Default.AlertsViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
            }

            Settings.Default.ActiveAlertsViewForecastPanelVisible = ForecastPanelVisible;
            Settings.Default.ActiveAlertsViewDetailsPanelExpanded = DetailsPanelVisible;
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "ActiveAlertsViewDetailsPanelExpanded":
                    DetailsPanelVisible = (bool)e.NewValue;
                    break;
            }
        }

        private void ConfigureAlertsGrid()
        {
            alertsGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

            // switch out editor from EditorWithCombo to EditorWithText
            EditorWithText textEditor = new EditorWithText();
            alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].Editor = textEditor;
            alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].Editor = textEditor;

            // populate the metric value list
            ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
            valueList.ValueListItems.Clear();

            // configure the bindings for the details pane metric label
            if (metricLabel.DataBindings["Text"] == null)
            {
                ValueListBinding metricBindingText = new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".Metric");
                metricBindingText.ValueList = valueList;
                metricLabel.DataBindings.Add(metricBindingText);
            }
            ValueListBinding servertypeBindingText = new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".ServerType");
            servertypeBindingText.ValueList = valueList;
            ServerTypeText.DataBindings.Add(servertypeBindingText);
            // build the severity value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetSeverityValueListItems());

            // configure the bindings for the details pane severity image and label
            if (severityImage.DataBindings["Image"] == null)
            {
                ValueListBinding severityBindingImage = new ValueListBinding("Image", alertsGrid.DataSource, alertsGrid.DataMember + ".Severity");
                severityBindingImage.ValueList = valueList;
                severityImage.DataBindings.Add(severityBindingImage);
            }
            if (severityLabel.DataBindings["Text"] == null)
            {
                ValueListBinding severityBindingText = new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".Severity");
                severityBindingText.ValueList = valueList;
                severityLabel.DataBindings.Add(severityBindingText);
            }

            // load the transition value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTransitionValueListItems());
            alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].GroupByEvaluator =
                new AlertFilter.TransitionGroupByEvaluator(valueList);

            // configure the bindings for the details pane transition image and label
            if (transitionImage.DataBindings["Image"] == null)
            {
                ValueListBinding transitionBindingImage = new ValueListBinding("Image", alertsGrid.DataSource, alertsGrid.DataMember + ".StateEvent");
                transitionBindingImage.ValueList = valueList;
                transitionImage.DataBindings.Add(transitionBindingImage);
            }
            if (transitionLabel.DataBindings["Text"] == null)
            {
                ValueListBinding transitionBindingText = new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".StateEvent");
                transitionBindingText.ValueList = valueList;
                transitionLabel.DataBindings.Add(transitionBindingText);
            }
        }

        #endregion

        public bool GridGroupByBoxVisible
        {
            get { return !alertsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                alertsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
            }
        }

        #region Forecast Panel Stuff

        public bool ForecastPanelVisible
        {
            get { return forecastPanel.Visible; }
            set
            {
                forecastPanel.Visible = value;

                if (ForecastPanelVisibleChanged != null)
                    ForecastPanelVisibleChanged(this, EventArgs.Empty);
            }
        }

        private void toggleForecastPanelButton_Click(object sender, EventArgs e)
        {
            ForecastPanelVisible = !ForecastPanelVisible;
        }

        #endregion

        #region Details Panel Stuff

        public bool DetailsPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (DetailsPanelVisibleChanged != null)
                    DetailsPanelVisibleChanged(this, EventArgs.Empty);
            }
        }

        private void toggleDetailsPanelButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = !DetailsPanelVisible;
        }

        #endregion

        #region Toolbar Event Handling

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "toggleColumnTextButton":
                    ToggleColumnText();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "editAlertConfigurationButton":
                    EditAlertConfiguration();
                    break;
                case "showDetailsButton":
                    DetailsPanelVisible = true;
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                case "copyToClipboardButton":
                    UltraGridHelper.CopyToClipboard(alertsGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "clearAlertButton":
                    ClearAlert(false);
                    break;
                case "clearAllAlertsButton":
                    ClearAlert(true);
                    break;
                case "snoozeAlertButton":
                    SnoozeAlert();
                    break;
                case "viewRealTimeSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], false);
                    break;
                case "viewHistoricalSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], true);
                    break;
                case "viewAlertHelpButton":
                    ShowAlertHelp();
                    break;
                case "viewDeadlockDetailsButton":
                    ShowDeadlockDetails();
                    break;
                case "viewBlockDetailsButton":
                    ShowBlockReport();
                    break;
            }
        }

        private void ShowBlockReport()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            }
            catch (Exception)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.");
                return;
            }

            Log.Verbose("Showing detailsContentPanel, alert id = ", alertId);
            BlockDialog.Show(this, alertId);
        }

        private void ShowDeadlockDetails()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            }
            catch (Exception)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.");
                return;
            }

            Log.Verbose("Showing detailsContentPanel, alert id = ", alertId);
            DeadlockDialog.Show(this, alertId);
        }

        private void ShowAlertHelp()
        {
            if (alertsGrid.Selected.Rows.Count != 0)
            {
                // Get instance id and metric for the selected alert.
                if (alertsGrid.Selected.Rows[0].Cells["ServerName"].Value != null
                    && alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                {
                    // Get instance and metric.
                    string serverName = alertsGrid.Selected.Rows[0].Cells["ServerName"].Value as string;
                    int metricId = (int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value;
                    long alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
                    MonitoredSqlServerWrapper serverWrapper = ApplicationModel.Default.ActiveInstances[serverName];
                    Metric metric = MetricDefinition.GetMetric(metricId);

                    // If server and metric are valid, then proceed, else do nothing.
                    if (serverWrapper != null && metric != Metric.Custom)
                    {
                        string helpTopic;
                        if (ClickThroughHelper.GetMetricHelp(metric, out helpTopic))
                        {
                            ApplicationHelper.ShowHelpTopic(helpTopic);
                        }
                    }
                }
            }
        }

        private void SnoozeAlert()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];
                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    string selectedServerName = (string)dataRow["ServerName"] ?? String.Empty;
                    MonitoredSqlServerWrapper selectedServer =
                        ApplicationModel.Default.ActiveInstances[selectedServerName];

                    // Get Alert summary.
                    String alertSummary = dataRow["Heading"] as String;

                    // Displays snooze dialog.
                    SnoozeInfo snoozeInfo = SnoozeAlertsDialog.SnoozeAlert(
                        ParentForm,
                        selectedServer.Id,
                        (int)metric,
                        SnoozeAlertsDialog.SnoozeAction.Snooze,
                        alertSummary);

                    if (snoozeInfo != null)
                    {
                        dataRow.ParentCollection.Remove(dataRow);
                    }
                }
            }
        }

        private void ClearAlert(bool allAlerts)
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];
                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    switch (m)
                    {
                        case Metric.ReorganisationPct:
                        case Metric.BombedJobs:
                        case Metric.LongJobs:
                        case Metric.LongJobsMinutes:
                        case Metric.ClusterFailover:
                        case Metric.JobCompletion:
                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                            long alertID = (long)dataRow["AlertID"];
                            try
                            {
                                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                                managementService.ClearActiveAlerts(alertID, allAlerts);

                                //bool activeOnly = true;

                                // remove the row if we are showing only active alerts
                                if (allAlerts)
                                {
                                    string selectedServer = dataRow["ServerName"] as string ?? "";

                                    List<UltraDataRow> rows = new List<UltraDataRow>();
                                    foreach (UltraDataRow udr in dataRow.ParentCollection)
                                    {
                                        if (((bool)udr["Active"]) && metric.Equals(udr["Metric"]))
                                        {
                                            string rowServer = udr["ServerName"] as string ?? "";
                                            if (selectedServer.Equals(rowServer))
                                                rows.Add(udr);
                                        }
                                    }
                                    foreach (UltraDataRow udr in rows)
                                    {
                                        udr.ParentCollection.Remove(udr);
                                    }
                                }
                                else
                                {
                                    dataRow.ParentCollection.Remove(dataRow);
                                }
                            }
                            catch (Exception e)
                            {
                                ApplicationMessageBox.ShowError(this, e);
                            }

                            break;
                    }
                }
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridContextMenu")
            {
                if (alertsGrid.Selected.Rows.Count > 0)
                {
                    bool isViewRealTimeSnapshotButtonEnabled = false;
                    bool isViewHistoricalSnapshotButtonEnabled = false;
                    bool isViewAlertHelpButtonEnabled = false;
                    bool isViewDeadlockDetailsButtonEnabled = false;
                    bool isViewDetailsButtonEnabled = false;
                    bool isViewBlockDetailsButtonEnabled = false; 

                    if (alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                        switch (m)
                        {
                            case Metric.Deadlock:
                                isViewDeadlockDetailsButtonEnabled = true;
                                break;
                            case Metric.BlockingAlert:
                                isViewRealTimeSnapshotButtonEnabled = true;
                                isViewBlockDetailsButtonEnabled = true;
                                break;
                            default:
                                isViewRealTimeSnapshotButtonEnabled = m != Metric.Operational && m != Metric.SQLdmCollectionServiceStatus;
                                break;
                        }

                        isViewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                        isViewAlertHelpButtonEnabled = isViewRealTimeSnapshotButtonEnabled && m != Metric.Custom;
                        isViewDetailsButtonEnabled = !DetailsPanelVisible;
                    }

                    ((PopupMenuTool)e.Tool).Tools["showDetailsButton"].SharedProps.Visible =
                        isViewDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewBlockDetailsButton"].SharedProps.Visible =
                        isViewBlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible =
                        isViewDeadlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible =
                        isViewRealTimeSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible =
                        isViewHistoricalSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible =
                        isViewAlertHelpButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["showDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible = false;
                }

                bool isGrouped = alertsGrid.Rows.Count > 0 && alertsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {

                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(alertsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void EditAlertConfiguration()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object server = dataRow["ServerName"];
                if (server is string && !String.IsNullOrEmpty((string)server))
                {
                    MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[(string)server];
                    if (wrapper != null)
                    {
                        try
                        {
                            using (
                                AlertConfigurationDialog alertConfigDialog =
                                    new AlertConfigurationDialog(wrapper.Id, false)
                                )
                            {
                                object metric = dataRow["Metric"];
                                if (metric is int)
                                    metric = (Metric)Enum.ToObject(typeof(Metric), ((int)metric));

                                alertConfigDialog.CreateControl();
                                if (metric is Metric)
                                {
                                    alertConfigDialog.Select((Metric)metric);
                                }

                                alertConfigDialog.ShowDialog(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }
                    }
                }
            }
        }

        private void ToggleColumnText()
        {
            ValueList valueList = null;

            if (selectedColumn != null)
            {
                switch (selectedColumn.Key)
                {
                    case "Severity":
                        valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
                        break;
                    case "StateEvent":
                        valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
                        break;
                }

                if (valueList != null)
                {
                    if (valueList.DisplayStyle == ValueListDisplayStyle.Picture)
                    {
                        valueList.DisplayStyle = ValueListDisplayStyle.DisplayTextAndPicture;
                        selectedColumn.Header.Appearance.TextHAlign = HAlign.Default;
                        selectedColumn.CellAppearance.TextHAlign = HAlign.Default;
                        selectedColumn.CellAppearance.ImageHAlign = HAlign.Default;
                        if (valueList.Key == "Severity")
                        {
                            selectedColumn.Header.Caption = "Severity";
                            selectedColumn.Header.Appearance.ForeColor = Color.Black;
                            selectedColumn.Header.Appearance.FontData.Bold = DefaultableBoolean.Default;
                        }
                        else if (valueList.Key == "Transitions")
                        {
                            selectedColumn.Header.Caption = "Change";
                        }

                        selectedColumn.Width =
                            selectedColumn.CalculateAutoResizeWidth(PerformAutoSizeType.VisibleRows, true);
                    }
                    else
                    {
                        valueList.DisplayStyle = ValueListDisplayStyle.Picture;
                        selectedColumn.Width = 24;
                        selectedColumn.Header.Appearance.TextHAlign = HAlign.Center;
                        selectedColumn.CellAppearance.TextHAlign = HAlign.Center;
                        selectedColumn.CellAppearance.ImageHAlign = HAlign.Center;
                        if (valueList.Key == "Severity")
                        {
                            selectedColumn.Header.Caption = "!";
                            selectedColumn.Header.Appearance.ForeColor = Color.Red;
                            selectedColumn.Header.Appearance.FontData.Bold = DefaultableBoolean.True;
                        }
                        else if (valueList.Key == "Transitions")
                        {
                            selectedColumn.Header.Caption = DELTA_STRING;
                        }
                    }
                }
            }
        }

        private void PrintGrid()
        {
            MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[instanceId];
            string serverName = instance != null ? instance.InstanceName + " - " : string.Empty;
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{1}Active Alerts as of {0}",
                              DateTime.Now.ToString("G"),
                              serverName
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            ValueListDisplayStyle severityStyle =
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle;
            ValueListDisplayStyle changeStyle =
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "Alerts";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ultraGridExcelExporter.Export(alertsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                    severityStyle;
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle =
                    changeStyle;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    alertsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            alertsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            alertsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(alertsGrid);
            dialog.Show(this);
        }

        #endregion

        private void showRealTimeViewLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UltraGridRow row = alertsGrid.Selected.Rows[0];
            UltraDataRow dataRow = (UltraDataRow) row.ListObject;

            Metric m = MetricDefinition.GetMetric((int) row.Cells["Metric"].Value);
            switch (m)
            {
                case Metric.Deadlock:
                    ShowDeadlockDetails();
                    break;
                default:
                    NavigateToView(row, false);
                    break;
            }
        }

        private void helpHistoryLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (showHistoricalView)
            {
                NavigateToView(alertsGrid.Selected.Rows[0], true);
            }
            else
            {
                ShowAlertHelp();
            }
        }

        private void helpHistoryLinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAlertHelp();
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        private void alertsGrid_AfterRowRegionSize(object sender, RowScrollRegionEventArgs e)
        {
            if (DetailsPanelVisible && alertsGrid.Selected.Rows.Count > 0 && e.RowScrollRegion.IsActiveScrollRegion)
            {
                UltraGridRow selectedRow = alertsGrid.Selected.Rows[0];
                e.RowScrollRegion.ScrollRowIntoView(selectedRow);

            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this,AutoScaleFontHelper.ControlType.Container);
        }

        private void showDetailsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UltraGridRow row = alertsGrid.Selected.Rows[0];
            //UltraDataRow dataRow = (UltraDataRow)row.ListObject;

            Metric m = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
            switch (m)
            {
                case Metric.BlockingAlert:
                    ShowBlockReport();
                    break;
                default:
                    NavigateToView(row, false);
                    break;
            }
        }

    }
    [Serializable]
    internal enum ActiveServerAlertViews
    {
        ActiveServerAlerts
    }
}
