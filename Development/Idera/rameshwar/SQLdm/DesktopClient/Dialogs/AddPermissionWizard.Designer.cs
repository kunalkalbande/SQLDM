namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddPermissionWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPermissionWizard));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagsPopupMenu");
            this._wizard = new Divelements.WizardFramework.Wizard();
            this._introPage = new Divelements.WizardFramework.IntroductionPage();
            this._introPageChkBx = new System.Windows.Forms.CheckBox();
            this._introPageText = new System.Windows.Forms.RichTextBox();
            this._userPage = new Divelements.WizardFramework.WizardPage();
            this._userPageErrMsg = new Divelements.WizardFramework.InformationBox();
            this._userPageCmbBxAuthentication = new System.Windows.Forms.ComboBox();
            this._userPageLblAuthentication = new System.Windows.Forms.Label();
            this._userPageTxtBxUser = new System.Windows.Forms.TextBox();
            this._userPageLblUser = new System.Windows.Forms.Label();
            this._userPageInfo = new Divelements.WizardFramework.InformationBox();
            this._passwordPage = new Divelements.WizardFramework.WizardPage();
            this._passwordPageTxtBxConfirmPassword = new System.Windows.Forms.TextBox();
            this._passwordPageTxtBxPassword = new System.Windows.Forms.TextBox();
            this._passwordPageInfo = new Divelements.WizardFramework.InformationBox();
            this._passwordPageLblConfirmPassword = new System.Windows.Forms.Label();
            this._passwordPageLblPassword = new System.Windows.Forms.Label();
            this._permissionPage = new Divelements.WizardFramework.WizardPage();
            this._permissionPageWebInfo = new Divelements.WizardFramework.InformationBox();
            this._permissionPageChkBxWebAppAccess = new System.Windows.Forms.CheckBox();
            this._permissionPageRdBtnAdministrator = new System.Windows.Forms.RadioButton();
            this._permissionPageRdBtnModify = new System.Windows.Forms.RadioButton();
            this._permissionPageRdBtnView = new System.Windows.Forms.RadioButton();
            //Operator Security Role Changes - 10.3
            this._permissionPageRdBtnReadOnlyPlus = new System.Windows.Forms.RadioButton();
            this._permissionPageInfo = new Divelements.WizardFramework.InformationBox();
            this._serversPage = new Divelements.WizardFramework.WizardPage();
            this.label1 = new System.Windows.Forms.Label();
            this._serversPageInfo = new Divelements.WizardFramework.InformationBox();
            this.tagsDropDownButton = new Infragistics.Win.Misc.UltraDropDownButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._serversListSelectorControl = new Idera.SQLdm.DesktopClient.Controls.DualListSelectorControl();
            this._finishPage = new Divelements.WizardFramework.FinishPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._finishPagelLstBxServers = new System.Windows.Forms.ListBox();
            this._finishPageLblServers = new System.Windows.Forms.Label();
            this._finishPageLblUser = new System.Windows.Forms.Label();
            this._finishPageLblUserVal = new System.Windows.Forms.Label();
            this._finishPageLblAuhthentication = new System.Windows.Forms.Label();
            this._finishPageLblAuhthenticationVal = new System.Windows.Forms.Label();
            this._finishPageLblPermission = new System.Windows.Forms.Label();
            this._finishPageLblPermissionVal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._finishLabelTagsValue = new System.Windows.Forms.Label();
            this._AddPermissionWizard_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AddPermissionWizard_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AddPermissionWizard_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._wizard.SuspendLayout();
            this._introPage.SuspendLayout();
            this._userPage.SuspendLayout();
            this._passwordPage.SuspendLayout();
            this._permissionPage.SuspendLayout();
            this._serversPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this._finishPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _wizard
            // 
            this._wizard.AnimatePageTransitions = false;
            this._wizard.BannerImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddPermission48x48;
            this._wizard.Controls.Add(this._introPage);
            this._wizard.Controls.Add(this._passwordPage);
            this._wizard.Controls.Add(this._permissionPage);
            this._wizard.Controls.Add(this._serversPage);
            this._wizard.Controls.Add(this._finishPage);
            this._wizard.Controls.Add(this._userPage);
            this._wizard.HelpVisible = true;
            this._wizard.Location = new System.Drawing.Point(0, 0);
            this._wizard.MarginImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddUserPermissionsWizardMarginImage;
            this._wizard.Name = "_wizard";
            this._wizard.OwnerForm = this;
            this._wizard.Size = new System.Drawing.Size(565, 518);
            this._wizard.TabIndex = 0;
            this._wizard.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            // 
            // _introPage
            // 
            this._introPage.BackColor = System.Drawing.SystemColors.Window;
            this._introPage.Controls.Add(this._introPageChkBx);
            this._introPage.Controls.Add(this._introPageText);
            this._introPage.IntroductionText = "";
            this._introPage.Location = new System.Drawing.Point(175, 71);
            this._introPage.Name = "_introPage";
            this._introPage.NextPage = this._userPage;
            this._introPage.ProceedText = "";
            this._introPage.Size = new System.Drawing.Size(368, 389);
            this._introPage.TabIndex = 1004;
            this._introPage.Text = "Welcome to the Add Permission Wizard";
            // 
            // _introPageChkBx
            // 
            this._introPageChkBx.AutoSize = true;
            this._introPageChkBx.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.HideAddPermissionsWizardWelcomePage;
            this._introPageChkBx.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "HideAddPermissionsWizardWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._introPageChkBx.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._introPageChkBx.Location = new System.Drawing.Point(0, 372);
            this._introPageChkBx.Name = "_introPageChkBx";
            this._introPageChkBx.Size = new System.Drawing.Size(368, 17);
            this._introPageChkBx.TabIndex = 3;
            this._introPageChkBx.Text = "Don\'t show this welcome page again";
            this._introPageChkBx.UseVisualStyleBackColor = true;
            // 
            // _introPageText
            // 
            this._introPageText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._introPageText.Dock = System.Windows.Forms.DockStyle.Top;
            this._introPageText.Location = new System.Drawing.Point(0, 0);
            this._introPageText.Name = "_introPageText";
            this._introPageText.Size = new System.Drawing.Size(368, 154);
            this._introPageText.TabIndex = 0;
            this._introPageText.Text = resources.GetString("_introPageText.Text");
            // 
            // _userPage
            // 
            this._userPage.Controls.Add(this._userPageErrMsg);
            this._userPage.Controls.Add(this._userPageCmbBxAuthentication);
            this._userPage.Controls.Add(this._userPageLblAuthentication);
            this._userPage.Controls.Add(this._userPageTxtBxUser);
            this._userPage.Controls.Add(this._userPageLblUser);
            this._userPage.Controls.Add(this._userPageInfo);
            this._userPage.Description = "Specify the login to assign to SQLdm.";
            this._userPage.Location = new System.Drawing.Point(11, 71);
            this._userPage.Name = "_userPage";
            this._userPage.NextPage = this._passwordPage;
            this._userPage.PreviousPage = this._introPage;
            this._userPage.Size = new System.Drawing.Size(543, 389);
            this._userPage.TabIndex = 0;
            this._userPage.Text = "Add a login";
            this._userPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this._userPage_BeforeMoveNext);
            this._userPage.BeforeDisplay += new System.EventHandler(this._userPage_BeforeDisplay);
            // 
            // _userPageErrMsg
            // 
            this._userPageErrMsg.Icon = Divelements.WizardFramework.SystemIconType.Error;
            this._userPageErrMsg.Location = new System.Drawing.Point(26, 145);
            this._userPageErrMsg.Name = "_userPageErrMsg";
            this._userPageErrMsg.Size = new System.Drawing.Size(501, 131);
            this._userPageErrMsg.TabIndex = 5;
            this._userPageErrMsg.Text = "Invalid account error message";
            // 
            // _userPageCmbBxAuthentication
            // 
            this._userPageCmbBxAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._userPageCmbBxAuthentication.FormattingEnabled = true;
            this._userPageCmbBxAuthentication.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this._userPageCmbBxAuthentication.Location = new System.Drawing.Point(129, 96);
            this._userPageCmbBxAuthentication.Name = "_userPageCmbBxAuthentication";
            this._userPageCmbBxAuthentication.Size = new System.Drawing.Size(398, 21);
            this._userPageCmbBxAuthentication.TabIndex = 4;
            this._userPageCmbBxAuthentication.SelectedIndexChanged += new System.EventHandler(this._userPageCmbBxAuthentication_SelectedIndexChanged);
            // 
            // _userPageLblAuthentication
            // 
            this._userPageLblAuthentication.AutoSize = true;
            this._userPageLblAuthentication.Location = new System.Drawing.Point(23, 99);
            this._userPageLblAuthentication.Name = "_userPageLblAuthentication";
            this._userPageLblAuthentication.Size = new System.Drawing.Size(78, 13);
            this._userPageLblAuthentication.TabIndex = 3;
            this._userPageLblAuthentication.Text = "&Authentication:";
            // 
            // _userPageTxtBxUser
            // 
            this._userPageTxtBxUser.Location = new System.Drawing.Point(129, 68);
            this._userPageTxtBxUser.Name = "_userPageTxtBxUser";
            this._userPageTxtBxUser.Size = new System.Drawing.Size(398, 20);
            this._userPageTxtBxUser.TabIndex = 1;
            this._userPageTxtBxUser.TextChanged += new System.EventHandler(this._userPageTxtBxUser_TextChanged);
            // 
            // _userPageLblUser
            // 
            this._userPageLblUser.AutoSize = true;
            this._userPageLblUser.Location = new System.Drawing.Point(23, 71);
            this._userPageLblUser.Name = "_userPageLblUser";
            this._userPageLblUser.Size = new System.Drawing.Size(102, 13);
            this._userPageLblUser.TabIndex = 0;
            this._userPageLblUser.Text = "Domain\\&User name:";
            // 
            // _userPageInfo
            // 
            this._userPageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this._userPageInfo.Location = new System.Drawing.Point(0, 0);
            this._userPageInfo.Name = "_userPageInfo";
            this._userPageInfo.Size = new System.Drawing.Size(543, 56);
            this._userPageInfo.TabIndex = 0;
            this._userPageInfo.Text = resources.GetString("_userPageInfo.Text");
            // 
            // _passwordPage
            // 
            this._passwordPage.Controls.Add(this._passwordPageTxtBxConfirmPassword);
            this._passwordPage.Controls.Add(this._passwordPageTxtBxPassword);
            this._passwordPage.Controls.Add(this._passwordPageInfo);
            this._passwordPage.Controls.Add(this._passwordPageLblConfirmPassword);
            this._passwordPage.Controls.Add(this._passwordPageLblPassword);
            this._passwordPage.Description = "Specify a password for creating a SQL Server login.";
            this._passwordPage.Location = new System.Drawing.Point(11, 71);
            this._passwordPage.Name = "_passwordPage";
            this._passwordPage.NextPage = this._permissionPage;
            this._passwordPage.PreviousPage = this._userPage;
            this._passwordPage.Size = new System.Drawing.Size(543, 389);
            this._passwordPage.TabIndex = 0;
            this._passwordPage.Text = "Specify Password";
            this._passwordPage.BeforeDisplay += new System.EventHandler(this._passwordPage_BeforeDisplay);
            this._passwordPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this._passwordPage_BeforeMoveNext);
            // 
            // _passwordPageTxtBxConfirmPassword
            // 
            this._passwordPageTxtBxConfirmPassword.Location = new System.Drawing.Point(122, 76);
            this._passwordPageTxtBxConfirmPassword.Name = "_passwordPageTxtBxConfirmPassword";
            this._passwordPageTxtBxConfirmPassword.PasswordChar = '*';
            this._passwordPageTxtBxConfirmPassword.Size = new System.Drawing.Size(405, 20);
            this._passwordPageTxtBxConfirmPassword.TabIndex = 4;
            this._passwordPageTxtBxConfirmPassword.UseSystemPasswordChar = true;
            this._passwordPageTxtBxConfirmPassword.TextChanged += new System.EventHandler(this._passwordPageTxtBxConfirmPassword_TextChanged);
            // 
            // _passwordPageTxtBxPassword
            // 
            this._passwordPageTxtBxPassword.Location = new System.Drawing.Point(122, 50);
            this._passwordPageTxtBxPassword.Name = "_passwordPageTxtBxPassword";
            this._passwordPageTxtBxPassword.PasswordChar = '*';
            this._passwordPageTxtBxPassword.Size = new System.Drawing.Size(405, 20);
            this._passwordPageTxtBxPassword.TabIndex = 2;
            this._passwordPageTxtBxPassword.UseSystemPasswordChar = true;
            this._passwordPageTxtBxPassword.TextChanged += new System.EventHandler(this._passwordPageTxtBxPassword_TextChanged);
            // 
            // _passwordPageInfo
            // 
            this._passwordPageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this._passwordPageInfo.Location = new System.Drawing.Point(0, 0);
            this._passwordPageInfo.Name = "_passwordPageInfo";
            this._passwordPageInfo.Size = new System.Drawing.Size(543, 52);
            this._passwordPageInfo.TabIndex = 0;
            this._passwordPageInfo.Text = "The specified SQL Server login does not exist on the SQL Server hosting the SQLdm" +
    " Repoistory. To create a new SQL Server login, please specify a password.";
            // 
            // _passwordPageLblConfirmPassword
            // 
            this._passwordPageLblConfirmPassword.AutoSize = true;
            this._passwordPageLblConfirmPassword.Location = new System.Drawing.Point(23, 79);
            this._passwordPageLblConfirmPassword.Name = "_passwordPageLblConfirmPassword";
            this._passwordPageLblConfirmPassword.Size = new System.Drawing.Size(93, 13);
            this._passwordPageLblConfirmPassword.TabIndex = 3;
            this._passwordPageLblConfirmPassword.Text = "&Confirm password:";
            // 
            // _passwordPageLblPassword
            // 
            this._passwordPageLblPassword.AutoSize = true;
            this._passwordPageLblPassword.Location = new System.Drawing.Point(23, 53);
            this._passwordPageLblPassword.Name = "_passwordPageLblPassword";
            this._passwordPageLblPassword.Size = new System.Drawing.Size(56, 13);
            this._passwordPageLblPassword.TabIndex = 1;
            this._passwordPageLblPassword.Text = "&Password:";
            // 
            // _permissionPage
            // 
            this._permissionPage.Controls.Add(this._permissionPageWebInfo);
            this._permissionPage.Controls.Add(this._permissionPageChkBxWebAppAccess);
            this._permissionPage.Controls.Add(this._permissionPageRdBtnAdministrator);
            this._permissionPage.Controls.Add(this._permissionPageRdBtnModify);
            //Operator Security Role Changes - 10.3
            this._permissionPage.Controls.Add(this._permissionPageRdBtnReadOnlyPlus);
            this._permissionPage.Controls.Add(this._permissionPageRdBtnView);
            this._permissionPage.Controls.Add(this._permissionPageInfo);
            this._permissionPage.Description = "Specify the SQLDM permissins you want to assign to the account.";
            this._permissionPage.Location = new System.Drawing.Point(11, 71);
            this._permissionPage.Name = "_permissionPage";
            this._permissionPage.NextPage = this._serversPage;
            this._permissionPage.PreviousPage = this._passwordPage;
            this._permissionPage.Size = new System.Drawing.Size(543, 389);
            this._permissionPage.TabIndex = 1007;
            this._permissionPage.Text = "Specify Permission";
            this._permissionPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this._permissionPage_BeforeMoveNext);
            // 
            // _permissionPageWebInfo
            // 
            this._permissionPageWebInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._permissionPageWebInfo.Location = new System.Drawing.Point(1, 199);
            this._permissionPageWebInfo.Name = "_permissionPageWebInfo";
            this._permissionPageWebInfo.Size = new System.Drawing.Size(514, 38);
            this._permissionPageWebInfo.TabIndex = 6;
            this._permissionPageWebInfo.Text = "Access to the Web Application to the specified account. Please select the option " +
    "below.";
            // 
            // _permissionPageChkBxWebAppAccess
            // 
            this._permissionPageChkBxWebAppAccess.AutoSize = true;
            this._permissionPageChkBxWebAppAccess.Checked = false;
            this._permissionPageChkBxWebAppAccess.CheckState = System.Windows.Forms.CheckState.Checked;
            this._permissionPageChkBxWebAppAccess.Location = new System.Drawing.Point(23, 243);
            this._permissionPageChkBxWebAppAccess.Name = "_permissionPageChkBxWebAppAccess";
            this._permissionPageChkBxWebAppAccess.Size = new System.Drawing.Size(171, 17);
            this._permissionPageChkBxWebAppAccess.TabIndex = 5;
            this._permissionPageChkBxWebAppAccess.Text = "Grant Web Application Access";
            this._permissionPageChkBxWebAppAccess.UseVisualStyleBackColor = true;
            //Operator Security Role Changes - 10.3
            //
            //_permissionPageRdBtnReadOnlyPlus
            //
            this._permissionPageRdBtnReadOnlyPlus.AutoSize = true;
            this._permissionPageRdBtnReadOnlyPlus.Location = new System.Drawing.Point(23, 99);
            this._permissionPageRdBtnReadOnlyPlus.Name = "_permissionPageRdBtnReadOnlyPlus";
            this._permissionPageRdBtnReadOnlyPlus.Size = new System.Drawing.Size(252, 17);//252
            this._permissionPageRdBtnReadOnlyPlus.TabIndex = 4;
            this._permissionPageRdBtnReadOnlyPlus.Text = "&View data, acknowledge alarms, and control maintenance mode status";
            this._permissionPageRdBtnReadOnlyPlus.UseVisualStyleBackColor = true;
            // 
            // _permissionPageRdBtnAdministrator
            // 
            this._permissionPageRdBtnAdministrator.AutoSize = true;
            this._permissionPageRdBtnAdministrator.Location = new System.Drawing.Point(23, 122);
            this._permissionPageRdBtnAdministrator.Name = "_permissionPageRdBtnAdministrator";
            this._permissionPageRdBtnAdministrator.Size = new System.Drawing.Size(526, 17);
            this._permissionPageRdBtnAdministrator.TabIndex = 3;
            this._permissionPageRdBtnAdministrator.Text = "&Administrator powers in SQL diagnostic manager";
            this._permissionPageRdBtnAdministrator.UseVisualStyleBackColor = true;
            // 
            // _permissionPageRdBtnModify
            // 
            this._permissionPageRdBtnModify.AutoSize = true;
            this._permissionPageRdBtnModify.Location = new System.Drawing.Point(23, 76);
            this._permissionPageRdBtnModify.Name = "_permissionPageRdBtnModify";
            this._permissionPageRdBtnModify.Size = new System.Drawing.Size(406, 17);
            this._permissionPageRdBtnModify.TabIndex = 2;
            this._permissionPageRdBtnModify.Text = "&Modify configuration and view data collected for monitored SQL Server instances";
            this._permissionPageRdBtnModify.UseVisualStyleBackColor = true;
            // 
            // _permissionPageRdBtnView
            // 
            this._permissionPageRdBtnView.AutoSize = true;
            this._permissionPageRdBtnView.Checked = true;
            this._permissionPageRdBtnView.Location = new System.Drawing.Point(23, 53);
            this._permissionPageRdBtnView.Name = "_permissionPageRdBtnView";
            this._permissionPageRdBtnView.Size = new System.Drawing.Size(288, 17);
            this._permissionPageRdBtnView.TabIndex = 1;
            this._permissionPageRdBtnView.TabStop = true;
            this._permissionPageRdBtnView.Text = "&View data collected for monitored SQL Server instances";
            this._permissionPageRdBtnView.UseVisualStyleBackColor = true;
            // 
            // _permissionPageInfo
            // 
            this._permissionPageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this._permissionPageInfo.Location = new System.Drawing.Point(0, 0);
            this._permissionPageInfo.Name = "_permissionPageInfo";
            this._permissionPageInfo.Size = new System.Drawing.Size(543, 61);
            this._permissionPageInfo.TabIndex = 0;
            this._permissionPageInfo.Text = "Assign View, Modify, or Administrator permission to the specified account. The Vi" +
    "ew or Modify permissions apply to monitored SQL Server instances selected in the" +
    " following screen.";
            // 
            // _serversPage
            // 
            this._serversPage.Controls.Add(this.label1);
            this._serversPage.Controls.Add(this._serversPageInfo);
            this._serversPage.Controls.Add(this.tagsDropDownButton);
            this._serversPage.Controls.Add(this._serversListSelectorControl);
            this._serversPage.Description = "Select the Tags or SQL Servers to assign to the login.";
            this._serversPage.Location = new System.Drawing.Point(11, 71);
            this._serversPage.Name = "_serversPage";
            this._serversPage.NextPage = this._finishPage;
            this._serversPage.PreviousPage = this._permissionPage;
            this._serversPage.Size = new System.Drawing.Size(543, 389);
            this._serversPage.TabIndex = 0;
            this._serversPage.Text = "Select SQL Servers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Tags:";
            // 
            // _serversPageInfo
            // 
            this._serversPageInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this._serversPageInfo.Location = new System.Drawing.Point(0, 0);
            this._serversPageInfo.Name = "_serversPageInfo";
            this._serversPageInfo.Size = new System.Drawing.Size(543, 67);
            this._serversPageInfo.TabIndex = 0;
            this._serversPageInfo.Text = resources.GetString("_serversPageInfo.Text");
            // 
            // tagsDropDownButton
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BorderColor = System.Drawing.SystemColors.ControlDark;
            appearance1.TextHAlignAsString = "Left";
            this.tagsDropDownButton.Appearance = appearance1;
            this.tagsDropDownButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.tagsDropDownButton.Location = new System.Drawing.Point(26, 83);
            this.tagsDropDownButton.Name = "tagsDropDownButton";
            this.tagsDropDownButton.PopupItemKey = "tagsPopupMenu";
            this.tagsDropDownButton.PopupItemProvider = this.toolbarsManager;
            this.tagsDropDownButton.ShowFocusRect = false;
            this.tagsDropDownButton.ShowOutline = false;
            this.tagsDropDownButton.Size = new System.Drawing.Size(491, 22);
            this.tagsDropDownButton.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.tagsDropDownButton.TabIndex = 22;
            this.tagsDropDownButton.UseAppStyling = false;
            this.tagsDropDownButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.tagsDropDownButton.WrapText = false;
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
            // _serversListSelectorControl
            // 
            this._serversListSelectorControl.AvailableLabel = "Available Servers:";
            this._serversListSelectorControl.Location = new System.Drawing.Point(26, 108);
            this._serversListSelectorControl.Name = "_serversListSelectorControl";
            this._serversListSelectorControl.SelectedLabel = "Selected Servers:";
            this._serversListSelectorControl.Size = new System.Drawing.Size(491, 281);
            this._serversListSelectorControl.TabIndex = 21;
            this._serversListSelectorControl.SelectionChanged += new System.EventHandler(this._serversListSelectorControl_SelectionChanged);
            // 
            // _finishPage
            // 
            this._finishPage.BackColor = System.Drawing.SystemColors.Window;
            this._finishPage.Controls.Add(this.tableLayoutPanel1);
            this._finishPage.FinishText = "Verify that the following information is correct. To make changes, click the Back" +
    " button.";
            this._finishPage.Location = new System.Drawing.Point(175, 71);
            this._finishPage.Name = "_finishPage";
            this._finishPage.PreviousPage = this._serversPage;
            this._finishPage.ProceedText = "To add the permission, click Finish.";
            this._finishPage.Size = new System.Drawing.Size(368, 389);
            this._finishPage.TabIndex = 1006;
            this._finishPage.Text = "Completing the Add Permission Wizard";
            this._finishPage.BeforeDisplay += new System.EventHandler(this._finishPage_BeforeDisplay);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._finishPagelLstBxServers, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblServers, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblUser, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblUserVal, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblAuhthentication, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblAuhthenticationVal, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblPermission, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._finishPageLblPermissionVal, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._finishLabelTagsValue, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 33);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(359, 336);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // _finishPagelLstBxServers
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._finishPagelLstBxServers, 2);
            this._finishPagelLstBxServers.Enabled = false;
            this._finishPagelLstBxServers.FormattingEnabled = true;
            this._finishPagelLstBxServers.Location = new System.Drawing.Point(3, 68);
            this._finishPagelLstBxServers.Name = "_finishPagelLstBxServers";
            this._finishPagelLstBxServers.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this._finishPagelLstBxServers.Size = new System.Drawing.Size(347, 251);
            this._finishPagelLstBxServers.TabIndex = 13;
            this._finishPagelLstBxServers.TabStop = false;
            // 
            // _finishPageLblServers
            // 
            this._finishPageLblServers.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._finishPageLblServers, 2);
            this._finishPageLblServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._finishPageLblServers.Location = new System.Drawing.Point(3, 52);
            this._finishPageLblServers.Name = "_finishPageLblServers";
            this._finishPageLblServers.Size = new System.Drawing.Size(70, 13);
            this._finishPageLblServers.TabIndex = 12;
            this._finishPageLblServers.Text = "SQL Servers:";
            // 
            // _finishPageLblUser
            // 
            this._finishPageLblUser.AutoSize = true;
            this._finishPageLblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._finishPageLblUser.Location = new System.Drawing.Point(3, 0);
            this._finishPageLblUser.Name = "_finishPageLblUser";
            this._finishPageLblUser.Size = new System.Drawing.Size(32, 13);
            this._finishPageLblUser.TabIndex = 1;
            this._finishPageLblUser.Text = "User:";
            // 
            // _finishPageLblUserVal
            // 
            this._finishPageLblUserVal.AutoEllipsis = true;
            this._finishPageLblUserVal.Location = new System.Drawing.Point(87, 0);
            this._finishPageLblUserVal.Name = "_finishPageLblUserVal";
            this._finishPageLblUserVal.Size = new System.Drawing.Size(281, 13);
            this._finishPageLblUserVal.TabIndex = 2;
            this._finishPageLblUserVal.Text = "User:";
            // 
            // _finishPageLblAuhthentication
            // 
            this._finishPageLblAuhthentication.AutoSize = true;
            this._finishPageLblAuhthentication.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._finishPageLblAuhthentication.Location = new System.Drawing.Point(3, 13);
            this._finishPageLblAuhthentication.Name = "_finishPageLblAuhthentication";
            this._finishPageLblAuhthentication.Size = new System.Drawing.Size(78, 13);
            this._finishPageLblAuhthentication.TabIndex = 3;
            this._finishPageLblAuhthentication.Text = "Authentication:";
            // 
            // _finishPageLblAuhthenticationVal
            // 
            this._finishPageLblAuhthenticationVal.AutoSize = true;
            this._finishPageLblAuhthenticationVal.Location = new System.Drawing.Point(87, 13);
            this._finishPageLblAuhthenticationVal.Name = "_finishPageLblAuhthenticationVal";
            this._finishPageLblAuhthenticationVal.Size = new System.Drawing.Size(78, 13);
            this._finishPageLblAuhthenticationVal.TabIndex = 7;
            this._finishPageLblAuhthenticationVal.Text = "Authentication:";
            // 
            // _finishPageLblPermission
            // 
            this._finishPageLblPermission.AutoSize = true;
            this._finishPageLblPermission.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._finishPageLblPermission.Location = new System.Drawing.Point(3, 26);
            this._finishPageLblPermission.Name = "_finishPageLblPermission";
            this._finishPageLblPermission.Size = new System.Drawing.Size(60, 13);
            this._finishPageLblPermission.TabIndex = 8;
            this._finishPageLblPermission.Text = "Permission:";
            // 
            // _finishPageLblPermissionVal
            // 
            this._finishPageLblPermissionVal.AutoSize = true;
            this._finishPageLblPermissionVal.Location = new System.Drawing.Point(87, 26);
            this._finishPageLblPermissionVal.Name = "_finishPageLblPermissionVal";
            this._finishPageLblPermissionVal.Size = new System.Drawing.Size(60, 13);
            this._finishPageLblPermissionVal.TabIndex = 9;
            this._finishPageLblPermissionVal.Text = "Permission:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tags:";
            // 
            // _finishLabelTagsValue
            // 
            this._finishLabelTagsValue.AutoSize = true;
            this._finishLabelTagsValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._finishLabelTagsValue.Location = new System.Drawing.Point(87, 39);
            this._finishLabelTagsValue.Name = "_finishLabelTagsValue";
            this._finishLabelTagsValue.Size = new System.Drawing.Size(34, 13);
            this._finishLabelTagsValue.TabIndex = 11;
            this._finishLabelTagsValue.Text = "Tags:";
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Left
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Left";
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 518);
            this._AddPermissionWizard_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Right
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(565, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Right";
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 518);
            this._AddPermissionWizard_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Top
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Top";
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(565, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _AddPermissionWizard_Toolbars_Dock_Area_Bottom
            // 
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 518);
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Name = "_AddPermissionWizard_Toolbars_Dock_Area_Bottom";
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(565, 0);
            this._AddPermissionWizard_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // AddPermissionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 518);
            this.Controls.Add(this._wizard);
            this.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._AddPermissionWizard_Toolbars_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddPermissionWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Permission Wizard";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AddPermissionWizard_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddPermissionWizard_HelpRequested);
            this._wizard.ResumeLayout(false);
            this._introPage.ResumeLayout(false);
            this._introPage.PerformLayout();
            this._userPage.ResumeLayout(false);
            this._userPage.PerformLayout();
            this._passwordPage.ResumeLayout(false);
            this._passwordPage.PerformLayout();
            this._permissionPage.ResumeLayout(false);
            this._permissionPage.PerformLayout();
            this._serversPage.ResumeLayout(false);
            this._serversPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this._finishPage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard _wizard;
        private Divelements.WizardFramework.IntroductionPage _introPage;
        private Divelements.WizardFramework.WizardPage _userPage;
        private Divelements.WizardFramework.FinishPage _finishPage;
        private System.Windows.Forms.RichTextBox _introPageText;
        private System.Windows.Forms.CheckBox _introPageChkBx;
        private Divelements.WizardFramework.InformationBox _userPageInfo;
        private System.Windows.Forms.TextBox _userPageTxtBxUser;
        private System.Windows.Forms.Label _userPageLblUser;
        private Divelements.WizardFramework.WizardPage _permissionPage;
        private System.Windows.Forms.ComboBox _userPageCmbBxAuthentication;
        private System.Windows.Forms.Label _userPageLblAuthentication;
        private Divelements.WizardFramework.WizardPage _passwordPage;
        private System.Windows.Forms.TextBox _passwordPageTxtBxConfirmPassword;
        private System.Windows.Forms.Label _passwordPageLblConfirmPassword;
        private System.Windows.Forms.TextBox _passwordPageTxtBxPassword;
        private System.Windows.Forms.Label _passwordPageLblPassword;
        private Divelements.WizardFramework.InformationBox _passwordPageInfo;
        private Divelements.WizardFramework.InformationBox _permissionPageInfo;
        private System.Windows.Forms.RadioButton _permissionPageRdBtnView;
        private System.Windows.Forms.RadioButton _permissionPageRdBtnAdministrator;
        private System.Windows.Forms.RadioButton _permissionPageRdBtnModify;
        //Operator Security Role Changes - 10.3
        private System.Windows.Forms.RadioButton _permissionPageRdBtnReadOnlyPlus;
        private Divelements.WizardFramework.WizardPage _serversPage;
        private Divelements.WizardFramework.InformationBox _serversPageInfo;
        private Idera.SQLdm.DesktopClient.Controls.DualListSelectorControl _serversListSelectorControl;
        private Divelements.WizardFramework.InformationBox _userPageErrMsg;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.Misc.UltraDropDownButton tagsDropDownButton;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AddPermissionWizard_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox _finishPagelLstBxServers;
        private System.Windows.Forms.Label _finishPageLblServers;
        private System.Windows.Forms.Label _finishPageLblUser;
        private System.Windows.Forms.Label _finishPageLblUserVal;
        private System.Windows.Forms.Label _finishPageLblAuhthentication;
        private System.Windows.Forms.Label _finishPageLblAuhthenticationVal;
        private System.Windows.Forms.Label _finishPageLblPermission;
        private System.Windows.Forms.Label _finishPageLblPermissionVal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label _finishLabelTagsValue;
        private Divelements.WizardFramework.InformationBox _permissionPageWebInfo;
        private System.Windows.Forms.CheckBox _permissionPageChkBxWebAppAccess;
    }
}
