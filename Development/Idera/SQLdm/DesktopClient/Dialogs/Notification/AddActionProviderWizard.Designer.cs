using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class AddActionProviderWizard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddActionProviderWizard));
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("Server");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("Authentication");
            Infragistics.Win.Misc.ValidationGroup validationGroup3 = new Infragistics.Win.Misc.ValidationGroup("Sender");
            Infragistics.Win.Misc.ValidationGroup validationGroup4 = new Infragistics.Win.Misc.ValidationGroup("Manager");
            Infragistics.Win.Misc.ValidationGroup validationGroup5 = new Infragistics.Win.Misc.ValidationGroup("PulseServer");
            this.wizardFramework = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CusotmWizard();
            this.welcomePage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomIntroductionPage();
            this.hideWelcomePageCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.indroductoryTextLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chooseProviderTypePage = new Divelements.WizardFramework.WizardPage();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.providerType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.providerName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.pulseConfig = new Divelements.WizardFramework.WizardPage();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.pulsePortSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.pulseAddressTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.testPulseProvider = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.smtpConfig = new Divelements.WizardFramework.WizardPage();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.testSmtpButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.ultraGroupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraGroupBox();
            this.requiresSSL = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.timeoutEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.serverPort = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.timeoutLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.timeoutSlider = new System.Windows.Forms.TrackBar();
            this.serverAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGroupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraGroupBox();
            this.requiresAuthentication = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.password = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.logonName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGroupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraGroupBox();
            this.fromAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.fromName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.snmpConfig = new Divelements.WizardFramework.WizardPage();
            this.tableLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.testSnmpButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.managerAddress = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.managerPort = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraNumericEditor();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.managerCommunity = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.validator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.wizardFramework.SuspendLayout();
            this.welcomePage.SuspendLayout();
            this.chooseProviderTypePage.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.providerType)).BeginInit();
            this.pulseConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pulsePortSpinner)).BeginInit();
            this.smtpConfig.SuspendLayout();
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
            this.snmpConfig.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.managerPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardFramework
            // 
            this.wizardFramework.AnimatePageTransitions = false;
            this.wizardFramework.BannerImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Provider_icon_49x49;
            this.wizardFramework.Controls.Add(this.welcomePage);
            this.wizardFramework.Controls.Add(this.chooseProviderTypePage);
            this.wizardFramework.Controls.Add(this.smtpConfig);
            this.wizardFramework.Controls.Add(this.snmpConfig);
            this.wizardFramework.Controls.Add(this.pulseConfig);
            this.wizardFramework.Cursor = System.Windows.Forms.Cursors.Default;
            this.wizardFramework.Location = new System.Drawing.Point(0, 0);
            this.wizardFramework.MarginImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Provider_Wizard_164x477;
            this.wizardFramework.Name = "wizardFramework";
            this.wizardFramework.OwnerForm = this;
            this.wizardFramework.Size = new System.Drawing.Size(584, 551);
            this.wizardFramework.TabIndex = 0;
            this.wizardFramework.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizardFramework.Finish += new System.EventHandler(this.wizardFramework_Finish);
            this.wizardFramework.BackColor = backColor;
            // 
            // welcomePage
            // 
            this.welcomePage.Controls.Add(this.hideWelcomePageCheckbox);
            this.welcomePage.Controls.Add(this.indroductoryTextLabel1);
            this.welcomePage.IntroductionText = "";
            this.welcomePage.Location = new System.Drawing.Point(175, 71);
            this.welcomePage.Name = "welcomePage";
            this.welcomePage.NextPage = this.chooseProviderTypePage;
            this.welcomePage.ProceedText = "";
            this.welcomePage.Size = new System.Drawing.Size(387, 422);
            this.welcomePage.TabIndex = 1004;
            this.welcomePage.Text = "Welcome to the Alert Communications Wizard";
            this.welcomePage.BackColor = backColor;
            // 
            // hideWelcomePageCheckbox
            // 
            this.hideWelcomePageCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hideWelcomePageCheckbox.AutoSize = true;
            this.hideWelcomePageCheckbox.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.HideAddActionProviderWelcomePage;
            this.hideWelcomePageCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "HideAddActionProviderWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hideWelcomePageCheckbox.Location = new System.Drawing.Point(3, 402);
            this.hideWelcomePageCheckbox.Name = "hideWelcomePageCheckbox";
            this.hideWelcomePageCheckbox.Size = new System.Drawing.Size(199, 17);
            this.hideWelcomePageCheckbox.TabIndex = 0;
            this.hideWelcomePageCheckbox.Text = "Don\'t show this welcome page again";
            this.hideWelcomePageCheckbox.UseVisualStyleBackColor = true;
            this.hideWelcomePageCheckbox.CheckedChanged += new System.EventHandler(this.hideWelcomePageCheckBox_CheckedChanged);
            // 
            // indroductoryTextLabel1
            // 
            this.indroductoryTextLabel1.Location = new System.Drawing.Point(3, 3);
            this.indroductoryTextLabel1.Name = "indroductoryTextLabel1";
            this.indroductoryTextLabel1.Size = new System.Drawing.Size(388, 38);
            this.indroductoryTextLabel1.TabIndex = 0;
            this.indroductoryTextLabel1.Text = "Use this wizard to send SQLDM alerts to an email address (SMTP) or to your networ" +
                "k management service application (SNMP).";
            // 
            // chooseProviderTypePage
            // 
            this.chooseProviderTypePage.Controls.Add(this.tableLayoutPanel2);
            this.chooseProviderTypePage.Description = "Select the type of action provider to send Alerts to and give it a name.";
            this.chooseProviderTypePage.Location = new System.Drawing.Point(11, 71);
            this.chooseProviderTypePage.Name = "chooseProviderTypePage";
            this.chooseProviderTypePage.NextPage = this.pulseConfig;
            this.chooseProviderTypePage.PreviousPage = this.welcomePage;
            this.chooseProviderTypePage.Size = new System.Drawing.Size(562, 422);
            this.chooseProviderTypePage.TabIndex = 1007;
            this.chooseProviderTypePage.Text = "Choose Provider Type";
            this.chooseProviderTypePage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.chooseProviderTypePage_BeforeMoveNext);
            this.chooseProviderTypePage.BeforeDisplay += new System.EventHandler(this.chooseProviderTypePage_BeforeDisplay);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label1, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.informationBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.providerType, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.providerName, 2, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(11, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(541, 107);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Provider Name:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Provider Type:";
            // 
            // informationBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.informationBox1, 3);
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox1.Location = new System.Drawing.Point(3, 3);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(535, 45);
            this.informationBox1.TabIndex = 0;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // providerType
            // 
            this.providerType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.providerType.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Standard;
            this.providerType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem3.DataValue = "EMPTY";
            valueListItem3.DisplayText = "< Select provider type >";
            valueListItem1.DataValue = "SMTP";
            valueListItem1.DisplayText = "Simple Mail Transfer Protocol (SMTP)";
            valueListItem2.DataValue = "SNMP";
            valueListItem2.DisplayText = "Simple Network Management Protocol (SNMP)";
            valueListItem4.DataValue = "News";
            valueListItem4.DisplayText = "IDERA Newsfeed";
            this.providerType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem1,
            valueListItem2,
            valueListItem4});
            this.providerType.Location = new System.Drawing.Point(149, 54);
            this.providerType.Name = "providerType";
            this.providerType.Size = new System.Drawing.Size(335, 21);
            this.providerType.TabIndex = 6;
            this.providerType.UseAppStyling = false;
            this.providerType.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.providerType.SelectionChanged += new System.EventHandler(this.providerType_SelectionChanged);
            // 
            // providerName
            // 
            this.providerName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.providerName.Location = new System.Drawing.Point(149, 82);
            this.providerName.Name = "providerName";
            this.providerName.Size = new System.Drawing.Size(335, 20);
            this.providerName.TabIndex = 7;
            this.providerName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // pulseConfig
            // 
            this.pulseConfig.Controls.Add(this.label13);
            this.pulseConfig.Controls.Add(this.pulsePortSpinner);
            this.pulseConfig.Controls.Add(this.pulseAddressTextBox);
            this.pulseConfig.Controls.Add(this.label14);
            this.pulseConfig.Controls.Add(this.testPulseProvider);
            this.pulseConfig.Description = "Specify the name of the IDERA News server that will receive alerts from SQLdm.";
            this.pulseConfig.Location = new System.Drawing.Point(11, 71);
            this.pulseConfig.Name = "pulseConfig";
            this.pulseConfig.PreviousPage = this.chooseProviderTypePage;
            this.pulseConfig.Size = new System.Drawing.Size(562, 422);
            this.pulseConfig.TabIndex = 1010;
            this.pulseConfig.Text = "Newsfeed Configuration";
            this.pulseConfig.BeforeDisplay += new System.EventHandler(this.pulseConfig_BeforeDisplay);
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(61, 94);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Port:";
            this.label13.Visible = false;
            // 
            // pulsePortSpinner
            // 
            this.pulsePortSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pulsePortSpinner.AutoSize = false;
            this.pulsePortSpinner.Location = new System.Drawing.Point(131, 90);
            this.pulsePortSpinner.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.pulsePortSpinner.MaskInput = "nnnnn";
            this.pulsePortSpinner.MaxValue = 65530;
            this.pulsePortSpinner.MinValue = 2;
            this.pulsePortSpinner.Name = "pulsePortSpinner";
            this.pulsePortSpinner.Nullable = true;
            this.pulsePortSpinner.Size = new System.Drawing.Size(73, 21);
            this.pulsePortSpinner.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.pulsePortSpinner.SpinWrap = true;
            this.pulsePortSpinner.TabIndex = 14;
            this.pulsePortSpinner.UseAppStyling = false;
            this.pulsePortSpinner.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.pulsePortSpinner.Value = 5168;
            this.pulsePortSpinner.Visible = false;
            // 
            // pulseAddressTextBox
            // 
            this.pulseAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pulseAddressTextBox.Location = new System.Drawing.Point(131, 64);
            this.pulseAddressTextBox.Name = "pulseAddressTextBox";
            this.pulseAddressTextBox.Size = new System.Drawing.Size(334, 20);
            this.pulseAddressTextBox.TabIndex = 12;
            this.validator.GetValidationSettings(this.pulseAddressTextBox).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.pulseAddressTextBox).NotificationSettings.Caption = "Address Required";
            this.validator.GetValidationSettings(this.pulseAddressTextBox).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.pulseAddressTextBox).NotificationSettings.Text = "Please enter the name or IP address of the SMTP server.";
            this.validator.GetValidationSettings(this.pulseAddressTextBox).ValidationGroupKey = "PulseServer";
            this.validator.GetValidationSettings(this.pulseAddressTextBox).ValidationPropertyName = "Text";
            this.pulseAddressTextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(61, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "Address:";
            // 
            // testPulseProvider
            // 
            this.testPulseProvider.Location = new System.Drawing.Point(484, 396);
            this.testPulseProvider.Name = "testPulseProvider";
            this.testPulseProvider.Size = new System.Drawing.Size(75, 23);
            this.testPulseProvider.TabIndex = 9;
            this.testPulseProvider.Text = "Test";
            this.testPulseProvider.UseVisualStyleBackColor = true;
            this.testPulseProvider.Click += new System.EventHandler(this.testPulseProvider_Click);
            // 
            // smtpConfig
            // 
            this.smtpConfig.Controls.Add(this.tableLayoutPanel1);
            this.smtpConfig.Controls.Add(this.informationBox2);
            this.smtpConfig.Description = "Specify the mail server and authentication information for the SMTP server you wa" +
                "nt to use.";
            this.smtpConfig.Location = new System.Drawing.Point(11, 71);
            this.smtpConfig.Name = "smtpConfig";
            this.smtpConfig.PreviousPage = this.chooseProviderTypePage;
            this.smtpConfig.Size = new System.Drawing.Size(562, 422);
            this.smtpConfig.TabIndex = 1008;
            this.smtpConfig.Text = "SMTP Configuration";
            this.smtpConfig.BeforeDisplay += new System.EventHandler(this.smtpConfig_BeforeDisplay);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.testSmtpButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ultraGroupBox1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(58, 48);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(445, 337);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // testSmtpButton
            // 
            this.testSmtpButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.testSmtpButton.Location = new System.Drawing.Point(368, 303);
            this.testSmtpButton.Name = "testSmtpButton";
            this.testSmtpButton.Size = new System.Drawing.Size(75, 23);
            this.testSmtpButton.TabIndex = 11;
            this.testSmtpButton.Text = "Test";
            this.testSmtpButton.UseVisualStyleBackColor = true;
            this.testSmtpButton.Click += new System.EventHandler(this.testSmtpButton_Click);
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
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(440, 100);
            this.ultraGroupBox2.TabIndex = 0;
            this.ultraGroupBox2.Text = "Server Information";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // requiresSSL
            // 
            this.requiresSSL.AutoSize = true;
            this.requiresSSL.Location = new System.Drawing.Point(246, 45);
            this.requiresSSL.Name = "requiresSSL";
            this.requiresSSL.Size = new System.Drawing.Size(120, 17);
            this.requiresSSL.TabIndex = 3;
            this.requiresSSL.Text = "Server requires SSL";
            this.requiresSSL.UseVisualStyleBackColor = true;
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
            this.timeoutEditor.TabIndex = 5;
            this.timeoutEditor.UseAppStyling = false;
            this.timeoutEditor.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.timeoutEditor.Value = 90;
            this.timeoutEditor.ValueChanged += new System.EventHandler(this.timeoutEditor_ValueChanged);
            this.timeoutEditor.BeforeExitEditMode += new Infragistics.Win.BeforeExitEditModeEventHandler(this.generic_BeforeExitEditMode);
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
            this.serverPort.TabIndex = 2;
            this.serverPort.UseAppStyling = false;
            this.serverPort.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.serverPort.BeforeExitEditMode += new Infragistics.Win.BeforeExitEditModeEventHandler(this.generic_BeforeExitEditMode);
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
            this.timeoutSlider.Size = new System.Drawing.Size(160, 45);
            this.timeoutSlider.TabIndex = 4;
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
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Caption = "Address Required";
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.serverAddress).NotificationSettings.Text = "Please enter the name or IP address of the SMTP server.";
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
            this.ultraGroupBox3.Location = new System.Drawing.Point(3, 109);
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
            this.requiresAuthentication.TabIndex = 6;
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
            this.password.TabIndex = 8;
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // logonName
            // 
            this.logonName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logonName.Location = new System.Drawing.Point(83, 44);
            this.logonName.Name = "logonName";
            this.logonName.Size = new System.Drawing.Size(335, 20);
            this.logonName.TabIndex = 7;
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Caption = "User Name Required";
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.logonName).NotificationSettings.Text = "Please enter the user name used to authenticate with the SMTP server.";
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
            this.ultraGroupBox1.Controls.Add(this.label8);
            this.ultraGroupBox1.Controls.Add(this.label9);
            this.ultraGroupBox1.Location = new System.Drawing.Point(3, 218);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(440, 72);
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
            this.fromAddress.TabIndex = 10;
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Caption = "Sender Required";
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.fromAddress).NotificationSettings.Text = "Please enter a valid email address.";
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
            this.fromName.TabIndex = 9;
            this.fromName.Text = "SQL Diagnostic Manager";
            this.fromName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "E-mail:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Name:";
            // 
            // informationBox2
            // 
            this.informationBox2.Location = new System.Drawing.Point(14, 5);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(544, 46);
            this.informationBox2.TabIndex = 6;
            this.informationBox2.Text = "Note: If the server you are specifiying requires authentication before sending ou" +
                "t SMTP messages, check the \'Server requires authentication\' box and provide the " +
                "appropriate credentials.\r\n";
            // 
            // snmpConfig
            // 
            this.snmpConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.snmpConfig.Controls.Add(this.tableLayoutPanel3);
            this.snmpConfig.Controls.Add(this.informationBox3);
            this.snmpConfig.Description = "Specify the server, port and community name of the network management console you" +
                " would like to have SQLDM alert messages (SNMP Trap messages) sent to.";
            this.snmpConfig.Location = new System.Drawing.Point(11, 71);
            this.snmpConfig.Name = "snmpConfig";
            this.snmpConfig.PreviousPage = this.chooseProviderTypePage;
            this.snmpConfig.Size = new System.Drawing.Size(562, 422);
            this.snmpConfig.TabIndex = 1009;
            this.snmpConfig.Text = "SNMP Configuration";
            this.snmpConfig.BeforeDisplay += new System.EventHandler(this.snmpConfig_BeforeDisplay);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel3.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.testSnmpButton, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.managerAddress, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.managerPort, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label11, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.managerCommunity, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(73, 50);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(479, 115);
            this.tableLayoutPanel3.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Address:";
            // 
            // testSnmpButton
            // 
            this.testSnmpButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.testSnmpButton.Location = new System.Drawing.Point(329, 85);
            this.testSnmpButton.Name = "testSnmpButton";
            this.testSnmpButton.Size = new System.Drawing.Size(75, 23);
            this.testSnmpButton.TabIndex = 8;
            this.testSnmpButton.Text = "Test";
            this.testSnmpButton.UseVisualStyleBackColor = true;
            this.testSnmpButton.Click += new System.EventHandler(this.testSnmpButton_Click);
            // 
            // managerAddress
            // 
            this.managerAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.managerAddress.Location = new System.Drawing.Point(70, 3);
            this.managerAddress.Name = "managerAddress";
            this.managerAddress.Size = new System.Drawing.Size(334, 20);
            this.managerAddress.TabIndex = 2;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Caption = "Address Required";
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            this.validator.GetValidationSettings(this.managerAddress).NotificationSettings.Text = "The address of the Network Management System must be provided.  The value specifi" +
                "ed must be a valid computer name, or valid IPv4 address.";
            this.validator.GetValidationSettings(this.managerAddress).ValidationGroupKey = "Manager";
            this.validator.GetValidationSettings(this.managerAddress).ValidationPropertyName = "Text";
            this.managerAddress.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // managerPort
            // 
            this.managerPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.managerPort.AutoSize = false;
            this.managerPort.Location = new System.Drawing.Point(70, 29);
            this.managerPort.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.managerPort.MaskInput = "nnnnn";
            this.managerPort.MaxValue = 65530;
            this.managerPort.MinValue = 2;
            this.managerPort.Name = "managerPort";
            this.managerPort.Size = new System.Drawing.Size(73, 21);
            this.managerPort.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.managerPort.SpinWrap = true;
            this.managerPort.TabIndex = 4;
            this.managerPort.UseAppStyling = false;
            this.managerPort.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.managerPort.Value = 162;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Port:";
            // 
            // managerCommunity
            // 
            this.managerCommunity.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.managerCommunity.Location = new System.Drawing.Point(70, 56);
            this.managerCommunity.Name = "managerCommunity";
            this.managerCommunity.Size = new System.Drawing.Size(334, 20);
            this.managerCommunity.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Community:";
            // 
            // informationBox3
            // 
            this.informationBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox3.Location = new System.Drawing.Point(14, 5);
            this.informationBox3.Name = "informationBox3";
            this.informationBox3.Size = new System.Drawing.Size(543, 45);
            this.informationBox3.TabIndex = 7;
            this.informationBox3.Text = "Note: If you are using community names within your network, specify the community" +
                " name you would like to use for this provider.";
            // 
            // validator
            // 
            this.validator.MessageBoxIcon = System.Windows.Forms.MessageBoxIcon.None;
            this.validator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            this.validator.NotificationSettings.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
            validationGroup1.Key = "Server";
            validationGroup2.Key = "Authentication";
            validationGroup3.Key = "Sender";
            validationGroup4.Key = "Manager";
            validationGroup5.Key = "PulseServer";
            this.validator.ValidationGroups.Add(validationGroup1);
            this.validator.ValidationGroups.Add(validationGroup2);
            this.validator.ValidationGroups.Add(validationGroup3);
            this.validator.ValidationGroups.Add(validationGroup4);
            this.validator.ValidationGroups.Add(validationGroup5);
            this.validator.ValidationTrigger = Infragistics.Win.Misc.ValidationTrigger.Programmatic;
            this.validator.Validating += new Infragistics.Win.Misc.ValidatingHandler(this.validator_Validating);
            // 
            // AddActionProviderWizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(584, 551);
            this.Controls.Add(this.wizardFramework);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddActionProviderWizard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Communications Wizard";
            this.BackColor = System.Drawing.Color.Red;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AddActionProviderWizard_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AddActionProviderWizard_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddActionProviderWizard_HelpRequested);
            this.wizardFramework.ResumeLayout(false);
            this.welcomePage.ResumeLayout(false);
            this.welcomePage.PerformLayout();
            this.chooseProviderTypePage.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.providerType)).EndInit();
            this.pulseConfig.ResumeLayout(false);
            this.pulseConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pulsePortSpinner)).EndInit();
            this.smtpConfig.ResumeLayout(false);
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
            this.snmpConfig.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.managerPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.validator)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizardFramework;
        private Divelements.WizardFramework.IntroductionPage welcomePage;
        private Divelements.WizardFramework.WizardPage chooseProviderTypePage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel indroductoryTextLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox hideWelcomePageCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Divelements.WizardFramework.WizardPage smtpConfig;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor timeoutEditor;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor serverPort;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel timeoutLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private System.Windows.Forms.TrackBar timeoutSlider;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox serverAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox requiresAuthentication;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox password;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logonName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox fromAddress;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox fromName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Divelements.WizardFramework.WizardPage snmpConfig;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testSmtpButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox3;
        private Infragistics.Win.Misc.UltraValidator validator;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testSnmpButton;
        private Divelements.WizardFramework.WizardPage pulseConfig;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testPulseProvider;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor pulsePortSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox pulseAddressTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox requiresSSL;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor providerType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox providerName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox managerAddress;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor managerPort;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox managerCommunity;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
    }
}
