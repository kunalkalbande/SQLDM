namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class SmtpProviderConfigDialog
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Enter the mail servers name or its IP address.  Contact your network administrato" +
                    "r for help.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Sender name and e-mail are used as the from address in notification e-mails.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Sender name and e-mail are used as the from address in notification e-mails.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("Name");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("Server");
            Infragistics.Win.Misc.ValidationGroup validationGroup3 = new Infragistics.Win.Misc.ValidationGroup("Authentication");
            Infragistics.Win.Misc.ValidationGroup validationGroup4 = new Infragistics.Win.Misc.ValidationGroup("Sender");
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.timeoutEditor = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.serverPort = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.timeoutLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timeoutSlider = new System.Windows.Forms.TrackBar();
            this.serverAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.requiresAuthentication = new System.Windows.Forms.CheckBox();
            this.password = new System.Windows.Forms.TextBox();
            this.logonName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.fromAddress = new System.Windows.Forms.TextBox();
            this.fromName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.providerName = new System.Windows.Forms.TextBox();
            this.testButton = new System.Windows.Forms.Button();
            this.toolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.requiresSSL = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
            this.ultraGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 39);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(445, 326);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            this.ultraGroupBox2.Controls.Add(this.requiresSSL);
            this.ultraGroupBox2.Controls.Add(this.timeoutEditor);
            this.ultraGroupBox2.Controls.Add(this.serverPort);
            this.ultraGroupBox2.Controls.Add(this.timeoutLabel);
            this.ultraGroupBox2.Controls.Add(this.label5);
            this.ultraGroupBox2.Controls.Add(this.timeoutSlider);
            this.ultraGroupBox2.Controls.Add(this.serverAddress);
            this.ultraGroupBox2.Controls.Add(this.label3);
            this.ultraGroupBox2.Controls.Add(this.label4);
            this.ultraGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(440, 119);
            this.ultraGroupBox2.TabIndex = 0;
            this.ultraGroupBox2.Text = "Server Information";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // timeoutEditor
            // 
            this.timeoutEditor.AutoSize = false;
            this.timeoutEditor.Location = new System.Drawing.Point(246, 66);
            this.timeoutEditor.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.timeoutEditor.MaskInput = "nnn";
            this.timeoutEditor.MaxValue = 600;
            this.timeoutEditor.MinValue = 10;
            this.timeoutEditor.Name = "timeoutEditor";
            this.timeoutEditor.Nullable = true;
            this.timeoutEditor.Size = new System.Drawing.Size(54, 21);
            this.timeoutEditor.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.timeoutEditor.SpinWrap = true;
            this.timeoutEditor.TabIndex = 6;
            this.timeoutEditor.UseAppStyling = false;
            this.timeoutEditor.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.timeoutEditor.Value = 90;
            this.timeoutEditor.ValueChanged += new System.EventHandler(this.timeoutEditor_ValueChanged);
            this.timeoutEditor.BeforeExitEditMode += new Infragistics.Win.BeforeExitEditModeEventHandler(this.serverPort_BeforeExitEditMode);
            this.timeoutEditor.ValidationError += new Infragistics.Win.UltraWinEditors.UltraNumericEditorBase.ValidationErrorEventHandler(this.timeoutEditor_ValidationError);
            // 
            // serverPort
            // 
            this.serverPort.AutoSize = false;
            this.serverPort.Location = new System.Drawing.Point(84, 44);
            this.serverPort.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.serverPort.MaskInput = "nnnnn";
            this.serverPort.MaxValue = 65530;
            this.serverPort.MinValue = 2;
            this.serverPort.Name = "serverPort";
            this.serverPort.Nullable = true;
            this.serverPort.Size = new System.Drawing.Size(73, 21);
            this.serverPort.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.serverPort.SpinWrap = true;
            this.serverPort.TabIndex = 3;
            this.serverPort.UseAppStyling = false;
            this.serverPort.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.serverPort.BeforeExitEditMode += new Infragistics.Win.BeforeExitEditModeEventHandler(this.serverPort_BeforeExitEditMode);
            // 
            // timeoutLabel
            // 
            this.timeoutLabel.Location = new System.Drawing.Point(311, 74);
            this.timeoutLabel.Name = "timeoutLabel";
            this.timeoutLabel.Size = new System.Drawing.Size(119, 13);
            this.timeoutLabel.TabIndex = 7;
            this.timeoutLabel.Text = "1 minute 30 seconds";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Timeout:";
            // 
            // timeoutSlider
            // 
            this.timeoutSlider.LargeChange = 10;
            this.timeoutSlider.Location = new System.Drawing.Point(76, 72);
            this.timeoutSlider.Maximum = 600;
            this.timeoutSlider.Minimum = 10;
            this.timeoutSlider.Name = "timeoutSlider";
            this.timeoutSlider.Size = new System.Drawing.Size(160, 42);
            this.timeoutSlider.TabIndex = 5;
            this.timeoutSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.timeoutSlider.Value = 90;
            this.timeoutSlider.ValueChanged += new System.EventHandler(this.timeoutSlider_ValueChanged);
            // 
            // serverAddress
            // 
            this.serverAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverAddress.Location = new System.Drawing.Point(84, 19);
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(334, 20);
            this.serverAddress.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "Enter the mail servers name or its IP address.  Contact your network administrato" +
                "r for help.";
            this.toolTipManager.SetUltraToolTip(this.serverAddress, ultraToolTipInfo1);
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Caption = "Address Required";
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Text = "Please enter the name or IP Address of the SMTP Server";
            this.validator.GetValidationSettings(this.serverAddress).ValidationGroupKey = "Server";
            this.validator.GetValidationSettings(this.serverAddress).ValidationPropertyName = "Text";
            this.serverAddress.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Address:";
            // 
            // ultraGroupBox3
            // 
            this.ultraGroupBox3.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            this.ultraGroupBox3.Controls.Add(this.requiresAuthentication);
            this.ultraGroupBox3.Controls.Add(this.password);
            this.ultraGroupBox3.Controls.Add(this.logonName);
            this.ultraGroupBox3.Controls.Add(this.label6);
            this.ultraGroupBox3.Controls.Add(this.label7);
            this.ultraGroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox3.Location = new System.Drawing.Point(3, 128);
            this.ultraGroupBox3.Name = "ultraGroupBox3";
            this.ultraGroupBox3.Size = new System.Drawing.Size(440, 103);
            this.ultraGroupBox3.TabIndex = 1;
            this.ultraGroupBox3.Text = "Logon Information";
            this.ultraGroupBox3.UseAppStyling = false;
            // 
            // requiresAuthentication
            // 
            this.requiresAuthentication.AutoSize = true;
            this.requiresAuthentication.Location = new System.Drawing.Point(17, 21);
            this.requiresAuthentication.Name = "requiresAuthentication";
            this.requiresAuthentication.Size = new System.Drawing.Size(167, 17);
            this.requiresAuthentication.TabIndex = 0;
            this.requiresAuthentication.Text = "Server requires authentication";
            this.requiresAuthentication.UseVisualStyleBackColor = true;
            this.requiresAuthentication.CheckedChanged += new System.EventHandler(this.requiresAuthentication_CheckedChanged);
            // 
            // password
            // 
            this.password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.password.Location = new System.Drawing.Point(83, 70);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(335, 20);
            this.password.TabIndex = 4;
            this.password.UseSystemPasswordChar = true;
            this.validator.GetValidationSettings(this.password).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.password).NotificationSettings.Caption = "Password Required";
            this.validator.GetValidationSettings(this.password).NotificationSettings.Text = "Please enter the password used to authenticate to the SMTP Server";
            this.validator.GetValidationSettings(this.password).ValidationGroupKey = "Authentication";
            this.password.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // logonName
            // 
            this.logonName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logonName.Location = new System.Drawing.Point(83, 44);
            this.logonName.Name = "logonName";
            this.logonName.Size = new System.Drawing.Size(335, 20);
            this.logonName.TabIndex = 2;
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Caption = "User Name Required";
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Text = "Please enter the user name used to authenticate with the SMTP Server";
            this.validator.GetValidationSettings(this.logonName).ValidationGroupKey = "Authentication";
            this.logonName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Password:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "User Name:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularSolid;
            this.ultraGroupBox1.Controls.Add(this.fromAddress);
            this.ultraGroupBox1.Controls.Add(this.fromName);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox1.Location = new System.Drawing.Point(3, 237);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(440, 86);
            this.ultraGroupBox1.TabIndex = 2;
            this.ultraGroupBox1.Text = "Sender Information";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // fromAddress
            // 
            this.fromAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fromAddress.Location = new System.Drawing.Point(83, 43);
            this.fromAddress.Name = "fromAddress";
            this.fromAddress.Size = new System.Drawing.Size(335, 20);
            this.fromAddress.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "Sender name and e-mail are used as the from address in notification e-mails.";
            this.toolTipManager.SetUltraToolTip(this.fromAddress, ultraToolTipInfo2);
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Caption = "Sender Required";
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Text = "Please enter a valid e-mail address.";
            this.validator.GetValidationSettings(this.fromAddress).ValidationGroupKey = "Sender";
            this.fromAddress.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // fromName
            // 
            this.fromName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fromName.Location = new System.Drawing.Point(83, 17);
            this.fromName.Name = "fromName";
            this.fromName.Size = new System.Drawing.Size(335, 20);
            this.fromName.TabIndex = 1;
            this.fromName.Text = "SQL Diagnostic Manager";
            ultraToolTipInfo3.ToolTipText = "Sender name and e-mail are used as the from address in notification e-mails.";
            this.toolTipManager.SetUltraToolTip(this.fromName, ultraToolTipInfo3);
            this.fromName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "E-mail:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(364, 371);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(283, 371);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Provider Name:";
            // 
            // providerName
            // 
            this.providerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.providerName.Location = new System.Drawing.Point(90, 12);
            this.providerName.Name = "providerName";
            this.providerName.Size = new System.Drawing.Size(335, 20);
            this.providerName.TabIndex = 1;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Caption = "Provider Name Required";
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.providerName).NotificationSettings.Text = "Please enter a unique name for the SMTP Action Provider";
            this.validator.GetValidationSettings(this.providerName).ValidationGroupKey = "Name";
            this.validator.GetValidationSettings(this.providerName).ValidationPropertyName = "Text";
            this.providerName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(12, 371);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 3;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // toolTipManager
            // 
            this.toolTipManager.ContainingControl = this;
            this.toolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            // 
            // validator
            // 
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "Name";
            validationGroup2.Key = "Server";
            validationGroup3.Key = "Authentication";
            validationGroup4.Key = "Sender";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationGroups.Add(validationGroup2);
            this.validator.ValidationGroups.Add(validationGroup3);
            this.validator.ValidationGroups.Add(validationGroup4);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // requiresSSL
            // 
            this.requiresSSL.AutoSize = true;
            this.requiresSSL.Location = new System.Drawing.Point(17, 97);
            this.requiresSSL.Name = "requiresSSL";
            this.requiresSSL.Size = new System.Drawing.Size(120, 17);
            this.requiresSSL.TabIndex = 8;
            this.requiresSSL.Text = "Server requires SSL";
            this.requiresSSL.UseVisualStyleBackColor = true;
            // 
            // SmtpProviderConfigDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(451, 406);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.providerName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmtpProviderConfigDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SMTP Action Provider";
            this.Load += new System.EventHandler(this.SmtpProviderConfigDialog_Load);
            this.Shown += new System.EventHandler(this.SmtpProviderConfigDialog_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
            this.ultraGroupBox3.ResumeLayout(false);
            this.ultraGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private System.Windows.Forms.TextBox serverAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox fromAddress;
        private System.Windows.Forms.TextBox fromName;
        private System.Windows.Forms.Label timeoutLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar timeoutSlider;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox logonName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox providerName;
        private System.Windows.Forms.CheckBox requiresAuthentication;
        private System.Windows.Forms.Button testButton;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager toolTipManager;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor serverPort;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor timeoutEditor;
        private Infragistics.Win.Misc.UltraValidator validator;
        private System.Windows.Forms.CheckBox requiresSSL;
    }
}