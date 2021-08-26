
using Idera.SQLdm.DesktopClient.Controls.CustomControls;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AddServersWizard
    {
        //SQLdm 10.4 (Ruchika Salwan) -- Strings added for TopPlanCombobox
        private const string Duration = "Duration (milliseconds)";
        private const string LogicalDiskReads = "Logical disk reads";
        private const string CpuUsage = "CPU usage (milliseconds)";
        private const string PhysicalWrites = "Physical disk writes";
        //SQLdm 10.4 (Ruchika Salwan) -- Strings added for TopPlanCombobox
        private const string AWSRDSAccessKey = "AWS RDS Access Key";
        private const string AWSRDSSecretKey = "AWS RDS Secret Key";

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddServersWizard));
            this.wizardFramework = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CusotmWizard(); //new Divelements.WizardFramework.Wizard();
            this.welcomePage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomIntroductionPage();
            this.introductoryTextLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.introductoryTextLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.hideWelcomePageCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.configureAuthenticationPage = new Divelements.WizardFramework.WizardPage();
            this.addAzureServerButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.selectedAzureProfileLbl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.selectedAzureProfileNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.encryptDataCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.trustServerCertificateCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.passwordTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.passwordLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();//new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loginNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loginNameTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.useSqlServerAuthenticationRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.configureAuthenticationDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.useWindowsAuthenticationRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.selectInstancesPage = new Divelements.WizardFramework.WizardPage();
            this.availableLicensesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.availableInstancesStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loadingServersProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.addedInstancesListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.removeInstancesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addInstancesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.adhocInstancesTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.availableInstancesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.addedInstancesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.availableInstancesListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.configureCollectionPage = new Divelements.WizardFramework.WizardPage();
            this.featuresGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.groupBox6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.chkEnableActivityMonitor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.lblConfigureBlockedProcessThreshold = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.spnBlockedProcessThreshold = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.lblBlockedProcessesSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chkCaptureAutoGrow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.captureDeadlocksCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkCaptureBlocking = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.enableQueryMonitorTraceCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.groupBox5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - instantiating new checkboxes -start
            this.belowAnd2005CheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.aboveAnd2008CheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - instantiating new checkboxes -end            
            this.poorlyQueriesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.captureSqlBatchesCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.captureSqlStatementsCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.captureStoredProceduresCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.thresholdWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //SQLdm 10.4 (Ruchika Salwan) -- Add Top Plan 
            this.topPlanTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.topPlanSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.topPlanLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.topPlanSuffixLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.topPlanComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            //SQLdm 10.4 (Ruchika Salwan) -- Add Top Plan 
            this.traceThresholdsTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.durationThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.physicalWritesThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.physicalWritesThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.logicalReadsThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cpuThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.cpuThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logicalReadsThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.durationThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.queryMonitorAdvancedOptionsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.featuresDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.featuresIcon = new System.Windows.Forms.PictureBox();
            this.dataCollectionGroupBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanelDataCollection = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.dataCollectionDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dataCollectionDescriptionLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dataCollectionIcon = new System.Windows.Forms.PictureBox();
            this.collectionIntervalSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.wmiConfigPage = new Divelements.WizardFramework.WizardPage();
            this.groupBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.label24 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label28 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.directWmiPassword = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.directWmiUserName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.optionWmiDirect = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.optionWmiOleAutomation = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.optionWmiNone = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.optionWmiCSCreds = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.selectTagsPage = new Divelements.WizardFramework.WizardPage();
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cboAlertTemplates = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.groupBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.availableTagsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.addTagButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.availableTagsListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this.baselineConfiguration = new Divelements.WizardFramework.WizardPage();
            this.baselineConfiguration1 = new Idera.SQLdm.DesktopClient.Controls.BaselineConfigurationPage();
            this.loadAvailableServersWorker = new System.ComponentModel.BackgroundWorker();
            this.useSqlAunthenticationForAzureCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.wizardFramework.SuspendLayout();
            this.welcomePage.SuspendLayout();
            this.configureAuthenticationPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.selectInstancesPage.SuspendLayout();
            this.configureCollectionPage.SuspendLayout();
            this.featuresGroupBox.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBlockedProcessThreshold)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.traceThresholdsTableLayoutPanel.SuspendLayout();
            this.topPlanTableLayoutPanel.SuspendLayout(); //SQLdm 10.4 (Ruchika Salwan) -- Add Top Plan
            ((System.ComponentModel.ISupportInitialize)(this.durationThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalWritesThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanSpinner)).BeginInit(); //SQLdm 10.4 (Ruchika Salwan) -- Add Top Plan
            ((System.ComponentModel.ISupportInitialize)(this.logicalReadsThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.featuresIcon)).BeginInit();
            this.dataCollectionGroupBox.SuspendLayout();
            this.tableLayoutPanelDataCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataCollectionIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionIntervalSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanComboBox)).BeginInit(); //SQLdm 10.4 (Ruchika Salwan) -- Add Top Plan
            this.wmiConfigPage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.baselineConfiguration.SuspendLayout();
            this.selectTagsPage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlertTemplates)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing cloud provider id for all the added servers
            this.chooseCloudProviderPage = new Divelements.WizardFramework.WizardPage();
            this.grdCloudProviders = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.appearance21 = new Infragistics.Win.Appearance();
            this.appearance22 = new Infragistics.Win.Appearance();
            this.appearance23 = new Infragistics.Win.Appearance();
            this.appearance24 = new Infragistics.Win.Appearance();
            this.appearance25 = new Infragistics.Win.Appearance();
            this.appearance26 = new Infragistics.Win.Appearance();
            this.appearance27 = new Infragistics.Win.Appearance();
            this.appearance28 = new Infragistics.Win.Appearance();
            this.appearance29 = new Infragistics.Win.Appearance();
            this.appearance30 = new Infragistics.Win.Appearance();
            //-----------------------------------------------------------------------------------------------------------------------------------
            ((System.ComponentModel.ISupportInitialize)(this.grdCloudProviders)).BeginInit();
            this.chooseCloudProviderPage.SuspendLayout();
            //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing cloud provider id for all the added servers            
            // 
            // wizardFramework
            // 
            this.wizardFramework.AnimatePageTransitions = false;
            this.wizardFramework.BannerImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardBannerImage;
            this.wizardFramework.Controls.Add(this.welcomePage);
            this.wizardFramework.Controls.Add(this.configureAuthenticationPage);
            this.wizardFramework.Controls.Add(this.configureCollectionPage);
            this.wizardFramework.Controls.Add(this.selectTagsPage);
            this.wizardFramework.Controls.Add(this.selectInstancesPage);
            this.wizardFramework.Controls.Add(this.wmiConfigPage);
            this.wizardFramework.Controls.Add(this.baselineConfiguration);
            this.wizardFramework.Controls.Add(this.chooseCloudProviderPage);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud provider for all the added instances
            this.wizardFramework.Location = new System.Drawing.Point(0, 0);
            this.wizardFramework.MarginImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardWelcomePageMarginImage;
            this.wizardFramework.Name = "wizardFramework";
            this.wizardFramework.OwnerForm = this;
            this.wizardFramework.Size = new System.Drawing.Size(584, 552);
            this.wizardFramework.TabIndex = 0;
            this.wizardFramework.Margin = new Padding(2, 2, 2, 2);
            this.wizardFramework.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizardFramework.Cancel += new System.EventHandler(this.wizardFramework_Cancel);
            this.wizardFramework.Finish += new System.EventHandler(this.wizardFramework_Finish);

            // 
            // welcomePage
            // 
            this.welcomePage.Controls.Add(this.introductoryTextLabel1);
            this.welcomePage.Controls.Add(this.introductoryTextLabel2);
            this.welcomePage.Controls.Add(this.hideWelcomePageCheckBox);
            this.welcomePage.IntroductionText = "";
            this.welcomePage.Location = new System.Drawing.Point(175, 71);
            this.welcomePage.Name = "welcomePage";
            this.welcomePage.NextPage = this.configureAuthenticationPage;
            this.welcomePage.ProceedText = "";
            this.welcomePage.Size = new System.Drawing.Size(387, 423);
            this.welcomePage.TabIndex = 1004;
            // this.welcomePage.Text = "Welcome to the Add Servers Wizard";
            this.welcomePage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.welcomePage_HelpRequested);
            // 
            // introductoryTextLabel1
            // 
            this.introductoryTextLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            this.introductoryTextLabel1.Location = new System.Drawing.Point(0, 0);
            this.introductoryTextLabel1.Name = "introductoryTextLabel1";
            this.introductoryTextLabel1.Size = new System.Drawing.Size(337, 55);
            this.introductoryTextLabel1.TabIndex = 2;
            this.introductoryTextLabel1.Text = "This wizard helps you quickly add new monitored SQL Servers. The following config" +
    "uration options are available:";
            // 
            // introductoryTextLabel2
            // 
            this.introductoryTextLabel2.Location = new System.Drawing.Point(20, 55);
            this.introductoryTextLabel2.AutoSize = true;
            this.introductoryTextLabel2.MaximumSize = new System.Drawing.Size(400, 204);
            this.introductoryTextLabel2.Name = "introductoryTextLabel2";
            this.introductoryTextLabel2.Size = new System.Drawing.Size(400, 204);
            //this.introductoryTextLabel2.Size = new System.Drawing.Size(357, 254);
            this.introductoryTextLabel2.TabIndex = 1;
            this.introductoryTextLabel2.Text = resources.GetString("introductoryTextLabel2.Text");
            // 
            // hideWelcomePageCheckBox
            // 
            this.hideWelcomePageCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hideWelcomePageCheckBox.AutoSize = true;
            this.hideWelcomePageCheckBox.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.HideAddServersWizardWelcomePage;
            this.hideWelcomePageCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "HideAddServersWizardWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hideWelcomePageCheckBox.Location = new System.Drawing.Point(3, 402);
            this.hideWelcomePageCheckBox.Name = "hideWelcomePageCheckBox";
            this.hideWelcomePageCheckBox.Size = new System.Drawing.Size(199, 17);
            this.hideWelcomePageCheckBox.TabIndex = 0;
            this.hideWelcomePageCheckBox.Text = "Don\'t show this welcome page again";
            this.hideWelcomePageCheckBox.UseVisualStyleBackColor = true;
            this.hideWelcomePageCheckBox.CheckedChanged += new System.EventHandler(this.hideWelcomePageCheckBox_CheckedChanged);
            // 
            // configureAuthenticationPage
            // 
            this.configureAuthenticationPage.Controls.Add(this.useSqlAunthenticationForAzureCheckBox);
            this.configureAuthenticationPage.Controls.Add(this.addAzureServerButton);
            this.configureAuthenticationPage.Controls.Add(this.groupBox1);
            this.configureAuthenticationPage.Controls.Add(this.passwordTextbox);
            this.configureAuthenticationPage.Controls.Add(this.passwordLabel);
            this.configureAuthenticationPage.Controls.Add(this.loginNameLabel);
            this.configureAuthenticationPage.Controls.Add(this.loginNameTextbox);
            this.configureAuthenticationPage.Controls.Add(this.useSqlServerAuthenticationRadioButton);
            this.configureAuthenticationPage.Controls.Add(this.configureAuthenticationDescriptionLabel);
            this.configureAuthenticationPage.Controls.Add(this.useWindowsAuthenticationRadioButton);
            this.configureAuthenticationPage.Controls.Add(this.selectedAzureProfileLbl);
            this.configureAuthenticationPage.Controls.Add(this.selectedAzureProfileNameTextBox);
            this.configureAuthenticationPage.Controls.Add(this.informationBox1);
            this.configureAuthenticationPage.Description = "Select the authentication mode and credentials SQL Diagnostic Manager should use " +
    "when collecting diagnostic data from the monitored SQL Server instances.";
            this.configureAuthenticationPage.Location = new System.Drawing.Point(11, 71);
            this.configureAuthenticationPage.Name = "configureAuthenticationPage";
            this.configureAuthenticationPage.NextPage = this.selectInstancesPage;
            this.configureAuthenticationPage.PreviousPage = this.welcomePage;
            this.configureAuthenticationPage.Size = new System.Drawing.Size(562, 423);
            this.configureAuthenticationPage.TabIndex = 1007;
            this.configureAuthenticationPage.Text = "Configure Authentication";
            this.configureAuthenticationPage.BeforeDisplay += new System.EventHandler(this.configureAuthenticationPage_BeforeDisplay);
            this.configureAuthenticationPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.configureAuthenticationPage_HelpRequested);
            // 
            // addAzureServerButton
            // 
            this.addAzureServerButton.AutoSize = true;
            this.addAzureServerButton.Location = new System.Drawing.Point(93, 338);
            this.addAzureServerButton.Name = "addAzureServerCheckBox";
            this.addAzureServerButton.Size = new System.Drawing.Size(173, 17);
            this.addAzureServerButton.TabIndex = 26;
            this.addAzureServerButton.Text = "Azure Discovery Settings";
            this.addAzureServerButton.UseVisualStyleBackColor = true;
            this.addAzureServerButton.Click += new System.EventHandler(this.addAzureServerButton_Click);
            // 
            // selectedAzureProfileLbl
            // 
            this.selectedAzureProfileLbl.AutoSize = true;
            this.selectedAzureProfileLbl.Location = new System.Drawing.Point(126, 374);
            this.selectedAzureProfileLbl.Name = "selectedAzureProfileLbl";
            this.selectedAzureProfileLbl.Size = new System.Drawing.Size(111, 13);
            this.selectedAzureProfileLbl.TabIndex = 1;
            this.selectedAzureProfileLbl.Text = "Selected Azure Profile";
            this.selectedAzureProfileLbl.Visible = false;
            // 
            // selectedAzureProfileNameTextBox
            // 
            this.selectedAzureProfileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedAzureProfileNameTextBox.Location = new System.Drawing.Point(262, 371);
            this.selectedAzureProfileNameTextBox.Name = "selectedAzureProfileNameTextBox";
            this.selectedAzureProfileNameTextBox.Size = new System.Drawing.Size(230, 20);
            this.selectedAzureProfileNameTextBox.TabIndex = 25;
            this.selectedAzureProfileNameTextBox.Visible = false;
            this.selectedAzureProfileNameTextBox.Enabled = false;
            // 
            // useSqlAunthenticationForAzureCheckBox
            // 
            this.useSqlAunthenticationForAzureCheckBox.AutoSize = true;
            this.useSqlAunthenticationForAzureCheckBox.Location = new System.Drawing.Point(97, 219);
            this.useSqlAunthenticationForAzureCheckBox.Name = "useSqlAunthenticationForAzureCheckBox";
            this.useSqlAunthenticationForAzureCheckBox.Size = new System.Drawing.Size(264, 17);
            this.useSqlAunthenticationForAzureCheckBox.TabIndex = 27;
            this.useSqlAunthenticationForAzureCheckBox.Text = "Enable Azure Discovery Settings";
            this.useSqlAunthenticationForAzureCheckBox.UseVisualStyleBackColor = true;
            this.useSqlAunthenticationForAzureCheckBox.Enabled = false;
            this.useSqlAunthenticationForAzureCheckBox.CheckedChanged += new System.EventHandler(this.useSqlAunthenticationForAzureCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.encryptDataCheckbox);
            this.groupBox1.Controls.Add(this.trustServerCertificateCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(96, 252);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 71);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Encryption Options";
            // 
            // encryptDataCheckbox
            // 
            this.encryptDataCheckbox.AutoSize = true;
            this.encryptDataCheckbox.Location = new System.Drawing.Point(22, 21);
            this.encryptDataCheckbox.Name = "encryptDataCheckbox";
            this.encryptDataCheckbox.Size = new System.Drawing.Size(148, 17);
            this.encryptDataCheckbox.TabIndex = 22;
            this.encryptDataCheckbox.Text = "Encrypt Connection (SSL)";
            this.encryptDataCheckbox.UseVisualStyleBackColor = true;
            this.encryptDataCheckbox.CheckedChanged += new System.EventHandler(this.encryptDataCheckbox_CheckedChanged);
            // 
            // trustServerCertificateCheckbox
            // 
            this.trustServerCertificateCheckbox.AutoSize = true;
            this.trustServerCertificateCheckbox.Enabled = false;
            this.trustServerCertificateCheckbox.Location = new System.Drawing.Point(22, 44);
            this.trustServerCertificateCheckbox.Name = "trustServerCertificateCheckbox";
            this.trustServerCertificateCheckbox.Size = new System.Drawing.Size(273, 17);
            this.trustServerCertificateCheckbox.TabIndex = 23;
            this.trustServerCertificateCheckbox.Text = "Trust Server Certificate (bypass certificate validation)";
            this.trustServerCertificateCheckbox.UseVisualStyleBackColor = true;
            // 
            // passwordTextbox
            // 
            this.passwordTextbox.Enabled = false;
            this.passwordTextbox.Location = new System.Drawing.Point(202, 186);
            this.passwordTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.passwordTextbox.Name = "passwordTextbox";
            this.passwordTextbox.Size = new System.Drawing.Size(290, 20);
            this.passwordTextbox.TabIndex = 3;
            this.passwordTextbox.UseSystemPasswordChar = true;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(115, 189);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordLabel.TabIndex = 12;
            this.passwordLabel.Text = "Password:";
            // 
            // loginNameLabel
            // 
            this.loginNameLabel.AutoSize = true;
            this.loginNameLabel.Location = new System.Drawing.Point(115, 163);
            this.loginNameLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.loginNameLabel.Name = "loginNameLabel";
            this.loginNameLabel.Size = new System.Drawing.Size(65, 13);
            this.loginNameLabel.TabIndex = 11;
            this.loginNameLabel.Text = "Login name:";
            // 
            // loginNameTextbox
            // 
            this.loginNameTextbox.Enabled = false;
            this.loginNameTextbox.Location = new System.Drawing.Point(202, 160);
            this.loginNameTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.loginNameTextbox.Name = "loginNameTextbox";
            this.loginNameTextbox.Size = new System.Drawing.Size(290, 20);
            this.loginNameTextbox.TabIndex = 2;
            this.loginNameTextbox.TextChanged += new System.EventHandler(this.loginNameTextbox_TextChanged);
            // 
            // useSqlServerAuthenticationRadioButton
            // 
            this.useSqlServerAuthenticationRadioButton.AutoSize = true;
            this.useSqlServerAuthenticationRadioButton.Location = new System.Drawing.Point(97, 136);
            this.useSqlServerAuthenticationRadioButton.Name = "useSqlServerAuthenticationRadioButton";
            this.useSqlServerAuthenticationRadioButton.Size = new System.Drawing.Size(151, 17);
            this.useSqlServerAuthenticationRadioButton.TabIndex = 1;
            this.useSqlServerAuthenticationRadioButton.Text = "SQL Server Authentication";
            this.useSqlServerAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.useSqlServerAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.useSqlServerAuthenticationRadioButton_CheckedChanged);
            // 
            // configureAuthenticationDescriptionLabel
            // 
            this.configureAuthenticationDescriptionLabel.Location = new System.Drawing.Point(67, 88);
            this.configureAuthenticationDescriptionLabel.Name = "configureAuthenticationDescriptionLabel";
            this.configureAuthenticationDescriptionLabel.Size = new System.Drawing.Size(425, 16);
            this.configureAuthenticationDescriptionLabel.TabIndex = 1;
            this.configureAuthenticationDescriptionLabel.Text = "Connect using:";
            // 
            // useWindowsAuthenticationRadioButton
            // 
            this.useWindowsAuthenticationRadioButton.AutoSize = true;
            this.useWindowsAuthenticationRadioButton.Checked = true;
            this.useWindowsAuthenticationRadioButton.Location = new System.Drawing.Point(97, 113);
            this.useWindowsAuthenticationRadioButton.Name = "useWindowsAuthenticationRadioButton";
            this.useWindowsAuthenticationRadioButton.Size = new System.Drawing.Size(140, 17);
            this.useWindowsAuthenticationRadioButton.TabIndex = 0;
            this.useWindowsAuthenticationRadioButton.TabStop = true;
            this.useWindowsAuthenticationRadioButton.Text = "Windows Authentication";
            this.useWindowsAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.useWindowsAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.useWindowsAuthenticationRadioButton_CheckedChanged);
            //
            // informationBox1
            // 
            this.informationBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.informationBox1.Location = new System.Drawing.Point(30, 3);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(500, 81);
            this.informationBox1.TabIndex = 14;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");

            //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud providers
            // 
            // chooseCloudProviderPage
            //             
            this.chooseCloudProviderPage.Controls.Add(this.grdCloudProviders);
            // SQLdm 10.3 (Varun Chopra) SQL Server 2017 and Linux Support in UI

            this.chooseCloudProviderPage.Description = "Configure the type of server for each of the added SQL Server instances.";
            this.chooseCloudProviderPage.Location = new System.Drawing.Point(11, 71);
            this.chooseCloudProviderPage.Name = "chooseCloudProviderPage";
            this.chooseCloudProviderPage.NextPage = this.configureCollectionPage;
            this.chooseCloudProviderPage.PreviousPage = this.selectInstancesPage;
            this.chooseCloudProviderPage.Size = new System.Drawing.Size(562, 423);
            this.chooseCloudProviderPage.TabIndex = 0;
            // SQLdm 10.3 (Varun Chopra) SQL Server 2017 and Linux Support in UI

            this.chooseCloudProviderPage.Text = "Select the server type for the added servers";
            this.chooseCloudProviderPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.chooseCloudProviderPage_BeforeMoveNext);
            this.chooseCloudProviderPage.BeforeDisplay += new System.EventHandler(this.chooseCloudProviderPage_BeforeDisplay);
            this.chooseCloudProviderPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.chooseCloudProviderPage_HelpRequested);

            // 
            // grdCloudProviders
            // 
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdCloudProviders.DisplayLayout.Appearance = appearance21;
            this.grdCloudProviders.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.grdCloudProviders.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.grdCloudProviders.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance22.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.grdCloudProviders.DisplayLayout.GroupByBox.Appearance = appearance22;
            appearance23.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdCloudProviders.DisplayLayout.GroupByBox.BandLabelAppearance = appearance23;
            this.grdCloudProviders.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdCloudProviders.DisplayLayout.GroupByBox.Hidden = true;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance24.BackColor2 = System.Drawing.SystemColors.Control;
            appearance24.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance24.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdCloudProviders.DisplayLayout.GroupByBox.PromptAppearance = appearance24;
            this.grdCloudProviders.DisplayLayout.MaxColScrollRegions = 1;
            this.grdCloudProviders.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdCloudProviders.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdCloudProviders.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdCloudProviders.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdCloudProviders.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdCloudProviders.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            this.grdCloudProviders.DisplayLayout.Override.CardAreaAppearance = appearance25;
            appearance26.BorderColor = System.Drawing.Color.Silver;
            appearance26.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdCloudProviders.DisplayLayout.Override.CellAppearance = appearance26;
            this.grdCloudProviders.DisplayLayout.Override.CellPadding = 6;
            this.grdCloudProviders.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.grdCloudProviders.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance27.BackColor = System.Drawing.SystemColors.Control;
            appearance27.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance27.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance27.BorderColor = System.Drawing.SystemColors.Window;
            this.grdCloudProviders.DisplayLayout.Override.GroupByRowAppearance = appearance27;
            this.grdCloudProviders.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance28.TextHAlignAsString = "Left";
            this.grdCloudProviders.DisplayLayout.Override.HeaderAppearance = appearance28;
            this.grdCloudProviders.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.Color.Silver;
            this.grdCloudProviders.DisplayLayout.Override.RowAppearance = appearance29;
            this.grdCloudProviders.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdCloudProviders.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdCloudProviders.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdCloudProviders.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdCloudProviders.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdCloudProviders.DisplayLayout.Override.TemplateAddRowAppearance = appearance30;
            this.grdCloudProviders.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdCloudProviders.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdCloudProviders.DisplayLayout.UseFixedHeaders = true;
            this.grdCloudProviders.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdCloudProviders.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grdCloudProviders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdCloudProviders.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdCloudProviders.Location = new System.Drawing.Point(0, 0);
            this.grdCloudProviders.Name = "grdCloudProviders";
            this.grdCloudProviders.Size = new System.Drawing.Size(587, 383);
            this.grdCloudProviders.DisplayLayout.Override.DefaultColWidth = 250;
            this.grdCloudProviders.TabIndex = 33;
            this.grdCloudProviders.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdCloudProviders_KeyPress);
            this.grdCloudProviders.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdCloudProviders_MouseDown);
            this.grdCloudProviders.CellChange += GrdCloudProviders_CellChange;
            //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud providers

            // 
            // selectInstancesPage
            // 
            this.selectInstancesPage.Controls.Add(this.availableLicensesLabel);
            this.availableInstancesListBox.Controls.Add(this.availableInstancesStatusLabel);
            this.selectInstancesPage.Controls.Add(this.loadingServersProgressControl);
            this.selectInstancesPage.Controls.Add(this.addedInstancesListBox);
            this.selectInstancesPage.Controls.Add(this.removeInstancesButton);
            this.selectInstancesPage.Controls.Add(this.addInstancesButton);
            this.selectInstancesPage.Controls.Add(this.adhocInstancesTextBox);
            this.selectInstancesPage.Controls.Add(this.availableInstancesLabel);
            this.selectInstancesPage.Controls.Add(this.addedInstancesLabel);
            this.selectInstancesPage.Controls.Add(this.availableInstancesListBox);
            this.selectInstancesPage.Description = "Select the SQL Server instances you would like to monitor with SQL diagnostic man" +
    "ager.";
            this.selectInstancesPage.Location = new System.Drawing.Point(11, 71);
            this.selectInstancesPage.Name = "selectInstancesPage";
            this.selectInstancesPage.NextPage = this.chooseCloudProviderPage;
            this.selectInstancesPage.PreviousPage = this.configureAuthenticationPage;
            this.selectInstancesPage.Size = new System.Drawing.Size(562, 423);
            this.selectInstancesPage.TabIndex = 0;
            this.selectInstancesPage.Text = "Select Servers To Monitor";
            this.selectInstancesPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.selectInstancesPage_BeforeMoveNext);
            this.selectInstancesPage.BeforeDisplay += new System.EventHandler(this.selectInstancesPage_BeforeDisplay);
            this.selectInstancesPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.selectInstancesPage_HelpRequested);
            // 
            // availableLicensesLabel
            // 
            this.availableLicensesLabel.AutoSize = true;
            this.availableLicensesLabel.Location = new System.Drawing.Point(324, 58);
            this.availableLicensesLabel.Name = "availableLicensesLabel";
            this.availableLicensesLabel.Size = new System.Drawing.Size(115, 13);
            this.availableLicensesLabel.TabIndex = 12;
            this.availableLicensesLabel.Text = "Available Licenses: {0}";


            // 
            // availableInstancesStatusLabel
            // 
            this.availableInstancesStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.availableInstancesStatusLabel.BackColor = System.Drawing.Color.White;
            this.availableInstancesStatusLabel.Enabled = false;
            this.availableInstancesStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableInstancesStatusLabel.Name = "availableInstancesStatusLabel";
            this.availableInstancesStatusLabel.Size = new System.Drawing.Size(200, 288);
            this.availableInstancesStatusLabel.TabIndex = 5;
            this.availableInstancesStatusLabel.Text = "< Unavailable >";
            this.availableInstancesStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.availableInstancesStatusLabel.Visible = false;
            // 
            // loadingServersProgressControl
            // 
            this.loadingServersProgressControl.Active = false;
            this.loadingServersProgressControl.BackColor = System.Drawing.Color.White;
            this.loadingServersProgressControl.Color = System.Drawing.Color.DarkGray;
            this.loadingServersProgressControl.InnerCircleRadius = 5;
            this.loadingServersProgressControl.Location = new System.Drawing.Point(37, 106);
            this.loadingServersProgressControl.Name = "loadingServersProgressControl";
            this.loadingServersProgressControl.NumberSpoke = 12;
            this.loadingServersProgressControl.OuterCircleRadius = 11;
            this.loadingServersProgressControl.RotationSpeed = 80;
            this.loadingServersProgressControl.Size = new System.Drawing.Size(200, 257);
            this.loadingServersProgressControl.SpokeThickness = 2;
            this.loadingServersProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingServersProgressControl.TabIndex = 3;
            this.loadingServersProgressControl.TabStop = false;
            // 
            // addedInstancesListBox
            // 
            this.addedInstancesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.addedInstancesListBox.DisplayMember = "Text";
            this.addedInstancesListBox.FormattingEnabled = true;
            this.addedInstancesListBox.HorizontalScrollbar = true;
            this.addedInstancesListBox.Location = new System.Drawing.Point(324, 90);
            this.addedInstancesListBox.Name = "addedInstancesListBox";
            this.addedInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.addedInstancesListBox.Size = new System.Drawing.Size(202, 303);
            this.addedInstancesListBox.Sorted = true;
            this.addedInstancesListBox.TabIndex = 4;
            this.addedInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.addedInstancesListBox_SelectedIndexChanged);
            this.addedInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.addedInstancesListBox_MouseDoubleClick);
            // 
            // removeInstancesButton
            // 
            this.removeInstancesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeInstancesButton.Enabled = false;
            this.removeInstancesButton.Location = new System.Drawing.Point(244, 260);
            this.removeInstancesButton.Name = "removeInstancesButton";
            this.removeInstancesButton.Size = new System.Drawing.Size(75, 23);
            this.removeInstancesButton.TabIndex = 2;
            this.removeInstancesButton.Text = "Remove";
            this.removeInstancesButton.UseVisualStyleBackColor = true;
            this.removeInstancesButton.Click += new System.EventHandler(this.removeInstancesButton_Click);
            // 
            // addInstancesButton
            // 
            this.addInstancesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addInstancesButton.Enabled = false;
            this.addInstancesButton.Location = new System.Drawing.Point(244, 231);
            this.addInstancesButton.Name = "addInstancesButton";
            this.addInstancesButton.Size = new System.Drawing.Size(75, 23);
            this.addInstancesButton.TabIndex = 1;
            this.addInstancesButton.Text = "Add";
            this.addInstancesButton.UseVisualStyleBackColor = true;
            this.addInstancesButton.Click += new System.EventHandler(this.addInstancesButton_Click);
            // 
            // adhocInstancesTextBox
            // 
            this.adhocInstancesTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.adhocInstancesTextBox.Location = new System.Drawing.Point(36, 79);
            this.adhocInstancesTextBox.Name = "adhocInstancesTextBox";
            this.adhocInstancesTextBox.Size = new System.Drawing.Size(203, 20);
            this.adhocInstancesTextBox.TabIndex = 0;
            this.adhocInstancesTextBox.Text = "< Type comma separated names >";
            this.adhocInstancesTextBox.TextChanged += new System.EventHandler(this.adhocInstancesTextBox_TextChanged);
            this.adhocInstancesTextBox.Enter += new System.EventHandler(this.adhocInstancesTextBox_Enter);
            this.adhocInstancesTextBox.Leave += new System.EventHandler(this.adhocInstancesTextBox_Enter);
            // 
            // availableInstancesLabel
            // 
            this.availableInstancesLabel.AutoSize = true;
            this.availableInstancesLabel.Location = new System.Drawing.Point(39, 59);
            this.availableInstancesLabel.Name = "availableInstancesLabel";
            this.availableInstancesLabel.Size = new System.Drawing.Size(92, 13);
            this.availableInstancesLabel.TabIndex = 3;
            this.availableInstancesLabel.Text = "Available Servers:";
            // 
            // addedInstancesLabel
            // 
            this.addedInstancesLabel.AutoSize = true;
            this.addedInstancesLabel.Location = new System.Drawing.Point(324, 74);
            this.addedInstancesLabel.Name = "addedInstancesLabel";
            this.addedInstancesLabel.Size = new System.Drawing.Size(97, 13);
            this.addedInstancesLabel.TabIndex = 3;
            this.addedInstancesLabel.Text = "Added Servers: {0}";
            // 
            // availableInstancesListBox
            // 
            this.availableInstancesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.availableInstancesListBox.DisplayMember = "Text";
            this.availableInstancesListBox.FormattingEnabled = true;
            this.availableInstancesListBox.HorizontalScrollbar = true;
            this.availableInstancesListBox.Location = new System.Drawing.Point(36, 103);
            this.availableInstancesListBox.Name = "availableInstancesListBox";
            this.availableInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableInstancesListBox.Size = new System.Drawing.Size(203, 290);
            this.availableInstancesListBox.Sorted = true;
            this.availableInstancesListBox.TabIndex = 3;
            this.availableInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.availableInstancesListBox_SelectedIndexChanged);
            this.availableInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableInstancesListBox_MouseDoubleClick);
            // 
            // configureCollectionPage
            // 
            this.configureCollectionPage.Controls.Add(this.featuresGroupBox);
            this.configureCollectionPage.Controls.Add(this.dataCollectionGroupBox);
            this.configureCollectionPage.Description = "Configure the collection interval and SQL Diagnostic Manager features that should" +
    " be enabled for the monitored SQL Server instances.";
            this.configureCollectionPage.Location = new System.Drawing.Point(11, 71);
            this.configureCollectionPage.Name = "configureCollectionPage";
            this.configureCollectionPage.NextPage = this.wmiConfigPage;
            this.configureCollectionPage.PreviousPage = this.chooseCloudProviderPage;
            this.configureCollectionPage.Size = new System.Drawing.Size(562, 423);
            this.configureCollectionPage.TabIndex = 0;
            this.configureCollectionPage.Text = "Configure SQL Diagnostic Manager Collection";
            this.configureCollectionPage.BeforeDisplay += new System.EventHandler(this.configureCollectionPage_BeforeDisplay);
            this.configureCollectionPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.configureCollectionPage_BeforeMoveNext);//SQLdm 9.0 (Ankit Srivastava) -- Resolved defect DE44129 -- Add server wizard -handler to check if user has not chosen any of the SQL server versions.
            this.configureCollectionPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.configureCollectionPage_HelpRequested);
            // 
            // featuresGroupBox
            // 
            this.featuresGroupBox.Controls.Add(this.groupBox6);
            this.featuresGroupBox.Controls.Add(this.enableQueryMonitorTraceCheckBox);
            this.featuresGroupBox.Controls.Add(this.groupBox5);
            this.featuresGroupBox.Controls.Add(this.queryMonitorAdvancedOptionsButton);
            this.featuresGroupBox.Controls.Add(this.featuresDescriptionLabel);
            this.featuresGroupBox.Controls.Add(this.featuresIcon);
            this.featuresGroupBox.Location = new System.Drawing.Point(19, 62);
            this.featuresGroupBox.Name = "featuresGroupBox";
            this.featuresGroupBox.Size = new System.Drawing.Size(525, 355); //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            this.featuresGroupBox.TabIndex = 1;
            this.featuresGroupBox.TabStop = false;
            this.featuresGroupBox.Text = "Features";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkEnableActivityMonitor);
            this.groupBox6.Controls.Add(this.lblConfigureBlockedProcessThreshold);
            this.groupBox6.Controls.Add(this.tableLayoutPanel1);
            this.groupBox6.Controls.Add(this.chkCaptureAutoGrow);
            this.groupBox6.Controls.Add(this.captureDeadlocksCheckBox);
            this.groupBox6.Controls.Add(this.chkCaptureBlocking);
            this.groupBox6.Location = new System.Drawing.Point(281, 78); //SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(237, 203);//SQLdm 9.0 (Ankit Srivastava) -- size changed to fit new UI changes
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            // 
            // chkEnableActivityMonitor
            // 
            this.chkEnableActivityMonitor.AutoSize = true;
            this.chkEnableActivityMonitor.Checked = true;
            this.chkEnableActivityMonitor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableActivityMonitor.Location = new System.Drawing.Point(11, -2);
            this.chkEnableActivityMonitor.Name = "chkEnableActivityMonitor";
            this.chkEnableActivityMonitor.Size = new System.Drawing.Size(216, 17);
            this.chkEnableActivityMonitor.TabIndex = 11;
            this.chkEnableActivityMonitor.Text = "Enable monitoring of non-query activities";
            this.chkEnableActivityMonitor.UseVisualStyleBackColor = true;
            this.chkEnableActivityMonitor.CheckedChanged += new System.EventHandler(this.chkEnableActivityMonitor_CheckedChanged);
            // 
            // lblConfigureBlockedProcessThreshold
            // 
            this.lblConfigureBlockedProcessThreshold.Location = new System.Drawing.Point(11, 85);
            this.lblConfigureBlockedProcessThreshold.Name = "lblConfigureBlockedProcessThreshold";
            this.lblConfigureBlockedProcessThreshold.Size = new System.Drawing.Size(236, 46);
            this.lblConfigureBlockedProcessThreshold.TabIndex = 12;
            this.lblConfigureBlockedProcessThreshold.Text = "Configure the blocked process threshold (the frequency at which SQL (2005+) Serve" +
    "r will check for blocking sessions):";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.5F));
            this.tableLayoutPanel1.Controls.Add(this.spnBlockedProcessThreshold, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblBlockedProcessesSpinner, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 133);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.97959F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.02041F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 49);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // spnBlockedProcessThreshold
            // 
            this.spnBlockedProcessThreshold.Location = new System.Drawing.Point(158, 3);
            this.spnBlockedProcessThreshold.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.Name = "spnBlockedProcessThreshold";
            this.spnBlockedProcessThreshold.Size = new System.Drawing.Size(70, 20);
            this.spnBlockedProcessThreshold.TabIndex = 9;
            this.spnBlockedProcessThreshold.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.TextChanged += new System.EventHandler(this.SpnBlockedProcessThreshold_TextChanged);
            // 
            // lblBlockedProcessesSpinner
            // 
            this.lblBlockedProcessesSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBlockedProcessesSpinner.AutoSize = true;
            this.lblBlockedProcessesSpinner.Location = new System.Drawing.Point(3, 5);
            this.lblBlockedProcessesSpinner.Name = "lblBlockedProcessesSpinner";
            this.lblBlockedProcessesSpinner.Size = new System.Drawing.Size(143, 13);
            this.lblBlockedProcessesSpinner.TabIndex = 10;
            this.lblBlockedProcessesSpinner.Text = "Blocked process threshold(s)";
            // 
            // chkCaptureAutoGrow
            // 
            this.chkCaptureAutoGrow.AutoSize = true;
            this.chkCaptureAutoGrow.Checked = true;
            this.chkCaptureAutoGrow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCaptureAutoGrow.Location = new System.Drawing.Point(11, 18);
            this.chkCaptureAutoGrow.Name = "chkCaptureAutoGrow";
            this.chkCaptureAutoGrow.Size = new System.Drawing.Size(146, 17);
            this.chkCaptureAutoGrow.TabIndex = 8;
            this.chkCaptureAutoGrow.Text = "Capture Autogrow events";
            this.chkCaptureAutoGrow.UseVisualStyleBackColor = true;
            // 
            // captureDeadlocksCheckBox
            // 
            this.captureDeadlocksCheckBox.AutoSize = true;
            this.captureDeadlocksCheckBox.Checked = true;
            this.captureDeadlocksCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captureDeadlocksCheckBox.Location = new System.Drawing.Point(11, 41);
            this.captureDeadlocksCheckBox.Name = "captureDeadlocksCheckBox";
            this.captureDeadlocksCheckBox.Size = new System.Drawing.Size(180, 17);
            this.captureDeadlocksCheckBox.TabIndex = 1;
            this.captureDeadlocksCheckBox.Text = "Capture Deadlocks (SQL 2005+)";
            this.captureDeadlocksCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkCaptureBlocking
            // 
            this.chkCaptureBlocking.AutoSize = true;
            this.chkCaptureBlocking.Checked = true;
            this.chkCaptureBlocking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCaptureBlocking.Location = new System.Drawing.Point(11, 64);
            this.chkCaptureBlocking.Name = "chkCaptureBlocking";
            this.chkCaptureBlocking.Size = new System.Drawing.Size(170, 17);
            this.chkCaptureBlocking.TabIndex = 7;
            this.chkCaptureBlocking.Text = "Capture Blocking (SQL 2005+)";
            this.chkCaptureBlocking.UseVisualStyleBackColor = true;
            this.chkCaptureBlocking.CheckedChanged += new System.EventHandler(this.chkCaptureBlocking_CheckedChanged);
            // 
            // enableQueryMonitorTraceCheckBox
            // 
            this.enableQueryMonitorTraceCheckBox.AutoSize = true;
            this.enableQueryMonitorTraceCheckBox.Checked = true;
            //this.enableQueryMonitorTraceCheckBox.Location = new System.Drawing.Point(19, 76);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.enableQueryMonitorTraceCheckBox.Location = new System.Drawing.Point(25, 58); //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            this.enableQueryMonitorTraceCheckBox.Name = "enableQueryMonitorTraceCheckBox";
            this.enableQueryMonitorTraceCheckBox.Size = new System.Drawing.Size(146, 17);
            this.enableQueryMonitorTraceCheckBox.TabIndex = 0;
            this.enableQueryMonitorTraceCheckBox.Text = "Enable the Query Monitor";
            this.enableQueryMonitorTraceCheckBox.UseVisualStyleBackColor = true;
            this.enableQueryMonitorTraceCheckBox.CheckedChanged += new System.EventHandler(this.enableQueryMonitorTraceCheckBox_CheckedChanged);
            this.enableQueryMonitorTraceCheckBox.CheckState = System.Windows.Forms.CheckState.Unchecked;//SQLdm 10.1.1 (Srishti Purohit) -- disabled by default
                                                                                                        // 
                                                                                                        // groupBox5
                                                                                                        // 
                                                                                                        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard --new controls added to the groupbox-start                                     
            this.groupBox5.Controls.Add(this.belowAnd2005CheckBox);
            this.groupBox5.Controls.Add(this.aboveAnd2008CheckBox);
            //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard --new controls added to the groupbox- end
            this.groupBox5.Controls.Add(this.poorlyQueriesLabel);
            this.groupBox5.Controls.Add(this.captureSqlBatchesCheckBox);
            this.groupBox5.Controls.Add(this.captureSqlStatementsCheckBox);
            this.groupBox5.Controls.Add(this.captureStoredProceduresCheckBox);
            this.groupBox5.Controls.Add(this.thresholdWarningLabel);
            this.groupBox5.Controls.Add(this.traceThresholdsTableLayoutPanel);
            //this.groupBox5.Location = new System.Drawing.Point(7, 78);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.groupBox5.Location = new System.Drawing.Point(6, 60); //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(268, 266);//SQLdm 9.0 (Ankit Srivastava) -- size changed to fit new UI changes
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            // 
            ////SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard --new control properties -start
            // belowAnd2005CheckBox
            // 
            this.belowAnd2005CheckBox.AutoSize = true;
            this.belowAnd2005CheckBox.Location = new System.Drawing.Point(32, 17);
            this.belowAnd2005CheckBox.Name = "belowAnd2005CheckBox";
            this.belowAnd2005CheckBox.Size = new System.Drawing.Size(156, 17);
            this.belowAnd2005CheckBox.TabIndex = 11;
            this.belowAnd2005CheckBox.Text = "SQL Server 2000 and 2005";
            this.belowAnd2005CheckBox.UseVisualStyleBackColor = true;
            // 
            // aboveAnd2008CheckBox
            // 
            this.aboveAnd2008CheckBox.AutoSize = true;
            this.aboveAnd2008CheckBox.Checked = true;
            this.aboveAnd2008CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.aboveAnd2008CheckBox.Location = new System.Drawing.Point(32, 34);
            this.aboveAnd2008CheckBox.Name = "aboveAnd2008CheckBox";
            this.aboveAnd2008CheckBox.Size = new System.Drawing.Size(117, 17);
            this.aboveAnd2008CheckBox.TabIndex = 12;
            this.aboveAnd2008CheckBox.Text = "SQL Server 2008 +";
            this.aboveAnd2008CheckBox.UseVisualStyleBackColor = true;
            //SQLdm 10.4 (Nikhil Bansal) - TOP X Plan Filter - Added the check changed callback for servers 2008 and above check box
            this.aboveAnd2008CheckBox.CheckedChanged += new System.EventHandler(this.aboveAnd2008CheckBox_CheckedChanged);

            // 
            //////SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard --new control properties -start
            // poorlyQueriesLabel
            // 
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.poorlyQueriesLabel.Location = new System.Drawing.Point(19, 54);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes

            this.poorlyQueriesLabel.Name = "poorlyQueriesLabel";
            this.poorlyQueriesLabel.Size = new System.Drawing.Size(229, 16);
            this.poorlyQueriesLabel.TabIndex = 12;
            this.poorlyQueriesLabel.Text = "Types of poorly-performing queries to capture:";
            // 
            // captureSqlBatchesCheckBox
            // 
            this.captureSqlBatchesCheckBox.AutoSize = true;
            this.captureSqlBatchesCheckBox.Checked = true;
            this.captureSqlBatchesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.captureSqlBatchesCheckBox.Location = new System.Drawing.Point(32, 71);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes

            this.captureSqlBatchesCheckBox.Name = "captureSqlBatchesCheckBox";
            this.captureSqlBatchesCheckBox.Size = new System.Drawing.Size(128, 17);
            this.captureSqlBatchesCheckBox.TabIndex = 2;
            this.captureSqlBatchesCheckBox.Text = "Capture SQL batches";
            this.captureSqlBatchesCheckBox.UseVisualStyleBackColor = true;
            // 
            // captureSqlStatementsCheckBox
            // 
            this.captureSqlStatementsCheckBox.AutoSize = true;
            this.captureSqlStatementsCheckBox.Checked = true;
            this.captureSqlStatementsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default            
            this.captureSqlStatementsCheckBox.Location = new System.Drawing.Point(32, 89);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.captureSqlStatementsCheckBox.Name = "captureSqlStatementsCheckBox";
            this.captureSqlStatementsCheckBox.Size = new System.Drawing.Size(141, 17);
            this.captureSqlStatementsCheckBox.TabIndex = 3;
            this.captureSqlStatementsCheckBox.Text = "Capture SQL statements";
            this.captureSqlStatementsCheckBox.UseVisualStyleBackColor = true;
            // 
            // captureStoredProceduresCheckBox
            // 
            this.captureStoredProceduresCheckBox.AutoSize = true;
            this.captureStoredProceduresCheckBox.Checked = true;
            this.captureStoredProceduresCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.captureStoredProceduresCheckBox.Location = new System.Drawing.Point(32, 107);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.captureStoredProceduresCheckBox.Name = "captureStoredProceduresCheckBox";
            this.captureStoredProceduresCheckBox.Size = new System.Drawing.Size(209, 17);
            this.captureStoredProceduresCheckBox.TabIndex = 4;
            this.captureStoredProceduresCheckBox.Text = "Capture stored procedures and triggers";
            this.captureStoredProceduresCheckBox.UseVisualStyleBackColor = true;
            // 
            // thresholdWarningLabel
            // 
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default            
            this.thresholdWarningLabel.Location = new System.Drawing.Point(16, 127);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes
            this.thresholdWarningLabel.Name = "thresholdWarningLabel";
            this.thresholdWarningLabel.Size = new System.Drawing.Size(387, 19);
            this.thresholdWarningLabel.TabIndex = 6;
            this.thresholdWarningLabel.Text = "Configure the following thresholds to define poorly-performing queries:";
            // 
            // topPlanComboBox
            //
            //SQLdm 10.4 (Ruchika Salwan) -- UI changes for Add Plan
            this.topPlanComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.topPlanComboBox.Location = new System.Drawing.Point(138, 109);
            this.topPlanComboBox.Name = "topPlanComboBox";
            this.topPlanComboBox.Size = new System.Drawing.Size(160, 17);
            this.topPlanComboBox.TabIndex = 12;
            this.topPlanComboBox.Items.Add(0, "Duration (milliseconds)");
            this.topPlanComboBox.Items.Add(1, "Logical disk reads");
            this.topPlanComboBox.Items.Add(2, "CPU usage (milliseconds)");
            this.topPlanComboBox.Items.Add(3, "Physical disk writes");
            this.topPlanComboBox.Value = 0;
            // 
            // topPlanTableLayoutPanel
            // 
            this.topPlanTableLayoutPanel.AutoSize = true;
            this.topPlanTableLayoutPanel.ColumnCount = 3;
            this.topPlanTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topPlanTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topPlanTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topPlanTableLayoutPanel.Controls.Add(this.topPlanLabel, 0, 0);
            this.topPlanTableLayoutPanel.Controls.Add(this.topPlanSpinner, 1, 0);
            this.topPlanTableLayoutPanel.Controls.Add(this.topPlanSuffixLabel, 2, 0);
            this.topPlanTableLayoutPanel.Location = new System.Drawing.Point(0, 29);
            this.topPlanTableLayoutPanel.Name = "topPlanTableLayoutPanel";
            this.topPlanTableLayoutPanel.RowCount = 1;
            this.topPlanTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.topPlanTableLayoutPanel.Size = new System.Drawing.Size(109, 10);
            this.topPlanTableLayoutPanel.TabIndex = 11;
            // 
            // traceThresholdsTableLayoutPanel
            // 
            this.traceThresholdsTableLayoutPanel.AutoSize = true;
            this.traceThresholdsTableLayoutPanel.ColumnCount = 3;
            this.traceThresholdsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.traceThresholdsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.durationThresholdSpinner, 1, 0);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.physicalWritesThresholdLabel, 0, 3);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.physicalWritesThresholdSpinner, 1, 3);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.logicalReadsThresholdLabel, 0, 2);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.cpuThresholdSpinner, 1, 1);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.cpuThresholdLabel, 0, 1);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.logicalReadsThresholdSpinner, 1, 2);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.durationThresholdLabel, 0, 0);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.topPlanComboBox, 1, 4);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.topPlanTableLayoutPanel, 0, 4);
            this.traceThresholdsTableLayoutPanel.Location = new System.Drawing.Point(28, 147);
            this.traceThresholdsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.traceThresholdsTableLayoutPanel.Name = "traceThresholdsTableLayoutPanel";
            this.traceThresholdsTableLayoutPanel.RowCount = 5;
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.Size = new System.Drawing.Size(240, 105);
            this.traceThresholdsTableLayoutPanel.TabIndex = 5;
            // 
            // topPlanSpinner
            // 
            this.topPlanSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.topPlanSpinner.Enabled = true;
            this.topPlanSpinner.Increment = new decimal(new int[] {
                                                                                      1,
                                                                                      0,
                                                                                      0,
                                                                                      0});
            this.topPlanSpinner.Location = new System.Drawing.Point(0, 120);
            this.topPlanSpinner.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.topPlanSpinner.Maximum = new decimal(new int[] {
                                                                                    1000,
                                                                                    0,
                                                                                    0,
                                                                                    0});
            this.topPlanSpinner.Name = "topPlanSpinner";
            this.topPlanSpinner.Size = new System.Drawing.Size(77, 20);
            this.topPlanSpinner.TabIndex = 11;
            this.topPlanSpinner.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.topPlanSpinner.TextChanged += new System.EventHandler(this.TopPlanSpinner_TextChanged);
            //SQLdm 10.4 (Ruchika Salwan) -- UI changes for Add Plan
            // 
            // durationThresholdSpinner
            // 
            this.durationThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.durationThresholdSpinner.Location = new System.Drawing.Point(172, 3);
            this.durationThresholdSpinner.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.durationThresholdSpinner.Name = "durationThresholdSpinner";
            this.durationThresholdSpinner.Size = new System.Drawing.Size(64, 20);
            this.durationThresholdSpinner.TabIndex = 0;
            this.durationThresholdSpinner.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.durationThresholdSpinner.TextChanged += new System.EventHandler(this.DurationThresholdSpinner_TextChanged);
            // 
            // physicalWritesThresholdLabel
            // 
            this.physicalWritesThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.physicalWritesThresholdLabel.AutoSize = true;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.physicalWritesThresholdLabel.Location = new System.Drawing.Point(0, 85);
            this.physicalWritesThresholdLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.physicalWritesThresholdLabel.Name = "physicalWritesThresholdLabel";
            this.physicalWritesThresholdLabel.Size = new System.Drawing.Size(101, 13);
            this.physicalWritesThresholdLabel.TabIndex = 10;
            this.physicalWritesThresholdLabel.Text = "Physical disk writes:";
            this.physicalWritesThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // physicalWritesThresholdSpinner
            // 
            this.physicalWritesThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.physicalWritesThresholdSpinner.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.physicalWritesThresholdSpinner.Location = new System.Drawing.Point(172, 81);
            this.physicalWritesThresholdSpinner.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.physicalWritesThresholdSpinner.Name = "physicalWritesThresholdSpinner";
            this.physicalWritesThresholdSpinner.Size = new System.Drawing.Size(64, 20);
            this.physicalWritesThresholdSpinner.TabIndex = 3;
            this.physicalWritesThresholdSpinner.TextChanged += new System.EventHandler(this.PhysicalWritesThresholdSpinner_TextChanged);
            // 
            // logicalReadsThresholdLabel
            // 
            this.logicalReadsThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.logicalReadsThresholdLabel.AutoSize = true;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.logicalReadsThresholdLabel.Location = new System.Drawing.Point(0, 58);
            this.logicalReadsThresholdLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.logicalReadsThresholdLabel.Name = "logicalReadsThresholdLabel";
            this.logicalReadsThresholdLabel.Size = new System.Drawing.Size(95, 13);
            this.logicalReadsThresholdLabel.TabIndex = 9;
            this.logicalReadsThresholdLabel.Text = "Logical disk reads:";
            this.logicalReadsThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cpuThresholdSpinner
            // 
            this.cpuThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.cpuThresholdSpinner.Location = new System.Drawing.Point(172, 29);
            this.cpuThresholdSpinner.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.cpuThresholdSpinner.Name = "cpuThresholdSpinner";
            this.cpuThresholdSpinner.Size = new System.Drawing.Size(64, 20);
            this.cpuThresholdSpinner.TabIndex = 1;
            this.cpuThresholdSpinner.TextChanged += new System.EventHandler(this.CPUThresholdSpinner_TextChanged);
            // 
            // cpuThresholdLabel
            // 
            this.cpuThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cpuThresholdLabel.AutoSize = true;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.cpuThresholdLabel.Location = new System.Drawing.Point(0, 32);
            this.cpuThresholdLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.cpuThresholdLabel.Name = "cpuThresholdLabel";
            this.cpuThresholdLabel.Size = new System.Drawing.Size(129, 13);
            this.cpuThresholdLabel.TabIndex = 8;
            this.cpuThresholdLabel.Text = "CPU usage (milliseconds):";
            this.cpuThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // topPlanLabel
            //
            //SQLdm 10.4 (Ruchika Salwan) -- UI changes for Add Plan
            this.topPlanLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.topPlanLabel.AutoSize = true;
            this.topPlanLabel.Location = new System.Drawing.Point(0, 112);
            this.topPlanLabel.Name = "topPlanLabel";
            this.topPlanLabel.Size = new System.Drawing.Size(25, 13);
            this.topPlanLabel.TabIndex = 11;
            this.topPlanLabel.Text = "Select Top ";
            this.topPlanLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topPlanSuffixLabel
            // 
            this.topPlanSuffixLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.topPlanSuffixLabel.AutoSize = true;
            this.topPlanSuffixLabel.Location = new System.Drawing.Point(3, 162);
            this.topPlanSuffixLabel.Name = "topPlanSuffixLabel";
            this.topPlanSuffixLabel.Size = new System.Drawing.Size(47, 13);
            this.topPlanSuffixLabel.TabIndex = 11;
            this.topPlanSuffixLabel.Text = "Plans by ";
            this.topPlanSuffixLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //SQLdm 10.4 (Ruchika Salwan) -- UI changes for Add Plan
            // 
            // logicalReadsThresholdSpinner
            // 
            this.logicalReadsThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.logicalReadsThresholdSpinner.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.logicalReadsThresholdSpinner.Location = new System.Drawing.Point(172, 55);
            this.logicalReadsThresholdSpinner.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.logicalReadsThresholdSpinner.Name = "logicalReadsThresholdSpinner";
            this.logicalReadsThresholdSpinner.Size = new System.Drawing.Size(64, 20);
            this.logicalReadsThresholdSpinner.TabIndex = 2;
            this.logicalReadsThresholdSpinner.TextChanged += new System.EventHandler(this.LogicalReadsThresholdSpinner_TextChanged);
            // 
            // durationThresholdLabel
            // 
            this.durationThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.durationThresholdLabel.AutoSize = true;
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.durationThresholdLabel.Location = new System.Drawing.Point(0, 6);
            this.durationThresholdLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.durationThresholdLabel.Name = "durationThresholdLabel";
            this.durationThresholdLabel.Size = new System.Drawing.Size(115, 13);
            this.durationThresholdLabel.TabIndex = 7;
            this.durationThresholdLabel.Text = "Duration (milliseconds):";
            this.durationThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // queryMonitorAdvancedOptionsButton
            // 
            //SQLdm 9.0 (Ankit Srivastava) -- enabled by default
            this.queryMonitorAdvancedOptionsButton.Location = new System.Drawing.Point(434, 306);//SQLdm 9.0 (Ankit Srivastava) -- position changed to fit new UI changes

            this.queryMonitorAdvancedOptionsButton.Name = "queryMonitorAdvancedOptionsButton";
            this.queryMonitorAdvancedOptionsButton.Size = new System.Drawing.Size(83, 23);
            this.queryMonitorAdvancedOptionsButton.TabIndex = 6;
            this.queryMonitorAdvancedOptionsButton.Text = "Advanced...";
            this.queryMonitorAdvancedOptionsButton.UseVisualStyleBackColor = true;
            this.queryMonitorAdvancedOptionsButton.Click += new System.EventHandler(this.queryMonitorAdvancedOptionsButton_Click);
            // 
            // featuresDescriptionLabel
            // 
            this.featuresDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featuresDescriptionLabel.Location = new System.Drawing.Point(53, 20);
            this.featuresDescriptionLabel.Name = "featuresDescriptionLabel";
            this.featuresDescriptionLabel.Size = new System.Drawing.Size(458, 66);
            this.featuresDescriptionLabel.TabIndex = 1;
            this.featuresDescriptionLabel.Text = resources.GetString("featuresDescriptionLabel.Text");
            // 
            // featuresIcon
            // 
            this.featuresIcon.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardFeaturesImage;
            this.featuresIcon.Location = new System.Drawing.Point(15, 21);
            this.featuresIcon.Name = "featuresIcon";
            this.featuresIcon.Size = new System.Drawing.Size(32, 32);
            this.featuresIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.featuresIcon.TabIndex = 0;
            this.featuresIcon.TabStop = false;
            // 
            // dataCollectionGroupBox
            // 
            this.dataCollectionGroupBox.Controls.Add(this.tableLayoutPanelDataCollection);
            this.dataCollectionGroupBox.Location = new System.Drawing.Point(17, 3);
            this.dataCollectionGroupBox.Name = "dataCollectionGroupBox";
            this.dataCollectionGroupBox.Size = new System.Drawing.Size(525, 58);
            this.dataCollectionGroupBox.TabIndex = 0;
            this.dataCollectionGroupBox.TabStop = false;
            this.dataCollectionGroupBox.Text = "Data Collection";
            // 
            // tableLayoutPanelDataCollection
            // 
            this.tableLayoutPanelDataCollection.ColumnCount = 4;
            this.tableLayoutPanelDataCollection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataCollection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataCollection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataCollection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelDataCollection.Controls.Add(this.dataCollectionDescriptionLabel, 1, 0);
            this.tableLayoutPanelDataCollection.Controls.Add(this.dataCollectionDescriptionLabel2, 3, 0);
            this.tableLayoutPanelDataCollection.Controls.Add(this.dataCollectionIcon, 0, 0);
            this.tableLayoutPanelDataCollection.Controls.Add(this.collectionIntervalSpinner, 2, 0);
            this.tableLayoutPanelDataCollection.Location = new System.Drawing.Point(17, 14);
            this.tableLayoutPanelDataCollection.Name = "tableLayoutPanelDataCollection";
            this.tableLayoutPanelDataCollection.RowCount = 1;
            this.tableLayoutPanelDataCollection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDataCollection.Size = new System.Drawing.Size(487, 41);
            this.tableLayoutPanelDataCollection.TabIndex = 4;
            // 
            // dataCollectionDescriptionLabel
            // 
            this.dataCollectionDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dataCollectionDescriptionLabel.AutoSize = true;
            this.dataCollectionDescriptionLabel.Location = new System.Drawing.Point(41, 14);
            this.dataCollectionDescriptionLabel.Name = "dataCollectionDescriptionLabel";
            this.dataCollectionDescriptionLabel.Size = new System.Drawing.Size(217, 13);
            this.dataCollectionDescriptionLabel.TabIndex = 1;
            this.dataCollectionDescriptionLabel.Text = "Collect diagnostic data and raise alerts every";
            // 
            // dataCollectionDescriptionLabel2
            // 
            this.dataCollectionDescriptionLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dataCollectionDescriptionLabel2.AutoSize = true;
            this.dataCollectionDescriptionLabel2.Location = new System.Drawing.Point(308, 14);
            this.dataCollectionDescriptionLabel2.Name = "dataCollectionDescriptionLabel2";
            this.dataCollectionDescriptionLabel2.Size = new System.Drawing.Size(176, 13);
            this.dataCollectionDescriptionLabel2.TabIndex = 3;
            this.dataCollectionDescriptionLabel2.Text = "minutes.";
            // 
            // dataCollectionIcon
            // 
            this.dataCollectionIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dataCollectionIcon.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DataCollectionInterval;
            this.dataCollectionIcon.Location = new System.Drawing.Point(3, 4);
            this.dataCollectionIcon.Name = "dataCollectionIcon";
            this.dataCollectionIcon.Size = new System.Drawing.Size(32, 32);
            this.dataCollectionIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dataCollectionIcon.TabIndex = 2;
            this.dataCollectionIcon.TabStop = false;
            // 
            // collectionIntervalSpinner
            // 
            this.collectionIntervalSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.collectionIntervalSpinner.Location = new System.Drawing.Point(264, 10);
            this.collectionIntervalSpinner.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.collectionIntervalSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.collectionIntervalSpinner.Name = "collectionIntervalSpinner";
            this.collectionIntervalSpinner.Size = new System.Drawing.Size(38, 20);
            this.collectionIntervalSpinner.TabIndex = 0;
            this.collectionIntervalSpinner.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.collectionIntervalSpinner.TextChanged += new System.EventHandler(this.CollectionIntervalSpinner_TextChanged);
            // 
            // wmiConfigPage
            // 
            this.wmiConfigPage.Controls.Add(this.groupBox4);
            this.wmiConfigPage.Controls.Add(this.informationBox2);
            this.wmiConfigPage.Description = "Configure the method that SQL Diagnostic Manager uses to gather OS Metrics.";
            this.wmiConfigPage.Location = new System.Drawing.Point(11, 71);
            this.wmiConfigPage.Name = "wmiConfigPage";
            this.wmiConfigPage.NextPage = this.selectTagsPage;
            this.wmiConfigPage.PreviousPage = this.configureCollectionPage;
            this.wmiConfigPage.Size = new System.Drawing.Size(560, 423);
            this.wmiConfigPage.TabIndex = 1010;
            this.wmiConfigPage.Text = "Configure OS Metric Collection";
            this.wmiConfigPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.wmiConfigPage_BeforeMoveNext);
            this.wmiConfigPage.BeforeDisplay += new System.EventHandler(this.wmiConfigPage_BeforeDisplay);
            this.wmiConfigPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.wmiConfigPage_HelpRequested);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.label28);
            this.groupBox4.Controls.Add(this.directWmiPassword);
            this.groupBox4.Controls.Add(this.directWmiUserName);
            this.groupBox4.Controls.Add(this.optionWmiDirect);
            this.groupBox4.Controls.Add(this.optionWmiOleAutomation);
            this.groupBox4.Controls.Add(this.optionWmiNone);
            this.groupBox4.Controls.Add(this.optionWmiCSCreds);
            this.groupBox4.Location = new System.Drawing.Point(34, 86);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(491, 194);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "WMI Configuration";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(60, 150);
            this.label24.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 13);
            this.label24.TabIndex = 37;
            this.label24.Text = "Password:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(60, 124);
            this.label28.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(65, 13);
            this.label28.TabIndex = 36;
            this.label28.Text = "Login name:";
            // 
            // directWmiPassword
            // 
            this.directWmiPassword.Enabled = false;
            this.directWmiPassword.Location = new System.Drawing.Point(149, 147);
            this.directWmiPassword.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.directWmiPassword.Name = "directWmiPassword";
            this.directWmiPassword.Size = new System.Drawing.Size(290, 20);
            this.directWmiPassword.TabIndex = 35;
            this.directWmiPassword.UseSystemPasswordChar = true;
            this.directWmiPassword.TextChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // directWmiUserName
            // 
            this.directWmiUserName.Enabled = false;
            this.directWmiUserName.Location = new System.Drawing.Point(149, 121);
            this.directWmiUserName.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.directWmiUserName.Name = "directWmiUserName";
            this.directWmiUserName.Size = new System.Drawing.Size(290, 20);
            this.directWmiUserName.TabIndex = 34;
            this.directWmiUserName.TextChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiDirect
            // 
            this.optionWmiDirect.AutoSize = true;
            this.optionWmiDirect.Checked = true;
            this.optionWmiDirect.Location = new System.Drawing.Point(43, 69);
            this.optionWmiDirect.Name = "optionWmiDirect";
            this.optionWmiDirect.Size = new System.Drawing.Size(250, 17);
            this.optionWmiDirect.TabIndex = 33;
            this.optionWmiDirect.TabStop = true;
            this.optionWmiDirect.Text = "Collect Operating System data using direct WMI";
            this.optionWmiDirect.UseVisualStyleBackColor = true;
            this.optionWmiDirect.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiOleAutomation
            // 
            this.optionWmiOleAutomation.AutoSize = true;
            this.optionWmiOleAutomation.Location = new System.Drawing.Point(43, 46);
            this.optionWmiOleAutomation.Name = "optionWmiOleAutomation";
            this.optionWmiOleAutomation.Size = new System.Drawing.Size(275, 17);
            this.optionWmiOleAutomation.TabIndex = 32;
            this.optionWmiOleAutomation.TabStop = true;
            this.optionWmiOleAutomation.Text = "Collect Operating System data using OLE Automation";
            this.optionWmiOleAutomation.UseVisualStyleBackColor = true;
            this.optionWmiOleAutomation.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiNone
            // 
            this.optionWmiNone.AutoSize = true;
            this.optionWmiNone.Location = new System.Drawing.Point(43, 23);
            this.optionWmiNone.Name = "optionWmiNone";
            this.optionWmiNone.Size = new System.Drawing.Size(201, 17);
            this.optionWmiNone.TabIndex = 31;
            this.optionWmiNone.Text = "Do not collect Operating System data";
            this.optionWmiNone.UseVisualStyleBackColor = true;
            this.optionWmiNone.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiCSCreds
            // 
            this.optionWmiCSCreds.AutoSize = true;
            this.optionWmiCSCreds.Checked = true;
            this.optionWmiCSCreds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionWmiCSCreds.Location = new System.Drawing.Point(63, 96);
            this.optionWmiCSCreds.Name = "optionWmiCSCreds";
            this.optionWmiCSCreds.Size = new System.Drawing.Size(280, 17);
            this.optionWmiCSCreds.TabIndex = 30;
            this.optionWmiCSCreds.Text = "Connect using the SQLdm Collection Service account";
            this.optionWmiCSCreds.UseVisualStyleBackColor = true;
            this.optionWmiCSCreds.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // informationBox2
            // 
            this.informationBox2.Location = new System.Drawing.Point(29, 3);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(500, 77);
            this.informationBox2.TabIndex = 12;
            this.informationBox2.Text = resources.GetString("informationBox2.Text");
            // 
            // selectTagsPage
            // 
            this.selectTagsPage.Controls.Add(this.groupBox3);
            this.selectTagsPage.Controls.Add(this.groupBox2);
            this.selectTagsPage.Description = "Select the Alert Template that you would like to use for the added SQL Server ins" +
    "tances. Also select the tags you would like to associate with the monitored SQL " +
    "Servers instances. ";
            this.selectTagsPage.Location = new System.Drawing.Point(11, 71);
            this.selectTagsPage.Name = "selectTagsPage";
            this.selectTagsPage.PreviousPage = this.wmiConfigPage;
            this.selectTagsPage.Size = new System.Drawing.Size(560, 423);
            this.selectTagsPage.TabIndex = 1008;
            this.selectTagsPage.Text = "Select Alert Template and Tags";
            this.selectTagsPage.BeforeDisplay += new System.EventHandler(this.selectTagsPage_BeforeDisplay);
            this.selectTagsPage.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.selectTagsPage_HelpRequested);
            //
            // cboAlertTemplates
            // 
            this.cboAlertTemplates.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlertTemplates.Location = new System.Drawing.Point(26, 99);
            this.cboAlertTemplates.Name = "cboAlertTemplates";
            this.cboAlertTemplates.Size = new System.Drawing.Size(285, 21);
            this.cboAlertTemplates.TabIndex = 13;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboAlertTemplates);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(549, 127);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Alert Templates";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(526, 81);
            this.label1.TabIndex = 14;
            this.label1.Text = resources.GetString("label1.Text");
            //
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.availableTagsStatusLabel);
            this.groupBox2.Controls.Add(this.addTagButton);
            this.groupBox2.Controls.Add(this.availableTagsListBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(549, 269);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tags";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(52, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(485, 87);
            this.label4.TabIndex = 9;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Tag32x32;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Location = new System.Drawing.Point(14, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(32, 32);
            this.panel1.TabIndex = 0;
            // 
            // availableTagsStatusLabel
            // 
            this.availableTagsStatusLabel.BackColor = System.Drawing.Color.White;
            this.availableTagsStatusLabel.Location = new System.Drawing.Point(44, 154);
            this.availableTagsStatusLabel.Name = "availableTagsStatusLabel";
            this.availableTagsStatusLabel.Size = new System.Drawing.Size(472, 13);
            this.availableTagsStatusLabel.TabIndex = 12;
            this.availableTagsStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.availableTagsStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // addTagButton
            // 
            this.addTagButton.Location = new System.Drawing.Point(448, 231);
            this.addTagButton.Name = "addTagButton";
            this.addTagButton.Size = new System.Drawing.Size(75, 23);
            this.addTagButton.TabIndex = 11;
            this.addTagButton.Text = "Add Tag...";
            this.addTagButton.UseVisualStyleBackColor = true;
            this.addTagButton.Click += new System.EventHandler(this.addTagButton_Click);
            // 
            // availableTagsListBox
            // 
            this.availableTagsListBox.CheckOnClick = true;
            this.availableTagsListBox.FormattingEnabled = true;
            this.availableTagsListBox.Location = new System.Drawing.Point(28, 105);
            this.availableTagsListBox.Name = "availableTagsListBox";
            this.availableTagsListBox.Size = new System.Drawing.Size(495, 94);
            this.availableTagsListBox.Sorted = true;
            this.availableTagsListBox.TabIndex = 10;
            this.availableTagsListBox.Hide();
            // 
            // baselineConfiguration1
            // 
            this.baselineConfiguration1.BorderWidth = 0;
            this.baselineConfiguration1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baselineConfiguration1.HeaderVisible = false;
            this.baselineConfiguration1.IsMultiEdit = false;
            this.baselineConfiguration1.Location = new System.Drawing.Point(0, 0);
            this.baselineConfiguration1.Name = "baselineConfiguration1";
            this.baselineConfiguration1.Size = new System.Drawing.Size(562, 422);
            this.baselineConfiguration1.TabIndex = 0;
            // 
            // baselineConfiguration
            // 
            this.baselineConfiguration.Controls.Add(this.baselineConfiguration1);
            this.baselineConfiguration.Description = "Specify the configuration options for baseline collection of the monitored SQL Se" +
    "rver instances.";
            this.baselineConfiguration.Location = new System.Drawing.Point(11, 71);
            this.baselineConfiguration.Name = "baselineConfiguration";
            this.baselineConfiguration.NextPage = this.selectTagsPage;
            this.baselineConfiguration.PreviousPage = this.wmiConfigPage;
            this.baselineConfiguration.Size = new System.Drawing.Size(592, 422);
            this.baselineConfiguration.TabIndex = 1009;
            this.baselineConfiguration.Text = "Baseline Configuration";
            this.baselineConfiguration.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.baselineConfiguration_BeforeMoveNext);
            this.baselineConfiguration.BeforeDisplay += new System.EventHandler(this.baselineConfiguration_BeforeDisplay);
            this.baselineConfiguration.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.baselineConfiguration_HelpRequested);
            //
            // loadAvailableServersWorker
            //
            this.loadAvailableServersWorker.WorkerSupportsCancellation = true;
            this.loadAvailableServersWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadAvailableServersWorker_DoWork);
            this.loadAvailableServersWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadAvailableServersWorker_RunWorkerCompleted);
            // 
            // AddServersWizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            if (AutoScaleSizeHelper.scalingFactor > 1)
                this.ClientSize = new System.Drawing.Size(700, 552);
            else
                this.ClientSize = new System.Drawing.Size(614, 552);
            this.Controls.Add(this.wizardFramework);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddServersWizard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Servers Wizard";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AddServersWizard_HelpButtonClicked);
            this.Load += new System.EventHandler(this.AddServersWizard_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddServersWizard_HelpRequested);
            this.wizardFramework.ResumeLayout(false);
            this.welcomePage.ResumeLayout(false);
            this.welcomePage.PerformLayout();
            this.configureAuthenticationPage.ResumeLayout(false);
            this.configureAuthenticationPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.selectInstancesPage.ResumeLayout(false);
            this.selectInstancesPage.PerformLayout();
            //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud providers for all the added instances
            this.chooseCloudProviderPage.ResumeLayout(false);
            this.chooseCloudProviderPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCloudProviders)).EndInit();
            //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing the cloud providers for all the added instances
            this.configureCollectionPage.ResumeLayout(false);
            this.featuresGroupBox.ResumeLayout(false);
            this.featuresGroupBox.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBlockedProcessThreshold)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.topPlanTableLayoutPanel.ResumeLayout(false);
            this.topPlanTableLayoutPanel.PerformLayout();
            this.traceThresholdsTableLayoutPanel.ResumeLayout(false);
            this.traceThresholdsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durationThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalWritesThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logicalReadsThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.featuresIcon)).EndInit();
            this.dataCollectionGroupBox.ResumeLayout(false);
            this.tableLayoutPanelDataCollection.ResumeLayout(false);
            this.tableLayoutPanelDataCollection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataCollectionIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectionIntervalSpinner)).EndInit();
            this.wmiConfigPage.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.baselineConfiguration.ResumeLayout(false);
            this.selectTagsPage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlertTemplates)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void GrdCloudProviders_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key.ToString() == CLOUDPROVIDER)
            {
                grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSAccessKey].CellActivation = Activation.AllowEdit;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[AWSRDSSecretKey].CellActivation = Activation.AllowEdit;
                grdCloudProviders.DisplayLayout.Bands[0].Columns[CLOUDRDSREGION].CellActivation = Activation.AllowEdit;
                for (int i = 0; i < grdCloudProviders.Rows.Count; i++)
                {
                    // Reset all cells and set them to non editable for non-RDS instances
                    if (!grdCloudProviders.Rows[i].Cells[CLOUDPROVIDER].Text.Trim().ToLower().Contains("rds"))
                    {
                        grdCloudProviders.Rows[i].Cells[AWSRDSAccessKey].Value = string.Empty;
                        grdCloudProviders.Rows[i].Cells[AWSRDSAccessKey].Activation = Activation.NoEdit;
                        grdCloudProviders.Rows[i].Cells[AWSRDSSecretKey].Value = string.Empty;
                        grdCloudProviders.Rows[i].Cells[AWSRDSSecretKey].Activation = Activation.NoEdit;
                        grdCloudProviders.Rows[i].Cells[CLOUDRDSREGION].Value = string.Empty;
                        grdCloudProviders.Rows[i].Cells[CLOUDRDSREGION].Activation = Activation.NoEdit;
                    }
                    else
                    {
                        grdCloudProviders.Rows[i].Cells[AWSRDSAccessKey].Activation = Activation.AllowEdit;
                        grdCloudProviders.Rows[i].Cells[AWSRDSSecretKey].Activation = Activation.AllowEdit;
                        grdCloudProviders.Rows[i].Cells[CLOUDRDSREGION].Activation = Activation.AllowEdit;
                    }
                }
            }
        }

        #endregion
        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing cloud provider id
        private Divelements.WizardFramework.WizardPage chooseCloudProviderPage;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdCloudProviders;
        private Infragistics.Win.Appearance appearance21;
        private Infragistics.Win.Appearance appearance22;
        private Infragistics.Win.Appearance appearance23;
        private Infragistics.Win.Appearance appearance24;
        private Infragistics.Win.Appearance appearance25;
        private Infragistics.Win.Appearance appearance26;
        private Infragistics.Win.Appearance appearance27;
        private Infragistics.Win.Appearance appearance28;
        private Infragistics.Win.Appearance appearance29;
        private Infragistics.Win.Appearance appearance30;
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding another page for choosing cloud provider id
        private Divelements.WizardFramework.Wizard wizardFramework;
        private Divelements.WizardFramework.IntroductionPage welcomePage;
        private Divelements.WizardFramework.WizardPage selectInstancesPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox hideWelcomePageCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel introductoryTextLabel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel introductoryTextLabel1;
        private Divelements.WizardFramework.WizardPage configureAuthenticationPage;
        private Divelements.WizardFramework.WizardPage configureCollectionPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeInstancesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addInstancesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox adhocInstancesTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableInstancesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel addedInstancesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton useWindowsAuthenticationRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel configureAuthenticationDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton useSqlServerAuthenticationRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox passwordTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel passwordLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel loginNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox loginNameTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown collectionIntervalSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox dataCollectionGroupBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dataCollectionDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox featuresGroupBox;
        private System.Windows.Forms.PictureBox featuresIcon;
        private System.Windows.Forms.PictureBox dataCollectionIcon;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox enableQueryMonitorTraceCheckBox;
        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - declaring new checkboxes -start
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox belowAnd2005CheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox aboveAnd2008CheckBox;
        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - declaring new checkboxes - end
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel featuresDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel poorlyQueriesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureStoredProceduresCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureSqlStatementsCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureSqlBatchesCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel thresholdWarningLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel cpuThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel durationThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel physicalWritesThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logicalReadsThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel traceThresholdsTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel topPlanTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel topPlanLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel topPlanSuffixLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown topPlanSpinner;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor topPlanComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown durationThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown physicalWritesThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown cpuThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown logicalReadsThresholdSpinner;
        private System.Windows.Forms.ListBox addedInstancesListBox;
        private MRG.Controls.UI.LoadingCircle loadingServersProgressControl;
        private System.ComponentModel.BackgroundWorker loadAvailableServersWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableInstancesStatusLabel;
        private System.Windows.Forms.ListBox availableInstancesListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableLicensesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox trustServerCertificateCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox encryptDataCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton queryMonitorAdvancedOptionsButton;
        private Divelements.WizardFramework.WizardPage selectTagsPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addTagButton;
        private System.Windows.Forms.CheckedListBox availableTagsListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableTagsStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureDeadlocksCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlertTemplates;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox3;
        private Divelements.WizardFramework.WizardPage baselineConfiguration;
        private Controls.BaselineConfigurationPage baselineConfiguration1;
        private Divelements.WizardFramework.WizardPage wmiConfigPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox directWmiPassword;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox directWmiUserName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiDirect;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiOleAutomation;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiNone;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox optionWmiCSCreds;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label24;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label28;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkCaptureAutoGrow;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkCaptureBlocking;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblBlockedProcessesSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown spnBlockedProcessThreshold;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkEnableActivityMonitor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblConfigureBlockedProcessThreshold;
        private Label dataCollectionDescriptionLabel2;
        private TableLayoutPanel tableLayoutPanelDataCollection;
        private Button addAzureServerButton;
        private CheckBox useSqlAunthenticationForAzureCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox selectedAzureProfileNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel selectedAzureProfileLbl;

    }
}
