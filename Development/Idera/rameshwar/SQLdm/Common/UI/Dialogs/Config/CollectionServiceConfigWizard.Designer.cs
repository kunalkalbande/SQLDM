namespace Idera.SQLdm.Common.UI.Dialogs.Config
{
    partial class CollectionServiceConfigWizard
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.wizard = new Divelements.WizardFramework.Wizard();
            this.introductionPage = new Divelements.WizardFramework.IntroductionPage();
            this.informationBox2 = new Divelements.WizardFramework.InformationBox();
            this.selectServicePage = new Divelements.WizardFramework.WizardPage();
            this.portList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.machineName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.connectWaitPage = new Divelements.WizardFramework.WizardPage();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.settingsPage = new Divelements.WizardFramework.WizardPage();
            this.servicePortSpinner = new System.Windows.Forms.NumericUpDown();
            this.collectionServiceSource = new System.Windows.Forms.BindingSource(this.components);
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.txtInstanceName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.finishPage1 = new Divelements.WizardFramework.FinishPage();
            this.wizard.SuspendLayout();
            this.introductionPage.SuspendLayout();
            this.selectServicePage.SuspendLayout();
            this.connectWaitPage.SuspendLayout();
            this.settingsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servicePortSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionServiceSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // wizard
            // 
            this.wizard.BannerImage = global::Idera.SQLdm.Common.UI.Properties.Resources.ConfigurationWizardBannerImage_49x49;
            this.wizard.Controls.Add(this.introductionPage);
            this.wizard.Controls.Add(this.settingsPage);
            this.wizard.Controls.Add(this.selectServicePage);
            this.wizard.Controls.Add(this.finishPage1);
            this.wizard.Controls.Add(this.connectWaitPage);
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.MarginImage = global::Idera.SQLdm.Common.UI.Properties.Resources.ConfigurationWizardMargin_164x363;
            this.wizard.Name = "wizard";
            this.wizard.OwnerForm = this;
            this.wizard.Size = new System.Drawing.Size(523, 410);
            this.wizard.TabIndex = 0;
            this.wizard.Cancel += new System.EventHandler(this.wizard_Cancel);
            this.wizard.Finish += new System.EventHandler(this.wizard_Finish);
            // 
            // introductionPage
            // 
            this.introductionPage.Controls.Add(this.informationBox2);
            this.introductionPage.IntroductionText = "";
            this.introductionPage.Location = new System.Drawing.Point(42, 124);
            this.introductionPage.Name = "introductionPage";
            this.introductionPage.NextPage = this.selectServicePage;
            this.introductionPage.Size = new System.Drawing.Size(457, 209);
            this.introductionPage.TabIndex = 1004;
            this.introductionPage.Text = "Welcome to the SQLdm Collection Service Configuration Wizard";
            this.introductionPage.BeforeDisplay += new System.EventHandler(this.introductionPage_BeforeDisplay);
            // 
            // informationBox2
            // 
            this.informationBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox2.Location = new System.Drawing.Point(26, 0);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(274, 127);
            this.informationBox2.TabIndex = 2;
            this.informationBox2.Text = "This wizard will guide you through setting the connection properties for the SQLd" +
                "m Collection Service.";
            // 
            // selectServicePage
            // 
            this.selectServicePage.Controls.Add(this.portList);
            this.selectServicePage.Controls.Add(this.label2);
            this.selectServicePage.Controls.Add(this.machineName);
            this.selectServicePage.Controls.Add(this.label3);
            this.selectServicePage.Description = "Enter the computer name and port for the collection service.";
            this.selectServicePage.Location = new System.Drawing.Point(19, 73);
            this.selectServicePage.Name = "selectServicePage";
            this.selectServicePage.NextPage = this.connectWaitPage;
            this.selectServicePage.PreviousPage = this.introductionPage;
            this.selectServicePage.Size = new System.Drawing.Size(485, 277);
            this.selectServicePage.TabIndex = 1005;
            this.selectServicePage.Text = "Select Collection Service";
            this.selectServicePage.BeforeDisplay += new System.EventHandler(this.selectServicePage_BeforeDisplay);
            // 
            // portList
            // 
            this.portList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.portList.FormattingEnabled = true;
            this.portList.Location = new System.Drawing.Point(137, 123);
            this.portList.Name = "portList";
            this.portList.Size = new System.Drawing.Size(175, 21);
            this.portList.TabIndex = 13;
            this.portList.Text = "5167";
            this.portList.SelectionChangeCommitted += new System.EventHandler(this.portList_SelectionChangeCommitted);
            this.portList.TextUpdate += new System.EventHandler(this.portList_TextUpdate);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "Service Port:";
            // 
            // machineName
            // 
            this.machineName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.machineName.Location = new System.Drawing.Point(137, 99);
            this.machineName.Name = "machineName";
            this.machineName.Size = new System.Drawing.Size(299, 20);
            this.machineName.TabIndex = 11;
            this.machineName.Leave += new System.EventHandler(this.machineName_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Machine Name:";
            // 
            // connectWaitPage
            // 
            this.connectWaitPage.AllowMoveNext = false;
            this.connectWaitPage.AllowMovePrevious = false;
            this.connectWaitPage.Controls.Add(this.progressBar1);
            this.connectWaitPage.Description = "";
            this.connectWaitPage.Location = new System.Drawing.Point(19, 73);
            this.connectWaitPage.Name = "connectWaitPage";
            this.connectWaitPage.NextPage = this.settingsPage;
            this.connectWaitPage.PreviousPage = this.selectServicePage;
            this.connectWaitPage.Size = new System.Drawing.Size(485, 277);
            this.connectWaitPage.TabIndex = 1007;
            this.connectWaitPage.Text = "Please wait while the wizard connects to the Collection Service.";
            this.connectWaitPage.AfterDisplay += new System.EventHandler(this.connectWaitPage_AfterDisplay);
            this.connectWaitPage.BeforeDisplay += new System.EventHandler(this.connectWaitPage_BeforeDisplay);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(63, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(358, 18);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // settingsPage
            // 
            this.settingsPage.Controls.Add(this.servicePortSpinner);
            this.settingsPage.Controls.Add(this.numericUpDown1);
            this.settingsPage.Controls.Add(this.label14);
            this.settingsPage.Controls.Add(this.label6);
            this.settingsPage.Controls.Add(this.label1);
            this.settingsPage.Controls.Add(this.serverName);
            this.settingsPage.Controls.Add(this.label11);
            this.settingsPage.Controls.Add(this.label12);
            this.settingsPage.Controls.Add(this.cboPort);
            this.settingsPage.Controls.Add(this.txtInstanceName);
            this.settingsPage.Controls.Add(this.label4);
            this.settingsPage.Controls.Add(this.label5);
            this.settingsPage.Description = "Change the desired settings and click next.";
            this.settingsPage.Location = new System.Drawing.Point(19, 73);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.NextPage = this.finishPage1;
            this.settingsPage.PreviousPage = this.connectWaitPage;
            this.settingsPage.Size = new System.Drawing.Size(485, 277);
            this.settingsPage.TabIndex = 1008;
            this.settingsPage.Text = "Settings";
            this.settingsPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.settingsPage_BeforeMoveNext);
            this.settingsPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.settingsPage_BeforeMoveBack);
            this.settingsPage.BeforeDisplay += new System.EventHandler(this.settingsPage_BeforeDisplay);
            // 
            // servicePortSpinner
            // 
            this.servicePortSpinner.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.collectionServiceSource, "ServicePort", true));
            this.servicePortSpinner.Location = new System.Drawing.Point(143, 31);
            this.servicePortSpinner.Maximum = new decimal(new int[] {
            65530,
            0,
            0,
            0});
            this.servicePortSpinner.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.servicePortSpinner.Name = "servicePortSpinner";
            this.servicePortSpinner.Size = new System.Drawing.Size(120, 20);
            this.servicePortSpinner.TabIndex = 45;
            this.servicePortSpinner.Value = new decimal(new int[] {
            5167,
            0,
            0,
            0});
            // 
            // collectionServiceSource
            // 
            this.collectionServiceSource.DataSource = typeof(Idera.SQLdm.Common.Configuration.CollectionServiceConfigurationMessage);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.collectionServiceSource, "HeartbeatIntervalSeconds", true));
            this.numericUpDown1.Increment = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(143, 164);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 44;
            this.numericUpDown1.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(74, 166);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 15);
            this.label14.TabIndex = 42;
            this.label14.Text = "Seconds:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(406, 15);
            this.label6.TabIndex = 41;
            this.label6.Text = "How often would you like to send heartbeats to the Management Service?";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 15);
            this.label1.TabIndex = 40;
            this.label1.Text = "Port:";
            // 
            // serverName
            // 
            this.serverName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.collectionServiceSource, "ManagementHost", true));
            this.serverName.Location = new System.Drawing.Point(143, 88);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(274, 20);
            this.serverName.TabIndex = 39;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(37, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(308, 15);
            this.label11.TabIndex = 38;
            this.label11.Text = "Which Management Service do you want to connect to?";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(74, 91);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 15);
            this.label12.TabIndex = 37;
            this.label12.Text = "Server:";
            // 
            // cboPort
            // 
            this.cboPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.collectionServiceSource, "ManagementPort", true));
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(143, 111);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(274, 21);
            this.cboPort.TabIndex = 36;
            this.cboPort.SelectionChangeCommitted += new System.EventHandler(this.cboPort_SelectionChangeCommitted);
            // 
            // txtInstanceName
            // 
            this.txtInstanceName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.collectionServiceSource, "InstanceName", true));
            this.txtInstanceName.Location = new System.Drawing.Point(143, 8);
            this.txtInstanceName.Name = "txtInstanceName";
            this.txtInstanceName.ReadOnly = true;
            this.txtInstanceName.Size = new System.Drawing.Size(274, 20);
            this.txtInstanceName.TabIndex = 34;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 15);
            this.label4.TabIndex = 32;
            this.label4.Text = "Instance Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 15);
            this.label5.TabIndex = 33;
            this.label5.Text = "Service Port:";
            // 
            // finishPage1
            // 
            this.finishPage1.AllowCancel = false;
            this.finishPage1.AllowMovePrevious = false;
            this.finishPage1.FinishText = "You have successfully completed the Configuration Wizard.";
            this.finishPage1.Location = new System.Drawing.Point(177, 66);
            this.finishPage1.Name = "finishPage1";
            this.finishPage1.PreviousPage = this.settingsPage;
            this.finishPage1.Size = new System.Drawing.Size(333, 284);
            this.finishPage1.TabIndex = 1006;
            this.finishPage1.Text = "Completing the SQLdm Collection Service Configuration Wizard";
            // 
            // CollectionServiceConfigWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 410);
            this.Controls.Add(this.wizard);
            this.Name = "CollectionServiceConfigWizard";
            this.Text = "IDERA SQLdm Collection Service";
            this.Load += new System.EventHandler(this.CollectionServiceConfigWizard_Load);
            this.wizard.ResumeLayout(false);
            this.introductionPage.ResumeLayout(false);
            this.selectServicePage.ResumeLayout(false);
            this.selectServicePage.PerformLayout();
            this.connectWaitPage.ResumeLayout(false);
            this.settingsPage.ResumeLayout(false);
            this.settingsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.servicePortSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionServiceSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizard;
        private Divelements.WizardFramework.IntroductionPage introductionPage;
        private Divelements.WizardFramework.WizardPage selectServicePage;
        private Divelements.WizardFramework.FinishPage finishPage1;
        private System.Windows.Forms.ComboBox portList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox machineName;
        private System.Windows.Forms.Label label3;
        private Divelements.WizardFramework.WizardPage connectWaitPage;
        private System.Windows.Forms.ProgressBar progressBar1;
        private Divelements.WizardFramework.WizardPage settingsPage;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.TextBox txtInstanceName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox serverName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.BindingSource collectionServiceSource;
        private Divelements.WizardFramework.InformationBox informationBox2;
        private System.Windows.Forms.NumericUpDown servicePortSpinner;
    }
}

