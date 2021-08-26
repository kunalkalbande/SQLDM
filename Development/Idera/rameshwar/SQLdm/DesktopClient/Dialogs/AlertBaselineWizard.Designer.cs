namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AlertBaselineWizard
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("AlertBaselineEntry", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Selected");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReferenceRange");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WarningThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CriticalThreshold");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedWarning");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SuggestedCritical");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(8767172);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.wizard1 = new Divelements.WizardFramework.Wizard();
            this.introductionPage = new Divelements.WizardFramework.IntroductionPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.baselineWizardButton = new System.Windows.Forms.Button();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.configPage = new Divelements.WizardFramework.WizardPage();
            this.criticalPercentageEditor = new System.Windows.Forms.NumericUpDown();
            this.warningPercentageEditor = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.informationBox2 = new Divelements.WizardFramework.InformationBox();
            this.metricSelectionPage = new Divelements.WizardFramework.WizardPage();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.alertsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.alertConfigBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkAllButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.loadingCircle1 = new MRG.Controls.UI.LoadingCircle();
            this.contentStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.wizardSelectionContentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.showAlertBaselineWizardButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.showBaselineWizardButton = new Idera.SQLdm.DesktopClient.Controls.FeatureButton();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.dividerLabel1 = new System.Windows.Forms.Label();
            this.productDescriptionLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.wizard1.SuspendLayout();
            this.introductionPage.SuspendLayout();
            this.panel3.SuspendLayout();
            this.configPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.criticalPercentageEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningPercentageEditor)).BeginInit();
            this.metricSelectionPage.SuspendLayout();
            this.stackLayoutPanel1.SuspendLayout();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigBindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contentStackPanel.SuspendLayout();
            this.wizardSelectionContentPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard1
            // 
            this.wizard1.AnimatePageTransitions = false;
            this.wizard1.Controls.Add(this.introductionPage);
            this.wizard1.Controls.Add(this.configPage);
            this.wizard1.Controls.Add(this.metricSelectionPage);
            this.wizard1.Dock = System.Windows.Forms.DockStyle.None;
            this.wizard1.Location = new System.Drawing.Point(0, 0);
            this.wizard1.Name = "wizard1";
            this.wizard1.OwnerForm = this;
            this.wizard1.Size = new System.Drawing.Size(542, 360);
            this.wizard1.TabIndex = 0;
            this.wizard1.UserExperienceType = Divelements.WizardFramework.WizardUserExperienceType.Wizard97;
            this.wizard1.Finish += new System.EventHandler(this.wizard1_Finish);
            // 
            // introductionPage
            // 
            this.introductionPage.BackColor = System.Drawing.SystemColors.Window;
            this.introductionPage.Controls.Add(this.panel3);
            this.introductionPage.IntroductionText = "This wizard will guide you through configuring your alert threhsolds using statis" +
                "tics collected from your servers over a given time period.";
            this.introductionPage.Location = new System.Drawing.Point(175, 71);
            this.introductionPage.Name = "introductionPage";
            this.introductionPage.NextPage = this.configPage;
            this.introductionPage.Size = new System.Drawing.Size(345, 231);
            this.introductionPage.TabIndex = 1004;
            this.introductionPage.Text = "Welcome to the Alert Baseline Wizard";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.baselineWizardButton);
            this.panel3.Controls.Add(this.informationBox1);
            this.panel3.Location = new System.Drawing.Point(23, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(300, 96);
            this.panel3.TabIndex = 1;
            this.panel3.Visible = false;
            // 
            // baselineWizardButton
            // 
            this.baselineWizardButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.baselineWizardButton.Location = new System.Drawing.Point(101, 60);
            this.baselineWizardButton.Name = "baselineWizardButton";
            this.baselineWizardButton.Size = new System.Drawing.Size(96, 23);
            this.baselineWizardButton.TabIndex = 1;
            this.baselineWizardButton.Text = "Set Baseline...";
            this.baselineWizardButton.UseVisualStyleBackColor = true;
            this.baselineWizardButton.Click += new System.EventHandler(this.baselineWizardButton_Click);
            // 
            // informationBox1
            // 
            this.informationBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.informationBox1.Location = new System.Drawing.Point(0, 0);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(300, 54);
            this.informationBox1.TabIndex = 0;
            this.informationBox1.Text = "One or more metrics do not have baseline data available.  Click the \'Set Baseline" +
                "\' button to refresh baseline data.";
            // 
            // configPage
            // 
            this.configPage.Controls.Add(this.criticalPercentageEditor);
            this.configPage.Controls.Add(this.warningPercentageEditor);
            this.configPage.Controls.Add(this.label4);
            this.configPage.Controls.Add(this.label3);
            this.configPage.Controls.Add(this.informationBox2);
            this.configPage.Description = "";
            this.configPage.Location = new System.Drawing.Point(11, 71);
            this.configPage.Name = "configPage";
            this.configPage.NextPage = this.metricSelectionPage;
            this.configPage.PreviousPage = this.introductionPage;
            this.configPage.Size = new System.Drawing.Size(520, 231);
            this.configPage.TabIndex = 1007;
            this.configPage.Text = "Baseline Configuration";
            this.configPage.BeforeMoveBack += new Divelements.WizardFramework.WizardPageEventHandler(this.configPage_BeforeMoveBack);
            this.configPage.BeforeDisplay += new System.EventHandler(this.configPage_BeforeDisplay);
            // 
            // criticalPercentageEditor
            // 
            this.criticalPercentageEditor.Location = new System.Drawing.Point(131, 89);
            this.criticalPercentageEditor.Name = "criticalPercentageEditor";
            this.criticalPercentageEditor.Size = new System.Drawing.Size(84, 20);
            this.criticalPercentageEditor.TabIndex = 11;
            this.criticalPercentageEditor.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // warningPercentageEditor
            // 
            this.warningPercentageEditor.Location = new System.Drawing.Point(131, 63);
            this.warningPercentageEditor.Name = "warningPercentageEditor";
            this.warningPercentageEditor.Size = new System.Drawing.Size(84, 20);
            this.warningPercentageEditor.TabIndex = 10;
            this.warningPercentageEditor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Critical:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Warning:";
            // 
            // informationBox2
            // 
            this.informationBox2.Location = new System.Drawing.Point(27, 22);
            this.informationBox2.Name = "informationBox2";
            this.informationBox2.Size = new System.Drawing.Size(376, 35);
            this.informationBox2.TabIndex = 7;
            this.informationBox2.Text = "Enter the percentages above the reference range for each metric to set the warnin" +
                "g and critical thresholds.";
            // 
            // metricSelectionPage
            // 
            this.metricSelectionPage.Controls.Add(this.stackLayoutPanel1);
            this.metricSelectionPage.Description = "Select the metrics to update when the Finish button is clicked.";
            this.metricSelectionPage.Location = new System.Drawing.Point(11, 71);
            this.metricSelectionPage.Name = "metricSelectionPage";
            this.metricSelectionPage.PreviousPage = this.configPage;
            this.metricSelectionPage.Size = new System.Drawing.Size(520, 231);
            this.metricSelectionPage.TabIndex = 1005;
            this.metricSelectionPage.Text = "Review Baseline";
            this.metricSelectionPage.BeforeDisplay += new System.EventHandler(this.metricSelectionPage_BeforeDisplay);
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = this.gridPanel;
            this.stackLayoutPanel1.Controls.Add(this.gridPanel);
            this.stackLayoutPanel1.Controls.Add(this.panel1);
            this.stackLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Size = new System.Drawing.Size(520, 231);
            this.stackLayoutPanel1.TabIndex = 6;
            // 
            // gridPanel
            // 
            this.gridPanel.Controls.Add(this.alertsGrid);
            this.gridPanel.Controls.Add(this.panel2);
            this.gridPanel.Location = new System.Drawing.Point(0, 0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(520, 231);
            this.gridPanel.TabIndex = 2;
            // 
            // alertsGrid
            // 
            this.alertsGrid.DataSource = this.alertConfigBindingSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alertsGrid.DisplayLayout.Appearance = appearance1;
            this.alertsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridBand1.CardSettings.Style = Infragistics.Win.UltraWinGrid.CardStyle.StandardLabels;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn1.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn1.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 14);
            ultraGridColumn1.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn1.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn1.Width = 23;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RibbonCheckboxUnchecked;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn2.CellAppearance = appearance13;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn2.Header.Caption = "Metric";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 337;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridColumn3.Hidden = true;
            ultraGridColumn3.HiddenWhenGroupBy = Infragistics.Win.DefaultableBoolean.True;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn4.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn4.Header.Caption = "Reference Range";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn5.Header.Caption = "Current Warning";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn5.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegativeWithSpin;
            ultraGridColumn5.Width = 57;
            ultraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn6.Header.Caption = "Current Critical";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.IntegerNonNegativeWithSpin;
            ultraGridColumn6.Width = 74;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn7.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn7.Header.Caption = "Warning Threshold";
            ultraGridColumn7.Header.VisiblePosition = 5;
            ultraGridColumn7.Width = 99;
            ultraGridColumn8.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn8.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn8.Header.Caption = "Critical Threshold";
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.Width = 55;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            ultraGridBand1.Indentation = 0;
            ultraGridBand1.IndentationGroupByRow = 0;
            ultraGridBand1.IndentationGroupByRowExpansionIndicator = 0;
            ultraGridBand1.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.alertsGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.alertsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alertsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alertsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.alertsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alertsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alertsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alertsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alertsGrid.DisplayLayout.Override.CellAppearance = appearance6;
            this.alertsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alertsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alertsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alertsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.alertsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.alertsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Center";
            this.alertsGrid.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.alertsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.alertsGrid.DisplayLayout.Override.RowAppearance = appearance9;
            this.alertsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.alertsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alertsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.ExtendedAutoDrag;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alertsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.alertsGrid.DisplayLayout.Override.WrapHeaderText = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alertsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList1.Key = "CheckBoxes";
            appearance11.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem1.Appearance = appearance11;
            valueListItem1.DataValue = false;
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueListItem2.Appearance = appearance12;
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.alertsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.alertsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alertsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alertsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alertsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsGrid.Location = new System.Drawing.Point(0, 0);
            this.alertsGrid.Name = "alertsGrid";
            this.alertsGrid.Size = new System.Drawing.Size(520, 205);
            this.alertsGrid.TabIndex = 5;
            this.alertsGrid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.alertsGrid.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.alertsGrid_InitializeRow);
            this.alertsGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.alertsGrid_MouseClick);
            // 
            // alertConfigBindingSource
            // 
            this.alertConfigBindingSource.AllowNew = false;
            this.alertConfigBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Dialogs.AlertBaselineWizard.AlertBaselineEntry);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkAllButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 205);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 26);
            this.panel2.TabIndex = 6;
            // 
            // checkAllButton
            // 
            this.checkAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkAllButton.Location = new System.Drawing.Point(1, 4);
            this.checkAllButton.Name = "checkAllButton";
            this.checkAllButton.Size = new System.Drawing.Size(75, 23);
            this.checkAllButton.TabIndex = 0;
            this.checkAllButton.Text = "Check All";
            this.checkAllButton.UseVisualStyleBackColor = true;
            this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.loadingCircle1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 231);
            this.panel1.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Please wait...";
            // 
            // loadingCircle1
            // 
            this.loadingCircle1.Active = false;
            this.loadingCircle1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadingCircle1.BackColor = System.Drawing.SystemColors.Control;
            this.loadingCircle1.Color = System.Drawing.Color.Silver;
            this.loadingCircle1.InnerCircleRadius = 12;
            this.loadingCircle1.Location = new System.Drawing.Point(195, 80);
            this.loadingCircle1.Name = "loadingCircle1";
            this.loadingCircle1.NumberSpoke = 40;
            this.loadingCircle1.OuterCircleRadius = 15;
            this.loadingCircle1.RotationSpeed = 20;
            this.loadingCircle1.Size = new System.Drawing.Size(55, 48);
            this.loadingCircle1.SpokeThickness = 5;
            this.loadingCircle1.TabIndex = 0;
            this.loadingCircle1.Text = "loadingCircle1";
            // 
            // contentStackPanel
            // 
            this.contentStackPanel.ActiveControl = this.wizard1;
            this.contentStackPanel.Controls.Add(this.wizard1);
            this.contentStackPanel.Controls.Add(this.wizardSelectionContentPanel);
            this.contentStackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentStackPanel.Location = new System.Drawing.Point(0, 0);
            this.contentStackPanel.Name = "contentStackPanel";
            this.contentStackPanel.Size = new System.Drawing.Size(542, 360);
            this.contentStackPanel.TabIndex = 1;
            // 
            // wizardSelectionContentPanel
            // 
            this.wizardSelectionContentPanel.BackColor = System.Drawing.Color.White;
            this.wizardSelectionContentPanel.BackColor2 = System.Drawing.Color.White;
            this.wizardSelectionContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.wizardSelectionContentPanel.Controls.Add(this.showAlertBaselineWizardButton);
            this.wizardSelectionContentPanel.Controls.Add(this.showBaselineWizardButton);
            this.wizardSelectionContentPanel.Controls.Add(this.footerPanel);
            this.wizardSelectionContentPanel.Controls.Add(this.headerPanel);
            this.wizardSelectionContentPanel.Location = new System.Drawing.Point(0, 0);
            this.wizardSelectionContentPanel.Name = "wizardSelectionContentPanel";
            this.wizardSelectionContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.wizardSelectionContentPanel.Size = new System.Drawing.Size(542, 360);
            this.wizardSelectionContentPanel.TabIndex = 0;
            // 
            // showAlertBaselineWizardButton
            // 
            this.showAlertBaselineWizardButton.DescriptionText = "bla - bla - bla";
            this.showAlertBaselineWizardButton.HeaderColor = System.Drawing.Color.Black;
            this.showAlertBaselineWizardButton.HeaderText = "Configure Alert Threshold From Baseline";
            this.showAlertBaselineWizardButton.Image = null;
            this.showAlertBaselineWizardButton.Location = new System.Drawing.Point(12, 226);
            this.showAlertBaselineWizardButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.showAlertBaselineWizardButton.Name = "showAlertBaselineWizardButton";
            this.showAlertBaselineWizardButton.Size = new System.Drawing.Size(518, 83);
            this.showAlertBaselineWizardButton.TabIndex = 5;
            this.showAlertBaselineWizardButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.showAlertBaselineWizardButton_MouseClick);
            // 
            // showBaselineWizardButton
            // 
            this.showBaselineWizardButton.DescriptionText = "bla - bla - bla";
            this.showBaselineWizardButton.HeaderColor = System.Drawing.Color.Black;
            this.showBaselineWizardButton.HeaderText = "Set A Baseline";
            this.showBaselineWizardButton.Image = null;
            this.showBaselineWizardButton.Location = new System.Drawing.Point(12, 126);
            this.showBaselineWizardButton.MinimumSize = new System.Drawing.Size(0, 40);
            this.showBaselineWizardButton.Name = "showBaselineWizardButton";
            this.showBaselineWizardButton.Size = new System.Drawing.Size(518, 87);
            this.showBaselineWizardButton.TabIndex = 4;
            this.showBaselineWizardButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.showBaselineWizardButton_MouseClick);
            // 
            // footerPanel
            // 
            this.footerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GettingStartedWizardFooter;
            this.footerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(1, 315);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(540, 44);
            this.footerPanel.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(442, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Done";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // headerPanel
            // 
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.GettingStartedWizardHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.headerPanel.Controls.Add(this.dividerLabel1);
            this.headerPanel.Controls.Add(this.productDescriptionLabel);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(1, 1);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(540, 115);
            this.headerPanel.TabIndex = 2;
            // 
            // dividerLabel1
            // 
            this.dividerLabel1.BackColor = System.Drawing.Color.Transparent;
            this.dividerLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dividerLabel1.Location = new System.Drawing.Point(120, 45);
            this.dividerLabel1.Name = "dividerLabel1";
            this.dividerLabel1.Size = new System.Drawing.Size(440, 2);
            this.dividerLabel1.TabIndex = 9;
            // 
            // productDescriptionLabel
            // 
            this.productDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.productDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productDescriptionLabel.Location = new System.Drawing.Point(117, 54);
            this.productDescriptionLabel.Name = "productDescriptionLabel";
            this.productDescriptionLabel.Size = new System.Drawing.Size(412, 54);
            this.productDescriptionLabel.TabIndex = 1;
            this.productDescriptionLabel.Text = "This here is fer configurin alerts when you are too lazy or ignerent to set em yu" +
                "rself.";
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.BackColor = System.Drawing.Color.Transparent;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.headerLabel.Location = new System.Drawing.Point(116, 14);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(113, 24);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "Welcome :-)";
            // 
            // AlertBaselineWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 360);
            this.Controls.Add(this.contentStackPanel);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlertBaselineWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Baseline Wizard";
            this.Load += new System.EventHandler(this.AlertBaselineWizard_Load);
            this.wizard1.ResumeLayout(false);
            this.introductionPage.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.configPage.ResumeLayout(false);
            this.configPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.criticalPercentageEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningPercentageEditor)).EndInit();
            this.metricSelectionPage.ResumeLayout(false);
            this.stackLayoutPanel1.ResumeLayout(false);
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertConfigBindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contentStackPanel.ResumeLayout(false);
            this.wizardSelectionContentPanel.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Divelements.WizardFramework.Wizard wizard1;
        private Divelements.WizardFramework.IntroductionPage introductionPage;
        private Divelements.WizardFramework.WizardPage metricSelectionPage;
        private Divelements.WizardFramework.WizardPage configPage;
        private System.Windows.Forms.NumericUpDown criticalPercentageEditor;
        private System.Windows.Forms.NumericUpDown warningPercentageEditor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Divelements.WizardFramework.InformationBox informationBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid alertsGrid;
        private System.Windows.Forms.BindingSource alertConfigBindingSource;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private MRG.Controls.UI.LoadingCircle loadingCircle1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button checkAllButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button baselineWizardButton;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel contentStackPanel;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel wizardSelectionContentPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label dividerLabel1;
        private System.Windows.Forms.Label productDescriptionLabel;
        private System.Windows.Forms.Label headerLabel;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton showAlertBaselineWizardButton;
        private Idera.SQLdm.DesktopClient.Controls.FeatureButton showBaselineWizardButton;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Button cancelButton;
    }
}