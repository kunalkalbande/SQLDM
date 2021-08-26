using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using ChartFX.WinForms.Gauge;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.Common.Snapshots;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class VirtualizationControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private int selectedType = -1;
        private const string OPTIONTYPE_CHARTTYPE = @"ChartType";

        private enum vmChartType
        {
            [Description("Virtual Machine")]
            Vm,
            [Description("VM Host")]
            Host
        }

        public VirtualizationControl()
            : base(DashboardPanel.VM)
        {
            InitializeComponent();

            helpTopic = HelpTopics.ServerDashboardViewVMPanel;


            updateLinearScaleFontAsPerTheme(this.linearScale1);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            InitializeCharts();
            ConfigureDropDownSelector();
            ConfigureDataSource();

            CheckVmConfig();
        }

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            vmMemUsageChart.SetContextMenu(toolbarsManager);
            vmDiskUsageChart.SetContextMenu(toolbarsManager);
            esxMemUsageChart.SetContextMenu(toolbarsManager);
            esxDiskUsageChart.SetContextMenu(toolbarsManager);
            VmAvaiableByteHyperVChart.SetContextMenu(toolbarsManager);
            HostAvaialbleByteHyperVChart.SetContextMenu(toolbarsManager);

            InitializeVmMemUsageChart();
            InitializeEsxMemUsageChart();
            InitializeCpuReadyGauge();
            InitializeVmDiskUsageChart();
            InitializeEsxDiskUsageChart();
            InitializeVmAvaiableByteHyperVChart();
            InitializeHostAvaialbleByteHyperVChart();
            InitalizeDrilldown(vmMemUsageChart, vmDiskUsageChart, esxMemUsageChart, esxDiskUsageChart,
                VmAvaiableByteHyperVChart, HostAvaialbleByteHyperVChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeVmMemUsageChart()
        {
            vmMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            vmMemUsageChart.Printer.Compress = true;
            vmMemUsageChart.Printer.ForceColors = true;
            vmMemUsageChart.Printer.Document.DocumentName = "VM Memory Usage Chart";
            vmMemUsageChart.ToolTipFormat = "VM %s\n%v MB\n%x";

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

            vmMemUsageChart.DataSourceSettings.Fields.Clear();
            vmMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmMemActiveFieldMap,
                                                                            vmMemConsumedFieldMap,
                                                                            vmMemGrantedFieldMap,
                                                                            vmMemBalloonedFieldMap,
                                                                            vmMemSwappedFieldMap
                                                                        });

            foreach (SeriesAttributes series in vmMemUsageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            vmMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeEsxMemUsageChart()
        {
            esxMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            esxMemUsageChart.Printer.Compress = true;
            esxMemUsageChart.Printer.ForceColors = true;
            esxMemUsageChart.Printer.Document.DocumentName = "Host Memory Usage Chart";
            esxMemUsageChart.ToolTipFormat = "Host %s\n%v MB\n%x";

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

            esxMemUsageChart.DataSourceSettings.Fields.Clear();
            esxMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            esxMemActiveFieldMap,
                                                                            esxMemConsumedFieldMap,
                                                                            esxMemGrantedFieldMap,
                                                                            esxMemBalloonedFieldMap,
                                                                            esxMemSwappedFieldMap
                                                                        });
            foreach (SeriesAttributes series in esxMemUsageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            esxMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            esxMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
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
        }


        private void InitializeCpuReadyGauge()
        {
            cpuReadyGauge.ToolTip =
                cpuReadyGauge.MainIndicator.ToolTip = "%v ms";
        }

        private void InitializeVmDiskUsageChart()
        {
            vmDiskUsageChart.Printer.Compress = true;
            vmDiskUsageChart.Printer.ForceColors = true;
            vmDiskUsageChart.Printer.Document.DocumentName = "VM Disk Usage Chart";
            vmDiskUsageChart.ToolTipFormat = "VM %s\n%v KB/s\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var vmDiskWriteFieldMap = new FieldMap("vmDiskWrite", FieldUsage.Value);
            vmDiskWriteFieldMap.DisplayName = "Disk Write";
            var vmDiskReadFieldMap = new FieldMap("vmDiskRead", FieldUsage.Value);
            vmDiskReadFieldMap.DisplayName = "Disk Read";

            vmDiskUsageChart.DataSourceSettings.Fields.Clear();
            vmDiskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmDiskWriteFieldMap,
                                                                            vmDiskReadFieldMap
                                                                        });

            vmDiskUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmDiskUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeEsxDiskUsageChart()
        {
            esxDiskUsageChart.Printer.Compress = true;
            esxDiskUsageChart.Printer.ForceColors = true;
            esxDiskUsageChart.Printer.Document.DocumentName = "Host Disk Usage Chart";
            esxDiskUsageChart.ToolTipFormat = "Host %s\n%v KB/s\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var esxDiskWriteFieldMap = new FieldMap("esxDiskWrite", FieldUsage.Value);
            esxDiskWriteFieldMap.DisplayName = "Disk Write";
            var esxDiskReadFieldMap = new FieldMap("esxDiskRead", FieldUsage.Value);
            esxDiskReadFieldMap.DisplayName = "Disk Read";

            esxDiskUsageChart.DataSourceSettings.Fields.Clear();
            esxDiskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            esxDiskWriteFieldMap,
                                                                            esxDiskReadFieldMap
                                                                        });

            esxDiskUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            esxDiskUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void ConfigureDropDownSelector()
        {
            headerSelectTypeDropDownButton.DropDownItems.Clear();

            foreach (vmChartType type in Enum.GetValues(typeof(vmChartType)))
            {
                ToolStripMenuItem vmMenuItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(type), null, chartTypeSelectToolStripMenuItem_Click);
                vmMenuItem.Tag = (int) type;
                headerSelectTypeDropDownButton.DropDownItems.Add(vmMenuItem);
            }

            ToolStripMenuItem selectedItem = null;
            if (selectedType < 0)
            {
                selectedItem = (ToolStripMenuItem)headerSelectTypeDropDownButton.DropDownItems[0];
                selectedType = (int)selectedItem.Tag;
            }
            else
            {
                foreach (ToolStripMenuItem item in headerSelectTypeDropDownButton.DropDownItems)
                {
                    if (selectedType == (int)item.Tag)
                    {
                        selectedItem = item;
                        break;
                    }
                }
            }
            setDropDownSelection(selectedItem);

            if (Enum.IsDefined(typeof(vmChartType), selectedType))
            {
                headerSelectTypeDropDownButton.Text = headerSelectTypeDropDownButton.DropDownItems[selectedType].Text;
            }

            headerSelectTypeSeparator.Visible =
                headerSelectTypeDropDownButton.Visible = true;

        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item.Tag != null && item.Tag is int)
                {
                    selectedType = (int) item.Tag;
                    setDropDownSelection(item);
                }
            }
        }   

        private void setDropDownSelection(ToolStripMenuItem item)
        {
            headerSelectTypeDropDownButton.Text = item.Text.Length > 60
                                                      ? string.Format("{0}...", item.Text.Substring(0, 60))
                                                      : item.Text;
            headerSelectTypeDropDownButton.ToolTipText = item.Text;
            headerSelectTypeDropDownButton.Tag = item.Text;

            //Hyper V
            MonitoredSqlServerStatus status = MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
            string serverType = "Unknown";
            Boolean hyperVisibility = false;

            if (status != null && status.Instance.Instance.IsVirtualized)
            {
                serverType = status.Instance.Instance.VirtualizationConfiguration.VCServerType;
                hyperVisibility = serverType.Equals("HyperV") ? true : false;
            }

            switch ((vmChartType)item.Tag)
            {
                case vmChartType.Vm:
                    esxMemUsageChart.Visible = 
                        esxDiskUsageChart.Visible =
                        HostAvaialbleByteHyperVChart.Visible = false;
                    vmMemUsageChart.Visible = !hyperVisibility;
                    VmAvaiableByteHyperVChart.Visible = hyperVisibility;
                    vmDiskUsageChart.Visible = true;
                    break;
                case vmChartType.Host:
                    vmMemUsageChart.Visible =
                        vmDiskUsageChart.Visible = 
                        VmAvaiableByteHyperVChart.Visible = false;
                    esxMemUsageChart.Visible = !hyperVisibility;
                    HostAvaialbleByteHyperVChart.Visible = hyperVisibility;
                    esxDiskUsageChart.Visible = true;
                    break;
            }
            UpdateConfigOptions();
        }

        private void UpdateSeriesSettings()
        {
            foreach (SeriesAttributes series in vmDiskUsageChart.Series)
            {
                series.Gallery = Gallery.Lines;
            }

            foreach (SeriesAttributes series in esxDiskUsageChart.Series)
            {
                series.Gallery = Gallery.Lines;
            }

            foreach (SeriesAttributes series in vmMemUsageChart.Series)
            {
                series.Gallery = Gallery.Area;
            }

            foreach (SeriesAttributes series in esxMemUsageChart.Series)
            {
                series.Gallery = Gallery.Area;
            }

            vmMemUsageChart.SetAreaSeriesAlphaChannel(175, false);
            esxMemUsageChart.SetAreaSeriesAlphaChannel(175, false);
            VmAvaiableByteHyperVChart.SetAreaSeriesAlphaChannel(175, false);
            HostAvaialbleByteHyperVChart.SetAreaSeriesAlphaChannel(175, false);
        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == OPTIONTYPE_CHARTTYPE)
                {
                    foreach (string value in option.Values)
                    {
                        vmChartType chartType;
                        if (!vmChartType.TryParse(value, true, out chartType)) continue;
                        selectedType = (int)chartType;
                        UpdateConfigOptions();
                        break;
                    }
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = new List<string> { Enum.GetName(typeof(vmChartType), selectedType) };
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_CHARTTYPE, options) });
        }

        public override void ApplySettings()
        {

        }

        public override void SaveSettings()
        {

        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                // update the Mem Usage chart
                vmMemUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(vmMemUsageChart, 2);
                esxMemUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(esxMemUsageChart,2);
                VmAvaiableByteHyperVChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(VmAvaiableByteHyperVChart, 2);
                HostAvaialbleByteHyperVChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(HostAvaialbleByteHyperVChart, 2);

                var serverOverview = historyData.CurrentServerOverviewSnapshot;
                if (serverOverview != null)
                {
                    
                    if (serverOverview.VMConfig != null)
                    {
                        VmAvaiableByteHyperVChart.AxisY.Min = 0;
                        double avaiableByte = serverOverview.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes != null ? (double)serverOverview.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes : 2000D;
                        double avaiableByteHost = serverOverview.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes != null ? (double)serverOverview.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes : 2000D;
                        VmAvaiableByteHyperVChart.AxisY.Max = Convert.ToDouble((avaiableByte * 100) / (100 - serverOverview.VMConfig.PerfStats.MemUsage));
                        HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                        //SQLdm 10.0 (Srishti Purohit)
                        //SQLDM-25619 -- Getting UnhandledException when I click on 'Virtualization' tab in Overview/Dashboard view
                        HostAvaialbleByteHyperVChart.AxisY.Max = Convert.ToDouble((avaiableByteHost * 100) / (100 - serverOverview.VMConfig.ESXHost.PerfStats.MemUsage));
                    }
                }

                // update the CPU Ready Length gauge and text
                double value;
                if (historyData.CurrentServerOverviewSnapshot != null &&
                    historyData.CurrentServerOverviewSnapshot.VMConfig != null)
                {
                    value = historyData.CurrentServerOverviewSnapshot.VMConfig.PerfStats.CpuReady;
                }
                else
                {
                    value = 0;
                }

                cpuReadyGauge.MainValue = value;
                cpuReadyGauge.Invalidate(true);

                // update the disk usage chart
                vmDiskUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(vmDiskUsageChart, 2);
                esxDiskUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(esxDiskUsageChart, 2);

                UpdateSeriesSettings();

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.VMConfig != null)
                {
                    if (activeSnapshot.VMConfig.PerfStats.CpuReady > 0)
                    {
                        alerts.Add(new Pair<Metric, IComparable>(Metric.VmCPUReadyWaitTime,
                                                                 activeSnapshot.VMConfig.PerfStats.CpuReady));
                    }
                    if (vmMemUsageChart.Visible)
                    {
                        if (activeSnapshot.VMConfig.PerfStats.MemBallooned.Kilobytes > 0)
                        {
                            alerts.Add(new Pair<Metric, IComparable>(Metric.VmReclaimedMemory,
                                                                     activeSnapshot.VMConfig.PerfStats.MemBallooned.
                                                                         Kilobytes));
                        }
                        if ((activeSnapshot.VMConfig.PerfStats.MemSwapInRate > 0) || 
                            (activeSnapshot.VMConfig.PerfStats.MemSwapOutRate > 0))
                        {
                            alerts.Add(new Pair<Metric, IComparable>(Metric.VmMemorySwapDelayDetected,
                                                                     true));
                        }
                    }
                    else
                    {
                        if ((activeSnapshot.VMConfig.ESXHost.PerfStats.MemSwapInRate > 0) ||
                            (activeSnapshot.VMConfig.ESXHost.PerfStats.MemSwapOutRate > 0))
                        {
                            alerts.Add(new Pair<Metric, IComparable>(Metric.VmESXMemorySwapDetected,true));
                        }
                    }
                }
            }

            SetAlertStatus(alerts);
        }

        private void CheckVmConfig()
        {
            if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.IsVirtualized)
                HideVMConfigPanel();
            else
                ShowVMConfigPanel();
        }

        private void ShowVMConfigPanel()
        {
            VmConfigPanel.Visible = true;
            VmConfigPanel.BringToFront();
            statusLinkLabel.Visible = true;
        }

        private void HideVMConfigPanel()
        {
            VmConfigPanel.SendToBack();
            VmConfigPanel.Visible = false;
            statusLinkLabel.Visible = false;
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
            else
            {
                contextControl = cpuReadyGauge;
            }

        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "vmConfig":
                    ConfigureVirtualization(contextControl);
                    break;
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
        private void ConfigureVirtualization(Control targetControl)
        {
            if (targetControl != null)
            {
                try
                {
                    using (VirtualizationConfig virtualizationConfig = new VirtualizationConfig())
                    {
                        virtualizationConfig.ShowDialog(this);
                    }
                }
                catch (Exception except)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Error occurred showing the Virtualization Configuration dialog",
                                                    except);
                    Log.Error("Error showing VM Config Dialog from context menu ", except);
                }
            }
        }


        private void ConfigureControlAlerts(Control targetControl)
        {
            if (targetControl != null)
            {
                try
                {
                    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);

                    if (targetControl == vmMemUsageChart || targetControl == esxMemUsageChart || targetControl == VmAvaiableByteHyperVChart || targetControl == HostAvaialbleByteHyperVChart)
                    {
                        dialog.Select(Metric.VmReclaimedMemory);
                    }
                    else if (targetControl == cpuReadyGauge)
                    {
                        dialog.Select(Metric.VmCPUReadyWaitTime);
                    }
                    else if (targetControl == vmDiskUsageChart || targetControl == esxDiskUsageChart)
                    {
                        dialog.Select(Metric.VmConfigChange);
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
            string topic = string.Empty;

            if (targetControl != null)
            {
                if (targetControl == vmMemUsageChart || targetControl == esxMemUsageChart || targetControl ==  VmAvaiableByteHyperVChart || targetControl == HostAvaialbleByteHyperVChart)
                {
                    topic = HelpTopics.ServerDashboardViewVMPanelMemory;
                }
                else if (targetControl == cpuReadyGauge)
                {
                    topic = HelpTopics.ServerDashboardViewVMPanelCpu;
                }
                else if (targetControl == vmDiskUsageChart || targetControl == esxDiskUsageChart)
                {
                    topic = HelpTopics.ServerDashboardViewVMPanelDisk;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                if (targetControl == vmMemUsageChart || targetControl == esxMemUsageChart || targetControl == VmAvaiableByteHyperVChart || targetControl == HostAvaialbleByteHyperVChart)
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesMemory);
                }
                else if (targetControl == cpuReadyGauge)
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesCpu);
                }
                else if (targetControl == vmDiskUsageChart || targetControl == esxDiskUsageChart)
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesDisk);
                }
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

        #region Chart & Gauge Click Events

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

        private void gauge_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowControlDetails((Gauge)sender);
            }
        }

        #endregion

        private void statusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (VirtualizationConfig vcd = new VirtualizationConfig())
            {
                vcd.ShowDialog(this);

                CheckVmConfig();
            }
        }


        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            updateLinearScaleFontAsPerTheme(this.linearScale1);
        }
        public void updateLinearScaleFontAsPerTheme(LinearScale linearscale)
        {
            ThemeSetter ts = new ThemeSetter();
            ts.SetLinearScale(linearscale);
        }


    }
}
