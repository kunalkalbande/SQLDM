using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using ChartFX.WinForms.Gauge;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class CacheControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;

        public CacheControl() : base(DashboardPanel.Cache)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(areasChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(pageLifeChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewCachePanel;
            updateLinearScaleFontAsPerTheme(this.linearScale1);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);


            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            else
            {
                this.areasPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
                this.pageLifePanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            }
        }
        private void ScaleControlsAsPerResolution()
        {
            this.areasChart.LegendBox.AutoSize = true;
            this.pageLifeChart.LegendBox.AutoSize = true;
            this.areasPanel.Padding = new System.Windows.Forms.Padding(0,10,0,10);
            this.pageLifePanel.Padding = new System.Windows.Forms.Padding(0,10,0,10);
        }
        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);            
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            InitializePageLifeExpectancyChart();
            InitializeMemoryAreasChart();
            InitializeCacheHitGauge();
            InitalizeDrilldown(areasChart, pageLifeChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeCacheHitGauge()
        {
            hitRatioGauge.MainScale.Indicators[0].Value = 0.0d;
            hitRatioGauge.MainScale.Indicators[1].Value = 0.0d;
        }

        private void InitializePageLifeExpectancyChart()
        {
            pageLifeChart.Printer.Orientation = PageOrientation.Landscape;
            pageLifeChart.Printer.Compress = true;
            pageLifeChart.Printer.ForceColors = true;
            pageLifeChart.Printer.Document.DocumentName = "Page Life Expectancy Chart";
            pageLifeChart.ToolTipFormat = "%s\n%v seconds\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap pageLifeExpectancyPages = new FieldMap("PageLifeExpectancy", FieldUsage.Value);
            pageLifeExpectancyPages.DisplayName = "Page Life Expectancy";

            pageLifeChart.DataSourceSettings.Fields.Clear();
            pageLifeChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                       {
                                                                          dateFieldMap,
                                                                          pageLifeExpectancyPages
                                                                       });

            pageLifeChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            pageLifeChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            pageLifeChart.AxisX.LabelTrimming = StringTrimming.Character;
            pageLifeChart.AxisX.AutoScale = true;

            foreach (SeriesAttributes series in pageLifeChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }
        }

        private void InitializeMemoryAreasChart()
        {
            areasChart.Printer.Orientation = PageOrientation.Landscape;
            areasChart.Printer.Compress = true;
            areasChart.Printer.ForceColors = true;
            areasChart.Printer.Document.DocumentName = "Memory Areas Chart";
            areasChart.ToolTipFormat = "%s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var procedureCacheFieldMap = new FieldMap("ProcedureCachePages", FieldUsage.Value);
            procedureCacheFieldMap.DisplayName = "Procedure Cache";
            var databaseFieldMap = new FieldMap("UsedDataMemory", FieldUsage.Value);
            databaseFieldMap.DisplayName = "Database";
            var freeFieldMap = new FieldMap("BufferCacheFreePages", FieldUsage.Value);
            freeFieldMap.DisplayName = "Free";
            var locksFieldMap = new FieldMap("LockOptimizerConnectionSortHashIndexMemory", FieldUsage.Value);
            locksFieldMap.DisplayName = "Other";

            areasChart.DataSourceSettings.Fields.Clear();
            areasChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        freeFieldMap,
                                                                        databaseFieldMap,
                                                                        locksFieldMap,
                                                                        procedureCacheFieldMap
                                                                    });

            areasChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            areasChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            areasChart.LegendBox.LineSpacing = 1;
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                var snapshot = historyData.CurrentServerOverviewSnapshot;

                // update the CPU Usage chart
                pageLifeChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(pageLifeChart, 2);

                // update the Call Rates chart
                areasChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(areasChart, 2);

                // poke values into hit ratio gauge indicators
                hitRatioGauge.MainScale.Indicators[0].Value =
                    snapshot != null && snapshot.Statistics.BufferCacheHitRatio.HasValue
                        ? snapshot.Statistics.BufferCacheHitRatio.Value
                        : 0.0d;

                hitRatioGauge.MainScale.Indicators[1].Value =
                    snapshot != null && snapshot.Statistics.CacheHitRatio.HasValue
                        ? snapshot.Statistics.CacheHitRatio.Value
                        : 0.0d;

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.Statistics.PageLifeExpectancySeconds.HasValue
                    && activeSnapshot.Statistics.PageLifeExpectancySeconds.Value > 0)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.PageLifeExpectancy, activeSnapshot.Statistics.PageLifeExpectancySeconds.Value));
                if (activeSnapshot.Statistics.CacheHitRatio.HasValue)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.ProcCacheHitRatio, activeSnapshot.Statistics.CacheHitRatio.Value));
            }

            SetAlertStatus(alerts);
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
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
 
        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
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
                contextControl = hitRatioGauge;
            }

            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    if (contextControl == pageLifeChart)
                    {
                        ((ButtonTool)((PopupMenuTool)e.Tool).Tools["configureAlertsButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True;
                    }
                    else
                    {
                        ((ButtonTool)((PopupMenuTool)e.Tool).Tools["configureAlertsButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
                    }
                    break;
            }
        }

        #region Chart Actions

        private void ConfigureControlAlerts(Control targetControl)
        {
            if (targetControl != null)
            {
                try
                {
                    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);

                    if (targetControl == pageLifeChart)
                    {
                        dialog.Select(Metric.PageLifeExpectancy);
                    }
                    else if (targetControl == areasChart)
                    {
                        dialog.Select(Metric.SQLMemoryUsagePct);
                    }
                    else if (targetControl == hitRatioGauge)
                    {
                        dialog.Select(Metric.ProcCacheHitRatio);
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
            string target = HelpTopics.ServerDashboardViewCachePanel;
            if (targetControl != null)
            {
                if (targetControl == pageLifeChart)
                {
                    target = HelpTopics.ServerDashboardViewCachePanelPageLife;
                }
                else if (targetControl == areasChart)
                {
                    target = HelpTopics.ServerDashboardViewCachePanelAreas;
                }
                else if (targetControl == hitRatioGauge)
                {
                    target = HelpTopics.ServerDashboardViewCachePanelHitRatios;
                }
            }

            ShowHelp(target);
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
                string title = string.Format(chartTitleFormat, this.Caption, control.Tag.ToString());
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
