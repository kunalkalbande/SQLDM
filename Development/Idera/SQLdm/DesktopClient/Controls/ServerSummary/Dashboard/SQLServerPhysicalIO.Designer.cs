namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class SQLServerPhysicalIO
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
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.sqlServerReadsWritesChartStatusLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ultraGridBagLayoutPanel1 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.sqlServerReadsWritesChartTypeButton = new System.Windows.Forms.ToolStripLabel();
            this.maximizeSqlServerReadsWritesChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreSqlServerReadsWritesChartButton = new System.Windows.Forms.ToolStripButton();
            this.sqlServerReadsWritesPanel = new System.Windows.Forms.Panel();
            this.sqlServerReadsWritesHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.sqlServerReadsWritesChartContainerPanel = new System.Windows.Forms.Panel();
            this.sqlServerReadsWritesChart = new ChartFX.WinForms.Chart();
            this.sqlServerReadsWritesPanel.SuspendLayout();
            this.sqlServerReadsWritesChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlServerReadsWritesChart)).BeginInit();
            this.sqlServerReadsWritesHeaderStrip.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).BeginInit();
            //this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            // 
            // ultraGridBagLayoutPanel1
            // 
            this.ultraGridBagLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraGridBagLayoutPanel1.Name = "ultraGridBagLayoutPanel1";
            this.ultraGridBagLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.ultraGridBagLayoutPanel1.TabIndex = 0;
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);


            //this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.Controls.Add(this.sqlServerReadsWritesPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(586, 145);
            // 
            // sqlServerReadsWritesPanel
            // 
            this.sqlServerReadsWritesPanel.Controls.Add(this.sqlServerReadsWritesChartContainerPanel);
            this.sqlServerReadsWritesPanel.Controls.Add(this.sqlServerReadsWritesHeaderStrip);
            this.sqlServerReadsWritesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlServerReadsWritesPanel.Location = new System.Drawing.Point(356, 367);
            this.sqlServerReadsWritesPanel.Name = "sqlServerReadsWritesPanel";
            this.sqlServerReadsWritesPanel.Size = new System.Drawing.Size(500, 200);
            this.sqlServerReadsWritesPanel.TabIndex = 5;

            // 
            // sqlServerReadsWritesChartContainerPanel
            // 
            this.sqlServerReadsWritesChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.sqlServerReadsWritesChartContainerPanel.Controls.Add(this.sqlServerReadsWritesChart);
            this.sqlServerReadsWritesChartContainerPanel.Controls.Add(this.sqlServerReadsWritesChartStatusLabel);
            this.sqlServerReadsWritesChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlServerReadsWritesChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.sqlServerReadsWritesChartContainerPanel.Name = "sqlServerReadsWritesChartContainerPanel";
            this.sqlServerReadsWritesChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.sqlServerReadsWritesChartContainerPanel.Size = new System.Drawing.Size(347, 157);
            this.sqlServerReadsWritesChartContainerPanel.TabIndex = 12;

            // 
            // sqlServerReadsWritesChart
            // 
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.sqlServerReadsWritesChart.Background = gradientBackground1;
            this.sqlServerReadsWritesChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.sqlServerReadsWritesChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.sqlServerReadsWritesChart, "chartContextMenu");
            this.sqlServerReadsWritesChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sqlServerReadsWritesChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlServerReadsWritesChart.Location = new System.Drawing.Point(1, 0);
            this.sqlServerReadsWritesChart.Name = "sqlServerReadsWritesChart";
            this.sqlServerReadsWritesChart.Palette = "Schemes.Classic";
            this.sqlServerReadsWritesChart.PlotAreaColor = System.Drawing.Color.White;
            this.sqlServerReadsWritesChart.Size = new System.Drawing.Size(345, 156);
            this.sqlServerReadsWritesChart.TabIndex = 13;
            this.sqlServerReadsWritesChart.Visible = true;
            this.sqlServerReadsWritesChart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.OnChartMouseClick);

            // 
            // sqlServerReadsWritesChartStatusLabel
            // 
            this.sqlServerReadsWritesChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.sqlServerReadsWritesChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlServerReadsWritesChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.sqlServerReadsWritesChartStatusLabel.Name = "sqlServerReadsWritesChartStatusLabel";
            this.sqlServerReadsWritesChartStatusLabel.Size = new System.Drawing.Size(345, 156);
            this.sqlServerReadsWritesChartStatusLabel.TabIndex = 12;
            this.sqlServerReadsWritesChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.sqlServerReadsWritesChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // sqlServerReadsWritesHeaderStrip
            // 
            this.sqlServerReadsWritesHeaderStrip.AutoSize = false;
            this.sqlServerReadsWritesHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.sqlServerReadsWritesHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.sqlServerReadsWritesHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.sqlServerReadsWritesHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.sqlServerReadsWritesHeaderStrip.HotTrackEnabled = false;
            this.sqlServerReadsWritesHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sqlServerReadsWritesChartTypeButton,
            this.maximizeSqlServerReadsWritesChartButton,
            this.restoreSqlServerReadsWritesChartButton});
            this.sqlServerReadsWritesHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.sqlServerReadsWritesHeaderStrip.Name = "sqlServerReadsWritesHeaderStrip";
            this.sqlServerReadsWritesHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.sqlServerReadsWritesHeaderStrip.Size = new System.Drawing.Size(347, 19);
            this.sqlServerReadsWritesHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.sqlServerReadsWritesHeaderStrip.TabIndex = 10;

            // 
            // sqlServerReadsWritesChartTypeButton
            // 
            this.sqlServerReadsWritesChartTypeButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlServerReadsWritesChartTypeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.sqlServerReadsWritesChartTypeButton.Name = "sqlServerReadsWritesChartTypeButton";
            this.sqlServerReadsWritesChartTypeButton.Size = new System.Drawing.Size(140, 16); // 140
            this.sqlServerReadsWritesChartTypeButton.Text = "SQL Server Physical I/O";

            // 
            // maximizeSqlServerReadsWritesChartButton
            // 
            this.maximizeSqlServerReadsWritesChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeSqlServerReadsWritesChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeSqlServerReadsWritesChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeSqlServerReadsWritesChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeSqlServerReadsWritesChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeSqlServerReadsWritesChartButton.Name = "maximizeSqlServerReadsWritesChartButton";
            this.maximizeSqlServerReadsWritesChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeSqlServerReadsWritesChartButton.ToolTipText = "Maximize";
            //this.maximizeSqlServerReadsWritesChartButton.Click += new System.EventHandler(this.maximizeSqlServerReadsWritesChartButton_Click);

            // 
            // restoreSqlServerReadsWritesChartButton
            // 
            this.restoreSqlServerReadsWritesChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreSqlServerReadsWritesChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreSqlServerReadsWritesChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreSqlServerReadsWritesChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreSqlServerReadsWritesChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreSqlServerReadsWritesChartButton.Name = "restoreSqlServerReadsWritesChartButton";
            this.restoreSqlServerReadsWritesChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreSqlServerReadsWritesChartButton.Text = "Restore";
            this.restoreSqlServerReadsWritesChartButton.Visible = true;
            //this.restoreSqlServerReadsWritesChartButton.Click += new System.EventHandler(this.restoreSqlServerReadsWritesChartButton_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Caption = "SQL Server Physical I/O";
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SQLServerPhysicalIOControl";
            this.Size = new System.Drawing.Size(586, 164);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.sqlServerReadsWritesPanel.ResumeLayout(false);
            this.sqlServerReadsWritesChartContainerPanel.ResumeLayout(false);
            this.sqlServerReadsWritesChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlServerReadsWritesChart)).EndInit();
            this.sqlServerReadsWritesHeaderStrip.ResumeLayout(false);
            this.sqlServerReadsWritesHeaderStrip.PerformLayout();
            // this.toolbarsManager.SetContextMenuUltra(this.sqlServerReadsWritesChart, "ChartContextMenu");
            // this.toolbarsManager.SetContextMenuUltra(this.sqlServerReadsWritesChart, "ChartContextMenu");
            // 
            // toolbarsManager
            // 
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
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
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showHelpButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showDetailsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("configureAlertsButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            //  ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            
            popupMenuTool1.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
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
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            stateButtonTool2});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);



            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";

            //  this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            //  this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();


        }
        #endregion
        private System.Windows.Forms.Panel sqlServerReadsWritesPanel;
        private System.Windows.Forms.Panel sqlServerReadsWritesChartContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip sqlServerReadsWritesHeaderStrip;
        private ChartFX.WinForms.Chart sqlServerReadsWritesChart;
        private System.Windows.Forms.Label sqlServerReadsWritesChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.ToolStripLabel sqlServerReadsWritesChartTypeButton;
        private System.Windows.Forms.ToolStripButton maximizeSqlServerReadsWritesChartButton;
        private System.Windows.Forms.ToolStripButton restoreSqlServerReadsWritesChartButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel1;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;

    }
}
