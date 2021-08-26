using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using ColumnHeader=Infragistics.Win.UltraWinGrid.ColumnHeader;

namespace Idera.SQLdm.DesktopClient.Views
{
    using Idera.SQLdm.Common.Configuration;
    using Infragistics.Windows.Themes;
    using System.Data;

    internal partial class SQLdmTodayView : View
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SQLdmTodayView");

        private const string CriticalServersLabelText = "{0} server(s) in Critical state";
        private const string WarningServersLabelText = "{0} server(s) in Warning state";
        private const string MaintenanceModeServersLabelText = "{0} server(s) in Maintenance Mode";
        private const string OkServersLabelText = "{0} server(s) in OK state";

        private const char DELTA_CHAR = (char) 0x394;

        private Control focused;
        private bool showingTaskDialog = false;
        private object sync = new object();

        private int maintenanceModeServers;
        private int criticalServers;
        private int warningServers;
        private int okServers;

        private UltraGrid selectedGrid = null;
        private UltraGridColumn selectedColumn = null;

        private HelpPopupDialog helpPopupDialog;

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastAlertsGridSettings = null;
        private GridSettings lastTasksGridSettings = null;
        private int selectedTabIndex = 0;

        public SQLdmTodayView()
        {
            InitializeComponent();
            HideFocusRectangleDrawFilter drawFilter = new HideFocusRectangleDrawFilter();
            alertsGrid.DrawFilter = drawFilter;
            tasksGrid.DrawFilter = drawFilter;

            alertsGrid.Tag = recentAlertsHeaderLabel;

            ApplicationModel.Default.ActiveInstances.Changed +=
                new EventHandler<MonitoredSqlServerCollectionChangedEventArgs>(ActiveServers_Changed);

            ApplicationController.Default.BackgroundRefreshCompleted +=
                new EventHandler<BackgroundRefreshCompleteEventArgs>(BackgroundRefreshCompleted);

            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.tasksGrid);
            themeManager.updateGridTheme(this.alertsGrid);
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SQLdmTodayView);
        }

        public override void ApplySettings()
        {
            lastSplitterDistance =
                contentSplitter.SplitterDistance = Settings.Default.TodayViewMainSplitter;
            if (Settings.Default.TodayViewAlertsGrid is GridSettings)
            {
                lastAlertsGridSettings = Settings.Default.TodayViewAlertsGrid;
                GridSettings.ApplySettingsToGrid(lastAlertsGridSettings, alertsGrid);
            }

            if (Settings.Default.TodayViewTasksGrid is GridSettings)
            {
                lastTasksGridSettings = Settings.Default.TodayViewTasksGrid;
                GridSettings.ApplySettingsToGrid(lastTasksGridSettings, tasksGrid);
            }

            int selectedTabIndex = Settings.Default.TodayViewSelectedTabIndex;
            if(selectedTabIndex < tasksForecastTabGroup.Tabs.Count)
                tasksForecastTabGroup.Tabs[selectedTabIndex].Selected = true;
        }

        public override void SaveSettings()
        {
            GridSettings alertsGridSettings = GridSettings.GetSettings(alertsGrid);
            GridSettings tasksGridSettings = GridSettings.GetSettings(tasksGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != contentSplitter.SplitterDistance
                || !alertsGridSettings.Equals(lastAlertsGridSettings)
                || !tasksGridSettings.Equals(lastTasksGridSettings))
            {
                lastSplitterDistance =
                    Settings.Default.TodayViewMainSplitter = contentSplitter.SplitterDistance;
                lastAlertsGridSettings =
                    Settings.Default.TodayViewAlertsGrid = alertsGridSettings;
                lastTasksGridSettings =
                    Settings.Default.TodayViewTasksGrid = tasksGridSettings;
            }

            Settings.Default.TodayViewSelectedTabIndex = selectedTabIndex;
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            UpdateStatusArea();
        }

        public override void UpdateUserTokenAttributes()
        {
            UpdateStatusArea();
            addServersFeatureButton.Enabled = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
        }

        private void ActiveServers_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            RefreshView();
        }

        public override void RefreshView()
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "TodayViewRefresh";
            }

            using (LOG.DebugCall("DoRefreshWork"))
            {
                if (!alertsGrid.IsDisposed)
                {
                    ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];

                    if (valueList.ValueListItems.Count == 0)
                    {
                        valueList.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());
                    }
                }

                LoadAlerts();
                LoadTasks();
                LoadForecasts();

                return alertsViewDataSource;
            }
        }

        private void LoadAlerts()
        {
            using (LOG.DebugCall("LoadAlerts"))
            {
                DateTime start = DateTime.Now;
                alertsViewDataSource.SuspendBindingNotifications();
                try
                {
                    RepositoryHelper
                        .LoadDataSource(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, alertsViewDataSource,
                                        "p_GetAlerts",
                                        0, null, null, ApplicationModel.Default.UserToken.ActiveAssignedServersXmlFilter, null, null, null, null,null, true, null);
                }
                finally
                {
                    alertsViewDataSource.ResumeBindingNotifications();
                }

                TimeSpan span = DateTime.Now - start;
                LOG.Debug("RepositoryHelper.LoadDataSource[Alerts] took this long: ", span);
            }
        }

        private void LoadTasks()
        {
            using (LOG.DebugCall("LoadTasks"))
            {
                lock (sync)
                {
                    // if currently editing a task then skip the refresh
                    if (!showingTaskDialog)
                    {
                        DateTime start = DateTime.Now;

                        tasksGridDataSource.SuspendBindingNotifications();
                        try
                        {
                            RepositoryHelper.LoadDataSource(
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                tasksGridDataSource,
                                "p_GetTasks",
                                DateTime.UtcNow.AddDays(-5.0d),
                                TaskStatus.AnyAll,
                                MonitoredStateFlags.All,
                                ApplicationModel.Default.UserToken.ActiveAssignedServersXmlFilter,
                                null);
                        }
                        finally
                        {
                            tasksGridDataSource.ResumeBindingNotifications();
                        }
                        TimeSpan span = DateTime.Now - start;
                        LOG.Debug("RepositoryHelper.LoadDataSource[Tasks] took this long: ", span);
                    }
                }
            }
        }

        private void LoadForecasts()
        {
            using (LOG.DebugCall("LoadForecasts"))
            {
                DataTable table = null;
                DateTime  start = DateTime.Now;

                try
                {
                    table = RepositoryHelper.GetServerForecasts(-1);                    
                }
                catch(Exception e)
                {
                    LOG.Error("Caught exception loading forecasts for today view.", e);
                }

                TimeSpan span = DateTime.Now - start;
                LOG.Debug("RepositoryHelper.LoadForecasts() took this long: ", span);

                alertForecastPanel1.UpdateForecastData(table);
            }
        }

        public override void UpdateData(object data)
        {
            DateTime lastRefresh = DateTime.Now;
            refreshDateLabel.Text = lastRefresh.ToString("dddd, MMMM d, yyyy");
            refreshTimeLabel.Text = lastRefresh.ToString("h:mm tt");

            UpdateStatusArea();

            statusSummaryContentPanel.Visible = true;

            // hide/show alerts grid 
            if (alertsViewDataSource.Rows.Count == 0)
                alertsStatusLabel.BringToFront();
            else
                alertsGrid.BringToFront();

            // hide/show tasks grid 
            if (tasksGridDataSource.Rows.Count == 0)
                tasksStatusLabel.BringToFront();
            else
                tasksGrid.BringToFront();

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                new RefreshActiveViewCompletedEventArgs(lastRefresh));
        }

        private void UpdateStatusArea()
        {
            ApplicationModel model = ApplicationModel.Default;
            statusSummaryLabel.Enabled = true;
            statusSummaryDescriptionLabel.Enabled = true;

            if (model.ActiveInstances.Count == 0)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                {
                    statusSummaryLabel.Text = "No Actively Monitored Servers";
                    statusSummaryDescriptionLabel.Text = "Click here to add servers now.";
                }
                else
                {
                    statusSummaryLabel.Text = "No Monitored Servers Available";
                    statusSummaryLabel.Enabled = false;
                    statusSummaryDescriptionLabel.Text = "You do not have access to monitor any servers at this time.";
                    statusSummaryDescriptionLabel.Enabled = false;
                }

                statusSummaryPictureBox.Image =
                    Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                statusSummaryDescriptionLabel.LinkColor = Color.FromArgb(0, 0, 255);
                criticalServersPictureBox.Visible = false;
                criticalServersLabel.Visible = false;
                warningServersPictureBox.Visible = false;
                warningServersLabel.Visible = false;
                maintenanceModeServersPictureBox.Visible = false;
                maintenanceModeServersLabel.Visible = false;
                okServersPictureBox.Visible = false;
                okServersLabel.Visible = false;
                return;
            }

            maintenanceModeServers = 0;
            criticalServers = 0;
            warningServers = 0;
            okServers = 0;

            foreach (MonitoredSqlServerWrapper instance in Algorithms.ToArray(model.ActiveInstances))
            {
                MonitoredSqlServerStatus status = model.GetInstanceStatus(instance.Id);
                if (status != null)
                {
                    if (status.IsInMaintenanceMode)
                        maintenanceModeServers++;
                    else
                    {
                        switch (status.Severity)
                        {
                            case MonitoredState.Critical:
                                criticalServers++;
                                break;
                            case MonitoredState.Warning:
                                warningServers++;
                                break;
                            default:
                                okServers++;
                                break;
                        }
                    }
                }
                else
                {
                    if (instance.MaintenanceModeEnabled)
                        maintenanceModeServers++;
                    else
                        okServers++;
                }
            }
            if (criticalServers > 0)
            {
                statusSummaryPictureBox.Image =
                    Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryCriticalLarge;
                statusSummaryLabel.Text = "Critical";
                statusSummaryLabel.LinkColor = Color.Red;
                statusSummaryDescriptionLabel.Text =
                    String.Format(CriticalServersLabelText,
                                  criticalServers);
                statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
            }
            else if (warningServers > 0)
            {
                statusSummaryPictureBox.Image = Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryWarningLarge;
                statusSummaryLabel.Text = "Warning";
                statusSummaryLabel.LinkColor = Color.Black;
                statusSummaryDescriptionLabel.Text =
                    String.Format(WarningServersLabelText,
                                  warningServers);
                statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
            }
            else if (okServers == 0 && maintenanceModeServers > 0)
            {
                statusSummaryPictureBox.Image =
                    Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                statusSummaryLabel.Text = "Maintenance Mode";
                statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                statusSummaryDescriptionLabel.Text = "All servers are in maintenance mode.";
                statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
            }
            else if (okServers == 0)
            {
                statusSummaryPictureBox.Image =
                    Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                statusSummaryLabel.Text = "No Instances";
                statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                statusSummaryDescriptionLabel.Text = "There are no SQL Server instances being monitored.";
                statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
            }
            else
            {
                statusSummaryPictureBox.Image = Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryOKLarge;
                statusSummaryLabel.Text = "OK";
                statusSummaryLabel.LinkColor = Color.ForestGreen;
                statusSummaryDescriptionLabel.Text = "All servers are operating normally.";
                statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
            }

            criticalServersPictureBox.Visible = true;
            criticalServersLabel.Text = string.Format(CriticalServersLabelText, criticalServers);
            criticalServersLabel.Visible = true;
            warningServersPictureBox.Visible = true;
            warningServersLabel.Text = string.Format(WarningServersLabelText, warningServers);
            warningServersLabel.Visible = true;
            maintenanceModeServersPictureBox.Visible = true;
            maintenanceModeServersLabel.Text = string.Format(MaintenanceModeServersLabelText, maintenanceModeServers);
            maintenanceModeServersLabel.Visible = true;
            okServersPictureBox.Visible = true;
            okServersLabel.Text = string.Format(OkServersLabelText, okServers);
            okServersLabel.Visible = true;
        }

        private bool UpdateTask(UltraDataRow row)
        {
            bool result = true;
            using (LOG.DebugCall("UpdateTask"))
            {
                try
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    int taskId = (int) row.GetCellValue("TaskID", true);
                    string owner = row.GetCellValue("Owner", true) as string;
                    string comments = row.GetCellValue("Comments", true) as string;
                    byte statusValue = (byte) row.GetCellValue("Status", true);
                    TaskStatus newStatus = (TaskStatus) Enum.ToObject(typeof (TaskStatus), statusValue);

                    defaultManagementService.UpdateTask(taskId, newStatus, owner, comments);
                }
                catch (Exception e)
                {
                    result = false;
                    ApplicationMessageBox.ShowError(ParentForm,
                                                    "There was a problem upating the status of the selected task.", e);
                }
                return result;
            }
        }

        private void addServersFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            ShowManageServersDialog();
        }

        private void ShowManageServersDialog()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                ManageServersDialog manageServersDialog = new ManageServersDialog();
                manageServersDialog.ShowDialog(ParentForm);
            }
        }

        private void monitorAlertLogFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void manageTasksFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void generateReportsFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void SQLdmTodayView_Load(object sender, EventArgs e)
        {
            ConfigureAlertsGrid();

            if (Settings.Default.HideTodayViewCommonTasks)
            {
                HideCommonTasks();
            }
            else
            {
                ShowCommonTasks();
            }

            ApplySettings();
            RefreshView();
        }

        private void toggleCommonTasksButton_Click(object sender, EventArgs e)
        {
            ToggleCommonTasksVisble();
        }

        private void ToggleCommonTasksVisble()
        {
            Settings.Default.HideTodayViewCommonTasks = !Settings.Default.HideTodayViewCommonTasks;

            if (Settings.Default.HideTodayViewCommonTasks)
            {
                HideCommonTasks();
            }
            else
            {
                ShowCommonTasks();
            }
        }

        private void ShowCommonTasks()
        {
            commonTasksPanel.Height = 140;
            toggleCommonTasksButton.Image = Idera.SQLdm.DesktopClient.Properties.Resources.DownArrows;
            toggleCommonTasksButton.Text = "Hide Common Tasks";
        }

        private void HideCommonTasks()
        {
            commonTasksPanel.Height = 25;
            toggleCommonTasksButton.Image = Idera.SQLdm.DesktopClient.Properties.Resources.UpArrows;
            toggleCommonTasksButton.Text = "Show Common Tasks";
        }

        private void ConfigureAlertsGrid()
        {
            alertsGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;
            tasksGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

            // switch out editor from EditorWithCombo to EditorWithText (fixes tooltip issue)
            EditorWithText textEditor = new EditorWithText();
            tasksGrid.DisplayLayout.Bands[0].Columns["Severity"].Editor = textEditor;
            tasksGrid.DisplayLayout.Bands[0].Columns["Status"].Editor = textEditor;

            // switch out editor from EditorWithCombo to EditorWithText (fixes tooltip issue)
            textEditor = new EditorWithText();

            UltraGridColumn column = alertsGrid.DisplayLayout.Bands[0].Columns["Severity"];
            column.Editor = textEditor;
            column.Width = 24;
            // set the header to a bold red ! and center it
            column.Header.Caption = "!";
            column.Header.Appearance.TextHAlign = HAlign.Center;
            column.Header.Appearance.ForeColor = Color.Red;
            column.Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            // center the images in the cells
            column.CellAppearance.ImageHAlign = HAlign.Center;

            column = alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"];
            column.Editor = textEditor;
            //column.Width = 24;
            // set the header to a delta character and center it
            //column.Header.Caption = DELTA_STRING;
            //column.Header.Appearance.TextHAlign = HAlign.Center;
            // center the images in the cells
            //column.CellAppearance.ImageHAlign = HAlign.Center;

            // configure the sort order for the grids
            UltraGridBand band = tasksGrid.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add("CreatedOn", true);

            band = alertsGrid.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add("UTCOccurrenceDateTime", true);
            band.SortedColumns.Add("Severity", true);
            band.SortedColumns.Add("ServerName", false);

            // build the task status value list
            ValueList valueList = tasksGrid.DisplayLayout.ValueLists["Status"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTaskStatusValueListItems(false));

            // build the severity value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetSeverityValueListItems());
            // copy the value list to the tasksGrid
            ValueListHelpers.CopyValueList(valueList, tasksGrid.DisplayLayout.ValueLists["Severity"]);

            // load the transition value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTransitionValueListItems());

            // populate the metric value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
            valueList.ValueListItems.Clear();
        }

        private void commonTasksHeaderStrip_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToggleCommonTasksVisble();
        }

        #region toolbars

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

                    object metricColumnValue = alertsGrid.Selected.Rows[0].Cells["Metric"].Value;
                    if (metricColumnValue != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                        if (m == Metric.Deadlock)
                        {
                            isViewDeadlockDetailsButtonEnabled = true;
                            isViewRealTimeSnapshotButtonEnabled = false;
                        }
                        else
                        {
                            isViewRealTimeSnapshotButtonEnabled = m != Metric.Operational && m != Metric.SQLdmCollectionServiceStatus;
                        }

                        isViewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                        isViewAlertHelpButtonEnabled = isViewRealTimeSnapshotButtonEnabled && m != Metric.Custom;
                    }

                    ((PopupMenuTool) e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible =
                        isViewDeadlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertRealTimeSnapshotButton"].SharedProps.Visible =
                        isViewRealTimeSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHistoricalSnapshotButton"].SharedProps.Visible =
                        isViewHistoricalSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible =
                        isViewAlertHelpButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHistoricalSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible = false;
                }
            }
            else if (e.Tool.Key == "taskGridContextMenu")
            {
                if (tasksGrid.Selected.Rows.Count > 0)
                {
                    toolbarsManager.Tools["deleteButton"].SharedProps.Visible =
                        ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

                    ((PopupMenuTool)e.Tool).Tools["viewTaskRealTimeSnapshotButton"].SharedProps.Visible = true;

                    bool isViewHistoricalSnapshotButtonEnabled = false;

                    object metricColumnValue = tasksGrid.Selected.Rows[0].Cells["Metric"].Value;
                    if (metricColumnValue != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)tasksGrid.Selected.Rows[0].Cells["Metric"].Value);
                        isViewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                    }

                    ((PopupMenuTool)e.Tool).Tools["viewTaskHistoricalSnapshotButton"].SharedProps.Visible =
                        isViewHistoricalSnapshotButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["viewTaskRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewTaskHistoricalSnapshotButton"].SharedProps.Visible = false;
                }
            }
        }

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
                    GroupBySelectedColumn(((StateButtonTool) e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "configureAlertsButton":
                    ConfigureAlerts();
                    break;
                case "snoozeAlertButton":
                    SnoozeAlert();
                    break;

                // Tasks grid specific items - may need to add code to figure out the
                // sender if needed down the road.
                case "notStartedButton":
                    SetTaskStatus(tasksGrid.Selected.Rows[0], TaskStatus.NotStarted);
                    break;
                case "inProgressButton":
                    SetTaskStatus(tasksGrid.Selected.Rows[0], TaskStatus.InProgress);
                    break;
                case "onHoldButton":
                    SetTaskStatus(tasksGrid.Selected.Rows[0], TaskStatus.OnHold);
                    break;
                case "completedButton":
                    SetTaskStatus(tasksGrid.Selected.Rows[0], TaskStatus.Completed);
                    break;
                case "deleteButton":
                    DeleteSelectedTasks();
                    break;
                case "showAssociatedViewButton":
                    NavigateToView(tasksGrid.Selected.Rows[0], false);
                    break;
                case "propertiesButton":
                    ShowTaskProperties();
                    break;
                case "copyToClipboardButton":
                    UltraGridHelper.CopyToClipboard(selectedGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "clearAlertButton":
                    ClearAlert(false);
                    break;
                case "clearAllAlertsButton":
                    ClearAlert(true);
                    break;
                case "viewAlertRealTimeSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], false);
                    break;
                case "viewAlertHistoricalSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], true);
                    break;
                case "viewAlertHelpButton":
                    ShowAlertHelp();
                    break;
                case "viewTaskRealTimeSnapshotButton":
                    NavigateToView(tasksGrid.Selected.Rows[0], false);
                    break;
                case "viewTaskHistoricalSnapshotButton":
                    NavigateToView(tasksGrid.Selected.Rows[0], true);
                    break;
                case "viewDeadlockDetailsButton":
                    ShowDeadlockDetails();
                    break;
            }
        }

        private void ShowDeadlockDetails()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.", e);
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
                    MonitoredSqlServerWrapper serverWrapper = ApplicationModel.Default.ActiveInstances[serverName];
                    Metric metric = MetricDefinition.GetMetric(metricId);

                    // If server and metric are valid, then proceed, else do nothing.
                    if (serverWrapper != null && metric != Metric.Custom)
                    {
                        // Get alert help dialog parameters based on the instance id & metric,
                        // viz. help topic, default view, default view argument.
                        string helpTopic;
                        ServerViews view;
                        Object viewArg;
                        if (ClickThroughHelper.GetMetricHelp(metric, out helpTopic)
                            && ClickThroughHelper.GetMetricView(serverWrapper.Id, metric, out view, out viewArg))
                        {
                            if (helpPopupDialog == null)
                            {
                                helpPopupDialog = new HelpPopupDialog(serverWrapper.Id);
                            }

                            if (helpPopupDialog.Visible)
                            {
                                helpPopupDialog.Hide();
                            }

                            helpPopupDialog.HelpTopic = helpTopic;
                            helpPopupDialog.DetailsView = view;
                            helpPopupDialog.DetailsViewArgument = viewArg;
                            helpPopupDialog.Metric = metric;
                            helpPopupDialog.ConfigureAlertsOptionVisible = true;// configureAlertsOptionVisible;
                            helpPopupDialog.SetStartPosition(Cursor.Position);
                            helpPopupDialog.Show(this);
                        }
                    }
                }
            }
        }

        private void SnoozeAlert()
        {
            if (alertsGrid.Selected.Rows.Count == 1)
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

                    // Get Alert Summary description
                    String alertDescription = dataRow["Heading"] as String;

                    // Displays snooze dialog.
                    SnoozeInfo snoozeInfo = SnoozeAlertsDialog.SnoozeAlert(
                        ParentForm, 
                        selectedServer.Id, 
                        (int) metric,
                        SnoozeAlertsDialog.SnoozeAction.Snooze,
                        alertDescription);
                    
                    if (snoozeInfo != null)
                    {
                        dataRow.ParentCollection.Remove(dataRow);
                    }
                }
            }
        }

        #endregion

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

                                if (allAlerts)
                                {
                                    string selectedServer = dataRow["ServerName"] as string ?? "";

                                    List<UltraDataRow> rows = new List<UltraDataRow>();
                                    foreach (UltraDataRow udr in dataRow.ParentCollection)
                                    {
                                        if (((bool) udr["Active"]) && metric.Equals(udr["Metric"]))
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

        private void ConfigureAlerts()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow) row.ListObject;
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
                                    new AlertConfigurationDialog(wrapper.Id, false))
                            {
                                object metric = dataRow["Metric"];
                                if (metric is int)
                                    metric = (Metric) Enum.ToObject(typeof (Metric), ((int) metric));

                                alertConfigDialog.CreateControl();
                                if (metric is Metric)
                                {
                                    alertConfigDialog.Select((Metric) metric);
                                }

                                alertConfigDialog.ShowDialog(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }
                    }
                }
            }
        }

        

        #region grid

        private void PrintGrid()
        {
            if (selectedGrid != null)
            {
                ultraGridPrintDocument.Grid = selectedGrid;
                ultraGridPrintDocument.DefaultPageSettings.Landscape = false;
                ultraGridPrintDocument.Header.TextLeft = ultraGridPrintDocument.Header.TextLeft =
                                                         string.Format("{0} as of {1}",
                                                                       ((HeaderLabel) selectedGrid.Tag).HeaderText,
                                                                       DateTime.Now.ToString("G")
                                                             );

                ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

                ultraPrintPreviewDialog.ShowDialog();
            }
        }

        private void SaveGrid()
        {
            if (selectedGrid != null)
            {
                string severityCol = "Severity";
                string statusCol = "Status";
                if (selectedGrid == alertsGrid)
                {
                    statusCol = "StateEvent";
                }
                ValueListDisplayStyle severityStyle =
                    ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle;
                ValueListDisplayStyle statusStyle =
                    ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle;

                saveFileDialog.FileName =
                    ExportHelper.GetValidFileName(((HeaderLabel) selectedGrid.Tag).HeaderText, true);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle =
                            ValueListDisplayStyle.DisplayText;
                        ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle =
                            ValueListDisplayStyle.DisplayText;
                        ultraGridExcelExporter.Export(selectedGrid, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                    }

                    ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle =
                        severityStyle;
                    ((ValueList) selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle =
                        statusStyle;
                }
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            if (selectedGrid != null)
            {
                selectedGrid.DisplayLayout.GroupByBox.Hidden = !selectedGrid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                if (GroupBy)
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void ShowColumnChooser()
        {
            if (selectedGrid != null)
            {
                SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(selectedGrid);
                dialog.Show(this);
            }
        }

        #endregion

        private void tasksGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UIElement selectedElement = tasksGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (!(selectedElement is CheckIndicatorUIElement))
                    return;

                object contextObject = selectedElement.GetContext();
                if (contextObject is Infragistics.Win.UltraWinGrid.UltraGridColumn)
                {
                    if (((UltraGridColumn)contextObject).Key == "StatusBoolean")
                    {
                        UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;
                        if (selectedRow != null)
                        {
                            UltraDataRow dataRow = selectedRow.ListObject as UltraDataRow;
                            object statusValue = dataRow.GetCellValue("Status", true);
                            if (statusValue.GetType().IsPrimitive)
                            {
                                TaskStatus newStatus = TaskStatus.Completed;
                                byte byteValue = (byte)statusValue;
                                if (byteValue == (byte)TaskStatus.Completed)
                                    newStatus = TaskStatus.NotStarted;

                                SetTaskStatus(selectedRow,newStatus);
                            }
                        }
                    }
                }
            }
        }

        private void tasksGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = (UltraGrid) sender;
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof (ColumnHeader));

                if (!(contextObject is ColumnHeader))
                {
                    contextObject = selectedElement.GetAncestor(typeof (RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "taskGridContextMenu");
                        toolbarsManager.Tools["copyToClipboardButton"].SharedProps.Visible = true;
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), null);
                    }
                }
            }
        }

        private void tasksGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs args)
        {
            ShowTaskProperties();
        }

        private void ShowTaskProperties()
        {
            showingTaskDialog = true;

            // Get the currently selected row.
            bool isUpdated = false;
            UltraGridRow row = tasksGrid.Selected.Rows[0];
            if (row != null && row.IsDataRow)
            {
                ToDoPropertiesDialog dlg = new ToDoPropertiesDialog(row);
                DialogResult rc = dlg.ShowDialog();
                if (rc == DialogResult.OK)
                {
                    // Write new settings out to the MS.
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    defaultManagementService.UpdateTask(dlg.TaskId, dlg.TaskStatus, dlg.TaskOwner, dlg.TaskComments);
                    isUpdated = true;
                }
            }

            showingTaskDialog = false;
            if (isUpdated)
            {
                LoadTasks();
            }
        }

        private void DeleteSelectedTasks()
        {
            List<int> IDs = new List<int>();

            // Build a list of the task Ids and the corresponding rows in the DataTable.
            foreach (UltraGridRow row in tasksGrid.Selected.Rows)
            {
                IDs.Add((int)row.Cells["TaskID"].Value);
            }

            // Delete the tasks in the DB.
            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            defaultManagementService.DeleteTask(IDs);

            // This triggers the SelectionChanged event.
            tasksGrid.Selected.Rows.Clear();

            // Reload the tasks.
            LoadTasks();
        }

        private void SetTaskStatus(UltraGridRow selectedRow, TaskStatus newStatus)
        {
            UltraDataRow dataRow = selectedRow.ListObject as UltraDataRow;

            bool commitChanges = false;
            IEditableObject editableRow = dataRow as IEditableObject;
            editableRow.BeginEdit();
            try
            {
                // update the current row with the new value w/ no events
                dataRow.SetCellValue("Status", (byte) newStatus, false);
                // update the management service using the current row values
                commitChanges = UpdateTask(dataRow);

                // update the value again but raise the update event
                dataRow.SetCellValue("Status", (byte) newStatus, true);
                // force retrieval of the checkbox value (derived field)
                dataRow.GetCellValue("StatusBoolean", true);
            }
            finally
            {
                if (commitChanges)
                    editableRow.EndEdit();
                else
                    editableRow.CancelEdit();
            }
        }

        private void contentSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void contentSplitter_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
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

        private void alertsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = (UltraGrid) sender;
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof (ColumnHeader));

                if (!(contextObject is ColumnHeader))
                {
                    contextObject = selectedElement.GetAncestor(typeof (RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        // the user clicked a row
                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "gridContextMenu");

                        RowUIElement rowElement = contextObject as RowUIElement;
                        UltraGridRow row = rowElement.Row;

                        // select the row that was clicked
                        alertsGrid.Selected.Rows.Clear();
                        row.Activate();
                        row.Selected = true;

                        // set the alert configuration edit button enabled if the selected row has a server name
                        UltraDataRow dataRow = (UltraDataRow) row.ListObject;
                        object server = dataRow["ServerName"];
                        object metric = dataRow["Metric"];
                        ButtonTool tool = (ButtonTool)toolbarsManager.Tools["configureAlertsButton"];
                        tool.SharedProps.Visible = (server != null && !"".Equals(server.ToString()));

                        bool enableSnoozeButton = false;
                        if (alertsGrid.Selected.Rows.Count == 1)
                        {
                            if (server != null && !"".Equals(server.ToString()))
                            {
                                //Operator Security Role Changes - 10.3
                                int id = ApplicationModel.Default.ActiveInstances[server as string].Id;
                                if (metric is int && !((int)metric == (int)Metric.MaintenanceMode || (int)metric == (int)Metric.Operational))
                                    enableSnoozeButton = ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify|| ApplicationModel.Default.UserToken.GetServerPermission(id) == PermissionType.ReadOnlyPlus;
                            }
                        }
                        tool = (ButtonTool)toolbarsManager.Tools["snoozeAlertButton"];
                        tool.SharedProps.Visible = enableSnoozeButton;
                        
                        int instanceId = ApplicationModel.Default.ActiveInstances[server as string].Id;
                        bool enableClearOptions = false;

                        if (alertsGrid.Selected.Rows.Count > 0)
                        {
                            object active = dataRow["Active"];
                            if (active is bool && ((bool) active))
                            {
                                if (metric is int)
                                {
                                    Metric m = (Metric) Enum.ToObject(typeof (Metric), (int) metric);
                                    switch (m)
                                    {
                                        case Metric.ReorganisationPct:
                                        case Metric.BombedJobs:
                                        case Metric.LongJobs:
                                        case Metric.LongJobsMinutes:
                                        case Metric.ClusterFailover:
                                        case Metric.JobCompletion:
                                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                                            enableClearOptions = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                                            break;
                                    }
                                }
                            }
                        }
                        toolbarsManager.Tools["clearAlertButton"].SharedProps.Visible = enableClearOptions;
                        toolbarsManager.Tools["clearAllAlertsButton"].SharedProps.Visible = enableClearOptions;
                    }
                    else
                    {
                        // if the user clicked in the open area of a grid (after the last row) then there is no menu
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), null);
                    }
                }
            }
        }

        private void NavigateToView(UltraGridRow row, bool showHistoricalSnapshot)
        {
            if (row != null)
            {
                string instanceName = row.Cells["ServerName"].Value as string;

                if (row.Cells["Metric"].Value != null)
                {
                    Metric metric = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
                    if (showHistoricalSnapshot)
                    {
                        DateTime? snapshotDateTime = null;

                        if (row.Band.Layout.Grid == alertsGrid)
                        {
                            snapshotDateTime = (DateTime)row.Cells["UTCOccurrenceDateTime"].Value;
                        }
                        else if (row.Band.Layout.Grid == tasksGrid)
                        {
                            snapshotDateTime = (DateTime)row.Cells["CreatedOn"].Value;
                        }

                        ClickThroughHelper.NavigateToView(instanceName, metric, snapshotDateTime);
                    }
                    else
                    {
                        if (metric == Metric.Deadlock)
                        {
                            ShowDeadlockDetails();
                            return;
                        }

                        ClickThroughHelper.NavigateToView(instanceName, metric, null);
                    }
                }
            }
        }

        private void alertsGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            NavigateToView(e.Row, false);
        }

        private void ServersLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // if there are no instances defined and the user clicks a label - launch the add servers wizard
            if (ApplicationModel.Default.ActiveInstances.Count == 0)
            {
                ShowManageServersDialog();
                return;
            }

            Guid gotoViewID = Guid.Empty;

            // determine the user view and take the user there
            if (sender == criticalServersLabel)
                gotoViewID = SearchUserView.CriticalUserViewID;
            else if (sender == warningServersLabel)
                gotoViewID = SearchUserView.WarningUserViewID;
            else if (sender == okServersLabel)
                gotoViewID = SearchUserView.OKUserViewID;
            else if (sender == maintenanceModeServersLabel)
                gotoViewID = SearchUserView.MaintenanceModeUserViewID;
            else if (sender == statusSummaryLabel || sender == statusSummaryDescriptionLabel)
            {
                if (criticalServers > 0)
                    gotoViewID = SearchUserView.CriticalUserViewID;
                else if (warningServers > 0)
                    gotoViewID = SearchUserView.WarningUserViewID;
                else if (maintenanceModeServers > 0)
                    gotoViewID = SearchUserView.MaintenanceModeUserViewID;
                else if (okServers > 0)
                    gotoViewID = SearchUserView.OKUserViewID;
            }

            ApplicationController.Default.ShowUserView(gotoViewID);
        }

        private void tasksForecastTabGroup_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            selectedTabIndex = e.Tab.Index;
        }
    }

    #region Task Grid DataSource

    public class TaskGridDataSource : DataSourceWithID<int>, ISupportInitialize
    {
        public TaskGridDataSource(IContainer container) : base(container)
        {
        }

        public override void EndInit()
        {
            ConfigureColumns();
            base.EndInit();
        }

        private void ConfigureColumns()
        {
            this.Band.Columns["CreatedOn"].Tag = new DateConverter();
            this.Band.Columns["CompletedOn"].Tag = new DateConverter();
            this.Band.Columns["StatusBoolean"].Tag = new StatusToBoolValueConverter();
        }

        internal class StatusToBoolValueConverter : IValueConverter
        {
            private const string BasedOnColumn = "Status";

            public object ConvertData(UltraDataColumn column, UltraDataRow row)
            {
                UltraDataColumn sourceColumn = column.ParentCollection[BasedOnColumn];
                if (sourceColumn != null)
                {
                    byte value = (byte) row.GetCellValue(sourceColumn);
                    return (value == (byte) TaskStatus.Completed);
                }
                return false;
            }
        }
    }

    #endregion
}