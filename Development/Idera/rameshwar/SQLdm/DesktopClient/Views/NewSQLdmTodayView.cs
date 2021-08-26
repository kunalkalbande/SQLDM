using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Idera.Newsfeed.Plugins.UI;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ApplicationMessageBox = Idera.SQLdm.Common.UI.Dialogs.ApplicationMessageBox;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;

namespace Idera.SQLdm.DesktopClient.Views
{
    internal partial class NewSQLdmTodayView : View
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("NewSQLdmTodayView");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private const string CriticalServersLabelText = "{0} server(s) in Critical state";
        private const string WarningServersLabelText = "{0} server(s) in Warning state";
        private const string MaintenanceModeServersLabelText = "{0} server(s) in Maintenance Mode";
        private const string OkServersLabelText = "{0} server(s) in OK state";

        private int maintenanceModeServers;
        private int criticalServers;
        private int warningServers;
        private int okServers;
        private GridSettings lastAlertsGridSettings = null;
        private UltraGrid selectedGrid = null;
        private UltraGridColumn selectedColumn = null;
        private IList<int> _mostCriticalServers;
        private readonly List<Pair<PictureBox, LinkLabel>> _mostCriticalServersControls = new List<Pair<PictureBox, LinkLabel>>();
        private readonly List<Pair<PictureBox, LinkLabel>> _recentServersControls = new List<Pair<PictureBox, LinkLabel>>();

        Label osUnsupportedLabel;
        private LinkLabel _newsfeedLinkLabelRequirements;

        private delegate void ShowPulseViewDelegate();

        public NewSQLdmTodayView()
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            InitializeComponent();

            InitializeServerListControls();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            HideFocusRectangleDrawFilter drawFilter = new HideFocusRectangleDrawFilter();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by HideFocusRectangleDrawFilter() : {0}", stopWatch.ElapsedMilliseconds);

            _contentTabControl.DrawFilter = drawFilter;
            alertsGrid.DrawFilter = drawFilter;

            osUnsupportedLabel.Text = "IDERA Newsfeed is not supported on this operating system.";
            osUnsupportedLabel.BackColor = Color.White;
            osUnsupportedLabel.AutoSize = true;
            osUnsupportedLabel.Visible = false;
            osUnsupportedLabel.Location = new Point((_pulseNewsFeedPanel.Width / 2) - osUnsupportedLabel.Width, this._pulseNewsFeedPanel.Height / 2);

            _newsfeedLinkLabelRequirements.AutoSize = true;
            _newsfeedLinkLabelRequirements.TabStop = true;
            _newsfeedLinkLabelRequirements.Text = "IDERA Newsfeed requirements";
            _newsfeedLinkLabelRequirements.Visible = false;
            _newsfeedLinkLabelRequirements.BackColor = Color.Transparent;
            _newsfeedLinkLabelRequirements.LinkClicked += new LinkLabelLinkClickedEventHandler(this.NewsfeedRequirements_LinkClicked);
            _newsfeedLinkLabelRequirements.Location = new Point(osUnsupportedLabel.Location.X + osUnsupportedLabel.Width, this._pulseNewsFeedPanel.Height / 2);

            _pulseNewsFeedPanel.AutoSize = true;
            _pulseNewsFeedPanel.Controls.AddRange(new System.Windows.Forms.Control[] { osUnsupportedLabel, _newsfeedLinkLabelRequirements });

            // Auto scale font size.
            this.AdaptFontSize();
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by NewSQLdmTodayView : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        public override void UpdateTheme(ThemeName theme)
        {
            base.UpdateTheme(theme);
            if(theme == ThemeName.Dark)
            {
                pictureBox2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmToday_6Things_Dark;
            }
            else
            {
                pictureBox2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmToday_6Things;
            }
        }

        private void NewsfeedRequirements_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.SQLdmMobileNewsfeedRequirements);//SQLdm9.0 (Gaurav Karwal): changed to use idera wiki
        }

        private void InitializeServerListControls()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _mostCriticalServersControls.Add(new Pair<PictureBox, LinkLabel>(_mostCriticalServersStatusImage1, _mostCriticalServersLinkLabel1));
            _mostCriticalServersControls.Add(new Pair<PictureBox, LinkLabel>(_mostCriticalServersStatusImage2, _mostCriticalServersLinkLabel2));
            _mostCriticalServersControls.Add(new Pair<PictureBox, LinkLabel>(_mostCriticalServersStatusImage3, _mostCriticalServersLinkLabel3));
            _mostCriticalServersControls.Add(new Pair<PictureBox, LinkLabel>(_mostCriticalServersStatusImage4, _mostCriticalServersLinkLabel4));
            _mostCriticalServersControls.Add(new Pair<PictureBox, LinkLabel>(_mostCriticalServersStatusImage5, _mostCriticalServersLinkLabel5));

            _recentServersControls.Add(new Pair<PictureBox, LinkLabel>(_recentServersStatusImage1, _recentServersLinkLabel1));
            _recentServersControls.Add(new Pair<PictureBox, LinkLabel>(_recentServersStatusImage2, _recentServersLinkLabel2));
            _recentServersControls.Add(new Pair<PictureBox, LinkLabel>(_recentServersStatusImage3, _recentServersLinkLabel3));
            _recentServersControls.Add(new Pair<PictureBox, LinkLabel>(_recentServersStatusImage4, _recentServersLinkLabel4));
            _recentServersControls.Add(new Pair<PictureBox, LinkLabel>(_recentServersStatusImage5, _recentServersLinkLabel5));
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by InitializeServerListControls : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void PulseController_MoreInfoRequested(object sender, HandledEventArgs e)
        {
            if (ApplicationController.Default.ActiveView != this || e.Handled) return;
            ShowPulseGettingStarted();
            e.Handled = true;
        }

        private void PulseController_ActivityFeedLinkClicked(object sender, EventArgs e)
        {
            ShowPulseViewDelegate showPulseViewDelegate = ShowPulseView;
            Program.MainWindow.Dispatcher.BeginInvoke(showPulseViewDelegate);
        }

        private static void ShowPulseView()
        {
            ApplicationController.Default.ShowPulseView();
        }

        //public override void UpdateTheme(ThemeName theme)
        //{
        //    if (theme == ThemeName.Dark)
        //    {
        //        this.tableLayoutPanel1.BackColor = Color.FromArgb(2,16,23);
        //        this.tableLayoutPanel2.BackColor = Color.FromArgb(2,16,23);
        //    }
        //    else
        //    {
        //        this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
        //        this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;

        //    }
            
        //}

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.SQLdmTodayView);
        }

        public override void ApplySettings()
        {
            if (Settings.Default.NewTodayViewAlertsGrid != null)
            {
                lastAlertsGridSettings = Settings.Default.NewTodayViewAlertsGrid;
                GridSettings.ApplySettingsToGrid(lastAlertsGridSettings, alertsGrid);
            }

            if (Settings.Default.TodayViewSelectedTabIndex < _contentTabControl.Tabs.Count)
                _contentTabControl.Tabs[Settings.Default.TodayViewSelectedTabIndex].Selected = true;
        }

        public override void SaveSettings()
        {
            GridSettings alertsGridSettings = GridSettings.GetSettings(alertsGrid);

            if (alertsGridSettings != null && !alertsGridSettings.Equals(lastAlertsGridSettings))
            {
                lastAlertsGridSettings =
                    Settings.Default.NewTodayViewAlertsGrid = alertsGridSettings;
            }

            if (_contentTabControl != null && _contentTabControl.SelectedTab != null)
            {
                Settings.Default.TodayViewSelectedTabIndex = _contentTabControl.SelectedTab.Index;
            }
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            UpdateStatusArea();
        }

        public override void UpdateUserTokenAttributes()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            UpdateStatusArea();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by NewSQLdmTodayView.UpdateUserTokenAttributes : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void ActiveServers_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            RefreshView();
        }

        public override void RefreshView()
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "TodayViewRefresh";
            }

            using (LOG.DebugCall("DoRefreshWork"))
            {
                if (!alertsGrid.IsDisposed)
                {
                    ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];

                    if (valueList.ValueListItems.Count == 0)
                    {
                        valueList.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());
                    }
                }

                LoadAlerts();
                _mostCriticalServers = RepositoryHelper.GetMostCriticalServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                return alertsViewDataSource;
            }
        }

        private void LoadAlerts()
        {
            using (LOG.DebugCall("LoadAlerts"))
            {
                DateTime start = DateTime.Now;
                alertsViewDataSource.SuspendBindingNotifications();
                try
                {
                    RepositoryHelper
                        .LoadDataSource(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, alertsViewDataSource,
                                        "p_GetAlerts",
                                        0, null, null, ApplicationModel.Default.UserToken.ActiveAssignedServersXmlFilter, null, null, null, null, null, true, null);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    alertsViewDataSource.ResumeBindingNotifications();
                }

                TimeSpan span = DateTime.Now - start;
                LOG.Debug("RepositoryHelper.LoadDataSource[Alerts] took this long: ", span);
            }
        }

        public override void UpdateData(object data)
        {
            using (Log.VerboseCall("UpdateData"))
            {
                DateTime lastRefresh = DateTime.Now;
                refreshDateLabel.Text = lastRefresh.ToString("dddd, MMMM d, yyyy");
                System.Globalization.DateTimeFormatInfo dateTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
                string shortTimePattern = dateTimeFormat.LongTimePattern.Replace(":ss", String.Empty);
                refreshTimeLabel.Text = lastRefresh.ToString(shortTimePattern);

                UpdateStatusArea();
                PulseController.Default.RefreshActivityFeed();

                statusSummaryContentPanel.Visible = true;

                // hide/show alerts grid 
                if (alertsViewDataSource.Rows.Count == 0)
                    alertsStatusLabel.BringToFront();
                else
                    alertsGrid.BringToFront();

                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(lastRefresh));
            }
        }

        private void UpdateStatusArea()
        {
            using (Log.VerboseCall("UpdateStatusArea"))
            {
                ApplicationModel model = ApplicationModel.Default;
                statusSummaryContentPanel.Visible = true;//SqlDm 10.2 (Tushar)--Making content panel visible.
                statusSummaryLabel.Enabled = true;
                statusSummaryDescriptionLabel.Enabled = true;

                //SqlDm 10.2 (Tushar)--When active instances are getting loaded then displaying loading label.
                //SqlDM 10.2 (Tushar)--Fix for SQLDM-27156--When repository is not reachable then showing the standard "No servers" text.
                if ((model.ActiveInstances.Count == 0 && Settings.Default.ActiveRepositoryConnection.RepositoryInfo !=null && Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)||
                    (model.ActiveInstances.Count==0 && Settings.Default.ActiveRepositoryConnection.RepositoryInfo == null))
                {
                    statusSummaryLoadingLabel.Visible = false;//SqlDm 10.2 (Tushar)--When active instances are loaded then hiding loading label.
                    if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                    {
                        statusSummaryLabel.Text = "No Monitored Servers";
                        statusSummaryDescriptionLabel.Text = "Click here to add servers now.";
                    }
                    else
                    {
                        statusSummaryLabel.Text = "No Servers Available";
                        statusSummaryLabel.Enabled = false;
                        statusSummaryDescriptionLabel.Text =
                            "You do not have access to monitor any servers at this time.";
                        statusSummaryDescriptionLabel.Enabled = false;
                    }

                    statusSummaryPictureBox.Image =
                        Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                    statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                    statusSummaryDescriptionLabel.LinkColor = Color.FromArgb(0, 0, 255);
                    criticalServersPictureBox.Visible = false;
                    criticalServersLabel.Visible = false;
                    warningServersPictureBox.Visible = false;
                    warningServersLabel.Visible = false;
                    maintenanceModeServersPictureBox.Visible = false;
                    maintenanceModeServersLabel.Visible = false;
                    okServersPictureBox.Visible = false;
                    okServersLabel.Visible = false;
                }
                else
                {
                    if (model.ActiveInstances.Count > 0)
                    {
                        statusSummaryContentPanel.Visible = true;
                        statusSummaryLoadingLabel.Visible = false;
                        maintenanceModeServers = 0;
                        criticalServers = 0;
                        warningServers = 0;
                        okServers = 0;

                        foreach (MonitoredSqlServerWrapper instance in Algorithms.ToArray(model.ActiveInstances))
                        {
                            MonitoredSqlServerStatus status = model.GetInstanceStatus(instance.Id);
                            if (status != null)
                            {
                                if (status.IsInMaintenanceMode)
                                    maintenanceModeServers++;
                                else
                                {
                                    switch (status.Severity)
                                    {
                                        case MonitoredState.Critical:
                                            criticalServers++;
                                            break;
                                        case MonitoredState.Warning:
                                            warningServers++;
                                            break;
                                        default:
                                            okServers++;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (instance.MaintenanceModeEnabled)
                                    maintenanceModeServers++;
                                else
                                    okServers++;
                            }
                        }
                        if (criticalServers > 0)
                        {
                            statusSummaryPictureBox.Image =
                                Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryCriticalLarge;
                            statusSummaryLabel.Text = "Critical";
                            statusSummaryLabel.LinkColor = Color.Red;
                            statusSummaryDescriptionLabel.Text =
                                String.Format(CriticalServersLabelText,
                                              criticalServers);
                            statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
                        }
                        else if (warningServers > 0)
                        {
                            statusSummaryPictureBox.Image =
                                Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryWarningLarge;
                            statusSummaryLabel.Text = "Warning";
                            statusSummaryLabel.LinkColor = Color.Black;
                            statusSummaryDescriptionLabel.Text =
                                String.Format(WarningServersLabelText,
                                              warningServers);
                            statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
                        }
                        else if (okServers == 0 && maintenanceModeServers > 0)
                        {
                            statusSummaryPictureBox.Image =
                                Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                            statusSummaryLabel.Text = "Maintenance Mode";
                            statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                            statusSummaryDescriptionLabel.Text = "All servers are in maintenance mode.";
                            statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
                        }
                        else if (okServers == 0)
                        {
                            statusSummaryPictureBox.Image =
                                Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryInformationLarge;
                            statusSummaryLabel.Text = "No Instances";
                            statusSummaryLabel.LinkColor = Color.FromArgb(0, 0, 255);
                            statusSummaryDescriptionLabel.Text = "There are no SQL Server instances being monitored.";
                            statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
                        }
                        else
                        {
                            statusSummaryPictureBox.Image =
                                Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryOKLarge;
                            statusSummaryLabel.Text = "OK";
                            statusSummaryLabel.LinkColor = Color.ForestGreen;
                            statusSummaryDescriptionLabel.Text = "All servers are operating normally.";
                            statusSummaryDescriptionLabel.LinkColor = SystemColors.ControlText;
                        }

                        criticalServersPictureBox.Visible = true;
                        criticalServersLabel.Text = string.Format(CriticalServersLabelText, criticalServers);
                        criticalServersLabel.Visible = true;
                        warningServersPictureBox.Visible = true;
                        warningServersLabel.Text = string.Format(WarningServersLabelText, warningServers);
                        warningServersLabel.Visible = true;
                        maintenanceModeServersPictureBox.Visible = true;
                        maintenanceModeServersLabel.Text = string.Format(MaintenanceModeServersLabelText,
                                                                         maintenanceModeServers);
                        maintenanceModeServersLabel.Visible = true;
                        okServersPictureBox.Visible = true;
                        okServersLabel.Text = string.Format(OkServersLabelText, okServers);
                        okServersLabel.Visible = true;
                    }
                    else
                    {
                        //SqlDm 10.2 (Tushar)--When active instances are getting loaded then displaying loading label and hiding the content panel.
                        statusSummaryContentPanel.Visible = false;
                        statusSummaryLoadingLabel.Visible = true;
                    }
                }

                UpdateMostCriticalServers();
                UpdateRecentServers();
            }
        }

        private void UpdateMostCriticalServers()
        {
            using (Log.VerboseCall("UpdateMostCriticalServers"))
            {
                _mostCriticalServersPanel.Visible = ApplicationModel.Default.ActiveInstances.Count > 0 &&
                                                    _mostCriticalServers != null && _mostCriticalServers.Count > 0;

                if (ApplicationModel.Default.ActiveInstances.Count == 0 ||
                    _mostCriticalServers == null ||
                    _mostCriticalServers.Count == 0) return;

                int serverCount = 0;

                foreach (int serverId in _mostCriticalServers)
                {
                    if (!ApplicationModel.Default.ActiveInstances.Contains(serverId)) continue;

                    MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[serverId];
                    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);

                    if (status == null || status.Severity == MonitoredState.OK) continue;

                    serverCount++;

                    int index = serverCount - 1;
                    _mostCriticalServersControls[index].First.Visible =
                        _mostCriticalServersControls[index].Second.Visible = true;

                    if (!status.IsInMaintenanceMode)
                    {
                        switch (status.Severity)
                        {
                            case MonitoredState.Critical:
                                _mostCriticalServersControls[index].First.Image = Resources.StatusCriticalSmall;
                                break;
                            case MonitoredState.Warning:
                                _mostCriticalServersControls[index].First.Image = Resources.StatusWarningSmall;
                                break;
                            default:
                                _mostCriticalServersControls[index].First.Image = Resources.StatusOKSmall;
                                break;
                        }
                    }
                    else
                    {
                        _mostCriticalServersControls[index].First.Image = Resources.MaintenanceMode16x16;
                    }

                    _mostCriticalServersControls[index].Second.Text = instance.InstanceName;
                    _mostCriticalServersControls[index].Second.Tag = instance.Id;

                    if (serverCount == 5) break;
                }

                for (int i = serverCount; i < 5; i++)
                {
                    _mostCriticalServersControls[i].First.Visible =
                        _mostCriticalServersControls[i].Second.Visible = false;
                }

                _mostCriticalServersPanel.Height = 35 + (25 * serverCount);
            }
        }

        private void UpdateRecentServers()
        {
            using (Log.VerboseCall("UpdateRecentServers"))
            {
                _recentServersPanel.Visible = ApplicationModel.Default.ActiveInstances.Count > 0 &&
                                              Settings.Default.RecentServers.Count > 0;

                if (ApplicationModel.Default.ActiveInstances.Count == 0 ||
                    Settings.Default.RecentServers.Count == 0) return;

                Settings.Default.SyncRecentServers();

                for (int i = 0; i < Settings.Default.RecentServers.Count; i++)
                {
                    _recentServersControls[i].First.Visible =
                        _recentServersControls[i].Second.Visible = true;

                    MonitoredSqlServerWrapper instance =
                        ApplicationModel.Default.ActiveInstances[Settings.Default.RecentServers[i]];
                    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);

                    if (status != null && !status.IsInMaintenanceMode)
                    {
                        switch (status.Severity)
                        {
                            case MonitoredState.Critical:
                                _recentServersControls[i].First.Image = Resources.StatusCriticalSmall;
                                break;
                            case MonitoredState.Warning:
                                _recentServersControls[i].First.Image = Resources.StatusWarningSmall;
                                break;
                            default:
                                _recentServersControls[i].First.Image = Resources.StatusOKSmall;
                                break;
                        }
                    }
                    else
                    {
                        _recentServersControls[i].First.Image = (status != null) ? Resources.MaintenanceMode16x16 : Resources.StatusOKSmall;
                    }

                    _recentServersControls[i].Second.Text = instance.InstanceName;
                    _recentServersControls[i].Second.Tag = instance.Id;
                }

                for (int i = Settings.Default.RecentServers.Count; i < 5; i++)
                {
                    _recentServersControls[i].First.Visible =
                        _recentServersControls[i].Second.Visible = false;
                }
            }
        }

        private void addServersFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            ShowManageServersDialog();
        }

        private void ShowManageServersDialog()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                ManageServersDialog manageServersDialog = new ManageServersDialog();
                manageServersDialog.ShowDialog(ParentForm);
            }
        }

        private void monitorAlertLogFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void generateReportsFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void NewSQLdmTodayView_Load(object sender, EventArgs e)
        {
            using (Log.VerboseCall("NewSQLdmTodayView_Load"))
            {
                ApplicationModel.Default.ActiveInstances.Changed += ActiveServers_Changed;
                ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;

                Version osversion = Environment.OSVersion.Version;
                if (osversion.Major == 5 && osversion.Minor == 0)
                {
                    osUnsupportedLabel.Visible = true;
                    osUnsupportedLabel.BringToFront();
                    _newsfeedLinkLabelRequirements.Visible = true;
                    _newsfeedLinkLabelRequirements.BringToFront();
                }
                else
                {
                    PulseController.Default.IntializeActivityFeed(_pulseNewsFeedPanel, Settings.Default.PulseAccountName,
                                                                  Settings.Default.PulseAccountKeepLoggedIn);
                }

                PulseController.Default.MoreInfoRequested += PulseController_MoreInfoRequested;
                PulseController.Default.ActivityFeedLinkClicked += PulseController_ActivityFeedLinkClicked;

                //START: SQLdm 9.0 (Abhishek Joshi) -Get the web console URL from the management service and mark the link area before appending text
                int webConsoleLinkStartPositon = this.webConsoleLinkLabel.Text.Length;
                string webConsoleURL = FetchWebConsoleURL();
                this.webConsoleLinkLabel.Text += webConsoleURL;
                webConsoleLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(webConsoleLinkStartPositon, webConsoleURL.Length);
                webConsoleLinkLabel.Text += " to launch the web console.";
                if (string.IsNullOrEmpty(webConsoleURL))
                    webConsoleLinkLabel.Visible = false;
                else
                    webConsoleLinkLabel.Visible = true;//SQLdm 9.0 (vineet kumar) -- Fixing DE44333
                this.label19_1.Location = new System.Drawing.Point(5, 15);
                this.label19_2.Location = new System.Drawing.Point(this.label19_1.Width - 5, 15);
                this.label19_2.BringToFront();
                //END: SQLdm 9.0 (Abhishek Joshi) -Get the web console URL from the management service and mark the link area before appending text

                ConfigureAlertsGrid();
                ApplySettings();
            }
        }

        //START: SQLdm 9.0 (Abhishek Joshi) -Get the web console URL from the management service
        private string FetchWebConsoleURL()
        {
            string webConsoleURL = String.Empty;
            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                webConsoleURL = managementService.GetCWFWebURL();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to fetch the Web Console URL from Management Service => " + ex.InnerException);
            }

            return webConsoleURL;
        }
        //END: SQLdm 9.0 (Abhishek Joshi) -Get the web console URL from the management service

        private void ConfigureAlertsGrid()
        {
            alertsGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

            // switch out editor from EditorWithCombo to EditorWithText (fixes tooltip issue)
            EditorWithText textEditor = new EditorWithText();

            UltraGridColumn column = alertsGrid.DisplayLayout.Bands[0].Columns["Severity"];
            column.Editor = textEditor;
            column.Width = 24;
            // set the header to a bold red ! and center it
            column.Header.Caption = "!";
            column.Header.Appearance.TextHAlign = HAlign.Center;
            column.Header.Appearance.ForeColor = Color.Red;
            column.Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            // center the images in the cells
            column.CellAppearance.ImageHAlign = HAlign.Center;

            column = alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"];
            column.Editor = textEditor;
            //column.Width = 24;
            // set the header to a delta character and center it
            //column.Header.Caption = DELTA_STRING;
            //column.Header.Appearance.TextHAlign = HAlign.Center;
            // center the images in the cells
            //column.CellAppearance.ImageHAlign = HAlign.Center;

            UltraGridBand band = alertsGrid.DisplayLayout.Bands[0];
            band.SortedColumns.Clear();
            band.SortedColumns.Add("UTCOccurrenceDateTime", true);
            band.SortedColumns.Add("Severity", true);
            band.SortedColumns.Add("ServerName", false);

            // build the severity value list
            ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetSeverityValueListItems());

            // load the transition value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTransitionValueListItems());

            // populate the metric value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
            valueList.ValueListItems.Clear();
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridContextMenu")
            {
                if (alertsGrid.Selected.Rows.Count > 0)
                {
                    bool isViewRealTimeSnapshotButtonEnabled = false;
                    bool isViewHistoricalSnapshotButtonEnabled = false;
                    bool isViewAlertHelpButtonEnabled = false;
                    bool isViewDeadlockDetailsButtonEnabled = false;

                    object metricColumnValue = alertsGrid.Selected.Rows[0].Cells["Metric"].Value;
                    if (metricColumnValue != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                        if (m == Metric.Deadlock)
                        {
                            isViewDeadlockDetailsButtonEnabled = true;
                            isViewRealTimeSnapshotButtonEnabled = false;
                        }
                        else
                        {
                            isViewRealTimeSnapshotButtonEnabled = m != Metric.Operational && m != Metric.SQLdmCollectionServiceStatus;
                        }

                        isViewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                        isViewAlertHelpButtonEnabled = isViewRealTimeSnapshotButtonEnabled && m != Metric.Custom;
                    }

                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible =
                        isViewDeadlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertRealTimeSnapshotButton"].SharedProps.Visible =
                        isViewRealTimeSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHistoricalSnapshotButton"].SharedProps.Visible =
                        isViewHistoricalSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible =
                        isViewAlertHelpButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHistoricalSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible = false;
                }
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
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "configureAlertsButton":
                    ConfigureAlerts();
                    break;
                case "snoozeAlertButton":
                    SnoozeAlert();
                    break;
                case "copyToClipboardButton":
                    UltraGridHelper.CopyToClipboard(selectedGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "clearAlertButton":
                    ClearAlert(false);
                    break;
                case "clearAllAlertsButton":
                    ClearAlert(true);
                    break;
                case "viewAlertRealTimeSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], false);
                    break;
                case "viewAlertHistoricalSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], true);
                    break;
                case "viewAlertHelpButton":
                    ShowAlertHelp();
                    break;
                case "viewDeadlockDetailsButton":
                    ShowDeadlockDetails();
                    break;
            }
        }

        private void ShowDeadlockDetails()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.", e);
                return;
            }

            Log.Verbose("Showing detailsContentPanel, alert id = ", alertId);
            DeadlockDialog.Show(this, alertId);
        }

        private void ShowAlertHelp()
        {
            if (alertsGrid.Selected.Rows.Count != 0)
            {
                // Get instance id and metric for the selected alert.
                if (alertsGrid.Selected.Rows[0].Cells["ServerName"].Value != null
                    && alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                {
                    // Get instance and metric.
                    string serverName = alertsGrid.Selected.Rows[0].Cells["ServerName"].Value as string;
                    int metricId = (int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value;
                    MonitoredSqlServerWrapper serverWrapper = ApplicationModel.Default.ActiveInstances[serverName];
                    Metric metric = MetricDefinition.GetMetric(metricId);

                    // If server and metric are valid, then proceed, else do nothing.
                    if (serverWrapper != null && metric != Metric.Custom)
                    {
                        string helpTopic;
                        if (ClickThroughHelper.GetMetricHelp(metric, out helpTopic))
                        {
                            ApplicationHelper.ShowHelpTopic(helpTopic);
                        }
                    }
                }
            }
        }

        private void SnoozeAlert()
        {
            if (alertsGrid.Selected.Rows.Count == 1)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];
                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    string selectedServerName = (string)dataRow["ServerName"] ?? String.Empty;
                    MonitoredSqlServerWrapper selectedServer =
                        ApplicationModel.Default.ActiveInstances[selectedServerName];

                    // Get Alert Summary
                    String alertSummary = dataRow["Heading"] as String;

                    // Displays snooze dialog.
                    SnoozeInfo snoozeInfo = SnoozeAlertsDialog.SnoozeAlert(
                        ParentForm,
                        selectedServer.Id,
                        (int)metric,
                        SnoozeAlertsDialog.SnoozeAction.Snooze,
                        alertSummary);

                    if (snoozeInfo != null)
                    {
                        dataRow.ParentCollection.Remove(dataRow);
                    }
                }
            }
        }

        private void ClearAlert(bool allAlerts)
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];
                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    switch (m)
                    {
                        case Metric.ReorganisationPct:
                        case Metric.BombedJobs:
                        case Metric.LongJobs:
                        case Metric.LongJobsMinutes:
                        case Metric.ClusterFailover:
                        case Metric.JobCompletion:
                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                            long alertID = (long)dataRow["AlertID"];
                            try
                            {
                                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                                managementService.ClearActiveAlerts(alertID, allAlerts);

                                if (allAlerts)
                                {
                                    string selectedServer = dataRow["ServerName"] as string ?? "";

                                    List<UltraDataRow> rows = new List<UltraDataRow>();
                                    foreach (UltraDataRow udr in dataRow.ParentCollection)
                                    {
                                        if (((bool)udr["Active"]) && metric.Equals(udr["Metric"]))
                                        {
                                            string rowServer = udr["ServerName"] as string ?? "";
                                            if (selectedServer.Equals(rowServer))
                                                rows.Add(udr);
                                        }
                                    }
                                    foreach (UltraDataRow udr in rows)
                                    {
                                        udr.ParentCollection.Remove(udr);
                                    }
                                }
                                else
                                {
                                    dataRow.ParentCollection.Remove(dataRow);
                                }
                            }
                            catch (Exception e)
                            {
                                ApplicationMessageBox.ShowError(this, e);
                            }

                            break;
                    }
                }
            }
        }

        private void ConfigureAlerts()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object server = dataRow["ServerName"];
                if (server is string && !String.IsNullOrEmpty((string)server))
                {
                    MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[(string)server];
                    if (wrapper != null)
                    {
                        try
                        {
                            using (
                                AlertConfigurationDialog alertConfigDialog =
                                    new AlertConfigurationDialog(wrapper.Id, false))
                            {
                                object metric = dataRow["Metric"];
                                if (metric is int)
                                    metric = (Metric)Enum.ToObject(typeof(Metric), ((int)metric));

                                alertConfigDialog.CreateControl();
                                if (metric is Metric)
                                {
                                    alertConfigDialog.Select((Metric)metric);
                                }

                                alertConfigDialog.ShowDialog(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }
                    }
                }
            }
        }

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

        private void NavigateToView(UltraGridRow row, bool showHistoricalSnapshot)
        {
            if (row != null)
            {
                string instanceName = row.Cells["ServerName"].Value as string;
                object argument = null;

                if (row.Cells["Metric"].Value != null)
                {
                    Metric metric = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
                    //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup from filegroup alert message
                    string filegroup = GetFilegroupFromAlertMessage((string)row.Cells["Message"].Value);

                    switch (metric)
                    {
                        case Metric.ReorganisationPct:
                            argument = row.Cells["DatabaseName"].Value as object;
                            break;
                        case Metric.PageLifeExpectancy:
                            argument = Metric.PageLifeExpectancy;
                            break;
                        // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                        case Metric.FilegroupSpaceFullSize:
                        case Metric.FilegroupSpaceFullPct:
                            argument = (object)(new object[] { row.Cells["DatabaseName"].Value, row.Cells["Metric"].Value, filegroup });
                            break;
                        // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                        default:
                            argument = null;
                            break;
                    }

                    if (showHistoricalSnapshot)
                    {
                        DateTime? snapshotDateTime = null;

                        if (row.Band.Layout.Grid == alertsGrid)
                        {
                            snapshotDateTime = (DateTime)row.Cells["UTCOccurrenceDateTime"].Value;
                        }
                        ClickThroughHelper.NavigateToView(instanceName, metric, snapshotDateTime, argument);
                    }
                    else
                    {
                        if (metric == Metric.Deadlock)
                        {
                            ShowDeadlockDetails();
                            return;
                        }
                        ClickThroughHelper.NavigateToView(instanceName, metric, null, argument);
                    }
                }
            }
        }

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message
        private string GetFilegroupFromAlertMessage(string message)
        {
            string filegroup = null;
            string referenceString = "has a file group ";

            if (message.Length > 0)
            {
                int startIdx = message.IndexOf(referenceString) + referenceString.Length + 1;
                int lastIdx = message.LastIndexOf(']');
                if (startIdx < lastIdx)
                    filegroup = message.Substring(startIdx, lastIdx - startIdx);
            }

            return filegroup;
        }
        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message

        #region grid

        private void PrintGrid()
        {
            if (selectedGrid != null)
            {
                ultraGridPrintDocument.Grid = selectedGrid;
                ultraGridPrintDocument.DefaultPageSettings.Landscape = false;
                ultraGridPrintDocument.Header.TextLeft = ultraGridPrintDocument.Header.TextLeft =
                                                         string.Format("{0} as of {1}",
                                                                       ((HeaderLabel)selectedGrid.Tag).HeaderText,
                                                                       DateTime.Now.ToString("G")
                                                             );

                ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

                ultraPrintPreviewDialog.ShowDialog();
            }
        }

        private void SaveGrid()
        {
            if (selectedGrid != null)
            {
                string severityCol = "Severity";
                string statusCol = "Status";
                if (selectedGrid == alertsGrid)
                {
                    statusCol = "StateEvent";
                }
                ValueListDisplayStyle severityStyle =
                    ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle;
                ValueListDisplayStyle statusStyle =
                    ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle;

                saveFileDialog.FileName =
                    ExportHelper.GetValidFileName(((HeaderLabel)selectedGrid.Tag).HeaderText, true);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle =
                            ValueListDisplayStyle.DisplayText;
                        ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle =
                            ValueListDisplayStyle.DisplayText;
                        ultraGridExcelExporter.Export(selectedGrid, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                    }

                    ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[severityCol].ValueList).DisplayStyle =
                        severityStyle;
                    ((ValueList)selectedGrid.DisplayLayout.Bands[0].Columns[statusCol].ValueList).DisplayStyle =
                        statusStyle;
                }
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            if (selectedGrid != null)
            {
                selectedGrid.DisplayLayout.GroupByBox.Hidden = !selectedGrid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                if (GroupBy)
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedGrid != null && selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void ShowColumnChooser()
        {
            if (selectedGrid != null)
            {
                SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(selectedGrid);
                dialog.Show(this);
            }
        }

        #endregion

        private void alertsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = (UltraGrid)sender;
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (!(contextObject is ColumnHeader))
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        // the user clicked a row
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");

                        RowUIElement rowElement = contextObject as RowUIElement;
                        UltraGridRow row = rowElement.Row;

                        // select the row that was clicked
                        alertsGrid.Selected.Rows.Clear();
                        row.Activate();
                        row.Selected = true;

                        // set the alert configuration edit button enabled if the selected row has a server name
                        UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                        object server = dataRow["ServerName"];
                        object metric = dataRow["Metric"];
                        ButtonTool tool = (ButtonTool)toolbarsManager.Tools["configureAlertsButton"];
                        tool.SharedProps.Visible = (server != null && !"".Equals(server.ToString()));

                        bool enableSnoozeButton = false;
                        if (alertsGrid.Selected.Rows.Count == 1)
                        {
                            if (server != null && !"".Equals(server.ToString()))
                            {
                                int id = ApplicationModel.Default.ActiveInstances[server as string].Id;
                                if (metric is int && !((int)metric == (int)Metric.MaintenanceMode || (int)metric == (int)Metric.Operational))
                                    enableSnoozeButton = ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify;
                            }
                        }
                        tool = (ButtonTool)toolbarsManager.Tools["snoozeAlertButton"];
                        tool.SharedProps.Visible = enableSnoozeButton;

                        int instanceId = ApplicationModel.Default.ActiveInstances[server as string].Id;
                        bool enableClearOptions = false;

                        if (alertsGrid.Selected.Rows.Count > 0)
                        {
                            object active = dataRow["Active"];
                            if (active is bool && ((bool)active))
                            {
                                if (metric is int)
                                {
                                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                                    switch (m)
                                    {
                                        case Metric.ReorganisationPct:
                                        case Metric.BombedJobs:
                                        case Metric.LongJobs:
                                        case Metric.LongJobsMinutes:
                                        case Metric.ClusterFailover:
                                        case Metric.JobCompletion:
                                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                                            enableClearOptions = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                                            break;
                                    }
                                }
                            }
                        }
                        toolbarsManager.Tools["clearAlertButton"].SharedProps.Visible = enableClearOptions;
                        toolbarsManager.Tools["clearAllAlertsButton"].SharedProps.Visible = enableClearOptions;
                    }
                    else
                    {
                        // if the user clicked in the open area of a grid (after the last row) then there is no menu
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), null);
                    }
                }
            }
        }

        private void alertsGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            NavigateToView(e.Row, false);
        }

        private void ServersLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // if there are no instances defined and the user clicks a label - launch the add servers wizard
            if (ApplicationModel.Default.ActiveInstances.Count == 0)
            {
                ShowManageServersDialog();
                return;
            }

            Guid gotoViewID = Guid.Empty;

            // determine the user view and take the user there
            if (sender == criticalServersLabel)
                gotoViewID = SearchUserView.CriticalUserViewID;
            else if (sender == warningServersLabel)
                gotoViewID = SearchUserView.WarningUserViewID;
            else if (sender == okServersLabel)
                gotoViewID = SearchUserView.OKUserViewID;
            else if (sender == maintenanceModeServersLabel)
                gotoViewID = SearchUserView.MaintenanceModeUserViewID;
            else if (sender == statusSummaryLabel || sender == statusSummaryDescriptionLabel)
            {
                if (criticalServers > 0)
                    gotoViewID = SearchUserView.CriticalUserViewID;
                else if (warningServers > 0)
                    gotoViewID = SearchUserView.WarningUserViewID;
                else if (maintenanceModeServers > 0)
                    gotoViewID = SearchUserView.MaintenanceModeUserViewID;
                else if (okServers > 0)
                    gotoViewID = SearchUserView.OKUserViewID;
            }

            ApplicationController.Default.ShowUserView(gotoViewID);
        }

        private void _facebookLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.facebook.com/IderaSoftware");
        }

        private void _twitterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://twitter.com/idera_software");
        }

        //START: SQLdm 9.0 (Abhishek Joshi) -Opening Web Console Link and Wiki Idera link from SQLdmToday page in Desktop Client
        private void webConsoleLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string webConsoleURL = this.webConsoleLinkLabel.Text.Substring(this.webConsoleLinkLabel.LinkArea.Start, this.webConsoleLinkLabel.LinkArea.Length);
            Process.Start(webConsoleURL);
        }
        //END: SQLdm 9.0 (Abhishek Joshi) -Opening Web Console Link and Wiki Idera link from SQLdmToday page in Desktop Client

        private void ShowPulseGettingStarted()
        {
            PulseGettingStartedDialog dialog = new PulseGettingStartedDialog(ManagementServiceHelper.ServerName);
            dialog.ShowInTaskbar = true;

            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator && !ApplicationModel.Default.IsPulseConfigured)
            {
                dialog.AddConfigurationStep(new PulsePlatformConnectionConfigurationControl());
                dialog.AddConfigurationStep(new PulseNotificationsConfigurationControl());
                dialog.SetMode(GettingStartedMode.Administrator);
            }
            else if (ApplicationModel.Default.IsPulseConfigured)
            {
                dialog.SetMode(GettingStartedMode.SignUp);
            }
            else
            {
                dialog.SetMode(GettingStartedMode.LearnMoreOnly);
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ApplicationController.Default.ShowPulseView();
            }
        }

        private void _pulseGetStartedButton_MouseClick(object sender, MouseEventArgs e)
        {
            ShowPulseGettingStarted();
        }

        private void ServerLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!(sender is LinkLabel)) return;
            LinkLabel serverLinkLabel = sender as LinkLabel;
            ApplicationController.Default.ShowServerView((int)serverLinkLabel.Tag);
        }

        private void _trialCenterControl_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void _trialCenterControl_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.LaunchTrialCenterUrl();
        }
        private void pulseNewsFeddPanel_Resize(object sender, EventArgs e)
        {
            if (_pulseNewsFeedPanel != null && osUnsupportedLabel != null && _newsfeedLinkLabelRequirements != null)
            {
                if (_pulseNewsFeedPanel.Width > osUnsupportedLabel.Width + _newsfeedLinkLabelRequirements.Width)
                {
                    osUnsupportedLabel.Location =
                        new Point(
                            (_pulseNewsFeedPanel.Width / 2) -
                            ((osUnsupportedLabel.Width + _newsfeedLinkLabelRequirements.Width) / 2),
                            this._pulseNewsFeedPanel.Height / 2); //JSFIX
                    _newsfeedLinkLabelRequirements.Location =
                        new Point(osUnsupportedLabel.Location.X + osUnsupportedLabel.Width,
                                  this._pulseNewsFeedPanel.Height / 2);
                }
            }
        }

        private void AdaptFontSize()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by AdaptFontSize : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void lbl_feedback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://community.idera.com/DM85TechPreview");
        }
    }
}
