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
using System.Globalization;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    internal partial class SessionsLocksView : ServerBaseView, IShowFilterDialog
    {
        #region constants

        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string FORMAT_CHARTTYPE = "Lock Statistics: {0}";
        private const string ROW_LIMITED =
            "The lock details rowcount limiter has been exceeded.  Collection of the lock list has been cancelled to reduce load on the monitored server.";

        #endregion

        #region fields
        private DateTime? historicalSnapshotDateTime = null;
        private bool initialized = false;
        private DataTable realTimeChartDataTable;
        private DataTable historicalChartDataTable;
        private bool chartRealTimeDataTablePrePopulated = false;
        private LockDetails currentSnapshot;
        private LockDetailsConfiguration configuration;
        private SessionSummaryConfiguration configurationSummary;
        private int? _selectedSpid = null;
        private int? selectedSpid
        {
            get { return _selectedSpid; }
            set
            {
                _selectedSpid = value;
                if (TraceAllowedChanged != null)
                {
                    TraceAllowedChanged(this, EventArgs.Empty);
                }
                if (KillAllowedChanged != null)
                {
                    KillAllowedChanged(this, EventArgs.Empty);
                }
            }
        }
        private string selectedSpidCommand = string.Empty;

        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private static readonly object updateLock = new object();

        private readonly Dictionary<System.Guid, UltraDataRow> rowLookupTable = new Dictionary<System.Guid, UltraDataRow>();

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartVisible = true;
        private string lastChartType = string.Empty;
        private Exception historyModeLoadError = null;
        private volatile List<Lock> activeSessionLockList = null;

        #endregion

        #region constructors

        public SessionsLocksView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(lockStatisticsChart, toolbarsManager);
            locksGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            InitializeChartDataTable();
            InitializeChart();
            AdaptFontSize();

            locksGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            lockStatisticsStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;

            // create the initial configuration
            configuration = new LockDetailsConfiguration(instanceId, null);
            configurationSummary = new SessionSummaryConfiguration(instanceId);

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }

        #endregion

        #region properties

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler FilterChanged;
        public event EventHandler ChartVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler TraceAllowedChanged;
        public event EventHandler KillAllowedChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                if (historicalSnapshotDateTime != value)
                {
                    historicalSnapshotDateTime = value;
                    currentHistoricalSnapshotDateTime = null;
                    selectedSpid = null;
                    locksGridDataSource.Rows.Clear();
                    rowLookupTable.Clear();

                    if (HistoricalSnapshotDateTimeChanged != null)
                    {
                        HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Get the availability of tracing a session determined by whether a session is currently selected
        /// </summary>
        public bool TraceAllowed
        {
            get { return selectedSpid.HasValue; }
        }

        /// <summary>
        /// Get the availability of killing a session determined by whether a user session is currently selected
        /// </summary>
        public bool KillAllowed
        {
            get
            {
                bool showKill = false;

                if (locksGrid.Rows.Count > 0 &&
                    locksGrid.Selected.Rows.Count == 1 && locksGrid.Selected.Rows[0].IsDataRow)
                {
                    UltraDataRow row = (UltraDataRow)locksGrid.Selected.Rows[0].ListObject;
                    showKill = !(bool)row["IsSystem"];
                }

                return selectedSpid.HasValue && showKill;
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
                RestoreLockStatisticsChart();

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
            get { return !locksGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                locksGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public LockDetailsConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsLocksView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is int)
            {
                configuration.SpidFilter = (int)argument;
                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.SessionsLocksViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastChartVisible =
                ChartVisible = Settings.Default.SessionsLocksViewChartVisible;

            if (Settings.Default.SessionsLocksViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.SessionsLocksViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, locksGrid);
                // force a change to GroupByBox so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            string chartType = Settings.Default.SessionsLocksViewChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in lockStatisticsOptionsButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureLockStatisticsChart(chartType);
                        lastChartType = chartType;
                        break;
                    }
                }
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(locksGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastChartVisible != ChartVisible
                || !gridSettings.Equals(lastMainGridSettings)
                || string.Format(FORMAT_CHARTTYPE, lastChartType) != lockStatisticsOptionsButton.Text)
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.SessionsLocksViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastChartVisible =
                    Settings.Default.SessionsLocksViewChartVisible = ChartVisible;
                lastMainGridSettings =
                    Settings.Default.SessionsLocksViewMainGrid = gridSettings;
                lastChartType =
                    Settings.Default.SessionsLocksViewChartType = lockStatisticsOptionsButton.Text.Substring(FORMAT_CHARTTYPE.Length - 3);
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
            if (HistoricalSnapshotDateTime == null)
            {
                return GetRealTimeSnapshot(previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes);
            }
            else
            {
                if (ViewMode == ServerViewMode.Historical)
                {
                    return GetHistoricalSnapshot(null, HistoricalSnapshotDateTime.Value, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
                else
                {
                    return GetHistoricalSnapshot(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                        ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value, ApplicationModel.Default.HistoryTimeValue.CustomTimeMinutes);
                }
            }
        }

        private LockDetails GetRealTimeSnapshot(bool visibleMinutesIncreased)
        {
            if (!chartRealTimeDataTablePrePopulated)
            {
                PopulateRealTimeSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
                chartRealTimeDataTablePrePopulated = true;
            }
            else
            {
                BackfillScheduledRefreshData();
                
                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (visibleMinutesIncreased)
                    ForwardFillHistoricalData();

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();
            }

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (currentSnapshot != null)
            {
                configuration.SetPreviousSnapshot(currentSnapshot);
            }

            return managementService.GetLockDetails(configuration);
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.realTimeChartDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PopulateRealTimeSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow["Date"];
            
            // SqlDM 10.2 (New History Browser) - If SD = 5mins and KD = 60min, then we need to reload entire data only if
            // we have no data for previous max(5,60) = 60 minutes. 
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(ServerSummaryHistoryData.MaximumKeepData))
            {
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                PopulateRealTimeSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            PopulateRealTimeSnapshots(null, DateTime.Now, (int)timeDiff.TotalMinutes, false);
        }



        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Replaces realtime data that is now stale with data from repository.
        /// </summary>
        private void BackFillScheduledHistoricalData()
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (realTimeChartDataTable != null &&
                    Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = ServerSummaryHistoryData.GetBackFillHistoricalRange(realTimeChartDataTable,
                        out startDateTime, out endDateTime);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",
                        startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        private void ForwardFillHistoricalData()
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (realTimeChartDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    ServerSummaryHistoryData.GetForwardFillHistoricalRange(realTimeChartDataTable,
                        out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        private void PopulateRealTimeSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes, bool clear)
        {
            var data = RepositoryHelper.GetLocksDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, 
                endDateTime, minutes, startDateTime);
            UpdateRealTimeData(data, clear);
        }

        private void UpdateRealTimeData(Pair<LockDetails, DataTable> data, bool clearData)
        {
            lock (updateLock)
            {
                MethodInvoker UICode = delegate
                {
                    realTimeChartDataTable.BeginLoadData();
                    if (clearData) realTimeChartDataTable.Clear();

                    if (data.Second != null)
                    {
                        int insertionIndex = -1;
                        if (data.Second.Rows.Count > 0 && !clearData)
                        {
                            insertionIndex = ServerSummaryHistoryData.GetInsertionIndex(realTimeChartDataTable, "Date",
                                (DateTime)data.Second.Rows[0]["UTCCollectionDateTime"]);
                        }
                        foreach (DataRow row in data.Second.Rows)
                        {
                            if (row["LockStatistics"] != DBNull.Value)
                            {
                                var newRow = realTimeChartDataTable.NewRow();
                                newRow["Date"] = (DateTime)row["UTCCollectionDateTime"];
                                newRow["IsHistorical"] = true;
                                AddLockStatistics(
                                    Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>(
                                        (byte[])row["LockStatistics"]), newRow);
                                if (insertionIndex >= 0)
                                    realTimeChartDataTable.Rows.InsertAt(newRow, insertionIndex++);
                                else
                                    realTimeChartDataTable.Rows.Add(newRow);
                            }
                        }
                    }

                    realTimeChartDataTable.EndLoadData();
                };

                this.UIThread(UICode);
            }
        }

        private Pair<LockDetails, DataTable> GetHistoricalSnapshot(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            return
                RepositoryHelper.GetLocksDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId,
                                                 endDateTime, minutes, startDateTime);
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                SessionsLocksView_Fill_Panel.Visible = false;
            }

            locksGridStatusLabel.Text = UNABLE_TO_UPDATE;
            lockStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
                       
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                {
                    UpdateDataWithRealTimeSnapshot(data as LockDetails);
                }
                else
                {
                    UpdateDataWithHistoricalSnapshot((Pair<LockDetails, DataTable>)data);
                }
            }
        }

        private void UpdateDataWithRealTimeSnapshot(LockDetails snapshot)
        {
            int? cloudProviderId=ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId;
            operationalStatusPanel.Visible = false;
            SessionsLocksView_Fill_Panel.Visible = true;

            if (snapshot != null)
            {
                if (snapshot.Error == null)
                {
                    lockStatisticsChart.DataSource = realTimeChartDataTable;
                    UpdateLocksGrid(snapshot);
                    realTimeChartDataTable.BeginLoadData();
                    DataRow newRow = realTimeChartDataTable.Rows.Add(new object[] {DateTime.Now});
                    AddLockStatistics(snapshot.LockCounters, newRow);
                    GroomHistoryData();
                    UpdateChartDataFilter();
                    realTimeChartDataTable.EndLoadData();
                    lockStatisticsChart.Visible = true;
                    currentSnapshot = snapshot;
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
                else
                {
                    selectedSpid = null;
                    locksGrid.Visible = false;
                    locksGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    lockStatisticsChart.Visible = false;
                    lockStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
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

        private void UpdateDataWithHistoricalSnapshot(Pair<LockDetails, DataTable> snapshot)
        {
            int? cloudProviderId = ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId;

            // SqlDM 10.2 (Anshul Aggarwal) New History Browser - DB Snapshot datetime should match the snapshot user selected in console.
            // If user selected custom range, then the latest snapshot in the range will be returned from DB and displayed to user.
            if (snapshot.First != null && (ViewMode == ServerViewMode.Custom ||
                (snapshot.First.TimeStamp != null && HistoricalSnapshotDateTime == snapshot.First.TimeStamp.Value.ToLocalTime())))
            {
                if (snapshot.First.Error == null)
                {
                    //SQLDM 10.1 (Barkha Khatri) SQLDM-25642 
                    //if not sys admin and not cloud instance then display sys admin message
                    //if (!isUserSysadmin && cloudProviderId != null )
                    //{
                        lockStatisticsChart.DataSource = historicalChartDataTable;
                        UpdateLocksGrid(snapshot.First);
                        historicalChartDataTable.BeginLoadData();
                        historicalChartDataTable.Clear();

                        if (snapshot.Second != null)
                        {
                            foreach (DataRow row in snapshot.Second.Rows)
                            {
                                if (row["LockStatistics"] != DBNull.Value)
                                {
                                    DataRow newRow = historicalChartDataTable.Rows.Add((DateTime)row["UTCCollectionDateTime"]);
                                    AddLockStatistics(
                                        Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>(
                                            (byte[])row["LockStatistics"]), newRow);
                                }
                            }
                        }

                        historicalChartDataTable.EndLoadData();
                        lockStatisticsChart.Visible = true;
                        SessionsLocksView_Fill_Panel.Visible = true;
                        ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                              Properties.Resources.StatusWarningSmall);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                        currentHistoricalStartDateTime = HistoricalStartDateTime;
                    //}
                }
                else
                {
                    locksGrid.Visible = false;
                    locksGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    lockStatisticsChart.Visible = false;
                    lockStatisticsStatusLabel.Text = UNABLE_TO_UPDATE;
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.First.Error));
                }
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                SessionsLocksView_Fill_Panel.Visible = false;
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateLocksGrid(LockDetails snapshot)
        {
            if (snapshot != null)
            {
                locksGridDataSource.SuspendBindingNotifications();

                // first remove any sessions from the previous data that no longer exist
                List<Guid> deleteList = new List<Guid>();
                foreach (Guid key in rowLookupTable.Keys)
                {
                    if (!snapshot.LockList.ContainsKey(key))
                    {
                        deleteList.Add(key);
                    }
                }
                foreach (Guid key in deleteList)
                {
                    UltraDataRow row;
                    rowLookupTable.TryGetValue(key, out row);
                    locksGridDataSource.Rows.Remove(row);
                    rowLookupTable.Remove(key);
                }

                if (snapshot.LockList.Count > 0)
                {
                    // Initialize active lock session list.
                    this.activeSessionLockList = new List<Lock>();

                    foreach (Lock sessionLock in snapshot.LockList.Values)
                    {
                        UltraDataRow existingRow;

                        // Populate session list.
                        activeSessionLockList.Add(sessionLock);

                        if (rowLookupTable.TryGetValue(sessionLock.Id, out existingRow))
                        {
                            //existingRow["Session ID"] = sessionLock.Spid;
                            existingRow["Object"] = sessionLock.FormattedObjectName;
                            existingRow["Database"] = sessionLock.Database;
                            existingRow["Object Schema"] = sessionLock.ObjectSchema;
                            existingRow["Object Name"] = sessionLock.ObjectName;
                            existingRow["Instances"] = sessionLock.InstanceCount;
                            existingRow["Request Mode"] = sessionLock.ModeLongString;
                            existingRow["Request Type"] = sessionLock.TypeString;
                            existingRow["Request Status"] = sessionLock.StatusString;
                            existingRow["Blocked"] = sessionLock.Blocked;
                            existingRow["Blocked By"] = sessionLock.Blocked_by;
                            existingRow["Blocking"] = sessionLock.Blocking;
                            existingRow["User"] = sessionLock.User;
                            existingRow["Host"] = sessionLock.Host;
                            existingRow["Application"] = sessionLock.Application;
                            existingRow["IsSystem"] = sessionLock.IsSystemProcess;
                            existingRow["Wait Time"] = sessionLock.WaitTime.TotalMilliseconds;
                            existingRow["Command"] = sessionLock.Command.Length > 15000
                                ? string.Concat(sessionLock.Command.Substring(0, 15000), "...")
                                : sessionLock.Command;
                        }
                        else
                        {
                            UltraDataRow newUltraRow = locksGridDataSource.Rows.Add(
                                new object[]
                                    {
                                        sessionLock.Spid,
                                        sessionLock.FormattedObjectName,
                                        sessionLock.Database,
                                        sessionLock.ObjectSchema,
                                        sessionLock.ObjectName,
                                        sessionLock.InstanceCount,
                                        sessionLock.ModeLongString,
                                        sessionLock.TypeString,
                                        sessionLock.StatusString,
                                        sessionLock.Blocked,
                                        sessionLock.Blocked_by,
                                        sessionLock.Blocking,
                                        sessionLock.User,
                                        sessionLock.Host,
                                        sessionLock.Application,
                                        sessionLock.IsSystemProcess,
                                        sessionLock.WaitTime.TotalMilliseconds,
                                        sessionLock.Command.Length > 15000
                                            ? string.Concat(sessionLock.Command.Substring(0,15000),"...")
                                            : sessionLock.Command
                                    });
                            rowLookupTable.Add(sessionLock.Id, newUltraRow);
                        }
                    }
                    locksGrid.Visible = true;
                    ApplicationController.Default.SetCustomStatus(
                        configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                        String.Format("{0} Lock{1}",
                                      snapshot.LockList.Count,
                                      snapshot.LockList.Count == 1 ? string.Empty : "s")
                        );
                }
                else
                {
                    if (snapshot.HasBeenRowLimited)
                    {
                        locksGridStatusLabel.Text = ROW_LIMITED;
                        locksGrid.Visible = false;
                        ApplicationController.Default.SetCustomStatus(
                            configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                            "0 Locks"
                            );
                    }
                    else
                    {
                        locksGridStatusLabel.Text = NO_ITEMS;
                        locksGrid.Visible = false;
                        ApplicationController.Default.SetCustomStatus(
                            configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                            "0 Locks"
                            );
                    }
                }

                locksGridDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, locksGrid);

                        initialized = true;
                    }
                    else if (snapshot.LockList.Count > 0)
                    {
                        foreach (UltraGridColumn column in locksGrid.DisplayLayout.Bands[0].Columns)
                        {
                            // do not change the object column
                            if (String.Compare(column.Key, "Object") != 0)
                            {
                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                column.Width = Math.Min(column.Width, locksGrid.Width / 2);
                            }
                        }

                        initialized = true;
                    }
                }

                if (locksGrid.Rows.Count > 0 && locksGrid.Selected.Rows.Count == 0)
                {
                    selectedSpid = null;
                }
            }
        }

        private static void AddLockStatistics(LockStatistics lockStatistics, DataRow newRow)
        {
            if (lockStatistics != null && newRow != null)
            {
                if (lockStatistics.AllocUnitCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - AllocUnit"] = lockStatistics.AllocUnitCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Application"] = lockStatistics.ApplicationCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Database"] = lockStatistics.DatabaseCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Extent"] = lockStatistics.ExtentCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - File"] = lockStatistics.FileCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - HoBT"] = lockStatistics.HeapCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Key"] = lockStatistics.KeyCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Latch"] = lockStatistics.LatchCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Metadata"] = lockStatistics.MetadataCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Object"] = lockStatistics.ObjectCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Page"] = lockStatistics.PageCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - RID"] = lockStatistics.RidCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.AverageWaitTime.HasValue)
                {
                    newRow["Average Wait Time - Table"] = lockStatistics.TableCounters.AverageWaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Average Wait Time - Table"] = DBNull.Value;
                }

                if (lockStatistics.AllocUnitCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - AllocUnit"] = lockStatistics.AllocUnitCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Application"] = lockStatistics.ApplicationCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Database"] = lockStatistics.DatabaseCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Extent"] = lockStatistics.ExtentCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - File"] = lockStatistics.FileCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - HoBT"] = lockStatistics.HeapCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Key"] = lockStatistics.KeyCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Latch"] = lockStatistics.LatchCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Metadata"] = lockStatistics.MetadataCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Object"] = lockStatistics.ObjectCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Page"] = lockStatistics.PageCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - RID"] = lockStatistics.RidCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.Deadlocks.HasValue)
                {
                    newRow["Deadlocks - Table"] = lockStatistics.TableCounters.Deadlocks.Value;
                }
                else
                {
                    newRow["Deadlocks - Table"] = DBNull.Value;
                }

                if (lockStatistics.AllocUnitCounters.Requests.HasValue)
                {
                    newRow["Requests - AllocUnit"] = lockStatistics.AllocUnitCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.Requests.HasValue)
                {
                    newRow["Requests - Application"] = lockStatistics.ApplicationCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.Requests.HasValue)
                {
                    newRow["Requests - Database"] = lockStatistics.DatabaseCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.Requests.HasValue)
                {
                    newRow["Requests - Extent"] = lockStatistics.ExtentCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.Requests.HasValue)
                {
                    newRow["Requests - File"] = lockStatistics.FileCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.Requests.HasValue)
                {
                    newRow["Requests - HoBT"] = lockStatistics.HeapCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.Requests.HasValue)
                {
                    newRow["Requests - Key"] = lockStatistics.KeyCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.Requests.HasValue)
                {
                    newRow["Requests - Latch"] = lockStatistics.LatchCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.Requests.HasValue)
                {
                    newRow["Requests - Metadata"] = lockStatistics.MetadataCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.Requests.HasValue)
                {
                    newRow["Requests - Object"] = lockStatistics.ObjectCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.Requests.HasValue)
                {
                    newRow["Requests - Page"] = lockStatistics.PageCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.Requests.HasValue)
                {
                    newRow["Requests - RID"] = lockStatistics.RidCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.Requests.HasValue)
                {
                    newRow["Requests - Table"] = lockStatistics.TableCounters.Requests.Value;
                }
                else
                {
                    newRow["Requests - Table"] = DBNull.Value;
                }

                if (lockStatistics.AllocUnitCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - AllocUnit"] = lockStatistics.AllocUnitCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Application"] = lockStatistics.ApplicationCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Database"] = lockStatistics.DatabaseCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Extent"] = lockStatistics.ExtentCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - File"] = lockStatistics.FileCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - HoBT"] = lockStatistics.HeapCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Key"] = lockStatistics.KeyCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Latch"] = lockStatistics.LatchCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Metadata"] = lockStatistics.MetadataCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Object"] = lockStatistics.ObjectCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Page"] = lockStatistics.PageCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - RID"] = lockStatistics.RidCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.Timeouts.HasValue)
                {
                    newRow["Timeouts - Table"] = lockStatistics.TableCounters.Timeouts.Value;
                }
                else
                {
                    newRow["Timeouts - Table"] = DBNull.Value;
                }

                if (lockStatistics.AllocUnitCounters.Waits.HasValue)
                {
                    newRow["Waits - AllocUnit"] = lockStatistics.AllocUnitCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.Waits.HasValue)
                {
                    newRow["Waits - Application"] = lockStatistics.ApplicationCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.Waits.HasValue)
                {
                    newRow["Waits - Database"] = lockStatistics.DatabaseCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.Waits.HasValue)
                {
                    newRow["Waits - Extent"] = lockStatistics.ExtentCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.Waits.HasValue)
                {
                    newRow["Waits - File"] = lockStatistics.FileCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.Waits.HasValue)
                {
                    newRow["Waits - HoBT"] = lockStatistics.HeapCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.Waits.HasValue)
                {
                    newRow["Waits - Key"] = lockStatistics.KeyCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.Waits.HasValue)
                {
                    newRow["Waits - Latch"] = lockStatistics.LatchCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.Waits.HasValue)
                {
                    newRow["Waits - Metadata"] = lockStatistics.MetadataCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.Waits.HasValue)
                {
                    newRow["Waits - Object"] = lockStatistics.ObjectCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.Waits.HasValue)
                {
                    newRow["Waits - Page"] = lockStatistics.PageCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.Waits.HasValue)
                {
                    newRow["Waits - RID"] = lockStatistics.RidCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.Waits.HasValue)
                {
                    newRow["Waits - Table"] = lockStatistics.TableCounters.Waits.Value;
                }
                else
                {
                    newRow["Waits - Table"] = DBNull.Value;
                }

                if (lockStatistics.AllocUnitCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - AllocUnit"] = lockStatistics.AllocUnitCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - AllocUnit"] = DBNull.Value;
                }

                if (lockStatistics.ApplicationCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Application"] = lockStatistics.ApplicationCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Application"] = DBNull.Value;
                }

                if (lockStatistics.DatabaseCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Database"] = lockStatistics.DatabaseCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Database"] = DBNull.Value;
                }

                if (lockStatistics.ExtentCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Extent"] = lockStatistics.ExtentCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Extent"] = DBNull.Value;
                }

                if (lockStatistics.FileCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - File"] = lockStatistics.FileCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - File"] = DBNull.Value;
                }

                if (lockStatistics.HeapCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - HoBT"] = lockStatistics.HeapCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - HoBT"] = DBNull.Value;
                }

                if (lockStatistics.KeyCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Key"] = lockStatistics.KeyCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Key"] = DBNull.Value;
                }

                if (lockStatistics.LatchCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Latch"] = lockStatistics.LatchCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Latch"] = DBNull.Value;
                }

                if (lockStatistics.MetadataCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Metadata"] = lockStatistics.MetadataCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Metadata"] = DBNull.Value;
                }

                if (lockStatistics.ObjectCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Object"] = lockStatistics.ObjectCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Object"] = DBNull.Value;
                }

                if (lockStatistics.PageCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Page"] = lockStatistics.PageCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Page"] = DBNull.Value;
                }

                if (lockStatistics.RidCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - RID"] = lockStatistics.RidCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - RID"] = DBNull.Value;
                }

                if (lockStatistics.TableCounters.WaitTime.HasValue)
                {
                    newRow["Wait Time - Table"] = lockStatistics.TableCounters.WaitTime.Value.TotalMilliseconds;
                }
                else
                {
                    newRow["Wait Time - Table"] = DBNull.Value;
                }
            }
        }

        public void ShowFilter()
        {
            LockDetailsConfiguration selectFilter = new LockDetailsConfiguration(instanceId, null);
            selectFilter.UpdateValues(configuration);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                configuration.UpdateValues(selectFilter);
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

        public void ToggleBlockedFilter()
        {
            configuration.FilterForBlocked = !configuration.FilterForBlocked;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleBlockingFilter()
        {
            configuration.FilterForBlocking = !configuration.FilterForBlocking;
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

        public void TraceSession()
        {
            if (selectedSpid.HasValue)
            {
                Dialogs.SessionTraceDialog dialog = new SessionTraceDialog(instanceId, selectedSpid.Value);
                dialog.Show();
            }
        }

        public void KillSession()
        {
            if (selectedSpid.HasValue)
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                                        String.Format("You are about to kill Session ID {0}. Do you want to continue?",
                                                        selectedSpid.Value.ToString()),
                                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    SetKillSessionContextData();

                    KillSessionConfiguration config = new KillSessionConfiguration(instanceId, selectedSpid.Value);
                    Snapshot snapshot = managementService.SendKillSession(config);

                    String actionMessage = String.Empty;

                    if (snapshot.Error == null)
                    {
                        actionMessage = "The Session has been terminated.";
                        ApplicationMessageBox.ShowMessage(actionMessage);
                        ApplicationController.Default.ActiveView.CancelRefresh();
                        ApplicationController.Default.RefreshActiveView();
                    }
                    else
                    {
                        actionMessage = String.Format("Unable to Kill Session ID {0}.",
                            selectedSpid.Value);
                        ApplicationMessageBox.ShowError(this, actionMessage, snapshot.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Prepares an AUditable Entity to send it through the Context for Logging.
        /// </summary>
        private void SetKillSessionContextData()
        {
            #region Change Log. Setting up context data

            String blockedSessionMachineName = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
            var subset = (from oneLockSession in activeSessionLockList
                          where oneLockSession.Spid.HasValue && oneLockSession.Spid.Value == selectedSpid.Value
                          select oneLockSession).ToList();
            Lock killedSession = subset.First();

            // Proceded for auditing the action.
            AuditableEntity entity = killedSession.GetAuditableEntity();
            entity.SqlUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser;
            entity.AddMetadataProperty("Action", "The Session has been terminated.");
            entity.AddMetadataProperty("Server Name", blockedSessionMachineName);

            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("KillSessionEntity", entity);

            #endregion Change Log. Setting up context data
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
            saveFileDialog.FileName = "SessionLocks";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(locksGrid, saveFileDialog.FileName);
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
                locksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                locksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                locksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                locksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    locksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    locksGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            locksGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            locksGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(locksGrid);
            dialog.Show(this);
        }

        #endregion

        #region chart

        private void InitializeChartDataTable()
        {
            realTimeChartDataTable = new DataTable();
            realTimeChartDataTable.Columns.Add("Date", typeof(DateTime));
          
            //
            // Requests
            //
            realTimeChartDataTable.Columns.Add("Requests - AllocUnit", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Application", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Database", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Extent", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - File", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - HoBT", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Key", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Latch", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Metadata", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Object", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Page", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - RID", typeof(long));
            realTimeChartDataTable.Columns.Add("Requests - Table", typeof(long));

            //
            // Average Wait Time
            //
            realTimeChartDataTable.Columns.Add("Average Wait Time - AllocUnit", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Application", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Database", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Extent", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - File", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - HoBT", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Key", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Latch", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Metadata", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Object", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Page", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - RID", typeof(double));
            realTimeChartDataTable.Columns.Add("Average Wait Time - Table", typeof(double));

            //
            // Deadlocks
            //
            realTimeChartDataTable.Columns.Add("Deadlocks - AllocUnit", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Application", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Database", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Extent", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - File", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - HoBT", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Key", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Latch", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Metadata", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Object", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Page", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - RID", typeof(long));
            realTimeChartDataTable.Columns.Add("Deadlocks - Table", typeof(long));

            //
            // Timeouts
            //
            realTimeChartDataTable.Columns.Add("Timeouts - AllocUnit", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Application", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Database", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Extent", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - File", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - HoBT", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Key", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Latch", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Metadata", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Object", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Page", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - RID", typeof(long));
            realTimeChartDataTable.Columns.Add("Timeouts - Table", typeof(long));

            //
            // Waits
            //
            realTimeChartDataTable.Columns.Add("Waits - AllocUnit", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Application", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Database", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Extent", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - File", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - HoBT", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Key", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Latch", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Metadata", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Object", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Page", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - RID", typeof(long));
            realTimeChartDataTable.Columns.Add("Waits - Table", typeof(long));

            //
            // Wait Time
            //
            realTimeChartDataTable.Columns.Add("Wait Time - AllocUnit", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Application", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Database", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Extent", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - File", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - HoBT", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Key", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Latch", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Metadata", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Object", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Page", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - RID", typeof(double));
            realTimeChartDataTable.Columns.Add("Wait Time - Table", typeof(double));
            realTimeChartDataTable.Columns.Add("IsHistorical", typeof(bool));

            historicalChartDataTable = realTimeChartDataTable.Clone();
        }

        private void InitializeChart()
        {
            lockStatisticsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            lockStatisticsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            lockStatisticsChart.LegendBox.Width = 100;
            lockStatisticsChart.LegendBox.PlotAreaOnly = false;
            lockStatisticsChart.Printer.Orientation = PageOrientation.Landscape;
            lockStatisticsChart.Printer.Compress = true;
            lockStatisticsChart.Printer.ForceColors = true;
            lockStatisticsChart.Printer.Document.DocumentName = "Sessions Locks Chart";
            lockStatisticsChart.ToolBar.RemoveAt(0);
            lockStatisticsChart.DataSource = realTimeChartDataTable;

            ConfigureLockStatisticsChart("Requests");
            InitalizeDrilldown(lockStatisticsChart);  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ConfigureLockStatisticsChart(string lockType)
        {
            lockStatisticsOptionsButton.Text = string.Format(FORMAT_CHARTTYPE, lockType);
            string yAxisTitle = GetYAxisTitle(lockType);
            lockStatisticsChart.AxisY.Title.Text = yAxisTitle;
            if (lockType.IndexOf("Wait Time") > -1)
            {
                lockStatisticsChart.ToolTipFormat = string.Format("%s \n%v ms\n%x", lockType);
            }
            else
            {
                lockStatisticsChart.ToolTipFormat = string.Format("%v %s {0}\n%x", lockType);
            }
            lockStatisticsChart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            FieldMap allocUnitFieldMap = new FieldMap(string.Format("{0} - AllocUnit", lockType), FieldUsage.Value);
            allocUnitFieldMap.DisplayName = "AllocUnit";
            FieldMap applicationFieldMap = new FieldMap(string.Format("{0} - Application", lockType), FieldUsage.Value);
            applicationFieldMap.DisplayName = "Application";
            FieldMap databaseFieldMap = new FieldMap(string.Format("{0} - Database", lockType), FieldUsage.Value);
            databaseFieldMap.DisplayName = "Database";
            FieldMap extentFieldMap = new FieldMap(string.Format("{0} - Extent", lockType), FieldUsage.Value);
            extentFieldMap.DisplayName = "Extent";
            FieldMap fileFieldMap = new FieldMap(string.Format("{0} - File", lockType), FieldUsage.Value);
            fileFieldMap.DisplayName = "File";
            FieldMap hobtFieldMap = new FieldMap(string.Format("{0} - HoBT", lockType), FieldUsage.Value);
            hobtFieldMap.DisplayName = "HoBT";
            FieldMap keyFieldMap = new FieldMap(string.Format("{0} - Key", lockType), FieldUsage.Value);
            keyFieldMap.DisplayName = "Key";
            FieldMap latchFieldMap = new FieldMap(string.Format("{0} - Latch", lockType), FieldUsage.Value);
            latchFieldMap.DisplayName = "Latch";
            FieldMap metadataFieldMap = new FieldMap(string.Format("{0} - Metadata", lockType), FieldUsage.Value);
            metadataFieldMap.DisplayName = "Metadata";
            FieldMap objectFieldMap = new FieldMap(string.Format("{0} - Object", lockType), FieldUsage.Value);
            objectFieldMap.DisplayName = "Object";
            FieldMap pageFieldMap = new FieldMap(string.Format("{0} - Page", lockType), FieldUsage.Value);
            pageFieldMap.DisplayName = "Page";
            FieldMap ridFieldMap = new FieldMap(string.Format("{0} - RID", lockType), FieldUsage.Value);
            ridFieldMap.DisplayName = "RID";
            FieldMap tableFieldMap = new FieldMap(string.Format("{0} - Table", lockType), FieldUsage.Value);
            tableFieldMap.DisplayName = "Table";

            lockStatisticsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        allocUnitFieldMap,
                        applicationFieldMap,
                        databaseFieldMap,
                        extentFieldMap,
                        fileFieldMap,
                        hobtFieldMap,
                        keyFieldMap,
                        latchFieldMap,
                        metadataFieldMap,
                        objectFieldMap,
                        pageFieldMap,
                        ridFieldMap,
                        tableFieldMap,
                        dateFieldMap});

            lockStatisticsChart.DataSourceSettings.ReloadData();
        }

        private string GetYAxisTitle(string lockType)
        {
            string res;

            switch (lockType)
            {
                case "Average Wait Time":
                case "Wait Time":
                    res = "ms";
                    break;

                case "Deadlocks":
                    res = "dead locks";
                    break;

                case "Requests":
                    res = "requests";
                    break;

                case "Timeouts":
                    res = "timeouts";
                    break;

                case "Waits":
                    res = "waits";
                    break;

                default:
                    res = string.Empty;
                    break;
            }

            return res;
        }
        private void ToggleChartToolbar(bool Visible)
        {
            lockStatisticsChart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, lockStatisticsChart, lockStatisticsOptionsButton.Text, ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, lockStatisticsChart, ExportHelper.GetValidFileName(lockStatisticsOptionsButton.Text));
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, lockStatisticsChart, lockStatisticsOptionsButton.Text, ExportHelper.GetValidFileName(lockStatisticsOptionsButton.Text));
        }

        private void MaximizeLockStatisticsChart()
        {
            splitContainer.Visible = false;
            splitContainer.Panel2.Controls.Remove(lockStatisticsPanel);
            maximizeLockStatisticsChartButton.Visible = false;
            restoreLockStatisticsChartButton.Visible = true;
            contentPanel.Controls.Add(lockStatisticsPanel);
        }

        private void RestoreLockStatisticsChart()
        {
            contentPanel.Controls.Remove(lockStatisticsPanel);
            maximizeLockStatisticsChartButton.Visible = true;
            restoreLockStatisticsChartButton.Visible = false;
            splitContainer.Panel2.Controls.Add(lockStatisticsPanel);
            splitContainer.Visible = true;
        }

        private void UpdateChartDataFilter()
        {
            if (realTimeChartDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                realTimeChartDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                realTimeChartDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        private void GroomHistoryData()
        {
            if (realTimeChartDataTable != null)
            {
                var selectFilter = ServerSummaryHistoryData.GetGroomingFilter("Date");
                DataRow[] groomedRows = realTimeChartDataTable.Select(selectFilter);
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeChartDataTable.AcceptChanges();
            }
        }

        #endregion

        private void ShowDetailsView()
        {
            if (selectedSpid.HasValue)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsDetails, selectedSpid.Value);
            }
        }

        private void ShowQueryHistoryView()
        {
            if (selectedSpid.HasValue)
            {
                if (selectedSpidCommand.Length > 0)
                {
                    string SqlHash = SqlParsingHelper.GetSignatureHash(selectedSpidCommand);
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.QueriesHistory, SqlHash);
                }
            }
        }

        //[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --added onLoad Event Handler
        private void SessionsDetailsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }
        //[END] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --added onLoad Event Handler

        #endregion

        #region events

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

        private void closeLockStatisticsChartButton_Click(object sender, EventArgs e)
        {
            ChartVisible = false;
            RestoreLockStatisticsChart();
        }

        private void maximizeLockStatisticsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeLockStatisticsChart();
        }

        private void restoreLockStatisticsChartButton_Click(object sender, EventArgs e)
        {
            RestoreLockStatisticsChart();
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
                case "viewDetailsButton":
                    ShowDetailsView();
                    break;
                case "traceSessionButton":
                    TraceSession();
                    break;
                case "killSessionButton":
                    KillSession();
                    break;
                case "viewQueryHistoryButton":
                    ShowQueryHistoryView();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(((StateButtonTool)e.Tool).Checked);
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
            switch (e.Tool.Key)
            {
                case "gridDataContextMenu":
                    if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify &&
                        HistoricalSnapshotDateTime == null)
                    {
                        ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].SharedProps.Enabled = KillAllowed;
                    }
                    else
                    {
                        ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                    }
                    break;
                case "chartContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(lockStatisticsChart.ToolBar.Visible);
                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = locksGrid.Rows.Count > 0 && locksGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(locksGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void locksGrid_MouseDown(object sender, MouseEventArgs e)
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

        private void locksGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (locksGrid.Selected.Rows.Count == 1 && locksGrid.Selected.Rows[0].IsDataRow)
            {
                selectedSpid = (int)locksGrid.Selected.Rows[0].Cells["Session ID"].Value;
                selectedSpidCommand = locksGrid.Selected.Rows[0].Cells["Command"].Text;
            }
            else
            {
                selectedSpid = null;
            }
        }

        private void averageWaitTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Average Wait Time");
        }

        private void deadlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Deadlocks");
        }

        private void requestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Requests");
        }

        private void timeoutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Timeouts");
        }

        private void waitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Waits");
        }

        private void waitTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Wait Time");
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

        #endregion
    }
}
