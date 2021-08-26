using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using System.Globalization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesDiskView : ServerBaseView
    {
        private readonly ServerSummaryHistoryData historyData;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Exception historyModeLoadError = null;

        private ChartType initialChartType1 = ChartType.DiskBusyPerDisk;
        private ChartType initialChartType2 = ChartType.DiskTimePerReadPerDisk;
        private ChartType initialChartType3 = ChartType.DiskSqlIO;
        private ChartType initialChartType4 = ChartType.DiskTimePerWritePerDisk;

        private bool updateDiskSeries = false;
        private bool isVM = false;

        private List<string> overviewDiskKeys = new List<string>(20);
        // For the Avg Data IO
        private DataTable top5DataIODataTable;
        private List<string> selectedDataIODatabases = new List<string>();
        private HashSet<string> uniqueDataIODatabases = new HashSet<String>();
        private Dictionary<string, string> dbValMapDataIO = new Dictionary<string, string>();


        // For the Avg Log IO
        private DataTable top5LogIODataTable;
        private List<string> selectedLogIODatabases = new List<string>();
        private HashSet<string> uniqueLogIODatabases = new HashSet<String>();
        private Dictionary<string, string> dbValMapLogIO = new Dictionary<string, string>();


        public ResourcesDiskView(int instanceId, ServerSummaryHistoryData historyData) : base(instanceId)
        {
            // RefreshReportsProgress = true;
            this.historyData = historyData;

            var wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
            isVM = wrapper.Instance.IsVirtualized;

            InitializeComponent();

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                SetAmazonRDSResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId|| ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId))
            {
                SetAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                SetRegularRDSResourcesPanel();
            }
            ChartFxExtensions.SetContextMenu(averageDataIOChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(averageLogIOChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(WriteThroughputChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(WriteLatencyChart, toolbarsManager); 
            ChartFxExtensions.SetContextMenu(DiskQueueDepthChart, toolbarsManager);
            // Grid Initialisation
            InitializeDatabasesGrid();
            setBackgroundColor();
            //Average Data IO Chart
            CreateDataIOChartDataSource();
            InitializeAvgDataIOChart();

            //AverageLogIOChart
            CreateLogIOChartDataSource();
            InitializeAvgLogIOChart();


            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            wrapper.Changed += instance_Changed;
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        public void SetAmazonRDSResourcesPanel()
        {
            //this.tableLayoutPanel.Controls.Clear();
            //this.tableLayoutPanel.Controls.Add(this.chartPanel3, 0, 0);
            //this.tableLayoutPanel.ColumnCount = 1;
            //this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Percent, 100.00F));
            //this.tableLayoutPanel.RowCount = 1;
            //this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            //SQLdm 30843 missing titles for cpu graphs
            this.tableLayoutPanel.Controls.Add(this.chartPanel3, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.WriteLatencyPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.WriteThroughputPanel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.DiskQueueDepthPanel, 0,1);
            setBackgroundColor();
        }
        public void SetRegularRDSResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.Controls.Add(this.chartPanel1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.chartPanel2, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.chartPanel3, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.chartPanel4, 1, 1);
          

        }
        public void SetAzureResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.Controls.Add(this.chartPanel3, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.averageDataIOPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.averageLogIOPanel, 1, 1);
            this.tableLayoutPanel.SetColumnSpan(this.chartPanel3, 2);

        }
        void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            updateDiskSeries = true;
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

        public bool IsVM
        {
            get { return isVM; }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
                ///Baseline enabled event handler--Ankit Nagpal SQLdm 10.0
                case "EnableBaseline":
                    chartPanel1.ConfiureChart();
                    chartPanel2.ConfiureChart();
                    chartPanel4.ConfiureChart();
                    break;
            }
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesDiskView);
        }

        public override void ApplySettings()
        {
            // Get the saved chart types
            try
            {
                ChartType type = (ChartType)Settings.Default.ResourcesDiskViewChart1Type;
                if (type != ChartType.Unknown)
                {
                    initialChartType1 = type;
                }

                type = (ChartType)Settings.Default.ResourcesDiskViewChart2Type;
                if (type != ChartType.Unknown)
                {
                    initialChartType2 = type;
                }

                type = (ChartType)Settings.Default.ResourcesDiskViewChart3Type;
                if (type != ChartType.Unknown)
                {
                    initialChartType3 = type;
                }

                type = (ChartType)Settings.Default.ResourcesDiskViewChart4Type;
                if (type != ChartType.Unknown)
                {
                    initialChartType4 = type;
                }
            }
            catch { }
        }

        public override void SaveSettings()
        {
            Settings.Default.ResourcesDiskViewChart1Type = (int)chartPanel1.ChartType;
            Settings.Default.ResourcesDiskViewChart2Type = (int)chartPanel2.ChartType;
            Settings.Default.ResourcesDiskViewChart3Type = (int)chartPanel3.ChartType;
            Settings.Default.ResourcesDiskViewChart4Type = (int)chartPanel4.ChartType;
        }

        #region Initialize Charts

        private void InitializeCharts()
        {
            List<ChartType> selections = ChartHelper.GetDiskChartTypes(IsVM);

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                selections.Remove(ChartType.DiskBusyTotal);
                selections.Remove(ChartType.DiskBusyPerDisk);
                selections.Remove(ChartType.DiskQueueTotal);
                selections.Remove(ChartType.DiskQueuePerDisk);
                selections.Remove(ChartType.DiskReadsPerSecPerDisk);
                selections.Remove(ChartType.DiskTransfersPerSecPerDisk);
                selections.Remove(ChartType.DiskWritesPerSecPerDisk);
                selections.Remove(ChartType.DiskTimePerReadPerDisk);
                selections.Remove(ChartType.DiskTimePerTransferPerDisk);
                selections.Remove(ChartType.DiskTimePerWritePerDisk);
                if (IsVM)
                {
                    selections.Remove(ChartType.DiskVMTransferRate);
                    selections.Remove(ChartType.DiskHostTransferRate);
                }

                InitializeChart(selections, chartPanel3, initialChartType3);
                chartPanel3.ShowFileActivityMenuOption = true;
                chartPanel3.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                ForceChartColors();
                InitializeThroughputChart();
                InitializeDiskQueueDepthChart();
                InitializeLatencyChart();
                InitalizeDrilldown(chartPanel3.Chart, WriteThroughputChart, WriteLatencyChart, DiskQueueDepthChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
               
            }
            else if(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId||
                ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                selections.Remove(ChartType.DiskBusyTotal);
                selections.Remove(ChartType.DiskBusyPerDisk);
                selections.Remove(ChartType.DiskQueueTotal);
                selections.Remove(ChartType.DiskQueuePerDisk);
                selections.Remove(ChartType.DiskReadsPerSecPerDisk);
                selections.Remove(ChartType.DiskTransfersPerSecPerDisk);
                selections.Remove(ChartType.DiskWritesPerSecPerDisk);
                selections.Remove(ChartType.DiskTimePerReadPerDisk);
                selections.Remove(ChartType.DiskTimePerTransferPerDisk);
                selections.Remove(ChartType.DiskTimePerWritePerDisk);
                if (IsVM)
                {
                    selections.Remove(ChartType.DiskVMTransferRate);
                    selections.Remove(ChartType.DiskHostTransferRate);
                }
                InitializeChart(selections, chartPanel3, initialChartType3);
                chartPanel3.ShowFileActivityMenuOption = true;
                chartPanel3.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                ForceChartColors();
                InitalizeDrilldown(chartPanel3.Chart);

            }
            else
            {
                InitializeChart(selections, chartPanel1, initialChartType1);
                InitializeChart(selections, chartPanel2, initialChartType2);
                InitializeChart(selections, chartPanel3, initialChartType3);
                InitializeChart(selections, chartPanel4, initialChartType4);

                chartPanel1.ShowFileActivityMenuOption = true;
                chartPanel2.ShowFileActivityMenuOption = true;
                chartPanel3.ShowFileActivityMenuOption = true;
                chartPanel4.ShowFileActivityMenuOption = true;

                chartPanel1.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                chartPanel2.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                chartPanel3.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                chartPanel4.ToolbarManager.ToolClick += new ToolClickEventHandler(ToolbarManager_ToolClick);
                ForceChartColors();
                InitalizeDrilldown(chartPanel1.Chart, chartPanel2.Chart, chartPanel3.Chart, chartPanel4.Chart, averageDataIOChart, averageLogIOChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
        }

        void ResetChartTypes()
        {
            List<ChartType> selections = ChartHelper.GetDiskChartTypes(IsVM);

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                selections.Remove(ChartType.DiskBusyTotal);
                selections.Remove(ChartType.DiskBusyPerDisk);
                selections.Remove(ChartType.DiskQueueTotal);
                selections.Remove(ChartType.DiskQueuePerDisk);
                selections.Remove(ChartType.DiskReadsPerSecPerDisk);
                selections.Remove(ChartType.DiskTransfersPerSecPerDisk);
                selections.Remove(ChartType.DiskWritesPerSecPerDisk);
                selections.Remove(ChartType.DiskTimePerReadPerDisk);
                selections.Remove(ChartType.DiskTimePerTransferPerDisk);
                selections.Remove(ChartType.DiskTimePerWritePerDisk);
                if (IsVM)
                {
                    selections.Remove(ChartType.DiskVMTransferRate);
                    selections.Remove(ChartType.DiskHostTransferRate);
                }
                InitializeChart(selections, chartPanel3, selections.Contains(chartPanel3.ChartType) ? chartPanel3.ChartType : ChartType.DiskSqlIO);
            }
            else
            {
                InitializeChart(selections, chartPanel1, selections.Contains(chartPanel1.ChartType) ? chartPanel1.ChartType : ChartType.DiskBusyPerDisk);
                InitializeChart(selections, chartPanel2, selections.Contains(chartPanel2.ChartType) ? chartPanel2.ChartType : ChartType.DiskTimePerReadPerDisk);
                InitializeChart(selections, chartPanel3, selections.Contains(chartPanel3.ChartType) ? chartPanel3.ChartType : ChartType.DiskSqlIO);
                InitializeChart(selections, chartPanel4, selections.Contains(chartPanel4.ChartType) ? chartPanel4.ChartType : ChartType.DiskTimePerWritePerDisk);
            }
        }

        private void ForceChartColors()
        {
            chartPanel1.ForceChartColors();
            chartPanel2.ForceChartColors();
            chartPanel3.ForceChartColors();
            chartPanel4.ForceChartColors();
            WriteThroughputChart.SetAreaSeriesAlphaChannel(175, 0);
            DiskQueueDepthChart.SetAreaSeriesAlphaChannel(175, 0);
            WriteLatencyChart.SetAreaSeriesAlphaChannel(175, 0);
        }

        void ToolbarManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.Key != "showFileActivity")
                return;

            ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.ResourcesFileActivity, new string[] { });
        }

        private void InitializeChart(List<ChartType> selections, ChartPanel chart, ChartType type)
        {
            chart.InitializeChart(selections, type, IsVM);//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
            chart.SetDataSource(historyData, historyData.RealTimeSnapshotsDataTable, historyData.RealTimeDiskDataTable, IsVM);

            //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            drilldownProperties.AddCharts(chart.Chart);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Created new data sources for disk charts.
        /// Added support for chartpanel to use different data source depending upon chart type.
        /// </summary>
        private void ConfigureChartDataSource(DataTable dataSource, DataTable diskDataSource)
        {
            using (Log.VerboseCall("ConfigureChartDataSource"))
            {
               // if (WriteThroughputChart.DataSource == dataSource) return;
                // Make sure that chart columns are configured correctly once there is data
                //SQLdm 10.0 (Tarun Sapra): DE46062-Adding param to display graph in case of hyperV instance
                chartPanel1.CheckChartConfigured(IsVM);
                chartPanel2.CheckChartConfigured(IsVM);
                chartPanel3.CheckChartConfigured(IsVM);
                chartPanel4.CheckChartConfigured(IsVM);

                SetPanelDataSource(chartPanel1, dataSource, diskDataSource);
                SetPanelDataSource(chartPanel2, dataSource, diskDataSource);
                SetPanelDataSource(chartPanel3, dataSource, diskDataSource);
                SetPanelDataSource(chartPanel4, dataSource, diskDataSource);
                WriteThroughputChart.DataSource =
                DiskQueueDepthChart.DataSource =
                WriteLatencyChart.DataSource = dataSource;
            }
        }

        private void SetPanelDataSource(ChartPanel chartPanel, DataTable dataSource, DataTable diskDataSource)
        {
            chartPanel.SetDataSource(dataSource, diskDataSource);
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
                ConfigureDataSourceAverageDataIO(historyData.RealTimeSnapshotsDataTable);
                ConfigureDataSourceAverageLogIO(historyData.RealTimeSnapshotsDataTable);
                //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
                if (!isUserSysAdmin)
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, true);
                else
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, true);
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
                // backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

                // if (backgroundWorker.CancellationPending) return null;

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
                   // switch (progress)
                    //{
                       // case (int)Progress.Backfill:
                            //UpdateChartDataFilter();
                            //if (historyData.RealTimeSnapshotsDataTable.DefaultView.Count == 0) return;
                            //operationalStatusPanel.Visible = false;
                            //fillPanel.Visible = true;
                            //ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                            // ChartsVisible(true);
                            //break;
                    //}
                //}
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

            var overview = managementService.GetServerSummary(serverOverviewSnapshotConfiguration);

            if (updateDiskSeries) return overview;

            if (overview.ServerOverviewSnapshot != null && overview.ServerOverviewSnapshot.DiskDrives != null)
            {
                if (!Algorithms.EqualSets(overviewDiskKeys, overview.ServerOverviewSnapshot.DiskDrives.Keys))
                {
                    Log.Verbose("Disk sets are different.  Forcing charts to reconfigure their series.");
                    updateDiskSeries = true;
                }
            }

            return overview;
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (historyData.IsVM != IsVM)
                {
                    isVM = historyData.IsVM;
                    ResetChartTypes();
                }

                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    UpdateWithRealTimeSnapshots(data as ServerSummarySnapshots);
                }
                else
                {
                    UpdateWithHistoricalSnapshots(data as ServerSummarySnapshots);
                }
            }
        }

        private void UpdateWithRealTimeSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            using (Log.VerboseCall("UpdateWithRealTimeSnapshots"))
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
                            if (updateDiskSeries)
                            {

                                overviewDiskKeys = new List<string>(summarySnapshots.ServerOverviewSnapshot.DiskDrives.Keys);
                                chartPanel1.ForceChartReconfigure(IsVM);
                                chartPanel2.ForceChartReconfigure(IsVM);
                                chartPanel3.ForceChartReconfigure(IsVM);
                                chartPanel4.ForceChartReconfigure(IsVM);
                                updateDiskSeries = false;
                            }

                            // SqlDM 10.2 (Anshul Aggarwal) - Created new data sources for disk charts.
                            // Added support for chartpanel to use different data source depending upon chart type.
                            ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable, historyData.RealTimeDiskDataTable);
                            ConfigureDataSourceAverageDataIO(historyData.RealTimeSnapshotsDataTable);
                            ConfigureDataSourceAverageLogIO(historyData.RealTimeSnapshotsDataTable);

                            UpdateChartDataFilter();
                            ChartsVisible(true);
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                        else
                        {
                            ShowChartStatusMessage("No data available.", false);
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                                        summarySnapshots.ServerOverviewSnapshot.Error));
                        }
                    }
                }
            }
        }

        private void UpdateWithHistoricalSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                fillPanel.Visible = true;
                // SqlDM 10.2 (Anshul Aggarwal) - Created new data sources for disk charts.
                // Added support for chartpanel to use different data source depending upon chart type.
                ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable, historyData.HistoricalDiskDataTable);
                ConfigureDataSourceAverageDataIO(historyData.HistoricalSnapshotsDataTable);
                ConfigureDataSourceAverageLogIO(historyData.HistoricalSnapshotsDataTable);
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
            // SqlDM 10.2 (Anshul Aggarwal) - Created new data sources for disk charts.
            // Added support for chartpanel to use different data source depending upon chart type.
            DateTime viewFilter =
                DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
            if (historyData.RealTimeDiskDataTable != null)
            {
                //SQLDM - 19237 - Anshul Aggarwal - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
                historyData.RealTimeDiskDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeDiskDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
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
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
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

                chartPanel1.ChartVisible =
                chartPanel2.ChartVisible =
                chartPanel3.ChartVisible =
                WriteThroughputChart.Visible =
                WriteLatencyChart.Visible =
                DiskQueueDepthChart.Visible =
                chartPanel4.ChartVisible = value;


        }

        private void ShowChartStatusMessage(string message, bool forceHide)
        {
            if (forceHide)
            {
                ChartsVisible(false);
            }

            chartPanel1.StatusText =
                chartPanel2.StatusText =
                chartPanel3.StatusText =
                chartPanel4.StatusText =
                DiskQueueDepthChartStatusLabel.Text=
                WriteThroughputChartStatusLabel.Text =
                WriteLatencyChartStatusLabel.Text = message;
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

        private void ResourcesDiskView_Load(object sender, EventArgs e)
        {
            ApplySettings();
            InitializeCharts();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.topLogIODatabasesGrid);
            themeManager.updateGridTheme(this.topDataIODatabasesGrid);
        }

        private void chartPanel4_Load(object sender, EventArgs e)
        {

        }
        private void ConfigureLatencyChart(string chartType)
        {
            switch (chartType)
            {
                case "WriteLatency":
                    LatencyDropDownButton.Text = "Write Latency (MilliSeconds)";
                    InitializeWriteLatencyChart();
                    break;
                case "ReadLatency":
                    LatencyDropDownButton.Text = "Read Latency (MilliSeconds)";
                    InitializeReadLatencyChart();
                    break;
            }

            WriteLatencyChart.DataSourceSettings.ReloadData();
        }
        private void ConfigureThroughputChart(string chartType)
        {
            switch (chartType)
            {
                case "WriteThroughput":
                    ThroughputDropDownButton.Text = "Write Throughput (MB/Second)";
                    InitializeWriteThroughputChart();
                    break;
                case "ReadThroughput":
                    ThroughputDropDownButton.Text = "Read Throughput (MB/Second)";
                    InitializeReadThroughputChart();
                    break;
            }

            WriteThroughputChart.DataSourceSettings.ReloadData();
        }
        private void InitializeWriteLatencyChart()
        {
            WriteLatencyChart.DataSourceSettings.Fields.Clear();

            WriteLatencyChart.Printer.Document.DocumentName = "Write Latency (MilliSeconds)";
            WriteLatencyChart.ToolTipFormat = "%s\n%v MilliSecond\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap WriteLatencyFieldMap = new FieldMap("WriteLatency", FieldUsage.Value);
            WriteLatencyFieldMap.DisplayName = "Write Latency";
            //this.WriteLatencyChart.Series.Clear();
       

            WriteLatencyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      WriteLatencyFieldMap,
                                                                  });

            //this.seriesAttributes17.Gallery = Gallery.Lines;

            WriteLatencyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            WriteLatencyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            WriteLatencyChart.AxisY.LabelsFormat.Decimals = 3;
        
        }
        private void InitializeDiskQueueDepthChart()
        {
            DiskQueueDepthChart.Tag = DiskQueueDepthHeaderStripLabel;
            DiskQueueDepthChart.Printer.Orientation = PageOrientation.Landscape;
            DiskQueueDepthChart.Printer.Compress = true;
            DiskQueueDepthChart.Printer.ForceColors = true;
            DiskQueueDepthChart.Printer.Document.DocumentName = "DiskQueueDepth Chart (Count)";
            DiskQueueDepthChart.ToolBar.RemoveAt(0);
            DiskQueueDepthChart.ToolTipFormat = "%s\n%v count\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap DiskQueueDepthFieldMap = new FieldMap("DiskQueueDepth", FieldUsage.Value);
            DiskQueueDepthFieldMap.DisplayName = "Disk Queue Depth";


            DiskQueueDepthChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      DiskQueueDepthFieldMap,
                                                                  });

         
            DiskQueueDepthChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            DiskQueueDepthChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                DiskQueueDepthChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                DiskQueueDepthChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeWriteThroughputChart()
        {
            WriteThroughputChart.DataSourceSettings.Fields.Clear();
            //this.WriteThroughputChart.Series.Clear();
         

            WriteThroughputChart.Printer.Document.DocumentName = "WriteThroughput";
            WriteThroughputChart.ToolTipFormat = "%s\n%v MB/Second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap WriteThroughputFieldMap = new FieldMap("WriteThroughput", FieldUsage.Value);
            WriteThroughputFieldMap.DisplayName = "Write Throughput";


            WriteThroughputChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      WriteThroughputFieldMap,
                                                                  });
            // this.seriesAttributes16.Gallery = Gallery.Lines;

            WriteThroughputChart.AxisY.LabelsFormat.Decimals = 3;
            WriteThroughputChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            WriteThroughputChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
          
        }
        private void ReadLatencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLatencyChart("ReadLatency");

        }

        private void WriteLatencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLatencyChart("WriteLatency");

        }
        private void ReadThroughputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureThroughputChart("ReadThroughput");

        }

        private void WriteThroughputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureThroughputChart("WriteThroughput");
        }
        private void InitializeLatencyChart()
        {
            this.seriesAttributes17.Gallery = Gallery.Lines;

            WriteLatencyChart.Tag = LatencyDropDownButton;
            WriteLatencyChart.Printer.Orientation = PageOrientation.Landscape;
            WriteLatencyChart.Printer.Compress = true;
            WriteLatencyChart.Printer.ForceColors = true;
            WriteLatencyChart.Printer.Document.DocumentName = "WriteLatency";
            WriteLatencyChart.ToolBar.RemoveAt(0);
            InitializeWriteLatencyChart();

            WriteLatencyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            WriteLatencyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                WriteLatencyChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                WriteLatencyChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeThroughputChart()
        {
            this.seriesAttributes16.Gallery = Gallery.Lines;
            WriteThroughputChart.Tag = ThroughputDropDownButton;
            WriteThroughputChart.Printer.Orientation = PageOrientation.Landscape;
            WriteThroughputChart.Printer.Compress = true;
            WriteThroughputChart.Printer.ForceColors = true;
            WriteThroughputChart.Printer.Document.DocumentName = "WriteThroughput";
            WriteThroughputChart.ToolBar.RemoveAt(0);
            InitializeWriteThroughputChart();
          
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                WriteThroughputChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                WriteThroughputChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeReadLatencyChart()
        {
            WriteLatencyChart.DataSourceSettings.Fields.Clear();
            WriteLatencyChart.Printer.Document.DocumentName = "ReadLatency";
            WriteLatencyChart.ToolTipFormat = "%s\n%v MilliSecond\n%x";
            //this.WriteLatencyChart.Series.Clear();
        
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap WriteLatencyFieldMap = new FieldMap("ReadLatency", FieldUsage.Value);
            WriteLatencyFieldMap.DisplayName = "ReadLatency";


            WriteLatencyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      WriteLatencyFieldMap,
                                                                  });
            // this.seriesAttributes17.Gallery = Gallery.Lines;

            WriteLatencyChart.AxisY.LabelsFormat.Decimals = 3;
            WriteLatencyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            WriteLatencyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                WriteLatencyChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                WriteLatencyChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }
        private void InitializeReadThroughputChart()
        {
            WriteThroughputChart.DataSourceSettings.Fields.Clear();
            //this.WriteThroughputChart.Series.Clear();
          
            WriteThroughputChart.Printer.Document.DocumentName = "ReadThroughput";
            WriteThroughputChart.ToolTipFormat = "%s\n%v MB/Second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap WriteThroughputFieldMap = new FieldMap("ReadThroughput", FieldUsage.Value);
            WriteThroughputFieldMap.DisplayName = "ReadThroughput";


            WriteThroughputChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      WriteThroughputFieldMap,
                                                                  });
            // this.seriesAttributes17.Gallery = Gallery.Lines;

            WriteThroughputChart.AxisY.LabelsFormat.Decimals = 3;
            WriteThroughputChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            WriteThroughputChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
           
        }
        private void InitializeDatabasesGrid()
        {
            topDataIODatabasesGrid.DrawFilter = new PercentageBackgroundDrawFilter();
            topLogIODatabasesGrid.DrawFilter= new PercentageBackgroundDrawFilter();
        }
        private void InitializeAvgDataIOChart()
        {
            averageDataIOChart.Printer.Orientation = PageOrientation.Landscape;
            averageDataIOChart.Printer.Compress = true;
            averageDataIOChart.Printer.ForceColors = true;
            averageDataIOChart.Printer.Document.DocumentName = "Average Data IO Chart";

        }
        private void InitializeAvgLogIOChart()
        {
            averageLogIOChart.Printer.Orientation = PageOrientation.Landscape;
            averageLogIOChart.Printer.Compress = true;
            averageLogIOChart.Printer.ForceColors = true;
            averageLogIOChart.Printer.Document.DocumentName = "Average Log IO Chart";

        }
        private void setBackgroundColor()
        {
            this.averageDataIOPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.averageDataIOPanel.Padding = new System.Windows.Forms.Padding(2, 0, 3, 1);

            this.averageLogIOPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.averageLogIOPanel.Padding = new System.Windows.Forms.Padding(2, 1, 3, 2);
        }
        private void CreateDataIOChartDataSource()
        {

            top5DataIODataTable = new DataTable();
            top5DataIODataTable.Columns.Add("Date", typeof(DateTime));

            top5DataIODataTable.Columns.Add("AverageDataIO1", typeof(decimal));
            top5DataIODataTable.Columns.Add("AverageDataIO2", typeof(decimal));
            top5DataIODataTable.Columns.Add("AverageDataIO3", typeof(decimal));
            top5DataIODataTable.Columns.Add("AverageDataIO4", typeof(decimal));
            top5DataIODataTable.Columns.Add("AverageDataIO5", typeof(decimal));

            top5DataIODataTable.PrimaryKey = new DataColumn[] { top5DataIODataTable.Columns["Date"] };

            top5DataIODataTable.DefaultView.Sort = "Date";
        }
        private void CreateLogIOChartDataSource()
        {

            top5LogIODataTable = new DataTable();
            top5LogIODataTable.Columns.Add("Date", typeof(DateTime));

            top5LogIODataTable.Columns.Add("AverageLogIO1", typeof(decimal));
            top5LogIODataTable.Columns.Add("AverageLogIO2", typeof(decimal));
            top5LogIODataTable.Columns.Add("AverageLogIO3", typeof(decimal));
            top5LogIODataTable.Columns.Add("AverageLogIO4", typeof(decimal));
            top5LogIODataTable.Columns.Add("AverageLogIO5", typeof(decimal));

            top5LogIODataTable.PrimaryKey = new DataColumn[] { top5LogIODataTable.Columns["Date"] };

            top5LogIODataTable.DefaultView.Sort = "Date";
        }
        internal void ConfigureDataSourceAverageDataIO(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                try
                {
                    top5DataIODataTable.BeginLoadData();
                    top5DataIODataTable.Rows.Clear();
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
                                    top5Row = top5DataIODataTable.NewRow();
                                    top5Row["Date"] = date;
                                    if (top5DataIODataTable.Rows.Contains(date))
                                    {
                                        top5DataIODataTable.Rows.Remove(top5DataIODataTable.Rows.Find(date));
                                    }
                                    top5DataIODataTable.Rows.Add(top5Row);
                                    top5RowDate = date;
                                }

                                Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                                foreach (string s in keyColl)
                                {
                                    if (!uniqueDataIODatabases.Contains(s))
                                    {
                                        selectedDataIODatabases.Add(s);
                                        uniqueDataIODatabases.Add(s);
                                    }
                                }


                                for (int i = 0; i < (selectedDataIODatabases.Count > 5 ? 5 : selectedDataIODatabases.Count); i++)
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(selectedDataIODatabases[i]) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDataIODatabases[i]].ContainsKey("AverageDataIOPercent"))
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDataIODatabases[i]]["AverageDataIOPercent"] != DBNull.Value)
                                        {
                                            top5Row["AverageDataIO" + (i + 1)] = ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDataIODatabases[i]]["AverageDataIOPercent"];
                                            dbValMapDataIO["AverageDataIO" + (i + 1)] = selectedDataIODatabases[i];
                                        }
                                }
                            }
                           
                        }
                    }

                    // update the grid
                    ConfigureDataIODatabasesGrid(dataSource);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 databases", ex);
                }
                finally
                {
                    top5DataIODataTable.EndLoadData();
                }
                averageDataIOChart.DataSource = top5DataIODataTable;
                ChartFxExtensions.SetAxisXTimeScale(averageDataIOChart, 2);
                ConfigureDataIODatabasesChart();

                Invalidate();
            }
        }
        internal void ConfigureDataSourceAverageLogIO(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {

                try
                {
                    top5LogIODataTable.BeginLoadData();
                    top5LogIODataTable.Rows.Clear();
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
                                    top5Row = top5LogIODataTable.NewRow();
                                    top5Row["Date"] = date;
                                    if (top5LogIODataTable.Rows.Contains(date))
                                    {
                                        top5LogIODataTable.Rows.Remove(top5LogIODataTable.Rows.Find(date));
                                    }
                                    top5LogIODataTable.Rows.Add(top5Row);
                                    top5RowDate = date;
                                }

                                Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                                foreach (string s in keyColl)
                                {
                                    if (!uniqueLogIODatabases.Contains(s))
                                    {
                                        selectedLogIODatabases.Add(s);
                                        uniqueLogIODatabases.Add(s);
                                    }
                                }


                                for (int i = 0; i < (selectedLogIODatabases.Count > 5 ? 5 : selectedLogIODatabases.Count); i++)
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(selectedLogIODatabases[i]) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedLogIODatabases[i]].ContainsKey("AverageLogWritePercent"))
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedLogIODatabases[i]]["AverageLogWritePercent"] != DBNull.Value)
                                        {
                                            top5Row["AverageLogIO" + (i + 1)] = ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDataIODatabases[i]]["AverageLogWritePercent"];
                                            dbValMapLogIO["AverageLogIO" + (i + 1)] = selectedLogIODatabases[i];
                                        }
                                }
                            }

                        }
                    }

                    // update the grid
                    ConfigureLogIODatabasesGrid(dataSource);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 databases", ex);
                }
                finally
                {
                    top5LogIODataTable.EndLoadData();
                }
                averageLogIOChart.DataSource = top5LogIODataTable;
                ChartFxExtensions.SetAxisXTimeScale(averageLogIOChart, 2);
                ConfigureLogIODatabasesChart();

                Invalidate();
            }
        }
        private void ConfigureDataIODatabasesChart()
        {
            if (selectedDataIODatabases.Count == 0)
            {
                averageDataIOChart.Visible = false;
                return;
            }
            else
            {
                averageDataIOChart.Visible = true;
            }
            averageDataIOChart.SuspendLayout();
            averageDataIOChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            averageDataIOChart.DataSourceSettings.Fields.Add(dateFieldMap);

            if (selectedDataIODatabases.Count > 0)
            {
                for (int idx = 1; idx <= (selectedDataIODatabases.Count <= 5 ? selectedDataIODatabases.Count : 5); idx++)
                {
                    if (dbValMapDataIO.ContainsKey("AverageDataIO" + idx))
                    {
                        string database = selectedDataIODatabases[idx - 1];
                        FieldMap fieldMap = new FieldMap("AverageDataIO" + idx, FieldUsage.Value);
                        fieldMap.DisplayName = dbValMapDataIO["AverageDataIO" + idx];
                        averageDataIOChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                averageDataIOChart.DataSourceSettings.ReloadData();
                databaseChart_Resize(averageDataIOChart, new EventArgs());
                int showDecimals = 4;
                averageDataIOChart.AxisY.DataFormat.Decimals = showDecimals;
                averageDataIOChart.Invalidate();
            }
            averageDataIOChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
            averageDataIOChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            averageDataIOChart.ToolTipFormat = "%s\n%v \n%x";
            averageDataIOChart.ResumeLayout();
        }
        private void ConfigureLogIODatabasesChart()
        {
            if (selectedLogIODatabases.Count == 0)
            {
                averageLogIOChart.Visible = false;
                return;
            }
            else
            {
                averageLogIOChart.Visible = true;
            }
            averageLogIOChart.SuspendLayout();
            averageLogIOChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            averageLogIOChart.DataSourceSettings.Fields.Add(dateFieldMap);

            if (selectedLogIODatabases.Count > 0)
            {
                for (int idx = 1; idx <= (selectedLogIODatabases.Count <= 5 ? selectedLogIODatabases.Count : 5); idx++)
                {
                    if (dbValMapLogIO.ContainsKey("AverageLogIO" + idx))
                    {
                        string database = selectedLogIODatabases[idx - 1];
                        FieldMap fieldMap = new FieldMap("AverageLogIO" + idx, FieldUsage.Value);
                        fieldMap.DisplayName = dbValMapLogIO["AverageLogIO" + idx];
                        averageLogIOChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                averageLogIOChart.DataSourceSettings.ReloadData();
                databaseChart_Resize(averageLogIOChart, new EventArgs());
                int showDecimals = 4;
                averageLogIOChart.AxisY.DataFormat.Decimals = showDecimals;
                averageLogIOChart.Invalidate();
            }
            averageLogIOChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
            averageLogIOChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            averageLogIOChart.ToolTipFormat = "%s\n%v \n%x";
            averageLogIOChart.ResumeLayout();
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
        private void ConfigureLogIODatabasesGrid(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                topLogIODatabasesGrid.SuspendLayout();

                if (dataSource == null || dataSource.DefaultView == null)
                    return;

                DataView sourceView = new DataView(dataSource);
                DataTable lastSnapshot = sourceView.Table.Clone();

                // go backwards through the rows
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

                // sort descending by selected 
                DataTable dataTable = new DataTable("AzureCloudMetricsAverageLogIO");

                DataColumn databaseColumn = new DataColumn();
                databaseColumn.DataType = System.Type.GetType("System.String");
                databaseColumn.ColumnName = "Database";
                databaseColumn.Caption = "Database";
                databaseColumn.ReadOnly = true;
                databaseColumn.Unique = true;
                dataTable.Columns.Add(databaseColumn);

                DataColumn metricValueColumn = new DataColumn();
                metricValueColumn.DataType = System.Type.GetType("System.Decimal");
                metricValueColumn.ColumnName = "AvgLogIOPct";
                metricValueColumn.Caption = "AverageLogIOPct";
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
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("AverageLogWritePercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageLogWritePercent"] != DBNull.Value)
                                        {
                                            dr["AvgLogIOPct"] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageLogWritePercent"]);
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
                                if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("AverageLogWritePercent"))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageLogWritePercent"] != DBNull.Value)
                                    {
                                        theDataRow[0] = dbName;
                                        theDataRow[1] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageLogWritePercent"]);
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
                dv.Sort = "AvgLogIOPct desc";
                gridDataSet.Tables.Add(dv.ToTable());
                topLogIODatabasesGrid.DataSource = gridDataSet;

                foreach (UltraGridColumn col in topLogIODatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = false;
                }

                topLogIODatabasesGrid.ResumeLayout();
            }
        }

        private void ConfigureDataIODatabasesGrid(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                topDataIODatabasesGrid.SuspendLayout();

                if (dataSource == null || dataSource.DefaultView == null)
                    return;

                DataView sourceView = new DataView(dataSource);
                DataTable lastSnapshot = sourceView.Table.Clone();

                // go backwards through the rows
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
                DataTable dataTable = new DataTable("AzureCloudMetricsAverageDataIO");

                DataColumn databaseColumn = new DataColumn();
                databaseColumn.DataType = System.Type.GetType("System.String");
                databaseColumn.ColumnName = "Database";
                databaseColumn.Caption = "Database";
                databaseColumn.ReadOnly = true;
                databaseColumn.Unique = true;
                dataTable.Columns.Add(databaseColumn);

                DataColumn metricValueColumn = new DataColumn();
                metricValueColumn.DataType = System.Type.GetType("System.Decimal");
                metricValueColumn.ColumnName = "AvgDataIOPct";
                metricValueColumn.Caption = "AverageDataIOPct";
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
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("AverageDataIOPercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageDataIOPercent"] != DBNull.Value)
                                        {
                                            dr["AvgDataIOPct"] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageDataIOPercent"]);
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
                                if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("AverageDataIOPercent"))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageDataIOPercent"] != DBNull.Value)
                                    {
                                        theDataRow[0] = dbName;
                                        theDataRow[1] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["AverageDataIOPercent"]);
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
                dv.Sort = "AvgDataIOPct desc";
                gridDataSet.Tables.Add(dv.ToTable());
                topDataIODatabasesGrid.DataSource = gridDataSet;

                foreach (UltraGridColumn col in topDataIODatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = false;
                }

                topDataIODatabasesGrid.ResumeLayout();
            }
        }
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

        private void restoreAverageLogIOChartButton_Click(object sender, EventArgs e)
        {
           RestoreChart(averageLogIOPanel, maximizeAverageLogIOChartButton, restoreAverageLogIOChartButton, 0, 2);

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
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId|| ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId))
            {
                SetAzureResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null 
                && (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId|| ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId))
            {
                SetAmazonRDSResourcesPanel();
            }
            setBackgroundColor();
        }

        private void maximizeAverageLogIOChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(averageLogIOPanel, maximizeAverageLogIOChartButton, restoreAverageLogIOChartButton);

        }
        private void maximizeDiskQueueDepthChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(DiskQueueDepthPanel, maximizeDiskQueueDepthChartButton, restoreDiskQueueDepthChartButton);
        }

        private void restoreDiskQueueDepthChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(DiskQueueDepthPanel, maximizeDiskQueueDepthChartButton, restoreDiskQueueDepthChartButton, 1, 1);
        }
        private void maximizeAverageDataIOChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(averageDataIOPanel, maximizeAverageDataIOChartButton, restoreAverageDataIOChartButton);
        }

        private void restoreAverageDataIOChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(averageDataIOPanel, maximizeAverageDataIOChartButton, restoreAverageDataIOChartButton, 1, 2);

        }
        private void maximizeWriteThroughputChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(WriteThroughputPanel, maximizeWriteThroughputChartButton, restoreWriteThroughputChartButton);
        }

        private void restoreWriteThroughputChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(WriteThroughputPanel, maximizeWriteThroughputChartButton, restoreWriteThroughputChartButton, 1, 1);
        }

        private void maximizeWriteLatencyChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(WriteLatencyPanel, maximizeWriteLatencyChartButton, restoreWriteLatencyChartButton);
        }

        private void restoreWriteLatencyChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(WriteLatencyPanel, maximizeWriteLatencyChartButton, restoreWriteLatencyChartButton, 1, 1);
        }
        private void topLogIODatabasesGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }
    }

}
