namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class EditConfigurationValueDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.informationBox = new Divelements.WizardFramework.InformationBox();
            this.newValueDescriptionLabel = new System.Windows.Forms.Label();
            this.newValueTextBox = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(251, 89);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(170, 89);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // informationBox
            // 
            this.informationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox.Location = new System.Drawing.Point(12, 12);
            this.informationBox.Name = "informationBox";
            this.informationBox.Size = new System.Drawing.Size(314, 37);
            this.informationBox.TabIndex = 3;
            this.informationBox.Text = "Changes to this configuration option value will not take effect until the SQL Ser" +
                "ver instance is restarted.";
            // 
            // newValueDescriptionLabel
            // 
            this.newValueDescriptionLabel.AutoSize = true;
            this.newValueDescriptionLabel.Location = new System.Drawing.Point(12, 62);
            this.newValueDescriptionLabel.Name = "newValueDescriptionLabel";
            this.newValueDescriptionLabel.Size = new System.Drawing.Size(62, 13);
            this.newValueDescriptionLabel.TabIndex = 4;
            this.newValueDescriptionLabel.Text = "New Value:";
            // 
            // newValueTextBox
            // 
            this.newValueTextBox.Location = new System.Drawing.Point(80, 59);
            this.newValueTextBox.Name = "newValueTextBox";
            this.newValueTextBox.Size = new System.Drawing.Size(246, 20);
            this.newValueTextBox.TabIndex = 0;
            this.newValueTextBox.TextChanged += new System.EventHandler(this.newValueTextBox_TextChanged);
            this.newValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.newValueTextBox_KeyPress);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // EditConfigurationValueDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(338, 124);
            this.Controls.Add(this.newValueTextBox);
            this.Controls.Add(this.informationBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.newValueDescriptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditConfigurationValueDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Configuration Value";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.EditConfigurationValueDialog_HelpButtonClicked);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private Divelements.WizardFramework.InformationBox informationBox;
        private System.Windows.Forms.Label newValueDescriptionLabel;
        private System.Windows.Forms.TextBox newValueTextBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}