using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
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
using Idera.SQLdm.Common.Helpers;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    internal partial class SessionsDetailsView : ServerBaseView, IShowFilterDialog
    {
        #region constants

        private const string NO_ITEMS = @"There are no items to show in this view.";
		///Ankit Nagpal --Sqldm10.0.0
        private const string NO_ITEMS_SYSADMIN = @"No data is available for this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string SQL_CMD_NOT_AVAILABLE = @"<The SQL command is unavailable.>";
        private const string SQL_CMD_NOT_AVAILABLE_DBCC = @"<The SQL command is unavailable. This may be related to the use of the SQLDM inputbuffer limit option set in Server Properties.>";

        #endregion

        #region fields
        private bool filterChanged = false;
        private DateTime? historicalSnapshotDateTime = null;
        private bool initialized = false;
        private SessionSnapshot currentSnapshot = null;
        private SessionsConfiguration configuration;
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

        // get the version of the current instance from the snapshot or the app instance cache
        private decimal serverVersion
        {
            get
            {
                // If there is a snapshot
                if (currentSnapshot != null)
                {
                    //and a product version in the snapshot
                    if (currentSnapshot.ProductVersion != null)
                    {
                        //use the products product version
                        return currentSnapshot.ProductVersion.Major;
                    }
                }

                //if we got here then we need to make a plan to get the product version from elsewhere
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

        private bool selectedSpidIsSystem = false;
        private int? selectedSpidArgument = null;
        private string selectedSpidCommand = string.Empty;

        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private static readonly object updateLock = new object();

        private readonly Dictionary<Pair<int?, DateTime?>, UltraDataRow> rowLookupTable = new Dictionary<Pair<int?, DateTime?>, UltraDataRow>();

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastDetailPanelVisible = true;
        private Exception historyModeLoadError = null;
        private volatile List<Session> activeSessionList = null;

        #endregion

        #region constructors

        public SessionsDetailsView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();

            // Initialize position for the 'spaceUsedKbContainerPanel'.
            SetupNormalizedPositionForSpaceUsedKblabel();

            sessionsGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            // Setup Autoscale font size.
            AdaptFontSize();

            // load value lists for grid display

            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Clear();
            ValueListItem listItem = new ValueListItem(SessionStatus.Background, "Background");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusBackground;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(SessionStatus.Runnable, "Runnable");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusRunnable;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(SessionStatus.Running, "Running");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusRunning;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(SessionStatus.Sleeping, "Sleeping");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusSleeping;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(SessionStatus.Suspended, "Suspended");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusSuspended;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(SessionStatus.Other, "Other");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionStatusOther;
            sessionsGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);

            sessionsGrid.DisplayLayout.ValueLists["TypeValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(true, "System");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SystemSession;
            sessionsGrid.DisplayLayout.ValueLists["TypeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(false, "User");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.UserSession;
            sessionsGrid.DisplayLayout.ValueLists["TypeValueList"].ValueListItems.Add(listItem);

            // clear the design time values just in case it gets shown empty somehow
            boundStatusLabel.Image =
                boundSpidLabel.Image = null;
            boundSpidLabel.Text =
                boundUserLabel.Text =
                boundHostLabel.Text =
                boundDatabaseLabel.Text =
                boundStatusLabel.Text =
                boundOpenTransactionsLabel.Text =
                boundApplicationLabel.Text =
                boundExecutionContextLabel.Text =
                boundWaitTimeLabel.Text =
                boundWaitTypeLabel.Text =
                boundWaitResourceLabel.Text =
                boundCpuLabel.Text =
                boundPhysicalIoLabel.Text =
                boundMemoryLabel.Text =
                boundLoggedInSinceLabel.Text =
                boundLastActivityLabel.Text =
                boundBlockingLabel.Text =
                boundBlockedByLabel.Text =
                boundBlockingCountLabel.Text =
                boundNetLibraryLabel.Text =
                boundWorkstationNetAddressLabel.Text =
                boundLastCommandTextBox.Text = String.Empty;

            // create the initial configuration
            configuration = new SessionsConfiguration(instanceId, currentSnapshot);
            changeBackColor();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

        }

        #endregion

        #region properties

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler FilterChanged;
        public event EventHandler DetailsPanelVisibleChanged;
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
                    sessionsGridDataSource.Rows.Clear();
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
        /// Get the availability of killing a session determined by whether a session is currently selected
        /// </summary>
        public bool KillAllowed
        {
            get { return selectedSpid.HasValue && !selectedSpidIsSystem; }
        }

        /// <summary>
        /// Get or Set the Details Panel visibility and trigger state update event if changed
        /// </summary>
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

        /// <summary>
        /// Get or Set the Sessions Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !sessionsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                sessionsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public SessionsConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsDetailsView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is int)
            {
                selectedSpidArgument = (int)argument;
                foreach (UltraGridRow row in sessionsGrid.Rows.GetAllNonGroupByRows())
                {
                    if ((int)row.Cells["Session ID"].Value == (int)argument)
                    {
                        selectedSpidIsSystem = (bool)row.Cells["Type Image"].Value;
                        selectedSpid = (int)argument;
                        row.Selected = true;
                        sessionsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        selectedSpidArgument = null;
                        break;
                    }
                }
            }
            else if (argument is SessionsConfiguration)
            {
                configuration.UpdateValues((SessionsConfiguration)argument);
                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
                filterChanged = true;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.SessionsDetailsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastDetailPanelVisible =
                DetailsPanelVisible = Settings.Default.SessionsDetailsViewDetailsVisible;

            if (Settings.Default.SessionsDetailsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.SessionsDetailsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, sessionsGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(sessionsGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastDetailPanelVisible != DetailsPanelVisible
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.SessionsDetailsViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastDetailPanelVisible =
                    Settings.Default.SessionsDetailsViewDetailsVisible = DetailsPanelVisible;
                lastMainGridSettings =
                    Settings.Default.SessionsDetailsViewMainGrid = gridSettings;
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != null && (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime ||
                HistoricalStartDateTime != currentHistoricalStartDateTime || filterChanged))
            {
                filterChanged = false;
                historyModeLoadError = null;
                base.RefreshView();
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
                    return GetHistoricalSnapshot(null, HistoricalSnapshotDateTime.Value,
                        ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
                else
                {
                    return GetHistoricalSnapshot(HistoricalStartDateTime.Value,
                       HistoricalSnapshotDateTime.Value, MathHelper.GetCeilMinutes(HistoricalStartDateTime, HistoricalSnapshotDateTime));
                }
            }
        }

        private SessionSnapshot GetRealTimeSnapshot()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            configuration.SetPreviousValues(currentSnapshot);
            return managementService.GetSessions(configuration);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Fetches historical data for the selected start/end range.
        /// </summary>
        private SessionSnapshot GetHistoricalSnapshot(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            return
                RepositoryHelper.GetSessionsDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                    instanceId, endDateTime, startDateTime, minutes);
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                SessionsDetailsView_Fill_Panel.Visible = false;
            }

            sessionsGridStatusLabel.Text = UNABLE_TO_UPDATE;

            ///Ankit Nagpal --Sqldm10.0.0
            ///If not a sysadmin display sysadmin message

            //if (!isUserSysAdmin)
            //{
            //    sessionsGridStatusLabel.Text = NO_ITEMS_SYSADMIN;
            //}
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                {
                    UpdateDataWithRealTimeSnapshot(data as SessionSnapshot);
                }
                else
                {
                    UpdateDataWithHistoricalSnapshot(data as SessionSnapshot);
                }
            }
        }

        private void UpdateDataWithRealTimeSnapshot(SessionSnapshot snapshot)
        {
            int? cloudProviderId = ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId;
            operationalStatusPanel.Visible = false;
            SessionsDetailsView_Fill_Panel.Visible = true;

            if (snapshot != null)
            {
                if (snapshot.Error == null)
                {
                    //SQLDM 10.1 (Barkha Khatri) SQLDM-25642 
                    //if not sys admin and not cloud instance then display sys admin message
                    //if (!isUserSysAdmin && cloudProviderId==null)
                    //{
                    //    sessionsGridStatusLabel.Text = NO_ITEMS_SYSADMIN;
                    //}
                    //else
                    {
                        UpdateSessionsData(snapshot);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                }
                else
                {
                    sessionsGrid.Visible = false;
                    sessionsGridStatusLabel.Text = UNABLE_TO_UPDATE;

                    ///Ankit Nagpal --Sqldm10.0.0
                    ///If not a sysadmin display sysadmin message

                    //if (!isUserSysAdmin)
                    //{
                    //    sessionsGridStatusLabel.Text = NO_ITEMS_SYSADMIN;
                    //}
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

        private void UpdateDataWithHistoricalSnapshot(SessionSnapshot snapshot)
        {
            int? cloudProviderId = ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId;

            // SqlDM 10.2 (Anshul Aggarwal) New History Browser - DB Snapshot datetime should match the snapshot user selected in console.
            // If user selected custom range, then the latest snapshot in the range will be returned from DB and displayed to user.
            if (snapshot != null && (ViewMode == ServerViewMode.Custom ||
                (snapshot.TimeStamp != null && snapshot.TimeStamp.Value.ToLocalTime() == HistoricalSnapshotDateTime)))
            {
                if (snapshot.Error == null)
                {
                    //SQLDM 10.1 (Barkha Khatri) SQLDM-25642 
                    //if not sys admin and not cloud instance then display sys admin message
                    //if (!isUserSysAdmin && cloudProviderId == null)
                    //{
                    //    sessionsGridStatusLabel.Text = NO_ITEMS_SYSADMIN;
                    //}
                    //else
                    {
                        UpdateSessionsData(snapshot);
                        SessionsDetailsView_Fill_Panel.Visible = true;
                        ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                              Properties.Resources.StatusWarningSmall);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                        currentHistoricalStartDateTime = HistoricalStartDateTime;
                    }
                }
                else
                {

                    sessionsGrid.Visible = false;
                    sessionsGridStatusLabel.Text = UNABLE_TO_UPDATE;

                    ///Ankit Nagpal --Sqldm10.0.0
                    ///If not a sysadmin display sysadmin message

                    //if (!isUserSysAdmin)
                    //{
                    //    sessionsGridStatusLabel.Text = NO_ITEMS_SYSADMIN;
                    //}
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                }
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                SessionsDetailsView_Fill_Panel.Visible = false;
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateSessionsData(SessionSnapshot snapshot)
        {
            if (snapshot != null)
            {
                // first remove any sessions from the previous data that no longer exist
                sessionsGridDataSource.SuspendBindingNotifications();
                List<Pair<int?, DateTime?>> deleteList = new List<Pair<int?, DateTime?>>();
                foreach (Pair<int?, DateTime?> key in rowLookupTable.Keys)
                {
                    if (!snapshot.SessionList.ContainsKey(key))
                    {
                        deleteList.Add(key);
                    }
                }
                foreach (Pair<int?, DateTime?> key in deleteList)
                {
                    UltraDataRow row;
                    rowLookupTable.TryGetValue(key, out row);
                    sessionsGridDataSource.Rows.Remove(row);
                    rowLookupTable.Remove(key);
                }

                // Update Sessions Grid
                if (snapshot.SessionList.Count > 0)
                {
                    string sqlCmdNotAvailable = SQL_CMD_NOT_AVAILABLE;

                    if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.InputBufferLimited )
                    {
                        if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.InputBufferLimiter <= snapshot.SessionList.Count)
                        {
                            sqlCmdNotAvailable = SQL_CMD_NOT_AVAILABLE_DBCC;
                        }
                    }

                    // Initialize and select the new active sessions list.
                    this.activeSessionList = new List<Session>();

                    //now update any matching sessions or add new ones
                    foreach (Session session in snapshot.SessionList.Values)
                    {
                        UltraDataRow existingRow;

                        // Populate active session list.
                        activeSessionList.Add(session);

                        decimal sessionUserSpaceAllocated = session.SessionUserAllocatedTotal != null ? (session.SessionUserAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionUserSpaceDeallocated = session.SessionUserDeallocatedTotal != null ? (session.SessionUserDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionSpaceUsed = sessionUserSpaceAllocated >= sessionUserSpaceDeallocated ? sessionUserSpaceAllocated - sessionUserSpaceDeallocated : 0;
                        decimal taskUserSpaceAllocated = session.TaskUserAllocatedTotal != null ? (session.TaskUserAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskUserSpaceDeallocated = session.TaskUserDeallocatedTotal != null ? (session.TaskUserDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskSpaceUsed = taskUserSpaceAllocated >= taskUserSpaceDeallocated ? taskUserSpaceAllocated - taskUserSpaceDeallocated : 0;
                        decimal sessionInternalSpaceAllocated = session.SessionInternalAllocatedTotal != null ? (session.SessionInternalAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionInternalSpaceDeallocated = session.SessionInternalDeallocatedTotal != null ? (session.SessionInternalDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionInternalSpaceUsed = sessionInternalSpaceAllocated >= sessionInternalSpaceDeallocated ? sessionInternalSpaceAllocated - sessionInternalSpaceDeallocated : 0;
                        decimal taskInternalSpaceAllocated = session.TaskInternalAllocatedTotal != null ? (session.TaskInternalAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskInternalSpaceDeallocated = session.TaskInternalDeallocatedTotal != null ? (session.TaskInternalDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskInternalSpaceUsed = taskInternalSpaceAllocated >= taskInternalSpaceDeallocated ? taskInternalSpaceAllocated - taskInternalSpaceDeallocated : 0;

                        if (rowLookupTable.TryGetValue(session.InternalSessionIdentifier, out existingRow))
                        {
                            existingRow["Status Image"] = session.Status;
                            //existingRow["Session ID"] = session.Spid;
                            existingRow["Type Image"] = session.IsSystemProcess;
                            existingRow["Type"] = session.IsSystemProcess ? "System" : "User";
                            existingRow["User"] = session.UserName;
                            //existingRow["Host"] = session.Workstation;
                            existingRow["Database"] = session.Database;
                            existingRow["Status"] = session.Status;
                            existingRow["Open Transactions"] = session.OpenTransactions;
                            existingRow["Command"] = session.Command;
                            existingRow["Application"] = session.Application;
                            existingRow["Execution Context"] = session.ExecutionContext;
                            existingRow["Wait Time"] = session.WaitTime.TotalMilliseconds;
                            existingRow["Wait Type"] = session.WaitType;
                            existingRow["Wait Resource"] = session.WaitResource;
                            existingRow["CPU"] = session.Cpu.TotalMilliseconds;
                            existingRow["CPU Delta"] = session.CpuDelta.TotalMilliseconds;
                            existingRow["Physical I/O"] = session.PhysicalIo;
                            existingRow["Physical I/O Delta"] = session.PhysicalIoDelta;
                            existingRow["Memory Usage"] = session.Memory.Kilobytes;
                            //existingRow["Login Time"] = session.LoggedInSince.HasValue ? (object)((DateTime)session.LoggedInSince).ToLocalTime() : null;
                            existingRow["Last Batch"] = session.LastActivity.HasValue ? (object)((DateTime)session.LastActivity).ToLocalTime() : null;
                            existingRow["Blocked By"] = session.BlockedBy;
                            existingRow["Blocking"] = session.Blocking;
                            existingRow["Blocking Count"] = session.BlockingCount;
                            //existingRow["Net Library"] = session.NetLibrary;
                            //existingRow["Net Address"] = session.WorkstationNetAddress;
                            existingRow["Version Store Elapsed Seconds"] = session.VersionStoreElapsedTime.HasValue
                                                                               ? session.VersionStoreElapsedTime.
                                                                                     Value.
                                                                                     TotalSeconds
                                                                               : 0;
                            existingRow["Transaction Isolation Level"] =
                                ApplicationHelper.GetEnumDescription(session.TransactionIsolationLevel);


                            existingRow["Current Session User Space (KB)"] = sessionSpaceUsed;
                            existingRow["Session User Space Allocated (KB)"] = sessionUserSpaceAllocated;
                            existingRow["Session User Space Deallocated (KB)"] = sessionUserSpaceDeallocated;


                            existingRow["Current Task User Space (KB)"] = taskSpaceUsed;
                            existingRow["Task User Space Allocated (KB)"] = taskUserSpaceAllocated;
                            existingRow["Task User Space Deallocated (KB)"] = taskUserSpaceDeallocated;


                            existingRow["Current Session Internal Space (KB)"] = sessionInternalSpaceUsed;
                            existingRow["Session Internal Space Allocated (KB)"] = sessionInternalSpaceAllocated;
                            existingRow["Session Internal Space Deallocated (KB)"] = sessionInternalSpaceDeallocated;

                            existingRow["Current Task Internal Space (KB)"] = taskInternalSpaceUsed;
                            existingRow["Task Internal Space Allocated (KB)"] = taskInternalSpaceAllocated;
                            existingRow["Task Internal Space Deallocated (KB)"] = taskInternalSpaceDeallocated;

                            existingRow["Last Command"] = session.LastCommand != null ? session.LastCommand : sqlCmdNotAvailable;
                        }
                        else
                        {
                            UltraDataRow newRow = sessionsGridDataSource.Rows.Add(
                                new object[]
                                    {
                                        session.Status,
                                        session.Spid,
                                        session.IsSystemProcess,
                                        session.IsSystemProcess ? "System" : "User",
                                        session.UserName,
                                        session.Workstation,
                                        session.Database,
                                        session.Status,
                                        session.OpenTransactions,
                                        session.Command,
                                        session.Application,
                                        session.ExecutionContext,
                                        session.WaitTime.TotalMilliseconds,
                                        session.WaitType,
                                        session.WaitResource,
                                        session.Cpu.TotalMilliseconds,
                                        session.CpuDelta.TotalMilliseconds,
                                        session.PhysicalIo,
                                        session.PhysicalIoDelta,
                                        session.Memory.Kilobytes,
                                        session.LoggedInSince.HasValue ? (object)((DateTime)session.LoggedInSince).ToLocalTime() : null,
                                        session.LastActivity.HasValue ? (object)((DateTime)session.LastActivity).ToLocalTime() : null,
                                        session.BlockedBy,
                                        session.Blocking,
                                        session.BlockingCount,
                                        session.NetLibrary,
                                        session.WorkstationNetAddress,
                                        session.VersionStoreElapsedTime.HasValue
                                            ? session.VersionStoreElapsedTime.Value.TotalSeconds
                                            : 0,
                                        ApplicationHelper.GetEnumDescription(session.TransactionIsolationLevel),
                                        sessionSpaceUsed,
                                        sessionUserSpaceAllocated,
                                        sessionUserSpaceDeallocated,
                                        taskSpaceUsed,
                                        taskUserSpaceAllocated,
                                        taskUserSpaceDeallocated,
                                        sessionInternalSpaceUsed,
                                        sessionInternalSpaceAllocated,
                                        sessionInternalSpaceDeallocated,
                                        taskInternalSpaceUsed,
                                        taskInternalSpaceAllocated,
                                        taskInternalSpaceDeallocated,
                                        session.LastCommand != null ? session.LastCommand : sqlCmdNotAvailable
                                    });

                            rowLookupTable.Add(session.InternalSessionIdentifier, newRow);
                        }
                    }

                    foreach (UltraGridRow row in sessionsGrid.Rows.GetAllNonGroupByRows())
                    {
                        row.RefreshSortPosition();
                    }
                    sessionsGrid.DisplayLayout.RefreshFilters();
                    sessionsGrid.Visible = true;

                    // this must be done before referencing the counts in the grid for the status update
                    sessionsGridDataSource.ResumeBindingNotifications();

                    ApplicationController.Default.SetCustomStatus(
                        configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                        String.Format("{0} Session{1}",
                                sessionsGrid.Rows.FilteredInNonGroupByRowCount,
                                sessionsGrid.Rows.FilteredInNonGroupByRowCount == 1 ? string.Empty : "s")
                        );
                }
                else
                {
                    sessionsGridStatusLabel.Text = NO_ITEMS;
                    sessionsGrid.Visible = false;
                    ApplicationController.Default.SetCustomStatus(
                        configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                        "0 Sessions"
                        );
                }

                sessionsGridDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, sessionsGrid);

                        initialized = true;
                    }
                    else if (snapshot.SessionList.Count > 0)
                    {
                        foreach (UltraGridColumn column in sessionsGrid.DisplayLayout.Bands[0].Columns)
                        {
                            if (column.Key != "Status Image")
                            {
                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                            }
                        }

                        initialized = true;
                    }

                    if (FilterChanged != null)
                    {
                        FilterChanged(this, new EventArgs());
                    }
                }

                currentSnapshot = snapshot;

                // Hide 2005 only columns if necessary, but after currentSnapshot has been set
                UltraGridBand band = sessionsGrid.DisplayLayout.Bands[0];
                if (serverVersion == 8)
                {
                    band.Columns["Version Store Elapsed Seconds"].Hidden = true;
                    band.Columns["Version Store Elapsed Seconds"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Transaction Isolation Level"].Hidden = true;
                    band.Columns["Transaction Isolation Level"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Current Session User Space (KB)"].Hidden = true;
                    band.Columns["Current Session User Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Session User Space Allocated (KB)"].Hidden = true;
                    band.Columns["Session User Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Session User Space Deallocated (KB)"].Hidden = true;
                    band.Columns["Session User Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Current Task User Space (KB)"].Hidden = true;
                    band.Columns["Current Task User Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Task User Space Allocated (KB)"].Hidden = true;
                    band.Columns["Task User Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Task User Space Deallocated (KB)"].Hidden = true;
                    band.Columns["Task User Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Current Session Internal Space (KB)"].Hidden = true;
                    band.Columns["Current Session Internal Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Session Internal Space Allocated (KB)"].Hidden = true;
                    band.Columns["Session Internal Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Session Internal Space Deallocated (KB)"].Hidden = true;
                    band.Columns["Session Internal Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Current Task Internal Space (KB)"].Hidden = true;
                    band.Columns["Current Task Internal Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Task Internal Space Allocated (KB)"].Hidden = true;
                    band.Columns["Task Internal Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                    band.Columns["Task Internal Space Deallocated (KB)"].Hidden = true;
                    band.Columns["Task Internal Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }
                else
                {
                    band.Columns["Version Store Elapsed Seconds"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Transaction Isolation Level"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Current Session User Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Session User Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Session User Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Current Task User Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Task User Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Task User Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Current Session Internal Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Session Internal Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Session Internal Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Current Task Internal Space (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Task Internal Space Allocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                    band.Columns["Task Internal Space Deallocated (KB)"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.Default;
                }


                UpdateDetailsPanel(selectedSpid);

                //make sure the selection is still correct
                if (sessionsGrid.Visible == false || sessionsGrid.Rows == null || sessionsGrid.Rows.Count == 0)
                {
                    selectedSpid = null;
                }
                else
                {
                    // this probably should never happen, but fix it just in case
                    if (sessionsGrid.Selected.Rows.Count == 1 && sessionsGrid.Selected.Rows[0].IsDataRow)
                    {
                        int spid = (int)sessionsGrid.Selected.Rows[0].Cells["Session ID"].Value;
                        if (selectedSpid != spid)
                        {
                            selectedSpidIsSystem = (bool)sessionsGrid.Selected.Rows[0].Cells["Type Image"].Value;
                            selectedSpid = spid;
                        }
                    }
                    else
                    {
                        selectedSpid = null;
                    }
                }

                foreach (UltraGridRow row in sessionsGrid.Rows.GetAllNonGroupByRows())
                {
                    // fix the colors for critical value fields
                    if (row.IsDataRow)
                    {
                        UltraDataRow dataRow = row.ListObject as UltraDataRow;
                        updateCellAppearance(row.Cells["Blocked By"], dataRow["Blocked By"] != DBNull.Value);
                        updateCellAppearance(row.Cells["Blocking"], (bool)dataRow["Blocking"] == true);
                        updateCellAppearance(row.Cells["Blocking Count"], (bool)dataRow["Blocking"] == true);
                    }

                    // Check if the selection needs to be applied on the first time through
                    if (sessionsGrid.Rows.Count > 0 &&
                        sessionsGrid.Selected.Rows.Count == 0 &&
                        selectedSpidArgument != null &&
                        (int)row.Cells["Session ID"].Value == selectedSpidArgument)
                    {
                        row.Selected = true;
                        sessionsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        selectedSpidIsSystem = (bool)row.Cells["Type Image"].Value;
                        selectedSpid = selectedSpidArgument;
                        selectedSpidArgument = null;
                    }
                }

                if (selectedSpidArgument != null)
                {
                    string msg = string.Format("Session ID {0} has either ended or been excluded by a filter and was unable to be selected because it is not in the current list.", selectedSpidArgument);
                    // the Argument must be cleared before the message is shown or it might be shown several times
                    selectedSpidArgument = null;
                    ApplicationMessageBox.ShowWarning(this, msg);
                }
            }
        }

        public void ShowFilter()
        {
            // Validation for SQL 2000 to use specific session filters, to avoid show TempdbAffecting 2005+ only
            SessionsConfiguration selectFilter = this.serverVersion > 8 ? new SessionsConfiguration(instanceId, null) :
                                                                          new SessionsConfiguration2000(instanceId, null);
            selectFilter.UpdateValues(configuration);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                configuration.UpdateValues(selectFilter);
                filterChanged = true;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

        public void ToggleActiveFilter()
        {
            configuration.Active = !configuration.Active;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            filterChanged = true;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleUserFilter()
        {
            configuration.ExcludeSystemProcesses = !configuration.ExcludeSystemProcesses;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            filterChanged = true;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleBlockedFilter()
        {
            configuration.Blocked = !configuration.Blocked;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            filterChanged = true;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleDetailsPanel()
        {
            DetailsPanelVisible = !DetailsPanelVisible;
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

                    // Get session to try to kill.
                    IEnumerable<Session> subsetSession = (from oneSession in activeSessionList
                        where oneSession.Spid.HasValue && oneSession.Spid.Value == selectedSpid.Value select oneSession).ToList();

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
                        actionMessage = String.Format("Unable to Kill Session ID {0}.", selectedSpid.Value.ToString());
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
            IEnumerable<Session> subset = (from oneSession in activeSessionList
                                           where oneSession.Spid.HasValue && oneSession.Spid.Value == selectedSpid.Value
                                           select oneSession).ToList();
            Session killedSession = subset.First();

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

        private void UpdateDetailsPanel(int? spid)
        {
            try
            {
                if (spid != null
                    && sessionsGrid.Rows.Count > 0
                    && sessionsGrid.Selected.Rows.Count == 1
                    && sessionsGrid.Selected.Rows[0].IsDataRow
                    )
                {
                    if (sessionsGrid.Selected.Rows[0].Cells["Session ID"] != null
                        && (int)sessionsGrid.Selected.Rows[0].Cells["Session ID"].Value == spid)
                    {
                        UpdateDetailsTab();
                    }
                    else
                    {
                        detailsPanel.Visible = false;
                    }
                }
                else
                {
                    detailsPanel.Visible = false;
                }
            }
            catch
            {
                detailsPanel.Visible = false;
            }
        }

        private void UpdateDetailsTab()
        {
            ValueList valueList;
            ValueListItem valueListItem;
            UltraGridRow row = sessionsGrid.Selected.Rows[0];

            valueList = row.Cells["Status Image"].ValueListResolved as ValueList;
            valueListItem = valueList.FindByDataValue(row.Cells["Status Image"].Value);
            if (valueListItem != null)
            {
                boundStatusLabel.Image = valueListItem.Appearance.Image as Bitmap;
            }
            spidLabel.Text = String.Format("{0} Session:", row.Cells["Type"].Text);
            boundSpidLabel.Text = row.Cells["Session ID"].Text;
            valueList = row.Cells["Type Image"].ValueListResolved as ValueList;
            valueListItem = valueList.FindByDataValue(row.Cells["Type Image"].Value);
            if (valueListItem != null)
            {
                boundSpidLabel.Image = valueListItem.Appearance.Image as Bitmap;
            }
            boundUserLabel.Text = row.Cells["User"].Text;
            boundHostLabel.Text = row.Cells["Host"].Text;
            boundDatabaseLabel.Text = row.Cells["Database"].Text;
            boundStatusLabel.Text = row.Cells["Status"].Text;
            boundOpenTransactionsLabel.Text = row.Cells["Open Transactions"].Text;
            boundApplicationLabel.Text = row.Cells["Application"].Text;
            boundExecutionContextLabel.Text = row.Cells["Execution Context"].Text;
            boundWaitTimeLabel.Text = String.Format("{0}", row.Cells["Wait Time"].Text);
            boundWaitTypeLabel.Text = row.Cells["Wait Type"].Text;
            boundWaitResourceLabel.Text = row.Cells["Wait Resource"].Text;
            boundCpuLabel.Text = String.Format("{0} {1}",
                                                    row.Cells["CPU"].Text,
                                                    row.Cells["CPU Delta"].Value == null ?
                                                                string.Empty :
                                                                String.Format(" ({0} delta)",
                                                                                  row.Cells["CPU Delta"].Text)
                                                    );
            boundPhysicalIoLabel.Text = String.Format("{0} {1}",
                                                    row.Cells["Physical I/O"].Text,
                                                    row.Cells["Physical I/O Delta"].Value is System.DBNull ?
                                                                string.Empty :
                                                                String.Format(" ({0} delta)",
                                                                                  row.Cells["Physical I/O Delta"].Text)
                                                   );
            boundMemoryLabel.Text = row.Cells["Memory Usage"].Text;
            boundLoggedInSinceLabel.Text = row.Cells["Login Time"].Text;
            boundLastActivityLabel.Text = row.Cells["Last Batch"].Text;
            boundBlockingLabel.Text = row.Cells["Blocking"].Text;
            boundBlockedByLabel.Text = row.Cells["Blocked By"].Text;
            boundBlockingCountLabel.Text = row.Cells["Blocking Count"].Text;
            boundNetLibraryLabel.Text = row.Cells["Net Library"].Text;
            boundWorkstationNetAddressLabel.Text = row.Cells["Net Address"].Text;

            boundLastCommandTextBox.Text = row.Cells["Last Command"].Text;

            // only show tempdb values for 2005 and later
            if (currentSnapshot != null & serverVersion > 8)
            {
                detailsPanel.SuspendLayout();
                transactionIsolationLabel.Visible =
                    boundTransactionIsolationLevelLabel.Visible =
                    tempdbGroupBox.Visible = true;
                mainDetailsTableLayoutPanel.ColumnStyles[0].Width = 28;
                mainDetailsTableLayoutPanel.ColumnStyles[1].Width = 23;
                mainDetailsTableLayoutPanel.ColumnStyles[2].Width = 20;
                mainDetailsTableLayoutPanel.ColumnStyles[3].Width = 29;
                detailsPanel.ResumeLayout();

                boundVersionStoreElapsedLabel.Text = row.Cells["Version Store Elapsed Seconds"].Text;
                boundTransactionIsolationLevelLabel.Text = row.Cells["Transaction Isolation Level"].Text;
                string spaceFormat = "{0} ({1} / {2})";
                boundSessionUserSpaceLabel.Text = string.Format(spaceFormat,
                                                                    row.Cells["Current Session User Space (KB)"].Text,
                                                                    row.Cells["Session User Space Allocated (KB)"].Text,
                                                                    row.Cells["Session User Space Deallocated (KB)"].Text);
                boundTaskUserSpaceLabel.Text = string.Format(spaceFormat,
                                                                    row.Cells["Current Task User Space (KB)"].Text,
                                                                    row.Cells["Task User Space Allocated (KB)"].Text,
                                                                    row.Cells["Task User Space Deallocated (KB)"].Text);
                boundSessionInternalSpaceLabel.Text = string.Format(spaceFormat,
                                                                    row.Cells["Current Session Internal Space (KB)"].Text,
                                                                    row.Cells["Session Internal Space Allocated (KB)"].Text,
                                                                    row.Cells["Session Internal Space Deallocated (KB)"].Text);
                boundTaskInternalSpaceLabel.Text = string.Format(spaceFormat,
                                                                    row.Cells["Current Task Internal Space (KB)"].Text,
                                                                    row.Cells["Task Internal Space Allocated (KB)"].Text,
                                                                    row.Cells["Task Internal Space Deallocated (KB)"].Text);
            }
            else
            {
                detailsPanel.SuspendLayout();
                transactionIsolationLabel.Visible =
                    boundTransactionIsolationLevelLabel.Visible =
                    tempdbGroupBox.Visible = false;
                mainDetailsTableLayoutPanel.ColumnStyles[0].Width = 35;
                mainDetailsTableLayoutPanel.ColumnStyles[1].Width = 34;
                mainDetailsTableLayoutPanel.ColumnStyles[2].Width = 31;
                mainDetailsTableLayoutPanel.ColumnStyles[3].Width = 0;
                detailsPanel.ResumeLayout();
            }

            detailsPanel.Visible = true;
        }

        #region grid

        private void PrintGrid()
        {
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - session details as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "SessionDetails";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Fix the icon columns to export as text values with a heading
                sessionsGrid.DisplayLayout.Bands[0].Columns["Status Image"].Header.Caption = "Status";
                sessionsGrid.DisplayLayout.ValueLists["statusValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;
                sessionsGrid.DisplayLayout.ValueLists["TypeValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;

                try
                {
                    ultraGridExcelExporter.Export(sessionsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                sessionsGrid.DisplayLayout.Bands[0].Columns["Status Image"].Header.Caption = string.Empty;
                sessionsGrid.DisplayLayout.ValueLists["statusValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
                sessionsGrid.DisplayLayout.ValueLists["TypeValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
            }
        }

        private void updateCellAppearance(UltraGridCell cell, bool hilight)
        {
            if (hilight)
            {
                cell.Appearance.BackColor = Color.Red;
                cell.Appearance.ForeColor = Color.White;
                cell.Appearance.FontData.Bold = DefaultableBoolean.True;
            }
            else
            {
                cell.Appearance.FontData.Bold = DefaultableBoolean.False;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    sessionsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            sessionsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            sessionsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(sessionsGrid);
            dialog.Show(this);
        }

        #endregion

        private void ShowLocksView()
        {
            if (selectedSpid.HasValue)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsLocks, selectedSpid.Value);
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

        #endregion

        #region events

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

        private void hideDetailsPanelButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = false;
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
                case "viewLocksButton":
                    ShowLocksView();
                    break;
                case "viewQueryHistoryButton":
                    ShowQueryHistoryView();
                    break;
                case "traceSessionButton":
                    TraceSession();
                    break;
                case "killSessionButton":
                    KillSession();
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
            if (e.Tool.Key == "gridDataContextMenu")
            {
                if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify &&
                    HistoricalSnapshotDateTime == null)
                {
                    ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;
                    ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;

                    if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
                    {
                        ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].SharedProps.Enabled = false;
                    }
                    bool showKill = false;
                    if (sessionsGrid.Rows.Count > 0
                        && sessionsGrid.Selected.Rows.Count == 1
                        && sessionsGrid.Selected.Rows[0].IsDataRow)
                    {
                        if (sessionsGrid.Selected.Rows[0].Cells["Type"].Value.ToString() == "User")
                        {
                            showKill = true;
                        }
                    }
                    ((PopupMenuTool)e.Tool).Tools["killSessionButton"].SharedProps.Enabled = showKill;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                    ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                }
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = sessionsGrid.Rows.Count > 0 && sessionsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(sessionsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #region grid

        private void sessionsGrid_MouseDown(object sender, MouseEventArgs e)
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

        private void sessionsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (sessionsGrid.Rows.Count > 0
                && sessionsGrid.Selected.Rows.Count == 1
                && sessionsGrid.Selected.Rows[0].IsDataRow)
            {
                selectedSpidIsSystem = (bool)sessionsGrid.Selected.Rows[0].Cells["Type Image"].Value;
                selectedSpid = (int)sessionsGrid.Selected.Rows[0].Cells["Session ID"].Value;
                selectedSpidCommand = sessionsGrid.Selected.Rows[0].Cells["Last Command"].Text;
            }
            else
            {
                selectedSpid = null;
            }
            UpdateDetailsPanel(selectedSpid);
        }

        private void sessionsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = sessionsGrid.DisplayLayout.Bands[0];
            EditorWithText textEditor = new EditorWithText();
            band.Columns["Type Image"].Editor = textEditor;
            band.Columns["Type Image"].GroupByEvaluator = new TypeGroupByEvaluator(sessionsGrid.DisplayLayout.ValueLists["TypeValueList"]);
            band.Columns["Status Image"].Editor = textEditor;
            band.Columns["Status Image"].GroupByEvaluator = new StatusGroupByEvaluator(sessionsGrid.DisplayLayout.ValueLists["statusValueList"]);
        }

        private void sessionsGrid_FilterRow(object sender, FilterRowEventArgs e)
        {
            e.RowFilteredOut = SessionsHelper.RowShouldBeFiltered(configuration, (UltraDataRow)e.Row.ListObject, historicalSnapshotDateTime != null);
        }

        private void ultraGridExcelExporter_CellExporting(object sender, Infragistics.Win.UltraWinGrid.ExcelExport.CellExportingEventArgs e)
        {
            if (e.GridColumn.Key == "Last Command")
            {
                string value = e.Value.ToString();

                if (value.Length > 32767)
                {
                    e.Value = string.Format("{0}...", value.Substring(0, 32764));
                }
            }
        }

        #endregion

        private void SessionsDetailsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
            if(AutoScaleSizeHelper.isScalingRequired)
                this.splitContainer.SplitterDistance = 300;
        }

        #endregion

        public class StatusGroupByEvaluator : Infragistics.Win.UltraWinGrid.IGroupByEvaluatorEx
        {
            private ValueList valueList = null;
            public StatusGroupByEvaluator(ValueList valueList)
            {
                this.valueList = valueList;
            }

            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                ValueListItem item = valueList.FindByDataValue(groupbyRow.Value);
                if (item != null)
                {
                    return item.DisplayText;
                }

                return groupbyRow.Value;
            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                string groupbyValue = groupbyRow.Value as string;
                ValueListItem item = valueList.FindByDataValue(row.Cells[groupbyRow.Column].Value);

                if (groupbyValue == null && item == null)
                {
                    return true;
                }
                else if (groupbyValue != null && item != null)
                {
                    return groupbyValue.Equals(item.DisplayText);
                }

                return false;
            }

            public int Compare(UltraGridCell cell1, UltraGridCell cell2)
            {
                if (cell1.Value.Equals(cell2.Value))
                    return 0;
                else if ((int)cell1.Value < (int)cell2.Value)
                    return -1;
                else
                    return 1;
            }
        }

        public class TypeGroupByEvaluator : Infragistics.Win.UltraWinGrid.IGroupByEvaluatorEx
        {
            private ValueList valueList = null;
            public TypeGroupByEvaluator(ValueList valueList)
            {
                this.valueList = valueList;
            }

            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                ValueListItem item = valueList.FindByDataValue(groupbyRow.Value);
                if (item != null)
                {
                    return item.DisplayText;
                }

                return groupbyRow.Value;
            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                string groupbyValue = groupbyRow.Value as string;
                ValueListItem item = valueList.FindByDataValue(row.Cells[groupbyRow.Column].Value);
                if (item != null)
                    return groupbyValue.Equals(item.DisplayText);

                return false;
            }

            public int Compare(UltraGridCell cell1, UltraGridCell cell2)
            {
                if (((bool)cell1.Value) == ((bool)cell2.Value))
                    return 0;

                ValueListItem item1 = valueList.FindByDataValue(cell1.Value);
                ValueListItem item2 = valueList.FindByDataValue(cell2.Value);
                string text1 = (item1 != null) ? item1.DisplayText : string.Empty;
                string text2 = (item2 != null) ? item2.DisplayText : string.Empty;

                return text1.CompareTo(text2);
            }
        }

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

        /// <summary>
        /// Event called when the 'spaceUsedKbContainerPanel' is resizing. This method is responsible
        /// to put the 'SpaceUsedLabel' always in the middle.
        /// </summary>
        /// 
        /// <param name="sender">The object that has launched the event. There is not used at this
        /// point, but is required by the delegate definition.</param>
        /// <param name="eventArgs">The arguments for the event. There is not used at this point, but is
        /// required by the delegate definition.</param>
        private void OnResizeSpaceUsedKbContainerPanel(object sender, EventArgs eventArgs)
        {
            SetupNormalizedPositionForSpaceUsedKblabel();
        }

        /// <summary>
        /// Set relative position to container size for the 'spaceUsedKb' label field.
        /// </summary>
        private void SetupNormalizedPositionForSpaceUsedKblabel()
        {
            int normalizedSize = spaceUsedKbContainerPanel.Width - spaceUsedkbLabel.Width;
            int centerPanel2 = normalizedSize > 0 ? normalizedSize/2 : 0;
            spaceUsedkbLabel.Location = new Point(centerPanel2, 0);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            changeBackColor();
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.sessionsGrid);
        }

        private void changeBackColor()
        {
            BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.PanelBackColor) : Color.White;
        }
    }
}
