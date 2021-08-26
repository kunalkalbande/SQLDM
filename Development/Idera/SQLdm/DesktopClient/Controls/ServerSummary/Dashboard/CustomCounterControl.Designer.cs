namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class CustomCounterControl
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
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("configureCustomCountersButton");
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
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("configureCustomCountersButton");
            ChartFX.WinForms.Adornments.SolidBackground solidBackground1 = new ChartFX.WinForms.Adornments.SolidBackground();
            ChartFX.WinForms.Adornments.ImageBorder imageBorder1 = new ChartFX.WinForms.Adornments.ImageBorder(ChartFX.WinForms.Adornments.ImageBorderType.Rounded);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.counterChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this._DashboardControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DashboardControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.activityPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.statusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.counterChart)).BeginInit();
            this.activityPanel.SuspendLayout();
            this.SuspendLayout();
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
            buttonTool17,
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
            buttonTool16.SharedPropsInternal.Caption = "Configure Custom Counters";
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
            stateButtonTool7,
            buttonTool16});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // counterChart
            // 
            this.counterChart.AllSeries.Border.Visible = false;
            this.counterChart.AllSeries.Border.Width = ((short)(2));
            this.counterChart.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.None;
            this.counterChart.AllSeries.MarkerSize = ((short)(2));
            this.counterChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.counterChart.BackColor = System.Drawing.Color.White;
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            solidBackground1.Color = System.Drawing.Color.White;
            this.counterChart.Background = solidBackground1;
            imageBorder1.Color = System.Drawing.Color.Transparent;
            this.counterChart.Border = imageBorder1;
            this.counterChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.counterChart, "chartContextMenu");
            this.counterChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.counterChart.LegendBox.ContentLayout = ChartFX.WinForms.ContentLayout.Spread;
            this.counterChart.LegendBox.LineSpacing = 1D;
            this.counterChart.Location = new System.Drawing.Point(-2, 0);
            this.counterChart.Margin = new System.Windows.Forms.Padding(0);
            this.counterChart.Name = "counterChart";
            this.counterChart.Palette = "Schemes.Classic";
            this.counterChart.PlotAreaColor = System.Drawing.Color.White;
            this.counterChart.PlotAreaMargin.Bottom = 1;
            this.counterChart.PlotAreaMargin.Left = 30;
            this.counterChart.PlotAreaMargin.Right = 12;
            this.counterChart.PlotAreaMargin.Top = 5;
            this.counterChart.RandomData.Series = 6;
            this.counterChart.Size = new System.Drawing.Size(613, 173);
            this.counterChart.TabIndex = 0;
            this.counterChart.Tag = "Custom Counters";
            this.counterChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            // 
            // _DashboardControl_Toolbars_Dock_Area_Top
            // 
            this._DashboardControl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DashboardControl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.White;
            this._DashboardControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._DashboardControl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DashboardControl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 19);
            this._DashboardControl_Toolbars_Dock_Area_Top.Name = "_DashboardControl_Toolbars_Dock_Area_Top";
            this._DashboardControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(600, 0);
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
            this._DashboardControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(600, 0);
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
            this._DashboardControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(600, 19);
            this._DashboardControl_Toolbars_Dock_Area_Right.Name = "_DashboardControl_Toolbars_Dock_Area_Right";
            this._DashboardControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 165);
            this._DashboardControl_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // activityPanel
            // 
            this.activityPanel.Controls.Add(this.counterChart);
            this.activityPanel.Controls.Add(this.statusLinkLabel);
            this.activityPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activityPanel.Location = new System.Drawing.Point(0, 19);
            this.activityPanel.Margin = new System.Windows.Forms.Padding(0);
            this.activityPanel.Name = "activityPanel";
            this.activityPanel.Size = new System.Drawing.Size(600, 165);
            this.activityPanel.TabIndex = 9;
            // 
            // statusLinkLabel
            // 
            this.statusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(54, 26);
            this.statusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLinkLabel.Name = "statusLinkLabel";
            this.statusLinkLabel.Size = new System.Drawing.Size(600, 165);
            this.statusLinkLabel.TabIndex = 2;
            this.statusLinkLabel.TabStop = true;
            this.statusLinkLabel.Text = "No Custom Counters have been selected for viewing.\r\n\r\nSelect Custom Counters now\r" +
                "\n";
            this.statusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statusLinkLabel.UseCompatibleTextRendering = true;
            this.statusLinkLabel.Visible = false;
            this.statusLinkLabel.VisitedLinkColor = System.Drawing.Color.Blue;
            this.statusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.statusLinkLabel_LinkClicked);
            // 
            // CustomCounterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Caption = "Custom Counters";
            this.Controls.Add(this.activityPanel);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._DashboardControl_Toolbars_Dock_Area_Bottom);
            this.Name = "CustomCounterControl";
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Bottom, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Top, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Right, 0);
            this.Controls.SetChildIndex(this._DashboardControl_Toolbars_Dock_Area_Left, 0);
            this.Controls.SetChildIndex(this.activityPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.counterChart)).EndInit();
            this.activityPanel.ResumeLayout(false);
            this.activityPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DashboardControl_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  activityPanel;
        private ChartFX.WinForms.Chart counterChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel statusLinkLabel;
    }
}
