using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Reports.ReportControls;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;
using DatabaseStatistics = Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.DatabaseStatistics;
using ReplicationSummary = Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReplicationSummary;
using ServerStatistics = Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ServerStatistics;
using ServerSummary = Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ServerSummary;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    public enum ReportTypes
    {
        [Title("Getting Started")]
        GettingStarted,
        [Title("Enterprise Summary")]
        [ShortDescription("View the health of your SQL Servers")]
        [LongDescription("The Enterprise Summary report provides an overview of the health and performance of SQL Server instances monitored by SQLDM. Click on an instance and the Server Summary report will display.")]
        [Deployable(true)]
        EnterpriseSummary,
        [Title("Server Summary")]
        [ShortDescription("View health details of a single SQL Server")]
        [LongDescription("The Server Summary report provides a detailed overview of the health and performance of a specific SQL Server, such as response time and CPU usage.")]
        [Deployable(true)]
        ServerSummary,
        [Title("Active Alerts")]
        [ShortDescription("View the active alerts for all monitored SQL Servers")]
        [LongDescription("The Active Alerts report lists all outstanding, active alerts that SQLDM has raised against your SQL Server instances. To see more detail about the health of a specific instance, click on that instance and the Server Summary report will display.")]
        [Deployable(true)]
        ActiveAlerts,
        [Title("Alert History")]
        [ShortDescription("Analyze the alert history for your SQL Servers")]
        [LongDescription("The Alert History report lets you review the alerts that SQLDM has raised for the selected SQL Server instance over a specified period of time. You can also review alerts for multiple instances that belong to a tag.")]
        [Deployable(true)]
        AlertHistory,
        [Title("Server Inventory")]
        [ShortDescription("Find SQL Servers that share the same properties")]
        [LongDescription("The Server Inventory report lets you compile a list of SQL Server instances that share up to 4 different properties, such as the SQL Server version or whether the instance is clustered.")]
        [Deployable(true)]
        ServerInventory,
        [Title("Top Servers")]
        [ShortDescription("Identify your worst performing SQL Servers")]
        [LongDescription("The Top Servers report identifies your worst performing SQL Server instances based on the number of active alerts for the current monitored instances, response time, CPU usage, memory usage, and disk activity.  Use this report to compile a list of instances that need immediate attention.")]
        [Deployable(true)]
        TopServers,
        [Title("Server Statistics")]
        [ShortDescription("Analyze and compare performance trends across two SQL Servers")]
        [LongDescription("The Server Statistics report lets you analyze and compare performance trends across two SQL Server instances. You can focus on a single performance statistic, such as memory usage, and track its value on both instances over a specified period of time.")]
        [Deployable(true)]
        ServerStatistics,
        [Title("Query Overview")]
        [ShortDescription("Identify your worst performing queries")]
        [LongDescription("The Query Overview report identifies the worst performing queries run on a specific SQL Server instance over time, including the call volume of each query. This report requires that you enabled Query Monitoring in the SQLDM Console and have collected sufficient query data for the specified time period.")]
        [Deployable(true)]
        QueryOverview,
        [Title("Top Queries")]
        [ShortDescription("Find queries that are performing poorly or executing frequently")]
        [LongDescription("The Top Queries report lets you compile a list of queries based on call frequency, duration of execution, CPU usage, and the number of reads and writes performed on the databases hosted by the specified SQL Server instance. You can define minimum thresholds for each of these performance metrics and then see which queries match or exceed the selected values. This report requires that you enabled Query Monitoring in the SQLDM Console and have collected sufficient query data for the specified time period.")]
        [Deployable(true)]
        TopQueries,
        [Title("Query Statistics")]
        [ShortDescription("")]
        [LongDescription("The Query Statistics report lets you view details for an individual SQL statement or signature as collected by the Query Monitor. This report requires that you enabled Query Monitoring in the SQLDM Console and have collected sufficient query data for the specified time period.")]
        QueryStatistics,
        [Title("Top Databases")]
        [ShortDescription("Identify your worst performing databases")]
        [LongDescription("The Top Databases report identifies your worst performing SQL Server databases based on the size of the database, the growth rate of the database, and the number of reads, writes, and transactions per second performed on the database. Use this report to compile a list of databases that are used most often or have the heaviest loads.")]
        [Deployable(true)]
        TopDatabases,
        [Title("Database Statistics")]
        [ShortDescription("Analyze and compare performance trends across two databases")]
        [LongDescription("The Database Statistics report lets you analyze and compare performance trends, such as the percentage of growth, across two SQL Server databases. You can also compare performance metrics at different points in time for the same database.")]
        [Deployable(true)]
        DatabaseStatistics,
        [Title("Top Database Applications")]
        [ShortDescription("Analyze and compare performance trends across two databases")]
        [LongDescription("The Top Database Applications report lets you find the applications that are consuming the highest amount of your system resources, such as CPU usage, when run on a specific database. You can define minimum thresholds for each of these performance metrics and then see which applications match or exceed the selected values. This report requires that you enabled Query Monitoring in the SQLDM Console and have collected sufficient query data for the specified time period.")]
        [Deployable(true)]
        TopDatabaseApps,
        [Title("Top Tables by Growth")]
        [ShortDescription("Identify the fastest growing tables")]
        [LongDescription("The Top Tables by Growth report identifies the fastest growing tables in a database. You can select a specific database to analyze or evaluate all databases on a specific SQL Server instance. You can also choose a growth metric and its minimum threshold to see which tables match or exceed the selected value.")]
        [Deployable(true)]
        TopTablesGrowth,
        [Title("Top Tables by Fragmentation")]
        [ShortDescription("Identify the most fragmented tables")]
        [LongDescription("The Top Tables by Fragmentation report identifies the most fragmented tables in a database. You can select a specific database to analyze or evaluate all databases on a specific SQL Server instance. You can also choose a fragmentation metric and its minimum threshold to see which tables match or exceed the selected value.")]
        [Deployable(true)]
        TopTableFrag,
        [Title("Disk Space Usage Forecast")]
        [ShortDescription("Forecast disk space needs based on average historical growth rates")]
        [LongDescription("The Disk Space Usage Forecast report lets you predict how much disk space may be needed in the future based on current and historical growth rate trends. For more accurate forecasting, select the largest possible period of historical data and the shortest possible projection into the future. To successfully collect disk metrics, ensure that OS Metrics collection is enabled for the monitored instance.")]
        DiskSpaceForecast,
        [Title("Database Growth Forecast")]
        [ShortDescription("Forecast future database growth based on historical trends")]
        [LongDescription("The Database Growth Forecast report lets you predict how large selected databases may grow based on current and historical growth rate trends. For more accurate forecasting, select the largest possible period of historical data and the shortest possible projection into the future.")]
        DatabaseGrowthForecast,
        [Title("Table Growth Forecast")]
        [ShortDescription("Forecast future table growth based on historical trends")]
        [LongDescription("The Table Growth Forecast report lets you predict how large selected tables may grow based on current and historical growth rate trends. For more accurate forecasting, select the largest possible period of historical data and the shortest possible projection into the future.")]
        TableGrowthForecast,
        [Title("Mirroring Summary")]
        [ShortDescription("View the health of your mirrored databases")]
        [LongDescription("The Mirroring Summary report provides an overview of the health of mirrored databases and the health of their mirrors, such as the mirrored state and operational status.")]
        [Deployable(true)]
        MirroringSummary,
        [Title("Mirroring History")]
        [ShortDescription("Analyze the event history for a mirrored database")]
        [LongDescription("The Mirroring History report lets you analyze metrics collected for a mirrored database during the specified time period. You can filter the results to show only those metrics for which alerts have been raised. To filter your results, check Show Problems Only.")]
        [Deployable(true)]
        MirroringHistory,
        [Title("Memory Statistics")]
        [ShortDescription("Track key memory performance metrics")]
        [LongDescription("The Memory Statistics report lets you view trends for memory usage on a monitored SQL Server instance over a specified period of time. Check Show Baseline to include baseline data points in the graphical results and numeric baseline metrics in the tabular results. To successfully monitor OS metrics, ensure that OS Metrics collection is enabled on the monitored instance.")]
        [Deployable(true)]
        MemorySummary,
        [Title("Disk Statistics")]
        [ShortDescription("Track key disk performance metrics")]
        [LongDescription("The Disk Statistics report lets you view trends for the disk activity of a monitored SQL Server instance over a specified period of time. Check Show Baseline to include baseline data points in the graphical results and numeric baseline metrics in the tabular results. To successfully monitor OS metrics, ensure that OS Metrics collection is enabled on the monitored instance.")]
        [Deployable(true)]
        DiskSummary,
        [Title("CPU Statistics")]
        [ShortDescription("Track key CPU performance metrics")]
        [LongDescription("The CPU Statistics report lets you view trends for CPU usage on a monitored SQL Server instance over a specified period of time. Check Show Tabular Data to display results in a single tabular or grid format.Check Show Baseline to include baseline data points in the graphical results and numeric baseline metrics in the tabular results.")]
        [Deployable(true)]
        CPUSummary,
        [Title("Session Statistics")]
        [ShortDescription("Track key session and network performance metrics over time")]
        [LongDescription("The Session Statistics report lets you view trends for sessions and network connectivity, such as transactions per minute, on a monitored SQL Server instance over a specified period of time. Check Show Baseline to include baseline data points in the graphical results and numeric baseline metrics in the tabular results.")]
        [Deployable(true)]
        SessionsSummary,
        [Title("Replication Statistics")]
        [ShortDescription("Track key replication performance metrics")]
        [LongDescription("The Replication Statistics report lets you view trends for the replicated transactions on a monitored SQL Server instance over a specified period of time. Check Show Baseline to include baseline data points in the graphical results and numeric baseline metrics in the tabular results.")]
        [Deployable(true)]
        ReplicationSummary,
        [Title("Custom Reports")]
        [CustomReport(true)]
        [LongDescription("This report lets you view trends for your choice of SQLDM metrics on a monitored SQL Server instance over a specified period of time. To successfully monitor OS metrics, ensure that WMI/OLE Automation is enabled on the monitored instance.")]
        Custom,
        [Title("Metric Thresholds")]
        [ShortDescription("List all metric thresholds for a server")]
        [LongDescription("The Metric Thresholds report will display all the metric thresholds for a server.")]
        [Deployable(true)]
        MetricThresholds,
        [Title("Disk Details")]
        [ShortDescription("List key disk metrics for a server")]
        [LongDescription("The Disk Details report will display key disk metrics for a selected server.")]
        [Deployable(true)]
        DiskDetails,
        [Title("Virtualization Summary")]
        [ShortDescription("Summary of your virtualized environment")]
        [LongDescription("The Virtualization Summary report will give you an overall performance summary of your virtualized servers.")]
        [Deployable(true)]
        VirtualizationSummary,
        [Title("Tempdb Statistics")]
        [ShortDescription("Analyze the performance and space utilization of tempdb over time")]
        [LongDescription("The Tempdb Statistics report lets you view space utilization and data throughout statistics over time for the tempdb database.")]
        [Deployable(true)]
        TempdbStatistics,
        [Title("Virtualization Statistics")]
        [ShortDescription("Analyze your virtualization performance trends for a specific virtualized server")]
        [LongDescription("The Virtualization Statistics report lets you analyze your virtualization performance trends for a specific virtualized SQL Server instance. You can track CPU, Memory, Disk and Network performance metrics for the virtual machine and its host server to help identify potential performance bottle necks.")]
        [Deployable(true)]
        VirtualizationStatistics,
        [Title("Transaction Log Statistics")]
        [ShortDescription("Analyze the database transaction log")]
        [LongDescription("The Transaction Log Statistics report provides data about the database transaction log size and activity on a per-database basis. Note that SQLDM displays the Cache Reads and Flushes chart types as data per second. This report helps you troubleshoot performance problems related to transaction logs.")]
        [Deployable(true)]
        TransactionLogStatistics,
        [Title("Baseline Statistics")]
        [ShortDescription("Analyze and compare performance baselines across two SQL Server instances")]
        [LongDescription("The Baselines Statistics report lets you analyze and compare metric baselines either within a single SQL Server instance or between two instances at the same time. You can focus on a baseline for a specific date range, and compare it with the same or different baseline configuration on another instance to measure server performance.")]
        [Deployable(true)]
        BaselineStatistics,
        [Title("Change Log Summary")]
        [ShortDescription("Summary of all the key actions performed in SQLDM")]
        [LongDescription("The Change Log Summary report lists all key actions/changes performed in your SQLDM environment.")]
        [Deployable(true)]
        ChangeLogSummary,
        [Title("Availability Group Statistics")]
        [ShortDescription("View the historical health of your availability groups, availability replicas, and availability databases")]
        [LongDescription("The Availability Group Statistics report shows the health of your availability groups, availability replicas, and availability databases over a selected period of time.")]
        [Deployable(true)]
        AlwaysOnStatistics,
        [Title("Availability Group Topology")]
        [ShortDescription("View the current topology of your Availability Groups configuration")]
        [LongDescription("The Availability Group Topology report displays all of the availability groups on monitored servers, replicas that participate in the groups as well as the databases within these groups.")]
        [Deployable(true)]
        AlwaysOnTopology,
        [Title("Availability Group Database Statistics")]
        [ShortDescription("A sub-report from the Availability Group Statistics report")]
        [LongDescription("A sub-report from the Availability Group Statistics report")]
        [Deployable(true)]
        AlwaysOnDatabaseStatistics
        //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
        ,
        [Title("Query Wait Statistics")]
        [ShortDescription("Analyze the wait statistics for a server.")]
        [LongDescription("The Query Wait Statistics report lets you analyze different wait type categories on your SQL Server instance. By analyzing these waits you can better determine where your biggest bottlenecks are occurring and what changes could have the greatest performance impact. You can select from various wait type categories such as Backup, Excluded, I/O, Lock, Memory, Non-I/O Page Latch, Non-Page Latch, Transaction Log, and Other.")]
        [Deployable(true)]
        QueryWaitStatistics
        //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report

        //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History
        ,
        [Title("Disk Space Usage")]
        [ShortDescription("Analyze the Disk Space Usage for the disks on a server")]
        [LongDescription("The Disk Space Usage lets you analyze the most recent snapshot of available disk space. By analyzing these waits you can better determine where in their environment they have the most and the least disk space available.")]
        [Deployable(true)]
        DiskSpaceUsage
        ,
        [Title("Disk Space History")]
        [ShortDescription("Analyze the Disk Space history for the disks on a server")]
        [LongDescription("The Disk Space History report lets you analyze the history of activity for a disk. By analyzing these waits you can better determine how are the disks are performing in their envirnoment.")]
        [Deployable(true)]
        DiskSpaceHistory
        //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History
        ,
        //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
        [Title("Deadlock Report")]
        [ShortDescription("Analyze deadlock trends across database")]
        [LongDescription("The Deadlock report lets you analyze deadlock trends across SQL Server database.")]
        [Deployable(true)]
        DeadlockReport
        //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
        ,
        //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
        [Title("Detailed Session Report")]
        [ShortDescription("Analyze Detailed Session Information across database")]
        [LongDescription("The Detailed Session report provides detailed session information across SQL Server database.")]
        [Deployable(true)]
        DetailedSessionReport
        //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
        ,
        //START:

        [Title("Template Comparison")]
        [ShortDescription("Template Comparison for Source and Tartget Metrics")]
        [LongDescription("The Template Comparison report will display all the metric thresholds differences for selected Source and Target templates.")]
        [Deployable(true)]
        TemplateComparison
        ,
        [Title("Alert Template Report")]
        [ShortDescription("Alert Template for Instance with applied Alert template")]
        [LongDescription("The Alert Template report will display list of Instances with Alert Template Assigned per instance ")]
        [Deployable(true)]
        AlertTemplateReport
         ,
        [Title("Alert Threshold Report")]
        [ShortDescription("Alert Threshold for Server comparison threshold value")]
        [LongDescription("TheAlert Threshold report provides will display all the metric thresholds differences for Default and Metric Threshold.")]
        [Deployable(true)]
        AlertThreshold
		,
        //END
		//START: SQLdm-29252 10.4.1 Feature Request for Uptime report
        [Title("Server Uptime")]
        [ShortDescription("Analyze Uptime Information")]
        [LongDescription("Analyse Uptime Information")]
        [Deployable(true)]
        ServerUptime
        //END: SQLdm-29252 10.4.1 Feature Request for Uptime report
    }

    internal partial class ReportsView : View
    {
        private const int ReportHistoryLimit = 10;

        private const string TitleLabelText = "Reports - {0}";
        private ReportTypes currentView;
        private GettingStartedControl gettingStartedControl;
        private int reportHistoryActiveIndex = -1;
        private ReportContol activeReport = null;
        private string _FullCustomPath;
        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        public ReportsView()
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            InitializeComponent();
            ShowGettingStarted();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ReportsView : {0}", stopWatch.ElapsedMilliseconds);
        }

        public ReportTypes CurrentView
        {
            get { return currentView; } 
        }

        public ReportContol ActiveReport
        {
            get { return activeReport; }
        }

        public override void ShowHelp()
        {
            if (contentPanel.Controls.Count != 0 && !(contentPanel.Controls[0] is ReportContol))
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ReportsView);
            }
            else
            {
                activeReport.ShowHelp();
            }
        }

        public override void RefreshView()
        {
            // Do nothing.  This blocks the normal refresh cycle used by other views.
        }
        public string FullCustomPath
        {
            get { return _FullCustomPath; }
        }

        public void ShowGettingStarted()
        {
            using (Log.DebugCall())
            {
                _FullCustomPath = null;
                if (gettingStartedControl == null)
                {
                    gettingStartedControl = new GettingStartedControl();
                }

                ShowView(ReportTypes.GettingStarted, gettingStartedControl);
            }
        }

        private void ShowReport(ReportTypes reportType, ReportContol reportControl, DrillthroughEventArgs drillThroughArguments, bool suppressInitReport)
        {
            if (reportControl != null)
            {
                reportControl.DrillThroughArguments = drillThroughArguments;
                reportControl.CanCancelChanged += reportControl_CanCancelChanged;

                if (!suppressInitReport)
                {
                    reportControl.InitReport();
                }

                activeReport = reportControl;

                ShowView(reportType, reportControl);
            }
        }

        private void reportControl_CanCancelChanged(object sender, EventArgs e)
        {
            if (sender is ReportContol)
            {
                cancelButton.Enabled = ((ReportContol) sender).CanCancel;
            }
        }

        private void ShowView(ReportTypes viewType, Control view)
        {
            if (view != null)
            {
                view.Dock = DockStyle.Fill;
                contentPanel.Controls.Clear();
                contentPanel.Controls.Add(view);
            }

            if (viewType == ReportTypes.Custom && view != null)
            {
                string reportDescription = ((ReportContol) view).CustomReportName;
                titleLabel.Text = reportDescription != String.Empty
                                      ? string.Format(TitleLabelText, reportDescription)
                                      : "Reports";
            }
            else
            {
                UpdateTitle(viewType);
            }
            
            currentView = viewType;
            UpdateReportTools();
        }

        public void ShowReport(ReportTypes report, DrillthroughEventArgs drillThroughArguments, string fullPath, int? defaultServerID)
        {
            using (Log.DebugCall())
            {
                ReportContol reportControl = null;
                
                _FullCustomPath = fullPath;

                switch (report)
                {
                    case ReportTypes.Custom:
                        reportControl = new CustomReport(fullPath, defaultServerID);
                        break;
                    case ReportTypes.GettingStarted:
                        ShowGettingStarted();
                        break;
                    case ReportTypes.TopServers:
                        reportControl = new TopServersReport();
                        break;
                    //START: SQLDM-29252 10.4.1 Feature request for Uptime Report
                    case ReportTypes.ServerUptime:
                        reportControl = new ServerUptime();
                        break;
                    //END:  SQLDM-29252 10.4.1 Feature request for Uptime Report
                    case ReportTypes.DiskSpaceForecast:
                        reportControl = new DiskUsageForecast();
                        break;
                    case ReportTypes.DatabaseGrowthForecast:
                        reportControl = new DatabaseGrowthForecast();
                        break;
                    case ReportTypes.QueryOverview:
                        reportControl = new QueryOverview();
                        break;
                    case ReportTypes.TopQueries:
                        reportControl = new TopQueries();
                        break;
                    case ReportTypes.QueryStatistics:
                        reportControl = new QueryStatistics();
                        break;
                    case ReportTypes.TopDatabases:
                        reportControl = new TopDatabases();
                        break;
                    case ReportTypes.TopDatabaseApps:
                        reportControl = new TopDatabaseApplications();
                        break;
                    case ReportTypes.DatabaseStatistics:
                        reportControl = new DatabaseStatistics();
                        break;
                    case ReportTypes.TopTablesGrowth:
                        reportControl = new TopTablesGrowth();
                        break;
                    case ReportTypes.TopTableFrag:
                        reportControl = new TopTablesFragmentation();
                        break;
                    case ReportTypes.TableGrowthForecast:
                        reportControl = new TableGrowthForecast();
                        break;
                    case ReportTypes.EnterpriseSummary:
                        reportControl = new EnterpriseSummary();
                        break;
                    case ReportTypes.ServerSummary:
                        reportControl = new ServerSummary();
                        break;
                    //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                    case ReportTypes.DeadlockReport:
                        reportControl = new DeadlockReport();
                        break;
                    //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
                    case ReportTypes.ServerStatistics:
                        reportControl = new ServerStatistics();
                        break;
                    case ReportTypes.MirroringSummary:
                        reportControl = new MirroringSummary();
                        break;
                    case ReportTypes.MirroringHistory:
                        reportControl = new MirroringHistory();
                        break;
                    case ReportTypes.ServerInventory:
                        reportControl = new ServerInventory();
                        break;
                    case ReportTypes.AlertHistory:
                        reportControl = new AlertHistory();
                        break;
                    case ReportTypes.ActiveAlerts:
                        reportControl = new ActiveAlerts();
                        break;
                    case ReportTypes.MemorySummary:
                        reportControl = new MemorySummary();
                        break;
                    case ReportTypes.DiskSummary:
                        reportControl = new DiskSummary();
                        break;
                    case ReportTypes.CPUSummary:
                        reportControl = new CPUSummary();
                        break;
                    //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                    case ReportTypes.DetailedSessionReport:
                        reportControl = new DetailedSessionReport();
                        break;
                    //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
                    case ReportTypes.SessionsSummary:
                        reportControl = new SessionsSummary();
                        break;
                    case ReportTypes.ReplicationSummary:
                        reportControl = new ReplicationSummary();
                        break;
                   case ReportTypes.MetricThresholds:
                        reportControl = new MetricThresholds();
                        break;
                   case ReportTypes.DiskDetails:
                        reportControl = new DiskDetails();
                        break;
                   case ReportTypes.VirtualizationSummary:
                        reportControl = new VirtualizationSummary();
                        break;
                   case ReportTypes.TempdbStatistics:
                        reportControl = new TempdbStatistics();
                        break;
                    case ReportTypes.VirtualizationStatistics:
                        reportControl = new VirtualizationStatistics();
                        break;
                    case ReportTypes.TransactionLogStatistics:
                        reportControl = new TransactionLogStatistics();
                        break;
                    case ReportTypes.BaselineStatistics:
                        reportControl = new BaselineStatistics();
                        break;
                    case ReportTypes.ChangeLogSummary:
                        reportControl = new ChangeLogSummary();
                        break;
                    case ReportTypes.AlwaysOnStatistics:
                        reportControl = new AlwaysOnStatistics();
                        break;
                    case ReportTypes.AlwaysOnTopology:
                        reportControl = new AlwaysOnTopology();
                        break;
                    //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
                    case ReportTypes.QueryWaitStatistics:
                        reportControl = new QueryWaitStatistics();
                        break;
                    //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
                    //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
                    case ReportTypes.DiskSpaceUsage:
                        reportControl = new DiskSpaceUsage();
                        break;
                    case ReportTypes.DiskSpaceHistory:
                        reportControl = new DiskSpaceHistory();
                        break;
                    //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
                    case ReportTypes.AlertTemplateReport:
                        reportControl = new AlertTemplates();
                        break;
                    case ReportTypes.TemplateComparison:
                        reportControl = new TemplateComparison();
                        break;
                    case ReportTypes.AlertThreshold:
                        reportControl = new AlertThresholds();
                        break;
                }

                if (reportControl != null)
                {
                    ShowReport(report, reportControl, drillThroughArguments, false);
                    AddReportHistoryItem(report, reportControl, drillThroughArguments != null);
                }
            }
        }

        private void AddReportHistoryItem(ReportTypes reportType, ReportContol reportControl, bool removeForwardHistory)
        {
            string reportDescription = ReportsHelper.GetReportTitle(reportType);

            if (reportControl != null && reportDescription != String.Empty)
            {
                ToolStripMenuItem historyItem = new ToolStripMenuItem(reportDescription);
                historyItem.Tag = new Pair<ReportTypes, ReportContol>(reportType, reportControl);
                historyItem.Click += historyItem_Click;
                historyItem.Image = Resources.SelectedItem16x16;
                List<ToolStripItem> itemsToRemove = new List<ToolStripItem>();

                foreach (ToolStripItem item in backDropDownButton.DropDownItems)
                {
                    if (item.Image != null)
                    {
                        removeForwardHistory = false;
                    }
                    else if (removeForwardHistory)
                    {
                        itemsToRemove.Add(item);
                    }

                    item.Image = null;
                }

                foreach (ToolStripItem itemToRemove in itemsToRemove)
                {
                    backDropDownButton.DropDownItems.Remove(itemToRemove);
                }

                backDropDownButton.DropDownItems.Insert(0, historyItem);

                if (backDropDownButton.DropDownItems.Count > ReportHistoryLimit)
                {
                    int removeIndex = backDropDownButton.DropDownItems.Count - 1;
                    Pair<ReportTypes, ReportContol> itemTag =
                        (Pair<ReportTypes, ReportContol>) backDropDownButton.DropDownItems[removeIndex].Tag;
                    itemTag.Second.CanCancelChanged -= reportControl_CanCancelChanged;
                    itemTag.Second.Dispose();
                    backDropDownButton.DropDownItems[removeIndex].Click -= historyItem_Click;
                    backDropDownButton.DropDownItems.RemoveAt(removeIndex);
                }

                backDropDownButton.Enabled = backDropDownButton.DropDownItems.Count > 1;
                forwardButton.Enabled = false;
                reportHistoryActiveIndex = 0;
            }
        }

        private void historyItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem historyItem = (ToolStripMenuItem)sender;
                int index = backDropDownButton.DropDownItems.IndexOf(historyItem);

                if (index != -1)
                {
                    ShowReportHistoryItem(index);
                    reportHistoryActiveIndex = index;
                }
            }
        }

        private void UpdateTitle(ReportTypes report)
        {
            string reportDescription = ReportsHelper.GetReportTitle(report);
            titleLabel.Text = reportDescription != String.Empty
                                  ? string.Format(TitleLabelText, reportDescription)
                                  : "Reports";
        }

        private void UpdateReportTools()
        {
            if (contentPanel.Controls.Count == 1 &&
                contentPanel.Controls[0] is ReportContol)
            {
                ReportContol reportControl = (ReportContol)contentPanel.Controls[0];
                toggleFilterPanelButton.Text = reportControl.FiltersVisible ? "Hide Filters" : "Show Filters";
                toggleFilterPanelButton.Image = reportControl.FiltersVisible
                                                    ? Resources.OptionsGlyphUp16x16
                                                    : Resources.OptionsGlyph;
                reportToolsPanel.Visible = true;
            }
            else
            {
                reportToolsPanel.Visible = false;
            }
        }

        private void toggleFilterPanelButton_Click(object sender, EventArgs e)
        {
            if (contentPanel.Controls.Count == 1 &&
                contentPanel.Controls[0] is ReportContol)
            {
                ReportContol reportControl = (ReportContol)contentPanel.Controls[0];
                reportControl.FiltersVisible = !reportControl.FiltersVisible;
                UpdateReportTools();
            }
        }

        private void gettingStartedButton_Click(object sender, EventArgs e)
        {
            ApplicationController.Default.ShowReportsView(ReportTypes.GettingStarted);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshReport();
        }

        private void RefreshReport()
        {
            if (contentPanel.Controls.Count == 1 &&
                contentPanel.Controls[0] is ReportContol)
            {
                ReportContol reportControl = (ReportContol)contentPanel.Controls[0];
                reportControl.RunReport();
            }
        }

        private void backDropDownButton_ButtonClick(object sender, EventArgs e)
        {
            if (reportHistoryActiveIndex < backDropDownButton.DropDownItems.Count - 1)
            {
                ShowReportHistoryItem(++reportHistoryActiveIndex);
            }
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            if (reportHistoryActiveIndex > 0)
            {
                ShowReportHistoryItem(--reportHistoryActiveIndex);
            }
        }

        private void ShowReportHistoryItem(int index)
        {
            if (backDropDownButton.DropDownItems[index].Tag is Pair<ReportTypes, ReportContol>)
            {
                Pair<ReportTypes, ReportContol> historyItem =
                    (Pair<ReportTypes, ReportContol>)backDropDownButton.DropDownItems[index].Tag;
                ShowReport(historyItem.First, historyItem.Second, null, true);
                backDropDownButton.Enabled = index != backDropDownButton.DropDownItems.Count - 1;
                forwardButton.Enabled = index != 0;
                foreach (ToolStripItem item in backDropDownButton.DropDownItems)
                {
                    item.Image = null;
                }
                backDropDownButton.DropDownItems[index].Image = Resources.SelectedItem16x16;
                ApplicationController.Default.OnReportsViewChanged(new ReportsViewChangedEventArgs(historyItem.First));
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (contentPanel.Controls.Count == 1 &&
                contentPanel.Controls[0] is ReportContol)
            {
                ReportContol reportControl = (ReportContol)contentPanel.Controls[0];
                reportControl.CancelReportRefresh();
            }
        }

        private void resetFiltersButton_Click(object sender, EventArgs e)
        {
            if (contentPanel.Controls.Count == 1 &&
                contentPanel.Controls[0] is ReportContol)
            {
                ReportContol reportControl = (ReportContol)contentPanel.Controls[0];
                reportControl.ResetFilterCriteria();
            }
        }
    }

   internal static class ReportsHelper
   {
      public static string GetReportTitle(ReportTypes reportType)
      {
         FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
         TitleAttribute[] attributes =
             (TitleAttribute[])fieldInfo.GetCustomAttributes(typeof(TitleAttribute), false);
         return attributes.Length > 0 ? attributes[0].Title : String.Empty;
      }

      public static string GetReportShortDescription(ReportTypes reportType)
      {
         FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
         ShortDescriptionAttribute[] attributes =
             (ShortDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(ShortDescriptionAttribute), false);
         return attributes.Length > 0 ? attributes[0].Description : String.Empty;
      }

      public static string GetReportLongDescription(ReportTypes reportType)
      {
         FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
         LongDescriptionAttribute[] attributes =
             (LongDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(LongDescriptionAttribute), false);
         return attributes.Length > 0 ? attributes[0].Description : String.Empty;
      }

      public static bool IsReportDeployable(ReportTypes reportType)
      {
         FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
         DeployableAttribute[] attributes =
             (DeployableAttribute[])fieldInfo.GetCustomAttributes(typeof(DeployableAttribute), false);
         return attributes.Length > 0 ? attributes[0].Deployable : false;
      }

      public static bool IsCustomReport(ReportTypes reportType)
      {
         FieldInfo fieldInfo = typeof(ReportTypes).GetField(reportType.ToString());
         CustomReportAttribute[] attributes =
             (CustomReportAttribute[])fieldInfo.GetCustomAttributes(typeof(CustomReportAttribute), false);
         return attributes.Length > 0 ? attributes[0].CustomReport : false;
      }

      public static void ShowReportHelp(ReportTypes reportType)
      {
         string helpTopic = String.Empty;

         switch (reportType)
         {
            case ReportTypes.GettingStarted:
               helpTopic = HelpTopics.ReportsView;
               break;
            case ReportTypes.TopServers:
               helpTopic = HelpTopics.ReportsTopServers;
               break;
            case ReportTypes.DiskSpaceForecast:
               helpTopic = HelpTopics.ReportsDiskSpaceUsageForecast;
               break;
            case ReportTypes.DatabaseGrowthForecast:
               helpTopic = HelpTopics.ReportsDatabaseGrowthForecast;
               break;
            case ReportTypes.QueryOverview:
               helpTopic = HelpTopics.ReportsQueryOverview;
               break;
            case ReportTypes.TopQueries:
               helpTopic = HelpTopics.ReportsTopQueries;
               break;
            case ReportTypes.QueryStatistics:
               helpTopic = HelpTopics.ReportsQueryStatistics;
               break;
            case ReportTypes.TopDatabases:
               helpTopic = HelpTopics.ReportsTopDatabases;
               break;
            case ReportTypes.TopDatabaseApps:
               helpTopic = HelpTopics.ReportsTopDatabaseApplications;
               break;
            case ReportTypes.DatabaseStatistics:
               helpTopic = HelpTopics.ReportsDatabaseStatistics;
               break;
            case ReportTypes.TopTablesGrowth:
               helpTopic = HelpTopics.ReportsTopTablesByGrowth;
               break;
            case ReportTypes.TopTableFrag:
               helpTopic = HelpTopics.ReportsTopTablesByFragmentation;
               break;
            case ReportTypes.TableGrowthForecast:
               helpTopic = HelpTopics.ReportsTableGrowthForecast;
               break;
            case ReportTypes.EnterpriseSummary:
               helpTopic = HelpTopics.ReportsEnterpriseSummary;
               break;
            case ReportTypes.ServerSummary:
               helpTopic = HelpTopics.ReportsServerSummary;
               break;
            //START: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report
            case ReportTypes.DeadlockReport:
               // TODO: Help topic needs to be added for this deadlock report feature
               helpTopic = HelpTopics.ReportsDatabaseStatistics;
               break;
            //END: SQLdm-4789 10.2 (Varun Chopra) -Customer Enhancement Request for Deadlock report        
            case ReportTypes.ServerStatistics:
               helpTopic = HelpTopics.ReportsServerStatistics;
               break;
            case ReportTypes.MirroringSummary:
               helpTopic = HelpTopics.ReportsDatabaseMirroringSummary;
               break;
            case ReportTypes.MirroringHistory:
               helpTopic = HelpTopics.ReportsDatabaseMirroringHistory;
               break;
            case ReportTypes.ServerInventory:
               helpTopic = HelpTopics.ReportsServerInventory;
               break;
            case ReportTypes.AlertHistory:
               helpTopic = HelpTopics.ReportsAlertHistory;
               break;
            case ReportTypes.ActiveAlerts:
               helpTopic = HelpTopics.ReportsActiveAlerts;
               break;
            case ReportTypes.MemorySummary:
               helpTopic = HelpTopics.ReportsMemoryStatistics;
               break;
            case ReportTypes.DiskSummary:
               helpTopic = HelpTopics.ReportsDiskStatistics;
               break;
            case ReportTypes.CPUSummary:
               helpTopic = HelpTopics.ReportsCPUStatistics;
               break;
            case ReportTypes.SessionsSummary:
               helpTopic = HelpTopics.ReportsSessionStatistics;
               break;
            //START: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
            case ReportTypes.DetailedSessionReport:
                // TODO: Help topic needs to be added for this Detailed Session report feature
                helpTopic = HelpTopics.ReportsSessionStatistics;
                break;
            //END: SQLdm-26953 10.2.1 (Varun Chopra) -Enhancement Request for Detailed Session Information report
            case ReportTypes.ReplicationSummary:
               helpTopic = HelpTopics.ReportsReplicationStatistics;
               break;
            case ReportTypes.Custom:
               helpTopic = HelpTopics.ReportsCustom;
               break;
            case ReportTypes.MetricThresholds:
               helpTopic = HelpTopics.ReportsMetricThreshold;
               break;
            case ReportTypes.DiskDetails:
               helpTopic = HelpTopics.ReportsDiskDetails;
               break;
            case ReportTypes.TempdbStatistics:
               helpTopic = HelpTopics.ReportsTempdbStatistics;
               break;
            case ReportTypes.VirtualizationStatistics:
               helpTopic = HelpTopics.ReportsVirtualizationStatistics;
               break;
            case ReportTypes.VirtualizationSummary:
               helpTopic = HelpTopics.ReportsVirtualizationSummary;
               break;
            case ReportTypes.TransactionLogStatistics:
               helpTopic = HelpTopics.ReportsTransactionLogStatistics;
               break;
             case ReportTypes.BaselineStatistics:
               helpTopic = HelpTopics.ReportsBaselineStatistics;
               break;
             case ReportTypes.ChangeLogSummary:
               helpTopic = HelpTopics.ReportsChangeLogSummary;
               break;
             case ReportTypes.AlwaysOnStatistics:
               helpTopic = HelpTopics.ReportsAlwaysOnStatistics;
               break;
             case ReportTypes.AlwaysOnTopology:
               helpTopic = HelpTopics.ReportsAlwaysOnTopology;
               break;
             //[START] SQLdm9.0 (Gaurav Karwal): Adding help topic for query wait reports
             case ReportTypes.QueryWaitStatistics:
               helpTopic = HelpTopics.ReportsQueryWaitStatistics;
               break;
             //[END] SQLdm9.0 (Gaurav Karwal): Adding help topic for query wait reports
             //START: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
             case ReportTypes.DiskSpaceUsage:
               helpTopic = HelpTopics.ReportDiskSpaceUsage;
               break;
             case ReportTypes.DiskSpaceHistory:
               helpTopic = HelpTopics.ReportDiskSpaceHistory;
               break;
                //END: SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Added for Disk Space Usage and Disk Space History Reports
                case ReportTypes.AlertTemplateReport:
                    helpTopic = HelpTopics.ReportAlertTemplate;
                    break;
                case ReportTypes.TemplateComparison:
                    helpTopic = HelpTopics.ReportTemplateComparison;
                    break;
                case ReportTypes.AlertThreshold:
                    helpTopic = HelpTopics.ReportAlertThreshold;
                    break;

         }

         if (helpTopic != String.Empty)
         {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(helpTopic);
         }
      }
   }

   [AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal class TitleAttribute : Attribute
    {
        public readonly string Title;

        public TitleAttribute(string title)
        {
            Title = title;
        }
    }

    [AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal class ShortDescriptionAttribute : Attribute
    {
        public readonly string Description;

        public ShortDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal class LongDescriptionAttribute : Attribute
    {
        public readonly string Description;

        public LongDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
    
    [AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal class DeployableAttribute : Attribute
    {
        public readonly bool Deployable = false;

        public DeployableAttribute(bool deployable)
        {
            Deployable = deployable;
        }
    }

    [AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    internal class CustomReportAttribute : Attribute
    {
        public readonly bool CustomReport = false;

        public CustomReportAttribute(bool isCustom)
        {
            CustomReport = isCustom;
        }
    }
}
