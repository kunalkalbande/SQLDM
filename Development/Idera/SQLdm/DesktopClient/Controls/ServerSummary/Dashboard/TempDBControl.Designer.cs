namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class TempDBControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            linearScale1 = new ChartFX.WinForms.Gauge.LinearScale();
            ChartFX.WinForms.Gauge.Marker marker1 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Marker marker2 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Title title1 = new ChartFX.WinForms.Gauge.Title();
            ChartFX.WinForms.Gauge.Title title2 = new ChartFX.WinForms.Gauge.Title();
            ChartFX.WinForms.Adornments.SolidBackground solidBackground1 = new ChartFX.WinForms.Adornments.SolidBackground();
            ChartFX.WinForms.Adornments.ImageBorder imageBorder1 = new ChartFX.WinForms.Adornments.ImageBorder(ChartFX.WinForms.Adornments.ImageBorderType.Rounded);
            ChartFX.WinForms.Adornments.SolidBackground solidBackground2 = new ChartFX.WinForms.Adornments.SolidBackground();
            ChartFX.WinForms.Adornments.ImageBorder imageBorder2 = new ChartFX.WinForms.Adornments.ImageBorder(ChartFX.WinForms.Adornments.ImageBorderType.Rounded);
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
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
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleCpuAlertsButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleSqlAlertsButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gaugeContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool7 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleQueueLengthAlertsButton", "");
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.queueLengthPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.cleanupGauge = new ChartFX.WinForms.Gauge.VerticalGauge();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pageLifePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.usageChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.areasPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.contentionChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._DashboardControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.statusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.queueLengthPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cleanupGauge)).BeginInit();
            this.pageLifePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).BeginInit();
            this.areasPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentionChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.queueLengthPanel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pageLifePanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.areasPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(512, 101);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // queueLengthPanel
            // 
            this.queueLengthPanel.Controls.Add(this.cleanupGauge);
            this.queueLengthPanel.Controls.Add(this.label1);
            this.queueLengthPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queueLengthPanel.Location = new System.Drawing.Point(432, 0);
            this.queueLengthPanel.Margin = new System.Windows.Forms.Padding(0);
            this.queueLengthPanel.Name = "queueLengthPanel";
            this.queueLengthPanel.Size = new System.Drawing.Size(80, 101);
            this.queueLengthPanel.TabIndex = 6;
            // 
            // cleanupGauge
            // 
            this.cleanupGauge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cleanupGauge.Border.Color = System.Drawing.Color.Transparent;
            this.cleanupGauge.Border.ColorTransparency = ((byte)(0));
            this.cleanupGauge.Border.Glare = false;
            this.cleanupGauge.Border.InsideEffects = false;
            this.cleanupGauge.Border.Style = new ChartFX.WinForms.Gauge.LinearBorderStyle("LinearBorder16");
            this.toolbarsManager.SetContextMenuUltra(this.cleanupGauge, "gaugeContextMenu");
            this.cleanupGauge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cleanupGauge.Location = new System.Drawing.Point(5, 31);
            this.cleanupGauge.Name = "cleanupGauge";
            linearScale1.AutoScaleInterval = null;
            linearScale1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            linearScale1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            marker1.Color = System.Drawing.Color.RoyalBlue;
            marker1.Format.Decimals = 2;
            marker1.Position = ChartFX.WinForms.Gauge.Position.Top;
            marker1.SerializableAnimationSpan = 500;
            marker1.Size = 1.6F;
            marker1.ToolTip = "Cleanup Rate: %v KB\\sec";
            marker1.Value = 100;
            marker1.VerticalPosition = ChartFX.WinForms.Gauge.IndicatorVerticalPosition.BehindTickmarks;
            marker2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(160)))), ((int)(((byte)(48)))));
            marker2.Format.Decimals = 2;
            marker2.Main = false;
            marker2.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker2.SerializableAnimationSpan = 500;
            marker2.Size = 1.6F;
            marker2.ToolTip = "Generation Rate: %v KB\\sec";
            marker2.Value = 0;
            marker2.VerticalPosition = ChartFX.WinForms.Gauge.IndicatorVerticalPosition.BehindTickmarks;
            linearScale1.Indicators.AddRange(new ChartFX.WinForms.Gauge.Indicator[] {
            marker1,
            marker2});
            linearScale1.MaxAlwaysDisplayed = true;
            linearScale1.MinAlwaysDisplayed = true;
            linearScale1.Size = 0.9F;
            linearScale1.Thickness = 0.18F;
            linearScale1.Tickmarks.Format.Decimals = 0;
            linearScale1.Tickmarks.Minor.Visible = false;
            this.cleanupGauge.Scales.AddRange(new ChartFX.WinForms.Gauge.LinearScale[] {
            linearScale1});
            this.cleanupGauge.Size = new System.Drawing.Size(72, 66);
            this.cleanupGauge.TabIndex = 0;
            this.cleanupGauge.Tag = "Version Store Cleanup Rate";
            title1.Angle = 90F;
            title1.Layout.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            title1.Layout.SerializableAnchorPoint = new ChartFX.WinForms.Gauge.Internal.GfxPointF(-0.06F, 0.61F);
            title1.Layout.Target = ChartFX.WinForms.Gauge.LayoutTarget.AnchorPoint;
            title1.Text = "Cleanup";
            title1.Visible = false;
            title2.Angle = 270F;
            title2.Layout.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            title2.Layout.SerializableAnchorPoint = new ChartFX.WinForms.Gauge.Internal.GfxPointF(1.06F, 0.29F);
            title2.Layout.Target = ChartFX.WinForms.Gauge.LayoutTarget.AnchorPoint;
            title2.Text = "Generation";
            title2.Visible = false;
            this.cleanupGauge.Titles.AddRange(new ChartFX.WinForms.Gauge.Title[] {
            title1,
            title2});
            this.cleanupGauge.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gauge_MouseClick);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.Location = new System.Drawing.Point(4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Version Store Cleanup Rate";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pageLifePanel
            // 
            this.pageLifePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pageLifePanel.Controls.Add(this.usageChart);
            this.pageLifePanel.Location = new System.Drawing.Point(0, 0);
            this.pageLifePanel.Margin = new System.Windows.Forms.Padding(0);
            this.pageLifePanel.Name = "pageLifePanel";
            this.pageLifePanel.Size = new System.Drawing.Size(216, 101);
            this.pageLifePanel.TabIndex = 4;
            // 
            // usageChart
            // 
            this.usageChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Area;
            this.usageChart.AllSeries.MarkerSize = ((short)(2));
            this.usageChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            this.usageChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usageChart.AxisY.Title.Text = "MB";
            this.usageChart.BackColor = System.Drawing.Color.White;
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            solidBackground1.Color = System.Drawing.Color.White;
            this.usageChart.Background = solidBackground1;
            imageBorder1.Color = System.Drawing.Color.Transparent;
            this.usageChart.Border = imageBorder1;
            this.usageChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.usageChart, "chartContextMenu");
            this.usageChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.usageChart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            this.usageChart.LegendBox.LineSpacing = 1D;
            this.usageChart.LegendBox.PlotAreaOnly = false;
            this.usageChart.Location = new System.Drawing.Point(-7, -16);
            this.usageChart.Margin = new System.Windows.Forms.Padding(0);
            this.usageChart.Name = "usageChart";
            this.usageChart.Palette = "Schemes.Classic";
            this.usageChart.PlotAreaMargin.Bottom = 1;
            this.usageChart.PlotAreaMargin.Left = 6;
            this.usageChart.PlotAreaMargin.Right = 12;
            this.usageChart.PlotAreaMargin.Top = 4;
            this.usageChart.RandomData.Series = 4;
            this.usageChart.Size = new System.Drawing.Size(230, 125);
            this.usageChart.TabIndex = 1;
            this.usageChart.Tag = "Space Used";
            this.usageChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            // 
            // areasPanel
            // 
            this.areasPanel.Controls.Add(this.contentionChart);
            this.areasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.areasPanel.Location = new System.Drawing.Point(216, 0);
            this.areasPanel.Margin = new System.Windows.Forms.Padding(0);
            this.areasPanel.Name = "areasPanel";
            this.areasPanel.Size = new System.Drawing.Size(216, 101);
            this.areasPanel.TabIndex = 5;
            // 
            // contentionChart
            // 
            this.contentionChart.AllSeries.Border.Visible = false;
            this.contentionChart.AllSeries.Border.Width = ((short)(2));
            this.contentionChart.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.None;
            this.contentionChart.AllSeries.MarkerSize = ((short)(2));
            this.contentionChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentionChart.AxisY.Title.Text = "Wait ms";
            this.contentionChart.BackColor = System.Drawing.Color.White;
            solidBackground2.AssemblyName = "ChartFX.WinForms.Adornments";
            solidBackground2.Color = System.Drawing.Color.White;
            this.contentionChart.Background = solidBackground2;
            imageBorder2.Color = System.Drawing.Color.Transparent;
            this.contentionChart.Border = imageBorder2;
            this.contentionChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.contentionChart, "chartContextMenu");
            this.contentionChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.contentionChart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            this.contentionChart.LegendBox.LineSpacing = 1D;
            this.contentionChart.LegendBox.PlotAreaOnly = false;
            this.contentionChart.Location = new System.Drawing.Point(-2, -18);
            this.contentionChart.Margin = new System.Windows.Forms.Padding(0);
            this.contentionChart.Name = "contentionChart";
            this.contentionChart.Palette = "Schemes.Classic";
            this.contentionChart.PlotAreaMargin.Bottom = 1;
            this.contentionChart.PlotAreaMargin.Left = 6;
            this.contentionChart.PlotAreaMargin.Right = 12;
            this.contentionChart.PlotAreaMargin.Top = 4;
            this.contentionChart.Size = new System.Drawing.Size(224, 127);
            this.contentionChart.TabIndex = 2;
            this.contentionChart.Tag = "Contention";
            this.contentionChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "chartContextMenu";
            buttonTool10.InstanceProps.IsFirstInGroup = true;
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool10,
            buttonTool11,
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
            buttonTool7.SharedPropsInternal.Caption = "Show Help";
            buttonTool8.SharedPropsInternal.Caption = "Show Details";
            buttonTool9.SharedPropsInternal.Caption = "Configure Alerts...";
            stateButtonTool3.SharedPropsInternal.Caption = "Display Total Alerts and Baseline";
            stateButtonTool4.SharedPropsInternal.Caption = "Display Sql Alerts and Baseline";
            popupMenuTool2.SharedPropsInternal.Caption = "gaugeContextMenu";
            buttonTool12.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool15,
            buttonTool12,
            buttonTool14});
            stateButtonTool7.SharedPropsInternal.Caption = "Show Alerts and Baseline";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            stateButtonTool2,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            stateButtonTool3,
            stateButtonTool4,
            popupMenuTool2,
            stateButtonTool7});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _DashboardControl_Toolbars_Dock_Area_Top
            // 
            this._DashboardControl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._DashboardControl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 19);
            this._DashboardControl_Toolbars_Dock_Area_Top.Name = "_DashboardControl_Toolbars_Dock_Area_Top";
            this._DashboardControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(512, 0);
            this._DashboardControl_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _DashboardControl_Toolbars_Dock_Area_Bottom
            // 
            this._DashboardControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 120);
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Name = "_DashboardControl_Toolbars_Dock_Area_Bottom";
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(512, 0);
            this._DashboardControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // _DashboardControl_Toolbars_Dock_Area_Left
            // 
            this._DashboardControl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._DashboardControl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 19);
            this._DashboardControl_Toolbars_Dock_Area_Left.Name = "_DashboardControl_Toolbars_Dock_Area_Left";
            this._DashboardControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 101);
            this._DashboardControl_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _DashboardControl_Toolbars_Dock_Area_Right
            // 
            this._DashboardControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._DashboardControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(512, 19);
            this._DashboardControl_Toolbars_Dock_Area_Right.Name = "_DashboardControl_Toolbars_Dock_Area_Right";
            this._DashboardControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 101);
            this._DashboardControl_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Location = new System.Drawing.Point(0, 19);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(512, 101);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Tempdb data is only available on SQL Server 2005 and later";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statusLabel.Visible = false;
            // 
            // TempDBControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Caption = "Tempdb";
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Bottom);
            this.Name = "TempDBControl";
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.statusLabel, 0);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.queueLengthPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cleanupGauge)).EndInit();
            this.pageLifePanel.ResumeLayout(false);
            this.pageLifePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).EndInit();
            this.areasPanel.ResumeLayout(false);
            this.areasPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contentionChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  queueLengthPanel;
        private ChartFX.WinForms.Gauge.VerticalGauge cleanupGauge;
        private ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Bottom;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  pageLifePanel;
        private ChartFX.WinForms.Chart usageChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  areasPanel;
        private ChartFX.WinForms.Chart contentionChart;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusLabel;
        private ChartFX.WinForms.Gauge.LinearScale linearScale1;
    }
}
