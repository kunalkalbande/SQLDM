using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerDetailsView : ServerBaseView
    {
        #region constants

        private const string COL_VISIBLE = @"Visible In Chart";
        private const string COL_VALUE = @"Value";
        private const string COL_REFERENCERANGE = @"Reference Range";

        #region ServerDetailsMetrics Enum

        private enum ServerDetailsMetrics
        {
            [Description("DTC Service Status")]
            DtcServiceStatus, // 0
            [Description("Full-Text Service Status")]
            FullTextServiceStatus, // 1
            [Description("Sql Server Agent Service Status")]
            AgentServiceStatus, // 2
            [Description("Sql Server Service Status")]
            SqlServerServiceStatus, // 3
            [Description("Available Memory")]
            AvailableMemory, // 4
            [Description("Used Memory (Percent)")]
            AvailableMemoryPercent, // 5
            [Description("Batches")]
            Batches, // 6
            [Description("Batches Per Second")]
            BatchesPerSecond, // 7
            [Description("Blocked Sessions")]
            BlockedSessions, // 8
            [Description("Buffer Cache Hit Ratio")]
            BufferCacheHitRatio, // 9
            [Description("Buffer Cache Size")]
            BufferCacheSize, // 10
            [Description("Checkpoint Writes")]
            CheckpointWrites, // 11
            [Description("Checkpoint Writes Per Second")]
            CheckpointWritesPerSecond, // 12
            [Description("Client Computers")]
            ClientComputers, // 13
            [Description("Connections")]
            Connections, // 14
            [Description("Connections Per Second")]
            ConnectionsPerSecond, // 15
            [Description("Sql CPU Activity")]
            CPUActivity, // 16
            [Description("Database Count")]
            DatabaseCount, // 17
            [Description("Data File Count")]
            DataFileCount, // 18
            [Description("Data File Space Allocated")]
            DataFileSpaceAllocated, // 19
            [Description("Data File Space Used")]
            DataFileSpaceUsed, // 20
            [Description("Disk Queue Length")]
            DiskQueueLength, // 21
            [Description("Disk Time (Percent)")]
            DiskTime, // 22
            [Description("Full Scans")]
            FullScans, // 23
            [Description("Full Scans Per Second")]
            FullScansPerSecond, // 24
            [Description("Lazy Writer Writes")]
            LazyWriterWrites, // 25
            [Description("Lazy Writer Writes Per Second")]
            LazyWriterWritesPerSecond, // 26
            [Description("Lead Blockers")]
            LeadBlockers, // 27
            [Description("Lock Waits")]
            LockWaits, // 28
            [Description("Lock Waits Per Second")]
            LockWaitsPerSecond, // 29
            [Description("Log File Count")]
            LogFileCount, // 30
            [Description("Log File Space Allocated")]
            LogFileSpaceAllocated, // 31
            [Description("Log File Space Used")]
            LogFileSpaceUsed, // 32
            [Description("Log Flushes")]
            LogFlushes, // 33
            [Description("Log Flushes Per Second")]
            LogFlushesPerSecond, // 34
            [Description("Maintenance Mode")]
            MaintenanceModeEnabled, // 35
            [Description("OLE Automation Status")]
            OleAutomationStatus, // 36
            [Description("Open Transactions")]
            OpenTransactions, // 37
            [Description("OS Metrics Availability")]
            OsMetricsAvailability, // 38
            [Description("OS CPU Privileged Activity")]
            OsCpuPrivilegedActivity, // 39
            [Description("OS CPU User Time")]
            OsCpuUserTime, // 40
            [Description("OS Total Processor Activity")]
            OsTotalProcessorActivity, // 41
            [Description("Packet Errors")]
            PacketErrors, // 42
            [Description("Packet Errors Per Second")]
            PacketErrorsPerSecond, // 43
            [Description("Packets Received")]
            PacketsReceived, // 44
            [Description("Packets Received Per Second")]
            PacketsReceivedPerSecond, // 45
            [Description("Packets Sent")]
            PacketsSent, // 46
            [Description("Packets Sent Per Second")]
            PacketsSentPerSecond, // 47
            [Description("Page Life Expectancy")]
            PageLifeExpectancy, // 48
            [Description("Page Lookups")]
            PageLookups, // 49
            [Description("Page Lookups Per Second")]
            PageLookupsPerSecond, // 50
            [Description("Page Reads")]
            PageReads, // 51
            [Description("Page Reads Per Second")]
            PageReadsPerSecond, // 52
            [Description("Pages Per Second")]
            PagesPerSecond, // 53
            [Description("Page Splits")]
            PageSplits, // 54
            [Description("Page Splits Per Second")]
            PageSplitsPerSecond, // 55
            [Description("Page Writes")]
            PageWrites, // 56 
            [Description("Page Writes Per Second")]
            PageWritesPerSecond, // 57
            [Description("Physical Memory")]
            PhysicalMemory, // 58
            [Description("Procedure Cache Hit Ratio")]
            ProcedureCacheHitRatio, // 59
            [Description("Procedure Cache Size")]
            ProcedureCacheSize, // 60
            [Description("Processor Queue Length")]
            ProcessorQueueLength, // 61
            [Description("Read Ahead Pages")]
            ReadAheadPages, // 62
            [Description("Read Ahead Pages Per Second")]
            ReadAheadPagesPerSecond, // 63
            [Description("Read Write Errors")]
            ReadWriteErrors, // 64
            [Description("Response Time")]
            ResponseTime, // 65
            [Description("Running Since")]
            RunningSince, // 66
            [Description("Server Version")]
            ServerVersion, // 67
            [Description("Sql Compilations")]
            SqlCompilations, // 68
            [Description("Sql Compilations Per Second")]
            SqlCompilationsPerSecond, // 69
            [Description("Sql Memory Allocated")]
            SqlMemoryAllocated,  // 70
            [Description("Sql Memory Used")]
            SqlMemoryUsed,  // 71
            [Description("Sql Memory Used Percent")]
            SqlMemoryUsedPercent,  // 72
            [Description("Sql Recompilations")]
            SqlRecompilations, // 73
            [Description("Sql Recompilations Per Second")]
            SqlRecompilationsPerSecond, // 74
            [Description("System Processes")]
            SystemProcesses, // 75
            [Description("System Processes Consuming CPU")]
            SystemProcessesConsumingCPU, // 76
            [Description("User Connections Percent")]
            UserConnectionsPercent, // 77
            [Description("User Processes")]
            UserProcesses, // 78
            [Description("User Processes Consuming CPU")]
            UserProcessesConsumingCPU, // 79
            [Description("WMI Service Status")]
            WMIStatus, // 80
            [Description("Workfiles Created")]
            WorkfilesCreated, // 81
            [Description("Workfiles Created Per Second")]
            WorkfilesCreatedPerSecond, // 82
            [Description("Worktables Created")]
            WorktablesCreated, // 83
            [Description("Worktables Created Per Second")]
            WorktablesCreatedPerSecond, // 84

            [Description("Number of vCPUs")]
            VmNumCPUs, // 85
            [Description("vCPU Speed Limit")]
            VmCPULimit, // 86
            [Description("vCPU Reserved Speed")]
            VmCPUReserve, // 87
            [Description("VM Memory Size")]
            VmMemSize, // 88
            [Description("VM Memory Limit")]
            VmMemLimit, // 89
            [Description("VM Memory Reserved")]
            VmMemReserved, // 90
            [Description("Host Processor Speed")]
            EsxCPUMHz, // 91
            [Description("Host Physical Processors")]
            EsxNumCPUCores, // 92
            [Description("Host Logical Processors")]
            EsxNumCPUThreads, // 93
            [Description("Host Physical Memory")]
            EsxMemSize, // 94
            [Description("Host NICs")]
            EsxNumNICs, // 95
            [Description("Host Host Boot Time")]
            EsxBootTime, // 96
            [Description("VM Boot Time")]
            VmBootTime, // 97

            [Description("vCPU Usage")]
            VmCPUUsage, // 98
            [Description("vCPU Usage in MHz")]
            VmCPUUsageMHz, // 99
            [Description("vCPU Ready Time")]
            VmCPUReady, // 100
            [Description("vCPU Swap Wait Time")]
            VmCPUSwapWait, // 101
            [Description("VM Memory Swap In Rate")]
            VmMemSwapInRate, // 102
            [Description("VM Memory Swap Out Rate")]
            VmMemSwapOutRate, // 103
            [Description("VM Memory Swapped")]
            VmMemSwapped, // 104
            [Description("VM Active Memory")]
            VmMemActive, // 105
            [Description("VM Consumed Memory")]
            VmMemConsumed, // 106
            [Description("VM Memory Granted")]
            VmMemGranted, // 107
            [Description("VM Ballooned Memory")]
            VmMemBalooned, // 108
            [Description("VM Memory Usage")]
            VmMemUsage, // 109
            [Description("VM Disk Reads")]
            VmDiskRead, // 110
            [Description("VM Disk Writes")]
            VmDiskWrite, // 111
            [Description("VM Disk Usage")]
            VmDiskUsage, // 112
            [Description("VM Network Usage")]
            VmNetUsage, // 113
            [Description("VM Network Transmitted")]
            VmNetTransmitted, // 114
            [Description("VM Network Received")]
            VmNetReceived, // 115
            [Description("Host CPU Usage")]
            EsxCPUUsage, // 116
            [Description("Host CPU Usage in MHz")]
            EsxCPUUsageMHz, // 117
            [Description("Host Memory Swap In Rate")]
            EsxMemSwapInRate, // 118
            [Description("Host Memory Swap Out Rate")]
            EsxMemSwapOutRate, // 119
            [Description("Host Active Memory")]
            EsxMemActive, // 120
            [Description("Host Consumed Memory")]
            EsxMemConsumed, // 121
            [Description("Host Memory Granted")]
            EsxMemGranted, // 122
            [Description("Host Ballooned Memory")]
            EsxMemBalooned, // 123
            [Description("Host Memory Usage")]
            EsxMemUsage, // 124
            [Description("Host Disk Reads")]
            EsxDiskRead, // 125
            [Description("Host Disk Writes")]
            EsxDiskWrite, // 126
            [Description("Host Disk Usage")]
            EsxDiskUsage, // 127
            //[Description("Host Disk Device Latency (ms)")]
            //EsxDiskDeviceLatency, // 128
            //[Description("Host Disk Kernel Latency (ms)")]
            //EsxDiskKernelLatency, // 129
            //[Description("Host Disk Queue Latency (ms)")]
            //EsxDiskQueueLatency, // 130
            //[Description("Host Disk Total Latency (ms)")]
            //EsxDiskTotalLatency, // 131
            [Description("Host Network Usage")]
            EsxNetUsage = 132, // 132
            [Description("Host Network Transmitted")]
            EsxNetTransmitted, // 133
            [Description("Host Network Received")]
            EsxNetReceived, // 134

            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add Service status field
            [Description("SQL Browser Service Status")]
            SQLBrowserServiceStatus, // 135
            [Description("SQL Active Directory Helper Service Status")]
            SQLActiveDirectoryHelperServiceStatus,  // 136
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add Service status field

            // MISC
            [Description("IO Activity Percent")]
            IOActivityPercentage, // 137
            [Description("TempDB Size")]
            TempDBSize, // 138
            [Description("Logins Per Second")]
            LoginsPerSecond // 139
        }

        #endregion

        #region Categories

        private string[] MetricsCategories;
        private const string memoryCategory = "Memory";
        private const string sessionsCategory = "Sessions";
        private const string servicesCategory = "Services";
        private const string processorCategory = "Processor";
        private const string diskIOCategory = "Disk: I/O Activity";
        private const string diskUsageCategory = "Disk: Space Usage";
        private const string sqlActivityCategory = "SQL Activity";
        private const string networkCategory = "Network";
        private const string generalCategory = "General";
        private const string virtualizationCategory = "Virtualization";

        #endregion

        #endregion

        #region enums

        public enum Selection
        {
            All,
            Custom,
            ExcludeVirtualized
        }

        #endregion

        #region fields

        private DataTable detailsGridDataSource;
        private DataTable chartRealTimeDataTable;
        private bool chartRealTimeDataTablePrePopulated = false;
        private ServerOverview currentSnapshot = null;

        private readonly Dictionary<int, Triple<string, DataRow, DataColumn>> lookupTable =
            new Dictionary<int, Triple<string, DataRow, DataColumn>>();
        private readonly Dictionary<int, FieldMap> chartFieldMapLookupTable = new Dictionary<int, FieldMap>();
        private UltraGridColumn selectedColumn = null;
        private bool initialized = false;
        private Control focused = null;
        private static readonly object updateLock = new object();
        private List<string> unselectableMetrics = new List<string>();
        private DataTable historicalStatisticsDataTable = null;

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartVisible = true;
        private DateTime? historicalSnapshotDateTime = null;
        private DataTable chartHistoricalDataTable;
        private Exception historyModeLoadError = null;

        private MetricDefinitions metricDefinitions;
        private List<int> configuredCustomCounters = new List<int>();
        private CustomCounterCollectionSnapshot previousCounterSnapshot;
        private Selection filter;
        private ServerVersion version;
        private bool virtualized = false;
        private bool vmCountersInitialized = false;

        #endregion

        #region constructors

        public ServerDetailsView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(chart, toolbarsManager);
            detailsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            propertiesGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            unselectableMetrics.AddRange(new string[] {
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ServerVersion),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.MaintenanceModeEnabled),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OleAutomationStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WMIStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsMetricsAvailability),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.RunningSince),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AgentServiceStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DtcServiceStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullTextServiceStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlServerServiceStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmNumCPUs),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPULimit),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUReserve),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemSize),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemLimit),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemReserved),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxCPUMHz),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemSize),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumCPUCores),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumCPUThreads),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumNICs),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxBootTime),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmBootTime),
                                            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get sql service description
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SQLBrowserServiceStatus),
                                            ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus)
                                            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get sql service description
                                                        });

            InitializeDataSources();
            InitializeChart();

            UpdateServerPropertiesGrid(null);

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);

            // Autoscale font size.
            AdaptFontSize();
        }

        #endregion

        #region properties

        public event EventHandler ChartPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler FilterChanged;

        public bool ChartPanelVisible
        {
            get { return !horzSplitContainer.Panel2Collapsed; }
            set
            {
                horzSplitContainer.Panel2Collapsed = !value;
                RestoreChart();

                if (ChartPanelVisibleChanged != null)
                {
                    ChartPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool PropertiesPanelVisible
        {
            get { return !vertSplitContainer.Panel1Collapsed; }
            set { vertSplitContainer.Panel1Collapsed = !value; }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !detailsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                detailsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                historicalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }

        #endregion

        #region methods

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerDetailsView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = horzSplitContainer.Height - Settings.Default.ServerDetailsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                horzSplitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = horzSplitContainer.Height - horzSplitContainer.SplitterDistance;
            }

            lastChartVisible =
                ChartPanelVisible = Settings.Default.ServerDetailsViewChartVisible;

            if (Settings.Default.ServerDetailsViewMainGrid != null)
            {
                lastMainGridSettings = Settings.Default.ServerDetailsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, detailsGrid);
                // force a change to GroupByBox so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(detailsGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != horzSplitContainer.Height - horzSplitContainer.SplitterDistance
                || lastChartVisible != ChartPanelVisible
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.ServerDetailsViewMainSplitter = horzSplitContainer.Height - horzSplitContainer.SplitterDistance;
                lastChartVisible =
                    Settings.Default.ServerDetailsViewChartVisible = ChartPanelVisible;
                lastMainGridSettings =
                    Settings.Default.ServerDetailsViewMainGrid = gridSettings;
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            using (Log.InfoCall("DoRefreshWork"))
            {
                var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
                currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;

                // load current counter list from the repository
                List<int> currentCounters = null;
                Dictionary<int, List<int>> counterMap =
                    RepositoryHelper.GetMonitoredServerCustomCounters(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        instanceId, true);

                counterMap.TryGetValue(instanceId, out currentCounters);
                lock (updateLock)
                {
                    // update the data tables with the current list of custom counters
                    InvokeDelegate invokeDelegate = new InvokeDelegate(InitializeCustomCounters);
                    this.Invoke(invokeDelegate, currentCounters);
                }

                DateTime? historyDateTime = HistoricalSnapshotDateTime;
                if (historyDateTime == null)
                {
                    Log.Info("Getting real-time snapshot.");
                    return GetRealTimeSnapshot(previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes);
                }
                else
                {
                    Log.InfoFormat("Populating historical snapshots (end={0}).", historyDateTime.Value);
                    return PopulateHistoricalSnapshots();
                }
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                ServerDetailsView_Fill_Panel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        private delegate void InvokeDelegate(object arg);
        private void InitializeCustomCounters(object arg)
        {
            List<int> currentCounters = arg as List<int>;
            if (currentCounters != null)
            {
                foreach (int metricID in currentCounters)
                {
                    CustomCounterDefinition counterDef = metricDefinitions.GetCounterDefinition(metricID);
                    if (counterDef != null && counterDef.IsEnabled)
                    {
                        if (!lookupTable.ContainsKey(metricID))
                        {
                            InitializeCustomCounter(metricID);
                        }
                        configuredCustomCounters.Remove(metricID);
                    }
                }
                foreach (int metricID in configuredCustomCounters)
                    RemoveCustomCounter(metricID);

                configuredCustomCounters = currentCounters;
            }
            else
            {
                foreach (int metricID in configuredCustomCounters)
                    RemoveCustomCounter(metricID);

                configuredCustomCounters.Clear();
            }

            detailsGrid.Visible = detailsGrid.Rows.FilteredInRowCount > 0;
        }

        private Pair<ServerOverview, CustomCounterCollectionSnapshot> GetRealTimeSnapshot(bool visibleMinutesIncreased)
        {
            if (!chartRealTimeDataTablePrePopulated)
            {
                PrePopulateRealTimeDataTable();
                chartRealTimeDataTablePrePopulated = true;
            }
            else
            {
                BackfillScheduledRefreshData();

                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (visibleMinutesIncreased)
                    ForwardFillHistoricalData();

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();
            }

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            ServerOverviewConfiguration configuration = new ServerOverviewConfiguration(instanceId, currentSnapshot);
            ServerOverview serverOverviewSnapshot = managementService.GetServerOverview(configuration);

            // collect custom counters
            List<CustomCounterConfiguration> counterList = new List<CustomCounterConfiguration>();
            foreach (int metricID in configuredCustomCounters)
            {
                if (previousCounterSnapshot != null && previousCounterSnapshot.CustomCounterList.ContainsKey(metricID))
                {
                    counterList.Add(
                        new CustomCounterConfiguration(instanceId, previousCounterSnapshot.CustomCounterList[metricID]));
                }
                else
                {
                    CustomCounterDefinition ccd = metricDefinitions.GetCounterDefinition(metricID);
                    if (ccd != null && ccd.IsEnabled)
                    {
                        counterList.Add(new CustomCounterConfiguration(instanceId, ccd));
                    }
                }
            }
            if (counterList.Count > 0)
                previousCounterSnapshot = managementService.GetCustomCounter(counterList);
            else
                previousCounterSnapshot = null;

            if (!initialized)
            {
                // NOTE: The start and end date for server overview statisics is currently hard-coded to 
                // the last seven days.
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                DateTime startDate = endDate.Subtract(TimeSpan.FromDays(7));
                startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);

                historicalStatisticsDataTable =
                    RepositoryHelper.GetServerOverviewStatistics(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId,
                        startDate.ToUniversalTime(), endDate.ToUniversalTime());
            }

            return new Pair<ServerOverview, CustomCounterCollectionSnapshot>(serverOverviewSnapshot,
                                                                             previousCounterSnapshot);
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.chartRealTimeDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PrePopulateRealTimeDataTable();
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow["Date"];
            
            // SqlDM 10.2 (New History Browser) - If SD = 5mins and KD = 60min, then we need to reload entire data only if
            // we have no data for previous max(5,60) = 60 minutes. 
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(ServerSummaryHistoryData.MaximumKeepData))
            {
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                PrePopulateRealTimeDataTable();
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            PopulateRealTimeSnapshots(null, DateTime.Now, (int)timeDiff.TotalMinutes, false);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Replaces realtime data that is now stale with data from repository.
        /// </summary>
        private void BackFillScheduledHistoricalData()
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (chartRealTimeDataTable != null &&
                    Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = ServerSummaryHistoryData.GetBackFillHistoricalRange(chartRealTimeDataTable,
                        out startDateTime, out endDateTime);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",
                        startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        private void ForwardFillHistoricalData()
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (chartRealTimeDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    ServerSummaryHistoryData.GetForwardFillHistoricalRange(chartRealTimeDataTable,
                        out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        private void UpdateRealTimeData(ServerSummaryHistoryDataContainer serverSummary, DataTable customCounters, bool clearData)
        {
            DataView customCountersView = new DataView(customCounters);

            lock (updateLock)
            {
                MethodInvoker UICode = delegate
                {
                    chartRealTimeDataTable.BeginLoadData();
                    if (clearData) chartRealTimeDataTable.Clear();

                    if (serverSummary.OverviewStatistics != null)
                    {
                        int insertionIndex = -1;
                        if (serverSummary.OverviewStatistics.Rows.Count > 0 && !clearData)
                        {
                            insertionIndex = ServerSummaryHistoryData.GetInsertionIndex(chartRealTimeDataTable, "Date",
                                (DateTime)serverSummary.OverviewStatistics.Rows[0]["CollectionDateTime"]);
                        }
                        foreach (DataRow rawRow in serverSummary.OverviewStatistics.Rows)
                        {
                            string instanceName = Convert.ToString(rawRow["InstanceName"]);
                            var snapshot = new ServerOverview(instanceName, rawRow,
                                                                serverSummary.DiskDriveInfo.
                                                                    First, new DataRow[0]);
                            var newRow = chartRealTimeDataTable.NewRow();
                            var collectionDateTime = (DateTime)rawRow["CollectionDateTime"];
                            newRow["Date"] = collectionDateTime;
                            newRow["IsHistorical"] = true;
                            customCountersView.RowFilter = String.Format("CollectionDateTime=#{0}#", collectionDateTime.ToString("o"));
                            PopulateDataSources(snapshot, newRow, false, customCountersView);

                            if (insertionIndex >= 0)
                                chartRealTimeDataTable.Rows.InsertAt(newRow, insertionIndex++);
                            else
                                chartRealTimeDataTable.Rows.Add(newRow);
                        }
                    }
                    chartRealTimeDataTable.EndLoadData();
                };

                this.UIThread(UICode);
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser - Populates entire visible range from DB
        /// </summary>
        private void PrePopulateRealTimeDataTable()
        {
            PopulateRealTimeSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser - Populates real time data with snapshots within custom range.
        /// </summary>
        private void PopulateRealTimeSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes, bool clear)
        {
            ServerSummaryHistoryDataContainer serverSummary;
            DataTable customCounters;
            GetSnapshots(startDateTime, endDateTime, minutes, out serverSummary, out customCounters);
            UpdateRealTimeData(serverSummary, customCounters, clear);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser - Gets snapshots for the custom range.
        /// </summary>
        private void GetSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes, out ServerSummaryHistoryDataContainer serverSummary,
            out DataTable customCounters)
        {
            serverSummary = RepositoryHelper.GetServerSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                  instanceId, endDateTime, minutes, startDateTime);

            customCounters = RepositoryHelper.GetCustomCounterStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                            instanceId, endDateTime, minutes, startDateTime);
        }

        private ServerOverview PopulateHistoricalSnapshots()
        {
            ServerOverview serverOverview = null;

            ServerSummaryHistoryDataContainer serverSummary;
            DataTable customCounters;
            if (ViewMode == ServerViewMode.Historical)
            {
                GetSnapshots(null, HistoricalSnapshotDateTime.Value, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes,
                    out serverSummary, out customCounters);
            }
            else
            {
                GetSnapshots(ApplicationModel.Default.HistoryTimeValue.StartDateTime, ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value,
                    ApplicationModel.Default.HistoryTimeValue.CustomTimeMinutes, out serverSummary, out customCounters);
            }

            DataView customCountersView = new DataView(customCounters);

            lock (updateLock)
            {
                MethodInvoker UICode = delegate()
                {
                    chartHistoricalDataTable.BeginLoadData();
                    chartHistoricalDataTable.Clear();

                    if (serverSummary.ServerStatus != null &&
                        serverSummary.OverviewStatistics != null &&
                        serverSummary.OverviewStatistics.Rows.Count > 0 &&
                        (ViewMode == ServerViewMode.Custom || HistoricalSnapshotDateTime.Value == 
                        (DateTime)serverSummary.OverviewStatistics.Rows[serverSummary.OverviewStatistics.Rows.Count - 1]["CollectionDateTime"]))
                    {

                        foreach (DataRow rawRow in serverSummary.OverviewStatistics.Rows)
                        {
                            bool isLastRow = rawRow == serverSummary.OverviewStatistics.Rows[serverSummary.OverviewStatistics.Rows.Count - 1];
                            string instanceName = Convert.ToString(rawRow["InstanceName"]);
                            ServerOverview snapshot = new ServerOverview(instanceName, rawRow, serverSummary.DiskDriveInfo.First, new DataRow[0]);

                            DateTime collectionDateTime = (DateTime)rawRow["CollectionDateTime"];
                            DataRow newRow = chartHistoricalDataTable.NewRow();
                            newRow["Date"] = collectionDateTime;
                            customCountersView.RowFilter = String.Format("CollectionDateTime=#{0}#", collectionDateTime.ToString("o"));
                            PopulateDataSources(snapshot, newRow, isLastRow, customCountersView);
                            chartHistoricalDataTable.Rows.Add(newRow);

                            if (isLastRow)
                            {
                                serverOverview = snapshot;
                            }
                        }
                    }

                    chartHistoricalDataTable.EndLoadData();
                };

                this.UIThread(UICode);
            }


            return serverOverview;
        }

        private void PopulateDataSources(ServerOverview newSnapshot, DataRow dataRow, bool updateDetails, DataView customCountersView)
        {
            if (newSnapshot != null && dataRow != null)
            {
                if (newSnapshot.ProductVersion != null)
                    version = newSnapshot.ProductVersion;


                //
                // OLE Automation Status
                //
                int metric = (int)ServerDetailsMetrics.OleAutomationStatus;
                OptionStatus optionStatus =
                    (newSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.OLEAutomationUnavailable) ||
                    (newSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.Disabled)
                                                ? OptionStatus.Disabled
                                                : OptionStatus.Enabled;
                dataRow[lookupTable[metric].First] = optionStatus;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = optionStatus;
                }

                //
                // WMI Service Status
                //
                metric = (int)ServerDetailsMetrics.WMIStatus;
                optionStatus = (newSnapshot.OSMetricsStatistics.OsMetricsStatus ==
                               OSMetricsStatus.WMIServiceUnreachable || newSnapshot.OSMetricsStatistics.OsMetricsStatus ==
                               OSMetricsStatus.WMIServiceTimedOut)
                                   ? OptionStatus.Disabled
                                   : OptionStatus.Enabled;
                dataRow[lookupTable[metric].First] = optionStatus;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = optionStatus;
                }

                //
                // Available OS Memory MB
                //
                metric = (int)ServerDetailsMetrics.AvailableMemory;

                if (newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.OSMetricsStatistics.AvailableBytes.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["OSAvailableMemoryInMegabytes"];
                }

                //
                // Available OS Memory %
                //
                metric = (int)ServerDetailsMetrics.AvailableMemoryPercent;

                if (newSnapshot.OSMetricsStatistics.AvailableBytes.Kilobytes.HasValue &&
                    newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.HasValue)
                {
                    double? availableMemoryPercent = ((double)newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.Value -
                         (double)newSnapshot.OSMetricsStatistics.AvailableBytes.Kilobytes.Value) /
                        (double)newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.Value * 100.0d;

                    dataRow[lookupTable[metric].First] = availableMemoryPercent.Value;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] = GridPercentFormat(availableMemoryPercent);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["UsedMemoryPercent"];
                }

                //
                // Blocked Sessions
                //
                metric = (int)ServerDetailsMetrics.BlockedSessions;

                if (newSnapshot.SystemProcesses.BlockedProcesses.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.BlockedProcesses;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.BlockedProcesses;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["BlockedProcesses"];
                }

                //
                // Buffer Cache Hit Ratio
                //
                metric = (int)ServerDetailsMetrics.BufferCacheHitRatio;

                if (newSnapshot.Statistics.BufferCacheHitRatio.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.BufferCacheHitRatio;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.Statistics.BufferCacheHitRatio);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["BufferCacheHitRatioPercentage"];
                }

                //
                // Buffer Cache Size
                //
                metric = (int)ServerDetailsMetrics.BufferCacheSize;

                if (newSnapshot.Statistics.BufferCacheSize.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.BufferCacheSize.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.BufferCacheSize.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["BufferCacheSizeInMegabytes"];
                }

                //
                // Checkpoint Writes
                //
                metric = (int)ServerDetailsMetrics.CheckpointWrites;
                if (newSnapshot.Statistics.CheckpointPages.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.CheckpointPages;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.CheckpointPages;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Checkpoint Writes Per Second
                //
                metric = (int)ServerDetailsMetrics.CheckpointWritesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.CheckpointPages.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.CheckpointPages /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.CheckpointPages /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["CheckpointWritesPerSecond"];
                }

                //
                // Client Computers
                //
                metric = (int)ServerDetailsMetrics.ClientComputers;

                if (newSnapshot.SystemProcesses.ComputersHoldingProcesses.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.ComputersHoldingProcesses;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.ComputersHoldingProcesses;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ClientComputers"];
                }

                //
                // Connections
                //
                metric = (int)ServerDetailsMetrics.Connections;
                if (newSnapshot.Statistics.TotalConnections.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.TotalConnections;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.TotalConnections;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                // 
                // Connections Per Second
                //
                metric = (int)ServerDetailsMetrics.ConnectionsPerSecond;
                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.TotalConnections.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.TotalConnections /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.TotalConnections /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // CPU Activity
                // 
                metric = (int)ServerDetailsMetrics.CPUActivity;

                if (newSnapshot.Statistics.CpuPercentage.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.CpuPercentage;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.Statistics.CpuPercentage);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["CPUActivityPercentage"];
                }

                //
                // Database Count
                //
                metric = (int)ServerDetailsMetrics.DatabaseCount;

                if (newSnapshot.DatabaseSummary.DatabaseCount.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.DatabaseCount;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.DatabaseCount;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DatabaseCount"];
                }

                //
                // Data File Count
                //
                metric = (int)ServerDetailsMetrics.DataFileCount;

                if (newSnapshot.DatabaseSummary.DataFileCount.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.DataFileCount;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.DataFileCount;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                    dataRow[lookupTable[metric].First] = DBNull.Value;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DataFileCount"];
                }

                //
                // Data File Space Allocated
                //
                metric = (int)ServerDetailsMetrics.DataFileSpaceAllocated;

                if (newSnapshot.DatabaseSummary.DataFileSpaceAllocated.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.DataFileSpaceAllocated.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.DataFileSpaceAllocated.AsString(
                                CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DataFileSpaceAllocatedInMegabytes"];
                }

                //
                // Data File Space Used
                //
                metric = (int)ServerDetailsMetrics.DataFileSpaceUsed;

                if (newSnapshot.DatabaseSummary.DataFileSpaceUsed.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.DataFileSpaceUsed.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.DataFileSpaceUsed.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DataFileSpaceUsedInMegabytes"];
                }

                //
                // Disk Queue Length
                //
                metric = (int)ServerDetailsMetrics.DiskQueueLength;

                if (newSnapshot.OSMetricsStatistics.AvgDiskQueueLength.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.AvgDiskQueueLength;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.OSMetricsStatistics.AvgDiskQueueLength);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DiskQueueLength"];
                }

                //
                // Disk Time
                //
                metric = (int)ServerDetailsMetrics.DiskTime;

                if (newSnapshot.OSMetricsStatistics.PercentDiskTime.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.PercentDiskTime;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.OSMetricsStatistics.PercentDiskTime);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["DiskTimePercent"];
                }

                //
                // Full Scans
                //
                metric = (int)ServerDetailsMetrics.FullScans;
                if (newSnapshot.Statistics.FullScans.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.FullScans;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.FullScans;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Full Scans Per Second
                //
                metric = (int)ServerDetailsMetrics.FullScansPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.FullScans.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.FullScans / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.FullScans /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["FullScansPerSecond"];
                }

                // 
                // Lazy Writer Writes
                //
                metric = (int)ServerDetailsMetrics.LazyWriterWrites;
                if (newSnapshot.Statistics.LazyWrites.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LazyWrites;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.LazyWrites;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Lazy Writer Writes Per Second
                //
                metric = (int)ServerDetailsMetrics.LazyWriterWritesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.LazyWrites.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LazyWrites / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.LazyWrites /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LazyWriterWritesPerSecond"];
                }

                //
                // Lead Blockers
                //
                metric = (int)ServerDetailsMetrics.LeadBlockers;
                if (newSnapshot.SystemProcesses.LeadBlockers.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.LeadBlockers;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.LeadBlockers;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Lock Waits
                //
                metric = (int)ServerDetailsMetrics.LockWaits;
                if (newSnapshot.Statistics.LockWaits.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LockWaits;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.LockWaits;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Lock Waits Per Second
                //
                metric = (int)ServerDetailsMetrics.LockWaitsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.LockWaits.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LockWaits / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.LockWaits /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LockWaitsPerSecond"];
                }

                //
                // Log File Count
                //
                metric = (int)ServerDetailsMetrics.LogFileCount;

                if (newSnapshot.DatabaseSummary.LogFileCount.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.LogFileCount;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.LogFileCount;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LogFileCount"];
                }

                //
                // Log File Space Allocated
                //
                metric = (int)ServerDetailsMetrics.LogFileSpaceAllocated;

                if (newSnapshot.DatabaseSummary.LogFileSpaceAllocated.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.LogFileSpaceAllocated.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.LogFileSpaceAllocated.AsString(
                                CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LogFileSpaceAllocatedInMegabytes"];
                }

                //
                // Log Space Used
                //
                metric = (int)ServerDetailsMetrics.LogFileSpaceUsed;

                if (newSnapshot.DatabaseSummary.LogFileSpaceUsed.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.DatabaseSummary.LogFileSpaceUsed.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.DatabaseSummary.LogFileSpaceUsed.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LogFileSpaceUsedInMegabytes"];
                }

                //
                // Log Flushes
                //
                metric = (int)ServerDetailsMetrics.LogFlushes;

                if (newSnapshot.Statistics.LogFlushes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LogFlushes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.LogFlushes;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Log Flushes Per Second
                //
                metric = (int)ServerDetailsMetrics.LogFlushesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.LogFlushes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.LogFlushes / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.LogFlushes /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["LogFlushesPerSecond"];
                }

                //
                // Maintenance Mode
                //
                metric = (int)ServerDetailsMetrics.MaintenanceModeEnabled;
                bool maintenanceModeEnabled =
                    ApplicationModel.Default.ActiveInstances[instanceId].MaintenanceModeEnabled;
                dataRow[lookupTable[metric].First] = maintenanceModeEnabled
                                                         ? OptionStatus.Enabled
                                                         : OptionStatus.Disabled;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = maintenanceModeEnabled
                                                                ? OptionStatus.Enabled
                                                                : OptionStatus.Disabled;
                }

                //
                // Open Transactions
                //
                metric = (int)ServerDetailsMetrics.OpenTransactions;

                if (newSnapshot.SystemProcesses.OpenTransactions.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.OpenTransactions;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.OpenTransactions;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["OpenTransactions"];
                }

                //
                // OS Metrics Availability
                //
                metric = (int)ServerDetailsMetrics.OsMetricsAvailability;
                dataRow[lookupTable[metric].First] =
                    newSnapshot.OSMetricsStatistics.OsStatisticAvailability;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        newSnapshot.OSMetricsStatistics.OsStatisticAvailability;
                }

                //
                // Percent Privileged Time
                //
                metric = (int)ServerDetailsMetrics.OsCpuPrivilegedActivity;

                if (newSnapshot.OSMetricsStatistics.PercentPrivilegedTime.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.PercentPrivilegedTime;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.OSMetricsStatistics.PercentPrivilegedTime);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PrivilegedTimePercent"];
                }

                //
                // User Time Percent
                //
                metric = (int)ServerDetailsMetrics.OsCpuUserTime;

                if (newSnapshot.OSMetricsStatistics.PercentUserTime.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.PercentUserTime;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.OSMetricsStatistics.PercentUserTime);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["UserTimePercent"];
                }

                //
                // Processor Time Percent
                //
                metric = (int)ServerDetailsMetrics.OsTotalProcessorActivity;

                if (newSnapshot.OSMetricsStatistics.PercentProcessorTime.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.PercentProcessorTime;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.OSMetricsStatistics.PercentProcessorTime);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ProcessorTimePercent"];
                }

                //
                // Packet Errors
                //
                metric = (int)ServerDetailsMetrics.PacketErrors;
                if (newSnapshot.Statistics.PacketErrors.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketErrors;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PacketErrors;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Packet Errors Per Second
                //
                metric = (int)ServerDetailsMetrics.PacketErrorsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PacketErrors.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketErrors /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PacketErrors /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PacketErrorsPerSecond"];
                }

                //
                // Packets Received
                //
                metric = (int)ServerDetailsMetrics.PacketsReceived;

                if (newSnapshot.Statistics.PacketsReceived.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketsReceived;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PacketsReceived;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Packets Received Per Second
                //
                metric = (int)ServerDetailsMetrics.PacketsReceivedPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PacketsReceived.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketsReceived /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PacketsReceived /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PacketsReceivedPerSecond"];
                }

                //
                // Packets Sent
                //
                metric = (int)ServerDetailsMetrics.PacketsSent;
                if (newSnapshot.Statistics.PacketsSent.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketsSent;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PacketsSent;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Packets Sent Per Second
                //
                metric = (int)ServerDetailsMetrics.PacketsSentPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PacketsSent.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PacketsSent / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PacketsSent /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PacketsSentPerSecond"];
                }

                //
                // Page Life Expectancy
                //
                metric = (int)ServerDetailsMetrics.PageLifeExpectancy;

                if (newSnapshot.Statistics.PageLifeExpectancySeconds.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageLifeExpectancySeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PageLifeExpectancySeconds;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PageLifeExpectancy"];
                }

                //
                // Page Lookups
                //
                metric = (int)ServerDetailsMetrics.PageLookups;
                if (newSnapshot.Statistics.PageLookups.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageLookups;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PageLookups;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Page Lookups Per Second
                //
                metric = (int)ServerDetailsMetrics.PageLookupsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PageLookups.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageLookups / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PageLookups /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PageLookupsPerSecond"];
                }

                //
                // Page Reads
                //
                metric = (int)ServerDetailsMetrics.PageReads;
                if (newSnapshot.Statistics.PageReads.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageReads;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PageReads;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Page Reads Per Second
                //
                metric = (int)ServerDetailsMetrics.PageReadsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PageReads.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageReads / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PageReads /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PageReadsPerSecond"];
                }

                //
                // Pages Per Second
                //
                metric = (int)ServerDetailsMetrics.PagesPerSecond;

                if (newSnapshot.OSMetricsStatistics.PagesPersec.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.PagesPersec;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.OSMetricsStatistics.PagesPersec);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PagesPerSecond"];
                }

                //
                // Page Splits
                //
                metric = (int)ServerDetailsMetrics.PageSplits;
                if (newSnapshot.Statistics.PageSplits.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageSplits;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PageSplits;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Page Splits Per Second
                //
                metric = (int)ServerDetailsMetrics.PageSplitsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PageSplits.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageSplits / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PageSplits /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PageSplitsPerSecond"];
                }

                //
                // Page Writes
                //
                metric = (int)ServerDetailsMetrics.PageWrites;
                if (newSnapshot.Statistics.PageWrites.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageWrites;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.PageWrites;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Page Writes Per Second
                //
                metric = (int)ServerDetailsMetrics.PageWritesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.PageWrites.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.PageWrites / (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.PageWrites /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["PageWritesPerSecond"];
                }

                //
                // Physical Memory
                //
                metric = (int)ServerDetailsMetrics.PhysicalMemory;

                if (newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                       newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                          newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.AsString(
                                CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["OSTotalPhysicalMemoryInMegabytes"];
                }

                //
                // Procedure Cache Hit Ratio
                //
                metric = (int)ServerDetailsMetrics.ProcedureCacheHitRatio;

                if (newSnapshot.Statistics.CacheHitRatio.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.CacheHitRatio;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridPercentFormat(newSnapshot.Statistics.CacheHitRatio);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ProcedureCacheHitRatioPercentage"];
                }

                //
                // Procedure Cache Size
                //
                metric = (int)ServerDetailsMetrics.ProcedureCacheSize;

                if (newSnapshot.ProcedureCacheSize.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.ProcedureCacheSize.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.ProcedureCacheSize.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ProcedureCacheSizeInMegabytes"];
                }

                //
                // Processor Queue Length
                //
                metric = (int)ServerDetailsMetrics.ProcessorQueueLength;

                if (newSnapshot.OSMetricsStatistics.ProcessorQueueLength.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.OSMetricsStatistics.ProcessorQueueLength;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.OSMetricsStatistics.ProcessorQueueLength;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ProcessorQueueLength"];
                }

                //
                // Read Writes Errors
                //
                metric = (int)ServerDetailsMetrics.ReadWriteErrors;
                if (newSnapshot.Statistics.DiskErrors.HasValue)
                {
                    dataRow[lookupTable[metric].First] = newSnapshot.Statistics.DiskErrors.Value;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.DiskErrors.Value.ToString();
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Read Ahead Pages
                //
                metric = (int)ServerDetailsMetrics.ReadAheadPages;
                if (newSnapshot.Statistics.ReadaheadPages.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.ReadaheadPages;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.ReadaheadPages;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Read Ahead Pages Per Second
                //
                metric = (int)ServerDetailsMetrics.ReadAheadPagesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.ReadaheadPages.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.ReadaheadPages /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.ReadaheadPages /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ReadAheadPagesPerSecond"];
                }

                //
                // Response Time
                //
                metric = (int)ServerDetailsMetrics.ResponseTime;

                dataRow[lookupTable[metric].First] =
                    newSnapshot.ResponseTime;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        newSnapshot.ResponseTime;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["ResponseTimeInMilliseconds"];
                }

                //
                // Running Since
                //
                metric = (int)ServerDetailsMetrics.RunningSince;
                if (newSnapshot.RunningSince.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.RunningSince;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.RunningSince.Value.ToLocalTime().ToString(
                                CultureInfo.CurrentCulture.DateTimeFormat);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Server Version
                //
                metric = (int)ServerDetailsMetrics.ServerVersion;
                dataRow[lookupTable[metric].First] =
                    newSnapshot.ProductVersion.Version;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        newSnapshot.ProductVersion.Version;
                }

                //
                // SQL Compilations
                //
                metric = (int)ServerDetailsMetrics.SqlCompilations;
                if (newSnapshot.Statistics.SqlCompilations.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.SqlCompilations;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.SqlCompilations;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // SQL Compilations Per Second
                //
                metric = (int)ServerDetailsMetrics.SqlCompilationsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.SqlCompilations.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.SqlCompilations /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.SqlCompilations /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SqlCompilationsPerSecond"];
                }

                //
                // SQL Memory Allocated
                //
                metric = (int)ServerDetailsMetrics.SqlMemoryAllocated;

                if (newSnapshot.TargetServerMemory.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.TargetServerMemory.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.TargetServerMemory.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SqlMemoryAllocatedInMegabytes"];
                }

                //
                // SQL Memory Used
                //
                metric = (int)ServerDetailsMetrics.SqlMemoryUsed;

                if (newSnapshot.TotalServerMemory.Megabytes.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.TotalServerMemory.Megabytes;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.TotalServerMemory.AsString(CultureInfo.CurrentCulture);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SqlMemoryUsedInMegabytes"];
                }

                //
                // SQL Memory Used Percent
                //
                metric = (int)ServerDetailsMetrics.SqlMemoryUsedPercent;

                if (newSnapshot.TotalServerMemory.Kilobytes.HasValue &&
                    newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.HasValue)
                {
                    double? used = (double)(newSnapshot.TotalServerMemory.Kilobytes.Value /
                                             newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.
                                                 Value *
                                             100m);
                    if (used.Value > 100)
                        used = 100d;

                    dataRow[lookupTable[metric].First] = used;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] = GridPercentFormat(used);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["UsedSqlMemoryPercent"];
                }

                //
                // SQL Recompilations
                //
                metric = (int)ServerDetailsMetrics.SqlRecompilations;
                if (newSnapshot.Statistics.SqlRecompilations.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.SqlRecompilations;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.SqlRecompilations;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // SQL Recompilations Per Second
                //
                metric = (int)ServerDetailsMetrics.SqlRecompilationsPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.SqlRecompilations.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.SqlRecompilations /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.SqlRecompilations /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SqlRecompilationsPerSecond"];
                }

                //
                // System Processes
                //
                metric = (int)ServerDetailsMetrics.SystemProcesses;

                if (newSnapshot.SystemProcesses.CurrentSystemProcesses.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.CurrentSystemProcesses;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.CurrentSystemProcesses;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SystemProcesses"];
                }

                //
                // System Processes Consuming CPU
                //
                metric = (int)ServerDetailsMetrics.SystemProcessesConsumingCPU;

                if (newSnapshot.SystemProcesses.SystemProcessesConsumingCpu.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.SystemProcessesConsumingCpu;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.SystemProcessesConsumingCpu;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["SystemProcessesConsumingCPU"];
                }

                //
                // Batches
                //
                metric = (int)ServerDetailsMetrics.Batches;
                if (newSnapshot.Statistics.BatchRequests.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.BatchRequests;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.BatchRequests;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Batches Per Second
                //
                metric = (int)ServerDetailsMetrics.BatchesPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.BatchRequests.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.BatchRequests /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.BatchRequests /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["BatchesPerSecond"];
                }

                //
                // User Processes
                //
                metric = (int)ServerDetailsMetrics.UserProcesses;

                if (newSnapshot.SystemProcesses.CurrentUserProcesses.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.CurrentUserProcesses;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.CurrentUserProcesses;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["UserProcesses"];
                }

                //
                // User Connections Percent
                //
                metric = (int)ServerDetailsMetrics.UserConnectionsPercent;
                if (newSnapshot.SystemProcesses.CurrentUserProcesses.HasValue &&
                    newSnapshot.MaxConnections.HasValue)
                {
                    long lvalue = newSnapshot.MaxConnections.Value;
                    int ivalue = newSnapshot.SystemProcesses.CurrentUserProcesses.Value;
                    double? pvalue = ivalue * 100 / lvalue;

                    dataRow[lookupTable[metric].First] = pvalue;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] = GridPercentFormat(pvalue);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // User Processes Consuming CPU
                //
                metric = (int)ServerDetailsMetrics.UserProcessesConsumingCPU;

                if (newSnapshot.SystemProcesses.UserProcessesConsumingCpu.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.SystemProcesses.UserProcessesConsumingCpu;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.SystemProcesses.UserProcessesConsumingCpu;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["UserProcessesConsumingCPU"];
                }

                //
                // Workfiles Created
                //
                metric = (int)ServerDetailsMetrics.WorkfilesCreated;
                if (newSnapshot.Statistics.WorkfilesCreated.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.WorkfilesCreated;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.WorkfilesCreated;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Workfiles created per second
                //
                metric = (int)ServerDetailsMetrics.WorkfilesCreatedPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.WorkfilesCreated.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.WorkfilesCreated /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.WorkfilesCreated /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["WorkfilesCreatedPerSecond"];
                }

                //
                // Work Tables Created
                //
                metric = (int)ServerDetailsMetrics.WorktablesCreated;
                if (newSnapshot.Statistics.WorktablesCreated.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.WorktablesCreated;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            newSnapshot.Statistics.WorktablesCreated;
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                //
                // Work Tables Created Per Second
                //
                metric = (int)ServerDetailsMetrics.WorktablesCreatedPerSecond;

                if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.WorktablesCreated.HasValue)
                {
                    dataRow[lookupTable[metric].First] =
                        newSnapshot.Statistics.WorktablesCreated /
                        (double)newSnapshot.TimeDelta.Value.TotalSeconds;

                    if (updateDetails)
                    {
                        lookupTable[metric].Second[COL_VALUE] =
                            GridNumberFormat(newSnapshot.Statistics.WorktablesCreated /
                                             (double)newSnapshot.TimeDelta.Value.TotalSeconds);
                    }
                }
                else if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = null;
                }

                if (updateDetails &&
                    historicalStatisticsDataTable != null &&
                    historicalStatisticsDataTable.Rows.Count != 0)
                {
                    lookupTable[metric].Second[COL_REFERENCERANGE] =
                        historicalStatisticsDataTable.Rows[0]["WorkTablesCreatedPerSecond"];
                }

                //
                // VM Metrics
                //
                bool sortNeeded = PopulateVmMetrics(newSnapshot.VMConfig, dataRow, updateDetails);

                if (customCountersView != null)
                {
                    Set<int> configuredCounters = null;
                    if (configuredCustomCounters != null)
                        configuredCounters = new Set<int>(configuredCustomCounters);

                    foreach (DataRowView row in customCountersView)
                    {
                        int metricID = (int)row["MetricID"];
                        object value = DBNull.Value;
                        object errorMessage = row["ErrorMessage"];
                        decimal scaledValue = 0m;
                        decimal timeDelta = 0m;
                        if (errorMessage == DBNull.Value)
                        {
                            CustomCounterDefinition ccd = metricDefinitions.GetCounterDefinition(metricID);
                            if (ccd != null && ccd.IsEnabled)
                            {
                                try
                                {
                                    if (ccd.CalculationType == CalculationType.Value)
                                    {
                                        value = row["RawValue"];
                                        if (value is decimal)
                                        {
                                            // scaled value for chart
                                            scaledValue = Convert.ToDecimal(ccd.Scale) * (decimal)value;
                                            // formatted string for grid
                                            value = GridNumberFormat<decimal>(scaledValue);
                                        }
                                    }
                                    else
                                    {
                                        value = row["DeltaValue"];
                                        if (value is decimal)
                                        {
                                            if (row["TimeDeltaInSeconds"] is double)
                                            {
                                                timeDelta = Convert.ToDecimal((double)row["TimeDeltaInSeconds"]);
                                                if (timeDelta > 0)
                                                {
                                                    // scaled value for chart
                                                    scaledValue = Convert.ToDecimal(ccd.Scale) * (((decimal)value) / timeDelta);
                                                    // formatted string for grid
                                                    value = GridNumberFormat<decimal>(scaledValue);
                                                }
                                                else
                                                    value = DBNull.Value;
                                            }
                                            else
                                                value = DBNull.Value;
                                        }
                                    }
                                }
                                catch (OverflowException oe)
                                {
                                    value = DBNull.Value;
                                    errorMessage =
                                        "The value multiplied by the scaling factor resulted in a value that is larger than supported.  " +
                                        oe.Message;
                                }
                            }
                        }
                        // keep up with the counters we have processed
                        if (configuredCounters != null)
                            configuredCounters.Remove(metricID);

                        Triple<string, DataRow, DataColumn> lookupData;
                        if (lookupTable.TryGetValue(metricID, out lookupData))
                        {
                            // chart data is value or DBNull
                            dataRow[lookupData.Third.ColumnName] = value == DBNull.Value ? value : scaledValue;
                            if (updateDetails)
                            {
                                // details grid data is value or error message
                                lookupData.Second[COL_VALUE] = errorMessage == DBNull.Value ? value : errorMessage;
                            }
                        }
                    }

                    if (configuredCounters != null && configuredCounters.Count > 0)
                    {
                        foreach (int metricID in configuredCounters)
                        {
                            Triple<string, DataRow, DataColumn> lookupData;
                            if (lookupTable.TryGetValue(metricID, out lookupData))
                            {
                                dataRow[lookupData.Third.ColumnName] = DBNull.Value;
                                if (updateDetails)
                                {
                                    lookupData.Second[COL_VALUE] = DBNull.Value;
                                }
                            }
                        }
                    }
                }

                //                Random random = new Random(DateTime.Now.Millisecond);
                //                foreach (int metricID in configuredCustomCounters)
                //                {
                //                    object value = random.Next(100);
                //                    Triple<string, DataRow, DataColumn> lookupData;
                //                    if (lookupTable.TryGetValue(metricID, out lookupData))
                //                    {
                //                        dataRow[lookupData.Third] = value;
                //                        if (updateDetails)
                //                            lookupData.Second[COL_VALUE] = value;
                //                    }
                //                }
                //
                //
                // Custom Counters
                //
                //                MetricDefinitions metricDefinitions = ApplicationModel.Default.MetricDefinitions;
                //                List<CustomCounterSnapshot> ccss = new List<CustomCounterSnapshot>();
                //                foreach (CustomCounterSnapshot customSnapshot in ccss)
                //                {
                //                    metric = customSnapshot.Definition.MetricID;
                //                    CustomCounterDefinition definition = metricDefinitions.GetCounterDefinition(metric);
                //                    if (definition == null || !definition.IsEnabled)
                //                        continue;
                //
                //                    MetricDescription? metricDesc = metricDefinitions.GetMetricDescription(metric);
                //                    if (metricDesc == null)
                //                        continue;
                //
                //                    if (customSnapshot.DisplayValue.HasValue)                    
                //                    {
                //                        object value = customSnapshot.DisplayValue.Value;
                //
                //                        dataRow[lookupTable[metric].First] = value;
                //
                //                        if (updateDetails)
                //                        {
                //                            lookupTable[metric].Second[COL_VALUE] = value;
                //                        }
                //                    }
                //                    else if (updateDetails)
                //                    {
                //                        lookupTable[metric].Second[COL_VALUE] = null;
                //                    }
                //
                //                    if (updateDetails &&
                //                        historicalStatisticsDataTable != null &&
                //                        historicalStatisticsDataTable.Rows.Count != 0)
                //                    {
                //                        lookupTable[metric].Second[COL_REFERENCERANGE] =
                //                            historicalStatisticsDataTable.Rows[0]["CustomCounter" + metric.ToString()];
                //                    }   
                //                }
            }
        }

        private bool PopulateVmMetrics(Common.VMware.VMwareVirtualMachine VMConfig, DataRow dataRow, bool updateDetails)
        {
            bool sortNeeded = false;

            int metric;
            if (VMConfig == null)
            {
                if (vmCountersInitialized)
                    ZapVmMetrics();

                return sortNeeded;
            }

            if (!vmCountersInitialized)
            {
                InitializeVmMetrics();
                sortNeeded = true;
            }
            //
            // VM Num CPUs
            //
            metric = (int)ServerDetailsMetrics.VmNumCPUs;

            dataRow[lookupTable[metric].First] = VMConfig.NumCPUs;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.NumCPUs;
            }

            //
            // VM CPU Limit
            //
            metric = (int)ServerDetailsMetrics.VmCPULimit;

            dataRow[lookupTable[metric].First] = VMConfig.CPULimit;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.CPULimit < 0 ? "Unlimited" : GridCPUSpeedFormat<long>(VMConfig.CPULimit);
            }

            //
            // VM CPU Reserved
            //
            metric = (int)ServerDetailsMetrics.VmCPUReserve;

            dataRow[lookupTable[metric].First] = VMConfig.CPUReserve;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridCPUSpeedFormat<long>(VMConfig.CPUReserve);
            }

            //
            // VM Memory Size
            //
            if ((VMConfig.MemSize != null) && (VMConfig.MemSize.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemSize;

                dataRow[lookupTable[metric].First] = VMConfig.MemSize.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = VMConfig.MemSize.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            //
            // VM Memory Limit
            //
            if ((VMConfig.MemLimit != null) && (VMConfig.MemLimit.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemLimit;

                dataRow[lookupTable[metric].First] = VMConfig.MemLimit.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = VMConfig.MemLimit.Megabytes < 0 ? "Unlimited" : VMConfig.MemLimit.Megabytes.ToString();
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            //
            // VM Memory Reserved
            //
            if ((VMConfig.MemReserve != null) && (VMConfig.MemReserve.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemReserved;

                dataRow[lookupTable[metric].First] = VMConfig.MemReserve.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = VMConfig.MemReserve.Megabytes;
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            //
            // HOST CPU MHz
            //
            metric = (int)ServerDetailsMetrics.EsxCPUMHz;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.CPUMHz;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridCPUSpeedFormat<long>(VMConfig.ESXHost.CPUMHz);
            }

            //
            // HOST Number of Physical CPUs
            //
            metric = (int)ServerDetailsMetrics.EsxNumCPUCores;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.NumCPUCores;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.NumCPUCores;
            }

            //
            // HOST Number of Logical CPUs
            //
            metric = (int)ServerDetailsMetrics.EsxNumCPUThreads;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.NumCPUThreads;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.NumCPUThreads;
            }

            //
            // HOST Memory Size
            //
            if ((VMConfig.ESXHost.MemSize != null) && (VMConfig.ESXHost.MemSize.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.EsxMemSize;

                dataRow[lookupTable[metric].First] = VMConfig.ESXHost.MemSize.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.MemSize.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            //
            // HOST Number of NICs
            //
            metric = (int)ServerDetailsMetrics.EsxNumNICs;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.NumNICs;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.NumNICs;
            }

            //
            // HOST Last Boot Time
            //
            metric = (int)ServerDetailsMetrics.EsxBootTime;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.BootTime.ToLocalTime();

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.BootTime.ToLocalTime();
            }

            //
            // VM Last Boot Time
            //
            metric = (int)ServerDetailsMetrics.VmBootTime;

            dataRow[lookupTable[metric].First] = VMConfig.BootTime.ToLocalTime();

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = VMConfig.BootTime.ToLocalTime();
            }

            //
            // VM CPU Usage
            //
            metric = (int)ServerDetailsMetrics.VmCPUUsage;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.CpuUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridPercentFormat<double>(VMConfig.PerfStats.CpuUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMCPUUsage"];
            }

            //
            // VM CPU Usage (MHz)
            //
            metric = (int)ServerDetailsMetrics.VmCPUUsageMHz;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.CpuUsageMHz;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridCPUSpeedFormat<long>(VMConfig.PerfStats.CpuUsageMHz);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMCPUUsageMHz"];
            }

            //
            // VM CPU Ready
            //
            metric = (int)ServerDetailsMetrics.VmCPUReady;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.CpuReady;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDurationMsFormat<long>(VMConfig.PerfStats.CpuReady);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMCPUReady"];
            }

            //
            // VM CPU Swap Wait
            //
            metric = (int)ServerDetailsMetrics.VmCPUSwapWait;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.CpuSwapWait;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDurationMsFormat<long>(VMConfig.PerfStats.CpuSwapWait);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMCPUSwapWait"];
            }

            //
            // VM Memory Swap In Rate
            //
            metric = (int)ServerDetailsMetrics.VmMemSwapInRate;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemSwapInRate;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.MemSwapInRate);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemSwapInRate"];
            }

            //
            // VM Memory Swap Out Rate
            //
            metric = (int)ServerDetailsMetrics.VmMemSwapOutRate;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemSwapOutRate;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.MemSwapOutRate);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemSwapOutRate"];
            }

            //
            // VM Memory Swapped
            //
            if ((VMConfig.PerfStats.MemSwapped != null) && (VMConfig.PerfStats.MemSwapped.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemSwapped;

                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemSwapped.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.PerfStats.MemSwapped.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemSwapped"];
            }

            //
            // VM Memory Active
            //
            if ((VMConfig.PerfStats.MemActive != null) && (VMConfig.PerfStats.MemActive.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemActive;

                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemActive.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.PerfStats.MemActive.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemActive"];
            }

            //
            // VM Memory Consumed
            //
            if ((VMConfig.PerfStats.MemConsumed != null) && (VMConfig.PerfStats.MemConsumed.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemConsumed;

                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemConsumed.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.PerfStats.MemConsumed.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemConsumed"];
            }

            //
            // VM Memory Granted
            //
            if ((VMConfig.PerfStats.MemGranted != null) && (VMConfig.PerfStats.MemGranted.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemGranted;

                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemGranted.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.PerfStats.MemGranted.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemGranted"];
            }

            //
            // VM Memory Ballooned
            //
            if ((VMConfig.PerfStats.MemBallooned != null) && (VMConfig.PerfStats.MemBallooned.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.VmMemBalooned;

                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemBallooned.Kilobytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.PerfStats.MemBallooned.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemBalooned"];
            }

            //
            // VM Memory Usage
            //
            metric = (int)ServerDetailsMetrics.VmMemUsage;

            try
            {
                dataRow[lookupTable[metric].First] = VMConfig.PerfStats.MemUsage;
            }
            catch(System.ArgumentException argEx)
            {
                Log.ErrorFormat("Exception in PopulateVmMetrics: {0}", argEx.Message);
            }

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridPercentFormat<double>(VMConfig.PerfStats.MemUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMMemUsage"];
            }

            //
            // VM Disk Reads
            //
            metric = (int)ServerDetailsMetrics.VmDiskRead;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.DiskRead;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.DiskRead);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMDiskRead"];
            }

            //
            // VM Disk Writes
            //
            metric = (int)ServerDetailsMetrics.VmDiskWrite;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.DiskWrite;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.DiskWrite);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMDiskWrite"];
            }

            //
            // VM Disk Usage
            //
            metric = (int)ServerDetailsMetrics.VmDiskUsage;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.DiskUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.DiskUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMDiskUsage"];
            }

            //
            // VM Network Usage
            //
            metric = (int)ServerDetailsMetrics.VmNetUsage;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.NetUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.NetUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMNetUsage"];
            }

            //
            // VM Network Transmitted
            //
            metric = (int)ServerDetailsMetrics.VmNetTransmitted;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.NetTransmitted;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.NetTransmitted);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMNetTransmitted"];
            }

            //
            // VM Network Received
            //
            metric = (int)ServerDetailsMetrics.VmNetReceived;

            dataRow[lookupTable[metric].First] = VMConfig.PerfStats.NetReceived;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.PerfStats.NetReceived);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["VMNetReceived"];
            }

            //
            // HOST CPU Usage
            //
            metric = (int)ServerDetailsMetrics.EsxCPUUsage;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.CpuUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridPercentFormat<double>(VMConfig.ESXHost.PerfStats.CpuUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXCPUUsage"];
            }

            //
            // HOST CPU Usage (MHz)
            //
            metric = (int)ServerDetailsMetrics.EsxCPUUsageMHz;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.CpuUsageMHz;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridCPUSpeedFormat<long>(VMConfig.ESXHost.PerfStats.CpuUsageMHz);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXCPUUsageMHz"];
            }

            //
            // HOST Memory Swap In Rate
            //
            metric = (int)ServerDetailsMetrics.EsxMemSwapInRate;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemSwapInRate;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.MemSwapInRate);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemSwapInRate"];
            }

            //
            // VM Memory Swap Out Rate
            //
            metric = (int)ServerDetailsMetrics.EsxMemSwapOutRate;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemSwapOutRate;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.MemSwapOutRate);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemSwapOutRate"];
            }

            //
            // HOST Memory Active
            //
            if ((VMConfig.ESXHost.PerfStats.MemActive != null) && (VMConfig.ESXHost.PerfStats.MemActive.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.EsxMemActive;

                dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemActive.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.ESXHost.PerfStats.MemActive.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemActive"];
            }

            //
            // HOST Memory Consumed
            //
            if ((VMConfig.ESXHost.PerfStats.MemConsumed != null) && (VMConfig.ESXHost.PerfStats.MemConsumed.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.EsxMemConsumed;

                dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemConsumed.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.ESXHost.PerfStats.MemConsumed.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemConsumed"];
            }

            //
            // HOST Memory Granted
            //
            if ((VMConfig.ESXHost.PerfStats.MemGranted != null) && (VMConfig.ESXHost.PerfStats.MemGranted.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.EsxMemGranted;

                dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemGranted.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.ESXHost.PerfStats.MemGranted.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemGranted"];
            }

            //
            // HOST Memory Ballooned
            //
            if ((VMConfig.ESXHost.PerfStats.MemBallooned != null) && (VMConfig.ESXHost.PerfStats.MemBallooned.Megabytes.HasValue))
            {
                metric = (int)ServerDetailsMetrics.EsxMemBalooned;

                dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemBallooned.Megabytes;

                if (updateDetails)
                {
                    lookupTable[metric].Second[COL_VALUE] =
                        VMConfig.ESXHost.PerfStats.MemBallooned.AsString(CultureInfo.CurrentCulture);
                }
            }
            else if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = null;
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemBalooned"];
            }

            //
            // HOST Memory Usage
            //
            metric = (int)ServerDetailsMetrics.EsxMemUsage;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.MemUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridPercentFormat<double>(VMConfig.ESXHost.PerfStats.MemUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXMemUsage"];
            }

            //
            // HOST Disk Reads
            //
            metric = (int)ServerDetailsMetrics.EsxDiskRead;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskRead;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.DiskRead);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXDiskRead"];
            }

            //
            // HOST Disk Writes
            //
            metric = (int)ServerDetailsMetrics.EsxDiskWrite;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskWrite;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.DiskWrite);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXDiskWrite"];
            }

            //
            // HOST Disk Usage
            //
            metric = (int)ServerDetailsMetrics.EsxDiskUsage;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.DiskUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXDiskUsage"];
            }

            //
            // HOST Disk Device Latency
            //
            //    metric = (int) ServerDetailsMetrics.EsxDiskDeviceLatency;

            //    dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskDeviceLatency;

            //    if (updateDetails)
            //    {
            //        lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.PerfStats.DiskDeviceLatency;
            //    }

            //if (updateDetails &&
            //    historicalStatisticsDataTable != null &&
            //    historicalStatisticsDataTable.Rows.Count != 0)
            //{
            //    lookupTable[metric].Second[COL_REFERENCERANGE] =
            //        historicalStatisticsDataTable.Rows[0]["ESXDiskDeviceLatency"];
            //}

            //
            // HOST Disk Kernel Latency
            //
            //    metric = (int) ServerDetailsMetrics.EsxDiskKernelLatency;

            //    dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskKernelLatency;

            //    if (updateDetails)
            //    {
            //        lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.PerfStats.DiskKernelLatency;
            //    }

            //if (updateDetails &&
            //    historicalStatisticsDataTable != null &&
            //    historicalStatisticsDataTable.Rows.Count != 0)
            //{
            //    lookupTable[metric].Second[COL_REFERENCERANGE] =
            //        historicalStatisticsDataTable.Rows[0]["ESXDiskKernelLatency"];
            //}

            //
            // HOST Disk Queue Latency
            //
            //    metric = (int) ServerDetailsMetrics.EsxDiskQueueLatency;

            //    dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskQueueLatency;

            //    if (updateDetails)
            //    {
            //        lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.PerfStats.DiskQueueLatency;
            //    }

            //if (updateDetails &&
            //    historicalStatisticsDataTable != null &&
            //    historicalStatisticsDataTable.Rows.Count != 0)
            //{
            //    lookupTable[metric].Second[COL_REFERENCERANGE] =
            //        historicalStatisticsDataTable.Rows[0]["ESXDiskQueueLatency"];
            //}

            //
            // HOST Disk Total Latency
            //
            //    metric = (int) ServerDetailsMetrics.EsxDiskTotalLatency;

            //    dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.DiskTotalLatency;

            //    if (updateDetails)
            //    {
            //        lookupTable[metric].Second[COL_VALUE] = VMConfig.ESXHost.PerfStats.DiskTotalLatency;
            //    }

            //if (updateDetails &&
            //    historicalStatisticsDataTable != null &&
            //    historicalStatisticsDataTable.Rows.Count != 0)
            //{
            //    lookupTable[metric].Second[COL_REFERENCERANGE] =
            //        historicalStatisticsDataTable.Rows[0]["ESXDiskTotalLatency"];
            //}

            //
            // HOST Network Usage
            //
            metric = (int)ServerDetailsMetrics.EsxNetUsage;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.NetUsage;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.NetUsage);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXNetUsage"];
            }

            //
            // HOST Network Transmitted
            //
            metric = (int)ServerDetailsMetrics.EsxNetTransmitted;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.NetTransmitted;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.NetTransmitted);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXNetTransmitted"];
            }

            //
            // HOST Network Received
            //
            metric = (int)ServerDetailsMetrics.EsxNetReceived;

            dataRow[lookupTable[metric].First] = VMConfig.ESXHost.PerfStats.NetReceived;

            if (updateDetails)
            {
                lookupTable[metric].Second[COL_VALUE] = GridDataRateFormat<long>(VMConfig.ESXHost.PerfStats.NetReceived);
            }

            if (updateDetails &&
                historicalStatisticsDataTable != null &&
                historicalStatisticsDataTable.Rows.Count != 0)
            {
                lookupTable[metric].Second[COL_REFERENCERANGE] =
                    historicalStatisticsDataTable.Rows[0]["ESXNetReceived"];
            }

            return sortNeeded;
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, detailsGrid);
                        initialized = true;
                    }
                }

                if (HistoricalSnapshotDateTime == null)
                {
                    if (data is Pair<ServerOverview, CustomCounterCollectionSnapshot>)
                    {
                        UpdateDataWithRealTimeSnapshot((Pair<ServerOverview, CustomCounterCollectionSnapshot>)data);
                    }
                    else
                    {
                        UpdateServerPropertiesGrid(null);
                        ApplicationController.Default.RefreshActiveView();
                        return;
                    }
                }
                else
                {
                    UpdateDataWithHistoricalSnapshots(data as ServerOverview);
                }

                detailsGrid.Visible = detailsGrid.Rows.FilteredInRowCount > 0;
                detailsGrid.DisplayLayout.Rows.EnsureSortedAndFiltered();
            }
        }

        /// <summary>
        /// Unmaps a custom counter from the lookup and data tables.
        /// </summary>
        /// <param name="metricID"></param>
        private void RemoveCustomCounter(int metricID)
        {
            Triple<string, DataRow, DataColumn> triple;
            if (lookupTable.TryGetValue(metricID, out triple))
            {
                // remove the field from the chart
                if (chartFieldMapLookupTable.ContainsKey(metricID))
                    RemoveMetricFromChart(metricID, false);
                // remove the row in the details grid
                detailsGridDataSource.Rows.Remove(triple.Second);
                // remove the column in the chart data tables
                chartRealTimeDataTable.Columns.Remove(triple.Third);
                chartHistoricalDataTable.Columns.Remove(triple.Third.ColumnName);
                // remove the entry from the lookup table
                lookupTable.Remove(metricID);
            }
        }

        private void InitializeCustomCounter(int metricID)
        {
            CustomCounterDefinition counterDefinition = metricDefinitions.GetCounterDefinition(metricID);
            if (counterDefinition == null)
            {
                Log.InfoFormat("Can't find custom counter definition id={0}", metricID);
                return;
            }
            MetricDescription? description = metricDefinitions.GetMetricDescription(metricID);
            if (!description.HasValue)
            {
                Log.WarnFormat("Can't find custom counter description for id={0}", metricID);
                return;
            }
            if (!counterDefinition.IsEnabled)
            {
                Log.InfoFormat("Custom counter '{0}' is disabled.", description.Value.Name);
                return;
            }
            DataRow metricRow = detailsGridDataSource.Rows.Add(new object[] { false, description.Value.Name });
            metricRow["Category"] = description.Value.Category;
            metricRow["AlertMetric"] = metricID;
            metricRow["Metric Type"] = metricID;
            metricRow["State"] = 0;
            metricRow["Custom Counter"] = "Yes";
            // custom counters are decimal but we are going to treat them as doubles
            Type valueType = typeof(double);

            // create columns in the chart data tables
            DataColumn metricColumn = chartRealTimeDataTable.Columns.Add("CustomCounter" + metricID.ToString(), valueType);
            chartHistoricalDataTable.Columns.Add("CustomCounter" + metricID.ToString(), valueType);
            // add an assiciation between the metric id and the row and columns we created to show the data for the counter
            lookupTable.Add(metricID,
                            new Triple<string, DataRow, DataColumn>(
                                description.Value.Name, metricRow, metricColumn));
        }

        private void UpdateDataWithRealTimeSnapshot(Pair<ServerOverview, CustomCounterCollectionSnapshot> snapshot)
        {
            operationalStatusPanel.Visible = false;
            ServerDetailsView_Fill_Panel.Visible = true;

            if (snapshot.First != null)
            {
                if (snapshot.First.Error == null)
                {
                    if (snapshot.First.SqlServiceStatus != ServiceState.Running)
                    {
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                                    new ApplicationException(
                                                                        "The SQL Server service is not available. The SQL Server service must be running in order to collect statistics.")));
                    }
                    else
                    {
                        chart.DataSource = chartRealTimeDataTable;
                        DataRow dataRow = chartRealTimeDataTable.NewRow();
                        dataRow["Date"] = DateTime.Now;
                        PopulateDataSources(snapshot.First, dataRow, true, ConvertCustomCounterSnapshotToDataView(snapshot.Second));
                        chartRealTimeDataTable.Rows.Add(dataRow);
                        GroomHistoryData();
                        UpdateChartDataFilter();
                        UpdateCellColors(chartRealTimeDataTable);
                        currentSnapshot = snapshot.First;
                        UpdateServerPropertiesGrid(currentSnapshot);
                        ApplicationController.Default.SetCustomStatus(
                            String.Format("Server Details: {0} Item{1}",
                                          detailsGrid.Rows.Count,
                                          detailsGrid.Rows.Count == 1 ? string.Empty : "s")
                            );
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                }
                else
                {
                    currentSnapshot = snapshot.First;
                    UpdateServerPropertiesGrid(currentSnapshot);

                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.First.Error));
                }
            }
        }

        private DataView ConvertCustomCounterSnapshotToDataView(CustomCounterCollectionSnapshot customCounterCollectionSnapshot)
        {
            if (customCounterCollectionSnapshot != null && !customCounterCollectionSnapshot.CollectionFailed)
            {
                DataTable dataTable = new DataTable("CustomCounters");
                dataTable.Columns.Add(new DataColumn("InstanceName", typeof(string)));
                dataTable.Columns.Add(new DataColumn("CollectionDateTime", typeof(string)));
                dataTable.Columns.Add(new DataColumn("MetricID", typeof(int)));
                dataTable.Columns.Add(new DataColumn("TimeDeltaInSeconds", typeof(double)));
                dataTable.Columns.Add(new DataColumn("RawValue", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("DeltaValue", typeof(decimal)));
                dataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(string)));

                foreach (CustomCounterSnapshot snapshot in customCounterCollectionSnapshot.CustomCounterList.Values)
                {
                    DataRow row = dataTable.NewRow();
                    row["InstanceName"] = snapshot.ServerName;
                    row["CollectionDateTime"] = snapshot.TimeStampLocal;
                    row["MetricID"] = snapshot.Definition.MetricID;
                    row["TimeDeltaInSeconds"] = snapshot.TimeDelta.HasValue ? (object)snapshot.TimeDelta.Value.TotalSeconds : DBNull.Value;
                    row["RawValue"] = snapshot.RawValue.HasValue ? (object)snapshot.RawValue.Value : DBNull.Value;
                    row["DeltaValue"] = snapshot.DeltaValue.HasValue ? (object)snapshot.DeltaValue.Value : DBNull.Value;
                    row["ErrorMessage"] = snapshot.Error != null ? (object)snapshot.Error.Message : DBNull.Value;
                    dataTable.Rows.Add(row);
                }
                return new DataView(dataTable);
            }
            return null;
        }

        private void UpdateServerPropertiesGrid(ServerOverview activeSnapshot)
        {
            propertiesGridDataSource.SuspendBindingNotifications();
            propertiesGridDataSource.Rows.Clear();
 
            if (activeSnapshot != null)
            {

                string denomination;

                if (activeSnapshot.ProductVersion != null && activeSnapshot.ProductVersion.Version != null)
                {
                    AddPropertiesGridRow("Version",activeSnapshot.ProductVersion + " (" + activeSnapshot.ProductVersion.Version  + ")");
                }
                else
                {
                    AddPropertiesGridRow("Version", "?");
                }
               
                AddPropertiesGridRow("Edition", activeSnapshot.SqlServerEdition ?? "?");
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null &&  ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                {
                    AddPropertiesGridRow("Running", activeSnapshot.RunningSince.HasValue ? activeSnapshot.RunningSince.Value.ToLocalTime().ToString("G") : "?");
                }
                var propertiesIsClusteredLabel = activeSnapshot.IsClustered.HasValue? activeSnapshot.IsClustered.Value ? "Yes" : "No": "?";
                if (activeSnapshot.IsClustered.HasValue && activeSnapshot.IsClustered.Value)
                {
                    propertiesIsClusteredLabel = " / Active Node: ";
                    propertiesIsClusteredLabel += activeSnapshot.ClusterNodeName ?? "?";
                }
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                {
                    AddPropertiesGridRow("Clustered", propertiesIsClusteredLabel);
                }

                if (activeSnapshot.ProcessorCount.HasValue && activeSnapshot.ProcessorsUsed.HasValue)
                {
                    if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId!=null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                    {
                        AddPropertiesGridRow("Processors", string.Format("{1} of {0} used", activeSnapshot.ProcessorCount, activeSnapshot.ProcessorsUsed));
                    }
                }
                else
                {
                    if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null &&  ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                    {
                        AddPropertiesGridRow("Processors", "?");
                    }
                }

                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null &&  ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                {
                    AddPropertiesGridRow("Host", activeSnapshot.ServerHostName ?? "?");
                }
                AddPropertiesGridRow("Host OS", activeSnapshot.WindowsVersion ?? "?");

                decimal? totalPhysicalMemory =
                    activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory.BestDenomination(out denomination);
                if (!(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId))
                {
                    AddPropertiesGridRow("Host Memory", totalPhysicalMemory.HasValue ? string.Format("{0:F2} {1}", totalPhysicalMemory.Value, denomination) : "?");
                }
                AddPropertiesGridRow("Databases", activeSnapshot.DatabaseSummary.DatabaseCount.HasValue? activeSnapshot.DatabaseSummary.DatabaseCount.Value.ToString(): "?");
                if (!(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId || ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId))
                {
                    decimal? dataSize = activeSnapshot.DatabaseSummary.DataFileSpaceUsed.BestDenomination(out denomination); AddPropertiesGridRow("Data Size", dataSize.HasValue ? string.Format("{0:F2} {1}", dataSize.Value, denomination) : "?");
                }
                decimal? logSize = activeSnapshot.DatabaseSummary.LogFileSpaceUsed.BestDenomination(out denomination);
                AddPropertiesGridRow("Log Size", logSize.HasValue? string.Format("{0:F2} {1}", logSize.Value, denomination): "?");
                if (activeSnapshot.VMConfig != null)
                {
                    AddPropertiesGridRow("VM Name", activeSnapshot.VMConfig.Name);
                    AddPropertiesGridRow("Virtualization Host", activeSnapshot.VMConfig.ESXHost.Name);
                    decimal? esxMemory = activeSnapshot.VMConfig.ESXHost.MemSize != null ? activeSnapshot.VMConfig.ESXHost.MemSize.BestDenomination(out denomination) : null;
                    AddPropertiesGridRow("Virtualization Host Memory", esxMemory.HasValue ? string.Format("{0:F2} {1}", esxMemory.Value, denomination): "?");
                    AddPropertiesGridRow("Virtualization Host Physical Processors", activeSnapshot.VMConfig.ESXHost.NumCPUCores.ToString());
                    AddPropertiesGridRow("Virtualization Host Logical Processors", activeSnapshot.VMConfig.ESXHost.NumCPUThreads.ToString());
                }

            }
            else
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
                {
                    AddPropertiesGridRow("Version", "?");
                    AddPropertiesGridRow("Edition", "?");
                    AddPropertiesGridRow("Host OS", "?");
                    AddPropertiesGridRow("Databases", "?");
                    AddPropertiesGridRow("Log Size", "?");
                }
               else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                {
                    AddPropertiesGridRow("Version", "?");
                    AddPropertiesGridRow("Edition", "?");
                    AddPropertiesGridRow("Running", "?");
                    AddPropertiesGridRow("Clustered", "?");
                    AddPropertiesGridRow("Processors", "?");
                    AddPropertiesGridRow("Host", "?");
                    AddPropertiesGridRow("Host OS", "?");
                    AddPropertiesGridRow("Databases", "?");
                    AddPropertiesGridRow("Log Size", "?");
                }
                else
                {
                    AddPropertiesGridRow("Version", "?");
                    AddPropertiesGridRow("Edition", "?");
                    AddPropertiesGridRow("Running", "?");
                    AddPropertiesGridRow("Clustered", "?");
                    AddPropertiesGridRow("Processors", "?");
                    AddPropertiesGridRow("Host", "?");
                    AddPropertiesGridRow("Host OS", "?");
                    AddPropertiesGridRow("Host Memory", "?");
                    AddPropertiesGridRow("Databases", "?");
                    AddPropertiesGridRow("Data Size", "?");
                    AddPropertiesGridRow("Log Size", "?");
                }
            }

            propertiesGridDataSource.ResumeBindingNotifications(true);
        }

        private void AddPropertiesGridRow(string name, string value)
        {
            propertiesGridDataSource.Rows.Add(new object[] { name, value });
        }

        private void UpdateDataWithHistoricalSnapshots(ServerOverview lastSnapshot)
        {
            if (lastSnapshot != null)
            {
                ServerDetailsView_Fill_Panel.Visible = true;
                chart.DataSource = chartHistoricalDataTable;
                UpdateCellColors(chartHistoricalDataTable);
                ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                      Properties.Resources.StatusWarningSmall);
                UpdateServerPropertiesGrid(lastSnapshot);
                ApplicationController.Default.SetCustomStatus(
                            String.Format("Server Details: {0} Item{1}",
                                          detailsGrid.Rows.Count,
                                          detailsGrid.Rows.Count == 1 ? string.Empty : "s")
                            );
                currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                currentHistoricalStartDateTime = HistoricalStartDateTime;
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                ServerDetailsView_Fill_Panel.Visible = false;
            }

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        private void UpdateCellColors(DataTable sourceTable)
        {
            if (sourceTable != null)
            {
                AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);

                if (alertConfig != null)
                {
                    detailsGrid.SuspendLayout();

                    foreach (UltraGridRow gridRow in detailsGrid.Rows.GetAllNonGroupByRows())
                    {
                        DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                        if (dataRowView == null)
                            return;
                        DataRow dataRow = dataRowView.Row;
                        if (!dataRow.IsNull("AlertMetric"))
                            UpdateCellColor(sourceTable, (Metric)dataRow["AlertMetric"], alertConfig, gridRow, 1);
                        gridRow.RefreshSortPosition();
                    }

                    detailsGrid.ResumeLayout();
                }

                detailsGrid.Refresh();
            }
        }

        private void UpdateCellColor(DataTable sourceTable, Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, int adjustmentMultiplier)
        {
            if (sourceTable != null && sourceTable.Rows.Count > 0)
            {
                DataRow valueRow = sourceTable.Rows[sourceTable.Rows.Count - 1];

                UltraGridCell stateCell = gridRow.Cells["State"];
                UltraGridCell infoThresholdCell = gridRow.Cells["Info Threshold"];
                UltraGridCell warningThresholdCell = gridRow.Cells["Warning Threshold"];
                UltraGridCell criticalThresholdCell = gridRow.Cells["Critical Threshold"];
                UltraGridCell metricCell = gridRow.Cells["Metric Type"];

                AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
                if (alertConfigItem == null || !alertConfigItem.ThresholdEntry.IsEnabled)
                {
                    DataRowView dataRowView = (DataRowView)stateCell.Row.ListObject;
                    if (dataRowView == null)
                        return;
                    DataRow dataRow = dataRowView.Row;
                    dataRow["State"] = 0;
                    stateCell.Appearance.ResetBackColor();
                    stateCell.Appearance.ResetForeColor();
                    infoThresholdCell.Value = null;
                    warningThresholdCell.Value = null;
                    criticalThresholdCell.Value = null;
                }
                else
                {
                    Threshold infoThreshold = alertConfigItem.ThresholdEntry.InfoThreshold;
                    if (infoThreshold.Enabled)
                    {
                        if (infoThreshold.Value != null)
                        {
                            object value = infoThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                infoThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                infoThresholdCell.Value = value.ToString();
                        }
                        else
                            infoThresholdCell.Value = String.Empty;
                    }

                    Threshold warningThreshold = alertConfigItem.ThresholdEntry.WarningThreshold;
                    if (warningThreshold.Enabled)
                    {
                        if (warningThreshold.Value != null)
                        {
                            object value = warningThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                warningThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                warningThresholdCell.Value = value.ToString();
                        }
                        else
                            warningThresholdCell.Value = String.Empty;
                    }
                    Threshold criticalThreshold = alertConfigItem.ThresholdEntry.CriticalThreshold;
                    if (criticalThreshold.Enabled)
                    {
                        if (criticalThreshold.Value != null)
                        {
                            object value = criticalThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                criticalThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                criticalThresholdCell.Value = value.ToString();
                        }
                        else
                            criticalThresholdCell.Value = String.Empty;
                    }

                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull("Value"))
                    {
                        dataRow["State"] = 0;
                        stateCell.Appearance.ResetBackColor();
                        stateCell.Appearance.ResetForeColor();
                    }
                    else
                    {
                        ServerDetailsMetrics sdm = (ServerDetailsMetrics)metricCell.Value;
                        object cellValue = valueRow[lookupTable[(int)sdm].Third.ColumnName];
                        if (cellValue is IComparable)
                        {
                            IComparable value = (IComparable)cellValue;
                            if (sdm == ServerDetailsMetrics.OsMetricsAvailability)
                            {
                                switch ((string)value)
                                {
                                    case "available":
                                        value = OSMetricsStatus.Available;
                                        break;
                                    case "disabled":
                                        value = OSMetricsStatus.Disabled;
                                        break;
                                    case "service unavailable":
                                        value = OSMetricsStatus.WMIServiceUnreachable;
                                        break;
                                    default:
                                        value = OSMetricsStatus.OLEAutomationUnavailable;
                                        break;
                                }
                            }

                            if (value != null && adjustmentMultiplier != 1)
                            {
                                double dbl = (double)Convert.ChangeType(value, typeof(double));
                                value = dbl * adjustmentMultiplier;
                            }

                            switch (alertConfigItem.GetSeverity(value))
                            {
                                case MonitoredState.Critical:
                                    dataRow["State"] = 4;
                                    stateCell.Appearance.BackColor = Color.Red;
                                    stateCell.Appearance.ForeColor = Color.White;
                                    break;
                                case MonitoredState.Warning:
                                    dataRow["State"] = 3;
                                    stateCell.Appearance.BackColor = Color.Gold;
                                    stateCell.Appearance.ResetForeColor();
                                    break;
                                case MonitoredState.Informational:
                                    dataRow["State"] = 2;
                                    stateCell.Appearance.BackColor = Color.Blue;
                                    stateCell.Appearance.ForeColor = Color.White;
                                    break;
                                case MonitoredState.OK:
                                    dataRow["State"] = 1;
                                    stateCell.Appearance.BackColor = Color.Green;
                                    stateCell.Appearance.ForeColor = Color.White;
                                    break;
                                default:
                                    dataRow["State"] = 0;
                                    stateCell.Appearance.ResetBackColor();
                                    stateCell.Appearance.ResetForeColor();
                                    break;
                            }
                        }
                        else
                        {
                            dataRow["State"] = 0;
                            stateCell.Appearance.ResetBackColor();
                            stateCell.Appearance.ResetForeColor();
                        }
                    }
                }
            }
        }


        #endregion

        #region helpers

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl ?? controls[0];
        }

        private void InitializeDataSources()
        {
            InitializeCategories();

            metricDefinitions = ApplicationModel.Default.MetricDefinitions;

            chartRealTimeDataTable = new DataTable();
            chartRealTimeDataTable.Columns.Add("Date", typeof(DateTime));
          
            detailsGridDataSource = new DataTable();
            detailsGridDataSource.Columns.Add(COL_VISIBLE, typeof(bool));
            detailsGridDataSource.Columns.Add("Metric", typeof(string));
            detailsGridDataSource.Columns.Add(COL_VALUE, typeof(string));
            detailsGridDataSource.Columns.Add("Reference Range", typeof(string));
            detailsGridDataSource.Columns.Add("State", typeof(Int32));
            detailsGridDataSource.Columns.Add("Category", typeof(string));
            detailsGridDataSource.Columns.Add("Info Threshold", typeof(string));
            detailsGridDataSource.Columns.Add("Warning Threshold", typeof(string));
            detailsGridDataSource.Columns.Add("Critical Threshold", typeof(string));
            detailsGridDataSource.Columns.Add("Metric Type", typeof(object));
            detailsGridDataSource.Columns.Add("AlertMetric", typeof(int));
            detailsGridDataSource.Columns.Add("Custom Counter", typeof(string));

            InitializeMetric(ServerDetailsMetrics.AvailableMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AvailableMemory),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.AvailableMemoryPercent,
                Metric.OSMemoryUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AvailableMemoryPercent),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.BlockedSessions,
                Metric.BlockedSessions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BlockedSessions),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.BufferCacheHitRatio,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BufferCacheHitRatio),
                typeof(float));

            InitializeMetric(ServerDetailsMetrics.BufferCacheSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BufferCacheSize),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.CheckpointWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CheckpointWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.CheckpointWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CheckpointWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ClientComputers,
                Metric.ClientComputers,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ClientComputers),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.Connections,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.Connections),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ConnectionsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ConnectionsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.CPUActivity,
                Metric.SQLCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CPUActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.DatabaseCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DatabaseCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.DataFileCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.DataFileSpaceAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileSpaceAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.DataFileSpaceUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileSpaceUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.DiskQueueLength,
                Metric.OSDiskAverageDiskQueueLength,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskQueueLength),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.DiskTime,
                Metric.OSDiskPhysicalDiskTimePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskTime),
                typeof(double));

            //InitializeMetric(ServerDetailsMetrics.DtcServiceStatus,
            //    Metric.DtcServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DtcServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.FullScans,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullScans),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.FullScansPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullScansPerSecond),
                typeof(long));

            //InitializeMetric(ServerDetailsMetrics.FullTextServiceStatus,
            //    Metric.FullTextServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullTextServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.LazyWriterWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LazyWriterWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LazyWriterWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LazyWriterWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LeadBlockers,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LeadBlockers),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.LockWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockWaits),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LockWaitsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockWaitsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LogFileCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.LogFileSpaceAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileSpaceAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.LogFileSpaceUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileSpaceUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.LogFlushes,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFlushes),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LogFlushesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFlushesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.MaintenanceModeEnabled,
                Metric.MaintenanceMode,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.MaintenanceModeEnabled),
                typeof(OptionStatus));

            InitializeMetric(ServerDetailsMetrics.OleAutomationStatus,
                Metric.OLEAutomationStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OleAutomationStatus),
                typeof(OptionStatus));

            InitializeMetric(ServerDetailsMetrics.OpenTransactions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OpenTransactions),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.OsMetricsAvailability,
                Metric.OSMetricsStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsMetricsAvailability),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.OsCpuPrivilegedActivity,
                Metric.OSCPUPrivilegedTimePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsCpuPrivilegedActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.OsCpuUserTime,
                Metric.OSUserCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsCpuUserTime),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.OsTotalProcessorActivity,
                Metric.OSCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsTotalProcessorActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.PacketErrors,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketErrors),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketErrorsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketErrorsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsReceived,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsReceived),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsReceivedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsReceivedPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsSent,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsSent),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsSentPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsSentPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLifeExpectancy,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLifeExpectancy),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLookups,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLookups),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLookupsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLookupsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageReads,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageReads),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageReadsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageReadsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PagesPerSecond,
                Metric.OSMemoryPagesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PagesPerSecond),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.PageSplits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageSplits),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageSplitsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageSplitsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PhysicalMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PhysicalMemory),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.ProcedureCacheHitRatio,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcedureCacheHitRatio),
                typeof(float));

            InitializeMetric(ServerDetailsMetrics.ProcedureCacheSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcedureCacheSize),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.ProcessorQueueLength,
                Metric.OSCPUProcessorQueueLength,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcessorQueueLength),
                typeof(ulong));

            InitializeMetric(ServerDetailsMetrics.ReadAheadPages,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadAheadPages),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ReadAheadPagesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadAheadPagesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ReadWriteErrors,
                Metric.ReadWriteErrors,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadWriteErrors),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ResponseTime,
                Metric.ServerResponseTime,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ResponseTime),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.RunningSince,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.RunningSince),
                typeof(DateTime));

            InitializeMetric(ServerDetailsMetrics.SqlCompilations,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlCompilations),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlCompilationsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlCompilationsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlRecompilations,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlRecompilations),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlRecompilationsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlRecompilationsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryUsedPercent,
                Metric.SQLMemoryUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryUsedPercent),
                typeof(decimal));

            //InitializeMetric(ServerDetailsMetrics.AgentServiceStatus,
            //    Metric.AgentServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AgentServiceStatus),
            //    typeof(ServiceState));

            //InitializeMetric(ServerDetailsMetrics.SqlServerServiceStatus,
            //    Metric.SqlServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlServerServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.SystemProcesses,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SystemProcesses),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.SystemProcessesConsumingCPU,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SystemProcessesConsumingCPU),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.ServerVersion,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ServerVersion),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.Batches,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.Batches),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.BatchesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BatchesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.UserConnectionsPercent,
                Metric.UserConnectionPct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserConnectionsPercent),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.UserProcesses,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserProcesses),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.UserProcessesConsumingCPU,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserProcessesConsumingCPU),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.WMIStatus,
                Metric.WMIStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WMIStatus),
                typeof(OptionStatus));

            InitializeMetric(ServerDetailsMetrics.WorkfilesCreated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorkfilesCreated),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorkfilesCreatedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorkfilesCreatedPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorktablesCreated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorktablesCreated),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorktablesCreatedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorktablesCreatedPerSecond),
                typeof(long));

            if (!vmCountersInitialized)
            {
                InitializeVmMetrics();
            }

            chartRealTimeDataTable.Columns.Add("IsHistorical", typeof(bool));
            chartHistoricalDataTable = chartRealTimeDataTable.Clone();

            detailsGrid.SetDataBinding(detailsGridDataSource, string.Empty);
            detailsGrid.DisplayLayout.Bands[0].Columns["Metric"].PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
            detailsGrid.DisplayLayout.Bands[0].Columns["Category"].PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
            detailsGrid.DisplayLayout.Bands[0].Columns["AlertMetric"].Hidden = true;
            detailsGrid.DisplayLayout.Bands[0].Columns["AlertMetric"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            detailsGrid.DisplayLayout.Bands[0].Columns["State"].ValueList = detailsGrid.DisplayLayout.ValueLists["State"];
            detailsGrid.DisplayLayout.Bands[0].Columns["Reference Range"].NullText = "N/A";
        }

        private void InitializeVmMetrics()
        {
            // Virtualization Metrics
            InitializeMetric(ServerDetailsMetrics.VmNumCPUs,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmNumCPUs),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.VmCPULimit,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPULimit),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.VmCPUReserve,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUReserve),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemSize),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemLimit,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemLimit),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.VmMemReserved,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemReserved),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxCPUMHz,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxCPUMHz),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.EsxNumCPUCores,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumCPUCores),
                typeof(short));

            InitializeMetric(ServerDetailsMetrics.EsxNumCPUThreads,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumCPUThreads),
                typeof(short));

            InitializeMetric(ServerDetailsMetrics.EsxMemSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemSize),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxNumNICs,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNumNICs),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.EsxBootTime,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxBootTime),
                typeof(DateTime));

            InitializeMetric(ServerDetailsMetrics.VmBootTime,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmBootTime),
                typeof(DateTime));

            InitializeMetric(ServerDetailsMetrics.VmCPUUsage,
                Metric.VmCPUUtilization,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUUsage),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.VmCPUUsageMHz,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUUsageMHz),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.VmCPUReady,
                Metric.VmCPUReadyWaitTime,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUReady),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmCPUSwapWait,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUSwapWait),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemSwapInRate,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemSwapInRate),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemSwapOutRate,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemSwapOutRate),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemSwapped,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemSwapped),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemActive,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemActive),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemConsumed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemConsumed),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemGranted,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemGranted),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemBalooned,
                Metric.VmReclaimedMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemBalooned),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmMemUsage,
                Metric.VmMemoryUtilization,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemUsage),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.VmDiskRead,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmDiskRead),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmDiskWrite,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmDiskWrite),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmDiskUsage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmDiskUsage),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmNetUsage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmNetUsage),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmNetTransmitted,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmNetTransmitted),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.VmNetReceived,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmNetReceived),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxCPUUsage,
                Metric.VmESXCPUUtilization,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxCPUUsage),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.EsxCPUUsageMHz,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxCPUUsageMHz),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.EsxMemSwapInRate,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemSwapInRate),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemSwapOutRate,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemSwapOutRate),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemActive,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemActive),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemConsumed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemConsumed),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemGranted,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemGranted),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemBalooned,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemBalooned),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxMemUsage,
                Metric.VmESXMemoryUsage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxMemUsage),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.EsxDiskRead,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskRead),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxDiskWrite,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskWrite),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxDiskUsage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskUsage),
                typeof(long));

            //InitializeMetric(ServerDetailsMetrics.EsxDiskDeviceLatency,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskDeviceLatency),
            //    typeof(long));

            //InitializeMetric(ServerDetailsMetrics.EsxDiskKernelLatency,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskKernelLatency),
            //    typeof(long));

            //InitializeMetric(ServerDetailsMetrics.EsxDiskQueueLatency,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskQueueLatency),
            //    typeof(long));

            //InitializeMetric(ServerDetailsMetrics.EsxDiskTotalLatency,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxDiskTotalLatency),
            //    typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxNetUsage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNetUsage),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxNetTransmitted,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNetTransmitted),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.EsxNetReceived,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.EsxNetReceived),
                typeof(long));

            vmCountersInitialized = true;
        }


        private void ZapVmMetrics()
        {
            // Virtualization Metrics
            ZapMetric(ServerDetailsMetrics.VmNumCPUs);
            ZapMetric(ServerDetailsMetrics.VmCPULimit);
            ZapMetric(ServerDetailsMetrics.VmCPUReserve);
            ZapMetric(ServerDetailsMetrics.VmMemSize);
            ZapMetric(ServerDetailsMetrics.VmMemLimit);
            ZapMetric(ServerDetailsMetrics.VmMemReserved);
            ZapMetric(ServerDetailsMetrics.EsxCPUMHz);
            ZapMetric(ServerDetailsMetrics.EsxNumCPUCores);
            ZapMetric(ServerDetailsMetrics.EsxNumCPUThreads);
            ZapMetric(ServerDetailsMetrics.EsxMemSize);
            ZapMetric(ServerDetailsMetrics.EsxNumNICs);
            ZapMetric(ServerDetailsMetrics.EsxBootTime);
            ZapMetric(ServerDetailsMetrics.VmBootTime);
            ZapMetric(ServerDetailsMetrics.VmCPUUsage);
            ZapMetric(ServerDetailsMetrics.VmCPUUsageMHz);
            ZapMetric(ServerDetailsMetrics.VmCPUReady);
            ZapMetric(ServerDetailsMetrics.VmCPUSwapWait);
            ZapMetric(ServerDetailsMetrics.VmMemSwapInRate);
            ZapMetric(ServerDetailsMetrics.VmMemSwapOutRate);
            ZapMetric(ServerDetailsMetrics.VmMemSwapped);
            ZapMetric(ServerDetailsMetrics.VmMemActive);
            ZapMetric(ServerDetailsMetrics.VmMemConsumed);
            ZapMetric(ServerDetailsMetrics.VmMemGranted);
            ZapMetric(ServerDetailsMetrics.VmMemBalooned);
            ZapMetric(ServerDetailsMetrics.VmMemUsage);
            ZapMetric(ServerDetailsMetrics.VmDiskRead);
            ZapMetric(ServerDetailsMetrics.VmDiskWrite);
            ZapMetric(ServerDetailsMetrics.VmDiskUsage);
            ZapMetric(ServerDetailsMetrics.VmNetUsage);
            ZapMetric(ServerDetailsMetrics.VmNetTransmitted);
            ZapMetric(ServerDetailsMetrics.VmNetReceived);
            ZapMetric(ServerDetailsMetrics.EsxCPUUsage);
            ZapMetric(ServerDetailsMetrics.EsxCPUUsageMHz);
            ZapMetric(ServerDetailsMetrics.EsxMemSwapInRate);
            ZapMetric(ServerDetailsMetrics.EsxMemSwapOutRate);
            ZapMetric(ServerDetailsMetrics.EsxMemActive);
            ZapMetric(ServerDetailsMetrics.EsxMemConsumed);
            ZapMetric(ServerDetailsMetrics.EsxMemGranted);
            ZapMetric(ServerDetailsMetrics.EsxMemBalooned);
            ZapMetric(ServerDetailsMetrics.EsxMemUsage);
            ZapMetric(ServerDetailsMetrics.EsxDiskRead);
            ZapMetric(ServerDetailsMetrics.EsxDiskWrite);
            ZapMetric(ServerDetailsMetrics.EsxDiskUsage);
            //ZapMetric(ServerDetailsMetrics.EsxDiskDeviceLatency);
            //ZapMetric(ServerDetailsMetrics.EsxDiskKernelLatency);
            //ZapMetric(ServerDetailsMetrics.EsxDiskQueueLatency);
            //ZapMetric(ServerDetailsMetrics.EsxDiskTotalLatency);
            ZapMetric(ServerDetailsMetrics.EsxNetUsage);
            ZapMetric(ServerDetailsMetrics.EsxNetTransmitted);
            ZapMetric(ServerDetailsMetrics.EsxNetReceived);

            vmCountersInitialized = false;
        }

        private void InitializeMetric(ServerDetailsMetrics metricType, string metricName, Type dataType)
        {
            InitializeMetric(metricType, null, metricName, dataType);
        }

        private void InitializeMetric(ServerDetailsMetrics metricType, Metric? metric, string metricName, Type dataType)
        {
            DataRow metricRow;
            Triple<string, DataRow, DataColumn> lookupItem;
            if (lookupTable.TryGetValue((int)metricType, out lookupItem))
            {
                if (lookupItem.Second == null)
                {
                    // create a new row in in the table
                    metricRow = detailsGridDataSource.Rows.Add(new object[] { false, metricName });
                    metricRow["AlertMetric"] = metric == null ? (object)DBNull.Value : metric.Value;
                    metricRow["Metric Type"] = metricType;
                    metricRow["State"] = 0;
                    metricRow["Custom Counter"] = "No";
                    SetCategory(metricType, metricRow);
                    lookupItem.Second = metricRow;
                    // replace the lookupItem 
                    lookupTable.Remove((int)metricType);
                    lookupTable.Add((int)metricType, lookupItem);
                }
            }
            else
            {
                metricRow = detailsGridDataSource.Rows.Add(new object[] { false, metricName });
                metricRow["AlertMetric"] = metric == null ? (object)DBNull.Value : metric.Value;
                metricRow["Metric Type"] = metricType;
                metricRow["State"] = 0;
                metricRow["Custom Counter"] = "No";
                SetCategory(metricType, metricRow);
                DataColumn metricColumn = chartRealTimeDataTable.Columns.Add(metricName, dataType);
                
                lookupTable.Add((int)metricType, new Triple<string, DataRow, DataColumn>(metricName, metricRow, metricColumn));
            }
        }

        private void ZapMetric(ServerDetailsMetrics metricType)
        {
            Triple<string, DataRow, DataColumn> lookupItem;
            if (lookupTable.TryGetValue((int)metricType, out lookupItem))
            {
                RemoveMetricFromChart((int)metricType, false);

                if (lookupItem.Second != null)
                {
                    // delete the row
                    //lookupItem.Second.Delete();
                    // null row reference
                    lookupItem.Second = null;
                    // replace the lookupItem 
                    lookupTable.Remove((int)metricType);
                    lookupTable.Add((int)metricType, lookupItem);
                }
                else
                {
                    Log.InfoCall("Woops");
                }
            }
            else
            {
                Log.Info("Woops");
            }
        }

        private void InitializeCategories()
        {
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --change the size to 137 after adding new services
            MetricsCategories = new string[(int)ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus + 1];
            MetricsCategories[(int)ServerDetailsMetrics.AvailableMemory] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.AvailableMemoryPercent] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.Batches] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.BatchesPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.BlockedSessions] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.BufferCacheHitRatio] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.BufferCacheSize] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.CheckpointWrites] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.CheckpointWritesPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ClientComputers] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.Connections] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ConnectionsPerSecond] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.CPUActivity] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DatabaseCount] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileCount] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileSpaceAllocated] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileSpaceUsed] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DiskQueueLength] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DiskTime] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.DtcServiceStatus] = servicesCategory;
            MetricsCategories[(int)ServerDetailsMetrics.FullScans] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.FullScansPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.FullTextServiceStatus] = servicesCategory;
            MetricsCategories[(int)ServerDetailsMetrics.AgentServiceStatus] = servicesCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlServerServiceStatus] = servicesCategory;
            MetricsCategories[(int)ServerDetailsMetrics.MaintenanceModeEnabled] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OleAutomationStatus] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.WMIStatus] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LazyWriterWrites] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LazyWriterWritesPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LeadBlockers] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LockWaits] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LockWaitsPerSecond] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileCount] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileSpaceAllocated] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileSpaceUsed] = diskUsageCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LogFlushes] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.LogFlushesPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OpenTransactions] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OsMetricsAvailability] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OsCpuPrivilegedActivity] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OsCpuUserTime] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.OsTotalProcessorActivity] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketErrors] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketErrorsPerSecond] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsReceived] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsReceivedPerSecond] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsSent] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsSentPerSecond] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageLifeExpectancy] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageLookups] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageLookupsPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageReads] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageReadsPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PagesPerSecond] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageSplits] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageSplitsPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageWrites] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PageWritesPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.PhysicalMemory] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcedureCacheHitRatio] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcedureCacheSize] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcessorQueueLength] = processorCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ReadAheadPages] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ReadAheadPagesPerSecond] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ReadWriteErrors] = diskIOCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ResponseTime] = networkCategory;
            MetricsCategories[(int)ServerDetailsMetrics.RunningSince] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.ServerVersion] = generalCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlCompilations] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlCompilationsPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryAllocated] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryUsed] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryUsedPercent] = memoryCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlRecompilations] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlRecompilationsPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SystemProcesses] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SystemProcessesConsumingCPU] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.UserConnectionsPercent] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.UserProcesses] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.UserProcessesConsumingCPU] = sessionsCategory;
            MetricsCategories[(int)ServerDetailsMetrics.WorkfilesCreated] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.WorkfilesCreatedPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.WorktablesCreated] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.WorktablesCreatedPerSecond] = sqlActivityCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmNumCPUs] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPULimit] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUReserve] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemSize] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemLimit] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemReserved] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxCPUMHz] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNumCPUCores] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNumCPUThreads] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemSize] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNumNICs] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxBootTime] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmBootTime] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUUsageMHz] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUReady] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUSwapWait] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemSwapInRate] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemSwapOutRate] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemSwapped] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemActive] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemConsumed] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemGranted] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemBalooned] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmDiskRead] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmDiskWrite] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmDiskUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmNetUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmNetTransmitted] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.VmNetReceived] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxCPUUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxCPUUsageMHz] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemSwapInRate] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemSwapOutRate] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemActive] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemConsumed] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemGranted] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemBalooned] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxMemUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxDiskRead] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxDiskWrite] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxDiskUsage] = virtualizationCategory;
            //MetricsCategories[(int)ServerDetailsMetrics.EsxDiskDeviceLatency] = virtualizationCategory;
            //MetricsCategories[(int)ServerDetailsMetrics.EsxDiskKernelLatency] = virtualizationCategory;
            //MetricsCategories[(int)ServerDetailsMetrics.EsxDiskQueueLatency] = virtualizationCategory;
            //MetricsCategories[(int)ServerDetailsMetrics.EsxDiskTotalLatency] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNetUsage] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNetTransmitted] = virtualizationCategory;
            MetricsCategories[(int)ServerDetailsMetrics.EsxNetReceived] = virtualizationCategory;
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add the new services in the Service category
            MetricsCategories[(int)ServerDetailsMetrics.SQLBrowserServiceStatus] = servicesCategory;
            MetricsCategories[(int)ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus] = servicesCategory;
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add the new services in the Service category
        }

        #region grid

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - Server Details as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            detailsGrid.SuspendLayout();
            bool selectionHidden = detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden;
            detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden = true;

            ultraPrintPreviewDialog.ShowDialog();

            detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden = selectionHidden;
            detailsGrid.ResumeLayout();
        }

        private void SaveGrid()
        {
            bool selectionHidden = detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "ServerDetails";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    detailsGrid.SuspendLayout();
                    detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden = true;
                    ultraGridExcelExporter.Export(detailsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }

            detailsGrid.DisplayLayout.Bands[0].Columns[COL_VISIBLE].Hidden = selectionHidden;
            detailsGrid.ResumeLayout();
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            detailsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            detailsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(detailsGrid);
            dialog.Show(this);
        }

        private string GridNumberFormat<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return input.Value.ToString("N2", CultureInfo.CurrentCulture.NumberFormat);
            }
            else
            {
                return null;
            }
        }

        private string GridNumberFormatNoDecimal<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return input.Value.ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
            }
            else
            {
                return null;
            }
        }

        private string GridPercentFormat<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return GridNumberFormat(input) + '%';
            }
            else
            {
                return null;
            }
        }

        private string GridDataRateFormat<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return GridNumberFormatNoDecimal(input) + " KB/s";
            }
            else
            {
                return null;
            }
        }

        private string GridCPUSpeedFormat<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return GridNumberFormatNoDecimal(input) + " MHz";
            }
            else
            {
                return null;
            }
        }

        private string GridDurationMsFormat<T>(Nullable<T> input) where T : struct, IFormattable
        {
            if (input.HasValue)
            {
                return GridNumberFormatNoDecimal(input) + " ms";
            }
            else
            {
                return null;
            }
        }

        private void SetCategory(ServerDetailsMetrics metricType, DataRow metricRow)
        {
            metricRow["Category"] = MetricsCategories[(int)metricType];
        }

        #endregion

        #region chart

        private void InitializeChart()
        {
            chart.DataSource = chartRealTimeDataTable;

            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chart.LegendBox.PlotAreaOnly = false;
            chart.Printer.Orientation = PageOrientation.Landscape;
            chart.Printer.Compress = true;
            chart.Printer.ForceColors = true;
            chart.Printer.Document.DocumentName = "Server Details Chart";
            chart.ToolBar.RemoveAt(0);
            chart.ToolTipFormat = "%s\n%v\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            chart.DataSourceSettings.Fields.Add(dateFieldMap);

            AddMetricToChart(ServerDetailsMetrics.ResponseTime);
            AddMetricToChart(ServerDetailsMetrics.CPUActivity);
            AddMetricToChart(ServerDetailsMetrics.SqlMemoryUsed);
            AddMetricToChart(ServerDetailsMetrics.PagesPerSecond);
            InitalizeDrilldown(chart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void AddSelectedMetricToChart()
        {
            if (detailsGrid.Selected.Rows.Count == 1)
            {
                if (detailsGrid.Selected.Rows[0].ListObject is DataRowView)
                {
                    DataRowView dataRow = detailsGrid.Selected.Rows[0].ListObject as DataRowView;
                    object metricType = dataRow["Metric Type"];
                    if (metricType is ServerDetailsMetrics)
                        AddMetricToChart((ServerDetailsMetrics)metricType);
                    else if (metricType is int)
                        AddMetricToChart((int)metricType);
                }
            }
        }

        private void AddMetricToChart(ServerDetailsMetrics metricType)
        {
            if (
                (metricType == ServerDetailsMetrics.ServerVersion)
                || (metricType == ServerDetailsMetrics.MaintenanceModeEnabled)
                || (metricType == ServerDetailsMetrics.OleAutomationStatus)
                || (metricType == ServerDetailsMetrics.WMIStatus)
                || (metricType == ServerDetailsMetrics.OsMetricsAvailability)
                || (metricType == ServerDetailsMetrics.RunningSince)
                || (metricType == ServerDetailsMetrics.AgentServiceStatus)
                || (metricType == ServerDetailsMetrics.DtcServiceStatus)
                || (metricType == ServerDetailsMetrics.FullTextServiceStatus)
                || (metricType == ServerDetailsMetrics.SqlServerServiceStatus)
                || (metricType == ServerDetailsMetrics.SQLBrowserServiceStatus)  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add the newly added sql services
                || (metricType == ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus)
                )
            {
                ApplicationMessageBox.ShowInfo(this, "Only numeric metric types can be added to the chart.");
            }
            else
                AddMetricToChart((int)metricType);
        }

        private void AddMetricToChart(int metricType)
        {
            if (!chartFieldMapLookupTable.ContainsKey((int)metricType))
            {
                Triple<string, DataRow, DataColumn> metricInfo;

                if (lookupTable.TryGetValue(metricType, out metricInfo))
                {
                    FieldMap fieldMap;

                    metricInfo.Second[COL_VISIBLE] = true;
                    if (Enum.IsDefined(typeof(ServerDetailsMetrics), metricType))
                        fieldMap = new FieldMap(metricInfo.First, FieldUsage.Value);
                    else
                    {
                        fieldMap = new FieldMap("CustomCounter" + metricType.ToString(), FieldUsage.Value);
                        fieldMap.DisplayName = metricInfo.Second["Metric"].ToString();
                    }

                    chartFieldMapLookupTable.Add(metricType, fieldMap);
                    chart.DataSourceSettings.Fields.Add(fieldMap);
                    chart.DataSourceSettings.ReloadData();
                }

                chart.Visible = true;
                ChartPanelVisible = true;
            }
        }

        private void RemoveSelectedMetricFromChart()
        {
            if (detailsGrid.Selected.Rows.Count == 1)
            {
                if (detailsGrid.Selected.Rows[0].ListObject is DataRowView)
                {
                    DataRowView dataRow = detailsGrid.Selected.Rows[0].ListObject as DataRowView;
                    int metricType = (int)dataRow["Metric Type"];
                    RemoveMetricFromChart(metricType, false);
                }
            }
        }

        private void RemoveMetricFromChart(int metricType, bool supressFieldMapLookupTableModifications)
        {
            Triple<string, DataRow, DataColumn> metricInfo;
            FieldMap existingFieldMap;

            if (lookupTable.TryGetValue(metricType, out metricInfo))
            {
                metricInfo.Second[COL_VISIBLE] = false;
            }

            if (chartFieldMapLookupTable.TryGetValue(metricType, out existingFieldMap))
            {
                chart.DataSourceSettings.Fields.Remove(existingFieldMap);

                if (!supressFieldMapLookupTableModifications)
                {
                    chartFieldMapLookupTable.Remove(metricType);
                }

                if (chart.DataSourceSettings.Fields.Count == 1)
                {
                    chart.Visible = false;
                }
                else
                {
                    chart.DataSourceSettings.ReloadData();
                    chart.Invalidate();
                }
            }
        }

        private void RemoveAllMetricsFromChart()
        {
            chart.Visible = false;

            foreach (int metricType in chartFieldMapLookupTable.Keys)
            {
                RemoveMetricFromChart(metricType, true);
            }

            chartFieldMapLookupTable.Clear();
        }

        private void ToggleChartToolbar(bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, "Server Details", ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, chart, "ServerDetails");
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, "Server Details", "ServerDetails");
        }

        private void MaximizeChart()
        {
            horzSplitContainer.Visible = false;
            horzSplitContainer.Panel2.Controls.Remove(chartContentPanel);
            maximizeChartButton.Visible = false;
            restoreChartButton.Visible = true;
            contentPanel.Controls.Add(chartContentPanel);
        }

        private void RestoreChart()
        {
            contentPanel.Controls.Remove(chartContentPanel);
            maximizeChartButton.Visible = true;
            restoreChartButton.Visible = false;
            horzSplitContainer.Panel2.Controls.Add(chartContentPanel);
            horzSplitContainer.Visible = true;
        }

        private void UpdateChartDataFilter()
        {
            if (chartRealTimeDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                chartRealTimeDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                chartRealTimeDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        private void GroomHistoryData()
        {
            if (chartRealTimeDataTable != null)
            {
                DataRow[] groomedRows = chartRealTimeDataTable.Select(ServerSummaryHistoryData.GetGroomingFilter("Date"));
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                chartRealTimeDataTable.AcceptChanges();
            }
        }

        #endregion

        #endregion

        #region events

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void maximizeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart();
        }

        private void restoreChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart();
        }

        private void closeChartButton_Click(object sender, EventArgs e)
        {
            ChartPanelVisible = false;
            RestoreChart();
        }

        #region grid

        private void detailsGrid_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                if (e.Row.Cells[COL_VISIBLE].Style == Infragistics.Win.UltraWinGrid.ColumnStyle.Default)
                {
                    ChartPanelVisible = true;
                    bool isIncludedInChart = (bool)e.Row.Cells[COL_VISIBLE].Value;
                    if (isIncludedInChart)
                    {
                        RemoveSelectedMetricFromChart();
                    }
                    else
                    {
                        AddSelectedMetricToChart();
                    }
                    e.Row.Refresh();
                }
            }
        }

        private void detailsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridColumn col;
            UltraGridBand band;


            band = detailsGrid.DisplayLayout.Bands[0];
            col = band.Columns[COL_VISIBLE];
            col.Header.Caption = string.Empty;
            col.Header.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ChartGridHeader;
            col.Header.Appearance.ImageHAlign = HAlign.Center;
            col.Header.Fixed = true;
            col.Width = 33;

            col = band.Columns["State"];
            detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(col, true);

            col = band.Columns["Metric"];
            col.Header.Fixed = true;
            col.Width = 195;
            band.SortedColumns.Add(col, false);

            col = band.Columns["Reference Range"];
            col.Width = 105;

            col = band.Columns["Metric Type"];
            col.Hidden = true;
            col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

            col = band.Columns["Custom Counter"];
            col.Hidden = false;

            // all rows shown by default
            band.ColumnFilters[col].ClearFilterConditions();
            // never show row filter crap in the column headers
            band.Override.AllowRowFiltering = DefaultableBoolean.False;
        }

        private void SetCustomCounterFiltering(bool useFilter)
        {
            /* The strategery used for the chart is to use the chartFieldMapLookupTable to 
             * keep up with the total set of fields that should be showing in the chart.  When 
             * only showing custom counters, remove all the FieldMap items from the chart that
             * do not correspond to custom counters.  When going to show all counters, iterate the
             * chartFieldMapLookupTable and add the FieldMap items back to the chart.
             */

            UltraGridBand band = detailsGrid.DisplayLayout.Bands[0];
            if (useFilter)
            {
                // filter the data in the details view
                if (band.ColumnFilters["Custom Counter"].FilterConditions.Count == 0)
                    band.ColumnFilters["Custom Counter"].FilterConditions.Add(new FilterCondition(FilterComparisionOperator.Equals, "Yes"));
                // unmap all chart metrics that are not for custom counters
                Triple<string, DataRow, DataColumn> tableMapping;
                FieldMap fieldMap;
                foreach (int metric in Algorithms.ToArray(chartFieldMapLookupTable.Keys))
                {
                    if (lookupTable.TryGetValue(metric, out tableMapping))
                    {
                        bool isCustom = "Yes".Equals(tableMapping.Second["Custom Counter"]);
                        if (!isCustom)
                        {
                            if (chartFieldMapLookupTable.TryGetValue(metric, out fieldMap))
                            {
                                chart.DataSourceSettings.Fields.Remove(fieldMap);
                            }
                        }
                    }
                }
            }
            else
            {
                // show all data in the details view
                band.ColumnFilters.ClearAllFilters();
                foreach (int metric in Algorithms.ToArray(chartFieldMapLookupTable.Keys))
                {
                    Triple<string, DataRow, DataColumn> tableMapping;
                    FieldMap fieldMap;
                    if (lookupTable.TryGetValue(metric, out tableMapping))
                    {
                        if (chartFieldMapLookupTable.TryGetValue(metric, out fieldMap))
                        {
                            tableMapping.Second[COL_VISIBLE] = true;
                            if (!chart.DataSourceSettings.Fields.Contains(fieldMap))
                                chart.DataSourceSettings.Fields.Add(fieldMap);
                        }
                    }
                }
            }

            detailsGrid.Visible = detailsGrid.Rows.FilteredInRowCount > 0;

            if (chart.DataSourceSettings.Fields.Count == 1)
            {
                chart.Visible = false;
            }
            else
            {
                chart.DataSourceSettings.ReloadData();
                if (!chart.Visible)
                    chart.Visible = true;
                else
                    chart.Invalidate();
            }
        }

        private void detailsGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                DataRowView row = (DataRowView)e.Row.ListObject;
                if (unselectableMetrics.Contains((string)row["Metric"]))
                {
                    row[COL_VISIBLE] = System.DBNull.Value;
                    e.Row.Cells[0].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                }
            }
        }

        private void detailsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedElement is CheckIndicatorUIElement)
                    {
                        RowUIElement selectedRowUIElement = selectedElement.GetAncestor(typeof(RowUIElement)) as RowUIElement;

                        if (selectedRowUIElement != null && selectedRowUIElement.Row.ListObject is DataRowView)
                        {
                            DataRowView dataRow = selectedRowUIElement.Row.ListObject as DataRowView;
                            bool isIncludedInChart = (bool)dataRow[COL_VISIBLE];

                            if (isIncludedInChart)
                            {
                                RemoveSelectedMetricFromChart();
                            }
                            else
                            {
                                AddSelectedMetricToChart();
                            }
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");

                    object contextObject = selectedElement.GetContext();

                    if (contextObject is ColumnHeader)
                    {
                        ColumnHeader columnHeader =
                            contextObject as ColumnHeader;
                        selectedColumn = columnHeader.Column;
                        ((Infragistics.Win.UltraWinToolbars.StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                    }
                    else
                    {
                        contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                        if (contextObject is RowUIElement)
                        {
                            RowUIElement row = contextObject as RowUIElement;
                            row.Row.Selected = true;

                            if (row.Row.ListObject is DataRowView)
                            {
                                DataRowView dataRow = row.Row.ListObject as DataRowView;

                                if (dataRow != null && dataRow["Metric Type"] is ServerDetailsMetrics)
                                {
                                    if (unselectableMetrics.Contains((string)dataRow["Metric"]))
                                    {
                                        toolbarsManager.Tools["addMetricToChartButton"].SharedProps.Visible = false;
                                        toolbarsManager.Tools["removeMetricFromChartButton"].SharedProps.Visible = false;
                                    }
                                    else if (chartFieldMapLookupTable.ContainsKey((int)(ServerDetailsMetrics)dataRow["Metric Type"]))
                                    {
                                        toolbarsManager.Tools["addMetricToChartButton"].SharedProps.Visible = false;
                                        toolbarsManager.Tools["removeMetricFromChartButton"].SharedProps.Visible = true;
                                    }
                                    else
                                    {
                                        toolbarsManager.Tools["addMetricToChartButton"].SharedProps.Visible = true;
                                        toolbarsManager.Tools["removeMetricFromChartButton"].SharedProps.Visible = false;
                                    }
                                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "metricContextMenu");
                                }
                                toolbarsManager.Tools["configureAlertsButton"].SharedProps.Visible = !dataRow.Row.IsNull("AlertMetric");
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "addMetricToChartButton":
                    AddSelectedMetricToChart();
                    break;
                case "removeMetricFromChartButton":
                    RemoveSelectedMetricFromChart();
                    break;
                case "showChartToolbarButton":
                    chart.ToolBar.Visible = true;
                    break;
                case "hideChartToolbarButton":
                    chart.ToolBar.Visible = false;
                    break;
                case "removeAllMetricsFromChartButton":
                    RemoveAllMetricsFromChart();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(((StateButtonTool)e.Tool).Checked);
                    break;
                case "printChartButton":
                    PrintChart();
                    break;
                case "exportChartDataButton":
                    SaveChartData();
                    break;
                case "exportChartImageButton":
                    SaveChartImage();
                    break;
                case "configureAlertsButton":
                    ConfigureAlerts();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    break;
            }

            if (e.Tool.Key == "metricContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = detailsGrid.Rows.Count > 0 && detailsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {

                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(detailsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void ConfigureAlerts()
        {
            if (detailsGrid.Selected.Rows.Count == 1)
            {
                if (detailsGrid.Selected.Rows[0].ListObject is DataRowView)
                {
                    DataRowView dataRow = detailsGrid.Selected.Rows[0].ListObject as DataRowView;
                    if (!dataRow.Row.IsNull("AlertMetric"))
                    {
                        try
                        {
                            using (AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false))
                            {
                                dialog.Select((Metric)dataRow["AlertMetric"]);
                                dialog.ShowDialog(ParentForm);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }

                    }
                }
            }
        }

        private void ServerDetailsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
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
            ApplicationController.Default.SetActiveViewToRealTimeMode();
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

        internal Selection Filter
        {
            get { return filter; }
            set
            {
                this.filter = value;
                SetCustomCounterFiltering(filter == Selection.Custom);
                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
            }
        }

        public override void SetArgument(object argument)
        {
            if (argument is Selection)
                Filter = (Selection)argument;
            else
            {
                if (argument != null)
                {
                    string metricToSelect = argument.ToString();
                    if (!string.IsNullOrEmpty(metricToSelect))
                    {

                        foreach (UltraGridRow mrow in detailsGrid.Rows)
                        {
                            try 
                            {
                                if (mrow.Cells.Exists("Metric Type") && (mrow.Cells["Metric Type"].Value != null &&
                                     mrow.Cells["Metric Type"].Value.ToString().Equals(metricToSelect)))
                                {
                                    UltraGridCell cell = mrow.Cells[COL_VISIBLE];
                                    if (cell != null)
                                        cell.Value = true;

                                    mrow.Selected = true;
                                    detailsGrid.ActiveRow = mrow;
                                    detailsGrid.ActiveRowScrollRegion.ScrollPosition = mrow.Index;

                                    AddSelectedMetricToChart();
                                    break;
                                }
                            }
                            catch(Exception e)
                            {
                                Log.ErrorFormat("SetArgument threw an exception: {0}", e.Message);
                            }
                        }
                    }
                }
                base.SetArgument(argument);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
