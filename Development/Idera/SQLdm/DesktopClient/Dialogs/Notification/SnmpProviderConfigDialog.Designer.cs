namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class SnmpProviderConfigDialog
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
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("Name");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("ManagerAddress");
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.testButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.providerName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.managerAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.ultraGroupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraGroupBox();
            this.managerCommunity = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.managerPort = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.managerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(376, 155);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(295, 155);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(13, 155);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 4;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // providerName
            // 
            this.providerName.Location = new System.Drawing.Point(106, 16);
            this.providerName.Name = "providerName";
            this.providerName.Size = new System.Drawing.Size(318, 20);
            this.providerName.TabIndex = 14;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Caption = "Provider Name Required";
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Text = "Please enter a unique name for the Action Provider";
            this.validator.GetValidationSettings(this.providerName).ValidationGroupKey = "Name";
            this.validator.GetValidationSettings(this.providerName).ValidationPropertyName = "Text";
            this.providerName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Provider Name:";
            // 
            // validator
            // 
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "Name";
            validationGroup2.Key = "ManagerAddress";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationGroups.Add(validationGroup2);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // managerAddress
            // 
            this.managerAddress.Location = new System.Drawing.Point(92, 21);
            this.managerAddress.Name = "managerAddress";
            this.managerAddress.Size = new System.Drawing.Size(319, 20);
            this.managerAddress.TabIndex = 20;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Caption = "Address Required";
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Text = "The address of the Network Management System must be provided.  The value specifi" +
                "ed must be a valid computer name, or valid IPv4 address.";
            this.validator.GetValidationSettings(this.managerAddress).ValidationGroupKey = "ManagerAddress";
            this.validator.GetValidationSettings(this.managerAddress).ValidationPropertyName = "Text";
            this.managerAddress.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            this.ultraGroupBox1.Controls.Add(this.managerCommunity);
            this.ultraGroupBox1.Controls.Add(this.label12);
            this.ultraGroupBox1.Controls.Add(this.managerPort);
            this.ultraGroupBox1.Controls.Add(this.managerAddress);
            this.ultraGroupBox1.Controls.Add(this.label11);
            this.ultraGroupBox1.Controls.Add(this.label10);
            this.ultraGroupBox1.Location = new System.Drawing.Point(13, 42);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(438, 107);
            this.ultraGroupBox1.TabIndex = 15;
            this.ultraGroupBox1.Text = "Network Manager Information";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // managerCommunity
            // 
            this.managerCommunity.Location = new System.Drawing.Point(92, 73);
            this.managerCommunity.Name = "managerCommunity";
            this.managerCommunity.Size = new System.Drawing.Size(319, 20);
            this.managerCommunity.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Community:";
            // 
            // managerPort
            // 
            this.managerPort.AutoSize = false;
            this.managerPort.Location = new System.Drawing.Point(92, 46);
            this.managerPort.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.managerPort.MaskInput = "nnnnn";
            this.managerPort.MaxValue = 65530;
            this.managerPort.MinValue = 2;
            this.managerPort.Name = "managerPort";
            this.managerPort.Size = new System.Drawing.Size(73, 21);
            this.managerPort.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.managerPort.SpinWrap = true;
            this.managerPort.TabIndex = 22;
            this.managerPort.UseAppStyling = false;
            this.managerPort.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.managerPort.Value = 162;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Port:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Address:";
            // 
            // SnmpProviderConfigDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(463, 189);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.providerName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnmpProviderConfigDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SNMP Action Provider";
            this.Load += new System.EventHandler(this.SnmpProviderConfigDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.managerPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox providerName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Infragistics.Win.Misc.UltraValidator validator;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox managerCommunity;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor managerPort;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox managerAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
    }
}