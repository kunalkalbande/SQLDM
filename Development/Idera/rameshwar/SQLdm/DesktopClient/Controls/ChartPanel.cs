using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class ChartPanel : UserControl
    {
        #region Private variables

        private ChartFormat chartFormat = ChartFormat.SingleChart;

        private ChartInfo chartInfo;
        private List<ChartType> typesList = new List<ChartType>(new ChartType[] { ChartType.Unknown });
        private List<ChartView> viewsList = new List<ChartView>(new ChartView[] { ChartView.Unknown });

        // maximize and restore areas
        private Control maximizeToControl = null;
        private Control parentControl = null;
        private int parentRow = 0;
        private int parentCol = 0;

        private bool axisXTooltipShowsPointLabel = false;
        private HitTestEventArgs lastMouseHitTest;
        private bool showFileActivityMenuOption = false;

        #endregion

        #region Properties

        public event EventHandler<ChartClickEventArgs> ChartClicked;
        public event EventHandler<ChartSelectionEventArgs> ChartSelectionChanged;

        public bool AxisXTooltipShowsPointLabel
        {
            get { return axisXTooltipShowsPointLabel; }
            set { axisXTooltipShowsPointLabel = value; }
        }

        public bool ByDisk
        {
            get { return chartInfo.ByDisk; }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : ChartFX chart refrence for attaching/detaching drilldown functionality
        /// </summary>
        public Chart Chart
        {
            get { return chart; }
        }

        public ChartFormat ChartFormat
        {
            get { return chartFormat; }
        }

        public ChartType ChartType
        {
            get { return chartInfo.ChartType; }
        }

        public ChartView ChartView
        {
            get { return chartInfo.ChartView; }
        }

        public int ChartDisplayValues
        {
            get { return chartInfo.ChartDisplayValues; }
        }

        public bool ChartVisible
        {
            get { return chart.Visible; }
            set { chart.Visible = value; }
        }

        public bool FilterColumns
        {
            get { return chartInfo.FilterColumns; }
        }

        public Control MaximizeToControl
        {
            get { return maximizeToControl; }
            set
            {
                maximizeToControl = value;
                if (value != null)
                {
                    maximizeChartButton.Visible = true;
                }
            }
        }

        public string StatusText
        {
            get { return statusLabel.Text; }
            set { statusLabel.Text = value; }
        }

        public string Title
        {
            get 
            {
                return chartInfo.ChartTitle;
            }
        }

        public bool ShowFileActivityMenuOption
        {
            get { return showFileActivityMenuOption; }
            set { showFileActivityMenuOption = value; }
        }

        public ContextMenuManager ToolbarManager
        {
            get { return toolbarsManager; }
        }

        #endregion

        #region Constructors

        public ChartPanel()
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(chart, toolbarsManager);
            chartInfo = new ChartInfo();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Initialize a chart for the selected type of data with no selection lists
        /// </summary>
        /// <param name="type">The ChartType that represents the type of chart data to display</param>
        public void InitializeChart (ChartType type)
        {
            // These aren't used in this mode now, but keep them consistent just in case
            typesList = new List<ChartType>(new ChartType[] { type });
            viewsList = new List<ChartView>(new ChartView[] { ChartView.Standard });

            chartInfo = ChartHelper.InitializeChartLayout(chart, type);
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));

            selectViewDropDownButton.Visible =
                chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = false;
        }

        /// <summary>
        /// Initialize a chart for the selected type of data with no selection lists
        /// </summary>
        /// <param name="type">The ChartType that represents the type of chart data to display</param>
        /// <param name="showItems">The number of items the chart should initially show</param>
        /// <param name="minItems">The minimum number of items the user can select for the chart can show</param>
        /// <param name="maxItems">The maximum number of items the user can select for the chart can show</param>
        public void InitializeChart(ChartType type, int showItems, int minItems, int maxItems)
        {
            // These aren't used in this mode now, but keep them consistent just in case
            typesList = new List<ChartType>(new ChartType[] { type });
            viewsList = new List<ChartView>(new ChartView[] { ChartView.Standard });

            chartInfo = ChartHelper.InitializeChartLayout(chart, type);
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));

            chartValuesNumericUpDown.NumericUpDownControl.Value = showItems >= minItems && showItems <= maxItems ? showItems : minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Minimum = minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Maximum = maxItems;

            selectViewDropDownButton.Visible = false;
            chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = (maxItems > 0);
        }

        /// <summary>
        /// Initialize the chart with selections for the user and for the selected initial type of data
        /// </summary>
        /// <param name="typesAllowed">The list of ChartTypes that the user can select from</param>
        /// <param name="selectedType">The ChartType that represents the initial selected type of chart data to display from the typesAllowed list. If not in list, the first item in the list will be used.</param>
        public void InitializeChart(List<ChartType> typesAllowed, ChartType selectedType, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
        {
            typesList = typesAllowed;
            // This isn't used in this mode now, but keep them consistent just in case
            viewsList = new List<ChartView>(new ChartView[] { ChartView.Standard });

            loadTypes(typesAllowed);

            chartInfo = ChartHelper.InitializeChartLayout(chart, getValidChartType(selectedType), isHyperVInstance);//SQLdm 10.0 (Tarun Sapra)- Display Vm and Host Baselines, if the instance is of hyperV type
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));

            selectViewDropDownButton.Visible =
                chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = false;
        }

        /// <summary>
        /// Initialize the chart with selections for the user and for the selected initial type of data
        /// </summary>
        /// <param name="typesAllowed">The list of ChartTypes that the user can select from</param>
        /// <param name="selectedType">The ChartType that represents the initial selected type of chart data to display from the typesAllowed list. If not in list, the first item in the list will be used.</param>
        /// <param name="showItems">The number of items the chart should initially show</param>
        /// <param name="minItems">The minimum number of items the user can select for the chart can show</param>
        /// <param name="maxItems">The maximum number of items the user can select for the chart can show</param>
        public void InitializeChart(List<ChartType> typesAllowed, ChartType selectedType, int showItems, int minItems, int maxItems)
        {
            typesList = typesAllowed;
            // This isn't used in this mode now, but keep them consistent just in case
            viewsList = new List<ChartView>(new ChartView[] { ChartView.Standard });

            loadTypes(typesAllowed);

            chartInfo = ChartHelper.InitializeChartLayout(chart, getValidChartType(selectedType));
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));

            chartValuesNumericUpDown.NumericUpDownControl.Value = showItems >= minItems && showItems <= maxItems ? showItems : minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Minimum = minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Maximum = maxItems;

            selectViewDropDownButton.Visible = false;
            chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = (maxItems > 0);
        }

        /// <summary>
        /// Initialize the chart for the selected type and view of data
        /// </summary>
        /// <param name="typesAllowed">The list of ChartTypes that the user can select from</param>
        /// <param name="selectedType">The ChartType that represents the initial selected type of chart data to display from the typesAllowed list. If not in list, the first item in the list will be used.</param>
        /// <param name="viewsAllowed">The list of ChartViews that the user can select from to choose a view of the selected type of data</param>
        /// <param name="selectedView">The ChartViews that represents the initial way to view the selected type of chart data to display from the viewsAllowed list. If not in list, the first item in the list will be used.</param>
        public void InitializeChart(List<ChartType> typesAllowed, ChartType selectedType, List<ChartView> viewsAllowed, ChartView selectedView)
        {
            typesList = typesAllowed;
            viewsList = viewsAllowed;

            loadTypes(typesAllowed);
            loadViews(viewsAllowed);

            chartInfo = ChartHelper.InitializeChartLayout(chart, getValidChartType(selectedType), getValidChartView(selectedView));
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));
            setViewDisplay(ChartHelper.ChartViewName(chartInfo.ChartView));

            chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = false;
        }

        /// <summary>
        /// Initialize the chart for the selected type and view of the top nn data items
        /// </summary>
        /// <param name="typesAllowed">The list of ChartTypes that the user can select from</param>
        /// <param name="selectedType">The ChartType that represents the initial selected type of chart data to display from the typesAllowed list. If not in list, the first item in the list will be used.</param>
        /// <param name="viewsAllowed">The list of ChartViews that the user can select from to choose a view of the selected type of data</param>
        /// <param name="selectedView">The ChartViews that represents the initial way to view the selected type of chart data to display from the viewsAllowed list. If not in list, the first item in the list will be used.</param>
        /// <param name="showItems">The number of items the chart should initially show</param>
        /// <param name="minItems">The minimum number of items the user can select for the chart can show</param>
        /// <param name="maxItems">The maximum number of items the user can select for the chart can show</param>
        public void InitializeChart(List<ChartType> typesAllowed, ChartType selectedType, List<ChartView> viewsAllowed, ChartView selectedView, int showItems, int minItems, int maxItems)
        {
            typesList = typesAllowed;
            viewsList = viewsAllowed;

            loadTypes(typesAllowed);
            loadViews(viewsAllowed);

            chartInfo = ChartHelper.InitializeChartLayout(chart, getValidChartType(selectedType), getValidChartView(selectedView), showItems, minItems, maxItems);
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));
            setViewDisplay(ChartHelper.ChartViewName(chartInfo.ChartView));

            chartValuesNumericUpDown.NumericUpDownControl.Value = showItems >= minItems && showItems <= maxItems ? showItems : minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Minimum = minItems;
            chartValuesNumericUpDown.NumericUpDownControl.Maximum = maxItems;

            chartValuesLabel.Visible =
                chartValuesNumericUpDown.Visible = (maxItems > 0);
        }

		//SQLdm 10.0 (Tarun Sapra): DE46062-Adding param to display graph in case of hyperV instance
        public void CheckChartConfigured(bool isHyperVInstance= false)
        {
            if (!chartInfo.InitChartDataColumns)
            {
                ChartHelper.ConfigureChart(chart, chartInfo, isHyperVInstance);
            }
        }
		///Adding method to configure chart - Ankit  Nagpal SQLdm 10.0
        public void ConfiureChart() {
            ChartHelper.ConfigureChart(chart, chartInfo);
        }

		//SQLdm 10.0 (Tarun Sapra): DE46062-Adding param to display graph in case of hyperV instance
        public void ForceChartReconfigure(bool isHyperInstance = false)
        {
            if (chartInfo.DataObject != null && chartInfo.DataObject is ServerSummaryHistoryData)
                ChartHelper.ConfigureChart(chart, chartInfo, isHyperInstance);
        }

        public bool InitChartDataColumns
        {
            get { return chartInfo.InitChartDataColumns; }
        }

        public void ForceChartColors()
        {
            chart.SetAreaSeriesAlphaChannel(175, 0);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Allow chartpanel to support multiple datasources depending upon charttype
        /// Resources > Disk charts need to support multiple data sources.
        /// Add the datasource for the chart
        /// </summary>
        /// <param name="dataSource">First datasource for the chartpanel</param>
        /// <param name="dataSource2">Second datasource for the chartpanel</param>
        public void SetDataSource(DataTable dataSource, DataTable dataSource2)
        {
           
            if (chartInfo.ChartDataType == ChartDataType.DataSource)
            {
                chart.DataSource = ChartHelper.UseFirstDataSource(chartInfo.ChartType) ? dataSource : dataSource2;
            }
            chartInfo.DataSource = dataSource;
            chartInfo.DataSource2 = dataSource2;
        }

        /// <summary>
        /// Associate the data needed to display a chart
        /// </summary>
        /// <param name="dataObject">A data object needed for chart processing</param>
        /// <param name="dataSource">The datasource for the chart</param>
        public void SetDataSource(object dataObject, DataTable dataSource, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra): DE46062-Adding param to display graph in case of hyperV instance
        {
            // SqlDM 10.2 (Anshul Aggarwal) - Set datasources for chart panel.
            SetDataSources(dataObject, dataSource, null, isHyperVInstance);
        }

        /// <summary>
        /// Associate the data needed to display a chart
        /// </summary>
        /// <param name="dataObject">A data object needed for chart processing</param>
        /// <param name="dataSource">The datasource for the chart</param>
        public void SetDataSource(object dataObject, DataTable dataSource, DataTable dataSource2, bool isHyperVInstance = false)//SQLdm 10.0 (Tarun Sapra): DE46062-Adding param to display graph in case of hyperV instance
        {
            // SqlDM 10.2 (Anshul Aggarwal) - Set datasources for chart panel.
            SetDataSources(dataObject, dataSource, dataSource2, isHyperVInstance);
        }

        #endregion

        #region Helpers

        private void loadTypes(List<ChartType> types)
        {
            if (types.Count > 1)
            {
                if (chartFormat == ChartFormat.SingleChart)
                {
                    chartFormat = ChartFormat.MultipleCharts;
                }
                else if (chartFormat == ChartFormat.SingleChartWithCount)
                {
                    chartFormat = ChartFormat.MultipleChartsWithCount;
                }
                selectTypeDropDownButton.DropDownItems.Clear();
                foreach (ChartType type in types)
                {
                    ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(ChartHelper.ChartTypeName(type), null, chartTypeSelectToolStripMenuItem_Click);
                    chartTypeItem.Tag = type;
                    selectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
                }
                selectTypeDropDownButton.Visible = true;
                headerLabel.Visible = false;
            }
            else
            {
                if (chartFormat == ChartFormat.MultipleCharts)
                {
                    chartFormat = ChartFormat.SingleChart;
                }
                else if (chartFormat == ChartFormat.MultipleChartsWithCount)
                {
                    chartFormat = ChartFormat.SingleChartWithCount;
                }
                if (types.Count == 1)
                {
                    setTypeDisplay(ChartHelper.ChartTypeName(types[0]));
                }
                selectTypeDropDownButton.Visible = false;
                headerLabel.Visible = true;
            }
        }

        private void loadViews(List<ChartView> views)
        {
            ChartView lastView = chartInfo.ChartView;
            bool currentViewFound = false;

            if (views.Count == 0 ||
                    (views.Count == 1 && views[0] == ChartView.Standard))
            {
                //This chart is switching from multiple views to single view, so fix the layout
                selectViewDropDownButton.Visible = false;
                selectViewDropDownButton.DropDownItems.Clear();
                //Make sure the chart will still display the correct types and counts when removing the views
                switch (chartFormat)
                {
                    case ChartFormat.MultiViewCharts:
                        if (selectTypeDropDownButton.Visible)
                        {
                            chartFormat = ChartFormat.MultipleCharts;
                        }
                        else
                        {
                            chartFormat = ChartFormat.SingleChart;
                        }
                        break;
                    case ChartFormat.MultiViewChartsWithCount:
                        if (selectTypeDropDownButton.Visible)
                        {
                            chartFormat = ChartFormat.MultiViewChartsWithCount;
                        }
                        else
                        {
                            chartFormat = ChartFormat.SingleChartWithCount;
                        }
                        break;
                }

                if (lastView != ChartView.Standard)
                {
                    chartInfo = ChartHelper.ReinitializeChartLayout(chart, chartInfo, ChartView.Standard);
                    setViewDisplay(ChartHelper.ChartViewName(chartInfo.ChartView));
                    OnSelectionChanged();
                }
            }
            else
            {
                //Make sure the chart will display the view dropdown if there is a list
                switch (chartFormat)
                {
                    case ChartFormat.SingleChart:
                    case ChartFormat.MultipleCharts:
                        chartFormat = ChartFormat.MultiViewCharts;
                        break;
                    case ChartFormat.SingleChartWithCount:
                    case ChartFormat.MultipleChartsWithCount:
                        chartFormat = ChartFormat.MultiViewChartsWithCount;
                        break;
                }

                selectViewDropDownButton.DropDownItems.Clear();
                foreach (ChartView view in views)
                {
                    ToolStripMenuItem chartViewItem = new ToolStripMenuItem(ChartHelper.ChartViewName(view), null, chartViewSelectToolStripMenuItem_Click);
                    chartViewItem.Tag = view;
                    selectViewDropDownButton.DropDownItems.Add(chartViewItem);
                    if (lastView == view)
                    {
                        currentViewFound = true;
                    }
                }
                selectViewDropDownButton.Visible = true;

                if (!currentViewFound)
                {
                    chartInfo = ChartHelper.ReinitializeChartLayout(chart, chartInfo, views[0]);
                    setViewDisplay(ChartHelper.ChartViewName(chartInfo.ChartView));
                    OnSelectionChanged();
                }
            }
        }

        private ChartType getValidChartType(ChartType type)
        {
            if (typesList == null || typesList.Count == 0)
            {
                return ChartType.Unknown;
            }
            else if (!typesList.Contains(type))
            {
                return typesList[0];
            }

            return type;
        }

        private ChartView getValidChartView(ChartView view)
        {
            if (viewsList == null || viewsList.Count == 0)
            {
                return ChartView.Unknown;
            }
            else if (!viewsList.Contains(view))
            {
                return viewsList[0];
            }

            return view;
        }

        private void setTypeDisplay(string type)
        {
            if (chartFormat == ChartFormat.SingleChart)
            {
                headerLabel.Text = type;
            }
            else
            {
                selectTypeDropDownButton.Text = type;
            }
        }

        private void setViewDisplay(string view)
        {
            selectViewDropDownButton.Text = view;
        }

        /// <summary>
        /// Initialize the chart for the selected type of data display and view
        /// </summary>
        /// <param name="type">The ChartType that represents the type of chart data to display</param>
        /// <param name="view">The ChartView that represents the view of the chart data to display</param>
        private void reinitializeChart(ChartType type, ChartView view)
        {
            chartInfo = ChartHelper.ReinitializeChartLayout(chart, chartInfo, type, view);
            setTypeDisplay(ChartHelper.ChartTypeName(chartInfo.ChartType));
            setViewDisplay(ChartHelper.ChartViewName(chartInfo.ChartView));
            ForceChartColors();
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Allow chartpanel to support multiple datasources depending upon charttype
        /// Resources > Disk charts need to support multiple data sources.
        /// </summary>
        private void SetDataSources(object dataObject, DataTable dataSource, DataTable dataSource2, bool isHyperVInstance)
        {
            SetDataSource(dataSource, dataSource2);
            chartInfo.DataObject = dataObject;
            ChartHelper.ConfigureChart(chart, chartInfo, isHyperVInstance);
            ForceChartColors();
        }

        #region Charts

        private void ToggleChartToolbar(bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, Title, ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(Title, true));
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, Title, ExportHelper.GetValidFileName(Title, true));
        }

        private void SelectChartDataColumns()
        {
            if (FilterColumns)
            {
                ChartHelper.SelectChartDataColumns(ChartDataColumnType.Disk, chart, chartInfo);
            }
        }
        
        private void MaximizeChart()
        {
            if (maximizeToControl != null && Parent != null)
            {
                parentControl = Parent;
                if (parentControl is TableLayoutPanel)
                {
                    TableLayoutPanel tableLayoutPanel = Parent as TableLayoutPanel;
                    parentRow = tableLayoutPanel.GetRow(this);
                    parentCol = tableLayoutPanel.GetColumn(this);
                    tableLayoutPanel.Visible = false;
                    tableLayoutPanel.Controls.Remove(this);
                }
                else
                {
                    parentControl.Visible = false;
                    parentControl.Controls.Remove(this);
                }
                maximizeChartButton.Visible = false;
                restoreChartButton.Visible = true;
                maximizeToControl.Controls.Add(this);
                this.BringToFront();
            }
        }

        private void RestoreChart()
        {
            if (maximizeToControl != null && Parent != null)
            {
                restoreChartButton.Visible = false;
                maximizeChartButton.Visible = true;
                Parent.Controls.Remove(this);
                parentControl.Controls.Add(this);
                if (parentControl is TableLayoutPanel)
                {
                    TableLayoutPanel tableLayoutPanel = Parent as TableLayoutPanel;
                    tableLayoutPanel.SetCellPosition(this, new TableLayoutPanelCellPosition(parentCol, parentRow));
                }
                parentControl.Visible = true;
            }
        }

        #endregion

        #endregion

        #region Events

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item.Tag != null && item.Tag is ChartType)
                {
                    reinitializeChart((ChartType)item.Tag, chartInfo.ChartView);
                    OnSelectionChanged();
                }
            }
        }

        private void chartViewSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item.Tag != null && item.Tag is ChartView)
                {
                    reinitializeChart(chartInfo.ChartType, (ChartView)item.Tag);
                    OnSelectionChanged();
                }
            }
        }

        private void chartValuesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            chartInfo.ChartDisplayValues = (int)chartValuesNumericUpDown.NumericUpDownControl.Value;

            reinitializeChart(chartInfo.ChartType, chartInfo.ChartView);
            OnSelectionChanged();
        }

        private void maximizeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart();
        }

        private void restoreChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart();
        }

        #region toolbar

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(((StateButtonTool)e.Tool).Checked);
                    break;
                case "printChartButton":
                    PrintChart();
                    break;
                case "exportChartDataButton":
                    SaveChartData();
                    break;
                case "exportChartImageButton":
                    SaveChartImage();
                    break;
                case "selectDataColumnsButton":
                    SelectChartDataColumns();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            toolbarsManager.Tools["showFileActivity"].SharedProps.Visible = showFileActivityMenuOption;

            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);

                    ((ButtonTool)((PopupMenuTool)e.Tool).Tools["selectDataColumnsButton"]).SharedProps.Visible = FilterColumns;

                    break;
            }
        }

        #endregion

        private void chart_MouseClick(object sender, HitTestEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnChartClicked(e);
            }
        }

        private void OnChartClicked(HitTestEventArgs e)
        {
            if (ChartClicked != null)
            {
                ChartClicked(this, new ChartClickEventArgs(chart, chartInfo, e));
            }
        }

        private void OnSelectionChanged()
        {
            if (ChartSelectionChanged != null)
            {
                ChartSelectionChanged(this, new ChartSelectionEventArgs(ChartSettings.GetSettings(this)));
            }
        }

        private void chart_MouseMove(object sender, HitTestEventArgs e)
        {
            try
            {
                lastMouseHitTest = chart.HitTest(e.X, e.Y);

                if (ChartClicked != null)
                {
                    if (lastMouseHitTest.HitType == HitType.Point ||
                            (lastMouseHitTest.HitType == HitType.Axis
                                && lastMouseHitTest.Object == chart.AxisX))
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
            catch
            {
                lastMouseHitTest = null;
                chart.Cursor = Cursors.Default;
            }
        }

        private void chart_GetTip(object sender, GetTipEventArgs e)
        {
            if (AxisXTooltipShowsPointLabel
                    && lastMouseHitTest != null
                    && lastMouseHitTest.Object == chart.AxisX)
            {
                int closestPoint = (int)Math.Floor(lastMouseHitTest.Value);
                if (chart.Points[0, closestPoint] != null)
                {
                    e.Text = chart.Points[0, closestPoint].Text;
                }
            }
        }

        #endregion
    }

    public class ChartClickEventArgs : EventArgs
    {
        public ChartClickEventArgs(Chart chart, ChartInfo chartInfo, HitTestEventArgs hitTestEventArgs)
        {
            this.Chart = chart;
            this.ChartInfo = chartInfo;
            this.HitTestEventArgs = hitTestEventArgs;
        }

        public readonly Chart Chart;
        public readonly ChartInfo ChartInfo;
        public readonly HitTestEventArgs HitTestEventArgs;
    }

    public class ChartSelectionEventArgs : EventArgs
    {
        public ChartSelectionEventArgs(ChartSettings settings)
        {
            this.ChartSettings = settings;
        }

        public readonly ChartSettings ChartSettings;
    }
}
