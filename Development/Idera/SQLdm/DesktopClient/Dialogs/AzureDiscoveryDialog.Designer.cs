namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AzureDiscoveryDialog
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
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.textBoxPassword = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.textBoxLoginName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureProfileComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.addAzureProfileButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.selectAzureProfileLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(357, 189);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(271, 189);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxPassword);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxLoginName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.azureProfileComboBox);
            this.groupBox2.Controls.Add(this.addAzureProfileButton);
            this.groupBox2.Controls.Add(this.selectAzureProfileLbl);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 167);
            this.groupBox2.TabIndex = 56;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Azure Application";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(168, 47);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(230, 20);
            this.textBoxPassword.TabIndex = 1;
            this.textBoxPassword.Enabled = false;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 60;
            this.label4.Text = "Password:";
            // 
            // textBoxLoginName
            // 
            this.textBoxLoginName.Location = new System.Drawing.Point(168, 21);
            this.textBoxLoginName.Name = "textBoxLoginName";
            this.textBoxLoginName.Size = new System.Drawing.Size(230, 20);
            this.textBoxLoginName.TabIndex = 0;
            this.textBoxLoginName.Enabled = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 58;
            this.label3.Text = "Login name:";
            // 
            // azureProfileComboBox
            // 
            this.azureProfileComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureProfileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureProfileComboBox.Location = new System.Drawing.Point(168, 73);
            this.azureProfileComboBox.Name = "azureProfileComboBox";
            this.azureProfileComboBox.Size = new System.Drawing.Size(230, 21);
            this.azureProfileComboBox.TabIndex = 25;
            this.azureProfileComboBox.SelectedIndexChanged += new System.EventHandler(this.azureProfileComboBox_SelectedIndexChanged);
            // 
            // addAzureProfileButton
            // 
            this.addAzureProfileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addAzureProfileButton.Location = new System.Drawing.Point(9, 122);
            this.addAzureProfileButton.Name = "addAzureProfileButton";
            this.addAzureProfileButton.Size = new System.Drawing.Size(152, 23);
            this.addAzureProfileButton.TabIndex = 1;
            this.addAzureProfileButton.Text = "Manage Azure Profile";
            this.addAzureProfileButton.UseVisualStyleBackColor = true;
            this.addAzureProfileButton.Click += new System.EventHandler(this.addAzureProfileButton_Click);
            // 
            // selectAzureProfileLbl
            // 
            this.selectAzureProfileLbl.AutoSize = true;
            this.selectAzureProfileLbl.Location = new System.Drawing.Point(7, 81);
            this.selectAzureProfileLbl.Name = "selectAzureProfileLbl";
            this.selectAzureProfileLbl.Size = new System.Drawing.Size(99, 13);
            this.selectAzureProfileLbl.TabIndex = 1;
            this.selectAzureProfileLbl.Text = "Select Azure Profile";
            // 
            // AzureDiscoveryDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(444, 224);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AzureDiscoveryDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Application Configuration";
            this.Load += new System.EventHandler(this.AzureDiscoveryDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBoxPassword;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBoxLoginName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Infragistics.Win.Misc.UltraValidator azureApplicationValidator;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addAzureProfileButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel selectAzureProfileLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureProfileComboBox;


    }
}