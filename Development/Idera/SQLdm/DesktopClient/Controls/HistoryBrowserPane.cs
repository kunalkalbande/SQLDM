using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolTip;
using Infragistics.Win.UltraWinTree;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Objects;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class HistoryBrowserPane : UserControl
    {
        private const int MaxRecentlyViewedSnapshots = 5;

        private bool initialized = false;
        protected int instanceId = -1;
        private Logger logger;
        private readonly Stopwatch loadSnapshotsStopwatch = new Stopwatch();
        private BackgroundWorker loadSnapshotsBackgroundWorker = null;
        private bool recentlyViewedExpanded;
        private readonly UltraToolTipInfo historicalSnapshotsTreeToolTip = new UltraToolTipInfo();
        private readonly UltraToolTipInfo recentlyViewedTreeToolTip = new UltraToolTipInfo();
        private readonly HistoryBrowserOptionsDialog historyOptionsDialog = new HistoryBrowserOptionsDialog();
        //For analysis tab
        private bool analysisHistoryMode = false;

        public bool AnalysisHistoryMode
        {
            get { return analysisHistoryMode; }
            set { analysisHistoryMode = value; }
        }
        //sqldm - 28694 start
      
            //Adding the snapshot to recently viewed list
        public void RecentlyViewed(KeyValuePair<DateTime, MonitoredSqlServerStatus>? snapshotInfo, AppearanceBase appearance)
        {
            if (snapshotInfo != null && appearance != null)
                AddRecentlyViewedSnapshot((KeyValuePair<DateTime, MonitoredSqlServerStatus>)snapshotInfo, appearance);
        }

        //returns historical snapshot of a particular date
        public UltraTreeNode LoadSnapshot(DateTime? dt)
        {
            if (dt != null)
                return DoLoadSnapshotsWork((DateTime)dt);
            else
                return null;
        }

        //sqldm-28694 end
        public event EventHandler<HistoricalSnapshotSelectedEventArgs> HistoricalSnapshotSelected;
        public event EventHandler<EventArgs> HistoricalCustomRangeSelected; //SqlDM 10.2 (Anshul Aggarwal) : New History Browser

        public HistoryBrowserPane()
        {
            InitializeComponent();
            InitializeAppearance();
            SetHistoryBrowserTheme();
            Infragistics.Windows.Themes.ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            toolTipManager.SetUltraToolTip(historicalSnapshotsTree, historicalSnapshotsTreeToolTip);
            toolTipManager.SetUltraToolTip(recentlyViewedTree, recentlyViewedTreeToolTip);
        }

        private void SetHistoryBrowserTheme()
        {
            if (Settings.Default.ColorScheme == "Dark") {
                appearance1.BackColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarBackColor);
                appearance2.BackColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarBackColor);
                this.calendar.BackColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarBackColor);
                this.ultraCalendarLook.MonthHeaderAppearance.ForeColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarForeColor);
                this.ultraCalendarLook.ActiveDayAppearance.BackColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarSelectedDateBackColor);
                this.ultraCalendarLook.DayAppearance.ForeColor = System.Drawing.ColorTranslator.FromHtml(DarkThemeColorConstants.UltraCalendarForeColor);

                this.refreshHistoricalSnapshotsButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Refresh; //need to update
                appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh; //need to update
                this.toggleRecentlyViewedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse; //need to update
            } else {
                appearance1.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (235)))), ((int) (((byte) (235)))), ((int) (((byte) (235)))));
                appearance2.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (235)))), ((int) (((byte) (235)))), ((int) (((byte) (235)))));
                this.calendar.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (235)))), ((int) (((byte) (235)))), ((int) (((byte) (235)))));
                this.ultraCalendarLook.MonthHeaderAppearance.ForeColor = System.Drawing.Color.Black;
                this.ultraCalendarLook.ActiveDayAppearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDFB7");
                this.ultraCalendarLook.DayAppearance.ForeColor = System.Drawing.Color.Black;

                this.refreshHistoricalSnapshotsButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Refresh; 
                appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
                this.toggleRecentlyViewedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse;
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            InitializeAppearance();
            SetHistoryBrowserTheme();
        }

        private void HistoryBrowserPane_Load(object sender, EventArgs e)
        {
            SetRecentlyViewedExpanded(Settings.Default.RecentlyViewedSnapshotsPanelExpanded);
        }

        public void Initialize(int instanceId)
        {
            this.instanceId = instanceId;
            InitializeLogger();
            SelectDate(DateTime.Now.Date);
            autoRefreshTimer.Enabled = true;
        }

        private void SelectDate(DateTime date)
        {
            ultraCalendarInfo.SelectedDateRanges.Clear();
            ultraCalendarInfo.SelectedDateRanges.Add(date);
        }

        private void InitializeAppearance()
        {
            calendar.DrawFilter =
                recentlyViewedTree.DrawFilter =
                    historicalSnapshotsTree.DrawFilter = new HideFocusRectangleDrawFilter();
            treeImages.Images.Clear();
            if (Settings.Default.ColorScheme == "Dark") {
                treeImages.Images.Add(Properties.Resources.Calendar16x16); //need to update

            } else {
                treeImages.Images.Add(Properties.Resources.Calendar16x16);
            }
            
			if (Settings.Default.ColorScheme == "Dark") {
				treeImages.Images.Add(Properties.Resources.LeftLink16x16); //need to update

            } else {
				treeImages.Images.Add(Properties.Resources.LeftLink16x16);
			}

			if (Settings.Default.ColorScheme == "Dark") {
                treeImages.Images.Add(ImageHelper.GetBitmapFromSvgByteArray(Resources.Ok_Dark));

            } else {
                treeImages.Images.Add(Properties.Resources.StatusOKSmall);
            }

            if (Settings.Default.ColorScheme == "Dark") {
                treeImages.Images.Add(ImageHelper.GetBitmapFromSvgByteArray(Resources.Warning_Dark));

            } else {
                treeImages.Images.Add(Properties.Resources.StatusWarningSmall);
            }

            if (Settings.Default.ColorScheme == "Dark") {
                treeImages.Images.Add(ImageHelper.GetBitmapFromSvgByteArray(Resources.Critical_Dark));

            } else {
                treeImages.Images.Add(Properties.Resources.StatusCriticalSmall);
            }

            if (Settings.Default.ColorScheme == "Dark") {
                treeImages.Images.Add(Properties.Resources.darkTheme_StatusUnknown16x16);
            } else {
                treeImages.Images.Add(Properties.Resources.StatusUnknown16x16);
            }
            treeImages.Images.Add(Properties.Resources.Calendar16x16);
            treeImages.Images.Add(Properties.Resources.LeftLink16x16);
            treeImages.Images.Add(Properties.Resources.StatusOKSmall);
            treeImages.Images.Add(Properties.Resources.StatusWarningSmall);
            treeImages.Images.Add(Properties.Resources.StatusCriticalSmall);
            treeImages.Images.Add(Properties.Resources.StatusUnknown16x16);
            //treeImages.Images.Add(Properties.Resources.StatusInfoSmall);

            if (historicalSnapshotsTree.Appearances.Count != 0) {
                historicalSnapshotsTree.Appearances.Remove("SnapshotRoot");
                historicalSnapshotsTree.Appearances.Remove("SnapshotHotTrack");
                historicalSnapshotsTree.Appearances.Remove("SnapshotStatusOK");
                historicalSnapshotsTree.Appearances.Remove("SnapshotStatusWarning");
                historicalSnapshotsTree.Appearances.Remove("SnapshotStatusCritical");
                historicalSnapshotsTree.Appearances.Remove("SnapshotStatusUnknown");
            }


            Infragistics.Win.Appearance appearance;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotRoot");
            appearance.Image = 0;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotHotTrack");
            appearance.Image = 1;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotStatusOK");
            appearance.Image = 2;
            //appearance = historicalSnapshotsTree.Appearances.Add("SnapshotStatusInfo");
            //appearance.Image = 6;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotStatusWarning");
            appearance.Image = 3;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotStatusCritical");
            appearance.Image = 4;
            appearance = historicalSnapshotsTree.Appearances.Add("SnapshotStatusUnknown");
            appearance.Image = 5;

            historicalSnapshotsTree.NodeLevelOverrides[0].NodeAppearance = historicalSnapshotsTree.Appearances["SnapshotRoot"];
            historicalSnapshotsTree.NodeLevelOverrides[1].HotTrackingNodeAppearance = historicalSnapshotsTree.Appearances["SnapshotHotTrack"];

            recentlyViewedTree.NodeLevelOverrides[0].HotTrackingNodeAppearance = historicalSnapshotsTree.Appearances["SnapshotHotTrack"];
        }

        private void InitializeLogger()
        {
            string typeName = GetType().Name;

            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                logger =
                    Logger.GetLogger(
                        string.Format("{0} - {1}", typeName,
                                      ApplicationModel.Default.ActiveInstances[instanceId].InstanceName));
            }
            else
            {
                logger = Logger.GetLogger(typeName);
            }

            logger.Info("Logger initialized.");
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "refreshButton":
                    LoadSnapshotsForCurrentSelection();
                    break;
            }
        }

        private void LoadSnapshots(DateTime selectedDate)
        {
            if (!initialized ||
                (ApplicationController.Default.ActiveView is IServerView &&
                ((IServerView)ApplicationController.Default.ActiveView).InstanceId == instanceId))
            {
                if (loadSnapshotsBackgroundWorker == null)
                {
                    InitializeLoadSnapshotsBackgroundWorker();
                }
                else if (loadSnapshotsBackgroundWorker.IsBusy)
                {
                    CancelLoadSnapshotsBackgroundWorker();
                    InitializeLoadSnapshotsBackgroundWorker();
                }

                logger.InfoFormat("Loading historical snapshots for {0}...", selectedDate.ToShortDateString());
                historicalSnapshotsProgressControl.Active = true;
                historicalSnapshotsProgressControl.Visible = true;
                loadSnapshotsStopwatch.Reset();
                loadSnapshotsStopwatch.Start();
                loadSnapshotsBackgroundWorker.RunWorkerAsync(selectedDate);
                initialized = true;
            }
        }

        private void LoadSnapshotsForCurrentSelection()
        {
            // Only single date selection is supported at this point
            if (ultraCalendarInfo.SelectedDateRanges.Count > 0)
            {
                LoadSnapshots(ultraCalendarInfo.SelectedDateRanges[0].FirstDay.Date);
            }
        }

        private void InitializeLoadSnapshotsBackgroundWorker()
        {
            loadSnapshotsBackgroundWorker = new BackgroundWorker();
            loadSnapshotsBackgroundWorker.WorkerSupportsCancellation = true;
            loadSnapshotsBackgroundWorker.DoWork += loadSnapshotsBackgroundWorker_DoWork;
            loadSnapshotsBackgroundWorker.RunWorkerCompleted += loadSnapshotsBackgroundWorker_RunWorkerCompleted;
        }

        private void CancelLoadSnapshotsBackgroundWorker()
        {
            if (loadSnapshotsBackgroundWorker != null)
            {
                loadSnapshotsBackgroundWorker.CancelAsync();
                loadSnapshotsBackgroundWorker = null;
            }
        }

        private void loadSnapshotsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;

            try
            {
                e.Result = DoLoadSnapshotsWork((DateTime)e.Argument);
            }
            catch
            {
                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private void loadSnapshotsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }
            else if (e.Error != null)
            {
                historicalSnapshotsProgressControl.Visible = false;
                historicalSnapshotsProgressControl.Active = false;
                historicalSnapshotsStatusLabel.Text = "Unable to load historical snapshots.";
                historicalSnapshotsStatusLabel.Visible = true;
                logger.Error("An error occurred while loading historical snapshots.", e.Error);
            }
            else if (!IsDisposed)
            {
                loadSnapshotsStopwatch.Stop();
                logger.Info(string.Format("Load snapshots completed (Duration = {0}).", loadSnapshotsStopwatch.Elapsed));

                Stopwatch dataUpdateStopwatch = new Stopwatch();
                dataUpdateStopwatch.Start();
                logger.Info("Updating snapshots tree...");

                UpdateSnapshotsTree(e.Result as UltraTreeNode);

                dataUpdateStopwatch.Stop();
                logger.Info(string.Format("Snapshots tree updated (Duration = {0}).", dataUpdateStopwatch.Elapsed));
            }
        }

        private void ultraCalendarInfo_AfterSelectedDateRangeChange(object sender, EventArgs e)
        {
            LoadSnapshotsForCurrentSelection();
        }

        private UltraTreeNode DoLoadSnapshotsWork(DateTime selectedDate)
        {
            UltraTreeNode rootNode = null;
            var startEndDates = GetRangeForDate(selectedDate);
            DateTime startRange = startEndDates.First;
            DateTime endRange = startEndDates.Second;
            //Changing history behaviour for analysis tab

            Dictionary<DateTime, MonitoredSqlServerStatus> snapshots = new Dictionary<DateTime, MonitoredSqlServerStatus>();
            if (!ApplicationModel.Default.AnalysisHistoryMode)
            {
                snapshots = RepositoryHelper.GetServerActivityList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                           instanceId, startRange.ToUniversalTime(), endRange.ToUniversalTime());
            }
            else
            {
                snapshots =
                    RepositoryHelper.GetAnalysisActivityList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                           instanceId, startRange.ToUniversalTime(), endRange.ToUniversalTime());
            }

            if (snapshots != null && snapshots.Count > 0)
            {
                // [SQLDM-27443] History Range control_DC: Historical snapshots are not displayed when today's date is selected
                string nodeText = selectedDate.ToString("MMMM dd, yyyy"); 
                rootNode = new UltraTreeNode(null, nodeText);
                rootNode.Expanded = true;
                rootNode.Nodes.Override.SortComparer = new HistoricalSnapshotsTreeSortComparer();
                rootNode.Nodes.Override.Sort = SortType.Descending;

                foreach (KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotEntry in snapshots)
                {
                    rootNode.Nodes.Add(ConstructSnapshotNode(snapshotEntry));
                }
            }

            return rootNode;
        }

        private UltraTreeNode ConstructSnapshotNode(KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotEntry)
        {
            UltraTreeNode snapshotNode = new UltraTreeNode(null, snapshotEntry.Key.ToString("t"));
            snapshotNode.Tag = snapshotEntry;

            if (snapshotEntry.Value != null)
            {
                switch (snapshotEntry.Value.Severity)
                {
                    case MonitoredState.OK:
                        snapshotNode.Text = snapshotNode.Text + " - OK";
                        snapshotNode.Override.NodeAppearance =
                            historicalSnapshotsTree.Appearances["SnapshotStatusOK"];
                        break;
                    case MonitoredState.Informational:
                        snapshotNode.Text = snapshotNode.Text + " - Info";
                        snapshotNode.Override.NodeAppearance =
                            historicalSnapshotsTree.Appearances["SnapshotStatusInfo"];
                        break;
                    case MonitoredState.Warning:
                        snapshotNode.Text = snapshotNode.Text + " - Warning";
                        snapshotNode.Override.NodeAppearance =
                            historicalSnapshotsTree.Appearances["SnapshotStatusWarning"];
                        break;
                    case MonitoredState.Critical:
                        snapshotNode.Text = snapshotNode.Text + " - Critical";
                        snapshotNode.Override.NodeAppearance =
                            historicalSnapshotsTree.Appearances["SnapshotStatusCritical"];
                        break;
                    default:
                        snapshotNode.Text = snapshotNode.Text + " - Unknown";
                        snapshotNode.Override.NodeAppearance =
                            historicalSnapshotsTree.Appearances["SnapshotStatusUnknown"];
                        break;
                }
            }
            else
            {
                snapshotNode.Text = snapshotNode.Text + " - Unknown";
                snapshotNode.Override.NodeAppearance = historicalSnapshotsTree.Appearances["SnapshotStatusUnknown"];
            }

            return snapshotNode;
        }

        private void PurgeExistingSnapshots()
        {
            if (historicalSnapshotsTree.Nodes.Count > 0)
            {
                UltraTreeNode existingSnapshotsNode = historicalSnapshotsTree.Nodes[0];

                while (existingSnapshotsNode.Nodes.Count > 0)
                {
                    UltraTreeNode snapshotNode = existingSnapshotsNode.Nodes[0];
                    existingSnapshotsNode.Nodes.Remove(snapshotNode);
                    snapshotNode.Tag = null;
                    snapshotNode.Override.ResetAppearances();
                    snapshotNode.ResetOverride();
                    snapshotNode.Dispose();
                }

                historicalSnapshotsTree.Nodes.Remove(existingSnapshotsNode);
                existingSnapshotsNode.ResetOverride();
                existingSnapshotsNode.Dispose();
            }
        }

        private Pair<DateTime, DateTime> GetRangeForDate(DateTime dateTime)
        {
            // [SQLDM-27443] History Range control_DC: Historical snapshots are not displayed when today's date is selected
            DateTime startRange = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            DateTime endRange = startRange.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1));
            return new Pair<DateTime, DateTime>(startRange, endRange);
        }

        private void UpdateSnapshotsTree(UltraTreeNode snapshots)
        {
            string saveSelected = null;
            string topNode = null;

            // Save where we are in the tree and the node that was selected
            if ((snapshots != null) && (historicalSnapshotsTree.Nodes.Count > 0) &&
                (historicalSnapshotsTree.Nodes[0].Text == snapshots.Text))
            {
                topNode = historicalSnapshotsTree.TopNode.Text;
                if (historicalSnapshotsTree.SelectedNodes.Count > 0)
                    saveSelected = historicalSnapshotsTree.SelectedNodes[0].Text;
            }

            historicalSnapshotsTree.BeginUpdate();
            PurgeExistingSnapshots();
            
            if (snapshots != null)
            {
                historicalSnapshotsTree.Nodes.Add(snapshots);
                historicalSnapshotsStatusLabel.Visible = false;

                // reselect the same node previously selected
                if (saveSelected != null)
                {
                    for (int i = 0; i < historicalSnapshotsTree.Nodes[0].Nodes.Count; i++)
                    {
                        if (historicalSnapshotsTree.Nodes[0].Nodes[i].Text == saveSelected)
                        {
                            historicalSnapshotsTree.Nodes[0].Nodes[i].Selected = true;
                            break;
                        }
                    }
                }
                // reset position of the Tree
                if (topNode != null)
                {
                    for (int i = 0; i < historicalSnapshotsTree.Nodes[0].Nodes.Count; i++)
                    {
                        if (historicalSnapshotsTree.Nodes[0].Nodes[i].Text == topNode)
                        {
                            historicalSnapshotsTree.Nodes[0].Nodes[i].BringIntoView();
                            if (historicalSnapshotsTree.TopNode.Text != topNode)
                            {
                                historicalSnapshotsTree.TopNode = historicalSnapshotsTree.Nodes[0].Nodes[i];
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                // [SQLDM-27443] History Range control_DC: Historical snapshots are not displayed when today's date is selected
                historicalSnapshotsStatusLabel.Text = "No snapshots available for selected date."; 
                historicalSnapshotsStatusLabel.Visible = true;
            }

            historicalSnapshotsTree.EndUpdate();
            historicalSnapshotsProgressControl.Visible = false;
            historicalSnapshotsProgressControl.Active = false;
        }

        private void historicalSnapshotsTree_AfterSelect(object sender, SelectEventArgs e)
        {
            if (historicalSnapshotsTree.SelectedNodes.Count > 0)
            {
                if (historicalSnapshotsTree.SelectedNodes[0].IsRootLevelNode)
                {
                    historicalSnapshotsTree.SelectedNodes[0].Expanded =
                        !historicalSnapshotsTree.SelectedNodes[0].Expanded;
                }
                else if (historicalSnapshotsTree.SelectedNodes[0].Level == 1)
                {
                    KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotInfo =
                        (KeyValuePair<DateTime, MonitoredSqlServerStatus>)historicalSnapshotsTree.SelectedNodes[0].Tag;
                    AppearanceBase appearance = historicalSnapshotsTree.SelectedNodes[0].Override.NodeAppearance;
                    AddRecentlyViewedSnapshot(snapshotInfo, appearance);

                    if (HistoricalSnapshotSelected != null)
                    {
                        HistoricalSnapshotSelected(this,
                                                   new HistoricalSnapshotSelectedEventArgs(
                                                       snapshotInfo.Key.ToLocalTime()));
                    }
                }
            }
        }

        private void AddRecentlyViewedSnapshot(KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotInfo, AppearanceBase appearance)
        {
            string nodeKey = snapshotInfo.Key.ToLocalTime().ToString("f");

            if (recentlyViewedTree.Nodes.Exists(nodeKey))
            {
                recentlyViewedTree.Nodes[nodeKey].Remove();
            }
            else if (recentlyViewedTree.Nodes.Count == MaxRecentlyViewedSnapshots)
            {
                recentlyViewedTree.Nodes.RemoveAt(recentlyViewedTree.Nodes.Count - 1);
            }

            UltraTreeNode newRecentlyViewedNode = new UltraTreeNode(nodeKey, nodeKey);
            newRecentlyViewedNode.Tag = snapshotInfo;
            if (appearance != null)
            {
                newRecentlyViewedNode.Override.NodeAppearance = appearance;
            }
            recentlyViewedTree.Nodes.Insert(0, newRecentlyViewedNode);
        }

        public void AddRecentlyViewedSnapshot(DateTime snapshotDateTime)
        {
            BackgroundWorker loadRecentlyViewedSnapshotBackgroundWorker = new BackgroundWorker();
            loadRecentlyViewedSnapshotBackgroundWorker.DoWork += loadRecentlyViewedSnapshotBackgroundWorker_DoWork;
            loadRecentlyViewedSnapshotBackgroundWorker.RunWorkerCompleted += loadRecentlyViewedSnapshotBackgroundWorker_RunWorkerCompleted;
            loadRecentlyViewedSnapshotBackgroundWorker.RunWorkerAsync(snapshotDateTime);
        }

        private void loadRecentlyViewedSnapshotBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender != null)
            {
                e.Result =
                    RepositoryHelper.GetStateOverview(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                      instanceId, (DateTime) e.Argument);
            }
        }
        private void loadRecentlyViewedSnapshotBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null && e.Result is Pair<DateTime, MonitoredSqlServerStatus>?)
                {
                    Pair<DateTime, MonitoredSqlServerStatus>? stateOverview = (Pair<DateTime, MonitoredSqlServerStatus>?)e.Result;
                    UltraTreeNode snapshotNode = ConstructSnapshotNode(stateOverview.Value.ToKeyValuePair());
                    AddRecentlyViewedSnapshot((KeyValuePair<DateTime, MonitoredSqlServerStatus>) snapshotNode.Tag,
                                              snapshotNode.Override.NodeAppearance);
                }
                else
                {
                    logger.Info("Unable to load recently viewed snapshot.");
                }
            }
            else
            {
                logger.Error("An error occurred while adding a recently viewed historical snapshot.", e.Error);
            }
        }

        private void recentlyViewedTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (recentlyViewedTree.SelectedNodes.Count > 0 &&
                recentlyViewedTree.SelectedNodes[0].IsRootLevelNode)
            {
                KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotInfo =
                    (KeyValuePair<DateTime, MonitoredSqlServerStatus>) recentlyViewedTree.SelectedNodes[0].Tag;

                if (HistoricalSnapshotSelected != null)
                {
                    HistoricalSnapshotSelected(this,
                                               new HistoricalSnapshotSelectedEventArgs(snapshotInfo.Key.ToLocalTime()));
                }
            }
        }

        private void recentlyViewedHeaderStrip_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Settings.Default.RecentlyViewedSnapshotsPanelExpanded = !recentlyViewedExpanded;
                SetRecentlyViewedExpanded(Settings.Default.RecentlyViewedSnapshotsPanelExpanded);
            }
        }

        private void SetRecentlyViewedExpanded(bool expanded)
        {
            if (expanded)
            {

                toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallCollapse; //Babita Manral
                //if (Settings.Default.ColorScheme == "Dark") {
               //     toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallCollapse; //need to update
             //   } else {
             //       toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallCollapse;
             //   }
            //   toggleRecentlyViewedButton.Image = ImageHelper.GetBitmapFromSvgByteArray(Properties.Resources.darkTheme_Contract); //Babita Manral
                recentlyViewedTreeContainerPanel.Visible = true;
                recentlyViewedPanel.Height = 126;
            }
            else
            {

                toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallExpand; //Babita Manral
               // if (Settings.Default.ColorScheme == "Dark") {
                  //  toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallExpand; //need to update
              //  } else {
               //     toggleRecentlyViewedButton.Image = Properties.Resources.HeaderStripSmallExpand;
             //   }
             //   toggleRecentlyViewedButton.Image = ImageHelper.GetBitmapFromSvgByteArray(Properties.Resources.darkTheme_Expand); //Babita Manral
                recentlyViewedTreeContainerPanel.Visible = false;
                recentlyViewedPanel.Height = 20;
            }

            recentlyViewedExpanded = expanded;
        }

        private void historicalSnapshotsTree_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is TreeNodeUIElement)
            {
                bool toolTipAvailable = false;
                TreeNodeUIElement nodeElement = e.Element as TreeNodeUIElement;

                if (nodeElement != null && nodeElement.Node.Level == 1)
                {
                    KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotInfo =
                        (KeyValuePair<DateTime, MonitoredSqlServerStatus>) nodeElement.Node.Tag;

                    if (snapshotInfo.Value != null)
                    {
                        historicalSnapshotsTreeToolTip.ToolTipTitleAppearance.Image =
                            snapshotInfo.Value.ToolTipHeadingImage;
                        historicalSnapshotsTreeToolTip.ToolTipTitle = snapshotInfo.Value.ToolTipHeading;
                        historicalSnapshotsTreeToolTip.ToolTipText = snapshotInfo.Value.ToolTip;
                        toolTipAvailable = true;
                    }
                }

                historicalSnapshotsTreeToolTip.Enabled = toolTipAvailable
                                                         ? DefaultableBoolean.True
                                                         : DefaultableBoolean.False;
            }
        }

        private void historicalSnapshotsTree_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is TreeNodeUIElement)
            {
                toolTipManager.HideToolTip();
            }
        }

        private void recentlyViewedTree_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is TreeNodeUIElement)
            {
                bool toolTipAvailable = false;
                TreeNodeUIElement nodeElement = e.Element as TreeNodeUIElement;

                if (nodeElement != null && nodeElement.Node.Level == 0)
                {
                    KeyValuePair<DateTime, MonitoredSqlServerStatus> snapshotInfo =
                        (KeyValuePair<DateTime, MonitoredSqlServerStatus>)nodeElement.Node.Tag;

                    if (snapshotInfo.Value != null)
                    {
                        recentlyViewedTreeToolTip.ToolTipTitleAppearance.Image =
                            snapshotInfo.Value.ToolTipHeadingImage;
                        recentlyViewedTreeToolTip.ToolTipTitle = snapshotInfo.Value.ToolTipHeading;
                        recentlyViewedTreeToolTip.ToolTipText = snapshotInfo.Value.ToolTip;
                        toolTipAvailable = true;
                    }
                }

                recentlyViewedTreeToolTip.Enabled = toolTipAvailable
                                                        ? DefaultableBoolean.True
                                                        : DefaultableBoolean.False;
            }
        }

        private void recentlyViewedTree_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            if (e.Element is TreeNodeUIElement)
            {
                toolTipManager.HideToolTip();
            }
        }

        private void filterOptionsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ParentForm is ControlContainerDialog)
            {
                historyOptionsDialog.StartPosition = FormStartPosition.CenterScreen;
                ((ControlContainerDialog) ParentForm).HideOnDeactivate = false;
            }
            
            historyOptionsDialog.RefreshDialog();
            bool isOk = historyOptionsDialog.ShowDialog() == DialogResult.OK;
            if (ParentForm is ControlContainerDialog)
            {
                ((ControlContainerDialog)ParentForm).HideOnDeactivate = true;
            }

            if (isOk)
            {
                var handler = HistoricalCustomRangeSelected;
                if (handler != null)
                    HistoricalCustomRangeSelected(this, EventArgs.Empty);
            }
        }
        
        private class HistoricalSnapshotsTreeSortComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                UltraTreeNode xNode = x as UltraTreeNode;
                UltraTreeNode yNode = y as UltraTreeNode;

                if (xNode != null && yNode != null)
                {
                    KeyValuePair<DateTime, MonitoredSqlServerStatus> xTag =
                        (KeyValuePair<DateTime, MonitoredSqlServerStatus>) xNode.Tag;

                    KeyValuePair<DateTime, MonitoredSqlServerStatus> yTag =
                        (KeyValuePair<DateTime, MonitoredSqlServerStatus>)yNode.Tag;

                    return DateTime.Compare(xTag.Key, yTag.Key);
                }
                else
                {
                    return 0;
                }
            }
        }

        private void refreshHistoricalSnapshotsButton_Click(object sender, EventArgs e)
        {
            LoadSnapshotsForCurrentSelection();
        }

        public void ClearSnapshotSelection()
        {
            recentlyViewedTree.SelectedNodes.Clear();
            historicalSnapshotsTree.SelectedNodes.Clear();
        }

        private void autoRefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshTimer();
        }
        public void RefreshTimer()
        {
            try
            {
                DateTime now = DateTime.Now;
                if (ultraCalendarInfo.SelectedDateRanges.Count > 0)
                {
                    DateTime selectedDate = ultraCalendarInfo.SelectedDateRanges[0].FirstDay.Date;
                    var startEndDates = GetRangeForDate(selectedDate);
                    DateTime startRange = startEndDates.First;
                    DateTime endRange = startEndDates.Second;

                    if ((now >= startRange && now <= endRange) || analysisHistoryMode != ApplicationModel.Default.AnalysisHistoryMode)
                    {
                        analysisHistoryMode = ApplicationModel.Default.AnalysisHistoryMode;
                        LoadSnapshotsForCurrentSelection();
                    }
                }
            }catch(Exception e)
            {
                //Handle Exception
            }
        }
    }

    public sealed class HistoricalSnapshotSelectedEventArgs : EventArgs
    {
        public readonly DateTime SnapshotDateTime;

        public HistoricalSnapshotSelectedEventArgs(DateTime snapshotDateTime)
        {
            SnapshotDateTime = snapshotDateTime;
        }
    }
}
