using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Reflection;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views;
using Idera.SQLdm.DesktopClient.Views.Administration;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.DesktopClient.Views.Pulse;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup;
using Idera.SQLdm.DesktopClient.Views.Tasks;
using Idera.SQLdm.DesktopClient.Controls;
using Infragistics.Windows.Themes;
using Microsoft.Reporting.WinForms;
using View=Idera.SQLdm.DesktopClient.Views.View;
using System.Threading;
using System.Xml.Serialization;


namespace Idera.SQLdm.DesktopClient
{
    using System.ComponentModel;
    using System.Diagnostics;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;

    internal sealed class ApplicationController
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ApplicationController");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        private static ApplicationController defaultInstance = new ApplicationController();

        private MainWindow mainWindow;

        private XamNavigationPane navigationPaneControl;
        void setServersSecondaryMenuIndex(ServerViewTabs tab)
        {
            if(navigationPaneControl != null && navigationPaneControl.SecondaryNavigationPane != null)
            {
                navigationPaneControl.SecondaryNavigationPane.SetServersSecondaryMenuIndex(tab);
            }
        }
        private ViewContainer viewContainer;
        private ViewManager viewManager;

        private IView activeView;
        private NewSQLdmTodayView todayView;
        private ServerGroupView activeServersView;
        private AlertsView alertsView;
        private PulseView pulseView;
        private TasksView tasksView;
        private ReportsView reportsView;
        private AdministrationView administrationView;

        private BackgroundWorker backgroundRefreshWorker;
        private BackgroundWorker backgroundLicenseWorker;
        private BackgroundWorker backgroundPersistUserSettingsWorker;
        private object backgroundRefreshLock = new object();
        private object backgroundLicenseLock = new object();
        private object backgroundPersistUserSettingsLock = new object();

        private static object activeViewChangeLock = new object();

        public event EventHandler<ActiveViewChangingEventArgs> ActiveViewChanging;
        public event EventHandler ActiveViewChanged;
        public event EventHandler<ReportsViewChangedEventArgs> ReportsViewChanged;
        public event EventHandler<AdministrationViewChangedEventArgs> AdministrationViewChanged;
        public event EventHandler RefreshActiveViewStarted;
        public event EventHandler<RefreshActiveViewCompletedEventArgs> RefreshActiveViewCompleted;
        public event EventHandler<BackgroundRefreshCompleteEventArgs> BackgroundRefreshCompleted;

        public static ApplicationController Default
        {
            get { return defaultInstance; }
        }

        internal bool IsHooked { get; private set; }

        internal void HookEvents()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Settings.Default.ActiveRepositoryConnectionChanging += Settings_ActiveRepositoryConnectionChanging;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;
            PulseController.Default.LoggedIn += PulseController_LoggedIn;
            PulseController.Default.LoggedOut += PulseController_LoggedOut;
            IsHooked = true;
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationController.HookEvents : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void ShowRepositoryConnectionDialog()
        {
            RepositoryConnectionDialog dialog = new RepositoryConnectionDialog();

            if (dialog.ShowDialog((WinformWindow)mainWindow) == System.Windows.Forms.DialogResult.OK)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator &&
                    Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                {
                    GettingStartedWizard gettingStartedWizard = new GettingStartedWizard();
                    gettingStartedWizard.ShowInTaskbar = false;
                    gettingStartedWizard.ShowDialog((WinformWindow)mainWindow);
                }
            }
        }

        public void CleanUp()
        {
            PulseController.Default.LoggedIn -= PulseController_LoggedIn;
            PulseController.Default.LoggedOut -= PulseController_LoggedOut;

            if (backgroundRefreshWorker != null && backgroundRefreshWorker.IsBusy)
            {
                backgroundRefreshWorker.CancelAsync();
            }

            if (viewManager != null)
            {
                viewManager.Dispose();
            }
            if (viewContainer != null)
            {
                viewContainer.Dispose();
            }

            activeView = null;

            if (todayView != null)
            {
                todayView.Dispose();
                todayView = null;
            }
            if (activeServersView != null)
            {
                activeServersView.Dispose();
                activeServersView = null;
            }
            if (alertsView != null)
            {
                alertsView.Dispose();
                alertsView = null;
            }
            if (pulseView != null)
            {
                pulseView.Dispose();
                pulseView = null;
            }
            if (tasksView != null)
            {
                tasksView.Dispose();
                tasksView = null;
            }
            if (reportsView != null)
            {
                reportsView.Dispose();
                reportsView = null;
            }
            if (administrationView != null)
            {
                administrationView.Dispose();
                administrationView = null;
            }

            if (navigationPaneControl != null)
            {
                navigationPaneControl = null;
            }

            mainWindow = null;
        }

        public IView ActiveView
        {
            get { return activeView; }
            set
            {
                lock (activeViewChangeLock)
                {
                    CloseHeatMapOverlayPanels();
                    OnActiveViewChanging(new ActiveViewChangingEventArgs(activeView, value));
                    activeView = value;
                    OnActiveViewChanged(EventArgs.Empty);
                }
            }
        }

        //
        public bool IsNextPrevious
        {
            get; set;
        }
        public bool IsFromHistorySnapshot
        {
            get; set;
        }
        //

        void CloseHeatMapOverlayPanels()
        {
            foreach (var panel in ApplicationModel.Default.heatMapOverlayPanels)
            {
                panel.Close();
            }
        }

        private void OnActiveViewChanging(ActiveViewChangingEventArgs e)
        {
            if (ActiveViewChanging != null)
            {
                ActiveViewChanging(this, e);
            }
        }

        private void OnActiveViewChanged(EventArgs e)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, e);
            }
        }

        public void OnReportsViewChanged(ReportsViewChangedEventArgs e)
        {
            if (ReportsViewChanged != null)
            {
                ReportsViewChanged(this, e);
            }
        }

        private void OnAdministrationViewChanged(AdministrationViewChangedEventArgs e)
        {
            if (AdministrationViewChanged != null)
            {
                AdministrationViewChanged(this, e);
            }
        }

        public void FireBackgroundRefreshCompleted()
        {
            OnBackgroundRefreshCompleted(new BackgroundRefreshCompleteEventArgs(DateTime.Now));
        }

        private void OnBackgroundRefreshCompleted(BackgroundRefreshCompleteEventArgs e)
        {
            if (BackgroundRefreshCompleted != null)
            {
                BackgroundRefreshCompleted(this, e);
            }
        }

        public void OnRefreshActiveViewStarted(EventArgs e)
        {
            if (RefreshActiveViewStarted != null)
            {
                RefreshActiveViewStarted(this, e);
            }
        }

        public void OnRefreshActiveViewCompleted(RefreshActiveViewCompletedEventArgs e)
        {
            if (RefreshActiveViewCompleted != null)
            {
                RefreshActiveViewCompleted(this, e);

                if (activeView is ServerViewContainer)
                {
                    ((ServerViewContainer)activeView).OnRefreshCompleted(e);
                }
            }
        }

        private void Settings_ActiveRepositoryConnectionChanging(object sender, EventArgs e)
        {
            if (Program.MainWindow.CheckAccess())
                OnActiveRepositoryConnectionChanging();
            else
                Program.MainWindow.Dispatcher.Invoke(new Action(OnActiveRepositoryConnectionChanging));
        }

        private void OnActiveRepositoryConnectionChanging()
        {
            if (activeView != null && !activeView.IsDisposed)
            {
                activeView.CancelRefresh();
                activeView.SaveSettings();

                if (activeView is ServerViewContainer)
                {
                    ((ServerViewContainer)activeView).SetActive(false);
                }
            }

            activeView = null;
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            if (Program.MainWindow.CheckAccess())
                OnActiveRepositoryConnectionChanged();
            else
                Program.MainWindow.Dispatcher.Invoke(new Action(OnActiveRepositoryConnectionChanged));
        }

        private void OnActiveRepositoryConnectionChanged()
        {
            // Because of the refresh architecture, if one of the standard views has been 
            // created for a previous repository connection, it will be loaded initially
            // with the last known state and refreshed accordingly. That would mean that
            // users would potentially see data from a previous repository connection. To
            // prevent that, if standard view have already been instantiated, they will
            // be disposed whenever the active repository connection changes.

            //if (todayView != null)
            //{
            //    todayView.Dispose();
            //    todayView = new NewSQLdmTodayView();
            //}

            // We want do go ahead and show the new Today view
            Stopwatch stopWatch = new Stopwatch();
            //SQLDM-30956 - Need to comment this to load servers/servers overview
            //ShowTodayView();

            if (activeServersView != null)
            {
                activeServersView.Dispose();
                activeServersView = new ServerGroupView(null);
            }

            ShowActiveServersView();

            if (alertsView != null)
            {
                alertsView.Dispose();
                alertsView = new AlertsView();
            }

            stopWatch.Reset();
            stopWatch.Start();
            if (tasksView != null)
            {
                tasksView.Dispose();
                tasksView = new TasksView();
            }
            stopWatch.Stop();
            LOG.DebugFormat("Time taken by TaskView : {0}", stopWatch.ElapsedMilliseconds);

            if (reportsView != null)
            {
                reportsView.Dispose();
                reportsView = new ReportsView();
            }
        }

        private void UserTokenUpdated()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (navigationPaneControl != null)
            {
                navigationPaneControl.UpdateUserTokenAttributes();
            }

            if (ActiveView != null)
            {
                if (ActiveView is ServerBaseView)
                {
                    int instanceId = ((ServerBaseView)ActiveView).InstanceId;
                    PermissionType permission = ApplicationModel.Default.UserToken.GetServerPermission(instanceId);

                    if (permission == PermissionType.None)
                    {
                        ShowTodayView();

                        MonitoredSqlServer s = null;
                        if (ApplicationModel.Default.AllRegisteredInstances.TryGetValue(instanceId, out s))
                        {
                            ApplicationMessageBox.ShowInfo(mainWindow.GetWinformWindow(),
                                                           string.Format(
                                                               "You have been redirected to the SQLDM Today view because you no longer have access to monitor [{0}].",
                                                                    s.InstanceName));
                        }

                        return;
                    }
                }
                else if (ActiveView is AdministrationView && !ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                {
                    ShowTodayView();
                    ApplicationMessageBox.ShowInfo(mainWindow.GetWinformWindow(),
                                                   "You have been redirected to the SQLDM Today view because you no longer have administrator access.");
                }

                ActiveView.UpdateUserTokenAttributes();
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by UserTokenUpdated : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void Initialize(MainWindow mainWindow, XamNavigationPane navigationPaneControl, ViewContainer viewContainer)
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }

            if (navigationPaneControl == null)
            {
                throw new ArgumentNullException("navigationPaneControl");
            }

            if (viewContainer == null)
            {
                throw new ArgumentNullException("viewContainer");
            }

            this.mainWindow = mainWindow;
            this.navigationPaneControl = navigationPaneControl;
            this.viewContainer = viewContainer;

            //SqlDM 10.2 (Tushar)--Commenting these view construction calls because these can be constructed as and when required.
           // todayView = new NewSQLdmTodayView();
            //activeServersView = new ServerGroupView(null);
            //alertsView = new AlertsView();
            //pulseView = new PulseView();

            //if (tasksView != null)
            //    tasksView = new TasksView();

            viewManager = new ViewManager();
            //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Writing LM Registry Keys            
            //[START] SQLdm 10.0.2 (Barkha Khatri): Writing LM Registry Keys in Background
            lock (backgroundLicenseLock)
            {
                if (backgroundLicenseWorker == null)
                {
                    backgroundLicenseWorker = new BackgroundWorker();
                    backgroundLicenseWorker.WorkerSupportsCancellation = true;
                    backgroundLicenseWorker.DoWork += new DoWorkEventHandler(backgroundLicenseWorker_DoWork);
                    backgroundLicenseWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(backgroundLicenseWorker_RunWorkerCompleted);
                }

                if (!backgroundLicenseWorker.IsBusy)
                {
                    //SqlDM 10.2 (Tushar)--Adding check for valid repository connection before calling License worker.
                    if(Settings.Default.ActiveRepositoryConnection.RepositoryInfo != null && Settings.Default.ActiveRepositoryConnection.RepositoryInfo.IsValidVersion && !ApplicationModel.Default.RepositoryConnectionChanged)
                        backgroundLicenseWorker.RunWorkerAsync();
                }
            }
            //[END] SQLdm 10.0.2 (Barkha Khatri): Writing LM Registry Keys in Background
            //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Writing LM Registry Keys
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationController.Initialize : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        public void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
            System.Windows.Forms.Application.Exit();
        }

        public bool IsAutomaticRefreshPaused
        {
            get { return mainWindow.IsAutomaticRefreshPaused; }
        }

        public void PauseAutomaticRefresh()
        {
            if (!IsAutomaticRefreshPaused)
            {
                mainWindow.PauseAutomaticRefresh();
            }
        }

		//SQLDM-31025.
        public void EnableAutomaticRefresh()
        {
            if (IsAutomaticRefreshPaused)
            {
                mainWindow.EnableAutomaticRefresh();
            }
        }
		
        public void RefreshActiveView()
        {
            if (ActiveView != null)
            {
                ActiveView.RefreshView();
            }
        }

        public void RefreshActiveView(bool isBackgroundRefresh)
        {
            if (ActiveView is PulseView && isBackgroundRefresh)
            {
                ((PulseView)ActiveView).RefreshView(isBackgroundRefresh);
            }
            else if (ActiveView is ServerViewContainer)
            {
                if (((ServerViewContainer)ActiveView).ActiveView is ServerTimelineView)
                    ((ServerTimelineView)((ServerViewContainer)ActiveView).ActiveView).RefreshView(isBackgroundRefresh);
                else
                    RefreshActiveView();
            }
            else
            {
                RefreshActiveView();
            }
        }

        public void SetActiveViewToRealTimeMode()
        {
            if (ActiveView != null && ActiveView is ServerViewContainer)
            {
                ((ServerViewContainer)ActiveView).HistoricalSnapshotDateTime = null;
            }
        }

        public XamNavigationPanes SelectedNavigationPane
        {
            get
            {
                if (navigationPaneControl != null)
                {
                    return navigationPaneControl.SelectedPane;
                }
                else
                {
                    return XamNavigationPanes.Servers;
                }
            }
            set
            {
                if (navigationPaneControl != null)
                {
                    navigationPaneControl.SelectedPane = value;
                }
            }
        }

        public void ChangeTheme(ThemeName themeName)
        {
            Uri resourceUri;

            if (this.navigationPaneControl != null)
                this.navigationPaneControl.UpdateTheme(themeName);


            foreach (IView v in this.viewContainer.winformsHostPanel.Controls)
            {
                v.UpdateTheme(themeName);
            }

            

            if (themeName == ThemeName.Light)
            {
                ThemeManager.CurrentTheme = ThemeManager.ThemeNameOffice2k7Silver;

                Infragistics.Win.AppStyling.StyleManager.Load(
                       Assembly.GetExecutingAssembly().GetManifestResourceStream("Idera.SQLdm.DesktopClient.Style.Office2007Silver.isl"));
            }
            else
            {
                ThemeManager.CurrentTheme = ThemeManager.ThemeNameOffice2k7Black;

                Infragistics.Win.AppStyling.StyleManager.Load(
                       Assembly.GetExecutingAssembly().GetManifestResourceStream("Idera.SQLdm.DesktopClient.Style.Office2007Black.isl"));

            }

            resourceUri = new Uri("Style/" + themeName.ToString() + ".xaml", UriKind.Relative);

            var resources = System.Windows.Application.LoadComponent(resourceUri) as System.Windows.ResourceDictionary;
            if (resources != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(
                    System.Windows.Application.Current.Resources.MergedDictionaries[System.Windows.Application.Current.Resources.MergedDictionaries.Count - 1]);

                System.Windows.Application.Current.Resources.MergedDictionaries.Add(resources);
            }
        }

        public void ShowConsole()
        {
            if (mainWindow != null)
            {
                mainWindow.ShowConsole();
            }
        }

        public void UpdateConsoleStatus()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (mainWindow != null)
            {
                mainWindow.UpdateStatus();
            }
            UserTokenUpdated();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by UpdateConsoleStatus : {0}", stopWatch.ElapsedMilliseconds);
        }

        private void ShowView(View view)
        {
            ShowView(view, true);
        }

        private void ShowView(IView view, bool refreshActiveView)
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            if (view is AdministrationView || view is ReportsView || view is NewSQLdmTodayView || view is PulseView)
                mainWindow.LeftNavPane.Visibility = System.Windows.Visibility.Collapsed;
            else
                mainWindow.LeftNavPane.Visibility = System.Windows.Visibility.Visible;

            if (viewContainer != null && view != null && view != activeView)
            {
                viewContainer.SuspendLayout();

                if (activeView != null && !activeView.IsDisposed)
                {
                    activeView.CancelRefresh();
                    activeView.SaveSettings();
                    //  activeView.Dock = DockStyle.None;

                    if (activeView is ServerViewContainer)
                    {
                        ((ServerViewContainer)activeView).SetActive(false);
                    }
                }

                ClearCustomStatus();
                SetRefreshStatusVisible(true);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                viewContainer.Add(view);
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by viewContainer.Add : {0}", stopWatch.ElapsedMilliseconds);
                ActiveView = view;
                if (activeView is ServerViewContainer)
                {
                    ((ServerViewContainer)activeView).SetActive(true);
                }

                viewContainer.ResumeLayout();
            }
            if (refreshActiveView)
            {
                RefreshActiveView();
            }
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationController.ShowView : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        public void ShowTodayView()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (todayView == null)
            {
                todayView = new NewSQLdmTodayView();
                todayView.BackColor = Color.FromArgb(236, 235, 234);
            }

            todayView.UpdateUserTokenAttributes();
            ShowView(todayView);
            LOG.DebugFormat("Time taken by ShowTodayView.ShowView() : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void ShowAlertsView(StandardAlertsViews view)
        {
            if (alertsView == null)
            {
                alertsView = new AlertsView();
            }

            alertsView.SetView(view);
            ShowView(alertsView);
        }

        public void ShowAlertsView(StandardAlertsViews view, AlertFilter alertFilter)
        {
            bool viewChanged = false;

            if (alertsView == null)
                alertsView = new AlertsView();
            else
            if (ActiveView == alertsView && alertsView.GetView() != view)
                viewChanged = true;

            if (view == StandardAlertsViews.Active)
                alertFilter.ActiveOnly = true;

            alertsView.SetView(view);
            ShowView(alertsView, false);
            alertsView.SetArgument(alertFilter);
            if (viewChanged)
            {   // force firing of view changed event so nav pane will update correctly
                ActiveView = alertsView;
            }
        }

        public void ShowAlertsFilterOptions()
        {
            if (alertsView != null)
            {
                alertsView.ShowFilterOptions();
            }
        }

        public void ShowPulseView()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (pulseView == null)
            {
                pulseView = new PulseView();
            }

            bool isViewChanged = (ActiveView == pulseView);

            ShowView(pulseView);

            if (isViewChanged)
            {   // force firing of view changed event so nav pane will update correctly
                ActiveView = pulseView;
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationController.ShowPulseView : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void ShowPulseProfileView(int objectId)
        {
            if (pulseView == null)
            {
                pulseView = new PulseView();
            }

            bool isViewChanged = (ActiveView == pulseView);

            pulseView.ShowPulseProfile(objectId);
            ShowView(pulseView);

            if (isViewChanged)
            {   // force firing of view changed event so nav pane will update correctly
                ActiveView = pulseView;
            }
        }

        private void PulseController_LoggedIn(object sender, LoggedInEventArgs e)
        {
            Settings.Default.PulseAccountName = e.User.Name;
            Settings.Default.PulseAccountKeepLoggedIn = PulseController.Default.KeepUserLoggedIn;
        }

        private void PulseController_LoggedOut(object sender, EventArgs e)
        {
            Settings.Default.PulseAccountKeepLoggedIn = false;
        }

        public void ShowTasksView(StandardTasksViews view)
        {
            if (tasksView == null)
            {
                tasksView = new TasksView();
            }

            // Test if moving to custom view.
            bool isViewChanged = (ActiveView == tasksView && view == StandardTasksViews.Custom
                                    && tasksView.GetSelectedView() != view);

            tasksView.SetView(view);
            ShowView(tasksView);
            if (isViewChanged) // force firing to update nav pane (only if its custom view).
            {
                ActiveView = tasksView;
            }
        }

        public void ShowReportsView()
        {
            bool showGettingStarted = true;

            if (reportsView == null)
            {
                reportsView = new ReportsView();
                showGettingStarted = true;
            }
            ShowView(reportsView);
            SetRefreshStatusVisible(false);

            if (showGettingStarted)
            {
                reportsView.ShowGettingStarted();
            }
            OnReportsViewChanged(new ReportsViewChangedEventArgs(reportsView.CurrentView, reportsView.FullCustomPath));
        }

        public void ShowReportsView(ReportTypes report, DrillthroughEventArgs drillThroughArguments, string customReportKey, int? defaultServerID)
        {
            if (reportsView == null)
            {
                reportsView = new ReportsView();
            }

            ShowView(reportsView);
            SetRefreshStatusVisible(false);
            reportsView.ShowReport(report, drillThroughArguments, customReportKey, defaultServerID);
            OnReportsViewChanged(new ReportsViewChangedEventArgs(report, customReportKey));
            //OnReportsViewChanged(new ReportsViewChangedEventArgs(report));
        }

        public void ShowReportsView(ReportTypes report, DrillthroughEventArgs drillThroughArguments)
        {
            ShowReportsView(report, drillThroughArguments, null, null);
        }

        public void ShowReportsView(ReportTypes report)
        {
            ShowReportsView(report, null, null, null);
        }

        public void ShowDeployReportsWizard()
        {
            DeployReportsWizard wizard = new DeployReportsWizard();
            wizard.ShowDialog(mainWindow.GetWinformWindow());
        }

        public void ShowAdministrationView()
        {
            if (administrationView == null)
            {
                administrationView = new AdministrationView();
            }

            ShowView(administrationView);
            OnAdministrationViewChanged(new AdministrationViewChangedEventArgs(administrationView.NodeShown));
        }

        public void ShowAdministrationView(AdministrationView.AdministrationNode node)
        {
            if (administrationView == null)
            {
                administrationView = new AdministrationView();
            }
            administrationView.SetView(node);

            ShowView(administrationView);
            OnAdministrationViewChanged(new AdministrationViewChangedEventArgs(node));
        }

        public void ShowActiveServersView()
        {
            ShowActiveServersView(false);
        }

        public void ShowActiveServersView(bool properties)
        {
            if (activeServersView == null)
            {
                activeServersView = new ServerGroupView(null, properties);
            }
            if (properties)
            {
                activeServersView.ShowPropertiesView();
            }
            ShowView(activeServersView);
        }

        public void ShowUserView(Guid id)
        {
            if (id == Guid.Empty)
            {
                ShowActiveServersView();
            }
            else
            {
                ServerGroupView cachedView;

                if (viewManager.TryGetCachedServerGroupView(id, out cachedView))
                {
                    ShowView(cachedView);
                }
                else if (Settings.Default.ActiveRepositoryConnection.UserViews.Contains(id))
                {
                    UserView userView = Settings.Default.ActiveRepositoryConnection.UserViews[id];
                    ServerGroupView serverGroupView = new ServerGroupView(userView);
                    ShowView(serverGroupView);
                    viewManager.CacheServerGroupView(id, serverGroupView);
                }
            }
        }

        public void ShowTagView(int id)
        {
            //START: SQLdm 10.1(Srishti Purohit)
            //SQLDM-26636 --Commenting below lines as global tags will have id less than zero so it should still show servers associated
            //if (id <= 0)
            //{
            //    ShowActiveServersView();
            //}
            //else
            //{
            //END: SQLdm 10.1(Srishti Purohit)
            ServerGroupView cachedView;

            if (viewManager.TryGetCachedServerGroupView(id, out cachedView))
            {
                ShowView(cachedView);
            }
            else if (ApplicationModel.Default.Tags.Contains(id))
            {
                ServerGroupView serverGroupView = new ServerGroupView(id);
                ShowView(serverGroupView);
                viewManager.CacheServerGroupView(id, serverGroupView);
            }
            //}
        }

        public void ShowServerView(int id)
        {
            ShowServerView(id, ServerViews.Overview);
        }

        public void ShowServerView(int id, ServerViews subView)
        {
            ShowServerView(id, subView, null);
        }

        public void ShowServerView(int id, ServerViews subView, object argument)
        {
            ShowServerView(id, subView, argument, null);
        }

        public void ShowServerView(int id, ServerViews subView, object argument, DateTime? historicalSnapshotDateTime)
        {
            //SQLdm 9.1 (Gaurav Karwal): if statement added to prevent the error
            if (ApplicationModel.Default.ActiveInstances.Contains(id))
            {
                ServerViewContainer serverView;
                ApplicationModel.Default.SelectedInstanceId = id;
                MonitoredSqlServerWrapper instance = ApplicationModel.Default.ActiveInstances[id];
                ApplicationModel.Default.FocusObject = instance;

                if (!viewManager.TryGetCachedServerView(id, out serverView))
                {
                    serverView = new ServerViewContainer(id);
                    viewManager.CacheServerView(id, serverView);
                }

                if (historicalSnapshotDateTime.HasValue)
                {
                    serverView.HistoricalSnapshotDateTime = historicalSnapshotDateTime.Value;
                }
                mainWindow.UpdateNavigationPane(XamNavigationPanes.Servers);
                ShowView(serverView, false);
                serverView.ShowView(subView, argument);
                Settings.Default.AddRecentServer(id);
                setServersSecondaryMenuIndex(GetServerViewTabForServerView(subView));
            }
        }

        static ServerViewTabs GetServerViewTabForServerView(ServerViews serverView)
        {
            switch (serverView)
            {
                case ServerViews.Overview:
                case ServerViews.OverviewSummary:
                case ServerViews.OverviewDetails:
                case ServerViews.OverviewConfiguration:
                case ServerViews.OverviewAlerts:
                    return ServerViewTabs.Overview;                    
                case ServerViews.Sessions:
                case ServerViews.SessionsSummary:
                case ServerViews.SessionsDetails:
                case ServerViews.SessionsLocks:
                case ServerViews.SessionsBlocking:
                    return ServerViewTabs.Sessions;
                case ServerViews.Queries:
                case ServerViews.QueriesSignatures:
                case ServerViews.QueriesStatements:
                case ServerViews.QueriesHistory:
                case ServerViews.QueryWaitStatsActive:
                    return ServerViewTabs.Queries;
                case ServerViews.Resources:
                case ServerViews.ResourcesSummary:
                case ServerViews.ResourcesCpu:
                case ServerViews.ResourcesMemory:
                case ServerViews.ResourcesDisk:
                case ServerViews.ResourcesDiskSize:
                case ServerViews.ResourcesProcedureCache:
                case ServerViews.ResourcesWaitStats:
                case ServerViews.ResourcesFileActivity:
                    return ServerViewTabs.Resources;
                case ServerViews.Databases:
                case ServerViews.DatabasesSummary:
                case ServerViews.DatabaseAlwaysOn:
                case ServerViews.DatabasesConfiguration:
                case ServerViews.DatabasesBackupRestoreHistory:
                case ServerViews.DatabasesTablesIndexes:
                case ServerViews.DatabasesFiles:
                case ServerViews.DatabasesMirroring:
                case ServerViews.DatabasesTempdbView:
                    return ServerViewTabs.Databases;
                case ServerViews.Services:
                case ServerViews.ServicesSummary:
                case ServerViews.ServicesSqlAgentJobs:
                case ServerViews.ServicesFullTextSearch:
                case ServerViews.ServicesReplication:
                    return ServerViewTabs.Services;
                case ServerViews.Logs:
                    return ServerViewTabs.Logs;
                case ServerViews.Analysis:
                    return ServerViewTabs.Analysis;
                case ServerViews.RunAnalysis:
                    return ServerViewTabs.Analysis;
                default:
                    return ServerViewTabs.Overview;
            }
        }

        public void SetRefreshStatusVisible(bool isVisible)
        {
            if (mainWindow != null)
            {
                mainWindow.SetRefreshStatusVisible(isVisible);
            }
        }

        public void SetRefreshStatusText(string text)
        {
            if (mainWindow != null)
            {
                mainWindow.SetRefreshStatusText(text);
            }
        }

        public void ClearCustomStatus()
        {
            if (mainWindow != null)
            {
                mainWindow.ClearCustomStatus();
            }
        }

        public void SetCustomStatus(string statusText)
        {
            SetCustomStatus(statusText, String.Empty);
        }

        public void SetCustomStatus(string statusText1, string statusText2)
        {
            if (mainWindow != null)
            {
                mainWindow.SetCustomStatus(statusText1, statusText2);
            }
        }

        public void SetRefreshOptionsAvailability(bool enabled)
        {
            if (mainWindow != null)
            {
                mainWindow.SetRefreshOptionsAvailablity(enabled);
            }
        }

        public void RefreshBackgroundData()
        {
            using (LOG.DebugCall("RefreshBackgroundData"))
            {
                lock (backgroundRefreshLock)
                {
                    if (backgroundRefreshWorker == null)
                    {
                        backgroundRefreshWorker = new BackgroundWorker();
                        backgroundRefreshWorker.WorkerSupportsCancellation = true;
                        backgroundRefreshWorker.DoWork += new DoWorkEventHandler(backgroundRefreshWorker_DoWork);
                        backgroundRefreshWorker.RunWorkerCompleted +=
                            new RunWorkerCompletedEventHandler(backgroundRefreshWorker_RunWorkerCompleted);
                    }

                    if (!backgroundRefreshWorker.IsBusy)
                    {
                        backgroundRefreshWorker.RunWorkerAsync();
                    }
                    else
                        LOG.Debug("Background refresh worker is busy - refresh skipped!");
                }
            }
        }

        public void LaunchCommunityUrl()
        {
            string communityLink = "https://community.idera.com/database-tools/";

            bool isValidUrl = Uri.IsWellFormedUriString(communityLink, UriKind.RelativeOrAbsolute);

            if (isValidUrl)
                System.Diagnostics.Process.Start(communityLink);
        }
        public void LaunchTrialCenterUrl()
        {
            try
            {
                Process.Start(Settings.Default.TrialCenterUrl);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(mainWindow.GetWinformWindow(), "Failed to launch browser to connect to trial center.", ex);
            }
        }

        public void LaunchProductPurchaseUrl()
        {
            try
            {
                Process.Start(Settings.Default.ProductPurchaseUrl);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(mainWindow.GetWinformWindow(), "Failed to launch browser to connect to ecommerce site.", ex);
            }
        }
        //[START] SQLdm 10.0.2 (Barkha Khatri): Writing LM Registry Keys in Background
        void backgroundLicenseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ManagementServiceHelper.IManagementService.RegisterLicenseManager();
            stopWatch.Stop();
            LOG.DebugFormat("Time taken  by backgroundLicenseWorker_DoWork : {0}", stopWatch.ElapsedMilliseconds);
        }

        void backgroundLicenseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (e.Error is Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException)
            {
                ApplicationMessageBox.ShowError(null,
                                                "An error occurred while attempting to contact the Management Service.",
                                                e.Error);
            }
            else if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(null,
                                                Idera.SQLdm.DesktopClient.Properties.Resources.ExceptionUnhandled,
                                                e.Error);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken  by backgroundLicenseWorker_RunWorkerCompleted : {0}", stopWatch.ElapsedMilliseconds);
        }
        //[END] SQLdm 10.0.2 (Barkha Khatri): Writing LM Registry Keys in Background
        void backgroundRefreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "Background Refresh";
            Debug.Print("Current thread:" + Thread.CurrentThread.Name);
            ApplicationModel.Default.BackgroundThreadRefresh();
        }

        void backgroundRefreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (e.Cancelled)
                return;

            Exception error = e.Error;
            if (error != null)
            {
                LOG.Error("Background refresh failed.", error);
                return;
            }

            try
            {
                UserTokenUpdated();


                if (Settings.Default.ActiveRepositoryConnection != null)
                {
                    foreach (UserView userView in Settings.Default.ActiveRepositoryConnection.UserViews)
                    {
                        if (userView is SearchUserView)
                            userView.Update();
                    }
                }

                FireBackgroundRefreshCompleted();
            }
            finally
            {
                stopWatch.Stop();
                StartUpTimeLog.InfoFormat("Time taken by backgroundRefreshWorker_RunWorkerCompleted : {0} ", stopWatch.ElapsedMilliseconds);
            }
        }
        
        /// <summary>
        /// //SqlDm 10.2 -- Anshul Aggarwal - Saves the user settings in database.
        /// Saves the User Sesison setting in the database (Table : UserSessionSettings)
        /// </summary>
        public void PersistUserSettings()
        {
            using (LOG.InfoCall("PersistUserSettings"))
            {
                lock (backgroundPersistUserSettingsLock)
                {
                    if (backgroundPersistUserSettingsWorker == null)
                    {
                        backgroundPersistUserSettingsWorker = new BackgroundWorker();
                        backgroundPersistUserSettingsWorker.WorkerSupportsCancellation = true;
                        backgroundPersistUserSettingsWorker.DoWork += new DoWorkEventHandler(backgroundPersistUserSettingsWorker_DoWork);
                        backgroundPersistUserSettingsWorker.RunWorkerCompleted +=
                            new RunWorkerCompletedEventHandler(backgroundPersistUserSettingsWorker_RunWorkerCompleted);
                    }

                    if (!backgroundPersistUserSettingsWorker.IsBusy)
                    {
                        backgroundPersistUserSettingsWorker.RunWorkerAsync();
                    }
                    else
                        LOG.Info("Background Persist worker is busy - refresh skipped!");
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Persist User Settings
        /// </summary>
        void backgroundPersistUserSettingsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var userSettings = ApplicationModel.Default.HistoryTimeValue;
            if (userSettings == null || Settings.Default.ActiveRepositoryConnection == null)
                return;
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(HistoryTimeValue));
            using (TextWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, userSettings);
                var settingXml = textWriter.ToString();
                //SP call to add settings to database. Table Name : UserSessionSettings
                RepositoryHelper.SetUserSessionSettings(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, settingXml);
            }

            stopWatch.Stop();
            LOG.InfoFormat("Time taken  by backgroundPersistUserSettingsWorker_DoWork : {0}", stopWatch.ElapsedMilliseconds);
        }

        void backgroundPersistUserSettingsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            Exception error = e.Error;
            if (error != null)
            {
                LOG.Error("Background persist user settings failed.", error);
                return;
            }
        }
        
        public ServerViews GetCurrentServerViewForServer(int instanceId)
        {
            return viewManager.GetServerViewForCachedServer(instanceId);
        }

        public void DeleteCustomReport()
        {
            navigationPaneControl.SecondaryNavigationPane.DeleteCustomReport();
        }
    }

    internal sealed class ActiveViewChangingEventArgs : EventArgs
    {
        public readonly IView OldView;
        public readonly IView NewView;

        public ActiveViewChangingEventArgs(IView oldView, IView newView)
        {
            OldView = oldView;
            NewView = newView;
        }
    }

    internal sealed class AdministrationViewChangedEventArgs : EventArgs
    {
        public readonly AdministrationView.AdministrationNode Node;

        public AdministrationViewChangedEventArgs(AdministrationView.AdministrationNode node)
        {
            Node = node;
        }
    }

    internal sealed class ReportsViewChangedEventArgs : EventArgs
    {
        public readonly ReportTypes NewView;
        private readonly string _reportKey;

        public ReportsViewChangedEventArgs(ReportTypes newView)
        {
            NewView = newView;
        }

        public ReportsViewChangedEventArgs(ReportTypes newView, string reportKey)
        {
            NewView = newView;
            _reportKey = reportKey;
        }
        public string ReportKey
        {
            get { return _reportKey; }
        }
    }

    internal sealed class BackgroundRefreshCompleteEventArgs : EventArgs
    {
        public readonly DateTime Completed;
        public readonly Exception Error;

        public BackgroundRefreshCompleteEventArgs(DateTime completed)
            : this(completed, null)
        {
        }

        public BackgroundRefreshCompleteEventArgs(DateTime completed, Exception error)
        {
            Completed = completed;
            Error = error;
        }
    }

    internal sealed class RefreshActiveViewCompletedEventArgs : EventArgs
    {
        public readonly DateTime Completed;
        public readonly Exception Error;

        public RefreshActiveViewCompletedEventArgs(DateTime completed)
            : this(completed, null)
        {
        }

        public RefreshActiveViewCompletedEventArgs(DateTime completed, Exception error)
        {
            Completed = completed;
            Error = error;
        }
    }
}
