using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using DatabaseStatus = Idera.SQLdm.Common.Snapshots.DatabaseStatus;
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesFilesView : ServerBaseView, IDatabasesView
    {
        #region constants

        private const string AllDatabasesItemText = "< All Databases >";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        ///Ankit Nagpal --Sqldm10.0.0
        private const string SELECT_DB = @"Select one or more databases.";
        private const string SELECT_FILE = @"Select one or more files.";
        private const string SELECT_FILEGROUP = @"Select one or more file groups.";
        private const string SELECT_LOG = @"Select one or more logs.";
        private const string DB_NOT_SUPPORTED = "Database {0} cannot be selected.\nDatabase compatibility level 60 is not supported.";
        private const string FORMAT_KEY = "{0}..{1}";
        private const string FORMAT_FILES_CHARTTYPE = "File Usage: {0}";
        private const string FORMAT_GROUPS_CHARTTYPE = "File Group Usage: {0}";
        private const string FORMAT_LOGS_CHARTTYPE = "Transaction Log Usage: {0}";
        private const string FORMAT_DISKS_CHARTTYPE = "Current Disk Usage: {0}";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;
        private string selectedDatabaseFilter = null;
        private bool initialized = false;
        private bool initializedFiles = false;
        private bool initializedGroups = false;
        private bool initializedLogs = false;
        private DataTable currentDataTable;
        private DataTable dataFilesDataTable;
        private DataTable fileGroupsDataTable;
        private DataSet transactionLogsDataSet;
        private DataTable transactionLogsDataTable;
        private DataTable logFilesDataTable;
        private DataTable disksDataTable;
        private DataView fileUsageChartsDataSource;
        private DataView fileGroupUsageChartsDataSource;
        private DataView transactionLogsUsageChartsDataSource;
        private DataView diskUsageChartsDataSource;
        private DatabaseSummaryConfiguration configuration;
        private DatabaseFilesConfiguration filesConfiguration;
        private DatabaseSummary currentSnapshot = null;
        SortedList<string, string> selectedDatabases = new SortedList<string, string>();
        private List<string> selectedDatabaseArgument = new List<string>();
        private bool selectionChanged = false;
        SortedList<string, Pair<string, string>> selectedFiles = new SortedList<string, Pair<string, string>>();
        SortedList<string, Pair<string, string>> selectedFileGroups = new SortedList<string, Pair<string, string>>();
        SortedList<string, string> selectedLogs = new SortedList<string, string>();
        private bool showDiskOtherFiles = true;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private UltraGrid selectedGrid = null;
        private int chartMouseX = 0;
        private int chartMouseY = 0;
        private bool databaseFilterInitialized = false;
        private bool databaseFilterSelectionChanging = false;
        private Dictionary<string, DiskDrive> diskDrives = null;
        private TempdbStatistics tempdbStatistics = null;

        // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added fields for the drill down behaviour of filegroup alerts
        private int metricID;
        private string filegroupName = null;
        // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added fields for the drill down behaviour of filegroup alerts

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
                    Idera.SQLdm.DesktopClient.Objects.MonitoredSqlServerStatus server = ApplicationModel.Default.GetInstanceStatus(instanceId);
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
        private int lastMainSplitterDistance = 0;
        private int lastFilesSplitterDistance = 0;
        private int lastGroupsSplitterDistance = 0;
        private int lastLogsSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastFilesGridSettings = null;
        private GridSettings lastGroupsGridSettings = null;
        private GridSettings lastLogsGridSettings = null;
        private string lastFilesChartType = null;
        private string lastGroupsChartType = null;
        private string lastLogsChartType = null;
        private string lastDisksChartType = null;
        private int lastFilesTab = 0;

        private string currentDiskUsageType = null;
        private Dictionary<string, bool> diskDataColumns;

        #endregion

        #region constructors

        public DatabasesFilesView(int instanceId) : base(instanceId)
        {			
            InitializeComponent();

            ChartFxExtensions.SetContextMenu(fileUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(fileGroupUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(transactionLogUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(diskUsageChart, toolbarsManager);

            databasesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            tabControl.DrawFilter = new HideFocusRectangleDrawFilter();
            filesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            fileGroupsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            transactionLogsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            databasesGrid.CreationFilter = new ToolTipItemCreationFilter(instanceId);
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            databasesGrid.Tag = "Databases";
            filesGrid.Tag = filesTabPage;
            fileGroupsGrid.Tag = fileGroupsTabPage;
            transactionLogsGrid.Tag = transactionLogsTabPage;

            configuration = new DatabaseSummaryConfiguration(instanceId);
            configuration.IncludeSystemDatabases = true;
            filesConfiguration = new DatabaseFilesConfiguration(instanceId, null);

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

            // use the same autogrowth value list for the File Groups & Logs
            ValueListHelpers.CopyValueList(databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"],
                fileGroupsGrid.DisplayLayout.ValueLists["autogrowthValueList"]);
            ValueListHelpers.CopyValueList(databasesGrid.DisplayLayout.ValueLists["autogrowthValueList"],
                transactionLogsGrid.DisplayLayout.ValueLists["autogrowthValueList"]);

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

            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING +" >");
            databasesGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            databasesGridStatusLabel.Text =
                diskUsageChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;

            noFileSystemObjectDataLabel.Visible = false;

            diskDataColumns = new Dictionary<string, bool>();

            InitializeCurrentDataTable();
            InitializeFilesDataTable();
            InitializeFileGroupsDataTable();
            InitializeTransactionLogsDataSet();
            InitializeDisksDataTable();
            InitializeCharts();
            AdaptFontSize();
        }

        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public string SelectedDatabaseFilter
        {
            get { return selectedDatabaseFilter; }
        }
        
        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        /// <summary>
        /// Get or Set the Databases Grid GroupByBox visibility and trigger state update event if changed
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
        /// Get the current database configuration settings to manage state for current selections
        /// </summary>
        public DatabaseSummaryConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesFilesView);
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
                selectedDatabaseFilter = ((MonitoredObjectName)argument).DatabaseName;
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
            // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --process the arguments for the drill down behaviour of filegroup alerts
            else if (argument is object[])
            {
                object[] selection = argument as object[];

                if (selection.Length == 3 && ((int)selection[1] == 126 || (int)selection[1] == 127))
                {
                    selectedDatabaseFilter = selection[0] as string;
                    tabControl.Tabs[1].Selected = true;

                    metricID = (int)selection[1];
                    filegroupName = selection[2] as string;
                }
            }
            // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --process the arguments for the drill down behaviour of filegroup alerts

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
                            DataRowView dataRow = (DataRowView)row.ListObject;
                            if (dataRow["Version Compatibility"] != System.DBNull.Value
                                && (float)dataRow["Version Compatibility"] < 6.5f)
                            {
                                ApplicationMessageBox.ShowWarning(this, String.Format(DB_NOT_SUPPORTED, database));
                            }
                            else
                            {
                                selectedDatabases.Add(database, database);
                                row.Selected = true;
                                databasesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            }
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
            lastMainSplitterDistance = splitContainer1.Height - Settings.Default.DatabasesFilesViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer1.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer1.Height - splitContainer1.SplitterDistance;
            }

            // Fixed panel is second panel, so restore size of second panel
            lastFilesSplitterDistance = filesSplitContainer.Height - Settings.Default.DatabasesFilesViewFilesSplitter;
            if (lastFilesSplitterDistance > 0)
            {
                filesSplitContainer.SplitterDistance = lastFilesSplitterDistance;
            }
            else
            {
                lastFilesSplitterDistance = filesSplitContainer.Height - filesSplitContainer.SplitterDistance;
            }

            // Fixed panel is second panel, so restore size of second panel
            lastGroupsSplitterDistance = fileGroupsSplitContainer.Height - Settings.Default.DatabasesFilesViewGroupsSplitter;
            if (lastGroupsSplitterDistance > 0)
            {
                fileGroupsSplitContainer.SplitterDistance = lastGroupsSplitterDistance;
            }
            else
            {
                lastGroupsSplitterDistance = fileGroupsSplitContainer.Height - fileGroupsSplitContainer.SplitterDistance;
            }

            // Fixed panel is second panel, so restore size of second panel
            lastLogsSplitterDistance = transactionLogSplitContainer.Height - Settings.Default.DatabasesFilesViewLogsSplitter;
            if (lastLogsSplitterDistance > 0)
            {
                transactionLogSplitContainer.SplitterDistance = lastLogsSplitterDistance;
            }
            else
            {
                lastLogsSplitterDistance = transactionLogSplitContainer.Height - transactionLogSplitContainer.SplitterDistance;
            }

            string chartType = Settings.Default.DatabasesFilesViewFilesChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in fileUsageChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureFileUsageChart(chartType);
                        lastFilesChartType = chartType;
                        break;
                    }
                }
            }

            chartType = Settings.Default.DatabasesFilesViewGroupsChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in fileGroupUsageChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureFileGroupsUsageChart(chartType);
                        lastGroupsChartType = chartType;
                        break;
                    }
                }
            }

            chartType = Settings.Default.DatabasesFilesViewLogsChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in transactionLogUsageChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureTransactionLogUsageChart(chartType);
                        lastLogsChartType = chartType;
                        break;
                    }
                }
            }

            chartType = Settings.Default.DatabasesFilesViewDisksChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in diskUsageChartSelectionDropDownButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureDiskUsageChart(chartType);
                        lastDisksChartType = chartType;
                        break;
                    }
                }
            }

            if (Settings.Default.DatabasesFilesViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.DatabasesFilesViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, databasesGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            if (Settings.Default.DatabasesFilesViewFilesGrid is GridSettings)
            {
                lastFilesGridSettings = Settings.Default.DatabasesFilesViewFilesGrid;
                GridSettings.ApplySettingsToGrid(lastFilesGridSettings, filesGrid);
            }

            if (Settings.Default.DatabasesFilesViewGroupsGrid is GridSettings)
            {
                lastGroupsGridSettings = Settings.Default.DatabasesFilesViewGroupsGrid;
                GridSettings.ApplySettingsToGrid(lastGroupsGridSettings, fileGroupsGrid);
            }

            if (Settings.Default.DatabasesFilesViewLogsGrid is GridSettings)
            {
                lastLogsGridSettings = Settings.Default.DatabasesFilesViewLogsGrid;
                GridSettings.ApplySettingsToGrid(lastLogsGridSettings, transactionLogsGrid);
            }

            if (Settings.Default.DatabasesFilesViewFilesTab > -1
                && Settings.Default.DatabasesFilesViewFilesTab < tabControl.Tabs.Count)
            {
                lastFilesTab = Settings.Default.DatabasesFilesViewFilesTab;
                tabControl.Tabs[lastFilesTab].Selected = true;
            }
        }

        public override void SaveSettings()
        {
            try
            {
                GridSettings mainGridSettings = GridSettings.GetSettings(databasesGrid);
                GridSettings filesGridSettings = GridSettings.GetSettings(filesGrid);
                GridSettings groupsGridSettings = GridSettings.GetSettings(fileGroupsGrid);
                GridSettings logsGridSettings = GridSettings.GetSettings(transactionLogsGrid);
                // save all settings only if anything has changed
                if (lastMainSplitterDistance != splitContainer1.Height - splitContainer1.SplitterDistance
                    || lastFilesSplitterDistance != filesSplitContainer.Height - filesSplitContainer.SplitterDistance
                    ||
                    lastGroupsSplitterDistance !=
                    fileGroupsSplitContainer.Height - fileGroupsSplitContainer.SplitterDistance
                    ||
                    lastLogsSplitterDistance !=
                    transactionLogSplitContainer.Height - transactionLogSplitContainer.SplitterDistance
                    || !fileUsageChartSelectionDropDownButton.Text.EndsWith(lastFilesChartType)
                    || !fileGroupUsageChartSelectionDropDownButton.Text.EndsWith(lastGroupsChartType)
                    || !transactionLogUsageChartSelectionDropDownButton.Text.EndsWith(lastLogsChartType)
                    || !diskUsageChartSelectionDropDownButton.Text.EndsWith(lastDisksChartType)
                    || !mainGridSettings.Equals(lastMainGridSettings)
                    || !filesGridSettings.Equals(lastFilesGridSettings)
                    || !groupsGridSettings.Equals(lastGroupsGridSettings)
                    || !logsGridSettings.Equals(lastLogsGridSettings)
                    || lastFilesTab != tabControl.ActiveTab.Index)
                {
                    // Fixed panel is second panel, so save size of second panel
                    lastMainSplitterDistance =
                        Settings.Default.DatabasesFilesViewMainSplitter =
                        splitContainer1.Height - splitContainer1.SplitterDistance;
                    // Fixed panel is second panel, so save size of second panel
                    lastFilesSplitterDistance =
                        Settings.Default.DatabasesFilesViewFilesSplitter =
                        filesSplitContainer.Height - filesSplitContainer.SplitterDistance;
                    // Fixed panel is second panel, so save size of second panel
                    lastGroupsSplitterDistance =
                        Settings.Default.DatabasesFilesViewGroupsSplitter =
                        fileGroupsSplitContainer.Height - fileGroupsSplitContainer.SplitterDistance;
                    // Fixed panel is second panel, so save size of second panel
                    lastLogsSplitterDistance =
                        Settings.Default.DatabasesFilesViewLogsSplitter =
                        transactionLogSplitContainer.Height - transactionLogSplitContainer.SplitterDistance;
                    lastFilesChartType =
                        Settings.Default.DatabasesFilesViewFilesChartType =
                        fileUsageChartSelectionDropDownButton.Text.Substring(FORMAT_FILES_CHARTTYPE.Length - 3);
                    lastGroupsChartType =
                        Settings.Default.DatabasesFilesViewGroupsChartType =
                        fileGroupUsageChartSelectionDropDownButton.Text.Substring(FORMAT_GROUPS_CHARTTYPE.Length - 3);
                    lastLogsChartType =
                        Settings.Default.DatabasesFilesViewLogsChartType =
                        transactionLogUsageChartSelectionDropDownButton.Text.Substring(FORMAT_LOGS_CHARTTYPE.Length - 3);
                    lastDisksChartType =
                        Settings.Default.DatabasesFilesViewDisksChartType =
                        diskUsageChartSelectionDropDownButton.Text.Substring(FORMAT_DISKS_CHARTTYPE.Length - 3);
                    lastMainGridSettings =
                        Settings.Default.DatabasesFilesViewMainGrid = mainGridSettings;
                    lastFilesGridSettings =
                        Settings.Default.DatabasesFilesViewFilesGrid = filesGridSettings;
                    lastGroupsGridSettings =
                        Settings.Default.DatabasesFilesViewGroupsGrid = groupsGridSettings;
                    lastLogsGridSettings =
                        Settings.Default.DatabasesFilesViewLogsGrid = logsGridSettings;
                    lastFilesTab =
                        Settings.Default.DatabasesFilesViewFilesTab = tabControl.ActiveTab.Index;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while saving the view settings.", e);
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesFilesView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                DatabasesFilesView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Pair<DatabaseSummary, DatabaseFilesSnapshot> configs = new Pair<DatabaseSummary, DatabaseFilesSnapshot>();

            if (!selectionChanged)
            {
                configs.First = managementService.GetDatabaseSummary(configuration);
            }

            List<string> configDbs = new List<string>();
            List<string> deleteItems = new List<string>();
            if (selectedDatabases.Count > 0)
            {
                foreach (string database in selectedDatabases.Values)
                {
                    if (configs.First == null || configs.First.Databases.ContainsKey(database))
                    {
                        configDbs.Add(database);
                    }
                    else
                    {
                        deleteItems.Add(database);
                    }
                }
            }
            foreach (string item in deleteItems)
            {
                selectedDatabases.Remove(item);
            }
            filesConfiguration.DatabaseNames = configDbs;
            if (diskDrives != null)
                filesConfiguration.PreviousDiskDrives = diskDrives;
            filesConfiguration.PreviousTempdbStatistics = tempdbStatistics;
            configs.Second = managementService.GetDatabaseFiles(filesConfiguration);

            return configs;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
            tabControlStatusLabel.Text = UNABLE_TO_UPDATE;
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Pair<DatabaseSummary, DatabaseFilesSnapshot> snapshotPair;
            Exception e = null;

            if (data != null && data is Pair<DatabaseSummary, DatabaseFilesSnapshot>)
            {
                snapshotPair = (Pair<DatabaseSummary, DatabaseFilesSnapshot>)data;
                if (snapshotPair.First != null)
                {
                    DatabaseSummary snapshot = snapshotPair.First as DatabaseSummary;
                    lock (updateLock)
                    {
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

                                    currentDataTable.LoadDataRow(
                                            new object[]
                                    {
                                        severity,
                                        db.Name,
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
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
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
                                        DataRowView dataRow = row.ListObject as DataRowView;
                                        if (selectedDatabaseArgument != null && selectedDatabaseArgument.Count > 0)
                                        {
                                            if (selectedDatabaseArgument.Contains((string)dataRow["Database Name"]))
                                            {
                                                if (dataRow["Version Compatibility"] != System.DBNull.Value
                                                    && (float)dataRow["Version Compatibility"] < 6.5f)
                                                {
                                                    ApplicationMessageBox.ShowWarning(this, String.Format(DB_NOT_SUPPORTED, (string)dataRow["Database Name"]));
                                                }
                                                else
                                                {
                                                    row.Selected = true;
                                                    databasesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                                }
                                            }
                                        }
                                        row.RefreshSortPosition();
                                    }
                                }
                                if (databasesGrid.Rows.Count > 0)
                                {
                                    selectedDatabaseArgument = null;
                                }

                                databasesGrid.Visible = true;
                            }
                            else
                            {
                                currentDataTable.Clear();
                                databasesGridStatusLabel.Text = NO_ITEMS;
                                databasesGrid.Visible = false;
                                tabControlStatusLabel.Text = SELECT_DB;
                                tabControl.Visible = false;
                            }

                            databasesGrid.ResumeLayout();
                        }
                        else
                        {
                            e = snapshot.Error;
                            databasesGrid.Visible = false;
                            databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            if (snapshot.ProbeError != null)
                            {
                                databasesGridStatusLabel.Text = snapshot.ProbeError.ToString();
                            }
                            else if (e != null)
                            {
                                databasesGridStatusLabel.Text = e.Message;
                            }
                        }
                    }
                }

                if (snapshotPair.Second != null)
                {
                    lock (updateLock)
                    {
                        DatabaseFilesSnapshot snapshot = snapshotPair.Second as DatabaseFilesSnapshot;

                        if (snapshot.Error == null)
                        {
                            tempdbStatistics = snapshot.TempdbStatistics;

                            if (snapshot.DiskDrives != null)
                                diskDrives = snapshot.DiskDrives;

                            // Load Files grid data
                            dataFilesDataTable.BeginLoadData();
                            logFilesDataTable.BeginLoadData();

                            // remove any data files that are no longer in view
                            List<DataRow> deleteRows = new List<DataRow>();
                            foreach (DataRow row in dataFilesDataTable.Rows)
                            {
                                if (!snapshot.DatabaseFiles.ContainsKey(string.Concat((string)row["Database Name"], (string)row["Logical File Name"])))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                dataFilesDataTable.Rows.Remove(row);
                            }

                            // remove any log files that are no longer in view
                            deleteRows.Clear();
                            foreach (DataRow row in logFilesDataTable.Rows)
                            {
                                if (!snapshot.DatabaseFiles.ContainsKey(string.Concat((string)row["Database Name"], (string)row["Logical File Name"])))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                logFilesDataTable.Rows.Remove(row);
                            }

                            if (snapshot.DatabaseFiles.Count > 0)
                            {
                                foreach (DatabaseFile file in snapshot.DatabaseFiles.Values)
                                {
                                    string dbFileName = string.Format(FORMAT_KEY, file.DatabaseName, file.LogicalFilename);

                                    if (file.IsDataFile)
                                    {
                                        dataFilesDataTable.LoadDataRow(
                                            new object[]
                                                {
                                                    dbFileName,
                                                    dbFileName.Length > 60
                                                        ? string.Concat(dbFileName.Substring(0, 57), "...")
                                                        : dbFileName,
                                                    file.LogicalFilename,
                                                    file.IsDataFile ? "Data" : "Log",
                                                    file.DatabaseName,
                                                    file.FilegroupName,
                                                    file.CurrentSize.Megabytes,
                                                    file.CurrentUsedSize.Megabytes,
                                                    file.PercentUsed*100,
                                                    file.CurrentFreeSize.Megabytes,
                                                    file.PercentFreeCurrent*100,
                                                    file.GrowthLabel(System.Globalization.CultureInfo.CurrentCulture),
                                                    file.ExpansionSpace.Megabytes,
                                                    file.PercentFull*100,
                                                    file.PercentFreePotential*100,
                                                    file.FreeSpaceOnDisk.Megabytes,
                                                    file.FilePath
                                                }, true);
                                    }
                                    else
                                    {
                                        logFilesDataTable.LoadDataRow(
                                                new object[]
                                                {
                                                    dbFileName,
                                                    file.LogicalFilename,
                                                    file.DatabaseName,
                                                    file.FilegroupName,
                                                    file.CurrentSize.Megabytes,
                                                    file.GrowthLabel(System.Globalization.CultureInfo.CurrentCulture),
                                                    file.ExpansionSpace.Megabytes,
                                                    file.FreeSpaceOnDisk.Megabytes,
                                                    file.FilePath
                                                }, true);
                                    }
                                }

                                dataFilesDataTable.EndLoadData();
                                logFilesDataTable.EndLoadData();

                                if (!initializedFiles)
                                {
                                    if (lastFilesGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastFilesGridSettings, filesGrid);

                                        initializedFiles = true;
                                    }
                                    else if (dataFilesDataTable.Rows.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in filesGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, filesGrid.Width / 2);
                                        }

                                        initializedFiles = true;
                                    }
                                }

                                foreach (UltraGridRow row in filesGrid.Rows.GetAllNonGroupByRows())
                                {
                                    row.RefreshSortPosition();
                                }
                                filesGrid.Visible = true;
                            }
                            else
                            {
                                dataFilesDataTable.EndLoadData();
                                filesGrid.Visible = false;
                                filesGridStatusLabel.Text = NO_ITEMS;
                            }
                            UpdateFilesChartsDataFilter();
                            ConfigureFileUsageChart(fileUsageChartSelectionDropDownButton.Text.Substring(fileUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));


                            // Load File Groups grid
                            fileGroupsDataTable.BeginLoadData();

                            // remove any file groups that are no longer in view
                            deleteRows.Clear();
                            foreach (DataRow row in fileGroupsDataTable.Rows)
                            {
                                if (!snapshot.DatabaseFileGroups.ContainsKey(string.Concat((string)row["Database Name"], (string)row["File Group"])))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                fileGroupsDataTable.Rows.Remove(row);
                            }

                            if (snapshot.DatabaseFileGroups.Count > 0)
                            {
                                foreach (DatabaseFileGroup group in snapshot.DatabaseFileGroups.Values)
                                {
                                    string dbGroupName = string.Format(FORMAT_KEY, group.DatabaseName, group.FilegroupName);
                                    fileGroupsDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        dbGroupName,
                                        dbGroupName.Length > 60 ? string.Concat(dbGroupName.Substring(0, 57), "...") : dbGroupName,
                                        group.FilegroupName,
                                        group.DatabaseName,
                                        group.FileCount,
                                        group.CurrentSize.Megabytes,
                                        group.CurrentUsedSize.Megabytes,
                                        group.DataSize.Megabytes,
                                        group.PercentDataSizeCurrent * 100,
                                        group.IndexSize.Megabytes,
                                        group.PercentIndexSizeCurrent * 100,
                                        group.TextSize.Megabytes,
                                        group.PercentTextSizeCurrent * 100,
                                        group.CurrentFreeSize.Megabytes,
                                        group.PercentFreeCurrent * 100,
                                        group.Autogrow,
                                        group.ExpansionSpace.Megabytes,
                                        group.PercentFull * 100,
                                        group.PercentFreePotential * 100
                                    }, true);
                                }

                                fileGroupsDataTable.EndLoadData();

                                if (!initializedGroups)
                                {
                                    if (lastGroupsGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastGroupsGridSettings, fileGroupsGrid);

                                        initializedGroups = true;
                                    }
                                    else if (fileGroupsDataTable.Rows.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in fileGroupsGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, fileGroupsGrid.Width / 2);
                                        }

                                        initializedGroups = true;
                                    }
                                }

                                foreach (UltraGridRow row in fileGroupsGrid.Rows.GetAllNonGroupByRows())
                                {
                                    row.RefreshSortPosition();
                                }
                                fileGroupsGrid.Visible = true;

                                // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --search and select the filegroup corresponding to the filegroup alerts 
                                if ((metricID == 126 || metricID == 127) && !String.IsNullOrEmpty(filegroupName))
                                {
                                    fileGroupsGrid.Selected.Rows.Clear();
                                    foreach (UltraGridRow row in fileGroupsGrid.Rows)
                                    {
                                        if (row.Cells["File Group"].Value.Equals(filegroupName))
                                        {
                                            row.Selected = true;
                                            break;
                                        }
                                    }
                                }
                                // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --search and select the filegroup corresponding to the filegroup alerts
                            }
                            else
                            {
                                fileGroupsDataTable.EndLoadData();
                                fileGroupsGrid.Visible = false;
                                fileGroupsGridStatusLabel.Text = NO_ITEMS;
                            }
                            UpdateFileGroupsChartsDataFilter();
                            ConfigureFileGroupsUsageChart(fileGroupUsageChartSelectionDropDownButton.Text.Substring(fileGroupUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));


                            // Load Transaction Logs grid data
                            transactionLogsDataTable.BeginLoadData();

                            // remove any logs that are no longer in view & clear stats
                            deleteRows.Clear();
                            foreach (DataRow row in transactionLogsDataTable.Rows)
                            {
                                if (!snapshot.DatabaseLogs.ContainsKey((string)row["Database Name"]))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                transactionLogsDataTable.Rows.Remove(row);
                            }

                            if (snapshot.DatabaseLogs.Count > 0)
                            {
                                foreach (DatabaseFileGroup log in snapshot.DatabaseLogs.Values)
                                {
                                    transactionLogsDataTable.LoadDataRow(
                                                new object[]
                                            {
                                                log.DatabaseName,
                                                log.DatabaseName.Length > 60 ? string.Concat(log.DatabaseName.Substring(0, 57), "...") : log.DatabaseName,
                                                log.FileCount,
                                                log.CurrentSize.Megabytes,
                                                log.CurrentUsedSize.Megabytes,
                                                log.PercentUsed * 100,
                                                log.CurrentFreeSize.Megabytes,
                                                log.PercentFreeCurrent * 100,
                                                log.Autogrow,
                                                log.ExpansionSpace.Megabytes,
                                                log.PercentFull * 100,
                                                log.PercentFreePotential * 100
                                            }, true);
                                }

                                transactionLogsDataTable.EndLoadData();

                                if (!initializedLogs)
                                {
                                    if (lastLogsGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastLogsGridSettings, transactionLogsGrid);
                                        initializedLogs = true;
                                    }
                                    else if (transactionLogsDataTable.Rows.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in transactionLogsGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, transactionLogsGrid.Width / 2);
                                        }

                                        foreach (UltraGridColumn column in transactionLogsGrid.DisplayLayout.Bands[1].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, transactionLogsGrid.Width / 2);
                                        }

                                        initializedLogs = true;
                                    }
                                }

                                foreach (UltraGridRow row in transactionLogsGrid.Rows.GetAllNonGroupByRows())
                                {
                                    row.RefreshSortPosition();
                                }
                                transactionLogsGrid.Visible = true;
                            }
                            else
                            {
                                transactionLogsDataTable.EndLoadData();
                                transactionLogsGrid.Visible = false;
                                transactionLogsGridStatusLabel.Text = NO_ITEMS;
                            }
                            UpdateLogsChartsDataFilter();
                            ConfigureTransactionLogUsageChart(transactionLogUsageChartSelectionDropDownButton.Text.Substring(transactionLogUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));


                            // Load Disks datasource
                            disksDataTable.BeginLoadData();

                            // remove any disks that are no longer in view
                            deleteRows.Clear();
                            foreach (DataRow row in disksDataTable.Rows)
                            {
                                if (!snapshot.SqlDiskDrives.ContainsKey((string)row["Drive Letter"]))
                                {
                                    deleteRows.Add(row);
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                disksDataTable.Rows.Remove(row);
                            }

                            if (snapshot.SqlDiskDrives.Count > 0)
                            {
                                // reset it to true in the unlikely event it changes between refreshes
                                showDiskOtherFiles = true;
                                foreach (DiskDrive disk in snapshot.SqlDiskDrives.Values)
                                {
                                    disksDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        disk.DriveLetter,
                                        disk.UsedSize.Megabytes,
                                        disk.PercentUsed * 100,
                                        disk.SqlUsedSize.Megabytes,
                                        disk.PercentSqlUsed * 100,
                                        disk.SqlUnusedSize.Megabytes,
                                        disk.PercentSqlUnused * 100,
                                        disk.SqlLogSize.Megabytes,
                                        disk.PercentSqlLog * 100,
                                        disk.OtherFilesSize.Megabytes,
                                        disk.PercentOther * 100,
                                        disk.UnusedSize.Megabytes,
                                        disk.PercentUnused * 100
                                    }, true);

                                    if (disk.OtherFilesSize.Megabytes == null)
                                    {
                                        showDiskOtherFiles = false;
                                    }
                                }

                                disksDataTable.EndLoadData();
                            }
                            else
                            {
                                disksDataTable.EndLoadData();
                            }

                            UpdateDisksChartsDataFilter();
                            ConfigureDiskUsageChart(diskUsageChartSelectionDropDownButton.Text.Substring(diskUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
                            noFileSystemObjectDataLabel.Visible = !snapshot.IsFileSystemObjectDataAvailable;

                            // reset selectionChanged to enable full databasesummary refreshes again
                            selectionChanged = false;
                            tabControl.Visible = true;

                        }
                        else
                        {
                            e = snapshot.Error;
                            tabControl.Visible = false;
                            tabControlStatusLabel.Text = UNABLE_TO_UPDATE;
                            diskDrives = null;
                        }
                    }
                }
            }
            else
            {
                databasesGrid.Visible = false;
                databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                tabControl.Visible = false;
                tabControlStatusLabel.Text = UNABLE_TO_UPDATE;

            }

            foreach (UltraGridRow gridRow in databasesGrid.Rows.GetAllNonGroupByRows())
            {
                DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                DataRow dataRow = dataRowView.Row;

                AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                if (alertConfig != null)
                {
                    UpdateCellColor(Metric.DatabaseStatus, alertConfig, gridRow, "Status", 1);
                    //UpdateCellColor(Metric.OldestOpenTransMinutes, alertConfig, gridRow, "Oldest Open Transaction Start Time", 1);
                    UpdateCellColor(Metric.DatabaseSizePct, alertConfig, gridRow, "% Data Full", 1);
                    UpdateCellColor(Metric.TransLogSize, alertConfig, gridRow, "% Log Full", 1);
                }
            }

            ShowStatus();

            if (e == null)
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
            else
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, e));
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
                        }
                        else
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

        private void FocusLastFocusedControl()
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void ShowStatus()
        {
            if (currentDataTable.Rows.Count == 0)
            {
                ApplicationController.Default.ClearCustomStatus();
            }
            else
            {
                int subCount;
                switch (tabControl.ActiveTab.Text)
                {
                    case "Files":
                        subCount = dataFilesDataTable.Rows.Count;
                        break;
                    case "File Groups":
                        subCount = fileGroupsDataTable.Rows.Count;
                        break;
                    case "Transaction Logs":
                        subCount = transactionLogsDataTable.Rows.Count;
                        break;
                    default:
                        subCount = 0;
                        break;
                }
                if (subCount == 0)
                {
                    ApplicationController.Default.SetCustomStatus(
                        databasesFilterComboBox.SelectedIndex == 0 ? "Filter Applied" : string.Empty,
                        String.Format("Databases: {0} Item{1}",
                                currentDataTable.DefaultView.Count,
                                currentDataTable.DefaultView.Count == 1 ? string.Empty : "s")
                        );
                }
                else
                {
                    ApplicationController.Default.SetCustomStatus(
                        String.Format("{0}  Databases: {1} Item{2}",
                                databasesFilterComboBox.SelectedIndex == 0 ? "Filter Applied" : string.Empty,
                                currentDataTable.DefaultView.Count,
                                currentDataTable.DefaultView.Count == 1 ? string.Empty : "s"),
                        String.Format("{0}: {1} Item{2}",
                                tabControl.ActiveTab.Text,
                                subCount,
                                subCount == 1 ? string.Empty : "s")
                        );
                }
            }
        }

        private void ShowSummaryView()
        {
            if (selectedDatabases.Count > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesSummary, selectedDatabases.Values);
            }
        }

        private void ShowBackupRestoreHistoryView()
        {
            if (selectedDatabases.Count > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesBackupRestoreHistory, selectedDatabases.Values);
            }
        }

        private void ShowFileActivity()
        {
            string dbn = databasesFilterComboBox.SelectedItem.DisplayText;

            if(dbn == AllDatabasesItemText)
                ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.ResourcesFileActivity, new string[] { });
            else
                ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.ResourcesFileActivity, new string[] { dbn });
        }

        #region DataTables

        private void InitializeCurrentDataTable()
        {
            currentDataTable = new DataTable();
            currentDataTable.Columns.Add("State", typeof(string));
            currentDataTable.Columns.Add("Database Name", typeof(string));
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

            currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns["Database Name"] };
            currentDataTable.CaseSensitive = true;

            currentDataTable.DefaultView.Sort = "[Database Name]";

            databasesGrid.DataSource = currentDataTable;
        }

        private void InitializeFilesDataTable()
        {
            dataFilesDataTable = new DataTable();
            dataFilesDataTable.Columns.Add("Database File Name", typeof(string));
            dataFilesDataTable.Columns.Add("Chart Name", typeof(string));
            dataFilesDataTable.Columns.Add("Logical File Name", typeof(string));
            dataFilesDataTable.Columns.Add("Type", typeof(string));
            dataFilesDataTable.Columns.Add("Database Name", typeof(string));
            dataFilesDataTable.Columns.Add("File Group", typeof(string));
            dataFilesDataTable.Columns.Add("Size (MB)", typeof(decimal));
            dataFilesDataTable.Columns.Add("Size Used (MB)", typeof(decimal));
            dataFilesDataTable.Columns.Add("% Current Used", typeof(float));
            dataFilesDataTable.Columns.Add("Size Free (MB)", typeof(decimal));
            dataFilesDataTable.Columns.Add("% Current Free", typeof(float));
            dataFilesDataTable.Columns.Add("Auto Growth", typeof(string));
            dataFilesDataTable.Columns.Add("Potential Growth (MB)", typeof(decimal));
            dataFilesDataTable.Columns.Add("% Potential Used", typeof(float));
            dataFilesDataTable.Columns.Add("% Potential Free", typeof(float));
            dataFilesDataTable.Columns.Add("Disk Free (MB)", typeof(decimal));
            dataFilesDataTable.Columns.Add("File Path", typeof(string));

            dataFilesDataTable.PrimaryKey = new DataColumn[] { dataFilesDataTable.Columns["Database Name"], dataFilesDataTable.Columns["Logical File Name"] };
            dataFilesDataTable.CaseSensitive = true;

            dataFilesDataTable.DefaultView.Sort = "[Database Name], [Logical File Name]";

            filesGrid.DataSource = dataFilesDataTable;

            fileUsageChartsDataSource = new DataView(dataFilesDataTable, string.Empty, "[Database Name] desc, [Logical File Name] desc", DataViewRowState.CurrentRows);
        }

        private void InitializeFileGroupsDataTable()
        {
            fileGroupsDataTable = new DataTable();
            fileGroupsDataTable.Columns.Add("Database File Group", typeof(string));
            fileGroupsDataTable.Columns.Add("Chart Name", typeof(string));
            fileGroupsDataTable.Columns.Add("File Group", typeof(string));
            fileGroupsDataTable.Columns.Add("Database Name", typeof(string));
            fileGroupsDataTable.Columns.Add("Files", typeof(int));
            fileGroupsDataTable.Columns.Add("Size (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("Size Used (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("Tables (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("% Tables", typeof(float));
            fileGroupsDataTable.Columns.Add("Indexes (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("% Indexes", typeof(float));
            fileGroupsDataTable.Columns.Add("Text (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("% Text", typeof(float));
            fileGroupsDataTable.Columns.Add("Free (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("% Free", typeof(float));
            fileGroupsDataTable.Columns.Add("Auto Growth", typeof(int));
            fileGroupsDataTable.Columns.Add("Potential Growth (MB)", typeof(decimal));
            fileGroupsDataTable.Columns.Add("% Potential Used", typeof(float));
            fileGroupsDataTable.Columns.Add("% Potential Free", typeof(float));

            fileGroupsDataTable.PrimaryKey = new DataColumn[] { fileGroupsDataTable.Columns["Database Name"], fileGroupsDataTable.Columns["File Group"] };
            fileGroupsDataTable.CaseSensitive = true;

            fileGroupsDataTable.DefaultView.Sort = "[Database Name], [File Group]";

            fileGroupsGrid.DataSource = fileGroupsDataTable;

            fileGroupUsageChartsDataSource = new DataView(fileGroupsDataTable, string.Empty, "[Database Name] desc, [File Group] desc", DataViewRowState.CurrentRows);
        }

        private void InitializeTransactionLogsDataSet()
        {
            transactionLogsDataSet = new DataSet();
            
            transactionLogsDataTable = transactionLogsDataSet.Tables.Add("Transaction Logs");
            transactionLogsDataTable.Columns.Add("Database Name", typeof(string));
            transactionLogsDataTable.Columns.Add("Chart Name", typeof(string));
            transactionLogsDataTable.Columns.Add("Files", typeof(int));
            transactionLogsDataTable.Columns.Add("Size (MB)", typeof(decimal));
            transactionLogsDataTable.Columns.Add("Size Used (MB)", typeof(decimal));
            transactionLogsDataTable.Columns.Add("% Used", typeof(float));
            transactionLogsDataTable.Columns.Add("Size Free (MB)", typeof(decimal));
            transactionLogsDataTable.Columns.Add("% Free", typeof(float));
            transactionLogsDataTable.Columns.Add("Auto Growth", typeof(string));
            transactionLogsDataTable.Columns.Add("Potential Growth (MB)", typeof(decimal));
            transactionLogsDataTable.Columns.Add("% Potential Used", typeof(float));
            transactionLogsDataTable.Columns.Add("% Potential Free", typeof(float));
            transactionLogsDataTable.PrimaryKey = new DataColumn[] { transactionLogsDataTable.Columns["Database Name"] };
            transactionLogsDataTable.CaseSensitive = true;
            transactionLogsDataTable.DefaultView.Sort = "[Database Name]";

            logFilesDataTable = transactionLogsDataSet.Tables.Add("Transaction Log Files");
            logFilesDataTable.Columns.Add("Database File Name", typeof(string));
            logFilesDataTable.Columns.Add("Logical File Name", typeof(string));
            logFilesDataTable.Columns.Add("Database Name", typeof(string));
            logFilesDataTable.Columns.Add("File Group", typeof(string));
            logFilesDataTable.Columns.Add("Size (MB)", typeof(decimal));
            logFilesDataTable.Columns.Add("Auto Growth", typeof(string));
            logFilesDataTable.Columns.Add("Potential Growth (MB)", typeof(decimal));
            logFilesDataTable.Columns.Add("Disk Free (MB)", typeof(decimal));
            logFilesDataTable.Columns.Add("File Path", typeof(string));
            logFilesDataTable.PrimaryKey = new DataColumn[] { logFilesDataTable.Columns["Database Name"], logFilesDataTable.Columns["Logical File Name"] };
            logFilesDataTable.CaseSensitive = true;
            logFilesDataTable.DefaultView.Sort = "[Logical File Name]";

            transactionLogsDataSet.Relations.Add("Log Files", 
                                                 transactionLogsDataTable.Columns["Database Name"],
                                                 logFilesDataTable.Columns["Database Name"], false);

            transactionLogsGrid.SetDataBinding(transactionLogsDataSet, "Transaction Logs");
            transactionLogsUsageChartsDataSource = new DataView(transactionLogsDataTable, string.Empty, "[Database Name] desc", DataViewRowState.CurrentRows);
        }

        private void InitializeDisksDataTable()
        {
            disksDataTable = new DataTable();
            disksDataTable.Columns.Add("Drive Letter", typeof(string));
            disksDataTable.Columns.Add("Used (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% Used", typeof(float));
            disksDataTable.Columns.Add("SQL Data Used (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% SQL Data Used", typeof(float));
            disksDataTable.Columns.Add("SQL Data Free (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% SQL Data Free", typeof(float));
            disksDataTable.Columns.Add("SQL Log (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% SQL Log", typeof(float));
            disksDataTable.Columns.Add("Other Files (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% Other Files", typeof(float));
            disksDataTable.Columns.Add("Free Disk (MB)", typeof(decimal));
            disksDataTable.Columns.Add("% Free Disk", typeof(float));

            disksDataTable.PrimaryKey = new DataColumn[] { disksDataTable.Columns["Drive Letter"] };
            disksDataTable.CaseSensitive = true;

            disksDataTable.DefaultView.Sort = "[Drive Letter]";

            diskUsageChartsDataSource = new DataView(disksDataTable, string.Empty, "[Drive Letter] desc", DataViewRowState.CurrentRows);
        }

        private void UpdateFilesChartsDataFilter()
        {
            // Clean up the selection list and build the filter string for the DataView
            StringBuilder filter = new StringBuilder();
            List<string> deleteItems = new List<string>();
            foreach (Pair<string, string> file in selectedFiles.Values)
            {
                if (dataFilesDataTable.Rows.Contains(new string[] { file.First, file.Second }))
                {
                    filter.Append(filter.Length > 0 ? "," : string.Empty);
                    filter.Append("'");
                    filter.Append(string.Format(FORMAT_KEY, file.First, file.Second).Replace("'", "''"));
                    filter.Append("'");
                }
                else
                {
                    deleteItems.Add(string.Format(FORMAT_KEY, file.First, file.Second));
                }
            }
            if (filter.Length > 0)
            {
                filter.Insert(0, "[Database File Name] in (");
                filter.Append(")");
            }

            fileUsageChartsDataSource.RowFilter = filter.ToString();

            // remove any selections that are no longer in the list
            foreach (string item in deleteItems)
            {
                selectedFiles.Remove(item);
            }

            if (selectedFiles.Count > 0)
            {
                fileUsageChart.Visible = true;
            }
            else
            {
                fileUsageChartStatusLabel.Text = SELECT_FILE;
                fileUsageChart.Visible = false;
            }
        }

        private void UpdateFileGroupsChartsDataFilter()
        {
            // Clean up the selection list and build the filter string for the DataView
            StringBuilder filter = new StringBuilder();
            List<string> deleteItems = new List<string>();
            foreach (Pair<string, string> fileGroup in selectedFileGroups.Values)
            {
                if (fileGroupsDataTable.Rows.Contains(new string[] { fileGroup.First, fileGroup.Second }))
                {
                    filter.Append(filter.Length > 0 ? "," : string.Empty);
                    filter.Append("'");
                    filter.Append(string.Format(FORMAT_KEY, fileGroup.First, fileGroup.Second).Replace("'", "''"));
                    filter.Append("'");
                }
                else
                {
                    deleteItems.Add(string.Format(FORMAT_KEY, fileGroup.First, fileGroup.Second));
                }
            }
            if (filter.Length > 0)
            {
                filter.Insert(0, "[Database File Group] in (");
                filter.Append(")");
            }

            fileGroupUsageChartsDataSource.RowFilter = filter.ToString();

            // remove any selections that are no longer in the list
            foreach (string item in deleteItems)
            {
                selectedFileGroups.Remove(item);
            }

            if (selectedFileGroups.Count > 0)
            {
                fileGroupUsageChart.Visible = true;
            }
            else
            {
                fileGroupUsageChartStatusLabel.Text = SELECT_FILEGROUP;
                fileGroupUsageChart.Visible = false;
            }
        }

        private void UpdateLogsChartsDataFilter()
        {
            // Clean up the selection list and build the filter string for the DataView
            StringBuilder filter = new StringBuilder();
            List<string> deleteItems = new List<string>();
            foreach (string logFile in selectedLogs.Values)
            {
                if (transactionLogsDataTable.Rows.Contains(logFile))
                {
                    filter.Append(filter.Length > 0 ? "," : string.Empty);
                    filter.Append("'");
                    filter.Append(logFile.Replace("'", "''"));
                    filter.Append("'");
                }
                else
                {
                    deleteItems.Add(logFile);
                }
            }
            if (filter.Length > 0)
            {
                filter.Insert(0, "[Database Name] in (");
                filter.Append(")");
            }

            transactionLogsUsageChartsDataSource.RowFilter = filter.ToString();

            // remove any selections that are no longer in the list
            foreach (string item in deleteItems)
            {
                selectedLogs.Remove(item);
            }

            if (selectedLogs.Count > 0)
            {
                transactionLogUsageChart.Visible = true;
            }
            else
            {
                transactionLogUsageChartStatusLabel.Text = SELECT_LOG;
                transactionLogUsageChart.Visible = false;
            }
        }

        private void UpdateDisksChartsDataFilter()
        {
            diskUsageChart.Visible = true;
        }

        #endregion

        #region grid

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    selectedColumn.Band.SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedColumn.Band.SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void ToggleGroupByBox(UltraGrid grid)
        {
            if (grid == databasesGrid)
            {
                ToggleGroupByBox();
            }
            else
            {
                grid.DisplayLayout.GroupByBox.Hidden = !grid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups(UltraGrid grid)
        {
            grid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups(UltraGrid grid)
        {
            grid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser(UltraGrid grid)
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(grid);
            dialog.Show(this);
        }

        private void PrintGrid(UltraGrid grid)
        {
            ultraGridPrintDocument.Grid = grid;
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            string title;
            if (grid.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)grid.Tag).Text;
            }
            else if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                title = "Database Files";
            }
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{0} - {1} as of {2}",
                              ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                              title,
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid(UltraGrid grid)
        {
            saveFileDialog.DefaultExt = "xls";
            string title;
            if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                if (grid.Tag is UltraTabPageControl)
                {
                    title = String.Concat("Database ", ((UltraTabPageControl)grid.Tag).Tab.Text);
                }
                else
                {
                    title = "Databases";
                }
            }
            title = ExportHelper.GetValidFileName(title);

            saveFileDialog.FileName = title;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (grid == databasesGrid)
                {
                    // Fix the icon columns to export as text values with a heading
                    databasesGrid.DisplayLayout.Bands[0].Columns["State"].Header.Caption = "State";
                    databasesGrid.DisplayLayout.ValueLists["stateValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;
                }

                try
                {
                    ultraGridExcelExporter.Export(grid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                if (grid == databasesGrid)
                {
                    databasesGrid.DisplayLayout.Bands[0].Columns["State"].Header.Caption = string.Empty;
                    databasesGrid.DisplayLayout.ValueLists["stateValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
                }
            }
        }

        #endregion

        #region Charts

        private void InitializeCharts()
        {
            InitializeFilesUsageChart();
            InitializeFileGroupsUsageChart();
            InitializeLogsUsageChart();
            InitializeDisksUsageChart();
        }

        private void InitializeFilesUsageChart()
        {
            fileUsageChart.Tag = fileUsageChartSelectionDropDownButton;
            fileUsageChart.Printer.Orientation = PageOrientation.Landscape;
            fileUsageChart.Printer.Compress = true;
            fileUsageChart.Printer.ForceColors = true;
            fileUsageChart.Printer.Document.DocumentName = "File Usage Chart";
            fileUsageChart.ToolBar.RemoveAt(0);
            fileUsageChart.DataSource = fileUsageChartsDataSource;

            ConfigureFileUsageChart("Current Megabytes");
            fileUsageChart.Visible = false;
        }

        private void ConfigureFileUsageChart(string usageType)
        {
            fileUsageChartSelectionDropDownButton.Text = string.Format(FORMAT_FILES_CHARTTYPE, usageType);

            fileUsageChart.SuspendLayout();
            fileUsageChart.DataSourceSettings.Fields.Clear();

            FieldMap fileFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
            fileFieldMap.DisplayName = "File";
            FieldMap UsedFieldMap;
            FieldMap FreeFieldMap;

            switch (usageType)
            {
                case "Current Megabytes":
                    UsedFieldMap = new FieldMap("Size Used (MB)", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Used";
                    FreeFieldMap = new FieldMap("Size Free (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    fileUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileUsageChart.AxisY.AutoScale = true;
                    fileUsageChart.AxisY.DataFormat.Decimals = 2;
                    fileUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Current Percent":
                    UsedFieldMap = new FieldMap("% Current Used", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Used";
                    FreeFieldMap = new FieldMap("% Current Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    fileUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileUsageChart.AxisY.Min = 0;
                    fileUsageChart.AxisY.Max = 100;
                    fileUsageChart.AxisY.DataFormat.Decimals = 0;
                    fileUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
                case "Potential Megabytes":
                    UsedFieldMap = new FieldMap("Size (MB)", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("Potential Growth (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    fileUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileUsageChart.AxisY.AutoScale = true;
                    fileUsageChart.AxisY.DataFormat.Decimals = 2;
                    fileUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Potential Percent":
                    UsedFieldMap = new FieldMap("% Potential Used", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("% Potential Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    fileUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileUsageChart.AxisY.Min = 0;
                    fileUsageChart.AxisY.Max = 100;
                    fileUsageChart.AxisY.DataFormat.Decimals = 0;
                    fileUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            fileUsageChart.DataSourceSettings.ReloadData();
            fileUsageChart.Invalidate();
            fileUsageChart.ResumeLayout();
        }

        private void InitializeFileGroupsUsageChart()
        {
            fileGroupUsageChart.Tag = fileGroupUsageChartSelectionDropDownButton;
            fileGroupUsageChart.Printer.Orientation = PageOrientation.Landscape;
            fileGroupUsageChart.Printer.Compress = true;
            fileGroupUsageChart.Printer.ForceColors = true;
            fileGroupUsageChart.Printer.Document.DocumentName = "File Group Usage Chart";
            fileGroupUsageChart.ToolBar.RemoveAt(0);
            fileGroupUsageChart.DataSource = fileGroupUsageChartsDataSource;

            ConfigureFileGroupsUsageChart("Current Megabytes");
            fileGroupUsageChart.Visible = false;
        }

        private void ConfigureFileGroupsUsageChart(string usageType)
        {
            fileGroupUsageChartSelectionDropDownButton.Text = string.Format(FORMAT_GROUPS_CHARTTYPE, usageType);

            fileGroupUsageChart.SuspendLayout();
            fileGroupUsageChart.DataSourceSettings.Fields.Clear();

            FieldMap fileFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
            fileFieldMap.DisplayName = "File Group";
            FieldMap UsedFieldMap;
            FieldMap DataFieldMap;
            FieldMap IndexesFieldMap;
            FieldMap TextFieldMap;
            FieldMap FreeFieldMap;

            switch (usageType)
            {
                case "Current Megabytes":
                    DataFieldMap = new FieldMap("Tables (MB)", FieldUsage.Value);
                    DataFieldMap.DisplayName = "Tables";
                    IndexesFieldMap = new FieldMap("Indexes (MB)", FieldUsage.Value);
                    IndexesFieldMap.DisplayName = "Indexes";
                    TextFieldMap = new FieldMap("Text (MB)", FieldUsage.Value);
                    TextFieldMap.DisplayName = "Text";
                    FreeFieldMap = new FieldMap("Free (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    fileGroupUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        DataFieldMap,
                        IndexesFieldMap,
                        TextFieldMap,
                        FreeFieldMap});
                    fileGroupUsageChart.AxisY.AutoScale = true;
                    fileGroupUsageChart.AxisY.DataFormat.Decimals = 2;
                    fileGroupUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Current Percent":
                    DataFieldMap = new FieldMap("% Tables", FieldUsage.Value);
                    DataFieldMap.DisplayName = "Tables";
                    IndexesFieldMap = new FieldMap("% Indexes", FieldUsage.Value);
                    IndexesFieldMap.DisplayName = "Indexes";
                    TextFieldMap = new FieldMap("% Text", FieldUsage.Value);
                    TextFieldMap.DisplayName = "Text";
                    FreeFieldMap = new FieldMap("% Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    fileGroupUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        DataFieldMap,
                        IndexesFieldMap,
                        TextFieldMap,
                        FreeFieldMap});
                    fileGroupUsageChart.AxisY.Min = 0;
                    fileGroupUsageChart.AxisY.Max = 100;
                    fileGroupUsageChart.AxisY.DataFormat.Decimals = 0;
                    fileGroupUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
                case "Potential Megabytes":
                    UsedFieldMap = new FieldMap("Size (MB)", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("Potential Growth (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    fileGroupUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileGroupUsageChart.AxisY.AutoScale = true;
                    fileGroupUsageChart.AxisY.DataFormat.Decimals = 2;
                    fileGroupUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Potential Percent":
                    UsedFieldMap = new FieldMap("% Potential Used", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("% Potential Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    fileGroupUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    fileGroupUsageChart.AxisY.Min = 0;
                    fileGroupUsageChart.AxisY.Max = 100;
                    fileGroupUsageChart.AxisY.DataFormat.Decimals = 0;
                    fileGroupUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            fileGroupUsageChart.DataSourceSettings.ReloadData();
            fileGroupUsageChart.Invalidate();
            fileGroupUsageChart.ResumeLayout();
        }

        private void InitializeLogsUsageChart()
        {
            transactionLogUsageChart.Tag = transactionLogUsageChartSelectionDropDownButton;
            transactionLogUsageChart.Printer.Orientation = PageOrientation.Landscape;
            transactionLogUsageChart.Printer.Compress = true;
            transactionLogUsageChart.Printer.ForceColors = true;
            transactionLogUsageChart.Printer.Document.DocumentName = "Transaction Log Usage Chart";
            transactionLogUsageChart.ToolBar.RemoveAt(0);
            transactionLogUsageChart.DataSource = transactionLogsUsageChartsDataSource;

            ConfigureTransactionLogUsageChart("Current Megabytes");
            transactionLogUsageChart.Visible = false;
        }

        private void ConfigureTransactionLogUsageChart(string usageType)
        {
            transactionLogUsageChartSelectionDropDownButton.Text = string.Format(FORMAT_LOGS_CHARTTYPE, usageType);

            transactionLogUsageChart.SuspendLayout();
            transactionLogUsageChart.DataSourceSettings.Fields.Clear();

            FieldMap fileFieldMap = new FieldMap("Chart Name", FieldUsage.Label);
            fileFieldMap.DisplayName = "Database";
            FieldMap UsedFieldMap;
            FieldMap FreeFieldMap;

            switch (usageType)
            {
                case "Current Megabytes":
                    UsedFieldMap = new FieldMap("Size Used (MB)", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Used";
                    FreeFieldMap = new FieldMap("Size Free (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    transactionLogUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    transactionLogUsageChart.AxisY.AutoScale = true;
                    transactionLogUsageChart.AxisY.DataFormat.Decimals = 2;
                    transactionLogUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Current Percent":
                    UsedFieldMap = new FieldMap("% Used", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Used";
                    FreeFieldMap = new FieldMap("% Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Free";

                    transactionLogUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    transactionLogUsageChart.AxisY.Min = 0;
                    transactionLogUsageChart.AxisY.Max = 100;
                    transactionLogUsageChart.AxisY.DataFormat.Decimals = 0;
                    transactionLogUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
                case "Potential Megabytes":
                    UsedFieldMap = new FieldMap("Size (MB)", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("Potential Growth (MB)", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    transactionLogUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    transactionLogUsageChart.AxisY.AutoScale = true;
                    transactionLogUsageChart.AxisY.DataFormat.Decimals = 2;
                    transactionLogUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Potential Percent":
                    UsedFieldMap = new FieldMap("% Potential Used", FieldUsage.Value);
                    UsedFieldMap.DisplayName = "Allocated";
                    FreeFieldMap = new FieldMap("% Potential Free", FieldUsage.Value);
                    FreeFieldMap.DisplayName = "Potential";

                    transactionLogUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        fileFieldMap,
                        UsedFieldMap,
                        FreeFieldMap});
                    transactionLogUsageChart.AxisY.Min = 0;
                    transactionLogUsageChart.AxisY.Max = 100;
                    transactionLogUsageChart.AxisY.DataFormat.Decimals = 0;
                    transactionLogUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            transactionLogUsageChart.DataSourceSettings.ReloadData();
            transactionLogUsageChart.Invalidate();
            transactionLogUsageChart.ResumeLayout();
        }

        private void InitializeDisksUsageChart()
        {
            diskUsageChart.Tag = diskUsageChartSelectionDropDownButton;
            diskUsageChart.Printer.Orientation = PageOrientation.Landscape;
            diskUsageChart.Printer.Compress = true;
            diskUsageChart.Printer.ForceColors = true;
            diskUsageChart.Printer.Document.DocumentName = "Current Disks Usage Chart";
            diskUsageChart.ToolBar.RemoveAt(0);
            diskUsageChart.DataSource = diskUsageChartsDataSource;

            ConfigureDiskUsageChart("Megabytes");
            diskUsageChart.Visible = false;
        }

        private void ConfigureDiskUsageChart(string usageType)
        {
            currentDiskUsageType = usageType;            

            diskUsageChartSelectionDropDownButton.Text = string.Format(FORMAT_DISKS_CHARTTYPE, usageType);

            diskUsageChart.SuspendLayout();
            diskUsageChart.LegendBox.Width = 125;
            diskUsageChart.DataSourceSettings.Fields.Clear();

            FieldMap fileFieldMap = new FieldMap("Drive Letter", FieldUsage.Label);
            fileFieldMap.DisplayName = "Drive";
            FieldMap UsedFieldMap;
            FieldMap DataUsedFieldMap;
            FieldMap DataFreeFieldMap;
            FieldMap LogUsedFieldMap;
            FieldMap OtherUsedFieldMap;
            FieldMap FreeFieldMap;

            switch (usageType)
            {
                case "Megabytes":
                    if (selectedDatabases.Count == 0)
                    {
                        UsedFieldMap = new FieldMap("Used (MB)", FieldUsage.Value);
                        UsedFieldMap.DisplayName = "Used";
                        FreeFieldMap = new FieldMap("Free Disk (MB)", FieldUsage.Value);
                        FreeFieldMap.DisplayName = "Free";

                        diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            UsedFieldMap,
                            FreeFieldMap});

                        //if (diskUsageChart.Series.Count > 1)
                        //{
                        //    diskUsageChart.Series[0].Color = Color.FromArgb(118, 200, 45);
                        //    diskUsageChart.Series[1].Color = Color.FromArgb(236, 179, 70);
                        //}
                    }
                    else
                    {
                        DataUsedFieldMap = new FieldMap("SQL Data Used (MB)", FieldUsage.Value);
                        DataUsedFieldMap.DisplayName = "SQL Data Used";
                        DataFreeFieldMap = new FieldMap("SQL Data Free (MB)", FieldUsage.Value);
                        DataFreeFieldMap.DisplayName = "SQL Data Free";
                        LogUsedFieldMap = new FieldMap("SQL Log (MB)", FieldUsage.Value);
                        LogUsedFieldMap.DisplayName = "SQL Log";
                        OtherUsedFieldMap = new FieldMap("Other Files (MB)", FieldUsage.Value);
                        OtherUsedFieldMap.DisplayName = "Other Files";
                        FreeFieldMap = new FieldMap("Free Disk (MB)", FieldUsage.Value);
                        FreeFieldMap.DisplayName = "Free Disk";

                        if (showDiskOtherFiles)
                        {
                            diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            OtherUsedFieldMap,
                            FreeFieldMap});
                        }
                        else
                        {
                            diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            FreeFieldMap});
                        }

                        //if (diskUsageChart.Series.Count > 1)
                        //{
                        //    diskUsageChart.Series[0].Color = Color.FromArgb(38, 100, 193);
                        //    diskUsageChart.Series[1].Color = Color.FromArgb(199, 56, 0);
                        //}
                    }
                    diskUsageChart.AxisY.AutoScale = true;
                    diskUsageChart.AxisY.DataFormat.Decimals = 2;
                    diskUsageChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Percent":
                    if (selectedDatabases.Count == 0)
                    {
                        UsedFieldMap = new FieldMap("% Used", FieldUsage.Value);
                        UsedFieldMap.DisplayName = "Used";
                        FreeFieldMap = new FieldMap("% Free Disk", FieldUsage.Value);
                        FreeFieldMap.DisplayName = "Free";

                        diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            UsedFieldMap,
                            FreeFieldMap});
                        //if (diskUsageChart.Series.Count > 1)
                        //{
                        //    diskUsageChart.Series[0].Color = Color.FromArgb(118, 200, 45);
                        //    diskUsageChart.Series[1].Color = Color.FromArgb(236, 179, 70);
                        //}
                    }
                    else
                    {
                        DataUsedFieldMap = new FieldMap("% SQL Data Used", FieldUsage.Value);
                        DataUsedFieldMap.DisplayName = "SQL Data Used";
                        DataFreeFieldMap = new FieldMap("% SQL Data Free", FieldUsage.Value);
                        DataFreeFieldMap.DisplayName = "SQL Data Free";
                        LogUsedFieldMap = new FieldMap("% SQL Log", FieldUsage.Value);
                        LogUsedFieldMap.DisplayName = "SQL Log";
                        OtherUsedFieldMap = new FieldMap("% Other Files", FieldUsage.Value);
                        OtherUsedFieldMap.DisplayName = "Other Files";
                        FreeFieldMap = new FieldMap("% Free Disk", FieldUsage.Value);
                        FreeFieldMap.DisplayName = "Free Disk";

                        if (showDiskOtherFiles)
                        {
                            diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            OtherUsedFieldMap,
                            FreeFieldMap});
                        }
                        else
                        {
                            diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            FreeFieldMap});
                        }

                        //if (diskUsageChart.Series.Count > 1)
                        //{
                        //    diskUsageChart.Series[0].Color = Color.FromArgb(38, 100, 193);
                        //    diskUsageChart.Series[1].Color = Color.FromArgb(199, 56, 0);
                        //}
                    }
                    diskUsageChart.AxisY.Min = 0;
                    diskUsageChart.AxisY.Max = 100;
                    diskUsageChart.AxisY.DataFormat.Decimals = 0;
                    diskUsageChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            diskUsageChart.DataSourceSettings.ReloadData();
            diskUsageChart.Invalidate();
            diskUsageChart.ResumeLayout();
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

        private void SelectChartDataColumns(Chart chart)
        {
            if (diskDataColumns.Count == 0)
            {
                foreach (DataRow row in diskUsageChartsDataSource.Table.Rows)
                    diskDataColumns.Add(row["Drive Letter"].ToString(), true);
            }

            List<string> drives = new List<string>();

            using (CheckedListDialog<object> dialog = new CheckedListDialog<object>())
            {
                dialog.Text = "Select data to display";

                foreach (string drivename in diskDataColumns.Keys)
                {
                    dialog.AddItem(drivename, drivename, diskDataColumns[drivename]);
                    drives.Add(drivename);
                }

                if (dialog.ShowDialog(this) == DialogResult.Cancel)
                    return;

                foreach (string drivename in drives)
                    diskDataColumns[drivename] = false;

                foreach (object drive in dialog.SelectedItems)
                    diskDataColumns[(string)drive] = true;
            }

            StringBuilder filter = new StringBuilder();
            int i = 0;

            foreach (string drivename in diskDataColumns.Keys)
            {
                if (!diskDataColumns[drivename])
                    continue;

                if(i == 0)
                    filter.Append(string.Format("[Drive Letter] = '{0}'", drivename));
                else
                    filter.Append(string.Format("or [Drive Letter] = '{0}'", drivename));

                i++;
            }

            diskUsageChartsDataSource.RowFilter = filter.ToString();
            
            // update chart
            ConfigureDiskUsageChart(currentDiskUsageType);            
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

        private void MaximizeChart(Panel chartPanel, Control chartParent, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            splitContainer1.Visible = false;
            chartParent.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            contentContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart(Panel chartPanel, Control chartParent, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            contentContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            chartParent.Controls.Add(chartPanel);
            splitContainer1.Visible = true;
        }

        #endregion

        #endregion

        #region events

        #region Splitters

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            FocusLastFocusedControl();
        }

        private void filesSplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void filesSplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            FocusLastFocusedControl();
        }

        private void fileGroupsSplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void fileGroupsSplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            FocusLastFocusedControl();
        }

        private void transactionLogSplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void transactionLogSplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            FocusLastFocusedControl();
        }

        #endregion

        #region tabs

        private void tabControl_ActiveTabChanged(object sender, ActiveTabChangedEventArgs e)
        {
            ShowStatus();
        }

        #endregion

        #region toolbars

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(
                        chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;

                    ((ButtonTool)((PopupMenuTool)e.Tool).Tools["selectColumnsButton"]).SharedProps.Visible = (chart == diskUsageChart);

                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = selectedGrid.Rows.Count > 0 && selectedGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                // filesGrid
                // databasesGrid
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
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
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    if (selectedGrid != null)
                    {
                        ShowColumnChooser(selectedGrid);
                    }
                    break;
                case "toggleGroupByBoxButton":
                    if (selectedGrid != null)
                    {
                        ToggleGroupByBox(selectedGrid);
                    }
                    break;
                case "printGridButton":
                    if (selectedGrid != null)
                    {
                        PrintGrid(selectedGrid);
                    }
                    break;
                case "exportGridButton":
                    if (selectedGrid != null)
                    {
                        SaveGrid(selectedGrid);
                    }
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
                case "viewSummaryButton":
                    ShowSummaryView();
                    break;
                case "viewBackupRestoreHistoryButton":
                    ShowBackupRestoreHistoryView();
                    break;
                case "collapseAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        CollapseAllGroups(selectedGrid);
                    }
                    break;
                case "expandAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        ExpandAllGroups(selectedGrid);
                    }
                    break;
                case "selectColumnsButton":
                    SelectChartDataColumns(contextMenuSelectedChart);
                    break;
                case "showFileActivity":
                    ShowFileActivity();
                    break;
            }
        }

        #endregion

        #region grid

        private void databasesGrid_BeforeSelectChange(object sender, BeforeSelectChangeEventArgs e)
        {
            if (e.NewSelections.Rows.Count > 0)
            {
                List<UltraGridRow> rows = new List<UltraGridRow>();
                bool incompatibleFound = false;
                foreach (UltraGridRow row in e.NewSelections.Rows)
                {
                    if (row.IsDataRow
                        && ((DataRowView)row.ListObject)["Version Compatibility"] != System.DBNull.Value
                        && (float)((DataRowView)row.ListObject)["Version Compatibility"] < 6.5f)
                    {
                        incompatibleFound = true;
                        ApplicationMessageBox.ShowWarning(this, String.Format(DB_NOT_SUPPORTED, ((DataRowView)row.ListObject)["Database Name"]));
                    }
                    else
                    {
                        rows.Add(row);
                    }
                }
                if (incompatibleFound)
                {
                    e.Cancel = true;
                    foreach (UltraGridRow row in rows)
                    {
                        row.Selected = true;
                    }
                }
            }
        }

        private void databasesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (!databaseFilterSelectionChanging)
            {
                selectedDatabases.Clear();
                if (databasesGrid.Rows.Count > 0 && databasesGrid.Selected.Rows.Count > 0)
                {
                    foreach (UltraGridRow row in databasesGrid.Selected.Rows)
                    {
                        if (row.IsDataRow)
                        {
                            selectedDatabases.Add((string) row.Cells["Database Name"].Value,
                                                  (string) row.Cells["Database Name"].Value);
                        }
                    }
                }
                selectionChanged = true;
                tabControlStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                tabControl.Visible = false;

                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void databasesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = databasesGrid;
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

        private void filesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedFiles.Clear();
            if (filesGrid.Rows.Count > 0 && filesGrid.Selected.Rows.Count > 0)
            {
                foreach (UltraGridRow row in filesGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedFiles.Add(string.Format(FORMAT_KEY, (string)row.Cells["Database Name"].Value, (string)row.Cells["Logical File Name"].Value),
                            new Pair<string, string>((string)row.Cells["Database Name"].Value, (string)row.Cells["Logical File Name"].Value));
                    }
                }
            }

            UpdateFilesChartsDataFilter();
            ConfigureFileUsageChart(fileUsageChartSelectionDropDownButton.Text.Substring(fileUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
        }

        private void filesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = filesGrid;
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
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void fileGroupsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedFileGroups.Clear();
            if (fileGroupsGrid.Rows.Count > 0 && fileGroupsGrid.Selected.Rows.Count > 0)
            {
                foreach (UltraGridRow row in fileGroupsGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedFileGroups.Add(string.Format(FORMAT_KEY, (string)row.Cells["Database Name"].Value, (string)row.Cells["File Group"].Value),
                            new Pair<string, string>((string)row.Cells["Database Name"].Value, (string)row.Cells["File Group"].Value));
                    }
                }
            }

            UpdateFileGroupsChartsDataFilter();
            ConfigureFileGroupsUsageChart(fileGroupUsageChartSelectionDropDownButton.Text.Substring(fileGroupUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
        }

        private void fileGroupsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = fileGroupsGrid;
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
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void transactionLogsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedLogs.Clear();
            if (transactionLogsGrid.Rows.Count > 0 && transactionLogsGrid.Selected.Rows.Count > 0)
            {
                foreach (UltraGridRow row in transactionLogsGrid.Selected.Rows)
                {
                    if (row.IsDataRow && !selectedLogs.ContainsKey((string)row.Cells["Database Name"].Value))
                    {
                        selectedLogs.Add((string)row.Cells["Database Name"].Value,
                            (string)row.Cells["Database Name"].Value);
                    }
                }
            }

            UpdateLogsChartsDataFilter();
            ConfigureTransactionLogUsageChart(transactionLogUsageChartSelectionDropDownButton.Text.Substring(transactionLogUsageChartSelectionDropDownButton.Text.IndexOf(": ") + 2));
        }

        private void transactionLogsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = transactionLogsGrid;
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
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        #endregion

        #region Charts

        private void maximizeFileUsageChartButton_Click(object sender, System.EventArgs e)
        {
            MaximizeChart(fileUsagePanel, filesSplitContainer.Panel2, maximizeFileUsageChartButton, restoreFileUsageChartButton);
        }

        private void restoreFileUsageChartButton_Click(object sender, System.EventArgs e)
        {
            RestoreChart(fileUsagePanel, filesSplitContainer.Panel2, maximizeFileUsageChartButton, restoreFileUsageChartButton);
        }

        private void maximizeFileGroupUsageChartButton_Click(object sender, System.EventArgs e)
        {
            MaximizeChart(fileGroupUsagePanel, fileGroupsSplitContainer.Panel2, maximizeFileGroupUsageChartButton, restoreFileGroupUsageChartButton);
        }

        private void restoreFileGroupUsageChartButton_Click(object sender, System.EventArgs e)
        {
            RestoreChart(fileGroupUsagePanel, fileGroupsSplitContainer.Panel2, maximizeFileGroupUsageChartButton, restoreFileGroupUsageChartButton);
        }

        private void maximizeTransactionLogUsageChartButton_Click(object sender, System.EventArgs e)
        {
            MaximizeChart(transactionLogUsagePanel, transactionLogSplitContainer.Panel2, maximizeTransactionLogUsageChartButton, restoreTransactionLogUsageChartButton);
        }

        private void restoreTransactionLogUsageChartButton_Click(object sender, System.EventArgs e)
        {
            RestoreChart(transactionLogUsagePanel, transactionLogSplitContainer.Panel2, maximizeTransactionLogUsageChartButton, restoreTransactionLogUsageChartButton);
        }

        private void maximizeDiskUsageChartButton_Click(object sender, System.EventArgs e)
        {
            MaximizeChart(diskUsagePanel, disksTabPage, maximizeDiskUsageChartButton, restoreDiskUsageChartButton);
        }

        private void restoreDiskUsageChartButton_Click(object sender, System.EventArgs e)
        {
            RestoreChart(diskUsagePanel, disksTabPage, maximizeDiskUsageChartButton, restoreDiskUsageChartButton);
        }
        private void showFileUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureFileUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showFileUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureFileUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialFileGrowthInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureFileUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialFileGrowthInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureFileUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentFileGroupUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureFileGroupsUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentFileGroupUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureFileGroupsUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialFileGroupGrowthInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureFileGroupsUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialFileGroupGrowthInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureFileGroupsUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentTransactionLogUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureTransactionLogUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentTransactionLogUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureTransactionLogUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialTransactionLogUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureTransactionLogUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showPotentialTransactionLogUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureTransactionLogUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentDiskUsageInMegabytesButton_Click(object sender, EventArgs e)
        {
            ConfigureDiskUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void showCurrentDiskUsageInPercentButton_Click(object sender, EventArgs e)
        {
            ConfigureDiskUsageChart(((ToolStripDropDownItem)sender).Text);
        }

        private void chart_GetTip(object sender, GetTipEventArgs e)
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
                        e.Text = (string)((DataView)chart.DataSource)[currentPoint]["Database File Name"];
                    }
                }
            }
        }

        private void fileGroupUsageChart_GetTip(object sender, GetTipEventArgs e)
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
                        e.Text = (string)((DataView)chart.DataSource)[currentPoint]["Database File Group"];
                    }
                }
            }
        }

        private void transactionLogUsageChart_GetTip(object sender, GetTipEventArgs e)
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

        private void diskUsageChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = Math.Min(125, chart.Width / 2);

            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
            }
        }

        private void chart_MouseMove(object sender, HitTestEventArgs e)
        {
            // this is a common event handler for all charts
            chartMouseX = e.X;
            chartMouseY = e.Y;
        }

        #endregion

        private void DatabasesFilesView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

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
                if (databasesFilterComboBox.SelectedItem.DisplayText == AllDatabasesItemText)
                {
                    selectedDatabaseFilter = null;
                    currentDataTable.DefaultView.RowFilter = null;
                }
                else if (databasesFilterComboBox.Items.Count > 1)
                {
                    databaseFilterSelectionChanging = true;
                    selectedDatabaseFilter = databasesFilterComboBox.SelectedItem.DisplayText;
                    currentDataTable.DefaultView.RowFilter =
                        string.Format("[Database Name] = '{0}'", selectedDatabaseFilter.Replace("'", "''"));
                    databasesGrid.Selected.Rows.Clear();
                    UltraGridRow[] nonGroupByRows = databasesGrid.Rows.GetAllNonGroupByRows();
                    databaseFilterSelectionChanging = false;

                    if (nonGroupByRows.Length > 0)
                    {
                        databasesGrid.Selected.Rows.Add(nonGroupByRows[0]);
                    }
                }

                currentDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                ShowStatus();
                databasesGrid.Focus();
            }
        }

        #endregion

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void noFileSystemObjectDataLabel_DoubleClick(object sender, EventArgs e)
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EnablingOsMetricsMonitoring);
        }

        private void noFileSystemObjectDataLabel_Click(object sender, EventArgs e)
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EnablingOsMetricsMonitoring);
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
