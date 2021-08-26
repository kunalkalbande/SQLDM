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
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    using System.Configuration;
    using Common;
    using Controls;
    using Dialogs;
    using System.Globalization;
    internal partial class SessionsBlockingView : ServerBaseView
    {
        #region constants

        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
		///Ankit Nagpal --Sqldm10.0.0
        private const string UNABLE_TO_UPDATE_SYSADMIN = @"No data is available for this view.";
        private const string ROW_LIMITED =
            @"The blocking locks rowcount limiter has been exceeded.  Collection of the lock list has been cancelled to reduce load on the monitored server.";

        public enum BlockingType
        {
            Locks,
            Sessions
        }

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime = null;
        private Triple<SessionSnapshot, LockDetails, DataTable> currentHistoricalSnapshot;
        private bool locksInitialized = false;
        private bool sessionsInitialized = false;
        private bool blocksDeadlocksListVisible = true;
        private List<Triple<int, DateTime, string>> blocksList = new List<Triple<int, DateTime, string>>();//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 
        private List<Triple<int, DateTime, string>> deadlocksList = new List<Triple<int, DateTime, string>>();//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 

        private IEnumerable<IAuditable> auditableBlockedSessions;

        private bool initialized
        {
            get { return blockingType == BlockingType.Locks ? locksInitialized : sessionsInitialized; }
            set
            {
                if (blockingType == BlockingType.Locks)
                {
                    locksInitialized = value;
                }
                else
                {
                    sessionsInitialized = value;
                }
            }
        }
        private DataTable realTimeChartDataTable;        
        private bool realTimeChartDataTablePrePopulated = false;
        private SessionSummary currentSummary;
        private LockDetailsConfiguration configurationLocks;
        private SessionsConfiguration configurationSessions;
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
        private Control focused = null;
        private static readonly object updateLock = new object();

        private BlockingType blockingType = BlockingType.Sessions;
        private BlockingType currentRealTimeBlockingType = BlockingType.Sessions;
        private BlockingType currentHistoricalBlockingType = BlockingType.Sessions;
        private bool blockingTypeChanging
        {
            get { return blockingType != currentRealTimeBlockingType; }
        }

        private List<UltraTreeColumnSet> locksColumnSets = new List<UltraTreeColumnSet>();
        private List<UltraTreeColumnSet> sessionsColumnSets = new List<UltraTreeColumnSet>();
        private List<UltraTreeColumnSet> saveColumnSets
        {
            get { return (currentRealTimeBlockingType == BlockingType.Locks) ? locksColumnSets : sessionsColumnSets; }
        }
        private List<UltraTreeColumnSet> restoreColumnSets
        {
            get { return (blockingType == BlockingType.Locks) ? locksColumnSets : sessionsColumnSets; }
        }

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private bool lastChartVisible = true;
        private bool  lastBlocksDeadlocksListVisible = true;
        private Exception historyModeLoadError = null;

        #endregion

        #region constructors

        public SessionsBlockingView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(blockedSessionsChart, toolbarsManager);
            blockingTree.DrawFilter = new HideFocusRectangleDrawFilter();

            InitializeChart();

            // Autoscale font size.
            AdaptFontSize();

            blockingTreeStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            blockedSessionsChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;

            // create the initial configuration
            configurationLocks = new LockDetailsConfiguration(instanceId, null, true, true);
            configurationSessions = new SessionsConfiguration(instanceId, null);
            configurationSessions.ExcludeDiagnosticManagerProcesses = false;
            configurationSessions.ExcludeSystemProcesses = false;
            configurationSessions.BlockingOrBlocked = true;
            configurationSummary = new SessionSummaryConfiguration(instanceId);

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }

        #endregion

        #region properties

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler ChartVisibleChanged;
        public event EventHandler BlocksDeadlocksListVisibleChanged;
        public event EventHandler BlockingTypeChanged;
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

                if (blockingTree.Nodes.Count > 0 &&
                    blockingTree.SelectedNodes.Count == 1)
                {
                    DataRowView row = (DataRowView)blockingTree.SelectedNodes[0].ListObject;
                    showKill = !(bool)row["System Process"];
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
                RestoreBlockedSessionsChart();

                if (ChartVisibleChanged != null)
                {
                    ChartVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Blocks and Deadlocks List visibility and trigger state update event if changed
        /// </summary>
        public bool BlocksDeadlocksListVisible
        {
            get { return blocksDeadlocksListVisible; }
            set
            {
                blocksDeadlocksListVisible = value;
                toggleShowBlocksDeadlocksListBoxes();

                if (BlocksDeadlocksListVisibleChanged != null)
                {
                    BlocksDeadlocksListVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public BlockingType BlockingTypeShown
        {
            get { return blockingType; }
            set
            {
                if (blockingType != value)
                {
                    blockingType = value;
                    if (BlockingTypeChanged != null)
                    {
                        BlockingTypeChanged(this, EventArgs.Empty);
                    }
                    ApplicationController.Default.ActiveView.CancelRefresh();
                    ApplicationController.Default.RefreshActiveView();
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public LockDetailsConfiguration Configuration
        {
            get { return configurationLocks; }
        }

        #endregion

        #region methods

        public override void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsBlockingView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.SessionsBlockingViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastChartVisible =
                ChartVisible = Settings.Default.SessionsBlockingViewChartVisible;

            lastBlocksDeadlocksListVisible =
                BlocksDeadlocksListVisible = Settings.Default.SessionsBlockingViewLocksDeadlocksListVisible;
            //lastBlockingType =
            //    blockingType = Settings.Default.SessionsBlockingViewBlockingType;
        }

        public override void SaveSettings()
        {
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastChartVisible != ChartVisible
                || lastBlocksDeadlocksListVisible != BlocksDeadlocksListVisible)
                //|| lastBlockingType != blockingType)
            {
                lastSplitterDistance =
                    Settings.Default.SessionsBlockingViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastChartVisible =
                    Settings.Default.SessionsBlockingViewChartVisible = ChartVisible;
                lastBlocksDeadlocksListVisible =
                    Settings.Default.SessionsBlockingViewLocksDeadlocksListVisible = BlocksDeadlocksListVisible;
                //lastBlockingType =
                //    Settings.Default.SessionsBlockingViewBlockingType = currentBlockingType;
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                (HistoricalSnapshotDateTime != null && 
                (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)) ||
                blockingType != currentHistoricalBlockingType)
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
            if (HistoricalSnapshotDateTime == null)
            {
                return GetRealTimeSnapshot();
            }
            else
            {
                if (ViewMode == ServerViewMode.Historical)
                {
                    if (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime)
                    {
                        return GetHistoricalSnapshot(null, HistoricalSnapshotDateTime.Value, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    }
                    else
                    {
                        return currentHistoricalSnapshot;
                    }
                }
                else
                {
                    return GetHistoricalSnapshot(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                        ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value, ApplicationModel.Default.HistoryTimeValue.CustomTimeMinutes);
                }
            }
        }

        private object GetRealTimeSnapshot()
        {
            //START: SQLdm 10.0 (Tarun Sapra) (Resolving deadlock and blocking issue)- Stopped the backfill refresh, that is scheduled refresh of the foreground UI to sync the data with the list boxes
            PrePopulateRealTimeChartDataTable();
            realTimeChartDataTablePrePopulated = true;
            //END: SQLdm 10.0 (Tarun Sapra) (Resolving deadlock and blocking issue)- Stopped the backfill refresh, that is scheduled refresh of the foreground UI to sync the data with the list boxes            

            var managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            var snapshot = new Pair<Snapshot, SessionSummary>();

            if (blockingType == BlockingType.Locks)
            {
                snapshot.First = managementService.GetLockDetails(configurationLocks);
            }
            else
            {
                snapshot.First = managementService.GetSessions(configurationSessions);
            }

            configurationSummary = new SessionSummaryConfiguration(instanceId, currentSummary);
            snapshot.Second = managementService.GetSessionSummary(configurationSummary);

            return snapshot;
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.realTimeChartDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PrePopulateRealTimeChartDataTable();
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
                PrePopulateRealTimeChartDataTable();
                return;
            }

            FillBlocksDeadlocksData(null, now);
            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            var historicalSnapshot = RepositoryHelper.GetBlockingDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                         instanceId, now, (int)timeDiff.TotalMinutes);

            MethodInvoker UICode = () => AddDeadlockCountsToTable(ref historicalSnapshot.Third);
            this.UIThread(UICode);

        }

        private void PrePopulateRealTimeChartDataTable()
        {
            var now = DateTime.Now;
            var historicalSnapshot = RepositoryHelper.GetBlockingDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                         instanceId, now,
                                                                         ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

            AddDeadlockCountsToTable(ref historicalSnapshot.Third);
            realTimeChartDataTable = historicalSnapshot.Third;            
            FillBlocksDeadlocksData(null, now);
        }

        private object GetHistoricalSnapshot(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            FillBlocksDeadlocksData(startDateTime, endDateTime);
            return RepositoryHelper.GetBlockingDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                        instanceId, endDateTime, minutes, startDateTime);
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                SessionsBlockingView_Fill_Panel.Visible = false;
            }

			blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
            blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE;

            //SQLDM 10.1 (Pulkit Puri)
            //SQLDM-25642 and SQLDM-25651 (Pulkit Puri)-- The sysadmin check must be after assigning UNABLE_TO_UPDATE to text 
            //if (!isUserSysAdmin)
            //{
            //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
            //    blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
            //}

            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                // save the current ColumnSets to be restored if display type changes
                saveColumnSets.Clear();
                foreach (UltraTreeColumnSet cs in blockingTree.ColumnSettings.ColumnSets)
                {
                    saveColumnSets.Add(cs);
                }                

                if (HistoricalSnapshotDateTime == null)
                {
                   UpdateDataWithRealTimeSnapshot((Pair<Snapshot, SessionSummary>)data);
                }
                else
                {                    
                    UpdateDataWithHistoricalSnapshot((Triple<SessionSnapshot, LockDetails, DataTable>)data);
                }
                PopulateBlocksListbox();
                PopulateDeadlocksListbox();
            }
        }

        private void FillBlocksDeadlocksData(DateTime? startDateTime, DateTime endDateTime)
        {
            if(startDateTime == null)
                startDateTime = endDateTime.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

            blocksList = RepositoryHelper.GetBlocksRange(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, startDateTime.Value, endDateTime);
            deadlocksList = RepositoryHelper.GetDeadlocksRange(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, startDateTime.Value, endDateTime);
        }
        private void PopulateBlocksListbox()
        {//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 
            locksListBox.Items.Clear();
            if (blocksList != null && blocksList.Count > 0)
            {
                foreach (Triple<int, DateTime, string> blocksListEntry in blocksList)
                {
                    locksListBox.Items.Add(new DeadLockOrBlock(blocksListEntry.First, blocksListEntry.Second));
                }
                locksListBox.SetSelected(0, true);
            }
        }

        private void PopulateDeadlocksListbox()
        {
            deadlocksListBox.Items.Clear();
            if (deadlocksList != null && deadlocksList.Count > 0)
            {//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 
                foreach (Triple<int,DateTime, string> deadlocksListEntry in deadlocksList)
                {
                    deadlocksListBox.Items.Add(new DeadLockOrBlock(deadlocksListEntry.First, deadlocksListEntry.Second));
                }
                deadlocksListBox.SetSelected(0, true);
            }
        }

        private void locksListBox_MouseDoubleClick(object sender, EventArgs e)
        {
            object selectedItem = locksListBox.SelectedItem;
            if (selectedItem is DeadLockOrBlock)
            {//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same.
                DeadLockOrBlock selectedValue = (DeadLockOrBlock)selectedItem;
                foreach (Triple<int, DateTime, string> blockEntry in blocksList)
                {
                    if (blockEntry.First == selectedValue.Key)
                    {
                        string xdl = blockEntry.Third;
                        try
                        {
                            BlockDialog.Show(this, blockEntry.Second, xdl);
                        }
                        catch (Exception ex)
                        {
                            Log.DebugFormat("BlockDialog Show exception: {0}", ex.Message);
                        }
                    }
                }
               
            }
        }

        private void deadlocksListBox_MouseDoubleClick(object sender, EventArgs e)
        {
            object selectedItem = deadlocksListBox.SelectedItem;
            if (selectedItem is DeadLockOrBlock)
            {//SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 
                DeadLockOrBlock selectedValue = (DeadLockOrBlock)selectedItem;
                foreach (Triple<int, DateTime, string> deadlockEntry in deadlocksList)
                {
                    if (deadlockEntry.First == selectedValue.Key)
                    {
                        string xdl = deadlockEntry.Third;
                        try
                        {
                            DeadlockDialog.Show(this, deadlockEntry.Second, xdl);
                        }
                        catch (Exception ex)
                        {
                            Log.DebugFormat("DeadlockDialog Show exception: {0}", ex.Message);
                        }
                    }
                }
            }
        }

        private static IEnumerable<IAuditable> GetAuditableBlockedSessions<T, TK>(Dictionary<T, TK> blockSessionsCollection)
        {
            var blockedSessionsList = (from auditableElement in blockSessionsCollection 
                where auditableElement.Value is IAuditable select auditableElement.Value).ToList();

            IEnumerable<IAuditable> blockedSessions;

            if (blockedSessionsList.Any())
            {
                blockedSessions = blockedSessionsList as IEnumerable<IAuditable>;
            }
            else
            {
                blockedSessions = new List<IAuditable>() as IEnumerable<IAuditable>;
            }

            return blockedSessions;
        }

        private void UpdateDataWithRealTimeSnapshot(Pair<Snapshot, SessionSummary> snapshot)
        {
            operationalStatusPanel.Visible = false;
            SessionsBlockingView_Fill_Panel.Visible = true;

            if (snapshot.First != null)
            {
                if (snapshot.First.Error == null)
                {
                    if (snapshot.First is LockDetails)
                    {
                        selectedSpid = null;
                        restoreTreeColumns();
                        if (((LockDetails)snapshot.First).HasBeenRowLimited)
                        {
                            blockingTree.Visible = false;
                            blockingTreeStatusLabel.Text = ROW_LIMITED;
                            ApplicationController.Default.ClearCustomStatus();
                        }
                        else
                        {
                            LockDetails snapshotLockDetails = snapshot.First as LockDetails;
                            BuildBlockingTree(snapshotLockDetails.BlockingTreeByLock);

                            // Get Auditable session list.
                            auditableBlockedSessions = GetAuditableBlockedSessions(snapshotLockDetails.LockList);
                        }
                        currentRealTimeBlockingType = BlockingType.Locks;
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else if (snapshot.First is SessionSnapshot)
                    {
                        selectedSpid = null;
                        restoreTreeColumns();
                        SessionSnapshot snapshotSession = snapshot.First as SessionSnapshot;
                        BuildBlockingTree(snapshotSession.BlockingTreeBySession);
                        currentRealTimeBlockingType = BlockingType.Sessions;
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                        // Get Auditable session list.
                        auditableBlockedSessions = GetAuditableBlockedSessions(snapshotSession.SessionList);
                    }
                    else
                    {
                        selectedSpid = null;
                        blockingTree.Visible = false;
                        blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
                        ///Ankit Nagpal --Sqldm10.0.0
						///If not a sysadmin display sysadmin message
                        //if (!isUserSysAdmin)
                        //{
                        //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                         
                        //}
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                                new DesktopClientException("Unknown snapshot type.")));
                    }
                }
                else
                {
                    selectedSpid = null;
                    blockingTree.Visible = false;
                    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
					///Ankit Nagpal --Sqldm10.0.0
					///If not a sysadmin display sysadmin message
     
                    //if (!isUserSysAdmin)
                    //{
                    //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;

                    //}
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.First.Error));
                }

                 if (snapshot.Second != null)
                 {
                     if (snapshot.Second.Error == null)
                     {
                         realTimeChartDataTable.BeginLoadData();
                         
                         DataRow newRow = realTimeChartDataTable.Rows.Add(new object[] { DateTime.Now });

                         if (snapshot.Second.SystemProcesses.BlockedProcesses.HasValue)
                         {
                             newRow["Blocked Sessions"] = snapshot.Second.SystemProcesses.BlockedProcesses.Value;
                         }

                         if (snapshot.Second.SystemProcesses.BlockedProcesses.HasValue)
                         {
                             newRow["Lead Blockers"] = snapshot.Second.SystemProcesses.LeadBlockers.Value;
                         }

                         if (snapshot.Second.LockCounters.TotalCounters.Deadlocks.HasValue)
                         {
                             newRow["Total Deadlocks"] = snapshot.Second.LockCounters.TotalCounters.Deadlocks;
                         }
                         GroomHistoryData();
                         UpdateChartDataFilter();
                         realTimeChartDataTable.EndLoadData();

                         //START: Update the realTimeChartDataTable to fill up, only the historical data for deadlocks
                         DataTable copyOfRealTimeChartDataTable = realTimeChartDataTable.Copy();
                         SynchBlockDeadlockCount(ref copyOfRealTimeChartDataTable);//SQLdm 10.0 (Tarun Sapra) (Resolving deadlock and blocking issue)- With every refresh, the data of the list boxes and data tables is going to be synced up
                         blockedSessionsChart.DataSource = copyOfRealTimeChartDataTable;//SQLdm 10.0 (Tarun Sapra) (Resolving blocking and deadlock issue)- added a new data table
                         //END: Update the realTimeChartDataTable to fill up, only the historical data for deadlocks

                         blockedSessionsChart.Visible = true;
                         currentSummary = snapshot.Second;
                     }
                     else
                     {
                         blockedSessionsChart.Visible = false;
                         blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE;
                   
						///Ankit Nagpal --Sqldm10.0.0
						///If not a sysadmin display sysadmin message

                         //if (!isUserSysAdmin)
                         //{
                         
                         //    blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                         //}
                         ApplicationController.Default.OnRefreshActiveViewCompleted(
                             new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Second.Error));
                     }
                 }
             }
             else
             {
                 selectedSpid = null;
                 blockingTree.Visible = false;
                 blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
                 blockedSessionsChart.Visible = false;
                 blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE;
              
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message

                 //if (!isUserSysAdmin)
                 //{
                 //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                 //    blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                 //}
                 ApplicationController.Default.OnRefreshActiveViewCompleted(
                     new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                             new DesktopClientException("Snapshot data is null.")));
             }
         }

        private void UpdateDataWithHistoricalSnapshot(Triple<SessionSnapshot, LockDetails, DataTable> snapshot)
        {
            SessionSnapshot sessionSnapshot = snapshot.First;

            if (blockingType == BlockingType.Sessions)
            {
                // SqlDM 10.2 (Anshul Aggarwal) New History Browser - DB Snapshot datetime should match the snapshot user selected in console.
                // If user selected custom range, then the latest snapshot in the range will be returned from DB and displayed to user.
                if (sessionSnapshot != null && (ViewMode == ServerViewMode.Custom ||
                (sessionSnapshot.TimeStamp != null && HistoricalSnapshotDateTime == sessionSnapshot.TimeStamp.Value.ToLocalTime())))
                {
                    if (sessionSnapshot.Error == null)
                    {
                        selectedSpid = null;
                        restoreTreeColumns();
                        BuildBlockingTree(sessionSnapshot.BlockingTreeBySession);
                        SessionsBlockingView_Fill_Panel.Visible = true;
                        ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                              Properties.Resources.StatusWarningSmall);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        currentHistoricalBlockingType = BlockingType.Sessions;
                        currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                        currentHistoricalStartDateTime = HistoricalStartDateTime;
                        currentHistoricalSnapshot = snapshot;

                        // Get Auditable session list.
                        auditableBlockedSessions = GetAuditableBlockedSessions(sessionSnapshot.SessionList);
                    }
                    else
                    {
                        selectedSpid = null;
                        blockingTree.Visible = false;
                        blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
             			///Ankit Nagpal --Sqldm10.0.0
						///If not a sysadmin display sysadmin message

                        //if (!isUserSysAdmin)
                        //{
                        //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                           
                        //}
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, sessionSnapshot.Error));
                    }
                }
                else
                {
                    historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                    SessionsBlockingView_Fill_Panel.Visible = false;
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
            }
            else if (blockingType == BlockingType.Locks)
            {
                LockDetails lockDetails = snapshot.Second;
                // SqlDM 10.2 (Anshul Aggarwal) New History Browser - DB Snapshot datetime should match the snapshot user selected in console.
                // If user selected custom range, then the latest snapshot in the range will be returned from DB and displayed to user.
                if (lockDetails != null && (ViewMode == ServerViewMode.Custom ||
                (lockDetails.TimeStamp != null && HistoricalSnapshotDateTime == lockDetails.TimeStamp.Value.ToLocalTime())))
                {
                    if (lockDetails.Error == null)
                    {
                        selectedSpid = null;
                        restoreTreeColumns();
                        BuildBlockingTree(lockDetails.BlockingTreeByLock);
                        SessionsBlockingView_Fill_Panel.Visible = true;
                        ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                              Properties.Resources.StatusWarningSmall);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        currentHistoricalBlockingType = BlockingType.Locks;
                        currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                        currentHistoricalStartDateTime = HistoricalStartDateTime;
                        currentHistoricalSnapshot = snapshot;

                        // Get Auditable session list.
                        auditableBlockedSessions = GetAuditableBlockedSessions(lockDetails.LockList);
                    }
                    else
                    {
                        selectedSpid = null;
                        blockingTree.Visible = false;
                        blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE;
						///Ankit Nagpal --Sqldm10.0.0
						///If not a sysadmin display sysadmin message
                        //if (!isUserSysAdmin)
                        //{
                        //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                         
                        //}
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, sessionSnapshot.Error));
                    }
                }
                else
                {
                    historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                    SessionsBlockingView_Fill_Panel.Visible = false;
                }
            }

            if (snapshot.Third != null)
            {
                AddDeadlockCountsToTable(ref snapshot.Third);
                //START: SQLdm 10.0 (Tarun Sapra) (fixing deadlock and blocking issue)- Syncing up of the data btw table in listboxes

                DataTable copySnapshot = snapshot.Third.Copy();
                SynchBlockDeadlockCount(ref copySnapshot);                
                blockedSessionsChart.DataSource = copySnapshot;

                //END: SQLdm 10.0 (Tarun Sapra) (fixing deadlock and blocking issue)- Syncing up of the data btw table in listboxes
                blockedSessionsChart.Visible = true;
            }
            else
            {
                blockedSessionsChart.Visible = false;
                blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE;
   				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                //if (!isUserSysAdmin)
                //{
                //    blockingTreeStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                //    blockedSessionsChartStatusLabel.Text = UNABLE_TO_UPDATE_SYSADMIN;
                //}
            }
        }

        /// <summary>
        /// sqldm 10.0 -- vineet -- de45706 synch report data and chart data
        /// </summary>
        /// <param name="dataTable"></param>
        private void SynchBlockDeadlockCount(ref DataTable dataTable)
        {
            if (dataTable.Columns.Contains("Total Deadlocks") == false)
            {   // make sure the total deadlocks column is in the table
                dataTable.Columns.Add("Total Deadlocks", typeof(long));
            }
            if (dataTable.Columns.Contains("IsHistorical") == false)
                dataTable.Columns.Add("IsHistorical", typeof(bool));

            dataTable.BeginLoadData();

            bool found = false;

            //Clear all the entries of blocks and deadlocks
            foreach (DataRow row in dataTable.Rows)
                row["Total Deadlocks"] = 0;//(All panel realtime and only deadlock come from historical snapshot): Removing all the data for deadlocks and filling it up using listbox's data 
            //row["Blocked Sessions"] = row["Total Deadlocks"] = 0;

            //START: (All panel realtime and only deadlock come from historical snapshot): Removing this syncing with blocks listBox, we may need this later
            //Increment if timestamp is found, else add a new row
            /*foreach (var block in blocksList)
            {
                found = false;
                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime dt = (DateTime)row["Date"];
                    if (dt.Equals(block.Second))
                    {
                        row["Blocked Sessions"] = (int)row["Blocked Sessions"] + 1;
                        found = true;
                    }
                }
                if (found == false)
                {
                    DataRow newRow = dataTable.Rows.Add(new object[] { block.Second });
                    newRow["Blocked Sessions"] = 1;
                    newRow["Total Deadlocks"] = 0;
                }
            }*/
            //END: (All panel realtime and only deadlock come from historical snapshot): Removing this syncing with blocks listBox, we may need this later

            foreach (var deadlock in deadlocksList)
            {
                found = false;
                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime dt = (DateTime)row["Date"];
                    if (dt.Equals(deadlock.Second))
                    {
                        row["Total Deadlocks"] = (long)row["Total Deadlocks"] + 1;
                        found = true;
                    }
                }
                if (found == false)
                {
                    DataRow newRow = dataTable.Rows.Add(new object[] { deadlock.Second });
                    newRow["Blocked Sessions"] = 0;
                    newRow["Total Deadlocks"] = 1;
                }
            }
            
            //sorting data tables for the new rows(if) entered
            DataView dv = dataTable.DefaultView;
            dv.Sort = "Date asc";
            dataTable = dv.ToTable();            

            dataTable.EndLoadData();
        }

        private void AddDeadlockCountsToTable(ref DataTable dataTable)
        {
            if (dataTable.Columns.Contains("Total Deadlocks") == false)
            {   // make sure the total deadlocks column is in the table
                dataTable.Columns.Add("Total Deadlocks", typeof(long));
            }
            if (dataTable.Columns.Contains("IsHistorical") == false)
                dataTable.Columns.Add("IsHistorical", typeof(bool));

            dataTable.BeginLoadData();
            foreach (DataRow row in dataTable.Rows)
            {
                LockStatistics lockStatistics = null;
                row["IsHistorical"] = true;
                if (row["Lock Statistics"] != DBNull.Value)
                {
                    lockStatistics =
                        Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>((byte[]) row["Lock Statistics"]);

                    if (lockStatistics.TotalCounters.Deadlocks.HasValue)
                    {
                        row["Total Deadlocks"] = lockStatistics.TotalCounters.Deadlocks;
                    }
                    else
                    {
                        row["Total Deadlocks"] = 0;
                    }
                }
                else
                {
                    row["Total Deadlocks"] = 0;
                }
            }
            dataTable.EndLoadData();
        }

        private void BuildBlockingTree(DataSet dataSet)
        {
            if (dataSet != null)
            {
                if (dataSet.Tables["Lead Blockers"].Rows.Count > 0)
                {
                    blockingTree.SetDataBinding(dataSet, "Lead Blockers");

                    foreach (UltraTreeColumnSet cs in blockingTree.ColumnSettings.ColumnSets)
                    {
                        if (!initialized)
                        {
                            foreach (UltraTreeNodeColumn col in cs.Columns)
                            {
                                col.PerformAutoResize(ColumnAutoSizeMode.AllNodes);
                            }
                        }
                        
                        //This line is where we keep breaking when trying to view locks by lock
                        //cs.Columns["Session ID"].PerformAutoResize(ColumnAutoSizeMode.AllNodesWithDescendants);
                    }
                    if (!initialized && dataSet.Tables[0].Rows.Count > 0)
                    {
                        initialized = true;
                    }

                    ExpandAllTrees();
                    blockingTree.Visible = true;
                    ApplicationController.Default.SetCustomStatus(
                        String.Format("{0} Blocking Tree{1}",
                                dataSet.Tables["Lead Blockers"].Rows.Count,
                                dataSet.Tables["Lead Blockers"].Rows.Count == 1 ? string.Empty : "s")
                        );
                }
                else
                {
                    blockingTree.Visible = false;
                    blockingTreeStatusLabel.Text = NO_ITEMS;
                    ApplicationController.Default.ClearCustomStatus();
                }
            }
        }

        public void TraceSession()
        {
            if (selectedSpid.HasValue)
            {
                Dialogs.SessionTraceDialog dialog = new Dialogs.SessionTraceDialog(instanceId, selectedSpid.Value);
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
                    try
                    {
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
                            if (selectedSpid.HasValue)
                            {
                                actionMessage = String.Format("Unable to Kill Session ID {0}.", selectedSpid.Value.ToString());
                                ApplicationMessageBox.ShowError(this, actionMessage, snapshot.Error);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Log.DebugFormat("Asyncrhronus update exception: {0}", e.Message);
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

            IAuditable selectedSession;
            FindSelectedSession(selectedSpid.Value, out selectedSession);
            // Proceded for auditing the action.
            AuditableEntity entity = selectedSession.GetAuditableEntity();
            entity.SqlUser = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser;
            entity.AddMetadataProperty("Action", "The Session has been terminated.");
            entity.AddMetadataProperty("Server Name", blockedSessionMachineName);

            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("KillSessionEntity", entity);

            #endregion Change Log. Setting up context data
        }

        private bool FindSelectedSession(int selectedSPID, out IAuditable selectedSession)
        {
            bool hasFoundSelectedSession;
            selectedSession = null;
            IEnumerable<IAuditable> subsetSessions = null;

            if (this.auditableBlockedSessions is IEnumerable<Lock>)
            {
                // Get selected session sub set.
                var sessionLocks = auditableBlockedSessions as List<Lock>;
                subsetSessions = (from lockSession in sessionLocks
                    where lockSession.Spid.HasValue && lockSession.Spid.Value == selectedSPID
                    select lockSession).ToList();
            }
            else if (this.auditableBlockedSessions is IEnumerable<Session>)
            {
                // Get Selected session sub set.
                var sessionList = auditableBlockedSessions as IEnumerable<Session>;
                subsetSessions = (from sessionElement in sessionList
                                 where sessionElement.Spid.HasValue && sessionElement.Spid.Value == selectedSPID
                                 select sessionElement).ToList();
            }
            else
            {
                // Unable to get selected session.
                hasFoundSelectedSession = false;
            }

            // Get selected session.
            if (subsetSessions.Any())
            {
                // Get the first occurrence.
                selectedSession = subsetSessions.First();
                hasFoundSelectedSession = true;
            }
            else
            {
                hasFoundSelectedSession = false;
            }

            return hasFoundSelectedSession;
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

        #region tree

        private void restoreTreeColumns()
        {
            // if the type is changing, load any previous saved column settings
            if (blockingTypeChanging)
            {
                blockingTree.DataSource = null;
                blockingTree.ColumnSettings.ColumnSets.Clear();

                if (restoreColumnSets.Count > 0)
                {
                    foreach (UltraTreeColumnSet cs in restoreColumnSets)
                    {
                        blockingTree.ColumnSettings.ColumnSets.Add(cs);
                    }
                }
            }
        }

        #endregion

        #region chart

        private void InitializeChart()
        {
            blockedSessionsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            blockedSessionsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            blockedSessionsChart.Printer.Orientation = PageOrientation.Landscape;
            blockedSessionsChart.Printer.Compress = true;
            blockedSessionsChart.Printer.ForceColors = true;
            blockedSessionsChart.Printer.Document.DocumentName = "Sessions Blocking Chart";
            blockedSessionsChart.ToolBar.RemoveAt(0);
        
            ConfigureBlockedSessionsChart();
            InitalizeDrilldown(blockedSessionsChart);  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ConfigureBlockedSessionsChart()
        {
            blockedSessionsChart.DataSourceSettings.Fields.Clear();
            blockedSessionsChart.ToolTipFormat = "%v %s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            FieldMap blockedSessionsFieldMap = new FieldMap("Blocked Sessions", FieldUsage.Value);
            blockedSessionsFieldMap.DisplayName = "Blocked Sessions";
            FieldMap leadBlockersFieldMap = new FieldMap("Lead Blockers", FieldUsage.Value);
            leadBlockersFieldMap.DisplayName = "Lead Blockers";
            FieldMap deadlockedFieldMap = new FieldMap("Total Deadlocks", FieldUsage.Value);
            deadlockedFieldMap.DisplayName = "Total Deadlocks";

            blockedSessionsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        blockedSessionsFieldMap,
                        leadBlockersFieldMap,
                        deadlockedFieldMap});

            blockedSessionsChart.DataSourceSettings.ReloadData();
        }

        private void ToggleChartToolbar(bool Visible)
        {
            blockedSessionsChart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, blockedSessionsChart, "Blocked Sessions", ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, blockedSessionsChart, "BlockedSessions");
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, blockedSessionsChart, "Blocked Sessions", "BlockedSessions");
        }

        private void MaximizeBlockedSessionsChart()
        {
            splitContainer.Visible = false;
            splitContainer.Panel2.Controls.Remove(blockedSessionsChartPanel);
            maximizeBlockedSessionsChartButton.Visible = false;
            restoreBlockedSessionsChartButton.Visible = true;
            contentPanel.Controls.Add(blockedSessionsChartPanel);
        }

        private void RestoreBlockedSessionsChart()
        {
            contentPanel.Controls.Remove(blockedSessionsChartPanel);
            maximizeBlockedSessionsChartButton.Visible = true;
            restoreBlockedSessionsChartButton.Visible = false;
            splitContainer.Panel2.Controls.Add(blockedSessionsChartPanel);
            splitContainer.Visible = true;
        }

        private void toggleShowBlocksDeadlocksListBoxes()
        {
            if (!blocksDeadlocksListVisible)
            {
                this.splitContainer.Panel1.Controls.Remove(this.splitContainer1);
                this.splitContainer.Panel1.Controls.Add(this.blockingTree);
                this.splitContainer.Panel1.Controls.Add(this.blockingTreeStatusLabel);
            }
            else
            {
                this.splitContainer.Panel1.Controls.Add(this.splitContainer1);
                this.splitContainer.Panel1.Controls.Remove(this.blockingTree);
                this.splitContainer.Panel1.Controls.Remove(this.blockingTreeStatusLabel);
                this.splitContainer1.Panel1.Controls.Add(this.blockingTree);
                this.splitContainer1.Panel1.Controls.Add(this.blockingTreeStatusLabel);
            }
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

        private void ShowLocksView()
        {
            if (selectedSpid.HasValue)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsLocks, selectedSpid.Value);
            }
        }

        private void CollapseAllTrees()
        {
            blockingTree.CollapseAll();
        }

        private void ExpandAllTrees()
        {
            //Expand just the first few nodes because deadlocks produce an infinite tree
            foreach (UltraTreeNode node in blockingTree.Nodes)
            {
                node.Expanded = true;
                foreach (UltraTreeNode node2 in node.Nodes)
                {
                    node2.Expanded = true;
                    foreach (UltraTreeNode node3 in node2.Nodes)
                    {
                        node3.Expanded = true;
                        foreach (UltraTreeNode node4 in node3.Nodes)
                        {
                            node4.Expanded = true;
                            foreach (UltraTreeNode node5 in node4.Nodes)
                            {
                                node5.Expanded = true;
                            }
                        }
                    }
                }
            }
        }

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

        private void closeBlockedSessionsChartButton_Click(object sender, EventArgs e)
        {
            ChartVisible = false;
            RestoreBlockedSessionsChart();
        }

        private void maximizeBlockedSessionsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeBlockedSessionsChart();
        }

        private void restoreBlockedSessionsChartButton_Click(object sender, EventArgs e)
        {
            RestoreBlockedSessionsChart();
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

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "viewSessionDetailButton":
                    ShowDetailsView();
                    break;
                case "viewLockButton":
                    ShowLocksView();
                    break;
                case "traceSessionButton":
                    TraceSession();
                    break;
                case "killSessionButton":
                    KillSession();
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
                case "collapseAllTreesButton":
                    CollapseAllTrees();
                    break;
                case "expandAllTreesButton":
                    ExpandAllTrees();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "blockingTreeDataContextMenu":
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

                    ((PopupMenuTool)e.Tool).Tools["collapseAllTreesButton"].SharedProps.Enabled =
                        ((PopupMenuTool)e.Tool).Tools["expandAllTreesButton"].SharedProps.Enabled = blockingTree.Nodes.Count > 0;
                    break;
                case "chartContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(blockedSessionsChart.ToolBar.Visible);
                    break;
            }
        }

        private void blockingTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UltraTreeNode selectedNode =
                    ((UltraTree)sender).GetNodeFromPoint(new Point(e.X, e.Y));
                if (selectedNode != null)
                {
                    selectedNode.Selected = true;
                    toolbarsManager.SetContextMenuUltra(((UltraTree)sender), "blockingTreeDataContextMenu");
                }
                else
                {
                    toolbarsManager.SetContextMenuUltra(((UltraTree)sender), "blockingTreeContextMenu");
                }
            }
        }

        private void blockingTree_AfterSelect(object sender, SelectEventArgs e)
        {
            if (blockingTree.SelectedNodes.Count == 1)
            {
                try
                {
                    selectedSpid = Convert.ToInt32(blockingTree.SelectedNodes[0].Cells["Session ID"].Value);
                }
                catch
                {
                    selectedSpid = null;
                }
            }
            else
            {
                selectedSpid = null;
            }
        }

        private void blockingTree_ColumnSetGenerated(object sender, ColumnSetGeneratedEventArgs e)
        {
            e.ColumnSet.TipStyleCell = TipStyleCell.Show;
            e.ColumnSet.ShowSortIndicators = DefaultableBoolean.False;
            e.ColumnSet.ColumnAutoSizeMode = ColumnAutoSizeMode.None;
            e.ColumnSet.ColumnSettings.AutoFitColumns = AutoFitColumns.None;

            if (e.ColumnSet.Columns.Exists("Object Schema"))
                e.ColumnSet.Columns.Remove(e.ColumnSet.Columns["Object Schema"]);
            if (e.ColumnSet.Columns.Exists("Object Name"))
                e.ColumnSet.Columns.Remove(e.ColumnSet.Columns["Object Name"]);
            if (e.ColumnSet.Columns.Exists("Type"))
                e.ColumnSet.Columns.Remove(e.ColumnSet.Columns["Type"]);
            if (e.ColumnSet.Columns.Exists("RecordID"))
                e.ColumnSet.Columns.Remove(e.ColumnSet.Columns["RecordID"]);
            if (e.ColumnSet.Columns.Exists("ParentID"))
                e.ColumnSet.Columns.Remove(e.ColumnSet.Columns["ParentID"]);
        }

        private void SessionsBlockingView_Load(object sender, EventArgs e)
        {
            ApplySettings();
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

    /// <summary>
    /// //SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. 
    /// </summary>
    public class DeadLockOrBlock 
    {
        public int Key;
        public DateTime Value;
        public DeadLockOrBlock(int key, DateTime value)
        {
            this.Key = key;
            this.Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
