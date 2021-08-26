using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Objects;

using Infragistics.Windows.Themes;
using BBS.TracerX;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupDetailsView : UserControl
    {
        private DataTable detailsGridDataSource;
        private DataTable lastUpdateDataTable = null;
        private UltraGridColumn selectedColumn = null;
        private ServerGroupView parentView;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ServerGroupDetailsView");
        private object view;
        private static readonly Logger Log = Logger.GetLogger("ServerGroupDetailsView");

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings = null;

        public ServerGroupDetailsView()
        {
            InitializeComponent();
            detailsGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            // load value lists for grid display
            ValueListItem listItem;
            //START: 4.11 part B Add columns in grid :Babita Manral
            ValueList valueList = detailsGrid.DisplayLayout.ValueLists["statusValueList"];
            valueList.ValueListItems.Clear();
            listItem = new ValueListItem(State.OK, "OK");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(State.Warning, "Warning");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(State.Critical, "Critical");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            valueList.ValueListItems.Add(listItem);
            //END: 4.11 part B Add columns in grid :Babita Manral

            // populate the severity value list
            valueList = detailsGrid.DisplayLayout.ValueLists["severityValueList"];
            valueList.ValueListItems.Clear();
            //valueList.ValueListItems.AddRange(ValueListHelpers.GetSeverityValueListItems());
            listItem = new ValueListItem(MonitoredState.OK, "OK");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(MonitoredState.Warning, "Warning");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(MonitoredState.Critical, "Critical");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem("MM", "Maintenance Mode");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceMode16x16;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem("Refreshing", "Refreshing");
            listItem.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            valueList.ValueListItems.Add(listItem);

            // populate the service state value list
            valueList = detailsGrid.DisplayLayout.ValueLists["serviceStateValueList"];
            valueList.ValueListItems.Clear();
            listItem = new ValueListItem(ServiceState.Running, "Running");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.Paused, "Paused");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.Stopped, "Stopped");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.NotInstalled, "Not Installed");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.UnableToMonitor, "Unknown");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(-1, "Initializing...");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(-2, "Refreshing...");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(-3, "Integrated");
            valueList.ValueListItems.Add(listItem);

            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;

            lblNoSqlServers.Text = "There are no items to show in this view.";
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            ApplicationController.Default.RefreshActiveView();
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                //
                // Column context menu
                //
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;

                //
                // Instance context menu
                //
                case "openInstanceButton":
                    OpenSelectedInstance();
                    break;
                case "refreshInstanceButton":
                    RefreshSelectedInstance();
                    break;
                case "deleteInstanceButton":
                    DeleteSelectedInstance();
                    break;
                case "showInstancePropertiesButton":
                    ShowSelectedInstanceProperties();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            toolbarsManager.Tools["deleteInstanceButton"].SharedProps.Visible =
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            if (e.Tool.Key == "instanceContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = detailsGrid.Rows.Count > 0 && detailsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "instanceContextMenu")
            {
                bool enableRefreshButton = true;
                int selectedInstanceId = GetSelectedInstanceId();
                if (selectedInstanceId != -1)
                {
                    MonitoredSqlServerWrapper selectedInstance = ApplicationModel.Default.ActiveInstances[selectedInstanceId];
                    if (selectedInstance != null)
                    {
                        if (selectedInstance.IsRefreshing)
                            enableRefreshButton = false;
                    }
                }
                ButtonTool refreshButton = (ButtonTool)toolbarsManager.Tools["refreshInstanceButton"];
                refreshButton.SharedProps.Enabled = enableRefreshButton;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(detailsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        public void Initialize(ServerGroupView parent, object initView)
        {
            parentView = parent;
            view = initView;
        }

        public void ApplySettings()
        {
            if (Settings.Default.ServerGroupDetailsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServerGroupDetailsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, detailsGrid);
            }
        }

        public void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(detailsGrid);
            // save all settings only if anything has changed
            if (!mainGridSettings.Equals(lastMainGridSettings))
            {
                lastMainGridSettings =
                    Settings.Default.ServerGroupDetailsViewMainGrid = mainGridSettings;
            }
        }

        public void UpdateData(int instanceId, DataTable dataTable)
        {
            DataRow existingRow = detailsGridDataSource.Rows.Find(instanceId);
            if (existingRow == null)
                return; // shouldn't ever be the case

            detailsGrid.SuspendLayout();
            detailsGridDataSource.BeginLoadData();

            DataTable lastCollectionTable = dataTable.Clone();
            lastCollectionTable.Columns["UTCCollectionDateTime"].AllowDBNull = true;

            DataRow[] dataRows = dataTable.Select(string.Format("SQLServerID = {0}", instanceId));
            if (dataRows.Length > 0)
            {
                lastCollectionTable.ImportRow(dataRows[dataRows.Length - 1]);
            }
            else
            {
                if (detailsGridDataSource != null && detailsGridDataSource.Rows.Find(instanceId) == null)
                {
                    DataRow nullInstanceRow = lastCollectionTable.NewRow();
                    nullInstanceRow["SQLServerID"] = instanceId;
                    nullInstanceRow["InstanceName"] = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
                    lastCollectionTable.Rows.Add(nullInstanceRow);
                }
            }
            lastCollectionTable.Columns.Add("Severity", typeof(string));
            lastCollectionTable.Columns.Add("Status", typeof(string));

            if (dataRows.Length > 0)
            {
                foreach (DataColumn col in lastCollectionTable.Columns)
                {
                    existingRow[col.ColumnName] = lastCollectionTable.Rows[0][col.ColumnName];
                }
                AdjustValues(existingRow);
            }

            detailsGridDataSource.EndLoadData();
            detailsGrid.ResumeLayout();

            DisplayNoSqlServersLabel(detailsGrid.Rows.Count > 0);
        }

        public void UpdateData(DataTable dataTable)
        {
            if (!IsDisposed)
            {
                if (dataTable == null)
                {
                    throw new ArgumentNullException("dataTable");
                }

                lastUpdateDataTable = dataTable;

                List<int> instances = new List<int>();
                if (view == null)
                {
                    foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
                    {
                        instances.Add(instance.Id);
                    }
                }
                else if (view is UserView)
                {
                    instances = new List<int>(((UserView)view).Instances);
                }
                else if (view is int)
                {
                    int tagId = (int)view;

                    if (ApplicationModel.Default.Tags.Contains(tagId))
                    {
                        Tag tag = ApplicationModel.Default.Tags[tagId];
                        instances = new List<int>(tag.Instances);
                    }
                }

                DataTable lastCollectionTable = dataTable.Clone();
                lastCollectionTable.Columns["UTCCollectionDateTime"].AllowDBNull = true;

                foreach (int instanceId in instances)
                {
                    DataRow[] dataRows = dataTable.Select(string.Format("SQLServerID = {0}", instanceId));

                    if (dataRows.Length > 0)
                    {
                        lastCollectionTable.ImportRow(dataRows[dataRows.Length - 1]);
                    }
                    else
                    {
                        if (detailsGridDataSource != null
                            && detailsGridDataSource.Rows.Find(instanceId) == null
                            && ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                        {
                            DataRow nullInstanceRow = lastCollectionTable.NewRow();
                            nullInstanceRow["SQLServerID"] = instanceId;
                            nullInstanceRow["InstanceName"] =
                                ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
                            lastCollectionTable.Rows.Add(nullInstanceRow);
                        }
                        else
                        {
                            LOG.WarnFormat("The Grid Data Source is null, or the instance ID in Grid Data Source is null or the provided instance id {0} is not contained in the monitored instances.", instanceId);
                        }
                    }
                }
                lastCollectionTable.Columns.Add("Severity", typeof(string));
                lastCollectionTable.Columns.Add("Status", typeof(string));

                if (detailsGrid.DataSource == null)
                {
                    detailsGridDataSource = lastCollectionTable;
                    DataColumn[] keys = new DataColumn[1];
                    keys[0] = detailsGridDataSource.Columns["SQLServerID"];
                    detailsGridDataSource.PrimaryKey = keys;
                    AdjustValues();

                    detailsGrid.DataSource = detailsGridDataSource;
                    foreach (UltraGridColumn column in detailsGrid.DisplayLayout.Bands[0].Columns)
                    {
                        if (column.Key != "Severity" && column.Key != "Status")
                        {
                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                        }
                    }
                }
                else
                {
                    detailsGrid.SuspendLayout();
                    detailsGridDataSource.BeginLoadData();

                    //remove any servers from the grid that are no longer monitored
                    List<DataRow> deleteList = new List<DataRow>();
                    foreach (DataRow row in detailsGridDataSource.Rows)
                    {
                        if (!instances.Contains((int)row["SQLServerID"]))
                        {
                            deleteList.Add(row);
                        }
                    }
                    foreach (DataRow row in deleteList)
                    {
                        detailsGridDataSource.Rows.Remove(row);
                    }

                    //update values on each server
                    foreach (DataRow row in lastCollectionTable.Rows)
                    {
                        DataRow existingRow = detailsGridDataSource.Rows.Find((int)row["SQLServerID"]);
                        bool newRow = false;
                        if (existingRow == null)
                        {
                            newRow = true;
                            existingRow = detailsGridDataSource.NewRow();
                        }
                        foreach (DataColumn col in lastCollectionTable.Columns)
                        {
                            existingRow[col.ColumnName] = row[col.ColumnName];
                        }
                        if (newRow)
                        {
                            detailsGridDataSource.Rows.Add(existingRow);
                        }
                    }

                    AdjustValues();

                    detailsGridDataSource.EndLoadData();
                    detailsGrid.ResumeLayout();
                }

                foreach (UltraGridRow gridRow in detailsGrid.Rows.GetAllNonGroupByRows())
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    int instanceId = (int)dataRow["SQLServerID"];

                    ApplyCellColors(instanceId, gridRow);
                }

                ApplicationController.Default.SetCustomStatus(String.Format("{0} Item{1}",
                                                                            detailsGridDataSource.Rows.Count,
                                                                            detailsGridDataSource.Rows.Count == 1
                                                                                ? string.Empty
                                                                                : "s")
                    );

                DisplayNoSqlServersLabel(detailsGrid.Rows.Count > 0);
            }
        }

        /// <summary>
        /// Display a label to indicate that there are not have items to show in the view
        /// </summary>
        /// <param name="isEnable">Indicates if the label will display or not.</param>
        private void DisplayNoSqlServersLabel(bool isEnable)
        {
            if (isEnable)
            {
                lblNoSqlServers.Visible = false;
                lblNoSqlServers.SendToBack();
            }
            else
            {
                lblNoSqlServers.Visible = true;
                lblNoSqlServers.BringToFront();
            }
        }

        private void AdjustValues()
        {
            //update the severity of each server and adjust values for display
            foreach (DataRow row in detailsGridDataSource.Rows)
            {
                AdjustValues(row);
            }
        }

        private void AdjustValues(DataRow row)
        {
            FileSize fs;
            bool v10 = false;

            int instanceId = (int)row["SQLServerID"];
            MonitoredSqlServerStatus instanceStatus =
                ApplicationModel.Default.GetInstanceStatus(instanceId);
            if (instanceStatus != null)
            {
                string status = instanceStatus.ToolTip;
                if (String.IsNullOrEmpty(status))
                    status = instanceStatus.ToolTipHeading;

                row["Severity"] = instanceStatus.IsInMaintenanceMode ? (object)"MM" : instanceStatus.Severity;
                row["Status"] = status;
                if (instanceStatus.Instance.IsRefreshing)
                {
                    row["SqlServerServiceStatus"] = -2;
                    row["Severity"] = "Refreshing";
                }
                if (instanceStatus.Severity <= MonitoredState.OK && !instanceStatus.IsRefreshAvailable)
                {
                    row["SqlServerServiceStatus"] = -1;
                }
                else if (row["SqlServerServiceStatus"] == DBNull.Value)
                {
                    row["SqlServerServiceStatus"] = ServiceState.UnableToMonitor;
                }
            }
            else if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
                if (wrapper == null)
                    row["Severity"] = MonitoredState.OK;
                else
                    row["Severity"] = wrapper.MaintenanceModeEnabled ? (object)"MM" : MonitoredState.OK;

                row["Status"] = "";
                row["SqlServerServiceStatus"] = -1;
            }
            if (row["ServerVersion"] is string)
            {
                ServerVersion version = new ServerVersion((string)row["ServerVersion"]);
                row["ServerVersion"] = version.DisplayVersion;
                v10 = version.Major >= 10;
            }
            if (row["OSAvailableMemoryInKilobytes"] is long)
            {
                fs = new FileSize((long)row["OSAvailableMemoryInKilobytes"]);
                row["OSAvailableMemoryInKilobytes"] = fs.Megabytes;
            }

            if (row["OSTotalPhysicalMemoryInKilobytes"] is long)
            {
                fs = new FileSize((long)row["OSTotalPhysicalMemoryInKilobytes"]);
                row["OSTotalPhysicalMemoryInKilobytes"] = fs.Megabytes;
            }
            if (v10 && row["FullTextSearchStatus"] is int)
            {
                row["FullTextSearchStatus"] = -3;  // hackaroo
            }
            //START: 4.11 part B Add columns in grid :Babita Manral
            if (row.Table.Columns.Contains("Alert") && row["Alert"] != null && row["Alert"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["Alert"].Header.VisiblePosition = 3;
                int metricValue = (int)row["Alert"];
                row["Alert"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("CPU") && row["CPU"] != null && row["CPU"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["CPU"].Header.VisiblePosition = 4;
                int metricValue = (int)row["CPU"];
                row["CPU"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("Memory") && row["Memory"] != null && row["Memory"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["Memory"].Header.VisiblePosition = 5;
                int metricValue = (int)row["Memory"];
                row["Memory"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("I/O") && row["I/O"] != null && row["I/O"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["I/O"].Header.VisiblePosition = 6;
                int metricValue = (int)row["I/O"];
                row["I/O"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("Database") && row["Database"] != null && row["Database"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["Database"].Header.VisiblePosition = 7;
                int metricValue = (int)row["Database"];
                row["Database"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("Logs") && row["Logs"] !=null && row["Logs"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["Logs"].Header.VisiblePosition = 8;
                int metricValue = (int)row["Logs"];
                row["Logs"] = GetMetricStatus(metricValue);
            }
            if (row.Table.Columns.Contains("Queries") && row["Queries"] != null && row["Queries"] is int)
            {
                detailsGrid.DisplayLayout.Bands[0].Columns["Queries"].Header.VisiblePosition = 9;
                int metricValue = (int)row["Queries"];
                row["Queries"] = GetMetricStatus(metricValue);
            }
        }

        private int GetMetricStatus(int value)
        {
            int status = (int)State.OK;
            try
            {
                if (value == 8)
                    status = (int)State.Critical;
                else if (value == 4)
                    status = (int)State.Warning;
                else if (value == 2)
                    status = (int)State.OK;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return status;
        }
        //END: 4.11 part B Add columns in grid :Babita Manral
        private void ApplyCellColors(int instanceId, UltraGridRow gridRow)
        {
            AlertConfiguration alertConfig = ApplicationModel.Default.ActiveInstances[instanceId].AlertConfiguration;
            //AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
            if (alertConfig != null)
            {
                UpdateCellColor(Metric.AgentServiceStatus, alertConfig, gridRow, "AgentServiceStatus", 1);
                UpdateCellColor(Metric.DtcServiceStatus, alertConfig, gridRow, "DTCServiceStatus", 1);
                UpdateCellColor(Metric.FullTextServiceStatus, alertConfig, gridRow, "FullTextSearchStatus", 1);
                UpdateCellColor(Metric.SqlServiceStatus, alertConfig, gridRow, "SqlServerServiceStatus", 1);
                UpdateCellColor(Metric.ServerResponseTime, alertConfig, gridRow, "ResponseTimeInMilliseconds", 1);
                UpdateCellColor(Metric.OSCPUProcessorQueueLength, alertConfig, gridRow, "ProcessorQueueLength", 1);
                UpdateCellColor(Metric.OSMemoryPagesPerSecond, alertConfig, gridRow, "PagesPerSecond", 1);
                UpdateCellColor(Metric.OSDiskAverageDiskQueueLength, alertConfig, gridRow, "DiskQueueLength", 1);
                UpdateCellColor(Metric.OldestOpenTransMinutes, alertConfig, gridRow, "OldestOpenTransactionsInMinutes", 1);
                UpdateCellColor(Metric.SQLCPUUsagePct, alertConfig, gridRow, "CPUActivityPercentage", 1);
                UpdateCellColor(Metric.OSUserCPUUsagePct, alertConfig, gridRow, "UserTimePercent", 1);
                UpdateCellColor(Metric.OSCPUUsagePct, alertConfig, gridRow, "ProcessorTimePercent", 1);
                UpdateCellColor(Metric.OSCPUPrivilegedTimePct, alertConfig, gridRow, "PrivilegedTimePercent", 1);
                UpdateCellColor(Metric.OSDiskPhysicalDiskTimePct, alertConfig, gridRow, "DiskTimePercent", 1);
                //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the metrices in the server group details
                UpdateCellColor(Metric.SQLBrowserServiceStatus, alertConfig, gridRow, "SQLBrowserServiceStatus", 1);
                UpdateCellColor(Metric.SQLActiveDirectoryHelperServiceStatus, alertConfig, gridRow, "SQLActiveDirectoryHelperServiceStatus", 1);
                //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the metrices in the server group details
            }
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow,
                                     string columnName, int adjustmentMultiplier)
        {
            AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if ever called with a metric that supports multi-thresholds
            if (alertConfigItem != null)
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName))
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {
                        IComparable value = (IComparable)dataRow[columnName];
                        if (value != null && adjustmentMultiplier != 1)
                        {
                            double dbl = (double)Convert.ChangeType(value, typeof(double));
                            value = dbl * adjustmentMultiplier;
                        }

                        switch (alertConfigItem.GetSeverity(value))
                        {
                            case MonitoredState.Warning:
                                cell.Appearance.BackColor = Color.Gold;
                                cell.Appearance.ResetForeColor();
                                break;
                            case MonitoredState.Critical:
                                cell.Appearance.BackColor = Color.Red;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            default:
                                cell.Appearance.ResetBackColor();
                                cell.Appearance.ResetForeColor();
                                break;
                        }
                    }
                }
            }
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("Server Group as of {0}",
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            ValueListDisplayStyle severityStyle =
                ((ValueList)detailsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle;
            string fileName = "AllServers";
            string viewName = String.Empty;

            if (view is UserView)
            {
                viewName = ((UserView)view).Name;
            }
            else if (view is Tag)
            {
                viewName = ((Tag)view).Name;
            }

            if (viewName != String.Empty)
            {
                fileName = ExportHelper.GetValidFileName(string.Format("{0}{1}",
                                                                       viewName,
                                                                       viewName.IndexOf("Servers") > 0
                                                                           ? String.Empty
                                                                           : "Servers"
                                                             ));
            }

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((ValueList)detailsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ultraGridExcelExporter.Export(detailsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                ((ValueList)detailsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                    severityStyle;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            detailsGrid.DisplayLayout.GroupByBox.Hidden = !detailsGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }
        private void CollapseAllGroups()
        {
            detailsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            detailsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(detailsGrid);
            dialog.Show(this);
        }

        private void detailsGrid_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                int selectedInstanceId = (int)e.Row.Cells["SQLServerID"].Value;

                if (ApplicationModel.Default.ActiveInstances.Contains(selectedInstanceId))
                {
                    ApplicationController.Default.ShowServerView(selectedInstanceId);
                }
            }
        }

        private int GetSelectedInstanceId()
        {
            if (detailsGrid.Selected.Rows.Count == 1 && detailsGrid.Selected.Rows[0].Cells != null)
            {
                return (int)detailsGrid.Selected.Rows[0].Cells["SQLServerID"].Value;
            }

            return -1;
        }

        private void OpenSelectedInstance()
        {
            int selectedInstanceId = GetSelectedInstanceId();

            if (selectedInstanceId != -1)
            {
                ApplicationController.Default.ShowServerView(selectedInstanceId);
            }
        }

        private void RefreshSelectedInstance()
        {
            //int instanceId = GetSelectedInstanceId();
            //ServerGroupView groupView = ApplicationController.Default.ActiveView as ServerGroupView;
            //if (groupView == null || instanceId == -1)
            //    ApplicationController.Default.RefreshActiveView();
            //else
            //{
            //    groupView.RefreshInstance(instanceId);
            //}

            int instanceId = GetSelectedInstanceId();
            MonitoredSqlServerWrapper selectedInstance = ApplicationModel.Default.ActiveInstances[instanceId];
            if (selectedInstance != null)
            {
                if (!selectedInstance.IsRefreshing)
                {
                    selectedInstance.ForceScheduledRefresh();
                }
            }
            else
                ApplicationController.Default.RefreshActiveView();
        }

        private void DeleteSelectedInstance()
        {
            if (ApplicationController.Default.ActiveView == parentView &&
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                int selectedInstanceId = GetSelectedInstanceId();

                if (selectedInstanceId != -1 && ApplicationModel.Default.ActiveInstances.Contains(selectedInstanceId))
                {
                    DialogResult dialogResult =
                        ApplicationMessageBox.ShowWarning(ParentForm,
                                                          "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                          "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                          "Would you like to retain the data collected for the selected instance(s)?",
                                                          ExceptionMessageBoxButtons.YesNoCancel);

                    if (dialogResult != DialogResult.Cancel)
                    {
                        try
                        {
                            if (dialogResult == DialogResult.Yes)
                            {
                                ApplicationModel.Default.DeactivateMonitoredSqlServers(
                                    new MonitoredSqlServerWrapper[]
                                        {ApplicationModel.Default.ActiveInstances[selectedInstanceId]});
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                ApplicationModel.Default.DeleteMonitoredSqlServers(
                                    new MonitoredSqlServerWrapper[]
                                        {ApplicationModel.Default.ActiveInstances[selectedInstanceId]});
                            }
                        }
                        catch (Exception e)
                        {
                            ApplicationMessageBox.ShowError(ParentForm,
                                                            "An error occurred while removing the selected SQL Server instance.",
                                                            e);
                        }
                    }
                }
            }
        }

        private void ShowSelectedInstanceProperties()
        {
            int selectedInstanceId = GetSelectedInstanceId();

            if (selectedInstanceId != -1)
            {
                try
                {
                    MonitoredSqlServerInstancePropertiesDialog dialog =
                        new MonitoredSqlServerInstancePropertiesDialog(selectedInstanceId);
                    dialog.ShowDialog(this);
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(ParentForm, "Unable to show instance properties.", e);
                }
            }
        }

        private void detailsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                {
                    Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader =
                        contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((Infragistics.Win.UltraWinToolbars.StateButtonTool)
                     toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "instanceContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void detailsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            int selectedInstanceId = GetSelectedInstanceId();

            if (selectedInstanceId != -1 && ApplicationModel.Default.ActiveInstances.Contains(selectedInstanceId))
            {
                ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[selectedInstanceId];
            }
        }

        private void detailsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = detailsGrid.DisplayLayout.Bands[0];
            EditorWithText textEditor = new EditorWithText();
            band.Columns["Severity"].Editor = textEditor;
        }

        #region Nested type: HideFocusRectangleDrawFilter

        internal sealed class HideFocusRectangleDrawFilter : IUIElementDrawFilter
        {
            #region IUIElementDrawFilter Members

            public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
            {
                return true;
            }

            public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
            {
                return DrawPhase.BeforeDrawFocus;
            }

            #endregion
        }

        #endregion

        private void ServerGroupDetailsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        internal void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            if (lastUpdateDataTable == null)
                return;

            MonitoredSqlServerWrapper selectedInstance = e.Instance;

            if (detailsGrid.Rows != null)
            {
                foreach (UltraGridRow gridRow in detailsGrid.Rows.GetAllNonGroupByRows())
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    if (dataRowView != null)
                    {
                        DataRow row = dataRowView.Row;
                        if (row != null)
                        {
                            int instanceId = (int)row["SQLServerID"];
                            if (instanceId == selectedInstance.Id)
                            {
                                UpdateData(instanceId, lastUpdateDataTable);
                            }
                        }
                    }
                }
            }
        }


        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.detailsGrid);
            //4.11 part B: GroupBy DropDown :Babita Manral
        }
        private void toolstripButtonClick(object sender, EventArgs e)
        {
            try
            {
                ToolStripButton btn = (ToolStripButton)sender;
                if (btn.Text != null && string.Compare(btn.Text, "Group By", true) != 0)
                {
                    this.detailsGrid.Rows.Band.SortedColumns.Clear();
                    string key = btn.Text;
                    switch (key)
                    {
                        case "Severity":
                            key = "Alert";
                            break;
                        case "SQLdmRepo":
                            key = "InstanceName";
                            break;
                        case "Tags":
                            key = "Tags";
                            break;
                        default:
                            break;
                    }
                    this.detailsGrid.DataSource = detailsGridDataSource;
                    var band = detailsGrid.DisplayLayout.Bands[0];
                    var sortedColumns = band.SortedColumns;
                    sortedColumns.Add(key, false, true);
                }
                if (string.Compare(btn.Text, "Group By", true) == 0)
                {
                    this.detailsGrid.Rows.Band.SortedColumns.Clear();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

        }
    }

}