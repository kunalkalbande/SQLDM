using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    partial class ServerSummaryView4
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.linkLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.serverOverviewContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.dashboardTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.operationalStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.viewStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.viewStatusConnectingCircle = new MRG.Controls.UI.LoadingCircle();
            this.viewStatusImage = new System.Windows.Forms.PictureBox();
            this.viewStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.panelGalleryPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panelGalleryGradientPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.galleryPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.galleryTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.panelSelectorTrackBar = new Infragistics.Win.UltraWinEditors.UltraTrackBar();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.panelDesignerCloseButton = new System.Windows.Forms.ToolStripButton();
            this.panelDesignerPanelCountLabel = new System.Windows.Forms.ToolStripLabel();
            this.dashboardPanelDetails = new Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard.DashboardPanelDetails();
            this.serverOverviewContentPanel.SuspendLayout();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusPanel)).BeginInit();
            this.viewStatusPanel.Panel1.SuspendLayout();
            this.viewStatusPanel.Panel2.SuspendLayout();
            this.viewStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusImage)).BeginInit();
            this.panelGalleryPanel.SuspendLayout();
            this.panelGalleryGradientPanel.SuspendLayout();
            this.galleryPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelSelectorTrackBar)).BeginInit();
            this.headerStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoEllipsis = true;
            this.linkLabel1.DisabledLinkColor = System.Drawing.Color.Black;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Black;
            this.linkLabel1.Location = new System.Drawing.Point(45, 131);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(146, 13);
            this.linkLabel1.TabIndex = 52;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "? Informational";
            // 
            // serverOverviewContentPanel
            // 
            this.serverOverviewContentPanel.AutoScroll = true;
            this.serverOverviewContentPanel.Controls.Add(this.dashboardTableLayoutPanel);
            this.serverOverviewContentPanel.Controls.Add(this.operationalStatusPanel);
            this.serverOverviewContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverOverviewContentPanel.Location = new System.Drawing.Point(0, 0);
            this.serverOverviewContentPanel.Name = "serverOverviewContentPanel";
            this.serverOverviewContentPanel.Size = new System.Drawing.Size(1024, 768);
            this.serverOverviewContentPanel.TabIndex = 2;
            this.serverOverviewContentPanel.Visible = false;
            // 
            // dashboardTableLayoutPanel
            // 
            //this.dashboardTableLayoutPanel.CellBorderStyle = Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel.CellBorderStyle.OutsetPartial;
            this.dashboardTableLayoutPanel.ColumnCount = 2;
            this.dashboardTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardTableLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dashboardTableLayoutPanel.Location = new System.Drawing.Point(0, 26);
            this.dashboardTableLayoutPanel.Name = "dashboardTableLayoutPanel";
            this.dashboardTableLayoutPanel.RowCount = 2;
            this.dashboardTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardTableLayoutPanel.Size = new System.Drawing.Size(1024, 742);
            this.dashboardTableLayoutPanel.TabIndex = 1;
            // 
            // operationalStatusPanel
            // 
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(1024, 26);
            this.operationalStatusPanel.TabIndex = 0;
            this.operationalStatusPanel.Visible = false;
            // 
            // operationalStatusImage
            // 
            this.operationalStatusImage.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.operationalStatusImage.Location = new System.Drawing.Point(6, 3);
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
            this.operationalStatusLabel.Location = new System.Drawing.Point(3, 1);
            this.operationalStatusLabel.Name = "operationalStatusLabel";
            this.operationalStatusLabel.Size = new System.Drawing.Size(1016, 21);
            this.operationalStatusLabel.TabIndex = 2;
            this.operationalStatusLabel.Text = "       < status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.AutoEllipsis = true;
            this.historicalSnapshotStatusLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.historicalSnapshotStatusLinkLabel.LinkColor = System.Drawing.Color.Blue;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(1024, 768);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 40;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "Data for the selected historical snapshot does not exist for this view. Select an" +
                "other historical snapshot or click here to switch to real-time mode.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.Visible = false;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // viewStatusPanel
            // 
            this.viewStatusPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.viewStatusPanel.IsSplitterFixed = true;
            this.viewStatusPanel.Location = new System.Drawing.Point(384, 359);
            this.viewStatusPanel.Name = "viewStatusPanel";
            // 
            // viewStatusPanel.Panel1
            // 
            this.viewStatusPanel.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.viewStatusPanel.Panel1.Controls.Add(this.viewStatusConnectingCircle);
            this.viewStatusPanel.Panel1.Controls.Add(this.viewStatusImage);
            this.viewStatusPanel.Panel1.Padding = new System.Windows.Forms.Padding(1);
            // 
            // viewStatusPanel.Panel2
            // 
            this.viewStatusPanel.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.viewStatusPanel.Panel2.Controls.Add(this.viewStatusLabel);
            this.viewStatusPanel.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.viewStatusPanel.Size = new System.Drawing.Size(257, 50);
            this.viewStatusPanel.SplitterDistance = 54;
            this.viewStatusPanel.TabIndex = 4;
            // 
            // viewStatusConnectingCircle
            // 
            this.viewStatusConnectingCircle.Active = false;
            this.viewStatusConnectingCircle.BackColor = System.Drawing.Color.White;
            this.viewStatusConnectingCircle.Color = System.Drawing.Color.LimeGreen;
            this.viewStatusConnectingCircle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStatusConnectingCircle.InnerCircleRadius = 5;
            this.viewStatusConnectingCircle.Location = new System.Drawing.Point(1, 1);
            this.viewStatusConnectingCircle.Name = "viewStatusConnectingCircle";
            this.viewStatusConnectingCircle.NumberSpoke = 12;
            this.viewStatusConnectingCircle.OuterCircleRadius = 11;
            this.viewStatusConnectingCircle.RotationSpeed = 100;
            this.viewStatusConnectingCircle.Size = new System.Drawing.Size(52, 48);
            this.viewStatusConnectingCircle.SpokeThickness = 2;
            this.viewStatusConnectingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.viewStatusConnectingCircle.TabIndex = 0;
            // 
            // viewStatusImage
            // 
            this.viewStatusImage.BackColor = System.Drawing.Color.White;
            this.viewStatusImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStatusImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusSummaryCriticalLarge;
            this.viewStatusImage.Location = new System.Drawing.Point(1, 1);
            this.viewStatusImage.Name = "viewStatusImage";
            this.viewStatusImage.Size = new System.Drawing.Size(52, 48);
            this.viewStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.viewStatusImage.TabIndex = 1;
            this.viewStatusImage.TabStop = false;
            // 
            // viewStatusLabel
            // 
            this.viewStatusLabel.AutoEllipsis = true;
            this.viewStatusLabel.BackColor = System.Drawing.Color.White;
            this.viewStatusLabel.DisabledLinkColor = System.Drawing.Color.Black;
            this.viewStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStatusLabel.Enabled = false;
            this.viewStatusLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.viewStatusLabel.LinkColor = System.Drawing.Color.Black;
            this.viewStatusLabel.Location = new System.Drawing.Point(1, 1);
            this.viewStatusLabel.Name = "viewStatusLabel";
            this.viewStatusLabel.Padding = new System.Windows.Forms.Padding(3);
            this.viewStatusLabel.Size = new System.Drawing.Size(197, 48);
            this.viewStatusLabel.TabIndex = 24;
            this.viewStatusLabel.TabStop = true;
            this.viewStatusLabel.Text = "< status message >";
            this.viewStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.viewStatusLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewStatusLabel_LinkClicked);
            // 
            // panelGalleryPanel
            // 
            this.panelGalleryPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGalleryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGalleryPanel.Controls.Add(this.panelGalleryGradientPanel);
            this.panelGalleryPanel.Controls.Add(this.headerStrip1);
            this.panelGalleryPanel.Location = new System.Drawing.Point(675, 0);
            this.panelGalleryPanel.Name = "panelGalleryPanel";
            this.panelGalleryPanel.Size = new System.Drawing.Size(350, 768);
            this.panelGalleryPanel.TabIndex = 44;
            this.panelGalleryPanel.Visible = false;
            // 
            // panelGalleryGradientPanel
            // 
            this.panelGalleryGradientPanel.BackColor = System.Drawing.Color.Silver;
            this.panelGalleryGradientPanel.BackColor2 = System.Drawing.Color.White;
            this.panelGalleryGradientPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.panelGalleryGradientPanel.Controls.Add(this.galleryPanel);
            this.panelGalleryGradientPanel.Controls.Add(this.panelSelectorTrackBar);
            this.panelGalleryGradientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGalleryGradientPanel.Location = new System.Drawing.Point(0, 25);
            this.panelGalleryGradientPanel.Name = "panelGalleryGradientPanel";
            this.panelGalleryGradientPanel.Padding = new System.Windows.Forms.Padding(0, 5, 3, 5);
            this.panelGalleryGradientPanel.Size = new System.Drawing.Size(348, 741);
            this.panelGalleryGradientPanel.TabIndex = 1;
            this.panelGalleryGradientPanel.Resize += new System.EventHandler(this.panelGalleryGradientPanel_Resize);
            // 
            // galleryPanel
            // 
            this.galleryPanel.BackColor = System.Drawing.Color.Transparent;
            this.galleryPanel.Controls.Add(this.galleryTableLayoutPanel);
            this.galleryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.galleryPanel.Location = new System.Drawing.Point(0, 5);
            this.galleryPanel.Name = "galleryPanel";
            this.galleryPanel.Padding = new System.Windows.Forms.Padding(1, 2, 0, 2);
            this.galleryPanel.Size = new System.Drawing.Size(319, 731);
            this.galleryPanel.TabIndex = 9;
            // 
            // galleryTableLayoutPanel
            // 
            this.galleryTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.galleryTableLayoutPanel.ColumnCount = 1;
            this.galleryTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.galleryTableLayoutPanel.Location = new System.Drawing.Point(1, 4);
            this.galleryTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.galleryTableLayoutPanel.Name = "galleryTableLayoutPanel";
            this.galleryTableLayoutPanel.RowCount = 5;
            this.galleryTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.galleryTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.galleryTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.galleryTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.galleryTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.galleryTableLayoutPanel.Size = new System.Drawing.Size(318, 1163);
            this.galleryTableLayoutPanel.TabIndex = 7;
            // 
            // panelSelectorTrackBar
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.panelSelectorTrackBar.Appearance = appearance5;
            this.panelSelectorTrackBar.AutoSize = false;
            this.panelSelectorTrackBar.BackColor = System.Drawing.Color.Transparent;
            this.panelSelectorTrackBar.BackColorInternal = System.Drawing.Color.Transparent;
            appearance4.BackColor = System.Drawing.Color.Transparent;
            appearance4.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.navigate_close;
            this.panelSelectorTrackBar.ButtonSettings.DecrementButtonAppearance = appearance4;
            appearance3.BackColor = System.Drawing.Color.Transparent;
            appearance3.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.navigate_open;
            this.panelSelectorTrackBar.ButtonSettings.IncrementButtonAppearance = appearance3;
            this.panelSelectorTrackBar.ButtonSettings.ShowIncrementButtons = Infragistics.Win.DefaultableBoolean.True;
            this.panelSelectorTrackBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSelectorTrackBar.LargeChange = 100;
            this.panelSelectorTrackBar.Location = new System.Drawing.Point(319, 5);
            this.panelSelectorTrackBar.MaxValue = 0;
            this.panelSelectorTrackBar.MidpointSettings.Visible = Infragistics.Win.DefaultableBoolean.False;
            this.panelSelectorTrackBar.Name = "panelSelectorTrackBar";
            this.panelSelectorTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.panelSelectorTrackBar.Padding = new System.Drawing.Size(1, 1);
            this.panelSelectorTrackBar.Size = new System.Drawing.Size(26, 731);
            this.panelSelectorTrackBar.SmallChange = 100;
            this.panelSelectorTrackBar.TabIndex = 8;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.bullet_ball_glass_grey;
            this.panelSelectorTrackBar.ThumbAppearance = appearance1;
            this.panelSelectorTrackBar.ThumbOffset = new System.Drawing.Point(-1, 0);
            appearance2.BackColor = System.Drawing.Color.Transparent;
            appearance2.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.bullet_ball_glass_red;
            this.panelSelectorTrackBar.ThumbPressedAppearance = appearance2;
            this.panelSelectorTrackBar.TickmarkSettingsMajor.Visible = Infragistics.Win.DefaultableBoolean.False;
            this.panelSelectorTrackBar.TickmarkSettingsMinor.Visible = Infragistics.Win.DefaultableBoolean.False;
            this.panelSelectorTrackBar.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.panelSelectorTrackBar.ViewStyle = Infragistics.Win.UltraWinEditors.TrackBarViewStyle.Vista;
            this.panelSelectorTrackBar.ValueChanged += new System.EventHandler(this.panelSelectorTrackBar_ValueChanged);
            // 
            // headerStrip1
            // 
            this.headerStrip1.AutoSize = false;
            this.headerStrip1.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.headerStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.headerStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panelDesignerCloseButton,
            this.panelDesignerPanelCountLabel});
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.headerStrip1.Size = new System.Drawing.Size(348, 25);
            this.headerStrip1.TabIndex = 6;
            this.headerStrip1.Text = "headerStrip1";
            // 
            // panelDesignerCloseButton
            // 
            this.panelDesignerCloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.panelDesignerCloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.panelDesignerCloseButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.panelDesignerCloseButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.panelDesignerCloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.panelDesignerCloseButton.Name = "panelDesignerCloseButton";
            this.panelDesignerCloseButton.Size = new System.Drawing.Size(23, 20);
            this.panelDesignerCloseButton.Text = "Collapse Designer";
            this.panelDesignerCloseButton.Click += new System.EventHandler(this.panelDesignerCloseButton_Click);
            // 
            // panelDesignerPanelCountLabel
            // 
            this.panelDesignerPanelCountLabel.Enabled = false;
            this.panelDesignerPanelCountLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.panelDesignerPanelCountLabel.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.panelDesignerPanelCountLabel.Name = "panelDesignerPanelCountLabel";
            this.panelDesignerPanelCountLabel.Size = new System.Drawing.Size(53, 20);
            this.panelDesignerPanelCountLabel.Text = "9 panels";
            // 
            // dashboardPanelDetails
            // 
            this.dashboardPanelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dashboardPanelDetails.BackColor = System.Drawing.Color.White;
            this.dashboardPanelDetails.Location = new System.Drawing.Point(154, 135);
            this.dashboardPanelDetails.Name = "dashboardPanelDetails";
            this.dashboardPanelDetails.Size = new System.Drawing.Size(520, 270);
            this.dashboardPanelDetails.TabIndex = 0;
            this.dashboardPanelDetails.Visible = false;
            // 
            // ServerSummaryView4
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.dashboardPanelDetails);
            this.Controls.Add(this.panelGalleryPanel);
            this.Controls.Add(this.serverOverviewContentPanel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Controls.Add(this.viewStatusPanel);
            this.Name = "ServerSummaryView4";
            this.Size = new System.Drawing.Size(1024, 768);
            this.Load += new System.EventHandler(this.ServerSummaryView_Load);
            this.serverOverviewContentPanel.ResumeLayout(false);
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            this.viewStatusPanel.Panel1.ResumeLayout(false);
            this.viewStatusPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusPanel)).EndInit();
            this.viewStatusPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusImage)).EndInit();
            this.panelGalleryPanel.ResumeLayout(false);
            this.panelGalleryGradientPanel.ResumeLayout(false);
            this.galleryPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelSelectorTrackBar)).EndInit();
            this.headerStrip1.ResumeLayout(false);
            this.headerStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  serverOverviewContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel operationalStatusLabel;
        private System.Windows.Forms.SplitContainer viewStatusPanel;
        private MRG.Controls.UI.LoadingCircle viewStatusConnectingCircle;
        private System.Windows.Forms.PictureBox viewStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel viewStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel linkLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panelGalleryPanel;
        private Controls.GradientPanel panelGalleryGradientPanel;
        private Controls.HeaderStrip headerStrip1;
        private System.Windows.Forms.ToolStripButton panelDesignerCloseButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel galleryTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel dashboardTableLayoutPanel;
        private Infragistics.Win.UltraWinEditors.UltraTrackBar panelSelectorTrackBar;
        private System.Windows.Forms.ToolStripLabel panelDesignerPanelCountLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  galleryPanel;
        private Controls.ServerSummary.Dashboard.DashboardPanelDetails dashboardPanelDetails;
    }
}
