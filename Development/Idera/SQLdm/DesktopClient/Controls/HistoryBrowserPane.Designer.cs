using Idera.SQLdm.DesktopClient.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class HistoryBrowserPane
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
                if (recentlyViewedTreeToolTip      != null) recentlyViewedTreeToolTip.Dispose();
                if (historicalSnapshotsTreeToolTip != null) historicalSnapshotsTreeToolTip.Dispose();

                if(components != null)
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
            appearance1 = new Infragistics.Win.Appearance();
            appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryBrowserPane));
            Infragistics.Win.UltraWinTree.Override _override2 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("refreshButton");
            appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("historicalSnapshotsContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("refreshButton");
            this.calendarPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.filterOptionsLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.calendar = new Infragistics.Win.UltraWinSchedule.UltraMonthViewMulti();
            this.ultraCalendarInfo = new Infragistics.Win.UltraWinSchedule.UltraCalendarInfo(this.components);
            this.ultraCalendarLook = new Infragistics.Win.UltraWinSchedule.UltraCalendarLook(this.components);
            this.treeImages = new System.Windows.Forms.ImageList(this.components);
            this.HistoryBrowserPane_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.containerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.historicalSnapshotsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.gradientPanel1 = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.historicalSnapshotsProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.historicalSnapshotsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.historicalSnapshotsTree = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTree();
            this.historicalSnapshotsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip(true);
            this.historicalSnapshotsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.refreshHistoricalSnapshotsButton = new System.Windows.Forms.ToolStripButton();
            this.recentlyViewedPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.recentlyViewedTreeContainerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.recentlyViewedTree = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTree();
            this.recentlyViewedHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip(true);
            this.recentlyViewedHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toggleRecentlyViewedButton = new System.Windows.Forms.ToolStripButton();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.autoRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.calendarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calendar)).BeginInit();
            this.HistoryBrowserPane_Fill_Panel.SuspendLayout();
            this.containerPanel.SuspendLayout();
            this.historicalSnapshotsPanel.SuspendLayout();
            this.gradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historicalSnapshotsTree)).BeginInit();
            this.historicalSnapshotsHeaderStrip.SuspendLayout();
            this.recentlyViewedPanel.SuspendLayout();
            this.recentlyViewedTreeContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentlyViewedTree)).BeginInit();
            this.recentlyViewedHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // calendarPanel
            // 
            this.calendarPanel.Controls.Add(this.filterOptionsLinkLabel);
            this.calendarPanel.Controls.Add(this.calendar);
            this.calendarPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.calendarPanel.Location = new System.Drawing.Point(0, 0);
            this.calendarPanel.Name = "calendarPanel";
            this.calendarPanel.Size = new System.Drawing.Size(264, 175);
            this.calendarPanel.TabIndex = 3;
            // 
            // filterOptionsLinkLabel
            // 
            this.filterOptionsLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.filterOptionsLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.filterOptionsLinkLabel.Name = "filterOptionsLinkLabel";
            this.filterOptionsLinkLabel.Dock = DockStyle.Bottom;        // [SQLDM-27472] (Anshul) - History Range control_DC: 'Select History Range' link is not displayed properly
            this.filterOptionsLinkLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.filterOptionsLinkLabel.Size = new System.Drawing.Size(264, 13);
            this.filterOptionsLinkLabel.TabIndex = 3;
            this.filterOptionsLinkLabel.TabStop = true;
            this.filterOptionsLinkLabel.Text = "Select History Range";
            this.filterOptionsLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.filterOptionsLinkLabel_LinkClicked);
            // 
            // calendar
            // 
            this.calendar.AllowMonthSelection = false;
            this.calendar.AllowWeekSelection = false;
            appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.calendar.Appearance = appearance1;
            this.calendar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.calendar.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.calendar.CalendarInfo = this.ultraCalendarInfo;
            this.calendar.CalendarLook = this.ultraCalendarLook;
            this.calendar.Dock = System.Windows.Forms.DockStyle.Top;
            this.calendar.Location = new System.Drawing.Point(0, 0);
            this.calendar.Name = "calendar";
            this.calendar.ResizeMode = Infragistics.Win.UltraWinSchedule.ResizeMode.BaseOnControlSize;
            this.calendar.Size = new System.Drawing.Size(264, 158);
            this.calendar.TabIndex = 0;
            this.calendar.UseAppStyling = false;
            // 
            // ultraCalendarInfo
            // 
            this.ultraCalendarInfo.DataBindingsForAppointments.BindingContextControl = this;
            this.ultraCalendarInfo.DataBindingsForOwners.BindingContextControl = this;
            this.ultraCalendarInfo.SelectTypeDay = Infragistics.Win.UltraWinSchedule.SelectType.Single;
            this.ultraCalendarInfo.AfterSelectedDateRangeChange += new System.EventHandler(this.ultraCalendarInfo_AfterSelectedDateRangeChange);
            // 
            // ultraCalendarLook
            // 
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.ultraCalendarLook.MonthHeaderAppearance = appearance2;
            this.ultraCalendarLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Office2003;
            // 
            // treeImages
            // 
            this.treeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.treeImages.ImageSize = new System.Drawing.Size(16, 16);
            this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // HistoryBrowserPane_Fill_Panel
            // 
            this.HistoryBrowserPane_Fill_Panel.Controls.Add(this.containerPanel);
            this.HistoryBrowserPane_Fill_Panel.Controls.Add(this.calendarPanel);
            this.HistoryBrowserPane_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.HistoryBrowserPane_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HistoryBrowserPane_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.HistoryBrowserPane_Fill_Panel.Name = "HistoryBrowserPane_Fill_Panel";
            this.HistoryBrowserPane_Fill_Panel.Size = new System.Drawing.Size(264, 575);
            this.HistoryBrowserPane_Fill_Panel.TabIndex = 0;
            // 
            // containerPanel
            // 
            this.containerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerPanel.BackColor2 = System.Drawing.Color.White;
            this.containerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.containerPanel.Controls.Add(this.historicalSnapshotsPanel);
            this.containerPanel.Controls.Add(this.recentlyViewedPanel);
            this.containerPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.containerPanel.Location = new System.Drawing.Point(3, 177);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.containerPanel.ShowBorder = false;
            this.containerPanel.Size = new System.Drawing.Size(258, 395);
            this.containerPanel.TabIndex = 6;
            // 
            // historicalSnapshotsPanel
            // 
            this.historicalSnapshotsPanel.Controls.Add(this.gradientPanel1);
            this.historicalSnapshotsPanel.Controls.Add(this.historicalSnapshotsHeaderStrip);
            this.historicalSnapshotsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotsPanel.Location = new System.Drawing.Point(1, 127);
            this.historicalSnapshotsPanel.Name = "historicalSnapshotsPanel";
            this.historicalSnapshotsPanel.Size = new System.Drawing.Size(256, 267);
            this.historicalSnapshotsPanel.TabIndex = 5;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.BackColor = System.Drawing.Color.White;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.White;
            this.gradientPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.gradientPanel1.Controls.Add(this.historicalSnapshotsProgressControl);
            this.gradientPanel1.Controls.Add(this.historicalSnapshotsStatusLabel);
            this.gradientPanel1.Controls.Add(this.historicalSnapshotsTree);
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanel1.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.gradientPanel1.Location = new System.Drawing.Point(0, 19);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 1);
            this.gradientPanel1.Size = new System.Drawing.Size(256, 248);
            this.gradientPanel1.TabIndex = 4;
            // 
            // historicalSnapshotsProgressControl
            // 
            this.historicalSnapshotsProgressControl.Active = false;
            this.historicalSnapshotsProgressControl.BackColor = System.Drawing.Color.White;
            this.historicalSnapshotsProgressControl.Color = System.Drawing.Color.Silver;
            this.historicalSnapshotsProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotsProgressControl.InnerCircleRadius = 8;
            this.historicalSnapshotsProgressControl.Location = new System.Drawing.Point(2, 0);
            this.historicalSnapshotsProgressControl.Name = "historicalSnapshotsProgressControl";
            this.historicalSnapshotsProgressControl.NumberSpoke = 24;
            this.historicalSnapshotsProgressControl.OuterCircleRadius = 9;
            this.historicalSnapshotsProgressControl.RotationSpeed = 20;
            this.historicalSnapshotsProgressControl.Size = new System.Drawing.Size(253, 247);
            this.historicalSnapshotsProgressControl.SpokeThickness = 4;
            this.historicalSnapshotsProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this.historicalSnapshotsProgressControl.TabIndex = 2;
            this.historicalSnapshotsProgressControl.Visible = false;
            // 
            // historicalSnapshotsStatusLabel
            // 
            this.historicalSnapshotsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotsStatusLabel.Location = new System.Drawing.Point(2, 0);
            this.historicalSnapshotsStatusLabel.Name = "historicalSnapshotsStatusLabel";
            this.historicalSnapshotsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.historicalSnapshotsStatusLabel.Size = new System.Drawing.Size(253, 247);
            this.historicalSnapshotsStatusLabel.TabIndex = 1;
            this.historicalSnapshotsStatusLabel.Text = "< Status Label >";
            this.historicalSnapshotsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.historicalSnapshotsStatusLabel.Visible = false;
            // 
            // historicalSnapshotsTree
            // 
            this.historicalSnapshotsTree.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.toolbarsManager.SetContextMenuUltra(this.historicalSnapshotsTree, "historicalSnapshotsContextMenu");
            this.historicalSnapshotsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotsTree.FullRowSelect = true;
            this.historicalSnapshotsTree.HideSelection = false;
            this.historicalSnapshotsTree.ImageList = this.treeImages;
            this.historicalSnapshotsTree.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.historicalSnapshotsTree.Location = new System.Drawing.Point(2, 0);
            this.historicalSnapshotsTree.Name = "historicalSnapshotsTree";
            this.historicalSnapshotsTree.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.historicalSnapshotsTree.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
            _override1.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(233)))), ((int)(((byte)(237)))));
            appearance3.Cursor = System.Windows.Forms.Cursors.Hand;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(94)))), ((int)(((byte)(168)))));
            _override1.HotTrackingNodeAppearance = appearance3;
            _override1.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.Never;
            this.historicalSnapshotsTree.Override = _override1;
            this.historicalSnapshotsTree.ShowLines = false;
            this.historicalSnapshotsTree.ShowRootLines = false;
            this.historicalSnapshotsTree.Size = new System.Drawing.Size(253, 247);
            this.historicalSnapshotsTree.TabIndex = 0;
            this.historicalSnapshotsTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.historicalSnapshotsTree_AfterSelect);
            this.historicalSnapshotsTree.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.historicalSnapshotsTree_MouseEnterElement);
            this.historicalSnapshotsTree.MouseLeaveElement += new Infragistics.Win.UIElementEventHandler(this.historicalSnapshotsTree_MouseLeaveElement);
            // 
            // historicalSnapshotsHeaderStrip
            // 
            this.historicalSnapshotsHeaderStrip.AutoSize = false;
            this.historicalSnapshotsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.historicalSnapshotsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.historicalSnapshotsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.historicalSnapshotsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.historicalSnapshotsHeaderStrip.HotTrackEnabled = false;
            this.historicalSnapshotsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historicalSnapshotsHeaderStripLabel,
            this.refreshHistoricalSnapshotsButton});
            this.historicalSnapshotsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotsHeaderStrip.Name = "historicalSnapshotsHeaderStrip";
            this.historicalSnapshotsHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.historicalSnapshotsHeaderStrip.Size = new System.Drawing.Size(256, 19);
            this.historicalSnapshotsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.SmallSingle;
            this.historicalSnapshotsHeaderStrip.TabIndex = 3;
            // 
            // historicalSnapshotsHeaderStripLabel
            // 
            this.historicalSnapshotsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.historicalSnapshotsHeaderStripLabel.Name = "historicalSnapshotsHeaderStripLabel";
            this.historicalSnapshotsHeaderStripLabel.Size = new System.Drawing.Size(122, 16);
            this.historicalSnapshotsHeaderStripLabel.Text = "Historical Snapshots";
            // 
            // refreshHistoricalSnapshotsButton
            // 
            this.refreshHistoricalSnapshotsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.refreshHistoricalSnapshotsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshHistoricalSnapshotsButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Refresh; //Babita Manral
            this.refreshHistoricalSnapshotsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.refreshHistoricalSnapshotsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshHistoricalSnapshotsButton.Margin = new System.Windows.Forms.Padding(0, 1, 1, 1);
            this.refreshHistoricalSnapshotsButton.Name = "refreshHistoricalSnapshotsButton";
            this.refreshHistoricalSnapshotsButton.Size = new System.Drawing.Size(23, 17);
            this.refreshHistoricalSnapshotsButton.Text = "Refresh";
            this.refreshHistoricalSnapshotsButton.ToolTipText = "Refresh";
            this.refreshHistoricalSnapshotsButton.Click += new System.EventHandler(this.refreshHistoricalSnapshotsButton_Click);
            // 
            // recentlyViewedPanel
            // 
            this.recentlyViewedPanel.Controls.Add(this.recentlyViewedTreeContainerPanel);
            this.recentlyViewedPanel.Controls.Add(this.recentlyViewedHeaderStrip);
            this.recentlyViewedPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentlyViewedPanel.Location = new System.Drawing.Point(1, 1);
            this.recentlyViewedPanel.Name = "recentlyViewedPanel";
            this.recentlyViewedPanel.Size = new System.Drawing.Size(256, 126);
            this.recentlyViewedPanel.TabIndex = 4;
            // 
            // recentlyViewedTreeContainerPanel
            // 
            this.recentlyViewedTreeContainerPanel.BackColor = System.Drawing.Color.White;
            this.recentlyViewedTreeContainerPanel.BackColor2 = System.Drawing.Color.White;
            this.recentlyViewedTreeContainerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.recentlyViewedTreeContainerPanel.Controls.Add(this.recentlyViewedTree);
            this.recentlyViewedTreeContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentlyViewedTreeContainerPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.recentlyViewedTreeContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.recentlyViewedTreeContainerPanel.Name = "recentlyViewedTreeContainerPanel";
            this.recentlyViewedTreeContainerPanel.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.recentlyViewedTreeContainerPanel.Size = new System.Drawing.Size(256, 107);
            this.recentlyViewedTreeContainerPanel.TabIndex = 5;
            // 
            // recentlyViewedTree
            // 
            this.recentlyViewedTree.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.recentlyViewedTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentlyViewedTree.FullRowSelect = true;
            this.recentlyViewedTree.HideSelection = false;
            this.recentlyViewedTree.ImageList = this.treeImages;
            this.recentlyViewedTree.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.recentlyViewedTree.Location = new System.Drawing.Point(2, 0);
            this.recentlyViewedTree.Name = "recentlyViewedTree";
            this.recentlyViewedTree.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.recentlyViewedTree.NodeConnectorStyle = Infragistics.Win.UltraWinTree.NodeConnectorStyle.None;
            _override2.HotTracking = Infragistics.Win.DefaultableBoolean.True;
            appearance4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(233)))), ((int)(((byte)(237)))));
            appearance4.Cursor = System.Windows.Forms.Cursors.Hand;
            appearance4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(94)))), ((int)(((byte)(168)))));
            _override2.HotTrackingNodeAppearance = appearance4;
            _override2.ShowExpansionIndicator = Infragistics.Win.UltraWinTree.ShowExpansionIndicator.Never;
            this.recentlyViewedTree.Override = _override2;
            this.recentlyViewedTree.ShowLines = false;
            this.recentlyViewedTree.ShowRootLines = false;
            this.recentlyViewedTree.Size = new System.Drawing.Size(253, 107);
            this.recentlyViewedTree.TabIndex = 0;
            this.recentlyViewedTree.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.recentlyViewedTree_MouseEnterElement);
            this.recentlyViewedTree.MouseLeaveElement += new Infragistics.Win.UIElementEventHandler(this.recentlyViewedTree_MouseLeaveElement);
            this.recentlyViewedTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.recentlyViewedTree_MouseClick);
            // 
            // recentlyViewedHeaderStrip
            // 
            this.recentlyViewedHeaderStrip.AutoSize = false;
            this.recentlyViewedHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.recentlyViewedHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.recentlyViewedHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.recentlyViewedHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.recentlyViewedHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recentlyViewedHeaderStripLabel,
            this.toggleRecentlyViewedButton});
            this.recentlyViewedHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.recentlyViewedHeaderStrip.Name = "recentlyViewedHeaderStrip";
            this.recentlyViewedHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.recentlyViewedHeaderStrip.Size = new System.Drawing.Size(256, 19);
            this.recentlyViewedHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.SmallSingle;
            this.recentlyViewedHeaderStrip.TabIndex = 1;
            this.recentlyViewedHeaderStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(this.recentlyViewedHeaderStrip_MouseClick);
            // 
            // recentlyViewedHeaderStripLabel
            // 
            this.recentlyViewedHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recentlyViewedHeaderStripLabel.Name = "recentlyViewedHeaderStripLabel";
            this.recentlyViewedHeaderStripLabel.Size = new System.Drawing.Size(100, 16);
            this.recentlyViewedHeaderStripLabel.Text = "Recently Viewed";
            // 
            // toggleRecentlyViewedButton
            // 
            this.toggleRecentlyViewedButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toggleRecentlyViewedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleRecentlyViewedButton.Enabled = false;
            this.toggleRecentlyViewedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.HeaderStripSmallCollapse; //Babita Manral
            this.toggleRecentlyViewedButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toggleRecentlyViewedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleRecentlyViewedButton.Name = "toggleRecentlyViewedButton";
            this.toggleRecentlyViewedButton.Size = new System.Drawing.Size(23, 16);
            // 
            // toolTipManager
            // 
            this.toolTipManager.AutoPopDelay = 0;
            this.toolTipManager.ContainingControl = this;
            this.toolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh; //Babita Manral
            buttonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool1.SharedPropsInternal.Caption = "Refresh";
            popupMenuTool1.SharedPropsInternal.Caption = "historicalSnapshotsContextMenu";
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2});
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            popupMenuTool1});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // autoRefreshTimer
            // 
            this.autoRefreshTimer.Enabled = false;
            this.autoRefreshTimer.Interval = 60000;
            this.autoRefreshTimer.Tick += new System.EventHandler(this.autoRefreshTimer_Tick);
            // 
            // HistoryBrowserPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.HistoryBrowserPane_Fill_Panel);
            this.Name = "HistoryBrowserPane";
            this.Size = new System.Drawing.Size(264, 575);
            this.Load += new System.EventHandler(this.HistoryBrowserPane_Load);
            this.calendarPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendar)).EndInit();
            this.HistoryBrowserPane_Fill_Panel.ResumeLayout(false);
            this.containerPanel.ResumeLayout(false);
            this.historicalSnapshotsPanel.ResumeLayout(false);
            this.gradientPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.historicalSnapshotsTree)).EndInit();
            this.historicalSnapshotsHeaderStrip.ResumeLayout(false);
            this.historicalSnapshotsHeaderStrip.PerformLayout();
            this.recentlyViewedPanel.ResumeLayout(false);
            this.recentlyViewedTreeContainerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recentlyViewedTree)).EndInit();
            this.recentlyViewedHeaderStrip.ResumeLayout(false);
            this.recentlyViewedHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  calendarPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  recentlyViewedPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  historicalSnapshotsPanel;
        private HeaderStrip recentlyViewedHeaderStrip;
        private HeaderStrip historicalSnapshotsHeaderStrip;
        private Infragistics.Win.UltraWinSchedule.UltraMonthViewMulti calendar;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarLook ultraCalendarLook;
        private GradientPanel containerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel filterOptionsLinkLabel;
        private Infragistics.Win.UltraWinTree.UltraTree historicalSnapshotsTree;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarInfo ultraCalendarInfo;
        private System.Windows.Forms.ImageList treeImages;
        private GradientPanel gradientPanel1;
        private System.Windows.Forms.ToolStripLabel recentlyViewedHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton toggleRecentlyViewedButton;
        private System.Windows.Forms.ToolStripLabel historicalSnapshotsHeaderStripLabel;
        private GradientPanel recentlyViewedTreeContainerPanel;
        private Infragistics.Win.UltraWinTree.UltraTree recentlyViewedTree;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel historicalSnapshotsStatusLabel;
        private MRG.Controls.UI.LoadingCircle historicalSnapshotsProgressControl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  HistoryBrowserPane_Fill_Panel;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private System.Windows.Forms.ToolStripButton refreshHistoricalSnapshotsButton;
        private System.Windows.Forms.Timer autoRefreshTimer;
        Infragistics.Win.Appearance appearance1;
        Infragistics.Win.Appearance appearance2;
        Infragistics.Win.Appearance appearance3;
        Infragistics.Win.Appearance appearance15;
    }
}
