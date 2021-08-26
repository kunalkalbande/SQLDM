using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Reflection;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Controls.Presentation;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Windows.Themes;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.DesktopClient.Helpers;
using Application = System.Windows.Application;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.DesktopClient.Views;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
using Idera.SQLdm.Common.Launch;
using Infragistics.Controls.Menus;
using System.IO;
using Infragistics.Windows.DockManager;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Forms.Integration;
using Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations;
using System.Runtime.InteropServices;

namespace Idera.SQLdm.DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow, IWin32Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const string ApplicationTitleText = "{0} ({2}-{1})";

        private static readonly Logger Log = Logger.GetLogger("MainWindow");
        private static readonly Logger StartUpTimeLog = Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;
        private readonly NotificationPopupWindow _notificationPopupWindow = new NotificationPopupWindow();
        private readonly Dictionary<int, MonitoredState> _instanceStateTracking = new Dictionary<int, MonitoredState>();

        private DispatcherTimer foregroundRefreshTimer;
        private DispatcherTimer backgroundRefreshTimer;

        private Exception refreshError;

        private bool alreadydoingit;
        private MainWindowViewModel _viewModel;

        private WindowState restoreWindowState = WindowState.Normal;
        private GridSplitterPreview _splitterPreview;

        private bool _isClosed;

        private List<int> serverListToSnooze = new List<int>();
        private List<int> serverListToResume = new List<int>();

        //[START] SQldm 9.1 (Gaurav Karwal): populating sql server and view values
        private LaunchParameters _launchParams = null;
        //[END] SQldm 9.1 (Gaurav Karwal): populating sql server and view values

        private CheckBoxButtonMenuItem menuViewNavPaneNormal;
        private CheckBoxButtonMenuItem menuViewNavPaneOff;
        private XamMenuItem menuFileManageServers;
        private XamMenuItem menuGoAdmin;
        private XamMenuItem menuToolsAlertTemplates;
        private XamMenuItem menuFileManageTags;
        private XamMenuSeparator menuFileServersAndTagsSeparator;
        private XamMenuItem menuToolsAlertActions;
        private XamMenuItem menuToolsGrooming;
        private XamMenuItem menuHelpLicenses;
        private CheckBoxButtonMenuItem menuToolsPredictiveAnalytics;
        private XamMenuItem menuToolsVMConfiguration;
        private XamMenuSeparator toolMenuSeparator1;
        private XamMenuSeparator toolMenuSeparator2;
        private XamMenuSeparator toolMenuSeparator3;
        private XamMenuItem maintenanceModeScheduleButtonKey;
        private XamMenuItem menuGoToDo;
        private MainMenuItem toolsMenu;
        private XamMenuItem snoozeAllAlertsToolMenu;
        private XamMenuItem resumeAllAlertsToolMenu;
        private XamMenuItem MaintenanceModeButtonKey;
        private XamNavigationPane xamNavigationPane;
        private CollapsedLeftNavFlyoutHelper myViewsFlyoutHelper;
        private CollapsedLeftNavFlyoutHelper serversFlyoutHelper;
        private CollapsedLeftNavFlyoutHelper tagsFlyoutHelper;
        private CollapsedLeftNavFlyoutHelper searchFlyoutHelper;

        public MainWindow()
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeComponent();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by InitializeComponent() : {0}", stopWatch.ElapsedMilliseconds);

            Height = 960;
            Width = 1280;

            MinWidth = 800;
            MinHeight = 600;

            stopWatch.Reset();
            stopWatch.Start();
            this.DataContext = _viewModel = new MainWindowViewModel(this);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken for creating MainWindowViewModel object : {0}", stopWatch.ElapsedMilliseconds);

            SizeChanged += OnSizeChanged;

            WindowState = (Settings.Default.MainFormState == (int)WindowState.Maximized)
                                 ? WindowState.Maximized
                                 : WindowState.Normal;

            //            OnCurrentThemeChanged(this, EventArgs.Empty);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = Properties.Resources.App16Icon;
            _trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;

            stopWatch.Reset();
            stopWatch.Start();
            AttachTrayMenu();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by AttachTrayMenu() : {0}", stopWatch.ElapsedMilliseconds);

            foregroundRefreshTimer = new DispatcherTimer(DispatcherPriority.Normal);
            foregroundRefreshTimer.Interval = TimeSpan.FromMilliseconds(30000);
            foregroundRefreshTimer.Tick += automatedRefreshTimer_Tick;

            backgroundRefreshTimer = new DispatcherTimer(DispatcherPriority.Background);
            backgroundRefreshTimer.Interval = TimeSpan.FromMilliseconds(60000);
            backgroundRefreshTimer.Tick += backgroundRefreshTimer_Tick;

            UpdateRepositoryConnectionStatus();

            _viewModel.PropertyChanged += Settings_PropertyChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;
            ApplicationController.Default.RefreshActiveViewStarted += ApplicationController_RefreshActiveViewStarted;
            ApplicationController.Default.RefreshActiveViewCompleted += ApplicationController_RefreshActiveViewCompleted;
            ApplicationController.Default.BackgroundRefreshCompleted += ApplicationController_BackgroundRefreshCompleted;

            SetupSavedViewInformation();


            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by MainWindow : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        /// <summary>
        /// SQLdm 9.1 (Gaurav Karwal): 
        /// </summary>
        /// <param name="sqlServerId"></param>
        public MainWindow(LaunchParameters launchParams)
        {
            InitializeComponent();

            //[START] SQldm 9.1 (Gaurav Karwal): populating sql server and view values
            _launchParams = launchParams;
            //[END] SQldm 9.1 (Gaurav Karwal): populating sql server and view values

            Height = 960;
            Width = 1280;

            this.DataContext = _viewModel = new MainWindowViewModel(this);

            SizeChanged += OnSizeChanged;

            WindowState = (Settings.Default.MainFormState == (int)WindowState.Maximized)
                                 ? WindowState.Maximized
                                 : WindowState.Normal;

            //            OnCurrentThemeChanged(this, EventArgs.Empty);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = Properties.Resources.App16Icon;
            _trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
            AttachTrayMenu();

            foregroundRefreshTimer = new DispatcherTimer(DispatcherPriority.Normal);
            foregroundRefreshTimer.Interval = TimeSpan.FromMilliseconds(30000);
            foregroundRefreshTimer.Tick += automatedRefreshTimer_Tick;

            backgroundRefreshTimer = new DispatcherTimer(DispatcherPriority.Background);
            backgroundRefreshTimer.Interval = TimeSpan.FromMilliseconds(60000);
            backgroundRefreshTimer.Tick += backgroundRefreshTimer_Tick;

            UpdateRepositoryConnectionStatus();


            _viewModel.PropertyChanged += Settings_PropertyChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;
            ApplicationController.Default.RefreshActiveViewStarted += ApplicationController_RefreshActiveViewStarted;
            ApplicationController.Default.RefreshActiveViewCompleted += ApplicationController_RefreshActiveViewCompleted;
            ApplicationController.Default.BackgroundRefreshCompleted += ApplicationController_BackgroundRefreshCompleted;
            
            ApplicationController.Default.Initialize(this, xamNavigationPane, viewContainer);
        }

        private string FullPathToViewConfigurationFile(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Idera", "SQLdm", fileName);
        }

        private void LoadCustomViewLayout(string viewFileName)
        {
            try
            {
                var fullPathToViewFile = FullPathToViewConfigurationFile(viewFileName);

                if (File.Exists(fullPathToViewFile))
                {
                    using (FileStream fs = new FileStream(fullPathToViewFile, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            this.dockMgr.LoadLayout(fs);

                            if (_viewModel == null)
                                throw new NullReferenceException("_viewModel is null in MainWindow");

                            _viewModel.SelectedViewFileName = viewFileName;
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Main window dock layout xml existed, but was not valid on app start", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        private void SetupSavedViewInformation()
        {
            
            //check settings if selected file exists
            if (Settings.Default.SelectedViewFileName != null && !String.IsNullOrWhiteSpace(Settings.Default.SelectedViewFileName))
            {
                //if file exists us it to load
                LoadCustomViewLayout(Settings.Default.SelectedViewFileName);
            }

        }
        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            var newTheme = ThemeManager.CurrentTheme;
           
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            UpdateRestoreBounds();
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateRestoreBounds();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            UpdateRestoreBounds();
            UpdateSearchPopupBounds();
        }

       private void UpdateSearchPopupBounds()
        {
            if (this.SearchPopup != null)
            {
                var originalOffset = this.SearchPopup.HorizontalOffset;
                this.SearchPopup.HorizontalOffset = originalOffset + 1;
                this.SearchPopup.HorizontalOffset = originalOffset; // forces Popup position update
            }
            else
            {
                Log.Warn("MainWindow's SearchPopup property is null");
            }
        }

        private void UpdateRestoreBounds()
        {
            if (!IsLoaded) return;

            switch (WindowState)
            {
                case WindowState.Normal:
                    restoreWindowState = WindowState;
                    break;
                case WindowState.Minimized:
                    Visibility = Settings.Default.HideConsoleWhenMinimized ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case WindowState.Maximized:
                    restoreWindowState = WindowState;
                    break;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            menuViewNavPaneNormal = GetTemplateChild("menuViewNavPaneNormal") as CheckBoxButtonMenuItem;
            menuViewNavPaneOff = GetTemplateChild("menuViewNavPaneOff") as CheckBoxButtonMenuItem;
            menuFileManageServers = GetTemplateChild("menuFileManageServers") as XamMenuItem;
            menuGoAdmin = GetTemplateChild("menuGoAdmin") as XamMenuItem;
            menuToolsAlertTemplates = GetTemplateChild("menuToolsAlertTemplates") as XamMenuItem;
            menuFileManageTags = GetTemplateChild("menuFileManageTags") as XamMenuItem;
            menuToolsAlertActions = GetTemplateChild("menuToolsAlertActions") as XamMenuItem;
            menuToolsGrooming = GetTemplateChild("menuToolsGrooming") as XamMenuItem;
            menuHelpLicenses = GetTemplateChild("menuHelpLicenses") as XamMenuItem;
            menuToolsVMConfiguration = GetTemplateChild("menuToolsVMConfiguration") as XamMenuItem;
            maintenanceModeScheduleButtonKey = GetTemplateChild("maintenanceModeScheduleButtonKey") as XamMenuItem;
            menuGoToDo = GetTemplateChild("menuGoToDo") as XamMenuItem;
            snoozeAllAlertsToolMenu = GetTemplateChild("snoozeAllAlertsToolMenu") as XamMenuItem;
            resumeAllAlertsToolMenu = GetTemplateChild("resumeAllAlertsToolMenu") as XamMenuItem;
            MaintenanceModeButtonKey = GetTemplateChild("MaintenanceModeButtonKey") as XamMenuItem;

            menuFileServersAndTagsSeparator = GetTemplateChild("menuFileServersAndTagsSeparator") as XamMenuSeparator;
            toolMenuSeparator1 = GetTemplateChild("toolMenuSeparator1") as XamMenuSeparator;
            toolMenuSeparator2 = GetTemplateChild("toolMenuSeparator2") as XamMenuSeparator;
            toolMenuSeparator3 = GetTemplateChild("toolMenuSeparator3") as XamMenuSeparator;
            menuToolsPredictiveAnalytics = GetTemplateChild("menuToolsPredictiveAnalytics") as CheckBoxButtonMenuItem;

            toolsMenu = GetTemplateChild("toolsMenu") as MainMenuItem;

            xamNavigationPane = GetTemplateChild("xamNavigationPane") as XamNavigationPane;

            ApplicationController.Default.Initialize(this, xamNavigationPane, viewContainer);

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel) return;

            Log.Info("Closing MainForm.");

            _isClosed = true;
            foregroundRefreshTimer.Stop();
            backgroundRefreshTimer.Stop();

            //Idera.Newsfeed.Plugins.UI.PulseController.StopClipboardListener(this);

            // save the splitter location for next time
            var ncw = new double?(0);// _viewModel.IsNavigationPaneVisible ? navigationPaneColumn.Width : navigationColumnRestoreWidth;
            var restoreWidth = ncw.Value;
            if (restoreWidth < 10.0d)
                restoreWidth = 10.0d;

            Settings.Default.MainWindowNavigationPanelSize = restoreWidth;

            if (ApplicationController.Default.ActiveView != null)
            {
                ApplicationController.Default.ActiveView.SaveSettings();
            }

            Settings.Default.SelectedViewFileName = _viewModel.SelectedViewFileName;

            Settings.Default.Save();

            _trayIcon.Dispose();
            
            ApplicationController.Default.CleanUp();

            Application.Current.Shutdown();
        }

        private void automatedRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (ApplicationController.Default.ActiveView is ServerViewContainer)
            {
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void backgroundRefreshTimer_Tick(object sender, EventArgs e)
        {
            ApplicationController.Default.RefreshBackgroundData();

            if (!(ApplicationController.Default.ActiveView is ServerViewContainer))
            {
                ApplicationController.Default.RefreshActiveView(true);
            }
        }

        public bool IsAutomaticRefreshPaused
        {
            get { return !_viewModel.IsForegroundRefreshEnabled; }
        }

        public void PauseAutomaticRefresh()
        {
            _viewModel.IsForegroundRefreshEnabled = false;
        }

        //SQLDM-31025
        public void EnableAutomaticRefresh()
        {
            _viewModel.IsForegroundRefreshEnabled = true;
        }

        public void ShowConsole()
        {
            Visibility = Visibility.Visible;
            // push restoring window state until after visibility change gets a chance to complete
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (MethodInvoker)delegate
                                                                                 {
                                                                                     WindowState = restoreWindowState;
                                                                                     Activate();
                                                                                 });
        }

        private void ApplicationController_BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            if (_isClosed) return;

            UpdateStatus();
            _viewModel.UpdateSelectedObject();
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            SetTrayStatus(MonitoredState.None, 0, 0);
            UpdateRepositoryConnectionStatus();
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HideConsoleWhenMinimized":
                    //               UpdateHideConsoleWhenMinimizedButton();
                    break;
                case "ConsoleAlertNotificationDisplayOption":
                    //               UpdateShowAlertsNotificationsButton();
                    break;
                case "BackgroundRefreshIntervalInMinutes":
                    UpdateBackgroundRefreshTimer();
                    break;
                case "ForegroundRefreshIntervalInSeconds":
                    UpdateForegroundRefreshTimer(false);
                    break;
                case "IsForegroundRefreshEnabled":
                    UpdateAutomatedRefresh();
                    break;
                case "NavigationPaneVisible":
                    // UpdateNavigationPaneMenuOptions(); Removed pane in 10.6
                    break;
                case "LeftNavExpanded":
                    if (_viewModel.LeftNavExpanded)
                    {
                        if (myViewsFlyoutHelper.ShowFlyout)
                        {
                            myViewsFlyoutHelper.ShowFlyout = false;
                            myViewsFlyoutHelper.ShowOrHideFlyout();
                        }
                        if (serversFlyoutHelper.ShowFlyout)
                        {
                            serversFlyoutHelper.ShowFlyout = false;
                            serversFlyoutHelper.ShowOrHideFlyout();
                        }
                        if (tagsFlyoutHelper.ShowFlyout)
                        {
                            tagsFlyoutHelper.ShowFlyout = false;
                            tagsFlyoutHelper.ShowOrHideFlyout();
                        }
                        if (searchFlyoutHelper.ShowFlyout)
                        {
                            searchFlyoutHelper.ShowFlyout = false;
                            searchFlyoutHelper.ShowOrHideSearchFlyout();
                        }
                    }
                    break;
            }
        }


        private void ApplicationController_RefreshActiveViewStarted(object sender, EventArgs e)
        {
            refreshError = null;
            viewRefreshStatusImage.Source = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/ToolbarRefresh.png", UriKind.RelativeOrAbsolute));
            viewRefreshStatusImage.Visibility = Visibility.Visible;
            viewRefreshStatusText.Text = "Refreshing...";
        }

        private void ApplicationController_RefreshActiveViewCompleted(object sender, RefreshActiveViewCompletedEventArgs e)
        {
            if (_isClosed) return;

            if (e.Error == null)
            {
                refreshError = null;

                viewRefreshStatusImage.Visibility = Visibility.Hidden;

                var activeView = ApplicationController.Default.ActiveView as ServerViewContainer;
                if (activeView != null && activeView.HistoricalSnapshotDateTime.HasValue)
                {
                    viewRefreshStatusText.Text = Properties.Resources.HistoryModeStatusBarLabel;
                }
                else
                {
                    viewRefreshStatusText.Text = string.Format("Refreshed: {0}", e.Completed.ToString("G"));
                }
            }
            else
            {
                refreshError = e.Error;
                viewRefreshStatusImage.Visibility = Visibility.Visible;
                viewRefreshStatusImage.Source = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/RefreshError.png", UriKind.RelativeOrAbsolute));
                viewRefreshStatusText.Text = "Refresh Error - click here";
            }
        }

        private void UpdateRepositoryConnectionStatus()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (Settings.Default.ActiveRepositoryConnection != null)
            {
                Title =
                    string.Format(ApplicationTitleText, System.Windows.Forms.Application.ProductName,
                                  Settings.Default.ActiveRepositoryConnection.ConnectionInfo.InstanceName,
                                  Settings.Default.ActiveRepositoryConnection.ConnectionInfo.DatabaseName);
            }
            else
            {
                Title = System.Windows.Forms.Application.ProductName + " - Disconnected";
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by UpdateRepositoryConnectionStatus : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void UpdateStatus()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            MonitoredState highestSeverity = MonitoredState.None;
            int criticalCount = 0;
            int warningCount = 0;

            bool gotStatusForAll = true;
            bool statusChanged = false;
            bool showWarCritServersOnly = false;
            bool showCriticalServersOnly = false;
            bool showAllServerStatuses = true;

            if (ApplicationModel.Default.ActiveInstances.Count > 0)
            {
                ConsoleAlertNotificationDisplayOptions option;
                var changeList = new List<MonitoredSqlServerStatus>();

                try
                {
                    option = (ConsoleAlertNotificationDisplayOptions)Settings.Default.ConsoleAlertNotificationDisplayOption;
                    showWarCritServersOnly = Settings.Default.PopupShowPopupServerInfoWarCrit;
                    showCriticalServersOnly = Settings.Default.PopupShowCriticalServersOnly;
                    showAllServerStatuses = Settings.Default.PopupShowAllServerStatuses;
                }
                catch
                {
                    option = ConsoleAlertNotificationDisplayOptions.AlwaysShow;
                }
                foreach (var repoinstances in ApplicationModel.Default.RepoActiveInstances)
                {
                    foreach (var wrapper in repoinstances.Value)
                    {
                        var status = ApplicationModel.Default.GetInstanceStatus(wrapper.Id,repoinstances.Key);
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

                            if (_instanceStateTracking.TryGetValue(wrapper.Id, out cachedSeverity))
                            {
                                // The state tracking table must be updated regardless of the console
                                // alert notification settings
                                if (cachedSeverity != status.Severity)
                                {
                                    _instanceStateTracking.Remove(wrapper.Id);
                                    _instanceStateTracking.Add(wrapper.Id, status.Severity);
                                    statusChanged = true;
                                }

                                if (option == ConsoleAlertNotificationDisplayOptions.AlwaysShow)
                                {
                                    if (showAllServerStatuses)
                                    {
                                        changeList.Add(status);
                                    }
                                    // only show Critical Servers 
                                    else if (showCriticalServersOnly && status.Severity == MonitoredState.Critical)
                                    {
                                        changeList.Add(status);
                                    }
                                    // add if it is warnig or critical
                                    else if ((status.Severity > MonitoredState.OK) && showWarCritServersOnly)
                                    {
                                        changeList.Add(status);
                                    }
                                }
                                else if (option == ConsoleAlertNotificationDisplayOptions.ShowOnlyOnStateTransition)
                                {
                                    if (cachedSeverity != status.Severity)
                                    {
                                        if (showAllServerStatuses)
                                        {
                                            changeList.Add(status);
                                        }
                                        // only show Critical Servers 
                                        if (showCriticalServersOnly && status.Severity == MonitoredState.Critical)
                                        {
                                            changeList.Add(status);
                                        }
                                        // 
                                        else if (showWarCritServersOnly && status.Severity > MonitoredState.OK)
                                        {
                                            changeList.Add(status);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _instanceStateTracking.Add(wrapper.Id, status.Severity);
                                if (status.Severity > MonitoredState.OK)
                                {
                                    changeList.Add(status);
                                }
                            }
                        }
                        else
                            gotStatusForAll = false;
                    }
                }
                // NOTE: Check to make sure the status bar is shown, which is an indication that
                // the main form is shown, otherwise an exception can occur when the notification
                // popup window is clicked. The result if this behavior is that the popup window
                // is not shown until the second change to the severity list occurs.
                if (IsVisible && option != ConsoleAlertNotificationDisplayOptions.NeverShow)
                {
                    _notificationPopupWindow.SoundAlertEnabled = true;
                    if (Settings.Default.PopupEnableCriticalAlertSound)
                    {
                        if (criticalCount == 0 || !HasServerState(changeList, MonitoredState.Critical))
                            _notificationPopupWindow.SoundAlertEnabled = false;
                    }
                    // Show the notification popup window if we have changes to show
                    if (gotStatusForAll && warningCount == 0 && criticalCount == 0 && statusChanged && !showWarCritServersOnly)
                    {   // be careful about showing 'all servers are ok' - pisses folks off if they aren't
                        _notificationPopupWindow.SetMessage("All monitored instances are OK.");
                        _notificationPopupWindow.Show();
                    }
                    else
                    if (changeList.Count > 0)
                    {
                        _notificationPopupWindow.SetInstanceStatus(changeList);
                        _notificationPopupWindow.Show();
                    }
                }
            }

            SetTrayStatus(highestSeverity, criticalCount, warningCount);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by UpdateConsoleStatus() : {0}", stopWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Verify if exists at least one given MonitoredState in the statusList.
        /// </summary>
        /// <param name="statusList">The status list.</param>
        /// <param name="state">The state to count.</param>
        /// <returns></returns>
        private bool HasServerState(IEnumerable<MonitoredSqlServerStatus> statusList, MonitoredState state)
        {
            bool sstatusCounter = false;
            foreach (MonitoredSqlServerStatus serverStatus in statusList)
            {
                if (serverStatus.Severity == state)
                {
                    sstatusCounter = true;
                    break;
                }
            }
            return sstatusCounter;
        }
        public void SetRefreshOptionsAvailablity(bool enabled)
        {
            _viewModel.IsRefreshOptionsEnabled = enabled;
        }

        public void SetRefreshStatusVisible(bool isVisible)
        {
            viewRefreshStatusItem.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetRefreshStatusText(string text)
        {
            viewRefreshStatusImage.Visibility = Visibility.Hidden;
            viewRefreshStatusText.Text = text;
        }

        public void ClearCustomStatus()
        {
            SetCustomStatus(String.Empty, String.Empty);
        }

        public void SetCustomStatus(string statusText1, string statusText2)
        {
            customStatusText1.Text = statusText1;
            customStatusText2.Text = statusText2;
        }

        private void viewRefreshStatusItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (refreshError != null)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this, "An error occurred while refreshing the active view.", refreshError, false);
            }
        }

        public void SetRefreshOptionsAvailability(bool enabled)
        {
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU); //To disable default control buttons
            using (Log.DebugCall("OnWindowLoaded"))
            {
                _trayIcon.Visible = true;
                
                UpdateBackgroundRefreshTimer();
                UpdateForegroundRefreshTimer(true);

                if (_launchParams != null)
                {
                    //[START] SQldm 9.1 (Gaurav Karwal): launching the intended view from Web UI
                    if (_launchParams is AlertLaunchParameters)
                        ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Active, new AlertFilter() { Instance = ((AlertLaunchParameters)_launchParams).InstanceName, AlertID = ((AlertLaunchParameters)_launchParams).AlertId });
                    else if (_launchParams is InstanceLaunchParameters)
                    {
                        ApplicationModel.Default.RefreshActiveInstances();
                        ApplicationController.Default.ShowServerView(((InstanceLaunchParameters)_launchParams).InstanceId, ServerViews.Overview);
                    }
                    /// SQLdm10.2 (srishti purohit )defect fix
                    /// SQLDM-27637('Prescriptive Analysis Summary' component click not working)
                    else if (_launchParams is AnalysisLaunchParameters)
                    {
                        ApplicationModel.Default.RefreshActiveInstances();
                        ApplicationController.Default.ShowServerView(((AnalysisLaunchParameters)_launchParams).InstanceId, ServerViews.Analysis);
                    }
                }

                //[END] SQldm 9.1 (Gaurav Karwal): launching the intended view from Web UI

                Topmost = true;

                // update the color scheme based on user setting
                ThemeName colorScheme = ThemeName.Light;
                Enum.TryParse<ThemeName>(Settings.Default.ColorScheme, true, out colorScheme);

                ApplicationController.Default.ChangeTheme(colorScheme);
                
                _viewModel.UpdatePredictiveAnalyticsState();

                Topmost = false;
                var templateSearchResult =this.Template.FindName("SearchPopups",this);
                if (templateSearchResult != null && templateSearchResult is Popup)
                    SearchPopup = templateSearchResult as Popup;
                else
                    Log.Warn("MainWindow's SearchPopup is null when window loaded");
                myViewsFlyoutHelper = new CollapsedLeftNavFlyoutHelper(CollapsedMyViewsButtonGrid, this.xamNavigationPane.ServersNavigationPane.UserViewsTreeView, this.xamNavigationPane.ServersNavigationPane.UserViewsHeader, this.xamNavigationPane.ServersNavigationPane.UserViewsHeader.Items[1]);
                serversFlyoutHelper = new CollapsedLeftNavFlyoutHelper(CollapsedServersButtonGrid, this.xamNavigationPane.ServersNavigationPane.UserViewTreeView, this.xamNavigationPane.ServersNavigationPane.UserViewHeader);
                tagsFlyoutHelper = new CollapsedLeftNavFlyoutHelper(CollapsedTagsButtonGrid, this.xamNavigationPane.ServersNavigationPane.TagsTreeView, this.xamNavigationPane.ServersNavigationPane.TagsHeader, this.xamNavigationPane.ServersNavigationPane.TagsHeader.Items[1], this.xamNavigationPane.ServersNavigationPane.ManageTagsLabel);
                searchFlyoutHelper = new CollapsedLeftNavFlyoutHelper(CollapsedSearchButtonGrid, this.SearchGrid, this.SearchTextBox, this.SearchGridDock,this.SearchPopup);
            }
        }
        //SqlDm 10.2 (Tushar)--Calling the NewSqlDMTodayView when content is rendered to reduce the gap between splash screen closure and UI.
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (ApplicationController.Default.ActiveView == null)
                ApplicationController.Default.ShowTodayView();
        }

       
        private void UpdateTheme()
        {
            var mainMenuBorderBrush = Application.Current.Resources["MenuItemWhiteForegroundBrush"];
            if (Settings.Default.ColorScheme.Contains("Silver"))
            {
                ThemeManager.CurrentTheme = ThemeManager.ThemeNameOffice2k7Silver;
                mainMenuBorderBrush = Application.Current.Resources["MenuItemBlackForegroundBrush"];
            }
            else
            {
                ThemeManager.CurrentTheme = ThemeManager.ThemeNameOffice2k7Black;
            }
        }


        private void UpdateBackgroundRefreshTimer()
        {
            
            var interval = TimeSpan.FromMinutes(Settings.Default.BackgroundRefreshIntervalInMinutes);

            backgroundRefreshTimer.Stop();
            backgroundRefreshTimer.Interval = interval;
            backgroundRefreshTimer.Start();

            Log.DebugFormat("Status & Alerts refresh interval set to {0} minutes.", interval.TotalMinutes);
        }

        private void UpdateForegroundRefreshTimer(bool isLoadStart)
        {
            var interval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds);

            // If the timer is enabled or the function was called during form load, then start
            // the timer.   Else just change the interval.
            if (foregroundRefreshTimer.IsEnabled || isLoadStart)
            {
                foregroundRefreshTimer.Stop();
                foregroundRefreshTimer.Interval = interval;
                foregroundRefreshTimer.Start();
            }
            else
            {
                foregroundRefreshTimer.Interval = interval;
            }

            _viewModel.IsForegroundRefreshEnabled = foregroundRefreshTimer.IsEnabled;

            Log.DebugFormat("Server View refresh interval set to {0} seconds.", interval.TotalSeconds);
        }


        #region tray

        void trayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ShowConsole();
        }

        void AttachTrayMenu()
        {
            _trayMenu = new ContextMenuStrip();
            _trayMenu.Opening += _trayMenu_Opening;
            var checkButton1 = _trayMenu.Items.Add("Show Alert Notifications", null, OnShowAlertNotificationsChanged);
            checkButton1.Name = "ShowAlertNotifications";
            _trayMenu.Items.Add(new ToolStripSeparator());
            var checkButton2 = _trayMenu.Items.Add("Hide When Minimized", null, OnHideWhenMinimized);
            checkButton2.Name = "HideWhenMinimized";
            var checkButton3 = _trayMenu.Items.Add("Open SQL Diagnostic Manager", null, (sender, args) => ShowConsole());
            checkButton3.Name = "ShowConsole";
            _trayIcon.ContextMenuStrip = _trayMenu;
        }

        void _trayMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {   // update checkbox state before showing menu
            var checkButton1 = (ToolStripMenuItem)_trayMenu.Items["ShowAlertNotifications"];
            checkButton1.Checked = Settings.Default.ConsoleAlertNotificationDisplayOption != (int)ConsoleAlertNotificationDisplayOptions.NeverShow;
            var checkButton2 = (ToolStripMenuItem)_trayMenu.Items["HideWhenMinimized"];
            checkButton2.Checked = Settings.Default.HideConsoleWhenMinimized;
        }

        void OnShowAlertNotificationsChanged(object sender, EventArgs args)
        {
            if (Settings.Default.ConsoleAlertNotificationDisplayOption == (int)ConsoleAlertNotificationDisplayOptions.NeverShow)
                Settings.Default.ConsoleAlertNotificationDisplayOption = (int)ConsoleAlertNotificationDisplayOptions.AlwaysShow;
            else
                Settings.Default.ConsoleAlertNotificationDisplayOption = (int)ConsoleAlertNotificationDisplayOptions.NeverShow;
        }

        void OnHideWhenMinimized(object sender, EventArgs args)
        {
            bool hideWhenMinimized = !Settings.Default.HideConsoleWhenMinimized;
            Settings.Default.HideConsoleWhenMinimized = hideWhenMinimized;
            if (WindowState == WindowState.Minimized)
            {
                Visibility = hideWhenMinimized ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void SetTrayStatus(MonitoredState state, int criticalCount, int warningCount)
        {
            if (_isClosed) return;

            string stateIconText = System.Windows.Forms.Application.ProductName + " - {0}";

            _trayIcon.Tag = state;

            switch (state)
            {
                case MonitoredState.Critical:
                    _trayIcon.Text = string.Format(stateIconText, criticalCount + " Critical Instances");
                    _trayIcon.Icon = Properties.Resources.App16CriticalIcon;
                    break;
                case MonitoredState.Warning:
                    _trayIcon.Text = string.Format(stateIconText, warningCount + " Warning Instances");
                    _trayIcon.Icon = Properties.Resources.App16WarningIcon;
                    break;
                case MonitoredState.OK:
                    _trayIcon.Text = string.Format(stateIconText, "All Instances OK");
                    _trayIcon.Icon = Properties.Resources.App16OKIcon;
                    break;
                default:
                    _trayIcon.Text = System.Windows.Forms.Application.ProductName;
                    _trayIcon.Icon = Properties.Resources.App16Icon;
                    break;
            }
        }

        #endregion

        #region toolbar events

        private void toolBarTodayButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationController.Default.ShowTodayView();
        }

        private void toolBarRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationController.Default.RefreshActiveView(false);
        }

        private void toolbarHelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationController.Default.ActiveView == null)
            {
                ApplicationHelper.ShowHelpTopic(HelpTopics.HelpStartPage);
            }
            else
            {
                ApplicationController.Default.ActiveView.ShowHelp();
            }
        }

        #endregion

        #region menu

        private void OnMenuItemClicked(object sender, RoutedEventArgs e)
        {
            var item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            if (String.IsNullOrEmpty(item.Name)) return;

            switch (item.Name)
            {
                case "menuFile":
                    _viewModel.UpdateSelectedObject();
                    break;
                case "menuFileConnect":
                    ShowRepositoryConnectionDialog();
                    break;
                case "menuFileManageServers":
                    ShowManageServersDialog();
                    break;
                case "menuFileManageTags":
                    ShowManageTagsDialog();
                    break;
                case "menuFileCreateView":
                    ShowAddUserViewDialog();
                    break;
                case "menuFileDeleteServer":
                    DeleteFocusObject();
                    break;
                case "menuFileServerProperties":
                    ShowFocusObjectProperties();
                    break;
                case "menuFileExit":
                    ExitApplication();
                    break;
                case "menuViewRefresh":
                    ApplicationController.Default.RefreshActiveView();
                    break;
                case "menuViewPauseRefresh":
                    //            UpdateAutomatedRefresh();
                    break;
                case "menuViewNavPaneNormal":
                    Settings.Default.NavigationPaneVisible = true;
                    break;
                case "menuViewNavPaneOff":
                    Settings.Default.NavigationPaneVisible = false;
                    break;
                case "menuGoAdmin":
                    break;
                case "menuGoAlerts":
                    break;
                case "menuGoNewsfeed":
                    break;
                case "menuGoReports":
                    break;
                case "menuGoServers":
                    break;
                case "menuGoToday":
                    ApplicationController.Default.ShowTodayView();
                    break;
                case "menuGoToDo":
                    break;
                case "menuToolsAlertActions":
                    ShowNotificationRulesDialog();
                    break;
                case "menuToolsAlertTemplates":
                    ShowAlertConfigurationDialog();
                    break;
                case "menuToolsConsole":
                    ShowConsoleOptionsDialog();
                    break;
                case "menuToolsGrooming":
                    ShowGroomingOptionsDialog();
                    break;
                case "menuToolsPredictiveAnalytics":
                    TogglePredictiveAnalyticsService();
                    break;
                case "menuToolsVMConfiguration":
                    ShowVirtualizationConfigDialog();
                    break;
                case "menuHelpAbout":
                case "menuHelpAboutH":
                    ShowAboutBox();
                    break;
                case "menuHelpCheckUpdatesH":
                case "menuHelpCheckUpdates":
                    Process.Start(string.Format("{0}{1}", Common.Constants.CheckForUpdatesUrl, Common.Constants.VersionCheckProductId));
                    break;
                case "menuHelpContactSupportH":
                case "menuHelpContactSupport":
                    Process.Start(Common.Constants.ContactSupportUrl);
                    break;
                case "menuHelpContentsH":
                case "menuHelpContents":
                    Help.ShowHelp(new Form(), Common.Constants.HelpFileName, HelpNavigator.TableOfContents);
                    break;
                case "menuHelpDiagnosticsH":
                case "menuHelpDiagnostics":
                    using (var diagDialog = new SystemDiagnosticsDialog())
                    {
                        diagDialog.ShowDialog((WinformWindow)this);
                    }
                    break;
                case "menuHelpHowTo1H":
                case "menuHelpHowTo1":
                    ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic1);
                    break;
                case "menuHelpHowTo2H":
                case "menuHelpHowTo2":
                    ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic2);
                    break;
                case "menuHelpHowTo3H":
                case "menuHelpHowTo3":
                    ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic3);
                    break;
                case "menuHelpHowTo4H":
                case "menuHelpHowTo4":
                    ApplicationHelper.ShowHelpTopic(HelpTopics.HowToTopic4);
                    break;
                case "menuHelpIderaProductsH":
                case "menuHelpIderaProducts":
                    Process.Start(Common.Constants.AboutIderaProductsUrl);
                    break;
                case "menuHelpIndexH":
                case "menuHelpIndex":
                    Help.ShowHelp(new Form(), Common.Constants.HelpFileName, HelpNavigator.Index);
                    break;
                case "menuHelpLicensesH":
                case "menuHelpLicenses":
                    LicenseDialog.Display((WinformWindow)this);
                    break;
                case "menuHelpSearchH":
                case "menuHelpSearch":
                    Help.ShowHelp(new Form(), Common.Constants.HelpFileName, HelpNavigator.Find, string.Empty);
                    break;
                case "menuHelpSearchKBH":
                case "menuHelpSearchKB":
                    Process.Start(Common.Constants.SearchKnowledgeBaseUrl);
                    break;
                //SQLdm 9.1 (Vineet Kumar) (Community Integration) -- Add community link to help menu
                case "menuHelpVisitCommunityCenterH":
                case "menuHelpVisitCommunityCenter":
                    Process.Start(Common.Constants.CommunitySiteBaseUrl);
                    break;
                case "menuHelpThisWindowH":
                case "menuHelpThisWindow":
                    ShowActiveViewHelp();
                    break;
                case "menuHelpVisitTrialCenterH":
                case "menuHelpVisitTrialCenter":
                    ApplicationController.Default.LaunchTrialCenterUrl();
                    break;
                case "snoozeAllAlertsToolMenu":
                    SnoozeAllServerAlerts();
                    break;
                case "resumeAllAlertsToolMenu":
                    ResumeAllServerAlerts();
                    break;
                case TextConstants.MaintenanceModeEnableButtonKey:
                    MaintenanceModeAllNow(true);
                    break;
                case TextConstants.MaintenanceModeDisableButtonKey:
                    MaintenanceModeAllNow(false);
                    break;
                case TextConstants.MaintenanceModeScheduleButtonKey:
                    MaintenanceModeScheduleMode();
                    break;
                default:
                    // No op
                    break;

            }

            e.Handled = true;
        }

        private void MaintenanceModeAllNow(bool status)
        {
            Dictionary<int, MonitoredSqlServerWrapper> servers = new Dictionary<int, MonitoredSqlServerWrapper>();

            servers = ApplicationModel.Default.ActiveInstances.ToDictionary(a => a.Id);

            if (servers.Count <= 0)
            {
                return;
            }

            foreach (var server in servers)
            {
                MonitoredSqlServerStatus serverStatus = ApplicationModel.Default.GetInstanceStatus(server.Key,server.Value.RepoId);
                MonitoredSqlServerConfiguration config = serverStatus.Instance.Instance.GetConfiguration();

                MaintenanceModeType modeType = status ? MaintenanceModeType.Always : MaintenanceModeType.Never;

                config.MaintenanceMode.MaintenanceModeType = modeType;
                config.MaintenanceModeEnabled = status;

                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                ApplicationModel.Default.UpdateMonitoredSqlServer(server.Key, config);

                //Then actually turn off or on maintenance mode.
                ApplicationModel.Default.ConfigureMaintenanceMode(server.Key, status);
            }
        }

        private void MaintenanceModeScheduleMode()
        {
            MassMaintenanceModeDialog dialog = dialog = new MassMaintenanceModeDialog();

            DialogResult result = dialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var server in dialog.SelectedServers())
                {
                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                    try
                    {
                        ApplicationModel.Default.UpdateMonitoredSqlServer(server.Key, server.Value);
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        "An error occurred while changing maintenance mode status.", e);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Snooze all alerts for the instances contained in 'serverListToSnooze'.
        /// </summary>
        private void SnoozeAllServerAlerts()
        {
            if (this.serverListToSnooze != null && this.serverListToSnooze.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Snooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(this, this.serverListToSnooze, action, "All Servers");
            }
        }

        /// <summary>
        /// Resume all alerts for the instances contained in 'serverListToSnooze'.
        /// </summary>
        private void ResumeAllServerAlerts()
        {
            if (this.serverListToResume != null && this.serverListToResume.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Unsnooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(this, this.serverListToResume, action, "All Servers");
            }
        }

        private void UpdateAutomatedRefresh()
        {
            foregroundRefreshTimer.IsEnabled = _viewModel.IsForegroundRefreshEnabled;
            if (foregroundRefreshTimer.IsEnabled)
            {
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void ShowActiveViewHelp()
        {
            // Show the help topic for the current view.
            if (ApplicationController.Default.ActiveView == null)
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HelpStartPage);
            }
            else
            {
                ApplicationController.Default.ActiveView.ShowHelp();
            }
        }

        private static void ExitApplication()
        {
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Exit();
        }

        private void ShowManageServersDialog()
        {
            ManageServersDialog manageServersDialog = new ManageServersDialog();
            manageServersDialog.ShowDialog((WinformWindow)this);
        }

        private void ShowManageTagsDialog()
        {
            ManageTagsDialog manageTagsDialog = new ManageTagsDialog();
            manageTagsDialog.ShowDialog((WinformWindow)this);
        }

        private void ShowAddUserViewDialog()
        {
            StaticViewPropertiesDialog dialog = new StaticViewPropertiesDialog();
            dialog.ShowDialog((WinformWindow)this);
        }

        private void ShowRepositoryConnectionDialog()
        {
            RepositoryConnectionDialog dialog = new RepositoryConnectionDialog();

            if (dialog.ShowDialog((WinformWindow)this) == System.Windows.Forms.DialogResult.OK)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator &&
                    Settings.Default.ActiveRepositoryConnection.RepositoryInfo.MonitoredServerCount == 0)
                {
                    GettingStartedWizard gettingStartedWizard = new GettingStartedWizard();
                    gettingStartedWizard.ShowInTaskbar = false;
                    gettingStartedWizard.ShowDialog((WinformWindow)this);
                }
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
                    dialog.ShowDialog((WinformWindow)this);
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
                            dialog.ShowDialog((WinformWindow)this);
                        }
                        catch (Exception e)
                        {
                            ApplicationMessageBox.ShowError((WinformWindow)this,
                                                            string.Format("Unable to show properties for {0}.",
                                                                          instance), e);
                        }
                    }
                }
                else if (ApplicationModel.Default.FocusObject is Tag)
                {
                    TagPropertiesDialog dialog = new TagPropertiesDialog(ApplicationModel.Default.FocusObject as Tag);
                    dialog.ShowDialog((WinformWindow)this);
                }
            }
        }

        private void DeleteFocusObject()
        {
            //focused = GetFocusedControl(Controls);
            //if (ApplicationModel.Default.FocusObject != null)
            //{
            //    if (ApplicationModel.Default.FocusObject is UserView &&
            //        navigationPaneControl.SelectedPane == NavigationPanes.Servers)
            //    {
            //        DeleteUserView(ApplicationModel.Default.FocusObject as UserView);
            //    }
            //    else if (ApplicationModel.Default.FocusObject is Tag &&
            //        navigationPaneControl.SelectedPane == NavigationPanes.Servers)
            //    {
            //        DeleteTagView(ApplicationModel.Default.FocusObject as Tag);
            //    }
            //    else if (ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper &&
            //        navigationPaneControl.SelectedPane == NavigationPanes.Servers && !(focused is TextBox) && !(focused is DateTimePicker))
            //    {
            //        DeleteInstance(ApplicationModel.Default.FocusObject as MonitoredSqlServerWrapper);
            //    }
            //}
        }

        private void DeleteTagView(Tag tag)
        {
            if (tag != null)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowWarning((WinformWindow)this,
                                                      "Deleting a tag will remove all links to associated SQLDM objects. If custom counters or permissions are associated with servers via tags, custom counters may no longer be collected for tagged servers and permissions may not longer apply. You can use the Manage Tags dialog to view all SQLDM objects associated with this tag. Would like to proceed with deleting this tag?",
                                                      ExceptionMessageBoxButtons.YesNo);

                if (dialogResult != System.Windows.Forms.DialogResult.No)
                {
                    try
                    {
                        ApplicationModel.Default.RemoveTags(new Tag[] { tag });
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)this,
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
                    ApplicationMessageBox.ShowWarning((WinformWindow)this,
                                                      string.Format(
                                                          "Are you sure you want to delete the view \"{0}\"?",
                                                          userView.Name), ExceptionMessageBoxButtons.YesNo) ==
                    System.Windows.Forms.DialogResult.Yes)
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
                    ApplicationMessageBox.ShowWarning((WinformWindow)this,
                                                      "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                      "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                      "Would you like to retain the data collected for the selected instance(s)?",
                                                      ExceptionMessageBoxButtons.YesNoCancel);

                if (dialogResult != System.Windows.Forms.DialogResult.Cancel)
                {
                    try
                    {
                        if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        {
                            ApplicationModel.Default.DeactivateMonitoredSqlServers(new MonitoredSqlServerWrapper[] { instance });
                        }
                        else if (dialogResult == System.Windows.Forms.DialogResult.No)
                        {
                            ApplicationModel.Default.DeleteMonitoredSqlServers(new MonitoredSqlServerWrapper[] { instance });
                        }
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)this,
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
                    dialog.ShowDialog((WinformWindow)this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this,
                                                "Unable to retrieve the grooming options from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private void ShowConsoleOptionsDialog()
        {
            using (ConsoleOptionsDialog consoleOptionsDialog = new ConsoleOptionsDialog())
            {
                consoleOptionsDialog.ShowDialog((WinformWindow)this);
            }
        }

        private void ShowAddServersWizard()
        {
            Dictionary<string, object> existingServers = new Dictionary<string, object>();

            foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
            {
                existingServers.Add(instance.InstanceName, existingServers);
            }

            AddServersWizard wizard = new AddServersWizard(true, existingServers);
            wizard.ShowDialog((WinformWindow)this);
        }

        private void ShowAlertConfigurationDialog()
        {
            try
            {
                using (AlertTemplateDialog alertTempalteDialog = new AlertTemplateDialog())
                {
                    alertTempalteDialog.ShowDialog((WinformWindow)this);
                }
                //using (AlertConfigurationDialog alertConfigDialog = new AlertConfigurationDialog(0, true))
                //{
                //    alertConfigDialog.ShowDialog(this);
                //}
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this,
                                                "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }

        }

        private void ShowVirtualizationConfigDialog()
        {
            try
            {
                using (VirtualizationConfig virtualizationConfig = new VirtualizationConfig())
                {
                    virtualizationConfig.ShowDialog((WinformWindow)this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this, "Error in the Virtualization Configuration Dialog", ex);
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
                    notificationRulesDialog.ShowDialog((WinformWindow)this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this,
                                                "Unable to retrieve the list of notification rules.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private void ShowAboutBox()
        {
            AboutDialog dialog = new AboutDialog();
            dialog.ShowDialog((WinformWindow)this);
        }

        private void TogglePredictiveAnalyticsService()
        {
            bool newstate = !_viewModel.IsPredictiveAnalyticsEnabled;

            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService();

                AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                // enable PA service
                managementService.SetPredictiveAnalyticsEnabled(newstate);

                _viewModel.UpdatePredictiveAnalyticsState();

                // Let them know we were successful
                if (_viewModel.IsPredictiveAnalyticsEnabled == newstate)
                {
                    ApplicationModel.Default.NotifyPredictiveAnaltyicsStateChanged();
                    System.Windows.MessageBox.Show(string.Format("Predictive Analytics has been {0}.", newstate ? "enabled" : "disabled"),
                                                   "Success Dialog",
                                                   MessageBoxButton.OK,
                                                   MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError((WinformWindow)this, "Failed to change the state Predictive Analytics Service.", ex);
            }
        }


        
        private void XamMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var visibleToAdmin = ApplicationModel.Default.UserToken.IsSQLdmAdministrator ? Visibility.Visible : Visibility.Collapsed;
            menuFileManageServers.Visibility = visibleToAdmin;
            menuFileManageTags.Visibility = visibleToAdmin;
            menuFileServersAndTagsSeparator.Visibility = visibleToAdmin;
            
            menuToolsAlertTemplates.Visibility = visibleToAdmin;
            menuToolsAlertActions.Visibility = visibleToAdmin;
            menuToolsGrooming.Visibility = visibleToAdmin;
            
            menuToolsPredictiveAnalytics.Visibility = visibleToAdmin;
            menuToolsVMConfiguration.Visibility = visibleToAdmin;

            toolMenuSeparator1.Visibility = toolMenuSeparator2.Visibility = toolMenuSeparator3.Visibility = visibleToAdmin;

            _viewModel.UpdatePredictiveAnalyticsState();

            _viewModel.PrepareSnoozeAndUnsnoozeOperation();

            maintenanceModeScheduleButtonKey.Visibility = maintenanceModeScheduleButtonVisibility() ? Visibility.Visible : Visibility.Collapsed;

            RefreshMaintenanceModeToolMenus();

        }

        // SQLDM 10.3 - SQLDM-28675 - Operator security role changes
        /// <summary>
        /// returns a boolean value that helps to set the visibilty of maintenance Mode Schedule Button
        /// </summary>
        /// <returns></returns>
        private bool maintenanceModeScheduleButtonVisibility()
        {
            ICollection<int> instancesIds = ApplicationModel.Default.ActiveInstances.Keys;
            //when the application loads on the system for the first time with no instance, the default role at that point of time is Administrator hence returning true
            if (instancesIds == null)
            {
                return true;
            }
            return ApplicationModel.Default.UserToken.IsHasModifyPermission(instancesIds.ToList());
        }

        

        void RefreshMaintenanceModeToolMenus()
        {
            ICollection<int> instancesIds = ApplicationModel.Default.ActiveInstances.Keys;

            if (instancesIds != null && instancesIds.Count > 0)
            {
                if (ApplicationModel.Default.UserToken.IsHasModifyServerPermission(instancesIds.ToList()))
                {
                    if (!this.toolsMenu.Items.Contains(MaintenanceModeButtonKey))
                    {
                        this.toolsMenu.Items.Add(MaintenanceModeButtonKey);
                    }
                }
                else
                {
                    this.toolsMenu.Items.Remove(MaintenanceModeButtonKey);
                }
            }
            else
            {
                this.toolsMenu.Items.Remove(MaintenanceModeButtonKey);
            }
        }

        private void trialcentergraphic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ApplicationController.Default.LaunchTrialCenterUrl();
        }
        
        private GridLength navigationColumnRestoreWidth = new GridLength(200.0d);
        
        private void OnHelpRequested(object sender, ExecutedRoutedEventArgs e)
        {
            ShowActiveViewHelp();
        }

        private void OnRefreshRequested(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationController.Default.RefreshActiveView();
        }

        private void OnGotoPageRequested(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter.ToString())
            {
                case "6":
                    //ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Administration;
                    break;
                case "2":
                    //ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Alerts;
                    break;
                case "3":
                    //ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Pulse;
                    break;
                case "5":
                    ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Reports;
                    break;
                case "1":
                    ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Servers;
                    break;
                case "4":
                    //ApplicationController.Default.SelectedNavigationPane = XamNavigationPanes.Tasks;
                    break;
                case "ToggleNavPane":
                    Settings.Default.NavigationPaneVisible = !Settings.Default.NavigationPaneVisible;
                    break;
            }
        }

        /// <summary>
        /// Gets the handle to the window represented by the implementer.
        /// </summary>
        /// <returns>
        /// A handle to the window represented by the implementer.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IntPtr Handle
        {
            get
            {
                var interopHelper = new WindowInteropHelper(this);
                return interopHelper.Handle;
            }
        }

        
        private void MenuLight_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ColorScheme = "Light";
            ApplicationController.Default.ChangeTheme(ThemeName.Light);
            var flyoutPanelBackgroundColor = System.Drawing.Color.White;
            setFlyoutsPanelsBackgroundColor(flyoutPanelBackgroundColor);
        }

        private void MenuDark_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ColorScheme = "Dark";
            ApplicationController.Default.ChangeTheme(ThemeName.Dark);
            var flyoutPanelBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#072d40");
            setFlyoutsPanelsBackgroundColor(flyoutPanelBackgroundColor);
        }

        private void setFlyoutsPanelsBackgroundColor(System.Drawing.Color color)
        {
            if (this.myViewsFlyoutHelper != null)
            {
                this.myViewsFlyoutHelper.SetBackgroundColor(color);
            }
            if (this.tagsFlyoutHelper != null)
            {
                this.tagsFlyoutHelper.SetBackgroundColor(color);
            }
            if (this.serversFlyoutHelper != null)
            {
                this.serversFlyoutHelper.SetBackgroundColor(color);
            }
            if (this.searchFlyoutHelper != null)
            {
                this.searchFlyoutHelper.SetBackgroundColor(color);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {


        }

        private void AlertsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ShowDefaultOrExisting);
        }

        private void LoggedInMenu_Click(object sender, RoutedEventArgs e)
        {
            ApplicationController.Default.LaunchCommunityUrl();
        }
        
        private void LeftNavPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setLeftNavPaneMinWidth();
            if (LeftNavPane.ActualWidth <= 96)
            {
                _viewModel.LeftNavExpanded = false;
                this.SearchGrid.Visibility = Visibility.Collapsed;
            }
            else 
            {
                this.SearchGrid.Visibility = Visibility.Visible;
                _viewModel.LeftNavExpanded = true;
            }
            if (xamNavigationPane.topMenu.SelectedIndex == (int)XamNavigationPanes.Alerts)
            {
                this.SearchGrid.Visibility = Visibility.Collapsed;
                _viewModel.LeftNavExpanded = true;
            }
        }

        private void setLeftNavPaneMinWidth()
        {
            int minWidth = 48;
            if (Convert.ToInt32(LeftNavPane.MinWidth) != minWidth)
                LeftNavPane.MinWidth = minWidth;
            var parent = LeftNavPane.Parent as SplitPane;
            if (parent != null && Convert.ToInt32((LeftNavPane.Parent as SplitPane).MinWidth) != minWidth)
                parent.MinWidth = minWidth;
        }
       
        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)SearchPopup.Child;
            SearchItem selectedSearchItem = listBox.SelectedItem as SearchItem;
            if (selectedSearchItem != null)
            {
                if (selectedSearchItem.Type == SearchItemType.Server)
                {
                    var id = (int)selectedSearchItem.Id;
                    ApplicationController.Default.ShowServerView(id);
                }
                if (selectedSearchItem.Type == SearchItemType.Tag)
                {
                    var id = (int)selectedSearchItem.Id;
                    var tag = ApplicationModel.Default.Tags.FirstOrDefault(x => x.Id == id);
                    if (tag == null)
                    {
                        Log.Error("Tag was in search results, but not found in the ApplicationModel");
                        return;
                    }
                    ApplicationModel.Default.FocusObject = tag;
                    ApplicationController.Default.ShowTagView(id);
                }
                if (selectedSearchItem.Type == SearchItemType.UserView)
                {
                    var id = (Guid)selectedSearchItem.Id;
                    var userView = Settings.Default.ActiveRepositoryConnection.UserViews.FirstOrDefault(x => Guid.Equals(id, x.Id));
                    if (userView == null)
                    {
                        Log.Error("Userview was in search results, but not found in Settings.Default.ActiveRepositoryConnection.UserViews");
                        return;
                    }
                    ApplicationModel.Default.FocusObject = userView;
                    ApplicationController.Default.ShowUserView(id);
                }
                _viewModel.SearchText = null;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var newFocusedElement = FocusManager.GetFocusedElement(this);
            if (newFocusedElement is System.Windows.Controls.ListBoxItem)
                return;
        }
        private void CollapsedSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            searchFlyoutHelper.ShowFlyout = !searchFlyoutHelper.ShowFlyout;
            if (searchFlyoutHelper.ShowFlyout && (serversFlyoutHelper.ShowFlyout || tagsFlyoutHelper.ShowFlyout || myViewsFlyoutHelper.ShowFlyout))
            {
                serversFlyoutHelper.ShowFlyout = false;
                serversFlyoutHelper.ShowOrHideFlyout();
                tagsFlyoutHelper.ShowFlyout = false;
                tagsFlyoutHelper.ShowOrHideFlyout();
                myViewsFlyoutHelper.ShowFlyout = false;
                myViewsFlyoutHelper.ShowOrHideFlyout();
            }
            if (searchFlyoutHelper.ShowFlyout && this.xamNavigationPane.ServersNavigationPane.UserViewsTreeView != null && this.xamNavigationPane.ServersNavigationPane.UserViewsTreeView.Nodes.Count == 0)
            {
                this.xamNavigationPane.ServersNavigationPane.InitializeUserViews();
            }
            searchFlyoutHelper.ShowOrHideSearchFlyout();
        }
        private void CollapsedSearchBtn_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!searchFlyoutHelper.ShowFlyout && !CollapsedSearchHeaderPopup.IsOpen)
            {
                CollapsedSearchHeaderPopup.IsOpen = true;
            }
        }
        private void CollapsedSearchBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CollapsedSearchHeaderPopup.IsOpen)
            {
                CollapsedSearchHeaderPopup.IsOpen = false;
            }
        }
        private void CollapsedMyViewsBtn_Click(object sender, RoutedEventArgs e)
        {
            myViewsFlyoutHelper.ShowFlyout = !myViewsFlyoutHelper.ShowFlyout;
            if (myViewsFlyoutHelper.ShowFlyout && (serversFlyoutHelper.ShowFlyout || tagsFlyoutHelper.ShowFlyout || searchFlyoutHelper.ShowFlyout))
            {
                serversFlyoutHelper.ShowFlyout = false;
                serversFlyoutHelper.ShowOrHideFlyout();
                tagsFlyoutHelper.ShowFlyout = false;
                tagsFlyoutHelper.ShowOrHideFlyout();
                searchFlyoutHelper.ShowFlyout = false;
                searchFlyoutHelper.ShowOrHideSearchFlyout();
            }
            if (myViewsFlyoutHelper.ShowFlyout && this.xamNavigationPane.ServersNavigationPane.UserViewsTreeView != null && this.xamNavigationPane.ServersNavigationPane.UserViewsTreeView.Nodes.Count == 0)
            {
                this.xamNavigationPane.ServersNavigationPane.InitializeUserViews();
            }
            myViewsFlyoutHelper.ShowOrHideFlyout();
        }

        private void CollapsedServersBtn_Click(object sender, RoutedEventArgs e)
        {
            serversFlyoutHelper.ShowFlyout = !serversFlyoutHelper.ShowFlyout;
            if (serversFlyoutHelper.ShowFlyout && (myViewsFlyoutHelper.ShowFlyout || tagsFlyoutHelper.ShowFlyout || searchFlyoutHelper.ShowFlyout))
            {
                myViewsFlyoutHelper.ShowFlyout = false;
                myViewsFlyoutHelper.ShowOrHideFlyout();
                tagsFlyoutHelper.ShowFlyout = false;
                tagsFlyoutHelper.ShowOrHideFlyout();
                searchFlyoutHelper.ShowFlyout = false;
                searchFlyoutHelper.ShowOrHideSearchFlyout();
            }
            if (serversFlyoutHelper.ShowFlyout && this.xamNavigationPane.ServersNavigationPane.UserViewTreeView != null && (this.xamNavigationPane.ServersNavigationPane.UserViewTreeView.Nodes.Count == 0 || this.xamNavigationPane.ServersNavigationPane.UserViewTreeView.Nodes[0].Nodes.Count == 0))
            {
                this.xamNavigationPane.ServersNavigationPane.InitializeUserViews();
            }
            serversFlyoutHelper.ShowOrHideFlyout();
        }

        private void dockMgr_PaneDragEnded(object sender, Infragistics.Windows.DockManager.Events.PaneDragEndedEventArgs e)
        {
            setLeftNavPaneMinWidth();
            myViewsFlyoutHelper.OnPaneDragEnded(_viewModel.LeftNavExpanded);
            serversFlyoutHelper.OnPaneDragEnded(_viewModel.LeftNavExpanded);
            tagsFlyoutHelper.OnPaneDragEnded(_viewModel.LeftNavExpanded);
            searchFlyoutHelper.OnPaneDragEnded(_viewModel.LeftNavExpanded);
        }

        private void CollapsedMyViewsBtn_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(!myViewsFlyoutHelper.ShowFlyout && !CollapsedMyViewsHeaderPopup.IsOpen)
            {
                CollapsedMyViewsHeaderPopup.IsOpen = true;
            }
        }

        private void CollapsedMyViewsBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CollapsedMyViewsHeaderPopup.IsOpen)
            {
                CollapsedMyViewsHeaderPopup.IsOpen = false;
            }
        }

        private void CollapsedHeaderPopup_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var popupSender = sender as Popup;
            if(popupSender != null)
            {
                popupSender.IsOpen = false;
                e.Handled = true;
            }
        }

        private void CollapsedServersBtn_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!serversFlyoutHelper.ShowFlyout && !CollapsedServersHeaderPopup.IsOpen)
            {
                CollapsedServersHeaderPopup.IsOpen = true;
            }
        }

        private void CollapsedServersBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CollapsedServersHeaderPopup.IsOpen)
            {
                CollapsedServersHeaderPopup.IsOpen = false;
            }
        }
        
        private void CollapsedTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            tagsFlyoutHelper.ShowFlyout = !tagsFlyoutHelper.ShowFlyout;
            if (tagsFlyoutHelper.ShowFlyout && (serversFlyoutHelper.ShowFlyout || myViewsFlyoutHelper.ShowFlyout || searchFlyoutHelper.ShowFlyout))
            {
                myViewsFlyoutHelper.ShowFlyout = false;
                serversFlyoutHelper.ShowFlyout = false;
                searchFlyoutHelper.ShowFlyout = false;
                myViewsFlyoutHelper.ShowOrHideFlyout();
                serversFlyoutHelper.ShowOrHideFlyout();
                searchFlyoutHelper.ShowOrHideSearchFlyout();
            }
            if (tagsFlyoutHelper.ShowFlyout && this.xamNavigationPane.ServersNavigationPane.TagsTreeView != null && this.xamNavigationPane.ServersNavigationPane.TagsTreeView.Nodes.Count == 0)
            {
                this.xamNavigationPane.ServersNavigationPane.InitializeUserViews();
                this.xamNavigationPane.ServersNavigationPane.InitializeTagViews();
            }
            tagsFlyoutHelper.ShowOrHideFlyout();
        }

        private void CollapsedTagsBtn_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!tagsFlyoutHelper.ShowFlyout && !CollapsedTagsHeaderPopup.IsOpen)
            {
                CollapsedTagsHeaderPopup.IsOpen = true;
            }
        }

        private void CollapsedTagsBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CollapsedTagsHeaderPopup.IsOpen)
            {
                CollapsedTagsHeaderPopup.IsOpen = false;
            }
        }
        private void SaveLayout(string fullFileName)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));
                }
                using (FileStream fs = new FileStream(fullFileName, FileMode.Create, FileAccess.Write))
                {
                    this.dockMgr.SaveLayout(fs);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        void removeLayout(string fullFileName)
        {
            try
            {
                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                if(_viewModel != null)
                {
                    _viewModel.RemoveLayout(fullFileName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        

        private void MenuSaveView_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            var dialog = new XamSingleInputDialog();
            if (dialog.ShowDialog() == true)
            {
                _viewModel.SelectedViewFileName = dialog.ViewName + ".xml";

                SaveLayout(FullPathToViewConfigurationFile(_viewModel.SelectedViewFileName));

                _viewModel.AddLayoutToList(_viewModel.SelectedViewFileName);
                
            }
                
        }
        private void my_new(object sender, RoutedEventArgs e)
        {
            Log.Debug("hello");
        }
        private void CustomView_Click(object sender, RoutedEventArgs e)
        {
            var item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            var viewFileName = item.Header.ToString() + ".xml";

            _viewModel.SelectedViewFileName = viewFileName;

            LoadCustomViewLayout(viewFileName);

            e.Handled = true;
        }
        private void FilesMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            switch (item.Name)
            {
                case "connectMenu":
                    ApplicationController.Default.ShowRepositoryConnectionDialog();
                    break;
                case "manageServersMenu":
                    ShowManageServersDialog();
                    break;
                case "manageTagsMenu":
                    ShowManageTagsDialog();
                    break;
                case "createViewMenu":
                    ShowAddUserViewDialog();
                    break;
                case "exitMenu":
                    ApplicationController.Default.ExitApplication();
                    break;
            }

            e.Handled = true;
        }

        private void ToolsMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            var win = Window.GetWindow(this);

            switch (item.Name)
            {
                case "toolsMenuAlertConfig":
                    try
                    {
                        using (AlertTemplateDialog alertTemplateDialog = new AlertTemplateDialog())
                        {
                            alertTemplateDialog.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuAlertActions":
                    try
                    {
                        IManagementService managementService =
                            ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                        NotificationRulesDialog notificationRulesDialog = new NotificationRulesDialog(managementService);
                        notificationRulesDialog.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the alert actions from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }

                    break;
                case "toolsMenuVMConfiguration":
                    try
                    {
                        using (VirtualizationConfig virtualizationConfig = new VirtualizationConfig())
                        {
                            virtualizationConfig.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the vm configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuAzureProfileConfiguration":
                    try
                    {
                        using (var azureProfilesConfiguration = new AzureProfilesConfiguration())
                        {
                            azureProfilesConfiguration.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the azure profile configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuGroomingOptions":
                    try
                    {
                        using (GroomingOptionsDialog dialog = new GroomingOptionsDialog())
                        {
                            dialog.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the grooming options from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuConsoleOptions":
                    try
                    {
                        using (ConsoleOptionsDialog dialog = new ConsoleOptionsDialog())
                        {
                            dialog.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the console options from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuSnoozeAlerts":
                    try
                    {
                        ((MainWindowViewModel)this.DataContext).SnoozeAlerts();

                        ((MainWindowViewModel)this.DataContext).PrepareSnoozeAndUnsnoozeOperation();

                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the snooze alerts from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuResumeAlerts":
                    try
                    {
                        ((MainWindowViewModel)this.DataContext).ResumeAlerts();

                        ((MainWindowViewModel)this.DataContext).PrepareSnoozeAndUnsnoozeOperation();

                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the resume alerts from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
                case "toolsMenuMaintenanceMode":
                    try
                    {
                        using (MassMaintenanceModeDialog dialog = new MassMaintenanceModeDialog())
                        {
                            dialog.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError((WinformWindow)win,
                            "Unable to retrieve the maintenance mode from the SQLdm Repository.  Please resolve the following error and try again.",
                            ex);
                    }
                    break;
            }

            e.Handled = true;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            _viewModel.UpdatePredictiveAnalyticsState();

            _viewModel.PrepareSnoozeAndUnsnoozeOperation();


            System.Windows.Controls.MenuItem item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            
            string friendlyName = item.Name.Remove(item.Name.Length - 4, 4);

            char last = friendlyName[friendlyName.Length - 1];

            if (last == 'H')
                friendlyName = friendlyName.Remove(friendlyName.Length - 1, 1);

            XamNavigationPanes itemType = (XamNavigationPanes)Enum.Parse(typeof(XamNavigationPanes), friendlyName, true);

            UpdateNavigationPane(itemType);

            UpdateSearchBarVisibility(itemType);

            xamNavigationPane.ItemSelectetdChanged(itemType);
            

            if (itemType == XamNavigationPanes.News) {
                xamNavigationPane.topMenu.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(xamNavigationPane.TopMenu_SelectionChanged);
                xamNavigationPane.topMenu.SelectedIndex = -1;
                xamNavigationPane.topMenu.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(xamNavigationPane.TopMenu_SelectionChanged);
            }

            e.Handled = true;
        }

        private void menuDeleteView_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ViewDeletionDialog();
            dialog.PopulateViewNameComboBox(_viewModel.CustomViews.OrderBy(x => x).ToList());
            dialog.ShowDialog();
            
            var viewNameToDelete = dialog.SelectedViewName;
            if(viewNameToDelete != null)
            {
                var fileNameToDelete = FullPathToViewConfigurationFile(viewNameToDelete + ".xml");
                this.removeLayout(fileNameToDelete);
            }
        }
        
        private void DockPane_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void mainWindowViewContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ApplicationController.Default.ActiveView is ServerViewContainer)
            {
                var activeView = ApplicationController.Default.ActiveView as ServerViewContainer;
                activeView.SetQuickHistoricalSnapshotVisible(activeView.ActualWidth > 699);
            }
        }
        public void UpdateNavigationPane(XamNavigationPanes itemType)
        {
            if (!navigationPaneHost.Child.Name.Equals("serversNavigationPaneControl") && itemType.Equals(XamNavigationPanes.Servers))
            {
                navigationPaneHeading.Text = "SERVERS";
                navigationPaneHost.Child = serversNavigationPaneControl;
                navigationPaneHost.Child.Name = "serversNavigationPaneControl";
                xamNavigationPane.ServersNavigationPane = serversNavigationPaneControl;
            }
            else if (itemType.Equals(XamNavigationPanes.Alerts))
            {
                navigationPaneHeading.Text = "ALERTS";
                AlertsNavigationPane alertsNavigationPaneControl = new AlertsNavigationPane();
                navigationPaneHost.Child = alertsNavigationPaneControl;
                navigationPaneHost.Child.Name = "alertsNavigationPaneControl";
                xamNavigationPane.AlertsNavigationPane = alertsNavigationPaneControl;
            }
        }

       public void UpdateSearchBarVisibility(XamNavigationPanes itemType)
        {
            if (itemType.Equals(XamNavigationPanes.Servers))
            {
                SearchGrid.Visibility = Visibility.Visible;
            }
            else if(itemType.Equals(XamNavigationPanes.Alerts))
            {
                SearchGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void RestoreDownWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
