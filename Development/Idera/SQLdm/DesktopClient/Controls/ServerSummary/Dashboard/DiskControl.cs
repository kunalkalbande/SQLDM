using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class DiskControl : DashboardControl
    {
        private const string OPTIONTYPE_DISK = @"Disk";

        private Control contextControl = null;

        private OrderedDictionary<string, DiskDrive> diskDrives;
        private string selectedDisk;
        private DataTable emptyDiskTable;

        public DiskControl() : base(DashboardPanel.Disk)
        {
            InitializeComponent();

            helpTopic = HelpTopics.ServerDashboardViewDiskPanel;
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            else
            {
                this.sqlServerPhysicalIoPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
                this.throughputPanel.Padding = new System.Windows.Forms.Padding(10);
                this.latencyPanel.Padding = new System.Windows.Forms.Padding(10);
            }
        }
        private void ScaleControlsAsPerResolution()
        {
            this.sqlServerPhysicalIoPanel.Padding = new System.Windows.Forms.Padding(10,0,10,10);
            this.throughputPanel.Padding = new System.Windows.Forms.Padding(10);
            this.latencyPanel.Padding = new System.Windows.Forms.Padding(10);
            this.physicalIoChart.LegendBox.AutoSize = true;
            this.physicalIoChart.AxesStyle = AxesStyle.Frame3D;
            this.throughputChart.LegendBox.AutoSize = true;
            this.throughputChart.LegendBox.Style = LegendBoxStyles.HidePartial;
            this.throughputChart.AxesStyle = AxesStyle.Math;
            this.latencyChart.LegendBox.AutoSize = true;
            this.latencyChart.AxesStyle = AxesStyle.FlatFrame;
        }
        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            InitializeDiskSelection();
            InitializeCharts();
        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == OPTIONTYPE_DISK)
                {
                    if (option.Values.Count > 0)
                    {
                        // just set the values because the list of disk drives may not exist yet
                        selectedDisk = option.Values[0];
                        UpdateConfigOptions();
                        ConfigureDiskSelection();
                    }
                    break;
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = new List<string> { selectedDisk };
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_DISK, options) });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (headerSelectTypeDropDownButton.Tag.ToString() != item.Text)
                {
                    setDiskSelection(item.Text);
                    UpdateConfigOptions();
                    ConfigureDataSource();
                }
            }
        }

        private void setDiskSelection(string disk)
        {
            if (disk != null)
            {
                selectedDisk = disk;
                headerSelectTypeDropDownButton.Text = disk.Length > 60
                                                          ? string.Format("{0}...", disk.Substring(0, 60))
                                                          : disk;
                headerSelectTypeDropDownButton.ToolTipText = disk;
                headerSelectTypeDropDownButton.Tag = disk;
            }
        }

        private void InitializeDiskSelection()
        {
            diskDrives = new OrderedDictionary<string, DiskDrive>();
            ConfigureDiskSelection();

            headerSelectTypeSeparator.Visible =
                headerSelectTypeDropDownButton.Visible = true;
        }

        private void ConfigureDiskSelection()
        {
            headerSelectTypeDropDownButton.DropDownItems.Clear();
            if (historyData != null)
            {
                diskDrives = ChartHelper.GetOrderedDiskDrives(historyData);
                if (diskDrives != null)
                {
                    foreach (string drive in diskDrives.Keys)
                    {
                        ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(drive, null, chartTypeSelectToolStripMenuItem_Click);
                        chartTypeItem.Tag = drive;
                        headerSelectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
                    }

                    setDiskSelection((IsDiskValid() || headerSelectTypeDropDownButton.DropDownItems.Count == 0)
                                         ? selectedDisk
                                         : headerSelectTypeDropDownButton.DropDownItems[0].Text);
                }
            }
            headerSelectTypeDropDownButton.Visible = headerSelectTypeDropDownButton.DropDownItems.Count > 0;
            UpdateStatus();
        }

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            ChartFxExtensions.SetContextMenu(latencyChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(throughputChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(physicalIoChart, toolbarsManager);

            //create a dummy datasource for the disk charts to initialize with until a disk drive exists
            emptyDiskTable = new DataTable();
            emptyDiskTable.Columns.Add("Date", typeof(DateTime));
            emptyDiskTable.Columns.Add(historyData.DiskTimePerReadPerDiskColumnPrefix, typeof(int));
            emptyDiskTable.Columns.Add(historyData.DiskTimePerWritePerDiskColumnPrefix, typeof(int));
            emptyDiskTable.Columns.Add(historyData.DiskReadsPerSecPerDiskColumnPrefix, typeof(int));
            emptyDiskTable.Columns.Add(historyData.DiskWritesPerSecPerDiskColumnPrefix, typeof(int));
            DataRow row = emptyDiskTable.NewRow();
            row["Date"] = DateTime.Now;
            row[historyData.DiskTimePerReadPerDiskColumnPrefix] = 0;
            row[historyData.DiskTimePerWritePerDiskColumnPrefix] = 0;
            row[historyData.DiskReadsPerSecPerDiskColumnPrefix] = 0;
            row[historyData.DiskWritesPerSecPerDiskColumnPrefix] = 0;

            emptyDiskTable.Rows.Add(row);
            InitializeLatencyChart();
            InitializeThroughputChart();
            InitializeSqlServerPhysicalIoChart();
            InitalizeDrilldown(latencyChart, throughputChart, physicalIoChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeLatencyChart()
        {
            latencyChart.Printer.Compress = true;
            latencyChart.Printer.ForceColors = true;
            latencyChart.Printer.Document.DocumentName = "Disk Latency Chart";
            latencyChart.ToolTipFormat = "%vms %s\n%x";

            ConfigureLatencyChart();
        }

        private void ConfigureLatencyChart()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            string disk = IsDiskFound() ? selectedDisk : string.Empty;
            FieldMap readsFieldMap = new FieldMap(historyData.DiskTimePerReadPerDiskColumnPrefix + disk, FieldUsage.Value);
            readsFieldMap.DisplayName = "per Read";
            FieldMap writesFieldMap = new FieldMap(historyData.DiskTimePerWritePerDiskColumnPrefix + disk, FieldUsage.Value);
            writesFieldMap.DisplayName = "per Write";

            latencyChart.DataSourceSettings.Fields.Clear();
            latencyChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            readsFieldMap,
                                                            writesFieldMap
                                                        });

            latencyChart.ToolTipFormat = string.Format("Disk {0}\n%v ms %s\n%x", selectedDisk);

            latencyChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            latencyChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ChartFxExtensions.SetAxisXTimeScale(latencyChart, 2);
        }

        private void InitializeThroughputChart()
        {
            throughputChart.Printer.Compress = true;
            throughputChart.Printer.ForceColors = true;
            throughputChart.Printer.Document.DocumentName = "Disk Throughput Chart";
            throughputChart.ToolTipFormat = "%v %s\\sec\n%x";

            ConfigureThroughputChart();
        }

        private void ConfigureThroughputChart()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            string disk = IsDiskFound() ? selectedDisk : string.Empty;
            FieldMap readsFieldMap = new FieldMap(historyData.DiskReadsPerSecPerDiskColumnPrefix + disk, FieldUsage.Value);
            readsFieldMap.DisplayName = "Reads";
            FieldMap writesFieldMap = new FieldMap(historyData.DiskWritesPerSecPerDiskColumnPrefix + disk, FieldUsage.Value);
            writesFieldMap.DisplayName = "Writes";

            throughputChart.DataSourceSettings.Fields.Clear();
            throughputChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            readsFieldMap,
                                                            writesFieldMap
                                                        });

            throughputChart.ToolTipFormat = string.Format("Disk {0}\n%v %s/sec\n%x", selectedDisk);

            foreach (SeriesAttributes series in throughputChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            throughputChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            throughputChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ChartFxExtensions.SetAxisXTimeScale(throughputChart, 2);
        }

        private void InitializeSqlServerPhysicalIoChart()
        {
            physicalIoChart.Printer.Compress = true;
            physicalIoChart.Printer.ForceColors = true;
            physicalIoChart.Printer.Document.DocumentName = "Physical I/O Chart";
            physicalIoChart.ToolTipFormat = "%v %s/sec\n%x";

            physicalIoChart.DataSourceSettings.Fields.Clear();

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

            physicalIoChart.DataSourceSettings.Fields.Clear();
            physicalIoChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            checkpointWritesFieldMap,
                                                            lazyWriterWritesFieldMap,
                                                            readAheadReadsFieldMap,
                                                            synchronousReadsFieldMap,
                                                            synchronousWritesFieldMap
                                                        });

            physicalIoChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            physicalIoChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {

                ConfigureDiskSelection();
                UpdateAlerts();

                // SqlDM 10.2 (Anshul Aggarwal) - Added a new disk datatable for disk charts.
                // update the charts
                // make sure the selected disk is in the data or use the empty datasource to prevent exceptions
                DataTable diskDataSource = IsDiskFound()
                                               ? historyData.CurrentDiskDataTable
                                               : emptyDiskTable;

                // SqlDM 10.2 (Anshul Aggarwal) - Filter disk table in realtime mode
                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    DateTime viewFilter = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                     //SQLDM - 19237 - Anshul Aggarwal - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
					historyData.RealTimeDiskDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                    historyData.RealTimeDiskDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                }

                latencyChart.SuspendLayout();
                ConfigureLatencyChart();
                latencyChart.DataSource = diskDataSource;
                latencyChart.ResumeLayout();

                throughputChart.SuspendLayout();
                ConfigureThroughputChart();
                throughputChart.DataSource = diskDataSource;
                throughputChart.ResumeLayout();

                physicalIoChart.DataSource = historyData.CurrentSnapshotsDataTable;
                ChartFxExtensions.SetAxisXTimeScale(physicalIoChart, 2);

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<Metric, IEnumerable<Pair<string, IComparable>>>> alerts = new List<Pair<Metric, IEnumerable<Pair<string, IComparable>>>>();

            if (historyData != null && historyData.CurrentServerOverviewSnapshot != null)
            {
                ServerOverview activeSnapshot = historyData.CurrentServerOverviewSnapshot;

                if (activeSnapshot.DiskDrives != null)
                {
                    List<Pair<string, IComparable>> readTimeAlerts = new List<Pair<string, IComparable>>();
                    List<Pair<string, IComparable>> writeTimeAlerts = new List<Pair<string, IComparable>>();
                    List<Pair<string, IComparable>> readAlerts = new List<Pair<string, IComparable>>();
                    List<Pair<string, IComparable>> writeAlerts = new List<Pair<string, IComparable>>();
                    if (diskDrives != null)
                    {
                        foreach (string drive in diskDrives.Keys)
                        {
                            if (activeSnapshot.DiskDrives.ContainsKey(drive) && activeSnapshot.DiskDrives[drive] != null)
                            {
                                if (activeSnapshot.DiskDrives[drive].AvgDiskSecPerRead.HasValue && activeSnapshot.DiskDrives[drive].AvgDiskSecPerRead.Value.TotalMilliseconds > 0)
                                    readTimeAlerts.Add(new Pair<string, IComparable>(drive, activeSnapshot.DiskDrives[drive].AvgDiskSecPerRead.Value.TotalMilliseconds));
                                if (activeSnapshot.DiskDrives[drive].AvgDiskSecPerWrite.HasValue && activeSnapshot.DiskDrives[drive].AvgDiskSecPerWrite.Value.TotalMilliseconds > 0)
                                    writeTimeAlerts.Add(new Pair<string, IComparable>(drive, activeSnapshot.DiskDrives[drive].AvgDiskSecPerWrite.Value.TotalMilliseconds));
                                if (activeSnapshot.DiskDrives[drive].DiskReadsPerSec.HasValue && activeSnapshot.DiskDrives[drive].DiskReadsPerSec.Value > 0)
                                    readAlerts.Add(new Pair<string, IComparable>(drive, activeSnapshot.DiskDrives[drive].DiskReadsPerSec.Value));
                                if (activeSnapshot.DiskDrives[drive].DiskWritesPerSec.HasValue && activeSnapshot.DiskDrives[drive].DiskWritesPerSec.Value > 0)
                                    writeAlerts.Add(new Pair<string, IComparable>(drive, activeSnapshot.DiskDrives[drive].DiskWritesPerSec.Value));
                            }
                        }
                    }
                    Pair<string, IComparable>[] x = readAlerts.ToArray();
                    if (readTimeAlerts.Count > 0)
                        alerts.Add(new Pair<Metric, IEnumerable<Pair<string, IComparable>>>(Metric.AverageDiskMillisecondsPerRead, readTimeAlerts));
                    if (writeTimeAlerts.Count > 0)
                        alerts.Add(new Pair<Metric, IEnumerable<Pair<string, IComparable>>>(Metric.AverageDiskMillisecondsPerWrite, writeTimeAlerts));
                    if (readAlerts.Count > 0)
                        alerts.Add(new Pair<Metric, IEnumerable<Pair<string, IComparable>>>(Metric.DiskReadsPerSecond, readAlerts));
                    if (writeAlerts.Count > 0)
                        alerts.Add(new Pair<Metric, IEnumerable<Pair<string, IComparable>>>(Metric.DiskWritesPerSecond, writeAlerts));
                }
            }

            SetMultipleItemAlertStatus(alerts);
        }

        private bool IsDiskValid()
        {
            return !(string.IsNullOrEmpty(selectedDisk) ||
                     diskDrives == null ||
                     !diskDrives.ContainsKey(selectedDisk));
        }

        /// <summary>
        /// SqlDM 10.2 - Added a new disk datatable for disk charts.
        /// </summary>
        private bool IsDiskFound()
        {
            bool isfound = false;

            if(historyData != null && historyData.CurrentDiskDataTable != null && historyData.CurrentDiskDataTable.Columns != null)
                isfound = historyData.CurrentDiskDataTable.Columns.Contains(historyData.DiskTimePerReadPerDiskColumnPrefix + selectedDisk);

            return isfound;
        }

        private void UpdateStatus()
        {
            if (IsDiskValid())
            {
                headerStatusLabel.Visible = false;
            }
            else
            {
                headerStatusLabel.ToolTipText = string.IsNullOrEmpty(selectedDisk)
                                                    ? @"No valid disk selection was available."
                                                    : string.Format("The selected disk '{0}' does not exist on this server.",
                                                                    selectedDisk);
                headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
                headerStatusLabel.Visible = true;
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
                contextControl = null;
            }

            switch (e.Tool.Key)
            {
                case "latencyChartContextMenu":
                    break;
                case "throughputChartContextMenu":
                    break;
                case "physicalIoChartContextMenu":
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

                    if (targetControl == latencyChart)
                    {
                        dialog.Select(Metric.AverageDiskMillisecondsPerRead);
                    }
                    else if (targetControl == throughputChart)
                    {
                        dialog.Select(Metric.DiskReadsPerSecond);
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
                if (targetControl == latencyChart)
                {
                    topic = HelpTopics.ServerDashboardViewDiskPanelLatency;
                }
                else if (targetControl == throughputChart)
                {
                    topic = HelpTopics.ServerDashboardViewDiskPanelThroughput;
                }
                else if (targetControl == physicalIoChart)
                {
                    topic = HelpTopics.ServerDashboardViewDiskPanelPhysicalIo;
                }
            }

            ShowHelp(topic);
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
                StringBuilder title = new StringBuilder(this.Caption);
                if (chart != physicalIoChart)
                {
                    title.Append(" ");
                    title.Append(selectedDisk);
                }
                title.Append(" ");
                title.Append(chart.Tag.ToString());

                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title.ToString(), ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                StringBuilder title = new StringBuilder(this.Caption);
                if (chart != physicalIoChart)
                {
                    title.Append(" ");
                    title.Append(selectedDisk);
                }
                title.Append(" ");
                title.Append(chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title.ToString(), true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                StringBuilder title = new StringBuilder(this.Caption);
                if (chart != physicalIoChart)
                {
                    title.Append(" ");
                    title.Append(selectedDisk);
                }
                title.Append(" ");
                title.Append(chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title.ToString(), ExportHelper.GetValidFileName(title.ToString(), true));
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
