using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
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
using Infragistics.Windows.Themes;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesConfigurationView : ServerBaseView, IDatabasesView
    {
        #region constants

        private const string AllDatabasesItemText = "< All Databases >";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;
        private string selectedDatabaseFilter = null;
        private bool initialized = false;
        private bool includeSystemDatabases = true;
        SortedList<string, string> selectedDatabases = new SortedList<string, string>();
        private List<string> selectedDatabaseArgument = new List<string>();
        private static readonly object updateLock = new object();
        private UltraGridColumn selectedColumn = null;
        private DataTable configurationDataTable;
        private readonly Dictionary<string, DataRow> configurationDataRowLookupTable = new Dictionary<string, DataRow>();
        private DatabaseConfigurationSnapshot currentSnapshot = null;
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
        private GridSettings lastMainGridSettings = null;

        #endregion

        #region constructors

        public DatabasesConfigurationView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            configurationGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            InitializeDataTable();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            // load value lists for grid display
            ValueListItem listItem;

            configurationGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(RecoveryModel.Full, "Full");
            configurationGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(RecoveryModel.Bulk_Logged, "Bulk-logged");
            configurationGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(RecoveryModel.Simple, "Simple");
            configurationGrid.DisplayLayout.ValueLists["recoveryModelValueList"].ValueListItems.Add(listItem);

            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(6f, "SQL Server 6.0 (60)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(6.5f, "SQL Server 6.5 (65)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(7f, "SQL Server 7.0 (70)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(8f, "SQL Server 2000 (80)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(9f, "SQL Server 2005 (90)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(10f, "SQL Server 2008 (100)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(10.5f, "SQL Server 2008 R2 (105)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(11f, "SQL Server 2012 (110)");
            configurationGrid.DisplayLayout.ValueLists["compatibilityLevelValueList"].ValueListItems.Add(listItem);

            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING+" >");
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }
        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isLargeSize)
            {
                this.panel1.Location = new Point(this.panel1.Location.X, this.panel1.Location.Y + 5);
                return;
            }
            if(AutoScaleSizeHelper.isXLargeSize)
            {
                this.panel1.Location = new Point(this.panel1.Location.X, this.panel1.Location.Y + 8);
                return;
            }
            if(AutoScaleSizeHelper.isXXLargeSize)
            {
                this.panel1.Location = new Point(this.panel1.Location.X, this.panel1.Location.Y + 10);
                return;
            }
        }

        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public bool IncludeSystemDatabases
        {
            get { return includeSystemDatabases; }
            set
            {
                includeSystemDatabases = value;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();

                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !configurationGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                configurationGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public string SelectedDatabaseFilter
        {
            get { return selectedDatabaseFilter; }
        }

        #endregion

        #region methods

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesConfigurationView);
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
                configurationGrid.Selected.Rows.Clear();
                foreach (string database in selectedDatabaseArgument)
                {
                    foreach (UltraGridRow row in configurationGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((string)row.Cells["DatabaseName"].Value == database)
                        {
                            selectedDatabases.Add(database, database);
                            row.Selected = true;
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
            if (Settings.Default.DatabasesConfigurationViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.DatabasesConfigurationViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, configurationGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(configurationGrid);
            // save all settings only if anything has changed
            if (!mainGridSettings.Equals(lastMainGridSettings))
            {
                lastMainGridSettings =
                    Settings.Default.DatabasesConfigurationViewMainGrid = mainGridSettings;
            }
        }

        #region grid

        private void configurationsGrid_MouseDown(object sender, MouseEventArgs e)
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
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = configurationGrid.Rows.Count > 0 && configurationGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(configurationGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
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
                    configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    configurationGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            configurationGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            configurationGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(configurationGrid);
            dialog.Show(this);
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{0} - database configuration as of {1}",
                              ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "DatabaseConfiguration";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(configurationGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        #endregion

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesConfigurationView_Fill_Panel.Visible = true;
                configurationGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                base.RefreshView();
            }
            else
            {
                DatabasesConfigurationView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            DatabaseProbeConfiguration configuration = new DatabaseProbeConfiguration(instanceId, includeSystemDatabases);
            return (Snapshot)managementService.GetDatabaseConfiguration(configuration);
        }

        public override void UpdateData(object data)
        {
            if (data != null && data is DatabaseConfigurationSnapshot)
            {
                lock (updateLock)
                {
                    DatabaseConfigurationSnapshot snapshot = data as DatabaseConfigurationSnapshot;

                    if (snapshot.Error == null)
                    {
                        if (!databaseFilterInitialized)
                        {
                            UpdateDatabasesFilter(snapshot.ConfigurationSettings);
                        }

                        if (snapshot.ConfigurationSettings != null && snapshot.ConfigurationSettings.Rows.Count > 0)
                        {
                            UpdateDataTable(snapshot.ConfigurationSettings);
                            configurationGrid.Visible = true;

                            if (!initialized)
                            {
                                if (lastMainGridSettings != null)
                                {
                                    GridSettings.ApplySettingsToGrid(lastMainGridSettings, configurationGrid);

                                    initialized = true;
                                }
                                else if (snapshot.ConfigurationSettings.Rows.Count > 0)
                                {
                                    foreach (UltraGridColumn column in configurationGrid.DisplayLayout.Bands[0].Columns)
                                    {
                                        column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                        column.Width = Math.Min(column.Width, configurationGrid.Width / 2);
                                    }

                                    initialized = true;
                                }

                                if (configurationGrid.Rows.Count > 0 &&
                                    configurationGrid.Selected.Rows.Count == 0 &&
                                    selectedDatabaseFilter != null)
                                {
                                    configurationGrid.Rows[0].Selected = true;
                                }
                            }
                        }
                        else
                        {
                            configurationGridStatusLabel.Text = NO_ITEMS;
                            configurationGrid.Visible = false;
                            ApplicationController.Default.ClearCustomStatus();
                        }

                        currentSnapshot = snapshot;

                        // Hide 2005 only columns if necessary, but after currentSnapshot has been set
                        UltraGridBand band = configurationGrid.DisplayLayout.Bands[0];
                        if (showOnly2000Options)
                        {
                            band.Columns["IsParameterizationForced"].Hidden = true;
                            band.Columns["IsParameterizationForced"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsDateCorrelationOn"].Hidden = true;
                            band.Columns["IsDateCorrelationOn"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsVardecimalEnabled"].Hidden = true;
                            band.Columns["IsVardecimalEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["PageVerifyOption"].Hidden = true;
                            band.Columns["PageVerifyOption"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsAutoUpdateStatsAsyncOn"].Hidden = true;
                            band.Columns["IsAutoUpdateStatsAsyncOn"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsBrokerEnabled"].Hidden = true;
                            band.Columns["IsBrokerEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsTrustworthy"].Hidden = true;
                            band.Columns["IsTrustworthy"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["SnapshotIsolationState"].Hidden = true;
                            band.Columns["SnapshotIsolationState"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            band.Columns["IsTornPageDetectionEnabled"].Hidden = false;
                            band.Columns["IsTornPageDetectionEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                        }
                        else
                        {
                            band.Columns["IsParameterizationForced"].Hidden = false;
                            band.Columns["IsParameterizationForced"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsDateCorrelationOn"].Hidden = false;
                            band.Columns["IsDateCorrelationOn"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsVardecimalEnabled"].Hidden = false;
                            band.Columns["IsVardecimalEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["PageVerifyOption"].Hidden = false;
                            band.Columns["PageVerifyOption"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsAutoUpdateStatsAsyncOn"].Hidden = false;
                            band.Columns["IsAutoUpdateStatsAsyncOn"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsBrokerEnabled"].Hidden = false;
                            band.Columns["IsBrokerEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsTrustworthy"].Hidden = false;
                            band.Columns["IsTrustworthy"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["SnapshotIsolationState"].Hidden = false;
                            band.Columns["SnapshotIsolationState"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                            band.Columns["IsTornPageDetectionEnabled"].Hidden = true;
                            band.Columns["IsTornPageDetectionEnabled"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                        }

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        databasesFilterComboBox.Enabled = false;
                        databasesFilterComboBox.Items.Clear();
                        databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                        configurationGridStatusLabel.Text = UNABLE_TO_UPDATE;
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                    }
                }
            }
        }

        #endregion

        #region Data Table

        private void InitializeDataTable()
        {
            configurationDataTable = new DataTable();

            configurationDataTable.Columns.Add("DatabaseName", typeof(string));
            configurationDataTable.Columns.Add("Collation", typeof(string));
            configurationDataTable.Columns.Add("Compatibility", typeof(string));
            configurationDataTable.Columns.Add("IsAnsiNullDefault", typeof(bool));
            configurationDataTable.Columns.Add("IsAnsiNullsEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsAnsiPaddingEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsAnsiWarningsEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsArithmeticAbortEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsAutoClose", typeof(bool));
            configurationDataTable.Columns.Add("IsAutoCreateStatistics", typeof(bool));
            configurationDataTable.Columns.Add("IsAutoShrink", typeof(bool));
            configurationDataTable.Columns.Add("IsAutoUpdateStatistics", typeof(bool));
            configurationDataTable.Columns.Add("IsCloseCursorsOnCommitEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsFulltextEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsInStandBy", typeof(bool));
            configurationDataTable.Columns.Add("IsLocalCursorsDefault", typeof(bool));
            configurationDataTable.Columns.Add("IsMergePublished", typeof(bool));
            configurationDataTable.Columns.Add("IsNullConcat", typeof(bool));
            configurationDataTable.Columns.Add("IsNumericRoundAbortEnabled", typeof(bool));
            // IsParameterizationForced type is different from input data
            configurationDataTable.Columns.Add("IsParameterizationForced", typeof(string));
            configurationDataTable.Columns.Add("IsQuotedIdentifiersEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsPublished", typeof(bool));
            configurationDataTable.Columns.Add("IsRecursiveTriggersEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsSubscribed", typeof(bool));
            configurationDataTable.Columns.Add("IsSyncWithBackup", typeof(bool));
            configurationDataTable.Columns.Add("IsTornPageDetectionEnabled", typeof(bool));
            configurationDataTable.Columns.Add("Recovery", typeof(string));
            configurationDataTable.Columns.Add("Status", typeof(string));
            configurationDataTable.Columns.Add("Updateability", typeof(string));
            configurationDataTable.Columns.Add("UserAccess", typeof(string));
            configurationDataTable.Columns.Add("Version", typeof(int));
            configurationDataTable.Columns.Add("IsDbChainingOn", typeof(bool));

            //SQL 2005 Only
            configurationDataTable.Columns.Add("IsDateCorrelationOn", typeof(bool));
            configurationDataTable.Columns.Add("IsVardecimalEnabled", typeof(bool));
            configurationDataTable.Columns.Add("PageVerifyOption", typeof(string));
            configurationDataTable.Columns.Add("IsAutoUpdateStatsAsyncOn", typeof(bool));
            configurationDataTable.Columns.Add("IsBrokerEnabled", typeof(bool));
            configurationDataTable.Columns.Add("IsTrustworthy", typeof(bool));
            configurationDataTable.Columns.Add("SnapshotIsolationState", typeof(string));

            configurationGrid.DataSource = configurationDataTable;
        }

        private void UpdateDataTable(DataTable dataTable)
        {
            if (dataTable != null)
            {
                List<string> existingDatabases = new List<string>();
                foreach (string existingDatabase in configurationDataRowLookupTable.Keys)
                {
                    existingDatabases.Add(existingDatabase);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow existingRow;
                    string databaseName = row["DatabaseName"] as string;

                    if (configurationDataRowLookupTable.TryGetValue(databaseName, out existingRow))
                    {
                        existingRow["Collation"] = row["Collation"];
                        existingRow["Compatibility"] = row["Compatibility"];
                        existingRow["IsAnsiNullDefault"] = row["IsAnsiNullDefault"];
                        existingRow["IsAnsiNullsEnabled"] = row["IsAnsiNullsEnabled"];
                        existingRow["IsAnsiPaddingEnabled"] = row["IsAnsiPaddingEnabled"];
                        existingRow["IsAnsiWarningsEnabled"] = row["IsAnsiWarningsEnabled"];
                        existingRow["IsArithmeticAbortEnabled"] = row["IsArithmeticAbortEnabled"];
                        existingRow["IsAutoClose"] = row["IsAutoClose"];
                        existingRow["IsAutoCreateStatistics"] = row["IsAutoCreateStatistics"];
                        existingRow["IsAutoShrink"] = row["IsAutoShrink"];
                        existingRow["IsAutoUpdateStatistics"] = row["IsAutoUpdateStatistics"];
                        existingRow["IsCloseCursorsOnCommitEnabled"] = row["IsCloseCursorsOnCommitEnabled"];
                        existingRow["IsFulltextEnabled"] = row["IsFulltextEnabled"];
                        existingRow["IsInStandBy"] = row["IsInStandBy"];
                        existingRow["IsLocalCursorsDefault"] = row["IsLocalCursorsDefault"];
                        existingRow["IsMergePublished"] = row["IsMergePublished"];
                        existingRow["IsNullConcat"] = row["IsNullConcat"];
                        existingRow["IsNumericRoundAbortEnabled"] = row["IsNumericRoundAbortEnabled"];
                        existingRow["IsParameterizationForced"] = (row["IsParameterizationForced"] is System.DBNull) ? null : (object)(((bool)row["IsParameterizationForced"]) ? "Forced" : "Simple");
                        existingRow["IsQuotedIdentifiersEnabled"] = row["IsQuotedIdentifiersEnabled"];
                        existingRow["IsPublished"] = row["IsPublished"];
                        existingRow["IsRecursiveTriggersEnabled"] = row["IsRecursiveTriggersEnabled"];
                        existingRow["IsSubscribed"] = row["IsSubscribed"];
                        existingRow["IsSyncWithBackup"] = row["IsSyncWithBackup"];
                        existingRow["IsTornPageDetectionEnabled"] = row["IsTornPageDetectionEnabled"];
                        existingRow["Recovery"] = row["Recovery"];
                        existingRow["Status"] = row["Status"];
                        existingRow["Updateability"] = (row["Updateability"] is System.DBNull) ? null : (object)(((string)row["Updateability"]).Equals("Read Only", StringComparison.CurrentCultureIgnoreCase) ? true : false);
                        existingRow["UserAccess"] = row["UserAccess"];
                        existingRow["Version"] = row["Version"];
                        existingRow["IsDbChainingOn"] = row["IsDbChainingOn"];

                        existingRow["IsDateCorrelationOn"] = row["IsDateCorrelationOn"];
                        existingRow["IsVardecimalEnabled"] = row["IsVardecimalEnabled"];
                        existingRow["PageVerifyOption"] = row["PageVerifyOption"];
                        existingRow["IsAutoUpdateStatsAsyncOn"] = row["IsAutoUpdateStatsAsyncOn"];
                        existingRow["IsBrokerEnabled"] = row["IsBrokerEnabled"];
                        existingRow["IsTrustworthy"] = row["IsTrustworthy"];
                        existingRow["SnapshotIsolationState"] = row["SnapshotIsolationState"];

                        existingDatabases.Remove(databaseName);
                    }
                    else
                    {
                        DataRow newRow = configurationDataTable.NewRow();

                        newRow["DatabaseName"] = row["DatabaseName"];
                        newRow["Collation"] = row["Collation"];
                        newRow["Compatibility"] = row["Compatibility"];
                        newRow["IsAnsiNullDefault"] = row["IsAnsiNullDefault"];
                        newRow["IsAnsiNullsEnabled"] = row["IsAnsiNullsEnabled"];
                        newRow["IsAnsiPaddingEnabled"] = row["IsAnsiPaddingEnabled"];
                        newRow["IsAnsiWarningsEnabled"] = row["IsAnsiWarningsEnabled"];
                        newRow["IsArithmeticAbortEnabled"] = row["IsArithmeticAbortEnabled"];
                        newRow["IsAutoClose"] = row["IsAutoClose"];
                        newRow["IsAutoCreateStatistics"] = row["IsAutoCreateStatistics"];
                        newRow["IsAutoShrink"] = row["IsAutoShrink"];
                        newRow["IsAutoUpdateStatistics"] = row["IsAutoUpdateStatistics"];
                        newRow["IsCloseCursorsOnCommitEnabled"] = row["IsCloseCursorsOnCommitEnabled"];
                        newRow["IsFulltextEnabled"] = row["IsFulltextEnabled"];
                        newRow["IsInStandBy"] = row["IsInStandBy"];
                        newRow["IsLocalCursorsDefault"] = row["IsLocalCursorsDefault"];
                        newRow["IsMergePublished"] = row["IsMergePublished"];
                        newRow["IsNullConcat"] = row["IsNullConcat"];
                        newRow["IsNumericRoundAbortEnabled"] = row["IsNumericRoundAbortEnabled"];
                        newRow["IsParameterizationForced"] = (row["IsParameterizationForced"] is System.DBNull) ? null : (object)(((bool)row["IsParameterizationForced"]) ? "Forced" : "Simple");
                        newRow["IsQuotedIdentifiersEnabled"] = row["IsQuotedIdentifiersEnabled"];
                        newRow["IsPublished"] = row["IsPublished"];
                        newRow["IsRecursiveTriggersEnabled"] = row["IsRecursiveTriggersEnabled"];
                        newRow["IsSubscribed"] = row["IsSubscribed"];
                        newRow["IsSyncWithBackup"] = row["IsSyncWithBackup"];
                        newRow["IsTornPageDetectionEnabled"] = row["IsTornPageDetectionEnabled"];
                        newRow["Recovery"] = row["Recovery"];
                        newRow["Status"] = row["Status"];
                        newRow["Updateability"] = (row["Updateability"] is System.DBNull) ? null : (object)(((string)row["Updateability"]).Equals("Read Only", StringComparison.CurrentCultureIgnoreCase) ? true : false);
                        newRow["UserAccess"] = row["UserAccess"];
                        newRow["Version"] = row["Version"];
                        newRow["IsDbChainingOn"] = row["IsDbChainingOn"];

                        newRow["IsDateCorrelationOn"] = row["IsDateCorrelationOn"];
                        newRow["IsVardecimalEnabled"] = row["IsVardecimalEnabled"];
                        newRow["PageVerifyOption"] = row["PageVerifyOption"];
                        newRow["IsAutoUpdateStatsAsyncOn"] = row["IsAutoUpdateStatsAsyncOn"];
                        newRow["IsBrokerEnabled"] = row["IsBrokerEnabled"];
                        newRow["IsTrustworthy"] = row["IsTrustworthy"];
                        newRow["SnapshotIsolationState"] = row["SnapshotIsolationState"];

                        configurationDataTable.Rows.Add(newRow);
                        if (configurationGrid.Rows.Count > 0)
                        {
                            configurationGrid.Rows[configurationGrid.Rows.Count - 1].RefreshSortPosition();
                        }
                        configurationDataRowLookupTable.Add(databaseName, newRow);
                    }
                }

                // Remove databases that no longer exist
                foreach (string diffDatabase in existingDatabases)
                {
                    DataRow diffDataRow;

                    if (configurationDataRowLookupTable.TryGetValue(diffDatabase, out diffDataRow))
                    {
                        configurationDataTable.Rows.Remove(diffDataRow);
                        configurationDataRowLookupTable.Remove(diffDatabase);
                    }
                }
            }
        }

        #endregion

        private void DatabasesConfigurationView_Load(object sender, EventArgs e)
        {
            ApplySettings();
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }

        #endregion

        #region Databases Filter

        public void UpdateDatabasesFilter(DataTable configurationSettings)
        {
            databasesFilterComboBox.Items.Clear();

            if (configurationSettings != null && configurationSettings.Rows.Count > 0)
            {
                databasesFilterComboBox.Enabled = true;
                databasesFilterComboBox.Items.Add(null, AllDatabasesItemText);

                foreach (DataRow row in configurationSettings.Rows)
                {
                    databasesFilterComboBox.Items.Add(row["DatabaseName"], row["DatabaseName"] as string);
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
                    configurationDataTable.DefaultView.RowFilter = null;
                    ApplicationController.Default.SetCustomStatus(String.Format("Databases: {0} Item{1}",
                                                                                configurationDataTable.DefaultView.Count,
                                                                                configurationDataTable.DefaultView.Count == 1
                                                                                    ? string.Empty
                                                                                    : "s"));
                }
                else if (databasesFilterComboBox.Items.Count > 1)
                {
                    selectedDatabaseFilter = databasesFilterComboBox.SelectedItem.DisplayText;
                    configurationDataTable.DefaultView.RowFilter =
                        string.Format("[DatabaseName] = '{0}'", selectedDatabaseFilter.Replace("'", "''"));
                    configurationGrid.Selected.Rows.Clear();
                    UltraGridRow[] nonGroupByRows = configurationGrid.Rows.GetAllNonGroupByRows();
                    if (nonGroupByRows.Length > 0)
                    {
                        configurationGrid.Selected.Rows.Add(nonGroupByRows[0]);
                    }
                    ApplicationController.Default.SetCustomStatus("Filter Applied", "Databases: 1 Item");
                }

                configurationDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                configurationGrid.Focus();
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
        private void mouseEnter_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark" && appearance1 != null)
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefreshHover;
        }
        private void mouseLeave_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark" && appearance1 != null)
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            if (Settings.Default.ColorScheme == "Dark")
            {
                if (!refreshDatabasesButton.Enabled)
                    appearance1.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.configurationGrid);
        }
    }
}
