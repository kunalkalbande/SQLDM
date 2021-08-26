/* Class Representing the view in which the heatmap will be shown together with all the business logic for showing the heatmap - Saurabh*/

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
using System.Linq;
using Idera.SQLdm.DesktopClient.Helpers.HealthIndicesModel;
using System.Drawing;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupHeatMapView : UserControl
    {
        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("ServerGroupHeatMapView");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        //private SqlServerInstanceHeatmap mouseOverHeatmap;
        private SqlServerInstanceHeatmap selectedHeatmap;
        private readonly List<SqlServerInstanceHeatmap> heatmapInstanceList =
            new List<SqlServerInstanceHeatmap>();
        private readonly Dictionary<int, SqlServerInstanceHeatmap> filteredInstanceList =
            new Dictionary<int, SqlServerInstanceHeatmap>();
        private static readonly object updateLock = new object();
        private ServerGroupView parentView;
        private object view;
        private int MAX_HEATMAP_ROWS = 8;
        private int MAX_HEATMAP_COLUMNS = 12;

        public ServerGroupHeatMapView()
        {
            InitializeComponent();
        }

        public void Initialize(ServerGroupView parent, object initView)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            parentView = parent; // for reinitializing heatmap view, once the "All Servers" in navigation pane is clicked.
            view = initView;
            this.Click += new EventHandler(CloseHeatMapOverlayPanel); ;
            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            //ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;

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
            LoadHeatMaps();
            RefreshMainPanelAfterSorting();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupHeatMapView.Initialize : {0}", stopWatch.ElapsedMilliseconds);

        }
        void CloseHeatMapOverlayPanel(object sender, EventArgs e)
        {
            foreach (var panel in ApplicationModel.Default.heatMapOverlayPanels)
            {
                panel.Close();
            }
        }


        private void RefreshMainPanelAfterSorting()
        {
            var instanceListHealthIndex = getSortedbyHealthIndex(heatmapInstanceList);
            var maintenanceModeList = instanceListHealthIndex.FindAll(x => x.IsMaintenanceMode);
            instanceListHealthIndex.RemoveAll(x => x.IsMaintenanceMode);//remove all maintenance mode instances
            instanceListHealthIndex.AddRange(maintenanceModeList);// add to the last
            if (instanceListHealthIndex.Count > 0)
                CreateHeatMapView(instanceListHealthIndex);
            if (instanceListHealthIndex.Count >= 1)
            {
                flowLayoutPanel.Visible = true;
            }

            flowLayoutPanel.ResumeLayout();
        }

        private void CreateHeatMapView(List<SqlServerInstanceHeatmap> instances)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Size size = getEachInstanceSize(instances.Count);
            instances.ForEach(x => x.Size = size);
            flowLayoutPanel.Controls.Clear();
            int m = MAX_HEATMAP_ROWS; //rows
            int n = MAX_HEATMAP_COLUMNS; //columns
            int index = 0;
            bool isBreak = false;
            for (int slice = 0; slice < m + n - 1; ++slice)
            {
                int z1 = slice < n ? 0 : slice - n + 1;
                int z2 = slice < m ? 0 : slice - m + 1;
                for (int j = slice - z1; j >= z2; --j)
                {
                    if (index < instances.Count)
                        flowLayoutPanel.Controls.Add(instances[index++], j, slice - j);
                    else
                    {
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak) break;
            }
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupHeatMapView.CreateHeatMapView : {0}", stopWatch.ElapsedMilliseconds);
        }

        private Size getEachInstanceSize(int noOfInstances)
        {
            Size size = new Size(90, 75);
            int panelWidth = ServerGroupHeatMapView_Fill_Panel.Width;
            int panelHeight = ServerGroupHeatMapView_Fill_Panel.Height;
            int noOfColumns = 7 * ((int)Math.Ceiling(Math.Sqrt(noOfInstances / 28.0)));
            int noOfRows = 4 * ((int)Math.Ceiling(Math.Sqrt(noOfInstances / 28.0)));
            if (noOfColumns > 11 && noOfRows > 7)
            {
                MAX_HEATMAP_COLUMNS = noOfColumns - 1;
                MAX_HEATMAP_ROWS = noOfRows - 1;
                size = new Size((panelWidth / noOfColumns), (panelHeight / noOfRows) - 5);
            }
            else
            {
                MAX_HEATMAP_COLUMNS = 11;
                MAX_HEATMAP_ROWS = 7;
                if (AutoScaleSizeHelper.isScalingRequired)
                {
                    if (AutoScaleSizeHelper.isLargeSize) size = new Size(130, 115);
                    if (AutoScaleSizeHelper.isXLargeSize) size = new Size(150, 135);
                    if (AutoScaleSizeHelper.isXXLargeSize) size = new Size(170, 155);
                }
                else
                {
                    size = new Size(90, 75);
                }
            }
            return size;
        }

        // Working of Search Instances Text Box
        public void OnEnterPress(object sender, KeyPressEventArgs k)
        {
            var text = ((ToolStripTextBox)sender).Text;
            flowLayoutPanel.SuspendLayout();
            if (text == string.Empty)
            {
                RefreshMainPanelAfterSorting();
                return;
            }
            bool instanceFound = false;
            if (heatmapInstanceList != null && heatmapInstanceList.Count > 0)
            {
                foreach (var instance in heatmapInstanceList)
                {
                    var wrapper = instance.Instance;
                    if (string.Compare(wrapper.DisplayInstanceName.ToUpper(), text.ToUpper().Trim()) == 0)
                    {
                        AddSearchedHeatMapOnlyToUI(instance.InstanceId, wrapper.DisplayInstanceName);
                        instanceFound = true;
                        break;
                    }
                }
                if (!instanceFound)
                {
                    flowLayoutPanel.Controls.Clear();
                    flowLayoutPanel.Visible = false;
                    flowLayoutPanel.Controls.Add(NoInstancesLabel);
                    flowLayoutPanel.Visible = true;
                    flowLayoutPanel.ResumeLayout();
                }
            }
        }

        private void AddSearchedHeatMapOnlyToUI(int id, string instanceName)
        {
            SqlServerInstanceHeatmap heatMap = heatmapInstanceList.Find(x => x.InstanceId == id);
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Visible = false;
            flowLayoutPanel.Controls.Add(heatMap, 0, 0);
            flowLayoutPanel.Controls.SetChildIndex(heatMap, GetHeatMapInsertIndex(instanceName));
            flowLayoutPanel.Visible = true;
            flowLayoutPanel.ResumeLayout();

        }


        private void LoadHeatMaps()
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            lock (updateLock)
            {
                heatmapInstanceList.Clear();
                selectedHeatmap = null;
                flowLayoutPanel.SuspendLayout();
                flowLayoutPanel.Controls.Clear();

                if (view == null)
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
                    {
                        AddHeatMap(instance.Id);
                    }
                    stopWatch.Stop();
                    StartUpTimeLog.DebugFormat("Time taken by AddHeatMap() number of monitored sql server times : {0}", stopWatch.ElapsedMilliseconds);
                }
                else if (view is UserView)
                {
                    UserView staticUserView = (UserView)view;
                    staticUserView.Update();

                    foreach (int instanceId in staticUserView.Instances)
                    {
                        AddHeatMap(instanceId);
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
                            AddHeatMap(instanceId);
                        }
                    }
                }
            }
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupHeatMapView.LoadHeatMaps() : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        //private List<SqlServerInstanceHeatmap> getSortedbyHealthIndex(List<SqlServerInstanceHeatmap> heatMapList)
        //{
        //    //performance enhancement
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    try
        //    {
        //        //IList<HealthIndicesModel.ServerSummaryContainer> container = summaryHelper.GetAllInstancesSummary(connectioninfo);

        //        foreach (MonitoredSqlServerWrapper wrapper in ApplicationModel.Default.ActiveInstances)
        //        {
        //            if(heatMapList !=null && heatmapInstanceList.Count > 0)
        //            {
        //                var inst = heatMapList.Find(x => x.InstanceId == wrapper.Instance.Id);
        //                if (inst != null)
        //                {
        //                    inst.TotalAlerts = wrapper.BaseHealthIndex;
        //                    inst.IsMaintenanceMode = ApplicationModel.Default.GetInstanceStatus(inst.InstanceId) != null ?
        //                        ApplicationModel.Default.GetInstanceStatus(inst.InstanceId).IsInMaintenanceMode : false;
        //                }
        //            }
        //        }
        //        if (heatMapList != null)
        //        {
        //            heatMapList.ForEach(instance => instance.calculateHealthIndex());
        //        }
        //    }
        //    catch (Exception e) { }
        //    StartUpTimeLog.DebugFormat("Time taken by ServerGroupHeatMapView.getsorted health index : {0}", stopWatch.ElapsedMilliseconds);
        //    return heatMapList !=null ? heatMapList.OrderBy(x => x.HealthIndex).ToList() : new List<SqlServerInstanceHeatmap>();
        //}


        private List<SqlServerInstanceHeatmap> getSortedbyHealthIndex(List<SqlServerInstanceHeatmap> heatMapList)
        {
            try
            {
                IList<ServerSummaryContainer> baseIndexList = new HealthIndicesHelper().GetAllInstancesSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                if (baseIndexList != null)
                {
                    foreach (ServerSummaryContainer cont in baseIndexList)
                    {
                        if (heatMapList != null && heatmapInstanceList.Count > 0)
                        {
                            var inst = heatMapList.Find(x => x.InstanceId == cont.Overview.SQLServerId);
                            if (inst != null)
                            {
                                inst.TotalAlerts = cont.ServerStatus.BaseHealthIndex;
                                inst.IsMaintenanceMode = ApplicationModel.Default.GetInstanceStatus(cont.Overview.SQLServerId) != null ?
                                    ApplicationModel.Default.GetInstanceStatus(cont.Overview.SQLServerId).IsInMaintenanceMode : false;
                            }
                        }
                    }
                }
                if (heatMapList != null)
                {
                    heatMapList.ForEach(instance => instance.calculateHealthIndex());
                }
            }
            catch (Exception e) { }
            return heatMapList != null ? heatMapList.OrderBy(x => x.HealthIndex).ToList() : new List<SqlServerInstanceHeatmap>();
        }



        private void AddHeatMap(int instanceId)
        {
            lock (updateLock)
            {
                if (heatmapInstanceList != null && !heatmapInstanceList.Any(item => item.InstanceId == instanceId) &&
                    ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                {
                    MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[instanceId];

                    SqlServerInstanceHeatmap heatMap = new SqlServerInstanceHeatmap(instance.Id);
                    heatmapInstanceList.Add(heatMap);

                }
            }
        }

        private int GetHeatMapInsertIndex(string instanceName)
        {
            int index = 0;

            foreach (SqlServerInstanceHeatmap heatMap in flowLayoutPanel.Controls)
            {
                if (string.Compare(instanceName, heatMap.Title) > 0)
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

        //change here as well to Group servers based on status.
        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            ICollection<MonitoredSqlServerWrapper> totalInstances = new List<MonitoredSqlServerWrapper>();
            if (view == null)
            {
                flowLayoutPanel.SuspendLayout();

                switch (e.ChangeType)
                {
                    case KeyedCollectionChangeType.Added:
                        foreach (MonitoredSqlServerWrapper instance in e.Instances)
                        {
                            AddHeatMap(instance.Id);
                        }
                        RefreshMainPanelAfterSorting();
                        ApplicationController.Default.RefreshActiveView();
                        break;
                    case KeyedCollectionChangeType.Removed:
                        foreach (MonitoredSqlServerWrapper instance in e.Instances)
                        {
                            RemoveHeatMap(instance.Id);
                        }
                        RefreshMainPanelAfterSorting();
                        break;
                    case KeyedCollectionChangeType.Cleared:
                        ClearHeatMaps();
                        break;
                    case KeyedCollectionChangeType.Replaced: //added to refresh view on click of "All Servers"
                        foreach (MonitoredSqlServerWrapper instance in e.Instances)
                        {
                            AddHeatMap(instance.Id);
                        }
                        RefreshMainPanelAfterSorting();
                        ApplicationController.Default.RefreshActiveView();
                        break;
                }

                flowLayoutPanel.ResumeLayout();
            }
            else if (view is UserView)
            {
                ((UserView)view).Update();
            }
        }

        private ICollection<MonitoredSqlServerWrapper> getAllInstances(ICollection<MonitoredSqlServerWrapper> newInstances, bool add = false)
        {
            //converting already existing instances into a list
            List<MonitoredSqlServerWrapper> wrapperList = new List<MonitoredSqlServerWrapper>();
            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                wrapperList.Add(instance);
            }
            if (add)
                wrapperList.AddRange(newInstances);
            else
                wrapperList.RemoveAt(0);
            return wrapperList;
        }

        private void UserViewInstances_Changed(object sender, UserViewInstancesChangedEventArgs e)
        {
            flowLayoutPanel.SuspendLayout();

            switch (e.ChangeType)
            {
                case UserViewInstancesChangeType.Added:
                    AddHeatMap(e.InstanceId);
                    RefreshMainPanelAfterSorting();
                    ApplicationController.Default.RefreshActiveView();
                    break;
                case UserViewInstancesChangeType.Removed:
                    RemoveHeatMap(e.InstanceId);
                    RefreshMainPanelAfterSorting();
                    break;
                case UserViewInstancesChangeType.Cleared:
                    ClearHeatMaps();
                    flowLayoutPanel.Visible = false;
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
                        int tagId = (int)view;

                        if (e.Tags.ContainsKey(tagId))
                        {
                            switch (e.ChangeType)
                            {
                                case KeyedCollectionChangeType.Replaced:
                                    SyncHeatMaps(e.Tags[tagId]);
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

        private void SyncHeatMaps(Tag tag)
        {
            lock (updateLock)
            {
                if (tag != null)
                {
                    if (tag.Instances.Count == 0)
                    {
                        ClearHeatMaps();
                    }
                    else
                    {
                        List<int> newInstances = new List<int>();
                        List<int> diffInstances = new List<int>();
                        foreach (var instances in heatmapInstanceList)
                        {
                            diffInstances.Add(instances.InstanceId);
                        }

                        foreach (int instanceId in tag.Instances)
                        {
                            if (heatmapInstanceList.Find(x => x.InstanceId == instanceId) != null)
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
                            RemoveHeatMap(instanceId);
                        }

                        foreach (int instanceId in newInstances)
                        {
                            AddHeatMap(instanceId);
                        }
                        RefreshMainPanelAfterSorting();
                    }
                }
            }
        }



        private void ClearHeatMaps()
        {
            flowLayoutPanel.Controls.Clear();

            foreach (SqlServerInstanceHeatmap heatmap in heatmapInstanceList)
            {
                heatmap.Dispose();
            }

            heatmapInstanceList.Clear();
            flowLayoutPanel.Visible = false;
        }

        private void RemoveHeatMap(int instanceId)
        {
            lock (updateLock)
            {
                SqlServerInstanceHeatmap heatmap;

                if (heatmapInstanceList != null)
                {
                    heatmap = heatmapInstanceList.Find(x => x.InstanceId == instanceId);
                    if (selectedHeatmap == heatmap)
                    {
                        selectedHeatmap = null;
                    }

                    flowLayoutPanel.Controls.Remove(heatmap);
                    heatmapInstanceList.Remove(heatmap);
                    heatmap.Dispose();

                    if (heatmapInstanceList.Count == 0)
                    {
                        flowLayoutPanel.Visible = false;
                    }
                }
            }
        }

        public void UpdateData(Pair<DataTable, DataTable> data)
        {
            foreach (var instance in heatmapInstanceList)
            {
                Pair<DataView, DataView> instanceViews = new Pair<DataView, DataView>();

                instanceViews.First = new DataView(data.First,
                                                   string.Format("SQLServerID = {0}", instance.InstanceId),
                                                   "",
                                                   DataViewRowState.CurrentRows);

                instanceViews.Second = new DataView(data.Second,
                                                    string.Format("SQLServerID = {0}", instance.InstanceId),
                                                    "",
                                                    DataViewRowState.CurrentRows);

                UpdateData(instance.InstanceId, instanceViews);
            }
            RefreshMainPanelAfterSorting();
        }

        public void UpdateData(int instanceId, Pair<DataView, DataView> instanceViews)
        {
            if (heatmapInstanceList != null && heatmapInstanceList.Count > 0)
            {
                var heatMap = heatmapInstanceList.Find(x => x.InstanceId == instanceId);
                if (heatMap != null) heatMap.UpdateData(instanceViews);
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            RefreshMainPanelAfterSorting();
        }

    }
}
