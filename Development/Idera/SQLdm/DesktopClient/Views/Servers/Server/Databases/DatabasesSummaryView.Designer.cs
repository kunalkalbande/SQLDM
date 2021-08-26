using System;
using System.Drawing;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    partial class DatabasesSummaryView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        
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
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("State", -1, 443993204);
            appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabasesSummaryView));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Chart Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status", -1, 28817266);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Recovery Model", -1, 953144188);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data File Size (MB)", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Tables (MB)");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Tables");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Indexes (MB)");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Indexes");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Text (MB)");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Text");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Used (MB)");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Used");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Unused (MB)");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Unused");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Free");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Data Full");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Potential Growth");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log File Size (MB)");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Used (MB)");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Log Used");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Unused (MB)");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Log Unused");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Log Full");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Potential Growth");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Oldest Open Transaction");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Oldest Open Transaction Start Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Backup");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date Created");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Version Compatibility", -1, 772174454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("System Tables");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Tables");
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("File Groups");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Files");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Autogrowth", -1, 450062485);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Autogrowth", -1, 450062485);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Active Sessions");
            Infragistics.Win.UltraWinGrid.UltraGridColumn inMemStorageUsage = new Infragistics.Win.UltraWinGrid.UltraGridColumn("In Memory Storage Usage Percent");
        Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance inMemAppearance = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(443993204);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(450062485);
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(953144188);
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(772174454);
            Infragistics.Win.ValueList valueList5 = new Controls.CustomControls.CustomValueList(28817266);
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground2 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("viewBackupRestoreHistoryButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("viewFilesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("viewOldestOpenTransactionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("viewBackupRestoreHistoryButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("viewFilesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("viewOldestOpenTransactionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            appearance1 = new Infragistics.Win.Appearance();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.databasesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.databasesGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.recentTrendsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.recentTrendsChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.recentTrendsChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.recentTrendsChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.recentTrendsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.recentTrendsChartSelectionDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.showActiveSessionsTrendButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showTransactionsPerSecondTrendButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showDataSizeTrendButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showLogSizeTrendButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showLogFlushesTrendButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.maximizeRecentTrendsChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreRecentTrendsChartButton = new System.Windows.Forms.ToolStripButton();
            this.capacityUsagePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.capacityUsageChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.capacityUsageChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.capacityUsageChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.capacityUsageHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.capacityUsageChartSelectionDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.showCapacityUsageInMegabytesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showCapacityUsageInPercentButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showCapacityUsageLogInMegabytesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.showCapacityUsageLogInPercent = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.maximizeCapacityUsageChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreCapacityUsageChartButton = new System.Windows.Forms.ToolStripButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.DatabasesSummaryView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.contentContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.refreshDatabasesButton = new Infragistics.Win.Misc.UltraButton();
            this.databasesFilterComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databasesGrid)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.recentTrendsPanel.SuspendLayout();
            this.recentTrendsChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentTrendsChart)).BeginInit();
            this.recentTrendsHeaderStrip.SuspendLayout();
            this.capacityUsagePanel.SuspendLayout();
            this.capacityUsageChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.capacityUsageChart)).BeginInit();
            this.capacityUsageHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.DatabasesSummaryView_Fill_Panel.SuspendLayout();
            this.contentContainerPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databasesFilterComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer.Panel1.Controls.Add(this.databasesGrid);
            this.splitContainer.Panel1.Controls.Add(this.databasesGridStatusLabel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel);
            this.splitContainer.Size = new System.Drawing.Size(689, 487);
            this.splitContainer.SplitterDistance = 314;
            this.splitContainer.SplitterWidth = 2;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // databasesGrid
            // 
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            appearance47.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.databasesGrid.DisplayLayout.Appearance = appearance47;
            this.databasesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn1.Header.Appearance = appearance2;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.LockedWidth = true;
            ultraGridColumn1.Width = 24;
            ultraGridColumn2.Header.Fixed = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 127;
            ultraGridColumn3.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 64;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.Caption = "Recovery model";
            ultraGridColumn6.Header.VisiblePosition = 5;
            appearance3.TextHAlignAsString = "Right";
            ultraGridColumn7.CellAppearance = appearance3;
            ultraGridColumn7.Format = "N2";
            ultraGridColumn7.Header.VisiblePosition = 6;
            appearance4.TextHAlignAsString = "Right";
            ultraGridColumn8.CellAppearance = appearance4;
            ultraGridColumn8.Format = "N2";
            ultraGridColumn8.Header.VisiblePosition = 7;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn9.CellAppearance = appearance5;
            ultraGridColumn9.Format = "0.00\\%";
            ultraGridColumn9.Header.VisiblePosition = 8;
            appearance6.TextHAlignAsString = "Right";
            ultraGridColumn10.CellAppearance = appearance6;
            ultraGridColumn10.Format = "N2";
            ultraGridColumn10.Header.VisiblePosition = 9;
            appearance7.TextHAlignAsString = "Right";
            ultraGridColumn11.CellAppearance = appearance7;
            ultraGridColumn11.Format = "0.00\\%";
            ultraGridColumn11.Header.VisiblePosition = 10;
            appearance8.TextHAlignAsString = "Right";
            ultraGridColumn12.CellAppearance = appearance8;
            ultraGridColumn12.Format = "N2";
            ultraGridColumn12.Header.VisiblePosition = 11;
            appearance9.TextHAlignAsString = "Right";
            ultraGridColumn13.CellAppearance = appearance9;
            ultraGridColumn13.Format = "0.00\\%";
            ultraGridColumn13.Header.VisiblePosition = 12;
            appearance10.TextHAlignAsString = "Right";
            ultraGridColumn14.CellAppearance = appearance10;
            ultraGridColumn14.Format = "N2";
            ultraGridColumn14.Header.VisiblePosition = 13;
            ultraGridColumn14.Hidden = true;
            appearance11.TextHAlignAsString = "Right";
            ultraGridColumn15.CellAppearance = appearance11;
            ultraGridColumn15.Format = "0.00\\%";
            ultraGridColumn15.Header.VisiblePosition = 14;
            ultraGridColumn15.Hidden = true;
            appearance12.TextHAlignAsString = "Right";
            ultraGridColumn16.CellAppearance = appearance12;
            ultraGridColumn16.Format = "N2";
            ultraGridColumn16.Header.VisiblePosition = 15;
            appearance13.TextHAlignAsString = "Right";
            ultraGridColumn17.CellAppearance = appearance13;
            ultraGridColumn17.Format = "0.00\\%";
            ultraGridColumn17.Header.VisiblePosition = 16;
            appearance14.TextHAlignAsString = "Right";
            ultraGridColumn18.CellAppearance = appearance14;
            ultraGridColumn18.Format = "0.00\\%";
            ultraGridColumn18.Header.VisiblePosition = 17;
            appearance15.TextHAlignAsString = "Right";
            ultraGridColumn19.CellAppearance = appearance15;
            ultraGridColumn19.Format = "0.00\\%";
            ultraGridColumn19.Header.VisiblePosition = 18;
            appearance16.TextHAlignAsString = "Right";
            ultraGridColumn20.CellAppearance = appearance16;
            ultraGridColumn20.Format = "N0";
            ultraGridColumn20.Header.Caption = "Data Potential Growth (MB)";
            ultraGridColumn20.Header.VisiblePosition = 19;
            appearance17.TextHAlignAsString = "Right";
            ultraGridColumn21.CellAppearance = appearance17;
            ultraGridColumn21.Format = "N2";
            ultraGridColumn21.Header.VisiblePosition = 20;
            appearance18.TextHAlignAsString = "Right";
            ultraGridColumn22.CellAppearance = appearance18;
            ultraGridColumn22.Format = "N2";
            ultraGridColumn22.Header.VisiblePosition = 21;
            appearance19.TextHAlignAsString = "Right";
            ultraGridColumn23.CellAppearance = appearance19;
            ultraGridColumn23.Format = "0.00\\%";
            ultraGridColumn23.Header.VisiblePosition = 22;
            appearance20.TextHAlignAsString = "Right";
            ultraGridColumn24.CellAppearance = appearance20;
            ultraGridColumn24.Format = "N2";
            ultraGridColumn24.Header.VisiblePosition = 23;
            appearance21.TextHAlignAsString = "Right";
            ultraGridColumn25.CellAppearance = appearance21;
            ultraGridColumn25.Format = "0.00\\%";
            ultraGridColumn25.Header.VisiblePosition = 24;
            appearance22.TextHAlignAsString = "Right";
            ultraGridColumn26.CellAppearance = appearance22;
            ultraGridColumn26.Format = "0.00\\%";
            ultraGridColumn26.Header.VisiblePosition = 25;
            appearance23.TextHAlignAsString = "Right";
            ultraGridColumn27.CellAppearance = appearance23;
            ultraGridColumn27.Format = "N0";
            ultraGridColumn27.Header.Caption = "Log Potential Growth (MB)";
            ultraGridColumn27.Header.VisiblePosition = 26;
            ultraGridColumn28.Header.VisiblePosition = 27;
            ultraGridColumn29.Format = "G";
            ultraGridColumn29.Header.VisiblePosition = 28;
            ultraGridColumn30.Format = "G";
            ultraGridColumn30.Header.VisiblePosition = 29;
            ultraGridColumn31.Format = "G";
            ultraGridColumn31.Header.VisiblePosition = 30;
            ultraGridColumn32.Header.Caption = "Compatibility level";
            ultraGridColumn32.Header.VisiblePosition = 31;
            appearance24.TextHAlignAsString = "Right";
            ultraGridColumn33.CellAppearance = appearance24;
            ultraGridColumn33.Format = "N0";
            ultraGridColumn33.Header.VisiblePosition = 32;
            ultraGridColumn33.Hidden = true;
            appearance25.TextHAlignAsString = "Right";
            ultraGridColumn34.CellAppearance = appearance25;
            ultraGridColumn34.Format = "N0";
            ultraGridColumn34.Header.Caption = "User Tables";
            ultraGridColumn34.Header.VisiblePosition = 33;
            appearance26.TextHAlignAsString = "Right";
            ultraGridColumn35.CellAppearance = appearance26;
            ultraGridColumn35.Format = "N0";
            ultraGridColumn35.Header.VisiblePosition = 34;
            appearance27.TextHAlignAsString = "Right";
            ultraGridColumn36.CellAppearance = appearance27;
            ultraGridColumn36.Format = "N0";
            ultraGridColumn36.Header.VisiblePosition = 35;
            ultraGridColumn37.Header.VisiblePosition = 36;
            ultraGridColumn38.Header.VisiblePosition = 37;
            appearance28.TextHAlignAsString = "Right";
            ultraGridColumn39.CellAppearance = appearance28;
            ultraGridColumn39.Format = "N0";
            ultraGridColumn39.Header.VisiblePosition = 38;
            inMemStorageUsage.Header.Caption = "In Memory Storage Usage Percent";
            inMemStorageUsage.Header.VisiblePosition = 39;
            inMemAppearance.TextHAlignAsString = "Right";
            inMemStorageUsage.CellAppearance = inMemAppearance;
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
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            inMemStorageUsage});
            this.databasesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.databasesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.databasesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance29.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.databasesGrid.DisplayLayout.GroupByBox.Appearance = appearance29;
            appearance30.ForeColor = System.Drawing.SystemColors.GrayText;
            this.databasesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance30;
            this.databasesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.databasesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance31.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance31.BackColor2 = System.Drawing.SystemColors.Control;
            appearance31.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance31.ForeColor = System.Drawing.SystemColors.GrayText;
            this.databasesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance31;
            this.databasesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.databasesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.databasesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.databasesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.databasesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.databasesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.databasesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance32.BackColor = System.Drawing.SystemColors.Window;
            this.databasesGrid.DisplayLayout.Override.CardAreaAppearance = appearance32;
            appearance33.BorderColor = System.Drawing.Color.Silver;
            appearance33.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.databasesGrid.DisplayLayout.Override.CellAppearance = appearance33;
            this.databasesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.databasesGrid.DisplayLayout.Override.CellPadding = 0;
            this.databasesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.databasesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance34.BackColor = System.Drawing.SystemColors.Control;
            appearance34.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance34.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance34.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance34.BorderColor = System.Drawing.SystemColors.Window;
            this.databasesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance34;
            this.databasesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance35.TextHAlignAsString = "Left";
            this.databasesGrid.DisplayLayout.Override.HeaderAppearance = appearance35;
            this.databasesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            appearance36.BorderColor = System.Drawing.Color.Silver;
            this.databasesGrid.DisplayLayout.Override.RowAppearance = appearance36;
            this.databasesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.databasesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.databasesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.databasesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.databasesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance37.BackColor = System.Drawing.SystemColors.ControlLight;
            this.databasesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance37;
            this.databasesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.databasesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.databasesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "stateValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.Key = "autoGrowthValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.Key = "recoveryModelValueList";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList4.Key = "compatibilityLevelValueList";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList4.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            valueList5.Key = "statusValueList";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.databasesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3,
            valueList4,
            valueList5});
            this.databasesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.databasesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.databasesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databasesGrid.Location = new System.Drawing.Point(0, 1);
            this.databasesGrid.Name = "databasesGrid";
            this.databasesGrid.Size = new System.Drawing.Size(689, 312);
            this.databasesGrid.TabIndex = 3;
            this.databasesGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.databasesGrid_InitializeLayout);
            this.databasesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.databasesGrid_AfterSelectChange);
            this.databasesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.databasesGrid_MouseDown);
            // 
            // databasesGridStatusLabel
            // 
            this.databasesGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.databasesGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesGridStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.databasesGridStatusLabel.Name = "databasesGridStatusLabel";
            this.databasesGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.databasesGridStatusLabel.Size = new System.Drawing.Size(689, 312);
            this.databasesGridStatusLabel.TabIndex = 5;
            this.databasesGridStatusLabel.Text = "< Status Message >";
            this.databasesGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.recentTrendsPanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.capacityUsagePanel, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(689, 171);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // recentTrendsPanel
            // 
            this.recentTrendsPanel.Controls.Add(this.recentTrendsChartContainerPanel);
            this.recentTrendsPanel.Controls.Add(this.recentTrendsHeaderStrip);
            this.recentTrendsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentTrendsPanel.Location = new System.Drawing.Point(347, 3);
            this.recentTrendsPanel.Name = "recentTrendsPanel";
            this.recentTrendsPanel.Size = new System.Drawing.Size(339, 165);
            this.recentTrendsPanel.TabIndex = 4;
            // 
            // recentTrendsChartContainerPanel
            // 
            this.recentTrendsChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.recentTrendsChartContainerPanel.Controls.Add(this.recentTrendsChart);
            this.recentTrendsChartContainerPanel.Controls.Add(this.recentTrendsChartStatusLabel);
            this.recentTrendsChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentTrendsChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.recentTrendsChartContainerPanel.Name = "recentTrendsChartContainerPanel";
            this.recentTrendsChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.recentTrendsChartContainerPanel.Size = new System.Drawing.Size(339, 146);
            this.recentTrendsChartContainerPanel.TabIndex = 12;
            // 
            // recentTrendsChart
            // 
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.recentTrendsChart.Background = gradientBackground1;
            this.recentTrendsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.recentTrendsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.recentTrendsChart, "chartContextMenu");
            this.recentTrendsChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.recentTrendsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentTrendsChart.Location = new System.Drawing.Point(1, 0);
            this.recentTrendsChart.Name = "recentTrendsChart";
            this.recentTrendsChart.Palette = "Schemes.Classic";
            this.recentTrendsChart.PlotAreaColor = System.Drawing.Color.White;
            this.recentTrendsChart.Size = new System.Drawing.Size(337, 145);
            this.recentTrendsChart.TabIndex = 13;
            this.recentTrendsChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            this.recentTrendsChart.Resize += new System.EventHandler(this.recentTrendsChart_Resize);
            // 
            // recentTrendsChartStatusLabel
            // 
            this.recentTrendsChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.recentTrendsChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentTrendsChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.recentTrendsChartStatusLabel.Name = "recentTrendsChartStatusLabel";
            this.recentTrendsChartStatusLabel.Size = new System.Drawing.Size(337, 145);
            this.recentTrendsChartStatusLabel.TabIndex = 12;
            this.recentTrendsChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.recentTrendsChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // recentTrendsHeaderStrip
            // 
            this.recentTrendsHeaderStrip.AutoSize = false;
            this.recentTrendsHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.recentTrendsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.recentTrendsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.recentTrendsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.recentTrendsHeaderStrip.HotTrackEnabled = false;
            this.recentTrendsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recentTrendsChartSelectionDropDownButton,
            this.maximizeRecentTrendsChartButton,
            this.restoreRecentTrendsChartButton});
            this.recentTrendsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.recentTrendsHeaderStrip.Name = "recentTrendsHeaderStrip";
            this.recentTrendsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.recentTrendsHeaderStrip.Size = new System.Drawing.Size(339, 19);
            this.recentTrendsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.recentTrendsHeaderStrip.TabIndex = 10;
            // 
            // recentTrendsChartSelectionDropDownButton
            // 
            this.recentTrendsChartSelectionDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showActiveSessionsTrendButton,
            this.showTransactionsPerSecondTrendButton,
            this.showDataSizeTrendButton,
            this.showLogSizeTrendButton,
            this.showLogFlushesTrendButton});
            this.recentTrendsChartSelectionDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recentTrendsChartSelectionDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.recentTrendsChartSelectionDropDownButton.Name = "recentTrendsChartSelectionDropDownButton";
            this.recentTrendsChartSelectionDropDownButton.Size = new System.Drawing.Size(196, 16);
            this.recentTrendsChartSelectionDropDownButton.Text = "Recent Trends: Active Sessions";
            // 
            // showActiveSessionsTrendButton
            // 
            this.showActiveSessionsTrendButton.Name = "showActiveSessionsTrendButton";
            this.showActiveSessionsTrendButton.Size = new System.Drawing.Size(172, 22);
            this.showActiveSessionsTrendButton.Text = "Active Sessions";
            this.showActiveSessionsTrendButton.Click += new System.EventHandler(this.showActiveSessionsTrendButton_Click);
            // 
            // showTransactionsPerSecondTrendButton
            // 
            this.showTransactionsPerSecondTrendButton.Name = "showTransactionsPerSecondTrendButton";
            this.showTransactionsPerSecondTrendButton.Size = new System.Drawing.Size(172, 22);
            this.showTransactionsPerSecondTrendButton.Text = "Transactions/sec";
            this.showTransactionsPerSecondTrendButton.Click += new System.EventHandler(this.showTransactionsPerSecondTrendButton_Click);
            // 
            // showDataSizeTrendButton
            // 
            this.showDataSizeTrendButton.Name = "showDataSizeTrendButton";
            this.showDataSizeTrendButton.Size = new System.Drawing.Size(172, 22);
            this.showDataSizeTrendButton.Text = "Data Size (MB)";
            this.showDataSizeTrendButton.Click += new System.EventHandler(this.showDataSizeTrendButton_Click);
            // 
            // showLogSizeTrendButton
            // 
            this.showLogSizeTrendButton.Name = "showLogSizeTrendButton";
            this.showLogSizeTrendButton.Size = new System.Drawing.Size(172, 22);
            this.showLogSizeTrendButton.Text = "Log Size (MB)";
            this.showLogSizeTrendButton.Click += new System.EventHandler(this.showLogSizeTrendButton_Click);
            // 
            // showLogFlushesTrendButton
            // 
            this.showLogFlushesTrendButton.Name = "showLogFlushesTrendButton";
            this.showLogFlushesTrendButton.Size = new System.Drawing.Size(172, 22);
            this.showLogFlushesTrendButton.Text = "Log Flushes";
            this.showLogFlushesTrendButton.Click += new System.EventHandler(this.showLogFlushesTrendButton_Click);
            // 
            // maximizeRecentTrendsChartButton
            // 
            this.maximizeRecentTrendsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeRecentTrendsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeRecentTrendsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeRecentTrendsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeRecentTrendsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeRecentTrendsChartButton.Name = "maximizeRecentTrendsChartButton";
            this.maximizeRecentTrendsChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeRecentTrendsChartButton.ToolTipText = "Maximize";
            this.maximizeRecentTrendsChartButton.Click += new System.EventHandler(this.maximizeRecentTrendsChartButton_Click);
            // 
            // restoreRecentTrendsChartButton
            // 
            this.restoreRecentTrendsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreRecentTrendsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreRecentTrendsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreRecentTrendsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreRecentTrendsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreRecentTrendsChartButton.Name = "restoreRecentTrendsChartButton";
            this.restoreRecentTrendsChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreRecentTrendsChartButton.Text = "Restore";
            this.restoreRecentTrendsChartButton.Visible = false;
            this.restoreRecentTrendsChartButton.Click += new System.EventHandler(this.restoreRecentTrendsChartButton_Click);
            // 
            // capacityUsagePanel
            // 
            this.capacityUsagePanel.Controls.Add(this.capacityUsageChartContainerPanel);
            this.capacityUsagePanel.Controls.Add(this.capacityUsageHeaderStrip);
            this.capacityUsagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capacityUsagePanel.Location = new System.Drawing.Point(3, 3);
            this.capacityUsagePanel.Name = "capacityUsagePanel";
            this.capacityUsagePanel.Size = new System.Drawing.Size(338, 165);
            this.capacityUsagePanel.TabIndex = 3;
            // 
            // capacityUsageChartContainerPanel
            // 
            this.capacityUsageChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.capacityUsageChartContainerPanel.Controls.Add(this.capacityUsageChart);
            this.capacityUsageChartContainerPanel.Controls.Add(this.capacityUsageChartStatusLabel);
            this.capacityUsageChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capacityUsageChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.capacityUsageChartContainerPanel.Name = "capacityUsageChartContainerPanel";
            this.capacityUsageChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.capacityUsageChartContainerPanel.Size = new System.Drawing.Size(338, 146);
            this.capacityUsageChartContainerPanel.TabIndex = 12;
            // 
            // capacityUsageChart
            // 
            this.capacityUsageChart.AllSeries.BarShape = ChartFX.WinForms.BarShape.Cylinder;
            this.capacityUsageChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Gantt;
            this.capacityUsageChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            gradientBackground2.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.capacityUsageChart.Background = gradientBackground2;
            this.capacityUsageChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.capacityUsageChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.capacityUsageChart, "chartContextMenu");
            this.capacityUsageChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.capacityUsageChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capacityUsageChart.LegendBox.PlotAreaOnly = false;
            this.capacityUsageChart.Location = new System.Drawing.Point(1, 0);
            this.capacityUsageChart.Name = "capacityUsageChart";
            this.capacityUsageChart.Palette = "Schemes.Classic";
            this.capacityUsageChart.PlotAreaColor = System.Drawing.Color.White;
            this.capacityUsageChart.RandomData.Series = 4;
            this.capacityUsageChart.Size = new System.Drawing.Size(336, 145);
            this.capacityUsageChart.TabIndex = 13;
            this.capacityUsageChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            this.capacityUsageChart.MouseMove += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseMove);
            this.capacityUsageChart.GetTip += new ChartFX.WinForms.GetTipEventHandler(this.capacityUsageChart_GetTip);
            this.capacityUsageChart.Resize += new System.EventHandler(this.capacityUsageChart_Resize);
            // 
            // capacityUsageChartStatusLabel
            // 
            this.capacityUsageChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.capacityUsageChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.capacityUsageChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.capacityUsageChartStatusLabel.Name = "capacityUsageChartStatusLabel";
            this.capacityUsageChartStatusLabel.Size = new System.Drawing.Size(336, 145);
            this.capacityUsageChartStatusLabel.TabIndex = 12;
            this.capacityUsageChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.capacityUsageChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // capacityUsageHeaderStrip
            // 
            this.capacityUsageHeaderStrip.AutoSize = false;
            this.capacityUsageHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.capacityUsageHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.capacityUsageHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.capacityUsageHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.capacityUsageHeaderStrip.HotTrackEnabled = false;
            this.capacityUsageHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.capacityUsageChartSelectionDropDownButton,
            this.maximizeCapacityUsageChartButton,
            this.restoreCapacityUsageChartButton});
            this.capacityUsageHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.capacityUsageHeaderStrip.Name = "capacityUsageHeaderStrip";
            this.capacityUsageHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.capacityUsageHeaderStrip.Size = new System.Drawing.Size(338, 19);
            this.capacityUsageHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.capacityUsageHeaderStrip.TabIndex = 10;
            // 
            // capacityUsageChartSelectionDropDownButton
            // 
            this.capacityUsageChartSelectionDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCapacityUsageInMegabytesButton,
            this.showCapacityUsageInPercentButton,
            this.showCapacityUsageLogInMegabytesButton,
            this.showCapacityUsageLogInPercent});
            this.capacityUsageChartSelectionDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.capacityUsageChartSelectionDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.capacityUsageChartSelectionDropDownButton.Name = "capacityUsageChartSelectionDropDownButton";
            this.capacityUsageChartSelectionDropDownButton.Size = new System.Drawing.Size(252, 16);
            this.capacityUsageChartSelectionDropDownButton.Text = "Current Capacity Usage: Data Megabytes";
            // 
            // showCapacityUsageInMegabytesButton
            // 
            this.showCapacityUsageInMegabytesButton.Name = "showCapacityUsageInMegabytesButton";
            this.showCapacityUsageInMegabytesButton.Size = new System.Drawing.Size(167, 22);
            this.showCapacityUsageInMegabytesButton.Text = "Data Megabytes";
            this.showCapacityUsageInMegabytesButton.Click += new System.EventHandler(this.showCapacityUsageInMegabytesButton_Click);
            // 
            // showCapacityUsageInPercentButton
            // 
            this.showCapacityUsageInPercentButton.Name = "showCapacityUsageInPercentButton";
            this.showCapacityUsageInPercentButton.Size = new System.Drawing.Size(167, 22);
            this.showCapacityUsageInPercentButton.Text = "Data Percent";
            this.showCapacityUsageInPercentButton.Click += new System.EventHandler(this.showCapacityUsageInPercentButton_Click);
            // 
            // showCapacityUsageLogInMegabytesButton
            // 
            this.showCapacityUsageLogInMegabytesButton.Name = "showCapacityUsageLogInMegabytesButton";
            this.showCapacityUsageLogInMegabytesButton.Size = new System.Drawing.Size(167, 22);
            this.showCapacityUsageLogInMegabytesButton.Text = "Log Megabytes";
            this.showCapacityUsageLogInMegabytesButton.Click += new System.EventHandler(this.showCapacityUsageLogInMegabytesButton_Click);
            // 
            // showCapacityUsageLogInPercent
            // 
            this.showCapacityUsageLogInPercent.Name = "showCapacityUsageLogInPercent";
            this.showCapacityUsageLogInPercent.Size = new System.Drawing.Size(167, 22);
            this.showCapacityUsageLogInPercent.Text = "Log Percent";
            this.showCapacityUsageLogInPercent.Click += new System.EventHandler(this.showCapacityUsageLogInPercentButton_Click);
            // 
            // maximizeCapacityUsageChartButton
            // 
            this.maximizeCapacityUsageChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeCapacityUsageChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeCapacityUsageChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeCapacityUsageChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeCapacityUsageChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeCapacityUsageChartButton.Name = "maximizeCapacityUsageChartButton";
            this.maximizeCapacityUsageChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeCapacityUsageChartButton.ToolTipText = "Maximize";
            this.maximizeCapacityUsageChartButton.Click += new System.EventHandler(this.maximizeCapacityUsageChartButton_Click);
            // 
            // restoreCapacityUsageChartButton
            // 
            this.restoreCapacityUsageChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreCapacityUsageChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreCapacityUsageChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreCapacityUsageChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreCapacityUsageChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreCapacityUsageChartButton.Name = "restoreCapacityUsageChartButton";
            this.restoreCapacityUsageChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreCapacityUsageChartButton.Text = "Restore";
            this.restoreCapacityUsageChartButton.Visible = false;
            this.restoreCapacityUsageChartButton.Click += new System.EventHandler(this.restoreCapacityUsageChartButton_Click);
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
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
            appearance38.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance38;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance39.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance39;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance40.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance40;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance41.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance41;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.SharedPropsInternal.Caption = "Toolbar";
            appearance42.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance42;
            buttonTool11.SharedPropsInternal.Caption = "Print";
            appearance43.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance43;
            buttonTool12.SharedPropsInternal.Caption = "Print";
            appearance44.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance44;
            buttonTool13.SharedPropsInternal.Caption = "Export to Excel";
            appearance45.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance45;
            buttonTool14.SharedPropsInternal.Caption = "Export to Excel (csv)";
            appearance46.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ExportChartImage16x16;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance46;
            buttonTool15.SharedPropsInternal.Caption = "Save Image";
            popupMenuTool2.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool16.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool4,
            buttonTool16,
            buttonTool17,
            buttonTool18});
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22});
            popupMenuTool4.SharedPropsInternal.Caption = "gridDataContextMenu";
            buttonTool26.InstanceProps.IsFirstInGroup = true;
            buttonTool28.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            buttonTool29});
            buttonTool30.SharedPropsInternal.Caption = "View Backups && Restores";
            buttonTool31.SharedPropsInternal.Caption = "View Files";
            buttonTool32.SharedPropsInternal.Caption = "View Oldest Open Transaction";
            buttonTool33.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool34.SharedPropsInternal.Caption = "Expand All Groups";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            stateButtonTool3,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            popupMenuTool2,
            popupMenuTool3,
            popupMenuTool4,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            buttonTool33,
            buttonTool34});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // DatabasesSummaryView_Fill_Panel
            // 
            this.DatabasesSummaryView_Fill_Panel.Controls.Add(this.contentContainerPanel);
            this.DatabasesSummaryView_Fill_Panel.Controls.Add(this.tableLayoutPanel1);
            this.DatabasesSummaryView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DatabasesSummaryView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabasesSummaryView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.DatabasesSummaryView_Fill_Panel.Name = "DatabasesSummaryView_Fill_Panel";
            this.DatabasesSummaryView_Fill_Panel.Size = new System.Drawing.Size(689, 517);
            this.DatabasesSummaryView_Fill_Panel.TabIndex = 9;
            // 
            // contentContainerPanel
            // 
            this.contentContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentContainerPanel.Controls.Add(this.splitContainer);
            this.contentContainerPanel.Location = new System.Drawing.Point(0, 30);
            this.contentContainerPanel.Name = "contentContainerPanel";
            this.contentContainerPanel.Size = new System.Drawing.Size(689, 487);
            this.contentContainerPanel.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Controls.Add(this.refreshDatabasesButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.databasesFilterComboBox, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(689, 32);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // refreshDatabasesButton
            //
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
            this.refreshDatabasesButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            if (!refreshDatabasesButton.Enabled)
                appearance1.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
            else
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.refreshDatabasesButton.Appearance = appearance1;
            this.refreshDatabasesButton.Location = new System.Drawing.Point(659, 3);
            this.refreshDatabasesButton.Name = "refreshDatabasesButton";
            this.refreshDatabasesButton.ShowFocusRect = false;
            this.refreshDatabasesButton.ShowOutline = false;
            this.refreshDatabasesButton.Size = new System.Drawing.Size(27, 23);
            this.refreshDatabasesButton.TabIndex = 17;
            this.refreshDatabasesButton.Click += new System.EventHandler(this.refreshDatabasesButton_Click);
            this.refreshDatabasesButton.MouseEnterElement += new UIElementEventHandler(mouseEnter_refreshDatabasesButton);
            this.refreshDatabasesButton.MouseLeaveElement += new UIElementEventHandler(mouseLeave_refreshDatabasesButton);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            //Event Methods
            
            // 
            // databasesFilterComboBox
            // 
            this.databasesFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesFilterComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.databasesFilterComboBox.Location = new System.Drawing.Point(3, 3);
            this.databasesFilterComboBox.Name = "databasesFilterComboBox";
            this.databasesFilterComboBox.Size = new System.Drawing.Size(650, 21);
            this.databasesFilterComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.databasesFilterComboBox.TabIndex = 6;
            this.databasesFilterComboBox.SelectionChanged += new System.EventHandler(this.databasesFilterComboBox_SelectionChanged);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Databases Summary";
            this.ultraGridPrintDocument.Grid = this.databasesGrid;
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(689, 517);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 32;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
                "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // DatabasesSummaryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.DatabasesSummaryView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "DatabasesSummaryView";
            this.Size = new System.Drawing.Size(689, 517);
            this.Load += new System.EventHandler(this.DatabasesSummaryView_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.databasesGrid)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.recentTrendsPanel.ResumeLayout(false);
            this.recentTrendsChartContainerPanel.ResumeLayout(false);
            this.recentTrendsChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentTrendsChart)).EndInit();
            this.recentTrendsHeaderStrip.ResumeLayout(false);
            this.recentTrendsHeaderStrip.PerformLayout();
            this.capacityUsagePanel.ResumeLayout(false);
            this.capacityUsageChartContainerPanel.ResumeLayout(false);
            this.capacityUsageChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.capacityUsageChart)).EndInit();
            this.capacityUsageHeaderStrip.ResumeLayout(false);
            this.capacityUsageHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.DatabasesSummaryView_Fill_Panel.ResumeLayout(false);
            this.contentContainerPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databasesFilterComboBox)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  recentTrendsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  recentTrendsChartContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel recentTrendsChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip recentTrendsHeaderStrip;
        private System.Windows.Forms.ToolStripButton maximizeRecentTrendsChartButton;
        private System.Windows.Forms.ToolStripButton restoreRecentTrendsChartButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  capacityUsagePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  capacityUsageChartContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel capacityUsageChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip capacityUsageHeaderStrip;
        private System.Windows.Forms.ToolStripButton maximizeCapacityUsageChartButton;
        private System.Windows.Forms.ToolStripButton restoreCapacityUsageChartButton;
        private System.Windows.Forms.ToolStripDropDownButton recentTrendsChartSelectionDropDownButton;
        private System.Windows.Forms.ToolStripDropDownButton capacityUsageChartSelectionDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem showCapacityUsageInMegabytesButton;
        private System.Windows.Forms.ToolStripMenuItem showActiveSessionsTrendButton;
        private System.Windows.Forms.ToolStripMenuItem showDataSizeTrendButton;
        private System.Windows.Forms.ToolStripMenuItem showLogSizeTrendButton;
        private System.Windows.Forms.ToolStripMenuItem showCapacityUsageInPercentButton;
        private ChartFX.WinForms.Chart capacityUsageChart;
        private ChartFX.WinForms.Chart recentTrendsChart;
        private System.Windows.Forms.ToolStripMenuItem showTransactionsPerSecondTrendButton;
        private System.Windows.Forms.ToolStripMenuItem showLogFlushesTrendButton;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  DatabasesSummaryView_Fill_Panel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databasesGridStatusLabel;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private System.Windows.Forms.ToolStripMenuItem showCapacityUsageLogInMegabytesButton;
        private System.Windows.Forms.ToolStripMenuItem showCapacityUsageLogInPercent;
        private Infragistics.Win.UltraWinGrid.UltraGrid databasesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.Misc.UltraButton refreshDatabasesButton;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor databasesFilterComboBox;
        private Infragistics.Win.Appearance appearance2;
        private Infragistics.Win.Appearance appearance1;

    }
}
