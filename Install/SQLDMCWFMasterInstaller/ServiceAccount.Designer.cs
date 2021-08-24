namespace Installer_form_application
{
    partial class ServiceAccount
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceAccount));
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.labelDashboardCredentials = new System.Windows.Forms.Label();
            this.labelSQLdmPassword = new System.Windows.Forms.Label();
            this.textBoxSPSPassword = new System.Windows.Forms.TextBox();
            this.labelSQLdmUsername = new System.Windows.Forms.Label();
            this.textBoxSPSUserName = new System.Windows.Forms.TextBox();
            this.checkBoxSameCreds = new System.Windows.Forms.CheckBox();
            this.labelIDSUsername = new System.Windows.Forms.Label();
            this.textBoxIDSUsername = new System.Windows.Forms.TextBox();
            this.labelIDSPassword = new System.Windows.Forms.Label();
            this.textBoxIDSPassword = new System.Windows.Forms.TextBox();
            this.labelSQLDMCredential = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
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
            this.buttonBack.TabIndex = 6;
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
            this.buttonCancel.TabIndex = 7;
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
            this.buttonNext.TabIndex = 5;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(190, 26);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(128, 21);
            this.labelHeading.TabIndex = 49;
            this.labelHeading.Text = "Service Account";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(191, 58);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(380, 61);
            this.labelDesc.TabIndex = 50;
            this.labelDesc.Text = resources.GetString("labelDesc.Text");
            // 
            // labelDashboardCredentials
            // 
            this.labelDashboardCredentials.AutoSize = true;
            this.labelDashboardCredentials.BackColor = System.Drawing.Color.Transparent;
            this.labelDashboardCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelDashboardCredentials.Location = new System.Drawing.Point(191, 244);
            this.labelDashboardCredentials.Name = "labelDashboardCredentials";
            this.labelDashboardCredentials.Size = new System.Drawing.Size(183, 13);
            this.labelDashboardCredentials.TabIndex = 55;
            this.labelDashboardCredentials.Text = "Service Account for Idera Dashboard\r\n";
            this.labelDashboardCredentials.Visible = false;
            // 
            // labelSQLdmPassword
            // 
            this.labelSQLdmPassword.AutoSize = true;
            this.labelSQLdmPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLdmPassword.Location = new System.Drawing.Point(191, 199);
            this.labelSQLdmPassword.Name = "labelSQLdmPassword";
            this.labelSQLdmPassword.Size = new System.Drawing.Size(56, 13);
            this.labelSQLdmPassword.TabIndex = 52;
            this.labelSQLdmPassword.Text = "Password:";
            // 
            // textBoxSPSPassword
            // 
            this.textBoxSPSPassword.Location = new System.Drawing.Point(304, 199);
            this.textBoxSPSPassword.Name = "textBoxSPSPassword";
            this.textBoxSPSPassword.PasswordChar = '*';
            this.textBoxSPSPassword.Size = new System.Drawing.Size(255, 20);
            this.textBoxSPSPassword.TabIndex = 2;
            // 
            // labelSQLdmUsername
            // 
            this.labelSQLdmUsername.AutoSize = true;
            this.labelSQLdmUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLdmUsername.Location = new System.Drawing.Point(191, 176);
            this.labelSQLdmUsername.Name = "labelSQLdmUsername";
            this.labelSQLdmUsername.Size = new System.Drawing.Size(107, 13);
            this.labelSQLdmUsername.TabIndex = 51;
            this.labelSQLdmUsername.Text = "Domain \\ UserName:";
            // 
            // textBoxSPSUserName
            // 
            this.textBoxSPSUserName.Location = new System.Drawing.Point(304, 173);
            this.textBoxSPSUserName.Name = "textBoxSPSUserName";
            this.textBoxSPSUserName.Size = new System.Drawing.Size(255, 20);
            this.textBoxSPSUserName.TabIndex = 1;
            // 
            // checkBoxSameCreds
            // 
            this.checkBoxSameCreds.AutoSize = true;
            this.checkBoxSameCreds.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSameCreds.Checked = true;
            this.checkBoxSameCreds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSameCreds.Location = new System.Drawing.Point(194, 122);
            this.checkBoxSameCreds.Name = "checkBoxSameCreds";
            this.checkBoxSameCreds.Size = new System.Drawing.Size(304, 17);
            this.checkBoxSameCreds.TabIndex = 8;
            this.checkBoxSameCreds.Text = "Use the same account for IDERA Dashboard and SQL DM";
            this.checkBoxSameCreds.UseVisualStyleBackColor = false;
            this.checkBoxSameCreds.CheckedChanged += new System.EventHandler(this.checkBoxSameCreds_CheckedChanged);
            // 
            // labelIDSUsername
            // 
            this.labelIDSUsername.AutoSize = true;
            this.labelIDSUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSUsername.Location = new System.Drawing.Point(191, 274);
            this.labelIDSUsername.Name = "labelIDSUsername";
            this.labelIDSUsername.Size = new System.Drawing.Size(107, 13);
            this.labelIDSUsername.TabIndex = 58;
            this.labelIDSUsername.Text = "Domain \\ UserName:";
            this.labelIDSUsername.Visible = false;
            // 
            // textBoxIDSUsername
            // 
            this.textBoxIDSUsername.Location = new System.Drawing.Point(304, 271);
            this.textBoxIDSUsername.Name = "textBoxIDSUsername";
            this.textBoxIDSUsername.Size = new System.Drawing.Size(255, 20);
            this.textBoxIDSUsername.TabIndex = 3;
            this.textBoxIDSUsername.Visible = false;
            // 
            // labelIDSPassword
            // 
            this.labelIDSPassword.AutoSize = true;
            this.labelIDSPassword.BackColor = System.Drawing.Color.Transparent;
            this.labelIDSPassword.Location = new System.Drawing.Point(191, 300);
            this.labelIDSPassword.Name = "labelIDSPassword";
            this.labelIDSPassword.Size = new System.Drawing.Size(56, 13);
            this.labelIDSPassword.TabIndex = 60;
            this.labelIDSPassword.Text = "Password:";
            this.labelIDSPassword.Visible = false;
            // 
            // textBoxIDSPassword
            // 
            this.textBoxIDSPassword.Location = new System.Drawing.Point(304, 297);
            this.textBoxIDSPassword.Name = "textBoxIDSPassword";
            this.textBoxIDSPassword.PasswordChar = '*';
            this.textBoxIDSPassword.Size = new System.Drawing.Size(255, 20);
            this.textBoxIDSPassword.TabIndex = 4;
            this.textBoxIDSPassword.Visible = false;
            // 
            // labelSQLDMCredential
            // 
            this.labelSQLDMCredential.AutoSize = true;
            this.labelSQLDMCredential.BackColor = System.Drawing.Color.Transparent;
            this.labelSQLDMCredential.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelSQLDMCredential.Location = new System.Drawing.Point(191, 148);
            this.labelSQLDMCredential.Name = "labelSQLDMCredential";
            this.labelSQLDMCredential.Size = new System.Drawing.Size(142, 13);
            this.labelSQLDMCredential.TabIndex = 61;
            this.labelSQLDMCredential.Text = "Service Account for SQLDM";
            this.labelSQLDMCredential.Visible = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // ServiceAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.labelSQLDMCredential);
            this.Controls.Add(this.textBoxIDSPassword);
            this.Controls.Add(this.labelIDSPassword);
            this.Controls.Add(this.textBoxIDSUsername);
            this.Controls.Add(this.labelIDSUsername);
            this.Controls.Add(this.checkBoxSameCreds);
            this.Controls.Add(this.labelDashboardCredentials);
            this.Controls.Add(this.labelSQLdmPassword);
            this.Controls.Add(this.textBoxSPSPassword);
            this.Controls.Add(this.labelSQLdmUsername);
            this.Controls.Add(this.textBoxSPSUserName);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServiceAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServiceAccount_FormClosing);
            this.Load += new System.EventHandler(this.ServiceAccount_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Label labelDashboardCredentials;
        private System.Windows.Forms.Label labelSQLdmPassword;
        private System.Windows.Forms.TextBox textBoxSPSPassword;
        private System.Windows.Forms.Label labelSQLdmUsername;
        private System.Windows.Forms.TextBox textBoxSPSUserName;
        private System.Windows.Forms.CheckBox checkBoxSameCreds;
        private System.Windows.Forms.Label labelIDSUsername;
        private System.Windows.Forms.TextBox textBoxIDSUsername;
        private System.Windows.Forms.Label labelIDSPassword;
        private System.Windows.Forms.TextBox textBoxIDSPassword;
        private System.Windows.Forms.Label labelSQLDMCredential;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}