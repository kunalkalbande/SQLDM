using System.Drawing;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries
{
    partial class QueryMonitorView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("showQueryPlanButton");
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("showQueryPlanButton");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Controls.CustomControls.CustomButtonTool("showQueryPlanButton");
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContexMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("queryHistoryButton");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryMonitorView));
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("showQueryTextButton");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("showQueryTextButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("queryHistoryButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool5 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("keepDetailedHistoryButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool6 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("keepDetailedHistoryButton", "");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCStartTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UTCCompletionTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DurationMilliseconds");
            Infragistics.Win.Appearance appearance72 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AvgCPUMilliseconds");
            Infragistics.Win.Appearance appearance73 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AvgReads");
            Infragistics.Win.Appearance appearance74 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AvgWrites");
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WaitMilliseconds");
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LoginName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ApplicationName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("StatementType", -1, 426658501);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BlockingTimeMilliseconds");
            Infragistics.Win.Appearance appearance77 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Deadlocks");
            Infragistics.Win.Appearance appearance78 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLSignatureID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLStatementText");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("HostName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CPUPerSecond", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance79 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IOPerSecond");
            Infragistics.Win.Appearance appearance80 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Occurrences");
            Infragistics.Win.Appearance appearance81 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AggregationFlag", -1, 1036051375);
            Infragistics.Win.Appearance appearance82 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLTextID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DoNotAggregate", -1, 629637243);
            Infragistics.Win.Appearance appearance83 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Spid");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CPUPctCalc");
            Infragistics.Win.Appearance appearance84 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReadsPctCalc");
            Infragistics.Win.Appearance appearance85 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WritesPctCalc");
            Infragistics.Win.Appearance appearance86 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryNameSort");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalReads");
            Infragistics.Win.Appearance appearance87 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalWrites");
            Infragistics.Win.Appearance appearance88 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TotalCPUMilliseconds");
            Infragistics.Win.Appearance appearance89 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(426658501);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(1036051375);
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(629637243);
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.QueryMonitorView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.chartsTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.chartPanel2 = new Idera.SQLdm.DesktopClient.Controls.ChartPanel();
            this.chartPanel1 = new Idera.SQLdm.DesktopClient.Controls.ChartPanel();
            this.queryMonitorGridPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.queryMonitorGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.queryMonitorGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryMonitorGridHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.closeQueryMonitorGridButton = new System.Windows.Forms.ToolStripButton();
            this.maximizeQueryMonitorGridButton = new System.Windows.Forms.ToolStripButton();
            this.restoreQueryMonitorGridButton = new System.Windows.Forms.ToolStripButton();
            this.detailValuesNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.ToolStripNumericUpDown();
            this.detailValuesLabel = new System.Windows.Forms.ToolStripLabel();
            this.queryMonitorHistoryPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.queryNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.splitLabel = new System.Windows.Forms.Label();
            this.keepHistoryValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryNameValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryMonitorHistoryPanelHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.historySignatureLabel = new System.Windows.Forms.ToolStripLabel();
            this.querySignatureValueLabel = new System.Windows.Forms.ToolStripLabel();
            this.keepDetailedHistoryButton = new System.Windows.Forms.ToolStripButton();
            this.viewHistorySqlTextButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.executionsPerDayValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.executionsPerDayLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.executionsValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.executionsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxWritesValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxWritesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgWritesValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgWritesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxReadsValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxReadsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgReadsValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgReadsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxCPUValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maxCPULabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgCPUValueLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.avgCPULabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryMonitorFiltersPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.includeOnlyResourceRowsCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.includeSqlBatchesCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.applicationTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.includeStoredProcedureCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.appLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.includeSqlStatementsCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox(); //new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.dbLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endTimeDateTimePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.databaseTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.EndTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.userLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.beginTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.userTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.sqlTextIncludeTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.beginTimeDateTimePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.wsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endDateLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.includeSqlLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.endDateDateTimePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.hostTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.beginDateLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlTextExcludeTextBox = new Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox();
            this.beginDateDateTimePicker = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDateTimePicker();
            this.excludeSqlLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryMonitorFiltersHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.closeQueryMonitorFiltersButton = new System.Windows.Forms.ToolStripButton();
            this.clearQueryMonitorFiltersButton = new System.Windows.Forms.ToolStripButton();
            this.historySelectLabel = new System.Windows.Forms.ToolStripLabel();
            this.useWildcardLabel = new System.Windows.Forms.ToolStripLabel();
            this.updatingStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.updatingStatusImage = new System.Windows.Forms.PictureBox();
            this.updatingStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            // this.advancedQueryViewPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            // this.advancedQueryViewImage = new System.Windows.Forms.PictureBox();
            // this.advancedQueryViewLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.operationalStatusPanel = new System.Windows.Forms.Panel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new System.Windows.Forms.Label();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.QueryMonitorView_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.chartsTableLayoutPanel.SuspendLayout();
            this.queryMonitorGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.queryMonitorGrid)).BeginInit();
            this.queryMonitorGridHeaderStrip.SuspendLayout();
            this.queryMonitorHistoryPanel.SuspendLayout();
            this.queryMonitorHistoryPanelHeaderStrip.SuspendLayout();
            this.queryMonitorFiltersPanel.SuspendLayout();
            this.queryMonitorFiltersHeaderStrip.SuspendLayout();
            this.updatingStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updatingStatusImage)).BeginInit();
           // this.advancedQueryViewPanel.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.advancedQueryViewImage)).BeginInit();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.SettingsKey = "QueryMonitorView.toolbarsManager";
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "columnContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            stateButtonTool1,
            buttonTool3,
            buttonTool4,
            buttonTool5});
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance2;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance4;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            appearance5.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance5;
            buttonTool11.SharedPropsInternal.Caption = "Export To Excel";
            appearance6.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance6;
            buttonTool12.SharedPropsInternal.Caption = "Print";
            appearance7.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance7;
            buttonTool13.SharedPropsInternal.Caption = "Print";
            appearance8.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ExportChartImage16x16;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance8;
            buttonTool14.SharedPropsInternal.Caption = "Save Image";
            appearance9.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance9;
            buttonTool15.SharedPropsInternal.Caption = "Export To Excel (csv)";
            popupMenuTool2.SharedPropsInternal.Caption = "chartContexMenu";
            appearance33.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool33.SharedPropsInternal.AppearancesSmall.Appearance = appearance33;
            buttonTool33.SharedPropsInternal.Caption = "Show Query Plan";
            appearance35.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool35.SharedPropsInternal.AppearancesSmall.Appearance = appearance35;
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool16.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool3,
            buttonTool16,
            buttonTool17,
            buttonTool18
            });
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool19.InstanceProps.IsFirstInGroup = true;
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            buttonTool35
            });
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool4.SharedPropsInternal.Caption = "Toolbar";
            buttonTool23.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool24.SharedPropsInternal.Caption = "Expand All Groups";
            appearance10.Image = ((object)(resources.GetObject("appearance10.Image")));
            buttonTool25.SharedPropsInternal.AppearancesSmall.Appearance = appearance10;
            buttonTool25.SharedPropsInternal.Caption = "Show Query History";
            appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Queries;
            buttonTool26.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool26.SharedPropsInternal.Caption = "Show Query Text";
            popupMenuTool4.SharedPropsInternal.Caption = "gridDataContextMenu";
            stateButtonTool5.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool29.InstanceProps.IsFirstInGroup = true;
            buttonTool31.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool27,
            buttonTool28,
            stateButtonTool5,
            buttonTool29,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            buttonTool34});
            stateButtonTool6.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            appearance12.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.History16;
            stateButtonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            stateButtonTool6.SharedPropsInternal.Caption = "Keep Detailed History";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            popupMenuTool2,
            popupMenuTool3,
            stateButtonTool4,
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            popupMenuTool4,
            buttonTool33,
            stateButtonTool6});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // QueryMonitorView_Fill_Panel
            // 
            this.QueryMonitorView_Fill_Panel.Controls.Add(this.splitContainer);
            this.QueryMonitorView_Fill_Panel.Controls.Add(this.queryMonitorHistoryPanel);
            this.QueryMonitorView_Fill_Panel.Controls.Add(this.queryMonitorFiltersPanel);
            this.QueryMonitorView_Fill_Panel.Controls.Add(this.updatingStatusPanel);
          //  this.QueryMonitorView_Fill_Panel.Controls.Add(this.advancedQueryViewPanel);
            this.QueryMonitorView_Fill_Panel.Controls.Add(this.operationalStatusPanel);
            this.QueryMonitorView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.QueryMonitorView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.QueryMonitorView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.QueryMonitorView_Fill_Panel.Name = "QueryMonitorView_Fill_Panel";
            this.QueryMonitorView_Fill_Panel.Size = new System.Drawing.Size(1073, 562);
            this.QueryMonitorView_Fill_Panel.TabIndex = 8;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 294);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.splitContainer.Panel1.Controls.Add(this.chartsTableLayoutPanel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.splitContainer.Panel1MinSize = 50;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.queryMonitorGridPanel);
            this.splitContainer.Panel2MinSize = 50;
            this.splitContainer.Size = new System.Drawing.Size(1073, 295);
            this.splitContainer.SplitterDistance = 198;
            this.splitContainer.TabIndex = 1;
            this.splitContainer.SizeChanged += new System.EventHandler(this.splitContainer_SizeChanged);
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // chartsTableLayoutPanel
            // 
            this.chartsTableLayoutPanel.ColumnCount = 2;
            this.chartsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.chartsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.chartsTableLayoutPanel.Controls.Add(this.chartPanel2, 1, 0);
            this.chartsTableLayoutPanel.Controls.Add(this.chartPanel1, 0, 0);
            this.chartsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartsTableLayoutPanel.Location = new System.Drawing.Point(0, 1);
            this.chartsTableLayoutPanel.Name = "chartsTableLayoutPanel";
            this.chartsTableLayoutPanel.RowCount = 1;
            this.chartsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.chartsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.chartsTableLayoutPanel.Size = new System.Drawing.Size(1073, 196);
            this.chartsTableLayoutPanel.TabIndex = 5;
            // 
            // chartPanel2
            // 
            this.chartPanel2.AxisXTooltipShowsPointLabel = false;
            this.chartPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.chartPanel2.ChartVisible = false;
            this.chartPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel2.Location = new System.Drawing.Point(538, 2);
            this.chartPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.chartPanel2.MaximizeToControl = this.QueryMonitorView_Fill_Panel;
            this.chartPanel2.Name = "chartPanel2";
            this.chartPanel2.ShowFileActivityMenuOption = false;
            this.chartPanel2.Size = new System.Drawing.Size(533, 192);
            this.chartPanel2.StatusText = Idera.SQLdm.Common.Constants.LOADING;
            this.chartPanel2.TabIndex = 18;
            this.chartPanel2.ChartSelectionChanged += new System.EventHandler<Idera.SQLdm.DesktopClient.Controls.ChartSelectionEventArgs>(this.chartPanel2_ChartSelectionChanged);
            // 
            // chartPanel1
            // 
            this.chartPanel1.AxisXTooltipShowsPointLabel = false;
            this.chartPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.chartPanel1.ChartVisible = false;
            this.chartPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel1.Location = new System.Drawing.Point(2, 2);
            this.chartPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.chartPanel1.MaximizeToControl = this.QueryMonitorView_Fill_Panel;
            this.chartPanel1.Name = "chartPanel1";
            this.chartPanel1.ShowFileActivityMenuOption = false;
            this.chartPanel1.Size = new System.Drawing.Size(532, 192);
            this.chartPanel1.StatusText = Idera.SQLdm.Common.Constants.LOADING;
            this.chartPanel1.TabIndex = 17;
            this.chartPanel1.ChartSelectionChanged += new System.EventHandler<Idera.SQLdm.DesktopClient.Controls.ChartSelectionEventArgs>(this.chartPanel1_ChartSelectionChanged);
            // 
            // queryMonitorGridPanel
            // 
            this.queryMonitorGridPanel.Controls.Add(this.queryMonitorGrid);
            this.queryMonitorGridPanel.Controls.Add(this.queryMonitorGridStatusLabel);
            this.queryMonitorGridPanel.Controls.Add(this.queryMonitorGridHeaderStrip);
            this.queryMonitorGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMonitorGridPanel.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorGridPanel.Name = "queryMonitorGridPanel";
            this.queryMonitorGridPanel.Size = new System.Drawing.Size(1073, 66);
            this.queryMonitorGridPanel.TabIndex = 7;
            // 
            // queryMonitorGrid
            // 
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.queryMonitorGrid.DisplayLayout.Appearance = appearance13;
            this.queryMonitorGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.Caption = "Query";
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Width = 79;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.Caption = "Database";
            ultraGridColumn2.Header.VisiblePosition = 11;
            //ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 125;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Format = "G";
            ultraGridColumn3.Header.Caption = "Start Time";
            ultraGridColumn3.Header.VisiblePosition = 22;
            //ultraGridColumn3.Hidden = true;
            ultraGridColumn3.Width = 150;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Format = "G";
            ultraGridColumn4.Header.Caption = "Most Recent Completion";
            ultraGridColumn4.Header.VisiblePosition = 23;
            ultraGridColumn4.Width = 150;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance72.TextHAlignAsString = "Right";
            ultraGridColumn5.CellAppearance = appearance72;
            ultraGridColumn5.Format = "N0";
            ultraGridColumn5.Header.Caption = "Avg. Duration (ms)";
            ultraGridColumn5.Header.VisiblePosition = 10;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance73.TextHAlignAsString = "Right";
            ultraGridColumn6.CellAppearance = appearance73;
            ultraGridColumn6.Format = "N0";
            ultraGridColumn6.Header.Caption = "Avg. CPU Time (ms)";
            ultraGridColumn6.Header.VisiblePosition = 12;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance74.TextHAlignAsString = "Right";
            ultraGridColumn7.CellAppearance = appearance74;
            ultraGridColumn7.Format = "N0";
            ultraGridColumn7.Header.Caption = "Avg. Reads";
            ultraGridColumn7.Header.VisiblePosition = 20;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance75.TextHAlignAsString = "Right";
            ultraGridColumn8.CellAppearance = appearance75;
            ultraGridColumn8.Format = "N0";
            ultraGridColumn8.Header.Caption = "Avg. Writes";
            ultraGridColumn8.Header.VisiblePosition = 14;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance76.TextHAlignAsString = "Right";
            ultraGridColumn9.CellAppearance = appearance76;
            ultraGridColumn9.Format = "N0";
            ultraGridColumn9.Header.Caption = "Avg. Wait Time (ms)";
            ultraGridColumn9.Header.VisiblePosition = 18;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Header.Caption = "User";
            ultraGridColumn10.Header.VisiblePosition = 8;
            //ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 125;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.Caption = "Application";
            ultraGridColumn11.Header.VisiblePosition = 9;
            //ultraGridColumn11.Hidden = true;
            ultraGridColumn11.Width = 125;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Header.Caption = "Event Type";
            ultraGridColumn12.Header.VisiblePosition = 3;
            ultraGridColumn12.Width = 130;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance77.TextHAlignAsString = "Right";
            ultraGridColumn13.CellAppearance = appearance77;
            ultraGridColumn13.Format = "N0";
            ultraGridColumn13.Header.Caption = "Avg. Blocking Time (ms)";
            ultraGridColumn13.Header.VisiblePosition = 25;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance78.TextHAlignAsString = "Right";
            ultraGridColumn14.CellAppearance = appearance78;
            ultraGridColumn14.Format = "N0";
            ultraGridColumn14.Header.VisiblePosition = 26;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn15.Header.VisiblePosition = 24;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.Header.Caption = "SQL Text";
            ultraGridColumn16.Header.VisiblePosition = 7;
            ultraGridColumn16.Width = 244;
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn17.Header.Caption = "Client";
            ultraGridColumn17.Header.VisiblePosition = 27;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance79.TextHAlignAsString = "Right";
            ultraGridColumn18.CellAppearance = appearance79;
            ultraGridColumn18.Format = "N2";
            ultraGridColumn18.Header.Caption = "Avg. CPU per sec";
            ultraGridColumn18.Header.VisiblePosition = 19;
            ultraGridColumn19.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance80.TextHAlignAsString = "Right";
            ultraGridColumn19.CellAppearance = appearance80;
            ultraGridColumn19.Format = "N2";
            ultraGridColumn19.Header.Caption = "Avg. I\\O per sec";
            ultraGridColumn19.Header.VisiblePosition = 17;
            ultraGridColumn20.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance81.TextHAlignAsString = "Right";
            ultraGridColumn20.CellAppearance = appearance81;
            ultraGridColumn20.Format = "N0";
            ultraGridColumn20.Header.VisiblePosition = 21;
            ultraGridColumn21.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn21.ColumnChooserCaption = "Aggregated";
            appearance82.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Summation9x9;
            appearance82.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn21.Header.Appearance = appearance82;
            ultraGridColumn21.Header.Caption = "";
            ultraGridColumn21.Header.VisiblePosition = 0;
            ultraGridColumn21.MaxWidth = 20;
            ultraGridColumn21.MinWidth = 20;
            ultraGridColumn21.Width = 20;
            ultraGridColumn22.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn22.Header.VisiblePosition = 29;
            //ultraGridColumn22.Hidden = true;
            ultraGridColumn23.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn23.ColumnChooserCaption = "Keep Detailed History";
            appearance83.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.History16;
            appearance83.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn23.Header.Appearance = appearance83;
            ultraGridColumn23.Header.Caption = "";
            ultraGridColumn23.Header.VisiblePosition = 1;
            ultraGridColumn23.MaxWidth = 20;
            ultraGridColumn23.MinWidth = 20;
            ultraGridColumn23.Width = 20;
            ultraGridColumn24.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn24.Header.VisiblePosition = 28;
            ultraGridColumn25.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance84.TextHAlignAsString = "Right";
            ultraGridColumn25.CellAppearance = appearance84;
            ultraGridColumn25.Format = "N2";
            ultraGridColumn25.Header.Caption = "CPU as % of List";
            ultraGridColumn25.Header.VisiblePosition = 15;
            //ultraGridColumn25.Hidden = true;
            ultraGridColumn26.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance85.TextHAlignAsString = "Right";
            ultraGridColumn26.CellAppearance = appearance85;
            ultraGridColumn26.Format = "N2";
            ultraGridColumn26.Header.Caption = "Reads as % of List";
            ultraGridColumn26.Header.VisiblePosition = 13;
            //ultraGridColumn26.Hidden = true;
            ultraGridColumn27.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance86.TextHAlignAsString = "Right";
            ultraGridColumn27.CellAppearance = appearance86;
            ultraGridColumn27.Format = "N2";
            ultraGridColumn27.Header.Caption = "Writes as % of List";
            ultraGridColumn27.Header.VisiblePosition = 16;
            //ultraGridColumn27.Hidden = true;
            ultraGridColumn28.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn28.Header.VisiblePosition = 30;
            //ultraGridColumn28.Hidden = true;
            ultraGridColumn29.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance87.TextHAlignAsString = "Right";
            ultraGridColumn29.CellAppearance = appearance87;
            ultraGridColumn29.Format = "N0";
            ultraGridColumn29.Header.Caption = "Reads";
            ultraGridColumn29.Header.VisiblePosition = 5;
            ultraGridColumn30.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance88.TextHAlignAsString = "Right";
            ultraGridColumn30.CellAppearance = appearance88;
            ultraGridColumn30.Format = "N0";
            ultraGridColumn30.Header.Caption = "Writes";
            ultraGridColumn30.Header.VisiblePosition = 6;
            ultraGridColumn31.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance89.TextHAlignAsString = "Right";
            ultraGridColumn31.CellAppearance = appearance89;
            ultraGridColumn31.Format = "N0";
            ultraGridColumn31.Header.Caption = "CPU Time (ms)";
            ultraGridColumn31.Header.VisiblePosition = 4;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31});
            this.queryMonitorGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.queryMonitorGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.queryMonitorGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.queryMonitorGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.queryMonitorGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.queryMonitorGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.queryMonitorGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.queryMonitorGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.queryMonitorGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.queryMonitorGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.queryMonitorGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.queryMonitorGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.queryMonitorGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.queryMonitorGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.queryMonitorGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            this.queryMonitorGrid.DisplayLayout.Override.CardAreaAppearance = appearance17;
            appearance18.BorderColor = System.Drawing.Color.Silver;
            appearance18.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.queryMonitorGrid.DisplayLayout.Override.CellAppearance = appearance18;
            this.queryMonitorGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.queryMonitorGrid.DisplayLayout.Override.CellPadding = 0;
            this.queryMonitorGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.queryMonitorGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.SystemColors.Control;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.BorderColor = System.Drawing.SystemColors.Window;
            this.queryMonitorGrid.DisplayLayout.Override.GroupByRowAppearance = appearance19;
            this.queryMonitorGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            this.queryMonitorGrid.DisplayLayout.Override.GroupBySummaryDisplayStyle = Infragistics.Win.UltraWinGrid.GroupBySummaryDisplayStyle.SummaryCells;
            appearance20.TextHAlignAsString = "Left";
            this.queryMonitorGrid.DisplayLayout.Override.HeaderAppearance = appearance20;
            this.queryMonitorGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            this.queryMonitorGrid.DisplayLayout.Override.RowAppearance = appearance21;
            this.queryMonitorGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.queryMonitorGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.queryMonitorGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.queryMonitorGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.queryMonitorGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.queryMonitorGrid.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.InGroupByRows;
            appearance22.BackColor = System.Drawing.SystemColors.ControlLight;
            this.queryMonitorGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance22;
            this.queryMonitorGrid.DisplayLayout.Override.TipStyleCell = Infragistics.Win.UltraWinGrid.TipStyle.Hide;
            this.queryMonitorGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.queryMonitorGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.queryMonitorGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "EventType";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "Stored Procedure";
            valueListItem2.DataValue = 1;
            valueListItem2.DisplayText = "SQL Statement";
            valueListItem3.DataValue = 2;
            valueListItem3.DisplayText = "SQL Batch";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Aggregation";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem4.DataValue = ((short)(0));
            valueListItem4.DisplayText = "No";
            appearance23.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Summation9x9;
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem5.Appearance = appearance23;
            valueListItem5.DataValue = ((short)(1));
            valueListItem5.DisplayText = "Yes";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5});
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList3.Key = "DoNotAggregate";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem6.DataValue = false;
            valueListItem6.DisplayText = "No";
            appearance24.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.History16;
            appearance24.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem7.Appearance = appearance24;
            valueListItem7.DataValue = true;
            valueListItem7.DisplayText = "Yes";
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7});
            this.queryMonitorGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.queryMonitorGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.queryMonitorGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.queryMonitorGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMonitorGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryMonitorGrid.Location = new System.Drawing.Point(0, 19);
            this.queryMonitorGrid.Name = "queryMonitorGrid";
            this.queryMonitorGrid.Size = new System.Drawing.Size(1073, 74);
            this.queryMonitorGrid.TabIndex = 21;
            this.queryMonitorGrid.Visible = false;
            this.queryMonitorGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.queryMonitorGrid_InitializeLayout);
            this.queryMonitorGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.queryMonitorGrid_InitializeRow);
            this.queryMonitorGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.queryMonitorGrid_AfterSelectChange);
            this.queryMonitorGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.queryMonitorGrid_MouseDown);
            // 
            // queryMonitorGridStatusLabel
            // 
            this.queryMonitorGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.queryMonitorGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMonitorGridStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.queryMonitorGridStatusLabel.Name = "queryMonitorGridStatusLabel";
            this.queryMonitorGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.queryMonitorGridStatusLabel.Size = new System.Drawing.Size(1073, 47);
            this.queryMonitorGridStatusLabel.TabIndex = 15;
            this.queryMonitorGridStatusLabel.Text = "< Status Message >";
            this.queryMonitorGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // queryMonitorGridHeaderStrip
            // 
            this.queryMonitorGridHeaderStrip.AutoSize = false;
            this.queryMonitorGridHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.queryMonitorGridHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.queryMonitorGridHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.queryMonitorGridHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.queryMonitorGridHeaderStrip.HotTrackEnabled = false;
            this.queryMonitorGridHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.closeQueryMonitorGridButton,
            this.maximizeQueryMonitorGridButton,
            this.restoreQueryMonitorGridButton,
            this.detailValuesNumericUpDown,
            this.detailValuesLabel});
            this.queryMonitorGridHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorGridHeaderStrip.Name = "queryMonitorGridHeaderStrip";
            this.queryMonitorGridHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.queryMonitorGridHeaderStrip.Size = new System.Drawing.Size(1073, 19);
            this.queryMonitorGridHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.queryMonitorGridHeaderStrip.TabIndex = 20;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(112, 16);
            this.toolStripLabel1.Text = "Event Occurrences";
            // 
            // closeQueryMonitorGridButton
            // 
            this.closeQueryMonitorGridButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeQueryMonitorGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeQueryMonitorGridButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.closeQueryMonitorGridButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeQueryMonitorGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeQueryMonitorGridButton.Name = "closeQueryMonitorGridButton";
            this.closeQueryMonitorGridButton.Size = new System.Drawing.Size(23, 16);
            this.closeQueryMonitorGridButton.ToolTipText = "Close";
            this.closeQueryMonitorGridButton.Click += new System.EventHandler(this.closeQueryMonitorGridButton_Click);
            // 
            // maximizeQueryMonitorGridButton
            // 
            this.maximizeQueryMonitorGridButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeQueryMonitorGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeQueryMonitorGridButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeQueryMonitorGridButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeQueryMonitorGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeQueryMonitorGridButton.Name = "maximizeQueryMonitorGridButton";
            this.maximizeQueryMonitorGridButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeQueryMonitorGridButton.ToolTipText = "Maximize";
            this.maximizeQueryMonitorGridButton.Click += new System.EventHandler(this.maximizeQueryMonitorGridButton_Click);
            // 
            // restoreQueryMonitorGridButton
            // 
            this.restoreQueryMonitorGridButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreQueryMonitorGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreQueryMonitorGridButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreQueryMonitorGridButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreQueryMonitorGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreQueryMonitorGridButton.Name = "restoreQueryMonitorGridButton";
            this.restoreQueryMonitorGridButton.Size = new System.Drawing.Size(23, 16);
            this.restoreQueryMonitorGridButton.Text = "Restore";
            this.restoreQueryMonitorGridButton.Visible = false;
            this.restoreQueryMonitorGridButton.Click += new System.EventHandler(this.restoreQueryMonitorGridButton_Click);
            // 
            // detailValuesNumericUpDown
            // 
            this.detailValuesNumericUpDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.detailValuesNumericUpDown.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailValuesNumericUpDown.ForeColor = System.Drawing.Color.Black;
            this.detailValuesNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 1, 14, 2);
            this.detailValuesNumericUpDown.Name = "detailValuesNumericUpDown";
            this.detailValuesNumericUpDown.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.detailValuesNumericUpDown.Size = new System.Drawing.Size(47, 16);
            this.detailValuesNumericUpDown.Text = "100";
            this.detailValuesNumericUpDown.ValueChanged += new System.EventHandler(this.detailValuesNumericUpDown_ValueChanged);
            // 
            // detailValuesLabel
            // 
            this.detailValuesLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.detailValuesLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.detailValuesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.detailValuesLabel.Margin = new System.Windows.Forms.Padding(0);
            this.detailValuesLabel.Name = "detailValuesLabel";
            this.detailValuesLabel.Size = new System.Drawing.Size(28, 19);
            this.detailValuesLabel.Text = "Show";
            this.detailValuesLabel.Visible = true;
            // 
            // queryMonitorHistoryPanel
            // 
            this.queryMonitorHistoryPanel.AutoScroll = true;
            this.queryMonitorHistoryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.queryMonitorHistoryPanel.Controls.Add(this.queryNameLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.splitLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.keepHistoryValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.queryNameValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.queryMonitorHistoryPanelHeaderStrip);
            this.queryMonitorHistoryPanel.Controls.Add(this.panel1);
            this.queryMonitorHistoryPanel.Controls.Add(this.executionsPerDayValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.executionsPerDayLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.executionsValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.executionsLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxWritesValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxWritesLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgWritesValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgWritesLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxReadsValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxReadsLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgReadsValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgReadsLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxCPUValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.maxCPULabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgCPUValueLabel);
            this.queryMonitorHistoryPanel.Controls.Add(this.avgCPULabel);
            this.queryMonitorHistoryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryMonitorHistoryPanel.Location = new System.Drawing.Point(0, 197);
            this.queryMonitorHistoryPanel.Name = "queryMonitorHistoryPanel";
            this.queryMonitorHistoryPanel.Size = new System.Drawing.Size(1073, 97);
            this.queryMonitorHistoryPanel.TabIndex = 7;
            this.queryMonitorHistoryPanel.Visible = false;
            // 
            // queryNameLabel
            // 
            this.queryNameLabel.AutoSize = true;
            this.queryNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.queryNameLabel.Location = new System.Drawing.Point(5, 30);
            this.queryNameLabel.Name = "queryNameLabel";
            this.queryNameLabel.Size = new System.Drawing.Size(129, 13);
            this.queryNameLabel.TabIndex = 49;
            this.queryNameLabel.Text = "Matching signature name:";
            // 
            // splitLabel
            // 
            this.splitLabel.AutoSize = false;
            this.splitLabel.Height = 2;
            this.splitLabel.BackColor = System.Drawing.Color.Black;
            if (Settings.Default.ColorScheme == "Dark")
                this.splitLabel.BackColor = Color.White;
            this.splitLabel.Location = new System.Drawing.Point(4, 50);
            this.splitLabel.Name = "queryNameLabel";
            this.splitLabel.Size = new System.Drawing.Size(1073, 1);
            this.splitLabel.TabIndex = 49;
            this.splitLabel.Text = "";
            this.splitLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            // 
            // keepHistoryValueLabel
            // 
            this.keepHistoryValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.keepHistoryValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.keepHistoryValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keepHistoryValueLabel.Location = new System.Drawing.Point(671, 30);
            this.keepHistoryValueLabel.Name = "keepHistoryValueLabel";
            this.keepHistoryValueLabel.Size = new System.Drawing.Size(399, 18);
            this.keepHistoryValueLabel.TabIndex = 48;
            this.keepHistoryValueLabel.Text = "Detailed history is currently being kept for this query";
            this.keepHistoryValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.keepHistoryValueLabel.Visible = false;
            // 
            // queryNameValueLabel
            // 
            this.queryNameValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.queryNameValueLabel.AutoEllipsis = true;
            this.queryNameValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.queryNameValueLabel.Location = new System.Drawing.Point(182, 30);
            this.queryNameValueLabel.Name = "queryNameValueLabel";
            this.queryNameValueLabel.Size = new System.Drawing.Size(351, 17);
            this.queryNameValueLabel.TabIndex = 47;
            this.queryNameValueLabel.Text = "Query 1\r\n";
            // 
            // queryMonitorHistoryPanelHeaderStrip
            // 
            this.queryMonitorHistoryPanelHeaderStrip.AutoSize = false;
            this.queryMonitorHistoryPanelHeaderStrip.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.queryMonitorHistoryPanelHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.queryMonitorHistoryPanelHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.queryMonitorHistoryPanelHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.queryMonitorHistoryPanelHeaderStrip.HotTrackEnabled = false;
            this.queryMonitorHistoryPanelHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historySignatureLabel,
            this.querySignatureValueLabel,
            this.keepDetailedHistoryButton,
            this.viewHistorySqlTextButton});
            this.queryMonitorHistoryPanelHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorHistoryPanelHeaderStrip.Name = "queryMonitorHistoryPanelHeaderStrip";
            this.queryMonitorHistoryPanelHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.queryMonitorHistoryPanelHeaderStrip.Size = new System.Drawing.Size(1073, 25);
            this.queryMonitorHistoryPanelHeaderStrip.TabIndex = 46;
            // 
            // historySignatureLabel
            // 
            this.historySignatureLabel.BackColor = System.Drawing.Color.Silver;
            this.historySignatureLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historySignatureLabel.ForeColor = System.Drawing.Color.Black;
            this.historySignatureLabel.Name = "historySignatureLabel";
            this.historySignatureLabel.Size = new System.Drawing.Size(102, 20);
            this.historySignatureLabel.Text = "Query Signature:";
            // 
            // querySignatureValueLabel
            // 
            this.querySignatureValueLabel.BackColor = System.Drawing.Color.Silver;
            this.querySignatureValueLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.querySignatureValueLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.querySignatureValueLabel.ForeColor = System.Drawing.Color.Black;
            this.querySignatureValueLabel.Name = "querySignatureValueLabel";
            this.querySignatureValueLabel.Size = new System.Drawing.Size(62, 20);
            this.querySignatureValueLabel.Text = "Signature";
            // 
            // keepDetailedHistoryButton
            // 
            this.keepDetailedHistoryButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.keepDetailedHistoryButton.BackColor = System.Drawing.Color.Silver;
            this.keepDetailedHistoryButton.Checked = true;
            this.keepDetailedHistoryButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.keepDetailedHistoryButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.keepDetailedHistoryButton.ForeColor = System.Drawing.Color.Black;
            this.keepDetailedHistoryButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.QueryHistory16;
            this.keepDetailedHistoryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.keepDetailedHistoryButton.Margin = new System.Windows.Forms.Padding(0);
            this.keepDetailedHistoryButton.Name = "keepDetailedHistoryButton";
            this.keepDetailedHistoryButton.Size = new System.Drawing.Size(144, 23);
            this.keepDetailedHistoryButton.Text = "Keep detailed history";
            this.keepDetailedHistoryButton.Click += new System.EventHandler(this.keepDetailedHistoryButton_Click);
            // 
            // viewHistorySqlTextButton
            // 
            this.viewHistorySqlTextButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.viewHistorySqlTextButton.BackColor = System.Drawing.Color.Silver;
            this.viewHistorySqlTextButton.Checked = true;
            this.viewHistorySqlTextButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewHistorySqlTextButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.viewHistorySqlTextButton.ForeColor = System.Drawing.Color.Black;
            this.viewHistorySqlTextButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Queries;
            this.viewHistorySqlTextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewHistorySqlTextButton.Margin = new System.Windows.Forms.Padding(0);
            this.viewHistorySqlTextButton.Name = "viewHistorySqlTextButton";
            this.viewHistorySqlTextButton.Size = new System.Drawing.Size(101, 23);
            this.viewHistorySqlTextButton.Text = "View Sql Text";
            this.viewHistorySqlTextButton.Click += new System.EventHandler(this.viewHistorySqlTextButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(7, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1059, 1);
            this.panel1.TabIndex = 44;
            // 
            // executionsPerDayValueLabel
            // 
            this.executionsPerDayValueLabel.AutoEllipsis = true;
            this.executionsPerDayValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.executionsPerDayValueLabel.Location = new System.Drawing.Point(946, 56);
            this.executionsPerDayValueLabel.Name = "executionsPerDayValueLabel";
            this.executionsPerDayValueLabel.Size = new System.Drawing.Size(96, 17);
            this.executionsPerDayValueLabel.TabIndex = 43;
            this.executionsPerDayValueLabel.Text = "0.00";
            this.executionsPerDayValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // executionsPerDayLabel
            // 
            this.executionsPerDayLabel.AutoSize = true;
            this.executionsPerDayLabel.BackColor = System.Drawing.Color.Transparent;
            this.executionsPerDayLabel.Location = new System.Drawing.Point(812, 56);
            this.executionsPerDayLabel.Name = "executionsPerDayLabel";
            this.executionsPerDayLabel.Size = new System.Drawing.Size(103, 13);
            this.executionsPerDayLabel.TabIndex = 42;
            this.executionsPerDayLabel.Text = "Executions Per Day:";
            // 
            // executionsValueLabel
            // 
            this.executionsValueLabel.AutoEllipsis = true;
            this.executionsValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.executionsValueLabel.Location = new System.Drawing.Point(946, 75);
            this.executionsValueLabel.Name = "executionsValueLabel";
            this.executionsValueLabel.Size = new System.Drawing.Size(96, 17);
            this.executionsValueLabel.TabIndex = 41;
            this.executionsValueLabel.Text = "0";
            this.executionsValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // executionsLabel
            // 
            this.executionsLabel.AutoSize = true;
            this.executionsLabel.BackColor = System.Drawing.Color.Transparent;
            this.executionsLabel.Location = new System.Drawing.Point(812, 75);
            this.executionsLabel.Name = "executionsLabel";
            this.executionsLabel.Size = new System.Drawing.Size(89, 13);
            this.executionsLabel.TabIndex = 40;
            this.executionsLabel.Text = "Total Executions:";
            // 
            // maxWritesValueLabel
            // 
            this.maxWritesValueLabel.AutoEllipsis = true;
            this.maxWritesValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.maxWritesValueLabel.Location = new System.Drawing.Point(615, 75);
            this.maxWritesValueLabel.Name = "maxWritesValueLabel";
            this.maxWritesValueLabel.Size = new System.Drawing.Size(157, 17);
            this.maxWritesValueLabel.TabIndex = 39;
            this.maxWritesValueLabel.Text = "0";
            this.maxWritesValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // maxWritesLabel
            // 
            this.maxWritesLabel.AutoSize = true;
            this.maxWritesLabel.BackColor = System.Drawing.Color.Transparent;
            this.maxWritesLabel.Location = new System.Drawing.Point(536, 75);
            this.maxWritesLabel.Name = "maxWritesLabel";
            this.maxWritesLabel.Size = new System.Drawing.Size(63, 13);
            this.maxWritesLabel.TabIndex = 38;
            this.maxWritesLabel.Text = "Max Writes:";
            // 
            // avgWritesValueLabel
            // 
            this.avgWritesValueLabel.AutoEllipsis = true;
            this.avgWritesValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.avgWritesValueLabel.Location = new System.Drawing.Point(642, 56);
            this.avgWritesValueLabel.Name = "avgWritesValueLabel";
            this.avgWritesValueLabel.Size = new System.Drawing.Size(130, 17);
            this.avgWritesValueLabel.TabIndex = 37;
            this.avgWritesValueLabel.Text = "0";
            this.avgWritesValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // avgWritesLabel
            // 
            this.avgWritesLabel.AutoSize = true;
            this.avgWritesLabel.BackColor = System.Drawing.Color.Transparent;
            this.avgWritesLabel.Location = new System.Drawing.Point(536, 56);
            this.avgWritesLabel.Name = "avgWritesLabel";
            this.avgWritesLabel.Size = new System.Drawing.Size(83, 13);
            this.avgWritesLabel.TabIndex = 36;
            this.avgWritesLabel.Text = "Average Writes:";
            // 
            // maxReadsValueLabel
            // 
            this.maxReadsValueLabel.AutoEllipsis = true;
            this.maxReadsValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.maxReadsValueLabel.Location = new System.Drawing.Point(343, 75);
            this.maxReadsValueLabel.Name = "maxReadsValueLabel";
            this.maxReadsValueLabel.Size = new System.Drawing.Size(157, 17);
            this.maxReadsValueLabel.TabIndex = 35;
            this.maxReadsValueLabel.Text = "0";
            this.maxReadsValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // maxReadsLabel
            // 
            this.maxReadsLabel.AutoSize = true;
            this.maxReadsLabel.BackColor = System.Drawing.Color.Transparent;
            this.maxReadsLabel.Location = new System.Drawing.Point(264, 75);
            this.maxReadsLabel.Name = "maxReadsLabel";
            this.maxReadsLabel.Size = new System.Drawing.Size(64, 13);
            this.maxReadsLabel.TabIndex = 34;
            this.maxReadsLabel.Text = "Max Reads:";
            // 
            // avgReadsValueLabel
            // 
            this.avgReadsValueLabel.AutoEllipsis = true;
            this.avgReadsValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.avgReadsValueLabel.Location = new System.Drawing.Point(370, 56);
            this.avgReadsValueLabel.Name = "avgReadsValueLabel";
            this.avgReadsValueLabel.Size = new System.Drawing.Size(130, 17);
            this.avgReadsValueLabel.TabIndex = 33;
            this.avgReadsValueLabel.Text = "0";
            this.avgReadsValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // avgReadsLabel
            // 
            this.avgReadsLabel.AutoSize = true;
            this.avgReadsLabel.BackColor = System.Drawing.Color.Transparent;
            this.avgReadsLabel.Location = new System.Drawing.Point(264, 56);
            this.avgReadsLabel.Name = "avgReadsLabel";
            this.avgReadsLabel.Size = new System.Drawing.Size(84, 13);
            this.avgReadsLabel.TabIndex = 32;
            this.avgReadsLabel.Text = "Average Reads:";
            // 
            // maxCPUValueLabel
            // 
            this.maxCPUValueLabel.AutoEllipsis = true;
            this.maxCPUValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.maxCPUValueLabel.Location = new System.Drawing.Point(80, 75);
            this.maxCPUValueLabel.Name = "maxCPUValueLabel";
            this.maxCPUValueLabel.Size = new System.Drawing.Size(161, 17);
            this.maxCPUValueLabel.TabIndex = 31;
            this.maxCPUValueLabel.Text = "0 ms";
            this.maxCPUValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // maxCPULabel
            // 
            this.maxCPULabel.AutoSize = true;
            this.maxCPULabel.BackColor = System.Drawing.Color.Transparent;
            this.maxCPULabel.Location = new System.Drawing.Point(5, 75);
            this.maxCPULabel.Name = "maxCPULabel";
            this.maxCPULabel.Size = new System.Drawing.Size(55, 13);
            this.maxCPULabel.TabIndex = 30;
            this.maxCPULabel.Text = "Max CPU:";
            // 
            // avgCPUValueLabel
            // 
            this.avgCPUValueLabel.AutoEllipsis = true;
            this.avgCPUValueLabel.BackColor = System.Drawing.Color.Transparent;
            this.avgCPUValueLabel.Location = new System.Drawing.Point(104, 56);
            this.avgCPUValueLabel.Name = "avgCPUValueLabel";
            this.avgCPUValueLabel.Size = new System.Drawing.Size(137, 17);
            this.avgCPUValueLabel.TabIndex = 29;
            this.avgCPUValueLabel.Text = "0 ms";
            this.avgCPUValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // avgCPULabel
            // 
            this.avgCPULabel.AutoSize = true;
            this.avgCPULabel.BackColor = System.Drawing.Color.Transparent;
            this.avgCPULabel.Location = new System.Drawing.Point(5, 56);
            this.avgCPULabel.Name = "avgCPULabel";
            this.avgCPULabel.Size = new System.Drawing.Size(75, 13);
            this.avgCPULabel.TabIndex = 28;
            this.avgCPULabel.Text = "Average CPU:";
            // 
            // queryMonitorFiltersPanel
            // 
            this.queryMonitorFiltersPanel.AutoScroll = true;
            this.queryMonitorFiltersPanel.Controls.Add(this.includeOnlyResourceRowsCheckBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.includeSqlBatchesCheckBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.applicationTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.includeStoredProcedureCheckBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.appLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.includeSqlStatementsCheckBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.dbLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.endTimeDateTimePicker);
            this.queryMonitorFiltersPanel.Controls.Add(this.databaseTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.EndTimeLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.userLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.beginTimeLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.userTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.sqlTextIncludeTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.beginTimeDateTimePicker);
            this.queryMonitorFiltersPanel.Controls.Add(this.wsLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.endDateLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.includeSqlLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.endDateDateTimePicker);
            this.queryMonitorFiltersPanel.Controls.Add(this.hostTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.beginDateLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.sqlTextExcludeTextBox);
            this.queryMonitorFiltersPanel.Controls.Add(this.beginDateDateTimePicker);
            this.queryMonitorFiltersPanel.Controls.Add(this.excludeSqlLabel);
            this.queryMonitorFiltersPanel.Controls.Add(this.queryMonitorFiltersHeaderStrip);
            this.queryMonitorFiltersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryMonitorFiltersPanel.Location = new System.Drawing.Point(0, 81);
            this.queryMonitorFiltersPanel.Name = "queryMonitorFiltersPanel";
            this.queryMonitorFiltersPanel.Size = new System.Drawing.Size(1073, 116);
            this.queryMonitorFiltersPanel.TabIndex = 8;
            // 
            // includeOnlyResourceRowsCheckBox
            // 
            this.includeOnlyResourceRowsCheckBox.AutoSize = true;
            this.includeOnlyResourceRowsCheckBox.Location = new System.Drawing.Point(924, 94);
            this.includeOnlyResourceRowsCheckBox.Name = "includeOnlyResourceRowsCheckBox";
            this.includeOnlyResourceRowsCheckBox.Size = new System.Drawing.Size(190, 17);
            this.includeOnlyResourceRowsCheckBox.TabIndex = 30;
            this.includeOnlyResourceRowsCheckBox.Text = "Exclude Currently Running Queries";
            this.includeOnlyResourceRowsCheckBox.UseVisualStyleBackColor = true;
            this.includeOnlyResourceRowsCheckBox.CheckedChanged += new System.EventHandler(this.ResourceRowsNeededCheckBox_CheckedChanged);
            // 
            // includeSqlBatchesCheckBox
            // 
            this.includeSqlBatchesCheckBox.AutoSize = true;
            this.includeSqlBatchesCheckBox.Location = new System.Drawing.Point(924, 72);
            this.includeSqlBatchesCheckBox.Name = "includeSqlBatchesCheckBox";
            this.includeSqlBatchesCheckBox.Size = new System.Drawing.Size(119, 17);
            this.includeSqlBatchesCheckBox.TabIndex = 16;
            this.includeSqlBatchesCheckBox.Text = "Show SQL Batches";
            this.includeSqlBatchesCheckBox.UseVisualStyleBackColor = true;
            this.includeSqlBatchesCheckBox.CheckedChanged += new System.EventHandler(this.includeSqlBatchesCheckBox_CheckedChanged);
            // 
            // applicationTextBox
            // 
            this.applicationTextBox.Location = new System.Drawing.Point(92, 54);
            this.applicationTextBox.Name = "applicationTextBox";
            this.applicationTextBox.Size = new System.Drawing.Size(149, 20);
            this.applicationTextBox.TabIndex = 8;
            this.applicationTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.applicationTextBox_KeyPress);
            this.applicationTextBox.Leave += new System.EventHandler(this.applicationTextBox_Leave);
            // 
            // includeStoredProcedureCheckBox
            // 
            this.includeStoredProcedureCheckBox.AutoSize = true;
            this.includeStoredProcedureCheckBox.Location = new System.Drawing.Point(924, 50);
            this.includeStoredProcedureCheckBox.Name = "includeStoredProcedureCheckBox";
            this.includeStoredProcedureCheckBox.Size = new System.Drawing.Size(144, 17);
            this.includeStoredProcedureCheckBox.TabIndex = 15;
            this.includeStoredProcedureCheckBox.Text = "Show Stored Procedures";
            this.includeStoredProcedureCheckBox.UseVisualStyleBackColor = true;
            this.includeStoredProcedureCheckBox.CheckedChanged += new System.EventHandler(this.includeStoredProcedureCheckBox_CheckedChanged);
            // 
            // appLabel
            // 
            this.appLabel.AutoEllipsis = true;
            this.appLabel.Location = new System.Drawing.Point(5, 54);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(81, 20);
            this.appLabel.TabIndex = 11;
            this.appLabel.Text = "Application:";
            this.appLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // includeSqlStatementsCheckBox
            // 
            this.includeSqlStatementsCheckBox.AutoSize = true;
            this.includeSqlStatementsCheckBox.CausesValidation = false;
            this.includeSqlStatementsCheckBox.Location = new System.Drawing.Point(924, 28);
            this.includeSqlStatementsCheckBox.Name = "includeSqlStatementsCheckBox";
            this.includeSqlStatementsCheckBox.Size = new System.Drawing.Size(133, 17);
            this.includeSqlStatementsCheckBox.TabIndex = 14;
            this.includeSqlStatementsCheckBox.Text = "Show SQL Statements";
            this.includeSqlStatementsCheckBox.UseVisualStyleBackColor = true;
            this.includeSqlStatementsCheckBox.CheckedChanged += new System.EventHandler(this.includeSqlStatementsCheckBox_CheckedChanged);
            // 
            // dbLabel
            // 
            this.dbLabel.AutoEllipsis = true;
            this.dbLabel.Location = new System.Drawing.Point(5, 82);
            this.dbLabel.Name = "dbLabel";
            this.dbLabel.Size = new System.Drawing.Size(81, 20);
            this.dbLabel.TabIndex = 13;
            this.dbLabel.Text = "Database:";
            this.dbLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // endTimeDateTimePicker
            // 
            this.endTimeDateTimePicker.CustomFormat = "";
            this.endTimeDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.endTimeDateTimePicker.Location = new System.Drawing.Point(792, 27);
            this.endTimeDateTimePicker.Name = "endTimeDateTimePicker";
            this.endTimeDateTimePicker.ShowUpDown = true;
            this.endTimeDateTimePicker.Size = new System.Drawing.Size(120, 20);
            this.endTimeDateTimePicker.TabIndex = 7;
            this.endTimeDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.endTimeDateTimePicker.ValueChanged += new System.EventHandler(this.endTimeDateTimePicker_ValueChanged);
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Location = new System.Drawing.Point(92, 82);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.Size = new System.Drawing.Size(149, 20);
            this.databaseTextBox.TabIndex = 11;
            this.databaseTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.databaseTextBox_KeyPress);
            this.databaseTextBox.Leave += new System.EventHandler(this.databaseTextBox_Leave);
            // 
            // EndTimeLabel
            // 
            this.EndTimeLabel.AutoEllipsis = true;
            this.EndTimeLabel.Location = new System.Drawing.Point(736, 27);
            this.EndTimeLabel.Name = "EndTimeLabel";
            this.EndTimeLabel.Size = new System.Drawing.Size(54, 20);
            this.EndTimeLabel.TabIndex = 29;
            this.EndTimeLabel.Text = "End:";
            this.EndTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userLabel
            // 
            this.userLabel.AutoEllipsis = true;
            this.userLabel.Location = new System.Drawing.Point(247, 54);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(51, 20);
            this.userLabel.TabIndex = 15;
            this.userLabel.Text = "User:";
            this.userLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // beginTimeLabel
            // 
            this.beginTimeLabel.AutoEllipsis = true;
            this.beginTimeLabel.Location = new System.Drawing.Point(481, 27);
            this.beginTimeLabel.Name = "beginTimeLabel";
            this.beginTimeLabel.Size = new System.Drawing.Size(128, 20);
            this.beginTimeLabel.TabIndex = 28;
            this.beginTimeLabel.Text = "Time Period - Begin:";
            this.beginTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // userTextBox
            // 
            this.userTextBox.Location = new System.Drawing.Point(304, 54);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(164, 20);
            this.userTextBox.TabIndex = 9;
            this.userTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userTextBox_KeyPress);
            this.userTextBox.Leave += new System.EventHandler(this.userTextBox_Leave);
            // 
            // sqlTextIncludeTextBox
            // 
            this.sqlTextIncludeTextBox.Location = new System.Drawing.Point(615, 82);
            this.sqlTextIncludeTextBox.Name = "sqlTextIncludeTextBox";
            this.sqlTextIncludeTextBox.Size = new System.Drawing.Size(300, 20);
            this.sqlTextIncludeTextBox.TabIndex = 13;
            this.sqlTextIncludeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sqlTextIncludeTextBox_KeyPress);
            this.sqlTextIncludeTextBox.Leave += new System.EventHandler(this.sqlTextIncludeTextBox_Leave);
            // 
            // beginTimeDateTimePicker
            // 
            this.beginTimeDateTimePicker.CustomFormat = "";
            this.beginTimeDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.beginTimeDateTimePicker.Location = new System.Drawing.Point(615, 27);
            this.beginTimeDateTimePicker.Name = "beginTimeDateTimePicker";
            this.beginTimeDateTimePicker.ShowUpDown = true;
            this.beginTimeDateTimePicker.Size = new System.Drawing.Size(120, 20);
            this.beginTimeDateTimePicker.TabIndex = 6;
            this.beginTimeDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.beginTimeDateTimePicker.ValueChanged += new System.EventHandler(this.beginTimeDateTimePicker_ValueChanged);
            // 
            // wsLabel
            // 
            this.wsLabel.AutoEllipsis = true;
            this.wsLabel.Location = new System.Drawing.Point(247, 82);
            this.wsLabel.Name = "wsLabel";
            this.wsLabel.Size = new System.Drawing.Size(51, 20);
            this.wsLabel.TabIndex = 17;
            this.wsLabel.Text = "Client:";
            this.wsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // endDateLabel
            // 
            this.endDateLabel.AutoEllipsis = true;
            this.endDateLabel.Location = new System.Drawing.Point(286, 27);
            this.endDateLabel.Name = "endDateLabel";
            this.endDateLabel.Size = new System.Drawing.Size(42, 20);
            this.endDateLabel.TabIndex = 22;
            this.endDateLabel.Text = "End:";
            this.endDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // includeSqlLabel
            // 
            this.includeSqlLabel.AutoEllipsis = true;
            this.includeSqlLabel.Location = new System.Drawing.Point(478, 82);
            this.includeSqlLabel.Name = "includeSqlLabel";
            this.includeSqlLabel.Size = new System.Drawing.Size(128, 20);
            this.includeSqlLabel.TabIndex = 25;
            this.includeSqlLabel.Text = "Include SQL Text:";
            this.includeSqlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // endDateDateTimePicker
            // 
            this.endDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateDateTimePicker.Location = new System.Drawing.Point(331, 27);
            this.endDateDateTimePicker.Name = "endDateDateTimePicker";
            this.endDateDateTimePicker.Size = new System.Drawing.Size(137, 20);
            this.endDateDateTimePicker.TabIndex = 5;
            this.endDateDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.endDateDateTimePicker.ValueChanged += new System.EventHandler(this.endDateDateTimePicker_ValueChanged);
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(304, 82);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(164, 20);
            this.hostTextBox.TabIndex = 12;
            this.hostTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.hostTextBox_KeyPress);
            this.hostTextBox.Leave += new System.EventHandler(this.hostTextBox_Leave);
            // 
            // beginDateLabel
            // 
            this.beginDateLabel.AutoEllipsis = true;
            this.beginDateLabel.Location = new System.Drawing.Point(5, 27);
            this.beginDateLabel.Name = "beginDateLabel";
            this.beginDateLabel.Size = new System.Drawing.Size(132, 20);
            this.beginDateLabel.TabIndex = 20;
            this.beginDateLabel.Text = "Date Range - Begin:";
            this.beginDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sqlTextExcludeTextBox
            // 
            this.sqlTextExcludeTextBox.Location = new System.Drawing.Point(615, 54);
            this.sqlTextExcludeTextBox.Name = "sqlTextExcludeTextBox";
            this.sqlTextExcludeTextBox.Size = new System.Drawing.Size(300, 20);
            this.sqlTextExcludeTextBox.TabIndex = 10;
            this.sqlTextExcludeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sqlTextExcludeTextBox_KeyPress);
            this.sqlTextExcludeTextBox.Leave += new System.EventHandler(this.sqlTextExcludeTextBox_Leave);
            // 
            // beginDateDateTimePicker
            // 
            this.beginDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.beginDateDateTimePicker.Location = new System.Drawing.Point(143, 27);
            this.beginDateDateTimePicker.Name = "beginDateDateTimePicker";
            this.beginDateDateTimePicker.Size = new System.Drawing.Size(137, 20);
            this.beginDateDateTimePicker.TabIndex = 4;
            this.beginDateDateTimePicker.Value = new System.DateTime(2010, 9, 1, 0, 0, 0, 0);
            this.beginDateDateTimePicker.ValueChanged += new System.EventHandler(this.beginDateDateTimePicker_ValueChanged);
            // 
            // excludeSqlLabel
            // 
            this.excludeSqlLabel.AutoEllipsis = true;
            this.excludeSqlLabel.Location = new System.Drawing.Point(478, 54);
            this.excludeSqlLabel.Name = "excludeSqlLabel";
            this.excludeSqlLabel.Size = new System.Drawing.Size(128, 20);
            this.excludeSqlLabel.TabIndex = 23;
            this.excludeSqlLabel.Text = "Exclude SQL Text:";
            this.excludeSqlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // queryMonitorFiltersHeaderStrip
            // 
            this.queryMonitorFiltersHeaderStrip.AutoSize = false;
            this.queryMonitorFiltersHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.queryMonitorFiltersHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.queryMonitorFiltersHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.queryMonitorFiltersHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.queryMonitorFiltersHeaderStrip.HotTrackEnabled = false;
            this.queryMonitorFiltersHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.closeQueryMonitorFiltersButton,
            this.clearQueryMonitorFiltersButton,
            this.historySelectLabel,
            this.useWildcardLabel});
            this.queryMonitorFiltersHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorFiltersHeaderStrip.Name = "queryMonitorFiltersHeaderStrip";
            this.queryMonitorFiltersHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.queryMonitorFiltersHeaderStrip.Size = new System.Drawing.Size(1114, 19);
            this.queryMonitorFiltersHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.queryMonitorFiltersHeaderStrip.TabIndex = 3;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(42, 16);
            this.toolStripLabel2.Text = "Filters";
            // 
            // closeQueryMonitorFiltersButton
            // 
            this.closeQueryMonitorFiltersButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeQueryMonitorFiltersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeQueryMonitorFiltersButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.closeQueryMonitorFiltersButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeQueryMonitorFiltersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeQueryMonitorFiltersButton.Name = "closeQueryMonitorFiltersButton";
            this.closeQueryMonitorFiltersButton.Size = new System.Drawing.Size(23, 16);
            this.closeQueryMonitorFiltersButton.ToolTipText = "Maximize";
            this.closeQueryMonitorFiltersButton.Click += new System.EventHandler(this.closeQueryMonitorFiltersButton_Click);
            // 
            // clearQueryMonitorFiltersButton
            // 
            this.clearQueryMonitorFiltersButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearQueryMonitorFiltersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearQueryMonitorFiltersButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.funnel_delete;
            this.clearQueryMonitorFiltersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearQueryMonitorFiltersButton.Name = "clearQueryMonitorFiltersButton";
            this.clearQueryMonitorFiltersButton.Size = new System.Drawing.Size(23, 16);
            this.clearQueryMonitorFiltersButton.Text = "Clear Filters";
            this.clearQueryMonitorFiltersButton.Click += new System.EventHandler(this.clearQueryMonitorFiltersButton_Click);
            // 
            // historySelectLabel
            // 
            this.historySelectLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.historySelectLabel.Font = new System.Drawing.Font("Segoe UI", 6.25F);
            this.historySelectLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.historySelectLabel.Name = "historySelectLabel";
            this.historySelectLabel.Size = new System.Drawing.Size(268, 15);
            this.historySelectLabel.Text = "Use Signature Mode or Statement Mode to select a different Query";
            this.historySelectLabel.Visible = false;
            this.historySelectLabel.ForeColor = Color.White;
            // 
            // useWildcardLabel
            // 
            this.useWildcardLabel.Font = new System.Drawing.Font("Segoe UI", 6.25F);
            this.useWildcardLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.useWildcardLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.useWildcardLabel.Name = "useWildcardLabel";
            this.useWildcardLabel.Size = new System.Drawing.Size(80, 15);
            this.useWildcardLabel.Text = "(use % as wildcard)";
            // 
            // updatingStatusPanel
            // 
            this.updatingStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(189)))), ((int)(((byte)(105)))));
            this.updatingStatusPanel.Controls.Add(this.updatingStatusImage);
            this.updatingStatusPanel.Controls.Add(this.updatingStatusLabel);
            this.updatingStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.updatingStatusPanel.ForeColor = System.Drawing.Color.Black;
            this.updatingStatusPanel.Location = new System.Drawing.Point(0, 54);
            this.updatingStatusPanel.Name = "updatingStatusPanel";
            this.updatingStatusPanel.Size = new System.Drawing.Size(1073, 27);
            this.updatingStatusPanel.TabIndex = 9;
            this.updatingStatusPanel.Visible = false;
            // 
            // updatingStatusImage
            // 
            this.updatingStatusImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(189)))), ((int)(((byte)(105)))));
            this.updatingStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.updatingStatusImage.Location = new System.Drawing.Point(7, 5);
            this.updatingStatusImage.Name = "updatingStatusImage";
            this.updatingStatusImage.Size = new System.Drawing.Size(16, 16);
            this.updatingStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.updatingStatusImage.TabIndex = 3;
            this.updatingStatusImage.TabStop = false;
            // 
            // updatingStatusLabel
            // 
            this.updatingStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updatingStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(189)))), ((int)(((byte)(105)))));
            this.updatingStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.updatingStatusLabel.Location = new System.Drawing.Point(4, 3);
            this.updatingStatusLabel.Name = "updatingStatusLabel";
            this.updatingStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.updatingStatusLabel.Size = new System.Drawing.Size(1065, 21);
            this.updatingStatusLabel.TabIndex = 2;
            this.updatingStatusLabel.Text = "< updating status message >";
            this.updatingStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // advancedQueryViewPanel SQLdm 10.4 Remove
            /*
            this.advancedQueryViewImage.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewPanel.Controls.Add(this.advancedQueryViewImage);
            this.advancedQueryViewPanel.Controls.Add(this.advancedQueryViewLabel);
            this.advancedQueryViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advancedQueryViewPanel.Location = new System.Drawing.Point(0, 27);
            this.advancedQueryViewPanel.Name = "advancedQueryViewPanel";
            this.advancedQueryViewPanel.Size = new System.Drawing.Size(1073, 27);
            this.advancedQueryViewPanel.TabIndex = 10;
            this.advancedQueryViewPanel.Visible = false;
            // 
            // advancedQueryViewImage
            // 
            this.advancedQueryViewImage.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusInfoSmall;
            this.advancedQueryViewImage.Location = new System.Drawing.Point(7, 5);
            this.advancedQueryViewImage.Name = "advancedQueryViewImage";
            this.advancedQueryViewImage.Size = new System.Drawing.Size(16, 16);
            this.advancedQueryViewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.advancedQueryViewImage.TabIndex = 0;
            this.advancedQueryViewImage.TabStop = false;
            // 
            // advancedQueryViewLabel
            // 
            this.advancedQueryViewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedQueryViewLabel.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewLabel.ForeColor = System.Drawing.Color.Black;
            this.advancedQueryViewLabel.Location = new System.Drawing.Point(4, 3);
            this.advancedQueryViewLabel.Name = "advancedQueryViewLabel";
            this.advancedQueryViewLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.advancedQueryViewLabel.Size = new System.Drawing.Size(1065, 21);
            this.advancedQueryViewLabel.TabIndex = 1;
            this.advancedQueryViewLabel.Text = "< Advanced Query status message >";
            this.advancedQueryViewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.advancedQueryViewLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.advancedQueryViewLabel_MouseDown);
            this.advancedQueryViewLabel.MouseEnter += new System.EventHandler(this.advancedQueryViewLabel_MouseEnter);
            this.advancedQueryViewLabel.MouseLeave += new System.EventHandler(this.advancedQueryViewLabel_MouseLeave);
            this.advancedQueryViewLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.advancedQueryViewLabel_MouseUp);
            */ 
            // operationalStatusPanel
            /// 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(1073, 27);
            this.operationalStatusPanel.TabIndex = 3;
            this.operationalStatusPanel.Visible = false;
            // 
            // operationalStatusImage
            // 
            this.operationalStatusImage.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.operationalStatusImage.Location = new System.Drawing.Point(7, 5);
            this.operationalStatusImage.Name = "operationalStatusImage";
            this.operationalStatusImage.Size = new System.Drawing.Size(16, 16);
            this.operationalStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.operationalStatusImage.TabIndex = 3;
            this.operationalStatusImage.TabStop = false;
            // 
            // operationalStatusLabel
            // 
            this.operationalStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.operationalStatusLabel.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.operationalStatusLabel.Location = new System.Drawing.Point(4, 3);
            this.operationalStatusLabel.Name = "operationalStatusLabel";
            this.operationalStatusLabel.TabIndex = 1;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
                    this.operationalStatusLabel.Size = new System.Drawing.Size(1065, 25);
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(28, 0, 0, 0);
                    this.operationalStatusLabel.Size = new System.Drawing.Size(1065, 25);
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
                    this.operationalStatusLabel.Size = new System.Drawing.Size(1065, 25);
                }
            }
            else
            {
                this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
                this.operationalStatusLabel.Size = new System.Drawing.Size(1065, 20);
            }
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Queries";
            this.ultraGridPrintDocument.Grid = this.queryMonitorGrid;
            this.ultraGridPrintDocument.SettingsKey = "QueryMonitorView.ultraGridPrintDocument";
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(1073, 562);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 18;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
    "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // QueryMonitorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.QueryMonitorView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "QueryMonitorView";
            this.Size = new System.Drawing.Size(1073, 562);
            this.Load += new System.EventHandler(this.QueryMonitorView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.QueryMonitorView_Fill_Panel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.chartsTableLayoutPanel.ResumeLayout(false);
            this.queryMonitorGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.queryMonitorGrid)).EndInit();
            this.queryMonitorGridHeaderStrip.ResumeLayout(false);
            this.queryMonitorGridHeaderStrip.PerformLayout();
            this.queryMonitorHistoryPanel.ResumeLayout(false);
            this.queryMonitorHistoryPanel.PerformLayout();
            this.queryMonitorHistoryPanelHeaderStrip.ResumeLayout(false);
            this.queryMonitorHistoryPanelHeaderStrip.PerformLayout();
            this.queryMonitorFiltersPanel.ResumeLayout(false);
            this.queryMonitorFiltersPanel.PerformLayout();
            this.queryMonitorFiltersHeaderStrip.ResumeLayout(false);
            this.queryMonitorFiltersHeaderStrip.PerformLayout();
            this.updatingStatusPanel.ResumeLayout(false);
            this.updatingStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updatingStatusImage)).EndInit();
           // this.advancedQueryViewPanel.ResumeLayout(false);
           // this.advancedQueryViewPanel.PerformLayout();
           // ((System.ComponentModel.ISupportInitialize)(this.advancedQueryViewImage)).EndInit();
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  QueryMonitorView_Fill_Panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinGrid.UltraGrid queryMonitorGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel queryMonitorGridStatusLabel;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel  operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private System.Windows.Forms.Label operationalStatusLabel;
    //  private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  advancedQueryViewPanel;
    //  private System.Windows.Forms.PictureBox advancedQueryViewImage;
    //  private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel advancedQueryViewLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel chartsTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  queryMonitorGridPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip queryMonitorGridHeaderStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton closeQueryMonitorGridButton;
        private System.Windows.Forms.ToolStripButton maximizeQueryMonitorGridButton;
        private System.Windows.Forms.ToolStripButton restoreQueryMonitorGridButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  queryMonitorFiltersPanel;
        private Idera.SQLdm.DesktopClient.Controls.ChartPanel chartPanel1;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox hostTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel wsLabel;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox databaseTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dbLabel;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox applicationTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel appLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel beginDateLabel;
        private System.Windows.Forms.DateTimePicker beginDateDateTimePicker;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox sqlTextExcludeTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel excludeSqlLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endDateLabel;
        private System.Windows.Forms.DateTimePicker endDateDateTimePicker;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox userTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel userLabel;
        private Idera.SQLdm.DesktopClient.Controls.AllowDeleteTextBox sqlTextIncludeTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel includeSqlLabel;
        private Idera.SQLdm.DesktopClient.Controls.ChartPanel chartPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel beginTimeLabel;
        private System.Windows.Forms.DateTimePicker beginTimeDateTimePicker;
        private System.Windows.Forms.DateTimePicker endTimeDateTimePicker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel EndTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeSqlBatchesCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeStoredProcedureCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeSqlStatementsCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  queryMonitorHistoryPanel;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgCPUValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgCPULabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxReadsValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxReadsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgReadsValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgReadsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxCPUValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxCPULabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxWritesValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel maxWritesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgWritesValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel avgWritesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel executionsPerDayValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel executionsPerDayLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel executionsValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel executionsLabel;
        private Idera.SQLdm.DesktopClient.Controls.ToolStripNumericUpDown detailValuesNumericUpDown;
        private System.Windows.Forms.ToolStripLabel detailValuesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  updatingStatusPanel;
        private System.Windows.Forms.PictureBox updatingStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel updatingStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip queryMonitorFiltersHeaderStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripButton closeQueryMonitorFiltersButton;
        private System.Windows.Forms.ToolStripButton clearQueryMonitorFiltersButton;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip queryMonitorHistoryPanelHeaderStrip;
        private System.Windows.Forms.ToolStripLabel historySignatureLabel;
        private System.Windows.Forms.ToolStripButton keepDetailedHistoryButton;
        private System.Windows.Forms.ToolStripLabel querySignatureValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel keepHistoryValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel queryNameValueLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel queryNameLabel;
        private System.Windows.Forms.Label splitLabel;
        private System.Windows.Forms.ToolStripButton viewHistorySqlTextButton;
        private System.Windows.Forms.ToolStripLabel historySelectLabel;
        private System.Windows.Forms.ToolStripLabel useWildcardLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeOnlyResourceRowsCheckBox;
    }
}
