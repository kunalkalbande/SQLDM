namespace SQLdmCWFInstaller
{
    partial class SQLdmInstallWizard
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLdmInstallWizard));
            this.welcomePage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.registerPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.groupBoxRegisterCWF = new System.Windows.Forms.GroupBox();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.radioButtonRemote = new System.Windows.Forms.RadioButton();
            this.groupBoxRemoteValues = new System.Windows.Forms.GroupBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelHostName = new System.Windows.Forms.Label();
            this.textBoxHostName = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelServiceAccount = new System.Windows.Forms.Label();
            this.textBoxServiceAccount = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.descriptionText = new System.Windows.Forms.RichTextBox();
            this.titleText = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.displayNamePage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.groupDisplayNameDetails = new System.Windows.Forms.GroupBox();
            this.labelRegisterName = new System.Windows.Forms.Label();
            this.textBoxRegisterName = new System.Windows.Forms.TextBox();
            this.labelDisplayNameMessage = new System.Windows.Forms.Label();
            this.finishPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.labelFinish = new System.Windows.Forms.Label();
            this.descriptionText = new System.Windows.Forms.RichTextBox();
            this.lnkLabel1 = new System.Windows.Forms.Label();//10.0
            this.lnkLabel2 = new System.Windows.Forms.LinkLabel();//10.0
            this.lnkLabel3 = new System.Windows.Forms.Label();//10.0
            this.titleText = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.installWizard = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.sharedPage = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.previousBtn = new System.Windows.Forms.Button();
            this.nextBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.welcomePage.SuspendLayout();
            this.registerPage.SuspendLayout();
            this.groupBoxRegisterCWF.SuspendLayout();
            this.groupBoxRemoteValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.displayNamePage.SuspendLayout();
            this.groupDisplayNameDetails.SuspendLayout();
            this.finishPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installWizard)).BeginInit();
            this.installWizard.SuspendLayout();
            this.sharedPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // welcomePage
            // 
            this.welcomePage.Controls.Add(this.descriptionText);
            this.welcomePage.Controls.Add(this.titleText);
            this.welcomePage.Controls.Add(this.pictureBox1);
            this.welcomePage.Location = new System.Drawing.Point(0, 0);
            this.welcomePage.Name = "welcomePage";
            this.welcomePage.Size = new System.Drawing.Size(503, 313);
            // 
            // registerPage
            // 
            this.registerPage.Controls.Add(this.groupBoxRegisterCWF);
            this.registerPage.Controls.Add(this.groupBoxRemoteValues);
            this.registerPage.Controls.Add(this.descriptionText);
            this.registerPage.Controls.Add(this.lnkLabel1);//10.0
            this.registerPage.Controls.Add(this.lnkLabel2);//10.0
            this.registerPage.Controls.Add(this.lnkLabel3);//10.0
            this.registerPage.Controls.Add(this.titleText);
            this.registerPage.Controls.Add(this.pictureBox1);
            this.registerPage.Location = new System.Drawing.Point(0, 0);
            this.registerPage.Name = "registerPage";
            this.registerPage.Size = new System.Drawing.Size(503, 313);
            // 
            // groupBoxRegisterCWF
            // 
            this.groupBoxRegisterCWF.BackColor = System.Drawing.Color.White;
            this.groupBoxRegisterCWF.Controls.Add(this.radioButtonLocal);
            this.groupBoxRegisterCWF.Controls.Add(this.radioButtonRemote);
            this.groupBoxRegisterCWF.Location = new System.Drawing.Point(178, 119);
            this.groupBoxRegisterCWF.Name = "groupBoxRegisterCWF";
            this.groupBoxRegisterCWF.Size = new System.Drawing.Size(314, 58);
            this.groupBoxRegisterCWF.TabIndex = 0;
            this.groupBoxRegisterCWF.TabStop = false;
            this.groupBoxRegisterCWF.Text = "Dashboard Registration Options";
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.Checked = true;
            this.radioButtonLocal.Location = new System.Drawing.Point(5, 15);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(14, 13);
            this.radioButtonLocal.TabIndex = 6;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.UseVisualStyleBackColor = true;
            this.radioButtonLocal.CheckedChanged += new System.EventHandler(this.radioButtonCWF_Click);
            // 
            // radioButtonRemote
            // 
            this.radioButtonRemote.AutoSize = true;
            this.radioButtonRemote.Location = new System.Drawing.Point(5, 33);
            this.radioButtonRemote.Name = "radioButtonRemote";
            this.radioButtonRemote.Size = new System.Drawing.Size(215, 17);
            this.radioButtonRemote.TabIndex = 7;
            this.radioButtonRemote.TabStop = true;
            this.radioButtonRemote.Text = "Register with a remote IDERA Dashboard.";
            this.radioButtonRemote.UseVisualStyleBackColor = true;
            // 
            // groupBoxRemoteValues
            // 
            this.groupBoxRemoteValues.BackColor = System.Drawing.Color.White;
            this.groupBoxRemoteValues.Controls.Add(this.labelPassword);
            this.groupBoxRemoteValues.Controls.Add(this.label2);
            this.groupBoxRemoteValues.Controls.Add(this.label1);
            this.groupBoxRemoteValues.Controls.Add(this.labelHostName);
            this.groupBoxRemoteValues.Controls.Add(this.textBoxHostName);
            this.groupBoxRemoteValues.Controls.Add(this.labelPort);
            this.groupBoxRemoteValues.Controls.Add(this.textBoxPort);
            this.groupBoxRemoteValues.Controls.Add(this.labelServiceAccount);
            this.groupBoxRemoteValues.Controls.Add(this.textBoxServiceAccount);
            this.groupBoxRemoteValues.Controls.Add(this.textBoxPassword);
            this.groupBoxRemoteValues.Location = new System.Drawing.Point(178, 184);
            this.groupBoxRemoteValues.Name = "groupBoxRemoteValues";
            this.groupBoxRemoteValues.Size = new System.Drawing.Size(313, 121);
            this.groupBoxRemoteValues.TabIndex = 0;
            this.groupBoxRemoteValues.TabStop = false;
            this.groupBoxRemoteValues.Text = "Dashboard Credentials";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(18, 96);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 13);
            this.labelPassword.TabIndex = 0;
            this.labelPassword.Text = "Password:";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(11, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(11, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "*";
            // 
            // labelHostName
            // 
            this.labelHostName.AutoSize = true;
            this.labelHostName.Location = new System.Drawing.Point(18, 24);
            this.labelHostName.Name = "labelHostName";
            this.labelHostName.Size = new System.Drawing.Size(63, 13);
            this.labelHostName.TabIndex = 0;
            this.labelHostName.Text = "Host Name:";
            this.labelHostName.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // textBoxHostName
            // 
            this.textBoxHostName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxHostName.Location = new System.Drawing.Point(119, 22);
            this.textBoxHostName.Name = "textBoxHostName";
            this.textBoxHostName.Size = new System.Drawing.Size(169, 20);
            this.textBoxHostName.TabIndex = 1;
            this.textBoxHostName.TextChanged += new System.EventHandler(this.textBoxCWF_Click);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(18, 48);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(29, 13);
            this.labelPort.TabIndex = 0;
            this.labelPort.Text = "Port:";
            this.labelPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxPort
            // 
            this.textBoxPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPort.Location = new System.Drawing.Point(119, 46);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(169, 20);
            this.textBoxPort.TabIndex = 1;
            this.textBoxPort.Text = "9292";
            this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxCWF_Click);
            // 
            // labelServiceAccount
            // 
            this.labelServiceAccount.AutoSize = true;
            this.labelServiceAccount.Location = new System.Drawing.Point(18, 72);
            this.labelServiceAccount.Name = "labelServiceAccount";
            this.labelServiceAccount.Size = new System.Drawing.Size(99, 13);
            this.labelServiceAccount.TabIndex = 0;
            this.labelServiceAccount.Text = "Domain\\Username:";
            this.labelServiceAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxServiceAccount
            // 
            this.textBoxServiceAccount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxServiceAccount.Location = new System.Drawing.Point(119, 70);
            this.textBoxServiceAccount.Name = "textBoxServiceAccount";
            this.textBoxServiceAccount.Size = new System.Drawing.Size(169, 20);
            this.textBoxServiceAccount.TabIndex = 1;
            this.textBoxServiceAccount.TextChanged += new System.EventHandler(this.textBoxCWF_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPassword.Location = new System.Drawing.Point(119, 94);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(169, 20);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBoxCWF_Click);
            // 
            // descriptionText
            // 
            this.descriptionText.BackColor = System.Drawing.Color.White;
            this.descriptionText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descriptionText.ForeColor = System.Drawing.Color.Black;
            this.descriptionText.Location = new System.Drawing.Point(175, 46);
            this.descriptionText.Name = "descriptionText";
            this.descriptionText.ReadOnly = true;
            this.descriptionText.Size = new System.Drawing.Size(310, 84);
            this.descriptionText.TabIndex = 0;
            this.descriptionText.Text = "";
            // 
            // titleText
            // 
            this.titleText.BackColor = System.Drawing.Color.White;
            this.titleText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.titleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleText.ForeColor = System.Drawing.Color.Black;
            this.titleText.Location = new System.Drawing.Point(175, 11);
            this.titleText.Name = "titleText";
            this.titleText.ReadOnly = true;
            this.titleText.Size = new System.Drawing.Size(310, 35);
            this.titleText.TabIndex = 1;
            this.titleText.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = global::SQLdmCWFInstaller.Properties.Resources.Image;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(503, 313);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // displayNamePage
            // 
            this.displayNamePage.Controls.Add(this.groupDisplayNameDetails);
            this.displayNamePage.Location = new System.Drawing.Point(-10000, -10000);
            this.displayNamePage.Name = "displayNamePage";
            this.displayNamePage.Size = new System.Drawing.Size(503, 313);
            // 
            // groupDisplayNameDetails
            // 
            this.groupDisplayNameDetails.BackColor = System.Drawing.Color.White;
            this.groupDisplayNameDetails.Controls.Add(this.labelRegisterName);
            this.groupDisplayNameDetails.Controls.Add(this.textBoxRegisterName);
            this.groupDisplayNameDetails.Controls.Add(this.labelDisplayNameMessage);
            this.groupDisplayNameDetails.Location = new System.Drawing.Point(178, 135);
            this.groupDisplayNameDetails.Name = "groupDisplayNameDetails";
            this.groupDisplayNameDetails.Size = new System.Drawing.Size(314, 169);
            this.groupDisplayNameDetails.TabIndex = 3;
            this.groupDisplayNameDetails.TabStop = false;
            // 
            // labelRegisterName
            // 
            this.labelRegisterName.AutoSize = true;
            this.labelRegisterName.BackColor = System.Drawing.Color.White;
            this.labelRegisterName.Location = new System.Drawing.Point(8, 20);
            this.labelRegisterName.Name = "labelRegisterName";
            this.labelRegisterName.Size = new System.Drawing.Size(75, 13);
            this.labelRegisterName.TabIndex = 0;
            this.labelRegisterName.Text = "Display Name:";
            this.labelRegisterName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxRegisterName
            // 
            this.textBoxRegisterName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRegisterName.Location = new System.Drawing.Point(90, 18);
            this.textBoxRegisterName.Name = "textBoxRegisterName";
            this.textBoxRegisterName.Size = new System.Drawing.Size(145, 20);
            this.textBoxRegisterName.TabIndex = 1;
            // 
            // labelDisplayNameMessage
            // 
            this.labelDisplayNameMessage.AutoSize = true;
            this.labelDisplayNameMessage.ForeColor = System.Drawing.Color.Red;
            this.labelDisplayNameMessage.Location = new System.Drawing.Point(90, 40);
            this.labelDisplayNameMessage.Name = "labelDisplayNameMessage";
            this.labelDisplayNameMessage.Size = new System.Drawing.Size(0, 13);
            this.labelDisplayNameMessage.TabIndex = 2;
            this.labelDisplayNameMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // finishPage
            // 
            this.finishPage.Controls.Add(this.labelFinish);
            this.finishPage.Location = new System.Drawing.Point(-10000, -10000);
            this.finishPage.Name = "finishPage";
            this.finishPage.Size = new System.Drawing.Size(503, 313);
            // 
            // labelFinish
            // 
            this.labelFinish.AutoSize = true;
            this.labelFinish.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFinish.Location = new System.Drawing.Point(268, 178);
            this.labelFinish.Name = "labelFinish";
            this.labelFinish.Size = new System.Drawing.Size(105, 15);
            this.labelFinish.TabIndex = 3;
            this.labelFinish.Text = "Finish Page Controls";
            this.labelFinish.Visible = false;
            // 
            // descriptionText
            // 
            this.descriptionText.BackColor = System.Drawing.Color.White;
            this.descriptionText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descriptionText.ForeColor = System.Drawing.Color.Black;
            this.descriptionText.Location = new System.Drawing.Point(175, 46);
            this.descriptionText.Name = "descriptionText";
            this.descriptionText.ReadOnly = true;
            this.descriptionText.Size = new System.Drawing.Size(310, 160);
            this.descriptionText.TabIndex = 0;
            this.descriptionText.Text = "";
            // 
            // lnklabel1
            // 
            this.lnkLabel1.BackColor = System.Drawing.Color.White;
            this.lnkLabel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lnkLabel1.ForeColor = System.Drawing.Color.Black;
            this.lnkLabel1.Location = new System.Drawing.Point(175, 210);
            this.lnkLabel1.Name = "lnkLabel1";
            this.lnkLabel1.Size = new System.Drawing.Size(30, 15);
            this.lnkLabel1.TabIndex = 0;
            this.lnkLabel1.Text = "Click";
            // 
            // lnklabel2
            // 
            this.lnkLabel2.BackColor = System.Drawing.Color.White;
            this.lnkLabel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lnkLabel2.ForeColor = System.Drawing.Color.Black;
            this.lnkLabel2.Location = new System.Drawing.Point(205, 210);
            this.lnkLabel2.Name = "lnkLabel2";
            this.lnkLabel2.Size = new System.Drawing.Size(28, 40);
            this.lnkLabel2.TabIndex = 0;
            this.lnkLabel2.Text = "here";
            // 
            // lnklabel3
            // 
            this.lnkLabel3.BackColor = System.Drawing.Color.White;
            this.lnkLabel3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lnkLabel3.ForeColor = System.Drawing.Color.Black;
            this.lnkLabel3.Location = new System.Drawing.Point(233, 210);
            this.lnkLabel3.Name = "lnkLabel3";
            this.lnkLabel3.Size = new System.Drawing.Size(250, 15);
            this.lnkLabel3.TabIndex = 0;
            this.lnkLabel3.Text = "to learn more about the IDERA Dashboard.";
            // 
            // titleText
            // 
            this.titleText.BackColor = System.Drawing.Color.White;
            this.titleText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.titleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleText.ForeColor = System.Drawing.Color.Black;
            this.titleText.Location = new System.Drawing.Point(175, 11);
            this.titleText.Name = "titleText";
            this.titleText.ReadOnly = true;
            this.titleText.Size = new System.Drawing.Size(310, 35);
            this.titleText.TabIndex = 1;
            this.titleText.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = global::SQLdmCWFInstaller.Properties.Resources.Image;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(503, 313);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // installWizard
            // 
            this.installWizard.Controls.Add(this.welcomePage);
            this.installWizard.Controls.Add(this.registerPage);
            this.installWizard.Controls.Add(this.displayNamePage);
            this.installWizard.Controls.Add(this.finishPage);
            this.installWizard.Controls.Add(this.sharedPage);
            this.installWizard.Dock = System.Windows.Forms.DockStyle.Top;
            this.installWizard.Location = new System.Drawing.Point(0, 0);
            this.installWizard.Name = "installWizard";
            this.installWizard.SharedControls.AddRange(new System.Windows.Forms.Control[] {
            this.descriptionText,
            this.titleText,
            this.pictureBox1,this.lnkLabel1,this.lnkLabel2,this.lnkLabel3});
            this.installWizard.SharedControlsPage = this.sharedPage;
            this.installWizard.Size = new System.Drawing.Size(503, 313);
            this.installWizard.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.installWizard.TabIndex = 2;
            ultraTab1.TabPage = this.welcomePage;
            ultraTab1.Text = "welcome page";
            ultraTab2.TabPage = this.registerPage;
            ultraTab2.Text = "register Page";
            ultraTab3.TabPage = this.displayNamePage;
            ultraTab3.Text = "Display name page";
            ultraTab4.TabPage = this.finishPage;
            ultraTab4.Text = "finish Page";
            this.installWizard.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4});
            // 
            // sharedPage
            // 
            this.sharedPage.Controls.Add(this.descriptionText);
            this.sharedPage.Controls.Add(this.titleText);
            this.sharedPage.Controls.Add(this.lnkLabel1);//10.0
            this.sharedPage.Controls.Add(this.lnkLabel2);//10.0
            this.sharedPage.Controls.Add(this.lnkLabel3);//10.0
            this.sharedPage.Controls.Add(this.pictureBox1);
            this.sharedPage.Location = new System.Drawing.Point(-10000, -10000);
            this.sharedPage.Name = "sharedPage";
            this.sharedPage.Size = new System.Drawing.Size(503, 313);
            // 
            // previousBtn
            // 
            this.previousBtn.Location = new System.Drawing.Point(259, 319);
            this.previousBtn.Name = "previousBtn";
            this.previousBtn.Size = new System.Drawing.Size(75, 23);
            this.previousBtn.TabIndex = 1;
            this.previousBtn.Text = "Previous";
            this.previousBtn.UseVisualStyleBackColor = true;
            this.previousBtn.Click += new System.EventHandler(this.previousBtn_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.Location = new System.Drawing.Point(340, 319);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(75, 23);
            this.nextBtn.TabIndex = 2;
            this.nextBtn.Text = "Next";
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(421, 319);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 3;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // SQLdmInstallWizard
            // 
            this.AcceptButton = this.nextBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(503, 346);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.nextBtn);
            this.Controls.Add(this.previousBtn);
            this.Controls.Add(this.installWizard);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SQLdmInstallWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager";
            this.welcomePage.ResumeLayout(false);
            this.welcomePage.PerformLayout();
            this.registerPage.ResumeLayout(false);
            this.groupBoxRegisterCWF.ResumeLayout(false);
            this.groupBoxRegisterCWF.PerformLayout();
            this.groupBoxRemoteValues.ResumeLayout(false);
            this.groupBoxRemoteValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.displayNamePage.ResumeLayout(false);
            this.groupDisplayNameDetails.ResumeLayout(false);
            this.groupDisplayNameDetails.PerformLayout();
            this.finishPage.ResumeLayout(false);
            this.finishPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installWizard)).EndInit();
            this.installWizard.ResumeLayout(false);
            this.sharedPage.ResumeLayout(false);
            this.sharedPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl installWizard;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl welcomePage;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl registerPage;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl displayNamePage;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl finishPage;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage sharedPage;
        private System.Windows.Forms.Button previousBtn;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.RichTextBox descriptionText;
        private System.Windows.Forms.Label lnkLabel1;//10.0
        private System.Windows.Forms.LinkLabel lnkLabel2;//10.0
        private System.Windows.Forms.Label lnkLabel3;//10.0
        private System.Windows.Forms.RichTextBox titleText;
        private System.Windows.Forms.GroupBox groupBoxRegisterCWF;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.RadioButton radioButtonRemote;
        private System.Windows.Forms.GroupBox groupBoxRemoteValues;
        private System.Windows.Forms.Label labelHostName;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelServiceAccount;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxHostName;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxServiceAccount;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelRegisterName;
        private System.Windows.Forms.Label labelDisplayNameMessage;
        private System.Windows.Forms.TextBox textBoxRegisterName;
        private System.Windows.Forms.Label labelFinish;
        private System.Windows.Forms.GroupBox groupDisplayNameDetails;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

