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
using System.Collections.Generic;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.Drawing.Drawing2D;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesMemoryView : ServerBaseView
    {
        private const string CacheChartHeader = "{0} Cache";
        private readonly ServerSummaryHistoryData historyData;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Exception historyModeLoadError = null;
        private bool procedureCacheChartsChecked = false;

        private DataTable top5DataTable;
        private List<string> selectedDatabases = new List<string>();
        private HashSet<string> uniqueDatabases = new HashSet<string>();
        private Dictionary<string, string> dbValMap = new Dictionary<string, string>();

        public ResourcesMemoryView(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            RefreshReportsProgress = true;
            this.historyData = historyData;

            InitializeComponent();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                
                SetAmazonRDSResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                SetManagedAzureResourcesPanel();
            }
            //IF NOT AZURE, SET REGULAR
            else if(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                SetRegularRDSResourcesPanel();
            }
            ChartFxExtensions.SetContextMenu(pageLifeExpectancyChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cacheChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cacheHitRatesChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(memoryAreasChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(pagingChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(memoryUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(vmMemUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(hostMemAreaChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(vmMemAreaChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(VmAvaiableByteHyperVChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(HostAvaialbleByteHyperVChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(dbAvgMemUsageChart, toolbarsManager);

            InitializeDatabasesGrid();
            CreateDbAvgChartDataSource();
            InitializeCharts();
            setBackgroundColor();
            // Autoscale font size.
            AdaptFontSize();

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ShowVMData(instanceObject.Instance.IsVirtualized);
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
                ///baseline enabled event handler --Ankit Nagpal SQLdm 10.0
                case "EnableBaseline":
                    InitializeMemoryAreasChart();
                    InitializeMemoryUsageChart();
                    InitializePageLifeExpectancyChart();
                    InitializePagingChart();
                    InitializeCacheHitRatesChart();
                    break;
            }
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesMemoryView);
        }

        #region Initialize Charts

        public void InitializeCharts()
        {
            InitializeMemoryUsageChart();
            InitializeVMMemoryUsageChart();
            InitializePagingChart();
            InitializeMemoryAreasChart();
            InitializeHostMemoryAreasChart();
            InitializeVMMemoryAreasChart();
            InitializeCacheHitRatesChart();
            InitializeCacheChart();
            //InitializeProcedureCacheChart();
            InitializePageLifeExpectancyChart();
            InitializeVmAvaiableByteHyperVChart();
            InitializeHostAvaialbleByteHyperVChart();
            InitializeDbAvgMemUsageChart(); // SQLdm 11 DB Avg Mem Usage Chart
            ForceChartColors();
            InitalizeDrilldown(memoryUsageChart, vmMemUsageChart, pagingChart, memoryAreasChart,    //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            hostMemAreaChart, vmMemAreaChart, cacheHitRatesChart, cacheChart, pageLifeExpectancyChart, VmAvaiableByteHyperVChart, HostAvaialbleByteHyperVChart,dbAvgMemUsageChart);
        }

        private void ForceChartColors()
        {
            pageLifeExpectancyChart.SetAreaSeriesAlphaChannel(175, 0);
            cacheChart.SetAreaSeriesAlphaChannel(175, 0);
            pagingChart.SetAreaSeriesAlphaChannel(175, 0);
            hostMemAreaChart.SetAreaSeriesAlphaChannel(175, 0);
            vmMemAreaChart.SetAreaSeriesAlphaChannel(175, 0);
            memoryAreasChart.SetAreaSeriesAlphaChannel(175, 0);
            vmMemUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            memoryUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            VmAvaiableByteHyperVChart.SetAreaSeriesAlphaChannel(175, 0);
            HostAvaialbleByteHyperVChart.SetAreaSeriesAlphaChannel(175, 0);
        }

        private void InitializeDbAvgMemUsageChart()
        {
            dbAvgMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            dbAvgMemUsageChart.Printer.Compress = true;
            dbAvgMemUsageChart.Printer.ForceColors = true;
            dbAvgMemUsageChart.Printer.Document.DocumentName = "DB Average Memory Usage Chart";
            dbAvgMemUsageChart.ToolTipFormat = "%v %s\n%x";
            ConfigureDatabasesChart();
        }
        private void setBackgroundColor()
        {
            this.dbAvgMemUsagePanel .BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.dbAvgMemUsagePanel.Padding = new System.Windows.Forms.Padding(3, 0, 4, 1);
        }
        private void InitializeMemoryUsageChart()
        {
            memoryUsageChart.Tag = memUsageDropDownButton.Text.Trim() + "-SQLServer";
            memoryUsageChart.Printer.Orientation = PageOrientation.Landscape;
            memoryUsageChart.Printer.Compress = true;
            memoryUsageChart.Printer.ForceColors = true;
            memoryUsageChart.Printer.Document.DocumentName = "Memory Usage Chart";
            memoryUsageChart.ToolBar.RemoveAt(0);
            memoryUsageChart.ToolTipFormat = "%s\n%v MB\n%x";
            this.seriesAttributes18.Gallery = Gallery.Area;
            this.seriesAttributes19.Gallery = Gallery.Area;
            this.seriesAttributes20.Gallery = Gallery.Area;

            //START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            FieldMap sqlServerMemoryUsedFieldMap = new FieldMap("SqlServerMemoryUsed", FieldUsage.Value);
            sqlServerMemoryUsedFieldMap.DisplayName = "SQL Used";

            FieldMap sqlServerMemoryAllocatedFieldMap = new FieldMap("SqlServerMemoryAllocated", FieldUsage.Value);
            sqlServerMemoryAllocatedFieldMap.DisplayName = "SQL Allocated";

            FieldMap totalMemoryUsedFieldMap = new FieldMap("OsMemoryUsed", FieldUsage.Value);
            totalMemoryUsedFieldMap.DisplayName = "Total Used";

            FieldMap sqlServerMemoryUsageBaseline = new FieldMap("SQLServerMemoryUsageBaseline", FieldUsage.Value);
            sqlServerMemoryUsageBaseline.DisplayName = "Baseline-Usage";

            FieldMap sqlServerMemoryUsedBaseline = new FieldMap("SQLServerMemoryUsedBaseline", FieldUsage.Value);
            sqlServerMemoryUsedBaseline.DisplayName = "Baseline-Used";

            FieldMap sqlMemoryAllocatedBaseline = new FieldMap("SQLMemoryAllocatedBaseline", FieldUsage.Value);
            sqlMemoryAllocatedBaseline.DisplayName = "Baseline-Allocated";

            FieldMap hostMemoryUsageBaseline = new FieldMap("HostMemoryUsageBaseline", FieldUsage.Value);
            hostMemoryUsageBaseline.DisplayName = "Baseline-Host Usage";

            FieldMap vmMemoryUsageBaseline = new FieldMap("VmMemoryUsageBaseline", FieldUsage.Value);
            vmMemoryUsageBaseline.DisplayName = "Baseline-VM Usage";

            memoryUsageChart.DataSourceSettings.Fields.Clear();
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            if (Settings.Default.EnableBaseline)
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.AmazonRDSId &&
                    ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureManagedInstanceId)
                {
                    memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                        totalMemoryUsedFieldMap,
                                                                        sqlServerMemoryUsageBaseline,//Baseline1
                                                                        sqlMemoryAllocatedBaseline,//Baseline2
                                                                        sqlServerMemoryUsedBaseline,//Baseline3
                                                                        hostMemoryUsageBaseline,//Baseline4
                                                                        vmMemoryUsageBaseline//Baseline5
                                                                    });
                }
                else
                {
                    memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                        sqlServerMemoryUsageBaseline,//Baseline1
                                                                        sqlMemoryAllocatedBaseline,//Baseline2
                                                                        sqlServerMemoryUsedBaseline,//Baseline3
                                                                        hostMemoryUsageBaseline,//Baseline4
                                                                        vmMemoryUsageBaseline//Baseline5  
                                                                    });
                }
                this.baselinePlotForMemoryUsage_allocated.Gallery = Gallery.Lines;
                this.baselinePlotForMemoryUsage_usage.Gallery = Gallery.Lines;
                this.baselinePlotForMemoryUsage_used.Gallery = Gallery.Lines;
                this.baselinePlotForHostMemoryUsage.Gallery = Gallery.Lines;
                this.baselinePlotForVmMemoryUsage.Gallery = Gallery.Lines;
            }
            else
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.AmazonRDSId &&
                    ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureManagedInstanceId)
                {
                    memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                        totalMemoryUsedFieldMap
                                                                    });
                }
                else
                {
                    memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap
                                                                    });
                }
                this.baselinePlotForMemoryUsage_allocated.Gallery = Gallery.Area;
                this.baselinePlotForMemoryUsage_usage.Gallery = Gallery.Area;
                this.baselinePlotForMemoryUsage_used.Gallery = Gallery.Area;
                this.baselinePlotForHostMemoryUsage.Gallery = Gallery.Area;
                this.baselinePlotForVmMemoryUsage.Gallery = Gallery.Area;
            }
            //END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            memoryUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            memoryUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            memoryUsageChart.LegendBox.Width = 155;
            memoryUsageChart.LegendBox.PlotAreaOnly = false;
            if (historyData.HistoricalSnapshotDateTime == null)
                memoryUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else memoryUsageChart.DataSource = historyData.HistoricalSnapshotsDataTable;
            setMemUsageDropDownSelection(sqlMemUsageMenuItem);
        }

        private void InitializeVMMemoryUsageChart()
        {
            vmMemUsageChart.Tag = memUsageDropDownButton.Text.Trim() + "-VM";
            vmMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            vmMemUsageChart.Printer.Compress = true;
            vmMemUsageChart.Printer.ForceColors = true;
            vmMemUsageChart.Printer.Document.DocumentName = "Virtualization Memory Usage Chart";
            vmMemUsageChart.ToolBar.RemoveAt(0);
            vmMemUsageChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap vmMemUsagePct = new FieldMap("vmMemUsage", FieldUsage.Value);
            vmMemUsagePct.DisplayName = "VM Memory Usage";
            FieldMap hostMemUsagePct = new FieldMap("esxMemUsage", FieldUsage.Value);
            hostMemUsagePct.DisplayName = "Host Memory Usage";

            vmMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        vmMemUsagePct,
                                                                        hostMemUsagePct
                                                                    });


            vmMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            vmMemUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;

        }

        private void InitializePagingChart()
        {
            pagingChart.Tag = pagingHeaderStripLabel;
            pagingChart.Printer.Orientation = PageOrientation.Landscape;
            pagingChart.Printer.Compress = true;
            pagingChart.Printer.ForceColors = true;
            pagingChart.Printer.Document.DocumentName = "Paging Chart";
            pagingChart.ToolBar.RemoveAt(0);
            pagingChart.ToolTipFormat = "%s\n%v pages/second\n%x";
            pagingChart.AxisY.DataFormat.Decimals = 2;
            this.seriesAttributes15.Gallery = Gallery.Area;

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap pagesPerSecFieldMap = new FieldMap("PagesPerSec", FieldUsage.Value);
            pagesPerSecFieldMap.DisplayName = @"Pages";

            //START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            FieldMap osPagingBaseline = new FieldMap("OSPagingBaseline", FieldUsage.Value);
            osPagingBaseline.DisplayName = "Baseline-OS Paging";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            pagingChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
            {
                pagingChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   pagesPerSecFieldMap,
                                                                   osPagingBaseline
                                                               });
                this.baselinePlotForPaging.Gallery = Gallery.Lines;
            }
            else
            {
                pagingChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   pagesPerSecFieldMap
                                                               });
                this.baselinePlotForPaging.Gallery = Gallery.Area;
            }
            //END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines

            pagingChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pagingChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pagingChart.LegendBox.Width = 150;
            pagingChart.LegendBox.PlotAreaOnly = false;

            if (historyData.HistoricalSnapshotDateTime == null)
                pagingChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                pagingChart.DataSource = historyData.HistoricalSnapshotsDataTable;
            setMemUsageDropDownSelection(sqlMemUsageMenuItem);
        }

        private void InitializeMemoryAreasChart()
        {
            memoryAreasChart.Tag = memoryAreasHeaderStripLabel.Text.Trim() + "-SQLServer";
            memoryAreasChart.Printer.Orientation = PageOrientation.Landscape;
            memoryAreasChart.Printer.Compress = true;
            memoryAreasChart.Printer.ForceColors = true;
            memoryAreasChart.Printer.Document.DocumentName = "Memory Areas Chart";
            memoryAreasChart.ToolBar.RemoveAt(0);
            memoryAreasChart.ToolTipFormat = "%s\n%v MB\n%x";


            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            //FieldMap bufferCacheFieldMap = new FieldMap("CommittedPages", FieldUsage.Value);
            //bufferCacheFieldMap.DisplayName = "Buffer Cache";

            //START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            FieldMap procedureCacheSizeBaseline = new FieldMap("ProcedureCacheSizeBaseline", FieldUsage.Value);
            procedureCacheSizeBaseline.DisplayName = "Baseline-Procedure Cache Size";
            //END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            FieldMap procedureCacheFieldMap = new FieldMap("ProcedureCachePages", FieldUsage.Value);
            procedureCacheFieldMap.DisplayName = "Procedure Cache";
            FieldMap connectionsFieldMap = new FieldMap("ConnectionMemory", FieldUsage.Value);
            connectionsFieldMap.DisplayName = "Connections";
            FieldMap locksFieldMap = new FieldMap("LockMemory", FieldUsage.Value);
            locksFieldMap.DisplayName = "Locks";
            FieldMap databaseFieldMap = new FieldMap("UsedDataMemory", FieldUsage.Value);
            databaseFieldMap.DisplayName = "Database";
            FieldMap optimizerFieldMap = new FieldMap("OptimizerMemory", FieldUsage.Value);
            optimizerFieldMap.DisplayName = "Optimizer";
            FieldMap sortHashIndexFieldMap = new FieldMap("SortHashIndexMemory", FieldUsage.Value);
            sortHashIndexFieldMap.DisplayName = "Sort, Hash, Index";
            FieldMap freeFieldMap = new FieldMap("BufferCacheFreePages", FieldUsage.Value);
            freeFieldMap.DisplayName = "Free";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            memoryAreasChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
            {
                memoryAreasChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        procedureCacheSizeBaseline,//SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines                                                                      
                                                                        freeFieldMap,
                                                                        sortHashIndexFieldMap,
                                                                        optimizerFieldMap,
                                                                        databaseFieldMap,
                                                                        locksFieldMap,
                                                                        connectionsFieldMap,
                                                                        procedureCacheFieldMap//,
                                                                        //bufferCacheFieldMap
                                                                    });
                this.baselinePlotForMemoryAreas.Gallery = Gallery.Lines;
            }
            else
            {
                memoryAreasChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        freeFieldMap,
                                                                        sortHashIndexFieldMap,
                                                                        optimizerFieldMap,
                                                                        databaseFieldMap,
                                                                        locksFieldMap,
                                                                        connectionsFieldMap,
                                                                        procedureCacheFieldMap//,
                                                                        //bufferCacheFieldMap
                                                                    });
                this.baselinePlotForMemoryAreas.Gallery = Gallery.Area;
            }

            memoryAreasChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            memoryAreasChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            memoryAreasChart.LegendBox.Width = 150;
            memoryAreasChart.LegendBox.PlotAreaOnly = false;
            if (historyData.HistoricalSnapshotDateTime == null)
                memoryAreasChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else memoryAreasChart.DataSource = historyData.HistoricalSnapshotsDataTable;
            setMemDropDownSelection(sqlToolStripMenuItem);
        }

        private void InitializeVMMemoryAreasChart()
        {
            vmMemAreaChart.Tag = memoryAreasHeaderStripLabel.Text.Trim() + "-VM";
            vmMemAreaChart.Printer.Orientation = PageOrientation.Landscape;
            vmMemAreaChart.Printer.Compress = true;
            vmMemAreaChart.Printer.ForceColors = true;
            vmMemAreaChart.Printer.Document.DocumentName = memToolStripDropDownButton.Text.Trim() + " Memory Areas Chart";
            vmMemAreaChart.ToolBar.RemoveAt(0);
            vmMemAreaChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap vmMemActiveFieldMap = new FieldMap("vmMemActive", FieldUsage.Value);
            vmMemActiveFieldMap.DisplayName = "Active";
            FieldMap vmMemConsumedFieldMap = new FieldMap("vmMemConsumed", FieldUsage.Value);
            vmMemConsumedFieldMap.DisplayName = "Consumed";
            FieldMap vmMemGrantedFieldMap = new FieldMap("vmMemGranted", FieldUsage.Value);
            vmMemGrantedFieldMap.DisplayName = "Granted";
            FieldMap vmMemBalloonedFieldMap = new FieldMap("vmMemBallooned", FieldUsage.Value);
            vmMemBalloonedFieldMap.DisplayName = "Ballooned";
            FieldMap vmMemSwappedFieldMap = new FieldMap("vmMemSwapped", FieldUsage.Value);
            vmMemSwappedFieldMap.DisplayName = "Swapped";

            vmMemAreaChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmMemActiveFieldMap,
                                                                            vmMemConsumedFieldMap,
                                                                            vmMemGrantedFieldMap,
                                                                            vmMemBalloonedFieldMap,
                                                                            vmMemSwappedFieldMap
                                                                        });

            vmMemAreaChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmMemAreaChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            vmMemAreaChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeHostMemoryAreasChart()
        {
            hostMemAreaChart.Tag = memoryAreasHeaderStripLabel.Text.Trim() + "-Host";
            hostMemAreaChart.Printer.Orientation = PageOrientation.Landscape;
            hostMemAreaChart.Printer.Compress = true;
            hostMemAreaChart.Printer.ForceColors = true;
            hostMemAreaChart.Printer.Document.DocumentName = memToolStripDropDownButton.Text.Trim() + " Memory Areas Chart";
            hostMemAreaChart.ToolBar.RemoveAt(0);
            hostMemAreaChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap vmMemActiveFieldMap = new FieldMap("esxMemActive", FieldUsage.Value);
            vmMemActiveFieldMap.DisplayName = "Active";
            FieldMap vmMemConsumedFieldMap = new FieldMap("esxMemConsumed", FieldUsage.Value);
            vmMemConsumedFieldMap.DisplayName = "Consumed";
            FieldMap vmMemGrantedFieldMap = new FieldMap("esxMemGranted", FieldUsage.Value);
            vmMemGrantedFieldMap.DisplayName = "Granted";
            FieldMap vmMemBalloonedFieldMap = new FieldMap("esxMemBallooned", FieldUsage.Value);
            vmMemBalloonedFieldMap.DisplayName = "Ballooned";
            FieldMap vmMemSwappedFieldMap = new FieldMap("esxMemSwapped", FieldUsage.Value);
            vmMemSwappedFieldMap.DisplayName = "Swapped";

            hostMemAreaChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmMemActiveFieldMap,
                                                                            vmMemConsumedFieldMap,
                                                                            vmMemGrantedFieldMap,
                                                                            vmMemBalloonedFieldMap,
                                                                            vmMemSwappedFieldMap
                                                                        });

            hostMemAreaChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            hostMemAreaChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            hostMemAreaChart.DataSource = historyData.RealTimeSnapshotsDataTable;

        }

        private void InitializeCacheHitRatesChart()
        {
            cacheHitRatesChart.Tag = cacheHitRatesHeaderStripLabel;
            cacheHitRatesChart.Printer.Orientation = PageOrientation.Landscape;
            cacheHitRatesChart.Printer.Compress = true;
            cacheHitRatesChart.Printer.ForceColors = true;
            cacheHitRatesChart.Printer.Document.DocumentName = "Cache Hit Rates Chart";
            cacheHitRatesChart.ToolBar.RemoveAt(0);
            cacheHitRatesChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap bufferCacheFieldMap = new FieldMap("BufferCacheHitRatio", FieldUsage.Value);
            bufferCacheFieldMap.DisplayName = "Buffer Cache";
            FieldMap procedureCacheFieldMap = new FieldMap("ProcedureCacheHitRatio", FieldUsage.Value);
            procedureCacheFieldMap.DisplayName = "Procedure Cache";
            FieldMap procedureCacheHitRatioBaseline = new FieldMap("ProcedureCacheHitRatioBaseline", FieldUsage.Value);
            procedureCacheHitRatioBaseline.DisplayName = "Baseline-Procedure Cache";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            cacheHitRatesChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
                cacheHitRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                      {
                                                                          dateFieldMap,
                                                                          bufferCacheFieldMap,
                                                                          procedureCacheFieldMap,
                                                                          procedureCacheHitRatioBaseline
                                                                      });
            else
                cacheHitRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                      {
                                                                          dateFieldMap,
                                                                          bufferCacheFieldMap,
                                                                          procedureCacheFieldMap

                                                                      });
            cacheHitRatesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cacheHitRatesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            cacheHitRatesChart.AxisY.Min = 0;
            cacheHitRatesChart.AxisY.Max = 100;
            cacheHitRatesChart.AxisY.DataFormat.Decimals = 2;
            cacheHitRatesChart.LegendBox.Width = 150;
            cacheHitRatesChart.LegendBox.PlotAreaOnly = false;

            if (historyData.HistoricalSnapshotDateTime == null)
            {
                cacheHitRatesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            }
            else
            {
                cacheHitRatesChart.DataSource = historyData.HistoricalSnapshotsDataTable;
            }


        }

        private void InitializeCacheChart()
        {
            cacheChart.Tag = cacheToolStripDropDownButton;
            cacheChart.Printer.Orientation = PageOrientation.Landscape;
            cacheChart.Printer.Compress = true;
            cacheChart.Printer.ForceColors = true;
            cacheChart.Printer.Document.DocumentName = "Cache Chart";
            cacheChart.ToolBar.RemoveAt(0);

            ConfigureBufferCacheChart();
			cacheChart.LegendBox.Width = 150;
            cacheChart.LegendBox.PlotAreaOnly = false;
            cacheChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cacheChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            //Incorrect graphs in history browser (this was evidently introduced with the baseline change) Defect Fix
            //10.0 SQLdm (Srishti Purohit)
            if (historyData.HistoricalSnapshotDateTime == null)
                cacheChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else
                cacheChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }

        private void InitializePageLifeExpectancyChart()
        {
            this.pageLifeExpectancyHeaderStrip.Items.Clear();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                this.pageLifeExpectancyHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.maximizeProcedureCacheChartButton,
                this.restoreProcedureCacheChartButton,
                this.pageLifeExpectancyDropDownButton});
                pageLifeExpectancyChart.Tag = pageLifeExpectancyDropDownButton;//pageLifeExpectancyHeaderStripLabel;
            }
            else
            {
                this.pageLifeExpectancyHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.maximizeProcedureCacheChartButton,
                this.restoreProcedureCacheChartButton,
                this.pageLifeExpectancyHeaderStripLabel});
                pageLifeExpectancyChart.Tag = pageLifeExpectancyHeaderStripLabel;
            }
            pageLifeExpectancyChart.Printer.Orientation = PageOrientation.Landscape;
            pageLifeExpectancyChart.Printer.Compress = true;
            pageLifeExpectancyChart.Printer.ForceColors = true;
            pageLifeExpectancyChart.Printer.Document.DocumentName = "Page Life Expectancy Chart";
            pageLifeExpectancyChart.ToolBar.RemoveAt(0);
            ConfigurePageLifeExpectancyChart();
            //pageLifeExpectancyChart.ToolTipFormat = "%s\n%v seconds\n%x";
            //this.seriesAttributes1.Gallery = Gallery.Area;

            //FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            //FieldMap pageLifeExpectancyPages = new FieldMap("PageLifeExpectancy", FieldUsage.Value);
            //pageLifeExpectancyPages.DisplayName = "Page Life Expectancy";
            ////START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            //FieldMap baselinePlot = new FieldMap("PageLifeExpectancyBaseline", FieldUsage.Value);
            //baselinePlot.DisplayName = "Baseline";
            /////Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            //pageLifeExpectancyChart.DataSourceSettings.Fields.Clear();
            //if (Settings.Default.EnableBaseline)
            //{
            //    pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
            //                                                           {
            //                                                              dateFieldMap,
            //                                                              pageLifeExpectancyPages,
            //                                                              baselinePlot
            //                                                           });
            //    this.seriesAttributes2.Gallery = Gallery.Lines;
            //}
            //else
            //{
            //    pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
            //                                                           {
            //                                                              dateFieldMap,
            //                                                              pageLifeExpectancyPages

            //                                                           });
            //    this.seriesAttributes2.Gallery = Gallery.Area;
            //}
            ////END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            //pageLifeExpectancyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            //pageLifeExpectancyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pageLifeExpectancyChart.LegendBox.Width = 150;
            pageLifeExpectancyChart.LegendBox.PlotAreaOnly = false;
			if (historyData.HistoricalSnapshotDateTime == null)
                pageLifeExpectancyChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            else pageLifeExpectancyChart.DataSource = historyData.HistoricalSnapshotsDataTable;
        }

        //Hyper V
        private void InitializeVmAvaiableByteHyperVChart()
        {
            VmAvaiableByteHyperVChart.Printer.Orientation = PageOrientation.Landscape;
            VmAvaiableByteHyperVChart.Printer.Compress = true;
            VmAvaiableByteHyperVChart.Printer.ForceColors = true;
            VmAvaiableByteHyperVChart.Printer.Document.DocumentName = "Memory Chart";
            VmAvaiableByteHyperVChart.ToolTipFormat = "VM %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var availByteFieldMap = new FieldMap("AvailableByteVm", FieldUsage.Value);
            availByteFieldMap.DisplayName = @"AvailableByte";

            VmAvaiableByteHyperVChart.DataSourceSettings.Fields.Clear();
            VmAvaiableByteHyperVChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   availByteFieldMap
                                                               });

            VmAvaiableByteHyperVChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            VmAvaiableByteHyperVChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            foreach (SeriesAttributes series in VmAvaiableByteHyperVChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
                series.Gallery = Gallery.Lines;
            }
            VmAvaiableByteHyperVChart.LegendBox.PlotAreaOnly = false;
            VmAvaiableByteHyperVChart.AxisY.Min = 0D;


            VmAvaiableByteHyperVChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeHostAvaialbleByteHyperVChart()
        {
            HostAvaialbleByteHyperVChart.Printer.Orientation = PageOrientation.Landscape;
            HostAvaialbleByteHyperVChart.Printer.Compress = true;
            HostAvaialbleByteHyperVChart.Printer.ForceColors = true;
            HostAvaialbleByteHyperVChart.Printer.Document.DocumentName = "Memory Chart";
            HostAvaialbleByteHyperVChart.ToolTipFormat = "Host %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var availByteFieldMap = new FieldMap("AvailableByteHost", FieldUsage.Value);
            availByteFieldMap.DisplayName = @"AvailableByte";

            HostAvaialbleByteHyperVChart.DataSourceSettings.Fields.Clear();
            HostAvaialbleByteHyperVChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   availByteFieldMap
                                                               });

            HostAvaialbleByteHyperVChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            HostAvaialbleByteHyperVChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            foreach (SeriesAttributes series in HostAvaialbleByteHyperVChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
                series.Gallery = Gallery.Lines;
            }
            HostAvaialbleByteHyperVChart.LegendBox.PlotAreaOnly = false;
            HostAvaialbleByteHyperVChart.AxisY.Min = 0D;

            HostAvaialbleByteHyperVChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }


        private void ConfigureChartDataSource(DataTable dataSource)
        {
            if (memoryUsageChart.DataSource == dataSource) return;

            memoryUsageChart.DataSource =
                vmMemUsageChart.DataSource =
                pagingChart.DataSource =
                memoryAreasChart.DataSource =
                vmMemAreaChart.DataSource =
                hostMemAreaChart.DataSource =
                cacheHitRatesChart.DataSource =
                cacheChart.DataSource =
                pageLifeExpectancyChart.DataSource =
                VmAvaiableByteHyperVChart.DataSource =
                HostAvaialbleByteHyperVChart.DataSource = dataSource;

            // redo chart colors after assigning datasource
            ForceChartColors();
        }

        private void ShowVMData(bool showVM)
        {
            if (showVM == memToolStripDropDownButton.Visible) return;

            if (!showVM)
            {
                setMemDropDownSelection(sqlToolStripMenuItem);
                setMemUsageDropDownSelection(sqlMemUsageMenuItem);
            }

            memToolStripSeparator.Visible =
                memToolStripDropDownButton.Visible = showVM;
            memUsageDropDownButton.ShowDropDownArrow =
                vmMemUsageMenuItem.Visible = showVM;
        }

        private void setMemDropDownSelection(ToolStripMenuItem item)
        {
            memToolStripDropDownButton.Text = item.Text.Length > 60 ? string.Format("{0}...", item.Text.Substring(0, 60)) : item.Text;
            memToolStripDropDownButton.ToolTipText = item.Text;
            memToolStripDropDownButton.Tag = item.Text;

            //Hyper V
            MonitoredSqlServerStatus status = MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
            string serverType = "Unknown";
            Boolean hyperVisibility = false;

            if (status != null && status.Instance.Instance.IsVirtualized)
            {
                serverType = status.Instance.Instance.VirtualizationConfiguration.VCServerType;
                hyperVisibility = serverType.Equals("HyperV") ? true : false;
            }

            if (item.Text == vmToolStripMenuItem.Text)
            {
                memoryAreasChart.Visible =
                    hostMemAreaChart.Visible =
                    HostAvaialbleByteHyperVChart.Visible = false;
                vmMemAreaChart.Visible = !hyperVisibility;
                VmAvaiableByteHyperVChart.Visible = hyperVisibility;
            }
            else if (item.Text == hostToolStripMenuItem.Text)
            {
                memoryAreasChart.Visible =
                    vmMemAreaChart.Visible =
                    VmAvaiableByteHyperVChart.Visible = false;
                hostMemAreaChart.Visible = !hyperVisibility;
                HostAvaialbleByteHyperVChart.Visible = hyperVisibility;
            }
            else
            {
                vmMemAreaChart.Visible =
                    hostMemAreaChart.Visible =
                    VmAvaiableByteHyperVChart.Visible =
                    HostAvaialbleByteHyperVChart.Visible = false;
                if (isUserSysAdmin == true)
                    memoryAreasChart.Visible = true;
                else
                {
                    if(status.Instance.Instance.CloudProviderId==2)
                        memoryAreasChart.Visible = true;
                    else
                        memoryAreasChart.Visible = false;
                }
                   
            }
        }

        private void setMemUsageDropDownSelection(ToolStripMenuItem item)
        {
            memUsageDropDownButton.Text = item.Text.Length > 60 ? string.Format("{0}...", item.Text.Substring(0, 60)) : item.Text;
            memUsageDropDownButton.ToolTipText = item.Text;
            memUsageDropDownButton.Tag = item.Text;

            if (item.Text == vmMemUsageMenuItem.Text)
            {
                memoryUsageChart.Visible = false;
                vmMemUsageChart.Visible = true;
            }
            else
            {
                vmMemUsageChart.Visible = false;
                if (isUserSysAdmin == true)
                    memoryUsageChart.Visible = true;
                else
                    memoryUsageChart.Visible = false;
            }
        }

        private void memToolStripDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem)
            {
                ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
                if (memToolStripDropDownButton.Tag.ToString() != item.Text)
                {
                    setMemDropDownSelection(item);
                }
            }
        }

        private void memUsageDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem)
            {
                ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
                if (memUsageDropDownButton.Tag.ToString() != item.Text)
                {
                    setMemUsageDropDownSelection(item);
                }
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
                label1.Text = Idera.SQLdm.Common.Constants.LOADING;
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

                backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

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
                        ConfigureDataSourceForDbAvgMemUsage(historyData.RealTimeSnapshotsDataTable);
                        //ChartsVisible(true);
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

        private void UpdateProcedureCacheCharts(ServerSummarySnapshots summarySnapshots)
        {
            if ((!procedureCacheChartsChecked) && (summarySnapshots != null) && (summarySnapshots.ServerOverviewSnapshot != null) && (summarySnapshots.ServerOverviewSnapshot.Error == null))
            {
                if (summarySnapshots.ServerOverviewSnapshot.ProductVersion.Major < 9)
                {

                    pageLifeExpectancyChart.DataSourceSettings.Fields.Clear();

                    FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                    FieldMap activePagesFieldMap = new FieldMap("ProcedureCacheActivePages", FieldUsage.Value);
                    activePagesFieldMap.DisplayName = "Active Pages";
                    FieldMap freePagesFieldMap = new FieldMap("ProcedureCacheFreePages", FieldUsage.Value);
                    freePagesFieldMap.DisplayName = "Free Pages";

                    pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                           dateFieldMap,
                                                                           activePagesFieldMap,
                                                                           freePagesFieldMap
                                                                       });

                    pageLifeExpectancyChart.DataSourceSettings.ReloadData();

                    memoryAreasChart.DataSourceSettings.Fields.RemoveAt(7);

                    FieldMap procedureCacheFieldMap = new FieldMap("ProcedureCacheActivePages", FieldUsage.Value);
                    procedureCacheFieldMap.DisplayName = "Procedure Cache";

                    memoryAreasChart.DataSourceSettings.Fields.Add(procedureCacheFieldMap);

                    memoryAreasChart.DataSourceSettings.ReloadData();

                }

                procedureCacheChartsChecked = true;
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
                        ConfigureDataSourceForDbAvgMemUsage(historyData.RealTimeSnapshotsDataTable);
                        if (summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                        {
                            memoryUsageChart.AxisY.Min = 0;
                            memoryUsageChart.AxisY.Max =
                                Convert.ToDouble(summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes);
                        }
                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig != null)
                        {
                            if (summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                            {
                                VmAvaiableByteHyperVChart.AxisY.Min = 0;
                                VmAvaiableByteHyperVChart.AxisY.Max =
                                    Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.MemUsage));
                            }

                            if (summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                            {
                                HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                                HostAvaialbleByteHyperVChart.AxisY.Max =
                                    Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.MemUsage));
                            }
                        }

                        UpdateChartDataFilter();
                        ChartsVisible(true);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        ShowChartStatusMessage("No data available.", false);
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

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable);
                    ConfigureDataSourceForDbAvgMemUsage(historyData.HistoricalSnapshotsDataTable);
                    if (summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        memoryUsageChart.AxisY.Min = 0;
                        memoryUsageChart.AxisY.Max =
                            Convert.ToDouble(summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes);
                    }
                    if (summarySnapshots.ServerOverviewSnapshot.VMConfig != null)
                    {
                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                        {
                            VmAvaiableByteHyperVChart.AxisY.Min = 0;
                            VmAvaiableByteHyperVChart.AxisY.Max =
                                Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.MemUsage));
                        }

                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                        {
                            HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                            HostAvaialbleByteHyperVChart.AxisY.Max =
                                Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.MemUsage));
                        }
                    }

                    ChartsVisible(true);
                    ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                          Properties.Resources.StatusWarningSmall);
                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                }
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
            else if (chart.Tag is string)
                title = (string)chart.Tag;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            else if (chart.Tag is string)
                title = (string)chart.Tag;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            else if (chart.Tag is string)
                title = (string)chart.Tag;
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
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                SetAmazonRDSResourcesPanel();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                SetManagedAzureResourcesPanel();
            }
            //IF NOT AZURE, SET REGULAR
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                SetRegularRDSResourcesPanel();
            }
            setBackgroundColor();
        }

        public void SetRegularRDSResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            //mem usage, paging, memareas, cachehit,cache,pagelife
            this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.pagingPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.memoryAreasPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.cachePanel, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.pageLifeExpectancyPanel, 2, 1);

        }
        public void SetManagedAzureResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.RowStyles.Clear();
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            //mem usage, paging, memareas, cachehit,cache,pagelife
            this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.pagingPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.memoryAreasPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.cachePanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.pageLifeExpectancyPanel,1, 2);
            this.tableLayoutPanel.Controls.Add(this.dbAvgMemUsagePanel,0,3);

        }
        public void SetAmazonRDSResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 0);
            //this.tableLayoutPanel.Controls.Add(this.memoryAreasPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cachePanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.pageLifeExpectancyPanel, 1, 1);
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
        }

        private void maximizeDbAvgMemUsageChartButton_Click(object sender, EventArgs e)
        {

            MaximizeChart(dbAvgMemUsagePanel, maximizeDbAvgMemUsageChartButton, restoreDbAvgMemUsageChartButton);
        }

        private void restoreDbAvgMemUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(dbAvgMemUsagePanel, maximizeDbAvgMemUsageChartButton, restoreDbAvgMemUsageChartButton, 0, 3);
        }

        private void maximizeMemoryUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(memoryUsagePanel, maximizeMemoryUsageChartButton, restoreMemoryUsageChartButton);
        }

        private void restoreMemoryUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(memoryUsagePanel, maximizeMemoryUsageChartButton, restoreMemoryUsageChartButton, 0, 0);
        }

        private void maximizePagingChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(pagingPanel, maximizePagingChartButton, restorePagingChartButton);
        }

        private void restorePagingChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(pagingPanel, maximizePagingChartButton, restorePagingChartButton, 1, 0);
        }

        private void maximizeMemoryAreasChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(memoryAreasPanel, maximizeMemoryAreasChartButton, restoreMemoryAreasChartButton);
        }

        private void restoreMemoryAreasChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(memoryAreasPanel, maximizeMemoryAreasChartButton, restoreMemoryAreasChartButton, 0, 1);
        }

        private void maximizeCacheHitRatesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cacheHitRatesPanel, maximizeCacheHitRatesChartButton, restoreCacheHitRatesChartButton);
        }

        private void restoreCacheHitRatesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cacheHitRatesPanel, maximizeCacheHitRatesChartButton, restoreCacheHitRatesChartButton, 1, 1);
        }

        private void maximizeBufferCacheChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cachePanel, maximizeBufferCacheChartButton, restoreBufferCacheChartButton);
        }

        private void restoreBufferCacheChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cachePanel, maximizeBufferCacheChartButton, restoreBufferCacheChartButton, 0, 2);
        }

        private void maximizeProcedureCacheChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(pageLifeExpectancyPanel, maximizeProcedureCacheChartButton, restoreProcedureCacheChartButton);
        }

        private void restoreProcedureCacheChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(pageLifeExpectancyPanel, maximizeProcedureCacheChartButton, restoreProcedureCacheChartButton, 1, 2);
        }

        private void bufferCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureCacheChart("Buffer");
        }

        private void procedureCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureCacheChart("Procedure");
        }

        private void ConfigureCacheChart(string chartType)
        {
            cacheToolStripDropDownButton.Text = string.Format(CacheChartHeader, chartType);

            switch (chartType)
            {
                case "Buffer":
                    ConfigureBufferCacheChart();
                    break;
                case "Procedure":
                    ConfigureProcedureCacheChart();
                    break;
            }

            cacheChart.DataSourceSettings.ReloadData();
        }
        private void pageLifeExpectancyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurepageLifeExpectancyChart("PageLifeExpectancy");
        }

        private void SwapUsageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurepageLifeExpectancyChart("SwapUsage");
        }
        private void ConfigurepageLifeExpectancyChart(string chartType)
        {
            switch (chartType)
            {
                case "PageLifeExpectancy":
                    pageLifeExpectancyDropDownButton.Text = "Page Life Expectancy (sec)";
                    ConfigurePageLifeExpectancyChart();
                    break;
                case "SwapUsage":
                    pageLifeExpectancyDropDownButton.Text = "Swap Usage (MB)";
                    ConfigureSwapUsageChart();
                    break;
            }

            pageLifeExpectancyChart.DataSourceSettings.ReloadData();
        }

        private void ConfigureSwapUsageChart()
        {
            pageLifeExpectancyChart.ToolTipFormat = "%s\n%v MB\n%x";
            this.seriesAttributes1.Gallery = Gallery.Area;

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap SwapUsage = new FieldMap("SwapUsage", FieldUsage.Value);
            SwapUsage.DisplayName = "Swap Usage";
            //START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            //FieldMap baselinePlot = new FieldMap("PageLifeExpectancyBaseline", FieldUsage.Value);
          //  baselinePlot.DisplayName = "Baseline";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            pageLifeExpectancyChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
            {
                pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                          dateFieldMap,
                                                                          SwapUsage,
                                                                        //  baselinePlot
                                                                       });
                this.seriesAttributes2.Gallery = Gallery.Lines;
            }
            else
            {
                pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                          dateFieldMap,
                                                                          SwapUsage

                                                                       });
                this.seriesAttributes2.Gallery = Gallery.Area;
            }
            this.pageLifeExpectancyChart.AxisY.Title.Text = "MB";

            //END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            pageLifeExpectancyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pageLifeExpectancyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void ConfigurePageLifeExpectancyChart()
        {
            pageLifeExpectancyChart.ToolTipFormat = "%s\n%v seconds\n%x";
            this.seriesAttributes1.Gallery = Gallery.Area;

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap pageLifeExpectancyPages = new FieldMap("PageLifeExpectancy", FieldUsage.Value);
            pageLifeExpectancyPages.DisplayName = "Page Life Expectancy";
            //START: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            FieldMap baselinePlot = new FieldMap("PageLifeExpectancyBaseline", FieldUsage.Value);
            baselinePlot.DisplayName = "Baseline";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
            pageLifeExpectancyChart.DataSourceSettings.Fields.Clear();
            if (Settings.Default.EnableBaseline)
            {
                pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                          dateFieldMap,
                                                                          pageLifeExpectancyPages,
                                                                          baselinePlot
                                                                       });
                this.seriesAttributes2.Gallery = Gallery.Lines;
            }
            else
            {
                pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                          dateFieldMap,
                                                                          pageLifeExpectancyPages

                                                                       });
                this.seriesAttributes2.Gallery = Gallery.Area;
            }
            this.pageLifeExpectancyChart.AxisY.Title.Text = "seconds";

            //END: SQLdm 10.0 (Tarun Sapra) - Added FieldMaps for the baselines
            pageLifeExpectancyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pageLifeExpectancyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }
        private void ConfigureBufferCacheChart()
        {
            cacheChart.DataSourceSettings.Fields.Clear();
            cacheChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap activePagesFieldMap = new FieldMap("BufferCacheActivePages", FieldUsage.Value);
            activePagesFieldMap.DisplayName = "Active Pages";
            FieldMap freePagesFieldMap = new FieldMap("BufferCacheFreePages", FieldUsage.Value);
            freePagesFieldMap.DisplayName = "Free Pages";

            cacheChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        activePagesFieldMap,
                                                                        freePagesFieldMap
                                                                    });
        }

        private void ConfigureProcedureCacheChart()
        {
            cacheChart.DataSourceSettings.Fields.Clear();
            cacheChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap procedureCachePages = new FieldMap("ProcedureCachePages", FieldUsage.Value);
            procedureCachePages.DisplayName = "Procedure Cache";

            cacheChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                           dateFieldMap,
                                                                           procedureCachePages
                                                                       });
        }

        /*       private void ConfigureProcedureCacheChart()
               {
                   pageLifeExpectancyChart.DataSourceSettings.Fields.Clear();
                   pageLifeExpectancyChart.ToolTipFormat = "%s\n%v MB\n%x";

                   FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                   FieldMap procedureCachePages = new FieldMap("ProcedureCachePages", FieldUsage.Value);
                   procedureCachePages.DisplayName = "Procedure Cache";

                   pageLifeExpectancyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                              {
                                                                                  dateFieldMap,
                                                                                  procedureCachePages
                                                                              });

                   pageLifeExpectancyChart.DataSourceSettings.ReloadData();
               }
       */



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
            label1.Text = Idera.SQLdm.Common.Constants.LOADING;
            ApplicationController.Default.SetActiveViewToRealTimeMode();

        }

        private void ChartsVisible(bool value)
        {
            memoryUsageChart.Visible =
                pagingChart.Visible =
                // memoryAreasChart.Visible =
                cacheHitRatesChart.Visible =
                cacheChart.Visible =
                pageLifeExpectancyChart.Visible = value;
        }

        private void ShowChartStatusMessage(string message, bool forceHide)
        {
            if (forceHide)
            {
                ChartsVisible(false);
            }

            memoryUsageChartStatusLabel.Text =
                label1.Text =
                pagingChartStatusLabel.Text =
                memoryAreasChartStatusLabel.Text =
                cacheHitRatesChartStatusLabel.Text =
                cacheChartStatusLabel.Text =
                pageLifeExpectancyChartStatusLabel.Text = message;
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
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void InitializeDatabasesGrid()
        {
            topDatabasesGrid.DrawFilter = new PercentageBackgroundDrawFilter();
        }
        private void CreateDbAvgChartDataSource()
        {

            top5DataTable = new DataTable();
            top5DataTable.Columns.Add("Date", typeof(DateTime));

            top5DataTable.Columns.Add("DbAvgMemUsage1", typeof(decimal));
            top5DataTable.Columns.Add("DbAvgMemUsage2", typeof(decimal));
            top5DataTable.Columns.Add("DbAvgMemUsage3", typeof(decimal));
            top5DataTable.Columns.Add("DbAvgMemUsage4", typeof(decimal));
            top5DataTable.Columns.Add("DbAvgMemUsage5", typeof(decimal));

            top5DataTable.PrimaryKey = new DataColumn[] { top5DataTable.Columns["Date"] };

            top5DataTable.DefaultView.Sort = "Date";
        }
        private void ConfigureDatabasesGrid(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                if (selectedDatabases.Count == 0)
                    return;
                topDatabasesGrid.SuspendLayout();

                if (dataSource==null || dataSource.DefaultView == null)
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

                DataTable dataTable = new DataTable("AzureCloudMetricsDbAvgMemUsage");

                DataColumn databaseColumn = new DataColumn();
                databaseColumn.DataType = System.Type.GetType("System.String");
                databaseColumn.ColumnName = "Database";
                databaseColumn.Caption = "Database";
                databaseColumn.ReadOnly = true;
                databaseColumn.Unique = true;
                dataTable.Columns.Add(databaseColumn);

                DataColumn metricValueColumn = new DataColumn();
                metricValueColumn.DataType = System.Type.GetType("System.Decimal");
                metricValueColumn.ColumnName = "DbAvgMemUsagePct";
                metricValueColumn.Caption = "DbAvgMemUsagePct";
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
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("DatabaseAverageMemoryUsagePercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["DatabaseAverageMemoryUsagePercent"] != DBNull.Value)
                                        {
                                            dr["DbAvgMemUsagePct"] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["DatabaseAverageMemoryUsagePercent"]);
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
                                if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("DatabaseAverageMemoryUsagePercent"))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["DatabaseAverageMemoryUsagePercent"] != DBNull.Value)
                                    {
                                        theDataRow[0] = dbName;
                                        theDataRow[1] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["DatabaseAverageMemoryUsagePercent"]);
                                        dataTable.Rows.Add(theDataRow);

                                        dataTable.AcceptChanges();
                                    }
                                }
                            }


                        }
                    }
                }
                DataSet gridDataSet = new DataSet();
                // gridDataSet.Tables.Add(dataTable);
                DataView dv = dataTable.DefaultView;
                dv.Sort = "DbAvgMemUsagePct desc";
                gridDataSet.Tables.Add(dv.ToTable());
                topDatabasesGrid.DataSource = gridDataSet;
                                
                foreach (UltraGridColumn col in topDatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = false;
                }

                topDatabasesGrid.ResumeLayout();
            }
        }


        internal void ConfigureDataSourceForDbAvgMemUsage(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
              
                try
                {
                    top5DataTable.BeginLoadData();
                    top5DataTable.Rows.Clear();

                    // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
                    // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                    DataTable dataTableMetrics = dataSource; // historyData != null ? historyData.RealTimeSnapshotsDataTable : null;
                  //  bool shouldProceed = historicalDateTime == null ? (historyDataTableMetrics==null?false:true) : true;
                    if (dataTableMetrics!=null)
                    {
                        DataView currentData = new DataView(dataTableMetrics);

                        DateTime top5RowDate = DateTime.MinValue;
                        DataRow top5Row = null;

                        foreach (DataRowView row in currentData)
                        {
                            row.Row.Table.Columns.Contains("AzureCloudMetrics");
                            if (row["AzureCloudMetrics"] != DBNull.Value && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Count > 0)
                            {
                                DateTime date = (DateTime)row["Date"];
                                if (top5Row == null || date != top5RowDate)
                                {
                                    top5Row = top5DataTable.NewRow();
                                    top5Row["Date"] = date;
                                    if (top5DataTable.Rows.Contains(date))                                    {                                        top5DataTable.Rows.Remove(top5DataTable.Rows.Find(date));                                    }                                    top5DataTable.Rows.Add(top5Row);
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
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(selectedDatabases[i]) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]].ContainsKey("DatabaseAverageMemoryUsagePercent"))
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["DatabaseAverageMemoryUsagePercent"] != DBNull.Value)
                                        {
                                            top5Row["DbAvgMemUsage" + (i + 1)] = ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["DatabaseAverageMemoryUsagePercent"];
                                            dbValMap["DbAvgMemUsage" + (i + 1)] = selectedDatabases[i];
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
                dbAvgMemUsageChart.DataSource = top5DataTable;
                ChartFxExtensions.SetAxisXTimeScale(dbAvgMemUsageChart, 2);
                ConfigureDatabasesChart();

                Invalidate();
            }
        }
        private void dbAvgMemUsageChart_Resize(object sender, EventArgs e)
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
        private void ConfigureDatabasesChart()
        {
            if (selectedDatabases.Count == 0)
            {
                dbAvgMemUsageChart.Visible = false;
                label1.Text = Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE;
                return;
            }
            else
            {
                dbAvgMemUsageChart.Visible = true;
            }
            dbAvgMemUsageChart.SuspendLayout();
            dbAvgMemUsageChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            dbAvgMemUsageChart.DataSourceSettings.Fields.Add(dateFieldMap);
            selectedDatabases.Sort();

            if (selectedDatabases.Count > 0)
            {
                for (int idx = 1; idx <=( selectedDatabases.Count <= 5 ? selectedDatabases.Count : 5); idx++)
                {
                    if (dbValMap.ContainsKey("DbAvgMemUsage" + idx))
                    {
                        string database = selectedDatabases[idx - 1];
                        FieldMap fieldMap = new FieldMap("DbAvgMemUsage" + idx, FieldUsage.Value);
                        fieldMap.DisplayName = dbValMap["DbAvgMemUsage" + idx];
                        dbAvgMemUsageChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                dbAvgMemUsageChart.DataSourceSettings.ReloadData();
                dbAvgMemUsageChart_Resize(dbAvgMemUsageChart, new EventArgs());
                int showDecimals = 4;
                dbAvgMemUsageChart.AxisY.DataFormat.Decimals = showDecimals;            
                dbAvgMemUsageChart.Invalidate();
            }
            dbAvgMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            dbAvgMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            dbAvgMemUsageChart.ResumeLayout();
        }
                
    }
    internal sealed class PercentageBackgroundDrawFilter : IUIElementDrawFilter
    {
        private readonly IUIElementDrawFilter previousFilter;

        internal PercentageBackgroundDrawFilter()
        {
            this.previousFilter = null;
        }

        internal PercentageBackgroundDrawFilter(IUIElementDrawFilter previousFilter)
        {
            this.previousFilter = previousFilter;
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            if (drawPhase == DrawPhase.BeforeDrawFocus ||
                drawPhase == DrawPhase.BeforeDrawForeground)
            {
                return true;
            }

            if (drawPhase == DrawPhase.BeforeDrawBackColor)
            {
                if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
                {
                    CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                    if (cell.Column.Key == "ValueBar")
                    {
                        Rectangle rect = (Rectangle)drawParams.Element.RectInsideBorders;
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            drawParams.Graphics.FillRectangle(brush, rect);
                        }

                        rect.Width = Convert.ToInt32(rect.Width * Convert.ToDouble(cell.Cell.Value));
                        if (rect.Width > 0)
                        {
                            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, cell.Column.CellAppearance.BackColor, cell.Column.CellAppearance.BackColor2, LinearGradientMode.Vertical))
                            {
                                drawParams.Graphics.FillRectangle(brush, rect);
                            }
                        }
                    }
                }
                return true;
            }

            if (previousFilter != null)
                return previousFilter.DrawElement(drawPhase, ref drawParams);

            return false;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            DrawPhase result = DrawPhase.BeforeDrawFocus;

            if (drawParams.Element is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextUIElement && drawParams.Element.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (previousFilter != null)
                result |= previousFilter.GetPhasesToFilter(ref drawParams);

            return result;
        }
    }
}