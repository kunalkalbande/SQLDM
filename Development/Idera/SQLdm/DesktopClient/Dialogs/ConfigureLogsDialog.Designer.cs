namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ConfigureLogsDialog
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
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.optionsGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.configureLogsFlowLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.archiveNumberLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.archiveNumberNumericEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.archiveNumberLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.infoLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.unlimitedCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.optionsGroupBox.SuspendLayout();
            this.configureLogsFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.archiveNumberNumericEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(152, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(71, 126);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.configureLogsFlowLayoutPanel);
            this.optionsGroupBox.Controls.Add(this.infoLabel);
            this.optionsGroupBox.Controls.Add(this.unlimitedCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(7, 6);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(220, 112);
            this.optionsGroupBox.TabIndex = 7;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "SQL Server Log files";
            // 
            // configureLogsFlowLayoutPanel
            // 
            this.configureLogsFlowLayoutPanel.AutoSize = true;
            this.configureLogsFlowLayoutPanel.Controls.Add(this.archiveNumberLabel);
            this.configureLogsFlowLayoutPanel.Controls.Add(this.archiveNumberNumericEditor);
            this.configureLogsFlowLayoutPanel.Controls.Add(this.archiveNumberLabel1);
            this.configureLogsFlowLayoutPanel.Location = new System.Drawing.Point(5, 43);
            this.configureLogsFlowLayoutPanel.Name = "configureLogsFlowLayoutPanel";
            this.configureLogsFlowLayoutPanel.Size = new System.Drawing.Size(210, 28);
            this.configureLogsFlowLayoutPanel.TabIndex = 10;
            // 
            // archiveNumberLabel
            // 
            this.archiveNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.archiveNumberLabel.AutoSize = true;
            this.archiveNumberLabel.Location = new System.Drawing.Point(3, 7);
            this.archiveNumberLabel.Name = "archiveNumberLabel";
            this.archiveNumberLabel.Size = new System.Drawing.Size(32, 13);
            this.archiveNumberLabel.TabIndex = 4;
            this.archiveNumberLabel.Text = "Keep";
            // 
            // archiveNumberNumericEditor
            // 
            this.archiveNumberNumericEditor.FormatString = "#0";
            this.archiveNumberNumericEditor.Location = new System.Drawing.Point(41, 3);
            this.archiveNumberNumericEditor.MaxValue = 99;
            this.archiveNumberNumericEditor.MinValue = 1;
            this.archiveNumberNumericEditor.Name = "archiveNumberNumericEditor";
            this.archiveNumberNumericEditor.PromptChar = ' ';
            this.archiveNumberNumericEditor.Size = new System.Drawing.Size(46, 21);
            this.archiveNumberNumericEditor.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.archiveNumberNumericEditor.TabIndex = 3;
            // 
            // archiveNumberLabel1
            // 
            this.archiveNumberLabel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.archiveNumberLabel1.AutoSize = true;
            this.archiveNumberLabel1.Location = new System.Drawing.Point(93, 7);
            this.archiveNumberLabel1.Name = "archiveNumberLabel1";
            this.archiveNumberLabel1.Size = new System.Drawing.Size(63, 13);
            this.archiveNumberLabel1.TabIndex = 11;
            this.archiveNumberLabel1.Text = "archive files";
            // 
            // infoLabel
            // 
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(3, 72);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(214, 37);
            this.infoLabel.TabIndex = 9;
            this.infoLabel.Text = "< info >";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // unlimitedCheckBox
            // 
            this.unlimitedCheckBox.AutoSize = true;
            this.unlimitedCheckBox.Location = new System.Drawing.Point(10, 19);
            this.unlimitedCheckBox.Name = "unlimitedCheckBox";
            this.unlimitedCheckBox.Size = new System.Drawing.Size(69, 17);
            this.unlimitedCheckBox.TabIndex = 2;
            this.unlimitedCheckBox.Text = "Unlimited";
            this.unlimitedCheckBox.UseVisualStyleBackColor = true;
            this.unlimitedCheckBox.CheckedChanged += new System.EventHandler(this.unlimitedCheckBox_CheckedChanged);
            // 
            // ConfigureLogsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(234, 155);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureLogsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure SQL Server Logs";
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.configureLogsFlowLayoutPanel.ResumeLayout(false);
            this.configureLogsFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.archiveNumberNumericEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox optionsGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox unlimitedCheckBox;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor archiveNumberNumericEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel archiveNumberLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel infoLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel configureLogsFlowLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel archiveNumberLabel1;
    }
}