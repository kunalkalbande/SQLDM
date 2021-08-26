using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using Wintellect.PowerCollections;

    partial class CustomReportWizard
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
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(74068277);
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(74068292);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomReportWizard));
            this.grdSelectedCounters = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.wizard = new Divelements.WizardFramework.Wizard();
            this.introPage = new Divelements.WizardFramework.IntroductionPage();
            this.hideWelcomePageCheckBox = new System.Windows.Forms.CheckBox();
            this.introductoryTextLabel2 = new System.Windows.Forms.Label();
            this.newReportPage = new Divelements.WizardFramework.WizardPage();
            this.label10 = new System.Windows.Forms.Label();
            this.txtReportName = new System.Windows.Forms.TextBox();
            this.btnAddReport = new System.Windows.Forms.Button();
            this.lstReports = new System.Windows.Forms.ListBox();
            this.selectCounterPage = new Divelements.WizardFramework.WizardPage();
            this.customizeReportTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlButtonContainner = new System.Windows.Forms.Panel();
            this.btnAddCounters = new System.Windows.Forms.Button();
            this.btnRemoveCounters = new System.Windows.Forms.Button();
            this.avlbCounterGroupBox = new System.Windows.Forms.GroupBox();
            this.reportTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.chkCustomCounters = new System.Windows.Forms.CheckBox();
            this.chkOSCounters = new System.Windows.Forms.CheckBox();
            this.lstAvailableCounters = new System.Windows.Forms.ListBox();
            this.lblAvailableCounters = new System.Windows.Forms.Label();
            this.chkSQLCounters = new System.Windows.Forms.CheckBox();
            this.fmeReport = new System.Windows.Forms.GroupBox();
            this.chkShowTabularData = new System.Windows.Forms.CheckBox();
            this.chkShowGraphicalData = new System.Windows.Forms.RadioButton();
            this.chkShowTopServersData = new System.Windows.Forms.RadioButton();
            this.lstAddedCounters = new System.Windows.Forms.ListBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.counterAggregationPage = new Divelements.WizardFramework.WizardPage();
            this.finishPage1 = new Divelements.WizardFramework.FinishPage();
            this.lblReportWizardSummary = new System.Windows.Forms.Label();
            this.counterIdentificationPage = new Divelements.WizardFramework.WizardPage();
            this.categoryComboBox1 = new System.Windows.Forms.ComboBox();
            this.descriptionTextBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkVirtCounters = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectedCounters)).BeginInit();
            this.wizard.SuspendLayout();
            this.introPage.SuspendLayout();
            this.newReportPage.SuspendLayout();
            this.selectCounterPage.SuspendLayout();
            this.customizeReportTableLayout.SuspendLayout();
            this.pnlButtonContainner.SuspendLayout();
            this.avlbCounterGroupBox.SuspendLayout();
            this.reportTypeGroupBox.SuspendLayout();
            this.fmeReport.SuspendLayout();
            this.counterAggregationPage.SuspendLayout();
            this.finishPage1.SuspendLayout();
            this.counterIdentificationPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdSelectedCounters
            // 
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grdSelectedCounters.DisplayLayout.Appearance = appearance21;
            this.grdSelectedCounters.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.grdSelectedCounters.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.grdSelectedCounters.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance22.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.grdSelectedCounters.DisplayLayout.GroupByBox.Appearance = appearance22;
            appearance23.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSelectedCounters.DisplayLayout.GroupByBox.BandLabelAppearance = appearance23;
            this.grdSelectedCounters.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdSelectedCounters.DisplayLayout.GroupByBox.Hidden = true;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance24.BackColor2 = System.Drawing.SystemColors.Control;
            appearance24.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance24.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grdSelectedCounters.DisplayLayout.GroupByBox.PromptAppearance = appearance24;
            this.grdSelectedCounters.DisplayLayout.MaxColScrollRegions = 1;
            this.grdSelectedCounters.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdSelectedCounters.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdSelectedCounters.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdSelectedCounters.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.grdSelectedCounters.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grdSelectedCounters.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            this.grdSelectedCounters.DisplayLayout.Override.CardAreaAppearance = appearance25;
            appearance26.BorderColor = System.Drawing.Color.Silver;
            appearance26.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grdSelectedCounters.DisplayLayout.Override.CellAppearance = appearance26;
            this.grdSelectedCounters.DisplayLayout.Override.CellPadding = 0;
            this.grdSelectedCounters.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.grdSelectedCounters.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance27.BackColor = System.Drawing.SystemColors.Control;
            appearance27.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance27.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance27.BorderColor = System.Drawing.SystemColors.Window;
            this.grdSelectedCounters.DisplayLayout.Override.GroupByRowAppearance = appearance27;
            this.grdSelectedCounters.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance28.TextHAlignAsString = "Left";
            this.grdSelectedCounters.DisplayLayout.Override.HeaderAppearance = appearance28;
            this.grdSelectedCounters.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.Color.Silver;
            this.grdSelectedCounters.DisplayLayout.Override.RowAppearance = appearance29;
            this.grdSelectedCounters.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdSelectedCounters.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdSelectedCounters.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdSelectedCounters.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdSelectedCounters.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grdSelectedCounters.DisplayLayout.Override.TemplateAddRowAppearance = appearance30;
            this.grdSelectedCounters.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdSelectedCounters.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdSelectedCounters.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "Aggregation";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            object tempval = new object();
            tempval = "Max"; valueList1.ValueListItems.Add(tempval);
            tempval = "Weighted Avg"; valueList1.ValueListItems.Add(tempval);
            tempval = "PerMinute Avg"; valueList1.ValueListItems.Add(tempval);
            valueList2.Key = "Source";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.grdSelectedCounters.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2});
            this.grdSelectedCounters.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdSelectedCounters.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grdSelectedCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdSelectedCounters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdSelectedCounters.Location = new System.Drawing.Point(0, 0);
            this.grdSelectedCounters.Name = "grdSelectedCounters";
            this.grdSelectedCounters.Size = new System.Drawing.Size(587, 383);
            this.grdSelectedCounters.TabIndex = 33;
            this.grdSelectedCounters.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdSelectedCounters_KeyPress);
            this.grdSelectedCounters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdSelectedCounters_MouseDown);            
            // 
            // wizard
            // 
            this.wizard.AnimatePageTransitions = false;
            this.wizard.BannerImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounterWizardBannerImage;
            this.wizard.Controls.Add(this.introPage);
            this.wizard.Controls.Add(this.selectCounterPage);
            this.wizard.Controls.Add(this.counterAggregationPage);
            this.wizard.Controls.Add(this.finishPage1);
            this.wizard.Controls.Add(this.newReportPage);
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.MarginImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounterWizardWelcomePageMarginImage;
            this.wizard.Name = "wizard";
            this.wizard.OwnerForm = this;
            this.wizard.Size = new System.Drawing.Size(609, 512);
            this.wizard.TabIndex = 0;
            this.wizard.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizard.Finish += new System.EventHandler(this.wizard_Finish);
            // 
            // introPage
            // 
            this.introPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.introPage.Controls.Add(this.hideWelcomePageCheckBox);
            this.introPage.Controls.Add(this.introductoryTextLabel2);
            this.introPage.IntroductionText = "This wizard helps you create a custom report containing any metrics harvested by " +
    "SQL diagnostic manager. This wizard will walk you through the following steps:";
            this.introPage.Location = new System.Drawing.Point(175, 71);
            this.introPage.Name = "introPage";
            this.introPage.NextPage = this.newReportPage;
            this.introPage.ProceedText = "";
            this.introPage.Size = new System.Drawing.Size(412, 383);
            this.introPage.TabIndex = 1004;
            this.introPage.Text = "Welcome to the Custom Report Wizard";
            // 
            // hideWelcomePageCheckBox
            // 
            this.hideWelcomePageCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.hideWelcomePageCheckBox.AutoSize = true;
            this.hideWelcomePageCheckBox.Checked = global::Idera.SQLdm.DesktopClient.Properties.Settings.Default.HideAddServersWizardWelcomePage;
            this.hideWelcomePageCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "HideAddServersWizardWelcomePage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.hideWelcomePageCheckBox.Location = new System.Drawing.Point(3, 348);
            this.hideWelcomePageCheckBox.Name = "hideWelcomePageCheckBox";
            this.hideWelcomePageCheckBox.Size = new System.Drawing.Size(199, 17);
            this.hideWelcomePageCheckBox.TabIndex = 3;
            this.hideWelcomePageCheckBox.Text = "Don\'t show this welcome page again";
            this.hideWelcomePageCheckBox.UseVisualStyleBackColor = true;
            this.hideWelcomePageCheckBox.CheckedChanged += new System.EventHandler(this.hideWelcomePageCheckBox_CheckedChanged);
            // 
            // introductoryTextLabel2
            // 
            this.introductoryTextLabel2.Location = new System.Drawing.Point(20, 43);
            this.introductoryTextLabel2.Name = "introductoryTextLabel2";
            this.introductoryTextLabel2.Size = new System.Drawing.Size(383, 175);
            this.introductoryTextLabel2.TabIndex = 2;
            this.introductoryTextLabel2.Text = resources.GetString("introductoryTextLabel2.Text");
            // 
            // newReportPage
            // 
            this.newReportPage.Controls.Add(this.label10);
            this.newReportPage.Controls.Add(this.txtReportName);
            this.newReportPage.Controls.Add(this.btnAddReport);
            this.newReportPage.Controls.Add(this.lstReports);
            this.newReportPage.Description = "Add a new report or select an existing report to edit.";
            this.newReportPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newReportPage.Location = new System.Drawing.Point(11, 71);
            this.newReportPage.Name = "newReportPage";
            this.newReportPage.NextPage = this.selectCounterPage;
            this.newReportPage.PreviousPage = this.introPage;
            this.newReportPage.Size = new System.Drawing.Size(587, 383);
            this.newReportPage.TabIndex = 1014;
            this.newReportPage.Text = "Add\\Select a report";
            this.newReportPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.newReportPage_BeforeMoveNext);
            this.newReportPage.BeforeDisplay += new System.EventHandler(this.newReportPage_BeforeDisplay);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Report Name";
            // 
            // txtReportName
            // 
            this.txtReportName.Location = new System.Drawing.Point(105, 9);
            this.txtReportName.MaxLength = 30;
            this.txtReportName.Name = "txtReportName";
            this.txtReportName.Size = new System.Drawing.Size(462, 20);
            this.txtReportName.TabIndex = 4;
            this.txtReportName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtReportName_KeyUp);
            // 
            // btnAddReport
            // 
            this.btnAddReport.Location = new System.Drawing.Point(493, 201);
            this.btnAddReport.Name = "btnAddReport";
            this.btnAddReport.Size = new System.Drawing.Size(75, 23);
            this.btnAddReport.TabIndex = 1;
            this.btnAddReport.Text = "Add";
            this.btnAddReport.UseVisualStyleBackColor = true;
            this.btnAddReport.Visible = false;
            this.btnAddReport.Click += new System.EventHandler(this.btnAddReport_Click);
            // 
            // lstReports
            // 
            this.lstReports.FormattingEnabled = true;
            this.lstReports.Location = new System.Drawing.Point(105, 35);
            this.lstReports.Name = "lstReports";
            this.lstReports.Size = new System.Drawing.Size(462, 160);
            this.lstReports.TabIndex = 6;
            this.lstReports.Click += new System.EventHandler(this.lstReports_Click);
            this.lstReports.SelectedValueChanged += new System.EventHandler(this.lstReports_SelectedValueChanged);
            // 
            // selectCounterPage
            // 
            this.selectCounterPage.Controls.Add(this.customizeReportTableLayout);
            this.selectCounterPage.Controls.Add(this.lblMessage);
            this.selectCounterPage.Controls.Add(this.informationBox1);
            this.selectCounterPage.Description = "Choose a counter type and the specific counter that you would like to report on";
            this.selectCounterPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectCounterPage.Location = new System.Drawing.Point(11, 71);
            this.selectCounterPage.Name = "selectCounterPage";
            this.selectCounterPage.NextPage = this.counterAggregationPage;
            this.selectCounterPage.PreviousPage = this.newReportPage;
            this.selectCounterPage.Size = new System.Drawing.Size(587, 383);
            this.selectCounterPage.TabIndex = 1010;
            this.selectCounterPage.Text = "Select the counters that you would like to include in your custom report.";
            this.selectCounterPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.selectCounterPage_BeforeMoveBack);
            this.selectCounterPage.AfterDisplay += new System.EventHandler(this.selectCounterPage_AfterDisplay);
            this.selectCounterPage.BeforeDisplay += new System.EventHandler(this.selectCounterPage_BeforeDisplay);
            // 
            // customizeReportTableLayout
            // 
            this.customizeReportTableLayout.ColumnCount = 3;
            this.customizeReportTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.customizeReportTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.customizeReportTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.customizeReportTableLayout.Controls.Add(this.reportTypeGroupBox, 0, 0);
            this.customizeReportTableLayout.Controls.Add(this.avlbCounterGroupBox, 0, 1);
            this.customizeReportTableLayout.Controls.Add(this.pnlButtonContainner, 1, 1);
            this.customizeReportTableLayout.Controls.Add(this.fmeReport, 2, 1);
            this.customizeReportTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customizeReportTableLayout.Location = new System.Drawing.Point(0, 71);
            this.customizeReportTableLayout.Name = "customizeReportTableLayout";
            this.customizeReportTableLayout.RowCount = 2;
            this.customizeReportTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.customizeReportTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.customizeReportTableLayout.Size = new System.Drawing.Size(587, 312);
            this.customizeReportTableLayout.TabIndex = 28;
            // 
            // pnlButtonContainner
            // 
            this.pnlButtonContainner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlButtonContainner.Controls.Add(this.btnAddCounters);
            this.pnlButtonContainner.Controls.Add(this.btnRemoveCounters);
            this.pnlButtonContainner.Location = new System.Drawing.Point(251, 100);
            this.pnlButtonContainner.Name = "pnlButtonContainner";
            this.pnlButtonContainner.Size = new System.Drawing.Size(85, 112);
            this.pnlButtonContainner.TabIndex = 0;
            // 
            // btnAddCounters
            // 
            this.btnAddCounters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAddCounters.Enabled = false;
            this.btnAddCounters.Location = new System.Drawing.Point(0, 66);
            this.btnAddCounters.Name = "btnAddCounters";
            this.btnAddCounters.Size = new System.Drawing.Size(85, 23);
            this.btnAddCounters.TabIndex = 18;
            this.btnAddCounters.Text = "&Add >";
            this.btnAddCounters.UseVisualStyleBackColor = true;
            this.btnAddCounters.Click += new System.EventHandler(this.btnAddCounters_Click);
            // 
            // btnRemoveCounters
            // 
            this.btnRemoveCounters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRemoveCounters.Enabled = false;
            this.btnRemoveCounters.Location = new System.Drawing.Point(0, 89);
            this.btnRemoveCounters.Name = "btnRemoveCounters";
            this.btnRemoveCounters.Size = new System.Drawing.Size(85, 23);
            this.btnRemoveCounters.TabIndex = 19;
            this.btnRemoveCounters.Text = "< &Remove";
            this.btnRemoveCounters.UseVisualStyleBackColor = true;
            this.btnRemoveCounters.Click += new System.EventHandler(this.btnRemoveCounters_Click);
            // 
            // avlbCounterGroupBox
            // 
            this.avlbCounterGroupBox.Controls.Add(this.chkVirtCounters);
            this.avlbCounterGroupBox.Controls.Add(this.chkCustomCounters);
            this.avlbCounterGroupBox.Controls.Add(this.chkOSCounters);
            this.avlbCounterGroupBox.Controls.Add(this.lstAvailableCounters);
            this.avlbCounterGroupBox.Controls.Add(this.lblAvailableCounters);
            this.avlbCounterGroupBox.Controls.Add(this.chkSQLCounters);
            this.avlbCounterGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.avlbCounterGroupBox.Location = new System.Drawing.Point(3, 3);
            this.avlbCounterGroupBox.Name = "avlbCounterGroupBox";
            this.avlbCounterGroupBox.Size = new System.Drawing.Size(242, 306);
            this.avlbCounterGroupBox.TabIndex = 17;
            this.avlbCounterGroupBox.TabStop = false;
            this.avlbCounterGroupBox.Text = "Available Counters";
            // 
            // reportTypeGroupBox
            // 
            this.reportTypeGroupBox.Controls.Add(this.chkShowGraphicalData);
            this.reportTypeGroupBox.Controls.Add(this.chkShowTabularData);
            this.reportTypeGroupBox.Controls.Add(this.chkShowTopServersData);
            this.customizeReportTableLayout.SetColumnSpan(this.reportTypeGroupBox, 3);
            this.reportTypeGroupBox.Controls.Add(this.chkShowGraphicalData);
            this.reportTypeGroupBox.Controls.Add(this.chkShowTopServersData);
            this.reportTypeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportTypeGroupBox.Location = new System.Drawing.Point(3, 3);
            this.reportTypeGroupBox.Name = "avlbCountereportTypeGroupBoxrGroupBox";
            this.reportTypeGroupBox.Size = new System.Drawing.Size(742, 306);
            this.reportTypeGroupBox.TabIndex = 17;
            this.reportTypeGroupBox.TabStop = false;
            this.reportTypeGroupBox.Text = "Report Type";
            // 
            // Show Graphical Data Radio Button
            // 
            this.chkShowGraphicalData.AutoSize = true;
            this.chkShowGraphicalData.Checked = true;
            this.chkShowGraphicalData.Location = new System.Drawing.Point(6, 16);
            this.chkShowGraphicalData.Name = "chkShowGraphicalData";
            this.chkShowGraphicalData.Size = new System.Drawing.Size(140, 17);
            this.chkShowGraphicalData.TabIndex = 0;
            this.chkShowGraphicalData.TabStop = true;
            this.chkShowGraphicalData.Text = "Show Graphical Data";
            this.chkShowGraphicalData.UseVisualStyleBackColor = true;
            this.chkShowGraphicalData.CheckedChanged += new System.EventHandler(this.chkShowGraphicalData_CheckedChanged);

            // 
            // chkShowTabularData
            // 
            this.chkShowTabularData.AutoSize = true;
            this.chkShowTabularData.Checked = false;
            this.chkShowTabularData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowTabularData.Location = new System.Drawing.Point(20, 36);
            this.chkShowTabularData.Name = "chkShowTabularData";
            this.chkShowTabularData.Size = new System.Drawing.Size(153, 17);
            this.chkShowTabularData.TabIndex = 29;
            this.chkShowTabularData.Text = "Include tabular data in report";
            this.chkShowTabularData.UseVisualStyleBackColor = true;
            // 
            // Show Top Servers Radio Button
            // 
            this.chkShowTopServersData.AutoSize = true;
            this.chkShowTopServersData.Checked = false;
            this.chkShowTopServersData.Location = new System.Drawing.Point(6, 56);
            this.chkShowTopServersData.Name = "chkShowTopServersData";
            this.chkShowTopServersData.Size = new System.Drawing.Size(140, 17);
            this.chkShowTopServersData.TabIndex = 0;
            this.chkShowTopServersData.TabStop = true;
            this.chkShowTopServersData.Text = "Show worst performing servers for each metric.";
            this.chkShowTopServersData.UseVisualStyleBackColor = true;
            // 
            // chkCustomCounters
            // 
            this.chkCustomCounters.AutoSize = true;
            this.chkCustomCounters.Location = new System.Drawing.Point(12, 77);
            this.chkCustomCounters.Name = "chkCustomCounters";
            this.chkCustomCounters.Size = new System.Drawing.Size(106, 17);
            this.chkCustomCounters.TabIndex = 28;
            this.chkCustomCounters.Text = "Custom Counters";
            this.chkCustomCounters.UseVisualStyleBackColor = true;
            this.chkCustomCounters.CheckedChanged += new System.EventHandler(this.chkCustomCounters_CheckedChanged);
            // 
            // chkOSCounters
            // 
            this.chkOSCounters.AutoSize = true;
            this.chkOSCounters.Checked = true;
            this.chkOSCounters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOSCounters.Location = new System.Drawing.Point(12, 18);
            this.chkOSCounters.Name = "chkOSCounters";
            this.chkOSCounters.Size = new System.Drawing.Size(154, 17);
            this.chkOSCounters.TabIndex = 26;
            this.chkOSCounters.Text = "Operating System Counters";
            this.chkOSCounters.UseVisualStyleBackColor = true;
            this.chkOSCounters.CheckedChanged += new System.EventHandler(this.chkOSCounters_CheckedChanged);
            // 
            // lstAvailableCounters
            // 
            this.lstAvailableCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAvailableCounters.DisplayMember = "Text";
            this.lstAvailableCounters.FormattingEnabled = true;
            this.lstAvailableCounters.HorizontalScrollbar = true;
            this.lstAvailableCounters.Location = new System.Drawing.Point(5, 102);
            this.lstAvailableCounters.Name = "lstAvailableCounters";
            this.lstAvailableCounters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAvailableCounters.Size = new System.Drawing.Size(232, 199);
            this.lstAvailableCounters.Sorted = true;
            this.lstAvailableCounters.TabIndex = 22;
            this.lstAvailableCounters.SelectedValueChanged += new System.EventHandler(this.lstAvailableCounters_SelectedValueChanged);
            // 
            // lblAvailableCounters
            // 
            this.lblAvailableCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAvailableCounters.BackColor = System.Drawing.Color.White;
            this.lblAvailableCounters.Enabled = false;
            this.lblAvailableCounters.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAvailableCounters.Location = new System.Drawing.Point(5, 97);
            this.lblAvailableCounters.Name = "lblAvailableCounters";
            this.lblAvailableCounters.Size = new System.Drawing.Size(232, 204);
            this.lblAvailableCounters.TabIndex = 21;
            this.lblAvailableCounters.Text = "< Unavailable >";
            this.lblAvailableCounters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAvailableCounters.Visible = false;
            // 
            // chkSQLCounters
            // 
            this.chkSQLCounters.AutoSize = true;
            this.chkSQLCounters.Location = new System.Drawing.Point(12, 36);
            this.chkSQLCounters.Name = "chkSQLCounters";
            this.chkSQLCounters.Size = new System.Drawing.Size(126, 17);
            this.chkSQLCounters.TabIndex = 27;
            this.chkSQLCounters.Text = "SQL Server Counters";
            this.chkSQLCounters.UseVisualStyleBackColor = true;
            this.chkSQLCounters.CheckedChanged += new System.EventHandler(this.chkSQLCounters_CheckedChanged);
            // 
            // fmeReport
            // 
            this.fmeReport.Controls.Add(this.lstAddedCounters);
            this.fmeReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fmeReport.Location = new System.Drawing.Point(342, 3);
            this.fmeReport.Name = "fmeReport";
            this.fmeReport.Size = new System.Drawing.Size(242, 306);
            this.fmeReport.TabIndex = 26;
            this.fmeReport.TabStop = false;
            this.fmeReport.Text = "Report Name:";
            // 
            // lstAddedCounters
            // 
            this.lstAddedCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAddedCounters.DisplayMember = "Text";
            this.lstAddedCounters.FormattingEnabled = true;
            this.lstAddedCounters.Location = new System.Drawing.Point(5, 102);
            this.lstAddedCounters.Name = "lstAddedCounters";
            this.lstAddedCounters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAddedCounters.Size = new System.Drawing.Size(232, 199);
            this.lstAddedCounters.Sorted = true;
            this.lstAddedCounters.TabIndex = 28;
            this.lstAddedCounters.SelectedValueChanged += new System.EventHandler(this.lstAddedCounters_SelectedValueChanged);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(164, 366);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 13);
            this.lblMessage.TabIndex = 27;
            // 
            // informationBox1
            // 
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox1.Location = new System.Drawing.Point(0, 0);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(587, 71);
            this.informationBox1.TabIndex = 16;
            this.informationBox1.Text = resources.GetString("informationBox1.Text");
            // 
            // counterAggregationPage
            // 
            this.counterAggregationPage.Controls.Add(this.grdSelectedCounters);
            this.counterAggregationPage.Description = "Order and select aggregation for the counters that you have selected for this rep" +
    "ort.";
            this.counterAggregationPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.counterAggregationPage.Location = new System.Drawing.Point(11, 71);
            this.counterAggregationPage.Name = "counterAggregationPage";
            this.counterAggregationPage.NextPage = this.finishPage1;
            this.counterAggregationPage.PreviousPage = this.selectCounterPage;
            this.counterAggregationPage.Size = new System.Drawing.Size(587, 383);
            this.counterAggregationPage.TabIndex = 1007;
            this.counterAggregationPage.Text = "Select counters for this report";
            this.counterAggregationPage.BeforeMoveNext += new Divelements.WizardFramework.WizardPageEventHandler(this.counterAggregationPage_BeforeMoveNext);
            this.counterAggregationPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.counterAggregationPage_BeforeMoveBack);
            this.counterAggregationPage.BeforeDisplay += new System.EventHandler(this.counterAggregationPage_BeforeDisplay);
            // 
            // finishPage1
            // 
            this.finishPage1.Controls.Add(this.lblReportWizardSummary);
            this.finishPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishPage1.FinishText = "You have successfully configured your custom report. Your new report contains the" +
    " following counters:";
            this.finishPage1.Location = new System.Drawing.Point(175, 71);
            this.finishPage1.Name = "finishPage1";
            this.finishPage1.PreviousPage = this.counterAggregationPage;
            this.finishPage1.ProceedText = "Click Finish to complete this wizard.";
            this.finishPage1.Size = new System.Drawing.Size(412, 383);
            this.finishPage1.TabIndex = 1006;
            this.finishPage1.Text = "Completing the Custom Report Wizard";
            this.finishPage1.BeforeDisplay += new System.EventHandler(this.finishPage1_BeforeDisplay);
            // 
            // lblReportWizardSummary
            // 
            this.lblReportWizardSummary.Location = new System.Drawing.Point(20, 29);
            this.lblReportWizardSummary.Name = "lblReportWizardSummary";
            this.lblReportWizardSummary.Size = new System.Drawing.Size(383, 175);
            this.lblReportWizardSummary.TabIndex = 3;
            this.lblReportWizardSummary.Text = "• Counter\r\n\r\n• Counter";
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
            this.counterIdentificationPage.NextPage = this.counterAggregationPage;
            this.counterIdentificationPage.PreviousPage = this.selectCounterPage;
            this.counterIdentificationPage.Size = new System.Drawing.Size(610, 317);
            this.counterIdentificationPage.TabIndex = 1005;
            this.counterIdentificationPage.Text = "Counter Identification";
            // 
            // categoryComboBox1
            // 
            this.categoryComboBox1.FormattingEnabled = true;
            this.categoryComboBox1.Location = new System.Drawing.Point(119, 48);
            this.categoryComboBox1.MaxLength = 64;
            this.categoryComboBox1.Name = "categoryComboBox1";
            this.categoryComboBox1.Size = new System.Drawing.Size(458, 21);
            this.categoryComboBox1.TabIndex = 1;
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
            // chkVirtCounters
            // 
            this.chkVirtCounters.AutoSize = true;
            this.chkVirtCounters.Location = new System.Drawing.Point(12, 54);
            this.chkVirtCounters.Name = "chkVirtCounters";
            this.chkVirtCounters.Size = new System.Drawing.Size(130, 17);
            this.chkVirtCounters.TabIndex = 29;
            this.chkVirtCounters.Text = "Virtualization Counters";
            this.chkVirtCounters.UseVisualStyleBackColor = true;
            this.chkVirtCounters.CheckedChanged += new System.EventHandler(this.chkVirtCounters_CheckedChanged);
            // 
            // CustomReportWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 512);
            this.Controls.Add(this.wizard);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomReportWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Custom Report";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.CustomReportsWizard_HelpButtonClicked);
            this.Load += new System.EventHandler(this.CustomCounterWizard_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.CustomReportsWizard_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectedCounters)).EndInit();
            this.wizard.ResumeLayout(false);
            this.introPage.ResumeLayout(false);
            this.introPage.PerformLayout();
            this.newReportPage.ResumeLayout(false);
            this.newReportPage.PerformLayout();
            this.selectCounterPage.ResumeLayout(false);
            this.selectCounterPage.PerformLayout();
            this.customizeReportTableLayout.ResumeLayout(false);
            this.pnlButtonContainner.ResumeLayout(false);
            this.avlbCounterGroupBox.ResumeLayout(false);
            this.avlbCounterGroupBox.PerformLayout();
            this.reportTypeGroupBox.ResumeLayout(false);
            this.reportTypeGroupBox.PerformLayout();
            this.fmeReport.ResumeLayout(false);
            this.fmeReport.PerformLayout();
            this.counterAggregationPage.ResumeLayout(false);
            this.finishPage1.ResumeLayout(false);
            this.counterIdentificationPage.ResumeLayout(false);
            this.counterIdentificationPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizard;
        private Divelements.WizardFramework.IntroductionPage introPage;
        private Divelements.WizardFramework.WizardPage counterIdentificationPage;
        private Divelements.WizardFramework.FinishPage finishPage1;
        private System.Windows.Forms.TextBox descriptionTextBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox categoryComboBox1;
        private Divelements.WizardFramework.WizardPage counterAggregationPage;
        private Divelements.WizardFramework.WizardPage selectCounterPage;
        private System.Windows.Forms.Label introductoryTextLabel2;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.Label lblReportWizardSummary;
        private System.Windows.Forms.CheckBox hideWelcomePageCheckBox;
        private System.Windows.Forms.GroupBox avlbCounterGroupBox;
        private System.Windows.Forms.GroupBox reportTypeGroupBox;
        private Divelements.WizardFramework.WizardPage newReportPage;
        private System.Windows.Forms.ListBox lstReports;
        private System.Windows.Forms.Button btnAddReport;
        private System.Windows.Forms.Label lblAvailableCounters;
        private System.Windows.Forms.Button btnRemoveCounters;
        private System.Windows.Forms.Button btnAddCounters;
        private System.Windows.Forms.ListBox lstAvailableCounters;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdSelectedCounters;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtReportName;
        private System.Windows.Forms.CheckBox chkCustomCounters;
        private System.Windows.Forms.CheckBox chkOSCounters;
        private System.Windows.Forms.CheckBox chkSQLCounters;
        private System.Windows.Forms.GroupBox fmeReport;
        private System.Windows.Forms.ListBox lstAddedCounters;
        private System.Windows.Forms.CheckBox chkShowTabularData;
        private System.Windows.Forms.RadioButton chkShowGraphicalData;
        private System.Windows.Forms.RadioButton chkShowTopServersData;
        private System.Windows.Forms.Label lblMessage;
        private TableLayoutPanel customizeReportTableLayout;
        private Panel pnlButtonContainner;
        private CheckBox chkVirtCounters;
    }
}