namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SaveDashboardDesignDialog
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
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dashboardNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.useAsGlobalDefaultCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.selectedGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.selectedServersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.removeServersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addServersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.availableGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.availableServersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.availableServersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.selectedGroupBox.SuspendLayout();
            this.availableGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(482, 390);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(401, 390);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dashboard Name:";
            // 
            // dashboardNameTextBox
            // 
            this.dashboardNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dashboardNameTextBox.Location = new System.Drawing.Point(113, 19);
            this.dashboardNameTextBox.MaxLength = 128;
            this.dashboardNameTextBox.Name = "dashboardNameTextBox";
            this.dashboardNameTextBox.Size = new System.Drawing.Size(284, 20);
            this.dashboardNameTextBox.TabIndex = 1;
            // 
            // useAsGlobalDefaultCheckBox
            // 
            this.useAsGlobalDefaultCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.useAsGlobalDefaultCheckBox.AutoSize = true;
            this.useAsGlobalDefaultCheckBox.Location = new System.Drawing.Point(17, 394);
            this.useAsGlobalDefaultCheckBox.Name = "useAsGlobalDefaultCheckBox";
            this.useAsGlobalDefaultCheckBox.Size = new System.Drawing.Size(326, 17);
            this.useAsGlobalDefaultCheckBox.TabIndex = 6;
            this.useAsGlobalDefaultCheckBox.Text = "Use this dashboard as my default for new or unassigned servers";
            this.useAsGlobalDefaultCheckBox.UseVisualStyleBackColor = true;
            // 
            // selectedGroupBox
            // 
            this.selectedGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.selectedGroupBox.Controls.Add(this.selectedServersListBox);
            this.selectedGroupBox.Location = new System.Drawing.Point(334, 83);
            this.selectedGroupBox.Name = "selectedGroupBox";
            this.selectedGroupBox.Size = new System.Drawing.Size(223, 286);
            this.selectedGroupBox.TabIndex = 5;
            this.selectedGroupBox.TabStop = false;
            this.selectedGroupBox.Text = "Selected Servers";
            // 
            // selectedServersListBox
            // 
            this.selectedServersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedServersListBox.DisplayMember = "Text";
            this.selectedServersListBox.FormattingEnabled = true;
            this.selectedServersListBox.Location = new System.Drawing.Point(12, 25);
            this.selectedServersListBox.Name = "selectedServersListBox";
            this.selectedServersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedServersListBox.Size = new System.Drawing.Size(200, 251);
            this.selectedServersListBox.Sorted = true;
            this.selectedServersListBox.TabIndex = 0;
            this.selectedServersListBox.SelectedValueChanged += new System.EventHandler(this.selectedServersListBox_SelectedValueChanged);
            // 
            // removeServersButton
            // 
            this.removeServersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeServersButton.Enabled = false;
            this.removeServersButton.Location = new System.Drawing.Point(245, 223);
            this.removeServersButton.Name = "removeServersButton";
            this.removeServersButton.Size = new System.Drawing.Size(75, 23);
            this.removeServersButton.TabIndex = 4;
            this.removeServersButton.Text = "< Remove";
            this.removeServersButton.UseVisualStyleBackColor = true;
            this.removeServersButton.Click += new System.EventHandler(this.removeServersButton_Click);
            // 
            // addServersButton
            // 
            this.addServersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addServersButton.Enabled = false;
            this.addServersButton.Location = new System.Drawing.Point(245, 189);
            this.addServersButton.Name = "addServersButton";
            this.addServersButton.Size = new System.Drawing.Size(75, 23);
            this.addServersButton.TabIndex = 3;
            this.addServersButton.Text = "Add >";
            this.addServersButton.UseVisualStyleBackColor = true;
            this.addServersButton.Click += new System.EventHandler(this.addServersButton_Click);
            // 
            // availableGroupBox
            // 
            this.availableGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.availableGroupBox.Controls.Add(this.availableServersLabel);
            this.availableGroupBox.Controls.Add(this.availableServersListBox);
            this.availableGroupBox.Location = new System.Drawing.Point(8, 83);
            this.availableGroupBox.Name = "availableGroupBox";
            this.availableGroupBox.Size = new System.Drawing.Size(223, 286);
            this.availableGroupBox.TabIndex = 2;
            this.availableGroupBox.TabStop = false;
            this.availableGroupBox.Text = "Available Servers";
            // 
            // availableServersLabel
            // 
            this.availableServersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableServersLabel.BackColor = System.Drawing.Color.White;
            this.availableServersLabel.Enabled = false;
            this.availableServersLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableServersLabel.Location = new System.Drawing.Point(12, 25);
            this.availableServersLabel.Name = "availableServersLabel";
            this.availableServersLabel.Size = new System.Drawing.Size(200, 251);
            this.availableServersLabel.TabIndex = 0;
            this.availableServersLabel.Text = "< Unavailable >";
            this.availableServersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.availableServersLabel.Visible = false;
            // 
            // availableServersListBox
            // 
            this.availableServersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableServersListBox.DisplayMember = "Text";
            this.availableServersListBox.FormattingEnabled = true;
            this.availableServersListBox.HorizontalScrollbar = true;
            this.availableServersListBox.Location = new System.Drawing.Point(12, 25);
            this.availableServersListBox.Name = "availableServersListBox";
            this.availableServersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableServersListBox.Size = new System.Drawing.Size(200, 251);
            this.availableServersListBox.Sorted = true;
            this.availableServersListBox.TabIndex = 1;
            this.availableServersListBox.SelectedValueChanged += new System.EventHandler(this.availableServersListBox_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(14, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(537, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Saved dashboards can be assigned to multiple servers.  Select all servers that sh" +
                "ould use this dashboard.";
            // 
            // SaveDashboardDesignDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(565, 425);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectedGroupBox);
            this.Controls.Add(this.removeServersButton);
            this.Controls.Add(this.addServersButton);
            this.Controls.Add(this.availableGroupBox);
            this.Controls.Add(this.useAsGlobalDefaultCheckBox);
            this.Controls.Add(this.dashboardNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.HelpButton = true;
            this.Name = "SaveDashboardDesignDialog";
            this.Text = "Save Dashboard Design";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SaveDashboardDesignDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SaveDashboardDesignDialog_HelpRequested);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SaveDashboardDesignDialog_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SaveDashboardDesignDialog_KeyUp);
            this.selectedGroupBox.ResumeLayout(false);
            this.availableGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox dashboardNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox useAsGlobalDefaultCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox selectedGroupBox;
        private System.Windows.Forms.ListBox selectedServersListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeServersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addServersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox availableGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableServersLabel;
        private System.Windows.Forms.ListBox availableServersListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
    }
}