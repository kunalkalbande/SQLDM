using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.Newsfeed.Plugins.UI;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;
using System.Diagnostics;
using Idera.SQLdm.DesktopClient.Properties;


namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupThumbnailView : UserControl
    {
        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("ServerGroupThumbnailView");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        private SqlServerInstanceThumbnail mouseOverThumbnail;
        private SqlServerInstanceThumbnail selectedThumbnail;
        private readonly Dictionary<int, SqlServerInstanceThumbnail> thumbnails =
            new Dictionary<int, SqlServerInstanceThumbnail>();
        private static readonly object updateLock = new object();
        private ServerGroupView parentView;
        private object view;

        private EventHandler mouseOverChangedHandler;

        public ServerGroupThumbnailView()
        {
            InitializeComponent();
            mouseOverChangedHandler = thumbnail_MouseOverChanged;
            viewStatusLabel.Text = "There are no items to show in this view.";
        }

        public void Initialize(ServerGroupView parent, object initView)
        {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                parentView = parent;
                view = initView;

                ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;
                ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;

                if (view != null)
                {
                    if (view is UserView)
                    {
                        ((UserView)view).InstancesChanged += UserViewInstances_Changed;
                    }
                    else if (view is int)
                    {
                        ApplicationModel.Default.Tags.Changed += Tags_Changed;
                    }
                }
                LoadThumbnails();
                StartUpTimeLog.DebugFormat("Time taken by ServerGroupThumbnailView.Initialize : {0}",stopWatch.ElapsedMilliseconds);
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            //foreach (SqlServerInstanceThumbnail thumbnail in thumbnails.Values)
            //{
            //    //SQLdm 10.2.2--Fix for issue SQLDM-26966-- With every UI refresh updating the swaLaunchButton visibility.
            //    thumbnail.ValidateSWALaunch();
            //    int instanceId = thumbnail.InstanceId;
            //    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId);
            //    if (status != null)
            //    {
            //        thumbnail.UpdateStatus(status);
            //    }
            //}
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "openInstanceButton":
                    OpenSelectedInstance();
                    break;
                case "refreshInstanceButton":
                    RefreshSelectedInstance();
                    break;
                case "deleteInstanceButton":
                    DeleteSelectedInstance();
                    break;
                case "showInstancePropertiesButton":
                    ShowSelectedInstanceProperties();
                    break;
                case "showThumbnailSummaryButton":
                    ShowSelectedThumbnailSummary();
                    break;
                case "showThumbnailResponseTimeButton":
                    ShowSelectedThumbnailChart(ThumbnailChartType.ResponseTime);
                    break;
                case "showThumbnailUserSessionsButton":
                    ShowSelectedThumbnailChart(ThumbnailChartType.UserSessions);
                    break;
                case "showThumbnailCpuUsageButton":
                    ShowSelectedThumbnailChart(ThumbnailChartType.AvgCpuUsage);
                    break;
                case "showThumbnailMemoryUsageButton":
                    ShowSelectedThumbnailChart(ThumbnailChartType.MemoryUsage);
                    break;
                case "showThumbnailSqlReadsWritesButton":
                    ShowSelectedThumbnailChart(ThumbnailChartType.SqlReadsWrites);
                    break;
                case "pulseProfileButton":
                    ViewPulseProfile();
                    break;
                case "pulseFollowButton":
                    TogglePulseFollow();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            toolbarsManager.Tools["deleteInstanceButton"].SharedProps.Visible =
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            toolbarsManager.Tools["deleteInstanceButton"].SharedProps.Visible =
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            if (e.Tool.Key == "instanceContextMenu")
            {
                bool enableRefreshButton = true;

                if (selectedThumbnail != null)
                {
                    MonitoredSqlServerWrapper selectedInstance = selectedThumbnail.Instance;
                    if (selectedInstance != null)
                    {
                        if (selectedInstance.IsRefreshing)
                            enableRefreshButton = false;
                    }
                }
                ButtonTool refreshButton = (ButtonTool)toolbarsManager.Tools["refreshInstanceButton"];
                refreshButton.SharedProps.Enabled = enableRefreshButton;

                // fix the News Feed buttons
                if (ApplicationModel.Default.IsPulseConfigured && selectedThumbnail != null && selectedThumbnail.Instance != null)
                {
                    bool pulseEnabled = false;
                    int pulseId;
                    if (PulseController.Default.GetPulseServerId(selectedThumbnail.Instance.InstanceName, out pulseId))
                    {
                        UpdatePulseFollowButton(pulseId);
                        pulseEnabled = true;
                    }
                    toolbarsManager.Tools["pulseProfileButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["pulseFollowButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["pulseProfileButton"].SharedProps.Enabled = pulseEnabled;
                    toolbarsManager.Tools["pulseFollowButton"].SharedProps.Enabled = pulseEnabled && PulseController.Default.IsLoggedIn;
                }
                else
                {
                    toolbarsManager.Tools["pulseProfileButton"].SharedProps.Visible = false;
                    toolbarsManager.Tools["pulseFollowButton"].SharedProps.Visible = false;
                }
            }
        }

        private void ShowSelectedThumbnailSummary()
        {
            if (selectedThumbnail != null)
            {
                selectedThumbnail.HideChart();
            }
        }

        private void ShowSelectedThumbnailChart(ThumbnailChartType chartType)
        {
            if (selectedThumbnail != null)
            {
                selectedThumbnail.ShowChart(chartType);
            }
        }

        private void OpenSelectedInstance()
        {
            if (selectedThumbnail != null)
            {
                ApplicationController.Default.ShowServerView(selectedThumbnail.InstanceId);
            }
        }

        private void RefreshSelectedInstance()
        {
//            ServerGroupView groupView = ApplicationController.Default.ActiveView as ServerGroupView;
//            if (groupView == null || selectedThumbnail == null)
//                ApplicationController.Default.RefreshActiveView();
//            else
//            {
//                groupView.RefreshInstance(selectedThumbnail.InstanceId);
//            }

              if (selectedThumbnail != null)
              {
                  MonitoredSqlServerWrapper selectedInstance = selectedThumbnail.Instance;
                  if (!selectedInstance.IsRefreshing)
                  {
                      selectedInstance.ForceScheduledRefresh();                      
                  }
              } 
              else
                ApplicationController.Default.RefreshActiveView();
        }

        private void DeleteSelectedInstance()
        {
            if (ApplicationController.Default.ActiveView == parentView &&
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                if (selectedThumbnail != null)
                {
                    DialogResult dialogResult =
                        ApplicationMessageBox.ShowWarning(ParentForm,
                                                          "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                          "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                          "Would you like to retain the data collected for the selected instance(s)?",
                                                          ExceptionMessageBoxButtons.YesNoCancel);

                    if (dialogResult != DialogResult.Cancel)
                    {
                        try
                        {
                            if (dialogResult == DialogResult.Yes)
                            {
                                ApplicationModel.Default.DeactivateMonitoredSqlServers(
                                    new MonitoredSqlServerWrapper[]
                                        {ApplicationModel.Default.ActiveInstances[selectedThumbnail.InstanceId]});
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                ApplicationModel.Default.DeleteMonitoredSqlServers(
                                    new MonitoredSqlServerWrapper[]
                                        {ApplicationModel.Default.ActiveInstances[selectedThumbnail.InstanceId]});
                            }
                        }
                        catch (Exception e)
                        {
                            ApplicationMessageBox.ShowError(ParentForm,
                                                            "An error occurred while removing the selected SQL Server instance.",
                                                            e);
                        }
                    }
                }
            }
        }

        private void ShowSelectedInstanceProperties()
        {
            if (selectedThumbnail != null)
            {
                try
                {
                    MonitoredSqlServerInstancePropertiesDialog dialog =
                        new MonitoredSqlServerInstancePropertiesDialog(selectedThumbnail.InstanceId);
                    dialog.ShowDialog(this);
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(ParentForm, "Unable to show instance properties.", e);
                }
            }
        }

        private void LoadThumbnails()
        {
                Stopwatch stopWatchMain = new Stopwatch();
                stopWatchMain.Start();
                lock (updateLock)
                {
                    thumbnails.Clear();
                    selectedThumbnail = null;
                    flowLayoutPanel.SuspendLayout();
                    flowLayoutPanel.Controls.Clear();

                    if (view == null)
                    {
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
                        {
                            AddThumbnail(instance.Id);
                        }
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken by AddThumbnail() number of monitored sql server times : {0}", stopWatch.ElapsedMilliseconds);
                    }
                    else if (view is UserView)
                    {
                        UserView staticUserView = (UserView)view;
                        staticUserView.Update();

                        foreach (int instanceId in staticUserView.Instances)
                        {
                            AddThumbnail(instanceId);
                        }
                    }
                    else if (view is int)
                    {
                        int tagId = (int)view;

                        if (ApplicationModel.Default.Tags.Contains(tagId))
                        {
                            Tag tag = ApplicationModel.Default.Tags[tagId];

                            foreach (int instanceId in tag.Instances)
                            {
                                AddThumbnail(instanceId);
                            }
                        }
                    }

                    flowLayoutPanel.ResumeLayout();
                }
                stopWatchMain.Stop();
                StartUpTimeLog.DebugFormat("Time taken by ServerGroupThumbnailView.LoadThumbnails : {0}",stopWatchMain.ElapsedMilliseconds);
        }

        private void AddThumbnail(int instanceId)
        {
            lock (updateLock)
            {
                if (!thumbnails.ContainsKey(instanceId) &&
                    ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                {
                    MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[instanceId];
                    instance.Changed += parentView.instance_Changed;

                    SqlServerInstanceThumbnail thumbnail = new SqlServerInstanceThumbnail(instanceId);
                    thumbnail.SelectedChanged += thumbnail_SelectedChanged;
                    thumbnail.MouseDown += thumbnail_MouseDown;
                    thumbnail.MouseOverChanged += mouseOverChangedHandler;
                    thumbnail.ToolTipManager = toolTipManager;
                    thumbnails.Add(instanceId, thumbnail);

                    flowLayoutPanel.Controls.Add(thumbnail);
                    flowLayoutPanel.Controls.SetChildIndex(thumbnail, GetThumbnailInsertIndex(instance.InstanceName));

                    if (thumbnails.Count >= 1)
                    {
                        flowLayoutPanel.Visible = true;
                        shortcutPanel.Visible = true;
                    }
                }
            }
        }

        private int GetThumbnailInsertIndex(string instanceName)
        {
            int index = 0;

            foreach (SqlServerInstanceThumbnail thumbnail in flowLayoutPanel.Controls)
            {
                if (string.Compare(instanceName, thumbnail.Title) > 0)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            if (view == null)
            {
                flowLayoutPanel.SuspendLayout();

                switch (e.ChangeType)
                {
                    case KeyedCollectionChangeType.Added:
                        foreach (MonitoredSqlServerWrapper instance in e.Instances)
                        {
                            AddThumbnail(instance.Id);
                        }
                        ApplicationController.Default.RefreshActiveView();
                        break;
                    case KeyedCollectionChangeType.Removed:
                        foreach (MonitoredSqlServerWrapper instance in e.Instances)
                        {
                            RemoveThumbnail(instance.Id);
                        }
                        break;
                    case KeyedCollectionChangeType.Cleared:
                        ClearThumbnails();
                        flowLayoutPanel.Visible = false;
                        shortcutPanel.Visible = false;
                        break;
                }

                flowLayoutPanel.ResumeLayout();
            }
            else if (view is UserView)
            {
                ((UserView)view).Update();
            }
        }

        private void UserViewInstances_Changed(object sender, UserViewInstancesChangedEventArgs e)
        {
            flowLayoutPanel.SuspendLayout();

            switch (e.ChangeType)
            {
                case UserViewInstancesChangeType.Added:
                    AddThumbnail(e.InstanceId);
                    ApplicationController.Default.RefreshActiveView();
                    break;
                case UserViewInstancesChangeType.Removed:
                    RemoveThumbnail(e.InstanceId);
                    break;
                case UserViewInstancesChangeType.Cleared:
                    ClearThumbnails();
                    flowLayoutPanel.Visible = false;
                    shortcutPanel.Visible = false;
                    break;
            }

            flowLayoutPanel.ResumeLayout();
        }

        private void Tags_Changed(object sender, TagCollectionChangedEventArgs e)
        {
            if (e.ChangeType != KeyedCollectionChangeType.Cleared)
            {
                try
                {
                    if (view is int)
                    {
                        int tagId = (int) view;

                        if (e.Tags.ContainsKey(tagId))
                        {
                            switch (e.ChangeType)
                            {
                                case KeyedCollectionChangeType.Replaced:
                                    SyncThumbnails(e.Tags[tagId]);
                                    ApplicationController.Default.RefreshActiveView(true);
                                    break;
                                case KeyedCollectionChangeType.Removed:
                                    ApplicationModel.Default.Tags.Changed -= Tags_Changed;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("An error occurred while processing tag changes.", ex);
                }
            }
        }

        public void ViewPulseProfile()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    if (selectedThumbnail != null)
                    {
                        int instanceId = selectedThumbnail.InstanceId;
                        if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                        {
                            int pulseId;
                            if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                            {
                                ApplicationController.Default.ShowPulseProfileView(pulseId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error showing Newsfeed profile for server", ex);
            }
        }

        public void TogglePulseFollow()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    if (selectedThumbnail != null)
                    {
                        int instanceId = selectedThumbnail.InstanceId;
                        if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                        {
                            int pulseId;
                            if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                            {
                                ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
                                bool followed = (button.CaptionResolved.StartsWith("Unfollow"));
                                PulseController.Default.SetFollowing(pulseId, !followed);
                                UpdatePulseFollowButton(pulseId);
                                bool followedNew = button.CaptionResolved.StartsWith("Unfollow");
                                if (followed != followedNew)
                                {
                                    if (followedNew)
                                    {
                                        ApplicationMessageBox.ShowInfo(ParentForm, string.Format("You are now following server {0} in Newsfeed.", ApplicationModel.Default.ActiveInstances[instanceId].InstanceName));
                                    }
                                    else
                                    {
                                        ApplicationMessageBox.ShowInfo(ParentForm, string.Format("You are no longer following server {0} in Newsfeed.", ApplicationModel.Default.ActiveInstances[instanceId].InstanceName));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error following server in Newsfeed.", ex);
            }
        }

        private void UpdatePulseFollowButton(int pulseId)
        {
            ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
            bool following = PulseController.Default.GetFollowing(pulseId);
            if (following)
            {
                button.SharedProps.Caption = "Unfollow in Newsfeed";
            }
            else
            {
                button.SharedProps.Caption = "Follow Updates in Newsfeed";
            }
        }

        private void SyncThumbnails(Tag tag)
        {
            lock (updateLock)
            {
                if (tag != null)
                {
                    if (tag.Instances.Count == 0)
                    {
                        ClearThumbnails();
                    }
                    else
                    {
                        List<int> newInstances = new List<int>();
                        List<int> diffInstances = new List<int>(thumbnails.Keys);

                        foreach (int instanceId in tag.Instances)
                        {
                            if (thumbnails.ContainsKey(instanceId))
                            {
                                diffInstances.Remove(instanceId);
                            }
                            else
                            {
                                newInstances.Add(instanceId);
                            }
                        }

                        foreach (int instanceId in diffInstances)
                        {
                            RemoveThumbnail(instanceId);
                        }

                        foreach (int instanceId in newInstances)
                        {
                            AddThumbnail(instanceId);
                        }
                    }
                }
            }
        }

        private void ClearThumbnails()
        {
            flowLayoutPanel.Controls.Clear();

            foreach (SqlServerInstanceThumbnail thumbnail in thumbnails.Values)
            {
                thumbnail.SelectedChanged -= thumbnail_SelectedChanged;
                thumbnail.MouseDown -= thumbnail_MouseDown;
                thumbnail.MouseOverChanged -= mouseOverChangedHandler;
                thumbnail.Dispose();
            }

            thumbnails.Clear();
            flowLayoutPanel.Visible = false;
            shortcutPanel.Visible = false;
        }

        private void RemoveThumbnail(int instanceId)
        {
            lock (updateLock)
            {
                SqlServerInstanceThumbnail thumbnail;

                if (thumbnails.TryGetValue(instanceId, out thumbnail))
                {
                    thumbnail.SelectedChanged -= thumbnail_SelectedChanged;
                    thumbnail.MouseDown -= thumbnail_MouseDown;
                    thumbnail.MouseOverChanged -= mouseOverChangedHandler;

                    if (selectedThumbnail == thumbnail)
                    {
                        selectedThumbnail = null;
                    }

                    flowLayoutPanel.Controls.Remove(thumbnail);
                    thumbnails.Remove(instanceId);
                    thumbnail.Dispose();

                    if (thumbnails.Count == 0)
                    {
                        flowLayoutPanel.Visible = false;
                        shortcutPanel.Visible = false;
                    }
                }
            }
        }

        // We're only supporting single selection
        private void thumbnail_SelectedChanged(object sender, EventArgs e)
        {
            SqlServerInstanceThumbnail thumbnail = sender as SqlServerInstanceThumbnail;

            if (thumbnail != null && thumbnail.Selected)
            {
                if (selectedThumbnail != null)
                {
                    selectedThumbnail.Selected = false;
                }

                selectedThumbnail = thumbnail;
                ApplicationModel.Default.FocusObject =
                    ApplicationModel.Default.ActiveInstances[selectedThumbnail.InstanceId];
            }
        }

        private void thumbnail_MouseOverChanged(object sender, EventArgs e)
        {
            if (mouseOverThumbnail != sender)
            {
                if (mouseOverThumbnail != null)
                    mouseOverThumbnail.MouseOver = false;
                mouseOverThumbnail = sender as SqlServerInstanceThumbnail;
            }
        }

        private void thumbnail_MouseDown(object sender, MouseEventArgs e)
        {
            flowLayoutPanel.Focus();

            if (e.Button == MouseButtons.Right)
            {
                SqlServerInstanceThumbnail thumbnail = sender as SqlServerInstanceThumbnail;

                if (thumbnail != null)
                {
                    if (!thumbnail.ChartVisible)
                    {
                        ((StateButtonTool) (toolbarsManager.Tools["showThumbnailSummaryButton"])).Checked = true;
                    }
                    else
                    {
                        switch (thumbnail.ChartType)
                        {
                            case ThumbnailChartType.ResponseTime:
                                ((StateButtonTool) (toolbarsManager.Tools["showThumbnailResponseTimeButton"])).Checked =
                                    true;
                                break;
                            case ThumbnailChartType.UserSessions:
                                ((StateButtonTool) (toolbarsManager.Tools["showThumbnailUserSessionsButton"])).Checked =
                                    true;
                                break;
                            case ThumbnailChartType.AvgCpuUsage:
                                ((StateButtonTool) (toolbarsManager.Tools["showThumbnailCpuUsageButton"])).Checked =
                                    true;
                                break;
                            case ThumbnailChartType.MemoryUsage:
                                ((StateButtonTool) (toolbarsManager.Tools["showThumbnailMemoryUsageButton"])).Checked =
                                    true;
                                break;
                            case ThumbnailChartType.SqlReadsWrites:
                                ((StateButtonTool) (toolbarsManager.Tools["showThumbnailSqlReadsWritesButton"])).Checked
                                    = true;
                                break;
                        }
                    }

                    toolbarsManager.SetContextMenuUltra(thumbnail, "instanceContextMenu");
                }
            }
        }

        public void UpdateData(int instanceId, Pair<DataView, DataView> instanceViews)
        {
            thumbnails[instanceId].UpdateData(instanceViews);
        }

        public void UpdateData(Pair<DataTable, DataTable> data)
        {
            foreach (int instanceId in thumbnails.Keys)
            {
                Pair<DataView, DataView> instanceViews = new Pair<DataView, DataView>();

                instanceViews.First = new DataView(data.First,
                                                   string.Format("SQLServerID = {0}", instanceId),
                                                   "",
                                                   DataViewRowState.CurrentRows);

                instanceViews.Second = new DataView(data.Second,
                                                    string.Format("SQLServerID = {0}", instanceId),
                                                    "",
                                                    DataViewRowState.CurrentRows);

                UpdateData(instanceId, instanceViews);
            }
        }

        private void flowLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            flowLayoutPanel.Focus();
        }

        private void SelectFirstThumbnailOccurrence(string matchString)
        {
            if (matchString != null && matchString.Length > 0)
            {
                foreach (SqlServerInstanceThumbnail thumbnail in flowLayoutPanel.Controls)
                {
                    if (thumbnail.Title.StartsWith(matchString, StringComparison.OrdinalIgnoreCase))
                    {
                        thumbnail.Selected = true;
                        thumbnail.Focus();
                        break;
                    }
                }
            }
        }

        private void findNumericButton_Click(object sender, EventArgs e)
        {
            foreach (SqlServerInstanceThumbnail thumbnail in flowLayoutPanel.Controls)
            {
                if (Char.IsNumber(thumbnail.Title[0]))
                {
                    thumbnail.Selected = true;
                    thumbnail.Focus();
                    break;
                }
            }
        }

        private void findLetterButton_Click(object sender, EventArgs e)
        {
            UltraButton button = sender as UltraButton;

            if (button != null)
            {
                SelectFirstThumbnailOccurrence(button.Text[0].ToString());
            }
        }

        internal void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            SqlServerInstanceThumbnail thumbnail;
            if (thumbnails.TryGetValue(e.Instance.Id, out thumbnail))
            {
                thumbnail.instance_Changed(sender, e);
            }
        }
    }
}
