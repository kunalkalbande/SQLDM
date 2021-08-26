using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using DatabaseStatus = Idera.SQLdm.Common.Snapshots.DatabaseStatus;
using Idera.SQLdm.Common.Events;
using System.Globalization;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesSummaryView : ServerBaseView, IDatabasesView
    {
        #region constants

        private const string AllDatabasesItemText = "< All Databases >";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string SELECT_DB = @"Select one or more databases.";
        private const string FORMAT_CAPACITY_CHARTTYPE = "Capacity Usage: {0}";
        private const string FORMAT_TRENDS_CHARTTYPE = "Recent Trends: {0}";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;
        private string selectedDatabaseFilter = null;
        private bool initialized = false;
        private DataTable currentDataTable;
        private DataView capacityChartDataSource;
        private DataTable historyDataTable;
        SortedList<string, string> selectedDatabases = new SortedList<string, string>();
        private List<string> selectedDatabaseArgument = new List<string>();
        private DatabaseSummaryConfiguration configuration;
        private DatabaseSummary currentSnapshot = null;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private int chartMouseX = 0;
        private int chartMouseY = 0;
        private bool databaseFilterInitialized = false;

        // get the version of the current instance from the snapshot or the app instance cache
        private decimal serverVersion
        {
            get
            {
                if (currentSnapshot != null)
                {
                    return currentSnapshot.ProductVersion.Major;
                }
                else
                {
                    MonitoredSqlServerStatus server = ApplicationModel.Default.GetInstanceStatus(instanceId);

                    if (server != null && server.InstanceVersion != null)
                    {
                        return server.InstanceVersion.Major;
                    }
                    else
                    {
                        // default to assuming it is 2005
                        return 9;
                    }
                }
            }
        }
        private bool showOnly2000Options
        {
            get
            {
                return serverVersion < 9;
            }
        }

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartsVisible = true;
        private string lastCapacityChartType = string.Empty;
        private string lastTrendsChartType = string.Empty;

        #endregion

        #region constructors

        public DatabasesSummaryView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(recentTrendsChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(capacityUsageChart, toolbarsManager);

            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            databasesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            databasesGrid.CreationFilter = new ToolTipItemCreationFilter(instanceId);
            
            configuration = new DatabaseSummaryConfiguration(instanceId);
            configuration.IncludeSystemDatabases = true;
            configuration.IncludeSummaryData = true;
 
            // load value lists for grid display
            ValueListItem listItem;

            //State
            databasesGrid.DisplayLayout.ValueLists["stateValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(MonitoredState.OK, "OK");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            databasesGrid.DisplayLayout.ValueLists["stateValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(MonitoredState.Informational, "Info");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusInfoSmall;
            databasesGrid.DisplayLayout.ValueLists["stateValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(MonitoredState.Warning, "Warning");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            databasesGrid.DisplayLayout.ValueLists["stateValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(MonitoredState.Critical, "Critical");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            databasesGrid.DisplayLayout.ValueLists["stateValueList"].ValueListItems.Add(listItem);

            //Autogrowth
            databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(AutogrowStatus.AutogrowOff, "Off");
            databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(AutogrowStatus.AutogrowOn, "On");
            databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(AutogrowStatus.Mixed, "Mixed");
            databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(AutogrowStatus.Unknown, "Unknown");
            databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"].ValueListItems.Add(listItem);

            // Recovery Model
            databasesGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(RecoveryModel.Full, "Full");
            databasesGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(RecoveryModel.Bulk_Logged, "Bulk-logged");
            databasesGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(RecoveryModel.Simple, "Simple");
            databasesGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);

            // Compatibility Level
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(6f, "SQL Server 6.0 (60)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(6.5f, "SQL Server 6.5 (65)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(7f, "SQL Server 7.0 (70)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(8f, "SQL Server 2000 (80)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(9f, "SQL Server 2005 (90)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(10f, "SQL Server 2008 (100)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(10.5f, "SQL Server 2008 R2 (105)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(11f, "SQL Server 2012 (110)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(12f, "SQL Server 2014 (120)");
            databasesGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);

            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING+" >");
            databasesGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            databasesGrid.Visible = false;

            InitializeCurrentDataTable();
            InitializeHistoryDataTable();
            InitializeCharts();
            AdaptFontSize();

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }

        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler ChartVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public string SelectedDatabaseFilter
        {
            get { return selectedDatabaseFilter; }
        }

        /// <summary>
        /// Get or Set the Chart visibility and trigger state update event if changed
        /// </summary>
        public bool ChartVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;
                RestoreChart();

                if (ChartVisibleChanged != null)
                {
                    ChartVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Sessions Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !databasesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                databasesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public DatabaseSummaryConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods

        public override void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesSummaryView);
        }

        public override void SetArgument(object argument)
        {
            selectedDatabaseArgument = null;
            selectedDatabaseFilter = null;

            if (argument is string)
            {
                selectedDatabaseFilter = argument as string;
            }
            else if (argument is MonitoredObjectName && ((MonitoredObjectName)argument).IsDatabase)
            {
                selectedDatabaseFilter = ((MonitoredObjectName) argument).DatabaseName;
            }
            else if (argument is IList<string>)
            {
                IList<string> selection = argument as IList<string>;

                if (selection.Count == 1)
                {
                    selectedDatabaseFilter = selection[0];
                }
                else
                {
                    selectedDatabaseArgument = new List<string>(selection);
                }
            }

            if (selectedDatabaseFilter == null && databasesFilterComboBox.Items.Count > 0)
            {
                databasesFilterComboBox.SelectedIndex = 0;
            }
            else if (selectedDatabaseFilter != null && databasesFilterComboBox.Items.Count > 0)
            {
                databasesFilterComboBox.SelectedIndex = FindDatabaseIndex(selectedDatabaseFilter);
                if (databasesFilterComboBox.SelectedIndex == 0)
                    databaseFilterInitialized = false;
            }
            else if (selectedDatabaseArgument != null)
            {
                selectedDatabases = new SortedList<string, string>();
                databasesGrid.Selected.Rows.Clear();
                foreach (string database in selectedDatabaseArgument)
                {
                    foreach (UltraGridRow row in databasesGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((string)row.Cells["Database Name"].Value == database)
                        {
                            selectedDatabases.Add(database, database);
                            row.Selected = true;
                            databasesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            break;
                        }
                    }
                }
                if (selectedDatabases.Count > 0)
                {
                    selectedDatabaseArgument = null;
                }
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.DatabasesSummaryViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            if (Settings.Default.DatabasesSummaryViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.DatabasesSummaryViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, databasesGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            string chartType = Settings.Default.DatabasesSummaryViewCapacityChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in capacityUsageChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureCapacityUsageChart(chartType);
                        lastCapacityChartType = chartType;
                        break;
                    }
                }
            }

            chartType = Settings.Default.DatabasesSummaryViewTrendsChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in recentTrendsChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureRecentTrendsChart(chartType);
                        lastTrendsChartType = chartType;
                        break;
                    }
                }
            }

            lastChartsVisible =
                ChartVisible = Settings.Default.DatabasesSummaryViewChartsVisible;
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(databasesGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || !mainGridSettings.Equals(lastMainGridSettings)
                || string.Format(FORMAT_CAPACITY_CHARTTYPE, lastCapacityChartType) != capacityUsageChartSelectionDropDownButton.Text
                || string.Format(FORMAT_TRENDS_CHARTTYPE, lastTrendsChartType) != recentTrendsChartSelectionDropDownButton.Text
                || lastChartsVisible != ChartVisible)
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.DatabasesSummaryViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.DatabasesSummaryViewMainGrid = mainGridSettings;
                lastCapacityChartType =
                    Settings.Default.DatabasesSummaryViewCapacityChartType = capacityUsageChartSelectionDropDownButton.Text.Substring(FORMAT_CAPACITY_CHARTTYPE.Length - 3);
                lastTrendsChartType =
                    Settings.Default.DatabasesSummaryViewTrendsChartType = recentTrendsChartSelectionDropDownButton.Text.Substring(FORMAT_TRENDS_CHARTTYPE.Length - 3);
                lastChartsVisible =
                    Settings.Default.DatabasesSummaryViewChartsVisible = ChartVisible;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesSummaryView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                DatabasesSummaryView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            configuration.SetPreviousRefresh(currentSnapshot);
            return (Snapshot)managementService.GetDatabaseSummary(configuration);
        }

        //SQLDM-28182 - Need to implement HandleBackgroundWorkerError and update the databasesGridStatusLabel to display a proper message instead of displaying "Loading" always
        public override void HandleBackgroundWorkerError(Exception e)
        {
            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Clear();
            databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
            databasesGridStatusLabel.Text =
                capacityUsageChartStatusLabel.Text =
                recentTrendsChartStatusLabel.Text = UNABLE_TO_UPDATE;
          
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            if (data != null && data is DatabaseSummary)
            {
                lock (updateLock)
                {
                    DatabaseSummary snapshot = data as DatabaseSummary;

                    if (snapshot.Error == null)
                    {
                        if (!databaseFilterInitialized)
                        {
                            UpdateDatabasesFilter(snapshot.Databases);
                        }

                        if (snapshot.Databases != null && snapshot.Databases.Count > 0)
                        {
                            currentDataTable.BeginLoadData();

                            // remove any databases that have been deleted
                            List<DataRow> deleteRows = new List<DataRow>();
                            foreach (DataRow row in currentDataTable.Rows)
                            {
                                if (!snapshot.Databases.ContainsKey((string)row["Database Name"]))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                currentDataTable.Rows.Remove(row);
                            }

                            //now update any matching databases or add new ones
                            foreach (DatabaseDetail db in snapshot.Databases.Values)
                            {
                                // determine the database status for the grid
                                MonitoredState severity = MonitoredState.OK;
                                if (ApplicationModel.Default.GetInstanceStatus(instanceId) != null
                                    && ApplicationModel.Default.GetInstanceStatus(instanceId).DatabaseMap.ContainsKey(db.Name))
                                {
                                    Objects.DatabaseStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId).DatabaseMap[db.Name];
                                    if (status.Issues != null && status.IssueCount > 0)
                                    {
                                        severity = status.Issues[0].Severity;
                                    }
                                }
                                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId|| ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                                {
                                    currentDataTable.LoadDataRow(
                                        new object[]
                                    {
                                        severity,
                                        db.Name,
                                        db.Name.Length > 25 ? string.Concat(db.Name.Substring(0, 22), "...") : db.Name,
                                        db.IsSystemDatabase ? "System" : "User",
                                        new DatabaseStatusDisplay(db.Status),
                                        db.RecoveryType,
                                        db.DataFileSize.Megabytes,
                                        db.DataSize.Megabytes,
                                        db.PercentDataSizeCurrent * 100,
                                        db.IndexSize.Megabytes,
                                        db.PercentIndexSizeCurrent * 100,
                                        db.TextSize.Megabytes,
                                        db.PercentTextSizeCurrent * 100,
                                        db.UsedSize.Megabytes,
                                        db.PercentUsedSizeCurrent * 100,
                                        db.UnusedSize.Megabytes,
                                        db.PercentDataFreeCurrent * 100,
                                        db.PercentDataFreeSize * 100,
                                        db.PercentDataSize * 100,
                                        db.DatabaseExpansion.Megabytes,
                                        db.LogFileSize.Megabytes,
                                        db.LogSizeUsed.Megabytes,
                                        db.PercentLogSizeCurrent * 100,
                                        db.LogSizeUnused.Megabytes,
                                        db.PercentLogFreeCurrent * 100,
                                        db.PercentLogSpace * 100,
                                        db.LogExpansion.Megabytes,
                                        db.OldestOpenTransactionSpid,
                                        db.OldestOpenTransactionStartTime.HasValue ? (object)((DateTime)db.OldestOpenTransactionStartTime).ToLocalTime() : null,
                                        db.LastBackup.HasValue ? (object)((DateTime)db.LastBackup).ToLocalTime() : null,
                                        db.DateCreated,
                                        db.CompatibilityLevel,
                                        db.SystemTables,
                                        db.UserTables,
                                        db.FileGroupCount,
                                        db.FileCount,
                                        db.DataAutogrow,
                                        db.LogAutogrow,
                                        db.ProcessCount,
                                        db.InMemoryStorageUsage
                                    }, true);
                                }
                                else
                                {
                                    currentDataTable.LoadDataRow(
                                        new object[]
                                    {
                                        severity,
                                        db.Name,
                                        db.Name.Length > 25 ? string.Concat(db.Name.Substring(0, 22), "...") : db.Name,
                                        db.IsSystemDatabase ? "System" : "User",
                                        new DatabaseStatusDisplay(db.Status),
                                        db.RecoveryType,
                                        db.DataFileSize.Megabytes,
                                        db.DataSize.Megabytes,
                                        db.PercentDataSizeCurrent * 100,
                                        db.IndexSize.Megabytes,
                                        db.PercentIndexSizeCurrent * 100,
                                        db.TextSize.Megabytes,
                                        db.PercentTextSizeCurrent * 100,
                                        db.UsedSize.Megabytes,
                                        db.PercentUsedSizeCurrent * 100,
                                        db.UnusedSize.Megabytes,
                                        db.PercentDataFreeCurrent * 100,
                                        db.PercentDataFreeSize * 100,
                                        db.PercentDataSize * 100,
                                        db.DatabaseExpansion.Megabytes,
                                        db.LogFileSize.Megabytes,
                                        db.LogSizeUsed.Megabytes,
                                        db.PercentLogSizeCurrent * 100,
                                        db.LogSizeUnused.Megabytes,
                                        db.PercentLogFreeCurrent * 100,
                                        db.PercentLogSpace * 100,
                                        db.LogExpansion.Megabytes,
                                        db.OldestOpenTransactionSpid,
                                        db.OldestOpenTransactionStartTime.HasValue ? (object)((DateTime)db.OldestOpenTransactionStartTime).ToLocalTime() : null,
                                        db.LastBackup.HasValue ? (object)((DateTime)db.LastBackup).ToLocalTime() : null,
                                        db.DateCreated,
                                        db.CompatibilityLevel,
                                        db.SystemTables,
                                        db.UserTables,
                                        db.FileGroupCount,
                                        db.FileCount,
                                        db.DataAutogrow,
                                        db.LogAutogrow,
                                        db.ProcessCount
                                    }, true);
                                }                                
                            }
                            currentDataTable.EndLoadData();

                            if (!initialized)
                            {
                                if (lastMainGridSettings != null)
                                {
                                    GridSettings.ApplySettingsToGrid(lastMainGridSettings, databasesGrid);

                                    initialized = true;
                                }
                                else if (snapshot.Databases.Count > 0)
                                {
                                    foreach (UltraGridColumn column in databasesGrid.DisplayLayout.Bands[0].Columns)
                                    {
                                        if (column.Key != "State")
                                        {
                                            try
                                            {
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            }
                                            catch (Exception exception)
                                            {
                                                Log.Error("UpdateData Not State Column: " + exception);
                                            }
                                            column.Width = Math.Min(column.Width, databasesGrid.Width / 2);
                                        }
                                    }

                                    initialized = true;
                                }

                                if (databasesGrid.Rows.Count > 0 && databasesGrid.Selected.Rows.Count == 0
                                    && selectedDatabaseArgument == null)
                                {
                                    databasesGrid.Rows[0].Selected = true;
                                }
                            }
                            currentSnapshot = snapshot;

                            foreach (UltraGridRow row in databasesGrid.Rows.GetAllNonGroupByRows())
                            {
                                if (row.IsDataRow)
                                {
                                    if (selectedDatabaseArgument != null && selectedDatabaseArgument.Count > 0)
                                    {
                                        DataRowView dataRow = row.ListObject as DataRowView;
                                        if (selectedDatabaseArgument.Contains((string)dataRow["Database Name"]))
                                        {
                                            row.Selected = true;
                                            databasesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        }
                                    }
                                    row.RefreshSortPosition();
                                }
                            }

                            databasesGrid.Visible = true;
                            UpdateCapacityDataFilter();
                            AddHistoryData(snapshot);
                            ConfigureRecentTrendsChart(recentTrendsChartSelectionDropDownButton.Text.Substring(recentTrendsChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
                            UpdateChartDataFilter();
                        }
                        else
                        {
                            databasesGridStatusLabel.Text = NO_ITEMS;
                            databasesGrid.Visible = false;
                            databasesGrid.DataSource = null;
                            capacityUsageChartStatusLabel.Text =
                                recentTrendsChartStatusLabel.Text = SELECT_DB;
                            capacityUsageChart.Visible =
                                recentTrendsChart.Visible = false;
                            ApplicationController.Default.ClearCustomStatus();
                        }

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        databasesFilterComboBox.Enabled = false;
                        databasesFilterComboBox.Items.Clear();
                        databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                        databasesGridStatusLabel.Text =
                            capacityUsageChartStatusLabel.Text =
                            recentTrendsChartStatusLabel.Text = UNABLE_TO_UPDATE;
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                    }

                    foreach (UltraGridRow gridRow in databasesGrid.Rows.GetAllNonGroupByRows())
                    {
                        DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                        DataRow dataRow = dataRowView.Row;

                        AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                        if (alertConfig != null)
                        {
                            UpdateCellColor(Metric.DatabaseStatus, alertConfig, gridRow, "Status", 1);
                            UpdateCellColor(Metric.OldestOpenTransMinutes, alertConfig, gridRow, "Oldest Open Transaction Start Time", 1);
                            UpdateCellColor(Metric.DatabaseSizePct, alertConfig, gridRow, "% Data Full", 1);
                            UpdateCellColor(Metric.TransLogSize, alertConfig, gridRow, "% Log Full", 1);
                        }
                    }
                }
            }
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, string columnName, int adjustmentMultiplier)
        {
            AlertConfigurationItem alertConfigItem = alertConfig[metric, (string)gridRow.Cells["Database Name"].Value];
            if (alertConfigItem != null)
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName))
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {
                        IComparable value = (IComparable)dataRow[columnName];
                        if (metric == Metric.DatabaseStatus)
                        {
                            if (value.Equals(""))
                                value = DatabaseStatus.Undetermined;
                            else
                                value = ((DatabaseStatusDisplay)value).Status;   
                        } else
                        if (metric == Metric.OldestOpenTransMinutes)
                        {   
                            if (value == null)
                                value = 0;
                            else
                            {
                                TimeSpan diff = DateTime.Now - (DateTime)value;
                                value = (diff.TotalMinutes < 0) ? 0 : diff.TotalMinutes;
                            }
                        }
                        if (value != null && adjustmentMultiplier != 1)
                        {
                            double dbl = (double)Convert.ChangeType(value, typeof(double));
                            value = dbl * adjustmentMultiplier;
                        }
                        switch (alertConfigItem.GetSeverity(value))
                        {
                            case MonitoredState.Informational:
                                cell.Appearance.BackColor = Color.Blue;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            case MonitoredState.Warning:
                                cell.Appearance.BackColor = Color.Gold;
                                cell.Appearance.ResetForeColor();
                                break;
                            case MonitoredState.Critical:
                                cell.Appearance.BackColor = Color.Red;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            default:
                                cell.Appearance.ResetBackColor();
                                cell.Appearance.ResetForeColor();
                                break;
                        }
                    }
                }
            }
        }


        #endregion

        public void ToggleUserFilter()
        {
            configuration.IncludeSystemDatabases = !configuration.IncludeSystemDatabases;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }
        #endregion

        #region helpers

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

        private void ShowBackupRestoreHistoryView()
        {
            if (selectedDatabases.Count > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesBackupRestoreHistory, selectedDatabases.Values);
            }
        }

        private void ShowFilesView()
        {
            if (selectedDatabases.Count > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesFiles, selectedDatabases.Values);
            }
        }

        private void ShowSessionDetailsView(int sessionId)
        {
            if (sessionId > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsDetails, sessionId);
            }
        }

        #region DataTables

        private void InitializeCurrentDataTable()
        {
            currentDataTable = new DataTable();
            currentDataTable.Columns.Add("State", typeof(string));
            currentDataTable.Columns.Add("Database Name", typeof(string));
            currentDataTable.Columns.Add("Chart Name", typeof(string));
            currentDataTable.Columns.Add("Type", typeof(string));
            currentDataTable.Columns.Add("Status", typeof(DatabaseStatusDisplay));
            currentDataTable.Columns.Add("Recovery Model", typeof(string));
            currentDataTable.Columns.Add("Data File Size (MB)", typeof(decimal));
            currentDataTable.Columns.Add("Data Tables (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Data Tables", typeof(float));
            currentDataTable.Columns.Add("Data Indexes (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Data Indexes", typeof(float));
            currentDataTable.Columns.Add("Data Text (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Data Text", typeof(float));
            currentDataTable.Columns.Add("Data Used (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Data Used", typeof(float));
            currentDataTable.Columns.Add("Data Unused (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Data Unused", typeof(float));
            currentDataTable.Columns.Add("% Data Free", typeof(float));
            currentDataTable.Columns.Add("% Data Full", typeof(float));
            currentDataTable.Columns.Add("Data Potential Growth", typeof(decimal));
            currentDataTable.Columns.Add("Log File Size (MB)", typeof(decimal));
            currentDataTable.Columns.Add("Log Used (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Log Used", typeof(float));
            currentDataTable.Columns.Add("Log Unused (MB)", typeof(decimal));
            currentDataTable.Columns.Add("% Log Unused", typeof(float));
            currentDataTable.Columns.Add("% Log Full", typeof(float));
            currentDataTable.Columns.Add("Log Potential Growth", typeof(decimal));
            currentDataTable.Columns.Add("Oldest Open Transaction", typeof(int));
            currentDataTable.Columns.Add("Oldest Open Transaction Start Time", typeof(DateTime));
            currentDataTable.Columns.Add("Last Backup", typeof(DateTime));
            currentDataTable.Columns.Add("Date Created", typeof(DateTime));
            currentDataTable.Columns.Add("Version Compatibility", typeof(float));
            currentDataTable.Columns.Add("System Tables", typeof(int));
            currentDataTable.Columns.Add("Tables", typeof(int));
            currentDataTable.Columns.Add("File Groups", typeof(int));
            currentDataTable.Columns.Add("Files", typeof(int));
            currentDataTable.Columns.Add("Data Autogrowth", typeof(int));
            currentDataTable.Columns.Add("Log Autogrowth", typeof(int));
            currentDataTable.Columns.Add("Active Sessions", typeof(long));
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId|| ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
               currentDataTable.Columns.Add("In Memory Storage Usage Percent", typeof(decimal));

            currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns["Database Name"] };
            currentDataTable.CaseSensitive = true;

            currentDataTable.DefaultView.Sort = "Database Name";

            databasesGrid.DataSource = currentDataTable;

            capacityChartDataSource = new DataView(currentDataTable, string.Empty, "[Database Name] desc", DataViewRowState.CurrentRows);
        }

        private void InitializeHistoryDataTable()
        {
            historyDataTable = new DataTable();
            historyDataTable.Columns.Add("Date", typeof(DateTime));
        }

        private void ConfigureHistoryDataTable(DatabaseSummary snapshot)
        {
            // There needs to be a column in the row for each metric in each database in the grid
            // since databases can be added on the fly, this will get called on each refresh and will
            // add new columns when a new database appears. No attempt will be made to remove deleted db columns
            if (snapshot.Databases.Count > 0)
            {
                foreach (DatabaseDetail db in snapshot.Databases.Values)
                {
                    string colName = string.Format("{0} - Active Sessions", db.Name);
                    if (!historyDataTable.Columns.Contains(colName))
                    {
                        historyDataTable.Columns.Add(colName,typeof(long));

                        colName = string.Format("{0} - Transactions/sec", db.Name);
                        historyDataTable.Columns.Add(colName, typeof(long));

                        colName = string.Format("{0} - Data Size (MB)", db.Name);
                        historyDataTable.Columns.Add(colName, typeof(decimal));

                        colName = string.Format("{0} - Log Size (MB)", db.Name);
                        historyDataTable.Columns.Add(colName, typeof(decimal));

                        colName = string.Format("{0} - Log Flushes", db.Name);
                        historyDataTable.Columns.Add(colName, typeof(long));
                    }
                }
            }
        }

        private void AddHistoryData(DatabaseSummary snapshot)
        {
            if (snapshot != null && snapshot.Error == null)
            {
                ConfigureHistoryDataTable(snapshot);

                DataRow newRow = historyDataTable.NewRow();
                newRow["Date"] = DateTime.Now;
                foreach (DatabaseDetail db in snapshot.Databases.Values)
                {
                    if (db.ProcessCount.HasValue)
                    {
                        newRow[string.Format("{0} - Active Sessions", db.Name)] = db.ProcessCount;
                    }
                    if (db.Transactions.HasValue && snapshot.TimeDelta.HasValue)
                    {
                        // TimeDelta is 0 when previous snapshot time is higher than current snapshot time.
                        // This TimeDelta comes from CollectionServices DatabaseSummaryProbe class 
                        // The chart value  is set to null when there would be division by zero. 
                        // For the chart it doesnt matter this times, only the deltas, 
                        // and when one delta = 0 comes it will just skip it and show an empty space for that period of time.
                        
                        if (snapshot.TimeDelta.Value.TotalSeconds != 0)
                        {
                            newRow[string.Format("{0} - Transactions/sec", db.Name)] = db.Transactions.Value / snapshot.TimeDelta.Value.TotalSeconds;
                        }
                        else
                        {
                            newRow[string.Format("{0} - Transactions/sec", db.Name)] = DBNull.Value;   
                        }
                    }
                    if (db.DataFileSize.Megabytes.HasValue)
                    {
                        newRow[string.Format("{0} - Data Size (MB)", db.Name)] = db.DataFileSize.Megabytes;
                    }
                    if (db.LogFileSize.Megabytes.HasValue)
                    {
                        newRow[string.Format("{0} - Log Size (MB)", db.Name)] = db.LogFileSize.Megabytes;
                    }
                    if (db.LogFlushes.HasValue)
                    {
                        newRow[string.Format("{0} - Log Flushes", db.Name)] = db.LogFlushes;
                    }
                }
                historyDataTable.Rows.Add(newRow);
                GroomHistoryData();
            }
        }

        private void GroomHistoryData()
        {
            if (historyDataTable != null)
            {
                DateTime groomThreshold =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));

                DataRow[] groomedRows = historyDataTable.Select(string.Format("Date < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture))); // SQLDM-19237, Tolga K

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                historyDataTable.AcceptChanges();
            }
        }

        private void UpdateCapacityDataFilter()
        {
            // Clean up the selection list and build the filter string for the DataView
            StringBuilder filter = new StringBuilder();
            List<string> deleteItems = new List<string>();
            foreach (string database in selectedDatabases.Values)
            {
                if (currentDataTable.Rows.Contains(database))
                {
                    filter.Append(filter.Length > 0 ? "," : string.Empty);
                    filter.Append("'");
                    filter.Append(database.Replace("'", "''"));
                    filter.Append("'");
                }
                else
                {
                    deleteItems.Add(database);
                }
            }
            if (filter.Length > 0)
            {
                filter.Insert(0, "[Database Name] in (");
                filter.Append(")");
            }

            capacityChartDataSource.RowFilter = filter.ToString();

            // remove any selections that are no longer in the list
            foreach (string item in deleteItems)
            {
                selectedDatabases.Remove(item);
            }

            if (selectedDatabases.Count > 0)
            {
                capacityUsageChart.Visible = true;
            }
            else
            {
                if (initialized)
                {
                    capacityUsageChartStatusLabel.Text = SELECT_DB;
                }
                capacityUsageChart.Visible = false;
            }
        }

        private void UpdateChartDataFilter()
        {
            if (historyDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                historyDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion

        #region grid

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                databasesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                databasesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                databasesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                databasesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    databasesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    databasesGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            databasesGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            databasesGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(databasesGrid);
            dialog.Show(this);
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{0} - databases as of {1}",
                              ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "Databases";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Fix the icon columns to export as text values with a heading
                databasesGrid.DisplayLayout.Bands[0].Columns["State"].Header.Caption = "State";
                databasesGrid.DisplayLayout.ValueLists["stateValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;

                try
                {
                    ultraGridExcelExporter.Export(databasesGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                databasesGrid.DisplayLayout.Bands[0].Columns["State"].Header.Caption = string.Empty;
                databasesGrid.DisplayLayout.ValueLists["stateValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
            }
        }

        #endregion

        #region chart

        private void InitializeCharts()
        {
            InitializeCapacityUsageChart();
            InitializeRecentTrendsChart();
        }

        private void InitializeCapacityUsageChart()
        {
            capacityUsageChart.Tag = capacityUsageChartSelectionDropDownButton;
            capacityUsageChart.Printer.Orientation = PageOrientation.Landscape;
            capacityUsageChart.Printer.Compress = true;
            capacityUsageChart.Printer.ForceColors = true;
            capacityUsageChart.Printer.Document.DocumentName = "Capacity Usage Chart";
            capacityUsageChart.ToolBar.RemoveAt(0);
            capacityUsageChart.DataSource = capacityChartDataSource;

            ConfigureCapacityUsageChart("Data Megabytes");
            capacityUsageChart.Visible = false;
        }

        private void ConfigureCapacityUsageChart(string usageType)
        {
            capacityUsageChartSelectionDropDownButton.Text = string.Format(FORMAT_CAPACITY_CHARTTYPE, usageType);

            capacityUsageChart.SuspendLayout();
            capacityUsageChart.DataSourceSettings.Fields.Clear();

            switch (usageType)
            {
                case "Data Megabytes":
                    FieldMap dbFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
                    dbFieldMap.DisplayName = "Database";
                    FieldMap tableUsageFieldMap = new FieldMap("Data Tables (MB)", FieldUsage.Value);
                    tableUsageFieldMap.DisplayName = "Tables";
                    FieldMap indexUsageFieldMap = new FieldMap("Data Indexes (MB)", FieldUsage.Value);
                    indexUsageFieldMap.DisplayName = "Indexes";
                    FieldMap textUsageFieldMap = new FieldMap("Data Text (MB)", FieldUsage.Value);
                    textUsageFieldMap.DisplayName = "Text";
                    FieldMap unusedUsageFieldMap = new FieldMap("Data Unused (MB)", FieldUsage.Value);
                    unusedUsageFieldMap.DisplayName = "Unused";

                    capacityUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dbFieldMap,
                        tableUsageFieldMap,
                        indexUsageFieldMap,
                        textUsageFieldMap,
                        unusedUsageFieldMap});
                    capacityUsageChart.AxisY.AutoScale = true;
                    capacityUsageChart.AxisY.DataFormat.Decimals = 2;
                    capacityUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Data Percent":
                    FieldMap pctDbFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
                    pctDbFieldMap.DisplayName = "Database";
                    FieldMap pctTableUsageFieldMap = new FieldMap("% Data Tables", FieldUsage.Value);
                    pctTableUsageFieldMap.DisplayName = "Tables";
                    FieldMap pctIndexUsageFieldMap = new FieldMap("% Data Indexes", FieldUsage.Value);
                    pctIndexUsageFieldMap.DisplayName = "Indexes";
                    FieldMap pctTextUsageFieldMap = new FieldMap("% Data Text", FieldUsage.Value);
                    pctTextUsageFieldMap.DisplayName = "Text";
                    FieldMap pctUnusedUsageFieldMap = new FieldMap("% Data Unused", FieldUsage.Value);
                    pctUnusedUsageFieldMap.DisplayName = "Unused";

                    capacityUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        pctDbFieldMap,
                        pctTableUsageFieldMap,
                        pctIndexUsageFieldMap,
                        pctTextUsageFieldMap,
                        pctUnusedUsageFieldMap});
                    capacityUsageChart.AxisY.Min = 0;
                    capacityUsageChart.AxisY.Max = 100;
                    capacityUsageChart.AxisY.DataFormat.Decimals = 0;
                    capacityUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
                case "Log Megabytes":
                    FieldMap logDbFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
                    logDbFieldMap.DisplayName = "Database";
                    FieldMap logUsedUsageFieldMap = new FieldMap("Log Used (MB)", FieldUsage.Value);
                    logUsedUsageFieldMap.DisplayName = "Used";
                    FieldMap logFreeUsageFieldMap = new FieldMap("Log Unused (MB)", FieldUsage.Value);
                    logFreeUsageFieldMap.DisplayName = "Unused";

                    capacityUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        logDbFieldMap,
                        logUsedUsageFieldMap,
                        logFreeUsageFieldMap});
                    capacityUsageChart.AxisY.AutoScale = true;
                    capacityUsageChart.AxisY.DataFormat.Decimals = 2;
                    capacityUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Log Percent":
                    FieldMap pctLogDbFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
                    pctLogDbFieldMap.DisplayName = "Database";
                    FieldMap pctLogUsedUsageFieldMap = new FieldMap("% Log Used", FieldUsage.Value);
                    pctLogUsedUsageFieldMap.DisplayName = "Used";
                    FieldMap pctLogFreeUsageFieldMap = new FieldMap("% Log Unused", FieldUsage.Value);
                    pctLogFreeUsageFieldMap.DisplayName = "Unused";

                    capacityUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        pctLogDbFieldMap,
                        pctLogUsedUsageFieldMap,
                        pctLogFreeUsageFieldMap});
                    capacityUsageChart.AxisY.Min = 0;
                    capacityUsageChart.AxisY.Max = 100;
                    capacityUsageChart.AxisY.DataFormat.Decimals = 0;
                    capacityUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            capacityUsageChart.DataSourceSettings.ReloadData();
            capacityUsageChart.Invalidate();
            capacityUsageChart.ResumeLayout();
            capacityUsageChart_Resize(capacityUsageChart, new EventArgs());
        }

        private void InitializeRecentTrendsChart()
        {
            recentTrendsChart.Tag = recentTrendsChartSelectionDropDownButton;
            recentTrendsChart.AxisX.LabelsFormat.Format = AxisFormat.Time;
            recentTrendsChart.LegendBox.Width = 155;
            recentTrendsChart.LegendBox.PlotAreaOnly = false;

            recentTrendsChart.Printer.Orientation = PageOrientation.Landscape;
            recentTrendsChart.Printer.Compress = true;
            recentTrendsChart.Printer.ForceColors = true;
            recentTrendsChart.Printer.Document.DocumentName = "Recent Trends Chart";
            recentTrendsChart.ToolBar.RemoveAt(0);
            recentTrendsChart.DataSource = historyDataTable;

            ConfigureRecentTrendsChart("Active Sessions");
            recentTrendsChart.Visible = false;
        }

        private void ConfigureRecentTrendsChart(string trendType)
        {
            recentTrendsChartSelectionDropDownButton.Text = string.Format(FORMAT_TRENDS_CHARTTYPE, trendType);

            if (selectedDatabases.Count > 0)
            {
                recentTrendsChart.SuspendLayout();
                recentTrendsChart.DataSourceSettings.Fields.Clear();
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                dateFieldMap.DisplayName = "Date";
                recentTrendsChart.DataSourceSettings.Fields.Add(dateFieldMap);

                foreach (string database in selectedDatabases.Values)
                {
                    FieldMap trendFieldMap = new FieldMap(string.Format("{0} - {1}", database, trendType), FieldUsage.Value);
                    trendFieldMap.DisplayName = database;
                    recentTrendsChart.DataSourceSettings.Fields.Add(trendFieldMap);
                }

                recentTrendsChart.DataSourceSettings.ReloadData();
                recentTrendsChart.Visible = true;
                recentTrendsChart_Resize(recentTrendsChart, new EventArgs());
                switch (trendType)
                {
                    case "Active Sessions":
                        recentTrendsChart.AxisY.DataFormat.Decimals = 0;
                        recentTrendsChart.ToolTipFormat = "%s\n%v sessions\n%x";
                        break;
                    case "Transactions/sec":
                        recentTrendsChart.AxisY.DataFormat.Decimals = 0;
                        recentTrendsChart.ToolTipFormat = "%s\n%v transactions/second\n%x";
                        break;
                    case "Data Size (MB)":
                    case "Log Size (MB)":
                        recentTrendsChart.AxisY.DataFormat.Decimals = 2;
                        recentTrendsChart.ToolTipFormat = "%s\n%v MB\n%x";
                        break;
                    case "Log Flushes":
                        recentTrendsChart.AxisY.DataFormat.Decimals = 0;
                        recentTrendsChart.ToolTipFormat = "%s\n%v flushes/second\n%x";
                        break;
                    default:
                        recentTrendsChart.ToolTipFormat = "%s\n%v\n%x";
                        break;
                }
                recentTrendsChart.Invalidate();
                recentTrendsChart.ResumeLayout();
            }
            else
            {
                if (initialized)
                {
                    recentTrendsChartStatusLabel.Text = SELECT_DB;
                }
                recentTrendsChart.Visible = false;
            }
        }

        private void ToggleChartToolbar(Chart chart, bool visible)
        {
            if (chart != null)
            {
                chart.ToolBar.Visible = visible;
            }
        }

        private void PrintChart(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title,
                                                              ExportHelper.GetValidFileName(title, true));
            }
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            splitContainer.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            contentContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart()
        {
            if (restoreCapacityUsageChartButton.Visible)
            {
                RestoreChart(capacityUsagePanel, maximizeCapacityUsageChartButton, restoreCapacityUsageChartButton, 0, 0);
            }
            else if (restoreRecentTrendsChartButton.Visible)
            {
                RestoreChart(recentTrendsPanel, maximizeRecentTrendsChartButton, restoreRecentTrendsChartButton, 1, 0);
            }
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            contentContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            splitContainer.Visible = true;
        }

        #endregion

        #endregion

        #region events

        #region Splitter Focus

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion

        #region toolbars

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "gridDataContextMenu":
                    bool showSession = false;
                    if (databasesGrid.Rows.Count > 0 && databasesGrid.Selected.Rows.Count == 1)
                    {
                        DataRowView row = (DataRowView)databasesGrid.Selected.Rows[0].ListObject;
                        showSession = (row["Oldest Open Transaction"] != System.DBNull.Value
                                                        && (int)row["Oldest Open Transaction"] > 0);
                    }
                    ((PopupMenuTool)e.Tool).Tools["viewOldestOpenTransactionButton"].SharedProps.Enabled = showSession;
                    break;
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(
                        chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = databasesGrid.Rows.Count > 0 && databasesGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(databasesGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
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
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                    break;
                case "printChartButton":
                    PrintChart(contextMenuSelectedChart);
                    break;
                case "exportChartDataButton":
                    SaveChartData(contextMenuSelectedChart);
                    break;
                case "exportChartImageButton":
                    SaveChartImage(contextMenuSelectedChart);
                    break;
                case "viewBackupRestoreHistoryButton":
                    ShowBackupRestoreHistoryView();
                    break;
                case "viewFilesButton":
                    ShowFilesView();
                    break;
                case "viewOldestOpenTransactionButton":
                    if (databasesGrid.Rows.Count > 0 && databasesGrid.Selected.Rows.Count == 1)
                    {
                        DataRowView row = (DataRowView)databasesGrid.Selected.Rows[0].ListObject;

                        if (row["Oldest Open Transaction"] != null)
                        {
                            ShowSessionDetailsView((int)row["Oldest Open Transaction"]);
                        }
                    }
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }

            contextMenuSelectedChart = null;
        }

        #endregion

        #region grid

        private void databasesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedDatabases.Clear();
            if (databasesGrid.Rows.Count > 0 && databasesGrid.Selected.Rows.Count > 0)
            {
                foreach (UltraGridRow row in databasesGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedDatabases.Add((string)row.Cells["Database Name"].Value,
                            (string)row.Cells["Database Name"].Value);
                    }
                }
            }

            UpdateCapacityDataFilter();
            ConfigureRecentTrendsChart(recentTrendsChartSelectionDropDownButton.Text.Substring(recentTrendsChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
            UpdateChartDataFilter();
        }

        private void databasesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        this.databasesGrid.Selected.Rows.Clear();
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridDataContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void databasesGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = databasesGrid.DisplayLayout.Bands[0];
            EditorWithText textEditor = new EditorWithText();
            band.Columns["State"].Editor = textEditor;
        }

        #endregion

        #region chart

        private void maximizeCapacityUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(capacityUsagePanel, maximizeCapacityUsageChartButton, restoreCapacityUsageChartButton);
        }

        private void restoreCapacityUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(capacityUsagePanel, maximizeCapacityUsageChartButton, restoreCapacityUsageChartButton, 0, 0);
        }

        private void maximizeRecentTrendsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(recentTrendsPanel, maximizeRecentTrendsChartButton, restoreRecentTrendsChartButton);
        }

        private void restoreRecentTrendsChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(recentTrendsPanel, maximizeRecentTrendsChartButton, restoreRecentTrendsChartButton, 1, 0);
        }

        private void showCapacityUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureCapacityUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCapacityUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureCapacityUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCapacityUsageLogInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureCapacityUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCapacityUsageLogInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureCapacityUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showActiveSessionsTrendButton_Click(object sender, EventArgs e)
        {
            ConfigureRecentTrendsChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showTransactionsPerSecondTrendButton_Click(object sender, EventArgs e)
        {
            ConfigureRecentTrendsChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showDataSizeTrendButton_Click(object sender, EventArgs e)
        {
            ConfigureRecentTrendsChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showLogSizeTrendButton_Click(object sender, EventArgs e)
        {
            ConfigureRecentTrendsChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showLogFlushesTrendButton_Click(object sender, EventArgs e)
        {
            ConfigureRecentTrendsChart(((ToolStripDropDownItem)sender).Text);
        }

        private void capacityUsageChart_GetTip(object sender, GetTipEventArgs e)
        {
            Chart chart = ((Chart)sender);
            if (e.Object == chart.AxisX)
            {
                if (e.Text.Length > 0)
                {
                    HitTestEventArgs e1 = chart.HitTest(chartMouseX, chartMouseY);
                    int currentPoint = (int)Math.Floor(e1.Value);
                    if (currentPoint < ((DataView)chart.DataSource).Count)
                    {
                        e.Text = (string)((DataView)chart.DataSource)[currentPoint]["Database Name"];
                    }
                }
            }
        }

        private void capacityUsageChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 4;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
            }
        }

        private void recentTrendsChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 2;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }

        #region Chart Click Events

        private void chart_MouseMove(object sender, HitTestEventArgs e)
        {
            // this is a common event handler for all charts
            //Chart chart = ((Chart)sender);
            //e = chart.HitTest(e.AbsoluteLocation.X, e.AbsoluteLocation.Y, true);
            //chart.Titles.Clear();
            //chart.Titles.Add(new TitleDockable(e.HitType.ToString() + " object=" + e.Object.ToString() + " point=" + e.Point.ToString() + " series=" + e.Series.ToString() + " value=" + e.Value.ToString()));
            chartMouseX = e.X;
            chartMouseY = e.Y;
        }

        private void OnChartMouseClick(object sender, HitTestEventArgs e)
        {
            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other)
            {
                // hit type of other means they clicked on the chart toolbar
                ServerViews targetView;
                if (sender == capacityUsageChart)
                {
                    targetView = ServerViews.DatabasesFiles;
                    ApplicationController.Default.ShowServerView(instanceId, targetView, selectedDatabases.Values);
                }
                else if (sender == recentTrendsChart)
                {
                    // "Recent Trends : charttype"
                    switch (recentTrendsChartSelectionDropDownButton.Text.Substring(15))
                    {
                        case "Data Size (MB)":
                        case "Log Size (MB)":
                        case "Log Flushes":
                            targetView = ServerViews.DatabasesFiles;
                            ApplicationController.Default.ShowServerView(instanceId, targetView, selectedDatabases.Values);
                            break;
                        case "Active Sessions":
                        case "Transactions/sec":
                        default:
                            targetView = ServerViews.SessionsDetails;
                            ApplicationController.Default.ShowServerView(instanceId, targetView);
                            break;
                    }
                }
                else
                    return;
            }
        }

        #endregion

        private void DatabasesSummaryView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

        #endregion

        #region ToolTipItemCreationFilter
        /// <summary>
        /// A CreationFilter that will trap for elements within the UltraWinTree
        /// and set tooltip information on them.
        /// </summary>
        public class ToolTipItemCreationFilter : IUIElementCreationFilter
        {
            #region Private Members

            private int instanceId;
            private IToolTipItem toolTipItem = null;

            public ToolTipItemCreationFilter(int instanceId)
            {
                this.instanceId = instanceId;
            }

            #endregion Private Members

            #region Implementation of IUIElementCreationFilter
            void IUIElementCreationFilter.AfterCreateChildElements(Infragistics.Win.UIElement parent)
            {
                if (parent is CellUIElement && ((CellUIElement)parent).Column.Key.Equals("State"))
                {
                    parent.ToolTipItem = this.ToolTipItem;
                }
            }

            bool IUIElementCreationFilter.BeforeCreateChildElements(Infragistics.Win.UIElement parent)
            {
                // Do nothing here.
                return false;
            }
            #endregion

            #region ToolTipItem
            /// <summary>
            /// This single instance of a TreeToolTipItem will be assigned to 
            /// any UIElement in the tree that needs to display a tooltip. The TreeToolTipItem
            /// class will determine what tooltip to display based on the element. 
            /// </summary>
            private IToolTipItem ToolTipItem
            {
                get
                {
                    if (this.toolTipItem == null)
                        this.toolTipItem = new TableToolTipItem(instanceId);

                    return this.toolTipItem;
                }
            }
            #endregion ToolTipItem
        }
        #endregion ToolTipItemCreationFilter

        #region TreeToolTipItem
        /// <summary>
        /// This class provides tooltip information based on the type of UIElement
        /// in the UltraWinTree.
        /// </summary>
        public class TableToolTipItem : IToolTipItem
        {
            private int instanceId;

            public TableToolTipItem(int instanceId)
            {
                this.instanceId = instanceId;
            }

            ToolTipInfo IToolTipItem.GetToolTipInfo(Point mousePosition, UIElement element, UIElement previousToolTipElement, ToolTipInfo toolTipInfoDefault)
            {
                toolTipInfoDefault.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;

                if (element is CellUIElement && ((CellUIElement)element).Column.Key.Equals("State"))
                {
                    UltraGridCell cell = ((CellUIElement)element).Cell;

                    if (cell != null)
                    {
                        if (cell.Row.IsDataRow && cell.Column.Key.Equals("State"))
                        {
                            toolTipInfoDefault.ToolTipText = "";
                            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId);

                            if (status != null)
                            {
                                Objects.DatabaseStatus dbStatus;
                                string databaseName = cell.Row.Cells["Database Name"].Text;

                                if (status.DatabaseMap.TryGetValue(databaseName, out dbStatus) &&
                                    dbStatus.IssueCount > 0)
                                {
                                    toolTipInfoDefault.ToolTipText = FormatToolTipText(databaseName, dbStatus.Issues, dbStatus.IssueCount);
                                }
                                else
                                {
                                    toolTipInfoDefault.ToolTipText = databaseName + " - OK";
                                }
                            }
                        }

                        // Position the tooltip over the node
                        toolTipInfoDefault.Location = element.Control.PointToScreen(element.Rect.Location);
                    }
                }

                return toolTipInfoDefault;
            }

            public string FormatToolTipText(string databaseName, Issue[] issues, int count)
            {
                StringBuilder buffer = new StringBuilder();

                if (count > 0)
                {
                    buffer.AppendFormat("{0} - {1}\n", databaseName, issues[0].Severity);

                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0)
                            buffer.Append("\n");
                        buffer.AppendFormat("{0}. {1}", i + 1, issues[i].Subject);
                    }
                }

                return buffer.ToString();
            }
        }
        #endregion TreeToolTipItem

        #region Databases Filter

        public void UpdateDatabasesFilter(IDictionary<string, DatabaseDetail> databases)
        {
            databasesFilterComboBox.Items.Clear();

            if (databases != null && databases.Count > 0)
            {
                databasesFilterComboBox.Enabled = true;
                databasesFilterComboBox.Items.Add(null, AllDatabasesItemText);

                foreach (string database in databases.Keys)
                {
                    databasesFilterComboBox.Items.Add(database, database);
                }

                databaseFilterInitialized = true;
            }
            else
            {
                databasesFilterComboBox.Enabled = false;
                databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", NO_ITEMS));
            }

            databasesFilterComboBox.SelectedIndex = FindDatabaseIndex(selectedDatabaseFilter);
        }

        private int FindDatabaseIndex(string database)
        {
            int index = 0;

            if (database != null)
            {
                // Skip the first item since it's the "< All Databases >" item 
                for (int i = 1; i < databasesFilterComboBox.Items.Count; i++)
                {
                    if (string.CompareOrdinal(databasesFilterComboBox.Items[i].DataValue as string, database) == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        private void databasesFilterComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (databasesFilterComboBox.SelectedItem != null)
            {
                ApplicationController.Default.ClearCustomStatus();

                if (databasesFilterComboBox.SelectedItem.DisplayText == AllDatabasesItemText)
                {
                    selectedDatabaseFilter = null;
                    currentDataTable.DefaultView.RowFilter = null;
                    ApplicationController.Default.SetCustomStatus(String.Format("Databases: {0} Item{1}",
                                                                                currentDataTable.DefaultView.Count,
                                                                                currentDataTable.DefaultView.Count == 1
                                                                                    ? string.Empty
                                                                                    : "s"));
                }
                else if (databasesFilterComboBox.Items.Count > 1)
                {
                    selectedDatabaseFilter = databasesFilterComboBox.SelectedItem.DisplayText;
                    currentDataTable.DefaultView.RowFilter =
                        string.Format("[Database Name] = '{0}'", selectedDatabaseFilter.Replace("'", "''"));
                    
                    databasesGrid.Selected.Rows.Clear();
                    UltraGridRow[] nonGroupByRows = databasesGrid.Rows.GetAllNonGroupByRows();
                    if (nonGroupByRows.Length > 0)
                    {
                        databasesGrid.Selected.Rows.Add(nonGroupByRows[0]);
                    }
                    ApplicationController.Default.SetCustomStatus("Filter Applied", "Databases: 1 Item");
                }

                currentDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                databasesGrid.Focus();
            }
        }

        #endregion

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void refreshDatabasesButton_Click(object sender, EventArgs e)
        {
            databaseFilterInitialized = false;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);            
        }
    }
}
