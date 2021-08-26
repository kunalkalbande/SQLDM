using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win.AppStyling;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.MessageBox;
using Constants=Idera.SQLdm.Common.Constants;
using Resources=Idera.SQLdm.DesktopClient.Properties.Resources;

namespace Idera.SQLdm.DesktopClient
{
    public partial class MainForm : Form
    {
        private const string ApplicationTitleText = "{0} ({2}-{1})";

        private static readonly Logger Log = Logger.GetLogger("MainForm");

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;

        private FormWindowState restoreWindowState = FormWindowState.Normal;
        private Rectangle restoreBounds;

        private Control focused = null;
        private Exception refreshError = null;

        private bool alreadydoingit = false;
        private readonly NotificationPopupWindow notificationPopupWindow = new NotificationPopupWindow();
        private readonly Dictionary<int, MonitoredState> instanceStateTracking = new Dictionary<int, MonitoredState>();

        public MainForm()
        {
            Log.Info("Constructing MainForm.");
            InitializeComponent();
            Icon = Resources.AppIcon;
            trayIcon.Icon = Resources.App16Icon;
            Text = Application.ProductName;
            ((StateButtonTool)toolbarsManager.Tools["pauseActiveViewRefreshButton"]).Checked = false;

            NavigationPaneVisible = Settings.Default.NavigationPaneVisible;
            ((StateButtonTool)(toolbarsManager.Tools["toggleRibbonVisibilityButton"])).Checked =
                Settings.Default.CollapseRibbon;

            WindowState = (Settings.Default.MainFormState == (int)FormWindowState.Maximized)
                                 ? FormWindowState.Maximized
                                 : FormWindowState.Normal;

            UpdateRepositoryConnectionStatus();
            UpdateTools();

            //Settings.Default.PropertyChanged += Settings_PropertyChanged;

        //    ApplicationController.Default.Initialize(this, navigationPaneControl, viewContentPanel);
            //Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;
            //ApplicationModel.Default.FocusObjectChanged += ApplicationModel_FocusObjectChanged;
            //ApplicationController.Default.RefreshActiveViewStarted += ApplicationController_RefreshActiveViewStarted;
            //ApplicationController.Default.RefreshActiveViewCompleted += ApplicationController_RefreshActiveViewCompleted;
            //ApplicationController.Default.BackgroundRefreshCompleted += ApplicationController_BackgroundRefreshCompleted;
            //ApplicationController.Default.ActiveViewChanged += ApplicationController_ActiveViewChanged;


            // BUG: There appears to be a bug in the Infragistics status bar that causes
            // an exception when the cursor hovers over it before it's fully rendered. So,
            // we need to hide it initially and show it when the form is shown.
            statusBar.Visible = false;
            Shown += MainForm_Shown;
        }

        public bool NavigationPaneVisible
        {
            get { return Settings.Default.NavigationPaneVisible; }
            set
            {
                alreadydoingit = true;
                Settings.Default.NavigationPaneVisible = value;
                contentPanelSplitter.Collapsed = !Settings.Default.NavigationPaneVisible;

                if (Settings.Default.NavigationPaneVisible)
                {
                    ((StateButtonTool) (toolbarsManager.Tools["showNavigationPaneButton"])).Checked = true;
                    ((StateButtonTool)(toolbarsManager.Tools["navigationPaneVisibilityButton"])).Checked = false;
                }
                else
                {
                    ((StateButtonTool) (toolbarsManager.Tools["hideNavigationPaneButton"])).Checked = true;
                    ((StateButtonTool)(toolbarsManager.Tools["navigationPaneVisibilityButton"])).Checked = true;
                }
                alreadydoingit = false;
            }
        }

        protected override void OnResizeBegin(EventArgs e)
        {
       //     contentPanelSplitContainer.SuspendLayout();
            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
     //       contentPanelSplitContainer.ResumeLayout();
            base.OnResizeEnd(e);
        }

        void contentPanelSplitter_CollapsedChanged(object sender, System.EventArgs e)
        {
            if (!alreadydoingit)
                NavigationPaneVisible = !contentPanelSplitter.Collapsed;
        }
 
        /// <summary>
        /// Handles the LocationChanged event of the contentPanel control and resize trialImage according to the new available space.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected void contentPanel_LocationChanged(object sender, EventArgs e)
        {
            this.ResizeTrialImage();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // BUG: There appears to be a bug in the Infragistics status bar that causes
            // an exception when the cursor hovers over it before it's fully rendered. So,
            // we need to hide it initially and show it when the form is shown.
            statusBar.Visible = true;

            // When changing Windows FontSize to Large, the trialImage is growing out of the allowed bounds
            // Therefor it is necessary to resize the image to avoid this type of issues
            this.ResizeTrialImage();

            // The LocationChanged event is being binded here because we only need to follow this action
            // from here.
            this.contentPanel.LocationChanged += new EventHandler(contentPanel_LocationChanged);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            Point location = Settings.Default.MainFormLocation;
            Size size = Settings.Default.MainFormSize;
            Rectangle bounds = new Rectangle(location, size);

            Screen screen = Screen.FromPoint(location);
            Rectangle workingArea = screen.WorkingArea;
            if (bounds.Right > workingArea.Right)
            {
                if (bounds.Width > workingArea.Width)
                {
                    bounds.Width = workingArea.Width;
                    bounds.X = workingArea.X;
                }
                else
                {
                    bounds.X -= (bounds.Right - workingArea.Right);
                    if (bounds.X < workingArea.X)
                    {
                        bounds.Width = workingArea.Width;
                        bounds.X = workingArea.X;
                    }
                }
            }

            if (bounds.Bottom > workingArea.Bottom)
            {
                if (bounds.Height > workingArea.Height)
                {
                    bounds.Height = workingArea.Height;
                    bounds.Y = workingArea.Y;
                }
                else
                {
                    bounds.Y -= (bounds.Bottom - workingArea.Bottom);
                    if (bounds.Y < workingArea.Y)
                    {
                        bounds.Height = workingArea.Height;
                        bounds.Y = workingArea.Y;
                    }
                }
            }

            Bounds = bounds;

            trayIcon.Visible = true;

            // start the foreground and background refresh timers 
 //           UpdateBackgroundRefreshTimer();
 //           UpdateForegroundRefreshTimer(true);

            //try
            //{
            //    LicenseSummary licenseInfo = RepositoryHelper.GetLicenseKeys(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            //    if (licenseInfo != null)
            //    {
            //        trialCenterPicture.Visible = licenseInfo.IsTrial;

            //        if (trialCenterPicture.Visible)
            //        {
            //            // reparent the trial center picture to the toolbar docking area so the transparent background comes from the correct parent
            //            trialCenterPicture.Parent = null;
            //            trialCenterPicture.BackgroundImage = Settings.Default.ColorScheme.Contains("Black")
            //               ? Resources.trialcentergraphic
            //               : Resources.trialcentergraphic_dark;
            //            _MainFormClassic_Toolbars_Dock_Area_Top.Controls.Add(trialCenterPicture);
            //            trialCenterPicture.Top = SystemInformation.CaptionHeight + 5;
            //        }
            //    }
            //    else
            //        trialCenterPicture.Visible = false;
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("Could not validate license (for trial center link).", ex);
            //    trialCenterPicture.Visible = false;
            //}

 //           Idera.Newsfeed.Plugins.UI.PulseController.StartClipboardListener(this);

            // selected pane is deferred so that we don't start trying to connect to the repository before its time
 //           this.navigationPaneControl.SelectedPane = Idera.SQLdm.DesktopClient.Controls.NavigationPane.NavigationPanes.Servers;

 //           if (ApplicationController.Default.ActiveView == null)
 //               ApplicationController.Default.ShowTodayView();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Log.Info("Closing MainForm.");
            //Idera.Newsfeed.Plugins.UI.PulseController.StopClipboardListener(this);
            //Settings.Default.MainFormSize = restoreBounds.Size;
            //Settings.Default.MainFormLocation = restoreBounds.Location;
            //Settings.Default.MainFormState = (int) restoreWindowState;

            //if (ApplicationController.Default.ActiveView != null)
            //{
            //    ApplicationController.Default.ActiveView.SaveSettings();
            //}
        }

        private void ApplicationController_ActiveViewChanged(object sender, EventArgs e) 
        {
            // Disable the Refresh and Pause buttons for ReportsView.
            SetRefreshOptionsAvailablity(!(ApplicationController.Default.ActiveView is Views.Reports.ReportsView));
        }

        public void SetRefreshOptionsAvailablity(bool enabled)
        {
            toolbarsManager.Tools["refreshActiveViewButton"].SharedProps.Enabled =
                toolbarsManager.Tools["pauseActiveViewRefreshButton"].SharedProps.Enabled = enabled;
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            SetTrayStatus(MonitoredState.None, 0, 0);
            UpdateRepositoryConnectionStatus();
        }

        private void ApplicationController_RefreshActiveViewStarted(object sender, EventArgs e)
        {
            refreshError = null;

            if (statusBar != null && !statusBar.IsDisposed)
            {
                statusBar.Panels["viewRefreshStatusPanel"].Appearance.Image = Resources.ToolbarRefresh;
                statusBar.Panels["viewRefreshStatusPanel"].Text = "Refreshing...";
            }
        }

        private void ApplicationController_RefreshActiveViewCompleted(object sender,
                                                                      RefreshActiveViewCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                refreshError = null;

                if (statusBar != null && !statusBar.IsDisposed)
                {
                    statusBar.Panels["viewRefreshStatusPanel"].Appearance.Image = null;

                    if (ApplicationController.Default.ActiveView is ServerView &&
                        ((ServerView)ApplicationController.Default.ActiveView).HistoricalSnapshotDateTime.HasValue)
                    {
                        statusBar.Panels["viewRefreshStatusPanel"].Text = Resources.HistoryModeStatusBarLabel;
                    }
                    else
                    {
                        statusBar.Panels["viewRefreshStatusPanel"].Text =
                            string.Format("Refreshed: {0}", e.Completed.ToString("G"));
                    }
                }
            }
            else
            {
                refreshError = e.Error;

                if (statusBar != null && !statusBar.IsDisposed)
                {
                    statusBar.Panels["viewRefreshStatusPanel"].Appearance.Image = Resources.RefreshError;
                    statusBar.Panels["viewRefreshStatusPanel"].Text = "Refresh Error - click here";
                }
            }
        }

        private void UpdateRepositoryConnectionStatus()
        {
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Text =
                    string.Format(ApplicationTitleText, Application.ProductName,
                                  Settings.Default.ActiveRepositoryConnection.ConnectionInfo.InstanceName,
                                  Settings.Default.ActiveRepositoryConnection.ConnectionInfo.DatabaseName);
            }
            else
            {
                Text = Application.ProductName + " - Disconnected";
            }
        }

        private void ApplicationModel_FocusObjectChanged(object sender, EventArgs e)
        {
            UpdateTools();
        }

        public void SetRefreshStatusVisible(bool isVisible)
        {
            if (statusBar != null && !statusBar.IsDisposed)
            {
                statusBar.Panels["viewRefreshStatusPanel"].Visible = isVisible;
            }
        }

        public void SetRefreshStatusText(string text)
        {
            if (statusBar != null && !statusBar.IsDisposed)
            {
                statusBar.Panels["viewRefreshStatusPanel"].Appearance.Image = null;
                statusBar.Panels["viewRefreshStatusPanel"].Text = text;
            }
        }

        public void ClearCustomStatus()
        {
            SetCustomStatus(String.Empty, String.Empty);
        }

        public void SetCustomStatus(string statusText)
        {
            SetCustomStatus(statusText, String.Empty);
        }

        public void SetCustomStatus(string statusText1, string statusText2)
        {
            if (statusBar != null && !statusBar.IsDisposed)
            {
                statusBar.Panels["customStatusPanel1"].Text = statusText1;
                statusBar.Panels["customStatusPanel2"].Text = statusText2;
            }
        }

        private void UpdateTools()
        {
            if (ApplicationModel.Default.FocusObject != null &&
                !(ApplicationModel.Default.FocusObject is AllServersUserView) &&
                !(ApplicationModel.Default.FocusObject is SearchUserView) && 
                navigationPaneControl.SelectedPane == NavigationPanes.Servers)
            {
                toolbarsManager.Tools["deleteSelectedObjectButton"].SharedProps.Visible = 
                    ApplicationModel.Default.FocusObject is StaticUserView || ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = 
                    ApplicationModel.Default.UserToken.IsSQLdmAdministrator || !(ApplicationModel.Default.FocusObject is Tag);
            }
            else
            {
                toolbarsManager.Tools["deleteSelectedObjectButton"].SharedProps.Visible = false;
                toolbarsManager.Tools["propertiesButton"].SharedProps.Visible = false;
            }

            ((StateButtonTool) toolbarsManager.Tools["hideWhenMinimizedButton"]).InitializeChecked(
                Settings.Default.HideConsoleWhenMinimized);

            ((StateButtonTool) toolbarsManager.Tools["showAlertNotificationsButton"]).InitializeChecked(
                Settings.Default.ConsoleAlertNotificationDisplayOption !=
                (int) ConsoleAlertNotificationDisplayOptions.NeverShow);
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            toolbarsManager.Tools["manageServersButton"].SharedProps.Visible =
            toolbarsManager.Tools["manageTagsButton"].SharedProps.Visible =
            toolbarsManager.Tools["selectAdministrationPaneButton"].SharedProps.Visible =
            toolbarsManager.Tools["defaultAlertConfigurationButton"].SharedProps.Visible =
            toolbarsManager.Tools["notificationRulesButton"].SharedProps.Visible =
            toolbarsManager.Tools["groomingOptionsButton"].SharedProps.Visible =
            toolbarsManager.Tools["manageLicensesButton"].SharedProps.Visible =
            toolbarsManager.Tools["enablePredictiveAnalyticsButton"].SharedProps.Visible =
            toolbarsManager.Tools["vmConfigurationButton"].SharedProps.Visible = 
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            bool paEnabled = IsPredictiveAnalyticsEnabled();
            toolbarsManager.Tools["enablePredictiveAnalyticsButton"].SharedProps.Caption = paEnabled ? "Disable Predictive Analytics" : "Enable Predictive Analytics";

            toolbarsManager.Tools["selectTasksPaneButton"].SharedProps.Visible =
                ApplicationModel.Default.IsTasksViewEnabled;

            if (e.Tool.Key == "fileMenu")
            {
                UpdateTools();
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            using (Log.InfoCall())
            {
                Log.Info("e.Tool.Key = ", e.Tool.Key);

                switch (e.Tool.Key)
                {
                    case "showTodayViewButton":
                        ApplicationController.Default.ShowTodayView();
                        break;
                    case "connectToRepositoryButton":
                        ShowRepositoryConnectionDialog();
                        break;
                    case "manageServersButton":
                        ShowManageServersDialog();
                        break;
                    case "manageTagsButton":
                        ShowManageTagsDialog();
                        break;
                    case "addUserViewButton":
                        ShowAddUserViewDialog();
                        break;
                    case "addServersButton":
                        ShowAddServersWizard();
                        break;
                    case "exitApplicationButton":
                        ExitApplication();
                        break;
                    case "aboutButton":
                        ShowAboutBox();
                        break;
                    case "manageLicensesButton":
                        LicenseDialog.Display(this);
                        break;
                    case "refreshActiveViewButton":
                        ApplicationController.Default.RefreshActiveView();
                        break;
                    case "pauseActiveViewRefreshButton":
                        ToggleAutomatedRefresh();
                        break;
                    case "deleteSelectedObjectButton":
                        DeleteFocusObject();
                        break;
                    case "propertiesButton":
                        ShowFocusObjectProperties();
                        break;
                    case "showNavigationPaneButton":
                        if (!alreadydoingit)
                            NavigationPaneVisible = true;
                        break;
                    case "hideNavigationPaneButton":
                        if (!alreadydoingit)
                            NavigationPaneVisible = false;
                        break;
                    case "navigationPaneVisibilityButton":
                        if (!alreadydoingit)
                            NavigationPaneVisible = !NavigationPaneVisible;
                        break;
                    case "toggleRibbonVisibilityButton":
                        if (!alreadydoingit)
                        {
                            Settings.Default.CollapseRibbon =
                                ((StateButtonTool) (toolbarsManager.Tools["toggleRibbonVisibilityButton"])).Checked;
                        }
                        break;
                    case "selectServersPaneButton":
                        break;
                    case "selectAlertsPaneButton":
                        break;
                    case "selectPulsePaneButton":
                        break;
                    case "selectTasksPaneButton":
                        break;
                    case "selectReportsPaneButton":
                        break;
                    case "selectAdministrationPaneButton":
                        break;
                    case "defaultAlertConfigurationButton":
                        ShowAlertConfigurationDialog();
                        break;
                    case "vmConfigurationButton":
                        ShowVirtualizationConfigDialog();
                        break;
                    case "notificationRulesButton":
                        ShowNotificationRulesDialog();
                        break;
                    case "groomingOptionsButton":
                        ShowGroomingOptionsDialog();
                        break;
                    case "consoleOptionsButton":
                        ShowConsoleOptionsDialog();
                        break;
                    case "hideWhenMinimizedButton":
                        ToggleHideWhenMinimized(e.Tool as StateButtonTool);
                        break;
                    case "showAlertNotificationsButton":
                        ToggleShowAlertNotifications(e.Tool as StateButtonTool);
                        break;
                    case "openSQLdmButton":
                        ShowConsole();
                        break;
                    case "systemDiagnosticsButton":
                        using (SystemDiagnosticsDialog diagDialog = new SystemDiagnosticsDialog())
                        {
                            diagDialog.ShowDialog(this);
                        }
                        break;
                    case "helpSearchButton":
                        Help.ShowHelp(new Form(), Constants.HelpFileName, HelpNavigator.Find, string.Empty);
                        break;
                    case "helpContentsButton":
                        Help.ShowHelp(new Form(), Constants.HelpFileName, HelpNavigator.TableOfContents);
                        break;
                    case "helpIndexButton":
                        Help.ShowHelp(new Form(), Constants.HelpFileName, HelpNavigator.Index);
                        break;
                    case "helpOnThisWindowButton":
                    case "helpButton":
                        // Show the help topic for the current view.
                        if (ApplicationController.Default.ActiveView == null)
                        {
                            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HelpStartPage);
                        }
                        else
                        {
                            ApplicationController.Default.ActiveView.ShowHelp();
                        }
                        break;
                    case "checkForUpdatesButton":
                        Process.Start(string.Format("{0}{1}", Constants.CheckForUpdatesUrl, Constants.VersionCheckProductId));
                        break;
                    case "contactSupportButton":
                        Process.Start(Constants.ContactSupportUrl);
                        break;
                    case "searchKnowledgeBaseButton":
                        Process.Start(Constants.SearchKnowledgeBaseUrl);
                        break;
                    case "aboutIderaProductsButton":
                        Process.Start(Constants.AboutIderaProductsUrl);
                        break;
                    case "howDoITopic1Button":
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic1);
                        break;
                    case "howDoITopic2Button":
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic2);
                        break;
                    case "howDoITopic3Button":
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic3);
                        break;
                    case "howDoITopic4Button":
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic4);
                        break;
                    case "enablePredictiveAnalyticsButton":
                        TogglePredictiveAnalyticsService(e.Tool as StateButtonTool);
                        break;
                    case "visitTrialCenterButton":
                        ApplicationController.Default.LaunchTrialCenterUrl();
                        break;
                }
            }
        }

        private void ToggleShowAlertNotifications(StateButtonTool tool)
        {
            if (tool.Checked)
            {
                Settings.Default.ConsoleAlertNotificationDisplayOption =
                    (int) ConsoleAlertNotificationDisplayOptions.AlwaysShow;
            }
            else
            {
                Settings.Default.ConsoleAlertNotificationDisplayOption =
                    (int)ConsoleAlertNotificationDisplayOptions.NeverShow;
            }
        }

        private void ToggleHideWhenMinimized(StateButtonTool tool)
        {
            Settings.Default.HideConsoleWhenMinimized = tool.Checked;
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = !tool.Checked;
            }
        }

        public bool IsAutomaticRefreshPaused
        {
            get { return !foregroundRefreshTimer.Enabled; }
        }

        public void PauseAutomaticRefresh()
        {
            if (!((StateButtonTool)toolbarsManager.Tools["pauseActiveViewRefreshButton"]).Checked)
            {
                ((StateButtonTool)toolbarsManager.Tools["pauseActiveViewRefreshButton"]).Checked = true;
            }
        }

        private void ToggleAutomatedRefresh()
        {
            foregroundRefreshTimer.Enabled = !foregroundRefreshTimer.Enabled;

            if (foregroundRefreshTimer.Enabled)
            {
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void ShowFocusObjectProperties()
        {
            if (ApplicationModel.Default.FocusObject != null)
            {
                if (ApplicationModel.Default.FocusObject is StaticUserView)
                {
                    StaticUserView staticUserView = ApplicationModel.Default.FocusObject as StaticUserView;
                    StaticViewPropertiesDialog dialog = new StaticViewPropertiesDialog(staticUserView);
                    dialog.ShowDialog(this);
                }
                else if (ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper)
                {
                    MonitoredSqlServer instance = ApplicationModel.Default.FocusObject as MonitoredSqlServerWrapper;

                    if (instance != null)
                    {
                        try
                        {
                            MonitoredSqlServerInstancePropertiesDialog dialog =
                                new MonitoredSqlServerInstancePropertiesDialog(instance.Id);
                            dialog.ShowDialog(this);
                        }
                        catch (Exception e)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            string.Format("Unable to show properties for {0}.",
                                                                          instance), e);
                        }
                    }
                }
                else if (ApplicationModel.Default.FocusObject is Tag)
                {
                    TagPropertiesDialog dialog = new TagPropertiesDialog(ApplicationModel.Default.FocusObject as Tag);
                    dialog.ShowDialog(this);
                }
            }
        }

        private void DeleteFocusObject()
        {
            focused = GetFocusedControl(Controls);
            if (ApplicationModel.Default.FocusObject != null)
            {
                if (ApplicationModel.Default.FocusObject is UserView &&
                    navigationPaneControl.SelectedPane == NavigationPanes.Servers)
                {
                    DeleteUserView(ApplicationModel.Default.FocusObject as UserView);
                }
                else if (ApplicationModel.Default.FocusObject is Tag &&
                    navigationPaneControl.SelectedPane == NavigationPanes.Servers)
                {
                    DeleteTagView(ApplicationModel.Default.FocusObject as Tag);
                }
                else if (ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper &&
                    navigationPaneControl.SelectedPane == NavigationPanes.Servers && !(focused is TextBox) && !(focused is DateTimePicker))
                {
                    DeleteInstance(ApplicationModel.Default.FocusObject as MonitoredSqlServerWrapper);
                }
            }
        }

        private void DeleteTagView(Tag tag)
        {
            if (tag != null)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning(ParentForm,
                                                      "Deleting a tag will remove all links to associated SQLDM objects. If custom counters or permissions are associated with servers via tags, custom counters may no longer be collected for tagged servers and permissions may not longer apply. You can use the Manage Tags dialog to view all SQLDM objects associated with this tag. Would like to proceed with deleting this tag?",
                                                      ExceptionMessageBoxButtons.YesNo);

                if (dialogResult != DialogResult.No)
                {
                    try
                    {
                        ApplicationModel.Default.RemoveTags(new Tag[] { tag });
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(ParentForm,
                                                        "An error occurred while deleting the selected tag.",
                                                        e);
                    }
                }
            }
        }

        private void DeleteUserView(UserView userView)
        {
            if (userView != null)
            {
                if (
                    ApplicationMessageBox.ShowWarning(this,
                                                      string.Format(
                                                          "Are you sure you want to delete the view \"{0}\"?",
                                                          userView.Name), ExceptionMessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    Settings.Default.ActiveRepositoryConnection.UserViews.Remove(userView);
                }
            }
        }

        private void DeleteInstance(MonitoredSqlServerWrapper instance)
        {
            if (instance != null)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning(this,
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
                            ApplicationModel.Default.DeactivateMonitoredSqlServers(new MonitoredSqlServerWrapper[] { instance });
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            ApplicationModel.Default.DeleteMonitoredSqlServers(new MonitoredSqlServerWrapper[] { instance });
                        }
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        "An error occurred while removing the selected SQL Server instance.",
                                                        e);
                    }
                }
            }
        }

        private void ShowGroomingOptionsDialog()
        {
            try
            {
                using (GroomingOptionsDialog dialog = new GroomingOptionsDialog())
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the grooming options from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private void ShowConsoleOptionsDialog()
        {
            using (ConsoleOptionsDialog consoleOptionsDialog = new ConsoleOptionsDialog())
            {
                consoleOptionsDialog.ShowDialog(this);
            }
        }

        private void ShowManageServersDialog()
        {
            ManageServersDialog manageServersDialog = new ManageServersDialog();
            manageServersDialog.ShowDialog(this);
        }

        private void ShowManageTagsDialog()
        {
            ManageTagsDialog manageTagsDialog = new ManageTagsDialog();
            manageTagsDialog.ShowDialog(this);
        }

        private void ShowAddUserViewDialog()
        {
            StaticViewPropertiesDialog dialog = new StaticViewPropertiesDialog();
            dialog.ShowDialog(this);
        }

        private void ShowAddServersWizard()
        {
            Dictionary<string, object> existingServers = new Dictionary<string, object>();

            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                existingServers.Add(instance.InstanceName, existingServers);
            }

            AddServersWizard wizard = new AddServersWizard(true, existingServers);
            wizard.ShowDialog(this);
        }

        private void ShowRepositoryConnectionDialog()
        {
            RepositoryConnectionDialog dialog = new RepositoryConnectionDialog();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator && 
                    Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                {
                    GettingStartedWizard gettingStartedWizard = new GettingStartedWizard();
                    gettingStartedWizard.ShowInTaskbar = false;
                    gettingStartedWizard.ShowDialog(this);
                }
            }
        }

        private void ShowAlertConfigurationDialog()
        {
            try
            {
                using (AlertTemplateDialog alertTempalteDialog = new AlertTemplateDialog())
                {
                    alertTempalteDialog.ShowDialog(this);
                }
                //using (AlertConfigurationDialog alertConfigDialog = new AlertConfigurationDialog(0, true))
                //{
                //    alertConfigDialog.ShowDialog(this);
                //}
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }

        }

        private void ShowVirtualizationConfigDialog()
        {
            try
            {
                using (VirtualizationConfig  virtualizationConfig = new VirtualizationConfig())
                {
                    virtualizationConfig.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Error in the Virtualization Configuration Dialog", ex);
            }
        }

        private void ShowNotificationRulesDialog()
        {
            try
            {
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                
                using (NotificationRulesDialog notificationRulesDialog = new NotificationRulesDialog(managementService))
                {
                    notificationRulesDialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the list of notification rules.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private static void ExitApplication()
        {
            Application.Exit();
        }

        private void ShowAboutBox()
        {
            AboutDialog dialog = new AboutDialog();
            dialog.ShowDialog(this);
        }

        protected override void WndProc(ref Message msg)
        {
            switch ((Newsfeed.Plugins.Helpers.ClipboardHelper.Messages)msg.Msg)
            {
                case Newsfeed.Plugins.Helpers.ClipboardHelper.Messages.WM_DRAWCLIPBOARD:
                case Newsfeed.Plugins.Helpers.ClipboardHelper.Messages.WM_CHANGECBCHAIN:
                    if (Newsfeed.Plugins.UI.PulseController.HandleClipboardMessage(msg))
                    {
                        Log.InfoFormat("Newsfeed request handled by Newsfeed Controller: {0}", msg.Msg);
                        ApplicationController.Default.ShowPulseView();
                    }
                    else
                    {
                        base.WndProc(ref msg);
                    }
                    break;
                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Alt | Keys.F1:
                        NavigationPaneVisible = !NavigationPaneVisible;
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static Control GetFocusedControl(Control.ControlCollection controls)
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

            return focusedControl;
        }

        private void contentPanelSplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void contentPanelSplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void automatedRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (ApplicationController.Default.ActiveView is ServerView)
            {
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void statusBar_PanelClick(object sender, PanelClickEventArgs e)
        {
            if (e.Panel.Key == "viewRefreshStatusPanel" && refreshError != null)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while refreshing the active view.",
                                                refreshError, false);
            }
        }

        private void backgroundRefreshTimer_Tick(object sender, EventArgs e)
        {
            ApplicationController.Default.RefreshBackgroundData();

            if (!(ApplicationController.Default.ActiveView is ServerView))
            {
                ApplicationController.Default.RefreshActiveView(true);
            }
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            UpdateRestoreBounds();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            UpdateRestoreBounds();
        }

        private void UpdateRestoreBounds()
        {
            switch (WindowState)
            {
                case FormWindowState.Normal:
                    restoreBounds = Bounds;
                    restoreWindowState = WindowState;
                    break;
                case FormWindowState.Minimized:
                    restoreBounds = RestoreBounds;
                    Visible = !Settings.Default.HideConsoleWhenMinimized;
                    break;
                case FormWindowState.Maximized:
                    restoreBounds = RestoreBounds;
                    restoreWindowState = WindowState;
                    break;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HideConsoleWhenMinimized":
                    UpdateHideConsoleWhenMinimizedButton();
                    break;
                case "ConsoleAlertNotificationDisplayOption":
                    UpdateShowAlertsNotificationsButton();
                    break;
                case "BackgroundRefreshIntervalInMinutes":
                    UpdateBackgroundRefreshTimer();
                    break;
                case "ForegroundRefreshIntervalInSeconds":
                    UpdateForegroundRefreshTimer(false);
                    break;
                case "ColorScheme":
                    if (trialCenterPicture.Visible)
                    {
                        trialCenterPicture.BackgroundImage = Settings.Default.ColorScheme.Contains("Black") 
                            ? Resources.trialcentergraphic 
                            : Resources.trialcentergraphic_dark;
                    }
                    break;
            }
        }

        private void UpdateBackgroundRefreshTimer()
        {
            TimeSpan interval = TimeSpan.FromMinutes(Settings.Default.BackgroundRefreshIntervalInMinutes);

            backgroundRefreshTimer.Stop();
            backgroundRefreshTimer.Interval = (int)interval.TotalMilliseconds;
            backgroundRefreshTimer.Start();

            Log.DebugFormat("Status & Alerts refresh interval set to {0} minutes.", interval.TotalMinutes);
        }

        private void UpdateForegroundRefreshTimer(bool isLoadStart)
        {
            TimeSpan interval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds);

            // If the timer is enabled or the function was called during form load, then start
            // the timer.   Else just change the interval.
            if (foregroundRefreshTimer.Enabled || isLoadStart)
            {
                foregroundRefreshTimer.Stop();
                foregroundRefreshTimer.Interval = (int)interval.TotalMilliseconds;
                foregroundRefreshTimer.Start();
            }
            else
            {
                foregroundRefreshTimer.Interval = (int)interval.TotalMilliseconds;
            }

            Log.DebugFormat("Server View refresh interval set to {0} seconds.", interval.TotalSeconds);
        }

        private void UpdateHideConsoleWhenMinimizedButton()
        {
            bool hideWhenMinimized = Settings.Default.HideConsoleWhenMinimized;
            ((StateButtonTool) toolbarsManager.Tools["hideWhenMinimizedButton"]).InitializeChecked(hideWhenMinimized);
            if (WindowState == FormWindowState.Minimized)
                Visible = !hideWhenMinimized;
        }

        private void UpdateShowAlertsNotificationsButton()
        {
            ((StateButtonTool) toolbarsManager.Tools["showAlertNotificationsButton"]).InitializeChecked(
                Settings.Default.ConsoleAlertNotificationDisplayOption != (int)ConsoleAlertNotificationDisplayOptions.NeverShow);
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // NOTE: At some point we may want to show something we may want to 
            // show something other than just popping up the main form.
            MonitoredState state = MonitoredState.None;

            if (trayIcon.Tag is MonitoredState)
            {
                state = (MonitoredState) trayIcon.Tag;
            }

            switch (state)
            {
                case MonitoredState.Critical:
                case MonitoredState.Warning:
                case MonitoredState.OK:
                    //notificationPopupWindow.Show(false);
                    //break;
                default:
                    ShowConsole();
                    break;
            }
        }

        public void ShowConsole()
        {
            Visible = true;
            WindowState = restoreWindowState;
            BringToFront();
        }

        private void ApplicationController_BackgroundRefreshCompleted(object sender,
                                                                      BackgroundRefreshCompleteEventArgs e)
        {
            UpdateStatus();
            UpdateTools();
        }

        public void UpdateStatus()
        {
            MonitoredState highestSeverity = MonitoredState.None;
            int criticalCount = 0;
            int warningCount = 0;

            bool gotStatusForAll = true;
            bool statusChanged = false;
            bool neverShowServerGoesOk = false;

            if (ApplicationModel.Default.ActiveInstances.Count > 0)
            {
                ConsoleAlertNotificationDisplayOptions option;
                List<MonitoredSqlServerStatus> changeList = new List<MonitoredSqlServerStatus>();

                try
                {
                    option = (ConsoleAlertNotificationDisplayOptions)Settings.Default.ConsoleAlertNotificationDisplayOption;
                    neverShowServerGoesOk = (bool)Settings.Default.PopupShowPopupServerInfoWarCrit;
                }
                catch
                {
                    option = ConsoleAlertNotificationDisplayOptions.AlwaysShow;
                }

                foreach (MonitoredSqlServerWrapper wrapper in ApplicationModel.Default.ActiveInstances)
                {
                    MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(wrapper.Id,wrapper.RepoId);

                    if (status != null)
                    {
                        MonitoredState cachedSeverity;

                        if (status.Severity > highestSeverity)
                        {
                            highestSeverity = status.Severity;
                        }

                        if (status.Severity == MonitoredState.Critical)
                        {
                            ++criticalCount;
                        }
                        else if (status.Severity == MonitoredState.Warning)
                        {
                            ++warningCount;
                        }

                        if (instanceStateTracking.TryGetValue(wrapper.Id, out cachedSeverity))
                        {
                            // The state tracking table must be updated regardless of the console
                            // alert notification settings
                            if (cachedSeverity != status.Severity)
                            {
                                instanceStateTracking.Remove(wrapper.Id);
                                instanceStateTracking.Add(wrapper.Id, status.Severity);
                                statusChanged = true;
                            }

                            if (option == ConsoleAlertNotificationDisplayOptions.AlwaysShow)
                            {   // add if it was bad or if it is bad - show OK only on transition
                                if (cachedSeverity > MonitoredState.OK || status.Severity > MonitoredState.OK)
                                    changeList.Add(status);
                            }
                            else if (option == ConsoleAlertNotificationDisplayOptions.ShowOnlyOnStateTransition)
                            {
                                if(cachedSeverity != status.Severity)
                                {   // verify if the neverShowServerGoesOk option is checked and the serverity is not OK or just add if not checked
                                    if ((neverShowServerGoesOk && status.Severity > MonitoredState.OK) || !neverShowServerGoesOk)
                                    {
                                            changeList.Add(status);
                                    }
                                }
                            }
                        }
                        else
                        {
                            instanceStateTracking.Add(wrapper.Id, status.Severity);
                            if (status.Severity > MonitoredState.OK)
                            {
                                changeList.Add(status);
                            }
                        }
                    }
                    else
                        gotStatusForAll = false;
                }

                // NOTE: Check to make sure the status bar is shown, which is an indication that
                // the main form is shown, otherwise an exception can occur when the notification
                // popup window is clicked. The result if this behavior is that the popup window
                // is not shown until the second change to the severity list occurs.
                if (statusBar.Visible && option != ConsoleAlertNotificationDisplayOptions.NeverShow)
                {
                    // Show the notification popup window if we have changes to show
                    if (gotStatusForAll && warningCount == 0 && criticalCount == 0 && statusChanged && !neverShowServerGoesOk)
                    {   // be careful about showing 'all servers are ok' - pisses folks off if they aren't
                        notificationPopupWindow.SetMessage("All monitored instances are OK.");
                        notificationPopupWindow.Show();
                    }
                    else
                    if (changeList.Count > 0)
                    {
                        notificationPopupWindow.SetInstanceStatus(changeList);
                        notificationPopupWindow.Show();
                    }
                }
            }

            navigationPaneControl.SetVisible(NavigationPanes.Tasks, ApplicationModel.Default.IsTasksViewEnabled);

            SetTrayStatus(highestSeverity, criticalCount, warningCount);
        }

        private void SetTrayStatus(MonitoredState state, int criticalCount, int warningCount)
        {
            string stateIconText = Application.ProductName + " - {0}";

            trayIcon.Tag = state;

            switch (state)
            {
                case MonitoredState.Critical:
                    trayIcon.Text = string.Format(stateIconText, criticalCount + " Critical Instances");
                    trayIcon.Icon = Resources.App16CriticalIcon;
                    break;
                case MonitoredState.Warning:
                    trayIcon.Text = string.Format(stateIconText, warningCount + " Warning Instances");
                    trayIcon.Icon = Resources.App16WarningIcon;
                    break;
                case MonitoredState.OK:
                    trayIcon.Text = string.Format(stateIconText, "All Instances OK");
                    trayIcon.Icon = Resources.App16OKIcon;
                    break;
                default:
                    trayIcon.Text = Application.ProductName;
                    trayIcon.Icon = Resources.App16Icon;
                    break;
            }
        }

        private bool IsPredictiveAnalyticsEnabled()
        {
            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                return managementService.GetPredictiveAnalyticsEnabled();
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to contact management service", ex);
                Log.Error("Unable to determine if Predictive Analytics Service is enabled", ex);
                return false;
            }
        }

        private void TogglePredictiveAnalyticsService(StateButtonTool tool)
        {
            string enabledText  = "Disable Predictive Analytics";
            string disabledText = "Enable Predictive Analytics";

            bool state = tool.SharedProps.Caption == enabledText ? false : true;

            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService();

                AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                // enable PA service
                managementService.SetPredictiveAnalyticsEnabled(state);

                // Let them know we were successful
                if (managementService.GetPredictiveAnalyticsEnabled() == state)
                {
                    ApplicationModel.Default.NotifyPredictiveAnaltyicsStateChanged();
                    tool.SharedProps.Caption = state ? enabledText : disabledText;
                    MessageBox.Show(string.Format("Predictive Analytics has been {0}.", state ? "enabled" : "disabled"), "Success Dialog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Failed to enable Predictive Analytics", ex);
            }
        }

        private void trialCenterPicture_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.LaunchTrialCenterUrl();
        }

        /// <summary>
        /// Resizes the trial image according the available space between top and the contentPanel location.
        /// </summary>
        /// <remarks></remarks>
        void ResizeTrialImage()
        {
            int newWidth = this.trialCenterPicture.Width;
            int newHeigh = this.trialCenterPicture.Height;

            //// Calculated from the original image size: 46*300, this conversionFactor helps to resize the image.
            double conversionFactor = (newHeigh > 0) ? newWidth / newHeigh : 1;

            int trialImageY = this.trialCenterPicture.Location.Y;
            int contentPanelY = this.contentPanel.Location.Y;

            //// The difference is the available space for the image, and its new Heigh.
            int availableSpace = contentPanelY - trialImageY;
            newHeigh = (availableSpace > 0) ? availableSpace : newHeigh;
            newWidth = (int)Math.Round(newHeigh * conversionFactor);

            SuspendLayout();

            this.trialCenterPicture.Height = newHeigh;
            this.trialCenterPicture.Width = newWidth;

            //// 10 pixels for the right margin
            int tempNewWidth = this.Width - newWidth - 10;
            this.trialCenterPicture.Left = (tempNewWidth > 0) ? tempNewWidth : this.trialCenterPicture.Left;

            ResumeLayout();
        }
    }
}
