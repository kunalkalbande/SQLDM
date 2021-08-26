namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    partial class AddEditAzureProfile
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("profileValidationGroup");
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.comboBoxAppProfile = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.azureServerComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.descriptionTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.profileValidator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profileValidator)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(358, 250);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(278, 250);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.descriptionTextBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(430, 208);
            this.groupBox3.TabIndex = 58;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Azure Profile";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxAppProfile);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(10, 116);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 65);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Application Profile";
            // 
            // comboBoxAppProfile
            // 
            this.comboBoxAppProfile.FormattingEnabled = true;
            this.comboBoxAppProfile.Location = new System.Drawing.Point(130, 23);
            this.comboBoxAppProfile.Name = "comboBoxAppProfile";
            this.comboBoxAppProfile.Size = new System.Drawing.Size(258, 21);
            this.comboBoxAppProfile.TabIndex = 2;
            this.profileValidator.GetValidationSettings(this.comboBoxAppProfile).IsRequired = true;
            this.profileValidator.GetValidationSettings(this.comboBoxAppProfile).NotificationSettings.Caption = "Application Profile is required.";
            this.profileValidator.GetValidationSettings(this.comboBoxAppProfile).ValidationGroupKey = "profileValidationGroup";
            this.comboBoxAppProfile.SelectedValueChanged += new System.EventHandler(this.comboBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Application Profile";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.azureServerComboBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 59);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Azure Server";
            // 
            // azureServerComboBox
            // 
            this.azureServerComboBox.FormattingEnabled = true;
            this.azureServerComboBox.Location = new System.Drawing.Point(130, 25);
            this.azureServerComboBox.Name = "azureServerComboBox";
            this.azureServerComboBox.Size = new System.Drawing.Size(258, 21);
            this.azureServerComboBox.TabIndex = 1;
            this.profileValidator.GetValidationSettings(this.azureServerComboBox).IsRequired = true;
            this.profileValidator.GetValidationSettings(this.azureServerComboBox).NotificationSettings.Caption = "Azure Server is required.";
            this.profileValidator.GetValidationSettings(this.azureServerComboBox).ValidationGroupKey = "profileValidationGroup";
            this.azureServerComboBox.SelectedValueChanged += new System.EventHandler(this.comboBox_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Azure Server";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(140, 19);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(258, 20);
            this.descriptionTextBox.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 58;
            this.label8.Text = "Description";
            // 
            // profileValidator
            // 
            this.profileValidator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.profileValidator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "profileValidationGroup";
            this.profileValidator.ValidationGroups.Add(validationGroup1);
            this.profileValidator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.profileValidator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.profileValidator_Validating);
            // 
            // AddEditAzureProfile
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(454, 285);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditAzureProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Linked Profile";
            this.Load += new System.EventHandler(this.AddEditAzureProfile_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profileValidator)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox descriptionTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox comboBoxAppProfile;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureServerComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Infragistics.Win.Misc.UltraValidator profileValidator;
    }
}
