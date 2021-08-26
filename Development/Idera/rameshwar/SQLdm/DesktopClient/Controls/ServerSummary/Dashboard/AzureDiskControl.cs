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

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class AzureDiskControl : DashboardControl
    {
        private const string OPTIONTYPE_DISK = @"AzureDisk";

        private Control contextControl = null;

        private DataTable emptyDiskTable;

        public AzureDiskControl() : base(DashboardPanel.AzureDisk)
        {
            InitializeComponent();

            helpTopic = HelpTopics.ServerDashboardViewAzureDiskPanel;
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            //InitializeDiskSelection();
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
                        UpdateConfigOptions();
                        //ConfigureDiskSelection();
                    }
                    break;
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = new List<string> { "Azure" };
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_DISK, options) });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (headerSelectTypeDropDownButton.Tag.ToString() != item.Text)
                {
                    //setDiskSelection(item.Text);
                    UpdateConfigOptions();
                    ConfigureDataSource();
                }
            }
        }

        //private void setDiskSelection(string disk)
        //{
        //    if (disk != null)
        //    {
        //        headerSelectTypeDropDownButton.Text = disk.Length > 60
        //                                                  ? string.Format("{0}...", disk.Substring(0, 60))
        //                                                  : disk;
        //        headerSelectTypeDropDownButton.ToolTipText = disk;
        //        headerSelectTypeDropDownButton.Tag = disk;
        //    }
        //}

        //private void InitializeDiskSelection()
        //{
        //    ConfigureDiskSelection();

        //    headerSelectTypeSeparator.Visible =
        //        headerSelectTypeDropDownButton.Visible = true;
        //}

        //private void ConfigureDiskSelection()
        //{
        //    headerSelectTypeDropDownButton.DropDownItems.Clear();
        //    if (historyData != null)
        //    {
               
                   

        //            setDiskSelection((headerSelectTypeDropDownButton.DropDownItems.Count == 0)
        //                                 ? selectedDisk
        //                                 : headerSelectTypeDropDownButton.DropDownItems[0].Text);
             
        //    }
        //    headerSelectTypeDropDownButton.Visible = headerSelectTypeDropDownButton.DropDownItems.Count > 0;
        //    UpdateStatus();
        //}

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            ChartFxExtensions.SetContextMenu(usageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(rateChart, toolbarsManager);

            //create a dummy datasource for the disk charts to initialize with until a disk drive exists
            emptyDiskTable = new DataTable();
            emptyDiskTable.Columns.Add("Date", typeof(DateTime));
            emptyDiskTable.Columns.Add(historyData.DataIOUsage, typeof(double));
            emptyDiskTable.Columns.Add(historyData.LogIOUsage, typeof(double));
            emptyDiskTable.Columns.Add(historyData.DataIORate, typeof(double));
            emptyDiskTable.Columns.Add(historyData.LogIORate, typeof(double));

            //emptyDiskTable.Columns.Add(historyData.DiskTimePerReadPerDiskColumnPrefix, typeof(int));
            //emptyDiskTable.Columns.Add(historyData.DiskTimePerWritePerDiskColumnPrefix, typeof(int));
            //emptyDiskTable.Columns.Add(historyData.DiskReadsPerSecPerDiskColumnPrefix, typeof(int));
            //emptyDiskTable.Columns.Add(historyData.DiskWritesPerSecPerDiskColumnPrefix, typeof(int));
            DataRow row = emptyDiskTable.NewRow();
            row["Date"] = DateTime.Now;
            row[historyData.DataIOUsage] = 0;
            row[historyData.LogIOUsage] = 0;
            row[historyData.DataIORate] = 0;
            row[historyData.LogIORate] = 0;

            emptyDiskTable.Rows.Add(row);
            InitializeUsageChart();
            InitializeRateChart();
            InitalizeDrilldown(usageChart, rateChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeUsageChart()
        {
            usageChart.Printer.Compress = true;
            usageChart.Printer.ForceColors = true;
            usageChart.Printer.Document.DocumentName = "IO Usage Chart";
            usageChart.ToolTipFormat = "%vms %s\n%x";
            usageChart.AxisY.Title.Text = "% of Limit";

            ConfigureUsageChart();
        }

        private void ConfigureUsageChart()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            FieldMap dataFieldMap = new FieldMap(historyData.DataIOUsage, FieldUsage.Value);
            dataFieldMap.DisplayName = "Data";
            FieldMap logFieldMap = new FieldMap(historyData.LogIOUsage, FieldUsage.Value);
            logFieldMap.DisplayName = "Log";

            usageChart.DataSourceSettings.Fields.Clear();
            usageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            dataFieldMap,
                                                            logFieldMap
                                                        });

            int showDecimals = 2;
            usageChart.AxisY.DataFormat.Decimals = showDecimals;
            usageChart.ToolTipFormat = string.Format("Usage \n%v ms %s\n%x");

            usageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            usageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ChartFxExtensions.SetAxisXTimeScale(usageChart, 2);
        }

        private void InitializeRateChart()
        {
            rateChart.Printer.Compress = true;
            rateChart.Printer.ForceColors = true;
            rateChart.Printer.Document.DocumentName = "IO Rate Chart";
            rateChart.ToolTipFormat = "%v %s\\sec\n%x";
            rateChart.AxisY.Title.Text = "MB/Sec";

            ConfigureRateChart();
        }

        private void ConfigureRateChart()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);

            FieldMap dataFieldMap = new FieldMap(historyData.DataIORate, FieldUsage.Value);
            dataFieldMap.DisplayName = "Data";
            FieldMap logFieldMap = new FieldMap(historyData.LogIORate, FieldUsage.Value);
            logFieldMap.DisplayName = "Log";

            rateChart.DataSourceSettings.Fields.Clear();
            rateChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                        {
                                                            dateFieldMap,
                                                            dataFieldMap,
                                                            logFieldMap
                                                        });

            int showDecimals = 2;
            rateChart.AxisY.DataFormat.Decimals = showDecimals;
            rateChart.ToolTipFormat = string.Format("Rate\n%v %s/sec\n%x");
            

            foreach (SeriesAttributes series in rateChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
            }

            rateChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            rateChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ChartFxExtensions.SetAxisXTimeScale(rateChart, 2);
        }


        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {

                //ConfigureDiskSelection();
                UpdateAlerts();

                DataTable azureDiskDataSource = historyData.CurrentAzureDiskDataTable;
                                              

                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    DateTime viewFilter = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                    historyData.RealTimeAzureDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                    historyData.RealTimeAzureDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                }

                usageChart.SuspendLayout();
                ConfigureUsageChart();
                usageChart.DataSource = azureDiskDataSource;
                usageChart.ResumeLayout();

                rateChart.SuspendLayout();
                ConfigureRateChart();
                rateChart.DataSource = azureDiskDataSource;
                rateChart.ResumeLayout();


                Invalidate();
            }
        }


     

               //private void UpdateStatus()
        //{
        //    //if (IsDiskValid())
        //    //{
        //    //    headerStatusLabel.Visible = false;
        //    //}
        //    //else
        //    //{
        //    //    headerStatusLabel.ToolTipText = string.IsNullOrEmpty(selectedDisk)
        //    //                                        ? @"No valid disk selection was available."
        //    //                                        : string.Format("The selected disk '{0}' does not exist on this server.",
        //    //                                                        selectedDisk);
        //    //    headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
        //    //    headerStatusLabel.Visible = true;
        //    //}
        //}

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
                case "usageChartContextMenu":
                    break;
                case "rateChartContextMenu":
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

                    if (targetControl == usageChart)
                    {
                        dialog.Select(Metric.AverageDiskMillisecondsPerRead);
                    }
                    else if (targetControl == rateChart)
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
                if (targetControl == usageChart)
                {
                    topic = HelpTopics.ServerDashboardViewAzureDiskPanelUsage;
                }
                else if (targetControl == rateChart)
                {
                    topic = HelpTopics.ServerDashboardViewAzureDiskPanelRate;
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
