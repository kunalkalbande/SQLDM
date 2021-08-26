using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.UI.Dialogs;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class MemoryControl : DashboardControl
    {
        private enum memoryChartType
        {
            [Description("SQL Server")]
            SqlServer,
            [Description("Virtual Machine")]
            VM,
            [Description("VM Host")]
            Host
        }

        private const string OPTIONTYPE_CHARTTYPE = @"ChartType";

        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;

        private memoryChartType selectedType = memoryChartType.SqlServer;
        private bool showVM = false;

        public MemoryControl() : base(DashboardPanel.Memory)
        {
            InitializeComponent();

            usageChart.SetContextMenu(toolbarsManager);
            pagingChart.SetContextMenu(toolbarsManager);
            pagingChartHost.SetContextMenu(toolbarsManager);
            pagingChartVM.SetContextMenu(toolbarsManager);
            VmMemUsageChart.SetContextMenu(toolbarsManager);
            EsxMemUsageChart.SetContextMenu(toolbarsManager);
            VmAvaiableByteHyperVChart.SetContextMenu(toolbarsManager);
            HostAvaialbleByteHyperVChart.SetContextMenu(toolbarsManager);

            headerSelectTypeDropDownButton.DropDownItems.Clear();

            foreach (memoryChartType type in Enum.GetValues(typeof(memoryChartType)))
            {
                ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(type), null, chartTypeSelectToolStripMenuItem_Click);
                chartTypeItem.Tag = type;
                headerSelectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
            }

            helpTopic = HelpTopics.ServerDashboardViewMemoryPanel;
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            else
            {
                this.pagingPanel.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
                this.usagePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            }
        }
        private void ScaleControlsAsPerResolution()
        {
            this.pagingPanel.Padding = new System.Windows.Forms.Padding(10,0,10,10);
            this.usagePanel.Padding = new System.Windows.Forms.Padding(0,10,0,10);
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            headerSelectTypeDropDownButton.Text = ApplicationHelper.GetEnumDescription(showVM ? selectedType : memoryChartType.SqlServer);

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                this.tableLayoutPanel1.Controls.Clear();
                this.tableLayoutPanel1.Controls.Add(this.usagePanel, 0, 0);
                this.tableLayoutPanel1.RowCount = 1;
                this.tableLayoutPanel1.ColumnCount = 1;
                this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
                this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            }
                InitializeCharts();
            ConfigureDataSource();
        }

        private void InitializeCharts()
        {
            InitializeUsageChart();
            InitializePagingChart();
            InitializePagingChartVM();
            InitializePagingChartHost();
            InitializeVMMemUsageChart();
            InitializeEsxMemUsageChart();
            InitializeVmAvaiableByteHyperVChart();
            InitializeHostAvaialbleByteHyperVChart();
            ConfigureVMDropDownSelector();
            InitalizeDrilldown(usageChart, pagingChart, pagingChartVM, pagingChartHost,
            VmMemUsageChart, EsxMemUsageChart, VmAvaiableByteHyperVChart, HostAvaialbleByteHyperVChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ConfigureVMDropDownSelector()
        {
            if (historyData != null)
            {
                setDropDownSelection(selectedType);
            }

        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == OPTIONTYPE_CHARTTYPE)
                {
                    foreach (string value in option.Values)
                    {
                        memoryChartType chartType;
                        if (memoryChartType.TryParse(value, true, out chartType))
                        {
                            selectedType = chartType;
                            UpdateConfigOptions();
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = new List<string> { Enum.GetName(typeof(memoryChartType), selectedType) };
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_CHARTTYPE, options) });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                setDropDownSelection((memoryChartType)item.Tag);
                UpdateConfigOptions();
            }
        }

        private void setDropDownSelection(memoryChartType item)
        {
            selectedType = item;
            headerSelectTypeDropDownButton.Text = ApplicationHelper.GetEnumDescription(selectedType);
            MonitoredSqlServerStatus status =
                                   MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
            string serverType = "Unknown";
            Boolean pagingPanelVisibility = false;
            int colSpan = 2;
            if (status != null && status.Instance.Instance.IsVirtualized)
            {
                serverType = status.Instance.Instance.VirtualizationConfiguration.VCServerType;
                pagingPanelVisibility = serverType.Equals("HyperV") ? true : false;
                colSpan = serverType.Equals("HyperV") ? 1 : 2;
            }
            if (item == memoryChartType.VM)
            {
                usageChart.Visible =
                    pagingChart.Visible =
                    EsxMemUsageChart.Visible = 
                    pagingChartHost.Visible = false;
                pagingChartVM.Visible = pagingPanelVisibility;
                pagingPanel.Visible = pagingPanelVisibility;
                tableLayoutPanel1.SetColumnSpan(usagePanel, colSpan);
                HostAvaialbleByteHyperVChart.Visible = false;
                VmAvaiableByteHyperVChart.Visible = pagingPanelVisibility;
                VmMemUsageChart.Visible = !pagingPanelVisibility;
            }
            else if (item == memoryChartType.Host)
            {
                usageChart.Visible =
                    pagingChart.Visible =
                    VmMemUsageChart.Visible =
                    pagingChartVM.Visible = false;
                pagingChartHost.Visible = pagingPanelVisibility;
                pagingPanel.Visible = pagingPanelVisibility;
                tableLayoutPanel1.SetColumnSpan(usagePanel, colSpan);
                EsxMemUsageChart.Visible = !pagingPanelVisibility;
                VmAvaiableByteHyperVChart.Visible = false;
                HostAvaialbleByteHyperVChart.Visible = pagingPanelVisibility;
            }
            else
            {
                usageChart.Visible =
                    pagingChart.Visible = true;
                pagingPanel.Visible = true;
                tableLayoutPanel1.SetColumnSpan(usagePanel, 1);
                VmMemUsageChart.Visible =
                    EsxMemUsageChart.Visible =
                    pagingChartHost.Visible = 
                    pagingChartVM.Visible= false;
            }
        }

        private void InitializePagingChart()
        {
            pagingChart.Printer.Orientation = PageOrientation.Landscape;
            pagingChart.Printer.Compress = true;
            pagingChart.Printer.ForceColors = true;
            pagingChart.Printer.Document.DocumentName = "Paging Chart";
            pagingChart.ToolTipFormat = "%s\n%v pages/second\n%x";
            pagingChart.AxisY.DataFormat.Decimals = 2;

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var pagesPerSecFieldMap = new FieldMap("PagesPerSec", FieldUsage.Value);
            pagesPerSecFieldMap.DisplayName = @"Pages";

            pagingChart.DataSourceSettings.Fields.Clear();
            pagingChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   pagesPerSecFieldMap
                                                               });

            pagingChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pagingChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pagingChart.Gallery = Gallery.Area;
            foreach (SeriesAttributes series in pagingChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }
            pagingChart.LegendBox.PlotAreaOnly = false;
        }

        private void InitializePagingChartVM()
        {
            pagingChartVM.Printer.Orientation = PageOrientation.Landscape;
            pagingChartVM.Printer.Compress = true;
            pagingChartVM.Printer.ForceColors = true;
            pagingChartVM.Printer.Document.DocumentName = "Paging Chart VM";
            pagingChartVM.ToolTipFormat = "%s\n%v pages/second\n%x";
            pagingChartVM.AxisY.DataFormat.Decimals = 2;

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var pagesPerSecFieldMapVM = new FieldMap("PagePerSecVM", FieldUsage.Value);
            pagesPerSecFieldMapVM.DisplayName = @"Pages";

            pagingChartVM.DataSourceSettings.Fields.Clear();
            pagingChartVM.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   pagesPerSecFieldMapVM
                                                               });

            pagingChartVM.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pagingChartVM.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pagingChartVM.Gallery = Gallery.Area;
            foreach (SeriesAttributes series in pagingChartVM.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }
            pagingChartVM.LegendBox.PlotAreaOnly = false;
        }


        private void InitializePagingChartHost()
        {
            pagingChartHost.Printer.Orientation = PageOrientation.Landscape;
            pagingChartHost.Printer.Compress = true;
            pagingChartHost.Printer.ForceColors = true;
            pagingChartHost.Printer.Document.DocumentName = "Paging Chart Host";
            pagingChartHost.ToolTipFormat = "%s\n%v pages/second\n%x";
            pagingChartHost.AxisY.DataFormat.Decimals = 2;

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var pagesPerSecFieldMapHost = new FieldMap("PagePerSecHost", FieldUsage.Value);
            pagesPerSecFieldMapHost.DisplayName = @"Pages";

            pagingChartHost.DataSourceSettings.Fields.Clear();
            pagingChartHost.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   pagesPerSecFieldMapHost
                                                               });

            pagingChartHost.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pagingChartHost.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pagingChartHost.Gallery = Gallery.Area;
            foreach (SeriesAttributes series in pagingChartHost.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }
            pagingChartHost.LegendBox.PlotAreaOnly = false;
        }

        private void InitializeUsageChart()
        {
            usageChart.Printer.Orientation = PageOrientation.Landscape;
            usageChart.Printer.Compress = true;
            usageChart.Printer.ForceColors = true;
            usageChart.Printer.Document.DocumentName = "Server Memory Usage Chart";
            usageChart.ToolBar.RemoveAt(0);
            usageChart.ToolTipFormat = "%s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var sqlServerMemoryUsedFieldMap = new FieldMap("SqlServerMemoryUsed", FieldUsage.Value);
            sqlServerMemoryUsedFieldMap.DisplayName = "SQL Used";
            var sqlServerMemoryAllocatedFieldMap = new FieldMap("SqlServerMemoryAllocated", FieldUsage.Value);
            sqlServerMemoryAllocatedFieldMap.DisplayName = "SQL Allocated";
            var totalMemoryUsedFieldMap = new FieldMap("OsMemoryUsed", FieldUsage.Value);
            totalMemoryUsedFieldMap.DisplayName = "Total Used";

            usageChart.DataSourceSettings.Fields.Clear();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                usageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                    });
            }
            else
            {
                usageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                        totalMemoryUsedFieldMap
                                                                    });
            }

            foreach (SeriesAttributes series in usageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            usageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            usageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeVMMemUsageChart()
        {
            VmMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            VmMemUsageChart.Printer.Compress = true;
            VmMemUsageChart.Printer.ForceColors = true;
            VmMemUsageChart.Printer.Document.DocumentName = "VM Memory Usage Chart";
            VmMemUsageChart.ToolBar.RemoveAt(0);
            VmMemUsageChart.ToolTipFormat = "VM %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var vmMemActiveFieldMap = new FieldMap("vmMemActive", FieldUsage.Value);
            vmMemActiveFieldMap.DisplayName = "Active";
            var vmMemConsumedFieldMap = new FieldMap("vmMemConsumed", FieldUsage.Value);
            vmMemConsumedFieldMap.DisplayName = "Consumed";
            var vmMemGrantedFieldMap = new FieldMap("vmMemGranted", FieldUsage.Value);
            vmMemGrantedFieldMap.DisplayName = "Granted";
            var vmMemBalloonedFieldMap = new FieldMap("vmMemBallooned", FieldUsage.Value);
            vmMemBalloonedFieldMap.DisplayName = "Ballooned";
            var vmMemSwappedFieldMap = new FieldMap("vmMemSwapped", FieldUsage.Value);
            vmMemSwappedFieldMap.DisplayName = "Swapped";

            VmMemUsageChart.DataSourceSettings.Fields.Clear();
            VmMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmMemActiveFieldMap,
                                                                            vmMemConsumedFieldMap,
                                                                            vmMemGrantedFieldMap,
                                                                            vmMemBalloonedFieldMap,
                                                                            vmMemSwappedFieldMap
                                                                        });

            foreach (SeriesAttributes series in VmMemUsageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            VmMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            VmMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeEsxMemUsageChart()
        {
            EsxMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            EsxMemUsageChart.Printer.Compress = true;
            EsxMemUsageChart.Printer.ForceColors = true;
            EsxMemUsageChart.Printer.Document.DocumentName = "VM Memory Usage Chart";
            EsxMemUsageChart.ToolBar.RemoveAt(0);
            EsxMemUsageChart.ToolTipFormat = "Host %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var esxMemActiveFieldMap = new FieldMap("esxMemActive", FieldUsage.Value);
            esxMemActiveFieldMap.DisplayName = "Active";
            var esxMemConsumedFieldMap = new FieldMap("esxMemConsumed", FieldUsage.Value);
            esxMemConsumedFieldMap.DisplayName = "Consumed";
            var esxMemGrantedFieldMap = new FieldMap("esxMemGranted", FieldUsage.Value);
            esxMemGrantedFieldMap.DisplayName = "Granted";
            var esxMemBalloonedFieldMap = new FieldMap("esxMemBallooned", FieldUsage.Value);
            esxMemBalloonedFieldMap.DisplayName = "Ballooned";
            var esxMemSwappedFieldMap = new FieldMap("esxMemSwapped", FieldUsage.Value);
            esxMemSwappedFieldMap.DisplayName = "Swapped";

            EsxMemUsageChart.DataSourceSettings.Fields.Clear();
            EsxMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            esxMemActiveFieldMap,
                                                                            esxMemConsumedFieldMap,
                                                                            esxMemGrantedFieldMap,
                                                                            esxMemBalloonedFieldMap,
                                                                            esxMemSwappedFieldMap
                                                                        });
            foreach (SeriesAttributes series in EsxMemUsageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            EsxMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            EsxMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
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

        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                var serverOverview = historyData.CurrentServerOverviewSnapshot;
                if (serverOverview != null)
                {
                    if (serverOverview.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        usageChart.AxisY.Min = 0;
                        usageChart.AxisY.Max =
                            Convert.ToDouble(serverOverview.OSMetricsStatistics.TotalPhysicalMemory.Megabytes);
                    }

                    if (serverOverview.VMConfig != null)
                    {
                        VmMemUsageChart.AxisY.Min = 0;
                        VmMemUsageChart.AxisY.Max = Convert.ToDouble(serverOverview.VMConfig.MemSize.Megabytes);
                        EsxMemUsageChart.AxisY.Min = 0;
                        EsxMemUsageChart.AxisY.Max = Convert.ToDouble(serverOverview.VMConfig.ESXHost.MemSize.Megabytes);
                        VmAvaiableByteHyperVChart.AxisY.Min = 0;
                        double avaiableByte = serverOverview.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes != null ? (double) serverOverview.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes : 2000D;
                        double avaiableByteHost = serverOverview.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes != null ? (double)serverOverview.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes : 2000D;
                        VmAvaiableByteHyperVChart.AxisY.Max = Convert.ToDouble((avaiableByte * 100) / (100 - serverOverview.VMConfig.PerfStats.MemUsage));
                        HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                        //SQLdm 10.0 (Srishti Purohit)
                        //SQLDM-25432 -- Getting frequent UnhandledExceptions on screen after adding HyperV instance
                        HostAvaialbleByteHyperVChart.AxisY.Max = Convert.ToDouble((avaiableByteHost * 100) / (100 - serverOverview.VMConfig.ESXHost.PerfStats.MemUsage));
                    }
                }

                // update the CPU Usage chart
                pagingChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(pagingChart, 2);

                //update the Vm paging chart
                pagingChartVM.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(pagingChartVM, 2);

                // update the Host paging chart
                pagingChartHost.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(pagingChartHost, 2);

                // update the Call Rates chart
                usageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(usageChart, 2);

                // update Vm Mem Usage Chart
                VmMemUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(VmMemUsageChart, 2);

                // update Host Mem Usage Chart
                EsxMemUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(EsxMemUsageChart, 2);

                // update Vm available Byte Chart
                VmAvaiableByteHyperVChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(VmAvaiableByteHyperVChart, 2);

                // update Host available Byte Chart
                HostAvaialbleByteHyperVChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(HostAvaialbleByteHyperVChart, 2);

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.OSMetricsStatistics.AvailableBytes != null &&
                    activeSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.HasValue &&
                    activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory != null &&
                    activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue &&
                    activeSnapshot.TotalServerMemory != null &&
                    activeSnapshot.TotalServerMemory.Megabytes.HasValue)
                {
                    decimal totalUsedMemory = activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.Value -
                                              activeSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.Value;
                    decimal percentTotalMemoryUsed =
                        (totalUsedMemory / activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.Value) * 100;
                    alerts.Add(new Pair<Metric, IComparable>(Metric.OSMemoryUsagePct, percentTotalMemoryUsed));

                    decimal percentSqlAllocatedMemoryUsed = (activeSnapshot.TotalServerMemory.Megabytes.Value /
                         activeSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.Value) * 100;
                    alerts.Add(new Pair<Metric, IComparable>(Metric.SQLMemoryUsagePct, percentSqlAllocatedMemoryUsed));
                }

                if (activeSnapshot.OSMetricsStatistics.PagesPersec.HasValue &&
                    activeSnapshot.OSMetricsStatistics.PagesPersec.Value > 0)
                {
                    alerts.Add(new Pair<Metric, IComparable>(Metric.OSMemoryPagesPerSecond, activeSnapshot.OSMetricsStatistics.PagesPersec.Value));
                }
            }

            SetAlertStatus(alerts);
        }

        public override void ShowVMData(bool show)
        {
            showVM = show;

            if (showVM)
            {
                SetOptions(DashboardPanelConfiguration.Options);
            }
            else
            {
                selectedType = memoryChartType.SqlServer;
            }

            ConfigureVMDropDownSelector();

            headerSelectTypeSeparator.Visible =
                headerSelectTypeDropDownButton.Visible = showVM;
        }

        #region toolbar

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (designMode)
            {
                e.Cancel = true;
                return;
            }

            if (e.SourceControl is Chart)
            {
                contextControl = e.SourceControl;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "configureAlertsButton":
                    ConfigureControlAlerts(contextControl);
                    break;
                case "showHelpButton":
                    ShowControlHelp(contextControl);
                    break;
                case "showDetailsButton":
                    ShowControlDetails(contextControl);
                    break;
                case "printChartButton":
                    PrintChart(contextControl);
                    break;
                case "exportChartDataButton":
                    SaveChartData(contextControl);
                    break;
                case "exportChartImageButton":
                    SaveChartImage(contextControl);
                    break;
            }
            contextControl = null;
        }

        #endregion

        #region Chart Actions

        private void ConfigureControlAlerts(Control targetControl)
        {
            if (targetControl != null)
            {
                try
                {
                    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);

                    if (targetControl == usageChart)
                    {
                        dialog.Select(Metric.SQLMemoryUsagePct);
                    }
                    else if (targetControl == pagingChart)
                    {
                        dialog.Select(Metric.OSMemoryPagesPerSecond);
                    }
                    else if (targetControl == VmMemUsageChart)
                    {
                        dialog.Select(Metric.VmMemoryUtilization);
                    }
                    else if (targetControl == EsxMemUsageChart)
                    {
                        dialog.Select(Metric.VmESXMemoryUsage);
                    }

                    dialog.ShowDialog(ParentForm);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                    ex);
                }
            }
        }

        private void ShowControlHelp(Control targetControl)
        {
            string topic = HelpTopics.ServerDashboardViewMemoryPanel;

            if (targetControl != null)
            {
                if (targetControl == usageChart)
                {
                    topic = HelpTopics.ServerDashboardViewMemoryPanelUsage;
                }
                else if (targetControl == pagingChart)
                {
                    topic = HelpTopics.ServerDashboardViewMemoryPanelPaging;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesMemory);
            }
        }

        private void PrintChart(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
            }
        }

        #endregion

        #region Chart Click Events

        private void chart_MouseClick(object sender, HitTestEventArgs e)
        {
            if (designMode)
            {
                DashboardControl_MouseClick(sender, e);
                return;
            }

            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ShowControlDetails((Chart)sender);
            }
        }

        #endregion
    }
}
