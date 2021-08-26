using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using ChartFX.WinForms.Gauge;
using Idera.SQLdm.Common.Data;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class TempDBControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private ServerVersion serverVersion = null;

        public TempDBControl()
            : base(DashboardPanel.TempDB)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(usageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(contentionChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewTempDBPanel;
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            InitializeCharts();
            ConfigureDataSource();
        }

        private void InitializeCharts()
        {
            InitializeUsageChart();
            InitializeContentionChart();
            InitializeCleanupGauge();
            InitalizeDrilldown(usageChart, contentionChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeUsageChart()
        {
            usageChart.Printer.Orientation = PageOrientation.Landscape;
            usageChart.Printer.Compress = true;
            usageChart.Printer.ForceColors = true;
            usageChart.Printer.Document.DocumentName = "Tempdb Space Used Over Time";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            FieldMap userObjectsFieldMap = new FieldMap("TempdbUserObjectsMegabytes", FieldUsage.Value);
            userObjectsFieldMap.DisplayName = "User";
            FieldMap internalObjectsFieldMap = new FieldMap("TempdbInternalObjectsMegabytes", FieldUsage.Value);
            internalObjectsFieldMap.DisplayName = "Internal";
            FieldMap versionStoreFieldMap = new FieldMap("TempdbVersionStoreMegabytes", FieldUsage.Value);
            versionStoreFieldMap.DisplayName = "Version Store";
            FieldMap mixedExtentsFieldMap = new FieldMap("TempdbMixedExtentsMegabytes", FieldUsage.Value);
            mixedExtentsFieldMap.DisplayName = "Mixed";

            usageChart.DataSourceSettings.Fields.Clear();
            usageChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                dateFieldMap,
                userObjectsFieldMap,
                internalObjectsFieldMap,
                versionStoreFieldMap,
                mixedExtentsFieldMap});

            usageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            usageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            usageChart.AxisY.AutoScale = true;
            usageChart.AxisY.DataFormat.Decimals = 2;
            usageChart.ToolTipFormat = "%l\n%s\n%v MB\n%x";

            usageChart.DataSourceSettings.ReloadData();

            foreach (SeriesAttributes series in usageChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }
        }

        private void InitializeContentionChart()
        {
            contentionChart.Printer.Orientation = PageOrientation.Landscape;
            contentionChart.Printer.Compress = true;
            contentionChart.Printer.ForceColors = true;
            contentionChart.Printer.Document.DocumentName = "Tempdb Contention";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            FieldMap pfsFieldMap = new FieldMap("TempdbPFSWaitTime", FieldUsage.Value);
            pfsFieldMap.DisplayName = "PFS Page";
            FieldMap gamFieldMap = new FieldMap("TempdbGAMWaitTime", FieldUsage.Value);
            gamFieldMap.DisplayName = "GAM Page";
            FieldMap sgamFieldMap = new FieldMap("TempdbSGAMWaitTime", FieldUsage.Value);
            sgamFieldMap.DisplayName = "SGAM Page";

            contentionChart.DataSourceSettings.Fields.Clear();
            contentionChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                dateFieldMap,
                pfsFieldMap,
                gamFieldMap,
                sgamFieldMap});

            contentionChart.AllSeries.Gallery = Gallery.Lines;
            contentionChart.AxisY.AutoScale = true;
            contentionChart.AxisY.Title.Text = "Wait ms";
            contentionChart.AxisY.DataFormat.Decimals = 2;
            contentionChart.AllSeries.Border.Visible = false;
            contentionChart.ToolTipFormat = "%l\n%s\n%v ms\n%x";

            contentionChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            contentionChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        private void InitializeCleanupGauge()
        {
            cleanupGauge.MainScale.Indicators[0].Value = 0.0d;
            cleanupGauge.MainScale.Indicators[1].Value = 0.0d;
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                var snapshot = historyData.CurrentServerOverviewSnapshot;

                if (serverVersion == null)
                {
                    if (snapshot != null)
                    {
                        serverVersion = snapshot.ProductVersion;
                    }
                    else
                    {
                        int Id = RepositoryHelper.GetSelectedInstanceId(instanceId);
                        MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id);
                        if (status != null)
                        {
                            serverVersion = status.InstanceVersion;
                        }
                    }
                }

                if (DashboardHelper.IsVersionSupported(DashboardPanelConfiguration.Panel, serverVersion))
                {
                    UpdateAlerts();

                    // update the Tempdb Usage chart
                    usageChart.DataSource = historyData.CurrentSnapshotsDataTable;
                    ChartFxExtensions.SetAxisXTimeScale(usageChart, 2);

                    //// update the Tempdb Contention chart
                    contentionChart.DataSource = historyData.CurrentSnapshotsDataTable;
                    ChartFxExtensions.SetAxisXTimeScale(contentionChart, 2);

                    // poke values into Version Store Cleanup Rate gauge indicators
                    cleanupGauge.MainScale.Indicators[0].Value =
                        (snapshot != null && snapshot.TempdbStatistics != null && snapshot.TempdbStatistics.VersionStoreCleanupKilobytes.HasValue)
                            ? snapshot.TempdbStatistics.VersionStoreCleanupKilobytes.Value
                            : 0.0m;

                    cleanupGauge.MainScale.Indicators[1].Value =
                        (snapshot != null && snapshot.TempdbStatistics != null && snapshot.TempdbStatistics.VersionStoreGenerationKilobytes.HasValue)
                            ? snapshot.TempdbStatistics.VersionStoreGenerationKilobytes.Value
                            : 0.0m;
                    statusLabel.Visible = false;
                }
                else
                {
                    statusLabel.BringToFront();
                    statusLabel.Visible = true;
                }

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.TempdbStatistics != null)
                {
                    if (activeSnapshot.TempdbStatistics.VersionStoreMegabytes.HasValue)
                        alerts.Add(new Pair<Metric, IComparable>(Metric.VersionStoreSize, activeSnapshot.TempdbStatistics.VersionStoreMegabytes.Value));

                    double totalWaits = activeSnapshot.TempdbStatistics.TempdbPFSWaitTime.TotalMilliseconds
                                        + activeSnapshot.TempdbStatistics.TempdbGAMWaitTime.TotalMilliseconds
                                        + activeSnapshot.TempdbStatistics.TempdbSGAMWaitTime.TotalMilliseconds;
                    alerts.Add(new Pair<Metric, IComparable>(Metric.TempdbContention, totalWaits));

                    if (activeSnapshot.TempdbStatistics.VersionStoreCleanupKilobytes.HasValue
                            && activeSnapshot.TempdbStatistics.VersionStoreGenerationKilobytes.HasValue
                            && activeSnapshot.TimeDelta.HasValue
                            && activeSnapshot.TimeDelta.Value.TotalSeconds >= 60)
                    {
                        decimal value;
                        if (!(activeSnapshot.TempdbStatistics.VersionStoreCleanupKilobytes.Value == 0
                              && activeSnapshot.TempdbStatistics.VersionStoreGenerationKilobytes.Value > 0))
                        {
                            value = 9999;
                        }
                        else
                        {
                            var genPerSecond = activeSnapshot.TempdbStatistics.VersionStoreGenerationKilobytes.Value
                                               /(decimal) activeSnapshot.TimeDelta.Value.TotalSeconds;
                            var cleanPerSecond = activeSnapshot.TempdbStatistics.VersionStoreCleanupKilobytes.Value
                                                 /(decimal) activeSnapshot.TimeDelta.Value.TotalSeconds;

                            if (cleanPerSecond == 0)
                            {
                                value = 9999;
                            }
                            else
                            {
                                value = ((genPerSecond/cleanPerSecond) - 1)*100;
                            }
                        }

                        alerts.Add(new Pair<Metric, IComparable>(Metric.VersionStoreGenerationRatio, value));
                    }
                }
            }

            SetAlertStatus(alerts);
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
                contextControl = cleanupGauge;
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
                        dialog.Select(Metric.VersionStoreSize);
                    }
                    else if (targetControl == contentionChart)
                    {
                        dialog.Select(Metric.TempdbContention);
                    }
                    else if (targetControl == cleanupGauge)
                    {
                        dialog.Select(Metric.VersionStoreGenerationRatio);
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
                if (targetControl == usageChart)
                {
                    topic = HelpTopics.ServerDashboardViewTempDBPanelUsage;
                }
                else if (targetControl == contentionChart)
                {
                    topic = HelpTopics.ServerDashboardViewTempDBPanelContention;
                }
                else if (targetControl == cleanupGauge)
                {
                    topic = HelpTopics.ServerDashboardViewTempDBPanelVersionStore;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesTempdbView);
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
    }
}
