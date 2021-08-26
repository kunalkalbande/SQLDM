namespace Idera.SQLdm.ImportWizard
{
    partial class WizardMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardMainForm));
            this._wizard = new Divelements.WizardFramework.Wizard();
            this._page_Introduction = new Divelements.WizardFramework.IntroductionPage();
            this._chkBx_HideImportWizardWelcomPage = new System.Windows.Forms.CheckBox();
            this._lbl_Intro1 = new System.Windows.Forms.Label();
            this._page_RepositoryConnection = new Divelements.WizardFramework.WizardPage();
            this._infbx_RepositoryConnection = new Divelements.WizardFramework.InformationBox();
            this._grpbx_RepositoryConnection = new System.Windows.Forms.GroupBox();
            this._btn_BrowseDatabase = new System.Windows.Forms.Button();
            this._txtbx_Database = new System.Windows.Forms.TextBox();
            this._btn_BrowseServer = new System.Windows.Forms.Button();
            this._txtbx_Server = new System.Windows.Forms.TextBox();
            this._txtbx_Password = new System.Windows.Forms.TextBox();
            this._lbl_Password = new System.Windows.Forms.Label();
            this._lbl_UserName = new System.Windows.Forms.Label();
            this._txtbx_User = new System.Windows.Forms.TextBox();
            this._cmbbx_Authentication = new System.Windows.Forms.ComboBox();
            this._lbl_Authentication = new System.Windows.Forms.Label();
            this._lbl_Database = new System.Windows.Forms.Label();
            this._lbl_Server = new System.Windows.Forms.Label();
            this._grpbx_TestConnection = new System.Windows.Forms.GroupBox();
            this._testConnectionStatus = new Idera.SQLdm.ImportWizard.Controls.TestConnectionStatus();
            this._btn_TestRepositoryConnection = new System.Windows.Forms.Button();
            this._page_SelectServers = new Divelements.WizardFramework.WizardPage();
            this._btn_RemoveServer = new System.Windows.Forms.Button();
            this._btn_AddServer = new System.Windows.Forms.Button();
            this._lstbx_SelectedServers = new System.Windows.Forms.ListBox();
            this._lbl_AvailableServers = new System.Windows.Forms.Label();
            this._lbl_SelectedServers = new System.Windows.Forms.Label();
            this._infbx_SelectServers = new Divelements.WizardFramework.InformationBox();
            this._loadServersStatus = new Idera.SQLdm.ImportWizard.Controls.LoadServersStatus();
            this._lstbx_AvailableServers = new System.Windows.Forms.ListBox();
            this._page_SpecifyImportDate = new Divelements.WizardFramework.WizardPage();
            this._lstvw_SelectedServers = new System.Windows.Forms.ListView();
            this._colSQLServer = new System.Windows.Forms.ColumnHeader();
            this._colCurrentAsOf = new System.Windows.Forms.ColumnHeader();
            this._dtEdtr_DatePicker = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this._lbl_LoadFrom = new System.Windows.Forms.Label();
            this._infbx_SelectImportDate = new Divelements.WizardFramework.InformationBox();
            this._page_Finish = new Divelements.WizardFramework.FinishPage();
            this._lbl_FinishImportDateLbl2 = new System.Windows.Forms.Label();
            this._lbl_FinishImportDate = new System.Windows.Forms.Label();
            this._lstbx_FinishServers = new System.Windows.Forms.ListBox();
            this._lbl_FinishImportDateLbl1 = new System.Windows.Forms.Label();
            this._lbl_FinishSQLServers = new System.Windows.Forms.Label();
            this._wizard.SuspendLayout();
            this._page_Introduction.SuspendLayout();
            this._page_RepositoryConnection.SuspendLayout();
            this._grpbx_RepositoryConnection.SuspendLayout();
            this._grpbx_TestConnection.SuspendLayout();
            this._page_SelectServers.SuspendLayout();
            this._page_SpecifyImportDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dtEdtr_DatePicker)).BeginInit();
            this._page_Finish.SuspendLayout();
            this.SuspendLayout();
            // 
            // _wizard
            // 
            this._wizard.AnimatePageTransitions = false;
            this._wizard.BannerImage = global::Idera.SQLdm.ImportWizard.Properties.Resources.data_into_49;
            this._wizard.Controls.Add(this._page_Introduction);
            this._wizard.Controls.Add(this._page_SelectServers);
            this._wizard.Controls.Add(this._page_RepositoryConnection);
            this._wizard.Controls.Add(this._page_Finish);
            this._wizard.Controls.Add(this._page_SpecifyImportDate);
            this._wizard.FinishText = "&Start Import";
            this._wizard.HelpVisible = true;
            this._wizard.Location = new System.Drawing.Point(0, 0);
            this._wizard.MarginImage = global::Idera.SQLdm.ImportWizard.Properties.Resources.wiz_margin_image;
            this._wizard.Name = "_wizard";
            this._wizard.OwnerForm = this;
            this._wizard.Size = new System.Drawing.Size(563, 410);
            this._wizard.TabIndex = 0;
            this._wizard.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this._wizard.Finish += new System.EventHandler(this._wizard_Finish);
            this._wizard.HelpRequested += new System.Windows.Forms.HelpEventHandler(this._wizard_HelpRequested);
            this._wizard.Cancel += new System.EventHandler(this._wizard_Cancel);
            // 
            // _page_Introduction
            // 
            this._page_Introduction.BackColor = System.Drawing.SystemColors.Window;
            this._page_Introduction.Controls.Add(this._chkBx_HideImportWizardWelcomPage);
            this._page_Introduction.Controls.Add(this._lbl_Intro1);
            this._page_Introduction.IntroductionText = "";
            this._page_Introduction.Location = new System.Drawing.Point(175, 71);
            this._page_Introduction.Name = "_page_Introduction";
            this._page_Introduction.NextPage = this._page_RepositoryConnection;
            this._page_Introduction.ProceedText = "";
            this._page_Introduction.Size = new System.Drawing.Size(366, 281);
            this._page_Introduction.TabIndex = 1004;
            this._page_Introduction.Text = "Welcome to the SQL diagnostic manager Import Wizard";
            // 
            // _chkBx_HideImportWizardWelcomPage
            // 
            this._chkBx_HideImportWizardWelcomPage.AutoSize = true;
            this._chkBx_HideImportWizardWelcomPage.Checked = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.HideImportWizardWelcomePage;
            this._chkBx_HideImportWizardWelcomPage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.ImportWizard.Properties.Settings.Default, "HideImportWizardWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._chkBx_HideImportWizardWelcomPage.Location = new System.Drawing.Point(6, 264);
            this._chkBx_HideImportWizardWelcomPage.Name = "_chkBx_HideImportWizardWelcomPage";
            this._chkBx_HideImportWizardWelcomPage.Size = new System.Drawing.Size(199, 17);
            this._chkBx_HideImportWizardWelcomPage.TabIndex = 2;
            this._chkBx_HideImportWizardWelcomPage.Text = "Don\'t show this welcome page again";
            this._chkBx_HideImportWizardWelcomPage.UseVisualStyleBackColor = true;
            // 
            // _lbl_Intro1
            // 
            this._lbl_Intro1.Location = new System.Drawing.Point(0, 0);
            this._lbl_Intro1.Name = "_lbl_Intro1";
            this._lbl_Intro1.Size = new System.Drawing.Size(373, 261);
            this._lbl_Intro1.TabIndex = 0;
            this._lbl_Intro1.Text = resources.GetString("_lbl_Intro1.Text");
            // 
            // _page_RepositoryConnection
            // 
            this._page_RepositoryConnection.Controls.Add(this._infbx_RepositoryConnection);
            this._page_RepositoryConnection.Controls.Add(this._grpbx_RepositoryConnection);
            this._page_RepositoryConnection.Controls.Add(this._grpbx_TestConnection);
            this._page_RepositoryConnection.Description = "Specify the location and credentials used to connect to the new SQL diagnostic ma" +
                "nager Repository.";
            this._page_RepositoryConnection.Location = new System.Drawing.Point(19, 73);
            this._page_RepositoryConnection.Name = "_page_RepositoryConnection";
            this._page_RepositoryConnection.NextPage = this._page_SelectServers;
            this._page_RepositoryConnection.PreviousPage = this._page_Introduction;
            this._page_RepositoryConnection.Size = new System.Drawing.Size(525, 277);
            this._page_RepositoryConnection.TabIndex = 0;
            this._page_RepositoryConnection.Text = "Specify SQL diagnostic manager Repository";
            this._page_RepositoryConnection.BeforeDisplay += new System.EventHandler(this._page_RepositoryConnection_BeforeDisplay);
            // 
            // _infbx_RepositoryConnection
            // 
            this._infbx_RepositoryConnection.Location = new System.Drawing.Point(24, 0);
            this._infbx_RepositoryConnection.Name = "_infbx_RepositoryConnection";
            this._infbx_RepositoryConnection.Size = new System.Drawing.Size(476, 44);
            this._infbx_RepositoryConnection.TabIndex = 0;
            this._infbx_RepositoryConnection.TabStop = false;
            this._infbx_RepositoryConnection.Text = "The information you provide below is used to connect to the new Repository. The I" +
                "mport Wizard writes historical data from SQL diagnostic manager 4.x to the new R" +
                "epository.";
            // 
            // _grpbx_RepositoryConnection
            // 
            this._grpbx_RepositoryConnection.Controls.Add(this._btn_BrowseDatabase);
            this._grpbx_RepositoryConnection.Controls.Add(this._txtbx_Database);
            this._grpbx_RepositoryConnection.Controls.Add(this._btn_BrowseServer);
            this._grpbx_RepositoryConnection.Controls.Add(this._txtbx_Server);
            this._grpbx_RepositoryConnection.Controls.Add(this._txtbx_Password);
            this._grpbx_RepositoryConnection.Controls.Add(this._lbl_Password);
            this._grpbx_RepositoryConnection.Controls.Add(this._lbl_UserName);
            this._grpbx_RepositoryConnection.Controls.Add(this._txtbx_User);
            this._grpbx_RepositoryConnection.Controls.Add(this._cmbbx_Authentication);
            this._grpbx_RepositoryConnection.Controls.Add(this._lbl_Authentication);
            this._grpbx_RepositoryConnection.Controls.Add(this._lbl_Database);
            this._grpbx_RepositoryConnection.Controls.Add(this._lbl_Server);
            this._grpbx_RepositoryConnection.Location = new System.Drawing.Point(37, 52);
            this._grpbx_RepositoryConnection.Name = "_grpbx_RepositoryConnection";
            this._grpbx_RepositoryConnection.Size = new System.Drawing.Size(450, 159);
            this._grpbx_RepositoryConnection.TabIndex = 0;
            this._grpbx_RepositoryConnection.TabStop = false;
            // 
            // _btn_BrowseDatabase
            // 
            this._btn_BrowseDatabase.Location = new System.Drawing.Point(412, 46);
            this._btn_BrowseDatabase.Name = "_btn_BrowseDatabase";
            this._btn_BrowseDatabase.Size = new System.Drawing.Size(32, 20);
            this._btn_BrowseDatabase.TabIndex = 5;
            this._btn_BrowseDatabase.Text = "...";
            this._btn_BrowseDatabase.UseVisualStyleBackColor = true;
            this._btn_BrowseDatabase.Click += new System.EventHandler(this._btn_BrowseDatabase_Click);
            // 
            // _txtbx_Database
            // 
            this._txtbx_Database.Location = new System.Drawing.Point(91, 46);
            this._txtbx_Database.Name = "_txtbx_Database";
            this._txtbx_Database.Size = new System.Drawing.Size(315, 20);
            this._txtbx_Database.TabIndex = 4;
            this._txtbx_Database.TextChanged += new System.EventHandler(this._txtbx_Database_TextChanged);
            // 
            // _btn_BrowseServer
            // 
            this._btn_BrowseServer.Location = new System.Drawing.Point(412, 18);
            this._btn_BrowseServer.Name = "_btn_BrowseServer";
            this._btn_BrowseServer.Size = new System.Drawing.Size(32, 20);
            this._btn_BrowseServer.TabIndex = 2;
            this._btn_BrowseServer.Text = "...";
            this._btn_BrowseServer.UseVisualStyleBackColor = true;
            this._btn_BrowseServer.Click += new System.EventHandler(this._btn_BrowseServer_Click);
            // 
            // _txtbx_Server
            // 
            this._txtbx_Server.Location = new System.Drawing.Point(91, 18);
            this._txtbx_Server.Name = "_txtbx_Server";
            this._txtbx_Server.Size = new System.Drawing.Size(315, 20);
            this._txtbx_Server.TabIndex = 1;
            this._txtbx_Server.TextChanged += new System.EventHandler(this._txtbx_Server_TextChanged);
            // 
            // _txtbx_Password
            // 
            this._txtbx_Password.Location = new System.Drawing.Point(121, 126);
            this._txtbx_Password.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this._txtbx_Password.Name = "_txtbx_Password";
            this._txtbx_Password.Size = new System.Drawing.Size(323, 20);
            this._txtbx_Password.TabIndex = 11;
            this._txtbx_Password.UseSystemPasswordChar = true;
            this._txtbx_Password.TextChanged += new System.EventHandler(this._txtbx_Password_TextChanged);
            // 
            // _lbl_Password
            // 
            this._lbl_Password.AutoSize = true;
            this._lbl_Password.Location = new System.Drawing.Point(55, 129);
            this._lbl_Password.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this._lbl_Password.Name = "_lbl_Password";
            this._lbl_Password.Size = new System.Drawing.Size(56, 13);
            this._lbl_Password.TabIndex = 10;
            this._lbl_Password.Text = "&Password:";
            // 
            // _lbl_UserName
            // 
            this._lbl_UserName.AutoSize = true;
            this._lbl_UserName.Location = new System.Drawing.Point(55, 103);
            this._lbl_UserName.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this._lbl_UserName.Name = "_lbl_UserName";
            this._lbl_UserName.Size = new System.Drawing.Size(61, 13);
            this._lbl_UserName.TabIndex = 8;
            this._lbl_UserName.Text = "&User name:";
            // 
            // _txtbx_User
            // 
            this._txtbx_User.Location = new System.Drawing.Point(121, 100);
            this._txtbx_User.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this._txtbx_User.Name = "_txtbx_User";
            this._txtbx_User.Size = new System.Drawing.Size(323, 20);
            this._txtbx_User.TabIndex = 9;
            this._txtbx_User.TextChanged += new System.EventHandler(this._txtbx_User_TextChanged);
            // 
            // _cmbbx_Authentication
            // 
            this._cmbbx_Authentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbbx_Authentication.FormattingEnabled = true;
            this._cmbbx_Authentication.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this._cmbbx_Authentication.Location = new System.Drawing.Point(91, 73);
            this._cmbbx_Authentication.Name = "_cmbbx_Authentication";
            this._cmbbx_Authentication.Size = new System.Drawing.Size(353, 21);
            this._cmbbx_Authentication.TabIndex = 7;
            this._cmbbx_Authentication.SelectedIndexChanged += new System.EventHandler(this._cmbbx_Authentication_SelectedIndexChanged);
            // 
            // _lbl_Authentication
            // 
            this._lbl_Authentication.AutoSize = true;
            this._lbl_Authentication.Location = new System.Drawing.Point(6, 76);
            this._lbl_Authentication.Name = "_lbl_Authentication";
            this._lbl_Authentication.Size = new System.Drawing.Size(78, 13);
            this._lbl_Authentication.TabIndex = 6;
            this._lbl_Authentication.Text = "&Authentication:";
            // 
            // _lbl_Database
            // 
            this._lbl_Database.AutoSize = true;
            this._lbl_Database.Location = new System.Drawing.Point(6, 49);
            this._lbl_Database.Name = "_lbl_Database";
            this._lbl_Database.Size = new System.Drawing.Size(56, 13);
            this._lbl_Database.TabIndex = 3;
            this._lbl_Database.Text = "&Database:";
            // 
            // _lbl_Server
            // 
            this._lbl_Server.AutoSize = true;
            this._lbl_Server.Location = new System.Drawing.Point(6, 21);
            this._lbl_Server.Name = "_lbl_Server";
            this._lbl_Server.Size = new System.Drawing.Size(41, 13);
            this._lbl_Server.TabIndex = 0;
            this._lbl_Server.Text = "&Server:";
            // 
            // _grpbx_TestConnection
            // 
            this._grpbx_TestConnection.Controls.Add(this._testConnectionStatus);
            this._grpbx_TestConnection.Controls.Add(this._btn_TestRepositoryConnection);
            this._grpbx_TestConnection.Location = new System.Drawing.Point(37, 217);
            this._grpbx_TestConnection.Name = "_grpbx_TestConnection";
            this._grpbx_TestConnection.Size = new System.Drawing.Size(450, 57);
            this._grpbx_TestConnection.TabIndex = 1;
            this._grpbx_TestConnection.TabStop = false;
            // 
            // _testConnectionStatus
            // 
            this._testConnectionStatus.HideStatus = false;
            this._testConnectionStatus.Location = new System.Drawing.Point(106, 19);
            this._testConnectionStatus.Name = "_testConnectionStatus";
            this._testConnectionStatus.Size = new System.Drawing.Size(338, 23);
            this._testConnectionStatus.TabIndex = 2;
            // 
            // _btn_TestRepositoryConnection
            // 
            this._btn_TestRepositoryConnection.Location = new System.Drawing.Point(9, 19);
            this._btn_TestRepositoryConnection.Name = "_btn_TestRepositoryConnection";
            this._btn_TestRepositoryConnection.Size = new System.Drawing.Size(91, 23);
            this._btn_TestRepositoryConnection.TabIndex = 0;
            this._btn_TestRepositoryConnection.Text = "&Test";
            this._btn_TestRepositoryConnection.UseVisualStyleBackColor = true;
            this._btn_TestRepositoryConnection.Click += new System.EventHandler(this._btn_TestRepositoryConnection_Click);
            // 
            // _page_SelectServers
            // 
            this._page_SelectServers.Controls.Add(this._btn_RemoveServer);
            this._page_SelectServers.Controls.Add(this._btn_AddServer);
            this._page_SelectServers.Controls.Add(this._lstbx_SelectedServers);
            this._page_SelectServers.Controls.Add(this._lbl_AvailableServers);
            this._page_SelectServers.Controls.Add(this._lbl_SelectedServers);
            this._page_SelectServers.Controls.Add(this._infbx_SelectServers);
            this._page_SelectServers.Controls.Add(this._loadServersStatus);
            this._page_SelectServers.Controls.Add(this._lstbx_AvailableServers);
            this._page_SelectServers.Description = "Select the monitored SQL Server instances whose historical data you want to impor" +
                "t.";
            this._page_SelectServers.Location = new System.Drawing.Point(19, 73);
            this._page_SelectServers.Name = "_page_SelectServers";
            this._page_SelectServers.NextPage = this._page_SpecifyImportDate;
            this._page_SelectServers.PreviousPage = this._page_RepositoryConnection;
            this._page_SelectServers.Size = new System.Drawing.Size(525, 277);
            this._page_SelectServers.TabIndex = 1008;
            this._page_SelectServers.Text = "Select SQL Servers";
            this._page_SelectServers.BeforeDisplay += new System.EventHandler(this._page_SelectServers_BeforeDisplay);
            // 
            // _btn_RemoveServer
            // 
            this._btn_RemoveServer.Location = new System.Drawing.Point(225, 186);
            this._btn_RemoveServer.Name = "_btn_RemoveServer";
            this._btn_RemoveServer.Size = new System.Drawing.Size(75, 23);
            this._btn_RemoveServer.TabIndex = 5;
            this._btn_RemoveServer.Text = "< &Remove";
            this._btn_RemoveServer.UseVisualStyleBackColor = true;
            this._btn_RemoveServer.Click += new System.EventHandler(this._btn_RemoveServer_Click);
            // 
            // _btn_AddServer
            // 
            this._btn_AddServer.Location = new System.Drawing.Point(225, 132);
            this._btn_AddServer.Name = "_btn_AddServer";
            this._btn_AddServer.Size = new System.Drawing.Size(75, 23);
            this._btn_AddServer.TabIndex = 2;
            this._btn_AddServer.Text = "&Add >";
            this._btn_AddServer.UseVisualStyleBackColor = true;
            this._btn_AddServer.Click += new System.EventHandler(this._btn_AddServer_Click);
            // 
            // _lstbx_SelectedServers
            // 
            this._lstbx_SelectedServers.FormattingEnabled = true;
            this._lstbx_SelectedServers.Location = new System.Drawing.Point(313, 62);
            this._lstbx_SelectedServers.Name = "_lstbx_SelectedServers";
            this._lstbx_SelectedServers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._lstbx_SelectedServers.Size = new System.Drawing.Size(187, 212);
            this._lstbx_SelectedServers.Sorted = true;
            this._lstbx_SelectedServers.TabIndex = 4;
            this._lstbx_SelectedServers.SelectedIndexChanged += new System.EventHandler(this._lstbx_SelectedServers_SelectedIndexChanged);
            this._lstbx_SelectedServers.DoubleClick += new System.EventHandler(this._lstbx_SelectedServers_DoubleClick);
            // 
            // _lbl_AvailableServers
            // 
            this._lbl_AvailableServers.AutoSize = true;
            this._lbl_AvailableServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lbl_AvailableServers.Location = new System.Drawing.Point(21, 47);
            this._lbl_AvailableServers.Name = "_lbl_AvailableServers";
            this._lbl_AvailableServers.Size = new System.Drawing.Size(92, 13);
            this._lbl_AvailableServers.TabIndex = 0;
            this._lbl_AvailableServers.Text = "A&vailable Servers:";
            // 
            // _lbl_SelectedServers
            // 
            this._lbl_SelectedServers.AutoSize = true;
            this._lbl_SelectedServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lbl_SelectedServers.Location = new System.Drawing.Point(310, 47);
            this._lbl_SelectedServers.Name = "_lbl_SelectedServers";
            this._lbl_SelectedServers.Size = new System.Drawing.Size(91, 13);
            this._lbl_SelectedServers.TabIndex = 3;
            this._lbl_SelectedServers.Text = "&Selected Servers:";
            // 
            // _infbx_SelectServers
            // 
            this._infbx_SelectServers.Location = new System.Drawing.Point(24, 0);
            this._infbx_SelectServers.Name = "_infbx_SelectServers";
            this._infbx_SelectServers.Size = new System.Drawing.Size(476, 44);
            this._infbx_SelectServers.TabIndex = 1;
            this._infbx_SelectServers.TabStop = false;
            this._infbx_SelectServers.Text = resources.GetString("_infbx_SelectServers.Text");
            // 
            // _loadServersStatus
            // 
            this._loadServersStatus.BackColor = System.Drawing.SystemColors.Window;
            this._loadServersStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._loadServersStatus.Location = new System.Drawing.Point(24, 62);
            this._loadServersStatus.Name = "_loadServersStatus";
            this._loadServersStatus.Size = new System.Drawing.Size(187, 212);
            this._loadServersStatus.TabIndex = 5;
            // 
            // _lstbx_AvailableServers
            // 
            this._lstbx_AvailableServers.FormattingEnabled = true;
            this._lstbx_AvailableServers.Location = new System.Drawing.Point(24, 62);
            this._lstbx_AvailableServers.Name = "_lstbx_AvailableServers";
            this._lstbx_AvailableServers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._lstbx_AvailableServers.Size = new System.Drawing.Size(187, 212);
            this._lstbx_AvailableServers.Sorted = true;
            this._lstbx_AvailableServers.TabIndex = 1;
            this._lstbx_AvailableServers.SelectedIndexChanged += new System.EventHandler(this._lstbx_AvailableServers_SelectedIndexChanged);
            this._lstbx_AvailableServers.DoubleClick += new System.EventHandler(this._lstbx_AvailableServers_DoubleClick);
            // 
            // _page_SpecifyImportDate
            // 
            this._page_SpecifyImportDate.Controls.Add(this._lstvw_SelectedServers);
            this._page_SpecifyImportDate.Controls.Add(this._dtEdtr_DatePicker);
            this._page_SpecifyImportDate.Controls.Add(this._lbl_LoadFrom);
            this._page_SpecifyImportDate.Controls.Add(this._infbx_SelectImportDate);
            this._page_SpecifyImportDate.Description = "Specify the date from which to import historical data.";
            this._page_SpecifyImportDate.Location = new System.Drawing.Point(19, 73);
            this._page_SpecifyImportDate.Name = "_page_SpecifyImportDate";
            this._page_SpecifyImportDate.NextPage = this._page_Finish;
            this._page_SpecifyImportDate.PreviousPage = this._page_SelectServers;
            this._page_SpecifyImportDate.Size = new System.Drawing.Size(525, 277);
            this._page_SpecifyImportDate.TabIndex = 0;
            this._page_SpecifyImportDate.Text = "Specify Import Date";
            this._page_SpecifyImportDate.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this._page_SpecifyImportDate_BeforeMoveNext);
            this._page_SpecifyImportDate.BeforeDisplay += new System.EventHandler(this._page_SpecifyImportDate_BeforeDisplay);
            // 
            // _lstvw_SelectedServers
            // 
            this._lstvw_SelectedServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colSQLServer,
            this._colCurrentAsOf});
            this._lstvw_SelectedServers.FullRowSelect = true;
            this._lstvw_SelectedServers.Location = new System.Drawing.Point(24, 51);
            this._lstvw_SelectedServers.MultiSelect = false;
            this._lstvw_SelectedServers.Name = "_lstvw_SelectedServers";
            this._lstvw_SelectedServers.Size = new System.Drawing.Size(476, 194);
            this._lstvw_SelectedServers.TabIndex = 6;
            this._lstvw_SelectedServers.TabStop = false;
            this._lstvw_SelectedServers.UseCompatibleStateImageBehavior = false;
            this._lstvw_SelectedServers.View = System.Windows.Forms.View.Details;
            // 
            // _colSQLServer
            // 
            this._colSQLServer.Text = "SQL Server";
            this._colSQLServer.Width = 345;
            // 
            // _colCurrentAsOf
            // 
            this._colCurrentAsOf.Text = "Current As Of";
            this._colCurrentAsOf.Width = 126;
            // 
            // _dtEdtr_DatePicker
            // 
            this._dtEdtr_DatePicker.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._dtEdtr_DatePicker.Location = new System.Drawing.Point(248, 251);
            this._dtEdtr_DatePicker.MaskInput = "{date}";
            this._dtEdtr_DatePicker.Name = "_dtEdtr_DatePicker";
            this._dtEdtr_DatePicker.Size = new System.Drawing.Size(144, 21);
            this._dtEdtr_DatePicker.TabIndex = 1;
            // 
            // _lbl_LoadFrom
            // 
            this._lbl_LoadFrom.AutoSize = true;
            this._lbl_LoadFrom.Location = new System.Drawing.Point(21, 255);
            this._lbl_LoadFrom.Name = "_lbl_LoadFrom";
            this._lbl_LoadFrom.Size = new System.Drawing.Size(221, 13);
            this._lbl_LoadFrom.TabIndex = 0;
            this._lbl_LoadFrom.Text = "&Import historical data from 00\\00\\00 back to: ";
            // 
            // _infbx_SelectImportDate
            // 
            this._infbx_SelectImportDate.Location = new System.Drawing.Point(24, 0);
            this._infbx_SelectImportDate.Name = "_infbx_SelectImportDate";
            this._infbx_SelectImportDate.Size = new System.Drawing.Size(476, 45);
            this._infbx_SelectImportDate.TabIndex = 0;
            this._infbx_SelectImportDate.TabStop = false;
            this._infbx_SelectImportDate.Text = resources.GetString("_infbx_SelectImportDate.Text");
            // 
            // _page_Finish
            // 
            this._page_Finish.Controls.Add(this._lbl_FinishImportDateLbl2);
            this._page_Finish.Controls.Add(this._lbl_FinishImportDate);
            this._page_Finish.Controls.Add(this._lstbx_FinishServers);
            this._page_Finish.Controls.Add(this._lbl_FinishImportDateLbl1);
            this._page_Finish.Controls.Add(this._lbl_FinishSQLServers);
            this._page_Finish.FinishText = "Verify that the following information is correct. To make changes, click the Back" +
                " button.";
            this._page_Finish.Location = new System.Drawing.Point(177, 66);
            this._page_Finish.Name = "_page_Finish";
            this._page_Finish.PreviousPage = this._page_SpecifyImportDate;
            this._page_Finish.ProceedText = "Click the Start Import button to import historical data.";
            this._page_Finish.Size = new System.Drawing.Size(373, 284);
            this._page_Finish.TabIndex = 1006;
            this._page_Finish.Text = "Completing the SQL diagnostic manager Import Wizard";
            this._page_Finish.BeforeDisplay += new System.EventHandler(this._page_Finish_BeforeDisplay);
            // 
            // _lbl_FinishImportDateLbl2
            // 
            this._lbl_FinishImportDateLbl2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lbl_FinishImportDateLbl2.AutoSize = true;
            this._lbl_FinishImportDateLbl2.Location = new System.Drawing.Point(198, 222);
            this._lbl_FinishImportDateLbl2.Name = "_lbl_FinishImportDateLbl2";
            this._lbl_FinishImportDateLbl2.Size = new System.Drawing.Size(45, 13);
            this._lbl_FinishImportDateLbl2.TabIndex = 4;
            this._lbl_FinishImportDateLbl2.Text = "forward.";
            // 
            // _lbl_FinishImportDate
            // 
            this._lbl_FinishImportDate.Location = new System.Drawing.Point(125, 222);
            this._lbl_FinishImportDate.Name = "_lbl_FinishImportDate";
            this._lbl_FinishImportDate.Size = new System.Drawing.Size(67, 13);
            this._lbl_FinishImportDate.TabIndex = 3;
            this._lbl_FinishImportDate.Text = "12/12/2222";
            // 
            // _lstbx_FinishServers
            // 
            this._lstbx_FinishServers.FormattingEnabled = true;
            this._lstbx_FinishServers.Location = new System.Drawing.Point(0, 59);
            this._lstbx_FinishServers.Name = "_lstbx_FinishServers";
            this._lstbx_FinishServers.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this._lstbx_FinishServers.Size = new System.Drawing.Size(373, 160);
            this._lstbx_FinishServers.TabIndex = 2;
            this._lstbx_FinishServers.TabStop = false;
            // 
            // _lbl_FinishImportDateLbl1
            // 
            this._lbl_FinishImportDateLbl1.AutoSize = true;
            this._lbl_FinishImportDateLbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lbl_FinishImportDateLbl1.Location = new System.Drawing.Point(-3, 222);
            this._lbl_FinishImportDateLbl1.Name = "_lbl_FinishImportDateLbl1";
            this._lbl_FinishImportDateLbl1.Size = new System.Drawing.Size(122, 13);
            this._lbl_FinishImportDateLbl1.TabIndex = 1;
            this._lbl_FinishImportDateLbl1.Text = "Load historical data from";
            // 
            // _lbl_FinishSQLServers
            // 
            this._lbl_FinishSQLServers.AutoSize = true;
            this._lbl_FinishSQLServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lbl_FinishSQLServers.Location = new System.Drawing.Point(-3, 43);
            this._lbl_FinishSQLServers.Name = "_lbl_FinishSQLServers";
            this._lbl_FinishSQLServers.Size = new System.Drawing.Size(241, 13);
            this._lbl_FinishSQLServers.TabIndex = 0;
            this._lbl_FinishSQLServers.Text = "SQL Servers selected for importing historical data:";
            // 
            // WizardMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 410);
            this.Controls.Add(this._wizard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL diagnostic manager Import Wizard";
            this._wizard.ResumeLayout(false);
            this._page_Introduction.ResumeLayout(false);
            this._page_Introduction.PerformLayout();
            this._page_RepositoryConnection.ResumeLayout(false);
            this._grpbx_RepositoryConnection.ResumeLayout(false);
            this._grpbx_RepositoryConnection.PerformLayout();
            this._grpbx_TestConnection.ResumeLayout(false);
            this._page_SelectServers.ResumeLayout(false);
            this._page_SelectServers.PerformLayout();
            this._page_SpecifyImportDate.ResumeLayout(false);
            this._page_SpecifyImportDate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dtEdtr_DatePicker)).EndInit();
            this._page_Finish.ResumeLayout(false);
            this._page_Finish.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard _wizard;
        private Divelements.WizardFramework.IntroductionPage _page_Introduction;
        private Divelements.WizardFramework.WizardPage _page_RepositoryConnection;
        private Divelements.WizardFramework.FinishPage _page_Finish;
        private System.Windows.Forms.Label _lbl_Intro1;
        private System.Windows.Forms.CheckBox _chkBx_HideImportWizardWelcomPage;
        private System.Windows.Forms.GroupBox _grpbx_RepositoryConnection;
        private System.Windows.Forms.TextBox _txtbx_Password;
        private System.Windows.Forms.Label _lbl_Password;
        private System.Windows.Forms.Label _lbl_UserName;
        private System.Windows.Forms.TextBox _txtbx_User;
        private System.Windows.Forms.ComboBox _cmbbx_Authentication;
        private System.Windows.Forms.Label _lbl_Authentication;
        private System.Windows.Forms.Label _lbl_Database;
        private System.Windows.Forms.Label _lbl_Server;
        private System.Windows.Forms.Button _btn_TestRepositoryConnection;
        private Divelements.WizardFramework.InformationBox _infbx_RepositoryConnection;
        private Divelements.WizardFramework.WizardPage _page_SelectServers;
        private Divelements.WizardFramework.InformationBox _infbx_SelectServers;
        private System.Windows.Forms.Button _btn_RemoveServer;
        private System.Windows.Forms.Button _btn_AddServer;
        private System.Windows.Forms.Label _lbl_SelectedServers;
        private System.Windows.Forms.ListBox _lstbx_SelectedServers;
        private System.Windows.Forms.Label _lbl_AvailableServers;
        private System.Windows.Forms.ListBox _lstbx_AvailableServers;
        private System.Windows.Forms.GroupBox _grpbx_TestConnection;
        private Idera.SQLdm.ImportWizard.Controls.TestConnectionStatus _testConnectionStatus;
        private Idera.SQLdm.ImportWizard.Controls.LoadServersStatus _loadServersStatus;
        private System.Windows.Forms.Button _btn_BrowseServer;
        private System.Windows.Forms.TextBox _txtbx_Server;
        private System.Windows.Forms.Button _btn_BrowseDatabase;
        private System.Windows.Forms.TextBox _txtbx_Database;
        private Divelements.WizardFramework.WizardPage _page_SpecifyImportDate;
        private Divelements.WizardFramework.InformationBox _infbx_SelectImportDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor _dtEdtr_DatePicker;
        private System.Windows.Forms.Label _lbl_LoadFrom;
        private System.Windows.Forms.ListView _lstvw_SelectedServers;
        private System.Windows.Forms.ColumnHeader _colSQLServer;
        private System.Windows.Forms.ColumnHeader _colCurrentAsOf;
        private System.Windows.Forms.Label _lbl_FinishImportDateLbl1;
        private System.Windows.Forms.Label _lbl_FinishSQLServers;
        private System.Windows.Forms.ListBox _lstbx_FinishServers;
        private System.Windows.Forms.Label _lbl_FinishImportDate;
        private System.Windows.Forms.Label _lbl_FinishImportDateLbl2;
    }
}

