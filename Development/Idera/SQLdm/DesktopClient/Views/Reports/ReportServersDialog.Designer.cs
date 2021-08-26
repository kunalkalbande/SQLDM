namespace Idera.SQLdm.DesktopClient.Views.Reports {
    partial class ReportServersDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.selectAll = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.contentLayoutPanel = new System.Windows.Forms.Panel();
            this.infoLayoutPanel = new System.Windows.Forms.Panel();
            this.descriptionLabel = new Divelements.WizardFramework.InformationBox();
            this.buttonLayoutPanel = new System.Windows.Forms.Panel();
            this.contentLayoutPanel.SuspendLayout();
            this.infoLayoutPanel.SuspendLayout();
            this.buttonLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(222, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(141, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // selectAll
            // 
            this.selectAll.AutoSize = true;
            this.selectAll.Location = new System.Drawing.Point(3, 8);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(70, 17);
            this.selectAll.TabIndex = 5;
            this.selectAll.Text = "Select All";
            this.selectAll.UseVisualStyleBackColor = true;
            this.selectAll.CheckedChanged += new System.EventHandler(this.selectAll_CheckedChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.IntegralHeight = false;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 29);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(297, 209);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 6;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // contentLayoutPanel
            // 
            this.contentLayoutPanel.Controls.Add(this.checkedListBox1);
            this.contentLayoutPanel.Controls.Add(this.selectAll);
            this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayoutPanel.Location = new System.Drawing.Point(5, 60);
            this.contentLayoutPanel.Name = "contentLayoutPanel";
            this.contentLayoutPanel.Size = new System.Drawing.Size(297, 238);
            this.contentLayoutPanel.TabIndex = 7;
            // 
            // infoLayoutPanel
            // 
            this.infoLayoutPanel.Controls.Add(this.descriptionLabel);
            this.infoLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoLayoutPanel.Location = new System.Drawing.Point(5, 5);
            this.infoLayoutPanel.Name = "infoLayoutPanel";
            this.infoLayoutPanel.Size = new System.Drawing.Size(297, 55);
            this.infoLayoutPanel.TabIndex = 8;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionLabel.Location = new System.Drawing.Point(0, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(297, 55);
            this.descriptionLabel.TabIndex = 18;
            this.descriptionLabel.Text = "Select the server(s) for the report.";
            // 
            // buttonLayoutPanel
            // 
            this.buttonLayoutPanel.Controls.Add(this.okButton);
            this.buttonLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonLayoutPanel.Location = new System.Drawing.Point(5, 298);
            this.buttonLayoutPanel.Name = "buttonLayoutPanel";
            this.buttonLayoutPanel.Size = new System.Drawing.Size(297, 40);
            this.buttonLayoutPanel.TabIndex = 9;
            // 
            // ReportServersDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 343);
            this.Controls.Add(this.contentLayoutPanel);
            this.Controls.Add(this.buttonLayoutPanel);
            this.Controls.Add(this.infoLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(272, 208);
            this.Name = "ReportServersDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Select a Server";
            this.contentLayoutPanel.ResumeLayout(false);
            this.contentLayoutPanel.PerformLayout();
            this.infoLayoutPanel.ResumeLayout(false);
            this.buttonLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox selectAll;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Panel contentLayoutPanel;
        private System.Windows.Forms.Panel infoLayoutPanel;
        private System.Windows.Forms.Panel buttonLayoutPanel;
        private Divelements.WizardFramework.InformationBox descriptionLabel;
    }
}