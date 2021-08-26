using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class FileActivityControl : DashboardControl
    {
        private enum faChartType
        {
            [Description("Reads/sec")]
            Reads,
            [Description("Writes/sec")]
            Writes,
            [Description("Transfers/sec")]
            Transfers,
            //[Description("Reads KB/sec")]
            //ReadsKb,
            //[Description("Writes KB/sec")]
            //WritesKb,
            //[Description("Total KB/sec")]
            //TotalKb,
            //[Description("I/O Stall ms/sec")]
            //IoStall
        }

        private enum faDataFilter
        {
            [Description("All files")]
            All,
            [Description("Choose databases")]
            Database,
            [Description("Choose disks")]
            Disk,
            [Description("Choose files")]
            Files,
        }

        private enum faDataColumn
        {
            [Description("ReadsPerSecond")]
            Reads,
            [Description("WritesPerSecond")]
            Writes,
            [Description("TransfersPerSecond")]
            Transfers,
            //[Description("ReadsKbPerSec")]
            //ReadsKb,
            //[Description("WritesKbPerSec")]
            //WritesKb,
            //[Description("TransfersKbPerSec")]
            //TransfersKb,
            //[Description("IoStallMSPerSec")]
            //IoStall
        }

        private const string OPTIONTYPE_CHARTTYPE = @"ChartType";
        private const string OPTIONTYPE_FILTER = @"DataFilter";

        private const string chartTitleFormat = "{0} {1} {2}";
        private Control contextControl = null;

        private int selectedType = -1;
        private const string selectedTypeFormat = "Top {0}";
        private const string selectedDbFormat = "Database {0}";
        private const string selectedDiskFormat = "Disk {0}";
        private const string selectedFileFormat = "File {0}";
        private faDataFilter selectedFilterType = faDataFilter.All;
        private List<string> selectedFilterValues = new List<string>();
        private List<string> selectedFiles = new List<string>();
        private List<string> selectedFileNames = new List<string>();

        private DataTable top5DataTable;

        public FileActivityControl(): base(DashboardPanel.FileActivity)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(activityChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewFileActivityPanel;

            //Create datasource before initialize so SetStatus calls can reference it when setting options during initialization
            CreateChartDataSource();
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            headerSelectTypeDropDownButton.DropDownItems.Clear();

            foreach (faChartType type in Enum.GetValues(typeof(faChartType)))
            {
                ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(type), null, chartTypeSelectToolStripMenuItem_Click);
                chartTypeItem.Tag = (int)type;
                headerSelectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
            }

            if (selectedType < 0)
            {
                selectedType = (int) headerSelectTypeDropDownButton.DropDownItems[0].Tag;
            }
            if (Enum.IsDefined(typeof(faChartType), selectedType))
            {
                headerSelectTypeDropDownButton.Text = string.Format(selectedTypeFormat, headerSelectTypeDropDownButton.DropDownItems[selectedType].Text);
            }
            headerSelectTypeSeparator.Visible =
                headerSelectTypeDropDownButton.Visible = true;

            headerSelectOptionDropDownButton.DropDownItems.Clear();

            foreach (faDataFilter filter in Enum.GetValues(typeof(faDataFilter)))
            {
                ToolStripMenuItem chartfilterItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(filter), null, chartOptionSelectToolStripMenuItem_Click);
                headerSelectOptionDropDownButton.DropDownItems.Add(chartfilterItem);
                chartfilterItem.Tag = filter;
                if (filter != faDataFilter.All)
                {
                    chartfilterItem.Text += @"...";
                }
            }
            UpdateSelectButton();

            InitializeCharts();
        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                switch (option.Type)
                {
                    case OPTIONTYPE_CHARTTYPE:
                        foreach (string value in option.Values)
                        {
                            faChartType chartType;
                            if (Enum.TryParse(value, out chartType))
                            {
                                selectedType = (int) chartType;
                                UpdateStatus();
                                UpdateConfigOptions();
                                break;
                            }
                        }
                        break;
                    case OPTIONTYPE_FILTER:
                        bool hasType = false;
                        faDataFilter filter = faDataFilter.All;
                        foreach (string value in option.Values)
                        {
                            if (hasType)
                            {
                                selectedFilterType = filter;
                                selectedFilterValues.Add(value);
                            }
                            else if (Enum.TryParse(value, out filter))
                            {
                                hasType = true;
                            }
                        }
                        UpdateStatus();
                        UpdateConfigOptions();
                        break;
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> typeOptions = new List<string> { Enum.GetName(typeof(faChartType), selectedType) };
            List<string> filterOptions = new List<string> { Enum.GetName(typeof(faDataFilter), selectedFilterType) };
            filterOptions.AddRange(selectedFilterValues);
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption>
                                                       {
                                                           new DashboardPanelOption(OPTIONTYPE_CHARTTYPE, typeOptions),
                                                           new DashboardPanelOption(OPTIONTYPE_FILTER, filterOptions)
                                                       });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                headerSelectTypeDropDownButton.Text = string.Format(selectedTypeFormat, item.Text);
                if (item.Tag != null && item.Tag is int)
                {
                    selectedType = (int)item.Tag;
                    UpdateConfigOptions();
                    ConfigureDataSource();
                }
            }
        }

        private void chartOptionSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item.Tag != null && item.Tag is faDataFilter)
                {
                    faDataFilter filterType = (faDataFilter)item.Tag;
                    List<string> selectedObjects = new List<string>();
                    if (selectedFilterType == filterType)
                    {
                        selectedObjects.AddRange(selectedFilterValues);
                    }
                    if (filterType == faDataFilter.Database)
                    {
                        List<string> databases = GetDatabases();
                        if (databases.Count > 0)
                        {
                            SelectFileActivityObjectDialog dialog = new SelectFileActivityObjectDialog(SelectFileActivityObjectDialog.ObjectType.Database, databases, selectedObjects);
                            if (DialogResult.OK == dialog.ShowDialog(this))
                            {
                                selectedFilterType = filterType;
                                selectedFilterValues.Clear();
                                selectedFilterValues.AddRange(dialog.SelectedObjects);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowMessage(BaseView, "There are currently no databases available for selection.");
                        }
                    }
                    else if (filterType == faDataFilter.Disk)
                    {
                        List<string> drives = GetDrives();
                        if (drives.Count > 0)
                        {
                            SelectFileActivityObjectDialog dialog = new SelectFileActivityObjectDialog(SelectFileActivityObjectDialog.ObjectType.Disk, drives, selectedObjects);
                            if (DialogResult.OK == dialog.ShowDialog(this))
                            {
                                selectedFilterType = filterType;
                                selectedFilterValues.Clear();
                                selectedFilterValues.AddRange(dialog.SelectedObjects);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowMessage(BaseView, "There are currently no disks available for selection.");
                        }
                    }
                    else if (filterType == faDataFilter.Files)
                    {
                        List<string> files = GetFiles();
                        if (files.Count > 0)
                        {
                            SelectFileActivityObjectDialog dialog = new SelectFileActivityObjectDialog(SelectFileActivityObjectDialog.ObjectType.File, files, selectedObjects);
                            if (DialogResult.OK == dialog.ShowDialog(this))
                            {
                                selectedFilterType = filterType;
                                selectedFilterValues.Clear();
                                selectedFilterValues.AddRange(dialog.SelectedObjects);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowMessage(BaseView, "There are currently no files available for selection.");
                        }
                    }
                    else
                    {
                        selectedFilterType = filterType;
                        selectedFilterValues.Clear();
                    }
                    UpdateConfigOptions();
                    UpdateSelectButton();
                    ConfigureDataSource();
                }
            }
        }

        private void UpdateSelectButton()
        {
            int maxLength = 38;
            if (selectedFilterValues.Count > 0)
            {
                switch (selectedFilterType)
                {
                    case faDataFilter.Disk:
                        if (selectedFilterValues.Count > 1)
                        {
                            headerSelectOptionDropDownButton.Text = @"Multiple Disks Selected";
                        }
                        else
                        {
                            headerSelectOptionDropDownButton.Text = string.Format(selectedDiskFormat, selectedFilterValues[0].Substring(0, Math.Min(maxLength, selectedFilterValues[0].Length)));
                            headerSelectOptionDropDownButton.ToolTipText = selectedFilterValues[0];
                        }
                        break;
                    case faDataFilter.Database:
                        if (selectedFilterValues.Count > 1)
                        {
                            headerSelectOptionDropDownButton.Text = @"Multiple Databases Selected";
                        }
                        else
                        {
                            headerSelectOptionDropDownButton.Text = string.Format(selectedDbFormat, selectedFilterValues[0].Substring(0, Math.Min(maxLength, selectedFilterValues[0].Length)));
                            headerSelectOptionDropDownButton.ToolTipText = selectedFilterValues[0];
                        }
                        break;
                    case faDataFilter.Files:
                        if (selectedFilterValues.Count > 1)
                        {
                            headerSelectOptionDropDownButton.Text = @"Multiple Files Selected";
                        }
                        else
                        {
                            string fileName = selectedFilterValues[0];
                            headerSelectOptionDropDownButton.Text = string.Format(selectedFileFormat, fileName.Substring(0, Math.Min(maxLength, fileName.Length)));
                            headerSelectOptionDropDownButton.ToolTipText = fileName;
                        }
                        break;
                    default:
                        headerSelectOptionDropDownButton.Text = ApplicationHelper.GetEnumDescription(selectedFilterType);
                        headerSelectOptionDropDownButton.ToolTipText = string.Empty;
                        break;
                }
            }
            else
            {
                headerSelectOptionDropDownButton.Text = ApplicationHelper.GetEnumDescription(selectedFilterType);
                headerSelectOptionDropDownButton.ToolTipText = string.Empty;
            }
            headerSelectOptionSeparator.Visible =
                headerSelectOptionDropDownButton.Visible = true;
        }

        private List<string> GetDatabases()
        {
            OrderedSet<string> databases = new OrderedSet<string>();

            // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
            // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
            DataTable historyDataTable = historyData != null ? historyData.CurrentDbSnapshotsDataTable : null;
            if (historyDataTable != null)
            {
                DataView sourceView = historyDataTable.DefaultView;
                DataTable lastSnapshot = sourceView.Table.Clone();

                // go backwards through the rows
                if (sourceView.Count > 0)
                {
                    lastSnapshot.BeginLoadData();
                    DataRowView lastRow = sourceView[sourceView.Count - 1];
                    DateTime match = (DateTime) lastRow["Date"];

                    for (int i = sourceView.Count - 1; i >= 0; i--)
                    {
                        DataRowView row = sourceView[i];
                        if (match.Equals(row["Date"]))
                            lastSnapshot.ImportRow(row.Row);
                        else
                            break;
                    }
                    lastSnapshot.EndLoadData();

                    foreach (DataRowView row in lastSnapshot.DefaultView)
                    {
                        databases.Add((string) row["DatabaseName"]);
                    }
                }
            }

            return databases.ToList();
        }

        private List<string> GetDrives()
        {
            List<string> drives = new List<string>();

            var driveList = ChartHelper.GetOrderedDiskDrives(historyData);

            if (driveList != null)
            {
                foreach (var drive in driveList.Keys)
                {
                    drives.Add(drive);
                }
            }

            return drives;
        }

        private List<string> GetFiles()
        {
            OrderedSet<string> files = new OrderedSet<string>();

            // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
            // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
            DataTable historyDataTable = historyData != null ? historyData.CurrentFileSnapshotsDataTable : null;
            if (historyDataTable != null)
            {
                DataView sourceView = historyDataTable.DefaultView;
                DataTable lastSnapshot = sourceView.Table.Clone();

                // go backwards through the rows
                if (sourceView.Count > 0)
                {
                    lastSnapshot.BeginLoadData();
                    DataRowView lastRow = sourceView[sourceView.Count - 1];
                    DateTime match = (DateTime)lastRow["Date"];

                    for (int i = sourceView.Count - 1; i >= 0; i--)
                    {
                        DataRowView row = sourceView[i];
                        if (match.Equals(row["Date"]))
                            lastSnapshot.ImportRow(row.Row);
                        else
                            break;
                    }
                    lastSnapshot.EndLoadData();

                    foreach (DataRowView row in lastSnapshot.DefaultView)
                    {
                        files.Add((string)row["FilePath"]);
                    }
                }
            }

            return files.ToList();
        }

        private void CreateChartDataSource()
        {

            top5DataTable = new DataTable();
            top5DataTable.Columns.Add("Date", typeof(DateTime));
            top5DataTable.Columns.Add("ReadsPerSecond1", typeof(double));
            top5DataTable.Columns.Add("WritesPerSecond1", typeof(double));
            top5DataTable.Columns.Add("TransfersPerSecond1", typeof(double));
            top5DataTable.Columns.Add("ReadsPerSecond2", typeof(double));
            top5DataTable.Columns.Add("WritesPerSecond2", typeof(double));
            top5DataTable.Columns.Add("TransfersPerSecond2", typeof(double));
            top5DataTable.Columns.Add("ReadsPerSecond3", typeof(double));
            top5DataTable.Columns.Add("WritesPerSecond3", typeof(double));
            top5DataTable.Columns.Add("TransfersPerSecond3", typeof(double));
            top5DataTable.Columns.Add("ReadsPerSecond4", typeof(double));
            top5DataTable.Columns.Add("WritesPerSecond4", typeof(double));
            top5DataTable.Columns.Add("TransfersPerSecond4", typeof(double));
            top5DataTable.Columns.Add("ReadsPerSecond5", typeof(double));
            top5DataTable.Columns.Add("WritesPerSecond5", typeof(double));
            top5DataTable.Columns.Add("TransfersPerSecond5", typeof(double));

//            top5DataTable.PrimaryKey = new DataColumn[] { top5DataTable.Columns["Date"] };

            top5DataTable.DefaultView.Sort = "Date";
        }

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            ChartFxExtensions.SetContextMenu(activityChart, toolbarsManager);

            InitializeActivityChart();
            ConfigureActivityChart();
            InitalizeDrilldown(activityChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeActivityChart()
        {
            activityChart.Printer.Orientation = PageOrientation.Landscape;
            activityChart.Printer.Compress = true;
            activityChart.Printer.ForceColors = true;
            activityChart.Printer.Document.DocumentName = "File Activity Chart";
            activityChart.ToolTipFormat = string.Format("%s\n%v {0}\n%x", headerSelectTypeDropDownButton.Text.Substring(4));
        }

        private void ConfigureActivityChart()
        {
            activityChart.SuspendLayout();
            activityChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            activityChart.DataSourceSettings.Fields.Add(dateFieldMap);

            string selectedCol = ApplicationHelper.GetEnumDescription((faDataColumn)selectedType);
            if (selectedFiles.Count > 0)
            {
                for (int idx = 1; idx <= selectedFiles.Count; idx++)
                {
                    string fileName = selectedFileNames[idx - 1];
                    FieldMap fieldMap = new FieldMap(selectedCol + idx, FieldUsage.Value);
                    fieldMap.DisplayName = fileName;
                    activityChart.DataSourceSettings.Fields.Add(fieldMap);
                }

                activityChart.DataSourceSettings.ReloadData();
                activityChart_Resize(activityChart, new EventArgs());
                int showDecimals = 2; // (selectedType == (int)faDataColumn.IoStall) ? 0 : 2;
                activityChart.AxisY.DataFormat.Decimals = showDecimals;
                activityChart.ToolTipFormat = string.Format("%s\n%v {0}\n%x", headerSelectTypeDropDownButton.Text.Substring(4));
                activityChart.Invalidate();
            }
            activityChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            activityChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            activityChart.ResumeLayout();
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                try
                {
                    top5DataTable.BeginLoadData();
                    top5DataTable.Rows.Clear();

                    // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
                    // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                    DataTable historyDataTable = historyData != null ? historyData.CurrentFileSnapshotsDataTable : null;
                    if (historyDataTable != null)
                    { 
                        string selectedCol = ApplicationHelper.GetEnumDescription((faDataColumn)selectedType);

                        //historyData.
                        DataView sourceView = historyDataTable.DefaultView;
                        DataTable lastSnapshot = sourceView.Table.Clone();

                        // go backwards through the rows
                        if (sourceView.Count > 0)
                        {
                            lastSnapshot.BeginLoadData();
                            DataRowView lastRow = sourceView[sourceView.Count - 1];
                            DateTime match = (DateTime)lastRow["Date"];

                            for (int i = sourceView.Count - 1; i >= 0; i--)
                            {
                                DataRowView row = sourceView[i];
                                if (match.Equals(row["Date"]))
                                    lastSnapshot.ImportRow(row.Row);
                                else
                                    break;
                            }
                            lastSnapshot.EndLoadData();
                        }

                        string rowFilter;
                        switch (selectedFilterType)
                        {
                            case faDataFilter.Disk:
                                List<string> disks = selectedFilterValues.Select(disk => disk.Replace("'", "''")).ToList();
                                rowFilter = string.Format("DriveName in ('{0}')", string.Join("','", disks));
                                break;
                            case faDataFilter.Database:
                                List<string> dbs = selectedFilterValues.Select(db => db.Replace("'", "''")).ToList();
                                rowFilter = string.Format("DatabaseName in ('{0}')", string.Join("','", dbs));
                                break;
                            case faDataFilter.Files:
                                List<string> files = selectedFilterValues.Select(file => file.Replace("'", "''")).ToList();
                                rowFilter = string.Format("FilePath in ('{0}')", string.Join("','", files));
                                break;
                            default:
                                rowFilter = string.Empty;
                                break;
                        }
                        lastSnapshot.DefaultView.RowFilter = rowFilter;

                        // sort descending by selected column
                        lastSnapshot.DefaultView.Sort = string.Format("{0} desc", selectedCol);

                        selectedFiles.Clear();
                        selectedFileNames.Clear();
                        foreach (DataRowView row in lastSnapshot.DefaultView)
                        {
                            selectedFiles.Add((string)row["FilePath"]);
                            selectedFileNames.Add((string)row["FileName"]);
                            if (selectedFiles.Count == 5)
                                break;
                        }

                        if (selectedFiles.Count > 0)
                        {
                            // update the databases chart
                            StringBuilder fileItems = new StringBuilder();
                            foreach (string file in selectedFiles)
                            {
                                if (fileItems.Length > 0)
                                    fileItems.Append(",");
                                fileItems.AppendFormat("'{0}'", file.Replace("'", "''"));
                            }

                            string viewFilter = string.Format("FilePath in ({0})", fileItems);
                            if (historyData.HistoricalSnapshotDateTime == null)
                            {
                                DateTime dateLimit = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                                viewFilter = String.Format("Date > #{0:o}# AND {1}", dateLimit, viewFilter);
                            }

                            DataView currentData = new DataView(historyDataTable,
                                                                viewFilter,
                                                                "Date",
                                                                DataViewRowState.CurrentRows);

                            DateTime top5RowDate = DateTime.MinValue;
                            DataRow top5Row = null;

                            foreach (DataRowView row in currentData)
                            {
                                DateTime date = (DateTime)row["Date"];
                                if (top5Row == null || date != top5RowDate)
                                {
                                    top5Row = top5DataTable.NewRow();
                                    top5Row["Date"] = date;
                                    top5DataTable.Rows.Add(top5Row);
                                    top5RowDate = date;
                                }

                                int fileIndex = selectedFiles.IndexOf(row["FilePath"].ToString()) + 1;

                                foreach (faDataColumn col in Enum.GetValues(typeof(faDataColumn)))
                                {
                                    try
                                    {
                                        string columnName = ApplicationHelper.GetEnumDescription(col);
                                        if (currentData.Table.Columns.Contains(columnName))
                                            top5Row[columnName + fileIndex] = row[columnName];
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 selections to display. ", ex);
                }
                finally
                {
                    top5DataTable.EndLoadData();
                }

                activityChart.DataSource = top5DataTable;
                ChartFxExtensions.SetAxisXTimeScale(activityChart, 2);
                ConfigureActivityChart();

                UpdateStatus();

                Invalidate();
            }
        }

        private void UpdateStatus()
        {
            if (top5DataTable.Rows.Count > 0)
            {
                switch (selectedFilterType)
                {
                    case faDataFilter.All:
                        headerStatusLabel.Visible = false;
                        break;
                    case faDataFilter.Disk:
                        headerStatusLabel.ToolTipText = string.Format("Disks selected for displaying files:\r\n  {0}",
                                                                      string.Join("\r\n  ", selectedFilterValues));
                        headerStatusLabel.Image = Properties.Resources.StatusInfoSmall;
                        headerStatusLabel.Visible = true;
                        break;
                    case faDataFilter.Database:
                        headerStatusLabel.ToolTipText = string.Format("Databases selected for displaying files:\r\n  {0}",
                                                                      string.Join("\r\n  ", selectedFilterValues));
                        headerStatusLabel.Image = Properties.Resources.StatusInfoSmall;
                        headerStatusLabel.Visible = true;
                        break;
                    case faDataFilter.Files:
                        headerStatusLabel.ToolTipText = string.Format("Files selected for display:\r\n  {0}",
                                                                      string.Join("\r\n  ", selectedFilterValues));
                        headerStatusLabel.Image = Properties.Resources.StatusInfoSmall;
                        headerStatusLabel.Visible = true;
                        break;
                }
                statusLinkLabel.SendToBack();
                statusLinkLabel.Visible = false;
            }
            else
            {
                statusLinkLabel.Visible = true;
                statusLinkLabel.BringToFront();
                headerStatusLabel.ToolTipText = @"No files found for display.";
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
                //try
                //{
                //    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);
                //    dialog.ShowDialog(ParentForm);
                //}
                //catch (Exception ex)
                //{
                //    ApplicationMessageBox.ShowError(this,
                //                                    "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                //                                    ex);
                //}
            }
        }

        private void ShowControlHelp(Control targetControl)
        {
            string topic = string.Empty;

            if (targetControl != null)
            {
                if (targetControl == activityChart)
                {
                    if (selectedType == (int)faChartType.Reads)
                    {
                        topic = HelpTopics.ServerDashboardViewFileActivityPanelReads;
                    }
                    else if (selectedType == (int)faChartType.Writes)
                    {
                        topic = HelpTopics.ServerDashboardViewFileActivityPanelWrites;
                    }
                    else if (selectedType == (int)faChartType.Transfers)
                    {
                        topic = HelpTopics.ServerDashboardViewFileActivityPanelTransfers;
                    }
                }
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesFileActivity);
            }
        }

        private void PrintChart(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, chart.Tag, headerSelectTypeDropDownButton.Text, headerSelectOptionDropDownButton.Text);

                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, chart.Tag, headerSelectTypeDropDownButton.Text, headerSelectOptionDropDownButton.Text);
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, chart.Tag, headerSelectTypeDropDownButton.Text, headerSelectOptionDropDownButton.Text);
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

        private void activityChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 3;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }

        private void statusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowHelp(HelpTopics.ServerDashboardViewFileActivityPanelNoData);
        }
    }
}
