using Idera.SQLdm.DesktopClient.Properties;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    partial class SessionsSummaryView
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
            if (disposing)
            {
                Settings.Default.PropertyChanged -= Settings_PropertyChanged;

                if (components != null)
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
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionsSummaryView));
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground2 = new ChartFX.WinForms.Adornments.GradientBackground();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground3 = new ChartFX.WinForms.Adornments.GradientBackground();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground4 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.maxSessionPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chartPanel = new System.Windows.Forms.Panel();
            this.maxSessionChart = new ChartFX.WinForms.Chart();
            this.label2 = new System.Windows.Forms.Label();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.topDatabasesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.maximizeMaxSessionChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreMaxSessionChartButton = new System.Windows.Forms.ToolStripButton();
            this.lockStatisticsPanel = new System.Windows.Forms.Panel();
            this.lockStatisticsChartContainerPanel = new System.Windows.Forms.Panel();
            this.lockStatisticsChart = new ChartFX.WinForms.Chart();
            this.lockStatisticsChartStatusLabel = new System.Windows.Forms.Label();
            this.lockStatisticsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.maximizeLockStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.lockStatisticsOptionsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.averageWaitTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deadlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.requestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeoutsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waitTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreLockStatisticsChartButton = new System.Windows.Forms.ToolStripButton();
            this.blockedSessionsPanel = new System.Windows.Forms.Panel();
            this.blockedProcessesChartContainerPanel = new System.Windows.Forms.Panel();
            this.blockedSessionsChart = new ChartFX.WinForms.Chart();
            this.blockedProcessesChartStatusLabel = new System.Windows.Forms.Label();
            this.blockedSessionsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.blockedSessionsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.maximizeBlockedSessionsChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreBlockedSessionsChartButton = new System.Windows.Forms.ToolStripButton();
            this.responseTimePanel = new System.Windows.Forms.Panel();
            this.responseTimeChartContainerPanel = new System.Windows.Forms.Panel();
            this.responseTimeChart = new ChartFX.WinForms.Chart();
            this.responseTimeChartStatusLabel = new System.Windows.Forms.Label();
            this.responseTimeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.responseTimeHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.maximizeResponseTimeChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreResponseTimeChartButton = new System.Windows.Forms.ToolStripButton();
            this.sessionsPanel = new System.Windows.Forms.Panel();
            this.sessionsChartContainerPanel = new System.Windows.Forms.Panel();
            this.sessionsChart = new ChartFX.WinForms.Chart();
            this.sessionsChartStatusLabel = new System.Windows.Forms.Label();
            this.sessionsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.sessionsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.maximizeSessionsChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreSessionsChartButton = new System.Windows.Forms.ToolStripButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.historicalSnapshotStatusLinkLabel = new System.Windows.Forms.LinkLabel();
            this.fillPanel = new System.Windows.Forms.Panel();
            this.chartContainerPanel = new System.Windows.Forms.Panel();
            this.operationalStatusPanel = new System.Windows.Forms.Panel();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.maxSessionPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.chartPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxSessionChart)).BeginInit();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.topDatabasesGrid)).BeginInit();
            this.headerStrip1.SuspendLayout();
            this.lockStatisticsPanel.SuspendLayout();
            this.lockStatisticsChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockStatisticsChart)).BeginInit();
            this.lockStatisticsHeaderStrip.SuspendLayout();
            this.blockedSessionsPanel.SuspendLayout();
            this.blockedProcessesChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockedSessionsChart)).BeginInit();
            this.blockedSessionsHeaderStrip.SuspendLayout();
            this.responseTimePanel.SuspendLayout();
            this.responseTimeChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.responseTimeChart)).BeginInit();
            this.responseTimeHeaderStrip.SuspendLayout();
            this.sessionsPanel.SuspendLayout();
            this.sessionsChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sessionsChart)).BeginInit();
            this.sessionsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.fillPanel.SuspendLayout();
            this.chartContainerPanel.SuspendLayout();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.maxSessionPanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.lockStatisticsPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.blockedSessionsPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.sessionsPanel, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(804, 966);
            this.tableLayoutPanel.TabIndex = 0;
            this.tableLayoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel_Paint);
            // 
            // maxSessionPanel
            // 
            this.maxSessionPanel.Controls.Add(this.tableLayoutPanel1);
            this.maxSessionPanel.Controls.Add(this.headerStrip1);
            this.maxSessionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maxSessionPanel.Location = new System.Drawing.Point(3, 639);
            this.maxSessionPanel.Name = "maxSessionPanel";
            this.maxSessionPanel.Size = new System.Drawing.Size(396, 324);
            this.maxSessionPanel.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.chartPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gridPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(396, 305);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // chartPanel
            // 
            this.chartPanel.Controls.Add(this.maxSessionChart);
            this.chartPanel.Controls.Add(this.label2);
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel.Location = new System.Drawing.Point(118, 0);
            this.chartPanel.Margin = new System.Windows.Forms.Padding(0);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(278, 305);
            this.chartPanel.TabIndex = 3;
            // 
            // maxSessionChart
            // 
            this.maxSessionChart.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.None;
            this.maxSessionChart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            this.maxSessionChart.Background = solidBackground1;
            this.maxSessionChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.maxSessionChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.maxSessionChart, "ChartContextMenu");
            this.maxSessionChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.maxSessionChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maxSessionChart.LegendBox.PlotAreaOnly = false;
            this.maxSessionChart.Location = new System.Drawing.Point(0, 0);
            this.maxSessionChart.Margin = new System.Windows.Forms.Padding(0);
            this.maxSessionChart.Name = "Max Session Percent";
            this.maxSessionChart.Palette = "Schemes.Classic";
            this.maxSessionChart.PlotAreaColor = System.Drawing.Color.White;
            this.maxSessionChart.PlotAreaMargin.Bottom = 1;
            this.maxSessionChart.PlotAreaMargin.Left = 6;
            this.maxSessionChart.PlotAreaMargin.Right = 12;
            this.maxSessionChart.PlotAreaMargin.Top = 8;
            this.maxSessionChart.RandomData.Series = 5;
            seriesAttributes4.Text = "User db 1";
            seriesAttributes5.Text = "User db 2";
            this.maxSessionChart.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes1,
            seriesAttributes2,
            seriesAttributes3,
            seriesAttributes4,
            seriesAttributes5});
            this.maxSessionChart.Size = new System.Drawing.Size(278, 305);
            this.maxSessionChart.TabIndex = 0;
            this.maxSessionChart.Tag = "Trends";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 305);
            this.label2.TabIndex = 1;
            this.label2.Text = "Loading..";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gridPanel
            // 
            this.gridPanel.Controls.Add(this.topDatabasesGrid);
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Location = new System.Drawing.Point(0, 0);
            this.gridPanel.Margin = new System.Windows.Forms.Padding(0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(118, 305);
            this.gridPanel.TabIndex = 2;
            // 
            // topDatabasesGrid
            // 
            this.toolbarsManager.SetContextMenuUltra(this.topDatabasesGrid, "gridContextMenu");
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
            this.topDatabasesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topDatabasesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topDatabasesGrid.Location = new System.Drawing.Point(0, 0);
            this.topDatabasesGrid.Margin = new System.Windows.Forms.Padding(0);
            this.topDatabasesGrid.Name = "topDatabasesGrid";
            this.topDatabasesGrid.Size = new System.Drawing.Size(118, 305);
            this.topDatabasesGrid.TabIndex = 0;
            this.topDatabasesGrid.UseAppStyling = false;
            this.topDatabasesGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // headerStrip1
            // 
            this.headerStrip1.AutoSize = false;
            this.headerStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.headerStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.headerStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip1.HotTrackEnabled = false;
            this.headerStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.headerStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.maximizeMaxSessionChartButton,
            this.restoreMaxSessionChartButton});
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.headerStrip1.Size = new System.Drawing.Size(396, 19);
            this.headerStrip1.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.headerStrip1.TabIndex = 10;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(92, 16);
            this.toolStripLabel1.Text = "Max Session Percent";
            // 
            // maximizeMaxSessionChartButton
            // 
            this.maximizeMaxSessionChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeMaxSessionChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeMaxSessionChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeMaxSessionChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeMaxSessionChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeMaxSessionChartButton.Name = "maximizeMaxSessionChartButton";
            this.maximizeMaxSessionChartButton.Size = new System.Drawing.Size(29, 16);
            this.maximizeMaxSessionChartButton.ToolTipText = "Maximize";
            this.maximizeMaxSessionChartButton.Click += new System.EventHandler(this.maximizeMaxSessionChartButton_Click);
            // 
            // restoreMaxSessionChartButton
            // 
            this.restoreMaxSessionChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreMaxSessionChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreMaxSessionChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreMaxSessionChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreMaxSessionChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreMaxSessionChartButton.Name = "restoreMaxSessionChartButton";
            this.restoreMaxSessionChartButton.Size = new System.Drawing.Size(29, 16);
            this.restoreMaxSessionChartButton.Text = "Restore";
            this.restoreMaxSessionChartButton.ToolTipText = "Restore";
            this.restoreMaxSessionChartButton.Visible = false;
            this.restoreMaxSessionChartButton.Click += new System.EventHandler(this.restoreMaxSessionChartButton_Click);
            // 
            // lockStatisticsPanel
            // 
            this.lockStatisticsPanel.Controls.Add(this.lockStatisticsChartContainerPanel);
            this.lockStatisticsPanel.Controls.Add(this.lockStatisticsHeaderStrip);
            this.lockStatisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsPanel.Location = new System.Drawing.Point(405, 321);
            this.lockStatisticsPanel.Name = "lockStatisticsPanel";
            this.lockStatisticsPanel.Size = new System.Drawing.Size(396, 312);
            this.lockStatisticsPanel.TabIndex = 3;
            // 
            // lockStatisticsChartContainerPanel
            // 
            this.lockStatisticsChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.lockStatisticsChartContainerPanel.Controls.Add(this.lockStatisticsChart);
            this.lockStatisticsChartContainerPanel.Controls.Add(this.lockStatisticsChartStatusLabel);
            this.lockStatisticsChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.lockStatisticsChartContainerPanel.Name = "lockStatisticsChartContainerPanel";
            this.lockStatisticsChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lockStatisticsChartContainerPanel.Size = new System.Drawing.Size(396, 293);
            this.lockStatisticsChartContainerPanel.TabIndex = 12;
            // 
            // lockStatisticsChart
            // 
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.lockStatisticsChart.Background = gradientBackground1;
            this.lockStatisticsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.lockStatisticsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.lockStatisticsChart, "chartContextMenu");
            this.lockStatisticsChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lockStatisticsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsChart.Location = new System.Drawing.Point(1, 0);
            this.lockStatisticsChart.Name = "lockStatisticsChart";
            this.lockStatisticsChart.Palette = "Schemes.Classic";
            this.lockStatisticsChart.PlotAreaColor = System.Drawing.Color.White;
            this.lockStatisticsChart.RandomData.Series = 13;
            this.lockStatisticsChart.Size = new System.Drawing.Size(394, 292);
            this.lockStatisticsChart.TabIndex = 11;
            this.lockStatisticsChart.Visible = false;
            this.lockStatisticsChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            // 
            // lockStatisticsChartStatusLabel
            // 
            this.lockStatisticsChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.lockStatisticsChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lockStatisticsChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.lockStatisticsChartStatusLabel.Name = "lockStatisticsChartStatusLabel";
            this.lockStatisticsChartStatusLabel.Size = new System.Drawing.Size(394, 292);
            this.lockStatisticsChartStatusLabel.TabIndex = 15;
            this.lockStatisticsChartStatusLabel.Text = "Loading...";
            this.lockStatisticsChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lockStatisticsHeaderStrip
            // 
            this.lockStatisticsHeaderStrip.AutoSize = false;
            this.lockStatisticsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lockStatisticsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.lockStatisticsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.lockStatisticsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.lockStatisticsHeaderStrip.HotTrackEnabled = false;
            this.lockStatisticsHeaderStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.lockStatisticsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maximizeLockStatisticsChartButton,
            this.lockStatisticsOptionsButton,
            this.restoreLockStatisticsChartButton});
            this.lockStatisticsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.lockStatisticsHeaderStrip.Name = "lockStatisticsHeaderStrip";
            this.lockStatisticsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lockStatisticsHeaderStrip.Size = new System.Drawing.Size(396, 19);
            this.lockStatisticsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.lockStatisticsHeaderStrip.TabIndex = 10;
            // 
            // maximizeLockStatisticsChartButton
            // 
            this.maximizeLockStatisticsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeLockStatisticsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeLockStatisticsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeLockStatisticsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeLockStatisticsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeLockStatisticsChartButton.Name = "maximizeLockStatisticsChartButton";
            this.maximizeLockStatisticsChartButton.Size = new System.Drawing.Size(29, 16);
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
            this.lockStatisticsOptionsButton.Size = new System.Drawing.Size(193, 16);
            this.lockStatisticsOptionsButton.Text = "Lock Statistics: Requests";
            // 
            // averageWaitTimeToolStripMenuItem
            // 
            this.averageWaitTimeToolStripMenuItem.Name = "averageWaitTimeToolStripMenuItem";
            this.averageWaitTimeToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.averageWaitTimeToolStripMenuItem.Text = "Average Wait Time";
            this.averageWaitTimeToolStripMenuItem.Click += new System.EventHandler(this.averageWaitTimeToolStripMenuItem_Click);
            // 
            // deadlocksToolStripMenuItem
            // 
            this.deadlocksToolStripMenuItem.Name = "deadlocksToolStripMenuItem";
            this.deadlocksToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.deadlocksToolStripMenuItem.Text = "Deadlocks";
            this.deadlocksToolStripMenuItem.Click += new System.EventHandler(this.deadlocksToolStripMenuItem_Click);
            // 
            // requestsToolStripMenuItem
            // 
            this.requestsToolStripMenuItem.Name = "requestsToolStripMenuItem";
            this.requestsToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.requestsToolStripMenuItem.Text = "Requests";
            this.requestsToolStripMenuItem.Click += new System.EventHandler(this.requestsToolStripMenuItem_Click);
            // 
            // timeoutsToolStripMenuItem
            // 
            this.timeoutsToolStripMenuItem.Name = "timeoutsToolStripMenuItem";
            this.timeoutsToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.timeoutsToolStripMenuItem.Text = "Timeouts";
            this.timeoutsToolStripMenuItem.Click += new System.EventHandler(this.timeoutsToolStripMenuItem_Click);
            // 
            // waitsToolStripMenuItem
            // 
            this.waitsToolStripMenuItem.Name = "waitsToolStripMenuItem";
            this.waitsToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.waitsToolStripMenuItem.Text = "Waits";
            this.waitsToolStripMenuItem.Click += new System.EventHandler(this.waitsToolStripMenuItem_Click);
            // 
            // waitTimeToolStripMenuItem
            // 
            this.waitTimeToolStripMenuItem.Name = "waitTimeToolStripMenuItem";
            this.waitTimeToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
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
            this.restoreLockStatisticsChartButton.Size = new System.Drawing.Size(29, 16);
            this.restoreLockStatisticsChartButton.Text = "Restore";
            this.restoreLockStatisticsChartButton.Visible = false;
            this.restoreLockStatisticsChartButton.Click += new System.EventHandler(this.restoreLockStatisticsChartButton_Click);
            // 
            // blockedSessionsPanel
            // 
            this.blockedSessionsPanel.Controls.Add(this.blockedProcessesChartContainerPanel);
            this.blockedSessionsPanel.Controls.Add(this.blockedSessionsHeaderStrip);
            this.blockedSessionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedSessionsPanel.Location = new System.Drawing.Point(3, 321);
            this.blockedSessionsPanel.Name = "blockedSessionsPanel";
            this.blockedSessionsPanel.Size = new System.Drawing.Size(396, 312);
            this.blockedSessionsPanel.TabIndex = 2;
            // 
            // blockedProcessesChartContainerPanel
            // 
            this.blockedProcessesChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.blockedProcessesChartContainerPanel.Controls.Add(this.blockedSessionsChart);
            this.blockedProcessesChartContainerPanel.Controls.Add(this.blockedProcessesChartStatusLabel);
            this.blockedProcessesChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedProcessesChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.blockedProcessesChartContainerPanel.Name = "blockedProcessesChartContainerPanel";
            this.blockedProcessesChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.blockedProcessesChartContainerPanel.Size = new System.Drawing.Size(396, 293);
            this.blockedProcessesChartContainerPanel.TabIndex = 12;
            // 
            // blockedSessionsChart
            // 
            gradientBackground2.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.blockedSessionsChart.Background = gradientBackground2;
            this.blockedSessionsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.blockedSessionsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.blockedSessionsChart, "chartContextMenu");
            this.blockedSessionsChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.blockedSessionsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedSessionsChart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            this.blockedSessionsChart.Location = new System.Drawing.Point(1, 0);
            this.blockedSessionsChart.Name = "blockedSessionsChart";
            this.blockedSessionsChart.Palette = "Schemes.Classic";
            this.blockedSessionsChart.PlotAreaColor = System.Drawing.Color.White;
            this.blockedSessionsChart.RandomData.Series = 2;
            this.blockedSessionsChart.Size = new System.Drawing.Size(394, 292);
            this.blockedSessionsChart.TabIndex = 11;
            this.blockedSessionsChart.Visible = false;
            this.blockedSessionsChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            // 
            // blockedProcessesChartStatusLabel
            // 
            this.blockedProcessesChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.blockedProcessesChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockedProcessesChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.blockedProcessesChartStatusLabel.Name = "blockedProcessesChartStatusLabel";
            this.blockedProcessesChartStatusLabel.Size = new System.Drawing.Size(394, 292);
            this.blockedProcessesChartStatusLabel.TabIndex = 14;
            this.blockedProcessesChartStatusLabel.Text = "Loading...";
            this.blockedProcessesChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // blockedSessionsHeaderStrip
            // 
            this.blockedSessionsHeaderStrip.AutoSize = false;
            this.blockedSessionsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.blockedSessionsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.blockedSessionsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.blockedSessionsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.blockedSessionsHeaderStrip.HotTrackEnabled = false;
            this.blockedSessionsHeaderStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.blockedSessionsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blockedSessionsHeaderStripLabel,
            this.maximizeBlockedSessionsChartButton,
            this.restoreBlockedSessionsChartButton});
            this.blockedSessionsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.blockedSessionsHeaderStrip.Name = "blockedSessionsHeaderStrip";
            this.blockedSessionsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.blockedSessionsHeaderStrip.Size = new System.Drawing.Size(396, 19);
            this.blockedSessionsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.blockedSessionsHeaderStrip.TabIndex = 10;
            // 
            // blockedSessionsHeaderStripLabel
            // 
            this.blockedSessionsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blockedSessionsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.blockedSessionsHeaderStripLabel.Name = "blockedSessionsHeaderStripLabel";
            this.blockedSessionsHeaderStripLabel.Size = new System.Drawing.Size(125, 16);
            this.blockedSessionsHeaderStripLabel.Text = "Blocked Sessions";
            // 
            // maximizeBlockedSessionsChartButton
            // 
            this.maximizeBlockedSessionsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeBlockedSessionsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeBlockedSessionsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeBlockedSessionsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeBlockedSessionsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeBlockedSessionsChartButton.Name = "maximizeBlockedSessionsChartButton";
            this.maximizeBlockedSessionsChartButton.Size = new System.Drawing.Size(29, 16);
            this.maximizeBlockedSessionsChartButton.ToolTipText = "Maximize";
            this.maximizeBlockedSessionsChartButton.Click += new System.EventHandler(this.maximizeBlockedSessionsChartButton_Click);
            // 
            // restoreBlockedSessionsChartButton
            // 
            this.restoreBlockedSessionsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreBlockedSessionsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreBlockedSessionsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreBlockedSessionsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreBlockedSessionsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreBlockedSessionsChartButton.Name = "restoreBlockedSessionsChartButton";
            this.restoreBlockedSessionsChartButton.Size = new System.Drawing.Size(29, 16);
            this.restoreBlockedSessionsChartButton.Text = "Restore";
            this.restoreBlockedSessionsChartButton.Visible = false;
            this.restoreBlockedSessionsChartButton.Click += new System.EventHandler(this.restoreBlockedSessionsChartButton_Click);
            // 
            // responseTimePanel
            // 
            this.responseTimePanel.Controls.Add(this.responseTimeChartContainerPanel);
            this.responseTimePanel.Controls.Add(this.responseTimeHeaderStrip);
            this.responseTimePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.responseTimePanel.Location = new System.Drawing.Point(3, 3);
            this.responseTimePanel.Name = "responseTimePanel";
            this.responseTimePanel.Size = new System.Drawing.Size(396, 312);
            this.responseTimePanel.TabIndex = 0;
            // 
            // responseTimeChartContainerPanel
            // 
            this.responseTimeChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.responseTimeChartContainerPanel.Controls.Add(this.responseTimeChart);
            this.responseTimeChartContainerPanel.Controls.Add(this.responseTimeChartStatusLabel);
            this.responseTimeChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.responseTimeChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.responseTimeChartContainerPanel.Name = "responseTimeChartContainerPanel";
            this.responseTimeChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.responseTimeChartContainerPanel.Size = new System.Drawing.Size(396, 293);
            this.responseTimeChartContainerPanel.TabIndex = 12;
            // 
            // responseTimeChart
            // 
            this.responseTimeChart.AxisY.Title.Text = "ms";
            gradientBackground3.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.responseTimeChart.Background = gradientBackground3;
            this.responseTimeChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.responseTimeChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.responseTimeChart, "chartContextMenu");
            this.responseTimeChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.responseTimeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.responseTimeChart.LegendBox.Visible = false;
            this.responseTimeChart.Location = new System.Drawing.Point(1, 0);
            this.responseTimeChart.Name = "responseTimeChart";
            this.responseTimeChart.Palette = "Schemes.Classic";
            this.responseTimeChart.PlotAreaColor = System.Drawing.Color.White;
            this.responseTimeChart.RandomData.Series = 1;
            this.responseTimeChart.Size = new System.Drawing.Size(394, 292);
            this.responseTimeChart.TabIndex = 11;
            this.responseTimeChart.Visible = false;
            this.responseTimeChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            // 
            // responseTimeChartStatusLabel
            // 
            this.responseTimeChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.responseTimeChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.responseTimeChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.responseTimeChartStatusLabel.Name = "responseTimeChartStatusLabel";
            this.responseTimeChartStatusLabel.Size = new System.Drawing.Size(394, 292);
            this.responseTimeChartStatusLabel.TabIndex = 12;
            this.responseTimeChartStatusLabel.Text = "Loading...";
            this.responseTimeChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // responseTimeHeaderStrip
            // 
            this.responseTimeHeaderStrip.AutoSize = false;
            this.responseTimeHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.responseTimeHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.responseTimeHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.responseTimeHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.responseTimeHeaderStrip.HotTrackEnabled = false;
            this.responseTimeHeaderStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.responseTimeHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.responseTimeHeaderStripLabel,
            this.maximizeResponseTimeChartButton,
            this.restoreResponseTimeChartButton});
            this.responseTimeHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.responseTimeHeaderStrip.Name = "responseTimeHeaderStrip";
            this.responseTimeHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.responseTimeHeaderStrip.Size = new System.Drawing.Size(396, 19);
            this.responseTimeHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.responseTimeHeaderStrip.TabIndex = 10;
            // 
            // responseTimeHeaderStripLabel
            // 
            this.responseTimeHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.responseTimeHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.responseTimeHeaderStripLabel.Name = "responseTimeHeaderStripLabel";
            this.responseTimeHeaderStripLabel.Size = new System.Drawing.Size(112, 16);
            this.responseTimeHeaderStripLabel.Text = "Response Time";
            // 
            // maximizeResponseTimeChartButton
            // 
            this.maximizeResponseTimeChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeResponseTimeChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeResponseTimeChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeResponseTimeChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeResponseTimeChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeResponseTimeChartButton.Name = "maximizeResponseTimeChartButton";
            this.maximizeResponseTimeChartButton.Size = new System.Drawing.Size(29, 16);
            this.maximizeResponseTimeChartButton.ToolTipText = "Maximize";
            this.maximizeResponseTimeChartButton.Click += new System.EventHandler(this.maximizeResponseTimeChartButton_Click);
            // 
            // restoreResponseTimeChartButton
            // 
            this.restoreResponseTimeChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreResponseTimeChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreResponseTimeChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreResponseTimeChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreResponseTimeChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreResponseTimeChartButton.Name = "restoreResponseTimeChartButton";
            this.restoreResponseTimeChartButton.Size = new System.Drawing.Size(29, 16);
            this.restoreResponseTimeChartButton.Text = "Restore";
            this.restoreResponseTimeChartButton.Visible = false;
            this.restoreResponseTimeChartButton.Click += new System.EventHandler(this.restoreResponseTimeChartButton_Click);
            // 
            // sessionsPanel
            // 
            this.sessionsPanel.Controls.Add(this.sessionsChartContainerPanel);
            this.sessionsPanel.Controls.Add(this.sessionsHeaderStrip);
            this.sessionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsPanel.Location = new System.Drawing.Point(405, 3);
            this.sessionsPanel.Name = "sessionsPanel";
            this.sessionsPanel.Size = new System.Drawing.Size(396, 312);
            this.sessionsPanel.TabIndex = 1;
            // 
            // sessionsChartContainerPanel
            // 
            this.sessionsChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.sessionsChartContainerPanel.Controls.Add(this.sessionsChart);
            this.sessionsChartContainerPanel.Controls.Add(this.sessionsChartStatusLabel);
            this.sessionsChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.sessionsChartContainerPanel.Name = "sessionsChartContainerPanel";
            this.sessionsChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.sessionsChartContainerPanel.Size = new System.Drawing.Size(396, 293);
            this.sessionsChartContainerPanel.TabIndex = 12;
            // 
            // sessionsChart
            // 
            this.sessionsChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Area;
            this.sessionsChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            gradientBackground4.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.sessionsChart.Background = gradientBackground4;
            this.sessionsChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.sessionsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.sessionsChart, "chartContextMenu");
            this.sessionsChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sessionsChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsChart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            this.sessionsChart.LegendBox.PlotAreaOnly = false;
            this.sessionsChart.Location = new System.Drawing.Point(1, 0);
            this.sessionsChart.Name = "sessionsChart";
            this.sessionsChart.Palette = "Schemes.Classic";
            this.sessionsChart.PlotAreaColor = System.Drawing.Color.White;
            this.sessionsChart.Size = new System.Drawing.Size(394, 292);
            this.sessionsChart.TabIndex = 12;
            this.sessionsChart.Visible = false;
            this.sessionsChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);
            // 
            // sessionsChartStatusLabel
            // 
            this.sessionsChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.sessionsChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.sessionsChartStatusLabel.Name = "sessionsChartStatusLabel";
            this.sessionsChartStatusLabel.Size = new System.Drawing.Size(394, 292);
            this.sessionsChartStatusLabel.TabIndex = 13;
            this.sessionsChartStatusLabel.Text = "Loading...";
            this.sessionsChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sessionsHeaderStrip
            // 
            this.sessionsHeaderStrip.AutoSize = false;
            this.sessionsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.sessionsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.sessionsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.sessionsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.sessionsHeaderStrip.HotTrackEnabled = false;
            this.sessionsHeaderStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.sessionsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sessionsHeaderStripLabel,
            this.maximizeSessionsChartButton,
            this.restoreSessionsChartButton});
            this.sessionsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.sessionsHeaderStrip.Name = "sessionsHeaderStrip";
            this.sessionsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.sessionsHeaderStrip.Size = new System.Drawing.Size(396, 19);
            this.sessionsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.sessionsHeaderStrip.TabIndex = 10;
            // 
            // sessionsHeaderStripLabel
            // 
            this.sessionsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sessionsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.sessionsHeaderStripLabel.Name = "sessionsHeaderStripLabel";
            this.sessionsHeaderStripLabel.Size = new System.Drawing.Size(67, 16);
            this.sessionsHeaderStripLabel.Text = "Sessions";
            // 
            // maximizeSessionsChartButton
            // 
            this.maximizeSessionsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeSessionsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeSessionsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeSessionsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeSessionsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeSessionsChartButton.Name = "maximizeSessionsChartButton";
            this.maximizeSessionsChartButton.Size = new System.Drawing.Size(29, 16);
            this.maximizeSessionsChartButton.ToolTipText = "Maximize";
            this.maximizeSessionsChartButton.Click += new System.EventHandler(this.maximizeSessionsChartButton_Click);
            // 
            // restoreSessionsChartButton
            // 
            this.restoreSessionsChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreSessionsChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreSessionsChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreSessionsChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreSessionsChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreSessionsChartButton.Name = "restoreSessionsChartButton";
            this.restoreSessionsChartButton.Size = new System.Drawing.Size(29, 16);
            this.restoreSessionsChartButton.Text = "Restore";
            this.restoreSessionsChartButton.Visible = false;
            this.restoreSessionsChartButton.Click += new System.EventHandler(this.restoreSessionsChartButton_Click);
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            buttonTool1,
            buttonTool2,
            buttonTool3});
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
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            stateButtonTool2});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(804, 993);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 9;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "Data for the selected historical snapshot does not exist for this view. Select an" +
    "other historical snapshot or click here to switch to real-time mode.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // fillPanel
            // 
            this.fillPanel.Controls.Add(this.chartContainerPanel);
            this.fillPanel.Controls.Add(this.operationalStatusPanel);
            this.fillPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fillPanel.Location = new System.Drawing.Point(0, 0);
            this.fillPanel.Name = "fillPanel";
            this.fillPanel.Size = new System.Drawing.Size(804, 993);
            this.fillPanel.TabIndex = 10;
            // 
            // chartContainerPanel
            // 
            this.chartContainerPanel.Controls.Add(this.tableLayoutPanel);
            this.chartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartContainerPanel.Location = new System.Drawing.Point(0, 27);
            this.chartContainerPanel.Name = "chartContainerPanel";
            this.chartContainerPanel.Size = new System.Drawing.Size(804, 966);
            this.chartContainerPanel.TabIndex = 16;
            // 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(804, 27);
            this.operationalStatusPanel.TabIndex = 15;
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
            this.operationalStatusLabel.Size = new System.Drawing.Size(796, 21);
            this.operationalStatusLabel.TabIndex = 2;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // SessionsSummaryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.fillPanel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "SessionsSummaryView";
            this.Size = new System.Drawing.Size(804, 993);
            this.Load += new System.EventHandler(this.SessionsSummaryView_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.maxSessionPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.chartPanel.ResumeLayout(false);
            this.chartPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxSessionChart)).EndInit();
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.topDatabasesGrid)).EndInit();
            this.headerStrip1.ResumeLayout(false);
            this.headerStrip1.PerformLayout();
            this.lockStatisticsPanel.ResumeLayout(false);
            this.lockStatisticsChartContainerPanel.ResumeLayout(false);
            this.lockStatisticsChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockStatisticsChart)).EndInit();
            this.lockStatisticsHeaderStrip.ResumeLayout(false);
            this.lockStatisticsHeaderStrip.PerformLayout();
            this.blockedSessionsPanel.ResumeLayout(false);
            this.blockedProcessesChartContainerPanel.ResumeLayout(false);
            this.blockedProcessesChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockedSessionsChart)).EndInit();
            this.blockedSessionsHeaderStrip.ResumeLayout(false);
            this.blockedSessionsHeaderStrip.PerformLayout();
            this.responseTimePanel.ResumeLayout(false);
            this.responseTimeChartContainerPanel.ResumeLayout(false);
            this.responseTimeChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.responseTimeChart)).EndInit();
            this.responseTimeHeaderStrip.ResumeLayout(false);
            this.responseTimeHeaderStrip.PerformLayout();
            this.sessionsPanel.ResumeLayout(false);
            this.sessionsChartContainerPanel.ResumeLayout(false);
            this.sessionsChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sessionsChart)).EndInit();
            this.sessionsHeaderStrip.ResumeLayout(false);
            this.sessionsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.fillPanel.ResumeLayout(false);
            this.chartContainerPanel.ResumeLayout(false);
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel responseTimePanel;
        private ChartFX.WinForms.Chart responseTimeChart;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip responseTimeHeaderStrip;
        private System.Windows.Forms.ToolStripLabel responseTimeHeaderStripLabel;
        private System.Windows.Forms.Panel responseTimeChartContainerPanel;
        private System.Windows.Forms.Panel lockStatisticsPanel;
        private System.Windows.Forms.Panel lockStatisticsChartContainerPanel;
        private ChartFX.WinForms.Chart lockStatisticsChart;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip lockStatisticsHeaderStrip;
        private System.Windows.Forms.Panel blockedSessionsPanel;
        private System.Windows.Forms.Panel blockedProcessesChartContainerPanel;
        private ChartFX.WinForms.Chart blockedSessionsChart;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip blockedSessionsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel blockedSessionsHeaderStripLabel;
        private System.Windows.Forms.Panel sessionsPanel;
        private System.Windows.Forms.Panel sessionsChartContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip sessionsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel sessionsHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton maximizeResponseTimeChartButton;
        private System.Windows.Forms.ToolStripButton maximizeLockStatisticsChartButton;
        private System.Windows.Forms.ToolStripButton maximizeBlockedSessionsChartButton;
        private System.Windows.Forms.ToolStripButton maximizeSessionsChartButton;
        private ChartFX.WinForms.Chart sessionsChart;
        private System.Windows.Forms.Label responseTimeChartStatusLabel;
        private System.Windows.Forms.Label sessionsChartStatusLabel;
        private System.Windows.Forms.Label blockedProcessesChartStatusLabel;
        private System.Windows.Forms.ToolStripDropDownButton lockStatisticsOptionsButton;
        private System.Windows.Forms.Label lockStatisticsChartStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem averageWaitTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deadlocksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem requestsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeoutsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton restoreLockStatisticsChartButton;
        private System.Windows.Forms.ToolStripButton restoreBlockedSessionsChartButton;
        private System.Windows.Forms.ToolStripButton restoreResponseTimeChartButton;
        private System.Windows.Forms.ToolStripButton restoreSessionsChartButton;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.Panel fillPanel;
        private System.Windows.Forms.LinkLabel historicalSnapshotStatusLinkLabel;
        private System.Windows.Forms.Panel chartContainerPanel;
        private System.Windows.Forms.Panel operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private System.Windows.Forms.Label operationalStatusLabel;
        private Panel maxSessionPanel;
        private TableLayoutPanel tableLayoutPanel1;
        private Controls.HeaderStrip headerStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton maximizeMaxSessionChartButton;
        private ToolStripButton restoreMaxSessionChartButton;
        private Panel gridPanel;
        private Infragistics.Win.UltraWinGrid.UltraGrid topDatabasesGrid;
        private Panel chartPanel;
        private ChartFX.WinForms.Chart maxSessionChart;
        private Label label2;
    }
}
