using System;
using System.Collections;
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
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using DatabaseStatus = Idera.SQLdm.Common.Snapshots.DatabaseStatus;
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    using Infragistics.Windows.Themes;
    using System.Text;

    internal partial class DatabasesBackupRestoreHistoryView : ServerBaseView, IDatabasesView, IShowFilterDialog
    {
        #region constants

        private const string AllDatabasesItemText = "< All Databases >";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        ///Ankit Nagpal --Sqldm10.0.0
        ///sysadmin privilege message
        private const string SYSADMIN_MESSAGE = @"Sysadmin privileges are required to obtain data for this view.";
        private const string SELECT_DB = @"Select one or more databases.";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;
        private string selectedDatabaseFilter = null;
        private bool initialized = false;
        private bool initializedBackupRestore = false;
        private DataTable currentDataTable;
        private DataTable backupRestoreDataTable;
        private DatabaseSummaryConfiguration configuration;
        private BackupRestoreHistoryConfiguration backupRestoreConfiguration;
        private DatabaseSummary currentSnapshot = null;
        SortedList<string, string> selectedDatabases = new SortedList<string, string>();
        private List<string> selectedDatabaseArgument = new List<string>();
        private bool selectionChanged = false;
        private static readonly object updateLock = new object();
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private UltraGrid selectedGrid = null;
        private bool databaseFilterInitialized = false;
        private bool databaseFilterSelectionChanging = false;
        
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
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastHistoryGridSettings = null;

        #endregion

        #region constructors

        public DatabasesBackupRestoreHistoryView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            databasesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            databasesGrid.CreationFilter = new ToolTipItemCreationFilter(instanceId);
            backupRestoreHistoryGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            configuration = new DatabaseSummaryConfiguration(instanceId);
            configuration.IncludeSystemDatabases = true;
            backupRestoreConfiguration = new BackupRestoreHistoryConfiguration(instanceId, null);

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

            //Backup Type
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(BackupType.Database, "Database");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.DatabaseDifferential, "Database Differential");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.File, "File");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.FileDifferential, "File Differential");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.Log, "Log");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.Partial, "Partial");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(BackupType.PartialDifferential, "Partial Differential");
            backupRestoreHistoryGrid.DisplayLayout.ValueLists["backupTypeValueList"].ValueListItems.Add(listItem);

            InitializeCurrentDataTable();
            InitializeBackupRestoreDataTable();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Add(null, "< "+ Idera.SQLdm.Common.Constants.LOADING+">");
            databasesGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            backupRestoreHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            backupRestoreHistoryGridStatusLabel.BringToFront();
            backupRestoreHistoryGridStatusLabel.Visible = true;
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            if(AutoScaleSizeHelper.isXXLargeSize)
                this.refreshDatabasesButton.Size = new System.Drawing.Size(27, 37);
            if (AutoScaleSizeHelper.isXLargeSize)
                this.refreshDatabasesButton.Size = new System.Drawing.Size(27, 37);

        }
        private void ScaleControlsAsPerResolution()
        {
            this.tableLayoutPanel1.Size = new System.Drawing.Size(689, 40);
            //this.refreshDatabasesButton.Size = new System.Drawing.Size(27, 35);
            this.databasesFilterComboBox.Size = new System.Drawing.Size(653, 35);
            this.splitContainer.Location = new System.Drawing.Point(0, 45);
        }

        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler DatabasesGridGroupByBoxVisibleChanged;
        public event EventHandler HistoryGridGroupByBoxVisibleChanged;

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
        public bool DatabasesGridGroupByBoxVisible
        {
            get { return !databasesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                databasesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (DatabasesGridGroupByBoxVisibleChanged != null)
                {
                    DatabasesGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Backup/Restore History Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool HistoryGridGroupByBoxVisible
        {
            get { return !backupRestoreHistoryGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                backupRestoreHistoryGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (HistoryGridGroupByBoxVisibleChanged != null)
                {
                    HistoryGridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public BackupRestoreHistoryConfiguration Configuration
        {
            get { return backupRestoreConfiguration; }
        }

        /// <summary>
        /// Get the current database configuration settings to manage state for current selections
        /// </summary>
        public DatabaseSummaryConfiguration DatabasesConfiguration
        {
            get { return configuration; }
        }

        #endregion

        #region methods


        public override void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesBackupsAndRestoresView);
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
            lastSplitterDistance = splitContainer.Height - Settings.Default.DatabasesBackupsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            if (Settings.Default.DatabasesBackupsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.DatabasesBackupsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, databasesGrid);
                // force a change so ribbon stays in sync
                DatabasesGridGroupByBoxVisible = DatabasesGridGroupByBoxVisible;
            }

            if (Settings.Default.DatabasesBackupsViewHistoryGrid is GridSettings)
            {
                lastHistoryGridSettings = Settings.Default.DatabasesBackupsViewHistoryGrid;
                GridSettings.ApplySettingsToGrid(lastHistoryGridSettings, backupRestoreHistoryGrid);
                // force a change so ribbon stays in sync
                HistoryGridGroupByBoxVisible = HistoryGridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(databasesGrid);
            GridSettings historyGridSettings = GridSettings.GetSettings(backupRestoreHistoryGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || !mainGridSettings.Equals(lastMainGridSettings)
                || !historyGridSettings.Equals(lastHistoryGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.DatabasesBackupsViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.DatabasesBackupsViewMainGrid = mainGridSettings;
                lastHistoryGridSettings =
                    Settings.Default.DatabasesBackupsViewHistoryGrid = historyGridSettings;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                DatabasesBackupRestoreHistoryView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                DatabasesBackupRestoreHistoryView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Pair<DatabaseSummary, BackupRestoreHistory> configs = new Pair<DatabaseSummary, BackupRestoreHistory>();

            if (!selectionChanged)
            {
                configs.First = managementService.GetDatabaseSummary(configuration);
            }

            List<string> configDbs = new List<string>();
            if (selectedDatabases.Count > 0)
            {
                foreach (string database in selectedDatabases.Values)
                {
                    configDbs.Add(database);
                }
            }
            backupRestoreConfiguration.DatabaseNames = configDbs;
            configs.Second = managementService.GetBackupRestoreHistory(backupRestoreConfiguration);

            return configs;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            databasesFilterComboBox.Enabled = false;
            databasesFilterComboBox.Items.Clear();
			///Ankit Nagpal --Sqldm10.0.0
			///If not a sysadmin display sysadmin message
            if (!isUserSysAdmin) {
                databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", SYSADMIN_MESSAGE));
                databasesGridStatusLabel.Text = SYSADMIN_MESSAGE;
                backupRestoreHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
            }
            else
            {
                databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                backupRestoreHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
            }
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Pair<DatabaseSummary, BackupRestoreHistory> snapshotPair;
            Exception e = null;

            if (data != null && data is Pair<DatabaseSummary, BackupRestoreHistory>)
            {
                snapshotPair = (Pair<DatabaseSummary, BackupRestoreHistory>)data;
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
                                        && selectedDatabaseFilter != null)
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

                                databasesGridStatusLabel.Visible = false;
                            }
                            else
                            {
                                currentDataTable.Clear();
                                databasesGridStatusLabel.Text = NO_ITEMS;
                                databasesGridStatusLabel.BringToFront();
                                databasesGridStatusLabel.Visible = true;
                                backupRestoreDataTable.Clear();
                                backupRestoreHistoryGridStatusLabel.Visible = true;
                                backupRestoreHistoryGridStatusLabel.BringToFront();
                                backupRestoreHistoryGridStatusLabel.Text = SELECT_DB;
                            }

                            databasesGrid.ResumeLayout();
                        }
                        else
                        {
                            e = snapshot.Error;
                            currentDataTable.Clear();
                            databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
							///Ankit Nagpal --Sqldm10.0.0
							///If not a sysadmin display sysadmin message
                            if (!isUserSysAdmin) databasesGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            databasesGridStatusLabel.BringToFront();
                            databasesGridStatusLabel.Visible = true;
                        }
                    }
                }

                if (snapshotPair.Second != null)
                {
                    lock (updateLock)
                    {
                        BackupRestoreHistory snapshot = snapshotPair.Second as BackupRestoreHistory;

                        if (snapshot.Error == null)
                        {
                            backupRestoreDataTable.BeginLoadData();

                            backupRestoreDataTable.Clear();

                            if (snapshot.BackupHistory.Rows.Count > 0
                                || snapshot.RestoreHistory.Rows.Count > 0)
                            {
                                foreach (DataRow backup in snapshot.BackupHistory.Rows)
                                {
                                    //sqldm-30244 start
                                    DateTime dateTime = DateTime.Now;

                                    if (!(backup["DateTime"] is System.DBNull))
                                    {
                                        dateTime = ((DateTime)backup["DateTime"]).ToLocalTime();
                                        IManagementService mgmt = DesktopClient.Helpers.ManagementServiceHelper.IManagementService;
                                        DateTime dummy = new DateTime(2019, 10, 9, 0, 0, 0);
                                        DateTime.SpecifyKind(dummy, DateTimeKind.Utc);
                                        DateTime local = dummy.ToLocalTime(),remote= mgmt.GetDateTime(instanceId,dummy);
                                        if (DateTime.Compare(remote ,local )<0)
                                        dateTime = dateTime.Subtract(local.Subtract(remote));
                                        else
                                        dateTime = dateTime.Add(remote.Subtract(local));
                                       

                                    }
                                    //sqldm-30244 end
                                    backupRestoreDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        backup["DatabaseName"],
                                        backup["DateTime"] is System.DBNull ? null : (object)(dateTime),
                                        "Backup",
                                        backup["Type"],
                                        backup["User"],
                                        ((FileSize)backup["Size"]).Megabytes,
                                        null,
                                        null,
                                        backup["Path"],
                                        backup["Filename"]
                                    }, true);
                                }

                                foreach (DataRow restore in snapshot.RestoreHistory.Rows)
                                {
                                    backupRestoreDataTable.LoadDataRow(
                                                new object[]
                                    {
                                        restore["DatabaseName"],
                                        restore["DateTime"] is System.DBNull ? null : (object)((DateTime)restore["DateTime"]).ToLocalTime(),
                                        "Restore",
                                        restore["Type"],
                                        restore["User"],
                                        null,
                                        restore["StopAtTime"] is System.DBNull ? null : (object)((DateTime)restore["StopAtTime"]).ToLocalTime(),
                                        restore["Replace"],
                                        restore["Path"],
                                        restore["Filename"]
                                    }, true);
                                }

                                backupRestoreDataTable.EndLoadData();

                                if (!initializedBackupRestore)
                                {
                                    if (lastMainGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, backupRestoreHistoryGrid);

                                        initializedBackupRestore = true;
                                    }
                                    else if (backupRestoreDataTable.Rows.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in backupRestoreHistoryGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, backupRestoreHistoryGrid.Width / 2);
                                        }

                                        initializedBackupRestore = true;
                                    }
                                }

                                backupRestoreHistoryGridStatusLabel.Visible = false;
                            }
                            else
                            {
                                backupRestoreDataTable.Clear();
                                backupRestoreHistoryGridStatusLabel.Text = NO_ITEMS;
                                backupRestoreHistoryGridStatusLabel.BringToFront();
                                backupRestoreHistoryGridStatusLabel.Visible = true;
                            }
                            // reset selectionChanged to enable full databasesummary refreshes again
                            selectionChanged = false;
                        }
                        else
                        {
                            e = snapshot.Error;
                            backupRestoreDataTable.Clear();
                            backupRestoreHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
							///Ankit Nagpal --Sqldm10.0.0
							///If not a sysadmin display sysadmin message                            
							if (!isUserSysAdmin) backupRestoreHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            backupRestoreHistoryGridStatusLabel.BringToFront();
                            backupRestoreHistoryGridStatusLabel.Visible = true;
                        }
                    }
                }
            }
            else
            {
                databasesFilterComboBox.Enabled = false;
                databasesFilterComboBox.Items.Clear();
                databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                currentDataTable.Clear();
                databasesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                databasesGridStatusLabel.BringToFront();
                databasesGridStatusLabel.Visible = true;
                backupRestoreDataTable.Clear();
                backupRestoreHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
                backupRestoreHistoryGridStatusLabel.BringToFront();
                backupRestoreHistoryGridStatusLabel.Visible = true;
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                if (!isUserSysAdmin) {

                    databasesGridStatusLabel.Text = backupRestoreHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                    databasesFilterComboBox.Items.Add(null, string.Format("< {0} >", SYSADMIN_MESSAGE));
                }
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

        public void ShowFilter()
        {
            BackupRestoreHistoryConfiguration selectFilter = new BackupRestoreHistoryConfiguration(instanceId, null);
            selectFilter.UpdateValues(backupRestoreConfiguration);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                backupRestoreConfiguration.UpdateValues(selectFilter);
                selectionChanged = true;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

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

        public void ToggleBackupsFilter()
        {
            backupRestoreConfiguration.ShowBackups = !backupRestoreConfiguration.ShowBackups;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleRestoresFilter()
        {
            backupRestoreConfiguration.ShowRestores = !backupRestoreConfiguration.ShowRestores;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleDatabasesGroupByBox()
        {
            DatabasesGridGroupByBoxVisible = !DatabasesGridGroupByBoxVisible;
        }

        public void ToggleHistoryGroupByBox()
        {
            HistoryGridGroupByBoxVisible = !HistoryGridGroupByBoxVisible;
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

        private void ShowStatus()
        {
            if (currentDataTable.Rows.Count == 0)
            {
                ApplicationController.Default.ClearCustomStatus();
            }
            else
            {
                if (backupRestoreDataTable.Rows.Count == 0)
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
                        String.Format("History: {0} Item{1}",
                                backupRestoreDataTable.Rows.Count,
                                backupRestoreDataTable.Rows.Count == 1 ? string.Empty : "s")
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

        private void ShowFilesView()
        {
            if (selectedDatabases.Count > 0)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesFiles, selectedDatabases.Values);
            }
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

            currentDataTable.DefaultView.Sort = "Database Name";

            databasesGrid.DataSource = currentDataTable;
        }

        private void InitializeBackupRestoreDataTable()
        {
            backupRestoreDataTable = new DataTable();
            backupRestoreDataTable.Columns.Add("Database Name", typeof(string));
            backupRestoreDataTable.Columns.Add("Date", typeof(DateTime));
            backupRestoreDataTable.Columns.Add("Operation", typeof(string));
            backupRestoreDataTable.Columns.Add("Type", typeof(int));
            backupRestoreDataTable.Columns.Add("User", typeof(string));
            backupRestoreDataTable.Columns.Add("Size (MB)", typeof(double));
            backupRestoreDataTable.Columns.Add("Point In Time", typeof(string));
            backupRestoreDataTable.Columns.Add("Replace", typeof(string));
            backupRestoreDataTable.Columns.Add("Path", typeof(string));
            backupRestoreDataTable.Columns.Add("Logical File Name", typeof(string));

            backupRestoreDataTable.DefaultView.Sort = "Date";

            backupRestoreHistoryGrid.DataSource = backupRestoreDataTable;
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
            if (grid == backupRestoreHistoryGrid)
            {
                HistoryGridGroupByBoxVisible = !HistoryGridGroupByBoxVisible;
            }
            else
            {
                DatabasesGridGroupByBoxVisible = !DatabasesGridGroupByBoxVisible;
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
            if (grid == backupRestoreHistoryGrid)
            {
                title = backupRestoreHistoryHeaderLabel.Text;
            }
            else
            {
                title = "databases";
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
            if (grid == backupRestoreHistoryGrid)
            {
                title = ExportHelper.GetValidFileName(backupRestoreHistoryHeaderLabel.Text);
            }
            else
            {
                title = "Databases";
            }
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

        #endregion

        #region events

        #region Splitter Focus

        private void splitContainer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion

        #region toolbars

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
                case "viewSummaryButton":
                    ShowSummaryView();
                    break;
                case "viewFilesButton":
                    ShowFilesView();
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
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
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

        #endregion

        #region grid

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

                backupRestoreDataTable.Clear();
                backupRestoreHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                backupRestoreHistoryGridStatusLabel.BringToFront();
                backupRestoreHistoryGridStatusLabel.Visible = true;

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

        private void backupRestoreHistoryGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = backupRestoreHistoryGrid;
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

        private void DatabasesBackupRestoreHistoryView_Load(object sender, EventArgs e)
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

        private void refreshDatabasesButton_Click(object sender, EventArgs e)
        {
            databaseFilterInitialized = false;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }
        void mouseEnter_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
                this.appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefreshHover;
        }
        void mouseLeave_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
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
            themeManager.updateGridTheme(this.databasesGrid);
            themeManager.updateGridTheme(this.backupRestoreHistoryGrid);
        }
    }
}
