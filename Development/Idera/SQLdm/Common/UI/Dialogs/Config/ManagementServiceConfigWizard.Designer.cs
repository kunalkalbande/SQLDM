namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    using Divelements.WizardFramework;
    using System.Windows.Forms;

    partial class ManagementServiceConfigWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagementServiceConfigWizard));
            this.wizard = new Divelements.WizardFramework.Wizard();
            this.finishPage = new Divelements.WizardFramework.FinishPage();
            this._pnl_FinishSummary = new System.Windows.Forms.Panel();
            this.nServer = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nAuthentication = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nDatabase = new System.Windows.Forms.Label();
            this.oAuthentication = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.oDatabase = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.oServer = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.settingsPage = new Divelements.WizardFramework.WizardPage();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._grpbx_TestConnection = new System.Windows.Forms.GroupBox();
            this.testStatus = new System.Windows.Forms.LinkLabel();
            this.btnTest = new System.Windows.Forms.Button();
            this.testProgress = new MRG.Controls.UI.LoadingCircle();
            this.testErrorImage = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cboAuthentication = new System.Windows.Forms.ComboBox();
            this._lbl_Authentication = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtLoginID = new System.Windows.Forms.TextBox();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this._introductionPage = new Divelements.WizardFramework.IntroductionPage();
            this._lbl_Status = new System.Windows.Forms.Label();
            this._lbl_Intro1 = new System.Windows.Forms.Label();
            this._lnklbl_ConnectionStatus = new System.Windows.Forms.LinkLabel();
            this._loadingCircle = new MRG.Controls.UI.LoadingCircle();
            this._pctrbx_IntroError = new System.Windows.Forms.PictureBox();
            this.wizard.SuspendLayout();
            this.finishPage.SuspendLayout();
            this._pnl_FinishSummary.SuspendLayout();
            this.settingsPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this._grpbx_TestConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testErrorImage)).BeginInit();
            this._introductionPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pctrbx_IntroError)).BeginInit();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.BannerImage = global::Idera.SQLdm.Common.UI.Properties.Resources.ConfigurationWizardBannerImage_49x49;
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this._introductionPage);
            this.wizard.Controls.Add(this.settingsPage);
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.MarginImage = global::Idera.SQLdm.Common.UI.Properties.Resources.ConfigurationWizardMargin_164x363;
            this.wizard.Name = "wizard";
            this.wizard.OwnerForm = this;
            this.wizard.Size = new System.Drawing.Size(537, 410);
            this.wizard.TabIndex = 0;
            this.wizard.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizard.Cancel += new System.EventHandler(this.wizard_Cancel);
            this.wizard.Finish += new System.EventHandler(this.wizard_Finish);
            // 
            // finishPage
            // 
            this.finishPage.Controls.Add(this._pnl_FinishSummary);
            this.finishPage.FinishText = "FinishText";
            this.finishPage.Location = new System.Drawing.Point(175, 71);
            this.finishPage.Name = "finishPage";
            this.finishPage.PreviousPage = this.settingsPage;
            this.finishPage.ProceedText = "ProceedText";
            this.finishPage.Size = new System.Drawing.Size(340, 281);
            this.finishPage.TabIndex = 0;
            this.finishPage.Text = "Completing the SQLdm Management Service Configuration Wizard";
            this.finishPage.BeforeDisplay += new System.EventHandler(this.finishPage_BeforeDisplay);
            // 
            // _pnl_FinishSummary
            // 
            this._pnl_FinishSummary.Controls.Add(this.nServer);
            this._pnl_FinishSummary.Controls.Add(this.label6);
            this._pnl_FinishSummary.Controls.Add(this.label7);
            this._pnl_FinishSummary.Controls.Add(this.nAuthentication);
            this._pnl_FinishSummary.Controls.Add(this.label11);
            this._pnl_FinishSummary.Controls.Add(this.nDatabase);
            this._pnl_FinishSummary.Controls.Add(this.oAuthentication);
            this._pnl_FinishSummary.Controls.Add(this.label4);
            this._pnl_FinishSummary.Controls.Add(this.label2);
            this._pnl_FinishSummary.Controls.Add(this.label8);
            this._pnl_FinishSummary.Controls.Add(this.oDatabase);
            this._pnl_FinishSummary.Controls.Add(this.label3);
            this._pnl_FinishSummary.Controls.Add(this.oServer);
            this._pnl_FinishSummary.Controls.Add(this.label1);
            this._pnl_FinishSummary.Location = new System.Drawing.Point(3, 45);
            this._pnl_FinishSummary.Name = "_pnl_FinishSummary";
            this._pnl_FinishSummary.Size = new System.Drawing.Size(341, 164);
            this._pnl_FinishSummary.TabIndex = 0;
            // 
            // nServer
            // 
            this.nServer.AutoSize = true;
            this.nServer.Location = new System.Drawing.Point(97, 102);
            this.nServer.Name = "nServer";
            this.nServer.Size = new System.Drawing.Size(41, 15);
            this.nServer.TabIndex = 16;
            this.nServer.Text = "label2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Database:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Server:";
            // 
            // nAuthentication
            // 
            this.nAuthentication.AutoSize = true;
            this.nAuthentication.Location = new System.Drawing.Point(97, 134);
            this.nAuthentication.Name = "nAuthentication";
            this.nAuthentication.Size = new System.Drawing.Size(41, 15);
            this.nAuthentication.TabIndex = 13;
            this.nAuthentication.Text = "label4";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 15);
            this.label11.TabIndex = 12;
            this.label11.Text = "Authentication:";
            // 
            // nDatabase
            // 
            this.nDatabase.AutoSize = true;
            this.nDatabase.Location = new System.Drawing.Point(97, 118);
            this.nDatabase.Name = "nDatabase";
            this.nDatabase.Size = new System.Drawing.Size(41, 15);
            this.nDatabase.TabIndex = 11;
            this.nDatabase.Text = "label2";
            // 
            // oAuthentication
            // 
            this.oAuthentication.AutoSize = true;
            this.oAuthentication.Location = new System.Drawing.Point(97, 54);
            this.oAuthentication.Name = "oAuthentication";
            this.oAuthentication.Size = new System.Drawing.Size(41, 15);
            this.oAuthentication.TabIndex = 10;
            this.oAuthentication.Text = "label2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Database:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Server:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "New Configuration:";
            // 
            // oDatabase
            // 
            this.oDatabase.AutoSize = true;
            this.oDatabase.Location = new System.Drawing.Point(97, 38);
            this.oDatabase.Name = "oDatabase";
            this.oDatabase.Size = new System.Drawing.Size(41, 15);
            this.oDatabase.TabIndex = 3;
            this.oDatabase.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Authentication:";
            // 
            // oServer
            // 
            this.oServer.AutoSize = true;
            this.oServer.Location = new System.Drawing.Point(97, 22);
            this.oServer.Name = "oServer";
            this.oServer.Size = new System.Drawing.Size(41, 15);
            this.oServer.TabIndex = 1;
            this.oServer.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Original Configuration:";
            // 
            // settingsPage
            // 
            this.settingsPage.Controls.Add(this.informationBox1);
            this.settingsPage.Controls.Add(this.groupBox1);
            this.settingsPage.Description = "Specify the location and credentials used by the Management Service to connect to" +
                " the Repository.";
            this.settingsPage.Location = new System.Drawing.Point(19, 73);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.NextPage = this.finishPage;
            this.settingsPage.PreviousPage = this._introductionPage;
            this.settingsPage.Size = new System.Drawing.Size(499, 277);
            this.settingsPage.TabIndex = 0;
            this.settingsPage.Text = "Specify SQLdm Repository";
            this.settingsPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.settingsPage_BeforeMoveNext);
            this.settingsPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.settingsPage_BeforeMoveBack);
            this.settingsPage.BeforeDisplay += new System.EventHandler(this.settingsPage_BeforeDisplay);
            // 
            // informationBox1
            // 
            this.informationBox1.Location = new System.Drawing.Point(32, 4);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(437, 40);
            this.informationBox1.TabIndex = 0;
            this.informationBox1.TabStop = false;
            this.informationBox1.Text = "The information you provide below is used by the Management Service to connect to" +
                " the Repository. The Management Service writes configuration and scheduled colle" +
                "ction data to the Repository.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._grpbx_TestConnection);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.cboAuthentication);
            this.groupBox1.Controls.Add(this._lbl_Authentication);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtLoginID);
            this.groupBox1.Controls.Add(this.cboServer);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.cboDatabase);
            this.groupBox1.Location = new System.Drawing.Point(32, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(437, 209);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Repository";
            // 
            // _grpbx_TestConnection
            // 
            this._grpbx_TestConnection.Controls.Add(this.testStatus);
            this._grpbx_TestConnection.Controls.Add(this.btnTest);
            this._grpbx_TestConnection.Controls.Add(this.testProgress);
            this._grpbx_TestConnection.Controls.Add(this.testErrorImage);
            this._grpbx_TestConnection.Location = new System.Drawing.Point(6, 152);
            this._grpbx_TestConnection.Name = "_grpbx_TestConnection";
            this._grpbx_TestConnection.Size = new System.Drawing.Size(425, 50);
            this._grpbx_TestConnection.TabIndex = 10;
            this._grpbx_TestConnection.TabStop = false;
            // 
            // testStatus
            // 
            this.testStatus.AutoSize = true;
            this.testStatus.LinkArea = new System.Windows.Forms.LinkArea(37, 17);
            this.testStatus.Location = new System.Drawing.Point(137, 22);
            this.testStatus.Name = "testStatus";
            this.testStatus.Size = new System.Drawing.Size(259, 17);
            this.testStatus.TabIndex = 2;
            this.testStatus.TabStop = true;
            this.testStatus.Text = "Failed to connect to the Repository, click for details";
            this.testStatus.UseCompatibleTextRendering = true;
            this.testStatus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.testStatus_LinkClicked);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(5, 19);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(91, 23);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "&Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // testProgress
            // 
            this.testProgress.Active = false;
            this.testProgress.Color = System.Drawing.Color.DarkGray;
            this.testProgress.InnerCircleRadius = 5;
            this.testProgress.Location = new System.Drawing.Point(106, 16);
            this.testProgress.Name = "testProgress";
            this.testProgress.NumberSpoke = 12;
            this.testProgress.OuterCircleRadius = 11;
            this.testProgress.RotationSpeed = 100;
            this.testProgress.Size = new System.Drawing.Size(25, 28);
            this.testProgress.SpokeThickness = 2;
            this.testProgress.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.testProgress.TabIndex = 1;
            this.testProgress.TabStop = false;
            // 
            // testErrorImage
            // 
            this.testErrorImage.Image = global::Idera.SQLdm.Common.UI.Properties.Resources.Critical32x32;
            this.testErrorImage.Location = new System.Drawing.Point(106, 16);
            this.testErrorImage.Name = "testErrorImage";
            this.testErrorImage.Size = new System.Drawing.Size(25, 28);
            this.testErrorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.testErrorImage.TabIndex = 6;
            this.testErrorImage.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "&Server:";
            // 
            // cboAuthentication
            // 
            this.cboAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAuthentication.FormattingEnabled = true;
            this.cboAuthentication.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.cboAuthentication.Location = new System.Drawing.Point(92, 73);
            this.cboAuthentication.Name = "cboAuthentication";
            this.cboAuthentication.Size = new System.Drawing.Size(339, 21);
            this.cboAuthentication.TabIndex = 5;
            this.cboAuthentication.SelectionChangeCommitted += new System.EventHandler(this.cboAuthentication_SelectionChangeCommitted);
            this.cboAuthentication.TextUpdate += new System.EventHandler(this.cboAuthentication_TextUpdate);
            // 
            // _lbl_Authentication
            // 
            this._lbl_Authentication.AutoSize = true;
            this._lbl_Authentication.Location = new System.Drawing.Point(8, 76);
            this._lbl_Authentication.Name = "_lbl_Authentication";
            this._lbl_Authentication.Size = new System.Drawing.Size(87, 15);
            this._lbl_Authentication.TabIndex = 4;
            this._lbl_Authentication.Text = "&Authentication:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(121, 126);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(310, 20);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // txtLoginID
            // 
            this.txtLoginID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLoginID.Enabled = false;
            this.txtLoginID.Location = new System.Drawing.Point(121, 100);
            this.txtLoginID.Name = "txtLoginID";
            this.txtLoginID.Size = new System.Drawing.Size(310, 20);
            this.txtLoginID.TabIndex = 7;
            this.txtLoginID.TextChanged += new System.EventHandler(this.txtLoginID_TextChanged);
            // 
            // cboServer
            // 
            this.cboServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboServer.FormattingEnabled = true;
            this.cboServer.Location = new System.Drawing.Point(92, 19);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(339, 21);
            this.cboServer.TabIndex = 1;
            this.cboServer.SelectionChangeCommitted += new System.EventHandler(this.cboServer_SelectionChangeCommitted);
            this.cboServer.TextUpdate += new System.EventHandler(this.cboServer_TextUpdate);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(52, 129);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 15);
            this.label13.TabIndex = 8;
            this.label13.Text = "&Password:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 15);
            this.label17.TabIndex = 2;
            this.label17.Text = "&Database:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(52, 103);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 15);
            this.label14.TabIndex = 6;
            this.label14.Text = "&User Name:";
            // 
            // cboDatabase
            // 
            this.cboDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Location = new System.Drawing.Point(92, 46);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(339, 21);
            this.cboDatabase.TabIndex = 3;
            this.cboDatabase.SelectionChangeCommitted += new System.EventHandler(this.cboDatabase_SelectionChangeCommitted);
            this.cboDatabase.TextUpdate += new System.EventHandler(this.cboDatabase_TextUpdate);
            // 
            // _introductionPage
            // 
            this._introductionPage.Controls.Add(this._lbl_Status);
            this._introductionPage.Controls.Add(this._lbl_Intro1);
            this._introductionPage.Controls.Add(this._lnklbl_ConnectionStatus);
            this._introductionPage.Controls.Add(this._loadingCircle);
            this._introductionPage.Controls.Add(this._pctrbx_IntroError);
            this._introductionPage.IntroductionText = "";
            this._introductionPage.Location = new System.Drawing.Point(193, 78);
            this._introductionPage.Name = "_introductionPage";
            this._introductionPage.NextPage = this.settingsPage;
            this._introductionPage.ProceedText = "";
            this._introductionPage.Size = new System.Drawing.Size(320, 268);
            this._introductionPage.TabIndex = 0;
            this._introductionPage.Text = "Welcome to the SQLdm Management Service Configuration Wizard";
            // 
            // _lbl_Status
            // 
            this._lbl_Status.AutoSize = true;
            this._lbl_Status.Location = new System.Drawing.Point(0, 237);
            this._lbl_Status.Name = "_lbl_Status";
            this._lbl_Status.Size = new System.Drawing.Size(52, 15);
            this._lbl_Status.TabIndex = 7;
            this._lbl_Status.Text = "Port info";
            // 
            // _lbl_Intro1
            // 
            this._lbl_Intro1.Location = new System.Drawing.Point(0, 0);
            this._lbl_Intro1.Name = "_lbl_Intro1";
            this._lbl_Intro1.Size = new System.Drawing.Size(346, 71);
            this._lbl_Intro1.TabIndex = 6;
            this._lbl_Intro1.Text = "This wizard helps you configure the Repository used by the SQLdm Management Servi" +
                "ce.\r\n\r\nThe SQLdm Management Service writes configuration and scheduled collectio" +
                "n data to the Repository.\r\n";
            // 
            // _lnklbl_ConnectionStatus
            // 
            this._lnklbl_ConnectionStatus.AutoSize = true;
            this._lnklbl_ConnectionStatus.LinkArea = new System.Windows.Forms.LinkArea(45, 17);
            this._lnklbl_ConnectionStatus.Location = new System.Drawing.Point(34, 259);
            this._lnklbl_ConnectionStatus.Name = "_lnklbl_ConnectionStatus";
            this._lnklbl_ConnectionStatus.Size = new System.Drawing.Size(312, 17);
            this._lnklbl_ConnectionStatus.TabIndex = 0;
            this._lnklbl_ConnectionStatus.TabStop = true;
            this._lnklbl_ConnectionStatus.Text = "Failed to connect to the Management Service, click for details";
            this._lnklbl_ConnectionStatus.UseCompatibleTextRendering = true;
            this._lnklbl_ConnectionStatus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnklbl_ConnectionStatus_LinkClicked);
            // 
            // _loadingCircle
            // 
            this._loadingCircle.Active = false;
            this._loadingCircle.Color = System.Drawing.Color.DarkGray;
            this._loadingCircle.InnerCircleRadius = 5;
            this._loadingCircle.Location = new System.Drawing.Point(3, 253);
            this._loadingCircle.Name = "_loadingCircle";
            this._loadingCircle.NumberSpoke = 12;
            this._loadingCircle.OuterCircleRadius = 11;
            this._loadingCircle.RotationSpeed = 100;
            this._loadingCircle.Size = new System.Drawing.Size(25, 28);
            this._loadingCircle.SpokeThickness = 2;
            this._loadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this._loadingCircle.TabIndex = 3;
            this._loadingCircle.TabStop = false;
            // 
            // _pctrbx_IntroError
            // 
            this._pctrbx_IntroError.Image = global::Idera.SQLdm.Common.UI.Properties.Resources.Critical32x32;
            this._pctrbx_IntroError.Location = new System.Drawing.Point(3, 253);
            this._pctrbx_IntroError.Name = "_pctrbx_IntroError";
            this._pctrbx_IntroError.Size = new System.Drawing.Size(25, 28);
            this._pctrbx_IntroError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pctrbx_IntroError.TabIndex = 5;
            this._pctrbx_IntroError.TabStop = false;
            // 
            // ManagementServiceConfigWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 410);
            this.Controls.Add(this.wizard);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagementServiceConfigWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQLdm Management Service Configuration Wizard";
            this.Load += new System.EventHandler(this.ConfigWizardDialog_Load);
            this.wizard.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this._pnl_FinishSummary.ResumeLayout(false);
            this._pnl_FinishSummary.PerformLayout();
            this.settingsPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this._grpbx_TestConnection.ResumeLayout(false);
            this._grpbx_TestConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testErrorImage)).EndInit();
            this._introductionPage.ResumeLayout(false);
            this._introductionPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pctrbx_IntroError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private Divelements.WizardFramework.Wizard wizard;
        private Divelements.WizardFramework.IntroductionPage _introductionPage;
        private Divelements.WizardFramework.FinishPage finishPage;
        private WizardPage settingsPage;
        private Label label12;
        private ComboBox cboServer;
        private TextBox txtPassword;
        private TextBox txtLoginID;
        private Label label13;
        private Label label14;
        private ComboBox cboDatabase;
        private Label label17;
        private MRG.Controls.UI.LoadingCircle _loadingCircle;
        private LinkLabel _lnklbl_ConnectionStatus;
        private PictureBox _pctrbx_IntroError;
        private GroupBox groupBox1;
        private ComboBox cboAuthentication;
        private Label _lbl_Authentication;
        private GroupBox _grpbx_TestConnection;
        private Button btnTest;
        private MRG.Controls.UI.LoadingCircle testProgress;
        private LinkLabel testStatus;
        private PictureBox testErrorImage;
        private InformationBox informationBox1;
        private Label _lbl_Intro1;
        private Label _lbl_Status;
        private Panel _pnl_FinishSummary;
        private Label label4;
        private Label label2;
        private Label label8;
        private Label oDatabase;
        private Label label3;
        private Label oServer;
        private Label label1;
        private Label nServer;
        private Label label6;
        private Label label7;
        private Label nAuthentication;
        private Label label11;
        private Label nDatabase;
        private Label oAuthentication;

    }
}