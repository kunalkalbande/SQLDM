using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;

using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class ServerWaitsControl : DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private ServerVersion serverVersion = null;

        public ServerWaitsControl() : base(DashboardPanel.ServerWaits)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(waitChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewServerWaitsPanel;
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            InitializeCharts();
        }



        private void InitializeCharts()
        {
            waitChart.Printer.Compress = true;
            waitChart.Printer.ForceColors = true;
            waitChart.ToolTipFormat = "%s\n%v per second\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var io = new FieldMap("IOWaits", FieldUsage.Value);
            io.DisplayName = "I/O";
            var locks = new FieldMap("LockWaits", FieldUsage.Value);
            locks.DisplayName = "Locks";
            var memory = new FieldMap("MemoryWaits", FieldUsage.Value);
            memory.DisplayName = "Memory";
            var log = new FieldMap("TransactionLogWaits", FieldUsage.Value);
            log.DisplayName = "Log";
            var other = new FieldMap("OtherWaits", FieldUsage.Value);
            other.DisplayName = "Other";
            var signal = new FieldMap("SignalWaits", FieldUsage.Value);
            signal.DisplayName = "Signal";

            waitChart.DataSourceSettings.Fields.Clear();
            waitChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     io,
                                                                     locks,
                                                                     memory,
                                                                     log,
                                                                     other,
                                                                     signal
                                                                 });

            waitChart.Series[5].Gallery = Gallery.Area;
            waitChart.Series[5].Color = Color.FromArgb(175, waitChart.Series[5].Color);
            waitChart.Series[5].FillMode = FillMode.Solid;
            waitChart.Series[5].Border.Visible = true;
            waitChart.Series[5].Border.Width = 1;

            waitChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            waitChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            InitalizeDrilldown(waitChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
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
                    waitChart.DataSource = historyData.CurrentSnapshotsDataTable;
                    ChartFxExtensions.SetAxisXTimeScale(waitChart, 5);
                    statusLabel.Visible = false;
                }
                else
                {
                    statusLabel.BringToFront();
                    statusLabel.Visible = true;
                }
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

            ((ButtonTool)((PopupMenuTool)e.Tool).Tools["configureAlertsButton"]).InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
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
            string topic = HelpTopics.ServerDashboardViewServerWaitsPanel;

            if (targetControl != null)
            {
                if (targetControl == waitChart)
                {
                    topic = HelpTopics.ServerDashboardViewServerWaitsPanelWaits;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesWaitStats);
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
