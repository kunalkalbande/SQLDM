using System;
using System.Diagnostics;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Logs;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Services;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions;
using Idera.Newsfeed.Plugins.UI;
using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.MessageBox;
using Images = Idera.SQLdm.DesktopClient.Properties.Resources;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using ToolTip = Infragistics.Win.ToolTip;
using ToolTipDisplayStyle = Infragistics.Win.ToolTipDisplayStyle;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    using Alerts;
    using System.Collections.Generic;
using System.Drawing;
    using Wintellect.PowerCollections;

    internal partial class ServerViewRibbonControl : UserControl
    {
        private int instanceId;
        private ServerView parentView;
        private ServerBaseView activeView;

        private ServerSummaryView4 serverSummaryView;
        private ServerDetailsView serverDetailsView;
        private ServerConfigurationView serverConfigurationView;
        private ActiveAlertsView serverActiveAlertsView;
        private SessionsSummaryView sessionsSummaryView;
        private SessionsDetailsView sessionsDetailsView;
        private SessionsLocksView sessionsLocksView;
        private SessionsBlockingView sessionsBlockingView;
        private QueryMonitorView queryMonitorView;
        private ResourcesWaitStatsActive queriesWaitStatsActiveView;
        private ResourcesSummaryView resourcesSummaryView;
        private ResourcesCpuView resourcesCpuView;
        private ResourcesMemoryView resourcesMemoryView;
        private ResourcesDiskView resourcesDiskView;
        private ResourcesDiskSizeView resourcesDiskSizeView;     // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
        private ResourcesProcedureCacheView resourcesProcedureCacheView;
        private ResourcesWaitStats resourcesWaitStatsView;
        private ResourcesFileActivity resourcesFileActivityView;
        private DatabasesSummaryView databasesSummaryView;
        private DatabasesConfigurationView databasesConfigurationView;
        private DatabasesBackupRestoreHistoryView databasesBackupRestoreHistoryView;
        private DatabasesTablesIndexesView databasesTablesIndexesView;
        private DatabasesMirroringView databasesMirroringView;
        private DatabasesFilesView databasesFilesView;
        private DatabasesTempdbView databasesTempdbView;
        private DatabasesAlwaysOnView databasesAlwaysOnView;
        private ServicesSummaryView servicesSummaryView;
        private ServicesSqlAgentJobsView servicesSqlAgentJobsView;
        private ServicesFullTextSearchView servicesFullTextSearchView;
        private ServicesReplicationView servicesReplicationView;
        private LogsView logsView;
        //SQLdm 10.0 Srishti Purohit -- Doctors UI
        private Analysis.Recommendations analysisView;
       
        private readonly ServerSummaryHistoryData serverSummaryhistoryData = new ServerSummaryHistoryData();
        private RibbonTab overviewTab;
        private RibbonTab sessionsTab;
        private RibbonTab queriesTab;
        private RibbonTab resourcesTab;
        private RibbonTab databasesTab;
        private RibbonTab servicesTab;
        private RibbonTab logsTab;
        private RibbonTab analysisTab;
        private Infragistics.Win.ToolTip tooltip;
        private MonitoredSqlServerStatus activeStatus;

        public event EventHandler<SubViewChangedEventArgs> SubViewChanged;

        private bool ignoreToolClick = false;
        private bool ignoreTabSelect = false;

        public ServerViewRibbonControl()
        {
            InitializeComponent();

            tooltip = new ToolTip(_ServerViewContainer_Toolbars_Dock_Area_Top);
            tooltip.AutoPopDelay = 0;
            tooltip.DisplayStyle = ToolTipDisplayStyle.Office2007;
            tooltip.InitialDelay = 500;

            // reference ribbon tabs
            overviewTab = toolbarsManager.Ribbon.Tabs["Overview"];
            sessionsTab = toolbarsManager.Ribbon.Tabs["Sessions"];
            queriesTab = toolbarsManager.Ribbon.Tabs["Queries"];
            resourcesTab = toolbarsManager.Ribbon.Tabs["Resources"];
            databasesTab = toolbarsManager.Ribbon.Tabs["Databases"];
            servicesTab = toolbarsManager.Ribbon.Tabs["Services"];
            logsTab = toolbarsManager.Ribbon.Tabs["Logs"];
            analysisTab = toolbarsManager.Ribbon.Tabs["Analysis"];

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            UpdateRibbonState();

            PulseController.Default.AuthenticationChanged += PulseController_AuthenticationChanged;
            PulseController.Default.PulseConnectionChanged += PulseController_PulseConnectionChanged;
            PulseController.Default.AssetChanged += PulseController_ServerChanged;
        }

 
        public ServerViewTabs SelectedTab
        {
            get { return (ServerViewTabs) toolbarsManager.Ribbon.SelectedTab.Index; }
            set { toolbarsManager.Ribbon.SelectedTab = toolbarsManager.Ribbon.Tabs[(int)value]; }
        }

        public ServerBaseView ActiveView
        {
            get { return activeView; }
        }

        void ApplicationController_BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            MonitoredSqlServerStatus status = null;

            if (ActiveView.HistoricalSnapshotDateTime != null)
                status = serverSummaryhistoryData.HistoricalSnapshotStatus;
            else
                status = ApplicationModel.Default.GetInstanceStatus(instanceId);

            UpdateStatus(status);
        }

        private void UpdateStatus(MonitoredSqlServerStatus status)
        {
            activeStatus = status;
            overviewTab.Settings.TabItemAppearance.Image = (status != null) ? status.ServerImage : Images.ServerInformation;

            SetTabImage(status, queriesTab, MetricCategory.Queries, Images.Queries,Images.QueriesWarning16x16, Images.QueriesCritical16x16);
            SetTabImage(status, sessionsTab, MetricCategory.Sessions, Images.Sessions, Images.SessionsWarning16x16, Images.SessionsCritical16x16);
            SetTabImage(status, resourcesTab, MetricCategory.Resources, Images.ResourcesSmall, Images.ResourcesWarning16x16, Images.ResourcesCritical16x16);
            SetTabImage(status, databasesTab, MetricCategory.Databases, Images.Databases, Images.DatabasesWarning16x16, Images.DatabasesCritical16x16);
            SetTabImage(status, servicesTab, MetricCategory.Services, Images.Services, Images.ServicesWarning16x16, Images.ServicesCritical16x16);
            SetTabImage(status, logsTab, MetricCategory.Logs, Images.Logs, Images.LogsWarning16x16, Images.LogsCritical16x16);
            //10.0 SQLdm srishti purohit Doctro's UI
            SetTabImage(status,analysisTab, MetricCategory.Analyze, Images.Logs, Images.LogsWarning16x16, Images.LogsCritical16x16);
        }

        private void SetTabImage(MonitoredSqlServerStatus status, RibbonTab tab, MetricCategory metricCategory, Bitmap ok, Bitmap warning, Bitmap critical)
        {
            ICollection<Issue> issues = null;
            if (status != null)
                issues = status[metricCategory];

            SetTabImage(tab, issues, ok, warning, critical);
        }

        private void SetTabImage(RibbonTab tab, ICollection<Issue> issues, Image ok,  Image warning, Image critical)
        {
            var severity = MonitoredState.OK;
            if (issues != null && issues.Count > 0)
            {
                IEnumerator<Issue> e_issues = issues.GetEnumerator();
                if (e_issues.MoveNext())
                    severity = e_issues.Current.Severity;
            }

            switch (severity)
            {
                case MonitoredState.Informational:
                case MonitoredState.OK:
                    tab.Settings.TabItemAppearance.Image = ok;
                    break;
                case MonitoredState.Warning:
                    tab.Settings.TabItemAppearance.Image = warning;
                    break;
                case MonitoredState.Critical:
                    tab.Settings.TabItemAppearance.Image = critical;
                    break;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowServerSummaryAnimations":
                    UpdateServerSummaryViewShowAnimationsButton();
                    break;

                case "CollapseRibbon":
                    UpdateRibbonState();
                    break;
            }
        }

        private void UpdateRibbonState()
        {
            toolbarsManager.Ribbon.IsMinimized = Settings.Default.CollapseRibbon;
        }

        public void ShowActiveViewHelp() {
            if (activeView == null) {
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName, HelpNavigator.TableOfContents);
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName);
            } else {
                activeView.ShowHelp();
            }
        }

        public void SaveViewSettings()
        {
            if (activeView != null)
            {
                activeView.SaveSettings();
            }
        }

        public void ShowView(ServerViews view, object argument)
        {
            SaveViewSettings();

            switch (view)
            {
                case ServerViews.Overview:
                case ServerViews.OverviewSummary:
                    ShowView(ServerViewTabs.Overview, "overviewTabSummaryViewButton");
                    break;
                case ServerViews.OverviewDetails:
                    ShowView(ServerViewTabs.Overview, "overviewTabDetailsViewButton");
                    break;
                case ServerViews.OverviewConfiguration:
                    ShowView(ServerViewTabs.Overview, "overviewTabConfigurationViewButton");
                    break;
                case ServerViews.OverviewAlerts:
                    ShowView(ServerViewTabs.Overview, "overviewTabAlertsViewButton");
                    break;
                case ServerViews.Sessions:
                    SelectedTab = ServerViewTabs.Sessions;
                    break;
                case ServerViews.SessionsSummary:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabSummaryViewButton");
                    break;
                case ServerViews.SessionsDetails:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabDetailsViewButton");
                    break;
                case ServerViews.SessionsLocks:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabLocksViewButton");
                    break;
                case ServerViews.SessionsBlocking:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabBlockingViewButton");
                    break;
                case ServerViews.Queries:
                    //SelectedTab = ServerViewTabs.Queries;
                    //break;
                case ServerViews.QueriesSignatures:
                    ShowView(ServerViewTabs.Queries, "queriesTabSignatureModeViewButton");
                    break;
                case ServerViews.QueriesStatements:
                    ShowView(ServerViewTabs.Queries, "queriesTabStatementModeViewButton");
                    break;
                case ServerViews.QueriesHistory:
                    ShowView(ServerViewTabs.Queries, "queriesTabHistoryModeViewButton");
                    break;
                case ServerViews.Resources:
                    SelectedTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesSummary:
                    ShowView(ServerViewTabs.Resources, "resourcesTabSummaryViewButton");
                    break;
                case ServerViews.ResourcesCpu:
                    ShowView(ServerViewTabs.Resources, "resourcesTabCpuViewButton");
                    break;
                case ServerViews.ResourcesMemory:
                    ShowView(ServerViewTabs.Resources, "resourcesTabMemoryViewButton");
                    break;
                case ServerViews.ResourcesDisk:
                    ShowView(ServerViewTabs.Resources, "resourcesTabDiskViewButton");
                    break;
                case ServerViews.ResourcesDiskSize:
                    ShowView(ServerViewTabs.Resources, "resourcesTabDiskSizeViewButton");        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
                    break;
                case ServerViews.ResourcesProcedureCache:
                    ShowView(ServerViewTabs.Resources, "resourcesTabProcedureCacheViewButton");
                    break;
                case ServerViews.ResourcesWaitStats:
                    ShowView(ServerViewTabs.Resources, "resourcesTabWaitStatsViewButton");
                    break;
                case ServerViews.QueryWaitStatsActive:
                    ShowView(ServerViewTabs.Resources, "resourcesTabWaitStatsActiveViewButton");
                    break;
                case ServerViews.ResourcesFileActivity:
                    ShowView(ServerViewTabs.Resources, "resourcesTabFileActivityViewButton");
                    break;
                case ServerViews.Databases:
                    SelectedTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesSummary:
                    ShowView(ServerViewTabs.Databases, "databasesTabSummaryViewButton");
                    break;
                case ServerViews.DatabasesConfiguration:
                    ShowView(ServerViewTabs.Databases, "databasesTabConfigurationViewButton");
                    break;
                case ServerViews.DatabasesBackupRestoreHistory:
                    ShowView(ServerViewTabs.Databases, "databasesTabBackupRestoreHistoryViewButton");
                    break;
                case ServerViews.DatabasesTablesIndexes:
                    ShowView(ServerViewTabs.Databases, "databasesTabTablesIndexesViewButton");
                    break;
                case ServerViews.DatabasesFiles:
                    ShowView(ServerViewTabs.Databases, "databasesTabFilesViewButton");
                    break;
                case ServerViews.DatabasesMirroring:
                    ShowView(ServerViewTabs.Databases, "databasesTabMirroringViewButton");
                    break;
                case ServerViews.DatabasesTempdbView:
                    ShowView(ServerViewTabs.Databases, "databasesTabTempdbSummaryViewButton");
                    break;
                case ServerViews.DatabaseAlwaysOn:
                    ShowView(ServerViewTabs.Databases, "databasesTabAlwaysOnViewButton");
                    break;
                case ServerViews.Services:
                    SelectedTab = ServerViewTabs.Services;
                    break;
                case ServerViews.ServicesSummary:
                    ShowView(ServerViewTabs.Services, "servicesTabSummaryViewButton");
                    break;
                case ServerViews.ServicesSqlAgentJobs:
                    ShowView(ServerViewTabs.Services, "servicesTabSqlAgentJobsViewButton");
                    break;
                case ServerViews.ServicesFullTextSearch:
                    ShowView(ServerViewTabs.Services, "servicesTabFullTextSearchViewButton");
                    break;
                case ServerViews.ServicesReplication:
                    ShowView(ServerViewTabs.Services, "servicesTabReplicationViewButton");
                    break;
                case ServerViews.Logs:
                    SelectedTab = ServerViewTabs.Logs;
                    break;
            }

            if (activeView != null)
            {
                activeView.SetArgument(argument);

                if (activeView.HistoricalSnapshotDateTime.HasValue)
                {
                    ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
                }
            }
        }

        private void ShowView(ServerViewTabs tab, string viewButtonKey)
        {
            if (viewButtonKey != null && viewButtonKey.Length > 0)
            {
                if (!((StateButtonTool)toolbarsManager.Tools[viewButtonKey]).Checked)
                {
                    ignoreTabSelect = true;
                    ((StateButtonTool)toolbarsManager.Tools[viewButtonKey]).Checked = true;
                }
                else if (SelectedTab != tab)
                {
                    // The selected view is already current on it's tab, but it wasn't on the shown tab, 
                    // so it needs to be made the active view again since the tab is changing
                    toolbarsManager_ToolClick(toolbarsManager, new ToolClickEventArgs(toolbarsManager.Tools[viewButtonKey], null));
                }


                // If the tab is already selected, the tab select event will not fire
                // and the view will not refresh, so it must be forced
                if (!ignoreTabSelect && SelectedTab == tab)
                {
                    ApplicationController.Default.RefreshActiveView();
                }

                SelectedTab = tab;
                ignoreTabSelect = false;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            //setting the Checked property on the tools for appearance fires the click event
            if (ignoreToolClick)
            {
                return;
            }

            switch (e.Tool.Key)
            {
                case "stopServerButton":
                    StopSqlServer();
                    break;
                case "toggleHistoryBrowserButton":
                    ToggleHistoryBrowser();
                    break;
                case "showPreviousSnapshotButton":
                    ShowPreviousHistoricalSnapshot();
                    break;
                case "showNextSnapshotButton":
                    ShowNextHistoricalSnapshot();
                    break;
                case "maintenanceModeButton":
                    ToggleMaintenanceMode();
                    break;
                case "toggleServerSummaryViewShowAnimationsButton":
                    ToggleServerSummaryViewShowAnimations();
                    break;
                case "overviewTabSummaryViewButton":
                    ShowServerSummaryView();
                    break;
                case "overviewTabDetailsViewButton":
                    ShowServerDetailsView();
                    break;
                case "overviewTabConfigurationViewButton":
                    ShowServerConfigurationView();
                    break;
                case "overviewTabPulseShowProfileButton":
                    ShowServerConfigurationView();
                    break;
                case "overviewTabAlertsViewButton":
                    ShowServerAlertsView();
                    break;
                case "sessionsTabSummaryViewButton":
                    ShowSessionsSummaryView();
                    break;
                case "sessionsTabDetailsViewButton":
                    ShowSessionsDetailsView();
                    break;
                case "sessionsTabLocksViewButton":
                    ShowSessionsLocksView();
                    break;
                case "sessionsTabBlockingViewButton":
                    ShowSessionsBlockingView();
                    break;
                case "toggleServerDetailsChartButton":
                    ToggleServerDetailsChart();
                    break;
                case "toggleServerDetailsGroupByBoxButton":
                    ToggleServerDetailsGroupByBox();
                    break;
                case "toggleServerConfigurationDetailsButton":
                    ToggleServerConfigurationDetails();
                    break;
                case "toggleServerDetailsPropertiesButton":
                    ToggleServerDetailsProperties();
                    break;
                case "toggleServerConfigurationGroupByBoxButton":
                    ToggleServerConfigurationGroupByBox();
                    break;
                case "toggleActiveAlertsForecastButton":
                    ToggleActiveAlertsForecast();
                    break;
                case "toggleActiveAlertsDetailsButton":
                    ToggleActiveAlertsDetails();
                    break;
                case "toggleActiveAlertsGroupByBoxButton":
                    ToggleActiveAlertsGroupByBox();
                    break;
                case "traceSessionsSessionButton":
                    TraceSessionsSession();
                    break;
                case "killSessionsSessionButton":
                    KillSessionsSession();
                    break;
                case "filterSessionsButton":
                    ShowSessionsDetailsFilterDialog();
                    break;
                case "showActiveSessionsOnlyButton":
                    ToggleSessionsDetailsActiveFilter();
                    break;
                case "showUserSessionsOnlyButton":
                    ToggleSessionsDetailsUserFilter();
                    break;
                case "showBlockedSessionsButton":
                    ToggleSessionsDetailsBlockedFilter();
                    break;
                case "toggleSessionsDetailsDetailsPanelButton":
                    ToggleSessionsDetailsDetailsPanel();
                    break;
                case "toggleSessionsDetailsGroupByBoxButton":
                    ToggleSessionsDetailsGroupByBox();
                    break;
                case "traceSessionsLocksSessionButton":
                    TraceSessionsLocksSession();
                    break;
                case "killSessionsLocksSessionButton":
                    KillSessionsLocksSession();
                    break;
                case "filterLocksButton":
                    ShowSessionsLocksFilterDialog();
                    break;
                case "showBlockedLocksOnlyButton":
                    ToggleSessionsLocksBlockedFilter();
                    break;
                case "showBlockingLocksOnlyButton":
                    ToggleSessionsLocksBlockingFilter();
                    break;
                case "toggleSessionsLocksChartButton":
                    ToggleSessionsLocksChart();
                    break;
                case "toggleSessionsLocksGroupByBoxButton":
                    ToggleSessionsLocksGroupByBox();
                    break;
                case "traceSessionsBlockingSessionButton":
                    TraceSessionsBlockingSession();
                    break;
                case "killSessionsBlockingSessionButton":
                    KillSessionsBlockingSession();
                    break;
                case "toggleSessionsBlockingChartButton":
                    ToggleSessionsBlockingChart();
                    break;
                case "showSessionsBlockingByLockButton":
                    ShowSessionsBlockingType(SessionsBlockingView.BlockingType.Locks);
                    break;
                case "showSessionsBlockingBySessionButton":
                    ShowSessionsBlockingType(SessionsBlockingView.BlockingType.Sessions);
                    break;
                case "queriesTabSignatureModeViewButton":
                    ShowQueryMonitorView(ServerViews.QueriesSignatures);
                    break;
                case "queriesTabStatementModeViewButton":
                    ShowQueryMonitorView(ServerViews.QueriesStatements);
                    break;
                case "queriesTabHistoryModeViewButton":
                    ShowQueryMonitorView(ServerViews.QueriesHistory);
                    break;
                case "filterQueryMonitorButton":
                    ShowQueryMonitorFilterOptions();
                    break;
                case "toggleQueryMonitorFiltersButton":
                    ToggleQueryMonitorFilters();
                    break;
                case "toggleQueryMonitorGridButton":
                    ToggleQueryMonitorGrid();
                    break;
                case "toggleQueryMonitorGroupByBoxButton":
                    ToggleQueryMonitorGroupByBox();
                    break;
                case "showQueryMonitorSqlStatementsButton":
                    ToggleQueryMonitorSqlStatementsFilter();
                    break;
                case "showQueryMonitorStoredProceduresButton":
                    ToggleQueryMonitorStoredProceduresFilter();
                    break;
                case "showQueryMonitorSqlBatchesButton":
                    ToggleQueryMonitorSqlBatchesFilter();
                    break;
                case "showQueryMonitorCurrentQueriesButton":
                    ToggleQueryMonitorCurrentQueriesFilter();
                    break;
                case "configureQueryMonitorButton":
                    ShowQueryMonitorConfigurationProperties();
                    break;
                case "filterLogsButton":
                    ShowLogsFilterDialog();
                    break;
                case "filterLogsShowErrorsButton":
                    ToggleLogsShowErrorsFilter();
                    break;
                case "filterLogsShowWarningsButton":
                    ToggleLogsShowWarningsFilter();
                    break;
                case "filterLogsShowInformationalButton":
                    ToggleLogsShowInformationalFilter();
                    break;
                case "searchLogsButton":
                    ShowLogsSearchDialog();
                    break;
                case "cycleServerLogButton":
                    CycleLogs();
                    break;
                case "configureServerLogButton":
                    ConfigureLogs();
                    break;
                case "toggleAvailableLogsButton":
                    ToggleLogsViewAvailableLogs();
                    break;
                case "toggleLogDetailsButton":
                    ToggleLogsViewDetails();
                    break;
                case "toggleLogGroupByBoxButton":
                    ToggleLogsViewGroupByBox();
                    break;
                case "resourcesTabSummaryViewButton":
                    ShowResourcesSummaryView();
                    break;
                case "resourcesTabCpuViewButton":
                    ShowResourcesCpuView();
                    break;
                case "resourcesTabMemoryViewButton":
                    ShowResourcesMemoryView();
                    break;
                case "resourcesTabDiskViewButton":
                    ShowResourcesDiskView();
                    break;
                case "resourcesTabDiskSizeViewButton":         // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
                    ShowResourcesDiskSizeView();
                    break;
                case "resourcesTabProcedureCacheViewButton":
                    ShowResourcesProcedureCacheView();
                    break;
                case "resourcesTabWaitStatsViewButton":
                    ShowResourcesWaitStatsView();
                    break;
                case "toggleWaitStatsGroupByBoxButton":
                    ToggleWaitStatsGroupByBox();
                    break;
                case "resourcesShowFileActivityFilterButton":
                    ToggleFileActivityFilter();
                    break;
                case "resourcesTabWaitStatsActiveViewButton":
                    ShowQueriesWaitStatsActiveView();
                    break;
                case "resourcesTabFileActivityViewButton":
                    ShowResourcesFileActivityView();
                    break;
                case "toggleProcedureCacheChartsButton":
                    ToggleProcedureCacheCharts();
                    break;
                case "toggleProcedureCacheGroupByBoxButton":
                    ToggleProcedureCacheGroupByBox();
                    break;
                case "clearProcedureCacheButton":
                    ClearProcedureCache();
                    break;
                case "databasesTabSummaryViewButton":
                    ShowDatabasesSummaryView(true);
                    break;
                case "databasesTabConfigurationViewButton":
                    ShowDatabasesConfigurationView(true);
                    break;
                case "databasesSummaryShowUserDatabasesOnlyButton":
                    ToggleDatabasesSummaryShowUserDatabasesOnly();
                    break;
                case "toggleDatabasesSummaryChartsButton":
                    ToggleDatabasesSummaryCharts();
                    break;
                case "toggleDatabasesSummaryGroupByBoxButton":
                    ToggleDatabasesSummaryGroupByBox();
                    break;
                case "databasesConfigurationShowUserDatabasesOnlyButton":
                    ToggleDatabasesConfigurationShowUserDatabasesOnly();
                    break;
                case "toggleDatabasesConfigurationGroupByBoxButton":
                    ToggleDatabasesConfigurationGroupByBox();
                    break;
                case "databasesTabBackupRestoreHistoryViewButton":
                    ShowDatabasesBackupRestoreHistoryView(true);
                    break;
                case "filterDatabasesBackupRestoreHistoryButton":
                    ShowDatabasesBackupRestoreHistoryFilterDialog();
                    break;
                case "databasesBackupRestoreHistoryShowUserDatabasesOnlyButton":
                    ToggleDatabasesBackupRestoreHistoryUsersFilter();
                    break;
                case "databasesBackupRestoreHistoryShowBackupsButton":
                    ToggleDatabasesBackupRestoreHistoryBackupsFilter();
                    break;
                case "databasesBackupRestoreHistoryShowRestoresButton":
                    ToggleDatabasesBackupRestoreHistoryRestoresFilter();
                    break;
                case "toggleDatabasesBackupRestoreHistoryDatabasesGroupByBoxButton":
                    ToggleDatabasesBackupRestoreHistoryDatabasesGroupByBox();
                    break;
                case "toggleDatabasesBackupRestoreHistoryGroupByBoxButton":
                    ToggleDatabasesBackupRestoreHistoryHistoryGroupByBox();
                    break;
                case "databasesTabTablesIndexesViewButton":
                    ShowDatabasesTablesIndexesView(true);
                    break;
                case "databasesTabFilesViewButton":
                    ShowDatabasesFilesView(true);
                    break;
                case "databasesFilesShowUserDatabasesOnlyButton":
                    ToggleDatabasesFilesUsersFilter();
                    break;
                case "toggleDatabasesFilesGroupByBoxButton":
                    ToggleDatabasesFilesGroupByBox();
                    break;
                case "databasesTablesShowUserDatabasesOnlyButton":
                    ToggleDatabasesTablesShowUserDatabasesOnly();
                    break;
                case "databasesTabTempdbSummaryViewButton":
                    ShowDatabasesTempdbView(true);
                    break;
                case "databasesTablesShowUserTablesOnlyButton":
                    ToggleDatabasesTablesShowUserTablesOnly();
                    break;
                case "toggleDatabasesTablesTableDetailsButton":
                    ToggleDatabasesTablesTableDetails();
                    break;
                case "toggleDatabasesTablesGroupByBoxButton":
                    ToggleDatabasesTablesGroupByBox();
                    break;
                case "updateTableStatisticsButton":
                    UpdateTableStatistics();
                    break;
                case "rebuildTableIndexesButton":
                    RebuildTableIndexes();
                    break;
                case "servicesTabSummaryViewButton":
                    ShowServicesSummaryView();
                    break;
                case "startServicesServiceButton":
                    StartServicesSummaryService();
                    break;
                case "stopServicesServiceButton":
                    StopServicesSummaryService();
                    break;
                case "toggleServicesSummaryGroupByBoxVisibleButton":
                    ToggleServicesSummaryGroupByBox();
                    break;
                case "toggleServicesSummaryChartsButton":
                    ToggleServicesSummaryChart();
                    break;
                case "servicesTabSqlAgentJobsViewButton":
                    ShowServicesSqlAgentJobsView();
                    break;
                case "startServicesAgentButton":
                    StartServicesAgentService();
                    break;
                case "stopServicesAgentButton":
                    StopServicesAgentService();
                    break;
                case "startAgentJobButton":
                    StartAgentJob();
                    break;
                case "stopAgentJobButton":
                    StopAgentJob();
                    break;
                case "filterAgentJobsButton":
                    ShowServicesAgentJobsFilterDialog();
                    break;
                case "showAgentJobsAllButton":
                    ToggleServicesAgentJobsShowAllFilter();
                    break;
                case "showAgentJobsRunningOnlyButton":
                    ToggleServicesAgentJobsShowRunningOnlyFilter();
                    break;
                case "showAgentJobsFailedOnlyButton":
                    ToggleServicesAgentJobsShowFailedOnlyFilter();
                    break;
                case "toggleServicesAgentJobsHistoryVisibleButton":
                    ToggleServicesAgentJobsHistoryPanel();
                    break;
                case "toggleServicesAgentJobsGroupByBoxVisibleButton":
                    ToggleServicesAgentJobsGroupByBox();
                    break;
                case "servicesTabFullTextSearchViewButton":
                    ShowServicesFullTextSearchView();
                    break;
                case "startServicesFullTextSearchButton":
                    StartServicesFullTextSearchService();
                    break;
                case "stopServicesFullTextSearchButton":
                    StopServicesFullTextSearchService();
                    break;
                case "optimizeFullTextSearchCatalogButton":
                    OptimizeServicesFullTextSearchCatalog();
                    break;
                case "repopulateFullTextSearchCatalogButton":
                    RepopulateServicesSummaryService();
                    break;
                case "rebuildFullTextSearchCatalogButton":
                    RebuildServicesSummaryService();
                    break;
                case "toggleServicesFullTextSearchDetailsButton":
                    ToggleServicesFullTextSearchDetailsVisible();
                    break;
                case "toggleServicesFullTextSearchGroupByBoxVisibleButton":
                    ToggleServicesFullTextSearchGroupByBox();
                    break;
                case "servicesTabReplicationViewButton":
                    ShowServicesReplicationView();
                    break;
                case "filterReplicationButton":
                    ShowServicesReplicationFilterDialog();
                    break;
                case "toggleServicesReplicationTopologyGroupByBoxVisibleButton":
                    ToggleServicesReplicationTopologyGroupByBox();
                    break;
                case "toggleServicesReplicationDistributorGroupByBoxVisibleButton":
                    ToggleServicesReplicationDistributorGroupByBox();
                    break;
                case "toggleServicesReplicationReplicationGraphsVisibleButton":
                    ToggleServicesReplicationGraphsVisible();
                    break;
                case "detailsViewShowAllCountersButton":
                    ToggleOverviewDetailsShowAllFilter();
                    break;
                case "detailsViewShowCustomCountersButton":
                    ToggleOverviewDetailsShowCustomOnlyFilter();
                    break;
                case "toggleDatabasesMirroredDatabasesGroupByBoxButton":
                    ToggleDatabasesMirroredDatabasesGroupByBox();
                    break;
                case "toggleDatabaseMirroringHistoryGroupByBoxButton":
                    ToggleDatabasesMirroringHistoryGroupByBox();
                    break;
                case "databasesTabMirroringViewButton":
                    ShowDatabasesMirroringView(true);
                    break;
                case "databasesTabMirroringViewFailOverButton":
                    MirroringFailover();
                    break;
                case "databasesTabMirroringViewPauseButton":
                    MirroringSuspendResume();
                    break;
                case "resourcesQueryWaitsConfigureTopXButton":
                        ShowResourcesQueryWaitsFilterDialog();
                        break;
                case "resourcesQueryWaitsConfigureButton":
                        ShowResourcesQueryWaitsConfigureDialog();
                        break;
                case "filterDatabasesTempdbSessionsButton":
                        ShowDatabasesTempdbFilterDialog();
                        break;
                case "toggleDatabasesTempdbGroupByBoxButton":
                        ToggleDatabasesTempdbGroupByBox();
                        break;

                // News Feed functions
                case "pulseProfileButton":
                    ShowPulseProfile();
                    break;
                case "pulseFollowButton":
                    TogglePulseFollow();
                    break;
                case "pulsePostButton":
                    ShowPulsePost();
                    break;
            }
        }

        private void ToggleOverviewDetailsShowCustomOnlyFilter()
        {
            if (serverDetailsView != null)
            {
                serverDetailsView.Filter = ServerDetailsView.Selection.Custom;
            }
        }

        private void ToggleOverviewDetailsShowAllFilter()
        {
            if (serverDetailsView != null)
            {
                serverDetailsView.Filter = ServerDetailsView.Selection.All;
            }
        }

        private void ShowView(ServerBaseView view)
        {
            ShowView(view, false);
        }

        private void ShowView(ServerBaseView view, bool handleActiveViewFilter)
        {
            if (view != null && view != activeView)
            {
                if (activeView != null)
                {
                    activeView.CancelRefresh();
                    activeView.SaveSettings();
                    activeView.Dock = DockStyle.None;
                    activeView.Visible = false;
                }

                if (handleActiveViewFilter && activeView is IDatabasesView)
                {
                    view.SetArgument(((IDatabasesView)activeView).SelectedDatabaseFilter);
                }

                subViewContainerPanel.SuspendLayout();
                ApplicationController.Default.ClearCustomStatus();
                view.Size = subViewContainerPanel.Size;
                view.Dock = DockStyle.Fill;
                
                if (!subViewContainerPanel.Controls.Contains(view))
                {
                    subViewContainerPanel.Controls.Add(view);
                }

                view.UpdateUserTokenAttributes();
                view.BringToFront();
                view.Visible = true;
                activeView = view;

                subViewContainerPanel.ResumeLayout();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void OnSubViewChanged(SubViewChangedEventArgs e)
        {
            if (SubViewChanged != null)
            {
                SubViewChanged(this, e);
            }
        }

        private delegate void SetSelectedTabDelegate(RibbonTab tab);

        private void SetSelectedTab(RibbonTab tab)
        {
            if (ignoreTabSelect)
            {
                return;
            }

            switch ((ServerViewTabs)tab.Index)
            {
                case ServerViewTabs.Overview:
                    ShowSelectedOverviewView();
                    break;
                case ServerViewTabs.Sessions:
                    ShowSelectedSessionsView();
                    break;
                case ServerViewTabs.Queries:
                    ShowSelectedQueriesView();
                    break;
                case ServerViewTabs.Resources:
                    ShowSelectedResourcesView();
                    break;
                case ServerViewTabs.Databases:
                    ShowSelectedDatabasesView();
                    break;
                case ServerViewTabs.Services:
                    ShowSelectedServicesView();
                    break;
                case ServerViewTabs.Logs:
                    ShowLogsView();
                    break;
                //10.0 srishti purohit -- for doctors UI view 
                case ServerViewTabs.Analysis:
                    ShowServerRunAnalysisView(false);
                    break;
            }

            toolbarsManager.ResetAutoGenerateKeyTips();
        }

        private void toolbarsManager_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        {
            SetSelectedTabDelegate post = SetSelectedTab;
            BeginInvoke(post, e.Tab);
        }

        public void Initialize(int id, ServerView parentView)
        {
            instanceId = id;
            this.parentView = parentView;
            
            if (parentView != null)
            {
                parentView.HistoryBrowserVisibleChanged += parentView_HistoryBrowserVisibleChanged;
            }

            ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;

            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[id];
            if (wrapper != null)
            {
                wrapper.Changed += OnInstanceChanged;
                UpdateMaintenanceModeButton(wrapper);
                UpdateMirroringAndTempdbViewButton(wrapper);
                UpdateWaitsViewButtons(wrapper);
                UpdateWaitsActiveViewButton(wrapper);
                UpdateStatus();
            }
        }

        private void parentView_HistoryBrowserVisibleChanged(object sender, EventArgs e)
        {
            UpdateHistoryGroupOptions(true);
        }

        private void BackgroundRefreshCompleted( object sender, BackgroundRefreshCompleteEventArgs e )
        {
            if( !ApplicationModel.Default.ActiveInstances.Contains( this.instanceId ) )
                return;

            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[this.instanceId];

            if( wrapper == null )
                return;

            UpdateMaintenanceModeButton( wrapper );
            UpdateMirroringAndTempdbViewButton( wrapper );
            UpdateWaitsViewButtons( wrapper );
            UpdateWaitsActiveViewButton(wrapper);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            MonitoredSqlServerStatus status = null;
            if (ActiveView == null || ActiveView.HistoricalSnapshotDateTime == null)
                status = ApplicationModel.Default.GetInstanceStatus(instanceId);
            else
                status = serverSummaryhistoryData.HistoricalSnapshotStatus;
            UpdateStatus(status);
        }

        void OnInstanceChanged(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            UpdateMaintenanceModeButton(e.Instance);
            UpdateMirroringAndTempdbViewButton(e.Instance);
            UpdateWaitsViewButtons(e.Instance);
            UpdateWaitsActiveViewButton(e.Instance);
            UpdateStatus();
        }

        public void CancelRefresh()
        {
            if (activeView != null)
            {
                activeView.CancelRefresh();
            }
        }

        public void RefreshView()
        {
            if (parentView != null && activeView != null)
            {
                switch (parentView.ViewMode)
                {
                    case ServerViewMode.RealTime:
                        activeView.HistoricalSnapshotDateTime = null;
                        ApplicationController.Default.SetRefreshOptionsAvailability(true);
                        UpdateHistoryGroupOptions(true);
                        break;
                    case ServerViewMode.Historical:
                        ApplicationController.Default.SetRefreshOptionsAvailability(false);

                        if (activeView.HistoricalSnapshotDateTime != parentView.HistoricalSnapshotDateTime)
                        {
                            activeView.HistoricalSnapshotDateTime = parentView.HistoricalSnapshotDateTime;
                            UpdateHistoryGroupOptions(false);
                        }
                        break;
                }

                activeView.RefreshView();
            }
        }

        public void UpdateUserTokenAttributes()
        {
            switch (SelectedTab)
            {
                case ServerViewTabs.Overview:
                    UpdateOverviewGroups();
                    break;
                case ServerViewTabs.Sessions:
                    UpdateSessionsGroups();
                    break;
                case ServerViewTabs.Queries:
                    UpdateQueriesGroups();
                    break;
                case ServerViewTabs.Resources:
                    UpdateResourcesGroups();
                    break;
                case ServerViewTabs.Databases:
                    UpdateDatabasesGroups();
                    break;
                case ServerViewTabs.Services:
                    UpdateServicesGroups();
                    break;
                case ServerViewTabs.Logs:
                    UpdateLogsGroups();
                    break;
            }

            if (activeView != null)
            {
                activeView.UpdateUserTokenAttributes();
            }
        }

        private void SetCheckboxImage(ButtonTool tool, bool showChecked)
        {
            if (showChecked)
            {
                tool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
            }
            else
            {
                tool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
            }
        }

        private void SetRadioButtonImage(ButtonTool tool, bool showChecked)
        {
            if (showChecked)
            {
                tool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonRadioButtonChecked;
            }
            else
            {
                tool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.RibbonRadioButtonUnchecked;
            }
        }

        private void toolbarsManager_BeforeToolbarListDropdown(object sender, BeforeToolbarListDropdownEventArgs e)
        {
            e.ShowQuickAccessToolbarAddRemoveMenuItem = false;
            e.ShowQuickAccessToolbarPositionMenuItem = false;
        }

        public void SetActive(bool isActive)
        {
          
        }

        private void ToggleHistoryBrowser()
        {
            if (parentView != null)
            {
                parentView.HistoryBrowserVisible = !parentView.HistoryBrowserVisible;
            }
        }

        public void UpdateHistoryGroupOptions(bool enableNavigation)
        {
            if (parentView != null)
            {
                toolbarsManager.Tools["toggleHistoryBrowserButton"].SharedProps.Enabled = true;
                ((StateButtonTool)toolbarsManager.Tools["toggleHistoryBrowserButton"]).InitializeChecked(
                    parentView.HistoryBrowserVisible);
                toolbarsManager.Tools["showPreviousSnapshotButton"].SharedProps.Enabled = enableNavigation;
                toolbarsManager.Tools["showNextSnapshotButton"].SharedProps.Enabled = enableNavigation && parentView.ViewMode != ServerViewMode.RealTime;
            }
            else
            {
                toolbarsManager.Tools["toggleHistoryBrowserButton"].SharedProps.Enabled = false;
                toolbarsManager.Tools["showPreviousSnapshotButton"].SharedProps.Enabled = false;
                toolbarsManager.Tools["showNextSnapshotButton"].SharedProps.Enabled = false;
            }
        }

        private void ShowPreviousHistoricalSnapshot()
        {
            if (parentView != null)
            {
                parentView.ShowPreviousHistoricalSnapshot();
            }
        }

        private void ShowNextHistoricalSnapshot()
        {
            if (parentView != null)
            {
                parentView.ShowNextHistoricalSnapshot();
            }
        }

        #region Overview Tab

        private void ShowSelectedOverviewView()
        {
            if (toolbarsManager.OptionSets["overviewTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["overviewTabViewOptionSet"].SelectedTool.Key)
                {
                    case "overviewTabSummaryViewButton":
                        ShowServerSummaryView();
                        break;
                    case "overviewTabDetailsViewButton":
                        ShowServerDetailsView();
                        break;
                    case "overviewTabConfigurationViewButton":
                        ShowServerConfigurationView();
                        break;
                    case "overviewTabAlertsViewButton":
                        ShowServerAlertsView();
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["overviewTabSummaryViewButton"]).Checked = true;
            }
        }

        private void UpdateOverviewGroups()
        {
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabSummaryViewActionsGroup"].Visible =
                activeView == serverSummaryView && !serverSummaryView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabSummaryViewHistoryGroup"].Visible =
                activeView == serverSummaryView || activeView == serverDetailsView || activeView == serverActiveAlertsView;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabSummaryViewOptionsGroup"].Visible = false;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabDetailsViewShowHideGroup"].Visible = activeView == serverDetailsView;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabActiveAlertsShowHideGroup"].Visible = activeView == serverActiveAlertsView;
            toolbarsManager.Tools["toggleActiveAlertsForecastButton"].SharedProps.Visible =
                activeView == serverActiveAlertsView && !serverActiveAlertsView.HistoricalSnapshotDateTime.HasValue;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabConfigurationViewShowHideGroup"].Visible = activeView == serverConfigurationView;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabDetailsViewFilterGroup"].Visible = activeView == serverDetailsView;
            toolbarsManager.Ribbon.Tabs["Overview"].Groups["overviewTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        #region Overview Summary View

        private void ShowServerSummaryView()
        {
            if (serverSummaryView == null)
            {
                serverSummaryView = new ServerSummaryView4(instanceId, serverSummaryhistoryData);
                serverSummaryView.HistoricalSnapshotDateTimeChanged += new EventHandler(serverSummaryView_HistoricalSnapshotDateTimeChanged);
                UpdateServerSummaryViewShowAnimationsButton();
            }

            ShowView(serverSummaryView);
            UpdateOverviewGroups();
            UpdateHistoryGroupOptions(true);
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewSummary, ServerViewTabs.Overview));
        }

        private void serverSummaryView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateOverviewGroups();
        }

        private void ToggleServerSummaryViewShowAnimations()
        {
            Settings.Default.ShowServerSummaryAnimations = !Settings.Default.ShowServerSummaryAnimations;
        }

        private void UpdateServerSummaryViewShowAnimationsButton()
        {
            SetCheckboxImage((ButtonTool) toolbarsManager.Tools["toggleServerSummaryViewShowAnimationsButton"],
                             Settings.Default.ShowServerSummaryAnimations);
        }

        private void StopSqlServer()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            managementService.SendShutdownSQLServer(new ShutdownSQLServerConfiguration(instanceId, true));
        }

        private void UpdateMaintenanceModeButton()
        {
            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
            if (wrapper != null)
            {
                UpdateMaintenanceModeButton(wrapper);   
            }
        }
        /// <summary>
        /// Show or hide the mirroring view and tempdb view navigation buttons depending on the sql server version
        /// </summary>
        /// <param name="instance"></param>
        private void UpdateMirroringAndTempdbViewButton(MonitoredSqlServer instance)
        {
            if (instance == null) return;

            bool boolInstance2005AndUp = false;
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);
            
            if (status != null)
            {
                if (status.InstanceVersion != null)
                {
                    if (status.InstanceVersion.Major > 8)
                    {
                        boolInstance2005AndUp = true;
                    }
                }
            }
            ((StateButtonTool) toolbarsManager.Tools["databasesTabMirroringViewButton"]).SharedProps.Visible =
                boolInstance2005AndUp;
            ((StateButtonTool)toolbarsManager.Tools["databasesTabTempdbSummaryViewButton"]).SharedProps.Visible =
                boolInstance2005AndUp;
        }
      

        private void UpdateWaitsViewButtons( MonitoredSqlServer instance )
        {
            if( instance == null )
                return;

            bool boolServerSupportsWaitsMonitoring = false;
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);
            
            if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major > 8)
                boolServerSupportsWaitsMonitoring = true;

            ((StateButtonTool) toolbarsManager.Tools["resourcesTabWaitStatsViewButton"]).SharedProps.Visible       = boolServerSupportsWaitsMonitoring;
            //((StateButtonTool) toolbarsManager.Tools["resourcesTabWaitStatsActiveViewButton"]).SharedProps.Visible = boolServerSupportsWaitsMonitoring;
        }
        
        private void UpdateWaitsActiveViewButton(MonitoredSqlServer instance)
        {
            if (instance == null)
                return;

            bool boolServerSupportsWaitsMonitoring = false;
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);

            if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major > 8)
                boolServerSupportsWaitsMonitoring = true;

            ((StateButtonTool)toolbarsManager.Tools["resourcesTabWaitStatsActiveViewButton"]).SharedProps.Visible = boolServerSupportsWaitsMonitoring;
        }

        private void UpdateMaintenanceModeButton(MonitoredSqlServer instance)
        {
            if (instance != null)
            {
                ((StateButtonTool)toolbarsManager.Tools["maintenanceModeButton"]).InitializeChecked(instance.MaintenanceModeEnabled);
            }
        }

        private void ToggleMaintenanceMode()
        {
            MonitoredSqlServerInstancePropertiesDialog dialog =
            new MonitoredSqlServerInstancePropertiesDialog(instanceId);
            dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
            dialog.ShowDialog(this);

            try
            {
                ApplicationModel.Default.ConfigureMaintenanceMode(instanceId, dialog.Configuration.MaintenanceModeEnabled);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while changing maintenance mode status.", e);
                // on failure - update the state of the button to match reality
                UpdateMaintenanceModeButton();
            }
        }

        #endregion

        #region Overview Details View

        private void ShowServerDetailsView()
        {
            if (serverDetailsView == null)
            {
                serverDetailsView = new ServerDetailsView(instanceId);
                serverDetailsView.ChartPanelVisibleChanged += serverDetailsView_ChartPanelVisibleChanged;
                serverDetailsView.GridGroupByBoxVisibleChanged += serverDetailsView_GridGroupByBoxVisibleChanged;
                serverDetailsView.FilterChanged += serverDetailsView_FilterChanged;
                UpdateServerDetailsChartVisibleButton();
                UpdateServerDetailsPropertiesVisibleButton();
                UpdateServerDetailsGroupByBoxVisibleButton();
            }

            ShowView(serverDetailsView);
            UpdateOverviewGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewDetails, ServerViewTabs.Overview));
        }

        void serverDetailsView_FilterChanged(object sender, EventArgs e)
        {
            UpdateServerDetailsFilterButtons();
        }

        private void serverDetailsView_ChartPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateServerDetailsChartVisibleButton();
        }

        private void serverDetailsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServerDetailsGroupByBoxVisibleButton();
        }

        private void ToggleServerDetailsChart()
        {
            if (serverDetailsView != null)
            {
                serverDetailsView.ChartPanelVisible = !serverDetailsView.ChartPanelVisible;
            }
        }

        private void ToggleServerDetailsGroupByBox()
        {
            if (serverDetailsView != null)
            {
                serverDetailsView.GridGroupByBoxVisible = !serverDetailsView.GridGroupByBoxVisible;
            }
        }

        private void UpdateServerDetailsChartVisibleButton()
        {
            if (serverDetailsView != null)
            {
                if (serverDetailsView.ChartPanelVisible)
                {
                    toolbarsManager.Tools["toggleServerDetailsChartButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleServerDetailsChartButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateServerDetailsGroupByBoxVisibleButton()
        {
            if (serverDetailsView != null)
            {
                if (serverDetailsView.GridGroupByBoxVisible)
                {
                    toolbarsManager.Tools["toggleServerDetailsGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleServerDetailsGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        #endregion

        #region Overview Configuration View

        private void ShowServerConfigurationView()
        {
            if (serverConfigurationView == null)
            {
                serverConfigurationView = new ServerConfigurationView(instanceId);
                serverConfigurationView.DetailsPanelVisibleChanged += new EventHandler(serverConfigurationView_DetailsPanelVisibleChanged);
                serverConfigurationView.GridGroupByBoxVisibleChanged += new EventHandler(serverConfigurationView_GridGroupByBoxVisibleChanged);
                UpdateServerConfigurationDetailsVisibleButton();
                UpdateServerConfigurationGroupByBoxVisibleButton();
            }

            ShowView(serverConfigurationView);
            UpdateOverviewGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewConfiguration, ServerViewTabs.Overview));
        }

        private void UpdateServerConfigurationDetailsVisibleButton()
        {
            if (serverConfigurationView != null)
            {
                if (serverConfigurationView.DetailsPanelVisible)
                {
                    toolbarsManager.Tools["toggleServerConfigurationDetailsButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleServerConfigurationDetailsButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateServerDetailsPropertiesVisibleButton()
        {
            if (serverDetailsView != null)
            {
                if (serverDetailsView.PropertiesPanelVisible)
                {
                    toolbarsManager.Tools["toggleServerDetailsPropertiesButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleServerDetailsPropertiesButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateServerConfigurationGroupByBoxVisibleButton()
        {
            if (serverConfigurationView != null)
            {
                if (serverConfigurationView.GridGroupByBoxVisible)
                {
                    toolbarsManager.Tools["toggleServerConfigurationGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleServerConfigurationGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        void serverConfigurationView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServerConfigurationGroupByBoxVisibleButton();
        }

        private void serverConfigurationView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateServerConfigurationDetailsVisibleButton();
        }

        private void ToggleServerConfigurationDetails()
        {
            if (serverConfigurationView != null)
            {
                serverConfigurationView.DetailsPanelVisible = !serverConfigurationView.DetailsPanelVisible;
            }
        }

        private void ToggleServerDetailsProperties()
        {
            if (serverDetailsView != null)
            {
                serverDetailsView.PropertiesPanelVisible = !serverDetailsView.PropertiesPanelVisible;
                UpdateServerDetailsPropertiesVisibleButton();
            }
        }

        private void ToggleServerConfigurationGroupByBox()
        {
            if (serverConfigurationView != null)
            {
                serverConfigurationView.GridGroupByBoxVisible = !serverConfigurationView.GridGroupByBoxVisible;
            }
        }


        #endregion

        #region Overview Alerts View

        private void ShowServerAlertsView()
        {
            if (serverActiveAlertsView == null)
            {
                serverActiveAlertsView = new ActiveAlertsView(instanceId);
                serverActiveAlertsView.HistoricalSnapshotDateTimeChanged += new EventHandler(activeAlertsView_HistoricalSnapshotDateTimeChanged);
                serverActiveAlertsView.ForecastPanelVisibleChanged += new EventHandler(activeAlertsView_ForecastPanelVisibleChanged);
                serverActiveAlertsView.DetailsPanelVisibleChanged += new EventHandler(activeAlertsView_DetailsPanelVisibleChanged);
                serverActiveAlertsView.GridGroupByBoxVisibleChanged += new EventHandler(activeAlertsView_GridGroupByBoxVisibleChanged);
                UpdateActiveAlertsForecastVisibleButton();
                UpdateActiveAlertsDetailsVisibleButton();
                UpdateActiveAlertsGroupByBoxVisibleButton();
            }

            ShowView(serverActiveAlertsView);
            UpdateOverviewGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewAlerts, ServerViewTabs.Overview));
        }

        private void activeAlertsView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateOverviewGroups();
        }

        private void UpdateActiveAlertsForecastVisibleButton()
        {
            if (serverActiveAlertsView != null)
            {
                if (serverActiveAlertsView.ForecastPanelVisible)
                {
                    toolbarsManager.Tools["toggleActiveAlertsForecastButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleActiveAlertsForecastButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateActiveAlertsDetailsVisibleButton()
        {
            if (serverActiveAlertsView != null)
            {
                if (serverActiveAlertsView.DetailsPanelVisible)
                {
                    toolbarsManager.Tools["toggleActiveAlertsDetailsButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleActiveAlertsDetailsButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateActiveAlertsGroupByBoxVisibleButton()
        {
            if (serverActiveAlertsView != null)
            {
                if (serverActiveAlertsView.GridGroupByBoxVisible)
                {
                    toolbarsManager.Tools["toggleActiveAlertsGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleActiveAlertsGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void activeAlertsView_ForecastPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateActiveAlertsForecastVisibleButton();
        }

        private void activeAlertsView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateActiveAlertsDetailsVisibleButton();
        }

        void activeAlertsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateActiveAlertsGroupByBoxVisibleButton();
        }

        private void ToggleActiveAlertsForecast()
        {
            if (serverActiveAlertsView != null)
            {
                serverActiveAlertsView.ForecastPanelVisible = !serverActiveAlertsView.ForecastPanelVisible;
            }
        }

        private void ToggleActiveAlertsDetails()
        {
            if (serverActiveAlertsView != null)
            {
                serverActiveAlertsView.DetailsPanelVisible = !serverActiveAlertsView.DetailsPanelVisible;
            }
        }

        private void ToggleActiveAlertsGroupByBox()
        {
            if (serverActiveAlertsView != null)
            {
                serverActiveAlertsView.GridGroupByBoxVisible = !serverActiveAlertsView.GridGroupByBoxVisible;
            }
        }


        #endregion

        #endregion

        #region Sessions Tab

        private void ShowSelectedSessionsView()
        {
            if (toolbarsManager.OptionSets["sessionsTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["sessionsTabViewOptionSet"].SelectedTool.Key)
                {
                    case "sessionsTabSummaryViewButton":
                        ShowSessionsSummaryView();
                        break;
                    case "sessionsTabDetailsViewButton":
                        ShowSessionsDetailsView();
                        break;
                    case "sessionsTabLocksViewButton":
                        ShowSessionsLocksView();
                        break;
                    case "sessionsTabBlockingViewButton":
                        ShowSessionsBlockingView();
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["sessionsTabSummaryViewButton"]).Checked = true;
            }
        }

        #region Sessions Summary View

        private void UpdateSessionsGroups()
        {
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabHistoryGroup"].Visible = true;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabDetailsActionsGroup"].Visible =
                activeView == sessionsDetailsView && !sessionsDetailsView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabDetailsFilterGroup"].Visible = activeView == 
                                                                                                        sessionsDetailsView;
            toolbarsManager.Tools["showActiveSessionsOnlyButton"].SharedProps.Visible = 
                activeView == sessionsDetailsView && !sessionsDetailsView.HistoricalSnapshotDateTime.HasValue;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabDetailsShowHideGroup"].Visible = activeView ==
                                                                                                        sessionsDetailsView;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabLocksActionsGroup"].Visible =
                activeView == sessionsLocksView && !sessionsLocksView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabLocksFilterGroup"].Visible =
                activeView == sessionsLocksView && !sessionsLocksView.HistoricalSnapshotDateTime.HasValue;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabLocksShowHideGroup"].Visible = activeView ==
                                                                                                      sessionsLocksView;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabBlockingActionsGroup"].Visible =
                activeView == sessionsBlockingView && !sessionsBlockingView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabBlockingTypeGroup"].Visible = activeView ==
                                                                                                     sessionsBlockingView;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabBlockingShowHideGroup"].Visible = activeView ==
                                                                                                         sessionsBlockingView;
            toolbarsManager.Ribbon.Tabs["Sessions"].Groups["sessionsTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        private void ShowSessionsSummaryView()
        {
            if (sessionsSummaryView == null)
            {
                sessionsSummaryView = new SessionsSummaryView(instanceId, serverSummaryhistoryData);
            }

            ShowView(sessionsSummaryView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsSummary, ServerViewTabs.Sessions));
        }

        #endregion

        #region Sessions Details View

        private void ShowSessionsDetailsView()
        {
            if (sessionsDetailsView == null)
            {
                sessionsDetailsView = new SessionsDetailsView(instanceId);
                sessionsDetailsView.HistoricalSnapshotDateTimeChanged += new EventHandler(sessionsDetailsView_HistoricalSnapshotDateTimeChanged);
                sessionsDetailsView.DetailsPanelVisibleChanged += new EventHandler(sessionsDetailsView_DetailsPanelVisibleChanged);
                sessionsDetailsView.GridGroupByBoxVisibleChanged += new EventHandler(sessionsDetailsView_GridGroupByBoxVisibleChanged);
                sessionsDetailsView.FilterChanged += new EventHandler(sessionsDetailsView_FilterChanged);
                sessionsDetailsView.TraceAllowedChanged += new EventHandler(sessionsDetailsView_TraceEnabledChanged);
                sessionsDetailsView.KillAllowedChanged += new EventHandler(sessionsDetailsView_KillEnabledChanged);
            }

            ShowView(sessionsDetailsView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsDetails, ServerViewTabs.Sessions));
        }

        private void sessionsDetailsView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsDetailsView_TraceEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsDetailsTraceButton();
        }

        private void sessionsDetailsView_KillEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsDetailsKillButton();
        }

        private void sessionsDetailsView_FilterChanged(object sender, EventArgs e)
        {
            UpdateSessionsDetailsFilterButtons();
        }

        private void sessionsDetailsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsDetailsGroupByBoxButton();
        }

        private void sessionsDetailsView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsDetailsDetailsButton();
        }

        private void TraceSessionsSession()
        {
            sessionsDetailsView.TraceSession();
        }

        private void KillSessionsSession()
        {
            sessionsDetailsView.KillSession();
        }

        private void ShowSessionsDetailsFilterDialog()
        {
            sessionsDetailsView.ShowFilter();
        }

        private void ToggleSessionsDetailsActiveFilter()
        {
            sessionsDetailsView.ToggleActiveFilter();
        }

        private void ToggleSessionsDetailsUserFilter()
        {
            sessionsDetailsView.ToggleUserFilter();
        }

        private void ToggleSessionsDetailsBlockedFilter()
        {
            sessionsDetailsView.ToggleBlockedFilter();
        }

        private void ToggleSessionsDetailsDetailsPanel()
        {
            sessionsDetailsView.ToggleDetailsPanel();
        }

        private void ToggleSessionsDetailsGroupByBox()
        {
            sessionsDetailsView.ToggleGroupByBox();
        }

        private void UpdateSessionsDetailsTraceButton()
        {
            if (sessionsDetailsView != null)
            {
                toolbarsManager.Tools["traceSessionsSessionButton"].SharedProps.Enabled = sessionsDetailsView.TraceAllowed;
            }
        }

        private void UpdateSessionsDetailsKillButton()
        {
            if (sessionsDetailsView != null)
            {
                toolbarsManager.Tools["killSessionsSessionButton"].SharedProps.Enabled = sessionsDetailsView.KillAllowed;
            }
        }

        private void UpdateSessionsDetailsFilterButtons()
        {
            if (sessionsDetailsView != null)
            {
                SessionsConfiguration config = sessionsDetailsView.Configuration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterSessionsButton"]).Checked = config.IsFiltered();
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showActiveSessionsOnlyButton"], config.Active);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showUserSessionsOnlyButton"], config.ExcludeSystemProcesses);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showBlockedSessionsButton"], config.Blocked);
                ignoreToolClick = false;
            }
        }

        private void UpdateSessionsDetailsDetailsButton()
        {
            if (sessionsDetailsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleSessionsDetailsDetailsPanelButton"],
                                        sessionsDetailsView.DetailsPanelVisible);
            }
        }

        private void UpdateSessionsDetailsGroupByBoxButton()
        {
            if (sessionsDetailsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleSessionsDetailsGroupByBoxButton"],
                                        sessionsDetailsView.GridGroupByBoxVisible);
            }
        }

        #endregion

        #region Sessions Locks View

        private void ShowSessionsLocksView()
        {
            if (sessionsLocksView == null)
            {
                sessionsLocksView = new SessionsLocksView(instanceId);
                sessionsLocksView.HistoricalSnapshotDateTimeChanged += new EventHandler(sessionsLocksView_HistoricalSnapshotDateTimeChanged);
                sessionsLocksView.FilterChanged += new EventHandler(sessionsLocksView_FilterChanged);
                sessionsLocksView.ChartVisibleChanged += new EventHandler(sessionsLocksView_ChartVisibleChanged);
                sessionsLocksView.GridGroupByBoxVisibleChanged += new EventHandler(sessionsLocksView_GridGroupByBoxVisibleChanged);
                sessionsLocksView.TraceAllowedChanged += new EventHandler(sessionsLocksView_TraceEnabledChanged);
                sessionsLocksView.KillAllowedChanged += new EventHandler(sessionsLocksView_KillEnabledChanged);
                UpdateSessionsLocksChartVisibleButton();
                UpdateSessionsLocksGroupByBoxVisibleButton();
            }

            ShowView(sessionsLocksView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsLocks, ServerViewTabs.Sessions));
        }

        private void sessionsLocksView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsLocksView_TraceEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsLocksTraceButton();
        }

        private void sessionsLocksView_KillEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsLocksKillButton();
        }

        private void sessionsLocksView_FilterChanged(object sender, EventArgs e)
        {
            UpdateSessionsLocksFilterButtons();
        }

        private void sessionsLocksView_ChartVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsLocksChartVisibleButton();
        }

        private void sessionsLocksView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsLocksGroupByBoxVisibleButton();
        }

        private void TraceSessionsLocksSession()
        {
            sessionsLocksView.TraceSession();
        }

        private void KillSessionsLocksSession()
        {
            sessionsLocksView.KillSession();
        }

        private void ShowSessionsLocksFilterDialog()
        {
            sessionsLocksView.ShowFilter();
        }

        private void ToggleSessionsLocksBlockedFilter()
        {
            sessionsLocksView.ToggleBlockedFilter();
        }

        private void ToggleSessionsLocksBlockingFilter()
        {
            sessionsLocksView.ToggleBlockingFilter();
        }

        private void UpdateSessionsLocksTraceButton()
        {
            if (sessionsLocksView != null)
            {
                toolbarsManager.Tools["traceSessionsLocksSessionButton"].SharedProps.Enabled = sessionsLocksView.TraceAllowed;
            }
        }

        private void UpdateSessionsLocksKillButton()
        {
            if (sessionsLocksView != null)
            {
                toolbarsManager.Tools["killSessionsLocksSessionButton"].SharedProps.Enabled = sessionsLocksView.KillAllowed;
            }
        }

        private void UpdateSessionsLocksFilterButtons()
        {
            if (sessionsLocksView != null)
            {
                LockDetailsConfiguration config = sessionsLocksView.Configuration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterLocksButton"]).Checked = config.IsFiltered();
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showBlockedLocksOnlyButton"], config.FilterForBlocked);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showBlockingLocksOnlyButton"], config.FilterForBlocking);
                ignoreToolClick = false;
            }
        }

        private void UpdateSessionsLocksChartVisibleButton()
        {
            if (sessionsLocksView != null)
            {
                if (sessionsLocksView.ChartVisible)
                {
                    toolbarsManager.Tools["toggleSessionsLocksChartButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleSessionsLocksChartButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void UpdateSessionsLocksGroupByBoxVisibleButton()
        {
            if (sessionsLocksView != null)
            {
                if (sessionsLocksView.GridGroupByBoxVisible)
                {
                    toolbarsManager.Tools["toggleSessionsLocksGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxChecked;
                }
                else
                {
                    toolbarsManager.Tools["toggleSessionsLocksGroupByBoxButton"].SharedProps.AppearancesSmall.
                        Appearance.
                        Image = Properties.Resources.RibbonCheckboxUnchecked;
                }
            }
        }

        private void ToggleSessionsLocksChart()
        {
            if (sessionsLocksView != null)
            {
                sessionsLocksView.ChartVisible = !sessionsLocksView.ChartVisible;
            }
        }

        private void ToggleSessionsLocksGroupByBox()
        {
            if (sessionsLocksView != null)
            {
                sessionsLocksView.GridGroupByBoxVisible = !sessionsLocksView.GridGroupByBoxVisible;
            }
        }

        #endregion

        #region Sessions Blocking View

        private void ShowSessionsBlockingView()
        {
            if (sessionsBlockingView == null)
            {
                sessionsBlockingView = new SessionsBlockingView(instanceId);
                sessionsBlockingView.HistoricalSnapshotDateTimeChanged += new EventHandler(sessionsBlockingView_HistoricalSnapshotDateTimeChanged);
                sessionsBlockingView.ChartVisibleChanged += new EventHandler(sessionsBlockingView_ChartVisibleChanged);
                sessionsBlockingView.BlockingTypeChanged += new EventHandler(sessionsBlockingView_BlockingTypeChanged);
                sessionsBlockingView.TraceAllowedChanged += new EventHandler(sessionsBlockingView_TraceEnabledChanged);
                sessionsBlockingView.KillAllowedChanged += new EventHandler(sessionsBlockingView_KillEnabledChanged);
                UpdateSessionsBlockingChartVisibleButton();
                UpdateSessionsBlockingTypeButtons();
            }

            ShowView(sessionsBlockingView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsBlocking, ServerViewTabs.Sessions));
        }

        private void sessionsBlockingView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsBlockingView_TraceEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlockingTraceButton();
        }

        private void sessionsBlockingView_KillEnabledChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlockingKillButton();
        }

        private void sessionsBlockingView_ChartVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlockingChartVisibleButton();
        }

        private void sessionsBlockingView_BlockingTypeChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlockingTypeButtons();
        }

        private void TraceSessionsBlockingSession()
        {
            sessionsBlockingView.TraceSession();
        }

        private void KillSessionsBlockingSession()
        {
            sessionsBlockingView.KillSession();
        }

        private void UpdateSessionsBlockingChartVisibleButton()
        {
            if (sessionsBlockingView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleSessionsBlockingChartButton"], sessionsBlockingView.ChartVisible);
            }
        }

        private void UpdateSessionsBlockingTypeButtons()
        {
            if (sessionsBlockingView != null)
            {
                if (sessionsBlockingView.BlockingTypeShown == SessionsBlockingView.BlockingType.Locks)
                {
                    SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showSessionsBlockingByLockButton"], true);
                    SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showSessionsBlockingBySessionButton"], false);
                }
                else
                {
                    SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showSessionsBlockingByLockButton"], false);
                    SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showSessionsBlockingBySessionButton"], true);
                }
            }
        }

        private void UpdateSessionsBlockingTraceButton()
        {
            if (sessionsBlockingView != null)
            {
                toolbarsManager.Tools["traceSessionsBlockingSessionButton"].SharedProps.Enabled = sessionsBlockingView.TraceAllowed;
            }
        }

        private void UpdateSessionsBlockingKillButton()
        {
            if (sessionsBlockingView != null)
            {
                toolbarsManager.Tools["killSessionsBlockingSessionButton"].SharedProps.Enabled = sessionsBlockingView.KillAllowed;
            }
        }

        private void ToggleSessionsBlockingChart()
        {
            if (sessionsBlockingView != null)
            {
                sessionsBlockingView.ChartVisible = !sessionsBlockingView.ChartVisible;
            }
        }

        private void ShowSessionsBlockingType(SessionsBlockingView.BlockingType blockingType)
        {
            if (sessionsBlockingView != null)
            {
                sessionsBlockingView.BlockingTypeShown = blockingType;
            }
        }

        #endregion

        #endregion

        #region Query Monitor Tab

        private void ShowSelectedQueriesView()
        {
            if (toolbarsManager.OptionSets["queriesTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["queriesTabViewOptionSet"].SelectedTool.Key)
                {
                    case "queriesTabSignatureModeViewButton":
                        ShowQueryMonitorView(ServerViews.QueriesSignatures);
                        break;
                    case "queriesTabStatementModeViewButton":
                        ShowQueryMonitorView(ServerViews.QueriesStatements);
                        break;
                    case "queriesTabHistoryModeViewButton":
                        ShowQueryMonitorView(ServerViews.QueriesHistory);
                        break;
                    default:
                        ShowQueryMonitorView(ServerViews.Queries);
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["queriesTabSignatureModeViewButton"]).Checked = true;
            }
        }

        /// <summary>
        /// only show the properties group if the user has modify permission
        /// </summary>
        private void UpdateQueriesGroups()
        {
            toolbarsManager.Ribbon.Tabs["Queries"].Groups["queriesTabPropertiesGroup"].Visible =
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Queries"].Groups["queriesTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        /// <summary>
        /// Show one of the three QM views
        /// </summary>
        /// <param name="view"></param>
        private void ShowQueryMonitorView(ServerViews view)
        {
            if (queryMonitorView == null)
            {
                //create the view
                queryMonitorView = new QueryMonitorView(instanceId);
                //hook up the events
                queryMonitorView.FiltersVisibleChanged += new EventHandler(queryMonitorView_FiltersVisibleChanged);
                queryMonitorView.GridVisibleChanged += new EventHandler(queryMonitorView_GridVisibleChanged);
                queryMonitorView.GridGroupByBoxVisibleChanged += new EventHandler(queryMonitorView_GridGroupByBoxVisibleChanged);
                queryMonitorView.FilterChanged += new EventHandler(queryMonitorView_FilterChanged);
                
                //hide\show the ribbon groups
                UpdateQueryMonitorFilterButtons();
                UpdateQueryMonitorFiltersVisibleButton();
                UpdateQueryMonitorGridVisibleButton();
                UpdateQueryMonitorGroupByBoxVisibleButton();
            }
            //show the view
            ShowView(queryMonitorView);

            switch (view)
            {
                case ServerViews.QueriesSignatures:
                    queryMonitorView.ShowSignatureMode();
                    break;
                case ServerViews.QueriesStatements:
                    queryMonitorView.ShowStatementMode();
                    break;
                case ServerViews.QueriesHistory:
                    queryMonitorView.ShowHistoryMode();
                    break;
            }

            UpdateQueriesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(view, ServerViewTabs.Queries));
        }

        private void UpdateQueryMonitorFiltersVisibleButton()
        {
            if (queryMonitorView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleQueryMonitorFiltersButton"], queryMonitorView.FiltersVisible);
            }
        }

        private void UpdateQueryMonitorGridVisibleButton()
        {
            if (queryMonitorView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleQueryMonitorGridButton"], queryMonitorView.GridVisible);
            }
        }

        private void UpdateQueryMonitorGroupByBoxVisibleButton()
        {
            if (queryMonitorView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleQueryMonitorGroupByBoxButton"], queryMonitorView.GridGroupByBoxVisible);
            }
        }

        private void ToggleQueryMonitorFilters()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.FiltersVisible = !queryMonitorView.FiltersVisible;
            }
        }

        private void ToggleQueryMonitorGrid()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.GridVisible = !queryMonitorView.GridVisible;
            }
        }

        private void ToggleQueryMonitorGroupByBox()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.GridGroupByBoxVisible = !queryMonitorView.GridGroupByBoxVisible;
            }
        }

        private void queryMonitorView_FiltersVisibleChanged(object sender, EventArgs e)
        {
            UpdateQueryMonitorFiltersVisibleButton();
        }

        private void queryMonitorView_GridVisibleChanged(object sender, EventArgs e)
        {
            UpdateQueryMonitorGridVisibleButton();
        }

        private void queryMonitorView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateQueryMonitorGroupByBoxVisibleButton();
        }

        private void queryMonitorView_FilterChanged(object sender, EventArgs e)
        {
            UpdateQueryMonitorFilterButtons();
        }

        private void UpdateQueryMonitorFilterButtons()
        {
            if (queryMonitorView != null)
            {
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterQueryMonitorButton"]).Checked = queryMonitorView.Filter.IsFiltered();
                ignoreToolClick = false;
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showQueryMonitorSqlStatementsButton"], queryMonitorView.Filter.IncludeSqlStatements);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showQueryMonitorStoredProceduresButton"], queryMonitorView.Filter.IncludeStoredProcedures);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showQueryMonitorSqlBatchesButton"], queryMonitorView.Filter.IncludeSqlBatches);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showQueryMonitorCurrentQueriesButton"], queryMonitorView.Filter.IncludeOnlyResourceRows);
            }
        }

        private void ToggleQueryMonitorSqlStatementsFilter()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.SetSqlStatementFilter(!queryMonitorView.Filter.IncludeSqlStatements);
            }
        }

        private void ToggleQueryMonitorStoredProceduresFilter()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.SetStoredProceduresFilter(!queryMonitorView.Filter.IncludeStoredProcedures);
            }
        }

        private void ToggleQueryMonitorSqlBatchesFilter()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.SetSqlBatchesFilter(!queryMonitorView.Filter.IncludeSqlBatches);
            }
        }

        private void ToggleQueryMonitorCurrentQueriesFilter()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.SetRowsFilter(!queryMonitorView.Filter.IncludeOnlyResourceRows);
            }
        }

        private void ShowQueryMonitorFilterOptions()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.ShowFilter();
            }
        }

        private void ShowQueryMonitorConfigurationProperties()
        {
            if (queryMonitorView != null)
            {
                queryMonitorView.ShowConfigurationProperties();
            }
        }

        #endregion

        #region Resources Tab

        private void ShowSelectedResourcesView()
        {
            if (toolbarsManager.OptionSets["resourcesTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["resourcesTabViewOptionSet"].SelectedTool.Key)
                {
                    case "resourcesTabSummaryViewButton":
                        ShowResourcesSummaryView();
                        break;
                    case "resourcesTabCpuViewButton":
                        ShowResourcesCpuView();
                        break;
                    case "resourcesTabMemoryViewButton":
                        ShowResourcesMemoryView();
                        break;
                    case "resourcesTabDiskViewButton":
                        ShowResourcesDiskView();
                        break;
                    case "resourcesTabDiskSizeViewButton":   // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
                        ShowResourcesDiskSizeView();
                        break;
                    case "resourcesTabProcedureCacheViewButton":
                        ShowResourcesProcedureCacheView();
                        break;
                    case "resourcesTabWaitStatsViewButton":
                        ShowResourcesWaitStatsView();
                        break;
                    case "resourcesTabWaitStatsActiveViewButton":
                        ShowQueriesWaitStatsActiveView();
                        break;
                    case "resourcesTabFileActivityViewButton":
                        ShowResourcesFileActivityView();
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["resourcesTabSummaryViewButton"]).Checked = true;
            }
        }

        private void UpdateResourcesGroups()
        {
            UpdateHistoryGroupOptions(true);
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabHistoryGroup"].Visible = activeView != resourcesProcedureCacheView;
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabActionsGroup"].Visible = activeView == resourcesProcedureCacheView &&
                                                                                                  ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabProcedureCacheViewShowHideGroup"].Visible = activeView == resourcesProcedureCacheView;
            // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --show the Show/Hide group, if disk sixe view is selected
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabWaitStatsViewShowHideGroup"].Visible = activeView == resourcesWaitStatsView;
            //toolbarsManager.Ribbon.Tabs["Resources"].Groups["actionsRibbonGroup"].Visible = activeView == resourcesWaitStatsActiveView;
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabDiskSizeViewShowHideGroup"].Visible = activeView == resourcesDiskSizeView;
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesShowFileActivityFilterGroup"].Visible = activeView == resourcesFileActivityView;
            toolbarsManager.Ribbon.Tabs["Resources"].Groups["resourcesTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        private void ShowResourcesSummaryView()
        {
            if (resourcesSummaryView == null)
            {
                resourcesSummaryView = new ResourcesSummaryView(instanceId, serverSummaryhistoryData);
            }

            ShowView(resourcesSummaryView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesSummary, ServerViewTabs.Resources));
        }

        private void ShowResourcesCpuView()
        {
            if (resourcesCpuView == null)
            {
                resourcesCpuView = new ResourcesCpuView(instanceId, serverSummaryhistoryData);
            }

            ShowView(resourcesCpuView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesCpu, ServerViewTabs.Resources));
        }

        private void ShowResourcesMemoryView()
        {
            if (resourcesMemoryView == null)
            {
                resourcesMemoryView = new ResourcesMemoryView(instanceId, serverSummaryhistoryData);
            }

            ShowView(resourcesMemoryView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesMemory, ServerViewTabs.Resources));
        }

        private void ShowResourcesDiskView()
        {
            if (resourcesDiskView == null)
            {
                resourcesDiskView = new ResourcesDiskView(instanceId, serverSummaryhistoryData);
            }

            ShowView(resourcesDiskView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesDisk, ServerViewTabs.Resources));
        }

        // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --show the data of disk size view
        private void ShowResourcesDiskSizeView()
        {
            if (resourcesDiskSizeView == null)
            {
                resourcesDiskSizeView = new ResourcesDiskSizeView(instanceId);
            }

            ShowView(resourcesDiskSizeView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesDiskSize, ServerViewTabs.Resources));
        }
        // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --show the data of disk size view

        private void ShowResourcesProcedureCacheView()
        {
            if (resourcesProcedureCacheView == null)
            {
                resourcesProcedureCacheView = new ResourcesProcedureCacheView(instanceId);
                resourcesProcedureCacheView.ChartsVisibleChanged += new EventHandler(resourcesProcedureCacheView_ChartsVisibleChanged);
                resourcesProcedureCacheView.GridGroupByBoxVisibleChanged += new EventHandler(resourcesProcedureCacheView_GridGroupByBoxVisibleChanged);
                UpdateProcedureCacheChartsVisibleButton();
                UpdateProcedureCacheGroupByBoxVisibleButton();
            }

            ShowView(resourcesProcedureCacheView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesProcedureCache, ServerViewTabs.Resources));
        }

        private void ShowResourcesWaitStatsView()
        {
            if (resourcesWaitStatsView == null)
            {
                resourcesWaitStatsView = new ResourcesWaitStats(instanceId, serverSummaryhistoryData);
                resourcesWaitStatsView.GridGroupByBoxVisibleChanged += new EventHandler(resourcesWaitStatsView_GridGroupByBoxVisibleChanged);
                UpdateWaitStatsGroupByBoxVisibleButton();
            }

            ShowView(resourcesWaitStatsView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesWaitStats, ServerViewTabs.Resources));
        }

        private void ShowQueriesWaitStatsActiveView()
        {
            if (queriesWaitStatsActiveView == null)
            {
                queriesWaitStatsActiveView = new ResourcesWaitStatsActive(instanceId, serverSummaryhistoryData);                
            }

            ShowView(queriesWaitStatsActiveView);
            UpdateQueriesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.QueryWaitStatsActive, ServerViewTabs.Queries));
        }

        private void ShowResourcesFileActivityView()
        {
            if (resourcesFileActivityView == null)
            {
                resourcesFileActivityView = new ResourcesFileActivity(instanceId);
                resourcesFileActivityView.FilterPaneVisibleChanged += new EventHandler(resourcesFileActivityView_FilterPaneVisibleChanged);
                UpdateResourcesFileActivityShowFilterButton();
            }

            ShowView(resourcesFileActivityView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesFileActivity, ServerViewTabs.Resources));
        }

        void resourcesProcedureCacheView_ChartsVisibleChanged(object sender, EventArgs e)
        {
            UpdateProcedureCacheChartsVisibleButton();
        }

        private void resourcesProcedureCacheView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateProcedureCacheGroupByBoxVisibleButton();
        }

        private void UpdateProcedureCacheChartsVisibleButton()
        {
            if (resourcesProcedureCacheView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleProcedureCacheChartsButton"], resourcesProcedureCacheView.ChartsVisible);
            }
        }

        private void UpdateProcedureCacheGroupByBoxVisibleButton()
        {
            if (resourcesProcedureCacheView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleProcedureCacheGroupByBoxButton"], resourcesProcedureCacheView.GridGroupByBoxVisible);
            }
        }

        private void ToggleProcedureCacheCharts()
        {
            if (resourcesProcedureCacheView != null)
            {
                resourcesProcedureCacheView.ChartsVisible = !resourcesProcedureCacheView.ChartsVisible;
            }
        }

        private void ToggleProcedureCacheGroupByBox()
        {
            if (resourcesProcedureCacheView != null)
            {
                resourcesProcedureCacheView.GridGroupByBoxVisible = !resourcesProcedureCacheView.GridGroupByBoxVisible;
            }
        }

        private void resourcesWaitStatsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateWaitStatsGroupByBoxVisibleButton();
        }

        private void resourcesFileActivityView_FilterPaneVisibleChanged(object sender, EventArgs e)
        {
            UpdateResourcesFileActivityShowFilterButton();
        }

        private void UpdateWaitStatsGroupByBoxVisibleButton()
        {
            if (resourcesWaitStatsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleWaitStatsGroupByBoxButton"], resourcesWaitStatsView.GridGroupByBoxVisible);
            }
        }

        private void UpdateResourcesFileActivityShowFilterButton()
        {
            if (resourcesFileActivityView != null)
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["resourcesShowFileActivityFilterButton"], resourcesFileActivityView.FilterPaneVisible);
        }

        private void ToggleWaitStatsGroupByBox()
        {
            if (resourcesWaitStatsView != null)
            {
                resourcesWaitStatsView.GridGroupByBoxVisible = !resourcesWaitStatsView.GridGroupByBoxVisible;
            }
        }

        private void ToggleFileActivityFilter()
        {
            if (resourcesFileActivityView != null)
                resourcesFileActivityView.FilterPaneVisible = !resourcesFileActivityView.FilterPaneVisible;
        }

        private void ClearProcedureCache()
        {
            if (resourcesProcedureCacheView != null)
            {
                resourcesProcedureCacheView.ClearCache();
            }
        }

        private void ShowResourcesQueryWaitsFilterDialog()
        {
            if( queriesWaitStatsActiveView != null )
            {
                queriesWaitStatsActiveView.ShowFilter();
            }
        }

        private void ShowResourcesQueryWaitsConfigureDialog()
        {
            if( queriesWaitStatsActiveView != null )
            {
                queriesWaitStatsActiveView.ShowConfigureDialog();
            }
        }

        #endregion

        #region Databases Tab

        private void ShowSelectedDatabasesView()
        {
            if (toolbarsManager.OptionSets["databasesTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["databasesTabViewOptionSet"].SelectedTool.Key)
                {
                    case "databasesTabSummaryViewButton":
                        ShowDatabasesSummaryView();
                        break;
                    case "databasesTabConfigurationViewButton":
                        ShowDatabasesConfigurationView();
                        break;
                    case "databasesTabBackupRestoreHistoryViewButton":
                        ShowDatabasesBackupRestoreHistoryView();
                        break;
                    case "databasesTabTablesIndexesViewButton":
                        ShowDatabasesTablesIndexesView();
                        break;
                    case "databasesTabFilesViewButton":
                        ShowDatabasesFilesView();
                        break;
                    case "databasesTabMirroringViewButton":
                        ShowDatabasesMirroringView();
                        break;
                    case "databasesTabTempdbSummaryViewButton":
                        ShowDatabasesTempdbView();
                        break;
                    default:
                        ((StateButtonTool)toolbarsManager.Tools["databasesTabSummaryViewButton"]).Checked = true;
                        break;

                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["databasesTabSummaryViewButton"]).Checked = true;
            }
        }

        private void UpdateDatabasesGroups()
        {
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabSummaryViewFilterGroup"].Visible = false;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabSummaryViewShowHideGroup"].Visible = activeView == databasesSummaryView;
			///SQLdm 10.0.0(Ankit Nagpal)
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabHistoryGroup"].Visible = activeView == databasesTempdbView || activeView == databasesAlwaysOnView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabTempdbViewFilterGroup"].Visible = activeView == databasesTempdbView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabTempdbViewShowHideGroup"].Visible = activeView == databasesTempdbView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabConfigurationViewFilterGroup"].Visible = false;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabConfigurationViewShowHideGroup"].Visible = activeView == databasesConfigurationView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabBackupRestoreHistoryViewFilterGroup"].Visible = activeView == databasesBackupRestoreHistoryView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabBackupRestoreHistoryViewShowHideGroup"].Visible = activeView == databasesBackupRestoreHistoryView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabFilesViewFilterGroup"].Visible = false;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabFilesViewShowHideGroup"].Visible = activeView == databasesFilesView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabTablesViewActionsGroup"].Visible = 
                activeView == databasesTablesIndexesView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabTablesViewFilterGroup"].Visible = activeView == databasesTablesIndexesView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabTablesViewShowHideGroup"].Visible = activeView == databasesTablesIndexesView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabMirroringViewShowHideGroup"].Visible = activeView == databasesMirroringView;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabMirroringViewActionsGroup"].Visible = 
                activeView == databasesMirroringView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Databases"].Groups["databasesTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        private void ShowDatabasesSummaryView()
        {
            ShowDatabasesSummaryView(false);
        }

        private void ShowDatabasesSummaryView(bool handleActiveViewFilter)
        {
            if (databasesSummaryView == null)
            {
                databasesSummaryView = new DatabasesSummaryView(instanceId);
                databasesSummaryView.FilterChanged += new EventHandler(databasesSummaryView_FilterChanged);
                databasesSummaryView.GridGroupByBoxVisibleChanged += new EventHandler(databasesSummaryView_GridGroupByBoxVisibleChanged);
                databasesSummaryView.ChartVisibleChanged += new EventHandler(databasesSummaryView_ChartVisibleChanged);
                UpdateDatabasesSummaryShowUserDatabasesOnlyButton();
                UpdateDatabasesSummaryGroupByBoxVisibleButton();
                UpdateDatabasesSummaryChartVisibleButton();
            }

            ShowView(databasesSummaryView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesSummary, ServerViewTabs.Databases));
        }

        private void databasesSummaryView_FilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesSummaryShowUserDatabasesOnlyButton();
        }

        private void databasesSummaryView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesSummaryGroupByBoxVisibleButton();
        }

        private void databasesSummaryView_ChartVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesSummaryChartVisibleButton();
        }

        private void UpdateDatabasesSummaryShowUserDatabasesOnlyButton()
        {
            if (databasesSummaryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesSummaryShowUserDatabasesOnlyButton"], !databasesSummaryView.Configuration.IncludeSystemDatabases);
            }
        }

        private void UpdateDatabasesSummaryGroupByBoxVisibleButton()
        {
            if (databasesSummaryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesSummaryGroupByBoxButton"], databasesSummaryView.GridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesSummaryChartVisibleButton()
        {
            if (databasesSummaryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesSummaryChartsButton"], databasesSummaryView.ChartVisible);
            }
        }

        private void ToggleDatabasesSummaryShowUserDatabasesOnly()
        {
            if (databasesSummaryView != null)
            {
                databasesSummaryView.ToggleUserFilter();
            }
        }

        private void ToggleDatabasesSummaryGroupByBox()
        {
            if (databasesSummaryView != null)
            {
                databasesSummaryView.ToggleGroupByBox();
            }
        }

        private void ToggleDatabasesSummaryCharts()
        {
            if (databasesSummaryView != null)
            {
                databasesSummaryView.ChartVisible = !databasesSummaryView.ChartVisible;
            }
        }

        private void ShowDatabasesTempdbView()
        {
            ShowDatabasesTempdbView(false);
        }

        private void ShowDatabasesTempdbView(bool handleActiveViewFilter)
        {
            if (databasesTempdbView == null)
            {
                databasesTempdbView = new DatabasesTempdbView(instanceId);
                databasesTempdbView.HistoricalSnapshotDateTimeChanged += new EventHandler(databasesTempdbView_HistoricalSnapshotDateTimeChanged);
                databasesTempdbView.GridGroupByBoxVisibleChanged += new EventHandler(databasesTempdbView_GridGroupByBoxVisibleChanged);
                databasesTempdbView.FilterChanged += new EventHandler(databasesTempdbView_FilterChanged);
            }

            ShowView(databasesTempdbView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesTempdbView, ServerViewTabs.Databases));
        }

        private void databasesTempdbView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateDatabasesGroups();
        }

        private void databasesTempdbView_FilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTempdbFilterButtons();
        }

        private void databasesTempdbView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTempdbGroupByBoxButton();
        }

        private void ShowDatabasesTempdbFilterDialog()
        {
            databasesTempdbView.ShowFilter();
        }

        private void ToggleDatabasesTempdbGroupByBox()
        {
            if (databasesTempdbView != null)
            {
                databasesTempdbView.ToggleGroupByBox();
            }
        }

        private void UpdateDatabasesTempdbFilterButtons()
        {
            if (databasesTempdbView != null)
            {
                SessionsConfiguration config = databasesTempdbView.SessionsConfiguration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterDatabasesTempdbSessionsButton"]).Checked = config.IsFiltered();
                //SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showActiveSessionsOnlyButton"], config.Active);
                //SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showUserSessionsOnlyButton"], config.ExcludeSystemProcesses);
                //SetCheckboxImage((ButtonTool)toolbarsManager.Tools["showBlockedSessionsButton"], config.Blocked);
                ignoreToolClick = false;
            }
        }

        private void UpdateDatabasesTempdbGroupByBoxButton()
        {
            if (databasesTempdbView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesTempdbGroupByBoxButton"],
                                        databasesTempdbView.GridGroupByBoxVisible);
            }
        }

        private void ShowDatabasesConfigurationView()
        {
            ShowDatabasesConfigurationView(false);
        }

        private void ShowDatabasesConfigurationView(bool handleActiveViewFilter)
        {
            if (databasesConfigurationView == null)
            {
                databasesConfigurationView = new DatabasesConfigurationView(instanceId);
                databasesConfigurationView.FilterChanged += new EventHandler(databasesConfigurationView_FilterChanged);
                databasesConfigurationView.GridGroupByBoxVisibleChanged += new EventHandler(databasesConfigurationView_GridGroupByBoxVisibleChanged);
                UpdateDatabasesConfigurationShowUserDatabasesOnlyButton();
                UpdateDatabasesConfigurationGroupByBoxVisibleButton();
            }

            ShowView(databasesConfigurationView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesConfiguration, ServerViewTabs.Databases));
        }

        void databasesConfigurationView_FilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesConfigurationShowUserDatabasesOnlyButton();
        }

        void databasesConfigurationView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesConfigurationGroupByBoxVisibleButton();
        }

        private void UpdateDatabasesConfigurationShowUserDatabasesOnlyButton()
        {
            if (databasesConfigurationView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesConfigurationShowUserDatabasesOnlyButton"], !databasesConfigurationView.IncludeSystemDatabases);
            }
        }

        private void UpdateDatabasesConfigurationGroupByBoxVisibleButton()
        {
            if (databasesConfigurationView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesConfigurationGroupByBoxButton"], databasesConfigurationView.GridGroupByBoxVisible);
            }
        }

        private void ToggleDatabasesConfigurationShowUserDatabasesOnly()
        {
            if (databasesConfigurationView != null)
            {
                databasesConfigurationView.IncludeSystemDatabases = !databasesConfigurationView.IncludeSystemDatabases;
            }
        }

        private void ToggleDatabasesConfigurationGroupByBox()
        {
            if (databasesConfigurationView != null)
            {
                databasesConfigurationView.GridGroupByBoxVisible = !databasesConfigurationView.GridGroupByBoxVisible;
            }
        }

        private void ShowDatabasesBackupRestoreHistoryView()
        {
            ShowDatabasesBackupRestoreHistoryView(false);
        }

        private void ShowDatabasesBackupRestoreHistoryView(bool handleActiveViewFilter)
        {
            if (databasesBackupRestoreHistoryView == null)
            {
                databasesBackupRestoreHistoryView = new DatabasesBackupRestoreHistoryView(instanceId);
                databasesBackupRestoreHistoryView.FilterChanged += new EventHandler(databasesBackupRestoreHistoryView_FilterChanged);
                databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisibleChanged += new EventHandler(databasesBackupRestoreHistoryView_DatabasesGridGroupByBoxVisibleChanged);
                databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisibleChanged += new EventHandler(databasesBackupRestoreHistoryView_HistoryGridGroupByBoxVisibleChanged);
                UpdateDatabasesBackupRestoreHistoryFilterButtons();
                UpdateDatabasesBackupRestoreHistoryDatabasesGroupByBoxVisibleButton();
                UpdateDatabasesBackupRestoreHistoryHistoryGroupByBoxVisibleButton();
            }

            ShowView(databasesBackupRestoreHistoryView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesBackupRestoreHistory, ServerViewTabs.Databases));
        }

        void databasesBackupRestoreHistoryView_FilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesBackupRestoreHistoryFilterButtons();
        }

        void databasesBackupRestoreHistoryView_DatabasesGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesBackupRestoreHistoryDatabasesGroupByBoxVisibleButton();
        }

        void databasesBackupRestoreHistoryView_HistoryGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesBackupRestoreHistoryHistoryGroupByBoxVisibleButton();
        }

        private void ShowDatabasesBackupRestoreHistoryFilterDialog()
        {
            databasesBackupRestoreHistoryView.ShowFilter();
        }

        private void UpdateDatabasesBackupRestoreHistoryFilterButtons()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                BackupRestoreHistoryConfiguration config = databasesBackupRestoreHistoryView.Configuration;
                DatabaseSummaryConfiguration configDb = databasesBackupRestoreHistoryView.DatabasesConfiguration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterDatabasesBackupRestoreHistoryButton"]).Checked = config.IsFiltered();
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesBackupRestoreHistoryShowUserDatabasesOnlyButton"], !configDb.IncludeSystemDatabases);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesBackupRestoreHistoryShowBackupsButton"], config.ShowBackups);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesBackupRestoreHistoryShowRestoresButton"], config.ShowRestores);
                ignoreToolClick = false;
            }
        }

        private void UpdateDatabasesBackupRestoreHistoryDatabasesGroupByBoxVisibleButton()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesBackupRestoreHistoryDatabasesGroupByBoxButton"], databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesBackupRestoreHistoryHistoryGroupByBoxVisibleButton()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesBackupRestoreHistoryGroupByBoxButton"], databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisible);
            }
        }

        private void ToggleDatabasesBackupRestoreHistoryUsersFilter()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                databasesBackupRestoreHistoryView.ToggleUserFilter();
            }
        }

        private void ToggleDatabasesBackupRestoreHistoryBackupsFilter()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                databasesBackupRestoreHistoryView.ToggleBackupsFilter();
            }
        }

        private void ToggleDatabasesBackupRestoreHistoryRestoresFilter()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                databasesBackupRestoreHistoryView.ToggleRestoresFilter();
            }
        }

        private void ToggleDatabasesBackupRestoreHistoryDatabasesGroupByBox()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                databasesBackupRestoreHistoryView.ToggleDatabasesGroupByBox();
            }
        }

        private void ToggleDatabasesBackupRestoreHistoryHistoryGroupByBox()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                databasesBackupRestoreHistoryView.ToggleHistoryGroupByBox();
            }
        }

        private void ShowDatabasesFilesView()
        {
            ShowDatabasesFilesView(false);
        }

        private void ShowDatabasesFilesView(bool handleActiveViewFilter)
        {
            if (databasesFilesView == null)
            {
                databasesFilesView = new DatabasesFilesView(instanceId);
                databasesFilesView.FilterChanged += new EventHandler(databasesFilesView_FilterChanged);
                databasesFilesView.GridGroupByBoxVisibleChanged += new EventHandler(databasesFilesView_GridGroupByBoxVisibleChanged);
                UpdateDatabasesFilesFilterButtons();
                UpdateDatabasesFilesGroupByBoxVisibleButton();
            }

            ShowView(databasesFilesView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesFiles, ServerViewTabs.Databases));
        }

        void databasesFilesView_FilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesFilesFilterButtons();
        }

        void databasesFilesView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesFilesGroupByBoxVisibleButton();
        }

        private void UpdateDatabasesFilesFilterButtons()
        {
            if (databasesFilesView != null)
            {
                DatabaseSummaryConfiguration configDb = databasesFilesView.Configuration;
                ignoreToolClick = true;
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesFilesShowUserDatabasesOnlyButton"], !configDb.IncludeSystemDatabases);
                ignoreToolClick = false;
            }
        }

        private void UpdateDatabasesFilesGroupByBoxVisibleButton()
        {
            if (databasesFilesView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesFilesGroupByBoxButton"], databasesFilesView.GridGroupByBoxVisible);
            }
        }

        private void ToggleDatabasesFilesUsersFilter()
        {
            if (databasesFilesView != null)
            {
                databasesFilesView.ToggleUserFilter();
            }
        }

        private void ToggleDatabasesFilesGroupByBox()
        {
            if (databasesFilesView != null)
            {
                databasesFilesView.ToggleGroupByBox();
            }
        }

        private void ShowDatabasesTablesIndexesView()
        {
            ShowDatabasesTablesIndexesView(false);
        }

        private void ShowDatabasesTablesIndexesView(bool handleActiveViewFilter)
        {
            if (databasesTablesIndexesView == null)
            {
                databasesTablesIndexesView = new DatabasesTablesIndexesView(instanceId);
                databasesTablesIndexesView.SystemDatabasesFilterChanged += new EventHandler(databasesTablesIndexesView_SystemDatabasesFilterChanged);
                databasesTablesIndexesView.SystemTablesFilterChanged += new EventHandler(databasesTablesIndexesView_SystemTablesFilterChanged);
                databasesTablesIndexesView.DetailsPanelVisibleChanged += new EventHandler(databasesTablesIndexesView_DetailsPanelVisibleChanged);
                databasesTablesIndexesView.GridGroupByBoxVisibleChanged += new EventHandler(databasesTablesIndexesView_GridGroupByBoxVisibleChanged);
                databasesTablesIndexesView.ActionsAllowedChanged += new EventHandler(databasesTablesIndexesView_ActionsAllowedChanged);
                UpdateDatabasesTablesShowUserDatabasesOnlyButton();
                UpdateDatabasesTablesShowUserTablesOnlyButton();
                UpdateDatabasesTablesDetailsButton();
                UpdateDatabasesTablesGroupByBoxButton();
            }

            ShowView(databasesTablesIndexesView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesTablesIndexes, ServerViewTabs.Databases));
        }

        private void ShowDatabasesMirroringView()
        {
            ShowDatabasesMirroringView(false);
        }

        private void ShowDatabasesMirroringView(bool handleActiveViewFilter)
        {
            if (databasesMirroringView == null)
            {
                databasesMirroringView = new DatabasesMirroringView(instanceId);
                databasesMirroringView.MirroredDatabasesGridGroupByBoxVisibleChanged += new EventHandler(databasesMirroringView_MirroredDatabasesGridGroupByBoxVisibleChanged);
                databasesMirroringView.MirroringHistoryGridGroupByBoxVisibleChanged += new EventHandler(databasesMirroringView_MirroringHistoryGridGroupByBoxVisibleChanged);

                //databasesMirroringView.SystemDatabasesFilterChanged += new EventHandler(databasesMiTablesIndexesView_SystemDatabasesFilterChanged);
                //databasesTablesIndexesView.SystemTablesFilterChanged += new EventHandler(databasesTablesIndexesView_SystemTablesFilterChanged);
                //databasesTablesIndexesView.DetailsPanelVisibleChanged += new EventHandler(databasesTablesIndexesView_DetailsPanelVisibleChanged);
                //databasesTablesIndexesView.GridGroupByBoxVisibleChanged += new EventHandler(databasesTablesIndexesView_GridGroupByBoxVisibleChanged);
                databasesMirroringView.ActionsAllowedChanged += new EventHandler(databasesMirroringView_ActionsAllowedChanged);
                //UpdateDatabasesTablesShowUserDatabasesOnlyButton();
                //UpdateDatabasesTablesShowUserTablesOnlyButton();
                UpdateDatabaseMirroringActionButtons();
                UpdateDatabasesMirroredDatabasesGroupByBoxButton();
                UpdateDatabaseMirroringHistoryGroupByBoxButton();
            }

            ShowView(databasesMirroringView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesMirroring, ServerViewTabs.Databases));
        }

        void databasesTablesIndexesView_ActionsAllowedChanged(object sender, EventArgs e)
        {
            if (databasesTablesIndexesView != null)
            {
                toolbarsManager.Tools["updateTableStatisticsButton"].SharedProps.Enabled =
                    databasesTablesIndexesView.UpdateStatisticsAllowed;
                toolbarsManager.Tools["rebuildTableIndexesButton"].SharedProps.Enabled =
                    databasesTablesIndexesView.RebuildIndexesAllowed;
            }
        }

        private void databasesTablesIndexesView_SystemDatabasesFilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTablesShowUserDatabasesOnlyButton();
        }

        private void UpdateDatabasesTablesShowUserDatabasesOnlyButton()
        {
            if (databasesTablesIndexesView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesTablesShowUserDatabasesOnlyButton"], !databasesTablesIndexesView.IncludeSystemDatabases);
            }
        }

        private void ToggleDatabasesTablesShowUserDatabasesOnly()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.IncludeSystemDatabases = !databasesTablesIndexesView.IncludeSystemDatabases;
            }
        }

        private void databasesTablesIndexesView_SystemTablesFilterChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTablesShowUserTablesOnlyButton();
        }

        private void UpdateDatabasesTablesShowUserTablesOnlyButton()
        {
            if (databasesTablesIndexesView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["databasesTablesShowUserTablesOnlyButton"], !databasesTablesIndexesView.IncludeSystemTables);
            }
        }

        private void ToggleDatabasesTablesShowUserTablesOnly()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.IncludeSystemTables = !databasesTablesIndexesView.IncludeSystemTables;
            }
        }

        private void databasesTablesIndexesView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTablesDetailsButton();
        }

        private void UpdateDatabasesTablesDetailsButton()
        {
            if (databasesTablesIndexesView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesTablesTableDetailsButton"], databasesTablesIndexesView.DetailsPanelVisible);
            }
        }

        private void ToggleDatabasesTablesTableDetails()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.DetailsPanelVisible = !databasesTablesIndexesView.DetailsPanelVisible;
            }
        }
        private void databasesTablesIndexesView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesTablesGroupByBoxButton();
        }
        
        private void databasesMirroringView_MirroredDatabasesGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesMirroredDatabasesGroupByBoxButton();
        }
        private void databasesMirroringView_MirroringHistoryGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabaseMirroringHistoryGroupByBoxButton();
        }

        private void UpdateDatabasesMirroredDatabasesGroupByBoxButton()
        {
            if (databasesMirroringView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesMirroredDatabasesGroupByBoxButton"], databasesMirroringView.MirroredDatabasesGridGroupByBoxVisible);
            }
        }
        private void UpdateDatabaseMirroringHistoryGroupByBoxButton()
        {
            if (databasesMirroringView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabaseMirroringHistoryGroupByBoxButton"], databasesMirroringView.MirroringHistoryGridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesTablesGroupByBoxButton()
        {
            if (databasesTablesIndexesView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleDatabasesTablesGroupByBoxButton"], databasesTablesIndexesView.GridGroupByBoxVisible);
            }
        }

        private void ToggleDatabasesTablesGroupByBox()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.GridGroupByBoxVisible = !databasesTablesIndexesView.GridGroupByBoxVisible;
            }
        }

        private void UpdateTableStatistics()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.UpdateTableStatistics();
            }
        }

        private void RebuildTableIndexes()
        {
            if (databasesTablesIndexesView != null)
            {
                databasesTablesIndexesView.RebuildTableIndexes();
            }
        }
        /// <summary>
        /// Failover mirrored database role to partner in mirroring session
        /// </summary>
        private void MirroringFailover()
        {
            if (databasesMirroringView != null)
            {
                databasesMirroringView.FailOverToPartner();
            }
        }
        /// <summary>
        /// suspend or resume the selected mirroring session
        /// </summary>
        private void MirroringSuspendResume()
        {
            if (databasesMirroringView != null)
            {
                databasesMirroringView.SuspendResumeSession();
            }
        }
        /// <summary>
        /// Enable the fail over and pause buttons to modify users of both pricipal and mirror
        /// </summary>
        private void UpdateDatabaseMirroringActionButtons()
        {
            if (databasesMirroringView != null)
            {
                ((ButtonTool)toolbarsManager.Tools["databasesTabMirroringViewFailOverButton"]).SharedProps.Enabled = databasesMirroringView.ActionsAllowed;
                ((ButtonTool)toolbarsManager.Tools["databasesTabMirroringViewPauseButton"]).SharedProps.Enabled = databasesMirroringView.ActionsAllowed;
            }
        }
        void databasesMirroringView_ActionsAllowedChanged(object sender, EventArgs e)
        {
            UpdateDatabaseMirroringActionButtons();
        }

        #endregion

        #region Services Tab

        private void ShowSelectedServicesView()
        {
            if (toolbarsManager.OptionSets["servicesTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["servicesTabViewOptionSet"].SelectedTool.Key)
                {
                    case "servicesTabSummaryViewButton":
                        ShowServicesSummaryView();
                        break;
                    case "servicesTabSqlAgentJobsViewButton":
                        ShowServicesSqlAgentJobsView();
                        break;
                    case "servicesTabFullTextSearchViewButton":
                        ShowServicesFullTextSearchView();
                        break;
                    case "servicesTabReplicationViewButton":
                        ShowServicesReplicationView();
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["servicesTabSummaryViewButton"]).Checked = true;
            }
        }

        private void UpdateServicesGroups()
        {
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabServicesSummaryActionGroup"].Visible =
                activeView == servicesSummaryView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabServicesSummaryShowHideGroup"].Visible = activeView == servicesSummaryView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabReplicationShowHideGroup"].Visible = activeView == servicesReplicationView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabAgentJobsActionGroup"].Visible = 
                activeView == servicesSqlAgentJobsView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabAgentJobsFilterGroup"].Visible = activeView == servicesSqlAgentJobsView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabAgentJobsShowHideGroup"].Visible = activeView == servicesSqlAgentJobsView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabReplicationFilterGroup"].Visible = activeView == servicesReplicationView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabReplicationShowHideGroup"].Visible = activeView == servicesReplicationView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabFullTextSearchActionGroup"].Visible = 
                activeView == servicesFullTextSearchView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabFullTextSearchShowHideGroup"].Visible = activeView == servicesFullTextSearchView;
            toolbarsManager.Ribbon.Tabs["Services"].Groups["servicesTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        private void ShowServicesSummaryView()
        {
            if (servicesSummaryView == null)
            {
                servicesSummaryView = new ServicesSummaryView(instanceId, serverSummaryhistoryData);
                servicesSummaryView.ServiceActionAllowedChanged += new EventHandler(servicesSummaryView_ServiceActionAllowedChanged);
                servicesSummaryView.GridGroupByBoxVisibleChanged += new EventHandler(servicesSummaryView_GridGroupByBoxVisibleChanged);
                servicesSummaryView.ChartPanelVisibleChanged += new EventHandler(servicesSummaryView_ChartPanelVisibleChanged);
                UpdateServicesSummaryServiceActionButtons();
                UpdateServicesSummaryChartVisibleButton();
                UpdateServicesSummaryGroupByBoxVisibleButton();
            }

            ShowView(servicesSummaryView);
            UpdateServicesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesSummary, ServerViewTabs.Services));
        }

        //10.0 srishti purohit -- doctors UI implementation
        ////private void ShowRecommendationView()
        ////{
        ////    if (Analysis.Recommendations == null)
        ////    {
        ////        servicesSummaryView = new ServicesSummaryView(instanceId, serverSummaryhistoryData);
        ////        servicesSummaryView.ServiceActionAllowedChanged += new EventHandler(servicesSummaryView_ServiceActionAllowedChanged);
        ////        servicesSummaryView.GridGroupByBoxVisibleChanged += new EventHandler(servicesSummaryView_GridGroupByBoxVisibleChanged);
        ////        servicesSummaryView.ChartPanelVisibleChanged += new EventHandler(servicesSummaryView_ChartPanelVisibleChanged);
        ////        UpdateServicesSummaryServiceActionButtons();
        ////        UpdateServicesSummaryChartVisibleButton();
        ////        UpdateServicesSummaryGroupByBoxVisibleButton();
        ////    }

        ////    ShowView(servicesSummaryView);
        ////    UpdateServicesGroups();
        ////    UpdatePulseGroup();
        ////    OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesSummary, ServerViewTabs.Services));
        ////}

        void servicesSummaryView_ServiceActionAllowedChanged(object sender, EventArgs e)
        {
            UpdateServicesSummaryServiceActionButtons();
        }

        void servicesSummaryView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesSummaryGroupByBoxVisibleButton();
        }

        void servicesSummaryView_ChartPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesSummaryChartVisibleButton();
        }

        private void UpdateServicesSummaryServiceActionButtons()
        {
            if (servicesSummaryView != null)
            {
                
                ignoreToolClick = true;
                if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null)
                {
                    ((ButtonTool) toolbarsManager.Tools["startServicesServiceButton"]).SharedProps.ToolTipText =
                        "Cannot start services on a clustered server.";
                    ((ButtonTool) toolbarsManager.Tools["stopServicesServiceButton"]).SharedProps.ToolTipText =
                        "Cannot stop services on a clustered server.";
                }
                else
                {
                    ((ButtonTool)toolbarsManager.Tools["startServicesServiceButton"]).SharedProps.ToolTipText =
                        "Start service.";
                    ((ButtonTool)toolbarsManager.Tools["stopServicesServiceButton"]).SharedProps.ToolTipText =
                        "Stop service.";
                }
                ((ButtonTool) toolbarsManager.Tools["startServicesServiceButton"]).SharedProps.Enabled =
                    servicesSummaryView.StartAllowed;
                ((ButtonTool) toolbarsManager.Tools["stopServicesServiceButton"]).SharedProps.Enabled =
                    servicesSummaryView.StopAllowed;
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesSummaryGroupByBoxVisibleButton()
        {
            if (servicesSummaryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesSummaryGroupByBoxVisibleButton"], servicesSummaryView.GridGroupByBoxVisible);
            }
        }

        private void UpdateServicesSummaryChartVisibleButton()
        {
            if (servicesSummaryView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesSummaryChartsButton"], servicesSummaryView.ChartPanelVisible);
            }
        }

        private void StartServicesSummaryService()
        {
            if (servicesSummaryView != null)
            {
                servicesSummaryView.StartService();
            }
        }

        private void StopServicesSummaryService()
        {
            if (servicesSummaryView != null)
            {
                servicesSummaryView.StopService();
            }
        }

        private void ToggleServicesSummaryGroupByBox()
        {
            if (servicesSummaryView != null)
            {
                servicesSummaryView.ToggleGroupByBox();
            }
        }

        private void ToggleServicesSummaryChart()
        {
            if (servicesSummaryView != null)
            {
                servicesSummaryView.ChartPanelVisible = !servicesSummaryView.ChartPanelVisible;
            }
        }

        private void ShowServicesSqlAgentJobsView()
        {
            if (servicesSqlAgentJobsView == null)
            {
                servicesSqlAgentJobsView = new ServicesSqlAgentJobsView(instanceId);
                servicesSqlAgentJobsView.ServiceActionAllowedChanged += new EventHandler(servicesSqlAgentJobsView_ServiceActionAllowedChanged);
                servicesSqlAgentJobsView.FilterChanged += new EventHandler(servicesSqlAgentJobsView_FilterChanged);
                servicesSqlAgentJobsView.HistoryPanelVisibleChanged += new EventHandler(servicesSqlAgentJobsView_HistoryPanelVisibleChanged);
                servicesSqlAgentJobsView.GridGroupByBoxVisibleChanged += new EventHandler(servicesSqlAgentJobsView_GridGroupByBoxVisibleChanged);
                servicesSqlAgentJobsView.JobSelectionChanged += new EventHandler(servicesSqlAgentJobsView_JobSelectionChanged);
                servicesSqlAgentJobsView.UpdateDataCompleted += new EventHandler(servicesSqlAgentJobsView_UpdateDataCompleted);
                UpdateServicesAgentJobsFilterButtons();
                UpdateServicesAgentJobsHistoryVisibleButton();
                UpdateServicesAgentJobsGroupByBoxVisibleButton();
                UpdateServicesAgentJobActionsButtons();
            }

            ShowView(servicesSqlAgentJobsView);
            UpdateServicesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesSqlAgentJobs, ServerViewTabs.Services));
        }

        void servicesSqlAgentJobsView_UpdateDataCompleted(object sender, EventArgs e)
        {
            UpdateServicesAgentJobActionsButtons();
        }

        void servicesSqlAgentJobsView_JobSelectionChanged(object sender, EventArgs e)
        {
            UpdateServicesAgentJobActionsButtons();
        }

        private void UpdateServicesAgentJobActionsButtons()
        {
            if (servicesSqlAgentJobsView != null)
            {
                ignoreToolClick = true;
                if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null)
                {
                    (toolbarsManager.Tools["startServicesAgentButton"]).SharedProps.ToolTipText = @"Cannot start services on a clustered server.";
                    (toolbarsManager.Tools["stopServicesAgentButton"]).SharedProps.ToolTipText = @"Cannot stop services on a clustered server.";
                }
                else
                {
                    (toolbarsManager.Tools["startServicesAgentButton"]).SharedProps.ToolTipText = @"Start service.";
                    (toolbarsManager.Tools["stopServicesAgentButton"]).SharedProps.ToolTipText = @"Stop service.";
                }

                (toolbarsManager.Tools["startServicesAgentButton"]).SharedProps.Enabled = servicesSqlAgentJobsView.StartAllowed;
                (toolbarsManager.Tools["stopServicesAgentButton"]).SharedProps.Enabled = servicesSqlAgentJobsView.StopAllowed;

                toolbarsManager.Tools["startAgentJobButton"].SharedProps.Enabled = servicesSqlAgentJobsView.IsStartJobAllowed();
                toolbarsManager.Tools["stopAgentJobButton"].SharedProps.Enabled = servicesSqlAgentJobsView.IsStopJobAllowed();
                ignoreToolClick = false;
            }
        }

        void servicesSqlAgentJobsView_ServiceActionAllowedChanged(object sender, EventArgs e)
        {
            UpdateServicesAgentServiceActionButtons();
        }

        void servicesSqlAgentJobsView_FilterChanged(object sender, EventArgs e)
        {
            UpdateServicesAgentJobsFilterButtons();
        }

        void servicesSqlAgentJobsView_HistoryPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesAgentJobsHistoryVisibleButton();
        }

        void servicesSqlAgentJobsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesAgentJobsGroupByBoxVisibleButton();
        }

        private void UpdateServerDetailsFilterButtons()
        {
            if (serverDetailsView != null)
            {
                ServerDetailsView.Selection selection = serverDetailsView.Filter;
                ignoreToolClick = true;
                SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["detailsViewShowAllCountersButton"], selection == ServerDetailsView.Selection.All);
                SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["detailsViewShowCustomCountersButton"], selection == ServerDetailsView.Selection.Custom);
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesAgentServiceActionButtons()
        {
            if (servicesSqlAgentJobsView != null)
            {
                ignoreToolClick = true;
                ((ButtonTool)toolbarsManager.Tools["startServicesAgentButton"]).SharedProps.Enabled = servicesSqlAgentJobsView.StartAllowed;
                ((ButtonTool)toolbarsManager.Tools["stopServicesAgentButton"]).SharedProps.Enabled = servicesSqlAgentJobsView.StopAllowed;
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesAgentJobsFilterButtons()
        {
            if (servicesSqlAgentJobsView != null)
            {
                AgentJobFilter config = servicesSqlAgentJobsView.Configuration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterAgentJobsButton"]).Checked = config.IsFiltered();
                SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showAgentJobsAllButton"], config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.All);
                SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showAgentJobsRunningOnlyButton"], config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Running);
                SetRadioButtonImage((ButtonTool)toolbarsManager.Tools["showAgentJobsFailedOnlyButton"], config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Failed);
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesAgentJobsHistoryVisibleButton()
        {
            if (servicesSqlAgentJobsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesAgentJobsHistoryVisibleButton"], servicesSqlAgentJobsView.HistoryPanelVisible);
            }
        }

        private void UpdateServicesAgentJobsGroupByBoxVisibleButton()
        {
            if (servicesSqlAgentJobsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesAgentJobsGroupByBoxVisibleButton"], servicesSqlAgentJobsView.GridGroupByBoxVisible);
            }
        }

        private void ShowServicesAgentJobsFilterDialog()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.ShowFilter();
            }
        }

        private void StartServicesAgentService()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.StartService();
            }
        }

        private void StopServicesAgentService()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.StopService();
            }
        }

        private void StartAgentJob()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.PerformJobAction(JobControlAction.Start);
            }
        }

        private void StopAgentJob()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.PerformJobAction(JobControlAction.Stop);
            }
        }

        private void ToggleServicesAgentJobsShowAllFilter()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.ToggleStatusFilter(AgentJobSummaryConfiguration.JobSummaryFilterType.All);
            }
        }

        private void ToggleServicesAgentJobsShowRunningOnlyFilter()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.ToggleStatusFilter(AgentJobSummaryConfiguration.JobSummaryFilterType.Running);
            }
        }

        private void ToggleServicesAgentJobsShowFailedOnlyFilter()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.ToggleStatusFilter(AgentJobSummaryConfiguration.JobSummaryFilterType.Failed);
            }
        }

        private void ToggleServicesAgentJobsHistoryPanel()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.HistoryPanelVisible = !servicesSqlAgentJobsView.HistoryPanelVisible;
            }
        }

        private void ToggleServicesAgentJobsGroupByBox()
        {
            if (servicesSqlAgentJobsView != null)
            {
                servicesSqlAgentJobsView.ToggleGroupByBox();
            }
        }

        private void ShowServicesFullTextSearchView()
        {
            if (servicesFullTextSearchView == null)
            {
                servicesFullTextSearchView = new ServicesFullTextSearchView(instanceId);
                servicesFullTextSearchView.ServiceActionsAllowedChanged += new EventHandler(servicesFullTextSearchView_ServiceActionsAllowedChanged);
                servicesFullTextSearchView.CatalogActionsAllowedChanged += new EventHandler(servicesFullTextSearchView_CatalogActionsAllowedChanged);
                servicesFullTextSearchView.GridGroupByBoxVisibleChanged += new EventHandler(servicesFullTextSearchView_GridGroupByBoxVisibleChanged);
                servicesFullTextSearchView.DetailsPanelVisibleChanged += new EventHandler(servicesFullTextSearchView_DetailsPanelVisibleChanged);
                UpdateServicesFullTextSearchCatalogActionButtons();
                UpdateServicesFullTextSearchGroupByBoxVisibleButton();
                UpdateServicesFullTextSearchDetailsVisibleButton();
            }

            ShowView(servicesFullTextSearchView);
            UpdateServicesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesFullTextSearch, ServerViewTabs.Services));
        }

        void servicesFullTextSearchView_ServiceActionsAllowedChanged(object sender, EventArgs e)
        {
            UpdateServicesFullTextSearchServiceActionButtons();
        }

        void servicesFullTextSearchView_CatalogActionsAllowedChanged(object sender, EventArgs e)
        {
            UpdateServicesFullTextSearchCatalogActionButtons();
        }

        void servicesFullTextSearchView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesFullTextSearchGroupByBoxVisibleButton();
        }

        void servicesFullTextSearchView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesFullTextSearchDetailsVisibleButton();
        }

        private void UpdateServicesFullTextSearchServiceActionButtons()
        {
            if (servicesFullTextSearchView != null)
            {
                ignoreToolClick = true;

                if (servicesFullTextSearchView.CurrentSnapshot != null && servicesFullTextSearchView.CurrentSnapshot.ProductVersion.Major > 9)
                {
                    ((ButtonTool)toolbarsManager.Tools["startServicesFullTextSearchButton"]).SharedProps.Visible = false;
                    ((ButtonTool)toolbarsManager.Tools["stopServicesFullTextSearchButton"]).SharedProps.Visible = false;
                }
                else
                {
                    ((ButtonTool)toolbarsManager.Tools["startServicesFullTextSearchButton"]).SharedProps.Visible = true;
                    ((ButtonTool)toolbarsManager.Tools["stopServicesFullTextSearchButton"]).SharedProps.Visible = true;
                    ((ButtonTool)toolbarsManager.Tools["startServicesFullTextSearchButton"]).SharedProps.Enabled = servicesFullTextSearchView.StartAllowed;
                    ((ButtonTool)toolbarsManager.Tools["stopServicesFullTextSearchButton"]).SharedProps.Enabled = servicesFullTextSearchView.StopAllowed;
                }

                ignoreToolClick = false;
            }
        }

        private void UpdateServicesFullTextSearchCatalogActionButtons()
        {
            if (servicesFullTextSearchView != null)
            {
                ignoreToolClick = true;
                ((ButtonTool)toolbarsManager.Tools["optimizeFullTextSearchCatalogButton"]).SharedProps.Visible = servicesFullTextSearchView.OptimizeAvailable;
                ((ButtonTool)toolbarsManager.Tools["repopulateFullTextSearchCatalogButton"]).SharedProps.Visible = servicesFullTextSearchView.RepopulateAvailable;
                ((ButtonTool)toolbarsManager.Tools["rebuildFullTextSearchCatalogButton"]).SharedProps.Visible = servicesFullTextSearchView.RebuildAvailable;

                ((ButtonTool)toolbarsManager.Tools["optimizeFullTextSearchCatalogButton"]).SharedProps.Enabled =
                ((ButtonTool)toolbarsManager.Tools["repopulateFullTextSearchCatalogButton"]).SharedProps.Enabled =
                ((ButtonTool)toolbarsManager.Tools["rebuildFullTextSearchCatalogButton"]).SharedProps.Enabled = servicesFullTextSearchView.ActionsAllowed;
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesFullTextSearchGroupByBoxVisibleButton()
        {
            if (servicesFullTextSearchView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesFullTextSearchGroupByBoxVisibleButton"], servicesFullTextSearchView.GridGroupByBoxVisible);
            }
        }

        private void UpdateServicesFullTextSearchDetailsVisibleButton()
        {
            if (servicesFullTextSearchView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesFullTextSearchDetailsButton"], servicesFullTextSearchView.DetailsPanelVisible);
            }
        }

        private void StartServicesFullTextSearchService()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.StartService();
            }
        }

        private void StopServicesFullTextSearchService()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.StopService();
            }
        }

        private void OptimizeServicesFullTextSearchCatalog()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.OptimizeCatalog();
            }
        }

        private void RepopulateServicesSummaryService()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.RepopulateCatalog();
            }
        }

        private void RebuildServicesSummaryService()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.RebuildCatalog();
            }
        }

        private void ToggleServicesFullTextSearchGroupByBox()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.ToggleGroupByBox();
            }
        }

        private void ToggleServicesFullTextSearchDetailsVisible()
        {
            if (servicesFullTextSearchView != null)
            {
                servicesFullTextSearchView.DetailsPanelVisible = !servicesFullTextSearchView.DetailsPanelVisible;
            }
        }

        private void ShowServicesReplicationView()
        {
            if (servicesReplicationView == null)
            {
                servicesReplicationView = new ServicesReplicationView(instanceId);
                servicesReplicationView.FilterChanged += new EventHandler(servicesReplicationView_FilterChanged);
                servicesReplicationView.TopologyGridGroupByBoxVisibleChanged += new EventHandler(servicesReplicationView_PublisherGridGroupByBoxVisibleChanged);
                servicesReplicationView.DistributorGridGroupByBoxVisibleChanged += new EventHandler(servicesReplicationView_DistributorGridGroupByBoxVisibleChanged);
                servicesReplicationView.ReplicationGraphVisibleChanged += new EventHandler(servicesReplicationView_ReplicationGraphsVisibleChanged);
                UpdateServicesReplicationFilterButtons();
                UpdateServicesReplicationPublisherGroupByBoxVisibleButton();
                UpdateServicesReplicationDistributorGroupByBoxVisibleButton();
                UpdateServicesReplicationGraphsVisibleButton();
            }

            ShowView(servicesReplicationView);
            UpdateServicesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesReplication, ServerViewTabs.Services));
        }

        void servicesReplicationView_FilterChanged(object sender, EventArgs e)
        {
            UpdateServicesReplicationFilterButtons();
        }

        void servicesReplicationView_PublisherGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesReplicationPublisherGroupByBoxVisibleButton();
        }

        void servicesReplicationView_ReplicationGraphsVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesReplicationGraphsVisibleButton();
        }

        void servicesReplicationView_DistributorGridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateServicesReplicationDistributorGroupByBoxVisibleButton();
        }

        private void UpdateServicesReplicationFilterButtons()
        {
            if (servicesReplicationView != null)
            {
                ReplicationFilter config = servicesReplicationView.Configuration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterReplicationButton"]).Checked = config.IsFiltered();
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesReplicationPublisherGroupByBoxVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesReplicationTopologyGroupByBoxVisibleButton"], servicesReplicationView.TopologyGridGroupByBoxVisible);
            }
        }
        private void UpdateServicesReplicationDistributorGroupByBoxVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesReplicationDistributorGroupByBoxVisibleButton"], servicesReplicationView.DistributorGridGroupByBoxVisible);
            }
        }
        private void UpdateServicesReplicationGraphsVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleServicesReplicationReplicationGraphsVisibleButton"], servicesReplicationView.ReplicationGraphsVisible);
            }
        }
        private void ShowServicesReplicationFilterDialog()
        {
            if (servicesReplicationView != null)
            {
                servicesReplicationView.ShowFilter();
            }
        }

        private void ToggleServicesReplicationTopologyGroupByBox()
        {
            if (servicesReplicationView != null)
            {
                servicesReplicationView.ToggleTopologyGroupByBox();
            }
        }
        private void ToggleServicesReplicationGraphsVisible()
        {
            if (servicesReplicationView != null)
            {
                servicesReplicationView.ToggleReplicationGraphsVisible();
            }
        }

        private void ToggleServicesReplicationDistributorGroupByBox()
        {
            if (servicesReplicationView != null)
            {
                servicesReplicationView.ToggleDistributorGroupByBox();
            }
        }
        private void ToggleDatabasesMirroredDatabasesGroupByBox()
        {
            if (databasesMirroringView != null)
            {
                databasesMirroringView.ToggleMirroredDatabasesGroupByBox();
            }
        }
        private void ToggleDatabasesMirroringHistoryGroupByBox()
        {
            if (databasesMirroringView != null)
            {
                databasesMirroringView.ToggleMirroringHistoryGroupByBox();
            }
        }

        #endregion

        #region Logs Tab

        private void UpdateLogsGroups()
        {
            toolbarsManager.Ribbon.Tabs["Logs"].Groups["logsTabActionsGroup"].Visible =
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            toolbarsManager.Ribbon.Tabs["Logs"].Groups["logsTabPulseGroup"].Visible = ApplicationModel.Default.IsPulseConfigured;
        }

        private void ShowLogsView()
        {
            if (logsView == null)
            {
                logsView = new LogsView(instanceId);
                logsView.FilterChanged += new EventHandler(logsView_FilterChanged);
                logsView.AvailableLogsPanelVisibleChanged += new EventHandler(logsView_AvailableLogsPanelVisibleChanged);
                logsView.DetailsPanelVisibleChanged += new EventHandler(logsView_DetailsPanelVisibleChanged);
                logsView.GridGroupByBoxVisibleChanged += new EventHandler(logsView_GridGroupByBoxVisibleChanged);
                UpdateLogsViewAvailableLogsVisibleButton();
                UpdateLogsViewDetailsVisibleButton();
                UpdateLogsViewGroupByBoxVisibleButton();
            }

            ShowView(logsView);
            UpdateLogsGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.Logs, ServerViewTabs.Logs));
        }

        private void UpdateLogsViewFilterButtons()
        {
            if (logsView != null)
            {
                ErrorLogConfiguration config = logsView.Configuration;
                ignoreToolClick = true;
                ((StateButtonTool)toolbarsManager.Tools["filterLogsButton"]).Checked = config.IsFiltered();
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["filterLogsShowErrorsButton"], config.ShowErrors);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["filterLogsShowWarningsButton"], config.ShowWarnings);
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["filterLogsShowInformationalButton"], config.ShowInformational);
                ignoreToolClick = false;
            }
        }

        private void UpdateLogsViewAvailableLogsVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleAvailableLogsButton"], logsView.AvailableLogsPanelVisible);
            }
        }

        private void UpdateLogsViewDetailsVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleLogDetailsButton"], logsView.DetailsPanelVisible);
            }
        }

        private void UpdateLogsViewGroupByBoxVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckboxImage((ButtonTool)toolbarsManager.Tools["toggleLogGroupByBoxButton"], logsView.GridGroupByBoxVisible);
            }
        }

        private void ShowLogsFilterDialog()
        {
            logsView.ShowFilter();
        }

        private void ToggleLogsShowErrorsFilter()
        {
            logsView.ToggleErrorFilter();
        }

        private void ToggleLogsShowWarningsFilter()
        {
            logsView.ToggleWarningFilter();
        }

        private void ToggleLogsShowInformationalFilter()
        {
            logsView.ToggleInformationalFilter();
        }

        private void ShowLogsSearchDialog()
        {
            logsView.ShowSearch();
        }

        private void CycleLogs()
        {
            logsView.CycleServerLog();
        }

        private void ConfigureLogs()
        {
            logsView.ConfigureLogs();
        }

        private void ToggleLogsViewAvailableLogs()
        {
            if (logsView != null)
            {
                logsView.AvailableLogsPanelVisible = !logsView.AvailableLogsPanelVisible;
            }
        }

        private void ToggleLogsViewDetails()
        {
            if (logsView != null)
            {
                logsView.DetailsPanelVisible = !logsView.DetailsPanelVisible;
            }
        }

        private void ToggleLogsViewGroupByBox()
        {
            if (logsView != null)
            {
                logsView.GridGroupByBoxVisible = !logsView.GridGroupByBoxVisible;
            }
        }

        private void logsView_FilterChanged(object sender, EventArgs e)
        {
            UpdateLogsViewFilterButtons();
        }

        private void logsView_AvailableLogsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateLogsViewAvailableLogsVisibleButton();
        }

        private void logsView_DetailsPanelVisibleChanged(object sender, EventArgs e)
        {
            UpdateLogsViewDetailsVisibleButton();
        }

        private void logsView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateLogsViewGroupByBoxVisibleButton();
        }

        #endregion

        #region Analysis Tab

        private void ShowSelectedAnalysisView()
        {
            if (toolbarsManager.OptionSets["analysisTabViewOptionSet"].SelectedTool != null)
            {
                switch (toolbarsManager.OptionSets["analysisTabViewOptionSet"].SelectedTool.Key)
                {
                    case "analysisRunAnalysisButton":
                        ShowServerRunAnalysisView(false);
                        break;
                    case "analysisWorkloadAnalysisButton":
                        ShowServerRunAnalysisView(true);
                        break;
                }
            }
            else
            {
                ((StateButtonTool)toolbarsManager.Tools["analysisRunAnalysisButton"]).Checked = true;
            }
        }

        private void UpdateAnalysisGroups()
        {
            toolbarsManager.Ribbon.Tabs["Analysis"].Groups["analyzeTabRunGroup"].Visible = Visible;
            
        }

        #region Run Analysis View

        private void ShowServerRunAnalysisView(bool isWorkloadAnalysis)
        {
            if (analysisView == null)
            {
                analysisView = new Analysis.Recommendations(instanceId, isWorkloadAnalysis);
            }

            ShowView(analysisView);
            UpdateAnalysisGroups();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
        }
        #endregion

        #endregion

        #region News Feed functions for all Tabs

        private void PulseController_PulseConnectionChanged(object sender, Idera.Newsfeed.Plugins.Objects.PulseConnectionChangedEventArgs e)
        {
            UpdateUserTokenAttributes();
            UpdatePulseGroup();
        }

        private void PulseController_AuthenticationChanged(object sender, Idera.Newsfeed.Plugins.Objects.AuthenticationChangedEventArgs e)
        {
            UpdatePulseGroup();
        }

        private void PulseController_ServerChanged(object sender, Idera.Newsfeed.Plugins.Objects.AssetChangedEventArgs e)
        {
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                if (e.Name == ApplicationModel.Default.ActiveInstances[instanceId].InstanceName)
                {
                    UpdatePulseGroup();
                }
            }
        }

        /// <summary>
        /// Enable the pulse group if the user is monitoring in pulse
        /// </summary>
        public void UpdatePulseGroup()
        {
            if (parentView != null)
            {
                bool enabled = false;
                if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                {
                    int pulseId;
                    if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                    {
                        UpdatePulseFollowButton(pulseId);
                        enabled = true;
                    }
                }

                toolbarsManager.Tools["pulseProfileButton"].SharedProps.Enabled = enabled;
                toolbarsManager.Tools["pulseFollowButton"].SharedProps.Enabled = enabled && PulseController.Default.IsLoggedIn;
                toolbarsManager.Tools["pulsePostButton"].SharedProps.Enabled = enabled;
            }
            else
            {
                toolbarsManager.Tools["pulseProfileButton"].SharedProps.Enabled = false;
                toolbarsManager.Tools["pulseFollowButton"].SharedProps.Enabled = false;
                toolbarsManager.Tools["pulsePostButton"].SharedProps.Enabled = false;
            }
        }

        private void ShowPulseProfile()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
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
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error showing Newsfeed profile for server", ex);
            }
        }

        private void TogglePulseFollow()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                    {
                        int pulseId;
                        if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                        {
                            ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
                            bool followed = (button.CaptionResolved == "Unfollow Server");
                            PulseController.Default.SetFollowing(pulseId, !followed);
                            UpdatePulseFollowButton(pulseId);
                            bool followedNew = button.CaptionResolved == "Unfollow Server";
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
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error following server in Newsfeed.", ex);
            }
        }

        private void UpdatePulseFollowButton(int pulseId)
        {
            ButtonTool button = (ButtonTool)toolbarsManager.Tools["pulseFollowButton"];
            bool following = PulseController.Default.GetFollowing(pulseId);
            ignoreToolClick = true;
            if (following)
            {
                button.SharedProps.Caption = "Unfollow Server";
                button.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Unfollow32;
            }
            else
            {
                button.SharedProps.Caption = "Follow Server";
                button.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Follow32;
            }
            ignoreToolClick = false;
        }


        private void ShowPulsePost()
        {
            try
            {
                if (PulseHelper.CheckPulseLogin(ParentForm))
                {
                    if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                    {
                        int pulseId;
                        if (PulseController.Default.GetPulseServerId(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, out pulseId))
                        {
                            PulseController.Default.ShowPostDialog(ParentForm, pulseId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(ParentForm, "Error following server in Newsfeed.", ex);
            }
        }

        #endregion

        private UIElement activeElement = null;
        private void toolbarsManager_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (activeElement == e.Element) return;
            if (e.Element is ImageAndTextUIElement.ImageAndTextDependentImageUIElement)
            {
                if (activeElement != null)
                    tooltip.Hide();
                ShowTooltip(e.Element);
            }
            else
            {
                if (activeElement != null)
                {
                    activeElement = null;
                    tooltip.Hide();
                }
            }

        }

        bool ConfigureTooltip(string tabKey)
        {
            var status = activeStatus;
            if (activeStatus == null)
                status = ApplicationModel.Default.GetInstanceStatus(instanceId);
            if (status == null)
                return false;


            MetricCategory category = MetricCategory.All;
            switch (tabKey)
            {
                case "Queries":
                    category = MetricCategory.Queries;
                    break;
                case "Sessions":
                    category = MetricCategory.Sessions;
                    break;
                case "Resources":
                    category = MetricCategory.Resources;
                    break;
                case "Databases":
                    category = MetricCategory.Databases;
                    break;
                case "Services":
                    category = MetricCategory.Services;
                    break;
                case "Logs":
                    category = MetricCategory.Logs;
                    break;
                //SQLdm srishti purohit -- Doctors UI
                case "Analysis":
                    category = MetricCategory.Analyze;
                    break;
                default:
                    tooltip.ToolTipText = status.ToolTip;
                    tooltip.ToolTipTitle = status.ToolTipHeading;
                    tooltip.CustomToolTipImage = status.ToolTipHeadingImage;
                    return true;
            }
            var issues = status[category];
            if (issues != null && issues.Count > 0)
            {
                var issueArray = Algorithms.ToArray(issues);
                tooltip.ToolTipText = status.GetToolTip(category);
                tooltip.ToolTipTitle = String.Format("{0} are {1}", category, issueArray[0].Severity);
                tooltip.CustomToolTipImage = MonitoredSqlServerStatus.GetSeverityImage(issueArray[0].Severity);
                return true;
            }
            return false;
        }

        private void ShowTooltip(UIElement uiElement)
        {

            var tabElement = (RibbonTabItemUIElement) uiElement.GetAncestor(typeof (RibbonTabItemUIElement));
            if (tabElement == null) return;
            var ribbonTab = tabElement.TabItem as RibbonTab;
            if (ribbonTab == null) return;

            if (ConfigureTooltip(ribbonTab.Key))
            {
                activeElement = uiElement;
                var parent = uiElement.Parent;
                var point = this.PointToScreen(new Point(uiElement.Rect.X, parent.RectInsideBorders.Bottom));
                tooltip.Show(
                    point,
                    0,
                    true,
                    true,
                    true,
                    Rectangle.Empty,
                    false,
                    false,
                    false);
            }
        }

        private void toolbarsManager_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (activeElement != null || tooltip.IsVisible)
            {
                activeElement = null;
                tooltip.Hide();
            }
        }
    }

    internal sealed class SubViewChangedEventArgs : EventArgs
    {
        public readonly ServerViews NewView;
        public readonly ServerViewTabs SelectedTab;

        public SubViewChangedEventArgs(ServerViews newView, ServerViewTabs selectedTab)
        {
            NewView = newView;
            SelectedTab = selectedTab;
        }
    }
}
