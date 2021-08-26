namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectCustomCounters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectCustomCounters));
            this.lblMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.selectedGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.selectedCountersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.availableCountersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.removeCountersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addCountersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.availableGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.availableCountersLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.selectedGroupBox.SuspendLayout();
            this.availableGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(172, 368);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 13);
            this.lblMessage.TabIndex = 35;
            // 
            // selectedGroupBox
            // 
            this.selectedGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.selectedGroupBox.Controls.Add(this.selectedCountersListBox);
            this.selectedGroupBox.Location = new System.Drawing.Point(349, 72);
            this.selectedGroupBox.Name = "selectedGroupBox";
            this.selectedGroupBox.Size = new System.Drawing.Size(223, 286);
            this.selectedGroupBox.TabIndex = 34;
            this.selectedGroupBox.TabStop = false;
            this.selectedGroupBox.Text = "Selected Counters";
            // 
            // selectedCountersListBox
            // 
            this.selectedCountersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedCountersListBox.DisplayMember = "Text";
            this.selectedCountersListBox.FormattingEnabled = true;
            this.selectedCountersListBox.HorizontalScrollbar = true;
            this.selectedCountersListBox.Location = new System.Drawing.Point(12, 25);
            this.selectedCountersListBox.Name = "selectedCountersListBox";
            this.selectedCountersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedCountersListBox.Size = new System.Drawing.Size(200, 251);
            this.selectedCountersListBox.Sorted = true;
            this.selectedCountersListBox.TabIndex = 28;
            this.selectedCountersListBox.SelectedValueChanged += new System.EventHandler(this.selectedCountersListBox_SelectedValueChanged);
            // 
            // availableCountersListBox
            // 
            this.availableCountersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableCountersListBox.DisplayMember = "Text";
            this.availableCountersListBox.FormattingEnabled = true;
            this.availableCountersListBox.HorizontalScrollbar = true;
            this.availableCountersListBox.Location = new System.Drawing.Point(12, 25);
            this.availableCountersListBox.Name = "availableCountersListBox";
            this.availableCountersListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableCountersListBox.Size = new System.Drawing.Size(200, 251);
            this.availableCountersListBox.Sorted = true;
            this.availableCountersListBox.TabIndex = 33;
            this.availableCountersListBox.SelectedValueChanged += new System.EventHandler(this.availableCountersListBox_SelectedValueChanged);
            // 
            // removeCountersButton
            // 
            this.removeCountersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeCountersButton.Enabled = false;
            this.removeCountersButton.Location = new System.Drawing.Point(254, 212);
            this.removeCountersButton.Name = "removeCountersButton";
            this.removeCountersButton.Size = new System.Drawing.Size(75, 23);
            this.removeCountersButton.TabIndex = 31;
            this.removeCountersButton.Text = "< Remove";
            this.removeCountersButton.UseVisualStyleBackColor = true;
            this.removeCountersButton.Click += new System.EventHandler(this.removeCountersButton_Click);
            // 
            // addCountersButton
            // 
            this.addCountersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addCountersButton.Enabled = false;
            this.addCountersButton.Location = new System.Drawing.Point(254, 178);
            this.addCountersButton.Name = "addCountersButton";
            this.addCountersButton.Size = new System.Drawing.Size(75, 23);
            this.addCountersButton.TabIndex = 30;
            this.addCountersButton.Text = "Add >";
            this.addCountersButton.UseVisualStyleBackColor = true;
            this.addCountersButton.Click += new System.EventHandler(this.addCountersButton_Click);
            // 
            // availableGroupBox
            // 
            this.availableGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.availableGroupBox.Controls.Add(this.availableCountersLabel);
            this.availableGroupBox.Controls.Add(this.availableCountersListBox);
            this.availableGroupBox.Location = new System.Drawing.Point(11, 72);
            this.availableGroupBox.Name = "availableGroupBox";
            this.availableGroupBox.Size = new System.Drawing.Size(223, 286);
            this.availableGroupBox.TabIndex = 29;
            this.availableGroupBox.TabStop = false;
            this.availableGroupBox.Text = "Available Counters";
            // 
            // availableCountersLabel
            // 
            this.availableCountersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableCountersLabel.BackColor = System.Drawing.Color.White;
            this.availableCountersLabel.Enabled = false;
            this.availableCountersLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableCountersLabel.Location = new System.Drawing.Point(12, 25);
            this.availableCountersLabel.Name = "availableCountersLabel";
            this.availableCountersLabel.Size = new System.Drawing.Size(200, 251);
            this.availableCountersLabel.TabIndex = 32;
            this.availableCountersLabel.Text = "< Unavailable >";
            this.availableCountersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.availableCountersLabel.Visible = false;
            // 
            // informationBox
            // 
            this.informationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox.Location = new System.Drawing.Point(10, 16);
            this.informationBox.Name = "informationBox";
            this.informationBox.Size = new System.Drawing.Size(563, 50);
            this.informationBox.TabIndex = 28;
            this.informationBox.Text = resources.GetString("informationBox.Text");
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(416, 384);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 37;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(497, 384);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 36;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SelectCustomCounters
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(594, 419);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.selectedGroupBox);
            this.Controls.Add(this.removeCountersButton);
            this.Controls.Add(this.addCountersButton);
            this.Controls.Add(this.availableGroupBox);
            this.Controls.Add(this.informationBox);
            this.MinimumSize = new System.Drawing.Size(600, 380);
            this.Name = "SelectCustomCounters";
            this.Text = "Select Custom Counters";
            this.selectedGroupBox.ResumeLayout(false);
            this.availableGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblMessage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox selectedGroupBox;
        private System.Windows.Forms.ListBox selectedCountersListBox;
        private System.Windows.Forms.ListBox availableCountersListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeCountersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addCountersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox availableGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableCountersLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
    }
}