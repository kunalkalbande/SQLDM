namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddEditvCenterConnection
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
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("vCenterInfo");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("vCenterName");
            Infragistics.Win.Misc.ValidationGroup validationGroup3 = new Infragistics.Win.Misc.ValidationGroup("vCenterAddress");
            this.btnCancel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.btnOK = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.txtHostPassword = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.txtHostUser = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.txtHostAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.txtHostName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.btnTest = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.statusMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.vCenterHostsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vCenterHostsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 148);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(137, 148);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // validator
            // 
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "vCenterInfo";
            validationGroup2.Key = "vCenterName";
            validationGroup3.Key = "vCenterAddress";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationGroups.Add(validationGroup2);
            this.validator.ValidationGroups.Add(validationGroup3);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // txtHostPassword
            // 
            this.txtHostPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vCenterHostsBindingSource, "vcPassword", true));
            this.txtHostPassword.Location = new System.Drawing.Point(92, 84);
            this.txtHostPassword.MaxLength = 128;
            this.txtHostPassword.Name = "txtHostPassword";
            this.txtHostPassword.Size = new System.Drawing.Size(200, 20);
            this.txtHostPassword.TabIndex = 3;
            this.txtHostPassword.UseSystemPasswordChar = true;
            this.validator.GetValidationSettings(this.txtHostPassword).NotificationSettings.Caption = "Required";
            this.validator.GetValidationSettings(this.txtHostPassword).NotificationSettings.Text = "A valid password for the specified user is requried.";
            this.validator.GetValidationSettings(this.txtHostPassword).ValidationGroupKey = "vCenterInfo";
            this.txtHostPassword.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // txtHostUser
            // 
            this.txtHostUser.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vCenterHostsBindingSource, "vcUser", true));
            this.txtHostUser.Location = new System.Drawing.Point(92, 58);
            this.txtHostUser.MaxLength = 128;
            this.txtHostUser.Name = "txtHostUser";
            this.txtHostUser.Size = new System.Drawing.Size(200, 20);
            this.txtHostUser.TabIndex = 2;
            this.validator.GetValidationSettings(this.txtHostUser).NotificationSettings.Caption = "Required";
            this.validator.GetValidationSettings(this.txtHostUser).NotificationSettings.Text = "A valid user name for the specified server is required.";
            this.validator.GetValidationSettings(this.txtHostUser).ValidationGroupKey = "vCenterInfo";
            this.txtHostUser.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // txtHostAddress
            // 
            this.txtHostAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vCenterHostsBindingSource, "vcAddress", true));
            this.txtHostAddress.Location = new System.Drawing.Point(92, 6);
            this.txtHostAddress.MaxLength = 256;
            this.txtHostAddress.Name = "txtHostAddress";
            this.txtHostAddress.Size = new System.Drawing.Size(200, 20);
            this.txtHostAddress.TabIndex = 0;
            this.validator.GetValidationSettings(this.txtHostAddress).NotificationSettings.Caption = "Required";
            this.validator.GetValidationSettings(this.txtHostAddress).NotificationSettings.Text = "A valid address for this server is required and cannot already be used.";
            this.validator.GetValidationSettings(this.txtHostAddress).ValidationGroupKey = "vCenterAddress";
            this.txtHostAddress.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // txtHostName
            // 
            this.txtHostName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.vCenterHostsBindingSource, "vcName", true));
            this.txtHostName.Location = new System.Drawing.Point(92, 32);
            this.txtHostName.MaxLength = 256;
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(200, 20);
            this.txtHostName.TabIndex = 1;
            this.validator.GetValidationSettings(this.txtHostName).NotificationSettings.Caption = "Required";
            this.validator.GetValidationSettings(this.txtHostName).NotificationSettings.Text = "A descriptive name is required and cannot already be used.";
            this.validator.GetValidationSettings(this.txtHostName).ValidationGroupKey = "vCenterName";
            this.txtHostName.TextChanged += new System.EventHandler(this.onTextChanged);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnTest.Location = new System.Drawing.Point(10, 153);  //4.12 DarkTheme UI Alignment issue  Babita Manral
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(74, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "User";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Name";
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // statusMessage
            // 
            this.statusMessage.Location = new System.Drawing.Point(123, 113);
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(163, 23);
            this.statusMessage.TabIndex = 48;
            this.statusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle.InnerCircleRadius = 5;
            this.loadingCircle.Location = new System.Drawing.Point(92, 113);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 12;
            this.loadingCircle.OuterCircleRadius = 11;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.Size = new System.Drawing.Size(25, 28);
            this.loadingCircle.SpokeThickness = 2;
            this.loadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle.TabIndex = 50;
            this.loadingCircle.TabStop = false;
            this.loadingCircle.Visible = false;
            // 
            // vCenterHostsBindingSource
            // 
            this.vCenterHostsBindingSource.DataSource = typeof(Idera.SQLdm.Common.VMware.vCenterHosts);
            // 
            // AddEditvCenterConnection
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(315, 181);
            this.Controls.Add(this.loadingCircle);
            this.Controls.Add(this.statusMessage);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtHostPassword);
            this.Controls.Add(this.txtHostUser);
            this.Controls.Add(this.txtHostAddress);
            this.Controls.Add(this.txtHostName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditvCenterConnection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Virtualization Host Configuration";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AddEditvCenterConnection_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AddEditvCenterConnection_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddEditvCenterConnection_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vCenterHostsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnCancel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnOK;
        private Infragistics.Win.Misc.UltraValidator validator;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnTest;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtHostPassword;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtHostUser;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtHostAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox txtHostName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private System.Windows.Forms.BindingSource vCenterHostsBindingSource;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusMessage;
        private MRG.Controls.UI.LoadingCircle loadingCircle;
    }
}