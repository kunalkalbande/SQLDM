namespace Installer_form_application
{
    partial class LicenseScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseScreen));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.existingLicenseKeyLabel = new System.Windows.Forms.Label();
            this.existingLicenseKeyTextbox = new System.Windows.Forms.TextBox();
            this.sqldmRepoLabel = new System.Windows.Forms.Label();
            this.totalLicensesLabel = new System.Windows.Forms.Label();
            this.monitoredInstancesLabel = new System.Windows.Forms.Label();
            this.sqldmRepoTextbox = new System.Windows.Forms.TextBox();
            this.totalLicensesTextbox = new System.Windows.Forms.TextBox();
            this.monitoredInstancesTextbox = new System.Windows.Forms.TextBox();
            this.copyExistingLicenseInfo = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.licenseKeyLabel = new System.Windows.Forms.Label();
            this.newLicenseKeyTextbox = new System.Windows.Forms.TextBox();
            this.back = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(192, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 21);
            this.label1.TabIndex = 34;
            this.label1.Text = "New License Key Required";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(193, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(369, 61);
            this.label2.TabIndex = 46;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // existingLicenseKeyLabel
            // 
            this.existingLicenseKeyLabel.AutoSize = true;
            this.existingLicenseKeyLabel.BackColor = System.Drawing.Color.Transparent;
            this.existingLicenseKeyLabel.Location = new System.Drawing.Point(193, 127);
            this.existingLicenseKeyLabel.Name = "existingLicenseKeyLabel";
            this.existingLicenseKeyLabel.Size = new System.Drawing.Size(113, 13);
            this.existingLicenseKeyLabel.TabIndex = 57;
            this.existingLicenseKeyLabel.Text = "Existing license key(s):";
            // 
            // existingLicenseKeyTextbox
            // 
            this.existingLicenseKeyTextbox.Enabled = false;
            this.existingLicenseKeyTextbox.Location = new System.Drawing.Point(315, 120);
            this.existingLicenseKeyTextbox.Name = "existingLicenseKeyTextbox";
            this.existingLicenseKeyTextbox.Size = new System.Drawing.Size(231, 20);
            this.existingLicenseKeyTextbox.TabIndex = 58;
            // 
            // sqldmRepoLabel
            // 
            this.sqldmRepoLabel.AutoSize = true;
            this.sqldmRepoLabel.BackColor = System.Drawing.Color.Transparent;
            this.sqldmRepoLabel.Location = new System.Drawing.Point(193, 157);
            this.sqldmRepoLabel.Name = "sqldmRepoLabel";
            this.sqldmRepoLabel.Size = new System.Drawing.Size(98, 13);
            this.sqldmRepoLabel.TabIndex = 59;
            this.sqldmRepoLabel.Text = "SQLdm Repository:";
            // 
            // totalLicensesLabel
            // 
            this.totalLicensesLabel.AutoSize = true;
            this.totalLicensesLabel.BackColor = System.Drawing.Color.Transparent;
            this.totalLicensesLabel.Location = new System.Drawing.Point(193, 190);
            this.totalLicensesLabel.Name = "totalLicensesLabel";
            this.totalLicensesLabel.Size = new System.Drawing.Size(79, 13);
            this.totalLicensesLabel.TabIndex = 60;
            this.totalLicensesLabel.Text = "Total Licenses:";
            // 
            // monitoredInstancesLabel
            // 
            this.monitoredInstancesLabel.AutoSize = true;
            this.monitoredInstancesLabel.BackColor = System.Drawing.Color.Transparent;
            this.monitoredInstancesLabel.Location = new System.Drawing.Point(193, 225);
            this.monitoredInstancesLabel.Name = "monitoredInstancesLabel";
            this.monitoredInstancesLabel.Size = new System.Drawing.Size(106, 13);
            this.monitoredInstancesLabel.TabIndex = 61;
            this.monitoredInstancesLabel.Text = "Monitored Instances:";
            // 
            // sqldmRepoTextbox
            // 
            this.sqldmRepoTextbox.Enabled = false;
            this.sqldmRepoTextbox.Location = new System.Drawing.Point(315, 157);
            this.sqldmRepoTextbox.Name = "sqldmRepoTextbox";
            this.sqldmRepoTextbox.Size = new System.Drawing.Size(231, 20);
            this.sqldmRepoTextbox.TabIndex = 62;
            // 
            // totalLicensesTextbox
            // 
            this.totalLicensesTextbox.Enabled = false;
            this.totalLicensesTextbox.Location = new System.Drawing.Point(315, 190);
            this.totalLicensesTextbox.Name = "totalLicensesTextbox";
            this.totalLicensesTextbox.Size = new System.Drawing.Size(31, 20);
            this.totalLicensesTextbox.TabIndex = 63;
            // 
            // monitoredInstancesTextbox
            // 
            this.monitoredInstancesTextbox.Enabled = false;
            this.monitoredInstancesTextbox.Location = new System.Drawing.Point(315, 225);
            this.monitoredInstancesTextbox.Name = "monitoredInstancesTextbox";
            this.monitoredInstancesTextbox.Size = new System.Drawing.Size(31, 20);
            this.monitoredInstancesTextbox.TabIndex = 64;
            // 
            // copyExistingLicenseInfo
            // 
            this.copyExistingLicenseInfo.Location = new System.Drawing.Point(362, 225);
            this.copyExistingLicenseInfo.Name = "copyExistingLicenseInfo";
            this.copyExistingLicenseInfo.Size = new System.Drawing.Size(184, 23);
            this.copyExistingLicenseInfo.TabIndex = 65;
            this.copyExistingLicenseInfo.Text = "Copy Existing License Information";
            this.copyExistingLicenseInfo.UseVisualStyleBackColor = true;
            this.copyExistingLicenseInfo.Click += new System.EventHandler(this.copyExistingLicenseInfo_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(193, 263);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(283, 21);
            this.label7.TabIndex = 66;
            this.label7.Text = "Enter the new License key to proceed";
            // 
            // licenseKeyLabel
            // 
            this.licenseKeyLabel.AutoSize = true;
            this.licenseKeyLabel.BackColor = System.Drawing.Color.Transparent;
            this.licenseKeyLabel.Location = new System.Drawing.Point(193, 292);
            this.licenseKeyLabel.Name = "licenseKeyLabel";
            this.licenseKeyLabel.Size = new System.Drawing.Size(68, 13);
            this.licenseKeyLabel.TabIndex = 67;
            this.licenseKeyLabel.Text = "License Key:";
            // 
            // newLicenseKeyTextbox
            // 
            this.newLicenseKeyTextbox.Location = new System.Drawing.Point(315, 287);
            this.newLicenseKeyTextbox.Name = "newLicenseKeyTextbox";
            this.newLicenseKeyTextbox.Size = new System.Drawing.Size(231, 20);
            this.newLicenseKeyTextbox.TabIndex = 68;
            // 
            // back
            // 
            this.back.Location = new System.Drawing.Point(340, 419);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 23);
            this.back.TabIndex = 69;
            this.back.Text = "Back";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(421, 419);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 23);
            this.next.TabIndex = 70;
            this.next.Text = "Next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(502, 419);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 71;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // LicenseScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Installer_form_application.Properties.Resources.Main_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(593, 454);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.next);
            this.Controls.Add(this.back);
            this.Controls.Add(this.newLicenseKeyTextbox);
            this.Controls.Add(this.licenseKeyLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.copyExistingLicenseInfo);
            this.Controls.Add(this.monitoredInstancesTextbox);
            this.Controls.Add(this.totalLicensesTextbox);
            this.Controls.Add(this.sqldmRepoTextbox);
            this.Controls.Add(this.monitoredInstancesLabel);
            this.Controls.Add(this.totalLicensesLabel);
            this.Controls.Add(this.sqldmRepoLabel);
            this.Controls.Add(this.existingLicenseKeyTextbox);
            this.Controls.Add(this.existingLicenseKeyLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IDERA SQL Diagnostic Manager Setup";
            this.Load += new System.EventHandler(this.LicenseScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label existingLicenseKeyLabel;
        private System.Windows.Forms.TextBox existingLicenseKeyTextbox;
        private System.Windows.Forms.Label sqldmRepoLabel;
        private System.Windows.Forms.Label totalLicensesLabel;
        private System.Windows.Forms.Label monitoredInstancesLabel;
        private System.Windows.Forms.TextBox sqldmRepoTextbox;
        private System.Windows.Forms.TextBox totalLicensesTextbox;
        private System.Windows.Forms.TextBox monitoredInstancesTextbox;
        private System.Windows.Forms.Button copyExistingLicenseInfo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label licenseKeyLabel;
        private System.Windows.Forms.TextBox newLicenseKeyTextbox;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button cancel;
    }
}