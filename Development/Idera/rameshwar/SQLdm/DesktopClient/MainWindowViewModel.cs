using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;

using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MainWindow _window;
        private const string visible = "Visible";
        private const string collapsed = "Collapsed";
        private const double widthToShowHamburger = 1600;
        private List<int> serverListToSnooze = new List<int>();
        private List<int> serverListToResume = new List<int>();

        public MainWindowViewModel(MainWindow window)
        {
            populateSearchItems();
            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;
            ApplicationModel.Default.Tags.Changed += Tags_Changed;
            _window = window;
            ApplicationModel.Default.FocusObjectChanged += ApplicationModel_FocusObjectChanged;
            ApplicationController.Default.ActiveViewChanged += ApplicationController_ActiveViewChanged;
            ApplicationModel.Default.CustomReports.Changed += CustomReports_Changed;
            ApplicationController.Default.ReportsViewChanged += Default_ReportsViewChanged;
            window.SizeChanged += Window_SizeChanged;
            ApplicationController.Default.BackgroundRefreshCompleted += Default_BackgroundRefreshCompleted;

        }

        private void Default_BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            PrepareSnoozeAndUnsnoozeOperation();
        }

        private void Window_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            FirePropertyChanged("ShowHamburger");
            FirePropertyChanged("HideHamburger");
            FirePropertyChanged("MenuWidth");
        }

        ~MainWindowViewModel()
        {
            ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
            ApplicationModel.Default.Tags.Changed -= Tags_Changed;
            ApplicationModel.Default.FocusObjectChanged -= ApplicationModel_FocusObjectChanged;
            ApplicationController.Default.ActiveViewChanged -= ApplicationController_ActiveViewChanged;
            ApplicationController.Default.BackgroundRefreshCompleted -= Default_BackgroundRefreshCompleted;
        }

        private void Tags_Changed(object sender, TagCollectionChangedEventArgs e)
        {
            populateSearchItems();
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            populateSearchItems();

        }

        void populateSearchItems()
        {
            var servers = ApplicationModel.Default.ActiveInstances.Select(x => new SearchItem { Name = x.DisplayInstanceName, Id = x.Id, Type = SearchItemType.Server });
            var tags = ApplicationModel.Default.Tags.Select(x => new SearchItem { Name = x.Name, Id = x.Id, Type=SearchItemType.Tag });
            var userViews = Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.UserViews.Select(x => new SearchItem { Name = x.Name, Id = x.Id, Type=SearchItemType.UserView });
            SearchItems = servers.Concat(tags).Concat(userViews).ToList();
            FirePropertyChanged("SearchItemsFiltered");
        }

        public Settings Settings
        {
            get { return Properties.Settings.Default; }
        }


        #region CustomReports
        private const string DELETECUSTOMREPORT =
            "Are you sure you would like to delete \"{0}\"?.  This action cannot be undone.";


        public void DeleteCustomReport()
        {
            if (String.IsNullOrWhiteSpace(selectedCustomReport)) return;

            if (ApplicationMessageBox.ShowQuestion(_window, string.Format(DELETECUSTOMREPORT, selectedCustomReport)) != DialogResult.Yes) return;

            CustomReports.Remove(selectedCustomReport);

            ApplicationModel.Default.RemoveCustomReport(selectedCustomReport);

            var previous = CustomReports.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(previous))
            {
                selectedCustomReport = string.Empty;

                ApplicationController.Default.ShowReportsView(ReportTypes.GettingStarted);
            }
            else
            {
                selectedCustomReport = previous;

                ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, previous, null);
            }

            FirePropertyChanged("HasNoCustomReports");
            FirePropertyChanged("CustomReports");
            FirePropertyChanged("ReportIsSelected");

        }

        public void NewCustomReport()
        {
            CustomReportWizard wizard = new CustomReportWizard();

            wizard.ShowDialog(this._window);
        }

        private ObservableCollection<string> customReports = new ObservableCollection<string>();
        public ObservableCollection<string> CustomReports
        {
            get { return customReports; }
            set { customReports = value; }
        }

        private string selectedCustomReport = null;

        private void SelectReport(ReportTypes report, string reportKey)
        {
            if (report != Idera.SQLdm.DesktopClient.Views.Reports.ReportTypes.Custom)
            {
                selectedCustomReport = null;
            }

            switch (report)
            {
                case Idera.SQLdm.DesktopClient.Views.Reports.ReportTypes.Custom:
                    selectedCustomReport = reportKey;

                    break;
                default:
                    break;
            }

            FirePropertyChanged("HasNoCustomReports");
            FirePropertyChanged("CustomReports");
            FirePropertyChanged("ReportIsSelected");
        }

        private void Default_ReportsViewChanged(object sender, ReportsViewChangedEventArgs e)
        {
            SelectReport(e.NewView, e.ReportKey);
        }

        public void EditSelectedCustomReport()
        {
            if (String.IsNullOrWhiteSpace(selectedCustomReport))
                throw new NullReferenceException("Must provide a custom report name.");

            TreeNode tn = new TreeNode(selectedCustomReport);
            tn.Name = selectedCustomReport;

            var p = new Pair<ReportsNavigationPane.ReportCategory, ReportTypes>();
            p.First = ReportsNavigationPane.ReportCategory.Custom;

            tn.Tag = p;

            CustomReportWizard wizard = new CustomReportWizard(tn);

            if (wizard.IsDisposed) return;

            wizard.ShowDialog(_window);

            FirePropertyChanged("HasNoCustomReports");
            FirePropertyChanged("CustomReports");
            FirePropertyChanged("ReportIsSelected");
        }
        public void SelectCustomReport(string customReportName)
        {
            ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, customReportName, null);
            selectedCustomReport = customReportName;
        }

        private void CustomReports_Changed(object server, CustomReportCollectionChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach (Common.Objects.CustomReport report in e.Reports.Values)
                    {
                        if (!CustomReports.Contains(report.Name))
                        {
                            CustomReports.Add(report.Name);
                        }
                    }
                    break;
                //case KeyedCollectionChangeType.Removed:
                //    foreach (Common.Objects.CustomReport report in e.Reports.Values)
                //    {
                //        while (CustomReports.Contains(report.Name))
                //        {
                //            CustomReports.Remove(report.Name);
                //        }
                //    }
                //    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach (Common.Objects.CustomReport report in e.Reports.Values)
                    {
                        if (!CustomReports.Contains(report.Name))
                        {
                            CustomReports.Add(report.Name);
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    CustomReports.Clear();
                    break;
            }

            //I don't think we need this since we are binding to wpf for list of custom reports
            //RepopulateCustomReports();
            FirePropertyChanged("HasNoCustomReports");
            FirePropertyChanged("CustomReports");
            FirePropertyChanged("ReportIsSelected");

        }

        public bool ReportIsSelected
        {
            get
            {
                return !String.IsNullOrWhiteSpace(selectedCustomReport);
            }
        }
        public bool HasNoCustomReports
        {
            get
            {
                if (CustomReports == null || CustomReports.Count <= 0)
                    return true;
                else
                    return false;
            }
        }

        public void ImportCustomReport()
        {
            Idera.SQLdm.Common.Objects.CustomReport report;
            DialogResult dialogResult = CustomReportImportWizard.ImportNewReport(_window, out report);

            FirePropertyChanged("HasNoCustomReports");
            FirePropertyChanged("CustomReports");
            FirePropertyChanged("ReportIsSelected");
            
        }

        public void ExportCustomReport()
        {
            var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            var managementService = Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(connectionInfo);
            var selectedReport = selectedCustomReport;
            if (selectedReport != null)
            {
                try
                {
                    var _CurrentCustomReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                selectedReport);

                    //includes aggregation that may have been set up previously
                    var _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                    //populate the counters that have already been selected
                    if (_CurrentCustomReport.Metrics == null) _CurrentCustomReport.Metrics = new SortedDictionary<int, Idera.SQLdm.Common.Objects.CustomReportMetric>();

                    _CurrentCustomReport.Metrics.Clear();

                    foreach (System.Data.DataRow row in _selectedCountersDataTable.Rows)
                    {
                        string metricName = row["CounterName"].ToString();
                        string metricDescription = row["CounterShortDescription"].ToString();

                        //_selectedCounters.Add(metricName, metricDescription);

                        Idera.SQLdm.Common.Objects.CustomReport.CounterType type = (Idera.SQLdm.Common.Objects.CustomReport.CounterType)int.Parse(row["CounterType"].ToString());
                        Idera.SQLdm.Common.Objects.CustomReport.Aggregation aggregation = (Idera.SQLdm.Common.Objects.CustomReport.Aggregation)int.Parse(row["Aggregation"].ToString());

                        _CurrentCustomReport.Metrics.Add(int.Parse(row["GraphNumber"].ToString()),
                                    new Idera.SQLdm.Common.Objects.CustomReportMetric(metricName, metricDescription, type, aggregation));

                    }

                    string xml = Idera.SQLdm.DesktopClient.Helpers.CustomReportHelper.SerializeCustomReport(_CurrentCustomReport);

                    using (var sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "xml files (*.xml)|*.xml";
                        sfd.FileName = _CurrentCustomReport.Name + ".xml";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.File.WriteAllText(sfd.FileName, xml);
                            ApplicationMessageBox.ShowInfo(_window, "Selected custom report exported successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(_window, "Export operation failed for selected report: ", ex);
                }
            }
        }

        public void ScheduleEmailCustomReport()
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol reportControl = ((ReportsView)ApplicationController.Default.ActiveView).ActiveReport;

                string message;

                if (reportControl.CanRunReport(out message))
                {
                    ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    DeployReportsWizard wizard =
                        new DeployReportsWizard(reportControl.ReportType, reportControl.GetReportParmeters(), selectedCustomReport, reports);
                    wizard.ShowDialog(_window);
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(_window, message);
                }
            }
        }

        public void DeployCustomReport()
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                DeployReportsWizard wizard =
                    new DeployReportsWizard(((ReportsView)
                ApplicationController.Default.ActiveView).ActiveReport.ReportType, null, selectedCustomReport, reports);

                wizard.ShowDialog(_window);
            }
        }

        #endregion

        public ObservableCollection<string> CustomViews
        {
            get
            {
                var listOfViews = Settings.Default.SavedViews.Cast<string>().ToList();
                return new ObservableCollection<string>(listOfViews);
            }
        }

        public bool HasNoCustomViews
        {
            get
            {
                if (CustomViews == null)
                    return true;

                return CustomViews.Count <= 0;
            }
        }

        public string SelectedViewName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(SelectedViewFileName);
            }
        }

        public void AddLayoutToList(string fileName)
        {

            if (Settings.Default.SavedViews != null)
            {
                var nameOnly = System.IO.Path.GetFileNameWithoutExtension(fileName);
                Settings.Default.SavedViews.Add(nameOnly);
                Settings.Default.Save();
            }
                
            FirePropertyChanged("CustomViews");
        }

        public void RemoveLayout(string fileName)
        {
            if (Settings.Default.SavedViews != null)
            {
                var nameOnly = System.IO.Path.GetFileNameWithoutExtension(fileName);
                Settings.Default.SavedViews.Remove(nameOnly);
                Settings.Default.Save();
            }

            FirePropertyChanged("CustomViews");
        }

        public System.Windows.GridLength MenuWidth
        {
            get
            {
                System.Windows.GridLength gl;
                if (_window.ActualWidth <= widthToShowHamburger)
                    gl = System.Windows.GridLength.Auto;
                else
                    gl = new System.Windows.GridLength(390);


                System.Diagnostics.Debug.WriteLine("MenuWidth: " + gl.ToString());
                return gl;
            }
        }

        private bool _showHamburger = false;
        public bool ShowHamburger
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("Window ActualWidth: " + _window.ActualWidth.ToString());

                if (_window.ActualWidth <= widthToShowHamburger)
                    _showHamburger = true;
                else
                    _showHamburger = false;

                return _showHamburger;
            }
        }

        public bool HideHamburger
        {
            get { return !_showHamburger; }
        }

        public void SnoozeAlerts()
        {
            if (this.serverListToSnooze != null && this.serverListToSnooze.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Snooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(_window, this.serverListToSnooze, action, "All Servers");
            }

            FirePropertyChanged("SnoozeAllAlertsEnabled");
            FirePropertyChanged("ResumeAllAlertsEnabled");
        }

        public void ResumeAlerts()
        {
            if (this.serverListToResume != null && this.serverListToResume.Count > 0)
            {
                SnoozeAlertsDialog.SnoozeAction action = SnoozeAlertsDialog.SnoozeAction.Unsnooze;
                SnoozeAlertsDialog.SnoozeAllServerAlerts(_window, this.serverListToResume, action, "All Servers");
            }

            FirePropertyChanged("SnoozeAllAlertsEnabled");
            FirePropertyChanged("ResumeAllAlertsEnabled");
        }
        /// <summary>
        /// Get in its respective list all on which can perform snooze or resume all alerts,
        /// according that enables or disables this operation from UI.
        /// </summary>
        public void PrepareSnoozeAndUnsnoozeOperation()
        {
            this.serverListToSnooze = new List<int>();
            this.serverListToResume = new List<int>();
            ICollection<int> instanceIds = ApplicationModel.Default.ActiveInstances.Keys;

            // Fill snooze and un-snooze lists.
            if (instanceIds != null && instanceIds.Count > 0)
            {
                //check User permision over a servers
                bool isEnableSnoozeMenu = ApplicationModel.Default.UserToken.IsHasModifyServerPermission(instanceIds.ToList());
                if (isEnableSnoozeMenu)
                {
                    foreach (int instanceId in instanceIds)
                    {
                        MonitoredSqlServerStatus serverStatus = MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);

                        if (serverStatus != null)
                        {
                            if (!serverStatus.AreAllAlertsSnoozed)
                            {
                                this.serverListToSnooze.Add(instanceId);
                            }
                            if (serverStatus.SnoozingAlertCount > 0)
                            {
                                this.serverListToResume.Add(instanceId);
                            }
                        }
                    }
                }
                else
                {
                    this.SnoozeAllAlertsEnabled = false;
                    this.ResumeAllAlertsEnabled = false;
                }
            }
            else
            {
                this.SnoozeAllAlertsEnabled = false;
                this.ResumeAllAlertsEnabled = false;
            }

            FirePropertyChanged("SnoozeAllAlertsEnabled");
            FirePropertyChanged("ResumeAllAlertsEnabled");
        }

        private bool snoozeAllAlertsEnabled = false;
        public bool SnoozeAllAlertsEnabled
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("snoozeCount: " + this.serverListToSnooze.Count.ToString());
                snoozeAllAlertsEnabled = this.serverListToSnooze.Count > 0;
                return snoozeAllAlertsEnabled; 
            }
            set
            {
                snoozeAllAlertsEnabled = value;
            }
        }

        private bool resumeAllAlertsEnabled = false;
        public bool ResumeAllAlertsEnabled
        {
            get
            {
                System.Diagnostics.Debug.WriteLine("resumeCount: " + this.serverListToResume.Count.ToString());
                resumeAllAlertsEnabled = this.serverListToResume.Count > 0;
                return resumeAllAlertsEnabled;
            }
            set
            {
                resumeAllAlertsEnabled = value;
            }
        }

        private string _selectedViewFileName = visible;
        public string SelectedViewFileName
        {
            get { return _selectedViewFileName; }
            set
            {
                if (_selectedViewFileName == value) return;
                _selectedViewFileName = value;
                FirePropertyChanged("SelectedViewFileName");
                FirePropertyChanged("SelectedViewName");
                FirePropertyChanged("CustomViews");
            }
        }

        private string _servicestabitemvisibilityforazuredb = visible;
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
        private string _logstabitemvisibility1 = visible;
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
        private string _VisibilityQueries = visible;
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

        public bool IsNavigationPaneVisible
        {
            get { return Settings.Default.NavigationPaneVisible; }
            set
            {
                if(Settings.Default.NavigationPaneVisible != value)
                    Settings.Default.NavigationPaneVisible = value;
            }
        }

        public bool IsRibbonMinimized
        {
            get { return Settings.Default.CollapseRibbon; }
            set
            {
                if (Settings.Default.CollapseRibbon != value)
                    Settings.Default.CollapseRibbon = value;
            }
        }

        private void ApplicationModel_FocusObjectChanged(object sender, EventArgs e)
        {
            UpdateSelectedObject();
        }

        



        private void ApplicationController_ActiveViewChanged(object sender, EventArgs e)
        {
            IsRefreshOptionsEnabled = !(ApplicationController.Default.ActiveView is Views.Reports.ReportsView);
        }
        
        public bool LeftNavExpanded
        {
            get { return Settings.Default.NavigationPaneVisible; }
            set
            {
                if (Settings.Default.NavigationPaneVisible != value)
                {
                    Settings.Default.NavigationPaneVisible = value;
                    FirePropertyChanged("LeftNavExpanded");
                    FirePropertyChanged("LeftNavCollapsed");
                }
            }
        }
        public bool LeftNavCollapsed
        {
            get { return !Settings.Default.NavigationPaneVisible; }
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

        private bool _isDeleteSelectedObjectEnabled = false;
        public bool IsDeleteSelectedObjectEnabled
        {
            get { return _isDeleteSelectedObjectEnabled; }
            set
            {
                if (_isDeleteSelectedObjectEnabled == value) return;
                _isDeleteSelectedObjectEnabled = value;
                FirePropertyChanged("IsDeleteSelectedObjectEnabled");
            }
        }

        private bool _isPropertiesForSelectedObjectEnabled = false;
        public bool IsPropertiesForSelectedObjectEnabled
        {
            get { return _isPropertiesForSelectedObjectEnabled; }
            set
            {
                if (_isPropertiesForSelectedObjectEnabled == value) return;
                _isPropertiesForSelectedObjectEnabled = value;
                FirePropertyChanged("IsPropertiesForSelectedObjectEnabled");
            }
        }

        public void UpdateSelectedObject()
        {
            if (ApplicationModel.Default.FocusObject != null &&
                !(ApplicationModel.Default.FocusObject is AllServersUserView) &&
                !(ApplicationModel.Default.FocusObject is SearchUserView))
            {
                IsDeleteSelectedObjectEnabled = ApplicationModel.Default.FocusObject is StaticUserView || ApplicationModel.Default.UserToken.IsSQLdmAdministrator;          
                IsPropertiesForSelectedObjectEnabled = ApplicationModel.Default.UserToken.IsSQLdmAdministrator || !(ApplicationModel.Default.FocusObject is Idera.SQLdm.Common.Objects.Tag);
            }
            else
            {
                IsDeleteSelectedObjectEnabled = false;
                IsPropertiesForSelectedObjectEnabled = false;
            }

            ShowHideMenuItems();
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

        internal void UpdatePredictiveAnalyticsState()
        {
            try
            {
                IsPredictiveAnalyticsEnabled = RepositoryHelper.GetPredictiveAnalyticsEnabled(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            }
            catch (Exception ex)
            {
            }
        }

        private bool _isPredictiveAnalyticsEnabled = false;
        public bool IsPredictiveAnalyticsEnabled
        {
            get { return _isPredictiveAnalyticsEnabled; }
            set
            {
                if (value == _isPredictiveAnalyticsEnabled) return;
                _isPredictiveAnalyticsEnabled = value;
                FirePropertyChanged("IsPredictiveAnalyticsEnabled");
                FirePropertyChanged("PredictiveAnalyticsToggleText");
            }
        }

        private const string enabledText = "Disable Predictive Analytics";
        private const string disabledText = "Enable Predictive Analytics";

        public string PredictiveAnalyticsToggleText
        {
            get { return IsPredictiveAnalyticsEnabled ? enabledText : disabledText; }
        }

        private string _searchText = null;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                FirePropertyChanged("SearchText");
                FirePropertyChanged("SearchTextNotPresent");
                FirePropertyChanged("SearchResultsPresent");
                FirePropertyChanged("SearchItemsFiltered");
            }
        }
        public bool SearchTextNotPresent
        {
            get { return string.IsNullOrEmpty(_searchText); }
        }
        public bool SearchResultsPresent
        {
            get { return SearchItemsFiltered.Any(); }
        }
        public List<SearchItem> SearchItems { get; set; }
        public IEnumerable<SearchItem> SearchItemsFiltered
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_searchText)) return new List<SearchItem>();
                return SearchItems.Where(x => x.Name.ToUpper().Contains(SearchText.ToUpper()));
            }
        }


        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShowHideMenuItems()
        {
            if (ApplicationModel.Default.SelectedInstanceId == 0 &&
                !ApplicationModel.Default.AllInstances.ContainsKey(ApplicationModel.Default.SelectedInstanceId))
            {
                return;
            }

            // Handle Exceptions because of Desktop Client UI on deleting server
            int? cloudProviderId = null;
            try
            {
                cloudProviderId = ApplicationModel.Default.AllInstances[ApplicationModel.Default.SelectedInstanceId]
                    .CloudProviderId;
            }
            catch
            {
                return;
            }

            switch (cloudProviderId)

            {

                //case 1: // For RDS instance
                //    VisibilityQueries = visible;
                //    ServicesTabItemVisibilityforAzureDb = collapsed;
                //    LogTabItemVisibility1 = collapsed;

                case 1: // For RDS instance
                    VisibilityQueries = visible;
                    ServicesTabItemVisibilityforAzureDb = visible;
                    LogTabItemVisibility1 = visible;

                    break;
                case 2: // For AzureDB instance

                    ServicesTabItemVisibilityforAzureDb = collapsed;
                    VisibilityQueries = visible; //SQLdm 11.0 Queries Tab
                    LogTabItemVisibility1 = collapsed;
                    break;

               default:  //For Windows instance
                        VisibilityQueries = visible;
                        ServicesTabItemVisibilityforAzureDb = visible;
                        LogTabItemVisibility1 = visible;
                        break;
                }
        }
        public void UpdateNavigataionPane(XamNavigationPanes pane)
        {
            this._window.UpdateNavigationPane(pane);
        }
        public void UpdateSearchBarVisibility(XamNavigationPanes pane)
        {
            this._window.UpdateSearchBarVisibility(pane);
        }
    }


}
