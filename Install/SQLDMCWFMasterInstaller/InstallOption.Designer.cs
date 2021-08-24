namespace Installer_form_application
{
    partial class InstallOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallOption));
            this.labelHeading = new System.Windows.Forms.Label();
            this.labelDesc = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.checkBoxDMServicesAndRepository = new System.Windows.Forms.CheckBox();
            this.checkBoxDMConsoleOnly = new System.Windows.Forms.CheckBox();
            this.backgroundWorkerForPopulatingData = new System.ComponentModel.BackgroundWorker();
            this.checkBoxInstallDashbaord = new System.Windows.Forms.CheckBox();
            this.labelForDashboardReInstall = new System.Windows.Forms.Label();
            this.labelForSqlDmReInstall = new System.Windows.Forms.Label();
            this.LabelForDMConsoleOnly = new System.Windows.Forms.Label();
            this.LabelForDMServicesAndRepository = new System.Windows.Forms.Label();
            this.LabelForInstallDashbaord = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelHeading
            // 
            this.labelHeading.AutoSize = true;
            this.labelHeading.BackColor = System.Drawing.Color.Transparent;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelHeading.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelHeading.Location = new System.Drawing.Point(191, 18);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(287, 21);
            this.labelHeading.TabIndex = 30;
            this.labelHeading.Text = "SQL Diagnostic Manager Components";
            // 
            // labelDesc
            // 
            this.labelDesc.BackColor = System.Drawing.Color.Transparent;
            this.labelDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelDesc.Location = new System.Drawing.Point(193, 48);
            this.labelDesc.Name = "labelDesc";
            this.labelDesc.Size = new System.Drawing.Size(379, 28);
            this.labelDesc.TabIndex = 31;
            this.labelDesc.Text = "Please select the SQL Diagnostic Manager components that you wish to install/upgr" +
    "ade.";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.LightGray;
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonBack.Location = new System.Drawing.Point(347, 420);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(72, 22);
            this.buttonBack.TabIndex = 7;
            this.buttonBack.Text = "Back\r\n";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.LightGray;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonCancel.Location = new System.Drawing.Point(503, 421);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 22);
            this.buttonCancel.TabIndex = 8;
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
            this.buttonNext.TabIndex = 6;
            this.buttonNext.Text = "Next";
            this.buttonNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // checkBoxDMServicesAndRepository
            // 
            this.checkBoxDMServicesAndRepository.AutoSize = true;
            this.checkBoxDMServicesAndRepository.BackColor = System.Drawing.Color.White;
            this.checkBoxDMServicesAndRepository.Checked = true;
            this.checkBoxDMServicesAndRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDMServicesAndRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.checkBoxDMServicesAndRepository.Location = new System.Drawing.Point(195, 148);
            this.checkBoxDMServicesAndRepository.Name = "checkBoxDMServicesAndRepository";
            this.checkBoxDMServicesAndRepository.Size = new System.Drawing.Size(309, 17);
            this.checkBoxDMServicesAndRepository.TabIndex = 2;
            this.checkBoxDMServicesAndRepository.Text = "SQL Diagnostic Manager Services and Repository";
            this.checkBoxDMServicesAndRepository.UseVisualStyleBackColor = false;
            this.checkBoxDMServicesAndRepository.CheckedChanged += new System.EventHandler(this.checkChangedForInstallingComponents);
            // 
            // checkBoxDMConsoleOnly
            // 
            this.checkBoxDMConsoleOnly.AutoSize = true;
            this.checkBoxDMConsoleOnly.BackColor = System.Drawing.Color.White;
            this.checkBoxDMConsoleOnly.Checked = true;
            this.checkBoxDMConsoleOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDMConsoleOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.checkBoxDMConsoleOnly.Location = new System.Drawing.Point(196, 84);
            this.checkBoxDMConsoleOnly.Name = "checkBoxDMConsoleOnly";
            this.checkBoxDMConsoleOnly.Size = new System.Drawing.Size(271, 17);
            this.checkBoxDMConsoleOnly.TabIndex = 1;
            this.checkBoxDMConsoleOnly.Text = "SQL Diagnostic Manager Windows Console";
            this.checkBoxDMConsoleOnly.UseVisualStyleBackColor = false;
            this.checkBoxDMConsoleOnly.CheckedChanged += new System.EventHandler(this.checkChangedForInstallingComponents);
            // 
            // backgroundWorkerForPopulatingData
            // 
            this.backgroundWorkerForPopulatingData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerForPopulatingData_DoWork);
            this.backgroundWorkerForPopulatingData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerForPopulatingData_RunWorkerCompleted);
            // 
            // checkBoxInstallDashbaord
            // 
            this.checkBoxInstallDashbaord.AutoSize = true;
            this.checkBoxInstallDashbaord.BackColor = System.Drawing.Color.White;
            this.checkBoxInstallDashbaord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.checkBoxInstallDashbaord.Location = new System.Drawing.Point(195, 262);
            this.checkBoxInstallDashbaord.Name = "checkBoxInstallDashbaord";
            this.checkBoxInstallDashbaord.Size = new System.Drawing.Size(216, 17);
            this.checkBoxInstallDashbaord.TabIndex = 3;
            this.checkBoxInstallDashbaord.Text = "IDERA Dashboard (Web Console)";
            this.checkBoxInstallDashbaord.UseVisualStyleBackColor = false;
            this.checkBoxInstallDashbaord.CheckedChanged += new System.EventHandler(this.checkChangedForInstallingComponents);
            // 
            // labelForDashboardReInstall
            // 
            this.labelForDashboardReInstall.AutoSize = true;
            this.labelForDashboardReInstall.BackColor = System.Drawing.Color.Transparent;
            this.labelForDashboardReInstall.ForeColor = System.Drawing.Color.Red;
            this.labelForDashboardReInstall.Location = new System.Drawing.Point(210, 340);
            this.labelForDashboardReInstall.Name = "labelForDashboardReInstall";
            this.labelForDashboardReInstall.Size = new System.Drawing.Size(348, 39);
            this.labelForDashboardReInstall.TabIndex = 32;
            this.labelForDashboardReInstall.Text = "The installed version of dashboard on the local machine is incompatible. \r\nPlease" +
    " upgrade the dashboard to any version greater than 2.1.0, \r\nto proceed with loca" +
    "l registration";
            this.labelForDashboardReInstall.Visible = false;
            // 
            // labelForSqlDmReInstall
            // 
            this.labelForSqlDmReInstall.AutoSize = true;
            this.labelForSqlDmReInstall.BackColor = System.Drawing.Color.Transparent;
            this.labelForSqlDmReInstall.ForeColor = System.Drawing.Color.Red;
            this.labelForSqlDmReInstall.Location = new System.Drawing.Point(214, 231);
            this.labelForSqlDmReInstall.Name = "labelForSqlDmReInstall";
            this.labelForSqlDmReInstall.Size = new System.Drawing.Size(350, 26);
            this.labelForSqlDmReInstall.TabIndex = 33;
            this.labelForSqlDmReInstall.Text = "Previous version (2.3.1.11) of SQLdm is detected and it will be upgraded \r\nusing " +
    "this installer.";
            this.labelForSqlDmReInstall.Visible = false;
            // 
            // LabelForDMConsoleOnly
            // 
            this.LabelForDMConsoleOnly.BackColor = System.Drawing.Color.Transparent;
            this.LabelForDMConsoleOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.LabelForDMConsoleOnly.Location = new System.Drawing.Point(213, 102);
            this.LabelForDMConsoleOnly.Name = "LabelForDMConsoleOnly";
            this.LabelForDMConsoleOnly.Size = new System.Drawing.Size(351, 47);
            this.LabelForDMConsoleOnly.TabIndex = 34;
            this.LabelForDMConsoleOnly.Text = "The SQL Diagnostic Manager Windows Console allows you to view/setup/configure the" +
    " information that has been collected by SQL Diagnostic Manager.";
            // 
            // LabelForDMServicesAndRepository
            // 
            this.LabelForDMServicesAndRepository.BackColor = System.Drawing.Color.Transparent;
            this.LabelForDMServicesAndRepository.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.LabelForDMServicesAndRepository.Location = new System.Drawing.Point(213, 163);
            this.LabelForDMServicesAndRepository.Name = "LabelForDMServicesAndRepository";
            this.LabelForDMServicesAndRepository.Size = new System.Drawing.Size(349, 66);
            this.LabelForDMServicesAndRepository.TabIndex = 35;
            this.LabelForDMServicesAndRepository.Text = resources.GetString("LabelForDMServicesAndRepository.Text");
            // 
            // LabelForInstallDashbaord
            // 
            this.LabelForInstallDashbaord.BackColor = System.Drawing.Color.Transparent;
            this.LabelForInstallDashbaord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.LabelForInstallDashbaord.Location = new System.Drawing.Point(213, 280);
            this.LabelForInstallDashbaord.Name = "LabelForInstallDashbaord";
            this.LabelForInstallDashbaord.Size = new System.Drawing.Size(325, 60);
            this.LabelForInstallDashbaord.TabIndex = 36;
            this.LabelForInstallDashbaord.Text = "Similar to the SQL Diagnostic Manager Windows Console, the IDERA Dashboard is the" +
    " web portal that allows you to view/setup/configure the information that has bee" +
    "n collected by SQL Diagnostic Manager.";
            // 
            // InstallOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.LabelForInstallDashbaord);
            this.Controls.Add(this.LabelForDMServicesAndRepository);
            this.Controls.Add(this.LabelForDMConsoleOnly);
            this.Controls.Add(this.labelForSqlDmReInstall);
            this.Controls.Add(this.labelForDashboardReInstall);
            this.Controls.Add(this.checkBoxDMServicesAndRepository);
            this.Controls.Add(this.checkBoxDMConsoleOnly);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelDesc);
            this.Controls.Add(this.labelHeading);
            this.Controls.Add(this.checkBoxInstallDashbaord);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallOption_FormClosing);
            this.Load += new System.EventHandler(this.InstallOption_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelDesc;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.CheckBox checkBoxDMServicesAndRepository;
        private System.Windows.Forms.CheckBox checkBoxDMConsoleOnly;
        private System.ComponentModel.BackgroundWorker backgroundWorkerForPopulatingData;
        private System.Windows.Forms.CheckBox checkBoxInstallDashbaord;
        private System.Windows.Forms.Label labelForDashboardReInstall;
        private System.Windows.Forms.Label labelForSqlDmReInstall;
        private System.Windows.Forms.Label LabelForDMConsoleOnly;
        private System.Windows.Forms.Label LabelForDMServicesAndRepository;
        private System.Windows.Forms.Label LabelForInstallDashbaord;
    }
}