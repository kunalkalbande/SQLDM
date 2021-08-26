using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using System.Globalization;
namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class SQLServerPhysicalIO : DashboardControl
    {
        private const string OPTIONTYPE_DISK = @"SQL Server Physical I/O";
        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;
        private Chart contextMenuSelectedChart = null;
        public SQLServerPhysicalIO() : base(DashboardPanel.SQLServerPhysicalIO)
        {
            InitializeComponent();
            helpTopic = HelpTopics.ServerDashboardViewServerWaitsPanel;
        }
        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
                base.Initialize(baseView, history);
                ChartFxExtensions.SetContextMenu(sqlServerReadsWritesChart, toolbarsManager);
                Settings.Default.PropertyChanged += Settings_PropertyChanged;
                MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
                AdaptFontSize();
                InitializeCharts();
          
        }
        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();
                var snapshot = historyData.CurrentServerOverviewSnapshot;
                sqlServerReadsWritesChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(sqlServerReadsWritesChart, 5);
            }
        }
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }
        private void UpdateChartDataFilter()
        {
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                DateTime viewFilter =DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }
        private void InitializeCharts()
        {
            InitializeSqlServerReadsWritesChart();
            drilldownProperties.ChartCursor = Cursors.Hand;
            InitalizeDrilldown(sqlServerReadsWritesChart);
        }
        private void InitializeSqlServerReadsWritesChart()
        {
            sqlServerReadsWritesChart.Tag = sqlServerReadsWritesChartTypeButton;
            sqlServerReadsWritesChart.Printer.Orientation = PageOrientation.Landscape;
            sqlServerReadsWritesChart.Printer.Compress = true;
            sqlServerReadsWritesChart.Printer.ForceColors = true;
            sqlServerReadsWritesChart.Printer.Document.DocumentName = "SQL Server I/O Chart";
            sqlServerReadsWritesChart.ToolBar.RemoveAt(0);
            sqlServerReadsWritesChart.ToolTipFormat = "%v %s\n%x";

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

            sqlServerReadsWritesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                             {
                                                                                 dateFieldMap,
                                                                                 checkpointWritesFieldMap,
                                                                                 lazyWriterWritesFieldMap,
                                                                                 readAheadReadsFieldMap,
                                                                                 synchronousReadsFieldMap,
                                                                                 synchronousWritesFieldMap
                                                                             });

            sqlServerReadsWritesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            sqlServerReadsWritesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            sqlServerReadsWritesChart.LegendBox.Width = 145;
            sqlServerReadsWritesChart.LegendBox.PlotAreaOnly = false;
            sqlServerReadsWritesChart.LegendBox.LineSpacing = 1;
            sqlServerReadsWritesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }
        private void ShowControlHelp(Control targetControl)
        {
            string topic = HelpTopics.ServerDashboardViewServerWaitsPanel;

            if (targetControl != null)
            {
                if (targetControl == sqlServerReadsWritesChart)
                {
                    topic = HelpTopics.ServerDashboardViewDiskPanelPhysicalIo;
                }
            }

            ShowHelp(topic);
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
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
        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

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
        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesDisk);
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


        #region Chart Click Events
        private void OnChartMouseClick(object sender, HitTestEventArgs e)
        {
            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ServerViews targetView;
                  targetView = ServerViews.ResourcesDisk;
                 if (sender == sqlServerReadsWritesChart)
                    targetView = ServerViews.ResourcesDisk;
                else
                    return;
                ApplicationController.Default.ShowServerView(instanceId, targetView);
            }
        }
        #endregion
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
