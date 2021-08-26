using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;

using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Dialogs;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class LockWaitsControl : Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard.DashboardControl
    {
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private ServerVersion serverVersion = null;

        public LockWaitsControl() : base(DashboardPanel.LockWaits)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(waitChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewLocksPanel;
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

            ConfigureWaitChart();
            InitalizeDrilldown(waitChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ConfigureWaitChart()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            var alloc = new FieldMap("Wait Time - AllocUnit", FieldUsage.Value);
            alloc.DisplayName = "AllocUnit";
            var app = new FieldMap("Wait Time - Application", FieldUsage.Value);
            app.DisplayName = "Application";
            var db = new FieldMap("Wait Time - Database", FieldUsage.Value);
            db.DisplayName = "Database";
            var extent = new FieldMap("Wait Time - Extent", FieldUsage.Value);
            extent.DisplayName = "Extent";
            var file = new FieldMap("Wait Time - File", FieldUsage.Value);
            file.DisplayName = "File";
            var hoBT = new FieldMap("Wait Time - HoBT", FieldUsage.Value);
            hoBT.DisplayName = "HoBT";
            var key = new FieldMap("Wait Time - Key", FieldUsage.Value);
            key.DisplayName = "Key";
            var keyRid = new FieldMap("Wait Time - KeyRID", FieldUsage.Value);
            keyRid.DisplayName = "Key/RID";
            var latch = new FieldMap("Wait Time - Latch", FieldUsage.Value);
            latch.DisplayName = "Latch";
            var metadata = new FieldMap("Wait Time - Metadata", FieldUsage.Value);
            metadata.DisplayName = "Metadata";
            var obj = new FieldMap("Wait Time - Object", FieldUsage.Value);
            obj.DisplayName = "Object";
            var page = new FieldMap("Wait Time - Page", FieldUsage.Value);
            page.DisplayName = "Page";
            var rid = new FieldMap("Wait Time - RID", FieldUsage.Value);
            rid.DisplayName = "RID";
            var table = new FieldMap("Wait Time - Table", FieldUsage.Value);
            table.DisplayName = "Table";

            waitChart.DataSourceSettings.Fields.Clear();
            if (serverVersion != null && serverVersion.Major == 8)
            {
                waitChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     db,
                                                                     extent,
                                                                     key,
                                                                     page,
                                                                     rid,
                                                                     table
                                                                 });
            }
            else
            {
                waitChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                 {
                                                                     dateFieldMap,
                                                                     db,
                                                                     extent,
                                                                     file,
                                                                     hoBT,
                                                                     keyRid,
                                                                     metadata,
                                                                     obj,
                                                                     page
                                                                 });
            }

            waitChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            waitChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
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
                        MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(Id, Settings.Default.RepoId);
                        if (status != null)
                        {
                            serverVersion = status.InstanceVersion;
                        }
                    }
                }

                ConfigureWaitChart();
                waitChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(waitChart, 5);
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
            string topic = HelpTopics.ServerDashboardViewLocksPanel;

            if (targetControl != null)
            {
                if (targetControl == waitChart)
                {
                    topic = HelpTopics.ServerDashboardViewLocksPanelWaits;
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsLocks);
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
