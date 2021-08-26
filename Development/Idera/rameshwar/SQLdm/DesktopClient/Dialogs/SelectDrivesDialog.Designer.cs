namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectDrivesDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.driveListBox = new System.Windows.Forms.CheckedListBox();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.selectAllDrivesCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(169, 308);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(250, 308);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // driveListBox
            // 
            this.driveListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.driveListBox.CheckOnClick = true;
            this.driveListBox.FormattingEnabled = true;
            this.driveListBox.Location = new System.Drawing.Point(12, 62);
            this.driveListBox.Name = "driveListBox";
            this.driveListBox.Size = new System.Drawing.Size(307, 214);
            this.driveListBox.Sorted = true;
            this.driveListBox.TabIndex = 3;
            this.driveListBox.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            this.driveListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.driveListBox_ItemCheck);
            // 
            // informationBox1
            // 
            this.informationBox1.Location = new System.Drawing.Point(14, 6);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(382, 35);
            this.informationBox1.TabIndex = 4;
            this.informationBox1.Text = "Specify the disk drives to be excluded for this alert.";
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearButton.Location = new System.Drawing.Point(14, 308);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 5;
            this.clearButton.Text = "Clear All";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // selectAllDrivesCheckBox
            // 
            this.selectAllDrivesCheckBox.AutoSize = true;
            this.selectAllDrivesCheckBox.Location = new System.Drawing.Point(15, 43);
            this.selectAllDrivesCheckBox.Name = "selectAllDrivesCheckBox";
            this.selectAllDrivesCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllDrivesCheckBox.TabIndex = 6;
            this.selectAllDrivesCheckBox.Text = "Select All";
            this.selectAllDrivesCheckBox.UseVisualStyleBackColor = true;
            this.selectAllDrivesCheckBox.CheckedChanged += new System.EventHandler(this.selectAllDrivesCheckBox_CheckedChanged);
            // 
            // SelectDrivesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.MinimumSize = new System.Drawing.Size(315, 250);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(331, 337);
            this.Controls.Add(this.selectAllDrivesCheckBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.informationBox1);
            this.Controls.Add(this.driveListBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectDrivesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Excluded Disk Drives";
            this.Load += new System.EventHandler(this.SelectDrivesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckedListBox driveListBox;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.CheckBox selectAllDrivesCheckBox;
    }
}