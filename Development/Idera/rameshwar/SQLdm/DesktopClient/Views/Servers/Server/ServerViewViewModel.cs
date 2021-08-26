using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infragistics.Windows.Ribbon;

using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using System.Collections.ObjectModel;
using DashboardFilter = Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview.DashboardLayoutGalleryViewModel.DashboardFilter;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis;
using System.Threading;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    internal class ServerViewViewModel : INotifyPropertyChanged
    {
        private int instanceId;
        public event PropertyChangedEventHandler PropertyChanged;
        private const string visible = "Visible";
        private const string collapsed = "Collapsed";

        public ServerViewViewModel()
        {
        }

        public ServerViewViewModel(int instanceId) : this()
        {
            this.instanceId = instanceId;
            ShowHideItems = new ObservableCollection<ShowHideItem>();
            Settings.Default.PropertyChanged += SettingsPropertyChanged;

            PulseController.Default.AuthenticationChanged += PulseControllerAuthenticationChanged;
            PulseController.Default.PulseConnectionChanged += PulseControllerPulseConnectionChanged;
            PulseController.Default.AssetChanged += PulseControllerServerChanged;

            ApplicationController.Default.ActiveViewChanged += ApplicationController_ActiveViewChanged;
            var dateTimePatterns = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetAllDateTimePatterns();
            DayMonthYearFormat = !dateTimePatterns.Any(x => string.Equals(x, "MM/dd/yyyy"));
            QuickHistoricalSnapshotText = DateTimeHelper.GetRealTimeDateQuickHistoricalSnapshots();
            QuickHistoricalSnapshotVisible = true;
            Initialize();
        }

        public void Initialize()
        {
            IsRibbonMinimized = Settings.Default.CollapseRibbon;
            OpenCloseTabsFromSettings();
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CollapseRibbon":
                    IsRibbonMinimized = Settings.Default.CollapseRibbon;
                    break;
            }
        }

        private bool _isRibbonMinimized = false;
        public bool IsRibbonMinimized
        {
            get { return _isRibbonMinimized; }
            set 
            { 
                if (_isRibbonMinimized == value) return;
                _isRibbonMinimized = value;

                if (_isRibbonMinimized != Settings.Default.CollapseRibbon)
                    Settings.Default.CollapseRibbon = _isRibbonMinimized;
                
                FirePropertyChanged("IsRibbonMinimized");
            }
        }

        public ObservableCollection<ShowHideItem> ShowHideItems { get; set; }

        private void ApplicationController_ActiveViewChanged(object sender, EventArgs e)
        {
            IsRefreshOptionsEnabled = !(ApplicationController.Default.ActiveView is Views.Reports.ReportsView);
            FirePropertyChanged("SelectedServerTab");
            FirePropertyChanged("ShowBaselineVisualizerButton");
            FirePropertyChanged("ShowTraceSessionButton");
            FirePropertyChanged("ShowKillSessionButton");
            FirePropertyChanged("ShowBlockingTreeButton");
            FirePropertyChanged("ShowConfigureQueryMonitorButton");
            FirePropertyChanged("ShowClearCacheButton");
            FirePropertyChanged("ShowUpdateStatisticsButton");
            FirePropertyChanged("ShowRebuildIndexesButton");
            FirePropertyChanged("ShowFailOverButton");
            FirePropertyChanged("ShowSuspendResumeButton");
            FirePropertyChanged("ShowConfigureButton");
            FirePropertyChanged("ShowFilterDialogButton");
            FirePropertyChanged("ShowShowHideButton");
            FirePropertyChanged("ShowCustomizeDashboardButton");
            FirePropertyChanged("ShowMaintenanceModeButton");
            updateBasicFilterButtonProperties();
        }

        private bool _isRefreshOptionsEnabled = false;
        public bool IsRefreshOptionsEnabled
        {
            get { return _isRefreshOptionsEnabled; }
            set
            {
                if (_isRefreshOptionsEnabled == value) return;
                _isRefreshOptionsEnabled = value;
                FirePropertyChanged("IsRefreshOptionsEnabled");
            }
        }

        private bool _isForegroundRefreshEnabled = false;
        public bool IsForegroundRefreshEnabled
        {
            get { return _isForegroundRefreshEnabled; }
            set
            {
                if (_isForegroundRefreshEnabled == value) return;
                _isForegroundRefreshEnabled = value;
                FirePropertyChanged("IsForegroundRefreshEnabled");
            }
        }

        #region History browser
        public bool HistoryBrowserBarVisible
        {
            get { return !(SelectedServerTab == ServerViews.OverviewConfiguration || SelectedServerTab == ServerViews.OverviewTimeline || SelectedServerTab == ServerViews.ResourcesFileActivity || SelectedServerTab == ServerViews.ResourcesProcedureCache || SelectedServerTab == ServerViews.DatabasesSummary || SelectedServerTab == ServerViews.Databases || SelectedServerTab == ServerViews.DatabasesConfiguration || SelectedServerTab == ServerViews.DatabasesFiles || SelectedServerTab == ServerViews.DatabasesBackupRestoreHistory || SelectedServerTab == ServerViews.DatabasesTablesIndexes || SelectedServerTab == ServerViews.DatabasesMirroring || SelectedServerTab == ServerViews.ServicesSummary || SelectedServerTab == ServerViews.Services || SelectedServerTab == ServerViews.ServicesSqlAgentJobs || SelectedServerTab == ServerViews.ServicesFullTextSearch || SelectedServerTab == ServerViews.ServicesReplication || SelectedServerTab == ServerViews.Logs); }
        }

        private bool _historyBrowserVisible = false;
        public bool HistoryBrowserVisible
        {
            get { return _historyBrowserVisible;  }
            set 
            {
                if (_historyBrowserVisible == value) return;
                _historyBrowserVisible = value;
                FirePropertyChanged("HistoryBrowserVisible");      
            }
        }

        private bool _historyBrowserEnableNext = false;
        public bool HistoryBrowserEnableNext
        {
            get { return _historyBrowserEnableNext; }
            set
            {
                if (_historyBrowserEnableNext == value) return;
                _historyBrowserEnableNext = value;
                FirePropertyChanged("HistoryBrowserEnableNext");
            }
        }

        private bool _historyBrowserEnablePrevious = true;
        public bool HistoryBrowserEnablePrevious
        {
            get { return _historyBrowserEnablePrevious; }
            set
            {
                if (_historyBrowserEnablePrevious == value) return;
                _historyBrowserEnablePrevious = value;
                FirePropertyChanged("HistoryBrowserEnablePrevious");
            }
        }
        private bool _drillOutButtonVisible = false;
        public bool DrillOutButtonVisible
        {
            get { return _drillOutButtonVisible; }
            set
            {
                if (_drillOutButtonVisible == value) return;
                _drillOutButtonVisible = value;
                FirePropertyChanged("DrillOutButtonVisible");
            }
        }

        #endregion

        #region Maintenance Mode

        public void UpdateMaintenanceMode()
        {
            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
            if (wrapper != null)
            {
                IsInMaintenanceMode = wrapper.MaintenanceModeEnabled;
            }
        }

        private bool _inMaintMode;
        public bool IsInMaintenanceMode
        {
            get { return _inMaintMode; }
            set 
            { 
                if (_inMaintMode == value) return;
                _inMaintMode = value;
                FirePropertyChanged("IsInMaintenanceMode");
            }
        }

        #endregion

        #region Pulse

        private void PulseControllerPulseConnectionChanged(object sender, Idera.Newsfeed.Plugins.Objects.PulseConnectionChangedEventArgs e)
        {
    //        UpdateUserTokenAttributes();
            UpdatePulseGroup();
        }

        private void PulseControllerAuthenticationChanged(object sender, Idera.Newsfeed.Plugins.Objects.AuthenticationChangedEventArgs e)
        {
            UpdatePulseGroup();
        }

        private void PulseControllerServerChanged(object sender, Idera.Newsfeed.Plugins.Objects.AssetChangedEventArgs e)
        {
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                if (e.Name == ApplicationModel.Default.ActiveInstances[instanceId].InstanceName)
                {
                    UpdatePulseGroup();
                }
            }
        }

        public void UpdatePulseGroup()
        {
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                int pulseId;
                if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                {
                    IsPulseEnabled = true;
                    IsPulseLoggedIn = PulseController.Default.IsLoggedIn;
                    IsFollowingServer = PulseController.Default.GetFollowing(pulseId);
                    return;
                }
            }

            IsPulseEnabled = false;
            IsPulseLoggedIn = false;
        }

        private bool _isPulseEnabled = false;
        public bool IsPulseEnabled
        {
            get { return _isPulseEnabled; }
            set
            {
                if (value == _isPulseEnabled) return;
                _isPulseEnabled = value;
                FirePropertyChanged("IsPulseEnabled");
            }
        }

       private bool _isPulseLoggedIn = false;
       public bool IsPulseLoggedIn
        {
            get { return IsPulseEnabled && _isPulseLoggedIn; }
            set 
            {
                if (value == IsPulseLoggedIn) return;
                _isPulseLoggedIn = value;
                FirePropertyChanged("IsPulseLoggedIn");
            }

        }

        private bool _isFollowing = false;
        public bool IsFollowingServer
        {
            get { return _isFollowing; }
            set 
            { 
                if (value == _isFollowing) return;
                _isFollowing = value;

                FirePropertyChanged("IsFollowingServer");
                FirePropertyChanged("PulseFollowServerButtonText");
                FirePropertyChanged("PulseFollowServerImageSource");
            }
        }


        private const string FollowServerText = "Follow Server";
        private const string UnfollowServerText = "Unfollow Server";
        public string PulseFollowServerButtonText
        {
            get { return _isFollowing ? UnfollowServerText : FollowServerText; }
        }

        private ImageSource _followServerImageSource;
        private ImageSource _unfollowServerImageSource;
        public ImageSource PulseFollowServerImageSource
        {
            get
            {
                if (_followServerImageSource == null)
                {
                    _followServerImageSource = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/Follow32.png", UriKind.RelativeOrAbsolute));
                    _unfollowServerImageSource = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/Unfollow32.png", UriKind.RelativeOrAbsolute));
                }

                return _isFollowing ? _unfollowServerImageSource : _followServerImageSource;
            }
        }

        #endregion

        #region Tab Add Button Binding Properties
                
        public bool CanAddOverviewTab
        {
            get
            {
                var canAdd = VisibilityOverviewDashboard == "Visible" 
                    && VisibilityOverviewDetails == "Visible"
                    && VisibilityOverviewConfiguration == "Visible"
                    && VisibilityOverviewActiveAlerts == "Visible"
                    && VisibilityOverviewTimeline == "Visible";

                return !canAdd;
            }
        }

        public bool CanAddSessionsTab
        {
            get
            {
                var canAdd = VisibilitySessionsSummary == "Visible"
                    && VisibilitySessionsDetails == "Visible"
                    && VisibilitySessionsLocks == "Visible"
                    && VisibilitySessionsBlocking == "Visible";

                return !canAdd;
            }
        }

        public bool CanAddQueriesTab
        {
            get
            {
                var canAdd = VisibilityQueriesSignatureMode == "Visible"
                    && VisibilityQueriesStatementMode == "Visible"
                    && VisibilityQueriesQueryHistory == "Visible"
                    && VisibilityQueriesQueryWaits == "Visible";

                return !canAdd;
            }
        }

        public bool CanAddResourcesTab
        {
            get
            {
                var canAdd = VisibilityResourcesSummary == "Visible"
                    && VisibilityResourcesCPU == "Visible"
                    && VisibilityResourcesMemory == "Visible"
                    && VisibilityResourcesDisk == "Visible"
                    && VisibilityResourcesDiskSize == "Visible"
                    && VisibilityResourcesFileActivity == "Visible"
                    && VisibilityResourcesProcedureCache == "Visible"
                    && VisibilityResourcesServerWaits == "Visible";

                return !canAdd;
            }
        }
        
        public bool CanAddDatabaseTab
        {
            get
            {
                var canAdd = VisibilityDatabaseSummary == "Visible"
                    && VisibilityDatabaseAvailabilityGroup == "Visible"
                    && VisibilityDatabaseTempdbSummary == "Visible"
                    && VisibilityDatabaseConfiguration == "Visible"
                    && VisibilityDatabaseFiles == "Visible"
                    && VisibilityDatabaseBackupRestore == "Visible"
                    && VisibilityDatabaseTablesIndexes == "Visible"
                    && VisibilityDatabaseMirroring == "Visible";

                return !canAdd;
            }
        }

        public bool CanAddServicesTab
        {
            get
            {
                var canAdd = VisibilityServicesSummary == "Visible"
                    && VisibilityServicesSQLAgentJobs == "Visible"
                    && VisibilityServicesFullTextSearch == "Visible"
                    && VisibilityServicesReplication == "Visible";

                return !canAdd;
            }
        }

        #endregion

        #region Tab Status

        internal void UpdateStatus(MonitoredSqlServerStatus status)
        {
            OverviewState = status != null ? status.ServerImageKey : "ServerInformation";
            SessionsState = GetStatus(status, MetricCategory.Sessions);
            QueriesState = GetStatus(status, MetricCategory.Queries);
            ResourcesState = GetStatus(status, MetricCategory.Resources);
            DatabasesState = GetStatus(status, MetricCategory.Databases);
            ServicesState = GetStatus(status, MetricCategory.Services);
            LogsState = GetStatus(status, MetricCategory.Logs);
            AnalysisState = GetStatus(status, MetricCategory.Analyze);
            LaunchSWAState = GetStatus(status, MetricCategory.LaunchSWA);
            ShowHideMenuItems();
            
        }

        
        #region 10.6 tab visibility
        //10.6 visible states for closing/opening tabs
        private string _VisibilityOverviewDashboard = visible;
        public string VisibilityOverviewDashboard
        {
            get { return _VisibilityOverviewDashboard; }
            set
            {
                if (_VisibilityOverviewDashboard == value) return;
                _VisibilityOverviewDashboard = value;
                FirePropertyChanged("VisibilityOverviewDashboard");
                FirePropertyChanged("CanAddOverviewTab");
            }
        }

        private string _VisibilityOverviewDetails = visible;
        public string VisibilityOverviewDetails
        {
            get { return _VisibilityOverviewDetails; }
            set
            {
                if (_VisibilityOverviewDetails == value) return;
                _VisibilityOverviewDetails = value;
                FirePropertyChanged("VisibilityOverviewDetails");
                FirePropertyChanged("CanAddOverviewTab");
            }
        }

        private string _VisibilityOverviewConfiguration = visible;
        public string VisibilityOverviewConfiguration
        {
            get
            {
                if (_VisibilityOverviewConfiguration == "Visible" && ResourceConfigurationVisibility == "Visible")
                    return "Visible";
                else
                    return "Collapsed";
            }
            set
            {
                if (_VisibilityOverviewConfiguration == value) return;
                _VisibilityOverviewConfiguration = value;
                FirePropertyChanged("VisibilityOverviewConfiguration");
                FirePropertyChanged("CanAddOverviewTab");
            }
        }

        private string _VisibilityOverviewActiveAlerts = visible;
        public string VisibilityOverviewActiveAlerts
        {
            get { return _VisibilityOverviewActiveAlerts; }
            set
            {
                if (_VisibilityOverviewActiveAlerts == value) return;
                _VisibilityOverviewActiveAlerts = value;
                FirePropertyChanged("VisibilityOverviewActiveAlerts");
                FirePropertyChanged("CanAddOverviewTab");
            }
        }
        private string _VisibilityOverviewTimeline = visible;
        public string VisibilityOverviewTimeline
        {
            get { return _VisibilityOverviewTimeline; }
            set
            {
                if (_VisibilityOverviewTimeline == value) return;
                _VisibilityOverviewTimeline = value;
                FirePropertyChanged("VisibilityOverviewTimeline");
                FirePropertyChanged("CanAddOverviewTab");
            }
        }

        private string _VisibilitySessionsSummary = visible;
        public string VisibilitySessionsSummary
        {
            get { return _VisibilitySessionsSummary; }
            set
            {
                if (_VisibilitySessionsSummary == value) return;
                _VisibilitySessionsSummary = value;
                FirePropertyChanged("VisibilitySessionsSummary");
                FirePropertyChanged("CanAddSessionsTab");
            }
        }

        private string _VisibilitySessionsDetails = visible;
        public string VisibilitySessionsDetails
        {
            get { return _VisibilitySessionsDetails; }
            set
            {
                if (_VisibilitySessionsDetails == value) return;
                _VisibilitySessionsDetails = value;
                FirePropertyChanged("VisibilitySessionsDetails");
                FirePropertyChanged("CanAddSessionsTab");
            }
        }

        private string _VisibilitySessionsLocks = visible;
        public string VisibilitySessionsLocks
        {
            get
            {
                if (_VisibilitySessionsLocks == visible && VisibilityLocks == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilitySessionsLocks == value) return;
                _VisibilitySessionsLocks = value;
                FirePropertyChanged("VisibilitySessionsLocks");
                FirePropertyChanged("CanAddSessionsTab");
            }
        }
        private string _VisibilitySessionsBlocking = visible;
        public string VisibilitySessionsBlocking
        {
            get
            {
                if (_VisibilitySessionsBlocking == "Visible" && VisibilityBlocking == "Visible")
                    return "Visible";
                else
                    return "Collapsed";
            }
            set
            {
                if (_VisibilitySessionsBlocking == value) return;
                _VisibilitySessionsBlocking = value;
                FirePropertyChanged("VisibilitySessionsBlocking");
                FirePropertyChanged("CanAddSessionsTab");
            }
        }
        private string _VisibilityQueriesSignatureMode = visible;
        public string VisibilityQueriesSignatureMode
        {
            get { return _VisibilityQueriesSignatureMode; }
            set
            {
                if (_VisibilityQueriesSignatureMode == value) return;
                _VisibilityQueriesSignatureMode = value;
                FirePropertyChanged("VisibilityQueriesSignatureMode");
                FirePropertyChanged("CanAddQueriesTab"); 
            }
        }

        private string _VisibilityQueriesStatementMode = visible;
        public string VisibilityQueriesStatementMode
        {
            get { return _VisibilityQueriesStatementMode; }
            set
            {
                if (_VisibilityQueriesStatementMode == value) return;
                _VisibilityQueriesStatementMode = value;
                FirePropertyChanged("VisibilityQueriesStatementMode");
                FirePropertyChanged("CanAddQueriesTab");
            }
        }
        private string _VisibilityQueriesQueryHistory = visible;
        public string VisibilityQueriesQueryHistory
        {
            get { return _VisibilityQueriesQueryHistory; }
            set
            {
                if (_VisibilityQueriesQueryHistory == value) return;
                _VisibilityQueriesQueryHistory = value;
                FirePropertyChanged("VisibilityQueriesQueryHistory");
                FirePropertyChanged("CanAddQueriesTab");
            }
        }
        private string _VisibilityQueriesQueryWaits = visible;
        public string VisibilityQueriesQueryWaits
        {
            get { return _VisibilityQueriesQueryWaits; }
            set
            {
                if (_VisibilityQueriesQueryWaits == value) return;
                _VisibilityQueriesQueryWaits = value;
                FirePropertyChanged("VisibilityQueriesQueryWaits");
                FirePropertyChanged("CanAddQueriesTab");
            }
        }
        private string _VisibilityResourcesSummary = visible;
        public string VisibilityResourcesSummary
        {
            get { return _VisibilityResourcesSummary; }
            set
            {
                if (_VisibilityResourcesSummary == value) return;
                _VisibilityResourcesSummary = value;
                FirePropertyChanged("VisibilityResourcesSummary");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesCPU = visible;
        public string VisibilityResourcesCPU
        {
            get
            {
                if (_VisibilityResourcesCPU == visible && VisibilityCPU == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesCPU == value) return;
                _VisibilityResourcesCPU = value;
                FirePropertyChanged("VisibilityResourcesCPU");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesMemory = visible;
        public string VisibilityResourcesMemory
        {
            get
            {
                if (_VisibilityResourcesMemory == visible && VisibilityMemory == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesMemory == value) return;
                _VisibilityResourcesMemory = value;
                FirePropertyChanged("VisibilityResourcesMemory");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesDisk = visible;
        public string VisibilityResourcesDisk
        {
            get
            {
                if (_VisibilityResourcesDisk == visible && VisibilityDisk == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesDisk == value) return;
                _VisibilityResourcesDisk = value;
                FirePropertyChanged("VisibilityResourcesDisk");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesDiskSize = visible;
        public string VisibilityResourcesDiskSize
        {
            get
            {
                if (_VisibilityResourcesDiskSize == visible && VisibilityDiskSize == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesDiskSize == value) return;
                _VisibilityResourcesDiskSize = value;
                FirePropertyChanged("VisibilityResourcesDiskSize");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesFileActivity = visible;
        public string VisibilityResourcesFileActivity
        {
            get
            {
                if (_VisibilityResourcesFileActivity == visible && ResourceTabFileActivityVisibility == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesFileActivity == value) return;
                _VisibilityResourcesFileActivity = value;
                FirePropertyChanged("VisibilityResourcesFileActivity");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesProcedureCache = visible;
        public string VisibilityResourcesProcedureCache
        {
            get
            {
                if (_VisibilityResourcesProcedureCache == visible && VisibilityProcedurecache == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesProcedureCache == value) return;
                _VisibilityResourcesProcedureCache = value;
                FirePropertyChanged("VisibilityResourcesProcedureCache");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityResourcesServerWaits = visible;
        public string VisibilityResourcesServerWaits
        {
            get
            {
                if (_VisibilityResourcesServerWaits == visible && VisibilityServerwaits == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityResourcesServerWaits == value) return;
                _VisibilityResourcesServerWaits = value;
                FirePropertyChanged("VisibilityResourcesServerWaits");
                FirePropertyChanged("CanAddResourcesTab");
            }
        }
        private string _VisibilityDatabaseSummary = visible;
        public string VisibilityDatabaseSummary
        {
            get
            {
                if (_VisibilityDatabaseSummary == visible && VisibilitySummary == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseSummary == value) return;
                _VisibilityDatabaseSummary = value;
                FirePropertyChanged("VisibilityDatabaseSummary");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseAvailabilityGroup = visible;
        public string VisibilityDatabaseAvailabilityGroup
        {
            get
            {
                if (_VisibilityDatabaseAvailabilityGroup == visible && VisibilityAvailabilityGroup == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseAvailabilityGroup == value) return;
                _VisibilityDatabaseAvailabilityGroup = value;
                FirePropertyChanged("VisibilityDatabaseAvailabilityGroup");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseTempdbSummary = visible;
        public string VisibilityDatabaseTempdbSummary
        {
            get
            {
                if (_VisibilityDatabaseTempdbSummary == visible && VisibilityTempdbSummary == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseTempdbSummary == value) return;
                _VisibilityDatabaseTempdbSummary = value;
                FirePropertyChanged("VisibilityDatabaseTempdbSummary");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseConfiguration = visible;
        public string VisibilityDatabaseConfiguration
        {
            get { return _VisibilityDatabaseConfiguration; }
            set
            {
                if (_VisibilityDatabaseConfiguration == value) return;
                _VisibilityDatabaseConfiguration = value;
                FirePropertyChanged("VisibilityDatabaseConfiguration");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseFiles = visible;
        public string VisibilityDatabaseFiles
        {
            get
            {
                if (_VisibilityDatabaseFiles == visible && VisibilityFiles == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseFiles == value) return;
                _VisibilityDatabaseFiles = value;
                FirePropertyChanged("VisibilityDatabaseFiles");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseBackupRestore = visible;
        public string VisibilityDatabaseBackupRestore
        {
            get
            {
                if (_VisibilityDatabaseBackupRestore == visible && VisibilityBackupsRestores == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseBackupRestore == value) return;
                _VisibilityDatabaseBackupRestore = value;
                FirePropertyChanged("VisibilityDatabaseBackupRestore");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseTablesIndexes = visible;
        public string VisibilityDatabaseTablesIndexes
        {
            get
            {
                if (_VisibilityDatabaseTablesIndexes == visible && VisibilityTablesIndexes == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseTablesIndexes == value) return;
                _VisibilityDatabaseTablesIndexes = value;
                FirePropertyChanged("VisibilityDatabaseTablesIndexes");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityDatabaseMirroring = visible;
        public string VisibilityDatabaseMirroring
        {
            get
            {
                if (_VisibilityDatabaseMirroring == visible && VisibilityMirroring == visible)
                    return visible;
                else
                    return collapsed;
            }
            set
            {
                if (_VisibilityDatabaseMirroring == value) return;
                _VisibilityDatabaseMirroring = value;
                FirePropertyChanged("VisibilityDatabaseMirroring");
                FirePropertyChanged("CanAddDatabaseTab");
            }
        }
        private string _VisibilityServicesSummary = visible;
        public string VisibilityServicesSummary
        {
            get { return _VisibilityServicesSummary; }
            set
            {
                if (_VisibilityServicesSummary == value) return;
                _VisibilityServicesSummary = value;
                FirePropertyChanged("VisibilityServicesSummary");
                FirePropertyChanged("CanAddServicesTab");
            }
        }
        private string _VisibilityServicesSQLAgentJobs = visible;
        public string VisibilityServicesSQLAgentJobs
        {
            get { return _VisibilityServicesSQLAgentJobs; }
            set
            {
                if (_VisibilityServicesSQLAgentJobs == value) return;
                _VisibilityServicesSQLAgentJobs = value;
                FirePropertyChanged("VisibilityServicesSQLAgentJobs");
                FirePropertyChanged("CanAddServicesTab");
            }
        }
        private string _VisibilityServicesFullTextSearch = visible;
        public string VisibilityServicesFullTextSearch
        {
            get { return _VisibilityServicesFullTextSearch; }
            set
            {
                if (_VisibilityServicesFullTextSearch == value) return;
                _VisibilityServicesFullTextSearch = value;
                FirePropertyChanged("VisibilityServicesFullTextSearch");
                FirePropertyChanged("CanAddServicesTab");
            }
        }
        private string _VisibilityServicesReplication = visible;
        public string VisibilityServicesReplication
        {
            get { return _VisibilityServicesReplication; }
            set
            {
                if (_VisibilityServicesReplication == value) return;
                _VisibilityServicesReplication = value;
                FirePropertyChanged("VisibilityServicesReplication");
                FirePropertyChanged("CanAddServicesTab");
            }
        }
        #endregion

        //Properties added to hide/show ribbons depend on instance type selection.
        private string _VisibilityServerwaits = string.Empty;
        public string VisibilityServerwaits
        {
            get { return _VisibilityServerwaits; }
            set
            {
                if (_VisibilityServerwaits == value) return;
                _VisibilityServerwaits = value;
                FirePropertyChanged("VisibilityServerwaits");
                FirePropertyChanged("VisibilityResourcesServerWaits");
            }
        }
        private string _VisibilityTablesIndexes = string.Empty;
        public string VisibilityTablesIndexes
        {
            get { return _VisibilityTablesIndexes; }
            set
            {
                if (_VisibilityTablesIndexes == value) return;
                _VisibilityTablesIndexes = value;
                FirePropertyChanged("VisibilityTablesIndexes");
                FirePropertyChanged("VisibilityDatabaseTablesIndexes");
            }
        }

        private string _VisibilityProcedurecache = string.Empty;
        public string VisibilityProcedurecache
        {
            get { return _VisibilityProcedurecache; }
            set
            {
                if (_VisibilityProcedurecache == value) return;
                _VisibilityProcedurecache = value;
                FirePropertyChanged("VisibilityProcedurecache");
                FirePropertyChanged("VisibilityResourcesProcedureCache");
            }
        }
        private string _VisibilityDisk = string.Empty;
        public string VisibilityDisk
        {
            get { return _VisibilityDisk; }
            set
            {
                if (_VisibilityDisk == value) return;
                _VisibilityDisk = value;
                FirePropertyChanged("VisibilityDisk");
                FirePropertyChanged("VisibilityResourcesDisk");
            }
        }
        private string _VisibilityMemory = string.Empty;
        public string VisibilityMemory
        {
            get { return _VisibilityMemory; }
            set
            {
                if (_VisibilityMemory == value) return;
                _VisibilityMemory = value;
                FirePropertyChanged("VisibilityMemory");
                FirePropertyChanged("VisibilityResourcesMemory");
            }
        }
        private string _VisibilityDiskSize = string.Empty;
        public string VisibilityDiskSize
        {
            get { return _VisibilityDiskSize; }
            set
            {
                if (_VisibilityDiskSize == value) return;
                _VisibilityDiskSize = value;
                FirePropertyChanged("VisibilityDiskSize");
                FirePropertyChanged("VisibilityResourcesDiskSize");
            }
        }
        
        private string _VisibilitySummary = string.Empty;
        public string VisibilitySummary
        {
            get { return _VisibilitySummary; }
            set
            {
                if (_VisibilitySummary == value) return;
                _VisibilitySummary = value;
                FirePropertyChanged("VisibilitySummary");
                FirePropertyChanged("VisibilityDatabaseSummary");
            }
        }
        private string _VisibilityMirroring = string.Empty;
        public string VisibilityMirroring
        {
            get { return _VisibilityMirroring; }
            set
            {
                if (_VisibilityMirroring == value) return;
                _VisibilityMirroring = value;
                FirePropertyChanged("VisibilityMirroring");
                FirePropertyChanged("VisibilityDatabaseMirroring");
            }
        }
        private string _VisibilityBackupsRestores = string.Empty;
        public string VisibilityBackupsRestores
        {
            get { return _VisibilityBackupsRestores; }
            set
            {
                if (_VisibilityBackupsRestores == value) return;
                _VisibilityBackupsRestores = value;
                FirePropertyChanged("VisibilityBackupsRestores");
                FirePropertyChanged("VisibilityDatabaseBackupRestore");
            }
        }
        private string _VisibilityFiles = string.Empty;
        public string VisibilityFiles
        {
            get { return _VisibilityFiles; }
            set
            {
                if (_VisibilityFiles == value) return;
                _VisibilityFiles = value;
                FirePropertyChanged("VisibilityFiles");
                FirePropertyChanged("VisibilityDatabaseFiles");
            }
        }

        private string _VisibilityTempdbSummary = string.Empty;
        public string VisibilityTempdbSummary
        {
            get { return _VisibilityTempdbSummary; }
            set
            {
                if (_VisibilityTempdbSummary == value) return;
                _VisibilityTempdbSummary = value;
                FirePropertyChanged("VisibilityTempdbSummary");
                FirePropertyChanged("VisibilityDatabaseTempdbSummary");
            }
        }


        private string _VisibilityAvailabilityGroup = string.Empty;
        public string VisibilityAvailabilityGroup
        {
            get { return _VisibilityAvailabilityGroup; }
            set
            {
                if (_VisibilityAvailabilityGroup == value) return;
                _VisibilityAvailabilityGroup = value;
                FirePropertyChanged("VisibilityAvailabilityGroup");
                FirePropertyChanged("VisibilityDatabaseAvailabilityGroup");
            }
        }
        private string _visibilityDashboard = string.Empty;
        public string VisibilityDashboard
        {
            get { return _visibilityDashboard; }
            set
            {
                if (_visibilityDashboard == value) return;
                _visibilityDashboard = value;
                FirePropertyChanged("VisibilityDashboard");
            }
        }
        private string _VisibilityLocks = string.Empty;
        public string VisibilityLocks
        {
            get { return _VisibilityLocks; }
            set
            {
                if (_VisibilityLocks == value) return;
                _VisibilityLocks = value;
                FirePropertyChanged("VisibilityLocks");
                FirePropertyChanged("VisibilitySessionsLocks");
            }
        }

        private string _VisibilityBlocking = string.Empty;
        public string VisibilityBlocking
        {
            get { return _VisibilityBlocking; }
            set
            {
                if (_VisibilityBlocking == value) return;
                _VisibilityBlocking = value;
                FirePropertyChanged("VisibilityBlocking");
                FirePropertyChanged("VisibilitySessionsBlocking");
            }
        }

        // 10.6 copied this to the mainwindow viewmodel for new navigation access 
        private string _VisibilityQueries = string.Empty;
        public string VisibilityQueries
        {
            get { return _VisibilityQueries; }
            set
            {
                if (_VisibilityQueries == value) return;
                _VisibilityQueries = value;
                FirePropertyChanged("VisibilityQueries");
            }
        }
        private string _visibilityfileactivityresource = string.Empty;
        public string VisibilityFileActivityResource
        {
            get { return _visibilityfileactivityresource; }
            set
            {
                if (_visibilityfileactivityresource == value) return;
                _visibilityfileactivityresource = value;
                FirePropertyChanged("VisibilityFileActivityResource");
            }
        }


       
        private string _servicestabitemvisibility = string.Empty;
        public string ServicesTabItemVisibility
        {
            get { return _servicestabitemvisibility; }
            set
            {
                if (_servicestabitemvisibility == value) return;
                _servicestabitemvisibility = value;
                FirePropertyChanged("ServicesTabItemVisibility");
            }
        }

        // 10.6 copied this to the mainwindow viewmodel for new navigation access 
        private string _servicestabitemvisibilityforazuredb = string.Empty;
        public string ServicesTabItemVisibilityforAzureDb
        {
            get { return _servicestabitemvisibilityforazuredb; }
            set
            {
                if (_servicestabitemvisibilityforazuredb == value) return;
                _servicestabitemvisibilityforazuredb = value;
                FirePropertyChanged("ServicesTabItemVisibilityforAzureDb");
            }
        }

        // 10.6 copied this to the mainwindow viewmodel for new navigation access 
        private string _logstabitemvisibility = string.Empty;
        public string LogTabItemVisibility
        {
            get { return _logstabitemvisibility; }
            set
            {
                if (_logstabitemvisibility == value) return;
                _logstabitemvisibility = value;
                FirePropertyChanged("LogTabItemVisibility");
            }
        }

        // 10.6 copied this to the mainwindow viewmodel for new navigation access 
        private string _logstabitemvisibility1 = string.Empty;
        public string LogTabItemVisibility1
        {
            get { return _logstabitemvisibility1; }
            set
            {
                if (_logstabitemvisibility1 == value) return;
                _logstabitemvisibility1 = value;
                FirePropertyChanged("LogTabItemVisibility1");
            }
        }

        private string _resourcetabdisksizevisibility = string.Empty;
        public string ResourceTabDiskSizeVisibility
        {
            get { return _resourcetabdisksizevisibility; }
            set
            {
                if (_resourcetabdisksizevisibility == value) return;
                _resourcetabdisksizevisibility = value;
                FirePropertyChanged("ResourceTabDiskSizeVisibility");
            }
        }

        private string _resourcetabfileactivityvisibility = string.Empty;
        public string ResourceTabFileActivityVisibility
        {
            get { return _resourcetabfileactivityvisibility; }
            set
            {
                if (_resourcetabfileactivityvisibility == value) return;
                _resourcetabfileactivityvisibility = value;
                FirePropertyChanged("ResourceTabFileActivityVisibility");
                FirePropertyChanged("VisibilityResourcesFileActivity");
            }
        }

        private string _VisibilityCPU = string.Empty;
        public string VisibilityCPU
        {
            get { return _VisibilityCPU; }
            set
            {
                if (_VisibilityCPU == value) return;
                _VisibilityCPU = value;
                FirePropertyChanged("VisibilitySummary");
                FirePropertyChanged("VisibilityResourcesCPU");
            }
        }
        //End

        private MonitoredState GetStatus(MonitoredSqlServerStatus status, MetricCategory metricCategory)
        {
            if (status == null) return MonitoredState.OK;

            ICollection<Issue> issues = null;
            if (status != null)
                issues = status[metricCategory];

            var severity = MonitoredState.OK;
            if (issues != null && issues.Count > 0)
            {
                var e_issues = issues.GetEnumerator();
                if (e_issues.MoveNext())
                    severity = e_issues.Current.Severity;
            }

            return severity;
        }

        private string _overviewState = MonitoredState.OK.ToString();
        public string OverviewState
        {
            get { return _overviewState; }
            set
            {
                if (_overviewState == value) return;
                _overviewState = value;
                FirePropertyChanged("OverviewState");
            }
        }

        private MonitoredState _sessionState = MonitoredState.OK;
        public MonitoredState SessionsState
        {
            get { return _sessionState; }
            set
            {
                if (_sessionState == value) return;
                _sessionState = value;
                FirePropertyChanged("SessionsState");
            }
        }
        private MonitoredState _queriesState = MonitoredState.OK;
        public MonitoredState QueriesState
        {
            get { return _queriesState; }
            set
            {
                if (_queriesState == value) return;
                _queriesState = value;
                FirePropertyChanged("QueriesState");
            }
        }
        private MonitoredState _resourcesState = MonitoredState.OK;
        public MonitoredState ResourcesState
        {
            get { return _resourcesState; }
            set
            {
                if (_resourcesState == value) return;
                _resourcesState = value;
                FirePropertyChanged("ResourcesState");
            }
        }
        private MonitoredState _databasesState = MonitoredState.OK;
        public MonitoredState DatabasesState
        {
            get { return _databasesState; }
            set
            {
                if (_databasesState == value) return;
                _databasesState = value;
                FirePropertyChanged("DatabasesState");
            }
        }
        private MonitoredState _servicesState = MonitoredState.OK;
        public MonitoredState ServicesState
        {
            get { return _servicesState; }
            set
            {
                if (_servicesState == value) return;
                _servicesState = value;
                FirePropertyChanged("ServicesState");
            }
        }
        private MonitoredState _logsState = MonitoredState.OK;
        public MonitoredState LogsState
        {
            get { return _logsState; }
            set
            {
                if (_logsState == value) return;
                _logsState = value;
                FirePropertyChanged("LogsState");
            }
        }
        private MonitoredState _analysisState = MonitoredState.OK;
        public MonitoredState AnalysisState
        {
            get { return _analysisState; }
            set
            {
                if (_analysisState == value) return;
                _analysisState = value;
                FirePropertyChanged("AnalysisState");
            }
        }
        private MonitoredState _launchSWAState = MonitoredState.OK;
        public MonitoredState LaunchSWAState
        {
            get { return _launchSWAState; }
            set
            {
                if (_launchSWAState == value) return;
                _launchSWAState = value;
                FirePropertyChanged("LaunchSWAState");
            }
        }

        #endregion

        #region 10.6 New Action Buttons
        public void UpdateBindingProperties()
        {
            FirePropertyChanged("SelectedServerTab");
            FirePropertyChanged("ShowBaselineVisualizerButton");
            FirePropertyChanged("ShowTraceSessionButton");
            FirePropertyChanged("ShowKillSessionButton");
            FirePropertyChanged("ShowBlockingTreeButton");
            FirePropertyChanged("ShowConfigureQueryMonitorButton");
            FirePropertyChanged("ShowClearCacheButton");
            FirePropertyChanged("ShowUpdateStatisticsButton");
            FirePropertyChanged("ShowRebuildIndexesButton");
            FirePropertyChanged("ShowFailOverButton");
            FirePropertyChanged("ShowSuspendResumeButton");
            FirePropertyChanged("ShowConfigureButton");
            FirePropertyChanged("ShowFilterDialogButton");
            FirePropertyChanged("ShowShowHideButton");
            FirePropertyChanged("ShowCustomizeDashboardButton");
            FirePropertyChanged("ShowMaintenanceModeButton");
            FirePropertyChanged("SelectedSubTab");
            FirePropertyChanged("ShowBaselineVisualizerButton");
            FirePropertyChanged("ShowTraceSessionButton");
            FirePropertyChanged("ShowKillSessionButton");
            FirePropertyChanged("ShowBlockingTreeButton");
            FirePropertyChanged("ShowConfigureQueryMonitorButton");
            FirePropertyChanged("ShowClearCacheButton");
            FirePropertyChanged("ShowUpdateStatisticsButton");
            FirePropertyChanged("ShowRebuildIndexesButton");
            FirePropertyChanged("ShowFailOverButton");
            FirePropertyChanged("ShowSuspendResumeButton");
            FirePropertyChanged("ShowQueriesConfigureButton");
            FirePropertyChanged("ShowFilterDialogButton");
            FirePropertyChanged("ShowShowHideButton");
            FirePropertyChanged("ShowCustomizeDashboardButton");
            FirePropertyChanged("ShowMaintenanceModeButton");
            FirePropertyChanged("ShowStartButton");
            FirePropertyChanged("ShowStopButton");
            FirePropertyChanged("ShowStartServiceButton");
            FirePropertyChanged("ShowStopServiceButton");
            FirePropertyChanged("ShowStartJobButton");
            FirePropertyChanged("ShowStopJobButton");
            FirePropertyChanged("ShowCycleLogsButton");
            FirePropertyChanged("ShowLogsConfigureButton");
            FirePropertyChanged("ShowAnalyzeActionButtons");
            FirePropertyChanged("HistoryBrowserBarVisible");
            FirePropertyChanged("QuickHistoricalSnapshotVisible");
            updateBasicFilterButtonProperties();
        }

        private void updateBasicFilterButtonProperties()
        {
            FirePropertyChanged("ShowBasicFilterButton");
            FirePropertyChanged("ShowBasicFilter_OvDetailsButton");
            FirePropertyChanged("ShowBasicFilter_OvTimelineButton");
            FirePropertyChanged("ShowBasicFilter_DbIndexButton");
        }

        private ServerViews selectedServerTab = ServerViews.Overview;
        public ServerViews SelectedServerTab
        {
            get { return selectedServerTab; }
            set
            {
                if (value != selectedServerTab)
                    selectedServerTab = value;

                UpdateBindingProperties();
            }
        }
        private ServerViewTabs selectedSubTab = ServerViewTabs.Dashboard;
        public ServerViewTabs SelectedSubTab
        {
            get { return selectedSubTab; }
            set
            {
                if (value == selectedSubTab) return;
                selectedSubTab = value;

                UpdateBindingProperties();
            }
        }
        public void ResetShowHideItems(IEnumerable<ShowHideItem> items = null)
        {
            ShowHideItems.Clear();
            if(items != null)
            {
                foreach(var item in items)
                {
                    ShowHideItems.Add(item);
                }
            }
        }
        public bool ShowBaselineVisualizerButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Overview)
                    return true;
                else
                    return false;
            }
        }
        public bool ShowTraceSessionButton
        {
            get {
                if (SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab != ServerViews.SessionsSummary)
                    return true;
                else
                    return false;

            }
        }

        private bool enableTraceSessionsButton;
        public bool EnableTraceSessionButton
        {
            get { return enableTraceSessionsButton; }
            set
            {
                enableTraceSessionsButton = value;
                FirePropertyChanged("EnableTraceSessionButton");
            }
        }
        
        public bool ShowKillSessionButton
        {
            get {
                if (SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab != ServerViews.SessionsSummary)
                    return true;
                else
                    return false;

            }
        }

        private bool enableAnalyzeRunButton;
        public bool EnableAnalyzeRunButton
        {
            get { return enableAnalyzeRunButton; }
            set
            {
                enableAnalyzeRunButton = value;
                FirePropertyChanged("EnableAnalyzeRunButton");
            }
        }

        private bool enableAnalyzeUndoButton;
        public bool EnableAnalyzeUndoButton
        {
            get { return enableAnalyzeUndoButton; }
            set
            {
                enableAnalyzeUndoButton = value;
                FirePropertyChanged("EnableAnalyzeUndoButton");
            }
        }

        private bool enableAnalyzeBlockButton;
        public bool EnableAnalyzeBlockButton
        {
            get { return enableAnalyzeBlockButton; }
            set
            {
                enableAnalyzeBlockButton = value;
                FirePropertyChanged("EnableAnalyzeBlockButton");
            }
        }

        private bool enableAnalyzeExportButton;
        public bool EnableAnalyzeExportButton
        {
            get { return enableAnalyzeExportButton; }
            set
            {
                enableAnalyzeExportButton = value;
                FirePropertyChanged("EnableAnalyzeExportButton");
            }
        }

        private bool enableAnalyzeCopyButton;
        public bool EnableAnalyzeCopyButton
        {
            get { return enableAnalyzeCopyButton; }
            set
            {
                enableAnalyzeCopyButton = value;
                FirePropertyChanged("EnableAnalyzeCopyButton");
            }
        }

        private bool enableAnalyzeEmailButton;
        public bool EnableAnalyzeEmailButton
        {
            get { return enableAnalyzeEmailButton; }
            set
            {
                enableAnalyzeEmailButton = value;
                FirePropertyChanged("EnableAnalyzeEmailButton");
            }
        }

        private bool enableAnalyzeShowProblemButton;
        public bool EnableAnalyzeShowProblemButton
        {
            get { return enableAnalyzeShowProblemButton; }
            set
            {
                enableAnalyzeShowProblemButton = value;
                FirePropertyChanged("EnableAnalyzeShowProblemButton");
            }
        }

        private bool enableAnalyzeUndoScriptButton;
        public bool EnableAnalyzeUndoScriptButton
        {
            get { return enableAnalyzeUndoScriptButton; }
            set
            {
                enableAnalyzeUndoScriptButton = value;
                FirePropertyChanged("EnableAnalyzeUndoScriptButton");
            }
        }

		//SQLDM-31035
        private bool enableAnalyzeBackButton;
        public bool EnableAnalyzeBackButton
        {
            get { return enableAnalyzeBackButton; }
            set
            {
                enableAnalyzeBackButton = value;
                FirePropertyChanged("EnableAnalyzeBackButton");
            }
        }  

		private bool enableAnalyzeOptimizeScriptButton;
        public bool EnableAnalyzeOptimizeScriptButton
        {
            get { return enableAnalyzeOptimizeScriptButton; }
            set
            {
                enableAnalyzeOptimizeScriptButton = value;
                FirePropertyChanged("EnableAnalyzeOptimizeScriptButton");
            }
        }

        private bool enableKillSessionsButton;
        public bool EnableKillSessionButton
        {
            get { return enableKillSessionsButton; }
            set
            {
                enableKillSessionsButton = value;
                FirePropertyChanged("EnableKillSessionButton");
            }
        }

        private bool enableDatabaseStats;
        public bool EnableDatabaseStats
        {
            get { return enableDatabaseStats; }
            set
            {
                enableDatabaseStats = value;
                FirePropertyChanged("EnableDatabaseStats");
            }
        }

        private bool enableRebuildIndices;
        public bool EnableRebuildIndices
        {
            get { return enableRebuildIndices; }
            set
            {
                enableRebuildIndices = value;
                FirePropertyChanged("EnableRebuildIndices");
            }
        }

        private bool enableFailOver;
        public bool EnableFailOver
        {
            get { return enableFailOver; }
            set
            {
                enableFailOver = value;
                FirePropertyChanged("EnableFailOver");
            }
        }

        private bool enableSuspendResume;
        public bool EnableSuspendResume
        {
            get { return enableSuspendResume; }
            set
            {
                enableSuspendResume = value;
                FirePropertyChanged("EnableSuspendResume");
            }
        }
        private bool showOptimizeButton;
        public bool ShowOptimizeButton
        {
            get
            {
                return showOptimizeButton;
            }
            set
            {
                showOptimizeButton = value;
                FirePropertyChanged("ShowOptimizeButton");
            }
        }
        private bool showRepopulateButton;
        public bool ShowRepopulateButton
        {
            get { return showRepopulateButton; }
            set
            {
                showRepopulateButton = value;
                FirePropertyChanged("ShowRepopulateButton");
            }
        }
        private bool showRebuildButton;
        public bool ShowRebuildButton
        {
            get { return showRebuildButton; }
            set
            {
                showRebuildButton = value;
                FirePropertyChanged("ShowRebuildButton");
            }
        }
        private bool showStopServiceButton;
        public bool ShowStopServiceButton
        {
            get
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                    return false;
                else
                    return (SelectedSubTab == ServerViewTabs.Services && 
                            (SelectedServerTab == ServerViews.ServicesSqlAgentJobs || 
                            SelectedServerTab == ServerViews.ServicesFullTextSearch && showStopServiceButton));
            }
            set
            {
                showStopServiceButton = value;
                FirePropertyChanged("ShowStopServiceButton");
            }
        }

        private bool showStartServiceButton;
        public bool ShowStartServiceButton
        {
            get
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                    return false;
                else
                    return (SelectedSubTab == ServerViewTabs.Services &&
                            (SelectedServerTab == ServerViews.ServicesSqlAgentJobs ||
                            SelectedServerTab == ServerViews.ServicesFullTextSearch && showStartServiceButton));
            }
            set
            {
                showStartServiceButton = value;
                FirePropertyChanged("ShowStartServiceButton");
            }
        }

        public bool ShowStopJobButton
        {
            get
            {
                return (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesSqlAgentJobs);
            }
        }

        public bool ShowStartJobButton
        {
            get
            {
                return (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesSqlAgentJobs);
            }
        }

        public bool ShowStopButton
        {
            get
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                    return false;
                else
                 return (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesSummary);
            }
        }

        public bool ShowStartButton
        {
            get
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                    return false;
                else
                    return (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesSummary);
            }
        }

        public bool ShowAnalyzeActionButtons
        {
            get
            {
                return (SelectedSubTab == ServerViewTabs.Analysis);
            }
        }

        private bool enableRebuildButton;
        public bool EnableRebuildButton
        {
            get { return enableRebuildButton; }
            set
            {
                enableRebuildButton = value;
                FirePropertyChanged("EnableRebuildButton");
            }
        }

        private bool enableOptimizeButton;
        public bool EnableOptimizeButton
        {
            get { return enableOptimizeButton; }
            set
            {
                enableOptimizeButton = value;
                FirePropertyChanged("EnableOptimizeButton");
            }
        }

        private bool enableRepopulateButton;
        public bool EnableRepopulateButton
        {
            get { return enableRepopulateButton; }
            set
            {
                enableRepopulateButton = value;
                FirePropertyChanged("EnableRepopulateButton");
            }
        }

        private bool enableStopServiceButton;
        public bool EnableStopServiceButton
        {
            get { return enableStopServiceButton; }
            set
            {
                enableStopServiceButton = value;
                FirePropertyChanged("EnableStopServiceButton");
            }
        }

        private bool enableStartServiceButton;
        public bool EnableStartServiceButton
        {
            get { return enableStartServiceButton; }
            set
            {
                enableStartServiceButton = value;
                FirePropertyChanged("EnableStartServiceButton");
            }
        }

        private bool enableStopJobButton;
        public bool EnableStopJobButton
        {
            get { return enableStopJobButton; }
            set
            {
                enableStopJobButton = value;
                FirePropertyChanged("EnableStopJobButton");
            }
        }

        private bool enableStartJobButton;
        public bool EnableStartJobButton
        {
            get { return enableStartJobButton; }
            set
            {
                enableStartJobButton = value;
                FirePropertyChanged("EnableStartJobButton");
            }
        }

        private bool enableStopButton;
        public bool EnableStopButton
        {
            get { return enableStopButton; }
            set
            {
                enableStopButton = value;
                FirePropertyChanged("EnableStopButton");
            }
        }

        private bool enableStartButton;
        public bool EnableStartButton
        {
            get { return enableStartButton; }
            set
            {
                enableStartButton = value;
                FirePropertyChanged("EnableStartButton");
            }
        }

        public bool ShowBlockingTreeButton
        {
            get {
                if (SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab == ServerViews.SessionsBlocking)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowConfigureQueryMonitorButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Queries && SelectedServerTab != ServerViews.QueryWaitStatsActive)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowClearCacheButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Resources && SelectedServerTab == ServerViews.ResourcesProcedureCache)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowUpdateStatisticsButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesTablesIndexes)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowRebuildIndexesButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesTablesIndexes)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowFailOverButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesMirroring)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowSuspendResumeButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesMirroring)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowQueriesConfigureButton
        {
            get
            {
                if (SelectedSubTab == ServerViewTabs.Queries && SelectedServerTab == ServerViews.QueryWaitStatsActive)
                    return true;
                else
                    return false;

            }
        }
        public bool ShowLogsConfigureButton
        {
            get
            {
                return SelectedSubTab == ServerViewTabs.Logs;
            }
        }
        public bool ShowCycleLogsButton
        {
            get
            {
                return SelectedSubTab == ServerViewTabs.Logs;
            }
        }
        public bool ShowFilterDialogButton
        {
            get
            {
                if ((SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab == ServerViews.SessionsDetails) ||
                    (SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab == ServerViews.SessionsLocks) ||
                    (SelectedSubTab == ServerViewTabs.Queries) ||
                    (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesTempdbView) ||
                    (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesBackupRestoreHistory) ||
                    (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesReplication) ||
                    (SelectedSubTab == ServerViewTabs.Services && SelectedServerTab == ServerViews.ServicesSqlAgentJobs) ||
                    SelectedSubTab == ServerViewTabs.Logs)

                    return true;
                else
                    return false;
            }
        }
        public bool ShowBasicFilterButton
        {
            get
            {
                if ((SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewDetails) ||
                    (SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewTimeline) ||
                    (SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesTablesIndexes))
                    return true;
                else
                    return false;
            }
        }
        private bool showDashGalleryButtons = false;
        public bool ShowDashGalleryButtons
        {
            get { return showDashGalleryButtons; }
            set
            {
                showDashGalleryButtons = value;
                FirePropertyChanged("ShowDashGalleryButtons");
                FirePropertyChanged("ShowCustomizeDashboardButton");
            }
        }
        public bool ShowBasicFilter_OvDetailsButton { get { return SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewDetails; } }
        public bool ShowBasicFilter_OvTimelineButton { get { return SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewTimeline; } }
        public bool ShowBasicFilter_DbIndexButton { get { return SelectedSubTab == ServerViewTabs.Databases && SelectedServerTab == ServerViews.DatabasesTablesIndexes; } }
        bool showOvDetailsShowAll;
        public bool ShowOvDetailsShowAll
        {
            get { return showOvDetailsShowAll; }
            set
            {
                showOvDetailsShowAll = value;
                FirePropertyChanged("ShowOvDetailsShowAll");
            }
        }
        bool showOvDetailsCustomCounter;
        public bool ShowOvDetailsCustomCounter
        {
            get { return showOvDetailsCustomCounter; }
            set
            {
                showOvDetailsCustomCounter = value;
                FirePropertyChanged("ShowOvDetailsCustomCounter");
            }
        }
        bool showBtBySession;
        public bool ShowBtBySession
        {
            get { return showBtBySession; }
            set
            {
                showBtBySession = value;
                FirePropertyChanged("ShowBtBySession");
            }
        }
        bool showBtByLock;
        public bool ShowBtByLock
        {
            get { return showBtByLock; }
            set
            {
                showBtByLock = value;
                FirePropertyChanged("ShowBtByLock");
            }
        }
        bool showDbUserDatabases;
        public bool ShowDbUserDatabases
        {
            get { return showDbUserDatabases; }
            set
            {
                showDbUserDatabases = value;
                FirePropertyChanged("ShowDbUserDatabases");
            }
        }
        bool showDbUserTables;
        public bool ShowDbUserTables
        {
            get { return showDbUserTables; }
            set
            {
                showDbUserTables = value;
                FirePropertyChanged("ShowDbUserTables");
            }
        }
        public bool ShowShowHideButton
        {
            get
            {
                if ((SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewDetails) ||
                    (SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewConfiguration) ||
                    (SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewAlerts) ||
                    (SelectedSubTab == ServerViewTabs.Sessions && SelectedServerTab != ServerViews.SessionsSummary) ||
                    (SelectedSubTab == ServerViewTabs.Queries && SelectedServerTab != ServerViews.QueryWaitStatsActive) ||
                    (SelectedSubTab == ServerViewTabs.Resources && SelectedServerTab == ServerViews.ResourcesDiskSize) ||
                    (SelectedSubTab == ServerViewTabs.Resources && SelectedServerTab == ServerViews.ResourcesFileActivity) ||
                    (SelectedSubTab == ServerViewTabs.Resources && SelectedServerTab == ServerViews.ResourcesProcedureCache) ||
                    (SelectedSubTab == ServerViewTabs.Resources && SelectedServerTab == ServerViews.ResourcesWaitStats) ||
                    SelectedSubTab == ServerViewTabs.Databases ||
                    SelectedSubTab == ServerViewTabs.Services ||
                    SelectedSubTab == ServerViewTabs.Logs)

                    return true;
                else
                    return false;
            }
        }
        public bool ShowMaintenanceModeButton
        {
            get
            {
                return SelectedSubTab == ServerViewTabs.Overview && SelectedServerTab == ServerViews.OverviewSummary;
            }
        }
        public bool ShowCustomizeDashboardButton
        {
            get
            {
                //SQLDM-31074. Show Customize button for Cloud Providers.
                if ((SelectedSubTab == ServerViewTabs.Overview) && (SelectedServerTab == ServerViews.OverviewSummary) && !ShowDashGalleryButtons)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        private DashboardFilter _dashboardSelectorFilter = DashboardFilter.User;
        public DashboardFilter DashboardSelectorFilter
        {
            get { return _dashboardSelectorFilter; }
            set
            {
                if (_dashboardSelectorFilter == value) return;
                _dashboardSelectorFilter = value;
                FirePropertyChanged("DashboardSelectorFilter");
            }
        }

        private bool _dashboardCanDeleteMode = false;
        public bool DashboardCanDeleteMode
        {
            get { return _dashboardCanDeleteMode; }
            set
            {
                if (_dashboardCanDeleteMode == value) return;
                _dashboardCanDeleteMode = value;
                FirePropertyChanged("DashboardCanDeleteMode");
            }
        }

        private DashboardMode _dashboardMode = DashboardMode.Design;
        public DashboardMode DashboardMode
        {   
            get { return _dashboardMode; }
            set
            {
                if (_dashboardMode == value) return;
                _dashboardMode = value;
                FirePropertyChanged("DashboardMode");
            }
        }

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _resourceConfigurationVisibility = string.Empty;
        public string ResourceConfigurationVisibility
        {
            get { return _resourceConfigurationVisibility; }
            set
            {
                if (_resourceConfigurationVisibility == value) return;
                _resourceConfigurationVisibility = value;
                FirePropertyChanged("ResourceConfigurationVisibility");
                FirePropertyChanged("VisibilityOverviewConfiguration");
            }
        }

        private void OpenCloseTabsFromSettings()
        {
            VisibilityOverviewDashboard = Settings.Default.VisibilityOverviewDashboard;
            VisibilityOverviewDetails = Settings.Default.VisibilityOverviewDetails;
            VisibilityOverviewConfiguration = Settings.Default.VisibilityOverviewConfiguration;
            VisibilityOverviewActiveAlerts = Settings.Default.VisibilityOverviewActiveAlerts;
            VisibilityOverviewTimeline = Settings.Default.VisibilityOverviewTimeline;
            VisibilitySessionsSummary = Settings.Default.VisibilitySessionsSummary;
            VisibilitySessionsDetails = Settings.Default.VisibilitySessionsSummary;
            VisibilitySessionsLocks = Settings.Default.VisibilitySessionsLocks;
            VisibilitySessionsBlocking = Settings.Default.VisibilitySessionsBlocking;
            VisibilityQueriesSignatureMode = Settings.Default.VisibilityQueriesSignatureMode;
            VisibilityQueriesStatementMode = Settings.Default.VisibilityQueriesStatementMode;
            VisibilityQueriesQueryHistory = Settings.Default.VisibilityQueriesQueryHistory;
            VisibilityQueriesQueryWaits = Settings.Default.VisibilityQueriesQueryWaits;
            VisibilityResourcesSummary = Settings.Default.VisibilityResourcesSummary;
            VisibilityResourcesCPU = Settings.Default.VisibilityResourcesCPU;
            VisibilityResourcesMemory = Settings.Default.VisibilityResourcesMemory;
            VisibilityResourcesDisk = Settings.Default.VisibilityResourcesDisk;
            VisibilityResourcesDiskSize = Settings.Default.VisibilityResourcesDiskSize;
            VisibilityResourcesFileActivity = Settings.Default.VisibilityResourcesFileActivity;
            VisibilityResourcesProcedureCache = Settings.Default.VisibilityResourcesProcedureCache;
            VisibilityResourcesServerWaits = Settings.Default.VisibilityResourcesServerWaits;
            VisibilityDatabaseSummary = Settings.Default.VisibilityDatabaseSummary;
            VisibilityDatabaseAvailabilityGroup = Settings.Default.VisibilityDatabaseAvailabilityGroup;
            VisibilityDatabaseTempdbSummary = Settings.Default.VisibilityDatabaseTempdbSummary;
            VisibilityDatabaseConfiguration = Settings.Default.VisibilityDatabaseConfiguration;
            VisibilityDatabaseFiles = Settings.Default.VisibilityDatabaseFiles;
            VisibilityDatabaseBackupRestore = Settings.Default.VisibilityDatabaseBackupRestore;
            VisibilityDatabaseTablesIndexes = Settings.Default.VisibilityDatabaseTablesIndexes;
            VisibilityDatabaseMirroring = Settings.Default.VisibilityDatabaseMirroring;
            VisibilityServicesSummary = Settings.Default.VisibilityServicesSummary;
            VisibilityServicesSQLAgentJobs = Settings.Default.VisibilityServicesSQLAgentJobs;
            if(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId==Common.Constants.AmazonRDSId)
            {
                VisibilityServicesFullTextSearch = collapsed; //Settings.Default.VisibilityServicesFullTextSearch;
                VisibilityServicesReplication =  collapsed;//Settings.Default.VisibilityServicesReplication;
                VisibilityDatabaseMirroring = collapsed; //Settings.Default.VisibilityDatabaseMirroring;
            }
            else
            {
                VisibilityServicesFullTextSearch = Settings.Default.VisibilityServicesFullTextSearch;
                VisibilityServicesReplication = Settings.Default.VisibilityServicesReplication;
            }
           
        }
        private void SaveOpenCloseSettings()
        {
            Settings.Default.VisibilityOverviewDashboard = VisibilityOverviewDashboard;
            Settings.Default.VisibilityOverviewDetails = VisibilityOverviewDetails;
            Settings.Default.VisibilityOverviewConfiguration = VisibilityOverviewConfiguration;
            Settings.Default.VisibilityOverviewActiveAlerts = VisibilityOverviewActiveAlerts;
            Settings.Default.VisibilityOverviewTimeline = VisibilityOverviewTimeline;
            Settings.Default.VisibilitySessionsSummary = VisibilitySessionsSummary;
            Settings.Default.VisibilitySessionsSummary = VisibilitySessionsSummary;
            Settings.Default.VisibilitySessionsLocks = VisibilitySessionsLocks;
            Settings.Default.VisibilitySessionsBlocking = VisibilitySessionsBlocking;
            Settings.Default.VisibilityQueriesSignatureMode = VisibilityQueriesSignatureMode;
            Settings.Default.VisibilityQueriesStatementMode = VisibilityQueriesStatementMode;
            Settings.Default.VisibilityQueriesQueryHistory = VisibilityQueriesQueryHistory;
            Settings.Default.VisibilityQueriesQueryWaits = VisibilityQueriesQueryWaits;
            Settings.Default.VisibilityResourcesSummary = VisibilityResourcesSummary;
            Settings.Default.VisibilityResourcesCPU = VisibilityResourcesCPU;
            Settings.Default.VisibilityResourcesMemory = VisibilityResourcesMemory;
            Settings.Default.VisibilityResourcesDisk = VisibilityResourcesDisk;
            Settings.Default.VisibilityResourcesDiskSize = VisibilityResourcesDiskSize;
            Settings.Default.VisibilityResourcesFileActivity = VisibilityResourcesFileActivity;
            Settings.Default.VisibilityResourcesProcedureCache = VisibilityResourcesProcedureCache;
            Settings.Default.VisibilityResourcesServerWaits = VisibilityResourcesServerWaits;
            Settings.Default.VisibilityDatabaseSummary = VisibilityDatabaseSummary;
            Settings.Default.VisibilityDatabaseAvailabilityGroup = VisibilityDatabaseAvailabilityGroup;
            Settings.Default.VisibilityDatabaseTempdbSummary = VisibilityDatabaseTempdbSummary;
            Settings.Default.VisibilityDatabaseConfiguration = VisibilityDatabaseConfiguration;
            Settings.Default.VisibilityDatabaseFiles = VisibilityDatabaseFiles;
            Settings.Default.VisibilityDatabaseBackupRestore = VisibilityDatabaseBackupRestore;
            Settings.Default.VisibilityDatabaseTablesIndexes = VisibilityDatabaseTablesIndexes;
            Settings.Default.VisibilityDatabaseMirroring = VisibilityDatabaseMirroring;
            Settings.Default.VisibilityServicesSummary = VisibilityServicesSummary;
            Settings.Default.VisibilityServicesSQLAgentJobs = VisibilityServicesSQLAgentJobs;
            Settings.Default.VisibilityServicesFullTextSearch = VisibilityServicesFullTextSearch;
            Settings.Default.VisibilityServicesReplication = VisibilityServicesReplication;
            Settings.Default.Save();
        }
        public void AddTab(string tabName)
        {
            switch (tabName)
            {
                case "addOverviewDashboardTab":
                    VisibilityOverviewDashboard = "Visible";
                    break;
                case "addOverviewDetailsTab":
                    VisibilityOverviewDetails = "Visible";
                    break;
                case "addOverviewConfigurationTab":
                    VisibilityOverviewConfiguration = "Visible";
                    break;
                case "addOverviewActiveAlertTab":
                    VisibilityOverviewActiveAlerts = "Visible";
                    break;
                case "addOverviewTimelineTab":
                    VisibilityOverviewTimeline = "Visible";
                    break;
                case "addSessionsSummaryTab":
                    VisibilitySessionsSummary = "Visible";
                    break;
                case "addSessionsDetailsTab":
                    VisibilitySessionsDetails = "Visible";
                    break;
                case "addSessionsLocksTab":
                    VisibilitySessionsLocks = "Visible";
                    break;
                case "addSessionsBlockingTab":
                    VisibilitySessionsBlocking = "Visible";
                    break;
                case "addQueriesSignatureModeTab":
                    VisibilityQueriesSignatureMode = "Visible";
                    break;
                case "addQueriesStatementModeTab":
                    VisibilityQueriesStatementMode = "Visible";
                    break;
                case "addQueriesQueryHistoryTab":
                    VisibilityQueriesQueryHistory = "Visible";
                    break;
                case "addQueriesQueryWaitTab":
                    VisibilityQueriesQueryWaits = "Visible";
                    break;
                case "addResourcesSummaryTab":
                    VisibilityResourcesSummary = "Visible";
                    break;
                case "addResourcesCPUTab":
                    VisibilityResourcesCPU = "Visible";
                    break;
                case "addResourcesMemoryTab":
                    VisibilityResourcesMemory = "Visible";
                    break;
                case "addResourcesDiskTab":
                    VisibilityResourcesDisk = "Visible";
                    break;
                case "addResourcesDiskSizeTab":
                    VisibilityResourcesDiskSize = "Visible";
                    break;
                case "addResourcesFileActivityTab":
                    VisibilityResourcesFileActivity = "Visible";
                    break;
                case "addResourcesProcedureCacheTab":
                    VisibilityResourcesProcedureCache = "Visible";
                    break;
                case "addResourcesWaitStatsTab":
                    VisibilityResourcesServerWaits = "Visible";
                    break;
                case "addDatabasesSummaryTab":
                    VisibilityDatabaseSummary = "Visible";
                    break;
                case "addDatabasesAlwaysOnTab":
                    VisibilityDatabaseAvailabilityGroup = "Visible";
                    break;
                case "addDatabasesTempdbTab":
                    VisibilityDatabaseTempdbSummary = "Visible";
                    break;
                case "addDatabasesConfigurationTab":
                    VisibilityDatabaseConfiguration = "Visible";
                    break;
                case "addDatabasesFilesTab":
                    VisibilityDatabaseFiles = "Visible";
                    break;
                case "addDatabasesBackupRestoreTab":
                    VisibilityDatabaseBackupRestore = "Visible";
                    break;
                case "addDatabasesTablesIndexesTab":
                    VisibilityDatabaseTablesIndexes = "Visible";
                    break;
                case "addDatabasesMirroringTab":
                    VisibilityDatabaseMirroring = "Visible";
                    break;
                case "addServicesSummaryTab":
                    VisibilityServicesSummary = "Visible";
                    break;
                case "addSqlAgentJobsTab":
                    VisibilityServicesSQLAgentJobs = "Visible";
                    break;
                case "addServicesFullTextSearchTab":
                    VisibilityServicesFullTextSearch = "Visible";
                    break;
                case "addServicesReplicationTab":
                    VisibilityServicesReplication = "Visible";
                    break;             

            }
            SaveOpenCloseSettings();
        }
        public void RemoveTab(string tabName)
        {
            switch (tabName)
            {
                case "overviewDashboard":
                    VisibilityOverviewDashboard = "Collapsed";
                    break;
                case "overviewDetails":
                    VisibilityOverviewDetails = "Collapsed";
                    break;
                case "overviewConfiguration":
                    VisibilityOverviewConfiguration = "Collapsed";
                    break;
                case "overviewActiveAlerts":
                    VisibilityOverviewActiveAlerts = "Collapsed";
                    break;
                case "overviewTimeline":
                    VisibilityOverviewTimeline = "Collapsed";
                    break;
                case "sessionsSummary":
                    VisibilitySessionsSummary = "Collapsed";
                    break;
                case "sessionsDetails":
                    VisibilitySessionsDetails = "Collapsed";
                    break;
                case "sessionsLocks":
                    VisibilitySessionsLocks = "Collapsed";
                    break;
                case "sessionsBlocking":
                    VisibilitySessionsBlocking = "Collapsed";
                    break;
                case "queriesSignatureMode":
                    VisibilityQueriesSignatureMode = "Collapsed";
                    break;
                case "queriesStatementMode":
                    VisibilityQueriesStatementMode = "Collapsed";
                    break;
                case "queriesQueryHistory":
                    VisibilityQueriesQueryHistory = "Collapsed";
                    break;
                case "queriesQueryWaits":
                    VisibilityQueriesQueryWaits = "Collapsed";
                    break;
                case "resourcesSummary":
                    VisibilityResourcesSummary = "Collapsed";
                    break;
                case "resourcesCPU":
                    VisibilityResourcesCPU = "Collapsed";
                    break;
                case "resourcesMemory":
                    VisibilityResourcesMemory = "Collapsed";
                    break;                
                case "resourcesDisk":
                    VisibilityResourcesDisk = "Collapsed";
                    break;
                case "resourcesDiskSize":
                    VisibilityResourcesDiskSize = "Collapsed";
                    break;
                case "resourcesFileActivity":
                    VisibilityResourcesFileActivity = "Collapsed";
                    break;
                case "resourcesProcedureCache":
                    VisibilityResourcesProcedureCache = "Collapsed";
                    break;
                case "resourcesServerWaits":
                    VisibilityResourcesServerWaits = "Collapsed";
                    break;
                case "databasesSummary":
                    VisibilityDatabaseSummary = "Collapsed";
                    break;
                case "databasesAvailabilityGroup":
                    VisibilityDatabaseAvailabilityGroup = "Collapsed";
                    break;
                case "databasesTempDBSummary":
                    VisibilityDatabaseTempdbSummary = "Collapsed";
                    break;
                case "databasesConfiguration":
                    VisibilityDatabaseConfiguration = "Collapsed";
                    break;
                case "databasesFiles":
                    VisibilityDatabaseFiles = "Collapsed";
                    break;
                case "databasesBackupRestore":
                    VisibilityDatabaseBackupRestore = "Collapsed";
                    break;
                case "databasesTablesIndexes":
                    VisibilityDatabaseTablesIndexes = "Collapsed";
                    break;
                case "databasesMirroring":
                    VisibilityDatabaseMirroring = "Collapsed";
                    break;
                case "servicesSummary":
                    VisibilityServicesSummary = "Collapsed";
                    break;
                case "servicesSQLAgentJobs":
                    VisibilityServicesSQLAgentJobs = "Collapsed";
                    break;
                case "servicesFullTextSearch":
                    VisibilityServicesFullTextSearch = "Collapsed";
                    break;
                case "servicesReplication":
                    VisibilityServicesReplication = "Collapsed";
                    break;
            }

            SaveOpenCloseSettings();
        }
        //Method to show hide tabs/ribbons on instance type selection.
        public void ShowHideMenuItems()
        {
            switch (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId)
            {
               
                case 1: // For RDS instance
                    VisibilityQueries = visible;
                    VisibilityDashboard = visible;
                    VisibilityLocks = visible;
                    VisibilityBlocking = visible;
                    ServicesTabItemVisibility = visible;
                    LogTabItemVisibility = visible;
                    VisibilityCPU = visible;
                    VisibilityAvailabilityGroup = visible;//collapsed;
                    VisibilityTempdbSummary = visible;
                    VisibilityFiles = visible;
                    VisibilityMirroring = visible;//collapsed;
                    VisibilityBackupsRestores = visible;
                    VisibilityTablesIndexes = visible;
                    VisibilitySummary = visible;
                    VisibilityMemory = visible;
                    VisibilityDisk = visible;
                    ServicesTabItemVisibilityforAzureDb = collapsed;
                    VisibilityProcedurecache = visible;
                    VisibilityServerwaits = visible;
                    LogTabItemVisibility1 = collapsed;
                    ServicesTabItemVisibility = collapsed;
                    LogTabItemVisibility = collapsed;
                    ResourceTabDiskSizeVisibility = visible;//collapsed;
                    ResourceTabFileActivityVisibility = visible;//collapsed;
                    ResourceConfigurationVisibility = visible;
                    VisibilityDiskSize = visible;//collapsed;
                    break;
                case 2: // For AzureDB instance
                  
                    ServicesTabItemVisibilityforAzureDb = collapsed;
                    VisibilityDashboard = collapsed;
                    VisibilityLocks = visible;
                    VisibilityBlocking = visible;
                    VisibilityQueries = visible;
                    ServicesTabItemVisibility = collapsed;
                    LogTabItemVisibility = collapsed;
                    LogTabItemVisibility1 = collapsed;
                    VisibilityCPU = visible;
                    VisibilityAvailabilityGroup = collapsed;
                    VisibilityTempdbSummary = collapsed;
                    VisibilityFiles = collapsed;
                    VisibilityMirroring = collapsed;
                    VisibilityBackupsRestores = collapsed;
                    VisibilityTablesIndexes = collapsed;

                    VisibilitySummary = visible;
                    VisibilityMemory = visible;
                    VisibilityDisk = visible;

                    VisibilityProcedurecache = collapsed;
                    VisibilityServerwaits = visible;
                    ServicesTabItemVisibility = visible;
                    LogTabItemVisibility = visible;
                    ResourceTabDiskSizeVisibility = visible;
                    ResourceTabFileActivityVisibility = collapsed;
                    ResourceConfigurationVisibility = collapsed;
                    VisibilityDiskSize = collapsed;
                    break;
                //case 3: // For Linux instance
                //    VisibilityServerwaits = visible;
                //    ServicesTabItemVisibilityforAzureDb = visible;
                //    ServicesTabItemVisibility = visible;
                //    ResourceTabDiskSizeVisibility = visible;
                //    ResourceTabFileActivityVisibility = visible;
                //    ResourceConfigurationVisibility = visible;
                //    VisibilityDiskSize = visible;
                //    LogTabItemVisibility1 = visible;
                //    break;

                default:  //For Windows instance
                    VisibilityQueries = visible;
                    VisibilityDashboard = visible;
                    VisibilityLocks = visible;
                    VisibilityBlocking = visible;
                    ServicesTabItemVisibility = visible;
                    LogTabItemVisibility = visible;
                    VisibilityCPU = visible;
                    VisibilityAvailabilityGroup = visible;
                    VisibilityTempdbSummary = visible;
                    VisibilityFiles = visible;
                    VisibilityMirroring = visible;
                    ServicesTabItemVisibilityforAzureDb = visible;
                    VisibilityBackupsRestores = visible;
                    VisibilityTablesIndexes = visible;
                    VisibilitySummary = visible;
                    VisibilityMemory = visible;
                    VisibilityDisk = visible;

                    VisibilityProcedurecache = visible;
                    VisibilityServerwaits = visible;
                    ServicesTabItemVisibility = visible;
                    LogTabItemVisibility = visible;
                    ResourceTabDiskSizeVisibility = visible;
                    ResourceTabFileActivityVisibility = visible;
                    ResourceConfigurationVisibility = visible;
                    VisibilityDiskSize = visible;
                    LogTabItemVisibility1 = visible;
                    break;
            }
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == 5)
                VisibilityMirroring = collapsed;
        }

        private string _parentBreadcrumb = string.Empty;
        public string ParentBreadcrumb
        {
            get { return _parentBreadcrumb; }
            set
            {
                if (_parentBreadcrumb == value) return;
                _parentBreadcrumb = value;
                FirePropertyChanged("ParentBreadcrumb");
            }
        }

        private string _childBreadcrumb = string.Empty;
        public string ChildBreadcrumb
        {
            get { return _childBreadcrumb; }
            set
            {
                if (_childBreadcrumb == value) return;
                _childBreadcrumb = value;
                FirePropertyChanged("ChildBreadcrumb");
                FirePropertyChanged("ChildBreadcrumbVisible");
            }
        }

        public bool ChildBreadcrumbVisible
        {
            get
            {
                return !string.IsNullOrEmpty(ChildBreadcrumb);
            }
        }

        private DateTime _endHistoryDate = DateTime.Now;
        public DateTime EndHistoryDate
        {
            get { return _endHistoryDate; }
            set
            {
                if (_endHistoryDate == value) return;
                _endHistoryDate = value;
                FirePropertyChanged("EndHistoryDate");
            }
        }
        private DateTime _startHistoryDate = DateTime.Now;
        public DateTime StartHistoryDate
        {
            get { return _startHistoryDate; }
            set
            {
                if (_startHistoryDate == value) return;
                _startHistoryDate = value;
                FirePropertyChanged("StartHistoryDate");
            }
        }
        private DateTime _endHistoryTime = DateTime.Now;
        public DateTime EndHistoryTime
        {
            get { return _endHistoryTime; }
            set
            {
                if (_endHistoryTime == value) return;
                _endHistoryTime = value;
                FirePropertyChanged("EndHistoryTime");
            }
        }
        private DateTime _startHistoryTime = DateTime.Now;
        public DateTime StartHistoryTime
        {
            get { return _startHistoryTime; }
            set
            {
                if (_startHistoryTime == value) return;
                _startHistoryTime = value;
                FirePropertyChanged("StartHistoryTime");
            }
        }
        public DateTime StartHistoryDateTime
        {
            get
            {
                var time = StartHistoryTime.TimeOfDay;
                var date = StartHistoryDate.Date;
                return date + time;
            }
            set
            {
                var time = value.TimeOfDay;
                var date = value.Date;
                _startHistoryTime = new DateTime().Date + time;
                _startHistoryDate = date;
                FirePropertyChanged("StartHistoryDate");
                FirePropertyChanged("StartHistoryTime");
            }
        }
        public DateTime EndHistoryDateTime
        {
            get
            {
                var time = EndHistoryTime.TimeOfDay;
                var date = EndHistoryDate.Date;
                return date + time;
            }
            set
            {
                var time = value.TimeOfDay;
                var date = value.Date;
                _endHistoryTime = new DateTime().Date + time;
                _endHistoryDate = date;
                FirePropertyChanged("EndHistoryDate");
                FirePropertyChanged("EndHistoryTime");
            }
        }
        private bool dayMonthYearFormat;
        public bool DayMonthYearFormat
        {
            get
            {
                return dayMonthYearFormat;
            }
            set
            {
                dayMonthYearFormat = value;
                FirePropertyChanged("DayMonthYearFormat");
            }
        }

        private bool _isCustomHistory = false;
        public bool IsCustomHistory
        {
            get { return _isCustomHistory; }
            set
            {
                _isCustomHistory = value;
                FirePropertyChanged("IsCustomHistory");
                FirePropertyChanged("ShowCustomControls");
            }
        }
        private bool _isLive = true;
        public bool IsLive
        {
            get { return _isLive; }
            set
            {
                _isLive = value;
                FirePropertyChanged("IsLive");
                FirePropertyChanged("ShowCustomControls");
            }
        }


        public bool ShowCustomControls
        {
            get { return _isCustomHistory; }
            
        }

        #region Quick Historical Snapshot

        private bool _quickHistoricalSnapshotVisible;
        public bool QuickHistoricalSnapshotVisible
        {
            get { return _quickHistoricalSnapshotVisible &&
                    !(SelectedServerTab == ServerViews.OverviewConfiguration 
                    || SelectedServerTab == ServerViews.OverviewTimeline 
                    || SelectedServerTab == ServerViews.ResourcesFileActivity 
                    || SelectedServerTab == ServerViews.ResourcesProcedureCache 
                    || SelectedServerTab == ServerViews.DatabasesSummary
                    || SelectedServerTab == ServerViews.DatabasesConfiguration 
                    || SelectedServerTab == ServerViews.DatabasesFiles 
                    || SelectedServerTab == ServerViews.DatabasesBackupRestoreHistory 
                    || SelectedServerTab == ServerViews.DatabasesTablesIndexes 
                    || SelectedServerTab == ServerViews.DatabasesMirroring 
                    || SelectedServerTab == ServerViews.ServicesSummary
                    || SelectedServerTab == ServerViews.ServicesSqlAgentJobs 
                    || SelectedServerTab == ServerViews.ServicesFullTextSearch 
                    || SelectedServerTab == ServerViews.ServicesReplication 
                    || SelectedServerTab == ServerViews.Logs); 
            }

            set
            {
                if (_quickHistoricalSnapshotVisible == value)
                    return;

                _quickHistoricalSnapshotVisible = value;
                FirePropertyChanged("QuickHistoricalSnapshotVisible");
            }
        }

        private string _quickHistoricalSnapshotText = string.Empty;
        public string QuickHistoricalSnapshotText
        {
            get { return _quickHistoricalSnapshotText; }
            set
            {
                if (_quickHistoricalSnapshotText == value) 
                    return;
                
                _quickHistoricalSnapshotText = value;
                FirePropertyChanged("QuickHistoricalSnapshotText");
            }
        }

        #endregion
    }

    internal static class RibbonExtensions
    {
        public static string ExtractToolId(string id)
        {
            var i = id.IndexOf("_");
            return i > 0 ? id.Substring(i + 1) : id;
        }

        public static string GetId(this IRibbonTool tool)
        {
            if (tool is ButtonTool)
                return ((ButtonTool)tool).GetId();
            if (tool is ToggleButtonTool)
                return ((ToggleButtonTool)tool).GetId();
            if (tool is RadioButtonTool)
                return ((RadioButtonTool)tool).GetId();

            return null;
        }

        public static string GetId(this ButtonTool tool)
        {
            return ExtractToolId(tool.Id);
        }
        public static string GetId(this ToggleButtonTool tool)
        {
            return ExtractToolId(tool.Id);
        }
        public static string GetId(this RadioButtonTool tool)
        {
            return ExtractToolId(tool.Id);
        }

        public static RadioButtonTool FindCheckedTool(this RibbonGroup group)
        {
            if (group.Items.Count == 0) return null;

            IList items = group.Items;
            if (items[0] is Panel)
                items = ((Panel)items[0]).Children;

            return items.Cast<object>()
                        .Where(tool => tool is RadioButtonTool 
                           && ((RadioButtonTool) tool).IsChecked.HasValue 
                           && ((RadioButtonTool) tool).IsChecked.Value).Cast<RadioButtonTool>().FirstOrDefault();
        }
    }

    public enum DashboardMode
    {
        Gallery,
        Design
    }
}
