using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using System.Globalization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesCpuView : ServerBaseView
    {
        private readonly ServerSummaryHistoryData historyData;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Exception historyModeLoadError = null;
        private DataTable top5DataTable;
        private List<string> selectedDatabases = new List<string>();
        private HashSet<string> uniqueDatabases = new HashSet<String>();
        private Dictionary<string, string> dbValMap = new Dictionary<string, string>();

        public ResourcesCpuView(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            RefreshReportsProgress = true;
            this.historyData = historyData;

            InitializeComponent();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                //SetRegularRDSResourcesPanel();
                SetAmazonRDSResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId )
            {
                SetAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                SetManagedAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                SetRegularRDSResourcesPanel();
            }
            // fix context menus on chartfx charts
            ChartFxExtensions.SetContextMenu(callRatesChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cpuCreditBalanceChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cPUCreditUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cpuUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(processorQueueLengthChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(processorTimeChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(maxWorkerChart, toolbarsManager);

            InitializeDatabasesGrid();
            CreateChartDataSource();
            InitializeCharts();
            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ShowVMData(instanceObject.Instance.IsVirtualized);
            AdaptFontSize();
        }
        public void SetRegularRDSResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent,50.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            //SQLdm 30843 missing titles for cpu graphs
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.cpuUsagePanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.processorQueueLengthPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.processorTimePanel, 1, 1);
        }

        /// <summary>
        /// Start datetime for custom range, otherwise null.
        /// </summary>
        public override DateTime? HistoricalStartDateTime
        {
            get { return historyData.HistoricalStartDateTime; }
            set
            {
                if (historyData.HistoricalStartDateTime != value)
                {
                    historyData.HistoricalStartDateTime = value;
                    currentHistoricalStartDateTime = null;
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historyData.HistoricalSnapshotDateTime; }
            set
            {
                historyData.HistoricalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
                ///Handling baseline enabled event --Ankit Nagpal SQLdm 10.0
                case "EnableBaseline":
                    InitializeCpuUsageChart();
                    InitializeProcessorTimeChart();
                    InitializeProcessorQueueLengthChart();
                    break;
            }
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesCpuView);
        }

        private void ShowVMData(bool showVM)
        {
            if (cpuUsageChart.Series.Count == 0 || showVM == cpuUsageChart.Series[0].Visible) return;

            cpuUsageChart.Series[0].Visible =
                cpuUsageChart.Series[1].Visible = showVM;
        }

        #region Initialize Charts

        public void InitializeCharts()
        {

            InitializeCpuUsageChart();
            InitializeProcessorQueueLengthChart();
            InitializeProcessorTimeChart();
            InitializeCallRatesChart();
            InitializeMaxWorkerChart();
            InitializeCpuCreditBalanceChart();
            InitializeCPUCreditUsageChart();
            ForceChartColors();
            InitalizeDrilldown(cpuUsageChart, processorQueueLengthChart, processorTimeChart, callRatesChart,maxWorkerChart, cpuCreditBalanceChart,cPUCreditUsageChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ForceChartColors()
        {
            processorQueueLengthChart.SetAreaSeriesAlphaChannel(175, 0);
            callRatesChart.SetAreaSeriesAlphaChannel(175, 0);
            cpuCreditBalanceChart.SetAreaSeriesAlphaChannel(175, 0);
            cPUCreditUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            cpuUsageChart.SetAreaSeriesAlphaChannel(175, 0);
        }

        public void InitializeCpuUsageChart()
        {
            cpuUsageChart.Tag = cpuStatusHeaderStripLabel;
            cpuUsageChart.Printer.Orientation = PageOrientation.Landscape;
            cpuUsageChart.Printer.Compress = true;
            cpuUsageChart.Printer.ForceColors = true;
            cpuUsageChart.Printer.Document.DocumentName = "CPU Usage Chart";
            cpuUsageChart.ToolBar.RemoveAt(0);
            cpuUsageChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap totalCpuUsageFieldMap = new FieldMap("TotalCpuUsage", FieldUsage.Value);
            totalCpuUsageFieldMap.DisplayName = "OS";
            FieldMap sqlServerCpuUsageFieldMap = new FieldMap("SqlServerCpuUsage", FieldUsage.Value);
            sqlServerCpuUsageFieldMap.DisplayName = "SQL Server";
            FieldMap vmCpuUsageFieldMap = new FieldMap("vmCPUUsage", FieldUsage.Value);
            vmCpuUsageFieldMap.DisplayName = "VM";
            FieldMap esxCpuUsageFieldMap = new FieldMap("esxCPUUsage", FieldUsage.Value);
            esxCpuUsageFieldMap.DisplayName = "Host";

            //START: SQLdm 10.0 (Tarun Sapra)- Displaying baselines 
            FieldMap sqlServerCPUUsage = new FieldMap("SqlServerCPUUsageBaseline", FieldUsage.Value);
            sqlServerCPUUsage.DisplayName = "Baseline- CPU Usage";

            FieldMap vmCPUUsage = new FieldMap("VmCPUUsageBaseline", FieldUsage.Value);
            vmCPUUsage.DisplayName = "Baseline- VM Usage";

            FieldMap hostCPUUsage = new FieldMap("HostCPUUsageBaseline", FieldUsage.Value);
            hostCPUUsage.DisplayName = "Baseline- Host Usage";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            cpuUsageChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
                cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                     {
                                                                     dateFieldMap,
                                                                     vmCpuUsageFieldMap,
                                                                     esxCpuUsageFieldMap,
                                                                     sqlServerCpuUsageFieldMap,
                                                                     totalCpuUsageFieldMap,
                                                                     sqlServerCPUUsage,
                                                                     vmCPUUsage,
                                                                     hostCPUUsage
                                                                     });
            else
                cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     vmCpuUsageFieldMap,
                                                                     esxCpuUsageFieldMap,
                                                                     sqlServerCpuUsageFieldMap,
                                                                     totalCpuUsageFieldMap

                                                                 });
            //START: SQLdm 10.0 (Tarun Sapra)- Displaying baselines 

            ChartFxExtensions.SetAreaSeriesAlphaChannel(cpuUsageChart, 175);
            cpuUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cpuUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            cpuUsageChart.AxisY.Min = 0;
            cpuUsageChart.AxisY.Max = 100;
            ///Check for historicl data --Ankit Nagpal SQLdm 10.0
            if (historyData.HistoricalSnapshotDateTime == null)

                cpuUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else cpuUsageChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }

        public void InitializeProcessorQueueLengthChart()
        {
            processorQueueLengthChart.Tag = processorQueueLengthHeaderStripLabel;
            processorQueueLengthChart.Printer.Orientation = PageOrientation.Landscape;
            processorQueueLengthChart.Printer.Compress = true;
            processorQueueLengthChart.Printer.ForceColors = true;
            processorQueueLengthChart.Printer.Document.DocumentName = "Processor Queue Length Chart";
            processorQueueLengthChart.ToolBar.RemoveAt(0);
            processorQueueLengthChart.ToolTipFormat = "%v items queued\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap processorQueueLenthFieldMap = new FieldMap("ProcessorQueueLength", FieldUsage.Value);
            processorQueueLenthFieldMap.DisplayName = "Total";
            //START: SQLdm 10.0 (Tarun Sapra) - Added fieldMap for the baseline plot
            FieldMap osProcessorQueueLengthBaseline = new FieldMap("OSProcessorQueueLengthBaseline", FieldUsage.Value);
            osProcessorQueueLengthBaseline.DisplayName = "Baseline-Processor Queue Length";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            processorQueueLengthChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
                processorQueueLengthChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                                 {
                                                                                 dateFieldMap,
                                                                                 processorQueueLenthFieldMap,
                                                                                 osProcessorQueueLengthBaseline
                                                                                 });
            else
                processorQueueLengthChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                                 {
                                                                                 dateFieldMap,
                                                                                 processorQueueLenthFieldMap
                                                                                 });
            //END: SQLdm 10.0 (Tarun Sapra) - Added fieldMap for the baseline plot
            ChartFxExtensions.SetAreaSeriesAlphaChannel(processorQueueLengthChart, 175);
            processorQueueLengthChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            processorQueueLengthChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            if (historyData.HistoricalSnapshotDateTime == null)
                processorQueueLengthChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                processorQueueLengthChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }

        private void InitializeProcessorTimeChart()
        {
            processorTimeChart.Tag = processorTimeHeaderLabel;
            processorTimeChart.Printer.Orientation = PageOrientation.Landscape;
            processorTimeChart.Printer.Compress = true;
            processorTimeChart.Printer.ForceColors = true;
            processorTimeChart.Printer.Document.DocumentName = "Processor Time Chart";
            processorTimeChart.ToolBar.RemoveAt(0);
            processorTimeChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap privilegedTimeFieldMap = new FieldMap("PercentPrivilegedTime", FieldUsage.Value);
            privilegedTimeFieldMap.DisplayName = "Privileged Time";
            FieldMap userTimeFieldMap = new FieldMap("PercentUserTime", FieldUsage.Value);
            userTimeFieldMap.DisplayName = "User Time";
            //START: SQLdm 10.0 (Tarun Sapra) - Added fieldMaps for the baseline plot
            FieldMap osPrivilegeTimeBaseline = new FieldMap("OSPrivilegeTimeBaseline", FieldUsage.Value);
            osPrivilegeTimeBaseline.DisplayName = "Baseline-Privileged Time";
            FieldMap osUserTimeBaseline = new FieldMap("OSUserTimeBaseline", FieldUsage.Value);
            osUserTimeBaseline.DisplayName = "Baseline-User Time";
            FieldMap osProcessorTimeBaseline = new FieldMap("OSProcessorTimeBaseline", FieldUsage.Value);
            osProcessorTimeBaseline.DisplayName = "Baseline-OS Processor Time";

            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            processorTimeChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
                processorTimeChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                          {
                                                                          dateFieldMap,
                                                                          privilegedTimeFieldMap,
                                                                          userTimeFieldMap,
                                                                          osPrivilegeTimeBaseline,
                                                                          osUserTimeBaseline,
                                                                          osProcessorTimeBaseline
                                                                          });
            else
                processorTimeChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                      {
                                                                          dateFieldMap,
                                                                          privilegedTimeFieldMap,
                                                                          userTimeFieldMap
                                                                      });
            //START: SQLdm 10.0 (Tarun Sapra) - Added fieldMap for the baseline plot

            processorTimeChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            processorTimeChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            processorTimeChart.AxisY.Min = 0;
            processorTimeChart.AxisY.Max = 100;
            if (historyData.HistoricalSnapshotDateTime == null)
                processorTimeChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else processorTimeChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeCPUCreditUsageChart()
        {
            cPUCreditUsageChart.Tag = cPUCreditUsageHeaderStripLabel;
            cPUCreditUsageChart.Printer.Orientation = PageOrientation.Landscape;
            cPUCreditUsageChart.Printer.Compress = true;
            cPUCreditUsageChart.Printer.ForceColors = true;
            cPUCreditUsageChart.Printer.Document.DocumentName = "CPU Credit Usage Chart";
            cPUCreditUsageChart.ToolBar.RemoveAt(0);
            cPUCreditUsageChart.ToolTipFormat = "%s\n%v Count\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap cPUCreditUsageFieldMap = new FieldMap("CPUCreditUsage", FieldUsage.Value);
            cPUCreditUsageFieldMap.DisplayName = "CPU Credit Usage";


            cPUCreditUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      cPUCreditUsageFieldMap,
                                                                  });

            cPUCreditUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cPUCreditUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                cPUCreditUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                cPUCreditUsageChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeCpuCreditBalanceChart()
        {
            cpuCreditBalanceChart.Tag = cpuCreditBalanceHeaderStripLabel;
            cpuCreditBalanceChart.Printer.Orientation = PageOrientation.Landscape;
            cpuCreditBalanceChart.Printer.Compress = true;
            cpuCreditBalanceChart.Printer.ForceColors = true;
            cpuCreditBalanceChart.Printer.Document.DocumentName = "CPU Credit Balance Chart";
            cpuCreditBalanceChart.ToolBar.RemoveAt(0);
            cpuCreditBalanceChart.ToolTipFormat = "%s\n%v Count\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap CPUCreditBalanceFieldMap = new FieldMap("CPUCreditBalance", FieldUsage.Value);
            CPUCreditBalanceFieldMap.DisplayName = "CPU Credit Balance";
         

            cpuCreditBalanceChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      CPUCreditBalanceFieldMap,
                                                                  });

            cpuCreditBalanceChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cpuCreditBalanceChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                cpuCreditBalanceChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                cpuCreditBalanceChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeCallRatesChart()
        {
            callRatesChart.Tag = callRatesHeaderStripLabel;
            callRatesChart.Printer.Orientation = PageOrientation.Landscape;
            callRatesChart.Printer.Compress = true;
            callRatesChart.Printer.ForceColors = true;
            callRatesChart.Printer.Document.DocumentName = "Call Rates Chart";
            callRatesChart.ToolBar.RemoveAt(0);
            callRatesChart.ToolTipFormat = "%v %s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap batchesFieldMap = new FieldMap("CallRatesBatches", FieldUsage.Value);
            batchesFieldMap.DisplayName = "Batches";
            FieldMap compilesFieldMap = new FieldMap("CallRatesCompiles", FieldUsage.Value);
            compilesFieldMap.DisplayName = "Compiles";
            FieldMap recompilesFieldMap = new FieldMap("CallRatesReCompiles", FieldUsage.Value);
            recompilesFieldMap.DisplayName = "Re-Compiles";
            FieldMap transactionsFieldMap = new FieldMap("CallRatesTransactions", FieldUsage.Value);
            transactionsFieldMap.DisplayName = "Transactions";

            callRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      compilesFieldMap,
                                                                      recompilesFieldMap,
                                                                      batchesFieldMap,
                                                                      transactionsFieldMap
                                                                  });

            callRatesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            callRatesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                callRatesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                callRatesChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeMaxWorkerChart()
        {
            maxWorkerChart.Printer.Orientation = PageOrientation.Landscape;
            maxWorkerChart.Printer.Compress = true;
            maxWorkerChart.Printer.ForceColors = true;
            maxWorkerChart.Printer.Document.DocumentName = "Max Worker Chart";

        }

        private void ConfigureChartDataSource(DataTable dataSource)
        {
            if (cpuUsageChart.DataSource == dataSource) return;

                cpuUsageChart.DataSource =
                processorQueueLengthChart.DataSource =
                processorTimeChart.DataSource =
                callRatesChart.DataSource =
                maxWorkerChart.DataSource=
                cpuCreditBalanceChart.DataSource=
                cPUCreditUsageChart.DataSource=
                 dataSource;

            ForceChartColors();
        }

        private void InitializeDatabasesGrid()
        {
            topDatabasesGrid.DrawFilter = new PercentageBackgroundDrawFilter();
        }
        private void CreateChartDataSource()
        {

            top5DataTable = new DataTable();
            top5DataTable.Columns.Add("Date", typeof(DateTime));

            top5DataTable.Columns.Add("MaxWorker1", typeof(decimal));
            top5DataTable.Columns.Add("MaxWorker2", typeof(decimal));
            top5DataTable.Columns.Add("MaxWorker3", typeof(decimal));
            top5DataTable.Columns.Add("MaxWorker4", typeof(decimal));
            top5DataTable.Columns.Add("MaxWorker5", typeof(decimal));

            top5DataTable.PrimaryKey = new DataColumn[] { top5DataTable.Columns["Date"] };

            top5DataTable.DefaultView.Sort = "Date";
        }
        private void ConfigureDatabasesGrid(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                topDatabasesGrid.SuspendLayout();

                if (dataSource == null || dataSource.DefaultView == null)
                    return;
                DataView sourceView = new DataView(dataSource);
                DataTable lastSnapshot = sourceView.Table.Clone();

                if (sourceView.Count > 0)
                {
                    lastSnapshot.BeginLoadData();
                    DataRowView lastRow = sourceView[sourceView.Count - 1];
                    DateTime match = (DateTime)lastRow["Date"];

                    for (int i = sourceView.Count - 1; i >= 0; i--)
                    {
                        DataRowView row = sourceView[i];
                        if (match.Equals(row["Date"]))
                            lastSnapshot.ImportRow(row.Row);
                        else
                            break;
                    }
                    lastSnapshot.EndLoadData();
                }

                // sort descending by selected column
                DataTable dataTable = new DataTable("AzureCloudMetricsMaxWorker");

                DataColumn databaseColumn = new DataColumn();
                databaseColumn.DataType = System.Type.GetType("System.String");
                databaseColumn.ColumnName = "Database";
                databaseColumn.Caption = "Database";
                databaseColumn.ReadOnly = true;
                databaseColumn.Unique = true;
                dataTable.Columns.Add(databaseColumn);

                DataColumn metricValueColumn = new DataColumn();
                metricValueColumn.DataType = System.Type.GetType("System.Decimal");
                metricValueColumn.ColumnName = "MaxWorkerPct";
                metricValueColumn.Caption = "MaxWorkerPct";
                metricValueColumn.ReadOnly = false;
                metricValueColumn.Unique = false;
                dataTable.Columns.Add(metricValueColumn);

                DataView dataView = new DataView(dataSource);
                foreach (DataRowView row in dataView)
                {
                    if (row["AzureCloudMetrics"] != DBNull.Value && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Count > 0)
                    {
                        Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                        foreach (string dbName in keyColl)
                        {
                            bool exists = false;
                            foreach (DataRow dr in dataTable.Rows) // search whole table
                            {
                                if (dr["Database"].Equals(dbName))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("MaxWorkerPercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxWorkerPercent"] != DBNull.Value)
                                        {
                                            dr["MaxWorkerPct"] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxWorkerPercent"]);
                                            exists = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!exists)
                            {
                                DataRow theDataRow = dataTable.NewRow();

                                // Add data
                                if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("MaxWorkerPercent"))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxWorkerPercent"] != DBNull.Value)
                                    {
                                        theDataRow[0] = dbName;
                                        theDataRow[1] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxWorkerPercent"]);
                                        dataTable.Rows.Add(theDataRow);

                                        dataTable.AcceptChanges();
                                    }
                                }
                            }


                        }
                    }
                }
                DataSet gridDataSet = new DataSet();
                DataView dv = dataTable.DefaultView;
                dv.Sort = "MaxWorkerPct desc";
                gridDataSet.Tables.Add(dv.ToTable());
                topDatabasesGrid.DataSource = gridDataSet;

                foreach (UltraGridColumn col in topDatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = false;
                }

               

                topDatabasesGrid.ResumeLayout();
            }
        }

        internal void ConfigureDataSourceForMaxWorker(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {

                try
                {
                    top5DataTable.BeginLoadData();
                    top5DataTable.Rows.Clear();
                    DataTable dataTableMetrics = dataSource;

                    if (dataTableMetrics != null)
                    {
                        DataView currentData = new DataView(dataTableMetrics);

                        DateTime top5RowDate = DateTime.MinValue;
                        DataRow top5Row = null;



                        foreach (DataRowView row in currentData)
                        {
                            if (row["AzureCloudMetrics"] != DBNull.Value && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Count > 0)
                            {
                                DateTime date = (DateTime)row["Date"];
                                if (top5Row == null || date != top5RowDate)
                                {
                                    top5Row = top5DataTable.NewRow();
                                    top5Row["Date"] = date;
                                    if (top5DataTable.Rows.Contains(date))
                                    {
                                        top5DataTable.Rows.Remove(top5DataTable.Rows.Find(date));
                                    }
                                    top5DataTable.Rows.Add(top5Row);
                                    top5RowDate = date;
                                }

                                Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                                foreach (string s in keyColl)
                                {
                                    if (!uniqueDatabases.Contains(s))
                                    {
                                        selectedDatabases.Add(s);
                                        uniqueDatabases.Add(s);
                                    }
                                }

                                for (int i = 0; i < (selectedDatabases.Count > 5 ? 5 : selectedDatabases.Count); i++)
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(selectedDatabases[i]) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]].ContainsKey("MaxWorkerPercent"))
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["MaxWorkerPercent"] != DBNull.Value)
                                        {
                                            top5Row["MaxWorker" + (i + 1)] = ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["MaxWorkerPercent"];
                                            dbValMap["MaxWorker" + (i + 1)] = selectedDatabases[i];
                                        }
                                }
                            }
                            
                        }
                    }
                    // update the grid
                    ConfigureDatabasesGrid(dataSource);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 databases", ex);
                }
                finally
                {
                    top5DataTable.EndLoadData();
                }
                maxWorkerChart.DataSource = top5DataTable;
                ChartFxExtensions.SetAxisXTimeScale(maxWorkerChart, 2);
                ConfigureDatabasesChart();

                Invalidate();
            }
        }
        private void ConfigureDatabasesChart()
        {
            if (selectedDatabases.Count == 0)
            {
                maxWorkerChart.Visible = false;
                return;
            }
            else
                maxWorkerChart.Visible = true;
            maxWorkerChart.SuspendLayout();
            maxWorkerChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            maxWorkerChart.DataSourceSettings.Fields.Add(dateFieldMap);

            if (selectedDatabases.Count > 0)
            {
                for (int idx = 1; idx <= (selectedDatabases.Count <= 5 ? selectedDatabases.Count : 5); idx++)
                {
                    if (dbValMap.ContainsKey("MaxWorker" + idx))
                    {
                        string database = selectedDatabases[idx - 1];
                        FieldMap fieldMap = new FieldMap("MaxWorker" + idx, FieldUsage.Value);
                        fieldMap.DisplayName = dbValMap["MaxWorker" + idx];
                        maxWorkerChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                maxWorkerChart.DataSourceSettings.ReloadData();
                databaseChart_Resize(maxWorkerChart, new EventArgs());
                int showDecimals = 4;
                maxWorkerChart.AxisY.DataFormat.Decimals = showDecimals;
                maxWorkerChart.Invalidate();
            }
            maxWorkerChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
            maxWorkerChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            maxWorkerChart.ToolTipFormat = "%s\n%v \n%x";
            maxWorkerChart.ResumeLayout();
        }
        private void databaseChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 3;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }
        #endregion

        #region Refresh Data

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();

                //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
                if (!isUserSysAdmin)
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, false);
                else
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, false);
            }
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
            if (HistoricalSnapshotDateTime == null)
            {
                if (!historyData.RealTimeSnapshotsPrePopulated)
                    historyData.PrePopulateRealTimeSnapshots(instanceId, this);
                else
                {
                    historyData.BackfillScheduledRefreshData(instanceId, this);

                    // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (previousVisibleLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.ForwardFillHistoricalData(instanceId, this);

                    // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.BackFillScheduledHistoricalData(instanceId, this);
                }
                if (backgroundWorker.CancellationPending) return null;

                // this will refresh the current realtime data on entry and switching from history to realtime
                backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

                if (backgroundWorker.CancellationPending) return null;

                // go fetch the realtime snapshot
                return GetRealTimeSnapshots();
            }
            else
            {
                // If historical start/end did not change global, the snapshot is already populated.
                if (!historyData.HistoricalSnapshotsPopulated)
                    return historyData.PopulateHistoricalSnapshots(instanceId, this, backgroundWorker);
                else
                    return historyData.HistoricalSnapshots;
            }
        }

        protected override void UpdateProgress(object state, int progress)
        {
            using (Log.VerboseCall("UpdateProgress"))
            {
                switch (progress)
                {
                    case (int)Progress.Backfill:
                        UpdateChartDataFilter();
                        if (historyData.RealTimeSnapshotsDataTable.DefaultView.Count == 0) return;
                        operationalStatusPanel.Visible = false;
                        fillPanel.Visible = true;
                        ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        ConfigureDataSourceForMaxWorker(historyData.RealTimeSnapshotsDataTable);
                        ChartsVisible(true);
                        break;
                }
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                fillPanel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        private ServerSummarySnapshots GetRealTimeSnapshots()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            ServerOverviewConfiguration serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, historyData.LastServerOverviewSnapshot);
            if (serverOverviewSnapshotConfiguration.LastRefresh == null)
            {
                ServerSummarySnapshots summarySnapshots = managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
                if (summarySnapshots != null)
                {
                    serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, summarySnapshots.ServerOverviewSnapshot);
                }
            }

            return managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    UpdateWithRealTimeSnapshots(data as ServerSummarySnapshots);
                }
                else
                {
                    UpdateWithHistoricalSnapshots(data as ServerSummarySnapshots);
                }

                ShowVMData(historyData.IsVM);
            }
        }

        private void UpdateWithRealTimeSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            operationalStatusPanel.Visible = false;
            fillPanel.Visible = true;

            if (summarySnapshots != null)
            {
                historyData.AddRealTimeSnapshots(summarySnapshots);

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    if (summarySnapshots.ServerOverviewSnapshot.Error == null)
                    {
                        ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        ConfigureDataSourceForMaxWorker(historyData.RealTimeSnapshotsDataTable);
                        UpdateChartDataFilter();
                        ChartsVisible(true);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, false);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, summarySnapshots.ServerOverviewSnapshot.Error));
                    }
                }
            }
        }

        private void UpdateWithHistoricalSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                fillPanel.Visible = true;
                ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable);
                ConfigureDataSourceForMaxWorker(historyData.HistoricalSnapshotsDataTable); 
                ChartsVisible(true);
                ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                      Properties.Resources.StatusWarningSmall);
                currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                currentHistoricalStartDateTime = HistoricalStartDateTime;
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                fillPanel.Visible = false;
            }

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        private void UpdateChartDataFilter()
        {
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        #endregion

        #region Charts

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            tableLayoutPanel.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            chartContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            chartContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            tableLayoutPanel.Visible = true;
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
               // SetRegularRDSResourcesPanel();
                SetAmazonRDSResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            { 
                SetAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                SetManagedAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                SetRegularRDSResourcesPanel();
            }
        }
        public void SetAmazonRDSResourcesPanel()
        {
            //this.tableLayoutPanel.Controls.Clear();
            //this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 0, 0);
            //this.tableLayoutPanel.Controls.Add(this.cpuCreditBalancePanel, 0, 1);
            //this.tableLayoutPanel.ColumnCount = 1;
            //this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100.00F));
            //this.tableLayoutPanel.RowCount = 1;
            //this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            //SQLdm 30843 missing titles for cpu graphs
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.cPUCreditUsagePanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cpuCreditBalancePanel, 0, 1);
            setBackgroundColor();
        }

        public void SetAzureResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.maxWorkerpanel, 1 , 0);
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            setBackgroundColor();
        }

        public void SetManagedAzureResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            //SQLdm 30843 missing titles for cpu graphs
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cpuUsagePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.maxWorkerpanel, 0, 1);
            setBackgroundColor();
        
            
        }
        private void setBackgroundColor()
        {
            this.maxWorkerpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.maxWorkerpanel.Padding = new System.Windows.Forms.Padding(1, 0, 2, 1);
        }
      
        private void maximizeCpuUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cpuUsagePanel, maximizeCpuUsageChartButton, restoreCpuUsageChartButton);
        }

        private void restoreCpuUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cpuUsagePanel, maximizeCpuUsageChartButton, restoreCpuUsageChartButton, 0, 0);
        }

        private void maximizeProcessorQueueLengthChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(processorQueueLengthPanel, maximizeProcessorQueueLengthChartButton,
                          restoreProcessorQueueLengthChartButton);
        }

        private void restoreProcessorQueueLengthChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(processorQueueLengthPanel, maximizeProcessorQueueLengthChartButton,
                         restoreProcessorQueueLengthChartButton, 1, 0);
        }

        private void maximizeProcessorTimeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(processorTimePanel, maximizeProcessorTimeChartButton, restoreProcessorTimeChartButton);
        }

        private void restoreProcessorTimeChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(processorTimePanel, maximizeProcessorTimeChartButton, restoreProcessorTimeChartButton, 0, 1);
        }

        private void maximizeCallRatesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(callRatesPanel, maximizeCallRatesChartButton, restoreCallRatesChartButton);
        }
        private void restoreCallRatesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(callRatesPanel, maximizeCallRatesChartButton, restoreCallRatesChartButton, 1, 1);
        }
        private void maximizeCpuCreditBalanceChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cpuCreditBalancePanel, maximizeCpuCreditBalanceChartButton, restoreCpuCreditBalanceChartButton);
        }
       
        private void restoreCpuCreditBalanceChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cpuCreditBalancePanel, maximizeCpuCreditBalanceChartButton, restoreCpuCreditBalanceChartButton, 1, 1);
        }

        private void maximizeCPUCreditUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cPUCreditUsagePanel, maximizeCPUCreditUsageChartButton, restoreCPUCreditUsageChartButton);
        }

        private void restoreCPUCreditUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cPUCreditUsagePanel, maximizeCPUCreditUsageChartButton, restoreCPUCreditUsageChartButton, 1, 1);
        }
        #endregion

        #region toolbar

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (contextMenuSelectedChart != null)
            {
                switch (e.Tool.Key)
                {
                    case "toggleChartToolbarButton":
                        ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                        break;
                    case "printChartButton":
                        PrintChart(contextMenuSelectedChart);
                        break;
                    case "exportChartDataButton":
                        SaveChartData(contextMenuSelectedChart);
                        break;
                    case "exportChartImageButton":
                        SaveChartImage(contextMenuSelectedChart);
                        break;
                }
            }
            contextMenuSelectedChart = null;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
            }
        }

        #endregion

        #region Operational Status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(212, 212, 212);
            operationalStatusImage.BackColor = Color.FromArgb(212, 212, 212);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            // Switching to real-time mode is the only supported opertional action at this time
            SwitchToRealTimeMode();
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;

            //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
            if (!isUserSysAdmin)
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, true);
            else
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, true);
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void ChartsVisible(bool value)
        {
            cpuUsageChart.Visible =
            processorQueueLengthChart.Visible =
            processorTimeChart.Visible =
            callRatesChart.Visible =
            cpuCreditBalanceChart.Visible =
            cPUCreditUsageChart.Visible =
            maxWorkerChart.Visible = value;
        }

        private void ShowChartStatusMessage(string message, bool forceHide)
        {
            if (forceHide)
            {
                ChartsVisible(false);
            }

            cpuUsageChartStatusLabel.Text =
                processorQueueLengthChartStatusLabel.Text =
                processorTimeChartStatusLabel.Text =
                callRatesChartStatusLabel.Text =
                cpuCreditBalanceChartStatusLabel.Text=
                cPUCreditUsageChartStatusLabel.Text= message;
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        #endregion

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void maximizeMaxWorkerChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(maxWorkerpanel, maximizeMaxWorkerChartButton, restoreMaxWorkerChartButton);

        }

        private void restoreMaxWorkerChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(maxWorkerpanel, maximizeMaxWorkerChartButton, restoreMaxWorkerChartButton,0,2);
        }
    }
   
}