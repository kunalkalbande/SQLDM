using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Logs
{
    partial class LogsView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Current - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Archive #1 - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Archive #2 - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("SQL Server", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Current - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Archive #1 - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Archive #2 - <Date\\Time>");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("SQL Server Agent", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7});
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MessageType", -1, 1290415313);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogsView));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Local Time", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Source");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message Number");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Source", -1, 1300419016);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Name", -1, 1295464844);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(1290415313);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(1295464844);
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(1300419016);
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("treeContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("refreshTreeButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("refreshTreeButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("editAlertsConfigurationButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("editAlertsConfigurationButton");
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.availableLogsContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.availableLogsTreeView = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTreeView();
            this.availableLogsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.hideAvailableLogsPanel = new System.Windows.Forms.ToolStripButton();
            this.availableLogsHeaderLabel = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.logGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.logProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.logGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.detailsContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.detailsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.detailsLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.boundMessageTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundMessageNumberLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.messageNumberLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dateLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.severityLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundDateLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sourceLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundSourceLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.detailsLogGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.boundLogFileLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundLogNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundLogTypeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logFileLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logTypeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundSeverityPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.boundSeverityLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.boundSeverityPictureBox = new System.Windows.Forms.PictureBox();
            this.detailsPanelLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.detailsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.hideDetailsPanelButton = new System.Windows.Forms.ToolStripButton();
            this.detailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.lblLogLimitExceeded = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.LogsView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.informationLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.availableLogsContainerPanel.SuspendLayout();
            this.availableLogsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logGrid)).BeginInit();
            this.detailsContainerPanel.SuspendLayout();
            this.detailsPanel.SuspendLayout();
            this.detailsLayoutPanel.SuspendLayout();
            this.detailsLogGroupBox.SuspendLayout();
            this.boundSeverityPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boundSeverityPictureBox)).BeginInit();
            this.detailsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.LogsView_Fill_Panel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 26);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Panel1.Controls.Add(this.availableLogsContainerPanel);
            this.splitContainer1.Panel1.Controls.Add(this.availableLogsHeaderStrip);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.lblLogLimitExceeded);
            this.splitContainer1.Size = new System.Drawing.Size(600, 433);
            this.splitContainer1.SplitterDistance = 202;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_MouseDown);
            this.splitContainer1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_MouseUp);
            // 
            // availableLogsContainerPanel
            // 
            this.availableLogsContainerPanel.Controls.Add(this.availableLogsTreeView);
            this.availableLogsContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableLogsContainerPanel.Location = new System.Drawing.Point(0, 25);
            this.availableLogsContainerPanel.Name = "availableLogsContainerPanel";
            this.availableLogsContainerPanel.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.availableLogsContainerPanel.Size = new System.Drawing.Size(202, 408);
            this.availableLogsContainerPanel.TabIndex = 17;
            // 
            // availableLogsTreeView
            // 
            this.availableLogsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.availableLogsTreeView.CheckBoxes = true;
            this.availableLogsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableLogsTreeView.FullRowSelect = true;
            this.availableLogsTreeView.Location = new System.Drawing.Point(0, 0);
            this.availableLogsTreeView.Name = "availableLogsTreeView";
            treeNode1.Name = "Node2";
            treeNode1.Text = "Current - <Date\\Time>";
            treeNode2.Name = "Node4";
            treeNode2.Text = "Archive #1 - <Date\\Time>";
            treeNode3.Name = "Node5";
            treeNode3.Text = "Archive #2 - <Date\\Time>";
            treeNode4.Name = "Node0";
            treeNode4.Text = "SQL Server";
            treeNode5.Name = "Node3";
            treeNode5.Text = "Current - <Date\\Time>";
            treeNode6.Name = "Node6";
            treeNode6.Text = "Archive #1 - <Date\\Time>";
            treeNode7.Name = "Node7";
            treeNode7.Text = "Archive #2 - <Date\\Time>";
            treeNode8.Name = "Node1";
            treeNode8.Text = "SQL Server Agent";
            this.availableLogsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode8});
            this.availableLogsTreeView.Size = new System.Drawing.Size(201, 408);
            this.availableLogsTreeView.TabIndex = 0;
            this.availableLogsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.availableLogsTreeView_AfterCheck);
            this.availableLogsTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.availableLogsTreeView_BeforeSelect);
            this.availableLogsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.availableLogsTreeView_MouseDown);
            // 
            // availableLogsHeaderStrip
            // 
            this.availableLogsHeaderStrip.AutoSize = false;
            this.availableLogsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.availableLogsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.availableLogsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.availableLogsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.availableLogsHeaderStrip.HotTrackEnabled = false;
            this.availableLogsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideAvailableLogsPanel,
            this.availableLogsHeaderLabel});
            this.availableLogsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.availableLogsHeaderStrip.Name = "availableLogsHeaderStrip";
            this.availableLogsHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 1, 1, 0);
            this.availableLogsHeaderStrip.Size = new System.Drawing.Size(202, 25);
            this.availableLogsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.availableLogsHeaderStrip.TabIndex = 16;
            // 
            // hideAvailableLogsPanel
            // 
            this.hideAvailableLogsPanel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideAvailableLogsPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideAvailableLogsPanel.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.hideAvailableLogsPanel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.hideAvailableLogsPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideAvailableLogsPanel.Name = "hideAvailableLogsPanel";
            this.hideAvailableLogsPanel.Size = new System.Drawing.Size(23, 21);
            this.hideAvailableLogsPanel.ToolTipText = "Close";
            this.hideAvailableLogsPanel.Click += new System.EventHandler(this.hideAvailableLogsPanel_Click);
            // 
            // availableLogsHeaderLabel
            // 
            this.availableLogsHeaderLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.availableLogsHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.availableLogsHeaderLabel.Name = "availableLogsHeaderLabel";
            this.availableLogsHeaderLabel.Size = new System.Drawing.Size(33, 21);
            this.availableLogsHeaderLabel.Text = "Logs";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer2.Panel1.Controls.Add(this.logGrid);
            this.splitContainer2.Panel1.Controls.Add(this.logProgressControl);
            this.splitContainer2.Panel1.Controls.Add(this.logGridStatusLabel);
            this.splitContainer2.Panel1.Padding = new System.Windows.Forms.Padding(1, 1, 0, 1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.AutoScrollMinSize = new System.Drawing.Size(390, 120);
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer2.Panel2.Controls.Add(this.detailsContainerPanel);
            this.splitContainer2.Panel2.Controls.Add(this.detailsHeaderStrip);
            this.splitContainer2.Size = new System.Drawing.Size(394, 433);
            this.splitContainer2.SplitterDistance = 250;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer2_MouseDown);
            this.splitContainer2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer2_MouseUp);
            // 
            // logGrid
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.logGrid.DisplayLayout.Appearance = appearance1;
            this.logGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn1.Header.Appearance = appearance2;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 24;
            ultraGridColumn2.Format = "G";
            ultraGridColumn2.Header.Caption = "Date";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 141;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 70;
            ultraGridColumn4.Header.Caption = "Msg #";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn4.Width = 40;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 360;
            ultraGridColumn6.Header.Caption = "Log Type";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 113;
            ultraGridColumn7.Header.Caption = "Log Source";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 222;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.logGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.logGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.logGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.logGrid.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.logGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.logGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.logGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.logGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.logGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.logGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.logGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.logGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.logGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.logGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.logGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.logGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.logGrid.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.logGrid.DisplayLayout.Override.CellAppearance = appearance7;
            this.logGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.logGrid.DisplayLayout.Override.CellPadding = 0;
            this.logGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.logGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.logGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.logGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.logGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.logGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.logGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.logGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.logGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.logGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.logGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.logGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.logGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.logGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.logGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.logGrid.DisplayLayout.UseFixedHeaders = true;
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueList1.Appearance = appearance12;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "severityValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.Key = "logNameValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.Key = "logTypeValueList";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.logGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.logGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.logGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.logGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logGrid.Location = new System.Drawing.Point(1, 1);
            this.logGrid.Name = "logGrid";
            this.logGrid.Size = new System.Drawing.Size(393, 270);
            this.logGrid.TabIndex = 3;
            this.logGrid.Visible = false;
            this.logGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.logGrid_InitializeLayout);
            this.logGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.logGrid_AfterSelectChange);
            this.logGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.logGrid_MouseDown);
            // Scaling for high resolution screens
            int detailRowHeight = 20;
            int detailColumnWidth = 75;
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    detailRowHeight = 22;
                    detailColumnWidth = 80;
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    detailRowHeight = 25;
                    detailColumnWidth = 90;
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    detailRowHeight = 30;
                    detailColumnWidth = 100;
                }
            }
            else
            {
                detailRowHeight = 20;
                detailColumnWidth = 75;
            }
                

            // 
            // logProgressControl
            // 
            this.logProgressControl.Active = false;
            this.logProgressControl.BackColor = System.Drawing.Color.White;
            this.logProgressControl.Color = System.Drawing.Color.DarkGray;
            this.logProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logProgressControl.InnerCircleRadius = 5;
            this.logProgressControl.Location = new System.Drawing.Point(1, 1);
            this.logProgressControl.Name = "logProgressControl";
            this.logProgressControl.NumberSpoke = 12;
            this.logProgressControl.OuterCircleRadius = 11;
            this.logProgressControl.RotationSpeed = 80;
            this.logProgressControl.Size = new System.Drawing.Size(393, 270);
            this.logProgressControl.SpokeThickness = 2;
            this.logProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.logProgressControl.TabIndex = 5;
            // 
            // logGridStatusLabel
            // 
            this.logGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.logGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGridStatusLabel.Location = new System.Drawing.Point(1, 1);
            this.logGridStatusLabel.Name = "logGridStatusLabel";
            this.logGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.logGridStatusLabel.Size = new System.Drawing.Size(393, 270);
            this.logGridStatusLabel.TabIndex = 4;
            this.logGridStatusLabel.Text = "< Status Label >";
            this.logGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // detailsContainerPanel
            // 
            this.detailsContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.detailsContainerPanel.Controls.Add(this.detailsPanel);
            this.detailsContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.detailsContainerPanel.Name = "detailsContainerPanel";
            this.detailsContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.detailsContainerPanel.Size = new System.Drawing.Size(394, 138);
            this.detailsContainerPanel.TabIndex = 16;
            // 
            // detailsPanel
            // 
            this.detailsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.detailsPanel.Controls.Add(this.detailsLayoutPanel);
            this.detailsPanel.Controls.Add(this.detailsPanelLabel);
            this.detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanel.Location = new System.Drawing.Point(1, 0);
            this.detailsPanel.Name = "detailsPanel";
            this.detailsPanel.Size = new System.Drawing.Size(393, 138);
            this.detailsPanel.TabIndex = 0;
            // 
            // detailsLayoutPanel
            // 
            this.detailsLayoutPanel.ColumnCount = 3;
            this.detailsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, detailColumnWidth));
            this.detailsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.0531F));
            this.detailsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.9469F));
            this.detailsLayoutPanel.Controls.Add(this.boundMessageTextBox, 0, 5);
            this.detailsLayoutPanel.Controls.Add(this.label2, 0, 4);
            this.detailsLayoutPanel.Controls.Add(this.boundMessageNumberLabel, 1, 3);
            this.detailsLayoutPanel.Controls.Add(this.messageNumberLabel, 0, 3);
            this.detailsLayoutPanel.Controls.Add(this.dateLabel, 0, 1);
            this.detailsLayoutPanel.Controls.Add(this.severityLabel, 0, 0);
            this.detailsLayoutPanel.Controls.Add(this.boundDateLabel, 1, 1);
            this.detailsLayoutPanel.Controls.Add(this.sourceLabel, 0, 2);
            this.detailsLayoutPanel.Controls.Add(this.boundSourceLabel, 1, 2);
            this.detailsLayoutPanel.Controls.Add(this.detailsLogGroupBox, 2, 0);
            this.detailsLayoutPanel.Controls.Add(this.boundSeverityPanel, 1, 0);
            this.detailsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.detailsLayoutPanel.Name = "detailsLayoutPanel";
            this.detailsLayoutPanel.RowCount = 6;
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10+detailRowHeight));
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, detailRowHeight));
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, detailRowHeight));
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, detailRowHeight));
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, detailRowHeight));
            this.detailsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsLayoutPanel.Size = new System.Drawing.Size(393, 138);
            this.detailsLayoutPanel.TabIndex = 15;
            this.detailsLayoutPanel.Visible = false;
            // 
            // boundMessageTextBox
            // 
            this.boundMessageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundMessageTextBox.BackColor = System.Drawing.Color.White;
            this.detailsLayoutPanel.SetColumnSpan(this.boundMessageTextBox, 3);
            this.boundMessageTextBox.Location = new System.Drawing.Point(3, 100);
            this.boundMessageTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.boundMessageTextBox.MinimumSize = new System.Drawing.Size(0, 20);
            this.boundMessageTextBox.Multiline = true;
            this.boundMessageTextBox.Name = "boundMessageTextBox";
            this.boundMessageTextBox.ReadOnly = true;
            this.boundMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.boundMessageTextBox.Size = new System.Drawing.Size(387, 35);
            this.boundMessageTextBox.TabIndex = 26;
            this.boundMessageTextBox.Text = "< Message >";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.detailsLayoutPanel.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(3, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(190, 20);
            this.label2.TabIndex = 25;
            this.label2.Text = "Message:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundMessageNumberLabel
            // 
            this.boundMessageNumberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundMessageNumberLabel.AutoEllipsis = true;
            this.boundMessageNumberLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundMessageNumberLabel.Location = new System.Drawing.Point(78, 60);
            this.boundMessageNumberLabel.Name = "boundMessageNumberLabel";
            this.boundMessageNumberLabel.Size = new System.Drawing.Size(115, 20);
            this.boundMessageNumberLabel.TabIndex = 24;
            this.boundMessageNumberLabel.Text = "< Number >";
            this.boundMessageNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // messageNumberLabel
            // 
            this.messageNumberLabel.AutoSize = true;
            this.messageNumberLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageNumberLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.messageNumberLabel.Location = new System.Drawing.Point(3, 60);
            this.messageNumberLabel.Name = "messageNumberLabel";
            this.messageNumberLabel.Size = new System.Drawing.Size(69, 20);
            this.messageNumberLabel.TabIndex = 23;
            this.messageNumberLabel.Text = "Msg #:";
            this.messageNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dateLabel.Location = new System.Drawing.Point(3, 20);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(69, 20);
            this.dateLabel.TabIndex = 6;
            this.dateLabel.Text = "Date:";
            this.dateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // severityLabel
            // 
            this.severityLabel.AutoSize = true;
            this.severityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.severityLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.severityLabel.Location = new System.Drawing.Point(3, 0);
            this.severityLabel.Name = "severityLabel";
            this.severityLabel.Size = new System.Drawing.Size(69, 20);
            this.severityLabel.TabIndex = 7;
            this.severityLabel.Text = "Severity:";
            this.severityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundDateLabel
            // 
            this.boundDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundDateLabel.AutoEllipsis = true;
            this.boundDateLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundDateLabel.Location = new System.Drawing.Point(78, 20);
            this.boundDateLabel.Name = "boundDateLabel";
            this.boundDateLabel.Size = new System.Drawing.Size(115, 20);
            this.boundDateLabel.TabIndex = 19;
            this.boundDateLabel.Text = "< Date >";
            this.boundDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sourceLabel
            // 
            this.sourceLabel.AutoSize = true;
            this.sourceLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sourceLabel.Location = new System.Drawing.Point(3, 40);
            this.sourceLabel.Name = "sourceLabel";
            this.sourceLabel.Size = new System.Drawing.Size(69, 20);
            this.sourceLabel.TabIndex = 20;
            this.sourceLabel.Text = "Source:";
            this.sourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundSourceLabel
            // 
            this.boundSourceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundSourceLabel.AutoEllipsis = true;
            this.boundSourceLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundSourceLabel.Location = new System.Drawing.Point(78, 40);
            this.boundSourceLabel.Name = "boundSourceLabel";
            this.boundSourceLabel.Size = new System.Drawing.Size(115, 20);
            this.boundSourceLabel.TabIndex = 21;
            this.boundSourceLabel.Text = "< Source >";
            this.boundSourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // detailsLogGroupBox
            // 
            this.detailsLogGroupBox.Controls.Add(this.boundLogFileLabel);
            this.detailsLogGroupBox.Controls.Add(this.boundLogNameLabel);
            this.detailsLogGroupBox.Controls.Add(this.boundLogTypeLabel);
            this.detailsLogGroupBox.Controls.Add(this.logFileLabel);
            this.detailsLogGroupBox.Controls.Add(this.logNameLabel);
            this.detailsLogGroupBox.Controls.Add(this.logTypeLabel);
            this.detailsLogGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsLogGroupBox.Location = new System.Drawing.Point(199, 3);
            this.detailsLogGroupBox.Name = "detailsLogGroupBox";
            this.detailsLayoutPanel.SetRowSpan(this.detailsLogGroupBox, 5);
            this.detailsLogGroupBox.Size = new System.Drawing.Size(191, 94);
            this.detailsLogGroupBox.TabIndex = 22;
            this.detailsLogGroupBox.TabStop = false;
            this.detailsLogGroupBox.Text = "Log Information"; 
            // 
            // boundLogFileLabel
            // 
            this.boundLogFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundLogFileLabel.AutoEllipsis = true;
            this.boundLogFileLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundLogFileLabel.Location = new System.Drawing.Point(50, 61);
            this.boundLogFileLabel.Name = "boundLogFileLabel";
            this.boundLogFileLabel.Size = new System.Drawing.Size(127, 16);
            this.boundLogFileLabel.TabIndex = 22;
            this.boundLogFileLabel.Text = "< File >";
            this.boundLogFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundLogNameLabel
            // 
            this.boundLogNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundLogNameLabel.AutoEllipsis = true;
            this.boundLogNameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundLogNameLabel.Location = new System.Drawing.Point(50, 41);
            this.boundLogNameLabel.Name = "boundLogNameLabel";
            this.boundLogNameLabel.Size = new System.Drawing.Size(127, 16);
            this.boundLogNameLabel.TabIndex = 21;
            this.boundLogNameLabel.Text = "< Log >";
            this.boundLogNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundLogTypeLabel
            // 
            this.boundLogTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boundLogTypeLabel.AutoEllipsis = true;
            this.boundLogTypeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.boundLogTypeLabel.Location = new System.Drawing.Point(46, 21);
            this.boundLogTypeLabel.Name = "boundLogTypeLabel";
            this.boundLogTypeLabel.Size = new System.Drawing.Size(127, 16);
            this.boundLogTypeLabel.TabIndex = 20;
            this.boundLogTypeLabel.Text = "< Type >";
            this.boundLogTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logFileLabel
            // 
            this.logFileLabel.AutoSize = true;
            this.logFileLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logFileLabel.Location = new System.Drawing.Point(6, 21 + (2 * detailRowHeight));
            this.logFileLabel.Name = "logFileLabel";
            this.logFileLabel.Size = new System.Drawing.Size(26, 13);
            this.logFileLabel.TabIndex = 9;
            this.logFileLabel.Text = "File:";
            this.logFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logNameLabel
            // 
            this.logNameLabel.AutoSize = true;
            this.logNameLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logNameLabel.Location = new System.Drawing.Point(6, 21+detailRowHeight);
            this.logNameLabel.Name = "logNameLabel";
            this.logNameLabel.Size = new System.Drawing.Size(28, 13);
            this.logNameLabel.TabIndex = 8;
            this.logNameLabel.Text = "Log:";
            this.logNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // logTypeLabel
            // 
            this.logTypeLabel.AutoSize = true;
            this.logTypeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.logTypeLabel.Location = new System.Drawing.Point(6, 21);
            this.logTypeLabel.Name = "logTypeLabel";
            this.logTypeLabel.Size = new System.Drawing.Size(34, 13);
            this.logTypeLabel.TabIndex = 7;
            this.logTypeLabel.Text = "Type:";
            this.logTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundSeverityPanel
            // 
            this.boundSeverityPanel.Controls.Add(this.boundSeverityLabel);
            this.boundSeverityPanel.Controls.Add(this.boundSeverityPictureBox);
            this.boundSeverityPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boundSeverityPanel.Location = new System.Drawing.Point(78, 3);
            this.boundSeverityPanel.Name = "boundSeverityPanel";
            this.boundSeverityPanel.Size = new System.Drawing.Size(115, 14);
            this.boundSeverityPanel.TabIndex = 27;
            this.boundSeverityPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            // 
            // boundSeverityLabel
            // 
            this.boundSeverityLabel.AutoEllipsis = true;
            this.boundSeverityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boundSeverityLabel.Location = new System.Drawing.Point(27, 0);
            this.boundSeverityLabel.Name = "boundSeverityLabel";
            this.boundSeverityLabel.Size = new System.Drawing.Size(88, 14);
            this.boundSeverityLabel.TabIndex = 1;
            this.boundSeverityLabel.Text = "<Severity>";
            this.boundSeverityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // boundSeverityPictureBox
            // 
            this.boundSeverityPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.boundSeverityPictureBox.Location = new System.Drawing.Point(0, 0);
            this.boundSeverityPictureBox.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.boundSeverityPictureBox.Name = "boundSeverityPictureBox";
            this.boundSeverityPictureBox.Size = new System.Drawing.Size(27, 14);
            this.boundSeverityPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.boundSeverityPictureBox.TabIndex = 0;
            this.boundSeverityPictureBox.TabStop = false;
            // 
            // detailsPanelLabel
            // 
            this.detailsPanelLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsPanelLabel.Location = new System.Drawing.Point(0, 0);
            this.detailsPanelLabel.Name = "detailsPanelLabel";
            this.detailsPanelLabel.Size = new System.Drawing.Size(393, 138);
            this.detailsPanelLabel.TabIndex = 14;
            this.detailsPanelLabel.Text = "Click on a Log Item to view Log Details";
            this.detailsPanelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // detailsHeaderStrip
            // 
            this.detailsHeaderStrip.AutoSize = false;
            this.detailsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.detailsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.detailsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.detailsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.detailsHeaderStrip.HotTrackEnabled = false;
            this.detailsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideDetailsPanelButton,
            this.detailsHeaderStripLabel});
            this.detailsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.detailsHeaderStrip.Name = "detailsHeaderStrip";
            this.detailsHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 1, 1, 0);
            this.detailsHeaderStrip.Size = new System.Drawing.Size(394, 19);
            this.detailsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.detailsHeaderStrip.TabIndex = 15;
            // 
            // hideDetailsPanelButton
            // 
            this.hideDetailsPanelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideDetailsPanelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideDetailsPanelButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.hideDetailsPanelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.hideDetailsPanelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideDetailsPanelButton.Name = "hideDetailsPanelButton";
            this.hideDetailsPanelButton.Size = new System.Drawing.Size(23, 15);
            this.hideDetailsPanelButton.ToolTipText = "Close";
            this.hideDetailsPanelButton.Click += new System.EventHandler(this.hideDetailsPanelButton_Click);
            // 
            // detailsHeaderStripLabel
            // 
            this.detailsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.detailsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.detailsHeaderStripLabel.Name = "detailsHeaderStripLabel";
            this.detailsHeaderStripLabel.Size = new System.Drawing.Size(46, 15);
            this.detailsHeaderStripLabel.Text = "Details";
            // 
            // lblLogLimitExceeded
            // 
            this.lblLogLimitExceeded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogLimitExceeded.Location = new System.Drawing.Point(0, 0);
            this.lblLogLimitExceeded.Name = "lblLogLimitExceeded";
            this.lblLogLimitExceeded.Size = new System.Drawing.Size(394, 433);
            this.lblLogLimitExceeded.TabIndex = 31;
            this.lblLogLimitExceeded.TabStop = true;
            this.lblLogLimitExceeded.Text = "Viewing the current logs is not recommended because they have exceeded the config" +
    "ured maximum values. It is recommended that you cycle your log.";
            this.lblLogLimitExceeded.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLogLimitExceeded.Visible = false;
            this.lblLogLimitExceeded.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLogLimitExceeded_LinkClicked);
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
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            popupMenuTool2.SharedPropsInternal.Caption = "treeContextMenu";
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool11});
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool12.SharedPropsInternal.Caption = "Refresh Log File List";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool13.SharedPropsInternal.Caption = "Print";
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool14.SharedPropsInternal.Caption = "Export to Excel";
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool22,
            buttonTool15,
            buttonTool16,
            buttonTool17,
            buttonTool18});
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            buttonTool19.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool20.SharedPropsInternal.Caption = "Expand All Groups";
            buttonTool21.SharedPropsInternal.Caption = "Configure Alerts...";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            popupMenuTool2,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            popupMenuTool3,
            stateButtonTool2,
            buttonTool19,
            buttonTool20,
            buttonTool21});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // LogsView_Fill_Panel
            // 
            this.LogsView_Fill_Panel.Controls.Add(this.splitContainer1);
            this.LogsView_Fill_Panel.Controls.Add(this.infoPanel);
            this.LogsView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.LogsView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogsView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.LogsView_Fill_Panel.Name = "LogsView_Fill_Panel";
            this.LogsView_Fill_Panel.Size = new System.Drawing.Size(600, 459);
            this.LogsView_Fill_Panel.TabIndex = 17;
            // 
            // infoPanel
            // 
            this.infoPanel.Controls.Add(this.pictureBox1);
            this.infoPanel.Controls.Add(this.informationLabel);
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoPanel.Location = new System.Drawing.Point(0, 0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(600, 26);
            this.infoPanel.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
            this.pictureBox1.Location = new System.Drawing.Point(6, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // informationLabel
            // 
            this.informationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.informationLabel.ForeColor = System.Drawing.Color.Black;
            this.informationLabel.Location = new System.Drawing.Point(3, 3);
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.Size = new System.Drawing.Size(594, 20);
            this.informationLabel.TabIndex = 0;
            this.informationLabel.Text = "       Log event times shown in this view are local to the monitored SQL Server i" +
    "nstance.";
            this.informationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.informationLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.informationLabel_MouseDown);
            this.informationLabel.MouseEnter += new System.EventHandler(this.informationLabel_MouseEnter);
            this.informationLabel.MouseLeave += new System.EventHandler(this.informationLabel_MouseLeave);
            this.informationLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.informationLabel_MouseUp);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xls";
            this.saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            this.saveFileDialog.Title = "Save as Excel Spreadsheet";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Server Logs";
            this.ultraGridPrintDocument.FitWidthToPages = 1;
            this.ultraGridPrintDocument.Grid = this.logGrid;
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
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(600, 459);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 30;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
    "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // LogsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.LogsView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "LogsView";
            this.Size = new System.Drawing.Size(600, 459);
            this.Load += new System.EventHandler(this.LogsView_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.availableLogsContainerPanel.ResumeLayout(false);
            this.availableLogsHeaderStrip.ResumeLayout(false);
            this.availableLogsHeaderStrip.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logGrid)).EndInit();
            this.detailsContainerPanel.ResumeLayout(false);
            this.detailsPanel.ResumeLayout(false);
            this.detailsLayoutPanel.ResumeLayout(false);
            this.detailsLayoutPanel.PerformLayout();
            this.detailsLogGroupBox.ResumeLayout(false);
            this.detailsLogGroupBox.PerformLayout();
            this.boundSeverityPanel.ResumeLayout(false);
            this.boundSeverityPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boundSeverityPictureBox)).EndInit();
            this.detailsHeaderStrip.ResumeLayout(false);
            this.detailsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.LogsView_Fill_Panel.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView availableLogsTreeView;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip detailsHeaderStrip;
        private System.Windows.Forms.ToolStripButton hideDetailsPanelButton;
        private System.Windows.Forms.ToolStripLabel detailsHeaderStripLabel;
        private Infragistics.Win.UltraWinGrid.UltraGrid logGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  detailsContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip availableLogsHeaderStrip;
        private System.Windows.Forms.ToolStripButton hideAvailableLogsPanel;
        private System.Windows.Forms.ToolStripLabel availableLogsHeaderLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  detailsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  availableLogsContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  LogsView_Fill_Panel;
        private System.Windows.Forms.Panel  infoPanel;
        private System.Windows.Forms.Label informationLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logGridStatusLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel detailsPanelLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel detailsLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dateLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel severityLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundDateLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel sourceLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundSourceLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox detailsLogGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundMessageNumberLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel messageNumberLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox boundMessageTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logTypeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundLogTypeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logFileLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundLogFileLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundLogNameLabel;
        private MRG.Controls.UI.LoadingCircle logProgressControl;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  boundSeverityPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel boundSeverityLabel;
        private System.Windows.Forms.PictureBox boundSeverityPictureBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel lblLogLimitExceeded;
    }
}
