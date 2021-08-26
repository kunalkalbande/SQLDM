namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    partial class AddEditAzureApplication
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
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("applicationNameValidationGroup");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("applicationDetailsValidationGroup");
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxSecret = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxClientId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTenantId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.azureApplicationValidator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.azureApplicationValidator)).BeginInit();
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
            this.groupBox2.Controls.Add(this.textBoxDescription);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxSecret);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxClientId);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxTenantId);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 167);
            this.groupBox2.TabIndex = 56;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Azure Application";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(92, 125);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(306, 20);
            this.textBoxDescription.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 66;
            this.label7.Text = "Description";
            // 
            // textBoxSecret
            // 
            this.textBoxSecret.Location = new System.Drawing.Point(92, 99);
            this.textBoxSecret.Name = "textBoxSecret";
            this.textBoxSecret.Size = new System.Drawing.Size(306, 20);
            this.textBoxSecret.TabIndex = 3;
            this.textBoxSecret.UseSystemPasswordChar = true;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxSecret).IsRequired = true;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxSecret).NotificationSettings.Caption = "Azure Application Client Secret field is required.";
            this.azureApplicationValidator.GetValidationSettings(this.textBoxSecret).ValidationGroupKey = "applicationDetailsValidationGroup";
            this.textBoxSecret.TextChanged += new System.EventHandler(this.textBoxSecret_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 64;
            this.label6.Text = "Secret";
            // 
            // textBoxClientId
            // 
            this.textBoxClientId.Location = new System.Drawing.Point(92, 73);
            this.textBoxClientId.Name = "textBoxClientId";
            this.textBoxClientId.Size = new System.Drawing.Size(306, 20);
            this.textBoxClientId.TabIndex = 2;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxClientId).IsRequired = true;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxClientId).NotificationSettings.Caption = "Azure Application Client Id field is required.";
            this.azureApplicationValidator.GetValidationSettings(this.textBoxClientId).ValidationGroupKey = "applicationDetailsValidationGroup";
            this.textBoxClientId.TextChanged += new System.EventHandler(this.textBoxClientId_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 62;
            this.label5.Text = "Client Id";
            // 
            // textBoxTenantId
            // 
            this.textBoxTenantId.Location = new System.Drawing.Point(92, 47);
            this.textBoxTenantId.Name = "textBoxTenantId";
            this.textBoxTenantId.Size = new System.Drawing.Size(306, 20);
            this.textBoxTenantId.TabIndex = 1;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxTenantId).IsRequired = true;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxTenantId).NotificationSettings.Caption = "Azure Application Tenant Id field is required.";
            this.azureApplicationValidator.GetValidationSettings(this.textBoxTenantId).ValidationGroupKey = "applicationDetailsValidationGroup";
            this.textBoxTenantId.TextChanged += new System.EventHandler(this.textBoxTenantId_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 60;
            this.label4.Text = "Tenant Id";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(92, 21);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(306, 20);
            this.textBoxName.TabIndex = 0;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxName).IsRequired = true;
            this.azureApplicationValidator.GetValidationSettings(this.textBoxName).NotificationSettings.Caption = "Azure Application Name field is required.";
            this.azureApplicationValidator.GetValidationSettings(this.textBoxName).ValidationGroupKey = "applicationNameValidationGroup";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 58;
            this.label3.Text = "Name";
            // 
            // azureApplicationValidator
            // 
            this.azureApplicationValidator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.azureApplicationValidator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "applicationNameValidationGroup";
            validationGroup2.Key = "applicationDetailsValidationGroup";
            this.azureApplicationValidator.ValidationGroups.Add(validationGroup1);
            this.azureApplicationValidator.ValidationGroups.Add(validationGroup2);
            this.azureApplicationValidator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.azureApplicationValidator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.azureApplicationValidator_Validating);
            // 
            // AddEditAzureApplication
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
            this.Name = "AddEditAzureApplication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Application Configuration";
            this.Load += new System.EventHandler(this.AddEditAzureApplication_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.azureApplicationValidator)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxSecret;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxClientId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTenantId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.Misc.UltraValidator azureApplicationValidator;
    }
}