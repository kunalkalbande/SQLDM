using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.MessageBox;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerSummaryView4 : ServerDesignBaseView
    {
        private const string UnableToUpdateDataText = @"Unable to update data for this view. Click here to view details.";
        private const string OperationalStatusMessage = "       {0}";

        private const int DashboardPanelMinimumWidth = 512;
        private const int DashboardPanelMinimumHeight = 132;
        int col = 0;

        #region Help Popup Members

        private const string OsMetricsUnavailableHelp = HelpTopics.EnablingOsMetricsMonitoring;
        private const string LightweightPoolingHelp = HelpTopics.EnablingOsMetricsMonitoring;

        #endregion

        private bool customizeBaselineConfigurationMessageShown = false;
        private DateTime? earliestAvailableData;
        private bool enableOleAutomation = false;
        private bool displayWMIErrorDialog = true;
        private bool displayWMITimedoutDialog = true;
        private bool historyBannerShowing = false;
        private readonly ServerSummaryHistoryData historyData;
        private ServerOverview activeSnapshot;
        private Exception lastRefreshError = null;
        private static readonly object updateLock = new object();
        private Exception historyModeLoadError = null;

        private MetricDefinitions metricDefinitions;
        private List<int> configuredCustomCounters = new List<int>();
        private CustomCounterCollectionSnapshot previousCounterSnapshot;

        private ServerVersion _serverVersion;

        private int dashboardLayoutID;
        private DashboardConfiguration dashboardConfiguration;
        private List<DashboardControl> dashboardControls;
        private string userName;
        ViewContainer vHost = null;
        public event EventHandler PanelGalleryVisibleChanged;

        ThemeSetter themeSetter = new ThemeSetter();
        public ServerSummaryView4(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            this.historyData = historyData;
            RefreshReportsProgress = true;

            InitializeComponent();
            InitializeDashboard();

            // Set up autoscale font size.
            AdaptFontSize();

            viewStatusLabel.Text = String.Empty;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            metricDefinitions = ApplicationModel.Default.MetricDefinitions;

            SetAboveContentAlertTheme();

            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            ScaleControlAsPerResolution();
        }

        public ServerSummaryView4(int instanceId, ServerSummaryHistoryData historyData, ViewContainer vHost)
           : base(instanceId)
        {
            this.historyData = historyData;
            this.vHost = vHost;
            RefreshReportsProgress = true;

            InitializeComponent();

            InitializeDashboard(this.vHost);

            // Set up autoscale font size.
            AdaptFontSize();

            viewStatusLabel.Text = String.Empty;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            metricDefinitions = ApplicationModel.Default.MetricDefinitions;

            SetAboveContentAlertTheme();

            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            ScaleControlAsPerResolution();
        }

        #region Properties

        public event EventHandler HistoricalSnapshotDateTimeChanged;

        /// <summary>
        /// Start datetime for custom range, otherwise null.
        /// </summary>
        public override DateTime? HistoricalStartDateTime
        {
            get { return historyData.HistoricalStartDateTime; }
            set
            {
                if (historyData.HistoricalStartDateTime != value)
                {
                    historyData.HistoricalStartDateTime = value;
                    currentHistoricalStartDateTime = null;
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historyData.HistoricalSnapshotDateTime; }
            set
            {
                if (historyData.HistoricalSnapshotDateTime != value)
                {
                    historyData.HistoricalSnapshotDateTime = value;
                    currentHistoricalSnapshotDateTime = null;

                    if (HistoricalSnapshotDateTimeChanged != null)
                    {
                        HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool PanelGalleryVisible
        {
            get { return panelGalleryPanel.Visible; }
            set
            {
                panelGalleryPanel.Visible = value;
                if (value)
                    panelGalleryPanel.BringToFront();
                else
                    dashboardPanelDetails.Visible = false;

                if (PanelGalleryVisibleChanged != null)
                {
                    PanelGalleryVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Initialization

        private void InitializeDashboard()
        {
            using (Log.VerboseCall("InitializeDashboard"))
            {
                if (dashboardConfiguration == null)
                {
                    // don't let this fail since it is called from the constructor
                    try
                    {
                        SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                        userName = RepositoryHelper.GetRepositoryUser(connectionInfo);

                        dashboardConfiguration =
                            RepositoryHelper.GetDashboardLayout(connectionInfo,
                                                                userName, instanceId, out dashboardLayoutID);
                        RepositoryHelper.UpdateDashboardLayoutViewed(connectionInfo, dashboardLayoutID);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error retrieving dashboard layout from repository. Using internal default instead.",
                                  ex);
                        userName = "Unknown";
                        dashboardLayoutID = 0;
                        dashboardConfiguration = DashboardHelper.GetDefaultConfig(new ServerVersion("9.00.00.00"), instanceId);
                    }
                }

                dashboardTableLayoutPanel.SuspendLayout();

                // Need to remove controls here instead of clear to unhook events when changing layouts
                dashboardTableLayoutPanel.Controls.Clear();
                dashboardTableLayoutPanel.RowStyles.Clear();
                dashboardTableLayoutPanel.ColumnStyles.Clear();

                dashboardTableLayoutPanel.RowCount = dashboardConfiguration.Rows;
                float size = 100f / dashboardConfiguration.Rows;
                for (int rows = 0; rows <= dashboardConfiguration.Rows; rows++)
                {
                    dashboardTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, size));
                }

                dashboardTableLayoutPanel.ColumnCount = dashboardConfiguration.Columns;
                size = 100f / dashboardConfiguration.Columns;
                for (int columns = 1; columns <= dashboardConfiguration.Columns; columns++)
                {
                    dashboardTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, size));
                }

                dashboardControls = new List<DashboardControl>();
                
                foreach (DashboardPanelConfiguration panelConfig in dashboardConfiguration.Panels)
                {
                    AddPanel(panelConfig);
                }
                FillLastRowWithEmptyPanels();
                dashboardTableLayoutPanel.MinimumSize = new Size(DashboardPanelMinimumWidth * dashboardConfiguration.Columns, DashboardPanelMinimumHeight * dashboardConfiguration.Rows + 200);

                dashboardTableLayoutPanel.ResumeLayout();

                // recalculate sizes for display
                ShowOperationalStatusPanel(operationalStatusPanel.Visible);
            }
        }

        private void InitializeDashboard(ViewContainer vHost)
        {
            using (Log.VerboseCall("InitializeDashboard"))
            {
                if (dashboardConfiguration == null)
                {
                    // don't let this fail since it is called from the constructor
                    try
                    {
                        SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                        userName = RepositoryHelper.GetRepositoryUser(connectionInfo);

                        dashboardConfiguration =
                            RepositoryHelper.GetDashboardLayout(connectionInfo,
                                                                userName, instanceId, out dashboardLayoutID);
                        RepositoryHelper.UpdateDashboardLayoutViewed(connectionInfo, dashboardLayoutID);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error retrieving dashboard layout from repository. Using internal default instead.",
                                  ex);
                        userName = "Unknown";
                        dashboardLayoutID = 0;
                        dashboardConfiguration = DashboardHelper.GetDefaultConfig(new ServerVersion("9.00.00.00"), instanceId);
                    }
                }

                dashboardTableLayoutPanel.SuspendLayout();

                // Need to remove controls here instead of clear to unhook events when changing layouts
                dashboardTableLayoutPanel.Controls.Clear();
                dashboardTableLayoutPanel.RowStyles.Clear();
                dashboardTableLayoutPanel.ColumnStyles.Clear();
                float size = 0.0f;
                dashboardTableLayoutPanel.RowCount = dashboardConfiguration.Rows;
                size = 100f / dashboardConfiguration.Rows;

                for (int rows = 0; rows <= dashboardConfiguration.Rows; rows++)
                {
                    dashboardTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, size));
                }
                dashboardTableLayoutPanel.ColumnCount = dashboardConfiguration.Columns;
                size = 100f / dashboardConfiguration.Columns;

                for (int columns = 1; columns <= dashboardConfiguration.Columns; columns++)
                {
                    dashboardTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, size));
                }
                dashboardControls = new List<DashboardControl>();

                foreach (DashboardPanelConfiguration panelConfig in dashboardConfiguration.Panels)
                {
                    AddPanel(panelConfig);
                }
                FillLastRowWithEmptyPanels();
                 dashboardTableLayoutPanel.MinimumSize = new Size(DashboardPanelMinimumWidth * dashboardConfiguration.Columns, DashboardPanelMinimumHeight * dashboardConfiguration.Rows + 200); 
                 dashboardTableLayoutPanel.ResumeLayout();

                // recalculate sizes for display
                ShowOperationalStatusPanel(operationalStatusPanel.Visible);
            }
        }

        private void FillLastRowWithEmptyPanels()
        {
            for (int row = 0; row < this.dashboardTableLayoutPanel.RowCount; row++)
            {
                for (int column = 0; column < this.dashboardTableLayoutPanel.ColumnCount; column++)
                {
                    Control c = this.dashboardTableLayoutPanel.GetControlFromPosition(column, row);

                    if (c == null)
                    {
                        AddPanel(new DashboardPanelConfiguration(row, column, DashboardPanel.Empty));
                    }
                }
            }
        }

        public override void SetArgument(object argument)
        {
            if (argument is int)
            {
                int id = (int)argument;

                // if nothing was selected it will return 0 and can be ignored
                if (id == 0)
                    return;

                try
                {
                    SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                    DashboardConfiguration config = RepositoryHelper.GetDashboardLayout(connectionInfo, id);
                    if (config != null)
                    {
                        dashboardLayoutID = id;
                        dashboardConfiguration = config;
                        RepositoryHelper.UpdateDashboardLayoutViewed(connectionInfo, dashboardLayoutID);
                        InitializeDashboard(this.vHost);
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, "The selected dashboard layout was not found in the repository.");
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to retrieve the selected dashboard layout from the repository.", ex);
                }
            }
        }

        public override void ApplySettings()
        {
            foreach (var dashboardControl in dashboardControls)
            {
                dashboardControl.ApplySettings();
            }
        }

        public override void SaveSettings()
        {
            foreach (var dashboardControl in dashboardControls)
            {
                dashboardControl.SaveSettings();
            }
        }

        private void ShowVMData(bool showVM)
        {
            Log.DebugFormat("Show VM : {0}", showVM);

            foreach (var dashboardControl in dashboardControls)
            {
                dashboardControl.ShowVMData(showVM);
            }
        }

        #endregion

        #region Event Handlers

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateRealTimeChartDataFilter();
                    break;
            }
        }


        private void viewStatusLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lastRefreshError != null)
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while refreshing the Server Summary view.",
                                                lastRefreshError, false);
            }
            else if (activeSnapshot != null &&
                     activeSnapshot.SqlServiceStatus.HasValue)
            {
                switch (activeSnapshot.SqlServiceStatus.Value)
                {
                    case ServiceState.Paused:
                        ApplicationMessageBox.ShowError(ParentForm,
                                                       "The SQL Server service is paused. The SQL Server service must be running in order to collect statistics.",
                                                       null, false);
                        break;
                    default:
                        ApplicationMessageBox.ShowError(ParentForm,
                                                       "The SQL Server service is not available. The SQL Server service must be running in order to collect statistics.",
                                                       null, false);
                        break;
                }
            }
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
                operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
            }          
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
                operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
            }            
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.White;
                operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
                operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
            }           

            if (HistoricalSnapshotDateTime != null)
            {
                ShowConnectingStatus();
                serverOverviewContentPanel.Visible = false;
                ApplicationController.Default.SetActiveViewToRealTimeMode();
            }
            else if (activeSnapshot != null)
            {
                switch (activeSnapshot.OSMetricsStatistics.OsMetricsStatus)
                {
                    case OSMetricsStatus.WMIServiceTimedOut:
                        ShowOSMetricsConfigurationDialog();
                        break;
                    case OSMetricsStatus.OLEAutomationUnavailable:
                    case OSMetricsStatus.Disabled:
                        if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                        {
                            operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                            using (MonitoredSqlServerInstancePropertiesDialog dlg = new MonitoredSqlServerInstancePropertiesDialog(instanceId))
                            {
                                dlg.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation;
                                if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
                                {
                                    if (!dlg.SavedServer.OleAutomationUseDisabled)
                                    {
                                        enableOleAutomation = true;
                                        operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "Enabling OLE Automation...");
                                    }
                                    ApplicationController.Default.RefreshActiveView();
                                }
                            }
                        }
                        else
                        {
                            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(OsMetricsUnavailableHelp);
                        }
                        break;
                    case OSMetricsStatus.WMIServiceUnreachable:
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(OsMetricsUnavailableHelp);
                        break;
                    case OSMetricsStatus.UnavailableDueToLightweightPooling:
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(LightweightPoolingHelp);
                        break;
                    default:
                        ShowOperationalStatusPanel(false);
                        break;
                }
            }
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
                operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
            }           
        }

        private void ServerSummaryView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

        #region Helpers

        public override void ShowHelp()
        {
            if (designMode)
                ApplicationHelper.ShowHelpTopic(HelpTopics.ServerDashboardViewDesignMode);
            else
                ApplicationHelper.ShowHelpTopic(HelpTopics.ServerDashboardView);
        }

        public void ShowOperationalStatusPanel(bool visible)
        {
            SetAboveContentAlertTheme();

            if (visible)
            {
                serverOverviewContentPanel.AutoScrollMinSize = new Size(dashboardTableLayoutPanel.MinimumSize.Width, dashboardTableLayoutPanel.MinimumSize.Height + operationalStatusPanel.Height);
                operationalStatusPanel.Visible = true;

                ThemeSetter themeSetter = new ThemeSetter();
                if (Settings.Default.ColorScheme == "Dark")
                {
                    themeSetter.SetAboveContentAlertLabelTheme(this.operationalStatusLabel, instanceId);
                    themeSetter.SetAboveContentAlertPictureBoxTheme(this.operationalStatusImage, instanceId);
                }
            }
            else
            {
                serverOverviewContentPanel.AutoScrollMinSize = new Size(dashboardTableLayoutPanel.MinimumSize.Width, dashboardTableLayoutPanel.MinimumSize.Height);
                operationalStatusPanel.Visible = false;
                historyBannerShowing = false;
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetAboveContentAlertTheme();
        }

        private void SetAboveContentAlertTheme()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                themeSetter.SetAboveContentAlertLabelTheme(this.operationalStatusLabel, instanceId);
                themeSetter.SetAboveContentAlertPictureBoxTheme(this.operationalStatusImage, instanceId);
            }
            else
            {
                themeSetter.SetLabelTheme(this.operationalStatusLabel, System.Drawing.Color.LightGray, System.Drawing.Color.Black);
                themeSetter.SetPictureBoxTheme(this.operationalStatusImage, System.Drawing.Color.LightGray, global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall);
            }
        }

        private void ScaleControlAsPerResolution()
        {
            if(AutoScaleSizeHelper.isScalingRequired)
            {
                this.panelGalleryPanel.Width += 200;
                this.panelGalleryPanel.Location = new Point(this.panelGalleryPanel.Location.X - 200, this.panelGalleryGradientPanel.Location.Y);
                this.panelGalleryGradientPanel.Width += 200;
                this.panelGalleryGradientPanel.Location = new Point(this.panelGalleryGradientPanel.Location.X - 200, this.panelGalleryGradientPanel.Location.Y);
                this.galleryPanel.Width += 200;
                this.galleryPanel.Location = new Point(this.galleryPanel.Location.X - 200, this.galleryPanel.Location.Y);
                this.galleryTableLayoutPanel.Width += 200;
                this.panelSelectorTrackBar.Location = new Point(this.panelSelectorTrackBar.Location.X - 50, this.panelSelectorTrackBar.Location.Y-10);
                //this.galleryTableLayoutPanel.Location = new Point(this.galleryTableLayoutPanel.Location.X - 200, this.galleryTableLayoutPanel.Location.Y);
            }
        }

        #region Designer

        public override void ToggleDesignMode(bool enabled)
        {
            base.ToggleDesignMode(enabled);

            foreach (DashboardControl control in dashboardTableLayoutPanel.Controls)
            {
                control.SetDesignMode(enabled);
            }

            if (enabled && galleryTableLayoutPanel.Controls.Count == 0)
            {
                List<DashboardPanelSelector> panels = DashboardHelper.GetPanelList();
                galleryTableLayoutPanel.SuspendLayout();
                galleryTableLayoutPanel.RowStyles.Clear();

                int count = 0;
                int? cloudID = ApplicationModel.Default.AllInstances[instanceId].CloudProviderId;

                foreach (DashboardPanelSelector panel in panels)
                {
                    if (cloudID == Common.Constants.AmazonRDSId)
                    {
                        switch (panel.DashboardPanel)
                        {
                            //Do not show the following panels for RDS.
                            case DashboardPanel.CustomCounters:
                            case DashboardPanel.Disk:
                            case DashboardPanel.Empty:
                            case DashboardPanel.FileActivity:
                            case DashboardPanel.Network:
                            case DashboardPanel.TempDB:
                            case DashboardPanel.VM:
                            case DashboardPanel.AzureDisk:
                            {
                                continue;
                            }
                        }
                    }
                    else if (cloudID == Common.Constants.MicrosoftAzureId || cloudID == Common.Constants.MicrosoftAzureManagedInstanceId)
                    {
                        switch (panel.DashboardPanel)
                        {
                            //Do not show the following panels for Azure and Azure Managed Instance
                            case DashboardPanel.Disk:
                            case DashboardPanel.Empty:
                            case DashboardPanel.FileActivity:
                            case DashboardPanel.Network:
                            case DashboardPanel.TempDB:
                            case DashboardPanel.VM:
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        //we want to hide cloud specific panels.  There is no default on purpose.
                        switch (panel.DashboardPanel)
                        {
                            case DashboardPanel.AzureDisk:
                            case DashboardPanel.Empty:
                            case DashboardPanel.SQLServerPhysicalIO:
                            {
                                continue;
                            }
                        }
                    }
                    DraggablePanel draggablePanel = new DraggablePanel(panel, panel.GetPanelImage());
                    draggablePanel.Dock = DockStyle.Fill;
                    draggablePanel.TabStop = false;
                    draggablePanel.MouseClick += draggablePanel_MouseClick;
                    galleryTableLayoutPanel.Controls.Add(draggablePanel, 0, count);

                    galleryTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100f));
                    count++;
                }
                galleryTableLayoutPanel.Height = count * 100;
                galleryTableLayoutPanel.ResumeLayout();

                //panelDesignerPanelCountLabel.Text = string.Format("{0} Panels", panels.Count);
                panelDesignerPanelCountLabel.Text = string.Format("{0} Panels", count);
                galleryTableLayoutPanel.RowCount = count;


                panelSelectorTrackBar.MinValue = Math.Min(0, galleryPanel.Height - galleryTableLayoutPanel.Height);
                panelSelectorTrackBar.MaxValue = 0;
                panelSelectorTrackBar.Value = 0;
            }
            PanelGalleryVisible = enabled;
            hasDesignChanges = hasDesignChanges && enabled;
        }

        internal void SetDashboardLayout(Size layout)
        {
            dashboardTableLayoutPanel.SuspendLayout();

            //remove any controls that are outside the new bounds
            List<DashboardControl> removedControls = new List<DashboardControl>();
            foreach (var dashboardControl in dashboardControls)
            {
                if (dashboardControl.DashboardPanelConfiguration.Row >= layout.Height
                    || dashboardControl.DashboardPanelConfiguration.Column >= layout.Width)
                {
                    dashboardTableLayoutPanel.Controls.Remove(dashboardControl);
                    removedControls.Add(dashboardControl);
                }
            }
            foreach (DashboardControl control in removedControls)
            {
                dashboardControls.Remove(control);
            }
            dashboardTableLayoutPanel.RowStyles.Clear();
            dashboardTableLayoutPanel.ColumnStyles.Clear();

            dashboardConfiguration.SetSize(layout.Width, layout.Height);
            dashboardTableLayoutPanel.RowCount = dashboardConfiguration.Rows;
            float width = 100f / dashboardConfiguration.Rows;
            for (int rows = 1; rows <= dashboardConfiguration.Rows; rows++)
            {
                dashboardTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, width));
            }

            dashboardTableLayoutPanel.ColumnCount = dashboardConfiguration.Columns;
            width = 100f / dashboardConfiguration.Columns;
            for (int columns = 1; columns <= dashboardConfiguration.Columns; columns++)
            {
                dashboardTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, width));
            }

            for (int i = 0; i < dashboardConfiguration.Columns; i++)
            {
                for (int j = 0; j < dashboardConfiguration.Rows; j++)
                {
                    if (dashboardTableLayoutPanel.GetControlFromPosition(i, j) == null)
                    {
                        DashboardPanelConfiguration panelConfig = new DashboardPanelConfiguration(j, i, DashboardPanel.Empty);
                      //  DashboardPanelConfiguration panelConfig = new DashboardPanelConfiguration(j, i, DashboardPanel.AnalysisView);
                        AddPanel(panelConfig);
                    }
                }
            }
            hasDesignChanges = true;

            dashboardTableLayoutPanel.MinimumSize = new Size(DashboardPanelMinimumWidth * dashboardConfiguration.Columns, DashboardPanelMinimumHeight * dashboardConfiguration.Rows);

            dashboardTableLayoutPanel.ResumeLayout();

            // recalculate sizes for display
            ShowOperationalStatusPanel(operationalStatusPanel.Visible);
            serverOverviewContentPanel.Refresh();
        }

        private void panelDesignerCloseButton_Click(object sender, EventArgs e)
        {
            PanelGalleryVisible = false;
        }

        private void panelSelectorTrackBar_ValueChanged(object sender, EventArgs e)
        {
            galleryTableLayoutPanel.Top = panelSelectorTrackBar.Value;
            galleryTableLayoutPanel.Invalidate();
            dashboardPanelDetails.Hide();
        }

        private void panelGalleryGradientPanel_Resize(object sender, EventArgs e)
        {
            panelSelectorTrackBar.MinValue = Math.Min(0, galleryPanel.Height - galleryTableLayoutPanel.Height);
            panelSelectorTrackBar.Value = Math.Min(panelSelectorTrackBar.MaxValue, panelSelectorTrackBar.Value);
        }

        private void draggablePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender is DraggablePanel)
            {
                DraggablePanel panel = (DraggablePanel)sender;
                dashboardPanelDetails.Visible = false;
                int pos = galleryTableLayoutPanel.GetRow(panel);
                int top = 36 + galleryTableLayoutPanel.Top + (pos * 100);
                if (top + dashboardPanelDetails.Height > this.Bottom)
                {
                    top = Math.Max(0, this.Bottom - dashboardPanelDetails.Height);
                }
                else
                {
                    top = Math.Max(0, top);
                }
                dashboardPanelDetails.Top = top;
                if (AutoScaleSizeHelper.isScalingRequired) dashboardPanelDetails.Left = galleryPanel.Location.X+ 2440;
                dashboardPanelDetails.ShowPanel(this, panel.DashboardPanel, historyData, this.vHost);
                dashboardPanelDetails.BringToFront();
            }
        }

        private void HandleChangePanel(object sender, DropPanelEventArgs e)
        {
            if (sender is DashboardControl
                && PanelShouldBeDropped(e.DashboardPanel))
            {
                DashboardControl oldControl = (DashboardControl)sender;

                TableLayoutPanelCellPosition dropPosition = dashboardTableLayoutPanel.GetCellPosition(oldControl);

                dashboardTableLayoutPanel.SuspendLayout();

                if (e.PanelDragType == PanelDragType.Drop)
                {
                    DashboardPanelConfiguration panelConfig = new DashboardPanelConfiguration(dropPosition.Row,
                                                                                              dropPosition.Column,
                                                                                              e.DashboardPanel);
                    RemovePanel(oldControl);
                    AddPanel(panelConfig);

                    hasDesignChanges = true;
                }
                else if (e.PanelDragType == PanelDragType.Swap && e.DraggedPanelConfiguration != null)
                {
                    if (e.DraggedPanelConfiguration.Column == -1 && e.DraggedPanelConfiguration.Row == -1)
                    {
                        // if -1 is passed for position, it is dragged from the details panel and can't be swapped so it becomes a drop
                        DashboardPanelConfiguration panelConfig = new DashboardPanelConfiguration(e.DraggedPanelConfiguration.Row,
                                                                                                  e.DraggedPanelConfiguration.Column,
                                                                                                  e.DashboardPanel);
                        RemovePanel(oldControl);
                        AddPanel(panelConfig);

                        hasDesignChanges = true;
                    }
                    else
                    {
                        TableLayoutPanelCellPosition dragPosition = new TableLayoutPanelCellPosition(e.DraggedPanelConfiguration.Column, e.DraggedPanelConfiguration.Row);
                        DashboardControl draggedControl = (DashboardControl)dashboardTableLayoutPanel.GetControlFromPosition(dragPosition.Column, dragPosition.Row);
                        if (draggedControl != null)
                        {
                            draggedControl.SetPosition(dropPosition.Column, dropPosition.Row);
                            dashboardTableLayoutPanel.SetCellPosition(draggedControl, dropPosition);

                            oldControl.SetPosition(dragPosition.Column, dragPosition.Row);
                            dashboardTableLayoutPanel.SetCellPosition(oldControl, dragPosition);
                        }

                        if (dragPosition.Column != dropPosition.Column ||
                            dragPosition.Row != dropPosition.Row)
                        {
                            hasDesignChanges = true;
                        }
                    }
                }

                dashboardTableLayoutPanel.ResumeLayout();
            }

            // Autoscale font size.
            AdaptFontSize();
        }

        private bool PanelShouldBeDropped(DashboardPanel dashboardPanel)
        {
            if (_serverVersion == null)
            {
                MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instanceId);
                if (status != null)
                {
                    _serverVersion = status.InstanceVersion;
                }
            }

            if (_serverVersion == null || DashboardHelper.IsVersionSupported(dashboardPanel, _serverVersion)
                || DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, "This panel is not supported for the SQL Server version of the current instance.\n\nDo you wish to place the panel on the dashboard anyway?"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddPanel(DashboardPanelConfiguration panelConfig)
        {
            DashboardControl dashboardControl = panelConfig.GetNewDashboardControl(this.vHost);
            if (dashboardControl != null)
            {
                hasDesignChanges = designMode;
                int? cloudID = ApplicationModel.Default.AllInstances[instanceId].CloudProviderId;

                if (cloudID == Common.Constants.MicrosoftAzureId || cloudID == Common.Constants.MicrosoftAzureManagedInstanceId)
                {
                    switch (panelConfig.Panel)
                    {
                        case DashboardPanel.Cache:
                        case DashboardPanel.Cpu:
                        case DashboardPanel.CustomCounters:
                        case DashboardPanel.Databases:
                        case DashboardPanel.LockWaits:
                        case DashboardPanel.Memory:
                        case DashboardPanel.ServerWaits:
                        case DashboardPanel.Sessions:
                        case DashboardPanel.SQLServerPhysicalIO:
                        case DashboardPanel.AzureDisk:
                        case DashboardPanel.Empty:
                        {
                            dashboardControls.Add(dashboardControl);
                            dashboardTableLayoutPanel.Controls.Add(dashboardControl, panelConfig.Column, panelConfig.Row);
                            break;
                        }
                        default:
                        {
                            dashboardControls.Remove(dashboardControl);
                            break;
                        }
                    }
                }
                else if (cloudID == Common.Constants.AmazonRDSId)
                {
                    switch (panelConfig.Panel)
                    {
                        case DashboardPanel.Cache:
                        case DashboardPanel.Cpu:
                        case DashboardPanel.Databases:
                        case DashboardPanel.LockWaits:
                        case DashboardPanel.Memory:
                        case DashboardPanel.ServerWaits:
                        case DashboardPanel.Sessions:
                        case DashboardPanel.SQLServerPhysicalIO:
                        case DashboardPanel.Empty:
                        {
                            dashboardControls.Add(dashboardControl);
                            dashboardTableLayoutPanel.Controls.Add(dashboardControl, panelConfig.Column, panelConfig.Row);
                            break;
                        }
                        default:
                        {
                            dashboardControls.Remove(dashboardControl);
                            break;
                        }
                    }
                }
                else
                {
                    switch (panelConfig.Panel)
                    {
                        case DashboardPanel.Cache:
                        case DashboardPanel.Cpu:
                        case DashboardPanel.CustomCounters:
                        case DashboardPanel.Databases:
                        case DashboardPanel.Disk:
                        case DashboardPanel.FileActivity:
                        case DashboardPanel.LockWaits:
                        case DashboardPanel.Memory:
                        case DashboardPanel.Network:
                        case DashboardPanel.ServerWaits:
                        case DashboardPanel.Sessions:
                        case DashboardPanel.TempDB:
                        case DashboardPanel.VM:
                        case DashboardPanel.Empty:
                        {
                            dashboardControls.Add(dashboardControl);
                            dashboardTableLayoutPanel.Controls.Add(dashboardControl, panelConfig.Column, panelConfig.Row);
                            break;
                        }
                        default:
                        {
                            dashboardControls.Remove(dashboardControl);
                            break;
                        }
                    }
                }
                dashboardControl.Dock = DockStyle.Fill;
                dashboardControl.Margin = new Padding(0,0,10,0);
                dashboardControl.DragDropPanel += HandleChangePanel;
                dashboardControl.ChartDrilldown += OnChartDrilldown;  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                                                                      // initialize the control and backfill
                dashboardControl.Initialize(this, historyData, panelConfig);
                // show vm data if available
                MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
                dashboardControl.ShowVMData(instanceObject.Instance.IsVirtualized);
                // load the current data to the charts
                dashboardControl.ConfigureDataSource();
                dashboardControl.SetDesignMode(designMode);
            }
        }

        private void RemovePanel(DashboardControl dashboardControl)
        {
            hasDesignChanges = designMode;
            dashboardControl.DragDropPanel -= HandleChangePanel;
            dashboardControl.ChartDrilldown -= OnChartDrilldown;  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            dashboardTableLayoutPanel.Controls.Remove(dashboardControl);
            dashboardControls.Remove(dashboardControl);
            dashboardControl.Dispose();
        }

        public override void SaveDashboardDesign()
        {
            SaveDashboardDesignDialog saveDialog = new SaveDashboardDesignDialog(instanceId, userName, dashboardLayoutID, dashboardConfiguration.Name);

            if (DialogResult.OK == saveDialog.ShowDialog(this.FindForm()))
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    // update the configuration with the panel selections
                    dashboardConfiguration.Panels.Clear();
                    foreach (DashboardControl control in dashboardControls)
                    {
                        dashboardConfiguration.Panels.Add(control.DashboardPanelConfiguration);
                    }

                    // save an image of the dashboard
                    Rectangle bounds = new Rectangle(0, 0, dashboardTableLayoutPanel.Bounds.Width, dashboardTableLayoutPanel.Bounds.Height);
                    Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
                    using (Log.DebugCall("SaveDashboardDesign DrawToBitmap"))
                    {
                        dashboardTableLayoutPanel.DrawToBitmap(bitmap, bounds);
                    }
                    if (saveDialog.SaveToFile)
                    {
                        string fileName = string.Format("C://{0}.png", saveDialog.DashboardName.Replace(' ', '_'));
                        try
                        {
                            bitmap.Save(fileName, ImageFormat.Png);
                        }
                        catch (Exception ex)
                        {
                            Log.DebugFormat(string.Format("Unable to save dashboard layout image to file {0}", fileName), ex);
                        }
                    }

                    dashboardConfiguration.Name = saveDialog.DashboardName;
                    SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                    if (saveDialog.ReplaceDashboardLayoutId == 0)
                    {
                        dashboardLayoutID =
                            RepositoryHelper.AddDashboardLayout(
                                connectionInfo,
                                userName,
                                saveDialog.DashboardName,
                                dashboardConfiguration,
                                bitmap,
                                saveDialog.UseAsGlobalDefault);
                    }
                    else
                    {
                        RepositoryHelper.UpdateDashboardLayout(
                            connectionInfo,
                            saveDialog.ReplaceDashboardLayoutId,
                            saveDialog.DashboardName,
                            dashboardConfiguration,
                            bitmap,
                            saveDialog.UseAsGlobalDefault);
                        dashboardLayoutID = saveDialog.ReplaceDashboardLayoutId;
                    }
                    hasDesignChanges = false;

                    List<string> selectedItems = new List<string>();
                    if (saveDialog.SelectedInstances.Count > 0)
                    {
                        foreach (var instance in saveDialog.SelectedInstances)
                        {
                            RepositoryHelper.SetDefaultDashboardLayout(connectionInfo, userName, instance.Id, dashboardLayoutID);
                            selectedItems.Add(instance.InstanceName);
                        }
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat(dashboardConfiguration.Name != saveDialog.DashboardName
                                        ? "The dashboard layout was saved as '{0}'"
                                        : "Dashboard layout '{0}' was succesfully saved", saveDialog.DashboardName);
                    if (selectedItems.Count > 0)
                    {
                        sb.AppendFormat(" and is now assigned to {0}",
                                        string.Join(",", selectedItems));
                    }
                    sb.Append(".");
                    if (saveDialog.UseAsGlobalDefault)
                    {
                        sb.AppendFormat("\n\nDashboard layout '{0}' is now your default for new and unassigned servers.",
                                        saveDialog.DashboardName);
                    }
                    ApplicationMessageBox.ShowInfo(this, sb.ToString());
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Error saving Dashboard Layout.", ex);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        #endregion

        #endregion

        #region Data Management

        private void UpdateRealTimeChartDataFilter()
        {
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                //SQLDM - 19237 - Vamshi Krishna - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion

        #region Refresh Data

        public override void RefreshView()
        {
            alerts = ApplicationModel.Default.GetAlertConfiguration(instanceId);

            historyModeLoadError = null;

            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                ShowConnectingStatus();
                base.RefreshView();

            
            }
        }

        private void ShowConnectingStatus()
        {
            historicalSnapshotStatusLinkLabel.Visible = false;
            viewStatusImage.Visible = false;
            viewStatusConnectingCircle.Visible = true;
            viewStatusConnectingCircle.Active = true;
            viewStatusLabel.Text = "Connecting...";
            viewStatusLabel.Enabled = false;
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            using (Log.VerboseCall("DoRefreshWork"))
            {
                // load current counter list from the repository
                List<int> currentCounters = null;
                Dictionary<int, List<int>> counterMap =
                    RepositoryHelper.GetMonitoredServerCustomCounters(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        instanceId, true);
                counterMap.TryGetValue(instanceId, out currentCounters);
                configuredCustomCounters = currentCounters;

                var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
                currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;

                if (HistoricalSnapshotDateTime == null)
                {
                    if (!historyData.RealTimeSnapshotsPrePopulated)
                        historyData.PrePopulateRealTimeSnapshots(instanceId, this);
                    else
                    {
                        historyData.BackfillScheduledRefreshData(instanceId, this);

                        // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                        if (previousVisibleLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                            historyData.ForwardFillHistoricalData(instanceId, this);

                        // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                        if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                            historyData.BackFillScheduledHistoricalData(instanceId, this);
                    }

                    if (backgroundWorker.CancellationPending) return null;

                    // this will refresh the current realtime data on entry and switching from history to realtime
                    backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

                    if (backgroundWorker.CancellationPending) return null;

                    // go fetch the realtime snapshot
                    return GetRealTimeSnapshots();
                }
                else
                {
                    object result = null;

                    if (!backgroundWorker.CancellationPending)
                    {
                        // If historical start/end did not change global, the snapshot is already populated.
                        if (!historyData.HistoricalSnapshotsPopulated)
                            result = historyData.PopulateHistoricalSnapshots(instanceId, this, backgroundWorker);
                        else
                            result = historyData.HistoricalSnapshots;
                    }

                    return result;
                }
            }
        }

        protected override void UpdateProgress(object state, int progress)
        {
            using (Log.VerboseCall("UpdateProgress"))
            {
                switch (progress)
                {
                    case (int)Progress.Backfill:
                        if (historyData.RealTimeSnapshotsDataTable.Rows.Count == 0) return;
                        if (historyBannerShowing)
                            ShowOperationalStatusPanel(false);
                        UpdateBindings();
                        lastRefreshError = null;
                        ShowServerOverviewContentPanel();
                        break;
                }
            }
        }

        private void ShowServerOverviewContentPanel()
        {
            if (serverOverviewContentPanel.Visible) return;

            if (!serverOverviewContentPanel.IsHandleCreated)
            {
                serverOverviewContentPanel.CreateControl();
                serverOverviewContentPanel.Size = this.ClientSize;
            }

            serverOverviewContentPanel.Visible = true;
            viewStatusConnectingCircle.Active = false;
        }

        private void UpdateBindings()
        {
            using (Log.VerboseCall("UpdateBindings"))
            {
                foreach (var dashboardControl in dashboardControls)
                {
                    dashboardControl.ConfigureDataSource();
                }

                if (dashboardPanelDetails.Visible)
                    dashboardPanelDetails.ConfigureControlDataSource();
            }
        }

        private ServerSummarySnapshots GetRealTimeSnapshots()
        {
            using (Log.VerboseCall("GetRealTimeSnapshots"))
            {

                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(connectionInfo);

                if (!customizeBaselineConfigurationMessageShown)
                {
                    if (!earliestAvailableData.HasValue)
                    {
                        using (
                            System.Data.SqlClient.SqlConnection connection =
                                connectionInfo.GetConnection(
                                    Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                        {
                            bool defaultBaselineConfig;
                            DateTime start;
                            DateTime end;
                            short days;

                            connection.Open();
                            // one way to the the date time of the first collection available from the repository
                            bool baselineSet = BaselineHelpers.GetBaselineParameters(connection,
                                                                                     instanceId,
                                                                                     out defaultBaselineConfig,
                                                                                     out start,
                                                                                     out end,
                                                                                     out days, out earliestAvailableData);

                            if (baselineSet)
                                customizeBaselineConfigurationMessageShown = true;
                        }
                    }
                }

                if (enableOleAutomation)
                {
                    managementService.SendReconfiguration(
                        new ReconfigurationConfiguration(instanceId, "Ole Automation Procedures", 1));
                    enableOleAutomation = false;
                }

                ServerOverviewConfiguration serverOverviewSnapshotConfiguration =
                    new ServerOverviewConfiguration(instanceId, historyData.LastServerOverviewSnapshot);

                // collect custom counters
                if (configuredCustomCounters != null && configuredCustomCounters.Count > 0)
                {
                    List<CustomCounterConfiguration> counterList = new List<CustomCounterConfiguration>();
                    foreach (int metricID in configuredCustomCounters)
                    {
                        if (previousCounterSnapshot != null &&
                            previousCounterSnapshot.CustomCounterList.ContainsKey(metricID))
                        {
                            counterList.Add(
                                new CustomCounterConfiguration(instanceId,
                                                               previousCounterSnapshot.CustomCounterList[metricID]));
                        }
                        else
                        {
                            CustomCounterDefinition ccd = metricDefinitions.GetCounterDefinition(metricID);
                            if (ccd != null && ccd.IsEnabled)
                            {
                                counterList.Add(new CustomCounterConfiguration(instanceId, ccd));
                            }
                        }
                    }
                    if (counterList.Count > 0)
                        serverOverviewSnapshotConfiguration.CustomCounterConfigurations = counterList;
                }

                // If no snapshot has been taken, go ahead and take one so that we can calculate values that
                // require two snapshots to initialize
                var now = DateTime.Now;
                var timeDiff = historyData.LastServerOverviewSnapshot == null ? TimeSpan.MinValue : now - historyData.LastServerOverviewSnapshot.TimeStampLocal;

                if (historyData.LastServerOverviewSnapshot == null || timeDiff > TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds))
                {
                    ServerSummarySnapshots summarySnapshots =
                        managementService.GetServerSummary(serverOverviewSnapshotConfiguration);

                    if (summarySnapshots != null)
                    {
                        serverOverviewSnapshotConfiguration =
                            new ServerOverviewConfiguration(instanceId, summarySnapshots);
                    }
                }

                var serverOverviewSnapshots = managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
                previousCounterSnapshot = serverOverviewSnapshots.CustomCounterCollectionSnapshot;

                return serverOverviewSnapshots;
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (enableOleAutomation)
            {
                operationalStatusImage.Image = Properties.Resources.StatusCriticalSmall;
                operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "Failed to enable collection of OS metrics.");
                enableOleAutomation = false;
            }
            else
            {
                if (HistoricalSnapshotDateTime != null)
                {
                    historyModeLoadError = e;
                    historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                    historicalSnapshotStatusLinkLabel.Visible = true;
                    serverOverviewContentPanel.Visible = false;
                }
                else
                {
                    lastRefreshError = e;
                    viewStatusConnectingCircle.Active = false;
                    viewStatusConnectingCircle.Visible = false;
                    viewStatusImage.Image = Properties.Resources.StatusSummaryCriticalLarge;
                    viewStatusImage.Visible = true;
                    viewStatusLabel.Text = UnableToUpdateDataText;
                    viewStatusLabel.Enabled = true;
                    serverOverviewContentPanel.Visible = false;
                }
            }

            base.HandleBackgroundWorkerError(e);
        }

        private void UpdateOperationalStatus()
        {
            if (activeSnapshot != null)
            {
                if (HistoricalSnapshotDateTime != null)
                {
                    operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                    operationalStatusLabel.Text =
                        string.Format(OperationalStatusMessage,
                                      Properties.Resources.HistoryModeOperationalStatusLabel);
                    ShowOperationalStatusPanel(true);
                    historyBannerShowing = true;
                }
                else
                {
                    historyBannerShowing = false;
                    ApplicationMessageBox box = new ApplicationMessageBox();
                    DialogResult result = DialogResult.Cancel;
                    box.Symbol = ExceptionMessageBoxSymbol.Information;
                    box.Buttons = ExceptionMessageBoxButtons.OK;
                    box.ShowCheckBox = true;
                    box.CheckBoxText = "Don't show this message again.";
                    box.Caption = "OS Metrics Unavailable";

                    switch (activeSnapshot.OSMetricsStatistics.OsMetricsStatus)
                    {
                        case OSMetricsStatus.WMIServiceTimedOut:
                            ShowWMIServiceTimedoutMessage(box, result);
                            break;
                        case OSMetricsStatus.Disabled:
                        case OSMetricsStatus.OLEAutomationUnavailable:
                            if (IsWindowsInstance())        //SQLDM 10.3(Manali H) : Fix for SQLDM-28754
                            {
                                operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                                if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify &&
                                    activeSnapshot.ProductVersion.Major >= 9) // SQL Server 2005/2008
                                {
                                    if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.AmazonRDSId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                                    {
                                        operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "WMI or OLE automation must be enabled to collect Operating System data. Click here to configure collection of OS metrics.");
                                    }
                                    box.Text = "Unable to collect Operating System (OS) data." + System.Environment.NewLine +
                                        "To troubleshoot:" + System.Environment.NewLine +
                                        "1. Verify that OLE automation is enabled to collect OS data. To configure the OLE automation collector click 'Yes'." + System.Environment.NewLine +
                                        "2. Verify that server is turned on or that it can accept connections from outside.";
                                    box.Buttons = ExceptionMessageBoxButtons.YesNo;
                                    box.Symbol = ExceptionMessageBoxSymbol.Question;
                                }
                                else
                                {
                                    operationalStatusLabel.Text =
                                        string.Format(OperationalStatusMessage,
                                                      "WMI must be available to collect Operating System data. Click here for more information.");
                                    box.Text = "WMI must be available to collect Operating System data.";
                                }

                                bool showDialog = Settings.Default.ShowMessage_OLEAutomationUnavailable;
                                if (activeSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.Disabled)
                                    showDialog = Settings.Default.ShowMessage_OLEAutomationDisabled;

                                if (displayWMIErrorDialog && !operationalStatusPanel.Visible && showDialog)
                                {
                                    displayWMIErrorDialog = false;
                                    result = box.Show(this);

                                    if (activeSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.Disabled)
                                        Settings.Default.ShowMessage_OLEAutomationDisabled = !box.IsCheckBoxChecked;
                                    else
                                        Settings.Default.ShowMessage_OLEAutomationUnavailable = !box.IsCheckBoxChecked;
                                }

                                if (result == DialogResult.Yes)
                                {
                                    operationalStatusLabel_MouseDown(null, null);
                                    operationalStatusLabel_MouseUp(null, null);
                                }
                                else
                                {
                                    ShowOperationalStatusPanel(true);
                                }
                            }
                            break;
                        case OSMetricsStatus.WMIServiceUnreachable:
                            if (IsWindowsInstance())        //SQLDM 10.3(Manali H) : Fix for SQLDM-28754
                            {
                                operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                                operationalStatusLabel.Text =
                                    string.Format(OperationalStatusMessage,
                                                  "The Windows Management Instrumentation service must be running to collect operating system statistics. Click here for more information.");
                                box.Text = "The Windows Management Instrumentation service must be running to collect operating system statistics.";
                                box.Caption = "WMI Unavailable";

                                if (displayWMIErrorDialog &&
                                    (operationalStatusPanel.Visible == false) &&
                                    (Properties.Settings.Default.ShowMessage_WMIServiceUnreachable))
                                {
                                    displayWMIErrorDialog = false;
                                    box.Show(this);
                                    Properties.Settings.Default.ShowMessage_WMIServiceUnreachable = !box.IsCheckBoxChecked;
                                }
                                ShowOperationalStatusPanel(true);
                            }
                            break;
                        case OSMetricsStatus.UnavailableDueToLightweightPooling:
                            if (IsWindowsInstance())        //SQLDM 10.3(Manali H) : Fix for SQLDM-28754
                            {
                                operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                                operationalStatusLabel.Text =
                                    string.Format(OperationalStatusMessage,
                                                  "OLE Automation is needed to collect operating system statistics, but is unavailable because Lightweight Pooling is enabled. Click here for more information.");
                                box.Text = "OLE Automation is needed to collect operating system statistics, but is unavailable because Lightweight Pooling is enabled.";

                                if (displayWMIErrorDialog &&
                                    (operationalStatusPanel.Visible == false) &&
                                    (Properties.Settings.Default.ShowMessage_LightweightPooling))
                                {
                                    displayWMIErrorDialog = false;
                                    box.Show(this);
                                    Properties.Settings.Default.ShowMessage_LightweightPooling = !box.IsCheckBoxChecked;
                                }
                                ShowOperationalStatusPanel(true);
                            }
                            break;
                        default:
                            ShowOperationalStatusPanel(false);
                            break;
                    }
                }
            }
        }

        private void ShowWMIServiceTimedoutMessage(ApplicationMessageBox box, DialogResult result)
        {
            bool hasPermissions = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;

            if (hasPermissions)
            {
                operationalStatusLabel.Text =
                                    string.Format(OperationalStatusMessage,
                                                  "WMI service timed out. Click here to change the Operating System metrics collection settings.");
                box.Text = "WMI service timed out. Would you like to change the Operating System metrics collection settings?";
                box.Buttons = ExceptionMessageBoxButtons.YesNo;
                box.Symbol = ExceptionMessageBoxSymbol.Question;
            }
            else
            {
                operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "WMI service timed out.");
                box.Text = "WMI service timed out.";
                box.Buttons = ExceptionMessageBoxButtons.OK;
                box.Symbol = ExceptionMessageBoxSymbol.Information;
            }

            if (displayWMITimedoutDialog && !operationalStatusPanel.Visible && Settings.Default.ShowMessage_WMIServiceTimedout)
            {
                displayWMITimedoutDialog = false;
                result = box.Show(this);
            }

            Settings.Default.ShowMessage_WMIServiceTimedout = !box.IsCheckBoxChecked;

            if (result == DialogResult.Yes)
            {
                ShowOSMetricsConfigurationDialog();
            }
            else
            {
                ShowOperationalStatusPanel(true);
            }
        }

        private void ShowOSMetricsConfigurationDialog()
        {
            if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
            {
                operationalStatusImage.Image = Properties.Resources.StatusWarningSmall;
                using (MonitoredSqlServerInstancePropertiesDialog dlg = new MonitoredSqlServerInstancePropertiesDialog(instanceId))
                {
                    dlg.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation;
                    if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
                    {
                        if (!dlg.SavedServer.OleAutomationUseDisabled)
                        {
                            enableOleAutomation = true;
                            operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "Enabling OLE Automation...");
                        }

                        ApplicationController.Default.RefreshActiveView();
                    }
                }
            }
            else
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(OsMetricsUnavailableHelp);
            }
        }

        public override void UpdateData(object data)
        {
            lastRefreshError = null;
            viewStatusConnectingCircle.Active = false;

            lock (updateLock)
            {
                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    if (data != null && data is ServerSummarySnapshots)
                    {
                        UpdateWithRealTimeSnapshots(data as ServerSummarySnapshots);
                        UpdateBindings();
                    }
                    else
                    {
                        HandleBackgroundWorkerError(new DesktopClientException("The snapshot type is not valid."));
                    }
                    MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
                    ShowVMData(instanceObject.Instance.IsVirtualized);
                }
                else
                {
                    if (data != null && data is ServerSummarySnapshots)
                    {
                        UpdateWithHistoricalSnapshots(data as ServerSummarySnapshots);
                        UpdateBindings();
                    }
                    else
                    {
                        historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                        historicalSnapshotStatusLinkLabel.Visible = true;
                        serverOverviewContentPanel.Visible = false;
                        ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    ShowVMData(historyData.IsVM);
                }
            }
        }

        private void UpdateWithRealTimeSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                historyData.AddRealTimeSnapshots(summarySnapshots);

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    activeSnapshot = summarySnapshots.ServerOverviewSnapshot;
                    DateTime snapshotCompleteTime = DateTime.Now;

                    if (summarySnapshots.ServerOverviewSnapshot.Error == null)
                    {
                        if (summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.HasValue &&
                            summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.Value == ServiceState.Running)
                        {
                            UpdateOperationalStatus();
                            UpdateRealTimeChartDataFilter();
                            ShowServerOverviewContentPanel();
                            viewStatusLabel.Text = String.Empty;
                        }
                        else if (summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.HasValue)
                        {
                            viewStatusConnectingCircle.Visible = false;

                            switch (summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.Value)
                            {
                                case ServiceState.Paused:
                                    viewStatusImage.Image = Properties.Resources.Pause32x32;
                                    viewStatusImage.Visible = true;
                                    viewStatusLabel.Text = "The SQL Server service is paused.";
                                    break;
                                default:
                                    viewStatusImage.Image = Properties.Resources.Critical32x32;
                                    viewStatusImage.Visible = true;
                                    viewStatusLabel.Text = "The SQL Server service is unavailable.";
                                    break;
                            }

                            viewStatusLabel.Enabled = true;
                            serverOverviewContentPanel.Visible = false;
                        }

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(snapshotCompleteTime));
                    }
                    else
                    {
                        ShowServerOverviewContentPanel();
                        //lastRefreshError = summarySnapshots.ServerOverviewSnapshot.Error;
                        //viewStatusConnectingCircle.Visible = false;
                        //viewStatusImage.Image = Properties.Resources.StatusSummaryCriticalLarge;
                        //viewStatusImage.Visible = true;
                        //viewStatusLabel.Text = UnableToUpdateDataText;
                        //viewStatusLabel.Enabled = true;
                        //serverOverviewContentPanel.Visible = false;
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(snapshotCompleteTime,
                                                                    summarySnapshots.ServerOverviewSnapshot.Error));
                    }
                }
            }
        }

        private void UpdateWithHistoricalSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    activeSnapshot = summarySnapshots.ServerOverviewSnapshot;
                    // SQLDM-30163 - For Cloud instances service status is not available, hence skip the check
                    bool isCloudInstance = ((ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)) ? true : false;
                    if (isCloudInstance || summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.HasValue &&
                        summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.Value == ServiceState.Running)
                    {
                        UpdateOperationalStatus();
                        ShowServerOverviewContentPanel();
                        viewStatusLabel.Text = String.Empty;
                    }
                    else if (summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.HasValue)
                    {
                        viewStatusConnectingCircle.Visible = false;

                        switch (summarySnapshots.ServerOverviewSnapshot.SqlServiceStatus.Value)
                        {
                            case ServiceState.Paused:
                                viewStatusImage.Image = Properties.Resources.Pause32x32;
                                viewStatusImage.Visible = true;
                                viewStatusLabel.Text = "The SQL Server service is paused.";
                                break;
                            default:
                                viewStatusImage.Image = Properties.Resources.Critical32x32;
                                viewStatusImage.Visible = true;
                                viewStatusLabel.Text = "The SQL Server service is unavailable.";
                                break;
                        }

                        viewStatusLabel.Enabled = true;
                        serverOverviewContentPanel.Visible = false;
                    }

                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
            }
        }

        #endregion

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                ShowConnectingStatus();
                ApplicationController.Default.SetActiveViewToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        internal override void OnServerAlertConfigurationChanged(IEnumerable<MetricThresholdEntry> thresholdEntries)
        {
            foreach (var dashboardControl in dashboardControls)
            {
                dashboardControl.OnServerAlertConfigurationChanged(thresholdEntries);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        ///summary
        ///Helper method to if it is windows instance or not   
        ///SQLDM 10.3(Manali H) : Fix for SQLDM-28754
        protected bool IsWindowsInstance()
        {
            return activeSnapshot.WindowsVersion.Contains("Windows");
        }
    }
}