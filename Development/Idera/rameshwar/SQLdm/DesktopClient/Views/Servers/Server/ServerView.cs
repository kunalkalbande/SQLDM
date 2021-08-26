using System;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    internal partial class ServerView : ServerBaseView
    {
        private enum SnapshotBoundaryCondition
        {
            Earliest,
            MostRecent
        }

        private const int HistoryBrowserMinimizedSize = 33;

        private ServerViewMode viewMode = ServerViewMode.RealTime;
        private DateTime? currentHistoricalSnapshotDateTime = null;
        private Control focused = null;
        private bool historyBrowserSnapshotChanged = false;

        public event EventHandler HistoryBrowserVisibleChanged;
        public event EventHandler<SubViewChangedEventArgs> SubViewChanged;

        public ServerView(int instanceId) : base(instanceId)
        {
            if (!ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                throw new ArgumentException(
                    string.Format("The instance ID {0} is not an active instance.", instanceId), "instanceId");
            }

            InitializeComponent();

            UpdateInstanceTitle();
            historyTimestampLabel.Text = String.Empty;
            HistoryBrowserVisible = false;
            ribbonControl.Initialize(instanceId, this);
            historyBrowserControl.Initialize(instanceId);
        }

        public override void ShowHelp() {
            ribbonControl.ShowActiveViewHelp();
        }

        //public int InstanceId
        //{
        //    get { return instanceId; }
        //}

        public ServerViewMode ViewMode
        {
            get { return viewMode; }
        }

        public ServerViewTabs SelectedTab
        {
            get { return ribbonControl.SelectedTab; }
            set { ribbonControl.SelectedTab = value; }
        }

        public object ActiveView
        {
            get { return ribbonControl.ActiveView; }
        }

        public bool HistoryBrowserVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (HistoryBrowserVisibleChanged != null)
                {
                    HistoryBrowserVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return currentHistoricalSnapshotDateTime; }
            set
            {
                if (!historyBrowserSnapshotChanged)
                {
                    historyBrowserControl.ClearSnapshotSelection();
                }

                if (currentHistoricalSnapshotDateTime != value)
                {
                    currentHistoricalSnapshotDateTime = value;

                    if (currentHistoricalSnapshotDateTime.HasValue)
                    {
                        historyTimestampLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                        headerStrip.State = HeaderStripState.Pressed;
                        SetViewMode(ServerViewMode.Historical);
                    }
                    else
                    {
                        historyTimestampLabel.Text = String.Empty;
                        headerStrip.State = HeaderStripState.Normal;
                        SetViewMode(ServerViewMode.RealTime);
                    }
                }
            }
        }

        private void UpdateInstanceTitle()
        {
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                titleLabel.Text = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;

                if (ViewMode == ServerViewMode.RealTime &&
                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) == PermissionType.View)
                {
                    titleLabel.Text += " (View Only)";
                }
            }
        }

        public void ShowPreviousHistoricalSnapshot()
        {
            if (historyBrowserBackgroundWorker.IsBusy)
            {
                ApplicationMessageBox.ShowInfo(ParentForm,
                                               "A historical snapshot is currently being retrieved. Please wait until the operation completes and try again.");
            }
            else
            {
                if (ViewMode == ServerViewMode.RealTime)
                {
                    historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(DateTime.Now, true));
                }
                else
                {
                    historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(currentHistoricalSnapshotDateTime.Value, true));
                }
            }
        }

        public void ShowNextHistoricalSnapshot()
        {
            if (ViewMode == ServerViewMode.RealTime)
            {
                ApplicationMessageBox.ShowInfo(ParentForm,
                                               "You are currently viewing a real time snapshot. A more recent historical snapshot does not exist.");
            }
            else if (historyBrowserBackgroundWorker.IsBusy)
            {
                ApplicationMessageBox.ShowInfo(ParentForm,
                                               "A historical snapshot is currently being retrieved. Please wait until the operation completes and try again.");
            }
            else
            {
                historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(currentHistoricalSnapshotDateTime.Value, false));
            }
        }

        private void SetViewMode(ServerViewMode newViewMode)
        {
            viewMode = newViewMode;
            UpdateInstanceTitle();
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ShowView(ServerViews view, object argument)
        {
            ribbonControl.ShowView(view, argument);
        }

        public override void SaveSettings()
        {
            ribbonControl.SaveViewSettings();
        }

        public void SetActive(bool value)
        {
            ribbonControl.SetActive(value);
        }

        private void OnSubViewChanged(SubViewChangedEventArgs e)
        {
            if (SubViewChanged != null)
            {
                SubViewChanged(this, e);
            }
        }

        public override void CancelRefresh()
        {
            ribbonControl.CancelRefresh();
        }

        public override void RefreshView()
        {
            ribbonControl.RefreshView();
        }

        public override void UpdateUserTokenAttributes()
        {
            UpdateInstanceTitle();
            ribbonControl.UpdateUserTokenAttributes();
        }

        internal override void OnServerAlertConfigurationChanged(System.Collections.Generic.IEnumerable<Common.Thresholds.MetricThresholdEntry> thresholdEntries)
        {
            // pass this information along to the selected active server view
            var activeView = ActiveView as ServerBaseView;
            if (activeView != null)
                activeView.OnServerAlertConfigurationChanged(thresholdEntries);
        }

        /// <summary>
        /// This method does not raise an event; it's called by the ApplicationController to allow
        /// the ServerView to perform view specific actions after a refresh completes.
        /// </summary>
        public void OnRefreshCompleted(RefreshActiveViewCompletedEventArgs e)
        {
            if (HistoricalSnapshotDateTime.HasValue)
            {
                historyTimestampLabel.Text = currentHistoricalSnapshotDateTime.Value.ToString("f");
                historyBrowserControl.AddRecentlyViewedSnapshot(currentHistoricalSnapshotDateTime.Value);
            }

            ribbonControl.UpdateHistoryGroupOptions(true);
        }

        private void ribbonControl_SubViewChanged(object sender, SubViewChangedEventArgs e)
        {
            OnSubViewChanged(e);
        }

        private Control GetFocusedControl(Control.ControlCollection controls)
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

            return focusedControl ?? splitContainer.Panel1;
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

        private void historyBrowserControl_MinimizedChanged(object sender, EventArgs e)
        {
            if (historyBrowserControl.Minimized)
            {
                historyBrowserControl.ExpandedWidth = splitContainer.Width - splitContainer.SplitterDistance +
                                                      splitContainer.SplitterWidth;
                splitContainer.SplitterDistance = splitContainer.Width - HistoryBrowserMinimizedSize - splitContainer.SplitterWidth;
            }
            else
            {
                splitContainer.SplitterDistance = splitContainer.Width - historyBrowserControl.ExpandedWidth +
                                                  splitContainer.SplitterWidth;
            }
        }

        private void historyBrowserControl_CloseButtonClicked(object sender, EventArgs e)
        {
            HistoryBrowserVisible = false;
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer.Panel2.Width > splitContainer.Panel2MinSize &&
                historyBrowserControl.Minimized)
            {
                historyBrowserControl.ShowExpanded();
            }
        }

        private void historyBrowserControl_HistoricalSnapshotSelected(object sender, HistoricalSnapshotSelectedEventArgs e)
        {
            historyBrowserSnapshotChanged = true;
            HistoricalSnapshotDateTime = e.SnapshotDateTime;
            historyBrowserSnapshotChanged = false;
        }

        private void historyBrowserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // A pair of objects is passed in as an argument. This first object in the pair is the DateTime from which to navigate;
            // the second object in the pair indicates if the navigation is to be earlier than the specified DateTime, 
            // otherwise it will be later than the DateTime.

            if (e.Argument is Pair<DateTime, bool>)
            {
                Pair<DateTime, bool> argument = (Pair<DateTime, bool>) e.Argument;

                if (argument.Second)
                {

                    DateTime? result = null;
                    //For analysis tab
                    if (!ApplicationModel.Default.AnalysisHistoryMode)
                        result = RepositoryHelper.GetPreviousServerActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    else
                        result = RepositoryHelper.GetPreviousAnalysisActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    if (result.HasValue)
                    {
                        e.Result = result.Value;
                    }
                    else
                    {
                        e.Result = SnapshotBoundaryCondition.Earliest;
                    }
                }
                else
                {

                    DateTime? result = null;
                    //For analysis tab
                    if (!ApplicationModel.Default.AnalysisHistoryMode)
                        result = RepositoryHelper.GetNextServerActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    else
                        result = RepositoryHelper.GetNextAnalysisActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    if (result.HasValue)
                    {
                        e.Result = result.Value;
                    }
                    else
                    {
                        e.Result = SnapshotBoundaryCondition.MostRecent;
                    }
                }
            }
            else
            {
                throw new DesktopClientException("Invalid argument type passed to historyBrowserBackgroundWorker_DoWork method.");
            }
        }

        private void historyBrowserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result is DateTime)
                {
                    HistoricalSnapshotDateTime = (DateTime) e.Result;
                }
                else if (e.Result is SnapshotBoundaryCondition)
                {
                    switch ((SnapshotBoundaryCondition)e.Result)
                    {
                        case SnapshotBoundaryCondition.Earliest:
                            ApplicationMessageBox.ShowInfo(ParentForm,
                                                           "You are viewing the earliest historical snapshot for this SQL Server instance.");
                            break;
                        case SnapshotBoundaryCondition.MostRecent:
                            if (ApplicationMessageBox.ShowQuestion(
                                    ParentForm, "You are viewing the most recent historical snapshot for this SQL Server instance. Would you like to switch to Real Time Mode?") == DialogResult.Yes)
                            {
                                ApplicationController.Default.SetActiveViewToRealTimeMode();
                            }
                            break;
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm, "An error occurred while retreiving a historical snapshot.", e.Error);
            }
        }
    }

 
    internal enum ServerViewTabs
    {
        Overview,
        Sessions,
        Queries,
        Resources,
        Databases,
        Services,
        Logs,
        Analysis,
        WaitStats,
        Dashboard
    }

    internal enum ServerViews
    {
        Overview,
        OverviewSummary,
        OverviewDetails,
        OverviewConfiguration,
        OverviewAlerts,
        OverviewTimeline,
        Sessions,
        SessionsSummary,
        SessionsDetails,
        SessionsLocks,
        SessionsBlocking,
        Queries,
        QueriesSignatures,
        QueriesStatements,
        QueriesHistory,
        Resources,
        ResourcesSummary,
        ResourcesCpu,
        ResourcesMemory,
        ResourcesDisk,
        ResourcesProcedureCache,
        Databases,
        DatabasesSummary,
        DatabaseAlwaysOn,
        DatabasesConfiguration,
        DatabasesBackupRestoreHistory,
        DatabasesTablesIndexes,
        DatabasesFiles,
        DatabasesMirroring,
        Services,
        ServicesSummary,
        ServicesSqlAgentJobs,
        ServicesFullTextSearch,
        ServicesReplication,
        Logs,
        Deadlock,
        ResourcesWaitStats,
        QueryWaitStatsActive,
        ResourcesFileActivity,
        DatabasesTempdbView,
        ResourcesDiskSize,  // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding the disk size view in Resources
        Analysis,// SQL dm 10.0 UI for doctors recommendation requirement
        RunAnalysis
    }
}
