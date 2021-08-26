//------------------------------------------------------------------------------
// <copyright file="ChartHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using ChartFX.WinForms;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;
using QueryMonitorViewMode = Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries.QueryMonitorView.QueryMonitorViewMode;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    #region enums and constants for chart processing

    public enum ChartDataColumnType
    {
        Disk,
        Other
    }

    public enum ChartDataType
    {
        DataSource,
        Manual
    }

    public enum ChartFormat
    {
        SingleChart,
        SingleChartWithCount,
        MultipleCharts,
        MultipleChartsWithCount,
        MultiViewCharts,
        MultiViewChartsWithCount
    }

    // These values cannot be changed once they are released because they will be stored in preferences
    // Although the numbers don't really matter,
    // my plan to keep them grouped is to start each new type of charts every 100
    public enum ChartType
    {
        [Description("Select a chart...")]
        Unknown = 0,
        // Resource Disk Charts
        [Description("Disk Busy Per Disk")]
        DiskBusyPerDisk = 1,
        [Description("Disk Busy Total")]
        DiskBusyTotal,
        [Description("SQL Server Physical I/O Errors")]
        DiskSqlIOErrors,
        [Description("SQL Server Physical I/O")]
        DiskSqlIO,
        [Description("Average Disk Queue Length Per Disk")]
        DiskQueuePerDisk,
        [Description("Average Disk Queue Length Total")]
        DiskQueueTotal,
        [Description("Disk Reads/Second Per Disk")]
        DiskReadsPerSecPerDisk,
        [Description("Average Disk ms/Read Per Disk")]
        DiskTimePerReadPerDisk,
        [Description("Average Disk ms/Transfer Per Disk")]
        DiskTimePerTransferPerDisk,
        [Description("Average Disk ms/Write Per Disk")]
        DiskTimePerWritePerDisk,
        [Description("Disk Transfers/Second Per Disk")]
        DiskTransfersPerSecPerDisk,
        [Description("Disk Writes/Second Per Disk")]
        DiskWritesPerSecPerDisk,
        [Description("Virtual Machine Disk Usage")]
        DiskVMTransferRate,
        [Description("VM Host Disk Usage")]
        DiskHostTransferRate,

        // REMINDER: DO NOT CHANGE THESE VALUES
        // Query Charts
        [Description("Average Duration")]
        QueryDuration = 101,
        [Description("Average CPU")]
        QueryCPU,
        [Description("Average Reads")]
        QueryReads,
        [Description("Average Writes")]
        QueryWrites,
        [Description("Average Waits")]
        QueryWaits,
        [Description("Deadlocks")]
        QueryDeadlocks,
        [Description("Blocking")]
        QueryBlocking,
        [Description("CPU Per Second")]
        QueryCPUPerSecond,
        [Description("I/O Per Second")]
        QueryIOPerSecond,
        // Query History Charts
        [Description("Average Duration")]
        QueryHistoryDuration = 151,
        [Description("Average CPU")]
        QueryHistoryCPU,
        [Description("Average Reads")]
        QueryHistoryReads,
        [Description("Average Writes")]
        QueryHistoryWrites,
        [Description("Average Waits")]
        QueryHistoryWaits,
        [Description("Deadlocks")]
        QueryHistoryDeadlocks,
        [Description("Blocking")]
        QueryHistoryBlocking,
        [Description("CPU Per Second")]
        QueryHistoryCPUPerSecond,
        [Description("I/O Per Second")]
        QueryHistoryIOPerSecond
    }

    // These values cannot be changed once they are released because they will be stored in preferences
    public enum ChartView
    {
        [Description("Select a view...")]
        Unknown = 0,
        // used for charts that don't have multiple views
        [Description("Standard")]
        Standard = 1,

        // matching these to the same range as the ChartTypes
        [Description("by SQL Text")]
        QuerySQL = 101,
        [Description("by Application")]
        QueryApplication,
        [Description("by Database")]
        QueryDatabase,
        [Description("by User")]
        QueryUser,
        [Description("by Client")]
        QueryHost
    }

    #endregion

    public class ChartInfo
    {
        private const string TITLE_FORMAT_SINGLEVIEW = "{0}";
        private const string TITLE_FORMAT_MULTIVIEW = "{0} {1}";
        private const string TITLE_FORMAT_TOP_VALUES_SINGLEVIEW = "{0}: Top {1}";
        private const string TITLE_FORMAT_TOP_VALUES_MULTIVIEW = "{0}: Top {1} {2}";

        private string titleFormat = string.Empty;

        public ChartDataType ChartDataType = ChartDataType.DataSource;
        public ChartType ChartType = ChartType.Unknown;
        public ChartView ChartView = ChartView.Unknown;
        public int ChartDisplayValues = 0;
        public int ChartDisplayValuesMin = 1;
        public int ChartDisplayValuesMax = 20;
        public bool FilterColumns = false;
        //SQLdm 10.0 (Srishti Purohit)
        //SQLDM-25460 -- [10.0.2 Regression] : Baseline is not coming for few metric graphs
        public bool IsHyperVInstance = false;
        public bool InitChartDataColumns = false;

        public object DataObject = null;
        public object DataSource = null;
        public object DataSource2 = null; // SqlDM 10.2 (Anshul Aggarwal) - Allow chartpanel to support multiple datasources depending upon charttype


        #region Disk related items

        public bool ByDisk = false;
        public Dictionary<ChartType, Dictionary<string, bool>> DiskDataColumns = null;
        public bool InitDiskDataColumns = false;
        public OrderedDictionary<string, DiskDrive> OrderedDiskDrives = null;

        #endregion

        /// <summary>
        /// Gets the formatted chart title using the TitleFormat
        /// </summary>
        public string ChartTitle
        {
            get
            {
                return string.Format(TitleFormat, ChartHelper.ChartTypeName(ChartType), ChartHelper.ChartViewName(ChartView), ChartDisplayValues); ;
            }
        }

        /// <summary>
        /// Gets or Sets the format used to display the chart selections as the title on a report or message, etc.
        /// It can have up to 3 substitution values 1=ChartType, 2=ChartView, 3=ChartDisplayValues.
        /// To reset it to the default format set it to string.Empty
        /// </summary>
        public string TitleFormat
        {
            get
            {
                string format;

                if (titleFormat != string.Empty)
                {
                    format = titleFormat;
                }
                else if (ChartView == ChartView.Standard)
                {
                    format = ChartDisplayValues > 0 ? TITLE_FORMAT_TOP_VALUES_SINGLEVIEW : TITLE_FORMAT_SINGLEVIEW;
                }
                else
                {
                    format = ChartDisplayValues > 0 ? TITLE_FORMAT_TOP_VALUES_MULTIVIEW : TITLE_FORMAT_MULTIVIEW;
                }

                return format;
            }
            set
            {
                titleFormat = value;
            }
        }

        #region Constructors

        public ChartInfo()
        {
            ChartHelper.InitDiskDataColumns(this);
        }

        public ChartInfo(ChartType chartType)
        {
            ChartType = chartType;
            ChartView = Helpers.ChartView.Standard;

            ChartHelper.InitDiskDataColumns(this);
        }

        public ChartInfo(ChartType chartType, ChartView chartView)
        {
            ChartType = chartType;
            ChartView = chartView;

            ChartHelper.InitDiskDataColumns(this);
        }

        #endregion
    }

    public class ChartHelper
    {
        private static Logger LOG = Logger.GetLogger("ChartHelper");
        private const double BarChartLabelStart = 0.5; // SqlDM 10.2 (Anshul Aggarwal) - Gives us the first label position in chartfx bar chart

        public const AxisFormat TimeChartAxisFormat = AxisFormat.DateTime;
        
        /// <summary>
        /// Gets the time chart custom format excluding the Year component.
        /// It works with the local date time format exluding the Year component. Eg.: 10-jul 10:00 AM
        /// </summary>
        public static string TimeChartCustomFormat
        {
            get
            {
                return GetTimeChartCustomFormat(includeYear: false);
            }
        }

        /// <summary>
        /// Gets the time chart custom format including the Year component.
        /// It works with the local date time format exluding the Year component. Eg.: 10-jul 10:00 AM
        /// </summary>
        public static string TimeChartCustomFormatWithYear
        {
            get
            {
                return GetTimeChartCustomFormat(includeYear: true);
            }
        }

        /// <summary>
        /// Gets the time chart custom format, selectivelyit can include/exclude the year for the output.
        /// </summary>
        /// <param name="includeYear">A flag that indicates whether the result should include the  year or do not. when it is true the year is included, otherwise the year is removed.</param>
        /// <returns>the time chart custom format, selectivelyit can include/exclude the year for the output.</returns>
        private static string GetTimeChartCustomFormat(bool includeYear)
        {
            // The current culture to be used. 
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;

            // Get the current short date pattern.
            var uiShortDatePattern = currentCulture.DateTimeFormat.ShortDatePattern;

            if (!includeYear)
            {
                // Exclude the Year component
                var yearComponent = "y";
                uiShortDatePattern = uiShortDatePattern.Replace(yearComponent, string.Empty).Replace(yearComponent.ToUpper(), string.Empty);

                // Remove the separator next to the removed year
                var dateSeparator = currentCulture.DateTimeFormat.DateSeparator;
                var dateComponents = uiShortDatePattern.Split(new string[] { dateSeparator }, StringSplitOptions.RemoveEmptyEntries);
                uiShortDatePattern = string.Join(dateSeparator, dateComponents);
            }

            // Compose the result adding the date format and return
            return string.Format("{0} {1}", uiShortDatePattern, currentCulture.DateTimeFormat.ShortTimePattern);
        }

        public static string ChartTypeName(ChartType chartType)
        {
            return ApplicationHelper.GetEnumDescription(chartType);
        }
        public static string ChartViewName(ChartView chartView)
        {
            return ApplicationHelper.GetEnumDescription(chartView);
        }

        public static List<ChartType> GetDiskChartTypes()
        {
            return GetDiskChartTypes(false);
        }

        public static List<ChartType> GetDiskChartTypes(bool showVM)
        {
            List<ChartType> selections = new List<ChartType>();
            selections.Add(ChartType.DiskBusyTotal);
            selections.Add(ChartType.DiskBusyPerDisk);
            selections.Add(ChartType.DiskQueueTotal);
            selections.Add(ChartType.DiskQueuePerDisk);
            selections.Add(ChartType.DiskSqlIO);
            selections.Add(ChartType.DiskSqlIOErrors);
            selections.Add(ChartType.DiskReadsPerSecPerDisk);
            selections.Add(ChartType.DiskTransfersPerSecPerDisk);
            selections.Add(ChartType.DiskWritesPerSecPerDisk);
            selections.Add(ChartType.DiskTimePerReadPerDisk);
            selections.Add(ChartType.DiskTimePerTransferPerDisk);
            selections.Add(ChartType.DiskTimePerWritePerDisk);
            if (showVM)
            {
                selections.Add(ChartType.DiskVMTransferRate);
                selections.Add(ChartType.DiskHostTransferRate);
            }

            return selections;
        }

        internal static List<ChartType> GetQueryChartTypes(QueryMonitorViewMode viewMode)
        {
            List<ChartType> selections = new List<ChartType>();
            if (viewMode == QueryMonitorViewMode.History)
            {
                selections.Add(ChartType.QueryHistoryDuration);
                selections.Add(ChartType.QueryHistoryCPU);
                selections.Add(ChartType.QueryHistoryReads);
                selections.Add(ChartType.QueryHistoryWrites);
                selections.Add(ChartType.QueryHistoryWaits);
                selections.Add(ChartType.QueryHistoryDeadlocks);
                selections.Add(ChartType.QueryHistoryBlocking);
                selections.Add(ChartType.QueryHistoryCPUPerSecond);
                selections.Add(ChartType.QueryHistoryIOPerSecond);
            }
            else
            {
                selections.Add(ChartType.QueryDuration);
                selections.Add(ChartType.QueryCPU);
                selections.Add(ChartType.QueryReads);
                selections.Add(ChartType.QueryWrites);
                selections.Add(ChartType.QueryWaits);
                selections.Add(ChartType.QueryDeadlocks);
                selections.Add(ChartType.QueryBlocking);
                selections.Add(ChartType.QueryCPUPerSecond);
                selections.Add(ChartType.QueryIOPerSecond);
            }

            return selections;
        }

        internal static List<ChartView> GetQueryChartViews(QueryMonitorViewMode viewMode)
        {
            List<ChartView> selections = new List<ChartView>();

            if (viewMode == QueryMonitorViewMode.History)
            {
                selections.Add(ChartView.Standard);
            }
            else
            {
                selections.Add(ChartView.QuerySQL);
                selections.Add(ChartView.QueryApplication);
                selections.Add(ChartView.QueryDatabase);
                if (viewMode == QueryMonitorViewMode.Statement)
                {
                    selections.Add(ChartView.QueryUser);
                    selections.Add(ChartView.QueryHost);
                }
            }

            return selections;
        }

        #region Initialize

        //SQLdm 10.0 (Srishti Purohit)
        //Not getting called from any where
        //public static ChartInfo ReinitializeChartLayout(Chart chart, ChartInfo chartInfo, ChartType newChartType)
        //{
        //    return ReinitializeChartLayout(chart, chartInfo, newChartType, chartInfo.ChartView);
        //}

        public static ChartInfo ReinitializeChartLayout(Chart chart, ChartInfo chartInfo, ChartView newChartView)
        {
            return ReinitializeChartLayout(chart, chartInfo, chartInfo.ChartType, newChartView);
        }

        public static ChartInfo ReinitializeChartLayout(Chart chart, ChartInfo chartInfo, ChartType newChartType, ChartView newChartView)
        {
            ChartInfo newChartInfo = new ChartInfo(newChartType, newChartView);

            newChartInfo.ChartDisplayValues = chartInfo.ChartDisplayValues;
            newChartInfo.DataSource = chartInfo.DataSource;     // Preserve the DataSource if we are reconfiguring an existing chart
            newChartInfo.DataSource2 = chartInfo.DataSource2;     // Preserve the DataSource if we are reconfiguring an existing chart
            newChartInfo.DataObject = chartInfo.DataObject;
            newChartInfo.DiskDataColumns = chartInfo.DiskDataColumns;

            InitializeChartLayout(chart, newChartInfo, chartInfo.IsHyperVInstance);

            return newChartInfo;
        }

        public static ChartInfo InitializeChartLayout(Chart chart, ChartType chartType, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        {
            ChartInfo newChartInfo = new ChartInfo(chartType);

            newChartInfo.DataSource = chart.DataSource;   // Preserve the DataSource if we are reconfiguring an existing chart

            //SQLdm 10.0 (Srishti Purohit)
            //SQLDM-25460 -- [10.0.2 Regression] : Baseline is not coming for few metric graphs
            newChartInfo.IsHyperVInstance = isHyperVInstance;

            InitializeChartLayout(chart, newChartInfo, isHyperVInstance);//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type

            return newChartInfo;
        }

        public static ChartInfo InitializeChartLayout(Chart chart, ChartType chartType, int showItems, int minItems, int maxItems)
        {
            ChartInfo newChartInfo = new ChartInfo(chartType);

            newChartInfo.ChartDisplayValues = showItems;
            newChartInfo.ChartDisplayValuesMin = minItems;
            newChartInfo.ChartDisplayValuesMax = maxItems;
            newChartInfo.DataSource = chart.DataSource;   // Preserve the DataSource if we are reconfiguring an existing chart

            InitializeChartLayout(chart, newChartInfo);

            return newChartInfo;
        }

        public static ChartInfo InitializeChartLayout(Chart chart, ChartType chartType, ChartView chartView)
        {
            ChartInfo newChartInfo = new ChartInfo(chartType, chartView);

            newChartInfo.DataSource = chart.DataSource;   // Preserve the DataSource if we are reconfiguring an existing chart

            InitializeChartLayout(chart, newChartInfo);

            return newChartInfo;
        }

        public static ChartInfo InitializeChartLayout(Chart chart, ChartType chartType, ChartView chartView, int showItems, int minItems, int maxItems)
        {
            ChartInfo newChartInfo = new ChartInfo(chartType, chartView);

            newChartInfo.ChartDisplayValues = showItems;
            newChartInfo.ChartDisplayValuesMin = minItems;
            newChartInfo.ChartDisplayValuesMax = maxItems;
            newChartInfo.DataSource = chart.DataSource;   // Preserve the DataSource if we are reconfiguring an existing chart

            InitializeChartLayout(chart, newChartInfo);

            return newChartInfo;
        }

        private static void InitializeChartLayout(Chart chart, ChartInfo chartInfo, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        {
            chart.Reset();   // resets the chart to it's original state

            // chart.Reset() overrides the black style background, so we need to re set it to 222, 222, 222
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground = new ChartFX.WinForms.Adornments.GradientBackground();
            gradientBackground.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            chart.Background = gradientBackground;

            // basic layout settings for all charts
            chart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            chart.ContextMenus = false;

            chart.Printer.Orientation = PageOrientation.Landscape;
            chart.Printer.Compress = true;
            chart.Printer.ForceColors = true;
            chart.Printer.Document.DocumentName = "SQLDM Chart";
            chart.ToolBar.RemoveAt(0);
            chart.Palette = "Schemes.Classic";
            chart.PlotAreaColor = Color.White;
            //SQLdm 10.0 (Srishti Purohit)
            //SQLDM-25460 -- [10.0.2 Regression] : Baseline is not coming for few metric graphs
            chartInfo.IsHyperVInstance = isHyperVInstance;

            switch (chartInfo.ChartType)
            {
                case ChartType.DiskBusyPerDisk:
                    InitializeDiskBusyLayout(chart);
                    chartInfo.ByDisk = true;
                    chartInfo.FilterColumns = true;
                    break;
                case ChartType.DiskBusyTotal:
                    InitializeDiskBusyLayout(chart);
                    break;
                case ChartType.DiskQueuePerDisk:
                    InitializeDiskQueueLayout(chart);
                    chartInfo.ByDisk = true;
                    chartInfo.FilterColumns = true;
                    break;
                case ChartType.DiskQueueTotal:
                    InitializeDiskQueueLayout(chart);
                    break;
                case ChartType.DiskSqlIO:
                    InitializeDiskIOLayout(chart);
                    break;
                case ChartType.DiskSqlIOErrors:
                    InitializeDiskErrorsLayout(chart);
                    break;
                case ChartType.DiskReadsPerSecPerDisk:
                case ChartType.DiskTransfersPerSecPerDisk:
                case ChartType.DiskWritesPerSecPerDisk:
                    InitializeDiskIOPerDiskLayout(chart);
                    chartInfo.ByDisk = true;
                    chartInfo.FilterColumns = true;
                    break;
                case ChartType.DiskTimePerReadPerDisk:
                case ChartType.DiskTimePerTransferPerDisk:
                case ChartType.DiskTimePerWritePerDisk:
                    InitializeDiskTimePerDiskLayout(chart);
                    chartInfo.ByDisk = true;
                    chartInfo.FilterColumns = true;
                    break;
                case ChartType.DiskVMTransferRate:
                    InitializeDiskVMTransferRate(chart);
                    break;
                case ChartType.DiskHostTransferRate:
                    InitializeDiskHostTransferRate(chart);
                    break;
                case ChartType.QueryDuration:
                case ChartType.QueryCPU:
                case ChartType.QueryReads:
                case ChartType.QueryWrites:
                case ChartType.QueryWaits:
                case ChartType.QueryDeadlocks:
                case ChartType.QueryBlocking:
                case ChartType.QueryCPUPerSecond:
                case ChartType.QueryIOPerSecond:
                    InitializeQueryChartLayout(chart);
                    chartInfo.ChartDataType = ChartDataType.Manual;
                    break;
                case ChartType.QueryHistoryDuration:
                case ChartType.QueryHistoryCPU:
                case ChartType.QueryHistoryReads:
                case ChartType.QueryHistoryWrites:
                case ChartType.QueryHistoryWaits:
                case ChartType.QueryHistoryDeadlocks:
                case ChartType.QueryHistoryBlocking:
                case ChartType.QueryHistoryCPUPerSecond:
                case ChartType.QueryHistoryIOPerSecond:
                    InitializeQueryHistoryChartLayout(chart);
                    chartInfo.ChartDataType = ChartDataType.DataSource;
                    break;
                default:
                    break;
            }

            ConfigureChart(chart, chartInfo, isHyperVInstance);//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        }

        private static void InitializeDiskBusyLayout(Chart chart)
        {
            chart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Lines;
            chart.AxisY.Title.Text = "%";
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = false;
            chart.Name = "diskBusyChart";

            chart.Printer.Document.DocumentName = "Disk Busy Chart";
            chart.AxisY.Min = 0;
            chart.AxisY.Max = 100;
        }

        private static void InitializeDiskErrorsLayout(Chart chart)
        {
            chart.LegendBox.Visible = false;
            chart.Name = "readWriteErrorsChart";

            chart.Printer.Document.DocumentName = "Physical I/O Errors Chart";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeDiskIOLayout(Chart chart)
        {
            chart.Name = "physicalReadsWritesChart";

            chart.Printer.Document.DocumentName = "Physical I/O Chart";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeDiskIOPerDiskLayout(Chart chart)
        {
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = false;
            chart.Name = "diskIOPerSecondChart";

            chart.Printer.Document.DocumentName = "Disk I/O Per Second Chart";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeDiskQueueLayout(Chart chart)
        {
            //Chart series gallery issue (existing bug 10.0)
            //SQLdm 10.0 (Srishti Purohit)
            //chart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Bar;
            chart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = false;
            chart.Name = "diskQueueLengthChart";

            chart.Printer.Document.DocumentName = "Average Disk Queue Length Chart";
            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private static void InitializeDiskTimePerDiskLayout(Chart chart)
        {
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = false;
            chart.Name = "diskIOPerSecondChart";

            chart.Printer.Document.DocumentName = "Disk Milliseconds Per I/O Chart";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeDiskVMTransferRate(Chart chart)
        {
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = true;
            chart.Name = "diskVMTransferRate";

            chart.Printer.Document.DocumentName = "VM Data Transfer Per Second";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeDiskHostTransferRate(Chart chart)
        {
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.LegendBox.Visible = true;
            chart.Name = "diskHostTransferRate";

            chart.Printer.Document.DocumentName = "Host Data Transfer Per Second";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        private static void InitializeQueryChartLayout(Chart chart)
        {
            chart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Bar;
            chart.AllSeries.Stacked = ChartFX.WinForms.Stacked.No;
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Right;
            chart.LegendBox.Visible = false;
            chart.Name = "QueryMonitorChart";

            chart.Printer.Document.DocumentName = "Query Monitor Chart";
        }

        private static void InitializeQueryHistoryChartLayout(Chart chart)
        {
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Right;
            chart.LegendBox.Visible = false;
            chart.Name = "QueryMonitorHistoryChart";

            chart.Printer.Document.DocumentName = "Query Monitor History Chart";
            chart.ToolTipFormat = "%v %s\n%x";
        }

        #endregion

        #region configure

        private static void ConfigureChart(Chart chart, ChartInfo chartInfo, object dataSource)
        {
            // restore the saved datasource if the chart type was changed
            if (chartInfo.ChartDataType == ChartDataType.DataSource)
            {
                chart.DataSource = dataSource;
            }
            ConfigureChart(chart, chartInfo);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Allow chartpanel to support multiple datasources depending upon charttype
        /// Resources > Disk charts need to support multiple data sources.
        /// </summary>
        /// <returns></returns>
        public static bool UseFirstDataSource(ChartType type)
        {
            using (LOG.VerboseCall("UseFirstDataSource"))
            {
                switch (type)
                {
                    case ChartType.DiskBusyPerDisk:
                    case ChartType.DiskQueuePerDisk:
                    case ChartType.DiskReadsPerSecPerDisk:
                    case ChartType.DiskTransfersPerSecPerDisk:
                    case ChartType.DiskWritesPerSecPerDisk:
                    case ChartType.DiskTimePerReadPerDisk:
                    case ChartType.DiskTimePerTransferPerDisk:
                    case ChartType.DiskTimePerWritePerDisk:
                        return false;
                }
                return true;
            }
        }

        public static void ConfigureChart(Chart chart, ChartInfo chartInfo, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        {
            using (LOG.VerboseCall("ConfigureChart"))
            {
                switch (chartInfo.ChartType)
                {
                    case ChartType.DiskBusyPerDisk:
                        ConfigureDiskBusyPerDisk(chart, chartInfo);
                        break;
                    case ChartType.DiskBusyTotal:
                        ConfigureDiskBusyTotal(chart, chartInfo);
                        break;
                    case ChartType.DiskQueuePerDisk:
                        ConfigureDiskQueuePerDisk(chart, chartInfo);
                        break;
                    case ChartType.DiskQueueTotal:
                        ConfigureDiskQueueTotal(chart, chartInfo);
                        break;
                    case ChartType.DiskSqlIO:
                        ConfigureDiskIO(chart, chartInfo);
                        break;
                    case ChartType.DiskSqlIOErrors:
                        ConfigureDiskErrors(chart, chartInfo);
                        break;
                    case ChartType.DiskReadsPerSecPerDisk:
                    case ChartType.DiskTransfersPerSecPerDisk:
                    case ChartType.DiskWritesPerSecPerDisk:
                        ConfigureDiskIOPerDisk(chart, chartInfo, isHyperVInstance);//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
                        break;
                    case ChartType.DiskTimePerReadPerDisk:
                    case ChartType.DiskTimePerTransferPerDisk:
                    case ChartType.DiskTimePerWritePerDisk:
                        ConfigureDiskTimePerDisk(chart, chartInfo);
                        break;
                    case ChartType.DiskVMTransferRate:
                        ConfigureDiskVMTransferRate(chart, chartInfo);
                        break;
                    case ChartType.DiskHostTransferRate:
                        ConfigureDiskHostTransferRate(chart, chartInfo);
                        break;
                    case ChartType.QueryDuration:
                    case ChartType.QueryCPU:
                    case ChartType.QueryReads:
                    case ChartType.QueryWrites:
                    case ChartType.QueryWaits:
                    case ChartType.QueryDeadlocks:
                    case ChartType.QueryBlocking:
                    case ChartType.QueryCPUPerSecond:
                    case ChartType.QueryIOPerSecond:
                        ConfigureQueryChart(chart, chartInfo);
                        break;
                    case ChartType.QueryHistoryDuration:
                    case ChartType.QueryHistoryCPU:
                    case ChartType.QueryHistoryReads:
                    case ChartType.QueryHistoryWrites:
                    case ChartType.QueryHistoryWaits:
                    case ChartType.QueryHistoryDeadlocks:
                    case ChartType.QueryHistoryBlocking:
                    case ChartType.QueryHistoryCPUPerSecond:
                    case ChartType.QueryHistoryIOPerSecond:
                        ConfigureQueryHistoryChart(chart, chartInfo);
                        break;
                    default:
                        break;
                }


                chart.SuspendLayout();
                
                // SqlDM 10.2 (Anshul Aggarwal) - Allow chartpanel to support multiple datasources depending upon charttype
                // Resources > Disk charts need to support multiple data sources.
                var dataSource = UseFirstDataSource(chartInfo.ChartType) ? chartInfo.DataSource : chartInfo.DataSource2;
                if (chartInfo.ChartDataType == ChartDataType.DataSource &&
                    chart.DataSource != dataSource &&
                    dataSource != null)
                {
                    chart.DataSource = dataSource;
                }
                chart.DataSourceSettings.ReloadData();

                // force the colors again
                chart.SetAreaSeriesAlphaChannel(175, 0);
                chart.Refresh(); // SQLDM-27489 - Unhandled exception occurring in DM.
                chart.ResumeLayout(false);
            }
        }

        private static void ConfigureDiskBusyPerDisk(Chart chart, ChartInfo chartInfo)
        {
            using (LOG.VerboseCall("ConfigureDiskBusyPerDisk"))
            {
                if (chartInfo.DataObject != null && chartInfo.DataObject is ServerSummaryHistoryData)
                {
                    var historyData = chartInfo.DataObject as ServerSummaryHistoryData;

                    chartInfo.OrderedDiskDrives = GetOrderedDiskDrives(historyData);

                    if (!chartInfo.InitDiskDataColumns)
                        InitDiskDataColumns(chartInfo);
                    else
                        InitializeNewDrives(chartInfo);

                    if (chartInfo.OrderedDiskDrives != null)
                    {
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;
                        chart.ToolTipFormat = "%v%% Disk Busy on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Clear();
                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var map = chartInfo.DiskDataColumns[ChartType.DiskBusyPerDisk];
                        foreach (string diskDrive in chartInfo.OrderedDiskDrives.Reversed().Keys)
                        {
                            var mapDrive = false;
                            if (!map.TryGetValue(diskDrive, out mapDrive) || !mapDrive)
                                continue;

                            var diskFieldMap = new FieldMap(historyData.DiskUsagePerDiskColumnPrefix + diskDrive,
                                                            FieldUsage.Value);
                            diskFieldMap.DisplayName = diskDrive;
                            chart.DataSourceSettings.Fields.Add(diskFieldMap);
                        }
						//Check if baseline enabled --Ankit Nagpal SQLdm 10.0
                        if (Settings.Default.EnableBaseline)
                        {
                            //START: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                            var baselinePlot_os = new FieldMap("OSDiskTimeBaseline", FieldUsage.Value);
                            baselinePlot_os.DisplayName = "Baseline-OS";
                            chart.DataSourceSettings.Fields.Add(baselinePlot_os);
                            //END: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                        }
                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                        chartInfo.InitChartDataColumns = true;
                    }
                    else
                    {
                        LOG.Verbose("Adding dummy field to chart.");
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;
                        chart.ToolTipFormat = "%v%% Disk Busy on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var diskFieldMap = new FieldMap("Junk", FieldUsage.Value);
                        diskFieldMap.DisplayName = "No Drives Available";
                        chart.DataSourceSettings.Fields.Add(diskFieldMap);

                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                    }
                }
            }
        }

        private static void ConfigureDiskBusyTotal(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();
            chart.AllSeries.Gallery = Gallery.Area;
            chart.LegendBox.Visible = false;
            chart.ToolTipFormat = "%v%s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap diskTimeFieldMap = new FieldMap("PercentDiskTime", FieldUsage.Value);
            diskTimeFieldMap.DisplayName = "% Total Disk Busy";

            chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            diskTimeFieldMap
                                                        });

            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureDiskErrors(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap errorsFieldMap = new FieldMap("ReadWriteErrors", FieldUsage.Value);
            errorsFieldMap.DisplayName = "Errors";

            chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                            {
                                                                dateFieldMap,
                                                                errorsFieldMap
                                                            });

            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureDiskIO(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap checkpointWritesFieldMap = new FieldMap("CheckpointWrites", FieldUsage.Value);
            checkpointWritesFieldMap.DisplayName = "Checkpoint Writes";
            FieldMap lazyWriterWritesFieldMap = new FieldMap("LazyWriterWrites", FieldUsage.Value);
            lazyWriterWritesFieldMap.DisplayName = "Lazy Writer Writes";
            FieldMap readAheadReadsFieldMap = new FieldMap("ReadAheadReads", FieldUsage.Value);
            readAheadReadsFieldMap.DisplayName = "Read Ahead Reads";
            FieldMap synchronousReadsFieldMap = new FieldMap("SynchronousReads", FieldUsage.Value);
            synchronousReadsFieldMap.DisplayName = "Page Reads";
            FieldMap synchronousWritesFieldMap = new FieldMap("SynchronousWrites", FieldUsage.Value);
            synchronousWritesFieldMap.DisplayName = "Page Writes";

            chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            checkpointWritesFieldMap,
                                                            lazyWriterWritesFieldMap,
                                                            readAheadReadsFieldMap,
                                                            synchronousReadsFieldMap,
                                                            synchronousWritesFieldMap
                                                        });

            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                    chart.LegendBox.Width = 190;
                if (AutoScaleSizeHelper.isXLargeSize)
                    chart.LegendBox.Width = 225;
                if (AutoScaleSizeHelper.isXXLargeSize)
                    chart.LegendBox.Width = 260;
            }
            else
                chart.LegendBox.Width = 145;
            chart.LegendBox.PlotAreaOnly = false;

            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureDiskIOPerDisk(Chart chart, ChartInfo chartInfo, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        {            
            using (LOG.VerboseCall("ConfigureDiskIOPerDisk"))
            {
                if (chartInfo.DataObject != null && chartInfo.DataObject is ServerSummaryHistoryData)
                {
                    var historyData = chartInfo.DataObject as ServerSummaryHistoryData;

                    chartInfo.OrderedDiskDrives = GetOrderedDiskDrives(historyData);

                    if (!chartInfo.InitDiskDataColumns)
                        InitDiskDataColumns(chartInfo);
                    else
                        InitializeNewDrives(chartInfo);

                    if (chartInfo.OrderedDiskDrives != null)
                    {
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;

                        //START: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                        var baselinePlot_host = new FieldMap("HostDiskReadBaseline", FieldUsage.Value);//Dummy value assigned, would assign final value in the below defined switch only                        
                        var baselinePlot_vm = new FieldMap("VMDiskReadBaseline", FieldUsage.Value);//Dummy value assigned, would assign final value in the below defined switch only                        
                        //END: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 

                        string ioType = "I/O";
                        string columnPrefix = string.Empty;
                        switch (chartInfo.ChartType)
                        {
                            case ChartType.DiskReadsPerSecPerDisk:
                                ioType = "Reads";
                                columnPrefix = historyData.DiskReadsPerSecPerDiskColumnPrefix;
                                ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
                                if (Settings.Default.EnableBaseline && isHyperVInstance)//SQLdm 10.0 (Tarun Sapra)- DE45616: VM&Host related baseline is displaying without VM configuration
                                {                                    
                                    //START: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                                    baselinePlot_host = new FieldMap("HostDiskReadBaseline", FieldUsage.Value);
                                    baselinePlot_host.DisplayName = "Baseline-Host";
                                    chart.DataSourceSettings.Fields.Add(baselinePlot_host);

                                    baselinePlot_vm = new FieldMap("VMDiskReadBaseline", FieldUsage.Value);
                                    baselinePlot_vm.DisplayName = "Baseline-VM";
                                    chart.DataSourceSettings.Fields.Add(baselinePlot_vm);
                                    //END: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                                }
                                break;
                            case ChartType.DiskTransfersPerSecPerDisk:
                                ioType = "Transfers";
                                columnPrefix = historyData.DiskTransfersPerSecPerDiskColumnPrefix;
                                break;
                            case ChartType.DiskWritesPerSecPerDisk:
                                ioType = "Writes";
                                columnPrefix = historyData.DiskWritesPerSecPerDiskColumnPrefix;
                                ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
                                if (Settings.Default.EnableBaseline && isHyperVInstance)//SQLdm 10.0 (Tarun Sapra)- DE45616: VM&Host related baseline is displaying without VM configuration
                                {
                                    //START: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                                    baselinePlot_host = new FieldMap("HostDiskWriteBaseline", FieldUsage.Value);
                                    baselinePlot_host.DisplayName = "Baseline-Host";
                                    chart.DataSourceSettings.Fields.Add(baselinePlot_host);

                                    baselinePlot_vm = new FieldMap("VMDiskWriteBaseline", FieldUsage.Value);
                                    baselinePlot_vm.DisplayName = "Baseline-VM";
                                    chart.DataSourceSettings.Fields.Add(baselinePlot_vm);
                                    //END: SQLdm 10.0 (Tarun Sapra) - Baseline plotting 
                                }
                                break;
                            default:
                                break;
                        }
                        chart.Printer.Document.DocumentName = string.Format("Disk {0} Per Second Chart", ioType);

                        chart.ToolTipFormat = string.Format("%v {0} Per Second on Drive %s\n%x", ioType);
                        
                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var map = chartInfo.DiskDataColumns[chartInfo.ChartType];
                        foreach (string diskDrive in chartInfo.OrderedDiskDrives.Reversed().Keys)
                        {
                            var mapDrive = false;
                            if (!map.TryGetValue(diskDrive, out mapDrive) || !mapDrive)
                                continue;

                            var diskFieldMap = new FieldMap(string.Format("{0}{1}", columnPrefix, diskDrive),
                                                            FieldUsage.Value);
                            diskFieldMap.DisplayName = diskDrive;
                            chart.DataSourceSettings.Fields.Add(diskFieldMap);
                        }

                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                        chart.AxisY.DataFormat.Decimals = 2;

                        chartInfo.InitChartDataColumns = true;
                    }
                    else
                    {
                        LOG.Verbose("Adding dummy field to chart.");
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;
                        chart.ToolTipFormat = "%v%% IO Per Second on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var diskFieldMap = new FieldMap("Junk", FieldUsage.Value);
                        diskFieldMap.DisplayName = "No Drives Available";
                        chart.DataSourceSettings.Fields.Add(diskFieldMap);

                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                    }
                }
            }
        }

        private static void ConfigureDiskTimePerDisk(Chart chart, ChartInfo chartInfo)
        {
            using (LOG.VerboseCall("ConfigureDiskTimePerDisk"))
            {
                if (chartInfo.DataObject != null && chartInfo.DataObject is ServerSummaryHistoryData)
                {
                    var historyData = chartInfo.DataObject as ServerSummaryHistoryData;

                    chartInfo.OrderedDiskDrives = GetOrderedDiskDrives(historyData);

                    if (!chartInfo.InitDiskDataColumns)
                        InitDiskDataColumns(chartInfo);
                    else
                        InitializeNewDrives(chartInfo);

                    if (chartInfo.OrderedDiskDrives != null)
                    {
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;
                        string ioType = "I/O";
                        string columnPrefix = string.Empty;
                        switch (chartInfo.ChartType)
                        {
                            case ChartType.DiskTimePerReadPerDisk:
                                ioType = "Read";
                                columnPrefix = historyData.DiskTimePerReadPerDiskColumnPrefix;
                                break;
                            case ChartType.DiskTimePerTransferPerDisk:
                                ioType = "Transfer";
                                columnPrefix = historyData.DiskTimePerTransferPerDiskColumnPrefix;
                                break;
                            case ChartType.DiskTimePerWritePerDisk:
                                ioType = "Write";
                                columnPrefix = historyData.DiskTimePerWritePerDiskColumnPrefix;
                                break;
                            default:
                                break;
                        }
                        chart.Printer.Document.DocumentName = string.Format("Disk Milliseconds Per {0} Chart", ioType);

                        chart.ToolTipFormat = string.Format("%v Milliseconds Per {0} on Drive %s\n%x", ioType);

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var map = chartInfo.DiskDataColumns[chartInfo.ChartType];
                        foreach (string diskDrive in chartInfo.OrderedDiskDrives.Reversed().Keys)
                        {
                            var mapDrive = false;
                            if (!map.TryGetValue(diskDrive, out mapDrive) || !mapDrive)
                                continue;

                            var diskFieldMap = new FieldMap(string.Format("{0}{1}", columnPrefix, diskDrive),
                                                            FieldUsage.Value);
                            diskFieldMap.DisplayName = diskDrive;
                            chart.DataSourceSettings.Fields.Add(diskFieldMap);
                        }
                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                        chart.AxisY.DataFormat.Decimals = 2;

                        chartInfo.InitChartDataColumns = true;
                    }
                    else
                    {
                        LOG.Verbose("Adding dummy field to chart.");
                        chart.DataSourceSettings.Fields.Clear();
                        chart.AllSeries.Gallery = Gallery.Lines;
                        chart.LegendBox.Visible = true;
                        chart.LegendBox.ItemAttributes[chart.Series].Inverted = true;
                        chart.ToolTipFormat = "%v%% Milliseconds Per IO on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var diskFieldMap = new FieldMap("Junk", FieldUsage.Value);
                        diskFieldMap.DisplayName = "No Drives Available";
                        chart.DataSourceSettings.Fields.Add(diskFieldMap);

                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                    }
                }
            }
        }

        private static void ConfigureDiskQueuePerDisk(Chart chart, ChartInfo chartInfo)
        {
            using (LOG.VerboseCall("ConfigureDiskQueuePerDisk"))
            {
                if (chartInfo.DataObject != null && chartInfo.DataObject is ServerSummaryHistoryData)
                {
                    var historyData = chartInfo.DataObject as ServerSummaryHistoryData;

                    chartInfo.OrderedDiskDrives = GetOrderedDiskDrives(historyData);

                    if (!chartInfo.InitDiskDataColumns)
                        InitDiskDataColumns(chartInfo);
                    else
                        InitializeNewDrives(chartInfo);

                    if (chartInfo.OrderedDiskDrives != null)
                    {
                        chart.DataSourceSettings.Fields.Clear();
                        chart.LegendBox.Visible = true;
                        chart.ToolTipFormat = "%v Items Queued on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var map = chartInfo.DiskDataColumns[ChartType.DiskQueuePerDisk];
                        foreach (string diskDrive in chartInfo.OrderedDiskDrives.Reversed().Keys)
                        {
                            var mapDrive = false;
                            if (!map.TryGetValue(diskDrive, out mapDrive) || !mapDrive)
                                continue;

                            var diskFieldMap = new FieldMap(historyData.DiskQueueLengthPerDiskColumnPrefix + diskDrive,
                                                            FieldUsage.Value);
                            diskFieldMap.DisplayName = diskDrive;
                            chart.DataSourceSettings.Fields.Add(diskFieldMap);
                        }

                        chartInfo.InitChartDataColumns = true;
                    }
                    else
                    {
                        LOG.Verbose("Adding dummy field to chart.");
                        chart.DataSourceSettings.Fields.Clear();
                        chart.LegendBox.Visible = true;

                        chart.ToolTipFormat = "%v%% Items Queued on Drive %s\n%x";

                        chart.DataSourceSettings.Fields.Add(new FieldMap("Date", FieldUsage.XValue));

                        var diskFieldMap = new FieldMap("Junk", FieldUsage.Value);
                        diskFieldMap.DisplayName = "No Drives Available";
                        chart.DataSourceSettings.Fields.Add(diskFieldMap);

                        chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                        chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                    }
                }
            }
        }

        private static void ConfigureDiskQueueTotal(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();

            chart.LegendBox.Visible = true;           
            chart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            chart.ToolTipFormat = "%v Total Items Queued\n%x";

            chart.RandomData.Series = 2;
            chart.Series[0].Gallery = Gallery.Bar;
            //Series[1] will only be available if data source is also containing baseline data
            if (chart.Series.Count > 1)
                  chart.Series[1].Gallery = Gallery.Lines;
                        
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap diskQueueLenthFieldMap = new FieldMap("DiskQueueLength", FieldUsage.Value);
            diskQueueLenthFieldMap.DisplayName = "Total";

            //START: SQLdm 10.0 - Baseline plot
            FieldMap osAverageDiskQueueLengthBaseline = new FieldMap("OSAverageDiskQueueLengthBaseline", FieldUsage.Value);
            osAverageDiskQueueLengthBaseline.DisplayName = "Baseline-OSAverageDiskQueueLength";
            ///Check if baseline enabled --Ankit Nagpal SQLdm 10.0
			if (Settings.Default.EnableBaseline)
            {
                chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            diskQueueLenthFieldMap,
                                                            osAverageDiskQueueLengthBaseline
                                                        });
            }
            else
            {
                chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            diskQueueLenthFieldMap
                                                           
                                                        });
            }
            //END: SQLdm 10.0 - Baseline plot
            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureDiskVMTransferRate(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap readsFieldMap = new FieldMap("vmDiskRead", FieldUsage.Value);
            readsFieldMap.DisplayName = "Disk Read (KB/s)";
            FieldMap writesFieldMap = new FieldMap("vmDiskWrite", FieldUsage.Value);
            writesFieldMap.DisplayName = "Disk Write (KB/s)";

            chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                            {
                                                                dateFieldMap,
                                                                readsFieldMap,
                                                                writesFieldMap
                                                            });
            //START: SQLdm 10.0 (Tarun Sapra) - DE45729: 'VM Disk Usage& Host Disk Usage' baselines are implemented in wrong location
            if (Settings.Default.EnableBaseline)
            {
                var baselinePlot_vm = new FieldMap("VMDiskUsageBaseline", FieldUsage.Value);
                baselinePlot_vm.DisplayName = "Baseline-VM";
                chart.DataSourceSettings.Fields.Add(baselinePlot_vm);
            }
            //END: SQLdm 10.0 (Tarun Sapra) - DE45729: 'VM Disk Usage& Host Disk Usage' baselines are implemented in wrong location
            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureDiskHostTransferRate(Chart chart, ChartInfo chartInfo)
        {
            chart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap readsFieldMap = new FieldMap("esxDiskRead", FieldUsage.Value);
            readsFieldMap.DisplayName = "Disk Read (KB/s)";
            FieldMap writesFieldMap = new FieldMap("esxDiskWrite", FieldUsage.Value);
            writesFieldMap.DisplayName = "Disk Write (KB/s)";

            chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                            {
                                                                dateFieldMap,
                                                                readsFieldMap,
                                                                writesFieldMap
                                                            });
            //START: SQLdm 10.0 (Tarun Sapra) - DE45729: 'VM Disk Usage& Host Disk Usage' baselines are implemented in wrong location
            if (Settings.Default.EnableBaseline)
            {
                var baselinePlot_host = new FieldMap("HostDiskUsageBaseline", FieldUsage.Value);
                baselinePlot_host.DisplayName = "Baseline-Host";
                chart.DataSourceSettings.Fields.Add(baselinePlot_host);
            }
            //END: SQLdm 10.0 (Tarun Sapra) - DE45729: 'VM Disk Usage& Host Disk Usage' baselines are implemented in wrong location

            chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            chartInfo.InitChartDataColumns = true;
        }

        private static void ConfigureQueryChart(Chart chart, ChartInfo chartInfo)
        {
            if (chartInfo.DataObject != null && chartInfo.DataObject is QueryMonitorData &&
                chartInfo.DataSource != null && chartInfo.DataSource is DataTable)
            {
                QueryMonitorData queryMonitorData = chartInfo.DataObject as QueryMonitorData;
                string dataColumn = QueryMonitorData.GetSummaryDataColumn(chartInfo.ChartType, chartInfo.ChartView);
                switch (chartInfo.ChartType)
                {
                    case ChartType.QueryDuration:
                    case ChartType.QueryCPU:
                    case ChartType.QueryWaits:
                    case ChartType.QueryBlocking:
                        chart.AxisX.LabelsFormat.Format = AxisFormat.Time;
                        chart.AxisY.DataFormat.Decimals = 0;
                        chart.AxisY.Title.Text = "ms";
                        break;
                    case ChartType.QueryCPUPerSecond:
                    case ChartType.QueryIOPerSecond:
                        chart.AxisX.LabelsFormat.Format = AxisFormat.Time;
                        chart.AxisY.DataFormat.Decimals = 2;
                        chart.AxisY.Title.Text = "per second";
                        break;
                    case ChartType.QueryReads:
                    case ChartType.QueryWrites:
                    case ChartType.QueryDeadlocks:
                        break;
                    default:
                        dataColumn = string.Empty;
                        break;
                }

                //chart.DataSourceSettings.Fields.Clear();
                chart.SuspendLayout();
                chart.Series.Clear();

                DataView queries = queryMonitorData.GetSummaryView(chartInfo.ChartType, chartInfo.ChartView);
                if (queries != null)
                {
                    string rowFilter = null;
                    string labelColumn = QueryMonitorData.COL_SUM_ColumnValue;
                    switch (chartInfo.ChartView)
                    {
                        case ChartView.QueryApplication:
                            rowFilter = string.Format(QueryMonitorData.FORMAT_SUMMARY_ROWFILTER, QueryMonitorData.COL_DB_Application);
                            break;
                        case ChartView.QueryDatabase:
                            rowFilter = string.Format(QueryMonitorData.FORMAT_SUMMARY_ROWFILTER, QueryMonitorData.COL_DB_Database);
                            break;
                        case ChartView.QuerySQL:
                            rowFilter = string.Empty;
                            labelColumn = QueryMonitorData.COL_NAME;
                            break;
                        case ChartView.QueryUser:
                            rowFilter = string.Format(QueryMonitorData.FORMAT_SUMMARY_ROWFILTER, QueryMonitorData.COL_DB_User);
                            break;
                        case ChartView.QueryHost:
                            rowFilter = string.Format(QueryMonitorData.FORMAT_SUMMARY_ROWFILTER, QueryMonitorData.COL_DB_Host);
                            break;
                        default:
                            break;
                    }

                    if (rowFilter != null)
                    {
                        queries.RowFilter = rowFilter;
                        int rowCount = System.Math.Min(queries.Count, chartInfo.ChartDisplayValues);
                        PointAttributes[] chartPointAttributes = getCommonPointAttributes(rowCount);

                        chart.Data.Series = 1;
                        chart.Data.Points = rowCount;
                        chart.ToolTipFormat = string.Format("%v {0}\n%L", chart.AxisY.Title.Text);
                        chart.Series[0].Text = ChartHelper.ChartViewName(chartInfo.ChartView);
                        chart.Series[0].AxisX.LabelAngle = 0;
                        chart.Series[0].AxisX.Staggered = true;
                        chart.Series[0].AxisX.LabelTrimming = StringTrimming.EllipsisCharacter;

                        for (int i = 0; i < rowCount; i++)
                        {
                            DataRowView row = queries[i];
                            long val = 0;
                            // value can be null for some fields with small data sets
                            if (row[dataColumn] != DBNull.Value)
                            {
                                val = Convert.ToInt64(row[dataColumn]);
                            }
                            chart.Data[0, i] = val;
                            string label = row[labelColumn].ToString();
                            if (label.Length > 30)
                            {
                                label = string.Format("{0}...", label.Substring(0, 25));
                            }
                            chart.Data.Labels[i] = label;
                            chartPointAttributes[i].Text = row[labelColumn].ToString();
                            chart.Points[0, i] = chartPointAttributes[i];
                            chart.Points[0, i].Text = row[labelColumn].ToString();
                            chart.Points[0, i].PointLabels.Angle = 0;
                            if (chartInfo.ChartView == ChartView.QuerySQL)
                            {
                                chart.Points[0, i].Tag = row[QueryMonitorData.COL_DB_SignatureID].ToString();
                            }
                            else
                            {
                                chart.Points[0, i].Tag = row[labelColumn].ToString();
                            }
                        }
                    }
                    chart.Printer.Document.DocumentName = string.Format("{0} Chart", chartInfo.ChartTitle);

                    chart.RecalculateScale();

                    chartInfo.InitChartDataColumns = true;
                }
                chart.Invalidate();
                chart.ResumeLayout();
            }
        }

        private static void ConfigureQueryHistoryChart(Chart chart, ChartInfo chartInfo)
        {
            if (chartInfo.DataObject != null && chartInfo.DataObject is QueryMonitorData &&
                chartInfo.DataSource != null && chartInfo.DataSource is DataTable)
            {
                QueryMonitorData queryMonitorData = chartInfo.DataObject as QueryMonitorData;
                string dataColumn = QueryMonitorData.GetSummaryDataColumn(chartInfo.ChartType, chartInfo.ChartView);

                chart.DataSourceSettings.Fields.Clear();

                FieldMap dateFieldMap = new FieldMap(QueryMonitorData.COL_DB_CompletionTime, FieldUsage.XValue);
                FieldMap valueFieldMap = new FieldMap(dataColumn, FieldUsage.Value);
                valueFieldMap.DisplayName = dataColumn;

                chart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                            {
                                                                dateFieldMap,
                                                                valueFieldMap
                                                            });

                chart.AxisX.LabelsFormat.Format = AxisFormat.DateTime;

                chart.Printer.Document.DocumentName = string.Format("{0} Chart", chartInfo.ChartTitle);

                chart.LegendBox.Width = 145;
                chart.LegendBox.PlotAreaOnly = false;
                chartInfo.InitChartDataColumns = true;
            }
        }

        private static PointAttributes[] getCommonPointAttributes(int count)
        {
            PointAttributes[] chartPointAttributes = new PointAttributes[count];
            for (int i = 0; i < count; i++)
            {
                chartPointAttributes[i] = new PointAttributes();

                //repeat the colors if there are more values than colors
                chartPointAttributes[i].Color = defaultLegendColors[i % defaultLegendColors.Length];
            }

            return chartPointAttributes;
        }

        // copied from ResourcesWaitStats
        private static Color[] defaultLegendColors = new Color[]
        {
            Color.FromArgb(38, 100, 193),
            Color.FromArgb(199, 56, 0),
            Color.FromArgb(70, 177, 194),
            Color.FromArgb(118, 200, 45),
            Color.FromArgb(236, 179, 70),
            Color.FromArgb(186, 163, 240),
            Color.FromArgb(34, 137, 153),
            Color.FromArgb(250, 117, 80),
            Color.FromArgb(154, 206, 214),
            Color.FromArgb(30, 84, 92),
            Color.FromArgb(45, 118, 227),
            Color.FromArgb(205, 252, 162),
            Color.FromArgb(116, 38, 189),
            Color.FromArgb(154, 206, 214),
            Color.FromArgb(255, 231, 171),
            Color.FromArgb(215, 57, 214),
            Color.FromArgb(38, 100, 193),
            Color.FromArgb(199, 56, 0),
            Color.FromArgb(70, 177, 194),
        };

        #endregion

        #region Columns

        public static void SelectChartDataColumns(ChartDataColumnType columnType, Chart chart, ChartInfo chartInfo)
        {
            if (columnType == ChartDataColumnType.Disk)
            {
                selectDiskDataColumns(chart, chartInfo);
            }
        }

        private static void selectDiskDataColumns(Chart chart, ChartInfo chartInfo)
        {
            var selectedColumns = chartInfo.DiskDataColumns[chartInfo.ChartType];
            var drives = new List<DiskDrive>();

            if (chartInfo.OrderedDiskDrives != null)
            {
                using (var dialog = new CheckedListDialog<DiskDrive>())
                {
                    dialog.Text = "Select data to display";

                    foreach (string drivename in chartInfo.OrderedDiskDrives.Keys)
                    {
                        dialog.AddItem(drivename, chartInfo.OrderedDiskDrives[drivename], selectedColumns[drivename]);
                        drives.Add(chartInfo.OrderedDiskDrives[drivename]);
                    }

                    if (dialog.ShowDialog(chart) == System.Windows.Forms.DialogResult.Cancel)
                        return;

                    foreach (string drivename in chartInfo.OrderedDiskDrives.Keys)
                        selectedColumns[drivename] = false;

                    foreach (var drive in dialog.SelectedItems)
                        selectedColumns[drive.DriveLetter] = true;
                }

            }
            // update the data
            ConfigureChart(chart, chartInfo);
        }

        #endregion


        #region Disk

        internal static void InitDiskDataColumns(ChartInfo chartInfo)
        {
            chartInfo.DiskDataColumns = new Dictionary<ChartType, Dictionary<string, bool>>();
            chartInfo.DiskDataColumns.Add(ChartType.DiskBusyPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskQueuePerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskReadsPerSecPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskTransfersPerSecPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskWritesPerSecPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskTimePerReadPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskTimePerTransferPerDisk, new Dictionary<string, bool>());
            chartInfo.DiskDataColumns.Add(ChartType.DiskTimePerWritePerDisk, new Dictionary<string, bool>());

            if (chartInfo.OrderedDiskDrives != null && chartInfo.OrderedDiskDrives.Count > 0)
            {
                foreach (string drivename in chartInfo.OrderedDiskDrives.Keys)
                {
                    chartInfo.DiskDataColumns[ChartType.DiskBusyPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskQueuePerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskReadsPerSecPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskTransfersPerSecPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskWritesPerSecPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerReadPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerTransferPerDisk].Add(drivename, true);
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerWritePerDisk].Add(drivename, true);
                }

                chartInfo.InitDiskDataColumns = true;
            }
        }

        internal static OrderedDictionary<string, DiskDrive> GetOrderedDiskDrives(ServerSummaryHistoryData historyData)
        {
            OrderedDictionary<string, DiskDrive> orderedDiskDrives = null;

            if (historyData.LastServerOverviewSnapshot != null)
            {
                orderedDiskDrives = new OrderedDictionary<string, DiskDrive>(historyData.LastServerOverviewSnapshot.DiskDrives);
            }
            else if (historyData.HistoricalSnapshots != null && historyData.HistoricalSnapshots.ServerOverviewSnapshot != null)
            {
                orderedDiskDrives = new OrderedDictionary<string, DiskDrive>(historyData.HistoricalSnapshots.ServerOverviewSnapshot.DiskDrives);
            }
            if (orderedDiskDrives == null || orderedDiskDrives.Count == 0)
                return null;

            var instance = ApplicationModel.Default.ActiveInstances[historyData.InstanceId].Instance;
            if (instance.DiskCollectionSettings.AutoDiscover)
                return orderedDiskDrives;

            if (instance.DiskCollectionSettings.Drives == null || instance.DiskCollectionSettings.Drives.Length == 0)
                return orderedDiskDrives;

            var result = new OrderedDictionary<string, DiskDrive>();
            var configuredDrives = instance.DiskCollectionSettings.Drives.Select(drive => drive.TrimEnd(':')).ToList();
            var intersection = Algorithms.SetIntersection(configuredDrives, orderedDiskDrives.Keys, StringComparer.CurrentCultureIgnoreCase);
            foreach (var key in intersection)
            {
                result.Add(key, orderedDiskDrives[key]);
            }

            LogDiskJunk(configuredDrives, orderedDiskDrives.Keys, result);

            return result.Count != 0 ? result : null;
        }

        private static void LogDiskJunk(List<string> configuredDrives, ICollection<string> collectedDrives, OrderedDictionary<string,DiskDrive> intersection)
        {
            var cfg = new StringBuilder();
            foreach (var drive in configuredDrives)
                cfg.Append(drive).Append(",");
            if (cfg.Length > 0) cfg.Length -= 1;
            var coll = new StringBuilder();
            foreach (var drive in collectedDrives)
                coll.Append(drive).Append(",");
            if (coll.Length > 0) coll.Length -= 1;

            var ix = new StringBuilder();
            foreach (var drive in intersection.Keys)
                ix.Append(drive).Append(",");
            if (ix.Length > 0) ix.Length -= 1;

            LOG.Verbose("configured: ", cfg, "    collected: ", coll, "    intersection: ", ix);
        }

        private static void InitializeNewDrives(ChartInfo chartInfo)
        {
            if (chartInfo.OrderedDiskDrives == null || chartInfo.OrderedDiskDrives.Count == 0) return;

            foreach (var drivename in Algorithms.SetDifference(chartInfo.OrderedDiskDrives.Keys, chartInfo.DiskDataColumns[ChartType.DiskBusyPerDisk].Keys))
            {
                if (!chartInfo.DiskDataColumns[ChartType.DiskBusyPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskBusyPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskQueuePerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskQueuePerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskReadsPerSecPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskReadsPerSecPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskTransfersPerSecPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskTransfersPerSecPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskWritesPerSecPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskWritesPerSecPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskTimePerReadPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerReadPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskTimePerTransferPerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerTransferPerDisk].Add(drivename, true);
                if (!chartInfo.DiskDataColumns[ChartType.DiskTimePerWritePerDisk].ContainsKey(drivename))
                    chartInfo.DiskDataColumns[ChartType.DiskTimePerWritePerDisk].Add(drivename, true);
            }
        }

        #endregion

        #region Chart Drilldown Functionality

        /// <summary>
        /// Constructs drilldown event args with specified x-range
        /// </summary>
        /// <param name="chart">Chart</param>
        /// <param name="downXValue">start x-value</param>
        /// <param name="upXValue">end x-value</param>
        public static ChartDrilldownEventArgs ConstructDrilldownEventArgs(Chart chart, double downXValue, double upXValue)
        {
            if (downXValue > upXValue)
            {
                var temp = downXValue;
                downXValue = upXValue;
                upXValue = temp;
            }

            DateTime startDateTime, endDateTime;
            // SqlDM 10.2 (Anshul Aggarwal) - Continous and Categorical charts have different logic for drilldown functionality.
            if (IsContinousChart(chart))
            {
                if (downXValue < chart.AxisX.Min)
                    downXValue = chart.AxisX.Min;

                if (upXValue > chart.AxisX.Max)
                    upXValue = chart.AxisX.Max;

                startDateTime = ConvertXToDateTime(downXValue); // Extract Time from x-values
                endDateTime = ConvertXToDateTime(upXValue);
            }
            else
            {
                // SqlDM 10.2 (Anshul Aggarwal) - Calculate start and end labels, extract datetime from these labels.
                downXValue -= BarChartLabelStart;
                if (downXValue < 0)
                    downXValue = 0;

                upXValue-= BarChartLabelStart;
                if (upXValue < 0)
                    upXValue = 0;

                if (downXValue > chart.AxisX.Labels.Count - 1)
                    downXValue = chart.AxisX.Labels.Count - 1;

                if (upXValue > chart.AxisX.Labels.Count - 1)
                    upXValue = chart.AxisX.Labels.Count - 1;

                if (!DateTime.TryParse(chart.AxisX.Labels[(int)downXValue], out startDateTime))
                    return null;

                if (!DateTime.TryParse(chart.AxisX.Labels[(int)upXValue], out endDateTime))
                    return null;
            }

            return new ChartDrilldownEventArgs(endDateTime, (int)Math.Ceiling(endDateTime.Subtract(startDateTime).TotalMinutes));
        }

        /// <summary>
        /// Converts x-axis value to datetime.
        /// </summary>
        /// <param name="xValue">x-axis value</param>
        /// <returns>Datetime</returns>
        private static DateTime ConvertXToDateTime(double xValue)
        {
            return DateTime.FromOADate(xValue);
        }
        
        /// <summary>
        /// Checks if mouse is inside chart plotting area.
        /// </summary>
        public static bool IsMouseInsideChartArea(HitTestEventArgs e)
        {
            return (e.HitType == HitType.InsideArea || e.HitType == HitType.Point || e.HitType == HitType.AxisSection ||
                    e.HitType == HitType.CustomGridLine || e.HitType == HitType.Between);
        }

        /// <summary>
        /// Initiates drilldown UI on the chart.
        /// </summary>
        public static void StartChartDrilldown(Chart chart, double downXValue)
        {
            // Axis sections are being used to highlight the region not in drilldown
            var leftRegion = new AxisSection() { BackColor = Color.FromArgb(175, Color.Gray), Visible = false };
            var rightRegion = new AxisSection() { BackColor = Color.FromArgb(175, Color.Gray), Visible = false };
            chart.AxisX.Sections.AddRange(new[] { leftRegion, rightRegion });

            // SqlDM 10.2 (Anshul Aggarwal) - Continous and Categorical charts have different logic for drilldown functionality.
            if (IsContinousChart(chart))
                chart.AxisX.Sections[0].From = chart.AxisX.Min; // On Mouse down, we need to set left gray region
            else
                chart.AxisX.Sections[0].From = 0;

            chart.AxisX.Sections[0].To = downXValue;
            chart.AxisX.Sections[0].Visible = true;

            LOG.Info("Perform drilldown on chart - " + chart.Name);
        }

        /// <summary>
        /// Changes chart cursor based on mouse location.
        /// </summary>
        public static void OnDrilldownHover(Chart chart, HitTestEventArgs e, Cursor cursor)
        {
            try
            {
                // Need to change cursor to Cross when hovering over the chart
                // HitTest is not performed by default to improve performance, but we need it to determine mouse location.
                e = chart.HitTest(e.AbsoluteLocation.X, e.AbsoluteLocation.Y, true);
                if (!ChartHelper.IsMouseInsideChartArea(e))
                    chart.Cursor = cursor;
                else
                    chart.Cursor = Cursors.Cross;
            }
            catch (Exception exception)
            {
                LOG.Warn("Caught mouse hover exception - " + exception);
            }
        }

        /// <summary>
        /// Updates drilldown UI with mouse movements.
        /// </summary>
        public static void UpdateChartDrilldown(Chart chart, double downXValue, double curXValue)
        {
            if (downXValue.Equals(curXValue))  // If downValue and curXValue are same, no need to change the shaded regions
                return;

            chart.SuspendLayout(); // Performing bulk changes
            
            chart.AxisX.Sections[1].Visible = true; // Make right shaded area visible.
            
            // SqlDM 10.2 (Anshul Aggarwal) - Continous and Categorical charts have different logic for drilldown functionality.
            if (IsContinousChart(chart))
            {
                chart.AxisX.Sections[0].From = chart.AxisX.Min;
                chart.AxisX.Sections[1].To = chart.AxisX.Max;
            }
            else
            {
                chart.AxisX.Sections[0].From = 0;
                chart.AxisX.Sections[1].To = chart.AxisX.Labels.Count + 1;
            }

            if (downXValue < curXValue)
            {
                chart.AxisX.Sections[0].To = downXValue;
                chart.AxisX.Sections[1].From = curXValue;

            }
            else
            {
                chart.AxisX.Sections[0].To = curXValue;
                chart.AxisX.Sections[1].From = downXValue;
            }

            chart.ResumeLayout(false);  // Finished Bulk Changes
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Indicates whether drilldown is supported on current chart or not.
        /// </summary>
        public static bool IsDrilldownSupportedOnChart(Chart chart)
        {
            return (chart.Gallery == Gallery.Lines || chart.Gallery == Gallery.Area) || 
                (chart.Gallery == Gallery.Bar && chart.Tag != null && chart.Tag.ToString() == 
                Common.Constants.QUERY_WAITS_CHART_NAME);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Indicares whether current chart is continous or categorical
        /// </summary>
        public static bool IsContinousChart(Chart chart)
        {
            return (chart.Gallery == Gallery.Lines || chart.Gallery == Gallery.Area);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Due to real-time data, chart's axis changes will user might still be performing drilldown.
        /// To update the highlighted section due to axis change, this method is being called.
        /// </summary>
        public static void OnChartPrePaint(object sender)
        {
            Chart chart = sender as Chart;
            if (chart == null || !ChartHelper.IsDrilldownSupportedOnChart(chart) || chart.AxisX.Sections.Count < 2 || !chart.AxisX.Sections[1].Visible)
                return;

            // SqlDM 10.2 (Anshul Aggarwal) - Continous and Categorical charts have different logic for drilldown functionality.
            if (ChartHelper.IsContinousChart(chart))
            {
                chart.AxisX.Sections[0].From = chart.AxisX.Min;  // To handle real time data, in which x-Axis changes
                chart.AxisX.Sections[1].To = chart.AxisX.Max;
            }
            else
            {
                chart.AxisX.Sections[0].From = 0;
                chart.AxisX.Sections[1].To = chart.AxisX.Labels.Count + 1;
            }
        }

        #endregion

    }


    #region Chart Drilldown Classes
   
    /// <summary>
    /// Encapsulates properties used for drilldown functionality.
    /// </summary>
    public class ChartDrillDownProperties
    {
        public IList<Chart> Charts { get; private set; }
        public Cursor ChartCursor { get; set; }

        public ChartDrillDownProperties()
        {
            this.Charts = new List<Chart>();
            this.ChartCursor = Cursors.Default;
        }

        public void AddCharts(params Chart[] charts)
        {
            if (charts == null)
                return;
            foreach(var chart in charts)
                this.Charts.Add(chart);
        }

        public void ClearCharts()
        {
            this.Charts.Clear();
        }
    }

    /// <summary>
    /// EventArgs containing drilldown information.
    /// </summary>
    public class ChartDrilldownEventArgs : EventArgs
    {
        public int Minutes { get; private set; }
        public DateTime HistoricalSnapshotDateTime { get; private set; }

        public ChartDrilldownEventArgs(DateTime historicalSnapshotDateTime, int minutes = 0)
        {
            this.Minutes = minutes;
            this.HistoricalSnapshotDateTime = historicalSnapshotDateTime;
        }

    }
    
    #endregion

    public class ChartFxContextMenu : ContextMenu
    {
        private readonly ChartFX.WinForms.Chart chart;
        private readonly ContextMenuManager toolbarsManager;
        private string popupMenuKey;

        public ChartFxContextMenu(ChartFX.WinForms.Chart chart, ContextMenuManager toolbarsManager, string key)
        {
            this.chart = chart;
            this.toolbarsManager = toolbarsManager;
            this.popupMenuKey = key;
        }

        public string PopupMenuKey
        {
            get { return popupMenuKey ?? String.Empty; }
            set { popupMenuKey = value; }
        }

        protected override void OnPopup(EventArgs e)
        {
            if (chart.ContextMenu == this)
            {
                toolbarsManager.ShowPopup(popupMenuKey, Control.MousePosition, chart, true);
            }
        }
    }

    public static class ChartFxExtensions
    {
        public static void SetAreaSeriesAlphaChannel(this Chart chart, int alpha)
        {
            SetAreaSeriesAlphaChannel(chart, alpha, false);
        }

        public static void SetAreaSeriesAlphaChannel(this Chart chart, int alpha, bool allSeriesTypes)
        {
            foreach (SeriesAttributes series in chart.Series)
            {
                if (allSeriesTypes || series.Gallery == Gallery.Area)
                {
                    if (series.Color.A != alpha)
                        series.Color = Color.FromArgb(alpha, series.Color);
                }
            }
        }

        public static void SetAreaSeriesAlphaChannel(this Chart chart, int alpha, int alternateAlpha)
        {
            if (chart.Series.Count == 1)
                alternateAlpha = 255;

            foreach (SeriesAttributes series in chart.Series)
            {
                if (series.Gallery == Gallery.Area)
                {
                    if (series.Color.A != alpha)
                        series.Color = Color.FromArgb(alpha, series.Color);

                    if (series.Stacked || series.FillMode == FillMode.Gradient)
                    {
                        if (alternateAlpha == 255)
                        {
                            if (series.Color != series.AlternateColor)
                                series.AlternateColor = series.Color;
                        }
                        else
                            if (series.AlternateColor.A != alternateAlpha)
                                series.AlternateColor = Color.FromArgb(alpha, series.AlternateColor);
                    }
                }
                else
                {
                    series.Color = Color.FromArgb(255, series.Color);
                } 
            }
        }

        public static void SetContextMenu(this Chart chart, ContextMenuManager toolbarsManager)
        {
            string popupMenuKey = toolbarsManager.GetContextMenuUltra(chart);
            if (String.IsNullOrEmpty(popupMenuKey)) return;
            var menu = new ChartFxContextMenu(chart, toolbarsManager, popupMenuKey);
            chart.ContextMenu = menu;
        }

        private const double OAHour = 24;
        private const double OAMinute = 1440;
        private const double OASecond = 86400;

        public static void SetAxisXTimeScale(ChartFX.WinForms.Chart chart, int steps)
        {
            if (chart.AxisX.LabelsFormat.Format != ChartHelper.TimeChartAxisFormat)
            {
                chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            }

            if (chart.AxisX.LabelTrimming == StringTrimming.None)
                chart.AxisX.LabelTrimming = StringTrimming.Character;

            if (chart.Data.Points < 2) return;

            var data = chart.Data.X;
            if (data == null || !data.HasData) return;

            //    if (chart.AxisX.AutoScale)
            //        chart.AxisX.AutoScale = false;

            chart.AxisX.Style |= AxisStyles.ShowEnds;

            //            Debug.Print("Style={0} AxisX.Style={1}", chart.ExtraStyle, chart.AxisX.Style);

            //var first = data[0];//sql dm 10.0 vineet - commented out because not in use
            //var last = data[chart.Data.Points - 1];//sql dm 10.0 vineet - commented out because not in use

            //TimeSpan stepSpan = TimeSpan.FromHours((last - first) / steps * 24); //sqldm 10.0 vineet - it is not used anywhere but sometimes used to throw exception of overflow, so commented it out.

            //if (stepSpan.TotalDays >= 1.0d)
            //    chart.AxisX.Step = Math.Truncate(stepSpan.TotalDays);
            //else
            //if (stepSpan.TotalHours >= 1.0d)
            //    chart.AxisX.Step = Math.Truncate(stepSpan.TotalHours) / OAHour;
            //else
            //if (stepSpan.TotalMinutes >= 1.0d)
            //    chart.AxisX.Step = Math.Truncate(stepSpan.TotalMinutes) / OAMinute;
            //else
            //    chart.AxisX.Step = Math.Truncate(stepSpan.TotalSeconds) / OASecond;

            //chart.AxisX.Min = first;
            //chart.AxisX.Max = last;
        }

        public static void SetAxisXTimeScale(ChartFX.WinForms.Chart chart, ServerOverview lastSnapshot, int steps, double fail)
        {
            DateTime last_x_value;
            if (lastSnapshot == null || lastSnapshot.TimeStamp == null)
            {
                DataTable tableSource = chart.DataSource as DataTable;
                if (tableSource == null)
                {
                    DataView viewSource = chart.DataSource as DataView;
                    if (viewSource != null)
                        tableSource = viewSource.Table;
                }

                if (tableSource != null)
                {
                    DataRow lastRow = null;
                    if (tableSource.Rows.Count > 0)
                        lastRow = tableSource.Rows[tableSource.Rows.Count - 1];

                    if (lastRow != null)
                    {
                        object column1 = lastRow[0];
                        if (column1 is DateTime)
                            last_x_value = (DateTime)column1;
                        else
                            last_x_value = DateTime.Now;
                    }
                    else
                        last_x_value = DateTime.Now;
                }
                else
                {
                    last_x_value = DateTime.Now;
                }
            }
            else
                last_x_value = lastSnapshot.TimeStamp.Value.ToLocalTime();

            SetAxisXTimeScale(chart, last_x_value, steps);
        }

        public static void SetAxisXTimeScale(ChartFX.WinForms.Chart chart, DateTime last_x_value, int steps)
        {
            AxisX xaxis = chart.AxisX;
            if (!xaxis.LabelsFormat.IsDateTime)
            {
                xaxis.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                xaxis.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            }

            xaxis.Min = (last_x_value - TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)).ToOADate();
            xaxis.Max = last_x_value.ToOADate();
            xaxis.Step = (xaxis.Max - xaxis.Min) / ((steps <= 2) ? 2 : steps);
        }
    }
}
