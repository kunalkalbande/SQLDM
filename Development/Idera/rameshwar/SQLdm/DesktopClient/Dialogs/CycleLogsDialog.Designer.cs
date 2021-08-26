namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class CycleLogsDialog
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
            this.sqlServerCheckBox = new System.Windows.Forms.CheckBox();
            this.selectGroupBox = new System.Windows.Forms.GroupBox();
            this.sqlAgentCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.selectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // sqlServerCheckBox
            // 
            this.sqlServerCheckBox.AutoSize = true;
            this.sqlServerCheckBox.Checked = true;
            this.sqlServerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sqlServerCheckBox.Location = new System.Drawing.Point(19, 23);
            this.sqlServerCheckBox.Name = "sqlServerCheckBox";
            this.sqlServerCheckBox.Size = new System.Drawing.Size(81, 17);
            this.sqlServerCheckBox.TabIndex = 1;
            this.sqlServerCheckBox.Text = "SQL Server";
            this.sqlServerCheckBox.UseVisualStyleBackColor = true;
            this.sqlServerCheckBox.CheckedChanged += new System.EventHandler(this.sqlServerCheckBox_CheckedChanged);
            // 
            // selectGroupBox
            // 
            this.selectGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectGroupBox.Controls.Add(this.sqlAgentCheckBox);
            this.selectGroupBox.Controls.Add(this.sqlServerCheckBox);
            this.selectGroupBox.Location = new System.Drawing.Point(12, 11);
            this.selectGroupBox.Name = "selectGroupBox";
            this.selectGroupBox.Size = new System.Drawing.Size(207, 81);
            this.selectGroupBox.TabIndex = 0;
            this.selectGroupBox.TabStop = false;
            this.selectGroupBox.Text = "Select Logs to Cycle";
            // 
            // sqlAgentCheckBox
            // 
            this.sqlAgentCheckBox.AutoSize = true;
            this.sqlAgentCheckBox.Location = new System.Drawing.Point(19, 48);
            this.sqlAgentCheckBox.Name = "sqlAgentCheckBox";
            this.sqlAgentCheckBox.Size = new System.Drawing.Size(112, 17);
            this.sqlAgentCheckBox.TabIndex = 2;
            this.sqlAgentCheckBox.Text = "SQL Server Agent";
            this.sqlAgentCheckBox.UseVisualStyleBackColor = true;
            this.sqlAgentCheckBox.CheckedChanged += new System.EventHandler(this.sqlAgentCheckBox_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(63, 108);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(144, 108);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // CycleLogsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(231, 144);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.selectGroupBox);
            this.HelpButton = true;
            this.Name = "CycleLogsDialog";
            this.Text = " Cycle Logs";
            this.selectGroupBox.ResumeLayout(false);
            this.selectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox sqlServerCheckBox;
        private System.Windows.Forms.GroupBox selectGroupBox;
        private System.Windows.Forms.CheckBox sqlAgentCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}