namespace Installer_form_application
{
    partial class RepositoryDetailsDM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryDetailsDM));
            this.buttonChange_Repo = new System.Windows.Forms.Button();
            this.checkBox_UseSQLAuth_Repo = new System.Windows.Forms.CheckBox();
            this.labelNote = new System.Windows.Forms.Label();
            this.textBoxDMDBName = new System.Windows.Forms.TextBox();
            this.textBoxDMInstance = new System.Windows.Forms.TextBox();
            this.labelDMDBName = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelHeading = new System.Windows.Forms.Label();
            this.buttonChange_Service = new System.Windows.Forms.Button();
            this.checkBox_UseSQLAuth_Service = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerForLicenseInfo = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerForCheckingCorruptedConfig = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // buttonChange_Repo
            // 
            this.buttonChange_Repo.BackColor = System.Drawing.Color.LightGray;
            this.buttonChange_Repo.Enabled = false;
            this.buttonChange_Repo.Location = new System.Drawing.Point(418, 189);
            this.buttonChange_Repo.Name = "buttonChange_Repo";
            this.buttonChange_Repo.Size = new System.Drawing.Size(75, 23);
            this.buttonChange_Repo.TabIndex = 6;
            this.buttonChange_Repo.Text = "Change";
            this.buttonChange_Repo.UseVisualStyleBackColor = false;
            this.buttonChange_Repo.Click += new System.EventHandler(this.buttonChange_Repo_Click);
            // 
            // checkBox_UseSQLAuth_Repo
            // 
            this.checkBox_UseSQLAuth_Repo.AutoSize = true;
            this.checkBox_UseSQLAuth_Repo.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_UseSQLAuth_Repo.Location = new System.Drawing.Point(190, 191);
            this.checkBox_UseSQLAuth_Repo.Name = "checkBox_UseSQLAuth_Repo";
            this.checkBox_UseSQLAuth_Repo.Size = new System.Drawing.Size(220, 17);
            this.checkBox_UseSQLAuth_Repo.TabIndex = 5;
            this.checkBox_UseSQLAuth_Repo.Text = "Use Microsoft SQL Server Authentication";
            this.checkBox_UseSQLAuth_Repo.UseVisualStyleBackColor = false;
            this.checkBox_UseSQLAuth_Repo.CheckedChanged += new System.EventHandler(this.checkBox_UseSQLAuth_Repo_CheckedChanged);
            // 
            // labelNote
            // 
            this.labelNote.BackColor = System.Drawing.Color.Transparent;
            this.labelNote.Location = new System.Drawing.Point(187, 152);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(355, 36);
            this.labelNote.TabIndex = 79;
            this.labelNote.Text = "Connection Credentials:  By default, the setup program uses the Windows credentia" +
    "ls you provided to create the repository.";
            // 
            // textBoxDMDBName
            // 
            this.textBoxDMDBName.Location = new System.Drawing.Point(302, 107);
            this.textBoxDMDBName.Name = "textBoxDMDBName";
            this.textBoxDMDBName.Size = new System.Drawing.Size(254, 20);
            this.textBoxDMDBName.TabIndex = 4;
            this.textBoxDMDBName.Text = "DiagnosticManagerRepository";
            // 
            // textBoxDMInstance
            // 
            this.textBoxDMInstance.Location = new System.Drawing.Point(302, 83);
            this.textBoxDMInstance.Name = "textBoxDMInstance";
            this.textBoxDMInstance.Size = new System.Drawing.Size(254, 20);
            this.textBoxDMInstance.TabIndex = 3;
            this.textBoxDMInstance.Text = "(local)";
            // 
            // labelDMDBName
            // 
            this.labelDMDBName.AutoSize = true;
            this.labelDMDBName.BackColor = System.Drawing.Color.Transparent;
            this.labelDMDBName.Location = new System.Drawing.Point(187, 110);
            this.labelDMDBName.Name = "labelDMDBName";
            this.labelDMDBName.Size = new System.Drawing.Size(87, 13);
            this.labelDMDBName.TabIndex = 76;
            this.labelDMDBName.Text = "Database Name:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(187, 86);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 13);
            this.label10.TabIndex = 75;
            this.label10.Text = "SQL Server Instance:";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Location = new System.Drawing.Point(187, 55);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(369, 18);
            this.labelDesc.TabIndex = 68;
            this.labelDesc.Text = "Create SQL Diagnostic Manager Repository";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(347, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 8;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(503, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.LightGray;
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(425, 420);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(72, 22);
            this.buttonNext.TabIndex = 7;
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
            this.labelHeading.Location = new System.Drawing.Point(186, 27);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(274, 21);
            this.labelHeading.TabIndex = 64;
            this.labelHeading.Text = "SQL Diagnostic Manager Repository";
            // 
            // buttonChange_Service
            // 
            this.buttonChange_Service.BackColor = System.Drawing.Color.LightGray;
            this.buttonChange_Service.Enabled = false;
            this.buttonChange_Service.Location = new System.Drawing.Point(416, 288);
            this.buttonChange_Service.Name = "buttonChange_Service";
            this.buttonChange_Service.Size = new System.Drawing.Size(75, 23);
            this.buttonChange_Service.TabIndex = 81;
            this.buttonChange_Service.Text = "Change";
            this.buttonChange_Service.UseVisualStyleBackColor = false;
            this.buttonChange_Service.Click += new System.EventHandler(this.buttonChange_Service_Click);
            // 
            // checkBox_UseSQLAuth_Service
            // 
            this.checkBox_UseSQLAuth_Service.AutoSize = true;
            this.checkBox_UseSQLAuth_Service.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_UseSQLAuth_Service.Location = new System.Drawing.Point(190, 292);
            this.checkBox_UseSQLAuth_Service.Name = "checkBox_UseSQLAuth_Service";
            this.checkBox_UseSQLAuth_Service.Size = new System.Drawing.Size(220, 17);
            this.checkBox_UseSQLAuth_Service.TabIndex = 80;
            this.checkBox_UseSQLAuth_Service.Text = "Use Microsoft SQL Server Authentication";
            this.checkBox_UseSQLAuth_Service.UseVisualStyleBackColor = false;
            this.checkBox_UseSQLAuth_Service.CheckedChanged += new System.EventHandler(this.checkBox_UseSQLAuth_Service_CheckedChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(187, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(355, 53);
            this.label1.TabIndex = 82;
            this.label1.Text = "If you want the SQL Diagnostic Manager Management Service to use SQL Server authe" +
    "ntication to connect to the SQLDM Repository, select the following check box:";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorkerForLicenseInfo
            // 
            this.backgroundWorkerForLicenseInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerForLicenseInfo_DoWork);
            this.backgroundWorkerForLicenseInfo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerForLicenseInfo_RunWorkerCompleted);
            // 
            // backgroundWorkerForCheckingCorruptedConfig
            // 
            this.backgroundWorkerForCheckingCorruptedConfig.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerForCheckingCorruptedConfig_DoWork);
            this.backgroundWorkerForCheckingCorruptedConfig.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerForCheckingCorruptedConfig_RunWorkerCompleted);
            // 
            // RepositoryDetailsDM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.buttonChange_Service);
            this.Controls.Add(this.checkBox_UseSQLAuth_Service);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonChange_Repo);
            this.Controls.Add(this.checkBox_UseSQLAuth_Repo);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.textBoxDMDBName);
            this.Controls.Add(this.textBoxDMInstance);
            this.Controls.Add(this.labelDMDBName);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepositoryDetailsDM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RepositoryDetailsDM_FormClosing);
            this.Load += new System.EventHandler(this.RepositoryDetailsDM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChange_Repo;
        private System.Windows.Forms.CheckBox checkBox_UseSQLAuth_Repo;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.TextBox textBoxDMDBName;
        private System.Windows.Forms.TextBox textBoxDMInstance;
        private System.Windows.Forms.Label labelDMDBName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Button buttonChange_Service;
        private System.Windows.Forms.CheckBox checkBox_UseSQLAuth_Service;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerForLicenseInfo;
        private System.ComponentModel.BackgroundWorker backgroundWorkerForCheckingCorruptedConfig;
    }
}