namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class StateSelectionDialog
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkInfo = new System.Windows.Forms.CheckBox();
            this.informationBox2 = new Divelements.WizardFramework.InformationBox();
            this.chkOK = new System.Windows.Forms.CheckBox();
            this.chkCritical = new System.Windows.Forms.CheckBox();
            this.chkWarning = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(193, 151);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(112, 151);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkInfo);
            this.panel1.Controls.Add(this.informationBox2);
            this.panel1.Controls.Add(this.chkOK);
            this.panel1.Controls.Add(this.chkCritical);
            this.panel1.Controls.Add(this.chkWarning);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(277, 145);
            this.panel1.TabIndex = 9;
            // 
            // chkInfo
            // 
            this.chkInfo.AutoSize = true;
            this.chkInfo.Location = new System.Drawing.Point(50, 77);
            this.chkInfo.Name = "chkInfo";
            this.chkInfo.Size = new System.Drawing.Size(86, 17);
            this.chkInfo.TabIndex = 19;
            this.chkInfo.Text = "Informational";
            this.chkInfo.UseVisualStyleBackColor = true;
            // 
            // informationBox2
            // 
            this.informationBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox2.Location = new System.Drawing.Point(5, 5);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(267, 42);
            this.informationBox2.TabIndex = 18;
            this.informationBox2.Text = "The OK and Informational options have an effect only if the rule is configured to" +
                " respond when metric severity has changed.";
            // 
            // chkOK
            // 
            this.chkOK.AutoSize = true;
            this.chkOK.Location = new System.Drawing.Point(50, 56);
            this.chkOK.Name = "chkOK";
            this.chkOK.Size = new System.Drawing.Size(41, 17);
            this.chkOK.TabIndex = 3;
            this.chkOK.Text = "OK";
            this.chkOK.UseVisualStyleBackColor = true;
            this.chkOK.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkCritical
            // 
            this.chkCritical.AutoSize = true;
            this.chkCritical.Checked = true;
            this.chkCritical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCritical.Location = new System.Drawing.Point(50, 119);
            this.chkCritical.Name = "chkCritical";
            this.chkCritical.Size = new System.Drawing.Size(57, 17);
            this.chkCritical.TabIndex = 2;
            this.chkCritical.Text = "Critical";
            this.chkCritical.UseVisualStyleBackColor = true;
            this.chkCritical.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkWarning
            // 
            this.chkWarning.AutoSize = true;
            this.chkWarning.Location = new System.Drawing.Point(50, 98);
            this.chkWarning.Name = "chkWarning";
            this.chkWarning.Size = new System.Drawing.Size(66, 17);
            this.chkWarning.TabIndex = 1;
            this.chkWarning.Text = "Warning";
            this.chkWarning.UseVisualStyleBackColor = true;
            this.chkWarning.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // StateSelectionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(277, 186);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StateSelectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Severity";
            this.Load += new System.EventHandler(this.StateSelectionDialog_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkCritical;
        private System.Windows.Forms.CheckBox chkWarning;
        private System.Windows.Forms.CheckBox chkOK;
        private Divelements.WizardFramework.InformationBox informationBox2;
        private System.Windows.Forms.CheckBox chkInfo;
    }
}