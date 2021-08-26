namespace Idera.SQLdm.DesktopClient.Dialogs.AzureConfigurations
{
    partial class AddEditAzureAppProfile
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
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("appProfileNameValidationGroup");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("appProfileDetailsValidationGroup");
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.nameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.appProfileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.buttonDelApp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.comboBoxApp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.buttonEditApp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.buttonAddApp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.subscriptionComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.btnEditSubs = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnAddSubs = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnDeleteSubs = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.descriptionTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.statusMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureAppProfileValidator = new Infragistics.Win.Misc.UltraValidator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.appProfileBindingSource)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.azureAppProfileValidator)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(367, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(276, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.appProfileBindingSource, "Name", true));
            this.nameTextBox.Location = new System.Drawing.Point(101, 19);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(301, 20);
            this.nameTextBox.TabIndex = 0;
            this.azureAppProfileValidator.GetValidationSettings(this.nameTextBox).IsRequired = true;
            this.azureAppProfileValidator.GetValidationSettings(this.nameTextBox).NotificationSettings.Caption = "Azure Application Profile Name field is required.";
            this.azureAppProfileValidator.GetValidationSettings(this.nameTextBox).ValidationGroupKey = "appProfileNameValidationGroup";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // appProfileBindingSource
            // 
            this.appProfileBindingSource.DataSource = typeof(Idera.SQLdm.Common.Events.AzureMonitor.Interfaces.IAzureApplicationProfile);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.descriptionTextBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.nameTextBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(430, 311);
            this.groupBox3.TabIndex = 58;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Profile";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonDelApp);
            this.groupBox2.Controls.Add(this.comboBoxApp);
            this.groupBox2.Controls.Add(this.buttonEditApp);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonAddApp);
            this.groupBox2.Location = new System.Drawing.Point(5, 200);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 100);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Azure Application";
            // 
            // buttonDelApp
            // 
            this.buttonDelApp.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDelApp.Location = new System.Drawing.Point(186, 55);
            this.buttonDelApp.Name = "buttonDelApp";
            this.buttonDelApp.Size = new System.Drawing.Size(84, 23);
            this.buttonDelApp.TabIndex = 9;
            this.buttonDelApp.Text = "Delete";
            this.buttonDelApp.UseVisualStyleBackColor = true;
            this.buttonDelApp.Click += new System.EventHandler(this.buttonDelApp_Click);
            // 
            // comboBoxApp
            // 
            this.comboBoxApp.FormattingEnabled = true;
            this.comboBoxApp.Location = new System.Drawing.Point(96, 23);
            this.comboBoxApp.Name = "comboBoxApp";
            this.comboBoxApp.Size = new System.Drawing.Size(301, 21);
            this.comboBoxApp.TabIndex = 6;
            this.azureAppProfileValidator.GetValidationSettings(this.comboBoxApp).IsRequired = true;
            this.azureAppProfileValidator.GetValidationSettings(this.comboBoxApp).NotificationSettings.Caption = "Azure Application field is required.";
            this.azureAppProfileValidator.GetValidationSettings(this.comboBoxApp).ValidationGroupKey = "appProfileDetailsValidationGroup";
            this.comboBoxApp.SelectedIndexChanged += new System.EventHandler(this.comboBoxApp_SelectedIndexChanged);
            // 
            // buttonEditApp
            // 
            this.buttonEditApp.BackColor = System.Drawing.SystemColors.Control;
            this.buttonEditApp.Location = new System.Drawing.Point(96, 55);
            this.buttonEditApp.Name = "buttonEditApp";
            this.buttonEditApp.Size = new System.Drawing.Size(84, 23);
            this.buttonEditApp.TabIndex = 8;
            this.buttonEditApp.Text = "View/Edit";
            this.buttonEditApp.UseVisualStyleBackColor = true;
            this.buttonEditApp.Click += new System.EventHandler(this.buttonEditApp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Application";
            // 
            // buttonAddApp
            // 
            this.buttonAddApp.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAddApp.Location = new System.Drawing.Point(6, 55);
            this.buttonAddApp.Name = "buttonAddApp";
            this.buttonAddApp.Size = new System.Drawing.Size(84, 23);
            this.buttonAddApp.TabIndex = 7;
            this.buttonAddApp.Text = "New";
            this.buttonAddApp.UseVisualStyleBackColor = true;
            this.buttonAddApp.Click += new System.EventHandler(this.buttonAddApp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.subscriptionComboBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnEditSubs);
            this.groupBox1.Controls.Add(this.btnAddSubs);
            this.groupBox1.Controls.Add(this.btnDeleteSubs);
            this.groupBox1.Location = new System.Drawing.Point(5, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 100);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azure Subscription";
            // 
            // subscriptionComboBox
            // 
            this.subscriptionComboBox.FormattingEnabled = true;
            this.subscriptionComboBox.Location = new System.Drawing.Point(96, 25);
            this.subscriptionComboBox.Name = "subscriptionComboBox";
            this.subscriptionComboBox.Size = new System.Drawing.Size(301, 21);
            this.subscriptionComboBox.TabIndex = 2;
            this.azureAppProfileValidator.GetValidationSettings(this.subscriptionComboBox).IsRequired = true;
            this.azureAppProfileValidator.GetValidationSettings(this.subscriptionComboBox).NotificationSettings.Caption = "Azure Subscription field is required.";
            this.azureAppProfileValidator.GetValidationSettings(this.subscriptionComboBox).ValidationGroupKey = "appProfileDetailsValidationGroup";
            this.subscriptionComboBox.SelectedIndexChanged += new System.EventHandler(this.subscriptionComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Subscription";
            // 
            // btnEditSubs
            // 
            this.btnEditSubs.BackColor = System.Drawing.SystemColors.Control;
            this.btnEditSubs.Location = new System.Drawing.Point(96, 63);
            this.btnEditSubs.Name = "btnEditSubs";
            this.btnEditSubs.Size = new System.Drawing.Size(84, 23);
            this.btnEditSubs.TabIndex = 4;
            this.btnEditSubs.Text = "View/Edit";
            this.btnEditSubs.UseVisualStyleBackColor = true;
            this.btnEditSubs.Click += new System.EventHandler(this.btnEditSubs_Click);
            // 
            // btnAddSubs
            // 
            this.btnAddSubs.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddSubs.Location = new System.Drawing.Point(6, 63);
            this.btnAddSubs.Name = "btnAddSubs";
            this.btnAddSubs.Size = new System.Drawing.Size(84, 23);
            this.btnAddSubs.TabIndex = 3;
            this.btnAddSubs.Text = "New";
            this.btnAddSubs.UseVisualStyleBackColor = true;
            this.btnAddSubs.Click += new System.EventHandler(this.btnAddSubs_Click);
            // 
            // btnDeleteSubs
            // 
            this.btnDeleteSubs.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeleteSubs.Location = new System.Drawing.Point(186, 63);
            this.btnDeleteSubs.Name = "btnDeleteSubs";
            this.btnDeleteSubs.Size = new System.Drawing.Size(84, 23);
            this.btnDeleteSubs.TabIndex = 5;
            this.btnDeleteSubs.Text = "Delete";
            this.btnDeleteSubs.UseVisualStyleBackColor = true;
            this.btnDeleteSubs.Click += new System.EventHandler(this.btnDeleteSubs_Click);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.appProfileBindingSource, "Description", true));
            this.descriptionTextBox.Location = new System.Drawing.Point(101, 45);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(301, 20);
            this.descriptionTextBox.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 58;
            this.label8.Text = "Description";
            // 
            // statusMessage
            // 
            this.statusMessage.Location = new System.Drawing.Point(279, 339);
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(163, 23);
            this.statusMessage.TabIndex = 59;
            this.statusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // azureAppProfileValidator
            // 
            this.azureAppProfileValidator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.azureAppProfileValidator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "appProfileNameValidationGroup";
            validationGroup2.Key = "appProfileDetailsValidationGroup";
            this.azureAppProfileValidator.ValidationGroups.Add(validationGroup1);
            this.azureAppProfileValidator.ValidationGroups.Add(validationGroup2);
            this.azureAppProfileValidator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.azureAppProfileValidator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.azureAppProfileValidator_Validating);
            // 
            // AddEditAzureAppProfile
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(454, 411);
            this.Controls.Add(this.statusMessage);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditAzureAppProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Application Profile";
            this.Load += new System.EventHandler(this.AddEditAzureAppProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.appProfileBindingSource)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.azureAppProfileValidator)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox nameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox descriptionTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton buttonDelApp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox comboBoxApp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton buttonEditApp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton buttonAddApp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox subscriptionComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnEditSubs;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnAddSubs;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnDeleteSubs;
        private System.Windows.Forms.BindingSource appProfileBindingSource;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusMessage;
        private Infragistics.Win.Misc.UltraValidator azureAppProfileValidator;
    }
}