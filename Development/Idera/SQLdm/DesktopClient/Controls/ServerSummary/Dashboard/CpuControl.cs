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
    internal partial class CpuControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CPUControl");

        public CpuControl() : base(DashboardPanel.Cpu)
        {
            InitializeComponent();
            helpTopic = HelpTopics.ServerDashboardViewCpuPanel;
            updateLinearScaleFontAsPerTheme(this.linearScale1);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                this.mainTableLayoutPanel.Controls.Clear();
                this.mainTableLayoutPanel.Controls.Add(this.callRatesChart, 0, 0);
                this.mainTableLayoutPanel.RowCount = 1;
                this.mainTableLayoutPanel.ColumnCount = 1;
                this.mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
                this.mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.00F));
            }
            InitializeCharts();
            ConfigureDataSource();
            //SQLDM-30848, adapting resolutions, Kartik
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            ChartFxExtensions.SetContextMenu(cpuUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(callRatesChart, toolbarsManager);

            InitializeCpuUsageChart();
            InitializeQueueLengthGauge();
            InitializeCallRatesChart();
            InitalizeDrilldown(cpuUsageChart, callRatesChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeCpuUsageChart()
        {
            cpuUsageChart.Printer.Compress = true;
            cpuUsageChart.Printer.ForceColors = true;
            cpuUsageChart.ToolTipFormat = "%s\n%v%%\n%x";
            cpuUsageChart.DataSourceSettings.Fields.Clear();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                FieldMap totalCpuUsageFieldMap = new FieldMap("TotalCpuUsage", FieldUsage.Value);
                totalCpuUsageFieldMap.DisplayName = "SQL Server";
                cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                     {
                                                                     dateFieldMap,
                                                                     totalCpuUsageFieldMap
                                                                     });
                this.cpuUsageChart.AxisY.Title.Text = "VCORE Usage %";
            }
            else
            {
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                FieldMap totalCpuUsageFieldMap = new FieldMap("TotalCpuUsage", FieldUsage.Value);
                totalCpuUsageFieldMap.DisplayName = "OS";
                FieldMap sqlServerCpuUsageFieldMap = new FieldMap("SqlServerCpuUsage", FieldUsage.Value);
                sqlServerCpuUsageFieldMap.DisplayName = "SQL Server";
                FieldMap vmCpuUsageFieldMap = new FieldMap("vmCPUUsage", FieldUsage.Value);
                vmCpuUsageFieldMap.DisplayName = "VM";
                FieldMap esxCpuUsageFieldMap = new FieldMap("esxCPUUsage", FieldUsage.Value);
                esxCpuUsageFieldMap.DisplayName = "Host";
                cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                     {
                                                                     dateFieldMap,
                                                                     vmCpuUsageFieldMap,
                                                                     esxCpuUsageFieldMap,
                                                                     sqlServerCpuUsageFieldMap,
                                                                     totalCpuUsageFieldMap
                                                                     });
            }

            foreach (SeriesAttributes series in cpuUsageChart.Series)
            {
                if (series.Text.Equals("VM") || series.Text.Equals("Host"))
                {
                    series.Gallery = Gallery.Lines;
                }
            }

            cpuUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cpuUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            cpuUsageChart.AxisY.Min = 0;
            cpuUsageChart.AxisY.Max = 100;

        }

        private void InitializeQueueLengthGauge()
        {
            queueLengthGauge.ToolTip =
                queueLengthGauge.MainIndicator.ToolTip = "%v items queued";
        }

        private void InitializeCallRatesChart()
        {
            callRatesChart.Printer.Compress = true;
            callRatesChart.Printer.ForceColors = true;
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

            callRatesChart.DataSourceSettings.Fields.Clear();
            callRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      compilesFieldMap,
                                                                      recompilesFieldMap,
                                                                      batchesFieldMap,
                                                                      transactionsFieldMap
                                                                  });

            callRatesChart.Series[0].Gallery = Gallery.Lines;
            callRatesChart.Series[1].Gallery = Gallery.Lines;

            UpdateSeriesSettings();

            callRatesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            callRatesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void UpdateSeriesSettings()
        {
            try
            {
                if (callRatesChart.Series[2].Gallery != Gallery.Area)
                {
                    callRatesChart.Series[2].Gallery = Gallery.Area;
                    callRatesChart.Series[2].AlternateColor = System.Drawing.Color.Transparent;
                    callRatesChart.Series[2].Border.Visible = true;
                    callRatesChart.Series[2].FillMode = ChartFX.WinForms.FillMode.Gradient;
                }
                if (callRatesChart.Series[3].Gallery != Gallery.Area)
                {
                    callRatesChart.Series[3].Gallery = Gallery.Area;
                    callRatesChart.Series[3].AlternateColor = System.Drawing.Color.Transparent;
                    callRatesChart.Series[3].Border.Visible = true;
                    callRatesChart.Series[3].FillMode = ChartFX.WinForms.FillMode.Gradient;
                }
            }
            catch (Exception e) {
                LOG.Warn("UpdateSeriesSettings encountered an exception", e);
            }

            foreach (SeriesAttributes series in cpuUsageChart.Series)
            {
                if (series.Text.Equals("VM") || series.Text.Equals("Host"))
                {
                    series.Gallery = Gallery.Lines;
                }
            }

            ChartFxExtensions.SetAreaSeriesAlphaChannel(cpuUsageChart, 175, false);
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

                // update the CPU Usage chart
                cpuUsageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(cpuUsageChart, 2);

                // update the Processor Queue Length gauge and text
                double value;
                if (historyData.CurrentServerOverviewSnapshot != null &&
                    historyData.CurrentServerOverviewSnapshot.OSMetricsStatistics.ProcessorQueueLength.HasValue)
                {
                    queueLengthLabel.Text =
                        historyData.CurrentServerOverviewSnapshot.OSMetricsStatistics.ProcessorQueueLength.Value.
                            ToString();
                    value = historyData.CurrentServerOverviewSnapshot.OSMetricsStatistics.ProcessorQueueLength.Value;
                }
                else
                {
                    value = 0;
                    queueLengthLabel.Text = "?";
                }

                queueLengthGauge.MainValue = value;
                queueLengthGauge.Invalidate(true);

                // update the Call Rates chart
                callRatesChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(callRatesChart, 2);

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

                if (activeSnapshot.OSMetricsStatistics.PercentProcessorTime.HasValue
                    && activeSnapshot.OSMetricsStatistics.PercentProcessorTime > 0)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.OSCPUUsagePct, activeSnapshot.OSMetricsStatistics.PercentProcessorTime.Value));
                if (activeSnapshot.Statistics.CpuPercentage.HasValue)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.SQLCPUUsagePct, activeSnapshot.Statistics.CpuPercentage.Value));
                if (activeSnapshot.OSMetricsStatistics.ProcessorQueueLength.HasValue)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.OSCPUProcessorQueueLength, activeSnapshot.OSMetricsStatistics.ProcessorQueueLength.Value));
                if (activeSnapshot.VMConfig != null)
                {
                    if (activeSnapshot.VMConfig.PerfStats.CpuUsage > 0)
                        alerts.Add(new Pair<Metric, IComparable>(Metric.VmCPUUtilization, activeSnapshot.VMConfig.PerfStats.CpuUsage));
                    if (activeSnapshot.VMConfig.ESXHost.PerfStats.CpuUsage > 0)
                        alerts.Add(new Pair<Metric, IComparable>(Metric.VmESXCPUUtilization, activeSnapshot.VMConfig.ESXHost.PerfStats.CpuUsage));
                }
            }

            SetAlertStatus(alerts);
        }

        public override void ShowVMData(bool showVM)
        {
            foreach (SeriesAttributes series in cpuUsageChart.Series)
            {
                if ((series.Text == "VM") || (series.Text == "Host"))
                    series.Visible = showVM;
            }
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
                contextControl = queueLengthGauge;
            }

            switch (e.Tool.Key)
            {
                case "usageChartContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InstanceProps.Visible =
                        ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleCpuAlertsButton"]).InstanceProps.Visible =
                        ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleSqlAlertsButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                    break;
                case "queueLengthGaugeContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleQueueLengthAlertsButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                    break;
                case "callRatesChartContextMenu":
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                    break;
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

                    if (targetControl == cpuUsageChart)
                    {
                        dialog.Select(Metric.OSCPUUsagePct);
                    }
                    else if (targetControl == queueLengthGauge)
                    {
                        dialog.Select(Metric.OSCPUProcessorQueueLength);
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
                if (targetControl == cpuUsageChart)
                {
                    topic = HelpTopics.ServerDashboardViewCpuPanelUsage;
                }
                else if (targetControl == queueLengthGauge)
                {
                    topic = HelpTopics.ServerDashboardViewCpuPanelQueueLength;
                }
                else if (targetControl == callRatesChart)
                {
                    topic = HelpTopics.ServerDashboardViewCpuPanelCallRates;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesCpu);
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
        //SQLDM-30848, adapting resolutions, Kartik
        private void ScaleControlsAsPerResolution()
        {
            this.cpuUsageChart.LegendBox.AutoSize = true;
            this.callRatesChart.AxisY.Max = 100D;
            this.callRatesChart.AxisY.Min = 0D;
            this.callRatesChart.LegendBox.LineSpacing = 1D;
            this.cpuUsagePanel.Padding = new System.Windows.Forms.Padding(0,10,10,10);
            this.callRatesPanel.Padding = new System.Windows.Forms.Padding(0,10,10,10);
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
