namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class DatabaseFileActivity
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
            ChartFX.WinForms.Adornments.SolidBackground solidBackground1 = new ChartFX.WinForms.Adornments.SolidBackground();
            ChartFX.WinForms.SeriesAttributes seriesAttributes1 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes2 = new ChartFX.WinForms.SeriesAttributes();
            ChartFX.WinForms.SeriesAttributes seriesAttributes3 = new ChartFX.WinForms.SeriesAttributes();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("expandAllDb");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("expandAllFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("collapseAllDb");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("collapseAllFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("navigateToDisksView");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("navigateToDatabaseFilesView");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("expandAllDb");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("collapseAllDb");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("expandAllFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("collapseAllFiles");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("navigateToDisksView");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("navigateToDatabaseFilesView");
            this.headerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.writeActivityIndicator = new System.Windows.Forms.PictureBox();
            this.readActivityIndicator = new System.Windows.Forms.PictureBox();
            this.fileTypeIndicator = new System.Windows.Forms.PictureBox();
            this.databaseName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databaseFileName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chartGroupPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.pbWriteTrend = new System.Windows.Forms.PictureBox();
            this.pbReadTrend = new System.Windows.Forms.PictureBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.writeActivity = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.readActivity = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chart1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.miniActivityChartGroup = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.miniChartWritesBar = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.miniChartReadsBar = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.writeActivityIndicator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.readActivityIndicator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileTypeIndicator)).BeginInit();
            this.chartGroupPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWriteTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReadTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.miniActivityChartGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.Transparent;
            this.toolbarsManager.SetContextMenuUltra(this.headerPanel, "chartContextMenu");
            this.headerPanel.Controls.Add(this.writeActivityIndicator);
            this.headerPanel.Controls.Add(this.readActivityIndicator);
            this.headerPanel.Controls.Add(this.fileTypeIndicator);
            this.headerPanel.Controls.Add(this.databaseName);
            this.headerPanel.Controls.Add(this.databaseFileName);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(400, 40);
            this.headerPanel.TabIndex = 0;
            this.headerPanel.DoubleClick += new System.EventHandler(this.Toggle_DoubleClick);
            // 
            // writeActivityIndicator
            // 
            this.writeActivityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.writeActivityIndicator.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.DiskDriveNoActivity_16x16;
            this.writeActivityIndicator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolbarsManager.SetContextMenuUltra(this.writeActivityIndicator, "chartContextMenu");
            this.writeActivityIndicator.Location = new System.Drawing.Point(381, 6);
            this.writeActivityIndicator.Name = "writeActivityIndicator";
            this.writeActivityIndicator.Size = new System.Drawing.Size(12, 12);
            this.writeActivityIndicator.TabIndex = 4;
            this.writeActivityIndicator.TabStop = false;
            // 
            // readActivityIndicator
            // 
            this.readActivityIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.readActivityIndicator.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.DiskDriveNoActivity_16x16;
            this.readActivityIndicator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolbarsManager.SetContextMenuUltra(this.readActivityIndicator, "chartContextMenu");
            this.readActivityIndicator.Location = new System.Drawing.Point(364, 6);
            this.readActivityIndicator.Name = "readActivityIndicator";
            this.readActivityIndicator.Size = new System.Drawing.Size(12, 12);
            this.readActivityIndicator.TabIndex = 3;
            this.readActivityIndicator.TabStop = false;
            // 
            // fileTypeIndicator
            // 
            this.fileTypeIndicator.BackColor = System.Drawing.Color.Transparent;
            this.fileTypeIndicator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.toolbarsManager.SetContextMenuUltra(this.fileTypeIndicator, "chartContextMenu");
            this.fileTypeIndicator.Dock = System.Windows.Forms.DockStyle.Left;
            this.fileTypeIndicator.Location = new System.Drawing.Point(0, 0);
            this.fileTypeIndicator.Name = "fileTypeIndicator";
            this.fileTypeIndicator.Size = new System.Drawing.Size(29, 40);
            this.fileTypeIndicator.TabIndex = 2;
            this.fileTypeIndicator.TabStop = false;
            // 
            // databaseName
            // 
            this.databaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseName.AutoEllipsis = true;
            this.databaseName.BackColor = System.Drawing.Color.Transparent;
            this.toolbarsManager.SetContextMenuUltra(this.databaseName, "chartContextMenu");
            this.databaseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseName.ForeColor = System.Drawing.Color.DarkGray;
            this.databaseName.Location = new System.Drawing.Point(36, 23);
            this.databaseName.Name = "databaseName";
            this.databaseName.Size = new System.Drawing.Size(320, 13);
            this.databaseName.TabIndex = 1;
            this.databaseName.Text = "label2";
            this.databaseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.databaseName.DoubleClick += new System.EventHandler(this.Toggle_DoubleClick);
            // 
            // databaseFileName
            // 
            this.databaseFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseFileName.BackColor = System.Drawing.Color.Transparent;
            this.toolbarsManager.SetContextMenuUltra(this.databaseFileName, "chartContextMenu");
            this.databaseFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseFileName.ForeColor = System.Drawing.Color.Black;
            this.databaseFileName.Location = new System.Drawing.Point(35, 3);
            this.databaseFileName.Name = "databaseFileName";
            this.databaseFileName.Size = new System.Drawing.Size(319, 16);
            this.databaseFileName.TabIndex = 0;
            this.databaseFileName.Text = "label1";
            this.databaseFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.databaseFileName.DoubleClick += new System.EventHandler(this.Toggle_DoubleClick);
            // 
            // chartGroupPanel
            // 
            this.chartGroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chartGroupPanel.BackColor = System.Drawing.Color.Transparent;
            this.chartGroupPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chartGroupPanel.Controls.Add(this.pbWriteTrend);
            this.chartGroupPanel.Controls.Add(this.pbReadTrend);
            this.chartGroupPanel.Controls.Add(this.label2);
            this.chartGroupPanel.Controls.Add(this.label1);
            this.chartGroupPanel.Controls.Add(this.writeActivity);
            this.chartGroupPanel.Controls.Add(this.readActivity);
            this.chartGroupPanel.Controls.Add(this.chart1);
            this.chartGroupPanel.Location = new System.Drawing.Point(0, 46);
            this.chartGroupPanel.Name = "chartGroupPanel";
            this.chartGroupPanel.Size = new System.Drawing.Size(400, 238);
            this.chartGroupPanel.TabIndex = 1;
            // 
            // pbWriteTrend
            // 
            this.pbWriteTrend.Location = new System.Drawing.Point(282, 211);
            this.pbWriteTrend.Name = "pbWriteTrend";
            this.pbWriteTrend.Size = new System.Drawing.Size(16, 16);
            this.pbWriteTrend.TabIndex = 6;
            this.pbWriteTrend.TabStop = false;
            // 
            // pbReadTrend
            // 
            this.pbReadTrend.Location = new System.Drawing.Point(71, 211);
            this.pbReadTrend.Name = "pbReadTrend";
            this.pbReadTrend.Size = new System.Drawing.Size(16, 16);
            this.pbReadTrend.TabIndex = 5;
            this.pbReadTrend.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Reads / Sec:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(215, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Writes / Sec:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // writeActivity
            // 
            this.writeActivity.Location = new System.Drawing.Point(301, 206);
            this.writeActivity.Name = "writeActivity";
            this.writeActivity.Size = new System.Drawing.Size(82, 25);
            this.writeActivity.TabIndex = 1;
            this.writeActivity.Text = "{0}";
            this.writeActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // readActivity
            // 
            this.readActivity.Location = new System.Drawing.Point(89, 206);
            this.readActivity.Name = "readActivity";
            this.readActivity.Size = new System.Drawing.Size(82, 25);
            this.readActivity.TabIndex = 0;
            this.readActivity.Text = "{0}";
            this.readActivity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chart1
            // 
            this.chart1.AllSeries.Gallery = ChartFX.WinForms.Gallery.Area;
            this.chart1.AllSeries.MarkerShape = ChartFX.WinForms.MarkerShape.HorizontalLine;
            this.chart1.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.AxesStyle = ChartFX.WinForms.AxesStyle.None;
            this.chart1.AxisX.Title.Text = "X Axis";
            this.chart1.AxisX.Visible = false;
            this.chart1.AxisY.Title.Text = "Y Axis";
            this.chart1.AxisY.Visible = false;
            solidBackground1.AssemblyName = "ChartFX.WinForms.Adornments";
            this.chart1.Background = solidBackground1;
            this.chart1.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.Color, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.chart1.ContextMenus = false;
            this.chart1.LegendBox.Border = ChartFX.WinForms.DockBorder.None;
            this.chart1.LegendBox.Visible = false;
            this.chart1.Location = new System.Drawing.Point(7, 3);
            this.chart1.Name = "chart1";
            this.chart1.PlotAreaMargin.Bottom = -1;
            this.chart1.PlotAreaMargin.Left = -1;
            this.chart1.PlotAreaMargin.Right = -1;
            this.chart1.PlotAreaMargin.Top = -1;
            seriesAttributes1.MarkerShape = ChartFX.WinForms.MarkerShape.HorizontalLine;
            seriesAttributes1.Pattern = System.Drawing.Drawing2D.HatchStyle.Horizontal;
            this.chart1.Series.AddRange(new ChartFX.WinForms.SeriesAttributes[] {
            seriesAttributes1,
            seriesAttributes2,
            seriesAttributes3});
            this.chart1.Size = new System.Drawing.Size(385, 200);
            this.chart1.TabIndex = 2;
            // 
            // miniActivityChartGroup
            // 
            this.miniActivityChartGroup.BackColor = System.Drawing.Color.White;
            this.miniActivityChartGroup.Controls.Add(this.miniChartWritesBar);
            this.miniActivityChartGroup.Controls.Add(this.miniChartReadsBar);
            this.miniActivityChartGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.miniActivityChartGroup.Location = new System.Drawing.Point(0, 290);
            this.miniActivityChartGroup.Name = "miniActivityChartGroup";
            this.miniActivityChartGroup.Size = new System.Drawing.Size(400, 10);
            this.miniActivityChartGroup.TabIndex = 2;
            this.miniActivityChartGroup.Visible = false;
            // 
            // miniChartWritesBar
            // 
            this.miniChartWritesBar.BackColor = System.Drawing.Color.RoyalBlue;
            this.miniChartWritesBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.miniChartWritesBar.Location = new System.Drawing.Point(78, 0);
            this.miniChartWritesBar.Name = "miniChartWritesBar";
            this.miniChartWritesBar.Size = new System.Drawing.Size(78, 10);
            this.miniChartWritesBar.TabIndex = 1;
            // 
            // miniChartReadsBar
            // 
            this.miniChartReadsBar.BackColor = System.Drawing.Color.IndianRed;
            this.miniChartReadsBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.miniChartReadsBar.Location = new System.Drawing.Point(0, 0);
            this.miniChartReadsBar.Name = "miniChartReadsBar";
            this.miniChartReadsBar.Size = new System.Drawing.Size(78, 10);
            this.miniChartReadsBar.TabIndex = 0;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedProps.Caption = "chartContextMenu";
            buttonTool1.InstanceProps.IsFirstInGroup = true;
            buttonTool10.InstanceProps.IsFirstInGroup = true;
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool12,
            buttonTool10,
            buttonTool8,
            buttonTool13,
            buttonTool15});
            buttonTool4.SharedProps.Caption = "Expand All DB";
            buttonTool5.SharedProps.Caption = "Collapse All DB";
            buttonTool6.SharedProps.Caption = "Expand All Files";
            buttonTool2.SharedProps.Caption = "Collapse All Files";
            buttonTool3.SharedProps.Caption = "Show Disks View";
            buttonTool7.SharedProps.Caption = "Show Database Files View";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool4,
            buttonTool5,
            buttonTool6,
            buttonTool2,
            buttonTool3,
            buttonTool7});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            // 
            // _DatabaseFileActivity_Toolbars_Dock_Area_Left
            // 
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.Transparent;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.Name = "_DatabaseFileActivity_Toolbars_Dock_Area_Left";
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 300);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _DatabaseFileActivity_Toolbars_Dock_Area_Right
            // 
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.Transparent;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(400, 0);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.Name = "_DatabaseFileActivity_Toolbars_Dock_Area_Right";
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 300);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _DatabaseFileActivity_Toolbars_Dock_Area_Top
            // 
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.Transparent;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.Name = "_DatabaseFileActivity_Toolbars_Dock_Area_Top";
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(400, 0);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _DatabaseFileActivity_Toolbars_Dock_Area_Bottom
            // 
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.Transparent;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 300);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.Name = "_DatabaseFileActivity_Toolbars_Dock_Area_Bottom";
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(400, 0);
            this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // DatabaseFileActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.miniActivityChartGroup);
            this.Controls.Add(this.chartGroupPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this._DatabaseFileActivity_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._DatabaseFileActivity_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._DatabaseFileActivity_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._DatabaseFileActivity_Toolbars_Dock_Area_Bottom);
            this.DoubleBuffered = true;
            this.Name = "DatabaseFileActivity";
            this.Size = new System.Drawing.Size(400, 300);
            this.Load += new System.EventHandler(this.DatabaseFileActivity_Load);
            this.VisibleChanged += new System.EventHandler(this.DatabaseFileActivity_VisibleChanged);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.writeActivityIndicator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.readActivityIndicator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileTypeIndicator)).EndInit();
            this.chartGroupPanel.ResumeLayout(false);
            this.chartGroupPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWriteTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReadTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.miniActivityChartGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  headerPanel;
        private System.Windows.Forms.PictureBox fileTypeIndicator;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databaseName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databaseFileName;
        private System.Windows.Forms.PictureBox writeActivityIndicator;
        private System.Windows.Forms.PictureBox readActivityIndicator;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  chartGroupPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  miniActivityChartGroup;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel writeActivity;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel readActivity;
        private ChartFX.WinForms.Chart chart1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  miniChartWritesBar;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  miniChartReadsBar;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DatabaseFileActivity_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DatabaseFileActivity_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DatabaseFileActivity_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DatabaseFileActivity_Toolbars_Dock_Area_Bottom;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private System.Windows.Forms.PictureBox pbWriteTrend;
        private System.Windows.Forms.PictureBox pbReadTrend;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;

    }
}
