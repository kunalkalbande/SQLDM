using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Idera.SQLdm.DesktopClient.Views.Reports.ReportControls;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using System.Data;
using System.IO;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    internal partial class ReportsNavigationPane : UserControl
    {
        #region ReportCategory enum

        public enum ReportCategory
        {
            GettingStarted,
            Monitor,
            Analyze,
            Plan,
            Custom,
            Unknown
        }

        #endregion

        private readonly Font childNodeFont = new Font("Arial", 8.25F, FontStyle.Regular);
        private readonly Font rootNodeFont = new Font("Arial", 9.75F, FontStyle.Regular);
        private ReportCategory activeReportCategory = ReportCategory.Unknown;
        private string selectedCustomReport = null;
        private List<string> customReports = new List<string>();

        private const string DELETECUSTOMREPORT =
            "Are you sure you would like to delete \"{0}\"?.  This action cannot be undone.";

        //Text for the name of the Activity TreeNode
        private const string ACTIVITY = "Activity";

        public ReportsNavigationPane()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            
            InitializeReportsNavigationTree();
            ApplicationModel.Default.CustomReports.Changed += CustomReports_Changed;
            ApplicationController.Default.ReportsViewChanged += ApplicationController_ReportsViewChanged;
            Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;       
        }

        public ReportTypes SelectedReport
        {
            get
            {
                if (reportsNavigationTreeView.SelectedNode != null)
                {
                    return ((Pair<ReportCategory, ReportTypes>) reportsNavigationTreeView.SelectedNode.Tag).Second;
                }
                else
                {
                    return ReportTypes.GettingStarted;
                }
            }
            set { SelectReport(value); }
        }

        private void ApplicationController_ReportsViewChanged(object sender, ReportsViewChangedEventArgs e)
        {
                SelectReport(e.NewView,e.ReportKey);
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            //customReports.Clear();
            InitializeCustomReportsCategoryNode();
            SelectReport(ReportTypes.GettingStarted);
        }
        
        private void SelectReport(ReportTypes report)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            SelectReport(report, null);
        }

        private void SelectReport(ReportTypes report, string reportKey)
        {
            TreeNode monitorNode = reportsNavigationTreeView.Nodes["Monitor"];
            TreeNode monitorServersNode = monitorNode.Nodes["Servers"];
            TreeNode monitorVirtualizationNode = monitorNode.Nodes["Virtualization"];
            TreeNode monitorChangeLogNode = monitorNode.Nodes[ACTIVITY];
            TreeNode analyzeNode = reportsNavigationTreeView.Nodes["Analyze"];
            TreeNode planNode = reportsNavigationTreeView.Nodes["Plan"];
            TreeNode servers = analyzeNode.Nodes["Servers"];
            TreeNode databases = analyzeNode.Nodes["Databases"];
            TreeNode resources = analyzeNode.Nodes["Resources"];
            TreeNode customNode = reportsNavigationTreeView.Nodes["Custom"];

            monitorNode.Collapse();
            analyzeNode.Collapse();
            planNode.Collapse();
            customNode.Collapse();

            deploymentOptionsContainerPanel.Visible = ReportsHelper.IsReportDeployable(report) &&
                                                      ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            //show the custom report options if the selected report is a custom report and the user is an administrator
            customReportOptionsContainerPanel.Visible = ReportsHelper.IsCustomReport(report) &&
                                                      ApplicationModel.Default.UserToken.IsSQLdmAdministrator;


            //If we have determined that we need to show custom report options then we also need deployment options
            if (customReportOptionsContainerPanel.Visible) 
                deploymentOptionsContainerPanel.Visible = true;

            reportOptionsPanel.Visible = report != ReportTypes.GettingStarted;
            //Start: SQLdm 8.6 (Vineet Kumar): Fixing defect DE20099
            //When user selects a non-custom report, it does not clear the selection of selectedCustomReport variable. So while deployement report name in this variable gets deployed. 
            //Fix :- clear the selection of selectedCustomReport variable
            if (report != ReportTypes.Custom)
            {
                selectedCustomReport = null;
            }
            //End: SQLdm 8.6 (Vineet Kumar): Fixing defect DE20099
            switch (report)
            {
                case ReportTypes.GettingStarted:
                    reportsNavigationTreeView.SelectedNode = reportsNavigationTreeView.Nodes[report.ToString()];
                    reportsNavigationTreeView.SelectedNode.Expand();
                    break;
                case ReportTypes.Custom:
                    selectedCustomReport = reportKey;

                    if (selectedCustomReport == null)
                    {
                        reportsNavigationTreeView.SelectedNode = reportsNavigationTreeView.Nodes[report.ToString()];
                    }
                    else
                    {
                        reportsNavigationTreeView.SelectedNode = customNode.Nodes[selectedCustomReport];
                    }
                    
                    if(reportsNavigationTreeView.SelectedNode != null) reportsNavigationTreeView.SelectedNode.Expand();
                    break;
                case ReportTypes.EnterpriseSummary:
                //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                case ReportTypes.DeadlockReport:
                //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                case ReportTypes.ServerSummary:
                case ReportTypes.ActiveAlerts:
                case ReportTypes.MirroringSummary:
                case ReportTypes.MetricThresholds:
                case ReportTypes.AlwaysOnTopology:
                case ReportTypes.AlertTemplateReport:
                case ReportTypes.AlertThreshold:
                case ReportTypes.TemplateComparison:
                    reportsNavigationTreeView.SelectedNode = monitorServersNode.Nodes[report.ToString()];
                    break;
                case ReportTypes.VirtualizationSummary:
                case ReportTypes.VirtualizationStatistics:
                    reportsNavigationTreeView.SelectedNode = monitorVirtualizationNode.Nodes[report.ToString()];
                    break;
                case ReportTypes.ChangeLogSummary:
                    reportsNavigationTreeView.SelectedNode = monitorChangeLogNode.Nodes[report.ToString()];
                    break;
                case ReportTypes.AlertHistory:
                case ReportTypes.ServerInventory:
                case ReportTypes.TopServers:
             
                //START: SQLDM-29252 10.4.1 - Feature Request for Uptime Report
                case ReportTypes.ServerUptime:
                //END: SQLDM-29252 10.4.1 - Feature Request for Uptime Report
                case ReportTypes.ServerStatistics:
                case ReportTypes.QueryOverview:
                case ReportTypes.QueryStatistics:
                case ReportTypes.TopQueries:
                //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report( To add this report under Servers Node in Analyze section)
                case ReportTypes.QueryWaitStatistics:
                //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report( To add this report under Servers Node in Analyze section)
                case ReportTypes.BaselineStatistics:
                reportsNavigationTreeView.SelectedNode = servers.Nodes[report.ToString()];
                    break;
                case ReportTypes.TopDatabases:
                case ReportTypes.DatabaseStatistics:
                case ReportTypes.TopDatabaseApps:
                case ReportTypes.MirroringHistory:
                case ReportTypes.TopTablesGrowth:
                case ReportTypes.TopTableFrag:
                case ReportTypes.TransactionLogStatistics:
                case ReportTypes.TempdbStatistics:
                case ReportTypes.AlwaysOnStatistics:
                    reportsNavigationTreeView.SelectedNode = databases.Nodes[report.ToString()];
                    break;
                case ReportTypes.MemorySummary:
                case ReportTypes.DiskSummary:
                case ReportTypes.CPUSummary:
                case ReportTypes.SessionsSummary:
                //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                case ReportTypes.DetailedSessionReport:
                //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                case ReportTypes.ReplicationSummary:
                case ReportTypes.DiskDetails:
                //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports (( To add this report under Resources Node in Analyze section)
                case ReportTypes.DiskSpaceUsage:
                case ReportTypes.DiskSpaceHistory:
                //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports (( To add this report under Resources Node in Analyze section)
                reportsNavigationTreeView.SelectedNode = resources.Nodes[report.ToString()];
                    break;
                case ReportTypes.DiskSpaceForecast:
                case ReportTypes.DatabaseGrowthForecast:
                case ReportTypes.TableGrowthForecast:
                    reportsNavigationTreeView.SelectedNode = planNode.Nodes[report.ToString()];
                    break;
                default:
                    reportsNavigationTreeView.SelectedNode = null;
                    break;
            }
        }

        private void InitializeReportsNavigationTree()
        {
            reportsNavigationTreeView.BeginUpdate();
            
            InitializeStartCategoryNode();
            InitializeMonitorCategoryNode();
            InitializeAnalyzeCategoryNode();
            InitializePlanCategoryNode();
            InitializeCustomReportsCategoryNode();

            reportsNavigationTreeView.EndUpdate();
        }

        private void CustomReports_Changed(object server, CustomReportCollectionChangedEventArgs e)
        {            
            reportsNavigationTreeView.BeginUpdate();

            switch (e.ChangeType)
            {
                case KeyedCollectionChangeType.Added:
                    foreach(Common.Objects.CustomReport report in e.Reports.Values)
                    {
                        if (!customReports.Contains(report.Name))
                        {
                            customReports.Add(report.Name);
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Removed:
                    foreach(Common.Objects.CustomReport report in e.Reports.Values)
                    {
                        while (customReports.Contains(report.Name))
                        {
                            customReports.Remove(report.Name);
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Replaced:
                    foreach(Common.Objects.CustomReport report in e.Reports.Values)
                    {
                        if(!customReports.Contains(report.Name))
                        {
                            customReports.Add(report.Name);
                        }
                    }
                    break;
                case KeyedCollectionChangeType.Cleared:
                    customReports.Clear();
                    break;
            }
            RepopulateCustomReports();
            
            reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();

            reportsNavigationTreeView.EndUpdate();
            //reportsNavigationTreeView.Invalidate();
        }

        /// <summary>
        /// Initialize custom reports page
        /// </summary>
        private void InitializeCustomReportsCategoryNode()
        {
            foreach (TreeNode node in reportsNavigationTreeView.Nodes)
            {
                if (node.Text == "Custom")
                {
                    reportsNavigationTreeView.Nodes.Remove(node);
                }
            }

            TreeNode customNode = reportsNavigationTreeView.Nodes.Add("Custom", "Custom");

            customNode.Tag = ReportCategory.Custom;

            PopulateCustomReports(customNode, true);
        }
        
        /// <summary>
        /// Find the custom nodes and load all the custom reports into it
        /// </summary>
        private void RepopulateCustomReports()
        {
            foreach(TreeNode node in reportsNavigationTreeView.Nodes)
            {
                if (node.Name != "Custom") continue;

                PopulateCustomReports(node, false);
                
                break;
            }
        }

        /// <summary>
        /// get custom reports from the repository.
        /// If there are none then just show the option for creating a new one
        /// </summary>
        /// <param name="customNode"></param>
        /// <param name="blnClear"></param>
        private void PopulateCustomReports(TreeNode customNode, bool blnClear)
        {
            if(blnClear) customNode.Nodes.Clear();
            

            if (customNode == null) return;
            
            //go through every repository report
            foreach (string reportName in customReports)
            {
                TreeNode report1Node = null;
                //if this is not already in the tree
                report1Node = !customNode.Nodes.ContainsKey(reportName) ? 
                    customNode.Nodes.Add(reportName, reportName) : 
                    customNode.Nodes[reportName];

                report1Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Custom, ReportTypes.Custom);
            }

            //look through the tree for items that have been deleted from the repository
            foreach(TreeNode node in customNode.Nodes)
            {
                if(node != null && !customReports.Contains(node.Name))
                {
                    if (customNode.Nodes.ContainsKey(node.Name))
                    {
                        customNode.Nodes[node.Name].Remove();
                    }
                }
            }

            if(customReports.Count == 0)
            {
                customReportOptionsContainerPanel.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            }
        }

        private void InitializeStartCategoryNode()
        {
            TreeNode startNode;

            startNode = reportsNavigationTreeView.Nodes.Add(ReportTypes.GettingStarted.ToString(), "Getting Started");
            startNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.GettingStarted, ReportTypes.GettingStarted);
        }

        private void InitializeMonitorCategoryNode()
        {
            TreeNode monitorNode;

            monitorNode = reportsNavigationTreeView.Nodes.Add("Monitor", "Monitor");
            monitorNode.Tag = ReportCategory.Monitor;

            // Servers Node
            TreeNode serversNode = monitorNode.Nodes.Add("Servers", "Servers");
            serversNode.Tag = ReportCategory.Monitor;

            TreeNode serversReport1Node =
                serversNode.Nodes.Add(ReportTypes.EnterpriseSummary.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.EnterpriseSummary));
            serversReport1Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.EnterpriseSummary);

            TreeNode serversReport2Node =
                serversNode.Nodes.Add(ReportTypes.ServerSummary.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.ServerSummary));
            serversReport2Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.ServerSummary);

            TreeNode serversReport3Node =
                serversNode.Nodes.Add(ReportTypes.ActiveAlerts.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.ActiveAlerts));
            serversReport3Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.ActiveAlerts);

            TreeNode serversReport4Node =
                serversNode.Nodes.Add(ReportTypes.MirroringSummary.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.MirroringSummary));
            serversReport4Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.MirroringSummary);

            TreeNode serversReport5Node =
                 serversNode.Nodes.Add(ReportTypes.MetricThresholds.ToString(),
                                       ReportsHelper.GetReportTitle(ReportTypes.MetricThresholds));
            serversReport5Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.MetricThresholds);

            TreeNode serversReport6Node =
                serversNode.Nodes.Add(ReportTypes.AlwaysOnTopology.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.AlwaysOnTopology));
            serversReport6Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.AlwaysOnTopology);
            //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
            TreeNode serversReport7Node =
                serversNode.Nodes.Add(ReportTypes.DeadlockReport.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.DeadlockReport));
            serversReport7Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.DeadlockReport);

            TreeNode serversReport8Node =
               serversNode.Nodes.Add(ReportTypes.AlertTemplateReport.ToString(),
                                     ReportsHelper.GetReportTitle(ReportTypes.AlertTemplateReport));
            serversReport8Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.AlertTemplateReport);

            TreeNode serversReport9Node =
          serversNode.Nodes.Add(ReportTypes.AlertThreshold.ToString(),
                                ReportsHelper.GetReportTitle(ReportTypes.AlertThreshold));
            serversReport9Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.AlertThreshold);
            TreeNode serversReport10Node =
            serversNode.Nodes.Add(ReportTypes.TemplateComparison.ToString(),
                                  ReportsHelper.GetReportTitle(ReportTypes.TemplateComparison));
            serversReport10Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.TemplateComparison);

            //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report

            // Virtualization Node
            TreeNode virtualizationNode = monitorNode.Nodes.Add("Virtualization", "Virtualization");
            virtualizationNode.Tag = ReportCategory.Monitor;

            TreeNode virtualizationReport1Node =
                virtualizationNode.Nodes.Add(ReportTypes.VirtualizationSummary.ToString(),
                          ReportsHelper.GetReportTitle(ReportTypes.VirtualizationSummary));
            virtualizationReport1Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.VirtualizationSummary);

            TreeNode virtualizationReport2Node =
                virtualizationNode.Nodes.Add(ReportTypes.VirtualizationStatistics.ToString(),
                          ReportsHelper.GetReportTitle(ReportTypes.VirtualizationStatistics));
            virtualizationReport2Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.VirtualizationStatistics);

            //Change Log Node
            AddChangeLogNode(monitorNode, ACTIVITY);
         }

        private void InitializeAnalyzeCategoryNode()
        {
            TreeNode analyzeNode;

            analyzeNode = reportsNavigationTreeView.Nodes.Add("Analyze", "Analyze");
            analyzeNode.Tag = ReportCategory.Analyze;

            TreeNode serversNode = analyzeNode.Nodes.Add("Servers", "Servers");
            serversNode.Tag = ReportCategory.Analyze;

            TreeNode serversReport3Node =
                serversNode.Nodes.Add(ReportTypes.TopServers.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.TopServers));
            serversReport3Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopServers);

            
            //START: SQLdm-29252 10.4.1 Feature Request for Uptime report

            TreeNode serverUptimeReportNode =
                serversNode.Nodes.Add(ReportTypes.ServerUptime.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.ServerUptime));
            serverUptimeReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.ServerUptime);

            //END: SQLdm-29252 10.4.1 Feature Request for Uptime report

            TreeNode serversReport2Node =
                serversNode.Nodes.Add(ReportTypes.ServerStatistics.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.ServerStatistics));
            serversReport2Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.ServerStatistics);

            TreeNode serversReport1Node =
                serversNode.Nodes.Add(ReportTypes.ServerInventory.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.ServerInventory));
            serversReport1Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.ServerInventory);

            TreeNode serversReport4Node =
                serversNode.Nodes.Add(ReportTypes.QueryOverview.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.QueryOverview));
            serversReport4Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.QueryOverview);

            TreeNode serversReport5Node =
                serversNode.Nodes.Add(ReportTypes.TopQueries.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.TopQueries));
            serversReport5Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopQueries);

            TreeNode serversReport6Node =
                serversNode.Nodes.Add(ReportTypes.AlertHistory.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.AlertHistory));
            serversReport6Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.AlertHistory);

            TreeNode serversReport7Node =
                serversNode.Nodes.Add(ReportTypes.BaselineStatistics.ToString(),
                                      ReportsHelper.GetReportTitle(ReportTypes.BaselineStatistics));
            serversReport7Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.BaselineStatistics);
            //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. (Added to add Query Waits Statistics Reportof Left Navigation Pane)
            TreeNode serversReport8Node =
               serversNode.Nodes.Add(ReportTypes.QueryWaitStatistics.ToString(),
                                     ReportsHelper.GetReportTitle(ReportTypes.QueryWaitStatistics));
            serversReport8Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.QueryWaitStatistics);
            //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report. (Added to add Query Waits Statistics Reportof Left Navigation Pane)
            TreeNode databasesNode = analyzeNode.Nodes.Add("Databases", "Databases");
            databasesNode.Tag = ReportCategory.Analyze;

            TreeNode databasesReport2Node =
                databasesNode.Nodes.Add(ReportTypes.TopDatabases.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.TopDatabases));
            databasesReport2Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopDatabases);

            TreeNode databasesReport3Node =
                databasesNode.Nodes.Add(ReportTypes.DatabaseStatistics.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DatabaseStatistics));
            databasesReport3Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DatabaseStatistics);

            TreeNode databasesReport1Node =
                databasesNode.Nodes.Add(ReportTypes.TopDatabaseApps.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.TopDatabaseApps));
            databasesReport1Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopDatabaseApps);

            TreeNode databasesReport4Node =
                databasesNode.Nodes.Add(ReportTypes.MirroringHistory.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.MirroringHistory));
            databasesReport4Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.MirroringHistory);

            TreeNode transactionLogReportNode =
                databasesNode.Nodes.Add(ReportTypes.TransactionLogStatistics.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.TransactionLogStatistics));
            transactionLogReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TransactionLogStatistics);

            TreeNode tablesReport1Node =
                databasesNode.Nodes.Add(ReportTypes.TopTablesGrowth.ToString(),
                                     ReportsHelper.GetReportTitle(ReportTypes.TopTablesGrowth));
            tablesReport1Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopTablesGrowth);

            TreeNode tablesReport2Node =
                databasesNode.Nodes.Add(ReportTypes.TopTableFrag.ToString(),
                                     ReportsHelper.GetReportTitle(ReportTypes.TopTableFrag));
            tablesReport2Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TopTableFrag);

            TreeNode tempdbStatisticsReportNode =
                databasesNode.Nodes.Add(ReportTypes.TempdbStatistics.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.TempdbStatistics));
            tempdbStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.TempdbStatistics);

            TreeNode tempdbAlwaysOnStatisticsReportNode =
                databasesNode.Nodes.Add(ReportTypes.AlwaysOnStatistics.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.AlwaysOnStatistics));
            tempdbAlwaysOnStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.AlwaysOnStatistics);

            TreeNode resourcesNode = analyzeNode.Nodes.Add("Resources", "Resources");
            resourcesNode.Tag = ReportCategory.Analyze;

            TreeNode sessionsStatisticsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.SessionsSummary.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.SessionsSummary));
            sessionsStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.SessionsSummary);
            //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                
            TreeNode detailedSessionReportNode =
                resourcesNode.Nodes.Add(ReportTypes.DetailedSessionReport.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DetailedSessionReport));
            detailedSessionReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DetailedSessionReport);

            //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report


            TreeNode cpuStatisticsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.CPUSummary.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.CPUSummary));
            cpuStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.CPUSummary);

            TreeNode memoryStatisticsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.MemorySummary.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.MemorySummary));
            memoryStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.MemorySummary);

            TreeNode diskDetailsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.DiskDetails.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DiskDetails));
            diskDetailsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DiskDetails);

           TreeNode diskStatisticsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.DiskSummary.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DiskSummary));
            diskStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DiskSummary);

            TreeNode replicationStatisticsReportNode =
                resourcesNode.Nodes.Add(ReportTypes.ReplicationSummary.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.ReplicationSummary));
            replicationStatisticsReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.ReplicationSummary);

            //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
            TreeNode diskSpaceUsageReportNode =
                resourcesNode.Nodes.Add(ReportTypes.DiskSpaceUsage.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DiskSpaceUsage));
            diskSpaceUsageReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DiskSpaceUsage);

            TreeNode diskSpaceHistoryReportNode =
                resourcesNode.Nodes.Add(ReportTypes.DiskSpaceHistory.ToString(),
                                        ReportsHelper.GetReportTitle(ReportTypes.DiskSpaceHistory));
            diskSpaceHistoryReportNode.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Analyze, ReportTypes.DiskSpaceHistory);
            //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
         }

        private void InitializePlanCategoryNode()
        {
            TreeNode planNode;

            planNode = reportsNavigationTreeView.Nodes.Add("Plan", "Plan");
            planNode.Tag = ReportCategory.Plan;

            TreeNode report1Node =
                planNode.Nodes.Add(ReportTypes.DiskSpaceForecast.ToString(),
                                   ReportsHelper.GetReportTitle(ReportTypes.DiskSpaceForecast));
            report1Node.Tag = new Pair<ReportCategory, ReportTypes>(ReportCategory.Plan, ReportTypes.DiskSpaceForecast);

            TreeNode report2Node =
                planNode.Nodes.Add(ReportTypes.DatabaseGrowthForecast.ToString(),
                                   ReportsHelper.GetReportTitle(ReportTypes.DatabaseGrowthForecast));
            report2Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Plan, ReportTypes.DatabaseGrowthForecast);

            TreeNode report3Node =
                planNode.Nodes.Add(ReportTypes.TableGrowthForecast.ToString(),
                                   ReportsHelper.GetReportTitle(ReportTypes.TableGrowthForecast));
            report3Node.Tag =
                new Pair<ReportCategory, ReportTypes>(ReportCategory.Plan, ReportTypes.TableGrowthForecast);
        }

        private void reportsNavigationTreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            if (e.Node.Level == 0)
            {
                DrawReportsNavigationRootNode(e);
            }
            else
            {
                DrawReportsNavigationChildNode(e);
            }
        }

        private void DrawReportsNavigationRootNode(DrawTreeNodeEventArgs e)
        {
            Font nodeFont = rootNodeFont;
            Font underlineFont = new Font(nodeFont, FontStyle.Underline);
            StringFormat nodeTextFormat = new StringFormat();

            nodeTextFormat.LineAlignment = StringAlignment.Center;
            nodeTextFormat.Trimming = StringTrimming.EllipsisCharacter;
            nodeTextFormat.FormatFlags |= StringFormatFlags.NoWrap;

            Rectangle nodeBounds = Rectangle.Inflate(e.Bounds, -5, -2);
            nodeBounds.X += reportsNavigationTreeView.Indent * e.Node.Level;
            nodeBounds.Width -= reportsNavigationTreeView.Indent * e.Node.Level;

            Rectangle textBounds = Rectangle.Inflate(nodeBounds, -4, 0);

            Color dimGray = Color.FromArgb(105, 105, 105);
            Color gray228 = Color.FromArgb(228, 228, 228);
            Color gray215 = Color.FromArgb(215, 215, 215);

            //if the mousepointer is over the node and the node has not been selected
            bool hotNotSelected = (e.State & TreeNodeStates.Hot) != 0 
                && (e.State & TreeNodeStates.Selected) == 0;

            //if no node has been selected
            if (reportsNavigationTreeView.SelectedNode == null)
            {
                if (hotNotSelected) nodeFont = underlineFont;
            }
            else
            {
                switch (e.Node.Text) //if a node has been selected
                {
                    case "Getting Started":
                        //if we are not drawing the selected node
                        if (e.Node.Tag != reportsNavigationTreeView.SelectedNode.Tag)
                            //mousepointer is over a non selected node
                            if (hotNotSelected) nodeFont = underlineFont;
                        break;
                    case "Custom Reports":
                        //if we are not drawing the selected node
                        if (e.Node.Tag != reportsNavigationTreeView.SelectedNode.Tag)
                            //mousepointer is over a non selected node
                            if (hotNotSelected) nodeFont = underlineFont;
                        break;
                    default:
                        if (reportsNavigationTreeView.SelectedNode.Tag.ToString() != "Custom")
                        {
                            //if drawing a non selected category
                            if ((ReportCategory)e.Node.Tag !=
                                ((Pair<ReportCategory, ReportTypes>)reportsNavigationTreeView.SelectedNode.Tag).First)
                                if (hotNotSelected) nodeFont = underlineFont;
                        }
                        break;
                }
            }

            //if a node has been selected
            if (reportsNavigationTreeView.SelectedNode != null)
            {
                object tagObject = null;
                //if the selected node is the custom category
                if (reportsNavigationTreeView.SelectedNode.Tag.ToString() == "Custom")
                {
                    tagObject = reportsNavigationTreeView.SelectedNode.Tag;
                }
                else
                {
                    tagObject = (e.Node.Text == "Getting Started" || e.Node.Text == "Custom Reports"
                                            ? reportsNavigationTreeView.SelectedNode.Tag
                                            : ((Pair<ReportCategory, ReportTypes>)
                                               reportsNavigationTreeView.SelectedNode.Tag).First);
                }

                //if the selected node is the getting started node
                if ((e.Node.Text == "Getting Started") || (e.Node.Tag == tagObject))
                {
                    //draw light on dark
                    DrawRoundRectangle(e.Graphics, nodeBounds, 3, dimGray, dimGray);
                    e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, textBounds, nodeTextFormat);

                    if ((e.Node.Text == "Getting Started"))
                    {
                        //Right arrow
                        e.Graphics.DrawImage(Resources.RightArrow_White_9h_5w, nodeBounds.Right - 10,
                                             nodeBounds.Top + (nodeBounds.Height / 2) -
                                             (Resources.RightArrow_White_9h_5w.Height / 2));

                    }
                    return;
                }
            }

            if (e.Node.Text == "Custom Reports")
            {
                string test = e.Node.Text;
            }

            //draw dark on light
            DrawRoundRectangle(e.Graphics, nodeBounds, 3, gray228, gray215);
            e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.Black, textBounds, nodeTextFormat);

            //up arrow if expanded else down arrow
            e.Graphics.DrawImage(e.Node.IsExpanded ? Resources.UpArrow_Gray_6h_11w : Resources.DownArrow_Gray_6h_11w,
                                 nodeBounds.Right - 15,
                                 nodeBounds.Top + (nodeBounds.Height / 2) -
                                 (Resources.UpArrow_Gray_6h_11w.Height / 2));

        }


        private void DrawReportsNavigationChildNode(DrawTreeNodeEventArgs e)
        {
            Font nodeFont = childNodeFont;
            StringFormat nodeTextFormat = new StringFormat();
            nodeTextFormat.LineAlignment = StringAlignment.Center;
            nodeTextFormat.Trimming = StringTrimming.EllipsisCharacter;
            nodeTextFormat.FormatFlags |= StringFormatFlags.NoWrap;

            Rectangle nodeBounds = Rectangle.Inflate(e.Bounds, -5, -2);
            nodeBounds.X += reportsNavigationTreeView.Indent*e.Node.Level;
            nodeBounds.Width -= reportsNavigationTreeView.Indent*e.Node.Level;
            Rectangle textBounds = Rectangle.Inflate(nodeBounds, -8, 0);

            if ((e.State & TreeNodeStates.Hot) != 0 &&
                (e.State & TreeNodeStates.Selected) == 0)
            {
                nodeFont = new Font(nodeFont, FontStyle.Underline);
            }

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                DrawRoundRectangle(e.Graphics, nodeBounds, 3, Color.FromArgb(105, 105, 105),
                                   Color.FromArgb(105, 105, 105));
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, textBounds, nodeTextFormat);
                e.Graphics.DrawImage(Resources.RightArrow_White_9h_5w, nodeBounds.Right - 10,
                                     nodeBounds.Top + (nodeBounds.Height/2) -
                                     (Resources.RightArrow_White_9h_5w.Height/2));
            }
            else
            {
                using (SolidBrush fillBrush = new SolidBrush(reportsNavigationTreeView.BackColor))
                {
                    e.Graphics.FillRectangle(fillBrush, e.Node.Bounds);
                }

                using (Pen dotLinePen = new Pen(Color.LightGray))
                {
                    dotLinePen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawLine(dotLinePen, nodeBounds.Left, nodeBounds.Bottom, nodeBounds.Right,
                                        nodeBounds.Bottom);
                }

                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.Black, textBounds, nodeTextFormat);

                if (e.Node.Nodes.Count > 0)
                {
                    if (e.Node.IsExpanded)
                    {
                        e.Graphics.DrawImage(Resources.DownArrow_Gray_7w_4h, nodeBounds.Left + 2,
                                             nodeBounds.Top + (nodeBounds.Height/2) -
                                             (Resources.DownArrow_Gray_7w_4h.Height/2));
                    }
                    else
                    {
                        e.Graphics.DrawImage(Resources.RightArrow_Gray_4w_7h, nodeBounds.Left + 2,
                                             nodeBounds.Top + (nodeBounds.Height/2) -
                                             (Resources.RightArrow_Gray_4w_7h.Height/2));
                    }
                }
            }
        }

        private static bool IsAncestorNode(TreeNode targetNode, TreeNode testNode)
        {
            if (targetNode != null && targetNode.Parent != null)
            {
                if (targetNode.Parent == testNode)
                {
                    return true;
                }
                else
                {
                    return IsAncestorNode(targetNode.Parent, testNode);
                }
            }
            else
            {
                return false;
            }
        }

        private static void DrawRoundRectangle(Graphics graphics, RectangleF bounds, float radius, Color fillColor,
                                               Color borderColor)
        {
            if (graphics != null && bounds.Width > 0 && bounds.Height > 0)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(bounds.Left + radius, bounds.Top, bounds.Width - (radius*2), bounds.Top);
                    gp.AddArc(bounds.Left + bounds.Width - (radius*2), bounds.Top, radius*2, radius*2, 270, 90);
                    gp.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Top + bounds.Height - (radius*2));
                    gp.AddArc(bounds.Right - (radius*2), bounds.Top + bounds.Height - (radius*2), radius*2, radius*2, 0,
                              90);
                    gp.AddLine(bounds.Right - (radius*2), bounds.Top + bounds.Height, bounds.Left + radius,
                               bounds.Top + bounds.Height);
                    gp.AddArc(bounds.Left, bounds.Top + bounds.Height - (radius*2), radius*2, radius*2, 90, 90);
                    gp.AddLine(bounds.Left, bounds.Top + bounds.Height - (radius*2), bounds.Left, bounds.Top + radius);
                    gp.AddArc(bounds.Left, bounds.Top, radius*2, radius*2, 180, 90);
                    gp.CloseFigure();

                    if (fillColor != Color.Empty)
                    {
                        using (SolidBrush fillBrush = new SolidBrush(fillColor))
                        {
                            graphics.FillPath(fillBrush, gp);
                        }
                    }

                    if (borderColor != Color.Empty)
                    {
                        using (Pen pen = new Pen(borderColor, 1))
                        {
                            graphics.DrawPath(pen, gp);
                        }
                    }
                }
            }
        }

        private void reportsNavigationTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();
        }

        private void reportsNavigationTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();
        }

        private int GetReportsNavigationTreeViewHeight()
        {
            return GetExpandedNodeCount(reportsNavigationTreeView.Nodes)*(reportsNavigationTreeView.ItemHeight + 1);
        }

        private static int GetExpandedNodeCount(TreeNodeCollection nodes)
        {
            int nodeCount = 0;

            foreach (TreeNode node in nodes)
            {
                ++nodeCount;

                if (node.IsExpanded)
                {
                    nodeCount += GetExpandedNodeCount(node.Nodes);
                }
            }
            return nodeCount;
        }

        private void reportsNavigationTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                if (!IsAncestorNode(reportsNavigationTreeView.SelectedNode, e.Node))
                {
                    e.Node.Toggle();
                }
                e.Cancel = true;
            }
        }

        private void reportsNavigationTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (IsAncestorNode(reportsNavigationTreeView.SelectedNode, e.Node))
            {
                e.Cancel = true;
            }
        }

        private void reportsNavigationTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string customReportKey = null;
            
            lblEditCustomReport.Enabled = false;
            lblDeleteCustomReport.Enabled = false;
            lblExportCustomReport.Enabled = false;

            if (e.Node.Name == "Custom")
            {
                if(reportOptionsPanel.Visible ==false) reportOptionsPanel.Visible = true;
                customReportOptionsContainerPanel.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            }

            if (e.Node.Tag is Pair<ReportCategory, ReportTypes>)
            {
                Pair<ReportCategory, ReportTypes> tag = (Pair<ReportCategory, ReportTypes>)e.Node.Tag;
                if (tag.First.ToString() == "Custom")
                {
                    customReportKey = e.Node.Name;
                    lblEditCustomReport.Enabled = true;
                    lblDeleteCustomReport.Enabled = true;
                    lblExportCustomReport.Enabled = true;
                }
                //sqldm -29917 start  - Disabling deploy report option when Detailed Session Report is clicked
                if (tag.Second == ReportTypes.DetailedSessionReport)
                    reportDeploy.Enabled = false;
                else
                    reportDeploy.Enabled = true;
                //sqldm -29917 end
                ApplicationController.Default.ShowReportsView(tag.Second, null, customReportKey, null);
                //ApplicationController.Default.ShowReportsView(tag.Second);
            }
            else if (e.Node.Tag.ToString() == "Custom")
            {
                if (reportsNavigationTreeView.SelectedNode != null)
                    PopulateCustomReports(e.Node, true);
            }
        }

        private void reportsNavigationTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (reportsNavigationTreeView.SelectedNode.Tag is Pair<ReportCategory, ReportTypes>)
            {
                Pair<ReportCategory, ReportTypes> tag =
                    (Pair<ReportCategory, ReportTypes>)reportsNavigationTreeView.SelectedNode.Tag;

                if (activeReportCategory != tag.First)
                {
                    reportsNavigationTreeView.Invalidate();
                }

                activeReportCategory = tag.First;
            }
            else
            {
                if (reportsNavigationTreeView.SelectedNode.Text == "Custom")
                {
                    PopulateCustomReports(reportsNavigationTreeView.SelectedNode, true);
                }
            }
        }

        void lblImportCustomReport_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Idera.SQLdm.Common.Objects.CustomReport report;
            DialogResult dialogResult = CustomReportImportWizard.ImportNewReport(this, out report);
            if (report != null)
            {
                reportsNavigationTreeView.BeginUpdate();
                RepopulateCustomReports();
                reportsNavigationTreeView.Invalidate();
                reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();
                reportsNavigationTreeView.EndUpdate();
            }
        }



        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Integration) -- Exports the selected report to an xml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lblExportCustomReport_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            var managementService = Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(connectionInfo);
            var selectedReport = reportsNavigationTreeView.SelectedNode.Name;
            if (selectedReport != null)
            {
                try
                {
                    var _CurrentCustomReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                selectedReport);

                    //includes aggregation that may have been set up previously
                    var _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                    //populate the counters that have already been selected
                    if (_CurrentCustomReport.Metrics == null) _CurrentCustomReport.Metrics = new SortedDictionary<int, CustomReportMetric>();

                    _CurrentCustomReport.Metrics.Clear();

                    foreach (DataRow row in _selectedCountersDataTable.Rows)
                    {
                        string metricName = row["CounterName"].ToString();
                        string metricDescription = row["CounterShortDescription"].ToString();

                        //_selectedCounters.Add(metricName, metricDescription);

                        Idera.SQLdm.Common.Objects.CustomReport.CounterType type = (Idera.SQLdm.Common.Objects.CustomReport.CounterType)int.Parse(row["CounterType"].ToString());
                        Idera.SQLdm.Common.Objects.CustomReport.Aggregation aggregation = (Idera.SQLdm.Common.Objects.CustomReport.Aggregation)int.Parse(row["Aggregation"].ToString());

                        _CurrentCustomReport.Metrics.Add(int.Parse(row["GraphNumber"].ToString()),
                                    new CustomReportMetric(metricName, metricDescription, type, aggregation));

                    }

                    string xml = Idera.SQLdm.DesktopClient.Helpers.CustomReportHelper.SerializeCustomReport(_CurrentCustomReport);

                    using (var sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "xml files (*.xml)|*.xml";
                        sfd.FileName = _CurrentCustomReport.Name + ".xml";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(sfd.FileName, xml);
                            ApplicationMessageBox.ShowInfo(this, "Selected custom report exported successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Export operation failed for selected report: ", ex);
                }
            }
        }
        
        /// <summary>
        /// SQLdm 9.1 (Vineet Kumar) (Community Center) -- Adding community centre link to reports navigation pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lblCommunityCentre_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Common.Constants.CommunitySiteBaseUrl);
        }
        private void reportDeploy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                DeployReportsWizard wizard =
                    new DeployReportsWizard(((ReportsView)
                ApplicationController.Default.ActiveView).ActiveReport.ReportType, null, selectedCustomReport, reports);

                wizard.ShowDialog(ParentForm);
            }
        }
        private void scheduleEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ApplicationController.Default.ActiveView is ReportsView)
            {
                ReportContol reportControl = ((ReportsView) ApplicationController.Default.ActiveView).ActiveReport;
                
                string message;

                if (reportControl.CanRunReport(out message))
                {
                    ICollection<Common.Objects.CustomReport> reports =
                    Helpers.RepositoryHelper.GetCustomReportsList(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    DeployReportsWizard wizard =
                        new DeployReportsWizard(reportControl.ReportType, reportControl.GetReportParmeters(), selectedCustomReport, reports);
                    wizard.ShowDialog(ParentForm);
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(ParentForm, message);
                }
            }
        }

        private void reportAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.ActiveView.ShowHelp();
        }

        private void lblNewCustomReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (reportsNavigationTreeView.SelectedNode == null) return;

            CustomReportWizard wizard = new CustomReportWizard();
            wizard.ShowDialog(ParentForm);
            
            reportsNavigationTreeView.BeginUpdate();
            RepopulateCustomReports();
            reportsNavigationTreeView.Invalidate();
            reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();
            reportsNavigationTreeView.EndUpdate();
        }

        private void lblEditCustomReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomReportWizard wizard = new CustomReportWizard(reportsNavigationTreeView.SelectedNode);
            
            if (wizard.IsDisposed) return;

            wizard.ShowDialog(ParentForm);

            RepopulateCustomReports();
            reportsNavigationTreeView.Invalidate();
        }

        private void lblDeleteCustomReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            if (!(reportsNavigationTreeView.SelectedNode.Tag is Pair<ReportCategory, ReportTypes>)) return;
            
            if(ApplicationMessageBox.ShowQuestion(Parent, string.Format(DELETECUSTOMREPORT,reportsNavigationTreeView.SelectedNode.Name)) != DialogResult.Yes) return;

            TreeNode previous = reportsNavigationTreeView.SelectedNode.PrevNode;

            ApplicationModel.Default.RemoveCustomReport(reportsNavigationTreeView.SelectedNode.Name);

            RepopulateCustomReports();
            
            reportsNavigationTreePanel.Height = GetReportsNavigationTreeViewHeight();
            reportsNavigationTreeView.Invalidate();

            if (previous == null)
            {
                ApplicationController.Default.ShowReportsView(ReportTypes.GettingStarted);
                return;
            }

            reportsNavigationTreeView.SelectedNode = previous;
            
            if(reportsNavigationTreeView.SelectedNode.Tag is Pair<ReportCategory, ReportTypes>)
            {
                if(((Pair<ReportCategory, ReportTypes>)reportsNavigationTreeView.SelectedNode.Tag).Second.ToString().Equals("Custom"))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, previous.Name, null);
                }
            }
        }

        /// <summary>
        /// Verify if the user that are accessing to the Report tab is Administrator, 
        /// and if it is an administrator the method will add an "Activity" tree node
        /// </summary>
        public void ValidateActivityVisibility()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                //Add Change Log Node
                TreeNode monitorNode = reportsNavigationTreeView.Nodes["Monitor"];
                AddChangeLogNode(monitorNode, ACTIVITY);
            }
            else
            {
                //Remove Change Log Node
                TreeNode monitorNode = reportsNavigationTreeView.Nodes["Monitor"];
                RemoveNode(monitorNode, ACTIVITY);
            }
        }

        /// <summary>
        /// Add a Change Log child Node to the Monitoring parentNode
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childName"></param>
        private void AddChangeLogNode(TreeNode parentNode, string childName)
        {
            if (!parentNode.Nodes.ContainsKey(childName))
            {
                TreeNode changeLogNode = parentNode.Nodes.Add(childName, childName);
                changeLogNode.Tag = ReportCategory.Monitor;

                TreeNode changeLogReport1Node =
                    changeLogNode.Nodes.Add(ReportTypes.ChangeLogSummary.ToString(),
                                            ReportsHelper.GetReportTitle(ReportTypes.ChangeLogSummary));
                changeLogReport1Node.Tag
                    = new Pair<ReportCategory, ReportTypes>(ReportCategory.Monitor, ReportTypes.ChangeLogSummary);
            }
        }

        /// <summary>
        /// Remove any childNode from a parentNode using the childName
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childName"></param>
        private void RemoveNode(TreeNode parentNode, string childName)
        {
            if (parentNode.Nodes.ContainsKey(childName))
            {
                parentNode.Nodes.RemoveByKey(childName);
            }

        }
    }
}
