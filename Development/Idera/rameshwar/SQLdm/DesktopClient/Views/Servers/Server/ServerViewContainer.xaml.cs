using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows.Media;
using Application = System.Windows.Application;
using Size = System.Drawing.Size;
using UserControl = System.Windows.Controls.UserControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Windows;
using Infragistics.Windows.Ribbon;
using Infragistics.Windows.Ribbon.Events;
using Infragistics.Windows.Themes;
using ButtonTool = Infragistics.Windows.Ribbon.ButtonTool;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Alerts;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Logs;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Services;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions;
using DashboardFilter = Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview.DashboardLayoutGalleryViewModel.DashboardFilter;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis;
using Idera.SQLdm.DesktopClient.Dialogs.Analysis;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using System.Linq;
using Idera.SQLdm.DesktopClient.Controls.Presentation;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    /// <summary>
    /// Interaction logic for ServerViewContainer.xaml
    /// </summary>
    internal partial class ServerViewContainer : UserControl, IServerView
    {
        /// <summary>
        /// The offset used to fix the column width to 75% of the defined value on the XAML.
        /// </summary>
        private const double DecreasedOffset = 0.75d;

        private int instanceId;
        private System.Windows.Forms.Label analyzeOperationalStatusLabel;
        private System.Windows.Forms.PictureBox analyzeOperationalStatusImage;
        private Panel analyzeOperationalStatusPanel;
        private ServerViewMode viewMode = ServerViewMode.RealTime;
        private int currentScale = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
        private DateTime? currentHistoricalStartDateTime = null;
        private DateTime? currentHistoricalSnapshotDateTime = null;
        private bool historyBrowserSnapshotChanged = false;
        private BackgroundWorker historyBrowserBackgroundWorker;

        private IServerView activeView;

        private ServerSummaryView4 serverSummaryView;
        private ServerDetailsView serverDetailsView;
        private ServerConfigurationView serverConfigurationView;
        private ActiveAlertsView serverActiveAlertsView;
        private ServerTimelineView serverTimelineView;
        private SessionsSummaryView sessionsSummaryView;
        private SessionsDetailsView sessionsDetailsView;
        private SessionsLocksView sessionsLocksView;
        private SessionsBlockingView sessionsBlockingView;
        private QueryMonitorView queryMonitorView;
        private ResourcesSummaryView resourcesSummaryView;
        private ResourcesCpuView resourcesCpuView;
        private ResourcesMemoryView resourcesMemoryView;
        private ResourcesDiskView resourcesDiskView;
        private ResourcesDiskSizeView resourcesDiskSizeView;           // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
        private ResourcesProcedureCacheView resourcesProcedureCacheView;
        private ResourcesWaitStats resourcesWaitStatsView;
        private ResourcesWaitStatsActive queriesWaitStatsActiveView;
        private ResourcesFileActivity resourcesFileActivityView;
        private DatabasesSummaryView databasesSummaryView;
        private DatabasesConfigurationView databasesConfigurationView;
        private DatabasesBackupRestoreHistoryView databasesBackupRestoreHistoryView;
        private DatabasesTablesIndexesView databasesTablesIndexesView;
        private DatabasesMirroringView databasesMirroringView;
        private DatabasesAlwaysOnView databasesAlwaysOnView;
        private DatabasesFilesView databasesFilesView;
        private DatabasesTempdbView databasesTempdbView;
        private ServicesSummaryView servicesSummaryView;
        private ServicesSqlAgentJobsView servicesSqlAgentJobsView;
        private ServicesFullTextSearchView servicesFullTextSearchView;
        private ServicesReplicationView servicesReplicationView;
        private LogsView logsView;
        //SQLdm 10.0.2 Barkha Khatri 
        private BackgroundWorker backgroundProductVersionWorker;
        //SQLdm 10.0 srishti purohit implementing dostor's UI 
        private Analysis.Recommendations recommendationAnalysis;
        private DefaultScreenAnalysisTab defaultAnalysisView;
        //SQLdm10.1 (srishti purohit) Launch SWA from instance view
        private string swaLaunchURL = null;
        private readonly HistoryBrowserOptionsDialog historyOptionsDialog = new HistoryBrowserOptionsDialog();
        public event EventHandler<EventArgs> HistoricalCustomRangeSelected;

        private DashboardLayoutGallery _dashboardLayoutGallery;


        private readonly ServerSummaryHistoryData serverSummaryhistoryData = new ServerSummaryHistoryData();
        private readonly ServerViewViewModel _viewModel;

        //        private Infragistics.Win.ToolTip tooltip;
        private MonitoredSqlServerWrapper activeInstance;
        public event EventHandler<SubViewChangedEventArgs> SubViewChanged;
        private bool ignoreToolClick = false;
        private bool ignoreTabSelect = false;
        private bool _disposed = false;
        private string selectedTabName;
        private string selectedServerTabTag;
        private string selectedRibbonMenu;
        private MonitoredSqlServerWrapper[] activeInstances;
        private string parentBreadcrumb = string.Empty;
        private System.Windows.Controls.Primitives.Popup comboBoxItemPopup = new System.Windows.Controls.Primitives.Popup();
        private System.Windows.Controls.Primitives.Popup comboBoxPopup = new System.Windows.Controls.Primitives.Popup();

        public ServerViews? CurrentServerView
        {
            get
            {
                if (_viewModel == null)
                    return null;
                else
                    return _viewModel.SelectedServerTab;
            }
        }
        public ServerViewContainer()
        {
            InitializeComponent();
            InitializeHistoryBrowserColumn();
            
            // hook up the history browser
            historyBrowserBackgroundWorker = new BackgroundWorker();
            this.historyBrowserBackgroundWorker.DoWork += historyBrowserBackgroundWorker_DoWork;
            this.historyBrowserBackgroundWorker.RunWorkerCompleted += historyBrowserBackgroundWorker_RunWorkerCompleted;
            this.historyBrowserControl.HistoricalSnapshotSelected += historyBrowserControl_HistoricalSnapshotSelected;
            this.historyBrowserControl.HistoricalSnapshotSelected += QuickHistorialSnapshotTextChange;
            this.historyBrowserControl.HistoricalCustomRangeSelected += historyBrowserControl_HistoricalCustomRangeSelected;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
            Settings.Default.PropertyChanged += OnSettingChanged;
            this.IsVisibleChanged += ServerViewContainer_IsVisibleChanged;
            activeInstances = ApplicationModel.Default.ActiveInstances.ToArray();
            ResetInstancesCombobox();
            InstanceComboBox.SelectionChanged += InstanceComboBox_SelectionChanged;
            InstanceComboBox.KeyUp += InstanceComboBox_KeyUp;
            InstanceComboBox.LostFocus += InstanceComboBox_LostFocus;
            InstanceComboBox.MouseEnter+= InstanceComboBox_MouseEnter;
            InstanceComboBox.MouseLeave += InstanceComboBox_MouseLeave;
            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;
        }
        private void InstanceComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            var selectedItem = InstanceComboBox.SelectedItem as MonitoredSqlServerWrapper;
            String comboItem=null;
            if (selectedItem != null)
            {
                comboItem = selectedItem.InstanceName;
                comboBoxPopup.PlacementTarget = InstanceComboBox;
                comboBoxPopup.Placement = PlacementMode.Top;
                label.Content = comboItem;
                label.Background = Brushes.White;
                label.FontSize = 10;
                comboBoxPopup.Child = label;
                if (InstanceComboBox.ActualWidth >= InstanceComboBox.MaxWidth - 1)
                {
                    comboBoxPopup.IsOpen = true;
                    comboBoxItemPopup.IsOpen = false;
                }
            }
        }
        private void InstanceComboBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            comboBoxPopup.IsOpen = false;
            comboBoxItemPopup.IsOpen = false;
        }
        private void ComboBoxItem_OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            comboBoxPopup.IsOpen = false;
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            System.Windows.Controls.TextBlock comboItem =(System.Windows.Controls.TextBlock) e.Source;
            comboBoxItemPopup.PlacementTarget = comboItem;
            comboBoxItemPopup.Placement = PlacementMode.Right;
            label.Content = comboItem.Text;
            label.Background = Brushes.White;
            label.FontSize = 10;
            comboBoxItemPopup.Child = label;
            if (comboItem.ActualWidth >= comboItem.MaxWidth - 10)
            {
                comboBoxItemPopup.IsOpen = true;
                comboBoxPopup.IsOpen = false;
            }
        }
        private void ComboBoxItem_OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            comboBoxItemPopup.IsOpen = false;
            comboBoxPopup.IsOpen = false;
        }
        
        private void InstanceComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            InstanceComboBox.IsDropDownOpen = true;
            InstanceComboBox.SelectedValue = null;
            var text = InstanceComboBox.Text ?? string.Empty;
            InstanceComboBox.ItemsSource = activeInstances.Where(x => x.InstanceName.ToLower().Contains(text.ToLower()));
        }

        private void InstanceComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(InstanceComboBox.SelectedIndex < 0)
            {
                ResetInstancesCombobox();
            }
            comboBoxPopup.IsOpen = false;
            comboBoxItemPopup.IsOpen = false;
        }

        private void InstanceComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            comboBoxItemPopup.IsOpen = false;
            comboBoxPopup.IsOpen = false;
            var selectedItem = InstanceComboBox.SelectedItem as MonitoredSqlServerWrapper;
            if (selectedItem != null && selectedItem.Id != instanceId)
            {
                ApplicationModel.Default.SelectedInstanceId = selectedItem.Id;
                if(!selectedItem.Instance.CloudProviderId.HasValue || (selectedItem.Instance.CloudProviderId.Value != Common.Constants.MicrosoftAzureId && selectedItem.Instance.CloudProviderId.Value != Common.Constants.AmazonRDSId))
                    ApplicationController.Default.ShowServerView(selectedItem.Id,_viewModel.SelectedServerTab);
                else
                {
                    List<ServerViews> disallowedViews = null;
                    if (selectedItem.Instance.CloudProviderId.Value == Common.Constants.MicrosoftAzureId) //SQLdm 11.0 Azure Query Monitor
                        disallowedViews = new List<ServerViews> { ServerViews.OverviewConfiguration, ServerViews.SessionsLocks, ServerViews.SessionsBlocking, ServerViews.Logs, ServerViews.ResourcesCpu, ServerViews.DatabaseAlwaysOn, ServerViews.DatabasesTempdbView, ServerViews.DatabasesFiles, ServerViews.DatabasesMirroring, ServerViews.DatabasesTablesIndexes, ServerViews.DatabasesBackupRestoreHistory, ServerViews.DatabasesSummary, ServerViews.Databases, ServerViews.ResourcesMemory, ServerViews.ResourcesDisk, ServerViews.ResourcesProcedureCache, ServerViews.ResourcesFileActivity, ServerViews.ResourcesDiskSize, ServerViews.Services, ServerViews.ServicesFullTextSearch, ServerViews.ServicesReplication, ServerViews.ServicesSqlAgentJobs, ServerViews.ServicesSummary };
                    else
                        disallowedViews = new List<ServerViews> { ServerViews.DatabaseAlwaysOn, ServerViews.DatabasesMirroring, ServerViews.Services, ServerViews.ServicesFullTextSearch, ServerViews.ServicesReplication, ServerViews.ServicesSqlAgentJobs, ServerViews.ServicesSummary, ServerViews.ResourcesDiskSize, ServerViews.ResourcesFileActivity };
                    if (disallowedViews.Contains(_viewModel.SelectedServerTab))
                        ApplicationController.Default.ShowServerView(selectedItem.Id, ApplicationController.Default.GetCurrentServerViewForServer(selectedItem.Id));
                    else
                        ApplicationController.Default.ShowServerView(selectedItem.Id, _viewModel.SelectedServerTab);
                }
            }
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            activeInstances = ApplicationModel.Default.ActiveInstances.ToArray();
            ResetInstancesCombobox();
        }

        private void ServerViewContainer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                ResetInstancesCombobox();
            }
        }
        
        public ServerViewContainer(int instanceId)
            : this()
        {
            this.instanceId = instanceId;
            ResetInstancesCombobox();
            if (backgroundProductVersionWorker == null)
            {
                backgroundProductVersionWorker = new BackgroundWorker();
                backgroundProductVersionWorker.WorkerSupportsCancellation = true;
                backgroundProductVersionWorker.DoWork += new DoWorkEventHandler(backgroundProductVersionWorker_DoWork);
                backgroundProductVersionWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(backgroundProductVersionWorker_RunWorkerCompleted);
            }

            if (!backgroundProductVersionWorker.IsBusy)
            {
                object[] parameters = new object[] { instanceId };

                backgroundProductVersionWorker.RunWorkerAsync(parameters);
            }

            serverSummaryhistoryData.InstanceId = instanceId;
            
            _viewModel = new ServerViewViewModel(instanceId);

            var shortTimeFormat = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern;
            var twelveHourTimeFormat = shortTimeFormat.Contains("tt");
            var timeFormatString = "hh:mm tt";
            if (!twelveHourTimeFormat)
                timeFormatString = "HH:mm";
            startTime.SetBinding(Infragistics.Windows.Editors.XamDateTimeEditor.TextProperty, new System.Windows.Data.Binding { Path= new PropertyPath("StartHistoryTime"),StringFormat= timeFormatString });
            endTime.SetBinding(Infragistics.Windows.Editors.XamDateTimeEditor.TextProperty, new System.Windows.Data.Binding { Path = new PropertyPath("EndHistoryTime"), StringFormat = timeFormatString });

            DataContext = _viewModel;
            
            _viewModel.ParentBreadcrumb = parentBreadcrumb;

            historyTimestampLabel.Text = String.Empty;
            UpdateInstanceTitle();
            UpdateTheme();
            Initialize();
        }

        public int InstanceId
        {
            get { return instanceId; }
        }

        #region disposal

        ~ServerViewContainer()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //disconnect the event handlers before disposing
                Settings.Default.PropertyChanged -= OnSettingChanged;
                ApplicationController.Default.BackgroundRefreshCompleted -= BackgroundRefreshCompleted;

                if (serverSummaryView != null)
                {
                    serverSummaryView.PanelGalleryVisibleChanged -= dashboardView_PanelGalleryVisibleChanged;
                    serverSummaryView.HistoricalSnapshotDateTimeChanged -= serverSummaryView_HistoricalSnapshotDateTimeChanged;
                    serverSummaryView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    serverSummaryView.Dispose();
                }
                if (serverDetailsView != null)
                {
                    serverDetailsView.ChartPanelVisibleChanged -= serverDetailsView_ChartPanelVisibleChanged;
                    serverDetailsView.GridGroupByBoxVisibleChanged -= serverDetailsView_GridGroupByBoxVisibleChanged;
                    serverDetailsView.FilterChanged -= serverDetailsView_FilterChanged;
                    serverDetailsView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    serverDetailsView.Dispose();
                }
                if (serverConfigurationView != null)
                {
                    serverConfigurationView.DetailsPanelVisibleChanged -= serverConfigurationView_DetailsPanelVisibleChanged;
                    serverConfigurationView.GridGroupByBoxVisibleChanged -= serverConfigurationView_GridGroupByBoxVisibleChanged;
                    serverConfigurationView.Dispose();
                }
                if (serverActiveAlertsView != null)
                {
                    serverActiveAlertsView.HistoricalSnapshotDateTimeChanged -= activeAlertsView_HistoricalSnapshotDateTimeChanged;
                    serverActiveAlertsView.ForecastPanelVisibleChanged -= activeAlertsView_ForecastPanelVisibleChanged;
                    serverActiveAlertsView.DetailsPanelVisibleChanged -= activeAlertsView_DetailsPanelVisibleChanged;
                    serverActiveAlertsView.GridGroupByBoxVisibleChanged -= activeAlertsView_GridGroupByBoxVisibleChanged;
                    serverActiveAlertsView.Dispose();
                }
                if (serverTimelineView != null)
                {
                    serverTimelineView.Dispose();
                }
                if (sessionsSummaryView != null)
                {
                    sessionsSummaryView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    sessionsSummaryView.Dispose();
                }
                if (sessionsDetailsView != null)
                {
                    sessionsDetailsView.HistoricalSnapshotDateTimeChanged -= sessionsDetailsView_HistoricalSnapshotDateTimeChanged;
                    sessionsDetailsView.DetailsPanelVisibleChanged -= sessionsDetailsView_DetailsPanelVisibleChanged;
                    sessionsDetailsView.GridGroupByBoxVisibleChanged -= sessionsDetailsView_GridGroupByBoxVisibleChanged;
                    sessionsDetailsView.FilterChanged -= sessionsDetailsView_FilterChanged;
                    sessionsDetailsView.TraceAllowedChanged -= sessionsDetailsView_TraceEnabledChanged;
                    sessionsDetailsView.KillAllowedChanged -= sessionsDetailsView_KillEnabledChanged;
                    sessionsDetailsView.Dispose();
                }
                if (sessionsLocksView != null)
                {
                    sessionsLocksView.HistoricalSnapshotDateTimeChanged -= sessionsLocksView_HistoricalSnapshotDateTimeChanged;
                    sessionsLocksView.FilterChanged -= sessionsLocksView_FilterChanged;
                    sessionsLocksView.ChartVisibleChanged -= sessionsLocksView_ChartVisibleChanged;
                    sessionsLocksView.GridGroupByBoxVisibleChanged -= sessionsLocksView_GridGroupByBoxVisibleChanged;
                    sessionsLocksView.TraceAllowedChanged -= sessionsLocksView_TraceEnabledChanged;
                    sessionsLocksView.KillAllowedChanged -= sessionsLocksView_KillEnabledChanged;
                    sessionsLocksView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    sessionsLocksView.Dispose();
                }
                if (sessionsBlockingView != null)
                {
                    sessionsBlockingView.HistoricalSnapshotDateTimeChanged -= sessionsBlockingView_HistoricalSnapshotDateTimeChanged;
                    sessionsBlockingView.ChartVisibleChanged -= sessionsBlockingView_ChartVisibleChanged;
                    sessionsBlockingView.BlocksDeadlocksListVisibleChanged -= sessionsBlockingView_BlocksDeadlocksListVisibleChanged;
                    sessionsBlockingView.BlockingTypeChanged -= sessionsBlockingView_BlockingTypeChanged;
                    sessionsBlockingView.TraceAllowedChanged -= sessionsBlockingView_TraceEnabledChanged;
                    sessionsBlockingView.KillAllowedChanged -= sessionsBlockingView_KillEnabledChanged;
                    sessionsBlockingView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    sessionsBlockingView.Dispose();
                }
                if (queryMonitorView != null)
                {
                    queryMonitorView.FiltersVisibleChanged -= queryMonitorView_FiltersVisibleChanged;
                    queryMonitorView.GridVisibleChanged -= queryMonitorView_GridVisibleChanged;
                    queryMonitorView.GridGroupByBoxVisibleChanged -= queryMonitorView_GridGroupByBoxVisibleChanged;
                    queryMonitorView.FilterChanged -= queryMonitorView_FilterChanged;
                    queryMonitorView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    queryMonitorView.Dispose();
                }
                if (resourcesSummaryView != null)
                {
                    resourcesSummaryView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    resourcesSummaryView.Dispose();
                }
                if (resourcesCpuView != null)
                {
                    resourcesCpuView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    resourcesCpuView.Dispose();
                }
                if (resourcesMemoryView != null)
                {
                    resourcesMemoryView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    resourcesMemoryView.Dispose();
                }
                if (resourcesDiskView != null)
                {
                    resourcesDiskView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    resourcesDiskView.Dispose();
                }
                // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --disposing disk size view from Resources
                if (resourcesDiskSizeView != null)
                {
                    resourcesDiskSizeView.Dispose();
                }
                if (resourcesProcedureCacheView != null)
                {
                    resourcesProcedureCacheView.ChartsVisibleChanged -= resourcesProcedureCacheView_ChartsVisibleChanged;
                    resourcesProcedureCacheView.GridGroupByBoxVisibleChanged -= resourcesProcedureCacheView_GridGroupByBoxVisibleChanged;
                    resourcesProcedureCacheView.Dispose();
                }
                if (resourcesWaitStatsView != null)
                {
                    resourcesWaitStatsView.GridGroupByBoxVisibleChanged -= resourcesWaitStatsView_GridGroupByBoxVisibleChanged;
                    resourcesWaitStatsView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    resourcesWaitStatsView.Dispose();
                }
                if (queriesWaitStatsActiveView != null)
                {
                    queriesWaitStatsActiveView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    queriesWaitStatsActiveView.Dispose();
                }
                if (resourcesFileActivityView != null)
                {
                    resourcesFileActivityView.FilterPaneVisibleChanged -= resourcesFileActivityView_FilterPaneVisibleChanged;
                    resourcesFileActivityView.Dispose();
                }
                if (databasesSummaryView != null)
                {
                    databasesSummaryView.FilterChanged -= databasesSummaryView_FilterChanged;
                    databasesSummaryView.GridGroupByBoxVisibleChanged -= databasesSummaryView_GridGroupByBoxVisibleChanged;
                    databasesSummaryView.ChartVisibleChanged -= databasesSummaryView_ChartVisibleChanged;
                    databasesSummaryView.Dispose();
                }
                if (databasesAlwaysOnView != null)
                {
                    databasesAlwaysOnView.GridGroupByBoxVisibleChanged -= databasesAlwaysOnView_GridGroupByBoxVisibleChanged;
                    databasesAlwaysOnView.ChartVisibleChanged -= databasesAlwaysOnView_ChartVisibleChanged;
                    databasesAlwaysOnView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    databasesAlwaysOnView.Dispose();
                }
                if (databasesConfigurationView != null)
                {
                    databasesConfigurationView.FilterChanged -= databasesConfigurationView_FilterChanged;
                    databasesConfigurationView.GridGroupByBoxVisibleChanged -= databasesConfigurationView_GridGroupByBoxVisibleChanged;
                    databasesConfigurationView.Dispose();
                }
                if (databasesBackupRestoreHistoryView != null)
                {
                    databasesBackupRestoreHistoryView.FilterChanged -= databasesBackupRestoreHistoryView_FilterChanged;
                    databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisibleChanged -= databasesBackupRestoreHistoryView_DatabasesGridGroupByBoxVisibleChanged;
                    databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisibleChanged -= databasesBackupRestoreHistoryView_HistoryGridGroupByBoxVisibleChanged;
                    databasesBackupRestoreHistoryView.Dispose();
                }
                if (databasesTablesIndexesView != null)
                {
                    databasesTablesIndexesView.SystemDatabasesFilterChanged -= databasesTablesIndexesView_SystemDatabasesFilterChanged;
                    databasesTablesIndexesView.SystemTablesFilterChanged -= databasesTablesIndexesView_SystemTablesFilterChanged;
                    databasesTablesIndexesView.DetailsPanelVisibleChanged -= databasesTablesIndexesView_DetailsPanelVisibleChanged;
                    databasesTablesIndexesView.GridGroupByBoxVisibleChanged -= databasesTablesIndexesView_GridGroupByBoxVisibleChanged;
                    databasesTablesIndexesView.ActionsAllowedChanged -= databasesTablesIndexesView_ActionsAllowedChanged;
                    databasesTablesIndexesView.Dispose();
                }
                if (databasesMirroringView != null)
                {
                    databasesMirroringView.MirroredDatabasesGridGroupByBoxVisibleChanged -= databasesMirroringView_MirroredDatabasesGridGroupByBoxVisibleChanged;
                    databasesMirroringView.MirroringHistoryGridGroupByBoxVisibleChanged -= databasesMirroringView_MirroringHistoryGridGroupByBoxVisibleChanged;
                    databasesMirroringView.ActionsAllowedChanged -= databasesMirroringView_ActionsAllowedChanged;
                    databasesMirroringView.Dispose();
                }
                if (databasesFilesView != null)
                {
                    databasesFilesView.FilterChanged -= databasesFilesView_FilterChanged;
                    databasesFilesView.GridGroupByBoxVisibleChanged -= databasesFilesView_GridGroupByBoxVisibleChanged;
                    databasesFilesView.Dispose();
                }
                if (databasesTempdbView != null)
                {
                    databasesTempdbView.HistoricalSnapshotDateTimeChanged -= databasesTempdbView_HistoricalSnapshotDateTimeChanged;
                    databasesTempdbView.GridGroupByBoxVisibleChanged -= databasesTempdbView_GridGroupByBoxVisibleChanged;
                    databasesTempdbView.FilterChanged -= databasesTempdbView_FilterChanged;
                    databasesTempdbView.ChartDrilldown -= View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                    databasesTempdbView.Dispose();
                }
                if (servicesSummaryView != null)
                {
                    servicesSummaryView.ServiceActionAllowedChanged -= servicesSummaryView_ServiceActionAllowedChanged;
                    servicesSummaryView.GridGroupByBoxVisibleChanged -= servicesSummaryView_GridGroupByBoxVisibleChanged;
                    servicesSummaryView.ChartPanelVisibleChanged -= servicesSummaryView_ChartPanelVisibleChanged;
                    servicesSummaryView.Dispose();
                }
                if (servicesSqlAgentJobsView != null)
                {
                    servicesSqlAgentJobsView.ServiceActionAllowedChanged -= servicesSqlAgentJobsView_ServiceActionAllowedChanged;
                    servicesSqlAgentJobsView.FilterChanged -= servicesSqlAgentJobsView_FilterChanged;
                    servicesSqlAgentJobsView.HistoryPanelVisibleChanged -= servicesSqlAgentJobsView_HistoryPanelVisibleChanged;
                    servicesSqlAgentJobsView.GridGroupByBoxVisibleChanged -= servicesSqlAgentJobsView_GridGroupByBoxVisibleChanged;
                    servicesSqlAgentJobsView.JobSelectionChanged -= servicesSqlAgentJobsView_JobSelectionChanged;
                    servicesSqlAgentJobsView.UpdateDataCompleted -= servicesSqlAgentJobsView_UpdateDataCompleted;
                    servicesSqlAgentJobsView.Dispose();
                }
                if (servicesFullTextSearchView != null)
                {
                    servicesFullTextSearchView.ServiceActionsAllowedChanged -= servicesFullTextSearchView_ServiceActionsAllowedChanged;
                    servicesFullTextSearchView.CatalogActionsAllowedChanged -= servicesFullTextSearchView_CatalogActionsAllowedChanged;
                    servicesFullTextSearchView.GridGroupByBoxVisibleChanged -= servicesFullTextSearchView_GridGroupByBoxVisibleChanged;
                    servicesFullTextSearchView.DetailsPanelVisibleChanged -= servicesFullTextSearchView_DetailsPanelVisibleChanged;
                    servicesFullTextSearchView.Dispose();
                }
                if (servicesReplicationView != null)
                {
                    servicesReplicationView.FilterChanged -= servicesReplicationView_FilterChanged;
                    servicesReplicationView.TopologyGridGroupByBoxVisibleChanged -= servicesReplicationView_PublisherGridGroupByBoxVisibleChanged;
                    servicesReplicationView.DistributorGridGroupByBoxVisibleChanged -= servicesReplicationView_DistributorGridGroupByBoxVisibleChanged;
                    servicesReplicationView.ReplicationGraphVisibleChanged -= servicesReplicationView_ReplicationGraphsVisibleChanged;
                    servicesReplicationView.Dispose();
                }
                if (logsView != null)
                {
                    logsView.FilterChanged -= logsView_FilterChanged;
                    logsView.AvailableLogsPanelVisibleChanged -= logsView_AvailableLogsPanelVisibleChanged;
                    logsView.DetailsPanelVisibleChanged -= logsView_DetailsPanelVisibleChanged;
                    logsView.GridGroupByBoxVisibleChanged -= logsView_GridGroupByBoxVisibleChanged;
                    logsView.Dispose();
                }
                //10.0 srishti
                if (recommendationAnalysis != null)
                {
                    recommendationAnalysis.ScriptAnalysisActionAllowedChanged -= new EventHandler(scriptAnalysisView_AnalysisActionAllowedChanged);

                    recommendationAnalysis.Dispose();
                }

                if (_dashboardLayoutGallery != null)
                {
                    _dashboardLayoutGallery.SelectionChanged -= dashboardLayoutGalleryView_SelectionChanged;
                    _dashboardLayoutGallery.Dispose();
                }

                var parent = Parent as System.Windows.Controls.Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }

                if (activeInstance != null)
                {
                    activeInstance.Changed -= OnInstanceChanged;
                    activeInstance = null;
                }

                if (historyBrowserBackgroundWorker != null && !historyBrowserBackgroundWorker.IsBusy)
                {
                    historyBrowserBackgroundWorker.DoWork -= historyBrowserBackgroundWorker_DoWork;
                    historyBrowserBackgroundWorker.RunWorkerCompleted -= historyBrowserBackgroundWorker_RunWorkerCompleted;
                    historyBrowserBackgroundWorker.Dispose();
                }

                historyBrowserControl.HistoricalSnapshotSelected -= historyBrowserControl_HistoricalSnapshotSelected;
                historyBrowserControl.HistoricalSnapshotSelected -= QuickHistorialSnapshotTextChange;
                historyBrowserControl.HistoricalCustomRangeSelected -= historyBrowserControl_HistoricalCustomRangeSelected;
                historyBrowserControl.Dispose();

                //this is removing the keyboard hook from the WindowsFormHost
                var ikis = historyBrowserControlHost as System.Windows.Interop.IKeyboardInputSink;

                if (ikis != null)
                {
                    var kis = ikis.KeyboardInputSite;
                    if (kis != null)
                        kis.Unregister();
                }
                historyBrowserControlHost.Dispose();
                viewHost.Dispose();
            }
            _disposed = true;
        }

        public bool IsDisposed { get { return _disposed; } }

        #endregion

        void OnSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ColorScheme":
                    UpdateTheme();
                    OnRibbonLoaded(ribbon, null);
                    break;
            }
        }

        void ResetInstancesCombobox()
        {
            if(!this.IsVisible && (InstanceComboBox == null || activeInstance == null || !string.Equals(InstanceComboBox.Text, activeInstance.InstanceName)))
            {
                InstanceComboBox.Text = string.Empty;
            }
            InstanceComboBox.ItemsSource = activeInstances;
            InstanceComboBox.SelectedIndex = Array.FindIndex(activeInstances, x => x.Id == instanceId);            
        }

        void UpdateInstanceTitle()
        {
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                //Check for Azure cloud and assign the image acordingly.
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                {
                    ImageSource objImg = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/16x16/CloudServer/cloud_database_icon_normal_16x16.png", UriKind.RelativeOrAbsolute));
                    TitleImage.Source = objImg;
                    TitleImage.Visibility = Visibility.Visible;
                }
                else
                {
                    ImageSource objImg = WpfHelpers.GetImageSource(new Uri("/SQLdmDesktopClient;component/Resources/16x16/Server.png", UriKind.RelativeOrAbsolute));
                    TitleImage.Source = objImg;
                    TitleImage.Visibility = Visibility.Visible;
                }                
            }
        }


        private void UpdateTheme()
        {
            Brush brush = null;

            var colorScheme = Settings.Default.ColorScheme;
            if (String.IsNullOrEmpty(colorScheme) || colorScheme.Contains("Silver"))
            {
                brush = RibbonOffice2k7SilverBrushes.Instance[RibbonBrushKeys.RibbonGroupCaptionTextFillKey] as Brush;
            }
            else
            {
                brush = RibbonOffice2k7BlackBrushes.Instance[RibbonBrushKeys.RibbonGroupCaptionTextFillKey] as Brush;
            }

            if (brush != null)
            {
                //titleLabel.Foreground = brush;
                historyTimestampLabel.Foreground = brush;
            }
        }

        private void OnServerViewContainerLoaded(object sender, RoutedEventArgs e)
        {
            UpdateHistoryBrowserVisibility();
        }

        private void OnRibbonLoaded(object sender, RoutedEventArgs e)
        {
            // hide scenic ribbon caption area and application menu presenter
            var srca = Utilities.GetDescendantFromType((DependencyObject)sender, typeof(ScenicRibbonCaptionArea), true) as ScenicRibbonCaptionArea;
            if (srca != null) srca.Visibility = Visibility.Collapsed;
            var ams = Utilities.GetDescendantFromName((DependencyObject)sender, "PART_ApplicationMenuSite");
            if (ams != null) ams.Visibility = Visibility.Collapsed;
        }

        public ServerViewMode ViewMode
        {
            get { return viewMode; }
            set
            {
                if (viewMode != value)
                {
                    viewMode = value;
                    bool changed = ApplicationModel.Default.HistoryTimeValue.SetServerViewMode(value);
                    if (changed)
                    {
                        ApplicationController.Default.PersistUserSettings(); // Persist user settings on background thread.
                    }
                }
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

        #region IView Members

        public virtual void UpdateTheme(ThemeName theme)
        {
            var v = viewHost.Content as IView;

            if (v != null)
                v.UpdateTheme(theme);

        }

        public BBS.TracerX.Logger Log
        {
            get { return null; }
        }

        public void SetArgument(object argument)
        {
            // the server view doesn't have arguments
            // hosted views have their argument set at the time they are shown
        }

        public void SaveSettings()
        {
            if (activeView != null)
            {
                if (activeView is IServerDesignView)
                {
                    IServerDesignView designView = activeView as IServerDesignView;
                    designView.CheckIfSaveNeeded();
                    designView.ToggleDesignMode(false);
                    if (!string.IsNullOrEmpty(designView.DesignTab))
                    {
                        var tab = (RibbonTabItem)ribbon.FindName(designView.DesignTab);
                        if (tab != null && tab != ribbon.SelectedTab)
                        {
                            tab.Visibility = Visibility.Collapsed;
                            tab.IsSelected = false;
                        }

                    }
                }
                activeView.SaveSettings();
            }
        }

        public void ShowHelp()
        {
            if (activeView == null)
            {
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName, HelpNavigator.TableOfContents);
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName);
            }
            else
            {
                activeView.ShowHelp();
            }
        }

        #endregion

        public ServerViewTabs SelectedTab
        {
            get
            {
                return (ServerViewTabs)ribbon.Tabs.IndexOf(ribbon.SelectedTab);
            }
            set
            {
                ribbon.SelectedTab = ribbon.Tabs[(int)value];
            }
        }

        /// <summary>
        /// Override this property to manage the historical snapshot start timestamp
        /// </summary>
        public virtual DateTime? HistoricalStartDateTime
        {
            get { return currentHistoricalStartDateTime; }
            set
            {
                if (currentHistoricalStartDateTime != value)
                {
                    currentHistoricalStartDateTime = value;
                }
            }
        }

        /// <summary>
        /// The date time on which has got the snapshoot.
        /// </summary>
        public DateTime? HistoricalSnapshotDateTime
        {
            get
            {
                return currentHistoricalSnapshotDateTime;
            }
            set { SetHistoricalSnapshotDateTime(value); }
        }

        /// <summary>
        /// Sets HistoricalSnapshotDateTime, ViewMode, LoadingText.
        /// </summary>
        private void SetHistoricalSnapshotDateTime(DateTime? value, bool refreshView = true)
        {
            if (ViewMode != ServerViewMode.Historical || currentHistoricalSnapshotDateTime != value)
            {
                // Clear snapshot selection if user switches to real time mode or does prev/next snapshot
                if (!historyBrowserSnapshotChanged)
                    historyBrowserControl.ClearSnapshotSelection();

                currentHistoricalStartDateTime = null;
                currentHistoricalSnapshotDateTime = value;
                ApplicationModel.Default.HistoryTimeValue.SetHistoricalSnapshotDateTime(currentHistoricalSnapshotDateTime);
                var setHistoryElementsLive = false;
                if (currentHistoricalSnapshotDateTime.HasValue)
                {
                    historyTimestampLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                    _viewModel.QuickHistoricalSnapshotText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(currentHistoricalSnapshotDateTime.Value, 
                                                                                                                    currentHistoricalSnapshotDateTime.Value);
                    SetViewMode(ServerViewMode.Historical, refreshView);
                }
                else
                {
                    setHistoryElementsLive = historySelectionCombobox.SelectedItem == LiveHistoryComboBoxItem;
                    historyTimestampLabel.Text = String.Empty;
                    _viewModel.QuickHistoricalSnapshotText = DateTimeHelper.GetRealTimeDateQuickHistoricalSnapshots();
                    SetViewMode(ServerViewMode.RealTime, refreshView);
                }
                syncViewHistoryToCurrentHistoryBrowser(setHistoryElementsLive);
            }
        }

        /// <summary>
        /// Sets Historical Start and End DateTime, ViewMode, LoadingText.
        /// </summary>
        private void SetCustomStartEndDateTime(DateTime? startDateTime, DateTime? endDateTime, bool refreshView = true)
        {
            if (ViewMode != ServerViewMode.Custom || currentHistoricalStartDateTime != startDateTime ||
                currentHistoricalSnapshotDateTime != endDateTime)
            {
                historyBrowserControl.ClearSnapshotSelection();
                currentHistoricalStartDateTime = startDateTime;
                currentHistoricalSnapshotDateTime = endDateTime;
                if (currentHistoricalSnapshotDateTime.HasValue)
                {
                    historyTimestampLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                    SetViewMode(ServerViewMode.Custom, refreshView);
                }
                else
                {
                    historyTimestampLabel.Text = String.Empty;
                    SetViewMode(ServerViewMode.RealTime, refreshView);
                }
                syncViewHistoryToCurrentHistoryBrowser();
            }
        }

        /// <summary>
        /// Set scale, view mode and refreshes view.
        /// </summary>
        private void SetScale(int scale, bool refreshView = true)
        {
            if (ViewMode != ServerViewMode.RealTime || scale != currentScale)
            {
                historyBrowserControl.ClearSnapshotSelection();
                currentScale = scale;
                currentHistoricalStartDateTime = currentHistoricalSnapshotDateTime = null;
                historyTimestampLabel.Text = String.Empty;
                SetViewMode(ServerViewMode.RealTime, refreshView);
                syncViewHistoryToCurrentHistoryBrowser();
            }
            else if (!refreshView)
                syncViewHistoryToCurrentHistoryBrowser();
        }

        private void OnRibbonTabItemSelected(object sender, RibbonTabItemSelectedEventArgs e)
        {
            //update breadcrumb with selected instance
            if (_viewModel != null && e.NewSelectedRibbonTabItem != null && e.NewSelectedRibbonTabItem.Name != null)
            {
                _viewModel.ParentBreadcrumb = e.NewSelectedRibbonTabItem.Name;
                parentBreadcrumb = e.NewSelectedRibbonTabItem.Name;
            }
            else
            {
                parentBreadcrumb = e.NewSelectedRibbonTabItem.Name;
            }
                        
            SetSelectedTabDelegate post = SetSelectedTab;
            Dispatcher.BeginInvoke(post, DispatcherPriority.Normal, e.NewSelectedRibbonTabItem);
        }

        private void OnToolChecked(object sender, RoutedEventArgs e)
        {
            OnToolClick(e.OriginalSource, e);
        }
        void OnClickLaunchSWA(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(swaLaunchURL);
        }

        private void OnToolClick(object sender, RoutedEventArgs e)
        {
            string id = string.Empty;
            System.Windows.Controls.Primitives.ToggleButton toggle = null;
            if (sender is System.Windows.Controls.TabControl)
            {
                var tabItem = (((System.Windows.Controls.TabControl)sender).SelectedItem as System.Windows.Controls.TabItem);
                id = tabItem.Name;
                var tabControlParent = ((System.Windows.Controls.TabControl)sender).Parent as System.Windows.Controls.StackPanel;
                if(tabControlParent != null && tabControlParent.Name.Contains("TabControlPanel"))
                {
                    selectedServerTabTag = tabItem.Tag.ToString();
                }
            }
            else if(sender is System.Windows.Controls.Primitives.ToggleButton)
            {
                toggle = sender as System.Windows.Controls.Primitives.ToggleButton;
                id = toggle.Name;
            }
            else if (sender is System.Windows.Controls.Button)
            {
                var button = sender as System.Windows.Controls.Button;
                id = button.Name;
            }
            else if(sender is System.Windows.Controls.MenuItem)
            {
                var menuitem = sender as System.Windows.Controls.MenuItem;
                id = menuitem.Name;
                if(id== "showSessionsBlockingBySessionButton")
                {
                    _viewModel.ShowBtByLock = false;
                    _viewModel.ShowBtBySession = true;
                }
                else if(id== "showSessionsBlockingByLockButton")
                {
                    _viewModel.ShowBtByLock = true;
                    _viewModel.ShowBtBySession = false;
                }
            }
            else
            {
                id = ((IRibbonTool)sender).GetId();
            }
            
            if (!IsInitialized) return;

            if (ignoreToolClick) return;

            _viewModel.UpdateBindingProperties();

            selectedRibbonMenu = id;
            if (String.IsNullOrEmpty(id)) return;
            
            var isChecked = false;
            if (toggle != null && toggle.IsChecked.HasValue)
            {
                isChecked = toggle.IsChecked.Value;
            }
            if (id.Equals("toggleHistoryBrowserButton"))
            {
                ApplicationController.Default.IsFromHistorySnapshot = isChecked;
            }
            
            historyBrowserControl.RefreshHistoryPaneTimer();

            viewHistoryControls.Visibility = Visibility.Visible;
            customDashboardControls.Visibility = Visibility.Collapsed;

            switch (id)
            {
                case "toggleCustomDashboard":
                    if (isChecked)
                        EnableDisableCustomizeDashboard(true);
                    else
                        EnableDisableCustomizeDashboard(false);

                    break;
                case "stopServerButton":
                    StopSqlServer();
                    break;
                case "toggleHistoryBrowserButton":
                    ToggleHistoryBrowser();
                    if (activeView == defaultAnalysisView && _viewModel.HistoryBrowserVisible)
                    {
                        recommendationAnalysis = new Analysis.Recommendations(instanceId, null);
                        defaultAnalysisView = null;
                        ShowView(recommendationAnalysis);
                        SetButtonStateOfAnalysisView(true);
                    }
                    break;
                case "showPreviousSnapshotButton":
                    ApplicationController.Default.IsNextPrevious = true;
                    _viewModel.DrillOutButtonVisible = true;
                    ShowPreviousHistoricalSnapshot();
                    if (activeView == defaultAnalysisView)
                    {
                        recommendationAnalysis = new Analysis.Recommendations(instanceId, currentHistoricalSnapshotDateTime == null ? DateTime.Now : currentHistoricalSnapshotDateTime.Value);
                        defaultAnalysisView = null;
                        HistoricalSnapshotDateTime = currentHistoricalSnapshotDateTime == null ? DateTime.Now : currentHistoricalSnapshotDateTime.Value;
                        ShowView(recommendationAnalysis);
                        SetButtonStateOfAnalysisView(true);
                        SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", true);
                        UpdateAnalyzeGroups();
                        UpdatePulseGroup();
                        recommendationAnalysis.ScriptAnalysisActionAllowedChanged += new EventHandler(scriptAnalysisView_AnalysisActionAllowedChanged);
                        OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
                    }
                    else if (activeView == recommendationAnalysis)
                    {
                        SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", true);
                    }
                    break;
                case "showNextSnapshotButton":
                    ApplicationController.Default.IsNextPrevious = true;
                    _viewModel.DrillOutButtonVisible = true;
                    ShowNextHistoricalSnapshot();
                    if (activeView == defaultAnalysisView)
                    {
                        recommendationAnalysis = new Analysis.Recommendations(instanceId, currentHistoricalSnapshotDateTime.Value);
                        defaultAnalysisView = null;
                        HistoricalSnapshotDateTime = currentHistoricalSnapshotDateTime.Value;
                        ShowView(recommendationAnalysis);
                        SetButtonStateOfAnalysisView(true);
                        UpdateAnalyzeGroups();
                        UpdatePulseGroup();
                        OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
                    }

                    break;
                case "maintenanceModeButton":
                    ToggleMaintenanceMode();
                    break;
                case "baselineAssistantButton":
                    ShowBaselineAssistant();
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
                case "overviewTabTimelineViewButton":
                    ShowServerTimelineView();
                    break;
                case "overviewTabDesignButton":
                    ShowDashboardDesignView();
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
                    //Making first snapshot as default view for Session Blocking View -  Ankit Nagpal SQLDM 10.0.0
                    //ShowPreviousHistoricalSnapshot();//SQLdm 10.0 (Tarun Sapra)- Removed the above requirement
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
                case "toggleSessionsBlocksDeadlocksListButton":
                    ToggleSessionsBlocksDeadlocksListButton();
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
                case "resourcesTabDiskSizeViewButton":  // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
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
                case "toggleDiskSizeViewGroupByBoxButton":   // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --group by box toggling functionality in disk size view
                    ToggleDiskSizeGroupByBox();
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
                case "toggleDatabasesAlwaysOnChartsButton":
                    ToggleDatabasesAlwaysOnCharts();
                    break;
                case "toggleDatabasesAlwaysOnGroupByBoxButton":
                    ToggleDatabasesAlwaysOnGroupByBox();
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
                case "databasesTabAlwaysOnViewButton":
                    ShowDatabasesAlwaysOnView(true);
                    break;
                case "databasesTabMirroringViewFailOverButton":
                    MirroringFailover();
                    break;
                case "databasesTabMirroringViewPauseButton":
                    MirroringSuspendResume();
                    break;
                case "queriesActiveWaitsConfigureTopXButton":
                    ShowQueryWaitsActiveFilterDialog();
                    break;
                case "queriesActiveWaitsConfigureButton":
                    ShowQueriesWaitStatsActiveView();
                    break;
                case "filterDatabasesTempdbSessionsButton":
                    ShowDatabasesTempdbFilterDialog();
                    break;
                case "toggleDatabasesTempdbGroupByBoxButton":
                    ToggleDatabasesTempdbGroupByBox();
                    break;
                case "previousDashboardButton":
                    break;
                case "nextDashboardButton":
                    break;
                case "showDefaultDashboardsButton":
                    ToggleDashboardFilter(DashboardFilter.Default);
                    break;
                case "showUserDashboardsButton":
                    ToggleDashboardFilter(DashboardFilter.User);
                    break;
                case "showAllDashboardsButton":
                    ToggleDashboardFilter(DashboardFilter.All);
                    break;
                case "deleteDashboardButton":
                    DeleteDashboard();
                    break;
                case "selectDashboardButton":
                    SelectDashboardGalleryView();
                    break;
                case "closeDashboardGalleryButton":
                    CloseDashboardGalleryView();
                    break;
                case "dashboardGalleryButton":
                    viewHistoryControls.Visibility = Visibility.Collapsed;
                    customDashboardControls.Visibility = Visibility.Collapsed;
                    _viewModel.ShowDashGalleryButtons = true;
                    ShowDashboardLayoutView();
                    break;
                case "saveDashboardButton":
                    SaveDashboardDesign();
                    break;
                case "setMyDefaultDashboardButton":
                    SetDashboardAsGlobalDefault();
                    break;
                case "setServerDefaultDashboardButton":
                    SetDashboardAsServerDefault();
                    break;
                case "closeDesignerButton":
                    HideDashboardDesignView();
                    break;
                case "togglePanelGalleryButton":
                    viewHistoryControls.Visibility = Visibility.Collapsed;
                    customDashboardControls.Visibility = Visibility.Visible;
                    TogglePanelGallery();
                    break;
                case "pulseProfileButton":
                    ShowPulseProfile();
                    break;
                case "pulseFollowButton":
                    TogglePulseFollow();
                    break;
                case "pulsePostButton":
                    ShowPulsePost();
                    break;

                //SQLdm 10.0 srishti purohit implementing doctor's UI (code for run anlysis button click)
                case "analysisRunAnalysisButton":
                    ////if (recommendationAnalysis == null)
                    ////{
                    //recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                    //defaultAnalysisView = null;
                    ////}
                    //HistoricalSnapshotDateTime = null;
                    //ShowView(recommendationAnalysis);
                    //SetButtonStateOfAnalysisView(true);
                    //SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", true);
                    //UpdateAnalyzeGroups();
                    //UpdatePulseGroup();
                    //recommendationAnalysis.ScriptAnalysisActionAllowedChanged += new EventHandler(scriptAnalysisView_AnalysisActionAllowedChanged);

                    //scriptAnalysisView_AnalysisActionAllowedChanged(this, EventArgs.Empty);
                    //OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
                    ShowRunAnalysisWizard(activeInstance);
                    break;

                //SQLdm 10.0 srishti purohit implementing doctor's UI (code for workload anlysis button click)
                case "analysisWorkloadAnalysisButton":
                    //if (recommendationAnalysis == null)
                    //{
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, true);
                    defaultAnalysisView = null;
                    //}
                    HistoricalSnapshotDateTime = null;
                    ShowView(recommendationAnalysis);
                    SetButtonStateOfAnalysisView(true);
                    SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", true);
                    UpdateAnalyzeGroups();
                    UpdatePulseGroup();
                    recommendationAnalysis.ScriptAnalysisActionAllowedChanged += new EventHandler(scriptAnalysisView_AnalysisActionAllowedChanged);

                    scriptAnalysisView_AnalysisActionAllowedChanged(this, EventArgs.Empty);
                    OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));

                    break;
                case "drillOutButton":
                    ShowAnalysisDefaultView();
                    break;
                case "analyzeTabCopyButton":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.CopyRecommendations();
                    }
                    break;
                case "analyzeTabExportButton":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.ExportSelectedRecommendations();
                    }
                    break;
                case "analyzeTabBlockButton":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.BlockRecommendations(instanceId);
                    }
                    break;
                case "analyzeTabEMailButton":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.EmailRecommendations();
                    }
                    break;
                case "analyzeTabUndoScript":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.UndoScriptRecommendations();
                    }
                    break;
                case "analyzeTabOptimizeScript":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.OptimizeScriptRecommendations();
                    }
                    break;
                case "analyzeTabShowProblem":
                    if (defaultAnalysisView == null)
                    {
                        if (recommendationAnalysis == null)
                        {
                            recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                        }
                        recommendationAnalysis.ShowProblemRecommendations();
                    }
                    break;
                case "refreshView":
					//SQLDM-31025
                    if (ApplicationController.Default.IsAutomaticRefreshPaused)
                    {
                        pauseAutoRefreshView.IsEnabled = true;
                        ApplicationController.Default.EnableAutomaticRefresh();
                    }
                    if(viewMode == ServerViewMode.Custom)
                    {
                        if(!currentHistoricalStartDateTime.HasValue || !DateTime.Equals(currentHistoricalStartDateTime.Value, _viewModel.StartHistoryDateTime))
                        {
                            currentHistoricalStartDateTime = _viewModel.StartHistoryDateTime;
                        }
                        if (!currentHistoricalSnapshotDateTime.HasValue || !DateTime.Equals(currentHistoricalSnapshotDateTime.Value, _viewModel.StartHistoryDateTime))
                        {
                            currentHistoricalSnapshotDateTime = _viewModel.EndHistoryDateTime;
                        }
                    }
                    ApplicationController.Default.RefreshActiveView(false);
                    break;
                case "pauseAutoRefreshView"://SQLDM-31025
                    ApplicationController.Default.PauseAutomaticRefresh();
                    pauseAutoRefreshView.IsEnabled = false;
                    break;

            }
            
            e.Handled = true;
        }

        private void EnableDisableCustomizeDashboard(bool IsEnable)
        {
            Visibility dashBoradControlVisible = Visibility.Visible;
            Visibility otherControlVisible = Visibility.Collapsed;

            if (!IsEnable)
            {
                dashBoradControlVisible = Visibility.Collapsed;
                otherControlVisible = Visibility.Visible;
            }

            customDashboardControls.Visibility = dashBoradControlVisible;
            CustomizeDashboardText.Visibility = dashBoradControlVisible;
            viewHistoryControls.Visibility = otherControlVisible;
            serverControls.Visibility = otherControlVisible;
            serverMenuControls.Visibility = otherControlVisible;

            var parentWindow = (MainWindow) Window.GetWindow(this);
            parentWindow.LeftNavPane.Visibility = otherControlVisible;
        }

        //Code Added by Satyajit 05-12-2018 to display Analysis Wizard.
        private void ShowRunAnalysisWizard(MonitoredSqlServer instance)
        {
            try
            {
                Form dialog = RunAnalysisWizard.GetAnalysisForm(instance, viewHost);
                dialog.ShowDialog(this.GetWinformWindow());
            }
            catch (Exception ex)
            {
                Log.Error("Error while opening RunAnalysisWizard.", ex);
            }
        }

        private void OnGalleryItemClicked(object sender, GalleryItemEventArgs e)
        {
            switch (e.Item.Key)
            {
                case "DashboardLayoutTwoByTwo":
                    SelectDashboardLayout(new Size(2, 2));
                    break;
                case "DashboardLayoutTwoByThree":
                    SelectDashboardLayout(new Size(2, 3));
                    break;
                case "DashboardLayoutTwoByFour":
                    SelectDashboardLayout(new Size(2, 4));
                    break;
                case "DashboardLayoutTwoByFive":
                    SelectDashboardLayout(new Size(2, 5));
                    break;
                case "DashboardLayoutThreeByTwo":
                    SelectDashboardLayout(new Size(3, 2));
                    break;
                case "DashboardLayoutThreeByThree":
                    SelectDashboardLayout(new Size(3, 3));
                    break;
                case "DashboardLayoutThreeByFour":
                    SelectDashboardLayout(new Size(3, 4));
                    break;
                case "DashboardLayoutFourByTwo":
                    SelectDashboardLayout(new Size(4, 2));
                    break;
                case "DashboardLayoutFourByThree":
                    SelectDashboardLayout(new Size(4, 3));
                    break;
            }
        }

        public IServerView ActiveView
        {
            get { return activeView; }
        }

        public void ShowActiveViewHelp()
        {
            if (activeView == null)
            {
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName, HelpNavigator.TableOfContents);
                Help.ShowHelp(new Form(), Idera.SQLdm.Common.Constants.HelpFileName);
            }
            else
            {
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

        private bool QueryConfigurationMonitorDialogVisible
        {
            get { return queryMonitorView != null && queryMonitorView.ConfigurationPropertiesDialogVisible && (_viewModel.SelectedServerTab == ServerViews.Queries || _viewModel.SelectedServerTab == ServerViews.QueriesHistory || _viewModel.SelectedServerTab == ServerViews.QueriesSignatures || _viewModel.SelectedServerTab == ServerViews.QueriesStatements); }
        }

        private bool QueryWaitsConfigurationDialogVisible
        {
            get { return queriesWaitStatsActiveView != null && queriesWaitStatsActiveView.ConfigurationDialogVisible && _viewModel.SelectedServerTab == ServerViews.QueryWaitStatsActive; }
        }

        public bool IgnoreActiveInstancesChanging { get { return QueryConfigurationMonitorDialogVisible || QueryWaitsConfigurationDialogVisible; } }

        public void ShowView(ServerViews view, object argument)
        {
            if (view == ServerViews.Overview && (QueryConfigurationMonitorDialogVisible || QueryWaitsConfigurationDialogVisible))
                return;
            RefreshInstanceState();
            SaveViewSettings();

            _viewModel.SelectedServerTab = view;

            switch (view)
            {
                case ServerViews.Overview:
                case ServerViews.OverviewSummary:
                    ShowView(ServerViewTabs.Overview, "overviewTabSummaryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Overview;
                    break;
                case ServerViews.OverviewDetails:
                    ShowView(ServerViewTabs.Overview, "overviewTabDetailsViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Overview;
                    break;
                case ServerViews.OverviewConfiguration:
                    ShowView(ServerViewTabs.Overview, "overviewTabConfigurationViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Overview;
                    break;
                case ServerViews.OverviewAlerts:
                    ShowView(ServerViewTabs.Overview, "overviewTabAlertsViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Overview;
                    break;
                case ServerViews.Sessions:
                case ServerViews.SessionsSummary:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabSummaryViewButton");
                    SelectedTab = ServerViewTabs.Sessions;
                    _viewModel.SelectedSubTab = ServerViewTabs.Sessions;
                    break;
                case ServerViews.SessionsDetails:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabDetailsViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Sessions;
                    break;
                case ServerViews.SessionsLocks:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabLocksViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Sessions;
                    break;
                case ServerViews.SessionsBlocking:
                    ShowView(ServerViewTabs.Sessions, "sessionsTabBlockingViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Sessions;
                    break;
                case ServerViews.Queries:
                //SelectedTab = ServerViewTabs.Queries;
                //break;
                case ServerViews.QueriesSignatures:
                    ShowView(ServerViewTabs.Queries, "queriesTabSignatureModeViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Queries;
                    break;
                case ServerViews.QueriesStatements:
                    ShowView(ServerViewTabs.Queries, "queriesTabStatementModeViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Queries;
                    break;
                case ServerViews.QueriesHistory:
                    ShowView(ServerViewTabs.Queries, "queriesTabHistoryModeViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Queries;
                    break;
                case ServerViews.Resources:
                    SelectedTab = ServerViewTabs.Resources;
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesSummary:
                    ShowView(ServerViewTabs.Resources, "resourcesTabSummaryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesCpu:
                    ShowView(ServerViewTabs.Resources, "resourcesTabCpuViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesMemory:
                    ShowView(ServerViewTabs.Resources, "resourcesTabMemoryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesDisk:
                    ShowView(ServerViewTabs.Resources, "resourcesTabDiskViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesDiskSize:           // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
                    ShowView(ServerViewTabs.Resources, "resourcesTabDiskSizeViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesProcedureCache:
                    ShowView(ServerViewTabs.Resources, "resourcesTabProcedureCacheViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.ResourcesWaitStats:
                    ShowView(ServerViewTabs.Resources, "resourcesTabWaitStatsViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.QueryWaitStatsActive:
                    ShowView(ServerViewTabs.Queries, "resourcesTabWaitStatsActiveViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Queries;
                    break;
                case ServerViews.ResourcesFileActivity:
                    ShowView(ServerViewTabs.Resources, "resourcesTabFileActivityViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Resources;
                    break;
                case ServerViews.Databases:
                    SelectedTab = ServerViewTabs.Databases;
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesSummary:
                    ShowView(ServerViewTabs.Databases, "databasesTabSummaryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabaseAlwaysOn:
                    ShowView(ServerViewTabs.Databases, "databasesTabAlwaysOnViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesConfiguration:
                    ShowView(ServerViewTabs.Databases, "databasesTabConfigurationViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesBackupRestoreHistory:
                    ShowView(ServerViewTabs.Databases, "databasesTabBackupRestoreHistoryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesTablesIndexes:
                    ShowView(ServerViewTabs.Databases, "databasesTabTablesIndexesViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesFiles:
                    ShowView(ServerViewTabs.Databases, "databasesTabFilesViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesMirroring:
                    ShowView(ServerViewTabs.Databases, "databasesTabMirroringViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.DatabasesTempdbView:
                    ShowView(ServerViewTabs.Databases, "databasesTabTempdbSummaryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Databases;
                    break;
                case ServerViews.Services:
                    SelectedTab = ServerViewTabs.Services;
                    _viewModel.SelectedSubTab = ServerViewTabs.Services;
                    break;
                case ServerViews.ServicesSummary:
                    ShowView(ServerViewTabs.Services, "servicesTabSummaryViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Services;
                    break;
                case ServerViews.ServicesSqlAgentJobs:
                    ShowView(ServerViewTabs.Services, "servicesTabSqlAgentJobsViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Services;
                    break;
                case ServerViews.ServicesFullTextSearch:
                    ShowView(ServerViewTabs.Services, "servicesTabFullTextSearchViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Services;
                    break;
                case ServerViews.ServicesReplication:
                    ShowView(ServerViewTabs.Services, "servicesTabReplicationViewButton");
                    _viewModel.SelectedSubTab = ServerViewTabs.Services;
                    break;
                case ServerViews.Logs:
                    SelectedTab = ServerViewTabs.Logs;
                    _viewModel.SelectedSubTab = ServerViewTabs.Logs;
                    break;
                case ServerViews.Analysis:
                case ServerViews.RunAnalysis:
                    SelectedTab = ServerViewTabs.Analysis;
                    _viewModel.SelectedSubTab = ServerViewTabs.Analysis;
                    ShowAnalysisView(false);
                    break;
            }

            
            if (activeView != null)
            {
                activeView.SetArgument(argument);

                if (activeView.HistoricalSnapshotDateTime.HasValue)
                {
                    ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
                }
                // SQLdm 10.1.3 - Varun Chopra -- SQLDM-19887 -- refresh the active view
                activeView.RefreshView();
            }
        }

        public void SetQuickHistoricalSnapshotVisible(bool isVisbleForSize)
        {
            _viewModel.QuickHistoricalSnapshotVisible = isVisbleForSize;
        }

        private void ShowView(ServerViewTabs tab, string viewButtonKey)
        {
            if (!string.IsNullOrEmpty(viewButtonKey))
            {
                string toolid = null;
                _viewModel.SelectedSubTab = tab;
                System.Windows.Controls.TabControl currentTabControl = null;
                switch (tab)
                {
                    case ServerViewTabs.Overview:
                        
                        toolid = "overviewTabViewsGroup_";
                        currentTabControl = overviewTabControl;
                        break;
                    case ServerViewTabs.Sessions:
                        toolid = "sessionsTabViewsGroup_";
                        currentTabControl = sessionsTabControl;
                        break;
                    case ServerViewTabs.Databases:
                        toolid = "databasesTabViewsGroup_";
                        currentTabControl = databaseTabControl;
                        break;
                    case ServerViewTabs.Resources:
                        toolid = "resourcesTabViewsGroup_";
                        currentTabControl = resourcesTabControl;
                        break;
                    case ServerViewTabs.Services:
                        toolid = "servicesTabViewsGroup_";
                        currentTabControl = servicesTabControl;
                        break;
                    case ServerViewTabs.Logs:
                        toolid = "logsTabFilterGroup_";
                        break;
                    case ServerViewTabs.Queries:
                        toolid = "queriesTabViewsGroup_";
                        currentTabControl = queriesTabControl;
                        break;
                    //10.0 SQLdm srishti purohit -- Doctors UI
                    case ServerViewTabs.Analysis:
                        toolid = "analyzeTabRunGroup_";
                        break;
                    default: return;
                }

                toolid = toolid + viewButtonKey;
                var tool = ribbon.GetToolById(toolid) as RadioButtonTool;
                if (tool == null) return;


                if (!tool.IsChecked.GetValueOrDefault())
                {
                    ignoreTabSelect = true;
                    tool.IsChecked = true;
                }
                else if (SelectedTab != tab)
                {
                    //// The selected view is already current on it's tab, but it wasn't on the shown tab, 
                    //// so it needs to be made the active view again since the tab is changing
                    //toolbarsManager_ToolClick(toolbarsManager, new ToolClickEventArgs(toolbarsManager.Tools[viewButtonKey], null));
                }

                // If the tab is already selected, the tab select event will not fire
                // and the view will not refresh, so it must be forced
                if (!ignoreTabSelect && SelectedTab == tab)
                {
                    ApplicationController.Default.RefreshActiveView();
                }

                if(currentTabControl != null)
                {
                    var currentTabControlItem = currentTabControl.FindName(viewButtonKey) as System.Windows.Controls.TabItem;
                    if(currentTabControlItem != null && currentTabControlItem != currentTabControl.SelectedItem)
                    {
                        currentTabControl.SelectionChanged -= this.OnToolClick;
                        currentTabControl.SelectedItem = currentTabControlItem;
                        currentTabControl.SelectionChanged += this.OnToolClick;
                    }
                }

                //SQLdm 10.6.3 - Sonali Dogra - The selectedTab(ServerView) is same but the sub-tab(selectedTool) changed.
                //Then switch to the respective sub-tab.
                if (SelectedTab == tab)
                {
                    switch (tab)
                    {
                        case ServerViewTabs.Overview:
                            ShowSelectedOverviewView();
                            overviewTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Sessions:
                            ShowSelectedSessionsView();
                            sessionsTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Databases:
                            ShowSelectedDatabasesView();
                            databaseTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Resources:
                            ShowSelectedResourcesView();
                            resourcesTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Services:
                            ShowSelectedServicesView();
                            servicesTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Queries:
                            ShowSelectedQueriesView();
                            queriesTabControlPanel.Visibility = Visibility.Visible;
                            break;
                        case ServerViewTabs.Analysis:
                            ShowSelectedRunAnalysisView();
                            break;
                        default:
                            ShowSelectedOverviewView();
                            overviewTabControlPanel.Visibility = Visibility.Visible;
                            break;
                    }
                }
                SelectedTab = tab;
                ignoreTabSelect = false;
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

        private void SetViewMode(ServerViewMode newViewMode, bool refreshView)
        {
            ViewMode = newViewMode;
            UpdateInstanceTitle();
            SetButtonToolEnabled("overviewTabSummaryViewLayoutGroup_overviewTabDesignButton", viewMode == ServerViewMode.RealTime);
            if (refreshView)
            {
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void ShowView(IServerView view)
        {
            ShowView(view, false);
        }

        private void ShowView(IServerView view, bool handleActiveViewFilter)
        {
           
            if (view != null)
            {
                if (activeView is IServerDesignView)
                {
                    IServerDesignView designView = activeView as IServerDesignView;
                    designView.CheckIfSaveNeeded();
                    designView.ToggleDesignMode(false);
                    if (!string.IsNullOrEmpty(designView.DesignTab))
                    {
                        var tab = (RibbonTabItem)ribbon.FindName(designView.DesignTab);
                        if (tab != null && tab != ribbon.SelectedTab)
                        {
                            tab.Visibility = Visibility.Collapsed;
                            tab.IsSelected = false;
                        }

                    }
                }
                if (view != activeView || view is Servers.Server.Analysis.Recommendations)
                {
                    if (activeView != null)
                    {
                        activeView.CancelRefresh();
                        activeView.SaveSettings();

                        if (activeView is Control)
                        {
                            ((Control)activeView).Dock = DockStyle.None;
                            ((Control)activeView).Visible = false;
                        }
                    }

                    if (handleActiveViewFilter && activeView is IDatabasesView)
                    {
                        view.SetArgument(((IDatabasesView)activeView).SelectedDatabaseFilter);
                    }

                    viewHost.SuspendLayout();

                    ApplicationController.Default.ClearCustomStatus();

                    viewHost.Add((IView)view);

                    view.UpdateUserTokenAttributes();
                    if (view is Control)
                    {
                        if (view is Servers.Server.Analysis.DefaultScreenAnalysisTab)
                        {
                            ((Control)view).Dock = DockStyle.Fill;
                            ((Control)view).Visible = true;
                            ((Control)view).BringToFront();
                        }
                        else
                        {
                            ((Control)view).BringToFront();
                            ((Control)view).Visible = true;
                        }
                    }
                    activeView = view;

                    viewHost.ResumeLayout();

                    ApplicationController.Default.RefreshActiveView();
                }
            }
        }

        /// <summary>
        /// This method does not raise an event; it's called by the ApplicationController to allow
        /// the ServerView to perform view specific actions after a refresh completes.
        /// </summary>
        public void OnRefreshCompleted(RefreshActiveViewCompletedEventArgs e)
        {
            if (HistoricalSnapshotDateTime.HasValue)
            {
                historyTimestampLabel.Text = currentHistoricalSnapshotDateTime.Value.ToString("f");
                historyBrowserControl.AddRecentlyViewedSnapshot(currentHistoricalSnapshotDateTime.Value);
            }

            UpdateHistoryGroupOptions(true);
        }


        private void OnSubViewChanged(SubViewChangedEventArgs e)
        {
            if (toggleCustomDashboard.IsChecked.HasValue && toggleCustomDashboard.IsChecked.Value)
            {
                EnableDisableCustomizeDashboard(false);
                _viewModel.ShowDashGalleryButtons = false;
                toggleCustomDashboard.IsChecked = false;
            }
            if (SubViewChanged != null)
            {
                SubViewChanged(this, e);
            }

            if (_viewModel != null)
            {
                _viewModel.SelectedServerTab = e.NewView;
            }
        }

        private void syncViewHistoryToCurrentHistoryBrowser(bool live = false)
        {
            if (ViewMode == ServerViewMode.RealTime)
            {
                _viewModel.EndHistoryDateTime = DateTime.Now;
                _viewModel.StartHistoryDateTime = DateTime.Now.AddMinutes(-currentScale);
                _viewModel.IsLive = live;
                _viewModel.IsCustomHistory = false;
                this.historySelectionCombobox.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(this.HistorySelectionCombobox_SelectionChanged);
                if (!_viewModel.IsLive)
                {
                    var historyItems = historySelectionCombobox.Items.SourceCollection;
                    System.Windows.Controls.ComboBoxItem historyItem = null;
                    foreach (System.Windows.Controls.ComboBoxItem item in historySelectionCombobox.Items)
                    {
                        if (item.Tag != null)
                        {
                            int tag = -999;
                            if (int.TryParse(item.Tag.ToString(), out tag))
                            {
                                if (tag == currentScale)
                                {
                                    historyItem = item;
                                    break;
                                }
                            }
                        }
                    }
                    historySelectionCombobox.SelectedItem = historyItem;
                }
                else
                {
                    historySelectionCombobox.SelectedItem = LiveHistoryComboBoxItem;
                }
                this.historySelectionCombobox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.HistorySelectionCombobox_SelectionChanged);
                }
            if (ViewMode == ServerViewMode.Custom || ViewMode == ServerViewMode.Historical)
            {
                var startDateTime = currentHistoricalStartDateTime;
                if (ViewMode == ServerViewMode.Historical)
                    startDateTime = currentHistoricalSnapshotDateTime;
                var endDateTime = currentHistoricalSnapshotDateTime;
                _viewModel.IsLive = false;
                _viewModel.IsCustomHistory = true;
                if (startDateTime.HasValue)
                    _viewModel.StartHistoryDateTime = startDateTime.Value;
                if (endDateTime.HasValue)
                    _viewModel.EndHistoryDateTime = endDateTime.Value;
                this.historySelectionCombobox.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(this.HistorySelectionCombobox_SelectionChanged);
                this.historySelectionCombobox.SelectedItem = CustomHistoryComboBoxItem;
                this.historySelectionCombobox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.HistorySelectionCombobox_SelectionChanged);
            }
        }

        private delegate void SetSelectedTabDelegate(RibbonTabItem tab);

        private void TabControlReset()
        {
            overviewTabControlPanel.Visibility = Visibility.Collapsed;
            sessionsTabControlPanel.Visibility = Visibility.Collapsed;
            queriesTabControlPanel.Visibility = Visibility.Collapsed;
            resourcesTabControlPanel.Visibility = Visibility.Collapsed;
            databaseTabControlPanel.Visibility = Visibility.Collapsed;
            servicesTabControlPanel.Visibility = Visibility.Collapsed;
        }

        private void SetSelectedTab(RibbonTabItem tab)
        {
            if (ignoreTabSelect || tab == null)
            {
                return;
            }
            //To handle history browser mode
            if (tab.Name == "Analyze")
                ApplicationModel.Default.AnalysisHistoryMode = true;
            else
            {
                ApplicationModel.Default.AnalysisHistoryMode = false;
                ApplicationController.Default.IsNextPrevious = false;
            }
            historyBrowserControl.RefreshHistoryPaneTimer();
            TabControlReset();
            switch (tab.Name)
            {
                case "Overview":
                    ShowSelectedOverviewView();
                    overviewTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Sessions":
                    ShowSelectedSessionsView();
                    sessionsTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Queries":
                    ShowSelectedQueriesView();
                    queriesTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Resources":
                    ShowSelectedResourcesView();
                    resourcesTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Databases":
                    ShowSelectedDatabasesView();
                    databaseTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Services":
                    ShowSelectedServicesView();
                    servicesTabControlPanel.Visibility = Visibility.Visible;
                    break;
                case "Logs":
                    ShowLogsView();
                    break;
                //10.0 srishti 
                case "Analyze":
                    ShowSelectedRunAnalysisView();
                    break;
            }
            selectedTabName = tab.Name;
            //          toolbarsManager.ResetAutoGenerateKeyTips();
        }

        //private void toolbarsManager_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        //{
        //    SetSelectedTabDelegate post = SetSelectedTab;
        //    BeginInvoke(post, e.Tab);
        //}

        public void Initialize()
        {
            startDate.Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            endDate.Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            historyBrowserControl.Initialize(instanceId);
            activeInstance = ApplicationModel.Default.ActiveInstances[instanceId];
            if (activeInstance != null)
            {
                activeInstance.Changed += OnInstanceChanged;
                UpdateMaintenanceModeButton(activeInstance);
                UpdateMirroringAndTempdbViewButton(activeInstance);
                UpdateWaitsViewButtons(activeInstance);
                UpdateAlwaysOnViewButton(activeInstance);
                InitTimelineControls();
                UpdateStatus();
            }

            ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;
        }

        //[START] SQLdm 10.0.2 (Barkha Khatri )
        void backgroundProductVersionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];
            //var instance = parameters[0];
            MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[(int)parameters[0]];
            if (instance != null)
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService();
                int sqlVersionMajor = 0;
                //try
                {
                    if (instance.MostRecentSQLVersion == null)
                    {
                        if (instance.ConnectionInfo != null)
                        {
                            //SQLdm 10.0.2 (Barkha Khatri) get Product version using management service
                            var productVersion = managementService.GetProductVersion(instance.Id);
                            //var productVersion = RepositoryHelper.GetProductVersion(instance.ConnectionInfo.ConnectionString);

                            sqlVersionMajor = productVersion != null ? productVersion.Major : 0;
                        }
                        else
                            sqlVersionMajor = 0;
                    }
                    else
                        sqlVersionMajor = instance.MostRecentSQLVersion.Major;

                }
                //SQLdm 10.1 (Srishti Purohit)- Launch SWA using registered id for the monitored instance of the sql server
                swaLaunchURL = managementService.GetSWAWebURL(ApplicationModel.Default.ActiveInstances[instance.Id].InstanceName);

                e.Result = Tuple.Create<MonitoredSqlServer, int, string>(instance, sqlVersionMajor, swaLaunchURL);
            }

        }

        void backgroundProductVersionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<MonitoredSqlServer, int, string> result = e.Result as Tuple<MonitoredSqlServer, int, string>;
            try
            {
                if ((MonitoredSqlServer)result.Item1 != null && (int)result.Item2 < 0)
                {
                    this.Analyze.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Analyze.Visibility = Visibility.Visible;
                }
                //SQLdm 10.1 (Srishti Purohit)- Launch SWA using registered id for the monitored instance of the sql server
                if (result.Item3 != null)
                {
                    this.LaunchSWA.Visibility = Visibility.Visible;
                }
                else
                    this.LaunchSWA.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                this.Analyze.Visibility = Visibility.Hidden;
                Log.Error("Error in visibility set of Analyze tab/LaunchSWA in tree view. " + ex);
            }
        }
        //[END] SQLdm 10.0.2 (Barkha Khatri)

        private void parentView_HistoryBrowserVisibleChanged(object sender, EventArgs e)
        {
            UpdateHistoryGroupOptions(true);
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            if (!ApplicationModel.Default.ActiveInstances.Contains(this.instanceId))
                return;

            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[this.instanceId];

            if (wrapper == null)
                return;

            UpdateMaintenanceModeButton(wrapper);
            UpdateMirroringAndTempdbViewButton(wrapper);
            UpdateWaitsViewButtons(wrapper);
            UpdateAlwaysOnViewButton(wrapper);

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            MonitoredSqlServerStatus status = null;
            if (ActiveView == null || ActiveView.HistoricalSnapshotDateTime == null)
                status = ApplicationModel.Default.GetInstanceStatus(instanceId);
            else
                status = serverSummaryhistoryData.HistoricalSnapshotStatus;

            _viewModel.UpdateStatus(status);
        }

        void OnInstanceChanged(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            
            instanceId = e.Instance.Id;
            UpdateMaintenanceModeButton(e.Instance);
            UpdateMirroringAndTempdbViewButton(e.Instance);
            UpdateWaitsViewButtons(e.Instance);
            UpdateAlwaysOnViewButton(e.Instance);
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
            if (activeView != null)
            {
                switch (ViewMode)
                {
                    case ServerViewMode.RealTime:
                        if (activeView.ViewMode != ServerViewMode.RealTime)
                        {
                            activeView.HistoricalStartDateTime = activeView.HistoricalSnapshotDateTime = null;
                            activeView.ViewMode = ServerViewMode.RealTime;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                            ApplicationController.Default.SetRefreshOptionsAvailability(true);
                            UpdateHistoryGroupOptions(true);
                        }
                        break;
                    case ServerViewMode.Historical:
                        if (activeView.ViewMode != ServerViewMode.Historical || activeView.HistoricalSnapshotDateTime != HistoricalSnapshotDateTime)
                        {
                            if (!ApplicationModel.Default.AnalysisHistoryMode && !ApplicationController.Default.IsNextPrevious)
                            {
                                if (selectedTabName == "Analyze")
                                {
                                    DateTime? result = null;
                                    if (!ApplicationModel.Default.AnalysisHistoryMode)
                                        result = RepositoryHelper.GetPreviousServerActivitySnapshotDateTime(
                                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, currentHistoricalSnapshotDateTime.Value);

                                    activeView.ViewMode = ServerViewMode.Historical;
                                    SetHistoricalSnapshotDateTime(result, false);
                                    activeView.HistoricalStartDateTime = null;
                                    activeView.HistoricalSnapshotDateTime = result;
                                    UpdateHistoryGroupOptions(true);
                                    ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                                }
                                else if (ApplicationController.Default.IsFromHistorySnapshot)
                                {
                                    GetSnapshotHistory();
                                }
                            }
                            else
                            {
                                GetSnapshotHistory();
                            }
                        }
                        break;
                    case ServerViewMode.Custom:
                        if (activeView.ViewMode != ServerViewMode.Custom || activeView.HistoricalStartDateTime != HistoricalStartDateTime ||
                            activeView.HistoricalSnapshotDateTime != HistoricalSnapshotDateTime)
                        {
                            activeView.ViewMode = ServerViewMode.Custom; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                            activeView.HistoricalStartDateTime = HistoricalStartDateTime;
                            activeView.HistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                            ApplicationController.Default.SetRefreshOptionsAvailability(false);
                            UpdateHistoryGroupOptions(true);
                        }
                        break;
                }

                activeView.RefreshView();
            }
        }

        private void GetSnapshotHistory()
        {
            activeView.ViewMode = ServerViewMode.Historical; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
            activeView.HistoricalStartDateTime = null;
            activeView.HistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
            ApplicationController.Default.SetRefreshOptionsAvailability(false);
            UpdateHistoryGroupOptions(true);
            ApplicationController.Default.IsNextPrevious = false;
        }

        public void UpdateUserTokenAttributes()
        {
            UpdateInstanceTitle();

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

        private void toolbarsManager_BeforeToolbarListDropdown(object sender, BeforeToolbarListDropdownEventArgs e)
        {
            e.ShowQuickAccessToolbarAddRemoveMenuItem = false;
            e.ShowQuickAccessToolbarPositionMenuItem = false;
        }

        public void SetActive(bool isActive)
        {

        }

        #region Overview Tab

        private void ShowSelectedOverviewView()
        {
            var selectedTool = overviewTabViewsGroup.FindCheckedTool();
            
            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
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
                    case "overviewTabTimelineViewButton":
                        ShowServerTimelineView();
                        break;
                }
            }
            else
            {
                var tool = (RadioButtonTool)ribbon.GetToolById("overviewTabViewsGroup_overviewTabSummaryViewButton");
                tool.IsChecked = true;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Refresh Instance state as per global state.
        /// </summary>
        private void RefreshInstanceState()
        {

            // SqlDM 10.2 (Anshul Aggarwal) - Switch view mode as per global setting.
            if (ApplicationModel.Default.HistoryTimeValue.ViewMode == ServerViewMode.Custom)
            {
                SetCustomStartEndDateTime(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                    ApplicationModel.Default.HistoryTimeValue.EndDateTime, false);
            }
            else if (ApplicationModel.Default.HistoryTimeValue.ViewMode == ServerViewMode.Historical)
            {
                SetHistoricalSnapshotDateTime(ApplicationModel.Default.HistoryTimeValue.HistoricalSnapshotDateTime, false);
            }
            else
            {
                SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, false);
            }
        }

        private void UpdateOverviewGroups()
        {
            if (activeView == null || serverSummaryView == null)
                return;

            overviewTabViewsGroup.Visibility = Visibility.Visible;
            overviewTabSummaryViewActionsGroup.Visibility =
                activeView == serverSummaryView && !serverSummaryView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            overviewTabSummaryViewHistoryGroup.Visibility =
                activeView == serverSummaryView || activeView == serverDetailsView ||
                activeView == serverActiveAlertsView
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            overviewTabDetailsViewShowHideGroup.Visibility = activeView == serverDetailsView
                                                                 ? Visibility.Visible
                                                                 : Visibility.Collapsed;
            overviewTabActiveAlertsShowHideGroup.Visibility = activeView == serverActiveAlertsView
                                                                  ? Visibility.Visible
                                                                  : Visibility.Collapsed;

            //toolbarsManager.Tools["toggleActiveAlertsForecastButton"].SharedProps.Visible =
            //    activeView == serverActiveAlertsView && !serverActiveAlertsView.HistoricalSnapshotDateTime.HasValue;
            overviewTabConfigurationViewShowHideGroup.Visibility = activeView == serverConfigurationView
                                                                       ? Visibility.Visible
                                                                       : Visibility.Collapsed;
            overviewTabDetailsViewFilterGroup.Visibility = activeView == serverDetailsView
                                                               ? Visibility.Visible
                                                               : Visibility.Collapsed;
            overviewTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                   ? Visibility.Visible
                                                   : Visibility.Collapsed;
            bool isHistorical = false;
            if (activeView != null && activeView.HistoricalSnapshotDateTime.HasValue)
                isHistorical = true;

            if (ApplicationModel.Default.ActiveInstances.Any(i => i.Id == instanceId) && ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId != null && ApplicationModel.Default.ActiveInstances[instanceId].Instance.CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                overviewTabSummaryViewLayoutGroup.Visibility = activeView == serverSummaryView || activeView == serverTimelineView
                   || activeView == serverActiveAlertsView || activeView == serverDetailsView || activeView == serverConfigurationView
                                                              // && !serverSummaryView.HistoricalSnapshotDateTime.HasValue
                                                              ? Visibility.Visible
                                                              : Visibility.Collapsed;
            }

            //overviewTabOverviewViewerGroup.Visibility = Visibility.Collapsed;
            //overviewTabOverviewLayoutActionsGroup.Visibility = Visibility.Collapsed;
            //overviewTabOverviewDesignActionsGroup.Visibility = Visibility.Collapsed;
            //overviewTabOverviewDesignShowHideGroup.Visibility = Visibility.Collapsed;


            overviewActivityTimelineFilterGroup.Visibility = activeView == serverTimelineView ? Visibility.Visible : Visibility.Collapsed;
            overviewTimelineButton.IsEnabled = !isHistorical;
        }
        private void showFirstVisibleOverviewServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilityOverviewDashboard))
            {
                if (overviewTabControl.SelectedItem != overviewTabSummaryViewButton)
                    overviewTabControl.SelectedItem = overviewTabSummaryViewButton;
                else
                    ShowServerSummaryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityOverviewDetails))
            {
                if (overviewTabControl.SelectedItem != overviewTabDetailsViewButton)
                    overviewTabControl.SelectedItem = overviewTabDetailsViewButton;
                else
                    ShowServerDetailsView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityOverviewConfiguration))
            {
                if (overviewTabControl.SelectedItem != overviewTabConfigurationViewButton)
                    overviewTabControl.SelectedItem = overviewTabConfigurationViewButton;
                else
                    ShowServerConfigurationView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityOverviewActiveAlerts))
            {
                if (overviewTabControl.SelectedItem != overviewTabAlertsViewButton)
                    overviewTabControl.SelectedItem = overviewTabAlertsViewButton;
                else
                    ShowServerAlertsView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityOverviewTimeline))
            {
                if (overviewTabControl.SelectedItem != overviewTabTimelineViewButton)
                    overviewTabControl.SelectedItem = overviewTabTimelineViewButton;
                else
                    ShowServerTimelineView();
            }
            else
            {
                if (overviewTabControl.SelectedItem != overviewTabSummaryViewButton)
                    overviewTabControl.SelectedItem = overviewTabSummaryViewButton;
                else
                    ShowServerSummaryView();
            }
            selectedServerTabTag = (overviewTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        private void showFirstVisibleSessionsServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilitySessionsSummary))
            {
                if (sessionsTabControl.SelectedItem != sessionsTabSummaryViewButton)
                    sessionsTabControl.SelectedItem = sessionsTabSummaryViewButton;
                else
                    ShowSessionsSummaryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilitySessionsDetails))
            {
                if (sessionsTabControl.SelectedItem != sessionsTabDetailsViewButton)
                    sessionsTabControl.SelectedItem = sessionsTabDetailsViewButton;
                else
                    ShowSessionsDetailsView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilitySessionsLocks))
            {
                if (sessionsTabControl.SelectedItem != sessionsTabLocksViewButton)
                    sessionsTabControl.SelectedItem = sessionsTabLocksViewButton;
                else
                    ShowSessionsLocksView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilitySessionsBlocking))
            {
                if (sessionsTabControl.SelectedItem != sessionsTabBlockingViewButton)
                    sessionsTabControl.SelectedItem = sessionsTabBlockingViewButton;
                else
                    ShowSessionsBlockingView();
            }
            else
            {
                if (sessionsTabControl.SelectedItem != sessionsTabSummaryViewButton)
                    sessionsTabControl.SelectedItem = sessionsTabSummaryViewButton;
                else
                    ShowSessionsSummaryView();
            }
            selectedServerTabTag = (sessionsTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        private void showFirstVisibleQueriesServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilityQueriesSignatureMode))
            {
                if (queriesTabControl.SelectedItem != queriesTabSignatureModeViewButton)
                    queriesTabControl.SelectedItem = queriesTabSignatureModeViewButton;
                else
                    ShowQueryMonitorView(ServerViews.QueriesSignatures);
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityQueriesStatementMode))
            {
                if (queriesTabControl.SelectedItem != queriesTabStatementModeViewButton)
                    queriesTabControl.SelectedItem = queriesTabStatementModeViewButton;
                else
                    ShowQueryMonitorView(ServerViews.QueriesStatements);
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityQueriesQueryHistory))
            {
                if (queriesTabControl.SelectedItem != queriesTabHistoryModeViewButton)
                    queriesTabControl.SelectedItem = queriesTabHistoryModeViewButton;
                else
                    ShowQueryMonitorView(ServerViews.QueriesHistory);
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityQueriesQueryWaits))
            {
                if (queriesTabControl.SelectedItem != queriesActiveWaitsConfigureButton)
                    queriesTabControl.SelectedItem = queriesActiveWaitsConfigureButton;
                else
                    ShowView(ServerViewTabs.Queries, "resourcesTabWaitStatsActiveViewButton");
            }
            else
            {
                if (queriesTabControl.SelectedItem != queriesTabSignatureModeViewButton)
                    queriesTabControl.SelectedItem = queriesTabSignatureModeViewButton;
                else
                    ShowQueryMonitorView(ServerViews.QueriesSignatures);
            }
            selectedServerTabTag = (queriesTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        private void showFirstVisibleDatabasesServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilityDatabaseSummary))
            {
                if (databaseTabControl.SelectedItem != databasesTabSummaryViewButton)
                    databaseTabControl.SelectedItem = databasesTabSummaryViewButton;
                else
                    ShowDatabasesSummaryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseAvailabilityGroup))
            {
                if (databaseTabControl.SelectedItem != databasesTabAlwaysOnViewButton)
                    databaseTabControl.SelectedItem = databasesTabAlwaysOnViewButton;
                else
                    ShowDatabasesAlwaysOnView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseTempdbSummary))
            {
                if (databaseTabControl.SelectedItem != databasesTabTempdbSummaryViewButton)
                    databaseTabControl.SelectedItem = databasesTabTempdbSummaryViewButton;
                else
                    ShowDatabasesTempdbView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseConfiguration))
            {
                if (databaseTabControl.SelectedItem != databasesTabConfigurationViewButton)
                    databaseTabControl.SelectedItem = databasesTabConfigurationViewButton;
                else
                    ShowDatabasesConfigurationView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseFiles))
            {
                if (databaseTabControl.SelectedItem != databasesTabFilesViewButton)
                    databaseTabControl.SelectedItem = databasesTabFilesViewButton;
                else
                    ShowDatabasesFilesView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseBackupRestore))
            {
                if (databaseTabControl.SelectedItem != databasesTabBackupRestoreHistoryViewButton)
                    databaseTabControl.SelectedItem = databasesTabBackupRestoreHistoryViewButton;
                else
                    ShowDatabasesBackupRestoreHistoryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseTablesIndexes))
            {
                if (databaseTabControl.SelectedItem != databasesTabTablesIndexesViewButton)
                    databaseTabControl.SelectedItem = databasesTabTablesIndexesViewButton;
                else
                    ShowDatabasesTablesIndexesView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityDatabaseMirroring))
            {
                if (databaseTabControl.SelectedItem != databasesTabMirroringViewButton)
                    databaseTabControl.SelectedItem = databasesTabMirroringViewButton;
                else
                    ShowDatabasesMirroringView();
            }
            else
            {
                if (databaseTabControl.SelectedItem != databasesTabSummaryViewButton)
                    databaseTabControl.SelectedItem = databasesTabSummaryViewButton;
                else
                    ShowDatabasesSummaryView();
            }
            selectedServerTabTag = (databaseTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        private void showFirstVisibleServicesServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilityServicesSummary))
            {
                if (servicesTabControl.SelectedItem != servicesTabSummaryViewButton)
                    servicesTabControl.SelectedItem = servicesTabSummaryViewButton;
                else
                    ShowServicesSummaryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityServicesSQLAgentJobs))
            {
                if (servicesTabControl.SelectedItem != servicesTabSqlAgentJobsViewButton)
                    servicesTabControl.SelectedItem = servicesTabSqlAgentJobsViewButton;
                else
                    ShowServicesSqlAgentJobsView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityServicesFullTextSearch))
            {
                if (servicesTabControl.SelectedItem != servicesTabFullTextSearchViewButton)
                    servicesTabControl.SelectedItem = servicesTabFullTextSearchViewButton;
                else
                    ShowServicesFullTextSearchView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityServicesReplication))
            {
                if (servicesTabControl.SelectedItem != servicesTabReplicationViewButton)
                    servicesTabControl.SelectedItem = servicesTabReplicationViewButton;
                else
                    ShowServicesReplicationView();
            }
            else
            {
                if (servicesTabControl.SelectedItem != servicesTabSummaryViewButton)
                    servicesTabControl.SelectedItem = servicesTabSummaryViewButton;
                else
                    ShowServicesSummaryView();
            }
            selectedServerTabTag = (servicesTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        private void showFirstVisibleResourcesServerView()
        {
            if (string.Equals("Visible", Settings.Default.VisibilityResourcesSummary))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabSummaryViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabSummaryViewButton;
                else
                    ShowResourcesSummaryView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesCPU))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabCpuViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabCpuViewButton;
                else
                    ShowResourcesCpuView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesMemory))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabMemoryViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabMemoryViewButton;
                else
                    ShowResourcesMemoryView();

            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesDisk))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabDiskViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabDiskViewButton;
                else
                    ShowResourcesDiskView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesDiskSize))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabDiskSizeViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabDiskSizeViewButton;
                else
                    ShowResourcesDiskSizeView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesFileActivity))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabFileActivityViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabFileActivityViewButton;
                else
                    ShowResourcesFileActivityView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesProcedureCache))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabProcedureCacheViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabProcedureCacheViewButton;
                else
                    ShowResourcesProcedureCacheView();
            }
            else if (string.Equals("Visible", Settings.Default.VisibilityResourcesServerWaits))
            {
                if (resourcesTabControl.SelectedItem != resourcesTabWaitStatsViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabWaitStatsViewButton;
                else
                    ShowResourcesWaitStatsView();
            }
            else
            {
                if (resourcesTabControl.SelectedItem != resourcesTabSummaryViewButton)
                    resourcesTabControl.SelectedItem = resourcesTabSummaryViewButton;
                else
                    ShowResourcesSummaryView();
            }
            selectedServerTabTag = (resourcesTabControl.SelectedItem as System.Windows.Controls.TabItem).Tag as string;
        }
        #region Overview Summary View

        private void ShowServerSummaryView()
        {
            if (!string.Equals("Visible", Settings.Default.VisibilityOverviewDashboard))
            {
                showFirstVisibleOverviewServerView();
                return;
            }

            if (serverSummaryView == null)
            {
                serverSummaryView = new ServerSummaryView4(instanceId, serverSummaryhistoryData, viewHost);
                serverSummaryView.HistoricalSnapshotDateTimeChanged += new EventHandler(serverSummaryView_HistoricalSnapshotDateTimeChanged);
                serverSummaryView.PanelGalleryVisibleChanged += new EventHandler(dashboardView_PanelGalleryVisibleChanged);
                serverSummaryView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (overviewTabControl.SelectedItem != overviewTabSummaryViewButton)
            {
                ignoreToolClick = true;
                overviewTabControl.SelectedItem = overviewTabSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Dashboard";
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

        private void StopSqlServer()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            managementService.SendShutdownSQLServer(new ShutdownSQLServerConfiguration(instanceId, true));
        }

        private void UpdateMaintenanceModeButton()
        {
            //MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
            //if (wrapper != null)
            //{
            //    UpdateMaintenanceModeButton(wrapper);
            //}
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

            var tool = ribbon.GetToolById("databasesTabViewsGroup_databasesTabMirroringViewButton");
            tool.Visibility = boolInstance2005AndUp ? Visibility.Visible : Visibility.Collapsed;

            tool = ribbon.GetToolById("databasesTabViewsGroup_databasesTabTempdbSummaryViewButton");
            tool.Visibility = boolInstance2005AndUp ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Show or hide the AlwaysOn view navigation buttons depending on the sql server version.
        /// The ribbon toolbar item will be shown only for SQL Server versions above 2012
        /// </summary>
        /// <param name="instance"></param>
        private void UpdateAlwaysOnViewButton(MonitoredSqlServer instance)
        {
            if (instance == null) return;

            bool sqlServerInstance2012AndUp = false;
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(instance.Id);

            if (status != null)
            {
                if (status.InstanceVersion != null)
                {
                    if (status.InstanceVersion.Major >= 11)
                    {
                        sqlServerInstance2012AndUp = true;
                    }
                }
            }

            var tool = ribbon.GetToolById("databasesTabViewsGroup_databasesTabAlwaysOnViewButton");
            tool.Visibility = sqlServerInstance2012AndUp ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Update server waits buttons
        /// </summary>
        /// <param name="instance"></param>
        private void UpdateWaitsViewButtons(MonitoredSqlServer instance)
        {
            if (instance == null)
                return;

            bool boolServerSupportsWaitsMonitoring = false;
            var status = ApplicationModel.Default.GetInstanceStatus(instance.Id);

            if (status != null && status.InstanceVersion != null && status.InstanceVersion.Major > 8)
                boolServerSupportsWaitsMonitoring = true;

            var visibility = boolServerSupportsWaitsMonitoring ? Visibility.Visible : Visibility.Collapsed;

            var tool = ribbon.GetToolById("resourcesTabViewsGroup_resourcesTabWaitStatsViewButton");
            tool.Visibility = visibility;
            tool = ribbon.GetToolById("queriesTabViewsGroup_resourcesTabWaitStatsActiveViewButton");
            tool.Visibility = visibility;
        }


        private void UpdateMaintenanceModeButton(MonitoredSqlServer instance)
        {
            //if (instance != null)
            //{
            //    ((StateButtonTool)toolbarsManager.Tools["maintenanceModeButton"]).InitializeChecked(instance.MaintenanceModeEnabled);
            //}
        }

        private void ToggleMaintenanceMode()
        {
            MonitoredSqlServerInstancePropertiesDialog dialog =
            new MonitoredSqlServerInstancePropertiesDialog(instanceId);
            dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
            dialog.ShowDialog(this.GetWinformWindow());

            try
            {
                ApplicationModel.Default.ConfigureMaintenanceMode(instanceId, dialog.Configuration.MaintenanceModeEnabled);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this.GetWinformWindow(), "An error occurred while changing maintenance mode status.", e);
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
                serverDetailsView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateServerDetailsChartVisibleButton();
                UpdateServerDetailsPropertiesVisibleButton();
                UpdateServerDetailsGroupByBoxVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = serverDetailsView.ChartPanelVisible, Name = "Chart", ShowHideCommand = new RelayCommand((x) => serverDetailsView.ChartPanelVisible = (bool)x) },
                new ShowHideItem { Checked = serverDetailsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => serverDetailsView.GridGroupByBoxVisible = (bool)x) },
                new ShowHideItem { Checked = serverDetailsView.PropertiesPanelVisible, Name = "Properties", ShowHideCommand = new RelayCommand((x) => { serverDetailsView.PropertiesPanelVisible = (bool)x; UpdateServerDetailsPropertiesVisibleButton(); }) }
            };
            if (overviewTabControl.SelectedItem != overviewTabDetailsViewButton)
            {
                ignoreToolClick = true;
                overviewTabControl.SelectedItem = overviewTabDetailsViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Details";
            _viewModel.ResetShowHideItems(showHideItems);
            _viewModel.ShowOvDetailsCustomCounter = serverDetailsView.Filter == ServerDetailsView.Selection.Custom;
            _viewModel.ShowOvDetailsShowAll = serverDetailsView.Filter == ServerDetailsView.Selection.All;
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
                var tool = ribbon.GetToolById("overviewTabDetailsViewShowHideGroup_toggleServerDetailsChartButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverDetailsView.ChartPanelVisible;
            }
        }

        private void UpdateServerDetailsGroupByBoxVisibleButton()
        {
            if (serverDetailsView != null)
            {
                var tool = ribbon.GetToolById("overviewTabDetailsViewShowHideGroup_toggleServerDetailsGroupByBoxButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverDetailsView.GridGroupByBoxVisible;
            }
        }

        private void UpdateServerDetailsPropertiesVisibleButton()
        {
            if (serverDetailsView != null)
            {
                var tool = ribbon.GetToolById("overviewTabDetailsViewShowHideGroup_toggleServerDetailsPropertiesButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverDetailsView.PropertiesPanelVisible;
            }
        }

        private void UpdateServerDetailsFilterButtons()
        {
            if (serverDetailsView != null)
            {
                ServerDetailsView.Selection selection = serverDetailsView.Filter;
                ignoreToolClick = true;
                SetRadioButtonToolState("overviewTabDetailsViewFilterGroup_detailsViewShowAllCountersButton", selection == ServerDetailsView.Selection.All);
                SetRadioButtonToolState("overviewTabDetailsViewFilterGroup_detailsViewShowCustomCountersButton", selection == ServerDetailsView.Selection.Custom);
                ignoreToolClick = false;
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = serverConfigurationView.DetailsPanelVisible, Name = "Details", ShowHideCommand = new RelayCommand((x) => serverConfigurationView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = serverConfigurationView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => serverConfigurationView.GridGroupByBoxVisible = (bool)x) }
            };
            if (overviewTabControl.SelectedItem != overviewTabConfigurationViewButton)
            {
                ignoreToolClick = true;
                overviewTabControl.SelectedItem = overviewTabConfigurationViewButton;
                ignoreToolClick = true;
            }
            _viewModel.ChildBreadcrumb = "Configuration";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(serverConfigurationView);
            UpdateOverviewGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewConfiguration, ServerViewTabs.Overview));
        }

        private void UpdateServerConfigurationDetailsVisibleButton()
        {
            if (serverConfigurationView != null)
            {
                var tool = ribbon.GetToolById("overviewTabConfigurationViewShowHideGroup_toggleServerConfigurationDetailsButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverConfigurationView.DetailsPanelVisible;
            }
        }

        private void UpdateServerConfigurationGroupByBoxVisibleButton()
        {
            if (serverConfigurationView != null)
            {
                var tool = ribbon.GetToolById("overviewTabConfigurationViewShowHideGroup_toggleServerConfigurationGroupByBoxButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverConfigurationView.GridGroupByBoxVisible;
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = serverActiveAlertsView.ForecastPanelVisible, Name = "12 Hour Forecast", ShowHideCommand = new RelayCommand((x) => serverActiveAlertsView.ForecastPanelVisible = (bool)x) },
                new ShowHideItem { Checked = serverActiveAlertsView.DetailsPanelVisible, Name = "Details", ShowHideCommand = new RelayCommand((x) => serverActiveAlertsView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = serverActiveAlertsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) =>  serverActiveAlertsView.GridGroupByBoxVisible = (bool)x) }
            };
            if (overviewTabControl.SelectedItem != overviewTabAlertsViewButton)
            {
                ignoreToolClick = true;
                overviewTabControl.SelectedItem = overviewTabAlertsViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Alerts";
            _viewModel.ResetShowHideItems(showHideItems);
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
                var tool = ribbon.GetToolById("overviewTabActiveAlertsShowHideGroup_toggleActiveAlertsForecastButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverActiveAlertsView.ForecastPanelVisible;
            }
        }

        private void UpdateActiveAlertsDetailsVisibleButton()
        {
            if (serverActiveAlertsView != null)
            {
                var tool = ribbon.GetToolById("overviewTabActiveAlertsShowHideGroup_toggleActiveAlertsDetailsButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverActiveAlertsView.DetailsPanelVisible;
            }
        }

        private void UpdateActiveAlertsGroupByBoxVisibleButton()
        {
            if (serverActiveAlertsView != null)
            {
                var tool = ribbon.GetToolById("overviewTabActiveAlertsShowHideGroup_toggleActiveAlertsGroupByBoxButton") as CheckBoxTool;
                if (tool == null) return;
                tool.IsChecked = serverActiveAlertsView.GridGroupByBoxVisible;
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

        #region Overview Timeline View

        private void ShowServerTimelineView()
        {
            if (serverTimelineView == null)
            {
                serverTimelineView = new ServerTimelineView(instanceId);
            }
            if (overviewTabControl.SelectedItem != overviewTabTimelineViewButton)
            {
                ignoreToolClick = true;
                overviewTabControl.SelectedItem = overviewTabTimelineViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Timeline";
            ShowView(serverTimelineView);
            UpdateOverviewGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.OverviewTimeline, ServerViewTabs.Overview));
        }

        #region Activity timeline filter

        private void ComboEditorTool_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (serverTimelineView == null)
                return;

            TimelineFilterRange range = (TimelineFilterRange)int.Parse(activityTimelineRange.Value as string);

            if (range != TimelineFilterRange.Custom)
                serverTimelineView.SetRange(range, (DateTime)activityTimelineStart.Value);

            UpdateTimelineControls();
        }

        private void activityTimelineStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (serverTimelineView == null || activityTimelineStart.Value == DBNull.Value)
                return;

            DateTime min = new DateTime(1753, 1, 1, 0, 0, 0);
            DateTime max = new DateTime(9999, 12, 31, 23, 59, 59);

            try
            {
                DateTime date = (DateTime)activityTimelineStart.Value;

                if (date < min || date > max)
                    return;

                serverTimelineView.SetRange(TimelineFilterRange.Custom, (DateTime)activityTimelineStart.Value);
            }
            catch
            {
            }
        }

        private void UpdateTimelineControls()
        {
            int range = int.Parse(activityTimelineRange.Value as string);

            label1.Visibility = range == 4 ? Visibility.Visible : Visibility.Hidden;
            activityTimelineStart.Visibility = range == 4 ? Visibility.Visible : Visibility.Hidden;
            OvTimelineFilter.Height = range == 4 ? 80 : 40;
        }

        private void InitTimelineControls()
        {
            activityTimelineStart.Value = DateTime.Now;
            label1.Visibility = System.Windows.Visibility.Hidden;
            activityTimelineStart.Visibility = System.Windows.Visibility.Hidden;
            OvTimelineFilter.Height = 40;
        }

        #endregion

        #endregion
        #region Baseline Visualizer

        /// <summary>
        /// SQLdm10.1 (Srishti Purohit) 
        /// Move the baseline visualizer to the ribbon of the overview section under "Views" category
        /// </summary>
        private void ShowBaselineAssistant()
        {
            try
            {
                BaselineAssistantDialog dialog =
                new BaselineAssistantDialog(instanceId);
                dialog.ShowDialog(this.GetWinformWindow());
            }
            catch (Exception ex)
            {
                Log.Error("Error while opening BaselineAssistantDialog.", ex);
            }
        }

        #endregion

        #endregion

        #region Sessions Tab

        private void ShowSelectedSessionsView()
        {
            var selectedTool = sessionsTabViewsGroup.FindCheckedTool();
            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
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
                var tool = (RadioButtonTool)ribbon.GetToolById("sessionsTabViewsGroup_sessionsTabSummaryViewButton");
                tool.IsChecked = true;
            }
        }

        #region Sessions Summary View

        private void UpdateSessionsGroups()
        {
            sessionsTabHistoryGroup.Visibility = Visibility.Visible;
            sessionsTabDetailsActionsGroup.Visibility =
                (activeView == sessionsDetailsView && !sessionsDetailsView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabDetailsFilterGroup.Visibility = activeView == sessionsDetailsView
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            showActiveSessionsOnlyCheckbox.Visibility = activeView == sessionsDetailsView && !sessionsDetailsView.HistoricalSnapshotDateTime.HasValue
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabDetailsShowHideGroup.Visibility = activeView == sessionsDetailsView
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabLocksActionsGroup.Visibility =
               (activeView == sessionsLocksView && !sessionsLocksView.HistoricalSnapshotDateTime.HasValue &&
               ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabLocksFilterGroup.Visibility = activeView == sessionsLocksView && !sessionsLocksView.HistoricalSnapshotDateTime.HasValue
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabLocksShowHideGroup.Visibility = activeView == sessionsLocksView
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabBlockingActionsGroup.Visibility =
                (activeView == sessionsBlockingView && !sessionsBlockingView.HistoricalSnapshotDateTime.HasValue &&
                ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabBlockingTypeGroup.Visibility = activeView == sessionsBlockingView
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabBlockingShowHideGroup.Visibility = activeView == sessionsBlockingView
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
            sessionsTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;
        }

        private void ShowSessionsSummaryView()
        {
            if (!string.Equals("Visible", Settings.Default.VisibilitySessionsSummary))
            {
                showFirstVisibleSessionsServerView();
                return;
            }
            if (sessionsSummaryView == null)
            {
                sessionsSummaryView = new SessionsSummaryView(instanceId, serverSummaryhistoryData);
                sessionsSummaryView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (sessionsTabControl.SelectedItem != sessionsTabSummaryViewButton)
            {
                ignoreToolClick = true;
                sessionsTabControl.SelectedItem = sessionsTabSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Summary";
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = sessionsDetailsView.DetailsPanelVisible, Name = "Details", ShowHideCommand = new RelayCommand((x) => sessionsDetailsView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = sessionsDetailsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => sessionsDetailsView.GridGroupByBoxVisible = (bool)x) }
            };
            if (sessionsTabControl.SelectedItem != sessionsTabDetailsViewButton)
            {
                ignoreToolClick = true;
                sessionsTabControl.SelectedItem = sessionsTabDetailsViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Details";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(sessionsDetailsView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            _viewModel.EnableTraceSessionButton = sessionsDetailsView.TraceAllowed;
            _viewModel.EnableKillSessionButton = sessionsDetailsView.KillAllowed;
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsDetails, ServerViewTabs.Sessions));
        }

        private void sessionsDetailsView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsDetailsView_TraceEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsDetailsView == activeView)
                _viewModel.EnableTraceSessionButton = sessionsDetailsView.TraceAllowed;
            UpdateSessionsDetailsTraceButton();
        }

        private void sessionsDetailsView_KillEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsDetailsView == activeView)
                _viewModel.EnableKillSessionButton = sessionsDetailsView.KillAllowed;
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
                SetButtonToolEnabled("sessionsTabDetailsActionsGroup_traceSessionsSessionButton", sessionsDetailsView.TraceAllowed);
            }
        }

        private void UpdateSessionsDetailsKillButton()
        {
            if (sessionsDetailsView != null)
            {
                SetButtonToolEnabled("sessionsTabDetailsActionsGroup_killSessionsSessionButton", sessionsDetailsView.KillAllowed);
            }
        }

        private void UpdateSessionsDetailsFilterButtons()
        {
            if (sessionsDetailsView != null)
            {
                SessionsConfiguration config = sessionsDetailsView.Configuration;
                ignoreToolClick = true;
                SetToggleButtonToolState("sessionsTabDetailsFilterGroup_filterSessionsButton", config.IsFiltered());
                SetCheckBoxToolState("sessionsTabDetailsFilterGroup_showActiveSessionsOnlyButton", config.Active);
                SetCheckBoxToolState("sessionsTabDetailsFilterGroup_showUserSessionsOnlyButton", config.ExcludeSystemProcesses);
                SetCheckBoxToolState("sessionsTabDetailsFilterGroup_showBlockedSessionsButton", config.Blocked);
                ignoreToolClick = false;
            }
        }

        private void UpdateSessionsDetailsDetailsButton()
        {
            if (sessionsDetailsView != null)
            {
                SetCheckBoxToolState("sessionsTabDetailsShowHideGroup_toggleSessionsDetailsDetailsPanelButton", sessionsDetailsView.DetailsPanelVisible);
            }
        }

        private void UpdateSessionsDetailsGroupByBoxButton()
        {
            if (sessionsDetailsView != null)
            {
                SetCheckBoxToolState("sessionsTabDetailsShowHideGroup_toggleSessionsDetailsGroupByBoxButton", sessionsDetailsView.GridGroupByBoxVisible);
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
                sessionsLocksView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateSessionsLocksChartVisibleButton();
                UpdateSessionsLocksGroupByBoxVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = sessionsLocksView.ChartVisible, Name = "Chart", ShowHideCommand = new RelayCommand((x) => sessionsLocksView.ChartVisible = (bool)x) },
                new ShowHideItem { Checked = sessionsLocksView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => sessionsLocksView.GridGroupByBoxVisible = (bool)x) }
            };
            if (sessionsTabControl.SelectedItem != sessionsTabLocksViewButton)
            {
                ignoreToolClick = true;
                sessionsTabControl.SelectedItem = sessionsTabLocksViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Locks";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(sessionsLocksView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            _viewModel.EnableTraceSessionButton = sessionsLocksView.TraceAllowed;
            _viewModel.EnableKillSessionButton = sessionsLocksView.KillAllowed;
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsLocks, ServerViewTabs.Sessions));
        }

        private void sessionsLocksView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsLocksView_TraceEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsLocksView == activeView)
                _viewModel.EnableTraceSessionButton = sessionsLocksView.TraceAllowed;
            UpdateSessionsLocksTraceButton();
        }

        private void sessionsLocksView_KillEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsLocksView == activeView)
                _viewModel.EnableKillSessionButton = sessionsLocksView.KillAllowed;
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
                SetButtonToolEnabled("sessionsTabLocksActionsGroup_traceSessionsLocksSessionButton", sessionsLocksView.TraceAllowed);
            }
        }

        private void UpdateSessionsLocksKillButton()
        {
            if (sessionsLocksView != null)
            {
                SetButtonToolEnabled("sessionsTabLocksActionsGroup_killSessionsLocksSessionButton", sessionsLocksView.KillAllowed);
            }
        }

        private void UpdateSessionsLocksFilterButtons()
        {
            if (sessionsLocksView != null)
            {
                LockDetailsConfiguration config = sessionsLocksView.Configuration;
                ignoreToolClick = true;
                SetToggleButtonToolState("sessionsTabLocksFilterGroup_filterLocksButton", config.IsFiltered());
                SetCheckBoxToolState("sessionsTabLocksFilterGroup_showBlockedLocksOnlyButton", config.FilterForBlocked);
                SetCheckBoxToolState("sessionsTabLocksFilterGroup_showBlockingLocksOnlyButton", config.FilterForBlocking);
                ignoreToolClick = false;
            }
        }

        private void UpdateSessionsLocksChartVisibleButton()
        {
            if (sessionsLocksView != null)
            {
                SetCheckBoxToolState("sessionsTabLocksShowHideGroup_toggleSessionsLocksChartButton", sessionsLocksView.ChartVisible);
            }
        }

        private void UpdateSessionsLocksGroupByBoxVisibleButton()
        {
            if (sessionsLocksView != null)
            {
                SetCheckBoxToolState("sessionsTabLocksShowHideGroup_toggleSessionsLocksGroupByBoxButton", sessionsLocksView.GridGroupByBoxVisible);
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
                sessionsBlockingView.BlocksDeadlocksListVisibleChanged += new EventHandler(sessionsBlockingView_BlocksDeadlocksListVisibleChanged);
                sessionsBlockingView.BlockingTypeChanged += new EventHandler(sessionsBlockingView_BlockingTypeChanged);
                sessionsBlockingView.TraceAllowedChanged += new EventHandler(sessionsBlockingView_TraceEnabledChanged);
                sessionsBlockingView.KillAllowedChanged += new EventHandler(sessionsBlockingView_KillEnabledChanged);
                sessionsBlockingView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateSessionsBlockingChartVisibleButton();
                UpdateSessionsBlocksDeadlocksListVisibleButton();
                UpdateSessionsBlockingTypeButtons();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = sessionsBlockingView.ChartVisible, Name = "Chart", ShowHideCommand = new RelayCommand((x) => sessionsBlockingView.ChartVisible = (bool)x) },
                new ShowHideItem { Checked = sessionsBlockingView.BlocksDeadlocksListVisible, Name = "Blocks and Deadlocks", ShowHideCommand = new RelayCommand((x) => sessionsBlockingView.BlocksDeadlocksListVisible = (bool)x) }
            };
            if (sessionsTabControl.SelectedItem != sessionsTabBlockingViewButton)
            {
                ignoreToolClick = true;
                sessionsTabControl.SelectedItem = sessionsTabBlockingViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Blocking";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(sessionsBlockingView);
            UpdateSessionsGroups();
            UpdatePulseGroup();
            _viewModel.EnableKillSessionButton = sessionsBlockingView.KillAllowed;
            _viewModel.EnableTraceSessionButton = sessionsBlockingView.TraceAllowed;
            if (_viewModel.ShowBtByLock == null || _viewModel.ShowBtByLock == false)
                _viewModel.ShowBtBySession = true;
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.SessionsBlocking, ServerViewTabs.Sessions));
        }

        private void sessionsBlockingView_HistoricalSnapshotDateTimeChanged(object sender, EventArgs e)
        {
            UpdateSessionsGroups();
        }

        private void sessionsBlockingView_TraceEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsBlockingView == activeView)
                _viewModel.EnableTraceSessionButton = sessionsBlockingView.TraceAllowed;
            UpdateSessionsBlockingTraceButton();
        }

        private void sessionsBlockingView_KillEnabledChanged(object sender, EventArgs e)
        {
            if (_viewModel != null && sessionsBlockingView == activeView)
                _viewModel.EnableKillSessionButton = sessionsBlockingView.KillAllowed;
            UpdateSessionsBlockingKillButton();
        }

        private void sessionsBlockingView_ChartVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlockingChartVisibleButton();
        }

        private void sessionsBlockingView_BlocksDeadlocksListVisibleChanged(object sender, EventArgs e)
        {
            UpdateSessionsBlocksDeadlocksListVisibleButton();
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
                SetCheckBoxToolState("sessionsTabBlockingShowHideGroup_toggleSessionsBlockingChartButton", sessionsBlockingView.ChartVisible);
            }
        }

        private void UpdateSessionsBlocksDeadlocksListVisibleButton()
        {
            if (sessionsBlockingView != null)
            {
                SetCheckBoxToolState("sessionsTabBlockingShowHideGroup_toggleSessionsBlocksDeadlocksListButton", sessionsBlockingView.BlocksDeadlocksListVisible);
            }
        }

        private void UpdateSessionsBlockingTypeButtons()
        {
            if (sessionsBlockingView != null)
            {
                SetRadioButtonToolState("", true);

                if (sessionsBlockingView.BlockingTypeShown == SessionsBlockingView.BlockingType.Locks)
                {
                    SetRadioButtonToolState("sessionsTabBlockingTypeGroup_showSessionsBlockingByLockButton", true);
                }
                else
                {
                    SetRadioButtonToolState("sessionsTabBlockingTypeGroup_showSessionsBlockingBySessionButton", true);
                }
            }
        }

        private void UpdateSessionsBlockingTraceButton()
        {
            if (sessionsBlockingView != null)
            {
                SetButtonToolEnabled("sessionsTabBlockingActionsGroup_traceSessionsBlockingSessionButton", sessionsBlockingView.TraceAllowed);
            }
        }

        private void UpdateSessionsBlockingKillButton()
        {
            if (sessionsBlockingView != null)
            {
                SetButtonToolEnabled("sessionsTabBlockingActionsGroup_killSessionsBlockingSessionButton", sessionsBlockingView.KillAllowed);
            }
        }

        private void ToggleSessionsBlockingChart()
        {
            if (sessionsBlockingView != null)
            {
                sessionsBlockingView.ChartVisible = !sessionsBlockingView.ChartVisible;
            }
        }

        private void ToggleSessionsBlocksDeadlocksListButton()
        {
            if (sessionsBlockingView != null)
            {
                sessionsBlockingView.BlocksDeadlocksListVisible = !sessionsBlockingView.BlocksDeadlocksListVisible;
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
            var selectedTool = queriesTabViewsGroup.FindCheckedTool();
            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
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
                    case "resourcesTabWaitStatsActiveViewButton":
                        ShowQueriesWaitStatsActiveView();
                        break;
                    default:
                        ShowQueryMonitorView(ServerViews.Queries);
                        break;
                }
            }
            else
            {
                var tool = (RadioButtonTool)ribbon.GetToolById("queriesTabViewsGroup_queriesTabSignatureModeViewButton");
                tool.IsChecked = true;
            }
        }

        /// <summary>
        /// Show or hide groups from the ribbon
        /// </summary>
        private void UpdateQueriesGroups()
        {
            queriesTabPropertiesGroup.Visibility = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
                                                       ? Visibility.Visible
                                                       : Visibility.Collapsed;

            queriesTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                       ? Visibility.Visible
                                                       : Visibility.Collapsed;

            actionsRibbonGroup.Visibility = activeView == queriesWaitStatsActiveView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;

            //Inverse of actionsRibbonGroup
            queriesTabPropertiesGroup.Visibility = activeView == queriesWaitStatsActiveView
                                                    ? Visibility.Collapsed
                                                    : Visibility.Visible;

            queriesTabShowHideGroup.Visibility = queriesTabPropertiesGroup.Visibility;
            queriesTabFilterGroup.Visibility = queriesTabPropertiesGroup.Visibility;

        }

        private void ShowQueryMonitorView(ServerViews view)
        {
            if (view == ServerViews.QueriesSignatures && !string.Equals("Visible", Settings.Default.VisibilityQueriesSignatureMode))
            {
                showFirstVisibleQueriesServerView();
                return;
            }
            if (queryMonitorView == null)
            {
                queryMonitorView = new QueryMonitorView(instanceId);
                queryMonitorView.FiltersVisibleChanged += new EventHandler(queryMonitorView_FiltersVisibleChanged);
                queryMonitorView.GridVisibleChanged += new EventHandler(queryMonitorView_GridVisibleChanged);
                queryMonitorView.GridGroupByBoxVisibleChanged += new EventHandler(queryMonitorView_GridGroupByBoxVisibleChanged);
                queryMonitorView.FilterChanged += new EventHandler(queryMonitorView_FilterChanged);
                queryMonitorView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateQueryMonitorFilterButtons();
                UpdateQueryMonitorFiltersVisibleButton();
                UpdateQueryMonitorGridVisibleButton();
                UpdateQueryMonitorGroupByBoxVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = queryMonitorView.FiltersVisible, Name = "Filters", ShowHideCommand = new RelayCommand((x) => queryMonitorView.FiltersVisible = (bool)x) },
                new ShowHideItem { Checked = queryMonitorView.GridVisible, Name = "List", ShowHideCommand = new RelayCommand((x) => queryMonitorView.GridVisible = (bool)x) },
                new ShowHideItem { Checked = queryMonitorView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => queryMonitorView.GridGroupByBoxVisible = (bool)x) }
            };
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(queryMonitorView);

            switch (view)
            {
                case ServerViews.QueriesSignatures:
                    _viewModel.ChildBreadcrumb = this.queriesTabSignatureModeViewButton.Header as string;
                    if (queriesTabControl.SelectedItem != queriesTabSignatureModeViewButton)
                    {
                        ignoreToolClick = true;
                        queriesTabControl.SelectedItem = queriesTabSignatureModeViewButton;
                        ignoreToolClick = false;
                    }
                    queryMonitorView.ShowSignatureMode();
                    break;
                case ServerViews.QueriesStatements:
                    _viewModel.ChildBreadcrumb = this.queriesTabStatementModeViewButton.Header as string;
                    if (queriesTabControl.SelectedItem != queriesTabStatementModeViewButton)
                    {
                        ignoreToolClick = true;
                        queriesTabControl.SelectedItem = queriesTabStatementModeViewButton;
                        ignoreToolClick = false;
                    }
                    queryMonitorView.ShowStatementMode();
                    break;
                case ServerViews.QueriesHistory:
                    _viewModel.ChildBreadcrumb = this.queriesTabHistoryModeViewButton.Header as string;
                    if (queriesTabControl.SelectedItem != queriesTabHistoryModeViewButton)
                    {
                        ignoreToolClick = true;
                        queriesTabControl.SelectedItem = queriesTabHistoryModeViewButton;
                        ignoreToolClick = false;
                    }
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
                SetCheckBoxToolState("queriesTabShowHideGroup_toggleQueryMonitorFiltersButton", queryMonitorView.FiltersVisible);
            }
        }

        private void UpdateQueryMonitorGridVisibleButton()
        {
            if (queryMonitorView != null)
            {
                SetCheckBoxToolState("queriesTabShowHideGroup_toggleQueryMonitorGridButton", queryMonitorView.GridVisible);
            }
        }

        private void UpdateQueryMonitorGroupByBoxVisibleButton()
        {
            if (queryMonitorView != null)
            {
                SetCheckBoxToolState("queriesTabShowHideGroup_toggleQueryMonitorGroupByBoxButton", queryMonitorView.GridGroupByBoxVisible);
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
                SetToggleButtonToolState("queriesTabFilterGroup_filterQueryMonitorButton", queryMonitorView.Filter.IsFiltered());
                ignoreToolClick = false;
                SetCheckBoxToolState("queriesTabFilterGroup_showQueryMonitorSqlStatementsButton", queryMonitorView.Filter.IncludeSqlStatements);
                SetCheckBoxToolState("queriesTabFilterGroup_showQueryMonitorStoredProceduresButton", queryMonitorView.Filter.IncludeStoredProcedures);
                SetCheckBoxToolState("queriesTabFilterGroup_showQueryMonitorSqlBatchesButton", queryMonitorView.Filter.IncludeSqlBatches);
                SetCheckBoxToolState("queriesTabFilterGroup_showQueryMonitorCurrentQueriesButton", queryMonitorView.Filter.IncludeOnlyResourceRows);
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
            var selectedTool = resourcesTabViewsGroup.FindCheckedTool();

            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
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
                    case "resourcesTabDiskSizeViewButton":     // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --add disk size view button to resource tab
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
                var tool = (RadioButtonTool)ribbon.GetToolById("resourcesTabViewsGroup_resourcesTabSummaryViewButton");
                tool.IsChecked = true;
            }
        }

        private void UpdateResourcesGroups()
        {
            UpdateHistoryGroupOptions(true);

            resourcesTabHistoryGroup.Visibility = activeView != resourcesProcedureCacheView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;

            // SQLdm 9.0 (Abhishek Joshi) --DE4240 -collapse the Visibility mode of historical snapshots for File activity in Resources
            if (activeView == resourcesFileActivityView)
                resourcesTabHistoryGroup.Visibility = Visibility.Collapsed;

            resourcesTabActionsGroup.Visibility =
                (activeView == resourcesProcedureCacheView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            resourcesTabProcedureCacheViewShowHideGroup.Visibility = activeView == resourcesProcedureCacheView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            resourcesTabWaitStatsViewShowHideGroup.Visibility = activeView == resourcesWaitStatsView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --show the Show/Hide group, if disk sixe view is selected
            resourcesTabDiskSizeViewShowHideGroup.Visibility = activeView == resourcesDiskSizeView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;

            resourcesShowFileActivityFilterGroup.Visibility = activeView == resourcesFileActivityView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;

            resourcesTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
        }

        private void ShowResourcesSummaryView()
        {
            if (!string.Equals("Visible", Settings.Default.VisibilityResourcesSummary))
            {
                showFirstVisibleResourcesServerView();
                return;
            }
            if (resourcesSummaryView == null)
            {
                resourcesSummaryView = new ResourcesSummaryView(instanceId, serverSummaryhistoryData);
                resourcesSummaryView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (resourcesTabControl.SelectedItem != resourcesTabSummaryViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Summary";
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
                resourcesCpuView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (resourcesTabControl.SelectedItem != resourcesTabCpuViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabCpuViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabCpuViewButton.Header as string;
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
                resourcesMemoryView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (resourcesTabControl.SelectedItem != resourcesTabMemoryViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabMemoryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabMemoryViewButton.Header as string;
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
                resourcesDiskView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            if (resourcesTabControl.SelectedItem != resourcesTabDiskViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabDiskViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabDiskViewButton.Header as string;
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
                resourcesDiskSizeView.GridGroupByBoxVisibleChanged += new EventHandler(resourcesDiskSizeView_GridGroupByBoxVisibleChanged);
                UpdateDiskSizeViewGroupByBoxVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = resourcesDiskSizeView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => resourcesDiskSizeView.GridGroupByBoxVisible = (bool)x) }
            };
            if (resourcesTabControl.SelectedItem != resourcesTabDiskSizeViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabDiskSizeViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabDiskSizeViewButton.Header as string;
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(resourcesDiskSizeView);
            UpdateResourcesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ResourcesDiskSize, ServerViewTabs.Resources));
        }

        private void resourcesDiskSizeView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDiskSizeViewGroupByBoxVisibleButton();
        }

        private void UpdateDiskSizeViewGroupByBoxVisibleButton()
        {
            if (resourcesDiskSizeView != null)
            {
                SetCheckBoxToolState("resourcesTabDiskSizeViewShowHideGroup_toggleDiskSizeViewGroupByBoxButton", resourcesDiskSizeView.GridGroupByBoxVisible);
            }
        }

        private void ToggleDiskSizeGroupByBox()
        {
            if (resourcesDiskSizeView != null)
            {
                resourcesDiskSizeView.GridGroupByBoxVisible = !resourcesDiskSizeView.GridGroupByBoxVisible;
            }
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = resourcesProcedureCacheView.ChartsVisible, Name = "Charts", ShowHideCommand = new RelayCommand((x) => resourcesProcedureCacheView.ChartsVisible = (bool)x) },
                new ShowHideItem { Checked = resourcesProcedureCacheView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => resourcesProcedureCacheView.GridGroupByBoxVisible = (bool)x) }
            };
            if (resourcesTabControl.SelectedItem != resourcesTabProcedureCacheViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabProcedureCacheViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabProcedureCacheViewButton.Header as string;
            _viewModel.ResetShowHideItems(showHideItems);
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
                resourcesWaitStatsView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateWaitStatsGroupByBoxVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = resourcesWaitStatsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => resourcesWaitStatsView.GridGroupByBoxVisible = (bool)x) }
            };
            if (resourcesTabControl.SelectedItem != resourcesTabWaitStatsViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabWaitStatsViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabWaitStatsViewButton.Header as string;
            _viewModel.ResetShowHideItems(showHideItems);
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
                queriesWaitStatsActiveView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = resourcesFileActivityView.FilterPaneVisible, Name = "Show Filter", ShowHideCommand = new RelayCommand((x) => resourcesFileActivityView.FilterPaneVisible = (bool)x) }
            };
            if (resourcesTabControl.SelectedItem != resourcesTabFileActivityViewButton)
            {
                ignoreToolClick = true;
                resourcesTabControl.SelectedItem = resourcesTabFileActivityViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = this.resourcesTabFileActivityViewButton.Header as string;
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetCheckBoxToolState("resourcesTabProcedureCacheViewShowHideGroup_toggleProcedureCacheChartsButton", resourcesProcedureCacheView.ChartsVisible);
            }
        }

        private void UpdateProcedureCacheGroupByBoxVisibleButton()
        {
            if (resourcesProcedureCacheView != null)
            {
                SetCheckBoxToolState("resourcesTabProcedureCacheViewShowHideGroup_toggleProcedureCacheGroupByBoxButton", resourcesProcedureCacheView.GridGroupByBoxVisible);
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
                SetCheckBoxToolState("resourcesTabWaitStatsViewShowHideGroup_toggleWaitStatsGroupByBoxButton", resourcesWaitStatsView.GridGroupByBoxVisible);
            }
        }

        private void UpdateResourcesFileActivityShowFilterButton()
        {
            if (resourcesFileActivityView != null)
            {
                SetCheckBoxToolState("resourcesShowFileActivityFilterGroup_resourcesShowFileActivityFilterButton", resourcesFileActivityView.FilterPaneVisible);
            }
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

        private void ShowQueryWaitsActiveFilterDialog()
        {
            if (queriesWaitStatsActiveView != null)
            {
                queriesWaitStatsActiveView.ShowFilter();
            }
        }

        private void ShowQueryWaitsActiveConfigureDialog()
        {
            if (queriesWaitStatsActiveView != null)
            {
                queriesWaitStatsActiveView.ShowConfigureDialog();
            }
        }

        #endregion

        #region Databases Tab

        private void ShowSelectedDatabasesView()
        {
            var selectedTool = databasesTabViewsGroup.FindCheckedTool();
            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
                {
                    case "databasesTabSummaryViewButton":
                        ShowDatabasesSummaryView();
                        break;
                    case "databasesTabAlwaysOnViewButton":
                        ShowDatabasesAlwaysOnView();
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
                        var tool = (RadioButtonTool)ribbon.GetToolById("databasesTabViewsGroup_databasesTabSummaryViewButton");
                        tool.IsChecked = true;
                        break;
                }
            }
            else
            {
                var tool = (RadioButtonTool)ribbon.GetToolById("databasesTabViewsGroup_databasesTabSummaryViewButton");
                tool.IsChecked = true;
            }
        }

        private void UpdateDatabasesGroups()
        {
            databasesTabSummaryViewFilterGroup.Visibility = Visibility.Collapsed;
            databasesTabAlwaysOnViewShowHideGroup.Visibility = activeView == databasesAlwaysOnView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabSummaryViewShowHideGroup.Visibility = activeView == databasesSummaryView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            ///SQLDM 10.0.0 (Ankit Nagpal)
            databasesTabHistoryGroup.Visibility = (activeView == databasesTempdbView || activeView == databasesAlwaysOnView)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabTempdbViewFilterGroup.Visibility = activeView == databasesTempdbView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabTempdbViewShowHideGroup.Visibility = activeView == databasesTempdbView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabConfigurationViewFilterGroup.Visibility = Visibility.Collapsed;
            databasesTabConfigurationViewShowHideGroup.Visibility = activeView == databasesConfigurationView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabBackupRestoreHistoryViewFilterGroup.Visibility = activeView == databasesBackupRestoreHistoryView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabBackupRestoreHistoryViewShowHideGroup.Visibility = activeView == databasesBackupRestoreHistoryView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabFilesViewFilterGroup.Visibility = Visibility.Collapsed;
            databasesTabFilesViewShowHideGroup.Visibility = activeView == databasesFilesView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabTablesViewActionsGroup.Visibility =
                activeView == databasesTablesIndexesView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabTablesViewFilterGroup.Visibility = activeView == databasesTablesIndexesView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabTablesViewShowHideGroup.Visibility = activeView == databasesTablesIndexesView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabMirroringViewShowHideGroup.Visibility = activeView == databasesMirroringView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabMirroringViewActionsGroup.Visibility =
                activeView == databasesMirroringView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            databasesTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
        }

        #region Databases Summary

        private void ShowDatabasesSummaryView()
        {
            ShowDatabasesSummaryView(false);
        }

        private void ShowDatabasesSummaryView(bool handleActiveViewFilter)
        {
            if (!string.Equals("Visible", Settings.Default.VisibilityDatabaseSummary))
            {
                showFirstVisibleDatabasesServerView();
                return;
            }
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesSummaryView.ChartVisible, Name = "Charts", ShowHideCommand = new RelayCommand((x) => databasesSummaryView.ChartVisible = (bool)x) },
                new ShowHideItem { Checked = databasesSummaryView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesSummaryView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabSummaryViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Summary";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetCheckBoxToolState("databasesTabSummaryViewFilterGroup_databasesSummaryShowUserDatabasesOnlyButton", !databasesSummaryView.Configuration.IncludeSystemDatabases);
            }
        }

        private void UpdateDatabasesSummaryGroupByBoxVisibleButton()
        {
            if (databasesSummaryView != null)
            {
                SetCheckBoxToolState("databasesTabSummaryViewShowHideGroup_toggleDatabasesSummaryGroupByBoxButton", databasesSummaryView.GridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesSummaryChartVisibleButton()
        {
            if (databasesSummaryView != null)
            {
                SetCheckBoxToolState("databasesTabSummaryViewShowHideGroup_toggleDatabasesSummaryChartsButton", databasesSummaryView.ChartVisible);
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

        #endregion

        #region Database Configuration

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesConfigurationView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesConfigurationView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabConfigurationViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabConfigurationViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Configuration";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetCheckBoxToolState("databasesTabConfigurationViewFilterGroup_databasesConfigurationShowUserDatabasesOnlyButton", !databasesConfigurationView.IncludeSystemDatabases);
            }
        }

        private void UpdateDatabasesConfigurationGroupByBoxVisibleButton()
        {
            if (databasesConfigurationView != null)
            {
                SetCheckBoxToolState("databasesTabConfigurationViewShowHideGroup_toggleDatabasesConfigurationGroupByBoxButton", databasesConfigurationView.GridGroupByBoxVisible);
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

        #endregion

        #region Database Backup

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisible, Name = "Databases Group By Box", ShowHideCommand = new RelayCommand((x) => databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisible = (bool)x) },
                new ShowHideItem { Checked = databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisible, Name = "History Group By Box", ShowHideCommand = new RelayCommand((x) => databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabBackupRestoreHistoryViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabBackupRestoreHistoryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Backup Restore";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetToggleButtonToolState("databasesTabBackupRestoreHistoryViewFilterGroup_filterDatabasesBackupRestoreHistoryButton", config.IsFiltered());
                SetCheckBoxToolState("databasesTabBackupRestoreHistoryViewFilterGroup_databasesBackupRestoreHistoryShowUserDatabasesOnlyButton", !configDb.IncludeSystemDatabases);
                SetCheckBoxToolState("databasesTabBackupRestoreHistoryViewFilterGroup_databasesBackupRestoreHistoryShowBackupsButton", config.ShowBackups);
                SetCheckBoxToolState("databasesTabBackupRestoreHistoryViewFilterGroup_databasesBackupRestoreHistoryShowRestoresButton", config.ShowRestores);
                ignoreToolClick = false;
            }
        }

        private void UpdateDatabasesBackupRestoreHistoryDatabasesGroupByBoxVisibleButton()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                SetCheckBoxToolState("databasesTabBackupRestoreHistoryViewShowHideGroup_toggleDatabasesBackupRestoreHistoryDatabasesGroupByBoxButton", databasesBackupRestoreHistoryView.DatabasesGridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesBackupRestoreHistoryHistoryGroupByBoxVisibleButton()
        {
            if (databasesBackupRestoreHistoryView != null)
            {
                SetCheckBoxToolState("databasesTabBackupRestoreHistoryViewShowHideGroup_toggleDatabasesBackupRestoreHistoryGroupByBoxButton", databasesBackupRestoreHistoryView.HistoryGridGroupByBoxVisible);
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

        #endregion

        #region Database Files

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesFilesView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesFilesView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabFilesViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabFilesViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Files";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetCheckBoxToolState("databasesTabFilesViewFilterGroup_databasesFilesShowUserDatabasesOnlyButton", !configDb.IncludeSystemDatabases);
                ignoreToolClick = false;
            }
        }

        private void UpdateDatabasesFilesGroupByBoxVisibleButton()
        {
            if (databasesFilesView != null)
            {
                SetCheckBoxToolState("databasesTabFilesViewShowHideGroup_toggleDatabasesFilesGroupByBoxButton", databasesFilesView.GridGroupByBoxVisible);
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

        #endregion

        #region Database Tables & Indexes

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesTablesIndexesView.DetailsPanelVisible, Name = "Table Details", ShowHideCommand = new RelayCommand((x) => databasesTablesIndexesView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = databasesTablesIndexesView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesTablesIndexesView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabTablesIndexesViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabTablesIndexesViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Indexes";
            _viewModel.ResetShowHideItems(showHideItems);
            _viewModel.ShowDbUserTables = databasesTablesIndexesView.IncludeSystemTables;
            _viewModel.ShowDbUserDatabases = databasesTablesIndexesView.IncludeSystemDatabases;
            ShowView(databasesTablesIndexesView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            if(_viewModel != null)
            {
                _viewModel.EnableDatabaseStats = databasesTablesIndexesView.UpdateStatisticsAllowed;
                _viewModel.EnableRebuildIndices = databasesTablesIndexesView.RebuildIndexesAllowed;
            }
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesTablesIndexes, ServerViewTabs.Databases));
        }

        void databasesTablesIndexesView_ActionsAllowedChanged(object sender, EventArgs e)
        {
            if (databasesTablesIndexesView != null)
            {
                if(_viewModel != null)
                {
                    _viewModel.EnableDatabaseStats = databasesTablesIndexesView.UpdateStatisticsAllowed;
                    _viewModel.EnableRebuildIndices = databasesTablesIndexesView.RebuildIndexesAllowed;
                }
                SetButtonToolEnabled("databasesTabTablesViewActionsGroup_updateTableStatisticsButton", databasesTablesIndexesView.UpdateStatisticsAllowed);
                SetButtonToolEnabled("databasesTabTablesViewActionsGroup_rebuildTableIndexesButton", databasesTablesIndexesView.RebuildIndexesAllowed);
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
                SetCheckBoxToolState("databasesTabTablesViewFilterGroup_databasesTablesShowUserDatabasesOnlyButton", !databasesTablesIndexesView.IncludeSystemDatabases);
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
                SetCheckBoxToolState("databasesTabTablesViewFilterGroup_databasesTablesShowUserTablesOnlyButton", !databasesTablesIndexesView.IncludeSystemTables);
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
                SetCheckBoxToolState("databasesTabTablesViewShowHideGroup_toggleDatabasesTablesTableDetailsButton", databasesTablesIndexesView.DetailsPanelVisible);
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

        private void UpdateDatabasesTablesGroupByBoxButton()
        {
            if (databasesTablesIndexesView != null)
            {
                SetCheckBoxToolState("databasesTabTablesViewShowHideGroup_toggleDatabasesTablesGroupByBoxButton", databasesTablesIndexesView.GridGroupByBoxVisible);
            }
        }

        #endregion

        #region AlwaysOn

        private void ShowDatabasesAlwaysOnView()
        {
            ShowDatabasesAlwaysOnView(false);
        }

        private void ShowDatabasesAlwaysOnView(bool handleActiveViewFilter)
        {
            if (databasesAlwaysOnView == null)
            {
                databasesAlwaysOnView = new DatabasesAlwaysOnView(instanceId);
                databasesAlwaysOnView.GridGroupByBoxVisibleChanged += new EventHandler(databasesAlwaysOnView_GridGroupByBoxVisibleChanged);
                databasesAlwaysOnView.ChartVisibleChanged += new EventHandler(databasesAlwaysOnView_ChartVisibleChanged);
                databasesAlwaysOnView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                UpdateDatabasesAlwaysOnGroupByBoxVisibleButton();
                UpdateDatabasesAlwaysOnChartVisibleButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesAlwaysOnView.ChartVisible, Name = "Charts", ShowHideCommand = new RelayCommand((x) => databasesAlwaysOnView.ChartVisible = (bool)x) },
                new ShowHideItem { Checked = databasesAlwaysOnView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesAlwaysOnView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabAlwaysOnViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabAlwaysOnViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Availability";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(databasesAlwaysOnView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabaseAlwaysOn, ServerViewTabs.Databases));
        }

        private void databasesAlwaysOnView_GridGroupByBoxVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesAlwaysOnGroupByBoxVisibleButton();
        }

        private void databasesAlwaysOnView_ChartVisibleChanged(object sender, EventArgs e)
        {
            UpdateDatabasesAlwaysOnChartVisibleButton();
        }

        private void UpdateDatabasesAlwaysOnGroupByBoxVisibleButton()
        {
            if (databasesAlwaysOnView != null)
            {
                SetCheckBoxToolState("databasesTabAlwaysOnViewShowHideGroup_toggleDatabasesAlwaysOnGroupByBoxButton", databasesAlwaysOnView.GridGroupByBoxVisible);
            }
        }

        private void UpdateDatabasesAlwaysOnChartVisibleButton()
        {
            if (databasesAlwaysOnView != null)
            {
                SetCheckBoxToolState("databasesTabAlwaysOnViewShowHideGroup_toggleDatabasesAlwaysOnChartsButton", databasesAlwaysOnView.ChartVisible);
            }
        }

        private void ToggleDatabasesAlwaysOnGroupByBox()
        {
            if (databasesAlwaysOnView != null)
            {
                databasesAlwaysOnView.ToggleGroupByBox();
            }
        }

        private void ToggleDatabasesAlwaysOnCharts()
        {
            if (databasesAlwaysOnView != null)
            {
                databasesAlwaysOnView.ChartVisible = !databasesAlwaysOnView.ChartVisible;
            }
        }

        #endregion

        #region Mirroring

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
                databasesMirroringView.ActionsAllowedChanged += new EventHandler(databasesMirroringView_ActionsAllowedChanged);
                UpdateDatabaseMirroringActionButtons();
                UpdateDatabasesMirroredDatabasesGroupByBoxButton();
                UpdateDatabaseMirroringHistoryGroupByBoxButton();
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesMirroringView.MirroredDatabasesGridGroupByBoxVisible, Name = "Databases Group By Box", ShowHideCommand = new RelayCommand((x) => databasesMirroringView.MirroredDatabasesGridGroupByBoxVisible = (bool)x) },
                new ShowHideItem { Checked = databasesMirroringView.MirroringHistoryGridGroupByBoxVisible, Name = "History Group By Box", ShowHideCommand = new RelayCommand((x) => databasesMirroringView.MirroringHistoryGridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabMirroringViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabMirroringViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Mirroring";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(databasesMirroringView, handleActiveViewFilter);
            UpdateDatabasesGroups();
            UpdatePulseGroup();            
            if (_viewModel != null)
            {
                _viewModel.EnableFailOver = databasesMirroringView.ActionsAllowed;
                _viewModel.EnableSuspendResume = databasesMirroringView.ActionsAllowed;
            }
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.DatabasesMirroring, ServerViewTabs.Databases));
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
                SetCheckBoxToolState("databasesTabMirroringViewShowHideGroup_toggleDatabasesMirroredDatabasesGroupByBoxButton", databasesMirroringView.MirroredDatabasesGridGroupByBoxVisible);
            }
        }
        private void UpdateDatabaseMirroringHistoryGroupByBoxButton()
        {
            if (databasesMirroringView != null)
            {
                SetCheckBoxToolState("databasesTabMirroringViewShowHideGroup_toggleDatabaseMirroringHistoryGroupByBoxButton", databasesMirroringView.MirroringHistoryGridGroupByBoxVisible);
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
                if(_viewModel != null)
                {
                    _viewModel.EnableFailOver = databasesMirroringView.ActionsAllowed;
                    _viewModel.EnableSuspendResume = databasesMirroringView.ActionsAllowed;
                }
                SetButtonToolEnabled("databasesTabMirroringViewActionsGroup_databasesTabMirroringViewFailOverButton", databasesMirroringView.ActionsAllowed);
                SetButtonToolEnabled("databasesTabMirroringViewActionsGroup_databasesTabMirroringViewPauseButton", databasesMirroringView.ActionsAllowed);
            }
        }

        void databasesMirroringView_ActionsAllowedChanged(object sender, EventArgs e)
        {
            UpdateDatabaseMirroringActionButtons();
        }

        #endregion

        #region Tempdb

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
                databasesTempdbView.ChartDrilldown += View_ChartDrilldown; //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = databasesTempdbView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => databasesTempdbView.GridGroupByBoxVisible = (bool)x) }
            };
            if (databaseTabControl.SelectedItem != databasesTabTempdbSummaryViewButton)
            {
                ignoreToolClick = true;
                databaseTabControl.SelectedItem = databasesTabTempdbSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Temp DB";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetToggleButtonToolState("databasesTabTempdbViewFilterGroup_filterDatabasesTempdbSessionsButton", config.IsFiltered());
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
                SetCheckBoxToolState("databasesTabTempdbViewShowHideGroup_toggleDatabasesTempdbGroupByBoxButton", databasesTempdbView.GridGroupByBoxVisible);
            }
        }

        #endregion


        #endregion

        #region Services Tab

        private void ShowSelectedServicesView()
        {
            var selectedTool = servicesTabViewsGroup.FindCheckedTool();
            if (selectedTool != null)
            {
                switch (selectedTool.GetId())
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
                var tool = (RadioButtonTool)ribbon.GetToolById("servicesTabViewsGroup_servicesTabSummaryViewButton");
                tool.IsChecked = true;
            }
        }

        private void UpdateServicesGroups()
        {
            servicesTabServicesSummaryActionGroup.Visibility =
                (activeView == servicesSummaryView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabServicesSummaryShowHideGroup.Visibility = activeView == servicesSummaryView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabReplicationShowHideGroup.Visibility = activeView == servicesReplicationView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabAgentJobsActionGroup.Visibility =
                (activeView == servicesSqlAgentJobsView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabAgentJobsFilterGroup.Visibility = activeView == servicesSqlAgentJobsView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabAgentJobsShowHideGroup.Visibility = activeView == servicesSqlAgentJobsView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabReplicationFilterGroup.Visibility = activeView == servicesReplicationView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabReplicationShowHideGroup.Visibility = activeView == servicesReplicationView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabFullTextSearchActionGroup.Visibility =
                (activeView == servicesFullTextSearchView && ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabFullTextSearchShowHideGroup.Visibility = activeView == servicesFullTextSearchView
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
            servicesTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                    ? Visibility.Visible
                                                    : Visibility.Collapsed;
        }

        #region Services Summary

        private void ShowServicesSummaryView()
        {
            if (!string.Equals("Visible", Settings.Default.VisibilityServicesSummary))
            {
                showFirstVisibleServicesServerView();
                return;
            }
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = servicesSummaryView.ChartPanelVisible, Name = "Chart", ShowHideCommand = new RelayCommand((x) => servicesSummaryView.ChartPanelVisible = (bool)x) },
                new ShowHideItem { Checked = servicesSummaryView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => servicesSummaryView.GridGroupByBoxVisible = (bool)x) }
            };
            if (servicesTabControl.SelectedItem != servicesTabSummaryViewButton)
            {
                ignoreToolClick = true;
                servicesTabControl.SelectedItem = servicesTabSummaryViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Summary";
            _viewModel.ResetShowHideItems(showHideItems);
            ShowView(servicesSummaryView);
            UpdateServicesGroups();
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.ServicesSummary, ServerViewTabs.Services));
        }

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

                string startTooltip = "Start service.";
                string stopTooltip = "Stop service.";

                if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null)
                {
                    startTooltip = "Cannot start services on a clustered server.";
                    stopTooltip = "Cannot stop services on a clustered server.";
                }
                SetButtonToolEnabled("servicesTabServicesSummaryActionGroup_startServicesServiceButton", servicesSummaryView.StartAllowed, startTooltip);
                SetButtonToolEnabled("servicesTabServicesSummaryActionGroup_stopServicesServiceButton", servicesSummaryView.StopAllowed, stopTooltip);
                if (!ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin)
                {
                    SetButtonToolEnabled("servicesTabServicesSummaryActionGroup_startServicesServiceButton", false, startTooltip);
                    SetButtonToolEnabled("servicesTabServicesSummaryActionGroup_stopServicesServiceButton", false, stopTooltip);
                }
                ignoreToolClick = false;
                _viewModel.EnableStartButton = servicesSummaryView.StartAllowed;
                _viewModel.EnableStopButton = servicesSummaryView.StopAllowed;
            }
        }

        private void UpdateServicesSummaryGroupByBoxVisibleButton()
        {
            if (servicesSummaryView != null)
            {
                SetCheckBoxToolState("servicesTabServicesSummaryShowHideGroup_toggleServicesSummaryGroupByBoxVisibleButton", servicesSummaryView.GridGroupByBoxVisible);
            }
        }

        private void UpdateServicesSummaryChartVisibleButton()
        {
            if (servicesSummaryView != null)
            {
                SetCheckBoxToolState("servicesTabServicesSummaryShowHideGroup_toggleServicesSummaryChartsButton", servicesSummaryView.ChartPanelVisible);
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

        #endregion

        #region Agent Jobs

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = servicesSqlAgentJobsView.HistoryPanelVisible, Name = "Job History", ShowHideCommand = new RelayCommand((x) => servicesSqlAgentJobsView.HistoryPanelVisible = (bool)x) },
                new ShowHideItem { Checked = servicesSqlAgentJobsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => servicesSqlAgentJobsView.GridGroupByBoxVisible = (bool)x) }
            };
            if (servicesTabControl.SelectedItem != servicesTabSqlAgentJobsViewButton)
            {
                ignoreToolClick = true;
                servicesTabControl.SelectedItem = servicesTabSqlAgentJobsViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Jobs";
            _viewModel.ResetShowHideItems(showHideItems);
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
                string SYSADMIN_MESSAGE = "The SQL DM monitoring account does not have sufficient privileges to manage the SQL Agent.";
                var startText = "Start service.";
                var stopText = "Stop service.";
                var startJobText = "Start Job.";
                var stopJobText = "Stop Job.";

                if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null)
                {
                    startText = @"Cannot start services on a clustered server.";
                    stopText = @"Cannot stop services on a clustered server.";
                }
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startServicesAgentButton", servicesSqlAgentJobsView.StartAllowed, startText);
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopServicesAgentButton", servicesSqlAgentJobsView.StopAllowed, stopText);
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startAgentJobButton", servicesSqlAgentJobsView.StartAllowed, startJobText);
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopAgentJobButton", servicesSqlAgentJobsView.StopAllowed, stopJobText);
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startAgentJobButton", servicesSqlAgentJobsView.IsStartJobAllowed());
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopAgentJobButton", servicesSqlAgentJobsView.IsStopJobAllowed());
                _viewModel.EnableStartServiceButton = servicesSqlAgentJobsView.StartAllowed;
                _viewModel.EnableStopServiceButton = servicesSqlAgentJobsView.StopAllowed;
                _viewModel.EnableStartJobButton = servicesSqlAgentJobsView.IsStartJobAllowed();
                _viewModel.EnableStopJobButton = servicesSqlAgentJobsView.IsStopJobAllowed();
                
                if (!ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin)
                {
                    startText = SYSADMIN_MESSAGE;
                    stopText = SYSADMIN_MESSAGE;
                    startJobText = SYSADMIN_MESSAGE;
                    stopJobText = SYSADMIN_MESSAGE;
                    SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startServicesAgentButton", false, startText);
                    SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopServicesAgentButton", false, stopText);
                    SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startAgentJobButton", false, startJobText);
                    SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopAgentJobButton", false, stopJobText);
                    SetButtonToolVisible("servicesTabAgentJobsActionGroup_startServicesAgentButton", false);
                    SetButtonToolVisible("servicesTabAgentJobsActionGroup_stopServicesAgentButton", false);
                }
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

        private void UpdateServicesAgentServiceActionButtons()
        {
            if (servicesSqlAgentJobsView != null)
            {
                ignoreToolClick = true;
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_startServicesAgentButton", servicesSqlAgentJobsView.StartAllowed);
                SetButtonToolEnabled("servicesTabAgentJobsActionGroup_stopServicesAgentButton", servicesSqlAgentJobsView.StopAllowed);
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesAgentJobsFilterButtons()
        {
            if (servicesSqlAgentJobsView != null)
            {
                AgentJobFilter config = servicesSqlAgentJobsView.Configuration;
                ignoreToolClick = true;
                SetToggleButtonToolState("servicesTabAgentJobsFilterGroup_filterAgentJobsButton", config.IsFiltered());
                SetRadioButtonToolState("servicesTabAgentJobsFilterGroup_showAgentJobsAllButton", config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.All);
                SetRadioButtonToolState("servicesTabAgentJobsFilterGroup_showAgentJobsFailedOnlyButton", config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Failed);
                SetRadioButtonToolState("servicesTabAgentJobsFilterGroup_showAgentJobsRunningOnlyButton", config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Running);
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesAgentJobsHistoryVisibleButton()
        {
            if (servicesSqlAgentJobsView != null)
            {
                SetCheckBoxToolState("servicesTabAgentJobsShowHideGroup_toggleServicesAgentJobsHistoryVisibleButton", servicesSqlAgentJobsView.HistoryPanelVisible);
            }
        }

        private void UpdateServicesAgentJobsGroupByBoxVisibleButton()
        {
            if (servicesSqlAgentJobsView != null)
            {
                SetCheckBoxToolState("servicesTabAgentJobsShowHideGroup_toggleServicesAgentJobsGroupByBoxVisibleButton", servicesSqlAgentJobsView.GridGroupByBoxVisible);
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

        #endregion

        #region Full Text Search

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = servicesFullTextSearchView.DetailsPanelVisible, Name = "Catalog Details", ShowHideCommand = new RelayCommand((x) => servicesFullTextSearchView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = servicesFullTextSearchView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => servicesFullTextSearchView.GridGroupByBoxVisible = (bool)x) }
            };
            if (servicesTabControl.SelectedItem != servicesTabFullTextSearchViewButton)
            {
                ignoreToolClick = true;
                servicesTabControl.SelectedItem = servicesTabFullTextSearchViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Full Text Search";
            _viewModel.ResetShowHideItems(showHideItems);
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

                if (servicesFullTextSearchView.CurrentSnapshot != null && servicesFullTextSearchView.CurrentSnapshot.ProductVersion!=null && servicesFullTextSearchView.CurrentSnapshot.ProductVersion.Major > 9)
                {
                    SetButtonToolVisible("servicesTabFullTextSearchActionGroup_startServicesFullTextSearchButton", false);
                    SetButtonToolVisible("servicesTabFullTextSearchActionGroup_stopServicesFullTextSearchButton", false);
                }
                else
                {
                    SetButtonToolVisible("servicesTabFullTextSearchActionGroup_startServicesFullTextSearchButton", true);
                    SetButtonToolVisible("servicesTabFullTextSearchActionGroup_stopServicesFullTextSearchButton", true);
                    SetButtonToolEnabled("servicesTabFullTextSearchActionGroup_startServicesFullTextSearchButton", servicesFullTextSearchView.StartAllowed);
                    SetButtonToolEnabled("servicesTabFullTextSearchActionGroup_stopServicesFullTextSearchButton", servicesFullTextSearchView.StopAllowed);
                }
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesFullTextSearchCatalogActionButtons()
        {
            if (servicesFullTextSearchView != null)
            {
                ignoreToolClick = true;
                SetButtonToolVisible("servicesTabFullTextSearchActionGroup_optimizeFullTextSearchCatalogButton", servicesFullTextSearchView.OptimizeAvailable);
                SetButtonToolVisible("servicesTabFullTextSearchActionGroup_repopulateFullTextSearchCatalogButton", servicesFullTextSearchView.RepopulateAvailable);
                SetButtonToolVisible("servicesTabFullTextSearchActionGroup_rebuildFullTextSearchCatalogButton", servicesFullTextSearchView.RebuildAvailable);

                SetButtonToolEnabled("servicesTabFullTextSearchActionGroup_optimizeFullTextSearchCatalogButton", servicesFullTextSearchView.ActionsAllowed);
                SetButtonToolEnabled("servicesTabFullTextSearchActionGroup_repopulateFullTextSearchCatalogButton", servicesFullTextSearchView.ActionsAllowed);
                SetButtonToolEnabled("servicesTabFullTextSearchActionGroup_rebuildFullTextSearchCatalogButton", servicesFullTextSearchView.ActionsAllowed);
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesFullTextSearchGroupByBoxVisibleButton()
        {
            if (servicesFullTextSearchView != null)
            {
                SetCheckBoxToolState("servicesTabFullTextSearchShowHideGroup_toggleServicesFullTextSearchGroupByBoxVisibleButton", servicesFullTextSearchView.GridGroupByBoxVisible);
            }
        }

        private void UpdateServicesFullTextSearchDetailsVisibleButton()
        {
            if (servicesFullTextSearchView != null)
            {
                SetCheckBoxToolState("servicesTabFullTextSearchShowHideGroup_toggleServicesFullTextSearchDetailsButton", servicesFullTextSearchView.DetailsPanelVisible);
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

        #endregion

        #region Replication

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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = servicesReplicationView.TopologyGridGroupByBoxVisible, Name = "Topology Group By Box", ShowHideCommand = new RelayCommand((x) => servicesReplicationView.TopologyGridGroupByBoxVisible = (bool)x) },
                new ShowHideItem { Checked = servicesReplicationView.DistributorGridGroupByBoxVisible, Name = "Non-Subscribed Queue Group By Box", ShowHideCommand = new RelayCommand((x) => servicesReplicationView.DistributorGridGroupByBoxVisible = (bool)x) },
                new ShowHideItem { Checked = servicesReplicationView.ReplicationGraphsVisible, Name = "Replication Charts", ShowHideCommand = new RelayCommand((x) => servicesReplicationView.ReplicationGraphsVisible = (bool)x) }
            };
            if (servicesTabControl.SelectedItem != servicesTabReplicationViewButton)
            {
                ignoreToolClick = true;
                servicesTabControl.SelectedItem = servicesTabReplicationViewButton;
                ignoreToolClick = false;
            }
            _viewModel.ChildBreadcrumb = "Replication";
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetToggleButtonToolState("servicesTabReplicationFilterGroup_filterReplicationButton", config.IsFiltered());
                ignoreToolClick = false;
            }
        }

        private void UpdateServicesReplicationPublisherGroupByBoxVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckBoxToolState("servicesTabReplicationShowHideGroup_toggleServicesReplicationTopologyGroupByBoxVisibleButton", servicesReplicationView.TopologyGridGroupByBoxVisible);
            }
        }
        private void UpdateServicesReplicationDistributorGroupByBoxVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckBoxToolState("servicesTabReplicationShowHideGroup_toggleServicesReplicationDistributorGroupByBoxVisibleButton", servicesReplicationView.DistributorGridGroupByBoxVisible);
            }
        }
        private void UpdateServicesReplicationGraphsVisibleButton()
        {
            if (servicesReplicationView != null)
            {
                SetCheckBoxToolState("servicesTabReplicationShowHideGroup_toggleServicesReplicationReplicationGraphsVisibleButton", servicesReplicationView.ReplicationGraphsVisible);
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

        #endregion

        #region Logs Tab

        private void UpdateLogsGroups()
        {
            logsTabActionsGroup.Visibility = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
                                                   ? Visibility.Visible
                                                   : Visibility.Collapsed;
            logsTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                   ? Visibility.Visible
                                                   : Visibility.Collapsed;
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
            var showHideItems = new ShowHideItem[] {
                new ShowHideItem { Checked = logsView.AvailableLogsPanelVisible, Name = "Logs", ShowHideCommand = new RelayCommand((x) => logsView.AvailableLogsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = logsView.DetailsPanelVisible, Name = "Details", ShowHideCommand = new RelayCommand((x) => logsView.DetailsPanelVisible = (bool)x) },
                new ShowHideItem { Checked = logsView.GridGroupByBoxVisible, Name = "Group By Box", ShowHideCommand = new RelayCommand((x) => logsView.GridGroupByBoxVisible = (bool)x) }
            };
            _viewModel.ResetShowHideItems(showHideItems);
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
                SetToggleButtonToolState("logsTabFilterGroup_filterLogsButton", config.IsFiltered());
                SetCheckBoxToolState("logsTabFilterGroup_filterLogsShowErrorsButton", config.ShowErrors);
                SetCheckBoxToolState("logsTabFilterGroup_filterLogsShowWarningsButton", config.ShowWarnings);
                SetCheckBoxToolState("logsTabFilterGroup_filterLogsShowInformationalButton", config.ShowInformational);
                ignoreToolClick = false;
            }
        }

        private void UpdateLogsViewAvailableLogsVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckBoxToolState("logsTabShowHideGroup_toggleAvailableLogsButton", logsView.AvailableLogsPanelVisible);
            }
        }

        private void UpdateLogsViewDetailsVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckBoxToolState("logsTabShowHideGroup_toggleLogDetailsButton", logsView.DetailsPanelVisible);
            }
        }

        private void UpdateLogsViewGroupByBoxVisibleButton()
        {
            if (logsView != null)
            {
                SetCheckBoxToolState("logsTabShowHideGroup_toggleLogGroupByBoxButton", logsView.GridGroupByBoxVisible);
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

        #region Analyze

        private void ShowSelectedRunAnalysisView()
        {
            /** SQLDM-30184 **/
            var ribbonTool = ribbon.GetToolById("analyzeTabRunGroup_analysisRunAnalysisButton");
            ButtonTool selectedTool = ribbonTool is ButtonTool ? (ButtonTool)ribbonTool : null;
            bool isDiagnoseQueryBlank = string.IsNullOrWhiteSpace(SqlTextDialog.DiagnoseQuery);

            if (selectedTool != null && !isDiagnoseQueryBlank)
            {
                switch (selectedTool.GetId())
                {
                    case "analysisRunAnalysisButton":
                        ShowAnalysisView(false, isDiagnoseQueryBlank);
                        break;
                    //To run work load analysis
                    case "analysisWorkloadAnalysisButton":
                        ShowAnalysisView(true);
                        break;
                    //To set default view the same as run analysis temporary (need to decide default view)
                    default:
                        ShowAnalysisDefaultView();
                        break;
                }
            }
            else
            {
                ShowAnalysisDefaultView();
                //tool.IsEnabled = false;
            }
        }

        private void UpdateAnalyzeGroups()
        {
            analyzeTabRunGroup.Visibility = Visibility.Visible;
            analyzeTabViewsGroup.Visibility = Visibility.Visible;
            //analyzeTabRunGroup.Visibility =
            //    activeView == serverSummaryView && !serverSummaryView.HistoricalSnapshotDateTime.HasValue &&
            //    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;
            analyzeTabPulseGroup.Visibility = ApplicationModel.Default.IsPulseConfigured
                                                    ? Visibility.Visible
                                                 : Visibility.Collapsed;
        }

        #region HandleOptimization/UndoEnabling
        // Handling button tools enabling using rows selection event from analysis UI
        //Srishti Purohit
        private void UpdateScriptAnalysisActionButtons()
        {
            if (recommendationAnalysis != null)
            {
                ignoreToolClick = true;

                SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabShowProblem", recommendationAnalysis.ShowProblemAllowed);
                _viewModel.EnableAnalyzeShowProblemButton = recommendationAnalysis.ShowProblemAllowed;
                SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabUndoScript", recommendationAnalysis.UndoAllowed);
                _viewModel.EnableAnalyzeUndoScriptButton = recommendationAnalysis.UndoAllowed;
                SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabOptimizeScript", recommendationAnalysis.OptimizationAllowed);
                _viewModel.EnableAnalyzeOptimizeScriptButton = recommendationAnalysis.OptimizationAllowed;
                analyzeTabRunGroup.IsEnabled = true;
                _viewModel.EnableAnalyzeRunButton = recommendationAnalysis.IsAnalysisDone;
                _viewModel.EnableAnalyzeUndoButton = recommendationAnalysis.UndoAllowed;
                /** SQLDM-30184 **/
                SetButtonToolEnabled("analyzeTabRunGroup_analysisRunAnalysisButton", recommendationAnalysis.IsAnalysisDone);
                /** SQLDM-30184 - Button is removed from UI **/
                // SetButtonToolEnabled("analyzeTabRunGroup_analysisWorkloadAnalysisButton", recommendationAnalysis.IsAnalysisDone);

                SetButtonToolEnabled("analyzeTabHistoryGroup_showPreviousSnapshotButton", recommendationAnalysis.IsAnalysisDone);
                SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", recommendationAnalysis.IsAnalysisDone);
                SetToggleButtonToolEnable("analyzeTabHistoryGroup_toggleHistoryBrowserButton", recommendationAnalysis.IsAnalysisDone);
                ignoreToolClick = false;
            }
            else
            {
                _viewModel.EnableAnalyzeRunButton = false;
                _viewModel.EnableAnalyzeUndoButton = false;
            }
        }

        void scriptAnalysisView_AnalysisActionAllowedChanged(object sender, EventArgs e)
        {
            UpdateScriptAnalysisActionButtons();
        }
        #endregion

        #region Run Analysis

        private void ShowAnalysisView(bool isWorkloadAnalysis, bool initialize = true)
        {
            if (SqlTextDialog.DiagnoseQuery != null)
            {
                _viewModel.HistoryBrowserVisible = false;
                HistoricalSnapshotDateTime = null;
                recommendationAnalysis = initialize ? new Analysis.Recommendations(SqlTextDialog.DiagnoseQuery, SqlTextDialog.Database, instanceId) : recommendationAnalysis;
            }
            else
            {
                recommendationAnalysis = initialize ? new Analysis.Recommendations(instanceId, HistoricalSnapshotDateTime) : recommendationAnalysis;
            }
            defaultAnalysisView = null;

            recommendationAnalysis.ScriptAnalysisActionAllowedChanged += new EventHandler(scriptAnalysisView_AnalysisActionAllowedChanged);
            scriptAnalysisView_AnalysisActionAllowedChanged(this, EventArgs.Empty);
            ShowView(recommendationAnalysis);
            SetButtonStateOfAnalysisView(true);
            SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", true);
            UpdateAnalyzeGroups();
            //UpdateHistoryGroupOptions(true);
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
        }

        private void ShowAnalysisDefaultView()
        {
            //if (recommendationAnalysis == null)
            //{
            _viewModel.DrillOutButtonVisible = false;
            defaultAnalysisView = new DefaultScreenAnalysisTab(instanceId, viewHost, false, _viewModel);

            // Skip Permissions UI for Cloud providers           
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.AmazonRDSId && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                this.analyzeOperationalStatusImage = new System.Windows.Forms.PictureBox();
                this.analyzeOperationalStatusImage.BackColor = System.Drawing.Color.LightGray;
                this.analyzeOperationalStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
                this.analyzeOperationalStatusImage.Location = new System.Drawing.Point(7, 5);
                this.analyzeOperationalStatusImage.Name = "operationalStatusImage";
                this.analyzeOperationalStatusImage.Size = new System.Drawing.Size(16, 16);
                this.analyzeOperationalStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                this.analyzeOperationalStatusImage.TabIndex = 3;
                this.analyzeOperationalStatusImage.TabStop = false;

                this.analyzeOperationalStatusLabel = new System.Windows.Forms.Label();
                this.analyzeOperationalStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.analyzeOperationalStatusLabel.BackColor = System.Drawing.Color.LightGray;
                this.analyzeOperationalStatusLabel.ForeColor = System.Drawing.Color.Black;
                this.analyzeOperationalStatusLabel.Location = new System.Drawing.Point(4, 3);
                this.analyzeOperationalStatusLabel.Name = "analyseOperationalStatusLabel";
                this.analyzeOperationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
                this.analyzeOperationalStatusLabel.Size = new System.Drawing.Size(1065, 21);
                this.analyzeOperationalStatusLabel.TabIndex = 1;
                this.analyzeOperationalStatusLabel.Text = "Some analysis was skipped due to insufficient permissions. Click here to see what was skipped";
                this.analyzeOperationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                this.analyzeOperationalStatusLabel.MouseDown += new MouseEventHandler(this.analyzeOperationalStatusLabel_MouseDown);
                this.analyzeOperationalStatusLabel.MouseEnter += new System.EventHandler(this.analyzeOperationalStatusLabel_MouseEnter);
                this.analyzeOperationalStatusLabel.MouseLeave += new System.EventHandler(this.analyzeOperationalStatusLabel_MouseLeave);
                this.analyzeOperationalStatusLabel.MouseUp += new MouseEventHandler(this.analyzeOperationalStatusLabel_MouseUp);

                this.analyzeOperationalStatusPanel = new Panel();
                this.analyzeOperationalStatusPanel.SuspendLayout();
                this.analyzeOperationalStatusPanel.Controls.Add(this.analyzeOperationalStatusImage);
                this.analyzeOperationalStatusPanel.Controls.Add(this.analyzeOperationalStatusLabel);
                this.analyzeOperationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
                this.analyzeOperationalStatusPanel.Location = new System.Drawing.Point(0, 0);
                this.analyzeOperationalStatusPanel.Name = "operationalStatusPanel";
                this.analyzeOperationalStatusPanel.Size = new System.Drawing.Size(1073, 27);
                this.analyzeOperationalStatusPanel.TabIndex = 3;
                this.analyzeOperationalStatusPanel.Visible = true;
                this.defaultAnalysisView.Controls.Add(analyzeOperationalStatusPanel);
            }
            ShowView(defaultAnalysisView);
            if (defaultAnalysisView.IsAnalyzeEnabled)
            {
                SetButtonStateOfAnalysisView(true);
                _viewModel.DrillOutButtonVisible = false;
            }
            else
                SetButtonStateOfAnalysisView(false);
            //}
            //else
            //{
            //    ShowView(recommendationAnalysis);
            //    SetButtonStateOfAnalysisView(true);
            //}
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabShowProblem", false);
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabUndoScript", false);
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabOptimizeScript", false);
            _viewModel.EnableAnalyzeOptimizeScriptButton = false;
            _viewModel.EnableAnalyzeUndoScriptButton = false;
            _viewModel.EnableAnalyzeShowProblemButton = false;
            SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", false);
            // analyzeTabRunGroup.IsEnabled = ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin;
            UpdateAnalyzeGroups();
            //UpdateHistoryGroupOptions(true);
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
        }

        private void ShowAnalysisDefaultView(List<IRecommendation> ListOfRecommendations)
        {
            if (recommendationAnalysis == null)
            {
                defaultAnalysisView = new DefaultScreenAnalysisTab(instanceId, viewHost, false);
                ShowView(defaultAnalysisView);
                SetButtonStateOfAnalysisView(false);
            }
            else
            {
                ShowView(recommendationAnalysis);
                SetButtonStateOfAnalysisView(true);
            }
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabShowProblem", false);
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabUndoScript", false);
            SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabOptimizeScript", false);
            _viewModel.EnableAnalyzeOptimizeScriptButton = false;
            _viewModel.EnableAnalyzeUndoScriptButton = false;
            _viewModel.EnableAnalyzeShowProblemButton = false;
            SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", false);
            // analyzeTabRunGroup.IsEnabled = ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin;
            UpdateAnalyzeGroups();
            //UpdateHistoryGroupOptions(true);
            UpdatePulseGroup();
            OnSubViewChanged(new SubViewChangedEventArgs(ServerViews.RunAnalysis, ServerViewTabs.Analysis));
        }

        private void analyzeOperationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            analyzeOperationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            analyzeOperationalStatusLabel.BackColor = System.Drawing.Color.FromArgb(255, 189, 105);
            analyzeOperationalStatusImage.BackColor = System.Drawing.Color.FromArgb(255, 189, 105);
        }

        private void analyzeOperationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            analyzeOperationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            analyzeOperationalStatusLabel.BackColor = System.Drawing.Color.FromArgb(211, 211, 211);
            analyzeOperationalStatusImage.BackColor = System.Drawing.Color.FromArgb(211, 211, 211);
        }

        private void analyzeOperationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            analyzeOperationalStatusLabel.ForeColor = System.Drawing.Color.White;
            analyzeOperationalStatusLabel.BackColor = System.Drawing.Color.FromArgb(251, 140, 60);
            analyzeOperationalStatusImage.BackColor = System.Drawing.Color.FromArgb(251, 140, 60);
        }

        private void analyzeOperationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            analyzeOperationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            analyzeOperationalStatusLabel.BackColor = System.Drawing.Color.FromArgb(255, 189, 105);
            analyzeOperationalStatusImage.BackColor = System.Drawing.Color.FromArgb(255, 189, 105);

            if (HistoricalSnapshotDateTime != null)
            {
                analyzeOperationalStatusPanel.Visible = false;
                ApplicationController.Default.SetActiveViewToRealTimeMode();
            }
            else
            {
                ShowSkippedChecks();
                /*
                 * BackupAndRecovery2005
                    BufferPoolExtIO2014
                    ColumnStoreIndex2016
                    Configuration2012
                    DBSecurity2005
                    DependentObjectsPrescriptiveRecommendation
                    DisabledIndexes2005
                    FilteredIndex2008
                    FragmentedIndexes2005
                    GetAdhocCachedPlanBytes2005
                    GetLockedPageKB2005
                    GetLockedPageKB2008
                    GetWorstFillFactorIndexes2005
                    HashIndex2014
                    HighCPUTimeProcedure2016
                    HighIndexUpdates2005
                    IndexContention2005
                    LargeTableStats2008
                    NonIncrementalColumnStatOnPartitionedTable2014
                    NUMANodeCounters2005
                    OutOfDateStats2005
                    OverlappingIndexes2005
                    OutOfDateStats2005
                    OverlappingIndexes2008
                    QueryAnalyzer2016
                    QueryPlanEstRows2005
                    QueryStore2016
                    RarelyUsedIndexOnInMemoryTable2014
                    ServerConfiguration2014
                    ServerConfiguration2016
                    SQLModuleOptions2005

                 */
            }
        }

        public void ShowSkippedChecks()
        {
            string skippedCheckList = @"Unable to run analysis as the following probes require elevated permissions:

                    BackupAndRecovery2005
                    BufferPoolExtIO2014
                    ColumnStoreIndex2016
                    Configuration2012
                    DBSecurity2005
                    DependentObjectsPrescriptiveRecommendation
                    DisabledIndexes2005
                    FilteredIndex2008
                    FragmentedIndexes2005
                    GetAdhocCachedPlanBytes2005
                    GetLockedPageKB2005
                    GetLockedPageKB2008
                    GetWorstFillFactorIndexes2005
                    HashIndex2014
                    HighCPUTimeProcedure2016
                    HighIndexUpdates2005
                    IndexContention2005
                    LargeTableStats2008
                    NonIncrementalColumnStatOnPartitionedTable2014
                    NUMANodeCounters2005
                    OutOfDateStats2005
                    OverlappingIndexes2005
                    OutOfDateStats2005
                    OverlappingIndexes2008
                    QueryAnalyzer2016
                    QueryPlanEstRows2005
                    QueryStore2016
                    RarelyUsedIndexOnInMemoryTable2014
                    ServerConfiguration2014
                    ServerConfiguration2016
                    SQLModuleOptions2005";
            ApplicationMessageBox.ShowWarning(this.GetWinformWindow(), skippedCheckList, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.OK);
        }
        #endregion
        #endregion

        #region Dashboard Tab

        private const string RibbonTabDashboardDesign = @"Dashboard";

        private bool UpdateDashboardTab(bool show)
        {
            var tab = (RibbonTabItem)ribbon.FindName(RibbonTabDashboardDesign);
            if (tab != null)
            {
                tab.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                return true;
            }
            return false;
        }

        private void UpdateDashboardGroups()
        {
            if (UpdateDashboardTab(true))
            {
                switch (_viewModel.DashboardMode)
                {
                    case DashboardMode.Gallery:
                        dashboardTabGalleryActionsGroup.Visibility = Visibility.Visible;
                        dashboardTabGalleryFiltersGroup.Visibility = Visibility.Visible;
                        dashboardTabDesignerActionsGroup.Visibility = Visibility.Collapsed;
                        dashboardTabDesignerShowHideGroup.Visibility = Visibility.Collapsed;
                        break;
                    case DashboardMode.Design:
                        dashboardTabGalleryActionsGroup.Visibility = Visibility.Collapsed;
                        dashboardTabGalleryFiltersGroup.Visibility = Visibility.Collapsed;
                        dashboardTabDesignerActionsGroup.Visibility = Visibility.Visible;
                        dashboardTabDesignerShowHideGroup.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        #region Dashboard Layout View

        private void ShowDashboardLayoutView()
        {
            _viewModel.DashboardMode = DashboardMode.Gallery;
            UpdateDashboardGroups();

            if (_dashboardLayoutGallery == null)
            {
                _dashboardLayoutGallery = new DashboardLayoutGallery(instanceId);
                _dashboardLayoutGallery.SelectionChanged += dashboardLayoutGalleryView_SelectionChanged;
                _dashboardLayoutGallery.DataContext = new DashboardLayoutGalleryViewModel(serverSummaryView);
                _dashboardLayoutGallery.DesignTab = RibbonTabDashboardDesign;
            }

            UpdateDashboardGalleryButtons();

            //viewHost.Add((UIElement)_dashboardLayoutGallery);
            ShowView(_dashboardLayoutGallery);
        }

        private void SelectDashboardGalleryView()
        {
            serverSummaryView.SetArgument(_dashboardLayoutGallery.SelectedDashboardLayoutID);
            CloseDashboardGalleryView();
        }

        private void CloseDashboardGalleryView()
        {
            ShowView(serverSummaryView);
            _dashboardLayoutGallery.SelectionChanged -= dashboardLayoutGalleryView_SelectionChanged;
            _dashboardLayoutGallery.Dispose();
            _dashboardLayoutGallery = null;
            this.toggleCustomDashboard.IsChecked = false;
            EnableDisableCustomizeDashboard(false);
            _viewModel.ShowDashGalleryButtons = false;

        }
        private void ToggleDashboardFilter(DashboardFilter filter)
        {
            if (_dashboardLayoutGallery != null)
            {
                _dashboardLayoutGallery.Filter = filter;
            }
        }

        private void SetDashboardAsServerDefault()
        {
            if (_dashboardLayoutGallery != null)
            {
                _dashboardLayoutGallery.SetAsServerDefault();
            }
        }

        private void SetDashboardAsGlobalDefault()
        {
            if (_dashboardLayoutGallery != null)
            {
                _dashboardLayoutGallery.SetAsGlobalDefault();
            }
        }

        private void DeleteDashboard()
        {
            if (_dashboardLayoutGallery != null)
            {
                _dashboardLayoutGallery.DeleteDashboard();
            }
        }

        private void dashboardLayoutGalleryView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDashboardGalleryButtons();
        }

        private void UpdateDashboardGalleryButtons()
        {
            if (_dashboardLayoutGallery != null)
            {
                SetButtonToolEnabled("dashboardTabGalleryActionsGroup_deleteDashboardButton",
                                     _dashboardLayoutGallery.DeleteAllowed);
            }
        }

        #endregion

        #region Dashboard Designer View

        private void ShowDashboardDesignView()
        {
            if (serverSummaryView != null)
            {
                if (_viewModel.HistoryBrowserVisible)
                {
                    _viewModel.HistoryBrowserVisible = false;
                }
                serverSummaryView.DesignTab = RibbonTabDashboardDesign;
                serverSummaryView.ToggleDesignMode(true);
                _viewModel.DashboardMode = DashboardMode.Design;
                UpdateDashboardGroups();
            }
        }

        private void HideDashboardDesignView()
        {
            if (serverSummaryView != null)
            {
                serverSummaryView.CheckIfSaveNeeded();
                serverSummaryView.ToggleDesignMode(false);
                ShowServerSummaryView();
                var tab = (RibbonTabItem)ribbon.FindName("Overview");
                if (tab != null)
                {
                    tab.IsSelected = true;
                }
                UpdateDashboardTab(false);
            }
        }

        private void dashboardView_PanelGalleryVisibleChanged(object sender, EventArgs e)
        {
            UpdatePanelGalleryButton();
        }

        private void UpdatePanelGalleryButton()
        {
            if (serverSummaryView != null)
            {
                SetToggleButtonToolState("dashboardTabDesignerShowHideGroup_togglePanelGalleryButton", serverSummaryView.PanelGalleryVisible);
            }
        }

        private void SelectDashboardLayout(Size size)
        {
            if (serverSummaryView != null)
            {
                serverSummaryView.SetDashboardLayout(size);
            }
        }

        private void TogglePanelGallery()
        {
            if (serverSummaryView != null)
            {
                serverSummaryView.PanelGalleryVisible = !serverSummaryView.PanelGalleryVisible;
                if (serverSummaryView.PanelGalleryVisible)
                {
                    serverSummaryView.ToggleDesignMode(true);
                }
            }
        }

        private void SaveDashboardDesign()
        {
            if (serverSummaryView != null)
            {
                serverSummaryView.SaveDashboardDesign();
            }
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

        public void UpdatePulseGroup()
        {
            _viewModel.UpdatePulseGroup();
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
                            bool followed = _viewModel.PulseFollowServerButtonText == "Unfollow Server";

                            PulseController.Default.SetFollowing(pulseId, !followed);
                            UpdatePulseFollowButton(pulseId);

                            bool followedNew = _viewModel.PulseFollowServerButtonText == "Unfollow Server";

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
            bool following = PulseController.Default.GetFollowing(pulseId);

            _viewModel.IsFollowingServer = following;
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
        //private void toolbarsManager_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        //{
        //    if (activeElement == e.Element) return;
        //    if (e.Element is ImageAndTextUIElement.ImageAndTextDependentImageUIElement)
        //    {
        //        if (activeElement != null)
        //            tooltip.Hide();
        //        ShowTooltip(e.Element);
        //    }
        //    else
        //    {
        //        if (activeElement != null)
        //        {
        //            activeElement = null;
        //            tooltip.Hide();
        //        }
        //    }

        //}

        bool ConfigureTooltip(string tabKey)
        {
            //var status = activeStatus;
            //if (activeStatus == null)
            //    status = ApplicationModel.Default.GetInstanceStatus(instanceId);
            //if (status == null)
            //    return false;


            //MetricCategory category = MetricCategory.All;
            //switch (tabKey)
            //{
            //    case "Queries":
            //        category = MetricCategory.Queries;
            //        break;
            //    case "Sessions":
            //        category = MetricCategory.Sessions;
            //        break;
            //    case "Resources":
            //        category = MetricCategory.Resources;
            //        break;
            //    case "Databases":
            //        category = MetricCategory.Databases;
            //        break;
            //    case "Services":
            //        category = MetricCategory.Services;
            //        break;
            //    case "Logs":
            //        category = MetricCategory.Logs;
            //        break;
            //    default:
            //        tooltip.ToolTipText = status.ToolTip;
            //        tooltip.ToolTipTitle = status.ToolTipHeading;
            //        tooltip.CustomToolTipImage = status.ToolTipHeadingImage;
            //        return true;
            //}
            //var issues = status[category];
            //if (issues != null && issues.Count > 0)
            //{
            //    var issueArray = Algorithms.ToArray(issues);
            //    tooltip.ToolTipText = status.GetToolTip(category);
            //    tooltip.ToolTipTitle = String.Format("{0} are {1}", category, issueArray[0].Severity);
            //    tooltip.CustomToolTipImage = MonitoredSqlServerStatus.GetSeverityImage(issueArray[0].Severity);
            //    return true;
            //}
            return false;
        }

        private void ShowTooltip(UIElement uiElement)
        {

            //var tabElement = (RibbonTabItemUIElement)uiElement.GetAncestor(typeof(RibbonTabItemUIElement));
            //if (tabElement == null) return;
            //var ribbonTab = tabElement.TabItem as RibbonTab;
            //if (ribbonTab == null) return;

            //if (ConfigureTooltip(ribbonTab.Key))
            //{
            //    activeElement = uiElement;
            //    var parent = uiElement.Parent;
            //    var point = this.PointToScreen(new Point(uiElement.Rect.X, parent.RectInsideBorders.Bottom));
            //    tooltip.Show(
            //        point,
            //        0,
            //        true,
            //        true,
            //        true,
            //        Rectangle.Empty,
            //        false,
            //        false,
            //        false);
            //}
        }


        //private void toolbarsManager_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        //{
        //    if (activeElement != null || tooltip.IsVisible)
        //    {
        //        activeElement = null;
        //        tooltip.Hide();
        //    }
        //}

        private IWin32Window ParentForm
        {
            get { return Application.Current.MainWindow.GetWinformWindow(); }
        }

        private void SetButtonToolEnabled(string toolId, bool isEnabled)
        {
            SetButtonToolEnabled(toolId, isEnabled, null);
        }

        private void SetButtonToolEnabled(string toolId, bool isEnabled, string tooltip)
        {
            var tool = ribbon.GetToolById(toolId) as ButtonTool;
            if (tool == null) return;               
                tool.IsEnabled = isEnabled;
            if (tooltip != null)
                tool.ToolTip = tooltip;
        }

        private void SetButtonToolVisible(string toolId, bool isVisible)
        {
            var tool = ribbon.GetToolById(toolId) as ButtonTool;
            if (tool == null) return;
            tool.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetCheckBoxToolState(string toolId, bool isChecked)
        {
            var tool = ribbon.GetToolById(toolId) as CheckBoxTool;
            if (tool == null) return;
            tool.IsChecked = isChecked;
        }

        private void SetRadioButtonToolState(string toolId, bool isChecked)
        {
            var tool = ribbon.GetToolById(toolId) as RadioButtonTool;
            if (tool == null) return;
            tool.IsChecked = isChecked;
        }
        private void SetRadioButtonToolEnable(string toolId, bool isEnabled)
        {
            var tool = ribbon.GetToolById(toolId) as RadioButtonTool;
            if (tool == null) return;
            tool.IsEnabled = isEnabled;
        }

        private void SetToggleButtonToolState(string toolId, bool isChecked)
        {
            var tool = ribbon.GetToolById(toolId) as ToggleButtonTool;
            if (tool == null) return;
            tool.IsChecked = isChecked;
        }
        private void SetToggleButtonToolEnable(string toolId, bool isEnabled)
        {
            var tool = ribbon.GetToolById(toolId) as ToggleButtonTool;
            if (tool == null) return;
            tool.IsEnabled = isEnabled;
        }

        #region History Browser

        private enum SnapshotBoundaryCondition
        {
            Earliest,
            MostRecent
        }

        private void historyBrowserControl_HistoricalSnapshotSelected(object sender, HistoricalSnapshotSelectedEventArgs e)
        {
            historyBrowserSnapshotChanged = true;
            HistoricalSnapshotDateTime = e.SnapshotDateTime;
            historyBrowserSnapshotChanged = false;
        }

        private void QuickHistorialSnapshotTextChange(object sender, HistoricalSnapshotSelectedEventArgs e)
        {
            _viewModel.QuickHistoricalSnapshotText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(e.SnapshotDateTime, e.SnapshotDateTime);
        }

        /// <summary>
        /// SqlDM 10.2(Anshul Aggarwal) : New History Browser
        /// Handles custom range selection event.
        /// </summary>
        private void historyBrowserControl_HistoricalCustomRangeSelected(object sender, EventArgs e)
        {
            if (ApplicationModel.Default.HistoryTimeValue.ViewMode == ServerViewMode.Custom)
            {
                if (ApplicationModel.Default.HistoryTimeValue.StartDateTime.HasValue && ApplicationModel.Default.HistoryTimeValue.EndDateTime.HasValue)
                {
                    _viewModel.QuickHistoricalSnapshotText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(ApplicationModel.Default.HistoryTimeValue.StartDateTime.Value,
                                                                                                                    ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value);
                }

                SetCustomStartEndDateTime(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                    ApplicationModel.Default.HistoryTimeValue.EndDateTime);
            }
            else
            {
                DateTime endTime = DateTime.Now;
                DateTime startTime = endTime.AddMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes * (-1));
                _viewModel.QuickHistoricalSnapshotText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(startTime, endTime);

                SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
            }
        }

        private void historyBrowserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // A pair of objects is passed in as an argument. This first object in the pair is the DateTime from which to navigate;
            // the second object in the pair indicates if the navigation is to be earlier than the specified DateTime, 
            // otherwise it will be later than the DateTime.

            if (e.Argument is Pair<DateTime, bool>)
            {
                Pair<DateTime, bool> argument = (Pair<DateTime, bool>)e.Argument;

                if (argument.Second)
                {
                    DateTime? result = null;
                    //For analysis tab
                    if (!ApplicationModel.Default.AnalysisHistoryMode)
                        result = RepositoryHelper.GetPreviousServerActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    else
                        result = RepositoryHelper.GetPreviousAnalysisActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);


                    if (result.HasValue)
                    {
                        e.Result = result.Value;
                    }
                    else
                    {
                        e.Result = SnapshotBoundaryCondition.Earliest;
                    }
                }
                else
                {
                    DateTime? result = null;
                    //For analysis tab
                    if (!ApplicationModel.Default.AnalysisHistoryMode)
                        result = RepositoryHelper.GetNextServerActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    else
                        result = RepositoryHelper.GetNextAnalysisActivitySnapshotDateTime(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, argument.First);
                    if (result.HasValue)
                    {
                        e.Result = result.Value;
                    }
                    else
                    {
                        e.Result = SnapshotBoundaryCondition.MostRecent;
                    }
                }
            }
            else
            {
                throw new DesktopClientException("Invalid argument type passed to historyBrowserBackgroundWorker_DoWork method.");
            }
        }

        private void historyBrowserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result is DateTime)
                {
                    HistoricalSnapshotDateTime = (DateTime)e.Result;
                }
                else if (e.Result is SnapshotBoundaryCondition)
                {
                    switch ((SnapshotBoundaryCondition)e.Result)
                    {
                        case SnapshotBoundaryCondition.Earliest:
                            ApplicationMessageBox.ShowInfo(ParentForm,
                                                           "You are viewing the earliest historical snapshot for this SQL Server instance.");
                            break;
                        case SnapshotBoundaryCondition.MostRecent:
                            //If active view is session blocking view disable HistoryBrowserEnableNext button
                            if (activeView == sessionsBlockingView)
                            {
                                _viewModel.HistoryBrowserEnableNext = false;
                                break;
                            }
                            //If active view is analysis view disable HistoryBrowserEnableNext button
                            //so switch to real time view not needed, as user can directly go to Real time view by clicking on Run Analysis or Run workoad.
                            //10.0 SQLdm Srishti Purohit
                            if (activeView == recommendationAnalysis)
                            {
                                SetButtonToolEnabled("analyzeTabHistoryGroup_showNextSnapshotButton", false);
                                break;
                            }
                            if (ApplicationMessageBox.ShowQuestion(ParentForm,
                                    "You are viewing the most recent historical snapshot for this SQL Server instance. Would you like to switch to Real Time Mode?") == DialogResult.Yes)
                            {
                                ApplicationController.Default.SetActiveViewToRealTimeMode();

                            }
                            break;
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm, "An error occurred while retreiving a historical snapshot.", e.Error);
            }
        }

        private void ToggleHistoryBrowser()
        {
            UpdateHistoryGroupOptions(true);
        }

        private void historyBrowserControl_CloseButtonClicked(object sender, EventArgs e)
        {
            _viewModel.HistoryBrowserVisible = false;
        }

        //SQLDM-28019: Code change to allow caching of historybrowser width based on this new param value
        private void historyBrowserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateHistoryBrowserVisibility(true);
        }

        //SQLDM-28019: Removed the code introduced earlier which is hardcoding the length to small size
        private GridLength historyBrowserColumnRestoreWidth = new GridLength(250.0d);

        /// <summary>
        /// Update History Browser Visibility - called when container is loaded and Visibility changed
        /// </summary>
        /// <param name="visibilityChangedEvent">
        /// To ensure caching of historybrowser width only in case of Visibility changed
        /// </param>
        private void UpdateHistoryBrowserVisibility(bool visibilityChangedEvent = false)
        {
            if (historyBrowserControlHost.Visibility == Visibility.Collapsed)
            {
                //SQLDM-28019: Code change to allow caching of historybrowser width only in case of Visibility changed event
                if (visibilityChangedEvent)
                {
                    historyBrowserColumnRestoreWidth = historyBrowserColumn.Width;
                }
                historyBrowserColumn.Width = GridLength.Auto;
            }
            else
            {
                historyBrowserColumn.Width = historyBrowserColumnRestoreWidth;
            }
        }

        public void UpdateHistoryGroupOptions(bool enableNavigation)
        {
            //if (parentView != null)
            //{
            //    _viewModel.h

            //toolbarsManager.Tools["toggleHistoryBrowserButton"].SharedProps.Enabled = true;
            //((StateButtonTool)toolbarsManager.Tools["toggleHistoryBrowserButton"]).InitializeChecked(
            //    parentView.HistoryBrowserVisible);

            _viewModel.HistoryBrowserEnablePrevious = enableNavigation;
            _viewModel.HistoryBrowserEnableNext = enableNavigation && ViewMode != ServerViewMode.RealTime;

            //}
            //else
            //{
            //    toolbarsManager.Tools["toggleHistoryBrowserButton"].SharedProps.Enabled = false;
            //    toolbarsManager.Tools["showPreviousSnapshotButton"].SharedProps.Enabled = false;
            //    toolbarsManager.Tools["showNextSnapshotButton"].SharedProps.Enabled = false;
            //}
        }

        private void ShowPreviousHistoricalSnapshot()
        {
            if (historyBrowserBackgroundWorker.IsBusy)
            {
                ApplicationMessageBox.ShowInfo(ParentForm, "A historical snapshot is currently being retrieved. Please wait until the operation completes and try again.");
            }
            else
            {
                if (ViewMode == ServerViewMode.RealTime)
                {
                    historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(DateTime.Now, true));
                }
                else
                {
                    SetHistoricalSnapshotDateTime(currentHistoricalSnapshotDateTime.Value, false);  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                    historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(currentHistoricalSnapshotDateTime.Value, true));
                }
            }
        }

        private void ShowNextHistoricalSnapshot()
        {
            if (ViewMode == ServerViewMode.RealTime)
            {
                ApplicationMessageBox.ShowInfo(ParentForm,
                                               "You are currently viewing a real time snapshot. A more recent historical snapshot does not exist.");
            }
            else if (historyBrowserBackgroundWorker.IsBusy)
            {
                ApplicationMessageBox.ShowInfo(ParentForm,
                                               "A historical snapshot is currently being retrieved. Please wait until the operation completes and try again.");
            }
            else
            {
                SetHistoricalSnapshotDateTime(currentHistoricalSnapshotDateTime.Value, false);  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                historyBrowserBackgroundWorker.RunWorkerAsync(new Pair<DateTime, bool>(currentHistoricalSnapshotDateTime.Value, false));
            }
        }

        #endregion

        /// <summary>
        /// Initialize the HistoryBrowserColumn according the OS DPI resolution.
        /// </summary>
        private void InitializeHistoryBrowserColumn()
        {
            if (!AutoScaleFontHelper.Default.IsNormalDPI)
            {
                double currentWidth = historyBrowserColumn.Width.Value;
                double preferredWidth = currentWidth * DecreasedOffset;

                // Set new with for the column.
                historyBrowserColumn.Width = new GridLength(preferredWidth, GridUnitType.Pixel);
            }
        }
        /// <summary>
        /// 10.0 SQLdm srishti purohit
        /// change state of tab button depending upon view shown to user
        /// </summary>
        private void SetButtonStateOfAnalysisView(bool isEnable)
        {
            SetButtonToolEnabled("analyzeTabViewsGroup_analyzeTabBlockButton", isEnable);
            SetButtonToolEnabled("analyzeTabViewsGroup_analyzeTabExportButton", isEnable);
            SetButtonToolEnabled("analyzeTabViewsGroup_analyzeTabCopyButton", isEnable);
            SetButtonToolEnabled("analyzeTabViewsGroup_analyzeTabEMailButton", isEnable);
            var defaultAnalysisIsNull = defaultAnalysisView == null;
			//SQLDM-31035
            _viewModel.EnableAnalyzeBackButton = isEnable && defaultAnalysisIsNull;
            _viewModel.EnableAnalyzeBlockButton = isEnable && defaultAnalysisIsNull;
            _viewModel.EnableAnalyzeCopyButton = isEnable && defaultAnalysisIsNull;
            _viewModel.EnableAnalyzeEmailButton = isEnable && defaultAnalysisIsNull;
            _viewModel.EnableAnalyzeExportButton = isEnable && defaultAnalysisIsNull;
            SetButtonToolEnabled("analyzeTabHistoryGroup_drillOutButton", isEnable);
            _viewModel.DrillOutButtonVisible = isEnable;

            //SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabShowProblem", isEnable);
            //SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabUndoScript", isEnable);
            //SetButtonToolEnabled("analyzeTabScriptGroup_analyzeTabOptimizeScript", isEnable);
        }

        /// <summary>
        ///  SqlDM 10.2(Anshul Aggarwal) : New History Browser
        /// Notifies history browser to reflect drilldown.
        /// </summary>
        private void View_ChartDrilldown(object sender, ChartDrilldownEventArgs e)
        {
            historyBrowserControl.SetHistoricalCustomRange(e);
        }

        private void ParentBreadcrumbLabel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel != null)
                SetRibbonToDefaultToolForTab(_viewModel.ParentBreadcrumb);
        }
        private void SetRibbonToDefaultToolForTab(string tabName)
        {
            if (tabName == "Resources" || tabName == "Overview" || tabName == "Sessions" || tabName == "Databases" || tabName == "Services")
            {
                var tool = (RadioButtonTool)ribbon.GetToolById(string.Format("{0}TabViewsGroup_{0}TabSummaryViewButton",tabName.ToLower()));
                tool.IsChecked = true;
            }
            if(tabName == "Queries")
            {
                var tool = (RadioButtonTool)ribbon.GetToolById("queriesTabViewsGroup_queriesTabSignatureModeViewButton");
                tool.IsChecked = true;
            }
            if(tabName == "Analysis")
            {
                ShowAnalysisDefaultView();
            }
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var buttonName = ((Button)sender).Name;


            }
            
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            var button = sender as System.Windows.Controls.Button;

            if (button == null)
                return;

            var content = (sender as System.Windows.Controls.Button).DataContext as System.Windows.Controls.ContentPresenter;

            var tabItem = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(content))) as System.Windows.Controls.TabItem;

            if (tabItem == null)
                return;

            if (tabItem.Tag == null)
                return;
            if(string.Equals(tabItem.Tag.ToString(), selectedServerTabTag))
            {
                var parent = tabItem.Parent as System.Windows.Controls.TabControl;
                for(var i = 0; i < parent.Items.Count; i++)
                {
                    var item = parent.Items[i] as System.Windows.Controls.TabItem;
                    if (item != null && !string.Equals(item.Tag.ToString(), selectedServerTabTag) && item.IsVisible)
                    {
                        parent.SelectedIndex = i;
                        break;
                    }
                }
            }
            _viewModel.RemoveTab(tabItem.Tag.ToString());

            //removeOverviewTabButton.IsChecked = false;
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            var tab = sender as System.Windows.Controls.Primitives.ToggleButton;

            if (tab == null)
                return;

            _viewModel.AddTab(tab.Name);

            //removeOverviewTabButton.IsChecked = false;
        }

        private void QuickHistoricalSnapshotCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedTimeFrame = QuickHistoricalSnapshotComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;

            if (selectedTimeFrame == null)
                return;

            DateTime endTime = DateTime.Now;
            DateTime startTime = new DateTime();
            var dateText = string.Empty;
            bool isTextChanged = true;
            bool isHistoryBrowserRangeSelected = false;

            var timeIntervalInMinutes = Convert.ToInt32(selectedTimeFrame.Tag);
            if (timeIntervalInMinutes > 0)
            {
                startTime = endTime.AddMinutes(timeIntervalInMinutes * (-1));
                dateText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(startTime, endTime);
                _viewModel.IsCustomHistory = false;
            }
            else if (timeIntervalInMinutes == 0)
            {
                _viewModel.IsLive = true;
                _viewModel.IsCustomHistory = false;
                dateText = DateTimeHelper.GetRealTimeDateQuickHistoricalSnapshots();
                ApplicationController.Default.SetActiveViewToRealTimeMode();
                ApplicationController.Default.PersistUserSettings();
                isTextChanged = false;
            }
            else if (timeIntervalInMinutes == -1)
            {
                if (HistoryRangeBowserSelection())
                {
                    if (ApplicationModel.Default.HistoryTimeValue.ViewMode == ServerViewMode.Custom)
                    {
                        if (ApplicationModel.Default.HistoryTimeValue.EndDateTime.HasValue && ApplicationModel.Default.HistoryTimeValue.StartDateTime.HasValue)
                        {
                            endTime = ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value;
                            startTime = ApplicationModel.Default.HistoryTimeValue.StartDateTime.Value;
                            dateText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(startTime, endTime);
                            _viewModel.IsCustomHistory = true;
                            isHistoryBrowserRangeSelected = true;
                        }
                        else
                        {
                            dateText = QuickHistoricalSnapshotComboBox.Text;
                            isTextChanged = false;
                        }
                    }
                    else
                    {
                        startTime = endTime.AddMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes * (-1));
                        dateText = DateTimeHelper.GetStartEndDateQuickHistoricalSnapshots(startTime, endTime);
                        _viewModel.IsCustomHistory = false;
                        isHistoryBrowserRangeSelected = true;
                    }
                }
                else
                {
                    dateText = QuickHistoricalSnapshotComboBox.Text;
                    isTextChanged = false;
                }
            }

            QuickHistoricalSnapshotComboBox.SelectionChanged -= this.QuickHistoricalSnapshotCombobox_SelectionChanged;
            Action ChangeText = () => _viewModel.QuickHistoricalSnapshotText = dateText;
            Dispatcher.BeginInvoke(ChangeText);
            QuickHistoricalSnapshotComboBox.SelectionChanged += this.QuickHistoricalSnapshotCombobox_SelectionChanged;

            if (isTextChanged)
            {
                _viewModel.EndHistoryDateTime = endTime;
                _viewModel.StartHistoryDateTime = startTime;
                _viewModel.IsLive = false;

                //This action is already performed in historyOptionsDialog ok button click
                if (!isHistoryBrowserRangeSelected)
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(timeIntervalInMinutes);
                
                if (_viewModel.IsCustomHistory)
                    SetCustomStartEndDateTime(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                                              ApplicationModel.Default.HistoryTimeValue.EndDateTime);
                else
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

                //This action is already performed in historyOptionsDialog ok button click
                if (!isHistoryBrowserRangeSelected)
                    ApplicationController.Default.PersistUserSettings(); // Persist user settings on background thread.
            }
        }

        private bool HistoryRangeBowserSelection()
        {
            if (ParentForm is ControlContainerDialog)
            {
                historyOptionsDialog.StartPosition = FormStartPosition.CenterScreen;
                ((ControlContainerDialog)ParentForm).HideOnDeactivate = false;
            }

            historyOptionsDialog.RefreshDialog();
            bool isOk = historyOptionsDialog.ShowDialog() == DialogResult.OK;
            if (ParentForm is ControlContainerDialog)
            {
                ((ControlContainerDialog)ParentForm).HideOnDeactivate = true;
            }

            if (isOk)
            {
                var handler = HistoricalCustomRangeSelected;
                if (handler != null)
                    HistoricalCustomRangeSelected(this, EventArgs.Empty);
            }

            return isOk;
        }

        private void HistorySelectionCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedTimeFrame = historySelectionCombobox.SelectedItem as System.Windows.Controls.ComboBoxItem;

            if (selectedTimeFrame == null)
                return;

            var minutesInSelection = Convert.ToInt32(selectedTimeFrame.Tag);
            switch (selectedTimeFrame.Content as string)
            {
                case "4 Weeks":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddDays(-28);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;                    
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "5 Days":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddDays(-5);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "1 Day":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddDays(-1);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "8 Hours":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddHours(-8);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "4 Hours":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddHours(-4);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "1 Hour":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddHours(-1);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "30 Minutes":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddMinutes(-30);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "15 Minutes":
                    _viewModel.EndHistoryDateTime = DateTime.Now;
                    _viewModel.StartHistoryDateTime = DateTime.Now.AddMinutes(-15);
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = false;
                    ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(minutesInSelection);
                    SetScale(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    break;
                case "Live":
                    _viewModel.IsLive = true;
                    _viewModel.IsCustomHistory = false;
                    ApplicationController.Default.SetActiveViewToRealTimeMode();
                    break;
                case "Custom":
                    _viewModel.IsLive = false;
                    _viewModel.IsCustomHistory = true;
                    ApplicationModel.Default.HistoryTimeValue.SetCustomRange(_viewModel.StartHistoryDateTime, _viewModel.EndHistoryDateTime);
                    SetCustomStartEndDateTime(ApplicationModel.Default.HistoryTimeValue.StartDateTime,ApplicationModel.Default.HistoryTimeValue.EndDateTime, false);
                    break;
                default:
                    break;
            }

            ApplicationController.Default.PersistUserSettings(); // Persist user settings on background thread.

        }
        
        private void OverviewBaselineVisualizer_Click(object sender, RoutedEventArgs e)
        {
            ShowBaselineAssistant();
        }

        private void OverviewMaintenanceMode_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaintenanceMode();
        }

        private void SessionsKillSession_Click(object sender, RoutedEventArgs e)
        {
            if (activeView == sessionsDetailsView)
                sessionsDetailsView.KillSession();
            else if (activeView == sessionsLocksView)
                sessionsLocksView.KillSession();
            else if (activeView == sessionsBlockingView)
                sessionsBlockingView.KillSession();
        }

        private void SessionsTraceSession_Click(object sender, RoutedEventArgs e)
        {
            if (activeView == sessionsDetailsView)
                sessionsDetailsView.TraceSession();
            else if (activeView == sessionsLocksView)
                sessionsLocksView.TraceSession();
            else if (activeView == sessionsBlockingView)
                sessionsBlockingView.TraceSession();
        }

        private void QueriesConfigQueryMonitor_Click(object sender, RoutedEventArgs e)
        {
            ShowQueryMonitorConfigurationProperties();
        }

        private void QueriesConfig_Click(object sender, RoutedEventArgs e)
        {            
            if(queriesWaitStatsActiveView != null)
                queriesWaitStatsActiveView.ShowConfigureDialog();            
        }

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            if (resourcesProcedureCacheView != null)
                resourcesProcedureCacheView.ClearCache();
        }

        private void UpdateStats_Click(object sender, RoutedEventArgs e)
        {
            if (databasesTablesIndexesView != null)
                databasesTablesIndexesView.UpdateTableStatistics();
        }

        private void RebuildIndices_Click(object sender, RoutedEventArgs e)
        {
            if (databasesTablesIndexesView != null)
                databasesTablesIndexesView.RebuildTableIndexes();
        }

        private void FailOver_Click(object sender, RoutedEventArgs e)
        {
            if (databasesMirroringView != null)
                databasesMirroringView.FailOverToPartner();
        }

        private void SuspendResume_Click(object sender, RoutedEventArgs e)
        {
            if (databasesMirroringView != null)
                databasesMirroringView.SuspendResumeSession();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSummaryView != null)
                servicesSummaryView.StartService();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSummaryView != null)
                servicesSummaryView.StopService();
        }

        private void StartService_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSqlAgentJobsView == activeView)
                servicesSqlAgentJobsView.StartService();
            else if (servicesFullTextSearchView == activeView)
                servicesFullTextSearchView.StartService();
        }

        private void StopService_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSqlAgentJobsView == activeView)
                servicesSqlAgentJobsView.StopService();
            else if (servicesFullTextSearchView == activeView)
                servicesFullTextSearchView.StopService();
        }

        private void StartJob_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSqlAgentJobsView != null)
                servicesSqlAgentJobsView.PerformJobAction(JobControlAction.Start);
        }

        private void StopJob_Click(object sender, RoutedEventArgs e)
        {
            if (servicesSqlAgentJobsView != null)
                servicesSqlAgentJobsView.PerformJobAction(JobControlAction.Stop);
        }
        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            if (servicesFullTextSearchView != null)
                servicesFullTextSearchView.OptimizeCatalog();
        }
        private void Repopulate_Click(object sender, RoutedEventArgs e)
        {
            if (servicesFullTextSearchView != null)
                servicesFullTextSearchView.RepopulateCatalog();
        }
        private void Rebuild_Click(object sender, RoutedEventArgs e)
        {
            if (servicesFullTextSearchView != null)
                servicesFullTextSearchView.RebuildCatalog();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var viewWithFilter = activeView as IShowFilterDialog;
            if (viewWithFilter != null)
                viewWithFilter.ShowFilter();
        }

        private void LogsConfigure_Click(object sender, RoutedEventArgs e)
        {
            if(logsView != null)
                logsView.ConfigureLogs();
        }

        private void CycleLogs_Click(object sender, RoutedEventArgs e)
        {
            if (logsView != null)
                logsView.CycleServerLog();
        }

        private void AnalyzeBlock_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.BlockRecommendations(instanceId);
            }
        }
        private void AnalyzeExport_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.ExportSelectedRecommendations();
            }
        }

        private void AnalyzeCopy_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.CopyRecommendations();
            }
        }

        private void AnalyzeEmail_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.EmailRecommendations();
            }
        }

        private void AnalyzeRun_Click(object sender, RoutedEventArgs e)
        {
            ShowRunAnalysisWizard(activeInstance);
        }
        private void AnalyzeOptimizeScript_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.OptimizeScriptRecommendations();
            }
        }
        private void AnalyzeShowProblem_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.ShowProblemRecommendations();
            }
        }
		
		//SQLDM-31035
        private void AnalyzeBack_Click(object sender, RoutedEventArgs e)
        {

            ShowAnalysisDefaultView();

        }
		
        private void AnalyzeUndoScript_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.UndoScriptRecommendations();
            }
        }

        private void AnalyzeUndo_Click(object sender, RoutedEventArgs e)
        {
            if (defaultAnalysisView == null)
            {
                if (recommendationAnalysis == null)
                {
                    recommendationAnalysis = new Analysis.Recommendations(instanceId, false);
                }
                recommendationAnalysis.UndoScriptRecommendations();
            }
        }

        private void LayoutButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.Source as System.Windows.Controls.Button;
            
            if (button == null)
                return;

            string tag = button.Tag as string;

            if (tag == null)
                return;

            switch (tag)
            {
                case "DashboardLayoutTwoByTwo":
                    SelectDashboardLayout(new Size(2, 2));
                    break;
                case "DashboardLayoutTwoByThree":
                    SelectDashboardLayout(new Size(2, 3));
                    break;
                case "DashboardLayoutTwoByFour":
                    SelectDashboardLayout(new Size(2, 4));
                    break;
                case "DashboardLayoutTwoByFive":
                    SelectDashboardLayout(new Size(2, 5));
                    break;
                case "DashboardLayoutThreeByTwo":
                    SelectDashboardLayout(new Size(3, 2));
                    break;
                case "DashboardLayoutThreeByThree":
                    SelectDashboardLayout(new Size(3, 3));
                    break;
                case "DashboardLayoutThreeByFour":
                    SelectDashboardLayout(new Size(3, 4));
                    break;
                case "DashboardLayoutFourByTwo":
                    SelectDashboardLayout(new Size(4, 2));
                    break;
                case "DashboardLayoutFourByThree":
                    SelectDashboardLayout(new Size(4, 3));
                    break;
            }
        }

        private void OvDetailsShowAll_Click(object sender, RoutedEventArgs e)
        {
            if (serverDetailsView != null)
            {
                _viewModel.ShowOvDetailsShowAll = true;
                _viewModel.ShowOvDetailsCustomCounter = false;
                serverDetailsView.Filter = ServerDetailsView.Selection.All;
            }
        }
        private void OvDetailsCustomCounter_Click(object sender, RoutedEventArgs e)
        {
            if (serverDetailsView != null)
            {
                _viewModel.ShowOvDetailsShowAll = false;
                _viewModel.ShowOvDetailsCustomCounter = true;
                serverDetailsView.Filter = ServerDetailsView.Selection.Custom;
            }
        }
        private void DbUserDatabases_Click(object sender, RoutedEventArgs e)
        {
            if (databasesTablesIndexesView != null)
                databasesTablesIndexesView.IncludeSystemDatabases = !databasesTablesIndexesView.IncludeSystemDatabases;
        }
        private void DbUserTables_Click(object sender, RoutedEventArgs e)
        {
            if (databasesTablesIndexesView != null)
                databasesTablesIndexesView.IncludeSystemTables = !databasesTablesIndexesView.IncludeSystemTables;
        }

        private void DashGallerySelect_Click(object sender, RoutedEventArgs e)
        {
            SelectDashboardGalleryView();
        }

        private void DashGalleryClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDashboardGalleryView();
        }

        private void customizeDashBoardCancel_Click(object sender, RoutedEventArgs e)
        {
            this.toggleCustomDashboard.IsChecked = false;
            if (serverSummaryView.PanelGalleryVisible)
                serverSummaryView.PanelGalleryVisible = false;
            EnableDisableCustomizeDashboard(false);
        }

        private void customizeDashBoardSave_Click(object sender, RoutedEventArgs e)
        {
            IServerDesignView designView = activeView as IServerDesignView;
            designView.SaveDashboardDesign();
            customizeDashBoardCancel_Click(sender, e);
        }
    }
}