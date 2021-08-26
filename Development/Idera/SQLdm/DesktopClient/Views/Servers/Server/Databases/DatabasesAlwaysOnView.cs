using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Snapshots.AlwaysOn;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.Common.Events;
using System.Diagnostics;
using System.ComponentModel;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using BBS.TracerX;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesAlwaysOnView : ServerBaseView, IDatabasesView
    {
        #region constants

        private const string AllDatabasesItemText = "< All Availability Groups >";
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";

       private DateTime? historicalSnapshotDateTime;
        #endregion
        #region fields
        protected Logger LOG = Logger.GetLogger("DatabasesAlwaysOnView");
        
        private enum ViewStatus
        {
            NoItems,
            FailingOver,
            Suspending,
            Refreshing,
            Normal
        }
        private ViewStatus currentViewStatus = ViewStatus.Normal;
        /// <summary>
        /// This is the database that has been selected by the combo box
        /// </summary>
        private string selectedAvailabilityGroupFilter;

        /// <summary>
        /// This is the Group ID that has been selected in the Availability Groups Grid
        /// </summary>
        private string selectedGroupId = null;
        /// <summary>
        /// This is the Replica ID that has been selected in the Availability Groups Grid
        /// </summary>
        private string selectedReplicaId = null;
        /// <summary>
        /// This is the Database Name that has been selected in the Availability Groups Grid
        /// </summary>
        private string selectedDatabaseName = null;

        /// <summary>
        /// Datatable containing the available groups for the always on feature
        /// </summary>
        private DataTable alwaysOnAvailabilityGroupsDataTable;
        /// <summary>
        /// This config is for the initial harvesting of all available groups for the always on feature
        /// </summary>
        private AlwaysOnAvailabilityGroupsConfiguration alwaysOnAvailabilityGroupsConfiguration;
        /// <summary>
        /// Flags the completion of the initial population of the Availability Groups combo and the grid
        /// </summary>
        private bool blnAvailabilityGroupsComboInitialized;
        /// <summary>
        /// Lock object to ensure that updateData is thread safe
        /// </summary>
        private static readonly object updatingAlwaysOnLock = new object();

        private UltraGridColumn selectedColumn = null;
        private UltraGrid selectedGrid = null;

        private DataView alwaysOnSizeChartDataSource;
        private DataTable realTimeDataTable; // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        private DataTable historyDataTable;
        private int chartMouseX = 0;
        private int chartMouseY = 0;

        //last Settings values used to determine if changed for saving when leaving
        private int lastMainSplitterDistance = 0;
        private GridSettings lastAvailabilityGroupsSettings = null;
        private GridSettings lastStatisticsSettings = null;
        
        private int? _selectedSpid = null;
        private bool filterChanged = false;
        private bool isPrePopulated = false;

        private static readonly object updateLock = new object();

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler FilterChanged;
      //  public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler TraceAllowedChanged;
        public event EventHandler KillAllowedChanged;
        private int? selectedSpid
        {
            get { return _selectedSpid; }
            set
            {
                _selectedSpid = value;
                if (TraceAllowedChanged != null)
                {
                    TraceAllowedChanged(this, EventArgs.Empty);
                }
                if (KillAllowedChanged != null)
                {
                    KillAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion
        #region Constructors
        public DatabasesAlwaysOnView(int instanceId): base(instanceId)
        {
            InitializeComponent();
            //new AlwaysOnAvailabilityGroupsSnapshot() is passed as a dummy data currently to make the function call happy.
            alwaysOnAvailabilityGroupsConfiguration = new AlwaysOnAvailabilityGroupsConfiguration(instanceId, new AlwaysOnAvailabilityGroupsSnapshot());
            
            databasesGridStatusLabel.Text = 
                alwaysOnRateChartStatusLabel.Text = 
                alwaysOnSizeChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            alwaysOnAvailabilityGroupsGrid.Visible = false;
            alwaysOnSizeChart.Visible = false;
            alwaysOnRateChart.Visible = false;
            availabilityGroupsComboBox.Enabled = false;
            availabilityGroupsComboBox.Items.Clear();
            availabilityGroupsComboBox.Items.Add(null, string.Format("< {0} >", Idera.SQLdm.Common.Constants.LOADING));

            InitializeAlwaysOnAvailabilityGroupsTable();
            InitializeHistoryDataTable();
            InitializeCharts();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        #endregion
        
        #region IDatabasesView Members
        public string SelectedDatabaseFilter
        {
            get { return selectedAvailabilityGroupFilter; }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                if (historicalSnapshotDateTime != value)
                {
                    historicalSnapshotDateTime = value;
                    currentHistoricalSnapshotDateTime = null;
                    selectedSpid = null;
                    //tempdbSessionsGridDataSource.Rows.Clear();
                    //rowLookupTable.Clear();

                    if (HistoricalSnapshotDateTimeChanged != null)
                    {
                        HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        #endregion

        #region chart

        private void InitializeCharts()
        {
            InitializeAlwaysOnSizeChart();
            InitializeAlwaysOnRateChart();
            InitalizeDrilldown(alwaysOnRateChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeAlwaysOnSizeChart()
        {
            alwaysOnSizeChart.Printer.Orientation = PageOrientation.Landscape;
            alwaysOnSizeChart.Printer.Compress = true;
            alwaysOnSizeChart.Printer.ForceColors = true;
            alwaysOnSizeChart.Printer.Document.DocumentName = "Queue Size Chart";
            alwaysOnSizeChart.ToolBar.RemoveAt(0);
            alwaysOnSizeChart.DataSource = alwaysOnSizeChartDataSource;

            ConfigureAlwaysOnSizeChart();
            alwaysOnSizeChart.Visible = false;
        }

        private void ConfigureAlwaysOnSizeChart()
        {
            alwaysOnSizeChart.SuspendLayout();
            alwaysOnSizeChart.DataSourceSettings.Fields.Clear();

            FieldMap dbFieldMap = new FieldMap("Database Name", FieldUsage.Label);
            dbFieldMap.DisplayName = "Database";
            FieldMap logSendQueueSizeFieldMap = new FieldMap("Log Send Queue (KB)", FieldUsage.Value);
            logSendQueueSizeFieldMap.DisplayName = "Log Send Q Size";
            FieldMap redoQueueSizeFieldMap = new FieldMap("Redo Queue (KB)", FieldUsage.Value);
            redoQueueSizeFieldMap.DisplayName = "Redo Q Size";

            alwaysOnSizeChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                dbFieldMap,
                logSendQueueSizeFieldMap,
                redoQueueSizeFieldMap});
            alwaysOnSizeChart.AxisY.AutoScale = true;
            alwaysOnSizeChart.AxisY.DataFormat.Decimals = 0;
            alwaysOnSizeChart.ToolTipFormat = "%l\n%s\n%v KB";

            alwaysOnSizeChart.DataSourceSettings.ReloadData();
            alwaysOnSizeChart.Invalidate();
            alwaysOnSizeChart.ResumeLayout();
            alwaysOnSizeChart_Resize(alwaysOnSizeChart, new EventArgs());
        }

        private void InitializeAlwaysOnRateChart()
        {
            alwaysOnRateChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SQLdm 10.2 (Anshul Aggarwal) - New History Browser
            alwaysOnRateChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            alwaysOnRateChart.LegendBox.Width = 155;
            alwaysOnRateChart.LegendBox.PlotAreaOnly = false;

            alwaysOnRateChart.Printer.Orientation = PageOrientation.Landscape;
            alwaysOnRateChart.Printer.Compress = true;
            alwaysOnRateChart.Printer.ForceColors = true;
            alwaysOnRateChart.Printer.Document.DocumentName = "Transfer Rate Chart";
            alwaysOnRateChart.ToolBar.RemoveAt(0);
            alwaysOnRateChart.DataSource = realTimeDataTable;

            ConfigureAlwaysOnRateChart();
            alwaysOnRateChart.Visible = false;
        }

        private void ConfigureAlwaysOnRateChart()
        {
            alwaysOnRateChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            FieldMap logSendRateFieldMap = new FieldMap("Log Send Rate (KB/s)", FieldUsage.Value);
            logSendRateFieldMap.DisplayName = "Log Send Rate";
            FieldMap redoRateFieldMap = new FieldMap("Redo Rate (KB/s)", FieldUsage.Value);
            redoRateFieldMap.DisplayName = "Redo Rate";

            alwaysOnRateChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                logSendRateFieldMap,
                redoRateFieldMap,
                dateFieldMap});

            alwaysOnSizeChart.AxisY.AutoScale = true;
            alwaysOnRateChart.AxisY.DataFormat.Decimals = 0;
            alwaysOnRateChart.ToolTipFormat = "%l\n%s\n%v KB/s\n%x";
            
            alwaysOnRateChart.DataSourceSettings.ReloadData();
            alwaysOnRateChart.Invalidate();
            alwaysOnRateChart.ResumeLayout();
            alwaysOnRateChart_Resize(alwaysOnRateChart, new EventArgs());
        }

        private void ToggleChartToolbar(Chart chart, bool visible)
        {
            if (chart != null)
            {
                chart.ToolBar.Visible = visible;
            }
        }

        private void PrintChart(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Chart chart)
        {
            if (chart != null)
            {
                string title = string.Empty;
                if (chart.Tag is ToolStripItem)
                    title = ((ToolStripItem)chart.Tag).Text;
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title,
                                                              ExportHelper.GetValidFileName(title, true));
            }
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            splitContainer1.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            contentContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart()
        {
            if (restoreAlwaysOnSizeChartButton.Visible)
            {
                RestoreChart(alwaysOnSizePanel, maximizeAlwaysOnSizeChartButton, restoreAlwaysOnSizeChartButton, 0, 0);
            }
            else if (restoreAlwaysOnRateChartButton.Visible)
            {
                RestoreChart(alwaysOnRatePanel, maximizeAlwaysOnRateChartButton, restoreAlwaysOnRateChartButton, 1, 0);
            }
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            contentContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            splitContainer1.Visible = true;
        }

        private void maximizeAlwaysOnSizeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(alwaysOnSizePanel, maximizeAlwaysOnSizeChartButton, restoreAlwaysOnSizeChartButton);
        }

        private void restoreAlwaysOnSizeChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(alwaysOnSizePanel, maximizeAlwaysOnSizeChartButton, restoreAlwaysOnSizeChartButton, 0, 0);
        }

        private void maximizeAlwaysOnRateChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(alwaysOnRatePanel, maximizeAlwaysOnRateChartButton, restoreAlwaysOnRateChartButton);
        }

        private void restoreAlwaysOnRateChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(alwaysOnRatePanel, maximizeAlwaysOnRateChartButton, restoreAlwaysOnRateChartButton, 1, 0);
        }

        private void alwaysOnSizeChart_GetTip(object sender, GetTipEventArgs e)
        {
            Chart chart = ((Chart)sender);
            if (e.Object == chart.AxisX)
            {
                if (e.Text.Length > 0)
                {
                    HitTestEventArgs e1 = chart.HitTest(chartMouseX, chartMouseY);
                    int currentPoint = (int)Math.Floor(e1.Value);
                    if (currentPoint < ((DataView)chart.DataSource).Count)
                    {
                        e.Text = (string)((DataView)chart.DataSource)[currentPoint]["Database Name"];
                    }
                }
            }
        }

        private void alwaysOnSizeChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 2;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
            }
        }

        private void alwaysOnRateChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 2;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }

        private void UpdateAlwaysOnSizeChartDataFilter()
        {
            // Clean up the selection list and build the filter string for the DataView
            StringBuilder filter = new StringBuilder();
            filter.Append("[Database Name] = '");
            filter.Append(selectedDatabaseName);
            filter.Append("' AND [Group ID] = '");
            filter.Append(selectedGroupId);
            filter.Append("' AND [Replica ID] = '");
            filter.Append(selectedReplicaId);
            filter.Append("'");

            alwaysOnSizeChartDataSource.RowFilter = filter.ToString();

            if (alwaysOnSizeChartDataSource.Count > 0)
            {
                alwaysOnSizeChart.Visible = true;
            }
            else
            {
                alwaysOnSizeChart.Visible = false;
            }
        }

        private void UpdateAlwaysOnRateChartDataFilter()
        {
            var dataTable = alwaysOnRateChart.DataSource as DataTable;
            if (dataTable != null)
            {
                DateTime viewFilter = DateTime.MinValue;
                if (ViewMode == ServerViewMode.RealTime)
                    viewFilter = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                else if (ViewMode == ServerViewMode.Historical)
                    viewFilter = HistoricalSnapshotDateTime.Value.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                else
                    viewFilter = HistoricalStartDateTime.Value;

                StringBuilder filter = new StringBuilder();
                filter.Append(string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture)));
                filter.Append(" AND [Database Name] = '");
                filter.Append(selectedDatabaseName);
                filter.Append("' AND [Group ID] = '");
                filter.Append(selectedGroupId);
                filter.Append("' AND [Replica ID] = '");
                filter.Append(selectedReplicaId);
                filter.Append("'");

                dataTable.DefaultView.RowFilter = filter.ToString();
                dataTable.DefaultView.Sort = "Date";
                if (dataTable.DefaultView.Count > 0)
                {
                    alwaysOnRateChart.Visible = true;
                }
                else
                {
                    alwaysOnRateChart.Visible = false;
                }
            }
        }
        
        #endregion

        private void SetViewStatus(ViewStatus status)
        {
            currentViewStatus = status;

            switch (status)
            {
                case ViewStatus.NoItems:
                    databasesGridStatusLabel.Text = 
                        alwaysOnRateChartStatusLabel.Text = 
                        alwaysOnSizeChartStatusLabel.Text = NO_ITEMS;
                    alwaysOnAvailabilityGroupsGrid.Visible = false;
                    alwaysOnSizeChart.Visible = false;
                    alwaysOnRateChart.Visible = false;
                    ApplicationController.Default.ClearCustomStatus();
                    availabilityGroupsComboBox.Enabled = false;
                    availabilityGroupsComboBox.Items.Clear();
                    availabilityGroupsComboBox.Items.Add(null, string.Format("< {0} >", NO_ITEMS));
                    break;
                case ViewStatus.Refreshing:
                case ViewStatus.Suspending:   
                case ViewStatus.FailingOver:
                    availabilityGroupsComboBox.Enabled = false;
                    break;
                default:
                    availabilityGroupsComboBox.Enabled = true;
                    alwaysOnAvailabilityGroupsGrid.Enabled = true;
                    alwaysOnAvailabilityGroupsGrid.Visible = true;
                    break;
            }
        }
        public void DatabasesAlwaysOnView_Load(object sender, System.EventArgs e)
        {
            ApplySettings();
            SetViewStatus(ViewStatus.Refreshing);
        }

        /// <summary>
        /// When a new database is selected, initialize the gathering of all relevant metrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void availabilityGroupsComboBox_SelectionChanged(object sender, EventArgs e)
        {
            //if at least one database has been selected
            if (availabilityGroupsComboBox.SelectedItem != null)
            {

                ApplicationController.Default.ClearCustomStatus();

                //If the combobox is selecting "All Databases"
                if (availabilityGroupsComboBox.SelectedItem.DisplayText == AllDatabasesItemText)
                {
                    selectedAvailabilityGroupFilter = null;
                    alwaysOnAvailabilityGroupsDataTable.DefaultView.RowFilter = null;
                    ApplicationController.Default.SetCustomStatus(
                                                String.Format("Availability Groups: {0}, Statistics: {1} Item{2}",
                                                              availabilityGroupsComboBox.Items.Count - 1,
                                                              alwaysOnAvailabilityGroupsDataTable.DefaultView.Count,
                                                              alwaysOnAvailabilityGroupsDataTable.DefaultView.Count == 1 ? string.Empty : "s")
                                                );
                }
                else if (availabilityGroupsComboBox.Items.Count > 1) //if there is at least one database
                {
                    string selectedDatabase = availabilityGroupsComboBox.SelectedItem.DisplayText;

                    if (!selectedDatabase.Equals(selectedAvailabilityGroupFilter))
                    {
                        selectedAvailabilityGroupFilter = availabilityGroupsComboBox.SelectedItem.DisplayText;

                        //Filter the datatable on the name of the selected database
                        alwaysOnAvailabilityGroupsDataTable.DefaultView.RowFilter =
                            string.Format("[Group Name] = '{0}'", selectedAvailabilityGroupFilter.Replace("'", "''"));

                        //clear the grid that will show all availability groups
                        alwaysOnAvailabilityGroupsGrid.Selected.Rows.Clear();

                        UltraGridRow[] nonGroupByRows = alwaysOnAvailabilityGroupsGrid.Rows.GetAllNonGroupByRows();
                        if (nonGroupByRows.Length > 0)
                        {
                            alwaysOnAvailabilityGroupsGrid.Selected.Rows.Add(nonGroupByRows[0]);
                            UltraGridRow groupData = (UltraGridRow)alwaysOnAvailabilityGroupsGrid.Selected.Rows[0];
                            selectedGroupId = groupData.Cells[2].Text;
                            selectedReplicaId = groupData.Cells[4].Text;
                            selectedDatabaseName = groupData.Cells[0].Text;
                        }
                        ApplicationController.Default.SetCustomStatus("Availability Group Filter Applied,", 
                                                       String.Format("Statistics: {0} Item{1}",
                                                              alwaysOnAvailabilityGroupsDataTable.DefaultView.Count,
                                                              alwaysOnAvailabilityGroupsDataTable.DefaultView.Count == 1 ? string.Empty : "s")
                                                );
                    }
                }

                alwaysOnAvailabilityGroupsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                UpdateColors();
                alwaysOnAvailabilityGroupsGrid.Focus();
            }
        }

        private void refreshDatabasesButton_Click(object sender, EventArgs e)
        {
            blnAvailabilityGroupsComboInitialized = false;
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.ActiveView.RefreshView();
        }

        private void alwaysOnAvailabilityGroupsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].AutoSizeMode = ColumnAutoSizeMode.VisibleRows;
            //e.Layout.Bands[0].Columns["Time Recorded"].Format = "G";
            e.Layout.Bands[0].Columns["Redo Rate (KB/s)"].Format = "###,###,##0 KB/sec";
            e.Layout.Bands[0].Columns["Log Send Rate (KB/s)"].Format = "###,###,##0 KB/sec";
        }

        private void alwaysOnAvailabilityGroupsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            //if there are no selected rows then return
            if (alwaysOnAvailabilityGroupsGrid.Rows.Count <= 0 || alwaysOnAvailabilityGroupsGrid.Selected.Rows.Count <= 0) return;
            UltraGridRow groupData = (UltraGridRow)alwaysOnAvailabilityGroupsGrid.Selected.Rows[0];
            selectedGroupId = groupData.Cells[2].Text;
            selectedReplicaId = groupData.Cells[4].Text;
            selectedDatabaseName = groupData.Cells[0].Text;
            UpdateAlwaysOnSizeChartDataFilter();
            UpdateAlwaysOnRateChartDataFilter();
        }

        private void alwaysOnAvailabilityGroupsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedGrid = alwaysOnAvailabilityGroupsGrid;

            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                
                    toolbarsManager.Tools["groupByThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["toggleGroupByBoxButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["removeThisColumnButton"].SharedProps.Visible = true;
                    toolbarsManager.Tools["showColumnChooserButton"].SharedProps.Visible = true;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));
                
                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "detailsGridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void InitializeAlwaysOnAvailabilityGroupsTable()
        {
            alwaysOnAvailabilityGroupsDataTable = new DataTable();
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Database Name", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Group Name", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Group ID", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Replica Name", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Replica ID", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Failover Mode", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Availability Mode", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Replica Role", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Synchronization Health", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Log Send Queue (KB)", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Log Send Rate (KB/s)", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Redo Queue (KB)", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Redo Rate (KB/s)", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Listener DNS Name", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Listener IP Address", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Listener Port", typeof(int));
            //Data From Statistics.
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Database ID", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Failover Readiness", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Synchronization Database Status", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Database Status", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Suspended Status", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Last Hardened Time", typeof(DateTime));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Operational Status", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Connection Status", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Last Connect Error #", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Last Connect Error Description", typeof(string));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Last Connect Error Time", typeof(DateTime));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Synchronization Performance", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Estimated Data Loss", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("Estimated Recovery Time", typeof(int));
            alwaysOnAvailabilityGroupsDataTable.Columns.Add("FileStream Send Rate", typeof(int));
            
            //alwaysOnAvailabilityGroupsDataTable.PrimaryKey = new DataColumn[] { alwaysOnAvailabilityGroupsDataTable.Columns["Database Name"] };
            //alwaysOnAvailabilityGroupsDataTable.CaseSensitive = true;
            //alwaysOnAvailabilityGroupsDataTable.DefaultView.Sort = "Database Name";

            alwaysOnAvailabilityGroupsGrid.DataSource = alwaysOnAvailabilityGroupsDataTable;
            alwaysOnAvailabilityGroupsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            alwaysOnAvailabilityGroupsGrid.InitializeLayout += new InitializeLayoutEventHandler(alwaysOnAvailabilityGroupsGrid_InitializeLayout);

            alwaysOnSizeChartDataSource = new DataView(alwaysOnAvailabilityGroupsDataTable, string.Empty, "[Database Name] desc", DataViewRowState.CurrentRows);

        }

        private void InitializeHistoryDataTable()
        {
            realTimeDataTable = new DataTable();
            realTimeDataTable.Columns.Add("Date", typeof(DateTime));
            realTimeDataTable.Columns.Add("Database Name", typeof(string));
            realTimeDataTable.Columns.Add("Group ID", typeof(string));
            realTimeDataTable.Columns.Add("Replica ID", typeof(string));
            realTimeDataTable.Columns.Add("Log Send Rate (KB/s)", typeof(int));
            realTimeDataTable.Columns.Add("Redo Rate (KB/s)", typeof(int));
            realTimeDataTable.Columns.Add("IsHistorical", typeof(bool));
            realTimeDataTable.DefaultView.Sort = "Date";

            historyDataTable = realTimeDataTable.Clone();
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        ///  Adds real time data for the view.
        /// </summary>
        private void AddRealTimeData(DataTable table, AlwaysOnStatistics statistics, string databaseName, Guid groupId)
        {
            DataRow newRow = table.NewRow();
            newRow["Date"] = DateTime.Now;
            FillData(newRow, statistics, databaseName, groupId);
            table.Rows.Add(newRow);
            GroomHistoryData();
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        ///  Adds historical data for the view.
        /// </summary>
        private void AddHistoryData(DataTable table, AlwaysOnAvailabilityGroupsSnapshot snapshot, Guid replicaGroupId, Guid replicaId, long databaseId
            , string databaseName, Guid avlGroupGroupId)
        {
            foreach (AlwaysOnStatistics statistics in snapshot.ReplicaStatistics)
            {
                if (statistics.GroupId == replicaGroupId &&
                    statistics.ReplicaId == replicaId &&
                    statistics.DatabaseId == databaseId)
                {
                    DataRow newRow = table.NewRow();
                    newRow["Date"] = statistics.UTCCollectionDateTime != null ? statistics.UTCCollectionDateTime.Value.ToLocalTime() :
                        (object)DBNull.Value;   // Provide acceptable DBNull Value
                    newRow["IsHistorical"] = true;
                    FillData(newRow, statistics, databaseName, avlGroupGroupId);
                    table.Rows.Add(newRow);
                }
            }
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        ///  Fills data into the new row.
        /// </summary>
        private void FillData(DataRow newRow, AlwaysOnStatistics statistics, string databaseName, Guid groupId)
        {
            newRow["Database Name"] = databaseName;
            newRow["Group ID"] = groupId;
            newRow["Replica ID"] = statistics.ReplicaId;
            newRow["Log Send Rate (KB/s)"] = statistics.LogSendRate;
            newRow["Redo Rate (KB/s)"] = statistics.RedoRate;
        }

        private void GroomHistoryData()
        {
            if (realTimeDataTable != null)
            {
                DataRow[] groomedRows = realTimeDataTable.Select(ServerSummaryHistoryData.GetGroomingFilter("Date"));
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeDataTable.AcceptChanges();
            }
        }

        /// <summary>
        /// Get the ID of the server if it is monitored
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns>integer Server ID</returns>
        private static int ServerNameToID(string ServerName)
        {
            try
            {
                //MonitoredSqlServerCollection tmpServers = ApplicationModel.Default.ActiveInstances;
                foreach(MonitoredSqlServer tmpServer in ApplicationModel.Default.ActiveInstances)
                {
                    if(tmpServer.InstanceName.ToLower().Equals(ServerName.ToLower())) return tmpServer.Id;
                }
                return RepositoryHelper.GetAlternateServerID(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                     ServerName);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Populate the Availability Groups combo box, defaults selectedIndex to that of selectedAvailabilityGroupFilter
        /// </summary>
        /// <param name="databases"></param>
        public void PopulateAvailabilityGroupsCombo(List<string> groupNames)
        {
            //clear the available databases combo box
            availabilityGroupsComboBox.Items.Clear();

            //if there are database details in the dictionary
            if (groupNames != null && groupNames.Count > 0)
            {
                //enable the combo box
                availabilityGroupsComboBox.Enabled = true;
                availabilityGroupsComboBox.Items.Add(null, AllDatabasesItemText);

                //add names of all Availability Groups to the combo box
                foreach (string groupName in groupNames)
                {
                    availabilityGroupsComboBox.Items.Add(groupName, groupName);
                }

                blnAvailabilityGroupsComboInitialized = true;
            }
            else//if there are no Availability Groups
            {
                availabilityGroupsComboBox.Enabled = false;
                availabilityGroupsComboBox.Items.Add(null, string.Format("< {0} >", NO_ITEMS));
            }

            availabilityGroupsComboBox.SelectedIndex = FindAvailabilityGroupIndex(selectedAvailabilityGroupFilter);
        }

        public void UpdateDataWithRealTimeSnapshot(object returnedData) {
            //bool blnPopulateDetails = false; 
            //Making Operational status panel invisible for real time mode - Ankit Nagpal SQLDM 10.0.0
            operationalStatusPanel.Visible = false;
            DatabasesAlwaysOn_Fill_Panel.Visible = true;
            SetViewStatus(ViewStatus.Refreshing);
            try
            {
                //if valid data has been returned
                if (returnedData != null && returnedData is AlwaysOnAvailabilityGroupsSnapshot)
                {
                    // SQLDM-27811 - SQLDM 10.2 : OutofMemoryException is thrown on desktop console under 'Databases-->AvailabilityGroups' tab
                    var currentAvailabilityGroupData = (AlwaysOnAvailabilityGroupsSnapshot)returnedData;

                    if (currentAvailabilityGroupData.Error == null)
                    {
                        //TODO do the snapshot returned Error handling.
                        lock (updatingAlwaysOnLock)
                        {
                            IDictionary<Guid, AvailabilityGroup> availabilityGroups = currentAvailabilityGroupData.AvailabilityGroups;
                            List<string> groupNames = new List<string>();

                            if (!blnAvailabilityGroupsComboInitialized)
                            {
                                selectedGrid = alwaysOnAvailabilityGroupsGrid;
                            }
                            SetViewStatus(ViewStatus.Normal);

                            //if the snapshot does contain valid Availability Groups 
                            if (availabilityGroups != null && availabilityGroups.Count > 0)
                            {
                                //load the data table
                                alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                                alwaysOnAvailabilityGroupsDataTable.Rows.Clear();

                                // load real time datatable
                                realTimeDataTable.BeginLoadData();

                                foreach (AvailabilityGroup avlGroups in availabilityGroups.Values)
                                {
                                    IDictionary<Guid, AvailabilityReplica> replicas = avlGroups.Replicas;
                                    if (replicas != null && replicas.Count > 0)
                                    {
                                        foreach (AvailabilityReplica replica in replicas.Values)
                                        {
                                            IDictionary<long, AlwaysOnDatabase> databases = replica.Databases;
                                            if (databases != null && databases.Count > 0)
                                            {
                                                foreach (AlwaysOnDatabase db in databases.Values)
                                                {
                                                    AlwaysOnStatistics statistics =
                                                        currentAvailabilityGroupData.GetReplicaStatistics(replica.GroupId, replica.ReplicaId, db.DatabaseId);

                                                    if (statistics == null)
                                                    {
                                                        LOG.Warn("Could not find statistics details for Group id [" + replica.GroupId +
                                                            "], Replica id [" + replica.ReplicaId + "], Database id [" + db.DatabaseId + "]");
                                                        continue;
                                                    }

                                                    alwaysOnAvailabilityGroupsDataTable.LoadDataRow(new object[]
                                                    {
                                                        db.DatabaseName,
                                                        avlGroups.GroupName,
                                                        avlGroups.GroupId,
                                                        replica.ReplicaName,
                                                        replica.ReplicaId,
                                                        replica.FailoverMode,
                                                        replica.AvailabilityMode,
                                                        replica.ReplicaRole,
                                                        ApplicationHelper.GetEnumDescription(statistics.SynchronizationDatabaseHealth),
                                                        statistics.LogSendQueueSize,
                                                        statistics.LogSendRate,
                                                        statistics.RedoQueueSize,
                                                        statistics.RedoRate,
                                                        avlGroups.ListenerDnsName.Length <= 0? "None":avlGroups.ListenerDnsName,
                                                        avlGroups.ListenerIPAddress.Length <= 0? "None":avlGroups.ListenerIPAddress,
                                                        avlGroups.ListenerPort,
                                                        //Statistics Details
	                                                    statistics.DatabaseId,
	                                                    statistics.IsFailoverReady,
	                                                    ApplicationHelper.GetEnumDescription(statistics.SynchronizationDatabaseState),
	                                                    ApplicationHelper.GetEnumDescription(statistics.DatabaseState),
	                                                    statistics.IsSuspended,
	                                                    statistics.LastHardenedTime,
	                                                    ApplicationHelper.GetEnumDescription(statistics.OperationalState),
	                                                    ApplicationHelper.GetEnumDescription(statistics.ConnectedState),
	                                                    statistics.LastConnectionErrorNumber,
	                                                    statistics.LastConnectedErrorDescription,
	                                                    statistics.LastConnectErrorTimestamp,
	                                                    statistics.SynchronizationPerformace,
	                                                    statistics.EstimatedDataLossTime,
                                                        statistics.EstimatedRecoveryTime,
	                                                    statistics.FileStreamSendRate

                                                    }, true);
                                                    if (!groupNames.Contains(avlGroups.GroupName))
                                                        groupNames.Add(avlGroups.GroupName);
                                                    AddRealTimeData(realTimeDataTable, statistics, db.DatabaseName, avlGroups.GroupId);
                                                }
                                            }
                                        }
                                    }
                                }
                                //Update cell colors depending on the alerts that have been configured
                                UpdateColors();
                                alwaysOnAvailabilityGroupsDataTable.EndLoadData();
                                realTimeDataTable.EndLoadData();
                                //SQLdm 10.0 (Srishti Purohit)
                                //Defect fix SQLDM-25444
                                if (alwaysOnAvailabilityGroupsGrid.Rows.Count > 0)
                                {
                                    if (selectedGroupId == null || selectedReplicaId == null)
                                    {
                                        SelectFirstRowInGrid();
                                    }
                                    else
                                    {
                                        bool foundRow = false;
                                        foreach (UltraGridRow row in alwaysOnAvailabilityGroupsGrid.Rows.GetAllNonGroupByRows())
                                        {
                                            if (row.IsDataRow)
                                            {
                                                DataRowView dataRow = row.ListObject as DataRowView;
                                                if (selectedGroupId.Equals((string)dataRow["Group ID"]) &&
                                                    selectedReplicaId.Equals
                                                    ((string)dataRow["Replica ID"]) &&
                                                    selectedDatabaseName.Equals(dataRow["Database Name"]))
                                                {
                                                    foundRow = true;
                                                    row.Selected = true;
                                                    alwaysOnAvailabilityGroupsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                                }
                                                row.RefreshSortPosition();
                                            }
                                        }

                                        // SqlDM 10.2 (Anshul Aggarwal) - SQLDM-27488 - History mode not working properly.
                                        // If selected no row matches, select the first row
                                        if (!foundRow)
                                            SelectFirstRowInGrid();
                                    }
                                }

                                PopulateAvailabilityGroupsCombo(groupNames);
                                FixupBindings(false);
                                UpdateAlwaysOnSizeChartDataFilter();
                                UpdateAlwaysOnRateChartDataFilter();
                                SetViewStatus(ViewStatus.Normal);
                            }
                            else //databases is null or there are no databases
                            {
                                SetViewStatus(ViewStatus.NoItems);
                                //Clear the grid
                                alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                                alwaysOnAvailabilityGroupsDataTable.Rows.Clear();
                                alwaysOnAvailabilityGroupsDataTable.EndLoadData();
                            }

                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                    }
                    else   //if an error occurred in the refresh
                    {
                        SetViewStatus(ViewStatus.NoItems);
                        databasesGridStatusLabel.Text =
                            alwaysOnRateChartStatusLabel.Text =
                            alwaysOnSizeChartStatusLabel.Text = UNABLE_TO_UPDATE;
                        availabilityGroupsComboBox.Items.Clear();
                        availabilityGroupsComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                        //Clear the grid
                        alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                        alwaysOnAvailabilityGroupsDataTable.Rows.Clear();
                        alwaysOnAvailabilityGroupsDataTable.EndLoadData();

                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, currentAvailabilityGroupData.Error));
                        return;
                    }
                }
            }
            finally
            {
                //SetViewStatus(ViewStatus.Normal);
            }
        }

        /// <summary>
        /// Updates data with historical snapshot - SQLdm10.0.0(Ankit Nagpal)
        /// </summary>
        /// <param name="returnedData"></param>
        public void UpdateDataWithHistoricalSnapshot(object returnedData) {
            //bool blnPopulateDetails = false; 
            SetViewStatus(ViewStatus.Refreshing);
            try
            {
                //if valid data has been returned
                if (returnedData != null && returnedData is AlwaysOnAvailabilityGroupsSnapshot)
                {
                    // SQLDM-27811 - SQLDM 10.2 : OutofMemoryException is thrown on desktop console under 'Databases-->AvailabilityGroups' tab
                    var currentAvailabilityGroupData = (AlwaysOnAvailabilityGroupsSnapshot)returnedData;

                    if (currentAvailabilityGroupData.Error == null)
                    {
                        //TODO do the snapshot returned Error handling.
                        lock (updatingAlwaysOnLock)
                        {
                            ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,Properties.Resources.StatusWarningSmall);
                            IDictionary<Guid, AvailabilityGroup> availabilityGroups = currentAvailabilityGroupData.AvailabilityGroups;
                            List<string> groupNames = new List<string>();

                            if (!blnAvailabilityGroupsComboInitialized)
                            {
                                selectedGrid = alwaysOnAvailabilityGroupsGrid;
                            }
                            SetViewStatus(ViewStatus.Normal);

                            //if the snapshot does contain valid Availability Groups 
                            if (availabilityGroups != null && availabilityGroups.Count > 0)
                            {
                                //load the data table
                                alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                                alwaysOnAvailabilityGroupsDataTable.Rows.Clear();

                                historyDataTable.BeginLoadData();
                                historyDataTable.Rows.Clear();
                                foreach (AvailabilityGroup avlGroups in availabilityGroups.Values)
                                {
                                    IDictionary<Guid, AvailabilityReplica> replicas = avlGroups.Replicas;
                                    if (replicas != null && replicas.Count > 0)
                                    {
                                        foreach (AvailabilityReplica replica in replicas.Values)
                                        {
                                            IDictionary<long, AlwaysOnDatabase> databases = replica.Databases;
                                            if (databases != null && databases.Count > 0)
                                            {
                                                foreach (AlwaysOnDatabase db in databases.Values)
                                                {
                                                    AlwaysOnStatistics statistics =
                                                        currentAvailabilityGroupData.GetReplicaStatistics(replica.GroupId, replica.ReplicaId, db.DatabaseId);

                                                    if (statistics == null)
                                                    {
                                                        LOG.Warn("Could not find statistics details for Group id [" + replica.GroupId +
                                                            "], Replica id [" + replica.ReplicaId + "], Database id [" + db.DatabaseId + "]");
                                                        continue;
                                                    }

                                                    alwaysOnAvailabilityGroupsDataTable.LoadDataRow(new object[]
                                                    {
                                                        db.DatabaseName,
                                                        avlGroups.GroupName,
                                                        avlGroups.GroupId,
                                                        replica.ReplicaName,
                                                        replica.ReplicaId,
                                                        replica.FailoverMode,
                                                        replica.AvailabilityMode,
                                                        replica.ReplicaRole,
                                                        ApplicationHelper.GetEnumDescription(statistics.SynchronizationDatabaseHealth),
                                                        statistics.LogSendQueueSize,
                                                        statistics.LogSendRate,
                                                        statistics.RedoQueueSize,
                                                        statistics.RedoRate,
                                                        avlGroups.ListenerDnsName.Length <= 0? "None":avlGroups.ListenerDnsName,
                                                        avlGroups.ListenerIPAddress.Length <= 0? "None":avlGroups.ListenerIPAddress,
                                                        avlGroups.ListenerPort,
                                                        //Statistics Details
	                                                    statistics.DatabaseId,
	                                                    statistics.IsFailoverReady,
	                                                    ApplicationHelper.GetEnumDescription(statistics.SynchronizationDatabaseState),
	                                                    ApplicationHelper.GetEnumDescription(statistics.DatabaseState),
	                                                    statistics.IsSuspended,
	                                                    statistics.LastHardenedTime,
	                                                    ApplicationHelper.GetEnumDescription(statistics.OperationalState),
	                                                    ApplicationHelper.GetEnumDescription(statistics.ConnectedState),
	                                                    statistics.LastConnectionErrorNumber,
	                                                    statistics.LastConnectedErrorDescription,
	                                                    statistics.LastConnectErrorTimestamp,
	                                                    statistics.SynchronizationPerformace,
	                                                    statistics.EstimatedDataLossTime,
                                                        statistics.EstimatedRecoveryTime,
	                                                    statistics.FileStreamSendRate

                                                    }, true);
                                                    if (!groupNames.Contains(avlGroups.GroupName))
                                                        groupNames.Add(avlGroups.GroupName);
                                                    AddHistoryData(historyDataTable,currentAvailabilityGroupData, replica.GroupId, replica.ReplicaId, db.DatabaseId,
                                                        db.DatabaseName, avlGroups.GroupId);
                                                }
                                            }
                                        }
                                    }
                                }
                                //Update cell colors depending on the alerts that have been configured
                                UpdateColors();
                                alwaysOnAvailabilityGroupsDataTable.EndLoadData();
                                historyDataTable.EndLoadData();
                                //SQLdm 10.0 (Srishti Purohit)
                                //Defect fix SQLDM-25444
                                if (alwaysOnAvailabilityGroupsGrid.Rows.Count > 0)
                                {
                                    if (selectedGroupId == null || selectedReplicaId == null)
                                    {
                                        SelectFirstRowInGrid();
                                    }
                                    else
                                    {
                                        bool foundRow = false;
                                        foreach (UltraGridRow row in alwaysOnAvailabilityGroupsGrid.Rows.GetAllNonGroupByRows())
                                        {
                                            if (row.IsDataRow)
                                            {
                                                DataRowView dataRow = row.ListObject as DataRowView;
                                                if (selectedGroupId.Equals((string)dataRow["Group ID"]) &&
                                                    selectedReplicaId.Equals
                                                    ((string)dataRow["Replica ID"]) &&
                                                    selectedDatabaseName.Equals(dataRow["Database Name"]))
                                                {
                                                    foundRow = true;
                                                    row.Selected = true;
                                                    alwaysOnAvailabilityGroupsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                                }
                                                row.RefreshSortPosition();
                                            }
                                        }

                                        // SqlDM 10.2 (Anshul Aggarwal) - SQLDM-27488 - History mode not working properly.
                                        // If selected no row matches, select the first row
                                        if (!foundRow)
                                            SelectFirstRowInGrid();
                                    }
                                }

                                PopulateAvailabilityGroupsCombo(groupNames);
                                FixupBindings(true);
                                UpdateAlwaysOnSizeChartDataFilter();
                                UpdateAlwaysOnRateChartDataFilter();
                                SetViewStatus(ViewStatus.Normal);
                            }
                            else //databases is null or there are no databases
                            {
                                SetViewStatus(ViewStatus.NoItems);
                                //Clear the grid
                                alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                                alwaysOnAvailabilityGroupsDataTable.Rows.Clear();
                                alwaysOnAvailabilityGroupsDataTable.EndLoadData();

                                // Clear historical data
                                historyDataTable.BeginLoadData();
                                historyDataTable.Rows.Clear();
                                historyDataTable.EndLoadData();
                            }

                            currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime; //SQLdm 10.2 (Anshul Aggarwal) - New History Browser
                            currentHistoricalStartDateTime = HistoricalStartDateTime;
                            ApplicationController.Default.OnRefreshActiveViewCompleted(
                                new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                        }
                    }
                    else   //if an error occurred in the refresh
                    {
                        SetViewStatus(ViewStatus.NoItems);
                        databasesGridStatusLabel.Text =
                            alwaysOnRateChartStatusLabel.Text =
                            alwaysOnSizeChartStatusLabel.Text = UNABLE_TO_UPDATE;
                        availabilityGroupsComboBox.Items.Clear();
                        availabilityGroupsComboBox.Items.Add(null, string.Format("< {0} >", UNABLE_TO_UPDATE));
                        //Clear the grid
                        alwaysOnAvailabilityGroupsDataTable.BeginLoadData();
                        alwaysOnAvailabilityGroupsDataTable.Rows.Clear();
                        alwaysOnAvailabilityGroupsDataTable.EndLoadData();

                        // Clear historical data
                        historyDataTable.BeginLoadData();
                        historyDataTable.Rows.Clear();
                        historyDataTable.EndLoadData();

                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, currentAvailabilityGroupData.Error));
                        return;
                    }
                }
            }
            finally
            {
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
                //SetViewStatus(ViewStatus.Normal);
            } 
        }

        /// <summary>
        /// Overrides UpdateData in the view object. Is the callback for all scheduled refreshes
        /// Returns all realtime data. This is used for the population of the combo box as well as the grid.
        /// Combo and Availability groups contains the same database but combo will not have duplicate database names.
        /// The data table does filter on whatever is selected in the combo box
        /// </summary>
        /// <param name="returnedData"></param>
        public override void UpdateData(object returnedData)
        {
            if (returnedData == null) return;

            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                {
                    UpdateDataWithRealTimeSnapshot((AlwaysOnAvailabilityGroupsSnapshot)returnedData);
                }
                else
                {
                    UpdateDataWithHistoricalSnapshot((AlwaysOnAvailabilityGroupsSnapshot)returnedData);
                }
            }
            
        }

        private void UpdateColors()
        {
            //Update cell colors depending on the alerts that have been configured
            foreach (UltraGridRow gridRow in alwaysOnAvailabilityGroupsGrid.Rows.GetAllNonGroupByRows())
            {
                DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                DataRow dataRow = dataRowView.Row;

                AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
                if (alertConfig != null)
                {
                    UpdateCellColor(Metric.AlwaysOnAvailabilityGroupRoleChange, alertConfig, gridRow, "Replica Role", 1);
                    UpdateCellColor(Metric.AlwaysOnSynchronizationHealthState, alertConfig, gridRow, "Synchronization Health", 1);
                    UpdateCellColor(Metric.AlwaysOnLogSendQueueSize, alertConfig, gridRow, "Log Send Queue (KB)", 1);
                    UpdateCellColor(Metric.AlwaysOnRedoQueueSize, alertConfig, gridRow, "Redo Queue (KB)", 1);
                    UpdateCellColor(Metric.AlwaysOnRedoRate, alertConfig, gridRow, "Redo Rate (KB/s)", 1);
                    UpdateCellColor(Metric.AlwaysOnEstimatedDataLossTime, alertConfig, gridRow, "Estimated Data Loss", 1);
                    UpdateCellColor(Metric.AlwaysOnEstimatedRecoveryTime, alertConfig, gridRow, "Estimated Recovery Time", 1);
                    UpdateCellColor(Metric.AlwaysOnSynchronizationPerformance, alertConfig, gridRow, "Synchronization Performance", 1);
                }
            }

        }
        /// <summary>
        /// Search the available databases combo box for a match and return the index of the match if one is found
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        private int FindAvailabilityGroupIndex(string group)
        {
            int index = -1;

            for (int i = 0; i < availabilityGroupsComboBox.Items.Count; i++)
            {
                if (string.CompareOrdinal(availabilityGroupsComboBox.Items[i].DataValue as string, group) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Formatting of timespans is limited so this function formats the timespan in the way we want
        /// </summary>
        /// <param name="span"></param>
        /// <param name="showSign"></param>
        /// <returns></returns>
        private static string FormatTimeSpan(TimeSpan span, bool showSign)
        {
            string sign = String.Empty;
            if (showSign && (span > TimeSpan.Zero)) sign = "+";
            //if the span is negative by a second (we will show the second so we must add the sign)
            if (span < TimeSpan.Zero & Math.Abs(span.TotalSeconds) >= 1) sign = "-";
            return sign + Math.Abs(span.Days).ToString("00") + "." +
                Math.Abs(span.Hours).ToString("00") + ":" +
                Math.Abs(span.Minutes).ToString("00");
            // +":" +
            //span.Seconds.ToString("00");
            //+ "." + span.Milliseconds.ToString("00");
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, string columnName, int adjustmentMultiplier)
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
                        //if this is the current role column we actually want to highlight based on deviation from preferred role

                        IComparable value = (IComparable)dataRow[columnName];

                        if (metric == Metric.AlwaysOnAvailabilityGroupRoleChange)
                        {
                            //if the preferred config value is normal or dont care then value must be 0 (good) else set value to 1 (bad)
                            if ((value.Equals("Primary") || value.Equals(1) || value.Equals("None") || value.Equals(255)))
                            {
                                cell.Appearance.BackColor = Color.Green;
                                cell.Appearance.ForeColor = Color.White;
                            }
                            else
                            {
                                cell.Appearance.BackColor = Color.Gold;
                                cell.Appearance.ResetForeColor();
                            }
                        }
                        else if (metric == Metric.AlwaysOnSynchronizationHealthState)
                        {
                            //if the preferred config value is normal or dont care then value must be 0 (good) else set value to 1 (bad)
                            if ((value.Equals("Healthy") || value.Equals(2) || value.Equals("None") || value.Equals(255)))
                            {
                                cell.Appearance.BackColor = Color.Green;
                                cell.Appearance.ForeColor = Color.White;
                            }
                            else
                            {
                                Idera.SQLdm.Common.Thresholds.MetricThresholdEntry mte = alertConfigItem.ThresholdEntry;
                                if (mte.CriticalThreshold.Enabled)
                                {
                                    cell.Appearance.BackColor = Color.Red;
                                    cell.Appearance.ForeColor = Color.White;
                                }
                                else if (mte.WarningThreshold.Enabled)
                                {
                                    cell.Appearance.BackColor = Color.Gold;
                                    cell.Appearance.ResetForeColor();
                                }
                                else if (mte.InfoThreshold.Enabled)
                                {
                                    cell.Appearance.BackColor = Color.Blue;
                                    cell.Appearance.ForeColor = Color.White;
                                }
                                else
                                {
                                    cell.Appearance.ResetBackColor();
                                    cell.Appearance.ResetForeColor();
                                }
                            }
                        }
                        else
                        {
                            //For Redo Rate we should not do the color display if the value is 0
                            if (!(metric == Metric.AlwaysOnRedoRate && value.Equals(0)))
                            {
                                switch (alertConfigItem.GetSeverity(value))
                                {
                                    case MonitoredState.Informational:
                                        cell.Appearance.BackColor = Color.Blue;
                                        cell.Appearance.ForeColor = Color.White;
                                        break;
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
            }
        }

        #region Persistent Settings
        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance = splitContainer1.Height - Settings.Default.DatabaseAlwaysOnViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer1.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer1.Height - splitContainer1.SplitterDistance;
            }

            if (Settings.Default.DatabaseAlwaysOnAvailabilityGroupsGrid is GridSettings)
            {
                lastAvailabilityGroupsSettings = Settings.Default.DatabaseAlwaysOnAvailabilityGroupsGrid;
                GridSettings.ApplySettingsToGrid(lastAvailabilityGroupsSettings, alwaysOnAvailabilityGroupsGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            try
            {
                GridSettings mainGridSettings = GridSettings.GetSettings(alwaysOnAvailabilityGroupsGrid);
                lastAvailabilityGroupsSettings = Settings.Default.DatabaseAlwaysOnAvailabilityGroupsGrid = mainGridSettings;

                if (lastMainSplitterDistance != splitContainer1.Height - splitContainer1.SplitterDistance)
                {
                    // Fixed panel is second panel, so save size of second panel
                    lastMainSplitterDistance = Settings.Default.DatabaseAlwaysOnViewMainSplitter =
                        splitContainer1.Height - splitContainer1.SplitterDistance;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while saving the alwayson view settings.", e);
            }
        }
        #endregion
        #region Public Functions

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesAlwaysOnView); 
        }

        #endregion
        #region Refresh overrides
        public override void RefreshView()
        {
            //if (historicalsnapshotdatetime == null)
            //{
            //    databasesalwayson_fill_panel.visible = true;

            //    base.refreshview();

            //}
            //else
            //{
            //    databasesalwayson_fill_panel.visible = false;
            //    applicationcontroller.default.setrefreshstatustext(properties.resources.historymodestatusbarlabel);
            //}

            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != null && (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime ||
                HistoricalStartDateTime != currentHistoricalStartDateTime || filterChanged))
            {
                DatabasesAlwaysOn_Fill_Panel.Visible = true;
                filterChanged = false;
                //historyModeLoadError = null;
                base.RefreshView();
            }
        }

        //cancel all asyncronous calls
        public override void CancelRefresh()
        {
            base.CancelRefresh();
        }

        #endregion
        //Event handlers for Operational Status Panel -  SQLDM 10.0.0 Ankit Nagpal
        #region Operational Status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            // Switching to real-time mode is the only supported opertional action at this time
            SwitchToRealTimeMode();
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }
        #endregion
        void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = selectedGrid.Rows.Count > 0 && selectedGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
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
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        CollapseAllGroups(selectedGrid);
                    }
                    break;
                case "expandAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        ExpandAllGroups(selectedGrid);
                    }
                    break;
            }
        }
        #region Grid
        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null && selectedGrid != null)
            {
                if (GroupBy)
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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

        private void CollapseAllGroups(UltraGrid grid)
        {
            grid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups(UltraGrid grid)
        {
            grid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            if (selectedGrid != null)
            {
                SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(selectedGrid);
                dialog.Show(this);
            }
        }

        private void PrintGrid()
        {
            if (selectedGrid != null)
            {
                ultraGridPrintDocument.Grid = selectedGrid;
                ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
                ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
                ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - {1}",
                                  ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                  DateTime.Now.ToString("G")
                        );
                ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

                ultraPrintPreviewDialog.ShowDialog();
            }
        }

        private void SaveGrid()
        {
            if (selectedGrid != null)
            {
                saveFileDialog.DefaultExt = "xls";
                if (selectedGrid == alwaysOnAvailabilityGroupsGrid)
                {
                    saveFileDialog.FileName = "AlwaysOn Groups";
                }
                else
                {
                    saveFileDialog.FileName = "AlwaysOn Statistics";
                }
                saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
                saveFileDialog.Title = "Save as Excel Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ultraGridExcelExporter.Export(selectedGrid, saveFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while exporting data.", ex);
                    }
                }
            }
        }

  
        #endregion

        #region Properties

        public event EventHandler ChartVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        /// <summary>
        /// Get or Set the Chart visibility and trigger state update event if changed
        /// </summary>
        public bool ChartVisible
        {
            get { return !splitContainer1.Panel2Collapsed; }
            set
            {
                splitContainer1.Panel2Collapsed = !value;
                RestoreChart();

                if (ChartVisibleChanged != null)
                {
                    ChartVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Availability Groups Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        #endregion

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        //void OnCurrentThemeChanged(object sender, EventArgs e)
        //{
        //    SetGridTheme();
        //}

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.transactionLogsGrid);
            themeManager.updateGridTheme(this.alwaysOnAvailabilityGroupsGrid);
        }

        public override object DoRefreshWork()
        {
            //this is largely redundant. The problem is that a customer is getting a button to allow him to access the screen
            //even on 2000.  If that is happenening there is no reason to think the application model has the right data
            var server = ApplicationModel.Default.GetInstanceStatus(instanceId);
            //if (server != null && server.InstanceVersion != null)
            //{
            //    if (server.InstanceVersion.Major <= 8)
            //    {
            //        MethodInvoker uiCode = () => ShowOperationalStatus(INVALID_VERSION, Properties.Resources.StatusWarningSmall);
            //        this.operationalStatusLabel.UIThread(uiCode);
            //        return null;
            //    }
            //}

            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;

            if (HistoricalSnapshotDateTime != null)
            {
                if(ViewMode == ServerViewMode.Historical)
                {
                    DateTime end = HistoricalSnapshotDateTime.Value;
                    DateTime start = end.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                    return GetHistoricalSnapshot(start, end);
                }
                else
                {
                    return GetHistoricalSnapshot(HistoricalStartDateTime.Value, historicalSnapshotDateTime.Value);
                }
            }

            else {

                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (!isPrePopulated || previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes)
                {
                    ForwardFillHistoricalData();
                    isPrePopulated = true;
                }

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();

                IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                AlwaysOnAvailabilityGroupsSnapshot _snapshotTopology = (AlwaysOnAvailabilityGroupsSnapshot)managementService.GetDatabaseAlwaysOnTopology(alwaysOnAvailabilityGroupsConfiguration);
                AlwaysOnAvailabilityGroupsSnapshot _snapshotStatistics = (AlwaysOnAvailabilityGroupsSnapshot)managementService.GetDatabaseAlwaysOnStatistics(alwaysOnAvailabilityGroupsConfiguration);

                //Merge the output of the snapshots into a single snaphot
                List<AlwaysOnStatistics> replicaStatistics = _snapshotStatistics.ReplicaStatistics;
                foreach (AlwaysOnStatistics stat in replicaStatistics)
                {
                    _snapshotTopology.AddReplicaStatistic(stat);
                }

                return _snapshotTopology;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Fetches historical snapshot between the specified start and end dates.
        /// </summary>
        private object GetHistoricalSnapshot(DateTime start, DateTime end)
        {
            return RepositoryHelper.GetInstanceAvailbilityGroupDetailsData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                    instanceId, start , end );
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Changes chart bindings when user switches between real and historical modes.
        /// </summary>
        private void FixupBindings(bool historical)
        {
            if (historical)
            {
                if (alwaysOnRateChart.DataSource == historyDataTable) return;
                alwaysOnRateChart.DataSource = historyDataTable;
            }
            else
            {
                if (alwaysOnRateChart.DataSource == realTimeDataTable) return;
                alwaysOnRateChart.DataSource = realTimeDataTable;

                // no reason to keep history data around
                historyDataTable.Clear();
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        private void ForwardFillHistoricalData()
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (realTimeDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    ServerSummaryHistoryData.GetForwardFillHistoricalRange(realTimeDataTable,
                        out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;

                    Log.InfoFormat("ForwardFillHistoricalData from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateSnapshots(startDateTime, endDateTime);
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Replaces stale real-time data with historical data.
        /// </summary>
        private void BackFillScheduledHistoricalData()
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (realTimeDataTable != null &&
                    Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = ServerSummaryHistoryData.GetBackFillHistoricalRange(realTimeDataTable,
                        out startDateTime, out endDateTime);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",
                        startDateTime, endDateTime);
                    PopulateSnapshots(startDateTime, endDateTime);
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Fetches data from repository.
        /// </summary>
        private void PopulateSnapshots(DateTime startDateTime, DateTime endDateTime)
        {
            var repositoryData = GetHistoricalSnapshot(startDateTime, endDateTime);
            MethodInvoker UICode = () => PopulateAlwaysOnStatistics(repositoryData);
            this.UIThread(UICode);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Populates data fected from repoitory.
        /// </summary>
        private void PopulateAlwaysOnStatistics(object returnedData)
        {
            //if valid data has been returned
            if (returnedData != null && returnedData is AlwaysOnAvailabilityGroupsSnapshot)
            {
                // SQLDM-27811 - SQLDM 10.2 : OutofMemoryException is thrown on desktop console under 'Databases-->AvailabilityGroups' tab
                var currentAvailabilityGroupData = (AlwaysOnAvailabilityGroupsSnapshot)returnedData;

                if (currentAvailabilityGroupData.Error == null)
                {
                    lock (updatingAlwaysOnLock)
                    {
                        IDictionary<Guid, AvailabilityGroup> availabilityGroups = currentAvailabilityGroupData.AvailabilityGroups;
                        //if the snapshot does contain valid Availability Groups 
                        if (availabilityGroups != null && availabilityGroups.Count > 0)
                        {
                            // load real time datatable
                            realTimeDataTable.BeginLoadData();
                            foreach (AvailabilityGroup avlGroups in availabilityGroups.Values)
                            {
                                IDictionary<Guid, AvailabilityReplica> replicas = avlGroups.Replicas;
                                if (replicas != null && replicas.Count > 0)
                                {
                                    foreach (AvailabilityReplica replica in replicas.Values)
                                    {
                                        IDictionary<long, AlwaysOnDatabase> databases = replica.Databases;
                                        if (databases != null && databases.Count > 0)
                                        {
                                            foreach (AlwaysOnDatabase db in databases.Values)
                                            {
                                                AlwaysOnStatistics statistics =
                                                    currentAvailabilityGroupData.GetReplicaStatistics(replica.GroupId, replica.ReplicaId, db.DatabaseId);

                                                if (statistics == null)
                                                {
                                                    LOG.Warn("Could not find statistics details for Group id [" + replica.GroupId +
                                                        "], Replica id [" + replica.ReplicaId + "], Database id [" + db.DatabaseId + "]");
                                                    continue;
                                                }

                                                AddHistoryData(realTimeDataTable, currentAvailabilityGroupData, replica.GroupId, replica.ReplicaId, db.DatabaseId,
                                                       db.DatabaseName, avlGroups.GroupId);
                                            }
                                        }
                                    }
                                }
                            }
                            realTimeDataTable.EndLoadData();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Selects first row in the grid.
        /// </summary>
        private void SelectFirstRowInGrid()
        {
            if (alwaysOnAvailabilityGroupsGrid == null || alwaysOnAvailabilityGroupsGrid.Rows.Count == 0)
                return;

            selectedGroupId = selectedGroupId = selectedDatabaseName = null;
            UltraGridRow row = alwaysOnAvailabilityGroupsGrid.Rows[0];
            row.Selected = true;
            alwaysOnAvailabilityGroupsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
        }

        private void mouseEnter_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
                appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefreshHover;
        }
        private void mouseLeave_refreshDatabasesButton(Object Sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Dark")
                appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
        }
        private void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            if (Settings.Default.ColorScheme == "Dark")
            {
                if (!refreshDatabasesButton.Enabled)
                    appearance11.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
        }
    }
}
