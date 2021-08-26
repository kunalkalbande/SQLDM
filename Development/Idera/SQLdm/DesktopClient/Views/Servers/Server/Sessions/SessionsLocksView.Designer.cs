using Idera.SQLdm.DesktopClient.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    partial class SessionsLocksView
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
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("viewDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Controls.CustomControls.CustomButtonTool("viewQueryHistoryButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("traceSessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("killSessionButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("traceSessionButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("viewDetailsButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("killSessionButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Controls.CustomControls.CustomButtonTool("viewQueryHistoryButton");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Session ID", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Object");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Object Schema");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Object Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Instances");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Request Mode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Request Type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Request Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Blocked");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Blocked By");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Blocking");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("User");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Host");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Application");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsSystem");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Wait Time");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Command");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Session ID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Object");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Database");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Object Schema");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Object Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Instances");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Request Mode");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Request Type");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Request Status");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Blocked");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Blocked By");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Blocking");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("User");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Host");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn15 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Application");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn16 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("IsSystem");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn17 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Wait Time");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn18 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Command");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionsLocksView));
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.lockStatisticsChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.SessionsLocksView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.locksGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.locksGridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.locksGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lockStatisticsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.lockStatisticsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lockStatisticsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.closeLockStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.maximizeLockStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.lockStatisticsOptionsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.averageWaitTimeToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.deadlocksToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.requestsToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.timeoutsToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.waitsToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.waitTimeToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.restoreLockStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.operationalStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lockStatisticsChart)).BeginInit();
            this.SessionsLocksView_Fill_Panel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.locksGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locksGridDataSource)).BeginInit();
            this.lockStatisticsPanel.SuspendLayout();
            this.lockStatisticsHeaderStrip.SuspendLayout();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            this.SuspendLayout();
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
            appearance12.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            popupMenuTool2.SharedPropsInternal.Caption = "gridDataContextMenu";
            buttonTool14.InstanceProps.IsFirstInGroup = true;
            buttonTool16.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11,
            buttonTool37,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool17});
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StartTrace;
            buttonTool18.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool18.SharedPropsInternal.Caption = "Trace Session";
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SessionsDetailsViewSmall;
            buttonTool19.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool19.SharedPropsInternal.Caption = "View Session Details";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.KillProcess;
            buttonTool20.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool20.SharedPropsInternal.Caption = "Kill Session";
            popupMenuTool3.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool2,
            buttonTool21,
            buttonTool22,
            buttonTool23});
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool24.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool24.SharedPropsInternal.Caption = "Print";
            appearance20.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool25.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool25.SharedPropsInternal.Caption = "Export to Excel";
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool26.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool26.SharedPropsInternal.Caption = "Print";
            appearance22.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool27.SharedPropsInternal.AppearancesSmall.Appearance = appearance22;
            buttonTool27.SharedPropsInternal.Caption = "Export to Excel (csv)";
            appearance23.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ExportChartImage16x16;
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool28.SharedPropsInternal.Caption = "Save Image";
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.SharedPropsInternal.Caption = "Toolbar";
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool4.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool4.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool31.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool29,
            buttonTool30,
            buttonTool31,
            buttonTool32});
            buttonTool33.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool34.SharedPropsInternal.Caption = "Expand All Groups";
            appearance34.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.QueryHistory16;
            buttonTool35.SharedPropsInternal.AppearancesSmall.Appearance = appearance34;
            buttonTool35.SharedPropsInternal.Caption = "Show Query History";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            popupMenuTool2,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            popupMenuTool3,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28,
            stateButtonTool3,
            stateButtonTool4,
            popupMenuTool4,
            buttonTool33,
            buttonTool34,
            buttonTool35});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // lockStatisticsChart
            // 
            this.lockStatisticsChart.AllSeries.MarkerSize = ((short)(2));
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.lockStatisticsChart.Background = gradientBackground1;
            this.lockStatisticsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.lockStatisticsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.lockStatisticsChart, "chartContextMenu");
            this.lockStatisticsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsChart.LegendBox.LineSpacing = 1D;
            this.lockStatisticsChart.Location = new System.Drawing.Point(0, 19);
            this.lockStatisticsChart.Name = "lockStatisticsChart";
            this.lockStatisticsChart.Palette = "Schemes.Classic";
            this.lockStatisticsChart.PlotAreaColor = System.Drawing.Color.White;
            this.lockStatisticsChart.RandomData.Series = 13;
            this.lockStatisticsChart.Size = new System.Drawing.Size(625, 213);
            this.lockStatisticsChart.TabIndex = 16;
            this.lockStatisticsChart.Visible = false;
            // 
            // SessionsLocksView_Fill_Panel
            // 
            this.SessionsLocksView_Fill_Panel.Controls.Add(this.contentPanel);
            this.SessionsLocksView_Fill_Panel.Controls.Add(this.operationalStatusPanel);
            this.SessionsLocksView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.SessionsLocksView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SessionsLocksView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.SessionsLocksView_Fill_Panel.Name = "SessionsLocksView_Fill_Panel";
            this.SessionsLocksView_Fill_Panel.Size = new System.Drawing.Size(625, 478);
            this.SessionsLocksView_Fill_Panel.TabIndex = 8;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.splitContainer);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 27);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(625, 451);
            this.contentPanel.TabIndex = 18;
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
            this.splitContainer.Panel1.Controls.Add(this.locksGrid);
            this.splitContainer.Panel1.Controls.Add(this.locksGridStatusLabel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.lockStatisticsPanel);
            this.splitContainer.Size = new System.Drawing.Size(625, 451);
            this.splitContainer.SplitterDistance = 215;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // locksGrid
            // 
            this.locksGrid.DataSource = this.locksGridDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.locksGrid.DisplayLayout.Appearance = appearance1;
            this.locksGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 195;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.Caption = "Lock Count";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Header.VisiblePosition = 8;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Header.VisiblePosition = 9;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Header.VisiblePosition = 10;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Header.VisiblePosition = 11;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Header.VisiblePosition = 12;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.Header.VisiblePosition = 13;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.Header.VisiblePosition = 14;
            ultraGridColumn15.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn15.Header.VisiblePosition = 15;
            ultraGridColumn16.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn16.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn16.Header.VisiblePosition = 16;
            ultraGridColumn16.Hidden = true;
            ultraGridColumn17.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            appearance2.TextHAlignAsString = "Right";
            ultraGridColumn17.CellAppearance = appearance2;
            ultraGridColumn17.Header.Caption = "Wait Time (ms)";
            ultraGridColumn17.Header.VisiblePosition = 6;
            ultraGridColumn18.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn18.Header.VisiblePosition = 17;
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
            ultraGridColumn18});
            this.locksGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.locksGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.locksGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.locksGrid.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.locksGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.locksGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.locksGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.locksGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.locksGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.locksGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.locksGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.locksGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.locksGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.locksGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.locksGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.locksGrid.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.locksGrid.DisplayLayout.Override.CellAppearance = appearance7;
            this.locksGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.locksGrid.DisplayLayout.Override.CellPadding = 0;
            this.locksGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.locksGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.locksGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.locksGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.locksGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.locksGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.locksGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.locksGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.locksGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.locksGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.locksGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.locksGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.locksGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.locksGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.locksGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.locksGrid.DisplayLayout.UseFixedHeaders = true;
            this.locksGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.locksGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.locksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locksGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locksGrid.Location = new System.Drawing.Point(0, 0);
            this.locksGrid.Name = "locksGrid";
            this.locksGrid.Size = new System.Drawing.Size(625, 214);
            this.locksGrid.TabIndex = 3;
            this.locksGrid.Visible = false;
            this.locksGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.locksGrid_AfterSelectChange);
            this.locksGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.locksGrid_MouseDown);
            // 
            // locksGridDataSource
            // 
            ultraDataColumn1.DataType = typeof(int);
            ultraDataColumn6.DataType = typeof(long);
            ultraDataColumn16.DataType = typeof(bool);
            ultraDataColumn17.DataType = typeof(decimal);
            this.locksGridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11,
            ultraDataColumn12,
            ultraDataColumn13,
            ultraDataColumn14,
            ultraDataColumn15,
            ultraDataColumn16,
            ultraDataColumn17,
            ultraDataColumn18});
            // 
            // locksGridStatusLabel
            // 
            this.locksGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.locksGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locksGridStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.locksGridStatusLabel.Name = "locksGridStatusLabel";
            this.locksGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.locksGridStatusLabel.Size = new System.Drawing.Size(625, 214);
            this.locksGridStatusLabel.TabIndex = 4;
            this.locksGridStatusLabel.Text = "< Status Message >";
            this.locksGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lockStatisticsPanel
            // 
            this.lockStatisticsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.lockStatisticsPanel.Controls.Add(this.lockStatisticsChart);
            this.lockStatisticsPanel.Controls.Add(this.lockStatisticsStatusLabel);
            this.lockStatisticsPanel.Controls.Add(this.lockStatisticsHeaderStrip);
            this.lockStatisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsPanel.Location = new System.Drawing.Point(0, 0);
            this.lockStatisticsPanel.Name = "lockStatisticsPanel";
            this.lockStatisticsPanel.Size = new System.Drawing.Size(625, 232);
            this.lockStatisticsPanel.TabIndex = 0;
            // 
            // lockStatisticsStatusLabel
            // 
            this.lockStatisticsStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.lockStatisticsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.lockStatisticsStatusLabel.Name = "lockStatisticsStatusLabel";
            this.lockStatisticsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lockStatisticsStatusLabel.Size = new System.Drawing.Size(625, 213);
            this.lockStatisticsStatusLabel.TabIndex = 15;
            this.lockStatisticsStatusLabel.Text = "< Status Message >";
            this.lockStatisticsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lockStatisticsHeaderStrip
            // 
            this.lockStatisticsHeaderStrip.AutoSize = false;
            this.lockStatisticsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lockStatisticsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.lockStatisticsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.lockStatisticsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.lockStatisticsHeaderStrip.HotTrackEnabled = false;
            this.lockStatisticsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeLockStatisticsChartButton,
            this.maximizeLockStatisticsChartButton,
            this.lockStatisticsOptionsButton,
            this.restoreLockStatisticsChartButton});
            this.lockStatisticsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.lockStatisticsHeaderStrip.Name = "lockStatisticsHeaderStrip";
            this.lockStatisticsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lockStatisticsHeaderStrip.Size = new System.Drawing.Size(625, 19);
            this.lockStatisticsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.lockStatisticsHeaderStrip.TabIndex = 13;
            // 
            // closeLockStatisticsChartButton
            // 
            this.closeLockStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeLockStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeLockStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.closeLockStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeLockStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeLockStatisticsChartButton.Name = "closeLockStatisticsChartButton";
            this.closeLockStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.closeLockStatisticsChartButton.ToolTipText = "Close";
            this.closeLockStatisticsChartButton.Click += new System.EventHandler(this.closeLockStatisticsChartButton_Click);
            // 
            // maximizeLockStatisticsChartButton
            // 
            this.maximizeLockStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeLockStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeLockStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeLockStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeLockStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeLockStatisticsChartButton.Name = "maximizeLockStatisticsChartButton";
            this.maximizeLockStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeLockStatisticsChartButton.ToolTipText = "Maximize";
            this.maximizeLockStatisticsChartButton.Click += new System.EventHandler(this.maximizeLockStatisticsChartButton_Click);
            // 
            // lockStatisticsOptionsButton
            // 
            this.lockStatisticsOptionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lockStatisticsOptionsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.averageWaitTimeToolStripMenuItem,
            this.deadlocksToolStripMenuItem,
            this.requestsToolStripMenuItem,
            this.timeoutsToolStripMenuItem,
            this.waitsToolStripMenuItem,
            this.waitTimeToolStripMenuItem});
            this.lockStatisticsOptionsButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lockStatisticsOptionsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.lockStatisticsOptionsButton.Image = ((System.Drawing.Image)(resources.GetObject("lockStatisticsOptionsButton.Image")));
            this.lockStatisticsOptionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lockStatisticsOptionsButton.Name = "lockStatisticsOptionsButton";
            this.lockStatisticsOptionsButton.Size = new System.Drawing.Size(161, 16);
            this.lockStatisticsOptionsButton.Text = "Lock Statistics: Requests";
            // 
            // averageWaitTimeToolStripMenuItem
            // 
            this.averageWaitTimeToolStripMenuItem.Name = "averageWaitTimeToolStripMenuItem";
            this.averageWaitTimeToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.averageWaitTimeToolStripMenuItem.Text = "Average Wait Time";
            this.averageWaitTimeToolStripMenuItem.Click += new System.EventHandler(this.averageWaitTimeToolStripMenuItem_Click);
            // 
            // deadlocksToolStripMenuItem
            // 
            this.deadlocksToolStripMenuItem.Name = "deadlocksToolStripMenuItem";
            this.deadlocksToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.deadlocksToolStripMenuItem.Text = "Deadlocks";
            this.deadlocksToolStripMenuItem.Click += new System.EventHandler(this.deadlocksToolStripMenuItem_Click);
            // 
            // requestsToolStripMenuItem
            // 
            this.requestsToolStripMenuItem.Name = "requestsToolStripMenuItem";
            this.requestsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.requestsToolStripMenuItem.Text = "Requests";
            this.requestsToolStripMenuItem.Click += new System.EventHandler(this.requestsToolStripMenuItem_Click);
            // 
            // timeoutsToolStripMenuItem
            // 
            this.timeoutsToolStripMenuItem.Name = "timeoutsToolStripMenuItem";
            this.timeoutsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.timeoutsToolStripMenuItem.Text = "Timeouts";
            this.timeoutsToolStripMenuItem.Click += new System.EventHandler(this.timeoutsToolStripMenuItem_Click);
            // 
            // waitsToolStripMenuItem
            // 
            this.waitsToolStripMenuItem.Name = "waitsToolStripMenuItem";
            this.waitsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.waitsToolStripMenuItem.Text = "Waits";
            this.waitsToolStripMenuItem.Click += new System.EventHandler(this.waitsToolStripMenuItem_Click);
            // 
            // waitTimeToolStripMenuItem
            // 
            this.waitTimeToolStripMenuItem.Name = "waitTimeToolStripMenuItem";
            this.waitTimeToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.waitTimeToolStripMenuItem.Text = "Wait Time";
            this.waitTimeToolStripMenuItem.Click += new System.EventHandler(this.waitTimeToolStripMenuItem_Click);
            // 
            // restoreLockStatisticsChartButton
            // 
            this.restoreLockStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreLockStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreLockStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreLockStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreLockStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreLockStatisticsChartButton.Name = "restoreLockStatisticsChartButton";
            this.restoreLockStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreLockStatisticsChartButton.Text = "Restore";
            this.restoreLockStatisticsChartButton.Visible = false;
            this.restoreLockStatisticsChartButton.Click += new System.EventHandler(this.restoreLockStatisticsChartButton_Click);
            // 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(625, 27);
            this.operationalStatusPanel.TabIndex = 17;
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
            this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.operationalStatusLabel.Size = new System.Drawing.Size(617, 21);
            this.operationalStatusLabel.TabIndex = 2;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Session Locks";
            this.ultraGridPrintDocument.Grid = this.locksGrid;
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
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(625, 478);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 18;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "Data for the selected historical snapshot does not exist for this view. Select an" +
    "other historical snapshot or click here to switch to real-time mode.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // SessionsLocksView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.SessionsLocksView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "SessionsLocksView";
            this.Size = new System.Drawing.Size(625, 478);
            this.Load += new System.EventHandler(this.SessionsDetailsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lockStatisticsChart)).EndInit();
            this.SessionsLocksView_Fill_Panel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.locksGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locksGridDataSource)).EndInit();
            this.lockStatisticsPanel.ResumeLayout(false);
            this.lockStatisticsPanel.PerformLayout();
            this.lockStatisticsHeaderStrip.ResumeLayout(false);
            this.lockStatisticsHeaderStrip.PerformLayout();
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.ResumeLayout(false);            
        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  SessionsLocksView_Fill_Panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinGrid.UltraGrid locksGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  lockStatisticsPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip lockStatisticsHeaderStrip;
        private System.Windows.Forms.ToolStripButton closeLockStatisticsChartButton;
        private System.Windows.Forms.ToolStripButton maximizeLockStatisticsChartButton;
        private System.Windows.Forms.ToolStripDropDownButton lockStatisticsOptionsButton;
        private System.Windows.Forms.ToolStripMenuItem averageWaitTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deadlocksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeoutsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton restoreLockStatisticsChartButton;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource locksGridDataSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel locksGridStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lockStatisticsStatusLabel;
        private ChartFX.WinForms.Chart lockStatisticsChart;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel operationalStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentPanel;
    }
}
