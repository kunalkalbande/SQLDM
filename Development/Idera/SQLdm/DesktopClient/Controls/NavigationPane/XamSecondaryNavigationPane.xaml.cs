using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using System.Configuration;
using System.Windows.Forms;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;

using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Tasks;
using Idera.SQLdm.DesktopClient.Objects;


namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    /// <summary>
    /// Interaction logic for XamSecondaryNavigationPane.xaml
    /// </summary>
    public partial class XamSecondaryNavigationPane : System.Windows.Controls.UserControl
    {
        public enum paneStates { Open, Closed }

        public paneStates paneState = paneStates.Open;

        private XamNavigationPanes selectedPane = XamNavigationPanes.Servers;

       

        public XamSecondaryNavigationPane()
        {
            InitializeComponent();

            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;

            Change(selectedPane, 0);

            
        }
        bool firstInstanceChange = true;
        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            if (firstInstanceChange || (ApplicationController.Default.ActiveView is Views.Servers.Server.ServerViewContainer && (ApplicationController.Default.ActiveView as Views.Servers.Server.ServerViewContainer).IgnoreActiveInstancesChanging))
                firstInstanceChange = false;
            else if(!(ApplicationController.Default.ActiveView is Views.NewSQLdmTodayView))
               Change(XamNavigationPanes.Servers, 1);
        }

        public void SetAllToCollapsed()
        {
            
            reportsSecondaryMenu.Visibility = Visibility.Collapsed;
            serversSecondaryMenu.Visibility = Visibility.Collapsed;
            //SQLdmSecondaryMenu.Visibility = Visibility.Collapsed;
            toolsSecondaryMenu.Visibility = Visibility.Collapsed;
            administrationSecondaryMenu.Visibility = Visibility.Collapsed;
            alertsSecondaryMenu.Visibility = Visibility.Collapsed;
        }
        Views.IView previousView = null;
        public void Change(XamNavigationPanes pane, int? selectedIndex = null)
        {
            SetAllToCollapsed();

            switch (pane)
            {
                case XamNavigationPanes.Reports:
                    reportsSecondaryMenu.Visibility = Visibility.Visible;
                    //toggleNavButtonUp.Visibility = Visibility.Visible;

                    break;
                case XamNavigationPanes.Servers:
                    serversSecondaryMenu.Visibility = Visibility.Visible;
                    //toggleNavButtonUp.Visibility = Visibility.Visible;

                    break;
               // case XamNavigationPanes.SQLDM:
                 //   SQLdmSecondaryMenu.Visibility = Visibility.Collapsed;
                   // toggleNavButtonUp.Visibility = Visibility.Collapsed;
                   
                case XamNavigationPanes.Alerts:

                    alertsSecondaryMenu.Visibility = Visibility.Visible;
                    //toggleNavButtonUp.Visibility = Visibility.Visible;
                    break;

                case XamNavigationPanes.Administration:
                    //toggleNavButtonUp.Visibility = Visibility.Collapsed;
                    administrationSecondaryMenu.Visibility = Visibility.Visible;

                    ApplicationController.Default.ShowAdministrationView(Views.Administration.AdministrationView.AdministrationNode.Administration);
                    
                    break;
            }
            paneState = paneStates.Open;
            //setToggleNavButtonBackground();
            selectedPane = pane;
            if(selectedIndex.HasValue)
                serversSecondaryMenu.SelectedIndex = selectedIndex.Value;
            previousView = ApplicationController.Default.ActiveView;
        }

        internal void SetServersSecondaryMenuIndex(Views.Servers.Server.ServerViewTabs tab)
        {
            var index = (int)tab + 1;
            if (serversSecondaryMenu.SelectedIndex != index && index >= 1 && index <= 8)
            {
                IgnoreServersSelectionChanged = true;
                serversSecondaryMenu.SelectedIndex = index;
            }
        }

        public void TogglePane()
        {
            if (paneState == paneStates.Open)
            {
                SetAllToCollapsed();
                paneState = paneStates.Closed;
            }
            else
            {
                paneState = paneStates.Open;
                Change(selectedPane, serversSecondaryMenu.SelectedIndex);
            }
        }

        private void ShowManageServersDialog()
        {
            ManageServersDialog manageServersDialog = new ManageServersDialog();
            var win = Window.GetWindow(this);
            manageServersDialog.ShowDialog((WinformWindow)win);
        }

        private void ShowManageTagsDialog()
        {
            ManageTagsDialog manageTagsDialog = new ManageTagsDialog();
            var win = Window.GetWindow(this);
            manageTagsDialog.ShowDialog((WinformWindow)win);
        }

        private void ShowAddUserViewDialog()
        {
            StaticViewPropertiesDialog dialog = new StaticViewPropertiesDialog();
            var win = Window.GetWindow(this);
            dialog.ShowDialog((WinformWindow)win);
        }

       /* private void setToggleNavButtonBackground()
        {
            if (paneState == paneStates.Closed)
                toggleNavButtonUp.Background = new ImageBrush(FindResource("up_arrow_wht_iconDrawingImage") as ImageSource);
            else
                toggleNavButtonUp.Background = new ImageBrush(FindResource("down_arrow_wht_iconDrawingImage") as ImageSource);
        }
        */
        /*private void ToggleNavButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePane();
            setToggleNavButtonBackground();
        }
        */
        public bool IgnoreServersSelectionChanged = false;
        private void ServersSecondaryMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IgnoreServersSelectionChanged)
            {
                IgnoreServersSelectionChanged = false;
                return;
            }
            if (e.AddedItems == null || e.AddedItems[0] == null)
                return;

            TabItem item = e.AddedItems[0] as TabItem;

            if (item == null)
                return;

            if (e.RemovedItems.Count == 0)
                return;

            int selectedInstanceId = ApplicationModel.Default.SelectedInstanceId == 0 ? 1 : ApplicationModel.Default.SelectedInstanceId;
            switch (item.Name)
            {
                case "serverMenuOverview":
                    ApplicationController.Default.ShowServerView(selectedInstanceId);
                    break;
                case "serverMenuSessions":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Sessions);
                    break;
                case "serverMenuQuery":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Queries);
                    break;
                case "serverMenuResources":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Resources);
                    break;
                case "serverMenuDatabase":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Databases);
                    break;
                case "serverMenuServices":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Services);
                    break;
                case "serverMenuLogs":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Logs);
                    break;
                case "serverMenuAnalyze":
                    ApplicationController.Default.ShowServerView(selectedInstanceId, Views.Servers.Server.ServerViews.Analysis);
                    break;

            }

            e.Handled = true;
        }

        
        private void MenuItemMonitorServer_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {
                
                case "menuItemEnterpriseSummary":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.EnterpriseSummary);
                    break;
                case "menuItemServerSummary":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ServerSummary);
                    break;
                case "menuItemActiveAlerts":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ActiveAlerts);
                    break;
                case "menuItemMirroringSummary":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.MirroringSummary);
                    break;
                case "menuItemMetricThresholds":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.MetricThresholds);
                    break;
                case "menuItemAvailabilityGroupTopology":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.AlwaysOnTopology);
                    break;
                case "menuItemDeadlockReport":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DeadlockReport);
                    break;
                case "menuItemAlertTemplate":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.AlertTemplateReport);
                    break;
                case "menuItemAlertThreshold":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.AlertThreshold);
                    break;
                case "menuItemTemplateComparison":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TemplateComparison);
                    break;
            }

            e.Handled = true;
        }

        private void MenuItemMonitorVirtualization_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemVirtualizationSummary":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.VirtualizationSummary);
                    break;
                case "menuItemVirtualizationStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.VirtualizationStatistics);
                    break;
            }

            e.Handled = true;
        }

        private void MenuItemMonitorActivity_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemChangeLogSummary":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ChangeLogSummary);
                    break;
            }

            e.Handled = true;
        }

        private void MenuItemAnalyzeServers_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemTopServers":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopServers);
                    break;
                case "menuItemServerStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ServerStatistics);
                    break;
                case "menuItemServerInventory":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ServerInventory);
                    break;
                case "menuItemQueryOverview":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.QueryOverview);
                    break;
                case "menuItemTopQueries":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopQueries);
                    break;
                case "menuItemAlertHistory":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.AlertHistory);
                    break;
                case "menuItemBaselineStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.BaselineStatistics);
                    break;
                case "menuItemQueryWaitStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.QueryWaitStatistics);
                    break;
            }

            e.Handled = true;
        }

        private void MenuItemAnalyzeDatabases_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemTopDatabases":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopDatabases);
                    break;
                case "menuItemDatabaseStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DatabaseStatistics);
                    break;
                case "menuItemTopDatabaseApplications":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopDatabaseApps);
                    break;
                case "menuItemMirroringHistory":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.MirroringHistory);
                    break;
                case "menuItemTransactionLogStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TransactionLogStatistics);
                    break;
                case "menuItemTopTablesByGrowth":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopTablesGrowth);
                    break;
                case "menuItemTopTablesByFragmentation":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TopTableFrag);
                    break;
                case "menuItemTempdbStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TempdbStatistics);
                    break;
                case "menuItemAvailabilityGroupStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.AlwaysOnStatistics);
                    break;
            }


            e.Handled = true;
        }

        private void MenuItemAnalyzeResources_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemSessionStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.SessionsSummary);
                    break;
                case "menuItemDetailedSessionReport":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DetailedSessionReport);
                    break;
                case "menuItemCPUStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.CPUSummary);
                    break;
                case "menuItemMemoryStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.MemorySummary);
                    break;
                case "menuItemDiskDetails":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DiskDetails);
                    break;
                case "menuItemDiskStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DiskSummary);
                    break;
                case "menuItemReplicationStatistics":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.ReplicationSummary);
                    break;
                case "menuItemDiskSpaceUsage":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DiskSpaceUsage);
                    break;
                case "menuItemDiskSpaceHistory":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DiskSpaceHistory);
                    break;

            }

            e.Handled = true;
        }

        private void MenuItemAnalyzePlan_Click(object sender, RoutedEventArgs e)
        {
            var menuItemClicked = sender as System.Windows.Controls.MenuItem;

            if (menuItemClicked == null)
                return;

            switch (menuItemClicked.Name)
            {

                case "menuItemDiskSpaceUsageForecast":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DiskSpaceForecast);
                    break;
                case "menuItemDatabaseGrowthForecast":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.DatabaseGrowthForecast);
                    break;
                case "menuItemTableGrowthForecast":
                    ApplicationController.Default.ShowReportsView(Views.Reports.ReportTypes.TableGrowthForecast);
                    break;
            }

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
        private void AdminitrationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems[0] == null)
                return;

            TabItem item = e.AddedItems[0] as TabItem;
            if (item == null)
                return;

            switch (item.Name)
            {
                case "applicationSecurityMenu":
                    ApplicationController.Default.ShowAdministrationView(Views.Administration.AdministrationView.AdministrationNode.ApplicationSecurity);
                    break;
                case "customCountersMenu":
                    ApplicationController.Default.ShowAdministrationView(Views.Administration.AdministrationView.AdministrationNode.CustomCounters);
                    break;
                case "changeLogMenu":
                    ApplicationController.Default.ShowAdministrationView(Views.Administration.AdministrationView.AdministrationNode.AuditedActions);
                    break;
                case "importExportMenu":
                    ApplicationController.Default.ShowAdministrationView(Views.Administration.AdministrationView.AdministrationNode.ImportExport);
                    break;

            }
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

        private bool ReportIsSelected
        {
            get
            {
                
                return ((MainWindowViewModel)this.DataContext).ReportIsSelected;
            }
        }
        private bool HasNoCustomReports
        {
            get { return ((MainWindowViewModel)this.DataContext).HasNoCustomReports; }
        }
        private void NewCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).NewCustomReport();

            e.Handled = true;
        }

        private void DeleteCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).DeleteCustomReport();

            e.Handled = true;
        }

        private void EditCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).EditSelectedCustomReport();

            e.Handled = true;
        }

        private void ImportCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).ImportCustomReport();

            e.Handled = true;
        }

        private void ExportCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).ExportCustomReport();

            e.Handled = true;
        }

        private void ScheduleEmailCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).ScheduleEmailCustomReport();

            e.Handled = true;
        }

        private void DeployCustomReport_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).DeployCustomReport();

            e.Handled = true;
        }

        private void CustomReport_Click(object sender, RoutedEventArgs e)
        {
            var item = e.Source as System.Windows.Controls.MenuItem;

            if (item == null)
                return;

            var reportName = item.Header.ToString();


            if (reportName == "New" || reportName == "Delete" || reportName == "Edit" || reportName == "Export" || reportName == "Import")
                return;

            ((MainWindowViewModel)this.DataContext).SelectCustomReport(item.Header.ToString());
        }
    }
}
