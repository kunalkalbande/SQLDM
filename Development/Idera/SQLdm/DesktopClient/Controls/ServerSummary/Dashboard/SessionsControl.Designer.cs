using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class SessionsControl
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
            ChartFX.WinForms.Adornments.ImageBorder imageBorder1 = new ChartFX.WinForms.Adornments.ImageBorder(ChartFX.WinForms.Adornments.ImageBorderType.Rounded);
            ChartFX.WinForms.SeriesAttributes seriesAttributes1 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes2 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes3 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes4 = new ChartFX.WinForms.SeriesAttributes();
            linearScale1 = new ChartFX.WinForms.Gauge.LinearScale();
            ChartFX.WinForms.Gauge.Marker marker1 = new ChartFX.WinForms.Gauge.Marker();
            ChartFX.WinForms.Gauge.Title title1 = new ChartFX.WinForms.Gauge.Title();
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
            this.sessionsChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.queueLengthPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel(true);
            this.clientComputersGauge = new ChartFX.WinForms.Gauge.VerticalGauge();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel(true);
            this.sessionsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._DashboardControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.sessionsChart)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.queueLengthPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientComputersGauge)).BeginInit();
            this.sessionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // sessionsChart
            // 
            this.sessionsChart.AllSeries.Border.Visible = false;
            this.sessionsChart.AllSeries.Border.Width = ((short)(2));
            this.sessionsChart.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.None;
            this.sessionsChart.AllSeries.MarkerSize = ((short)(2));
            this.sessionsChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionsChart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            this.sessionsChart.Background = solidBackground1;
            imageBorder1.Color = System.Drawing.Color.Transparent;
            this.sessionsChart.Border = imageBorder1;
            this.sessionsChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.sessionsChart, "chartContextMenu");
            this.sessionsChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sessionsChart.LegendBox.Border = ChartFX.WinForms.DockBorder.None;
            this.sessionsChart.LegendBox.ContentLayout = ChartFX.WinForms.ContentLayout.Spread;
            this.sessionsChart.LegendBox.Dock = ChartFX.WinForms.DockArea.Top;
            this.sessionsChart.LegendBox.LineSpacing = 1D;
            this.sessionsChart.LegendBox.PlotAreaOnly = false;
            this.sessionsChart.Location = new System.Drawing.Point(-3, -14);
            this.sessionsChart.Margin = new System.Windows.Forms.Padding(0);
            this.sessionsChart.Name = "sessionsChart";
            this.sessionsChart.Palette = "Schemes.Classic";
            this.sessionsChart.PlotAreaMargin.Bottom = 1;
            this.sessionsChart.PlotAreaMargin.Left = 6;
            this.sessionsChart.PlotAreaMargin.Right = 12;
            this.sessionsChart.PlotAreaMargin.Top = 4;
            this.sessionsChart.RandomData.Series = 4;
            seriesAttributes1.AlternateColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(181)))));
            seriesAttributes1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(0)))), ((int)(((byte)(145)))));
            seriesAttributes2.AlternateColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(0)))));
            seriesAttributes2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(155)))), ((int)(((byte)(0)))));
            seriesAttributes3.AlternateColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(200)))), ((int)(((byte)(60)))));
            seriesAttributes3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(160)))), ((int)(((byte)(48)))));
            seriesAttributes4.AlternateColor = System.Drawing.Color.Transparent;
            seriesAttributes4.BarShape = ChartFX.WinForms.BarShape.Rectangle;
            seriesAttributes4.Border.Visible = true;
            seriesAttributes4.Border.Width = ((short)(1));
            seriesAttributes4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(124)))), ((int)(((byte)(209)))));
            seriesAttributes4.FillMode = ChartFX.WinForms.FillMode.Gradient;
            seriesAttributes4.Gallery = ChartFX.WinForms.Gallery.Area;
            seriesAttributes4.Line.Width = ((short)(1));
            this.sessionsChart.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes1,
            seriesAttributes2,
            seriesAttributes3,
            seriesAttributes4});
            this.sessionsChart.Size = new System.Drawing.Size(560, 187);
            this.sessionsChart.TabIndex = 3;
            this.sessionsChart.Tag = "Active & Blocked Sessions";
            this.sessionsChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.queueLengthPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sessionsPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(631, 165);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // queueLengthPanel
            // 
            this.queueLengthPanel.Controls.Add(this.clientComputersGauge);
            this.queueLengthPanel.Controls.Add(this.label1);
            this.queueLengthPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queueLengthPanel.Location = new System.Drawing.Point(0, 0);
            this.queueLengthPanel.Margin = new System.Windows.Forms.Padding(0);
            this.queueLengthPanel.Name = "queueLengthPanel";
            this.queueLengthPanel.Size = new System.Drawing.Size(80, 165);
            this.queueLengthPanel.TabIndex = 7;
            // 
            // clientComputersGauge
            // 
            this.clientComputersGauge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.clientComputersGauge.Border.Color = System.Drawing.Color.Transparent;
            this.clientComputersGauge.Border.ColorTransparency = ((byte)(0));
            this.clientComputersGauge.Border.Glare = false;
            this.clientComputersGauge.Border.InsideEffects = false;
            this.clientComputersGauge.Border.Style = new ChartFX.WinForms.Gauge.LinearBorderStyle("LinearBorder14");
            this.toolbarsManager.SetContextMenuUltra(this.clientComputersGauge, "gaugeContextMenu");
            this.clientComputersGauge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clientComputersGauge.Location = new System.Drawing.Point(5, 32);
            this.clientComputersGauge.Name = "clientComputersGauge";
            linearScale1.AutoScaleInterval = "0, 30";
            linearScale1.Bar.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            linearScale1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(225)))), ((int)(((byte)(243)))));
            marker1.Color = System.Drawing.Color.Green;
            marker1.Position = ChartFX.WinForms.Gauge.Position.Bottom;
            marker1.SerializableAnimationSpan = 500;
            marker1.Size = 1.7F;
            marker1.Value = 25;
            marker1.VerticalPosition = ChartFX.WinForms.Gauge.IndicatorVerticalPosition.BehindTickmarks;
            linearScale1.Indicators.AddRange(new ChartFX.WinForms.Gauge.Indicator[] {
            marker1});
            linearScale1.Layout.Alignment = System.Drawing.ContentAlignment.TopCenter;
            linearScale1.Size = 0.9F;
            linearScale1.Thickness = 0.18F;
            linearScale1.Tickmarks.Format.Decimals = 0;
            this.clientComputersGauge.Scales.AddRange(new ChartFX.WinForms.Gauge.LinearScale[] {
            linearScale1});
            this.clientComputersGauge.Size = new System.Drawing.Size(72, 130);
            this.clientComputersGauge.TabIndex = 0;
            this.clientComputersGauge.Titles.AddRange(new ChartFX.WinForms.Gauge.Title[] {
            title1});
            this.clientComputersGauge.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gauge_MouseClick);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.Location = new System.Drawing.Point(4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Client Computers";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // sessionsPanel
            // 
            this.sessionsPanel.Controls.Add(this.sessionsChart);
            this.sessionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsPanel.Location = new System.Drawing.Point(80, 0);
            this.sessionsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.sessionsPanel.Name = "sessionsPanel";
            this.sessionsPanel.Size = new System.Drawing.Size(551, 165);
            this.sessionsPanel.TabIndex = 4;
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
            this._DashboardControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(631, 0);
            this._DashboardControl_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _DashboardControl_Toolbars_Dock_Area_Bottom
            // 
            this._DashboardControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 184);
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Name = "_DashboardControl_Toolbars_Dock_Area_Bottom";
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(631, 0);
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
            this._DashboardControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 165);
            this._DashboardControl_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _DashboardControl_Toolbars_Dock_Area_Right
            // 
            this._DashboardControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._DashboardControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(631, 19);
            this._DashboardControl_Toolbars_Dock_Area_Right.Name = "_DashboardControl_Toolbars_Dock_Area_Right";
            this._DashboardControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 165);
            this._DashboardControl_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // SessionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Caption = "Sessions";
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Bottom);
            this.Name = "SessionsControl";
            this.Size = new System.Drawing.Size(631, 184);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.sessionsChart)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.queueLengthPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.clientComputersGauge)).EndInit();
            this.sessionsPanel.ResumeLayout(false);
            this.sessionsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
        private ChartFX.WinForms.Chart sessionsChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sessionsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  queueLengthPanel;
        private ChartFX.WinForms.Gauge.VerticalGauge clientComputersGauge;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private ChartFX.WinForms.Gauge.LinearScale linearScale1;
    }
}
