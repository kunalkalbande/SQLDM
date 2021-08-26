using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    partial class ResourcesDiskSizeView
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
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
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
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Disk Name", 0, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Used (MB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Free Disk (MB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQL Data Used (MB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQL Data Free (MB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQL Log (MB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Other Files (MB)");
            
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Used");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Free Disk");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% SQL Data Used");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% SQL Data Free");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% SQL Log");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Other Files");

            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Total Size (MB)");
            
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            ChartFX.WinForms.SeriesAttributes seriesAttributes9 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes10 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes11 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes12 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes13 = new ChartFX.WinForms.SeriesAttributes();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Disk Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Total Size (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Used (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Free Disk (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SQL Data Used (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SQL Data Free (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SQL Log (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Other Files (MB)");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% Used");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% Free Disk");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% SQL Data Used");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% SQL Data Free");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% SQL Log");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("% Other Files");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourcesDiskSizeView));
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.driveStatisticsChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.drivesView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.drivesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.drivesGridDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.drivesGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.driveStatisticsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.driveStatisticsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.driveStatisticsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.closeDriveStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.maximizeDriveStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.driveStatisticsOptionsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.MegaBytesToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.PercentageToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.restoreDiskDriveStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.operationalStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.driveStatisticsChart)).BeginInit();
            this.drivesView_Fill_Panel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drivesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.drivesGridDataSource)).BeginInit();
            this.driveStatisticsPanel.SuspendLayout();
            this.driveStatisticsHeaderStrip.SuspendLayout();
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
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool17});
            popupMenuTool3.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
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
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            popupMenuTool2,
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
            buttonTool34});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // lockStatisticsChart
            // 
            this.driveStatisticsChart.AllSeries.BarShape = ChartFX.WinForms.BarShape.Cylinder;
            this.driveStatisticsChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Gantt;
            this.driveStatisticsChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            this.driveStatisticsChart.AllSeries.MarkerSize = ((short)(2));
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.driveStatisticsChart.Background = gradientBackground1;
            this.driveStatisticsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.driveStatisticsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.driveStatisticsChart, "chartContextMenu");
            this.driveStatisticsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            // this.driveStatisticsChart.LegendBox.LineSpacing = 1D;
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            this.driveStatisticsChart.Location = new System.Drawing.Point(0, 19);
            this.driveStatisticsChart.Name = "driveStatisticsChart";
            this.driveStatisticsChart.Palette = "Schemes.Classic";
            this.driveStatisticsChart.PlotAreaColor = System.Drawing.Color.White;
            this.driveStatisticsChart.RandomData.Series = 5;
            seriesAttributes9.Text = "SQL Data Used";
            seriesAttributes10.Text = "SQL Data Free";
            seriesAttributes11.Text = "SQL Log";
            seriesAttributes12.Text = "Other Files";
            seriesAttributes13.Text = "Free Disk";
            this.driveStatisticsChart.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes9,
            seriesAttributes10,
            seriesAttributes11,
            seriesAttributes12,
            seriesAttributes13});
            this.driveStatisticsChart.Size = new System.Drawing.Size(625, 213);
            this.driveStatisticsChart.TabIndex = 16;
            this.driveStatisticsChart.Visible = false;
            // 
            // SessionsLocksView_Fill_Panel
            // 
            this.drivesView_Fill_Panel.Controls.Add(this.contentPanel);
            this.drivesView_Fill_Panel.Controls.Add(this.operationalStatusPanel);
            this.drivesView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.drivesView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drivesView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.drivesView_Fill_Panel.Name = "drivesView_Fill_Panel";
            this.drivesView_Fill_Panel.Size = new System.Drawing.Size(625, 478);
            this.drivesView_Fill_Panel.TabIndex = 8;
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
            this.splitContainer.Panel1.Controls.Add(this.drivesGrid);
            this.splitContainer.Panel1.Controls.Add(this.drivesGridStatusLabel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.driveStatisticsPanel);
            this.splitContainer.Size = new System.Drawing.Size(625, 451);
            this.splitContainer.SplitterDistance = 215;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // drivesGrid
            // 
            this.drivesGrid.DataSource = this.drivesGridDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.drivesGrid.DisplayLayout.Appearance = appearance1;
            this.drivesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 195;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn8.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn11.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn12.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn13.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn14.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn14.Header.VisiblePosition = 1;
            
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn14,
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
            ultraGridColumn13});
            this.drivesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.drivesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.drivesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.drivesGrid.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.drivesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.drivesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.drivesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.drivesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.drivesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.drivesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.drivesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.drivesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.drivesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.drivesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.drivesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.Color.DarkRed;
            this.drivesGrid.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Black;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.drivesGrid.DisplayLayout.Override.CellAppearance = appearance7;
            this.drivesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.drivesGrid.DisplayLayout.Override.CellPadding = 0;
            this.drivesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.drivesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.Color.IndianRed;
            appearance8.BackColor2 = System.Drawing.Color.IndianRed;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.drivesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.drivesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.drivesGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.drivesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance10.BackColor = System.Drawing.Color.PaleVioletRed;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.drivesGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.drivesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.drivesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.drivesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.drivesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.drivesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance11.BackColor2 = System.Drawing.Color.OrangeRed;
            this.drivesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.drivesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.drivesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.drivesGrid.DisplayLayout.UseFixedHeaders = true;
            this.drivesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.drivesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.drivesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drivesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drivesGrid.Location = new System.Drawing.Point(0, 0);
            this.drivesGrid.Name = "drivesGrid";
            this.drivesGrid.Size = new System.Drawing.Size(625, 214);
            this.drivesGrid.TabIndex = 3;
            this.drivesGrid.Visible = false;
            this.drivesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.drivesGrid_AfterSelectChange);
            this.drivesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drivesGrid_MouseDown);
            this.drivesGrid.DisplayLayout.Override.RowAppearance = appearance11;
            // 
            // drivesGridDataSource
            // 
            //ultraDataColumn1.DataType = typeof(string);
            //ultraDataColumn6.DataType = typeof(double?);
            this.drivesGridDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn14,
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
            ultraDataColumn13});
            // 
            // drivesGridStatusLabel
            // 
            this.drivesGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.drivesGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drivesGridStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.drivesGridStatusLabel.Name = "drivesGridStatusLabel";
            this.drivesGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.drivesGridStatusLabel.Size = new System.Drawing.Size(625, 214);
            this.drivesGridStatusLabel.TabIndex = 4;
            this.drivesGridStatusLabel.Text = "< Status Message >";
            this.drivesGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lockStatisticsPanel
            // 
            this.driveStatisticsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.driveStatisticsPanel.Controls.Add(this.driveStatisticsChart);
            this.driveStatisticsPanel.Controls.Add(this.driveStatisticsStatusLabel);
            this.driveStatisticsPanel.Controls.Add(this.driveStatisticsHeaderStrip);
            this.driveStatisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.driveStatisticsPanel.Location = new System.Drawing.Point(0, 0);
            this.driveStatisticsPanel.Name = "driveStatisticsPanel";
            this.driveStatisticsPanel.Size = new System.Drawing.Size(625, 232);
            this.driveStatisticsPanel.TabIndex = 0;
            // 
            // lockStatisticsStatusLabel
            // 
            this.driveStatisticsStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.driveStatisticsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.driveStatisticsStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.driveStatisticsStatusLabel.Name = "driveStatisticsStatusLabel";
            this.driveStatisticsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.driveStatisticsStatusLabel.Size = new System.Drawing.Size(625, 213);
            this.driveStatisticsStatusLabel.TabIndex = 15;
            this.driveStatisticsStatusLabel.Text = "< Status Message >";
            this.driveStatisticsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lockStatisticsHeaderStrip
            // 
            this.driveStatisticsHeaderStrip.AutoSize = false;
            this.driveStatisticsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.driveStatisticsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.driveStatisticsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.driveStatisticsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.driveStatisticsHeaderStrip.HotTrackEnabled = false;
            this.driveStatisticsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeDriveStatisticsChartButton,
            this.maximizeDriveStatisticsChartButton,
            this.driveStatisticsOptionsButton,
            this.restoreDiskDriveStatisticsChartButton});
            this.driveStatisticsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.driveStatisticsHeaderStrip.Name = "driveStatisticsHeaderStrip";
            this.driveStatisticsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.driveStatisticsHeaderStrip.Size = new System.Drawing.Size(625, 19);
            this.driveStatisticsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.driveStatisticsHeaderStrip.TabIndex = 13;
            // 
            // closeLockStatisticsChartButton
            // 
            this.closeDriveStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeDriveStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeDriveStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.closeDriveStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeDriveStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeDriveStatisticsChartButton.Name = "closeLockStatisticsChartButton";
            this.closeDriveStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.closeDriveStatisticsChartButton.ToolTipText = "Close";
            this.closeDriveStatisticsChartButton.Click += new System.EventHandler(this.closeDiskDriveStatisticsChartButton_Click);
            // 
            // maximizeLockStatisticsChartButton
            // 
            this.maximizeDriveStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeDriveStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeDriveStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeDriveStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeDriveStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeDriveStatisticsChartButton.Name = "maximizeDriveStatisticsChartButton";
            this.maximizeDriveStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeDriveStatisticsChartButton.ToolTipText = "Maximize";
            this.maximizeDriveStatisticsChartButton.Click += new System.EventHandler(this.maximizeDiskDriveStatisticsChartButton_Click);
            // 
            // driveStatisticsOptionsButton
            // 
            this.driveStatisticsOptionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.driveStatisticsOptionsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MegaBytesToolStripMenuItem,
            this.PercentageToolStripMenuItem});
            this.driveStatisticsOptionsButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.driveStatisticsOptionsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.driveStatisticsOptionsButton.Name = "driveStatisticsOptionsButton";
            this.driveStatisticsOptionsButton.Size = new System.Drawing.Size(161, 16);
            this.driveStatisticsOptionsButton.Text = "Drive Statistics: Megabytes";
            
            // 
            // deadlocksToolStripMenuItem
            // 
            this.MegaBytesToolStripMenuItem.Name = "MegaBytesToolStripMenuItem";
            this.MegaBytesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.MegaBytesToolStripMenuItem.Text = "Megabytes";
            this.MegaBytesToolStripMenuItem.Click += new System.EventHandler(this.megabytesToolStripMenuItem_Click);
            // 
            // requestsToolStripMenuItem
            // 
            this.PercentageToolStripMenuItem.Name = "PercentageToolStripMenuItem";
            this.PercentageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.PercentageToolStripMenuItem.Text = "Percentage";
            this.PercentageToolStripMenuItem.Click += new System.EventHandler(this.percentageToolStripMenuItem_Click);
            // 
            // restoreLockStatisticsChartButton
            // 
            this.restoreDiskDriveStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreDiskDriveStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreDiskDriveStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreDiskDriveStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreDiskDriveStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreDiskDriveStatisticsChartButton.Name = "restoreLockStatisticsChartButton";
            this.restoreDiskDriveStatisticsChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreDiskDriveStatisticsChartButton.Text = "Restore";
            this.restoreDiskDriveStatisticsChartButton.Visible = false;
            this.restoreDiskDriveStatisticsChartButton.Click += new System.EventHandler(this.restoreDiskDriveStatisticsChartButton_Click);
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
            this.ultraGridPrintDocument.DocumentName = "disk size details";
            this.ultraGridPrintDocument.Grid = this.drivesGrid;
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
            this.Controls.Add(this.drivesView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "ResourcesDiskSizeView";
            this.Size = new System.Drawing.Size(625, 478);
            this.Load += new System.EventHandler(this.ResourcesDiskSizeView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.driveStatisticsChart)).EndInit();
            this.drivesView_Fill_Panel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.drivesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.drivesGridDataSource)).EndInit();
            this.driveStatisticsPanel.ResumeLayout(false);
            this.driveStatisticsPanel.PerformLayout();
            this.driveStatisticsHeaderStrip.ResumeLayout(false);
            this.driveStatisticsHeaderStrip.PerformLayout();
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private void ScaleControlsAsPerResolution()
        {
            driveStatisticsChart.LegendBox.AutoSize = true;
        }
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  drivesView_Fill_Panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinGrid.UltraGrid drivesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  driveStatisticsPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip driveStatisticsHeaderStrip;
        private System.Windows.Forms.ToolStripButton closeDriveStatisticsChartButton;
        private System.Windows.Forms.ToolStripButton maximizeDriveStatisticsChartButton;
        private System.Windows.Forms.ToolStripDropDownButton driveStatisticsOptionsButton;
        private System.Windows.Forms.ToolStripMenuItem MegaBytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PercentageToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton restoreDiskDriveStatisticsChartButton;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource drivesGridDataSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel drivesGridStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel driveStatisticsStatusLabel;
        private ChartFX.WinForms.Chart driveStatisticsChart;
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
