using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ChartFX.WinForms;
using ChartFX.WinForms.Gauge;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class NetworkControl : DashboardControl
    {
        private enum networkChartType
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

        private networkChartType selectedType = networkChartType.SqlServer;
        private bool showVM = false;

        public NetworkControl() : base(DashboardPanel.Network)
        {
            InitializeComponent();

            throughputChart.SetContextMenu(toolbarsManager);
            vmNetUsageChart.SetContextMenu(toolbarsManager);
            hostNetUsageChart.SetContextMenu(toolbarsManager);

            headerSelectTypeDropDownButton.DropDownItems.Clear();

            foreach (networkChartType type in Enum.GetValues(typeof(networkChartType)))
            {
                ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(type), null, chartTypeSelectToolStripMenuItem_Click);
                chartTypeItem.Tag = type;
                headerSelectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
            }

            helpTopic = HelpTopics.ServerDashboardViewNetworkPanel;
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            headerSelectTypeDropDownButton.Text = ApplicationHelper.GetEnumDescription(showVM ? selectedType : networkChartType.SqlServer);

            InitializeCharts();
            ConfigureDataSource();
        }

        private void InitializeCharts()
        {
            InitializeThroughputChart();
            InitializeVMNetUsageChart();
            InitializeHostNetUsageChart();
            InitializeResponseTimeGauge();

            ConfigureVMDropDownSelector();
            InitalizeDrilldown(throughputChart, vmNetUsageChart, hostNetUsageChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
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
                        networkChartType chartType;
                        if (networkChartType.TryParse(value, true, out chartType))
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
            List<string> options = new List<string> { Enum.GetName(typeof(networkChartType), selectedType) };
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_CHARTTYPE, options) });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                setDropDownSelection((networkChartType)item.Tag);
                UpdateConfigOptions();
            }
        }

        private void setDropDownSelection(networkChartType item)
        {
            selectedType = item;
            headerSelectTypeDropDownButton.Text = ApplicationHelper.GetEnumDescription(selectedType);

            if (item == networkChartType.VM)
            {
                throughputChart.Visible =
                    hostNetUsageChart.Visible = false;
                vmNetUsageChart.Visible = true;
            }
            else if (item == networkChartType.Host)
            {
                throughputChart.Visible =
                    vmNetUsageChart.Visible = false;
                hostNetUsageChart.Visible = true;
            }
            else
            {
                throughputChart.Visible = true;
                vmNetUsageChart.Visible =
                    hostNetUsageChart.Visible = false;
            }
        }

        private void InitializeThroughputChart()
        {
            throughputChart.Printer.Compress = true;
            throughputChart.Printer.ForceColors = true;
            throughputChart.ToolTipFormat = "%s\n%v per second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap PacketsSentFieldMap = new FieldMap("PacketsSentPerSecond", FieldUsage.Value);
            PacketsSentFieldMap.DisplayName = "Packets Sent";
            FieldMap PacketsReceivedFieldMap = new FieldMap("PacketsReceivedPerSecond", FieldUsage.Value);
            PacketsReceivedFieldMap.DisplayName = "Packets Received";

            throughputChart.DataSourceSettings.Fields.Clear();
            throughputChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     PacketsSentFieldMap,
                                                                     PacketsReceivedFieldMap
                                                                 });

            throughputChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            throughputChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeVMNetUsageChart()
        {
            vmNetUsageChart.Printer.Compress = true;
            vmNetUsageChart.Printer.ForceColors = true;
            vmNetUsageChart.ToolTipFormat = "%s\n%v per second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap vmTransmittedFieldMap = new FieldMap("vmNetTransmitted", FieldUsage.Value);
            vmTransmittedFieldMap.DisplayName = "Transmitted";
            FieldMap vmReceivedFieldMap = new FieldMap("vmNetReceived", FieldUsage.Value);
            vmReceivedFieldMap.DisplayName = "Received";

            vmNetUsageChart.DataSourceSettings.Fields.Clear();
            vmNetUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     vmTransmittedFieldMap,
                                                                     vmReceivedFieldMap
                                                                 });
            vmNetUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmNetUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;

        }

        private void InitializeHostNetUsageChart()
        {
            hostNetUsageChart.Printer.Compress = true;
            hostNetUsageChart.Printer.ForceColors = true;
            hostNetUsageChart.ToolTipFormat = "%s\n%v per second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap hostTransmittedFieldMap = new FieldMap("esxNetTransmitted", FieldUsage.Value);
            hostTransmittedFieldMap.DisplayName = "Transmitted";
            FieldMap hostReceivedFieldMap = new FieldMap("esxNetReceived", FieldUsage.Value);
            hostReceivedFieldMap.DisplayName = "Received";

            hostNetUsageChart.DataSourceSettings.Fields.Clear();
            hostNetUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     hostTransmittedFieldMap,
                                                                     hostReceivedFieldMap
                                                                 });
            hostNetUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            hostNetUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeResponseTimeGauge()
        {
            responseTimeGauge.ToolTip =
                responseTimeGauge.MainIndicator.ToolTip = "%v ms Response Time";
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                throughputChart.DataSource = historyData.CurrentSnapshotsDataTable;
                vmNetUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                hostNetUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(throughputChart, 2);
                ChartFxExtensions.SetAxisXTimeScale(vmNetUsageChart, 2);
                ChartFxExtensions.SetAxisXTimeScale(hostNetUsageChart, 2);

                // update the Response time gauge
                double value;
                if (historyData.CurrentServerOverviewSnapshot != null)
                {
                    value = historyData.CurrentServerOverviewSnapshot.ResponseTime;
                }
                else
                {
                    value = 0;
                }

                responseTimeGauge.MainValue = value;

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.ResponseTime > 0)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.ServerResponseTime, activeSnapshot.ResponseTime));
            }

            SetAlertStatus(alerts);
        }

        private void UpdateResponseTimeDisplay()
        {
            double value = (double)responseTimeGauge.MainValue;
            decimal gaugeMax = Math.Max(0, Convert.ToDecimal(value));
            // set the scale to be min of 1000 and rounded to the nearest 1000
            gaugeMax = Math.Max(Math.Round((gaugeMax * 1.2m) / 1000, 1) * 1000, 1000);
            responseTimeGauge.MainScale.Max = Convert.ToDouble(gaugeMax);
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
                selectedType = networkChartType.SqlServer;
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
            else
            {
                contextControl = responseTimeGauge;
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

                    if (targetControl == throughputChart)
                    {
                        dialog.Select(Metric.ServerResponseTime);
                    }
                    else if (targetControl == responseTimeGauge)
                    {
                        dialog.Select(Metric.ServerResponseTime);
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
                if (targetControl == throughputChart)
                {
                    topic = HelpTopics.ServerDashboardViewNetworkPanelThroughput;
                }
                else if (targetControl == responseTimeGauge)
                {
                    topic = HelpTopics.ServerDashboardViewNetworkPanelResponseTime;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                if (targetControl == responseTimeGauge)
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.Sessions);
                }
                else
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.OverviewDetails);
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
            if (designMode)
            {
                DashboardControl_MouseClick(sender, e);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                ShowControlDetails((Gauge)sender);
            }
        }

        #endregion

        internal override void OnServerAlertConfigurationChanged(IEnumerable<Common.Thresholds.MetricThresholdEntry> thresholdEntries)
        {
            UpdateResponseTimeDisplay();
        }
    }
}
