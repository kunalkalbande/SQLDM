using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Messages;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
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
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using TableAction = Idera.SQLdm.DesktopClient.Objects.TableActionObject.TableAction;
using TableActionStatus = Idera.SQLdm.DesktopClient.Objects.TableActionObject.TableActionStatus;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesTablesIndexesView : ServerBaseView, IDatabasesView
    {
        private const string DB_NOT_SUPPORTED = @"Database compatibility level 60 is not supported";

        private DateTime? historicalSnapshotDateTime;
        private DataTable tablesDataTable;
        private DataTable dependenciesDataTable;
        private DataTable indexesDataTable;
        private DataTable indexStatisticsColumnsDataTable;
        private DataTable indexStatisticsDataDistributionTable;
        private bool includeSystemDatabases = true;
        private bool includeSystemTables = false;
        private string selectedDatabase;
        private string selectedDatabaseArgument = null;
        private int selectedTableId;
        private string selectedTableNameArgument = null;
        private string selectedIndex;
        private bool availableDatabasesInitialized = false;
        private bool tablesGridInitialized = false;
        private bool indexesGridInitialized = false;
        private static readonly object updateLock = new object();
        private UltraGridColumn selectedColumn = null;
        private Control focused = null;
        private readonly Dictionary<int, DataRow> tableDataRowLookupTableById = new Dictionary<int, DataRow>();
        private readonly Dictionary<string, DataRow> tableDataRowLookupTableByQualifiedName = new Dictionary<string, DataRow>();
        private UltraGrid selectedGrid = null;
        private Chart contextMenuSelectedChart = null;
        private TableSummary currentSnapshot = null;
        private TableDetail currentTableDetailsSnapshot = null;
        private Dictionary<Pair<string, int>, TableActionObject> tableRebuilds = new Dictionary<Pair<string, int>, TableActionObject>();
        private List<Pair<string, int>> pendingTableRebuilds = new List<Pair<string, int>>();
        private List<Pair<string, int>> pendingTableUpdates = new List<Pair<string, int>>();
        private bool userTablesInList = false;
        private Stopwatch availableDatabasesRefreshStopwatch = new Stopwatch();
        private Stopwatch tableDetailsRefreshStopwatch = new Stopwatch();
        private Stopwatch indexStatisticsRefreshStopwatch = new Stopwatch();
        private AlertConfigurationItem alertConfigItem = null;

        private readonly int MaxHoursForShadingAlerts = 12;

        private string dependencyType
        {
            get
            {
                if (dependenciesReferencesViewRadioButton.Checked)
                {
                    return dependenciesReferencesViewRadioButton.Text;
                }
                else if (dependenciesReferencedByViewRadioButton.Checked)
                {
                    return dependenciesReferencedByViewRadioButton.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        private string statisticsType
        {
            get
            {
                if (indexStatisticsColumnsViewRadioButton.Checked)
                {
                    return indexStatisticsColumnsViewRadioButton.Text;
                }
                else if (indexStatisticsDataDistributionViewRadioButton.Checked)
                {
                    return indexStatisticsDataDistributionViewRadioButton.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

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
        private int lastMainSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastDependenciesGridSettings = null;
        private GridSettings lastIndexesGridSettings = null;
        private GridSettings lastColumnStatisticsGridSettings = null;
        private GridSettings lastDataDistributionGridSettings = null;
        private bool lastDetailsVisible = true;
        private int lastIndexesTab = 0;
        private string lastDependencyType = string.Empty;
        private string lastStatisticsType = string.Empty;

        public event EventHandler SystemDatabasesFilterChanged;
        public event EventHandler SystemTablesFilterChanged;
        public event EventHandler DetailsPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler ActionsAllowedChanged;

        public DatabasesTablesIndexesView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            InitializeDataTables();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            HideFocusRectangleDrawFilter hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();
            availableDatabasesComboBox.DrawFilter = hideFocusRectangleDrawFilter;
            tablesGrid.DrawFilter = hideFocusRectangleDrawFilter;
            tableDetailsTabControl.DrawFilter = hideFocusRectangleDrawFilter;
            dependenciesGrid.DrawFilter = hideFocusRectangleDrawFilter;
            indexesGrid.DrawFilter = hideFocusRectangleDrawFilter;
            indexStatisticsIndexSelectionComboBox.DrawFilter = hideFocusRectangleDrawFilter;
            indexStatisticsColumnsGrid.DrawFilter = hideFocusRectangleDrawFilter;
            indexStatisticsDataDistributionGrid.DrawFilter = hideFocusRectangleDrawFilter;

            ChartFxExtensions.SetContextMenu(tableSizeChart, toolbarsManager);
            tableSizeChart.Printer.Orientation = PageOrientation.Landscape;
            tableSizeChart.Printer.Compress = true;
            tableSizeChart.Printer.ForceColors = true;
            tableSizeChart.Printer.Document.DocumentName = "Table Size";
            tableSizeChart.ToolTipFormat = "%s\n%vKB";
            tableSizeChart.ToolBar.RemoveAt(0);

            ShowTablesGridStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
            ShowTableDetailsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);

            RefreshAvailableDatabases();
            AdaptFontSize();
        }

        public string SelectedDatabaseFilter
        {
            get { return selectedDatabase; }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public override void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesTablesAndIndexesView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is string)
            {
                selectedDatabaseArgument = argument as string;
                selectedTableNameArgument = null;
            }
            else if (argument is MonitoredObjectName && ((MonitoredObjectName)argument).IsDatabase)
            {
                selectedDatabaseArgument = ((MonitoredObjectName)argument).DatabaseName;
                selectedTableNameArgument = null;
            }
            else if (argument is MonitoredObjectName && ((MonitoredObjectName)argument).IsTable)
            {
                selectedDatabaseArgument = ((MonitoredObjectName)argument).DatabaseName;
                selectedTableNameArgument = ((MonitoredObjectName)argument).TableName;
            }
            else
            {
                selectedDatabaseArgument = null;
                selectedTableNameArgument = null;
            }

            if (selectedDatabaseArgument != null)
            {
                if (selectedDatabase == selectedDatabaseArgument)
                {
                    selectedDatabaseArgument = null;
                }
                else
                {
                    int index = FindDatabaseIndex(selectedDatabaseArgument);
                    if (index >= 0)
                    {
                        availableDatabasesComboBox.SelectedIndex = index;
                        selectedDatabaseArgument = null;
                        UpdateSelectedTableId(-1);
                        tablesGrid.Selected.Rows.Clear();
                    }
                    else
                        availableDatabasesInitialized = false;
                }

                if (selectedDatabaseArgument == null && selectedTableNameArgument != null)
                {
                    bool found = false;
                    foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((string)row.Cells["Qualified Table Name"].Value == selectedTableNameArgument)
                        {
                            row.Selected = true;
                            tablesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        UpdateSelectedTableId(-1);
                        tablesGrid.Selected.Rows.Clear();
                    }

                    if (selectedTableId != -1)
                    {
                        selectedTableNameArgument = null;
                    }
                }
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance = splitContainer.Height - Settings.Default.DatabasesTablesViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            string gridType = Settings.Default.DatabasesTablesViewDependencyType;
            if (gridType != null && gridType.Length > 0)
            {
                if (gridType == dependenciesReferencesViewRadioButton.Text)
                {
                    dependenciesReferencesViewRadioButton.Checked = true;
                    lastDependencyType = gridType;
                }
                if (gridType == dependenciesReferencedByViewRadioButton.Text)
                {
                    dependenciesReferencedByViewRadioButton.Checked = true;
                    lastDependencyType = gridType;
                }
                else if (dependenciesReferencesViewRadioButton.Checked)
                {
                    lastDependencyType = dependenciesReferencesViewRadioButton.Text;
                }
                else
                {
                    lastDependencyType = dependenciesReferencedByViewRadioButton.Text;
                }
            }

            gridType = Settings.Default.DatabasesTablesViewStatisticsType;
            if (gridType != null && gridType.Length > 0)
            {
                if (gridType == indexStatisticsColumnsViewRadioButton.Text)
                {
                    indexStatisticsColumnsViewRadioButton.Checked = true;
                    lastDependencyType = gridType;
                }
                if (gridType == indexStatisticsDataDistributionViewRadioButton.Text)
                {
                    indexStatisticsDataDistributionViewRadioButton.Checked = true;
                    lastDependencyType = gridType;
                }
                else if (indexStatisticsColumnsViewRadioButton.Checked)
                {
                    lastDependencyType = indexStatisticsColumnsViewRadioButton.Text;
                }
                else
                {
                    lastDependencyType = indexStatisticsDataDistributionViewRadioButton.Text;
                }
            }

            lastDetailsVisible =
                DetailsPanelVisible = Settings.Default.DatabasesTablesViewDetailsVisible;

            if (Settings.Default.DatabasesTablesViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.DatabasesTablesViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, tablesGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            if (Settings.Default.DatabasesTablesViewDependenciesGrid is GridSettings)
            {
                lastDependenciesGridSettings = Settings.Default.DatabasesTablesViewDependenciesGrid;
                GridSettings.ApplySettingsToGrid(lastDependenciesGridSettings, dependenciesGrid);
            }

            if (Settings.Default.DatabasesTablesViewIndexesGrid is GridSettings)
            {
                lastIndexesGridSettings = Settings.Default.DatabasesTablesViewIndexesGrid;
                GridSettings.ApplySettingsToGrid(lastIndexesGridSettings, indexesGrid);
                if (Settings.Default.UpgradeIndexesDataTableRemoveRows)
                {
                    indexesGrid.DisplayLayout.Bands[0].Columns["Rows"].Hidden = true;
                    Settings.Default.UpgradeIndexesDataTableRemoveRows = false;
                }
            }

            if (Settings.Default.DatabasesTablesViewColumnStatisticsGrid is GridSettings)
            {
                lastColumnStatisticsGridSettings = Settings.Default.DatabasesTablesViewColumnStatisticsGrid;
                GridSettings.ApplySettingsToGrid(lastColumnStatisticsGridSettings, indexStatisticsColumnsGrid);
            }

            if (Settings.Default.DatabasesTablesViewDataDistributionGrid is GridSettings)
            {
                lastDataDistributionGridSettings = Settings.Default.DatabasesTablesViewDataDistributionGrid;
                GridSettings.ApplySettingsToGrid(lastDataDistributionGridSettings, indexStatisticsDataDistributionGrid);
            }

            if (Settings.Default.DatabasesTablesViewDetailsTab > -1
                && Settings.Default.DatabasesTablesViewDetailsTab < tableDetailsTabControl.Tabs.Count)
            {
                lastIndexesTab = Settings.Default.DatabasesTablesViewDetailsTab;
                tableDetailsTabControl.Tabs[lastIndexesTab].Selected = true;
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(tablesGrid);
            GridSettings dependenciesGridSettings = GridSettings.GetSettings(dependenciesGrid);
            GridSettings indexesGridSettings = GridSettings.GetSettings(indexesGrid);
            GridSettings columnStatisticsGridSettings = GridSettings.GetSettings(indexStatisticsColumnsGrid);
            GridSettings dataDistributionGridSettings = GridSettings.GetSettings(indexStatisticsDataDistributionGrid);
            // save all settings only if anything has changed
            if (lastMainSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || !mainGridSettings.Equals(lastMainGridSettings)
                || !dependenciesGridSettings.Equals(lastDependenciesGridSettings)
                || !indexesGridSettings.Equals(lastIndexesGridSettings)
                || !columnStatisticsGridSettings.Equals(lastColumnStatisticsGridSettings)
                || !dataDistributionGridSettings.Equals(lastDataDistributionGridSettings)
                || lastDetailsVisible != DetailsPanelVisible
                || lastDependencyType != dependencyType
                || lastStatisticsType != statisticsType
                || lastIndexesTab != tableDetailsTabControl.ActiveTab.Index)
            {
                // Fixed panel is second panel, so save size of second panel
                lastMainSplitterDistance =
                    Settings.Default.DatabasesTablesViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.DatabasesTablesViewMainGrid = mainGridSettings;
                lastDependenciesGridSettings =
                    Settings.Default.DatabasesTablesViewDependenciesGrid = dependenciesGridSettings;
                lastIndexesGridSettings =
                    Settings.Default.DatabasesTablesViewIndexesGrid = indexesGridSettings;
                lastColumnStatisticsGridSettings =
                    Settings.Default.DatabasesTablesViewColumnStatisticsGrid = columnStatisticsGridSettings;
                lastDataDistributionGridSettings =
                    Settings.Default.DatabasesTablesViewDataDistributionGrid = dataDistributionGridSettings;
                lastDetailsVisible =
                    Settings.Default.DatabasesTablesViewDetailsVisible = DetailsPanelVisible;
                lastDependencyType =
                    Settings.Default.DatabasesTablesViewDependencyType = dependencyType;
                lastStatisticsType =
                    Settings.Default.DatabasesTablesViewStatisticsType = statisticsType;
                lastIndexesTab =
                    Settings.Default.DatabasesTablesViewDetailsTab = tableDetailsTabControl.ActiveTab.Index;
            }
        }

        #region Properties

        public bool UpdateStatisticsAllowed
        {
            get
            {
                return tablesDataTable.Rows.Count > 0;
            }
        }

        public bool RebuildIndexesAllowed
        {
            get
            {
                return userTablesInList;
            }
        }

        public bool IncludeSystemDatabases
        {
            get { return includeSystemDatabases; }
            set
            {
                includeSystemDatabases = value;

                if (SystemDatabasesFilterChanged != null)
                {
                    SystemDatabasesFilterChanged(this, EventArgs.Empty);
                }

                ApplicationController.Default.ActiveView.CancelRefresh();
                RefreshAvailableDatabases();
            }
        }

        public bool IncludeSystemTables
        {
            get { return includeSystemTables; }
            set
            {
                includeSystemTables = value;

                if (SystemTablesFilterChanged != null)
                {
                    SystemTablesFilterChanged(this, EventArgs.Empty);
                }

                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !tablesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                tablesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool DetailsPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (DetailsPanelVisibleChanged != null)
                {
                    DetailsPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Data Management

        private void InitializeDataTables()
        {
            tablesDataTable = new DataTable();
            tablesDataTable.Columns.Add("Object ID", typeof (int));
            tablesDataTable.Columns.Add("Qualified Table Name", typeof (string));
            tablesDataTable.Columns.Add("Type", typeof (bool));
            tablesDataTable.Columns.Add("Database", typeof (string));
            tablesDataTable.Columns.Add("Owner/Schema", typeof (string));
            tablesDataTable.Columns.Add("Table", typeof (string));
            tablesDataTable.Columns.Add("Data Size (MB)", typeof (decimal));
            tablesDataTable.Columns.Add("Index Size (MB)", typeof (decimal));
            tablesDataTable.Columns.Add("Text Size (MB)", typeof (decimal));
            tablesDataTable.Columns.Add("% Database Size", typeof (decimal));
            tablesDataTable.Columns.Add("% Fragmentation (Quiet Time)", typeof (decimal));
            tablesDataTable.Columns.Add("Last Fragmentation Collection", typeof(DateTime));
            tablesDataTable.Columns.Add("% Fragmentation (Current)", typeof(decimal));
            tablesDataTable.Columns.Add("Fragmentation Status", typeof(string));
            tablesDataTable.Columns.Add("Rows", typeof(long));
            tablesDataTable.Columns.Add("Data Space Name", typeof (string));
            tablesDataTable.Columns.Add("Data Space Type", typeof(string));
            tablesDataTable.Columns.Add("Date Created", typeof (DateTime));
            tablesDataTable.Columns.Add("Pinned", typeof (bool));
            tablesDataTable.Columns.Add("BCP Table Locking", typeof (bool));
            tablesDataTable.Columns.Add("Full-Text Catalog", typeof (string));
            tablesGrid.DataSource = tablesDataTable;

            dependenciesDataTable = new DataTable();
            dependenciesDataTable.Columns.Add("Object", typeof (string));
            dependenciesDataTable.Columns.Add("Type", typeof (int));
            dependenciesDataTable.Columns.Add("Action", typeof (int));
            dependenciesGrid.DataSource = dependenciesDataTable;

            indexesDataTable = new DataTable();
            indexesDataTable.Columns.Add("Index", typeof (string));
            indexesDataTable.Columns.Add("Clustered", typeof(bool));
            indexesDataTable.Columns.Add("Unique", typeof(bool));
            indexesDataTable.Columns.Add("Size (MB)", typeof (decimal));
            indexesDataTable.Columns.Add("Rows", typeof (long));
            indexesDataTable.Columns.Add("Last Statistics Update", typeof (DateTime));
            indexesDataTable.Columns.Add("Rows Modified Since Last Update", typeof (int));
            indexesDataTable.Columns.Add("Fill Factor", typeof(int));
            indexesDataTable.Columns.Add("Levels", typeof(int));
            indexesGrid.DataSource = indexesDataTable;

            indexStatisticsColumnsDataTable = new DataTable();
            indexStatisticsColumnsDataTable.Columns.Add("Column(s)", typeof (string));
            indexStatisticsColumnsDataTable.Columns.Add("Average Length", typeof (double));
            indexStatisticsColumnsDataTable.Columns.Add("Average Row Hits", typeof (double));
            indexStatisticsColumnsDataTable.Columns.Add("Row Hits %", typeof(double));
            indexStatisticsColumnsGrid.DataSource = indexStatisticsColumnsDataTable;

            indexStatisticsDataDistributionTable = new DataTable();
            indexStatisticsDataDistributionTable.Columns.Add("Data Value", typeof (string));
            indexStatisticsDataDistributionTable.Columns.Add("Rows With Same Key", typeof (double));
            indexStatisticsDataDistributionGrid.DataSource = indexStatisticsDataDistributionTable;
        }

        private int UpdateTablesDataRow(FileSize totalDatabaseSize, DataRow updatedRow)
        {
            TableActionObject tableAction;
            DataRow existingRow;

            int objectId = (int)updatedRow["ObjectID"];
            string database = updatedRow["DatabaseName"] as string;
            string schema = updatedRow["Schema/Owner"] as string;
            string table = updatedRow["TableName"] as string;
            string qualifiedTableName = string.Format("{0}.{1}", schema, table);
            bool isSystemTable = (bool)updatedRow["IsSystemTable"];
            FileSize dataSize = updatedRow["DataSize"] as FileSize;
            FileSize indexSize = updatedRow["IndexSize"] as FileSize;
            FileSize textSize = updatedRow["TextSize"] as FileSize;
            long rowCount = (long)updatedRow["RowCount"];
            string dataSpace = updatedRow["DataSpaceName"] as string;
            string dataSpaceType = updatedRow["DataSpaceType"] as string;
            DateTime dateCreated = ((DateTime)updatedRow["CreateDate"]);
            bool isPinned = (bool)updatedRow["Pinned"];
            bool isBcpTableLockingEnabled = (bool)updatedRow["BCPLocked"];
            string fullTextCatalog = updatedRow["FulltextCatalog"] as string;

            decimal percentDatabaseSize = 0;
            if (totalDatabaseSize != null && totalDatabaseSize.Bytes.HasValue &&
                dataSize != null && dataSize.Bytes.HasValue &&
                indexSize != null && indexSize.Bytes.HasValue &&
                textSize != null && textSize.Bytes.HasValue)
            {
                percentDatabaseSize = (dataSize.Bytes.Value + indexSize.Bytes.Value + textSize.Bytes.Value) /
                                       totalDatabaseSize.Bytes.Value;
            }

            bool isExistingRow = false;
            DataRow tableRow;

            if (tableDataRowLookupTableById.TryGetValue(objectId, out existingRow))
            {
                tableRow = existingRow;
                isExistingRow = true;
            }
            else
            {
                tableRow = this.tablesDataTable.NewRow();
                tableRow["Object ID"] = objectId;
            }

            tableRow["Qualified Table Name"] = qualifiedTableName;
            tableRow["Type"] = isSystemTable;
            tableRow["Database"] = database;
            tableRow["Owner/Schema"] = schema;
            tableRow["Table"] = table;

            if (dataSize != null && dataSize.Megabytes.HasValue)
            {
                tableRow["Data Size (MB)"] = dataSize.Megabytes.Value;
            }

            if (indexSize != null && indexSize.Megabytes.HasValue)
            {
                tableRow["Index Size (MB)"] = indexSize.Megabytes.Value;
            }

            if (textSize != null && textSize.Megabytes.HasValue)
            {
                tableRow["Text Size (MB)"] = textSize.Megabytes.Value;
            }

            tableRow["% Database Size"] = percentDatabaseSize;
            // do not initialize to zero value or N/A will never show for tables with no info
            //tableRow["% Fragmentation (Quiet Time)"] = 0;

            // pull any updated fragmentation info from the tableUpdates cache
            if (tableRebuilds.TryGetValue(new Pair<string, int>(database, objectId), out tableAction))
            {
                tableRow["% Fragmentation (Current)"] = tableAction.PercentFragmentation.HasValue
                                                        ? (object)tableAction.PercentFragmentation.Value
                                                        : DBNull.Value;
                tableRow["Fragmentation Status"] = tableAction.FragmentationMessage;
                tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
            }

            tableRow["Rows"] = rowCount;
            tableRow["Data Space Name"] = dataSpace;
            tableRow["Data Space Type"] = dataSpaceType;
            tableRow["Date Created"] = dateCreated;
            tableRow["Pinned"] = isPinned;
            tableRow["BCP Table Locking"] = isBcpTableLockingEnabled;
            tableRow["Full-Text Catalog"] = fullTextCatalog;

            if (!isExistingRow)
            {
                tablesDataTable.Rows.Add(tableRow);
                tablesGrid.Rows[tablesGrid.Rows.Count - 1].RefreshSortPosition();

                if (!tableDataRowLookupTableById.ContainsKey(objectId))
                {
                    tableDataRowLookupTableById.Add(objectId, tableRow);
                }
                else
                {
                    tableDataRowLookupTableById[objectId] = tableRow;
                }

                if (!tableDataRowLookupTableByQualifiedName.ContainsKey(qualifiedTableName))
                {
                    tableDataRowLookupTableByQualifiedName.Add(qualifiedTableName, tableRow);
                }
                else
                {
                    tableDataRowLookupTableByQualifiedName[qualifiedTableName] = tableRow;
                }
            }

            if (!isSystemTable)
            {
                userTablesInList = true;
            }

            return objectId;
        }

        private void RemoveTables(IEnumerable<int> tableList)
        {
            Log.Debug("Removing tables that no longer exist");
            // Remove tables that no longer exist
            foreach (int tableId in tableList)
            {
                DataRow diffDataRow;
                if (tableDataRowLookupTableById.TryGetValue(tableId, out diffDataRow))
                {
                    tableDataRowLookupTableById.Remove(tableId);
                    tableDataRowLookupTableByQualifiedName.Remove(diffDataRow["Qualified Table Name"] as string);
                    tablesDataTable.Rows.Remove(diffDataRow);
                }
            }
        }

        private void UpdateFragmentation(DataTable fragmentationDataTable)
        {
            Log.Debug("Updating table fragmentation rows");
            // Update fragmentation for remaining rows
            foreach (DataRow fragmentationRow in fragmentationDataTable.Rows)
            {
                DataRow existingRow;

                if (
                    tableDataRowLookupTableByQualifiedName.TryGetValue(fragmentationRow[0] as string,
                                                                       out existingRow))
                {
                    if (fragmentationRow["Fragmentation"] != DBNull.Value &&
                        fragmentationRow["UTCCollectionDateTime"] != DBNull.Value)
                    {
                        existingRow["% Fragmentation (Quiet Time)"] = ((double)fragmentationRow["Fragmentation"]) /
                                                                      100;
                        existingRow["Last Fragmentation Collection"] =
                            ((DateTime)fragmentationRow["UTCCollectionDateTime"]);
                    }
                    else
                    {
                        existingRow["% Fragmentation (Quiet Time)"] = DBNull.Value;
                        existingRow["Last Fragmentation Collection"] = DBNull.Value;
                    }
                }
            }

            // this works slightly differently from other alerts by persisting the alertConfigItem between
            // refreshes so that background index rebuilds can use the same config levels until next refresh
            // adjusts them all
            Log.Debug("Updating % fragmentation cell color");
            AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
            if (alertConfig != null)
            {
                alertConfigItem = alertConfig[Metric.ReorganisationPct, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
                if (alertConfigItem != null && alertConfigItem.Enabled)
                {
                    UpdateCellColor(alertConfigItem, tablesGrid, "% Fragmentation (Quiet Time)", 100);
                }
                UpdateAllFragmentationCells();
            }
        }

        private void UpdateTablesDataTable(FileSize totalDatabaseSize, DataTable summaryDataTable, DataTable fragmentationDataTable)
        {
            using (Log.DebugCall("UpdateTablesDataTable"))
            {
                userTablesInList = false;
                if (summaryDataTable != null)
                {
                    Log.Debug("Createing list of existing tables");
                    Set<int> existingTables = new Set<int>(tableDataRowLookupTableById.Keys);
                    //                    foreach (int existingTable in tableDataRowLookupTableById.Keys)
                    //                    {
                    //                        existingTables.Add(existingTable);
                    //                    }

                    Log.Debug("Foreach data row in summary data table");
                    TableActionObject tableAction;
                    foreach (DataRow updatedRow in summaryDataTable.Rows)
                    {
                        DataRow existingRow;
                        int objectId = (int)updatedRow["ObjectID"];
                        string database = updatedRow["DatabaseName"] as string;
                        string schema = updatedRow["Schema/Owner"] as string;
                        string table = updatedRow["TableName"] as string;
                        string qualifiedTableName = string.Format("{0}.{1}", schema, table);
                        bool isSystemTable = (bool)updatedRow["IsSystemTable"];
                        FileSize dataSize = updatedRow["DataSize"] as FileSize;
                        FileSize indexSize = updatedRow["IndexSize"] as FileSize;
                        FileSize textSize = updatedRow["TextSize"] as FileSize;
                        long rowCount = (long)updatedRow["RowCount"];
                        string dataSpace = updatedRow["DataSpaceName"] as string;
                        string dataSpaceType = updatedRow["DataSpaceType"] as string;
                        DateTime dateCreated = ((DateTime)updatedRow["CreateDate"]);
                        bool isPinned = (bool)updatedRow["Pinned"];
                        bool isBcpTableLockingEnabled = (bool)updatedRow["BCPLocked"];
                        string fullTextCatalog = updatedRow["FulltextCatalog"] as string;

                        decimal percentDatabaseSize = 0;
                        if (totalDatabaseSize != null && totalDatabaseSize.Bytes.HasValue &&
                            dataSize != null && dataSize.Bytes.HasValue &&
                            indexSize != null && indexSize.Bytes.HasValue &&
                            textSize != null && textSize.Bytes.HasValue)
                        {
                            percentDatabaseSize = (dataSize.Bytes.Value + indexSize.Bytes.Value + textSize.Bytes.Value) /
                                                  totalDatabaseSize.Bytes.Value;
                        }

                        bool isExistingRow = false;
                        DataRow tableRow;

                        if (tableDataRowLookupTableById.TryGetValue(objectId, out existingRow))
                        {
                            tableRow = existingRow;
                            isExistingRow = true;
                            existingTables.Remove(objectId);
                        }
                        else
                        {
                            tableRow = this.tablesDataTable.NewRow();
                            tableRow["Object ID"] = objectId;
                        }

                        tableRow["Qualified Table Name"] = qualifiedTableName;
                        tableRow["Type"] = isSystemTable;
                        tableRow["Database"] = database;
                        tableRow["Owner/Schema"] = schema;
                        tableRow["Table"] = table;

                        if (dataSize != null && dataSize.Megabytes.HasValue)
                        {
                            tableRow["Data Size (MB)"] = dataSize.Megabytes.Value;
                        }

                        if (indexSize != null && indexSize.Megabytes.HasValue)
                        {
                            tableRow["Index Size (MB)"] = indexSize.Megabytes.Value;
                        }

                        if (textSize != null && textSize.Megabytes.HasValue)
                        {
                            tableRow["Text Size (MB)"] = textSize.Megabytes.Value;
                        }

                        tableRow["% Database Size"] = percentDatabaseSize;
                        // do not initialize to zero value or N/A will never show for tables with no info
                        //tableRow["% Fragmentation (Quiet Time)"] = 0;

                        // pull any updated fragmentation info from the tableUpdates cache
                        if (tableRebuilds.TryGetValue(new Pair<string, int>(database, objectId), out tableAction))
                        {
                            tableRow["% Fragmentation (Current)"] = tableAction.PercentFragmentation.HasValue
                                                                        ? (object)
                                                                          tableAction.PercentFragmentation.Value
                                                                        : DBNull.Value;
                            tableRow["Fragmentation Status"] = tableAction.FragmentationMessage;
                            tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
                        }

                        tableRow["Rows"] = rowCount;
                        tableRow["Data Space Name"] = dataSpace;
                        tableRow["Data Space Type"] = dataSpaceType;
                        tableRow["Date Created"] = dateCreated;
                        tableRow["Pinned"] = isPinned;
                        tableRow["BCP Table Locking"] = isBcpTableLockingEnabled;
                        tableRow["Full-Text Catalog"] = fullTextCatalog;

                        if (!isExistingRow)
                        {
                            tablesDataTable.Rows.Add(tableRow);
                            tablesGrid.Rows[tablesGrid.Rows.Count - 1].RefreshSortPosition();

                            if (!tableDataRowLookupTableById.ContainsKey(objectId))
                            {
                                tableDataRowLookupTableById.Add(objectId, tableRow);
                            }
                            else
                            {
                                tableDataRowLookupTableById[objectId] = tableRow;
                            }

                            if (!tableDataRowLookupTableByQualifiedName.ContainsKey(qualifiedTableName))
                            {
                                tableDataRowLookupTableByQualifiedName.Add(qualifiedTableName, tableRow);
                            }
                            else
                            {
                                tableDataRowLookupTableByQualifiedName[qualifiedTableName] = tableRow;
                            }
                        }

                        if (!isSystemTable)
                        {
                            userTablesInList = true;
                        }
                    }

                    Log.Debug("Removing tables that no longer exist");
                    // Remove tables that no longer exist
                    foreach (int diffTable in existingTables)
                    {
                        DataRow diffDataRow;

                        if (tableDataRowLookupTableById.TryGetValue(diffTable, out diffDataRow))
                        {
                            tableDataRowLookupTableById.Remove(diffTable);
                            tableDataRowLookupTableByQualifiedName.Remove(diffDataRow["Qualified Table Name"] as string);
                            tablesDataTable.Rows.Remove(diffDataRow);
                        }
                    }

                    Log.Debug("Updating table fragmentation rows");
                    // Update fragmentation for remaining rows
                    foreach (DataRow fragmentationRow in fragmentationDataTable.Rows)
                    {
                        DataRow existingRow;

                        if (
                            tableDataRowLookupTableByQualifiedName.TryGetValue(fragmentationRow[0] as string,
                                                                               out existingRow))
                        {
                            if (fragmentationRow["Fragmentation"] != DBNull.Value &&
                                fragmentationRow["UTCCollectionDateTime"] != DBNull.Value)
                            {
                                existingRow["% Fragmentation (Quiet Time)"] = ((double)fragmentationRow["Fragmentation"]) / 100;
                                existingRow["Last Fragmentation Collection"] =
                                    ((DateTime)fragmentationRow["UTCCollectionDateTime"]);
                            }
                            else
                            {
                                existingRow["% Fragmentation (Quiet Time)"] = DBNull.Value;
                                existingRow["Last Fragmentation Collection"] = DBNull.Value;
                            }
                        }
                    }

                    // this works slightly differently from other alerts by persisting the alertConfigItem between
                    // refreshes so that background index rebuilds can use the same config levels until next refresh
                    // adjusts them all
                    Log.Debug("Updating % fragmentation cell color");
                    AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                    if (alertConfig != null)
                    {
                        alertConfigItem = alertConfig[Metric.ReorganisationPct, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
                        if (alertConfigItem != null && alertConfigItem.Enabled)
                        {
                            UpdateCellColor(alertConfigItem, tablesGrid, "% Fragmentation (Quiet Time)", 100);
                        }
                        UpdateAllFragmentationCells();
                    }
                }
            }
        }

        private void UpdateTableDetails(TableDetail snapshot)
        {
            using (Log.DebugCall("UpdateTableDetails"))
            {
                currentTableDetailsSnapshot = snapshot;

                if (snapshot != null)
                {
                    UpdateTableDetailsSizeChart(snapshot);
                    UpdateTableDetailsDependencies(snapshot);
                    UpdateTableDetailsIndexes(snapshot);
                    UpdateTableDetailsIndexStatisticsComboBox(snapshot);
                }
            }
        }

        private void UpdateIndexStatistics(IndexStatistics snapshot)
        {
            using (Log.DebugCall("UpdateIndexStatistics"))
            {
                indexStatisticsColumnsDataTable.Rows.Clear();
                indexStatisticsDataDistributionTable.Rows.Clear();

                if (snapshot != null)
                {
                    foreach (IndexColumn column in snapshot.IndexColumns)
                    {
                        DataRow columnRow = indexStatisticsColumnsDataTable.NewRow();

                        columnRow["Column(s)"] = column.Columns;

                        if (column.AverageLength.HasValue)
                        {
                            columnRow["Average Length"] = column.AverageLength.Value;
                        }

                        if (column.AverageRowHits.HasValue)
                        {
                            columnRow["Average Row Hits"] = column.AverageRowHits.Value;
                        }

                        if (column.RowHitsPercent.HasValue)
                        {
                            columnRow["Row Hits %"] = column.RowHitsPercent.Value;
                        }

                        indexStatisticsColumnsDataTable.Rows.Add(columnRow);
                    }

                    foreach (IndexDataDistribution dataDistributionItem in snapshot.DataDistribution)
                    {
                        DataRow dataDistributionRow = indexStatisticsDataDistributionTable.NewRow();

                        dataDistributionRow["Data Value"] = dataDistributionItem.DataValue;

                        if (dataDistributionItem.Rows.HasValue)
                        {
                            dataDistributionRow["Rows With Same Key"] = dataDistributionItem.Rows.Value;
                        }

                        indexStatisticsDataDistributionTable.Rows.Add(dataDistributionRow);
                    }

                    AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                    if (alertConfig != null)
                    {
                        AlertConfigurationItem alertConfigItem = alertConfig[Metric.IndexRowHits, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
                        if (alertConfigItem != null && alertConfigItem.Enabled)
                        {
                            UpdateCellColor(alertConfigItem, indexStatisticsColumnsGrid, "Row Hits %", 1);
                        }
                    }

                    if (indexStatisticsColumnsGrid != null && indexStatisticsColumnsGrid.Rows.Count > 0)
                    {
                        indexStatisticsColumnsGrid.Rows[0].Selected = true;
                    }
                    
                }
            }
        }

        private void UpdateCellColor(AlertConfigurationItem alertConfigItem, UltraGrid grid, string columnName, int adjustmentMultiplier)
        {
            using (Log.DebugCall("UpdateCellColor"))
            {
                foreach (UltraGridRow gridRow in grid.Rows.GetAllNonGroupByRows())
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
                            if (value != null && adjustmentMultiplier != 1)
                            {
                                double dbl = (double)Convert.ChangeType(value, typeof(double));
                                value = dbl * adjustmentMultiplier;
                            }
                            
                            bool insideThresholdBounds = true;

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
                                    insideThresholdBounds = false;
                                    break;
                            }

                            if (insideThresholdBounds && grid == tablesGrid)
                            {                                                                 
                                UltraGridCell lastFragmentationCollectionCell = gridRow.Cells["Last Fragmentation Collection"];
                                DateTime lastFragmentation = lastFragmentationCollectionCell != null ? (DateTime)lastFragmentationCollectionCell.OriginalValue : DateTime.Now;
                                int totalHours = (int)DateTime.Now.Subtract(lastFragmentation).TotalHours;

                                if (totalHours >= this.MaxHoursForShadingAlerts)
                                {
                                    cell.Appearance.BackColor = Color.LightGray;
                                    cell.Appearance.ForeColor = Color.Black;
                                }    
                            }
                            
                        }
                    }
                }
            }
        }

        private void UpdateAllFragmentationCells()
        {
            TableActionObject table;
            using (Log.DebugCall("UpdateAllFragmentationCells"))
            {
                foreach (UltraGridRow gridRow in tablesGrid.Rows.GetAllNonGroupByRows())
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    Pair<string, int> key =
                        new Pair<string, int>((string)dataRowView["Database"], (int)dataRowView["Object ID"]);

                    if (tableRebuilds.TryGetValue(key, out table))
                    {
                        updateFragmentationCell(gridRow, table);
                    }
                }
            }
        }

        private void updateFragmentationCell(UltraGridRow row, TableActionObject table)
        {
            UltraGridCell cell = row.Cells["Fragmentation Status"];
            cell.ToolTipText = table.StatusMessage;
            if (table.Status == TableActionStatus.Failed)
            {
                cell.Appearance.BackColor = Color.Red;
                cell.Appearance.ForeColor = Color.White;
            }
            else
            {
                if (table.Status == TableActionStatus.Successful
                    && table.PercentFragmentation.HasValue
                    && alertConfigItem != null && alertConfigItem.Enabled)
                {
                    switch (alertConfigItem.GetSeverity((IComparable)(table.PercentFragmentation.Value * 100)))
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
                else
                {
                    cell.Appearance.ResetBackColor();
                    cell.Appearance.ResetForeColor();
                }
            }
        }

        private void UpdateTableDetailsSizeChart(TableDetail snapshot)
        {
            DataRow selectedTableDataRow;
            tableSizeChart.SuspendLayout();
            tableSizeChart.Series.Clear();

            if (tableDataRowLookupTableById.TryGetValue(selectedTableId, out selectedTableDataRow) &&
                snapshot != null)
            {
                tableSizeChart.Tag = string.Format("Table Size: {0}", selectedTableDataRow["Qualified Table Name"]);
                tableSizeChart.Data.Series = 2 + snapshot.Indexes.Count;
                tableSizeChart.Data.Points = 1;
                tableSizeChart.Series[0].Text = "Data";
                tableSizeChart.Data[0, 0] = Convert.ToDouble((decimal) selectedTableDataRow["Data Size (MB)"])*1024;
                tableSizeChart.Series[1].Text = "Text";
                tableSizeChart.Data[1, 0] = Convert.ToDouble((decimal) selectedTableDataRow["Text Size (MB)"])*1024;

                for (int i=0; i < snapshot.Indexes.Count; i++)
                {
                    Index index = snapshot.Indexes[i];
                    tableSizeChart.Series[i + 2].Text = index.Name;
                    tableSizeChart.Data[i + 2, 0] = index.Size.Kilobytes.HasValue
                                                        ? Convert.ToDouble(index.Size.Kilobytes.Value)
                                                        : 0;
                }

                tableSizeChart_Resize(tableSizeChart, new EventArgs());
                tableSizeChart.RecalculateScale();
            }
            tableSizeChart.Invalidate();
            tableSizeChart.ResumeLayout();
        }

        private void UpdateTableDetailsDependencies(TableDetail snapshot)
        {
            dependenciesDataTable.Rows.Clear();

            if (snapshot != null)
            {
                if (dependenciesReferencesViewRadioButton.Checked)
                {
                    foreach (TableDependency dependency in snapshot.References)
                    {
                        DataRow row = dependenciesDataTable.NewRow();
                        row["Object"] = dependency.KeyName;

                        if (dependency.Type.HasValue)
                        {
                            row["Type"] = dependency.Type.Value;
                        }

                        if (dependency.Action.HasValue)
                        {
                            row["Action"] = dependency.Action.Value;
                        }

                        dependenciesDataTable.Rows.Add(row);
                    }
                }
                else if (dependenciesReferencedByViewRadioButton.Checked)
                {
                    foreach (TableDependency dependency in snapshot.ReferencedBy)
                    {
                        DataRow row = dependenciesDataTable.NewRow();
                        row["Object"] = dependency.Fullname;

                        if (dependency.Type.HasValue)
                        {
                            row["Type"] = dependency.Type.Value;
                        }

                        if (dependency.Action.HasValue)
                        {
                            row["Action"] = dependency.Action.Value;
                        }

                        dependenciesDataTable.Rows.Add(row);
                    }
                }
            }
        }

        private void UpdateTableDetailsIndexes(TableDetail snapshot)
        {
            indexesDataTable.Rows.Clear();

            if (snapshot != null)
            {
                foreach (Index index in snapshot.Indexes)
                {
                    DataRow row = indexesDataTable.NewRow();
                    row["Index"] = index.Name;

                    if (index.Clustered.HasValue)
                    {
                        row["Clustered"] = index.Clustered.Value;
                    }

                    if (index.Unique.HasValue)
                    {
                        row["Unique"] = index.Unique.Value;
                    }

                    if (index.Size.Megabytes.HasValue)
                    {
                        row["Size (MB)"] = index.Size.Megabytes.Value;
                    }

                    if (index.Tablerows.HasValue)
                    {
                        row["Rows"] = index.Tablerows.Value;
                    }

                    if (index.LastUpdate.HasValue)
                    {
                        row["Last Statistics Update"] = index.LastUpdate.Value;
                    }

                    if (index.RowsModifiedSinceStatistics.HasValue)
                    {
                        row["Rows Modified Since Last Update"] = index.RowsModifiedSinceStatistics.Value;
                    }

                    if (index.FillFactor.HasValue)
                    {
                        row["Fill Factor"] = index.FillFactor.Value;
                    }

                    if (index.Levels.HasValue)
                    {
                        row["Levels"] = index.Levels.Value;
                    }

                    indexesDataTable.Rows.Add(row);
                }

                if (!indexesGridInitialized && snapshot.Indexes.Count > 0)
                {
                    foreach (UltraGridColumn column in indexesGrid.DisplayLayout.Bands[0].Columns)
                    {
                        column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                        column.Width = Math.Min(column.Width, indexesGrid.Width / 2);
                    }

                    indexesGridInitialized = true;
                }
            }
        }

        private void UpdateTableDetailsIndexStatisticsComboBox(TableDetail snapshot)
        {
            indexStatisticsIndexSelectionComboBox.Items.Clear();
            indexStatisticsColumnsDataTable.Rows.Clear();
            indexStatisticsDataDistributionTable.Rows.Clear();

            if (snapshot != null)
            {
                foreach (Index index in snapshot.Indexes)
                {
                    indexStatisticsIndexSelectionComboBox.Items.Add(index.Name);
                }

                indexStatisticsIndexSelectionComboBox.SelectedIndex = 0;
            }
        }

        #endregion

        #region Events Handlers

        private void dependenciesReferencesViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableDetailsDependencies(currentTableDetailsSnapshot);
        }

        private void indexStatisticsColumnsViewRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            indexStatisticsColumnsGrid.Visible = indexStatisticsColumnsViewRadioButton.Checked;
            indexStatisticsDataDistributionGrid.Visible = indexStatisticsDataDistributionViewRadioButton.Checked;
        }

        private void hideTableDetailsButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = false;
        }

        private void refreshDatabasesButton_Click(object sender, EventArgs e)
        {
            RefreshAvailableDatabases();
        }

        private void refreshAvailableDatabasesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshAvailableDatabasesWorker";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            e.Result = managementService.GetDatabases(instanceId, includeSystemDatabases, true);

            if (refreshAvailableDatabasesBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void refreshAvailableDatabasesBackgroundWorker_RunWorkerCompleted(object sender,
                                                                                  RunWorkerCompletedEventArgs e)
        {
            availableDatabasesRefreshStopwatch.Stop();
            availableDatabasesComboBox.Items.Clear();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    Log.Info(string.Format("Refresh available databases completed (Duration = {0}).", availableDatabasesRefreshStopwatch.Elapsed));
                    IDictionary<string, bool> availableDatabases = e.Result as IDictionary<string, bool>;

                    if (availableDatabases != null && availableDatabases.Count > 0)
                    {
                        foreach (string database in availableDatabases.Keys)
                        {
                            availableDatabasesComboBox.Items.Add(database, database);
                        }

                        int index = -1;
                        if (selectedDatabaseArgument != null)
                        {
                            index = FindDatabaseIndex(selectedDatabaseArgument);
                            selectedDatabaseArgument = null;
                        }
                        if (index == -1)
                        {
                            index = FindDatabaseIndex(selectedDatabase);
                        }
                        availableDatabasesComboBox.SelectedIndex = index >= 0 ? index : 0;

                        availableDatabasesInitialized = true;
                        availableDatabasesComboBox.Enabled = true;
                    }

                    if (availableDatabasesComboBox.Items.Count == 0)
                    {
                        availableDatabasesInitialized = false;
                        availableDatabasesComboBox.Items.Add(null,
                                                             "< No databases are currently available. Please refresh again... >");
                        availableDatabasesComboBox.Enabled = false;
                    }
                }
                else
                {
                    availableDatabasesInitialized = false;
                    availableDatabasesComboBox.Items.Add(null,
                                                         "< Error retrieving databases. >");
                    availableDatabasesComboBox.Enabled = false;
                    ShowTablesGridStatusMessage("Unable to update data for this view.");
                    ShowTableDetailsStatusMessage("Unable to update data for this view.");
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, e.Error));
                }
            }
        }

        private void availableDatabasesComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (availableDatabasesComboBox.SelectedItem != null && 
                availableDatabasesComboBox.SelectedItem.DataValue != null)
            {
                UpdateSelectedTableId(-1);
                ApplicationController.Default.ClearCustomStatus();
                tablesDataTable.Clear();
                tableDataRowLookupTableById.Clear();
                tableDataRowLookupTableByQualifiedName.Clear();
                ShowTablesGridStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
                ShowTableDetailsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
                selectedDatabase = availableDatabasesComboBox.SelectedItem.DataValue as string;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
                tablesGrid.Focus();
            }
        }

        private void tablesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (tablesGrid.Selected.Rows.Count > 0 && tablesGrid.Selected.Rows[0].Cells != null)
            {
                UpdateSelectedTableId((int) tablesGrid.Selected.Rows[0].Cells["Object ID"].Value);
                ShowTableDetailsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);

                Log.Info(string.Format("Refreshing table details for {0}...", tablesGrid.Selected.Rows[0].Cells["Qualified Table Name"].Value));
                tableDetailsRefreshStopwatch.Reset();
                tableDetailsRefreshStopwatch.Start();
                InitializeRefreshTableDetailsBackgroundWorker();
                refreshTableDetailsBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                UpdateSelectedTableId(-1);
                ShowTableDetailsStatusMessage("Select a table to view more details.");
            }
        }

        private void refreshTableDetailsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshTableDetailsWorker";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            TableDetailConfiguration configuration = new TableDetailConfiguration(instanceId, selectedTableId, selectedDatabase);
            e.Result = (TableDetail)managementService.GetTableDetails(configuration);

            if (refreshTableDetailsBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void refreshTableDetailsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tableDetailsRefreshStopwatch.Stop();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    TableDetail snapshot = e.Result as TableDetail;

                    if (snapshot != null && snapshot.Error == null)
                    {
                        if (snapshot.Error == null)
                        {
                            Log.Info(
                                string.Format("Refresh table details completed for {0} (Duration = {1}).",
                                              snapshot.Schema + "." + snapshot.TableName,
                                              availableDatabasesRefreshStopwatch.Elapsed));

                            UpdateTableDetails(snapshot);
                            tableDetailsTabControl.Visible = true;
                        }
                        else
                        {
                            ShowTableDetailsStatusMessage("An error occurred while refreshing table details.");
                            Log.Error("An error occurred while refreshing table details.", snapshot.Error);
                        }
                    }
                    else
                    {
                        ShowTableDetailsStatusMessage("No table details are available.");
                    }
                }
                else
                {
                    ShowTableDetailsStatusMessage("An error occurred while refreshing table details.");
                    Log.Error("An error occurred while refreshing table details.", e.Error);
                }
            }
        }

        private void indexStatisticsIndexSelectionComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (indexStatisticsIndexSelectionComboBox.SelectedItem.DataValue != null)
            {
                indexStatisticsColumnsDataTable.Rows.Clear();
                indexStatisticsDataDistributionTable.Rows.Clear();
                ShowIndexStatisticsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
                selectedIndex = indexStatisticsIndexSelectionComboBox.SelectedItem.DataValue as string;

                Log.Info(
                    string.Format("Refreshing index statistics for {0}...",
                                  indexStatisticsIndexSelectionComboBox.SelectedItem.DisplayText));
                availableDatabasesRefreshStopwatch.Reset();
                availableDatabasesRefreshStopwatch.Start();
                InitializeRefreshIndexStatisticsBackgroundWorker();
                refreshIndexStatisticsBackgroundWorker.RunWorkerAsync();
            }
        }

        private void refreshIndexStatisticsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RefreshIndexStatisticsWorker";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            IndexStatisticsConfiguration configuration =
                new IndexStatisticsConfiguration(instanceId, selectedDatabase, selectedIndex, selectedTableId);
            e.Result = (IndexStatistics)managementService.GetIndexStatistics(configuration);

            if (refreshIndexStatisticsBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void refreshIndexStatisticsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            indexStatisticsRefreshStopwatch.Stop();

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    IndexStatistics snapshot = e.Result as IndexStatistics;

                    if (snapshot != null && snapshot.Error == null)
                    {
                        if (snapshot.Error == null)
                        {
                            Log.Info(string.Format("Refresh index statistics completed (Duration = {0}).", availableDatabasesRefreshStopwatch.Elapsed));
                            UpdateIndexStatistics(snapshot);
                        }
                        else
                        {
                            ShowIndexStatisticsStatusMessage("An error occurred while refreshing index statistics.");
                            Log.Error("An error occurred while refreshing index statistics.", snapshot.Error);
                        }
                    }
                    else
                    {
                        ShowIndexStatisticsStatusMessage("No index statistics are available.");
                    }
                }
                else
                {
                    ShowIndexStatisticsStatusMessage("An error occurred while refreshing index statistics.");
                    Log.Error("An error occurred while refreshing index statistics.", e.Error);
                }
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
                case "gridDataContextMenu":
                    ((PopupMenuTool)e.Tool).Tools["updateTableStatisticsButton"].SharedProps.Visible =
                        ((PopupMenuTool)e.Tool).Tools["rebuildTableIndexesButton"].SharedProps.Visible = 
                            ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
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
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void rebuildIndexesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "RebuildIndexesWorker";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (pendingTableRebuilds.Count > 0)
            {
                Pair<string, int> key = pendingTableRebuilds[0];
                rebuildIndexesBackgroundWorker.ReportProgress(0, key);
                pendingTableRebuilds.RemoveAt(0);
                TableActionObject table;
                if (tableRebuilds.TryGetValue(key, out table))
                {
                    Pair<ReindexConfiguration, object> results = new Pair<ReindexConfiguration, object>();
                    try
                    {
                        ReindexConfiguration configuration = new ReindexConfiguration(instanceId, table.DatabaseName, table.TableId);
                        results.First = configuration;
                        results.Second = (Snapshot)managementService.SendReindex(configuration);
                    }
                    catch (Exception ex)
                    {
                        results.Second = ex;
                    }
                    e.Result = results;
                }
            }

            if (rebuildIndexesBackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void rebuildIndexesBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TableActionObject table;
            DataRow tableRow;
            if (tableRebuilds.TryGetValue((Pair<string, int>)e.UserState, out table))
            {
                table.Status = TableActionStatus.InProcess;
                if (table.DatabaseName == selectedDatabase
                    && tableDataRowLookupTableById.TryGetValue(table.TableId, out tableRow))
                {
                    tableRow["Fragmentation Status"] = table.FragmentationMessage;
                    tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
                    foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((int)row.Cells["Object Id"].Value == table.TableId)
                        {
                            updateFragmentationCell(row, table);
                            break;
                        }
                    }
                }
            }
        }

        private void rebuildIndexesBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ReindexConfiguration configuration;
            TableActionObject table;
            DataRow tableRow;
            if (!e.Cancelled)
            {
                Pair<ReindexConfiguration, object> results = (Pair<ReindexConfiguration, object>)e.Result;

                if (results.Second is ServiceException)
                {
                    configuration = results.First;
                    ServiceException ex = (ServiceException)results.Second;

                    if (tableRebuilds.TryGetValue(new Pair<string, int>(configuration.DatabaseName, configuration.TableId.Value), out table))
                    {
                        table.ActionTime = DateTime.Now;
                        // the management service has stopped waiting for the task to finish, but it continues
                        if (ex.HRESULT.Equals(ServiceException.StatusToInt(Status.ErrorRequestTimeout)))
                        {
                            table.Status = TableActionStatus.Ongoing;
                            table.ActionError = null;
                        }
                        else
                        {
                            table.Status = TableActionStatus.Failed;
                            table.ActionError = ex;
                        }
                        table.PercentFragmentation = null;
                        if (tableDataRowLookupTableById.TryGetValue(table.TableId, out tableRow))
                        {
                            tableRow["Fragmentation Status"] = table.FragmentationMessage;
                            tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
                            foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                            {
                                if ((int)row.Cells["Object Id"].Value == table.TableId)
                                {
                                    updateFragmentationCell(row, table);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (results.Second is ReindexSnapshot)
                {
                    ReindexSnapshot snapshot = results.Second as ReindexSnapshot;

                    if (snapshot != null)
                    {
                        if (snapshot.Error == null)
                        {
                            if (tableRebuilds.TryGetValue(new Pair<string, int>(snapshot.DatabaseName, snapshot.TableId.Value), out table))
                            {
                                table.ActionTime = snapshot.TimeStampLocal;
                                table.Status = TableActionStatus.Successful;
                                table.ActionError = null;
                                table.PercentFragmentation = snapshot.Fragmentation.HasValue ? ((decimal?)snapshot.Fragmentation) / 100: null;
                                if (tableDataRowLookupTableById.TryGetValue(table.TableId, out tableRow))
                                {
                                    tableRow["% Fragmentation (Current)"] = table.PercentFragmentation.HasValue ? (object)table.PercentFragmentation.Value : DBNull.Value;
                                    tableRow["Fragmentation Status"] = table.FragmentationMessage;
                                    tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
                                    foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                                    {
                                        if ((int)row.Cells["Object Id"].Value == table.TableId)
                                        {
                                            updateFragmentationCell(row, table);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (snapshot.DatabaseName == selectedDatabase
                                && snapshot.TableId == selectedTableId)
                            {
                                ShowTableDetailsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
                                Log.Info(string.Format("Refreshing table details for {0}...", snapshot.TableName));
                                tableDetailsRefreshStopwatch.Reset();
                                tableDetailsRefreshStopwatch.Start();
                                InitializeRefreshTableDetailsBackgroundWorker();
                                refreshTableDetailsBackgroundWorker.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            configuration = results.First;
                            if (tableRebuilds.TryGetValue(new Pair<string, int>(configuration.DatabaseName, configuration.TableId.Value), out table))
                            {
                                table.ActionTime = snapshot.TimeStampLocal;
                                table.Status = TableActionStatus.Failed;
                                table.ActionError = snapshot.Error;
                                table.PercentFragmentation = snapshot.Fragmentation.HasValue ? ((decimal?)snapshot.Fragmentation.Value) / 100: null;
                                if (tableDataRowLookupTableById.TryGetValue(table.TableId, out tableRow))
                                {
                                    tableRow["Fragmentation Status"] = table.FragmentationMessage;
                                    tablesGrid.DisplayLayout.Bands[0].Columns["Fragmentation Status"].Hidden = false;
                                    foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                                    {
                                        if ((int)row.Cells["Object Id"].Value == table.TableId)
                                        {
                                            updateFragmentationCell(row, table);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // kick it off again to process the next one
                if (pendingTableRebuilds.Count > 0)
                {
                    rebuildIndexesBackgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                foreach (Pair<string, int> key in pendingTableRebuilds)
                {
                    if (tableRebuilds.TryGetValue(key, out table))
                    {
                        table.Status = TableActionStatus.Cancelled;
                        if (tableDataRowLookupTableById.TryGetValue(table.TableId, out tableRow))
                        {
                            tableRow["Fragmentation Status"] = table.FragmentationMessage;
                            foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                            {
                                if ((int)row.Cells["Object Id"].Value == table.TableId)
                                {
                                    updateFragmentationCell(row, table);
                                    break;
                                }
                            }
                        }
                    }
                }
                pendingTableRebuilds.Clear();
            }
        }


        private void updateStatisticsBackgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "UpdateStatisticsWorker";

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (pendingTableUpdates.Count > 0)
            {
                Pair<string, int> key = pendingTableUpdates[0];
                pendingTableUpdates.RemoveAt(0);
                UpdateStatisticsConfiguration configuration = new UpdateStatisticsConfiguration(instanceId, key.First, key.Second);
                Pair<Pair<string, int>, object> result = new Pair<Pair<string, int>, object>();
                result.First = key;
                try
                {
                    //Remove the audit event on management service
                   //AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                    result.Second = managementService.SendUpdateStatistics(configuration);
                }
                catch (Exception e)
                {
                    result.Second = e;
                }
                args.Result = result;
            }

            if (updateStatisticsBackgroundWorker.CancellationPending)
            {
                args.Cancel = true;
            }
        }

        private void updateStatisticsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Result is Pair<Pair<string, int>, object>)
                {
                    Pair<Pair<string, int>, object> pair = (Pair<Pair<string, int>, object>)e.Result;
                    Pair<string, int> key = pair.First;

                    if (pair.Second is Exception)
                    {
                        Exception ex = (Exception)pair.Second;
                        Log.ErrorFormat("Error executing update statistics on database {0} table id {1}.  {2}", key.First, key.Second, ex);
                    }
                    else
                    {
                        Snapshot snapshot = pair.Second as Snapshot;
                        // Try to populate the snapshot
                        if (snapshot == null && pair.Second is Serialized<Snapshot>)
                        {
                            snapshot = ((Serialized<Snapshot>) pair.Second).Deserialize();
                        }
                        if (snapshot != null)
                        {
                            if (snapshot.Error == null)
                            {
                                if (key.First == selectedDatabase
                                    && key.Second == selectedTableId)
                                {
                                    ShowTableDetailsStatusMessage(Idera.SQLdm.Common.Constants.LOADING);

                                    if (tablesGrid.Selected.Rows.Count > 0 && tablesGrid.Selected.Rows[0].Cells != null)
                                    {
                                        Log.Info(
                                            string.Format("Refreshing table details for {0}...",
                                                tablesGrid.Selected.Rows[0].Cells["Qualified Table Name"].Value));
                                    }
                                    else
                                    {
                                        Log.Info(string.Format("Refreshing table details for {0}...", key.Second));
                                    }

                                    tableDetailsRefreshStopwatch.Reset();
                                    tableDetailsRefreshStopwatch.Start();
                                    InitializeRefreshTableDetailsBackgroundWorker();
                                    refreshTableDetailsBackgroundWorker.RunWorkerAsync();
                                }
                            }
                            else
                            {
                                // Update UI for failures of update statistics command
                                var message =
                                    string.Format(
                                        "Error executing update statistics on database {0} table id {1}.  {2}",
                                        key.First, key.Second, snapshot.Error);
                                ShowTableDetailsStatusMessage(message);
                                Log.ErrorFormat(message);
                            }
                        }
                    }
                }

                // kick it off again to process the next one
                if (pendingTableUpdates.Count > 0)
                {
                    updateStatisticsBackgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                pendingTableUpdates.Clear();
            }
        }

        #endregion

        #region Helpers

        private void InitializeRefreshTableDetailsBackgroundWorker()
        {
            if (refreshTableDetailsBackgroundWorker != null)
            {
                if (refreshTableDetailsBackgroundWorker.IsBusy)
                {
                    refreshTableDetailsBackgroundWorker.CancelAsync();
                    refreshTableDetailsBackgroundWorker = null;
                    InitializeRefreshTableDetailsBackgroundWorker();
                }
            }
            else
            {
                refreshTableDetailsBackgroundWorker = new BackgroundWorker();
                refreshTableDetailsBackgroundWorker.WorkerSupportsCancellation = true;
                refreshTableDetailsBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshTableDetailsBackgroundWorker_DoWork);
                refreshTableDetailsBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshTableDetailsBackgroundWorker_RunWorkerCompleted);
            }
        }

        private void InitializeRefreshIndexStatisticsBackgroundWorker()
        {
            if (refreshIndexStatisticsBackgroundWorker != null)
            {
                if (refreshIndexStatisticsBackgroundWorker.IsBusy)
                {
                    refreshIndexStatisticsBackgroundWorker.CancelAsync();
                    refreshIndexStatisticsBackgroundWorker = null;
                    InitializeRefreshIndexStatisticsBackgroundWorker();
                }
            }
            else
            {
                refreshIndexStatisticsBackgroundWorker = new BackgroundWorker();
                refreshIndexStatisticsBackgroundWorker.WorkerSupportsCancellation = true;
                refreshIndexStatisticsBackgroundWorker.DoWork +=
                    new DoWorkEventHandler(refreshIndexStatisticsBackgroundWorker_DoWork);
                refreshIndexStatisticsBackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(refreshIndexStatisticsBackgroundWorker_RunWorkerCompleted);
            }
        }

        private int FindDatabaseIndex(string database)
        {
            int index = -1;

            for (int i = 0; i < availableDatabasesComboBox.Items.Count; i++)
            {
                if (string.CompareOrdinal(availableDatabasesComboBox.Items[i].DataValue as string, database) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void ShowTablesGridStatusMessage(string message)
        {
            tablesGridStatusMessage.Text = message;
            tablesGridStatusMessage.BringToFront();
            tablesGridStatusMessage.Visible = true;
        }

        private void ShowTableDetailsStatusMessage(string message)
        {
            tableDetailsStatusLabel.Text = message;
            tableDetailsTabControl.Visible = false;
        }

        private void ShowIndexStatisticsStatusMessage(string message)
        {
            
        }

        private void UpdateSelectedTableId(int id)
        {
            selectedTableId = id;
        }

        #endregion

        #region Splitter Focus

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

        #region Refresh Data

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesTablesIndexesView_Fill_Panel.Visible = true;

                if (!availableDatabasesInitialized)
                {
                    RefreshAvailableDatabases();
                }
                else
                {
                    base.RefreshView();
                }
            }
            else
            {
                DatabasesTablesIndexesView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override void CancelRefresh()
        {
            refreshAvailableDatabasesBackgroundWorker.CancelAsync();
            refreshTableDetailsBackgroundWorker.CancelAsync();
            base.CancelRefresh();
        }

        private void RefreshAvailableDatabases()
        {
            if (!refreshAvailableDatabasesBackgroundWorker.IsBusy)
            {
                availableDatabasesComboBox.Enabled = false;
                availableDatabasesComboBox.Items.Clear();
                availableDatabasesComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING+" >");
                availableDatabasesComboBox.SelectedIndex = 0;

                Log.Info("Refreshing available databases...");
                availableDatabasesRefreshStopwatch.Reset();
                availableDatabasesRefreshStopwatch.Start();
                refreshAvailableDatabasesBackgroundWorker.RunWorkerAsync();
            }
        }

        private delegate void RemoveTablesDelegate(IEnumerable<int> tables);
        private delegate int UpdateTablesDataRowDelegate(FileSize totalDatabaseSize, DataRow tableRow);

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (backgroundWorker.CancellationPending)
                return null;

            TableSummaryConfiguration configuration =
                new TableSummaryConfiguration(instanceId, true, includeSystemTables, selectedDatabase);
            TableSummary tableSummary = managementService.GetTableSummary(configuration);           

            if (backgroundWorker.CancellationPending)
                return null;

            DataTable tableFragmentationDataTable =
                RepositoryHelper.GetTableFragmentation(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                       instanceId, selectedDatabase);

            tablesGrid.Invoke((MethodInvoker)delegate()
                {
                    Log.Debug("Suspending table events");
                    tablesGrid.SuspendLayout();
                });

            UpdateTablesDataRowDelegate updateTableRowDelegate = new UpdateTablesDataRowDelegate(UpdateTablesDataRow);
            try
            {
                FileSize totalDatabaseSize = tableSummary.TotalDatabaseSize;
                Log.Debug("Getting current table set");
                Set<int> currentTableSet = new Set<int>(tableDataRowLookupTableById.Keys);
                Log.Debug("Iterating table list");
                foreach (DataRow tableRow in tableSummary.TableList.Rows)
                {
                    if (backgroundWorker.CancellationPending)
                        return null;

                    object objectId = tablesGrid.Invoke(updateTableRowDelegate, totalDatabaseSize, tableRow);
                    if (objectId is int)
                        currentTableSet.Remove((int)objectId);
                }

                if (backgroundWorker.CancellationPending)
                    return null;

                if (currentTableSet.Count > 0)
                {
                    this.Invoke((RemoveTablesDelegate)
                                delegate(IEnumerable<int> currentTables) { RemoveTables(currentTables); },
                                currentTableSet);
                }
                if (backgroundWorker.CancellationPending)
                    return null;

            }
            finally
            {
                tablesGrid.Invoke((MethodInvoker)delegate() { tablesGrid.ResumeLayout(); });
            }

            return new Pair<TableSummary, DataTable>(tableSummary, tableFragmentationDataTable);
        }

        public override void UpdateData(object data)
        {
            using (Log.DebugCall("UpdateData"))
            {
                if (data != null && data is Pair<TableSummary, DataTable>)
                {
                    lock (updateLock)
                    {
                        Pair<TableSummary, DataTable> snapshot = (Pair<TableSummary, DataTable>)data;

                        if (snapshot.First.Error == null)
                        {
                            currentSnapshot = snapshot.First;

                            tablesGrid.SuspendLayout();

                            //   UpdateTablesDataTable(snapshot.First.TotalDatabaseSize, snapshot.First.TableList,
                            //                         snapshot.Second);

                            UpdateFragmentation(snapshot.Second);

                            if (snapshot.First.CompatibilityLevel != null
                                && snapshot.First.CompatibilityLevel < 6.5f)
                            {
                                ShowTablesGridStatusMessage(DB_NOT_SUPPORTED);
                                ShowTableDetailsStatusMessage(DB_NOT_SUPPORTED);
                            }
                            else if (snapshot.First.TableList.Rows.Count == 0)
                            {
                                ShowTablesGridStatusMessage("There are no items to show in this view.");
                                ShowTableDetailsStatusMessage("There are no items to show in this view.");
                            }
                            else
                            {
                                tablesGridStatusMessage.Visible = false;

                                if (tablesGrid.Selected.Rows.Count == 0)
                                {
                                    ShowTableDetailsStatusMessage("Select a table to view details.");

                                    if (selectedTableNameArgument != null)
                                    {
                                        Log.Debug("Iterating grid to reselect row (!)");
                                        foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                                        {
                                            if ((string)row.Cells["Qualified Table Name"].Value ==
                                                selectedTableNameArgument)
                                            {
                                                row.Selected = true;
                                                tablesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                                break;
                                            }
                                        }

                                        selectedTableNameArgument = null;
                                    }
                                }
                            }

                            if (!tablesGridInitialized)
                            {
                                if (lastMainGridSettings != null)
                                {
                                    GridSettings.ApplySettingsToGrid(lastMainGridSettings, tablesGrid);

                                    tablesGridStatusMessage.Visible = false;
                                    tablesGridInitialized = true;
                                }
                                else if (snapshot.First.TableList.Rows.Count > 0)
                                {
                                    Log.Debug("Iterating grid columns to resize fragmentation status");
                                    foreach (UltraGridColumn column in tablesGrid.DisplayLayout.Bands[0].Columns)
                                    {
                                        if (column.Key != "Fragmentation Status")
                                        {
                                            Log.Debug("Performing autoresize on fragmentation status column");
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, indexesGrid.Width / 2);
                                        }
                                    }

                                    tablesGridStatusMessage.Visible = false;
                                    tablesGridInitialized = true;
                                }
                            }

                            // Hide 2005 only columns if necessary, but after currentSnapshot has been set
                            UltraGridBand band = tablesGrid.DisplayLayout.Bands[0];
                            if (showOnly2000Options)
                            {
                                // Note: Pinned is a SQL 7 and 2000 option only and NOT a 2005 option
                                // so handling of this column works backward from some other views
                                band.Columns["Pinned"].Hidden = false;
                                band.Columns["Pinned"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            }
                            else
                            {
                                band.Columns["Pinned"].Hidden = true;
                                band.Columns["Pinned"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            }

                            Log.Debug("Iterating grid to reselect row (!)");

                            if (selectedTableNameArgument != null || tableRebuilds.Count > 0)
                            {
                                foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                                {
                                    if (selectedTableNameArgument != null
                                        &&
                                        selectedTableNameArgument.Equals(
                                            (string)row.Cells["Qualified Table Name"].Value))
                                    {
                                        row.Selected = true;
                                        selectedTableNameArgument = null;
                                        tablesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                    }

                                    TableActionObject table;
                                    if (
                                        tableRebuilds.TryGetValue(
                                            new Pair<string, int>((string)row.Cells["Database"].Value,
                                                                  (int)row.Cells["Object Id"].Value), out table))
                                    {
                                        updateFragmentationCell(row, table);
                                    }

                                    //                                row.RefreshSortPosition(true);
                                }
                            }

                            Log.Debug("Refreshen grouping/sorting");
                            tablesGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);

                            if (selectedTableNameArgument != null)
                            {
                                Log.Debug("Doing new row reselect");
                                DataRow row = tableDataRowLookupTableByQualifiedName[""];
                                int rowIndex = row.Table.Rows.IndexOf(row);
                                UltraGridRow rowToSelect = tablesGrid.Rows.GetRowWithListIndex(rowIndex);
                                if (rowToSelect != null)
                                {
                                    rowToSelect.Selected = true;
                                    tablesGrid.ActiveColScrollRegion.ScrollRowIntoView(rowToSelect);
                                    Log.Debug("Row reselected");
                                }
                                selectedTableNameArgument = null;
                            }

                            Log.Debug("Resuming table events");
                            tablesGrid.ResumeLayout();

                            if (ActionsAllowedChanged != null)
                            {
                                ActionsAllowedChanged(this, EventArgs.Empty);
                            }

                            ApplicationController.Default.SetCustomStatus(
                                String.Format("{0} Table{1}",
                                              snapshot.First.TableList.Rows.Count,
                                              snapshot.First.TableList.Rows.Count == 1 ? string.Empty : "s")
                                );

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                        else
                        {
                            ShowTablesGridStatusMessage("Unable to update data for this view.");
                            ShowTableDetailsStatusMessage("Unable to update data for this view.");
                            ApplicationController.Default.ClearCustomStatus();
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.First.Error));
                        }
                    }
                }
            }
        }

        #endregion

        #region Context Menus

        private void tablesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = tablesGrid;
            ultraGridPrintDocument.DocumentName = "Tables Summary";

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = true;
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

                        ((ButtonTool)toolbarsManager.Tools["rebuildTableIndexesButton"]).SharedProps.Enabled = !(bool)((DataRowView)row.Row.ListObject)["Type"];
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridDataContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void dependenciesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = dependenciesGrid;
            ultraGridPrintDocument.DocumentName = "Table Dependencies";

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = false;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                }
            }
        }

        private void indexesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = indexesGrid;
            ultraGridPrintDocument.DocumentName = "Indexes Summary";

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = true;
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

        private void indexStatisticsColumnsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = indexStatisticsColumnsGrid;
            ultraGridPrintDocument.DocumentName = "Index Column Statistics";

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = false;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                }
            }
        }

        private void indexStatisticsDataDistributionGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = indexStatisticsDataDistributionGrid;
            ultraGridPrintDocument.DocumentName = "Index Data Distribution Statistics";

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = false;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridNoGroupsContextMenu");
                    }
                }
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
                case "updateTableStatisticsButton":
                    UpdateTableStatistics(selectedTableId);
                    break;
                case "rebuildTableIndexesButton":
                    RebuildTableIndexes(selectedTableId, true);
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
            }

            contextMenuSelectedChart = null;
        }

        public void UpdateTableStatistics()
        {
            Dictionary<Pair<string, int>, TableActionObject> selectedTables;
            if (GetActionSelections(TableAction.UpdateStatistics, out selectedTables) == DialogResult.OK)
            {
                LogAction(selectedTables.Values);

                //if selections were made, schedule the action
                foreach (TableActionObject table in selectedTables.Values)
                {
                    UpdateTableStatistics(table.TableId);
                }
            }
        }

        private void LogAction(Dictionary<Pair<string, int>, TableActionObject>.ValueCollection valueCollection)
        {
            AuditableEntity entity=new AuditableEntity();
            
            entity.MetadataProperties.Clear();            
            entity.Name = selectedDatabase;
            entity.AddMetadataProperty("Data Base Name", selectedDatabase);
            entity.AddMetadataProperty("Updated Tables include:", string.Empty);

            foreach (TableActionObject table in valueCollection)
            {
                entity.AddMetadataProperty("Table Name", table.TableName);
            }


            AuditingEngine.Instance.ManagementService =
                      ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            AuditingEngine.Instance.SQLUser =
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
            AuditingEngine.Instance.LogAction(entity, AuditableActionType.UpdateStatistics, AuditingEngine.Instance.SQLUser);
        }

        public void UpdateTableStatistics(int tableId)
        {
            Pair<string, int> key = new Pair<string, int>(selectedDatabase, tableId);
            pendingTableUpdates.Add(key);

            if (!updateStatisticsBackgroundWorker.IsBusy)
            {
                updateStatisticsBackgroundWorker.RunWorkerAsync();
            }
        }

        public void RebuildTableIndexes()
        {
            bool showMessage = true;
            Dictionary<Pair<string, int>, TableActionObject> selectedTables;
            if (GetActionSelections(TableAction.RebuildIndexes, out selectedTables) == DialogResult.OK)
            {
                //if selections were made, schedule the action
                foreach (TableActionObject table in selectedTables.Values)
                {
                    RebuildTableIndexes(table.TableId, showMessage);
                    showMessage = false;
                }
            }
        }

        public void RebuildTableIndexes(int tableId, bool showMessage)
        {
            DataRow tableRow;
            TableActionObject table;

            if (showMessage && !ApplicationController.Default.IsAutomaticRefreshPaused)
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, "SQL Diagnostic Manager may experience refresh errors due to exclusive locks held by the rebuild process. It is recommended that you pause the automatic refresh until this process completes.\n\nWould you like to pause the automatic refresh now?"))
                {
                    ApplicationController.Default.PauseAutomaticRefresh();
                }
            }

            if (tableDataRowLookupTableById.TryGetValue(tableId, out tableRow))
            {
                Pair<string, int> key = new Pair<string, int>(selectedDatabase, tableId);
                pendingTableRebuilds.Add(key);
                if (!tableRebuilds.TryGetValue(key, out table))
                {
                    table = new TableActionObject(selectedDatabase,
                                                    tableId,
                                                    (string)tableRow["Qualified Table Name"],
                                                    tableRow["% Fragmentation (Current)"] == DBNull.Value ?
                                                        (tableRow["% Fragmentation (Quiet Time)"] == DBNull.Value ?
                                                            null :
                                                            (decimal?)tableRow["% Fragmentation (Quiet Time)"]) :
                                                        (decimal?)tableRow["% Fragmentation (Current)"],
                                                    true);
                    tableRebuilds.Add(key, table);
                }

                table.Action = TableAction.RebuildIndexes;
                table.ActionError = null;
                table.ActionTime = null;
                table.Status = TableActionStatus.Scheduled;
                tableRow["Fragmentation Status"] = table.FragmentationMessage;
                foreach (UltraGridRow row in tablesGrid.Rows.GetAllNonGroupByRows())
                {
                    if ((int)row.Cells["Object Id"].Value == table.TableId)
                    {
                        updateFragmentationCell(row, table);
                        break;
                    }
                }
            }

            if (!rebuildIndexesBackgroundWorker.IsBusy)
            {
                rebuildIndexesBackgroundWorker.RunWorkerAsync();
            }
        }

        private DialogResult GetActionSelections(TableAction action, out Dictionary<Pair<string, int>, TableActionObject> selectedTables)
        {
            Dictionary<Pair<string, int>, TableActionObject> tableList = new Dictionary<Pair<string, int>, TableActionObject>();

            // build the list of tables and fragmentation statistics to pass to the selection dialog
            foreach (KeyValuePair<int, DataRow> row in tableDataRowLookupTableById)
            {
                // Rebuild indexes is not allowed on system tables
                if (action != TableAction.RebuildIndexes || !(bool)row.Value["Type"])
                {
                    tableList.Add(new Pair<string, int>(selectedDatabase, row.Key),
                                    new TableActionObject(selectedDatabase,
                                        (int)row.Value["Object Id"],
                                        (string)row.Value["Qualified Table Name"],
                                        row.Value["% Fragmentation (Current)"] == DBNull.Value ?
                                                (row.Value["% Fragmentation (Quiet Time)"] == DBNull.Value ?
                                                        null :
                                                        (decimal?)row.Value["% Fragmentation (Quiet Time)"]) :
                                                (decimal?)row.Value["% Fragmentation (Current)"],
                                        false)
                                    );
                }
            }
            if (tableList.Count > 0)
            {
                foreach (UltraGridRow row in tablesGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        Pair<string, int> key = new Pair<string, int>((string)row.Cells["Database"].Value, (int)row.Cells["Object Id"].Value);
                        // must check if key is in list because selection could be a system table
                        if (tableList.ContainsKey(key))
                        {
                            tableList[key].Selected = true;
                        }
                    }
                }
            }
            // show the selection dialog
            SelectTablesDialog dialog = new SelectTablesDialog(action, tableList, alertConfigItem);

            DialogResult result = dialog.ShowDialog(this);
            selectedTables = dialog.SelectedTables;

            return result;
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            if (selectedGrid == tablesGrid)
            {
                GridGroupByBoxVisible = !GridGroupByBoxVisible;
            }
            else if (selectedGrid == indexesGrid)
            {
                selectedGrid.DisplayLayout.GroupByBox.Hidden = !selectedGrid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null && selectedGrid != null)
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

        private void ShowColumnChooser()
        {
            if (selectedGrid != null)
            {
                SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(selectedGrid);
                dialog.Show(this);
            }
        }

        private void PrintGrid()
        {
            if (selectedGrid != null)
            {
                ultraGridPrintDocument.Grid = selectedGrid;
                ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
                ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
                ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - {1}",
                                  ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
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
                saveFileDialog.DefaultExt = "xls";
                saveFileDialog.FileName = "Databases";
                saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
                saveFileDialog.Title = "Save as Excel Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ultraGridExcelExporter.Export(selectedGrid, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while exporting data.", ex);
                    }
                }
            }
        }

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart(Chart chart)
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, chart.Tag as string, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(chart.Tag as string, true));
        }

        private void SaveChartImage(Chart chart)
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, chart.Tag as string, ExportHelper.GetValidFileName(chart.Tag as string, true));
        }

        private void tableSizeChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 2;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
            }
        }

        #endregion

        private void DatabasesTablesIndexesView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
