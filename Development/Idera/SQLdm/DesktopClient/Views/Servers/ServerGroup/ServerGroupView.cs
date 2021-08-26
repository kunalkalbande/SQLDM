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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Infragistics.Windows.Themes;
using System.Drawing;


namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupView : View
    {
        /*private const int EM_SETCUEBANNER = 0x1501;*/ // to set the cue text in search text box, Saurabh UX-DM
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

            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            this.searchInstances.ForeColor = string.Compare(Settings.Default.ColorScheme, "Dark", true)==0 ? Color.White : Color.Black;
            this.searchInstances.BackColor = string.Compare(Settings.Default.ColorScheme, "Dark", true) == 0 ? Color.Black : Color.White;
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ServerGroupView : {0}", stopWatch.ElapsedMilliseconds);
        }

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);


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
            //performance enhancement
            thumbnailView.Initialize(this, view);
            detailsView.Initialize(this, view);
            propertiesView.Initialize(this, view);
            heatmapView.Initialize(this, view);
            
            //detailsView.Initialize(this, view);
            //propertiesView.Initialize(this, view);
            //heatmapView.Initialize(this, view); //Saurabh UX-DM
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
                case "showHeatmapButton": //Saurabh UX-DM
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
            showHeatMapViewButton.Checked = true;//Saurabh UX-DM
            searchInstances.Visible = true;//Saurabh UX-DM
            ((StateButtonTool)(toolbarsManager.Tools["showHeatmapButton"])).Checked = true;
        }

        //Saurabh UX-DM
        private void UpdateHeatMapInstancesOnSearch(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Return)
                heatmapView.OnEnterPress(sender, e);
        }
        //Saurabh UX-DM
        private void focusGained(object sender, EventArgs e) 
        {
            searchInstances.Text = "";
        }
        //Saurabh UX-DM
        private void focusLost(object sender, EventArgs e)
        {
            searchInstances.Text = "Search Instances..";
        }

        private void ShowThumbnailsView()
        {
            thumbnailView.BringToFront();
            showThumbnailsViewButton.Checked = true;
            showDetailsViewButton.Checked = false;
            showPropertiesViewButton.Checked = false;
            showHeatMapViewButton.Checked = false; //Saurabh UX-DM
            searchInstances.Visible = false; //saurabh UX-DM
            //((StateButtonTool)(toolbarsManager.Tools["showThumbnailsButton"])).Checked = true;
        }

        private void ShowDetailsView()
        {
            detailsView.BringToFront();
            showThumbnailsViewButton.Checked = false;
            showDetailsViewButton.Checked = true;
            showPropertiesViewButton.Checked = false;
            showHeatMapViewButton.Checked = false; //Saurabh UX-DM
            searchInstances.Visible = false; //Saurabh UX-DM
            ((StateButtonTool)(toolbarsManager.Tools["showDetailsButton"])).Checked = true;
        }

        public void ShowPropertiesView()
        {
            propertiesView.BringToFront();
            showThumbnailsViewButton.Checked = false;
            showDetailsViewButton.Checked = false;
            showPropertiesViewButton.Checked = true;
            showHeatMapViewButton.Checked = false; //Saurabh UX-DM
            searchInstances.Visible = false; //Saurabh UX-DM
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
                data.First = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, null);
                data.Second = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, historyMarker);
            }
            else if (view is UserView)
            {
                UserView userView = view as UserView;
                data.First = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, userView, null);
                data.Second = RepositoryHelper.GetServerWideStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, userView, historyMarker);
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
                    heatmapView.UpdateData(dataTable);
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

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        protected void ScaleControlsAsPerResolution()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.showHeatMapViewButton.Image = Properties.Resources.ServerGroupHeatMapView32x32;
                this.showThumbnailsViewButton.Image = Properties.Resources.larger_wht_server_group_thumbnail_view;
                this.showDetailsViewButton.Image = Properties.Resources.larger_wht_list_view;
                this.showPropertiesViewButton.Image = Properties.Resources.larger_wht_properties;
                this.headerStrip.Font = new Font("Arial", 15F, System.Drawing.FontStyle.Bold);
                this.headerStrip.Size = new Size(this.headerStrip.Width, this.headerStrip.Height + 10);
            }
            else
            {
                this.showHeatMapViewButton.Image = Properties.Resources.ServerGroupHeatMapView32x32;
                this.showThumbnailsViewButton.Image = Properties.Resources.ServerGroupThumbnailView32x32;
                this.showDetailsViewButton.Image = Properties.Resources.ServerGroupDetailsView32x32;
                this.showPropertiesViewButton.Image = Properties.Resources.Properties32x32;
                //this.headerStrip.HeaderImage = Properties.Resources.NavigationPaneServer32x32;
                this.headerStrip.Font = new Font("Arial", 15F, System.Drawing.FontStyle.Bold);
                this.headerStrip.Size = new Size(this.headerStrip.Width, this.headerStrip.Height + 10);
            }
            if (AutoScaleSizeHelper.isLargeSize)
            {
                this.headerStrip.ImageScalingSize = new System.Drawing.Size(30, 30);
                this.searchInstances.Size = new System.Drawing.Size(220, 40);
                this.titleLabel.Margin = new Padding(20, 0, 0, 0);
                this.headerStrip.Padding = new Padding(20, 0, 20, 0);
                this.showPropertiesViewButton.Margin = new Padding(10, 0, 7, 0);
                this.showDetailsViewButton.Padding = new Padding(7, 0, 7, 0);
                this.showThumbnailsViewButton.Padding = new Padding(7, 0, 7, 0);
                this.showHeatMapViewButton.Padding = new Padding(7, 0, 7, 0);
                this.searchInstances.Padding = new Padding(7, 0, 7, 0);
                this.headerStrip.HeaderImage = Settings.Default.ColorScheme == "Dark" ? Properties.Resources.larger_navigation_pane_servers : Properties.Resources.NavigationPaneServer32x32; ;
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                this.headerStrip.ImageScalingSize = new System.Drawing.Size(33, 30);
                this.searchInstances.Size = new System.Drawing.Size(270, 50);
                this.titleLabel.Margin = new Padding(20, 0, 0, 0);
                this.headerStrip.Padding = new Padding(20, 0, 20, 0);
                this.showPropertiesViewButton.Margin = new Padding(7, 0, 7, 0);
                this.showDetailsViewButton.Padding = new Padding(7, 0, 7, 0);
                this.showThumbnailsViewButton.Padding = new Padding(7, 0, 7, 0);
                this.showHeatMapViewButton.Padding = new Padding(7, 0, 7, 0);
                this.searchInstances.Padding = new Padding(7, 0, 7, 0);
                this.headerStrip.HeaderImage = Settings.Default.ColorScheme == "Dark" ? Properties.Resources.larger_navigation_pane_servers : Properties.Resources.NavigationPaneServer32x32; ;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                this.headerStrip.ImageScalingSize = new System.Drawing.Size(38, 35);
                this.searchInstances.Size = new System.Drawing.Size(300, 60);
                this.titleLabel.Margin = new Padding(20, 0, 0, 0);
                this.headerStrip.Padding = new Padding(20, 0, 20, 0);
                this.showPropertiesViewButton.Margin = new Padding(10, 0, 10, 0);
                this.showDetailsViewButton.Padding = new Padding(10, 0, 10, 0);
                this.showThumbnailsViewButton.Padding = new Padding(10, 0, 10, 0);
                this.showHeatMapViewButton.Padding = new Padding(10, 0, 10, 0);
                this.searchInstances.Padding = new Padding(7, 0, 7, 0);
                this.headerStrip.HeaderImage = Settings.Default.ColorScheme == "Dark" ? Properties.Resources.larger_navigation_pane_servers : Properties.Resources.NavigationPaneServer32x32; ;
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.searchInstances != null)
                {
                    this.searchInstances.ForeColor = string.Compare(Settings.Default.ColorScheme, "DARK", true) == 0 ? System.Drawing.Color.White : System.Drawing.Color.Black;
                    this.searchInstances.BackColor = string.Compare(Settings.Default.ColorScheme, "DARK", true) == 0 ? System.Drawing.Color.Black : System.Drawing.Color.White;
                    //START:DarkTheme 4.12 Icons Switching : Babita Manral
                    if (Settings.Default.ColorScheme == "Dark")
                    {
                        this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneServersSmall;                        
                        this.showPropertiesViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;                        
                        this.showDetailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupDetailsView;
                        this.showThumbnailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupThumbnailView;
                    }
                    else
                    {
                        this.headerStrip.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.NavigationPaneServersSmall;
                        this.showPropertiesViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
                        this.showDetailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupDetailsView;
                        this.showThumbnailsViewButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ServerGroupThumbnailView;
                        this.headerStrip.BackColor = Color.Transparent;
                    }
                    //END:DarkTheme 4.12 Icons Switching : Babita Manral
                }
                if(this.borderPanel != null)
                    this.borderPanel.Padding = string.Compare(Settings.Default.ColorScheme, "DARK", true) == 0 ? new System.Windows.Forms.Padding(0, 0, 0, 0) : new Padding(1);
                if (AutoScaleSizeHelper.isScalingRequired)
                    ScaleControlsAsPerResolution();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //Handle Exception
            }
        }
    }
}
