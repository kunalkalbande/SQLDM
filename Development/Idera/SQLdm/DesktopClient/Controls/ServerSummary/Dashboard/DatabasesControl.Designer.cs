using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class DatabasesControl
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
            ChartFX.WinForms.Adornments.SolidBackground solidBackground1 = new ChartFX.WinForms.Adornments.SolidBackground();
            ChartFX.WinForms.SeriesAttributes seriesAttributes1 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes2 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes3 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes4 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes5 = new ChartFX.WinForms.SeriesAttributes();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DatabaseName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("TransactionsPerSec");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("LogFlushesPerSec");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("NumberReadsPerSec");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("NumberWritesPerSec");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("IoStallMSPerSec");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("dbChartContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TransactionsPerSec");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LogFlushesPerSec");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NumberReadsPerSec");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NumberWritesPerSec");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IoStallMSPerSec");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ValueBar", 0);
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            this.databaseChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.ultraDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.topDatabasesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraToolbarsDockArea1 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsDockArea2 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsDockArea3 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsDockArea4 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.mainTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.gridPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.chartPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            ((System.ComponentModel.ISupportInitialize)(this.databaseChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topDatabasesGrid)).BeginInit();
            this.mainTableLayoutPanel.SuspendLayout();
            this.gridPanel.SuspendLayout();
            this.chartPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // databaseChart
            // 
            this.databaseChart.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.None;
            this.databaseChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseChart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            this.databaseChart.Background = solidBackground1;
            this.databaseChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.databaseChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.databaseChart, "dbChartContextMenu");
            this.databaseChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.databaseChart.LegendBox.PlotAreaOnly = false;
            this.databaseChart.Location = new System.Drawing.Point(-2, 0);
            this.databaseChart.Margin = new System.Windows.Forms.Padding(0);
            this.databaseChart.Name = "databaseChart";
            this.databaseChart.Palette = "Schemes.Classic";
            this.databaseChart.PlotAreaColor = System.Drawing.Color.White;
            this.databaseChart.PlotAreaMargin.Bottom = 1;
            this.databaseChart.PlotAreaMargin.Left = 6;
            this.databaseChart.PlotAreaMargin.Right = 12;
            this.databaseChart.PlotAreaMargin.Top = 8;
            this.databaseChart.RandomData.Series = 5;
            seriesAttributes1.Text = "User db 1";
            seriesAttributes2.Text = "User db 2";
            seriesAttributes3.Text = "tempdb";
            seriesAttributes4.Text = "msdb";
            seriesAttributes5.Text = "AdventureWorks";
            this.databaseChart.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes1,
            seriesAttributes2,
            seriesAttributes3,
            seriesAttributes4,
            seriesAttributes5});
            this.databaseChart.Size = new System.Drawing.Size(380, 173);
            this.databaseChart.TabIndex = 0;
            this.databaseChart.Tag = "Trends";
            this.databaseChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            this.databaseChart.Resize += new System.EventHandler(this.databaseChart_Resize);
            // 
            // ultraDataSource
            // 
            ultraDataColumn2.DataType = typeof(long);
            ultraDataColumn3.DataType = typeof(long);
            ultraDataColumn4.DataType = typeof(decimal);
            ultraDataColumn5.DataType = typeof(decimal);
            ultraDataColumn6.DataType = typeof(decimal);
            this.ultraDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6});
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool10.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool10,
            buttonTool11});
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool4.SharedPropsInternal.AppearancesSmall.Appearance = appearance1;
            buttonTool4.SharedPropsInternal.Caption = "Print";
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool5.SharedPropsInternal.AppearancesSmall.Appearance = appearance2;
            buttonTool5.SharedPropsInternal.Caption = "Export To Excel (csv)";
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ExportChartImage16x16;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance3;
            buttonTool6.SharedPropsInternal.Caption = "Save Image";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Toolbar";
            buttonTool7.SharedPropsInternal.Caption = "Show Help";
            buttonTool8.SharedPropsInternal.Caption = "Show Details";
            buttonTool9.SharedPropsInternal.Caption = "Configure Alerts...";
            popupMenuTool2.SharedPropsInternal.Caption = "dbChartContextMenu";
            buttonTool12.InstanceProps.IsFirstInGroup = true;
            buttonTool22.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool12,
            buttonTool14,
            buttonTool22,
            buttonTool16,
            buttonTool25});
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            stateButtonTool2,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            popupMenuTool2});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // topDatabasesGrid
            // 
            this.topDatabasesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.toolbarsManager.SetContextMenuUltra(this.topDatabasesGrid, "gridContextMenu");
            this.topDatabasesGrid.DataSource = this.ultraDataSource;
            appearance4.BackColor = System.Drawing.Color.White;
            appearance4.BackColor2 = System.Drawing.Color.White;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.ForwardDiagonal;
            this.topDatabasesGrid.DisplayLayout.Appearance = appearance4;
            ultraGridBand1.ColHeadersVisible = false;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.MinWidth = 100;
            ultraGridColumn1.Width = 173;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn2.CellAppearance = appearance5;
            ultraGridColumn2.Format = "f0";
            ultraGridColumn2.Header.Caption = "Transactions";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.MaxWidth = 30;
            ultraGridColumn2.MinWidth = 30;
            ultraGridColumn2.Width = 30;
            appearance6.TextHAlignAsString = "Right";
            ultraGridColumn3.CellAppearance = appearance6;
            ultraGridColumn3.Format = "f0";
            ultraGridColumn3.Header.Caption = "Log Flushes";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn3.MaxWidth = 30;
            ultraGridColumn3.MinWidth = 30;
            ultraGridColumn3.Width = 30;
            appearance7.TextHAlignAsString = "Right";
            ultraGridColumn4.CellAppearance = appearance7;
            ultraGridColumn4.Format = "f0";
            ultraGridColumn4.Header.Caption = "Reads";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn4.MaxWidth = 30;
            ultraGridColumn4.MinWidth = 30;
            ultraGridColumn4.Width = 30;
            appearance8.TextHAlignAsString = "Right";
            ultraGridColumn5.CellAppearance = appearance8;
            ultraGridColumn5.Format = "f0";
            ultraGridColumn5.Header.Caption = "Writes";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.MaxWidth = 30;
            ultraGridColumn5.MinWidth = 30;
            ultraGridColumn5.Width = 30;
            appearance9.TextHAlignAsString = "Right";
            ultraGridColumn6.CellAppearance = appearance9;
            ultraGridColumn6.Format = "f0";
            ultraGridColumn6.Header.Caption = "I\\O Stalls";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn6.MaxWidth = 30;
            ultraGridColumn6.MinWidth = 30;
            ultraGridColumn6.Width = 30;
            appearance10.BackColor = System.Drawing.Color.LightSteelBlue;
            appearance10.BackColor2 = System.Drawing.Color.SteelBlue;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            ultraGridColumn7.CellAppearance = appearance10;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.MaxWidth = 37;
            ultraGridColumn7.MinWidth = 37;
            ultraGridColumn7.Width = 37;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.topDatabasesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.topDatabasesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.topDatabasesGrid.DisplayLayout.ColumnChooserEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.topDatabasesGrid.DisplayLayout.InterBandSpacing = 10;
            this.topDatabasesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.topDatabasesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.topDatabasesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.topDatabasesGrid.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.topDatabasesGrid.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.topDatabasesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.topDatabasesGrid.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.topDatabasesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.topDatabasesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.InsetSoft;
            this.topDatabasesGrid.DisplayLayout.Override.BorderStyleRowSelector = Infragistics.Win.UIElementBorderStyle.None;
            appearance11.BackColor = System.Drawing.Color.Transparent;
            this.topDatabasesGrid.DisplayLayout.Override.CardAreaAppearance = appearance11;
            appearance12.ForeColor = System.Drawing.Color.Navy;
            appearance12.TextVAlignAsString = "Middle";
            this.topDatabasesGrid.DisplayLayout.Override.CellAppearance = appearance12;
            this.topDatabasesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.topDatabasesGrid.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            appearance13.BackColor = System.Drawing.Color.DarkGray;
            appearance13.BackColor2 = System.Drawing.Color.Gainsboro;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance13.FontData.Name = "Tahoma";
            appearance13.FontData.SizeInPoints = 9F;
            appearance13.ForeColor = System.Drawing.Color.Navy;
            appearance13.TextHAlignAsString = "Left";
            appearance13.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.topDatabasesGrid.DisplayLayout.Override.HeaderAppearance = appearance13;
            this.topDatabasesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance27.BorderAlpha = Infragistics.Win.Alpha.Transparent;
            appearance27.BorderColor = System.Drawing.Color.White;
            appearance27.BorderColor2 = System.Drawing.Color.White;
            appearance27.BorderColor3DBase = System.Drawing.Color.White;
            this.topDatabasesGrid.DisplayLayout.Override.RowAppearance = appearance27;
            appearance15.BackColor = System.Drawing.Color.DarkGray;
            appearance15.BackColor2 = System.Drawing.Color.Gainsboro;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance15.ForeColor = System.Drawing.Color.Navy;
            this.topDatabasesGrid.DisplayLayout.Override.RowSelectorAppearance = appearance15;
            this.topDatabasesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.topDatabasesGrid.DisplayLayout.Override.RowSelectorWidth = 20;
            this.topDatabasesGrid.DisplayLayout.Override.RowSpacingAfter = 1;
            this.topDatabasesGrid.DisplayLayout.Override.RowSpacingBefore = 3;
            appearance16.BackColor = System.Drawing.Color.Navy;
            appearance16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.topDatabasesGrid.DisplayLayout.Override.SelectedRowAppearance = appearance16;
            this.topDatabasesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.topDatabasesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.topDatabasesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.topDatabasesGrid.DisplayLayout.RowConnectorColor = System.Drawing.Color.Gray;
            this.topDatabasesGrid.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.WindowsVista;
            this.topDatabasesGrid.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.topDatabasesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.topDatabasesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.topDatabasesGrid.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            this.topDatabasesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.topDatabasesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topDatabasesGrid.Location = new System.Drawing.Point(0, 0);
            this.topDatabasesGrid.Margin = new System.Windows.Forms.Padding(0);
            this.topDatabasesGrid.Name = "topDatabasesGrid";
            this.topDatabasesGrid.Size = new System.Drawing.Size(220, 168);
            this.topDatabasesGrid.TabIndex = 0;
            this.topDatabasesGrid.UseAppStyling = false;
            this.topDatabasesGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.topDatabasesGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.topDatabasesGrid_InitializeLayout);
            // 
            // ultraToolbarsDockArea1
            // 
            this.ultraToolbarsDockArea1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.ultraToolbarsDockArea1.BackColor = System.Drawing.Color.White;
            this.ultraToolbarsDockArea1.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this.ultraToolbarsDockArea1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraToolbarsDockArea1.Location = new System.Drawing.Point(0, 19);
            this.ultraToolbarsDockArea1.Name = "ultraToolbarsDockArea1";
            this.ultraToolbarsDockArea1.Size = new System.Drawing.Size(595, 0);
            this.ultraToolbarsDockArea1.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraToolbarsDockArea2
            // 
            this.ultraToolbarsDockArea2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.ultraToolbarsDockArea2.BackColor = System.Drawing.Color.White;
            this.ultraToolbarsDockArea2.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this.ultraToolbarsDockArea2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraToolbarsDockArea2.Location = new System.Drawing.Point(0, 187);
            this.ultraToolbarsDockArea2.Name = "ultraToolbarsDockArea2";
            this.ultraToolbarsDockArea2.Size = new System.Drawing.Size(595, 0);
            this.ultraToolbarsDockArea2.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraToolbarsDockArea3
            // 
            this.ultraToolbarsDockArea3.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.ultraToolbarsDockArea3.BackColor = System.Drawing.Color.White;
            this.ultraToolbarsDockArea3.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this.ultraToolbarsDockArea3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraToolbarsDockArea3.Location = new System.Drawing.Point(0, 19);
            this.ultraToolbarsDockArea3.Name = "ultraToolbarsDockArea3";
            this.ultraToolbarsDockArea3.Size = new System.Drawing.Size(0, 168);
            this.ultraToolbarsDockArea3.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraToolbarsDockArea4
            // 
            this.ultraToolbarsDockArea4.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.ultraToolbarsDockArea4.BackColor = System.Drawing.Color.White;
            this.ultraToolbarsDockArea4.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this.ultraToolbarsDockArea4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ultraToolbarsDockArea4.Location = new System.Drawing.Point(595, 19);
            this.ultraToolbarsDockArea4.Name = "ultraToolbarsDockArea4";
            this.ultraToolbarsDockArea4.Size = new System.Drawing.Size(0, 168);
            this.ultraToolbarsDockArea4.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraToolTipManager
            // 
            this.ultraToolTipManager.ContainingControl = this;
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 2;
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.5F));
                this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.5F));
            }
            else
            {
                this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
                this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            }
            this.mainTableLayoutPanel.Controls.Add(this.gridPanel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.chartPanel, 1, 0);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 19);
            this.mainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 1;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(595, 168);
            this.mainTableLayoutPanel.TabIndex = 9;
            // 
            // gridPanel
            // 
            this.gridPanel.Controls.Add(this.topDatabasesGrid);
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Location = new System.Drawing.Point(0, 0);
            this.gridPanel.Margin = new System.Windows.Forms.Padding(0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(220, 168);
            this.gridPanel.TabIndex = 1;
            // 
            // chartPanel
            // 
            this.chartPanel.Controls.Add(this.databaseChart);
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel.Location = new System.Drawing.Point(220, 0);
            this.chartPanel.Margin = new System.Windows.Forms.Padding(0);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(375, 168);
            this.chartPanel.TabIndex = 2;
            // 
            // DatabasesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Caption = "Databases";
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Controls.Add(this.ultraToolbarsDockArea3);
            this.Controls.Add(this.ultraToolbarsDockArea4);
            this.Controls.Add(this.ultraToolbarsDockArea1);
            this.Controls.Add(this.ultraToolbarsDockArea2);
            this.Name = "DatabasesControl";
            this.Size = new System.Drawing.Size(595, 187);
            this.Controls.SetChildIndex(this.ultraToolbarsDockArea2, 0);
            this.Controls.SetChildIndex(this.ultraToolbarsDockArea1, 0);
            this.Controls.SetChildIndex(this.ultraToolbarsDockArea4, 0);
            this.Controls.SetChildIndex(this.ultraToolbarsDockArea3, 0);
            this.Controls.SetChildIndex(this.mainTableLayoutPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.databaseChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topDatabasesGrid)).EndInit();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.gridPanel.ResumeLayout(false);
            this.chartPanel.ResumeLayout(false);
            this.chartPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ChartFX.WinForms.Chart databaseChart;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea ultraToolbarsDockArea3;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea ultraToolbarsDockArea4;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea ultraToolbarsDockArea1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea ultraToolbarsDockArea2;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel mainTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  gridPanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid topDatabasesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  chartPanel;
    }
}
