namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.DesktopClient.Properties;
    using System.Drawing;

    partial class SystemDiagnosticsDialog
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
                if (testValueList       != null) testValueList.Dispose();
                if (componentValueList  != null) componentValueList.Dispose();
                if (testStatusValueList != null) testStatusValueList.Dispose();

                if(components != null)
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemDiagnosticsDialog));
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("ServerPermission", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Server", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PermissionType");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(84474157);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this._pnlDiagnostics = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.desktopRepositoryDatabaseLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._generalHdrStrpStatus = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.desktopRepositoryHostLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.systemDiagTestControl1 = new Idera.SQLdm.DesktopClient.Controls.SystemDiagTestControl();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.viewStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.viewStatusConnectingCircle = new MRG.Controls.UI.LoadingCircle();
            this.viewStatusImage = new System.Windows.Forms.PictureBox();
            this.viewStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage2 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.stackLayoutPanelPermission = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this._pnlNoPermissions = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._pnlNoPermissionInfo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this._pnlPermissions = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._pnlPermissionStatusHdr = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._pnlPermissionServersGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this._pnlPermissionSysadminLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._pnlPermissionAdminVal = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._pnlPermissionAdminLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._pnlPermissionSysadminVal = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._pnlPermissionAssignedServersHdr = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._tabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.refreshButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.configToolButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.collectAndLogButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.ultraTabPageControl2.SuspendLayout();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            this._pnlDiagnostics.SuspendLayout();
            this.stackLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusPanel)).BeginInit();
            this.viewStatusPanel.Panel1.SuspendLayout();
            this.viewStatusPanel.Panel2.SuspendLayout();
            this.viewStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusImage)).BeginInit();
            this.ultraTabPageControl3.SuspendLayout();
            this.office2007PropertyPage2.ContentPanel.SuspendLayout();
            this.stackLayoutPanelPermission.SuspendLayout();
            this._pnlNoPermissions.SuspendLayout();
            this._pnlPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pnlPermissionServersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._tabControl)).BeginInit();
            this._tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.office2007PropertyPage1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(141, 0);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(426, 531);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this._pnlDiagnostics);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(424, 474);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.HomeTabMonitorView;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(426, 531);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.TabStop = false;
            this.office2007PropertyPage1.Text = "General diagnostic information";
            // 
            // _pnlDiagnostics
            // 
            this._pnlDiagnostics.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._pnlDiagnostics.Controls.Add(this.propertiesHeaderStrip1);
            this._pnlDiagnostics.Controls.Add(this.desktopRepositoryDatabaseLabel);
            this._pnlDiagnostics.Controls.Add(this._generalHdrStrpStatus);
            this._pnlDiagnostics.Controls.Add(this.desktopRepositoryHostLabel);
            this._pnlDiagnostics.Controls.Add(this.label12);
            this._pnlDiagnostics.Controls.Add(this.stackLayoutPanel1);
            this._pnlDiagnostics.Controls.Add(this.label10);
            this._pnlDiagnostics.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlDiagnostics.Location = new System.Drawing.Point(1, 1);
            this._pnlDiagnostics.Name = "_pnlDiagnostics";
            this._pnlDiagnostics.Padding = new System.Windows.Forms.Padding(5);
            this._pnlDiagnostics.Size = new System.Drawing.Size(422, 472);
            this._pnlDiagnostics.TabIndex = 1;
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(8, 77);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(406, 25);
            this.propertiesHeaderStrip1.TabIndex = 20;
            this.propertiesHeaderStrip1.TabStop = false;
            this.propertiesHeaderStrip1.Text = "Status";
            this.propertiesHeaderStrip1.WordWrap = false;
            // 
            // desktopRepositoryDatabaseLabel
            // 
            this.desktopRepositoryDatabaseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.desktopRepositoryDatabaseLabel.AutoEllipsis = true;
            this.desktopRepositoryDatabaseLabel.Location = new System.Drawing.Point(173, 51);
            this.desktopRepositoryDatabaseLabel.Name = "desktopRepositoryDatabaseLabel";
            this.desktopRepositoryDatabaseLabel.Size = new System.Drawing.Size(167, 13);
            this.desktopRepositoryDatabaseLabel.TabIndex = 19;
            this.desktopRepositoryDatabaseLabel.Text = "{0}";
            // 
            // _generalHdrStrpStatus
            // 
            this._generalHdrStrpStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._generalHdrStrpStatus.ForeColor = System.Drawing.Color.Black;
            this._generalHdrStrpStatus.Location = new System.Drawing.Point(8, 8);
            this._generalHdrStrpStatus.Name = "_generalHdrStrpStatus";
            this._generalHdrStrpStatus.Size = new System.Drawing.Size(406, 25);
            this._generalHdrStrpStatus.TabIndex = 13;
            this._generalHdrStrpStatus.TabStop = false;
            this._generalHdrStrpStatus.Text = "Repository";
            this._generalHdrStrpStatus.WordWrap = false;
            // 
            // desktopRepositoryHostLabel
            // 
            this.desktopRepositoryHostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.desktopRepositoryHostLabel.AutoEllipsis = true;
            this.desktopRepositoryHostLabel.Location = new System.Drawing.Point(173, 36);
            this.desktopRepositoryHostLabel.Name = "desktopRepositoryHostLabel";
            this.desktopRepositoryHostLabel.Size = new System.Drawing.Size(167, 13);
            this.desktopRepositoryHostLabel.TabIndex = 15;
            this.desktopRepositoryHostLabel.Text = "{0}";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(145, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Desktop repository database:";
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = this.panel1;
            this.stackLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stackLayoutPanel1.Controls.Add(this.panel1);
            this.stackLayoutPanel1.Controls.Add(this.panel2);
            this.stackLayoutPanel1.Location = new System.Drawing.Point(8, 108);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Size = new System.Drawing.Size(406, 356);
            this.stackLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.systemDiagTestControl1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 356);
            this.panel1.TabIndex = 2;
            // 
            // systemDiagTestControl1
            // 
            this.systemDiagTestControl1.BackColor = System.Drawing.Color.Transparent;
            this.systemDiagTestControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.systemDiagTestControl1.DividerVisible = true;
            this.systemDiagTestControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.systemDiagTestControl1.Location = new System.Drawing.Point(0, 0);
            this.systemDiagTestControl1.Margin = new System.Windows.Forms.Padding(0);
            this.systemDiagTestControl1.Name = "systemDiagTestControl1";
            this.systemDiagTestControl1.Selected = false;
            this.systemDiagTestControl1.Size = new System.Drawing.Size(404, 66);
            this.systemDiagTestControl1.TabIndex = 0;
            this.systemDiagTestControl1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.viewStatusPanel);
            this.panel2.Location = new System.Drawing.Point(71, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(555, 282);
            this.panel2.TabIndex = 1;
            // 
            // viewStatusPanel
            // 
            this.viewStatusPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.viewStatusPanel.IsSplitterFixed = true;
            this.viewStatusPanel.Location = new System.Drawing.Point(70, 93);
            this.viewStatusPanel.Name = "viewStatusPanel";
            // 
            // viewStatusPanel.Panel1
            // 
            this.viewStatusPanel.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(178)))), ((int)(((byte)(227)))));
            this.viewStatusPanel.Panel1.Controls.Add(this.viewStatusConnectingCircle);
            this.viewStatusPanel.Panel1.Controls.Add(this.viewStatusImage);
            this.viewStatusPanel.Panel1.Padding = new System.Windows.Forms.Padding(1);
            // 
            // viewStatusPanel.Panel2
            // 
            this.viewStatusPanel.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(178)))), ((int)(((byte)(227)))));
            this.viewStatusPanel.Panel2.Controls.Add(this.viewStatusLabel);
            this.viewStatusPanel.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.viewStatusPanel.Size = new System.Drawing.Size(417, 88);
            this.viewStatusPanel.SplitterDistance = 87;
            this.viewStatusPanel.TabIndex = 6;
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
            this.viewStatusConnectingCircle.Size = new System.Drawing.Size(85, 86);
            this.viewStatusConnectingCircle.SpokeThickness = 2;
            this.viewStatusConnectingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.viewStatusConnectingCircle.TabIndex = 0;
            // 
            // viewStatusImage
            // 
            this.viewStatusImage.BackColor = System.Drawing.Color.White;
            this.viewStatusImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewStatusImage.Image = ((System.Drawing.Image)(resources.GetObject("viewStatusImage.Image")));
            this.viewStatusImage.Location = new System.Drawing.Point(1, 1);
            this.viewStatusImage.Name = "viewStatusImage";
            this.viewStatusImage.Size = new System.Drawing.Size(85, 86);
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
            this.viewStatusLabel.Size = new System.Drawing.Size(324, 86);
            this.viewStatusLabel.TabIndex = 24;
            this.viewStatusLabel.TabStop = true;
            this.viewStatusLabel.Text = "< status message >";
            this.viewStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Desktop repository host:";
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.office2007PropertyPage2);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(426, 531);
            // 
            // office2007PropertyPage2
            // 
            this.office2007PropertyPage2.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage2.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage2.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage2.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.stackLayoutPanelPermission);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.label2);
            this.office2007PropertyPage2.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage2.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage2.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage2.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage2.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage2.ContentPanel.Size = new System.Drawing.Size(424, 474);
            this.office2007PropertyPage2.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.UserPermission32x32;
            this.office2007PropertyPage2.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage2.Name = "office2007PropertyPage2";
            this.office2007PropertyPage2.Size = new System.Drawing.Size(426, 531);
            this.office2007PropertyPage2.TabIndex = 0;
            this.office2007PropertyPage2.TabStop = false;
            this.office2007PropertyPage2.Text = "SQLDM permissions assigned to me";
            // 
            // stackLayoutPanelPermission
            // 
            this.stackLayoutPanelPermission.ActiveControl = null;
            this.stackLayoutPanelPermission.Controls.Add(this._pnlNoPermissions);
            this.stackLayoutPanelPermission.Controls.Add(this._pnlPermissions);
            this.stackLayoutPanelPermission.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackLayoutPanelPermission.Location = new System.Drawing.Point(1, 1);
            this.stackLayoutPanelPermission.Name = "stackLayoutPanelPermission";
            this.stackLayoutPanelPermission.Size = new System.Drawing.Size(422, 472);
            this.stackLayoutPanelPermission.TabIndex = 31;
            // 
            // _pnlNoPermissions
            // 
            this._pnlNoPermissions.Controls.Add(this._pnlNoPermissionInfo);
            this._pnlNoPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlNoPermissions.Location = new System.Drawing.Point(0, 0);
            this._pnlNoPermissions.Name = "_pnlNoPermissions";
            this._pnlNoPermissions.Size = new System.Drawing.Size(422, 472);
            this._pnlNoPermissions.TabIndex = 33;
            // 
            // _pnlNoPermissionInfo
            // 
            this._pnlNoPermissionInfo.Location = new System.Drawing.Point(8, 8);
            this._pnlNoPermissionInfo.Name = "_pnlNoPermissionInfo";
            this._pnlNoPermissionInfo.Size = new System.Drawing.Size(406, 80);
            this._pnlNoPermissionInfo.TabIndex = 1;
            this._pnlNoPermissionInfo.Text = "  SQLDM application security is not enabled.  You have full access to SQLDM.";
            // 
            // _pnlPermissions
            // 
            this._pnlPermissions.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._pnlPermissions.Controls.Add(this._pnlPermissionStatusHdr);
            this._pnlPermissions.Controls.Add(this._pnlPermissionServersGrid);
            this._pnlPermissions.Controls.Add(this._pnlPermissionSysadminLbl);
            this._pnlPermissions.Controls.Add(this._pnlPermissionAdminVal);
            this._pnlPermissions.Controls.Add(this._pnlPermissionAdminLbl);
            this._pnlPermissions.Controls.Add(this._pnlPermissionSysadminVal);
            this._pnlPermissions.Controls.Add(this._pnlPermissionAssignedServersHdr);
            this._pnlPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlPermissions.Location = new System.Drawing.Point(0, 0);
            this._pnlPermissions.Name = "_pnlPermissions";
            this._pnlPermissions.Size = new System.Drawing.Size(422, 472);
            this._pnlPermissions.TabIndex = 32;
            // 
            // _pnlPermissionStatusHdr
            // 
            this._pnlPermissionStatusHdr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlPermissionStatusHdr.ForeColor = System.Drawing.Color.Black;
            this._pnlPermissionStatusHdr.Location = new System.Drawing.Point(8, 8);
            this._pnlPermissionStatusHdr.Name = "_pnlPermissionStatusHdr";
            this._pnlPermissionStatusHdr.Size = new System.Drawing.Size(406, 25);
            this._pnlPermissionStatusHdr.TabIndex = 21;
            this._pnlPermissionStatusHdr.TabStop = false;
            this._pnlPermissionStatusHdr.Text = "Status";
            this._pnlPermissionStatusHdr.WordWrap = false;
            // 
            // _pnlPermissionServersGrid
            // 
            this._pnlPermissionServersGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlPermissionServersGrid.DataSource = this.bindingSource1;
            appearance16.BackColor = System.Drawing.SystemColors.Window;
            appearance16.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this._pnlPermissionServersGrid.DisplayLayout.Appearance = appearance16;
            this._pnlPermissionServersGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 262;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.Caption = "Permission";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 91;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this._pnlPermissionServersGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this._pnlPermissionServersGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this._pnlPermissionServersGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance17.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance17.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this._pnlPermissionServersGrid.DisplayLayout.GroupByBox.Appearance = appearance17;
            appearance18.ForeColor = System.Drawing.SystemColors.GrayText;
            this._pnlPermissionServersGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance18;
            this._pnlPermissionServersGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this._pnlPermissionServersGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance19.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance19.BackColor2 = System.Drawing.SystemColors.Control;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.ForeColor = System.Drawing.SystemColors.GrayText;
            this._pnlPermissionServersGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance19;
            this._pnlPermissionServersGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this._pnlPermissionServersGrid.DisplayLayout.MaxColScrollRegions = 1;
            this._pnlPermissionServersGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this._pnlPermissionServersGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this._pnlPermissionServersGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this._pnlPermissionServersGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this._pnlPermissionServersGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this._pnlPermissionServersGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this._pnlPermissionServersGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            this._pnlPermissionServersGrid.DisplayLayout.Override.CardAreaAppearance = appearance20;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            appearance21.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this._pnlPermissionServersGrid.DisplayLayout.Override.CellAppearance = appearance21;
            this._pnlPermissionServersGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this._pnlPermissionServersGrid.DisplayLayout.Override.CellPadding = 0;
            this._pnlPermissionServersGrid.DisplayLayout.Override.DefaultRowHeight = 20;
            this._pnlPermissionServersGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this._pnlPermissionServersGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.BorderColor = System.Drawing.SystemColors.Window;
            this._pnlPermissionServersGrid.DisplayLayout.Override.GroupByRowAppearance = appearance22;
            this._pnlPermissionServersGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Collapsed;
            appearance23.TextHAlignAsString = "Left";
            this._pnlPermissionServersGrid.DisplayLayout.Override.HeaderAppearance = appearance23;
            this._pnlPermissionServersGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this._pnlPermissionServersGrid.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.BorderColor = System.Drawing.Color.Silver;
            this._pnlPermissionServersGrid.DisplayLayout.Override.RowAppearance = appearance24;
            this._pnlPermissionServersGrid.DisplayLayout.Override.RowSelectorHeaderStyle = Infragistics.Win.UltraWinGrid.RowSelectorHeaderStyle.None;
            this._pnlPermissionServersGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this._pnlPermissionServersGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._pnlPermissionServersGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._pnlPermissionServersGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this._pnlPermissionServersGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLight;
            this._pnlPermissionServersGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance25;
            this._pnlPermissionServersGrid.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.False;
            this._pnlPermissionServersGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this._pnlPermissionServersGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this._pnlPermissionServersGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "BooleanYesNo";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = false;
            valueListItem1.DisplayText = "No";
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "Yes";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this._pnlPermissionServersGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this._pnlPermissionServersGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this._pnlPermissionServersGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this._pnlPermissionServersGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._pnlPermissionServersGrid.Location = new System.Drawing.Point(8, 108);
            this._pnlPermissionServersGrid.Name = "_pnlPermissionServersGrid";
            this._pnlPermissionServersGrid.Size = new System.Drawing.Size(406, 361);
            this._pnlPermissionServersGrid.TabIndex = 30;
            this._pnlPermissionServersGrid.TabStop = false;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(Idera.SQLdm.Common.Objects.ApplicationSecurity.ServerPermission);
            // 
            // _pnlPermissionSysadminLbl
            // 
            this._pnlPermissionSysadminLbl.AutoSize = true;
            this._pnlPermissionSysadminLbl.Location = new System.Drawing.Point(5, 36);
            this._pnlPermissionSysadminLbl.Name = "_pnlPermissionSysadminLbl";
            this._pnlPermissionSysadminLbl.Size = new System.Drawing.Size(126, 13);
            this._pnlPermissionSysadminLbl.TabIndex = 24;
            this._pnlPermissionSysadminLbl.Text = "Member of sysadmin role:";
            // 
            // _pnlPermissionAdminVal
            // 
            this._pnlPermissionAdminVal.AutoSize = true;
            this._pnlPermissionAdminVal.Location = new System.Drawing.Point(137, 49);
            this._pnlPermissionAdminVal.Name = "_pnlPermissionAdminVal";
            this._pnlPermissionAdminVal.Size = new System.Drawing.Size(25, 13);
            this._pnlPermissionAdminVal.TabIndex = 29;
            this._pnlPermissionAdminVal.Text = "Yes";
            // 
            // _pnlPermissionAdminLbl
            // 
            this._pnlPermissionAdminLbl.AutoSize = true;
            this._pnlPermissionAdminLbl.Location = new System.Drawing.Point(5, 49);
            this._pnlPermissionAdminLbl.Name = "_pnlPermissionAdminLbl";
            this._pnlPermissionAdminLbl.Size = new System.Drawing.Size(107, 13);
            this._pnlPermissionAdminLbl.TabIndex = 25;
            this._pnlPermissionAdminLbl.Text = "SQLDM administrator:";
            // 
            // _pnlPermissionSysadminVal
            // 
            this._pnlPermissionSysadminVal.AutoSize = true;
            this._pnlPermissionSysadminVal.Location = new System.Drawing.Point(137, 36);
            this._pnlPermissionSysadminVal.Name = "_pnlPermissionSysadminVal";
            this._pnlPermissionSysadminVal.Size = new System.Drawing.Size(21, 13);
            this._pnlPermissionSysadminVal.TabIndex = 28;
            this._pnlPermissionSysadminVal.Text = "No";
            // 
            // _pnlPermissionAssignedServersHdr
            // 
            this._pnlPermissionAssignedServersHdr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlPermissionAssignedServersHdr.ForeColor = System.Drawing.Color.Black;
            this._pnlPermissionAssignedServersHdr.Location = new System.Drawing.Point(8, 77);
            this._pnlPermissionAssignedServersHdr.Name = "_pnlPermissionAssignedServersHdr";
            this._pnlPermissionAssignedServersHdr.Size = new System.Drawing.Size(406, 25);
            this._pnlPermissionAssignedServersHdr.TabIndex = 26;
            this._pnlPermissionAssignedServersHdr.TabStop = false;
            this._pnlPermissionAssignedServersHdr.Text = "Assigned Servers";
            this._pnlPermissionAssignedServersHdr.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 23;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.SystemColors.Control;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(504, 549);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Close";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // _tabControl
            // 
            appearance7.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
            appearance7.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance7.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this._tabControl.ActiveTabAppearance = appearance7;
            this._tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.TextHAlignAsString = "Left";
            this._tabControl.Appearance = appearance1;
            appearance4.BackColor = System.Drawing.Color.DarkGray;
            this._tabControl.ClientAreaAppearance = appearance4;
            this._tabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this._tabControl.Controls.Add(this.ultraTabPageControl2);
            this._tabControl.Controls.Add(this.ultraTabPageControl3);
            appearance6.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.HotTrackTabBackground;
            appearance6.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance6.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this._tabControl.HotTrackAppearance = appearance6;
            this._tabControl.InterTabSpacing = new Infragistics.Win.DefaultableInteger(1);
            this._tabControl.Location = new System.Drawing.Point(12, 12);
            this._tabControl.MinTabWidth = 26;
            this._tabControl.Name = "_tabControl";
            this._tabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this._tabControl.Size = new System.Drawing.Size(567, 531);
            this._tabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.StateButtons;
            this._tabControl.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            appearance5.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : System.Drawing.Color.White;
            appearance5.BorderColor = System.Drawing.Color.Black;
            this._tabControl.TabHeaderAreaAppearance = appearance5;
            this._tabControl.TabIndex = 0;
            this._tabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop;
            this._tabControl.TabPageMargins.Left = 1;
            ultraTab3.Key = "_generalTab";
            ultraTab3.TabPage = this.ultraTabPageControl2;
            ultraTab3.Text = "General";
            ultraTab5.Key = "_permissionsTab";
            ultraTab5.TabPage = this.ultraTabPageControl3;
            ultraTab5.Text = "My Permissions";
            this._tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab3,
            ultraTab5});
            this._tabControl.TabSize = new System.Drawing.Size(0, 140);
            this._tabControl.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal;
            this._tabControl.UseAppStyling = false;
            this._tabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this._tabControl.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(426, 531);
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.BackColor = System.Drawing.SystemColors.Control;
            this.refreshButton.Location = new System.Drawing.Point(260, 549);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(98, 23);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Run Diagnostics";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // configToolButton
            // 
            this.configToolButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.configToolButton.Location = new System.Drawing.Point(12, 549);
            this.configToolButton.Name = "configToolButton";
            this.configToolButton.Size = new System.Drawing.Size(153, 23);
            this.configToolButton.TabIndex = 3;
            this.configToolButton.Text = "Service Configuration Tool...";
            this.configToolButton.UseVisualStyleBackColor = true;
            this.configToolButton.Visible = false;
            this.configToolButton.Click += new System.EventHandler(this.configToolButton_Click);
            // 
            // collectAndLogButton
            // 
            this.collectAndLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.collectAndLogButton.BackColor = System.Drawing.SystemColors.Control;
            this.collectAndLogButton.Location = new System.Drawing.Point(379, 549);
            this.collectAndLogButton.Name = "collectAndLogButton";
            this.collectAndLogButton.Size = new System.Drawing.Size(119, 23);
            this.collectAndLogButton.TabIndex = 4;
            this.collectAndLogButton.Text = "Run and Gather Logs";
            this.collectAndLogButton.UseVisualStyleBackColor = false;
            this.collectAndLogButton.Click += new System.EventHandler(this.collectAndLogButton_Click);
            // 
            // SystemDiagnosticsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(591, 584);
            this.Controls.Add(this.collectAndLogButton);
            this.Controls.Add(this.configToolButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SystemDiagnosticsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Diagnostics";
            this.Load += new System.EventHandler(this.SystemDiagnosticsDialog_Load);
            this.Shown += new System.EventHandler(this.SystemDiagnosticsDialog_Shown);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this._pnlDiagnostics.ResumeLayout(false);
            this._pnlDiagnostics.PerformLayout();
            this.stackLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.viewStatusPanel.Panel1.ResumeLayout(false);
            this.viewStatusPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusPanel)).EndInit();
            this.viewStatusPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewStatusImage)).EndInit();
            this.ultraTabPageControl3.ResumeLayout(false);
            this.office2007PropertyPage2.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage2.ContentPanel.PerformLayout();
            this.stackLayoutPanelPermission.ResumeLayout(false);
            this._pnlNoPermissions.ResumeLayout(false);
            this._pnlPermissions.ResumeLayout(false);
            this._pnlPermissions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pnlPermissionServersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._tabControl)).EndInit();
            this._tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private System.Windows.Forms.SplitContainer viewStatusPanel;
        private MRG.Controls.UI.LoadingCircle viewStatusConnectingCircle;
        private System.Windows.Forms.PictureBox viewStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel viewStatusLabel;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton refreshButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.SystemDiagTestControl systemDiagTestControl1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _pnlDiagnostics;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl _tabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel desktopRepositoryDatabaseLabel;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _generalHdrStrpStatus;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel desktopRepositoryHostLabel;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _pnlPermissionSysadminLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _pnlPermissionStatusHdr;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _pnlPermissionAdminVal;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _pnlPermissionSysadminVal;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _pnlPermissionAssignedServersHdr;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _pnlPermissionAdminLbl;
        private System.Windows.Forms.BindingSource bindingSource1;
        private Infragistics.Win.UltraWinGrid.UltraGrid _pnlPermissionServersGrid;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanelPermission;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _pnlPermissions;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _pnlNoPermissions;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox _pnlNoPermissionInfo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton configToolButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton collectAndLogButton;
    }
}