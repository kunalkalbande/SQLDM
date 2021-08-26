using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    partial class AnalysisConfigurationPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //start For Category Tree
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Disaster Recovery");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Index Optimization");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Query Optimization");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Disk");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Memory");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Network");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Processor");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Resources", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Database Configuration");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Server Configuration");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Security");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Configuration", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Deadlocks");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Blocking Processes");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Long Running Jobs");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Open Transactions");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Wait Stats");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Workload", new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode17});
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("All categories", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode8,
            treeNode12,
            treeNode18});
            //end For Category Tree

            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton5 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            Infragistics.Win.UltraWinEditors.DropDownEditorButton dropDownEditorButton6 = new Infragistics.Win.UltraWinEditors.DropDownEditorButton("DropDownList");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaselineConfigurationPage));
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton5 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton6 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage();
            this.analysisMainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertiesHeaderStripGeneralSettings = new PropertiesHeaderStrip();
            this.productionServerCheckBox = new CheckBox();
            this.onlineTranscationProcessingCheckBox = new CheckBox();
            this.btnAdvancedSettingButton = new Button();
            this.propertiesHeaderStripSelectCategory = new PropertiesHeaderStrip();
            this.labelCategoryTree = new System.Windows.Forms.Label();
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.timeDateContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.beginTimeCombo = new Idera.SQLdm.Common.UI.Controls.TimeComboEditor();
            this.label4 = new System.Windows.Forms.Label();
            this.NumericAnalysisDuration = new System.Windows.Forms.NumericUpDown();
            this.daysChkContainer = new System.Windows.Forms.TableLayoutPanel();
            this.sundayCheckbox = new System.Windows.Forms.CheckBox();
            this.mondayCheckBox = new System.Windows.Forms.CheckBox();
            this.tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this.fridayCheckBox = new System.Windows.Forms.CheckBox();
            this.saturdayCheckBox = new System.Windows.Forms.CheckBox();
            this.schedulingStatusCheckBox = new CheckBox();// SQLDm 10.0 - Praveen Suhalka - Scheduling status
            this.customDateMainContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.propertiesHeaderScheduling = new Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TreeViewCategories = new System.Windows.Forms.TreeView();
            this.analysisMainContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.timeDateContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericAnalysisDuration)).BeginInit();
            this.daysChkContainer.SuspendLayout();
            this.customDateMainContainer.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
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
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(503, 632);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 2;
            this.office2007PropertyPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Analyze32;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(0, 0);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(505, 689);
            this.office2007PropertyPage1.TabIndex = 0;
            this.office2007PropertyPage1.Text = "Setup Analysis Categories and Options.";
            // 
            // baselineMainContainer
            // 
            this.analysisMainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.analysisMainContainer.BackColor = System.Drawing.Color.White;
            this.analysisMainContainer.ColumnCount = 4;
            this.analysisMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.analysisMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.analysisMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.analysisMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analysisMainContainer.Controls.Add(this.panel1, 0, 0);
            this.analysisMainContainer.Controls.Add(this.timeDateContainer, 3, 8);
            this.analysisMainContainer.Controls.Add(this.daysChkContainer, 2, 7);
            this.analysisMainContainer.Controls.Add(this.schedulingStatusCheckBox, 9, 6);
            this.analysisMainContainer.Controls.Add(this.productionServerCheckBox, 9, 2);
            this.analysisMainContainer.Controls.Add(this.onlineTranscationProcessingCheckBox, 9, 3);
            this.analysisMainContainer.Controls.Add(this.panel2, 0, 5);
            this.analysisMainContainer.Controls.Add(this.panel3, 0, 9);
            this.analysisMainContainer.Controls.Add(this.TreeViewCategories, 9, 10);
            this.analysisMainContainer.Controls.Add(this.labelCategoryTree, 9, 12);
            this.analysisMainContainer.Controls.Add(this.btnAdvancedSettingButton,11, 13);
            this.analysisMainContainer.Location = new System.Drawing.Point(5, 54);
            this.analysisMainContainer.Name = "baselineMainContainer";
            this.analysisMainContainer.RowCount = 10;
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.analysisMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.analysisMainContainer.Size = new System.Drawing.Size(495, 615);
            this.analysisMainContainer.TabIndex = 65;
            // 
            // panel1
            // 
            this.analysisMainContainer.SetColumnSpan(this.panel1, 4);

            this.panel1.Controls.Add(this.propertiesHeaderStripGeneralSettings);
            
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 25);
            this.panel1.TabIndex = 0;
            // 
            // propertiesHeaderStripAnalysisConfiguration
            // 
            this.propertiesHeaderStripGeneralSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStripGeneralSettings.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStripGeneralSettings.Name = "propertiesHeaderStripGeneralSettings";
            this.propertiesHeaderStripGeneralSettings.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderStripGeneralSettings.TabIndex = 14;
            this.propertiesHeaderStripGeneralSettings.Text = "Analysis Options";
            this.propertiesHeaderStripGeneralSettings.WordWrap = false;
            // 
            // productionServerCheckBox
            // 
            this.productionServerCheckBox.AutoSize = true;
            this.productionServerCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.productionServerCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productionServerCheckBox.Location = new System.Drawing.Point(38, 67);
            this.productionServerCheckBox.Name = "enableQueryMonitorTraceCheckBox";
            this.productionServerCheckBox.Size = new System.Drawing.Size(146, 20);
            this.productionServerCheckBox.TabIndex = 0;
            this.productionServerCheckBox.Text = "Production Server";
            this.productionServerCheckBox.UseVisualStyleBackColor = false;
            this.productionServerCheckBox.CheckedChanged += new System.EventHandler(this.productionServerCheckBox_CheckedChanged);
            // 
            // onlineTranscationProcessingCheckBox
            // 
            this.onlineTranscationProcessingCheckBox.AutoSize = true;
            this.onlineTranscationProcessingCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.onlineTranscationProcessingCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onlineTranscationProcessingCheckBox.Location = new System.Drawing.Point(38, 67);
            this.onlineTranscationProcessingCheckBox.Name = "enableQueryMonitorTraceCheckBox";
            this.onlineTranscationProcessingCheckBox.Size = new System.Drawing.Size(146, 20);
            this.onlineTranscationProcessingCheckBox.TabIndex = 1;
            this.onlineTranscationProcessingCheckBox.Text = "Online Transaction Processing";
            this.onlineTranscationProcessingCheckBox.UseVisualStyleBackColor = false;
            this.onlineTranscationProcessingCheckBox.CheckedChanged += new System.EventHandler(this.onlineTranscationProcessingCheckBox_CheckedChanged);
            // 
            // AdvancedConnectionStringButton
            // 
            this.btnAdvancedSettingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvancedSettingButton.AutoSize = true;
            this.btnAdvancedSettingButton.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdvancedSettingButton.Location = new System.Drawing.Point(4, 456);
            this.btnAdvancedSettingButton.Name = "AdvancedSettingButton";
            this.btnAdvancedSettingButton.Size = new System.Drawing.Size(75, 23);
            this.btnAdvancedSettingButton.TabIndex = 30;
            this.btnAdvancedSettingButton.Text = "Advanced Settings";
            this.btnAdvancedSettingButton.UseVisualStyleBackColor = false;
            this.btnAdvancedSettingButton.Click += new System.EventHandler(this.btnAdvanceSetting_Click);
              
            
            // 
            // headerStrip1
            // 
            this.headerStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerStrip1.ForeColor = System.Drawing.Color.Black;
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Size = new System.Drawing.Size(489, 25);
            this.headerStrip1.TabIndex = 37;
            this.headerStrip1.Text = "What date range should be used to calculate the performance baseline for this ser" +
                "ver?";
            // 
            // timeDateContainer
            // 
            this.timeDateContainer.Controls.Add(this.label3);
            this.timeDateContainer.Controls.Add(this.beginTimeCombo);
            this.timeDateContainer.Controls.Add(this.label4);
            this.timeDateContainer.Controls.Add(this.NumericAnalysisDuration);
            this.timeDateContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeDateContainer.Location = new System.Drawing.Point(83, 242);
            this.timeDateContainer.Name = "timeDateContainer";
            this.timeDateContainer.Size = new System.Drawing.Size(409, 40);
            this.timeDateContainer.TabIndex = 64;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 55;
            this.label3.Text = "Time ";
            //for Scheduled Analysis:";
            // 
            // beginTimeCombo
            // 
            this.beginTimeCombo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.beginTimeCombo.AutoFillTime = Infragistics.Win.UltraWinMaskedEdit.AutoFillTime.Midnight;
            dropDownEditorButton5.Key = "DropDownList";
            dropDownEditorButton5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.beginTimeCombo.ButtonsRight.Add(dropDownEditorButton5);
            this.beginTimeCombo.DateTime = new System.DateTime(2008, 8, 1, 8, 0, 0, 0);
            this.beginTimeCombo.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.beginTimeCombo.ListInterval = System.TimeSpan.Parse("00:30:00");
            this.beginTimeCombo.Location = new System.Drawing.Point(42, 5);
            this.beginTimeCombo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.beginTimeCombo.MaskInput = "{time}";
            this.beginTimeCombo.Name = "beginTimeCombo";
            this.beginTimeCombo.Nullable = false;
            this.beginTimeCombo.Size = new System.Drawing.Size(88, 21);
            this.beginTimeCombo.TabIndex = 56;
            this.beginTimeCombo.Time = System.TimeSpan.Parse("00:00:00");
            this.beginTimeCombo.Value = new System.DateTime(2008, 8, 1, 0, 0, 0, 0);
            this.beginTimeCombo.ValueChanged += new System.EventHandler(this.Update);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 8);
            this.label4.TabIndex = 57;
            this.label4.Text = "Duration (in minutes)";
            //in Minutes (1 - 500):";
            
            // 
            // NumericAnalysisDuration
            // 
            this.NumericAnalysisDuration.Location = new System.Drawing.Point(165, 5);
            this.NumericAnalysisDuration.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.NumericAnalysisDuration.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NumericAnalysisDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericAnalysisDuration.Name = "NumericAnalysisDuration";
            this.NumericAnalysisDuration.Width = 50;
            this.NumericAnalysisDuration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            this.NumericAnalysisDuration.TabIndex = 58;
            this.NumericAnalysisDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericAnalysisDuration.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // schedulingStatusCheckBox
            // 
            this.schedulingStatusCheckBox.AutoSize = true;
            this.schedulingStatusCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.schedulingStatusCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schedulingStatusCheckBox.Location = new System.Drawing.Point(38, 67);
            this.schedulingStatusCheckBox.Name = "schedulingStatusCheckBox";
            this.schedulingStatusCheckBox.Size = new System.Drawing.Size(146, 20);
            this.schedulingStatusCheckBox.TabIndex = 0;
            this.schedulingStatusCheckBox.Text = "Scheduling Enabled";
            this.schedulingStatusCheckBox.UseVisualStyleBackColor = false;
            this.schedulingStatusCheckBox.CheckedChanged += new System.EventHandler(this.schedulingStatusCheckBox_CheckedChanged);
            // End:SQLDm 10.0 - Praveen Suhalka - Scheduling status
            // 
            // daysChkContainer
            // 
            this.daysChkContainer.ColumnCount = 7;
            this.analysisMainContainer.SetColumnSpan(this.daysChkContainer, 2);
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.daysChkContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.daysChkContainer.Controls.Add(this.sundayCheckbox, 0, 0);
            this.daysChkContainer.Controls.Add(this.mondayCheckBox, 1, 0);
            this.daysChkContainer.Controls.Add(this.tuesdayCheckBox, 2, 0);
            this.daysChkContainer.Controls.Add(this.wednesdayCheckBox, 3, 0);
            this.daysChkContainer.Controls.Add(this.thursdayCheckBox, 4, 0);
            this.daysChkContainer.Controls.Add(this.fridayCheckBox, 5, 0);
            this.daysChkContainer.Controls.Add(this.saturdayCheckBox, 6, 0);
            this.daysChkContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.daysChkContainer.Location = new System.Drawing.Point(33, 208);
            this.daysChkContainer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.daysChkContainer.Name = "daysChkContainer";
            this.daysChkContainer.RowCount = 1;
            this.daysChkContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.daysChkContainer.Size = new System.Drawing.Size(459, 26);
            this.daysChkContainer.TabIndex = 63;
            // 
            // sundayCheckbox
            // 
            this.sundayCheckbox.AutoSize = true;
            this.sundayCheckbox.Checked = false;
            this.sundayCheckbox.Location = new System.Drawing.Point(3, 3);
            this.sundayCheckbox.Name = "sundayCheckbox";
            this.sundayCheckbox.Size = new System.Drawing.Size(45, 17);
            this.sundayCheckbox.TabIndex = 47;
            this.sundayCheckbox.Text = "Sun";
            this.sundayCheckbox.UseVisualStyleBackColor = true;
            this.sundayCheckbox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // mondayCheckBox
            // 
            this.mondayCheckBox.AutoSize = true;
            this.mondayCheckBox.Checked = false;
            this.mondayCheckBox.Location = new System.Drawing.Point(54, 3);
            this.mondayCheckBox.Name = "mondayCheckBox";
            this.mondayCheckBox.Size = new System.Drawing.Size(47, 17);
            this.mondayCheckBox.TabIndex = 46;
            this.mondayCheckBox.Text = "Mon";
            this.mondayCheckBox.UseVisualStyleBackColor = true;
            this.mondayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // tuesdayCheckBox
            // 
            this.tuesdayCheckBox.AutoSize = true;
            this.tuesdayCheckBox.Checked = false;
            this.tuesdayCheckBox.Location = new System.Drawing.Point(107, 3);
            this.tuesdayCheckBox.Name = "tuesdayCheckBox";
            this.tuesdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.tuesdayCheckBox.TabIndex = 48;
            this.tuesdayCheckBox.Text = "Tue";
            this.tuesdayCheckBox.UseVisualStyleBackColor = true;
            this.tuesdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // wednesdayCheckBox
            // 
            this.wednesdayCheckBox.AutoSize = true;
            this.wednesdayCheckBox.Checked = false;
            this.wednesdayCheckBox.Location = new System.Drawing.Point(158, 3);
            this.wednesdayCheckBox.Name = "wednesdayCheckBox";
            this.wednesdayCheckBox.Size = new System.Drawing.Size(49, 17);
            this.wednesdayCheckBox.TabIndex = 49;
            this.wednesdayCheckBox.Text = "Wed";
            this.wednesdayCheckBox.UseVisualStyleBackColor = true;
            this.wednesdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // thursdayCheckBox
            // 
            this.thursdayCheckBox.AutoSize = true;
            this.thursdayCheckBox.Checked = false;
            this.thursdayCheckBox.Location = new System.Drawing.Point(213, 3);
            this.thursdayCheckBox.Name = "thursdayCheckBox";
            this.thursdayCheckBox.Size = new System.Drawing.Size(45, 17);
            this.thursdayCheckBox.TabIndex = 50;
            this.thursdayCheckBox.Text = "Thu";
            this.thursdayCheckBox.UseVisualStyleBackColor = true;
            this.thursdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // fridayCheckBox
            // 
            this.fridayCheckBox.AutoSize = true;
            this.fridayCheckBox.Checked = false;
            this.fridayCheckBox.Location = new System.Drawing.Point(264, 3);
            this.fridayCheckBox.Name = "fridayCheckBox";
            this.fridayCheckBox.Size = new System.Drawing.Size(37, 17);
            this.fridayCheckBox.TabIndex = 51;
            this.fridayCheckBox.Text = "Fri";
            this.fridayCheckBox.UseVisualStyleBackColor = true;
            this.fridayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            // 
            // saturdayCheckBox
            // 
            this.saturdayCheckBox.AutoSize = true;
            this.saturdayCheckBox.CheckedChanged += new System.EventHandler(this.Update);
            this.saturdayCheckBox.Checked = false;
            this.saturdayCheckBox.Location = new System.Drawing.Point(307, 3);
            this.saturdayCheckBox.Name = "saturdayCheckBox";
            this.saturdayCheckBox.Size = new System.Drawing.Size(42, 17);
            this.saturdayCheckBox.TabIndex = 52;
            this.saturdayCheckBox.Text = "Sat";
            this.saturdayCheckBox.UseVisualStyleBackColor = true;
            // 
            // customDateMainContainer
            // 
            this.customDateMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customDateMainContainer.Location = new System.Drawing.Point(83, 131);
            this.customDateMainContainer.Name = "customDateMainContainer";
            this.customDateMainContainer.Size = new System.Drawing.Size(409, 40);
            this.customDateMainContainer.TabIndex = 62;
            // 
            // panel2
            // 
            this.analysisMainContainer.SetColumnSpan(this.panel2, 4);
            this.panel2.Controls.Add(this.propertiesHeaderScheduling);
            //this.panel2.Controls.Add(this.propertiesHeaderStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 177);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(489, 25);
            this.panel2.TabIndex = 63;
            // 
            // header2label
            // 
            this.propertiesHeaderScheduling.AutoSize = true;
            this.propertiesHeaderScheduling.BackColor = System.Drawing.SystemColors.Control;
            this.propertiesHeaderScheduling.Location = new System.Drawing.Point(0, 6);
            this.propertiesHeaderScheduling.Name = "propertiesHeaderScheduling";
            this.propertiesHeaderScheduling.Size = new System.Drawing.Size(489, 25);
            this.propertiesHeaderScheduling.TabIndex = 60;
            this.propertiesHeaderScheduling.Text = "Scheduling (Choose days, time and duration to run Analysis on this server.)";
            this.propertiesHeaderScheduling.Visible = true;
            // 
            // panel3
            // 
            this.analysisMainContainer.SetColumnSpan(this.panel3, 4);
            this.panel3.Controls.Add(this.propertiesHeaderStripSelectCategory);
            //this.panel3.Controls.Add(this.labelCategoryTree);
            
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 288);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(489, 25);
            this.panel3.TabIndex = 65;

            // 
            // TreeViewCategories
            // 
            this.TreeViewCategories.CheckBoxes = true;
            this.TreeViewCategories.Location = new System.Drawing.Point(3, 3);
            this.TreeViewCategories.Name = "TreeViewCategories";
            treeNode1.Checked = true;
            treeNode1.Name = "treeNodeDisasterRecovery";
            treeNode1.Text = "Disaster Recovery";
            treeNode2.Checked = true;
            treeNode2.Name = "treeNodeIndexOptimization";
            treeNode2.Text = "Index Optimization";
            treeNode3.Checked = true;
            treeNode3.Name = "treeNodeQueryOptimization";
            treeNode3.Text = "Query Optimization";
            treeNode4.Checked = true;
            treeNode4.Name = "treeNodeDisk";
            treeNode4.Text = "Disk";
            treeNode5.Checked = true;
            treeNode5.Name = "treeNodeMemory";
            treeNode5.Text = "Memory";
            treeNode6.Checked = true;
            treeNode6.Name = "treeNodeNetwork";
            treeNode6.Text = "Network";
            treeNode7.Checked = true;
            treeNode7.Name = "treeNodeProcessor";
            treeNode7.Text = "Processor";
            treeNode8.Checked = true;
            treeNode8.Name = "treeNodeResources";
            treeNode8.Text = "Resources";
            treeNode9.Checked = true;
            treeNode9.Name = "treeNodeDatabaseConfiguration";
            treeNode9.Text = "Database Configuration";
            treeNode10.Checked = true;
            treeNode10.Name = "treeNodeServerConfiguration";
            treeNode10.Text = "Server Configuration";
            treeNode11.Checked = true;
            treeNode11.Name = "treeNodeSecurity";
            treeNode11.Text = "Security";
            treeNode12.Checked = true;
            treeNode12.Name = "treeNodeConfiguration";
            treeNode12.Text = "Configuration";
            treeNode13.Checked = true;
            treeNode13.Name = "treeNodeDeadlocks";
            treeNode13.Text = "Deadlocks";
            treeNode14.Checked = true;
            treeNode14.Name = "treeNodeBlockingProcesses";
            treeNode14.Text = "Blocking Processes";
            treeNode15.Checked = true;
            treeNode15.Name = "treeNodeLongRunningJobs";
            treeNode15.Text = "Long Running Jobs";
            treeNode16.Checked = true;
            treeNode16.Name = "treeNodeOpenTransactions";
            treeNode16.Text = "Open Transactions";
            treeNode17.Checked = true;
            treeNode17.Name = "treeNodeWaitStats";
            treeNode17.Text = "Wait Stats";
            treeNode18.Checked = true;
            treeNode18.Name = "treeNodeWorkload";
            treeNode18.Text = "Workload";
            treeNode19.Checked = true;
            treeNode19.Name = "treeNodeAllCategories";
            treeNode19.Text = "All categories";
            this.TreeViewCategories.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode19});
            this.TreeViewCategories.Size = new System.Drawing.Size(258, 310);
            this.TreeViewCategories.TabIndex = 0;
            this.TreeViewCategories.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.node_AfterCheck);
            this.TreeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewCategories_AfterSelect);
            // 
            // propertiesHeaderStripSelectCategory
            // 
            this.propertiesHeaderStripSelectCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesHeaderStripSelectCategory.Location = new System.Drawing.Point(3, 3);
            this.propertiesHeaderStripSelectCategory.Name = "propertiesHeaderStripGeneralSettings";
            this.propertiesHeaderStripSelectCategory.Size = new System.Drawing.Size(489, 35);
            this.propertiesHeaderStripSelectCategory.TabIndex = 14;
            this.propertiesHeaderStripSelectCategory.Text = "Select Categories";
            this.propertiesHeaderStripSelectCategory.WordWrap = false;
            // 
            // labelCategoryTree
            // 
            this.labelCategoryTree.AutoSize = false;
            this.labelCategoryTree.BackColor = System.Drawing.Color.Transparent;
            this.labelCategoryTree.Location = new System.Drawing.Point(11, 11);
            this.labelCategoryTree.Name = "labelCategoryTree";
            this.labelCategoryTree.Size = new System.Drawing.Size(400, 26);
            this.labelCategoryTree.TabIndex = 3;
            this.labelCategoryTree.Text = "";
            // 
            // BaselineConfigurationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.analysisMainContainer);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Name = "AnalysisConfigurationPage";
            this.Size = new System.Drawing.Size(505, 689);
            this.analysisMainContainer.ResumeLayout(false);
            this.analysisMainContainer.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.timeDateContainer.ResumeLayout(false);
            this.timeDateContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.beginTimeCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericAnalysisDuration)).EndInit();
            this.daysChkContainer.ResumeLayout(false);
            this.daysChkContainer.PerformLayout();
            this.customDateMainContainer.ResumeLayout(false);
            this.customDateMainContainer.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Office2007PropertyPage office2007PropertyPage1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NumericAnalysisDuration;
        private Common.UI.Controls.TimeComboEditor beginTimeCombo;
        private System.Windows.Forms.CheckBox saturdayCheckBox;
        private System.Windows.Forms.CheckBox fridayCheckBox;
        private System.Windows.Forms.CheckBox thursdayCheckBox;
        private System.Windows.Forms.CheckBox wednesdayCheckBox;
        private System.Windows.Forms.CheckBox tuesdayCheckBox;
        private System.Windows.Forms.CheckBox sundayCheckbox;
        private System.Windows.Forms.CheckBox mondayCheckBox;
        private PropertiesHeaderStrip headerStrip1;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStripSelectCategory;
        private System.Windows.Forms.Label labelCategoryTree;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderScheduling;
        private Idera.SQLdm.DesktopClient.Controls.PropertiesHeaderStrip propertiesHeaderStripGeneralSettings;
        private System.Windows.Forms.CheckBox productionServerCheckBox;
        private System.Windows.Forms.CheckBox onlineTranscationProcessingCheckBox;
        private Button btnAdvancedSettingButton;
        private System.Windows.Forms.FlowLayoutPanel customDateMainContainer;
        private System.Windows.Forms.TableLayoutPanel daysChkContainer;
        private System.Windows.Forms.FlowLayoutPanel timeDateContainer;
        private System.Windows.Forms.TableLayoutPanel analysisMainContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TreeView TreeViewCategories;
        private System.Windows.Forms.CheckBox schedulingStatusCheckBox;// SQLDm 10.0 - Praveen Suhalka - Scheduling status
    }
}
