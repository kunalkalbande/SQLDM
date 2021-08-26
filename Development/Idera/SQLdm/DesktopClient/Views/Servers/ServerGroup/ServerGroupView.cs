using System;
using System.Data;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
using System.Diagnostics;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupView : View
    {
        private object view;
        private readonly object updateLock = new object();
        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);


        public ServerGroupView(object view) : this(view, false)
        {

        }

        public ServerGroupView(object view, bool properties)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeComponent();

            Initialize(view, properties);

            // Autoscale font size.
            this.AdaptFontSize();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupView : {0}", stopWatch.ElapsedMilliseconds);
        }


        private void Initialize(object initView, bool properties)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            view = initView;

            if (view == null)
            {
                titleLabel.Text = "All Servers";
                InitializeLogger(titleLabel.Text);
            }
            else if (view is UserView)
            {
                UserView userView = (UserView)view;
                titleLabel.Text = userView.Name;
                InitializeLogger(userView.Name);
                userView.NameChanged += userView_NameChanged;
            }
            else if (view is int)
            {
                int tagId = (int)view;
                if (ApplicationModel.Default.Tags.Contains(tagId))
                {
                    Tag tag = ApplicationModel.Default.Tags[tagId];
                    titleLabel.Text = tag.Name;
                    headerStrip.HeaderImage = Resources.Tag16x16;
                    InitializeLogger(tag.Name);
                    ApplicationModel.Default.Tags.Changed += Tags_Changed;
                }
            }

            thumbnailView.Initialize(this, view);
            detailsView.Initialize(this, view);
            propertiesView.Initialize(this, view);
            if (properties)
            {
                ShowPropertiesView();
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupView.Initialize : {0}", stopWatch.ElapsedMilliseconds);
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
                                    titleLabel.Text = e.Tags[tagId].Name;
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

        //public override void UpdateTheme(ThemeName theme)
        //{
        //    foreach(Control c in this.Controls)
        //    {
        //        if (theme == ThemeName.Dark)
        //        {
        //            if (c.BackColor == System.Drawing.Color.White)
        //                c.BackColor = System.Drawing.Color.FromArgb(2, 16, 23);
        //        }
        //        else
        //        {
        //            if (c.BackColor == System.Drawing.Color.FromArgb(2, 16, 23))
        //                c.BackColor = System.Drawing.Color.White;
        //        }
                
        //    }    
        //}

        public override void ShowHelp()
        {
            if (showPropertiesViewButton.Checked)
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.ServerGroupPropertiesView);
            }
            else
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.ServerGroupView);

            }

        }

        public override void ApplySettings()
        {
            detailsView.ApplySettings();
            propertiesView.ApplySettings();
        }

        public override void SaveSettings()
        {
            detailsView.SaveSettings();
            propertiesView.SaveSettings();
        }

        public object View
        {
            get { return view; }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "showHeatmapButton":
                    ShowHeatmapView();
                    break;
                case "showThumbnailsButton":
                    ShowThumbnailsView();
                    break;
                case "showDetailsButton":
                    ShowDetailsView();
                    break;
                case "showPropertiesButton":
                    ShowPropertiesView();
                    break;
            }
        }

        private void ShowHeatmapView()
        {
            heatmapView.BringToFront();
            showThumbnailsViewButton.Checked = false;
            showDetailsViewButton.Checked = false;
            showPropertiesViewButton.Checked = false;
            ((StateButtonTool)(toolbarsManager.Tools["showHeatmapButton"])).Checked = true;
        }

        private void ShowThumbnailsView()
        {
            thumbnailView.BringToFront();
            showThumbnailsViewButton.Checked = true;
            showDetailsViewButton.Checked = false;
            showPropertiesViewButton.Checked = false;
            ((StateButtonTool)(toolbarsManager.Tools["showThumbnailsButton"])).Checked = true;
        }

        private void ShowDetailsView()
        {
            detailsView.BringToFront();
            showThumbnailsViewButton.Checked = false;
            showDetailsViewButton.Checked = true;
            showPropertiesViewButton.Checked = false;
            ((StateButtonTool)(toolbarsManager.Tools["showDetailsButton"])).Checked = true;
        }

        public void ShowPropertiesView()
        {
            propertiesView.BringToFront();
            showThumbnailsViewButton.Checked = false;
            showDetailsViewButton.Checked = false;
            showPropertiesViewButton.Checked = true;
            ((StateButtonTool)(toolbarsManager.Tools["showPropertiesButton"])).Checked = true;
        }

        private void userView_NameChanged(object sender, EventArgs e)
        {
            UserView userView = sender as UserView;

            if (userView != null)
            {
                titleLabel.Text = userView.Name;
            }
        }

        public override object DoRefreshWork()
        {
            Pair<DataTable, DataTable> data = new Pair<DataTable, DataTable>();
            DateTime historyMarker = DateTime.Now.Subtract(TimeSpan.FromMinutes(60));

            if (view == null)
            {
                foreach (var conn in Settings.Default.RepositoryConnections)
                {
                    System.Data.DataColumn newColumn = new System.Data.DataColumn("RepoId", typeof(System.Int32));
                    newColumn.DefaultValue = conn.RepositiryId;
                    var dt1=RepositoryHelper.GetServerWideStatistics(conn.ConnectionInfo, null);
                    dt1.Columns.Add(newColumn);
                    if (data.First == null) data.First = dt1; else data.First.Merge(dt1);
                    var dt2= RepositoryHelper.GetServerWideStatistics(conn.ConnectionInfo, historyMarker);
                    newColumn = new System.Data.DataColumn("RepoId", typeof(System.Int32));
                    newColumn.DefaultValue = conn.RepositiryId;
                    dt2.Columns.Add(newColumn);
                    if (data.Second == null) data.Second = dt2; else data.Second.Merge(dt2);


                }
            }
            else if (view is UserView)
            {
                UserView userView = view as UserView;
                foreach (var conn in Settings.Default.RepositoryConnections)
                {
                    System.Data.DataColumn newColumn = new System.Data.DataColumn("RepoId", typeof(System.Int32));
                    newColumn.DefaultValue = conn.RepositiryId;
                    var dt1 = RepositoryHelper.GetServerWideStatistics(conn.ConnectionInfo, userView, null);
                    dt1.Columns.Add(newColumn);
                    if (data.First == null) data.First = dt1; else data.First.Merge(dt1);
                    var dt2 = RepositoryHelper.GetServerWideStatistics(conn.ConnectionInfo, userView, historyMarker);
                    newColumn = new System.Data.DataColumn("RepoId", typeof(System.Int32));
                    newColumn.DefaultValue = conn.RepositiryId;
                    dt2.Columns.Add(newColumn);
                    if (data.Second == null) data.Second = dt2; else data.Second.Merge(dt2);


                }
               // data.First = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, userView, null);
               // data.Second = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, userView, historyMarker);
            }
            else if (view is int)
            {
                int tagId = (int)view;

                if (ApplicationModel.Default.Tags.Contains(tagId))
                {
                    Tag tag = ApplicationModel.Default.Tags[tagId];
                    data.First = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, tag, null);
                    data.Second = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, tag, historyMarker);
                }
            }

            return data;
        }

        public override void UpdateData(object data)
        {
            if (data != null && data is Pair<DataTable, DataTable>)
            {
                lock (updateLock)
                {
                    Pair<DataTable, DataTable> dataTable = (Pair<DataTable, DataTable>)data;

                    thumbnailView.UpdateData(dataTable);
                    detailsView.UpdateData(dataTable.Second);
                    propertiesView.UpdateData();

                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
            }
        }

        public void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            thumbnailView.instance_Changed(sender, e);
            detailsView.instance_Changed(sender, e);
            propertiesView.instance_Changed(sender, e);
        }

        private void showThumbnailsViewButton_Click(object sender, EventArgs e)
        {
            ShowThumbnailsView();
        }

        private void showDetailsViewButton_Click(object sender, EventArgs e)
        {
            ShowDetailsView();
        }

        private void showPropertiesViewButton_Click(object sender, EventArgs e)
        {
            ShowPropertiesView();
        }
        private void showHeatmapButton_Click(object sender, EventArgs e)
        {
            ShowHeatmapView();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.IsHandleCreated)
            {   // repost to continue propagation
                this.BeginInvoke((MethodInvoker)delegate { base.OnSizeChanged(e); });
                return;
            }
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
