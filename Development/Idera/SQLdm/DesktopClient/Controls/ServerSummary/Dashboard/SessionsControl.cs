using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using ChartFX.WinForms;
using ChartFX.WinForms.Gauge;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win.UltraWinToolbars;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.UI.Dialogs;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class SessionsControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;

        public SessionsControl() : base(DashboardPanel.Sessions)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(sessionsChart, toolbarsManager);
            helpTopic = HelpTopics.ServerDashboardViewSessionsPanel;
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            else
            {
                this.sessionsPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
                this.queueLengthPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            }
            updateLinearScaleFontAsPerTheme(this.linearScale1);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

        }
        private void ScaleControlsAsPerResolution()
        {
            this.sessionsPanel.Padding = new System.Windows.Forms.Padding(0,10,0,10);
            this.queueLengthPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
        }
        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            ConfigureSessionsChart();
            InitializeClientComputersGauge();
            InitalizeDrilldown(sessionsChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeClientComputersGauge()
        {
            clientComputersGauge.ToolTip =
                clientComputersGauge.MainIndicator.ToolTip = "Client Computers: %v";
        }

        private void ConfigureSessionsChart()
        {
            sessionsChart.Printer.Orientation = PageOrientation.Landscape;
            sessionsChart.Printer.Compress = true;
            sessionsChart.Printer.ForceColors = true;
            sessionsChart.Printer.Document.DocumentName = "Sessions Chart";
            sessionsChart.DataSourceSettings.Fields.Clear();
            sessionsChart.ToolTipFormat = "%v %s\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            var activeSessionsFieldMap = new FieldMap("ActiveSessions", FieldUsage.Value);
            activeSessionsFieldMap.DisplayName = "Active";
            var blockedSessionsFieldMap = new FieldMap("BlockedProcesses", FieldUsage.Value);
            blockedSessionsFieldMap.DisplayName = "Blocked Sessions";
            var leadBlockersFieldMap = new FieldMap("LeadBlockers", FieldUsage.Value);
            leadBlockersFieldMap.DisplayName = "Lead Blockers";
            var deadlockedFieldMap = new FieldMap("Total Deadlocks", FieldUsage.Value);
            deadlockedFieldMap.DisplayName = "Total Deadlocks";

            sessionsChart.DataSourceSettings.Fields.Clear();
            sessionsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        deadlockedFieldMap,
                        leadBlockersFieldMap,
                        blockedSessionsFieldMap,
                        activeSessionsFieldMap});

            sessionsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            sessionsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            sessionsChart.AxisY.ForceZero = true;
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                var snapshot = historyData.CurrentServerOverviewSnapshot;

                sessionsChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(sessionsChart, 5);

                if (snapshot != null && snapshot.SystemProcesses != null &&
                    snapshot.SystemProcesses.ComputersHoldingProcesses.HasValue)
                    clientComputersGauge.MainIndicator.Value = snapshot.SystemProcesses.ComputersHoldingProcesses.Value;
                else
                    clientComputersGauge.MainIndicator.Value = 0;
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IComparable>> alerts = new List<Pair<Metric, IComparable>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.SystemProcesses.BlockedProcesses.HasValue)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.BlockedSessions, activeSnapshot.SystemProcesses.BlockedProcesses.Value));
                if (activeSnapshot.SystemProcesses.ComputersHoldingProcesses.HasValue)
                    alerts.Add(new Pair<Metric, IComparable>(Metric.ClientComputers, activeSnapshot.SystemProcesses.ComputersHoldingProcesses.Value));
                //the snapshot isn't populated for history, so use the datatable
                if (historyData.CurrentSnapshotsDataTable.Rows.Count > 0)
                {
                    int row = historyData.CurrentSnapshotsDataTable.Rows.Count - 1;
                    if (historyData.CurrentSnapshotsDataTable.Rows[row]["Total Deadlocks"] != DBNull.Value)
                    {
                        long deadlocks = (long)historyData.CurrentSnapshotsDataTable.Rows[row]["Total Deadlocks"];
                        alerts.Add(new Pair<Metric, IComparable>(Metric.Deadlock, deadlocks));
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
                contextControl = clientComputersGauge;
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

                    if (targetControl == sessionsChart)
                    {
                        dialog.Select(Metric.BlockedSessions);
                    }
                    else if (targetControl == clientComputersGauge)
                    {
                        dialog.Select(Metric.ClientComputers);
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
            string topic = HelpTopics.ServerDashboardViewSessionsPanel;

            if (targetControl != null)
            {
                if (targetControl == sessionsChart)
                {
                    topic = HelpTopics.ServerDashboardViewSessionsPanelSessions;
                }
                else if (targetControl == clientComputersGauge)
                {
                    topic = HelpTopics.ServerDashboardViewSessionsPanelClients;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                if (targetControl == sessionsChart)
                {
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.Sessions);
                }
                else if (targetControl == clientComputersGauge)
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
                string title = chart.Tag.ToString();
                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = chart.Tag.ToString();
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = chart.Tag.ToString();
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
