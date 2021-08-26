using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.Reporting.WinForms;
using Infragistics.Win;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Dialogs;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    internal partial class ReportControl : UserControl {
        private static readonly Logger Log = Logger.GetLogger("ReportControl");

        // Class to contain query parameters, results and other objects associated
        // with the execution of a report.  This is passed into
        // and out of the background worker thread(s) for getting report data.
        // It is also used for cancelling a running report.
        private class WorkerData {
            public bool cancelled;
            public BackgroundWorker bgWorker;   // BackgroundWorker used for the report.
            public ReportViewer reportViewer;   // Viewer used to view the report.
            public ReportTypes reportType;     // Type of report to generate.
            public PeriodType periodType;       // The period option the user selected for the report
            public string periodDescription;    // Period string to show on the report.
            public List<DateRangeOffset> dateRanges;    // Date ranges to pass to the stored proc(s).
            public SampleSize sampleSize;   // Sample size parameter to pass to the stored proc(s).
            public DataTable table1;// DataTable returned by a stored proc.
            public DataTable table2;// DataTable returned by a stored proc.
            public double Y1Min;    // Min Y value for a special chart in a report.
            public double Y1Max;    // Max Y value for a special chart in a report.
            public double Y2Min;    // Min Y value for a special chart in a report.
            public double Y2Max;    // Max Y value for a special chart in a report.
            public DateTime X1Min;
            public DateTime X1Max;

            public void Cancel() {
                cancelled = true;

                if (bgWorker != null)
                    bgWorker.CancelAsync();

                if (reportViewer != null)
                    reportViewer.CancelRendering(0);
            }
        }

        private WorkerData _reportData;
        private bool _firstReport = true;
        private ValueListItem _prevPeriod; // Previously selected period in case user cancels custom period dialog.
        private ReportTypes _currentReport; // Currently selected report
        private List<DateRangeOffset> _customDates; // Currently specified custom dates.
        private WorstPerformingFilter _worstPerformingFilter = new WorstPerformingFilter();
        private bool _isExpanded;

        #region Constants

        // These constants are the names of Stored Procs.
        private const string _spOverview = "p_GetOverviewReport";
        private const string _spProcessor = "p_GetProcessorReport";
        private const string _spDisk = "p_GetDiskReport";
        private const string _spMemory = "p_GetMemoryReport";
        private const string _spSessions = "p_GetSessionsReport";
        private const string _spDBStats = "p_GetDatabaseStatisticsReport";
        private const string _spDBGrowth = "p_GetDatabaseGrowthReport";
        private const string _spTableGrowth = "p_GetTableGrowthReport";
        private const string _spTableReorg = "p_GetTableReorgReport";

        private enum PeriodType { Today, Last7, Last30, Last365, Custom };

        // These values appear in the periodCombo drop-down.
        readonly ValueListItem _periodToday = new ValueListItem(PeriodType.Today, "Today");
        readonly ValueListItem _period7 = new ValueListItem(PeriodType.Last7, "Last 7 Days");
        readonly ValueListItem _period30 = new ValueListItem(PeriodType.Last30, "Last 30 Days");
        readonly ValueListItem _period365 = new ValueListItem(PeriodType.Last365, "Last 365 Days");
        readonly ValueListItem _periodSetCustom = new ValueListItem(null, "Set Custom...");
        ValueListItem _periodCustom; // Contains the custom date string once it is set.

        private enum SampleSize { Minutes, Hours, Days, Months, Years };

        // These values appear in the sampleSizeCombo drop-down.
        readonly ValueListItem _sampleMinutes = new ValueListItem(SampleSize.Minutes, "Minutes");
        readonly ValueListItem _sampleHours = new ValueListItem(SampleSize.Hours, "Hours");
        readonly ValueListItem _sampleDays = new ValueListItem(SampleSize.Days, "Days");
        readonly ValueListItem _sampleMonths = new ValueListItem(SampleSize.Months, "Months");
        readonly ValueListItem _sampleYears = new ValueListItem(SampleSize.Years, "Years");

        // This value is the prefix for the custom date string.
        private const string _customPrefix = "Custom ";

        #endregion

        #region Enums

        // See file StateDiagram.docx for a diagram of the state transitions.
        private enum UIState
        {
            ParmsNeeded,
            GettingData,
            NoDataAcquired,
            NoTableDataAcquired,
            Rendering,
            Rendered,
            Cancelled,
            ReportError
        };

        #endregion

        #region Ctors

        public ReportControl() {
            using (Log.DebugCall()) {
                InitializeComponent();

                SetTextboxBackground();

                InitPeriodCombo();

                State = UIState.ParmsNeeded;

                splitContainer.Panel1Collapsed = !Settings.Default.ReportsViewFilterOptionsPanelVisible;
                Settings.Default.SettingChanging += Settings_SettingChanging;
                filterOptionsPanel.AutoSize = true;

                //reports always start all collapsed
                _isExpanded = false;
            }
        }

        #endregion

        private void InitPeriodCombo() {
            periodCombo.Items.Add(_periodToday);
            periodCombo.Items.Add(_period7);
            periodCombo.Items.Add(_period30);
            periodCombo.Items.Add(_period365);
            periodCombo.Items.Add(_periodSetCustom);
            periodCombo.SelectedItem = _periodToday;
        }

        private UIState State {
            set {
                Log.Debug("Setting state to ", value);

                reportViewer.Visible = value == UIState.Rendering || value == UIState.Rendered;
                reportDescriptionControl.Visible = value == UIState.ParmsNeeded;
                waitForDataLabel.Visible = value == UIState.GettingData;
                noDataLabel.Visible = value == UIState.NoDataAcquired;
                noDataForTableReport.Visible = value == UIState.NoTableDataAcquired;
                reportViewerErrorLabel.Visible = value == UIState.ReportError;
                cancelledLabel.Visible = value == UIState.Cancelled;
                UseWaitCursor = value == UIState.GettingData;

                switch (value) {
                    case UIState.GettingData:
                    case UIState.Rendering:
                        viewReportButton.Enabled = false;
                        expand_collapseButton.Enabled = false;
                        filterOptionsPanel.Enabled = false;
                        showPointLabels.Enabled = false;
                        cancelReport.Visible = true;
                        cancelReport.Cursor = Cursors.Arrow;
                        cancelReport.UseWaitCursor = false;
                        break;
                    case UIState.NoDataAcquired:
                    case UIState.NoTableDataAcquired:
                        viewReportButton.Enabled = true;
                        expand_collapseButton.Enabled = false;
                        filterOptionsPanel.Enabled = true;
                        showPointLabels.Enabled = true;
                        cancelReport.Visible = false;
                        break;
                    default:
                        viewReportButton.Enabled = value != UIState.ParmsNeeded;
                        expand_collapseButton.Enabled = value != UIState.ParmsNeeded;
                        filterOptionsPanel.Enabled = true;
                        showPointLabels.Enabled = true;
                        cancelReport.Visible = false;
                        break;
                }

                clearFilter.Enabled = !cancelReport.Visible;
            }
        }

        #region Servers

        // Combo box for single server select
        private void InitInstanceCombo() {
            instanceCombo.Items.Clear();

            //last parameter is false so that we get all servers, not just the active ones
            ValueListItem[] instances = new ValueListItem[ApplicationModel.Default.AllInstances.Count];
            int i = 0;
            foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
            {
                instances[i++] = new ValueListItem(server, server.InstanceName);
            }

            instanceCombo.Items.AddRange(instances);

            //now re-select the one they had selected, if they did have one
            if (instanceCombo.SelectedItem == null && _curServer != null) {
                instanceCombo.SelectedIndex = instanceCombo.FindStringExact(_curServer.InstanceName);
            }
        }

        // Init the combo box on each dropdown so that it'll reflect added/removed servers
        private void instanceCombo_BeforeDropDown(object sender, CancelEventArgs e) {
            InitInstanceCombo();
        }
        
        // Currently selected single server.
        private MonitoredSqlServer _curServer; // Current single single selection of servers.
        private MonitoredSqlServer SingleServer {
            get {
                if (instanceCombo.SelectedItem == null && _curServer != null) {
                    instanceCombo.SelectedIndex = instanceCombo.FindStringExact(_curServer.InstanceName);
                }

                return _curServer;
            }
            set {
                _curServer = value;

                if (_curServer == null) {
                    instanceCombo.SelectedItem = null;
                } else {
                    instanceCombo.SelectedIndex = instanceCombo.FindStringExact(_curServer.InstanceName);
                }

                SingleDatabase = null;
                MultiDatabases = null;
            }
        }

        // Currently selected list of servers.
        private List<MonitoredSqlServer> _curServers;
        private List<MonitoredSqlServer> MultiServers {
            get { return _curServers; }
            set {
                if (!ListsAreEqual(_curServers, value)) {
                    _curServers = value;
                    MakeCSVList(serversTextBox, _curServers);
                }
            }
        }

        // Combo box of Servers for single select
        private void instanceCombo_ValueChanged(object sender, EventArgs e)
        {
            if (instanceCombo.SelectedItem == null) return;
            
            SingleServer = (MonitoredSqlServer)(instanceCombo.SelectedItem.DataValue);

            databaseButton.Enabled = true;
            databasesButton.Enabled = true;

            UpdateParameterControls();
        }

        // Show server selection dialog in multi-select mode.
        private void serversButton_Click(object sender, EventArgs e)
        {
            ReportServersDialog dlg = new ReportServersDialog("Select Servers", "Select one or more servers for the report.");
            dlg.SingleSelect = false;
            dlg.SelectedServers = MultiServers;
            dlg.ActiveServersOnly = false; //get all the servers for reports

            if (DialogResult.OK == dlg.ShowDialog(this)) {
                MultiServers = dlg.SelectedServers;
            }

            UpdateParameterControls();
        }

        private string GetServerList()
        {
            StringBuilder builder = new StringBuilder(100);
            if (MultiServers != null) foreach (MonitoredSqlServer server in MultiServers) {
                if (server == MultiServers[0]) {
                    builder.Append(server.InstanceName);
                } else {
                    builder.AppendFormat(", {0}", server.InstanceName);
                }
            }

            return builder.ToString();
        }

        private int[] GetServerIds(List<MonitoredSqlServer> servers)
        {
            int[] serverIds = new int[servers.Count];
            int ndx = 0;
            foreach (MonitoredSqlServer server in servers) {
                serverIds[ndx] = server.Id;
                ++ndx;
            }

            return serverIds;
        }

        #endregion

        #region DBs

        // Currently selected single database.
        private string SingleDatabase {
            get { return databaseTextBox.Text; }
            set {
                if (databaseTextBox.Text != value) {
                    databaseTextBox.Text = value;

                    // Clear the table list when the DB changes.
                    MultiTables = null;
                }
            }
        }

        // Currently selected list of databases.
        private List<string> _curDbs;
        private List<string> MultiDatabases {
            get { return _curDbs; }
            set {
                if (!ListsAreEqual(_curDbs, value)) {
                    _curDbs = value;
                    MakeCSVList(databasesTextBox, _curDbs);
                }
            }
        }

        // Show DatabaseBrowserDialog in multi-select mode.
        private void databasesButton_Click(object sender, EventArgs e)
        {
            DatabaseBrowserDialog dlg = new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, SingleServer.Id, SingleServer.InstanceName, true, true, "Check one or more databases for the report.", "Select Databases");
            dlg.CheckedDatabases = MultiDatabases;

            if (DialogResult.OK == dlg.ShowDialog(FindForm())) {
                MultiDatabases = dlg.CheckedDatabases;
            }

            UpdateParameterControls();
        }

        // Show DatabaseBrowserDialog in single-select mode.
        private void databaseButton_Click(object sender, EventArgs e)
        {
            DatabaseBrowserDialog dlg = new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, SingleServer.Id, SingleServer.InstanceName, false, true, "Choose one database for the report.", "Select Database");
            dlg.SelectedDatabase = SingleDatabase;

            if (DialogResult.OK == dlg.ShowDialog(FindForm())) {
                SingleDatabase = dlg.SelectedDatabase;
                tablesButton.Enabled = true;
            }

            UpdateParameterControls();
        }

        #endregion

        #region Tables

        // Currently selected list of tables.
        // Each triple has a schema, table name, and bool indicating if system table.
        private List<Triple<string, string, bool>> _curTables; // Current multi-selection of tables.
        private List<Triple<string, string, bool>> MultiTables {
            get { return _curTables; }
            set {
                if (!ListsAreEqual(_curTables, value)) {
                    _curTables = value;
                    MakeCSVList(tablesTextBox, MakeTableNameArray(_curTables, true));
                }
            }
        }

        private static string[] MakeTableNameArray(List<Triple<string, string, bool>> list, bool withSchema) {
            string[] returns = null;

            if (list != null && list.Count != 0) {
                returns = new string[list.Count];
                int ndx = 0;
                foreach (Triple<string, string, bool> triple in list) {
                    if (withSchema) {
                        returns[ndx] = triple.First + "." + triple.Second;
                    } else {
                        returns[ndx] = triple.Second;
                    }
                    ++ndx;
                }
            }

            return returns;
        }

        // Show table selection dialog
        private void tablesButton_Click(object sender, EventArgs e) {
            TableBrowserDialog dlg = new TableBrowserDialog(SingleServer.Id, SingleDatabase);
            dlg.CheckedTables = MultiTables;
            
            if (DialogResult.OK == dlg.ShowDialog(this)) {
                MultiTables = dlg.CheckedTables;
            }

            UpdateParameterControls();
        }

        private string GetTableList() {
            if (MultiTables == null) return string.Empty; //bail out if nothing to do
            
            StringBuilder builder = new StringBuilder(100);
            
            foreach (Triple<string, string, bool> table in MultiTables) {
                if (table == MultiTables[0]) {
                    builder.Append(table.Second); //the table name
                } else {
                    builder.AppendFormat(", {0}", table.Second);
                }
            }

            return builder.ToString();
        }

        #endregion

        #region List Comparison

        private static bool ListsAreEqual(List<Triple<string, string, bool>> l1, List<Triple<string, string, bool>> l2) {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            return Algorithms.EqualCollections(l1, l2);
        }

        private static bool ListsAreEqual(List<MonitoredSqlServer> l1, List<MonitoredSqlServer> l2) {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            for (int i = 0; i < l1.Count; ++i) {
                if (l1[i].Id != l2[i].Id) return false;
            }

            return true;
        }

        private static bool ListsAreEqual(List<string> l1, List<string> l2) {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            for (int i = 0; i < l1.Count; ++i) {
                if (l1[i] != l2[i]) return false;
            }

            return true;
        }

        #endregion

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e) {
            if (e.SettingName == "ReportsViewFilterOptionsPanelVisible") {
                splitContainer.Panel1Collapsed = !(bool)e.NewValue;
                if (!splitContainer.Panel1Collapsed) {
                    AdjustParametersPaneHeight();
                }
            }
        }

        private void UpdateReportDescriptionControl(ReportTypes report) {
            switch (report) {
                case ReportTypes.ActiveAlerts:
                    reportDescriptionControl.TitleText = "Server Overview";
                    reportDescriptionControl.DescriptionText = 
                        "The Server Overview report provides a summary of key performance metrics covering " +
                        "all areas of your SQL Server system. This report can be viewed for a single server " +
                        "or across multiple servers for easy comparison.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Servers button and select one or more SQL Server instances to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     5.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.AlertSummary:
                    reportDescriptionControl.TitleText = "Sessions";
                    reportDescriptionControl.DescriptionText =
                        "The Sessions report covers metrics associated with bottlenecks in connected " +
                        "user sessions, such as wait time and blocking.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Server button and select a SQL Server instance to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     5.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.DatabaseGrowthForecast:
                    reportDescriptionControl.TitleText = "CPU";
                    reportDescriptionControl.DescriptionText =
                        "The CPU report covers metrics associated with processor bottlenecks, such as CPU usage rates and queue lengths.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Server button and select a SQL Server instance to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     5.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.DatabaseStatistics:
                    reportDescriptionControl.TitleText = "Memory";
                    reportDescriptionControl.DescriptionText =
                        "The Memory report covers metrics associated with memory bottlenecks, such as memory usage and cache hit ratios.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Server button and select a SQL Server instance to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     5.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.DiskSpaceForecast:
                    reportDescriptionControl.TitleText = "Disk";
                    reportDescriptionControl.DescriptionText =
                        "The Disk report covers metrics associated with disk bottlenecks, such as disk activity and queue lengths.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Server button and select a SQL Server instance to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     5.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.EnterpriseSummary:
                    reportDescriptionControl.TitleText = "Database Overview";
                    reportDescriptionControl.DescriptionText =
                        "The Database Overview report provides a summary of key performance metrics for your databases. " +
                        "This report can be viewed for a single database or across multiple databases for easy comparison.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded.\r\n\r\n" +
                        "     2.  Select a target SQL Server instance.\r\n\r\n" +
                        "     3.  Select one or more databases you would like to analyze.\r\n\r\n" +
                        "     4.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     5.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     6.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.QueryOverview:
                    reportDescriptionControl.TitleText = "Database Growth";
                    reportDescriptionControl.DescriptionText =
                        "The Database Growth report provides information about current and historical database growth rates. " +
                        "This report can be used to analyze growth patterns and assist in future capacity planning.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded.\r\n\r\n" +
                        "     2.  Select a target SQL Server instance.\r\n\r\n" +
                        "     3.  Select one or more databases you would like to analyze.\r\n\r\n" +
                        "     4.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     5.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     6.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.QueryStatistics:
                    reportDescriptionControl.TitleText = "Table Growth";
                    reportDescriptionControl.DescriptionText =
                        "The Table Growth report provides information about current and historical table growth rates. " +
                        "This report can be used to analyze growth patterns and assist in future capacity planning.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded.\r\n\r\n" +
                        "     2.  Select a target SQL Server instance.\r\n\r\n" +
                        "     3.  Select a target database.\r\n\r\n" +
                        "     4.  Select one or more tables you would like to analyze.\r\n\r\n" +
                        "     5.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     6.  Choose the sample size that should be used to compact data over the selected period of time.\r\n\r\n" +
                        "     7.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.ServerInventory:
                    reportDescriptionControl.TitleText = "Worst Performing Queries";
                    reportDescriptionControl.DescriptionText =
                        "The Worst Performing Queries report provides information about poorly performing SQL statements, " +
                        "batches, and stored procedures executing on your servers.  This report helps you to identify which commands, " +
                        "users, and applications are consuming the most resources and running longest.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Servers button and select one or more SQL Server instances to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the event types to include in the report.\r\n\r\n" +
                        "     5.  Enter the minimum duration of the events to include.\r\n\r\n" +
                        "     6.  Specify the number of queries to report on per server.\r\n\r\n" +
                        "     7.  Click the Run Report button to generate your custom report.";
                    break;
                case ReportTypes.ServerStatistics:
                    reportDescriptionControl.TitleText = "Most Frequent Queries";
                    reportDescriptionControl.DescriptionText =
                        "The Most Frequent Queries report provides information about the most frequently executed SQL statements, " +
                        "batches, and stored procedures executing on your servers.  This report provides details about each command including " +
                        "duration and resource consumption, as well as which users and applications are executing them.";
                    reportDescriptionControl.GettingStartedText =
                        "Follow these steps to create a report:\r\n\r\n" +
                        "     1.  Expand the filter options panel at the top of this view if it is not already expanded by clicking the Show Filter Options link in the Navigation Pane.\r\n\r\n" +
                        "     2.  Click the Servers button and select one or more SQL Server instances to analyze.\r\n\r\n" +
                        "     3.  Choose the period of time the report should cover.\r\n\r\n" +
                        "     4.  Choose the event types to include in the report.\r\n\r\n" +
                        "     5.  Enter the minimum duration of the events to include.\r\n\r\n" +
                        "     6.  Specify the number of queries to report on per server.\r\n\r\n" +
                        "     7.  Click the Run Report button to generate your custom report.";
                    break;
            }
        }

        public void ShowReport(ReportTypes report) {
            using (Log.DebugCall()) {
                Log.Debug("New report is ", report);
                _currentReport = report;
                bool wasMultiServer = serversPanel.Visible;
                bool wasMultiDb = databasesPanel.Visible;

                UpdateReportDescriptionControl(report);
                ShowParameterControls(report);

                // Keep single and multi server text boxes sync'd
                if (wasMultiServer && !serversPanel.Visible) {
                    // Switching from multi to single server.  Carry over server name if
                    // only one is selected.
                    if (MultiServers != null && MultiServers.Count == 1) {
                        SingleServer = MultiServers[0];
                        InitInstanceCombo();
                    }
                } else if (!wasMultiServer && serversPanel.Visible) {
                    // Switching from single to multi server.
                    // Carry over the server name if the multi-select control has 0 or 1 servers.
                    if (SingleServer != null && (MultiServers == null || MultiServers.Count <= 1)) {
                        List<MonitoredSqlServer> newlist = new List<MonitoredSqlServer>();
                        newlist.Add(SingleServer);
                        MultiServers = newlist;
                    }
                }

                // Keep single and multi db text boxes sync'd
                if (wasMultiDb && databasePanel.Visible) {
                    // Switching from multi to single db.  Carry over db name if
                    // only one is selected.
                    if (MultiDatabases != null && MultiDatabases.Count == 1) {
                        SingleDatabase = MultiDatabases[0];
                    }
                } else if (!wasMultiDb && databasesPanel.Visible) {
                    // Switching from single to multi db. 
                    if (!string.IsNullOrEmpty(SingleDatabase) && (MultiDatabases == null || MultiDatabases.Count <= 1)) {
                        List<string> newlist = new List<string>();
                        newlist.Add(SingleDatabase);
                        MultiDatabases = newlist;
                    }
                }

                if (_reportData != null) {
                    // Cancel the active query.
                    _reportData.Cancel();
                }

                if (report == ReportTypes.TableGrowthForecast&& periodCombo.Items.Contains(_periodToday)) {
                    if (periodCombo.SelectedItem == _periodToday) {
                        periodCombo.SelectedItem = _period7;
                    }
                    periodCombo.Items.Remove(_periodToday);                    
                } else if (report != ReportTypes.TableGrowthForecast && !periodCombo.Items.Contains(_periodToday)) {
                    periodCombo.Items.Insert(0, _periodToday);
                }
                
                AdjustParametersPaneHeight();

                if (UpdateParameterControls()) {
                    // Automatically generate the report if enough parameters are available.
                    // PR 2010217 calls this "Inconsistent and inscrutable".
                    viewReportButton_Click(null, null);
                } else {
                    State = UIState.ParmsNeeded;
                }
            }
        }

        // Determine the ReportViewer's DisplayMode.  There is no DisplayMode
        // property, GetDisplayMode method, or DisplayModeChanged event, so
        // we do it by finding the printPreview button on the toolbar and
        // testing if it is checked or not.
        private static DisplayMode GetDisplayMode(ReportViewer viewer) {
            ToolStrip strip = GetToolStrip(viewer);

            if (strip != null) {
                foreach (ToolStripItem item in strip.Items) {
                    if (item.Name == "printPreview") {
                        ToolStripButton button = item as ToolStripButton;
                        if (button != null && button.Checked) {
                            return DisplayMode.PrintLayout;
                        } else {
                            return DisplayMode.Normal;
                        }
                    }
                }
            }

            return DisplayMode.Normal;
        }

        private static ToolStrip GetToolStrip(Control ctrl) {
            foreach (Control subctrl in ctrl.Controls) {
                if (subctrl is ToolStrip) {
                    return subctrl as ToolStrip;
                } else {
                    ToolStrip ts = GetToolStrip(subctrl);
                    if (ts != null) {
                        return ts;
                    }
                }
            }

            return null;
        }

        private void SetTextboxBackground() {
            serversTextBox.BackColor = filterOptionsPanel.BackColor;
            databasesTextBox.BackColor = filterOptionsPanel.BackColor;
            databaseTextBox.BackColor = filterOptionsPanel.BackColor;
            tablesTextBox.BackColor = filterOptionsPanel.BackColor;
        }

        // Show/hide controls in the parameters pane based on
        // the specified report.
        private void ShowParameterControls(ReportTypes report) {
            switch (report) {
                case ReportTypes.QueryOverview:
                case ReportTypes.QueryStatistics:
                case ReportTypes.TopQueries:
                    serversPanel.Visible = true;
                    serverPanel.Visible = false;
                    databasesPanel.Visible = false;
                    databasePanel.Visible = false;
                    tablesPanel.Visible = false;
                    periodPanel.Visible = true;
                    samplePanel.Visible = false;
                    queryPanel.Visible = true;
                    if (minDuration.Text.Trim().Length == 0 || Convert.ToInt32(minDuration.Text) < 0)
                        minDuration.Text = "0";
                    break;
                case ReportTypes.ServerSummary:
                    serversPanel.SendToBack();
                    serversPanel.Visible = true;
                    serverPanel.Visible = false;
                    databasesPanel.Visible = false;
                    databasePanel.Visible = false;
                    tablesPanel.Visible = false;
                    periodPanel.Visible = true;
                    samplePanel.Visible = true;
                    queryPanel.Visible = false;
                    break;
                case ReportTypes.EnterpriseSummary:
                //case ReportTypes.Disk:
                //case ReportTypes.Memory:
                //case ReportTypes.Sessions:
                    serverPanel.SendToBack();
                    serversPanel.Visible = false;
                    serverPanel.Visible = true;
                    databasesPanel.Visible = false;
                    databasePanel.Visible = false;
                    tablesPanel.Visible = false;
                    periodPanel.Visible = true;
                    samplePanel.Visible = true;
                    queryPanel.Visible = false;
                    break;
                case ReportTypes.DatabaseStatistics:
                //case ReportTypes.DatabaseGrowth:
                    serverPanel.SendToBack();
                    serversPanel.Visible = false;
                    serverPanel.Visible = true;
                    databasesPanel.Visible = true;
                    databasePanel.Visible = false;
                    tablesPanel.Visible = false;
                    periodPanel.Visible = true;
                    samplePanel.Visible = true;
                    queryPanel.Visible = false;
                    break;
                //case ReportTypes.TableGrowth:
                default:
                    databasePanel.SendToBack();
                    serverPanel.SendToBack();
                    serversPanel.Visible = false;
                    serverPanel.Visible = true;
                    databasesPanel.Visible = false;
                    databasePanel.Visible = true;
                    tablesPanel.Visible = true;
                    periodPanel.Visible = true;
                    samplePanel.Visible = true;
                    queryPanel.Visible = false;
                    break; 
            }
        } // ShowParameterControls

        // Enable or disable controls in
        // the parameters pane based on prerequisites.
        // Return true if the user has specified enough parameters 
        // to run the report.  
        private bool UpdateParameterControls() {
            bool allParms = true;

            if (_currentReport == ReportTypes.ServerSummary) {
                // Server(s) is a prerequisite for periodCombo because the
                // servers are needed to calculate the minimum custom date.
                if (MultiServers == null || MultiServers.Count == 0) {
                    allParms = false;
                    periodCombo.Enabled = false;
                } else {
                    periodCombo.Enabled = true;
                }
            } else if ( _currentReport == ReportTypes.QueryOverview || _currentReport == ReportTypes.QueryStatistics) {
                if (MultiServers == null || MultiServers.Count == 0) {
                    allParms = false;
                    periodCombo.Enabled = false;
                } else {
                    periodCombo.Enabled = true;
                }

                // minimum duration must be >= 0
                if (minDuration.Text == string.Empty) {
                    minDuration.Text = "500";
                } else {
                    try {
                        long value = Convert.ToInt64(minDuration.Text.Trim());
                        if (value < 0) {
                            minDuration.Text = "500";
                        }
                    } catch {
                        minDuration.Text = "500";
                    }
                }

                // top N must be >= 1
                if (topN.Text == string.Empty) {
                    topN.Text = "1";
                } else {
                    try {
                        long value = Convert.ToInt64(topN.Text.Trim());
                        if (value < 1) {
                            topN.Text = "1";
                        }
                    } catch {
                        topN.Text = "1";
                    }
                }

                // execution count must be >= 0
                if (executions.Text == string.Empty) {
                    executions.Text = "0";
                } else {
                    try {
                        long value = Convert.ToInt64(executions.Text.Trim());
                        if (value < 0) {
                            executions.Text = "0";
                        }
                    } catch {
                        executions.Text = "0";
                    }
                }

            } else {
                if (SingleServer == null) {
                    allParms = false;
                    periodCombo.Enabled = false;
                } else {
                    periodCombo.Enabled = true;
                }

                if (_currentReport == ReportTypes.DatabaseStatistics || _currentReport == ReportTypes.DatabaseGrowthForecast) {
                    databasesButton.Enabled = allParms;
                    if (MultiDatabases == null || MultiDatabases.Count == 0) allParms = false;
                } else if (_currentReport == ReportTypes.TableGrowthForecast) {
                    databaseButton.Enabled = allParms;
                    if (string.IsNullOrEmpty(SingleDatabase)) allParms = false;
                    tablesButton.Enabled = allParms;
                    if (MultiTables == null || MultiTables.Count == 0) allParms = false;
                }
            }

            viewReportButton.Enabled = allParms;
            expand_collapseButton.Enabled = allParms;
            return allParms;
        }

        // Resize the specified textbox to show all the lines of text up to a max of 3.
        private void AdjustTextBox(TextBox box) {
            // Temporarily force the textbox to one line to calculate how much of its height
            // is "padding" around the font height.
            box.Multiline = false;
            int padding = box.Height - box.Font.Height;

            // Allow it to flow and
            // determine how many lines of text there are when flowed.
            box.Multiline = true;  // Allow text to flow.
            box.ScrollBars = ScrollBars.None; // for now.
            int textLines = box.GetLineFromCharIndex(box.TextLength - 1) + 1;

            // Resize the textbox to show all the lines without scroll bars up to a max of 3.
            int ctrlLines = Math.Min(textLines, 3);
            box.Height = (box.Font.Height * ctrlLines) + padding;

            // Might need vertical scroll bar.
            if (textLines > ctrlLines) box.ScrollBars = ScrollBars.Vertical;

            AdjustParametersPaneHeight();
        }

        // Make the parameters pane just large enough for the visible controls.
        private void AdjustParametersPaneHeight() {
            splitContainer.SplitterDistance = sharedPanel.Bottom;
        }

        // SQL identifiers can contain commas.  If this one does, delemit with [ and ].
        private string MaybeDelimit(string sqlIdentifier) {
            if (sqlIdentifier.Contains(",")) return "[" + sqlIdentifier + "]";
            else return sqlIdentifier;
        }

        private void MakeCSVList(TextBox textBox, IEnumerable list) {
            textBox.Clear();
            if (list != null) {
                foreach (object o in list) {
                    if (textBox.Text == string.Empty) {
                        textBox.Text = MaybeDelimit(o.ToString());
                    } else {
                        textBox.Text += ", " + MaybeDelimit(o.ToString());
                    }
                }
            }

            AdjustTextBox(textBox);
        }

        #region Sample Period

        private void periodCombo_SelectionChanged(object sender, EventArgs e) {
            if (periodCombo.SelectedItem == _prevPeriod) return; // Detect recursive calls.

           Debug.Print("periodCombo selection changed to {0}", periodCombo.SelectedItem);
            if (periodCombo.SelectedItem == _periodSetCustom) {
                // Show the custom period/duration dialog.  
                // If user cancels, retain the previous selection.
                List<MonitoredSqlServer> servers;

                if (_currentReport == ReportTypes.ServerSummary|| 
                    _currentReport == ReportTypes.QueryOverview ||
                    _currentReport == ReportTypes.QueryStatistics) 
                {
                    servers = MultiServers;
                } else {
                    servers = new List<MonitoredSqlServer>();
                    servers.Add(SingleServer);
                }

                ReportPeriodDialog dlg = new ReportPeriodDialog(servers);
                if (_periodCustom != null) dlg.ParseString(_periodCustom.DisplayText.Remove(0, _customPrefix.Length));
                if (dlg.ShowDialog(FindForm()) == DialogResult.OK) {
                    _customDates = dlg.GetDates();

                    if (_periodCustom == null) {
                        _periodCustom = new ValueListItem(PeriodType.Custom, null);
                        periodCombo.Items.Add(_periodCustom);
                    }
                    _periodCustom.DisplayText = _customPrefix + dlg.GetString();
                    _prevPeriod = _periodCustom; // Needs to come before the next line.
                    periodCombo.SelectedItem = _periodCustom; // This leads to a harmless recursive call.
                    
                    UpdateSampleSizes(false);
                } else {                    
                    periodCombo.SelectedItem = _prevPeriod; // This leads to a harmless recursive call.
                }
            } else {
                _prevPeriod = periodCombo.SelectedItem;
                UpdateSampleSizes(false);
            }

            UpdateParameterControls();
        }

        // Show the appropriate sample size options based on the period.
        private void UpdateSampleSizes(bool selectDefault) {
            ValueListItem selected = sampleSizeCombo.SelectedItem;
            sampleSizeCombo.Items.Clear();

            if (periodCombo.SelectedItem == _periodToday) {
                sampleSizeCombo.Items.Add(_sampleMinutes);
                sampleSizeCombo.Items.Add(_sampleHours);
            } else if (periodCombo.SelectedItem == _period7) {
                sampleSizeCombo.Items.Add(_sampleHours);
                sampleSizeCombo.Items.Add(_sampleDays);
            } else if (periodCombo.SelectedItem == _period30) {
                sampleSizeCombo.Items.Add(_sampleDays);
            } else if (periodCombo.SelectedItem == _period365) {
                sampleSizeCombo.Items.Add(_sampleDays);
                sampleSizeCombo.Items.Add(_sampleMonths);
                sampleSizeCombo.Items.Add(_sampleYears);
            } else if (periodCombo.SelectedItem == _periodCustom) {
                UpdateCustomSampleSizes();
            } else {
                // Unexpected
                throw new ApplicationException("An unexpected report period value was selected.");
            }

            if (!selectDefault && sampleSizeCombo.Items.Contains(selected)) {
                sampleSizeCombo.SelectedItem = selected;
            } else if (sampleSizeCombo.Items[0] == _sampleMinutes) {
                sampleSizeCombo.SelectedIndex = 1;
            } else {
                sampleSizeCombo.SelectedIndex = 0;
            }
        }

        // Show the appropriate sample size options based on _customDates.
        // The goal is to prevent the user from trying to plot too many or two few
        // data points.  The limits are arbitrary.  
        private void UpdateCustomSampleSizes() {
            DateTime first = _customDates[0].UtcStart.ToLocalTime();
            DateTime last = _customDates[_customDates.Count-1].UtcEnd.ToLocalTime();
            TimeSpan span = new TimeSpan(0);

            foreach (DateRangeOffset range in _customDates) {
                 span += range.UtcEnd.ToLocalTime().Date - range.UtcStart.ToLocalTime().Date;
            }

            if (span.TotalMinutes < 2000.0) sampleSizeCombo.Items.Add(_sampleMinutes);
            if (span.TotalHours < 500.0) sampleSizeCombo.Items.Add(_sampleHours);
            if (span.TotalDays < 500.0 && span.TotalDays > 1.0)  sampleSizeCombo.Items.Add(_sampleDays);
            if (first.Month != last.Month || first.Year != last.Year) sampleSizeCombo.Items.Add(_sampleMonths);
            if (first.Year != last.Year) sampleSizeCombo.Items.Add(_sampleYears);
        }

        // Based on the current period selection, get the start and 
        // end dates to pass to the stored proc and report.
        private void SetDateRanges(WorkerData workerData) {
            if (workerData.periodType == PeriodType.Custom) {
                workerData.dateRanges = _customDates;
                workerData.periodDescription = periodCombo.SelectedItem.DisplayText;
                workerData.X1Min = _customDates[0].UtcStart.ToLocalTime();
                workerData.X1Max = _customDates[_customDates.Count - 1].UtcEnd.ToLocalTime();
            } else {
                workerData.X1Max = DateTime.Now;

                switch (workerData.periodType) {
                    case PeriodType.Today:
                        workerData.X1Min = workerData.X1Max.Date;
                        break;
                    case PeriodType.Last7:
                        workerData.X1Min = workerData.X1Max.AddDays(-7);
                        break;
                    case PeriodType.Last30:
                        workerData.X1Min = workerData.X1Max.AddDays(-30);
                        break;
                    case PeriodType.Last365:
                        workerData.X1Min = workerData.X1Max.AddDays(-365);
                        break;
                    default:
                        // Unexpected
                        throw new ApplicationException("An unexpected report duration value was selected.");
                }

                workerData.dateRanges = new List<DateRangeOffset>();
                DateRangeOffset.AddDateRange(workerData.dateRanges, workerData.X1Min, workerData.X1Max);
                workerData.periodDescription = string.Format(
                    "{0} ({1} - {2})",
                    periodCombo.SelectedItem.DisplayText,
                    workerData.X1Min,
                    workerData.X1Max);
            }
        }

        #endregion

        // This is a debugging aid that displays the "raw" data returned by the
        // stored procs.
        //private void DisplayDataTable(DataTable dataTable) {
        //    DataTableDialog dlg = new DataTableDialog();
        //    dlg.ultraGrid.DataSource = dataTable;
        //    dlg.ShowDialog();
        //}

        // Create and initialize a new BackgroundWorker and start its thread.
        private void viewReportButton_Click(object sender, EventArgs e) {
            using (Log.DebugCall()) {
                State = UIState.GettingData;
                _reportData = new WorkerData();
                _reportData.bgWorker = new BackgroundWorker();
                _reportData.bgWorker.WorkerSupportsCancellation = true;
                _reportData.bgWorker.DoWork += bgWorker_DoWork;
                _reportData.bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;

                _reportData.reportType = _currentReport;
                _reportData.periodType = (PeriodType)periodCombo.SelectedItem.DataValue;
                _reportData.sampleSize = (SampleSize)sampleSizeCombo.SelectedItem.DataValue;
                SetDateRanges(_reportData);
                _reportData.bgWorker.RunWorkerAsync(_reportData);

                expand_collapseButton.Enabled = true;
            }
        }

        private void expand_collapseButton_Click(object sender, EventArgs e) {
            //if (_isExpanded) {
            //    //that means we need to collapse everything
            //    expand_collapseButton.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskShow_16;
            //    expand_collapseButton.Text = "Expand All";
            //} else {
            //    //show everything
            //    expand_collapseButton.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TaskHide_161;
            //    expand_collapseButton.Text = "Collapse All";
            //}

            //toggle
            _isExpanded = !_isExpanded;

            //run the report
            //viewReportButton_Click(this, null);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e) {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall()) {
                WorkerData reportData = (WorkerData)e.Argument;
                int[] serverIds;

                Log.Debug("reportData.reportType = ", reportData.reportType);
                reportData.cancelled = true;
                /*switch (reportData.reportType) {
                    case ReportTypes.QueryOverview:
                    case ReportTypes.QueryStatistics:
                        serverIds = GetServerIds(MultiServers);
                        reportData.table1 = RepositoryHelper.GetQueriesReportData(
                            ref reportData.cancelled,
                            reportData.reportType == ReportTypes.QueryOverview,
                            serverIds,
                            MultiServers,
                            Convert.ToInt32(topN.Text),
                            reportData.dateRanges,
                            storedProcs.Checked,
                            sqlStmts.Checked,
                            batches.Checked,
                            Convert.ToInt32(minDuration.Text),
                            Convert.ToInt32(executions.Text),
                            _worstPerformingFilter
                            );
                        break;
                    case ReportTypes.ServerOverview:
                        serverIds = GetServerIds(MultiServers);
                        reportData.table1 = RepositoryHelper.GetReportData(_spOverview, null, serverIds, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.Cpu:
                        reportData.table1 = RepositoryHelper.GetReportData(_spProcessor, null, new int[] { SingleServer.Id }, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.Disk:
                        reportData.table1 = RepositoryHelper.GetReportData(_spDisk, null, new int[] { SingleServer.Id }, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.Memory:
                        reportData.table1 = RepositoryHelper.GetReportData(_spMemory, null, new int[] { SingleServer.Id }, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.Sessions:
                        reportData.table1 = RepositoryHelper.GetReportData(_spSessions, null, new int[] { SingleServer.Id }, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.DatabaseOverview:
                        reportData.table1 = RepositoryHelper.GetDatabasesReportData(_spDBStats, null, SingleServer.Id, MultiDatabases, reportData.dateRanges, (int)reportData.sampleSize);
                        break;
                    case ReportTypes.DatabaseGrowth:
                        reportData.table1 = RepositoryHelper.GetDatabasesReportData(_spDBGrowth, null, SingleServer.Id, MultiDatabases, reportData.dateRanges, (int)reportData.sampleSize);
                        if (!reportData.cancelled) {
                            CalculatePercentChange(reportData.table1, "DatabaseName", "TotalSizeKb", "DataGrowth", out reportData.Y1Min, out reportData.Y1Max);
                        }
                        break;
                    case ReportTypes.TableGrowth:
                        string[] tableList = MakeTableNameArray(MultiTables, false);
                        reportData.table1 = RepositoryHelper.GetTablesReportData(_spTableGrowth, SingleServer.Id, SingleDatabase, tableList, reportData.dateRanges, (int)reportData.sampleSize);
                        if (!reportData.cancelled) {
                            CalculatePercentChange(reportData.table1, "TableName", "TotalSize", "GrowthPct", out reportData.Y1Min, out reportData.Y1Max);
                            CalculatePercentChange(reportData.table1, "TableName", "NumberOfRows", "RowGrowthPct", out reportData.Y2Min, out reportData.Y2Max);
                            if (!reportData.cancelled) { //check again just in case they cancelled while calculating change
                                reportData.table2 = RepositoryHelper.GetTablesReportData(_spTableReorg, SingleServer.Id, SingleDatabase, tableList, reportData.dateRanges, (int)reportData.sampleSize);
                            }
                        }
                        break;
                    default:
                        throw new ApplicationException("Unexpected report type: " + reportData.reportType);
                }*/
                        
                //System.Threading.Thread.Sleep(2000); // Simulate long run time.

                if (reportData.cancelled) {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                } else {
                    e.Result = reportData;

                  /*  if (reportData.reportType != ReportTypes.WorstPerforming && reportData.reportType != ReportTypes.MostFrequent) {
                        // Don't allow any column to have all null values since that
                        // causes the report generator to display a wierd error message.
                        AvoidAllNullColumns(reportData.table1);
                    }*/

                    // I keep forgetting to comment this out, so use a time-based cutoff.
                    //if (DateTime.Now < new DateTime(2007, 7, 4))
                      //  DisplayDataTable(reportData.table1);
                    LogDataTable(reportData.table1, 500, BBS.TracerX.TraceLevel.Verbose);

                    if (reportData.table2 != null) {
                        AvoidAllNullColumns(reportData.table2);
                        //DisplayDataTable(reportData.table2);
                        LogDataTable(reportData.table2, 500, BBS.TracerX.TraceLevel.Verbose);
                    }
                }
            }
        }

        static private void AvoidAllNullColumns(DataTable dataTable) {
            if (dataTable != null && dataTable.Rows.Count > 0) {
                foreach (DataColumn col in dataTable.Columns) {
                    if (col.DataType == typeof(double) ||
                        col.DataType == typeof(int) ||
                        col.DataType == typeof(Int64)) //
                    {
                        bool allNulls = true;
                        foreach (DataRow row in dataTable.Rows) {
                            if (row[col] != DBNull.Value) {
                                allNulls = false;
                                break;
                            }
                        }

                        if (allNulls) {
                            Log.Warn("Setting first row to 0 for column ", col);
                            col.ReadOnly = false;
                            dataTable.Rows[0][col] = 0;
                        }
                    }
                }
            }
        }

        // Log up to maxRows of the specified DataTable if the table is not null and
        // the trace level condition is met.
        static private void LogDataTable(DataTable t1, int maxRows, BBS.TracerX.TraceLevel level) {
            if (Log.FileTraceLevel >= level) {
                if (t1 == null) {
                    Log.Warn("Can't log a null DataTable!");
                } else {
                    StringBuilder str = new StringBuilder();
                    int rowCount = 0;
                    string[] formatStrings = new string[t1.Columns.Count];

                    Log.Info("maxRows = ", maxRows, ", Rows.Count = ", t1.Rows.Count);

                    foreach (DataColumn col in t1.Columns) {
                        str.AppendFormat("{0} | ", col.ColumnName);
                        formatStrings[col.Ordinal] = string.Format("{{0,-{0}}} | ", col.ColumnName.Length);
                    }

                    foreach (DataRow row in t1.Rows) {
                        str.Append("\n");
                        foreach (DataColumn col in t1.Columns) {
                            str.AppendFormat(formatStrings[col.Ordinal], row[col]);
                        }

                        if (++rowCount >= maxRows) break;
                    }

                    Log.Info("String to log is this long: ", str.Length);
                    Log.Info(str);
                }
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            using (Log.DebugCall()) {
                // Make sure this call is for the most recently requested report.
                Log.Debug("(_reportData.bgWorker == sender) = ", _reportData.bgWorker == sender);
                if (_reportData.bgWorker == sender) {
                    // This event handler was called by the currently active report
                    if (_reportData.cancelled) {
                        Log.Debug("_reportData.cancelled = true.");
                        return;
                    } else if (e.Error != null) {
                        if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2) {
                                Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                            }
                        } else {
                            ApplicationMessageBox.ShowError(this, "An error occurred while retrieving data for the report.  ",
                                            e.Error);
                        }

                        State = UIState.NoDataAcquired;
                    } else {
                        if (_reportData.reportType == ReportTypes.TableGrowthForecast) {
                            // This report requires two tables and has a special message if
                            // there is no data. We need something in one of the two tables.
                            if ((_reportData.table1 == null || _reportData.table1.Rows.Count == 0) &&
                                (_reportData.table2 == null || _reportData.table2.Rows.Count == 0))
                            {
                                State = UIState.NoTableDataAcquired;
                            } else {
                                RenderReport(_reportData);
                            }
                        } else if (_reportData.table1 == null || _reportData.table1.Rows.Count == 0) {
                            State = UIState.NoDataAcquired;
                        } else {
                            RenderReport(_reportData);
                        }
                    }
                }
            }
        } // bgWorker_RunWorkerCompleted

        // Calculate the percent change in a specified column from row to row.
        private static void CalculatePercentChange(
            DataTable dataTable,
            string nameField,   // Field containing the table or db name.  
            string inputField,  // Field whose percent change is being calculated.
            string outputField, // Field in which to store the calculated value.
            out double min,     // The minimum value calculated for the output field.
            out double max      // The maximum value calculated for the output field.
        ) {
            using (Log.DebugCall()) {
                min = 0;
                max = 1.0;

                Log.Debug("nameField = ", nameField);
                Log.Debug("inputField = ", inputField);
                Log.Debug("outputField = ", outputField);

                if (dataTable == null) {
                    Log.Error("dataTable is null in CalculatePercentChange!");
                } else {
                    string curName = null;
                    string lastName = string.Empty;
                    double? curSize = null;
                    double? lastSize;
                    double? growth;

                    dataTable.Columns[outputField].ReadOnly = false;

                    // This logic requires the rows to be sorted by
                    // the nameField field (e.g. table or db name) and then by time.
                    foreach (DataRow row in dataTable.Rows) {
                        try {
                            curName = (string)row[nameField];

                            if (lastName == curName) {
                                lastSize = curSize;
                            } else {
                                lastSize = null;
                            }

                            if (row[inputField] is double) {
                                curSize = (double)row[inputField];
                            } else {
                                // The table sometimes contains nulls due to the services being
                                // restarted.  In such cases, just use the last valid value instead
                                // of something like 0.0 which would look like -100% growth.
                                curSize = lastSize;
                            }

                            if (curSize == null || lastSize == null) {
                                growth = null;
                            } else if (lastSize == 0) {
                                // When changing from 0 to non-zero, report 100% instead of dividing by 0.
                                if (curSize > 0) growth = 100.0;
                                else if (curSize < 0) growth = -100.0;
                                else growth = 0.0;
                            } else {
                                growth = 100.0 * ((curSize / lastSize) - 1.0);
                            }
                        } catch {
                            growth = null;
                        }

                        if (growth == null) {
                            row[outputField] = DBNull.Value;
                        } else {
                            double dgrowth = (double)growth;
                            row[outputField] = dgrowth;

                            // min and max are used to address PR 2010319, which complained about
                            // the Y axis on charts having small/wierd label values like +/- 2E-14.
                            if (dgrowth < min) min = Math.Floor(dgrowth);
                            if (dgrowth > max) max = Math.Ceiling(dgrowth);
                        }
                        lastName = curName;
                    } // foreach row
                } // if table not null
            } // using log
        } // CalculatePercentChange

        // Configure the ReportViewer to display the specified report using the data
        // in reportData.  This always creates a new instance of ReportViewer
        // to replace the old one.  This seems to be the only way to avoid getting the 
        // "Report processing was cancelled." message.
        private void RenderReport(WorkerData reportData) {
            using (Log.DebugCall()) {
                Log.Debug("reportData.reportType = ", reportData.reportType);
                Control parent = reportViewer.Parent;
                ReportViewer oldViewer = reportViewer;

                reportViewer = CreateReportViewer(reportData);
                parent.Controls.Remove(oldViewer);
                parent.Controls.Add(reportViewer);

                SetReportDataSources(reportData);
                SetReportPath(reportData);
                SetReportParameters(reportData);

                if (_firstReport) {
                    // Set the DisplayMode and ZoomMode the first time a report is
                    // generated.  After that, don't override any changes made by the user.
                    // The data source must be set before calling SetDisplayMode or an
                    // exception will be thrown (it was set earlier).
                    Log.Debug("Rendering first report.");
                    reportViewer.SetDisplayMode(DisplayMode.Normal);
                    reportViewer.ZoomMode = ZoomMode.Percent;
                    reportViewer.ZoomPercent = 100;
                    _firstReport = false;
                } else {
                    // We created a new instance of ReportViewer.
                    // Copy some key properties from the old one.
                    // The data source must be set before calling SetDisplayMode or an
                    // exception will be thrown (it was set earlier).
                    Log.Debug("Rendering new reportViewer.");
                    reportViewer.SetDisplayMode(GetDisplayMode(oldViewer));
                    reportViewer.ZoomMode = oldViewer.ZoomMode;
                    reportViewer.ZoomPercent = oldViewer.ZoomPercent;
                }
            } // using Log
        }
        
        // A cache of ReportDef objects so we don't have to parse
        // the same rdlc file more than once.
        private Dictionary<ReportTypes, ReportDef> _reportDefCache = new Dictionary<ReportTypes, ReportDef>();

        private ReportDef GetReportDef(ReportTypes reportType) {
            ReportDef reportDef;

            if (!_reportDefCache.TryGetValue(reportType, out reportDef)) {
                string regularPath = GetReportFileName(reportType);
                reportDef = new ReportDef(regularPath);
                _reportDefCache.Add(reportType, reportDef);
            }

            return reportDef;
        }

        private void SetReportPath(WorkerData reportData) {
            using (Log.VerboseCall()) {
                switch (reportData.reportType) {
                    //case ReportTypes.WorstPerforming:
                   // case ReportTypes.MostFrequent:
                      //  reportData.reportViewer.LocalReport.ReportPath = GetReportFileName(reportData.reportType);
                        //break;
                    default:
                        if (reportData.periodType == PeriodType.Custom &&
                            !DateRangeOffset.IsContiguous(reportData.dateRanges)) //
                    {
                            ReportDef reportDef = GetReportDef(reportData.reportType);
                            reportData.reportViewer.LocalReport.ReportPath = reportDef.PathForNonContiguousDates;
                        } else {
                            reportData.reportViewer.LocalReport.ReportPath = GetReportFileName(reportData.reportType);
                        }
                        break;
                }
            }
        }

        private void SetReportParameters(WorkerData workerData) {
            using (Log.VerboseCall()) {
                string msg = string.Empty;
                
                switch (workerData.reportType) {
                   /* case ReportTypes.WorstPerforming:
                    case ReportTypes.MostFrequent:
                        if (!_worstPerformingFilter.HasDefaultValues())
                            msg = string.Format("Only those queries matching the following criteria were included: {0}.", _worstPerformingFilter);

                        reportViewer.LocalReport.SetParameters(new ReportParameter[] { 
                                new ReportParameter("Servers", GetServerList()),
                                new ReportParameter("PeriodHeader", "Period:"),
                                new ReportParameter("PeriodList", workerData.periodDescription),
                                new ReportParameter("EventTypes", GetEventTypes()),
                                new ReportParameter("Message1", string.Format("This report includes the {0} {2} for each server, ignoring individual invocations that lasted less than {1} milliseconds.", 
                                                    topN.Text.TrimStart('0'), 
                                                    minDuration.Text,
                                                    workerData.reportType == ReportTypes.WorstPerforming ? "worst performing queries (by duration)" : "most frequently executed queries")),
                                new ReportParameter("Message2", msg),
                                new ReportParameter("Expand_All", _isExpanded.ToString()), 
                                new ReportParameter("ShowPointLabels", GetShowPointLabels().ToString())
                                });
                        break;
                 */   
                    default:
                        ReportDef reportDef = GetReportDef(workerData.reportType);
                        List<string> chartsWithoutData = reportDef.AllChartsHaveEnoughData(workerData.reportViewer.LocalReport.DataSources);
                        string insufficientDataMsg = string.Empty;

                        if (chartsWithoutData.Count > 0) {
                            insufficientDataMsg += string.Format(
                                "There are not enough data points to generate {1} chart{2} for this report. " +
                                "To add more data points for the report, consider the following:\n\n" +
                                "   1. Select a longer time period.\n" +
                                "   2. Select a smaller sample size.\n" +
                                "   3. Wait for more data to be collected before generating the report.\n\n" +
                                "The following chart{2} {0} excluded from the report:\n\n",
                                chartsWithoutData.Count > 1 ? "are" : "is", chartsWithoutData.Count, chartsWithoutData.Count > 1 ? "s" : "");

                            //add each chart that we didn't render to the message
                            foreach (string title in chartsWithoutData) {
                                insufficientDataMsg += string.Format("   - {0}\n", title);
                            }
                        }

                        if (workerData.reportType == ReportTypes.TableGrowthForecast) { //has the addition of table list
                            reportViewer.LocalReport.SetParameters(new ReportParameter[] { 
                                new ReportParameter("ServerName", SingleServer == null ? "none (probably overview)" : SingleServer.InstanceName),
                                new ReportParameter("DBName", SingleDatabase ?? "none"),
                                new ReportParameter("PeriodList", workerData.periodDescription),
                                new ReportParameter("SampleSize", workerData.sampleSize.ToString()),
                                new ReportParameter("DateFormat", GetDateFormat(workerData.periodType, workerData.sampleSize)),
                                new ReportParameter("InsufficientDataMsg", insufficientDataMsg),
                                new ReportParameter("Y1Min", workerData.Y1Min.ToString()),
                                new ReportParameter("Y1Max", workerData.Y1Max.ToString()),
                                new ReportParameter("Y2Min", workerData.Y2Min.ToString()),
                                new ReportParameter("Y2Max", workerData.Y2Max.ToString()),
                                new ReportParameter("X1Min", workerData.X1Min.ToString(/*DateTimeFormatInfo.InvariantInfo*/)),
                                new ReportParameter("X1Max", workerData.X1Max.ToString(/*DateTimeFormatInfo.InvariantInfo*/)),
                                new ReportParameter("ShowPointLabels", GetShowPointLabels().ToString()),
                                new ReportParameter("Tables", GetTableList())});
                        } else {
                            reportViewer.LocalReport.SetParameters(new ReportParameter[] {
                                new ReportParameter("ServerName", SingleServer == null ? "none (probably overview)" : SingleServer. InstanceName),
                                new ReportParameter("DBName", SingleDatabase ?? "none") ,
                                new ReportParameter("PeriodList", workerData. periodDescription),
                                new ReportParameter("SampleSize", workerData.sampleSize.ToString()),
                                new ReportParameter("DateFormat", GetDateFormat(workerData.periodType, workerData.sampleSize)),
                                new ReportParameter("InsufficientDataMsg", insufficientDataMsg),
                                new ReportParameter("Y1Min", workerData.Y1Min.ToString()),
                                new ReportParameter("Y1Max", workerData.Y1Max.ToString()),
                                new ReportParameter("Y2Min", workerData.Y2Min.ToString()),
                                new ReportParameter("Y2Max", workerData.Y2Max.ToString()),
                                new ReportParameter("X1Min", workerData.X1Min.ToString(/*DateTimeFormatInfo.InvariantInfo*/)),
                                new ReportParameter("X1Max", workerData.X1Max.ToString(/*DateTimeFormatInfo.InvariantInfo*/)),
                                new ReportParameter("ShowPointLabels", GetShowPointLabels().ToString())});
                        }

                        break;
                }
            }
        }

        private ReportViewer CreateReportViewer(WorkerData reportData) {
            using (Log.VerboseCall()) {
                reportData.reportViewer = new ReportViewer();
                Log.Debug("Created new ReportViewer with hash code = ", reportData.reportViewer.GetHashCode());

                //reportData.reportViewer.LocalReport.ReportPath = GetReportFileName(reportData.reportType);
                reportData.reportViewer.ShowDocumentMapButton = false;
                reportData.reportViewer.ShowRefreshButton = false;
                reportData.reportViewer.ShowBackButton = false;
                reportData.reportViewer.ShowStopButton = false;
                reportData.reportViewer.ShowFindControls = false;
                reportData.reportViewer.Dock = reportViewer.Dock;
                reportData.reportViewer.Visible = reportViewer.Visible;
                reportData.reportViewer.RenderingComplete += this.reportViewer_RenderingComplete;
                reportData.reportViewer.ReportRefresh += reportViewer_ReportRefresh;
                reportData.reportViewer.ReportError += reportViewer_ReportError;
                reportData.reportViewer.RenderingBegin += reportViewer_RenderingBegin;
                reportData.reportViewer.Drillthrough += reportViewer_Drillthrough;
                reportData.reportViewer.ReportExport += reportViewer_ReportExport;

                foreach (RenderingExtension ext in reportData.reportViewer.LocalReport.ListRenderingExtensions()) {
                    if (ext.Name == "PDF") ReflectivelySetVisibility(ext, false);
                    //Debug.Print(ext.Name + " - " + ext.Visible);
                }

                //enable hyperlinks
                reportData.reportViewer.LocalReport.EnableHyperlinks = true;

                return reportData.reportViewer;
            }
        }

        private void ReflectivelySetVisibility(RenderingExtension extension, bool visible) {
            FieldInfo info = extension.GetType().GetField("m_serverExtension",
                                                          BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null) {
                Microsoft.SqlServer.ReportingServices2005.Execution.Extension rsExtension = info.GetValue(extension) as Microsoft.SqlServer.ReportingServices2005.Execution.Extension;
                if (rsExtension != null) {
                    rsExtension.Visible = visible;
                }
            }
        }

        void reportViewer_ReportExport(object sender, ReportExportEventArgs e) {
            Log.Debug("ReportExport event handler called.");
        } 

        public string GetEventTypes() {
            string str = string.Empty;
            if (sqlStmts.Checked) str += "SQL statements, ";
            if (batches.Checked) str += "SQL batches, ";
            if (storedProcs.Checked) str += "Stored procedures";
            return str.TrimEnd(' ', ',');
        }

        public bool GetShowPointLabels() {
            return showPointLabels.Checked;
        }

        private static string GetDateFormat(PeriodType periodType, SampleSize sampleSize) {
            switch (periodType) {
                case PeriodType.Today: 
                    return "t";
                case PeriodType.Last365:
                    // if it's 365 days, the charts can't handle it and need broader
                    // formatting.
                    return "y";
            }

            switch (sampleSize) {
                case SampleSize.Minutes: return "g"; // short date and short time (hh:mm)
                case SampleSize.Hours: return "g";
                case SampleSize.Days: return "d"; // short date m/d/y
                case SampleSize.Months: return "y"; // year month
                case SampleSize.Years: return "yyyy"; // four digit year
            }

            return "g";
        }

        // Return the file name of the report definition file
        // to use for the specified report type.
        private static string GetReportFileName(ReportTypes report)
        {
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            return exePath;
            
           /* switch (report) {
                case ReportTypes.ServerOverview:
                    return Path.Combine(exePath, "Overview.rdlc");
                case ReportTypes.Cpu:
                    return Path.Combine(exePath, "CPU.rdlc");
                case ReportTypes.Disk:
                    return Path.Combine(exePath, "Disk.rdlc");
                case ReportTypes.Memory:
                    return Path.Combine(exePath, "Memory.rdlc");
                case ReportTypes.Sessions:
                    return Path.Combine(exePath, "Sessions.rdlc");
                case ReportTypes.DatabaseOverview:
                    return Path.Combine(exePath, "DatabaseOverview.rdlc");
                case ReportTypes.DatabaseGrowth:
                    return Path.Combine(exePath, "DatabaseGrowth.rdlc");
                case ReportTypes.TableGrowth:
                    return Path.Combine(exePath, "TableGrowth.rdlc");
                case ReportTypes.WorstPerforming:
                    return Path.Combine(exePath, "WorstPerforming.rdlc");
                case ReportTypes.MostFrequent:
                    return Path.Combine(exePath, "MostFrequent.rdlc");
                default:
                    throw new ApplicationException("Unexpected report: " + report);
            }*/
        }

        public void ShowHelp() {
            string topic = HelpTopics.HelpStartPage;

         /*   switch (_currentReport) {
                case ReportTypes.ServerOverview:
                    topic = HelpTopics.ReportsServerOverview;
                    break;
                case ReportTypes.Cpu:
                    topic = HelpTopics.ReportsCpu;
                    break;
                case ReportTypes.Disk:
                    topic = HelpTopics.ReportsDisk;
                    break;
                case ReportTypes.Memory:
                    topic = HelpTopics.ReportsMemory;
                    break;
                case ReportTypes.Sessions:
                    topic = HelpTopics.ReportsSessions;
                    break;
                case ReportTypes.DatabaseOverview:
                    topic = HelpTopics.ReportsDatabaseOverview;
                    break;
                case ReportTypes.DatabaseGrowth:
                    topic = HelpTopics.ReportsDatabaseGrowth;
                    break;
                case ReportTypes.TableGrowth:
                    topic = HelpTopics.ReportsTableGrowth;
                    break;
                case ReportTypes.WorstPerforming:
                    topic = HelpTopics.ReportsWorstPerformingQueries;
                    break;
                case ReportTypes.MostFrequent:
                    topic = HelpTopics.ReportsMostFrequentQueries;
                    break;
            }*/

            ApplicationHelper.ShowHelpTopic(topic);
        }

        // Add the specified table(s) to the reportViewer's DataSources collection and give them
        // the appropriate names based on the report type.
        private void SetReportDataSources(WorkerData workerData) {
            using (Log.VerboseCall()) {
              /*  switch (workerData.reportType) {
                    case ReportTypes.ServerOverview:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetOverviewReportTemp", workerData.table1));
                        break;
                    case ReportTypes.Cpu:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetProcessorReportTemp", workerData.table1));
                        break;
                    case ReportTypes.Disk:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetDiskReportTemp", workerData.table1));
                        break;
                    case ReportTypes.Memory:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetMemoryReportTemp", workerData.table1));
                        break;
                    case ReportTypes.Sessions:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetSessionsReportTemp", workerData.table1));
                        break;
                    case ReportTypes.DatabaseOverview:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetDatabaseStatisticsReportTemp", workerData.table1));
                        break;
                    case ReportTypes.DatabaseGrowth:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetDatabaseGrowthReportTemp", workerData.table1));
                        break;
                    case ReportTypes.TableGrowth:
                        // At present, this is the only report that requires two datasources
                        // and this is the only reason WorkerData.table2 exists.
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetTableGrowthReportTemp", workerData.table1));
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("ReportsDataset_p_GetTableReorgReportTemp", workerData.table2));
                        break;
                    case ReportTypes.WorstPerforming:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("GetWorstPerformingDataSet_p_GetWorstPerformingReportTemp", workerData.table1));
                        break;
                    case ReportTypes.MostFrequent:
                        reportViewer.LocalReport.DataSources.Add(new ReportDataSource("GetWorstPerformingDataSet_p_GetMostFrequentReportTemp", workerData.table1));
                        break;
                    default:
                        throw new ApplicationException("Unexpected report type: " + workerData.reportType);
                }*/
            }
        }

        // This is part of the "drill through" handling for 
        // single-server server-wide reports.  The specific 
        // server in question is extracted from
        // localReport's parameters.
        private void SelectServerForDrillThrough(LocalReport rpt) {            
            // Get the name of the server that was clicked from
            // the parameters.
            ReportParameterInfo parm = rpt.GetParameters()["ServerName"];
            string server = parm.Values[0] as string;
            Log.Debug("ServerName parameter is ", server);
            foreach (MonitoredSqlServer instance in MultiServers) {
                if (instance.InstanceName == server) {
                    Log.Debug("Found server in MultiServers.");
                    SingleServer = instance;
                    break;
                }
            }
        }

        // Simulate the user selecting ALL databases on the server.
        private void SelectAllDbs(MonitoredSqlServer server) {
            using (Log.DebugCall()) {
                // This should really be called in a worker thread.
                IDictionary<string,bool> databases = DatabaseBrowserDialog.GetDatabases(ManagementServiceHelper.IManagementService, server.Id);
                MultiDatabases = new List<string>(databases.Keys); // Can't do in worker thread.
                UpdateParameterControls();                     // Can't do in worker thread.
            }
        }

        //// Simulate the user selecting ALL tables on the server + database.
        //private void SelectAllTables(MonitoredSqlServer server, string dbName) {
        //    using (Log.DebugCall()) {
        //        // This should really be called in a worker thread.
        //        IDictionary<string, bool> tables = TableBrowserDialog.GetTables(server.Id, dbName);
        //        ICollection<string>
        //        MultiTables = new List<string>(tables.Keys); // Can't do in worker thread.
        //        UpdateParameterControls(); // Can't do in worker thread.
        //    }
        //}

        private void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e) {
            using (Log.DebugCall()) {
                e.Cancel = true; // We'll handle this our way.
                Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Verbose;
                Log.Debug("e.ReportPath = ", e.ReportPath);
                LocalReport rpt = (LocalReport)e.Report;

              /*  if (_currentReport == ReportTypes.ServerOverview) {
                    // The current report is the overview report, which is a
                    // multi-server report.  The new report will be a single
                    // server report.  Select that server.
                    SelectServerForDrillThrough(rpt);
                }*/

              /*  switch (e.ReportPath) {
                    case "CPU":
                        ApplicationController.Default.ShowReportsView(ReportTypes.Cpu);
                        break;
                    case "Disk":
                        ApplicationController.Default.ShowReportsView(ReportTypes.Disk);
                        break;
                    case "Memory":
                        ApplicationController.Default.ShowReportsView(ReportTypes.Memory);
                        break;
                    case "Sessions":
                        ApplicationController.Default.ShowReportsView(ReportTypes.Sessions);
                        break;
                    case "DatabaseOverview":
                        SelectAllDbs(SingleServer);
                        ApplicationController.Default.ShowReportsView(ReportTypes.DatabaseOverview);
                        break;
                    case "DatabaseGrowth":
                        //SelectAllDbs(SingleServer);
                        ApplicationController.Default.ShowReportsView(ReportTypes.DatabaseGrowth);
                        break;
                    case "TableGrowth":
                        ReportParameterInfo parm = rpt.GetParameters()["DBName"];
                        SingleDatabase = parm.Values[0];
                        Log.Debug("DBName parameter is ", SingleDatabase);
                        //SelectAllTables(SingleServer, SingleDatabase);
                        ApplicationController.Default.ShowReportsView(ReportTypes.TableGrowth);
                        break;
                }*/
            }
        }

        private DateTime _renderStart;
        private void reportViewer_RenderingBegin(object sender, CancelEventArgs e) {
            _renderStart = DateTime.Now;
            Log.Debug("RenderingBegin event for ", sender.GetHashCode());
            State = UIState.Rendering;
        }

        // This generally occurs when the user switches to or from print layout view.
        private void reportViewer_ReportRefresh(object sender, CancelEventArgs e) {
            Log.Debug("ReportRefresh event for ", sender.GetHashCode());
        }

        private void reportViewer_ReportError(object sender, ReportErrorEventArgs e) {
            // Generally, errors occur when the datasource does not have enough
            // rows for the report viewer to generate one or more charts. 
           
            Log.Warn("ReportError event for ", sender.GetHashCode());
            //ReportViewer senderObject = (ReportViewer)sender;

            // Make sure this event is for the currently active instance of ReportViewer.
            if (sender == reportViewer) {
                Log.Warn("ReportErrorEventArgs.Exception = ", e.Exception);
                Log.Warn("ReportErrorEventArgs.Handled   = ", e.Handled);

                // Don't display an error for a cancelled report.
                if (cancelledLabel.Visible == false) {
                    State = UIState.ReportError;
                    ApplicationMessageBox.ShowError(this, e.Exception);
                }
            }
        }

        // This is called when rendering is cancelled or completed.
        private void reportViewer_RenderingComplete(object sender, RenderingCompleteEventArgs e) {
            Log.Debug("RenderingComplete event for ", sender.GetHashCode());

            // Make sure this event is for the currently active instance of ReportViewer.
            if (sender == reportViewer) {
                Log.Debug("Rendering took ", DateTime.Now - _renderStart);
                if (e.Exception == null) {
                    State = UIState.Rendered;
                } else {
                    Log.Warn("RenderingCompleteEventArgs.Exception = ", e.Exception);

                    if (e.Exception is OperationCanceledException) {
                        //State = UIState.Cancelled;
                    } else {
                        State = UIState.ReportError;
                    }
                }

                if (e.Warnings != null) {
                    foreach (Warning warning in e.Warnings) {
                        Log.Warn("RenderingCompleteEventArgs warning = ", warning);
                    }
                }
            }
        }

        private void cancelReport_Click(object sender, EventArgs e) {
            _reportData.Cancel();
            State = UIState.Cancelled;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            WorstPerformingFilter copy = new WorstPerformingFilter();
            _worstPerformingFilter.CopyTo(copy);
            GenericFilterDialog dlg = new GenericFilterDialog(copy);
            if (dlg.ShowDialog(this) == DialogResult.OK) 
            {
                copy.CopyTo(_worstPerformingFilter);
            }
        }

        // This event handler only allows backspace and numeric chars in the associated text boxes.
        private void KeyPress_NumbersOnly(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != '\b' && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void WorstPerformingFilterTextChanged(object sender, EventArgs e) {
            UpdateParameterControls();
        }

        private void clearFilter_Click(object sender, EventArgs e) {
            SingleServer = null;
            MultiServers = null;
            SingleDatabase = null;
            MultiDatabases = null;
            MultiTables = null;
         //   periodCombo.SelectedItem = _currentReport == ReportTypes.TableGrowth ? _period7 : _periodToday; // if table growth report set it to last 7 days.
            UpdateSampleSizes(true);
            _worstPerformingFilter.ResetValues();
            storedProcs.Checked = true;
            batches.Checked = true;
            sqlStmts.Checked = true;
            minDuration.Text = "500";
            executions.Text = "0";
            topN.Text = "10";
            AdjustParametersPaneHeight();
            UpdateParameterControls();
       }        

        //private void InvalidateTextBox(object sender, EventArgs e) {
        //    TextBox tb = sender as TextBox;
        //    if (tb != null) {
        //        tb.Invalidate();
        //    }
        //}
    }
}