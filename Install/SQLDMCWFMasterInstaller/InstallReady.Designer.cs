namespace Installer_form_application
{
    partial class InstallReady
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallReady));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.labeSPSAccount = new System.Windows.Forms.Label();
            this.labelIDSAccount = new System.Windows.Forms.Label();
            this.labelSetUpInfo = new System.Windows.Forms.Label();
            this.labelDashboardInstructions = new System.Windows.Forms.Label();
            this.labelInstallOrReview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(348, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 2;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(504, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(426, 420);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 1;
            this.buttonNext.Text = "Install";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(193, 28);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(122, 21);
            this.labelHeading.TabIndex = 71;
            this.labelHeading.Text = "Ready to Install";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(194, 63);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(382, 28);
            this.labelDesc.TabIndex = 72;
            this.labelDesc.Text = "Ready to install IDERA SQL Diagnostic Manager and IDERA Dashboard.";
            // 
            // labeSPSAccount
            // 
            this.labeSPSAccount.AutoSize = true;
            this.labeSPSAccount.BackColor = System.Drawing.Color.Transparent;
            this.labeSPSAccount.Location = new System.Drawing.Point(194, 194);
            this.labeSPSAccount.Name = "labeSPSAccount";
            this.labeSPSAccount.Size = new System.Drawing.Size(133, 13);
            this.labeSPSAccount.TabIndex = 74;
            this.labeSPSAccount.Text = "SQL DM Service Account:";
            // 
            // labelIDSAccount
            // 
            this.labelIDSAccount.AutoSize = true;
            this.labelIDSAccount.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSAccount.Location = new System.Drawing.Point(194, 137);
            this.labelIDSAccount.Name = "labelIDSAccount";
            this.labelIDSAccount.Size = new System.Drawing.Size(180, 13);
            this.labelIDSAccount.TabIndex = 73;
            this.labelIDSAccount.Text = "IDERA Dashboard Service Account:";
            // 
            // labelSetUpInfo
            // 
            this.labelSetUpInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelSetUpInfo.Location = new System.Drawing.Point(194, 95);
            this.labelSetUpInfo.Name = "labelSetUpInfo";
            this.labelSetUpInfo.Size = new System.Drawing.Size(395, 34);
            this.labelSetUpInfo.TabIndex = 75;
            this.labelSetUpInfo.Text = "Setup will grant these accounts access to SQL Diagnostic Manager and the IDERA Da" +
    "shboard:";
            // 
            // labelDashboardInstructions
            // 
            this.labelDashboardInstructions.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardInstructions.Location = new System.Drawing.Point(194, 150);
            this.labelDashboardInstructions.Name = "labelDashboardInstructions";
            this.labelDashboardInstructions.Size = new System.Drawing.Size(395, 35);
            this.labelDashboardInstructions.TabIndex = 76;
            this.labelDashboardInstructions.Text = "Use this account to log into the Idera Dashboard and grant other users access fro" +
    "m the Idera Dashboard Administration tab.\r\n";
            // 
            // labelInstallOrReview
            // 
            this.labelInstallOrReview.BackColor = System.Drawing.Color.Transparent;
            this.labelInstallOrReview.Location = new System.Drawing.Point(194, 226);
            this.labelInstallOrReview.Name = "labelInstallOrReview";
            this.labelInstallOrReview.Size = new System.Drawing.Size(395, 35);
            this.labelInstallOrReview.TabIndex = 77;
            this.labelInstallOrReview.Text = "Click Install to begin the installation process. Click Back to review or change y" +
    "our installation settings. Click Cancel to exit the wizard.";
            // 
            // InstallReady
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.labelInstallOrReview);
            this.Controls.Add(this.labelDashboardInstructions);
            this.Controls.Add(this.labelSetUpInfo);
            this.Controls.Add(this.labeSPSAccount);
            this.Controls.Add(this.labelIDSAccount);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallReady";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallReady_FormClosing);
            this.Load += new System.EventHandler(this.InstallReady_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labeSPSAccount;
        private System.Windows.Forms.Label labelIDSAccount;
        private System.Windows.Forms.Label labelSetUpInfo;
        private System.Windows.Forms.Label labelDashboardInstructions;
        private System.Windows.Forms.Label labelInstallOrReview;
    }
}