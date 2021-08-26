namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class ChartPanel
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
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("selectDataColumnsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showFileActivity");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("selectDataColumnsButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showFileActivity");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.chartContainerPanel = new System.Windows.Forms.Panel();
            this.chart = new ChartFX.WinForms.Chart();
            this.statusLabel = new System.Windows.Forms.Label();
            this.headerStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.selectTypeDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.maximizeChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreChartButton = new System.Windows.Forms.ToolStripButton();
            this.selectViewDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.headerLabel = new System.Windows.Forms.ToolStripLabel();
            this.chartValuesNumericUpDown = new Idera.SQLdm.DesktopClient.Controls.ToolStripNumericUpDown();
            this.chartValuesLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.mainPanel.SuspendLayout();
            this.chartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.headerStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.chartContainerPanel);
            this.mainPanel.Controls.Add(this.headerStrip);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(2);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(400, 258);
            this.mainPanel.TabIndex = 10;
            // 
            // chartContainerPanel
            // 
            this.chartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.chartContainerPanel.Controls.Add(this.chart);
            this.chartContainerPanel.Controls.Add(this.statusLabel);
            this.chartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.chartContainerPanel.Margin = new System.Windows.Forms.Padding(2);
            this.chartContainerPanel.Name = "chartContainerPanel";
            this.chartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.chartContainerPanel.Size = new System.Drawing.Size(400, 239);
            this.chartContainerPanel.TabIndex = 12;
            // 
            // chart
            // 
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.chart.Background = gradientBackground1;
            this.chart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.chart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.chart, "chartContextMenu");
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.LegendBox.Visible = false;
            this.chart.Location = new System.Drawing.Point(1, 0);
            this.chart.Margin = new System.Windows.Forms.Padding(2);
            this.chart.Name = "chart";
            this.chart.Palette = "Schemes.Classic";
            this.chart.PlotAreaColor = System.Drawing.Color.White;
            this.chart.RandomData.Series = 1;
            this.chart.Size = new System.Drawing.Size(398, 238);
            this.chart.TabIndex = 2;
            this.chart.Visible = false;
            this.chart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            this.chart.MouseMove += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseMove);
            this.chart.GetTip += new ChartFX.WinForms.GetTipEventHandler(this.chart_GetTip);
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Location = new System.Drawing.Point(1, 0);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(398, 238);
            this.statusLabel.TabIndex = 12;
            this.statusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // headerStrip
            // 
            this.headerStrip.AutoSize = false;
            this.headerStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.headerStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip.HotTrackEnabled = false;
            this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectTypeDropDownButton,
            this.maximizeChartButton,
            this.restoreChartButton,
            this.selectViewDropDownButton,
            this.headerLabel,
            this.chartValuesNumericUpDown,
            this.chartValuesLabel});
            this.headerStrip.Location = new System.Drawing.Point(0, 0);
            this.headerStrip.Name = "headerStrip";
            this.headerStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.headerStrip.Size = new System.Drawing.Size(400, 19);
            this.headerStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.headerStrip.TabIndex = 1;
            // 
            // selectTypeDropDownButton
            // 
            this.selectTypeDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.selectTypeDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.selectTypeDropDownButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectTypeDropDownButton.Name = "selectTypeDropDownButton";
            this.selectTypeDropDownButton.Size = new System.Drawing.Size(108, 19);
            this.selectTypeDropDownButton.Text = "Select a Chart...";
            this.selectTypeDropDownButton.Visible = false;
            // 
            // maximizeChartButton
            // 
            this.maximizeChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeChartButton.Name = "maximizeChartButton";
            this.maximizeChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeChartButton.ToolTipText = "Maximize";
            this.maximizeChartButton.Visible = false;
            this.maximizeChartButton.Click += new System.EventHandler(this.maximizeChartButton_Click);
            // 
            // restoreChartButton
            // 
            this.restoreChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreChartButton.Name = "restoreChartButton";
            this.restoreChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreChartButton.Text = "Restore";
            this.restoreChartButton.Visible = false;
            this.restoreChartButton.Click += new System.EventHandler(this.restoreChartButton_Click);
            // 
            // selectViewDropDownButton
            // 
            this.selectViewDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.selectViewDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.selectViewDropDownButton.Margin = new System.Windows.Forms.Padding(0);
            this.selectViewDropDownButton.Name = "selectViewDropDownButton";
            this.selectViewDropDownButton.Size = new System.Drawing.Size(103, 19);
            this.selectViewDropDownButton.Text = "Select a View...";
            this.selectViewDropDownButton.Visible = false;
            // 
            // headerLabel
            // 
            this.headerLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(80, 16);
            this.headerLabel.Text = "SQLDM Chart";
            // 
            // chartValuesNumericUpDown
            // 
            this.chartValuesNumericUpDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.chartValuesNumericUpDown.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chartValuesNumericUpDown.ForeColor = System.Drawing.Color.Black;
            this.chartValuesNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 1, 14, 2);
            this.chartValuesNumericUpDown.Name = "chartValuesNumericUpDown";
            this.chartValuesNumericUpDown.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.chartValuesNumericUpDown.Size = new System.Drawing.Size(47, 16);
            this.chartValuesNumericUpDown.Text = "5";
            this.chartValuesNumericUpDown.Visible = false;
            this.chartValuesNumericUpDown.ValueChanged += new System.EventHandler(this.chartValuesNumericUpDown_ValueChanged);
            // 
            // chartValuesLabel
            // 
            this.chartValuesLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.chartValuesLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.chartValuesLabel.Margin = new System.Windows.Forms.Padding(0);
            this.chartValuesLabel.Name = "chartValuesLabel";
            this.chartValuesLabel.Size = new System.Drawing.Size(28, 19);
            this.chartValuesLabel.Text = "Top";
            this.chartValuesLabel.Visible = false;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool1,
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool9,
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
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Filter;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool7.SharedPropsInternal.Caption = "Filter Columns";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ResourcesFileActivity32x32;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool8.SharedPropsInternal.Caption = "Show File Activity";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            stateButtonTool2,
            buttonTool7,
            buttonTool8});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ChartPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChartPanel";
            this.Size = new System.Drawing.Size(400, 258);
            this.mainPanel.ResumeLayout(false);
            this.chartContainerPanel.ResumeLayout(false);
            this.chartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.headerStrip.ResumeLayout(false);
            this.headerStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel chartContainerPanel;
        private System.Windows.Forms.Label statusLabel;
        private HeaderStrip headerStrip;
        private System.Windows.Forms.ToolStripDropDownButton selectTypeDropDownButton;
        private System.Windows.Forms.ToolStripButton maximizeChartButton;
        private System.Windows.Forms.ToolStripButton restoreChartButton;
        private System.Windows.Forms.ToolStripLabel headerLabel;
        private ChartFX.WinForms.Chart chart;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.ToolStripDropDownButton selectViewDropDownButton;
        private ToolStripNumericUpDown chartValuesNumericUpDown;
        private System.Windows.Forms.ToolStripLabel chartValuesLabel;
    }
}
