using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class PermissionPropertyDialog
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
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagsPopupMenu");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PermissionPropertyDialog));
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this._generalPropertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.mainContainerPanel = new System.Windows.Forms.TableLayoutPanel();
            this._generalHdrStrpStatus = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._generalChkBxEnabled = new System.Windows.Forms.CheckBox();
            this._generalHdrStrpPermission = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._generalRdBtnView = new System.Windows.Forms.RadioButton();
            this._generalRdBtnModify = new System.Windows.Forms.RadioButton();
            this._generalRdBtnAdministrator = new System.Windows.Forms.RadioButton();
            //Operator Security Role Changes - 10.3
            this._generalRdBtnReadOnlyPlus = new System.Windows.Forms.RadioButton();
            this._generalHdrStrpComment = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._generalTxtBxComment = new System.Windows.Forms.TextBox();
            this._generalHdrStrpWebAppPermission = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this._generalChkBxWebAppPermission = new System.Windows.Forms.CheckBox();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this._serversPropertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this._AddPermissionWizard_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AddPermissionWizard_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AddPermissionWizard_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.label1 = new System.Windows.Forms.Label();
            this.tagsDropDownButton = new Infragistics.Win.Misc.UltraDropDownButton();
            this._serversPageInfo = new Divelements.WizardFramework.InformationBox();
            this._serversListSelectorControl = new Idera.SQLdm.DesktopClient.Controls.DualListSelectorControl();
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage7 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.ultraTabPageControl6 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage5 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.smoothingContentPropertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this._tabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnOK = new System.Windows.Forms.Button();
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.initializationWorker = new System.ComponentModel.BackgroundWorker();
            this.ultraTabPageControl2.SuspendLayout();
            this._generalPropertyPage.ContentPanel.SuspendLayout();
            this.mainContainerPanel.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            this._serversPropertyPage.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.ultraTabPageControl8.SuspendLayout();
            this.ultraTabPageControl6.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tabControl)).BeginInit();
            this._tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this._generalPropertyPage);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(141, 0);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(505, 499);
            // 
            // _generalPropertyPage
            // 
            this._generalPropertyPage.BackColor = System.Drawing.Color.White;
            this._generalPropertyPage.BorderWidth = 1;
            // 
            // 
            // 
            this._generalPropertyPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this._generalPropertyPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this._generalPropertyPage.ContentPanel.Controls.Add(this.mainContainerPanel);
            this._generalPropertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._generalPropertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this._generalPropertyPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this._generalPropertyPage.ContentPanel.Name = "ContentPanel";
            this._generalPropertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this._generalPropertyPage.ContentPanel.ShowBorder = false;
            this._generalPropertyPage.ContentPanel.Size = new System.Drawing.Size(503, 442);
            this._generalPropertyPage.ContentPanel.TabIndex = 1;
            this._generalPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._generalPropertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.EditPermission32x32;
            this._generalPropertyPage.Location = new System.Drawing.Point(0, 0);
            this._generalPropertyPage.Name = "_generalPropertyPage";
            this._generalPropertyPage.Size = new System.Drawing.Size(505, 499);
            this._generalPropertyPage.TabIndex = 0;
            this._generalPropertyPage.Text = "Modify common permission properties.";
            // 
            // mainContainerPanel
            // 
            this.mainContainerPanel.BackColor = System.Drawing.Color.White;
            this.mainContainerPanel.ColumnCount = 3;
            this.mainContainerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.mainContainerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainContainerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainerPanel.Controls.Add(this._generalHdrStrpStatus, 1, 0);
            this.mainContainerPanel.Controls.Add(this._generalChkBxEnabled, 2, 1);
            this.mainContainerPanel.Controls.Add(this._generalHdrStrpPermission, 1, 2);
            this.mainContainerPanel.Controls.Add(this._generalRdBtnView, 2, 3);
            this.mainContainerPanel.Controls.Add(this._generalRdBtnModify, 2, 4);
            this.mainContainerPanel.Controls.Add(this._generalRdBtnReadOnlyPlus, 2, 5);
            this.mainContainerPanel.Controls.Add(this._generalRdBtnAdministrator, 2, 6);
            this.mainContainerPanel.Controls.Add(this._generalHdrStrpComment, 1, 7);
            this.mainContainerPanel.Controls.Add(this._generalTxtBxComment, 1, 8);
            this.mainContainerPanel.Controls.Add(this._generalHdrStrpWebAppPermission, 1, 9);
            this.mainContainerPanel.Controls.Add(this._generalChkBxWebAppPermission, 2, 10);
            this.mainContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainerPanel.Location = new System.Drawing.Point(1, 1);
            this.mainContainerPanel.Name = "mainContainerPanel";
            this.mainContainerPanel.RowCount = 12;
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.mainContainerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainContainerPanel.Size = new System.Drawing.Size(501, 460);
            this.mainContainerPanel.TabIndex = 4;
            // 
            // _generalHdrStrpStatus
            // 
            this._generalHdrStrpStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainContainerPanel.SetColumnSpan(this._generalHdrStrpStatus, 2);
            this._generalHdrStrpStatus.ForeColor = System.Drawing.Color.Black;
            this._generalHdrStrpStatus.Location = new System.Drawing.Point(18, 3);
            this._generalHdrStrpStatus.Name = "_generalHdrStrpStatus";
            this._generalHdrStrpStatus.Size = new System.Drawing.Size(480, 25);
            this._generalHdrStrpStatus.TabIndex = 12;
            this._generalHdrStrpStatus.TabStop = false;
            this._generalHdrStrpStatus.Text = "Status";
            this._generalHdrStrpStatus.WordWrap = false;
            // 
            // _generalChkBxEnabled
            // 
            this._generalChkBxEnabled.AutoSize = true;
            this._generalChkBxEnabled.Location = new System.Drawing.Point(38, 34);
            this._generalChkBxEnabled.Name = "_generalChkBxEnabled";
            this._generalChkBxEnabled.Size = new System.Drawing.Size(65, 17);
            this._generalChkBxEnabled.TabIndex = 0;
            this._generalChkBxEnabled.Text = "Enabled";
            this._generalChkBxEnabled.UseVisualStyleBackColor = true;
            this._generalChkBxEnabled.CheckedChanged += new System.EventHandler(this._generalChkBxEnabled_CheckedChanged);
            // 
            // _generalHdrStrpPermission
            // 
            this._generalHdrStrpPermission.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainContainerPanel.SetColumnSpan(this._generalHdrStrpPermission, 2);
            this._generalHdrStrpPermission.ForeColor = System.Drawing.Color.Black;
            this._generalHdrStrpPermission.Location = new System.Drawing.Point(18, 57);
            this._generalHdrStrpPermission.Name = "_generalHdrStrpPermission";
            this._generalHdrStrpPermission.Size = new System.Drawing.Size(480, 25);
            this._generalHdrStrpPermission.TabIndex = 13;
            this._generalHdrStrpPermission.TabStop = false;
            this._generalHdrStrpPermission.Text = "Permission";
            this._generalHdrStrpPermission.WordWrap = false;
            // 
            // _generalRdBtnView
            // 
            this._generalRdBtnView.AutoSize = true;
            this._generalRdBtnView.Checked = true;
            this._generalRdBtnView.Location = new System.Drawing.Point(38, 88);
            this._generalRdBtnView.Name = "_generalRdBtnView";
            this._generalRdBtnView.Size = new System.Drawing.Size(288, 17);
            this._generalRdBtnView.TabIndex = 1;
            this._generalRdBtnView.TabStop = true;
            this._generalRdBtnView.Text = "&View data collected for monitored SQL Server instances";
            this._generalRdBtnView.UseVisualStyleBackColor = true;
            this._generalRdBtnView.CheckedChanged += new System.EventHandler(this._generalRdBtnView_CheckedChanged);
            // 
            // _generalRdBtnModify
            // 
            this._generalRdBtnModify.AutoSize = true;
            this._generalRdBtnModify.Location = new System.Drawing.Point(38, 111);
            this._generalRdBtnModify.Name = "_generalRdBtnModify";
            this._generalRdBtnModify.Size = new System.Drawing.Size(406, 17);
            this._generalRdBtnModify.TabIndex = 17;
            this._generalRdBtnModify.Text = "&Modify configuration and view data collected for monitored SQL Server instances";
            this._generalRdBtnModify.UseVisualStyleBackColor = true;
            this._generalRdBtnModify.CheckedChanged += new System.EventHandler(this._generalRdBtnModify_CheckedChanged);
			// 
			// _generalRdBtnReadOnlyPlus
			// 
			this._generalRdBtnReadOnlyPlus.AutoSize = true;
			this._generalRdBtnReadOnlyPlus.Location = new System.Drawing.Point(38, 134);
			this._generalRdBtnReadOnlyPlus.Name = "_generalRdBtnReadOnlyPlus";
			this._generalRdBtnReadOnlyPlus.Size = new System.Drawing.Size(406, 17);
			this._generalRdBtnReadOnlyPlus.TabIndex = 18;
			this._generalRdBtnReadOnlyPlus.Text = "&View data, acknowledge alarms, and control maintenance mode status";
			this._generalRdBtnReadOnlyPlus.UseVisualStyleBackColor = true;
			this._generalRdBtnReadOnlyPlus.CheckedChanged += new System.EventHandler(this._generalRdBtnReadOnlyPlus_CheckedChanged);
            // 
            // _generalRdBtnAdministrator
            // 
            this._generalRdBtnAdministrator.AutoSize = true;
            this._generalRdBtnAdministrator.Location = new System.Drawing.Point(38, 157);
            this._generalRdBtnAdministrator.Name = "_generalRdBtnAdministrator";
            this._generalRdBtnAdministrator.Size = new System.Drawing.Size(252, 17);
            this._generalRdBtnAdministrator.TabIndex = 19;
            this._generalRdBtnAdministrator.Text = "&Administrator powers in SQL Diagnostic Manager";
            this._generalRdBtnAdministrator.UseVisualStyleBackColor = true;
            this._generalRdBtnAdministrator.CheckedChanged += new System.EventHandler(this._generalRdBtnAdministrator_CheckedChanged);
            // 
            // _generalHdrStrpComment
            // 
            this._generalHdrStrpComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainContainerPanel.SetColumnSpan(this._generalHdrStrpComment, 2);
            this._generalHdrStrpComment.ForeColor = System.Drawing.Color.Black;
            this._generalHdrStrpComment.Location = new System.Drawing.Point(18, 182);
            this._generalHdrStrpComment.Name = "_generalHdrStrpComment";
            this._generalHdrStrpComment.Size = new System.Drawing.Size(480, 25);
            this._generalHdrStrpComment.TabIndex = 14;
            this._generalHdrStrpComment.TabStop = false;
            this._generalHdrStrpComment.Text = "Comment";
            this._generalHdrStrpComment.WordWrap = false;
            // 
            // _generalTxtBxComment
            // 
            this.mainContainerPanel.SetColumnSpan(this._generalTxtBxComment, 2);
            this._generalTxtBxComment.Dock = System.Windows.Forms.DockStyle.Fill;
            this._generalTxtBxComment.Location = new System.Drawing.Point(18, 213);
            this._generalTxtBxComment.MaxLength = 1024;
            this._generalTxtBxComment.Multiline = true;
            this._generalTxtBxComment.Name = "_generalTxtBxComment";
            this._generalTxtBxComment.Size = new System.Drawing.Size(480, 153);
            this._generalTxtBxComment.TabIndex = 2;
            this._generalTxtBxComment.TextChanged += new System.EventHandler(this._generalTxtBxComment_TextChanged);
            // 
            // _generalHdrStrpWebAppPermission
            // 
            this.mainContainerPanel.SetColumnSpan(this._generalHdrStrpWebAppPermission, 2);
            this._generalHdrStrpWebAppPermission.ForeColor = System.Drawing.Color.Black;
            this._generalHdrStrpWebAppPermission.Location = new System.Drawing.Point(18, 372);
            this._generalHdrStrpWebAppPermission.Name = "_generalHdrStrpWebAppPermission";
            this._generalHdrStrpWebAppPermission.Size = new System.Drawing.Size(480, 23);
            this._generalHdrStrpWebAppPermission.TabIndex = 20;
            this._generalHdrStrpWebAppPermission.TabStop = false;
            this._generalHdrStrpWebAppPermission.Text = "Web Application Permission";
            this._generalHdrStrpWebAppPermission.WordWrap = false;
            // 
            // _generalChkBxWebAppPermission
            // 
            this._generalChkBxWebAppPermission.AutoSize = true;
            this._generalChkBxWebAppPermission.Checked = false;
            this._generalChkBxWebAppPermission.CheckState = System.Windows.Forms.CheckState.Checked;
            this._generalChkBxWebAppPermission.Location = new System.Drawing.Point(38, 401);
            this._generalChkBxWebAppPermission.Name = "_generalChkBxWebAppPermission";
            this._generalChkBxWebAppPermission.Size = new System.Drawing.Size(171, 16);
            this._generalChkBxWebAppPermission.TabIndex = 21;
            this._generalChkBxWebAppPermission.Text = "Grant Web Application Access";
            this._generalChkBxWebAppPermission.UseVisualStyleBackColor = true;
            this._generalChkBxWebAppPermission.CheckedChanged += new System.EventHandler(this._generalChkBxWebAppPermission_CheckedChanged);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this._serversPropertyPage);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(505, 499);
            // 
            // _serversPropertyPage
            // 
            this._serversPropertyPage.BackColor = System.Drawing.Color.White;
            this._serversPropertyPage.BorderWidth = 1;
            // 
            // 
            // 
            this._serversPropertyPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this._serversPropertyPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this._serversPropertyPage.ContentPanel.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Top);
            this._serversPropertyPage.ContentPanel.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Bottom);
            this._serversPropertyPage.ContentPanel.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Left);
            this._serversPropertyPage.ContentPanel.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Right);
            this._serversPropertyPage.ContentPanel.Controls.Add(this.label1);
            this._serversPropertyPage.ContentPanel.Controls.Add(this.tagsDropDownButton);
            this._serversPropertyPage.ContentPanel.Controls.Add(this._serversPageInfo);
            this._serversPropertyPage.ContentPanel.Controls.Add(this._serversListSelectorControl);
            this._serversPropertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serversPropertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this._serversPropertyPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this._serversPropertyPage.ContentPanel.Name = "ContentPanel";
            this._serversPropertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this._serversPropertyPage.ContentPanel.ShowBorder = false;
            this._serversPropertyPage.ContentPanel.Size = new System.Drawing.Size(503, 442);
            this._serversPropertyPage.ContentPanel.TabIndex = 1;
            this._serversPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serversPropertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.EditPermission32x32;
            this._serversPropertyPage.Location = new System.Drawing.Point(0, 0);
            this._serversPropertyPage.Name = "_serversPropertyPage";
            this._serversPropertyPage.Size = new System.Drawing.Size(505, 499);
            this._serversPropertyPage.TabIndex = 0;
            this._serversPropertyPage.Text = "Select the SQL Servers to assign to the permission.";
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Top
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(1, 1);
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Top";
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(501, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.DockWithinContainer = this;
            this.toolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "Tags Popup Menu";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Bottom
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(1, 441);
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Bottom";
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(501, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Left
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(1, 1);
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Left";
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 440);
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Right
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(502, 1);
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Right";
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 440);
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Tags:";
            // 
            // tagsDropDownButton
            // 
            this.tagsDropDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BorderColor = System.Drawing.SystemColors.ControlDark;
            appearance1.TextHAlignAsString = "Left";
            this.tagsDropDownButton.Appearance = appearance1;
            this.tagsDropDownButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.tagsDropDownButton.Location = new System.Drawing.Point(23, 105);
            this.tagsDropDownButton.Name = "tagsDropDownButton";
            this.tagsDropDownButton.PopupItemKey = "tagsPopupMenu";
            this.tagsDropDownButton.PopupItemProvider = this.toolbarsManager;
            this.tagsDropDownButton.ShowFocusRect = false;
            this.tagsDropDownButton.ShowOutline = false;
            this.tagsDropDownButton.Size = new System.Drawing.Size(459, 22);
            this.tagsDropDownButton.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.tagsDropDownButton.TabIndex = 24;
            this.tagsDropDownButton.UseAppStyling = false;
            this.tagsDropDownButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.tagsDropDownButton.WrapText = false;
            // 
            // _serversPageInfo
            // 
            this._serversPageInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serversPageInfo.Location = new System.Drawing.Point(23, 3);
            this._serversPageInfo.Name = "_serversPageInfo";
            this._serversPageInfo.Size = new System.Drawing.Size(459, 84);
            this._serversPageInfo.TabIndex = 1;
            this._serversPageInfo.Text = resources.GetString("_serversPageInfo.Text");
            // 
            // _serversListSelectorControl
            // 
            this._serversListSelectorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._serversListSelectorControl.AutoSize = true;
            this._serversListSelectorControl.AvailableLabel = "Available Servers";
            this._serversListSelectorControl.BackColor = System.Drawing.Color.White;
            this._serversListSelectorControl.Location = new System.Drawing.Point(24, 136);
            this._serversListSelectorControl.Name = "_serversListSelectorControl";
            this._serversListSelectorControl.SelectedLabel = "Selected Servers";
            this._serversListSelectorControl.Size = new System.Drawing.Size(457, 297);
            this._serversListSelectorControl.TabIndex = 0;
            this._serversListSelectorControl.SelectionChanged += new System.EventHandler(this._serversListSelectorControl_SelectionChanged);
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Controls.Add(this.office2007PropertyPage7);
            this.ultraTabPageControl8.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(505, 478);
            // 
            // office2007PropertyPage7
            // 
            this.office2007PropertyPage7.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage7.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage7.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage7.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage7.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage7.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage7.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage7.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage7.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage7.ContentPanel.Size = new System.Drawing.Size(505, 423);
            this.office2007PropertyPage7.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage7.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage7.Name = "office2007PropertyPage7";
            this.office2007PropertyPage7.Size = new System.Drawing.Size(505, 478);
            this.office2007PropertyPage7.TabIndex = 1;
            this.office2007PropertyPage7.Text = "Filter out objects for which you don\'t wish to receive alerts.";
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Controls.Add(this.office2007PropertyPage5);
            this.ultraTabPageControl6.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl6.Name = "ultraTabPageControl6";
            this.ultraTabPageControl6.Size = new System.Drawing.Size(505, 478);
            // 
            // office2007PropertyPage5
            // 
            this.office2007PropertyPage5.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage5.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage5.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage5.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage5.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage5.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage5.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage5.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage5.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage5.ContentPanel.Size = new System.Drawing.Size(505, 423);
            this.office2007PropertyPage5.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage5.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage5.Name = "office2007PropertyPage5";
            this.office2007PropertyPage5.Size = new System.Drawing.Size(505, 478);
            this.office2007PropertyPage5.TabIndex = 0;
            this.office2007PropertyPage5.Text = "Configure data collection alerts.";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.smoothingContentPropertyPage);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(570, 480);
            // 
            // smoothingContentPropertyPage
            // 
            this.smoothingContentPropertyPage.BackColor = System.Drawing.Color.White;
            this.smoothingContentPropertyPage.BorderWidth = 0;
            // 
            // 
            // 
            this.smoothingContentPropertyPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.smoothingContentPropertyPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.smoothingContentPropertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.smoothingContentPropertyPage.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.smoothingContentPropertyPage.ContentPanel.Name = "ContentPanel";
            this.smoothingContentPropertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.smoothingContentPropertyPage.ContentPanel.ShowBorder = false;
            this.smoothingContentPropertyPage.ContentPanel.Size = new System.Drawing.Size(570, 425);
            this.smoothingContentPropertyPage.ContentPanel.TabIndex = 1;
            this.smoothingContentPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.smoothingContentPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.smoothingContentPropertyPage.Name = "smoothingContentPropertyPage";
            this.smoothingContentPropertyPage.Size = new System.Drawing.Size(570, 480);
            this.smoothingContentPropertyPage.TabIndex = 0;
            this.smoothingContentPropertyPage.Text = "Customize when an alert should be raised.";
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(505, 499);
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
            appearance3.BackColor = System.Drawing.Color.Transparent;
            appearance3.TextHAlignAsString = "Left";
            this._tabControl.Appearance = appearance3;
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
            this._tabControl.Size = new System.Drawing.Size(646, 499);
            this._tabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.StateButtons;
            this._tabControl.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            appearance5.BackColor = System.Drawing.Color.White;
            appearance5.BorderColor = System.Drawing.Color.Black;
            this._tabControl.TabHeaderAreaAppearance = appearance5;
            this._tabControl.TabIndex = 0;
            this._tabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop;
            this._tabControl.TabPageMargins.Left = 1;
            ultraTab3.Key = "_generalTab";
            ultraTab3.TabPage = this.ultraTabPageControl2;
            ultraTab3.Text = "General";
            ultraTab5.Key = "_assignedServersTab";
            ultraTab5.TabPage = this.ultraTabPageControl3;
            ultraTab5.Text = "Assigned Servers";
            this._tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab3,
            ultraTab5});
            this._tabControl.TabSize = new System.Drawing.Size(0, 140);
            this._tabControl.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal;
            this._tabControl.UseAppStyling = false;
            this._tabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this._tabControl.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(582, 517);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "&Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _btnOK
            // 
            this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Location = new System.Drawing.Point(501, 517);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 1;
            this._btnOK.Text = "&OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
            // 
            // _PermissionPropertyDialog_Toolbars_Dock_Area_Left
            // 
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.Name = "_PermissionPropertyDialog_Toolbars_Dock_Area_Left";
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 552);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _PermissionPropertyDialog_Toolbars_Dock_Area_Right
            // 
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(670, 0);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.Name = "_PermissionPropertyDialog_Toolbars_Dock_Area_Right";
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 552);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _PermissionPropertyDialog_Toolbars_Dock_Area_Top
            // 
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.Name = "_PermissionPropertyDialog_Toolbars_Dock_Area_Top";
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(670, 0);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _PermissionPropertyDialog_Toolbars_Dock_Area_Bottom
            // 
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 552);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.Name = "_PermissionPropertyDialog_Toolbars_Dock_Area_Bottom";
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(670, 0);
            this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // initializationWorker
            // 
            this.initializationWorker.WorkerSupportsCancellation = true;
            this.initializationWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.initializationWorker_DoWork);
            this.initializationWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.initializationWorker_RunWorkerCompleted);
            // 
            // PermissionPropertyDialog
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(670, 552);
            this.Controls.Add(this._btnOK);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._PermissionPropertyDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._PermissionPropertyDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._PermissionPropertyDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._PermissionPropertyDialog_Toolbars_Dock_Area_Bottom);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PermissionPropertyDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Permission Properties - {0} ";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.PermissionPropertyDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PermissionPropertyDialog_FormClosing);
            this.SizeChanged += new System.EventHandler(this.PermissionPropertyDialog_SizeChanged);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.PermissionPropertyDialog_HelpRequested);
            this.ultraTabPageControl2.ResumeLayout(false);
            this._generalPropertyPage.ContentPanel.ResumeLayout(false);
            this.mainContainerPanel.ResumeLayout(false);
            this.mainContainerPanel.PerformLayout();
            this.ultraTabPageControl3.ResumeLayout(false);
            this._serversPropertyPage.ContentPanel.ResumeLayout(false);
            this._serversPropertyPage.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ultraTabPageControl8.ResumeLayout(false);
            this.ultraTabPageControl6.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._tabControl)).EndInit();
            this._tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl8;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage7;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl6;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage5;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage smoothingContentPropertyPage;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl _tabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage _generalPropertyPage;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.TextBox _generalTxtBxComment;
        private System.Windows.Forms.RadioButton _generalRdBtnAdministrator;
        private System.Windows.Forms.RadioButton _generalRdBtnModify;
        private System.Windows.Forms.RadioButton _generalRdBtnView;
        //Operator Security Role Changes - 10.3
        private System.Windows.Forms.RadioButton _generalRdBtnReadOnlyPlus;
        private System.Windows.Forms.CheckBox _generalChkBxEnabled;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _generalHdrStrpComment;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _generalHdrStrpPermission;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip _generalHdrStrpStatus;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage _serversPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.DualListSelectorControl _serversListSelectorControl;
        private Divelements.WizardFramework.InformationBox _serversPageInfo;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.Misc.UltraDropDownButton tagsDropDownButton;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Top;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _PermissionPropertyDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _PermissionPropertyDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _PermissionPropertyDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _PermissionPropertyDialog_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Right;
        private System.ComponentModel.BackgroundWorker initializationWorker;
        private System.Windows.Forms.TableLayoutPanel mainContainerPanel;
        private Controls.PropertiesHeaderStrip _generalHdrStrpWebAppPermission;
        private CheckBox _generalChkBxWebAppPermission;

    }
}
