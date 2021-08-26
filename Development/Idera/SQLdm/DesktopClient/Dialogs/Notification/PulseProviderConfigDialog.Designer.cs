namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class PulseProviderConfigDialog
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
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("PulsePort");
            Infragistics.Win.Misc.ValidationGroup validationGroup3 = new Infragistics.Win.Misc.ValidationGroup("PulseAddress");
            this.pulsePortSpinner = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label14 = new System.Windows.Forms.Label();
            this.pulseAddressTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.testButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.providerName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pulsePortSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pulsePortSpinner
            // 
            this.pulsePortSpinner.AutoSize = false;
            this.pulsePortSpinner.Location = new System.Drawing.Point(94, 56);
            this.pulsePortSpinner.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.pulsePortSpinner.MaskInput = "nnnnn";
            this.pulsePortSpinner.MaxValue = 65530;
            this.pulsePortSpinner.MinValue = 2;
            this.pulsePortSpinner.Name = "pulsePortSpinner";
            this.pulsePortSpinner.Nullable = true;
            this.pulsePortSpinner.Size = new System.Drawing.Size(73, 21);
            this.pulsePortSpinner.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.pulsePortSpinner.SpinWrap = true;
            this.pulsePortSpinner.TabIndex = 2;
            this.pulsePortSpinner.UseAppStyling = false;
            this.pulsePortSpinner.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.pulsePortSpinner.Value = 5168;
            this.pulsePortSpinner.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(23, 34);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "Address:";
            // 
            // pulseAddressTextBox
            // 
            this.pulseAddressTextBox.Location = new System.Drawing.Point(94, 31);
            this.pulseAddressTextBox.Name = "pulseAddressTextBox";
            this.pulseAddressTextBox.Size = new System.Drawing.Size(318, 20);
            this.pulseAddressTextBox.TabIndex = 1;
            this.validator.GetValidationSettings(this.pulseAddressTextBox).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.pulseAddressTextBox).ValidationGroupKey = "PulseAddress";
            this.validator.GetValidationSettings(this.pulseAddressTextBox).ValidationPropertyName = "Text";
            this.pulseAddressTextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(23, 60);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Port:";
            this.label13.Visible = false;
            // 
            // testButton
            // 
            this.testButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testButton.Location = new System.Drawing.Point(12, 135);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 3;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(295, 135);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(376, 135);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // validator
            // 
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "Name";
            validationGroup2.Key = "PulsePort";
            validationGroup3.Key = "PulseAddress";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationGroups.Add(validationGroup2);
            this.validator.ValidationGroups.Add(validationGroup3);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // providerName
            // 
            this.providerName.Location = new System.Drawing.Point(106, 16);
            this.providerName.Name = "providerName";
            this.providerName.Size = new System.Drawing.Size(318, 20);
            this.providerName.TabIndex = 0;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Caption = "Provider Name Required";
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Text = "Please enter a unique name for the Action Provider";
            this.validator.GetValidationSettings(this.providerName).ValidationGroupKey = "Name";
            this.validator.GetValidationSettings(this.providerName).ValidationPropertyName = "Text";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Provider Name:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            this.ultraGroupBox1.Controls.Add(this.pulseAddressTextBox);
            this.ultraGroupBox1.Controls.Add(this.label14);
            this.ultraGroupBox1.Controls.Add(this.label13);
            this.ultraGroupBox1.Controls.Add(this.pulsePortSpinner);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 45);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(438, 84);
            this.ultraGroupBox1.TabIndex = 17;
            this.ultraGroupBox1.Text = "Server Information";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // PulseProviderConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(463, 170);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.providerName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.testButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PulseProviderConfigDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Newsfeed Action Provider";
            this.Load += new System.EventHandler(this.PulseProviderConfigDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pulsePortSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraNumericEditor pulsePortSpinner;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox pulseAddressTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Infragistics.Win.Misc.UltraValidator validator;
        private System.Windows.Forms.TextBox providerName;
        private System.Windows.Forms.Label label8;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
    }
}