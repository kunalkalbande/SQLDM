using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AdvancedAlertConfigurationDialog
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
            bool isDarkThemeSelected = Settings.Default.ColorScheme == "Dark";
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton dropDownEditorButton2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomDropDownEditorButton("DropDownList");
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem1 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 1", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem2 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 2", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem3 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 3", null, null);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedAlertConfigurationDialog));
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList5 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem12 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList6 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem13 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem14 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList7 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem15 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem16 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem17 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList8 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem18 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem19 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList9 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem20 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem21 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList10 = new Controls.CustomControls.CustomValueList(10018657);
            Infragistics.Win.ValueListItem valueListItem22 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem23 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem24 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList11 = new Controls.CustomControls.CustomValueList(5632719);
            Infragistics.Win.ValueListItem valueListItem25 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem26 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList12 = new Controls.CustomControls.CustomValueList(5686891);
            Infragistics.Win.ValueListItem valueListItem27 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem28 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem4 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 1", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem5 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 2", null, null);
            Infragistics.Win.UltraWinListView.UltraListViewItem ultraListViewItem6 = new Infragistics.Win.UltraWinListView.UltraListViewItem("Item 3", null, null);
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance66 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance67 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance68 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance69 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.smoothingContentPropertyPage = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.agingAlertActivePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label26 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.agingAlertActiveDuration = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label25 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.agingAlertActiveTimeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.jobAlertActivePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.suppressionDaysSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label37 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobAlertActiveDuration = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor();
            this.label19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobAlertActiveTimeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.jobFailureSuppressionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.raiseJobFailureAlertLimitedRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.raiseJobFailureAlertAnytimeRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.propertiesHeaderStrip12 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.timeSuppressionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.suppressionMinutesSpinner = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.smoothingPropertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.ultraTabPageControl8 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage7 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
           this.spnLogSize = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.logSizeLimitPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel21 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.lblLogSize = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip20 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.driveFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel19 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip16 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.drivesExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterDrivesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label29 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel20 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label30 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tablesFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip15 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.tablesExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterTablesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label27 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel18 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label28 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanRegularExpressionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.logScanIncludeTextBoxRegexInfo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label34 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanIncludeTextBoxRegexWarning = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label23 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanIncludeTextBoxRegexCritical = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label21 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip14 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.panel16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label24 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label33 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanIncludeTextBoxInfo = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label22 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.logScanIncludeTextBoxWarning = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label20 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip13 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.logScanIncludeTextBoxCritical = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.logScanDirections = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sessionUsersFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip11 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sessionUsersExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel14 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sessionHostServersFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label13 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip10 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sessionHostServersExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel11 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label10 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sessionApplicationsFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label12 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.propertiesHeaderStrip9 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.sessionApplicationsExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.panel8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label9 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.sessionExludeDMappFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.excludeDMappHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.excludeDMapplicationCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.includeReadOnlyDatabases = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.jobCategoriesFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip8 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.jobCategoriesExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterJobCategoriesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label15 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label8 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobsFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip7 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.jobsExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterJobsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label16 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.databasesFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            this.readOnlyDatabaseIncludePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.readOnlyDatabaseIncludeFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();

            //SQLDM-29041 -- Add availability group alert options.
            this.availabilityGroupBackupAlertPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.availabilityGroupBackupAlertFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.availabilityGroupBackupAlertOptions = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.availabilityGroupPrimaryOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.availabilityGroupSecondaryOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.availabilityGroupBothOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.availabilityGroupDefaultOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();


            this.replicaCollectionOptionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.replicaCollectionOptionFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.replicaCollectionOptions = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox();
            this.replicaCollectionMonitoredOnlyOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.replicaCollectionAllOption = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();

            this.databasesExcludeFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();

            this.propertiesHeaderStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.databasesExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterDatabasesButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label17 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.filegroupsFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.filegroupsExcludeFilterPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.propertiesHeaderStrip21 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.filegroupsExcludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.filterFilegroupsButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label38 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel22 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label39 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.versionStorePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label35 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.minVersionStoreGenerationRate = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.thresholdHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.versionStoreSizePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label36 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.versionStoreSizeUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.versionStoreSizeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.ultraTabPageControl6 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage5 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.propertiesHeaderStrip6 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.skipAlertButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.generateAlertButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.ultraTabPageControl12 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage8 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.propertiesHeaderStrip17 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.ignoreAutogrow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.useAutogrow = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.ultraTabPageControl13 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage9 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.minRuntimeJobsPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.flowLayoutPanel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.minlRuntimeUnit0Label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.minimalRuntimeUpDown = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            this.minlRuntimeUnit1Label = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.minLongJobRuntimeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.removeExclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.editExclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addExclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.clearExclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label32 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.gridExclude = new System.Windows.Forms.DataGridView();
            this.excludeCatOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeJobOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeJobName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeStepOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excludeStepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.removeInclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.editInclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addInclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.clearInclude = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label31 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.gridInclude = new System.Windows.Forms.DataGridView();
            this.includeCatOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.includeCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.includeJobOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.includeJobName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.includeStepOpCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.includeStepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkAlertOnJobSteps = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.propertiesHeaderStrip18 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.ultraTabPageControl9 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl10 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl11 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage4 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.label7 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.excludeSelfBlockingCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this.button3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.blockingTypesListView = new Infragistics.Win.UltraWinListView.UltraListView();
            this.propertiesHeaderStrip4 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();


            this.FilterOptionPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.FilterOptionPanel.ResumeLayout(false);
            this.FilterOptionPanel.PerformLayout();
            this.FilterOptionPanel.SuspendLayout();
            this.propertiesHeaderStrip19 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.textBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.label42 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label43 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label41 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            
            this.label40 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.FilterOptionNumeric = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.FilterOptionNumeric)).BeginInit();
            this.SuspendLayout();
            this.FilterOptionNumericOutOf = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.FilterOptionNumericOutOf)).BeginInit();
            this.SuspendLayout();


            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage2 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.excludedJobNamesListBox = new Infragistics.Win.UltraWinListView.UltraListView();
            this.availableJobNamesStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.removeJobNameExclusionButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addJobNameExclusionButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.label5 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label6 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.adhocJobNameTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.propertiesHeaderStrip2 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.availableJobNamesListBox = new Infragistics.Win.UltraWinListView.UltraListView();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage3 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.informationBox3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.userNameGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.programNameGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.hostNameGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.appNameGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();

            this.propertiesHeaderStrip3 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.ultraTabPageControl7 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.office2007PropertyPage6 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.informationBox4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.jobCategoriesClearButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.jobCategoriesListBox = new Infragistics.Win.UltraWinListView.UltraListView();
            this.propertiesHeaderStrip5 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.gradientPanel1 = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.tabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl(true);
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1.SuspendLayout();
            this.smoothingContentPropertyPage.ContentPanel.SuspendLayout();
            this.agingAlertActivePanel.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agingAlertActiveDuration)).BeginInit();
            this.jobAlertActivePanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.suppressionDaysSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobAlertActiveDuration)).BeginInit();
            this.jobFailureSuppressionPanel.SuspendLayout();
            this.timeSuppressionPanel.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.suppressionMinutesSpinner)).BeginInit();
            this.ultraTabPageControl8.SuspendLayout();
            this.office2007PropertyPage7.ContentPanel.SuspendLayout();
            this.logSizeLimitPanel.SuspendLayout();
            this.panel21.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnLogSize)).BeginInit();
            this.driveFilterPanel.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel20.SuspendLayout();
            this.tablesFilterPanel.SuspendLayout();
            this.panel17.SuspendLayout();
            this.panel18.SuspendLayout();
            this.logScanRegularExpressionPanel.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel16.SuspendLayout();
            this.logScanPanel.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel12.SuspendLayout();
            this.sessionUsersFilterPanel.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel14.SuspendLayout();
            this.sessionHostServersFilterPanel.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            this.sessionApplicationsFilterPanel.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel8.SuspendLayout();
            this.sessionExludeDMappFilterPanel.SuspendLayout();
            this.jobCategoriesFilterPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.jobsFilterPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.databasesFilterPanel.SuspendLayout();
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            this.readOnlyDatabaseIncludePanel.SuspendLayout();
            this.readOnlyDatabaseIncludeFilterPanel.SuspendLayout();

            //SQLDM-29041 -- Add availability group alert options.
            this.availabilityGroupBackupAlertPanel.SuspendLayout();
            this.availabilityGroupBackupAlertFilterPanel.SuspendLayout();
            this.availabilityGroupBackupAlertOptions.SuspendLayout();

            this.replicaCollectionOptionPanel.SuspendLayout();
            this.replicaCollectionOptionFilterPanel.SuspendLayout();
            this.replicaCollectionOptions.SuspendLayout();

            this.databasesExcludeFilterPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.filegroupsFilterPanel.SuspendLayout();
            this.filegroupsExcludeFilterPanel.SuspendLayout();
            this.panel22.SuspendLayout();
            this.versionStorePanel.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minVersionStoreGenerationRate)).BeginInit();
            this.versionStoreSizePanel.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.versionStoreSizeUpDown)).BeginInit();
            this.ultraTabPageControl6.SuspendLayout();
            this.office2007PropertyPage5.ContentPanel.SuspendLayout();
            this.ultraTabPageControl12.SuspendLayout();
            this.office2007PropertyPage8.ContentPanel.SuspendLayout();
            this.ultraTabPageControl13.SuspendLayout();
            this.office2007PropertyPage9.ContentPanel.SuspendLayout();
            this.minRuntimeJobsPanel.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimalRuntimeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridExclude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInclude)).BeginInit();
            this.ultraTabPageControl5.SuspendLayout();
            this.office2007PropertyPage4.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockingTypesListView)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            this.office2007PropertyPage2.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.excludedJobNamesListBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableJobNamesListBox)).BeginInit();
            this.ultraTabPageControl4.SuspendLayout();
            this.office2007PropertyPage3.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userNameGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.programNameGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hostNameGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appNameGrid)).BeginInit();
            this.ultraTabPageControl7.SuspendLayout();
            this.office2007PropertyPage6.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jobCategoriesListBox)).BeginInit();
            this.gradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.smoothingContentPropertyPage);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(141, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(495, 495);
            // 
            // smoothingContentPropertyPage
            // 
            this.smoothingContentPropertyPage.BackColor = System.Drawing.Color.White;
            this.smoothingContentPropertyPage.BorderWidth = 0;
            // 
            // 
            // 
            this.smoothingContentPropertyPage.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.smoothingContentPropertyPage.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.smoothingContentPropertyPage.ContentPanel.Controls.Add(this.agingAlertActivePanel);
            this.smoothingContentPropertyPage.ContentPanel.Controls.Add(this.jobAlertActivePanel);
            this.smoothingContentPropertyPage.ContentPanel.Controls.Add(this.jobFailureSuppressionPanel);
            this.smoothingContentPropertyPage.ContentPanel.Controls.Add(this.timeSuppressionPanel);
            this.smoothingContentPropertyPage.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.smoothingContentPropertyPage.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.smoothingContentPropertyPage.ContentPanel.Name = "ContentPanel";
            this.smoothingContentPropertyPage.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.smoothingContentPropertyPage.ContentPanel.ShowBorder = false;
            this.smoothingContentPropertyPage.ContentPanel.Size = new System.Drawing.Size(495, 440);
            this.smoothingContentPropertyPage.ContentPanel.TabIndex = 1;
            this.smoothingContentPropertyPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothingContentPropertyPage.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.AlertsFeature1;
            this.smoothingContentPropertyPage.Location = new System.Drawing.Point(0, 0);
            this.smoothingContentPropertyPage.Name = "smoothingContentPropertyPage";
            this.smoothingContentPropertyPage.Size = new System.Drawing.Size(495, 495);
            this.smoothingContentPropertyPage.TabIndex = 0;
            this.smoothingContentPropertyPage.Text = "Customize when an alert should be raised.";
            // 
            // agingAlertActivePanel
            // 
            this.agingAlertActivePanel.Controls.Add(this.flowLayoutPanel3);
            this.agingAlertActivePanel.Controls.Add(this.agingAlertActiveTimeHeaderStrip);
            this.agingAlertActivePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.agingAlertActivePanel.Location = new System.Drawing.Point(1, 258);
            this.agingAlertActivePanel.Name = "agingAlertActivePanel";
            this.agingAlertActivePanel.Size = new System.Drawing.Size(493, 85);
            this.agingAlertActivePanel.TabIndex = 8;
            this.agingAlertActivePanel.Visible = false;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.label26);
            this.flowLayoutPanel3.Controls.Add(this.agingAlertActiveDuration);
            this.flowLayoutPanel3.Controls.Add(this.label25);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(7, 35);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(479, 48);
            this.flowLayoutPanel3.TabIndex = 57;
            // 
            // label26
            // 
            this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(3, 7);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(164, 13);
            this.label26.TabIndex = 5;
            this.label26.Text = "The alert should remain active for";
            // 
            // agingAlertActiveDuration
            // 
            this.agingAlertActiveDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton1.Key = "DropDownList";
            dropDownEditorButton1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.agingAlertActiveDuration.ButtonsRight.Add(dropDownEditorButton1);
            this.agingAlertActiveDuration.DateTime = new System.DateTime(2008, 9, 19, 12, 0, 0, 0);
            this.agingAlertActiveDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.agingAlertActiveDuration.FormatString = "HH:mm";
            this.agingAlertActiveDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.agingAlertActiveDuration.Location = new System.Drawing.Point(173, 3);
            this.agingAlertActiveDuration.MaskInput = "hh:mm";
            this.agingAlertActiveDuration.Name = "agingAlertActiveDuration";
            this.agingAlertActiveDuration.Size = new System.Drawing.Size(77, 21);
            this.agingAlertActiveDuration.TabIndex = 55;
            this.agingAlertActiveDuration.Time = System.TimeSpan.Parse("12:00:00");
            this.agingAlertActiveDuration.Value = new System.DateTime(2008, 9, 19, 12, 0, 0, 0);
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(256, 7);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(105, 13);
            this.label25.TabIndex = 56;
            this.label25.Text = "(Hours and Minutes).";
            // 
            // agingAlertActiveTimeHeaderStrip
            // 
            this.agingAlertActiveTimeHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.agingAlertActiveTimeHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.agingAlertActiveTimeHeaderStrip.Location = new System.Drawing.Point(7, 6);
            this.agingAlertActiveTimeHeaderStrip.Name = "agingAlertActiveTimeHeaderStrip";
            this.agingAlertActiveTimeHeaderStrip.Size = new System.Drawing.Size(479, 25);
            this.agingAlertActiveTimeHeaderStrip.TabIndex = 4;
            this.agingAlertActiveTimeHeaderStrip.Text = "How long should a {0} alert remain active?";
            this.agingAlertActiveTimeHeaderStrip.WordWrap = false;
            // 
            // jobAlertActivePanel
            // 
            this.jobAlertActivePanel.Controls.Add(this.flowLayoutPanel1);
            this.jobAlertActivePanel.Controls.Add(this.jobAlertActiveTimeHeaderStrip);
            this.jobAlertActivePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobAlertActivePanel.Location = new System.Drawing.Point(1, 173);
            this.jobAlertActivePanel.Name = "jobAlertActivePanel";
            this.jobAlertActivePanel.Size = new System.Drawing.Size(493, 85);
            this.jobAlertActivePanel.TabIndex = 6;
            this.jobAlertActivePanel.Visible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.label18);
            this.flowLayoutPanel1.Controls.Add(this.suppressionDaysSpinner);
            this.flowLayoutPanel1.Controls.Add(this.label37);
            this.flowLayoutPanel1.Controls.Add(this.jobAlertActiveDuration);
            this.flowLayoutPanel1.Controls.Add(this.label19);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(7, 31);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(479, 48);
            this.flowLayoutPanel1.TabIndex = 59;
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 7);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(164, 13);
            this.label18.TabIndex = 5;
            this.label18.Text = "The alert should remain active for";
            // 
            // suppressionDaysSpinner
            // 
            this.suppressionDaysSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.suppressionDaysSpinner.Location = new System.Drawing.Point(173, 3);
            this.suppressionDaysSpinner.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.suppressionDaysSpinner.Name = "suppressionDaysSpinner";
            this.suppressionDaysSpinner.Size = new System.Drawing.Size(44, 20);
            this.suppressionDaysSpinner.TabIndex = 57;
            // 
            // label37
            // 
            this.label37.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(223, 7);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(37, 13);
            this.label37.TabIndex = 58;
            this.label37.Text = "(Days)";
            // 
            // jobAlertActiveDuration
            // 
            this.jobAlertActiveDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            dropDownEditorButton2.Key = "DropDownList";
            dropDownEditorButton2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.jobAlertActiveDuration.ButtonsRight.Add(dropDownEditorButton2);
            this.jobAlertActiveDuration.DateTime = new System.DateTime(2008, 9, 19, 12, 0, 0, 0);
            this.jobAlertActiveDuration.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.jobAlertActiveDuration.FormatString = "HH:mm";
            this.jobAlertActiveDuration.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.jobAlertActiveDuration.Location = new System.Drawing.Point(266, 3);
            this.jobAlertActiveDuration.MaskInput = "hh:mm";
            this.jobAlertActiveDuration.Name = "jobAlertActiveDuration";
            this.jobAlertActiveDuration.Size = new System.Drawing.Size(77, 21);
            this.jobAlertActiveDuration.TabIndex = 55;
            this.jobAlertActiveDuration.Time = System.TimeSpan.Parse("12:00:00");
            this.jobAlertActiveDuration.Value = new System.DateTime(2008, 9, 19, 12, 0, 0, 0);
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(349, 7);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(105, 13);
            this.label19.TabIndex = 56;
            this.label19.Text = "(Hours and Minutes).";
            // 
            // jobAlertActiveTimeHeaderStrip
            // 
            this.jobAlertActiveTimeHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jobAlertActiveTimeHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.jobAlertActiveTimeHeaderStrip.Location = new System.Drawing.Point(7, 3);
            this.jobAlertActiveTimeHeaderStrip.Name = "jobAlertActiveTimeHeaderStrip";
            this.jobAlertActiveTimeHeaderStrip.Size = new System.Drawing.Size(479, 25);
            this.jobAlertActiveTimeHeaderStrip.TabIndex = 4;
            this.jobAlertActiveTimeHeaderStrip.Text = "How long should a {0} alert remain active?";
            this.jobAlertActiveTimeHeaderStrip.WordWrap = false;
            // 
            // jobFailureSuppressionPanel
            // 
            this.jobFailureSuppressionPanel.Controls.Add(this.raiseJobFailureAlertLimitedRadioButton);
            this.jobFailureSuppressionPanel.Controls.Add(this.raiseJobFailureAlertAnytimeRadioButton);
            this.jobFailureSuppressionPanel.Controls.Add(this.propertiesHeaderStrip12);
            this.jobFailureSuppressionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobFailureSuppressionPanel.Location = new System.Drawing.Point(1, 86);
            this.jobFailureSuppressionPanel.Name = "jobFailureSuppressionPanel";
            this.jobFailureSuppressionPanel.Size = new System.Drawing.Size(493, 87);
            this.jobFailureSuppressionPanel.TabIndex = 5;
            this.jobFailureSuppressionPanel.Visible = false;
            // 
            // raiseJobFailureAlertLimitedRadioButton
            // 
            this.raiseJobFailureAlertLimitedRadioButton.AutoSize = true;
            this.raiseJobFailureAlertLimitedRadioButton.Location = new System.Drawing.Point(25, 57);
            this.raiseJobFailureAlertLimitedRadioButton.Name = "raiseJobFailureAlertLimitedRadioButton";
            this.raiseJobFailureAlertLimitedRadioButton.Size = new System.Drawing.Size(224, 17);
            this.raiseJobFailureAlertLimitedRadioButton.TabIndex = 3;
            this.raiseJobFailureAlertLimitedRadioButton.Text = "Only if the most recent job execution failed";
            this.raiseJobFailureAlertLimitedRadioButton.UseVisualStyleBackColor = true;
            // 
            // raiseJobFailureAlertAnytimeRadioButton
            // 
            this.raiseJobFailureAlertAnytimeRadioButton.AutoSize = true;
            this.raiseJobFailureAlertAnytimeRadioButton.Checked = true;
            this.raiseJobFailureAlertAnytimeRadioButton.Location = new System.Drawing.Point(25, 34);
            this.raiseJobFailureAlertAnytimeRadioButton.Name = "raiseJobFailureAlertAnytimeRadioButton";
            this.raiseJobFailureAlertAnytimeRadioButton.Size = new System.Drawing.Size(154, 17);
            this.raiseJobFailureAlertAnytimeRadioButton.TabIndex = 2;
            this.raiseJobFailureAlertAnytimeRadioButton.TabStop = true;
            this.raiseJobFailureAlertAnytimeRadioButton.Text = "Anytime a job failure occurs";
            this.raiseJobFailureAlertAnytimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // propertiesHeaderStrip12
            // 
            this.propertiesHeaderStrip12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip12.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip12.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip12.Name = "propertiesHeaderStrip12";
            this.propertiesHeaderStrip12.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip12.TabIndex = 1;
            this.propertiesHeaderStrip12.Text = "When should a job failure alert be raised?";
            this.propertiesHeaderStrip12.WordWrap = false;
            // 
            // timeSuppressionPanel
            // 
            this.timeSuppressionPanel.Controls.Add(this.flowLayoutPanel2);
            this.timeSuppressionPanel.Controls.Add(this.smoothingPropertiesHeaderStrip1);
            this.timeSuppressionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.timeSuppressionPanel.Location = new System.Drawing.Point(1, 1);
            this.timeSuppressionPanel.Name = "timeSuppressionPanel";
            this.timeSuppressionPanel.Size = new System.Drawing.Size(493, 85);
            this.timeSuppressionPanel.TabIndex = 4;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.suppressionMinutesSpinner);
            this.flowLayoutPanel2.Controls.Add(this.label3);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(7, 31);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(479, 48);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "A metric threshold should be exceeded for ";
            // 
            // suppressionMinutesSpinner
            // 
            this.suppressionMinutesSpinner.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.suppressionMinutesSpinner.AutoSize = true;
            this.suppressionMinutesSpinner.Location = new System.Drawing.Point(217, 3);
            this.suppressionMinutesSpinner.Name = "suppressionMinutesSpinner";
            this.suppressionMinutesSpinner.Size = new System.Drawing.Size(41, 20);
            this.suppressionMinutesSpinner.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "minutes before an alert is raised.";
            // 
            // smoothingPropertiesHeaderStrip1
            // 
            this.smoothingPropertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.smoothingPropertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.smoothingPropertiesHeaderStrip1.Location = new System.Drawing.Point(7, 3);
            this.smoothingPropertiesHeaderStrip1.Name = "smoothingPropertiesHeaderStrip1";
            this.smoothingPropertiesHeaderStrip1.Size = new System.Drawing.Size(479, 25);
            this.smoothingPropertiesHeaderStrip1.TabIndex = 0;
            this.smoothingPropertiesHeaderStrip1.Text = "How long should a metric exceed its configured threshold before raising an alert?" +
                "";
            this.smoothingPropertiesHeaderStrip1.WordWrap = false;
            // 
            // ultraTabPageControl8
            // 
            this.ultraTabPageControl8.Controls.Add(this.office2007PropertyPage7);
            this.ultraTabPageControl8.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl8.Name = "ultraTabPageControl8";
            this.ultraTabPageControl8.Size = new System.Drawing.Size(600, 595);  
            // 
            // office2007PropertyPage7
            // 
            this.office2007PropertyPage7.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage7.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage7.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage7.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.logSizeLimitPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.driveFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.tablesFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.logScanRegularExpressionPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.logScanPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.sessionUsersFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.sessionHostServersFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.sessionApplicationsFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.sessionExludeDMappFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.jobCategoriesFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.jobsFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.filegroupsFilterPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.FilterOptionPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.databasesFilterPanel);
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.readOnlyDatabaseIncludePanel);
            //SQLDM-29041 -- Add availability group alert options.
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.availabilityGroupBackupAlertPanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.replicaCollectionOptionPanel);

            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.versionStorePanel);
            this.office2007PropertyPage7.ContentPanel.Controls.Add(this.versionStoreSizePanel);
            this.office2007PropertyPage7.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage7.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage7.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage7.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage7.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage7.ContentPanel.Size = new System.Drawing.Size(600, 440);
            this.office2007PropertyPage7.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage7.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage7.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage7.Name = "office2007PropertyPage7";
            this.office2007PropertyPage7.Size = new System.Drawing.Size(600, 495);
            this.office2007PropertyPage7.TabIndex = 1;
            this.office2007PropertyPage7.Text = "Filter out objects for which you don\'t wish to receive alerts.";
            this.office2007PropertyPage7.ContentPanel.AutoScrollMargin = new System.Drawing.Size(10, 0);
            this.office2007PropertyPage7.ContentPanel.HorizontalScroll.Maximum = 0;
            this.office2007PropertyPage7.ContentPanel.AutoScroll = false;
            this.office2007PropertyPage7.ContentPanel.VerticalScroll.Visible = false;
            this.office2007PropertyPage7.ContentPanel.AutoScroll = true;
            // 
            // logSizeLimitPanel
            // 
            this.logSizeLimitPanel.Controls.Add(this.panel21);
            this.logSizeLimitPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.logSizeLimitPanel.Location = new System.Drawing.Point(1, 1608);
            this.logSizeLimitPanel.Name = "logSizeLimitPanel";
            this.logSizeLimitPanel.Size = new System.Drawing.Size(493, 55); // 493,68
            this.logSizeLimitPanel.TabIndex = 17;
            // 
            // panel21
            // 
            this.panel21.Controls.Add(this.spnLogSize);
            this.panel21.Controls.Add(this.lblLogSize);
            this.panel21.Controls.Add(this.propertiesHeaderStrip20);
            this.panel21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel21.Location = new System.Drawing.Point(0, 0);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(450, 30); // 493,68 
            this.panel21.TabIndex = 5;
            // 
            // spnLogSize
            // 
            this.spnLogSize.Location = new System.Drawing.Point(100, 33);
            this.spnLogSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.spnLogSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnLogSize.Name = "spnLogSize";
            this.spnLogSize.Size = new System.Drawing.Size(56, 20);
            this.spnLogSize.TabIndex = 5;
            this.spnLogSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblLogSize
            // 
            this.lblLogSize.AutoSize = true;
            this.lblLogSize.Location = new System.Drawing.Point(28, 36);
            this.lblLogSize.Name = "lblLogSize";
            this.lblLogSize.Size = new System.Drawing.Size(55, 10); // 55, 13
            this.lblLogSize.TabIndex = 4;
            this.lblLogSize.Text = "Size (MB):";
            // 
            // propertiesHeaderStrip20
            // 
            this.propertiesHeaderStrip20.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip20.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip20.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip20.Name = "propertiesHeaderStrip20";
            this.propertiesHeaderStrip20.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip20.TabIndex = 1;
            this.propertiesHeaderStrip20.Text = "What size would you like to limit your log to?";
            this.propertiesHeaderStrip20.WordWrap = false;
            // 
            // driveFilterPanel
            // 
            this.driveFilterPanel.Controls.Add(this.panel19);
            this.driveFilterPanel.Controls.Add(this.panel20);
            this.driveFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.driveFilterPanel.Location = new System.Drawing.Point(1, 1533);
            this.driveFilterPanel.Name = "driveFilterPanel";
            this.driveFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.driveFilterPanel.TabIndex = 16;
            this.driveFilterPanel.Visible = false;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.propertiesHeaderStrip16);
            this.panel19.Controls.Add(this.drivesExcludeFilterTextBox);
            this.panel19.Controls.Add(this.filterDrivesButton);
            this.panel19.Controls.Add(this.label29);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel19.Location = new System.Drawing.Point(0, 0);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(493, 55);
            this.panel19.TabIndex = 5;
            // 
            // propertiesHeaderStrip16
            // 
            this.propertiesHeaderStrip16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip16.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip16.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip16.Name = "propertiesHeaderStrip16";
            this.propertiesHeaderStrip16.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip16.TabIndex = 1;
            this.propertiesHeaderStrip16.Text = "Which disk drives would you like to exclude from alerts?";
            this.propertiesHeaderStrip16.WordWrap = false;
            // 
            // drivesExcludeFilterTextBox
            // 
            this.drivesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.drivesExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.drivesExcludeFilterTextBox.Multiline = true;
            this.drivesExcludeFilterTextBox.Name = "drivesExcludeFilterTextBox";
            this.drivesExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.drivesExcludeFilterTextBox.TabIndex = 2;
            this.drivesExcludeFilterTextBox.TextChanged += new System.EventHandler(this.drivesExcludeFilterTextBox_TextChanged);
            this.drivesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.drivesExcludeFilterTextBox.Resize += new System.EventHandler(this.drivesExcludeFilterTextBox_Resize);
            // 
            // filterDrivesButton
            // 
            this.filterDrivesButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterDrivesButton.Location = new System.Drawing.Point(7, 32);
            this.filterDrivesButton.Name = "filterDrivesButton";
            this.filterDrivesButton.Size = new System.Drawing.Size(75, 23);
            this.filterDrivesButton.TabIndex = 3;
            this.filterDrivesButton.Text = "Exclude...";
            this.filterDrivesButton.UseVisualStyleBackColor = false;
            this.filterDrivesButton.Click += new System.EventHandler(this.filterDrivesButton_Click);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(25, 37);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(48, 13);
            this.label29.TabIndex = 6;
            this.label29.Text = "Exclude:";
            // 
            // panel20
            // 
            this.panel20.Controls.Add(this.label30);
            this.panel20.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel20.Location = new System.Drawing.Point(0, 55);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(493, 20);
            this.panel20.TabIndex = 6;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Dock = System.Windows.Forms.DockStyle.Right;
            this.label30.ForeColor = System.Drawing.Color.DimGray;
            this.label30.Location = new System.Drawing.Point(192, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(301, 13);
            this.label30.TabIndex = 4;
            this.label30.Text = "use semicolons to separate names; wildcards are not permitted";
            // 
            // tablesFilterPanel
            // 
            this.tablesFilterPanel.Controls.Add(this.panel17);
            this.tablesFilterPanel.Controls.Add(this.panel18);
            this.tablesFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tablesFilterPanel.Location = new System.Drawing.Point(1, 1458);
            this.tablesFilterPanel.Name = "tablesFilterPanel";
            this.tablesFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.tablesFilterPanel.TabIndex = 15;
            this.tablesFilterPanel.Visible = false;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.propertiesHeaderStrip15);
            this.panel17.Controls.Add(this.tablesExcludeFilterTextBox);
            this.panel17.Controls.Add(this.filterTablesButton);
            this.panel17.Controls.Add(this.label27);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel17.Location = new System.Drawing.Point(0, 0);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(493, 55);
            this.panel17.TabIndex = 5;
            // 
            // propertiesHeaderStrip15
            // 
            this.propertiesHeaderStrip15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip15.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip15.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip15.Name = "propertiesHeaderStrip15";
            this.propertiesHeaderStrip15.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip15.TabIndex = 1;
            this.propertiesHeaderStrip15.Text = "Which tables would you like to exclude from alerts?";
            this.propertiesHeaderStrip15.WordWrap = false;
            // 
            // tablesExcludeFilterTextBox
            // 
            this.tablesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tablesExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.tablesExcludeFilterTextBox.Multiline = true;
            this.tablesExcludeFilterTextBox.Name = "tablesExcludeFilterTextBox";
            this.tablesExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.tablesExcludeFilterTextBox.TabIndex = 2;
            this.tablesExcludeFilterTextBox.TextChanged += new System.EventHandler(this.tableExcludeFilterTextBox_TextChanged);
            this.tablesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.tablesExcludeFilterTextBox.Resize += new System.EventHandler(this.tableExcludeFilterTextBox_Resize);
            // 
            // filterTablesButton
            // 
            this.filterTablesButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterTablesButton.Location = new System.Drawing.Point(7, 32);
            this.filterTablesButton.Name = "filterTablesButton";
            this.filterTablesButton.Size = new System.Drawing.Size(75, 23);
            this.filterTablesButton.TabIndex = 3;
            this.filterTablesButton.Text = "Exclude...";
            this.filterTablesButton.UseVisualStyleBackColor = false;
            this.filterTablesButton.Click += new System.EventHandler(this.filterTablesButton_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(25, 37);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(48, 13);
            this.label27.TabIndex = 6;
            this.label27.Text = "Exclude:";
            // 
            // panel18
            // 
            this.panel18.Controls.Add(this.label28);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel18.Location = new System.Drawing.Point(0, 55);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(493, 20);
            this.panel18.TabIndex = 6;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Dock = System.Windows.Forms.DockStyle.Right;
            this.label28.ForeColor = System.Drawing.Color.DimGray;
            this.label28.Location = new System.Drawing.Point(228, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(265, 13);
            this.label28.TabIndex = 4;
            this.label28.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // logScanRegularExpressionPanel
            // 
            this.logScanRegularExpressionPanel.Controls.Add(this.panel15);
            this.logScanRegularExpressionPanel.Controls.Add(this.panel16);
            this.logScanRegularExpressionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.logScanRegularExpressionPanel.Location = new System.Drawing.Point(1, 1251);
            this.logScanRegularExpressionPanel.Name = "logScanRegularExpressionPanel";
            this.logScanRegularExpressionPanel.Size = new System.Drawing.Size(493, 207);
            this.logScanRegularExpressionPanel.TabIndex = 14;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.logScanIncludeTextBoxRegexInfo);
            this.panel15.Controls.Add(this.label34);
            this.panel15.Controls.Add(this.logScanIncludeTextBoxRegexWarning);
            this.panel15.Controls.Add(this.label23);
            this.panel15.Controls.Add(this.logScanIncludeTextBoxRegexCritical);
            this.panel15.Controls.Add(this.label21);
            this.panel15.Controls.Add(this.propertiesHeaderStrip14);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(493, 181);
            this.panel15.TabIndex = 5;
            // 
            // logScanIncludeTextBoxRegexInfo
            // 
            this.logScanIncludeTextBoxRegexInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxRegexInfo.Location = new System.Drawing.Point(105, 131);
            this.logScanIncludeTextBoxRegexInfo.Multiline = true;
            this.logScanIncludeTextBoxRegexInfo.Name = "logScanIncludeTextBoxRegexInfo";
            this.logScanIncludeTextBoxRegexInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScanIncludeTextBoxRegexInfo.Size = new System.Drawing.Size(382, 42);
            this.logScanIncludeTextBoxRegexInfo.TabIndex = 10;
            this.logScanIncludeTextBoxRegexInfo.WordWrap = false;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(12, 131);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(70, 13);
            this.label34.TabIndex = 9;
            this.label34.Text = "Informational:";
            // 
            // logScanIncludeTextBoxRegexWarning
            // 
            this.logScanIncludeTextBoxRegexWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxRegexWarning.Location = new System.Drawing.Point(105, 83);
            this.logScanIncludeTextBoxRegexWarning.Multiline = true;
            this.logScanIncludeTextBoxRegexWarning.Name = "logScanIncludeTextBoxRegexWarning";
            this.logScanIncludeTextBoxRegexWarning.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScanIncludeTextBoxRegexWarning.Size = new System.Drawing.Size(382, 42);
            this.logScanIncludeTextBoxRegexWarning.TabIndex = 8;
            this.logScanIncludeTextBoxRegexWarning.WordWrap = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(32, 83);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(50, 13);
            this.label23.TabIndex = 7;
            this.label23.Text = "Warning:";
            // 
            // logScanIncludeTextBoxRegexCritical
            // 
            this.logScanIncludeTextBoxRegexCritical.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxRegexCritical.Location = new System.Drawing.Point(105, 35);
            this.logScanIncludeTextBoxRegexCritical.Multiline = true;
            this.logScanIncludeTextBoxRegexCritical.Name = "logScanIncludeTextBoxRegexCritical";
            this.logScanIncludeTextBoxRegexCritical.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logScanIncludeTextBoxRegexCritical.Size = new System.Drawing.Size(382, 42);
            this.logScanIncludeTextBoxRegexCritical.TabIndex = 6;
            this.logScanIncludeTextBoxRegexCritical.WordWrap = false;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(41, 35);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(41, 13);
            this.label21.TabIndex = 5;
            this.label21.Text = "Critical:";
            // 
            // propertiesHeaderStrip14
            // 
            this.propertiesHeaderStrip14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip14.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip14.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip14.Name = "propertiesHeaderStrip14";
            this.propertiesHeaderStrip14.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip14.TabIndex = 2;
            this.propertiesHeaderStrip14.Text = "What regular expressions would you like to raise alerts for?";
            this.propertiesHeaderStrip14.WordWrap = false;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.label24);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel16.Location = new System.Drawing.Point(0, 181);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(493, 26);
            this.panel16.TabIndex = 6;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Dock = System.Windows.Forms.DockStyle.Right;
            this.label24.ForeColor = System.Drawing.Color.DimGray;
            this.label24.Location = new System.Drawing.Point(343, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(150, 13);
            this.label24.TabIndex = 5;
            this.label24.Text = "one regular expression per line";
            // 
            // logScanPanel
            // 
            this.logScanPanel.Controls.Add(this.panel9);
            this.logScanPanel.Controls.Add(this.panel12);
            this.logScanPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.logScanPanel.Location = new System.Drawing.Point(1, 1114);
            this.logScanPanel.Name = "logScanPanel";
            this.logScanPanel.Size = new System.Drawing.Size(493, 137);
            this.logScanPanel.TabIndex = 13;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label33);
            this.panel9.Controls.Add(this.logScanIncludeTextBoxInfo);
            this.panel9.Controls.Add(this.label22);
            this.panel9.Controls.Add(this.logScanIncludeTextBoxWarning);
            this.panel9.Controls.Add(this.label20);
            this.panel9.Controls.Add(this.propertiesHeaderStrip13);
            this.panel9.Controls.Add(this.logScanIncludeTextBoxCritical);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(493, 114);
            this.panel9.TabIndex = 5;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(12, 89);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(70, 13);
            this.label33.TabIndex = 8;
            this.label33.Text = "Informational:";
            // 
            // logScanIncludeTextBoxInfo
            // 
            this.logScanIncludeTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxInfo.Location = new System.Drawing.Point(105, 86);
            this.logScanIncludeTextBoxInfo.Name = "logScanIncludeTextBoxInfo";
            this.logScanIncludeTextBoxInfo.Size = new System.Drawing.Size(382, 20);
            this.logScanIncludeTextBoxInfo.TabIndex = 7;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(32, 63);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 13);
            this.label22.TabIndex = 6;
            this.label22.Text = "Warning:";
            this.label22.Click += new System.EventHandler(this.label22_Click);
            // 
            // logScanIncludeTextBoxWarning
            // 
            this.logScanIncludeTextBoxWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxWarning.Location = new System.Drawing.Point(105, 60);
            this.logScanIncludeTextBoxWarning.Name = "logScanIncludeTextBoxWarning";
            this.logScanIncludeTextBoxWarning.Size = new System.Drawing.Size(382, 20);
            this.logScanIncludeTextBoxWarning.TabIndex = 5;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(41, 37);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "Critical:";
            // 
            // propertiesHeaderStrip13
            // 
            this.propertiesHeaderStrip13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip13.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip13.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip13.Name = "propertiesHeaderStrip13";
            this.propertiesHeaderStrip13.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip13.TabIndex = 1;
            this.propertiesHeaderStrip13.Text = "What text would you like to raise alerts for?";
            this.propertiesHeaderStrip13.WordWrap = false;
            this.propertiesHeaderStrip13.Load += new System.EventHandler(this.propertiesHeaderStrip13_Load);
            // 
            // logScanIncludeTextBoxCritical
            // 
            this.logScanIncludeTextBoxCritical.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logScanIncludeTextBoxCritical.Location = new System.Drawing.Point(105, 34);
            this.logScanIncludeTextBoxCritical.Name = "logScanIncludeTextBoxCritical";
            this.logScanIncludeTextBoxCritical.Size = new System.Drawing.Size(382, 20);
            this.logScanIncludeTextBoxCritical.TabIndex = 2;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.logScanDirections);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel12.Location = new System.Drawing.Point(0, 114);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(493, 23);
            this.panel12.TabIndex = 6;
            // 
            // logScanDirections
            // 
            this.logScanDirections.AutoSize = true;
            this.logScanDirections.Dock = System.Windows.Forms.DockStyle.Right;
            this.logScanDirections.ForeColor = System.Drawing.Color.DimGray;
            this.logScanDirections.Location = new System.Drawing.Point(229, 0);
            this.logScanDirections.Name = "logScanDirections";
            this.logScanDirections.Size = new System.Drawing.Size(264, 13);
            this.logScanDirections.TabIndex = 4;
            this.logScanDirections.Text = "use semicolons to separate strings; use % for wildcards";
            // 
            // sessionUsersFilterPanel
            // 
            this.sessionUsersFilterPanel.Controls.Add(this.panel13);
            this.sessionUsersFilterPanel.Controls.Add(this.panel14);
            this.sessionUsersFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sessionUsersFilterPanel.Location = new System.Drawing.Point(1, 1039);
            this.sessionUsersFilterPanel.Name = "sessionUsersFilterPanel";
            this.sessionUsersFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.sessionUsersFilterPanel.TabIndex = 12;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.label14);
            this.panel13.Controls.Add(this.propertiesHeaderStrip11);
            this.panel13.Controls.Add(this.sessionUsersExcludeFilterTextBox);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(493, 55);
            this.panel13.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 37);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Exclude:";
            // 
            // propertiesHeaderStrip11
            // 
            this.propertiesHeaderStrip11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip11.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip11.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip11.Name = "propertiesHeaderStrip11";
            this.propertiesHeaderStrip11.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip11.TabIndex = 1;
            this.propertiesHeaderStrip11.Text = "Which users would you like to exclude from alerts?";
            this.propertiesHeaderStrip11.WordWrap = false;
            // 
            // sessionUsersExcludeFilterTextBox
            // 
            this.sessionUsersExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionUsersExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.sessionUsersExcludeFilterTextBox.Multiline = true;
            this.sessionUsersExcludeFilterTextBox.Name = "sessionUsersExcludeFilterTextBox";
            this.sessionUsersExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.sessionUsersExcludeFilterTextBox.TabIndex = 2;
            this.sessionUsersExcludeFilterTextBox.TextChanged += new System.EventHandler(this.sessionUsersExcludeFilterTextBox_TextChanged);
            this.sessionUsersExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.sessionUsersExcludeFilterTextBox.Resize += new System.EventHandler(this.sessionUsersExcludeFilterTextBox_Resize);
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.label11);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel14.Location = new System.Drawing.Point(0, 55);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(493, 20);
            this.panel14.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Right;
            this.label11.ForeColor = System.Drawing.Color.DimGray;
            this.label11.Location = new System.Drawing.Point(228, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(265, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // sessionHostServersFilterPanel
            // 
            this.sessionHostServersFilterPanel.Controls.Add(this.panel10);
            this.sessionHostServersFilterPanel.Controls.Add(this.panel11);
            this.sessionHostServersFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sessionHostServersFilterPanel.Location = new System.Drawing.Point(1, 964);
            this.sessionHostServersFilterPanel.Name = "sessionHostServersFilterPanel";
            this.sessionHostServersFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.sessionHostServersFilterPanel.TabIndex = 11;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.label13);
            this.panel10.Controls.Add(this.propertiesHeaderStrip10);
            this.panel10.Controls.Add(this.sessionHostServersExcludeFilterTextBox);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(493, 55);
            this.panel10.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(25, 37);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Exclude:";
            // 
            // propertiesHeaderStrip10
            // 
            this.propertiesHeaderStrip10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip10.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip10.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip10.Name = "propertiesHeaderStrip10";
            this.propertiesHeaderStrip10.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip10.TabIndex = 1;
            this.propertiesHeaderStrip10.Text = "Which host servers would you like to exclude from alerts?";
            this.propertiesHeaderStrip10.WordWrap = false;
            // 
            // sessionHostServersExcludeFilterTextBox
            // 
            this.sessionHostServersExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionHostServersExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.sessionHostServersExcludeFilterTextBox.Multiline = true;
            this.sessionHostServersExcludeFilterTextBox.Name = "sessionHostServersExcludeFilterTextBox";
            this.sessionHostServersExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.sessionHostServersExcludeFilterTextBox.TabIndex = 2;
            this.sessionHostServersExcludeFilterTextBox.TextChanged += new System.EventHandler(this.sessionHostServersExcludeFilterTextBox_TextChanged);
            this.sessionHostServersExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.sessionHostServersExcludeFilterTextBox.Resize += new System.EventHandler(this.sessionHostServersExcludeFilterTextBox_Resize);
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.label10);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel11.Location = new System.Drawing.Point(0, 55);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(493, 20);
            this.panel11.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Right;
            this.label10.ForeColor = System.Drawing.Color.DimGray;
            this.label10.Location = new System.Drawing.Point(228, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(265, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // sessionApplicationsFilterPanel
            // 
            this.sessionApplicationsFilterPanel.Controls.Add(this.panel5);
            this.sessionApplicationsFilterPanel.Controls.Add(this.panel8);
            this.sessionApplicationsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sessionApplicationsFilterPanel.Location = new System.Drawing.Point(1, 889);
            this.sessionApplicationsFilterPanel.Name = "sessionApplicationsFilterPanel";
            this.sessionApplicationsFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.sessionApplicationsFilterPanel.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.propertiesHeaderStrip9);
            this.panel5.Controls.Add(this.sessionApplicationsExcludeFilterTextBox);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(493, 55);
            this.panel5.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Exclude:";
            // 
            // propertiesHeaderStrip9
            // 
            this.propertiesHeaderStrip9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip9.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip9.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip9.Name = "propertiesHeaderStrip9";
            this.propertiesHeaderStrip9.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip9.TabIndex = 1;
            this.propertiesHeaderStrip9.Text = "Which applications would you like to exclude from alerts?";
            this.propertiesHeaderStrip9.WordWrap = false;
            // 
            // sessionApplicationsExcludeFilterTextBox
            // 
            this.sessionApplicationsExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionApplicationsExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.sessionApplicationsExcludeFilterTextBox.Multiline = true;
            this.sessionApplicationsExcludeFilterTextBox.Name = "sessionApplicationsExcludeFilterTextBox";
            this.sessionApplicationsExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.sessionApplicationsExcludeFilterTextBox.TabIndex = 2;
            this.sessionApplicationsExcludeFilterTextBox.TextChanged += new System.EventHandler(this.sessionApplicationsExcludeFilterTextBox_TextChanged);
            this.sessionApplicationsExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.sessionApplicationsExcludeFilterTextBox.Resize += new System.EventHandler(this.sessionApplicationsExcludeFilterTextBox_Resize);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 55);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(493, 20);
            this.panel8.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Right;
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(228, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(265, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "use semicolons to separate names; use % for wildcards";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sessionExludeDMappFilterPanel
            // 
            this.sessionExludeDMappFilterPanel.Controls.Add(this.excludeDMappHeaderStrip);
            this.sessionExludeDMappFilterPanel.Controls.Add(this.excludeDMapplicationCheckBox);
            this.sessionExludeDMappFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sessionExludeDMappFilterPanel.Location = new System.Drawing.Point(1, 814);
            this.sessionExludeDMappFilterPanel.Name = "sessionExludeDMappFilterPanel";
            this.sessionExludeDMappFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.sessionExludeDMappFilterPanel.TabIndex = 10;
            // 
            // excludeDMappHeaderStrip
            // 
            this.excludeDMappHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.excludeDMappHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.excludeDMappHeaderStrip.Location = new System.Drawing.Point(11, 3);
            this.excludeDMappHeaderStrip.Name = "excludeDMappHeaderStrip";
            this.excludeDMappHeaderStrip.Size = new System.Drawing.Size(465, 25);
            this.excludeDMappHeaderStrip.TabIndex = 5;
            this.excludeDMappHeaderStrip.Text = "Include SQL Diagnostic Manager queries in the results?";
            this.excludeDMappHeaderStrip.WordWrap = false;
            // 
            // excludeDMapplicationCheckBox
            // 
            this.excludeDMapplicationCheckBox.AutoSize = true;
            this.excludeDMapplicationCheckBox.Location = new System.Drawing.Point(25, 37);
            this.excludeDMapplicationCheckBox.Name = "excludeDMapplicationCheckBox";
            this.excludeDMapplicationCheckBox.Size = new System.Drawing.Size(159, 17);
            this.excludeDMapplicationCheckBox.TabIndex = 10;
            this.excludeDMapplicationCheckBox.Text = "Exclude SQL dm application";
            this.excludeDMapplicationCheckBox.UseVisualStyleBackColor = true;
            //         
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            // includeReadOnlyDatabases
            // 
            this.includeReadOnlyDatabases.AutoSize = true;
            this.includeReadOnlyDatabases.Location = new System.Drawing.Point(25, 37);
            this.includeReadOnlyDatabases.Name = "includeReadOnlyDatabases";
            this.includeReadOnlyDatabases.Size = new System.Drawing.Size(159, 17);
            this.includeReadOnlyDatabases.TabIndex = 10;
            this.includeReadOnlyDatabases.Text = "Include Read-Only Databases";
            this.includeReadOnlyDatabases.UseVisualStyleBackColor = true;
            // 
            // jobCategoriesFilterPanel
            // 
            this.jobCategoriesFilterPanel.Controls.Add(this.panel6);
            this.jobCategoriesFilterPanel.Controls.Add(this.panel7);
            this.jobCategoriesFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobCategoriesFilterPanel.Location = new System.Drawing.Point(1, 739);
            this.jobCategoriesFilterPanel.Name = "jobCategoriesFilterPanel";
            this.jobCategoriesFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.jobCategoriesFilterPanel.TabIndex = 9;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.propertiesHeaderStrip8);
            this.panel6.Controls.Add(this.jobCategoriesExcludeFilterTextBox);
            this.panel6.Controls.Add(this.filterJobCategoriesButton);
            this.panel6.Controls.Add(this.label15);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(493, 55);
            this.panel6.TabIndex = 5;
            // 
            // propertiesHeaderStrip8
            // 
            this.propertiesHeaderStrip8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip8.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip8.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip8.Name = "propertiesHeaderStrip8";
            this.propertiesHeaderStrip8.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip8.TabIndex = 1;
            this.propertiesHeaderStrip8.Text = "Which job categories would you like to exclude from alerts?";
            this.propertiesHeaderStrip8.WordWrap = false;
            // 
            // jobCategoriesExcludeFilterTextBox
            // 
            this.jobCategoriesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jobCategoriesExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.jobCategoriesExcludeFilterTextBox.Multiline = true;
            this.jobCategoriesExcludeFilterTextBox.Name = "jobCategoriesExcludeFilterTextBox";
            this.jobCategoriesExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.jobCategoriesExcludeFilterTextBox.TabIndex = 2;
            this.jobCategoriesExcludeFilterTextBox.TextChanged += new System.EventHandler(this.jobCategoriesExcludeFilterTextBox_TextChanged);
            this.jobCategoriesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.jobCategoriesExcludeFilterTextBox.Resize += new System.EventHandler(this.jobCategoriesExcludeFilterTextBox_Resize);
            // 
            // filterJobCategoriesButton
            // 
            this.filterJobCategoriesButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterJobCategoriesButton.Location = new System.Drawing.Point(7, 32);
            this.filterJobCategoriesButton.Name = "filterJobCategoriesButton";
            this.filterJobCategoriesButton.Size = new System.Drawing.Size(75, 23);
            this.filterJobCategoriesButton.TabIndex = 3;
            this.filterJobCategoriesButton.Text = "Exclude...";
            this.filterJobCategoriesButton.UseVisualStyleBackColor = false;
            this.filterJobCategoriesButton.Click += new System.EventHandler(this.filterJobCategoriesButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 37);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 5;
            this.label15.Text = "Exclude:";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 55);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(493, 20);
            this.panel7.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Right;
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.Location = new System.Drawing.Point(228, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(265, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // jobsFilterPanel
            // 
            this.jobsFilterPanel.Controls.Add(this.panel3);
            this.jobsFilterPanel.Controls.Add(this.panel4);
            this.jobsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobsFilterPanel.Location = new System.Drawing.Point(1, 664);
            this.jobsFilterPanel.Name = "jobsFilterPanel";
            this.jobsFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.jobsFilterPanel.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.propertiesHeaderStrip7);
            this.panel3.Controls.Add(this.jobsExcludeFilterTextBox);
            this.panel3.Controls.Add(this.filterJobsButton);
            this.panel3.Controls.Add(this.label16);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 55);
            this.panel3.TabIndex = 5;
            // 
            // propertiesHeaderStrip7
            // 
            this.propertiesHeaderStrip7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip7.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip7.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip7.Name = "propertiesHeaderStrip7";
            this.propertiesHeaderStrip7.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip7.TabIndex = 1;
            this.propertiesHeaderStrip7.Text = "Which jobs would you like to exclude from alerts?";
            this.propertiesHeaderStrip7.WordWrap = false;
            // 
            // jobsExcludeFilterTextBox
            // 
            this.jobsExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.jobsExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.jobsExcludeFilterTextBox.Multiline = true;
            this.jobsExcludeFilterTextBox.Name = "jobsExcludeFilterTextBox";
            this.jobsExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.jobsExcludeFilterTextBox.TabIndex = 2;
            this.jobsExcludeFilterTextBox.TextChanged += new System.EventHandler(this.jobsExcludeFilterTextBox_TextChanged);
            this.jobsExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.jobsExcludeFilterTextBox.Resize += new System.EventHandler(this.jobsExcludeFilterTextBox_Resize);
            // 
            // filterJobsButton
            // 
            this.filterJobsButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterJobsButton.Location = new System.Drawing.Point(7, 32);
            this.filterJobsButton.Name = "filterJobsButton";
            this.filterJobsButton.Size = new System.Drawing.Size(75, 23);
            this.filterJobsButton.TabIndex = 3;
            this.filterJobsButton.Text = "Exclude...";
            this.filterJobsButton.UseVisualStyleBackColor = false;
            this.filterJobsButton.Click += new System.EventHandler(this.filterJobsButton_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 37);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "Exclude:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 55);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(493, 20);
            this.panel4.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Right;
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(228, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(265, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // databasesFilterPanel
            // 
            this.databasesFilterPanel.Controls.Add(this.databasesExcludeFilterPanel);
            this.databasesFilterPanel.Controls.Add(this.panel2);
            this.databasesFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.databasesFilterPanel.Location = new System.Drawing.Point(1, 589);
            this.databasesFilterPanel.Name = "databasesFilterPanel";
            this.databasesFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.databasesFilterPanel.TabIndex = 7;
            // 
            //readOnlyDatabaseIncludePanel
            // SQLDM-29210 -- Add option to include alerts for Read-only databases.
            //
            // 
            this.readOnlyDatabaseIncludePanel.Controls.Add(this.readOnlyDatabaseIncludeFilterPanel);
            this.readOnlyDatabaseIncludePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.readOnlyDatabaseIncludePanel.Location = new System.Drawing.Point(1, 900);
            this.readOnlyDatabaseIncludePanel.Name = "readOnlyDatabaseIncludePanel";
            this.readOnlyDatabaseIncludePanel.Size = new System.Drawing.Size(493, 55);
            this.readOnlyDatabaseIncludePanel.TabIndex = 7;
            this.readOnlyDatabaseIncludePanel.Visible = false;
            // 
            // readOnlyDatabaseIncludeFilterPanel
            // SQLDM-29210 -- Add option to include alerts for Read-only databases.
            //
            this.readOnlyDatabaseIncludeFilterPanel.Controls.Add(this.includeReadOnlyDatabases);
            this.readOnlyDatabaseIncludeFilterPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.readOnlyDatabaseIncludeFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.readOnlyDatabaseIncludeFilterPanel.Name = "readOnlyDatabaseIncludeFilterPanel";
            this.readOnlyDatabaseIncludeFilterPanel.Size = new System.Drawing.Size(493, 55);
            this.readOnlyDatabaseIncludeFilterPanel.TabIndex = 5;
            //
            // availabilityGroupBackupAlertPanel
            // SQLDM-29041 -- Add availability group alert options.
            //
            // 
            this.availabilityGroupBackupAlertPanel.Controls.Add(this.availabilityGroupBackupAlertFilterPanel);
            this.availabilityGroupBackupAlertPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.availabilityGroupBackupAlertPanel.Location = new System.Drawing.Point(1, 300);
            this.availabilityGroupBackupAlertPanel.Name = "availabilityGroupBackupAlertPanel";
            this.availabilityGroupBackupAlertPanel.Size = new System.Drawing.Size(450, 200);
            this.availabilityGroupBackupAlertPanel.TabIndex = 7;
            this.availabilityGroupBackupAlertPanel.Visible = false;
            this.availabilityGroupBackupAlertPanel.Margin = new System.Windows.Forms.Padding(15);
            
            //
            // replicaCollectionOptionPanel
            // SQLDM-29041 -- Add availability group alert options.
            //
            // 
            this.replicaCollectionOptionPanel.Controls.Add(this.replicaCollectionOptionFilterPanel);
            this.replicaCollectionOptionPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.replicaCollectionOptionPanel.Location = new System.Drawing.Point(1, 200);
            this.replicaCollectionOptionPanel.Name = "replicaCollectionOptionPanel";
            this.replicaCollectionOptionPanel.Size = new System.Drawing.Size(450, 130);
            this.replicaCollectionOptionPanel.TabIndex = 6;
            this.replicaCollectionOptionPanel.Visible = false;
            this.replicaCollectionOptionPanel.Margin = new System.Windows.Forms.Padding(15);
            // 
            // availabilityGroupBackupAlertFilterPanel
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupBackupAlertFilterPanel.Controls.Add(this.availabilityGroupBackupAlertOptions);
            this.availabilityGroupBackupAlertFilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupBackupAlertFilterPanel.Location = new System.Drawing.Point(10, 300);
            this.availabilityGroupBackupAlertFilterPanel.Name = "availabilityGroupBackupAlertFilterPanel";
            this.availabilityGroupBackupAlertFilterPanel.Size = new System.Drawing.Size(450, 200);
            this.availabilityGroupBackupAlertFilterPanel.TabIndex = 5;
            // 
            // replicaCollectionOptionFilterPanel
            // SQLDM-29041 -- Add availability group alert options.
            ////
            this.replicaCollectionOptionFilterPanel.Controls.Add(this.replicaCollectionOptions);
            this.replicaCollectionOptionFilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicaCollectionOptionFilterPanel.Location = new System.Drawing.Point(10, 200);
            this.replicaCollectionOptionFilterPanel.Name = "replicaCollectionOptionFilterPanel";
            this.replicaCollectionOptionFilterPanel.Size = new System.Drawing.Size(450, 130);
            this.replicaCollectionOptionFilterPanel.TabIndex = 6;
            // 
            // availabilityGroupBackupAlertOptions
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupBackupAlertOptions.Controls.Add(this.availabilityGroupPrimaryOption);
            this.availabilityGroupBackupAlertOptions.Controls.Add(this.availabilityGroupSecondaryOption);
            this.availabilityGroupBackupAlertOptions.Controls.Add(this.availabilityGroupBothOption);
            this.availabilityGroupBackupAlertOptions.Controls.Add(this.availabilityGroupDefaultOption);
            this.availabilityGroupBackupAlertOptions.Text = "Availbility Group Collection Options";
            this.availabilityGroupBackupAlertOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupBackupAlertOptions.Location = new System.Drawing.Point(10, 30);
            this.availabilityGroupBackupAlertOptions.Name = "availabilityGroupBackupAlertOptions";
            this.availabilityGroupBackupAlertOptions.Size = new System.Drawing.Size(450, 200);
            this.availabilityGroupBackupAlertOptions.TabIndex = 5;
            // 
            // replicaCollectionOptions
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.replicaCollectionOptions.Controls.Add(this.replicaCollectionMonitoredOnlyOption);
            this.replicaCollectionOptions.Controls.Add(this.replicaCollectionAllOption);
            this.replicaCollectionOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicaCollectionOptions.Location = new System.Drawing.Point(10, 30);
            this.replicaCollectionOptions.Name = "replicaCollectionOptions";
            this.replicaCollectionOptions.Text = "Replica Collection Options";
            this.replicaCollectionOptions.Size = new System.Drawing.Size(450, 130);
            this.replicaCollectionOptions.TabIndex = 5;
            // 
            // availabilityGroupPrimaryOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupPrimaryOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupPrimaryOption.Location = new System.Drawing.Point(10, 30);
            this.availabilityGroupPrimaryOption.Name = "availabilityGroupPrimaryOption";
            this.availabilityGroupPrimaryOption.Size = new System.Drawing.Size(100, 30);
            this.availabilityGroupPrimaryOption.Text = "Primary";
            this.availabilityGroupPrimaryOption.Checked = false;
            this.availabilityGroupPrimaryOption.TabIndex = 5;
            // 
            // replicaCollectionMonitoredOnlyOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.replicaCollectionMonitoredOnlyOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicaCollectionMonitoredOnlyOption.Location = new System.Drawing.Point(10, 30);
            this.replicaCollectionMonitoredOnlyOption.Name = "replicaCollectionMonitoredOnlyOption";
            this.replicaCollectionMonitoredOnlyOption.Size = new System.Drawing.Size(100, 30);
            this.replicaCollectionMonitoredOnlyOption.Text = "MonitoredOnly";
            this.replicaCollectionMonitoredOnlyOption.Checked = false;
            this.replicaCollectionMonitoredOnlyOption.TabIndex = 5;
            // 
            // availabilityGroupSecondaryOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupSecondaryOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupSecondaryOption.Location = new System.Drawing.Point(10, 60);
            this.availabilityGroupSecondaryOption.Name = "availabilityGroupSecondaryOption";
            this.availabilityGroupSecondaryOption.Size = new System.Drawing.Size(100, 30);
            this.availabilityGroupSecondaryOption.Text = "Secondary";
            this.availabilityGroupSecondaryOption.Checked = false;
            this.availabilityGroupSecondaryOption.TabIndex = 5;
            // 
            // replicaCollectionMonitoredOnlyOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.replicaCollectionAllOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replicaCollectionAllOption.Location = new System.Drawing.Point(10, 60);
            this.replicaCollectionAllOption.Name = "replicaCollectionAllOption";
            this.replicaCollectionAllOption.Size = new System.Drawing.Size(100, 30);
            this.replicaCollectionAllOption.Text = "All";
            this.replicaCollectionAllOption.Checked = true;
            this.replicaCollectionAllOption.TabIndex = 5;
            // 
            // availabilityGroupDefaultOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupDefaultOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupDefaultOption.Location = new System.Drawing.Point(10, 90);
            this.availabilityGroupDefaultOption.Name = "availabilityGroupDefaultOption";
            this.availabilityGroupDefaultOption.Size = new System.Drawing.Size(100, 30);
            this.availabilityGroupDefaultOption.Text = "Default";
            this.availabilityGroupDefaultOption.Checked = false;
            this.availabilityGroupDefaultOption.TabIndex = 5;
            // 
            // availabilityGroupBothOption
            // SQLDM-29041 -- Add availability group alert options.
            //
            this.availabilityGroupBothOption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availabilityGroupBothOption.Location = new System.Drawing.Point(10, 120);
            this.availabilityGroupBothOption.Name = "availabilityGroupBothOption";
            this.availabilityGroupBothOption.Size = new System.Drawing.Size(100, 30);
            this.availabilityGroupBothOption.Text = "Both";
            this.availabilityGroupBothOption.Checked = true;
            this.availabilityGroupBothOption.TabIndex = 5;
            // 
            // databasesExcludeFilterPanel
            // 
            this.databasesExcludeFilterPanel.Controls.Add(this.propertiesHeaderStrip1);
            this.databasesExcludeFilterPanel.Controls.Add(this.databasesExcludeFilterTextBox);
            this.databasesExcludeFilterPanel.Controls.Add(this.filterDatabasesButton);
            this.databasesExcludeFilterPanel.Controls.Add(this.label17);
            this.databasesExcludeFilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesExcludeFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.databasesExcludeFilterPanel.Name = "databasesExcludeFilterPanel";
            this.databasesExcludeFilterPanel.Size = new System.Drawing.Size(493, 55);
            this.databasesExcludeFilterPanel.TabIndex = 5;
            // 
            // propertiesHeaderStrip1
            // 
            this.propertiesHeaderStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip1.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip1.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip1.Name = "propertiesHeaderStrip1";
            this.propertiesHeaderStrip1.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip1.TabIndex = 1;
            this.propertiesHeaderStrip1.Text = "Which databases would you like to exclude from alerts?";
            this.propertiesHeaderStrip1.WordWrap = false;
            // 
            // databasesExcludeFilterTextBox
            // 
            this.databasesExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.databasesExcludeFilterTextBox.Multiline = true;
            this.databasesExcludeFilterTextBox.Name = "databasesExcludeFilterTextBox";
            this.databasesExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.databasesExcludeFilterTextBox.TabIndex = 2;
            this.databasesExcludeFilterTextBox.TextChanged += new System.EventHandler(this.databaseExcludeFilterTextBox_TextChanged);
            this.databasesExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.databasesExcludeFilterTextBox.Resize += new System.EventHandler(this.databaseExcludeFilterTextBox_Resize);
            // 
            // filterDatabasesButton
            // 
            this.filterDatabasesButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterDatabasesButton.Location = new System.Drawing.Point(7, 32);
            this.filterDatabasesButton.Name = "filterDatabasesButton";
            this.filterDatabasesButton.Size = new System.Drawing.Size(75, 23);
            this.filterDatabasesButton.TabIndex = 3;
            this.filterDatabasesButton.Text = "Exclude...";
            this.filterDatabasesButton.UseVisualStyleBackColor = false;
            this.filterDatabasesButton.Click += new System.EventHandler(this.filterDatabasesButton_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(25, 37);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(48, 13);
            this.label17.TabIndex = 6;
            this.label17.Text = "Exclude:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 20);
            this.panel2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Right;
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(228, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(265, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "use semicolons to separate names; use % for wildcards";

            this.thresholdHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.thresholdHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.thresholdHeaderStrip.Location = new System.Drawing.Point(7, 10);
            this.thresholdHeaderStrip.Name = "lblThresholdHeader";
            this.thresholdHeaderStrip.Size = new System.Drawing.Size(520, 25);
            this.thresholdHeaderStrip.Text = "Filter by percentage of time over a period exceeding the specific threshold";
            this.thresholdHeaderStrip.WordWrap = false;
           
            this.FilterOptionPanel.Controls.Add(this.thresholdHeaderStrip);
            this.FilterOptionPanel.Controls.Add(this.label42);
            this.FilterOptionPanel.Controls.Add(this.label43);
            this.FilterOptionPanel.Controls.Add(this.FilterOptionNumeric);
            this.FilterOptionPanel.Controls.Add(this.FilterOptionNumericOutOf);
            this.FilterOptionPanel.Controls.Add(this.label41);
            this.FilterOptionPanel.Controls.Add(this.label40);
            this.FilterOptionPanel.Location = new System.Drawing.Point(0, 74);
            this.FilterOptionPanel.Name = "FilterOptionPanel";
            this.FilterOptionPanel.Size = new System.Drawing.Size(520, 85);
            this.FilterOptionPanel.TabIndex = 5;            
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(280, 43);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(34, 13);
            this.label42.TabIndex = 13;
            this.label42.Text = "out of";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(355, 43);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(50, 13);
            this.label43.TabIndex = 13;
            this.label43.Text = "snapshot";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilterOptionNumeric
            // 
            this.FilterOptionNumeric.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FilterOptionNumeric.Location = new System.Drawing.Point(240, 39);
            this.FilterOptionNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FilterOptionNumeric.Name = "FilterOptionNumeric";
            this.FilterOptionNumeric.Size = new System.Drawing.Size(40, 20);
            this.FilterOptionNumeric.TabIndex = 3;
            this.FilterOptionNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FilterOptionNumeric.ValueChanged += new System.EventHandler(this.FilterOptionNumeric_ValueChanged);
            // 
            // FilterOptionNumericOutOf
            // 
            this.FilterOptionNumericOutOf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FilterOptionNumericOutOf.Location = new System.Drawing.Point(315, 39);
            this.FilterOptionNumericOutOf.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FilterOptionNumericOutOf.Name = "FilterOptionNumericOutOf";
            this.FilterOptionNumericOutOf.Size = new System.Drawing.Size(40, 21);
            this.FilterOptionNumericOutOf.TabIndex = 3;
            this.FilterOptionNumericOutOf.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FilterOptionNumericOutOf.ValueChanged += new System.EventHandler(this.FilterOptionNumericOutOf_ValueChanged);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(105, 43);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(139, 13);
            this.label41.TabIndex = 11;
            this.label41.Text = "threshold has been reached";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBox1
          
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(17, 43);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(90, 13);
            this.label40.TabIndex = 6;
            this.label40.Text = "Only create when";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // filegroupsFilterPanel
            // 
            this.filegroupsFilterPanel.Controls.Add(this.filegroupsExcludeFilterPanel);
            this.filegroupsFilterPanel.Controls.Add(this.panel22);
            this.filegroupsFilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filegroupsFilterPanel.Location = new System.Drawing.Point(1, 514);
            this.filegroupsFilterPanel.Name = "filegroupsFilterPanel";
            this.filegroupsFilterPanel.Size = new System.Drawing.Size(493, 75);
            this.filegroupsFilterPanel.Visible = false;
            this.filegroupsFilterPanel.TabIndex = 6;
            // 
            // filegroupsExcludeFilterPanel
            // 
            this.filegroupsExcludeFilterPanel.Controls.Add(this.propertiesHeaderStrip21);
            this.filegroupsExcludeFilterPanel.Controls.Add(this.filegroupsExcludeFilterTextBox);
            this.filegroupsExcludeFilterPanel.Controls.Add(this.filterFilegroupsButton);
            this.filegroupsExcludeFilterPanel.Controls.Add(this.label38);
            this.filegroupsExcludeFilterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filegroupsExcludeFilterPanel.Location = new System.Drawing.Point(0, 0);
            this.filegroupsExcludeFilterPanel.Name = "filegroupsExcludeFilterPanel";
            this.filegroupsExcludeFilterPanel.Size = new System.Drawing.Size(493, 55);
            this.filegroupsExcludeFilterPanel.TabIndex = 4;
            // 
            // propertiesHeaderStrip21
            // 
            this.propertiesHeaderStrip21.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip21.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip21.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip21.Name = "propertiesHeaderStrip21";
            this.propertiesHeaderStrip21.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip21.TabIndex = 1;
            this.propertiesHeaderStrip21.Text = "Which filegroups would you like to exclude from alerts?";
            this.propertiesHeaderStrip21.WordWrap = false;
            // 
            // filegroupsExcludeFilterTextBox
            // 
            this.filegroupsExcludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filegroupsExcludeFilterTextBox.Location = new System.Drawing.Point(88, 34);
            this.filegroupsExcludeFilterTextBox.Multiline = true;
            this.filegroupsExcludeFilterTextBox.Name = "filegroupsExcludeFilterTextBox";
            this.filegroupsExcludeFilterTextBox.Size = new System.Drawing.Size(398, 20);
            this.filegroupsExcludeFilterTextBox.TabIndex = 2;
            this.filegroupsExcludeFilterTextBox.TextChanged += new System.EventHandler(this.filegroupExcludeFilterTextBox_TextChanged);
            this.filegroupsExcludeFilterTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.filegroupsExcludeFilterTextBox.Resize += new System.EventHandler(this.filegroupExcludeFilterTextBox_Resize);
            // 
            // filterFilegroupsButton
            // 
            this.filterFilegroupsButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.filterFilegroupsButton.Location = new System.Drawing.Point(7, 32);
            this.filterFilegroupsButton.Name = "filterFilegroupsButton";
            this.filterFilegroupsButton.Size = new System.Drawing.Size(75, 23);
            this.filterFilegroupsButton.TabIndex = 2;
            this.filterFilegroupsButton.Text = "Exclude...";
            this.filterFilegroupsButton.UseVisualStyleBackColor = false;
            this.filterFilegroupsButton.Click += new System.EventHandler(this.filterFilegroupsButton_Click);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(25, 37);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(48, 13);
            this.label38.TabIndex = 5;
            this.label38.Text = "Exclude:";
            // 
            // panel22
            // 
            this.panel22.Controls.Add(this.label39);
            this.panel22.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel22.Location = new System.Drawing.Point(0, 35);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(493, 20);
            this.panel22.TabIndex = 5;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Dock = System.Windows.Forms.DockStyle.Right;
            this.label39.ForeColor = System.Drawing.Color.DimGray;
            this.label39.Location = new System.Drawing.Point(228, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(265, 13);
            this.label39.TabIndex = 3;
            this.label39.Text = "use semicolons to separate names; use % for wildcards";
            // 
            // versionStorePanel
            // 
            this.versionStorePanel.Controls.Add(this.flowLayoutPanel5);
            this.versionStorePanel.Controls.Add(this.propertiesHeaderStrip19);
            this.versionStorePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.versionStorePanel.Location = new System.Drawing.Point(1, 151);
            this.versionStorePanel.Name = "versionStorePanel";
            this.versionStorePanel.Size = new System.Drawing.Size(493, 438);
            this.versionStorePanel.TabIndex = 6;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.Controls.Add(this.label35);
            this.flowLayoutPanel5.Controls.Add(this.minVersionStoreGenerationRate);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(25, 35);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(389, 34);
            this.flowLayoutPanel5.TabIndex = 4;
            // 
            // label35
            // 
            this.label35.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(3, 6);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(231, 13);
            this.label35.TabIndex = 2;
            this.label35.Text = "Minimum version store generation rate (kb/sec):";
            // 
            // minVersionStoreGenerationRate
            // 
            this.minVersionStoreGenerationRate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minVersionStoreGenerationRate.Location = new System.Drawing.Point(240, 3);
            this.minVersionStoreGenerationRate.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.minVersionStoreGenerationRate.Name = "minVersionStoreGenerationRate";
            this.minVersionStoreGenerationRate.Size = new System.Drawing.Size(70, 20);
            this.minVersionStoreGenerationRate.TabIndex = 3;
            // 
            // propertiesHeaderStrip19
            // 
            this.propertiesHeaderStrip19.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip19.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip19.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip19.Name = "propertiesHeaderStrip19";
            this.propertiesHeaderStrip19.Size = new System.Drawing.Size(479, 25);
            this.propertiesHeaderStrip19.TabIndex = 1;
            this.propertiesHeaderStrip19.Text = "What is the minimum version store generation rate at which this alert should be g" +
                "enerated?";
            this.propertiesHeaderStrip19.WordWrap = false;
            // 
            // versionStoreSizePanel
            // 
            this.versionStoreSizePanel.Controls.Add(this.flowLayoutPanel6);
            this.versionStoreSizePanel.Controls.Add(this.versionStoreSizeHeaderStrip);
            this.versionStoreSizePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.versionStoreSizePanel.Location = new System.Drawing.Point(1, 1);
            this.versionStoreSizePanel.Name = "versionStoreSizePanel";
            this.versionStoreSizePanel.Size = new System.Drawing.Size(493, 150);
            this.versionStoreSizePanel.TabIndex = 7;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel6.Controls.Add(this.label36);
            this.flowLayoutPanel6.Controls.Add(this.versionStoreSizeUpDown);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(25, 35);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(243, 37);
            this.flowLayoutPanel6.TabIndex = 7;
            // 
            // label36
            // 
            this.label36.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(3, 6);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(99, 13);
            this.label36.TabIndex = 5;
            this.label36.Text = "Minimum Size (MB):";
            // 
            // versionStoreSizeUpDown
            // 
            this.versionStoreSizeUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.versionStoreSizeUpDown.Location = new System.Drawing.Point(108, 3);
            this.versionStoreSizeUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.versionStoreSizeUpDown.Name = "versionStoreSizeUpDown";
            this.versionStoreSizeUpDown.Size = new System.Drawing.Size(78, 20);
            this.versionStoreSizeUpDown.TabIndex = 6;
            // 
            // versionStoreSizeHeaderStrip
            // 
            this.versionStoreSizeHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionStoreSizeHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.versionStoreSizeHeaderStrip.Location = new System.Drawing.Point(7, 3);
            this.versionStoreSizeHeaderStrip.Name = "versionStoreSizeHeaderStrip";
            this.versionStoreSizeHeaderStrip.Size = new System.Drawing.Size(479, 25);
            this.versionStoreSizeHeaderStrip.TabIndex = 1;
            this.versionStoreSizeHeaderStrip.Text = "What is the minimum version store size that should be reached before this alert i" +
                "s raised?";
            this.versionStoreSizeHeaderStrip.WordWrap = false;
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Controls.Add(this.office2007PropertyPage5);
            this.ultraTabPageControl6.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl6.Name = "ultraTabPageControl6";
            this.ultraTabPageControl6.Size = new System.Drawing.Size(495, 495);
            // 
            // office2007PropertyPage5
            // 
            this.office2007PropertyPage5.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage5.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage5.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage5.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage5.ContentPanel.Controls.Add(this.propertiesHeaderStrip6);
            this.office2007PropertyPage5.ContentPanel.Controls.Add(this.skipAlertButton);
            this.office2007PropertyPage5.ContentPanel.Controls.Add(this.generateAlertButton);
            this.office2007PropertyPage5.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage5.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage5.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage5.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage5.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage5.ContentPanel.Size = new System.Drawing.Size(495, 440);
            this.office2007PropertyPage5.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage5.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage5.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage5.Name = "office2007PropertyPage5";
            this.office2007PropertyPage5.Size = new System.Drawing.Size(495, 495);
            this.office2007PropertyPage5.TabIndex = 0;
            this.office2007PropertyPage5.Text = "Configure data collection alerts.";
            // 
            // propertiesHeaderStrip6
            // 
            this.propertiesHeaderStrip6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip6.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip6.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip6.Name = "propertiesHeaderStrip6";
            this.propertiesHeaderStrip6.Size = new System.Drawing.Size(481, 25);
            this.propertiesHeaderStrip6.TabIndex = 8;
            this.propertiesHeaderStrip6.Text = "Should an alert be generated if a custom counter fails to be collected?";
            this.propertiesHeaderStrip6.WordWrap = false;
            // 
            // skipAlertButton
            // 
            this.skipAlertButton.AutoSize = true;
            this.skipAlertButton.Location = new System.Drawing.Point(28, 57);
            this.skipAlertButton.Name = "skipAlertButton";
            this.skipAlertButton.Size = new System.Drawing.Size(348, 17);
            this.skipAlertButton.TabIndex = 7;
            this.skipAlertButton.Text = "No, do not generate an alert if a custom counter cannot be collected";
            this.skipAlertButton.UseVisualStyleBackColor = true;
            // 
            // generateAlertButton
            // 
            this.generateAlertButton.AutoSize = true;
            this.generateAlertButton.Checked = true;
            this.generateAlertButton.Location = new System.Drawing.Point(28, 34);
            this.generateAlertButton.Name = "generateAlertButton";
            this.generateAlertButton.Size = new System.Drawing.Size(319, 17);
            this.generateAlertButton.TabIndex = 6;
            this.generateAlertButton.TabStop = true;
            this.generateAlertButton.Text = "Yes, generate an alert if a custom counter cannot be collected";
            this.generateAlertButton.UseVisualStyleBackColor = true;
            // 
            // ultraTabPageControl12
            // 
            this.ultraTabPageControl12.Controls.Add(this.office2007PropertyPage8);
            this.ultraTabPageControl12.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl12.Name = "ultraTabPageControl12";
            this.ultraTabPageControl12.Size = new System.Drawing.Size(495, 495);
            // 
            // office2007PropertyPage8
            // 
            this.office2007PropertyPage8.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage8.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage8.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage8.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage8.ContentPanel.Controls.Add(this.propertiesHeaderStrip17);
            this.office2007PropertyPage8.ContentPanel.Controls.Add(this.ignoreAutogrow);
            this.office2007PropertyPage8.ContentPanel.Controls.Add(this.useAutogrow);
            this.office2007PropertyPage8.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage8.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage8.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage8.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage8.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage8.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage8.ContentPanel.Size = new System.Drawing.Size(495, 440);
            this.office2007PropertyPage8.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage8.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage8.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage8.Name = "office2007PropertyPage8";
            this.office2007PropertyPage8.Size = new System.Drawing.Size(495, 495);
            this.office2007PropertyPage8.TabIndex = 0;
            this.office2007PropertyPage8.Text = "Specify whether to consider autogrow in calculations.";
            //
            // propertiesHeaderStrip17
            // 
            this.propertiesHeaderStrip17.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip17.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip17.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip17.Name = "propertiesHeaderStrip17";
            this.propertiesHeaderStrip17.Size = new System.Drawing.Size(481, 25);
            this.propertiesHeaderStrip17.TabIndex = 11;
            this.propertiesHeaderStrip17.Text = "Should autogrow be considered when calculating size remaining?";
            this.propertiesHeaderStrip17.WordWrap = false;
            // 
            // ignoreAutogrow
            // 
            this.ignoreAutogrow.AutoSize = true;
            this.ignoreAutogrow.Location = new System.Drawing.Point(28, 57);
            this.ignoreAutogrow.Name = "ignoreAutogrow";
            this.ignoreAutogrow.Size = new System.Drawing.Size(326, 17);
            this.ignoreAutogrow.TabIndex = 10;
            this.ignoreAutogrow.Text = "No, alert on the current used size divided by the current file size.";
            this.ignoreAutogrow.UseVisualStyleBackColor = true;
            // 
            // useAutogrow
            // 
            this.useAutogrow.AutoSize = true;
            this.useAutogrow.Checked = true;
            this.useAutogrow.Location = new System.Drawing.Point(28, 34);
            this.useAutogrow.Name = "useAutogrow";
            this.useAutogrow.Size = new System.Drawing.Size(365, 17);
            this.useAutogrow.TabIndex = 9;
            this.useAutogrow.TabStop = true;
            this.useAutogrow.Text = "Yes, alert on the current used size divided by the maximum possible size.";
            this.useAutogrow.UseVisualStyleBackColor = true;
            // 
            // ultraTabPageControl13
            // 
            this.ultraTabPageControl13.Controls.Add(this.office2007PropertyPage9);
            this.ultraTabPageControl13.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl13.Name = "ultraTabPageControl13";
            this.ultraTabPageControl13.Size = new System.Drawing.Size(495, 495);           
            // 
            // office2007PropertyPage9
            // 
            this.office2007PropertyPage9.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage9.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage9.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage9.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.minRuntimeJobsPanel);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.removeExclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.editExclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.addExclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.clearExclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.label32);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.gridExclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.removeInclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.editInclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.addInclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.clearInclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.label31);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.gridInclude);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.chkAlertOnJobSteps);
            this.office2007PropertyPage9.ContentPanel.Controls.Add(this.propertiesHeaderStrip18);
            this.office2007PropertyPage9.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage9.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage9.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage9.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage9.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage9.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage9.ContentPanel.Size = new System.Drawing.Size(495, 440);
            this.office2007PropertyPage9.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage9.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FilterLarge;
            this.office2007PropertyPage9.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage9.Name = "office2007PropertyPage9";
            this.office2007PropertyPage9.Size = new System.Drawing.Size(495, 495);
            this.office2007PropertyPage9.TabIndex = 0;
            this.office2007PropertyPage9.Text = "Filter out jobs or job steps you do not want to receive alerts for";            
            // 
            // minRuntimeJobsPanel
            // 
            this.minRuntimeJobsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.minRuntimeJobsPanel.BackColor = System.Drawing.Color.Transparent;
            this.minRuntimeJobsPanel.Controls.Add(this.flowLayoutPanel4);
            this.minRuntimeJobsPanel.Controls.Add(this.minLongJobRuntimeHeaderStrip);
            this.minRuntimeJobsPanel.Location = new System.Drawing.Point(7, 375);
            this.minRuntimeJobsPanel.Name = "minRuntimeJobsPanel";
            this.minRuntimeJobsPanel.Size = new System.Drawing.Size(481, 68);
            this.minRuntimeJobsPanel.TabIndex = 30;
            this.minRuntimeJobsPanel.Visible = false;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel4.Controls.Add(this.minlRuntimeUnit0Label);
            this.flowLayoutPanel4.Controls.Add(this.minimalRuntimeUpDown);
            this.flowLayoutPanel4.Controls.Add(this.minlRuntimeUnit1Label);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 32);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(481, 31);
            this.flowLayoutPanel4.TabIndex = 34;
            // 
            // minlRuntimeUnit0Label
            // 
            this.minlRuntimeUnit0Label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minlRuntimeUnit0Label.AutoSize = true;
            this.minlRuntimeUnit0Label.Location = new System.Drawing.Point(3, 6);
            this.minlRuntimeUnit0Label.Name = "minlRuntimeUnit0Label";
            this.minlRuntimeUnit0Label.Size = new System.Drawing.Size(222, 13);
            this.minlRuntimeUnit0Label.TabIndex = 33;
            this.minlRuntimeUnit0Label.Text = "Do not raise an alert until it has run for at least";
            // 
            // minimalRuntimeUpDown
            // 
            this.minimalRuntimeUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minimalRuntimeUpDown.AutoSize = true;
            this.minimalRuntimeUpDown.Location = new System.Drawing.Point(231, 3);
            this.minimalRuntimeUpDown.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.minimalRuntimeUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.minimalRuntimeUpDown.Name = "minimalRuntimeUpDown";
            this.minimalRuntimeUpDown.Size = new System.Drawing.Size(47, 20);
            this.minimalRuntimeUpDown.TabIndex = 30;
            this.minimalRuntimeUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // minlRuntimeUnit1Label
            // 
            this.minlRuntimeUnit1Label.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.minlRuntimeUnit1Label.AutoSize = true;
            this.minlRuntimeUnit1Label.Location = new System.Drawing.Point(284, 6);
            this.minlRuntimeUnit1Label.Name = "minlRuntimeUnit1Label";
            this.minlRuntimeUnit1Label.Size = new System.Drawing.Size(148, 13);
            this.minlRuntimeUnit1Label.TabIndex = 31;
            this.minlRuntimeUnit1Label.Text = "seconds. (max 1800 seconds)";
            // 
            // minLongJobRuntimeHeaderStrip
            // 
            this.minLongJobRuntimeHeaderStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.minLongJobRuntimeHeaderStrip.ForeColor = System.Drawing.Color.Black;
            this.minLongJobRuntimeHeaderStrip.Location = new System.Drawing.Point(-3, 1);
            this.minLongJobRuntimeHeaderStrip.Name = "minLongJobRuntimeHeaderStrip";
            this.minLongJobRuntimeHeaderStrip.Size = new System.Drawing.Size(488, 25);
            this.minLongJobRuntimeHeaderStrip.TabIndex = 32;
            this.minLongJobRuntimeHeaderStrip.TabStop = false;
            this.minLongJobRuntimeHeaderStrip.Text = "How long should a job run before raising an alert?";
            this.minLongJobRuntimeHeaderStrip.WordWrap = false;
            // 
            // removeExclude
            // 
            this.removeExclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeExclude.AutoSize = true;
            this.removeExclude.Enabled = false;
            this.removeExclude.Location = new System.Drawing.Point(402, 340);
            this.removeExclude.Name = "removeExclude";
            this.removeExclude.Size = new System.Drawing.Size(80, 25);
            this.removeExclude.TabIndex = 24;
            this.removeExclude.Text = "Remove";
            this.removeExclude.UseVisualStyleBackColor = true;
            this.removeExclude.Click += new System.EventHandler(this.Remove_Click);
            // 
            // editExclude
            // 
            this.editExclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editExclude.AutoSize = true;
            this.editExclude.Enabled = false;
            this.editExclude.Location = new System.Drawing.Point(321, 340);
            this.editExclude.Name = "editExclude";
            this.editExclude.Size = new System.Drawing.Size(80, 25);
            this.editExclude.TabIndex = 23;
            this.editExclude.Text = "Edit";
            this.editExclude.UseVisualStyleBackColor = true;
            this.editExclude.Click += new System.EventHandler(this.Edit_Click);
            // 
            // addExclude
            // 
            this.addExclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addExclude.AutoSize = true;
            this.addExclude.Location = new System.Drawing.Point(240, 340);
            this.addExclude.Name = "addExclude";
            this.addExclude.Size = new System.Drawing.Size(80, 25);
            this.addExclude.TabIndex = 22;
            this.addExclude.Text = "Add";
            this.addExclude.UseVisualStyleBackColor = true;
            this.addExclude.Click += new System.EventHandler(this.Add_Click);
            // 
            // clearExclude
            // 
            this.clearExclude.AutoSize = true;
            this.clearExclude.Location = new System.Drawing.Point(7, 340);
            this.clearExclude.Name = "clearExclude";
            this.clearExclude.Size = new System.Drawing.Size(80, 25);
            this.clearExclude.TabIndex = 21;
            this.clearExclude.Text = "Remove All";
            this.clearExclude.UseVisualStyleBackColor = true;
            this.clearExclude.Visible = false;
            this.clearExclude.Click += new System.EventHandler(this.Clear_Click);
            // 
            // label32
            // 
            this.label32.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(14, 216);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(52, 13);
            this.label32.TabIndex = 20;
            this.label32.Text = "Exclude";
            // 
            // gridExclude
            // 
            this.gridExclude.AllowUserToAddRows = false;
            this.gridExclude.AllowUserToDeleteRows = false;
            this.gridExclude.AllowUserToResizeRows = false;
            this.gridExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridExclude.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridExclude.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridExclude.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridExclude.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridExclude.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridExclude.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridExclude.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.excludeCatOpCode,
            this.excludeCategory,
            this.excludeJobOpCode,
            this.excludeJobName,
            this.excludeStepOpCode,
            this.excludeStepName});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridExclude.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridExclude.GridColor = System.Drawing.SystemColors.Window;
            this.gridExclude.Location = new System.Drawing.Point(7, 242);
            this.gridExclude.MultiSelect = false;
            this.gridExclude.Name = "gridExclude";
            this.gridExclude.ReadOnly = true;
            this.gridExclude.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridExclude.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridExclude.RowTemplate.ReadOnly = true;
            this.gridExclude.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gridExclude.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridExclude.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridExclude.Size = new System.Drawing.Size(474, 92);
            this.gridExclude.StandardTab = true;
            this.gridExclude.TabIndex = 19;
            this.gridExclude.SelectionChanged += new System.EventHandler(this.gridExclude_SelectionChanged);
            // 
            // excludeCatOpCode
            // 
            this.excludeCatOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeCatOpCode.HeaderText = "Operator";
            this.excludeCatOpCode.Name = "excludeCatOpCode";
            this.excludeCatOpCode.ReadOnly = true;
            this.excludeCatOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // excludeCategory
            // 
            this.excludeCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeCategory.HeaderText = "Category";
            this.excludeCategory.Name = "excludeCategory";
            this.excludeCategory.ReadOnly = true;
            this.excludeCategory.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // excludeJobOpCode
            // 
            this.excludeJobOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeJobOpCode.HeaderText = "Operator";
            this.excludeJobOpCode.Name = "excludeJobOpCode";
            this.excludeJobOpCode.ReadOnly = true;
            this.excludeJobOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // excludeJobName
            // 
            this.excludeJobName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeJobName.HeaderText = "Job Name";
            this.excludeJobName.Name = "excludeJobName";
            this.excludeJobName.ReadOnly = true;
            this.excludeJobName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // excludeStepOpCode
            // 
            this.excludeStepOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeStepOpCode.HeaderText = "Operator";
            this.excludeStepOpCode.Name = "excludeStepOpCode";
            this.excludeStepOpCode.ReadOnly = true;
            this.excludeStepOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // excludeStepName
            // 
            this.excludeStepName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.excludeStepName.HeaderText = "Step Name";
            this.excludeStepName.Name = "excludeStepName";
            this.excludeStepName.ReadOnly = true;
            this.excludeStepName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // removeInclude
            // 
            this.removeInclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeInclude.AutoSize = true;
            this.removeInclude.Enabled = false;
            this.removeInclude.Location = new System.Drawing.Point(402, 188);
            this.removeInclude.Name = "removeInclude";
            this.removeInclude.Size = new System.Drawing.Size(80, 25);
            this.removeInclude.TabIndex = 18;
            this.removeInclude.Text = "Remove";
            this.removeInclude.UseVisualStyleBackColor = true;
            this.removeInclude.Click += new System.EventHandler(this.Remove_Click);
            // 
            // editInclude
            // 
            this.editInclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editInclude.AutoSize = true;
            this.editInclude.Enabled = false;
            this.editInclude.Location = new System.Drawing.Point(321, 188);
            this.editInclude.Name = "editInclude";
            this.editInclude.Size = new System.Drawing.Size(80, 25);
            this.editInclude.TabIndex = 17;
            this.editInclude.Text = "Edit";
            this.editInclude.UseVisualStyleBackColor = true;
            this.editInclude.Click += new System.EventHandler(this.Edit_Click);
            // 
            // addInclude
            // 
            this.addInclude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addInclude.AutoSize = true;
            this.addInclude.Location = new System.Drawing.Point(240, 188);
            this.addInclude.Name = "addInclude";
            this.addInclude.Size = new System.Drawing.Size(80, 25);
            this.addInclude.TabIndex = 16;
            this.addInclude.Text = "Add";
            this.addInclude.UseVisualStyleBackColor = true;
            this.addInclude.Click += new System.EventHandler(this.Add_Click);
            // 
            // clearInclude
            // 
            this.clearInclude.Location = new System.Drawing.Point(7, 181);
            this.clearInclude.Name = "clearInclude";
            this.clearInclude.Size = new System.Drawing.Size(80, 25);
            this.clearInclude.TabIndex = 15;
            this.clearInclude.Text = "Remove All";
            this.clearInclude.UseVisualStyleBackColor = true;
            this.clearInclude.Visible = false;
            this.clearInclude.Click += new System.EventHandler(this.Clear_Click);
            // 
            // label31
            // 
            this.label31.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(14, 63);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(49, 13);
            this.label31.TabIndex = 14;
            this.label31.Text = "Include";
            // 
            // gridInclude
            // 
            this.gridInclude.AllowUserToAddRows = false;
            this.gridInclude.AllowUserToDeleteRows = false;
            this.gridInclude.AllowUserToResizeRows = false;
            this.gridInclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridInclude.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridInclude.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridInclude.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridInclude.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridInclude.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridInclude.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridInclude.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.includeCatOpCode,
            this.includeCategory,
            this.includeJobOpCode,
            this.includeJobName,
            this.includeStepOpCode,
            this.includeStepName});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridInclude.DefaultCellStyle = dataGridViewCellStyle5;
            this.gridInclude.GridColor = System.Drawing.SystemColors.Window;
            this.gridInclude.Location = new System.Drawing.Point(7, 89);
            this.gridInclude.MultiSelect = false;
            this.gridInclude.Name = "gridInclude";
            this.gridInclude.ReadOnly = true;
            this.gridInclude.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridInclude.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.gridInclude.RowTemplate.ReadOnly = true;
            this.gridInclude.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gridInclude.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridInclude.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInclude.Size = new System.Drawing.Size(474, 92);
            this.gridInclude.StandardTab = true;
            this.gridInclude.TabIndex = 13;
            this.gridInclude.SelectionChanged += new System.EventHandler(this.gridInclude_SelectionChanged);
            // 
            // includeCatOpCode
            // 
            this.includeCatOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeCatOpCode.HeaderText = "Operator";
            this.includeCatOpCode.Name = "includeCatOpCode";
            this.includeCatOpCode.ReadOnly = true;
            this.includeCatOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // includeCategory
            // 
            this.includeCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeCategory.HeaderText = "Category";
            this.includeCategory.Name = "includeCategory";
            this.includeCategory.ReadOnly = true;
            this.includeCategory.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // includeJobOpCode
            // 
            this.includeJobOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeJobOpCode.HeaderText = "Operator";
            this.includeJobOpCode.Name = "includeJobOpCode";
            this.includeJobOpCode.ReadOnly = true;
            this.includeJobOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // includeJobName
            // 
            this.includeJobName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeJobName.HeaderText = "Job Name";
            this.includeJobName.Name = "includeJobName";
            this.includeJobName.ReadOnly = true;
            this.includeJobName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // includeStepOpCode
            // 
            this.includeStepOpCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeStepOpCode.HeaderText = "Operator";
            this.includeStepOpCode.Name = "includeStepOpCode";
            this.includeStepOpCode.ReadOnly = true;
            this.includeStepOpCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // includeStepName
            // 
            this.includeStepName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.includeStepName.HeaderText = "Step Name";
            this.includeStepName.Name = "includeStepName";
            this.includeStepName.ReadOnly = true;
            this.includeStepName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // chkAlertOnJobSteps
            // 
            this.chkAlertOnJobSteps.AutoSize = true;
            this.chkAlertOnJobSteps.Location = new System.Drawing.Point(28, 34);
            this.chkAlertOnJobSteps.Name = "chkAlertOnJobSteps";
            this.chkAlertOnJobSteps.Size = new System.Drawing.Size(154, 17);
            this.chkAlertOnJobSteps.TabIndex = 12;
            this.chkAlertOnJobSteps.Text = "Alert on individual job steps";
            this.chkAlertOnJobSteps.UseVisualStyleBackColor = true;
            this.chkAlertOnJobSteps.CheckedChanged += new System.EventHandler(this.chkAlertOnJobSteps_CheckedChanged);
            // 
            // propertiesHeaderStrip18
            // 
            this.propertiesHeaderStrip18.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip18.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip18.Location = new System.Drawing.Point(7, 3);
            this.propertiesHeaderStrip18.Name = "propertiesHeaderStrip18";
            this.propertiesHeaderStrip18.Size = new System.Drawing.Size(481, 25);
            this.propertiesHeaderStrip18.TabIndex = 11;
            this.propertiesHeaderStrip18.TabStop = false;
            this.propertiesHeaderStrip18.Text = "Should individual job steps be evaluated for alert conditions?";
            this.propertiesHeaderStrip18.WordWrap = false;
            // 
            // ultraTabPageControl9
            // 
            this.ultraTabPageControl9.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl9.Name = "ultraTabPageControl9";
            this.ultraTabPageControl9.Size = new System.Drawing.Size(498, 433);
            // 
            // ultraTabPageControl10
            // 
            this.ultraTabPageControl10.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl10.Name = "ultraTabPageControl10";
            this.ultraTabPageControl10.Size = new System.Drawing.Size(498, 433);
            // 
            // ultraTabPageControl11
            // 
            this.ultraTabPageControl11.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl11.Name = "ultraTabPageControl11";
            this.ultraTabPageControl11.Size = new System.Drawing.Size(498, 433);
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Controls.Add(this.office2007PropertyPage4);
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(498, 433);
            // 
            // office2007PropertyPage4
            // 
            this.office2007PropertyPage4.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage4.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage4.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage4.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage4.ContentPanel.Controls.Add(this.label7);
            this.office2007PropertyPage4.ContentPanel.Controls.Add(this.excludeSelfBlockingCheckBox);
            this.office2007PropertyPage4.ContentPanel.Controls.Add(this.button3);
            this.office2007PropertyPage4.ContentPanel.Controls.Add(this.blockingTypesListView);
            this.office2007PropertyPage4.ContentPanel.Controls.Add(this.propertiesHeaderStrip4);
            this.office2007PropertyPage4.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage4.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage4.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage4.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage4.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage4.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage4.ContentPanel.Size = new System.Drawing.Size(498, 378);
            this.office2007PropertyPage4.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage4.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage4.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage4.Name = "office2007PropertyPage4";
            this.office2007PropertyPage4.Size = new System.Drawing.Size(498, 433);
            this.office2007PropertyPage4.TabIndex = 0;
            this.office2007PropertyPage4.Text = "Customize blocking sessions to be excluded for this alert";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 120);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Blocking Types:";
            // 
            // excludeSelfBlockingCheckBox
            // 
            this.excludeSelfBlockingCheckBox.AutoSize = true;
            this.excludeSelfBlockingCheckBox.Location = new System.Drawing.Point(60, 52);
            this.excludeSelfBlockingCheckBox.Name = "excludeSelfBlockingCheckBox";
            this.excludeSelfBlockingCheckBox.Size = new System.Drawing.Size(245, 17);
            this.excludeSelfBlockingCheckBox.TabIndex = 10;
            this.excludeSelfBlockingCheckBox.Text = "Exclude sessions that are blocking themselves";
            this.excludeSelfBlockingCheckBox.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(60, 302);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Clear All";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // blockingTypesListView
            // 
            this.blockingTypesListView.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            ultraListViewItem1.Key = "1";
            ultraListViewItem2.Key = "2";
            ultraListViewItem3.Key = "3 ";
            this.blockingTypesListView.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem1,
            ultraListViewItem2,
            ultraListViewItem3});
            this.blockingTypesListView.Location = new System.Drawing.Point(60, 138);
            this.blockingTypesListView.Name = "blockingTypesListView";
            this.blockingTypesListView.Size = new System.Drawing.Size(366, 158);
            this.blockingTypesListView.TabIndex = 9;
            this.blockingTypesListView.Text = "ultraListView1";
            this.blockingTypesListView.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.blockingTypesListView.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.blockingTypesListView.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.blockingTypesListView.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.blockingTypesListView.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.blockingTypesListView.ViewSettingsList.MultiColumn = false;
            // 
            // propertiesHeaderStrip4
            // 
            this.propertiesHeaderStrip4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip4.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip4.Location = new System.Drawing.Point(6, 3);
            this.propertiesHeaderStrip4.Name = "propertiesHeaderStrip4";
            this.propertiesHeaderStrip4.Size = new System.Drawing.Size(484, 25);
            this.propertiesHeaderStrip4.TabIndex = 1;
            this.propertiesHeaderStrip4.Text = "Blocking Poop";
            this.propertiesHeaderStrip4.WordWrap = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(494, 515);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(574, 515);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.office2007PropertyPage1);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(498, 433);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(498, 378);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(498, 433);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Customize databases to be excluded for this alert";
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.office2007PropertyPage2);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(498, 433);
            // 
            // office2007PropertyPage2
            // 
            this.office2007PropertyPage2.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage2.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage2.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage2.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.excludedJobNamesListBox);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.availableJobNamesStatusLabel);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.removeJobNameExclusionButton);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.addJobNameExclusionButton);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.label5);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.label6);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.informationBox1);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.adhocJobNameTextBox);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.propertiesHeaderStrip2);
            this.office2007PropertyPage2.ContentPanel.Controls.Add(this.availableJobNamesListBox);
            this.office2007PropertyPage2.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage2.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage2.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage2.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage2.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage2.ContentPanel.Size = new System.Drawing.Size(498, 378);
            this.office2007PropertyPage2.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage2.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage2.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage2.Name = "office2007PropertyPage2";
            this.office2007PropertyPage2.Size = new System.Drawing.Size(498, 433);
            this.office2007PropertyPage2.TabIndex = 0;
            this.office2007PropertyPage2.Text = "Customize jobs names to be excluded for this alert";
            // 
            // excludedJobNamesListBox
            // 
            this.excludedJobNamesListBox.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.excludedJobNamesListBox.ItemSettings.HideSelection = false;
            this.excludedJobNamesListBox.Location = new System.Drawing.Point(292, 95);
            this.excludedJobNamesListBox.MainColumn.DataType = typeof(string);
            this.excludedJobNamesListBox.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.excludedJobNamesListBox.Name = "excludedJobNamesListBox";
            this.excludedJobNamesListBox.Size = new System.Drawing.Size(186, 261);
            this.excludedJobNamesListBox.TabIndex = 29;
            this.excludedJobNamesListBox.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.excludedJobNamesListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.excludedJobNamesListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.excludedJobNamesListBox.ViewSettingsList.MultiColumn = false;
            // 
            // availableJobNamesStatusLabel
            // 
            this.availableJobNamesStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableJobNamesStatusLabel.Location = new System.Drawing.Point(21, 122);
            this.availableJobNamesStatusLabel.Name = "availableJobNamesStatusLabel";
            this.availableJobNamesStatusLabel.Size = new System.Drawing.Size(186, 232);
            this.availableJobNamesStatusLabel.TabIndex = 28;
            this.availableJobNamesStatusLabel.Text = "< Unavailable >";
            this.availableJobNamesStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // removeJobNameExclusionButton
            // 
            this.removeJobNameExclusionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeJobNameExclusionButton.Location = new System.Drawing.Point(218, 237);
            this.removeJobNameExclusionButton.Name = "removeJobNameExclusionButton";
            this.removeJobNameExclusionButton.Size = new System.Drawing.Size(66, 24);
            this.removeJobNameExclusionButton.TabIndex = 27;
            this.removeJobNameExclusionButton.Text = "< Remove";
            this.removeJobNameExclusionButton.UseVisualStyleBackColor = true;
            // 
            // addJobNameExclusionButton
            // 
            this.addJobNameExclusionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addJobNameExclusionButton.Location = new System.Drawing.Point(218, 207);
            this.addJobNameExclusionButton.Name = "addJobNameExclusionButton";
            this.addJobNameExclusionButton.Size = new System.Drawing.Size(66, 24);
            this.addJobNameExclusionButton.TabIndex = 26;
            this.addJobNameExclusionButton.Text = "Add >";
            this.addJobNameExclusionButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(289, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Excluded Jobs:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Available Jobs:";
            // 
            // informationBox1
            // 
            this.informationBox1.Location = new System.Drawing.Point(20, 33);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(424, 35);
            this.informationBox1.TabIndex = 21;
            this.informationBox1.Text = "Job Name Info";
            // 
            // adhocJobNameTextBox
            // 
            this.adhocJobNameTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            this.adhocJobNameTextBox.Location = new System.Drawing.Point(20, 95);
            this.adhocJobNameTextBox.Name = "adhocJobNameTextBox";
            this.adhocJobNameTextBox.Size = new System.Drawing.Size(188, 20);
            this.adhocJobNameTextBox.TabIndex = 20;
            this.adhocJobNameTextBox.Text = "< Type semicolon separated names >";
            // 
            // propertiesHeaderStrip2
            // 
            this.propertiesHeaderStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip2.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip2.Location = new System.Drawing.Point(6, 3);
            this.propertiesHeaderStrip2.Name = "propertiesHeaderStrip2";
            this.propertiesHeaderStrip2.Size = new System.Drawing.Size(484, 25);
            this.propertiesHeaderStrip2.TabIndex = 1;
            this.propertiesHeaderStrip2.Text = "Job Poop";
            this.propertiesHeaderStrip2.WordWrap = false;
            // 
            // availableJobNamesListBox
            // 
            this.availableJobNamesListBox.ItemSettings.HideSelection = false;
            this.availableJobNamesListBox.Location = new System.Drawing.Point(20, 120);
            this.availableJobNamesListBox.MainColumn.DataType = typeof(string);
            this.availableJobNamesListBox.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.availableJobNamesListBox.Name = "availableJobNamesListBox";
            this.availableJobNamesListBox.Size = new System.Drawing.Size(189, 235);
            this.availableJobNamesListBox.TabIndex = 30;
            this.availableJobNamesListBox.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.availableJobNamesListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.availableJobNamesListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.availableJobNamesListBox.ViewSettingsList.MultiColumn = false;
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.office2007PropertyPage3);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(523, 447);
            // 
            // office2007PropertyPage3
            // 
            this.office2007PropertyPage3.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage3.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage3.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage3.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.informationBox3);
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.userNameGrid);
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.programNameGrid);
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.hostNameGrid);
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.appNameGrid);
            this.office2007PropertyPage3.ContentPanel.Controls.Add(this.propertiesHeaderStrip3);
            this.office2007PropertyPage3.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage3.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage3.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage3.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage3.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage3.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage3.ContentPanel.Size = new System.Drawing.Size(523, 392);
            this.office2007PropertyPage3.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage3.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage3.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage3.Name = "office2007PropertyPage3";
            this.office2007PropertyPage3.Size = new System.Drawing.Size(523, 447);
            this.office2007PropertyPage3.TabIndex = 0;
            this.office2007PropertyPage3.Text = "Customize sessions to be excluded for this alert";
            // 
            // informationBox3
            // 
            this.informationBox3.Location = new System.Drawing.Point(17, 30);
            this.informationBox3.Name = "informationBox3";
            this.informationBox3.Size = new System.Drawing.Size(424, 35);
            this.informationBox3.TabIndex = 13;
            this.informationBox3.Text = "Database Info";
            // 
            // userNameGrid
            // 
            appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.userNameGrid.DisplayLayout.Appearance = appearance1;
            this.userNameGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.userNameGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.userNameGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.userNameGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.userNameGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.userNameGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.userNameGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.userNameGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.userNameGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.userNameGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTopWithTabRepeat;
            this.userNameGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.userNameGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.userNameGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.userNameGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.userNameGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.userNameGrid.DisplayLayout.Override.CellAppearance = appearance6;
            appearance7.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance7.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.userNameGrid.DisplayLayout.Override.CellButtonAppearance = appearance7;
            this.userNameGrid.DisplayLayout.Override.CellPadding = 0;
            this.userNameGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.userNameGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.userNameGrid.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.userNameGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance9.TextHAlignAsString = "Left";
            this.userNameGrid.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.userNameGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.userNameGrid.DisplayLayout.Override.RowAppearance = appearance10;
            this.userNameGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.userNameGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.userNameGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.userNameGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance11.BackColor = System.Drawing.Color.White;
            this.userNameGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.userNameGrid.DisplayLayout.Override.TemplateAddRowPrompt = "[ Click here to add a new user name ]";
            appearance12.ForeColor = System.Drawing.Color.DarkGray;
            this.userNameGrid.DisplayLayout.Override.TemplateAddRowPromptAppearance = appearance12;
            this.userNameGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.userNameGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.userNameGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList1.Key = "ThresholdTypeItems";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = 0;
            valueListItem1.DisplayText = "OK";
            valueListItem2.DataValue = 1;
            valueListItem2.DisplayText = "Warning";
            valueListItem3.DataValue = 2;
            valueListItem3.DisplayText = "Critical";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "RadioButtons";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem4.Appearance = appearance13;
            valueListItem4.DataValue = false;
            appearance14.Image = ((object)(resources.GetObject("appearance14.Image")));
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem5.Appearance = appearance14;
            valueListItem5.DataValue = true;
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5});
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList3.Key = "CheckBoxes";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem6.Appearance = appearance15;
            valueListItem6.DataValue = false;
            appearance16.Image = ((object)(resources.GetObject("appearance16.Image")));
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem7.Appearance = appearance16;
            valueListItem7.DataValue = true;
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7});
            this.userNameGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.userNameGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.userNameGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.userNameGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userNameGrid.Location = new System.Drawing.Point(252, 221);
            this.userNameGrid.Name = "userNameGrid";
            this.userNameGrid.Size = new System.Drawing.Size(215, 147);
            this.userNameGrid.TabIndex = 9;
            this.userNameGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            // 
            // programNameGrid
            // 
            appearance17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance17.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.programNameGrid.DisplayLayout.Appearance = appearance17;
            this.programNameGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.programNameGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance18.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance18.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.programNameGrid.DisplayLayout.GroupByBox.Appearance = appearance18;
            appearance19.ForeColor = System.Drawing.SystemColors.GrayText;
            this.programNameGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance19;
            this.programNameGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.programNameGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance20.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance20.BackColor2 = System.Drawing.SystemColors.Control;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance20.ForeColor = System.Drawing.SystemColors.GrayText;
            this.programNameGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance20;
            this.programNameGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.programNameGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.programNameGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTopWithTabRepeat;
            this.programNameGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.programNameGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.programNameGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.programNameGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            this.programNameGrid.DisplayLayout.Override.CardAreaAppearance = appearance21;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            appearance22.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.programNameGrid.DisplayLayout.Override.CellAppearance = appearance22;
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance23.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.programNameGrid.DisplayLayout.Override.CellButtonAppearance = appearance23;
            this.programNameGrid.DisplayLayout.Override.CellPadding = 0;
            this.programNameGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.programNameGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance24.BackColor = System.Drawing.SystemColors.Control;
            appearance24.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance24.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance24.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance24.BorderColor = System.Drawing.SystemColors.Window;
            this.programNameGrid.DisplayLayout.Override.GroupByRowAppearance = appearance24;
            this.programNameGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance25.TextHAlignAsString = "Left";
            this.programNameGrid.DisplayLayout.Override.HeaderAppearance = appearance25;
            this.programNameGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance26.BackColor = System.Drawing.SystemColors.Window;
            appearance26.BorderColor = System.Drawing.Color.Silver;
            this.programNameGrid.DisplayLayout.Override.RowAppearance = appearance26;
            this.programNameGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.programNameGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.programNameGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.programNameGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance27.BackColor = System.Drawing.Color.White;
            this.programNameGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance27;
            this.programNameGrid.DisplayLayout.Override.TemplateAddRowPrompt = "[ Click here to add a new program ]";
            appearance28.ForeColor = System.Drawing.Color.DarkGray;
            this.programNameGrid.DisplayLayout.Override.TemplateAddRowPromptAppearance = appearance28;
            this.programNameGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.programNameGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.programNameGrid.DisplayLayout.UseFixedHeaders = true;
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList4.Key = "ThresholdTypeItems";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem8.DataValue = 0;
            valueListItem8.DisplayText = "OK";
            valueListItem9.DataValue = 1;
            valueListItem9.DisplayText = "Warning";
            valueListItem10.DataValue = 2;
            valueListItem10.DisplayText = "Critical";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem8,
            valueListItem9,
            valueListItem10});
            valueList5.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList5.Key = "RadioButtons";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance29.Image = ((object)(resources.GetObject("appearance29.Image")));
            appearance29.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem11.Appearance = appearance29;
            valueListItem11.DataValue = false;
            appearance30.Image = ((object)(resources.GetObject("appearance30.Image")));
            appearance30.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem12.Appearance = appearance30;
            valueListItem12.DataValue = true;
            valueList5.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem11,
            valueListItem12});
            valueList6.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList6.Key = "CheckBoxes";
            valueList6.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance31.Image = ((object)(resources.GetObject("appearance31.Image")));
            appearance31.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem13.Appearance = appearance31;
            valueListItem13.DataValue = false;
            appearance32.Image = ((object)(resources.GetObject("appearance32.Image")));
            appearance32.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem14.Appearance = appearance32;
            valueListItem14.DataValue = true;
            valueList6.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem13,
            valueListItem14});
            this.programNameGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList4,
            valueList5,
            valueList6});
            this.programNameGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.programNameGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.programNameGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.programNameGrid.Location = new System.Drawing.Point(252, 71);
            this.programNameGrid.Name = "programNameGrid";
            this.programNameGrid.Size = new System.Drawing.Size(215, 133);
            this.programNameGrid.TabIndex = 8;
            this.programNameGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            // 
            // hostNameGrid
            // 
            appearance33.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance33.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.hostNameGrid.DisplayLayout.Appearance = appearance33;
            this.hostNameGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.hostNameGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance34.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance34.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance34.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance34.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.hostNameGrid.DisplayLayout.GroupByBox.Appearance = appearance34;
            appearance35.ForeColor = System.Drawing.SystemColors.GrayText;
            this.hostNameGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance35;
            this.hostNameGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.hostNameGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance36.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance36.BackColor2 = System.Drawing.SystemColors.Control;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance36.ForeColor = System.Drawing.SystemColors.GrayText;
            this.hostNameGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance36;
            this.hostNameGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.hostNameGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.hostNameGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTopWithTabRepeat;
            this.hostNameGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.hostNameGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.hostNameGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.hostNameGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance37.BackColor = System.Drawing.SystemColors.Window;
            this.hostNameGrid.DisplayLayout.Override.CardAreaAppearance = appearance37;
            appearance38.BorderColor = System.Drawing.Color.Silver;
            appearance38.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.hostNameGrid.DisplayLayout.Override.CellAppearance = appearance38;
            appearance39.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance39.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.hostNameGrid.DisplayLayout.Override.CellButtonAppearance = appearance39;
            this.hostNameGrid.DisplayLayout.Override.CellPadding = 0;
            this.hostNameGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.hostNameGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance40.BackColor = System.Drawing.SystemColors.Control;
            appearance40.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance40.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance40.BorderColor = System.Drawing.SystemColors.Window;
            this.hostNameGrid.DisplayLayout.Override.GroupByRowAppearance = appearance40;
            this.hostNameGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance41.TextHAlignAsString = "Left";
            this.hostNameGrid.DisplayLayout.Override.HeaderAppearance = appearance41;
            this.hostNameGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance42.BackColor = System.Drawing.SystemColors.Window;
            appearance42.BorderColor = System.Drawing.Color.Silver;
            this.hostNameGrid.DisplayLayout.Override.RowAppearance = appearance42;
            this.hostNameGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.hostNameGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.hostNameGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.hostNameGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance43.BackColor = System.Drawing.Color.White;
            this.hostNameGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance43;
            this.hostNameGrid.DisplayLayout.Override.TemplateAddRowPrompt = "[ Click here to add a new host name ]";
            appearance44.ForeColor = System.Drawing.Color.DarkGray;
            this.hostNameGrid.DisplayLayout.Override.TemplateAddRowPromptAppearance = appearance44;
            this.hostNameGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.hostNameGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.hostNameGrid.DisplayLayout.UseFixedHeaders = true;
            valueList7.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList7.Key = "ThresholdTypeItems";
            valueList7.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem15.DataValue = 0;
            valueListItem15.DisplayText = "OK";
            valueListItem16.DataValue = 1;
            valueListItem16.DisplayText = "Warning";
            valueListItem17.DataValue = 2;
            valueListItem17.DisplayText = "Critical";
            valueList7.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem15,
            valueListItem16,
            valueListItem17});
            valueList8.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList8.Key = "RadioButtons";
            valueList8.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance45.Image = ((object)(resources.GetObject("appearance45.Image")));
            appearance45.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem18.Appearance = appearance45;
            valueListItem18.DataValue = false;
            appearance46.Image = ((object)(resources.GetObject("appearance46.Image")));
            appearance46.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem19.Appearance = appearance46;
            valueListItem19.DataValue = true;
            valueList8.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem18,
            valueListItem19});
            valueList9.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList9.Key = "CheckBoxes";
            valueList9.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance47.Image = ((object)(resources.GetObject("appearance47.Image")));
            appearance47.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem20.Appearance = appearance47;
            valueListItem20.DataValue = false;
            appearance48.Image = ((object)(resources.GetObject("appearance48.Image")));
            appearance48.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem21.Appearance = appearance48;
            valueListItem21.DataValue = true;
            valueList9.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem20,
            valueListItem21});
            this.hostNameGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList7,
            valueList8,
            valueList9});
            this.hostNameGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.hostNameGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.hostNameGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hostNameGrid.Location = new System.Drawing.Point(18, 221);
            this.hostNameGrid.Name = "hostNameGrid";
            this.hostNameGrid.Size = new System.Drawing.Size(215, 147);
            this.hostNameGrid.TabIndex = 7;
            this.hostNameGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            // 
            // appNameGrid
            // 
            appearance49.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance49.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.appNameGrid.DisplayLayout.Appearance = appearance49;
            this.appNameGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.appNameGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance50.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance50.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance50.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.appNameGrid.DisplayLayout.GroupByBox.Appearance = appearance50;
            appearance51.ForeColor = System.Drawing.SystemColors.GrayText;
            this.appNameGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance51;
            this.appNameGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.appNameGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance52.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance52.BackColor2 = System.Drawing.SystemColors.Control;
            appearance52.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance52.ForeColor = System.Drawing.SystemColors.GrayText;
            this.appNameGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance52;
            this.appNameGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.appNameGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.appNameGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTopWithTabRepeat;
            this.appNameGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
            this.appNameGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.appNameGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.appNameGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance53.BackColor = System.Drawing.SystemColors.Window;
            this.appNameGrid.DisplayLayout.Override.CardAreaAppearance = appearance53;
            appearance54.BorderColor = System.Drawing.Color.Silver;
            appearance54.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.appNameGrid.DisplayLayout.Override.CellAppearance = appearance54;
            appearance55.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance55.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.appNameGrid.DisplayLayout.Override.CellButtonAppearance = appearance55;
            this.appNameGrid.DisplayLayout.Override.CellPadding = 0;
            this.appNameGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.appNameGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance56.BackColor = System.Drawing.SystemColors.Control;
            appearance56.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance56.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance56.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance56.BorderColor = System.Drawing.SystemColors.Window;
            this.appNameGrid.DisplayLayout.Override.GroupByRowAppearance = appearance56;
            this.appNameGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance57.TextHAlignAsString = "Left";
            this.appNameGrid.DisplayLayout.Override.HeaderAppearance = appearance57;
            this.appNameGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance58.BackColor = System.Drawing.SystemColors.Window;
            appearance58.BorderColor = System.Drawing.Color.Silver;
            this.appNameGrid.DisplayLayout.Override.RowAppearance = appearance58;
            this.appNameGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.appNameGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.appNameGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.appNameGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance59.BackColor = System.Drawing.Color.White;
            this.appNameGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance59;
            this.appNameGrid.DisplayLayout.Override.TemplateAddRowPrompt = "[ Click here to add a new application ]";
            appearance60.ForeColor = System.Drawing.Color.DarkGray;
            this.appNameGrid.DisplayLayout.Override.TemplateAddRowPromptAppearance = appearance60;
            this.appNameGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.appNameGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.appNameGrid.DisplayLayout.UseFixedHeaders = true;
            valueList10.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList10.Key = "ThresholdTypeItems";
            valueList10.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem22.DataValue = 0;
            valueListItem22.DisplayText = "OK";
            valueListItem23.DataValue = 1;
            valueListItem23.DisplayText = "Warning";
            valueListItem24.DataValue = 2;
            valueListItem24.DisplayText = "Critical";
            valueList10.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem22,
            valueListItem23,
            valueListItem24});
            valueList11.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList11.Key = "RadioButtons";
            valueList11.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance61.Image = ((object)(resources.GetObject("appearance61.Image")));
            appearance61.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem25.Appearance = appearance61;
            valueListItem25.DataValue = false;
            appearance62.Image = ((object)(resources.GetObject("appearance62.Image")));
            appearance62.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem26.Appearance = appearance62;
            valueListItem26.DataValue = true;
            valueList11.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem25,
            valueListItem26});
            valueList12.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList12.Key = "CheckBoxes";
            valueList12.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance63.Image = ((object)(resources.GetObject("appearance63.Image")));
            appearance63.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem27.Appearance = appearance63;
            valueListItem27.DataValue = false;
            appearance64.Image = ((object)(resources.GetObject("appearance64.Image")));
            appearance64.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem28.Appearance = appearance64;
            valueListItem28.DataValue = true;
            valueList12.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem27,
            valueListItem28});
            this.appNameGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList10,
            valueList11,
            valueList12});
            this.appNameGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.appNameGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.appNameGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appNameGrid.Location = new System.Drawing.Point(18, 71);
            this.appNameGrid.Name = "appNameGrid";
            this.appNameGrid.Size = new System.Drawing.Size(215, 133);
            this.appNameGrid.TabIndex = 6;
            this.appNameGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            // 
            // propertiesHeaderStrip3
            // 
            this.propertiesHeaderStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip3.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip3.Location = new System.Drawing.Point(6, 3);
            this.propertiesHeaderStrip3.Name = "propertiesHeaderStrip3";
            this.propertiesHeaderStrip3.Size = new System.Drawing.Size(509, 25);
            this.propertiesHeaderStrip3.TabIndex = 1;
            this.propertiesHeaderStrip3.Text = "Session Poop";
            this.propertiesHeaderStrip3.WordWrap = false;
            // 
            // ultraTabPageControl7
            // 
            this.ultraTabPageControl7.Controls.Add(this.office2007PropertyPage6);
            this.ultraTabPageControl7.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl7.Name = "ultraTabPageControl7";
            this.ultraTabPageControl7.Size = new System.Drawing.Size(498, 433);
            // 
            // office2007PropertyPage6
            // 
            this.office2007PropertyPage6.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage6.BorderWidth = 0;
            // 
            // 
            // 
            this.office2007PropertyPage6.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage6.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage6.ContentPanel.Controls.Add(this.informationBox4);
            this.office2007PropertyPage6.ContentPanel.Controls.Add(this.jobCategoriesClearButton);
            this.office2007PropertyPage6.ContentPanel.Controls.Add(this.jobCategoriesListBox);
            this.office2007PropertyPage6.ContentPanel.Controls.Add(this.propertiesHeaderStrip5);
            this.office2007PropertyPage6.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage6.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage6.ContentPanel.Location = new System.Drawing.Point(0, 55);
            this.office2007PropertyPage6.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage6.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage6.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage6.ContentPanel.Size = new System.Drawing.Size(498, 378);
            this.office2007PropertyPage6.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage6.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GeneralProperties;
            this.office2007PropertyPage6.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage6.Name = "office2007PropertyPage6";
            this.office2007PropertyPage6.Size = new System.Drawing.Size(498, 433);
            this.office2007PropertyPage6.TabIndex = 1;
            this.office2007PropertyPage6.Text = "Customize jobs categories to be excluded for this alert";
            // 
            // informationBox4
            // 
            this.informationBox4.Location = new System.Drawing.Point(32, 34);
            this.informationBox4.Name = "informationBox4";
            this.informationBox4.Size = new System.Drawing.Size(429, 35);
            this.informationBox4.TabIndex = 13;
            this.informationBox4.Text = "Database Info";
            // 
            // jobCategoriesClearButton
            // 
            this.jobCategoriesClearButton.Location = new System.Drawing.Point(32, 343);
            this.jobCategoriesClearButton.Name = "jobCategoriesClearButton";
            this.jobCategoriesClearButton.Size = new System.Drawing.Size(72, 23);
            this.jobCategoriesClearButton.TabIndex = 6;
            this.jobCategoriesClearButton.Text = "Clear All";
            this.jobCategoriesClearButton.UseVisualStyleBackColor = true;
            // 
            // jobCategoriesListBox
            // 
            this.jobCategoriesListBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            ultraListViewItem4.Key = "1";
            ultraListViewItem5.Key = "2";
            ultraListViewItem6.Key = "3 ";
            this.jobCategoriesListBox.Items.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewItem[] {
            ultraListViewItem4,
            ultraListViewItem5,
            ultraListViewItem6});
            this.jobCategoriesListBox.Location = new System.Drawing.Point(32, 75);
            this.jobCategoriesListBox.Name = "jobCategoriesListBox";
            this.jobCategoriesListBox.Size = new System.Drawing.Size(429, 262);
            this.jobCategoriesListBox.TabIndex = 7;
            this.jobCategoriesListBox.Text = "ultraListView1";
            this.jobCategoriesListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.jobCategoriesListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.jobCategoriesListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.jobCategoriesListBox.ViewSettingsList.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            this.jobCategoriesListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.jobCategoriesListBox.ViewSettingsList.MultiColumn = false;
            // 
            // propertiesHeaderStrip5
            // 
            this.propertiesHeaderStrip5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertiesHeaderStrip5.ForeColor = System.Drawing.Color.Black;
            this.propertiesHeaderStrip5.Location = new System.Drawing.Point(6, 3);
            this.propertiesHeaderStrip5.Name = "propertiesHeaderStrip5";
            this.propertiesHeaderStrip5.Size = new System.Drawing.Size(484, 25);
            this.propertiesHeaderStrip5.TabIndex = 1;
            this.propertiesHeaderStrip5.Text = "Custom Poop";
            this.propertiesHeaderStrip5.WordWrap = false;
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gradientPanel1.BackColor = System.Drawing.Color.White;
            this.gradientPanel1.BackColor2 = System.Drawing.Color.White;
            this.gradientPanel1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.gradientPanel1.Controls.Add(this.tabControl);
            this.gradientPanel1.Location = new System.Drawing.Point(12, 12);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.gradientPanel1.Size = new System.Drawing.Size(680, 497);
            this.gradientPanel1.TabIndex = 4;           
            // 
            // tabControl
            // 
            appearance65.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.SelectedTabBackground;
            appearance65.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance65.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.tabControl.ActiveTabAppearance = appearance65;
            appearance66.BackColor = System.Drawing.Color.Transparent;
            appearance66.TextHAlignAsString = "Left";
            this.tabControl.Appearance = appearance66;
            appearance67.BackColor = System.Drawing.Color.DarkGray;
            this.tabControl.ClientAreaAppearance = appearance67;
            this.tabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabControl.Controls.Add(this.ultraTabPageControl1);
            this.tabControl.Controls.Add(this.ultraTabPageControl6);
            this.tabControl.Controls.Add(this.ultraTabPageControl8);
            this.tabControl.Controls.Add(this.ultraTabPageControl12);
            this.tabControl.Controls.Add(this.ultraTabPageControl13);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance68.ImageBackground = global::Idera.SQLdm.DesktopClient.Properties.Resources.HotTrackTabBackground;
            appearance68.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 1);
            appearance68.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.tabControl.HotTrackAppearance = appearance68;
            this.tabControl.InterTabSpacing = new Infragistics.Win.DefaultableInteger(1);
            this.tabControl.Location = new System.Drawing.Point(1, 1);
            this.tabControl.MinTabWidth = 26;
            this.tabControl.Name = "tabControl";
            this.tabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabControl.Size = new System.Drawing.Size(680, 495);
            this.tabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.StateButtons;
            this.tabControl.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            appearance69.BackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : System.Drawing.Color.White;
            appearance69.BorderColor = System.Drawing.Color.Black;
            this.tabControl.TabHeaderAreaAppearance = appearance69;
            this.tabControl.TabIndex = 3;
            this.tabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop;
            this.tabControl.TabPageMargins.Left = 1;
            ultraTab6.Key = "alertSuppressionTab";
            ultraTab6.TabPage = this.ultraTabPageControl1;
            ultraTab6.Text = "Alert Suppression";            
            ultraTab6.ToolTipText = "Configure time a metric must exceed its configured threshold before an alert is g" +
                "enerated.";
            ultraTab7.Key = "alertFiltersTab";
            ultraTab7.TabPage = this.ultraTabPageControl8;
            ultraTab7.Text = "Alert Filters";
            ultraTab8.Key = "collectionFailuresTab";
            ultraTab8.TabPage = this.ultraTabPageControl6;
            ultraTab8.Text = "Collection Failures";
            ultraTab8.ToolTipText = "Configure whether alerts should be generated when date collection fails.";
            ultraTab9.Key = "autogrowSettingsTab";
            ultraTab9.TabPage = this.ultraTabPageControl12;
            ultraTab9.Text = "Autogrow Settings";
            ultraTab10.Key = "jobFiltersTab";
            ultraTab10.TabPage = this.ultraTabPageControl13;
            ultraTab10.Text = "Job Filters";
            this.tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab6,
            ultraTab7,
            ultraTab8,
            ultraTab9,
            ultraTab10});
            this.tabControl.TabSize = new System.Drawing.Size(0, 140);
            this.tabControl.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal;
            this.tabControl.UseAppStyling = false;
            this.tabControl.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this.tabControl.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.tabControl.ActiveTabChanging += new Infragistics.Win.UltraWinTabControl.ActiveTabChangingEventHandler(this.tabControl_ActiveTabChanging);
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(495, 495);
            // 
            // AdvancedAlertConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 546);
            this.Controls.Add(this.gradientPanel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);            
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(565, 546);
            this.Name = "AdvancedAlertConfigurationDialog";
            this.ShowIcon = false;         
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Alert Configuration - {0}";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.AdvancedAlertConfigurationDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AdvancedAlertConfigurationDialog_HelpRequested);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.smoothingContentPropertyPage.ContentPanel.ResumeLayout(false);
            this.agingAlertActivePanel.ResumeLayout(false);
            this.agingAlertActivePanel.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agingAlertActiveDuration)).EndInit();
            this.jobAlertActivePanel.ResumeLayout(false);
            this.jobAlertActivePanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.suppressionDaysSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobAlertActiveDuration)).EndInit();
            this.jobFailureSuppressionPanel.ResumeLayout(false);
            this.jobFailureSuppressionPanel.PerformLayout();
            this.timeSuppressionPanel.ResumeLayout(false);
            this.timeSuppressionPanel.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.suppressionMinutesSpinner)).EndInit();
            this.ultraTabPageControl8.ResumeLayout(false);
            this.office2007PropertyPage7.ContentPanel.ResumeLayout(false);
            this.logSizeLimitPanel.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.panel21.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnLogSize)).EndInit();
            this.driveFilterPanel.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.panel19.PerformLayout();
            this.panel20.ResumeLayout(false);
            this.panel20.PerformLayout();
            this.tablesFilterPanel.ResumeLayout(false);
            this.panel17.ResumeLayout(false);
            this.panel17.PerformLayout();
            this.panel18.ResumeLayout(false);
            this.panel18.PerformLayout();
            this.logScanRegularExpressionPanel.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            this.logScanPanel.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.sessionUsersFilterPanel.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.sessionHostServersFilterPanel.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.sessionApplicationsFilterPanel.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.sessionExludeDMappFilterPanel.ResumeLayout(false);
            this.sessionExludeDMappFilterPanel.PerformLayout();
            this.jobCategoriesFilterPanel.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.jobsFilterPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.databasesFilterPanel.ResumeLayout(false);
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            this.readOnlyDatabaseIncludePanel.ResumeLayout(false);
            //SQLDM-29041 -- Add availability group alert options.
            this.availabilityGroupBackupAlertPanel.ResumeLayout(false);
            this.replicaCollectionOptionPanel.ResumeLayout(false);

            this.databasesExcludeFilterPanel.ResumeLayout(false);
            this.databasesExcludeFilterPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.filegroupsFilterPanel.ResumeLayout(false);
            this.filegroupsExcludeFilterPanel.ResumeLayout(false);
            this.filegroupsExcludeFilterPanel.PerformLayout();
            this.panel22.ResumeLayout(false);
            this.panel22.PerformLayout();
            this.versionStorePanel.ResumeLayout(false);
            this.versionStorePanel.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minVersionStoreGenerationRate)).EndInit();
            this.versionStoreSizePanel.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.versionStoreSizeUpDown)).EndInit();
            this.ultraTabPageControl6.ResumeLayout(false);
            this.office2007PropertyPage5.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage5.ContentPanel.PerformLayout();
            this.ultraTabPageControl12.ResumeLayout(false);
            this.office2007PropertyPage8.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage8.ContentPanel.PerformLayout();
            this.ultraTabPageControl13.ResumeLayout(false);
            this.office2007PropertyPage9.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage9.ContentPanel.PerformLayout();
            this.minRuntimeJobsPanel.ResumeLayout(false);
            this.minRuntimeJobsPanel.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimalRuntimeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridExclude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInclude)).EndInit();
            this.ultraTabPageControl5.ResumeLayout(false);
            this.office2007PropertyPage4.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage4.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blockingTypesListView)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl3.ResumeLayout(false);
            this.office2007PropertyPage2.ContentPanel.ResumeLayout(false);
            this.office2007PropertyPage2.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.excludedJobNamesListBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableJobNamesListBox)).EndInit();
            this.ultraTabPageControl4.ResumeLayout(false);
            this.office2007PropertyPage3.ContentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.userNameGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.programNameGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hostNameGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appNameGrid)).EndInit();
            this.ultraTabPageControl7.ResumeLayout(false);
            this.office2007PropertyPage6.ContentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.jobCategoriesListBox)).EndInit();
            this.gradientPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FilterOptionNumeric)).EndInit();
            this.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FilterOptionNumericOutOf)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage smoothingContentPropertyPage;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip smoothingPropertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown suppressionMinutesSpinner;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl6;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel gradientPanel1;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage2;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage3;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage4;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage5;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip21;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip2;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip3;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip4;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip5;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel availableJobNamesStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeJobNameExclusionButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addJobNameExclusionButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox adhocJobNameTextBox;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton jobCategoriesClearButton;
        private Infragistics.Win.UltraWinListView.UltraListView jobCategoriesListBox;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip6;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton skipAlertButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton generateAlertButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox excludeSelfBlockingCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox excludeDMapplicationCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox includeReadOnlyDatabases;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton button3;
        private Infragistics.Win.UltraWinListView.UltraListView blockingTypesListView;
        private Infragistics.Win.UltraWinGrid.UltraGrid appNameGrid;
        private Infragistics.Win.UltraWinGrid.UltraGrid userNameGrid;
        private Infragistics.Win.UltraWinGrid.UltraGrid programNameGrid;
        private Infragistics.Win.UltraWinGrid.UltraGrid hostNameGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox4;
        private Infragistics.Win.UltraWinListView.UltraListView availableJobNamesListBox;
        private Infragistics.Win.UltraWinListView.UltraListView excludedJobNamesListBox;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl8;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl9;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl10;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl11;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label39;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterDatabasesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterFilegroupsButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel22;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  databasesExcludeFilterPanel;
        //SQLDM-29210 -- Add option to include alerts for Read-only databases.
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  readOnlyDatabaseIncludeFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  filegroupsExcludeFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  databasesFilterPanel;
        //SQLDM-29210 -- Add option to include alerts for Read-only databases.
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  readOnlyDatabaseIncludePanel;

        //SQLDM-29041 -- Add AG Backup alert options.
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  availabilityGroupBackupAlertPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  availabilityGroupBackupAlertFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox availabilityGroupBackupAlertOptions;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton availabilityGroupPrimaryOption;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton availabilityGroupSecondaryOption;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton availabilityGroupBothOption;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton availabilityGroupDefaultOption;

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  replicaCollectionOptionPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  replicaCollectionOptionFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomGroupBox replicaCollectionOptions;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton replicaCollectionMonitoredOnlyOption;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton replicaCollectionAllOption;




        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  filegroupsFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobCategoriesFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel6;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox jobCategoriesExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterJobCategoriesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobsFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sessionUsersFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel13;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sessionUsersExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sessionHostServersFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel10;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sessionHostServersExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel11;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label10;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sessionApplicationsFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  sessionExludeDMappFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel5;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox sessionApplicationsExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel8;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobFailureSuppressionPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  timeSuppressionPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton raiseJobFailureAlertLimitedRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton raiseJobFailureAlertAnytimeRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox databasesExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox filegroupsExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label17;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobAlertActivePanel;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip jobAlertActiveTimeHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor jobAlertActiveDuration;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label19;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  logScanPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel9;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label20;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip13;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxCritical;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel12;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel logScanDirections;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label22;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxWarning;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  logScanRegularExpressionPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel16;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip14;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label21;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxRegexCritical;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxRegexWarning;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label23;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label24;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  agingAlertActivePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label25;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTimeComboEditor agingAlertActiveDuration;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label26;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip agingAlertActiveTimeHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  tablesFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel17;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip15;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox tablesExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterTablesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label27;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label28;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  driveFilterPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel19;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox drivesExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterDrivesButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label29;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel20;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label30;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl12;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage8;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip17;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton ignoreAutogrow;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton useAutogrow;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl13;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage9;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStrip18;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox chkAlertOnJobSteps;
        private System.Windows.Forms.DataGridView gridInclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label31;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeInclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editInclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addInclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton clearInclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeExclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editExclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addExclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton clearExclude;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label32;
        private System.Windows.Forms.DataGridView gridExclude;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeCatOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeJobOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeJobName;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeStepOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn excludeStepName;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeCatOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeJobOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeJobName;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeStepOpCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn includeStepName;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label33;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxInfo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox logScanIncludeTextBoxRegexInfo;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label34;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minVersionStoreGenerationRate;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  versionStorePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label35;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip19;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip7;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox jobsExcludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton filterJobsButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label16;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown versionStoreSizeUpDown;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  versionStoreSizePanel;
        private Controls.PropertiesHeaderStrip versionStoreSizeHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label36;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  minRuntimeJobsPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel minlRuntimeUnit0Label;
        private Controls.PropertiesHeaderStrip minLongJobRuntimeHeaderStrip;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel minlRuntimeUnit1Label;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown minimalRuntimeUpDown;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip excludeDMappHeaderStrip;

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown spnLogSize;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  logSizeLimitPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel21;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblLogSize;
        private Controls.PropertiesHeaderStrip propertiesHeaderStrip20;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label37;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label38;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown suppressionDaysSpinner;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel5;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel6;
        private Label label41;
        private Label label42;
        private Label label43;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip thresholdHeaderStrip;
        
        private TextBox textBox1;
        private Panel FilterOptionPanel;
        private Label label40;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown FilterOptionNumeric;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomNumericUpDown FilterOptionNumericOutOf;
    }
}
