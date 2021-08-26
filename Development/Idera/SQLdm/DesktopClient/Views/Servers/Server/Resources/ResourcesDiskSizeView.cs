using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
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
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using System.Text;
using Idera.SQLdm.Common.Helpers;
using System.Globalization;
using System.Collections;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
	//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - New class for Disk Size View
    internal partial class ResourcesDiskSizeView : ServerBaseView
    {
        #region constants

        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        ///Ankit Nagpal --Sqldm10.0.0
        private const string SELECT_DRIVE = @"Select one or more drives.";
        private const string ROW_LIMITED =
            "The lock details rowcount limiter has been exceeded.  Collection of the lock list has been cancelled to reduce load on the monitored server.";
        //SQLdm 9.1 moved here the constant string from below
		private const string FORMAT_DISKS_CHARTTYPE = "Current Disk Usage: {0}";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime = null;
        private bool initialized = false;
        private DataTable realTimeChartDataTable;
        private DataTable realTimeGridDataTable;
        private DataTable historicalChartDataTable;
        private DataTable historicalGridDataTable;
        private bool chartRealTimeDataTablePrePopulated = false;
        private DiskSizeDetails currentSnapshot;
        private DiskSizeDetailsConfiguration configuration;

        SortedList<string, string> selectedDrives = new SortedList<string, string>();
        private List<string> _selectedDriveArguements = new List<string>();
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private static readonly object updateLock = new object();

        private readonly Dictionary<string, UltraDataRow> rowLookupTable = new Dictionary<string, UltraDataRow>();

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartVisible = true;
        private string lastChartType = string.Empty;
        private Exception historyModeLoadError = null;
        private volatile List<DiskDriveStatistics> activeDriveList = null;

        #endregion

        #region constructors

        public ResourcesDiskSizeView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(driveStatisticsChart, toolbarsManager);
            drivesGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            InitializeChartDataTable();
            InitializeChart();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            drivesGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            driveStatisticsStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;

            // create the initial configuration
            configuration = new DiskSizeDetailsConfiguration(instanceId, currentSnapshot);

            MonitoredSqlServer instance = RepositoryHelper.GetMonitoredSqlServer(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);
            configuration.WmiConfig = instance.WmiConfig;

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }

        #endregion

        #region properties

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler ChartVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;



        private DataTable RealTimeChartDataTable
        {
            get
            {
				//START SQLdm 9.1 (Ankit Srivastava) Resolved MountPoint issue in the disk size view
                realTimeChartDataTable.Clear();
                _selectedDriveArguements = _selectedDriveArguements ?? new List<string>();
                _selectedDriveArguements.Clear();
                var areGridColumnsSelected = drivesGrid != null && drivesGrid.Selected.Rows.Count > 0;
                if (areGridColumnsSelected)
                {
                    foreach (var dRow in drivesGrid.Selected.Rows)
                    {
                        if (dRow.Cells != null && dRow.Cells.Count > 0 && dRow.Cells["Disk Name"] != null)
                        {
                            foreach (DataRow row in realTimeGridDataTable.Rows)
                            {
                                if (String.Equals((string)dRow.Cells["Disk Name"].Value, row["Disk Name"].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    _selectedDriveArguements.Add(row["Disk Name"].ToString());
                                    var newRow = realTimeChartDataTable.NewRow();
                                    CopyRow(row, newRow);
                                    realTimeChartDataTable.Rows.Add(newRow);
                                    break;
                                }
                            }
                        }
                    }
                    initialized = true;
                }
                else
                {
                    foreach (DataRow row in realTimeGridDataTable.Rows)
                    {
                        var newRow = realTimeChartDataTable.NewRow();
                        CopyRow(row, newRow);
                        realTimeChartDataTable.Rows.Add(newRow);
                    }
                    initialized = true;
                }
				//END SQLdm 9.1 (Ankit Srivastava) Resolved MountPoint issue in the disk size view
                return realTimeChartDataTable;
            }
        }

        public DataTable HistoricalChartDataTable
        {
            get
            {
				//START SQLdm 9.1 (Ankit Srivastava) Resolved MountPoint issue in the disk size view
                historicalChartDataTable.Clear();
                _selectedDriveArguements = _selectedDriveArguements ?? new List<string>();
                _selectedDriveArguements.Clear();
                var areGridColumnsSelected = drivesGrid != null && drivesGrid.Selected.Rows.Count > 0;
                if (areGridColumnsSelected)
                {
                    foreach (var dRow in drivesGrid.Selected.Rows)
                    {
                        if (dRow.Cells != null && dRow.Cells.Count > 0 && dRow.Cells["Disk Name"] != null)
                        {
                            foreach (DataRow row in historicalGridDataTable.Rows)
                            {
                                if (String.Equals((string)dRow.Cells["Disk Name"].Value, row["Disk Name"].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    _selectedDriveArguements.Add(row["Disk Name"].ToString());
                                    var newRow = historicalChartDataTable.NewRow();
                                    CopyRow(row, newRow);
                                    historicalChartDataTable.Rows.Add(newRow);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in historicalGridDataTable.Rows)
                    {
                        var newRow = historicalChartDataTable.NewRow();
                        CopyRow(row, newRow);
                        historicalChartDataTable.Rows.Add(newRow);
                    }
                    initialized = true;
                }
                
				//END SQLdm 9.1 (Ankit Srivastava) Resolved MountPoint issue in the disk size view
                return historicalChartDataTable;
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                if (historicalSnapshotDateTime != value)
                {
                    historicalSnapshotDateTime = value;
                    currentHistoricalSnapshotDateTime = null;
                    _selectedDriveArguements = new List<string>();
                    drivesGridDataSource.Rows.Clear();
                    rowLookupTable.Clear();

                    if (HistoricalSnapshotDateTimeChanged != null)
                    {
                        HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                    }
                }
            }
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
            }
        }

        /// <summary>
        /// Get or Set the Sessions Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !drivesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                drivesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public DiskSizeDetailsConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesDiskSizeView);
        }

        public override void SetArgument(object argument)
        {
            _selectedDriveArguements = null;
            if (argument is IList<string>)
            {
                IList<string> selection = argument as IList<string>;
                _selectedDriveArguements = new List<string>(selection);

            }
            if (_selectedDriveArguements != null && _selectedDriveArguements.Count > 0)
            {
                selectedDrives = new SortedList<string, string>();
                drivesGrid.Selected.Rows.Clear();
                foreach (string drive in _selectedDriveArguements)
                {
                    foreach (UltraGridRow row in drivesGrid.Rows.GetAllNonGroupByRows())
                    {
                        if ((string)row.Cells["Disk Name"].Value == drive)
                        {
                            DataRowView dataRow = (DataRowView)row.ListObject;
                            selectedDrives.Add(drive, drive);
                            row.Selected = true;
                            drivesGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);

                            break;
                        }
                    }
                }
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.ResourcesDiskSizeViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastChartVisible =
                ChartVisible = Settings.Default.ResourcesDiskSizeViewChartVisible;

            if (Settings.Default.ResourcesDiskSizeViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ResourcesDiskSizeViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, drivesGrid);
                // force a change to GroupByBox so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }


            if (lastChartType != null && lastChartType.Length > 0)
            {
                ConfigureDiskDriveStatisticsChart(lastChartType);
            }
            else
            {
                string chartType = Settings.Default.ResourcesDiskSizeViewChartType;
                if (chartType != null && chartType.Length > 0)
                {
                    foreach (ToolStripItem item in driveStatisticsOptionsButton.DropDownItems)
                    {
                        if (item.Text == chartType)
                        {
                            ConfigureDiskDriveStatisticsChart(chartType);
                            lastChartType = chartType;
                            break;
                        }
                    }
                }
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(drivesGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastChartVisible != ChartVisible
                || !gridSettings.Equals(lastMainGridSettings)
                || string.Format(FORMAT_DISKS_CHARTTYPE, lastChartType) != driveStatisticsOptionsButton.Text)
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.ResourcesDiskSizeViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastChartVisible =
                    Settings.Default.ResourcesDiskSizeViewChartVisible = ChartVisible;
                lastMainGridSettings =
                    Settings.Default.ResourcesDiskSizeViewMainGrid = gridSettings;
                lastChartType = 
                    Settings.Default.ResourcesDiskSizeViewChartType = driveStatisticsOptionsButton.Text.Substring(FORMAT_DISKS_CHARTTYPE.Length - 3);
            }
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
               drivesGridStatusLabel.Text = driveStatisticsStatusLabel.Text =  Idera.SQLdm.Common.Constants.LOADING;
            }
        }

        public override object DoRefreshWork()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                return GetRealTimeSnapshot();
            }
            else
            {
                // SqlDM 10.2 (Anshul Aggarwal) - History Browser - Fetch data depending on custom/historical mode.
                if (ViewMode == ServerViewMode.Historical)
                {
                    return GetHistoricalSnapshot(null, HistoricalSnapshotDateTime.Value, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
                else
                {
                    return GetHistoricalSnapshot(HistoricalStartDateTime.Value,
                       HistoricalSnapshotDateTime.Value, MathHelper.GetCeilMinutes(HistoricalStartDateTime, HistoricalSnapshotDateTime));
                }
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                drivesView_Fill_Panel.Visible = false;
            }

            drivesGridStatusLabel.Text = UNABLE_TO_UPDATE;
            driveStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                {
                    UpdateDataWithRealTimeSnapshot(data as DiskSizeDetails);
                }
                else
                {
                    UpdateDataWithHistoricalSnapshot((DiskSizeDetails)data);
                }
            }

            if (drivesGrid != null && drivesGrid.Selected.Rows.Count >= 0)
            {
                if (_selectedDriveArguements == null || _selectedDriveArguements.Count == 0)
                {
                    drivesGrid.Selected.Rows.Clear();
                    if (_selectedDriveArguements.Count == 0)
                    {
                        foreach (var row in drivesGrid.Rows)
                        {
                            if(row.Cells!=null && row.Cells.Count > 0 && row.Cells["Disk Name"] !=null)
                            _selectedDriveArguements.Add(row.Cells["Disk Name"].ToString());
                        }
                    }
                    else
                    {
                        foreach (string drive in _selectedDriveArguements)
                        {
                            foreach (var row in drivesGrid.Rows)
                            {
                                if (row.Cells != null && row.Cells.Count > 0 && row.Cells["Disk Name"] != null)
                                {
                                    if (drive.Equals(row.Cells["Disk Name"].ToString(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        row.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    ConfigureDiskDriveStatisticsChart(lastChartType);
                }
            }

        }

        private DiskSizeDetails GetRealTimeSnapshot()
        {
            if (!chartRealTimeDataTablePrePopulated)
            {
                PrePopulateRealTimeGridDataTable();
                chartRealTimeDataTablePrePopulated = true;
            }
            else
                BackfillScheduledRefreshData();

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (currentSnapshot != null)
            {
                configuration.SetPreviousSnapshot(currentSnapshot);
            }

            return managementService.GetDiskSizeDetails(configuration.DatabaseSizeConfig,configuration.WmiConfig);
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.realTimeGridDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PrePopulateRealTimeGridDataTable();
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow["Date"];
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(ServerSummaryHistoryData.MaximumKeepData))
            {
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                PrePopulateRealTimeGridDataTable();
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            var data = RepositoryHelper.GetDiskSizeDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, 
                                                        instanceId, now, (int)timeDiff.TotalMinutes);

            UpdateRealTimeData(data, false);
        }

        private void PrePopulateRealTimeGridDataTable()
        {
            var data = RepositoryHelper.GetDiskSizeDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId,
                                                        DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

            UpdateRealTimeData(data, true);
        }

        private void UpdateRealTimeData(DiskSizeDetails data, bool clearData)
        {
            lock (updateLock)
            {
                MethodInvoker UICode = delegate
                {
                    realTimeGridDataTable.BeginLoadData();
                    if (clearData)
                    {
                        realTimeGridDataTable.Clear();
                    }

                    if (data != null)
                    {
                        foreach (var drive in data.DiskDrives.Values)
                        {

                                DataRow newRow = realTimeGridDataTable.Rows.Add((DateTime)drive.UTCCollectionDateTime);
                                AddDrivesStatistics(drive, newRow);
                                _selectedDriveArguements = _selectedDriveArguements ?? new List<string>();
                                _selectedDriveArguements.Add(drive.DriveName);
                                initialized = true;
                            
                        }
                    }

                    realTimeGridDataTable.EndLoadData();
                };

                this.UIThread(UICode);
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Fetches historical data for disk size based on start/end/minutes.
        /// </summary>
        private DiskSizeDetails GetHistoricalSnapshot(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            return
                RepositoryHelper.GetDiskSizeDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId,
                                                 endDateTime, minutes, startDateTime);
        }

        private void UpdateDataWithRealTimeSnapshot(DiskSizeDetails snapshot)
        {
            operationalStatusPanel.Visible = false;
            drivesView_Fill_Panel.Visible = true;

            if (snapshot != null)
            {
                currentSnapshot = snapshot;
                if (snapshot.Error == null)
                {
                    UpdateDrivesGrid(snapshot);

                    realTimeGridDataTable.BeginLoadData();

                    realTimeGridDataTable.Clear();

                    foreach (var drive in snapshot.DiskDrives.Values)
                    {

                        DataRow newRow = realTimeGridDataTable.Rows.Add(new object[] { DateTime.Now });
                        AddDrivesStatistics(drive, newRow);

                    }
                    GroomHistoryData();
                    UpdateChartDataFilter();
                    realTimeGridDataTable.EndLoadData();

                    DataTable dataSource = RealTimeChartDataTable;

                    if (dataSource.Rows.Count > 0)
                    {
                        driveStatisticsChart.DataSource = dataSource;
                        driveStatisticsChart.Visible = true;
                    }

                    else
                    {
                        driveStatisticsChart.Visible = false;
                        driveStatisticsStatusLabel.Text = NO_ITEMS;
                    }

                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
                else
                {
                    _selectedDriveArguements = new List<string> ();
                    drivesGrid.Visible = false;
                    drivesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    driveStatisticsChart.Visible = false;
                    driveStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                }
            }
            else
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                            new DesktopClientException("Snapshot data is null.")));
            }
        }

        private void UpdateDataWithHistoricalSnapshot(DiskSizeDetails snapshot)
        {
            if (snapshot != null)
            {
                currentSnapshot = snapshot;
                if (snapshot.DiskDrives != null)
                {
                    UpdateDrivesGrid(snapshot);

                    historicalGridDataTable.BeginLoadData();

                    historicalGridDataTable.Clear();

                    foreach (var drive in snapshot.DiskDrives.Values)
                    {

                            DataRow newRow = historicalGridDataTable.Rows.Add((DateTime)drive.UTCCollectionDateTime);
                            AddDrivesStatistics(drive, newRow);
                        
                    }

                    historicalGridDataTable.EndLoadData();
                    DataTable dataSource = HistoricalChartDataTable;

                    if (dataSource.Rows.Count > 0)
                    {
                        driveStatisticsChart.DataSource = dataSource;
                        driveStatisticsChart.Visible = true;
                        drivesView_Fill_Panel.Visible = true;
                    }

                    else
                    {
                        driveStatisticsChart.Visible = false;
                        driveStatisticsStatusLabel.Text = NO_ITEMS;
                    }

                    ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                          Properties.Resources.StatusWarningSmall);
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                }
                else
                {
                    drivesGrid.Visible = false;
                    drivesGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    driveStatisticsChart.Visible = false;
                    driveStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, null));
                }
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                drivesView_Fill_Panel.Visible = false;
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateDrivesGrid(DiskSizeDetails snapshot)
        {
            if (snapshot != null)
            {
                drivesGridDataSource.SuspendBindingNotifications();

                // first remove any sessions from the previous data that no longer exist
                List<string> deleteList = new List<string>();
                foreach (string key in rowLookupTable.Keys)
                {
                    if (!snapshot.DiskDrives.ContainsKey(key))
                    {
                        deleteList.Add(key);
                    }
                }
                foreach (string key in deleteList)
                {
                    UltraDataRow row;
                    rowLookupTable.TryGetValue(key, out row);
                    drivesGridDataSource.Rows.Remove(row);
                    rowLookupTable.Remove(key);
                }

                if (snapshot.DiskDrives.Count > 0)
                {
                    // Initialize active drive list.
                    this.activeDriveList = new List<DiskDriveStatistics>();
                    DataTable tempTable = historicalGridDataTable.Clone();
                    
                    foreach (DiskDriveStatistics drive in snapshot.DiskDrives.Values)
                    {
                        UltraDataRow existingRow;
                        
                        // Populate drive list.
                        activeDriveList.Add(drive);

                        if (drive.UTCCollectionDateTime == null) drive.UTCCollectionDateTime = DateTime.UtcNow;

                        DataRow newRow = tempTable.Rows.Add((DateTime)drive.UTCCollectionDateTime);
                        AddDrivesStatistics(drive, newRow);
                        if (rowLookupTable.TryGetValue(drive.DriveName, out existingRow))
                        {
                            existingRow["Disk Name"] = newRow["Disk Name"];
                            existingRow["Total Size (MB)"] = newRow["Total Size (MB)"];//SQLdm 9.1 (Ankit Srivastava) --Fixed DE44560 Assigning new column value
                            existingRow["Used (MB)"] = newRow["Used (MB)"] ;
                            existingRow["Free Disk (MB)"] = newRow["Free Disk (MB)"];
                            existingRow["SQL Data Used (MB)"] = newRow["SQL Data Used (MB)"];
                            existingRow["SQL Data Free (MB)"] = newRow["SQL Data Free (MB)"];
                            existingRow["SQL Log (MB)"] = newRow["SQL Log (MB)"];
                            existingRow["Other Files (MB)"] = newRow["Other Files (MB)"];
							//START SQLdm9.1 (Ankit Srivastava_ --Fixed Rally Defect DE44663 Passed the hidden columns values to the grid as well
                            existingRow["% Used"] = newRow["% Used"];
                            existingRow["% Free Disk"] = newRow["% Free Disk"];
                            existingRow["% SQL Data Used"] = newRow["% SQL Data Used"];
                            existingRow["% SQL Data Free"] = newRow["% SQL Data Free"];
                            existingRow["% SQL Log"] = newRow["% SQL Log"];
                            existingRow["% Other Files"] = newRow["% Other Files"];
							//END SQLdm9.1 (Ankit Srivastava_ --Fixed Rally Defect DE44663 Passed the hidden columns values to the grid as well
                        }
                        else
                        {
                            UltraDataRow newUltraRow = drivesGridDataSource.Rows.Add(
                                new object[]
                                    {
                                        newRow["Disk Name"],
                                        newRow["Total Size (MB)"],//SQLdm 9.1 (Ankit Srivastava) --Fixed DE44560 Assigning new column value
                                        newRow["Used (MB)"],
                                        newRow["Free Disk (MB)"], 
                                        newRow["SQL Data Used (MB)"],
                                        newRow["SQL Data Free (MB)"],
                                        newRow["SQL Log (MB)"] ,
                                        newRow["Other Files (MB)"],
										//START SQLdm9.1 (Ankit Srivastava_ --Fixed Rally Defect DE44663 Passed the hidden columns values to the grid as well
                                        newRow["% Used"],
                                        newRow["% Free Disk"] ,
                                        newRow["% SQL Data Used"],
                                        newRow["% SQL Data Free"],
                                        newRow["% SQL Log"],
                                        newRow["% Other Files"]
										//END SQLdm9.1 (Ankit Srivastava_ --Fixed Rally Defect DE44663 Passed the hidden columns values to the grid as well
                                    });
                            rowLookupTable.Add(drive.DriveName, newUltraRow);
                        }
                        

                    }
                    historicalGridDataTable.EndLoadData();
                    drivesGrid.Visible = true;
                      
                }
                else
                {
                    if (snapshot.HasBeenRowLimited)
                    {
                        drivesGridStatusLabel.Text = ROW_LIMITED;
                        drivesGrid.Visible = false;
                    }
                    else
                    {
                        drivesGridStatusLabel.Text = NO_ITEMS;
                        drivesGrid.Visible = false;
                    }
                }

                drivesGridDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, drivesGrid);

                        initialized = true;
                    }
                    else if (snapshot.DiskDrives.Count > 0)
                    {
                        foreach (UltraGridColumn column in drivesGrid.DisplayLayout.Bands[0].Columns)
                        {
                            // do not change the object column
                            if (String.Compare(column.Key, "Object") != 0)
                            {
                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                column.Width = Math.Min(column.Width, drivesGrid.Width / 2);
                            }
                        }

                        initialized = true;
                    }
                }

            }
        }

        private static void AddDrivesStatistics(DiskDriveStatistics driveStatistics, DataRow newRow)
        {
            if (driveStatistics != null && newRow != null)
            {
                newRow["Disk Name"] = driveStatistics.DriveName;
                if (driveStatistics.TotalSizeKB > 0 )
                {
					//SQLdm 9.1 (Ankit Srivastava) --Fixed DE44560 Formatting and assigning  new column value
                    newRow["Total Size (MB)"] = String.Format("{0:#,##0.##}", Convert.ToDouble(driveStatistics.TotalSizeKB / 1024));
                    newRow["Used (MB)"] = String.Format("{0:#,##0.##}", Convert.ToDouble((driveStatistics.TotalSizeKB - driveStatistics.UnusedSizeKB) / 1024));
                    newRow["% Used"] = Math.Round(Convert.ToDouble(((driveStatistics.TotalSizeKB - driveStatistics.UnusedSizeKB) / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["Total Size (MB)"] = 0;
                    newRow["Used (MB)"] = 0;
                    newRow["% Used"] = 0;
                }

                if (driveStatistics.UnusedSizeKB > 0)
                {
                    newRow["Free Disk (MB)"] = String.Format("{0:#,##0.##}", Convert.ToDouble(driveStatistics.UnusedSizeKB / 1024));
                    newRow["% Free Disk"] = Math.Round(Convert.ToDouble((driveStatistics.UnusedSizeKB / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["Free Disk (MB)"] = 0;
                    newRow["% Free Disk"] = 0;
                }

                if (driveStatistics.SQLDataUsedMB >0)
                {
                    newRow["SQL Data Used (MB)"] = String.Format("{0:#,##0.##}", Convert.ToDouble(driveStatistics.SQLDataUsedMB));
                    newRow["% SQL Data Used"] = Math.Round(Convert.ToDouble(((driveStatistics.SQLDataUsedMB * 1024) / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["SQL Data Used (MB)"] = 0;
                    newRow["% SQL Data Used"] = 0;
                }

                if (driveStatistics.SQLDataFreeMB > 0)
                {
                    newRow["SQL Data Free (MB)"] = String.Format("{0:#,##0.##}",Convert.ToDouble(driveStatistics.SQLDataFreeMB));
                    newRow["% SQL Data Free"] = Math.Round(Convert.ToDouble(((driveStatistics.SQLDataFreeMB * 1024) / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["SQL Data Free (MB)"] = 0;
                    newRow["% SQL Data Free"] = 0;

                }

                if (driveStatistics.SQLLogFileMB>0)
                {
                    newRow["SQL Log (MB)"] = String.Format("{0:#,##0.##}",Convert.ToDouble(driveStatistics.SQLLogFileMB));
                    newRow["% SQL Log"] = Math.Round(Convert.ToDouble(((driveStatistics.SQLLogFileMB * 1024) / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["SQL Log (MB)"] = 0;
                    newRow["% SQL Log"] = 0;
                }
				//SQLdm 9.1 (Ankit Srivastava) - Fixed DE44566 .. since the getter does the calcualtion avoiding multiple calls
                var otherUsage = driveStatistics.NonSQLDiskUsageMB;
                if (otherUsage > 0)
                {
                    newRow["Other Files (MB)"] = String.Format("{0:#,##0.##}", Convert.ToDouble(otherUsage));
                    newRow["% Other Files"] = Math.Round(Convert.ToDouble(((otherUsage * 1024) / driveStatistics.TotalSizeKB) * 100), 2);
                }
                else
                {
                    newRow["Other Files (MB)"] = 0;
                    newRow["% Other Files"] = 0;
                }  
         
            }
        }

        public void ShowFilter()
        {
            LockDetailsConfiguration selectFilter = new LockDetailsConfiguration(instanceId, null);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            
        }

        public void ToggleFilter()
        {
          
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

        #region grid

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - session locks as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "DiskDetails";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(drivesGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                drivesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                drivesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                drivesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                drivesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    drivesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    drivesGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            drivesGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            drivesGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(drivesGrid);
            dialog.Show(this);
        }


        //[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --added onLoad Event Handler
        private void ResourcesDiskSizeView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }
        //[END] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --added onLoad Event Handler
        #endregion

        #region chart

        private static DataRow CopyRow(DataRow row,DataRow newRow)
        {
            
            newRow["Date"] = row["Date"];
            newRow["Disk Name"] = row["Disk Name"];
            newRow["Total Size (MB)"] = row["Total Size (MB)"];//SQLdm 9.1 (Ankit Srivastava) --Fixed DE44560 adding new column value
            newRow["Used (MB)"] = row["Used (MB)"];
            newRow["Free Disk (MB)"] = row["Free Disk (MB)"];
            newRow["SQL Data Used (MB)"] = row["SQL Data Used (MB)"];
            newRow["SQL Data Free (MB)"] = row["SQL Data Free (MB)"];
            newRow["SQL Log (MB)"] = row["SQL Log (MB)"];
            newRow["Other Files (MB)"] = row["Other Files (MB)"];
            newRow["% Used"] = row["% Used"];
            newRow["% Free Disk"] = row["% Free Disk"];
            newRow["% SQL Data Used"] = row["% SQL Data Used"];
            newRow["% SQL Data Free"] = row["% SQL Data Free"];
            newRow["% SQL Log"] = row["% SQL Log"];
            newRow["% Other Files"] = row["% Other Files"];


            return newRow;
        }

        private void InitializeChartDataTable()
        {
            realTimeGridDataTable = new DataTable();
            realTimeGridDataTable.Columns.Add("Date", typeof(DateTime));
            realTimeGridDataTable.Columns.Add("Disk Name",typeof(string));
            realTimeGridDataTable.Columns.Add("Total Size (MB)", typeof(string));//SQLdm 9.1 (Ankit Srivastava) --Fixed DE44560 adding new column value
            realTimeGridDataTable.Columns.Add("Used (MB)",typeof(string));
            realTimeGridDataTable.Columns.Add("Free Disk (MB)",typeof(double)); //SQLDM-28108 --Changed data type from string to int.
            realTimeGridDataTable.Columns.Add("SQL Data Used (MB)", typeof(string));
            realTimeGridDataTable.Columns.Add("SQL Data Free (MB)", typeof(string));
            realTimeGridDataTable.Columns.Add("SQL Log (MB)", typeof(string));
            realTimeGridDataTable.Columns.Add("Other Files (MB)", typeof(string));
            realTimeGridDataTable.Columns.Add("% Used", typeof(double));
            realTimeGridDataTable.Columns.Add("% Free Disk", typeof(double));
            realTimeGridDataTable.Columns.Add("% SQL Data Used", typeof(double)); ;
            realTimeGridDataTable.Columns.Add("% SQL Data Free", typeof(double));
            realTimeGridDataTable.Columns.Add("% SQL Log", typeof(double));
            realTimeGridDataTable.Columns.Add("% Other Files", typeof(double));

            realTimeChartDataTable = realTimeGridDataTable.Clone();
            historicalGridDataTable = realTimeGridDataTable.Clone();
            historicalChartDataTable = realTimeGridDataTable.Clone();

        }

        private void InitializeChart()
        {
            driveStatisticsChart.Tag = driveStatisticsOptionsButton;
            driveStatisticsChart.Printer.Orientation = PageOrientation.Landscape;
            driveStatisticsChart.Printer.Compress = true;
            driveStatisticsChart.Printer.ForceColors = true;
            driveStatisticsChart.Printer.Document.DocumentName = "Disk Usage Chart";
            driveStatisticsChart.ToolBar.RemoveAt(0);
           
            driveStatisticsChart.DataSource = RealTimeChartDataTable;
            ConfigureDiskDriveStatisticsChart("Megabytes");
        }

        private void ConfigureDiskDriveStatisticsChart(string usageType)
        {
            lastChartType = usageType;

            driveStatisticsOptionsButton.Text = string.Format(FORMAT_DISKS_CHARTTYPE, usageType);

            driveStatisticsChart.SuspendLayout();
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                    driveStatisticsChart.LegendBox.Width = 175;
                if (AutoScaleSizeHelper.isXLargeSize)
                    driveStatisticsChart.LegendBox.Width = 210;
                if (AutoScaleSizeHelper.isXXLargeSize)
                    driveStatisticsChart.LegendBox.Width = 250;
            }
            else
                driveStatisticsChart.LegendBox.Width = 125;
            driveStatisticsChart.DataSourceSettings.Fields.Clear();

            FieldMap fileFieldMap = new FieldMap("Disk Name", FieldUsage.Label);
            fileFieldMap.DisplayName = "Drive";

            //FieldMap UsedFieldMap;
            FieldMap DataUsedFieldMap;
            FieldMap DataFreeFieldMap;
            FieldMap LogUsedFieldMap;
            FieldMap OtherUsedFieldMap;
            FieldMap FreeFieldMap;

            switch (usageType)
            {
                case "Megabytes":
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

                    driveStatisticsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            OtherUsedFieldMap,
                            FreeFieldMap});
                    driveStatisticsChart.AxisY.AutoScale = true;
                    driveStatisticsChart.AxisY.DataFormat.Decimals = 2;
                    driveStatisticsChart.ToolTipFormat = "%l\n%s\n%v MB";
                    break;
                case "Percent":
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


                    driveStatisticsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                            fileFieldMap,
                            DataUsedFieldMap,
                            DataFreeFieldMap,
                            LogUsedFieldMap,
                            OtherUsedFieldMap,
                            FreeFieldMap});
                    driveStatisticsChart.Series[1].Color = Color.FromArgb(199, 56, 0);
                    driveStatisticsChart.AxisY.Min = 0;
                    driveStatisticsChart.AxisY.Max = 100;
                    driveStatisticsChart.AxisY.DataFormat.Decimals = 0;
                    driveStatisticsChart.ToolTipFormat = "%l\n%s\n%v%%";
                    break;
            }

            driveStatisticsChart.DataSourceSettings.ReloadData();
            driveStatisticsChart.Invalidate();
            driveStatisticsChart.ResumeLayout();

            driveStatisticsChart.AxisX.MaxSizePercentage = 7; //SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defects DE44607
            this.driveStatisticsChart.AxisX.AutoScroll = true;//SQLdm 9.1 (Ankit Srivastava) - Fixed Rally Defects DE44609
            

        }

        private void ToggleChartToolbar(bool Visible)
        {
            driveStatisticsChart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, driveStatisticsChart, driveStatisticsOptionsButton.Text, ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, driveStatisticsChart, ExportHelper.GetValidFileName(driveStatisticsOptionsButton.Text));
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, driveStatisticsChart, driveStatisticsOptionsButton.Text, ExportHelper.GetValidFileName(driveStatisticsOptionsButton.Text));
        }

        private void MaximizeDiskDriveStatisticsChart()
        {
            splitContainer.Visible = false;
            splitContainer.Panel2.Controls.Remove(driveStatisticsPanel);
            maximizeDriveStatisticsChartButton.Visible = false;
            restoreDiskDriveStatisticsChartButton.Visible = true;
            contentPanel.Controls.Add(driveStatisticsPanel);
        }

        private void RestoreDiskDriveStatisticsChart()
        {
            contentPanel.Controls.Remove(driveStatisticsPanel);
            maximizeDriveStatisticsChartButton.Visible = true;
            restoreDiskDriveStatisticsChartButton.Visible = false;
            splitContainer.Panel2.Controls.Add(driveStatisticsPanel);
            splitContainer.Visible = true;
        }

        private void UpdateChartDataFilter()
        {
            if (RealTimeChartDataTable != null)
            {

                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                realTimeChartDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                realTimeChartDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
            
        }

        private void GroomHistoryData()
        {
            if (RealTimeChartDataTable != null)
            {
                DateTime groomThreshold =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));

                DataRow[] groomedRows = realTimeChartDataTable.Select(string.Format("Date < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture))); // SQLDM-19237, Tolga K

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeChartDataTable.AcceptChanges();
            }
        }

        #endregion


        #endregion

        #region events

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
            }
        }

        private void closeDiskDriveStatisticsChartButton_Click(object sender, EventArgs e)
        {
            ChartVisible = false;
            RestoreDiskDriveStatisticsChart();
        }

        private void maximizeDiskDriveStatisticsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeDiskDriveStatisticsChart();
        }

        private void restoreDiskDriveStatisticsChartButton_Click(object sender, EventArgs e)
        {
            RestoreDiskDriveStatisticsChart();
        }

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
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "printChartButton":
                    PrintChart();
                    break;
                case "exportChartDataButton":
                    SaveChartData();
                    break;
                case "exportChartImageButton":
                    SaveChartImage();
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
            //SQLdm 9.1 (Ankit Srivastva) removed unnecessaey tool option 
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = drivesGrid.Rows.Count > 0 && drivesGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(drivesGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void drivesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader =
                        contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
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

        private void drivesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {   
                if (_selectedDriveArguements != null)
                    _selectedDriveArguements.Clear();
                else
                    _selectedDriveArguements = new List<string>();

                if (drivesGrid != null && drivesGrid.Selected != null && drivesGrid.Selected.Rows != null && drivesGrid.Selected.Rows.Count >= 1)
                {
                    if (drivesGrid.Selected.Rows[0].IsDataRow)
                    {
                        foreach (var item in drivesGrid.Selected.Rows)
                        {
                            if (item.Cells != null && item.Cells.Count > 0 && item.Cells["Disk Name"] != null)
                                _selectedDriveArguements.Add((string)item.Cells["Disk Name"].Value);
                        }

                    }
                    else
                    {
                        foreach (var item in drivesGrid.Rows)
                        {
                            if (item.Cells != null && item.Cells.Count > 0 && item.Cells["Disk Name"] != null)
                                _selectedDriveArguements.Add((string)item.Cells["Disk Name"].Value);
                        }
                    }
                    if (historicalSnapshotDateTime != null && driveStatisticsChart != null && driveStatisticsChart.DataSource != null && HistoricalChartDataTable !=null)
                    {
                        driveStatisticsChart.DataSource = HistoricalChartDataTable;
                    }
                    ConfigureDiskDriveStatisticsChart(lastChartType);
                    ApplicationController.Default.ActiveView.CancelRefresh();
                    ApplicationController.Default.RefreshActiveView();

                }
            
            
        }

        private void megabytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureDiskDriveStatisticsChart("Megabytes");
        }

        private void percentageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureDiskDriveStatisticsChart("Percent");
        }

        #endregion

        #region Operational Status

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
            drivesGridStatusLabel.Text = driveStatisticsStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        #endregion

        #region Autoscale font size

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
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
            themeManager.updateGridTheme(this.drivesGrid);
        }

        #endregion
    }
}
