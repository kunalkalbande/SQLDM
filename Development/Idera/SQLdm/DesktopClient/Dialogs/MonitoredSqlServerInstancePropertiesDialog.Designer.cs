using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Controls.CustomControls;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class MonitoredSqlServerInstancePropertiesDialog
    {
        //TODO: Add Server UI
        // private const string AllThresholds = "All Thresholds";
        private const string Duration = "Duration (milliseconds)";
        private const string LogicalDiskReads = "Logical disk reads";
        private const string CpuUsage = "CPU usage (milliseconds)";
        private const string PhysicalWrites = "Physical disk writes";

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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : Color.White;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitoredSqlServerInstancePropertiesDialog));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("tagsPopupMenu");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint4 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint5 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint6 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint7 = new Infragistics.Win.Layout.GridBagConstraint();
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint8 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint9 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint10 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint11 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint12 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint13 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint14 = new Infragistics.Win.Layout.GridBagConstraint();
            this.activeMonitorPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.propertyPage2 = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.tableLayoutPanel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.tableLayoutPanel15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.captureDeadlockCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkCaptureAutogrow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkCaptureBlocking = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.groupBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.tableLayoutPanel12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.lblBlockedProcessSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.spnBlockedProcessThreshold = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.blockedProcessWarningPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.blockedProcessWarningImage = new System.Windows.Forms.PictureBox();
            this.lblBlockedProcessWarning = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip22 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.chkActivityMonitorEnable = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.informationBox8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            //SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- adding new controls
            this.rButtonAMUseExtendedEvents = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.rButtonAMUseTrace = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();


            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- adding new controls
            this.propertiesHeaderStrip29 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip(); this.tableLayoutPanel19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.pnlQueryMonitoringTechniques = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.rButtonUseExtendedEvents = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            this.rButtonUseQueryStore = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.rButtonUseTrace = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.chkCollectQueryPlans = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.chkCollectEstimatedQueryPlans = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();


            this.propertiesHeaderStrip28 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.office2007PropertyPage6 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.queryMonitorAdvancedOptionsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.testConnectionBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.propertiesControl = new Idera.SQLdm.DesktopClient.Controls.PropertiesControl(isDarkThemeSelected);
            this.popularPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.mainTableLayout = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.tableLayoutPanel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.testConnectionCredentialsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.useSqlServerAuthenticationRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.useWindowsAuthenticationRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.loginNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.loginNameTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.passwordLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.passwordTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.groupBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.encryptDataCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.trustServerCertificateCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.collectExtendedSessionDataCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.inputBufferLimiter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.inputDayLimiter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.inputOfEveryMonthLimiter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.inputOfEveryTheMonthLimiter = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.limitInputBuffer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip8 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label23 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.dataCollectionDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.collectionIntervalTimeSpanEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTimeSpanEditor();
            this.serverPingTimeSpanEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTimeSpanEditor();
            this.label22 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databaseStatisticsTimeSpanEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTimeSpanEditor();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.headerStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.friendlyNameTextbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.tagsDropDownButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraDropDownButton();
            //  this.tagsDropDownButtonServerType = new Infragistics.Win.Misc.UltraDropDownButton();
            this.cmbServerType = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.propertiesHeaderStripServerType = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.toolbarManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.propertiesHeaderStrip17 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.popularPropertiesContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.propertyPage1 = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.baselineConfiguration1 = new Idera.SQLdm.DesktopClient.Controls.BaselineConfigurationPage();
            //10.0 SQLdm srishti purohit -- Doctor configuration for UI
            this.analysisConfigurationPage = new Controls.Analysis.AnalysisConfigurationPage(id, cloudProviderId);
            this.queryMonitorPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.tableLayoutPanel18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.enableQueryMonitorTraceCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.queryMonitorWarningImage = new System.Windows.Forms.PictureBox();
            this.queryMonitorRunningMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip4 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.captureSqlBatchesCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.captureSqlStatementsCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.captureStoredProceduresCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.fmePoorlyPerformingThresholds = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.traceThresholdsTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.topPlanTableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.topPlanSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.topPlanLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.topPlanSuffixLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.topPlanComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.durationThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.physicalWritesThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.physicalWritesThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.durationThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logicalReadsThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.cpuThresholdLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logicalReadsThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.cpuThresholdSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.office2007PropertyPage4 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.replicationPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.replicationMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.propertiesHeaderStrip12 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.disableReplicationStuff = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.replicationPropertyPageContentPanel = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.tableStatisticsPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.tableLayoutPanel9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label25 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.lastTableGrowthCollectionTimeDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lastTableGrowthCollectionTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lastTableFragmentationCollectionTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lastTableFragmentationCollectionTimeDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip9 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.selectTableStatisticsTablesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.propertiesHeaderStrip7 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.flowLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.minimumTableSize = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.propertiesHeaderStrip10 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.flowLayoutPanel7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.collectTableStatsSundayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsMondayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsTuesdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsWednesdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsThursdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsFridayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.collectTableStatsSaturdayCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip6 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.flowLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.quietTimeStartEditor = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.endTimeLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip5 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.office2007PropertyPage2 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.customCountersPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.customCountersContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.customCounterStackLayoutPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.customCounterContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.testCustomCountersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.customCountersSelectionList = new Idera.SQLdm.DesktopClient.Controls.DualListSelectorControl();
            this.propertiesHeaderStrip11 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.customCounterMessageLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.maintenancePropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.maintenanceModeControlsContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.informationBox6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip13 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.mmNeverRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmAlwaysRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmRecurringRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmMonthlyRecurringRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmMonthlyDayRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmMonthlyTheRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.mmOnceRadio = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.mmOncePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label21 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmServerDateTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmOnceBeginTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.mmOnceBeginDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmOnceStopTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.mmOnceStopDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.propertiesHeaderStrip16 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip27 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.mmRecurringPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.mmMonthlyRecurringPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmRecurringDuration = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.mmMonthRecurringDuration = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.flowLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmRecurringBegin = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.mmMonthRecurringBegin = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.flowLayoutPanel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label29 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label30 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.mmBeginSatCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginFriCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginThurCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginWedCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginTueCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginMonCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.mmBeginSunCheckbox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip15 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.propertiesHeaderStrip14 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.maintenanceModeContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.osPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.osMetricsMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.informationBox14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip18 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.optionWmiNone = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.optionWmiOleAutomation = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.optionWmiDirect = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.wmiCredentialsSecondaryContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.wmiCredentialsContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.optionWmiCSCreds = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.label28 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.directWmiUserName = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label24 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.directWmiPassword = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.wmiTestButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.osContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.diskPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.diskDriversMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.tableLayoutPanel13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.propertiesHeaderStrip20 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.propertiesHeaderStrip19 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.autoDiscoverDisksCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.availableDisksLayoutPanel = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.addDiskButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.removeDiskButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.availableDisksStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.availableDisksMessageLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.availableDisksListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.selectedDisksListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.adhocDisksTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.diskContentPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.clusterPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.clusterSettingsMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.flowLayoutPanel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            //SQLDM-30197
            this.clusterWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.preferredNode = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.setCurrentNode = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.propertiesHeaderStrip21 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.waitPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.waitMonitoringMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.collectStatisticsSecondaryContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.chkUseXE = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            this.chkUseQs = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            // SQLdm Minimum Privileges - Varun Chopra - Warning Label
            this.lblWaitMonitoringWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblActivityMonitorWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblQueryMonitoringWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblOSMetricOLEWarningLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chkCollectActiveWaits = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.ScheduledQueryWaits = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.tableLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.waitStatisticsServerDateTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.waitStatisticsStartDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.waitStatisticsStartTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.waitStatisticsRunTime = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.PerpetualQueryWaits = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.informationBox12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip23 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.flowLayoutPanel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label20 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.waitStatisticsFilterButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.propertiesHeaderStrip25 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.waitStatisticsStatus = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.stopWaitCollector = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.refreshWaitCollectorStatus = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.informationBox13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.propertiesHeaderStrip24 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.office2007PropertyPage3 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.virtualizationPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.virtualizationMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.propertiesHeaderStrip26 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.informationBox11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.tableLayoutPanel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.btnVMConfig = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label26 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.vCenterNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label27 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmMonthlyOfEveryLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmMonthlyOfTheEveryLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmMonthlyLabel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmMonthlyLabel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.mmMonthlyAtLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.WeekcomboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.DaycomboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.vmNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.office2007PropertyPage5 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraGridBagLayoutPanel1 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.availableTablesStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.availableTablesMessageLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.availableTablesListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.databaseComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.panel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.addButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.removeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.selectedTablesListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListBox();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.availableTablesTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.waitCollectorStatusBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.stopWaitCollectorBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.serverDateTimeVersionBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            // SQLdm Minimum Privileges - Varun Chopra - Initialize Permissions worker
            this.serverPermissionsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.blockedProcessThresholdBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.textBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();

            //For Doctors UI implementation
            //10.0 SQLdl Srishti Purohit -- For Confiuration Doctor UI
            #region Analysis Configuration controls
            this.analysisConfigurationPropertyPage = new Idera.SQLdm.DesktopClient.Controls.PropertyPage();
            this.office2007PropertyPageAnalysisConfiguration = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);

            this.analysisConfigurationPropertyPage.SuspendLayout();
            #endregion

            this.activeMonitorPropertyPage.SuspendLayout();
            this.propertyPage2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel15.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.blockedProcessWarningPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockedProcessWarningImage)).BeginInit();
            this.tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBlockedProcessThreshold)).BeginInit();
            this.tableLayoutPanel11.SuspendLayout();
            this.popularPropertyPage.SuspendLayout();
            this.mainTableLayout.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputBufferLimiter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputDayLimiter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryMonthLimiter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryTheMonthLimiter)).BeginInit();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collectionIntervalTimeSpanEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPingTimeSpanEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseStatisticsTimeSpanEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).BeginInit();
            this.propertyPage1.SuspendLayout();
            this.queryMonitorPropertyPage.SuspendLayout();
            this.tableLayoutPanel18.SuspendLayout();
            this.tableLayoutPanel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.queryMonitorWarningImage)).BeginInit();
            this.tableLayoutPanel16.SuspendLayout();
            this.fmePoorlyPerformingThresholds.SuspendLayout();
            this.traceThresholdsTableLayoutPanel.SuspendLayout();
            this.topPlanTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durationThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalWritesThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logicalReadsThresholdSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuThresholdSpinner)).BeginInit();
            this.replicationPropertyPage.SuspendLayout();
            this.replicationMainContainer.SuspendLayout();
            this.tableStatisticsPropertyPage.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimumTableSize)).BeginInit();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quietTimeStartEditor)).BeginInit();
            this.customCountersPropertyPage.SuspendLayout();
            this.customCountersContentPage.ContentPanel.SuspendLayout();
            this.customCounterStackLayoutPanel.SuspendLayout();
            this.customCounterContentPanel.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.maintenancePropertyPage.SuspendLayout();
            this.maintenanceModeControlsContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.mmOncePanel.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopDate)).BeginInit();
            this.mmRecurringPanel.SuspendLayout();
            this.mmMonthlyRecurringPanel.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringDuration)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringBegin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringBegin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringDuration)).BeginInit();
            this.osPropertyPage.SuspendLayout();
            this.osMetricsMainContainer.SuspendLayout();
            this.wmiCredentialsSecondaryContainer.SuspendLayout();
            this.wmiCredentialsContainer.SuspendLayout();
            this.diskPropertyPage.SuspendLayout();
            this.diskDriversMainContainer.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableDisksLayoutPanel)).BeginInit();
            this.availableDisksLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.availableDisksStackPanel.SuspendLayout();
            this.clusterPropertyPage.SuspendLayout();
            this.clusterSettingsMainContainer.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.waitPropertyPage.SuspendLayout();
            this.waitMonitoringMainContainer.SuspendLayout();
            this.collectStatisticsSecondaryContainer.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsRunTime)).BeginInit();
            this.flowLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.virtualizationPropertyPage.SuspendLayout();
            this.virtualizationMainContainer.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).BeginInit();
            this.ultraGridBagLayoutPanel1.SuspendLayout();
            this.availableTablesStackPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanComboBox)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // activeMonitorPropertyPage
            // 
            this.activeMonitorPropertyPage.Controls.Add(this.propertyPage2);
            this.activeMonitorPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeMonitorPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.activeMonitorPropertyPage.Name = "activeMonitorPropertyPage";
            this.activeMonitorPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.activeMonitorPropertyPage.TabIndex = 0;
            this.activeMonitorPropertyPage.Text = "Activity Monitor";
            this.activeMonitorPropertyPage.AutoScroll = true;
            // 
            // propertyPage2
            // 
            this.propertyPage2.Controls.Add(this.tableLayoutPanel4);
            this.propertyPage2.Controls.Add(this.office2007PropertyPage6);
            this.propertyPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPage2.Location = new System.Drawing.Point(0, 0);
            this.propertyPage2.Name = "propertyPage2";
            this.propertyPage2.Size = new System.Drawing.Size(506, 689);
            this.propertyPage2.TabIndex = 1;
            this.propertyPage2.Text = "Query Monitor";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel15, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.propertiesHeaderStrip22, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel11, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.propertiesHeaderStrip28, 0, 2);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5, 54);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(495, 632);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel15.ColumnCount = 2;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel15.Controls.Add(this.captureDeadlockCheckBox, 1, 0);
            this.tableLayoutPanel15.Controls.Add(this.chkCaptureAutogrow, 1, 1);
            this.tableLayoutPanel15.Controls.Add(this.chkCaptureBlocking, 1, 2);
            this.tableLayoutPanel15.Controls.Add(this.groupBox3, 1, 3);
            this.tableLayoutPanel15.Controls.Add(this.blockedProcessWarningPanel, 1, 4);
            this.tableLayoutPanel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel15.Location = new System.Drawing.Point(3, 161);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 3;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(489, 468);
            this.tableLayoutPanel15.TabIndex = 29;
            this.tableLayoutPanel15.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel15_Paint);
            // 
            // captureDeadlockCheckBox
            // 
            this.captureDeadlockCheckBox.AutoSize = true;
            this.captureDeadlockCheckBox.Checked = true;
            this.captureDeadlockCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captureDeadlockCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captureDeadlockCheckBox.Location = new System.Drawing.Point(33, 3);
            this.captureDeadlockCheckBox.Name = "captureDeadlockCheckBox";
            this.captureDeadlockCheckBox.Size = new System.Drawing.Size(453, 17);
            this.captureDeadlockCheckBox.TabIndex = 1;
            this.captureDeadlockCheckBox.Text = "Capture deadlocks (SQL 2005+)";
            this.captureDeadlockCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkCaptureAutogrow
            // 
            this.chkCaptureAutogrow.AutoSize = true;
            this.chkCaptureAutogrow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCaptureAutogrow.Location = new System.Drawing.Point(33, 26);
            this.chkCaptureAutogrow.Name = "chkCaptureAutogrow";
            this.chkCaptureAutogrow.Size = new System.Drawing.Size(453, 17);
            this.chkCaptureAutogrow.TabIndex = 24;
            this.chkCaptureAutogrow.Text = "Capture Autogrow";
            this.chkCaptureAutogrow.UseVisualStyleBackColor = true;
            // 
            // chkCaptureBlocking
            // 
            this.chkCaptureBlocking.AutoSize = true;
            this.chkCaptureBlocking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkCaptureBlocking.Location = new System.Drawing.Point(33, 49);
            this.chkCaptureBlocking.Name = "chkCaptureBlocking";
            this.chkCaptureBlocking.Size = new System.Drawing.Size(453, 17);
            this.chkCaptureBlocking.TabIndex = 23;
            this.chkCaptureBlocking.Text = "Capture Blocking (SQL 2005+)";
            this.chkCaptureBlocking.UseVisualStyleBackColor = true;
            this.chkCaptureBlocking.CheckedChanged += new System.EventHandler(this.chkCaptureBlocking_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.tableLayoutPanel12);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(3, 72);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(483, 45);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.AutoSize = true;
            this.tableLayoutPanel12.ColumnCount = 3;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel12.Controls.Add(this.lblBlockedProcessSpinner, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.spnBlockedProcessThreshold, 1, 0);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 1;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(477, 26);
            this.tableLayoutPanel12.TabIndex = 31;
            // 
            // lblBlockedProcessSpinner
            // 
            this.lblBlockedProcessSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBlockedProcessSpinner.AutoEllipsis = true;
            this.lblBlockedProcessSpinner.AutoSize = true;
            this.lblBlockedProcessSpinner.Location = new System.Drawing.Point(3, 0);
            this.lblBlockedProcessSpinner.Name = "lblBlockedProcessSpinner";
            this.lblBlockedProcessSpinner.Size = new System.Drawing.Size(189, 26);
            this.lblBlockedProcessSpinner.TabIndex = 25;
            this.lblBlockedProcessSpinner.Text = "Blocked Process Threshold (seconds):";
            this.lblBlockedProcessSpinner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spnBlockedProcessThreshold
            // 
            this.spnBlockedProcessThreshold.AutoSize = true;
            this.spnBlockedProcessThreshold.Dock = System.Windows.Forms.DockStyle.Left;
            this.spnBlockedProcessThreshold.Location = new System.Drawing.Point(198, 3);
            this.spnBlockedProcessThreshold.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.Name = "spnBlockedProcessThreshold";
            this.spnBlockedProcessThreshold.Size = new System.Drawing.Size(53, 20);
            this.spnBlockedProcessThreshold.TabIndex = 22;
            this.spnBlockedProcessThreshold.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spnBlockedProcessThreshold.TextChanged += new System.EventHandler(spnBlockedProcessThreshold_TextChanged);
            // 
            // blockedProcessWarningPanel
            // 
            this.blockedProcessWarningPanel.Controls.Add(this.blockedProcessWarningImage);
            this.blockedProcessWarningPanel.Controls.Add(this.lblBlockedProcessWarning);
            this.blockedProcessWarningPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.blockedProcessWarningPanel.Location = new System.Drawing.Point(0, 0);
            this.blockedProcessWarningPanel.Name = "operationalStatusPanel";
            this.blockedProcessWarningPanel.Size = new System.Drawing.Size(453, 75);
            // 
            // blockedProcessWarningImage
            // 
            this.blockedProcessWarningImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.blockedProcessWarningImage.Location = new System.Drawing.Point(3, 3);
            this.blockedProcessWarningImage.Name = "blockedProcessWarningImage";
            this.blockedProcessWarningImage.Size = new System.Drawing.Size(16, 16);
            this.blockedProcessWarningImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.blockedProcessWarningImage.TabStop = false;
            // 
            // lblBlockedProcessWarning
            // 
            this.lblBlockedProcessWarning.Anchor = ((System.Windows.Forms.AnchorStyles)
                (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBlockedProcessWarning.AutoEllipsis = true;
            this.lblBlockedProcessWarning.AutoSize = true;
            this.lblBlockedProcessWarning.Location = new System.Drawing.Point(20, 1);
            this.lblBlockedProcessWarning.Name = "lblBlockedProcessWarning";
            this.lblBlockedProcessWarning.Size = new System.Drawing.Size(300, 21);
            this.lblBlockedProcessWarning.Text =
                "Warning: Modifying the Activity Monitor Blocked Process Threshold automatically \n" +
                "changes the Blocked Process Threshold value on the monitored SQL Server \ninstance.";
            this.lblBlockedProcessWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // propertiesHeaderStrip22
            // 
            this.propertiesHeaderStrip22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip22.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip22.Name = "propertiesHeaderStrip22";
            this.propertiesHeaderStrip22.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip22.TabIndex = 14;
            this.propertiesHeaderStrip22.Text = "Would you like to enable the Activity Monitor?";
            this.propertiesHeaderStrip22.WordWrap = false;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.Controls.Add(this.chkActivityMonitorEnable, 1, 1);
            this.tableLayoutPanel11.Controls.Add(this.rButtonAMUseExtendedEvents, 1, 2);
            this.tableLayoutPanel11.Controls.Add(this.rButtonAMUseTrace, 1, 3);
            this.tableLayoutPanel11.Controls.Add(this.lblActivityMonitorWarningLabel, 1, 4);
            this.tableLayoutPanel11.Controls.Add(this.informationBox8, 0, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(3, 34);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 7;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(489, 90);
            this.tableLayoutPanel11.TabIndex = 3;
            // 
            // lblActivityMonitorWarningLabel
            // 
            this.tableLayoutPanel11.SetColumnSpan(this.lblActivityMonitorWarningLabel, 3);
            this.lblActivityMonitorWarningLabel.Location = new System.Drawing.Point(3, 213);
            this.lblActivityMonitorWarningLabel.Name = "lblActivityMonitorWarningLabel";
            this.lblActivityMonitorWarningLabel.Size = new System.Drawing.Size(450, 48);
            this.lblActivityMonitorWarningLabel.Visible = true;
            this.lblActivityMonitorWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.lblActivityMonitorWarningLabel.TabIndex = 105;
            this.lblActivityMonitorWarningLabel.Text = "Warning: The SQL Diagnostic Manager monitoring account does not have the necessary " +
                "permissions to collect this data.";
            // 
            // chkActivityMonitorEnable
            // 
            this.chkActivityMonitorEnable.AutoSize = true;
            this.chkActivityMonitorEnable.BackColor = System.Drawing.Color.Transparent;
            this.chkActivityMonitorEnable.Location = new System.Drawing.Point(38, 70);
            this.chkActivityMonitorEnable.Name = "chkActivityMonitorEnable";
            this.chkActivityMonitorEnable.Size = new System.Drawing.Size(217, 17);
            this.chkActivityMonitorEnable.TabIndex = 32;
            this.chkActivityMonitorEnable.Text = "Enable the Activity Monitor";
            this.chkActivityMonitorEnable.UseVisualStyleBackColor = false;
            this.chkActivityMonitorEnable.CheckedChanged += new System.EventHandler(this.chkActivityMonitorEnable_CheckedChanged);
            // 
            // informationBox8
            // 
            this.informationBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel11.SetColumnSpan(this.informationBox8, 2);
            this.informationBox8.Location = new System.Drawing.Point(3, 3);
            this.informationBox8.Name = "informationBox8";
            this.informationBox8.Size = new System.Drawing.Size(483, 61);
            this.informationBox8.TabIndex = 15;
            this.informationBox8.Text = resources.GetString("informationBox8.Text");
            // 
            // rButtonAMUseExtendedEvents //SQLdm 9.1 (Ankit Srivastava) - Improve Activity Monitoring - adding new radio button (Extended events)
            // 
            this.rButtonAMUseExtendedEvents.AutoSize = true;
            this.rButtonAMUseExtendedEvents.BackColor = System.Drawing.Color.Transparent;
            var location = this.rButtonAMUseExtendedEvents.Location;
            location.X = 43;
            this.rButtonAMUseExtendedEvents.Location = location;
            this.rButtonAMUseExtendedEvents.Name = "rButtonAMUseExtendedEvents";
            this.rButtonAMUseExtendedEvents.Size = new System.Drawing.Size(217, 17);
            this.rButtonAMUseExtendedEvents.Checked = true;
            this.rButtonAMUseExtendedEvents.Text = "Use Extended Events (SQL Server 2012 +)"; // SQLdm 9.1 (Ankit Srivastava) Resolved Rally Defect DE44438
            this.rButtonAMUseExtendedEvents.UseVisualStyleBackColor = false;

            // 
            // rButtonAMUseTrace //SQLdm 9.1 (Ankit Srivastava) - Improve Activity Monitoring - adding new radio button (SQL trace)
            // 
            this.rButtonAMUseTrace.AutoSize = true;
            this.rButtonAMUseTrace.BackColor = System.Drawing.Color.Transparent;
            location = this.rButtonAMUseExtendedEvents.Location;
            location.X = 43;
            this.rButtonAMUseExtendedEvents.Location = location;
            this.rButtonAMUseTrace.Name = "rButtonAMUseTrace";
            this.rButtonAMUseTrace.Size = new System.Drawing.Size(217, 17);
            this.rButtonAMUseTrace.Text = "Use SQL Trace";
            this.rButtonAMUseTrace.UseVisualStyleBackColor = false;
            this.rButtonAMUseTrace.CheckedChanged += new System.EventHandler(this.rButtonAMStatus_Changed);

            //
            // propertiesHeaderStrip28
            // 
            this.propertiesHeaderStrip28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip28.Location = new System.Drawing.Point(3, 130);
            this.propertiesHeaderStrip28.Name = "propertiesHeaderStrip28";
            this.propertiesHeaderStrip28.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip28.TabIndex = 16;
            this.propertiesHeaderStrip28.Text = "Types of SQL Sever activities to capture";
            this.propertiesHeaderStrip28.WordWrap = false;
            // 
            // office2007PropertyPage6
            // 
            this.office2007PropertyPage6.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage6.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage6.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage6.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage6.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage6.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage6.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage6.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage6.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage6.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage6.ContentPanel.Size = new System.Drawing.Size(504, 632);
            this.office2007PropertyPage6.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage6.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardFeaturesImage;
            this.office2007PropertyPage6.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage6.Name = "office2007PropertyPage4";
            this.office2007PropertyPage6.Size = new System.Drawing.Size(506, 689);
            this.office2007PropertyPage6.TabIndex = 0;
            this.office2007PropertyPage6.Text = "Customize the Activity Monitor.";
            this.office2007PropertyPage6.BackColor = backcolor;
            // 
            // queryMonitorAdvancedOptionsButton
            // 
            this.queryMonitorAdvancedOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.queryMonitorAdvancedOptionsButton.AutoSize = true;
            //this.queryMonitorAdvancedOptionsButton.BackColor = System.Drawing.SystemColors.Control;
            this.queryMonitorAdvancedOptionsButton.Location = new System.Drawing.Point(417, 456);
            this.queryMonitorAdvancedOptionsButton.Name = "queryMonitorAdvancedOptionsButton";
            this.queryMonitorAdvancedOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.queryMonitorAdvancedOptionsButton.TabIndex = 30;
            this.queryMonitorAdvancedOptionsButton.Text = "Advanced...";
            this.queryMonitorAdvancedOptionsButton.UseVisualStyleBackColor = true;
            this.queryMonitorAdvancedOptionsButton.Click += new System.EventHandler(this.queryMonitorAdvancedOptionsButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(516, 752);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(597, 752);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // testConnectionBackgroundWorker
            // 
            this.testConnectionBackgroundWorker.WorkerSupportsCancellation = true;
            this.testConnectionBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.testConnectionBackgroundWorker_DoWork);
            this.testConnectionBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.testConnectionBackgroundWorker_RunWorkerCompleted);
            // 
            // propertiesControl
            // 
            this.propertiesControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesControl.Location = new System.Drawing.Point(12, 12);
            this.propertiesControl.Name = "propertiesControl";
            this.propertiesControl.BackColor = backColor;
            this.propertiesControl.PropertyPages.Add(this.popularPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.propertyPage1);
            this.propertiesControl.PropertyPages.Add(this.queryMonitorPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.activeMonitorPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.replicationPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.tableStatisticsPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.customCountersPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.maintenancePropertyPage);
            this.propertiesControl.PropertyPages.Add(this.osPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.diskPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.clusterPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.waitPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.virtualizationPropertyPage);
            this.propertiesControl.PropertyPages.Add(this.analysisConfigurationPropertyPage);
            this.propertiesControl.SelectedPropertyPageIndex = 0;
            this.propertiesControl.Size = new System.Drawing.Size(660, 734);
            this.propertiesControl.TabIndex = 0;
            this.propertiesControl.PropertyPageChanged += new System.EventHandler(this.propertiesControl_PropertyPageChanged);
            // 
            // popularPropertyPage
            // 
            this.popularPropertyPage.Controls.Add(this.mainTableLayout);
            this.popularPropertyPage.Controls.Add(this.popularPropertiesContentPage);
            this.popularPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.popularPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.popularPropertyPage.Name = "popularPropertyPage";
            this.popularPropertyPage.Size = new System.Drawing.Size(506, 734);
            this.popularPropertyPage.TabIndex = 0;
            this.popularPropertyPage.Text = "General";
            //this.popularPropertyPage.BackColor = System.Drawing.Color.Green;
            this.popularPropertyPage.AutoScroll = true;
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTableLayout.BackColor = System.Drawing.Color.White;
            this.mainTableLayout.ColumnCount = 2;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.tableLayoutPanel6, 1, 11);
            this.mainTableLayout.Controls.Add(this.propertiesHeaderStrip1, 0, 10);
            this.mainTableLayout.Controls.Add(this.tableLayoutPanel8, 1, 9);
            this.mainTableLayout.Controls.Add(this.propertiesHeaderStrip8, 0, 8);
            this.mainTableLayout.Controls.Add(this.friendlyNameTextbox, 0, 7);
            this.mainTableLayout.Controls.Add(this.headerStrip2, 0, 6);
            this.mainTableLayout.Controls.Add(this.tableLayoutPanel7, 1, 5);
            this.mainTableLayout.Controls.Add(this.headerStrip1, 0, 4);
            this.mainTableLayout.Controls.Add(this.tagsDropDownButton, 0, 3);
            this.mainTableLayout.Controls.Add(this.propertiesHeaderStrip17, 0, 2);
          //  this.mainTableLayout.Controls.Add(this.tagsDropDownButtonServerType, 0, 1);
            this.mainTableLayout.Controls.Add(this.cmbServerType, 0, 1);
            this.mainTableLayout.Controls.Add(this.propertiesHeaderStripServerType, 0, 0);
            this.mainTableLayout.Location = new System.Drawing.Point(5, 54);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 12;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayout.Size = new System.Drawing.Size(495, 700);
            this.mainTableLayout.TabIndex = 4;
            this.mainTableLayout.AutoScroll = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.Controls.Add(this.testConnectionCredentialsButton, 2, 6);
            this.tableLayoutPanel6.Controls.Add(this.useSqlServerAuthenticationRadioButton, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.useWindowsAuthenticationRadioButton, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.loginNameLabel, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.loginNameTextbox, 2, 2);
            this.tableLayoutPanel6.Controls.Add(this.passwordLabel, 1, 3);
            this.tableLayoutPanel6.Controls.Add(this.passwordTextbox, 2, 3);
            this.tableLayoutPanel6.Controls.Add(this.groupBox1, 1, 5);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(23, 311);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(469, 266);
            this.tableLayoutPanel6.TabIndex = 36;
            // 
            // testConnectionCredentialsButton
            // 
            this.testConnectionCredentialsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testConnectionCredentialsButton.AutoSize = true;
            //this.testConnectionCredentialsButton.BackColor = System.Drawing.SystemColors.Control;
            //this.testConnectionCredentialsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.testConnectionCredentialsButton.Location = new System.Drawing.Point(304, 186);
            this.testConnectionCredentialsButton.Name = "testConnectionCredentialsButton";
            this.testConnectionCredentialsButton.Size = new System.Drawing.Size(71, 23);
            this.testConnectionCredentialsButton.TabIndex = 29;
            this.testConnectionCredentialsButton.Text = "Test";
            this.testConnectionCredentialsButton.UseVisualStyleBackColor = true;
            this.testConnectionCredentialsButton.Click += new System.EventHandler(this.testConnectionCredentialsButton_Click);
            // 
            // useSqlServerAuthenticationRadioButton
            // 
            this.useSqlServerAuthenticationRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.useSqlServerAuthenticationRadioButton.AutoSize = true;
            this.tableLayoutPanel6.SetColumnSpan(this.useSqlServerAuthenticationRadioButton, 3);
            this.useSqlServerAuthenticationRadioButton.Location = new System.Drawing.Point(3, 26);
            this.useSqlServerAuthenticationRadioButton.Name = "useSqlServerAuthenticationRadioButton";
            this.useSqlServerAuthenticationRadioButton.Size = new System.Drawing.Size(151, 17);
            this.useSqlServerAuthenticationRadioButton.TabIndex = 15;
            this.useSqlServerAuthenticationRadioButton.Text = "SQL Server Authentication";
            this.useSqlServerAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.useSqlServerAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.useSqlServerAuthenticationRadioButton_CheckedChanged);
            // 
            // useWindowsAuthenticationRadioButton
            // 
            this.useWindowsAuthenticationRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.useWindowsAuthenticationRadioButton.AutoSize = true;
            this.useWindowsAuthenticationRadioButton.Checked = true;
            this.tableLayoutPanel6.SetColumnSpan(this.useWindowsAuthenticationRadioButton, 3);
            this.useWindowsAuthenticationRadioButton.Location = new System.Drawing.Point(3, 3);
            this.useWindowsAuthenticationRadioButton.Name = "useWindowsAuthenticationRadioButton";
            this.useWindowsAuthenticationRadioButton.Size = new System.Drawing.Size(140, 17);
            this.useWindowsAuthenticationRadioButton.TabIndex = 14;
            this.useWindowsAuthenticationRadioButton.TabStop = true;
            this.useWindowsAuthenticationRadioButton.Text = "Windows Authentication";
            this.useWindowsAuthenticationRadioButton.UseVisualStyleBackColor = true;
            this.useWindowsAuthenticationRadioButton.CheckedChanged += new System.EventHandler(this.useWindowsAuthenticationRadioButton_CheckedChanged);
            // 
            // loginNameLabel
            // 
            this.loginNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.loginNameLabel.AutoSize = true;
            this.loginNameLabel.Location = new System.Drawing.Point(39, 52);
            this.loginNameLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.loginNameLabel.Name = "loginNameLabel";
            this.loginNameLabel.Size = new System.Drawing.Size(65, 13);
            this.loginNameLabel.TabIndex = 18;
            this.loginNameLabel.Text = "Login name:";
            // 
            // loginNameTextbox
            // 
            this.loginNameTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loginNameTextbox.Enabled = false;
            this.loginNameTextbox.Location = new System.Drawing.Point(127, 49);
            this.loginNameTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.loginNameTextbox.Name = "loginNameTextbox";
            this.loginNameTextbox.Size = new System.Drawing.Size(248, 20);
            this.loginNameTextbox.TabIndex = 19;
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(48, 78);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordLabel.TabIndex = 20;
            this.passwordLabel.Text = "Password:";
            // 
            // passwordTextbox
            // 
            this.passwordTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordTextbox.Enabled = false;
            this.passwordTextbox.Location = new System.Drawing.Point(127, 75);
            this.passwordTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.passwordTextbox.Name = "passwordTextbox";
            this.passwordTextbox.Size = new System.Drawing.Size(248, 20);
            this.passwordTextbox.TabIndex = 21;
            this.passwordTextbox.UseSystemPasswordChar = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.tableLayoutPanel6.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.encryptDataCheckbox);
            this.groupBox1.Controls.Add(this.trustServerCertificateCheckbox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(22, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 79);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Encryption Options";
            // 
            // encryptDataCheckbox
            // 
            this.encryptDataCheckbox.AutoSize = true;
            this.encryptDataCheckbox.Location = new System.Drawing.Point(28, 22);
            this.encryptDataCheckbox.Name = "encryptDataCheckbox";
            this.encryptDataCheckbox.Size = new System.Drawing.Size(148, 17);
            this.encryptDataCheckbox.TabIndex = 20;
            this.encryptDataCheckbox.Text = "Encrypt Connection (SSL)";
            this.encryptDataCheckbox.UseVisualStyleBackColor = true;
            this.encryptDataCheckbox.CheckedChanged += new System.EventHandler(this.encryptDataCheckbox_CheckedChanged);
            // 
            // trustServerCertificateCheckbox
            // 
            this.trustServerCertificateCheckbox.AutoSize = true;
            this.trustServerCertificateCheckbox.Enabled = false;
            this.trustServerCertificateCheckbox.Location = new System.Drawing.Point(28, 43);
            this.trustServerCertificateCheckbox.Name = "trustServerCertificateCheckbox";
            this.trustServerCertificateCheckbox.Size = new System.Drawing.Size(273, 17);
            this.trustServerCertificateCheckbox.TabIndex = 21;
            this.trustServerCertificateCheckbox.Text = "Trust Server Certificate (bypass certificate validation)";
            this.trustServerCertificateCheckbox.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip1
            // 
            this.mainTableLayout.SetColumnSpan(this.propertiesHeaderStrip1, 2);
            this.propertiesHeaderStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(3, 280);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip1.TabIndex = 35;
            this.propertiesHeaderStrip1.Text = "What credentials should be used when collecting data from this SQL Server?";
            this.propertiesHeaderStrip1.WordWrap = false;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.collectExtendedSessionDataCheckBox, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.inputBufferLimiter, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.limitInputBuffer, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(23, 222);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(469, 52);
            this.tableLayoutPanel8.TabIndex = 34;
            // 
            // collectExtendedSessionDataCheckBox
            // 
            this.collectExtendedSessionDataCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectExtendedSessionDataCheckBox.AutoSize = true;
            this.tableLayoutPanel8.SetColumnSpan(this.collectExtendedSessionDataCheckBox, 2);
            this.collectExtendedSessionDataCheckBox.Location = new System.Drawing.Point(3, 4);
            this.collectExtendedSessionDataCheckBox.Name = "collectExtendedSessionDataCheckBox";
            this.collectExtendedSessionDataCheckBox.Size = new System.Drawing.Size(372, 17);
            this.collectExtendedSessionDataCheckBox.TabIndex = 28;
            this.collectExtendedSessionDataCheckBox.Text = "Collect extended session data, including session details, locks and blocks";
            this.collectExtendedSessionDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputBufferLimiter
            // 
            this.inputBufferLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputBufferLimiter.Location = new System.Drawing.Point(223, 29);
            this.inputBufferLimiter.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.inputBufferLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputBufferLimiter.Name = "inputBufferLimiter";
            this.inputBufferLimiter.Size = new System.Drawing.Size(63, 20);
            this.inputBufferLimiter.TabIndex = 32;
            this.inputBufferLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // limitInputBuffer
            // 
            this.limitInputBuffer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.limitInputBuffer.AutoSize = true;
            this.limitInputBuffer.Location = new System.Drawing.Point(3, 30);
            this.limitInputBuffer.Name = "limitInputBuffer";
            this.limitInputBuffer.Size = new System.Drawing.Size(214, 17);
            this.limitInputBuffer.TabIndex = 31;
            this.limitInputBuffer.Text = "Limit executions of DBCC Inputbuffer to:";
            this.limitInputBuffer.UseVisualStyleBackColor = true;
            this.limitInputBuffer.CheckedChanged += new System.EventHandler(this.limitInputBuffer_CheckedChanged);
            // 
            // propertiesHeaderStrip8
            // 
            this.mainTableLayout.SetColumnSpan(this.propertiesHeaderStrip8, 2);
            this.propertiesHeaderStrip8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip8.Location = new System.Drawing.Point(3, 191);
            this.propertiesHeaderStrip8.Name = "propertiesHeaderStrip8";
            this.propertiesHeaderStrip8.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip8.TabIndex = 33;
            this.propertiesHeaderStrip8.Text = "Would you like to collect extended session data for this SQL Server?";
            this.propertiesHeaderStrip8.WordWrap = false;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.label23, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.dataCollectionDescriptionLabel, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.collectionIntervalTimeSpanEditor, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.serverPingTimeSpanEditor, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.label22, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.databaseStatisticsTimeSpanEditor, 1, 2);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(23, 96);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(469, 89);
            this.tableLayoutPanel7.TabIndex = 32;
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 65);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(256, 13);
            this.label23.TabIndex = 42;
            this.label23.Text = "Collect and alert on database metrics (max 24 hours):";
            // 
            // dataCollectionDescriptionLabel
            // 
            this.dataCollectionDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dataCollectionDescriptionLabel.AutoSize = true;
            this.dataCollectionDescriptionLabel.Location = new System.Drawing.Point(3, 7);
            this.dataCollectionDescriptionLabel.Name = "dataCollectionDescriptionLabel";
            this.dataCollectionDescriptionLabel.Size = new System.Drawing.Size(273, 13);
            this.dataCollectionDescriptionLabel.TabIndex = 4;
            this.dataCollectionDescriptionLabel.Text = "Collect diagnostic data and raise alerts (max 30 minutes):";
            // 
            // collectionIntervalTimeSpanEditor
            // 
            this.collectionIntervalTimeSpanEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectionIntervalTimeSpanEditor.Location = new System.Drawing.Point(282, 3);
            this.collectionIntervalTimeSpanEditor.Name = "collectionIntervalTimeSpanEditor";
            this.collectionIntervalTimeSpanEditor.Size = new System.Drawing.Size(90, 21);
            this.collectionIntervalTimeSpanEditor.TabIndex = 39;
            this.collectionIntervalTimeSpanEditor.TimeSpan = System.TimeSpan.Parse("00:06:00");
            // 
            // serverPingTimeSpanEditor
            // 
            this.serverPingTimeSpanEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serverPingTimeSpanEditor.Location = new System.Drawing.Point(282, 30);
            this.serverPingTimeSpanEditor.Name = "serverPingTimeSpanEditor";
            this.serverPingTimeSpanEditor.Size = new System.Drawing.Size(90, 21);
            this.serverPingTimeSpanEditor.TabIndex = 41;
            this.serverPingTimeSpanEditor.TimeSpan = System.TimeSpan.Parse("00:01:00");
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 34);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(242, 13);
            this.label22.TabIndex = 40;
            this.label22.Text = "Alert if the server is inaccessible (max 10 minutes):";
            // 
            // databaseStatisticsTimeSpanEditor
            // 
            this.databaseStatisticsTimeSpanEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.databaseStatisticsTimeSpanEditor.Location = new System.Drawing.Point(282, 61);
            this.databaseStatisticsTimeSpanEditor.Name = "databaseStatisticsTimeSpanEditor";
            this.databaseStatisticsTimeSpanEditor.Size = new System.Drawing.Size(90, 21);
            this.databaseStatisticsTimeSpanEditor.TabIndex = 43;
            this.databaseStatisticsTimeSpanEditor.TimeSpan = System.TimeSpan.Parse("00:01:00");
            // 
            // headerStrip1
            // 
            this.mainTableLayout.SetColumnSpan(this.headerStrip1, 2);
            this.headerStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStrip1.Location = new System.Drawing.Point(3, 65);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(489, 25);
            this.headerStrip1.TabIndex = 31;
            this.headerStrip1.Text = "How often should diagnostic data be collected for this SQL Server?";
            this.headerStrip1.WordWrap = false;
            // 
            // tagsDropDownButton
            // 
            this.tagsDropDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //appearance1.BackColor = System.Drawing.Color.White;
            //appearance1.BorderColor = System.Drawing.SystemColors.ControlDark;
            //appearance1.TextHAlignAsString = "Left";
            //this.tagsDropDownButton.Appearance = appearance1;
            this.tagsDropDownButton.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.mainTableLayout.SetColumnSpan(this.tagsDropDownButton, 2);
            this.tagsDropDownButton.Location = new System.Drawing.Point(3, 34);
            this.tagsDropDownButton.Name = "tagsDropDownButton";
            this.tagsDropDownButton.PopupItemKey = "tagsPopupMenu";
            this.tagsDropDownButton.PopupItemProvider = this.toolbarManager;
            this.tagsDropDownButton.ShowFocusRect = false;
            this.tagsDropDownButton.ShowOutline = false;
            this.tagsDropDownButton.Size = new System.Drawing.Size(489, 25);
            this.tagsDropDownButton.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            this.tagsDropDownButton.TabIndex = 30;
            this.tagsDropDownButton.UseAppStyling = false;
            this.tagsDropDownButton.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            //dropdown for server type


            this.cmbServerType.FormattingEnabled = true;
            this.mainTableLayout.SetColumnSpan(this.cmbServerType, 2);
            this.cmbServerType.Location = new System.Drawing.Point(3, 34);
            this.cmbServerType.Name = "comboBox1";
            this.cmbServerType.Size = new System.Drawing.Size(489, 25);
            this.cmbServerType.TabIndex = 30;
            this.cmbServerType.SelectedIndexChanged +=new System.EventHandler(cmbServerType_SelectedIndexChanged);



            //this.tagsDropDownButtonServerType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            //appearance1.BackColor = System.Drawing.Color.White;
            //appearance1.BorderColor = System.Drawing.SystemColors.ControlDark;
            //appearance1.TextHAlignAsString = "Left";
            //this.tagsDropDownButtonServerType.Appearance = appearance1;
            //this.tagsDropDownButtonServerType.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            //this.mainTableLayout.SetColumnSpan(this.tagsDropDownButtonServerType, 2);
            //this.tagsDropDownButtonServerType.Location = new System.Drawing.Point(3, 34);
            //this.tagsDropDownButtonServerType.Name = "tagsDropDownButtonServerType";
            //this.tagsDropDownButtonServerType.PopupItemKey = "tagsPopupMenu";
            //this.tagsDropDownButtonServerType.PopupItemProvider = this.toolbarManager;
            //this.tagsDropDownButtonServerType.ShowFocusRect = false;
            //this.tagsDropDownButtonServerType.ShowOutline = false;
            //this.tagsDropDownButtonServerType.Size = new System.Drawing.Size(489, 25);
            //this.tagsDropDownButtonServerType.Style = Infragistics.Win.Misc.SplitButtonDisplayStyle.DropDownButtonOnly;
            //this.tagsDropDownButtonServerType.TabIndex = 30;
            //this.tagsDropDownButtonServerType.UseAppStyling = false;
            //this.tagsDropDownButtonServerType.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;






            // 
            // headerStrip2
            // 
            this.mainTableLayout.SetColumnSpan(this.headerStrip2, 2);
            this.headerStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStrip2.Location = new System.Drawing.Point(3, 35);
            this.headerStrip2.Name = "headerStrip2";
            this.headerStrip2.Size = new System.Drawing.Size(489, 25);
            this.headerStrip2.TabIndex = 31;
            this.headerStrip2.Text = "Want to give a friendly name for this SQL Server?";
            this.headerStrip2.WordWrap = false;
            // 
            // friendlyNameTextbox
            // 
            this.mainTableLayout.SetColumnSpan(this.friendlyNameTextbox, 2);
            this.friendlyNameTextbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            //this.friendlyNameTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.friendlyNameTextbox.Enabled = true;
            this.friendlyNameTextbox.Location = new System.Drawing.Point(23, 35);
            //this.friendlyNameTextbox.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.friendlyNameTextbox.Name = "friendlyNameTextbox";
            this.friendlyNameTextbox.Size = new System.Drawing.Size(449, 20);
            this.friendlyNameTextbox.TabIndex = 19;

            // 
            // toolbarManager
            // 
            this.toolbarManager.DesignerFlags = 1;
            this.toolbarManager.DockWithinContainer = this;
            this.toolbarManager.DockWithinContainer.BackColor = backColor;
            this.toolbarManager.DockWithinContainer.ForeColor = foreColor;
            this.toolbarManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.toolbarManager.SettingsKey = "MonitoredSqlServerInstancePropertiesDialog.toolbarManager";
            this.toolbarManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "tagsPopupMenu";
            this.toolbarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            this.toolbarManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarManager_ToolClick);
            // 
            // propertiesHeaderStrip17
            // 
            this.mainTableLayout.SetColumnSpan(this.propertiesHeaderStrip17, 2);
            this.propertiesHeaderStrip17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip17.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip17.Name = "propertiesHeaderStrip17";
            this.propertiesHeaderStrip17.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip17.TabIndex = 29;
            this.propertiesHeaderStrip17.Text = "Which tags should be associated with this SQL Server?";
            this.propertiesHeaderStrip17.WordWrap = false;
            this.propertiesHeaderStrip17.BackColor = backColor;
            this.propertiesHeaderStrip17.ForeColor = foreColor;



            this.mainTableLayout.SetColumnSpan(this.propertiesHeaderStripServerType, 2);
            this.propertiesHeaderStripServerType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStripServerType.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStripServerType.Name = "propertiesHeaderStripServerType";
            this.propertiesHeaderStripServerType.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStripServerType.TabIndex = 29;
            this.propertiesHeaderStripServerType.Text = "What type of server is this?";
            this.propertiesHeaderStripServerType.WordWrap = false;
            this.propertiesHeaderStripServerType.BackColor = backcolor;
            this.propertiesHeaderStripServerType.ForeColor = foreColor;


            // 
            // popularPropertiesContentPage
            // 
            this.popularPropertiesContentPage.BackColor = backColor;
            this.popularPropertiesContentPage.ForeColor = foreColor;
            this.popularPropertiesContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.popularPropertiesContentPage.ContentPanel.AutoScroll = true;
            this.popularPropertiesContentPage.ContentPanel.BackColor = backColor;
            this.popularPropertiesContentPage.ContentPanel.ForeColor = foreColor;
            this.popularPropertiesContentPage.ContentPanel.BackColor2 = backColor;
            this.popularPropertiesContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.popularPropertiesContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.popularPropertiesContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.popularPropertiesContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.popularPropertiesContentPage.ContentPanel.Name = "ContentPanel";
            this.popularPropertiesContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.popularPropertiesContentPage.ContentPanel.ShowBorder = false;
            this.popularPropertiesContentPage.ContentPanel.Size = new System.Drawing.Size(504, 632);
            this.popularPropertiesContentPage.ContentPanel.TabIndex = 1;
            this.popularPropertiesContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.popularPropertiesContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.PopularProperties;
            this.popularPropertiesContentPage.Location = new System.Drawing.Point(0, 0);
            this.popularPropertiesContentPage.Name = "popularPropertiesContentPage";
            this.popularPropertiesContentPage.Size = new System.Drawing.Size(506, 689);
            this.popularPropertiesContentPage.TabIndex = 0;
            this.popularPropertiesContentPage.Text = "Change the most common properties for this monitored SQL Server.";
            this.popularPropertiesContentPage.BackColor = backcolor;
            

            
            // 
            // propertyPage1
            // 
            this.propertyPage1.Controls.Add(this.baselineConfiguration1);
            this.propertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPage1.Location = new System.Drawing.Point(0, 0);
            this.propertyPage1.Name = "propertyPage1";
            this.propertyPage1.Size = new System.Drawing.Size(251, 322);
            this.propertyPage1.TabIndex = 0;
            this.propertyPage1.Text = "Baseline Configuration";
            this.propertyPage1.AutoScroll = true;
            // 
            // baselineConfiguration1
            // 
            this.baselineConfiguration1.BorderWidth = 1;
            this.baselineConfiguration1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baselineConfiguration1.HeaderVisible = true;
            this.baselineConfiguration1.IsMultiEdit = false;
            this.baselineConfiguration1.Location = new System.Drawing.Point(0, 0);
            this.baselineConfiguration1.Name = "baselineConfiguration1";
            this.baselineConfiguration1.Size = new System.Drawing.Size(251, 322);
            this.baselineConfiguration1.TabIndex = 0;

            //10.0 SQLdm srishti purohit -- Doctor configuration for UI
            // 
            // analysisConfigurationPage
            // 
            this.analysisConfigurationPage.BorderWidth = 1;
            this.analysisConfigurationPage.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.analysisConfigurationPage.TableLayoutPanelOptionSettings = true;
            //this.analysisConfigurationPage.IsMultiEdit = false;
            this.analysisConfigurationPage.Location = new System.Drawing.Point(0, 0);
            this.analysisConfigurationPage.Name = "analysisConfiguration";
            this.analysisConfigurationPage.Size = new System.Drawing.Size(251, 322);
            this.analysisConfigurationPage.TabIndex = 0;
            this.analysisConfigurationPage.AutoScroll = true;
            // 
            // queryMonitorPropertyPage
            // 
            this.queryMonitorPropertyPage.Controls.Add(this.tableLayoutPanel18);
            this.queryMonitorPropertyPage.Controls.Add(this.office2007PropertyPage4);
            this.queryMonitorPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMonitorPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.queryMonitorPropertyPage.Name = "queryMonitorPropertyPage";
            this.queryMonitorPropertyPage.Size = new System.Drawing.Size(506, 689);
            this.queryMonitorPropertyPage.TabIndex = 0;
            this.queryMonitorPropertyPage.Text = "Query Monitor";
            this.queryMonitorPropertyPage.AutoScroll = true;
            // 
            // tableLayoutPanel18
            // 
            this.tableLayoutPanel18.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel18.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel18.ColumnCount = 1;
            this.tableLayoutPanel18.AutoScroll = true;
            this.tableLayoutPanel18.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel18.Controls.Add(this.propertiesHeaderStrip2, 0, 0);
            this.tableLayoutPanel18.Controls.Add(this.tableLayoutPanel17, 0, 1);
            this.tableLayoutPanel18.Controls.Add(this.propertiesHeaderStrip29, 0, 2); //SQLdm 9.0 (Ankit Srivastava) - Improve Query Monitoring - adding new header strip and panel to Query Monitor panel
            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- adding new controls
            this.tableLayoutPanel18.Controls.Add(this.tableLayoutPanel19, 0, 3);
            this.tableLayoutPanel18.Controls.Add(this.propertiesHeaderStrip4, 0, 4);
            this.tableLayoutPanel18.Controls.Add(this.tableLayoutPanel16, 0, 5);
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            this.tableLayoutPanel18.Controls.Add(this.queryMonitorAdvancedOptionsButton, 0, 7);
            //this.tableLayoutPanel18.Controls.Add(this.queryMonitorAdvancedOptionsButton, 0, 9);
            this.tableLayoutPanel18.Location = new System.Drawing.Point(5, 54);
            this.tableLayoutPanel18.Name = "tableLayoutPanel18";
            this.tableLayoutPanel18.RowCount = 10; //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- changed the row count
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel18.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel18.Size = new System.Drawing.Size(495, 622); //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  - increased the size of panel
            this.tableLayoutPanel18.TabIndex = 4;
            this.tableLayoutPanel18.AutoScroll = true;
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip2.TabIndex = 14;
            this.propertiesHeaderStrip2.Text = "Would you like to enable the Query Monitor?";
            this.propertiesHeaderStrip2.WordWrap = false;
            // 
            // tableLayoutPanel17
            // 
            this.tableLayoutPanel17.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel17.ColumnCount = 3;
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel17.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel17.Controls.Add(this.informationBox1, 0, 0);
            this.tableLayoutPanel17.Controls.Add(this.enableQueryMonitorTraceCheckBox, 1, 1);
            this.tableLayoutPanel17.Controls.Add(this.queryMonitorWarningImage, 0, 2);
            this.tableLayoutPanel17.Controls.Add(this.queryMonitorRunningMessage, 1, 2);
            this.tableLayoutPanel17.Controls.Add(this.lblQueryMonitoringWarningLabel, 0, 4); //Query monitoring warning label
            this.tableLayoutPanel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel17.Location = new System.Drawing.Point(3, 34);
            this.tableLayoutPanel17.Name = "tableLayoutPanel17";
            this.tableLayoutPanel17.RowCount = 4;
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            this.tableLayoutPanel17.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel17.Size = new System.Drawing.Size(489, 127);
            this.tableLayoutPanel17.TabIndex = 3;
            // 
            // informationBox1
            // 
            this.tableLayoutPanel17.SetColumnSpan(this.informationBox1, 3);
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox1.Location = new System.Drawing.Point(3, 3);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(483, 58);
            this.informationBox1.TabIndex = 15;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // enableQueryMonitorTraceCheckBox
            // 
            this.enableQueryMonitorTraceCheckBox.AutoSize = true;
            this.enableQueryMonitorTraceCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.enableQueryMonitorTraceCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enableQueryMonitorTraceCheckBox.Location = new System.Drawing.Point(38, 67);
            this.enableQueryMonitorTraceCheckBox.Name = "enableQueryMonitorTraceCheckBox";
            this.enableQueryMonitorTraceCheckBox.Size = new System.Drawing.Size(146, 20);
            this.enableQueryMonitorTraceCheckBox.TabIndex = 0;
            this.enableQueryMonitorTraceCheckBox.Text = "Enable the Query Monitor";
            this.enableQueryMonitorTraceCheckBox.UseVisualStyleBackColor = false;
            this.enableQueryMonitorTraceCheckBox.CheckedChanged += new System.EventHandler(this.enableQueryMonitorTraceCheckBox_CheckedChanged);
            // 
            // queryMonitorWarningImage
            // 
            this.queryMonitorWarningImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryMonitorWarningImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            this.queryMonitorWarningImage.Location = new System.Drawing.Point(16, 93);
            this.queryMonitorWarningImage.Name = "queryMonitorWarningImage";
            this.queryMonitorWarningImage.Size = new System.Drawing.Size(16, 16);
            this.queryMonitorWarningImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.queryMonitorWarningImage.TabIndex = 20;
            this.queryMonitorWarningImage.TabStop = false;
            // 
            // queryMonitorRunningMessage
            // 
            this.queryMonitorRunningMessage.AutoEllipsis = true;
            this.queryMonitorRunningMessage.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel17.SetColumnSpan(this.queryMonitorRunningMessage, 2);
            this.queryMonitorRunningMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryMonitorRunningMessage.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.queryMonitorRunningMessage.Location = new System.Drawing.Point(38, 90);
            this.queryMonitorRunningMessage.Name = "queryMonitorRunningMessage";
            this.queryMonitorRunningMessage.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.queryMonitorRunningMessage.Size = new System.Drawing.Size(448, 40);
            this.queryMonitorRunningMessage.TabIndex = 21;
            this.queryMonitorRunningMessage.Text = "The Query Monitor is running as the result of an alert action and will automatica" +
                "lly stop after {0}.  If the Query Monitor is enabled above, it will clear the st" +
                "op time and run indefinitely.";
            // 
            // propertiesHeaderStrip29 //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  - adding new property header strip
            // 
            this.propertiesHeaderStrip29.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.propertiesHeaderStrip29.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip29.Name = "propertiesHeaderStrip29";
            this.propertiesHeaderStrip29.Size = new System.Drawing.Size(489, 25);
            //this.propertiesHeaderStrip29.TabIndex = 14;
            this.propertiesHeaderStrip29.Text = "Select what data to collect";
            this.propertiesHeaderStrip29.WordWrap = false;
            // 
            // tableLayoutPanel19 //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- adding new controls - adding new panel 
            // 
            this.tableLayoutPanel19.AutoSize = true;
            this.tableLayoutPanel19.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel19.ColumnCount = 2;
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel19.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            // QUery Plan QS
            this.lblQueryPlanQsMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblQueryPlanQsMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQueryPlanQsMessage.Size = new System.Drawing.Size(450, 25);
            this.lblQueryPlanQsMessage.Text = "Enabling Query Store on the databases is required for collecting the data.";

            //START: SQLdm 10.0 (Tarun Sapra)- Added a checkbox for collecting only estimated query plans
            this.lblQueryPlanMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.lblQueryPlanMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblQueryPlanMessage.Size = new System.Drawing.Size(450, 45);
            this.lblQueryPlanMessage.Text = "Enabling actual query execution plans collection can have a significant performance overhead. " +
                                            "IDERA recommends using this feature only when troubleshooting or monitoring specific problems" +
                                            " for short periods of time. You can also use estimated query plans.";
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            this.tableLayoutPanel19.Controls.Add(this.rButtonUseQueryStore, 1, 0);
            this.tableLayoutPanel19.Controls.Add(this.lblQueryPlanQsMessage, 1, 1);
            this.tableLayoutPanel19.Controls.Add(this.rButtonUseExtendedEvents, 1, 2);
            this.tableLayoutPanel19.Controls.Add(this.lblQueryPlanMessage, 1, 3);
            this.tableLayoutPanel19.Controls.Add(this.chkCollectQueryPlans, 1, 4);
            this.tableLayoutPanel19.Controls.Add(this.chkCollectEstimatedQueryPlans, 1, 5);
            this.tableLayoutPanel19.Controls.Add(this.rButtonUseTrace, 1, 6);
            //END: SQLdm 10.0 (Tarun Sapra)- Added a checkbox for collecting only estimated query plans
            this.tableLayoutPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel19.Location = new System.Drawing.Point(3, 34);
            this.tableLayoutPanel19.Name = "tableLayoutPanel19";
            this.tableLayoutPanel19.RowCount = 4;

            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle());
            //this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel19.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel19.Size = new System.Drawing.Size(489, 115);
            this.tableLayoutPanel19.TabIndex = 3;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            // rButtonUseQueryStoreEvents
            this.rButtonUseQueryStore.AutoSize = true;
            this.rButtonUseQueryStore.BackColor = System.Drawing.Color.Transparent;
            //this.rButtonUseQueryStore.Location = new System.Drawing.Point(38, 70);
            this.rButtonUseQueryStore.Name = "rButtonUseQueryStore";
            this.rButtonUseQueryStore.Size = new System.Drawing.Size(217, 17);
            //this.rButtonUseQueryStore.TabIndex = 32;
            this.rButtonUseQueryStore.Checked = true;
            this.rButtonUseQueryStore.Text = "Collect query data using Query Store (SQL Server 2016 and up only)";
            this.rButtonUseQueryStore.UseVisualStyleBackColor = false;
            this.rButtonUseQueryStore.CheckedChanged += new System.EventHandler(this.rButtonClicked);
            // 
            // rButtonUseExtendedEvents //SQLdm 9.0 (Ankit Srivastava) - Improve Query Monitoring - adding new radio button (Extended events)
            // 
            this.rButtonUseExtendedEvents.AutoSize = true;
            this.rButtonUseExtendedEvents.BackColor = System.Drawing.Color.Transparent;
            //this.rButtonUseExtendedEvents.Location = new System.Drawing.Point(38, 70);
            this.rButtonUseExtendedEvents.Name = "rButtonUseExtendedEvents";
            this.rButtonUseExtendedEvents.Size = new System.Drawing.Size(217, 17);
            //this.rButtonUseExtendedEvents.TabIndex = 32;
            this.rButtonUseExtendedEvents.Checked = true;
            this.rButtonUseExtendedEvents.Text = "Collect query data using extended events (SQL Server 2008 and up only)";
            this.rButtonUseExtendedEvents.UseVisualStyleBackColor = false;
            this.rButtonUseExtendedEvents.CheckedChanged += new System.EventHandler(this.rButtonClicked);
            // 
            // rButtonUseTrace //SQLdm 9.0 (Ankit Srivastava) - Improve Query Monitoring - adding new radio button (SQL trace)
            // 
            this.rButtonUseTrace.AutoSize = true;
            this.rButtonUseTrace.BackColor = System.Drawing.Color.Transparent;
            //this.rButtonUseTrace.Location = new System.Drawing.Point(38, 70);
            this.rButtonUseTrace.Name = "rButtonUseTrace";
            this.rButtonUseTrace.Size = new System.Drawing.Size(217, 17);
            //this.rButtonUseTrace.TabIndex = 33;
            this.rButtonUseTrace.Text = "Collect query data using SQL Trace";
            this.rButtonUseTrace.UseVisualStyleBackColor = false;
            this.rButtonUseTrace.CheckedChanged += new System.EventHandler(this.rButtonClicked);
            // 
            // chkCollectQueryPlans //SQLdm 9.0 (Ankit Srivastava) - Improve Query Monitoring - adding new check box (query plans)
            // 
            this.chkCollectQueryPlans.AutoSize = true;
            this.chkCollectQueryPlans.BackColor = System.Drawing.Color.Transparent;
            //this.chkCollectQueryPlans.Location = new System.Drawing.Point(38, 70);
            //START: SQLdm 10.0 (Tarun Sapra): DE45794 - Query Monitor: Indent the query plan check boxes of extended events
            this.chkCollectQueryPlans.Anchor = AnchorStyles.Left;
            this.chkCollectQueryPlans.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            //END: SQLdm 10.0 (Tarun Sapra): DE45794 - Query Monitor: Indent the query plan check boxes of extended events
            this.chkCollectQueryPlans.Name = "chkCollectQueryPlans";
            this.chkCollectQueryPlans.Size = new System.Drawing.Size(217, 17);
            this.chkCollectQueryPlans.TabIndex = 32;
            this.chkCollectQueryPlans.Text = "Collect Actual Query Plans (SQL Server 2008 and up only)";
            this.chkCollectQueryPlans.UseVisualStyleBackColor = false;
            this.chkCollectQueryPlans.CheckedChanged += new System.EventHandler(this.chkCollectQueryPlans_CheckedChanged);
            //START: SQLdm 10.0 (Tarun Sapra)- Added a checkbox for collecting only estimated query plans
            // 
            // chkCollectEstimatedQueryPlans 
            // 
            this.chkCollectEstimatedQueryPlans.AutoSize = true;
            this.chkCollectEstimatedQueryPlans.BackColor = System.Drawing.Color.Transparent;
            //this.chkCollectEstimatedQueryPlans.Location = new System.Drawing.Point(38, 70);
            //START: SQLdm 10.0 (Tarun Sapra): DE45794 - Query Monitor: Indent the query plan check boxes of extended events
            this.chkCollectEstimatedQueryPlans.Anchor = AnchorStyles.Left;
            this.chkCollectEstimatedQueryPlans.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            //END: SQLdm 10.0 (Tarun Sapra): DE45794 - Query Monitor: Indent the query plan check boxes of extended events
            this.chkCollectEstimatedQueryPlans.Name = "chkCollectEstimatedQueryPlans";
            this.chkCollectEstimatedQueryPlans.Size = new System.Drawing.Size(217, 17);
            this.chkCollectEstimatedQueryPlans.TabIndex = 32;
            this.chkCollectEstimatedQueryPlans.Text = "Collect Estimated Query Plans Only";
            this.chkCollectEstimatedQueryPlans.UseVisualStyleBackColor = false;
            this.chkCollectEstimatedQueryPlans.CheckedChanged += new System.EventHandler(this.chkCollectEstimatedQueryPlans_CheckedChanged);
            //END: SQLdm 10.0 (Tarun Sapra)- Added a checkbox for collecting only estimated query plans
            // 
            // propertiesHeaderStrip4
            // 
            this.propertiesHeaderStrip4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip4.Location = new System.Drawing.Point(3, 167);
            this.propertiesHeaderStrip4.Name = "propertiesHeaderStrip4";
            this.propertiesHeaderStrip4.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStrip4.TabIndex = 17;
            this.propertiesHeaderStrip4.Text = "Types of poorly-performing queries to capture";
            this.propertiesHeaderStrip4.WordWrap = false;
            // 
            // tableLayoutPanel16
            // 
            this.tableLayoutPanel16.AutoSize = true;
            this.tableLayoutPanel16.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel16.ColumnCount = 2;
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel16.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel16.Controls.Add(this.captureSqlBatchesCheckBox, 1, 0);
            this.tableLayoutPanel16.Controls.Add(this.captureSqlStatementsCheckBox, 1, 1);
            this.tableLayoutPanel16.Controls.Add(this.captureStoredProceduresCheckBox, 1, 2);
            this.tableLayoutPanel16.Controls.Add(this.fmePoorlyPerformingThresholds, 1, 3); //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            this.tableLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel16.Location = new System.Drawing.Point(3, 198);
            this.tableLayoutPanel16.Name = "tableLayoutPanel16";
            this.tableLayoutPanel16.RowCount = 4; //SQLdm 10.4 (Ruchika Salwan) -- position changed to fit new UI changes
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel16.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel16.Size = new System.Drawing.Size(492, 252);
            this.tableLayoutPanel16.TabIndex = 2;
            // 
            // captureSqlBatchesCheckBox
            // 
            this.captureSqlBatchesCheckBox.AutoSize = true;
            this.captureSqlBatchesCheckBox.Checked = true;
            this.captureSqlBatchesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captureSqlBatchesCheckBox.Enabled = false;
            this.captureSqlBatchesCheckBox.Location = new System.Drawing.Point(33, 3);
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
            this.captureSqlStatementsCheckBox.Enabled = false;
            this.captureSqlStatementsCheckBox.Location = new System.Drawing.Point(33, 26);
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
            this.captureStoredProceduresCheckBox.Enabled = false;
            this.captureStoredProceduresCheckBox.Location = new System.Drawing.Point(33, 49);
            this.captureStoredProceduresCheckBox.Name = "captureStoredProceduresCheckBox";
            this.captureStoredProceduresCheckBox.Size = new System.Drawing.Size(209, 17);
            this.captureStoredProceduresCheckBox.TabIndex = 4;
            this.captureStoredProceduresCheckBox.Text = "Capture stored procedures and triggers";
            this.captureStoredProceduresCheckBox.UseVisualStyleBackColor = true;
            // 
            // topPlanComboBox
            // 
            this.topPlanComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.topPlanComboBox.Location = new System.Drawing.Point(138, 109);
            this.topPlanComboBox.Name = "topPlanComboBox";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.topPlanComboBox, new System.Drawing.Size(144, 21));
            this.topPlanComboBox.Size = new System.Drawing.Size(160, 17);
            this.topPlanComboBox.TabIndex = 12;
            // this.topPlanComboBox.Items.Add(0, AllThresholds);
            this.topPlanComboBox.Items.Add(0, Duration);
            this.topPlanComboBox.Items.Add(1, LogicalDiskReads);
            this.topPlanComboBox.Items.Add(2, CpuUsage);
            this.topPlanComboBox.Items.Add(3, PhysicalWrites);
            this.topPlanComboBox.Value = 0;
            // 
            // fmePoorlyPerformingThresholds
            // 
            this.fmePoorlyPerformingThresholds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fmePoorlyPerformingThresholds.AutoSize = true;
            this.fmePoorlyPerformingThresholds.Controls.Add(this.traceThresholdsTableLayoutPanel);
            this.fmePoorlyPerformingThresholds.Enabled = false;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            this.fmePoorlyPerformingThresholds.Location = new System.Drawing.Point(33, 92);
            this.fmePoorlyPerformingThresholds.Name = "fmePoorlyPerformingThresholds";
            this.fmePoorlyPerformingThresholds.Size = new System.Drawing.Size(416, 157);
            this.fmePoorlyPerformingThresholds.TabIndex = 5;
            this.fmePoorlyPerformingThresholds.TabStop = false;
            this.fmePoorlyPerformingThresholds.Text = "Define Poorly-Performing Thresholds";
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
            this.traceThresholdsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.durationThresholdSpinner, 1, 0);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.physicalWritesThresholdLabel, 0, 3);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.physicalWritesThresholdSpinner, 1, 3);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.durationThresholdLabel, 0, 0);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.logicalReadsThresholdLabel, 0, 1);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.cpuThresholdLabel, 0, 2);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.logicalReadsThresholdSpinner, 1, 1);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.cpuThresholdSpinner, 1, 2);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.topPlanComboBox, 1, 4);
            this.traceThresholdsTableLayoutPanel.Controls.Add(this.topPlanTableLayoutPanel, 0, 4);

            this.traceThresholdsTableLayoutPanel.Location = new System.Drawing.Point(8, 29);
            this.traceThresholdsTableLayoutPanel.Name = "traceThresholdsTableLayoutPanel";
            this.traceThresholdsTableLayoutPanel.RowCount = 5;
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.traceThresholdsTableLayoutPanel.Size = new System.Drawing.Size(402, 109);
            this.traceThresholdsTableLayoutPanel.TabIndex = 0;
            // 
            // durationThresholdSpinner
            // 
            this.durationThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.durationThresholdSpinner.Enabled = false;
            this.durationThresholdSpinner.Location = new System.Drawing.Point(138, 3);
            this.durationThresholdSpinner.Maximum = int.MaxValue;
            this.durationThresholdSpinner.Name = "durationThresholdSpinner";
            this.durationThresholdSpinner.Size = new System.Drawing.Size(77, 20);
            this.durationThresholdSpinner.TabIndex = 5;
            this.durationThresholdSpinner.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // physicalWritesThresholdLabel
            // 
            this.physicalWritesThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.physicalWritesThresholdLabel.AutoSize = true;
            this.physicalWritesThresholdLabel.Enabled = false;
            this.physicalWritesThresholdLabel.Location = new System.Drawing.Point(3, 87);
            this.physicalWritesThresholdLabel.Name = "physicalWritesThresholdLabel";
            this.physicalWritesThresholdLabel.Size = new System.Drawing.Size(101, 13);
            this.physicalWritesThresholdLabel.TabIndex = 10;
            this.physicalWritesThresholdLabel.Text = "Physical disk writes:";
            this.physicalWritesThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // physicalWritesThresholdSpinner
            // 
            this.physicalWritesThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.physicalWritesThresholdSpinner.Enabled = false;
            this.physicalWritesThresholdSpinner.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.physicalWritesThresholdSpinner.Location = new System.Drawing.Point(138, 83);
            this.physicalWritesThresholdSpinner.Maximum = int.MaxValue;
            this.physicalWritesThresholdSpinner.Name = "physicalWritesThresholdSpinner";
            this.physicalWritesThresholdSpinner.Size = new System.Drawing.Size(77, 20);
            this.physicalWritesThresholdSpinner.TabIndex = 8;
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
            // 
            // durationThresholdLabel
            // 
            this.durationThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.durationThresholdLabel.AutoSize = true;
            this.durationThresholdLabel.Enabled = false;
            this.durationThresholdLabel.Location = new System.Drawing.Point(3, 6);
            this.durationThresholdLabel.Name = "durationThresholdLabel";
            this.durationThresholdLabel.Size = new System.Drawing.Size(115, 13);
            this.durationThresholdLabel.TabIndex = 7;
            this.durationThresholdLabel.Text = "Duration (milliseconds):";
            this.durationThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logicalReadsThresholdLabel
            // 
            this.logicalReadsThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.logicalReadsThresholdLabel.AutoSize = true;
            this.logicalReadsThresholdLabel.Enabled = false;
            this.logicalReadsThresholdLabel.Location = new System.Drawing.Point(3, 32);
            this.logicalReadsThresholdLabel.Name = "logicalReadsThresholdLabel";
            this.logicalReadsThresholdLabel.Size = new System.Drawing.Size(95, 13);
            this.logicalReadsThresholdLabel.TabIndex = 9;
            this.logicalReadsThresholdLabel.Text = "Logical disk reads:";
            this.logicalReadsThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cpuThresholdLabel
            // 
            this.cpuThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cpuThresholdLabel.AutoSize = true;
            this.cpuThresholdLabel.Enabled = false;
            this.cpuThresholdLabel.Location = new System.Drawing.Point(3, 58);
            this.cpuThresholdLabel.Name = "cpuThresholdLabel";
            this.cpuThresholdLabel.Size = new System.Drawing.Size(129, 13);
            this.cpuThresholdLabel.TabIndex = 8;
            this.cpuThresholdLabel.Text = "CPU usage (milliseconds):";
            this.cpuThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // topPlanLabel
            // 
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
            // 
            // logicalReadsThresholdSpinner
            // 
            this.logicalReadsThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.logicalReadsThresholdSpinner.Enabled = false;
            this.logicalReadsThresholdSpinner.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.logicalReadsThresholdSpinner.Location = new System.Drawing.Point(138, 29);
            this.logicalReadsThresholdSpinner.Maximum = int.MaxValue;
            this.logicalReadsThresholdSpinner.Name = "logicalReadsThresholdSpinner";
            this.logicalReadsThresholdSpinner.Size = new System.Drawing.Size(77, 20);
            this.logicalReadsThresholdSpinner.TabIndex = 6;
            // 
            // cpuThresholdSpinner
            // 
            this.cpuThresholdSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cpuThresholdSpinner.Enabled = false;
            this.cpuThresholdSpinner.Location = new System.Drawing.Point(138, 55);
            this.cpuThresholdSpinner.Maximum = int.MaxValue;
            this.cpuThresholdSpinner.Name = "cpuThresholdSpinner";
            this.cpuThresholdSpinner.Size = new System.Drawing.Size(77, 20);
            this.cpuThresholdSpinner.TabIndex = 7;
            // 
            // office2007PropertyPage4
            // 
            this.office2007PropertyPage4.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage4.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage4.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage4.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage4.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage4.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage4.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage4.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage4.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage4.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage4.ContentPanel.Size = new System.Drawing.Size(504, 632);
            this.office2007PropertyPage4.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage4.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardFeaturesImage;
            this.office2007PropertyPage4.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage4.Name = "office2007PropertyPage4";
            this.office2007PropertyPage4.Size = new System.Drawing.Size(506, 689);
            this.office2007PropertyPage4.TabIndex = 0;
            this.office2007PropertyPage4.Text = "Customize the Query Monitor.";
            this.office2007PropertyPage4.BackColor = backcolor;
            // 
            // replicationPropertyPage
            // 
            this.replicationPropertyPage.Controls.Add(this.replicationMainContainer);
            this.replicationPropertyPage.Controls.Add(this.replicationPropertyPageContentPanel);
            this.replicationPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicationPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.replicationPropertyPage.Name = "replicationPropertyPage";
            this.replicationPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.replicationPropertyPage.TabIndex = 0;
            this.replicationPropertyPage.Text = "Replication";
            this.replicationPropertyPage.AutoScroll = true;
            // 
            // replicationMainContainer
            // 
            this.replicationMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replicationMainContainer.BackColor = System.Drawing.Color.White;
            this.replicationMainContainer.ColumnCount = 2;
            this.replicationMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.replicationMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.replicationMainContainer.Controls.Add(this.propertiesHeaderStrip12, 0, 0);
            this.replicationMainContainer.Controls.Add(this.informationBox3, 0, 1);
            this.replicationMainContainer.Controls.Add(this.disableReplicationStuff, 1, 2);
            this.replicationMainContainer.Location = new System.Drawing.Point(5, 54);
            this.replicationMainContainer.Name = "replicationMainContainer";
            this.replicationMainContainer.RowCount = 3;
            this.replicationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.replicationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.replicationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.replicationMainContainer.Size = new System.Drawing.Size(240, 200);
            this.replicationMainContainer.TabIndex = 1;
            // 
            // propertiesHeaderStrip12
            // 
            this.replicationMainContainer.SetColumnSpan(this.propertiesHeaderStrip12, 2);
            this.propertiesHeaderStrip12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip12.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip12.Name = "propertiesHeaderStrip12";
            this.propertiesHeaderStrip12.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip12.TabIndex = 17;
            this.propertiesHeaderStrip12.Text = "Would you like to disable collection of replication statistics?";
            this.propertiesHeaderStrip12.WordWrap = false;
            // 
            // informationBox3
            // 
            this.informationBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replicationMainContainer.SetColumnSpan(this.informationBox3, 2);
            this.informationBox3.Location = new System.Drawing.Point(3, 36);
            this.informationBox3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.informationBox3.Name = "informationBox3";
            this.informationBox3.Size = new System.Drawing.Size(234, 51);
            this.informationBox3.TabIndex = 18;
            this.informationBox3.Text = resources.GetString("informationBox3.Text");
            // 
            // disableReplicationStuff
            // 
            this.disableReplicationStuff.AutoSize = true;
            this.disableReplicationStuff.Location = new System.Drawing.Point(48, 95);
            this.disableReplicationStuff.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.disableReplicationStuff.Name = "disableReplicationStuff";
            this.disableReplicationStuff.Size = new System.Drawing.Size(189, 17);
            this.disableReplicationStuff.TabIndex = 16;
            this.disableReplicationStuff.Text = "Disable replication statistics collection";
            this.disableReplicationStuff.UseVisualStyleBackColor = true;
            // 
            // replicationPropertyPageContentPanel
            // 
            this.replicationPropertyPageContentPanel.BackColor = System.Drawing.Color.White;
            this.replicationPropertyPageContentPanel.BorderWidth = 1;
            // 
            // 
            // 
            this.replicationPropertyPageContentPanel.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.replicationPropertyPageContentPanel.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.replicationPropertyPageContentPanel.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicationPropertyPageContentPanel.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.replicationPropertyPageContentPanel.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.replicationPropertyPageContentPanel.ContentPanel.Name = "ContentPanel";
            this.replicationPropertyPageContentPanel.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.replicationPropertyPageContentPanel.ContentPanel.ShowBorder = false;
            this.replicationPropertyPageContentPanel.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.replicationPropertyPageContentPanel.ContentPanel.TabIndex = 1;
            this.replicationPropertyPageContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicationPropertyPageContentPanel.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.replicationPropertyPageContentPanel.Location = new System.Drawing.Point(0, 0);
            this.replicationPropertyPageContentPanel.Name = "replicationPropertyPageContentPanel";
            this.replicationPropertyPageContentPanel.Size = new System.Drawing.Size(251, 322);
            this.replicationPropertyPageContentPanel.TabIndex = 0;
            this.replicationPropertyPageContentPanel.Text = "Customize replication collection.";
            this.replicationPropertyPageContentPanel.BackColor = backcolor;
            // 
            // tableStatisticsPropertyPage
            // 
            this.tableStatisticsPropertyPage.Controls.Add(this.tableLayoutPanel9);
            this.tableStatisticsPropertyPage.Controls.Add(this.office2007PropertyPage2);
            this.tableStatisticsPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableStatisticsPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.tableStatisticsPropertyPage.Name = "tableStatisticsPropertyPage";
            this.tableStatisticsPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.tableStatisticsPropertyPage.TabIndex = 0;
            this.tableStatisticsPropertyPage.Text = "Table Statistics";
            this.tableStatisticsPropertyPage.AutoScroll = true;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel9.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.label25, 1, 11);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel1, 1, 10);
            this.tableLayoutPanel9.Controls.Add(this.propertiesHeaderStrip9, 0, 9);
            this.tableLayoutPanel9.Controls.Add(this.selectTableStatisticsTablesButton, 1, 8);
            this.tableLayoutPanel9.Controls.Add(this.propertiesHeaderStrip7, 0, 7);
            this.tableLayoutPanel9.Controls.Add(this.flowLayoutPanel2, 1, 6);
            this.tableLayoutPanel9.Controls.Add(this.propertiesHeaderStrip10, 0, 5);
            this.tableLayoutPanel9.Controls.Add(this.flowLayoutPanel7, 1, 4);
            this.tableLayoutPanel9.Controls.Add(this.propertiesHeaderStrip6, 0, 3);
            this.tableLayoutPanel9.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel9.Controls.Add(this.informationBox2, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.propertiesHeaderStrip5, 0, 0);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(5, 54);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 12;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(240, 530);
            this.tableLayoutPanel9.TabIndex = 5;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(23, 508);
            this.label25.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(184, 24);
            this.label25.TabIndex = 45;
            this.label25.Text = "Times are local to the monitored SQL Server instance.";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lastTableGrowthCollectionTimeDescriptionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lastTableGrowthCollectionTimeLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lastTableFragmentationCollectionTimeLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lastTableFragmentationCollectionTimeDescriptionLabel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(23, 379);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(214, 119);
            this.tableLayoutPanel1.TabIndex = 44;
            // 
            // lastTableGrowthCollectionTimeDescriptionLabel
            // 
            this.lastTableGrowthCollectionTimeDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lastTableGrowthCollectionTimeDescriptionLabel.AutoSize = true;
            this.lastTableGrowthCollectionTimeDescriptionLabel.Location = new System.Drawing.Point(3, 24);
            this.lastTableGrowthCollectionTimeDescriptionLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lastTableGrowthCollectionTimeDescriptionLabel.Name = "lastTableGrowthCollectionTimeDescriptionLabel";
            this.lastTableGrowthCollectionTimeDescriptionLabel.Size = new System.Drawing.Size(139, 13);
            this.lastTableGrowthCollectionTimeDescriptionLabel.TabIndex = 35;
            this.lastTableGrowthCollectionTimeDescriptionLabel.Text = "Last table growth collection:";
            // 
            // lastTableGrowthCollectionTimeLabel
            // 
            this.lastTableGrowthCollectionTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lastTableGrowthCollectionTimeLabel.AutoSize = true;
            this.lastTableGrowthCollectionTimeLabel.Location = new System.Drawing.Point(180, 5);
            this.lastTableGrowthCollectionTimeLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lastTableGrowthCollectionTimeLabel.Name = "lastTableGrowthCollectionTimeLabel";
            this.lastTableGrowthCollectionTimeLabel.Size = new System.Drawing.Size(30, 52);
            this.lastTableGrowthCollectionTimeLabel.TabIndex = 37;
            this.lastTableGrowthCollectionTimeLabel.Text = "< Date/Time >";
            // 
            // lastTableFragmentationCollectionTimeLabel
            // 
            this.lastTableFragmentationCollectionTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lastTableFragmentationCollectionTimeLabel.AutoSize = true;
            this.lastTableFragmentationCollectionTimeLabel.Location = new System.Drawing.Point(180, 62);
            this.lastTableFragmentationCollectionTimeLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lastTableFragmentationCollectionTimeLabel.Name = "lastTableFragmentationCollectionTimeLabel";
            this.lastTableFragmentationCollectionTimeLabel.Size = new System.Drawing.Size(30, 52);
            this.lastTableFragmentationCollectionTimeLabel.TabIndex = 39;
            this.lastTableFragmentationCollectionTimeLabel.Text = "< Date/Time >";
            // 
            // lastTableFragmentationCollectionTimeDescriptionLabel
            // 
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lastTableFragmentationCollectionTimeDescriptionLabel.AutoSize = true;
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Location = new System.Drawing.Point(3, 81);
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Name = "lastTableFragmentationCollectionTimeDescriptionLabel";
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Size = new System.Drawing.Size(171, 13);
            this.lastTableFragmentationCollectionTimeDescriptionLabel.TabIndex = 38;
            this.lastTableFragmentationCollectionTimeDescriptionLabel.Text = "Last table fragmentation collection:";
            // 
            // propertiesHeaderStrip9
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.propertiesHeaderStrip9, 2);
            this.propertiesHeaderStrip9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip9.Location = new System.Drawing.Point(3, 346);
            this.propertiesHeaderStrip9.Name = "propertiesHeaderStrip9";
            this.propertiesHeaderStrip9.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip9.TabIndex = 43;
            this.propertiesHeaderStrip9.Text = "When was the last successful table statistics collection?";
            this.propertiesHeaderStrip9.WordWrap = false;
            // 
            // selectTableStatisticsTablesButton
            // 
            this.selectTableStatisticsTablesButton.AutoSize = true;
            //this.selectTableStatisticsTablesButton.BackColor = System.Drawing.SystemColors.Control;
            //this.selectTableStatisticsTablesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.selectTableStatisticsTablesButton.Location = new System.Drawing.Point(23, 316);
            this.selectTableStatisticsTablesButton.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.selectTableStatisticsTablesButton.Name = "selectTableStatisticsTablesButton";
            this.selectTableStatisticsTablesButton.Size = new System.Drawing.Size(158, 22);
            this.selectTableStatisticsTablesButton.TabIndex = 42;
            this.selectTableStatisticsTablesButton.Text = "Select Databases to Exclude";
            this.selectTableStatisticsTablesButton.UseVisualStyleBackColor = true;
            this.selectTableStatisticsTablesButton.Click += new System.EventHandler(this.selectTableStatisticsTablesButton_Click);
            // 
            // propertiesHeaderStrip7
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.propertiesHeaderStrip7, 2);
            this.propertiesHeaderStrip7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip7.Location = new System.Drawing.Point(3, 283);
            this.propertiesHeaderStrip7.Name = "propertiesHeaderStrip7";
            this.propertiesHeaderStrip7.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip7.TabIndex = 41;
            this.propertiesHeaderStrip7.Text = "Would you like to exclude databases from having table statistics collected?";
            this.propertiesHeaderStrip7.WordWrap = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel2.Controls.Add(this.label3);
            this.flowLayoutPanel2.Controls.Add(this.minimumTableSize);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(23, 236);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(214, 39);
            this.flowLayoutPanel2.TabIndex = 40;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 41;
            this.label3.Text = "Table Size (Kilobytes):";
            // 
            // minimumTableSize
            // 
            this.minimumTableSize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.minimumTableSize.Location = new System.Drawing.Point(3, 16);
            this.minimumTableSize.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.minimumTableSize.Name = "minimumTableSize";
            this.minimumTableSize.Size = new System.Drawing.Size(107, 20);
            this.minimumTableSize.TabIndex = 42;
            this.minimumTableSize.Value = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            // 
            // propertiesHeaderStrip10
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.propertiesHeaderStrip10, 2);
            this.propertiesHeaderStrip10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip10.Location = new System.Drawing.Point(3, 203);
            this.propertiesHeaderStrip10.Name = "propertiesHeaderStrip10";
            this.propertiesHeaderStrip10.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip10.TabIndex = 39;
            this.propertiesHeaderStrip10.Text = "What is the minimum table size for collecting table reorganization statistics?";
            this.propertiesHeaderStrip10.WordWrap = false;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsSundayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsMondayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsTuesdayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsWednesdayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsThursdayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsFridayCheckBox);
            this.flowLayoutPanel7.Controls.Add(this.collectTableStatsSaturdayCheckBox);
            this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(23, 169);
            this.flowLayoutPanel7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(214, 26);
            this.flowLayoutPanel7.TabIndex = 24;
            // 
            // collectTableStatsSundayCheckBox
            // 
            this.collectTableStatsSundayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsSundayCheckBox.AutoSize = true;
            this.collectTableStatsSundayCheckBox.Location = new System.Drawing.Point(3, 3);
            this.collectTableStatsSundayCheckBox.Name = "collectTableStatsSundayCheckBox";
            this.collectTableStatsSundayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.collectTableStatsSundayCheckBox.TabIndex = 24;
            this.collectTableStatsSundayCheckBox.Text = "Sun";
            this.collectTableStatsSundayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsMondayCheckBox
            // 
            this.collectTableStatsMondayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsMondayCheckBox.AutoSize = true;
            this.collectTableStatsMondayCheckBox.Checked = true;
            this.collectTableStatsMondayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.collectTableStatsMondayCheckBox.Location = new System.Drawing.Point(54, 3);
            this.collectTableStatsMondayCheckBox.Name = "collectTableStatsMondayCheckBox";
            this.collectTableStatsMondayCheckBox.Size = new System.Drawing.Size(47, 17);
            this.collectTableStatsMondayCheckBox.TabIndex = 25;
            this.collectTableStatsMondayCheckBox.Text = "Mon";
            this.collectTableStatsMondayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsTuesdayCheckBox
            // 
            this.collectTableStatsTuesdayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsTuesdayCheckBox.AutoSize = true;
            this.collectTableStatsTuesdayCheckBox.Checked = true;
            this.collectTableStatsTuesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.collectTableStatsTuesdayCheckBox.Location = new System.Drawing.Point(107, 3);
            this.collectTableStatsTuesdayCheckBox.Name = "collectTableStatsTuesdayCheckBox";
            this.collectTableStatsTuesdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.collectTableStatsTuesdayCheckBox.TabIndex = 26;
            this.collectTableStatsTuesdayCheckBox.Text = "Tue";
            this.collectTableStatsTuesdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsWednesdayCheckBox
            // 
            this.collectTableStatsWednesdayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsWednesdayCheckBox.AutoSize = true;
            this.collectTableStatsWednesdayCheckBox.Checked = true;
            this.collectTableStatsWednesdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.collectTableStatsWednesdayCheckBox.Location = new System.Drawing.Point(158, 3);
            this.collectTableStatsWednesdayCheckBox.Name = "collectTableStatsWednesdayCheckBox";
            this.collectTableStatsWednesdayCheckBox.Size = new System.Drawing.Size(49, 17);
            this.collectTableStatsWednesdayCheckBox.TabIndex = 30;
            this.collectTableStatsWednesdayCheckBox.Text = "Wed";
            this.collectTableStatsWednesdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsThursdayCheckBox
            // 
            this.collectTableStatsThursdayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsThursdayCheckBox.AutoSize = true;
            this.collectTableStatsThursdayCheckBox.Checked = true;
            this.collectTableStatsThursdayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.collectTableStatsThursdayCheckBox.Location = new System.Drawing.Point(3, 26);
            this.collectTableStatsThursdayCheckBox.Name = "collectTableStatsThursdayCheckBox";
            this.collectTableStatsThursdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.collectTableStatsThursdayCheckBox.TabIndex = 31;
            this.collectTableStatsThursdayCheckBox.Text = "Thu";
            this.collectTableStatsThursdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsFridayCheckBox
            // 
            this.collectTableStatsFridayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsFridayCheckBox.AutoSize = true;
            this.collectTableStatsFridayCheckBox.Checked = true;
            this.collectTableStatsFridayCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.collectTableStatsFridayCheckBox.Location = new System.Drawing.Point(54, 26);
            this.collectTableStatsFridayCheckBox.Name = "collectTableStatsFridayCheckBox";
            this.collectTableStatsFridayCheckBox.Size = new System.Drawing.Size(37, 17);
            this.collectTableStatsFridayCheckBox.TabIndex = 32;
            this.collectTableStatsFridayCheckBox.Text = "Fri";
            this.collectTableStatsFridayCheckBox.UseVisualStyleBackColor = true;
            // 
            // collectTableStatsSaturdayCheckBox
            // 
            this.collectTableStatsSaturdayCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.collectTableStatsSaturdayCheckBox.AutoSize = true;
            this.collectTableStatsSaturdayCheckBox.Location = new System.Drawing.Point(97, 26);
            this.collectTableStatsSaturdayCheckBox.Name = "collectTableStatsSaturdayCheckBox";
            this.collectTableStatsSaturdayCheckBox.Size = new System.Drawing.Size(42, 17);
            this.collectTableStatsSaturdayCheckBox.TabIndex = 33;
            this.collectTableStatsSaturdayCheckBox.Text = "Sat";
            this.collectTableStatsSaturdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip6
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.propertiesHeaderStrip6, 2);
            this.propertiesHeaderStrip6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip6.Location = new System.Drawing.Point(3, 136);
            this.propertiesHeaderStrip6.Name = "propertiesHeaderStrip6";
            this.propertiesHeaderStrip6.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip6.TabIndex = 23;
            this.propertiesHeaderStrip6.Text = "What days would you like to collect table statistics?";
            this.propertiesHeaderStrip6.WordWrap = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.quietTimeStartEditor);
            this.flowLayoutPanel1.Controls.Add(this.endTimeLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(23, 88);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(214, 40);
            this.flowLayoutPanel1.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Collect table statistics between";
            // 
            // quietTimeStartEditor
            // 
            this.quietTimeStartEditor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.quietTimeStartEditor.ButtonsRight.Add(dropDownEditorButton1);
            this.quietTimeStartEditor.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.quietTimeStartEditor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.quietTimeStartEditor.FormatString = "h:mm tt";
            this.quietTimeStartEditor.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.quietTimeStartEditor.Location = new System.Drawing.Point(3, 16);
            this.quietTimeStartEditor.MaskInput = "{time}";
            this.quietTimeStartEditor.Name = "quietTimeStartEditor";
            this.quietTimeStartEditor.Size = new System.Drawing.Size(86, 21);
            this.quietTimeStartEditor.TabIndex = 34;
            this.quietTimeStartEditor.Time = System.TimeSpan.Parse("00:00:00");
            this.quietTimeStartEditor.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.quietTimeStartEditor.ValueChanged += new System.EventHandler(this.quietTimeStartEditor_ValueChanged);
            // 
            // endTimeLabel
            // 
            this.endTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endTimeLabel.AutoSize = true;
            this.endTimeLabel.Location = new System.Drawing.Point(95, 20);
            this.endTimeLabel.Name = "endTimeLabel";
            this.endTimeLabel.Size = new System.Drawing.Size(57, 13);
            this.endTimeLabel.TabIndex = 35;
            this.endTimeLabel.Text = "and + 2hrs";
            // 
            // informationBox2
            // 
            this.informationBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel9.SetColumnSpan(this.informationBox2, 2);
            this.informationBox2.Location = new System.Drawing.Point(3, 34);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(234, 51);
            this.informationBox2.TabIndex = 18;
            this.informationBox2.Text = resources.GetString("informationBox2.Text");
            // 
            // propertiesHeaderStrip5
            // 
            this.tableLayoutPanel9.SetColumnSpan(this.propertiesHeaderStrip5, 2);
            this.propertiesHeaderStrip5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip5.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip5.Name = "propertiesHeaderStrip5";
            this.propertiesHeaderStrip5.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip5.TabIndex = 17;
            this.propertiesHeaderStrip5.Text = "When would you like to collect table statistics?";
            this.propertiesHeaderStrip5.WordWrap = false;
            // 
            // office2007PropertyPage2
            // 
            this.office2007PropertyPage2.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage2.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage2.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage2.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage2.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage2.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage2.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage2.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage2.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage2.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.office2007PropertyPage2.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage2.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage2.Name = "office2007PropertyPage2";
            this.office2007PropertyPage2.Size = new System.Drawing.Size(251, 322);
            this.office2007PropertyPage2.TabIndex = 0;
            this.office2007PropertyPage2.Text = "Customize table statistics collection.";
            this.office2007PropertyPage2.BackColor = backcolor;
            // 
            // customCountersPropertyPage
            // 
            this.customCountersPropertyPage.Controls.Add(this.customCountersContentPage);
            this.customCountersPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customCountersPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.customCountersPropertyPage.Name = "customCountersPropertyPage";
            this.customCountersPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.customCountersPropertyPage.TabIndex = 0;
            this.customCountersPropertyPage.Text = "Custom Counters";
            this.customCountersPropertyPage.AutoScroll = true;
            // 
            // customCountersContentPage
            // 
            this.customCountersContentPage.BackColor = System.Drawing.Color.White;
            this.customCountersContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.customCountersContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.customCountersContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.customCountersContentPage.ContentPanel.Controls.Add(this.customCounterStackLayoutPanel);
            this.customCountersContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customCountersContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.customCountersContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.customCountersContentPage.ContentPanel.Name = "ContentPanel";
            this.customCountersContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.customCountersContentPage.ContentPanel.ShowBorder = false;
            this.customCountersContentPage.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.customCountersContentPage.ContentPanel.TabIndex = 1;
            this.customCountersContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customCountersContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.customCountersContentPage.Location = new System.Drawing.Point(0, 0);
            this.customCountersContentPage.Name = "customCountersContentPage";
            this.customCountersContentPage.Size = new System.Drawing.Size(251, 322);
            this.customCountersContentPage.TabIndex = 0;
            this.customCountersContentPage.Text = "Link custom counters to this monitored SQL Server.";
            this.customCountersContentPage.BackColor = backcolor;
            // 
            // customCounterStackLayoutPanel
            // 
            this.customCounterStackLayoutPanel.ActiveControl = this.customCounterContentPanel;
            this.customCounterStackLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customCounterStackLayoutPanel.Controls.Add(this.customCounterContentPanel);
            this.customCounterStackLayoutPanel.Controls.Add(this.customCounterMessageLabel);
            this.customCounterStackLayoutPanel.Location = new System.Drawing.Point(10, 10);
            this.customCounterStackLayoutPanel.Name = "customCounterStackLayoutPanel";
            this.customCounterStackLayoutPanel.Size = new System.Drawing.Size(227, 229);
            this.customCounterStackLayoutPanel.TabIndex = 22;
            // 
            // customCounterContentPanel
            // 
            this.customCounterContentPanel.Controls.Add(this.tableLayoutPanel10);
            this.customCounterContentPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.customCounterContentPanel.Location = new System.Drawing.Point(0, 0);
            this.customCounterContentPanel.Name = "customCounterContentPanel";
            this.customCounterContentPanel.Size = new System.Drawing.Size(227, 229);
            this.customCounterContentPanel.TabIndex = 0;
            this.customCounterContentPanel.AutoScroll = true;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.testCustomCountersButton, 1, 3);
            this.tableLayoutPanel10.Controls.Add(this.customCountersSelectionList, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.propertiesHeaderStrip11, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.informationBox7, 0, 1);
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 4;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.Size = new System.Drawing.Size(221, 223);
            this.tableLayoutPanel10.TabIndex = 24;
            this.tableLayoutPanel10.AutoScroll = true;
            // 
            // testCustomCountersButton
            // 
            this.testCustomCountersButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.testCustomCountersButton.AutoSize = true;
            this.testCustomCountersButton.BackColor = System.Drawing.SystemColors.Control;
            this.testCustomCountersButton.Location = new System.Drawing.Point(143, 197);
            this.testCustomCountersButton.Name = "testCustomCountersButton";
            this.testCustomCountersButton.Size = new System.Drawing.Size(75, 23);
            this.testCustomCountersButton.TabIndex = 26;
            this.testCustomCountersButton.Text = "Test...";
            this.testCustomCountersButton.UseVisualStyleBackColor = true;
            this.testCustomCountersButton.Click += new System.EventHandler(this.testCustomCountersButton_Click);
            // 
            // customCountersSelectionList
            // 
            this.customCountersSelectionList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customCountersSelectionList.AvailableLabel = "Available Counters:";
            this.tableLayoutPanel10.SetColumnSpan(this.customCountersSelectionList, 2);
            this.customCountersSelectionList.Location = new System.Drawing.Point(3, 109);
            this.customCountersSelectionList.Name = "customCountersSelectionList";
            this.customCountersSelectionList.SelectedLabel = "Linked Counters:";
            this.customCountersSelectionList.Size = new System.Drawing.Size(215, 82);
            this.customCountersSelectionList.TabIndex = 25;
            this.customCountersSelectionList.SelectionChanged += new System.EventHandler(this.customCountersSelectionList_SelectionChanged);
            // 
            // propertiesHeaderStrip11
            // 
            this.tableLayoutPanel10.SetColumnSpan(this.propertiesHeaderStrip11, 2);
            this.propertiesHeaderStrip11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip11.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip11.Name = "propertiesHeaderStrip11";
            this.propertiesHeaderStrip11.Size = new System.Drawing.Size(215, 25);
            this.propertiesHeaderStrip11.TabIndex = 24;
            this.propertiesHeaderStrip11.Text = "What custom counters would you like to monitor for this SQL Server instance?";
            this.propertiesHeaderStrip11.WordWrap = false;
            // 
            // informationBox7
            // 
            this.informationBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel10.SetColumnSpan(this.informationBox7, 2);
            this.informationBox7.Location = new System.Drawing.Point(3, 34);
            this.informationBox7.Name = "informationBox7";
            this.informationBox7.Size = new System.Drawing.Size(215, 69);
            this.informationBox7.TabIndex = 23;
            this.informationBox7.Text = resources.GetString("informationBox7.Text");
            // 
            // customCounterMessageLabel
            // 
            this.customCounterMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customCounterMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.customCounterMessageLabel.Name = "customCounterMessageLabel";
            this.customCounterMessageLabel.Size = new System.Drawing.Size(227, 229);
            this.customCounterMessageLabel.TabIndex = 1;
            this.customCounterMessageLabel.Text = "There are no custom counters defined.";
            this.customCounterMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // maintenancePropertyPage
            // 
            this.maintenancePropertyPage.Controls.Add(this.maintenanceModeControlsContainer);
            this.maintenancePropertyPage.Controls.Add(this.maintenanceModeContentPage);
            this.maintenancePropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenancePropertyPage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.maintenancePropertyPage.Location = new System.Drawing.Point(0, 0);
            this.maintenancePropertyPage.Name = "maintenancePropertyPage";
            this.maintenancePropertyPage.Size = new System.Drawing.Size(251, 322);
            this.maintenancePropertyPage.TabIndex = 0;
            this.maintenancePropertyPage.Text = "Maintenance Mode";
            this.maintenancePropertyPage.AutoScroll = true;
            // 
            // maintenanceModeControlsContainer
            // 
            this.maintenanceModeControlsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceModeControlsContainer.BackColor = System.Drawing.Color.White;
            this.maintenanceModeControlsContainer.ColumnCount = 2;
            this.maintenanceModeControlsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.maintenanceModeControlsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.maintenanceModeControlsContainer.Controls.Add(this.informationBox6, 0, 0);
            this.maintenanceModeControlsContainer.Controls.Add(this.propertiesHeaderStrip13, 0, 1);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmNeverRadio, 1, 2);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmAlwaysRadio, 1, 3);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmRecurringRadio, 1, 4);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmOnceRadio, 1, 6);
            this.maintenanceModeControlsContainer.Controls.Add(this.mmMonthlyRecurringRadio, 1, 5);
            this.maintenanceModeControlsContainer.Controls.Add(this.panel2, 0, 7);
            this.maintenanceModeControlsContainer.Location = new System.Drawing.Point(5, 54);
            this.maintenanceModeControlsContainer.Name = "maintenanceModeControlsContainer";
            this.maintenanceModeControlsContainer.RowCount = 8;
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.maintenanceModeControlsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.maintenanceModeControlsContainer.Size = new System.Drawing.Size(240, 450);
            this.maintenanceModeControlsContainer.TabIndex = 2;
            // 
            // informationBox6
            // 
            this.informationBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maintenanceModeControlsContainer.SetColumnSpan(this.informationBox6, 2);
            this.informationBox6.Location = new System.Drawing.Point(3, 3);
            this.informationBox6.Name = "informationBox6";
            this.informationBox6.Size = new System.Drawing.Size(234, 47);
            this.informationBox6.TabIndex = 44;
            this.informationBox6.Text = "Maintenance Mode allows you to temporarily stop alert generation and the collecti" +
    "on of performance metrics for the time period that your SQL Server instance will" +
    " be offline.\r\n";
            // 
            // propertiesHeaderStrip13
            // 
            this.maintenanceModeControlsContainer.SetColumnSpan(this.propertiesHeaderStrip13, 2);
            this.propertiesHeaderStrip13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip13.Location = new System.Drawing.Point(3, 56);
            this.propertiesHeaderStrip13.Name = "propertiesHeaderStrip13";
            this.propertiesHeaderStrip13.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip13.TabIndex = 17;
            this.propertiesHeaderStrip13.Text = "When should this server be in maintenance mode?";
            this.propertiesHeaderStrip13.WordWrap = false;
            // 
            // mmNeverRadio
            // 
            this.mmNeverRadio.AutoSize = true;
            this.mmNeverRadio.Checked = true;
            this.mmNeverRadio.Location = new System.Drawing.Point(63, 87);
            this.mmNeverRadio.Name = "mmNeverRadio";
            this.mmNeverRadio.Size = new System.Drawing.Size(54, 17);
            this.mmNeverRadio.TabIndex = 20;
            this.mmNeverRadio.TabStop = true;
            this.mmNeverRadio.Tag = "0";
            this.mmNeverRadio.Text = "Never";
            this.mmNeverRadio.UseVisualStyleBackColor = true;
            this.mmNeverRadio.CheckedChanged += new System.EventHandler(this.mmNeverRadio_CheckedChanged);
            // 
            // mmAlwaysRadio
            // 
            this.mmAlwaysRadio.AutoSize = true;
            this.mmAlwaysRadio.Location = new System.Drawing.Point(63, 110);
            this.mmAlwaysRadio.Name = "mmAlwaysRadio";
            this.mmAlwaysRadio.Size = new System.Drawing.Size(111, 17);
            this.mmAlwaysRadio.TabIndex = 21;
            this.mmAlwaysRadio.Tag = "1";
            this.mmAlwaysRadio.Text = "Until further notice";
            this.mmAlwaysRadio.UseVisualStyleBackColor = true;
            this.mmAlwaysRadio.CheckedChanged += new System.EventHandler(this.mmAlwaysRadio_CheckedChanged);
            // 
            // mmRecurringRadio
            // 
            this.mmRecurringRadio.AutoSize = true;
            this.mmRecurringRadio.Location = new System.Drawing.Point(63, 133);
            this.mmRecurringRadio.Name = "mmRecurringRadio";
            this.mmRecurringRadio.Size = new System.Drawing.Size(174, 17);
            this.mmRecurringRadio.TabIndex = 22;
            this.mmRecurringRadio.Tag = "2";
            this.mmRecurringRadio.Text = "Recurring every week at the specified time";
            this.mmRecurringRadio.UseVisualStyleBackColor = true;
            this.mmRecurringRadio.CheckedChanged += new System.EventHandler(this.mmWeeklyRadio_CheckedChanged);
            // 
            // mmOnceRadio
            // 
            this.mmOnceRadio.AutoSize = true;
            this.mmOnceRadio.Location = new System.Drawing.Point(63, 156);
            this.mmOnceRadio.Name = "mmOnceRadio";
            this.mmOnceRadio.Size = new System.Drawing.Size(174, 17);
            this.mmOnceRadio.TabIndex = 23;
            this.mmOnceRadio.Tag = "3";
            this.mmOnceRadio.Text = "Occurring once at the specified time";
            this.mmOnceRadio.UseVisualStyleBackColor = true;
            this.mmOnceRadio.CheckedChanged += new System.EventHandler(this.mmOnceRadio_CheckedChanged);
            // 
            // mmMonthlyRecurringRadio
            // 
            this.mmMonthlyRecurringRadio.AutoSize = true;
            this.mmMonthlyRecurringRadio.Location = new System.Drawing.Point(63, 179);
            this.mmMonthlyRecurringRadio.Name = "mmMonthlyRecurringRadio";
            this.mmMonthlyRecurringRadio.Size = new System.Drawing.Size(174, 17);
            this.mmMonthlyRecurringRadio.TabIndex = 24;
            this.mmMonthlyRecurringRadio.Tag = "4";
            this.mmMonthlyRecurringRadio.Text = "Recurring every month at the specified time";
            this.mmMonthlyRecurringRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyRecurringRadio.CheckedChanged += new System.EventHandler(this.mmMonthlyRecurringRadio_CheckedChanged);
            // 
            // panel2
            // 
            this.maintenanceModeControlsContainer.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.mmOncePanel);
            this.panel2.Controls.Add(this.mmRecurringPanel);
            this.panel2.Controls.Add(this.mmMonthlyRecurringPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 179);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(234, 268);
            this.panel2.TabIndex = 1;
            // 
            // mmOncePanel
            // 
            this.mmOncePanel.BackColor = System.Drawing.Color.White;
            this.mmOncePanel.Controls.Add(this.tableLayoutPanel14);
            this.mmOncePanel.Controls.Add(this.propertiesHeaderStrip16);
            this.mmOncePanel.Controls.Add(this.informationBox5);
            this.mmOncePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmOncePanel.Location = new System.Drawing.Point(0, 0);
            this.mmOncePanel.Name = "mmOncePanel";
            this.mmOncePanel.Size = new System.Drawing.Size(234, 268);
            this.mmOncePanel.TabIndex = 43;
            this.mmOncePanel.Visible = false;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.AutoSize = true;
            this.tableLayoutPanel14.ColumnCount = 3;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel14.Controls.Add(this.label21, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.mmServerDateTime, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceBeginTime, 2, 1);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceBeginDate, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceStopTime, 2, 2);
            this.tableLayoutPanel14.Controls.Add(this.mmOnceStopDate, 1, 2);
            this.tableLayoutPanel14.Location = new System.Drawing.Point(45, 85);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 3;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.Size = new System.Drawing.Size(363, 71);
            this.tableLayoutPanel14.TabIndex = 78;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(72, 13);
            this.label21.TabIndex = 77;
            this.label21.Text = "Service Time:";
            // 
            // mmServerDateTime
            // 
            this.mmServerDateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmServerDateTime.AutoSize = true;
            this.mmServerDateTime.Location = new System.Drawing.Point(81, 0);
            this.mmServerDateTime.Name = "mmServerDateTime";
            this.mmServerDateTime.Size = new System.Drawing.Size(67, 13);
            this.mmServerDateTime.TabIndex = 78;
            this.mmServerDateTime.Text = "Refreshing...";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 79;
            this.label6.Text = "Start time:";
            // 
            // mmOnceBeginTime
            // 
            this.mmOnceBeginTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmOnceBeginTime.ButtonsRight.Add(dropDownEditorButton2);
            this.mmOnceBeginTime.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmOnceBeginTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmOnceBeginTime.FormatString = "hh:mm tt";
            this.mmOnceBeginTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmOnceBeginTime.Location = new System.Drawing.Point(183, 16);
            this.mmOnceBeginTime.MaskInput = "hh:mm tt";
            this.mmOnceBeginTime.Name = "mmOnceBeginTime";
            this.mmOnceBeginTime.Size = new System.Drawing.Size(90, 21);
            this.mmOnceBeginTime.TabIndex = 81;
            this.mmOnceBeginTime.Time = System.TimeSpan.Parse("00:00:00");
            this.mmOnceBeginTime.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // mmOnceBeginDate
            // 
            this.mmOnceBeginDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmOnceBeginDate.DateTime = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            this.mmOnceBeginDate.Location = new System.Drawing.Point(81, 16);
            this.mmOnceBeginDate.Name = "mmOnceBeginDate";
            this.mmOnceBeginDate.Size = new System.Drawing.Size(96, 21);
            this.mmOnceBeginDate.TabIndex = 80;
            this.mmOnceBeginDate.Value = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 82;
            this.label7.Text = "Stop time:";
            // 
            // mmOnceStopTime
            // 
            this.mmOnceStopTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton3.Key = "DropDownList";
            dropDownEditorButton3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmOnceStopTime.ButtonsRight.Add(dropDownEditorButton3);
            this.mmOnceStopTime.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmOnceStopTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmOnceStopTime.FormatString = "hh:mm tt";
            this.mmOnceStopTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmOnceStopTime.Location = new System.Drawing.Point(183, 45);
            this.mmOnceStopTime.MaskInput = "hh:mm tt";
            this.mmOnceStopTime.Name = "mmOnceStopTime";
            this.mmOnceStopTime.Size = new System.Drawing.Size(90, 21);
            this.mmOnceStopTime.TabIndex = 84;
            this.mmOnceStopTime.Time = System.TimeSpan.Parse("00:00:00");
            this.mmOnceStopTime.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // mmOnceStopDate
            // 
            this.mmOnceStopDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmOnceStopDate.DateTime = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            this.mmOnceStopDate.Location = new System.Drawing.Point(81, 45);
            this.mmOnceStopDate.Name = "mmOnceStopDate";
            this.mmOnceStopDate.Size = new System.Drawing.Size(96, 21);
            this.mmOnceStopDate.TabIndex = 83;
            this.mmOnceStopDate.Value = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            // 
            // propertiesHeaderStrip16
            // 
            this.propertiesHeaderStrip16.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip16.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip16.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip16.Name = "propertiesHeaderStrip16";
            this.propertiesHeaderStrip16.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip16.TabIndex = 55;
            this.propertiesHeaderStrip16.Text = "What time should this server be in maintenance mode?";
            this.propertiesHeaderStrip16.WordWrap = false;
            // 
            // informationBox5
            // 
            this.informationBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox5.Location = new System.Drawing.Point(10, 31);
            this.informationBox5.Name = "informationBox5";
            this.informationBox5.Size = new System.Drawing.Size(234, 47);
            this.informationBox5.TabIndex = 54;
            this.informationBox5.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // mmRecurringPanel
            // 
            this.mmRecurringPanel.BackColor = System.Drawing.Color.White;
            this.mmRecurringPanel.Controls.Add(this.flowLayoutPanel4);
            this.mmRecurringPanel.Controls.Add(this.flowLayoutPanel3);
            this.mmRecurringPanel.Controls.Add(this.informationBox4);
            this.mmRecurringPanel.Controls.Add(this.mmBeginSatCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginFriCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginThurCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginWedCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginTueCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginMonCheckbox);
            this.mmRecurringPanel.Controls.Add(this.mmBeginSunCheckbox);
            this.mmRecurringPanel.Controls.Add(this.propertiesHeaderStrip15);
            this.mmRecurringPanel.Controls.Add(this.propertiesHeaderStrip14);
            this.mmRecurringPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmRecurringPanel.Location = new System.Drawing.Point(0, 0);
            this.mmRecurringPanel.Name = "mmRecurringPanel";
            this.mmRecurringPanel.Size = new System.Drawing.Size(234, 268);
            this.mmRecurringPanel.TabIndex = 42;
            this.mmRecurringPanel.Visible = false;
            // 
            // mmMonthlyRecurringPanel
            // 
            this.mmMonthlyRecurringPanel.BackColor = System.Drawing.Color.White;
            this.mmMonthlyRecurringPanel.Controls.Add(this.flowLayoutPanel8);
            this.mmMonthlyRecurringPanel.Controls.Add(this.propertiesHeaderStrip27);
            this.mmMonthlyRecurringPanel.Controls.Add(this.informationBox15);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyDayRadio);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputDayLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyOfEveryLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputOfEveryMonthLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyLabel1);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyTheRadio);
            this.mmMonthlyRecurringPanel.Controls.Add(this.WeekcomboBox);
            this.mmMonthlyRecurringPanel.Controls.Add(this.DaycomboBox);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyOfTheEveryLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.inputOfEveryTheMonthLimiter);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyLabel2);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthlyAtLabel);
            this.mmMonthlyRecurringPanel.Controls.Add(this.mmMonthRecurringBegin);
            this.mmMonthlyRecurringPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mmMonthlyRecurringPanel.Location = new System.Drawing.Point(0, 0);
            this.mmMonthlyRecurringPanel.Name = "mmMonthlyRecurringPanel";
            this.mmMonthlyRecurringPanel.Size = new System.Drawing.Size(234, 268);
            this.mmMonthlyRecurringPanel.TabIndex = 42;
            this.mmMonthlyRecurringPanel.Visible = false;
            // 
            // propertiesHeaderStrip27
            // 
            this.propertiesHeaderStrip27.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip27.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip27.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip27.Name = "propertiesHeaderStrip27";
            this.propertiesHeaderStrip27.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip27.TabIndex = 55;
            this.propertiesHeaderStrip27.Text = "What time should this server be in maintenance mode?";
            this.propertiesHeaderStrip27.WordWrap = false;
            // 
            // informationBox15
            // 
            this.informationBox15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox15.Location = new System.Drawing.Point(10, 31);
            this.informationBox15.Name = "informationBox15";
            this.informationBox15.Size = new System.Drawing.Size(234, 32);
            this.informationBox15.TabIndex = 54;
            this.informationBox15.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.label5);
            this.flowLayoutPanel4.Controls.Add(this.mmRecurringDuration);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(43, 181);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(221, 31);
            this.flowLayoutPanel4.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 57;
            this.label5.Text = "Duration (hr):";
            // 
            // mmRecurringDuration
            // 
            dropDownEditorButton4.Key = "DropDownList";
            dropDownEditorButton4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmRecurringDuration.ButtonsRight.Add(dropDownEditorButton4);
            this.mmRecurringDuration.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.mmRecurringDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmRecurringDuration.FormatString = "HH:mm";
            this.mmRecurringDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmRecurringDuration.Location = new System.Drawing.Point(77, 3);
            this.mmRecurringDuration.MaskInput = "hh:mm";
            this.mmRecurringDuration.Name = "mmRecurringDuration";
            this.mmRecurringDuration.Size = new System.Drawing.Size(69, 21);
            this.mmRecurringDuration.TabIndex = 56;
            this.mmRecurringDuration.Time = System.TimeSpan.Parse("00:00:00");
            this.mmRecurringDuration.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label4);
            this.flowLayoutPanel3.Controls.Add(this.mmRecurringBegin);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(43, 106);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(221, 30);
            this.flowLayoutPanel3.TabIndex = 56;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 54;
            this.label4.Text = "Start time:";
            // 
            // mmRecurringBegin
            // 
            dropDownEditorButton5.Key = "DropDownList";
            dropDownEditorButton5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmRecurringBegin.ButtonsRight.Add(dropDownEditorButton5);
            this.mmRecurringBegin.DateTime = new System.DateTime(2008, 6, 30, 2, 0, 0, 0);
            this.mmRecurringBegin.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmRecurringBegin.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmRecurringBegin.Location = new System.Drawing.Point(63, 3);
            this.mmRecurringBegin.MaskInput = "{time}";
            this.mmRecurringBegin.Name = "mmRecurringBegin";
            this.mmRecurringBegin.Size = new System.Drawing.Size(90, 21);
            this.mmRecurringBegin.TabIndex = 53;
            this.mmRecurringBegin.Time = System.TimeSpan.Parse("02:00:00");
            this.mmRecurringBegin.Value = new System.DateTime(2008, 6, 30, 2, 0, 0, 0);
            // 
            // informationBox4
            // 
            this.informationBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox4.Location = new System.Drawing.Point(10, 31);
            this.informationBox4.Name = "informationBox4";
            this.informationBox4.Size = new System.Drawing.Size(221, 47);
            this.informationBox4.TabIndex = 53;
            this.informationBox4.Text = "Schedule times should be specified in the collection service time zone.";
            // 
            // mmBeginSatCheckbox
            // 
            this.mmBeginSatCheckbox.AutoSize = true;
            this.mmBeginSatCheckbox.Location = new System.Drawing.Point(364, 84);
            this.mmBeginSatCheckbox.Name = "mmBeginSatCheckbox";
            this.mmBeginSatCheckbox.Size = new System.Drawing.Size(42, 17);
            this.mmBeginSatCheckbox.TabIndex = 50;
            this.mmBeginSatCheckbox.Text = "Sat";
            this.mmBeginSatCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginFriCheckbox
            // 
            this.mmBeginFriCheckbox.AutoSize = true;
            this.mmBeginFriCheckbox.Checked = true;
            this.mmBeginFriCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginFriCheckbox.Location = new System.Drawing.Point(320, 84);
            this.mmBeginFriCheckbox.Name = "mmBeginFriCheckbox";
            this.mmBeginFriCheckbox.Size = new System.Drawing.Size(37, 17);
            this.mmBeginFriCheckbox.TabIndex = 49;
            this.mmBeginFriCheckbox.Text = "Fri";
            this.mmBeginFriCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginThurCheckbox
            // 
            this.mmBeginThurCheckbox.AutoSize = true;
            this.mmBeginThurCheckbox.Checked = true;
            this.mmBeginThurCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginThurCheckbox.Location = new System.Drawing.Point(268, 84);
            this.mmBeginThurCheckbox.Name = "mmBeginThurCheckbox";
            this.mmBeginThurCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginThurCheckbox.TabIndex = 48;
            this.mmBeginThurCheckbox.Text = "Thu";
            this.mmBeginThurCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginWedCheckbox
            // 
            this.mmBeginWedCheckbox.AutoSize = true;
            this.mmBeginWedCheckbox.Checked = true;
            this.mmBeginWedCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginWedCheckbox.Location = new System.Drawing.Point(212, 84);
            this.mmBeginWedCheckbox.Name = "mmBeginWedCheckbox";
            this.mmBeginWedCheckbox.Size = new System.Drawing.Size(49, 17);
            this.mmBeginWedCheckbox.TabIndex = 47;
            this.mmBeginWedCheckbox.Text = "Wed";
            this.mmBeginWedCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginTueCheckbox
            // 
            this.mmBeginTueCheckbox.AutoSize = true;
            this.mmBeginTueCheckbox.Checked = true;
            this.mmBeginTueCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginTueCheckbox.Location = new System.Drawing.Point(160, 84);
            this.mmBeginTueCheckbox.Name = "mmBeginTueCheckbox";
            this.mmBeginTueCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginTueCheckbox.TabIndex = 46;
            this.mmBeginTueCheckbox.Text = "Tue";
            this.mmBeginTueCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginMonCheckbox
            // 
            this.mmBeginMonCheckbox.AutoSize = true;
            this.mmBeginMonCheckbox.Checked = true;
            this.mmBeginMonCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mmBeginMonCheckbox.Location = new System.Drawing.Point(106, 84);
            this.mmBeginMonCheckbox.Name = "mmBeginMonCheckbox";
            this.mmBeginMonCheckbox.Size = new System.Drawing.Size(47, 17);
            this.mmBeginMonCheckbox.TabIndex = 45;
            this.mmBeginMonCheckbox.Text = "Mon";
            this.mmBeginMonCheckbox.UseVisualStyleBackColor = true;
            // 
            // mmBeginSunCheckbox
            // 
            this.mmBeginSunCheckbox.AutoSize = true;
            this.mmBeginSunCheckbox.Location = new System.Drawing.Point(54, 84);
            this.mmBeginSunCheckbox.Name = "mmBeginSunCheckbox";
            this.mmBeginSunCheckbox.Size = new System.Drawing.Size(45, 17);
            this.mmBeginSunCheckbox.TabIndex = 44;
            this.mmBeginSunCheckbox.Text = "Sun";
            this.mmBeginSunCheckbox.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip15
            // 
            this.propertiesHeaderStrip15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip15.Location = new System.Drawing.Point(3, 148);
            this.propertiesHeaderStrip15.Name = "propertiesHeaderStrip15";
            this.propertiesHeaderStrip15.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip15.TabIndex = 43;
            this.propertiesHeaderStrip15.Text = "How long should the server stay in maintenance mode?";
            this.propertiesHeaderStrip15.WordWrap = false;
            // 
            // propertiesHeaderStrip14
            // 
            this.propertiesHeaderStrip14.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesHeaderStrip14.Location = new System.Drawing.Point(0, 0);
            this.propertiesHeaderStrip14.Name = "propertiesHeaderStrip14";
            this.propertiesHeaderStrip14.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip14.TabIndex = 42;
            this.propertiesHeaderStrip14.Text = "What time should maintenance mode begin?";
            this.propertiesHeaderStrip14.WordWrap = false;
            // 
            // maintenanceModeContentPage
            // 
            this.maintenanceModeContentPage.BackColor = System.Drawing.Color.White;
            this.maintenanceModeContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.maintenanceModeContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.maintenanceModeContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.maintenanceModeContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenanceModeContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.maintenanceModeContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.maintenanceModeContentPage.ContentPanel.Name = "ContentPanel";
            this.maintenanceModeContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.maintenanceModeContentPage.ContentPanel.ShowBorder = false;
            this.maintenanceModeContentPage.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.maintenanceModeContentPage.ContentPanel.TabIndex = 1;
            this.maintenanceModeContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maintenanceModeContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.MaintenanceMode32x32;
            this.maintenanceModeContentPage.Location = new System.Drawing.Point(0, 0);
            this.maintenanceModeContentPage.Name = "maintenanceModeContentPage";
            this.maintenanceModeContentPage.Size = new System.Drawing.Size(251, 322);
            this.maintenanceModeContentPage.TabIndex = 0;
            this.maintenanceModeContentPage.Text = "Customize maintenance mode settings.";
            this.maintenanceModeContentPage.BackColor = backcolor;
            // 
            // osPropertyPage
            // 
            this.osPropertyPage.Controls.Add(this.osMetricsMainContainer);
            this.osPropertyPage.Controls.Add(this.osContentPage);
            this.osPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.osPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.osPropertyPage.Name = "osPropertyPage";
            this.osPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.osPropertyPage.TabIndex = 0;
            this.osPropertyPage.Text = "OS Metrics";
            this.osPropertyPage.AutoScroll = true;
            // 
            // osMetricsMainContainer
            // 
            this.osMetricsMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.osMetricsMainContainer.BackColor = System.Drawing.Color.White;
            this.osMetricsMainContainer.ColumnCount = 2;
            this.osMetricsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.osMetricsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.osMetricsMainContainer.Controls.Add(this.informationBox14, 0, 0);
            this.osMetricsMainContainer.Controls.Add(this.propertiesHeaderStrip18, 0, 1);
            this.osMetricsMainContainer.Controls.Add(this.optionWmiDirect, 1, 2);
            this.osMetricsMainContainer.Controls.Add(this.wmiCredentialsSecondaryContainer, 1, 3);
            this.osMetricsMainContainer.Controls.Add(this.optionWmiOleAutomation, 1, 4);
            this.osMetricsMainContainer.Controls.Add(this.lblOSMetricOLEWarningLabel, 1, 5);
            this.osMetricsMainContainer.Controls.Add(this.optionWmiNone, 1, 6);
            this.osMetricsMainContainer.Location = new System.Drawing.Point(5, 54);
            this.osMetricsMainContainer.Name = "osMetricsMainContainer";
            this.osMetricsMainContainer.RowCount = 7;
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.osMetricsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.osMetricsMainContainer.Size = new System.Drawing.Size(240, 400);
            this.osMetricsMainContainer.TabIndex = 2;
            // 
            // informationBox14
            // 
            this.osMetricsMainContainer.SetColumnSpan(this.informationBox14, 2);
            this.informationBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox14.Location = new System.Drawing.Point(3, 3);
            this.informationBox14.Name = "informationBox14";
            this.informationBox14.Size = new System.Drawing.Size(234, 70);
            this.informationBox14.TabIndex = 22;
            this.informationBox14.Text = resources.GetString("informationBox14.Text");
            // 
            // propertiesHeaderStrip18
            // 
            this.osMetricsMainContainer.SetColumnSpan(this.propertiesHeaderStrip18, 2);
            this.propertiesHeaderStrip18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip18.Location = new System.Drawing.Point(3, 79);
            this.propertiesHeaderStrip18.Name = "propertiesHeaderStrip18";
            this.propertiesHeaderStrip18.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip18.TabIndex = 20;
            this.propertiesHeaderStrip18.Text = "How would you like Operating System metrics to be collected?";
            this.propertiesHeaderStrip18.WordWrap = false;
            //  
            // optionWmiDirect
            // 
            this.optionWmiDirect.AutoSize = true;
            this.optionWmiDirect.Location = new System.Drawing.Point(38, 156);
            this.optionWmiDirect.Name = "optionWmiDirect";
            this.optionWmiDirect.Size = new System.Drawing.Size(199, 17);
            this.optionWmiDirect.TabIndex = 27;
            this.optionWmiDirect.Text = "Collect Operating System metrics using direct WMI";
            this.optionWmiDirect.UseVisualStyleBackColor = true;
            this.optionWmiDirect.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiOleAutomation
            // 
            this.optionWmiOleAutomation.AutoSize = true;
            this.optionWmiOleAutomation.Checked = true;
            this.optionWmiOleAutomation.Location = new System.Drawing.Point(38, 110);
            this.optionWmiOleAutomation.Name = "optionWmiOleAutomation";
            this.optionWmiOleAutomation.Size = new System.Drawing.Size(199, 17);
            this.optionWmiOleAutomation.TabIndex = 26;
            this.optionWmiOleAutomation.TabStop = true;
            this.optionWmiOleAutomation.Text = "Collect Operating System metrics using OLE automation";
            this.optionWmiOleAutomation.UseVisualStyleBackColor = true;
            this.optionWmiOleAutomation.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // optionWmiNone
            // 
            this.optionWmiNone.AutoSize = true;
            this.optionWmiNone.Location = new System.Drawing.Point(38, 110);
            this.optionWmiNone.Name = "optionWmiNone";
            this.optionWmiNone.Size = new System.Drawing.Size(199, 17);
            this.optionWmiNone.TabIndex = 25;
            this.optionWmiNone.Text = "Do not collect Operating System metrics";
            this.optionWmiNone.UseVisualStyleBackColor = true;
            this.optionWmiNone.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            //
            // wmiCredentialsSecondaryContainer
            // 
            this.wmiCredentialsSecondaryContainer.Controls.Add(this.wmiCredentialsContainer);
            this.wmiCredentialsSecondaryContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wmiCredentialsSecondaryContainer.Location = new System.Drawing.Point(38, 179);
            this.wmiCredentialsSecondaryContainer.Name = "wmiCredentialsSecondaryContainer";
            this.wmiCredentialsSecondaryContainer.Size = new System.Drawing.Size(199, 110);
            this.wmiCredentialsSecondaryContainer.TabIndex = 28;
            // 
            // wmiCredentialsContainer
            // 
            this.wmiCredentialsContainer.BackColor = System.Drawing.Color.Transparent;
            this.wmiCredentialsContainer.ColumnCount = 3;
            this.wmiCredentialsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.wmiCredentialsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.wmiCredentialsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.wmiCredentialsContainer.Controls.Add(this.optionWmiCSCreds, 1, 0);
            this.wmiCredentialsContainer.Controls.Add(this.label28, 1, 1);
            this.wmiCredentialsContainer.Controls.Add(this.directWmiUserName, 2, 1);
            this.wmiCredentialsContainer.Controls.Add(this.label24, 1, 2);
            this.wmiCredentialsContainer.Controls.Add(this.directWmiPassword, 2, 2);
            this.wmiCredentialsContainer.Controls.Add(this.wmiTestButton, 2, 3);
            this.wmiCredentialsContainer.Location = new System.Drawing.Point(0, 0);
            this.wmiCredentialsContainer.Name = "wmiCredentialsContainer";
            this.wmiCredentialsContainer.RowCount = 4;
            this.wmiCredentialsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.wmiCredentialsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.wmiCredentialsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.wmiCredentialsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.wmiCredentialsContainer.Size = new System.Drawing.Size(393, 153);
            this.wmiCredentialsContainer.TabIndex = 1;
            // 
            // optionWmiCSCreds
            // 
            this.optionWmiCSCreds.AutoSize = true;
            this.optionWmiCSCreds.Checked = true;
            this.optionWmiCSCreds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wmiCredentialsContainer.SetColumnSpan(this.optionWmiCSCreds, 2);
            this.optionWmiCSCreds.Location = new System.Drawing.Point(23, 3);
            this.optionWmiCSCreds.Name = "optionWmiCSCreds";
            this.optionWmiCSCreds.Size = new System.Drawing.Size(280, 17);
            this.optionWmiCSCreds.TabIndex = 23;
            this.optionWmiCSCreds.Text = "Connect using the SQLDM Collection Service account";
            this.optionWmiCSCreds.UseVisualStyleBackColor = true;
            this.optionWmiCSCreds.CheckedChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // label28
            // 
            this.label28.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label28.Location = new System.Drawing.Point(22, 29);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(65, 13);
            this.label28.TabIndex = 30;
            this.label28.Text = "Login name:";
            // 
            // directWmiUserName
            // 
            this.directWmiUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directWmiUserName.Enabled = false;
            this.directWmiUserName.Location = new System.Drawing.Point(97, 26);
            this.directWmiUserName.Margin = new System.Windows.Forms.Padding(10, 3, 0, 3);
            this.directWmiUserName.Name = "directWmiUserName";
            this.directWmiUserName.Size = new System.Drawing.Size(296, 20);
            this.directWmiUserName.TabIndex = 28;
            this.directWmiUserName.TextChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(22, 55);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 13);
            this.label24.TabIndex = 31;
            this.label24.Text = "Password:";
            // 
            // directWmiPassword
            // 
            this.directWmiPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directWmiPassword.Enabled = false;
            this.directWmiPassword.Location = new System.Drawing.Point(97, 52);
            this.directWmiPassword.Margin = new System.Windows.Forms.Padding(10, 3, 0, 3);
            this.directWmiPassword.Name = "directWmiPassword";
            this.directWmiPassword.Size = new System.Drawing.Size(296, 20);
            this.directWmiPassword.TabIndex = 29;
            this.directWmiPassword.UseSystemPasswordChar = true;
            this.directWmiPassword.TextChanged += new System.EventHandler(this.optionWmiChanged);
            // 
            // wmiTestButton
            // 
            this.wmiTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wmiTestButton.AutoSize = true;
            this.wmiTestButton.Enabled = false;
            this.wmiTestButton.Location = new System.Drawing.Point(315, 85);
            this.wmiTestButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.wmiTestButton.Name = "wmiTestButton";
            this.wmiTestButton.Size = new System.Drawing.Size(75, 23);
            this.wmiTestButton.TabIndex = 32;
            this.wmiTestButton.Text = "Test";
            this.wmiTestButton.UseVisualStyleBackColor = true;
            this.wmiTestButton.Visible = false;
            this.wmiTestButton.Click += new System.EventHandler(this.wmiTestButton_Click);
            //
            // lblOSMetricOLEWarningLabel
            // 
            this.lblOSMetricOLEWarningLabel.Location = new System.Drawing.Point(23, 3);
            this.lblOSMetricOLEWarningLabel.Name = "lblOSMetricOLEWarningLabel";
            this.lblOSMetricOLEWarningLabel.Size = new System.Drawing.Size(450, 27);
            this.lblOSMetricOLEWarningLabel.Visible = false;
            this.lblOSMetricOLEWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.lblOSMetricOLEWarningLabel.TabIndex = 33;
            this.lblOSMetricOLEWarningLabel.Text = "This setting requires that the SQLDM Collection Service Account be a " +
                "member of the sysadmin role on the target server.";
            // 
            // osContentPage
            // 
            this.osContentPage.BackColor = System.Drawing.Color.White;
            this.osContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.osContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.osContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.osContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.osContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.osContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.osContentPage.ContentPanel.Name = "ContentPanel";
            this.osContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.osContentPage.ContentPanel.ShowBorder = false;
            this.osContentPage.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.osContentPage.ContentPanel.TabIndex = 1;
            this.osContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.osContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.osContentPage.Location = new System.Drawing.Point(0, 0);
            this.osContentPage.Name = "osContentPage";
            this.osContentPage.Size = new System.Drawing.Size(251, 322);
            this.osContentPage.TabIndex = 0;
            this.osContentPage.Text = "Configure OS metrics collection";
            this.osContentPage.BackColor = backcolor;
            // 
            // diskPropertyPage
            // 
            this.diskPropertyPage.Controls.Add(this.diskDriversMainContainer);
            this.diskPropertyPage.Controls.Add(this.diskContentPage);
            this.diskPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.diskPropertyPage.Name = "diskPropertyPage";
            this.diskPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.diskPropertyPage.TabIndex = 0;
            this.diskPropertyPage.Text = "Disk Drives";
            this.diskPropertyPage.AutoScroll = true;
            // 
            // diskDriversMainContainer
            // 
            this.diskDriversMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.diskDriversMainContainer.BackColor = System.Drawing.Color.White;
            this.diskDriversMainContainer.ColumnCount = 1;
            this.diskDriversMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.diskDriversMainContainer.Controls.Add(this.tableLayoutPanel13, 0, 0);
            this.diskDriversMainContainer.Controls.Add(this.availableDisksLayoutPanel, 0, 1);
            this.diskDriversMainContainer.Location = new System.Drawing.Point(5, 54);
            this.diskDriversMainContainer.Name = "diskDriversMainContainer";
            this.diskDriversMainContainer.RowCount = 2;
            this.diskDriversMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.diskDriversMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.diskDriversMainContainer.Size = new System.Drawing.Size(240, 632);
            this.diskDriversMainContainer.TabIndex = 2;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.AutoSize = true;
            this.tableLayoutPanel13.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.Controls.Add(this.propertiesHeaderStrip20, 0, 3);
            this.tableLayoutPanel13.Controls.Add(this.propertiesHeaderStrip19, 0, 1);
            this.tableLayoutPanel13.Controls.Add(this.informationBox9, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.autoDiscoverDisksCheckBox, 1, 2);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 4;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel13.Size = new System.Drawing.Size(234, 225);
            this.tableLayoutPanel13.TabIndex = 1;
            // 
            // propertiesHeaderStrip20
            // 
            this.tableLayoutPanel13.SetColumnSpan(this.propertiesHeaderStrip20, 2);
            this.propertiesHeaderStrip20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip20.Location = new System.Drawing.Point(3, 197);
            this.propertiesHeaderStrip20.Name = "propertiesHeaderStrip20";
            this.propertiesHeaderStrip20.Size = new System.Drawing.Size(228, 25);
            this.propertiesHeaderStrip20.TabIndex = 22;
            this.propertiesHeaderStrip20.Text = "Would you like to specify the list of disk drives and mount points to collect sta" +
    "tistics for?";
            this.propertiesHeaderStrip20.WordWrap = false;
            // 
            // propertiesHeaderStrip19
            // 
            this.tableLayoutPanel13.SetColumnSpan(this.propertiesHeaderStrip19, 2);
            this.propertiesHeaderStrip19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip19.Location = new System.Drawing.Point(3, 143);
            this.propertiesHeaderStrip19.Name = "propertiesHeaderStrip19";
            this.propertiesHeaderStrip19.Size = new System.Drawing.Size(228, 25);
            this.propertiesHeaderStrip19.TabIndex = 20;
            this.propertiesHeaderStrip19.Text = "Would you like to automatically discover connected disks?";
            this.propertiesHeaderStrip19.WordWrap = false;
            // 
            // informationBox9
            // 
            this.tableLayoutPanel13.SetColumnSpan(this.informationBox9, 2);
            this.informationBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox9.Location = new System.Drawing.Point(3, 3);
            this.informationBox9.Name = "informationBox9";
            this.informationBox9.Size = new System.Drawing.Size(228, 134);
            this.informationBox9.TabIndex = 19;
            this.informationBox9.Text = resources.GetString("informationBox9.Text");
            // 
            // autoDiscoverDisksCheckBox
            // 
            this.autoDiscoverDisksCheckBox.AutoSize = true;
            this.autoDiscoverDisksCheckBox.Location = new System.Drawing.Point(23, 174);
            this.autoDiscoverDisksCheckBox.Name = "autoDiscoverDisksCheckBox";
            this.autoDiscoverDisksCheckBox.Size = new System.Drawing.Size(121, 17);
            this.autoDiscoverDisksCheckBox.TabIndex = 21;
            this.autoDiscoverDisksCheckBox.Text = "Discover disk drives";
            this.autoDiscoverDisksCheckBox.UseVisualStyleBackColor = true;
            this.autoDiscoverDisksCheckBox.CheckedChanged += new System.EventHandler(this.autoDiscoverDisksCheckBox_CheckedChanged);
            // 
            // availableDisksLayoutPanel
            // 
            this.availableDisksLayoutPanel.Controls.Add(this.panel1);
            this.availableDisksLayoutPanel.Controls.Add(this.availableDisksStackPanel);
            this.availableDisksLayoutPanel.Controls.Add(this.label15);
            this.availableDisksLayoutPanel.Controls.Add(this.selectedDisksListBox);
            this.availableDisksLayoutPanel.Controls.Add(this.label14);
            this.availableDisksLayoutPanel.Controls.Add(this.adhocDisksTextBox);
            this.availableDisksLayoutPanel.Controls.Add(this.label13);
            this.availableDisksLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableDisksLayoutPanel.ExpandToFitWidth = true;
            this.availableDisksLayoutPanel.Location = new System.Drawing.Point(3, 236);
            this.availableDisksLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.availableDisksLayoutPanel.Name = "availableDisksLayoutPanel";
            this.availableDisksLayoutPanel.Size = new System.Drawing.Size(234, 393);
            this.availableDisksLayoutPanel.TabIndex = 19;
            this.availableDisksLayoutPanel.BackColor = backColor;
            this.availableDisksLayoutPanel.ForeColor = foreColor;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.addDiskButton);
            this.panel1.Controls.Add(this.removeDiskButton);
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Vertical;
            gridBagConstraint1.OriginX = 1;
            gridBagConstraint1.OriginY = 2;
            gridBagConstraint1.SpanY = 2;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.panel1, gridBagConstraint1);
            this.panel1.Location = new System.Drawing.Point(69, 59);
            this.panel1.Name = "panel1";
            this.availableDisksLayoutPanel.SetPreferredSize(this.panel1, new System.Drawing.Size(98, 100));
            this.panel1.Size = new System.Drawing.Size(98, 329);
            this.panel1.TabIndex = 6;
            // 
            // addDiskButton
            // 
            //this.addDiskButton.BackColor = System.Drawing.SystemColors.Control;
            this.addDiskButton.Enabled = false;
            this.addDiskButton.Location = new System.Drawing.Point(4, 69);
            this.addDiskButton.Name = "addDiskButton";
            this.addDiskButton.Size = new System.Drawing.Size(90, 24);
            this.addDiskButton.TabIndex = 2;
            this.addDiskButton.Text = "Add >    ";
            this.addDiskButton.UseVisualStyleBackColor = true;
            this.addDiskButton.Click += new System.EventHandler(this.addDiskButton_Click);
            // 
            // removeDiskButton
            // 
            //this.removeDiskButton.BackColor = System.Drawing.SystemColors.Control;
            this.removeDiskButton.Enabled = false;
            this.removeDiskButton.Location = new System.Drawing.Point(4, 99);
            this.removeDiskButton.Name = "removeDiskButton";
            this.removeDiskButton.Size = new System.Drawing.Size(90, 24);
            this.removeDiskButton.TabIndex = 3;
            this.removeDiskButton.Text = "< Remove";
            this.removeDiskButton.UseVisualStyleBackColor = true;
            this.removeDiskButton.Click += new System.EventHandler(this.removeDiskButton_Click);
            // 
            // availableDisksStackPanel
            // 
            this.availableDisksStackPanel.ActiveControl = this.availableDisksMessageLabel;
            this.availableDisksStackPanel.Controls.Add(this.availableDisksMessageLabel);
            this.availableDisksStackPanel.Controls.Add(this.availableDisksListBox);
            gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint2.OriginX = 0;
            gridBagConstraint2.OriginY = 2;
            gridBagConstraint2.SpanY = 2;
            gridBagConstraint2.WeightX = 1F;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.availableDisksStackPanel, gridBagConstraint2);
            this.availableDisksStackPanel.Location = new System.Drawing.Point(0, 59);
            this.availableDisksStackPanel.Name = "availableDisksStackPanel";
            this.availableDisksLayoutPanel.SetPreferredSize(this.availableDisksStackPanel, new System.Drawing.Size(186, 147));
            this.availableDisksStackPanel.Size = new System.Drawing.Size(69, 329);
            this.availableDisksStackPanel.TabIndex = 10;
            // 
            // availableDisksMessageLabel
            // 
            this.availableDisksMessageLabel.BackColor = System.Drawing.Color.White;
            this.availableDisksMessageLabel.Enabled = false;
            this.availableDisksMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.availableDisksMessageLabel.Multiline = true;
            this.availableDisksMessageLabel.Name = "availableDisksMessageLabel";
            this.availableDisksMessageLabel.ReadOnly = true;
            this.availableDisksMessageLabel.Size = new System.Drawing.Size(69, 329);
            this.availableDisksMessageLabel.TabIndex = 5;
            this.availableDisksMessageLabel.Text = "Please wait...";
            this.availableDisksMessageLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // availableDisksListBox
            // 
            this.availableDisksListBox.FormattingEnabled = true;
            this.availableDisksListBox.Location = new System.Drawing.Point(0, 0);
            this.availableDisksListBox.Name = "availableDisksListBox";
            this.availableDisksListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableDisksListBox.Size = new System.Drawing.Size(230, 290);
            this.availableDisksListBox.Sorted = true;
            this.availableDisksListBox.TabIndex = 4;
            this.availableDisksListBox.SelectedIndexChanged += new System.EventHandler(this.availableDisksListBox_SelectedIndexChanged);
            this.availableDisksListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableDisksListBox_MouseDoubleClick);
            // 
            // label15
            // 
            gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint3.OriginX = 2;
            gridBagConstraint3.OriginY = 1;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.label15, gridBagConstraint3);
            this.label15.Location = new System.Drawing.Point(167, 27);
            this.label15.Name = "label15";
            this.availableDisksLayoutPanel.SetPreferredSize(this.label15, new System.Drawing.Size(184, 32));
            this.label15.Size = new System.Drawing.Size(67, 32);
            this.label15.TabIndex = 9;
            this.label15.Text = "Selected disk drives:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label15.ForeColor = foreColor;
            // 
            // selectedDisksListBox
            // 
            this.selectedDisksListBox.FormattingEnabled = true;
            gridBagConstraint4.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint4.OriginX = 2;
            gridBagConstraint4.OriginY = 2;
            gridBagConstraint4.SpanY = 2;
            gridBagConstraint4.WeightX = 1F;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.selectedDisksListBox, gridBagConstraint4);
            this.selectedDisksListBox.Location = new System.Drawing.Point(167, 59);
            this.selectedDisksListBox.Name = "selectedDisksListBox";
            this.availableDisksLayoutPanel.SetPreferredSize(this.selectedDisksListBox, new System.Drawing.Size(184, 329));
            this.selectedDisksListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedDisksListBox.Size = new System.Drawing.Size(67, 329);
            this.selectedDisksListBox.Sorted = true;
            this.selectedDisksListBox.TabIndex = 8;
            this.selectedDisksListBox.SelectedIndexChanged += new System.EventHandler(this.selectedDisksListBox_SelectedIndexChanged);
            this.selectedDisksListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.selectedDisksListBox_MouseDoubleClick);
            // 
            // label14
            // 
            gridBagConstraint5.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint5.OriginX = 0;
            gridBagConstraint5.OriginY = 0;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.label14, gridBagConstraint5);
            this.label14.Location = new System.Drawing.Point(0, 4);
            this.label14.Name = "label14";
            this.availableDisksLayoutPanel.SetPreferredSize(this.label14, new System.Drawing.Size(186, 23));
            this.label14.Size = new System.Drawing.Size(69, 23);
            this.label14.TabIndex = 7;
            this.label14.Text = "Available disk drives:";
            this.label14.ForeColor = foreColor;
            // 
            // adhocDisksTextBox
            // 
            this.adhocDisksTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            gridBagConstraint6.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint6.Insets.Bottom = 5;
            gridBagConstraint6.OriginX = 0;
            gridBagConstraint6.OriginY = 1;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.adhocDisksTextBox, gridBagConstraint6);
            this.adhocDisksTextBox.Location = new System.Drawing.Point(0, 27);
            this.adhocDisksTextBox.Name = "adhocDisksTextBox";
            this.availableDisksLayoutPanel.SetPreferredSize(this.adhocDisksTextBox, new System.Drawing.Size(186, 27));
            this.adhocDisksTextBox.Size = new System.Drawing.Size(69, 20);
            this.adhocDisksTextBox.TabIndex = 2;
            this.adhocDisksTextBox.Text = "< Type semicolon separated names >";
            this.adhocDisksTextBox.TextChanged += new System.EventHandler(this.adhocDisksTextBox_TextChanged);
            this.adhocDisksTextBox.Enter += new System.EventHandler(this.adhocDisksTextBox_Enter);
            this.adhocDisksTextBox.Leave += new System.EventHandler(this.adhocDisksTextBox_Leave);
            // 
            // label13
            // 
            gridBagConstraint7.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint7.OriginX = 0;
            gridBagConstraint7.OriginY = 0;
            this.availableDisksLayoutPanel.SetGridBagConstraint(this.label13, gridBagConstraint7);
            this.label13.Location = new System.Drawing.Point(0, 4);
            this.label13.Name = "label13";
            this.availableDisksLayoutPanel.SetPreferredSize(this.label13, new System.Drawing.Size(186, 23));
            this.label13.Size = new System.Drawing.Size(69, 23);
            this.label13.TabIndex = 1;
            this.label13.Text = "Available Tables:";
            // 
            // diskContentPage
            // 
            this.diskContentPage.BackColor = System.Drawing.Color.White;
            this.diskContentPage.BorderWidth = 1;
            // 
            // 
            // 
            this.diskContentPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.diskContentPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.diskContentPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskContentPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.diskContentPage.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.diskContentPage.ContentPanel.Name = "ContentPanel";
            this.diskContentPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.diskContentPage.ContentPanel.ShowBorder = false;
            this.diskContentPage.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.diskContentPage.ContentPanel.TabIndex = 1;
            this.diskContentPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskContentPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.diskContentPage.Location = new System.Drawing.Point(0, 0);
            this.diskContentPage.Name = "diskContentPage";
            this.diskContentPage.Size = new System.Drawing.Size(251, 322);
            this.diskContentPage.TabIndex = 0;
            this.diskContentPage.Text = "Customize disk drive statistics collection.";
            this.diskContentPage.BackColor = backcolor;
            // 
            // clusterPropertyPage
            // 
            this.clusterPropertyPage.Controls.Add(this.clusterSettingsMainContainer);
            this.clusterPropertyPage.Controls.Add(this.office2007PropertyPage1);
            this.clusterPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clusterPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.clusterPropertyPage.Name = "clusterPropertyPage";
            this.clusterPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.clusterPropertyPage.TabIndex = 0;
            this.clusterPropertyPage.Text = "Cluster Settings";
            this.clusterPropertyPage.AutoScroll = true;
            // 
            // clusterSettingsMainContainer
            // 
            this.clusterSettingsMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clusterSettingsMainContainer.BackColor = System.Drawing.Color.White;
            this.clusterSettingsMainContainer.ColumnCount = 1;
            this.clusterSettingsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.clusterSettingsMainContainer.Controls.Add(this.flowLayoutPanel5, 0, 2);
            this.clusterSettingsMainContainer.Controls.Add(this.propertiesHeaderStrip21, 0, 0);
            this.clusterSettingsMainContainer.Controls.Add(this.informationBox10, 0, 1);
            this.clusterSettingsMainContainer.Location = new System.Drawing.Point(5, 54);
            this.clusterSettingsMainContainer.Name = "clusterSettingsMainContainer";
            this.clusterSettingsMainContainer.RowCount = 3;
            this.clusterSettingsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.clusterSettingsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.clusterSettingsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.clusterSettingsMainContainer.Size = new System.Drawing.Size(240, 367);
            this.clusterSettingsMainContainer.TabIndex = 2;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel5.Controls.Add(this.label16);
            this.flowLayoutPanel5.Controls.Add(this.preferredNode);
            this.flowLayoutPanel5.Controls.Add(this.setCurrentNode);
            this.flowLayoutPanel5.Controls.Add(this.clusterWarningLabel);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 109);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(234, 255);
            this.flowLayoutPanel5.TabIndex = 26;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(117, 13);
            this.label16.TabIndex = 33;
            this.label16.Text = "Preferred Cluster Node:";
            //
            //clusterWarning
            //
            this.clusterWarningLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clusterWarningLabel.AutoSize = true;
            this.clusterWarningLabel.Location = new System.Drawing.Point(5, 200);
            this.clusterWarningLabel.Name = "Cluster Warning";
            this.clusterWarningLabel.Size = new System.Drawing.Size(150, 30);
            this.clusterWarningLabel.TabIndex = 33;
            this.clusterWarningLabel.Text = "Monitored Server is not part of a cluster.";
            this.clusterWarningLabel.Visible = false;

            // 
            // preferredNode
            // 
            this.preferredNode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.preferredNode.Location = new System.Drawing.Point(3, 16);
            this.preferredNode.Name = "preferredNode";
            this.preferredNode.Size = new System.Drawing.Size(146, 20);
            this.preferredNode.TabIndex = 34;
            // 
            // setCurrentNode
            // 
            this.setCurrentNode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.setCurrentNode.AutoSize = true;
            //this.setCurrentNode.BackColor = System.Drawing.SystemColors.Control;
            //////this.setCurrentNode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.setCurrentNode.Location = new System.Drawing.Point(3, 42);
            this.setCurrentNode.Name = "setCurrentNode";
            this.setCurrentNode.Size = new System.Drawing.Size(104, 23);
            this.setCurrentNode.TabIndex = 35;
            this.setCurrentNode.Text = "Current Node";
            this.setCurrentNode.UseVisualStyleBackColor = true;
            this.setCurrentNode.Click += new System.EventHandler(this.setCurrentNode_Click);
            // 
            // propertiesHeaderStrip21
            // 
            this.propertiesHeaderStrip21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip21.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip21.Name = "propertiesHeaderStrip21";
            this.propertiesHeaderStrip21.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip21.TabIndex = 24;
            this.propertiesHeaderStrip21.Text = "What is the preferred cluster node?";
            this.propertiesHeaderStrip21.WordWrap = false;
            // 
            // informationBox10
            // 
            this.informationBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox10.Location = new System.Drawing.Point(3, 34);
            this.informationBox10.Name = "informationBox10";
            this.informationBox10.Size = new System.Drawing.Size(234, 69);
            this.informationBox10.TabIndex = 25;
            this.informationBox10.Text = resources.GetString("informationBox10.Text");
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(251, 322);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Configure cluster-specific settings";
            this.office2007PropertyPage1.BackColor = backcolor;
            // 
            // waitPropertyPage
            // 
            this.waitPropertyPage.Controls.Add(this.waitMonitoringMainContainer);
            this.waitPropertyPage.Controls.Add(this.office2007PropertyPage3);
            this.waitPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waitPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.waitPropertyPage.Name = "waitPropertyPage";
            this.waitPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.waitPropertyPage.TabIndex = 0;
            this.waitPropertyPage.Text = "Wait Monitoring";
            this.waitPropertyPage.AutoScroll = true;
            // 
            // waitMonitoringMainContainer
            // 
            this.waitMonitoringMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waitMonitoringMainContainer.BackColor = System.Drawing.Color.White;
            this.waitMonitoringMainContainer.ColumnCount = 2;
            this.waitMonitoringMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.waitMonitoringMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.waitMonitoringMainContainer.Controls.Add(this.collectStatisticsSecondaryContainer, 1, 7);
            this.waitMonitoringMainContainer.Controls.Add(this.informationBox12, 0, 6);
            this.waitMonitoringMainContainer.Controls.Add(this.propertiesHeaderStrip23, 0, 5);
            this.waitMonitoringMainContainer.Controls.Add(this.flowLayoutPanel6, 1, 4);
            this.waitMonitoringMainContainer.Controls.Add(this.propertiesHeaderStrip25, 0, 3);
            this.waitMonitoringMainContainer.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.waitMonitoringMainContainer.Controls.Add(this.informationBox13, 0, 1);
            this.waitMonitoringMainContainer.Controls.Add(this.propertiesHeaderStrip24, 0, 0);
            this.waitMonitoringMainContainer.Location = new System.Drawing.Point(5, 54);
            this.waitMonitoringMainContainer.Name = "waitMonitoringMainContainer";
            this.waitMonitoringMainContainer.RowCount = 8;
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.waitMonitoringMainContainer.Size = new System.Drawing.Size(240, 580);
            this.waitMonitoringMainContainer.TabIndex = 5;
            // 
            // collectStatisticsSecondaryContainer
            // 
            this.collectStatisticsSecondaryContainer.AutoSize = true;
            this.collectStatisticsSecondaryContainer.BackColor = System.Drawing.Color.White;
            this.collectStatisticsSecondaryContainer.ColumnCount = 3;
            this.collectStatisticsSecondaryContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.collectStatisticsSecondaryContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.collectStatisticsSecondaryContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.collectStatisticsSecondaryContainer.Controls.Add(this.chkUseXE, 0, 4);
            this.collectStatisticsSecondaryContainer.Controls.Add(this.chkUseQs, 0, 5);
            // SQLdm Minimum Privileges - Varun Chopra - set warning label
            this.collectStatisticsSecondaryContainer.Controls.Add(this.lblWaitMonitoringWarningLabel, 0, 6);
            this.collectStatisticsSecondaryContainer.Controls.Add(this.chkCollectActiveWaits, 0, 0);
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            this.collectStatisticsSecondaryContainer.Controls.Add(this.ScheduledQueryWaits, 1, 1);
            this.collectStatisticsSecondaryContainer.Controls.Add(this.tableLayoutPanel3, 2, 2);
            this.collectStatisticsSecondaryContainer.Controls.Add(this.PerpetualQueryWaits, 1, 3);
            this.collectStatisticsSecondaryContainer.Location = new System.Drawing.Point(23, 307);
            this.collectStatisticsSecondaryContainer.Name = "collectStatisticsSecondaryContainer";
            // SQLdm Minimum Privileges - Varun Chopra - update row count
            this.collectStatisticsSecondaryContainer.RowCount = 7;
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.11111F));
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.88889F));
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // SQLdm Minimum Privileges - Varun Chopra - add styles for warning label
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            this.collectStatisticsSecondaryContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.collectStatisticsSecondaryContainer.Size = new System.Drawing.Size(440, 177);
            this.collectStatisticsSecondaryContainer.TabIndex = 104;
            // 
            // chkUseXE
            // 
            this.chkUseXE.AutoSize = true;
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.chkUseXE, 3);
            this.chkUseXE.Location = new System.Drawing.Point(3, 156);
            this.chkUseXE.Name = "chkUseXE";
            this.chkUseXE.Size = new System.Drawing.Size(214, 17);
            this.chkUseXE.TabIndex = 103;
            this.chkUseXE.Text = "Use Extended Events (SQL 2012+ only)";
            this.chkUseXE.UseVisualStyleBackColor = true;
            this.chkUseXE.CheckedChanged += new System.EventHandler(this.chkUseXE_CheckChanged);
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            // 
            // chkUseQs
            // 
            this.chkUseQs.AutoSize = true;
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.chkUseQs, 3);
            this.chkUseQs.Location = new System.Drawing.Point(3, 156);
            this.chkUseQs.Name = "chkUseQs";
            this.chkUseQs.Size = new System.Drawing.Size(214, 17);
            this.chkUseQs.TabIndex = 104;
            this.chkUseQs.Text = "Use Query Store (SQL 2017+ only)";
            this.chkUseQs.UseVisualStyleBackColor = true;
            this.chkUseQs.CheckedChanged += new System.EventHandler(this.chkUseQs_CheckChanged);

            // 
            // lblWaitMonitoringWarningLabel
            // 
            // SQLdm Minimum Privileges - Varun Chopra - add styles for warning label
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.lblWaitMonitoringWarningLabel, 3);
            this.lblWaitMonitoringWarningLabel.Location = new System.Drawing.Point(3, 213);
            this.lblWaitMonitoringWarningLabel.Name = "lblWaitMonitoringWarningLabel";
            this.lblWaitMonitoringWarningLabel.Size = new System.Drawing.Size(450, 48);
            this.lblWaitMonitoringWarningLabel.Visible = false;
            this.lblWaitMonitoringWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.lblWaitMonitoringWarningLabel.TabIndex = 105;
            this.lblWaitMonitoringWarningLabel.Text = string.Empty;
            // 
            // lblQueryMonitoringWarningLabel
            // 
            // SQLdm Minimum Privileges - add styles for warning label
            this.tableLayoutPanel17.SetColumnSpan(this.lblQueryMonitoringWarningLabel, 3);
            this.lblQueryMonitoringWarningLabel.Location = new System.Drawing.Point(3, 213);
            this.lblQueryMonitoringWarningLabel.Name = "lblQueryMonitoringWarningLabel";
            this.lblQueryMonitoringWarningLabel.Size = new System.Drawing.Size(450, 48);
            this.lblQueryMonitoringWarningLabel.Visible = true;
            this.lblQueryMonitoringWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.lblQueryMonitoringWarningLabel.TabIndex = 105;
            this.lblQueryMonitoringWarningLabel.Text = "Warning: The SQL Diagnostic Manager monitoring account does not have the necessary " +
                "permissions to collect this data.";

            // 
            // chkCollectActiveWaits
            // 
            this.chkCollectActiveWaits.AutoSize = true;
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.chkCollectActiveWaits, 3);
            this.chkCollectActiveWaits.Location = new System.Drawing.Point(3, 3);
            this.chkCollectActiveWaits.Name = "chkCollectActiveWaits";
            this.chkCollectActiveWaits.Size = new System.Drawing.Size(237, 17);
            this.chkCollectActiveWaits.TabIndex = 96;
            this.chkCollectActiveWaits.Text = "Collect query wait statistics (SQL 2005+ only)";
            this.chkCollectActiveWaits.UseVisualStyleBackColor = true;
            this.chkCollectActiveWaits.CheckedChanged += new System.EventHandler(this.chkCollectActiveWaits_CheckedChanged);
            // 
            // ScheduledQueryWaits
            // 
            this.ScheduledQueryWaits.AutoSize = true;
            this.ScheduledQueryWaits.Checked = true;
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.ScheduledQueryWaits, 2);
            this.ScheduledQueryWaits.Location = new System.Drawing.Point(23, 27);
            this.ScheduledQueryWaits.Name = "ScheduledQueryWaits";
            this.ScheduledQueryWaits.Size = new System.Drawing.Size(207, 16);
            this.ScheduledQueryWaits.TabIndex = 97;
            this.ScheduledQueryWaits.TabStop = true;
            this.ScheduledQueryWaits.Text = "Collect at a specified time and duration";
            this.ScheduledQueryWaits.UseVisualStyleBackColor = true;
            this.ScheduledQueryWaits.CheckedChanged += new System.EventHandler(this.ScheduledQueryWaits_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label19, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.waitStatisticsServerDateTime, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label17, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.waitStatisticsStartDate, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.waitStatisticsStartTime, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.waitStatisticsRunTime, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(43, 49);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(353, 74);
            this.tableLayoutPanel3.TabIndex = 98;
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(67, 13);
            this.label19.TabIndex = 93;
            this.label19.Text = "Server Time:";
            // 
            // waitStatisticsServerDateTime
            // 
            this.waitStatisticsServerDateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.waitStatisticsServerDateTime.AutoSize = true;
            this.waitStatisticsServerDateTime.Location = new System.Drawing.Point(99, 0);
            this.waitStatisticsServerDateTime.Name = "waitStatisticsServerDateTime";
            this.waitStatisticsServerDateTime.Size = new System.Drawing.Size(67, 13);
            this.waitStatisticsServerDateTime.TabIndex = 95;
            this.waitStatisticsServerDateTime.Text = "Refreshing...";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 22);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(58, 13);
            this.label17.TabIndex = 96;
            this.label17.Text = "Start Time:";
            // 
            // waitStatisticsStartDate
            // 
            this.waitStatisticsStartDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.waitStatisticsStartDate.DateTime = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            this.waitStatisticsStartDate.Enabled = false;
            this.waitStatisticsStartDate.Location = new System.Drawing.Point(99, 18);
            this.waitStatisticsStartDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.waitStatisticsStartDate.Name = "waitStatisticsStartDate";
            this.waitStatisticsStartDate.Size = new System.Drawing.Size(96, 21);
            this.waitStatisticsStartDate.StyleSetName = "78";
            this.waitStatisticsStartDate.TabIndex = 97;
            this.waitStatisticsStartDate.Value = new System.DateTime(2009, 4, 6, 0, 0, 0, 0);
            // 
            // waitStatisticsStartTime
            // 
            this.waitStatisticsStartTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton6.Key = "DropDownList";
            dropDownEditorButton6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.waitStatisticsStartTime.ButtonsRight.Add(dropDownEditorButton6);
            this.waitStatisticsStartTime.DateTime = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.waitStatisticsStartTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.waitStatisticsStartTime.Enabled = false;
            this.waitStatisticsStartTime.FormatString = "";
            this.waitStatisticsStartTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.waitStatisticsStartTime.Location = new System.Drawing.Point(201, 18);
            this.waitStatisticsStartTime.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.waitStatisticsStartTime.MaskInput = "{time}";
            this.waitStatisticsStartTime.Name = "waitStatisticsStartTime";
            this.waitStatisticsStartTime.Size = new System.Drawing.Size(81, 21);
            this.waitStatisticsStartTime.TabIndex = 99;
            this.waitStatisticsStartTime.Time = System.TimeSpan.Parse("00:00:00");
            this.waitStatisticsStartTime.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // waitStatisticsRunTime
            // 
            this.waitStatisticsRunTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton7.Key = "DropDownList";
            dropDownEditorButton7.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.waitStatisticsRunTime.ButtonsRight.Add(dropDownEditorButton7);
            this.waitStatisticsRunTime.DateTime = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            this.waitStatisticsRunTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.waitStatisticsRunTime.FormatString = "HH:mm";
            this.waitStatisticsRunTime.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.waitStatisticsRunTime.Location = new System.Drawing.Point(99, 46);
            this.waitStatisticsRunTime.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.waitStatisticsRunTime.MaskInput = "hh:mm";
            this.waitStatisticsRunTime.Name = "waitStatisticsRunTime";
            this.waitStatisticsRunTime.Size = new System.Drawing.Size(96, 21);
            this.waitStatisticsRunTime.TabIndex = 101;
            this.waitStatisticsRunTime.Time = System.TimeSpan.Parse("00:00:00");
            this.waitStatisticsRunTime.Value = new System.DateTime(2009, 2, 11, 0, 0, 0, 0);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 100;
            this.label8.Text = "Duration (hh:mm):";
            // 
            // PerpetualQueryWaits
            // 
            this.PerpetualQueryWaits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.PerpetualQueryWaits.AutoSize = true;
            this.collectStatisticsSecondaryContainer.SetColumnSpan(this.PerpetualQueryWaits, 2);
            this.PerpetualQueryWaits.Location = new System.Drawing.Point(23, 131);
            this.PerpetualQueryWaits.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.PerpetualQueryWaits.Name = "PerpetualQueryWaits";
            this.PerpetualQueryWaits.Size = new System.Drawing.Size(109, 17);
            this.PerpetualQueryWaits.TabIndex = 99;
            this.PerpetualQueryWaits.Text = "Collect indefinitely";
            this.PerpetualQueryWaits.UseVisualStyleBackColor = true;
            this.PerpetualQueryWaits.CheckedChanged += new System.EventHandler(this.PerpetualQueryWaits_CheckedChanged);
            // 
            // informationBox12
            // 
            this.waitMonitoringMainContainer.SetColumnSpan(this.informationBox12, 2);
            this.informationBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox12.Location = new System.Drawing.Point(3, 237);
            this.informationBox12.Name = "informationBox12";
            this.informationBox12.Size = new System.Drawing.Size(495, 64);
            this.informationBox12.TabIndex = 103;
            this.informationBox12.Text = resources.GetString("informationBox12.Text");
            // 
            // propertiesHeaderStrip23
            // 
            this.waitMonitoringMainContainer.SetColumnSpan(this.propertiesHeaderStrip23, 2);
            this.propertiesHeaderStrip23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip23.Location = new System.Drawing.Point(3, 206);
            this.propertiesHeaderStrip23.Name = "propertiesHeaderStrip23";
            this.propertiesHeaderStrip23.Size = new System.Drawing.Size(495, 25);
            this.propertiesHeaderStrip23.TabIndex = 102;
            this.propertiesHeaderStrip23.Text = "Collect query-level wait statistics historically?";
            this.propertiesHeaderStrip23.WordWrap = false;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel6.Controls.Add(this.label20);
            this.flowLayoutPanel6.Controls.Add(this.waitStatisticsFilterButton);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(23, 167);
            this.flowLayoutPanel6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(475, 31);
            this.flowLayoutPanel6.TabIndex = 101;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 9);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(278, 13);
            this.label20.TabIndex = 103;
            this.label20.Text = "Filters apply to both scheduled and on-demand collection.";
            // 
            // waitStatisticsFilterButton
            // 
            this.waitStatisticsFilterButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.waitStatisticsFilterButton.BackColor = System.Drawing.SystemColors.Control;
            this.waitStatisticsFilterButton.Location = new System.Drawing.Point(287, 3);
            this.waitStatisticsFilterButton.Name = "waitStatisticsFilterButton";
            this.waitStatisticsFilterButton.Size = new System.Drawing.Size(100, 25);
            this.waitStatisticsFilterButton.TabIndex = 102;
            this.waitStatisticsFilterButton.Text = "Filter Options...";
            this.waitStatisticsFilterButton.UseVisualStyleBackColor = false;
            this.waitStatisticsFilterButton.Click += new System.EventHandler(this.waitStatisticsFilterButton_Click);
            // 
            // propertiesHeaderStrip25
            // 
            this.waitMonitoringMainContainer.SetColumnSpan(this.propertiesHeaderStrip25, 2);
            this.propertiesHeaderStrip25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip25.Location = new System.Drawing.Point(3, 134);
            this.propertiesHeaderStrip25.Name = "propertiesHeaderStrip25";
            this.propertiesHeaderStrip25.Size = new System.Drawing.Size(495, 25);
            this.propertiesHeaderStrip25.TabIndex = 100;
            this.propertiesHeaderStrip25.Text = "What data should be excluded from query wait collection?";
            this.propertiesHeaderStrip25.WordWrap = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.waitStatisticsStatus, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.stopWaitCollector, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.refreshWaitCollectorStatus, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(23, 95);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(475, 31);
            this.tableLayoutPanel2.TabIndex = 89;
            // 
            // waitStatisticsStatus
            // 
            this.waitStatisticsStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.waitStatisticsStatus.AutoSize = true;
            this.waitStatisticsStatus.Location = new System.Drawing.Point(178, 9);
            this.waitStatisticsStatus.Name = "waitStatisticsStatus";
            this.waitStatisticsStatus.Size = new System.Drawing.Size(53, 13);
            this.waitStatisticsStatus.TabIndex = 90;
            this.waitStatisticsStatus.Text = "Unknown";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 9);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(169, 13);
            this.label18.TabIndex = 89;
            this.label18.Text = "Query-Level Wait Collector Status:";
            // 
            // stopWaitCollector
            // 
            this.stopWaitCollector.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.stopWaitCollector.BackColor = System.Drawing.SystemColors.Control;
            this.stopWaitCollector.Location = new System.Drawing.Point(257, 3);
            this.stopWaitCollector.Name = "stopWaitCollector";
            this.stopWaitCollector.Size = new System.Drawing.Size(100, 25);
            this.stopWaitCollector.TabIndex = 91;
            this.stopWaitCollector.Text = "Stop";
            this.stopWaitCollector.UseVisualStyleBackColor = false;
            this.stopWaitCollector.Visible = false;
            this.stopWaitCollector.Click += new System.EventHandler(this.stopWaitCollector_Click);
            // 
            // refreshWaitCollectorStatus
            // 
            this.refreshWaitCollectorStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.refreshWaitCollectorStatus.BackColor = System.Drawing.SystemColors.Control;
            this.refreshWaitCollectorStatus.Location = new System.Drawing.Point(363, 3);
            this.refreshWaitCollectorStatus.Name = "refreshWaitCollectorStatus";
            this.refreshWaitCollectorStatus.Size = new System.Drawing.Size(100, 25);
            this.refreshWaitCollectorStatus.TabIndex = 92;
            this.refreshWaitCollectorStatus.Text = "Refresh";
            this.refreshWaitCollectorStatus.UseVisualStyleBackColor = false;
            this.refreshWaitCollectorStatus.Click += new System.EventHandler(this.refreshWaitCollectorStatus_Click);
            // 
            // informationBox13
            // 
            this.informationBox13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waitMonitoringMainContainer.SetColumnSpan(this.informationBox13, 2);
            this.informationBox13.Location = new System.Drawing.Point(3, 34);
            this.informationBox13.Name = "informationBox13";
            this.informationBox13.Size = new System.Drawing.Size(495, 53);
            this.informationBox13.TabIndex = 88;
            this.informationBox13.Text = resources.GetString("informationBox13.Text");
            // 
            // propertiesHeaderStrip24
            // 
            this.waitMonitoringMainContainer.SetColumnSpan(this.propertiesHeaderStrip24, 2);
            this.propertiesHeaderStrip24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip24.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip24.Name = "propertiesHeaderStrip24";
            this.propertiesHeaderStrip24.Size = new System.Drawing.Size(495, 25);
            this.propertiesHeaderStrip24.TabIndex = 87;
            this.propertiesHeaderStrip24.Text = "What is the current status of the query-level wait collector?";
            this.propertiesHeaderStrip24.WordWrap = false;
            // 
            // office2007PropertyPage3
            // 
            this.office2007PropertyPage3.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage3.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage3.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage3.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage3.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage3.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage3.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage3.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage3.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage3.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage3.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.office2007PropertyPage3.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage3.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage3.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage3.Name = "office2007PropertyPage3";
            this.office2007PropertyPage3.Size = new System.Drawing.Size(251, 322);
            this.office2007PropertyPage3.TabIndex = 0;
            this.office2007PropertyPage3.Text = "Configure Wait Monitoring";
            this.office2007PropertyPage3.BackColor = backcolor;
            // 
            // virtualizationPropertyPage
            // 
            this.virtualizationPropertyPage.AutoSize = true;
            this.virtualizationPropertyPage.Controls.Add(this.virtualizationMainContainer);
            this.virtualizationPropertyPage.Controls.Add(this.office2007PropertyPage5);
            this.virtualizationPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.virtualizationPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.virtualizationPropertyPage.Name = "virtualizationPropertyPage";
            this.virtualizationPropertyPage.Size = new System.Drawing.Size(251, 322);
            this.virtualizationPropertyPage.TabIndex = 0;
            this.virtualizationPropertyPage.Text = "Virtualization";
            this.virtualizationPropertyPage.AutoScroll = true;
            // 
            // virtualizationMainContainer
            // 
            this.virtualizationMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.virtualizationMainContainer.BackColor = System.Drawing.Color.White;
            this.virtualizationMainContainer.ColumnCount = 2;
            this.virtualizationMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.virtualizationMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.virtualizationMainContainer.Controls.Add(this.propertiesHeaderStrip26, 0, 0);
            this.virtualizationMainContainer.Controls.Add(this.informationBox11, 0, 1);
            this.virtualizationMainContainer.Controls.Add(this.tableLayoutPanel5, 1, 2);
            this.virtualizationMainContainer.Location = new System.Drawing.Point(5, 54);
            this.virtualizationMainContainer.Name = "virtualizationMainContainer";
            this.virtualizationMainContainer.RowCount = 3;
            this.virtualizationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.virtualizationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.virtualizationMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.virtualizationMainContainer.Size = new System.Drawing.Size(240, 400);
            this.virtualizationMainContainer.TabIndex = 2;
            // 
            // propertiesHeaderStrip26
            // 
            this.virtualizationMainContainer.SetColumnSpan(this.propertiesHeaderStrip26, 2);
            this.propertiesHeaderStrip26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStrip26.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStrip26.Name = "propertiesHeaderStrip26";
            this.propertiesHeaderStrip26.Size = new System.Drawing.Size(234, 25);
            this.propertiesHeaderStrip26.TabIndex = 87;
            this.propertiesHeaderStrip26.Text = "Collection statistics for Virtual Machine and Virtual Machine Host?";
            this.propertiesHeaderStrip26.WordWrap = false;
            // 
            // informationBox11
            // 
            this.virtualizationMainContainer.SetColumnSpan(this.informationBox11, 2);
            this.informationBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.informationBox11.Location = new System.Drawing.Point(3, 34);
            this.informationBox11.Name = "informationBox11";
            this.informationBox11.Size = new System.Drawing.Size(234, 90);
            this.informationBox11.TabIndex = 0;
            this.informationBox11.Text = resources.GetString("informationBox11.Text");
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.btnVMConfig, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label26, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.vCenterNameLabel, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label27, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.vmNameLabel, 1, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(53, 130);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(450, 267);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // btnVMConfig
            // 
            this.btnVMConfig.AutoEllipsis = true;
            this.btnVMConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel5.SetColumnSpan(this.btnVMConfig, 2);
            this.btnVMConfig.Location = new System.Drawing.Point(3, 65);
            this.btnVMConfig.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.btnVMConfig.Name = "btnVMConfig";
            this.btnVMConfig.Size = new System.Drawing.Size(130, 25);
            this.btnVMConfig.TabIndex = 99;
            this.btnVMConfig.Text = "VM Configuration";
            this.btnVMConfig.UseVisualStyleBackColor = false;
            this.btnVMConfig.Click += new System.EventHandler(this.btnVMConfig_Click);
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(3, 8);
            this.label26.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(81, 13);
            this.label26.TabIndex = 89;
            this.label26.Text = "Virtualization Host Server:";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vCenterNameLabel
            // 
            this.vCenterNameLabel.AutoEllipsis = true;
            this.vCenterNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vCenterNameLabel.Location = new System.Drawing.Point(92, 5);
            this.vCenterNameLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.vCenterNameLabel.Name = "vCenterNameLabel";
            this.vCenterNameLabel.Size = new System.Drawing.Size(89, 20);
            this.vCenterNameLabel.TabIndex = 96;
            this.vCenterNameLabel.Text = "Not Connected";
            this.vCenterNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(3, 38);
            this.label27.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(83, 13);
            this.label27.TabIndex = 97;
            this.label27.Text = "Virtual Machine:";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vmNameLabel
            // 
            this.vmNameLabel.AutoEllipsis = true;
            this.vmNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vmNameLabel.Location = new System.Drawing.Point(92, 35);
            this.vmNameLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.vmNameLabel.Name = "vmNameLabel";
            this.vmNameLabel.Size = new System.Drawing.Size(89, 25);
            this.vmNameLabel.TabIndex = 98;
            this.vmNameLabel.Text = "VM";
            this.vmNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // office2007PropertyPage5
            // 
            this.office2007PropertyPage5.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage5.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage5.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage5.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage5.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage5.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage5.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage5.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage5.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage5.ContentPanel.Size = new System.Drawing.Size(249, 265);
            this.office2007PropertyPage5.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage5.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage5.Name = "office2007PropertyPage5";
            this.office2007PropertyPage5.Size = new System.Drawing.Size(251, 322);
            this.office2007PropertyPage5.TabIndex = 0;
            this.office2007PropertyPage5.Text = "Configure Virtualization Properties";
            this.office2007PropertyPage5.BackColor = backcolor;
            // 
            // _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left
            // 
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.BackColor = backColor;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.ForeColor = foreColor;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.Name = "_MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left";
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 742);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarManager;
            // 
            // _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right
            // 
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(687, 0);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.Name = "_MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right";
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 742);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarManager;
            // 
            // _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top
            // 
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.Name = "_MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top";
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(687, 0);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarManager;
            // 
            // _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom
            // 
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 742);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.Name = "_MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom";
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(687, 0);
            this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarManager;
            // 
            // ultraGridBagLayoutPanel1
            // 
            this.ultraGridBagLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGridBagLayoutPanel1.Controls.Add(this.availableTablesStackPanel);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.databaseComboBox);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.panel5);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label10);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.selectedTablesListBox);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label11);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label12);
            this.ultraGridBagLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraGridBagLayoutPanel1.Name = "ultraGridBagLayoutPanel1";
            this.ultraGridBagLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.ultraGridBagLayoutPanel1.TabIndex = 0;
            // 
            // availableTablesStackPanel
            // 
            this.availableTablesStackPanel.ActiveControl = this.availableTablesMessageLabel;
            this.availableTablesStackPanel.Controls.Add(this.availableTablesMessageLabel);
            this.availableTablesStackPanel.Controls.Add(this.availableTablesListBox);
            gridBagConstraint8.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint8.OriginX = 0;
            gridBagConstraint8.OriginY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.availableTablesStackPanel, gridBagConstraint8);
            this.availableTablesStackPanel.Location = new System.Drawing.Point(0, 20);
            this.availableTablesStackPanel.Name = "availableTablesStackPanel";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.availableTablesStackPanel, new System.Drawing.Size(200, 100));
            this.availableTablesStackPanel.Size = new System.Drawing.Size(74, 80);
            this.availableTablesStackPanel.TabIndex = 9;
            // 
            // availableTablesMessageLabel
            // 
            this.availableTablesMessageLabel.BackColor = System.Drawing.Color.White;
            this.availableTablesMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.availableTablesMessageLabel.Multiline = true;
            this.availableTablesMessageLabel.Name = "availableTablesMessageLabel";
            this.availableTablesMessageLabel.ReadOnly = true;
            this.availableTablesMessageLabel.Size = new System.Drawing.Size(74, 80);
            this.availableTablesMessageLabel.TabIndex = 5;
            this.availableTablesMessageLabel.Text = "Please wait...";
            this.availableTablesMessageLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // availableTablesListBox
            // 
            this.availableTablesListBox.FormattingEnabled = true;
            this.availableTablesListBox.Location = new System.Drawing.Point(0, 0);
            this.availableTablesListBox.Name = "availableTablesListBox";
            this.availableTablesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableTablesListBox.Size = new System.Drawing.Size(195, 251);
            this.availableTablesListBox.Sorted = true;
            this.availableTablesListBox.TabIndex = 4;
            this.availableTablesListBox.SelectedIndexChanged += new System.EventHandler(this.availableTablesListBox_SelectedIndexChanged);
            this.availableTablesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableTablesListBox_MouseDoubleClick);
            // 
            // databaseComboBox
            // 
            this.databaseComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.databaseComboBox.Enabled = false;
            gridBagConstraint9.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint9.OriginX = 0;
            gridBagConstraint9.OriginY = 3;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.databaseComboBox, gridBagConstraint9);
            this.databaseComboBox.Location = new System.Drawing.Point(0, 12);
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.NullText = "Loading databases...";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.databaseComboBox, new System.Drawing.Size(144, 21));
            this.databaseComboBox.Size = new System.Drawing.Size(74, 21);
            this.databaseComboBox.TabIndex = 8;
            this.databaseComboBox.SelectionChanged += new System.EventHandler(this.databaseComboBox_SelectionChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.addButton);
            this.panel5.Controls.Add(this.removeButton);
            gridBagConstraint10.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint10.OriginX = 1;
            gridBagConstraint10.OriginY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.panel5, gridBagConstraint10);
            this.panel5.Location = new System.Drawing.Point(74, 20);
            this.panel5.Name = "panel5";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.panel5, new System.Drawing.Size(82, 100));
            this.panel5.Size = new System.Drawing.Size(30, 80);
            this.panel5.TabIndex = 7;
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(7, 64);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(67, 24);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add >";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(7, 94);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(67, 24);
            this.removeButton.TabIndex = 1;
            this.removeButton.Text = "< Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // label10
            // 
            gridBagConstraint11.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint11.OriginX = 2;
            gridBagConstraint11.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label10, gridBagConstraint11);
            this.label10.Location = new System.Drawing.Point(104, 0);
            this.label10.Name = "label10";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label10, new System.Drawing.Size(264, 15));
            this.label10.Size = new System.Drawing.Size(96, 6);
            this.label10.TabIndex = 6;
            this.label10.Text = "Selected Tables:";
            // 
            // selectedTablesListBox
            // 
            this.selectedTablesListBox.FormattingEnabled = true;
            gridBagConstraint12.Anchor = Infragistics.Win.Layout.AnchorType.TopLeft;
            gridBagConstraint12.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint12.Insets.Top = 2;
            gridBagConstraint12.OriginX = 2;
            gridBagConstraint12.OriginY = 1;
            gridBagConstraint12.SpanY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.selectedTablesListBox, gridBagConstraint12);
            this.selectedTablesListBox.Location = new System.Drawing.Point(104, 8);
            this.selectedTablesListBox.Name = "selectedTablesListBox";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.selectedTablesListBox, new System.Drawing.Size(264, 251));
            this.selectedTablesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedTablesListBox.Size = new System.Drawing.Size(96, 92);
            this.selectedTablesListBox.Sorted = true;
            this.selectedTablesListBox.TabIndex = 5;
            this.selectedTablesListBox.SelectedIndexChanged += new System.EventHandler(this.selectedTablesListBox_SelectedIndexChanged);
            this.selectedTablesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.selectedTablesListBox_MouseDoubleClick);
            // 
            // label11
            // 
            gridBagConstraint13.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint13.OriginX = 0;
            gridBagConstraint13.OriginY = 2;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label11, gridBagConstraint13);
            this.label11.Location = new System.Drawing.Point(0, 6);
            this.label11.Name = "label11";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label11, new System.Drawing.Size(184, 17));
            this.label11.Size = new System.Drawing.Size(74, 6);
            this.label11.TabIndex = 2;
            this.label11.Text = "Select a database to load it\'s tables:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label12
            // 
            gridBagConstraint14.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint14.OriginX = 0;
            gridBagConstraint14.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label12, gridBagConstraint14);
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Name = "label12";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label12, new System.Drawing.Size(184, 15));
            this.label12.Size = new System.Drawing.Size(74, 6);
            this.label12.TabIndex = 0;
            this.label12.Text = "Available Tables:";
            // 
            // availableTablesTextBox
            // 
            this.availableTablesTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableTablesTextBox.Location = new System.Drawing.Point(0, 19);
            this.availableTablesTextBox.Name = "availableTablesTextBox";
            this.availableTablesTextBox.Size = new System.Drawing.Size(195, 20);
            this.availableTablesTextBox.TabIndex = 1;
            this.availableTablesTextBox.Text = "< Type comma separated names >";
            this.availableTablesTextBox.TextChanged += new System.EventHandler(this.availableTablesTextBox_TextChanged);
            this.availableTablesTextBox.Enter += new System.EventHandler(this.availableTablesTextBox_Enter);
            this.availableTablesTextBox.Leave += new System.EventHandler(this.availableTablesTextBox_Leave);
            // 
            // waitCollectorStatusBackgroundWorker
            // 
            this.waitCollectorStatusBackgroundWorker.WorkerSupportsCancellation = true;
            this.waitCollectorStatusBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.waitCollectorStatusBackgroundWorker_DoWork);
            this.waitCollectorStatusBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.waitCollectorStatusBackgroundWorker_RunWorkerCompleted);
            // 
            // stopWaitCollectorBackgroundWorker
            // 
            this.stopWaitCollectorBackgroundWorker.WorkerSupportsCancellation = true;
            this.stopWaitCollectorBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.stopWaitCollectorBackgroundWorker_DoWork);
            this.stopWaitCollectorBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.stopWaitCollectorBackgroundWorker_RunWorkerCompleted);
            // 
            // serverDateTimeVersionBackgroundWorker
            // 
            this.serverDateTimeVersionBackgroundWorker.WorkerSupportsCancellation = true;
            this.serverDateTimeVersionBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.serverDateTimeVersionBackgroundWorker_DoWork);
            this.serverDateTimeVersionBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.serverDateTimeVersionBackgroundWorker_RunWorkerCompleted);
            // 
            // serverDateTimeVersionBackgroundWorker
            // 
            // SQLdm Minimum Privileges - Varun Chopra - init permissions worker
            this.serverPermissionsBackgroundWorker.WorkerSupportsCancellation = true;
            this.serverPermissionsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.serverPermissionsBackgroundWorker_DoWork);
            this.serverPermissionsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.serverPermissionsBackgroundWorker_RunWorkerCompleted);
            // 
            // blockedProcessThresholdBackgroundWorker
            // 
            this.blockedProcessThresholdBackgroundWorker.WorkerSupportsCancellation = true;
            this.blockedProcessThresholdBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.blockedProcessThresholdBackgroundWorker_DoWork);
            this.blockedProcessThresholdBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.blockedProcessThresholdBackgroundWorker_RunWorkerCompleted);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "label2";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "label9";
            // 
            // mmMonthlyDayRadio
            // 
            this.mmMonthlyDayRadio.AutoSize = true;
            this.mmMonthlyDayRadio.Location = new System.Drawing.Point(55, 74);
            this.mmMonthlyDayRadio.Name = "mmMonthlyDayRadio";
            this.mmMonthlyDayRadio.Size = new System.Drawing.Size(174, 17);
            this.mmMonthlyDayRadio.TabIndex = 59;
            this.mmMonthlyDayRadio.Text = "Day ";
            this.mmMonthlyDayRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyDayRadio.CheckedChanged += new System.EventHandler(mmMonthlyDayRadio_CheckedChanged);
            // 
            // inputDayLimiter
            // 
            this.inputDayLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputDayLimiter.Location = new System.Drawing.Point(105, 84);
            this.inputDayLimiter.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.inputDayLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputDayLimiter.Name = "inputDayLimiter";
            this.inputDayLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputDayLimiter.TabIndex = 32;
            this.inputDayLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mmMonthlyOfEveryLabel
            // 
            this.mmMonthlyOfEveryLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyOfEveryLabel.AutoSize = true;
            this.mmMonthlyOfEveryLabel.Location = new System.Drawing.Point(155, 90);
            //this.mmMonthlyOfEveryLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyOfEveryLabel.Name = "mmMonthlyOfEveryLabel";
            this.mmMonthlyOfEveryLabel.Size = new System.Drawing.Size(40, 13);
            this.mmMonthlyOfEveryLabel.TabIndex = 97;
            this.mmMonthlyOfEveryLabel.Text = "of every";
            this.mmMonthlyOfEveryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // inputOfEveryMonthLimiter
            // 
            this.inputOfEveryMonthLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputOfEveryMonthLimiter.Location = new System.Drawing.Point(205, 84);
            this.inputOfEveryMonthLimiter.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.inputOfEveryMonthLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputOfEveryMonthLimiter.Name = "inputOfEveryMonthLimiter";
            this.inputOfEveryMonthLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputOfEveryMonthLimiter.TabIndex = 32;
            this.inputOfEveryMonthLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mmMonthlyLabel1
            // 
            this.mmMonthlyLabel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyLabel1.AutoSize = true;
            this.mmMonthlyLabel1.Location = new System.Drawing.Point(250, 90);
            this.mmMonthlyLabel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyLabel1.Name = "mmMonthlyLabel1";
            this.mmMonthlyLabel1.Size = new System.Drawing.Size(83, 13);
            this.mmMonthlyLabel1.TabIndex = 98;
            this.mmMonthlyLabel1.Text = "month(s) ";
            this.mmMonthlyLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthlyTheRadio
            // 
            this.mmMonthlyTheRadio.AutoSize = true;
            this.mmMonthlyTheRadio.Location = new System.Drawing.Point(55, 114);
            this.mmMonthlyTheRadio.Name = "mmMonthlyTheRadio";
            this.mmMonthlyTheRadio.Size = new System.Drawing.Size(74, 17);
            this.mmMonthlyTheRadio.TabIndex = 59;
            this.mmMonthlyTheRadio.Text = "The ";
            this.mmMonthlyTheRadio.UseVisualStyleBackColor = true;
            this.mmMonthlyTheRadio.CheckedChanged += new System.EventHandler(mmMonthlyTheRadio_CheckedChanged);
            // 
            // WeekcomboBox
            // 
            this.WeekcomboBox.FormattingEnabled = true;

            this.WeekcomboBox.Location = new System.Drawing.Point(105, 114);
            this.WeekcomboBox.Name = "WeekcomboBox";
            this.WeekcomboBox.Size = new System.Drawing.Size(81, 21);
            // 
            // DaycomboBox
            // 
            this.DaycomboBox.FormattingEnabled = true;
            this.DaycomboBox.Location = new System.Drawing.Point(205, 114);
            this.DaycomboBox.Name = "DaycomboBox";
            this.DaycomboBox.Size = new System.Drawing.Size(81, 21);
            // 
            // mmMonthlyOfTheEveryLabel
            // 
            this.mmMonthlyOfTheEveryLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyOfTheEveryLabel.AutoSize = true;
            this.mmMonthlyOfTheEveryLabel.Location = new System.Drawing.Point(285, 130);
            this.mmMonthlyOfTheEveryLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyOfTheEveryLabel.Name = "mmMonthlyOfTheEveryLabel";
            this.mmMonthlyOfTheEveryLabel.Size = new System.Drawing.Size(40, 13);
            this.mmMonthlyOfTheEveryLabel.TabIndex = 97;
            this.mmMonthlyOfTheEveryLabel.Text = "of every";
            this.mmMonthlyOfTheEveryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // inputOfEveryTheMonthLimiter
            // 
            this.inputOfEveryTheMonthLimiter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputOfEveryTheMonthLimiter.Location = new System.Drawing.Point(335, 124);
            this.inputOfEveryTheMonthLimiter.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.inputOfEveryTheMonthLimiter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputOfEveryTheMonthLimiter.Name = "inputOfEveryTheMonthLimiter";
            this.inputOfEveryTheMonthLimiter.Size = new System.Drawing.Size(40, 20);
            this.inputOfEveryTheMonthLimiter.TabIndex = 32;
            this.inputOfEveryTheMonthLimiter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mmMonthlyLabel2
            // 
            this.mmMonthlyLabel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyLabel2.AutoSize = true;
            this.mmMonthlyLabel2.Location = new System.Drawing.Point(380, 130);
            this.mmMonthlyLabel2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyLabel2.Name = "mmMonthlyLabel2";
            this.mmMonthlyLabel2.Size = new System.Drawing.Size(83, 13);
            this.mmMonthlyLabel2.TabIndex = 98;
            this.mmMonthlyLabel2.Text = "month(s) ";
            this.mmMonthlyLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthlyAtLabel
            // 
            this.mmMonthlyAtLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mmMonthlyAtLabel.AutoSize = true;
            this.mmMonthlyAtLabel.Location = new System.Drawing.Point(57, 154);
            //this.mmMonthlyAtLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.mmMonthlyAtLabel.Name = "mmMonthlyAtLabel";
            this.mmMonthlyAtLabel.Size = new System.Drawing.Size(83, 13);
            this.mmMonthlyAtLabel.TabIndex = 99;
            this.mmMonthlyAtLabel.Text = "at ";
            this.mmMonthlyAtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mmMonthRecurringBegin
            // 
            dropDownEditorButton8.Key = "DropDownList";
            dropDownEditorButton8.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmMonthRecurringBegin.ButtonsRight.Add(dropDownEditorButton8);
            this.mmMonthRecurringBegin.DateTime = new System.DateTime(1900, 1, 1, 2, 0, 0, 0);
            this.mmMonthRecurringBegin.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmMonthRecurringBegin.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmMonthRecurringBegin.Location = new System.Drawing.Point(79, 140);
            this.mmMonthRecurringBegin.MaskInput = "{time}";
            this.mmMonthRecurringBegin.Name = "mmMonthRecurringBegin";
            this.mmMonthRecurringBegin.Size = new System.Drawing.Size(90, 21);
            this.mmMonthRecurringBegin.TabIndex = 53;
            this.mmMonthRecurringBegin.Time = System.TimeSpan.Parse("02:00:00");
            this.mmMonthRecurringBegin.Value = new System.DateTime(1900, 1, 1, 2, 0, 0, 0);
            // 
            // label29
            // 
            this.label29.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 7);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(68, 13);
            this.label29.TabIndex = 57;
            this.label29.Text = "for";
            // 
            // mmMonthRecurringDuration
            // 
            dropDownEditorButton9.Key = "DropDownList";
            dropDownEditorButton9.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.mmMonthRecurringDuration.ButtonsRight.Add(dropDownEditorButton9);
            this.mmMonthRecurringDuration.DateTime = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.mmMonthRecurringDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.mmMonthRecurringDuration.FormatString = "HH:mm";
            this.mmMonthRecurringDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.mmMonthRecurringDuration.Location = new System.Drawing.Point(77, 3);
            this.mmMonthRecurringDuration.MaskInput = "hh:mm";
            this.mmMonthRecurringDuration.Name = "mmMonthRecurringDuration";
            this.mmMonthRecurringDuration.Size = new System.Drawing.Size(69, 21);
            this.mmMonthRecurringDuration.TabIndex = 56;
            this.mmMonthRecurringDuration.Time = System.TimeSpan.Parse("00:00:00");
            this.mmMonthRecurringDuration.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label30
            // 
            this.label30.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(3, 7);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(68, 13);
            this.label30.TabIndex = 57;
            this.label30.Text = "hrs";
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Controls.Add(this.label29);
            this.flowLayoutPanel8.Controls.Add(this.mmMonthRecurringDuration);
            this.flowLayoutPanel8.Controls.Add(this.label30);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(52, 171);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(221, 31);
            this.flowLayoutPanel8.TabIndex = 57;


            // 
            // MonitoredSqlServerInstancePropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(687, 788);
            this.Controls.Add(this.propertiesControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(695, 570);
            this.Name = "MonitoredSqlServerInstancePropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Monitored SQL Server Properties - {0}";
            //this.backcolor = System.Drawing.Color.Red;
            //this.ForeColor = System.Drawing.Color.Black;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.MonitoredSqlServerInstancePropertiesDialog_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MonitoredSqlServerInstancePropertiesDialog_FormClosed);
            this.Load += new System.EventHandler(this.MonitoredSqlServerInstancePropertiesDialog_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.MonitoredSqlServerInstancePropertiesDialog_HelpRequested);
            this.blockedProcessWarningPanel.ResumeLayout(false);
            this.blockedProcessWarningPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockedProcessWarningImage)).EndInit();
            this.activeMonitorPropertyPage.ResumeLayout(false);
            this.propertyPage2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel15.ResumeLayout(false);
            this.tableLayoutPanel15.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBlockedProcessThreshold)).EndInit();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.popularPropertyPage.ResumeLayout(false);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputBufferLimiter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputDayLimiter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryMonthLimiter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputOfEveryTheMonthLimiter)).EndInit();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collectionIntervalTimeSpanEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverPingTimeSpanEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseStatisticsTimeSpanEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarManager)).EndInit();
            this.propertyPage1.ResumeLayout(false);
            this.queryMonitorPropertyPage.ResumeLayout(false);
            this.tableLayoutPanel18.ResumeLayout(false);
            this.tableLayoutPanel18.PerformLayout();
            this.tableLayoutPanel17.ResumeLayout(false);
            this.tableLayoutPanel17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.queryMonitorWarningImage)).EndInit();
            this.tableLayoutPanel16.ResumeLayout(false);
            this.tableLayoutPanel16.PerformLayout();
            this.fmePoorlyPerformingThresholds.ResumeLayout(false);
            this.fmePoorlyPerformingThresholds.PerformLayout();
            this.topPlanTableLayoutPanel.ResumeLayout(false);
            this.topPlanTableLayoutPanel.PerformLayout();
            this.traceThresholdsTableLayoutPanel.ResumeLayout(false);
            this.traceThresholdsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.durationThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.physicalWritesThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topPlanSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logicalReadsThresholdSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpuThresholdSpinner)).EndInit();
            this.replicationPropertyPage.ResumeLayout(false);
            this.replicationMainContainer.ResumeLayout(false);
            this.replicationMainContainer.PerformLayout();
            this.tableStatisticsPropertyPage.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimumTableSize)).EndInit();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quietTimeStartEditor)).EndInit();
            this.customCountersPropertyPage.ResumeLayout(false);
            this.customCountersContentPage.ContentPanel.ResumeLayout(false);
            this.customCounterStackLayoutPanel.ResumeLayout(false);
            this.customCounterContentPanel.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.maintenancePropertyPage.ResumeLayout(false);
            this.maintenanceModeControlsContainer.ResumeLayout(false);
            this.maintenanceModeControlsContainer.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.mmOncePanel.ResumeLayout(false);
            this.mmOncePanel.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceBeginDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmOnceStopDate)).EndInit();
            this.mmRecurringPanel.ResumeLayout(false);
            this.mmRecurringPanel.PerformLayout();
            this.mmMonthlyRecurringPanel.ResumeLayout(false);
            this.mmMonthlyRecurringPanel.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringDuration)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmRecurringBegin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringBegin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmMonthRecurringDuration)).EndInit();
            this.osPropertyPage.ResumeLayout(false);
            this.osMetricsMainContainer.ResumeLayout(false);
            this.osMetricsMainContainer.PerformLayout();
            this.wmiCredentialsSecondaryContainer.ResumeLayout(false);
            this.wmiCredentialsContainer.ResumeLayout(false);
            this.wmiCredentialsContainer.PerformLayout();
            this.diskPropertyPage.ResumeLayout(false);
            this.diskDriversMainContainer.ResumeLayout(false);
            this.diskDriversMainContainer.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableDisksLayoutPanel)).EndInit();
            this.availableDisksLayoutPanel.ResumeLayout(false);
            this.availableDisksLayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.availableDisksStackPanel.ResumeLayout(false);
            this.availableDisksStackPanel.PerformLayout();
            this.clusterPropertyPage.ResumeLayout(false);
            this.clusterSettingsMainContainer.ResumeLayout(false);
            this.clusterSettingsMainContainer.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.waitPropertyPage.ResumeLayout(false);
            this.waitMonitoringMainContainer.ResumeLayout(false);
            this.waitMonitoringMainContainer.PerformLayout();
            this.collectStatisticsSecondaryContainer.ResumeLayout(false);
            this.collectStatisticsSecondaryContainer.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waitStatisticsRunTime)).EndInit();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.virtualizationPropertyPage.ResumeLayout(false);
            this.virtualizationMainContainer.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).EndInit();
            this.ultraGridBagLayoutPanel1.ResumeLayout(false);
            this.ultraGridBagLayoutPanel1.PerformLayout();
            this.availableTablesStackPanel.ResumeLayout(false);
            this.availableTablesStackPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseComboBox)).EndInit();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

            #region Page for analysis tab
            //start 10.0 SQLdm srishti purohit

            // 
            // office2007PropertyPage4
            // 
            this.office2007PropertyPageAnalysisConfiguration.BackColor = backColor; 
            this.office2007PropertyPageAnalysisConfiguration.ForeColor = foreColor;
            this.office2007PropertyPageAnalysisConfiguration.BorderWidth = 1;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.ForeColor = foreColor;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.BackColor = backColor;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.BackColor2 = backColor;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.ShowBorder = false;
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.Size = new System.Drawing.Size(504, 632);
            this.office2007PropertyPageAnalysisConfiguration.ContentPanel.TabIndex = 1;
            this.office2007PropertyPageAnalysisConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPageAnalysisConfiguration.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AddServersWizardFeaturesImage;
            this.office2007PropertyPageAnalysisConfiguration.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPageAnalysisConfiguration.Name = "office2007PropertyPage4";
            this.office2007PropertyPageAnalysisConfiguration.Size = new System.Drawing.Size(506, 689);
            this.office2007PropertyPageAnalysisConfiguration.TabIndex = 0;
            this.office2007PropertyPageAnalysisConfiguration.Text = "Configuration settings for Analysis.";

            // 
            // analysisConfigurationPropertyPage
            // 
            this.analysisConfigurationPropertyPage.Controls.Add(this.analysisConfigurationPage);
            this.analysisConfigurationPropertyPage.Controls.Add(this.office2007PropertyPageAnalysisConfiguration);
            this.analysisConfigurationPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analysisConfigurationPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.analysisConfigurationPropertyPage.Name = "analysisConfigurationPropertyPage";
            this.analysisConfigurationPropertyPage.Size = new System.Drawing.Size(506, 689);
            this.analysisConfigurationPropertyPage.TabIndex = 0;
            this.analysisConfigurationPropertyPage.Text = "Analysis Configuration";
            this.analysisConfigurationPropertyPage.AutoScroll = true;
            this.analysisConfigurationPropertyPage.ResumeLayout();
            #endregion
            //end

        }

        #endregion

        #region Private properties
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesControl propertiesControl;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage popularPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage queryMonitorPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage tableStatisticsPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage popularPropertiesContentPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage2;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage4;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage maintenanceModeContentPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel traceThresholdsTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel topPlanTableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel topPlanLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel topPlanSuffixLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown topPlanSpinner;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor topPlanComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown durationThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel physicalWritesThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown physicalWritesThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logicalReadsThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown cpuThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel cpuThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown logicalReadsThresholdSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel durationThresholdLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureStoredProceduresCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureSqlStatementsCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox captureSqlBatchesCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox enableQueryMonitorTraceCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip2;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage customCountersPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage customCountersContentPage;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel customCounterStackLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  customCounterContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel customCounterMessageLabel;
        private System.ComponentModel.BackgroundWorker testConnectionBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage replicationPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage replicationPropertyPageContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox3;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox disableReplicationStuff;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage maintenancePropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmOnceRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmRecurringRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmMonthlyRecurringRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmAlwaysRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmNeverRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  mmRecurringPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  mmMonthlyRecurringPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginSatCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginFriCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginThurCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginWedCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginTueCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginMonCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox mmBeginSunCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmMonthlyDayRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton mmMonthlyTheRadio;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown inputDayLimiter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown inputOfEveryMonthLimiter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown inputOfEveryTheMonthLimiter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmMonthlyLabel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmMonthlyLabel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox WeekcomboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox DaycomboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmMonthlyOfEveryLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmMonthlyOfTheEveryLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmMonthlyAtLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmMonthRecurringBegin;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label29;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label30;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip15;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  mmOncePanel;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox5;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip27;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox6;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MonitoredSqlServerInstancePropertiesDialog_Toolbars_Dock_Area_Bottom;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage osPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage osContentPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip18;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage diskPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage diskContentPage;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel availableDisksLayoutPanel;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel1;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel availableTablesStackPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox availableTablesMessageLabel;
        private System.Windows.Forms.ListBox availableTablesListBox;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor databaseComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private System.Windows.Forms.ListBox selectedTablesListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox availableTablesTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox adhocDisksTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label14;
        private System.Windows.Forms.ListBox selectedDisksListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label15;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel availableDisksStackPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox availableDisksMessageLabel;
        private System.Windows.Forms.ListBox availableDisksListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addDiskButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeDiskButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel queryMonitorRunningMessage;
        private System.Windows.Forms.PictureBox queryMonitorWarningImage;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage clusterPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox fmePoorlyPerformingThresholds;
        private Idera.SQLdm.DesktopClient.Controls.PropertyPage waitPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage3;
        private System.ComponentModel.BackgroundWorker waitCollectorStatusBackgroundWorker;
        private System.ComponentModel.BackgroundWorker stopWaitCollectorBackgroundWorker;
        private System.ComponentModel.BackgroundWorker serverDateTimeVersionBackgroundWorker;
        // SQLdm Minimum Privileges - Varun Chopra - declare permissions worker
        private System.ComponentModel.BackgroundWorker serverPermissionsBackgroundWorker;
        private Controls.PropertyPage virtualizationPropertyPage;
        private Controls.Office2007PropertyPage office2007PropertyPage5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox11;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip26;
        private Controls.PropertyPage propertyPage1;
        private Controls.BaselineConfigurationPage baselineConfiguration1;
        //10.0 SQLdm srishti purohit -- Doctor configuration for UI
        private Controls.Analysis.AnalysisConfigurationPage analysisConfigurationPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox directWmiPassword;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label24;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label28;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox directWmiUserName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiDirect;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiOleAutomation;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton optionWmiNone;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox optionWmiCSCreds;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton wmiTestButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmRecurringDuration;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmMonthRecurringDuration;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmRecurringBegin;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton btnVMConfig;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label26;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel vCenterNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label27;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel vmNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel mainTableLayout;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraDropDownButton tagsDropDownButton;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip17;
      //  private Infragistics.Win.Misc.UltraDropDownButton tagsDropDownButtonServerType;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox cmbServerType;
        private Controls.PropertiesHeaderStrip propertiesHeaderStripServerType;
        private Controls.PropertiesHeaderStrip headerStrip1;
        private Controls.PropertiesHeaderStrip headerStrip2;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectExtendedSessionDataCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown inputBufferLimiter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox limitInputBuffer;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label23;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel dataCollectionDescriptionLabel;
        private Infragistics.Win.UltraWinEditors.UltraTimeSpanEditor collectionIntervalTimeSpanEditor;
        private Infragistics.Win.UltraWinEditors.UltraTimeSpanEditor serverPingTimeSpanEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label22;
        private Infragistics.Win.UltraWinEditors.UltraTimeSpanEditor databaseStatisticsTimeSpanEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox groupBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox encryptDataCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox trustServerCertificateCheckbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton useSqlServerAuthenticationRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton useWindowsAuthenticationRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel loginNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox loginNameTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel passwordLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox passwordTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox friendlyNameTextbox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label25;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastTableGrowthCollectionTimeDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastTableGrowthCollectionTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastTableFragmentationCollectionTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lastTableFragmentationCollectionTimeDescriptionLabel;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton selectTableStatisticsTablesButton;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minimumTableSize;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsSundayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsMondayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsTuesdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsWednesdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsThursdayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsFridayCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox collectTableStatsSaturdayCheckBox;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor quietTimeStartEditor;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel endTimeLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox2;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton testCustomCountersButton;
        private Controls.DualListSelectorControl customCountersSelectionList;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel clusterSettingsMainContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label16;
        //SQLDM-30197
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel clusterWarningLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox preferredNode;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton setCurrentNode;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip21;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel waitMonitoringMainContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel collectStatisticsSecondaryContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkUseXE;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkUseQs;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkCollectActiveWaits;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton ScheduledQueryWaits;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label19;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel waitStatisticsServerDateTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label17;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor waitStatisticsStartDate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor waitStatisticsStartTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor waitStatisticsRunTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton PerpetualQueryWaits;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox12;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip23;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label20;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton waitStatisticsFilterButton;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip25;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel waitStatisticsStatus;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton stopWaitCollector;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton refreshWaitCollectorStatus;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox13;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip24;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel13;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip20;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip19;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox autoDiscoverDisksCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label21;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel mmServerDateTime;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmOnceBeginTime;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor mmOnceBeginDate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor mmOnceStopTime;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor mmOnceStopDate;
        private System.ComponentModel.BackgroundWorker blockedProcessThresholdBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel17;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private TableLayoutPanel maintenanceModeControlsContainer;
        private TableLayoutPanel wmiCredentialsContainer;
        private TableLayoutPanel osMetricsMainContainer;
        private Panel wmiCredentialsSecondaryContainer;
        private TableLayoutPanel diskDriversMainContainer;
        private TableLayoutPanel virtualizationMainContainer;
        private TableLayoutPanel replicationMainContainer;
        private PropertyPage activeMonitorPropertyPage;
        private PropertyPage propertyPage2;
        private TableLayoutPanel tableLayoutPanel4;
        private PropertiesHeaderStrip propertiesHeaderStrip22;
        private TableLayoutPanel tableLayoutPanel11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox8;

        private PropertiesHeaderStrip propertiesHeaderStrip29; //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- declaring new controls
        private TableLayoutPanel tableLayoutPanel19;


        private PropertiesHeaderStrip propertiesHeaderStrip28;
        private Office2007PropertyPage office2007PropertyPage6;
        private TableLayoutPanel tableLayoutPanel15;
        private CheckBox captureDeadlockCheckBox;
        private CheckBox chkCaptureAutogrow;
        private CheckBox chkCaptureBlocking;
        private CheckBox chkActivityMonitorEnable;
        private RadioButton rButtonAMUseExtendedEvents;//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- declaring new controls
        private RadioButton rButtonAMUseTrace;


        private Panel pnlQueryMonitoringTechniques;//SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- declaring new controls
        private RadioButton rButtonUseExtendedEvents;
        // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
        private RadioButton rButtonUseQueryStore;
        private RadioButton rButtonUseTrace;
        private CheckBox chkCollectQueryPlans;
        private CheckBox chkCollectEstimatedQueryPlans;
        private Label lblQueryPlanMessage;
        private Label lblQueryPlanQsMessage;

        private GroupBox groupBox3;
        private TableLayoutPanel tableLayoutPanel12;
        private Label lblBlockedProcessSpinner;
        private CustomNumericUpDown spnBlockedProcessThreshold;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  blockedProcessWarningPanel;
        private System.Windows.Forms.PictureBox blockedProcessWarningImage;
        private Label lblBlockedProcessWarning;
        private Button queryMonitorAdvancedOptionsButton;
        private Button testConnectionCredentialsButton;

        //SQLdm 10.0 Srishti Purohit -- COnfiguration

        private Idera.SQLdm.DesktopClient.Controls.PropertyPage analysisConfigurationPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPageAnalysisConfiguration;

        // SQLdm 10.3 Varun Chopra Warning Label for Wait Monitoring Collection
        private Label lblWaitMonitoringWarningLabel;
        private Label lblActivityMonitorWarningLabel;
        private Label lblQueryMonitoringWarningLabel;
        private Label lblOSMetricOLEWarningLabel;
        #endregion

    }
}
