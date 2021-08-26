using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
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
using Idera.SQLdm.Common.Events;
using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Auditing;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Services
{
    internal partial class ServicesFullTextSearchView : ServerBaseView
    {
        #region constants

        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string SELECT_CATALOG = @"Select a catalog.";
        private const string SELECT_TABLE = @"Select a table.";
        private const string STATUS_NOT_INSTALLED = @"Full-Text Search is not installed on this SQL Server instance.";
        private const string STATUS_NOT_RUNNING = @"The Full-Text Search service is not currently started.";
        private const string STATUS_UNDETERMINED = @"The Full-Text Search service is currently in an unknown state.";
        private const int SQL_2000 = 8;
        private const int SQL_2005 = 9;
        private const int SQL_2008 = 10;

        private class Selections
        {
            public Pair<string, string> selectedCatalog = new Pair<string, string>(null, null);
            public long? selectedTable = null;
            public bool selectionChangedCatalog = false;
            public bool selectionChangedTable = false;
            public bool selectionChanged
            {
                get { return selectionChangedCatalog || selectionChangedTable; }
            }
        }

        #endregion

        #region fields
        private DateTime? historicalSnapshotDateTime;
        private bool initialized = false;
        private DataTable currentDataTable;
        private DataTable tablesDataTable;
        private DataTable columnsDataTable;
        private OnDemandConfiguration configuration;
        private FullTextTablesConfiguration configurationTables;
        private FullTextColumnsConfiguration configurationColumns;
        private FullTextCatalogs currentSnapshot = null;
        private FullTextTables currentSnapshotTables = null;
        private FullTextColumns currentSnapshotColumns = null;
        private Selections selections = new Selections();
        private Pair<string, string> selectedCatalog
        {
            get { return selections.selectedCatalog; }
            set
            {
                selections.selectedCatalog = value;
                selections.selectedTable = null;
                if (CatalogActionsAllowedChanged != null)
                {
                    CatalogActionsAllowedChanged(this, EventArgs.Empty);
                }
            }
        }
        private long? selectedTable
        {
            get { return selections.selectedTable; }
            set { selections.selectedTable = value; }
        }
        private bool selectionChangedCatalog
        {
            get { return selections.selectionChangedCatalog; }
            set
            {
                selections.selectionChangedCatalog = value;
                if (value)
                {
                    selections.selectionChangedTable = false;
                }
            }
        }
        private bool selectionChangedTable
        {
            get { return selections.selectionChangedTable; }
            set { selections.selectionChangedTable = value; }
        }
        private bool selectionChanged
        {
            get { return selections.selectionChanged; }
        }
        private ServiceState serviceState = ServiceState.UnableToMonitor;

        private static readonly object updateLock = new object();
        private UltraGrid selectedGrid = null;
        private UltraGridColumn selectedColumn = null;
        private Control focused = null;

        // get the version of the current instance from the snapshot or the app instance cache
        private int serverVersion
        {
            get
            {
                if (currentSnapshot != null)
                {
                    return currentSnapshot.ProductVersion!=null? currentSnapshot.ProductVersion.Major:0;//sqldm-30299 changes
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
                        return SQL_2005;
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
        private bool show2000Actions
        {
            get
            {
                return serverVersion < 9;
            }
        }
        private bool show2005Actions
        {
            get
            {
                return !show2000Actions;
            }
        }

        //last Settings values used to determine if changed for saving when leaving
        private int lastMainSplitterDistance = 0;
        private int lastCatalogSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastTablesGridSettings = null;
        private GridSettings lastColumnsGridSettings = null;
        private bool lastDetailPanelVisible = true;
        private FullTextCatalogs lastCatalogSnapshot = null;

        #endregion

        #region constructors

        public ServicesFullTextSearchView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            HideFocusRectangleDrawFilter hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();
            catalogsGrid.DrawFilter = hideFocusRectangleDrawFilter;
            tablesGrid.DrawFilter = hideFocusRectangleDrawFilter;
            columnsGrid.DrawFilter = hideFocusRectangleDrawFilter;

            catalogsGrid.Tag = "Full-Text Search catalogs";
            tablesGrid.Tag = "Full-Text Search catalog tables";
            columnsGrid.Tag = "Full-Text Search catalog columns";

            configuration = new OnDemandConfiguration(instanceId);
            configurationTables = new FullTextTablesConfiguration(instanceId, string.Empty, string.Empty);
            configurationColumns = new FullTextColumnsConfiguration(instanceId, string.Empty, null);

            // load value lists for grid display
            ValueListItem listItem;

            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(FullTextSearchPopulationStatus.BuildingIndex, "Building Index");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.ChangeTracking, "Change Tracking");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.FullPopulationInProgress, "Full Population In Progress");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.Idle, "Idle");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.IncrementalPopulationInProgress, "Incremental Population In Progress");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.Paused, "Paused");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.PausedDiskIsFull, "Paused Disk Is Full");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.Recoverying, "Recovering");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.Shutdown, "Shutdown");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(FullTextSearchPopulationStatus.Throttled, "Throttled");
            catalogsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);

            catalogsGridStatusLabel.Text =
                tablesGridStatusLabel.Text =
                columnsGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            catalogsGridStatusLabel.BringToFront();
            tablesGridStatusLabel.BringToFront();
            columnsGridStatusLabel.BringToFront();
            catalogsGridStatusLabel.Visible =
                tablesGridStatusLabel.Visible =
                columnsGridStatusLabel.Visible = true;

            InitializeCurrentDataTable();
            InitializeTablesDataTable();
            InitializeColumnsDataTable();
            AdaptFontSize();
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
             SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        private void ScaleControlsAsPerResolution()
        {
            this.catalogsGridStatusLabel.Location = new System.Drawing.Point(0, 34);
            this.tablesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.catalogsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.columnsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.tablesGridStatusLabel.Location = new System.Drawing.Point(0, 34);
            this.columnsGridStatusLabel.Location = new System.Drawing.Point(0, 34);
        }
        #endregion

        #region Properties

        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler DetailsPanelVisibleChanged;
        public event EventHandler CatalogActionsAllowedChanged;
        public event EventHandler ServiceActionsAllowedChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public FullTextCatalogs CurrentSnapshot
        {
            get { return currentSnapshot; }
        }

        /// <summary>
        /// Get or Set the Service state and trigger state update event if changed
        /// </summary>
        public ServiceState FullTextSearchServiceState
        {
            get
            {
                return serviceState;
            }
            private set
            {
                serviceState = value;

                if (ServiceActionsAllowedChanged != null)
                {
                    ServiceActionsAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the availability of starting the service determined by its current state
        /// </summary>
        public bool StartAllowed
        {
            get { return serviceState == ServiceState.Stopped; }
        }

        /// <summary>
        /// Get the availability of stopping the service determined by its current state
        /// </summary>
        public bool StopAllowed
        {
            get { return serviceState == ServiceState.Running; }
        }

        /// <summary>
        /// Get the availability of Rebuilding a catalog determined by whether a service is currently selected
        /// </summary>
        public bool ActionsAllowed
        {
            get { return selectedCatalog.First != null && selectedCatalog.Second != null; }
        }

        /// <summary>
        /// Get the availability of Optimizing a catalog determined by the server version
        /// </summary>
        public bool OptimizeAvailable
        {
            get
            {
                return show2005Actions;
            }
        }

        /// <summary>
        /// Get the availability of Repopulating a service determined by the server version
        /// </summary>
        public bool RepopulateAvailable
        {
            get
            {
                return show2000Actions;
            }
        }

        /// <summary>
        /// Get the availability of Rebuilding a catalog -- currently always true
        /// </summary>
        public bool RebuildAvailable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Get or Set the Databases Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !catalogsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                catalogsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current database configuration settings to manage state for current selections
        /// </summary>
        public OnDemandConfiguration Configuration
        {
            get { return configuration; }
        }

        public bool DetailsPanelVisible
        {
            get { return !splitContainer1.Panel2Collapsed; }
            set
            {
                splitContainer1.Panel2Collapsed = !value;

                if (DetailsPanelVisibleChanged != null)
                {
                    DetailsPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region methods


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServicesFullTextSearchView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance = splitContainer1.Height - Settings.Default.ServicesFullTextViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer1.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer1.Height - splitContainer1.SplitterDistance;
            }

            // Fixed panel is first panel, so just restore splitter distance
            lastCatalogSplitterDistance =
               splitContainer2.SplitterDistance = Settings.Default.ServicesFullTextViewCatalogSplitter;

            lastDetailPanelVisible =
                DetailsPanelVisible = Settings.Default.ServicesFullTextViewDetailsVisible;

            if (Settings.Default.ServicesFullTextViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServicesFullTextViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, catalogsGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            if (Settings.Default.ServicesFullTextViewTablesGrid is GridSettings)
            {
                lastTablesGridSettings = Settings.Default.ServicesFullTextViewTablesGrid;
                GridSettings.ApplySettingsToGrid(lastTablesGridSettings, tablesGrid);
            }

            if (Settings.Default.ServicesFullTextViewColumnsGrid is GridSettings)
            {
                lastColumnsGridSettings = Settings.Default.ServicesFullTextViewColumnsGrid;
                GridSettings.ApplySettingsToGrid(lastColumnsGridSettings, columnsGrid);
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(catalogsGrid);
            GridSettings tablesGridSettings = GridSettings.GetSettings(tablesGrid);
            GridSettings columnsGridSettings = GridSettings.GetSettings(columnsGrid);

            // save all settings only if anything has changed
            if (lastMainSplitterDistance != splitContainer1.Height - splitContainer1.SplitterDistance
                || lastCatalogSplitterDistance != splitContainer2.SplitterDistance
                || lastDetailPanelVisible != DetailsPanelVisible
                || !mainGridSettings.Equals(lastMainGridSettings)
                || !tablesGridSettings.Equals(lastTablesGridSettings)
                || !columnsGridSettings.Equals(lastColumnsGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastMainSplitterDistance =
                    Settings.Default.ServicesFullTextViewMainSplitter = splitContainer1.Height - splitContainer1.SplitterDistance;
                lastCatalogSplitterDistance =
                    Settings.Default.ServicesFullTextViewCatalogSplitter = splitContainer2.SplitterDistance;
                lastDetailPanelVisible =
                    Settings.Default.ServicesFullTextViewDetailsVisible = DetailsPanelVisible;
                lastMainGridSettings =
                    Settings.Default.ServicesFullTextViewMainGrid = mainGridSettings;
                lastTablesGridSettings =
                    Settings.Default.ServicesFullTextViewTablesGrid = tablesGridSettings;
                lastColumnsGridSettings =
                    Settings.Default.ServicesFullTextViewColumnsGrid = columnsGridSettings;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ServicesFullTextSearchView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                ServicesFullTextSearchView_Fill_Panel.Visible = false;
                viewStatusLabel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Pair<Selections, Triple<FullTextCatalogs, FullTextTables, FullTextColumns>> configs = new Pair<Selections,Triple<FullTextCatalogs,FullTextTables,FullTextColumns>>();
            Triple<FullTextCatalogs, FullTextTables, FullTextColumns> snapshots = new Triple<FullTextCatalogs, FullTextTables, FullTextColumns>();

            if (!selectionChanged)
            {
                snapshots.First = managementService.GetFullTextCatalogs(configuration);
                lastCatalogSnapshot = snapshots.First;
            }
            
            if (
                (lastCatalogSnapshot != null && lastCatalogSnapshot.ProductVersion != null && lastCatalogSnapshot.ProductVersion.Major > 9) ||
                (FullTextSearchServiceState != ServiceState.NotInstalled && FullTextSearchServiceState != ServiceState.Stopped))
            {
                if (!selectionChangedTable)
                {
                    if (snapshots.First == null ||
                        (snapshots.First != null && snapshots.First.Error == null))
                    {
                        configurationTables.DatabaseName = selectedCatalog.First;
                        configurationTables.CatalogName = selectedCatalog.Second;

                        snapshots.Second = managementService.GetFullTextTables(configurationTables);
                    }
                }

                configurationColumns.DatabaseName = configurationTables.DatabaseName;
                if (configurationColumns.DatabaseName != null)
                {
                    //if (selectedTable != null)
                    //{
                    configurationColumns.TableId = selectedTable;

                    snapshots.Third = managementService.GetFullTextColumns(configurationColumns);
                    //}
                }
            }

            configs.First = selections;
            configs.Second = snapshots;

            return configs;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            catalogsGridStatusLabel.Text =
                tablesGridStatusLabel.Text =
                columnsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                 base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Selections passedSelections;
            Triple<FullTextCatalogs, FullTextTables, FullTextColumns> snapshotTriple;
            Exception e = null;

            if (data != null && data is Pair<Selections, Triple<FullTextCatalogs, FullTextTables, FullTextColumns>>)
            {
                passedSelections = ((Pair<Selections, Triple<FullTextCatalogs, FullTextTables, FullTextColumns>>)data).First;
                snapshotTriple = ((Pair<Selections, Triple<FullTextCatalogs, FullTextTables, FullTextColumns>>)data).Second;
                if (snapshotTriple.First != null)
                {
                    FullTextCatalogs snapshot = snapshotTriple.First as FullTextCatalogs;
                    lock (updateLock)
                    {
                        if (snapshot.Error == null)
                        {
                            // this needs to be set first to determine ribbon button states correctly
                            currentSnapshot = snapshot;

                            // set the current service state
                            FullTextSearchServiceState = snapshot.FullTextServiceStatus;

                            // set the catalog service states
                            if (CatalogActionsAllowedChanged != null)
                            {
                                CatalogActionsAllowedChanged(null, new EventArgs());
                            }

                            if (snapshot.FullTextServiceStatus == ServiceState.Running || (snapshot.ProductVersion!=null &&  snapshot.ProductVersion.Major > 9))
                            {
                                if (snapshot.Catalogs != null && snapshot.Catalogs.Rows.Count > 0)
                                {
                                    currentDataTable.BeginLoadData();

                                    // remove any databases that have been deleted
                                    List<DataRow> deleteRows = new List<DataRow>();
                                    foreach (DataRow row in currentDataTable.Rows)
                                    {
                                        if (!snapshot.Catalogs.Rows.Contains(new object[] { (string)row["Database"], (string)row["Catalog"] }))
                                        {
                                            deleteRows.Add(row);
                                        }
                                    }
                                    foreach (DataRow row in deleteRows)
                                    {
                                        currentDataTable.Rows.Remove(row);
                                    }

                                    //now update any matching databases or add new ones
                                    foreach (DataRow catalog in snapshot.Catalogs.Rows)
                                    {
                                        currentDataTable.LoadDataRow(
                                                new object[]
                                        {
                                            catalog["Catalog"],
                                            catalog["Database"],
                                            catalog["PopulationStatus"],
                                            catalog["LastPopulation"] is System.DBNull ? null : (object)((DateTime)catalog["LastPopulation"]).ToLocalTime(),
                                            catalog["Size"] is System.DBNull ? null : ((FileSize)catalog["Size"]).Megabytes,
                                            catalog["IsDefault"],
                                            catalog["Accent Sensitivity"],
                                            catalog["Owner"],
                                            catalog["Filegroup"],
                                            catalog["TableCount"],
                                            catalog["ItemCount"],
                                            catalog["KeyCount"],
                                            catalog["Path"]
                                        }, true);
                                    }

                                    currentDataTable.EndLoadData();

                                    if (!initialized)
                                    {
                                        if (lastMainGridSettings != null)
                                        {
                                            GridSettings.ApplySettingsToGrid(lastMainGridSettings, catalogsGrid);

                                            initialized = true;
                                        }
                                        else if (snapshot.Catalogs.Rows.Count > 0)
                                        {
                                            foreach (UltraGridColumn column in catalogsGrid.DisplayLayout.Bands[0].Columns)
                                            {
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            }

                                            initialized = true;
                                        }

                                        if (catalogsGrid.Rows.Count > 0 && catalogsGrid.Selected.Rows.Count == 0)
                                        {
                                            catalogsGrid.Rows[0].Selected = true;
                                        }
                                    }

                                    // note RefreshSort is causing the selection to change here for some reason
                                    UltraGridRow[] gridRows = catalogsGrid.Rows.GetAllNonGroupByRows();
                                    foreach (UltraGridRow row in gridRows)
                                    {
                                        row.RefreshSortPosition();
                                    }
                                    //catalogsGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                    catalogsGridStatusLabel.Visible = false;

                                    ApplicationController.Default.SetCustomStatus(
                                        String.Format("{0} Catalog{1}",
                                                currentDataTable.Rows.Count,
                                                currentDataTable.Rows.Count == 1 ? string.Empty : "s")
                                        );
                                }
                                else
                                {
                                    currentDataTable.Clear();
                                    catalogsGridStatusLabel.Text = NO_ITEMS;
                                    catalogsGridStatusLabel.BringToFront();
                                    catalogsGridStatusLabel.Visible = true;
                                    tablesDataTable.Clear();
                                    tablesGridStatusLabel.Text = SELECT_CATALOG;
                                    tablesGridStatusLabel.BringToFront();
                                    tablesGridStatusLabel.Visible = true;
                                    columnsDataTable.Clear();
                                    columnsGridStatusLabel.Text = SELECT_CATALOG;
                                    columnsGridStatusLabel.BringToFront();
                                    columnsGridStatusLabel.Visible = true;
                                    ApplicationController.Default.ClearCustomStatus();
                                }
                                viewStatusLabel.Visible = false;
                            }
                            else
                            {
                                currentDataTable.Clear();
                                tablesDataTable.Clear();
                                columnsDataTable.Clear();
                                switch (snapshot.FullTextServiceStatus)
                                {
                                    case ServiceState.Stopped:
                                        viewStatusLabel.Text = STATUS_NOT_RUNNING;
                                        break;
                                    case ServiceState.NotInstalled:
                                        viewStatusLabel.Text = STATUS_NOT_INSTALLED;
                                        break;
                                    default:
                                        viewStatusLabel.Text = STATUS_UNDETERMINED;
                                        break;
                                }
                                viewStatusLabel.Visible = true;
                                viewStatusLabel.BringToFront();
                                ApplicationController.Default.ClearCustomStatus();
                            }

                            // Hide 2005 only columns if necessary, but after currentSnapshot has been set
                            UltraGridBand band = catalogsGrid.DisplayLayout.Bands[0];

                            switch (serverVersion)
                            {
                                case SQL_2000:
                                    band.Columns["Default"].Hidden =
                                        band.Columns["Accent Sensitive"].Hidden =
                                        band.Columns["Owner"].Hidden =
                                        band.Columns["File Group"].Hidden = true;
                                    band.Columns["Path"].Hidden = false;
                                    band.Columns["Default"].ExcludeFromColumnChooser =
                                        band.Columns["Accent Sensitive"].ExcludeFromColumnChooser =
                                        band.Columns["Owner"].ExcludeFromColumnChooser =
                                        band.Columns["File Group"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                                    band.Columns["Path"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                                    break;
                                case SQL_2008:
                                    band.Columns["File Group"].Hidden =
                                        band.Columns["Path"].Hidden = true;
                                    band.Columns["File Group"].ExcludeFromColumnChooser =
                                        band.Columns["Path"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                                    break;
                                default:
                                    band.Columns["Default"].Hidden =
                                        band.Columns["Accent Sensitive"].Hidden =
                                        band.Columns["Owner"].Hidden =
                                        band.Columns["Path"].Hidden = 
                                        band.Columns["File Group"].Hidden = false;
                                    band.Columns["Default"].ExcludeFromColumnChooser =
                                        band.Columns["Accent Sensitive"].ExcludeFromColumnChooser =
                                        band.Columns["Owner"].ExcludeFromColumnChooser =
                                        band.Columns["Path"].ExcludeFromColumnChooser = 
                                        band.Columns["File Group"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                                    break;
                            }

                        }
                        else
                        {
                            e = snapshot.Error;
                            FullTextSearchServiceState = ServiceState.UnableToMonitor;
                            currentDataTable.Clear();
                            catalogsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            catalogsGridStatusLabel.BringToFront();
                            catalogsGridStatusLabel.Visible = true;
                            tablesDataTable.Clear();
                            tablesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            tablesGridStatusLabel.BringToFront();
                            tablesGridStatusLabel.Visible = true;
                            columnsDataTable.Clear();
                            columnsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            columnsGridStatusLabel.BringToFront();
                            columnsGridStatusLabel.Visible = true;
                            ApplicationController.Default.ClearCustomStatus();
                        }
                    }
                }

                if (snapshotTriple.Second != null)
                {
                    lock (updateLock)
                    {
                        FullTextTables snapshot = snapshotTriple.Second as FullTextTables;

                        if (snapshot.Error == null)
                        {
                            // Load Tables grid data
                            tablesDataTable.BeginLoadData();

                            // remove any Tables that are no longer in view
                            if (passedSelections.selectionChangedCatalog)
                            {
                                tablesDataTable.Clear();
                            }
                            else
                            {
                                List<DataRow> deleteRows = new List<DataRow>();
                                foreach (DataRow row in tablesDataTable.Rows)
                                {
                                    if (!snapshot.Tables.Rows.Contains((long)row["TableId"]))
                                    {
                                        deleteRows.Add(row);
                                    }
                                }
                                foreach (DataRow row in deleteRows)
                                {
                                    tablesDataTable.Rows.Remove(row);
                                }
                            }

                            if (snapshot.Tables.Rows.Count > 0)
                            {
                                foreach (DataRow row in snapshot.Tables.Rows)
                                {
                                    tablesDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        row["TableID"],
                                        string.Concat(
                                            row["Schema name"] == null ? string.Empty : 
                                                                        string.Concat(row["Schema name"],"."),
                                            row["Name"] == null ? string.Empty : row["Name"]
                                        ),
                                        row["Rows"]
                                    }, true);
                                }

                                tablesDataTable.EndLoadData();

                                if (tablesGrid.Rows.Count > 0 && (passedSelections.selectionChangedCatalog || tablesGrid.Selected.Rows.Count == 0))
                                {
                                    tablesGrid.Rows.GetAllNonGroupByRows()[0].Selected = true;
                                }

                                // note RefreshSort is causing the selection to change here for some reason
                                UltraGridRow[] gridRows = tablesGrid.Rows.GetAllNonGroupByRows();
                                foreach (UltraGridRow row in gridRows)
                                {
                                    row.RefreshSortPosition();
                                }
                                //tablesGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                tablesGridStatusLabel.Visible = false;
                            }
                            else
                            {
                                tablesDataTable.Clear();
                                tablesDataTable.EndLoadData();
                                tablesGridStatusLabel.Text = NO_ITEMS;
                                tablesGridStatusLabel.BringToFront();
                                tablesGridStatusLabel.Visible = true;
                                columnsDataTable.Clear();
                                columnsGridStatusLabel.Text = NO_ITEMS;
                                columnsGridStatusLabel.BringToFront();
                                columnsGridStatusLabel.Visible = true;
                            }

                            currentSnapshotTables = snapshot;
                            // reset selectionChanged to enable full catalog refreshes again
                            selectionChangedCatalog = false;
                        }
                        else
                        {
                            if (e == null && (passedSelections.selectedCatalog.First == null || passedSelections.selectedCatalog.Second == null))
                            {
                                tablesGridStatusLabel.Text =
                                    columnsGridStatusLabel.Text = (currentDataTable.Rows.Count > 0 && passedSelections.selectedCatalog.First == null) ? SELECT_CATALOG : NO_ITEMS;
                            }
                            else
                            {
                                e = snapshot.Error;
                                tablesGridStatusLabel.Text =
                                    columnsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            }
                            tablesGridStatusLabel.BringToFront();
                            tablesGridStatusLabel.Visible = true;
                            columnsGridStatusLabel.BringToFront();
                            columnsGridStatusLabel.Visible = true;
                        }
                    }
                }

                if (snapshotTriple.Third != null)
                {
                    lock (updateLock)
                    {
                        FullTextColumns snapshot = snapshotTriple.Third as FullTextColumns;

                        if (snapshot.Error == null)
                        {
                            // Load Columns grid data
                            columnsDataTable.BeginLoadData();
                            // always clear the old columns and repopulate
                            columnsDataTable.Clear();

                            if (snapshot.FtColumns.Rows.Count > 0)
                            {
                                foreach (DataRow row in snapshot.FtColumns.Rows)
                                {
                                    columnsDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        row["Name"],
                                        row["Fulltext Indexed"],
                                        row["Language"],
                                        row["Type"]
                                    }, true);
                                }

                                columnsDataTable.EndLoadData();

                                columnsGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                columnsGridStatusLabel.Visible = false;
                            }
                            else
                            {
                                columnsDataTable.Clear();
                                columnsDataTable.EndLoadData();
                                if (e == null)
                                {
                                    columnsGridStatusLabel.Text = (tablesDataTable.Rows.Count > 0 && passedSelections.selectedTable == null) ? SELECT_TABLE : NO_ITEMS;
                                }
                                columnsGridStatusLabel.BringToFront();
                                columnsGridStatusLabel.Visible = true;
                            }

                            currentSnapshotColumns = snapshot;
                            // reset selectionChanged to enable table refreshes again
                            selectionChangedTable = false;
                        }
                        else
                        {
                            e = snapshot.Error;
                            columnsDataTable.Clear();
                            columnsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                            columnsGridStatusLabel.BringToFront();
                            columnsGridStatusLabel.Visible = true;
                        }
                    }
                }
            }
            else
            {
                currentDataTable.Clear();
                tablesDataTable.Clear();
                columnsDataTable.Clear();
                FullTextSearchServiceState = ServiceState.UnableToMonitor;
                catalogsGridStatusLabel.Text =
                    tablesGridStatusLabel.Text =
                    columnsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                catalogsGridStatusLabel.BringToFront();
                tablesGridStatusLabel.BringToFront();
                columnsGridStatusLabel.BringToFront();
                catalogsGridStatusLabel.Visible =
                    tablesGridStatusLabel.Visible =
                    columnsGridStatusLabel.Visible = false;
                ApplicationController.Default.ClearCustomStatus();
            }

            if (FullTextSearchServiceState == ServiceState.Running)
            {
                catalogsGrid.DisplayLayout.Bands[0].Columns["Last Population Date"].NullText = "Never";
            }
            else
            {
                catalogsGrid.DisplayLayout.Bands[0].Columns["Last Population Date"].NullText = string.Empty;
            }

            AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
            if (alertConfig != null)
            {
                foreach (UltraGridRow gridRow in catalogsGrid.Rows.GetAllNonGroupByRows())
                {
                    UpdateCellColor(Metric.FullTextRefreshHours, alertConfig, gridRow, "Last Population Date", FullTextSearchServiceState == ServiceState.Running || FullTextSearchServiceState == ServiceState.NotInstalled ? 1 : 0);
                }
            }

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
            AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
            if (alertConfigItem != null)
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName) && metric != Metric.FullTextRefreshHours)
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {
                        object value = dataRow[columnName];
                        if (metric == Metric.FullTextRefreshHours)
                        {
                            if (value == DBNull.Value)
                                value = DateTime.MinValue;
                            
                            TimeSpan diff = DateTime.Now - ((DateTime)value);
                            value = (diff.TotalHours < 0) ? 0 : diff.TotalHours;
                        }
                        if (value != null && adjustmentMultiplier != 1)
                        {
                            double dbl = (double)Convert.ChangeType(value, typeof(double));
                            value = dbl * adjustmentMultiplier;
                        }
                        switch (alertConfigItem.GetSeverity((IComparable)value))
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

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        public void StartService()
        {
            PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Start);
        }

        public void StopService()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to stop the {0} service on {1}. Do you want to continue?",
                                        ApplicationHelper.GetEnumDescription(ServiceName.FullTextSearch),
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Stop);
            }
        }

        public void OptimizeCatalog()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to optimize the catalog {0} in database {1} on {2}. Do you want to continue?",
                                        selectedCatalog.Second,
                                        selectedCatalog.First,
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformAction(FullTextActionConfiguration.FullTextAction.Optimize);
            }
        }

        public void RebuildCatalog()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to rebuild the catalog {0} in database {1} on {2}. Do you want to continue?",
                                        selectedCatalog.Second,
                                        selectedCatalog.First,
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformAction(FullTextActionConfiguration.FullTextAction.Rebuild);
            }
        }

        public void RepopulateCatalog()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to repopulate the catalog {0} in database {1} on {2}. Do you want to continue?",
                                        selectedCatalog.Second,
                                        selectedCatalog.First,
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformAction(FullTextActionConfiguration.FullTextAction.Repopulate);
            }
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

        private void PerformServiceAction(ServiceControlConfiguration.ServiceControlAction action)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            FullTextSearchServiceState = ServiceState.UnableToMonitor;
            ServiceControlConfiguration config = new ServiceControlConfiguration(instanceId, ServiceName.FullTextSearch, action);

            AuditingEngine.SetContextData(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            Snapshot snapshot = managementService.SendServiceControl(config);

            if (snapshot.Error == null)
            {
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            else
            {
                ApplicationMessageBox.ShowError(this, String.Format("Unable to {0} the {1} service.",
                                                                    ApplicationHelper.GetEnumDescription(action),
                                                                    ApplicationHelper.GetEnumDescription(ServiceName.FullTextSearch)),
                                                snapshot.Error);
            }
        }

        private void PerformAction(FullTextActionConfiguration.FullTextAction action)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            FullTextActionConfiguration config = new FullTextActionConfiguration(instanceId, selectedCatalog.First, selectedCatalog.Second, action);

            AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            Snapshot snapshot = managementService.SendFullTextAction(config);

            if (snapshot.Error == null)
            {
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            else
            {
                ApplicationMessageBox.ShowError(this, String.Format("Unable to {0} the {1} catalog in database {2}.",
                                                                    ApplicationHelper.GetEnumDescription(action),
                                                                    selectedCatalog.Second,
                                                                    selectedCatalog.First),
                                                snapshot.Error);
            }
        }

        #region DataTables

        private void InitializeCurrentDataTable()
        {
            currentDataTable = new DataTable();
            currentDataTable.Columns.Add("Catalog", typeof(string));
            currentDataTable.Columns.Add("Database", typeof(string));
            currentDataTable.Columns.Add("Status", typeof(int));
            currentDataTable.Columns.Add("Last Population Date", typeof(DateTime));
            currentDataTable.Columns.Add("Size (MB)", typeof(decimal));
            currentDataTable.Columns.Add("Default", typeof(string));
            currentDataTable.Columns.Add("Accent Sensitive", typeof(string));
            currentDataTable.Columns.Add("Owner", typeof(string));
            currentDataTable.Columns.Add("File Group", typeof(string));
            currentDataTable.Columns.Add("Tables", typeof(long));
            currentDataTable.Columns.Add("Item Count", typeof(long));
            currentDataTable.Columns.Add("Unique Keys", typeof(long));
            currentDataTable.Columns.Add("Path", typeof(string));

            currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns["Database"], currentDataTable.Columns["Catalog"] };
            currentDataTable.CaseSensitive = true;

            currentDataTable.DefaultView.Sort = "[Database], [Catalog]";

            catalogsGrid.DataSource = currentDataTable;
        }

        private void InitializeTablesDataTable()
        {
            tablesDataTable = new DataTable();
            tablesDataTable.Columns.Add("TableId", typeof(long));
            tablesDataTable.Columns.Add("TableName", typeof(string));
            tablesDataTable.Columns.Add("Rows", typeof(long));

            tablesDataTable.PrimaryKey = new DataColumn[] { tablesDataTable.Columns["TableId"] };
            tablesDataTable.CaseSensitive = true;

            tablesDataTable.DefaultView.Sort = "[TableName]";

            tablesGrid.DataSource = tablesDataTable;
        }

        private void InitializeColumnsDataTable()
        {
            columnsDataTable = new DataTable();
            columnsDataTable.Columns.Add("Eligible Column", typeof(string));
            columnsDataTable.Columns.Add("In Catalog", typeof(string));
            columnsDataTable.Columns.Add("Language", typeof(string));
            columnsDataTable.Columns.Add("Column Data Type", typeof(string));

            columnsDataTable.PrimaryKey = new DataColumn[] { tablesDataTable.Columns["Eligible Column"] };
            columnsDataTable.CaseSensitive = true;

            columnsDataTable.DefaultView.Sort = "[Eligible Column]";

            columnsGrid.DataSource = columnsDataTable;
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
            if (grid == catalogsGrid)
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
                title = "Full-Text Search";
            }
            ultraGridPrintDocument.DocumentName = title;
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
                title = "FullTextSearch";
            }
            title = ExportHelper.GetValidFileName(title, true);

            saveFileDialog.FileName = title;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(grid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        #endregion

        #endregion

        #region Event Handlers

        #region Splitter

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // When there are multiple splitters on a view, this must be saved manually
            Settings.Default.ServicesFullTextViewMainSplitter = ((SplitContainer)sender).SplitterDistance;
        }

        private void splitContainer2_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer2_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // When there are multiple splitters on a view, this must be saved manually
            Settings.Default.ServicesFullTextViewCatalogSplitter = ((SplitContainer)sender).SplitterDistance;
        }

        #endregion

        private void hideCatalogDetailsButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = false;
        }

        #region toolbars

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu")
            {
                ((PopupMenuTool)e.Tool).Tools["optimizeCatalogButton"].SharedProps.Visible = OptimizeAvailable &&
                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                ((PopupMenuTool)e.Tool).Tools["repopulateCatalogButton"].SharedProps.Visible = RepopulateAvailable &&
                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                ((PopupMenuTool)e.Tool).Tools["rebuildCatalogButton"].SharedProps.Visible = RebuildAvailable &&
                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;

                ((PopupMenuTool)e.Tool).Tools["optimizeCatalogButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["repopulateCatalogButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["rebuildCatalogButton"].SharedProps.Enabled = ActionsAllowed;
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
                case "optimizeCatalogButton":
                    OptimizeCatalog();
                    break;
                case "rebuildCatalogButton":
                    RebuildCatalog();
                    break;
                case "repopulateCatalogButton":
                    RepopulateCatalog();
                    break;
            }
        }

        #endregion

        #region grid

        private void catalogsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (catalogsGrid.Rows.Count > 0 && catalogsGrid.Selected.Rows.Count == 1
                && catalogsGrid.Selected.Rows[0].IsDataRow)
            {
                foreach (UltraGridRow row in catalogsGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedCatalog = new Pair<string, string>(
                            (string)row.Cells["Database"].Value, (string)row.Cells["Catalog"].Value);
                    }
                }
            }
            else
            {
                selectedCatalog = new Pair<string, string>(null, null);
            }
            selectionChangedCatalog = true;
            tablesGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            tablesGridStatusLabel.BringToFront();
            tablesGridStatusLabel.Visible = true;
            columnsGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            columnsGridStatusLabel.BringToFront();
            columnsGridStatusLabel.Visible = true;

            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        private void catalogsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = catalogsGrid;
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

        private void tablesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedTable = null;
            if (tablesGrid.Rows.Count > 0 && tablesGrid.Selected.Rows.Count == 1)
            {
                foreach (UltraGridRow row in tablesGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedTable = (long)row.Cells["TableId"].Value;
                    }
                }
            }
            selectionChangedTable = true;
            columnsGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            columnsGridStatusLabel.BringToFront();
            columnsGridStatusLabel.Visible = true;

            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        private void tablesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = tablesGrid;
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

        private void columnsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = columnsGrid;
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

        private void ServicesFullTextSearchView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

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
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.catalogsGrid);
            themeManager.updateGridTheme(this.tablesGrid);
            themeManager.updateGridTheme(this.columnsGrid);
        }
    }
}
