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
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Objects;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    internal partial class DatabasesTempdbView : ServerBaseView, IDatabasesView, IShowFilterDialog
    {
        private const string NO_ITEMS = @"There are no items to show in this view.";

        private const string CONTENTION_CHART_TEXT = "Tempdb Contention";
        private const string VERSIONSTORE_CHART_TEXT = "Version Store Cleanup Rate";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        private const string INVALID_VERSION = @"Only applies to SQL 2005 and later.";
        private const string HISTORY_ERROR = @"An error occurred while loading a historical snapshot."; //SqlDM 10.2 (Anshul Aggarwal) - New History Browser

        private bool filterChanged = false;
        private SessionSnapshot currentSessionSnapshot = null;
        private DatabaseFilesSnapshot currentFileSnapshot = null;
        private SessionsConfiguration sessionsConfiguration;
        private DatabaseFilesConfiguration filesConfiguration;
        private DateTime? historicalSnapshotDateTime = null;
        private static readonly object updateLock = new object();
        private readonly Dictionary<Pair<int?, DateTime?>, UltraDataRow> rowLookupTable = new Dictionary<Pair<int?, DateTime?>, UltraDataRow>();
        private bool initialized = false;
        private int? _selectedSpid = null;
        private bool selectedSpidIsSystem = false;
        private int? selectedSpidArgument = null;
        private string selectedSpidCommand = string.Empty;
        private bool isPrePopulated = false;

        private UltraGridColumn selectedColumn = null;
        private DataView fileSpaceUsedChartData;
        private DataTable fileSpaceUsedDataTable;
        private DataTable statsDataTable;

        private DataView _histFileSpaceUsedChartData;
        private DataTable _histFileSpaceUsedDataTable;
        private DataTable _histStatsDataTable;

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private string lastContentionChartVisible = VERSIONSTORE_CHART_TEXT;
        private Exception historyModeLoadError = null;
        private Chart contextMenuSelectedChart = null;

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

        public DatabasesTempdbView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            tempdbSessionsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            // create the initial configuration
            sessionsConfiguration = new SessionsConfiguration(instanceId, currentSessionSnapshot);//JSFIX
            filesConfiguration = new DatabaseFilesConfiguration(instanceId,new List<string>{"tempdb"});

            InitializeDataTables();

            ChartFxExtensions.SetContextMenu(fileSpaceUsedChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(totalSpaceUsedChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(contentionChart, toolbarsManager);
            InitializeCharts();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }

        public string SelectedDatabaseFilter
        {
            get { return "tempdb"; }
        }

        // Events to notify of changes in settings for the view
        public event EventHandler HistoricalSnapshotDateTimeChanged;
        public event EventHandler FilterChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler TraceAllowedChanged;
        public event EventHandler KillAllowedChanged;

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
                    tempdbSessionsGridDataSource.Rows.Clear();
                    rowLookupTable.Clear();

                    if (HistoricalSnapshotDateTimeChanged != null)
                    {
                        HistoricalSnapshotDateTimeChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Get or Set the Sessions Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !tempdbSessionsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                tempdbSessionsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public SessionsConfiguration SessionsConfiguration
        {
            get { return sessionsConfiguration; }
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.DatabasesTempdbView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is int)
            {
                selectedSpidArgument = (int)argument;
                foreach (UltraGridRow row in tempdbSessionsGrid.Rows.GetAllNonGroupByRows())
                {
                    if ((int)row.Cells["Session ID"].Value == (int)argument)
                    {
                        selectedSpidIsSystem = (bool)((string)row.Cells["Type"].Value == "System");
                        selectedSpid = (int)argument;
                        row.Selected = true;
                        tempdbSessionsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        selectedSpidArgument = null;
                        break;
                    }
                }
            }
            else if (argument is SessionsConfiguration)
            {
                sessionsConfiguration.UpdateValues((SessionsConfiguration)argument);
                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
                filterChanged = true;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.TempdbViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            if (Settings.Default.TempdbViewMainGrid is GridSettings && Settings.Default.TempdbViewMainGrid != null)
            {
                lastMainGridSettings = Settings.Default.TempdbViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, tempdbSessionsGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            lastContentionChartVisible = Settings.Default.TempdbViewContentionButton;
            ConfigureContentionChart(lastContentionChartVisible);
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(tempdbSessionsGrid);
            string buttonValue = contentionHeaderButton.Text;
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastContentionChartVisible != buttonValue
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.TempdbViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.TempdbViewMainGrid = gridSettings;
                lastContentionChartVisible = Settings.Default.TempdbViewContentionButton = buttonValue;

            }
        }

        public void ShowFilter()
        {
            SessionsConfiguration selectFilter = new TempdbConfiguration(instanceId, null);
            selectFilter.UpdateValues(sessionsConfiguration);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                sessionsConfiguration.UpdateValues(selectFilter);
                filterChanged = true;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != null && (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime ||
                HistoricalStartDateTime != currentHistoricalStartDateTime || filterChanged))
            {
                filterChanged = false;
                //historyModeLoadError = null;
                base.RefreshView();
                //SQLdm 10.1 (Barkha khatri) -- SQLDM-25894 fix
                //if (!isUserSysAdmin)
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE);
                //else
                //    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING);
               


            }
        }

        #region DataTables

        private void InitializeDataTables()
        {
            fileSpaceUsedDataTable = new DataTable();
            statsDataTable = new DataTable();
            fileSpaceUsedDataTable.Columns.Add("Date", typeof(DateTime));
            fileSpaceUsedDataTable.Columns.Add("Logical Filename", typeof(string));
            fileSpaceUsedDataTable.Columns.Add("File Path", typeof(string));
            fileSpaceUsedDataTable.Columns.Add("User Objects", typeof(double));
            fileSpaceUsedDataTable.Columns.Add("Internal Objects", typeof(double));
            fileSpaceUsedDataTable.Columns.Add("Version Store", typeof(double));
            fileSpaceUsedDataTable.Columns.Add("Mixed Extents", typeof(double));
            fileSpaceUsedDataTable.Columns.Add("Unallocated Space", typeof(double));

            fileSpaceUsedDataTable.PrimaryKey = new DataColumn[] { fileSpaceUsedDataTable.Columns["Logical Filename"] };
            fileSpaceUsedDataTable.CaseSensitive = true;

            fileSpaceUsedDataTable.DefaultView.Sort = "Logical Filename";

            fileSpaceUsedChartData = new DataView(fileSpaceUsedDataTable, null, "[Logical Filename] desc", DataViewRowState.CurrentRows);

            _histFileSpaceUsedDataTable = fileSpaceUsedDataTable.Clone();
            _histFileSpaceUsedChartData = new DataView(_histFileSpaceUsedDataTable, null, "[Logical Filename] desc", DataViewRowState.CurrentRows);

            statsDataTable.Columns.Add("Date", typeof(DateTime));
            statsDataTable.Columns.Add("User Objects", typeof(double));
            statsDataTable.Columns.Add("Internal Objects", typeof(double));
            statsDataTable.Columns.Add("Version Store", typeof(double));
            statsDataTable.Columns.Add("Mixed Extents", typeof(double));
            statsDataTable.Columns.Add("Unallocated Space", typeof(double));
            statsDataTable.Columns.Add("Used Space", typeof (double));
            statsDataTable.Columns.Add("PFS Wait", typeof(double));
            statsDataTable.Columns.Add("GAM Wait", typeof(double));
            statsDataTable.Columns.Add("SGAM Wait", typeof(double));
            statsDataTable.Columns.Add("Version Generation Rate", typeof(decimal));
            statsDataTable.Columns.Add("Version Cleanup Rate", typeof(decimal));
            statsDataTable.Columns.Add("IsHistorical", typeof(bool));
            statsDataTable.DefaultView.Sort = "Date";

            _histStatsDataTable = statsDataTable.Clone();
        }

        private void GroomHistoryData()
        {
            if (statsDataTable != null && HistoricalSnapshotDateTime == null)
            {
                var selectFilter = ServerSummaryHistoryData.GetGroomingFilter("Date");
                var groomedRows = statsDataTable.Select(selectFilter);
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                statsDataTable.AcceptChanges();
            }
        }

        #endregion

        #region chart

        private void InitializeCharts()
        {
            InitializeFileSpaceUsedChart();
            InitializeTotalSpaceUsedChart();
            InitializeContentionChart();
            InitalizeDrilldown(totalSpaceUsedChart, contentionChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        #region fileSpaceUsed chart

        private void InitializeFileSpaceUsedChart()
        {
            fileSpaceUsedChart.Tag = fileSpaceUsedHeaderStripLabel;
            fileSpaceUsedChart.Printer.Orientation = PageOrientation.Landscape;
            fileSpaceUsedChart.Printer.Compress = true;
            fileSpaceUsedChart.Printer.ForceColors = true;
            fileSpaceUsedChart.Printer.Document.DocumentName = "Tempdb Space Used By File";
            fileSpaceUsedChart.ToolBar.RemoveAt(0);
            fileSpaceUsedChart.DataSource = fileSpaceUsedChartData;

            ConfigureFileSpaceUsedChart();
            fileSpaceUsedChart.Visible = false;
        }

        private void ConfigureFileSpaceUsedChart()
        {
            fileSpaceUsedChart.SuspendLayout();
            fileSpaceUsedChart.DataSourceSettings.Fields.Clear();

            FieldMap filenameFieldMap = new FieldMap("Logical Filename", FieldUsage.Label);
            filenameFieldMap.DisplayName = "File";
            FieldMap userObjectsFieldMap = new FieldMap("User Objects", FieldUsage.Value);
            userObjectsFieldMap.DisplayName = "User Objects";
            FieldMap internalObjectsFieldMap = new FieldMap("Internal Objects", FieldUsage.Value);
            internalObjectsFieldMap.DisplayName = "Internal Objects";
            FieldMap versionStoreFieldMap = new FieldMap("Version Store", FieldUsage.Value);
            versionStoreFieldMap.DisplayName = "Version Store";
            FieldMap mixedExtentsFieldMap = new FieldMap("Mixed Extents", FieldUsage.Value);
            mixedExtentsFieldMap.DisplayName = "Mixed Extents";
            FieldMap unallocatedFieldMap = new FieldMap("Unallocated Space", FieldUsage.Value);
            unallocatedFieldMap.DisplayName = "Unallocated Space";

            fileSpaceUsedChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                filenameFieldMap,
                userObjectsFieldMap,
                internalObjectsFieldMap,
                versionStoreFieldMap,
                mixedExtentsFieldMap,
            unallocatedFieldMap});
            fileSpaceUsedChart.AxisY.AutoScale = true;
            fileSpaceUsedChart.AxisY.DataFormat.Decimals = 2;
            fileSpaceUsedChart.ToolTipFormat = "%l\n%s\n%v MB\n%x";

            fileSpaceUsedChart.DataSourceSettings.ReloadData();

            if (fileSpaceUsedChart.Series.Count >= 5)
            {
                ChangeSeriesColorA(fileSpaceUsedChart.Series[0],175);
                ChangeSeriesColorA(fileSpaceUsedChart.Series[1], 175);
                ChangeSeriesColorA(fileSpaceUsedChart.Series[2], 175);
                ChangeSeriesColorA(fileSpaceUsedChart.Series[3], 175);
                fileSpaceUsedChart.Series[4].Color = Color.FromArgb(175, 101, 0, 168);
            }
            fileSpaceUsedChart.Invalidate();
            fileSpaceUsedChart.ResumeLayout();
            //capacityUsageChart_Resize(fileSpaceUsedChart, new EventArgs());
        }

        private static void ChangeSeriesColorA(SeriesAttributes series, int A )
        {
            series.Color = Color.FromArgb(A, series.Color.R, series.Color.G, series.Color.B);
        }

        #endregion


        #region totalSpaceUsed chart
        private void InitializeTotalSpaceUsedChart()
        {
            totalSpaceUsedChart.Tag = totalSpaceUsedHeaderStripLabel;
            totalSpaceUsedChart.Printer.Orientation = PageOrientation.Landscape;
            totalSpaceUsedChart.Printer.Compress = true;
            totalSpaceUsedChart.Printer.ForceColors = true;
            totalSpaceUsedChart.Printer.Document.DocumentName = "Tempdb Space Used Over Time";
            totalSpaceUsedChart.ToolBar.RemoveAt(0);
            totalSpaceUsedChart.DataSource = statsDataTable;
            totalSpaceUsedChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
            totalSpaceUsedChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ConfigureTotalSpaceUsedChart();
            totalSpaceUsedChart.Visible = false;
        }

        private void ConfigureTotalSpaceUsedChart()
        {
            totalSpaceUsedChart.SuspendLayout();
            totalSpaceUsedChart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            FieldMap userObjectsFieldMap = new FieldMap("User Objects", FieldUsage.Value);
            userObjectsFieldMap.DisplayName = "User Objects";
            FieldMap internalObjectsFieldMap = new FieldMap("Internal Objects", FieldUsage.Value);
            internalObjectsFieldMap.DisplayName = "Internal Objects";
            FieldMap versionStoreFieldMap = new FieldMap("Version Store", FieldUsage.Value);
            versionStoreFieldMap.DisplayName = "Version Store";
            FieldMap mixedExtentsFieldMap = new FieldMap("Mixed Extents", FieldUsage.Value);
            mixedExtentsFieldMap.DisplayName = "Mixed Extents";
            FieldMap usedFieldMap = new FieldMap("Used Space",FieldUsage.Value);
            usedFieldMap.DisplayName = "Used Space";
            FieldMap unallocatedFieldMap = new FieldMap("Unallocated Space", FieldUsage.Value);
            unallocatedFieldMap.DisplayName = "Unallocated Space";
            

            totalSpaceUsedChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                dateFieldMap,
                userObjectsFieldMap,
                internalObjectsFieldMap,
                versionStoreFieldMap,
                mixedExtentsFieldMap,
                usedFieldMap,
            unallocatedFieldMap});
            totalSpaceUsedChart.AxisY.AutoScale = true;
            totalSpaceUsedChart.AxisY.DataFormat.Decimals = 2;
           

            totalSpaceUsedChart.DataSourceSettings.ReloadData();

            if (totalSpaceUsedChart.Series.Count >= 6)
            {
                ChangeSeriesColorA(totalSpaceUsedChart.Series[0], 175);
                ChangeSeriesColorA(totalSpaceUsedChart.Series[1], 175);
                ChangeSeriesColorA(totalSpaceUsedChart.Series[2], 175);
                ChangeSeriesColorA(totalSpaceUsedChart.Series[3], 175);
                totalSpaceUsedChart.Series[4].AlternateColor = Color.FromArgb(175, 0, 200, 245);
                totalSpaceUsedChart.Series[4].Color = Color.FromArgb(175, 0, 160, 196);
                totalSpaceUsedChart.Series[4].Pane = totalSpaceUsedChart.Panes[1];
                totalSpaceUsedChart.Series[5].AlternateColor = Color.FromArgb(255, 126, 0, 210);
                totalSpaceUsedChart.Series[5].Color = Color.FromArgb(175, 101, 0, 168);
                totalSpaceUsedChart.Series[5].Pane = totalSpaceUsedChart.Panes[1];
                //totalSpaceUsedChart.Series[5].FillMode = FillMode.Pattern;
                //totalSpaceUsedChart.Series[5].Pattern = System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal;
                totalSpaceUsedChart.ToolTipFormat = "%l\n%s\n%v MB\n%x";
            }
            totalSpaceUsedChart.Invalidate();
            totalSpaceUsedChart.ResumeLayout();
        }

       

        #endregion

        #region contention chart

        private void InitializeContentionChart()
        {
            contentionChart.Tag = contentionHeaderButton;
            contentionChart.Printer.Orientation = PageOrientation.Landscape;
            contentionChart.Printer.Compress = true;
            contentionChart.Printer.ForceColors = true;
            contentionChart.Printer.Document.DocumentName = "Tempdb Contention";
            contentionChart.ToolBar.RemoveAt(0);
            contentionChart.DataSource = statsDataTable;
            
            contentionChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
            contentionChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            ConfigureContentionChart(lastContentionChartVisible);
            contentionChart.Visible = false;
        }

        private void ConfigureContentionChart(string button)
        {
            contentionHeaderButton.Text = button;

            contentionChart.SuspendLayout();
            contentionChart.DataSourceSettings.Fields.Clear();

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            switch (button)
            {
                case "Tempdb Contention":
            
                    FieldMap pfsFieldMap = new FieldMap("PFS Wait", FieldUsage.Value);
                    pfsFieldMap.DisplayName = "PFS Page Waits";
                    FieldMap gamFieldMap = new FieldMap("GAM Wait", FieldUsage.Value);
                    gamFieldMap.DisplayName = "GAM Page Waits";
                    FieldMap sgamFieldMap = new FieldMap("SGAM Wait", FieldUsage.Value);
                    sgamFieldMap.DisplayName = "SGAM Page Waits";

                    contentionChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        pfsFieldMap,
                        gamFieldMap,
                        sgamFieldMap});

                    contentionChart.AllSeries.Gallery = Gallery.Lines;
                    contentionChart.AxisY.AutoScale = true;
                    contentionChart.AxisY.Title.Text = "Wait Milliseconds";
                    contentionChart.AxisY.DataFormat.Decimals = 2;
                    contentionChart.AllSeries.Border.Visible = false;
                    contentionChart.ToolTipFormat = "%l\n%s\n%v ms\n%x";
                    break;
                case "Version Store Cleanup Rate":
                    FieldMap cleanpFieldMap = new FieldMap("Version Cleanup Rate", FieldUsage.Value);
                    cleanpFieldMap.DisplayName = "Version Cleanup Rate";
                    FieldMap generationFieldMap = new FieldMap("Version Generation Rate", FieldUsage.Value);
                    generationFieldMap.DisplayName = "Version Generation Rate";

                    contentionChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        cleanpFieldMap,
                        generationFieldMap
                        });
                    contentionChart.AllSeries.Gallery = Gallery.Area;
                    contentionChart.AllSeries.AlternateColor = System.Drawing.Color.Transparent;
                    contentionChart.AllSeries.Border.Visible = true;
                    contentionChart.AllSeries.FillMode = ChartFX.WinForms.FillMode.Gradient;
                    contentionChart.AxisY.AutoScale = true;
                    contentionChart.AxisY.DataFormat.Decimals = 2;
                    contentionChart.AxisY.Title.Text = "Kilobytes per Second";
                    contentionChart.ToolTipFormat = "%l\n%s\n%v KB/sec\n%x";
                    break;
            }
            

            contentionChart.DataSourceSettings.ReloadData();
            contentionChart.Invalidate();
            contentionChart.ResumeLayout();
        }

        #endregion


        #endregion

        #region grid



        private void tempdbSessionsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                {
                    var columnHeader =
                        contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridDataContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void tempdbSessionsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (tempdbSessionsGrid.Rows.Count > 0
                && tempdbSessionsGrid.Selected.Rows.Count == 1
                && tempdbSessionsGrid.Selected.Rows[0].IsDataRow)
            {
                selectedSpidIsSystem = ((string)tempdbSessionsGrid.Selected.Rows[0].Cells["Type"].Value == "System");
                selectedSpid = (int)tempdbSessionsGrid.Selected.Rows[0].Cells["Session ID"].Value;
                selectedSpidCommand = tempdbSessionsGrid.Selected.Rows[0].Cells["Last Command"].Text;
            }
            else
            {
                selectedSpid = null;
            }
        }

        private void tempdbSessionsGrid_FilterRow(object sender, FilterRowEventArgs e)
        {
            e.RowFilteredOut = SessionsHelper.RowShouldBeFiltered(sessionsConfiguration, (UltraDataRow)e.Row.ListObject, historicalSnapshotDateTime != null);
        }

        private void ultraGridExcelExporter_CellExporting(object sender, Infragistics.Win.UltraWinGrid.ExcelExport.CellExportingEventArgs e)
        {
            if (e.GridColumn.Key == "Last Command")
            {
                string value = e.Value.ToString();

                if (value.Length > 32767)
                {
                    e.Value = string.Format("{0}...", value.Substring(0, 32764));
                }
            }
        }

        #endregion

        public override object DoRefreshWork()
        {
            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;

            //this is largely redundant. The problem is that a customer is getting a button to allow him to access the screen
            //even on 2000.  If that is happenening there is no reason to think the application model has the right data
            var server = ApplicationModel.Default.GetInstanceStatus(instanceId);
            if (server != null && server.InstanceVersion != null)
            {
                if (server.InstanceVersion.Major <= 8)
                {
                    MethodInvoker uiCode = () => ShowOperationalStatus(INVALID_VERSION, Properties.Resources.StatusWarningSmall);
                    this.operationalStatusLabel.UIThread(uiCode);
                    return null;
                }
            }

            if (HistoricalSnapshotDateTime == null)
            {
                return GetRealTimeSnapshot(previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes);
            }
            else
            {
                if (ViewMode == ServerViewMode.Historical)
                {
                    return GetHistoricalSnapshot(null, HistoricalSnapshotDateTime.Value,
                        ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
                else
                {
                    return GetHistoricalSnapshot(ApplicationModel.Default.HistoryTimeValue.StartDateTime.Value,
                       ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value, ApplicationModel.Default.HistoryTimeValue.CustomTimeMinutes);
                }
            }
        }

        private void FixupBindings(bool historical)
        {
            if (historical)
            {
                if (contentionChart.DataSource == _histStatsDataTable) return;
                contentionChart.DataSource = _histStatsDataTable;
                totalSpaceUsedChart.DataSource = _histStatsDataTable;
                fileSpaceUsedChart.DataSource = _histFileSpaceUsedChartData;
            }
            else
            {
                if (contentionChart.DataSource == statsDataTable) return;
                contentionChart.DataSource = statsDataTable;
                totalSpaceUsedChart.DataSource = statsDataTable;
                fileSpaceUsedChart.DataSource = fileSpaceUsedChartData;

                // no reason to keep history data around
                _histStatsDataTable.Clear();
                _histFileSpaceUsedDataTable.Clear();
            }
        }

        private void PrePopulate()
        {
            PopulateSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
        }

        private void PopulateSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes, bool clearData)
        {
            var repositoryData =
           RepositoryHelper.GetTempdbSummaryData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                          instanceId, endDateTime, minutes, startDateTime);

            MethodInvoker UICode = () => PopulateTempdbDataFromHistory(statsDataTable, repositoryData, clearData, false);
            this.UIThread(UICode);
        }

        private void Backfill()
        {
            var rtdt = this.statsDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PrePopulate();
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow["Date"];

            // SqlDM 10.2 (New History Browser) - If SD = 5mins and KD = 60min, then we need to reload entire data only if
            // we have no data for previous max(5,60) = 60 minutes. 
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(ServerSummaryHistoryData.MaximumKeepData))
            { 
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                PrePopulate();
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                MethodInvoker UICode = () => FixupBindings(false);
                this.UIThread(UICode);
                return;
            }

            var repositoryData = RepositoryHelper.GetTempdbSummaryData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                        instanceId, now, (int)timeDiff.TotalMinutes);

            MethodInvoker UICode2 = () => PopulateTempdbDataFromHistory(statsDataTable, repositoryData, false, false);
            this.UIThread(UICode2);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Replaces realtime data that is now stale with data from repository.
        /// </summary>
        private void BackFillScheduledHistoricalData()
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (statsDataTable != null &&
                    Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = ServerSummaryHistoryData.GetBackFillHistoricalRange(statsDataTable,
                        out startDateTime, out endDateTime);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",
                        startDateTime, endDateTime);
                    PopulateSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        private void ForwardFillHistoricalData()
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (statsDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    ServerSummaryHistoryData.GetForwardFillHistoricalRange(statsDataTable,
                        out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }
        private void PopulateTempdbDataFromHistory(DataTable targetData, DataTable repositoryData, bool clearData, bool history)
        {
            FixupBindings(history);

            targetData.BeginLoadData();
            if (clearData) targetData.Clear();

            int insertionIndex = -1;
            if (repositoryData.Rows.Count > 0 && !clearData)
            {
                insertionIndex = ServerSummaryHistoryData.GetInsertionIndex(targetData, "Date",
                    (DateTime)repositoryData.Rows[0]["UTCCollectionDateTime"]);
            }
            foreach (DataRow row in repositoryData.Rows)
            {
                DataRow newRow = targetData.NewRow();
                object value;
                double spaceUsed = 0;

                value = row["UTCCollectionDateTime"];

                if (value != DBNull.Value )
                {
                    newRow["Date"] = (DateTime) value;
                }
                newRow["IsHistorical"] = true;
                value = row["UserObjectsInMB"];
                if (value != DBNull.Value )
                {
                    newRow["User Objects"] = Convert.ToDouble(value);
                    spaceUsed += Convert.ToDouble(value);

                }

                value = row["InternalObjectsInMB"];
                if (value != DBNull.Value )
                {
                    newRow["Internal Objects"] =  Convert.ToDouble(value);
                    spaceUsed += Convert.ToDouble(value);
                }

                value = row["VersionStoreInMB"];
                if (value != DBNull.Value )
                {
                  newRow["Version Store"] =   Convert.ToDouble(value);;
                  spaceUsed += Convert.ToDouble(value);
                }

                value = row["MixedExtentsInMB"];
                if (value != DBNull.Value )
                {
                    newRow["Mixed Extents"] =  Convert.ToDouble(value);
                    spaceUsed += Convert.ToDouble(value);
                }

                value = row["UnallocatedSpaceInMB"];
                if (value != DBNull.Value )
                {
                    newRow["Unallocated Space"] =  Convert.ToDouble(value);
                    
                }

                newRow["Used Space"] = spaceUsed;

                value = row["TempdbPFSWaitTimeMilliseconds"];
                if (value != DBNull.Value )
                {
                    newRow["PFS Wait"] =  Convert.ToDouble(value);

                }

                value = row["TempdbGAMWaitTimeMilliseconds"];
                if (value != DBNull.Value )
                {
                    newRow["GAM Wait"] =  Convert.ToDouble(value);
                
                }

                value = row["TempdbSGAMWaitTimeMilliseconds"];
                if (value != DBNull.Value )
                {
                    newRow["SGAM Wait"] =   Convert.ToDouble(value);
                
                }

                value = row["VersionStoreGenerationKilobytesPerSec"];
                if (value != DBNull.Value )
                {
                    newRow["Version Generation Rate"] = Convert.ToDecimal(value);
                
                }

                value = row["VersionStoreCleanupKilobytesPerSec"];
                if (value != DBNull.Value)
                {
                    newRow["Version Cleanup Rate"] = Convert.ToDecimal(value);

                }
                if (insertionIndex >= 0)
                    targetData.Rows.InsertAt(newRow, insertionIndex++);
                else
                    targetData.Rows.Add(newRow);
            }
            targetData.AcceptChanges();
            targetData.EndLoadData();
        }

        private Pair<SessionSnapshot,DatabaseFilesSnapshot> GetRealTimeSnapshot(bool visibleMinutesIncreased)
        {
            if (!isPrePopulated)
            {
                PrePopulate();
                isPrePopulated = true;
            }
            else
            {
                Backfill();

                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (visibleMinutesIncreased)
                    ForwardFillHistoricalData();

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();
            }

            var snapshots = new Pair<SessionSnapshot, DatabaseFilesSnapshot>();
            var managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            sessionsConfiguration.SetPreviousValues(currentSessionSnapshot);

            if (currentFileSnapshot != null)
                filesConfiguration.PreviousTempdbStatistics = currentFileSnapshot.TempdbStatistics;

            snapshots.First = managementService.GetSessions(sessionsConfiguration);
            snapshots.Second = managementService.GetDatabaseFiles(filesConfiguration);
            return snapshots;
        }

        private Pair<SessionSnapshot, DataTable> GetHistoricalSnapshot(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            var snapshots = new Pair<SessionSnapshot, DataTable>();

            snapshots.First = RepositoryHelper.GetSessionsDetails(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                    instanceId, endDateTime, startDateTime, minutes);
            snapshots.Second = RepositoryHelper.GetTempdbFileData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                     instanceId, endDateTime, minutes, startDateTime);
            var repositoryData = RepositoryHelper.GetTempdbSummaryData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                           instanceId, endDateTime, minutes, startDateTime);

            MethodInvoker UICode = () => PopulateTempdbDataFromHistory(_histStatsDataTable, repositoryData, true, true);
            this.UIThread(UICode);

            return snapshots;
        }

        public override void UpdateData(object data)
        {
            if (data == null) return;

            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                {
                    UpdateDataWithRealTimeSnapshot((Pair<SessionSnapshot,DatabaseFilesSnapshot>)data);
                }
                else
                {
                    UpdateDataWithHistoricalSnapshot((Pair<SessionSnapshot,DataTable>)data);
                }
                GroomHistoryData();
            }
        }

        private void UpdateDataWithRealTimeSnapshot(Pair<SessionSnapshot,DatabaseFilesSnapshot> snapshots)
        {
            operationalStatusPanel.Visible = false;
            DatabasesTempdbView_Fill_Panel.Visible = true;

            int serverVersion = GetServerMajorVersion(snapshots.First);

            if (snapshots.First != null && snapshots.Second != null)
            {
                if (snapshots.First.Error == null & serverVersion > 8)
                {
                    UpdateSessionsData(snapshots.First);
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
                else
                {
                    tempdbSessionsGrid.Visible = false;
                    tempdbSessionsGridStatusLabel.Text = serverVersion > 8 ? UNABLE_TO_UPDATE : INVALID_VERSION;
                    if (serverVersion <= 8)
                    {
                        ShowOperationalStatus(INVALID_VERSION,
                            Properties.Resources.StatusWarningSmall);
                    }
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshots.First.Error));
                }

                if (snapshots.Second.Error == null & serverVersion > 8)
                {
                    UpdateTempdbData(snapshots.Second);
                    UpdateChartDataFilter();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
                else
                {
                    fileSpaceUsedChart.Visible = false;
                    tempdbSessionsGridStatusLabel.Text = serverVersion > 8 ? UNABLE_TO_UPDATE : INVALID_VERSION;

                    if (serverVersion <= 8)
                    {
                        ShowOperationalStatus(INVALID_VERSION,
                            Properties.Resources.StatusWarningSmall);
                    }

                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshots.Second.Error));
                }
            }
            else
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now,
                                                            new DesktopClientException("Snapshot data is null.")));
            }
        }

        private void UpdateDataWithHistoricalSnapshot(Pair<SessionSnapshot, DataTable> snapshots)
        {
            if (snapshots.First != null)
            {
                int serverVersion = GetServerMajorVersion(snapshots.First);

                if (snapshots.First.Error == null & serverVersion > 8)
                {
                    UpdateSessionsData(snapshots.First);
                    DatabasesTempdbView_Fill_Panel.Visible = true;
                    ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                          Properties.Resources.StatusWarningSmall);
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                }
                else
                {
                    tempdbSessionsGrid.Visible = false;
                    tempdbSessionsGridStatusLabel.Text = serverVersion > 8 ? UNABLE_TO_UPDATE : INVALID_VERSION;

                    if (serverVersion <= 8)
                    {
                        ShowOperationalStatus(INVALID_VERSION,
                            Properties.Resources.StatusWarningSmall);
                    }

                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshots.First.Error));
                }
                if (snapshots.Second != null & serverVersion > 8)
                {
                    UpdateTempdbFileDataFromHistory(snapshots.Second);
                }
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                DatabasesTempdbView_Fill_Panel.Visible = false;
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateSessionsData(SessionSnapshot snapshot)
        {
            if (snapshot != null)
            {
                // first remove any sessions from the previous data that no longer exist
                tempdbSessionsGridDataSource.SuspendBindingNotifications();

                var deleteList = rowLookupTable.Keys.Where(key => !snapshot.SessionList.ContainsKey(key)).ToList();
                foreach (var key in deleteList)
                {
                    var row = rowLookupTable[key];
                    tempdbSessionsGridDataSource.Rows.Remove(row);
                    rowLookupTable.Remove(key);
                }

                // Update Sessions Grid
                if (snapshot.SessionList.Count > 0)
                {
                    //string sqlCmdNotAvailable = SQL_CMD_NOT_AVAILABLE;

                    if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.InputBufferLimited)
                    {
                        if (ApplicationModel.Default.ActiveInstances[instanceId].Instance.InputBufferLimiter <= snapshot.SessionList.Count)
                        {
                            //sqlCmdNotAvailable = SQL_CMD_NOT_AVAILABLE_DBCC;
                        }
                    }

                    //now update any matching sessions or add new ones
                    foreach (var session in snapshot.SessionList.Values)
                    {
                        UltraDataRow existingRow;

                        decimal sessionUserSpaceAllocated = session.SessionUserAllocatedTotal != null ? (session.SessionUserAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionUserSpaceDeallocated = session.SessionUserDeallocatedTotal != null ? (session.SessionUserDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionSpaceUsed = sessionUserSpaceAllocated >= sessionUserSpaceDeallocated ? sessionUserSpaceAllocated - sessionUserSpaceDeallocated : 0;
                        decimal taskUserSpaceAllocated = session.TaskUserAllocatedTotal != null ? (session.TaskUserAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskUserSpaceDeallocated = session.TaskUserDeallocatedTotal != null ? (session.TaskUserDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskSpaceUsed = taskUserSpaceAllocated >= taskUserSpaceDeallocated ? taskUserSpaceAllocated - taskUserSpaceDeallocated : 0;
                        decimal sessionInternalSpaceAllocated = session.SessionInternalAllocatedTotal != null ? (session.SessionInternalAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionInternalSpaceDeallocated = session.SessionInternalDeallocatedTotal != null ? (session.SessionInternalDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal sessionInternalSpaceUsed = sessionInternalSpaceAllocated >= sessionInternalSpaceDeallocated ? sessionInternalSpaceAllocated - sessionInternalSpaceDeallocated : 0;
                        decimal taskInternalSpaceAllocated = session.TaskInternalAllocatedTotal != null ? (session.TaskInternalAllocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskInternalSpaceDeallocated = session.TaskInternalDeallocatedTotal != null ? (session.TaskInternalDeallocatedTotal.Kilobytes ?? 0) : 0;
                        decimal taskInternalSpaceUsed = taskInternalSpaceAllocated >= taskInternalSpaceDeallocated ? taskInternalSpaceAllocated - taskInternalSpaceDeallocated : 0;

                        // Only add sessions using tempdb
                        if (((session.VersionStoreElapsedTime.HasValue ? session.VersionStoreElapsedTime.Value.TotalSeconds : 0) > 0)
                            || (sessionSpaceUsed > 0)
                            || (taskSpaceUsed > 0)
                            || (sessionInternalSpaceUsed > 0)
                            || (taskInternalSpaceUsed > 0)
                            //Added the following two conditions to resolve rally issue DE4285 - Aditya Shukla (SQLdm 8.6)
                            || (sessionUserSpaceAllocated > 0)
                            || (sessionUserSpaceDeallocated > 0)
                            || (taskUserSpaceAllocated > 0)
                            || (taskUserSpaceDeallocated > 0)
                            || (sessionInternalSpaceAllocated > 0)
                            || (sessionInternalSpaceDeallocated > 0)
                            || (taskInternalSpaceAllocated > 0)
                            || (taskInternalSpaceDeallocated > 0))
                        {

                            if (rowLookupTable.TryGetValue(session.InternalSessionIdentifier, out existingRow))
                            {
                                existingRow["Type"] = session.IsSystemProcess ? "System" : "User";
                                existingRow["User"] = session.UserName;
                                existingRow["Database"] = session.Database;
                                existingRow["Status"] = session.Status;
                                existingRow["Open Transactions"] = session.OpenTransactions;
                                existingRow["Application"] = session.Application;
                                existingRow["Execution Context"] = session.ExecutionContext;
                                existingRow["Wait Time"] = session.WaitTime.TotalMilliseconds;
                                existingRow["Wait Type"] = session.WaitType;
                                existingRow["Wait Resource"] = session.WaitResource;
                                existingRow["CPU"] = session.Cpu.TotalMilliseconds;
                                existingRow["Physical I/O"] = session.PhysicalIo;
                                existingRow["Memory Usage"] = session.Memory.Kilobytes;
                                existingRow["Last Batch"] = session.LastActivity.HasValue
                                                                ? (object)
                                                                  ((DateTime) session.LastActivity).ToLocalTime()
                                                                : null;
                                existingRow["Blocked By"] = session.BlockedBy;
                                existingRow["Blocking"] = session.Blocking;
                                existingRow["Blocking Count"] = session.BlockingCount;
                                existingRow["Version Store Elapsed Seconds"] = session.VersionStoreElapsedTime.HasValue
                                                                                   ? session.VersionStoreElapsedTime.Value.TotalSeconds
                                                                                   : 0;
                                existingRow["Transaction Isolation Level"] =
                                    ApplicationHelper.GetEnumDescription(session.TransactionIsolationLevel);

                                existingRow["Current Session User Space (KB)"] = sessionSpaceUsed;
                                existingRow["Session User Space Allocated (KB)"] = sessionUserSpaceAllocated;
                                existingRow["Session User Space Deallocated (KB)"] = sessionUserSpaceDeallocated;

                                existingRow["Current Task User Space (KB)"] = taskSpaceUsed;
                                existingRow["Task User Space Allocated (KB)"] = taskUserSpaceAllocated;
                                existingRow["Task User Space Deallocated (KB)"] = taskUserSpaceDeallocated;

                                existingRow["Current Session Internal Space (KB)"] = sessionInternalSpaceUsed;
                                existingRow["Session Internal Space Allocated (KB)"] = sessionInternalSpaceAllocated;
                                existingRow["Session Internal Space Deallocated (KB)"] = sessionInternalSpaceDeallocated;

                                existingRow["Current Task Internal Space (KB)"] = taskInternalSpaceUsed;
                                existingRow["Task Internal Space Allocated (KB)"] = taskInternalSpaceAllocated;
                                existingRow["Task Internal Space Deallocated (KB)"] = taskInternalSpaceDeallocated;

                                existingRow["Last Command"] = session.LastCommand ?? "";
                            }
                            else
                            {
                                var newRow = tempdbSessionsGridDataSource.Rows.Add(
                                    new object[]
                                        {
                                            session.Spid,
                                            session.IsSystemProcess ? "System" : "User",
                                            session.UserName,
                                            session.Workstation,
                                            session.Database,
                                            session.Status,
                                            session.OpenTransactions,
                                            session.Application,
                                            session.ExecutionContext,
                                            session.WaitTime.TotalMilliseconds,
                                            session.WaitType,
                                            session.WaitResource,
                                            session.Cpu.TotalMilliseconds,
                                            session.PhysicalIo,
                                            session.Memory.Kilobytes,
                                            session.LastActivity.HasValue
                                                ? (object) ((DateTime) session.LastActivity).ToLocalTime()
                                                : null,
                                            session.BlockedBy,
                                            session.Blocking,
                                            session.BlockingCount,
                                            session.VersionStoreElapsedTime.HasValue
                                                ? session.VersionStoreElapsedTime.Value.TotalSeconds
                                                : 0,
                                            ApplicationHelper.GetEnumDescription(session.TransactionIsolationLevel),
                                            sessionSpaceUsed,
                                            sessionUserSpaceAllocated,
                                            sessionUserSpaceDeallocated,
                                            taskSpaceUsed,
                                            taskUserSpaceAllocated,
                                            taskUserSpaceDeallocated,
                                            sessionInternalSpaceUsed,
                                            sessionInternalSpaceAllocated,
                                            sessionInternalSpaceDeallocated,
                                            taskInternalSpaceUsed,
                                            taskInternalSpaceAllocated,
                                            taskInternalSpaceDeallocated,
                                            session.LastCommand ?? ""
                                        });

                                rowLookupTable.Add(session.InternalSessionIdentifier, newRow);
                            }
                        }
                    }

                    foreach (UltraGridRow row in tempdbSessionsGrid.Rows.GetAllNonGroupByRows())
                    {
                        row.RefreshSortPosition();
                    }
                    tempdbSessionsGrid.DisplayLayout.RefreshFilters();
                    tempdbSessionsGrid.Visible = true;

                    // this must be done before referencing the counts in the grid for the status update
                    tempdbSessionsGridDataSource.ResumeBindingNotifications();

                    ApplicationController.Default.SetCustomStatus(
                        sessionsConfiguration.IsFiltered() ? "Filter Applied" : string.Empty,
                        String.Format("{0} Session{1}",
                                tempdbSessionsGrid.Rows.FilteredInNonGroupByRowCount,
                                tempdbSessionsGrid.Rows.FilteredInNonGroupByRowCount == 1 ? string.Empty : "s")
                        );
                }
                else
                {
                    tempdbSessionsGridStatusLabel.Text = NO_ITEMS;
                    tempdbSessionsGrid.Visible = false;
                    ApplicationController.Default.SetCustomStatus(
                        sessionsConfiguration.IsFiltered() ? "Filter Applied" : string.Empty,
                        "0 Sessions"
                        );
                }

                tempdbSessionsGridDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, tempdbSessionsGrid);

                        initialized = true;
                    }
                    else if (snapshot.SessionList.Count > 0)
                    {
                        foreach (UltraGridColumn column in tempdbSessionsGrid.DisplayLayout.Bands[0].Columns)
                        {
                            if (column.Key != "Status Image")
                            {
                                bool hidden = column.Hidden;
                                column.Hidden = false;
                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                column.Hidden = hidden;
                            }
                        }

                        initialized = true;
                    }

                    if (FilterChanged != null)
                    {
                        FilterChanged(this, new EventArgs());
                    }
                }

                currentSessionSnapshot = snapshot;
                //UpdateDetailsPanel(selectedSpid);

                //make sure the selection is still correct
                if (tempdbSessionsGrid.Visible == false || tempdbSessionsGrid.Rows == null || tempdbSessionsGrid.Rows.Count == 0)
                {
                    selectedSpid = null;
                }
                else
                {
                    // this probably should never happen, but fix it just in case
                    if (tempdbSessionsGrid.Selected.Rows.Count == 1 && tempdbSessionsGrid.Selected.Rows[0].IsDataRow)
                    {
                        int spid = (int)tempdbSessionsGrid.Selected.Rows[0].Cells["Session ID"].Value;
                        if (selectedSpid != spid)
                        {
                            selectedSpidIsSystem = ((string)tempdbSessionsGrid.Selected.Rows[0].Cells["Type"].Value == "System");
                            selectedSpid = spid;
                        }
                    }
                    else
                    {
                        selectedSpid = null;
                    }
                }

                foreach (UltraGridRow row in tempdbSessionsGrid.Rows.GetAllNonGroupByRows())
                {
                    // fix the colors for critical value fields
                    if (row.IsDataRow)
                    {
                        UltraDataRow dataRow = row.ListObject as UltraDataRow;
                        updateCellAppearance(row.Cells["Blocked By"], dataRow["Blocked By"] != DBNull.Value);
                        updateCellAppearance(row.Cells["Blocking"], (bool)dataRow["Blocking"] == true);
                        updateCellAppearance(row.Cells["Blocking Count"], (bool)dataRow["Blocking"] == true);
                    }

                    // Check if the selection needs to be applied on the first time through
                    if (tempdbSessionsGrid.Rows.Count > 0 &&
                        tempdbSessionsGrid.Selected.Rows.Count == 0 &&
                        selectedSpidArgument != null &&
                        (int)row.Cells["Session ID"].Value == selectedSpidArgument)
                    {
                        row.Selected = true;
                        tempdbSessionsGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        selectedSpidIsSystem = (bool) ((string) row.Cells["Type"].Value == "System");
                        selectedSpid = selectedSpidArgument;
                        selectedSpidArgument = null;
                    }
                }

                if (selectedSpidArgument != null)
                {
                    string msg = string.Format("Session ID {0} has either ended or been excluded by a filter and was unable to be selected because it is not in the current list.", selectedSpidArgument);
                    // the Argument must be cleared before the message is shown or it might be shown several times
                    selectedSpidArgument = null;
                    ApplicationMessageBox.ShowWarning(this, msg);
                }
            }
        }

        private void UpdateTempdbFileDataFromHistory(DataTable dt)
        {
            _histFileSpaceUsedDataTable.BeginLoadData();
            _histFileSpaceUsedDataTable.Clear();
            object value;
            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = _histFileSpaceUsedDataTable.NewRow();

                value = dr["UTCCollectionDateTime"];
                if (value != DBNull.Value )
                {
                    newRow["Date"] = (DateTime) value;
                }

                value = dr["FileName"];
                if (value != DBNull.Value)
                {
                    newRow["Logical Filename"] = (string)value;
                }

                value = dr["FilePath"];
                if (value != DBNull.Value)
                {
                    newRow["File Path"] = (string)value;
                }

                value = dr["UserObjectsInMB"];
                if (value != DBNull.Value)
                {
                    newRow["User Objects"] = Convert.ToDouble(value);
                }

                value = dr["InternalObjectsInMB"];
                if (value != DBNull.Value)
                {
                    newRow["Internal Objects"] = Convert.ToDouble(value);
                }

                value = dr["VersionStoreInMB"];
                if (value != DBNull.Value)
                {
                    newRow["Version Store"] = Convert.ToDouble(value);
                }

                value = dr["MixedExtentsInMB"];
                if (value != DBNull.Value)
                {
                    newRow["Mixed Extents"] = Convert.ToDouble(value);
                }

                value = dr["UnallocatedSpaceInMB"];
                if (value != DBNull.Value)
                {
                    newRow["Unallocated Space"] = Convert.ToDouble(value);
                }

                _histFileSpaceUsedDataTable.Rows.Add(newRow);
            }

            _histFileSpaceUsedDataTable.EndLoadData();
            
            ConfigureFileSpaceUsedChart();
            ConfigureContentionChart(contentionHeaderButton.Text);
            ConfigureTotalSpaceUsedChart();

            fileSpaceUsedChart.Visible = true;
            totalSpaceUsedChart.Visible = true;
            contentionChart.Visible = true;
        }

        private void UpdateTempdbData(DatabaseFilesSnapshot snapshot)
        {

            fileSpaceUsedDataTable.BeginLoadData();
            statsDataTable.BeginLoadData();

            // remove any files that have been deleted
            var deleteRows = fileSpaceUsedDataTable.Rows
                                                   .Cast<DataRow>()
                                                   .Where(row => !snapshot.DatabaseFiles.ContainsKey((string) row["Logical Filename"]))
                                                   .ToList();
            foreach (var row in deleteRows)
            {
                fileSpaceUsedDataTable.Rows.Remove(row);
            }

            var totalUserObjects = new FileSize();
            var totalInternalObjects = new FileSize();
            var totalVersionStore = new FileSize();
            var totalMixedExtents = new FileSize();
            var totalUnallocatedSpace = new FileSize();
            decimal? userObjectsMB = null;
            decimal? internalObjectsMB= null;
            decimal? versionStoreMB=null;
            decimal? mixedExtentsMB = null;
            decimal? totalUnallocatedMB = null;

            //now update any matching databases or add new ones
            foreach (var file in snapshot.DatabaseFiles.Values)
            {
                if (file.TempdbFileActivity == null)
                {
                    continue;
                }

                if (file.TempdbFileActivity.UserObjects != null)
                {
                    if (file.TempdbFileActivity.UserObjects.Kilobytes.HasValue)
                    {
                        totalUserObjects.Kilobytes = (totalUserObjects.Kilobytes ?? 0) + file.TempdbFileActivity.UserObjects.Kilobytes;
                        userObjectsMB = file.TempdbFileActivity.UserObjects.Megabytes;
                    }
                }

                if (file.TempdbFileActivity.InternalObjects != null)
                {
                    if (file.TempdbFileActivity.InternalObjects.Kilobytes.HasValue)
                    {
                        totalInternalObjects.Kilobytes = (totalInternalObjects.Kilobytes ?? 0) + file.TempdbFileActivity.InternalObjects.Kilobytes;
                        internalObjectsMB = file.TempdbFileActivity.InternalObjects.Megabytes;
                    }
                }

                if (file.TempdbFileActivity.VersionStore != null)
                {
                    if (file.TempdbFileActivity.VersionStore.Kilobytes.HasValue)
                    {
                        totalVersionStore.Kilobytes = (totalVersionStore.Kilobytes ?? 0) + file.TempdbFileActivity.VersionStore.Kilobytes;
                        versionStoreMB = file.TempdbFileActivity.VersionStore.Megabytes;
                    }
                }

                if (file.TempdbFileActivity.MixedExtents != null)
                {
                    if (file.TempdbFileActivity.MixedExtents.Kilobytes.HasValue)
                    {
                        totalMixedExtents.Kilobytes = (totalMixedExtents.Kilobytes ?? 0) + file.TempdbFileActivity.MixedExtents.Kilobytes;
                        mixedExtentsMB = file.TempdbFileActivity.MixedExtents.Megabytes;
                    }
                }

                if (file.TempdbFileActivity.UnallocatedSpace != null)
                {
                    if (file.TempdbFileActivity.UnallocatedSpace.Kilobytes.HasValue)
                    {
                        totalUnallocatedSpace.Kilobytes = (totalUnallocatedSpace.Kilobytes ?? 0) + file.TempdbFileActivity.UnallocatedSpace.Kilobytes;
                        totalUnallocatedMB = file.TempdbFileActivity.UnallocatedSpace.Megabytes;
                    }
                }

                var rowData = new object[]
                                {
                                    DateTime.Now,
                                    file.TempdbFileActivity.Filename,
                                    file.TempdbFileActivity.Filepath,
                                    userObjectsMB.HasValue?userObjectsMB:0,
                                    internalObjectsMB.HasValue?internalObjectsMB:0,
                                    versionStoreMB.HasValue?versionStoreMB:0,
                                    mixedExtentsMB.HasValue?mixedExtentsMB:0,
                                    totalUnallocatedMB.HasValue?totalUnallocatedMB:0
                                };

                fileSpaceUsedDataTable.LoadDataRow(rowData, true);

            }

            var totalUsedSpace = new FileSize();
            totalUsedSpace.Kilobytes = (totalUserObjects.Kilobytes ?? 0)
                                       + (totalInternalObjects.Kilobytes ?? 0)
                                       + (totalVersionStore.Kilobytes ?? 0)
                                       + (totalMixedExtents.Kilobytes ?? 0);

            var summaryRowData = new object[]
                                    {
                                        DateTime.Now,
                                        totalUserObjects.Megabytes,
                                        totalInternalObjects.Megabytes,
                                        totalVersionStore.Megabytes,
                                        totalMixedExtents.Megabytes,
                                        totalUnallocatedSpace.Megabytes,
                                        totalUsedSpace.Megabytes,
                                        //sqldm-30299 start
                                       snapshot.TempdbStatistics!=null? snapshot.TempdbStatistics.TempdbPFSWaitTime.TotalMilliseconds:(object)null,
                                       snapshot.TempdbStatistics!=null?  snapshot.TempdbStatistics.TempdbGAMWaitTime.TotalMilliseconds:(object)null,
                                        snapshot.TempdbStatistics!=null? snapshot.TempdbStatistics.TempdbSGAMWaitTime.TotalMilliseconds:(object)null,
                                        snapshot.TempdbStatistics!=null? snapshot.TempdbStatistics.VersionStoreGenerationKilobytes : (object)null,
                                        snapshot.TempdbStatistics!=null? snapshot.TempdbStatistics.VersionStoreCleanupKilobytes:(object)null
                                        //sqldm-30299 end
                                    };
            

            statsDataTable.LoadDataRow(summaryRowData, true);
            
            fileSpaceUsedDataTable.EndLoadData();
            statsDataTable.EndLoadData();
            fileSpaceUsedChart.Visible = true;
            totalSpaceUsedChart.Visible = true;
            contentionChart.Visible = true;
            currentFileSnapshot = snapshot;

        }

        /// <summary>
        /// Get the major version from the snapshot.
        /// </summary>
        /// <param name="currentSnapshot"></param>
        /// <returns></returns>
        private int GetServerMajorVersion(Snapshot currentSnapshot)
        {
            // If there is a snapshot
            if (currentSnapshot != null)
            {
                //and a product version in the snapshot
                if (currentSnapshot.ProductVersion != null)
                {
                    //use the products product version
                    return currentSnapshot.ProductVersion.Major;
                }
            }

            //if we got here then we need to make a plan to get the product version from elsewhere
            Idera.SQLdm.DesktopClient.Objects.MonitoredSqlServerStatus server =
                ApplicationModel.Default.GetInstanceStatus(instanceId);
            if (server != null && server.InstanceVersion != null)
            {
                return server.InstanceVersion.Major;
            }

            // default to assuming it is 2005
            return 9;
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
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
                case "viewLocksButton":
                    ShowLocksView();
                    break;
                case "viewQueryHistoryButton":
                    ShowQueryHistoryView();
                    break;
                case "traceSessionButton":
                    TraceSession();
                    break;
                case "killSessionButton":
                    KillSession();
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
                case "viewSessionsButton":
                    ShowSessionsView();
                    break;
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                    break;
                case "printChartButton":
                    PrintChart(contextMenuSelectedChart);
                    break;
                case "exportChartDataButton":
                    SaveChartData(contextMenuSelectedChart);
                    break;
                case "exportChartImageButton":
                    SaveChartImage(contextMenuSelectedChart);
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
                case "gridDataContextMenu":
                    if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify &&
                        HistoricalSnapshotDateTime == null)
                    {
                        ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.True;

                        bool showKill = false;
                        if (tempdbSessionsGrid.Rows.Count > 0
                            && tempdbSessionsGrid.Selected.Rows.Count == 1
                            && tempdbSessionsGrid.Selected.Rows[0].IsDataRow)
                        {
                            if (tempdbSessionsGrid.Selected.Rows[0].Cells["Type"].Value.ToString() == "User")
                            {
                                showKill = true;
                            }
                        }
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].SharedProps.Enabled = showKill;
                    }
                    else
                    {
                        ((PopupMenuTool)e.Tool).Tools["traceSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                        ((PopupMenuTool)e.Tool).Tools["killSessionButton"].InstanceProps.Visible = DefaultableBoolean.False;
                    }
                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = tempdbSessionsGrid.Rows.Count > 0 && tempdbSessionsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(tempdbSessionsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #region Grid

        private void PrintGrid()
        {
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - sessions using tempdb as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "TempdbSessionDetails";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //// Fix the icon columns to export as text values with a heading
                //tempdbSessionsGrid.DisplayLayout.Bands[0].Columns["Status Image"].Header.Caption = "Status";
                //tempdbSessionsGrid.DisplayLayout.ValueLists["statusValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;
                //tempdbSessionsGrid.DisplayLayout.ValueLists["TypeValueList"].DisplayStyle = ValueListDisplayStyle.DisplayText;

                try
                {
                    ultraGridExcelExporter.Export(tempdbSessionsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                //tempdbSessionsGrid.DisplayLayout.Bands[0].Columns["Status Image"].Header.Caption = string.Empty;
                //tempdbSessionsGrid.DisplayLayout.ValueLists["statusValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
                //tempdbSessionsGrid.DisplayLayout.ValueLists["TypeValueList"].DisplayStyle = ValueListDisplayStyle.Picture;
            }
        }

        private void updateCellAppearance(UltraGridCell cell, bool hilight)
        {
            if (hilight)
            {
                cell.Appearance.BackColor = Color.Red;
                cell.Appearance.ForeColor = Color.White;
                cell.Appearance.FontData.Bold = DefaultableBoolean.True;
            }
            else
            {
                cell.Appearance.BackColor = Color.White;
                cell.Appearance.ForeColor = Color.Black;
                cell.Appearance.FontData.Bold = DefaultableBoolean.False;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    tempdbSessionsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            tempdbSessionsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            tempdbSessionsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(tempdbSessionsGrid);
            dialog.Show(this);
        }

        private void ShowLocksView()
        {
            if (selectedSpid.HasValue)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsLocks, selectedSpid.Value);
            }
        }

        private void ShowSessionsView()
        {
            if (selectedSpid.HasValue)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsDetails, selectedSpid.Value);
            }
        }

        private void ShowQueryHistoryView()
        {
            if (selectedSpid.HasValue)
            {
                if (selectedSpidCommand.Length > 0)
                {
                    string SqlHash = SqlParsingHelper.GetSignatureHash(selectedSpidCommand);
                    ApplicationController.Default.ShowServerView(instanceId, ServerViews.QueriesHistory, SqlHash);
                }
            }
        }

          public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        public void TraceSession()
        {
            if (selectedSpid.HasValue)
            {
                Dialogs.SessionTraceDialog dialog = new SessionTraceDialog(instanceId, selectedSpid.Value);
                dialog.Show();
            }
        }

        public void KillSession()
        {
            if (selectedSpid.HasValue)
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                                        String.Format("You are about to kill Session ID {0}. Do you want to continue?",
                                                        selectedSpid.Value.ToString()),
                                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    KillSessionConfiguration config = new KillSessionConfiguration(instanceId, selectedSpid.Value);
                    Snapshot snapshot = managementService.SendKillSession(config);

                    if (snapshot.Error == null)
                    {
                        ApplicationMessageBox.ShowMessage("The Session has been terminated.");
                        ApplicationController.Default.ActiveView.CancelRefresh();
                        ApplicationController.Default.RefreshActiveView();
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, String.Format("Unable to Kill Session ID {0}.",
                                                                            selectedSpid.Value.ToString()),
                                                        snapshot.Error);
                    }
                }
            }
        }

        #endregion

        #region Chart

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

        #endregion

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
            //SQLdm 10.1 (Barkha khatri) -- SQLDM-25894 fix
            //if (!isUserSysAdmin)
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE);
            //else
            //    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING);

            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm, HISTORY_ERROR, historyModeLoadError, false);
            }
        }

        #endregion

        private void tempdbContentionButton_Click(object sender, EventArgs e)
        {
            ConfigureContentionChart(((ToolStripDropDownItem)sender).Text);
        }

        private void versionStoreCleanupRateButton_Click(object sender, EventArgs e)
        {
            ConfigureContentionChart(((ToolStripDropDownItem)sender).Text);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
            }
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            splitContainer.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            DatabasesTempdbView_Fill_Panel.Controls.Add(chartPanel);
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            DatabasesTempdbView_Fill_Panel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            splitContainer.Visible = true;
        }

        private void maximizeFileSpaceUsedChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(fileSpaceUsedPanel, maximizeFileSpaceUsedChartButton, restoreFileSpaceUsedChartButton);
        }

        private void restoreFileSpaceUsedChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(fileSpaceUsedPanel, maximizeFileSpaceUsedChartButton, restoreFileSpaceUsedChartButton, 0, 0);
        }

        private void maximizeTotalSpaceUsedChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(totalSpaceUsedPanel, maximizeTotalSpaceUsedChartButton, restoreTotalSpaceUsedChartButton);
        }

        private void restoreTotalSpaceUsedChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(totalSpaceUsedPanel, maximizeTotalSpaceUsedChartButton, restoreTotalSpaceUsedChartButton, 1, 0);
        }

        private void maximizeContentionChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(contentionPanel, maximizeContentionChartButton, restoreContentionChartButton);
        }

        private void restoreContentionChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(contentionPanel, maximizeContentionChartButton, restoreContentionChartButton, 2, 0);
        }

        private void DatabasesTempdbView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void UpdateChartDataFilter()
        {
            if (statsDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                statsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                DatabasesTempdbView_Fill_Panel.Visible = false;
            }

            tempdbSessionsGridStatusLabel.Text = UNABLE_TO_UPDATE;
            base.HandleBackgroundWorkerError(e);
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.tempdbSessionsGrid);
        }


        /// <summary>
        /// SQLdm 10.1 (Barkha khatri) -- SQLDM-25894 fix
        /// common function to change status labels of charts
        /// </summary>
        /// <param name="message"></param>
        private void ShowChartStatusMessage(string message)
        {
            totalSpaceUsedStatusLabel.Text= spaceUsedStatusLabel.Text= contentionStatusLabel.Text= message;
        }
    }
}
