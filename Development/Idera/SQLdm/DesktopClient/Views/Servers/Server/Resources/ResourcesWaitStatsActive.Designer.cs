using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    partial class ResourcesWaitStatsActive
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
                try
                {
                    // kill the collection
                    IManagementService managementService = ManagementServiceHelper.GetDefaultService();

                    ActiveWaitsConfiguration configuration = new ActiveWaitsConfiguration(InstanceId);

                    configuration.ClientSessionId = ResourcesWaitStatsActive.ClientsessionGuid;

                    managementService.StopWaitingForActiveWaits(configuration);
                }
                catch {}

                if (components != null)
                {
                    components.Dispose();
                }
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab8 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewFullGroupByTextButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryHistoryButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("viewFullGroupByTextButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("showQueryHistoryButton");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourcesWaitStatsActive));
            this.ultraTabStripControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabStripControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.chart = new ChartFX.WinForms.Chart();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ResourcesWaitStatsActive_Fill_Panel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.queryWaitsOverTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryWaitsByDurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breadCrumb1 = new Idera.SQLdm.DesktopClient.Controls.BreadCrumb();
            this.headerStrip2 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
        //  this.advancedQueryViewPanel = new System.Windows.Forms.Panel();
        //  this.advancedQueryViewImage = new System.Windows.Forms.PictureBox();
        //  this.advancedQueryViewLabel = new System.Windows.Forms.Label();
            this.operationalStatusPanel = new System.Windows.Forms.Panel();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabStripControl1)).BeginInit();
            this.ultraTabStripControl1.SuspendLayout();
            this.ultraTabSharedControlsPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.ResourcesWaitStatsActive_Fill_Panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.headerStrip1.SuspendLayout();
            this.headerStrip2.SuspendLayout();
         // this.advancedQueryViewPanel.SuspendLayout();
        //  ((System.ComponentModel.ISupportInitialize)(this.advancedQueryViewImage)).BeginInit();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabStripControl1
            // 
            this.ultraTabStripControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabStripControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabStripControl1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraTabStripControl1.Location = new System.Drawing.Point(0, 19);
            this.ultraTabStripControl1.Margin = new System.Windows.Forms.Padding(0);
            this.ultraTabStripControl1.Name = "ultraTabStripControl1";
            this.ultraTabStripControl1.SharedControls.AddRange(new System.Windows.Forms.Control[] {
            this.chart});
            this.ultraTabStripControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabStripControl1.Size = new System.Drawing.Size(800, 591);
            this.ultraTabStripControl1.TabIndex = 0;
            ultraTab8.Text = "Waits";
            ultraTab1.Text = "Statements";
            ultraTab3.Text = "Applications";
            ultraTab4.Text = "Databases";
            ultraTab5.Text = "Clients";
            ultraTab6.Text = "Sessions";
            ultraTab7.Text = "Users";
            this.ultraTabStripControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab8,
            ultraTab1,
            ultraTab3,
            ultraTab4,
            ultraTab5,
            ultraTab6,
            ultraTab7});
            this.ultraTabStripControl1.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.ultraTabStripControl1_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Controls.Add(this.chart);
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(796, 565);
            // 
            // chart
            // 
            this.chart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Gantt;
            this.chart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            this.chart.AxisX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.chart.AxisX.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(118)))), ((int)(((byte)(227)))));
            this.chart.AxisX.Title.Text = "X Axis";
            this.chart.AxisY.Title.Text = "Y Axis";
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.chart.Background = gradientBackground1;
            this.chart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.chart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.chart, "chartContextMenu");
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.LegendBox.ContentLayout = ChartFX.WinForms.ContentLayout.Near;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Margin = new System.Windows.Forms.Padding(0);
            this.chart.Name = "chart";
            this.chart.Palette = "Schemes.Classic";
            this.chart.PlotAreaColor = System.Drawing.Color.White;
            this.chart.Size = new System.Drawing.Size(796, 565);
            this.chart.TabIndex = 2;
            this.chart.MouseClick += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseClick);
            this.chart.MouseDown += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseDown);
            this.chart.MouseMove += new ChartFX.WinForms.HitTestEventHandler(this.chart_MouseMove);
            this.chart.GetTip += new ChartFX.WinForms.GetTipEventHandler(this.chart_GetTip);
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
            buttonTool3,
            buttonTool9,
            buttonTool10});
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
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool7.SharedPropsInternal.Caption = "View Text";
            buttonTool7.SharedPropsInternal.Visible = false;
            buttonTool8.SharedPropsInternal.Caption = "Show Query History";
            buttonTool8.SharedPropsInternal.Visible = false;
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
            // ResourcesWaitStatsActive_Fill_Panel
            // 
            this.ResourcesWaitStatsActive_Fill_Panel.Controls.Add(this.panel1);
            this.ResourcesWaitStatsActive_Fill_Panel.Controls.Add(this.breadCrumb1);
            this.ResourcesWaitStatsActive_Fill_Panel.Controls.Add(this.headerStrip2);
            this.ResourcesWaitStatsActive_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ResourcesWaitStatsActive_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResourcesWaitStatsActive_Fill_Panel.Location = new System.Drawing.Point(0, 27);
            this.ResourcesWaitStatsActive_Fill_Panel.Name = "ResourcesWaitStatsActive_Fill_Panel";
            this.ResourcesWaitStatsActive_Fill_Panel.Size = new System.Drawing.Size(800, 652);
            this.ResourcesWaitStatsActive_Fill_Panel.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ultraTabStripControl1);
            this.panel1.Controls.Add(this.headerStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 610);
            this.panel1.TabIndex = 6;
            // 
            // headerStrip1
            // 
            this.headerStrip1.AutoSize = false;
            this.headerStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.headerStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip1.HotTrackEnabled = false;
            this.headerStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1});
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.headerStrip1.Size = new System.Drawing.Size(800, 19);
            this.headerStrip1.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.headerStrip1.TabIndex = 6;
            this.headerStrip1.Text = "headerStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queryWaitsOverTimeToolStripMenuItem,
            this.queryWaitsByDurationToolStripMenuItem});
            this.toolStripSplitButton1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.toolStripSplitButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(153, 16);
            this.toolStripSplitButton1.Text = "Query Waits Over Time";
            this.toolStripSplitButton1.ToolTipText = "Query Waits Over Time";
            // 
            // queryWaitsOverTimeToolStripMenuItem
            // 
            this.queryWaitsOverTimeToolStripMenuItem.Checked = true;
            this.queryWaitsOverTimeToolStripMenuItem.CheckOnClick = true;
            this.queryWaitsOverTimeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.queryWaitsOverTimeToolStripMenuItem.Name = "queryWaitsOverTimeToolStripMenuItem";
            this.queryWaitsOverTimeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.queryWaitsOverTimeToolStripMenuItem.Text = "Query Waits Over Time";
            this.queryWaitsOverTimeToolStripMenuItem.Click += new System.EventHandler(this.queryWaitsOverTimeToolStripMenuItem_Click);
            // 
            // queryWaitsByDurationToolStripMenuItem
            // 
            this.queryWaitsByDurationToolStripMenuItem.CheckOnClick = true;
            this.queryWaitsByDurationToolStripMenuItem.Name = "queryWaitsByDurationToolStripMenuItem";
            this.queryWaitsByDurationToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.queryWaitsByDurationToolStripMenuItem.Text = "Query Waits by Duration";
            this.queryWaitsByDurationToolStripMenuItem.Click += new System.EventHandler(this.queryWaitsByDurationToolStripMenuItem_Click);
            // 
            // breadCrumb1
            // 
            this.breadCrumb1.Dock = System.Windows.Forms.DockStyle.Top;
            this.breadCrumb1.Location = new System.Drawing.Point(0, 19);
            this.breadCrumb1.Name = "breadCrumb1";
            this.breadCrumb1.Size = new System.Drawing.Size(800, 23);
            this.breadCrumb1.TabIndex = 1;
            this.breadCrumb1.SizeChanged += new System.EventHandler(this.breadCrumb1_SizeChanged);
            // 
            // headerStrip2
            // 
            this.headerStrip2.AutoSize = false;
            this.headerStrip2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.headerStrip2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip2.HotTrackEnabled = false;
            this.headerStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.headerStrip2.Location = new System.Drawing.Point(0, 0);
            this.headerStrip2.Name = "headerStrip2";
            this.headerStrip2.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.headerStrip2.Size = new System.Drawing.Size(800, 19);
            this.headerStrip2.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.headerStrip2.TabIndex = 5;
            this.headerStrip2.Text = "headerStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.toolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(36, 16);
            this.toolStripLabel1.Text = "Filter";
            // 
            // advancedQueryViewPanel SQLdm 10.4 Removal of Web UI
            /*
            this.advancedQueryViewImage.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewPanel.Controls.Add(this.advancedQueryViewImage);
            this.advancedQueryViewPanel.Controls.Add(this.advancedQueryViewLabel);
            this.advancedQueryViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.advancedQueryViewPanel.Location = new System.Drawing.Point(0, 27);
            this.advancedQueryViewPanel.Name = "advancedQueryViewPanel";
            this.advancedQueryViewPanel.Size = new System.Drawing.Size(1073, 27);
            this.advancedQueryViewPanel.TabIndex = 10;
            this.advancedQueryViewPanel.Visible = false;
            // 
            // advancedQueryViewImage
            // 
            this.advancedQueryViewImage.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusInfoSmall;
            this.advancedQueryViewImage.Location = new System.Drawing.Point(7, 5);
            this.advancedQueryViewImage.Name = "advancedQueryViewImage";
            this.advancedQueryViewImage.Size = new System.Drawing.Size(16, 16);
            this.advancedQueryViewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.advancedQueryViewImage.TabStop = false;
            // 
            // advancedQueryViewLabel
            // 
            this.advancedQueryViewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedQueryViewLabel.BackColor = System.Drawing.Color.SkyBlue;
            this.advancedQueryViewLabel.ForeColor = System.Drawing.Color.Black;
            this.advancedQueryViewLabel.Location = new System.Drawing.Point(4, 3);
            this.advancedQueryViewLabel.Name = "advancedQueryViewLabel";
            this.advancedQueryViewLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.advancedQueryViewLabel.Size = new System.Drawing.Size(1065, 21);
            this.advancedQueryViewLabel.TabIndex = 1;
            this.advancedQueryViewLabel.Text = "< Advanced Query status message >";
            this.advancedQueryViewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.advancedQueryViewLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.advancedQueryViewLabel_MouseDown);
            this.advancedQueryViewLabel.MouseEnter += new System.EventHandler(this.advancedQueryViewLabel_MouseEnter);
            this.advancedQueryViewLabel.MouseLeave += new System.EventHandler(this.advancedQueryViewLabel_MouseLeave);
            this.advancedQueryViewLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.advancedQueryViewLabel_MouseUp);
            */ 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(800, 27);
            this.operationalStatusPanel.TabIndex = 19;
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
            this.operationalStatusLabel.Size = new System.Drawing.Size(792, 21);
            this.operationalStatusLabel.TabIndex = 2;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // ResourcesWaitStatsActive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ResourcesWaitStatsActive_Fill_Panel);
       //   this.Controls.Add(this.advancedQueryViewPanel);
            this.Controls.Add(this.operationalStatusPanel);
            this.Name = "QueriesWaitStatsActive";
            this.Size = new System.Drawing.Size(800, 679);
            this.Load += new System.EventHandler(this.ResourcesWaitStatsActive_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabStripControl1)).EndInit();
            this.ultraTabStripControl1.ResumeLayout(false);
            this.ultraTabSharedControlsPage1.ResumeLayout(false);
            this.ultraTabSharedControlsPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResourcesWaitStatsActive_Fill_Panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.headerStrip1.ResumeLayout(false);
            this.headerStrip1.PerformLayout();
            this.headerStrip2.ResumeLayout(false);
            this.headerStrip2.PerformLayout();
         // this.advancedQueryViewPanel.ResumeLayout(false);
         // this.advancedQueryViewPanel.PerformLayout();
         // ((System.ComponentModel.ISupportInitialize)(this.advancedQueryViewImage)).EndInit();
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabStripControl ultraTabStripControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private ChartFX.WinForms.Chart chart;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.Panel ResourcesWaitStatsActive_Fill_Panel;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Idera.SQLdm.DesktopClient.Controls.BreadCrumb breadCrumb1;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.Panel panel1;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem queryWaitsOverTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryWaitsByDurationToolStripMenuItem;
    //  private System.Windows.Forms.Panel advancedQueryViewPanel;
    //  private System.Windows.Forms.PictureBox advancedQueryViewImage;
    //  private System.Windows.Forms.Label advancedQueryViewLabel;
        private System.Windows.Forms.Panel operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private System.Windows.Forms.Label operationalStatusLabel;
    }
}
