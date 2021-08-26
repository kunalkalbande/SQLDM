using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class TagPropertiesDialog
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
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;

            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tagNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.availableServersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this.selectAllServersCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.serversStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tabControl1 = new Idera.SQLdm.DesktopClient.Views.Reports.CustomTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.selectAllCustomCountersCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.customCountersStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.customCountersListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.selectAllPermissionsCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.permissionsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.permissionsListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this.saveChangesWorker = new System.ComponentModel.BackgroundWorker();
            this.initializeWorker = new System.ComponentModel.BackgroundWorker();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(291, 404);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(372, 404);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // tagNameTextBox
            // 
            this.tagNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tagNameTextBox.Location = new System.Drawing.Point(56, 9);
            this.tagNameTextBox.MaxLength = 50;
            this.tagNameTextBox.Name = "tagNameTextBox";
            this.tagNameTextBox.Size = new System.Drawing.Size(391, 20);
            this.tagNameTextBox.TabIndex = 0;
            this.tagNameTextBox.TextChanged += new System.EventHandler(this.tagNameTextBox_TextChanged);
            this.tagNameTextBox.Leave += new System.EventHandler(this.tagNameTextBox_Leave);
            this.tagNameTextBox.Enter += new System.EventHandler(this.tagNameTextBox_Enter);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.availableServersListBox);
            this.tabPage1.Controls.Add(this.selectAllServersCheckBox);
            this.tabPage1.Controls.Add(this.serversStatusLabel);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(424, 330);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Servers";
            this.tabPage1.UseVisualStyleBackColor = true;
            
            this.tabPage1.BackColor = backColor;
            this.tabPage1.ForeColor = foreColor;
            // 
            // availableServersListBox
            // 
            this.availableServersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableServersListBox.CheckOnClick = true;
            this.availableServersListBox.FormattingEnabled = true;
            this.availableServersListBox.Location = new System.Drawing.Point(6, 50);
            this.availableServersListBox.Name = "availableServersListBox";
            this.availableServersListBox.Size = new System.Drawing.Size(412, 274);
            this.availableServersListBox.Sorted = true;
            this.availableServersListBox.TabIndex = 0;
            this.availableServersListBox.BackColor =backColor;
            this.availableServersListBox.ForeColor = foreColor;
            this.availableServersListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.availableServersListBox_ItemCheck);
            // 
            // selectAllServersCheckBox
            // 
            this.selectAllServersCheckBox.AutoSize = true;
            this.selectAllServersCheckBox.Location = new System.Drawing.Point(9, 32);
            this.selectAllServersCheckBox.Name = "selectAllServersCheckBox";
            this.selectAllServersCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllServersCheckBox.TabIndex = 3;
            this.selectAllServersCheckBox.Text = "Select All";
            this.selectAllServersCheckBox.UseVisualStyleBackColor = true;
            this.selectAllServersCheckBox.CheckedChanged += new System.EventHandler(this.selectAllServersCheckBox_CheckedChanged);
            // 
            // serversStatusLabel
            // 
            this.serversStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serversStatusLabel.BackColor = System.Drawing.Color.White;
            this.serversStatusLabel.Location = new System.Drawing.Point(15, 159);
            this.serversStatusLabel.Name = "serversStatusLabel";
            this.serversStatusLabel.Size = new System.Drawing.Size(395, 13);
            this.serversStatusLabel.TabIndex = 2;
            this.serversStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.serversStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(412, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Select the monitored servers that should be associated with this tag:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(15, 42);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(432, 356);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.selectAllCustomCountersCheckBox);
            this.tabPage2.Controls.Add(this.customCountersStatusLabel);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.customCountersListBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(424, 330);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Custom Counters";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.BackColor = backColor;
            this.tabPage2.ForeColor = foreColor;
            // 
            // selectAllCustomCountersCheckBox
            // 
            this.selectAllCustomCountersCheckBox.AutoSize = true;
            this.selectAllCustomCountersCheckBox.Location = new System.Drawing.Point(9, 32);
            this.selectAllCustomCountersCheckBox.Name = "selectAllCustomCountersCheckBox";
            this.selectAllCustomCountersCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllCustomCountersCheckBox.TabIndex = 5;
            this.selectAllCustomCountersCheckBox.Text = "Select All";
            this.selectAllCustomCountersCheckBox.UseVisualStyleBackColor = true;
            this.selectAllCustomCountersCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCustomCountersCheckBox_CheckedChanged);
            // 
            // customCountersStatusLabel
            // 
            this.customCountersStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.customCountersStatusLabel.BackColor = System.Drawing.Color.White;
            this.customCountersStatusLabel.Location = new System.Drawing.Point(15, 177);
            this.customCountersStatusLabel.Name = "customCountersStatusLabel";
            this.customCountersStatusLabel.Size = new System.Drawing.Size(395, 13);
            this.customCountersStatusLabel.TabIndex = 4;
            this.customCountersStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.customCountersStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(412, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Select the custom counters that should be associated with this tag:";
            // 
            // customCountersListBox
            // 
            this.customCountersListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.customCountersListBox.CheckOnClick = true;
            this.customCountersListBox.FormattingEnabled = true;
            this.customCountersListBox.Location = new System.Drawing.Point(6, 50);
            this.customCountersListBox.Name = "customCountersListBox";
            this.customCountersListBox.Size = new System.Drawing.Size(412, 274);
            this.customCountersListBox.Sorted = true;
            this.customCountersListBox.TabIndex = 2;
            this.customCountersListBox.BackColor = backColor;
            this.customCountersListBox.ForeColor = foreColor;
            this.customCountersListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.customCountersListBox_ItemCheck);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.selectAllPermissionsCheckBox);
            this.tabPage3.Controls.Add(this.permissionsStatusLabel);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.permissionsListBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(424, 330);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Application Security";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.BackColor = backColor;
            this.tabPage3.ForeColor = foreColor;
            // 
            // selectAllPermissionsCheckBox
            // 
            this.selectAllPermissionsCheckBox.AutoSize = true;
            this.selectAllPermissionsCheckBox.Location = new System.Drawing.Point(9, 32);
            this.selectAllPermissionsCheckBox.Name = "selectAllPermissionsCheckBox";
            this.selectAllPermissionsCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllPermissionsCheckBox.TabIndex = 7;
            this.selectAllPermissionsCheckBox.Text = "Select All";
            this.selectAllPermissionsCheckBox.UseVisualStyleBackColor = true;
            this.selectAllPermissionsCheckBox.CheckedChanged += new System.EventHandler(this.selectAllPermissionsCheckBox_CheckedChanged);
            // 
            // permissionsStatusLabel
            // 
            this.permissionsStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.permissionsStatusLabel.BackColor = System.Drawing.Color.White;
            this.permissionsStatusLabel.Location = new System.Drawing.Point(15, 177);
            this.permissionsStatusLabel.Name = "permissionsStatusLabel";
            this.permissionsStatusLabel.Size = new System.Drawing.Size(395, 13);
            this.permissionsStatusLabel.TabIndex = 6;
            this.permissionsStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.permissionsStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(412, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = "Select the application security permissions that should be associated with this t" +
                "ag:";
            // 
            // permissionsListBox
            // 
            this.permissionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.permissionsListBox.CheckOnClick = true;
            this.permissionsListBox.FormattingEnabled = true;
            this.permissionsListBox.Location = new System.Drawing.Point(6, 50);
            this.permissionsListBox.Name = "permissionsListBox";
            this.permissionsListBox.Size = new System.Drawing.Size(412, 274);
            this.permissionsListBox.Sorted = true;
            this.permissionsListBox.TabIndex = 4;
            this.permissionsListBox.BackColor = backColor;
            this.permissionsListBox.ForeColor = foreColor;
            this.permissionsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.permissionsListBox_ItemCheck);
            // 
            // saveChangesWorker
            // 
            this.saveChangesWorker.WorkerSupportsCancellation = true;
            this.saveChangesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.saveChangesWorker_DoWork);
            this.saveChangesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.saveChangesWorker_RunWorkerCompleted);
            // 
            // initializeWorker
            // 
            this.initializeWorker.WorkerSupportsCancellation = true;
            this.initializeWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.initializeWorker_DoWork);
            this.initializeWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.initializeWorker_RunWorkerCompleted);
            // 
            // TagPropertiesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(459, 439);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tagNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TagPropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tag Properties";
            this.backcolor = backcolor;
            this.ForeColor = foreColor;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.TagPropertiesDialog_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TagPropertiesDialog_FormClosed);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.TagPropertiesDialog_HelpRequested);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox tagNameTextBox;
        private System.Windows.Forms.TabPage tabPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private System.Windows.Forms.CheckedListBox availableServersListBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.ComponentModel.BackgroundWorker saveChangesWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serversStatusLabel;
        private System.ComponentModel.BackgroundWorker initializeWorker;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private System.Windows.Forms.CheckedListBox customCountersListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private System.Windows.Forms.CheckedListBox permissionsListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel customCountersStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel permissionsStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox selectAllServersCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox selectAllCustomCountersCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox selectAllPermissionsCheckBox;
    }
}