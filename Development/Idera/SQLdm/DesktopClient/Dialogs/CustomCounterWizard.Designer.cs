namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Idera.SQLdm.DesktopClient.Properties;
    using System.Drawing;
    using Wintellect.PowerCollections;

    partial class CustomCounterWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomCounterWizard));
            this.wizard = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CusotmWizard();
            this.introPage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomIntroductionPage();
            this.hideWelcomePageCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.introductoryTextLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.counterTypePage = new Divelements.WizardFramework.WizardPage();
            this.groupBox6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.vmCounterButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.wmiCounterButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.sqlCounterButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.sqlBatchButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.azureCounterButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.wmiCounterPage = new Divelements.WizardFramework.WizardPage();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label30 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiManualModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.wmiBrowseModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.wmiLoadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.wmiStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiInfoButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.wmiManualModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.wmiInstanceNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.wmiCounterNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.wmiObjectNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiBrowseModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label32 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiShowAllCountersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.wmiShowPerfCountersOnlyButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.wmiServerComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.wmiObjectNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label24 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiInstanceNameLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiCounterNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label23 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wmiInstanceNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label29 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlCounterPage = new Divelements.WizardFramework.WizardPage();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.panel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.sqlLoadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.sqlStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlManualModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.sqlInstanceNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.sqlCounterNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.sqlObjectNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label25 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label26 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label27 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlBrowseModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.sqlServerComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.sqlCounterNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.counterSourceLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlObjectNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.sqlInstanceNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label28 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label31 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sqlManualModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.sqlBrowseModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.sqlBatchPage = new Divelements.WizardFramework.WizardPage();
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.sqlBatchTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.vmCounterPage = new Divelements.WizardFramework.WizardPage();
            this.panel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label40 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.vmManualModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.vmBrowseModeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.groupBox11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.vmLoadingCircle = new MRG.Controls.UI.LoadingCircle();
            this.vmStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.vmManualModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.vmInstanceNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.vmCounterNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.vmObjectNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label20 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label21 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label22 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.vmBrowseModeContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.vmServerComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.vmCounterNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label35 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label36 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label37 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label38 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.vmObjectNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.vmInstanceNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label39 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.calculationTypePage = new Divelements.WizardFramework.WizardPage();
            this.testButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.groupBox9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.counterScaleComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.label34 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label33 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.groupBox8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.calcTypeDeltaButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.calcTypeValueButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.label19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.counterNamePage = new Divelements.WizardFramework.WizardPage();
            this.groupBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.alertNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.alertDescriptionTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.alertCategoryComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.informationBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.alertDefinitionPage = new Divelements.WizardFramework.WizardPage();
            this.groupBox10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.enableAlertCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.informationBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.groupBox7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.comparisonTypeLessThanButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.comparisonTypeGreaterThanButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.informationBox6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.groupBox5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.infoThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.alertAdvancedButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.informationBox5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.warningThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.criticalThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.finishPage1 = new Divelements.WizardFramework.FinishPage();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureServerConfigPage = new Divelements.WizardFramework.WizardPage();
            this.azureServerConfigGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.azureServerContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.azureProfileComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureProfileLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureResourceNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureResourceNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureServerComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureServerLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureNameSpaceNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureResourceTypeNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureProfileButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.azureServerMetricsPage = new Divelements.WizardFramework.WizardPage();
            this.azureServerMetricsGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.azureServerMetricsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.azureMetricPageServerNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureMetricPageResourceGroupNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureMetricPageObjectNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureServerNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureObjectNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureNameSpaceLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureMetricNameLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureMetricNameComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureDatabaseLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureDbComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.sqlServerNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.databaseNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.azureMetricValueLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.counterIdentificationPage = new Divelements.WizardFramework.WizardPage();
            this.categoryComboBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.descriptionTextBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.nameTextBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.azureMetricLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.wizard.SuspendLayout();
            this.introPage.SuspendLayout();
            this.counterTypePage.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.wmiCounterPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.wmiManualModeContentPanel.SuspendLayout();
            this.wmiBrowseModeContentPanel.SuspendLayout();
            this.sqlCounterPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.sqlManualModeContentPanel.SuspendLayout();
            this.sqlBrowseModeContentPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.sqlBatchPage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.vmCounterPage.SuspendLayout();
            this.panel8.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.panel4.SuspendLayout();
            this.vmManualModeContentPanel.SuspendLayout();
            this.vmBrowseModeContentPanel.SuspendLayout();
            this.calculationTypePage.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.counterNamePage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.alertDefinitionPage.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalThresholdSpinner)).BeginInit();
            this.finishPage1.SuspendLayout();
            this.azureServerConfigPage.SuspendLayout();
            this.azureServerConfigGroupBox.SuspendLayout();
            this.azureServerContentPanel.SuspendLayout();
            this.azureServerMetricsPage.SuspendLayout();
            this.azureServerMetricsGroupBox.SuspendLayout();
            this.azureServerMetricsPanel.SuspendLayout();
            this.counterIdentificationPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.AnimatePageTransitions = false;
            this.wizard.BannerImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounterWizardBannerImage;
            this.wizard.Controls.Add(this.introPage);
            this.wizard.Controls.Add(this.counterTypePage);
            this.wizard.Controls.Add(this.wmiCounterPage);
            this.wizard.Controls.Add(this.sqlCounterPage);
            this.wizard.Controls.Add(this.sqlBatchPage);
            this.wizard.Controls.Add(this.counterNamePage);
            this.wizard.Controls.Add(this.alertDefinitionPage);
            this.wizard.Controls.Add(this.finishPage1);
            this.wizard.Controls.Add(this.calculationTypePage);
            this.wizard.Controls.Add(this.vmCounterPage);
            this.wizard.Controls.Add(this.azureServerConfigPage);
            this.wizard.Controls.Add(this.azureServerMetricsPage);
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.MarginImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounterWizardWelcomePageMarginImage;
            this.wizard.Name = "wizard";
            this.wizard.OwnerForm = this;
            this.wizard.Size = new System.Drawing.Size(609, 516);
            this.wizard.TabIndex = 0;
            this.wizard.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizard.Finish += new System.EventHandler(this.wizard_Finish);
            // 
            // introPage
            // 
            this.introPage.Controls.Add(this.hideWelcomePageCheckBox);
            this.introPage.Controls.Add(this.introductoryTextLabel2);
            this.introPage.IntroductionText = "This wizard helps you configure a custom counter that can be monitored by SQL dia" +
    "gnostic manager. This wizard will walk you through the following steps:";
            this.introPage.Location = new System.Drawing.Point(175, 71);
            this.introPage.Name = "introPage";
            this.introPage.NextPage = this.counterTypePage;
            this.introPage.ProceedText = "";
            this.introPage.Size = new System.Drawing.Size(412, 387);
            this.introPage.TabIndex = 1004;
            this.introPage.Text = "Welcome to the Custom Counter Wizard";
            this.introPage.BeforeDisplay += new System.EventHandler(this.introPage_BeforeDisplay);
            // 
            // hideWelcomePageCheckBox
            // 
            this.hideWelcomePageCheckBox.AutoSize = true;
            this.hideWelcomePageCheckBox.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.HideAddServersWizardWelcomePage;
            this.hideWelcomePageCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "HideAddServersWizardWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hideWelcomePageCheckBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hideWelcomePageCheckBox.Location = new System.Drawing.Point(0, 370);
            this.hideWelcomePageCheckBox.Name = "hideWelcomePageCheckBox";
            this.hideWelcomePageCheckBox.Size = new System.Drawing.Size(412, 17);
            this.hideWelcomePageCheckBox.TabIndex = 3;
            this.hideWelcomePageCheckBox.Text = "Don\'t show this welcome page again";
            this.hideWelcomePageCheckBox.UseVisualStyleBackColor = true;
            this.hideWelcomePageCheckBox.CheckedChanged += new System.EventHandler(this.hideWelcomePageCheckBox_CheckedChanged);
            // 
            // introductoryTextLabel2
            // 
            this.introductoryTextLabel2.Location = new System.Drawing.Point(20, 43);
            this.introductoryTextLabel2.Name = "introductoryTextLabel2";
            this.introductoryTextLabel2.Size = new System.Drawing.Size(383, 201);
            this.introductoryTextLabel2.TabIndex = 2;
            this.introductoryTextLabel2.Text = resources.GetString("introductoryTextLabel2.Text");
            // 
            // counterTypePage
            // 
            this.counterTypePage.Controls.Add(this.groupBox6);
            this.counterTypePage.Controls.Add(this.informationBox1);
            this.counterTypePage.Description = "Choose a counter type that you would like to monitor";
            this.counterTypePage.Location = new System.Drawing.Point(11, 71);
            this.counterTypePage.Name = "counterTypePage";
            this.counterTypePage.NextPage = this.wmiCounterPage;
            this.counterTypePage.PreviousPage = this.introPage;
            this.counterTypePage.Size = new System.Drawing.Size(587, 387);
            this.counterTypePage.TabIndex = 1010;
            this.counterTypePage.Text = "Select Counter Type";
            this.counterTypePage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.counterTypePage_BeforeMoveNext);
            this.counterTypePage.BeforeDisplay += new System.EventHandler(this.counterTypePage_BeforeDisplay);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.vmCounterButton);
            this.groupBox6.Controls.Add(this.wmiCounterButton);
            this.groupBox6.Controls.Add(this.sqlCounterButton);
            this.groupBox6.Controls.Add(this.sqlBatchButton);
            this.groupBox6.Controls.Add(this.azureCounterButton);
            this.groupBox6.Location = new System.Drawing.Point(51, 86);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(503, 124);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Select Counter Type";
            // 
            // vmCounterButton
            // 
            this.vmCounterButton.AutoSize = true;
            this.vmCounterButton.Location = new System.Drawing.Point(25, 82);
            this.vmCounterButton.Name = "vmCounterButton";
            this.vmCounterButton.Size = new System.Drawing.Size(138, 17);
            this.vmCounterButton.TabIndex = 16;
            this.vmCounterButton.Text = "Virtual Machine Counter";
            this.vmCounterButton.UseVisualStyleBackColor = true;
            // 
            // wmiCounterButton
            // 
            this.wmiCounterButton.AutoSize = true;
            this.wmiCounterButton.Checked = true;
            this.wmiCounterButton.Location = new System.Drawing.Point(25, 25);
            this.wmiCounterButton.Name = "wmiCounterButton";
            this.wmiCounterButton.Size = new System.Drawing.Size(146, 17);
            this.wmiCounterButton.TabIndex = 12;
            this.wmiCounterButton.TabStop = true;
            this.wmiCounterButton.Text = "Windows System Counter";
            this.wmiCounterButton.UseVisualStyleBackColor = true;
            // 
            // sqlCounterButton
            // 
            this.sqlCounterButton.AutoSize = true;
            this.sqlCounterButton.Location = new System.Drawing.Point(25, 44);
            this.sqlCounterButton.Name = "sqlCounterButton";
            this.sqlCounterButton.Size = new System.Drawing.Size(157, 17);
            this.sqlCounterButton.TabIndex = 14;
            this.sqlCounterButton.Text = "SQL Server System Counter";
            this.sqlCounterButton.UseVisualStyleBackColor = true;
            // 
            // sqlBatchButton
            // 
            this.sqlBatchButton.AutoSize = true;
            this.sqlBatchButton.Location = new System.Drawing.Point(25, 63);
            this.sqlBatchButton.Name = "sqlBatchButton";
            this.sqlBatchButton.Size = new System.Drawing.Size(291, 17);
            this.sqlBatchButton.TabIndex = 15;
            this.sqlBatchButton.Text = "Custom SQL Script (must return a single numerical value)";
            this.sqlBatchButton.UseVisualStyleBackColor = true;
            // 
            // azureCounterButton
            // 
            this.azureCounterButton.AutoSize = true;
            this.azureCounterButton.Location = new System.Drawing.Point(25, 101);
            this.azureCounterButton.Name = "azureCounterButton";
            this.azureCounterButton.Size = new System.Drawing.Size(163, 17);
            this.azureCounterButton.TabIndex = 17;
            this.azureCounterButton.Text = "Azure Server System Counter";
            this.azureCounterButton.UseVisualStyleBackColor = true;
            // 
            // informationBox1
            // 
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox1.Location = new System.Drawing.Point(0, 0);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(587, 79);
            this.informationBox1.TabIndex = 16;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // wmiCounterPage
            // 
            this.wmiCounterPage.Controls.Add(this.panel1);
            this.wmiCounterPage.Controls.Add(this.groupBox1);
            this.wmiCounterPage.Description = "Select the Windows System Counter you would like to monitor";
            this.wmiCounterPage.Location = new System.Drawing.Point(11, 71);
            this.wmiCounterPage.Name = "wmiCounterPage";
            this.wmiCounterPage.NextPage = this.sqlCounterPage;
            this.wmiCounterPage.PreviousPage = this.counterTypePage;
            this.wmiCounterPage.Size = new System.Drawing.Size(587, 387);
            this.wmiCounterPage.TabIndex = 1007;
            this.wmiCounterPage.Text = "Select Windows System Counter";
            this.wmiCounterPage.BeforeDisplay += new System.EventHandler(this.wmiCounterPage_BeforeDisplay);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label30);
            this.panel1.Controls.Add(this.wmiManualModeButton);
            this.panel1.Controls.Add(this.wmiBrowseModeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(587, 52);
            this.panel1.TabIndex = 32;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label30.Location = new System.Drawing.Point(316, 8);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(98, 13);
            this.label30.TabIndex = 32;
            this.label30.Text = "(Recommended)";
            // 
            // wmiManualModeButton
            // 
            this.wmiManualModeButton.AutoSize = true;
            this.wmiManualModeButton.Location = new System.Drawing.Point(11, 29);
            this.wmiManualModeButton.Name = "wmiManualModeButton";
            this.wmiManualModeButton.Size = new System.Drawing.Size(197, 17);
            this.wmiManualModeButton.TabIndex = 30;
            this.wmiManualModeButton.Text = "Manually enter counter configuration";
            this.wmiManualModeButton.UseVisualStyleBackColor = true;
            // 
            // wmiBrowseModeButton
            // 
            this.wmiBrowseModeButton.AutoSize = true;
            this.wmiBrowseModeButton.Checked = true;
            this.wmiBrowseModeButton.Location = new System.Drawing.Point(11, 6);
            this.wmiBrowseModeButton.Name = "wmiBrowseModeButton";
            this.wmiBrowseModeButton.Size = new System.Drawing.Size(309, 17);
            this.wmiBrowseModeButton.TabIndex = 31;
            this.wmiBrowseModeButton.TabStop = true;
            this.wmiBrowseModeButton.Text = "Select a counter from an existing monitored SQL Server host";
            this.wmiBrowseModeButton.UseVisualStyleBackColor = true;
            this.wmiBrowseModeButton.CheckedChanged += new System.EventHandler(this.wmiBrowseModeButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.wmiManualModeContentPanel);
            this.groupBox1.Controls.Add(this.wmiBrowseModeContentPanel);
            this.groupBox1.Location = new System.Drawing.Point(11, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(566, 320);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Counter";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.wmiLoadingCircle);
            this.panel2.Controls.Add(this.wmiStatusLabel);
            this.panel2.Controls.Add(this.wmiInfoButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 258);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(560, 57);
            this.panel2.TabIndex = 35;
            // 
            // wmiLoadingCircle
            // 
            this.wmiLoadingCircle.Active = false;
            this.wmiLoadingCircle.Color = System.Drawing.Color.Silver;
            this.wmiLoadingCircle.InnerCircleRadius = 5;
            this.wmiLoadingCircle.Location = new System.Drawing.Point(111, 24);
            this.wmiLoadingCircle.Name = "wmiLoadingCircle";
            this.wmiLoadingCircle.NumberSpoke = 12;
            this.wmiLoadingCircle.OuterCircleRadius = 11;
            this.wmiLoadingCircle.RotationSpeed = 20;
            this.wmiLoadingCircle.Size = new System.Drawing.Size(27, 23);
            this.wmiLoadingCircle.SpokeThickness = 2;
            this.wmiLoadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.wmiLoadingCircle.TabIndex = 25;
            this.wmiLoadingCircle.Visible = false;
            // 
            // wmiStatusLabel
            // 
            this.wmiStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wmiStatusLabel.Location = new System.Drawing.Point(144, 29);
            this.wmiStatusLabel.Name = "wmiStatusLabel";
            this.wmiStatusLabel.Size = new System.Drawing.Size(413, 23);
            this.wmiStatusLabel.TabIndex = 24;
            this.wmiStatusLabel.Text = "{0}";
            // 
            // wmiInfoButton
            // 
            this.wmiInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiInfoButton.Enabled = false;
            this.wmiInfoButton.Location = new System.Drawing.Point(465, 0);
            this.wmiInfoButton.Name = "wmiInfoButton";
            this.wmiInfoButton.Size = new System.Drawing.Size(75, 23);
            this.wmiInfoButton.TabIndex = 39;
            this.wmiInfoButton.Text = "Details...";
            this.wmiInfoButton.UseVisualStyleBackColor = true;
            this.wmiInfoButton.Click += new System.EventHandler(this.wmiInfoButton_Click);
            // 
            // wmiManualModeContentPanel
            // 
            this.wmiManualModeContentPanel.Controls.Add(this.wmiInstanceNameTextBox);
            this.wmiManualModeContentPanel.Controls.Add(this.wmiCounterNameTextBox);
            this.wmiManualModeContentPanel.Controls.Add(this.wmiObjectNameTextBox);
            this.wmiManualModeContentPanel.Controls.Add(this.label13);
            this.wmiManualModeContentPanel.Controls.Add(this.label12);
            this.wmiManualModeContentPanel.Controls.Add(this.label10);
            this.wmiManualModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.wmiManualModeContentPanel.Location = new System.Drawing.Point(3, 177);
            this.wmiManualModeContentPanel.Name = "wmiManualModeContentPanel";
            this.wmiManualModeContentPanel.Size = new System.Drawing.Size(560, 81);
            this.wmiManualModeContentPanel.TabIndex = 34;
            this.wmiManualModeContentPanel.Visible = false;
            // 
            // wmiInstanceNameTextBox
            // 
            this.wmiInstanceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiInstanceNameTextBox.Location = new System.Drawing.Point(111, 55);
            this.wmiInstanceNameTextBox.Name = "wmiInstanceNameTextBox";
            this.wmiInstanceNameTextBox.Size = new System.Drawing.Size(429, 20);
            this.wmiInstanceNameTextBox.TabIndex = 20;
            this.wmiInstanceNameTextBox.TextChanged += new System.EventHandler(this.wmiInstanceNameTextBox_TextChanged);
            // 
            // wmiCounterNameTextBox
            // 
            this.wmiCounterNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiCounterNameTextBox.Location = new System.Drawing.Point(111, 29);
            this.wmiCounterNameTextBox.Name = "wmiCounterNameTextBox";
            this.wmiCounterNameTextBox.Size = new System.Drawing.Size(429, 20);
            this.wmiCounterNameTextBox.TabIndex = 19;
            this.wmiCounterNameTextBox.TextChanged += new System.EventHandler(this.wmiCounterNameTextBox_TextChanged);
            // 
            // wmiObjectNameTextBox
            // 
            this.wmiObjectNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiObjectNameTextBox.Location = new System.Drawing.Point(111, 3);
            this.wmiObjectNameTextBox.Name = "wmiObjectNameTextBox";
            this.wmiObjectNameTextBox.Size = new System.Drawing.Size(429, 20);
            this.wmiObjectNameTextBox.TabIndex = 18;
            this.wmiObjectNameTextBox.TextChanged += new System.EventHandler(this.wmiObjectNameTextBox_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(27, 6);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Object Name:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 32);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Counter Name:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 58);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Instance Name:";
            // 
            // wmiBrowseModeContentPanel
            // 
            this.wmiBrowseModeContentPanel.Controls.Add(this.label32);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiShowAllCountersButton);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiShowPerfCountersOnlyButton);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiServerComboBox);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiObjectNameComboBox);
            this.wmiBrowseModeContentPanel.Controls.Add(this.label15);
            this.wmiBrowseModeContentPanel.Controls.Add(this.label24);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiInstanceNameLabel1);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiCounterNameComboBox);
            this.wmiBrowseModeContentPanel.Controls.Add(this.label23);
            this.wmiBrowseModeContentPanel.Controls.Add(this.wmiInstanceNameComboBox);
            this.wmiBrowseModeContentPanel.Controls.Add(this.label29);
            this.wmiBrowseModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.wmiBrowseModeContentPanel.Location = new System.Drawing.Point(3, 16);
            this.wmiBrowseModeContentPanel.Name = "wmiBrowseModeContentPanel";
            this.wmiBrowseModeContentPanel.Size = new System.Drawing.Size(560, 161);
            this.wmiBrowseModeContentPanel.TabIndex = 33;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label32.Location = new System.Drawing.Point(444, 8);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(98, 13);
            this.label32.TabIndex = 41;
            this.label32.Text = "(Recommended)";
            // 
            // wmiShowAllCountersButton
            // 
            this.wmiShowAllCountersButton.Location = new System.Drawing.Point(11, 24);
            this.wmiShowAllCountersButton.Name = "wmiShowAllCountersButton";
            this.wmiShowAllCountersButton.Size = new System.Drawing.Size(328, 24);
            this.wmiShowAllCountersButton.TabIndex = 26;
            this.wmiShowAllCountersButton.Text = "Show all Windows Management Instrumentation (WMI) counters";
            this.wmiShowAllCountersButton.UseVisualStyleBackColor = true;
            this.wmiShowAllCountersButton.CheckedChanged += new System.EventHandler(this.wmiShowAllCountersButton_CheckedChanged);
            // 
            // wmiShowPerfCountersOnlyButton
            // 
            this.wmiShowPerfCountersOnlyButton.Checked = true;
            this.wmiShowPerfCountersOnlyButton.Location = new System.Drawing.Point(11, 3);
            this.wmiShowPerfCountersOnlyButton.Name = "wmiShowPerfCountersOnlyButton";
            this.wmiShowPerfCountersOnlyButton.Size = new System.Drawing.Size(437, 24);
            this.wmiShowPerfCountersOnlyButton.TabIndex = 25;
            this.wmiShowPerfCountersOnlyButton.TabStop = true;
            this.wmiShowPerfCountersOnlyButton.Text = "Show only those counters available in Windows System Performance Monitor (Perfmon" +
    ")";
            this.wmiShowPerfCountersOnlyButton.UseVisualStyleBackColor = true;
            // 
            // wmiServerComboBox
            // 
            this.wmiServerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiServerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wmiServerComboBox.BackColor = hoverBackColor;
            this.wmiServerComboBox.ForeColor = foreColor;
            this.wmiServerComboBox.FormattingEnabled = true;
            this.wmiServerComboBox.Location = new System.Drawing.Point(111, 54);
            this.wmiServerComboBox.Name = "wmiServerComboBox";
            this.wmiServerComboBox.Size = new System.Drawing.Size(429, 21);
            this.wmiServerComboBox.TabIndex = 28;
            this.wmiServerComboBox.DropDown += new System.EventHandler(this.serverPickList_DropDown);
            this.wmiServerComboBox.SelectionChangeCommitted += new System.EventHandler(this.wmiServerPickList_SelectionChangeCommitted);
            this.wmiServerComboBox.DropDownClosed += new System.EventHandler(this.serverPickList_DropDownClosed);
            // 
            // wmiObjectNameComboBox
            // 
            this.wmiObjectNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiObjectNameComboBox.DisplayMember = "First";
            this.wmiObjectNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wmiObjectNameComboBox.BackColor = hoverBackColor;
            this.wmiObjectNameComboBox.ForeColor = foreColor;
            this.wmiObjectNameComboBox.Enabled = false;
            this.wmiObjectNameComboBox.FormattingEnabled = true;
            this.wmiObjectNameComboBox.Location = new System.Drawing.Point(111, 81);
            this.wmiObjectNameComboBox.Name = "wmiObjectNameComboBox";
            this.wmiObjectNameComboBox.Size = new System.Drawing.Size(429, 21);
            this.wmiObjectNameComboBox.TabIndex = 33;
            this.wmiObjectNameComboBox.ValueMember = "First";
            this.wmiObjectNameComboBox.DropDown += new System.EventHandler(this.wmiObjectNameComboBox_DropDown);
            this.wmiObjectNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.wmiObjectNameComboBox_SelectionChangeCommitted);
            this.wmiObjectNameComboBox.DropDownClosed += new System.EventHandler(this.wmiObjectNameComboBox_DropDownClosed);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(27, 84);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Object Name:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(27, 111);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(78, 13);
            this.label24.TabIndex = 31;
            this.label24.Text = "Counter Name:";
            // 
            // wmiInstanceNameLabel1
            // 
            this.wmiInstanceNameLabel1.AutoSize = true;
            this.wmiInstanceNameLabel1.Location = new System.Drawing.Point(27, 139);
            this.wmiInstanceNameLabel1.Name = "wmiInstanceNameLabel1";
            this.wmiInstanceNameLabel1.Size = new System.Drawing.Size(82, 13);
            this.wmiInstanceNameLabel1.TabIndex = 32;
            this.wmiInstanceNameLabel1.Text = "Instance Name:";
            // 
            // wmiCounterNameComboBox
            // 
            this.wmiCounterNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiCounterNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wmiCounterNameComboBox.Enabled = false;
            this.wmiCounterNameComboBox.FormattingEnabled = true;
            this.wmiCounterNameComboBox.Location = new System.Drawing.Point(111, 108);
            this.wmiCounterNameComboBox.Name = "wmiCounterNameComboBox";
            this.wmiCounterNameComboBox.Size = new System.Drawing.Size(429, 21);
            this.wmiCounterNameComboBox.TabIndex = 34;
            this.wmiCounterNameComboBox.DropDown += new System.EventHandler(this.wmiCounterNameComboBox_DropDown);
            this.wmiCounterNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.wmiCounterNameComboBox_SelectionChangeCommitted);
            this.wmiCounterNameComboBox.DropDownClosed += new System.EventHandler(this.wmiCounterNameComboBox_DropDownClosed);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(27, 57);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(65, 13);
            this.label23.TabIndex = 29;
            this.label23.Text = "SQL Server:";
            // 
            // wmiInstanceNameComboBox
            // 
            this.wmiInstanceNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiInstanceNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wmiInstanceNameComboBox.Enabled = false;
            this.wmiInstanceNameComboBox.FormattingEnabled = true;
            this.wmiInstanceNameComboBox.Location = new System.Drawing.Point(111, 135);
            this.wmiInstanceNameComboBox.Name = "wmiInstanceNameComboBox";
            this.wmiInstanceNameComboBox.Size = new System.Drawing.Size(429, 21);
            this.wmiInstanceNameComboBox.TabIndex = 35;
            this.wmiInstanceNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.wmiInstanceNameComboBox_SelectionChangeCommitted);
            this.wmiInstanceNameComboBox.DropDownClosed += new System.EventHandler(this.wmiInstanceNameComboBox_DropDownClosed);
            // 
            // label29
            // 
            this.label29.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label29.Location = new System.Drawing.Point(111, 136);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(383, 19);
            this.label29.TabIndex = 40;
            this.label29.Text = "N/A";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sqlCounterPage
            // 
            this.sqlCounterPage.Controls.Add(this.groupBox2);
            this.sqlCounterPage.Controls.Add(this.panel3);
            this.sqlCounterPage.Description = "Select the SQL Server System Counter you would like to monitor";
            this.sqlCounterPage.Location = new System.Drawing.Point(11, 71);
            this.sqlCounterPage.Name = "sqlCounterPage";
            this.sqlCounterPage.NextPage = this.sqlBatchPage;
            this.sqlCounterPage.PreviousPage = this.wmiCounterPage;
            this.sqlCounterPage.Size = new System.Drawing.Size(587, 387);
            this.sqlCounterPage.TabIndex = 1008;
            this.sqlCounterPage.Text = "Select SQL Server System Counter";
            this.sqlCounterPage.BeforeDisplay += new System.EventHandler(this.sqlCounterPage_BeforeDisplay);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel6);
            this.groupBox2.Controls.Add(this.sqlManualModeContentPanel);
            this.groupBox2.Controls.Add(this.sqlBrowseModeContentPanel);
            this.groupBox2.Location = new System.Drawing.Point(11, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(566, 320);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Counter";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.sqlLoadingCircle);
            this.panel6.Controls.Add(this.sqlStatusLabel);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(3, 204);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(560, 67);
            this.panel6.TabIndex = 36;
            // 
            // sqlLoadingCircle
            // 
            this.sqlLoadingCircle.Active = false;
            this.sqlLoadingCircle.Color = System.Drawing.Color.Silver;
            this.sqlLoadingCircle.InnerCircleRadius = 5;
            this.sqlLoadingCircle.Location = new System.Drawing.Point(89, 39);
            this.sqlLoadingCircle.Name = "sqlLoadingCircle";
            this.sqlLoadingCircle.NumberSpoke = 12;
            this.sqlLoadingCircle.OuterCircleRadius = 11;
            this.sqlLoadingCircle.RotationSpeed = 20;
            this.sqlLoadingCircle.Size = new System.Drawing.Size(27, 23);
            this.sqlLoadingCircle.SpokeThickness = 2;
            this.sqlLoadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.sqlLoadingCircle.TabIndex = 26;
            this.sqlLoadingCircle.Visible = false;
            // 
            // sqlStatusLabel
            // 
            this.sqlStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlStatusLabel.Location = new System.Drawing.Point(122, 44);
            this.sqlStatusLabel.Name = "sqlStatusLabel";
            this.sqlStatusLabel.Size = new System.Drawing.Size(435, 23);
            this.sqlStatusLabel.TabIndex = 25;
            this.sqlStatusLabel.Text = "{0}";
            // 
            // sqlManualModeContentPanel
            // 
            this.sqlManualModeContentPanel.Controls.Add(this.sqlInstanceNameTextBox);
            this.sqlManualModeContentPanel.Controls.Add(this.sqlCounterNameTextBox);
            this.sqlManualModeContentPanel.Controls.Add(this.sqlObjectNameTextBox);
            this.sqlManualModeContentPanel.Controls.Add(this.label25);
            this.sqlManualModeContentPanel.Controls.Add(this.label26);
            this.sqlManualModeContentPanel.Controls.Add(this.label27);
            this.sqlManualModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sqlManualModeContentPanel.Location = new System.Drawing.Point(3, 125);
            this.sqlManualModeContentPanel.Name = "sqlManualModeContentPanel";
            this.sqlManualModeContentPanel.Size = new System.Drawing.Size(560, 79);
            this.sqlManualModeContentPanel.TabIndex = 35;
            this.sqlManualModeContentPanel.Visible = false;
            // 
            // sqlInstanceNameTextBox
            // 
            this.sqlInstanceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlInstanceNameTextBox.Location = new System.Drawing.Point(92, 55);
            this.sqlInstanceNameTextBox.Name = "sqlInstanceNameTextBox";
            this.sqlInstanceNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.sqlInstanceNameTextBox.TabIndex = 20;
            this.sqlInstanceNameTextBox.TextChanged += new System.EventHandler(this.sqlObjectNameTextBox_TextChanged);
            // 
            // sqlCounterNameTextBox
            // 
            this.sqlCounterNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlCounterNameTextBox.Location = new System.Drawing.Point(92, 29);
            this.sqlCounterNameTextBox.Name = "sqlCounterNameTextBox";
            this.sqlCounterNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.sqlCounterNameTextBox.TabIndex = 19;
            this.sqlCounterNameTextBox.TextChanged += new System.EventHandler(this.sqlObjectNameTextBox_TextChanged);
            // 
            // sqlObjectNameTextBox
            // 
            this.sqlObjectNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlObjectNameTextBox.Location = new System.Drawing.Point(92, 3);
            this.sqlObjectNameTextBox.Name = "sqlObjectNameTextBox";
            this.sqlObjectNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.sqlObjectNameTextBox.TabIndex = 18;
            this.sqlObjectNameTextBox.TextChanged += new System.EventHandler(this.sqlObjectNameTextBox_TextChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(8, 6);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(72, 13);
            this.label25.TabIndex = 15;
            this.label25.Text = "Object Name:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(8, 32);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(78, 13);
            this.label26.TabIndex = 16;
            this.label26.Text = "Counter Name:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(8, 59);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(82, 13);
            this.label27.TabIndex = 17;
            this.label27.Text = "Instance Name:";
            // 
            // sqlBrowseModeContentPanel
            // 
            this.sqlBrowseModeContentPanel.Controls.Add(this.sqlServerComboBox);
            this.sqlBrowseModeContentPanel.Controls.Add(this.sqlCounterNameComboBox);
            this.sqlBrowseModeContentPanel.Controls.Add(this.counterSourceLabel);
            this.sqlBrowseModeContentPanel.Controls.Add(this.label9);
            this.sqlBrowseModeContentPanel.Controls.Add(this.label8);
            this.sqlBrowseModeContentPanel.Controls.Add(this.label7);
            this.sqlBrowseModeContentPanel.Controls.Add(this.sqlObjectNameComboBox);
            this.sqlBrowseModeContentPanel.Controls.Add(this.sqlInstanceNameComboBox);
            this.sqlBrowseModeContentPanel.Controls.Add(this.label28);
            this.sqlBrowseModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sqlBrowseModeContentPanel.Location = new System.Drawing.Point(3, 16);
            this.sqlBrowseModeContentPanel.Name = "sqlBrowseModeContentPanel";
            this.sqlBrowseModeContentPanel.Size = new System.Drawing.Size(560, 109);
            this.sqlBrowseModeContentPanel.TabIndex = 34;
            // 
            // sqlServerComboBox
            // 
            this.sqlServerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlServerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sqlServerComboBox.FormattingEnabled = true;
            this.sqlServerComboBox.Location = new System.Drawing.Point(92, 3);
            this.sqlServerComboBox.Name = "sqlServerComboBox";
            this.sqlServerComboBox.Size = new System.Drawing.Size(448, 21);
            this.sqlServerComboBox.TabIndex = 23;
            this.sqlServerComboBox.DropDown += new System.EventHandler(this.serverPickList_DropDown);
            this.sqlServerComboBox.SelectionChangeCommitted += new System.EventHandler(this.sqlServerComboBox_SelectionChangeCommitted);
            this.sqlServerComboBox.DropDownClosed += new System.EventHandler(this.serverPickList_DropDownClosed);
            // 
            // sqlCounterNameComboBox
            // 
            this.sqlCounterNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlCounterNameComboBox.DisplayMember = "Name";
            this.sqlCounterNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sqlCounterNameComboBox.Enabled = false;
            this.sqlCounterNameComboBox.FormattingEnabled = true;
            this.sqlCounterNameComboBox.Location = new System.Drawing.Point(92, 57);
            this.sqlCounterNameComboBox.Name = "sqlCounterNameComboBox";
            this.sqlCounterNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.sqlCounterNameComboBox.TabIndex = 13;
            this.sqlCounterNameComboBox.DropDown += new System.EventHandler(this.sqlCounterNameComboBox_DropDown);
            this.sqlCounterNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.sqlCounterNameComboBox_SelectionChangeCommitted);
            this.sqlCounterNameComboBox.DropDownClosed += new System.EventHandler(this.sqlCounterNameComboBox_DropDownClosed);
            // 
            // counterSourceLabel
            // 
            this.counterSourceLabel.AutoSize = true;
            this.counterSourceLabel.Location = new System.Drawing.Point(8, 6);
            this.counterSourceLabel.Name = "counterSourceLabel";
            this.counterSourceLabel.Size = new System.Drawing.Size(65, 13);
            this.counterSourceLabel.TabIndex = 22;
            this.counterSourceLabel.Text = "SQL Server:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Object Name:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Counter Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Instance Name:";
            // 
            // sqlObjectNameComboBox
            // 
            this.sqlObjectNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlObjectNameComboBox.DisplayMember = "Name";
            this.sqlObjectNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sqlObjectNameComboBox.Enabled = false;
            this.sqlObjectNameComboBox.FormattingEnabled = true;
            this.sqlObjectNameComboBox.Location = new System.Drawing.Point(92, 30);
            this.sqlObjectNameComboBox.Name = "sqlObjectNameComboBox";
            this.sqlObjectNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.sqlObjectNameComboBox.TabIndex = 12;
            this.sqlObjectNameComboBox.DropDown += new System.EventHandler(this.sqlObjectNameComboBox_DropDown);
            this.sqlObjectNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.sqlObjectNameComboBox_SelectionChangeCommitted);
            this.sqlObjectNameComboBox.DropDownClosed += new System.EventHandler(this.sqlObjectNameComboBox_DropDownClosed);
            // 
            // sqlInstanceNameComboBox
            // 
            this.sqlInstanceNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlInstanceNameComboBox.DisplayMember = "Name";
            this.sqlInstanceNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sqlInstanceNameComboBox.Enabled = false;
            this.sqlInstanceNameComboBox.FormattingEnabled = true;
            this.sqlInstanceNameComboBox.Location = new System.Drawing.Point(92, 84);
            this.sqlInstanceNameComboBox.Name = "sqlInstanceNameComboBox";
            this.sqlInstanceNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.sqlInstanceNameComboBox.TabIndex = 14;
            this.sqlInstanceNameComboBox.DropDown += new System.EventHandler(this.sqlInstanceNameComboBox_DropDown);
            this.sqlInstanceNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.sqlInstanceNameComboBox_SelectionChangeCommitted);
            this.sqlInstanceNameComboBox.DropDownClosed += new System.EventHandler(this.sqlInstanceNameComboBox_DropDownClosed);
            // 
            // label28
            // 
            this.label28.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label28.Location = new System.Drawing.Point(92, 84);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(439, 19);
            this.label28.TabIndex = 21;
            this.label28.Text = "N/A";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label31);
            this.panel3.Controls.Add(this.sqlManualModeButton);
            this.panel3.Controls.Add(this.sqlBrowseModeButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(587, 52);
            this.panel3.TabIndex = 33;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label31.Location = new System.Drawing.Point(294, 8);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(98, 13);
            this.label31.TabIndex = 33;
            this.label31.Text = "(Recommended)";
            // 
            // sqlManualModeButton
            // 
            this.sqlManualModeButton.AutoSize = true;
            this.sqlManualModeButton.Location = new System.Drawing.Point(11, 29);
            this.sqlManualModeButton.Name = "sqlManualModeButton";
            this.sqlManualModeButton.Size = new System.Drawing.Size(197, 17);
            this.sqlManualModeButton.TabIndex = 30;
            this.sqlManualModeButton.Text = "Manually enter counter configuration";
            this.sqlManualModeButton.UseVisualStyleBackColor = true;
            // 
            // sqlBrowseModeButton
            // 
            this.sqlBrowseModeButton.AutoSize = true;
            this.sqlBrowseModeButton.Checked = true;
            this.sqlBrowseModeButton.Location = new System.Drawing.Point(11, 6);
            this.sqlBrowseModeButton.Name = "sqlBrowseModeButton";
            this.sqlBrowseModeButton.Size = new System.Drawing.Size(286, 17);
            this.sqlBrowseModeButton.TabIndex = 31;
            this.sqlBrowseModeButton.TabStop = true;
            this.sqlBrowseModeButton.Text = "Select a counter from an existing monitored SQL Server";
            this.sqlBrowseModeButton.UseVisualStyleBackColor = true;
            this.sqlBrowseModeButton.CheckedChanged += new System.EventHandler(this.sqlBrowseModeButton_CheckedChanged);
            // 
            // sqlBatchPage
            // 
            this.sqlBatchPage.Controls.Add(this.groupBox3);
            this.sqlBatchPage.Description = "Provide a custom SQL script that returns a numerical value that you would like to" +
    " monitor";
            this.sqlBatchPage.Location = new System.Drawing.Point(11, 71);
            this.sqlBatchPage.Name = "sqlBatchPage";
            this.sqlBatchPage.NextPage = this.vmCounterPage;
            this.sqlBatchPage.PreviousPage = this.sqlCounterPage;
            this.sqlBatchPage.Size = new System.Drawing.Size(587, 387);
            this.sqlBatchPage.TabIndex = 1009;
            this.sqlBatchPage.Text = "Provide Custom SQL Script";
            this.sqlBatchPage.BeforeDisplay += new System.EventHandler(this.sqlBatchPage_BeforeDisplay);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.informationBox2);
            this.groupBox3.Controls.Add(this.sqlBatchTextBox);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(7);
            this.groupBox3.Size = new System.Drawing.Size(587, 387);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Enter SQL Script";
            // 
            // informationBox2
            // 
            this.informationBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox2.Location = new System.Drawing.Point(31, 29);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(526, 38);
            this.informationBox2.TabIndex = 2;
            this.informationBox2.Text = "Please note that your SQL script must return a single numerical value in order to" +
    " collect successfully.";
            // 
            // sqlBatchTextBox
            // 
            this.sqlBatchTextBox.AcceptsReturn = true;
            this.sqlBatchTextBox.AcceptsTab = true;
            this.sqlBatchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlBatchTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlBatchTextBox.Location = new System.Drawing.Point(12, 73);
            this.sqlBatchTextBox.Multiline = true;
            this.sqlBatchTextBox.Name = "sqlBatchTextBox";
            this.sqlBatchTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sqlBatchTextBox.Size = new System.Drawing.Size(564, 300);
            this.sqlBatchTextBox.TabIndex = 1;
            this.sqlBatchTextBox.TextChanged += new System.EventHandler(this.sqlBatchTextBox_TextChanged);
            // 
            // vmCounterPage
            // 
            this.vmCounterPage.Controls.Add(this.panel8);
            this.vmCounterPage.Controls.Add(this.groupBox11);
            this.vmCounterPage.Description = "Select the Virtual Machine System Counter you would like to monitor";
            this.vmCounterPage.Location = new System.Drawing.Point(11, 71);
            this.vmCounterPage.Name = "vmCounterPage";
            this.vmCounterPage.NextPage = this.calculationTypePage;
            this.vmCounterPage.PreviousPage = this.sqlBatchPage;
            this.vmCounterPage.Size = new System.Drawing.Size(587, 387);
            this.vmCounterPage.TabIndex = 1014;
            this.vmCounterPage.Text = "Select Virtual Machine System Counter";
            this.vmCounterPage.BeforeDisplay += new System.EventHandler(this.vmCounterPage_BeforeDisplay);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label40);
            this.panel8.Controls.Add(this.vmManualModeButton);
            this.panel8.Controls.Add(this.vmBrowseModeButton);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(587, 52);
            this.panel8.TabIndex = 39;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label40.Location = new System.Drawing.Point(316, 8);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(98, 13);
            this.label40.TabIndex = 32;
            this.label40.Text = "(Recommended)";
            // 
            // vmManualModeButton
            // 
            this.vmManualModeButton.AutoSize = true;
            this.vmManualModeButton.Location = new System.Drawing.Point(11, 29);
            this.vmManualModeButton.Name = "vmManualModeButton";
            this.vmManualModeButton.Size = new System.Drawing.Size(197, 17);
            this.vmManualModeButton.TabIndex = 30;
            this.vmManualModeButton.Text = "Manually enter counter configuration";
            this.vmManualModeButton.UseVisualStyleBackColor = true;
            // 
            // vmBrowseModeButton
            // 
            this.vmBrowseModeButton.AutoSize = true;
            this.vmBrowseModeButton.Checked = true;
            this.vmBrowseModeButton.Location = new System.Drawing.Point(11, 6);
            this.vmBrowseModeButton.Name = "vmBrowseModeButton";
            this.vmBrowseModeButton.Size = new System.Drawing.Size(309, 17);
            this.vmBrowseModeButton.TabIndex = 31;
            this.vmBrowseModeButton.TabStop = true;
            this.vmBrowseModeButton.Text = "Select a counter from an existing monitored SQL Server host";
            this.vmBrowseModeButton.UseVisualStyleBackColor = true;
            this.vmBrowseModeButton.CheckedChanged += new System.EventHandler(this.vmBrowseModeButton_CheckedChanged);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.panel4);
            this.groupBox11.Controls.Add(this.vmManualModeContentPanel);
            this.groupBox11.Controls.Add(this.vmBrowseModeContentPanel);
            this.groupBox11.Location = new System.Drawing.Point(11, 58);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(566, 319);
            this.groupBox11.TabIndex = 38;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Select Counter";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.vmLoadingCircle);
            this.panel4.Controls.Add(this.vmStatusLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 204);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(560, 67);
            this.panel4.TabIndex = 36;
            // 
            // vmLoadingCircle
            // 
            this.vmLoadingCircle.Active = false;
            this.vmLoadingCircle.Color = System.Drawing.Color.Silver;
            this.vmLoadingCircle.InnerCircleRadius = 5;
            this.vmLoadingCircle.Location = new System.Drawing.Point(89, 39);
            this.vmLoadingCircle.Name = "vmLoadingCircle";
            this.vmLoadingCircle.NumberSpoke = 12;
            this.vmLoadingCircle.OuterCircleRadius = 11;
            this.vmLoadingCircle.RotationSpeed = 20;
            this.vmLoadingCircle.Size = new System.Drawing.Size(27, 23);
            this.vmLoadingCircle.SpokeThickness = 2;
            this.vmLoadingCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.vmLoadingCircle.TabIndex = 26;
            this.vmLoadingCircle.Visible = false;
            // 
            // vmStatusLabel
            // 
            this.vmStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vmStatusLabel.Location = new System.Drawing.Point(122, 44);
            this.vmStatusLabel.Name = "vmStatusLabel";
            this.vmStatusLabel.Size = new System.Drawing.Size(435, 23);
            this.vmStatusLabel.TabIndex = 25;
            this.vmStatusLabel.Text = "{0}";
            // 
            // vmManualModeContentPanel
            // 
            this.vmManualModeContentPanel.Controls.Add(this.vmInstanceNameTextBox);
            this.vmManualModeContentPanel.Controls.Add(this.vmCounterNameTextBox);
            this.vmManualModeContentPanel.Controls.Add(this.vmObjectNameTextBox);
            this.vmManualModeContentPanel.Controls.Add(this.label20);
            this.vmManualModeContentPanel.Controls.Add(this.label21);
            this.vmManualModeContentPanel.Controls.Add(this.label22);
            this.vmManualModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.vmManualModeContentPanel.Location = new System.Drawing.Point(3, 125);
            this.vmManualModeContentPanel.Name = "vmManualModeContentPanel";
            this.vmManualModeContentPanel.Size = new System.Drawing.Size(560, 79);
            this.vmManualModeContentPanel.TabIndex = 35;
            this.vmManualModeContentPanel.Visible = false;
            // 
            // vmInstanceNameTextBox
            // 
            this.vmInstanceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmInstanceNameTextBox.Location = new System.Drawing.Point(92, 55);
            this.vmInstanceNameTextBox.Name = "vmInstanceNameTextBox";
            this.vmInstanceNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.vmInstanceNameTextBox.TabIndex = 20;
            // 
            // vmCounterNameTextBox
            // 
            this.vmCounterNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmCounterNameTextBox.Location = new System.Drawing.Point(92, 29);
            this.vmCounterNameTextBox.Name = "vmCounterNameTextBox";
            this.vmCounterNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.vmCounterNameTextBox.TabIndex = 19;
            // 
            // vmObjectNameTextBox
            // 
            this.vmObjectNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmObjectNameTextBox.Location = new System.Drawing.Point(92, 3);
            this.vmObjectNameTextBox.Name = "vmObjectNameTextBox";
            this.vmObjectNameTextBox.Size = new System.Drawing.Size(448, 20);
            this.vmObjectNameTextBox.TabIndex = 18;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 6);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 13);
            this.label20.TabIndex = 15;
            this.label20.Text = "Object Name:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(8, 32);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(78, 13);
            this.label21.TabIndex = 16;
            this.label21.Text = "Counter Name:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 59);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(82, 13);
            this.label22.TabIndex = 17;
            this.label22.Text = "Instance Name:";
            // 
            // vmBrowseModeContentPanel
            // 
            this.vmBrowseModeContentPanel.Controls.Add(this.vmServerComboBox);
            this.vmBrowseModeContentPanel.Controls.Add(this.vmCounterNameComboBox);
            this.vmBrowseModeContentPanel.Controls.Add(this.label35);
            this.vmBrowseModeContentPanel.Controls.Add(this.label36);
            this.vmBrowseModeContentPanel.Controls.Add(this.label37);
            this.vmBrowseModeContentPanel.Controls.Add(this.label38);
            this.vmBrowseModeContentPanel.Controls.Add(this.vmObjectNameComboBox);
            this.vmBrowseModeContentPanel.Controls.Add(this.vmInstanceNameComboBox);
            this.vmBrowseModeContentPanel.Controls.Add(this.label39);
            this.vmBrowseModeContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.vmBrowseModeContentPanel.Location = new System.Drawing.Point(3, 16);
            this.vmBrowseModeContentPanel.Name = "vmBrowseModeContentPanel";
            this.vmBrowseModeContentPanel.Size = new System.Drawing.Size(560, 109);
            this.vmBrowseModeContentPanel.TabIndex = 34;
            // 
            // vmServerComboBox
            // 
            this.vmServerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmServerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vmServerComboBox.FormattingEnabled = true;
            this.vmServerComboBox.Location = new System.Drawing.Point(92, 3);
            this.vmServerComboBox.Name = "vmServerComboBox";
            this.vmServerComboBox.Size = new System.Drawing.Size(448, 21);
            this.vmServerComboBox.TabIndex = 23;
            this.vmServerComboBox.DropDown += new System.EventHandler(this.serverPickList_DropDown);
            this.vmServerComboBox.SelectionChangeCommitted += new System.EventHandler(this.vmServerComboBox_SelectionChangeCommitted);
            this.vmServerComboBox.DropDownClosed += new System.EventHandler(this.serverPickList_DropDownClosed);
            // 
            // vmCounterNameComboBox
            // 
            this.vmCounterNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmCounterNameComboBox.DisplayMember = "Name";
            this.vmCounterNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vmCounterNameComboBox.FormattingEnabled = true;
            this.vmCounterNameComboBox.Location = new System.Drawing.Point(92, 57);
            this.vmCounterNameComboBox.Name = "vmCounterNameComboBox";
            this.vmCounterNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.vmCounterNameComboBox.TabIndex = 13;
            this.vmCounterNameComboBox.DropDown += new System.EventHandler(this.vmCounterNameComboBox_DropDown);
            this.vmCounterNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.vmCounterNameComboBox_SelectionChangeCommitted);
            this.vmCounterNameComboBox.DropDownClosed += new System.EventHandler(this.vmCounterNameComboBox_DropDownClosed);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(8, 6);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(65, 13);
            this.label35.TabIndex = 22;
            this.label35.Text = "SQL Server:";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(8, 33);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(72, 13);
            this.label36.TabIndex = 6;
            this.label36.Text = "Object Name:";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(8, 60);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(78, 13);
            this.label37.TabIndex = 8;
            this.label37.Text = "Counter Name:";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(8, 87);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(82, 13);
            this.label38.TabIndex = 10;
            this.label38.Text = "Instance Name:";
            // 
            // vmObjectNameComboBox
            // 
            this.vmObjectNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmObjectNameComboBox.DisplayMember = "Name";
            this.vmObjectNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vmObjectNameComboBox.FormattingEnabled = true;
            this.vmObjectNameComboBox.Location = new System.Drawing.Point(92, 30);
            this.vmObjectNameComboBox.Name = "vmObjectNameComboBox";
            this.vmObjectNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.vmObjectNameComboBox.TabIndex = 12;
            this.vmObjectNameComboBox.DropDown += new System.EventHandler(this.vmObjectNameComboBox_DropDown);
            this.vmObjectNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.vmObjectNameComboBox_SelectionChangeCommitted);
            this.vmObjectNameComboBox.DropDownClosed += new System.EventHandler(this.vmObjectNameComboBox_DropDownClosed);
            // 
            // vmInstanceNameComboBox
            // 
            this.vmInstanceNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vmInstanceNameComboBox.DisplayMember = "Name";
            this.vmInstanceNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vmInstanceNameComboBox.FormattingEnabled = true;
            this.vmInstanceNameComboBox.Location = new System.Drawing.Point(92, 84);
            this.vmInstanceNameComboBox.Name = "vmInstanceNameComboBox";
            this.vmInstanceNameComboBox.Size = new System.Drawing.Size(448, 21);
            this.vmInstanceNameComboBox.TabIndex = 14;
            this.vmInstanceNameComboBox.DropDown += new System.EventHandler(this.vmInstanceNameComboBox_DropDown);
            this.vmInstanceNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.vmInstanceNameComboBox_SelectionChangeCommitted);
            this.vmInstanceNameComboBox.DropDownClosed += new System.EventHandler(this.vmInstanceNameComboBox_DropDownClosed);
            // 
            // label39
            // 
            this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label39.Location = new System.Drawing.Point(92, 84);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(439, 19);
            this.label39.TabIndex = 21;
            this.label39.Text = "N/A";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // calculationTypePage
            // 
            this.calculationTypePage.Controls.Add(this.testButton);
            this.calculationTypePage.Controls.Add(this.groupBox9);
            this.calculationTypePage.Controls.Add(this.groupBox8);
            this.calculationTypePage.Controls.Add(this.informationBox7);
            this.calculationTypePage.Description = "Customize the custom counter value to suit your needs";
            this.calculationTypePage.Location = new System.Drawing.Point(11, 71);
            this.calculationTypePage.Name = "calculationTypePage";
            this.calculationTypePage.NextPage = this.counterNamePage;
            this.calculationTypePage.PreviousPage = this.vmCounterPage;
            this.calculationTypePage.Size = new System.Drawing.Size(587, 387);
            this.calculationTypePage.TabIndex = 1013;
            this.calculationTypePage.Text = "Customize Counter Value";
            this.calculationTypePage.BeforeDisplay += new System.EventHandler(this.calculationTypePage_BeforeDisplay);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(479, 336);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 3;
            this.testButton.Text = "Test...";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.counterScaleComboBox);
            this.groupBox9.Controls.Add(this.label34);
            this.groupBox9.Controls.Add(this.label33);
            this.groupBox9.Location = new System.Drawing.Point(53, 208);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(501, 121);
            this.groupBox9.TabIndex = 2;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Customize Scale Factor";
            // 
            // counterScaleComboBox
            // 
            this.counterScaleComboBox.FormatString = "0.################";
            this.counterScaleComboBox.FormattingEnabled = true;
            this.counterScaleComboBox.Location = new System.Drawing.Point(94, 90);
            this.counterScaleComboBox.Name = "counterScaleComboBox";
            this.counterScaleComboBox.Size = new System.Drawing.Size(151, 21);
            this.counterScaleComboBox.TabIndex = 22;
            this.counterScaleComboBox.TextChanged += new System.EventHandler(this.counterScaleComboBox_TextChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(51, 93);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(37, 13);
            this.label34.TabIndex = 21;
            this.label34.Text = "Scale:";
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(9, 26);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(486, 61);
            this.label33.TabIndex = 1;
            this.label33.Text = resources.GetString("label33.Text");
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.calcTypeDeltaButton);
            this.groupBox8.Controls.Add(this.calcTypeValueButton);
            this.groupBox8.Controls.Add(this.label19);
            this.groupBox8.Location = new System.Drawing.Point(53, 47);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(501, 149);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Customize Calculation Type";
            // 
            // calcTypeDeltaButton
            // 
            this.calcTypeDeltaButton.AutoSize = true;
            this.calcTypeDeltaButton.Location = new System.Drawing.Point(54, 119);
            this.calcTypeDeltaButton.Name = "calcTypeDeltaButton";
            this.calcTypeDeltaButton.Size = new System.Drawing.Size(224, 17);
            this.calcTypeDeltaButton.TabIndex = 2;
            this.calcTypeDeltaButton.Text = "Use per second value since last collection";
            this.calcTypeDeltaButton.UseVisualStyleBackColor = true;
            // 
            // calcTypeValueButton
            // 
            this.calcTypeValueButton.AutoSize = true;
            this.calcTypeValueButton.Checked = true;
            this.calcTypeValueButton.Location = new System.Drawing.Point(54, 96);
            this.calcTypeValueButton.Name = "calcTypeValueButton";
            this.calcTypeValueButton.Size = new System.Drawing.Size(119, 17);
            this.calcTypeValueButton.TabIndex = 1;
            this.calcTypeValueButton.TabStop = true;
            this.calcTypeValueButton.Text = "Use collected value";
            this.calcTypeValueButton.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(6, 22);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(489, 71);
            this.label19.TabIndex = 0;
            this.label19.Text = resources.GetString("label19.Text");
            // 
            // informationBox7
            // 
            this.informationBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox7.Location = new System.Drawing.Point(0, 0);
            this.informationBox7.Name = "informationBox7";
            this.informationBox7.Size = new System.Drawing.Size(587, 48);
            this.informationBox7.TabIndex = 0;
            this.informationBox7.Text = resources.GetString("informationBox7.Text");
            // 
            // counterNamePage
            // 
            this.counterNamePage.Controls.Add(this.groupBox4);
            this.counterNamePage.Controls.Add(this.informationBox3);
            this.counterNamePage.Description = "Provide a name, category and description for the counter";
            this.counterNamePage.Location = new System.Drawing.Point(11, 71);
            this.counterNamePage.Name = "counterNamePage";
            this.counterNamePage.NextPage = this.alertDefinitionPage;
            this.counterNamePage.PreviousPage = this.calculationTypePage;
            this.counterNamePage.Size = new System.Drawing.Size(587, 387);
            this.counterNamePage.TabIndex = 1011;
            this.counterNamePage.Text = "Customize Counter Definition";
            this.counterNamePage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.counterNamePage_BeforeMoveNext);
            this.counterNamePage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.counterNamePage_BeforeMoveBack);
            this.counterNamePage.BeforeDisplay += new System.EventHandler(this.counterNamePage_BeforeDisplay);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.alertNameTextBox);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.alertDescriptionTextBox);
            this.groupBox4.Controls.Add(this.alertCategoryComboBox);
            this.groupBox4.Location = new System.Drawing.Point(50, 65);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(504, 229);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Customize Counter Information";
            // 
            // alertNameTextBox
            // 
            this.alertNameTextBox.Location = new System.Drawing.Point(92, 22);
            this.alertNameTextBox.MaxLength = 128;
            this.alertNameTextBox.Name = "alertNameTextBox";
            this.alertNameTextBox.Size = new System.Drawing.Size(395, 20);
            this.alertNameTextBox.TabIndex = 0;
            this.alertNameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 25);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 8;
            this.label18.Text = "Name:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 74);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Description:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 49);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Category:";
            // 
            // alertDescriptionTextBox
            // 
            this.alertDescriptionTextBox.AccessibleDescription = "";
            this.alertDescriptionTextBox.Location = new System.Drawing.Point(92, 75);
            this.alertDescriptionTextBox.MaxLength = 512;
            this.alertDescriptionTextBox.Multiline = true;
            this.alertDescriptionTextBox.Name = "alertDescriptionTextBox";
            this.alertDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.alertDescriptionTextBox.Size = new System.Drawing.Size(395, 141);
            this.alertDescriptionTextBox.TabIndex = 2;
            this.alertDescriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
            // 
            // alertCategoryComboBox
            // 
            this.alertCategoryComboBox.FormattingEnabled = true;
            this.alertCategoryComboBox.Location = new System.Drawing.Point(92, 48);
            this.alertCategoryComboBox.MaxLength = 64;
            this.alertCategoryComboBox.Name = "alertCategoryComboBox";
            this.alertCategoryComboBox.Size = new System.Drawing.Size(395, 21);
            this.alertCategoryComboBox.TabIndex = 1;
            this.alertCategoryComboBox.DropDown += new System.EventHandler(this.categoryComboBox_DropDown);
            this.alertCategoryComboBox.TextChanged += new System.EventHandler(this.categoryComboBox_TextChanged);
            // 
            // informationBox3
            // 
            this.informationBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox3.Location = new System.Drawing.Point(0, 0);
            this.informationBox3.Name = "informationBox3";
            this.informationBox3.Size = new System.Drawing.Size(587, 56);
            this.informationBox3.TabIndex = 17;
            this.informationBox3.Text = resources.GetString("informationBox3.Text");
            // 
            // alertDefinitionPage
            // 
            this.alertDefinitionPage.Controls.Add(this.groupBox10);
            this.alertDefinitionPage.Controls.Add(this.groupBox7);
            this.alertDefinitionPage.Controls.Add(this.groupBox5);
            this.alertDefinitionPage.Description = "Provide default alert settings to suit your needs";
            this.alertDefinitionPage.Location = new System.Drawing.Point(11, 71);
            this.alertDefinitionPage.Name = "alertDefinitionPage";
            this.alertDefinitionPage.NextPage = this.finishPage1;
            this.alertDefinitionPage.PreviousPage = this.counterNamePage;
            this.alertDefinitionPage.Size = new System.Drawing.Size(587, 387);
            this.alertDefinitionPage.TabIndex = 1012;
            this.alertDefinitionPage.Text = "Configure Alert Settings";
            this.alertDefinitionPage.BeforeDisplay += new System.EventHandler(this.alertDefinitionPage_BeforeDisplay);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.enableAlertCheckBox);
            this.groupBox10.Controls.Add(this.informationBox4);
            this.groupBox10.Location = new System.Drawing.Point(34, 283);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(520, 89);
            this.groupBox10.TabIndex = 21;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Enable Alert";
            // 
            // enableAlertCheckBox
            // 
            this.enableAlertCheckBox.AutoSize = true;
            this.enableAlertCheckBox.Checked = true;
            this.enableAlertCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableAlertCheckBox.Location = new System.Drawing.Point(70, 63);
            this.enableAlertCheckBox.Name = "enableAlertCheckBox";
            this.enableAlertCheckBox.Size = new System.Drawing.Size(170, 17);
            this.enableAlertCheckBox.TabIndex = 7;
            this.enableAlertCheckBox.Text = "Enable counter alert by default";
            this.enableAlertCheckBox.UseVisualStyleBackColor = true;
            // 
            // informationBox4
            // 
            this.informationBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox4.Icon = Divelements.WizardFramework.SystemIconType.Question;
            this.informationBox4.Location = new System.Drawing.Point(6, 23);
            this.informationBox4.Name = "informationBox4";
            this.informationBox4.Size = new System.Drawing.Size(508, 34);
            this.informationBox4.TabIndex = 17;
            this.informationBox4.Text = "Would you like to enable this custom counter alert by default when it is linked t" +
    "o a monitored SQL Server?";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comparisonTypeLessThanButton);
            this.groupBox7.Controls.Add(this.comparisonTypeGreaterThanButton);
            this.groupBox7.Controls.Add(this.informationBox6);
            this.groupBox7.Location = new System.Drawing.Point(34, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(520, 119);
            this.groupBox7.TabIndex = 20;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Configure Alert Evaluation";
            // 
            // comparisonTypeLessThanButton
            // 
            this.comparisonTypeLessThanButton.AutoSize = true;
            this.comparisonTypeLessThanButton.Location = new System.Drawing.Point(70, 91);
            this.comparisonTypeLessThanButton.Name = "comparisonTypeLessThanButton";
            this.comparisonTypeLessThanButton.Size = new System.Drawing.Size(227, 17);
            this.comparisonTypeLessThanButton.TabIndex = 2;
            this.comparisonTypeLessThanButton.Text = "Lower values are worse than higher values";
            this.comparisonTypeLessThanButton.UseVisualStyleBackColor = true;
            // 
            // comparisonTypeGreaterThanButton
            // 
            this.comparisonTypeGreaterThanButton.AutoSize = true;
            this.comparisonTypeGreaterThanButton.Checked = true;
            this.comparisonTypeGreaterThanButton.Location = new System.Drawing.Point(70, 68);
            this.comparisonTypeGreaterThanButton.Name = "comparisonTypeGreaterThanButton";
            this.comparisonTypeGreaterThanButton.Size = new System.Drawing.Size(225, 17);
            this.comparisonTypeGreaterThanButton.TabIndex = 1;
            this.comparisonTypeGreaterThanButton.TabStop = true;
            this.comparisonTypeGreaterThanButton.Text = "Higher values are worse than lower values";
            this.comparisonTypeGreaterThanButton.UseVisualStyleBackColor = true;
            this.comparisonTypeGreaterThanButton.CheckedChanged += new System.EventHandler(this.comparisonTypeGreaterThanButton_CheckedChanged);
            // 
            // informationBox6
            // 
            this.informationBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox6.Location = new System.Drawing.Point(6, 21);
            this.informationBox6.Name = "informationBox6";
            this.informationBox6.Size = new System.Drawing.Size(508, 41);
            this.informationBox6.TabIndex = 18;
            this.informationBox6.Text = resources.GetString("informationBox6.Text");
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.infoThresholdSpinner);
            this.groupBox5.Controls.Add(this.alertAdvancedButton);
            this.groupBox5.Controls.Add(this.informationBox5);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.warningThresholdSpinner);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.criticalThresholdSpinner);
            this.groupBox5.Location = new System.Drawing.Point(34, 129);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(520, 151);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Configure Alert Thresholds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Informational Threshold:";
            // 
            // infoThresholdSpinner
            // 
            this.infoThresholdSpinner.Location = new System.Drawing.Point(173, 71);
            this.infoThresholdSpinner.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.infoThresholdSpinner.Name = "infoThresholdSpinner";
            this.infoThresholdSpinner.Size = new System.Drawing.Size(151, 20);
            this.infoThresholdSpinner.TabIndex = 3;
            this.infoThresholdSpinner.ValueChanged += new System.EventHandler(this.warningThresholdSpinner_TextChanged);
            // 
            // alertAdvancedButton
            // 
            this.alertAdvancedButton.Location = new System.Drawing.Point(433, 115);
            this.alertAdvancedButton.Name = "alertAdvancedButton";
            this.alertAdvancedButton.Size = new System.Drawing.Size(75, 23);
            this.alertAdvancedButton.TabIndex = 6;
            this.alertAdvancedButton.Text = "Advanced";
            this.alertAdvancedButton.UseVisualStyleBackColor = true;
            this.alertAdvancedButton.Click += new System.EventHandler(this.alertAdvancedButton_Click);
            // 
            // informationBox5
            // 
            this.informationBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox5.Location = new System.Drawing.Point(6, 23);
            this.informationBox5.Name = "informationBox5";
            this.informationBox5.Size = new System.Drawing.Size(508, 54);
            this.informationBox5.TabIndex = 17;
            this.informationBox5.Text = resources.GetString("informationBox5.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Warning Threshold:";
            // 
            // warningThresholdSpinner
            // 
            this.warningThresholdSpinner.Location = new System.Drawing.Point(173, 97);
            this.warningThresholdSpinner.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.warningThresholdSpinner.Name = "warningThresholdSpinner";
            this.warningThresholdSpinner.Size = new System.Drawing.Size(151, 20);
            this.warningThresholdSpinner.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Critical Threshold:";
            // 
            // criticalThresholdSpinner
            // 
            this.criticalThresholdSpinner.Location = new System.Drawing.Point(173, 123);
            this.criticalThresholdSpinner.Maximum = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.criticalThresholdSpinner.Name = "criticalThresholdSpinner";
            this.criticalThresholdSpinner.Size = new System.Drawing.Size(151, 20);
            this.criticalThresholdSpinner.TabIndex = 5;
            // 
            // finishPage1
            // 
            this.finishPage1.Controls.Add(this.label11);
            this.finishPage1.FinishText = "You have successfully configured your custom counter. Please note the following:";
            this.finishPage1.Location = new System.Drawing.Point(175, 71);
            this.finishPage1.Name = "finishPage1";
            this.finishPage1.PreviousPage = this.alertDefinitionPage;
            this.finishPage1.ProceedText = "Click Finish to complete this wizard.";
            this.finishPage1.Size = new System.Drawing.Size(412, 387);
            this.finishPage1.TabIndex = 1006;
            this.finishPage1.Text = "Completing the Custom Counter Wizard";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(20, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(383, 175);
            this.label11.TabIndex = 3;
            this.label11.Text = resources.GetString("label11.Text");
            // 
            // azureServerConfigPage
            // 
            this.azureServerConfigPage.Controls.Add(this.azureServerConfigGroupBox);
            this.azureServerConfigPage.Description = "Provide information about azure instances that you would like to monitor";
            this.azureServerConfigPage.Location = new System.Drawing.Point(11, 71);
            this.azureServerConfigPage.Name = "azureServerConfigPage";
            this.azureServerConfigPage.PreviousPage = this.counterTypePage;
            this.azureServerConfigPage.Size = new System.Drawing.Size(587, 387);
            this.azureServerConfigPage.TabIndex = 1019;
            this.azureServerConfigPage.Text = "Provide information about Azure Server";
            this.azureServerConfigPage.BeforeDisplay += new System.EventHandler(this.AzureServerConfigPage_BeforeDisplay);
            // 
            // azureServerConfigGroupBox
            // 
            this.azureServerConfigGroupBox.Controls.Add(this.azureServerContentPanel);
            this.azureServerConfigGroupBox.Controls.Add(this.azureProfileButton);
            this.azureServerConfigGroupBox.Location = new System.Drawing.Point(11, 12);
            this.azureServerConfigGroupBox.Name = "azureServerConfigGroupBox";
            this.azureServerConfigGroupBox.Size = new System.Drawing.Size(566, 355);
            this.azureServerConfigGroupBox.TabIndex = 37;
            this.azureServerConfigGroupBox.TabStop = false;
            this.azureServerConfigGroupBox.Text = "Azure Server Configuration Details";
            // 
            // azureServerContentPanel
            // 
            this.azureServerContentPanel.Controls.Add(this.azureProfileComboBox);
            this.azureServerContentPanel.Controls.Add(this.azureProfileLbl);
            this.azureServerContentPanel.Controls.Add(this.azureResourceNameComboBox);
            this.azureServerContentPanel.Controls.Add(this.azureResourceNameLabel);
            this.azureServerContentPanel.Controls.Add(this.azureServerComboBox);
            this.azureServerContentPanel.Controls.Add(this.azureServerLbl);
            this.azureServerContentPanel.Controls.Add(this.azureNameSpaceNameLbl);
            this.azureServerContentPanel.Controls.Add(this.azureResourceTypeNameComboBox);
            this.azureServerContentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.azureServerContentPanel.Location = new System.Drawing.Point(3, 16);
            this.azureServerContentPanel.Name = "azureServerContentPanel";
            this.azureServerContentPanel.Size = new System.Drawing.Size(560, 235);
            this.azureServerContentPanel.TabIndex = 35;
            this.azureServerContentPanel.Visible = false;
            // 
            // azureProfileComboBox
            // 
            this.azureProfileComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureProfileComboBox.DisplayMember = "Profile Name";
            this.azureProfileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureProfileComboBox.Enabled = false;
            this.azureProfileComboBox.FormattingEnabled = true;
            this.azureProfileComboBox.Location = new System.Drawing.Point(175, 74);
            this.azureProfileComboBox.Name = "azureProfileComboBox";
            this.azureProfileComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureProfileComboBox.TabIndex = 35;
            this.azureProfileComboBox.SelectionChangeCommitted += new System.EventHandler(this.Profile_SelectionChangeCommitted);
            // 
            // azureProfileLbl
            // 
            this.azureProfileLbl.AutoSize = true;
            this.azureProfileLbl.Location = new System.Drawing.Point(17, 74);
            this.azureProfileLbl.Name = "azureProfileLbl";
            this.azureProfileLbl.Size = new System.Drawing.Size(72, 13);
            this.azureProfileLbl.TabIndex = 34;
            this.azureProfileLbl.Text = "Azure Profile: ";
            // 
            // azureResourceNameComboBox
            // 
            this.azureResourceNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureResourceNameComboBox.DisplayMember = "Resource Names";
            this.azureResourceNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureResourceNameComboBox.Enabled = false;
            this.azureResourceNameComboBox.FormattingEnabled = true;
            this.azureResourceNameComboBox.Location = new System.Drawing.Point(175, 168);
            this.azureResourceNameComboBox.Name = "azureResourceNameComboBox";
            this.azureResourceNameComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureResourceNameComboBox.TabIndex = 33;
            this.azureResourceNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.ResourceName_SelectionChangeCommitted);
            // 
            // azureResourceNameLabel
            // 
            this.azureResourceNameLabel.AutoSize = true;
            this.azureResourceNameLabel.Location = new System.Drawing.Point(20, 168);
            this.azureResourceNameLabel.Name = "azureResourceNameLabel";
            this.azureResourceNameLabel.Size = new System.Drawing.Size(87, 13);
            this.azureResourceNameLabel.TabIndex = 32;
            this.azureResourceNameLabel.Text = "Resource Name:";
            // 
            // azureServerComboBox
            // 
            this.azureServerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureServerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureServerComboBox.Location = new System.Drawing.Point(175, 25);
            this.azureServerComboBox.Name = "azureServerComboBox";
            this.azureServerComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureServerComboBox.TabIndex = 29;
            this.azureServerComboBox.SelectedIndexChanged += new System.EventHandler(this.azureServerComboBox_SelectedIndexChanged);
            // 
            // azureServerLbl
            // 
            this.azureServerLbl.AutoSize = true;
            this.azureServerLbl.Location = new System.Drawing.Point(17, 28);
            this.azureServerLbl.Name = "azureServerLbl";
            this.azureServerLbl.Size = new System.Drawing.Size(74, 13);
            this.azureServerLbl.TabIndex = 17;
            this.azureServerLbl.Text = "Azure Server: ";
            // 
            // azureNameSpaceNameLbl
            // 
            this.azureNameSpaceNameLbl.AutoSize = true;
            this.azureNameSpaceNameLbl.Location = new System.Drawing.Point(20, 122);
            this.azureNameSpaceNameLbl.Name = "azureNameSpaceNameLbl";
            this.azureNameSpaceNameLbl.Size = new System.Drawing.Size(83, 13);
            this.azureNameSpaceNameLbl.TabIndex = 18;
            this.azureNameSpaceNameLbl.Text = "Resource Type:";
            // 
            // azureResourceTypeNameComboBox
            // 
            this.azureResourceTypeNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureResourceTypeNameComboBox.DisplayMember = "Resource Type Name";
            this.azureResourceTypeNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureResourceTypeNameComboBox.Enabled = false;
            this.azureResourceTypeNameComboBox.FormattingEnabled = true;
            this.azureResourceTypeNameComboBox.Location = new System.Drawing.Point(175, 122);
            this.azureResourceTypeNameComboBox.Name = "azureResourceTypeNameComboBox";
            this.azureResourceTypeNameComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureResourceTypeNameComboBox.TabIndex = 18;
            this.azureResourceTypeNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.ResourceTypeName_SelectionChangeCommitted);
            // 
            // azureProfileButton
            // 
            this.azureProfileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.azureProfileButton.Location = new System.Drawing.Point(13, 303);
            this.azureProfileButton.Name = "azureProfileButton";
            this.azureProfileButton.Size = new System.Drawing.Size(100, 23);
            this.azureProfileButton.TabIndex = 39;
            this.azureProfileButton.Text = "View Profiles";
            this.azureProfileButton.UseVisualStyleBackColor = true;
            this.azureProfileButton.Click += new System.EventHandler(this.azureProfileButton_click);
            // 
            // azureServerMetricsPage
            // 
            this.azureServerMetricsPage.Controls.Add(this.azureMetricLabel);
            this.azureServerMetricsPage.Controls.Add(this.azureServerMetricsGroupBox);
            this.azureServerMetricsPage.Controls.Add(this.azureMetricNameLbl);
            this.azureServerMetricsPage.Controls.Add(this.azureMetricNameComboBox);
            this.azureServerMetricsPage.Description = "Choose Metrics for azure instances that you would like to monitor";
            this.azureServerMetricsPage.Location = new System.Drawing.Point(11, 71);
            this.azureServerMetricsPage.Name = "azureServerMetricsPage";
            this.azureServerMetricsPage.PreviousPage = this.azureServerConfigPage;
            this.azureServerMetricsPage.Size = new System.Drawing.Size(587, 387);
            this.azureServerMetricsPage.TabIndex = 1010;
            this.azureServerMetricsPage.Text = "Choose metric to configure for Azure Server";
            this.azureServerMetricsPage.BeforeDisplay += new System.EventHandler(this.azureServerMetricsPage_BeforeDisplay);
            // 
            // azureServerMetricsGroupBox
            // 
            this.azureServerMetricsGroupBox.Controls.Add(this.azureServerMetricsPanel);
            this.azureServerMetricsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.azureServerMetricsGroupBox.Name = "azureServerMetricsGroupBox";
            this.azureServerMetricsGroupBox.Size = new System.Drawing.Size(566, 152);
            this.azureServerMetricsGroupBox.TabIndex = 37;
            this.azureServerMetricsGroupBox.TabStop = false;
            this.azureServerMetricsGroupBox.Text = "Azure Server Metric Details";
            // 
            // azureServerMetricsPanel
            // 
            this.azureServerMetricsPanel.Controls.Add(this.azureMetricPageServerNameLbl);
            this.azureServerMetricsPanel.Controls.Add(this.azureMetricPageResourceGroupNameLbl);
            this.azureServerMetricsPanel.Controls.Add(this.azureMetricPageObjectNameLbl);
            this.azureServerMetricsPanel.Controls.Add(this.azureServerNameLbl);
            this.azureServerMetricsPanel.Controls.Add(this.azureObjectNameLbl);
            this.azureServerMetricsPanel.Controls.Add(this.azureNameSpaceLbl);
            this.azureServerMetricsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.azureServerMetricsPanel.Location = new System.Drawing.Point(3, 16);
            this.azureServerMetricsPanel.Name = "azureServerMetricsPanel";
            this.azureServerMetricsPanel.Size = new System.Drawing.Size(560, 368);
            this.azureServerMetricsPanel.TabIndex = 35;
            this.azureServerMetricsPanel.Visible = false;
            // 
            // azureMetricPageServerNameLbl
            // 
            this.azureMetricPageServerNameLbl.AutoSize = true;
            this.azureMetricPageServerNameLbl.Location = new System.Drawing.Point(158, 30);
            this.azureMetricPageServerNameLbl.Name = "azureMetricPageServerNameLbl";
            this.azureMetricPageServerNameLbl.Size = new System.Drawing.Size(0, 13);
            this.azureMetricPageServerNameLbl.TabIndex = 21;
            // 
            // azureMetricPageResourceGroupNameLbl
            // 
            this.azureMetricPageResourceGroupNameLbl.AutoSize = true;
            this.azureMetricPageResourceGroupNameLbl.Location = new System.Drawing.Point(158, 99);
            this.azureMetricPageResourceGroupNameLbl.Name = "azureMetricPageResourceGroupNameLbl";
            this.azureMetricPageResourceGroupNameLbl.Size = new System.Drawing.Size(0, 13);
            this.azureMetricPageResourceGroupNameLbl.TabIndex = 22;
            // 
            // azureMetricPageObjectNameLbl
            // 
            this.azureMetricPageObjectNameLbl.AutoSize = true;
            this.azureMetricPageObjectNameLbl.Location = new System.Drawing.Point(158, 67);
            this.azureMetricPageObjectNameLbl.Name = "azureMetricPageObjectNameLbl";
            this.azureMetricPageObjectNameLbl.Size = new System.Drawing.Size(0, 13);
            this.azureMetricPageObjectNameLbl.TabIndex = 23;
            // 
            // azureServerNameLbl
            // 
            this.azureServerNameLbl.AutoSize = true;
            this.azureServerNameLbl.Location = new System.Drawing.Point(28, 30);
            this.azureServerNameLbl.Name = "azureServerNameLbl";
            this.azureServerNameLbl.Size = new System.Drawing.Size(72, 13);
            this.azureServerNameLbl.TabIndex = 16;
            this.azureServerNameLbl.Text = "Server Name:";
            // 
            // azureObjectNameLbl
            // 
            this.azureObjectNameLbl.AutoSize = true;
            this.azureObjectNameLbl.Location = new System.Drawing.Point(28, 99);
            this.azureObjectNameLbl.Name = "azureObjectNameLbl";
            this.azureObjectNameLbl.Size = new System.Drawing.Size(117, 13);
            this.azureObjectNameLbl.TabIndex = 20;
            this.azureObjectNameLbl.Text = "Resource Type Name :";
            // 
            // azureNameSpaceLbl
            // 
            this.azureNameSpaceLbl.AutoSize = true;
            this.azureNameSpaceLbl.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.azureNameSpaceLbl.Location = new System.Drawing.Point(30, 67);
            this.azureNameSpaceLbl.Name = "azureNameSpaceLbl";
            this.azureNameSpaceLbl.Size = new System.Drawing.Size(86, 13);
            this.azureNameSpaceLbl.TabIndex = 18;
            this.azureNameSpaceLbl.Text = "Resource Type :";
            // 
            // azureMetricNameLbl
            // 
            this.azureMetricNameLbl.AutoSize = true;
            this.azureMetricNameLbl.Location = new System.Drawing.Point(32, 155);
            this.azureMetricNameLbl.Name = "azureMetricNameLbl";
            this.azureMetricNameLbl.Size = new System.Drawing.Size(70, 13);
            this.azureMetricNameLbl.TabIndex = 15;
            this.azureMetricNameLbl.Text = "Metric Name:";
            // 
            // azureMetricNameComboBox
            // 
            this.azureMetricNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureMetricNameComboBox.DisplayMember = "Metric Name";
            this.azureMetricNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureMetricNameComboBox.FormattingEnabled = true;
            this.azureMetricNameComboBox.Location = new System.Drawing.Point(164, 152);
            this.azureMetricNameComboBox.Name = "azureMetricNameComboBox";
            this.azureMetricNameComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureMetricNameComboBox.TabIndex = 19;
            this.azureMetricNameComboBox.SelectedIndexChanged += new System.EventHandler(this.azureMetricNameComboBox_SelectedIndexChanged);
            // 
            // azureDatabaseLabel
            // 
            this.azureDatabaseLabel.AutoSize = true;
            this.azureDatabaseLabel.Location = new System.Drawing.Point(20, 168);
            this.azureDatabaseLabel.Name = "azureDatabaseLabel";
            this.azureDatabaseLabel.Size = new System.Drawing.Size(56, 13);
            this.azureDatabaseLabel.TabIndex = 31;
            this.azureDatabaseLabel.Text = "Database:";
            this.azureDatabaseLabel.Visible = false;
            // 
            // azureDbComboBox
            // 
            this.azureDbComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureDbComboBox.DisplayMember = "Database Names";
            this.azureDbComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.azureDbComboBox.FormattingEnabled = true;
            this.azureDbComboBox.Location = new System.Drawing.Point(175, 168);
            this.azureDbComboBox.Name = "azureDbComboBox";
            this.azureDbComboBox.Size = new System.Drawing.Size(376, 21);
            this.azureDbComboBox.TabIndex = 30;
            this.azureDbComboBox.Visible = false;
            // 
            // sqlServerNameTextBox
            // 
            this.sqlServerNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.sqlServerNameTextBox.Name = "sqlServerNameTextBox";
            this.sqlServerNameTextBox.Size = new System.Drawing.Size(121, 21);
            this.sqlServerNameTextBox.TabIndex = 0;
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.Size = new System.Drawing.Size(121, 21);
            this.databaseNameTextBox.TabIndex = 0;
            // 
            // azureMetricValueLbl
            // 
            this.azureMetricValueLbl.AutoSize = true;
            this.azureMetricValueLbl.Location = new System.Drawing.Point(8, 137);
            this.azureMetricValueLbl.Name = "azureMetricValueLbl";
            this.azureMetricValueLbl.Size = new System.Drawing.Size(107, 13);
            this.azureMetricValueLbl.TabIndex = 19;
            this.azureMetricValueLbl.Text = "Choose Metric value:";
            // 
            // counterIdentificationPage
            // 
            this.counterIdentificationPage.Controls.Add(this.categoryComboBox1);
            this.counterIdentificationPage.Controls.Add(this.descriptionTextBox1);
            this.counterIdentificationPage.Controls.Add(this.label3);
            this.counterIdentificationPage.Controls.Add(this.label2);
            this.counterIdentificationPage.Controls.Add(this.nameTextBox1);
            this.counterIdentificationPage.Controls.Add(this.label1);
            this.counterIdentificationPage.Description = "Enter descriptive information about the counter";
            this.counterIdentificationPage.Location = new System.Drawing.Point(11, 71);
            this.counterIdentificationPage.Name = "counterIdentificationPage";
            this.counterIdentificationPage.NextPage = this.wmiCounterPage;
            this.counterIdentificationPage.PreviousPage = this.counterTypePage;
            this.counterIdentificationPage.Size = new System.Drawing.Size(610, 317);
            this.counterIdentificationPage.TabIndex = 1005;
            this.counterIdentificationPage.Text = "Counter Identification";
            this.counterIdentificationPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.counterIdentificationPage_BeforeMoveNext);
            // 
            // categoryComboBox1
            // 
            this.categoryComboBox1.FormattingEnabled = true;
            this.categoryComboBox1.Location = new System.Drawing.Point(119, 48);
            this.categoryComboBox1.MaxLength = 64;
            this.categoryComboBox1.Name = "categoryComboBox1";
            this.categoryComboBox1.Size = new System.Drawing.Size(458, 21);
            this.categoryComboBox1.TabIndex = 1;
            this.categoryComboBox1.DropDown += new System.EventHandler(this.categoryComboBox_DropDown);
            this.categoryComboBox1.TextChanged += new System.EventHandler(this.categoryComboBox_TextChanged);
            // 
            // descriptionTextBox1
            // 
            this.descriptionTextBox1.AcceptsReturn = true;
            this.descriptionTextBox1.Location = new System.Drawing.Point(119, 75);
            this.descriptionTextBox1.MaxLength = 512;
            this.descriptionTextBox1.Multiline = true;
            this.descriptionTextBox1.Name = "descriptionTextBox1";
            this.descriptionTextBox1.Size = new System.Drawing.Size(458, 226);
            this.descriptionTextBox1.TabIndex = 2;
            this.descriptionTextBox1.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Category:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Description:";
            // 
            // nameTextBox1
            // 
            this.nameTextBox1.Location = new System.Drawing.Point(119, 22);
            this.nameTextBox1.MaxLength = 128;
            this.nameTextBox1.Name = "nameTextBox1";
            this.nameTextBox1.Size = new System.Drawing.Size(458, 20);
            this.nameTextBox1.TabIndex = 0;
            this.nameTextBox1.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // azureMetricLabel
            // 
            this.azureMetricLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.azureMetricLabel.AutoSize = true;
            this.azureMetricLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.azureMetricLabel.Location = new System.Drawing.Point(33, 371);
            this.azureMetricLabel.Name = "azureMetricLabel";
            this.azureMetricLabel.Size = new System.Drawing.Size(0, 13);
            this.azureMetricLabel.TabIndex = 38;
            // 
            // CustomCounterWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 516);
            this.Controls.Add(this.wizard);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomCounterWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Custom Counter";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.CustomCounterWizard_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CustomCounterWizard_FormClosed);
            this.Load += new System.EventHandler(this.CustomCounterWizard_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.CustomCounterWizard_HelpRequested);
            this.wizard.ResumeLayout(false);
            this.introPage.ResumeLayout(false);
            this.introPage.PerformLayout();
            this.counterTypePage.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.wmiCounterPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.wmiManualModeContentPanel.ResumeLayout(false);
            this.wmiManualModeContentPanel.PerformLayout();
            this.wmiBrowseModeContentPanel.ResumeLayout(false);
            this.wmiBrowseModeContentPanel.PerformLayout();
            this.sqlCounterPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.sqlManualModeContentPanel.ResumeLayout(false);
            this.sqlManualModeContentPanel.PerformLayout();
            this.sqlBrowseModeContentPanel.ResumeLayout(false);
            this.sqlBrowseModeContentPanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.sqlBatchPage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.vmCounterPage.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.vmManualModeContentPanel.ResumeLayout(false);
            this.vmManualModeContentPanel.PerformLayout();
            this.vmBrowseModeContentPanel.ResumeLayout(false);
            this.vmBrowseModeContentPanel.PerformLayout();
            this.calculationTypePage.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.counterNamePage.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.alertDefinitionPage.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.criticalThresholdSpinner)).EndInit();
            this.finishPage1.ResumeLayout(false);
            this.azureServerConfigPage.ResumeLayout(false);
            this.azureServerConfigGroupBox.ResumeLayout(false);
            this.azureServerContentPanel.ResumeLayout(false);
            this.azureServerContentPanel.PerformLayout();
            this.azureServerMetricsPage.ResumeLayout(false);
            this.azureServerMetricsPage.PerformLayout();
            this.azureServerMetricsGroupBox.ResumeLayout(false);
            this.azureServerMetricsPanel.ResumeLayout(false);
            this.azureServerMetricsPanel.PerformLayout();
            this.counterIdentificationPage.ResumeLayout(false);
            this.counterIdentificationPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizard;
        private Divelements.WizardFramework.IntroductionPage introPage;
        private Divelements.WizardFramework.WizardPage counterIdentificationPage;
        private Divelements.WizardFramework.FinishPage finishPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox descriptionTextBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox nameTextBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox categoryComboBox1;
        private Divelements.WizardFramework.WizardPage wmiCounterPage;
        private Divelements.WizardFramework.WizardPage sqlCounterPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sqlInstanceNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sqlCounterNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sqlObjectNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Divelements.WizardFramework.WizardPage sqlBatchPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlBatchTextBox;
        private Divelements.WizardFramework.WizardPage counterTypePage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton sqlBatchButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton sqlCounterButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton wmiCounterButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton azureCounterButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Divelements.WizardFramework.WizardPage alertDefinitionPage;
        private Divelements.WizardFramework.WizardPage counterNamePage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown criticalThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown warningThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox enableAlertCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton wmiShowPerfCountersOnlyButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton wmiShowAllCountersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label23;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox wmiServerComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton wmiBrowseModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton wmiManualModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  wmiBrowseModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  wmiManualModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox wmiObjectNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label24;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton wmiInfoButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel wmiInstanceNameLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox wmiCounterNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox wmiInstanceNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox wmiInstanceNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox wmiCounterNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox wmiObjectNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel wmiStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sqlServerComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel counterSourceLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton sqlManualModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton sqlBrowseModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sqlBrowseModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sqlManualModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlInstanceNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlCounterNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sqlObjectNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label25;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label26;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label27;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel sqlStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label28;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label29;
        private MRG.Controls.UI.LoadingCircle wmiLoadingCircle;
        private MRG.Controls.UI.LoadingCircle sqlLoadingCircle;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel introductoryTextLabel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label30;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label31;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label32;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox alertNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label17;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox alertDescriptionTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox alertCategoryComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox hideWelcomePageCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox6;
        private Divelements.WizardFramework.WizardPage calculationTypePage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label19;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton calcTypeDeltaButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton calcTypeValueButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label33;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox counterScaleComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label34;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton comparisonTypeLessThanButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton comparisonTypeGreaterThanButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton alertAdvancedButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown infoThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton vmCounterButton;
        private Divelements.WizardFramework.WizardPage vmCounterPage;
        private Divelements.WizardFramework.WizardPage azureServerConfigPage;
        private Divelements.WizardFramework.WizardPage azureServerMetricsPage;

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label40;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton vmManualModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton vmBrowseModeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        private MRG.Controls.UI.LoadingCircle vmLoadingCircle;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel vmStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  vmManualModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox vmInstanceNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox vmCounterNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox vmObjectNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label20;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label21;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label22;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  vmBrowseModeContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox vmServerComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox vmCounterNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label35;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label36;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label37;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label38;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox vmObjectNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox vmInstanceNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label39;
        /// <summary>
        /// Added by Nandini
        /// azure Custom counter
        /// </summary>
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox azureServerConfigGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  azureServerContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox azureServerMetricsGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  azureServerMetricsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureNameSpaceNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureServerNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureObjectNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureServerLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureNameSpaceLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricValueLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureResourceTypeNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureMetricNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox sqlServerNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox databaseNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureResourceNameComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureResourceNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureDatabaseLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureDbComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureServerComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricPageServerNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricPageObjectNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricPageResourceGroupNameLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureProfileLbl;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox azureProfileComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton azureProfileButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel azureMetricLabel;

        ///
    }
}